using System;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;
using DWOS.Server.Admin.Tasks;
using DWOS.Shared.Wizard;
using NLog;

namespace DWOS.Server.Admin.Wizards.UpgradeWizard
{
    /// <summary>
    /// Used as the 'stop service' and 'start service' steps of the upgrade wizard.
    /// </summary>
    public partial class StartService : UserControl, IWizardPanel
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private bool _started;

        #endregion

        #region Properties

        public ServiceState RequiredServiceState { get; set; }

        #endregion

        #region Methods

        public StartService()
        {
            InitializeComponent();
        }

        private void UpdateStatus()
        {
            try
            {
                this.timerUpdate.Stop();

                var controller = ServiceController.GetServices().FirstOrDefault(sc => sc.ServiceName == ServerConfiguration.SERVICE_NAME);
                
                if(controller != null)
                {
                    this._started         = controller.Status == ServiceControllerStatus.Running;
                    this.txtState.Text    = controller.Status.ToString();
                    this.btnStart.Enabled = controller.Status != ServiceControllerStatus.Running && this.RequiredServiceState == ServiceState.Started;
                    this.btnStop.Enabled  = controller.Status != ServiceControllerStatus.Stopped && this.RequiredServiceState == ServiceState.Stopped;
                }
                else
                {
                    this._started = false;
                    this.txtState.Text = "Not Installed";
                    this.btnStart.Enabled = false;
                    this.btnStop.Enabled = false;
                }

                OnValidStateChanged?.Invoke(this);

                this.timerUpdate.Start();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error determining if service is installed.");
            }
        }

        #endregion

        #region Events

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceController controller = ServiceController.GetServices().FirstOrDefault(sc => sc.ServiceName == ServerConfiguration.SERVICE_NAME);
                if(controller != null && controller.Status != ServiceControllerStatus.Running)
                    controller.Start();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error starting service.");
            }
            finally
            {
                UpdateStatus();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceController controller = ServiceController.GetServices().FirstOrDefault(sc => sc.ServiceName == ServerConfiguration.SERVICE_NAME);

                if(controller != null && controller.Status != ServiceControllerStatus.Stopped)
                    controller.Stop();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error starting service.");
            }
            finally
            {
                UpdateStatus();
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        #endregion

        #region IWizardPanel

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public Action <IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller)
        {
            btnStart.Enabled = this.RequiredServiceState == ServiceState.Started;
            btnStop.Enabled = this.RequiredServiceState == ServiceState.Stopped;

            if(this.RequiredServiceState == ServiceState.Started)
            {
                this.Title = "Start DWOS Server";
                this.SubTitle = "Start the DWOS Server.";
                lblInfoMessage.Text = "Service must be started before continuing.";
            }
            else
            {
                this.Title = "Stop DWOS Server";
                this.SubTitle = "Stop the DWOS Server.";
                lblInfoMessage.Text = "Service must be stopped before continuing.";
            }
        }

        public Control PanelControl
        {
            get { return this; }
        }

        public bool IsValid
        {
            get
            {
                return this.RequiredServiceState == ServiceState.Started ? this._started : !this._started;
            }
        }

        public void OnMoveTo()
        {
            UpdateStatus();
        }

        public void OnMoveFrom()
        {
            this.timerUpdate.Stop();
        }

        public void OnFinished() { }

        #endregion

        #region ServiceState

        /// <summary>
        /// Represents the state of a service.
        /// </summary>
        public enum ServiceState
        {
            Started,
            Stopped
        }

        #endregion
    }
}