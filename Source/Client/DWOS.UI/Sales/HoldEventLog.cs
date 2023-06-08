using System;
using System.Windows.Forms;
using DWOS.Data.Datasets.UserLoggingTableAdapters;
using DWOS.Shared;
using DWOS.UI.Utilities;

namespace DWOS.UI.Sales
{
    public partial class HoldEventLog: Form
    {
        #region Properties

        public string UserName { get; set; }

        public string Notes { get { return txtReason.Text; } }

        public int ReasonCode
        {
            get
            {
                if(cboHoldReason.SelectedItem != null)
                    return Convert.ToInt32(cboHoldReason.SelectedItem.DataValue);
                else
                    return -1;
            }
        }

        public bool PrintHoldLabel
        {
            get => chkPrintLabel.Checked;
            set => chkPrintLabel.Checked = value;
        }

        #endregion

        #region Methods

        public HoldEventLog()
        {
            this.InitializeComponent();
        }
        
        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(this.cboHoldReason.SelectedIndex >= 0)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBoxUtilities.ShowMessageBoxWarn("Please select a reason for placing this order on hold.", "Hold Reason Required");
                this.txtReason.Focus();
            }
        }

        private void UserEventLog_Load(object sender, EventArgs e)
        {
            this.txtUserName.Text = this.UserName;

            using(var ta = new Data.Datasets.UserLoggingTableAdapters.d_HoldReasonTableAdapter())
            {
                var holdReasons = ta.GetData();
                cboHoldReason.DataSource = holdReasons;
                cboHoldReason.DisplayMember = holdReasons.NameColumn.ColumnName;
                cboHoldReason.ValueMember = holdReasons.HoldReasonIDColumn.ColumnName;
            }
        }

        #endregion
    }
}