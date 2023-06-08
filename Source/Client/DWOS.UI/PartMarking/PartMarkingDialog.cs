using System;
using System.Windows.Forms;
using NLog;

namespace DWOS.UI.PartMarking
{
    public partial class PartMarkingDialog : Form
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();
        private bool _isPartMarkSuccessful;

        public PartMarkingDialog() { InitializeComponent(); }

        public void LoadData(int orderID) { this.dpPartMarkingProcessing.SelectedOrder = orderID; }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                this.dpPartMarkingProcessing.SendMessagesToPartMarker();
                this.dpPartMarkingProcessing.SaveData();
                _isPartMarkSuccessful = true;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving part marking changes.";
                Log.Error(exc, errorMsg);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PartMarking_FormClosing(object sender, FormClosingEventArgs e)
        {
            dpPartMarkingProcessing.CloseConnection();

            DialogResult = _isPartMarkSuccessful
                ? DialogResult.OK
                : DialogResult.Cancel;

        }

        private void PartMarkingDialog_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.D)
            {
                dpPartMarkingProcessing.ToggleDebugMode();
            }
        }
    }
}