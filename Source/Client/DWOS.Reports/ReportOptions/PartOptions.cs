using DWOS.Data;
using DWOS.Reports.Properties;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DWOS.Reports.ReportOptions
{
    /// <summary>
    /// UI for generating the Part History report.
    /// </summary>
    public partial class PartOptions : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region Properties

        public IPartReport Report
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="PartOptions"/> class.
        /// </summary>
        public PartOptions()
        {
            InitializeComponent();

            dteFromDate.DateTime = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            dteToDate.DateTime = DateUtilities.GetLastDayOfMonth(DateTime.Now);

            var customers = dsCustomers.Customer;

            taCustomer.Fill(customers);
            cboCustomer.DataSource = customers.DefaultView;
            cboCustomer.DisplayMember = customers.NameColumn.ToString();
            cboCustomer.ValueMember = customers.CustomerIDColumn.ToString();


            int lastReportCustomerID = Settings.Default.LastReportCustomerID;

            if (lastReportCustomerID > 0)
            {
                var customerItem = cboCustomer.Items.ValueList.FindByDataValue(lastReportCustomerID);

                if (customerItem != null)
                {
                    cboCustomer.SelectedItem = customerItem;
                }
            }

            if (cboCustomer.SelectedIndex < 0)
            {
                cboCustomer.SelectedIndex = 1;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartOptions"/> class.
        /// </summary>
        /// <param name="report">Report to run.</param>
        public PartOptions(IPartReport report)
            : this()
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            Report = report;
            lblTitle.Text = string.Format("{0} Report", Report.Title);
        }

        /// <summary>
        /// Runs & shows the report
        /// </summary>
        /// <returns>true if the report ran successfully; otherwise, false</returns>
        private async Task<bool> RunReport()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();

                if (cboCustomer.Value == null)
                {
                    return false;
                }

                DateTime fromDate;
                if (dteFromDate.Value == null)
                {
                    fromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
                }
                else
                {
                    fromDate = dteFromDate.DateTime;
                }

                DateTime toDate;
                if (dteToDate.Value == null)
                {
                    toDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);
                }
                else
                {
                    toDate = dteToDate.DateTime;
                }

                var customerID = ((int?)cboCustomer.Value);
                var partID = (int?)cboPart.Value;

                if (customerID.HasValue && partID.HasValue && Report != null)
                {
                    Report.ToDate = toDate;
                    Report.FromDate = fromDate;
                    Report.CustomerID = customerID.Value;
                    Report.PartID = partID.Value;

                    await Task.Run(() => Report.DisplayReport(_cancellationTokenSource.Token));
                    return !_cancellationTokenSource.IsCancellationRequested;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error running report.");
                return false;
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        #endregion

        #region Events

        private void cboCustomer_ValueChanged(object sender, EventArgs e)
        {
            cboPart.SelectedIndex = -1;
            if (cboCustomer.SelectedIndex > 0)
            {
                // Get customer ID
                var selectedCustomerID = (int?)(cboCustomer.Value);

                if (selectedCustomerID.HasValue)
                {
                    var parts = dsParts.Part;
                    taPart.FillByCustomer(parts, selectedCustomerID.Value, true);

                    cboPart.DataSource = parts.DefaultView;
                    cboPart.DisplayMember = parts.NameColumn.ToString();
                    cboPart.ValueMember = parts.PartIDColumn.ToString();

                    if (cboPart.Items.Count > 0)
                    {
                        cboPart.SelectedIndex = 0;
                    }
                }
            }
        }

        private async void btnOK_Click(object sender, EventArgs e)
        {
            Enabled = false;
            this.activityIndicator.Visible = true;
            this.activityIndicator.Start();

            bool ranReport = await RunReport();
            if (ranReport)
            {
                int? customerID = (int?)cboCustomer.Value;

                if (customerID.HasValue)
                {
                    Settings.Default.LastReportCustomerID = customerID.Value;
                    Settings.Default.Save();
                }

            }

            Enabled = true;
            this.activityIndicator.Visible = false;
            this.activityIndicator.Stop();

            if (ranReport)
            {
                Close();
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
                _log.Error(exc, "Error setting dates to today.");
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
                _log.Error(exc, "Error setting dates to last month.");
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
                _log.Error(exc, "Error setting dates to YTD.");
            }
        }

        #endregion
    }
}
