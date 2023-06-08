using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Shared;
using NLog;

namespace DWOS.Reports.ReportOptions
{
    public partial class DateOptions : Form
    {
        #region Fields

        private Report _report;
        private Action <DateTime> _setFromDate;
        private Action <DateTime> _setToDate;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region Methods

        public DateOptions() { InitializeComponent(); }

        public void LoadReport(string name, Report report, Action <DateTime> setFromDate, Action <DateTime> setToDate)
        {
            this.lblName.Text = name;
            this._report = report;
            this._setFromDate = setFromDate;
            this._setToDate = setToDate;
        }

        private async Task RunReport()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                StartRunningReport();

                if(this.dteFromDate.Value == null)
                    this.dteFromDate.Value = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
                if(this.dteToDate.Value == null)
                    this.dteToDate.DateTime = DateUtilities.GetLastDayOfMonth(DateTime.Now);

                this._setFromDate(this.dteFromDate.DateTime);
                this._setToDate(this.dteToDate.DateTime);

                await Task.Run(() =>
                {
                    _report.DisplayReport(_cancellationTokenSource.Token);
                });
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
            dteFromDate.Enabled = false;
            dteFromDate.Enabled = false;
            lblLastMonth.Enabled = false;
            lblToday.Enabled = false;
            lblYTD.Enabled = false;
            btnOK.Enabled = false;

            this.activityIndicator.Visible = true;
            this.activityIndicator.Start();
        }

        private void StopRunningReport()
        {
            if (IsDisposed)
            {
                return;
            }

            ControlBox = true;
            dteFromDate.Enabled = true;
            dteFromDate.Enabled = true;
            lblLastMonth.Enabled = true;
            lblToday.Enabled = true;
            lblYTD.Enabled = true;
            btnOK.Enabled = true;

            this.activityIndicator.Visible = false;
            this.activityIndicator.Stop();
        }

        #endregion

        #region Events

        private void ClosedOrdersOptions_Load(object sender, EventArgs e)
        {
            this.dteFromDate.DateTime = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            this.dteToDate.DateTime = DateUtilities.GetLastDayOfMonth(DateTime.Now);
        }

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
                this.dteFromDate.DateTime = DateTime.Now;
                this.dteToDate.DateTime = DateTime.Now;
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

                this.dteFromDate.DateTime = DateUtilities.GetFirstDayOfMonth(lastMonth);
                this.dteToDate.DateTime = DateUtilities.GetLastDayOfMonth(lastMonth);
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
                this.dteFromDate.DateTime = DateUtilities.GetFirstDayOfYear(DateTime.Now);
                this.dteToDate.DateTime = DateTime.Now;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting date to YTD.");
            }
        }

        #endregion
    }
}