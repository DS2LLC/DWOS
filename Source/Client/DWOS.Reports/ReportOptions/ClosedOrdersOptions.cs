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
    public partial class ClosedOrdersOptions: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private bool _customerLoaded;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region Methods

        public ClosedOrdersOptions()
        {
            this.InitializeComponent();
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

                if (!this.chkAllCustomers.Checked && this.cboCustomer.SelectedItem == null)
                {
                    // Invalid options.
                    return;
                }

                if(this.dteFromDate.Value == null)
                    this.dteFromDate.Value = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
                if(this.dteToDate.Value == null)
                    this.dteToDate.DateTime = DateUtilities.GetLastDayOfMonth(DateTime.Now);

                if(this.chkAllCustomers.Checked)
                {
                    var report = new ClosedOrderReport();
                    report.FromDate = this.dteFromDate.DateTime;
                    report.ToDate = this.dteToDate.DateTime;
                    report.IncludeBoxedOrders = this.chkInculdeBoxed.Checked;
                    report.GroupOrdersBy = this.chkProductClass.Checked ? ClosedOrderReport.GroupBy.ProductClass : ClosedOrderReport.GroupBy.Customer;
                    await Task.Run(() => report.DisplayReport(_cancellationTokenSource.Token));
                }
                else
                {
                    var report = new ClosedOrdersByCustomerReport();
                    report.FromDate = this.dteFromDate.DateTime;
                    report.ToDate = this.dteToDate.DateTime;
                    report.CustomerID = Convert.ToInt32(this.cboCustomer.SelectedItem.DataValue);
                    Settings.Default.LastReportCustomerID = report.CustomerID;
                    Settings.Default.Save();
                    await Task.Run(() => report.DisplayReport(_cancellationTokenSource.Token));
                }
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
            dteFromDate.Enabled = false;
            dteToDate.Enabled = false;
            lblLastMonth.Enabled = false;
            lblToday.Enabled = false;
            lblYTD.Enabled = false;
            chkAllCustomers.Enabled = false;
            chkByCustomer.Enabled = false;
            chkInculdeBoxed.Enabled = false;
            chkProductClass.Enabled = false;
            cboCustomer.Enabled = false;
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
            dteToDate.Enabled = true;
            lblLastMonth.Enabled = true;
            lblToday.Enabled = true;
            lblYTD.Enabled = true;
            chkAllCustomers.Enabled = true;
            chkByCustomer.Enabled = true;
            chkInculdeBoxed.Enabled = true;
            chkProductClass.Enabled = true;
            cboCustomer.Enabled = true;
            btnOK.Enabled = true;

            this.activityIndicator.Visible = false;
            this.activityIndicator.Stop();
        }

        #endregion

        #region Events

        private void chkByCustomer_CheckedChanged(object sender, EventArgs e)
        {
            if(this.chkByCustomer.Checked)
                this.chkAllCustomers.Checked = false;

            this.cboCustomer.Enabled = this.chkByCustomer.Checked;

            if(this.cboCustomer.Enabled)
                this.LoadCustomers();

            if(!this.chkAllCustomers.Checked && !this.chkByCustomer.Checked)
                this.chkAllCustomers.Checked = true;
        }

        private void chkAllCustomers_CheckedChanged(object sender, EventArgs e)
        {
            if(this.chkAllCustomers.Checked)
                this.chkByCustomer.Checked = false;

            this.chkInculdeBoxed.Enabled = this.chkAllCustomers.Checked;

            if(!this.chkAllCustomers.Checked && !this.chkByCustomer.Checked)
            {
                this.chkAllCustomers.Checked = true;
                this.chkAllCustomers_CheckedChanged(null, null); //FYI: Setting the checked above does not recall this method
            }
        }

        private void ClosedOrdersOptions_Load(object sender, EventArgs e)
        {
            this.dteFromDate.DateTime = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            this.dteToDate.DateTime = DateUtilities.GetLastDayOfMonth(DateTime.Now);

            this.chkByCustomer.Checked = true;
            this.chkByCustomer.Checked = false;

            this.chkAllCustomers.Checked = true;

            chkProductClass.Enabled = FieldUtilities.IsFieldEnabled("Order", "Product Class");
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

                this.dteFromDate.DateTime = DateUtilities.GetFirstDayOfMonth(lastMonth);
                this.dteToDate.DateTime = DateUtilities.GetLastDayOfMonth(lastMonth);
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