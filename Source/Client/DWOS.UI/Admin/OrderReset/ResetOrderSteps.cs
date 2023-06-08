using System;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin.OrderReset
{
    public partial class ResetOrderSteps: UserControl, IOrderResetPanel
    {
        #region Fields

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        public ResetOrderSteps()
        {
            this.InitializeComponent();
        }


        private void ResetOrders()
        {
            OrderProcessesTableAdapter taOrderProcesses = null;
            OrderSummaryTableAdapter taOrder = null;
            BatchTableAdapter taBatch = null;

            try
            {
                taOrderProcesses = new OrderProcessesTableAdapter();
                taOrder = new OrderSummaryTableAdapter();
                taBatch = new BatchTableAdapter();

                txtSelectedOrders.AppendText(Environment.NewLine + "--------- Reseting Orders ------------" + Environment.NewLine);

                foreach (var orderID in ResetMain.SelectedOrders)
                {
                    _log.Info($"Resetting processes for WO {orderID}");

                    // If order is in active batch, show warning
                    if (taBatch.GetIsOrderInActiveBatch(orderID) > 0)
                    {
                        _log.Warn("Order is in active batch.");

                        txtSelectedOrders.AppendText($"Order {orderID} is in " +
                            $"an active batch.{Environment.NewLine}");
                    }

                    // Delete order processes
                    taOrderProcesses.DeleteAllInOrder(orderID);

                    // If order still has processes, show warning
                    if (taOrderProcesses.GetCountInOrder(orderID) > 0)
                    {
                        _log.Error("Unable to delete processes for order.");

                        txtSelectedOrders.AppendText($"Order {orderID} did not " +
                            $"have its processes reset.{Environment.NewLine}" +
                            $"    You may need to manually remove processes.{Environment.NewLine}");
                    }

                    // Create order processes
                    OrderProcessingDataSet.OrderSummaryDataTable.CreateOrderProcesses(orderID);
                    OrderHistoryDataSet.UpdateOrderHistory(orderID, "Reset Orders", "Order answers reset.", SecurityManager.Current.UserName);
                    txtSelectedOrders.AppendText("Order " + orderID + " answers reset." + Environment.NewLine);

                    //Reset department back to Sales
                    taOrder.UpdateOrderLocation(ApplicationSettings.Current.DepartmentSales, orderID);
                    OrderHistoryDataSet.UpdateOrderHistory(orderID, "Reset Orders", "Order department reset to " + ApplicationSettings.Current.DepartmentSales + ".", SecurityManager.Current.UserName);
                    txtSelectedOrders.AppendText("Order " + orderID + " department reset to " + ApplicationSettings.Current.DepartmentSales + "." + Environment.NewLine);

                    //Reset work status back to changing departments
                    taOrder.UpdateWorkStatus(ApplicationSettings.Current.WorkStatusChangingDepartment, orderID);
                    OrderHistoryDataSet.UpdateOrderHistory(orderID, "Reset Orders", "Order work status reset to " + ApplicationSettings.Current.WorkStatusChangingDepartment + ".", SecurityManager.Current.UserName);
                    txtSelectedOrders.AppendText("Order " + orderID + " work status reset to " + ApplicationSettings.Current.WorkStatusChangingDepartment + "." + Environment.NewLine);
                }
            }
            finally
            {
                taOrderProcesses?.Dispose();
                taOrder?.Dispose();
                taBatch?.Dispose();
            }
        }

        #endregion

        #region IOrderResetPanel Members

        public OrderResetMain ResetMain { get; set; }

        public void OnNavigateTo()
        {
            this.txtSelectedOrders.Clear();

            if(this.ResetMain != null && this.ResetMain.SelectedOrders.Count > 0)
            {
                this.txtSelectedOrders.AppendText("--------- Orders to Reset ------------" + Environment.NewLine);
                this.ResetMain.SelectedOrders.ForEach(o => this.txtSelectedOrders.AppendText(o.ToString() + Environment.NewLine));
            }
        }

        public void OnNavigateFrom() {}

        public void Finish()
        {
            this.ResetOrders();
        }

        public bool CanNavigateToNextPanel
        {
            get { return true; }
        }

        public Action<IOrderResetPanel, bool> OnCanNavigateToNextPanelChange { get; set; }

        #endregion
    }
}