using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Datasets.PartsDatasetTableAdapters;
using DWOS.Data.Datasets.ProcessesDatasetTableAdapters;
using DWOS.Data.Datasets.QuoteDataSetTableAdapters;
using DWOS.Data.Order;
using DWOS.Shared;
using DWOS.UI.Admin;
using DWOS.UI.Sales.Order;
using DWOS.UI.Utilities;
using DWOS.Utilities.Validation;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.UltraWinToolTip;
using NLog;
using OrderFeeTypeTableAdapter = DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderFeeTypeTableAdapter;

namespace DWOS.UI.Sales
{
    public partial class OrderInformation : DataPanel
    {
        #region Fields

        private CustomersDataset.Customer_FieldsDataTable _customerFields = new CustomersDataset.Customer_FieldsDataTable();

        private int _customerID;

        private ErrorProvider _errProvider;
        private Dictionary <string, string> _fieldDefaultValueCache = new Dictionary <string, string>();
        private List <string> _filesToDelete;
        private bool _isInPartEditor;
        private bool _newOrderBeingCreated;
        private bool _autoUpdateEstShipDate;
        private Lazy<ILeadTimeScheduler> _schedulerInstance = new Lazy<ILeadTimeScheduler>(NewSchedulerInstance);
        private PartSummaryTableAdapter _taPartSummary;
        private DWOS.Utilities.Validation.ValidatorManager _validationManager;
        private Data.Order.IPriceUnitPersistence _priceUnitPersistence = new Data.Order.PriceUnitPersistence();
        private DataView _priceUnitDataView;
        private int _OrderID;

        public event Action <int> BeforeCustomerChanged;
        public event Action <OrderChildRowType, DataRow> AfterChildRowAdded;
        public event Action <OrderChildRowType, DataRow> BeforeChildRowDeleted;

        /// <summary>
        ///     Occurs when quick filter is clicked, which allows filtering orders by the currents orders specified value.
        ///     string is Filter Value
        /// </summary>
        public event Action <OrderSearchField, string> QuickFilter;

        /// <summary>
        ///     Occurs when the parts are cleared and then reloaded.
        /// </summary>
        public event Action PartsReloaded;

        private readonly Lazy<ApplicationSettingsDataSet.FieldsRow> _adjustedDateFieldLazy = new Lazy<ApplicationSettingsDataSet.FieldsRow>(
            () =>
            {
                var fields = GetFieldSettings();

                var adjustedDateField = fields.FirstOrDefault(f => f.Name == "Adjusted Est. Ship Date");
                return adjustedDateField;
            });

        private readonly Lazy<ApplicationSettingsDataSet.FieldsRow> _requiredDateFieldLazy = new Lazy<ApplicationSettingsDataSet.FieldsRow>(
            () =>
            {
                var fields = GetFieldSettings();

                var requiredDateField = fields.FirstOrDefault(f => f.Name == "Required Date");
                return requiredDateField;
            });

        private readonly Lazy<ApplicationSettingsDataSet.FieldsRow> _serialNumberFieldLazy = new Lazy<ApplicationSettingsDataSet.FieldsRow>(
            () =>
            {
                var fields = GetFieldSettings();
                var requiredDateField = fields.FirstOrDefault(f => f.Name == "Serial Number");
                return requiredDateField;

            });

        private readonly Lazy<ApplicationSettingsDataSet.FieldsRow> _productClassFieldLazy = new Lazy<ApplicationSettingsDataSet.FieldsRow>(
            () =>
            {
                var fields = GetFieldSettings();
                var productClassField = fields.FirstOrDefault(f => f.Name == "Product Class");
                return productClassField;
            });

        private readonly Lazy<ApplicationSettingsDataSet.FieldsRow> _containerFieldLazy = new Lazy<ApplicationSettingsDataSet.FieldsRow>(
            () =>
            {
                var fields = GetFieldSettings();
                var productClassField = fields.FirstOrDefault(f => f.Name == "Container");
                return productClassField;
            });

        #endregion

        #region Properties

        public OrdersDataSet Dataset
        {
            get { return base._dataset as OrdersDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.Order.OrderIDColumn.ColumnName; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool ClearPartSummaryBeforeFill
        {
            get
            {
                if (_taPartSummary == null)
                {
                    throw new InvalidOperationException("Cannot get before calling LoadData()");
                }

                return _taPartSummary.ClearBeforeFill;
            }
            set
            {
                if (_taPartSummary == null)
                {
                    throw new InvalidOperationException("Cannot set before calling LoadData()");
                }

                _taPartSummary.ClearBeforeFill = value;
            }
        }

        /// <summary>
        ///     Get or Set the current customer ID. This can be called by switching to a differnt order OR by the user selecting a different customer in dropdown.
        /// </summary>
        public int CurrentCustomerID
        {
            get { return this._customerID; }
            set
            {
                if(this._customerID != value)
                {
                    _log.Debug("Customer ID changed from {0} to {1}.", this._customerID, value);

                    if(BeforeCustomerChanged != null)
                        BeforeCustomerChanged(value);

                    this._customerID = value;
                    var currentOrder = CurrentRecord as OrdersDataSet.OrderRow;

                    var customerFilter = $"CustomerID = {value}";

                    //update filter's anytime the customer changes
                    base.UpdateFilter(this.cboPart, customerFilter);
                    UpdateShippingFilters();

                    CreateCustomFields();
                    FixMinimumSize();
                    UpdatePartItemsAppearance();
                    UpdateValidators(this._customerID);

                    //if the user changed the customer via the dropdown and we are not in the middle of a move to a new record then set customer specific fields
                    if(!_recordLoading)
                    {
                        SelectDefaultShippingMethod();
                        SelectDefaultShippingAddress();
                        UpdateEstimatedShipDate();
                        UpdateDefaultFieldsPerCustomer(this._customerID);
                        BindCustomFields(true);

                        chkRequireCoc.Checked = CurrentCustomer?.RequireCOCByDefault ?? false;
                    }
                }
            }
        }

        private OrdersDataSet.CustomerSummaryRow CurrentCustomer
        {
            get { return Dataset.CustomerSummary.FindByCustomerID(CurrentCustomerID); }
        }

        public UltraNumericEditor PartQty
        {
            get { return this.numPartQty; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this panel is view only. Should be set before LoadData is called.
        /// </summary>
        /// <value> <c>true</c> if [view only]; otherwise, <c>false</c> . </value>
        public bool ViewOnly { get; set; }

        /// <summary>
        ///     Gets a value indicating whether current user requires order review.
        /// </summary>
        /// <value> <c>true</c> if [user requires order review]; otherwise, <c>false</c> . </value>
        public static bool UserRequiresOrderReview
        {
            get { return SecurityManager.Current.CurrentUser == null || SecurityManager.Current.CurrentUser.RequireOrderReview; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether parts are being loaded.
        /// </summary>
        /// <value> <c>true</c> if [parts loading]; otherwise, <c>false</c> . </value>
        public bool PartsLoading { get; set; }

        public bool DisableCustomerSelection
        {
            get { return this.cboCustomer.ReadOnly; }
            set { this.cboCustomer.ReadOnly = value; }
        }

        public bool IsQuickView { get; set; } = false;

        private ISerialNumberWidget ActiveSerialNumberWidget
            => ApplicationSettings.Current.SerialNumberEditorType == SerialNumberType.Advanced
                ? (ISerialNumberWidget)advancedSerialEditor
                : (ISerialNumberWidget)basicSerialEditor;

        private OrdersDataSet.PartSummaryRow CurrentPart
        {
            get
            {
                if (cboPart.SelectedItem == null || cboPart.SelectedItem.DataValue == DBNull.Value)
                {
                    return null;
                }

                return Dataset.PartSummary.FindByPartID(Convert.ToInt32(cboPart.SelectedItem.DataValue));
            }
        }

        #endregion

        #region Methods

        public OrderInformation() { InitializeComponent(); }

        public void LoadData(OrdersDataSet dataset)
        {
            Dataset = dataset;
            this._taPartSummary = new PartSummaryTableAdapter();
            this._priceUnitDataView = new DataView(Dataset.PriceUnit);
            //_OrderID = Dataset.Order.OrderIDColumn.ColumnNam

            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.Order.TableName;
            Dataset.Order.ColumnChanged += Order_ColumnChanged;

            //bind column to control
            //base.BindValue(this.txtOrderID, Dataset.Order.OrderIDColumn.ColumnName);
            base.BindValue(this.txtPONumber, Dataset.Order.PurchaseOrderColumn.ColumnName, false);
            base.BindValue(this.dtOrderDate, Dataset.Order.OrderDateColumn.ColumnName, false);
            base.BindValue(this.dtOrderRequiredDate, Dataset.Order.RequiredDateColumn.ColumnName, false);
            base.BindValue(this.dtAdjustedEstShipDate, Dataset.Order.AdjustedEstShipDateColumn.ColumnName, false);
            base.BindValue(this.cboCustomer, Dataset.Order.CustomerIDColumn.ColumnName, true);
            base.BindValue(this.cboPriority, Dataset.Order.PriorityColumn.ColumnName, false);
            base.BindValue(this.txtInvoice, Dataset.Order.InvoiceColumn.ColumnName);
            base.BindValue(this.cboStatus, Dataset.Order.StatusColumn.ColumnName);
            base.BindValue(this.dtCompleted, Dataset.Order.CompletedDateColumn.ColumnName);
            base.BindValue(this.dtShipDate, Dataset.Order.EstShipDateColumn.ColumnName, false);
            base.BindValue(this.cboUserCreated, Dataset.Order.CreatedByColumn.ColumnName);
            base.BindValue(this.cboPart, Dataset.Order.PartIDColumn.ColumnName, false);
            base.BindValue(this.numPartQty, Dataset.Order.PartQuantityColumn.ColumnName, false);
            base.BindValue(this.numUnitPrice, Dataset.Order.BasePriceColumn.ColumnName, false);
            base.BindValue(this.numWeight, Dataset.Order.WeightColumn.ColumnName, false);
            base.BindValue(this.cboUnit, Dataset.Order.PriceUnitColumn.ColumnName, false);
            base.BindValue(this.chkContractReview, Dataset.Order.ContractReviewedColumn.ColumnName, true);
            base.BindValue(this.cboShippingMethod, Dataset.Order.ShippingMethodColumn.ColumnName, false);
            base.BindValue(this.cboShipTo, Dataset.Order.CustomerAddressIDColumn.ColumnName, false, DataSourceUpdateMode.Never);
            base.BindValue(this.txtCustomerWO, Dataset.Order.CustomerWOColumn.ColumnName, false);
            base.BindValue(this.curImportedPrice, Dataset.Order.ImportedPriceColumn.ColumnName, false);
            base.BindValue(this.chkRequireCoc, Dataset.Order.RequireCocColumn.ColumnName, false, DataSourceUpdateMode.OnPropertyChanged);

            var currencyMaskInput = "{currency:6." + ApplicationSettings.Current.PriceDecimalPlaces + "}";
            this.numUnitPrice.MaskInput = currencyMaskInput;
            this.numTotalCurrency.MaskInput = currencyMaskInput;
            this.curFeesTotal.MaskInput = currencyMaskInput;
            this.curImportedPrice.MaskInput = currencyMaskInput;

            this.numWeight.MaskInput = string.Format("nnnnnn.{0} lbs",
                string.Concat(Enumerable.Repeat("n", ApplicationSettings.Current.WeightDecimalPlaces)));

            //bind lists
            base.BindList(this.cboCustomer, Dataset.CustomerSummary, Dataset.CustomerSummary.CustomerIDColumn.ColumnName, Dataset.CustomerSummary.NameColumn.ColumnName);
            base.BindList(this.cboPriority, Dataset.d_Priority, Dataset.d_Priority.PriorityIDColumn.ColumnName, Dataset.d_Priority.PriorityIDColumn.ColumnName);
            base.BindList(this.cboStatus, Dataset.d_OrderStatus, Dataset.d_OrderStatus.OrderStatusIDColumn.ColumnName, Dataset.d_OrderStatus.OrderStatusIDColumn.ColumnName);
            base.BindList(this.cboUserCreated, Dataset.UserSummary, Dataset.UserSummary.UserIDColumn.ColumnName, Dataset.UserSummary.NameColumn.ColumnName);
            base.BindList(this.cboPart, Dataset.PartSummary, Dataset.PartSummary.PartIDColumn.ColumnName, Dataset.PartSummary.NameColumn.ColumnName);
            base.BindList(this.cboUnit, this._priceUnitDataView, Dataset.PriceUnit.PriceUnitIDColumn.ColumnName, Dataset.PriceUnit.DisplayNameColumn.ColumnName);
            base.BindList(this.cboShippingMethod, Dataset.CustomerShippingSummary, Dataset.CustomerShippingSummary.CustomerShippingIDColumn.ColumnName, Dataset.CustomerShippingSummary.NameColumn.ColumnName);
            base.BindList(this.cboShipTo, Dataset.CustomerAddress, Dataset.CustomerAddress.CustomerAddressIDColumn.ColumnName, Dataset.CustomerAddress.NameColumn.ColumnName);

            this.mediaWidget.Setup(new MediaWidget.SetupArgs()
            {
                MediaJunctionTable = Dataset.Order_Media,
                MediaTable = Dataset.Media,
                MediaJunctionTableParentRowColumn = Dataset.Order_Media.OrderIDColumn,
                AllowEditing = Editable,
                MediaLinkType = Documents.LinkType.WorkOrder,
                DocumentLinkTable = Dataset.Order_DocumentLink,
                ScannerSettingsType = ScannerSettingsType.Order
            });

            this.mediaWidget.FileAdded += mediaWidget_FileAdded;
            LoadStatusList();


            if (ApplicationSettings.Current.SerialNumberEditorType == SerialNumberType.Advanced)
            {
                pnlBasicSerialNumber.Visible = false;

                if (IsQuickView)
                {
                    // Also hide advanced serial editor - shown as tab in quick view
                    pnlAdvSerialNumber.Visible = false;
                }
            }
            else
            {
                pnlAdvSerialNumber.Visible = false;
            }

            ActiveSerialNumberWidget.LoadData(Dataset);
            productClassWidget.LoadData(Dataset, _customerFields);
            workDescriptionWidget.LoadData(Dataset);

            if(ViewOnly)
            {
                this.neDayUntilReq.ReadOnly = true;
                this.txtPONumber.ReadOnly = true;
                this.dtOrderDate.ReadOnly = true;
                this.dtOrderRequiredDate.ReadOnly = true;
                this.cboCustomer.ReadOnly = true;
                this.cboPriority.ReadOnly = true;
                this.dtShipDate.ReadOnly = true;
                this.cboPart.Enabled = false;
                this.numPartQty.ReadOnly = true;
                this.numWeight.ReadOnly = true;
                this.cboShippingMethod.ReadOnly = true;
                this.cboShipTo.ReadOnly = true;
                this.chkContractReview.Enabled = false;
                this.curFeesTotal.Enabled = false;
                this.numUnitPrice.ReadOnly = true;
                this.curImportedPrice.ReadOnly = true;
                this.cboUnit.ReadOnly = true;
                this.txtCustomerWO.ReadOnly = true;
                chkRequireCoc.Enabled = true;

                //Allow adding / deleting docs if permissions set
                if (IsQuickView && SecurityManager.Current.IsInRole("Documents"))
                    this.mediaWidget.AllowEditing = true;
                else
                    this.mediaWidget.AllowEditing = false;

                //Allow viewing of fees if permissions set
                if (IsQuickView && SecurityManager.Current.IsInRole("OrderEntry"))
                    this.curFeesTotal.Enabled = true;
                else
                    this.curFeesTotal.Enabled = false;

                this.picPriceHistory.Enabled = false;
                this.ActiveSerialNumberWidget.ReadOnly = true;
                this.productClassWidget.ReadOnly = true;
                this.workDescriptionWidget.ReadOnly = true;

                //check to see if user can see pricing
                if(!SecurityManager.Current.IsInRole("OrderEntry"))
                {
                    this.pnlPrice.Height = 0;
                    this.pnlPrice.Visible = false;
                }

                this.txtPONumber.ButtonsRight["btnFilter"].Visible = false;

                txtContainers.ButtonsRight["Edit"].Visible = false;
            }

            pnlRepairStatementFields.Visible = ApplicationSettings.Current.RepairStatementEnabled;

            //prevent the part from changing
            this.cboPart.PreventMouseWheelScrolling();

            // Prevent accidental scrolling of customers dropdown
            cboCustomer.PreventMouseWheelScrolling();

            //Change color of Inactive customers
            foreach(var customerItem in this.cboCustomer.Items)
            {
                if(customerItem.ListObject != null)
                {
                    var customerRow = ((DataRowView) customerItem.ListObject).Row as OrdersDataSet.CustomerSummaryRow;
                    if(customerRow != null && !customerRow.Active)
                    {
                        var inactiveAppearance = new Infragistics.Win.Appearance();
                        inactiveAppearance.ForeColor = Color.Orange;
                        customerItem.Appearance = inactiveAppearance;
                    }
                }
            }

            // Show sync/desync icon
            picQuantitySync.Visible = ApplicationSettings.Current.SyncQuantityAndWeightForOrders;
            picQuantityDesync.Visible = !ApplicationSettings.Current.SyncQuantityAndWeightForOrders;

            // Contract review text
            txtContractReview.Value = ApplicationSettings.Current.ContractReviewText;

            base._panelLoaded = true;
        }

        public override void AddValidators(DWOS.Utilities.Validation.ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(
                new TextControlValidator(cboUserCreated, "User name required.")
                {
                    ValidationRequired = () => IsNewRow ?? false
                }, errProvider));

            manager.Add(new ImageDisplayValidator(new CustomerValidator(this.cboCustomer, this), errProvider));
            manager.Add(new ImageDisplayValidator(new PartsDropDownControlValiditor(this.cboPart, this._taPartSummary, this), errProvider));
            manager.Add(new ImageDisplayValidator(new DateTimeValidator(this.dtOrderDate, "Order date required."), errProvider));
            manager.Add(new ImageDisplayValidator(new DateTimeValidator(this.dtShipDate, "Estimated ship date."), errProvider));
            manager.Add(new ImageDisplayValidator(new PartQtyValiditor(this.numPartQty, this), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboPriority, "Priority required."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboStatus, "Status required."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtPONumber, "PO Number required."), errProvider));
            manager.Add(new ImageDisplayValidator(new CheckEditorControlValidator(this.chkContractReview, true), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtCustomerWO, "Customer WO required."), errProvider));
            manager.Add(new ImageDisplayValidator(new DocumentValiditor(this.mediaWidget, this), errProvider));
            manager.Add(new ImageDisplayValidator(new PriceUnitValidator(this.cboUnit, this), errProvider));
            manager.Add(new ImageDisplayValidator(new UnitPriceValidator(this.numUnitPrice, this), errProvider));
            manager.Add(new ImageDisplayValidator(new FeesDiscountsValidator(curFeesTotal, this), errProvider));
            manager.Add(new ImageDisplayValidator(new OrderPriceValidator(this.numTotalCurrency, this), errProvider));

            var requiredDateField = _requiredDateFieldLazy.Value;

            if (requiredDateField == null || requiredDateField.IsRequired)
            {
                manager.Add(
                    new ImageDisplayValidator(
                        new DateTimeValidator(this.dtOrderRequiredDate, "Required date is required."), errProvider));
            }
            else if (!requiredDateField.IsVisible)
            {
                requiredDateLabel.Visible = false;
                dtOrderRequiredDate.Visible = false;
                neDayUntilReq.Visible = false;
                lbDaysUntilReq.Visible = false;
            }

            var adjustedEstShipDateField = _adjustedDateFieldLazy.Value;
            if (!adjustedEstShipDateField.IsVisible)
            {
                pnlAdjustedShipDate.Visible = false;
            }

            var serialNumberField = _serialNumberFieldLazy.Value;

            if (serialNumberField == null || serialNumberField.IsRequired)
            {
                ActiveSerialNumberWidget.AddValidators(manager, errProvider);
            }
            else if (!serialNumberField.IsVisible)
            {
                pnlBasicSerialNumber.Visible = false;
                pnlAdvSerialNumber.Visible = false;
            }

            var productClassField = _productClassFieldLazy.Value;

            if (productClassField == null || productClassField.IsRequired || productClassField.IsVisible)
            {
                productClassWidget.AddValidators(manager, errProvider);
            }
            else
            {
                pnlProductClass.Visible = false;
            }

            var containerField = _containerFieldLazy.Value;

            if (containerField == null || containerField.IsRequired)
            {
                var containerValidator = new ContainerValidator(txtContainers, this);
                manager.Add(new ImageDisplayValidator(containerValidator, errProvider));
            }
            else if (!containerField.IsVisible)
            {
                pnlContainers.Visible = false;
            }


            //if (ApplicationSettings.Current.COCEnabled)
            //{
                pnlRequireCoc.Visible = ApplicationSettings.Current.COCEnabled;
            //    pnlRequireCoc.Height = 0;
            //}

            this._errProvider = errProvider;
            this._validationManager = manager;
        }

        /// <summary>
        ///     Update the validators based on if they are required by the customer or not
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        private void UpdateValidators(int customerId)
        {
            const int customerWorkOrderFieldID = 1;
            const int purchaseOrderFieldID = 2;
            const int documentFieldID = 6;

            try
            {
                if (_validationManager == null || _customerFields == null)
                {
                    return;
                }

                if(this._customerFields.Count(cf => cf.CustomerID == customerId) < 1)
                {
                    _log.Debug("Loading customer fields for customer " + customerId);

                    using(var ta = new Customer_FieldsTableAdapter {ClearBeforeFill = false})
                        ta.FillBy(this._customerFields, customerId);
                }

                var customerWOValidator = this._validationManager.Find(this.txtCustomerWO);
                if(customerWOValidator != null)
                {
                    var customerWOField = this._customerFields.FirstOrDefault(r => r.CustomerID == customerId && r.FieldID == customerWorkOrderFieldID);
                    customerWOValidator.IsEnabled = customerWOField != null && customerWOField.Required;
                }

                var poValidator = this._validationManager.Find(this.txtPONumber);
                if(poValidator != null)
                {
                    var poField = this._customerFields.FirstOrDefault(r => r.CustomerID == customerId && r.FieldID == purchaseOrderFieldID);
                    poValidator.IsEnabled = poField != null && poField.Required;
                }

                var documentValidator = this._validationManager.Find(this.mediaWidget);
                if(documentValidator != null)
                {
                    var documentField = this._customerFields.FirstOrDefault(r => r.CustomerID == customerId && r.FieldID == documentFieldID);
                    documentValidator.IsEnabled = documentField != null && documentField.Required;
                }

                productClassWidget.RefreshValidators(_validationManager, customerId);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating validators.");
            }
        }

        public OrdersDataSet.OrderRow AddOrderRow(int? customerId = null)
        {
            this._newOrderBeingCreated = true;
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as OrdersDataSet.OrderRow;

            cr.OrderDate = DateTime.Now;
            cr.Status = Properties.Settings.Default.OrderStatusOpen;
            cr.Priority = Properties.Settings.Default.OrderPriorityDefault;
            cr.CreatedBy = SecurityManager.Current.UserID;
            cr.ContractReviewed = false;
            cr.PartQuantity = 0;
            cr.BasePrice = 0;
            // Leaving PartID null - user must fill-in later
            cr.Hold = false;
            cr.OrderType = (int) OrderType.Normal;
            cr.OriginalOrderType = (int) OrderType.Normal;

            //set to currently used customer id if already set else the first one
            if (Dataset.CustomerSummary.Any())
            {
                var customerRow = customerId.HasValue
                    ? Dataset.CustomerSummary.FindByCustomerID(customerId.Value)
                    : CurrentCustomer;

                cr.CustomerSummaryRow = customerRow ?? Dataset.CustomerSummary.First();
            }

            cr.WorkStatus = OrderControllerExtensions.GetNewOrderWorkStatus(cr.IsCustomerIDNull() ? 0 : cr.CustomerID, UserRequiresOrderReview);
            cr.CurrentLocation = ApplicationSettings.Current.DepartmentSales;
            cr.PurchaseOrder = cr.CustomerSummaryRow == null ? null : (GetDefaultFieldValue(cr.CustomerID, "PO"));
            cr.CustomerWO = cr.CustomerSummaryRow == null ? null : GetDefaultFieldValue(cr.CustomerSummaryRow.CustomerID, "Customer WO");


            var productClass = cr.CustomerSummaryRow == null ? null : GetDefaultFieldValue(cr.CustomerSummaryRow.CustomerID, "Product Class");

            if (!string.IsNullOrEmpty(productClass))
            {
                var productClassRow = Dataset.OrderProductClass.NewOrderProductClassRow();
                productClassRow.OrderRow = cr;
                productClassRow.ProductClass = productClass;
                Dataset.OrderProductClass.AddOrderProductClassRow(productClassRow);
            }

            var defaultShipDate = DateTime.Now.AddBusinessDays(cr.CustomerSummaryRow?.LeadTime ?? ApplicationSettings.Current.OrderLeadTime);

            if (_requiredDateFieldLazy.Value?.IsVisible ?? true)
            {
                cr.RequiredDate = defaultShipDate;
            }

            cr.EstShipDate = defaultShipDate;

            if(cr.CustomerSummaryRow != null)
            {
                // Set default shipping method and address.
                var shippingMethods = cr.CustomerSummaryRow
                    .GetCustomerShippingSummaryRows()
                    .Where(ship => ship.Active)
                    .ToList();

                //attempt to set to the default shipping method, if there is no default shipping method then just select first one
                cr.CustomerShippingSummaryRow = shippingMethods.FirstOrDefault(r => r.DefaultShippingMethod)
                    ?? shippingMethods.FirstOrDefault();

                var addresses = cr.CustomerSummaryRow
                    .GetCustomerAddressRows()
                    .Where(addr => addr.Active)
                    .ToList();

                cr.CustomerAddressRow = addresses.FirstOrDefault(addr => addr.IsDefault) ??
                    addresses.FirstOrDefault();

                // Set Require COC
                cr.RequireCoc = cr.CustomerSummaryRow.RequireCOCByDefault;
            }

            cr.PriceUnit = _priceUnitPersistence.DefaultPriceUnit().ToString();

            //if (ApplicationSettings.Current.ApplyDefaultFeesEnabled)
            //    OrderFeeTools.AddDefaultFees(ref cr);


            // Add default work description
            if (ApplicationSettings.Current.RepairStatementEnabled)
            {
                var defaultWorkDescription = Dataset.WorkDescription
                    .Where(desc => desc.IsDefault)
                    .OrderBy(desc => desc.Description)
                    .FirstOrDefault();

                if (defaultWorkDescription != null)
                {
                    Dataset.OrderWorkDescription.AddOrderWorkDescriptionRow(cr, defaultWorkDescription);
                }
            }

            return cr;
        }

        public OrdersDataSet.OrderRow AddOrderRowFromReceivingOrder(int receivingID)
        {
            _log.Info("Creating order from a receivingOrderID.");

            PartsDataset.ReceivingRow row = null;

            using(var ta = new ReceivingTableAdapter())
            {
                var dt = ta.GetByID(receivingID);

                if(dt != null && dt.Count == 1)
                    row = dt[0];
                else
                    return null;
            }

            _newOrderBeingCreated = true;
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as OrdersDataSet.OrderRow;
            cr.OrderDate = DateTime.Now;
            cr.Status = Properties.Settings.Default.OrderStatusOpen;

            cr.Priority = row.IsPriorityNull()
                ? Properties.Settings.Default.OrderPriorityDefault
                : row.Priority;

            cr.CreatedBy = SecurityManager.Current.UserID;
            cr.CustomerID = row.CustomerID;
            cr.ContractReviewed = false;
            cr.PartQuantity = row.PartQuantity;
            cr.PartID = row.PartID;
            cr.CurrentLocation = ApplicationSettings.Current.DepartmentSales;
            cr.WorkStatus = OrderControllerExtensions.GetNewOrderWorkStatus(cr.IsCustomerIDNull() ? 0 : cr.CustomerID, UserRequiresOrderReview);
            cr.BasePrice = 0;
            cr.ReceivingID = receivingID;
            cr.Hold = false;
            cr.OrderType = (int) OrderType.Normal;

            if(!row.IsPurchaseOrderNull())
                cr.PurchaseOrder = row.PurchaseOrder;
            if(!row.IsCustomerWONull())
                cr.CustomerWO = row.CustomerWO;

            // Set price unit
            var priceUnitEntry = this.Dataset.PriceUnit.FindByPriceUnitID(OrderPrice.enumPriceUnit.Each.ToString());

            if (priceUnitEntry != null && !priceUnitEntry.Active)
            {
                cr.PriceUnit = OrderPrice.enumPriceUnit.EachByWeight.ToString();
            }
            else
            {
                cr.PriceUnit = OrderPrice.enumPriceUnit.Each.ToString();
            }

            var csr = Dataset.CustomerSummary.FindByCustomerID(row.CustomerID);

            var defaultShipDate = DateTime.Now.AddBusinessDays(csr?.LeadTime ?? ApplicationSettings.Current.OrderLeadTime);

            if (_requiredDateFieldLazy.Value?.IsVisible ?? true)
            {
                cr.RequiredDate = defaultShipDate;
            }

            cr.EstShipDate = defaultShipDate;

            if(csr != null)
            {
                // Set default shipping method and address
                var shippingMethods = cr.CustomerSummaryRow
                    .GetCustomerShippingSummaryRows()
                    .Where(ship => ship.Active)
                    .ToList();

                cr.CustomerShippingSummaryRow = shippingMethods.FirstOrDefault(r => r.DefaultShippingMethod)
                    ?? shippingMethods.FirstOrDefault();

                var addresses = cr.CustomerSummaryRow
                    .GetCustomerAddressRows()
                    .Where(addr => addr.Active)
                    .ToList();

                cr.CustomerAddressRow = addresses.FirstOrDefault(addr => addr.IsDefault) ??
                    addresses.FirstOrDefault();

                // Set Require COC
                cr.RequireCoc = cr.CustomerSummaryRow.RequireCOCByDefault;
            }

            //attempt to set default values based on part
            var psr = Dataset.PartSummary.FindByPartID(Convert.ToInt32(row.PartID));

            if(psr != null)
            {
                // Pricing - set base price & unit for the new Work Order
                var partWeight = psr.IsWeightNull() ? 0 : psr.Weight;
                cr.Weight = Math.Min(Convert.ToDecimal(this.numWeight.MaxValue), cr.PartQuantity * partWeight);

                (var unitPrice, var unitType) = GetDefaultPrice(psr, cr.PartQuantity, cr.Weight, OrderPrice.ParsePriceUnit(cr.PriceUnit));

                cr.PriceUnit = unitType.ToString();
                cr.BasePrice = unitPrice;

                // Set Require COC it wasn't previously enabled by the customer option.
                cr.RequireCoc = cr.RequireCoc || psr.RequireCocByDefault;
            }

            // Set default product class
            var productClass = GetDefaultFieldValue(cr.CustomerID, "Product Class");

            if (!string.IsNullOrEmpty(productClass))
            {
                var productClassRow = Dataset.OrderProductClass.NewOrderProductClassRow();
                productClassRow.OrderRow = cr;
                productClassRow.ProductClass = productClass;
                Dataset.OrderProductClass.AddOrderProductClassRow(productClassRow);
            }

            //Load any media from the order receiving
            using(var ta = new Receiving_MediaTableAdapter())
            {
                var dt = ta.GetDataByRecId(receivingID);

                if(dt != null && dt.Count > 0)
                {
                    using(var taMedia = new Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter {ClearBeforeFill = false})
                        taMedia.FillByReceivingIdWOMedia(Dataset.Media, receivingID);

                    foreach(var dr in dt)
                    {
                        var mediaRow = Dataset.Media.FindByMediaID(dr.MediaID);
                        if(mediaRow != null)
                            Dataset.Order_Media.AddOrder_MediaRow(cr, mediaRow);
                    }
                }
            }

            // Copy document links
            using (var ta = new Receiving_DocumentLinkTableAdapter())
            {
                var dt = ta.GetDataByRecID(receivingID);

                if (dt != null)
                {
                    foreach (var recvDocumentLink in dt)
                    {
                        Dataset.Order_DocumentLink.AddOrder_DocumentLinkRow(
                            recvDocumentLink.DocumentInfoID,
                            Documents.LinkType.WorkOrder.ToString(),
                            cr);
                    }
                }
            }

            //update cost since we set values that might be related to price
            UpdateOrderCost(); //AddOrderRowFromReceivingOrder

            //End editing then add order processes since wont get added else where
            rowVw.EndEdit();

            _newOrderBeingCreated = true;
            AddPartProcesses(cr);
            AddOrderPartMark(cr);

            return cr;
        }

        public OrdersDataSet.OrderRow AddOrderRowFromQuotePart(int quotePartId, int? receivingQty)
        {
            //WORKFLOW
            //If part exists, match on name, and pull in part as normal but override with quote pricing?

            //If part does not exist, "The part was not found in the system, please create the part and try to import the quote part again."
            //Later, if part does not exist, invoke CreateFromQuote from PartManager

            //For processes, do we  import what is on the quote
            //Do quotes have PO's?

            //How do we handle part marking?

            int? customerId;
            QuoteDataSet.QuotePartDataTable quotePart;
            PartsDataset.PartDataTable partMatch;

            //First get the underlying data and validate it           
            using (var taQuotePart = new QuotePartTableAdapter())
            {
                customerId = taQuotePart.GetCustomerId(quotePartId);
                if (customerId == null)
                    return null;
                quotePart = taQuotePart.GetDataById(quotePartId);
                if (quotePart.Rows.Count == 0) 
                    return null;
            }

            var quotePartRec = quotePart[0];

            //find the part in the system to get associated properties
            using (var taPart = new PartTableAdapter())
            {
                partMatch = taPart.GetPartByNameAndCustomer(quotePartRec.Name, customerId.Value);
                if (partMatch.Rows.Count == 0)
                {
                    //Part does not exist...
                    MessageBox.Show(
                        "The part was not found in the system, please create the part in Parts Manager and try to import the quote part again.",
                        "Part Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    // Show Parts manager and add QuotePart
                    
                    //SelectPartUsingManager(quotePartRec.QuotePartID);


                    return null;
                }
            }

            var partMatchRec = partMatch[0];

            //Check if part is unfinished
            if (!partMatchRec.IsCreatedInReceivingNull() && partMatchRec.CreatedInReceiving)
            {
                var result = MessageBoxUtilities.ShowMessageBoxYesOrNo(
                    "The selected part has been stubbed out from receiving but does not appear to have all of the expected information." +
                    "Would you like to update/ add the imported part from Parts Manager?", "Part Unfinished");
                if (result == DialogResult.Yes)
                {
                    using (var partManager = new PartManager())
                    {
                        //Need to let the part manager load before calling method to add quote part
                        partManager.Load += (s, evt) => { partManager.CreatePartFromQuote(null, quotePartId); };

                        if (partManager.ShowDialog() == DialogResult.OK)
                        {
                            //find the part in the system once again to get associated properties
                            using (var taPart = new PartTableAdapter())
                            {
                                partMatch = taPart.GetPartByNameAndCustomer(quotePartRec.Name, customerId.Value);
                                if (partMatch.Rows.Count == 0) //HOW????
                                {
                                    //Part does not exist...
                                    MessageBox.Show(
                                        "The part was not found in the system, please create the part in Parts Manager and try to import the quote part again.",
                                        "Part Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return null;
                                }
                            }
                        }
                        //set our part match to the new version of the part
                        partMatchRec = partMatch[0];
                    }
                }
            }

            this.Enabled = true; //must set OE back to enabled after leaving Parts Manager, or import from receiving dialog
            //We have valid data, start building our new order
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as OrdersDataSet.OrderRow;

            //Set baseline props
            cr.OrderDate = DateTime.Now;
            cr.Status = Properties.Settings.Default.OrderStatusOpen;
            cr.Priority = Properties.Settings.Default.OrderPriorityDefault;
            cr.CreatedBy = SecurityManager.Current.UserID;           
            cr.ContractReviewed = false;
            cr.CurrentLocation = ApplicationSettings.Current.DepartmentSales;
            cr.WorkStatus = OrderControllerExtensions.GetNewOrderWorkStatus(cr.IsCustomerIDNull() ? 0 : cr.CustomerID, UserRequiresOrderReview);
            cr.Hold = false;
            cr.OrderType = (int)OrderType.Normal;
            cr.PartQuantity = receivingQty ?? 0; // quotePartRec.Quantity; //For now, we force the user to check this
            cr.PartID = partMatchRec.PartID;
            cr.CustomerID = customerId.Value;
            cr.BasePrice = 0;
            cr.QuotePartId = quotePartId;
            
            //Get customer info
            var csr = Dataset.CustomerSummary.FindByCustomerID(customerId.Value);
            var defaultShipDate = DateTime.Now.AddBusinessDays(csr?.LeadTime ?? ApplicationSettings.Current.OrderLeadTime);

            if (_requiredDateFieldLazy.Value?.IsVisible ?? true)
                cr.RequiredDate = defaultShipDate;

            cr.EstShipDate = defaultShipDate;

            //TODO:
            //This may need to change based on quote info
            if (csr != null)
            {
                // Set default shipping method and address
                var shippingMethods = cr.CustomerSummaryRow
                    .GetCustomerShippingSummaryRows()
                    .Where(ship => ship.Active)
                    .ToList();

                cr.CustomerShippingSummaryRow = shippingMethods.FirstOrDefault(r => r.DefaultShippingMethod)
                                                ?? shippingMethods.FirstOrDefault();

                var addresses = cr.CustomerSummaryRow
                    .GetCustomerAddressRows()
                    .Where(addr => addr.Active)
                    .ToList();

                cr.CustomerAddressRow = addresses.FirstOrDefault(addr => addr.IsDefault) ??
                                        addresses.FirstOrDefault();

                // Set Require COC
                cr.RequireCoc = cr.CustomerSummaryRow.RequireCOCByDefault || partMatchRec.RequireCocByDefault;
            }

            // Set default product class
            var productClass = GetDefaultFieldValue(cr.CustomerID, "Product Class");

            if (!string.IsNullOrEmpty(productClass))
            {
                var productClassRow = Dataset.OrderProductClass.NewOrderProductClassRow();
                productClassRow.OrderRow = cr;
                productClassRow.ProductClass = productClass;
                Dataset.OrderProductClass.AddOrderProductClassRow(productClassRow);
            }

            //HANDLE PRICING
            // Set price unit
            var priceUnitEntry = this.Dataset.PriceUnit.FindByPriceUnitID(OrderPrice.enumPriceUnit.Each.ToString());

            if (priceUnitEntry != null && !priceUnitEntry.Active)
                cr.PriceUnit = OrderPrice.enumPriceUnit.EachByWeight.ToString();
            else
                cr.PriceUnit = OrderPrice.enumPriceUnit.Each.ToString();

            // Pricing - set base price & unit for the new Work Order
            cr.Weight = quotePartRec.IsWeightNull() ? 0 : Math.Min(Convert.ToDecimal(this.numWeight.MaxValue), cr.PartQuantity * quotePartRec.Weight);

            if (!quotePartRec.IsEachPriceNull() && !quotePartRec.IsLotPriceNull())
                (cr.BasePrice, cr.PriceUnit) = OrderPrice.CalculateQuoteImportPrice(quotePartRec);

            //Load any media from the quote
            using (var taQuoteMedia = new QuotePart_MediaTableAdapter())
            {
                QuoteDataSet.QuotePart_MediaDataTable dtMedia = new QuoteDataSet.QuotePart_MediaDataTable();
                var dt = taQuoteMedia.FillByQuotePart(dtMedia, quotePartId);

                if (dtMedia.Count > 0)
                {
                    using (var taMedia = new Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter { ClearBeforeFill = false })
                        taMedia.FillByQuotePartMedia(Dataset.Media, quotePartId);

                    foreach (var media in dtMedia)
                    {
                        var mediaRow = Dataset.Media.FindByMediaID(media.MediaID);
                        if (mediaRow != null)
                            Dataset.Order_Media.AddOrder_MediaRow(cr, mediaRow);
                    }
                }
            }

            // Copy document links
            using (var taQuoteDocs = new QuotePart_DocumentLinkTableAdapter())
            {
                QuoteDataSet.QuotePart_DocumentLinkDataTable dtDocs = new QuoteDataSet.QuotePart_DocumentLinkDataTable();
                taQuoteDocs.FillByQuotePart(dtDocs, quotePartId);
                foreach (var recDocumentLink in dtDocs)
                {
                    Dataset.Order_DocumentLink.AddOrder_DocumentLinkRow(
                        recDocumentLink.DocumentInfoID,
                        Documents.LinkType.WorkOrder.ToString(),
                        cr);
                }
            }

            //Need to build customizations of these most likely
            AddQuotePartProcesses(cr, quotePartId);
            AddOrderPartMark(cr); //Revisit
            AddQuotePartFeesToOrder(cr, quotePartId);

            //update cost since we set values that might be related to price
            UpdateOrderCost(); //AddOrderRowFromQuotePart

            //End editing then add order processes since wont get added else where
            rowVw.EndEdit();

            return cr;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            try
            {
                //this._newOrderBeingCreated = false;
                base.AfterMovedToNewRecord(id);

                var order = base.CurrentRecord as OrdersDataSet.OrderRow;
                this.grpData.Text = $"Work Order - {order.OrderID.ToString()}";
                _OrderID = order.OrderID;
                //Set the Day until required based on current day at noon until dtOrderRequiredDate at noon difference
                DateTime reqDate = new DateTime();
                TimeSpan ts = new TimeSpan(0);
                if (this.dtOrderRequiredDate.Value != null)
                {
                    reqDate = DataUtilities.ChangeTime((DateTime)this.dtOrderRequiredDate.Value, 12, 0, 0, 0);
                    ts = reqDate.Subtract((DateTime)DataUtilities.ChangeTime(DateTime.Now, 12, 0, 0, 0));
                }

                this.neDayUntilReq.Value = Convert.ToInt32(ts.Days);

                //this.neDayUntilReq.Value = 1;

                this.txtInvoice.ButtonsLeft[0].Enabled = !String.IsNullOrEmpty(this.txtInvoice.Text); //enable invoice to allow user to delete if there is an invoice to reset

                if (ApplicationSettings.Current.ApplyDefaultFeesEnabled && _newOrderBeingCreated)
                    OrderFeeTools.AddDefaultFees(ref order);

                LoadCurrentPart();
                if(!_newOrderBeingCreated)
                    UpdateOrderCost(); //After Move to new reord  | have to call because the change event may not get fired between changing orders if they have the same price
                BindCustomFields(false);

                _autoUpdateEstShipDate = order.IsSalesOrderIDNull();




                //Load media
                if (order != null)
                    this.mediaWidget.LoadMedia(order.GetOrder_MediaRows().ToList<DataRow>(),
                        order.GetOrder_DocumentLinkRows().ToList<DataRow>(),
                        order.OrderID);
                else
                    this.mediaWidget.ClearMedia();

                //if not view only mode (i.e. edit mode)
                if (!ViewOnly)
                {
                    //update edit permissions based on status of order and user role
                    if (order != null && order.Status == Properties.Settings.Default.OrderStatusClosed)
                    {
                        this.curFeesTotal.Enabled = SecurityManager.Current.IsInRole("EditClosedOrderPrice");
                        this.numUnitPrice.Enabled = SecurityManager.Current.IsInRole("EditClosedOrderPrice");
                    }
                    else
                    {
                        this.curFeesTotal.Enabled = SecurityManager.Current.IsInRole("OrderEntry.Edit");
                        this.numUnitPrice.Enabled = SecurityManager.Current.IsInRole("OrderEntry.Edit");
                    }
                }

                cboPart.ButtonsRight["AddProcessPackage"].Visible = true;
                UpdateAddProcessPackageEnabled(order);

                this.txtPONumber.ButtonsRight["btnFilter"].Visible = !IsNewRow.GetValueOrDefault(true);

                if (order != null && !Dataset.PriceUnit.Any(unit => !unit.Active && order.PriceUnit == unit.PriceUnitID))
                {
                    this._priceUnitDataView.RowFilter = "Active = 1";
                }

                // Establish minimum for unit price -if- the WO does not already have a negative price.
                // Workaround for #27568.
                if (!order.IsBasePriceNull() && order.BasePrice >= 0M)
                {
                    numUnitPrice.MinValue = 0M;
                }

                ActiveSerialNumberWidget.LoadOrder(order);
                productClassWidget.LoadRow(order);
                workDescriptionWidget.LoadRow(order);
                UpdateContainerText(order);
                UpdateShippingFilters();
                if (_newOrderBeingCreated)
                    chkRequireCoc.Checked = !ApplicationSettings.Current.AllowSkippingCoc;

                if (order.IsQuotePartIdNull())
                {
                    // Offer to sync processes from parts to open order w/o processes.
                    var checkPartProcesses = order.Status == Properties.Settings.Default.OrderStatusOpen
                        && !order.IsPartIDNull()
                        && order.GetOrderProcessesRows().Length == 0;

                    if (checkPartProcesses)
                    {
                        _log.Info($"Selected order does not have processes - loading from part.");
                        int processCount;
                        using (var taPartProcessSummary = new PartProcessSummaryTableAdapter())
                        {
                            processCount = taPartProcessSummary.GetData(order.PartID).Count;
                        }

                        if (processCount > 0)
                        {

                            if (!ApplicationSettings.Current.AutoImportProcessesToOrder)
                            {
                                _log.Info("Found processes on part - offering to sync.");
                                var dialogResult = MessageBoxUtilities.ShowMessageBoxYesOrNo(
                                    "Would you like to add processes from the part to the order?",
                                    "Add Processes");

                                if (dialogResult == DialogResult.Yes)
                                {
                                    _log.Info("Syncing processes");
                                    AddPartProcesses(order);
                                }
                                else
                                {
                                    _log.Info("User declined to sync processes.");
                                }
                            }
                            else
                            {
                                _log.Info("Syncing processes");
                                AddPartProcesses(order);
                            }
                        }
                        else
                        {
                            _log.Info("No processes found");
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error showing Work Order.");
            }
            finally
            {
                this._newOrderBeingCreated = false;
                cboUnit.ValueChanged += OnPriceUnit_ValueChanged;
            }
        }

        private void UpdateShippingFilters()
        {
            var currentOrder = CurrentRecord as OrdersDataSet.OrderRow;

            if (currentOrder == null)
            {
                return;
            }

            var customerId = currentOrder.IsCustomerIDNull()
                ? -1
                : currentOrder.CustomerID;

            var currentShippingMethodId = currentOrder == null || currentOrder.IsShippingMethodNull()
                ? -1
                : currentOrder.ShippingMethod;

            var currentShipToAddressId = currentOrder == null || currentOrder.IsCustomerAddressIDNull()
                ? -1
                : currentOrder.CustomerAddressID;

            // Show shipping methods that
            // - Belong to the current customer, and
            // - are active or the current shipping method for the order
            var shippingMethodFilter = $"CustomerID = {customerId} AND " +
                $"(Active = 1 OR CustomerShippingID = {currentShippingMethodId})";

            // Show ship-to addresses that
            // - Belong to the current customer, and
            // - are active or the current address for the order
            var shipToFilter = $"CustomerID = {customerId} AND " +
                $"(Active = 1 OR CustomerAddressID = {currentShipToAddressId})";

            UpdateFilter(cboShippingMethod, shippingMethodFilter);
            UpdateFilter(cboShipTo, shipToFilter);
        }

        private void UpdateAddProcessPackageEnabled(OrdersDataSet.OrderRow order)
        {
            cboPart.ButtonsRight["AddProcessPackage"].Enabled = !ViewOnly &&
                Editable &&
                !order.IsPartIDNull();
        }

        private void UpdateContainerText(OrdersDataSet.OrderRow order)
        {
            if (order == null)
            {
                txtContainers.Text = string.Empty;
                return;
            }

            var containerCount = order.GetOrderContainersRows().Length;

            txtContainers.Text = containerCount == 1
                ? "1 Container"
                : $"{containerCount} Containers";
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            base.BeforeMoveToNewRecord(id);

            //move focus off of PART becuase will prevent it from showing unselected part if you have the first part picked for the custmer
            cboCustomer.Focus();

            // prevents cboUnit from having a null value when changing record
            this._priceUnitDataView.RowFilter = null;

            // Prevent unit price & order process price updates
            // while switching
            cboUnit.ValueChanged -= OnPriceUnit_ValueChanged;

            // Reset MinValue for price picker in case the order has a negative price
            // Workaround for #27568.
            numUnitPrice.MinValue = -999999M;
        }

        public override void EndEditing()
        {
            var currentOrder = CurrentRecord as OrdersDataSet.OrderRow;

            var autoCheckIn = currentOrder != null &&
                              !ApplicationSettings.Current.OrderCheckInEnabled &&
                              currentOrder.RowState == DataRowState.Added &&
                              (currentOrder.WorkStatus == ApplicationSettings.Current.WorkStatusChangingDepartment ||
                              currentOrder.WorkStatus == ApplicationSettings.Current.WorkStatusInProcess);

            if (autoCheckIn)
            {
                var firstProcess = currentOrder.GetOrderProcessesRows()
                    .Where(op => op.IsValidState())
                    .OrderBy(op => op.StepOrder)
                    .FirstOrDefault();

                if (firstProcess != null)
                {
                    if (currentOrder.WorkStatus != ApplicationSettings.Current.WorkStatusInProcess)
                    {
                        currentOrder.WorkStatus = ApplicationSettings.Current.WorkStatusInProcess;
                    }

                    if (currentOrder.CurrentLocation != firstProcess.Department)
                    {
                        currentOrder.CurrentLocation = firstProcess.Department;
                    }

                    if (firstProcess.IsStartDateNull())
                    {
                        firstProcess.StartDate = DateTime.Now;
                    }
                }
            }

            base.EndEditing();
            ActiveSerialNumberWidget.SaveRow();
            productClassWidget.SaveRow();
            workDescriptionWidget.SaveRow();
        }

        public override void CancelEdits()
        {
            base.CancelEdits();
        }

        protected override void OnEditableStatusChange(bool editable)
        {
            base.OnEditableStatusChange(editable);

            this.cboPart.ReadOnly = !editable;
        }

        private void LoadStatusList()
        {
            foreach(var item in this.cboStatus.Items)
            {
                switch(item.DisplayText)
                {
                    case "Open":
                        item.Appearance.Image = Properties.Resources.Status_Open;
                        break;
                    case "Closed":
                        item.Appearance.Image = Properties.Resources.Status_Closed;
                        break;
                    default:
                        item.Appearance.Image = Properties.Resources.Status_None;
                        break;
                }
            }
        }

        private string GetDefaultFieldValue(int customerID, string field)
        {
            var key = customerID + "-" + field;

            if(!this._fieldDefaultValueCache.ContainsKey(key))
            {
                using(var ta = new Customer_FieldsTableAdapter {ClearBeforeFill = false})
                    this._fieldDefaultValueCache.Add(key, ta.GetDefaultValue(field, customerID));
            }

            return this._fieldDefaultValueCache[key];
        }

        /// <summary>
        ///     Loads the current part if it has not been downloaded, this could be due to the part being inactive.
        /// </summary>
        private void LoadCurrentPart()
        {
            if(base.CurrentRecord is OrdersDataSet.OrderRow)
            {
                var order = base.CurrentRecord as OrdersDataSet.OrderRow;

                if(!order.IsPartIDNull() && order.PartID > 0)
                {
                    var part = Dataset.PartSummary.FindByPartID(order.PartID);

                    if(part == null)
                    {
                        _log.Info("Manually loaded part because did not exist in current Part DataTable " + order.PartID);
                        this._taPartSummary.FillByPart(Dataset.PartSummary, order.PartID);
                    }

                    //if part does not belong to this customer
                    if(part != null && part.CustomerID != order.CustomerID)
                    {
                        this.cboPart.Text = part.Name; //set text to show what it was
                        order.SetPartIDNull();
                    }
                }
                else
                {
                    cboPart.Clear();
                }
            }
        }

        private void UpdateUnitPrice()
        {
            // Update the numUnitPrice
            var or = base.CurrentRecord as OrdersDataSet.OrderRow;

            if (or == null || or.IsPartIDNull() || or.OrderType == (int)OrderType.ReworkExt)
            {
                return;
            }

            var part = Dataset.PartSummary.FindByPartID(Convert.ToInt32(or.PartID));
            if (part == null)
            {
                return;
            }

            OrderPrice.enumPriceUnit unitType = OrderPrice.ParsePriceUnit(this.cboUnit.Text);

            decimal? unitPrice = OrderPrice.DetermineBasePrice(part, unitType);
            bool hasQty = numPartQty.Value != null && numPartQty.Value != DBNull.Value;

            if (ApplicationSettings.Current.PartPricingType == PricingType.Process && hasQty)
            {
                int partQty = Convert.ToInt32(numPartQty.Value);
                var orderWeight = numWeight.Value != null && numWeight.Value != DBNull.Value
                    ? Convert.ToDecimal(numWeight.Value)
                    : 0;

                var processLevelUnitPrice = GetProcessLevelUnitPrice(part, unitType, partQty, orderWeight);

                if (processLevelUnitPrice.HasValue)
                {
                    unitPrice = processLevelUnitPrice.Value;
                    UpdateProcessPrices(unitType);
                }
            }

            if (unitPrice.HasValue)
            {
                this.numUnitPrice.Value = Math.Max(unitPrice.Value, 0M);
            }
        }

        private void UpdateUnitPriceByQuotePart()
        {
            try
            {
                QuoteDataSet.QuotePartDataTable quotePart;
                // Update the numUnitPrice

                if (!(base.CurrentRecord is OrdersDataSet.OrderRow or) || or.IsPartIDNull() ||
                    or.OrderType == (int) OrderType.ReworkExt)
                    return;

                using (var taQuotePart = new QuotePartTableAdapter())
                {
                    quotePart = taQuotePart.GetDataById(or.QuotePartId);
                    if (quotePart.Rows.Count == 0)
                        return;
                }

                var quotePartRec = quotePart[0];
                if (quotePartRec.IsEachPriceNull() && quotePartRec.IsLotPriceNull()) return;
 
                OrderPrice.enumPriceUnit unitType = OrderPrice.ParsePriceUnit(this.cboUnit.Text);
                decimal? unitPrice;
                //Determine new price
                switch (unitType)
                {
                    case OrderPrice.enumPriceUnit.Each:
                    case OrderPrice.enumPriceUnit.EachByWeight:
                    {
                        if (quotePartRec.EachPrice > 0)
                            unitPrice = quotePartRec.EachPrice;
                        else if (quotePartRec.Quantity > 0)
                        {
                            unitPrice = quotePartRec.LotPrice / quotePartRec.Quantity;
                            MessageBox.Show(
                                $"Each price not found for {quotePartRec.Name}. Each price determined by the following calculation:\n" +
                                $"\tLot Price: ({quotePartRec.LotPrice}) / Quoted Quantity: ({quotePartRec.Quantity})",
                                "Each Price Inferred", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                            unitPrice = 0;
                    }
                        break;
                    case OrderPrice.enumPriceUnit.Lot:
                    case OrderPrice.enumPriceUnit.LotByWeight:
                        unitPrice = quotePartRec.LotPrice;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"{nameof(unitType)}");
                }

                bool hasQty = numPartQty.Value != null && numPartQty.Value != DBNull.Value;

                //if (ApplicationSettings.Current.PartPricingType == PricingType.Process && hasQty)
                //{
                //    int partQty = Convert.ToInt32(numPartQty.Value);
                //    var orderWeight = numWeight.Value != null && numWeight.Value != DBNull.Value
                //        ? Convert.ToDecimal(numWeight.Value)
                //        : 0;

                //    var processLevelUnitPrice = GetProcessLevelUnitPrice(part, unitType, partQty, orderWeight);

                //    if (processLevelUnitPrice.HasValue)
                //    {
                //        unitPrice = processLevelUnitPrice.Value;
                //        UpdateProcessPrices(unitType);
                //    }
                //}

                if (unitPrice.HasValue)
                    this.numUnitPrice.Value = Math.Max(unitPrice.Value, 0M);
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Error updating unit price for quote part");
            }
           
        }

        private static decimal? GetProcessLevelUnitPrice(OrdersDataSet.PartSummaryRow part, OrderPrice.enumPriceUnit unitType, int partQty, decimal orderWeight)
        {
            decimal? unitPrice = null;
            PartProcessPriceSummaryTableAdapter taPartProcessPrice = null;

            try
            {
                taPartProcessPrice = new PartProcessPriceSummaryTableAdapter();
                var processPriceData = taPartProcessPrice.GetData(part.PartID);
                var matchingProcessPrices = new List<OrdersDataSet.PartProcessPriceSummaryRow>();

                foreach (var processPrice in processPriceData)
                {
                    var pricePoint = Data.Order.PricePoint.From(processPrice);

                    if (pricePoint.Matches(partQty, orderWeight, unitType))
                    {
                        matchingProcessPrices.Add(processPrice);
                    }
                }

                // Only override price if process-level pricing defines a
                // price. This mirrors part-level pricing functionality.
                var processPriceSum = matchingProcessPrices
                    .Sum(price => price.Amount);

                if (processPriceSum > 0M)
                {
                    LogManager.GetCurrentClassLogger().Info("Substituting part price w/ process price sum.");
                    unitPrice = processPriceSum;
                }

                return unitPrice;
            }
            finally
            {
                taPartProcessPrice?.Dispose();
            }
        }

        /// <summary>
        ///     Updates the currently selected orders cost.
        /// </summary>
        private void UpdateOrderCost()
        {
            try
            {
                var currentOrder = base.CurrentRecord as OrdersDataSet.OrderRow;
                string currentPriceUnit = this.cboUnit.Value?.ToString();

                if (!PartsLoading && _panelLoaded && !_recordLoading && currentOrder != null && !string.IsNullOrEmpty(currentPriceUnit))
                {
                    _log.Info("Updating order cost.");
                    
                    //Update Total Price
                    if(this.numPartQty.Value == null || this.numPartQty.Value == DBNull.Value || this.numUnitPrice.ValueObject == null)
                    { 
                        this.numTotalCurrency.Value = 0;
                    }
                    else
                    {
                        //BUG - found when calculating each price and percentage fees where part quantity not getting updated.. SO, ensure that our qty matches [MF]

                        //if (!this._newOrderBeingCreated)
                        //    currentOrder.PartQuantity = Convert.ToInt32(this.numPartQty.Value); 

                        //Calculate the fees
                        this.curFeesTotal.Value = OrderPrice.CalculateFees(currentOrder, this.numUnitPrice.Value);

                        decimal weight = this.numWeight.Value == DBNull.Value ? 0M : Convert.ToDecimal(this.numWeight.Value);
                        int qty = Convert.ToInt32(this.numPartQty.Value);
                        decimal totalPrice = OrderPrice.CalculatePrice(this.numUnitPrice.Value, currentPriceUnit, this.curFeesTotal.Value, qty, weight);

                        if (totalPrice > this.numTotalCurrency.MaxValue)
                        {
                            MessageBoxUtilities.ShowMessageBoxWarn("Total currency value has exceeded maximum value. Please correct the price, quantity, or fees as needed.", "Incorrect Total Value");
                            this.numTotalCurrency.Value = this.numTotalCurrency.MaxValue;
                        }
                        else
                        {
                            this.numTotalCurrency.Value = totalPrice;
                        }

                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error updating the orders cost.";
                _log.Fatal(exc, errorMsg);
            }
        }

        /// <summary>
        /// Updates the weight based on the currently selected part and the
        /// current part quantity.
        /// </summary>
        private void UpdateWeight()
        {
            try
            {
                this.numWeight.ValueChanged -= this.numWeight_ValueChanged;
                bool hasPart = this.cboPart.SelectedItem != null && this.cboPart.SelectedItem.DataValue != DBNull.Value;
                bool hasQty = this.numPartQty.Value != DBNull.Value;

                if (ApplicationSettings.Current.SyncQuantityAndWeightForOrders && !PartsLoading && _panelLoaded && !_recordLoading && base.CurrentRecord != null && hasPart && hasQty)
                {
                    _log.Info("Updating order weight.");
                    var part = CurrentPart;

                    int qty = Convert.ToInt32(this.numPartQty.Value);
                    decimal weight = part.IsWeightNull() ? 0M : part.Weight * qty;

                    if (weight > Convert.ToDecimal(this.numWeight.MaxValue))
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            "Total weight has exceeded maximum value. Please correct the part weight or quantity as needed.",
                            "Incorrect Weight Total Value");

                        this.numWeight.Value = this.numWeight.MaxValue;
                    }
                    else if (weight > 0M && weight >= Convert.ToDecimal(this.numWeight.MinValue))
                    {
                        this.numWeight.Value = weight;
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating order weight.");
            }
            finally
            {
                this.numWeight.ValueChanged += this.numWeight_ValueChanged;
            }
        }

        private void UpdatePartQuantity()
        {
            try
            {
                this.numPartQty.ValueChanged -= this.numPartQty_ValueChanged;
                bool hasPart = this.cboPart.SelectedItem != null && this.cboPart.SelectedItem.DataValue != DBNull.Value;
                bool hasWeight = this.numWeight.Value != DBNull.Value;

                if (ApplicationSettings.Current.SyncQuantityAndWeightForOrders && !PartsLoading && _panelLoaded && !_recordLoading && base.CurrentRecord != null && hasPart && hasWeight)
                {
                    _log.Info("Updating part quantity.");

                    var part = Dataset.PartSummary.FindByPartID(Convert.ToInt32(this.cboPart.SelectedItem.DataValue));

                    decimal weight = Convert.ToDecimal(this.numWeight.Value);
                    int qty = -1;

                    if (part.IsWeightNull())
                    {
                        _log.Info("The Weight is null.");
                    }
                    if (!part.IsWeightNull() && part.Weight > 0)
                    {
                        qty = Convert.ToInt32(System.Math.Ceiling(weight / part.Weight));

                    }

                    if (qty == 0 && weight > 0)
                    {
                        qty = 1;
                    }
                       
                    if (qty > Convert.ToInt32(this.numPartQty.MaxValue))
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            "Quantity has exceeded maximum value. Please correct the part weight or total weight as needed.",
                            "Incorrect Quantity Value");

                        this.numPartQty.Value = this.numPartQty.MaxValue;
                    }
                    else if (qty > 0 && qty >= Convert.ToInt32(this.numPartQty.MinValue))
                    {
                        this.numPartQty.Value = qty;

                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, " - Error updating part quantity.");
            }
            finally
            {
                this.numPartQty.ValueChanged += this.numPartQty_ValueChanged;
            }
        }

        /// <summary>
        /// Pricing - automatically update the base price and unit
        /// </summary>
        private void UpdateDefaultPrice()
        {
            try
            {
                var currentOrder = CurrentRecord as OrdersDataSet.OrderRow;

                var skipUpdate = this.cboPart.SelectedItem == null ||
                    this.cboPart.SelectedItem.DataValue == DBNull.Value ||
                    this.numPartQty.Value == null ||
                    this.numPartQty.Value == DBNull.Value ||
                    currentOrder?.OrderType == (int)OrderType.ReworkExt;

                if (skipUpdate)
                {
                    return;
                }

                _log.Debug("Updating default price.");

                var part = Dataset.PartSummary.FindByPartID(Convert.ToInt32(this.cboPart.SelectedItem.DataValue));
                var partQty = Convert.ToInt32(this.numPartQty.Value);
                decimal weight = this.numWeight.Value == DBNull.Value ? 0M : Convert.ToDecimal(this.numWeight.Value);
                var currentPriceUnit = OrderPrice.ParsePriceUnit(this.cboUnit.Text);

                //attempt to set default values based on part
                (var newPrice, var priceUnit) = GetDefaultPrice(part, partQty, weight, currentPriceUnit);

                _log.Debug("Price unit {0}, New Price {1}".FormatWith(priceUnit, newPrice));
                this.numUnitPrice.Value = Math.Max(newPrice, 0M);
                this.numUnitPrice.DataBindings[0].WriteValue();

                //set part default price unit
                var unitItem = this.cboUnit.FindItemByValue <string>(i => i == priceUnit.ToString());

                if(unitItem != null && unitItem.DataValue != this.cboUnit.SelectedItem.DataValue)
                {
                    this.cboUnit.SelectedItem = unitItem;
                    this.cboUnit.DataBindings[0].WriteValue();
                }

                if (ApplicationSettings.Current.PartPricingType == PricingType.Process)
                {
                    UpdateProcessPrices(priceUnit);
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating default price.");
            }
        }

        private void UpdateProcessPrices(OrderPrice.enumPriceUnit priceUnit)
        {
            PartProcessPriceSummaryTableAdapter taPartProcessPrice = null;
            PartProcessSummaryTableAdapter ppsTA = null;

            try
            {
                var currentPart = Dataset.PartSummary.FindByPartID(Convert.ToInt32(this.cboPart.SelectedItem?.DataValue ?? 0));
                var currentOrder = base.CurrentRecord as OrdersDataSet.OrderRow;

                if (currentPart == null || currentOrder == null)
                {
                    return;
                }

                var partQty = Convert.ToInt32(this.numPartQty.Value);
                var orderWeight = numWeight.Value != null && numWeight.Value != DBNull.Value
                    ? Convert.ToDecimal(numWeight.Value)
                    : 0;

                taPartProcessPrice = new PartProcessPriceSummaryTableAdapter();
                ppsTA = new PartProcessSummaryTableAdapter();

                var partProcesses = ppsTA.GetData(currentPart.PartID);
                var partProcessPriceData = taPartProcessPrice.GetData(currentPart.PartID);

                if (partProcessPriceData.Count > 0)
                {
                    _log.Info("Updating per-process prices.");
                }

                foreach (var processPrice in partProcessPriceData)
                {
                    var pricePoint = Data.Order.PricePoint.From(processPrice);

                    if (pricePoint.Matches(partQty, orderWeight, priceUnit))
                    {
                        var partProcess = partProcesses
                            .FirstOrDefault(summary => summary.PartProcessID == processPrice.PartProcessID);

                        OrdersDataSet.OrderProcessesRow orderProcess = null;

                        if (partProcess != null)
                        {
                            orderProcess = Dataset
                                .OrderProcesses
                                .FirstOrDefault((process) =>
                                {
                                    return process.IsValidState() &&
                                        process.OrderID == currentOrder.OrderID &&
                                        process.StepOrder == partProcess.StepOrder &&
                                        process.ProcessAliasID == partProcess.ProcessAliasID;
                                });
                        }

                        if (orderProcess != null)
                        {
                            orderProcess.Amount = Math.Max(processPrice.Amount, 0M);
                        }
                    }
                }
            }
            finally
            {
                taPartProcessPrice?.Dispose();
            }
        }

        private PriceInfo GetDefaultPrice(OrdersDataSet.PartSummaryRow part, int partQty, decimal weight, OrderPrice.enumPriceUnit currentPriceUnit)
        {
            PartProcessPriceSummaryTableAdapter taPartProcessPrice = null;

            try
            {
                if (part == null)
                {
                    // Cannot retrieve price information
                    OrderPrice.enumPriceUnit defaultPriceUnit = _priceUnitPersistence.DefaultPriceUnit();

                    return new PriceInfo
                    {
                        BasePrice = 0,
                        DisplayName = _priceUnitPersistence.GetDisplayText(defaultPriceUnit),
                        PriceUnit = defaultPriceUnit
                    };
                }

                var currentPriceByType = OrderPrice.GetPriceByType(currentPriceUnit);

                var newPriceUnit = _priceUnitPersistence.DeterminePriceUnit(part.CustomerID, partQty, weight, currentPriceUnit);
                decimal newPrice = OrderPrice.DetermineBasePrice(part, newPriceUnit).GetValueOrDefault();

                var newPriceUnitData = _priceUnitPersistence.FindByPriceUnitId(part.CustomerID, newPriceUnit.ToString());
                int? minQty = newPriceUnitData.MinQuantity;
                int? maxQty = newPriceUnitData.MaxQuantity;
                decimal? minWeight = newPriceUnitData.MinWeight;
                decimal? maxWeight = newPriceUnitData.MaxWeight;

                if (ApplicationSettings.Current.PartPricingType == PricingType.Process)
                {
                    OrderPrice.enumPriceUnit priceUnitFromPart = newPriceUnit;
                    taPartProcessPrice = new PartProcessPriceSummaryTableAdapter();
                    var processPriceData = taPartProcessPrice.GetData(part.PartID);

                    if (processPriceData.Count > 0)
                    {
                        // Find matching volume discount price point (if available)
                        var volumeDiscountPricePoints = processPriceData
                            .Where(p => !p.IsMinValueNull())
                            .Select(PricePoint.From);

                        foreach (var pricePoint in volumeDiscountPricePoints)
                        {
                            var isMatch = pricePoint.PriceByType == currentPriceByType
                                && (pricePoint.MatchesQuantity(partQty) || pricePoint.MatchesWeight(weight));

                            if (isMatch)
                            {
                                // Found volume discount price point
                                priceUnitFromPart = pricePoint.PriceUnit;
                                minQty = pricePoint.MinQuantity;
                                maxQty = pricePoint.MaxQuantity;
                                minWeight = pricePoint.MinWeight;
                                maxWeight = pricePoint.MaxWeight;
                                break;
                            }
                        }

                        var processPriceSum = 0M;

                        foreach (var processPrice in processPriceData)
                        {
                            var pricePoint = Data.Order.PricePoint.From(processPrice);

                            if (pricePoint.Matches(partQty, weight, priceUnitFromPart))
                            {
                                processPriceSum += processPrice.Amount;
                            }
                        }

                        if (processPriceSum != 0M)
                        {
                            LogManager.GetCurrentClassLogger().Info("Substituting part price w/ process price sum.");
                            newPrice = processPriceSum;
                            newPriceUnit = priceUnitFromPart;
                        }
                    }
                }

                return new PriceInfo
                {
                    BasePrice = newPrice,
                    PriceUnit = newPriceUnit,
                    MinQuantity = minQty,
                    MaxQuantity = maxQty,
                    MinWeight = minWeight,
                    MaxWeight = maxWeight,
                    DisplayName = _priceUnitPersistence.GetDisplayText(newPriceUnit)
                };
            }
            finally
            {
                taPartProcessPrice?.Dispose();
            }
        }

        /// <summary>
        ///     Selects the default shipping method after the shipping methods for the selected customer has been updated.
        /// </summary>
        private void SelectDefaultShippingMethod()
        {
            try
            {
                var order = CurrentRecord as OrdersDataSet.OrderRow;

                if(order != null && order.CustomerSummaryRow != null)
                {
                    //if no current shipping method or current shipping method is not valid for this customer then reset it
                    if(order.IsShippingMethodNull() || (order.CustomerShippingSummaryRow != null && order.CustomerShippingSummaryRow.CustomerID != CurrentCustomerID))
                    {
                        var shippingMethods = order.CustomerSummaryRow
                            .GetCustomerShippingSummaryRows()
                            .Where(ship => ship.Active)
                            .ToList();

                        order.CustomerShippingSummaryRow = shippingMethods.FirstOrDefault(r => r.DefaultShippingMethod)
                            ?? shippingMethods.FirstOrDefault();
                    }

                    //force cbo to update from data row
                    this.cboShippingMethod.DataBindings[0].ReadValue();
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error selecting default shipping method.");
            }
        }

        private void SelectDefaultShippingAddress()
        {
            try
            {
                var order = CurrentRecord as OrdersDataSet.OrderRow;

                if (order?.CustomerSummaryRow == null)
                {
                    return;
                }

                if (order.IsCustomerAddressIDNull() || order.CustomerAddressRow?.CustomerID != CurrentCustomerID)
                {
                    var addresses = order.CustomerSummaryRow
                        .GetCustomerAddressRows()
                        .Where(addr => addr.Active)
                        .ToList();

                    order.CustomerAddressRow = addresses.FirstOrDefault(addr => addr.IsDefault) ??
                        addresses.FirstOrDefault();
                }

                // force editor update
                cboShipTo.DataBindings[0].ReadValue();

            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error selecting default shipping address.");
            }
        }

        /// <summary>
        ///     Deletes the uploaded Purchase Orders (txtMedia) off of the local hard drive.
        /// </summary>
        public void DeleteUploadedMediaFiles()
        {
            try
            {
                if(this._filesToDelete != null)
                {
                    foreach(var item in this._filesToDelete)
                    {
                        try
                        {
                            if(!String.IsNullOrEmpty(item) && File.Exists(item))
                                File.Delete(item);
                        }
                        catch(Exception exc)
                        {
                            string errorMsg = "Error deleting file " + item;
                            _log.Warn(exc, errorMsg);
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error deleting PO";
                _log.Warn(exc, errorMsg);
            }
        }

        /// <summary>
        ///     Adds the file to list of files to delete after a save operation occurs.
        /// </summary>
        /// <param name="filePath"> The file path. </param>
        public void AddMediaToDelete(string filePath)
        {
            if(this._filesToDelete == null)
                this._filesToDelete = new List <string>();

            if(!this._filesToDelete.Contains(filePath))
                this._filesToDelete.Add(filePath);
        }

        private void UpdatePartItemsAppearance()
        {
            foreach(var item in this.cboPart.Items)
            {
                if(!Convert.ToBoolean(((DataRowView) item.ListObject).Row["Active"]))
                {
                    item.Appearance.FontData.Italic = DefaultableBoolean.True;
                    item.Appearance.ForeColor = Color.Gray;
                }
            }
        }

        private void UpdateDefaultFieldsPerCustomer(int customerID)
        {
            try
            {
                //if customer has Default Field values defined then set it for this customer...
                if(IsActivePanel && customerID > 0)
                {
                    var po = GetDefaultFieldValue(customerID, "PO");
                    this.txtPONumber.Text = string.IsNullOrWhiteSpace(po) ? null : po;

                    var customerWO = GetDefaultFieldValue(customerID, "Customer WO");
                    this.txtCustomerWO.Text = string.IsNullOrWhiteSpace(customerWO) ? null : customerWO;

                    var productClass = GetDefaultFieldValue(customerID, "Product Class");
                    productClassWidget.Text = string.IsNullOrWhiteSpace(productClass) ? null : productClass;
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error setting blanked PO.";
                _log.Error(exc, errorMsg);
            }
        }

        private void UpdateEstimatedShipDate()
        {
            var currentOrder = CurrentRecord as OrdersDataSet.OrderRow;

            if (currentOrder == null || !IsNewRow.GetValueOrDefault())
            {
                return;
            }

            //update est ship date based on customer lead time
            var leadTime = CurrentCustomer == null ?
                ApplicationSettings.Current.OrderLeadTime :
                CurrentCustomer.LeadTime;

            var newShipDate = DateUtilities.AddBusinessDays(DateTime.Now, leadTime);

            var currentEstShipDate = dtShipDate.Value as DateTime?;

            var updateEstShipDate = newShipDate != currentEstShipDate;

            if (updateEstShipDate && !_autoUpdateEstShipDate)
            {
                // Prompt user before updating the estimated ship date
                const string msgHeader = "Est. Ship Date";

                string updateShipDateMsg = string.Format(
                    "Would you like to update the estimated shipping date to {0:MM/dd/yyyy}?",
                    newShipDate);

                updateEstShipDate = MessageBoxUtilities.ShowMessageBoxYesOrNo(updateShipDateMsg, msgHeader) == DialogResult.Yes;
            }

            if (updateEstShipDate)
            {
                dtShipDate.DateTime = newShipDate;
            }
        }

        /// <summary>
        ///     Create the custom fields based on the customer.
        /// </summary>
        private void CreateCustomFields()
        {
            try
            {
                const int customFieldMaxLength = 255;

                _log.Debug("Creating custom field controls.");
                int tableRowCount = 0;

                if(Dataset != null)
                {
                    //clean up previous controls
                    var controls = new Control[this.tableCustomFields.Controls.Count];
                    this.tableCustomFields.Controls.CopyTo(controls, 0);

                    foreach(var control in controls)
                    {
                        //remove the validator from the previous custom field
                        if(control.Tag is CustomFieldUIInfo && ((CustomFieldUIInfo) control.Tag).Validator != null)
                            this._validationManager.Remove(((CustomFieldUIInfo) control.Tag).Validator);

                        control.DataBindings.Clear();
                        control.Dispose();
                    }

                    this.tableCustomFields.Controls.Clear();

                    //find current customer
                    var customer = Dataset.CustomerSummary.FindByCustomerID(CurrentCustomerID);

                    if(customer != null)
                    {
                        var customFields = customer.GetCustomFieldRows()
                            .OrderByDescending(c => c.Required)
                            .ThenBy(c => c.Name);

                        foreach(var customField in customFields)
                        {
                            if (!customField.IsVisible)
                            {
                                continue;
                            }

                            var label = new Label
                            {
                                Text = customField.Name + ":",
                                Dock = DockStyle.Fill,
                                TextAlign = ContentAlignment.TopLeft
                            };

                            this.tableCustomFields.Controls.Add(label, 0, tableRowCount);

                            Control control;
                            if (customField.IsListIDNull())
                            {
                                control = new UltraTextEditor
                                {
                                    Tag = new CustomFieldUIInfo { CustomFieldID = customField.CustomFieldID },
                                    Dock = DockStyle.Fill,
                                    MaxLength = customFieldMaxLength,
                                    Margin = new Padding(2, 0, 28, 2),
                                    AcceptsReturn = true,
                                    Multiline = true,
                                    Scrollbars = ScrollBars.Both,
                                    Height = 35,
                                    WordWrap = false
                                };

                            }
                            else
                            {
                                control = new UltraComboEditor()
                                {
                                    Tag = new CustomFieldUIInfo { CustomFieldID = customField.CustomFieldID },
                                    Dock = DockStyle.Fill,
                                    MaxLength = customFieldMaxLength,
                                    Margin = new Padding(2, 0, 2, 2),
                                    Padding = new Padding(0),
                                    DropDownStyle = DropDownStyle.DropDownList
                                };
                            }

                            if (!customField.IsDescriptionNull())
                            {
                                tipManager.SetUltraToolTip(control, new UltraToolTipInfo(customField.Description, ToolTipImage.Info, customField.Name, DefaultableBoolean.True));
                            }

                            if (customField.Required)
                            {
                                var textValidator = new TextControlValidator(control, $"{customField.Name}  is required.")
                                {
                                    PreserveWhitespace = !customField.IsListIDNull()
                                };

                                var validator = new ImageDisplayValidator(textValidator, _errProvider);
                                ((CustomFieldUIInfo)control.Tag).Validator = validator;
                                _validationManager.Add(validator);
                            }

                            tableCustomFields.Controls.Add(control, 1, tableRowCount);

                            tableRowCount++;
                        }
                    }
                }

                this.tableCustomFields.RowCount = tableRowCount;

                var tableHeight = tableRowCount > 0 ? this.tableCustomFields.Height : 0;

                this.pnlCustomFields.Height = tableHeight;
                this.pnlCustomFields.Visible = tableRowCount > 0;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error setting blanked PO.";
                _log.Error(exc, errorMsg);
            }
        }

        private void FixMinimumSize()
        {
            try
            {
                var defaultMin = FirstMinimumSize ?? MinimumSize;

                var customFieldHeight = pnlCustomFields.Visible ? pnlCustomFields.Height : 0;
                var repairStatementFieldsHeight = pnlRepairStatementFields.Visible ? pnlRepairStatementFields.Height : 0 ;
                var productClassHeight = pnlProductClass.Visible ? pnlProductClass.Height : 0;
                var serialHeight = pnlBasicSerialNumber.Visible ? pnlBasicSerialNumber.Height : 0;
                var adjustedDateHeight = pnlAdjustedShipDate.Visible ? pnlAdjustedShipDate.Height : 0;

                var heightOffset = customFieldHeight + repairStatementFieldsHeight +
                    productClassHeight + serialHeight + adjustedDateHeight;

                MinimumSize = new Size(MinimumSize.Width, defaultMin.Height + heightOffset);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error resizing Order Information.");
            }
        }

        /// <summary>
        ///     Bind the orders values to the custom fields
        /// </summary>
        /// <param name="customerChanged"> if true, then ensure any previous custom fields for this customer are deleted </param>
        private void BindCustomFields(bool customerChanged)
        {
            try
            {
                _log.Debug("Binding custom fields.");

                if(Dataset != null)
                {
                    //find current customer and order
                    var order = CurrentRecord as OrdersDataSet.OrderRow;

                    if(order != null)
                    {
                        //if customer changed then ensure old custom fields for this order that are used by the other customer are removed
                        //NOTE: If you delete an item then add it back before a save, then DB will through a duplicate key error
                        if(customerChanged)
                        {
                            var orderCustomFields = order.GetOrderCustomFieldsRows();
                            foreach(var ocf in orderCustomFields)
                            {
                                if(ocf.CustomFieldRow != null && ocf.CustomFieldRow.CustomerID != order.CustomerID)
                                    ocf.Delete();
                            }
                        }

                        foreach(Control control in this.tableCustomFields.Controls)
                        {
                            var fieldInfo = control.Tag as CustomFieldUIInfo;
                            if (fieldInfo == null)
                            {
                                continue;
                            }

                            int customFieldID = fieldInfo.CustomFieldID;
                            var customField = Dataset.CustomField.FindByCustomFieldID(customFieldID);

                            var defaultValue = (order.OrderID > 0 && !customerChanged) || customField.IsDefaultValueNull() ?
                                string.Empty :
                                customField.DefaultValue;

                            //Only enable the validation if the order was created after relase date of this version. Can remove in the future after existing orders are all processed.
                            if(fieldInfo.Validator != null && fieldInfo.Validator.Validator is TextControlValidator)
                                ((TextControlValidator) fieldInfo.Validator.Validator).IsEnabled = !order.IsOrderDateNull() && order.OrderDate >= new DateTime(2012, 4, 30);

                            var orderCustomField = Dataset.OrderCustomFields.FindByOrderIDCustomFieldID(order.OrderID, customField.CustomFieldID);
                            if(orderCustomField == null)
                            {
                                //check to see if it was deleted earlier, then added back
                                var deletedCustomField = Dataset.OrderCustomFields
                                    .Select($"OrderID = {order.OrderID} AND CustomFieldID = {customField.CustomFieldID}", string.Empty, DataViewRowState.Deleted)
                                    .FirstOrDefault() as OrdersDataSet.OrderCustomFieldsRow;

                                if (deletedCustomField != null)
                                {
                                    _log.Info($"Reversing custom field deletion for OrderID = {order.OrderID}, CustomFieldID = {customField.CustomFieldID}");
                                    deletedCustomField.RejectChanges();
                                    orderCustomField = deletedCustomField;
                                }
                                else
                                {
                                    orderCustomField = Dataset.OrderCustomFields.AddOrderCustomFieldsRow(order,
                                        customField, defaultValue);
                                }
                            }
                            else if (orderCustomField.IsValueNull())
                            {
                                orderCustomField.Value = defaultValue;
                            }

                            var comboEditor = control as UltraComboEditor;
                            if (comboEditor != null && !customField.IsListIDNull())
                            {
                                // Bind list contents
                                comboEditor.Items.Clear();
                                comboEditor.BeginUpdate();
                                comboEditor.Items.Add(string.Empty);
                                var listValues = Dataset.ListValues.Select($"ListID = {customField.ListID}");

                                var addCurrentValue = !orderCustomField.IsValueNull() &&
                                    !string.IsNullOrEmpty(orderCustomField.Value) &&
                                    listValues.All(v => v["Value"].ToString() != orderCustomField.Value);

                                if (addCurrentValue)
                                {
                                    comboEditor.Items.Add(orderCustomField.Value);
                                }

                                foreach (var row in listValues)
                                {
                                    comboEditor.Items.Add(row[Dataset.ListValues.ValueColumn.ColumnName]);
                                }

                                comboEditor.EndUpdate();
                            }

                            control.DataBindings.Clear();

                            // Add text binding - also works for combo editor
                            control.DataBindings.Add(new Binding("Text", orderCustomField, "Value", true)
                            {
                                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged,
                                NullValue = string.Empty
                            });
                        }
                    }
                }

                //ensure panel is hidden if it doesn't have any controls.
                if(this.pnlCustomFields.Visible && this.tableCustomFields.Controls.Count < 1)
                    this.pnlCustomFields.Visible = false;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error setting blanked PO.";
                _log.Error(exc, errorMsg);
            }
        }

        private void DisplayPriceHistory()
        {
            try
            {
                if(IsDisposed)
                    return;

                this.lvwPriceHistory.Items.Clear();

                if(this.cboPart.SelectedItem != null)
                {
                    using(var taHistory = new Get_OrderPriceHistoryTableAdapter())
                    {
                        using(OrdersDataSet.Get_OrderPriceHistoryDataTable historyTable = taHistory.GetData(Convert.ToInt32(this.cboPart.SelectedItem.DataValue)))
                        {
                            var or = CurrentRecord as OrdersDataSet.OrderRow;
                            int orderID = -1;
                            if(or != null)
                                orderID = or.OrderID;

                            //SELECT OrderID, OrderDate, BasePrice, PriceUnit, PartQuantity FROM [Order]
                            foreach(var historyRow in historyTable)
                            {
                                if (historyRow.OrderID != orderID)
                                {
                                    var priceUnit = this.Dataset.PriceUnit.FindByPriceUnitID(historyRow.PriceUnit);
                                    string priceUnitDisplayName = priceUnit == null ? historyRow.PriceUnit : priceUnit.DisplayName;

                                    var subItems = new object[]
                                    {
                                        historyRow.OrderDate.ToShortDateString(),
                                        historyRow.IsBasePriceNull() ? null : historyRow.BasePrice.ToString(OrderPrice.CurrencyFormatString),
                                        priceUnitDisplayName,
                                        historyRow.IsPartQuantityNull() ? (int?)null : historyRow.PartQuantity
                                    };

                                    this.lvwPriceHistory.Items.Add(new UltraListViewItem(historyRow.OrderID, subItems));
                                }
                            }

                            this.lvwPriceHistory.PerformLayout();
                        }
                    }

                    this.popPriceHistory.Show(new PopupInfo {Owner = this.numUnitPrice, ExclusionRect = this.numUnitPrice.RectangleToScreen(this.numUnitPrice.DisplayRectangle), PreferredLocation = this.numUnitPrice.PointToScreen(new Point(this.numUnitPrice.DisplayRectangle.Right + 15, this.numUnitPrice.Top)), Position = DropDownPosition.RightOfExclusionRect});
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error displaying price history.";
                _log.Error(exc, errorMsg);
            }
        }

        protected override void OnDispose()
        {
            if(Dataset != null && Dataset.Order != null)
                Dataset.Order.ColumnChanged -= Order_ColumnChanged;

            this._filesToDelete = null;
            this._customerFields = null;

            if(this._validationManager != null)
            {
                this._validationManager.Dispose();
                this._validationManager = null;
            }

            if (this._taPartSummary != null)
            {
                this._taPartSummary.Dispose();
            }

            base.OnDispose();
        }

        public void AddPartProcesses(OrdersDataSet.OrderRow order)
        {
            if (order == null || !Enabled)
            {
                return;
            }

            _log.Info("Adding order processes to the order.");

            var orderProcessRows = order.GetOrderProcessesRows();

            //if contains existing process rows then nothing to do
            if(orderProcessRows.Length > 0)
                return;

            if(order.IsPartIDNull())
                return;

            //Find Part Processes
            OrdersDataSet.PartProcessSummaryDataTable processes;
            using (var taPartProcessSummary = new PartProcessSummaryTableAdapter())
            {
                processes = taPartProcessSummary.GetData(order.PartID);
            }

            AddProcesses(order, processes.Select(Process.From).ToList());
        }

        public void AddQuotePartProcesses(OrdersDataSet.OrderRow order, int quotePartId)
        {
            if (order == null || !Enabled)
                return;

            _log.Info("Adding quote part processes to the order.");

            var orderProcessRows = order.GetOrderProcessesRows();

            //if contains existing process rows then nothing to do
            if (orderProcessRows.Length > 0)
                return;

            if (order.IsPartIDNull())
                return;

            //Find Quote Part Processes
            QuoteDataSet.QuotePart_ProcessDataTable processes = new QuoteDataSet.QuotePart_ProcessDataTable();
            using (var taQuotePartProcess = new QuotePart_ProcessTableAdapter())
            {
                taQuotePartProcess.FillByQuotePart(processes, quotePartId);
            }

            AddProcesses(order, processes.Select(Process.From).ToList());

        }

        public void AddQuotePartFeesToOrder(OrdersDataSet.OrderRow order, int quotePartId)
        {
            using (var quotePartFeesTa = new QuotePartFeesTableAdapter())
            {
                var feesDataTable = quotePartFeesTa.GetByQuotePartId(quotePartId);
                if (feesDataTable.Rows.Count == 0) return;

                foreach (QuoteDataSet.QuotePartFeesRow feeRecord in feesDataTable.Rows)
                {
                    var feeType = Dataset.OrderFeeType.FindByOrderFeeTypeID(feeRecord.FeeType) ??
                                  AddQuoteFeeTypeToOrderFeeTypeTable(feeRecord);

                    if (feeType == null) continue; //If feetype is still null, errors occurred adding the quote fee type to the order fee type table, most likely a PK Unique constraint violation

                    var feeRow = Dataset.OrderFees.NewOrderFeesRow();
                    feeRow.OrderID = order.OrderID;
                    feeRow.Charge = feeRecord.Charge;
                    feeRow.OrderFeeTypeID = feeType.OrderFeeTypeID;
                    Dataset.OrderFees.AddOrderFeesRow(feeRow);
                }
            }
        }

        private OrdersDataSet.OrderFeeTypeRow AddQuoteFeeTypeToOrderFeeTypeTable(QuoteDataSet.QuotePartFeesRow feeRecord)
        {
            try
            {
                OrdersDataSet.OrderFeeTypeRow feeTypeRow = Dataset.OrderFeeType.NewOrderFeeTypeRow();
                using (var taOrderFeeType = new OrderFeeTypeTableAdapter())
                {
                    feeTypeRow.OrderFeeTypeID = feeRecord.FeeType;
                    feeTypeRow.Price = feeRecord.Charge;
                    feeTypeRow.FeeType = feeRecord.FeeCalculationType;
                    feeTypeRow.InvoiceItemName = "Processing Fee";
                    Dataset.OrderFeeType.Rows.Add(feeTypeRow);
                    if (taOrderFeeType.Update(Dataset.OrderFeeType) == 1)
                        return feeTypeRow;
                }
            }
            catch (Exception exc)
            {
                _log.Error((exc, $"An error occurred trying to add a new fee type during import from quote."));
                MessageBox.Show(
                    $"An error occurred transferring fee type {feeRecord.FeeType} to the new order. This was likely due to a naming conflict.");
            }

            return null;
        }

        private void AddProcesses(OrdersDataSet.OrderRow order, ICollection<Process> processes, int startingStep = 0)
        {
            ProcessRequisiteTableAdapter taProcessRequisite = null;
            PartProcessSummaryTableAdapter taPartProcessSummary = null;

            try
            {
                taProcessRequisite = new ProcessRequisiteTableAdapter();
                taPartProcessSummary = new PartProcessSummaryTableAdapter();

                var pastProcesses = new List<int>();

                var processLeadTimes = new OrderProcessLeadTimes();
                foreach (var process in processes.OrderBy(p => p.StepOrder))
                {
                    var orderProcess = Dataset.OrderProcesses.NewOrderProcessesRow();
                    orderProcess.OrderID = order.OrderID;
                    orderProcess.ProcessID = process.ProcessId;
                    orderProcess.ProcessAliasID = process.ProcessAliasId;
                    orderProcess.StepOrder = startingStep + process.StepOrder;
                    orderProcess.Department = process.Department;
                    orderProcess.COCData = (process.CocCount.HasValue && process.CocCount > 0) || ApplicationSettings.Current.DisplayProcessCOCByDefault; //only default on COC if COC Count exists 

                    orderProcess.OrderProcessType = (int)OrderProcessType.Normal;

                    // Load Capacity
                    if (process.LoadCapacityVariance.HasValue)
                    {
                        orderProcess.LoadCapacityVariance = process.LoadCapacityVariance.Value;
                    }

                    bool hasCapacity = false;

                    if (process.LoadCapacityWeight.HasValue)
                    {
                        orderProcess.LoadCapacityWeight = process.LoadCapacityWeight.Value;
                        hasCapacity = true;
                    }

                    if (process.LoadCapacityQuantity.HasValue)
                    {
                        orderProcess.LoadCapacityQuantity = process.LoadCapacityQuantity.Value;
                        hasCapacity = true;
                    }

                    if (!hasCapacity)
                    {
                        // Check process
                        var weight = taPartProcessSummary.GetLoadCapacityWeight(process.ProcessId);

                        if (weight.HasValue)
                        {
                            orderProcess.LoadCapacityWeight = weight.Value;
                        }
                        else
                        {
                            var qtyDecimal = taPartProcessSummary.GetLoadCapacityQuantity(process.ProcessId);

                            if (qtyDecimal.HasValue)
                            {
                                var qty = (int)Math.Round(qtyDecimal.Value, MidpointRounding.AwayFromZero);
                                orderProcess.LoadCapacityQuantity = qty;
                            }
                        }
                    }

                    if (pastProcesses.Count > 0)
                    {
                        var processReqTable = new ProcessesDataset.ProcessRequisiteDataTable();
                        taProcessRequisite.FillByParent(processReqTable, process.ProcessId);

                        //Add process pre requisites to the order process if they exist
                        var processReqs = processReqTable.Where(p => pastProcesses.Contains(p.ChildProcessID));

                        if(processReqs.Any())
                        {
                            var lowestReq = processReqs.OrderByDescending(r => r.Hours).FirstOrDefault();
                            orderProcess.RequisiteProcessID = lowestReq.ChildProcessID;
                            orderProcess.RequisiteHours = lowestReq.Hours;
                        }
                    }

                    Dataset.OrderProcesses.AddOrderProcessesRow(orderProcess);

                    //add list of processes already added
                    if(!pastProcesses.Contains(process.ProcessId))
                        pastProcesses.Add(process.ProcessId);

                    // Add lead times
                    if (process.PartProcessRow != null)
                    {
                        processLeadTimes.Add(orderProcess.OrderProcessesID, process.PartProcessRow);
                    }
                    else if (process.ProcessRow != null)
                    {
                        processLeadTimes.Add(orderProcess.OrderProcessesID, process.ProcessRow);
                    }
                }

                UpdateLeadTimes(order, processLeadTimes);

                _log.Info("Added {0} order processes to order {1}.", processes.Count, order.OrderID);
            }
            finally
            {
                taPartProcessSummary?.Dispose();
                taProcessRequisite?.Dispose();
            }
        }

        /// <summary>
        /// Updates the order's lead times and estimated ship date if using
        /// lead time scheduling.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="processLeadTimes"></param>
        private void UpdateLeadTimes(OrdersDataSet.OrderRow order, OrderProcessLeadTimes processLeadTimes)
        {
            var scheduler = _schedulerInstance.Value;
            if (scheduler == null)
            {
                return;
            }

            var estShipDate = scheduler.UpdateScheduleDates(order, processLeadTimes);
            var currentEstShipDate = dtShipDate.Value as DateTime?;

            var updateEstShipDate = estShipDate != currentEstShipDate;

            if (updateEstShipDate && !_autoUpdateEstShipDate)
            {
                // Prompt user before updating the estimated ship date
                const string msgHeader = "Est. Ship Date";

                string updateShipDateMsg = string.Format(
                    "Would you like to update the estimated shipping date to {0:MM/dd/yyyy}?",
                    estShipDate);

                updateEstShipDate = MessageBoxUtilities.ShowMessageBoxYesOrNo(updateShipDateMsg, msgHeader) == DialogResult.Yes;
            }

            if (updateEstShipDate)
            {
                order.EstShipDate = estShipDate;
                dtShipDate.DataBindings[0].ReadValue();
            }
        }

        private void RemoveOrderProcesses(OrdersDataSet.OrderRow order)
        {
            if (order == null || !Enabled)
            {
                return;
            }

            _log.Info("Removing order processes to the order.");

            var orderProcessRows = order.GetOrderProcessesRows();

            if (orderProcessRows.Length > 0)
            {
                foreach (var opr in orderProcessRows)
                {
                    opr.Delete();
                }

                _log.Info($"Removed {orderProcessRows.Length} order processes from order {order.OrderID}.");
            }
        }

        private DateTime? GetStartDateOfFirstProcess(OrdersDataSet.OrderRow order)
        {
            if (order == null)
            {
                return null;
            }

            var firstOrderProcess = order.GetOrderProcessesRows()
                .OrderBy(op => op.StepOrder)
                .FirstOrDefault();

            if (firstOrderProcess != null && !firstOrderProcess.IsStartDateNull())
            {
                return firstOrderProcess.StartDate;
            }

            return null;
        }

        public void AddOrderPartMark(OrdersDataSet.OrderRow order)
        {
            try
            {
                if (order == null || order.IsPartIDNull())
                {
                    return;
                }

                //if no part loaded then load yourself
                if (order.PartSummaryRow == null)
                {
                    _log.Info("Order {0} is missing part {1}, loading manually.", order.OrderID, order.PartID);
                    _taPartSummary.FillByPart(Dataset.PartSummary, order.PartID);
                }

                //if couldnt find part then an issue
                if (order.PartSummaryRow == null)
                {
                    _log.Info("Unable to load part " + order.PartID);
                    return;
                }

                _log.Info("Order part {0} has part marking {1}.", order.PartSummaryRow.PartID, order.PartSummaryRow.PartMarking);
                var existingOrderPartMark = order.GetOrderPartMarkRows().FirstOrDefault(); //NOTE: should only be one but historical orders may have more than one

                if (order.PartSummaryRow.PartMarking)
                {
                    _log.Info("Adding order {0} part mark to the order.", order.OrderID);

                    PartsDataset.Part_PartMarkingRow markFromPart;
                    using (var taPartMark = new Part_PartMarkingTableAdapter())
                    {
                        markFromPart = taPartMark.GetDataByPartID(order.PartID).FirstOrDefault();
                    }

                    if (markFromPart != null)
                    {
                        // Use part mark from part

                        // Delete old part mark
                        if (existingOrderPartMark != null)
                        {
                            BeforeChildRowDeleted?.Invoke(OrderChildRowType.PartMark, existingOrderPartMark);

                            existingOrderPartMark.Delete();
                        }

                        // Add new part mark
                        _log.Info($"Order creating new part marking {markFromPart.Part_PartMarkingID} from part.");

                        var newPartMark = Dataset.OrderPartMark.NewOrderPartMarkRow();

                        newPartMark.OrderRow = order;
                        newPartMark.ProcessSpec = markFromPart.IsProcessSpecNull() ? null : markFromPart.ProcessSpec;
                        newPartMark.Line1 = markFromPart.IsDef1Null() ? null : markFromPart.Def1;
                        newPartMark.Line2 = markFromPart.IsDef2Null() ? null : markFromPart.Def2;
                        newPartMark.Line3 = markFromPart.IsDef3Null() ? null : markFromPart.Def3;
                        newPartMark.Line4 = markFromPart.IsDef4Null() ? null : markFromPart.Def4;

                        Dataset.OrderPartMark.AddOrderPartMarkRow(newPartMark);
                        AfterChildRowAdded?.Invoke(OrderChildRowType.PartMark, newPartMark);
                    }
                    else
                    {
                        // Use part mark from model -or- use empty part mark
                        int? oldPartMarkId = null;
                        int? newPartMarkId = null;
                        OrderProcessingDataSet.PartMarkingRow partMarkForOrder = null;

                        //get existing part mark id
                        if (existingOrderPartMark != null && !existingOrderPartMark.IsPartMarkingIDNull())
                            oldPartMarkId = existingOrderPartMark.PartMarkingID;

                        using (var taPM = new Data.Datasets.OrderProcessingDataSetTableAdapters.PartMarkingTableAdapter())
                        {
                            var partMarkingRow = taPM.GetDataByPart(order.PartID).FirstOrDefault();

                            if (partMarkingRow != null)
                            {
                                partMarkForOrder = partMarkingRow;
                                newPartMarkId = partMarkingRow.PartMarkingID;
                            }
                        }

                        //if part mark id changed OR don't have a part mark id but part says to do one any ways then need blank template
                        if (oldPartMarkId != newPartMarkId || !newPartMarkId.HasValue)
                        {
                            //delete old part mark
                            if (existingOrderPartMark != null)
                            {
                                BeforeChildRowDeleted?.Invoke(OrderChildRowType.PartMark, existingOrderPartMark);

                                existingOrderPartMark.Delete();
                            }

                            //create new part mark
                            if (partMarkForOrder != null)
                            {
                                _log.Info("Order creating new part marking {0}.", partMarkForOrder.PartMarkingID);

                                var newPartMark = Dataset.OrderPartMark.NewOrderPartMarkRow();

                                newPartMark.OrderRow = order;
                                newPartMark.PartMarkingID = partMarkForOrder.PartMarkingID;
                                newPartMark.ProcessSpec = partMarkForOrder.ProcessSpec;
                                newPartMark.Line1 = partMarkForOrder.IsDef1Null() ? null : partMarkForOrder.Def1;
                                newPartMark.Line2 = partMarkForOrder.IsDef2Null() ? null : partMarkForOrder.Def2;
                                newPartMark.Line3 = partMarkForOrder.IsDef3Null() ? null : partMarkForOrder.Def3;
                                newPartMark.Line4 = partMarkForOrder.IsDef4Null() ? null : partMarkForOrder.Def4;

                                Dataset.OrderPartMark.AddOrderPartMarkRow(newPartMark);


                                AfterChildRowAdded?.Invoke(OrderChildRowType.PartMark, newPartMark);
                            }
                            else //if need to add a blank template because Part says use part marking but has no template
                            {
                                var newPartMark = Dataset.OrderPartMark.NewOrderPartMarkRow();
                                newPartMark.OrderRow = order;

                                _log.Info("Order creating new blank part marking.");

                                Dataset.OrderPartMark.AddOrderPartMarkRow(newPartMark);
                                AfterChildRowAdded?.Invoke(OrderChildRowType.PartMark, newPartMark);
                            }
                        }
                    }
                }
                else if (existingOrderPartMark != null)
                {
                    _log.Info($"Removing part mark for order {order.OrderID}.");
                    BeforeChildRowDeleted?.Invoke(OrderChildRowType.PartMark, existingOrderPartMark);
                    existingOrderPartMark.Delete();
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error adding order part mark.");
            }
        }

        private bool CurrentPartHasPricingIssue(int partID)
        {
            var skipPriceIssueCheck = ApplicationSettings.Current.PartPricingType != PricingType.Process ||
                !ApplicationSettings.Current.ProcessPriceWarningEnabled ||
                !IsNewRow.GetValueOrDefault();

            if (skipPriceIssueCheck)
            {
                return false;
            }

            int processesMissingData;
            int invalidPriceDataCount;
            using (var taPartProcessPrice = new PartProcessPriceSummaryTableAdapter())
            {
                processesMissingData = taPartProcessPrice.GetProcessesWithoutPriceCount(partID, _priceUnitPersistence.PriceByTypes());
                invalidPriceDataCount = taPartProcessPrice.GetInvalidVolumePriceCount(partID) ?? 0;
            }

            return processesMissingData > 0 || invalidPriceDataCount > 0;
        }

        private static ILeadTimeScheduler NewSchedulerInstance()
        {
            if (ApplicationSettings.Current.SchedulingEnabled)
            {
                if (ApplicationSettings.Current.SchedulerType == SchedulerType.ProcessLeadTime)
                {
                    var scheduler = new RoundingLeadTimeScheduler();
                    scheduler.LoadData();
                    return scheduler;
                }
                else if (ApplicationSettings.Current.SchedulerType == SchedulerType.ProcessLeadTimeHour)
                {
                    var scheduler = new LeadTimeScheduler();
                    scheduler.LoadData();
                    return scheduler;
                }
            }

            return null;
        }

        private static ApplicationSettingsDataSet.FieldsDataTable GetFieldSettings()
        {
            ApplicationSettingsDataSet.FieldsDataTable fields;

            using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
            {
                fields = ta.GetByCategory("Order");
            }

            return fields;
        }
        
        private void SelectPartUsingManager()
        {
            SelectPartUsingManager(0);
        }
        
        private void SelectPartUsingManager(int QuotePartID)
        {
            try
            {
                this._isInPartEditor = true;

                //clicked on add new part
                if(base.CurrentRecord is OrdersDataSet.OrderRow)
                {
                    var order = (OrdersDataSet.OrderRow) base.CurrentRecord;

                    using(var p = new PartManager())
                    {
                        p.CustomerFilterID = CurrentCustomerID;

                        if (this.cboPart.SelectedItem != null)
                        {
                            int partID = Convert.ToInt32(this.cboPart.SelectedItem.DataValue);
                            p.SelectedPartID = Convert.ToInt32(this.cboPart.SelectedItem.DataValue);
                            p.AutoSyncProcessPrices = CurrentPartHasPricingIssue(partID);
                        }
                        else if (!string.IsNullOrEmpty(this.cboPart.Text))
                        {
                            p.InitialPartFilterText = this.cboPart.Text; //filter by part name
                        }
                        if (QuotePartID > 0)
                        { }
                        p.AllowInActiveSelectedPart = false;

                        p.QuotePartID = QuotePartID;
                        var dialogResult = p.ShowDialog(this);

                        if(dialogResult == DialogResult.OK || p.DataChanged) //use data changed in case user clicked Apply then Cancel ELSE we wont pick up the change by using DialogResult.OK
                        {
                            //if data was changed then reload
                            if(p.DataChanged)
                            {
                                LogManager.GetCurrentClassLogger().Warn("Part data was changed so reload.");
                                Dataset.PartSummary.Clear(); //update parts table
                                this._taPartSummary.FillByCustomerActive(Dataset.PartSummary, CurrentCustomerID);

                                //let order entry know that the parts have been reloaded
                                if(PartsReloaded != null)
                                    PartsReloaded();

                                //rebind to ensure refreshes data
                                this.cboPart.DataBind();
                            }

                            //check to see if part changed
                            var partSelectionChanged = p.SelectedPartID >= 0 && Editable && (order.IsPartIDNull() || order.PartID != p.SelectedPartID);

                            if(partSelectionChanged)
                                order.PartID = p.SelectedPartID;

                            LoadCurrentPart(); //ensure selected part loaded (in case part is not active for some reason)
                            this.cboPart.DataBindings[0].ReadValue(); //re-read value to ensure is selected in cbo

                            //if part changed or processes changed add the processes
                            if(Editable && (p.PartProcessesChanged > 0 || partSelectionChanged))
                            {
                                var startDate = GetStartDateOfFirstProcess(order);
                                RemoveOrderProcesses(order);
                                AddPartProcesses(order);

                                // If part is changed prior to processing but after check-in,
                                // restore the start date for the first process.
                                // See #29827.
                                if (startDate.HasValue)
                                {
                                    var firstProcess = order.GetOrderProcessesRows().FirstOrDefault();

                                    if (firstProcess != null)
                                    {
                                        firstProcess.StartDate = startDate.Value;
                                    }
                                }

                                //if part changed then update part mark
                                if(partSelectionChanged)
                                    AddOrderPartMark(order);
                            }

                            UpdateAddProcessPackageEnabled(order);
                        }

                        if(this.cboPart.SelectedItem != null && order.OrderType != (int) OrderType.ReworkExt)
                        {
                            var partRow = ((DataRowView) this.cboPart.SelectedItem.ListObject).Row as OrdersDataSet.PartSummaryRow;

                            int partQty = 0;
                            if (this.numPartQty.Value != DBNull.Value)
                            {
                                partQty = Convert.ToInt32(this.numPartQty.Value);
                            }

                            decimal weight = this.numWeight.Value == DBNull.Value ? 0M : Convert.ToDecimal(this.numWeight.Value);
                            var currentPriceUnit = OrderPrice.ParsePriceUnit(this.cboUnit.Text);

                            if(partRow != null)
                            {
                                // Pricing - update the base price and unit for the Work Order.
                                // Update is automatic if the WO does not have a base price.
                                (var newPrice, var priceUnit) = GetDefaultPrice(partRow, partQty, weight, currentPriceUnit);

                                if(newPrice > 0)
                                {
                                    _log.Debug("The selected part has pricing info.");

                                    //if the part price or unit is different from the current price or unit ask the user if they want to update
                                    if(this.numUnitPrice.Value > 0 && (newPrice != this.numUnitPrice.Value || priceUnit.ToString() != this.cboUnit.Text))
                                    {
                                        var dlgResult = MessageBoxUtilities.ShowMessageBoxYesOrNo("Would you like to update the price and unit with the price and unit of the selected part?", "Update Unit Price and Unit");
                                        _log.Debug("User was asked if they wanted to update price and unit with that of the selected part.");

                                        if(dlgResult == DialogResult.Yes)
                                        {
                                            _log.Debug("User selected Yes to question.");
                                            //set price to part price
                                            UpdateDefaultPrice();
                                        }
                                        else
                                            _log.Debug("User selected No to question.");
                                    }
                                    else
                                    {
                                        _log.Debug("Automatically updating price using selected part.");
                                        UpdateDefaultPrice();
                                    }
                                }
                                else
                                {
                                    _log.Debug("The selected part does not have pricing info.");
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error adding a new part.", exc);
            }
            finally
            {
                this._isInPartEditor = false;
            }
        }

        /// <summary>
        /// Shows an 'add process package' dialog.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the dialog added any processes; otherwise, <c>false</c>.
        /// </returns>
        private bool AddProcessPackageFromDialog()
        {
            ProcessPackageDataset.ProcessPackage_ProcessesDataTable dtProcessPackageProcess = null;

            try
            {
                dtProcessPackageProcess = new ProcessPackageDataset.ProcessPackage_ProcessesDataTable();

                var order = CurrentRecord as OrdersDataSet.OrderRow;
                if (order == null)
                {
                    return false;
                }

                int? processPackageId = null;
                using (var processPackageManager = new ProcessPackages())
                {
                    if (processPackageManager.ShowDialog(ParentForm) == DialogResult.OK)
                    {
                        processPackageId = processPackageManager.SelectedProcessPackage?.ProcessPackageID;
                    }
                }

                if (processPackageId.HasValue)
                {
                    using (var taProcessPackageProcess = new Data.Datasets.ProcessPackageDatasetTableAdapters.ProcessPackage_ProcessesTableAdapter())
                    {
                        taProcessPackageProcess.FillByProcessPackageID(dtProcessPackageProcess, processPackageId.Value);
                    }

                    var processes = dtProcessPackageProcess.OfType<ProcessPackageDataset.ProcessPackage_ProcessesRow>().Select(Process.From).ToList();

                    var startingStep = order.GetOrderProcessesRows().OrderBy(s => s.StepOrder).LastOrDefault()?.StepOrder ?? 0;

                    AddProcesses(order, processes, startingStep);

                    return processes.Count > 0;
                }

                return false;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding process package to order.");
                return false;
            }
        }

        #endregion

        #region Events

        private void cboPart_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                switch (e.Button.Key)
                {
                    case "SelectPart":
                        SelectPartUsingManager();
                        break;
                    case "AddProcessPackage":
                        AddProcessPackageFromDialog();
                        break;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error clicking cboPart's editor button");
            }
        }

        private void curFeesTotal_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                int orderId = _OrderID;
                //int.TryParse(txtOrderID.Text, out orderId);

                using(var of = new OrderFees())
                {
                    of.OrderID = orderId;
                    of.OrdersDataset = Dataset;
                    of.IsQuickView = this.IsQuickView;
                    of.ShowDialog(this);

                    UpdateOrderCost(); //fee editor

                    var or = base.CurrentRecord as OrdersDataSet.OrderRow;
                    var ofRows = or.GetOrderFeesRows();

                    bool isRush = false;
                    decimal rushCharge = 0;

                    foreach(var ofRow in ofRows)
                    {
                        if(ofRow.OrderFeeTypeID == Properties.Settings.Default.OrderPriorityExpedite)
                        {
                            isRush = true;
                            rushCharge = ofRow.Charge;
                        }
                    }

                    //ask if user wants to set order to expedite
                    if(isRush && or.Priority == Properties.Settings.Default.OrderPriorityDefault)
                    {
                        if(MessageBoxUtilities.ShowMessageBoxYesOrNo("The current order has a 'Expedite' fee, but the priority is still 'Normal'.\r\n\r\nDo you want to set the priority to 'Expedite'?", "Incorrect Priority") == DialogResult.Yes)
                        {
                            or.Priority = Properties.Settings.Default.OrderPriorityExpedite;
                            this.cboPriority.DataBindings[0].ReadValue();
                        }
                    }

                    //ask user why amount is less than required for Expedite
                    if(isRush && rushCharge < Properties.Settings.Default.MinimumOrderRushCharge)
                        MessageBoxUtilities.ShowMessageBoxWarn("The current order has an 'Expedite' fee of " + rushCharge.ToString(OrderPrice.CurrencyFormatString) + ", which is less than than the minimum amount.\r\n\r\nPlease correct this to meet the minimum amount of " + Properties.Settings.Default.MinimumOrderRushCharge.ToString(OrderPrice.CurrencyFormatString) + ".", "Below Minimum Priority Charge");
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error editing fees.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
                _log.Fatal(exc, errorMsg);
            }
        }

        private void numUnitPrice_ValueChanged(object sender, EventArgs e)
        {
            if (!this._newOrderBeingCreated)
            {
                if (base.CurrentRecord is OrdersDataSet.OrderRow orderRow)
                    base.CurrentRecord["Baseprice"] = numUnitPrice.Value;
                UpdateOrderCost(); //Price change
            }
        }

        private void OnPriceUnit_ValueChanged(object sender, EventArgs e)
        {

            // While saving, cboUnit.Value may be temporarily null
            if (cboUnit.Value != null && !this._newOrderBeingCreated)
            {
                if (base.CurrentRecord is OrdersDataSet.OrderRow orderRow)
                {
                    base.CurrentRecord["PriceUnit"] = cboUnit.Value.ToString();

                    if (!orderRow.IsQuotePartIdNull())
                        UpdateUnitPriceByQuotePart();
                    else
                        UpdateUnitPrice(); // Make sure the unit price relates to the unit selected
                }

                UpdateOrderCost(); //unit change
            }
        }

        private void cboPart_SelectionChanged(object sender, EventArgs e)
        {
            var partQty = 0;

            try
            {
                //if this is a current item and the user is changing the part
                var userChangedPart = !PartsLoading
                    && _panelLoaded
                    && !_recordLoading
                    && cboPart.SelectedItem != null
                    && !_isInPartEditor
                    && !_newOrderBeingCreated;

                if (userChangedPart && Editable)
                {
                    var partRow = ((DataRowView)this.cboPart.SelectedItem.ListObject).Row as OrdersDataSet.PartSummaryRow;
                    var order = CurrentRecord as OrdersDataSet.OrderRow;

                    if (partRow != null && order != null && (order.IsPartIDNull() || order.PartID != partRow.PartID))
                    {
                        if (!order.IsQuotePartIdNull())
                            order.SetQuotePartIdNull();

                        //update partid on order so right processes will be added after you manually removed them
                        var startDate = GetStartDateOfFirstProcess(order);

                        RemoveOrderProcesses(order);
                        order.PartID = partRow.PartID;
                        AddPartProcesses(order);

                        // If part is changed prior to processing but after check-in,
                        // restore the start date for the first process.
                        // See #29827.
                        if (startDate.HasValue)
                        {
                            var firstProcess = order.GetOrderProcessesRows().FirstOrDefault();

                            if (firstProcess != null)
                            {
                                firstProcess.StartDate = startDate.Value;
                            }
                        }

                        AddOrderPartMark(order);
                        UpdateAddProcessPackageEnabled(order);

                        // Use Require COC from part only if the customer
                        // doesn't require COC by default.
                        if (!(CurrentCustomer?.RequireCOCByDefault ?? false))
                        {
                            chkRequireCoc.Checked = partRow.RequireCocByDefault;
                        }
                    }

                    if(!partRow.IsLotPriceNull() || !partRow.IsEachPriceNull())
                    {
                        // Pricing - Update default pricing using the selected part.
                        if(this.numPartQty.Value != DBNull.Value)
                            partQty = Convert.ToInt32(this.numPartQty.Value);

                        var currentPriceUnit = OrderPrice.ParsePriceUnit(this.cboUnit.Text);
                        decimal weight = this.numWeight.Value == DBNull.Value ? 0M : Convert.ToDecimal(this.numWeight.Value);

                        (var newPrice, var priceUnit) = GetDefaultPrice(partRow, partQty, weight, currentPriceUnit);

                        if(this.numUnitPrice.Value > 0 && (newPrice != this.numUnitPrice.Value || priceUnit.ToString() != this.cboUnit.Text))
                        {
                            //if the part price or unit is different from the current price or unit ask the user if they want to update
                            var dlgResult = MessageBoxUtilities.ShowMessageBoxYesOrNo("Would you like to update the unit price and unit with the unit price and unit of the selected part?", "Update Unit Price and Unit");
                            if(dlgResult == DialogResult.Yes)
                                UpdateDefaultPrice(); //update the default price based on the new part
                        }
                        else
                        {
                            //update the default price based on the new part
                            UpdateDefaultPrice();
                        }
                    }

                    UpdateWeight();
                    UpdateOrderCost(); //part change
                }
                else if (userChangedPart && !Editable)
                {
                    // The value changed even though the order is locked/non-editable.
                    // This value needs to be reset if possible.
                    try
                    {
                        cboPart.SelectionChanged -= cboPart_SelectionChanged;
                        if (CurrentRecord is OrdersDataSet.OrderRow order && !order.IsPartIDNull())
                        {
                            _log.Warn($"Issue with Order: {order.OrderID} - Part: {order.PartID}");
                            _log.Error("Attempted to update part even though order is locked");

                            // Attempt to restore original value from bound value.
                            cboPart.Value = order.PartID;
                        }
                    }
                    finally
                    {
                        cboPart.SelectionChanged += cboPart_SelectionChanged;
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error during part selection change.");
            }
        }

        private void cboCustomer_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboCustomer.SelectedItem != null)
                {
                    //force new value to be written to the order before setting current customer ID
                    this.cboCustomer.DataBindings[0].WriteValue();

                    CurrentCustomerID = Convert.ToInt32(this.cboCustomer.SelectedItem.DataValue);

                    if (!PartsLoading && _panelLoaded && !_recordLoading && base.CurrentRecord != null)
                    {
                        this.cboUnit.ValueChanged -= this.OnPriceUnit_ValueChanged;
                        this.numPartQty.ValueChanged -= this.numPartQty_ValueChanged;
                        this.numUnitPrice.ValueChanged -= this.numUnitPrice_ValueChanged;
                        this.numWeight.ValueChanged -= this.numWeight_ValueChanged;

                        // Reset values
                        this.numPartQty.Value = 0;
                        this.cboUnit.SelectedItem = this.cboUnit.FindItemByValue<string>(i => i == _priceUnitPersistence.DefaultPriceUnit().ToString());
                        cboPart.Clear(); // Part should already be cleared - text needs to be cleared as well
                        this.numUnitPrice.Value = 0;
                        this.numWeight.Value = DBNull.Value;

                        this.cboUnit.ValueChanged += this.OnPriceUnit_ValueChanged;
                        this.numPartQty.ValueChanged += this.numPartQty_ValueChanged;
                        this.numUnitPrice.ValueChanged += this.numUnitPrice_ValueChanged;
                        this.numWeight.ValueChanged += this.numWeight_ValueChanged;

                        var order = base.CurrentRecord as OrdersDataSet.OrderRow;
                        OrderFeeTools.DeleteDefaultFees(ref order);
                        if (ApplicationSettings.Current.ApplyDefaultFeesEnabled && !this._newOrderBeingCreated)
                            OrderFeeTools.AddDefaultFees(ref order);

                    }
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error binding on customer change.");
            }
        }

        private void Order_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if(e.Row.RowState == DataRowState.Added && e.Column == Dataset.Order.CustomerIDColumn && e.ProposedValue != null && e.ProposedValue != DBNull.Value)
            {
                //update new orders work status depending on its customer
                ((OrdersDataSet.OrderRow) CurrentRecord).WorkStatus = OrderControllerExtensions.GetNewOrderWorkStatus(Convert.ToInt32(e.ProposedValue), UserRequiresOrderReview);
            }
        }

        private void txtInvoice_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                //if there is a invoice
                if(this.txtInvoice.Text != null)
                {
                    string itemsAsString = "OrderID:" + ((OrdersDataSet.OrderRow) base.CurrentRecord).OrderID + "|Invoice:" + this.txtInvoice.Text;

                    using(var frm = new UserEventLog {Operation = "Clear Invoice", Form = "Order.Invoice", UserID = SecurityManager.Current.UserID, UserName = SecurityManager.Current.UserName, TransactionDetails = itemsAsString})
                    {
                        if(frm.ShowDialog(this) == DialogResult.OK)
                        {
                            //clear invoice after item has been logged.
                            this.txtInvoice.Text = null;
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error clearing the orders invoice.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void numPartQty_AfterEnterEditMode(object sender, EventArgs e)
        {
            try
            {
                if(this.numPartQty.IsInEditMode)
                    this.numPartQty.SelectAll();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error selecting text.";
                _log.Error(exc, errorMsg);
            }
        }

        private void picPriceHistory_Click(object sender, EventArgs e) { DisplayPriceHistory(); }

        private void numUnitPrice_Enter(object sender, EventArgs e) { DisplayPriceHistory(); }

        private void numUnitPrice_Leave(object sender, EventArgs e)
        {

            try
            {
                if (this.popPriceHistory.IsDisplayed)
                    this.popPriceHistory.Close();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error during unit price leave.");
            }
        }

        private void txtPONumber_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            if(QuickFilter != null)
                QuickFilter(OrderSearchField.PO, this.txtPONumber.Text);
        }

        private void mediaWidget_FileAdded(object sender, MediaWidget.FileAddedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.FilePath))
                {
                    return;
                }

                AddMediaToDelete(e.FilePath);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding media.");
            }
        }

        private void QuantityField_Leave(object sender, EventArgs e)
        {
            try
            {
                // Pricing - prompt user to update price unit if it doesn't match.
                var currentOrder = CurrentRecord as OrdersDataSet.OrderRow;

                var partQty = numPartQty.Value == DBNull.Value || numPartQty.Value == null
                    ? 0
                    : Convert.ToInt32(numPartQty.Value);

                var weight = numWeight.Value == DBNull.Value || numWeight.Value == null
                    ? 0M
                    : Convert.ToDecimal(numWeight.Value);

                var skipLeaveHandler =  numUnitPrice.ValueObject == null
                    || currentOrder == null
                    || !ApplicationSettings.Current.UsePriceUnitQuantities
                    || currentOrder.IsPartIDNull()
                    || partQty <= 0;

                if (skipLeaveHandler)
                {
                    return;
                }

                // Check to see if quantity/weight changed
                // Assumption: This event handler occurs before data binding
                // updates the value.
                // Testing on a Release build showed that this was the case,
                // but changing the binding's DataSourceUpdateMode may
                // cause issues.

                var changedPartQuantity = currentOrder.IsPartQuantityNull()
                    ? partQty > 0
                    : partQty != currentOrder.PartQuantity;

                var changedWeight = currentOrder.IsWeightNull()
                    ? weight > 0M
                    : weight != currentOrder.Weight;

                if (!changedPartQuantity && !changedWeight)
                {
                    return;
                }

                // Manually update part quantity - used by lead time calculation
                numPartQty.DataBindings[0].WriteValue();

                // Update price unit (if necessary)
                var priceUnitIs = OrderPrice.ParsePriceUnit(this.cboUnit.Text);
                var part = Dataset.PartSummary.FindByPartID(Convert.ToInt32(currentOrder.PartID));

                if (ApplicationSettings.Current.PartPricingType == PricingType.Part)
                {
                    var priceUnitShouldBe = _priceUnitPersistence.DeterminePriceUnit(currentOrder.CustomerID, partQty, weight, priceUnitIs);

                    if (priceUnitShouldBe != priceUnitIs)
                    {
                        var currentDisplayText = _priceUnitPersistence.GetDisplayText(priceUnitIs);
                        var shouldBeDisplayText = _priceUnitPersistence.GetDisplayText(priceUnitShouldBe);

                        var dialogText =
                            $"Price unit should be updated from {currentDisplayText} to {shouldBeDisplayText} based on price unit settings, would you like to update now?";

                        var dialogResult = MessageBoxUtilities.ShowMessageBoxYesOrNo(dialogText, "Update Price and Unit");

                        if (dialogResult == DialogResult.Yes)
                            this.cboUnit.SelectedItem = this.cboUnit.FindItemByValue<string>(i => i == priceUnitShouldBe.ToString());
                    }
                }
                else if (ApplicationSettings.Current.PartPricingType == PricingType.Process && part != null)
                {
                    var unitPriceIs = this.numUnitPrice.Value;
                    (var unitPriceShouldBe, var priceUnitShouldBe) = GetDefaultPrice(part, partQty, weight, priceUnitIs);

                    if ((unitPriceShouldBe != 0) && (unitPriceIs != 0M && (unitPriceIs != unitPriceShouldBe || priceUnitIs != priceUnitShouldBe)))
                    {
                        var dialogText = "";
                        if (unitPriceShouldBe == 0)
                        {
                            dialogText =
                                $"This order contain no processes pricing information. " +
                                $"Order will continue with the price defined?";
                            MessageBoxUtilities.ShowMessageBoxOK(dialogText, "No Process Pricing");

                        }
                        else
                        {
                            dialogText =
                                $"Unit price should be updated from " +
                                $"{unitPriceIs.ToString(OrderPrice.CurrencyFormatString)} " +
                                $"({_priceUnitPersistence.GetDisplayText(priceUnitIs)}) to " +
                                $"{unitPriceShouldBe.ToString(OrderPrice.CurrencyFormatString)} " +
                                $"({_priceUnitPersistence.GetDisplayText(priceUnitShouldBe)}) " +
                                $"based on price unit settings, would you like to update now?";

                            var dialogResult = MessageBoxUtilities.ShowMessageBoxYesOrNo(dialogText, "Update Price and Unit");

                            if (dialogResult == DialogResult.Yes)
                            {
                                try
                                {
                                    this.cboUnit.ValueChanged -= OnPriceUnit_ValueChanged;
                                    this.numUnitPrice.Value = Math.Max(unitPriceShouldBe, 0);
                                    UpdateProcessPrices(priceUnitShouldBe);

                                    this.cboUnit.SelectedItem = this.cboUnit.FindItemByValue<string>(i => i == priceUnitShouldBe.ToString());
                                    UpdateOrderCost(); //QuantityField_Leave
                                }
                                finally
                                {
                                    this.cboUnit.ValueChanged += OnPriceUnit_ValueChanged;
                                }
                            }
                        }
                    }
                }

                // Update lead time (if necessary)
                var usingScheduler = _schedulerInstance.Value != null;
                if (usingScheduler && changedPartQuantity)
                {
                    var canUpdateLeadTimes = true;
                    if (!Editable)
                    {
                        // WO has its part locked - prompt before updating lead time
                        var dialogResult = MessageBoxUtilities.ShowMessageBoxYesOrNo(
                            "Would you like to update lead times?",
                            "Update Lead Times");

                        canUpdateLeadTimes = dialogResult == DialogResult.Yes;
                    }

                    if (canUpdateLeadTimes)
                    {
                        _log.Info($"Updating lead times for order {currentOrder.OrderID}");
                        var processLeadTimes = new OrderProcessLeadTimes();

                        // Retrieve lead times by loading part processes and attempting
                        // to sync with order's current processes.
                        using (var partProcesses = new OrdersDataSet.PartProcessSummaryDataTable())
                        {
                            using (var taPartProcessSummary = new PartProcessSummaryTableAdapter())
                            {
                                taPartProcessSummary.FillByPart(partProcesses, currentOrder.PartID);
                            }

                            var orderProcesses = currentOrder.GetOrderProcessesRows()
                                .OrderBy(op => op.StepOrder)
                                .ToList();

                            var previousStepOrder = -1;

                            foreach (var partProcess in partProcesses)
                            {
                                // Match up with order process
                                var matchingOrderProcess = orderProcesses.Find(op =>
                                {
                                    return op.StepOrder > previousStepOrder
                                        && op.ProcessID == partProcess.ProcessID
                                        && op.ProcessAliasID == partProcess.ProcessAliasID;
                                });

                                if (matchingOrderProcess != null)
                                {
                                    previousStepOrder = matchingOrderProcess.StepOrder;
                                    processLeadTimes.Add(matchingOrderProcess.OrderProcessesID, partProcess);
                                }
                            }
                        }

                        UpdateLeadTimes(currentOrder, processLeadTimes);
                    }
                    else
                    {
                        _log.Info($"Skipping lead time update for order {currentOrder.OrderID}");
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error during part qty leave.");
            }
        }

        private void numWeight_ValueChanged(object sender, EventArgs e)
        {
            // Part quantity may be used to calculate total.
            if (!this._newOrderBeingCreated)
            {
                UpdatePartQuantity();
                UpdateOrderCost(); //Weight Change
            }
        }

        private void numPartQty_ValueChanged(object sender, EventArgs e)
        {
            // Weight may be used to calculate total.
            try
            {
                if(!this._newOrderBeingCreated)
                {
                    this.numWeight.ValueChanged -= this.numWeight_ValueChanged;
                    UpdateWeight();
                    UpdateOrderCost(); //Part Qty Change
                }
                
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error during part qty value change.");
            }
            finally
            {
                this.numWeight.ValueChanged += this.numWeight_ValueChanged;
            }
            
        }

        private void cboShipTo_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                // Fix issue where this handler runs multiple times if it
                // needs to update the order's current Work Status.
                cboShipTo.DataBindings[0].WriteValue();

                // Check for Import/Export workflow
                var appSettings = ApplicationSettings.Current;

                if (!appSettings.ImportExportApprovalEnabled)
                {
                    return;
                }

                // Update the status of the work order depending on the selected address.
                var currentOrder = CurrentRecord as OrdersDataSet.OrderRow;

                var processAddressUpdate = _panelLoaded
                    && !_recordLoading
                    && currentOrder != null
                    && currentOrder.GetOrderReviewRows().Length == 0;

                if (processAddressUpdate)
                {
                    // Setup
                    var customerId = currentOrder.IsCustomerIDNull()
                        ? 0
                        : currentOrder.CustomerID;

                    var selectedAddressId = cboShipTo.Value == null || string.IsNullOrEmpty(cboShipTo.Value.ToString())
                        ? -1
                        : Convert.ToInt32(cboShipTo.Value);

                    var selectedAddress = Dataset.CustomerAddress
                        .FindByCustomerAddressID(selectedAddressId);

                    var initialWorkStatus = OrderControllerExtensions.GetNewOrderWorkStatus(customerId, UserRequiresOrderReview);
                    var importExportWorkStatus = appSettings.WorkStatusPendingImportExportReview;

                    // Check work status and countries
                    var countriesMatch = selectedAddress == null
                        ? true // Do not enter Import/Export Review unless the customer's country is known.
                        : selectedAddress.CountryID == appSettings.CompanyCountry;

                    if (currentOrder.WorkStatus == initialWorkStatus && !countriesMatch)
                    {
                        // Move order to Import/Export Review
                        currentOrder.WorkStatus = importExportWorkStatus;
                    }
                    else if (currentOrder.WorkStatus == importExportWorkStatus && countriesMatch)
                    {
                        // Move order back to its initial status because it does not need
                        // Import/Export review.
                        currentOrder.WorkStatus = initialWorkStatus;
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing ship-to address.");
            }
        }

        private void dtShipDate_Leave(object sender, EventArgs e)
        {
            var order = CurrentRecord as OrdersDataSet.OrderRow;
            if (order != null && (order.IsEstShipDateNull() || order.EstShipDate != dtShipDate.DateTime))
            {
                _autoUpdateEstShipDate = false;
            }
        }

        private void txtContainers_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                var order = CurrentRecord as OrdersDataSet.OrderRow;

                if (e.Button.Key != "Edit" || order == null)
                {
                    return;
                }

                using (var dialog = new OrderContainersPopup { IsQuickView = IsQuickView })
                {
                    dialog.LoadOrder(Dataset, order);

                    // Changes are made if the user clicks OK or X to close the window.
                    dialog.ShowDialog(this);
                    UpdateContainerText(order);
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error showing order containers", exc);
            }
        }

        #endregion

        #region Part Selection Validator

        private class PartsDropDownControlValiditor : ControlValidatorBase
        {
            #region Fields

            private OrderInformation _orderInfo;
            private PartSummaryTableAdapter _taPS;

        /// <summary>
        /// IDs of new orders that had pricing issues.
        /// </summary>
        private readonly HashSet<int> _priceReminderOrderIds =
                new HashSet<int>();

            /// <summary>
            /// Part names that have been checked for open quotes.
            /// </summary>
            private readonly HashSet<string> _openQuoteParts =
                new HashSet<string>();

            #endregion

            #region Methods

            public PartsDropDownControlValiditor(UltraComboEditor control, PartSummaryTableAdapter taPS, OrderInformation orderInfo) : base(control)
            {
                this._taPS = taPS;
                this._orderInfo = orderInfo;
            }

            

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                var editor = Control as UltraComboEditor;
                var order = this._orderInfo.CurrentRecord as OrdersDataSet.OrderRow;

                if (order == null)
                {
                    // Skip validation - not fully loaded
                    e.Cancel = false;
                    FireAfterValidation(true, string.Empty);
                    return;
                }
                
                if (editor != null && editor.Enabled && order.Status != Properties.Settings.Default.OrderStatusClosed)
                {
                    if(editor.SelectedItem == null)
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "An existing part is required to be selected.");
                        return;
                    }
                    if(this._taPS != null && editor.SelectedItem.DataValue is Int32)
                    {
                        int partID = Convert.ToInt32(editor.SelectedItem.DataValue);
                        var partName = editor.Text;

                        if(ApplicationSettings.Current.PartProcessRequired)
                        {
                            while (order.GetOrderProcessesRows().Length == 0)
                            {
                                //Prompt to add process package
                                var dialogResult = MessageBoxUtilities.ShowMessageBoxYesOrNo(
                                    $"The part '{partName}' does not have any processes.\nWould you like to add a process package now?",
                                    "Order Entry");

                                if (dialogResult != DialogResult.Yes || !_orderInfo.AddProcessPackageFromDialog())
                                {
                                    //If no, check for existing part processes and prompt to import those instead
                                    int processCount;
                                    using (var taPartProcessSummary = new PartProcessSummaryTableAdapter())
                                    {
                                        processCount = taPartProcessSummary.GetData(order.PartID).Count;                                            
                                    }
                                    if (processCount > 0)
                                    {
                                        var importDialogResult = MessageBoxUtilities.ShowMessageBoxYesOrNo(
                                            "Would you like to add existing processes from the part to the order?",
                                            "Add Processes");

                                        if (importDialogResult == DialogResult.Yes)
                                            _orderInfo.AddPartProcesses(order);
                                        else
                                        {
                                            //User declined both options, tell them why we are requiring this an
                                            MessageBox.Show("Part processes are currently required during order entry.\nPlease add a process " +
                                                "or process package to the part to continue. This setting can be changed under the workflow section " +
                                                "of the administration settings.", "Part Processes Required",
                                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                                            e.Cancel = true;
                                            FireAfterValidation(false, "The part '" + partName + "' does not have any associated processes.");
                                            return;
                                        }
                                    }                                    
                                }
                            }
                        }

                        // While there is no processes, confirm that the user wishes to continue
                        // or prompt to add processes if admin settings require processes on parts.
                        if (!ApplicationSettings.Current.PartProcessRequired && order.GetOrderProcessesRows().Length == 0)
                        {                                                   
                            var warningDialog = MessageBoxUtilities.ShowMessageBoxYesOrNo("No processes have been included in this work order. " +
                                "Do you wish to continue?", "No Processes Selected");
                            
                            if (warningDialog != DialogResult.Yes)
                            {
                                e.Cancel = true;
                                FireAfterValidation(false, "The part '" + partName + "' does not have any associated processes.");
                                return;
                            }
                            else
                            {
                                //passed
                                e.Cancel = false;
                                FireAfterValidation(true, "");
                                return;
                            }

                        }

                        //Check for inactive processes
                        object inactiveResult = this._taPS.GetInActiveProcessCount(partID);
                        int inactiveProcessesCount = inactiveResult == null || inactiveResult == DBNull.Value ? 0 : Convert.ToInt32(inactiveResult);

                        if(inactiveProcessesCount > 0)
                        {
                            e.Cancel = true;
                            FireAfterValidation(false, "The part '" + partName + "' has  " + inactiveProcessesCount + " in-active processes.");
                            return;
                        }

                        // Check for unapproved processes
                        object unapprovedResult = this._taPS.GetUnapprovedProcessCount(partID);
                        int unapprovedProcessesCount = (unapprovedResult == null || unapprovedResult.Equals(DBNull.Value)) ? 0 : Convert.ToInt32(unapprovedResult);

                        if (unapprovedProcessesCount > 0)
                        {
                            e.Cancel = true;
                            FireAfterValidation(false, "The part '" + partName + "' has  " + unapprovedProcessesCount + " unapproved processes.");
                            return;
                        }

                        // Check for open quotes
                        if (!_openQuoteParts.Contains(partName) && order.IsQuotePartIdNull())
                        {
                            var openQuotes = new HashSet<int>();
                            using (var taQuotes = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.QuoteTableAdapter())
                            {
                                using (var quotes = taQuotes.GetOpenByPart(partName, order.CustomerID))
                                {
                                    foreach (var quote in quotes)
                                    {
                                        openQuotes.Add(quote.QuoteID);
                                    }
                                }
                            }

                            if (openQuotes.Count > 0)
                            {
                                string validationMsgFormat;
                                if (openQuotes.Count == 1)
                                {
                                    validationMsgFormat = "The part '{0}' is in {1} open quote:\n";
                                }
                                else
                                {
                                    validationMsgFormat = "The part '{0}' is in {1} open quotes:\n";
                                }

                                string validationMsg = string.Format(validationMsgFormat,
                                    partName,
                                    openQuotes.Count);

                                validationMsg += string.Join("\n", openQuotes);

                                MessageBoxUtilities.ShowMessageBoxWarn(validationMsg, "Part is in Open Quote");
                            }

                            _openQuoteParts.Add(partName);
                        }

                        // If using process-level pricing, check for pricing data
                        var checkProcessPrices = ApplicationSettings.Current.PartPricingType == PricingType.Process &&
                            ApplicationSettings.Current.ProcessPriceWarningEnabled &&
                            !this._priceReminderOrderIds.Contains(order.OrderID);

                        if (checkProcessPrices)
                        {                                                        
                            this._priceReminderOrderIds.Add(order.OrderID);

                            int processesMissingData;
                            int invalidPriceDataCount;
                            using (var taPartProcessPrice = new PartProcessPriceSummaryTableAdapter())
                            {
                                // Assumption - If a PartProcess has price data, it has price data
                                // for all specified price points
                                processesMissingData = taPartProcessPrice.GetProcessesWithoutPriceCount(partID, _orderInfo._priceUnitPersistence.PriceByTypes());
                                invalidPriceDataCount = taPartProcessPrice.GetInvalidVolumePriceCount(partID) ?? 0;
                            }

                            if (processesMissingData > 0)
                            {
                                var dialogText = string.Format(
                                    "The selected part does not have process-level pricing data for {0} processes.\nDo you want to correct pricing data for the part?",
                                    processesMissingData);

                                var dialogResult = MessageBoxUtilities.ShowMessageBoxYesOrNo(dialogText, "Invalid Process Pricing Data");
                                if (dialogResult == DialogResult.Yes)
                                {
                                    e.Cancel = true;
                                    FireAfterValidation(false, "Process-level pricing data is invalid.");
                                    return;
                                }
                            }
                            else if (invalidPriceDataCount > 0)
                            {
                                var dialogText = "The selected part has incomplete process-level pricing data.\nDo you want to correct process-level pricing data for the part?";
                                var dialogResult = MessageBoxUtilities.ShowMessageBoxYesOrNo(dialogText, "Invalid Process Pricing Data");
                                if (dialogResult == DialogResult.Yes)
                                {
                                    e.Cancel = true;
                                    FireAfterValidation(false, "Process-level pricing data is invalid.");
                                    return;
                                }
                            }
                        }                       
                    }
                }

                //passed
                e.Cancel = false;
                FireAfterValidation(true, "");
            }

            public override void Dispose()
            {
                this._orderInfo = null;
                this._taPS = null;

                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region DocumentValiditor

        private class DocumentValiditor : ControlValidatorBase
        {
            #region Fields

            private OrderInformation _orderInfo;
            private List <int> _ordersAlreadyReminded = new List <int>();
            private Dictionary <int, bool> _requiresPO = new Dictionary <int, bool>();

            #endregion

            #region Methods

            public DocumentValiditor(MediaWidget control, OrderInformation orderInfo) : base(control) { this._orderInfo = orderInfo; }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                try
                {
                    if(Control != null && Control.Enabled)
                    {
                        var mw = Control as MediaWidget;

                        //if there is no media added
                        if(mw != null && mw.AllowEditing && mw.MediaCount < 1)
                        {
                            var order = this._orderInfo.CurrentRecord as OrdersDataSet.OrderRow;

                            //if it is a new order and haven't asked before
                            if(order != null && order.OrderID < 1 && !this._ordersAlreadyReminded.Contains(order.OrderID))
                            {
                                this._ordersAlreadyReminded.Add(order.OrderID);
                                var customer = order.CustomerSummaryRow;

                                //if customer requires PO set
                                if(customer != null && RequiresPO(customer.CustomerID))
                                {
                                    if(MessageBoxUtilities.ShowMessageBoxYesOrNo("The customer requires a scanned PO attached. Do you want to attach a PO?", "Missing Purchase Order") == DialogResult.Yes)
                                    {
                                        e.Cancel = true;
                                        FireAfterValidation(false, "Add the scanned PO.");
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    //passed
                    e.Cancel = false;
                    FireAfterValidation(true, "");
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error determining if there is a scanned PO required.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }

            private bool RequiresPO(int customerID)
            {
                if(!this._requiresPO.ContainsKey(customerID))
                {
                    using(var ta = new Customer_FieldsTableAdapter {ClearBeforeFill = false})
                    {
                        var isRequired = ta.IsFieldRequired("Documents", customerID);
                        this._requiresPO.Add(customerID, isRequired.GetValueOrDefault());
                    }
                }

                return this._requiresPO[customerID];
            }

            public override void Dispose()
            {
                this._requiresPO = null;
                this._ordersAlreadyReminded = null;
                this._orderInfo = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region PriceUnit Validator

        private class PriceUnitValidator : ControlValidatorBase
        {
            #region Fields

            private OrderInformation _orderInfo;
            private List<OrdersDataSet.OrderRow> _ordersAlreadyReminded =
                new List<OrdersDataSet.OrderRow>();

            #endregion

            #region Methods

            public PriceUnitValidator(UltraComboEditor control, OrderInformation orderInfo) : base(control) { this._orderInfo = orderInfo; }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                try
                {
                    if(Control != null && Control.Enabled)
                    {
                        var cbo = Control as UltraComboEditor;

                        //if there is a value selected
                        if(cbo.SelectedItem != null)
                        {
                            var order = this._orderInfo.CurrentRecord as OrdersDataSet.OrderRow;

                            // Pricing - Offer to reset price unit if there
                            // is a mismatch between quantity/weight and price unit.
                            if(order != null && !HaveReminded(order) && UpdatedPriceUnit(order))
                            {
                                var currentPriceUnitRow = ((DataRowView)cbo.SelectedItem.ListObject).Row as OrdersDataSet.PriceUnitRow;

                                if (currentPriceUnitRow != null)
                                {
                                    var currentPriceUnit = OrderPrice.ParsePriceUnit(currentPriceUnitRow.PriceUnitID);

                                    var orderQty = _orderInfo.numPartQty.Value == DBNull.Value
                                        ? 0
                                        : Convert.ToInt32(_orderInfo.numPartQty.Value);

                                    decimal orderWeight = _orderInfo.numWeight.Value == DBNull.Value
                                        ? 0M
                                        : Convert.ToDecimal(_orderInfo.numWeight.Value);

                                    var defaultPriceInfo = _orderInfo.GetDefaultPrice(
                                        _orderInfo.CurrentPart,
                                        orderQty,
                                        orderWeight,
                                        currentPriceUnit);

                                    bool showRegularPrompt = ApplicationSettings.Current.UsePriceUnitQuantities
                                        && currentPriceUnit != defaultPriceInfo.PriceUnit
                                        && OrderPrice.GetPriceByType(defaultPriceInfo.PriceUnit) == PriceByType.Quantity;

                                    bool showWeightPrompt = ApplicationSettings.Current.UsePriceUnitQuantities
                                        && currentPriceUnit != defaultPriceInfo.PriceUnit
                                        && OrderPrice.GetPriceByType(defaultPriceInfo.PriceUnit) == PriceByType.Weight;

                                    if (showRegularPrompt)
                                    {
                                        _ordersAlreadyReminded.Add(order);
                                        var message = $"The '{defaultPriceInfo.DisplayName}' price unit " +
                                            $"suggests that the part quantity should be between " +
                                            $"{defaultPriceInfo.MinQuantity ?? 0} and " +
                                            $"{defaultPriceInfo.MaxQuantity ?? PriceUnitData.MAX_QUANTITY}." +
                                            "\r\n\r\n Do you want to update the price unit or quantity?";

                                        if (MessageBoxUtilities.ShowMessageBoxYesOrNo(message, "Invalid Price Unit") == DialogResult.Yes)
                                        {
                                            e.Cancel = true;
                                            FireAfterValidation(false, "Update quantity or price unit.");
                                            return;
                                        }
                                    }
                                    else if (showWeightPrompt)
                                    {
                                        _ordersAlreadyReminded.Add(order);
                                        var msg =
                                            $"The '{defaultPriceInfo.DisplayName}' " +
                                            $"price unit suggests that the total " +
                                            $"weight should be between " +
                                            $"{defaultPriceInfo.MinWeight ?? 0} and " +
                                            $"{defaultPriceInfo.MaxWeight ?? PriceUnitData.MAX_WEIGHT} " +
                                            $"lbs.\r\n\r\n Do you want to update the price unit or quantity?";

                                        if (MessageBoxUtilities.ShowMessageBoxYesOrNo(msg, "Invalid Price Unit") == DialogResult.Yes)
                                        {
                                            e.Cancel = true;
                                            FireAfterValidation(false, "Update quantity or price unit.");
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            e.Cancel = true;
                            FireAfterValidation(false, "Price unit value required.");
                            return;
                        }
                    }

                    //passed
                    e.Cancel = false;
                    FireAfterValidation(true, "");
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error validating the price unit.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }

            private bool UpdatedPriceUnit(OrdersDataSet.OrderRow order)
            {
                if (!order.IsValidState() || order.RowState == DataRowState.Added)
                {
                    return true;
                }

                var originalPriceUnit = order[_orderInfo.Dataset.Order.PriceUnitColumn, DataRowVersion.Original] as string;
                var currentPriceUnit = order.IsPriceUnitNull()
                    ? null
                    : order.PriceUnit;

                return originalPriceUnit != currentPriceUnit;
            }

            private bool HaveReminded(OrdersDataSet.OrderRow order) =>
                _ordersAlreadyReminded
                    .Any(o => o.IsValidState() && o == order);

            public override void Dispose()
            {
                this._orderInfo = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region Customer Validator

        private class CustomerValidator : ControlValidatorBase
        {
            #region Fields

            private OrderInformation _orderInfo;

            #endregion

            #region Methods

            public CustomerValidator(UltraComboEditor control, OrderInformation orderInfo) : base(control) { this._orderInfo = orderInfo; }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                try
                {
                    if(Control != null && Control.Enabled)
                    {
                        var cbo = Control as UltraComboEditor;

                        //if there is a value selected
                        if(cbo.SelectedItem != null)
                        {
                            var order = this._orderInfo.CurrentRecord as OrdersDataSet.OrderRow;

                            //if it is a new order and haven't asked before
                            if(order != null && order.OrderID < 1)
                            {
                                var customerRow = ((DataRowView) cbo.SelectedItem.ListObject).Row as OrdersDataSet.CustomerSummaryRow;

                                if(customerRow != null && !customerRow.IsCustomerStatusNull() && customerRow.CustomerStatus == Properties.Settings.Default.CustomerStatusOnHold)
                                {
                                    e.Cancel = true;
                                    FireAfterValidation(false, "Customer is On Hold, no new orders can be added for this customer.");
                                    return;
                                }

                                if(customerRow != null && !customerRow.Active)
                                {
                                    e.Cancel = true;
                                    FireAfterValidation(false, "Customer is inactive, no new orders can be added for this customer.");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            e.Cancel = true;
                            FireAfterValidation(false, "Customer required.");
                            return;
                        }
                    }

                    //passed
                    e.Cancel = false;
                    FireAfterValidation(true, "");
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error validating customer.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }

            public override void Dispose()
            {
                this._orderInfo = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region PartQty Validator

        private class PartQtyValiditor : ControlValidatorBase
        {
            #region Fields

            private OrderInformation _orderInfo;

            #endregion

            #region Methods

            public PartQtyValiditor(UltraNumericEditor control, OrderInformation orderInfo) : base(control) { this._orderInfo = orderInfo; }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                try
                {
                    var numEditor = (UltraNumericEditor) Control;

                    if(Control.Enabled)
                    {
                        var currentOrder = this._orderInfo.CurrentRecord as OrdersDataSet.OrderRow;

                        //if order is closed and internal rework then do not validate
                        if(currentOrder != null && currentOrder.Status == Properties.Settings.Default.OrderStatusClosed && currentOrder.OrderType == (int) OrderType.ReworkInt)
                        {
                            //passed
                            e.Cancel = false;
                            FireAfterValidation(true, "");
                            return;
                        }


                        // Do not validate rejoined orders
                        if (currentOrder != null && _orderInfo.Dataset.OrderChange.Any(c => c.IsValidState() && c.ParentOrderID == currentOrder.OrderID && c.ChangeType == (int)OrderChangeType.Rejoin))
                        {
                            e.Cancel = false;
                            FireAfterValidation(true, "");
                            return;
                        }


                        if (numEditor.Value == null && !numEditor.Nullable)
                        {
                            e.Cancel = true;
                            FireAfterValidation(false, "Numeric value must not be null.");
                            return;
                        }

                        if (numEditor.Value != null && numEditor.Value != DBNull.Value)
                        {
                            var value = Convert.ToInt32(numEditor.Value);

                            if (value < 1)
                            {
                                e.Cancel = true;
                                FireAfterValidation(false, "Value must be greater than or equal to " + 1);
                                return;
                            }

                            if (value < Convert.ToInt32(numEditor.MinValue))
                            {
                                e.Cancel = true;
                                FireAfterValidation(false, "Value must be greater than or equal to " + numEditor.MinValue);
                                return;
                            }

                            if (value > Convert.ToInt32(numEditor.MaxValue))
                            {
                                e.Cancel = true;
                                FireAfterValidation(false, "Value must be less than or equal to " + numEditor.MaxValue);
                                return;
                            }
                        }
                    }

                    //passed
                    e.Cancel = false;
                    FireAfterValidation(true, "");
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error validating the price unit.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }

            public override void Dispose()
            {
                this._orderInfo = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region CustomFieldUIInfo

        private class CustomFieldUIInfo
        {
            public int CustomFieldID { get; set; }
            public DisplayValidator Validator { get; set; }
        }

        #endregion

        public class OrderPriceValidator : ControlValidatorBase
        {
            #region Fields

            private OrderInformation _orderInfo;

            #endregion

            #region Methods

            public OrderPriceValidator(UltraCurrencyEditor control, OrderInformation orderInfo) : base(control) { this._orderInfo = orderInfo; }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                try
                {
                    var numEditor = (UltraCurrencyEditor) Control;

                    if(Control.Enabled)
                    {
                        if(numEditor.ValueObject == null)
                        {
                            e.Cancel = true;
                            FireAfterValidation(false, "Value is required.");
                            return;
                        }

                        var value = Convert.ToDecimal(numEditor.ValueObject);
                        if(value < numEditor.MinValue)
                        {
                            e.Cancel = true;
                            FireAfterValidation(false, "Value must be greater than or equal to " + numEditor.MinValue);
                            return;
                        }

                        if(value > numEditor.MaxValue)
                        {
                            e.Cancel = true;
                            FireAfterValidation(false, "Value must be less than or equal to " + numEditor.MaxValue);
                            return;
                        }

                        var order = this._orderInfo.CurrentRecord as OrdersDataSet.OrderRow;

                        if(order != null && order.OrderID < 0 && value > 0 && ApplicationSettings.Current.MinimumOrderPrice > 0 && value < (decimal) ApplicationSettings.Current.MinimumOrderPrice)
                        {
                            if(MessageBoxUtilities.ShowMessageBoxYesOrNo("The minimum price of " + ApplicationSettings.Current.MinimumOrderPrice.ToString(OrderPrice.CurrencyFormatString) + " has not been met.\r\n\r\n Do you want to update the price?", "Invalid Price") == DialogResult.Yes)
                            {
                                e.Cancel = true;
                                FireAfterValidation(false, "Update price.");
                                return;
                            }
                        }
                    }

                    //passed
                    e.Cancel = false;
                    FireAfterValidation(true, String.Empty);
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error validating the currency control: " + Control.Name;
                    LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
                }
            }

            public override void Dispose()
            {
                this._orderInfo = null;
                base.Dispose();
            }

            #endregion
        }

        public class UnitPriceValidator : ControlValidatorBase
        {
            #region Fields

            private OrderInformation _orderInfo;

            #endregion

            #region Methods

            public UnitPriceValidator(UltraCurrencyEditor control, OrderInformation orderInfo) : base(control) { this._orderInfo = orderInfo; }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                try
                {
                    var numEditor = (UltraCurrencyEditor) Control;

                    if(Control.Enabled)
                    {
                        var currentOrder = _orderInfo.CurrentRecord as OrdersDataSet.OrderRow;

                        if(numEditor.ValueObject == null)
                        {
                            e.Cancel = true;
                            FireAfterValidation(false, "Value is required.");
                            return;
                        }

                        var value = Convert.ToDecimal(numEditor.ValueObject);
                        if(value < numEditor.MinValue)
                        {
                            e.Cancel = true;
                            FireAfterValidation(false, "Value must be greater than or equal to " + numEditor.MinValue);
                            return;
                        }

                        if(value > numEditor.MaxValue)
                        {
                            e.Cancel = true;
                            FireAfterValidation(false, "Value must be less than or equal to " + numEditor.MaxValue);
                            return;
                        }
                    }

                    //passed
                    e.Cancel = false;
                    FireAfterValidation(true, String.Empty);
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error validating the currency control: " + Control.Name;
                    LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
                }
            }

            public override void Dispose()
            {
                this._orderInfo = null;
                base.Dispose();
            }

            #endregion
        }

        public class FeesDiscountsValidator : ControlValidatorBase
        {
            #region Fields

            private OrderInformation _orderInfo;

            #endregion

            #region Methods

            public FeesDiscountsValidator(Control control, OrderInformation orderInfo) : base(control)
            {
                _orderInfo = orderInfo
                    ?? throw new ArgumentNullException(nameof(orderInfo));
            }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                try
                {
                    var currentOrder = _orderInfo?.CurrentRecord as OrdersDataSet.OrderRow;

                    if (currentOrder == null || !Control.Enabled)
                    {
                        // Skip validation
                        e.Cancel = false;
                        FireAfterValidation(true, string.Empty);
                        return;
                    }

                    var validationError = string.Empty;

                    foreach (var orderFee in currentOrder.GetOrderFeesRows())
                    {
                        var isDiscount = orderFee.OrderFeeTypeRow.Price < 0;

                        if (isDiscount && orderFee.Charge > 0)
                        {
                            validationError = "This order has a discount with a positive price.";
                            break;
                        }

                        if (!isDiscount && orderFee.Charge < 0)
                        {
                            validationError = "This order has a fee with a negative price.";
                            break;
                        }
                    }

                    var hasValidationError = !string.IsNullOrEmpty(validationError);
                    e.Cancel = hasValidationError;
                    FireAfterValidation(!hasValidationError, validationError);
                }
                catch (Exception exc)
                {
                    string errorMsg = "Error validating the currency control: " + Control.Name;
                    LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
                }
            }

            public override void Dispose()
            {
                this._orderInfo = null;
                base.Dispose();
            }




            #endregion
        }

        private class ContainerValidator : ControlValidatorBase
        {
            #region Properties

            private OrderInformation OrderInformation { get; set; }

            #endregion

            #region Methods

            public ContainerValidator(UltraTextEditor txtContainers, OrderInformation orderInformation)
                : base(txtContainers)
            {
                OrderInformation = orderInformation;
            }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                try
                {
                    var currentOrder = OrderInformation.CurrentRecord as OrdersDataSet.OrderRow;

                    if (!(OrderInformation.IsNewRow ?? false) || currentOrder == null)
                    {
                        return;
                    }

                    if (currentOrder.GetOrderContainersRows().Length == 0)
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "You must add at least one container to the order.");
                    }
                    else
                    {
                        e.Cancel = false;
                        FireAfterValidation(true, string.Empty);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error validating containers.", exc);
                }
            }

            #endregion
        }

        #region Process

        private class Process
        {
            #region Methods

            public int ProcessId { get; private set; }

            public OrdersDataSet.PartProcessSummaryRow PartProcessRow { get; private set; }

            public ProcessesDataset.ProcessRow ProcessRow { get; private set; }

            public int ProcessAliasId { get; private set; }

            public int StepOrder { get; private set; }

            public string Department { get; private set; }

            public int? CocCount { get; private set; }

            public decimal? LoadCapacityVariance { get; private set; }

            public decimal? LoadCapacityWeight { get; private set; }

            public int? LoadCapacityQuantity { get; private set; }

            #endregion

            #region Methods

            public static Process From(OrdersDataSet.PartProcessSummaryRow processRow)
            {
                if (processRow == null)
                {
                    return null;
                }

                return new Process
                {
                    ProcessId = processRow.ProcessID,
                    PartProcessRow = processRow,
                    ProcessAliasId = processRow.ProcessAliasID,
                    StepOrder = processRow.StepOrder,
                    Department = processRow.Department,
                    CocCount = processRow.IsCOCCountNull() ? (int?)null : processRow.COCCount,
                    LoadCapacityVariance = processRow.IsLoadCapacityVarianceNull() ? (decimal?)null : processRow.LoadCapacityVariance,
                    LoadCapacityWeight = processRow.IsLoadCapacityWeightNull() ? (decimal?)null : processRow.LoadCapacityWeight,
                    LoadCapacityQuantity = processRow.IsLoadCapacityQuantityNull() ? (int?)null : processRow.LoadCapacityQuantity,
                };
            }

            public static Process From (ProcessPackageDataset.ProcessPackage_ProcessesRow processRow)
            {
                using (var taProcess = new Data.Datasets.ProcessesDatasetTableAdapters.ProcessTableAdapter())
                {
                    if (processRow == null)
                    {
                        return null;
                    }

                    var dtProcess = new ProcessesDataset.ProcessDataTable();
                    taProcess.FillByProcess(dtProcess, processRow.ProcessID);
                    var processInPackage = dtProcess.FirstOrDefault();

                    if (processInPackage == null)
                    {
                        return null;
                    }

                    return new Process
                    {
                        ProcessId = processRow.ProcessID,
                        ProcessRow = processInPackage,
                        ProcessAliasId = processRow.ProcessAliasID,
                        StepOrder = processRow.StepOrder,
                        Department = processInPackage.Department,
                        CocCount = GetCocData(processRow.ProcessID)
                    };
                }
            }

            public static Process From(QuoteDataSet.QuotePart_ProcessRow processRow)
            {
                using (var taProcess = new Data.Datasets.ProcessesDatasetTableAdapters.ProcessTableAdapter())
                {
                    if (processRow == null)
                        return null;

                    var dtProcess = new ProcessesDataset.ProcessDataTable();
                    taProcess.FillByProcess(dtProcess, processRow.ProcessID);
                    var processInPackage = dtProcess.FirstOrDefault();

                    if (processInPackage == null)
                        return null;

                    return new Process
                    {
                        ProcessId = processRow.ProcessID,
                        ProcessRow = processInPackage,
                        ProcessAliasId = processRow.ProcessAliasID,
                        StepOrder = processRow.StepOrder,
                        Department = processInPackage.Department,
                        CocCount = GetCocData(processRow.ProcessID)
                    };
                }
            }

            private static int? GetCocData(int processId)
            {
                using (var taProcess = new Data.Datasets.ProcessPackageDatasetTableAdapters.ProcessTableAdapter())
                {
                    return taProcess.GetCOCData(processId) ?? 0;
                }
            }

            #endregion
        }

        #endregion

        #region PriceInfo

        /// <summary>
        /// Represents pricing info.
        /// </summary>
        private class PriceInfo
        {
            public decimal BasePrice { get; set; }

            public OrderPrice.enumPriceUnit PriceUnit { get; set; }

            public int? MinQuantity { get; set; }

            public int? MaxQuantity { get; set; }

            public decimal? MinWeight { get; set; }

            public decimal? MaxWeight { get; set; }

            public string DisplayName { get; set; }

            public void Deconstruct(out decimal basePrice, out OrderPrice.enumPriceUnit priceUnit)
            {
                basePrice = BasePrice;
                priceUnit = PriceUnit;
            }
        }

        #endregion

        

        private void neDayUntilReq_ValueChanged(object sender, EventArgs e)
        {
            //if (this.dtOrderRequiredDate.Value != null)
            //{

            //    DateTime ReqDay = (DateTime)this.dtOrderRequiredDate.Value;

            //    DateTime NewReqDay = DataUtilities.ChangeTime(ReqDay, 12, 0, 0, 0);

            //    double Diff = NewReqDay.Subtract(DataUtilities.ChangeTime(DateTime.Now.Date, 12, 0, 0, 0)).TotalDays;

            //    int DayDiff = ((Int32)((UltraNumericEditor)sender).Value - Convert.ToInt32(Diff));
            //    NewReqDay.AddDays(Convert.ToDouble(DayDiff));

            //    if (ReqDay != NewReqDay)
            //    {
            //        this.dtOrderRequiredDate.Value = ReqDay.AddDays(Convert.ToDouble(DayDiff));
            //        this.dtOrderRequiredDate.Update();
            //    }
            //}

        }

        private void dtOrderRequiredDate_ValueChanged(object sender, EventArgs e)
        {
            //if (this.dtOrderRequiredDate.Value == null)
            //    return;
            //DateTime NewReqDay = DataUtilities.ChangeTime((DateTime)this.dtOrderRequiredDate.Value, 12, 0, 0, 0);

            //double Diff = NewReqDay.Subtract(DataUtilities.ChangeTime(DateTime.Now.Date, 12, 0, 0, 0)).TotalDays;

            //this.neDayUntilReq.Value = Diff;


        }

    }
}