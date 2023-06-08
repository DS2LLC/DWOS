using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Reports.Properties;
using DWOS.Shared;
using NLog;

namespace DWOS.Reports.ReportOptions
{
    public partial class TimeTrackingOptions : Form
    {

        #region Fields

        private bool _customerLoaded;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region Methods

        public TimeTrackingOptions()
        {
            InitializeComponent();
        }

        private void LoadCustomers()
        {
            try
            {
                if(_customerLoaded)
                    return;

                var customers = new CustomersDataset.CustomerDataTable();
                using(var taCustomers = new CustomerTableAdapter())
                {
                    taCustomers.Fill(customers);
                }

                cboCustomer.DataSource = customers.DefaultView;
                cboCustomer.DisplayMember = customers.NameColumn.ToString();
                cboCustomer.ValueMember = customers.CustomerIDColumn.ToString();

                int customerID = Settings.Default.LastReportCustomerID;

                if(customerID > 0)
                {
                    var item = cboCustomer.Items.ValueList.FindByDataValue(customerID);
                    if(item != null)
                        cboCustomer.SelectedItem = item;
                }

                if(cboCustomer.SelectedIndex < 1)
                    cboCustomer.SelectedIndex = 0;

                _customerLoaded = true;
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading customers.");
            }
        }

        private async Task RunReport()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();

                StartRunningReport();

                if (!chkAllCustomers.Checked && cboCustomer.SelectedItem == null)
                {
                    // Invalid options.
                    return;
                }

                if(dteFromDate.Value == null)
                    dteFromDate.Value = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
                if(dteToDate.Value == null)
                    dteToDate.DateTime = DateUtilities.GetLastDayOfMonth(DateTime.Now);
                
                var report = new TimeTrackingDetailReport();
                report.FromDate = dteFromDate.DateTime;
                report.ToDate = dteToDate.DateTime;

                if (!chkAllCustomers.Checked)
                {
                    report.CustomerID = Convert.ToInt32(cboCustomer.SelectedItem.DataValue);
                }

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
            dteFromDate.Enabled = false;
            dteToDate.Enabled = false;
            chkAllCustomers.Enabled = false;
            chkByCustomer.Enabled = false;
            cboCustomer.Enabled = false;
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
            chkAllCustomers.Enabled = true;
            chkByCustomer.Enabled = true;
            cboCustomer.Enabled = true;
            btnOK.Enabled = true;

            activityIndicator.Visible = false;
            activityIndicator.Stop();
        }

        #endregion

        #region Events

        private void chkByCustomer_CheckedChanged(object sender, EventArgs e)
        {
            if(chkByCustomer.Checked)
                chkAllCustomers.Checked = false;

            cboCustomer.Enabled = chkByCustomer.Checked;

            if(cboCustomer.Enabled)
                LoadCustomers();

            if(!chkAllCustomers.Checked && !chkByCustomer.Checked)
                chkAllCustomers.Checked = true;
        }

        private void chkAllCustomers_CheckedChanged(object sender, EventArgs e)
        {
            if(chkAllCustomers.Checked)
                chkByCustomer.Checked = false;

            if(!chkAllCustomers.Checked && !chkByCustomer.Checked)
            {
                chkAllCustomers.Checked = true;
                chkAllCustomers_CheckedChanged(null, null); //FYI: Setting the checked above does not recall this method
            }
        }

        private void TimeDataOptions_Load(object sender, EventArgs e)
        {
            dteFromDate.DateTime = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            dteToDate.DateTime = DateUtilities.GetLastDayOfMonth(DateTime.Now);

            chkByCustomer.Checked = true;
            chkByCustomer.Checked = false;

            chkAllCustomers.Checked = true;
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
            catch(Exception exc)
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
            catch(Exception exc)
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

        #endregion
    }
}
