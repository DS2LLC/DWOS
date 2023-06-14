using System;
using System.Threading;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Server.Admin.LicenseActivation;
using DWOS.Server.Admin.Licensing;
using DWOS.Server.Admin.Properties;
using DWOS.Server.Admin.Tasks;
using NLog;

namespace DWOS.Server.Admin.StatusPanels
{
    /// <summary>
    /// Shows license status information.
    /// </summary>
    public partial class LicenseStatus: UserControl
    {
        #region Fields
        
        private ActivationServiceClient _client;

        #endregion

        #region Methods

        public LicenseStatus()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Updates status asynchronously.
        /// </summary>
        public void UpdateStatus()
        {
            this.GetLicenseActivationStatus();
            this.GetLicenseUsageStatus();
        }

        private void UpdateStatusUI(bool isConfigured)
        {
            this.picActivate.Image      = isConfigured ? Resources.Check32 : Resources.Question64;
            this.txtLicenseStatus.Text  = isConfigured ? "Complete" : "Incomplete";
            this.txtCustomerID.Text = ServerSettings.Default.CompanyKey ?? "------------";
            this.txtLicenseKey.Text = ServerSettings.Default.LicenseKey ?? "------------";
        }

        private void GetLicenseActivationStatus()
        {
            try
            {
                if(this._client == null)
                {
                    //Connect to DS2 Servers to get license info
                    this._client = new ActivationServiceClient();
                    this._client.GetLicenseInfoCompleted += this._client_GetLicenseInfoCompleted;
                }

                if(this.txtCustomerID.Text != null && this.txtLicenseKey.Text != null)
                    this._client.GetLicenseInfoAsync(ServerSettings.Default.CompanyKey, ServerSettings.Default.Fingerprint);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting current license info.");
            }
        }

        private void GetLicenseUsageStatus()
        {
            try
            {
                var client = new LicenseServiceClient();
                client.GetLicenseSummaryCompleted += this.client_GetLicenseStatusCompleted;
                client.Open();
                client.GetLicenseSummaryAsync();
            }
            catch (Exception exc)
            {
                this.lblLicenseUsage.Text = "Unable to connect.";
                ultraToolTipManager1.SetUltraToolTip(lblLicenseUsage, new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo() { ToolTipText = "Unable to connect to the DWOS Server, ensure it is running." });
                LogManager.GetCurrentClassLogger().Info(exc, "Error getting current license status.");
            }
        }
        
        #endregion

        #region Events

        private void _client_GetLicenseInfoCompleted(object sender, GetLicenseInfoCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && string.IsNullOrEmpty(e.Result.ErrorInformation))
                {
                    this.UpdateStatusUI(true);
                    this.txtCustomerID.Text        = ServerSettings.Default.CompanyKey + " (" + e.Result.CompanyName + ")";
                    this.txtLicenseCount.Text      = e.Result.Activations.ToString();
                    this.txtLicenseExpiration.Text = e.Result.LicenseExpiration.ToShortDateString();

                    //if expired
                    if(DateTime.Today > e.Result.LicenseExpiration)
                    {
                        this.txtLicenseExpiration.Appearance.ForeColor = System.Drawing.Color.Red;
                        this.txtLicenseExpiration.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    }
                    else
                    {
                        this.txtLicenseExpiration.Appearance.ResetForeColor();
                        this.txtLicenseExpiration.Appearance.ResetFontData();
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Info(exc, "Error get license info.");
            }
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            using(var frm = new LicenseConfiguration())
            {
                frm.ShowDialog(this);
            }

            this.UpdateStatusUI(false);
            this.GetLicenseActivationStatus();
            this.GetLicenseUsageStatus();

            //force dwos server to reload from registry
            Main.ForceServerToReload();
        }

        private void LicenseStatus_Load(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            this.UpdateStatusUI(false);

            ThreadPool.QueueUserWorkItem(cb =>
                                         {
                                             Thread.Sleep(500);
                                             BeginInvoke(new Action(UpdateStatus));
                                         });
        }

        private void client_GetLicenseStatusCompleted(object sender, GetLicenseSummaryCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                this.lblLicenseUsage.Text = e.Result.CurrentActivations.Count + " of " + e.Result.TotalActivations + " In Use";

                if (e.Result.CurrentActivations.Count > 0)
                {
                    var users = e.Result.CurrentActivations.ConvertAll<string>(a => a.UserName);
                    ultraToolTipManager1.SetUltraToolTip(lblLicenseUsage, new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo() { ToolTipText = String.Join(", ", users.ToArray()) });
                }
                else
                    ultraToolTipManager1.SetUltraToolTip(lblLicenseUsage, null);
            }
            else
            {
                LogManager.GetCurrentClassLogger().Error(e.Error, "Error getting current license status.");
                ultraToolTipManager1.SetUltraToolTip(lblLicenseUsage, null);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            GetLicenseUsageStatus();
        }

        #endregion
    }
}