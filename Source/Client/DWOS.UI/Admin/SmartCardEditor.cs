using System;
using System.Windows.Forms;
using DWOS.Shared;
using DWOS.SmartCardManager;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class SmartCardEditor: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private SmartCardManager.SmartCardManager _smartcardManager;

        #endregion

        #region Methods

        public SmartCardEditor()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Events

        private void btnWrite_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtOutput.Clear();

                if(this.numUserID.Value != null)
                {
                    int userID = 0;
                    if(int.TryParse(this.numUserID.Value.ToString(), out userID))
                        this._smartcardManager.WriteUserID(this.numUserID.Value.ToString());
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error writing user id to the smart card.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtOutput.Clear();

                string currentID = this._smartcardManager.ReadCurrentUserID();

                if(currentID != null)
                {
                    int userID = 0;
                    if(int.TryParse(currentID, out userID))
                    {
                        if(userID >= Convert.ToInt32(this.numUserID.MinValue) && userID <= Convert.ToInt32(this.numUserID.MaxValue))
                            this.numUserID.Value = userID;
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error reading user id from the smart card.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void SmartCardEditor_Load(object sender, EventArgs e)
        {
            this.numUserID.Value = 1000;
            this._smartcardManager = new SmartCardManager.SmartCardManager();
            this._smartcardManager.ActionOccured += this._smartcardManager_ActionOccured;
        }

        private void _smartcardManager_ActionOccured(object sender, ActionTextEventArgs e)
        {
            try
            {
                if(String.IsNullOrEmpty(e.ErrorText))
                    this.txtOutput.AppendText(e.ActionText);
                else
                {
                    this.txtOutput.AppendText(e.ActionText + " => ");
                    this.txtOutput.AppendText(e.ErrorText);
                }

                this.txtOutput.AppendText(Environment.NewLine);
            }
            catch(Exception exc)
            {
                string errorMsg = "Error writing text from smart card manager.";
                _log.Error(exc, errorMsg);
            }
        }

        #endregion
    }
}