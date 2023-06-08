using System;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Order;
using DWOS.Reports;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.QA
{
    public partial class ReworkJoin : Form
    {
        #region Fields

        private TableAdapterManager _adapterManager;
        private OrdersDataSet _ordersDataset;

        #endregion

        #region Properties

        public int ReworkOrderID { get; set; }

        #endregion

        #region Methods

        public ReworkJoin() { InitializeComponent(); }

        private void LoadData()
        {
            this._ordersDataset = new OrdersDataSet {EnforceConstraints = false};
            this._adapterManager = new TableAdapterManager
            {
                OrderTableAdapter = new OrderTableAdapter {ClearBeforeFill = false},
                InternalReworkTableAdapter = new InternalReworkTableAdapter(),
                OrderSerialNumberTableAdapter = new OrderSerialNumberTableAdapter {  ClearBeforeFill = false },
                OrderFeesTableAdapter = new OrderFeesTableAdapter()
            };

            this._adapterManager.InternalReworkTableAdapter.FillByReworkOrderID(this._ordersDataset.InternalRework, ReworkOrderID);
            this._adapterManager.OrderSerialNumberTableAdapter.FillByOrder(this._ordersDataset.OrderSerialNumber, ReworkOrderID);
            this.txtReworkOrderID.Text = ReworkOrderID.ToString();

            OrdersDataSet.InternalReworkRow internalRework = this._ordersDataset.InternalRework.FirstOrDefault();

            if(internalRework != null)
            {
                this._adapterManager.OrderTableAdapter.FillByOrderID(this._ordersDataset.Order, ReworkOrderID);
                this._adapterManager.OrderTableAdapter.FillByOrderID(this._ordersDataset.Order, internalRework.OriginalOrderID);
                this._adapterManager.OrderSerialNumberTableAdapter.FillByOrder(this._ordersDataset.OrderSerialNumber, internalRework.OriginalOrderID);

                this.txtOriginalOrderID.Text = internalRework.OriginalOrderID.ToString();

                OrdersDataSet.OrderRow origOrder = this._ordersDataset.Order.FindByOrderID(internalRework.OriginalOrderID);
                OrdersDataSet.OrderRow reworkOrder = this._ordersDataset.Order.FindByOrderID(ReworkOrderID);

                if(origOrder != null)
                {
                    this.numOriginalQty.Value = origOrder.PartQuantity;
                    this.numOriginalQty.MaxValue = origOrder.PartQuantity;
                }
                if(reworkOrder != null)
                {
                    this.numReworkQty.Value = reworkOrder.PartQuantity;
                    this.numReworkQty.MaxValue = reworkOrder.PartQuantity;
                }
            }
            else
                this.btnOK.Enabled = false;
        }

        private void SaveData()
        {
            try
            {
                OrdersDataSet.InternalReworkRow internalRework = this._ordersDataset.InternalRework.FirstOrDefault();
                internalRework.Active = false;

                //update original order to get it back into production
                OrdersDataSet.OrderRow origOrder = this._ordersDataset.Order.FindByOrderID(internalRework.OriginalOrderID);
                origOrder.PartQuantity = Convert.ToInt32(this.numReworkQty.Value) + Convert.ToInt32(this.numOriginalQty.Value);
                origOrder.OrderType = origOrder.OriginalOrderType;
                origOrder.Hold = false;
                origOrder.WorkStatus = DetermineNextWorkStatus(origOrder);

                //close out the rework order and remove its quantity
                OrdersDataSet.OrderRow reworkOrder = this._ordersDataset.Order.FindByOrderID(ReworkOrderID);
                reworkOrder.PartQuantity = 0;
                reworkOrder.Status = Settings.Default.OrderStatusClosed;
                reworkOrder.BasePrice = 0M;
                reworkOrder.Invoice = "NA"; //add to prevent getting exported
                reworkOrder.CompletedDate = DateTime.Now;

                this._adapterManager.UpdateAll(this._ordersDataset);

                // Delete fees for rejoined order
                _adapterManager.OrderFeesTableAdapter.DeleteByOrder(ReworkOrderID);

                try
                {
                    if(this.chkReprintWO.Checked)
                    {
                        var wt = new WorkOrderTravelerReport(origOrder);
                        wt.DisplayReport();
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error printing WO Traveler.");
                }

                OrderHistoryDataSet.UpdateOrderHistory(origOrder.OrderID, "Rework Join", "Original order {0} join with rework order {1}.".FormatWith(origOrder.OrderID, reworkOrder.OrderID), SecurityManager.Current.UserName);
                OrderHistoryDataSet.UpdateOrderHistory(reworkOrder.OrderID, "Rework Join", "Original order {0} join with rework order {1}.".FormatWith(origOrder.OrderID, reworkOrder.OrderID), SecurityManager.Current.UserName);

                if (!ApplicationSettings.Current.OrderCheckInEnabled && origOrder.WorkStatus == ApplicationSettings.Current.WorkStatusChangingDepartment)
                {
                    var checkin = new OrderCheckInController(origOrder.OrderID);
                    var checkinResult = checkin.AutoCheckIn(SecurityManager.Current.UserID);

                    if (!checkinResult.Response)
                    {
                        LogManager.GetCurrentClassLogger().Warn($"Auto check-in failed for order {origOrder.OrderID}.");
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error saving rework join.");
            }
        }

        private string DetermineNextWorkStatus(OrdersDataSet.OrderRow order)
        {
            string workStatus =  ApplicationSettings.Current.WorkStatusChangingDepartment;

            using(var taOrderProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
            {
                var orderProcesses = new OrderProcessingDataSet.OrderProcessesDataTable();
                taOrderProcesses.FillBy(orderProcesses, order.OrderID);
                bool hasUncompletedProcesses = orderProcesses.Any(op => op.IsStartDateNull());

                if(!hasUncompletedProcesses)
                {
                    using(var taPart = new PartSummaryTableAdapter())
                    {
                        var partTable = new OrdersDataSet.PartSummaryDataTable();
                        taPart.FillByPart(partTable, order.PartID);
                        OrdersDataSet.PartSummaryRow part = partTable.FirstOrDefault();

                        workStatus = OrderUtilities.WorkStatusAfterProcessing((OrderType)order.OrderType, part?.PartMarking ?? false, order.RequireCoc);
                    }
                }
            }

            return workStatus;
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveData();
            Close();
        }

        private void ReworkJoin_Load(object sender, EventArgs e) { LoadData(); }

        #endregion
    }
}