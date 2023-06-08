using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Interop;
using DWOS.Utilities.Validation;
using DWOS.Data;
using DWOS.Data.Customer;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Shared;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinTabControl;
using DWOS.UI.Tools;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;
using System.Drawing;

namespace DWOS.UI.Sales
{
    public partial class CustomerInformation : DataPanel
    {
        #region Fields

        private readonly List <int> _customerFieldsLoaded = new List <int>();
        private int? _currentCustomerFieldsLoaded;
        private bool _fieldsInitialized;
        private ValidatorManager _shippingValidator = new ValidatorManager();
        private GridSettingsPersistence<UltraGridBandSettings> _fieldSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("Customers_Fields", Default);

        #endregion

        #region Properties

        public CustomersDataset Dataset
        {
            get { return base._dataset as CustomersDataset; }
            set { base._dataset = value; }
        }

        public Infragistics.Win.UltraWinToolbars.UltraToolbarsManager ToolbarsManager
        {
            get;
            set;
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.Customer.CustomerIDColumn.ColumnName; }
        }

        public CustomersDataset.CustomerAddressRow SelectedCustomerAddress
        {
            get
            {
                return SelectedNode?.DataRow;
            }
        }

        private CustomerAddressNode SelectedNode
        {
            get
            {
                return tvwAddress.SelectedNodes
                    .OfType<CustomerAddressNode>()
                    .FirstOrDefault();
            }
        }

        public static UltraGridBandSettings Default =>
            new UltraGridBandSettings
            {
                ColumnSort = new Dictionary<string, UltraGridBandSettings.ColumnSortSettings>
                {
                    { "Category", new UltraGridBandSettings.ColumnSortSettings { IsDescending = false, SortIndex = 0, IsGroupByColumn = true } }
                }
            };

        #endregion

        #region Methods

        public CustomerInformation() { InitializeComponent(); }

        public void LoadData(CustomersDataset dataset, CustomerTermsTableAdapter taCustomerTerms)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.Customer.TableName;

            bsCustomerAddress.DataSource = this.Dataset;
            bsCustomerAddress.DataMember = this.Dataset.CustomerAddress.TableName;
            bsCustomerAddress.Filter = "1 = 0";


            //bind column to control
            base.BindValue(this.txtCustomerName, Dataset.Customer.NameColumn.ColumnName);
            base.BindValue(this.txtCustomerAddress1, Dataset.Customer.Address1Column.ColumnName);
            base.BindValue(this.txtCustomerAddress2, Dataset.Customer.Address2Column.ColumnName);
            base.BindValue(this.txtCustomerCity, Dataset.Customer.CityColumn.ColumnName);
            base.BindValue(this.txtCustomerState, Dataset.Customer.StateColumn.ColumnName);
            base.BindValue(this.txtCustomerZip, Dataset.Customer.ZipColumn.ColumnName);
            base.BindValue(this.cboCustomerCountry, Dataset.Customer.CountryIDColumn.ColumnName);
            base.BindValue(this.cboTerms, Dataset.Customer.PaymentTermsColumn.ColumnName);
            base.BindValue(this.cboCustomerStanding, Dataset.Customer.CustomerStatusColumn.ColumnName);
            base.BindValue(this.txtCustomerNotes, Dataset.Customer.NotesColumn.ColumnName);
            base.BindValue(this.numLeadTime, Dataset.Customer.LeadTimeColumn.ColumnName);
            base.BindValue(this.chkEmail, Dataset.Customer.EmailInvoiceColumn.ColumnName);
            base.BindValue(this.chkPrint, Dataset.Customer.PrintInvoiceColumn.ColumnName);
            base.BindValue(this.chkOrderReview, Dataset.Customer.OrderReviewColumn.ColumnName);
            base.BindValue(this.cboPriority, Dataset.Customer.OrderPriorityColumn.ColumnName);
            base.BindValue(this.chkActive, Dataset.Customer.ActiveColumn.ColumnName);
            base.BindValue(this.txtAccountingID, Dataset.Customer.AccountingIDColumn.ColumnName);
            base.BindValue(this.cboInvoiceLevel, Dataset.Customer.InvoiceLevelIDColumn.ColumnName);
            base.BindValue(this.chkPrintCOC, Dataset.Customer.PrintCOCColumn.ColumnName);
            base.BindValue(this.chkRequireCoc, Dataset.Customer.RequireCocByDefaultColumn.ColumnName);
            base.BindValue(this.cboShowSNonApprovalSubjectLine, Dataset.Customer.ShowSNinApprovalSubjectLineColumn.ColumnName);

            //bind lists
            CustomersDataset.CustomerTermsDataTable customerTable = taCustomerTerms.GetData();
            DataUtilities.LoadComboBox(this.cboTerms, customerTable, customerTable.PaymentTermsColumn.ColumnName, customerTable.PaymentTermsColumn.ColumnName);
            base.BindList(this.cboCustomerStanding, Dataset.CustomerStatus, Dataset.CustomerStatus.StatusIDColumn.ColumnName, Dataset.CustomerStatus.StatusIDColumn.ColumnName);
            base.BindList(this.cboPriority, Dataset.d_CustomerOrderPriority, Dataset.d_CustomerOrderPriority.CustomerOrderPriorityColumn.ColumnName, Dataset.d_CustomerOrderPriority.CustomerOrderPriorityColumn.ColumnName);
            base.BindList(this.cboInvoiceLevel, Dataset.d_InvoiceLevel, Dataset.d_InvoiceLevel.InvoiceLevelIDColumn.ColumnName, Dataset.d_InvoiceLevel.InvoiceLevelIDColumn.ColumnName);
            base.BindList(this.cboCustomerCountry, Dataset.Country, Dataset.Country.CountryIDColumn.ColumnName, Dataset.Country.NameColumn.ColumnName);

            // bind shipping address fields
            txtShipName.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(txtShipName),
                this.bsCustomerAddress,
                this.Dataset.CustomerAddress.NameColumn.ColumnName,
                true));

            chkShipActive.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(chkShipActive),
                bsCustomerAddress,
                Dataset.CustomerAddress.ActiveColumn.ColumnName,
                true));

            txtShipAddress1.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(txtShipAddress1),
                this.bsCustomerAddress,
                this.Dataset.CustomerAddress.Address1Column.ColumnName,
                true));

            txtShipAddress2.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(txtShipAddress2),
                this.bsCustomerAddress,
                this.Dataset.CustomerAddress.Address2Column.ColumnName,
                true));

            txtShipCity.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(txtShipCity),
                this.bsCustomerAddress,
                this.Dataset.CustomerAddress.CityColumn.ColumnName,
                true));

            txtShipState.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(txtShipState),
                this.bsCustomerAddress,
                this.Dataset.CustomerAddress.StateColumn.ColumnName,
                true));

            cboShipCountry.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(cboShipCountry),
                this.bsCustomerAddress,
                this.Dataset.CustomerAddress.CountryIDColumn.ColumnName,
                true));

            BindList(cboShipCountry,
                Dataset.Country,
                Dataset.Country.CountryIDColumn.ColumnName,
                Dataset.Country.NameColumn.ColumnName);

            txtShipZip.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(txtShipZip),
                this.bsCustomerAddress,
                this.Dataset.CustomerAddress.ZipColumn.ColumnName,
                true));

            chkShipDefault.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(chkShipDefault),
                this.bsCustomerAddress,
                this.Dataset.CustomerAddress.IsDefaultColumn.ColumnName,
                true));

            chkShipRepairStatement.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(chkShipRepairStatement),
                this.bsCustomerAddress,
                this.Dataset.CustomerAddress.RequireRepairStatementColumn.ColumnName,
                true));

            // Reset max length for State - should be 2 but binding sets it to 50
            txtCustomerState.MaxLength = 2;

            base._panelLoaded = true;
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            var addressField = FieldUtilities.GetField("Customer", "Address");
            manager.Add(new ImageDisplayValidator(new CustomerNameControlValidator(this.txtCustomerName, this), errProvider));

            if (addressField.IsVisible)
            {
                manager.Add(new ImageDisplayValidator(
                    new TextControlValidator(this.txtCustomerAddress1, "Customer address 1 required.")
                    {
                        MinLength = 3,
                        IsRequired = addressField.IsRequired
                    }, errProvider));

                manager.Add(new ImageDisplayValidator(
                    new TextControlValidator(this.txtCustomerCity, "Customer city required.")
                    {
                        MinLength = 3,
                        IsRequired = addressField.IsRequired
                    }, errProvider));

                manager.Add(new ImageDisplayValidator(
                    new TextControlValidator(this.txtCustomerState, "Customer state required.")
                    {
                        MinLength = 2,
                        IsRequired = addressField.IsRequired
                    }, errProvider));

                manager.Add(new ImageDisplayValidator(
                    new TextControlValidator(this.txtCustomerZip, "Customer zip code required.")
                    {
                        MinLength = 5,
                        IsRequired = addressField.IsRequired
                    }, errProvider));

                manager.Add(new ImageDisplayValidator(
                    new TextControlValidator(this.cboCustomerCountry, "Customer country required.")
                    {
                        IsRequired = addressField.IsRequired
                    }, errProvider));
            }
            else
            {
                var pnlAddressHeight = pnlAddress.Height;
                pnlAddress.Visible = false;
                pnlGeneralBottom.Height += pnlAddressHeight;
            }

            if (!ApplicationSettings.Current.AllowSkippingCoc)
            {
                var pnlRequireCocHeight = pnlRequireCoc.Height;
                pnlRequireCoc.Visible = false;
                pnlGeneralBottom.Height += pnlRequireCocHeight;
            }

            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboTerms, "Customer payment terms required."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboCustomerStanding, "Customer standing selection required."), errProvider));
            manager.Add(new ImageDisplayValidator(new NumericControlValidator(this.numLeadTime), errProvider));         

            var shipToTab = tabCustomer.Tabs["ShipTo"];
            manager.Add(new TooltipDisplayValidator(tipManager, null)
            {
                RequiredStyleSetName = null,
                InvalidStyleSetName = null,
                Validator = new CustomerAddressValidator(shipToTab, tvwAddress, this)
            });

            var shipNameValidator = new ImageDisplayValidator(new TextControlValidator(this.txtShipName, "Shipping Address name required.") { MinLength = 3 }, errProvider) { Tab = shipToTab };
            var shipAddressValidator = new ImageDisplayValidator(new TextControlValidator(this.txtShipAddress1, "Shipping address 1 required.") { MinLength = 3 }, errProvider) { Tab = shipToTab };
            var shipCityValidator = new ImageDisplayValidator(new TextControlValidator(this.txtShipCity, "Shipping city required.") { MinLength = 3 }, errProvider) { Tab = shipToTab };
            var shipStateValidator = new ImageDisplayValidator(new TextControlValidator(this.txtShipState, "Shipping state required.") { MinLength = 2 }, errProvider) { Tab = shipToTab };
            var shipZipValidator = new ImageDisplayValidator(new TextControlValidator(this.txtShipZip, "Shipping zip code required.") { MinLength = 5 }, errProvider) { Tab = shipToTab };
            var shipCountryValidator = new ImageDisplayValidator(new TextControlValidator(this.cboShipCountry, "Shipping country required."), errProvider) { Tab = shipToTab };

            // _shippingValidator is used to validate the shipping tab...
            _shippingValidator.Add(shipNameValidator);
            _shippingValidator.Add(shipAddressValidator);
            _shippingValidator.Add(shipCityValidator);
            _shippingValidator.Add(shipStateValidator);
            _shippingValidator.Add(shipZipValidator);
            _shippingValidator.Add(shipCountryValidator);

            // ...but its validators must also be added to manager to
            // prevent users from submitting invalid data.
            manager.Add(shipNameValidator);
            manager.Add(shipAddressValidator);
            manager.Add(shipCityValidator);
            manager.Add(shipStateValidator);
            manager.Add(shipZipValidator);
            manager.Add(shipCountryValidator);

            chkShipRepairStatement.Visible = ApplicationSettings.Current.RepairStatementEnabled;
        }

        public CustomersDataset.CustomerRow AddCustomerRow()
        {
            var rowVw        = bsData.AddNew() as DataRowView;
            var cr           = rowVw.Row as CustomersDataset.CustomerRow;
            cr.Name          = "New Customer";
            cr.PrintInvoice  = false;
            cr.EmailInvoice  = true;
            cr.LeadTime      = ApplicationSettings.Current.OrderLeadTime;
            cr.OrderReview   = true;
            cr.OrderPriority = "Normal";
            cr.Active        = true;
            cr.InvoiceLevelID = "Default";
            cr.PrintCOC = true;
            cr.RequireCocByDefault = false;
            cr.CountryID = AddressUtilities.COUNTRY_ID_UNKNOWN;
            cr.ShowSNinApprovalSubjectLine = cboShowSNonApprovalSubjectLine.Checked;
            return cr;
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            // Unregister from ValueChanged events
            txtCustomerState.ValueChanged -= txtCustomerState_ValueChanged;

            //ensure move to first tab so Loading of Fields will occur in tab selection
            this.tabCustomer.Tabs[0].Selected = true;
            SaveFields();

            base.BeforeMoveToNewRecord(id);
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            try
            {
                base.AfterMovedToNewRecord(id);

                // Load customer addresses
                tvwAddress.Nodes.Clear();
                var currentCustomer = CurrentRecord as CustomersDataset.CustomerRow;

                if (currentCustomer != null)
                {
                    var shippingAddresses = currentCustomer.GetCustomerAddressRows();
                    foreach (var address in shippingAddresses)
                    {
                        var node = new CustomerAddressNode(address);
                        tvwAddress.Nodes.Add(node);
                    }
                }

            }
            finally
            {
                // Register ValueChanged events that were previously
                // unregistered in BeforeMoveToNewRecord.
                txtCustomerState.ValueChanged += txtCustomerState_ValueChanged;
            }
        }

        public override void EndEditing()
        {
            EndCustomerAddressEdit();
            base.EndEditing();
            SaveFields();
        }

        private void LoadFields()
        {
            try
            {
                if (!this._fieldsInitialized)
                {
                    foreach (var fieldRow in Dataset.Fields.Where(s => s.IsCustomer && s.IsVisible))
                    {
                        var row = this.dsFields.Rows.Add();
                        row.SetCellValue("FieldID", fieldRow.FieldID);
                        row.SetCellValue("Category", fieldRow.Category);
                        row.SetCellValue("Name", fieldRow.Name);
                        row.SetCellValue("Required", false);
                        row.SetCellValue("DefaultValue", null);
                    }

                    this._fieldsInitialized = true;
                }

                var customer = CurrentRecord as CustomersDataset.CustomerRow;

                if (customer != null)
                {
                    //if customer is the currently loaded customer then don't load because we haven't saved it yet
                    if (this._currentCustomerFieldsLoaded.GetValueOrDefault() == customer.CustomerID)
                        return;

                    this._currentCustomerFieldsLoaded = customer.CustomerID;

                    //if haven't load this customer's fields then load them
                    if (!this._customerFieldsLoaded.Contains(customer.CustomerID))
                    {
                        using (var ta = new Customer_FieldsTableAdapter { ClearBeforeFill = false })
                        {
                            ta.FillBy(Dataset.Customer_Fields, customer.CustomerID);
                            this._customerFieldsLoaded.Add(customer.CustomerID);
                        }
                    }

                    //Set the values for this customer
                    foreach (UltraDataRow fieldRow in this.dsFields.Rows)
                    {
                        var fieldID = Convert.ToInt32(fieldRow.GetCellValue("FieldID"));
                        var customerField = Dataset.Customer_Fields.FindByCustomerIDFieldID(customer.CustomerID, fieldID);

                        //if not customer field then add it
                        if (customerField == null)
                        {
                            customerField = Dataset.Customer_Fields.NewCustomer_FieldsRow();
                            customerField.CustomerID = customer.CustomerID;
                            customerField.FieldID = fieldID;
                            customerField.Required = false;
                            Dataset.Customer_Fields.AddCustomer_FieldsRow(customerField);
                        }

                        fieldRow.SetCellValue("Required", customerField.Required);
                        fieldRow.SetCellValue("DefaultValue", customerField.IsDefaultVaueNull() ? null : customerField.DefaultVaue);

                        fieldRow.Tag = customerField;
                    }

                    this.grdFields.DataBind();
                    this.grdFields.Rows.ExpandAll(true);
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading fields.");
            }
        }

        private void SaveFields()
        {
            try
            {
                if (this._fieldsInitialized)
                {
                    var customer = CurrentRecord as CustomersDataset.CustomerRow;

                    //if the current customer's fields where loaded then we can save them
                    if (customer != null && this._customerFieldsLoaded.Contains(customer.CustomerID))
                    {
                        foreach (UltraDataRow fieldRow in this.dsFields.Rows)
                        {
                            var customerField = fieldRow.Tag as CustomersDataset.Customer_FieldsRow;

                            if (customerField != null)
                            {
                                customerField.Required = Convert.ToBoolean(fieldRow.GetCellValue("Required"));

                                object defValue = fieldRow.GetCellValue("DefaultValue");
                                customerField.DefaultVaue = defValue == null || defValue == DBNull.Value ? null : Convert.ToString(defValue);
                            }
                        }

                        this._currentCustomerFieldsLoaded = null; //null out to reset it so will be loaded next time
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error saving fields.");
            }
        }

        private CustomerAddressNode AddCustomerAddress()
        {
            var currentCustomer = CurrentRecord as CustomersDataset.CustomerRow;

            var isDataInvalid = currentCustomer == null ||
                !IsShippingAddressValid() ||
                string.IsNullOrEmpty(currentCustomer.Name) ||
                currentCustomer.IsAddress1Null() ||
                string.IsNullOrEmpty(currentCustomer.Address1) ||
                currentCustomer.IsCityNull() ||
                string.IsNullOrEmpty(currentCustomer.City) ||
                currentCustomer.IsStateNull() ||
                string.IsNullOrEmpty(currentCustomer.State) ||
                currentCustomer.IsZipNull() ||
                string.IsNullOrEmpty(currentCustomer.Zip);

            if (isDataInvalid)
            {
                return null;
            }

            var currentCustomerAddresses = currentCustomer.GetCustomerAddressRows();
            bool isDefault = currentCustomerAddresses.Count(addr => addr.IsDefault) == 0;

            var address = Dataset.CustomerAddress.NewCustomerAddressRow();

            if (currentCustomerAddresses.Length > 0)
            {
                int addressNumber = currentCustomerAddresses.Length + 1;
                address.Name = string.Format("{0} #{1}", currentCustomer.Name, addressNumber);
            }
            else
            {
                address.Name = currentCustomer.Name;
            }
            address.Address1 = currentCustomer.Address1;

            if (!currentCustomer.IsAddress2Null())
            {
                address.Address2 = currentCustomer.Address2;
            }

            address.City = currentCustomer.City;
            address.State = currentCustomer.State;
            address.Zip = currentCustomer.Zip;
            address.CustomerRow = currentCustomer;
            address.CountryID = currentCustomer.CountryID;
            address.IsDefault = isDefault;

            address.RequireRepairStatement = ApplicationSettings.Current.CompanyCountry !=
                address.CountryID;

            Dataset.CustomerAddress.AddCustomerAddressRow(address);

            var addressNode = new CustomerAddressNode(address);
            tvwAddress.Nodes.Add(addressNode);

            return addressNode;
        }

        public void RemoveSelectedAddress()
        {
            CustomerAddressNode selection = SelectedNode;

            bool isDefault = SelectedNode?.DataRow?.IsDefault ?? false;

            if (selection == null || isDefault)
            {
                if (selection != null && isDefault)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("Default addresses cannot be deleted.", "Delete Address");
                }

                return;
            }
            else if (tvwAddress.Nodes.Count == 1)
            {
                MessageBoxUtilities.ShowMessageBoxWarn("Customer must have at least one shipping address.", "Delete Address");
                return;
            }

            // Select, then delete.
            // Validation errors trigger if the selection is deleted first.
            tvwAddress.SelectedNodes.Clear();
            selection.Delete();
        }

        private void EndCustomerAddressEdit()
        {
            bsCustomerAddress.EndEdit();
            if (this.bsCustomerAddress.Current is DataRowView && ((DataRowView)this.bsCustomerAddress.Current).IsEdit)
            {
                ((DataRowView)this.bsCustomerAddress.Current).EndEdit();
            }

            SelectedNode?.UpdateNodeUI();
        }
        private bool IsShippingAddressValid()
        {
            if (tvwAddress.SelectedNodes.Count == 0)
            {
                return true;
            }

            var selection = tvwAddress.SelectedNodes[0];

            return selection.Control == null ||
                _shippingValidator.ValidateControls();
        }

        #endregion

        #region Events

        private void tabCustomer_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            bool tabIsShipTo = e.Tab?.Key == "ShipTo";

            if (e.Tab.Text == "Fields")
            {
                LoadFields();
            }
            else if (tabIsShipTo)
            {
                // Select the first node after changing tab. Changing
                // selection during node creation does not work every time.
                if (tvwAddress.SelectedNodes.Count == 0 && tvwAddress.Nodes.Count > 0)
                {
                    tvwAddress.Nodes[0].Select();
                    pnlShipTo.Show();
                }
                else if (tvwAddress.Nodes.Count == 0)
                {
                    pnlShipTo.Hide();
                }
            }

            if (ToolbarsManager == null)
            {
                _log.ConditionalDebug("ToolbarsManager should not be null.");
            }
            else
            {
                var ribbonTabGroup = ToolbarsManager.Ribbon.ContextualTabGroups["shipTo"];
                var ribbonTab = ToolbarsManager.Ribbon.Tabs["shipTo"];

                ribbonTabGroup.Visible = tabIsShipTo;

                if (tabIsShipTo)
                {
                    ToolbarsManager.Ribbon.SelectedTab = ribbonTab;
                }
            }
        }

        private void txtCustomerState_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow letters and backspace
            if(!char.IsLetter(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void tvwAddress_BeforeSelect(object sender, Infragistics.Win.UltraWinTree.BeforeSelectEventArgs e)
        {
            if (e.NewSelections.Count == 0)
            {
                return;
            }

            if (!IsShippingAddressValid())
            {
                e.Cancel = true;
            }

            EndCustomerAddressEdit();
        }

        private void tvwAddress_AfterSelect(object sender, Infragistics.Win.UltraWinTree.SelectEventArgs e)
        {
            chkShipDefault.CheckedChanged -= chkShipDefault_CheckedChanged;
            txtShipState.ValueChanged -= txtShipState_ValueChanged;
            cboShipCountry.ValueChanged -= cboShipCountry_ValueChanged;

            try
            {
                // Show newly selected shipping address
                var selection = e.NewSelections.OfType<CustomerAddressNode>().FirstOrDefault();

                pnlShipTo.Enabled = selection != null; // controls shipping tab validation
                pnlShipTo.Visible = selection != null;

                if (selection == null)
                {
                    bsCustomerAddress.Filter = "1 = 0";
                }
                else
                {
                    var currentAddress = selection.DataRow;
                    bsCustomerAddress.Filter = string.Format("{0} = {1}",
                        Dataset.CustomerAddress.CustomerAddressIDColumn.ColumnName,
                        currentAddress.CustomerAddressID);
                }
            }
            finally
            {
                chkShipDefault.CheckedChanged += chkShipDefault_CheckedChanged;
                txtShipState.ValueChanged += txtShipState_ValueChanged;
                cboShipCountry.ValueChanged += cboShipCountry_ValueChanged;
            }
        }

        private void txtShipName_Validated(object sender, EventArgs e)
        {
            // Refresh name of selected shipping address node.
            var selection = tvwAddress.SelectedNodes
                .OfType<CustomerAddressNode>()
                .FirstOrDefault();

            if (selection != null)
            {
                selection.UpdateNodeUI();
            }
        }

        private void chkShipDefault_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkShipDefault.Checked)
            {
                return;
            }

            var currentCustomer = CurrentRecord as CustomersDataset.CustomerRow;
            var customerAddresses = currentCustomer?.GetCustomerAddressRows();

            var currentShippingAddress = SelectedCustomerAddress;
            if (currentShippingAddress != null && customerAddresses != null)
            {
                foreach (var otherAddress in customerAddresses.Where(addr => addr.CustomerAddressID != currentShippingAddress.CustomerAddressID))
                {
                    otherAddress.IsDefault = false;
                }
            }
        }

        private void txtCustomerState_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                // Update country for customer address
                var newCountryId = AddressUtilities.GetCountryId(txtCustomerState.Text);
                var currentCountryId = Convert.ToInt32(cboCustomerCountry.Value);

                if (newCountryId != AddressUtilities.COUNTRY_ID_UNKNOWN && newCountryId != currentCountryId)
                {
                    cboCustomerCountry.Value = newCountryId;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing customer's state.");
            }
        }

        private void txtShipState_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                // Update country for shipping address
                var newCountryId = AddressUtilities.GetCountryId(txtShipState.Text);
                var currentCountryId = Convert.ToInt32(cboShipCountry.Value);

                if (newCountryId != AddressUtilities.COUNTRY_ID_UNKNOWN && newCountryId != currentCountryId)
                {
                    cboShipCountry.Value = newCountryId;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing shipping address's state.");
            }
        }

        private void cboShipCountry_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                // Update 'require repair statement' depending on country.
                chkShipRepairStatement.Checked = ApplicationSettings.Current.CompanyCountry !=
                    Convert.ToInt32(cboShipCountry.Value);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing shipping address's country.");
            }
        }

        private void grdFields_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                grdFields.AfterColPosChanged -= grdFields_AfterColPosChanged;
                grdFields.AfterSortChange -= grdFields_AfterSortChange;

                // Because grdFields uses an UltraDataSource, it's apparent
                // that all fields are visible by default.
                var band = e.Layout.Bands[0];
                band.Columns["FieldID"].Hidden = true;

                // Load settings
                _fieldSettingsPersistence.LoadSettings().ApplyTo(band);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error initializing fields layout.");
            }
            finally
            {
                grdFields.AfterColPosChanged += grdFields_AfterColPosChanged;
                grdFields.AfterSortChange += grdFields_AfterSortChange;
            }
        }


        private void grdFields_AfterColPosChanged(object sender, Infragistics.Win.UltraWinGrid.AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdFields.DisplayLayout.Bands[0]);
                _fieldSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing column position in grid.");
            }
        }


        private void grdFields_AfterSortChange(object sender, BandEventArgs e)
        {
            try
            {
                // Save settings - this event gets called after enabling/disabling group by.
                // AfterColPosChanged gets fired too, but the grid's settings have not been updated
                // to include the change of sort.
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdFields.DisplayLayout.Bands[0]);
                _fieldSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing grid sort.");
            }
        }

        private void btnPricePoints_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(CurrentRecord is CustomersDataset.CustomerRow currentCustomer))
                {
                    return;
                }

                var window = new PricePointDialog(new CustomerPricePointContext(Dataset, currentCustomer));
                var helper = new WindowInteropHelper(window) { Owner = DWOSApp.MainForm.Handle };
                window.ShowDialog();

                GC.KeepAlive(helper);
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error showing price points.", exc);
            }
        }

        private void btnDefaultFees_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(CurrentRecord is CustomersDataset.CustomerRow currentCustomer))
                {
                    return;
                }

                var window = new CustomerFeesDialog();
                window.LoadData(Dataset, currentCustomer.CustomerID);
                var helper = new WindowInteropHelper(window) { Owner = DWOSApp.MainForm.Handle };
                window.ShowDialog();

                GC.KeepAlive(helper);
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error showing default fees.", exc);
            }
        }

        #endregion

        #region CustomerName Validator

        private class CustomerNameControlValidator : ControlValidatorBase
        {
            #region Fields

            private CustomerInformation _customerInfo;

            #endregion

            #region Methods

            public CustomerNameControlValidator(Control control, CustomerInformation customerInfo) : base(control) { this._customerInfo = customerInfo; }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                var minimumLength = 3;
                var editor = Control as UltraTextEditor;

                if(editor != null && editor.Enabled)
                {
                    if (!IsOnlyActiveCustomerName() || !IsOnlyActiveCustomerNameLocal())
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "Only one customer with the name '" + editor.Text + "' can exist. There may be an inactive customer with the same name or changes may not be saved.");
                        return;
                    }
                    else if (string.IsNullOrWhiteSpace(editor.Text))
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "Customer name is required.");
                        return;
                    }
                    else if (editor.Text.Length < minimumLength)
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "Customer name must be longer than " + minimumLength + " characters.");
                        return;
                    }
                }

                //passed
                e.Cancel = false;
                FireAfterValidation(true, String.Empty);
            }

            private bool IsOnlyActiveCustomerName()
            {
                var customer = this._customerInfo.CurrentRecord as CustomersDataset.CustomerRow;

                if(customer == null)
                    return true;

                var dt = new CustomersDataset.CustomerDataTable();
                using(var taCustomers = new CustomerTableAdapter())
                    taCustomers.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    var activeCustomers = dt.Select("CustomerID <> " + customer.CustomerID + " AND Name = '" + Data.Datasets.Utilities.SqlBless(Control.Text) + "'");
                    //If changed name of customer but didnt save
                    //activeCustomers.ForEach(ac => _customerInfo.Dataset.Customer.FindByCustomerID(ac.GetInt32("CustomerID")).Name == _control.Text);

                    return activeCustomers.Length <= 0;
                }

                return true;
            }

            private bool IsOnlyActiveCustomerNameLocal()
            {
                var customer = _customerInfo.CurrentRecord as CustomersDataset.CustomerRow;

                if(customer == null)
                    return true;

                var activeCustomers = _customerInfo.Dataset.Customer
                    .Select("CustomerID <> " + customer.CustomerID + " AND Name = '" + Data.Datasets.Utilities.SqlBless(Control.Text) + "'");

                return activeCustomers.Length <= 0;
            }

            public override void Dispose()
            {
                this._customerInfo = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region CustomerAddressNode

        private sealed class CustomerAddressNode : DataNode<CustomersDataset.CustomerAddressRow>
        {
            #region Fields

            private const string KEY_PREFIX = "ADDR";
            private Lazy<int> _usageCount;

            #endregion

            #region Properties

            public int UsageCount
            {
                get
                {
                    return _usageCount.Value;
                }
            }

            #endregion

            #region Methods

            public CustomerAddressNode(CustomersDataset.CustomerAddressRow row)
                : base(row, row.CustomerAddressID.ToString(), KEY_PREFIX, row.Name)
            {
                LeftImages.Add(Properties.Resources.Address_16);
                _usageCount = new Lazy<int>(() =>
                {
                    using (var taAddress = new CustomerAddressTableAdapter())
                    {
                        return taAddress.GetUsageCount(DataRow.CustomerAddressID) ?? 0;
                    }
                });

                UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                if (DataRow == null || !DataRow.IsValidState())
                {
                    return;
                }

                if (!string.IsNullOrEmpty(DataRow?.Name))
                {
                    Text = DataRow.Name;
                }

                if (DataRow.Active)
                {
                    Override.NodeAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.False;
                    Override.NodeAppearance.ResetForeColor();
                }
                else
                {
                    Override.NodeAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True;
                    Override.NodeAppearance.ForeColor = Color.Gray;
                }
            }

            #endregion
        }

        #endregion

        #region CustomerAddressValidator

        private sealed class CustomerAddressValidator : ControlValidatorBase
        {
            #region Properties

            public UltraTab Tab
            {
                get;
                private set;
            }

            public Infragistics.Win.UltraWinTree.UltraTree AddressTree
            {
                get;
                private set;
            }

            public CustomerInformation CustomerInformation
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public CustomerAddressValidator(UltraTab tab, Infragistics.Win.UltraWinTree.UltraTree addressTree, CustomerInformation customerInformation)
                : base(tab.TabPage)
            {
                Tab = tab;
                AddressTree = addressTree;
                CustomerInformation = customerInformation;
            }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                if (AddressTree != null && AddressTree.Enabled)
                {
                    if (AddressTree.Nodes.Count == 0)
                    {
                        CustomerInformation.AddCustomerAddress();
                    }
                }

                e.Cancel = false;
                FireAfterValidation(true, string.Empty);
            }

            #endregion
        }

        #endregion

        #region AddAddressCommand

        internal sealed class AddAddressCommand : CommandBase
        {
            #region Properties

            public CustomerInformation CustomerInfo
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public AddAddressCommand(ToolBase tool, CustomerInformation customerInfo)
                : base(tool)
            {
                CustomerInfo = customerInfo;
            }

            public override void OnClick()
            {
                try
                {
                    CustomerInfo.EndCustomerAddressEdit();
                    var newNode = CustomerInfo.AddCustomerAddress();
                    newNode?.Select();
                }
                catch (Exception exc)
                {
                    _log.Error(exc, "Error adding customer shipping address.");
                }
            }

            #endregion
        }

        #endregion

        #region RemoveAddressCommand

        internal sealed class RemoveAddressCommand : TreeNodeCommandBase
        {
            #region Properties

            public CustomerInformation CustomerInfo
            {
                get;
                private set;
            }

            public override bool Enabled
            {
                get
                {
                    return base.Enabled &&
                        CustomerInfo.SelectedNode?.UsageCount == 0 &&
                        CustomerInfo.SelectedCustomerAddress != null &&
                        !CustomerInfo.SelectedCustomerAddress.IsDefault;
                }
            }

            #endregion

            #region Methods

            public RemoveAddressCommand(ToolBase tool, CustomerInformation customerInfo)
                : base(tool)
            {
                CustomerInfo = customerInfo;
                base.TreeView = customerInfo.tvwAddress;
            }

            public override void OnClick()
            {
                try
                {
                    CustomerInfo.EndCustomerAddressEdit();
                    CustomerInfo.RemoveSelectedAddress();
                }
                catch (Exception exc)
                {
                    _log.Error(exc, "Error removing customer shipping address.");
                }
            }

            #endregion
        }

        #endregion
    }
}