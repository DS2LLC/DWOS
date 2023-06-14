using System.Data.SqlClient;
using DWOS.Data;
using NLog;
using System;
using System.Windows.Forms;

namespace DWOS.Server.Admin.SettingsPanels
{
    /// <summary>
    /// Interface for editing general settings.
    /// </summary>
    public partial class SettingsAuthenticationInfo : UserControl, ISettingsPanel
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public string PanelKey
        {
            get { return "Authentication"; }
        }

        #endregion

        #region Methods

        public SettingsAuthenticationInfo()
        {
            InitializeComponent();
        }

        private void UpdateSeed(string tableName, string columnName, int currentSeeed)
        {
            try
            {
                using (var frm = new ChangeTableSeed())
                {
                    frm.LoadData(tableName, columnName, currentSeeed);
                    frm.ShowDialog(this);
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error updating seed.");
            }
        }

        private long GetSeed(string tableName, string columnName)
        {
            try
            {
                var serverConnection = new Microsoft.SqlServer.Management.Common.ServerConnection(new SqlConnection(ServerSettings.Default.DBConnectionString));
                serverConnection.Connect();

                var srv = new Microsoft.SqlServer.Management.Smo.Server(serverConnection);
                var db = srv.Databases[serverConnection.DatabaseName];
                var t = db.Tables[tableName];
                var results = t.CheckIdentityValue();

                if (results.Count > 0)
                {
                    var firstResponse = results[0];
                    var splitValues = firstResponse.Split(",");
                    if (splitValues.Length == 2)
                    {
                        var currentIdentity = splitValues[0].GetAfter("current identity value").Trim(' ', '\'');
                        int seedValue = 0;
                        if (int.TryParse(currentIdentity, out seedValue))
                            return seedValue;
                    }
                }

                return -1;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting seed for {0}".FormatWith(tableName));
                return -1;
            }
        }

        #endregion Methods

        #region ISettingsPanel Members

        public bool Editable
        {
            get { return true; } 
        }

        public bool IsValid
        {
            get
            {
                return string.IsNullOrEmpty(txtAdminEmail.Text) ||
                    txtAdminEmail.Text.IsValidEmail();
            }
        }

        public void LoadData()
        {
            try
            {
                Enabled = this.Editable;

                cboAuthType.SelectedIndex = (int)ApplicationSettings.Current.LoginType;
                numMinPin.Value = ApplicationSettings.Current.UserPinMinLength;
                chkNotificationsDisabled.Checked = ApplicationSettings.Current.DisableNotifications;
                txtCompanyURL.Text = ApplicationSettings.Current.CompanyUrl;
                txtPortalURL.Text = ApplicationSettings.Current.PortalUrl;
                
                txtWorkOrderSeed.Text = GetSeed("Order", "OrderID").ToString();
                txtSalesOrderSeed.Text = GetSeed("SalesOrder", "SalesOrderID").ToString();
                txtQuoteSeed.Text = GetSeed("Quote", "QuoteID").ToString();

                txtAdminEmail.Text = ServerSettings.Default.AdminEmail;

                numInterval.Value = ApplicationSettings.Current.ClientUpdateIntervalSeconds;

                chkShrinkDatabase.Checked = ServerSettings.Default.EnableNightlyDatabaseShrink;
            }
            catch (Exception exc)
            {
                const string errorMsg = "Error loading settings.";
                _log.Error(exc, errorMsg);
            }
        }

        public void SaveData()
        {
            ApplicationSettings.Current.LoginType = cboAuthType.SelectedItem != null ? (LoginType)Convert.ToInt32(cboAuthType.SelectedItem.DataValue) : LoginType.PinOrSmartcard;
            ApplicationSettings.Current.UserPinMinLength = Convert.ToInt32(numMinPin.Value);
            ApplicationSettings.Current.DisableNotifications = this.chkNotificationsDisabled.Checked;
            ApplicationSettings.Current.CompanyUrl = this.txtCompanyURL.Text;
            ApplicationSettings.Current.PortalUrl = this.txtPortalURL.Text;
            ApplicationSettings.Current.ClientUpdateIntervalSeconds = Convert.ToInt32(this.numInterval.Value);

            ServerSettings.Default.AdminEmail = txtAdminEmail.Text;
            ServerSettings.Default.EnableNightlyDatabaseShrink = chkShrinkDatabase.Checked;
            ServerSettings.Default.Save();
        }

        #endregion

        #region Events

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            var seedValue = 0;
            if (int.TryParse(txtQuoteSeed.Text, out seedValue))
            {
                UpdateSeed("Quote", "QuoteID", seedValue);
                txtQuoteSeed.Text = GetSeed("Quote", "QuoteID").ToString();
            }
        }

        private void btnSalesOrderSeed_Click(object sender, EventArgs e)
        {
            var seedValue = 0;
            if (int.TryParse(txtSalesOrderSeed.Text, out seedValue))
            {
                UpdateSeed("SalesOrder", "SalesOrderID", seedValue);
                txtSalesOrderSeed.Text = GetSeed("SalesOrder", "SalesOrderID").ToString();
            }
        }

        private void btnWorkOrderSeed_Click(object sender, EventArgs e)
        {
            var seedValue = 0;
            if (int.TryParse(txtWorkOrderSeed.Text, out seedValue))
            {
                UpdateSeed("Order", "OrderID", seedValue);
                txtWorkOrderSeed.Text = GetSeed("Order", "OrderID").ToString();
            }
        }

        private void txtAdminEmail_ValueChanged(object sender, EventArgs e)
        {
            bool txtAdminEmailValid = string.IsNullOrEmpty(txtAdminEmail.Text) ||
                txtAdminEmail.Text.IsValidEmail();

            if (txtAdminEmailValid)
            {
                errorProvider.SetError(txtAdminEmail, string.Empty);
            }
            else
            {
                errorProvider.SetError(txtAdminEmail, "Admin email must be a valid email address.");
            }
        }

        #endregion
    }
}
