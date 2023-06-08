using System;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Reports.Properties;
using NLog;

namespace DWOS.Reports.ReportOptions
{
    public partial class OrderHistoryOptions: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private bool _customerLoaded;

        #endregion

        #region Methods

        public OrderHistoryOptions()
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

                this.cboType.DataSource = customers.DefaultView;
                this.cboType.DisplayMember = customers.NameColumn.ToString();
                this.cboType.ValueMember = customers.CustomerIDColumn.ToString();

                int customerID = Settings.Default.LastReportCustomerID;

                if(customerID > 0)
                {
                    var item = this.cboType.Items.ValueList.FindByDataValue(customerID);
                    if(item != null)
                        this.cboType.SelectedItem = item;
                }

                if(this.cboType.SelectedIndex < 1)
                    this.cboType.SelectedIndex = 0;

                this._customerLoaded = true;
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error loading customers.");
            }
        }

        private bool RunReport()
        {
            try
            {
                this.StartRunningReport();

                if(this.dteFromDate.Value == null)
                    this.dteFromDate.Value = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
                if(this.dteToDate.Value == null)
                    this.dteToDate.DateTime = DateUtilities.GetLastDayOfMonth(DateTime.Now);

                if(this.chkDateCreated.Checked)
                {
                    var report = new ClosedOrderReport();
                    report.FromDate = this.dteFromDate.DateTime;
                    report.ToDate = this.dteToDate.DateTime;
                    report.IncludeBoxedOrders = this.chkDateCompleted.Checked;
                    report.DisplayReport();
                }
                else
                {
                    var report = new ClosedOrdersByCustomerReport();
                    report.FromDate = this.dteFromDate.DateTime;
                    report.ToDate = this.dteToDate.DateTime;
                    report.CustomerID = Convert.ToInt32(this.cboType.SelectedItem.DataValue);
                    report.DisplayReport();
                }

                return true;
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error running report.");
                return false;
            }
            finally
            {
                this.StopRunningReport();
            }
        }

        private void StartRunningReport()
        {
            Enabled = false;
            this.activityIndicator.Visible = true;
            this.activityIndicator.Start();
        }

        private void StopRunningReport()
        {
            Enabled = true;
            this.activityIndicator.Visible = false;
            this.activityIndicator.Stop();
        }

        #endregion

        #region Events

        private void chkByCustomer_CheckedChanged(object sender, EventArgs e)
        {
            if(this.chkChangeType.Checked)
                this.chkDateCreated.Checked = false;

            this.cboType.Enabled = this.chkChangeType.Checked;

            if(this.cboType.Enabled)
                this.LoadCustomers();

            if(!this.chkDateCreated.Checked && !this.chkChangeType.Checked)
                this.chkDateCreated.Checked = true;
        }

        private void chkAllCustomers_CheckedChanged(object sender, EventArgs e)
        {
            if(this.chkDateCreated.Checked)
                this.chkChangeType.Checked = false;

            this.chkDateCompleted.Enabled = this.chkDateCreated.Checked;

            if(!this.chkDateCreated.Checked && !this.chkChangeType.Checked)
            {
                this.chkDateCreated.Checked = true;
                this.chkAllCustomers_CheckedChanged(null, null); //FYI: Setting the checked above does not recall this method
            }
        }

        private void ClosedOrdersOptions_Load(object sender, EventArgs e)
        {
            this.dteFromDate.DateTime = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            this.dteToDate.DateTime = DateUtilities.GetLastDayOfMonth(DateTime.Now);

            this.chkChangeType.Checked = true;
            this.chkChangeType.Checked = false;

            this.chkDateCreated.Checked = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(this.RunReport())
                Close();
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