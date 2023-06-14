using System;
using System.Threading;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Server.Admin.Properties;
using DWOS.Server.Admin.Tasks;

namespace DWOS.Server.Admin.StatusPanels
{
    /// <summary>
    /// Shows portal status information.
    /// </summary>
    public partial class WebStatus : UserControl
    {
        #region Fields

        private bool _isValidDbConnection;

        #endregion

        #region Methods

        public WebStatus()
        {
            this.InitializeComponent();
        }

        private void GetServerStatus()
        {
            try
            {
                //reset before testing
                _isValidDbConnection = false;
                SetDbConnectionStatus("Unsuccessful");
                UpdateStatus(false);

                var isServerValid = WebStatusHelpers.IsServerValid();

                _isValidDbConnection = isServerValid;
                SetDbConnectionStatus(_isValidDbConnection ? "Successful" : "Unsuccessful");
                UpdateStatus(_isValidDbConnection);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn(exc, "Error connecting to web server.");
            }
        }

        private void SetDbConnectionStatus(string status)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(SetDbConnectionStatus), status);
            }
            else
            {
                this.txtDBConnection.Text = status;
            }
        }

        private void UpdateStatus(bool isConfigured)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool>(UpdateStatus), isConfigured);
            }
            else
            {
                this.picActivate.Image = isConfigured ? Resources.Check32 : Resources.Question64;
                this.txtStatus.Text = isConfigured ? "Complete" : "Incomplete";
            }
        }

        #endregion

        #region Events

        private void LicenseStatus_Load(object sender, EventArgs e)
        {
            if(DesignMode)
                return;

            txtWebServerName.Text = String.IsNullOrWhiteSpace(ServerSettings.Default.CustomerPortalServer) ? "localhost" : ServerSettings.Default.CustomerPortalServer;
            txtWebsiteName.Text = ServerSettings.Default.CustomerPortalWebSite;

            ThreadPool.QueueUserWorkItem(cb =>
            {
                Thread.Sleep(500);
                GetServerStatus();
            });
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new WebServerConfiguration())
                {
                    if (frm.ShowDialog() == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                txtWebServerName.Text = String.IsNullOrWhiteSpace(ServerSettings.Default.CustomerPortalServer) ? "localhost" : ServerSettings.Default.CustomerPortalServer;
                txtWebsiteName.Text = ServerSettings.Default.CustomerPortalWebSite;

                ThreadPool.QueueUserWorkItem(cb =>
                {
                    Thread.Sleep(500);
                    GetServerStatus();

                    if (!_isValidDbConnection)
                    {
                        WebStatusHelpers.UpdatePortalConfiguration();
                        GetServerStatus();
                    }
                });
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading web server configuration.");
            }
        }

        private void btnRepair_Click(object sender, EventArgs e)
        {
            try
            {
                var result = WebStatusHelpers.UpdatePortalConfiguration();

                if (result.Success)
                {
                    MessageBox.Show(this, @"Successfully refreshed Portal settings.", @"DWOS Server Admin",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, $@"Failed to refresh Portal settings.{Environment.NewLine}{result.ErrorMessage}",
                        @"DWOS Server Admin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                GetServerStatus();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error repairing web server configuration.");
            }
        }

        #endregion
    }
}