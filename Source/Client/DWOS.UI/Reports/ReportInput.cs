using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using DWOS.Reports;
using DWOS.Shared;

namespace DWOS.UI.Reports
{
    public partial class ReportInput: Form
    {
        #region Fields

        private Report _report;

        #endregion

        #region Methods

        public ReportInput()
        {
            this.InitializeComponent();
        }

        internal void LoadReport(Report report)
        {
            this._report = report;

            this.prgReportInput.BrowsableAttributes = new AttributeCollection(new BrowsableAttribute(true));
            this.prgReportInput.SelectedObject = this._report;
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                Enabled = false;
                this.activitIndicator.AnimationEnabled = true;

                _report.DisplayReport();
                if(InvokeRequired)
                    BeginInvoke(new EventHandler(btnCancel_Click));
                else
                    Close();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error displaying report.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }
}