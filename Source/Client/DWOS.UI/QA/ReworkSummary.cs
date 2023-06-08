using System;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ListsDataSetTableAdapters;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using Infragistics.Win.UltraWinListView;

namespace DWOS.UI.QA
{
    public partial class ReworkSummary : Form
    {
        #region Properties

        public int OrderID { get; set; }

        public int NewOrderID { get; set; }

        public int InternalReworkID { get; set; }

        #endregion

        #region Methods

        public ReworkSummary() { InitializeComponent(); }

        private void LoadData()
        {
            this.lvwProcesses.Items.Clear();
            this.taOrder.FillByOrderID(this.dsOrders.Order, OrderID);
            this.taOrder.FillByOrderID(this.dsOrders.Order, NewOrderID);

            OrdersDataSet.OrderRow order = this.dsOrders.Order.FindByOrderID(OrderID);
            OrdersDataSet.OrderRow newOrder = this.dsOrders.Order.FindByOrderID(NewOrderID);

            var internalReworkTableAdapter = new InternalReworkTableAdapter {ClearBeforeFill = true};
            internalReworkTableAdapter.FillBy(this.dsOrders.InternalRework, InternalReworkID);

            OrdersDataSet.InternalReworkRow internalRework = this.dsOrders.InternalRework.FindByInternalReworkID(InternalReworkID);

            if(internalRework != null)
            {
                var reasons = new ListsDataSet.d_ReworkReasonDataTable();
                using(var taReworkReasons = new Data.Datasets.ListsDataSetTableAdapters.d_ReworkReasonTableAdapter())
                    taReworkReasons.Fill(reasons);

                ListsDataSet.d_ReworkReasonRow reason = reasons.FindByReworkReasonID(internalRework.ReworkReasonID);
                if(reason != null)
                    this.txtReason.Text = reason.Name;

                this.txtReworkType.Text = internalRework.ReworkType;

                if(internalRework.ReworkType == ReworkType.Split.ToString())
                    this.picReworkType.Image = Properties.Resources.Order_Split_32;
                else if(internalRework.ReworkType == ReworkType.SplitHold.ToString())
                    this.picReworkType.Image = Properties.Resources.Hold_32;
                else if(internalRework.ReworkType == ReworkType.Quarantine.ToString())
                    this.picReworkType.Image = Properties.Resources.Quarantine_32;
                else if(internalRework.ReworkType == ReworkType.Lost.ToString())
                    this.picReworkType.Image = Properties.Resources.Lost_32;
            }

            if(order != null)
                AddItem(order, internalRework);
            if(newOrder != null)
                AddItem(newOrder, internalRework);
        }

        private void AddItem(OrdersDataSet.OrderRow order, OrdersDataSet.InternalReworkRow internalRework)
        {
            string workStatus = order.WorkStatus;

            if(order.Hold && internalRework != null && !internalRework.IsHoldLocationIDNull())
                workStatus += (" @ " + internalRework.HoldLocationID);

            var partQuantity = order.IsPartQuantityNull() ? 0 : order.PartQuantity;

            var lvi = new UltraListViewItem(order.OrderID, new object[] { workStatus, partQuantity });

            switch(order.OrderType)
            {
                case 1:
                    if(order.Hold)
                        lvi.Appearance.Image = Properties.Resources.Hold_16;
                    break;
                case 3: //OrderType.ReworkExt
                    lvi.Appearance.Image = Properties.Resources.Repair_Blue_16;
                    break;
                case 4: //OrderType.ReworkInt
                    lvi.Appearance.Image = Properties.Resources.Repair_16;
                    break;
                case 5: //OrderType.ReworkHold
                case 6: //OrderType.Lost
                    lvi.Appearance.Image = Properties.Resources.Hold_16;
                    break;
                case 7: //OrderType.Quarantine
                    lvi.Appearance.Image = Properties.Resources.Quarantine_16;
                    break;
            }

            this.lvwProcesses.Items.Add(lvi);
        }

        #endregion

        #region Events

        private void ReworkSummary_Load(object sender, EventArgs e) { LoadData(); }

        #endregion
    }
}