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
using DWOS.Shared;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;
using DWOS.Utilities.Validation;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using NLog;

namespace DWOS.UI.Sales
{
    public partial class SalesOrderInformation : DataPanel
    {
        #region Fields
        private const string COLUMN_ORDER_ID = "OrderID";
        private const string COLUMN_PART_NAME = "PartName";
        private const string COLUMN_PART_QUANTITY = "PartQuantity";
        private const string COLUMN_TOTAL_PRICE = "TotalPrice";

        private CustomersDataset.Customer_FieldsDataTable _customerFields;
        private int _customerID;
        private ErrorProvider _errProvider;
        private Dictionary <string, string> _fieldDefaultValueCache = new Dictionary <string, string>();
        private List <string> _filesToDelete;
        private bool _autoUpdateEstShipDate;
        private DWOS.Utilities.Validation.ValidatorManager _validationManager;

        public event Action <int> BeforeCustomerChanged;

        private enum enumPropoagateFields
        {
            PurchaseOrder,
            CustomerWO,
            Invoice,
            OrderDate,
            EstShipDate,
            RequiredDate,
            AdjustedEstShipDate,
        };

        private readonly Lazy<ApplicationSettingsDataSet.FieldsRow> _requiredDateFieldLazy = new Lazy<ApplicationSettingsDataSet.FieldsRow>(
            () =>
            {
                ApplicationSettingsDataSet.FieldsDataTable fields;

                using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                {
                    fields = ta.GetByCategory("Order");
                }

                var requiredDateField = fields.FirstOrDefault(f => f.Name == "Required Date");
                return requiredDateField;
            });

        private readonly Lazy<ApplicationSettingsDataSet.FieldsRow> _adjustedDateFieldLazy = new Lazy<ApplicationSettingsDataSet.FieldsRow>(
            () =>
            {
                ApplicationSettingsDataSet.FieldsDataTable fields;

                using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                {
                    fields = ta.GetByCategory("Order");
                }

                var adjustedDateField = fields.FirstOrDefault(f => f.Name == "Adjusted Est. Ship Date");
                return adjustedDateField;
            });


        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("OrderEntry_SalesOrderInformation", new UltraGridBandSettings());

        #endregion

        #region Properties

        public OrdersDataSet Dataset
        {
            get { return base._dataset as OrdersDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.SalesOrder.SalesOrderIDColumn.ColumnName; }
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

                    UpdateValidators(this._customerID);

                    //if the user changed the customer via the dropdown and we are not in the middle of a move to a new record then set customer specific fields
                    if(!_recordLoading)
                    {
                        UpdateEstimatedShipDate();
                        UpdateDefaultFieldsPerCustomer(this._customerID);
                    }
                }
            }
        }

        private OrdersDataSet.CustomerSummaryRow CurrentCustomer
        {
            get { return Dataset.CustomerSummary.FindByCustomerID(CurrentCustomerID); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this panel is view only. Should be set before LoadData is called.
        /// </summary>
        /// <value> <c>true</c> if [view only]; otherwise, <c>false</c> . </value>
        public bool ViewOnly { get; set; }

        public bool DisableCustomerSelection
        {
            get { return this.cboCustomer.ReadOnly; }
            set { this.cboCustomer.ReadOnly = value; }
        }

        #endregion

        #region Methods

        public SalesOrderInformation()
        {
            InitializeComponent();
        }

        public void LoadData(OrdersDataSet dataset)
        {
            Dataset = dataset;

            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.SalesOrder.TableName;

            // bind columns to controls
            // OnPropertyChanged mode ensures that new WOs use up-to-date SO dates.
            base.BindValue(this.txtSalesOrderID, Dataset.SalesOrder.SalesOrderIDColumn.ColumnName);
            base.BindValue(this.txtPONumber, Dataset.SalesOrder.PurchaseOrderColumn.ColumnName, false);
            base.BindValue(this.dtOrderDate, Dataset.SalesOrder.OrderDateColumn.ColumnName, false,
                DataSourceUpdateMode.OnPropertyChanged);

            base.BindValue(this.dtOrderRequiredDate, Dataset.SalesOrder.RequiredDateColumn.ColumnName, false,
                DataSourceUpdateMode.OnPropertyChanged);

            base.BindValue(this.dtAdjustedShipDate, Dataset.SalesOrder.AdjustedEstShipDateColumn.ColumnName, false,
                DataSourceUpdateMode.OnPropertyChanged);

            base.BindValue(this.cboCustomer, Dataset.SalesOrder.CustomerIDColumn.ColumnName, true);
            base.BindValue(this.txtInvoice, Dataset.SalesOrder.InvoiceColumn.ColumnName);
            base.BindValue(this.dtShipDate, Dataset.SalesOrder.EstShipDateColumn.ColumnName, false,
                DataSourceUpdateMode.OnPropertyChanged);

            base.BindValue(this.cboUserCreated, Dataset.SalesOrder.CreatedByColumn.ColumnName);
            base.BindValue(this.txtCustomerWO, Dataset.SalesOrder.CustomerWOColumn.ColumnName, false);

            //bind lists
            base.BindList(this.cboCustomer, Dataset.CustomerSummary, Dataset.CustomerSummary.CustomerIDColumn.ColumnName, Dataset.CustomerSummary.NameColumn.ColumnName);
            base.BindList(this.cboUserCreated, Dataset.UserSummary, Dataset.UserSummary.UserIDColumn.ColumnName, Dataset.UserSummary.NameColumn.ColumnName);

            this.mediaWidget.Setup(new MediaWidget.SetupArgs()
            {
                MediaJunctionTable = Dataset.SalesOrder_Media,
                MediaTable = Dataset.Media,
                MediaJunctionTableParentRowColumn = Dataset.SalesOrder_Media.SalesOrderIDColumn,
                AllowEditing = Editable,
                MediaLinkType = Documents.LinkType.SalesOrder,
                DocumentLinkTable = Dataset.SalesOrder_DocumentLink,
                ScannerSettingsType = ScannerSettingsType.Order
            });

            this.mediaWidget.FileAdded += mediaWidget_FileAdded;

            if(ViewOnly)
            {
                this.txtPONumber.ReadOnly = true;
                this.dtOrderDate.ReadOnly = true;
                this.dtOrderRequiredDate.ReadOnly = true;
                this.cboCustomer.ReadOnly = true;
                this.dtShipDate.ReadOnly = true;
                this.txtCustomerWO.ReadOnly = true;
                this.mediaWidget.AllowEditing = false;
            }

            base._panelLoaded = true;
        }

        public override void AddValidators(DWOS.Utilities.Validation.ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboUserCreated, "User name required."), errProvider));
            manager.Add(new ImageDisplayValidator(new CustomerValidator(this.cboCustomer, this), errProvider));
            manager.Add(new ImageDisplayValidator(new DateTimeValidator(this.dtOrderDate, "Order date required."), errProvider));
            manager.Add(new ImageDisplayValidator(new DateTimeValidator(this.dtShipDate, "Estimated ship date."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtPONumber, "PO Number required."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtCustomerWO, "Customer WO required."), errProvider));
            manager.Add(new ImageDisplayValidator(new DocumentValiditor(this.mediaWidget, this), errProvider));

            var requiredDateField = _requiredDateFieldLazy.Value;

            if (requiredDateField == null || requiredDateField.IsRequired)
            {
                manager.Add(new ImageDisplayValidator(new DateTimeValidator(this.dtOrderRequiredDate, "Required date is required.")
                {
                    ValidationRequired = () => IsNewRow.GetValueOrDefault()
                }, errProvider));
            }
            else if (!requiredDateField.IsVisible)
            {
                pnlOrderRequiredDate.Visible = false;
            }

            var adjustedEstShipDateField = _adjustedDateFieldLazy.Value;

            if (!adjustedEstShipDateField.IsVisible)
            {
                pnlWorkOrders.Height += pnlAdjustedShipDate.Height;
                pnlAdjustedShipDate.Visible = false;
            }

            this._errProvider = errProvider;
            this._validationManager = manager;
        }

        /// <summary>
        ///     Update the validators based on if they are required by the customer or not
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        private void UpdateValidators(int customerId)
        {
            try
            {
                if(this._validationManager == null) //exit if not using a validation manager
                    return;

                if(this._customerFields == null || this._customerFields.Count(cf => cf.CustomerID == customerId) < 1)
                {
                    _log.Debug("Loading customer fields for customer " + customerId);

                    if(this._customerFields == null)
                        this._customerFields = new CustomersDataset.Customer_FieldsDataTable();

                    using(var ta = new Customer_FieldsTableAdapter {ClearBeforeFill = false})
                        ta.FillBy(this._customerFields, customerId);
                }

                var customerWOValidator = this._validationManager.Find(this.txtCustomerWO);
                if(customerWOValidator != null)
                {
                    var customerWOField = this._customerFields.FirstOrDefault(r => r.CustomerID == customerId && r.FieldID == 1); //MAGIC NUMBER
                    customerWOValidator.IsEnabled = customerWOField != null && customerWOField.Required;
                }

                var poValidator = this._validationManager.Find(this.txtPONumber);
                if(poValidator != null)
                {
                    var poField = this._customerFields.FirstOrDefault(r => r.CustomerID == customerId && r.FieldID == 2); //MAGIC NUMBER
                    //poValidator.IsEnabled = poField != null && poField.Required;
                    poValidator.IsEnabled = false;
                }

                var documentValidator = this._validationManager.Find(this.mediaWidget);
                if(documentValidator != null)
                {
                    var documentField = this._customerFields.FirstOrDefault(r => r.CustomerID == customerId && r.FieldID == 6); //MAGIC NUMBER
                    documentValidator.IsEnabled = documentField != null && documentField.Required;
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating validators.");
            }
        }

        public OrdersDataSet.SalesOrderRow AddSalesOrderRow()
        {
            var rowVw = bsData.AddNew() as DataRowView;
            var row = rowVw.Row as OrdersDataSet.SalesOrderRow;

            row.Status = Properties.Settings.Default.OrderStatusOpen;
            row.OrderDate = DateTime.Now;
            row.CreatedBy = SecurityManager.Current.UserID;
            
            //set to currently used customer id if already set else the first one
            var csr = CurrentCustomer;
            row.CustomerSummaryRow = csr ?? Dataset.CustomerSummary.First();
            row.PurchaseOrder = csr == null ? null : (GetDefaultFieldValue(csr.CustomerID, "PO") ?? "");
            row.CustomerWO = csr == null ? null : GetDefaultFieldValue(csr.CustomerID, "Customer WO");

            var defaultShipDate = DateTime.Now.AddBusinessDays(csr?.LeadTime ?? ApplicationSettings.Current.OrderLeadTime);

            if (_requiredDateFieldLazy.Value?.IsVisible ?? true)
            {
                row.RequiredDate = defaultShipDate;
            }

            row.EstShipDate = defaultShipDate;

            return row;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            var salesOrder = base.CurrentRecord as OrdersDataSet.SalesOrderRow;

            this.txtInvoice.ButtonsLeft[0].Enabled = !String.IsNullOrEmpty(this.txtInvoice.Text); //enable invoice to allow user to delete if there is an invoice to reset

            _autoUpdateEstShipDate = IsNewRow.GetValueOrDefault();

            //Load media and orders
            if(salesOrder != null)
            {
                this.mediaWidget.LoadMedia(salesOrder.GetSalesOrder_MediaRows().ToList<DataRow>(),
                    salesOrder.GetSalesOrder_DocumentLinkRows().ToList<DataRow>(),
                    salesOrder.SalesOrderID);

                //Add WOs to the grid that belong to this sales order
                AddOrders(salesOrder);
            }
            else
                this.mediaWidget.ClearMedia();
        }

        private void AddOrders(OrdersDataSet.SalesOrderRow salesOrder)
        {
            PartSummaryTableAdapter taPart = null;

            try
            {
                this.grdOrders.DataSource = null;
                taPart = new PartSummaryTableAdapter { ClearBeforeFill = false };

                //Create datatable of WOs that belong to this sales order and add to the grid
                var orders = salesOrder.GetOrderRows();

                var dt = new DataTable();
                dt.Columns.Add(COLUMN_ORDER_ID);
                dt.Columns.Add(COLUMN_PART_NAME);
                dt.Columns.Add(COLUMN_PART_QUANTITY);
                dt.Columns.Add(COLUMN_TOTAL_PRICE, typeof(decimal));

                foreach (var order in orders)
                {
                    //Assign the values
                    var row = dt.NewRow();
                    row[COLUMN_ORDER_ID] = order[COLUMN_ORDER_ID];
                    row[COLUMN_PART_QUANTITY] = order[COLUMN_PART_QUANTITY];

                    decimal totalPrice;

                    if (order.IsPartQuantityNull() || order.IsBasePriceNull() || order.IsPriceUnitNull())
                    {
                        totalPrice = 0;
                    }
                    else
                    {
                        var fees = OrderPrice.CalculateFees(order, order.BasePrice);
                        var weight = order.IsWeightNull() ? 0M : order.Weight;
                        totalPrice = OrderPrice.CalculatePrice(order.BasePrice, order.PriceUnit, fees, order.PartQuantity, weight);
                    }

                    row[COLUMN_TOTAL_PRICE] = totalPrice;

                    //Get the part for the order
                    var partTable = new OrdersDataSet.PartSummaryDataTable();

                    if (!order.IsPartIDNull())
                    {
                        taPart.FillByPart(partTable, order.PartID);

                        //Should only be one part
                        if (partTable.Rows.Count == 1)
                        {
                            var part = partTable.Rows[0] as OrdersDataSet.PartSummaryRow;
                            row[COLUMN_PART_NAME] = part?.Name ?? order["PartID"];
                        }
                    }
                    else
                    {
                        row[COLUMN_PART_NAME] = "N/A";
                    }

                    dt.Rows.Add(row);
                };

                this.grdOrders.DataSource = dt;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding sales work orders.");
            }
            finally
            {
                taPart?.Dispose();
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

        private void UpdateDefaultFieldsPerCustomer(int customerID)
        {
            try
            {
                //if customer has Default Field values defined then set it for this customer...
                if(IsActivePanel && customerID > 0)
                {
                    var po = GetDefaultFieldValue(customerID, "PO");
                    this.txtPONumber.Text = !String.IsNullOrWhiteSpace(po) ? po : "";

                    var customerWO = GetDefaultFieldValue(customerID, "Customer WO");
                    this.txtCustomerWO.Text = !String.IsNullOrWhiteSpace(customerWO) ? customerWO : null;
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
            //update est ship date based on customer lead time
            var salesOrder = base.CurrentRecord as OrdersDataSet.SalesOrderRow;

            if (salesOrder == null || !IsNewRow.GetValueOrDefault())
            {
                return;
            }

            var leadTime = CurrentCustomer == null ?
                ApplicationSettings.Current.OrderLeadTime :
                CurrentCustomer.LeadTime;

            var newShipDate = DateUtilities.AddBusinessDays(DateTime.Now, leadTime);

            if (salesOrder.IsEstShipDateNull() || salesOrder.EstShipDate != newShipDate)
            {
                var updateEstShipDate = true;

                if (!_autoUpdateEstShipDate)
                {
                    const string msgHeader = "Est. Ship Date";

                    string updateShipDateMsg = string.Format(
                        "Would you like to update the estimated shipping date to {0:MM/dd/yyyy}?",
                        newShipDate);

                    updateEstShipDate = MessageBoxUtilities.ShowMessageBoxYesOrNo(updateShipDateMsg, msgHeader) == DialogResult.Yes;
                }

                if (updateEstShipDate)
                {
                    salesOrder.EstShipDate = newShipDate;
                    this.dtShipDate.DataBindings[0].ReadValue();
                }
            }
        }

        private void PropagateFieldChanges(UltraTextEditor te, enumPropoagateFields field)
        {
            try
            {
                var salesOrder = base.CurrentRecord as OrdersDataSet.SalesOrderRow;

                if (salesOrder != null)
                {                    
                    //if value did not change then exit
                    switch (field)
                    {
                        case enumPropoagateFields.PurchaseOrder:
                            if(!salesOrder.IsPurchaseOrderNull() && salesOrder.PurchaseOrder == te.Text)
                                return;
                            break;
                        case enumPropoagateFields.CustomerWO:
                            if (!salesOrder.IsCustomerWONull() && salesOrder.CustomerWO == te.Text)
                                return;
                            break;
                        case enumPropoagateFields.Invoice:
                            if (!salesOrder.IsInvoiceNull() && salesOrder.Invoice == te.Text)
                                return;
                            break;
                        default:
                            break;
                    }

                    //if saved sales order then ask if fields should be propogated
                    if (salesOrder.SalesOrderID > 0)
                    {
                        if (MessageBoxUtilities.ShowMessageBoxYesOrNo("Do you want to update all child orders?", "Update Child Orders", "Changes to the Sales Order can be pushed to all child orders.") == DialogResult.No)
                            return;
                    }
                    
                    foreach (var order in salesOrder.GetOrderRows())
                    {
                        switch (field)
                        {
                            case enumPropoagateFields.PurchaseOrder:
                                if (order.IsPurchaseOrderNull() || order.PurchaseOrder != te.Text)
                                    order.PurchaseOrder = te.Text;
                                break;
                            case enumPropoagateFields.CustomerWO:
                                if (order.IsCustomerWONull() || order.CustomerWO != te.Text)
                                    order.CustomerWO = te.Text;
                                break;
                            case enumPropoagateFields.Invoice:
                                if (order.IsInvoiceNull() || order.Invoice != te.Text)
                                    order.Invoice = te.Text;
                                break;
                            default:
                                _log.Debug("Unable to propagate sales order field to WO: {1}.", field);
                                break;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error propagating fields to child orders.");
            }
        }

        private void PropagateFieldChanges(UltraDateTimeEditor dte, enumPropoagateFields field)
        {
            try
            {
                var salesOrder = base.CurrentRecord as OrdersDataSet.SalesOrderRow;
                var hasNewValue = dte.Value != null;
                DateTime newValue = dte.DateTime;

                if (salesOrder == null || !salesOrder.HasVersion(DataRowVersion.Proposed))
                {
                    return;
                }

                switch (field)
                {
                    case enumPropoagateFields.OrderDate:
                        if (!salesOrder.IsProposedValueChanged(Dataset.SalesOrder.OrderDateColumn))
                        {
                            salesOrder.EndEdit();
                            return;
                        }

                        break;
                    case enumPropoagateFields.EstShipDate:
                        if (!salesOrder.IsProposedValueChanged(Dataset.SalesOrder.EstShipDateColumn))
                        {
                            salesOrder.EndEdit();
                            return;
                        }

                        break;
                    case enumPropoagateFields.RequiredDate:
                        if (!salesOrder.IsProposedValueChanged(Dataset.SalesOrder.RequiredDateColumn))
                        {
                            salesOrder.EndEdit();
                            return;
                        }

                        break;
                    case enumPropoagateFields.AdjustedEstShipDate:
                        if (!salesOrder.IsProposedValueChanged(Dataset.SalesOrder.AdjustedEstShipDateColumn))
                        {
                            salesOrder.EndEdit();
                            return;
                        }

                        break;
                    default:
                        break;
                }

                salesOrder.EndEdit();

                //if saved sales order then ask if fields should be propagated
                if (salesOrder.SalesOrderID > 0)
                {
                    const string message = "Do you want to update all child orders?";
                    const string header = "Update Child Orders";
                    const string footer = "Changes to the Sales Order can be pushed to all child orders.";

                    if (MessageBoxUtilities.ShowMessageBoxYesOrNo(message, header, footer) == DialogResult.No)
                    {
                        return;
                    }
                }

                // Propagate value to work orders
                foreach (var workOrder in salesOrder.GetOrderRows())
                {
                    switch (field)
                    {
                        case enumPropoagateFields.OrderDate:
                            if (hasNewValue && (workOrder.IsOrderDateNull() || workOrder.OrderDate != newValue))
                            {
                                workOrder.OrderDate = newValue;
                            }
                            else if (!hasNewValue && !workOrder.IsOrderDateNull())
                            {
                                workOrder.SetOrderDateNull();
                            }

                            break;
                        case enumPropoagateFields.EstShipDate:
                            if (hasNewValue && (workOrder.IsEstShipDateNull() || workOrder.EstShipDate != newValue))
                            {
                                workOrder.EstShipDate = newValue;
                            }
                            else if (!hasNewValue && !workOrder.IsEstShipDateNull())
                            {
                                workOrder.SetEstShipDateNull();
                            }

                            break;
                        case enumPropoagateFields.RequiredDate:
                            if (hasNewValue && (workOrder.IsRequiredDateNull() || workOrder.RequiredDate != newValue))
                            {
                                workOrder.RequiredDate = newValue;
                            }
                            else if (!hasNewValue && !workOrder.IsRequiredDateNull())
                            {
                                workOrder.SetRequiredDateNull();
                            }

                            break;
                        case enumPropoagateFields.AdjustedEstShipDate:
                            if (hasNewValue && (workOrder.IsAdjustedEstShipDateNull() || workOrder.AdjustedEstShipDate != newValue))
                            {
                                workOrder.AdjustedEstShipDate = newValue;
                            }
                            else if (!hasNewValue && !workOrder.IsAdjustedEstShipDateNull())
                            {
                                workOrder.SetAdjustedEstShipDateNull();
                            }

                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error on propagate field changes.");
            }
        }

        private void FormatSummary(SummarySettings ss, bool currency)
        {
            ss.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
            ss.Appearance.FontData.Bold = DefaultableBoolean.True;
            ss.Appearance.ForeColor = Color.Black;
            ss.Appearance.TextHAlign = HAlign.Right;

            if(currency)
                ss.DisplayFormat = "{0:C}";
            else
                ss.DisplayFormat = "{0:N0}";
        }

        protected override void OnDispose()
        {
            this._filesToDelete = null;
            this._customerFields = null;

            if(this._validationManager != null)
            {
                this._validationManager.Dispose();
                this._validationManager = null;
            }

            base.OnDispose();
        }

        #endregion

        #region Events

        private void cboCustomer_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if(this.cboCustomer.SelectedItem != null)
                {
                    //force new value to be written to the order before setting current customer ID
                    this.cboCustomer.DataBindings[0].WriteValue();

                    CurrentCustomerID = Convert.ToInt32(this.cboCustomer.SelectedItem.DataValue);
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error binding on customer change.");
            }
        }

        private void txtInvoice_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                //if there is a invoice
                if(this.txtInvoice.Text != null)
                {
                    string itemsAsString = "SalesOrderID:" + ((OrdersDataSet.SalesOrderRow) base.CurrentRecord).SalesOrderID + "|Invoice:" + this.txtInvoice.Text;

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

        private void mediaWidget_FileAdded(object sender, MediaWidget.FileAddedEventArgs e)
        {
            // Add media to Work Orders
            if (!(CurrentRecord is OrdersDataSet.SalesOrderRow salesOrder))
            {
                return;
            }

            var mediaJunctionRows = salesOrder.GetSalesOrder_MediaRows().ToList <DataRow>();

            foreach (var order in salesOrder.GetOrderRows())
            {
                var existingMediaRelation = Dataset.Order_Media
                    .FindByOrderIDMediaID(order.OrderID, e.MediaId);

                if (existingMediaRelation == null)
                {
                    Dataset.Order_Media.Rows.Add(order.OrderID, e.MediaId);
                }
            }
        }

        private void grdOrders_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                grdOrders.AfterColPosChanged -= grdOrders_AfterColPosChanged;

                //Order columns and add user friendly column names
                this.grdOrders.DisplayLayout.Bands[0].Columns[COLUMN_ORDER_ID].Header.Caption = "Work Order";
                this.grdOrders.DisplayLayout.Bands[0].Columns[COLUMN_ORDER_ID].Header.VisiblePosition = 0;
                this.grdOrders.DisplayLayout.Bands[0].Columns[COLUMN_PART_NAME].Header.Caption = "Part Name";
                this.grdOrders.DisplayLayout.Bands[0].Columns[COLUMN_PART_NAME].Header.VisiblePosition = 2;
                this.grdOrders.DisplayLayout.Bands[0].Columns[COLUMN_PART_QUANTITY].Header.Caption = "Quantity";
                this.grdOrders.DisplayLayout.Bands[0].Columns[COLUMN_PART_QUANTITY].Header.VisiblePosition = 3;
                this.grdOrders.DisplayLayout.Bands[0].Columns[COLUMN_PART_QUANTITY].CellAppearance.TextHAlign = HAlign.Right;
                this.grdOrders.DisplayLayout.Bands[0].Columns[COLUMN_TOTAL_PRICE].Header.Caption = "Total Price";
                this.grdOrders.DisplayLayout.Bands[0].Columns[COLUMN_TOTAL_PRICE].Header.VisiblePosition = 6;
                this.grdOrders.DisplayLayout.Bands[0].Columns[COLUMN_TOTAL_PRICE].CellAppearance.TextHAlign = HAlign.Right;

                this.grdOrders.DisplayLayout.Bands[0].Columns[COLUMN_TOTAL_PRICE].MaskInput = "{double:6." +
                    ApplicationSettings.Current.PriceDecimalPlaces + "}";

                this.grdOrders.DisplayLayout.Bands[0].Columns[COLUMN_TOTAL_PRICE].PromptChar = ' ';

                // Disable updating on the entire order grid
                this.grdOrders.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
                this.grdOrders.DisplayLayout.Override.AllowRowSummaries = AllowRowSummaries.Default;

                SummarySettings ss = this.grdOrders.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Count, this.grdOrders.DisplayLayout.Bands[0].Columns[COLUMN_ORDER_ID]);
                FormatSummary(ss, false);
                ss = this.grdOrders.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Sum, this.grdOrders.DisplayLayout.Bands[0].Columns[COLUMN_PART_QUANTITY]);
                FormatSummary(ss, false);
                ss = this.grdOrders.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Sum, this.grdOrders.DisplayLayout.Bands[0].Columns[COLUMN_TOTAL_PRICE]);
                FormatSummary(ss, true);

                this.grdOrders.DisplayLayout.Bands[0].SummaryFooterCaption = "Summary Totals:";
                e.Layout.Override.SummaryFooterCaptionAppearance.FontData.Bold = DefaultableBoolean.True;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(grdOrders.DisplayLayout.Bands[0]);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error with work order summary grid.");
            }
            finally
            {
                grdOrders.AfterColPosChanged += grdOrders_AfterColPosChanged;
            }
        }

        private void grdOrders_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdOrders.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing column position in grid.");
            }
        }

        private void txtPONumber_Leave(object sender, EventArgs e)
        {
            PropagateFieldChanges(this.txtPONumber, enumPropoagateFields.PurchaseOrder);
        }

        private void txtCustomerWO_Leave(object sender, EventArgs e)
        {
            PropagateFieldChanges(this.txtCustomerWO, enumPropoagateFields.CustomerWO);
        }

        private void txtInvoice_Leave(object sender, EventArgs e)
        {
            PropagateFieldChanges(this.txtInvoice, enumPropoagateFields.Invoice);
        }

        private void dtOrderDate_Validated(object sender, EventArgs e)
        {
            PropagateFieldChanges(this.dtOrderDate, enumPropoagateFields.OrderDate);
        }

        private void dtShipDate_Validated(object sender, EventArgs e)
        {
            var salesOrder = base.CurrentRecord as OrdersDataSet.SalesOrderRow;

            if (salesOrder == null)
            {
                return;
            }

            if (salesOrder.IsEstShipDateNull() || salesOrder.EstShipDate != this.dtShipDate.DateTime)
            {
                _autoUpdateEstShipDate = false;
            }

            PropagateFieldChanges(this.dtShipDate, enumPropoagateFields.EstShipDate);
        }

        private void dtOrderRequiredDate_Validated(object sender, EventArgs e)
        {
            PropagateFieldChanges(this.dtOrderRequiredDate, enumPropoagateFields.RequiredDate);
        }

        private void dtAdjustedShipDate_Validated(object sender, EventArgs e)
        {
            PropagateFieldChanges(dtAdjustedShipDate, enumPropoagateFields.AdjustedEstShipDate);
        }

        #endregion

        #region DocumentValiditor

        private class DocumentValiditor : ControlValidatorBase
        {
            #region Fields

            private Dictionary <int, bool> _requiresPO = new Dictionary <int, bool>();
            private SalesOrderInformation _salesOrderInfo;
            private List <int> _salesOrdersAlreadyReminded = new List <int>();

            #endregion

            #region Methods

            public DocumentValiditor(MediaWidget control, SalesOrderInformation salesOrderInfo) : base(control) { this._salesOrderInfo = salesOrderInfo; }

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
                            var salesOrder = this._salesOrderInfo.CurrentRecord as OrdersDataSet.SalesOrderRow;

                            //if it is a new sales order and haven't asked before
                            if(salesOrder != null && salesOrder.SalesOrderID < 1 && !this._salesOrdersAlreadyReminded.Contains(salesOrder.SalesOrderID))
                            {
                                this._salesOrdersAlreadyReminded.Add(salesOrder.SalesOrderID);
                                var customer = salesOrder.CustomerSummaryRow;

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
                this._salesOrdersAlreadyReminded = null;
                this._salesOrderInfo = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region Customer Validator

        private class CustomerValidator : ControlValidatorBase
        {
            #region Fields

            private DataPanel _orderInfo;

            #endregion

            #region Methods

            public CustomerValidator(UltraComboEditor control, DataPanel orderInfo) : base(control) { this._orderInfo = orderInfo; }

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
                            var order = this._orderInfo.CurrentRecord as OrdersDataSet.SalesOrderRow;

                            //if it is a new sales order and haven't asked before
                            if(order != null && order.SalesOrderID < 1)
                            {
                                var customerRow = ((DataRowView) cbo.SelectedItem.ListObject).Row as OrdersDataSet.CustomerSummaryRow;

                                if(customerRow != null && !customerRow.IsCustomerStatusNull() && customerRow.CustomerStatus == Settings.Default.CustomerStatusOnHold)
                                {
                                    e.Cancel = true;
                                    FireAfterValidation(false, "Customer is On Hold, no new orders can be added for this customer.");
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
    }
}