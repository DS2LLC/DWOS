using System;
using System.Windows.Forms;
using DWOS.Data;
using NLog;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Reports.Properties;
using DWOS.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.Reports.ReportOptions
{
    public partial class DateAndCustomerOptions : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private bool _customerLoaded;
        private Report _report;
        private Action<DateTime> _setFromDate;
        private Action<DateTime> _setToDate;
        private Action<int> _setCustomer;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region Properties

        public DateTime DefaultFromDate
        {
            get;
            set;
        }

        public DateTime DefaultToDate
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public DateAndCustomerOptions()
        {
            InitializeComponent();
            DefaultFromDate = DateTime.Now.StartOfDay();
            DefaultToDate = DateTime.Now.EndOfDay();
        }

        public void LoadReport(string name, Report report, Action<DateTime> setFromDate, Action<DateTime> setToDate, Action<int> setCustomer)
        {
            this.lblName.Text = name;
            this._report = report;
            this._setFromDate = setFromDate;
            this._setToDate = setToDate;
            this._setCustomer = setCustomer;
        }

        private void LoadCustomers()
        {
            try
            {
                if(this._customerLoaded)
                    return;

                var customers = new CustomersDataset.CustomerDataTable();
                using(var taCustomers = new CustomerTableAdapter())
                {
                    taCustomers.Fill(customers);
                }

                this.cboCustomer.DataSource = customers.DefaultView;
                this.cboCustomer.DisplayMember = customers.NameColumn.ToString();
                this.cboCustomer.ValueMember = customers.CustomerIDColumn.ToString();

                int customerID = Settings.Default.LastReportCustomerID;

                if(customerID > 0)
                {
                    var item = this.cboCustomer.Items.ValueList.FindByDataValue(customerID);
                    if(item != null)
                        this.cboCustomer.SelectedItem = item;
                }

                if(this.cboCustomer.SelectedIndex < 1)
                    this.cboCustomer.SelectedIndex = 0;

                this._customerLoaded = true;
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error loading customers.");
            }
        }

        private async Task RunReport()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                this.StartRunningReport();

                if (this.chkByCustomer.Checked && this.cboCustomer.SelectedItem == null)
                {
                    // Invalid options
                    return;
                }

                if (this.dteReportFromDate.Value == null)
                {
                    this.dteReportFromDate.Value = DefaultFromDate;
                }

                if (this.dteReportToDate.Value == null)
                {
                    this.dteReportToDate.Value = DefaultToDate;
                }

                this._setFromDate(this.dteReportFromDate.DateTime);
                this._setToDate(this.dteReportToDate.DateTime);
                this._setCustomer(this.chkByCustomer.Checked ? Convert.ToInt32(this.cboCustomer.SelectedItem.DataValue) : -1);
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
            lblLastMonth.Enabled = false;
            lblToday.Enabled = false;
            lblYTD.Enabled = false;
            chkAllCustomers.Enabled = false;
            chkByCustomer.Enabled = false;
            cboCustomer.Enabled = false;
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
            dteReportFromDate.Enabled = true;
            dteReportToDate.Enabled = true;
            lblLastMonth.Enabled = true;
            lblToday.Enabled = true;
            lblYTD.Enabled = true;
            chkAllCustomers.Enabled = true;
            chkByCustomer.Enabled = true;
            cboCustomer.Enabled = true;
            btnOK.Enabled = false;

            this.activityIndicator.Visible = false;
            this.activityIndicator.Stop();
            this.Cursor = Cursors.Default;
            Application.DoEvents();
        }

        #endregion

        #region Events

        private void CurrentOrderStatusByCustomer_Load(object sender, EventArgs e)
        {
            this.dteReportFromDate.DateTime = DefaultFromDate;
            this.dteReportToDate.DateTime = DefaultToDate;
            this.LoadCustomers();
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

        private void chkByCustomer_CheckChanged(object sender, EventArgs e)
        {
            if (this.chkByCustomer.Checked)
                this.chkAllCustomers.Checked = false;

            this.cboCustomer.Enabled = this.chkByCustomer.Checked;

            if (this.cboCustomer.Enabled)
                this.LoadCustomers();

            if (!this.chkAllCustomers.Checked && !this.chkByCustomer.Checked)
                this.chkAllCustomers.Checked = true;
        }

        private void chkAllCustomer_CheckChanged(object sender, EventArgs e)
        {
            if (this.chkAllCustomers.Checked)
                this.chkByCustomer.Checked = false;

            if (!this.chkAllCustomers.Checked && !this.chkByCustomer.Checked)
            {
                this.chkAllCustomers.Checked = true;
                this.chkAllCustomer_CheckChanged(null, null); //FYI: Setting the checked above does not recall this method
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
