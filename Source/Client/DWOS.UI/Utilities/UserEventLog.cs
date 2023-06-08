using System;
using System.Windows.Forms;
using DWOS.Data.Datasets.UserLoggingTableAdapters;
using DWOS.Shared;
using DWOS.UI.Utilities;

namespace DWOS.UI
{
    public partial class UserEventLog : Form
    {
        #region Properties

        public string Operation { get; set; }

        public int UserID { get; set; }

        public string UserName { get; set; }

        public string Form { get; set; }

        public string TransactionDetails { get; set; }

        #endregion

        #region Methods

        public UserEventLog() { InitializeComponent(); }

        private void SaveData()
        {
            UserEventLogTableAdapter ta = null;

            try
            {
                ta = new UserEventLogTableAdapter();
                ta.Insert(UserID, Operation, Form, this.txtReason.Text, TransactionDetails);
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving log data.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(this.txtReason.Text))
            {
                SaveData();
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBoxUtilities.ShowMessageBoxWarn("Please enter a reason for this transaction.", "User Event Required");
                this.txtReason.Focus();
            }
        }

        private void UserEventLog_Load(object sender, EventArgs e)
        {
            this.txtEvent.Text = Operation;
            this.txtUserName.Text = UserName;
        }

        #endregion
    }
}