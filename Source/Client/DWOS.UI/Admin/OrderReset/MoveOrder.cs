using System;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Order;
using DWOS.UI.Utilities;

namespace DWOS.UI.Admin.OrderReset
{
    public partial class MoveOrder: UserControl, IOrderResetPanel
    {
        private bool _dataLoaded;

        public MoveOrder()
        {
            this.InitializeComponent();
        }

        #region IOrderResetPanel Members

        public OrderResetMain ResetMain { get; set; }

        public void OnNavigateTo()
        {
            this.LoadData();
            this.txtSelectedOrders.Clear();

            if(this.ResetMain != null && this.ResetMain.SelectedOrders.Count > 0)
            {
                this.txtSelectedOrders.AppendText("--------- Orders to Move ------------" + Environment.NewLine);
                this.ResetMain.SelectedOrders.ForEach(o => this.txtSelectedOrders.AppendText(o.ToString() + Environment.NewLine));
            }
        }

        public void OnNavigateFrom() {}

        public void Finish()
        {
            this.MoveOrders();
        }

        public bool CanNavigateToNextPanel
        {
            get { return true; }
        }

        public Action<IOrderResetPanel, bool> OnCanNavigateToNextPanelChange { get; set; }

        #endregion

        private void LoadData()
        {
            if(this._dataLoaded)
                return;

            //Work status
            using(var taWS = new d_WorkStatusTableAdapter())
            {
                var workStatus = new OrdersDataSet.d_WorkStatusDataTable();
                {
                    taWS.Fill(workStatus);

                    this.cboWorkStatus.DisplayMember = "WorkStatusID";
                    this.cboWorkStatus.ValueMember = "WorkStatusID";
                    this.cboWorkStatus.DataSource = workStatus;
                    this.cboWorkStatus.DataBind();

                    this.cboWorkStatus.SelectedItem = this.cboWorkStatus.FindItemByValue<string>(p => p == ApplicationSettings.Current.WorkStatusChangingDepartment);
                }
            }

            //Location
            using(var taDept = new d_DepartmentTableAdapter())
            {
                var dept = new OrdersDataSet.d_DepartmentDataTable();
                {
                    taDept.Fill(dept);

                    this.cboLocation.DisplayMember = "DepartmentID";
                    this.cboLocation.ValueMember = "DepartmentID";
                    this.cboLocation.DataSource = dept;
                    this.cboLocation.DataBind();

                    this.cboLocation.SelectedItem = this.cboLocation.FindItemByValue<string>(p => p == Properties.Settings.Default.CurrentDepartment);
                }
            }

            this._dataLoaded = true;
        }

        private void MoveOrders()
        {
            using(var osTA = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
            {
                this.txtSelectedOrders.AppendText(Environment.NewLine + "--------- Moving Orders ------------" + Environment.NewLine);

                foreach(var orderID in this.ResetMain.SelectedOrders)
                {
                    //Reset department
                    var department = this.cboLocation.SelectedItem.DisplayText;
                    osTA.UpdateOrderLocation(department, orderID);
                    OrderHistoryDataSet.UpdateOrderHistory(orderID, "Reset Orders", "Order department reset to " + department + ".", SecurityManager.Current.UserName);
                    this.txtSelectedOrders.AppendText("Order " + orderID + " department reset to " + department + "." + Environment.NewLine);

                    //Reset work status
                    var workStatus = this.cboWorkStatus.SelectedItem.DisplayText;
                    osTA.UpdateWorkStatus(workStatus, orderID);
                    OrderHistoryDataSet.UpdateOrderHistory(orderID, "Reset Orders", "Order work status reset to " + workStatus + ".", SecurityManager.Current.UserName);
                    this.txtSelectedOrders.AppendText("Order " + orderID + " work status reset to " + workStatus + "." + Environment.NewLine);

                    // Stop order timers
                    TimeCollectionUtilities.StopAllOrderTimers(orderID);
                }
            }
        }
    }
}