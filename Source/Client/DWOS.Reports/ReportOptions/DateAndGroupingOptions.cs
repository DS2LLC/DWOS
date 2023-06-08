using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Shared;
using NLog;

namespace DWOS.Reports.ReportOptions
{
    public partial class DateAndGroupingOptions : Form
    {
        #region Fields

        private Report _report; 
        private Action<DateTime> _setFromDate;
        private Action<DateTime> _setToDate;
        private Action<ShipRecReport.enumGroupBy> _setShipRecGrouping;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region Methods

        public DateAndGroupingOptions()
        {
            InitializeComponent();
        }

        public void LoadReport(string name, Report report, Action<DateTime> setFromDate, Action<DateTime> setToDate, Action<ShipRecReport.enumGroupBy> setShipRecGrouping)
        {
            this.lblName.Text = name;
            this._report = report;
            this._setFromDate = setFromDate;
            this._setToDate = setToDate;

            if (_report is ShipRecReport)
                this._setShipRecGrouping = setShipRecGrouping;


            cboGroupBy.DataSource = null;
            if (_report is ShipRecReport)
            {
                cboGroupBy.DataSource = Enum.GetValues(typeof(ShipRecReport.enumGroupBy));
                cboGroupBy.SelectedIndex = 0;
            }
            else if (_report is RevenueByProgramReport)
            {
                cboGroupBy.DataSource = Enum.GetValues(typeof(RevenueByProgramReport.enumGroupBy));
                cboGroupBy.SelectedIndex = 1;
            }
        }

        private async Task RunReport()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();

                this.StartRunningReport();

                if (this.dteReportFromDate.Value == null)
                    this.dteReportFromDate.Value = DateTime.Now;

                if (this.dteReportToDate.Value == null)
                    this.dteReportToDate.Value = DateTime.Now;

                this._setFromDate(this.dteReportFromDate.DateTime);
                this._setToDate(this.dteReportToDate.DateTime);

                if (_report is ShipRecReport)
                    this._setShipRecGrouping((ShipRecReport.enumGroupBy)this.cboGroupBy.SelectedItem.DataValue);
                
                await Task.Run(() => _report.DisplayReport(_cancellationTokenSource.Token));
            }
            finally
            {
                this.StopRunningReport();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private void StartRunningReport()
        {
            ControlBox = false;
            dteReportFromDate.Enabled = false;
            dteReportToDate.Enabled = false;
            cboGroupBy.Enabled = false;
            lblToday.Enabled = false;
            lblLastMonth.Enabled = false;
            lblYTD.Enabled = false;
            btnOK.Enabled = false;

            this.activityIndicator.Visible = true;
            this.activityIndicator.Start();
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
        }

        private void StopRunningReport()
        {
            if (IsDisposed)
            {
                return;
            }

            ControlBox = true;
            dteReportFromDate.Enabled = false;
            dteReportToDate.Enabled = false;
            cboGroupBy.Enabled = false;
            lblToday.Enabled = false;
            lblLastMonth.Enabled = false;
            lblYTD.Enabled = false;
            btnOK.Enabled = false;

            this.activityIndicator.Visible = false;
            this.activityIndicator.Stop();
            this.Cursor = Cursors.Default;
            Application.DoEvents();
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
                this.dteReportFromDate.DateTime = DateTime.Now;
                this.dteReportToDate.DateTime = DateTime.Now;
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

                this.dteReportFromDate.DateTime = DateUtilities.GetFirstDayOfMonth(lastMonth);
                this.dteReportToDate.DateTime = DateUtilities.GetLastDayOfMonth(lastMonth);
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
                this.dteReportFromDate.DateTime = DateUtilities.GetFirstDayOfYear(DateTime.Now);
                this.dteReportToDate.DateTime = DateTime.Now;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting date to YTD.");
            }
        }

        #endregion
    }
}
