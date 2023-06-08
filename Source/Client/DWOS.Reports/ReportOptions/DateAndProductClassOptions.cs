using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Reports.ProcessPartsReportTableAdapters;
using DWOS.Shared;
using Infragistics.Win;
using NLog;

namespace DWOS.Reports.ReportOptions
{
    public partial class DateAndProductClassOptions : Form
    {
        #region Fields

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();
        private Func<ReportData, Report> _createReport;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region Methods

        public DateAndProductClassOptions()
        {
            InitializeComponent();
        }

        public void LoadReportInfo(string name, Func<ReportData, Report> createReport)
        {
            lblName.Text = name;
            _createReport = createReport;

            cboProductClass.Items.Clear();

            ValueListItem defaultItem = null;
            using (var ta = new ProductClassTableAdapter())
            {
                using (var dtProductClass = ta.GetData())
                {
                    foreach (var productClass in dtProductClass.Where(pc => !pc.IsProductClassNull()))
                    {
                        var valueItem = cboProductClass.Items.Add(productClass.ProductClass);

                        if (productClass.ProductClass == Properties.Settings.Default.LastReportProductClass)
                        {
                            defaultItem = valueItem;
                        }
                    }
                }
            }

            // If there are items
            if (cboProductClass.Items.Count > 0)
            {
                if (defaultItem == null)
                {
                    // Select first
                    cboProductClass.SelectedIndex = 0;
                }
                else
                {
                    cboProductClass.SelectedItem = defaultItem;
                }
            }
            else
            {
                cboProductClass.SelectedIndex = -1;
            }
        }

        private async Task RunReport()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();

                StartRunningReport();

                var productClass = string.Empty;

                if (chkProductClass.Checked)
                {
                    productClass = cboProductClass.Text;
                }

                if (dteFromDate.Value == null)
                {
                    dteFromDate.Value = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
                }

                if (dteToDate.Value == null)
                {
                    dteToDate.DateTime = DateUtilities.GetLastDayOfMonth(DateTime.Now);
                }

                Properties.Settings.Default.LastReportProductClass = productClass;
                Properties.Settings.Default.Save();

                var reportData = new ReportData(dteFromDate.DateTime, dteToDate.DateTime, productClass);

                using (var report = _createReport?.Invoke(reportData))
                {
                    await Task.Run(() => report?.DisplayReport(_cancellationTokenSource.Token));
                }
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
            dteToDate.Enabled = false;
            lblLastMonth.Enabled = false;
            lblToday.Enabled = false;
            lblYTD.Enabled = false;
            chkAllProductClass.Enabled = false;
            chkProductClass.Enabled = false;
            cboProductClass.Enabled = false;
            btnOK.Enabled = false;

            activityIndicator.Visible = true;
            activityIndicator.Start();
        }

        private void StopRunningReport()
        {
            if (IsDisposed)
            {
                return;
            }

            ControlBox = true;
            dteFromDate.Enabled = true;
            dteToDate.Enabled = true;
            lblLastMonth.Enabled = true;
            lblToday.Enabled = true;
            lblYTD.Enabled = true;
            chkAllProductClass.Enabled = true;
            chkProductClass.Enabled = true;
            cboProductClass.Enabled = true;
            btnOK.Enabled = true;

            activityIndicator.Visible = false;
            activityIndicator.Stop();
        }

        #endregion

        #region Events

        private void ClosedOrdersOptions_Load(object sender, EventArgs e)
        {
            dteFromDate.DateTime = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            dteToDate.DateTime = DateUtilities.GetLastDayOfMonth(DateTime.Now);
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
                dteFromDate.DateTime = DateTime.Now;
                dteToDate.DateTime = DateTime.Now;
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

                dteFromDate.DateTime = DateUtilities.GetFirstDayOfMonth(lastMonth);
                dteToDate.DateTime = DateUtilities.GetLastDayOfMonth(lastMonth);
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
                dteFromDate.DateTime = DateUtilities.GetFirstDayOfYear(DateTime.Now);
                dteToDate.DateTime = DateTime.Now;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting date to YTD.");
            }
        }

        private void chkProductClass_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var singleProductClassSelected = chkProductClass.Checked;
                chkAllProductClass.Checked = !singleProductClassSelected;
                cboProductClass.Enabled = singleProductClassSelected;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating product class fields.");
            }
        }

        private void chkAllProductClass_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var allProductClassSelected = chkAllProductClass.Checked;
                chkProductClass.Checked = !allProductClassSelected;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating product class fields.");
            }
        }

        #endregion

        #region ReportData

        public class ReportData
        {
            public DateTime FromDate { get; }

            public DateTime ToDate { get; }

            public string ProductClass { get; }

            public ReportData(DateTime fromDate, DateTime toDate, string productClass)
            {
                FromDate = fromDate;
                ToDate = toDate;
                ProductClass = productClass;
            }
        }

        #endregion
    }
}