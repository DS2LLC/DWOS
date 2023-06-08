using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Order;
using DWOS.UI.Utilities;
using DWOS.Utilities.Validation;
using Infragistics.Win.UltraWinEditors;
using NLog;

namespace DWOS.UI.Sales.Order
{
    using Infragistics.Win;
    using Infragistics.Win.UltraWinToolTip;
    using Properties;
    using Reports;

    /// <summary>
    /// Dialog that adds a new Work Order to any active Blanket PO.
    /// </summary>
    public partial class NewBlanketPOOrderDialog : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private List <int> _customerPartsLoaded = new List <int>();

        private CustomersDataset.Customer_FieldsDataTable _customerFields;
        private DWOS.Utilities.Validation.ValidatorManager _validationManager;

        #endregion

        #region Methods

        public NewBlanketPOOrderDialog()
        {
            InitializeComponent();
            _validationManager = new DWOS.Utilities.Validation.ValidatorManager();
        }

        private void LoadData()
        {
            this.taCustomerSummary.FillByBlanketPOActive(dsOrders.CustomerSummary);
            this.taOrderTemplate.FillByActive(dsOrders.OrderTemplate, true);

            this.cboCustomer.DataSource = new DataView(dsOrders.CustomerSummary);
            this.cboCustomer.DisplayMember = dsOrders.CustomerSummary.NameColumn.ColumnName;
            this.cboCustomer.ValueMember = dsOrders.CustomerSummary.CustomerIDColumn.ColumnName;

            this.cboPart.DataSource = new DataView(dsOrders.PartSummary);
            this.cboPart.DisplayMember = dsOrders.PartSummary.NameColumn.ColumnName;
            this.cboPart.ValueMember = dsOrders.PartSummary.PartIDColumn.ColumnName;

            this.cboBlanketPO.DataSource = new DataView(dsOrders.OrderTemplate);
            this.cboBlanketPO.DisplayMember = dsOrders.OrderTemplate.OrderTemplateIDColumn.ColumnName;
            this.cboBlanketPO.ValueMember = dsOrders.OrderTemplate.OrderTemplateIDColumn.ColumnName;

            _validationManager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboCustomer, "Customer required."), errProvider));
            _validationManager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboPart, "Part required."), errProvider));
            _validationManager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboBlanketPO, "Blanket PO required."), errProvider));
            _validationManager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtCustomerWO, "Customer WO required."), errProvider));
            _validationManager.Add(new ImageDisplayValidator(new NumericControlValidator(this.numPartQty) { MinimumValue = 1 }, errProvider));
            _validationManager.ValidationSummary = new TabValidationDisplay(tabFields);
        }

        private void LoadPart()
        {
            //clear selection
            this.cboPart.SelectedItem = null;
            UpdateFilter(this.cboPart, "1 = 0");

            if(this.cboCustomer.SelectedItem == null)
            {
                this.cboPart.Enabled = false;
                return;
            }
            this.cboPart.Enabled = true;

            var row = ((DataRowView) this.cboCustomer.SelectedItem.ListObject).Row as OrdersDataSet.CustomerSummaryRow;

            //load parts for the customer
            if(!this._customerPartsLoaded.Contains(row.CustomerID))
            {
                this.taPartSummary.FillByBlanketPOActive(dsOrders.PartSummary, row.CustomerID);
                this._customerPartsLoaded.Add(row.CustomerID);
            }

            UpdateFilter(this.cboPart, "CustomerID = " + row.CustomerID);
        }

        private void LoadCustomFields()
        {
            var row = ((DataRowView) this.cboCustomer.SelectedItem.ListObject).Row as OrdersDataSet.CustomerSummaryRow;

            if(row != null)
            {
                var fieldCount = this.customFieldsWidget.CreateCustomFields(row.CustomerID, _validationManager, tipManager, errProvider);

                if(fieldCount > 0)
                    this.tabFields.Tabs[1].Visible = true;
                else
                    this.tabFields.Tabs[1].Visible = false;
            }
            else
                this.customFieldsWidget.ClearCustomFields(_validationManager);
        }

        private void LoadBlanketPO()
        {
            //clear selection
            this.cboBlanketPO.SelectedItem = null;
            UpdateFilter(this.cboBlanketPO, "1 = 0");

            if(this.cboPart.SelectedItem == null)
            {
                this.cboBlanketPO.Enabled = false;
                return;
            }
            this.cboBlanketPO.Enabled = true;

            var row = ((DataRowView) this.cboPart.SelectedItem.ListObject).Row as OrdersDataSet.PartSummaryRow;
            UpdateFilter(this.cboBlanketPO, "PartID = " + row.PartID);

            if(this.cboBlanketPO.Items.Count > 0)
                this.cboBlanketPO.SelectedIndex = 0;
        }

        private void LoadSelectedPO()
        {
            this.numPartQty.Enabled = this.cboBlanketPO.SelectedItem != null;
            this.txtCustomerWO.Enabled = this.cboBlanketPO.SelectedItem != null;

            if(this.cboBlanketPO.SelectedItem != null)
            {
                var row = ((DataRowView) this.cboBlanketPO.SelectedItem.ListObject).Row as OrdersDataSet.OrderTemplateRow;

                if (row != null)
                {
                    taOrder.FillByOrderTemplate(dsOrders.Order, row.OrderTemplateID);
                    var allocHelper = new BlanketAllocationHelper(dsOrders, taOrder);
                    var usedQty = allocHelper.GetAllocatedQuantity(row);

                    this.numPartQty.MaxValue = Math.Max(0, row.InitialQuantity - usedQty);

                    // DIsplay tooltip showing part availability
                    tipManager.SetUltraToolTip(this.numPartQty, new UltraToolTipInfo("{0} parts available of {1} total parts.".FormatWith(row.InitialQuantity - usedQty, row.InitialQuantity), ToolTipImage.Info, "Part Availability", DefaultableBoolean.True));
                }
                else
                {
                    this.numPartQty.Value = 0;
                    this.numPartQty.ResetMaxValue();
                }
            }
            else
            {
                this.numPartQty.Value = 0;
                this.numPartQty.ResetMaxValue();
            }
        }

        private void UpdateFilter(UltraComboEditor cbo, string filter)
        {
            var dv = cbo.DataSource as DataView;

            if(dv != null)
            {
                //if filter changed
                if(dv.RowFilter != filter)
                {
                    dv.RowFilter = filter;
                    cbo.DataBind();

                    //reread value from data source to ensure correct item is selected in list
                    if(cbo.DataBindings.Count > 0)
                        cbo.DataBindings[0].ReadValue();

                    _log.Info("Updating {0} filter to '{1}'", cbo.Name, filter);
                }
            }
        }

        private void UpdateValidators()
        {
            var row = ((DataRowView)this.cboCustomer.SelectedItem.ListObject).Row as OrdersDataSet.CustomerSummaryRow;

            if(row == null)
                return;

            var customerId = row.CustomerID;

            if (this._customerFields == null || this._customerFields.Count(cf => cf.CustomerID == customerId) < 1)
            {
                _log.Debug("Loading customer fields for customer " + customerId);

                if (this._customerFields == null)
                    this._customerFields = new CustomersDataset.Customer_FieldsDataTable();

                using (var ta = new DWOS.Data.Datasets.CustomersDatasetTableAdapters.Customer_FieldsTableAdapter { ClearBeforeFill = false })
                    ta.FillBy(this._customerFields, customerId);
            }

            var customerWOValidator = this._validationManager.Find(this.txtCustomerWO);
            if (customerWOValidator != null)
            {
                var customerWOField = this._customerFields.FirstOrDefault(r => r.CustomerID == customerId && r.FieldID == 1); //MAGIC NUMBER
                customerWOValidator.IsEnabled = customerWOField != null && customerWOField.Required;
            }
        }

        private OrdersDataSet.OrderRow AddOrder()
        {
            try
            {
                OrdersDataSet.OrderTemplateRow orderTemplate = null;

                if (cboBlanketPO.SelectedItem != null)
                {
                    var selectedTemplateView = cboBlanketPO.SelectedItem.ListObject
                        as DataRowView;

                    orderTemplate = selectedTemplateView?.Row
                        as OrdersDataSet.OrderTemplateRow;
                }

                if (orderTemplate == null)
                {
                    _log.Warn("User did not select Blanket PO.");
                    return null;
                }

                using (new UsingWaitCursor(this))
                {
                    var appSettings = ApplicationSettings.Current;

                    // Create Work Order and associate it with the selected Blanket PO
                    var factory = new OrderFactory();
                    factory.Load();

                    var order = factory.AddOrder(orderTemplate,
                        Convert.ToInt32(numPartQty.Value),
                        customFieldsWidget.CustomFields);

                    var customerWO = txtCustomerWO.Text;

                    if (!string.IsNullOrEmpty(customerWO))
                    {
                        order.CustomerWO = customerWO;
                    }

                    // Auto check-in
                    if (!appSettings.OrderCheckInEnabled && order.WorkStatus == appSettings.WorkStatusChangingDepartment)
                    {
                        var firstProcess = order.GetOrderProcessesRows()
                            .Where(op => op.IsValidState())
                            .OrderBy(op => op.StepOrder)
                            .FirstOrDefault();

                        if (firstProcess != null)
                        {
                            if (order.WorkStatus != appSettings.WorkStatusInProcess)
                            {
                                order.WorkStatus = appSettings.WorkStatusInProcess;
                            }

                            if (order.CurrentLocation != firstProcess.Department)
                            {
                                order.CurrentLocation = firstProcess.Department;
                            }

                            if (firstProcess.IsStartDateNull())
                            {
                                firstProcess.StartDate = DateTime.Now;
                            }
                        }
                    }

                    // Save Work Order
                    factory.Save();

                    // Save entry in order history
                    OrderHistoryDataSet.UpdateOrderHistory(order.OrderID,
                        "New PO Order",
                        $"Order was created from Blanket PO {orderTemplate.OrderTemplateID}",
                        SecurityManager.Current.UserName);

                    // Show notification
                    const string flyoutTitle = "New Order";
                    DWOSApp.MainForm.FlyoutManager.DisplayFlyout(
                            flyoutTitle,
                            "Order {0} was created.".FormatWith(order.OrderID));

                    return order;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error adding new order.");
                return null;
            }
        }

        private void DisposeMe()
        {
            dsOrders = null;
            taOrder?.Dispose();
            taOrderTemplate?.Dispose();
            taCustomerSummary?.Dispose();
            taPartSummary?.Dispose();

            taOrder = null;
            taOrderTemplate = null;
            taCustomerSummary = null;
            taPartSummary = null;
        }

        #endregion

        #region Events

        private void FindBlanketPO_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadPart();
        }

        private void cboBlanketPO_SelectionChanged(object sender, EventArgs e) { LoadSelectedPO(); }

        private void btnCancel_Click(object sender, EventArgs e) { Close(); }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(_validationManager.ValidateControls())
            {

                var order = AddOrder();
                
                if (order != null)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void cboCustomer_SelectionChanged(object sender, EventArgs e)
        {
            LoadPart();
            LoadCustomFields();
            UpdateValidators();
        }

        private void cboPart_SelectionChanged(object sender, EventArgs e) { LoadBlanketPO(); }

        #endregion
    }
}