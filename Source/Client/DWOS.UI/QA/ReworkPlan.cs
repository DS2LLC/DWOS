using System;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ListsDataSetTableAdapters;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Reports;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;
using NLog;
using DWOS.UI.Reports;
using DWOS.Data.Order;

namespace DWOS.UI.QA
{
    public partial class ReworkPlan : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public int OrderID { get; set; }

        private ReworkType OrderReworkType { get; set; }

        #endregion

        #region Methods

        public ReworkPlan() { InitializeComponent(); }

        private void LoadData()
        {
            try
            {
                //Load Order Info
                this.taOrder.FillByOrderID(this.dsOrders.Order, OrderID);

                //Load Part Info
                using(var taParts = new PartSummaryTableAdapter())
                    taParts.FillByOrder(this.dsOrders.PartSummary, OrderID);

                //Load Order Processes
                taOrderProcesses.FillBy(this.dsOrders.OrderProcesses, OrderID);

                // Load serial numbers
                taOrderSerialNumber.FillByOrder(this.dsOrders.OrderSerialNumber, OrderID);

                //Load Processes Aliases
                using(var taProcesses = new ProcessAliasSummaryTableAdapter())
                    taProcesses.FillByOrder(this.dsOrders.ProcessAliasSummary, OrderID);

                var orderRow = this.dsOrders.Order.FindByOrderID(OrderID);
                this.txtOrderID.Text = OrderID.ToString();
                this.txtPartID.Text = orderRow.PartSummaryRow != null ? orderRow.PartSummaryRow.Name : orderRow.PartID.ToString();
                this.txtUser.Text = SecurityManager.Current.UserName;
                this.dteDate.DateTime = DateTime.Now;

                OrdersDataSet.InternalReworkRow internalRework = null;

                //see if this is a split
                taInternalRework.FillByReworkOrderID(this.dsOrders.InternalRework, OrderID);
                internalRework = this.dsOrders.InternalRework.FirstOrDefault(ir => ir.ReworkOrderID == OrderID && ir.Active && (ir.ReworkType == ReworkType.Split.ToString() || ir.ReworkType == ReworkType.SplitHold.ToString()));

                if(internalRework == null)
                {
                    taInternalRework.FillByOriginalOrderID(this.dsOrders.InternalRework, OrderID);
                    internalRework = this.dsOrders.InternalRework.FirstOrDefault(ir => ir.OriginalOrderID == OrderID && ir.Active && (ir.ReworkType == ReworkType.Full.ToString()));
                }

                if(internalRework != null)
                {
                    var reasons = new ListsDataSet.d_ReworkReasonDataTable();
                    using (var taReworkReasons = new Data.Datasets.ListsDataSetTableAdapters.d_ReworkReasonTableAdapter())
                        taReworkReasons.Fill(reasons);

                    ListsDataSet.d_ReworkReasonRow reason = reasons.FindByReworkReasonID(internalRework.ReworkReasonID);
                    if(reason != null)
                        this.txtReason.Text = reason.Name;

                    this.txtReworkType.Text = internalRework.ReworkType;
                    this.OrderReworkType = internalRework.ReworkType.ConvertToEnum<ReworkType>();

                    if (OrderReworkType == ReworkType.Split)
                        this.picReworkType.Image = Resources.Order_Split_32;
                    else if (OrderReworkType == ReworkType.SplitHold)
                        this.picReworkType.Image = Resources.Hold_32;
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error during load data of rework plan.");
            }
        }

        private void SaveData()
        {
            try
            {
                var orderRow = this.dsOrders.Order.FindByOrderID(OrderID);

                if(orderRow == null)
                    return;
                
                //ensure order not on hold
                orderRow.Hold = false;
                orderRow.WorkStatus = ApplicationSettings.Current.WorkStatusChangingDepartment;

                this.taManager.UpdateAll(this.dsOrders);

                try
                {
                    var wt = new WorkOrderTravelerReport(orderRow);
                   
                    if(this.chkQuickPrint.Checked)
                        wt.PrintReport();
                    else
                        wt.DisplayReport();
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error printing WO Traveler.");
                }

                OrderHistoryDataSet.UpdateOrderHistory(OrderID, "Rework Plan", "Order rework was planned.", SecurityManager.Current.UserName);
                TimeCollectionUtilities.StopAllOrderTimers(OrderID);

                if (!ApplicationSettings.Current.OrderCheckInEnabled)
                {
                    var checkin = new OrderCheckInController(OrderID);
                    var checkinResult = checkin.AutoCheckIn(SecurityManager.Current.UserID);

                    if (!checkinResult.Response)
                    {
                        LogManager.GetCurrentClassLogger().Warn($"Auto check-in failed for order {OrderID}.");
                    }
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error saving rework planning data.");
            }
        }

        #endregion

        #region Events

        private void ReworkPlan_Load(object sender, EventArgs e)
        {
            try
            {
                LoadData();

                this.processEditor.DisplayCOCColumn = OrderReworkType != ReworkType.SplitHold; // Hide COC if SplitHold
                
                this.processEditor.LoadData(this.dsOrders, true);
                this.processEditor.LoadProcesses(OrderID);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error during load.");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                SaveData();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error during save.");
            }
        }

        #endregion
    }
}