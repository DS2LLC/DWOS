using DWOS.Data;
using DWOS.Shared;
using NLog;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace DWOS.Server.Admin.Tasks
{
    /// <summary>
    /// Shows server configuration options (saved to
    /// <see cref="ServerSettings"/>) and actions.
    /// </summary>
    public partial class ServerConfiguration: Form
    {
        #region Fields

        public const string SERVICE_NAME = "DWOSServer";
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private ServiceController _controller;
        private object _updateServiceFailureLock = new object();

        #endregion

        #region Methods

        public ServerConfiguration()
        {
            this.InitializeComponent();
        }

        private void UpdateStatus()
        {
            try
            {
                if(this._controller == null)
                    this._controller = ServiceController.GetServices().FirstOrDefault(sc => sc.ServiceName == SERVICE_NAME);

                if(this._controller == null)
                {
                    this.txtState.Text = "Not Found";
                    this.btnStart.Enabled = false;
                    this.btnStop.Enabled = false;
                }
                else
                {
                    this._controller.Refresh();
                    this.txtState.Text = this._controller.Status.ToString();
                    this.btnStart.Enabled = this._controller.Status == ServiceControllerStatus.Stopped;
                    this.btnStop.Enabled = this._controller.Status == ServiceControllerStatus.Running;
                }
            }
            catch(Exception exc)
            {
                this.txtState.Text = "------------";
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting current server status.");
            }
        }

        private void UpdateServiceFailureAction()
        {
            if (ServiceController.GetServices().Any(sc => sc.ServiceName == SERVICE_NAME))
            {
                bool enableRestart = ServerSettings.Default.EnableServiceFailureRestart;
                StringBuilder args = new StringBuilder();
                args.AppendFormat("failure {0} ", SERVICE_NAME)
                    .Append("reset=0 ")
                    .Append("actions= ");

                if (enableRestart)
                {
                    int timeoutInMilliseconds = ServerSettings.Default.ServiceRestartTimeout * 60000;
                    args.AppendFormat("restart/{0}/restart/{0}/restart/{0}", timeoutInMilliseconds);
                }
                else
                {
                    args.Append("\"\"");
                }

                try
                {
                    var procArgs = new ProcessStartInfo("sc.exe", args.ToString())
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    Process.Start(procArgs).WaitForExit();

                    if (enableRestart)
                    {
                        // Service won't restart on failure without setting failureflag
                        var failureFlagProcArgs = new ProcessStartInfo("sc.exe", "failureflag {0} 1".FormatWith(SERVICE_NAME))
                        {
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };

                        Process.Start(failureFlagProcArgs);
                    }
                }
                catch (Exception exc)
                {
                    _log.Debug(exc, "Error updating service failure-handling.");
                }
            }
        }

        #endregion

        #region Events

        private void ServerConfiguration_Load(object sender, EventArgs e)
        {
            try
            {
                this.UpdateStatus();
                this.timerUpdate.Start();

                this.chkRestartService.Checked = ServerSettings.Default.EnableServiceFailureRestart;
                this.txtRestartWaitTime.Value = ServerSettings.Default.ServiceRestartTimeout;
                UpdateServiceFailureAction(); // sync service settings with those found in the registry
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error loading server configuration window.");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                this.timerUpdate.Stop();
                
                if(this._controller != null)
                    this._controller.Start();
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error starting service.", exc);
            }
            finally
            {
                this.UpdateStatus();
                this.timerUpdate.Start();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                this.timerUpdate.Stop();
                if(this._controller != null)
                    this._controller.Stop();
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error starting service.", exc);
            }
            finally
            {
                this.UpdateStatus();
                this.timerUpdate.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.UpdateStatus();
        }

        private void chkRestartService_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                lock (_updateServiceFailureLock)
                {
                    bool restartService = this.chkRestartService.Checked;

                    if (ServerSettings.Default.EnableServiceFailureRestart != restartService)
                    {
                        ServerSettings.Default.EnableServiceFailureRestart = restartService;
                        ServerSettings.Default.Save();
                        UpdateServiceFailureAction();
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error occurred after clicking chkRestartService.");
            }
        }

        private void txtRestartWaitTime_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                lock (_updateServiceFailureLock)
                {
                    int waitTime = Convert.ToInt32(this.txtRestartWaitTime.Value);

                    if (ServerSettings.Default.ServiceRestartTimeout != waitTime)
                    {
                        ServerSettings.Default.ServiceRestartTimeout = waitTime;
                        ServerSettings.Default.Save();
                        UpdateServiceFailureAction();
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error occurred after changing restart wait time.");
            }
        }

        #endregion
    }
}