using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Shared;
using NLog;

namespace DWOS.Reports.ReportOptions
{
    public partial class RevenueByProgramOptions : Form
    {
        #region Fields

        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region Methods

        public RevenueByProgramOptions()
        {
            InitializeComponent();
            cboGroupBy.DataSource = null;
            cboGroupBy.DataSource = Enum.GetValues(typeof(RevenueByProgramReport.enumGroupBy));
            cboGroupBy.SelectedIndex = 1;

            cboType.Items.Add(RevenueByProgramReport.ProgramType.ModelAndManufacturer, "Model/Manufacturer");
            cboType.Items.Add(RevenueByProgramReport.ProgramType.ProductClass, "Product Class");
            cboType.SelectedIndex = 0;
        }

        private async Task RunReport()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                StartRunningReport();

                if (dteReportFromDate.Value == null)
                    dteReportFromDate.Value = DateTime.Now;

                if (dteReportToDate.Value == null)
                    dteReportToDate.Value = DateTime.Now;

                var report = new RevenueByProgramReport
                {
                    FromDate = dteReportFromDate.DateTime,
                    ToDate = dteReportToDate.DateTime,
                    GroupBy = (RevenueByProgramReport.enumGroupBy)cboGroupBy.SelectedItem.DataValue,
                    Program = (RevenueByProgramReport.ProgramType)cboType.SelectedItem.DataValue,
                };

                await Task.Run(() => report.DisplayReport(_cancellationTokenSource.Token));

            }
            finally
            {
                StopRunningReport();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private void StartRunningReport()
        {
            ControlBox = false;
            cboGroupBy.Enabled = false;
            cboType.Enabled = false;
            dteReportFromDate.Enabled = false;
            dteReportToDate.Enabled = false;
            lblLastMonth.Enabled = false;
            lblName.Enabled = false;
            lblYTD.Enabled = false;
            btnOK.Enabled = false;

            activityIndicator.Visible = true;
            activityIndicator.Start();
            Cursor = Cursors.WaitCursor;
            Application.DoEvents();
        }

        private void StopRunningReport()
        {
            if (IsDisposed)
            {
                return;
            }

            ControlBox = true;
            cboGroupBy.Enabled = true;
            cboType.Enabled = true;
            dteReportFromDate.Enabled = true;
            dteReportToDate.Enabled = true;
            lblLastMonth.Enabled = true;
            lblName.Enabled = true;
            lblYTD.Enabled = true;
            btnOK.Enabled = true;

            activityIndicator.Visible = false;
            activityIndicator.Stop();
        }

        #endregion Methods

        #region Events

        private async void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                await RunReport();
                Close();
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error running report", exc);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                DialogResult = DialogResult.Cancel;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc,
                    "Error canceling dialog.");
            }
        }

        private void lblToday_Click(object sender, EventArgs e)
        {
            try
            {
                dteReportFromDate.DateTime = DateTime.Now;
                dteReportToDate.DateTime = DateTime.Now;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting date to today.");
            }
        }

        private void lblLastMonth_Click(object sender, EventArgs e)
        {
            try
            {
                var lastMonth = DateTime.Now.AddMonths(-1);

                dteReportFromDate.DateTime = DateUtilities.GetFirstDayOfMonth(lastMonth);
                dteReportToDate.DateTime = DateUtilities.GetLastDayOfMonth(lastMonth);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting date to today.");
            }
        }

        private void lblYTD_Click(object sender, EventArgs e)
        {
            try
            {
                dteReportFromDate.DateTime = DateUtilities.GetFirstDayOfYear(DateTime.Now);
                dteReportToDate.DateTime = DateTime.Now;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting date to YTD.");
            }
        }

        private void RevenueByProgramOptions_Load(object sender, EventArgs e)
        {
            try
            {
                cboType.Enabled = FieldUtilities.IsFieldEnabled("Order", "Product Class");
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading revenue by program report options.");
            }
        }

        #endregion
    }
}
