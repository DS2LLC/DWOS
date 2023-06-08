using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DWOS.UI.Utilities;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data;
using DWOS.Data.Datasets;

namespace DWOS.UI.Admin.OrderReset
{
    public partial class OpenOrder : UserControl, IOrderResetPanel
    {
        #region Fields

        private bool _dataLoaded;

        #endregion

        #region Methods

        public OpenOrder()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            if (!this._dataLoaded)
            {
                using(var taWS = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.d_WorkStatusTableAdapter())
                {
                    using (var workStatus = new DWOS.Data.Datasets.OrdersDataSet.d_WorkStatusDataTable())
                    {
                        taWS.Fill(workStatus);

                        this.cboWorkStatus.DisplayMember = workStatus.WorkStatusIDColumn.ColumnName;
                        this.cboWorkStatus.ValueMember = workStatus.WorkStatusIDColumn.ColumnName;
                        this.cboWorkStatus.DataSource = new DataView(workStatus);
                        this.cboWorkStatus.DataBind();

                        this.cboWorkStatus.SelectedItem = this.cboWorkStatus.FindItemByValue<string>(p => p == ApplicationSettings.Current.WorkStatusChangingDepartment);
                    }
                }

                using (var taDept = new d_DepartmentTableAdapter())
                {
                    using (var dept = new DWOS.Data.Datasets.OrdersDataSet.d_DepartmentDataTable())
                    {
                        taDept.Fill(dept);
                        this.cboLocation.DisplayMember = dept.DepartmentIDColumn.ColumnName;
                        this.cboLocation.ValueMember = dept.DepartmentIDColumn.ColumnName;
                        this.cboLocation.DataSource = new DataView(dept);
                        this.cboLocation.DataBind();

                        this.cboLocation.SelectedItem = this.cboLocation.FindItemByValue<string>(p => p == Properties.Settings.Default.CurrentDepartment);
                    }
                }

                this._dataLoaded = true;
            }
        }

        private void OpenOrders()
        {
            this.txtSelectedOrders.AppendText(Environment.NewLine + "--------- Opening Orders ------------" + Environment.NewLine);

            var department = this.cboLocation.SelectedItem.DisplayText;
            var workStatus = this.cboWorkStatus.SelectedItem.DisplayText;

            DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter osTA = null;
            OrderTableAdapter taOrders = null;
            Data.Datasets.OrdersDataSetTableAdapters.SalesOrderTableAdapter taSalesOrders = null;
            OrdersDataSet dsOrders = null;

            try
            {
                osTA = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter();
                taOrders = new OrderTableAdapter();

                foreach (var orderID in this.ResetMain.SelectedOrders)
                {
                    // Reset department
                    osTA.UpdateOrderLocation(department, orderID);
                    OrderHistoryDataSet.UpdateOrderHistory(orderID, "Reset Orders", "Order department reset to " + department + ".", SecurityManager.Current.UserName);
                    this.txtSelectedOrders.AppendText("Order " + orderID + " department reset to " + department + "." + Environment.NewLine);

                    // Reset work status
                    osTA.UpdateWorkStatus(workStatus, orderID);
                    OrderHistoryDataSet.UpdateOrderHistory(orderID, "Reset Orders", "Order work status reset to " + workStatus + ".", SecurityManager.Current.UserName);
                    this.txtSelectedOrders.AppendText("Order " + orderID + " work status reset to " + workStatus + "." + Environment.NewLine);

                    // Reset order status
                    osTA.UpdateStatus(Properties.Settings.Default.OrderStatusOpen, orderID);
                    OrderHistoryDataSet.UpdateOrderHistory(orderID, "Reset Orders", "Order order status reset to " + Properties.Settings.Default.OrderStatusOpen + ".", SecurityManager.Current.UserName);
                    this.txtSelectedOrders.AppendText("Order " + orderID + " order status reset to " + Properties.Settings.Default.OrderStatusOpen + "." + Environment.NewLine);

                    // Remove order closed date
                    osTA.UpdateDateCompleted(null, orderID);
                    this.txtSelectedOrders.AppendText(Environment.NewLine);


                    dsOrders = new OrdersDataSet()
                    {
                        EnforceConstraints = false
                    };

                    taOrders.FillByOrderID(dsOrders.Order, orderID);

                    if (dsOrders.Order.Count == 1 && !dsOrders.Order[0].IsSalesOrderIDNull())
                    {
                        taSalesOrders = new Data.Datasets.OrdersDataSetTableAdapters.SalesOrderTableAdapter();
                        taSalesOrders.FillBySalesOrder(dsOrders.SalesOrder, dsOrders.Order[0].SalesOrderID);

                        if (dsOrders.SalesOrder.Count == 1)
                        {
                            dsOrders.SalesOrder[0].Status = Properties.Settings.Default.OrderStatusOpen;
                            dsOrders.SalesOrder[0].SetCompletedDateNull();
                            taSalesOrders.Update(dsOrders.SalesOrder);
                        }
                    }
                }
            }
            finally
            {
                if (osTA != null)
                {
                    osTA.Dispose();
                }

                if (taOrders != null)
                {
                    taOrders.Dispose();
                }

                if (dsOrders != null)
                {
                    dsOrders.Dispose();
                }

                if (taSalesOrders != null)
                {
                    taSalesOrders.Dispose();
                }
            }
        }

        #endregion

        #region IOrderResetPanel Members

        public OrderResetMain ResetMain { get; set; }

        public Action<IOrderResetPanel, bool> OnCanNavigateToNextPanelChange { get; set; }

        public bool CanNavigateToNextPanel
        {
            get { return true; }
        }

        public void OnNavigateTo()
        {
            LoadData();
            this.txtSelectedOrders.Clear();

            if (this.ResetMain != null)
            {
                this.txtSelectedOrders.AppendText("--------- Orders to Open ------------" + Environment.NewLine);

                foreach (var order in this.ResetMain.SelectedOrders)
                {
                    this.txtSelectedOrders.AppendText(order.ToString());
                    this.txtSelectedOrders.AppendText(Environment.NewLine);
                }
            }
        }

        public void OnNavigateFrom() { }

        public void Finish()
        {
            OpenOrders();
        }

        #endregion
    }
}
