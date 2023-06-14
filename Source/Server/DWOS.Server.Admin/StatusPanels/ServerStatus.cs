using System;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Server.Admin.LicenseActivation;
using DWOS.Server.Admin.Properties;
using DWOS.Server.Admin.Tasks;
using DWOS.Shared;
using NLog;
using System.Net;
using System.Collections.Generic;

namespace DWOS.Server.Admin.StatusPanels
{
    /// <summary>
    /// Shows server service information.
    /// </summary>
    public partial class ServerStatus: UserControl
    {
        #region Fields
        
        private Version _currentVersion;
        private Version _latestVersion;
        private readonly List<string> _knownIpAddresses = new List<string>();
        
        #endregion

        #region Methods

        public ServerStatus()
        {
            this.InitializeComponent();
        }
        
        private void GetCurrentVersion()
        {
            //This is server version; NOT DB Version
            txtVersion.Text = About.ApplicationVersion;

            //Ensure the client version is >= current version; required for upgrades to prevent the client version from being < then the current version
            Version clientVersion;

            if (ServerSettings.Default.MinimumClientVersion != null && Version.TryParse(ServerSettings.Default.MinimumClientVersion, out clientVersion))
            {
                var serverVersion = new Version(About.ApplicationVersion);

                //ensure the current server version and the min client version have same Major Minor Build numbers, if not then reset min client version to server version.
                if (!serverVersion.CompareMajorMinorBuild(clientVersion))
                {
                    clientVersion = serverVersion;
                    
                    //update the registry settings
                    ServerSettings.Default.MinimumClientVersion = serverVersion.ToString();
                    ServerSettings.Default.Save();

                    //force server to reload info
                    Main.ForceServerToReload();
                }

                txtMinClientVersion.Text = clientVersion.ToString();
            }
            else
                txtMinClientVersion.Text = null;
        }

        private void UpdateStatus(bool isRunning)
        {
            this.picActivate.Image = isRunning ? Resources.Check32 : Resources.Question64;
            this.txtStatus.Text    = isRunning ? "Complete" : "Incomplete";
        }

        /// <summary>
        /// Begins updating status asynchronously.
        /// </summary>
        public void BeginUpdateStatus()
        {
            this.BeginGetServerStatus();
            this.BeginGetLatestVersion();
        }

        private void BeginGetLatestVersion()
        {
            try
            {
                this._currentVersion = new Version(About.ApplicationVersion);

                var client = new ActivationServiceClient();
                client.GetLatestVersionCompleted += (s, e) =>
                                                    {
                                                        if(e.Error == null)
                                                            SetLatestVersion(new Version(e.Result.Version));
                                                    };

                var rg = ReleaseGroup.Normal;
                Enum.TryParse(Settings.Default.ReleaseGroup, out rg);
                client.GetLatestVersionAsync(Settings.Default.ProductID, rg);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting latest version.");
            }
        }

        private void BeginGetServerStatus()
        {
            try
            {
                ThreadPool.QueueUserWorkItem(cb =>
                {
                    var controller = ServiceController.GetServices().FirstOrDefault(sc => sc.ServiceName == "DWOSServer");
                        SetServerStatus(controller == null ? (ServiceControllerStatus?)null : controller.Status);
                });
            }
            catch(Exception exc)
            {
                this.UpdateStatus(false);
                this.txtState.Text = "------------";
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting current server status.");
            }
        }

        private void SetLatestVersion(Version latestVersion)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action <Version>(SetLatestVersion), latestVersion);
            }
            else
            {
                this._latestVersion = latestVersion;
                this.txtLatestVersion.Text = this._latestVersion.ToString();
                this.txtLatestVersion.Appearance.ForeColor = this._currentVersion < this._latestVersion ? Color.Red : Color.Black;
            }
        }

        private void SetServerStatus(ServiceControllerStatus? status)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<ServiceControllerStatus?>(SetServerStatus), status);
            }
            else
            {
                var stateText = "Unknown";
                switch (status)
                {
                    case null:
                        stateText = "Not Installed";
                        break;
                    case ServiceControllerStatus.ContinuePending:
                        stateText = "Continue Pending";
                        break;
                    case ServiceControllerStatus.Paused:
                        stateText = "Paused";
                        break;
                    case ServiceControllerStatus.PausePending:
                        stateText = "Pause Pending";
                        break;
                    case ServiceControllerStatus.Running:
                        stateText = "Running";
                        break;
                    case ServiceControllerStatus.StartPending:
                        stateText = "Start Pending";
                        break;
                    case ServiceControllerStatus.Stopped:
                        stateText = "Stopped";
                        break;
                    case ServiceControllerStatus.StopPending:
                        stateText = "Stop Pending";
                        break;
                }

                this.txtState.Text = stateText;
                this.UpdateStatus(status == ServiceControllerStatus.Running);
            }
        }

        #endregion

        #region Events
        
        private void btnActivate_Click(object sender, EventArgs e)
        {
            using(var frm = new ServerConfiguration())
            {
                frm.ShowDialog(this);
            }

            BeginUpdateStatus();
        }

        private void LicenseStatus_Load(object sender, EventArgs e)
        {
            if(DesignMode)
                return;

            this.UpdateStatus(false);
            this.GetCurrentVersion();

            ThreadPool.QueueUserWorkItem(cb =>
                                         {
                                             Thread.Sleep(500);
                                             BeginUpdateStatus();
                                         });

            // Server name & IP Address
            string hostname = Dns.GetHostName();


            _knownIpAddresses.Clear();
            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    _knownIpAddresses.Add(ip.ToString());
                }
            }

            var ipAddress = _knownIpAddresses.Count == 1
                ? _knownIpAddresses.First()
                : $"{_knownIpAddresses.Count} IP Addr.";

            txtServerAddress.Text = hostname;
            txtServerIp.Text = ipAddress;

            timerUpdate.Start();
        }

        private void txtMinClientVersion_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            Version versionOut;

            if(Version.TryParse(txtMinClientVersion.Text, out versionOut))
            {
                if(_latestVersion != null && versionOut > _latestVersion)
                {
                    if (MessageBox.Show("Minimum Version {0} is is greater than the latest version {1}. Are you sure you want to set {0} as the minimum version?".FormatWith(versionOut, _latestVersion), About.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        return;
                }

                ServerSettings.Default.MinimumClientVersion = versionOut.ToString();
                ServerSettings.Default.Save();

                //force server to reload info
                Main.ForceServerToReload();
            }

            else
                MessageBox.Show("Version is not in the correct format [X.X.X.X].", About.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void txtServerIp_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                using (var dialog = new IpAddressesDialog())
                {
                    dialog.LoadIpAddresses(_knownIpAddresses);
                    dialog.ShowDialog();
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Cannot show Server IP dialog.");
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            try
            {
                BeginUpdateStatus();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error with automatically updating service status.");
            }
        }

        #endregion

    }
}