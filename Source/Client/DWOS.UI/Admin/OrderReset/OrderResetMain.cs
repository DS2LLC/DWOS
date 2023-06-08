using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DWOS.UI.Admin.OrderReset
{
    public partial class OrderResetMain: Form
    {
        #region OrderResetTaskType enum

        public enum OrderResetTaskType
        {
            CloseOrder,
            Reset,
            MoveOrder,
            OpenOrder
        }

        #endregion

        private int _currentPanelIndex = -1;
        private List<IOrderResetPanel> _panels;
        public List<int> SelectedOrders { get; set; }

        public OrderResetTaskType CurrentTask { get; set; }

        #region Methods

        public OrderResetMain()
        {
            this.InitializeComponent();
        }

        private void InitializeWizard()
        {
            this._panels = new List<IOrderResetPanel>();
            this._panels.Add(new TaskSelection{ResetMain = this, OnCanNavigateToNextPanelChange = this.OnCanNavigateToNextPanelChanged});
            this._panels.Add(new OrderSelection{ResetMain = this, OnCanNavigateToNextPanelChange = this.OnCanNavigateToNextPanelChanged});
        }

        public void UpdateTaskType(OrderResetTaskType taskType)
        {
            this.CurrentTask = taskType;
            Text = "Work Order Administrative Tasks";

            while(this._panels.Count > 2)
                this._panels.RemoveAt(this._panels.Count - 1);

            switch(taskType)
            {
                case OrderResetTaskType.CloseOrder:
                    this._panels.Add(new CloseOrder{ResetMain = this, OnCanNavigateToNextPanelChange = this.OnCanNavigateToNextPanelChanged});
                    Text += " - Close Orders";
                    break;
                case OrderResetTaskType.Reset:
                    this._panels.Add(new ResetOrderSteps{ResetMain = this, OnCanNavigateToNextPanelChange = this.OnCanNavigateToNextPanelChanged});
                    Text += " - Reset Orders";
                    break;
                case OrderResetTaskType.MoveOrder:
                    this._panels.Add(new MoveOrder{ResetMain = this, OnCanNavigateToNextPanelChange = this.OnCanNavigateToNextPanelChanged});
                    Text += " - Move Orders";
                    break;
                case OrderResetTaskType.OpenOrder:
                    this._panels.Add(new OpenOrder{ResetMain = this, OnCanNavigateToNextPanelChange = this.OnCanNavigateToNextPanelChanged});
                    Text += " - Open Orders";
                    break;
                default:
                    break;
            }
        }

        private void MoveToPanel(int panelIndex)
        {
            //if not a valid index then exit
            if(!this.IsValidPanelIndex(panelIndex))
                return;

            //notify current panel that we are leaving it
            if(this._currentPanelIndex != panelIndex && this.IsValidPanelIndex(this._currentPanelIndex))
                this._panels[this._currentPanelIndex].OnNavigateFrom();

            this._currentPanelIndex = panelIndex;

            var panel = this._panels[panelIndex] as Control;
            this.pnlHolder.Controls.Clear();
            this.pnlHolder.Controls.Add(panel);
            panel.Dock = DockStyle.Fill;

            this._panels[panelIndex].OnNavigateTo();

            //Update buttons based on current index
            this.btnNext.Text = this._currentPanelIndex == this._panels.Count - 1 ? "Finish" : "Next >";
            this.btnNext.Enabled = this._panels[this._currentPanelIndex].CanNavigateToNextPanel;
            this.btnPrevious.Enabled = this._currentPanelIndex > 0;
        }

        private void Finish()
        {
            this._panels.ForEach(rp => rp.Finish());
            this.btnNext.Enabled = false;
            this.btnPrevious.Enabled = false;
            this.btnCancel.Text = "Close";
        }

        private bool IsValidPanelIndex(int panelIndex)
        {
            return panelIndex >= 0 && panelIndex < this._panels.Count;
        }

        private void OnCanNavigateToNextPanelChanged(IOrderResetPanel panel, bool status)
        {
            if(this.IsValidPanelIndex(this._currentPanelIndex) && this._panels[this._currentPanelIndex] == panel)
                this.btnNext.Enabled = status;
        }

        private void OnDisposing()
        {
            this._panels.ForEach(rp => ((Control)rp).Dispose());
            this._panels.Clear();
        }

        #endregion

        #region Events

        private void OrderResetMain_Load(object sender, EventArgs e)
        {
            this.InitializeWizard();
            this.MoveToPanel(0);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if(this._currentPanelIndex > 0)
                this.MoveToPanel(this._currentPanelIndex - 1);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if(this._currentPanelIndex < this._panels.Count - 1)
            {
                if(this.IsValidPanelIndex(this._currentPanelIndex))
                {
                    if(!this._panels[this._currentPanelIndex].CanNavigateToNextPanel)
                        return;
                }

                this.MoveToPanel(this._currentPanelIndex + 1);
            }
            else if(this._currentPanelIndex == this._panels.Count - 1)
                this.Finish();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }

    public interface IOrderResetPanel
    {
        /// <summary>
        ///   Handle back to the main form.
        /// </summary>
        OrderResetMain ResetMain { get; set; }
        /// <summary>
        ///   Get the method to call when the CanNavigateToNextPanel value changes.
        /// </summary>
        Action<IOrderResetPanel, bool> OnCanNavigateToNextPanelChange { get; set; }
        /// <summary>
        ///   Determines if the Next button is enabled
        /// </summary>
        bool CanNavigateToNextPanel { get; }

        /// <summary>
        ///   Called when navigating to this panel
        /// </summary>
        void OnNavigateTo();

        /// <summary>
        ///   Called before navigating away from this panel.
        /// </summary>
        void OnNavigateFrom();

        /// <summary>
        ///   Called when finish is clicked.
        /// </summary>
        void Finish();
    }
}