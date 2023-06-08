using System;
using System.Windows.Forms;

namespace DWOS.UI.Admin.OrderReset
{
    public partial class TaskSelection: UserControl, IOrderResetPanel
    {
        private bool _inResetingState;

        public TaskSelection()
        {
            this.InitializeComponent();
        }

        private OrderResetMain.OrderResetTaskType SelectedTask
        {
            get
            {
                if(this.chkChangeDepartment.Checked)
                    return OrderResetMain.OrderResetTaskType.MoveOrder;
                if(this.chkCloseOrders.Checked)
                    return OrderResetMain.OrderResetTaskType.CloseOrder;
                if(this.chkReset.Checked)
                    return OrderResetMain.OrderResetTaskType.Reset;
                if(this.chkOpenOrders.Checked)
                    return OrderResetMain.OrderResetTaskType.OpenOrder;

                return OrderResetMain.OrderResetTaskType.CloseOrder;
            }
        }

        #region IOrderResetPanel Members

        public OrderResetMain ResetMain { get; set; }

        public void OnNavigateTo()
        {
            if(this.OnCanNavigateToNextPanelChange != null)
                this.OnCanNavigateToNextPanelChange(this, this.CanNavigateToNextPanel);
        }

        public Action<IOrderResetPanel, bool> OnCanNavigateToNextPanelChange { get; set; }

        public void OnNavigateFrom()
        {
            this.ResetMain.UpdateTaskType(this.SelectedTask);
        }

        public void Finish() {}

        public bool CanNavigateToNextPanel
        {
            get { return this.chkChangeDepartment.Checked || this.chkCloseOrders.Checked || this.chkReset.Checked || this.chkOpenOrders.Checked;}
        }

        #endregion

        private void chkCloseOrders_AfterCheckStateChanged(object sender, EventArgs e)
        {
            if(this._inResetingState)
                return;

            this._inResetingState = true;

            if(this.chkReset != sender)
                this.chkReset.Checked = false;
            if(this.chkChangeDepartment != sender)
                this.chkChangeDepartment.Checked = false;
            if(this.chkCloseOrders != sender)
                this.chkCloseOrders.Checked = false;
            if(this.chkOpenOrders != sender)
                this.chkOpenOrders.Checked = false;

            if(this.OnCanNavigateToNextPanelChange != null)
                this.OnCanNavigateToNextPanelChange(this, this.CanNavigateToNextPanel);

            this._inResetingState = false;
        }
    }
}