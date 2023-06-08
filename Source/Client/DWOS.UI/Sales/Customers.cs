using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Interop;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Properties;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using DWOS.Data;
using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using NLog;

namespace DWOS.UI
{
    public partial class Customers : DataEditorBase
    {
        #region Fields

        private int _lastSelectedContactID = -1;
        private bool _isInactiveVisible;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the list of customers to show.
        /// </summary>
        public List<int> CustomerFilterIds { get; set; }

        /// <summary>
        ///     Gets the selected contact ID.
        /// </summary>
        /// <value> The selected contact ID. </value>
        public int SelectedContactID
        {
            get { return this._lastSelectedContactID; }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes a new instance of the <see cref="Customers" /> class.
        /// </summary>
        public Customers()
        {
            InitializeComponent();

            tvwTOC.Override.SortComparer = new DefaultNodeSorter();
        }

        /// <summary>
        ///     Loads the data.
        /// </summary>
        private void LoadData()
        {
            this.dsCustomers.EnforceConstraints = false;

            taPriceUnit.Fill(dsCustomers.d_PriceUnit);
            taCountry.Fill(dsCustomers.Country);

            using(new UsingDataTableLoad(this.dsCustomers.Customer))
            {
                this.taCustomer.Fill(this.dsCustomers.Customer);
            }

            this.taCustomField.Fill(this.dsCustomers.CustomField);
            taPartLevelCustomField.Fill(dsCustomers.PartLevelCustomField);

            using(var ta = new d_CustomerOrderPriorityTableAdapter())
                ta.Fill(this.dsCustomers.d_CustomerOrderPriority);

            using(var ta = new ReportTypeTableAdapter())
                ta.Fill(this.dsCustomers.ReportType);

            this.taPartMarking.Fill(this.dsCustomers.PartMarking);
            this.taAirframe.Fill(this.dsCustomers.d_Airframe);
            this.taManufacturer.Fill(this.dsCustomers.d_Manufacturer);
            this.taCustomerStatus.Fill(this.dsCustomers.CustomerStatus);
            this.taShippingCarrier.Fill(this.dsCustomers.d_ShippingCarrier);
            this.taInvoicePreference.Fill(this.dsCustomers.d_InvoicePreference);
            this.tad_InvoiceLevelTableAdapter.Fill(this.dsCustomers.d_InvoiceLevel);
            this.taFields.FillByIsCustomer(this.dsCustomers.Fields, true); //Get customer level fields
            this.taLists.Fill(this.dsCustomers.Lists);

            this.dsCustomers.EnforceConstraints = true;

            this.dpCustomerInformation.LoadData(this.dsCustomers, this.taCustomerTerms);
            base.AddDataPanel(this.dpCustomerInformation);

            this.dpCustomerContacts.LoadData(this.dsCustomers);
            base.AddDataPanel(this.dpCustomerContacts);

            this.dpCustomerShipping.LoadData(this.dsCustomers);
            base.AddDataPanel(this.dpCustomerShipping);

            this.dpPartMarking.LoadData(this.dsCustomers);
            base.AddDataPanel(this.dpPartMarking);

            this.dpCustomField.LoadData(this.dsCustomers);
            base.AddDataPanel(this.dpCustomField);

            dpPartCustomField.LoadData(dsCustomers);
            AddDataPanel(dpPartCustomField);

            dpRelatedContact.LoadData(dsCustomers);
            AddDataPanel(dpRelatedContact);

            dsCustomers.Contact.ContactRowDeleting += ContactOnContactRowDeleting;
            dsCustomers.Contact.ContactRowChanged += ContactOnContactRowChanged;
            dsCustomers.ContactAdditionalCustomer.ContactAdditionalCustomerRowDeleting += ContactAdditionalCustomer_RowDeleting;
            dsCustomers.ContactAdditionalCustomer.ContactAdditionalCustomerRowChanged += ContactAdditionalCustomer_ContactAdditionalCustomerRowChanged;
        }

        /// <summary>
        ///     Loads the TOC.
        /// </summary>
        private void LoadTOC()
        {
            using(new UsingTreeLoad(tvwTOC))
            {
                tvwTOC.Nodes.Clear();

                UltraTreeNode rootNode = new CustomersRootNode(this.dsCustomers);
                tvwTOC.Nodes.Add(rootNode);
                rootNode.Expanded = true;

                var showAll = ((StateButtonTool) toolbarManager.Tools["DisplayInActive"]).Checked;

                foreach (var cr in this.dsCustomers.Customer)
                {
                    if (CustomerFilterIds == null || CustomerFilterIds.Contains(cr.CustomerID))
                    {
                        rootNode.Nodes.Add(new CustomerNode(cr) { Visible = showAll || cr.Active });
                    }
                }
            }
        }

        protected override void ReloadTOC() { LoadTOC(); }

        /// <summary>
        ///     Loads the commands.
        /// </summary>
        private void LoadCommands()
        {
            if(SecurityManager.Current.IsInRole("Customers.Edit"))
            {
                //only add customers if not restricted by customer
                if (CustomerFilterIds == null || CustomerFilterIds.Count == 0)
                {
                    base.Commands.AddCommand("AddCustomer", new AddCustomerCommand(toolbarManager.Tools["AddCustomer"], tvwTOC) { AddCallback = AddNode });
                }

                base.Commands.AddCommand("AddContact", new AddContactCommand(toolbarManager.Tools["AddContact"], tvwTOC) {AddCallback = AddNode});
            }

            if(SecurityManager.Current.IsInRole("Customers.Edit") || SecurityManager.Current.IsInRole("Customers.Shipping"))
                base.Commands.AddCommand("AddShipping", new AddShippingCommand(toolbarManager.Tools["AddShipping"], tvwTOC) {AddCallback = AddNode});

            if(SecurityManager.Current.IsInRole("Customers.Edit") || SecurityManager.Current.IsInRole("Customers.PartMark"))
            {
                base.Commands.AddCommand("AddPartMarking", new AddPartMarkingCommand(toolbarManager.Tools["AddPartMarking"], tvwTOC) {AddCallback = AddNode});
                base.Commands.AddCommand("Copy", new CopyPasteCommand(toolbarManager.Tools["Copy"], tvwTOC));
            }

            if(SecurityManager.Current.IsInRole("Customers.Edit") || SecurityManager.Current.IsInRole("Customers.PartMark") || SecurityManager.Current.IsInRole("Customers.Shipping"))
            {
                base.Commands.AddCommand("Delete", new DeleteCommand(toolbarManager.Tools["Delete"], tvwTOC, this));
                base.Commands.AddCommand("AddCustomField", new AddCustomFieldCommand(toolbarManager.Tools["AddCustomField"], tvwTOC) {AddCallback = AddNode});
                base.Commands.AddCommand("AddPartLevelCustomField", new AddPartLevelCustomFieldCommand(toolbarManager.Tools["AddPartLevelCustomField"], tvwTOC) {AddCallback = AddNode});

                base.Commands.AddCommand("CopyFields", new CopyCustomFieldsCommand(toolbarManager.Tools["CopyFields"], tvwTOC, this));
            }

            var displayInactiveCommand = new DisplayInactiveCommand(toolbarManager.Tools["DisplayInActive"], tvwTOC);

            displayInactiveCommand.ShowInactive += DisplayInactiveCommandOnShowInactive;
            displayInactiveCommand.HideInactive += DisplayInactiveCommandOnHideInactive;
            Commands.AddCommand("DisplayInactiveCommand", displayInactiveCommand);

            base.Commands.AddCommand("AddAddress", new Sales.CustomerInformation.AddAddressCommand(toolbarManager.Tools["AddAddress"], dpCustomerInformation));
            base.Commands.AddCommand("RemoveAddress", new Sales.CustomerInformation.RemoveAddressCommand(toolbarManager.Tools["RemoveAddress"], dpCustomerInformation));
        }

        protected override bool SaveData()
        {
            try
            {
                base.EndAllEdits();

                this.taManager.UpdateAll(this.dsCustomers);

                return true;
            }
            catch(Exception exc)
            {
                _log.Warn(this.dsCustomers.GetDataErrors());
                ErrorMessageBox.ShowDialog("Error saving data.", exc);
                return false;
            }
        }

        /// <summary>
        ///     Adds the contact.
        /// </summary>
        /// <param name="rn"> The rn. </param>
        private void AddContact(CustomerNode rn)
        {
            //create new data source
            CustomersDataset.ContactRow cr = this.dpCustomerContacts.AddContactRow(rn.DataRow.CustomerID);

            //create new ui nodes
            var cn = new ContactNode(cr, true);
            rn.Nodes.Add(cn);
            cn.Select();
        }

        /// <summary>
        ///     Adds the shipping.
        /// </summary>
        /// <param name="rn"> The rn. </param>
        private void AddShipping(CustomerNode rn)
        {
            //create new data source
            CustomersDataset.CustomerShippingRow cr = this.dpCustomerShipping.AddShippingRow(rn.DataRow.CustomerID);

            //create new ui nodes
            var cn = new ShippingNode(cr);
            rn.Nodes.Add(cn);
            cn.Select();
        }

        /// <summary>
        ///     Adds the custom field.
        /// </summary>
        /// <param name="rn"> The rn. </param>
        private void AddCustomField(CustomerNode rn)
        {
            //create new data source
            var cr = this.dpCustomField.AddRow(rn.DataRow.CustomerID);

            //create new ui nodes
            var cn = new CustomFieldNode(cr);
            rn.Nodes.Add(cn);
            cn.Select();
        }

        /// <summary>
        /// Adds a part-level custom field.
        /// </summary>
        /// <param name="customerNode">
        /// The customer node to add a part-level custom field for.
        /// </param>
        private void AddPartLevelCustomField(CustomerNode customerNode)
        {
            var fieldRow = dsCustomers.PartLevelCustomField.NewPartLevelCustomFieldRow();
            fieldRow.CustomerID = customerNode.DataRow.CustomerID;
            fieldRow.Name = "New Custom Field";
            fieldRow.DisplayOnCOC = false;
            fieldRow.DisplayOnTraveler = false;
            fieldRow.IsVisible = true;

            dsCustomers.PartLevelCustomField.AddPartLevelCustomFieldRow(fieldRow);

            var node = new PartCustomFieldNode(fieldRow);
            customerNode.Nodes.Add(node);
            node.Select();
        }

        /// <summary>
        /// Adds a custom field to each of the given customers.
        /// </summary>
        /// <param name="customerNodes">Customers to add a custom field for.</param>
        private void BulkCustomField(IEnumerable<CustomerNode> customerNodes)
        {
            if (customerNodes == null || customerNodes.Count() == 0)
            {
                return;
            }

            var existingFields = dsCustomers.CustomField
                .Where(field => field.IsValidState())
                .ToList();

            var window = new Sales.Customer.BulkCustomFieldDialog(existingFields, dsCustomers.Lists);
            var helper = new WindowInteropHelper(window) { Owner = DWOSApp.MainForm.Handle };

            if (window.ShowDialog() ?? false)
            {
                var windowData = window.Data;
                var tokenName = windowData.TokenName.ToPartMarkingString();

                foreach (var customerNode in customerNodes)
                {
                    var customer = customerNode.DataRow;
                    var existingField = customer.GetCustomFieldRows()
                        .FirstOrDefault(field => field.IsValidState() && field.Name == windowData.DisplayName);

                    if (existingField == null)
                    {
                        var customField = dsCustomers.CustomField.NewCustomFieldRow();
                        customField.Name = windowData.DisplayName.TrimToMaxLength(50);
                        customField.Description = windowData.Description;
                        customField.DisplayOnTraveler = windowData.PrintOnTraveler;
                        customField.DisplayOnCOC = windowData.PrintOnCOC;
                        customField.Required = windowData.IsRequired;
                        customField.TokenName = tokenName;
                        customField.CustomerRow = customer;
                        customField.DefaultValue = windowData.DefaultValue;
                        customField.ProcessUnique = windowData.IsProcessUnique;
                        customField.IsVisible = windowData.IsVisible;

                        if (windowData.ListId.HasValue)
                        {
                            customField.ListID = windowData.ListId.Value;
                        }

                        dsCustomers.CustomField.AddCustomFieldRow(customField);

                        if (customerNode.IsDataLoaded)
                        {
                            var fieldNode = new CustomFieldNode(customField);
                            customerNode.Nodes.Add(fieldNode);
                        }
                    }
                    else
                    {
                        // edit existing
                        existingField.Description = windowData.Description;
                        existingField.DisplayOnTraveler = windowData.PrintOnTraveler;
                        existingField.DisplayOnCOC = windowData.PrintOnCOC;
                        existingField.ProcessUnique = windowData.IsProcessUnique;
                        existingField.Required = windowData.IsRequired;
                        existingField.TokenName = tokenName;
                        existingField.DefaultValue = windowData.DefaultValue;
                        existingField.IsVisible = windowData.IsVisible;

                        if (windowData.ListId.HasValue)
                        {
                            existingField.ListID = windowData.ListId.Value;
                        }
                        else
                        {
                            existingField.SetListIDNull();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a part-level custom field to each of the given customers.
        /// </summary>
        /// <param name="customerNodes">Customers to add a custom field for.</param>
        private void BulkPartCustomField(IEnumerable<CustomerNode> customerNodes)
        {
            if (customerNodes == null || customerNodes.Count() == 0)
            {
                return;
            }

            var existingFields = dsCustomers.PartLevelCustomField
                .Where(field => field.IsValidState())
                .ToList();

            var window = new Sales.Customer.BulkPartCustomFieldDialog(existingFields, dsCustomers.Lists);
            var helper = new WindowInteropHelper(window) { Owner = DWOSApp.MainForm.Handle };

            if (window.ShowDialog() ?? false)
            {
                var windowData = window.Data;

                foreach (var customerNode in customerNodes)
                {
                    var customer = customerNode.DataRow;
                    var existingField = customer.GetPartLevelCustomFieldRows()
                        .FirstOrDefault(field => field.IsValidState() && field.Name == windowData.DisplayName);

                    if (existingField == null)
                    {
                        var customField = dsCustomers.PartLevelCustomField.NewPartLevelCustomFieldRow();
                        customField.Name = windowData.DisplayName.TrimToMaxLength(50);
                        customField.Description = windowData.Description;
                        customField.DisplayOnTraveler = windowData.PrintOnTraveler;
                        customField.DisplayOnCOC = windowData.PrintOnCOC;
                        customField.CustomerRow = customer;
                        customField.DefaultValue = windowData.DefaultValue;
                        customField.IsVisible = windowData.IsVisible;

                        if (windowData.ListId.HasValue)
                        {
                            customField.ListID = windowData.ListId.Value;
                        }

                        dsCustomers.PartLevelCustomField.AddPartLevelCustomFieldRow(customField);

                        if (customerNode.IsDataLoaded)
                        {
                            var fieldNode = new PartCustomFieldNode(customField);
                            customerNode.Nodes.Add(fieldNode);
                        }
                    }
                    else
                    {
                        // edit existing
                        existingField.Description = windowData.Description;
                        existingField.DisplayOnTraveler = windowData.PrintOnTraveler;
                        existingField.DisplayOnCOC = windowData.PrintOnCOC;
                        existingField.DefaultValue = windowData.DefaultValue;
                        existingField.IsVisible = windowData.IsVisible;

                        if (windowData.ListId.HasValue)
                        {
                            existingField.ListID = windowData.ListId.Value;
                        }
                        else
                        {
                            existingField.SetListIDNull();
                        }
                    }
                }
            }
        }


        /// <summary>
        ///     Adds the customer.
        /// </summary>
        /// <param name="rn"> The rn. </param>
        private void AddCustomer(CustomersRootNode rn)
        {
            //create new data source
            CustomersDataset.CustomerRow cr = this.dpCustomerInformation.AddCustomerRow();

            //create new ui nodes
            var cn = new CustomerNode(cr);
            rn.Nodes.Add(cn);
            cn.Select();
        }

        /// <summary>
        ///     Adds the part marking.
        /// </summary>
        /// <param name="rn"> The rn. </param>
        private void AddPartMarking(CustomerNode rn)
        {
            if (dsCustomers.d_Airframe.Count == 0)
            {
                MessageBoxUtilities.ShowMessageBoxWarn(
                    "Please add at least one model before adding a part marking for a customer.",
                    "Customer Manager");

                return;
            }

            //create new data source
            CustomersDataset.PartMarkingRow cr = this.dpPartMarking.AddPartMarkingRow(rn.DataRow);

            //create new ui nodes
            var cn = new PartMarkingNode(cr);
            rn.Nodes.Add(cn);
            cn.Select();
        }

        /// <summary>
        ///     Loads a node in the display.
        /// </summary>
        /// <param name="node"> The node. </param>
        protected override void LoadNode(UltraTreeNode node)
        {
            toolbarManager.Ribbon.ContextualTabGroups["shipTo"].Visible = false;

            if(node is CustomerNode)
            {
                var customerNode = (CustomerNode) node;

                //Update usage count if not set
                if (!customerNode.UsageCount.HasValue)
                {
                    var usageCount = this.taCustomer.UsageCount(customerNode.DataRow.CustomerID) as int?;
                    customerNode.UsageCount = usageCount.GetValueOrDefault(0);
                }

                //Lazy load children Data and Nodes
                ((CustomerNode) node).LoadChildrenNodes(this);

                this.dpCustomerInformation.MoveToRecord(customerNode.ID);
                this.dpCustomerInformation.Editable = customerNode.UsageCount.GetValueOrDefault() < 1;

                if(SecurityManager.Current.IsInRole("Customers.Edit"))
                    splitContainer1.Panel2.Enabled = true;
                else
                    splitContainer1.Panel2.Enabled = false;

                DisplayPanel(this.dpCustomerInformation);
            }
            else if(node is ContactNode contactNode)
            {
                if(!(contactNode).UsageCount.HasValue)
                    (contactNode).UsageCount = this.taContact.UsageCount((contactNode).DataRow.ContactID).GetValueOrDefault(0);

                this.dpCustomerContacts.MoveToRecord((contactNode).ID);
                if(SecurityManager.Current.IsInRole("Customers.Edit"))
                    splitContainer1.Panel2.Enabled = true;
                else
                    splitContainer1.Panel2.Enabled = false;

                DisplayPanel(this.dpCustomerContacts);
            }
            else if(node is ShippingNode)
            {
                var shippingNode = node as ShippingNode;
                this.dpCustomerShipping.MoveToRecord(((ShippingNode) node).ID);

                //Update usage count if not set
                if (!shippingNode.UsageCount.HasValue)
                {
                    var usageCount = this.taCustomerShipping.UsageCount(shippingNode.DataRow.CustomerShippingID);
                    shippingNode.UsageCount = usageCount ?? 0;
                }

                if(SecurityManager.Current.IsInRole("Customers.Edit") || SecurityManager.Current.IsInRole("Customers.Shipping"))
                {
                    this.dpCustomerShipping.Editable = true;
                    splitContainer1.Panel2.Enabled = true;
                }
                else
                    splitContainer1.Panel2.Enabled = false;

                DisplayPanel(this.dpCustomerShipping);
            }
            else if(node is PartMarkingNode)
            {
                this.dpPartMarking.MoveToRecord(((PartMarkingNode) node).ID);
                this.dpPartMarking.LoadCustomCodes(((CustomerNode) node.Parent).DataRow.CustomerID);

                if(SecurityManager.Current.IsInRole("Customers.Edit") || SecurityManager.Current.IsInRole("Customers.PartMark"))
                {
                    this.dpPartMarking.Editable = true;
                    splitContainer1.Panel2.Enabled = true;
                }
                else
                    splitContainer1.Panel2.Enabled = false;

                DisplayPanel(this.dpPartMarking);
            }
            else if(node is CustomFieldNode)
            {
                this.dpCustomField.MoveToRecord(((CustomFieldNode) node).ID);

                if(SecurityManager.Current.IsInRole("Customers.Edit") || SecurityManager.Current.IsInRole("Customers.Shipping") || SecurityManager.Current.IsInRole("Customers.PartMark"))
                {
                    this.dpCustomField.Editable = true;
                    splitContainer1.Panel2.Enabled = true;
                }
                else
                    splitContainer1.Panel2.Enabled = false;

                DisplayPanel(this.dpCustomField);
            }
            else if (node is PartCustomFieldNode pcfNode)
            {
                dpPartCustomField.MoveToRecord(pcfNode.ID);

                pcfNode.UsageCount = taPartLevelCustomField.GetUsageCount(pcfNode.DataRow.PartLevelCustomFieldID)
                    ?? 0;

                if (SecurityManager.Current.IsInRole("Customers.Edit"))
                {
                    dpPartCustomField.Editable = true;
                    splitContainer1.Panel2.Enabled = true;
                }
                else
                {
                    splitContainer1.Panel2.Enabled = false;
                }

                DisplayPanel(dpPartCustomField);
            }
            else if (node is AdditionalContactNode)
            {
                dpRelatedContact.MoveToRecord(((AdditionalContactNode)node).ID);
                DisplayPanel(dpRelatedContact);
            }
            else
                DisplayPanel(null);
        }

        /// <summary>
        ///     Restores the last selected node.
        /// </summary>
        protected override void SaveSelectedNode()
        {
            if(tvwTOC.SelectedNodes.Count > 0)
                Settings.Default.LastSelectedCustomer = tvwTOC.SelectedNodes[0].Key;
        }

        /// <summary>
        ///     Refreshes the selected node's parent's child nodes.
        /// </summary>
        internal void RefreshSelectedNodeParent()
        {
            var selectedNode = tvwTOC.SelectedNodes[0] as ShippingNode;
            if(selectedNode != null)
            {
                var parent = selectedNode.Parent as CustomerNode;
                if(parent != null)
                {
                    foreach(var child in parent.Nodes)
                    {
                        if(child is ShippingNode)
                            ((ShippingNode) child).UpdateNodeUI();
                    }
                }
            }
        }

        private void AddRelatedContactNodes(CustomersDataset.ContactRow contactRow)
        {
            if (!ApplicationSettings.Current.AllowAdditionalCustomersForContacts)
            {
                return;
            }

            var additionalCustomerIds = contactRow.GetContactAdditionalCustomerRows()
                .Select(c => c.CustomerID)
                .ToList();

            foreach (var customerNode in tvwTOC.NodesOfType<CustomerNode>())
            {
                var nodeCustomerId = customerNode.DataRow.CustomerID;

                if (!customerNode.IsDataLoaded || !additionalCustomerIds.Contains(nodeCustomerId))
                {
                    continue;
                }

                if (!customerNode.Nodes.OfType<AdditionalContactNode>().Any(node => node.DataRow.ContactID == contactRow.ContactID))
                {
                    customerNode.Nodes.Add(new AdditionalContactNode(customerNode.DataRow.CustomerID,
                        contactRow,
                        true));
                }
            }
        }

        private void FilterTree(string customerName)
        {
            try
            {
                if (tvwTOC.Nodes.Count != 1 || !IsValidControls())
                {
                    return;
                }

                customerName = customerName?.ToUpper();

                using (new UsingWaitCursor(this))
                {
                    using (new UsingTimeMe("loading filtered parts."))
                    {
                        using (new UsingTreeLoad(tvwTOC))
                        {
                            var rootNode = tvwTOC.Nodes[0];
                            foreach (var node in rootNode.Nodes.OfType<CustomerNode>()) 
                            {
                                if (string.IsNullOrWhiteSpace(customerName))
                                {
                                    node.Visible = node.DataRow.Active || _isInactiveVisible;
                                }
                                else
                                {
                                    // Can show inactive customers
                                    node.Visible = node.DataRow.Name.ToUpper().Contains(customerName);
                                }
                            }

                            rootNode.Select();
                        }
                    }
                }

                if (string.IsNullOrEmpty(customerName))
                {
                    txtNodeFilter.Text = null;
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error updating in line filter.";
                _log.Error(exc, errorMsg);
            }
        }

        #endregion

        #region Events

        /// <summary>
        ///     Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="EventArgs" /> instance containing the event data. </param>
        /// the event data
        /// protected virtual void btnOK_Click(object send
        /// Args e)
        /// {
        /// {
        /// using(new UsingWaitCursor(this))
        /// {
        /// bool
        /// = IsValidControls();
        /// if(success)
        /// {
        /// this._log.Info("On btnOK_Click: " + N
        protected override void btnOK_Click(object sender, EventArgs e)
        {
            CustomersDataset.ContactRow currentRow = null;

            try
            {
                //find selected contact
                if(tvwTOC.SelectedNodes.Count > 0)
                {
                    if (tvwTOC.SelectedNodes[0] is ContactNode)
                    {
                        currentRow = ((ContactNode)tvwTOC.SelectedNodes[0]).DataRow;
                    }
                    else if (tvwTOC.SelectedNodes[0] is AdditionalContactNode)
                    {
                        currentRow = ((AdditionalContactNode)tvwTOC.SelectedNodes[0]).DataRow;
                    }
                    else if (tvwTOC.SelectedNodes[0] is CustomerNode && tvwTOC.SelectedNodes[0].Nodes.Count > 0)
                    {
                        foreach (var item in tvwTOC.SelectedNodes[0].Nodes.OfType<ContactNode>())
                        {
                            currentRow = (item).DataRow;
                            break;
                        }
                    }
                }

                if(IsValidControls())
                {
                    if(SaveData())
                    {
                        //if there is a selected contact
                        if(currentRow != null)
                            this._lastSelectedContactID = currentRow.ContactID;

                        SaveSelectedNode();
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error closing form.", exc);
                _log.Fatal(exc, "Error closing form.");
            }
        }

        /// <summary>
        ///     Handles the Load event of the Customers control.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="EventArgs" /> instance containing the event data. </param>
        private void Customers_Load(object sender, EventArgs e)
        {
            _loadingData = true; //prevent TOC from loading

            try
            {
                LoadCommands();
                LoadData();
                LoadTOC();
                base.LoadValidators();

                tvwTOC.Override.SelectionType = SelectType.Extended;
                _loadingData = false;

                //select first node customer node
                base.RestoreLastSelectedNode(Settings.Default.LastSelectedCustomer);

                splitContainer1.Panel2.Enabled = SecurityManager.Current.IsInRole("Customers.Edit");
            }
            catch(Exception exc)
            {
                splitContainer1.Enabled = false;
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
                _log.Fatal(exc, "Error loading form.");
            }
        }

        /// <summary>
        ///     Adds the node.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The <see cref="EventArgs" /> instance containing the event data. </param>
        private void AddNode(object sender, EventArgs e)
        {
            if(IsValidControls())
            {
                var selectedNode = tvwTOC.SelectedNode <UltraTreeNode>();
                _validators.Enabled = false;

                if (sender is AddCustomerCommand && selectedNode is CustomersRootNode)
                {
                    AddCustomer((CustomersRootNode)selectedNode);
                }
                else if (sender is AddContactCommand && selectedNode is CustomerNode)
                {
                    AddContact((CustomerNode)selectedNode);
                }
                else if (sender is AddShippingCommand && selectedNode is CustomerNode)
                {
                    AddShipping((CustomerNode)selectedNode);
                }
                else if (sender is AddPartMarkingCommand && selectedNode is CustomerNode)
                {
                    AddPartMarking((CustomerNode)selectedNode);
                }
                else if (sender is AddCustomFieldCommand)
                {
                    if (tvwTOC.SelectedNodes.Count == 1 && selectedNode is CustomerNode)
                    {
                        AddCustomField((CustomerNode)selectedNode);
                    }
                    else if (tvwTOC.SelectedNodes.Count == 1 && selectedNode is CustomersRootNode)
                    {
                        var rootNode = (CustomersRootNode)selectedNode;
                        BulkCustomField(rootNode.Nodes.OfType<CustomerNode>());
                    }
                    else if (tvwTOC.SelectedNodes.Count > 1 && tvwTOC.SelectedNodes.All.All(node => node is CustomerNode))
                    {
                        BulkCustomField((tvwTOC.SelectedNodes.OfType<CustomerNode>()));
                    }
                }
                else if (sender is AddPartLevelCustomFieldCommand)
                {
                    if (tvwTOC.SelectedNodes.Count == 1 && selectedNode is CustomerNode)
                    {
                        AddPartLevelCustomField((CustomerNode)selectedNode);
                    }
                    else if (tvwTOC.SelectedNodes.Count == 1 && selectedNode is CustomersRootNode)
                    {
                        var rootNode = (CustomersRootNode)selectedNode;
                        BulkPartCustomField(rootNode.Nodes.OfType<CustomerNode>());
                    }
                    else if (tvwTOC.SelectedNodes.Count > 1 && tvwTOC.SelectedNodes.All.All(node => node is CustomerNode))
                    {
                        BulkPartCustomField((tvwTOC.SelectedNodes.OfType<CustomerNode>()));
                    }
                }

                _validators.Enabled = true;
            }
        }

        private void DisplayInactiveCommandOnShowInactive(object sender, EventArgs eventArgs)
        {
            _isInactiveVisible = true;
        }

        private void DisplayInactiveCommandOnHideInactive(object sender, EventArgs eventArgs)
        {
            _isInactiveVisible = false;
        }

        private void ContactOnContactRowDeleting(object sender, CustomersDataset.ContactRowChangeEvent contactRowChangeEvent)
        {
            try
            {
                if (contactRowChangeEvent.Action != DataRowAction.Delete)
                {
                    return;
                }

                foreach (var node in tvwTOC.NodesOfType<AdditionalContactNode>())
                {
                    if (node.DataRow.ContactID == contactRowChangeEvent.Row.ContactID)
                    {
                        node.Remove();
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling contact row deletion");
            }
        }

        private void ContactAdditionalCustomer_RowDeleting(object sender, CustomersDataset.ContactAdditionalCustomerRowChangeEvent e)
        {
            try
            {
                if (e.Action != DataRowAction.Delete)
                {
                    return;
                }

                foreach (var node in tvwTOC.NodesOfType<AdditionalContactNode>())
                {
                    if (node.DataRow.ContactID == e.Row.ContactID && node.CustomerId == e.Row.CustomerID)
                    {
                        node.Remove();
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling additional customer deletion.");
            }
        }

        private void ContactOnContactRowChanged(object sender, CustomersDataset.ContactRowChangeEvent contactRowChangeEvent)
        {
            try
            {
                if (contactRowChangeEvent.Action == DataRowAction.Add && contactRowChangeEvent.Row.RowState == DataRowState.Added)
                {
                    var contactRow = contactRowChangeEvent.Row;
                    AddRelatedContactNodes(contactRow);
                }
                else if (contactRowChangeEvent.Action == DataRowAction.Change)
                {
                    // Refresh existing related contact nodes
                    foreach (var node in tvwTOC.NodesOfType<AdditionalContactNode>())
                    {
                        if (node.DataRow.ContactID == contactRowChangeEvent.Row.ContactID)
                        {
                            node.UpdateNodeUI();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling contact row change.");
            }
        }

        private void ContactAdditionalCustomer_ContactAdditionalCustomerRowChanged(object sender, CustomersDataset.ContactAdditionalCustomerRowChangeEvent e)
        {
            try
            {
                if (e.Row.ContactRow == null)
                {
                    return;
                }

                if (e.Action == DataRowAction.Add && e.Row.RowState == DataRowState.Added)
                {
                    var contactRow = e.Row;
                    AddRelatedContactNodes(e.Row.ContactRow);
                }
                else if (e.Action == DataRowAction.Change)
                {
                    // Refresh existing related contact nodes
                    foreach (var node in tvwTOC.NodesOfType<AdditionalContactNode>())
                    {
                        if (node.DataRow.ContactID == e.Row.ContactID)
                        {
                            node.UpdateNodeUI();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling additional customer row change.");
            }
        }

        private void dpRelatedContact_GoToContactClicked(object sender, Sales.Customer.RelatedContactInformation.GoToContactEventArgs e)
        {
            try
            {
                var contactNode = tvwTOC.FindNode(node => (node is ContactNode cNode) && cNode.DataRow.ContactID == e.ContactId);

                if (contactNode == null)
                {
                    // Load customer for contact
                    // Assumption - All contacts are loaded
                    var customerId = dsCustomers.Contact.FindByContactID(e.ContactId)?.CustomerID ?? -1;
                    var customerNodeForContact = tvwTOC.Nodes.FindNodeBFS(node => (node is CustomerNode cNode) && cNode.DataRow.CustomerID == customerId) as CustomerNode;

                    customerNodeForContact?.LoadChildrenNodes(this);
                    contactNode = tvwTOC.FindNode(node => (node is ContactNode cNode) && cNode.DataRow.ContactID == e.ContactId);
                }

                if (contactNode != null)
                {
                    contactNode.Select();
                }
                else
                {
                    MessageBoxUtilities.ShowMessageBoxError("Unable to find contact.", "Customers");
                    _log.Warn($"Could not find contact node for {e.ContactId}");
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Error navigating to contact {e.ContactId}.");
            }
        }

        private void dpRelatedContact_GoToCustomerClicked(object sender, Sales.Customer.RelatedContactInformation.GoToCustomerEventArgs e)
        {
            try
            {
                var customer = tvwTOC.Nodes.FindNodeBFS(node => (node is CustomerNode cNode) && cNode.DataRow.CustomerID == e.CustomerId);

                if (customer != null)
                {
                    customer.Select();
                }
                else
                {
                    MessageBoxUtilities.ShowMessageBoxError("Unable to find customer.", "Customers");
                    _log.Warn($"Could not find customer node for {e.CustomerId}");
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Error navigating to customer {e.CustomerId}.");
            }
        }

        private void txtNodeFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (_loadingData || e.KeyChar != '\r')
                {
                    return;
                }

                e.Handled = true;
                FilterTree(txtNodeFilter.Text);
            }
            catch (Exception exc)
            {
                var errorMsg = "Error updating filter.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void txtNodeFilter_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                if (_loadingData)
                {
                    return;
                }

                if (e.Button.Key == "GO")
                {
                    FilterTree(txtNodeFilter.Text);
                }

                else if (e.Button.Key == "Reset")
                {
                    FilterTree(null);
                }
            }
            catch (Exception exc)
            {
                var errorMsg = "Error updating filter.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion

        #region Nodes

        #region Nested type: ContactNode

        /// <summary>
        /// </summary>
        private class ContactNode : DataNode <CustomersDataset.ContactRow>
        {
            private const string KEY_PREFIX = "CO";

            /// <summary>
            ///     Gets the key used to sort the node by.
            /// </summary>
            /// <value> The sort key. </value>
            public override string SortKey
            {
                get { return "1_" + Text; }
            }

            /// <summary>
            ///     Gets or sets the usage count.
            /// </summary>
            /// <value> The usage count. </value>
            public int? UsageCount { get; set; }

            #region Methods

            /// <summary>
            ///     Creates a new instance of the <see cref="T:Infragistics.Win.UltraWinTree.UltraTreeNode" /> object.
            /// </summary>
            /// <param name="cr"> The cr. </param>
            /// <param name="forceVisible"></param>
            /// <remarks>
            ///     <p class="body">
            ///         In the case where no value is specified for the node's
            ///         <see cref="P:Infragistics.Win.UltraWinTree.UltraTreeNode.Text" />
            ///         property, the
            ///         <see cref="P:Infragistics.Win.UltraWinTree.UltraTreeNode.Key" />
            ///         property defines the display text for the node.
            ///     </p>
            /// </remarks>
            public ContactNode(CustomersDataset.ContactRow cr, bool forceVisible) : base(cr, cr.ContactID.ToString(), KEY_PREFIX, null)
            {
                LeftImages.Add(Properties.Resources.Contact_16);
                UpdateNodeUI();
                RefreshVisibility(forceVisible);
            }

            /// <summary>
            ///     Gets a value indicating whether this node can be deleted.
            /// </summary>
            /// <value> <c>true</c> if this instance can delete; otherwise, <c>false</c> . </value>
            public override bool CanDelete
            {
                get { return UsageCount < 1; }
            }

            /// <summary>
            ///     Called when the data source this node represents changes.
            /// </summary>
            public override void UpdateNodeUI() 
            {
                Text = DataRow.IsNameNull() ? string.Empty : base.DataRow.Name;

                if (!base.DataRow.Active)
                {
                    base.Override.NodeAppearance.FontData.Italic = DefaultableBoolean.True;
                    base.Override.NodeAppearance.ForeColor = Color.Gray;
                }
            }
        

            #endregion

            public void RefreshVisibility(bool forceVisible)
            {
                Visible = forceVisible || DataRow.Active;
            }
        }

        #endregion

        #region Nested type: RelatedContactNode

        private class AdditionalContactNode : DataNode<CustomersDataset.ContactRow>
        {
            #region Fields

            private const string KEY_PREFIX = "RelatedContact";

            #endregion

            #region Properties

            /// <summary>
            ///     Gets the key used to sort the node by.
            /// </summary>
            /// <value> The sort key. </value>
            public override string SortKey
            {
                get { return "1_" + Text; }
            }

            /// <summary>
            /// Gets the additional/secondary customer's ID for this instance.
            /// </summary>
            public int CustomerId { get; }

            #endregion

            #region Methods

            public AdditionalContactNode(int customerId, CustomersDataset.ContactRow cr, bool forceShow) : base(cr, $"{customerId}_{cr.ContactID}", KEY_PREFIX, null)
            {
                CustomerId = customerId;
                LeftImages.Add(Properties.Resources.Contact_16);
                LeftImages.Add(Properties.Resources.Link_16);
                UpdateNodeUI();
                RefreshVisibility(forceShow);
            }

            /// <summary>
            ///     Gets a value indicating whether this node can be deleted.
            /// </summary>
            /// <value> <c>true</c> if this instance can delete; otherwise, <c>false</c> . </value>
            public override bool CanDelete => false;

            /// <summary>
            ///     Called when the data source this node represents changes.
            /// </summary>
            public override void UpdateNodeUI()
            {
                Text = DataRow.IsNameNull() ? string.Empty : DataRow.Name;
            }

            public void RefreshVisibility(bool forceShow)
            {
                Visible = forceShow || (DataRow.Active && DataRow.CustomerRow.Active);
            }

            public override void Delete()
            {
                // Do nothing - contact belongs to a different customer
            }

            #endregion
        }

        #endregion

        #region Nested type: CustomFieldNode

        /// <summary>
        /// </summary>
        private class CustomFieldNode : DataNode <CustomersDataset.CustomFieldRow>
        {
            private const string KEY_PREFIX = "CF";

            /// <summary>
            ///     Gets the key used to sort the node by.
            /// </summary>
            /// <value> The sort key. </value>
            public override string SortKey =>
                $"4_{(DataRow.Required ? "1" : "2")}_{DataRow.Name}";

            #region Methods

            /// <summary>
            ///     Creates a new instance of the <see cref="T:Infragistics.Win.UltraWinTree.UltraTreeNode" /> object.
            /// </summary>
            /// <param name="cr"> The cr. </param>
            /// <remarks>
            ///     <p class="body">
            ///         In the case where no value is specified for the node's
            ///         <see cref="P:Infragistics.Win.UltraWinTree.UltraTreeNode.Text" />
            ///         property, the
            ///         <see cref="P:Infragistics.Win.UltraWinTree.UltraTreeNode.Key" />
            ///         property defines the display text for the node.
            ///     </p>
            /// </remarks>
            public CustomFieldNode(CustomersDataset.CustomFieldRow cr) : base(cr, cr.CustomFieldID.ToString(), KEY_PREFIX, cr.Name)
            {
                LeftImages.Add(Properties.Resources.CustomField_16);
                UpdateNodeUI();
            }

            /// <summary>
            ///     Called when the data source this node represents changes.
            /// </summary>
            public override void UpdateNodeUI()
            {
                Text = DataRow.Required
                    ? $"{DataRow.Name} (Required)"
                    : DataRow.Name;
            }

            #endregion
        }

        #endregion

        #region Nested type: PartCustomFieldNode

        /// <summary>
        /// </summary>
        private class PartCustomFieldNode : DataNode <CustomersDataset.PartLevelCustomFieldRow>
        {
            private const string KEY_PREFIX = "PCF";

            /// <summary>
            ///     Gets the key used to sort the node by.
            /// </summary>
            /// <value> The sort key. </value>
            public override string SortKey =>
                $"5_{DataRow.Name}";

            /// <summary>
            ///     Gets or sets the usage count.
            /// </summary>
            /// <value> The usage count. </value>
            public int? UsageCount { get; set; }

            public override bool CanDelete => UsageCount == 0;

            #region Methods

            /// <summary>
            /// Creates a new instance of the <see cref="PartCustomFieldNode"/> class.
            /// </summary>
            /// <param name="cr"> The custom field row. </param>
            public PartCustomFieldNode(CustomersDataset.PartLevelCustomFieldRow cr)
                : base(cr, cr.PartLevelCustomFieldID.ToString(), KEY_PREFIX, cr.Name)
            {
                LeftImages.Add(Properties.Resources.CustomField_16);
                LeftImages.Add(Properties.Resources.Part_16);
                UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                Text = DataRow.Name;
            }

            #endregion
        }

        #endregion

        #region Nested type: CustomerNode

        /// <summary>
        /// </summary>
        private class CustomerNode : DataNode <CustomersDataset.CustomerRow>
        {
            private const string KEY_PREFIX = "CU";

            /// <summary>
            ///     Gets or sets the usage count.
            /// </summary>
            /// <value> The usage count. </value>
            public int? UsageCount { get; set; }

            /// <summary>
            ///     Gets  value indicating whether this instance has loaded data.
            /// </summary>
            /// <value> <c>true</c> if data for this instance is loaded; otherwise, <c>false</c> . </value>
            public bool IsDataLoaded { get; private set; }

            #region Methods

            /// <summary>
            ///     Creates a new instance of the <see cref="T:Infragistics.Win.UltraWinTree.UltraTreeNode" /> object.
            /// </summary>
            /// <param name="cr"> The cr. </param>
            /// <remarks>
            ///     <p class="body">
            ///         In the case where no value is specified for the node's
            ///         <see cref="P:Infragistics.Win.UltraWinTree.UltraTreeNode.Text" />
            ///         property, the
            ///         <see cref="P:Infragistics.Win.UltraWinTree.UltraTreeNode.Key" />
            ///         property defines the display text for the node.
            ///     </p>
            /// </remarks>
            public CustomerNode(CustomersDataset.CustomerRow cr) : base(cr, cr.CustomerID.ToString(), KEY_PREFIX, cr.Name)
            {
                //update UI
                LeftImages.Add(Properties.Resources.Customer);
                UpdateNodeUI();
            }

            /// <summary>
            ///     Gets a value indicating whether this node can be deleted.
            /// </summary>
            /// <value> <c>true</c> if this instance can delete; otherwise, <c>false</c> . </value>
            public override bool CanDelete
            {
                get { return UsageCount < 1; }
            }

            /// <summary>
            ///     Gets the clipboard data format.
            /// </summary>
            /// <value> The clipboard data format. </value>
            public override string ClipboardDataFormat
            {
                get { return null; }
            }

            /// <summary>
            ///     Called when the data source this node represents changes.
            /// </summary>
            public override void UpdateNodeUI()
            {
                Text = base.DataRow.Name;

                if(base.DataRow.Active)
                {
                    base.Override.NodeAppearance.FontData.Italic = DefaultableBoolean.False;

                    if(!base.DataRow.IsCustomerStatusNull() && base.DataRow.CustomerStatusRow != null && !base.DataRow.CustomerStatusRow.IsColorNull())
                        base.Override.NodeAppearance.ForeColor = Color.FromName(base.DataRow.CustomerStatusRow.Color);
                    else
                        base.Override.NodeAppearance.ResetForeColor();
                }
                else
                {
                    base.Override.NodeAppearance.FontData.Italic = DefaultableBoolean.True;
                    base.Override.NodeAppearance.ForeColor = Color.Gray;
                }
            }

            /// <summary>
            ///     Loads the children nodes.
            /// </summary>
            /// <param name="customers"> The customers. </param>
            internal void LoadChildrenNodes(Customers customers)
            {
                try
                {
                    //if this is a new order then don't bother loading child rows, there aren't any
                    if(DataRow.RowState == DataRowState.Added)
                        return;

                    if(!IsDataLoaded)
                    {
                        using(new UsingTimeMe("Loading Nodes for customer " + base.DataRow.Name))
                        {
                            using(new UsingTreeLoad(base.Control))
                            {
                                var customerID = DataRow.CustomerID;
                                var showAll = ((StateButtonTool) customers.toolbarManager.Tools["DisplayInActive"]).Checked;

                                //LOAD Contacts
                                var deletedContactIds = customers.dsCustomers
                                    .Contact
                                    .FindDeletedRows()
                                    .Select(r => r[customers.dsCustomers.Contact.ContactIDColumn, DataRowVersion.Original] as int? ?? -1)
                                    .ToList();

                                using (var dtContacts = new CustomersDataset.ContactDataTable())
                                {
                                    customers.taContact.FillBy(dtContacts, customerID);
                                    customers.taContact.FillBySecondaryCustomer(dtContacts, customerID);
                                    foreach (var contact in dtContacts)
                                    {
                                        var loadedMatch = customers.dsCustomers
                                            .Contact
                                            .FindByContactID(contact.ContactID);

                                        var isDeleted = deletedContactIds.Contains(contact.ContactID);

                                        if (loadedMatch == null && !isDeleted)
                                        {
                                            customers.dsCustomers.Contact.ImportRow(contact);
                                        }
                                    }
                                }

                                foreach(var ccr in DataRow.GetContactRows())
                                    Nodes.Add(new ContactNode(ccr, showAll));

                                // Load additional customers for contacts
                                var deletedAdditionalCustomers = customers.dsCustomers
                                    .ContactAdditionalCustomer
                                    .FindDeletedRows()
                                    .Select(r => r[customers.dsCustomers.ContactAdditionalCustomer.ContactAdditionalCustomerIDColumn, DataRowVersion.Original] as int? ?? -1)
                                    .ToList();

                                using (var dtContactAdditionalCustomer = new CustomersDataset.ContactAdditionalCustomerDataTable())
                                {
                                    customers.taContactAdditionalCustomer.FillByPrimaryCustomer(dtContactAdditionalCustomer, customerID);
                                    customers.taContactAdditionalCustomer.FillBySecondaryCustomer(dtContactAdditionalCustomer, customerID);

                                    foreach (var additionalCustomer in dtContactAdditionalCustomer)
                                    {
                                        var loadedMatch = customers.dsCustomers
                                            .ContactAdditionalCustomer
                                            .FindByContactAdditionalCustomerID(additionalCustomer.ContactAdditionalCustomerID);

                                        var isDeleted = deletedAdditionalCustomers.Contains(additionalCustomer.ContactAdditionalCustomerID);

                                        if (!isDeleted)
                                        {
                                            if (loadedMatch == null)
                                            {
                                                customers.dsCustomers.ContactAdditionalCustomer.ImportRow(additionalCustomer);
                                            }
                                        }
                                    }
                                }

                                foreach (var additionalCustomerRow in DataRow.GetContactAdditionalCustomerRows())
                                {
                                    Nodes.Add(new AdditionalContactNode(customerID, additionalCustomerRow.ContactRow, showAll));
                                }

                                // LOAD report tasks for contacts
                                customers.taReportTask.FillBy(customers.dsCustomers.ReportTask, customerID);

                                //LOAD CustomerShipping
                                customers.taCustomerShipping.FillBy(customers.dsCustomers.CustomerShipping, customerID);

                                foreach(var csRow in DataRow.GetCustomerShippingRows())
                                    Nodes.Add(new ShippingNode(csRow));

                                //LOAD PartMarking 
                                customers.taCustomer_PartMarking.FillBy(customers.dsCustomers.Customer_PartMarking, customerID);

                                foreach(var pmRow in DataRow.GetCustomer_PartMarkingRows())
                                    Nodes.Add(new PartMarkingNode(pmRow.PartMarkingRow));

                                // CustomFields should already be loaded
                                LoadCustomFields();

                                // LOAD CustomerAddress
                                customers.taCustomerAddress.FillByCustomer(customers.dsCustomers.CustomerAddress, customerID);

                                // LOAD CustomerPricePoint
                                customers.taCustomerPricePoint.FillByCustomer(customers.dsCustomers.CustomerPricePoint, customerID);
                                customers.taCustomerPricePointDetail.FillByCustomer(customers.dsCustomers.CustomerPricePointDetail, customerID);

                                // Load CustomerFee
                                customers.taCustomerFee.FillByCustomer(customers.dsCustomers.CustomerFee, customerID);

                                IsDataLoaded = true;
                            }
                        }
                    }
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error loading order node children.";
                    LogManager.GetCurrentClassLogger().Error(exc, errorMsg);

                    IsDataLoaded = true;
                }
            }

            public void LoadCustomFields()
            {
                foreach (var cf in DataRow.GetCustomFieldRows())
                {
                    if (Nodes.OfType<CustomFieldNode>().All(n => n.DataRow.CustomFieldID != cf.CustomFieldID))
                    {
                        Nodes.Add(new CustomFieldNode(cf));
                    }
                }

                foreach (var pcf in DataRow.GetPartLevelCustomFieldRows())
                {
                    if (Nodes.OfType<PartCustomFieldNode>().All(n => n.DataRow.PartLevelCustomFieldID != pcf.PartLevelCustomFieldID))
                    {
                        Nodes.Add(new PartCustomFieldNode(pcf));
                    }
                }
            }

            /// <summary>
            ///     Pastes the data.
            /// </summary>
            /// <param name="format"> The format. </param>
            /// <param name="proxy"> The proxy. </param>
            /// <returns> </returns>
            public override UltraTreeNode PasteData(string format, DataRowProxy proxy)
            {
                if (proxy == null)
                {
                    return null;
                }

                if (DataRow == null)
                {
                    LogManager.GetCurrentClassLogger().Warn("Node was disposed - skipping paste");
                    return null;
                }

                var ds = DataRow.Table.DataSet as CustomersDataset;

                //remove the relations between customer/part marking from the copied part marking template
                proxy.ChildProxies?.Clear();

                //add new data row
                var dr = DataNode <DataRow>.AddPastedDataRows(proxy, ds.PartMarking) as CustomersDataset.PartMarkingRow;

                //relate new part mark template with this customer
                ds.Customer_PartMarking.AddCustomer_PartMarkingRow(DataRow, dr);

                var copiedPart = new PartMarkingNode(dr);
                base.Nodes.Add(copiedPart);

                copiedPart.Select();
                return copiedPart;
            }

            /// <summary>
            ///     Determines whether this instance [can paste data] the specified format.
            /// </summary>
            /// <param name="format"> The format. </param>
            /// <returns> <c>true</c> if this instance [can paste data] the specified format; otherwise, <c>false</c> . </returns>
            public override bool CanPasteData(string format) { return format == typeof(PartMarkingNode).FullName; }

            #endregion
        }

        #endregion

        #region Nested type: CustomersRootNode

        /// <summary>
        /// </summary>
        private class CustomersRootNode : UltraTreeNode, ICopyPasteNode
        {
            #region Properties

            /// <summary>
            /// </summary>
            private CustomersDataset _dataset;

            #endregion

            #region Methods

            /// <summary>
            ///     Creates a new instance of the <see cref="T:Infragistics.Win.UltraWinTree.UltraTreeNode" /> object.
            /// </summary>
            /// <param name="dataset"> The dataset. </param>
            /// <remarks>
            ///     <p class="body">
            ///         In the case where no value is specified for the node's
            ///         <see cref="P:Infragistics.Win.UltraWinTree.UltraTreeNode.Text" />
            ///         property, the
            ///         <see cref="P:Infragistics.Win.UltraWinTree.UltraTreeNode.Key" />
            ///         property defines the display text for the node.
            ///     </p>
            /// </remarks>
            public CustomersRootNode(CustomersDataset dataset) : base("ROOT", "Customers")
            {
                this._dataset = dataset;
                LeftImages.Add(Properties.Resources.Customer);
            }

            /// <summary>
            ///     Called to Dispose the object
            /// </summary>
            /// <remarks>
            ///     <b>Note:</b> Derived classes should call the base implementation so that the Disposed property gets set.
            /// </remarks>
            public override void Dispose()
            {
                this._dataset = null;
                base.Dispose();
            }

            #endregion

            #region ICopyPasteNode Members

            /// <summary>
            ///     Pastes the data based on the format type as a new child node.
            /// </summary>
            /// <param name="format"> The format. </param>
            /// <param name="proxy"> The proxy. </param>
            /// <returns> </returns>
            public UltraTreeNode PasteData(string format, DataRowProxy proxy)
            {
                if (proxy == null)
                {
                    return null;
                }

                if (_dataset == null)
                {
                    LogManager.GetCurrentClassLogger().Warn("Node was disposed - skipping paste");
                    return null;
                }

                DataRow dr = DataNode <DataRow>.AddPastedDataRows(proxy, this._dataset.Customer);
                var node = new CustomerNode(dr as CustomersDataset.CustomerRow);
                base.Nodes.Add(node);

                return node;
            }

            /// <summary>
            ///     Determines whether this instance can paste data of the specified format.
            /// </summary>
            /// <param name="format"> The format. </param>
            /// <returns> <c>true</c> if this instance [can paste data] the specified format; otherwise, <c>false</c> . </returns>
            public bool CanPasteData(string format) { return format == typeof(CustomerNode).FullName; }

            /// <summary>
            ///     Gets the clipboard data format that this instance represents.
            /// </summary>
            /// <value> The clipboard data format. </value>
            public string ClipboardDataFormat
            {
                get { return null; }
            }

            #endregion
        }

        #endregion

        #region Nested type: PartMarkingNode

        /// <summary>
        /// </summary>
        private class PartMarkingNode : DataNode <CustomersDataset.PartMarkingRow>
        {
            private const string KEY_PREFIX = "PM";

            /// <summary>
            ///     Gets the key used to sort the node by.
            /// </summary>
            /// <value> The sort key. </value>
            public override string SortKey
            {
                get { return "3_" + Text; }
            }

            #region Methods

            /// <summary>
            ///     Creates a new instance of the <see cref="T:Infragistics.Win.UltraWinTree.UltraTreeNode" /> object.
            /// </summary>
            /// <param name="cr"> The cr. </param>
            /// <remarks>
            ///     <p class="body">
            ///         In the case where no value is specified for the node's
            ///         <see cref="P:Infragistics.Win.UltraWinTree.UltraTreeNode.Text" />
            ///         property, the
            ///         <see cref="P:Infragistics.Win.UltraWinTree.UltraTreeNode.Key" />
            ///         property defines the display text for the node.
            ///     </p>
            /// </remarks>
            public PartMarkingNode(CustomersDataset.PartMarkingRow cr) : base(cr, cr.PartMarkingID.ToString(), KEY_PREFIX, cr.AirframeID_d)
            {
                LeftImages.Add(Properties.Resources.Tag_16);
                UpdateNodeUI();
            }

            /// <summary>
            ///     Gets a value indicating whether this node can be deleted.
            /// </summary>
            /// <value> <c>true</c> if this instance can delete; otherwise, <c>false</c> . </value>
            public override bool CanDelete
            {
                get { return SecurityManager.Current.IsInRole("Customers.Edit") || SecurityManager.Current.IsInRole("Customers.PartMark"); }
            }

            /// <summary>
            ///     Gets the clipboard data format.
            /// </summary>
            /// <value> The clipboard data format. </value>
            public override string ClipboardDataFormat
            {
                get { return GetType().FullName; }
            }

            /// <summary>
            ///     Called when the data source this node represents changes.
            /// </summary>
            public override void UpdateNodeUI() { Text = base.DataRow.AirframeID_d; }

            #endregion
        }

        #endregion

        #region Nested type: ShippingNode

        /// <summary>
        /// </summary>
        private class ShippingNode : DataNode <CustomersDataset.CustomerShippingRow>
        {
            private const string KEY_PREFIX = "SH";

            /// <summary>
            ///     Gets the key used to sort the node by.
            /// </summary>
            /// <value> The sort key. </value>
            public override string SortKey
            {
                get { return "2_" + Text; }
            }

            /// <summary>
            ///     Gets or sets the usage count.
            /// </summary>
            /// <value> The usage count. </value>
            public int? UsageCount { get; set; }

            #region Methods

            /// <summary>
            ///     Creates a new instance of the <see cref="T:Infragistics.Win.UltraWinTree.UltraTreeNode" /> object.
            /// </summary>
            /// <param name="cr"> The cr. </param>
            /// <remarks>
            ///     <p class="body">
            ///         In the case where no value is specified for the node's
            ///         <see cref="P:Infragistics.Win.UltraWinTree.UltraTreeNode.Text" />
            ///         property, the
            ///         <see cref="P:Infragistics.Win.UltraWinTree.UltraTreeNode.Key" />
            ///         property defines the display text for the node.
            ///     </p>
            /// </remarks>
            public ShippingNode(CustomersDataset.CustomerShippingRow cr) : base(cr, cr.CustomerShippingID.ToString(), KEY_PREFIX, cr.CarrierID)
            {
                LeftImages.Add(Properties.Resources.Shipping_16);
                UpdateNodeUI();
            }

            /// <summary>
            ///     Gets a value indicating whether this node can be deleted.
            /// </summary>
            /// <value> <c>true</c> if this instance can delete; otherwise, <c>false</c> . </value>
            public override bool CanDelete
            {
                get
                {
                    bool hasPermission = SecurityManager.Current.IsInRole("Customers.Edit") ||
                                         SecurityManager.Current.IsInRole("Customers.Shipping");

                    return hasPermission &&
                           UsageCount.HasValue &&
                           UsageCount.Value == 0;
                }
            }

            /// <summary>
            ///     Called when the data source this node represents changes.
            /// </summary>
            public override void UpdateNodeUI()
            {
                if (string.IsNullOrEmpty(DataRow.CarrierCustomerNumber))
                {
                    Text = DataRow.CarrierID;
                }
                else
                {
                    Text = DataRow.CarrierID + " - " + DataRow.CarrierCustomerNumber;
                }

                if (DataRow.Active)
                {
                    Override.NodeAppearance.FontData.Italic = DefaultableBoolean.False;
                    Override.NodeAppearance.FontData.Bold = DataRow.DefaultShippingMethod
                        ? DefaultableBoolean.True
                        : DefaultableBoolean.False;

                    Override.NodeAppearance.ResetForeColor();
                }
                else
                {
                    Override.NodeAppearance.FontData.Italic = DefaultableBoolean.True;
                    Override.NodeAppearance.FontData.Bold = DefaultableBoolean.False;
                    Override.NodeAppearance.ForeColor = Color.Gray;
                }
            }

            #endregion
        }

        #endregion

        #endregion

        #region Commands

        #region Nested type: AddContactCommand

        /// <summary>
        /// </summary>
        private class AddContactCommand : TreeNodeCommandBase
        {
            #region Fields

            /// <summary>
            /// </summary>
            public EventHandler AddCallback;

            #endregion

            #region Properties

            /// <summary>
            ///     Gets a value indicating whether this <see cref="AddContactCommand" /> is enabled.
            /// </summary>
            /// <value> <c>true</c> if enabled; otherwise, <c>false</c> . </value>
            public override bool Enabled
            {
                get { return (_node is CustomerNode && this.AddCallback != null); }
            }

            #endregion

            #region Methods

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="tool"> The tool. </param>
            /// <param name="toc"> The toc. </param>
            public AddContactCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            /// <summary>
            ///     Called when [click].
            /// </summary>
            public override void OnClick()
            {
                if(_node is CustomerNode)
                {
                    if(this.AddCallback != null)
                        this.AddCallback(this, EventArgs.Empty);
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: AddCustomFieldCommand

        /// <summary>
        /// </summary>
        private class AddCustomFieldCommand : TreeNodeCommandBase
        {
            #region Fields

            /// <summary>
            /// </summary>
            public EventHandler AddCallback;

            #endregion

            #region Properties

            /// <summary>
            ///     Gets a value indicating whether this <see cref="AddCustomFieldCommand" /> is enabled.
            /// </summary>
            /// <value> <c>true</c> if enabled; otherwise, <c>false</c> . </value>
            public override bool Enabled
            {
                get
                {
                    var correctNodeSelected = _node is CustomersRootNode ||
                                              (_node is CustomerNode && TreeView.SelectedNodes.All.All(node => node is CustomerNode));

                    return correctNodeSelected && this.AddCallback != null;
                }
            }

            #endregion

            #region Methods

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="tool"> The tool. </param>
            /// <param name="toc"> The toc. </param>
            public AddCustomFieldCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
                base.AllowMultipleSelection = true;
            }

            /// <summary>
            ///     Called when [click].
            /// </summary>
            public override void OnClick()
            {
                if (!Enabled)
                {
                    return;
                }

                this.AddCallback?.Invoke(this, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #region Nested type: AddPartLevelCustomFieldCommand

        /// <summary>
        /// </summary>
        private class AddPartLevelCustomFieldCommand : TreeNodeCommandBase
        {
            #region Properties

            /// <summary>
            /// Gets or sets the command callback for this instance.
            /// </summary>
            public EventHandler AddCallback { get; set; }

            /// <summary>
            /// Gets a value indicating whether this instance is enabled.
            /// </summary>
            /// <value>
            /// <c>true</c> if enabled; otherwise, <c>false</c> .
            /// </value>
            public override bool Enabled
            {
                get
                {
                    var correctNodeSelected = _node is CustomersRootNode ||
                        (_node is CustomerNode && TreeView.SelectedNodes.All.All(node => node is CustomerNode));

                    return correctNodeSelected && AddCallback != null;
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="AddPartLevelCustomField"/> class.
            /// </summary>
            /// <param name="tool"> The tool.</param>
            /// <param name="toc"> The tree of customer data.</param>
            public AddPartLevelCustomFieldCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                TreeView = toc;
                AllowMultipleSelection = true;
            }

            /// <summary>
            ///     Called when [click].
            /// </summary>
            public override void OnClick()
            {
                if (!Enabled)
                {
                    return;
                }

                AddCallback?.Invoke(this, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #region Nested type: CopyCustomFieldsCommand

        internal class CopyCustomFieldsCommand : TreeNodeCommandBase
        {
            #region Properties

            public Customers CustomersForm { get; set; }

            public override bool Enabled
            {
                get
                {
                    if (CustomersForm.CustomerFilterIds?.Any() ?? false)
                    {
                        return false;
                    }

                    var correctNodeSelected = _node is CustomersRootNode ||
                                              (_node is CustomerNode &&
                                               TreeView.SelectedNodes.All.All(node => node is CustomerNode));

                    return correctNodeSelected;
                }
            }

            #endregion

            #region Methods

            public CopyCustomFieldsCommand(ToolBase tool, UltraTree toc, Customers customersForm)
                : base(tool)
            {
                if (customersForm == null)
                {
                    throw new ArgumentNullException(nameof(customersForm));
                }

                CustomersForm = customersForm;
                TreeView = toc;
            }

            public override void OnClick()
            {
                ComboBoxForm customSelectForm = null;

                try
                {
                    var sourceCustomer = (_node as CustomerNode)?.DataRow;

                    if (!Enabled || sourceCustomer == null)
                    {
                        return;
                    }

                    customSelectForm = new ComboBoxForm
                    {
                        Text = "Copy To Customer"
                    };

                    customSelectForm.FormLabel.Text = "Customer:";
                    customSelectForm.ComboBox.DropDownStyle = DropDownStyle.DropDownList;

                    var customerIdColumn = CustomersForm.dsCustomers.Customer.CustomerIDColumn.ColumnName;
                    var customerActiveColumn = CustomersForm.dsCustomers.Customer.ActiveColumn.ColumnName;
                    var customerNameColumn = CustomersForm.dsCustomers.Customer.NameColumn.ColumnName;

                    customSelectForm.ComboBox.ValueMember = CustomersForm.dsCustomers.Customer.CustomerIDColumn.ColumnName;
                    customSelectForm.ComboBox.DisplayMember = CustomersForm.dsCustomers.Customer.NameColumn.ColumnName;
                    customSelectForm.ComboBox.DataSource = new DataView(CustomersForm.dsCustomers.Customer)
                    {
                        RowFilter = CustomersForm._isInactiveVisible
                            ? $"{customerIdColumn} <> {sourceCustomer.CustomerID}"
                            : $"{customerIdColumn} <> {sourceCustomer.CustomerID} AND {customerActiveColumn} = 1",

                        Sort = customerNameColumn
                    };

                    customSelectForm.ComboBox.DataBind();

                    if (customSelectForm.ComboBox.Items.Count > 0)
                    {
                        customSelectForm.ComboBox.SelectedIndex = 0;
                    }

                    if (customSelectForm.ShowDialog(CustomersForm) == DialogResult.OK && customSelectForm.ComboBox.SelectedItem != null)
                    {
                        var targetCustomerId = customSelectForm.ComboBox.SelectedItem.DataValue as int? ?? -1;
                        var targetCustomer = CustomersForm.dsCustomers.Customer.FindByCustomerID(targetCustomerId);
                        DoCopy(sourceCustomer, targetCustomer);

                        var targetNode = TreeView.Nodes.FindNode<CustomerNode>(node => node.DataRow.CustomerID == targetCustomerId);
                        targetNode.LoadCustomFields();
                    }
                }
                catch (Exception exc)
                {
                    _log.Error(exc, "Error copying custom fields.");
                }
                finally
                {
                    customSelectForm?.Dispose();
                }
            }

            private void DoCopy(CustomersDataset.CustomerRow sourceCustomer, CustomersDataset.CustomerRow targetCustomer)
            {
                if (sourceCustomer == null || targetCustomer == null)
                {
                    _log.Warn("Cannot copy - invalid customer specified.");
                    return;
                }

                // Copy order-level custom fields.
                var sourceFields = sourceCustomer.GetCustomFieldRows();
                var existingDestFields = targetCustomer.GetCustomFieldRows();

                foreach (var sourceField in sourceFields)
                {
                    var hasMatch = existingDestFields.Any(f => f.Name == sourceField.Name);

                    if (hasMatch)
                    {
                        continue;
                    }

                    var destField = CustomersForm.dsCustomers.CustomField.NewCustomFieldRow();
                    destField.CustomerRow = targetCustomer;
                    destField.Name = sourceField.Name;
                    destField.DisplayOnCOC = sourceField.DisplayOnCOC;
                    destField.DisplayOnTraveler = sourceField.DisplayOnTraveler;
                    destField.IsVisible = sourceField.IsVisible;
                    destField.ProcessUnique = sourceField.ProcessUnique;
                    destField.Required = sourceField.Required;
                   // destField.ListID = sourceField.ListID;

                    if (!sourceField.IsListIDNull())
                    {
                        destField.ListID = sourceField.ListID;
                    }

                    if (!sourceField.IsTokenNameNull())
                    {
                        destField.TokenName = sourceField.TokenName;
                    }

                    if (!sourceField.IsDefaultValueNull())
                    {
                        destField.DefaultValue = sourceField.DefaultValue;
                    }

                    if (!sourceField.IsDescriptionNull())
                    {
                        destField.Description = sourceField.Description;
                    }

                    CustomersForm.dsCustomers.CustomField.AddCustomFieldRow(destField);
                }

                // Copy part-level custom fields.
                var partSourceFields = sourceCustomer.GetPartLevelCustomFieldRows();
                var existingPartDestinationFields = targetCustomer.GetPartLevelCustomFieldRows();

                foreach (var sourceField in partSourceFields)
                {
                    var hasMatch = existingPartDestinationFields
                        .Any(f => f.Name == sourceField.Name);

                    if (hasMatch)
                    {
                        continue;
                    }

                    var destField = CustomersForm.dsCustomers.PartLevelCustomField.NewPartLevelCustomFieldRow();
                    destField.CustomerRow = targetCustomer;
                    destField.Name = sourceField.Name;
                    destField.DisplayOnCOC = sourceField.DisplayOnCOC;
                    destField.DisplayOnTraveler = sourceField.DisplayOnTraveler;
                    destField.IsVisible = sourceField.IsVisible;

                    if (!sourceField.IsListIDNull())
                    {
                        destField.ListID = sourceField.ListID;
                    }

                    if (!sourceField.IsDefaultValueNull())
                    {
                        destField.DefaultValue = sourceField.DefaultValue;
                    }

                    if (!sourceField.IsDescriptionNull())
                    {
                        destField.Description = sourceField.Description;
                    }

                    CustomersForm.dsCustomers.PartLevelCustomField.AddPartLevelCustomFieldRow(destField);
                }

            }

            #endregion
        }

        #endregion

        #region Nested type: AddCustomerCommand

        /// <summary>
        /// </summary>
        private class AddCustomerCommand : TreeNodeCommandBase
        {
            #region Fields

            /// <summary>
            /// </summary>
            public EventHandler AddCallback;

            #endregion

            #region Properties

            /// <summary>
            ///     Gets a value indicating whether this <see cref="AddCustomerCommand" /> is enabled.
            /// </summary>
            /// <value> <c>true</c> if enabled; otherwise, <c>false</c> . </value>
            public override bool Enabled
            {
                get { return (_node is CustomersRootNode && this.AddCallback != null); }
            }

            #endregion

            #region Methods

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="tool"> The tool. </param>
            /// <param name="toc"> The toc. </param>
            public AddCustomerCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            /// <summary>
            ///     Called when [click].
            /// </summary>
            public override void OnClick()
            {
                if(_node is CustomersRootNode)
                {
                    if(this.AddCallback != null)
                        this.AddCallback(this, EventArgs.Empty);
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: AddPartMarkingCommand

        /// <summary>
        /// </summary>
        private class AddPartMarkingCommand : TreeNodeCommandBase
        {
            #region Fields

            /// <summary>
            /// </summary>
            public EventHandler AddCallback;

            #endregion

            #region Properties

            /// <summary>
            ///     Gets a value indicating whether this <see cref="AddPartMarkingCommand" /> is enabled.
            /// </summary>
            /// <value> <c>true</c> if enabled; otherwise, <c>false</c> . </value>
            public override bool Enabled
            {
                get { return (_node is CustomerNode && this.AddCallback != null); }
            }

            #endregion

            #region Methods

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="tool"> The tool. </param>
            /// <param name="toc"> The toc. </param>
            public AddPartMarkingCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            /// <summary>
            ///     Called when [click].
            /// </summary>
            public override void OnClick()
            {
                if(_node is CustomerNode)
                {
                    if(this.AddCallback != null)
                        this.AddCallback(this, EventArgs.Empty);
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: AddShippingCommand

        /// <summary>
        /// </summary>
        private class AddShippingCommand : TreeNodeCommandBase
        {
            #region Fields

            /// <summary>
            /// </summary>
            public EventHandler AddCallback;

            #endregion

            #region Properties

            /// <summary>
            ///     Gets a value indicating whether this <see cref="AddShippingCommand" /> is enabled.
            /// </summary>
            /// <value> <c>true</c> if enabled; otherwise, <c>false</c> . </value>
            public override bool Enabled
            {
                get { return (_node is CustomerNode && this.AddCallback != null); }
            }

            #endregion

            #region Methods

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="tool"> The tool. </param>
            /// <param name="toc"> The toc. </param>
            public AddShippingCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            /// <summary>
            ///     Called when [click].
            /// </summary>
            public override void OnClick()
            {
                if(_node is CustomerNode)
                {
                    if(this.AddCallback != null)
                        this.AddCallback(this, EventArgs.Empty);
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: DisplayInactiveCommand

        /// <summary>
        /// </summary>
        private class DisplayInactiveCommand : TreeNodeCommandBase
        {
            #region Fields

            /// <summary>
            /// Occurs when this command shows inactive customers.
            /// </summary>
            public event EventHandler ShowInactive;

            /// <summary>
            /// Occurs when this command hides inactive customers.
            /// </summary>
            public event EventHandler HideInactive;

            #endregion

            #region Properties

            /// <summary>
            ///     Gets a value indicating whether this <see cref="DisplayInactiveCommand" /> is enabled.
            /// </summary>
            /// <value> <c>true</c> if enabled; otherwise, <c>false</c> . </value>
            public override bool Enabled
            {
                get { return base.TreeView != null && base.TreeView.Nodes.Count > 0; }
            }

            #endregion

            #region Methods

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="tool"> The tool. </param>
            /// <param name="toc"> The toc. </param>
            public DisplayInactiveCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            /// <summary>
            ///     Called when [click].
            /// </summary>
            public override void OnClick()
            {
                try
                {
                    var showAll = ((StateButtonTool) Button.Button).Checked;

                    var rootNode = base.TreeView.Nodes[0];

                    if(rootNode != null)
                    {
                        foreach(var node in rootNode.Nodes)
                        {
                            var userNode = node as CustomerNode;

                            if(userNode != null)
                            {
                                userNode.Visible = showAll || userNode.DataRow.Active;

                                // Set the visibility for the contacts as well
                                foreach (var child in userNode.Nodes.OfType<ContactNode>())
                                {
                                    child.RefreshVisibility(showAll);
                                }

                                foreach (var child in userNode.Nodes.OfType<AdditionalContactNode>())
                                {
                                    child.RefreshVisibility(showAll);
                                }
                            }
                        }
                    }

                    if (showAll)
                    {
                        ShowInactive?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        HideInactive?.Invoke(this, EventArgs.Empty);
                    }
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error updating visibility of nodes.");
                }
            }

            #endregion
        }

        #endregion

        #endregion


    }
}