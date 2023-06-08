using System;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Data.Order;
using DWOS.UI.Utilities;

namespace DWOS.UI.Admin.OrderReset
{
    public partial class CloseOrder: UserControl, IOrderResetPanel
    {
        public CloseOrder()
        {
            this.InitializeComponent();
            this.dtCloseDate.DateTime = DateTime.Now;
        }

        #region IOrderResetPanel Members

        public OrderResetMain ResetMain { get; set; }

        public void OnNavigateTo()
        {
            this.txtSelectedOrders.Clear();

            if(this.ResetMain != null && this.ResetMain.SelectedOrders.Count > 0)
            {
                this.txtSelectedOrders.AppendText("--------- Orders to Close ------------" + Environment.NewLine);
                this.ResetMain.SelectedOrders.ForEach(o => this.txtSelectedOrders.AppendText(o.ToString() + Environment.NewLine));
            }
        }

        public void OnNavigateFrom() {}

        public void Finish()
        {
            this.CloseOrders();
        }

        public bool CanNavigateToNextPanel
        {
            get { return true; }
        }

        public Action<IOrderResetPanel, bool> OnCanNavigateToNextPanelChange { get; set; }

        #endregion

        private void CloseOrders()
        {
            if(this.dtCloseDate.Value == null)
                return;

            OrdersDataSet dsOrders = new OrdersDataSet()
            {
                EnforceConstraints = false
            };

            Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter taOrders = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter();
            Data.Datasets.OrdersDataSetTableAdapters.SalesOrderTableAdapter taSalesOrders = new Data.Datasets.OrdersDataSetTableAdapters.SalesOrderTableAdapter();

            using(var osTA = new OrderSummaryTableAdapter())
            {
                this.txtSelectedOrders.AppendText(Environment.NewLine + "--------- Closing Orders ------------" + Environment.NewLine);

                foreach(var orderID in this.ResetMain.SelectedOrders)
                {
                    //Reset department
                    osTA.UpdateOrderLocation(ApplicationSettings.Current.DepartmentShipping, orderID);
                    OrderHistoryDataSet.UpdateOrderHistory(orderID, "Reset Orders", "Order department reset to " + ApplicationSettings.Current.DepartmentShipping + ".", SecurityManager.Current.UserName);
                    this.txtSelectedOrders.AppendText("Order " + orderID + " department reset to " + ApplicationSettings.Current.DepartmentShipping + "." + Environment.NewLine);

                    //Reset work status
                    osTA.UpdateWorkStatus( ApplicationSettings.Current.WorkStatusCompleted, orderID);
                    OrderHistoryDataSet.UpdateOrderHistory(orderID, "Reset Orders", "Order work status reset to " +  ApplicationSettings.Current.WorkStatusCompleted + ".", SecurityManager.Current.UserName);
                    this.txtSelectedOrders.AppendText("Order " + orderID + " work status reset to " +  ApplicationSettings.Current.WorkStatusCompleted + "." + Environment.NewLine);

                    //Reset order status
                    osTA.UpdateStatus(Properties.Settings.Default.OrderStatusClosed, orderID);
                    OrderHistoryDataSet.UpdateOrderHistory(orderID, "Reset Orders", "Order order status reset to " + Properties.Settings.Default.OrderStatusClosed + ".", SecurityManager.Current.UserName);
                    this.txtSelectedOrders.AppendText("Order " + orderID + " order status reset to " + Properties.Settings.Default.OrderStatusClosed + "." + Environment.NewLine);

                    //Set order closed date
                    osTA.UpdateDateCompleted(this.dtCloseDate.DateTime, orderID);
                    this.txtSelectedOrders.AppendText(Environment.NewLine);

                    // Stop order timers
                    TimeCollectionUtilities.StopAllOrderTimers(orderID);

                    //Verify and close sales order if no other work orders are assigned to it
                    taOrders.FillByOrderID(dsOrders.Order, orderID);
                    if (dsOrders.Order.Count == 1)
                    {
                        if (!dsOrders.Order[0].IsSalesOrderIDNull())
                        {
                            //Check if there are any open orders that are assigned to this sales order
                            int count = taOrders.GetOpenOrdersCountBySalesOrderID(dsOrders.Order[0].SalesOrderID).GetValueOrDefault();
                            if (count == 0)
                            {
                                taSalesOrders.FillBySalesOrder(dsOrders.SalesOrder, dsOrders.Order[0].SalesOrderID);
                                if (dsOrders.SalesOrder.Count == 1)
                                {
                                    dsOrders.SalesOrder[0].Status = "Closed";
                                    dsOrders.SalesOrder[0].CompletedDate = DateTime.Now;
                                    taSalesOrders.Update(dsOrders.SalesOrder);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}