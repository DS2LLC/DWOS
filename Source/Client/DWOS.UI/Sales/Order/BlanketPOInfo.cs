using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Order;
using DWOS.Shared;
using DWOS.UI.Admin;
using DWOS.UI.Utilities;
using DWOS.Utilities.Validation;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using NLog;

namespace DWOS.UI.Sales
{
    public partial class BlanketPOInfo : DataPanel
    {
        #region Fields

        private CustomersDataset.Customer_FieldsDataTable _customerFields;
        private int _customerID;
        private Dictionary <string, string> _fieldDefaultValueCache = new Dictionary <string, string>();
        private bool _isInPartEditor;
        private bool _newOrderBeingCreated;
        private OrderTableAdapter _taOrder;
        private PartSummaryTableAdapter _taPartSummary;
        private DWOS.Utilities.Validation.ValidatorManager _validationManager;
        private HashSet<string> _updatingColumns = new HashSet<string>();
        private IPriceUnitPersistence _priceUnitPersistence = new PriceUnitPersistence();
        private DataView _priceUnitDataView;
        private BlanketAllocationHelper _allocationHelper;

        public event Action <int> BeforeCustomerChanged;

        /// <summary>
        ///     Occurs when the parts are cleared and then reloaded.
        /// </summary>
        public event Action PartsReloaded;

        #endregion

        #region Properties

        public OrdersDataSet Dataset
        {
            get { return base._dataset as OrdersDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.OrderTemplate.OrderTemplateIDColumn.ColumnName; }
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
                    var customerFilter = "CustomerID = " + value;

                    //update filter's anytime the customer changes
                    base.UpdateFilter(this.cboPart, customerFilter);
                    base.UpdateFilter(this.cboShippingMethod, customerFilter);
                    base.UpdateFilter(this.cboShipTo, customerFilter);

                    ResizeCustomFieldsPanel();
                    UpdatePartItemsApperance();
                    UpdateValidators(this._customerID);

                    //if the user changed the customer via the dropdown and we are not in the middle of a move to a new record then set customer specific fields
                    if(!_recordLoading)
                    {
                        SelectDefaultShippingMethod();
                        SelectDefaultShippingAddress();
                        UpdateDefaultFieldsPerCustomer(this._customerID);
                        SetCustomFieldVisibility();
                    }
                }
            }
        }

        private OrdersDataSet.CustomerSummaryRow CurrentCustomer
        {
            get { return Dataset.CustomerSummary.FindByCustomerID(CurrentCustomerID); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether parts are being loaded.
        /// </summary>
        /// <value> <c>true</c> if [parts loading]; otherwise, <c>false</c> . </value>
        public bool PartsLoading { get; set; }

        #endregion

        #region Methods

        public BlanketPOInfo() { InitializeComponent(); }

        public void LoadData(OrdersDataSet dataset, PartSummaryTableAdapter partSummary, OrderTableAdapter taOrder)
        {
            this._taPartSummary = partSummary;
            this._taOrder = taOrder;
            Dataset = dataset;
            this._priceUnitDataView = new DataView(Dataset.PriceUnit);

            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.OrderTemplate.TableName;

            //bind column to control
            base.BindValue(this.txtOrderTemplateID, Dataset.OrderTemplate.OrderTemplateIDColumn.ColumnName, false);
            base.BindValue(this.txtPO, Dataset.OrderTemplate.PurchaseOrderColumn.ColumnName, false);
            base.BindValue(this.dtOrderDate, Dataset.OrderTemplate.OrderDateColumn.ColumnName, false);
            base.BindValue(this.dtEndDate, Dataset.OrderTemplate.EndDateColumn.ColumnName, false);
            base.BindValue(this.cboCustomer, Dataset.OrderTemplate.CustomerIDColumn.ColumnName, true);
            base.BindValue(this.cboPriority, Dataset.OrderTemplate.PriorityColumn.ColumnName, false);
            base.BindValue(this.dtStartDate, Dataset.OrderTemplate.StartDateColumn.ColumnName, false);
            base.BindValue(this.cboUserCreated, Dataset.OrderTemplate.CreatedByColumn.ColumnName);
            base.BindValue(this.cboPart, Dataset.OrderTemplate.PartIDColumn.ColumnName, true);
            base.BindValue(this.numPartInitialQty, Dataset.OrderTemplate.InitialQuantityColumn.ColumnName, false);
            base.BindValue(this.numUnitPrice, Dataset.OrderTemplate.BasePriceColumn.ColumnName, false);
            base.BindValue(this.numWeight, Dataset.OrderTemplate.WeightColumn.ColumnName, true);
            base.BindValue(this.cboUnit, Dataset.OrderTemplate.PriceUnitColumn.ColumnName, false);
            base.BindValue(this.cboShippingMethod, Dataset.OrderTemplate.ShippingMethodColumn.ColumnName, false);
            base.BindValue(this.cboShipTo, Dataset.OrderTemplate.CustomerAddressIDColumn.ColumnName, false);
            base.BindValue(this.chkIsActive, Dataset.OrderTemplate.IsActiveColumn.ColumnName, false);

            this.numUnitPrice.MaskInput = "{currency:6." + ApplicationSettings.Current.PriceDecimalPlaces + "}";
            this.numWeight.MaskInput = string.Format("nnnnnn.{0} lbs",
                string.Concat(Enumerable.Repeat("n", ApplicationSettings.Current.WeightDecimalPlaces)));

            //bind lists
            base.BindList(this.cboCustomer, Dataset.CustomerSummary, Dataset.CustomerSummary.CustomerIDColumn.ColumnName, Dataset.CustomerSummary.NameColumn.ColumnName);
            base.BindList(this.cboPriority, Dataset.d_Priority, Dataset.d_Priority.PriorityIDColumn.ColumnName, Dataset.d_Priority.PriorityIDColumn.ColumnName);
            base.BindList(this.cboUserCreated, Dataset.UserSummary, Dataset.UserSummary.UserIDColumn.ColumnName, Dataset.UserSummary.NameColumn.ColumnName);
            base.BindList(this.cboPart, Dataset.PartSummary, Dataset.PartSummary.PartIDColumn.ColumnName, Dataset.PartSummary.NameColumn.ColumnName);
            base.BindList(this.cboUnit, this._priceUnitDataView, Dataset.PriceUnit.PriceUnitIDColumn.ColumnName, Dataset.PriceUnit.DisplayNameColumn.ColumnName);
            base.BindList(this.cboShippingMethod, Dataset.CustomerShippingSummary, Dataset.CustomerShippingSummary.CustomerShippingIDColumn.ColumnName, Dataset.CustomerShippingSummary.NameColumn.ColumnName);
            base.BindList(this.cboShipTo, Dataset.CustomerAddress, Dataset.CustomerAddress.CustomerAddressIDColumn.ColumnName, Dataset.CustomerAddress.NameColumn.ColumnName);

            //prevent the part from changing
            this.cboPart.PreventMouseWheelScrolling();

            _allocationHelper = BlanketAllocationHelper.From(dataset, taOrder);

            base._panelLoaded = true;
        }

        public override void AddValidators(DWOS.Utilities.Validation.ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboUserCreated, "User name required."), errProvider));
            manager.Add(new ImageDisplayValidator(new CustomerValidator(this.cboCustomer, this), errProvider));
            manager.Add(new ImageDisplayValidator(new PartsDropDownControlValiditor(this.cboPart, this._taPartSummary), errProvider));
            manager.Add(new ImageDisplayValidator(new DateTimeValidator(this.dtOrderDate, "Order date required."), errProvider));
            manager.Add(new ImageDisplayValidator(new DateTimeValidator(this.dtStartDate, "Estimated ship date."), errProvider));
            manager.Add(new ImageDisplayValidator(new DateTimeValidator(this.dtEndDate, "Required date is required."), errProvider));
            manager.Add(new ImageDisplayValidator(new PartQtyValiditor(this.numPartInitialQty, this), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboPriority, "Priority required."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtPO, "PO Number required."), errProvider));
            manager.Add(new ImageDisplayValidator(new CurrencyControlValidator(this.numUnitPrice), errProvider));

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

                var poValidator = this._validationManager.Find(this.txtPO);
                if(poValidator != null)
                {
                    var poField = this._customerFields.FirstOrDefault(r => r.CustomerID == customerId && r.FieldID == 2); //MAGIC NUMBER
                    poValidator.IsEnabled = poField != null && poField.Required;
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error updating validators.");
            }
        }

        public OrdersDataSet.OrderTemplateRow AddOrderRow()
        {
            this._newOrderBeingCreated = true;
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as OrdersDataSet.OrderTemplateRow;

            cr.OrderDate = DateTime.Now;
            cr.IsActive = true;
            cr.StartDate = DateTime.Now;
            cr.EndDate = DateTime.Now.AddDays(60);
            cr.Priority = Properties.Settings.Default.OrderPriorityDefault;
            cr.CreatedBy = SecurityManager.Current.UserID;
            cr.InitialQuantity = 0;
            cr.BasePrice = 0;
            cr.PartID = -1;

            //set to currently used customer id if already set else the first one
            if(Dataset.CustomerSummary.Any())
                cr.CustomerSummaryRow = CurrentCustomer ?? Dataset.CustomerSummary.First();

            cr.PurchaseOrder = cr.CustomerSummaryRow == null ? null : (GetDefaultFieldValue(cr.CustomerID, "PO"));

            // Set default shipping method and address
            if(cr.CustomerSummaryRow != null)
            {
                var shippingMethods = cr.CustomerSummaryRow.GetCustomerShippingSummaryRows();
                if (shippingMethods != null)
                {
                    //attempt to set to the default shipping method, if there is no default shipping method then just select first one
                    cr.CustomerShippingSummaryRow = shippingMethods.FirstOrDefault(r => r.DefaultShippingMethod) ?? shippingMethods.FirstOrDefault();
                }

                var addresses = cr.CustomerSummaryRow
                    .GetCustomerAddressRows()
                    .Where(addr => addr.Active)
                    .ToList();

                cr.CustomerAddressRow = addresses.FirstOrDefault(addr => addr.IsDefault) ??
                    addresses.FirstOrDefault();
            }

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

            return cr;
        }

        public OrdersDataSet.OrderTemplateRow AddOrderRow(PartsDataset.PartRow part, int customerID)
        {
            this._newOrderBeingCreated = true;

            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as OrdersDataSet.OrderTemplateRow;

            cr.OrderDate = DateTime.Now;
            cr.IsActive = true;
            cr.StartDate = DateTime.Now;
            cr.EndDate = DateTime.Now.AddDays(60);
            cr.Priority = Properties.Settings.Default.OrderPriorityDefault;
            cr.CreatedBy = SecurityManager.Current.UserID;
            cr.PartID = part.PartID;
            cr.BasePrice = part.IsEachPriceNull() ? 0 : part.EachPrice;

            var customer = Dataset.CustomerSummary.FindByCustomerID(customerID);
            cr.CustomerID = customer.CustomerID;
            cr.CustomerSummaryRow = customer;

            cr.PurchaseOrder = cr.CustomerSummaryRow == null ? null : (GetDefaultFieldValue(cr.CustomerID, "PO"));

            // Set default shipping method and address
            if(cr.CustomerSummaryRow != null)
            {
                var shippingMethods = cr.CustomerSummaryRow.GetCustomerShippingSummaryRows();
                if (shippingMethods != null)
                {
                    //attempt to set to the default shipping method, if there is no default shipping method then just select first one
                    cr.CustomerShippingSummaryRow = shippingMethods.FirstOrDefault(r => r.DefaultShippingMethod) ?? shippingMethods.FirstOrDefault();
                }

                var addresses = cr.CustomerSummaryRow
                    .GetCustomerAddressRows()
                    .Where(addr => addr.Active)
                    .ToList();

                cr.CustomerAddressRow = addresses.FirstOrDefault(addr => addr.IsDefault) ??
                    addresses.FirstOrDefault();
            }

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

            return cr;
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            base.BeforeMoveToNewRecord(id);

            //move focus off of PART becuase will prevent it from showing unselected part if you have the first part picked for the custmer
            cboCustomer.Focus();

            // prevents cboUnit from having a null value when changing record
            this._priceUnitDataView.RowFilter = null;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            this._newOrderBeingCreated = false;
            base.AfterMovedToNewRecord(id);

            var orderTemplate = base.CurrentRecord as OrdersDataSet.OrderTemplateRow;
            UpdateUsageCounts(orderTemplate);
            LoadCurrentPart();
            SetCustomFieldVisibility();

            if (orderTemplate != null && !Dataset.PriceUnit.Any(unit => !unit.Active && orderTemplate.PriceUnit == unit.PriceUnitID))
            {
                this._priceUnitDataView.RowFilter = "Active = 1";
            }

            // Some controls are enabled/disabled independently of number of orders.
            bool enableEditableControls = SecurityManager.Current.IsInRole("BlanketPOManager.Edit");
            this.numUnitPrice.ReadOnly = !enableEditableControls;
            this.cboUnit.ReadOnly = !enableEditableControls;
            this.numPartInitialQty.ReadOnly = !enableEditableControls;
        }

        private void UpdateUsageCounts(OrdersDataSet.OrderTemplateRow orderTemplate)
        {
            this.numPartAllocatedQty.Value = 0;
            this.numPartRemainingQty.Value = 0;

            if(orderTemplate != null)
            {
                var initialQty = orderTemplate.InitialQuantity;
                var allocatedQty = _allocationHelper.GetAllocatedQuantity(orderTemplate);
                var remainingQty = initialQty - allocatedQty;

                this.numPartAllocatedQty.Value = allocatedQty;
                this.numPartRemainingQty.Value = remainingQty;
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

            var order = base.CurrentRecord as OrdersDataSet.OrderTemplateRow;
            if (order == null)
            {
                return;
            }

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
        }

        private void UpdateUnitPrice()
        {
            try
            {
                // Update the numUnitPrice
                var or = base.CurrentRecord as OrdersDataSet.OrderRow;

                if (or != null && !or.IsPartIDNull())
                {
                    var psr = Dataset.PartSummary.FindByPartID(Convert.ToInt32(or.PartID));
                    if (psr != null)
                    {
                        OrderPrice.enumPriceUnit unitType = OrderPrice.ParsePriceUnit(this.cboUnit.Text);
                        decimal? unitPrice = OrderPrice.DetermineBasePrice(psr, unitType);
                        if (unitPrice.HasValue)
                        {
                            this.numUnitPrice.Value = unitPrice.Value;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating unit price.");
            }
        }

        /// <summary>
        ///     Updates the default price information based on the part.
        /// </summary>
        private void UpdateDefaultPrice()
        {
            try
            {
                _log.Debug("Updating default price.");

                if (this.cboPart.SelectedItem == null)
                    return;

                //extra checks, getting raygun errors from ToInt32  on a DBNull (?)
                if (this.cboPart.SelectedItem.DataValue == DBNull.Value)
                    return;

                if (this.numPartInitialQty.Value == null || this.numPartInitialQty.Value == DBNull.Value)
                    return;

                //attempt to set default values based on part
                var part = Dataset.PartSummary.FindByPartID(Convert.ToInt32(this.cboPart.SelectedItem.DataValue));
                var partQty = Convert.ToInt32(this.numPartInitialQty.Value);

                decimal weight = this.numWeight.Value == DBNull.Value ? 0M : Convert.ToDecimal(this.numWeight.Value);
                var currentPriceUnit = OrderPrice.ParsePriceUnit(this.cboUnit.Text);
                var priceUnit = _priceUnitPersistence.DeterminePriceUnit(part.CustomerID, partQty, weight, currentPriceUnit);
                decimal newPrice = OrderPrice.DetermineBasePrice(part, priceUnit).GetValueOrDefault();

                this.numUnitPrice.Value = newPrice;
                this.numUnitPrice.DataBindings[0].WriteValue();

                //set part default price unit
                var unitItem = this.cboUnit.FindItemByValue<string>(i => i == priceUnit.ToString());

                if (unitItem != null && unitItem.DataValue != this.cboUnit.SelectedItem.DataValue)
                {
                    this.cboUnit.SelectedItem = unitItem;
                    this.cboUnit.DataBindings[0].WriteValue();
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error updating default price.");
            }
        }

        /// <summary>
        ///     Selects the default shipping method after the shipping methods for the selected customer has been updated.
        /// </summary>
        private void SelectDefaultShippingMethod()
        {
            try
            {
                var order = CurrentRecord as OrdersDataSet.OrderTemplateRow;

                if(order != null && order.CustomerSummaryRow != null)
                {
                    //if no current shipping method or current shipping method is not valid for this customer then reset it
                    if(order.IsShippingMethodNull() || (order.CustomerShippingSummaryRow != null && order.CustomerShippingSummaryRow.CustomerID != CurrentCustomerID))
                    {
                        var shippingMethods = order.CustomerSummaryRow.GetCustomerShippingSummaryRows();

                        if(shippingMethods != null)
                            //attempt to set to the default shipping method, if there is no default shipping method then just select first one
                            order.CustomerShippingSummaryRow = shippingMethods.FirstOrDefault(r => r.DefaultShippingMethod) ?? shippingMethods.FirstOrDefault();
                        else
                            order.SetShippingMethodNull();
                    }

                    //force cbo to update from data row
                    this.cboShippingMethod.DataBindings[0].ReadValue();
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error selecting default shipping method.");
            }
        }

        private void SelectDefaultShippingAddress()
        {
            try
            {
                var orderTemplate = CurrentRecord as OrdersDataSet.OrderTemplateRow;

                if (orderTemplate?.CustomerSummaryRow == null)
                {
                    return;
                }

                if (orderTemplate.IsCustomerAddressIDNull() || orderTemplate.CustomerAddressRow?.CustomerID != CurrentCustomerID)
                {
                    var addresses = orderTemplate.CustomerSummaryRow
                        .GetCustomerAddressRows()
                        .Where(addr => addr.Active)
                        .ToList();

                    orderTemplate.CustomerAddressRow = addresses.FirstOrDefault(addr => addr.IsDefault) ??
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

        private void UpdatePartItemsApperance()
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
                    this.txtPO.Text = !String.IsNullOrWhiteSpace(po) ? po : null;
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error setting blanket PO.";
                _log.Error(exc, errorMsg);
            }
        }

        private void ResizeCustomFieldsPanel()
        {
            try
            {
                _log.Debug("Creating custom field controls.");
                int tableRowCount = 0;

                this.tableCustomFields.RowCount = tableRowCount;
                this.pnlCustomFields.Height = tableRowCount > 0 ? this.tableCustomFields.Height : 0;
                this.pnlCustomFields.Visible = tableRowCount > 0;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error setting blanked PO.";
                _log.Error(exc, errorMsg);
            }
        }

        private void SetCustomFieldVisibility()
        {
            try
            {
                //ensure panel is hidden if it doesn't have any controls.
                if(this.pnlCustomFields.Visible && this.tableCustomFields.Controls.Count < 1)
                    this.pnlCustomFields.Visible = false;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error setting custom field visibility.";
                _log.Error(exc, errorMsg);
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
                if (_updatingColumns.Contains(this.Dataset.OrderTemplate.InitialQuantityColumn.ColumnName))
                {
                    // Automatically updating dependent property - skip.
                    return;
                }

                bool hasPart = this.cboPart.SelectedItem != null && this.cboPart.SelectedItem.DataValue != DBNull.Value;
                bool hasQty = this.numPartInitialQty.Value != DBNull.Value;

                if (!PartsLoading && _panelLoaded && !_recordLoading && base.CurrentRecord != null && hasPart && hasQty)
                {
                    _log.Info("Updating order weight.");
                    var part = Dataset.PartSummary.FindByPartID(Convert.ToInt32(this.cboPart.SelectedItem.DataValue));

                    int qty = Convert.ToInt32(this.numPartInitialQty.Value);
                    decimal weight = part.IsWeightNull() ? 0M : part.Weight * qty;

                    _updatingColumns.Add(this.Dataset.OrderTemplate.WeightColumn.ColumnName);
                    if (weight > Convert.ToDecimal(this.numWeight.MaxValue))
                    {
                        string msg = string.Format("Calculated weight {0} exceeds max value {1}. Using max value for weight.",
                            weight,
                            Convert.ToDecimal(this.numWeight.Value));

                        _log.Info(msg);

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
                _updatingColumns.Remove(this.Dataset.OrderTemplate.WeightColumn.ColumnName);
            }
        }

        private void UpdatePartQuantity()
        {
            try
            {
                if (_updatingColumns.Contains(this.Dataset.OrderTemplate.WeightColumn.ColumnName))
                {
                    // Automatically updating dependent property - skip.
                    return;
                }

                bool hasPart = this.cboPart.SelectedItem != null && this.cboPart.SelectedItem.DataValue != DBNull.Value;
                bool hasWeight = this.numWeight.Value != DBNull.Value;

                if (!PartsLoading && _panelLoaded && !_recordLoading && base.CurrentRecord != null && hasPart && hasWeight)
                {
                    _log.Info("Updating part quantity.");
                    var part = Dataset.PartSummary.FindByPartID(Convert.ToInt32(this.cboPart.SelectedItem.DataValue));

                    decimal weight = Convert.ToDecimal(this.numWeight.Value);
                    decimal partWeight = (part.IsWeightNull() ? 0M : part.Weight);

                    int qty = 0;

                    if (partWeight != 0M)
                    {
                        qty = Convert.ToInt32(weight / partWeight);
                    }

                    _updatingColumns.Add(this.Dataset.OrderTemplate.InitialQuantityColumn.ColumnName);

                    if (qty > Convert.ToInt32(this.numPartInitialQty.MaxValue))
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            "Quantity has exceeded maximum value. Please correct the part weight or total weight as needed.",
                            "Incorrect Quantity Value");

                        this.numPartInitialQty.Value = this.numPartInitialQty.MaxValue;
                    }
                    else if (qty > 0 && qty >= Convert.ToInt32(this.numPartInitialQty.MinValue))
                    {
                        this.numPartInitialQty.Value = qty;
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating part quantity.");
            }
            finally
            {
                _updatingColumns.Remove(this.Dataset.OrderTemplate.InitialQuantityColumn.ColumnName);
            }
        }

        protected override void OnDispose()
        {
            this._taPartSummary = null;
            this._taOrder = null;
            this._customerFields = null;

            if(this._validationManager != null)
            {
                this._validationManager.Dispose();
                this._validationManager = null;
            }

            _allocationHelper = null;

            base.OnDispose();
        }

        #endregion

        #region Events

        private void cboPart_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                //clicked on add new part
                this._isInPartEditor = true;

                var order = (OrdersDataSet.OrderTemplateRow) base.CurrentRecord;

                if (order == null)
                {
                    return;
                }

                using(var p = new PartManager())
                {
                    p.CustomerFilterID = CurrentCustomerID;

                    if(this.cboPart.SelectedItem != null)
                        p.SelectedPartID = Convert.ToInt32(this.cboPart.SelectedItem.DataValue); //select this part
                    else if(!string.IsNullOrEmpty(this.cboPart.Text) && this.cboPart.Text != "-1")
                        p.InitialPartFilterText = this.cboPart.Text; //filter by part name

                    p.AllowInActiveSelectedPart = false;
                    var dialogResult = p.ShowDialog(this);

                    if(dialogResult == DialogResult.OK || p.DataChanged) //use data changed in case user clicked Apply then Cancel ELSE we wont pick up the change by using DialogResult.OK
                    {
                        //if data was changed then reload
                        if(p.DataChanged)
                        {
                            _log.Warn("Part data was changed so reload.");
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
                        UpdateWeight();
                    }

                    if(this.cboPart.SelectedItem != null)
                    {
                        var partRow = ((DataRowView) this.cboPart.SelectedItem.ListObject).Row as OrdersDataSet.PartSummaryRow;
                        var partQty = Convert.ToInt32(this.numPartInitialQty.Value);

                        if(partRow != null)
                        {
                            decimal weight = this.numWeight.Value == DBNull.Value ? 0M : Convert.ToDecimal(this.numWeight.Value);
                            var currentPriceUnit = OrderPrice.ParsePriceUnit(this.cboUnit.Text);
                            var priceUnit = _priceUnitPersistence.DeterminePriceUnit(partRow.CustomerID, partQty, weight, currentPriceUnit);

                            decimal newPrice = OrderPrice.DetermineBasePrice(partRow, priceUnit).GetValueOrDefault();

                            if(newPrice > 0)
                            {
                                _log.Debug("price was set (was not zero) before part was selected.");

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
                                    _log.Debug("Price was not set (was zero) before part was selected so update without asking.");
                                    UpdateDefaultPrice();
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

        private void OnPriceUnit_ValueChanged(object sender, EventArgs e)
        {
            UpdateUnitPrice(); // Make sure the unit price relates to the unit selected
        }

        private void cboPart_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var partQty = 0;

            try
            {
                ////if this is a current item and the user is changing the part
                if(!PartsLoading && this.cboPart.SelectedItem != null && !this._isInPartEditor && !this._newOrderBeingCreated)
                {
                    var partRow = ((DataRowView) this.cboPart.SelectedItem.ListObject).Row as OrdersDataSet.PartSummaryRow;

                    if(partRow != null)
                    {
                        if(!partRow.IsLotPriceNull() || !partRow.IsEachPriceNull())
                        {
                            if(this.numPartInitialQty.Value != DBNull.Value)
                                partQty = Convert.ToInt32(this.numPartInitialQty.Value);

                            decimal weight = this.numWeight.Value == DBNull.Value ? 0M : Convert.ToDecimal(this.numWeight.Value);
                            var currentPriceUnit = OrderPrice.ParsePriceUnit(this.cboUnit.Text);
                            var priceUnit = _priceUnitPersistence.DeterminePriceUnit(partRow.CustomerID, partQty, weight, currentPriceUnit);
                            decimal newPrice = OrderPrice.DetermineBasePrice(partRow, priceUnit).GetValueOrDefault();

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
                    }
                }

                UpdateWeight();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error during part selection change.");
            }
        }

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

        private void numPartQty_AfterEnterEditMode(object sender, EventArgs e)
        {
            try
            {
                if(this.numPartInitialQty.IsInEditMode)
                    this.numPartInitialQty.SelectAll();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error selecting text.";
                _log.Error(exc, errorMsg);
            }
        }

        private void numUnitPrice_Leave(object sender, EventArgs e)
        {
            if(this.popPriceHistory.IsDisplayed)
                this.popPriceHistory.Close();
        }

        private void numPartInitialQty_Leave(object sender, EventArgs e)
        {
            try
            {
                if (!this.Disposing)
                {
                    this.numPartInitialQty.DataBindings[0].WriteValue();
                    UpdateUsageCounts(CurrentRecord as OrdersDataSet.OrderTemplateRow);
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error on leave of part initial quantity.");
            }
        }

        private void numPartInitialQty_ValueChanged(object sender, EventArgs e)
        {
            UpdateWeight();
        }

        private void numWeight_ValueChanged(object sender, EventArgs e)
        {
            UpdatePartQuantity();
        }

        #endregion

        #region Part Selection Validator

        private class PartsDropDownControlValiditor : ControlValidatorBase
        {
            #region Fields

            private PartSummaryTableAdapter _taPS;

            #endregion

            #region Methods

            public PartsDropDownControlValiditor(UltraComboEditor control, PartSummaryTableAdapter taPS) : base(control) { this._taPS = taPS; }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                var editor = Control as UltraComboEditor;

                if(editor != null && editor.Enabled)
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
                        if(this._taPS.PartProcessCount(partID) < 1)
                        {
                            e.Cancel = true;
                            FireAfterValidation(false, "The part '" + editor.Text + "' does not have any associated processes.");
                            return;
                        }
                    }
                }

                //passed
                e.Cancel = false;
                FireAfterValidation(true, "");
            }

            public override void Dispose()
            {
                this._taPS = null;

                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region Customer Validator

        private class CustomerValidator : ControlValidatorBase
        {
            #region Fields

            private BlanketPOInfo _orderInfo;

            #endregion

            #region Methods

            public CustomerValidator(UltraComboEditor control, BlanketPOInfo orderInfo) : base(control) { this._orderInfo = orderInfo; }

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

            private DataPanel _orderInfo;

            #endregion

            #region Methods

            public PartQtyValiditor(UltraNumericEditor control, DataPanel orderInfo) : base(control) { this._orderInfo = orderInfo; }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                try
                {
                    var numEditor = (UltraNumericEditor) Control;

                    if(Control.Enabled)
                    {
                        var currentOrder = this._orderInfo.CurrentRecord as OrdersDataSet.OrderTemplateRow;

                        //if order is closed
                        if(currentOrder != null && !currentOrder.IsActive)
                        {
                            //passed
                            e.Cancel = false;
                            FireAfterValidation(true, "");
                            return;
                        }

                        if(numEditor.Value == null && !numEditor.Nullable)
                        {
                            e.Cancel = true;
                            FireAfterValidation(false, "Numeric value must not be null.");
                            return;
                        }

                        if(numEditor.Value != null && numEditor.Value != DBNull.Value)
                        {
                            var value = Convert.ToInt32(numEditor.Value);

                            if(value < 1)
                            {
                                e.Cancel = true;
                                FireAfterValidation(false, "Value must be greater than or equal to " + 1);
                                return;
                            }

                            if(value < Convert.ToInt32(numEditor.MinValue))
                            {
                                e.Cancel = true;
                                FireAfterValidation(false, "Value must be greater than or equal to " + numEditor.MinValue);
                                return;
                            }

                            if(value > Convert.ToInt32(numEditor.MaxValue))
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
    }
}