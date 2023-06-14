using System;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;
using DWOS.Server.Admin.Tasks;
using DWOS.Shared.Wizard;
using NLog;

namespace DWOS.Server.Admin.Wizards.InstallWizard
{
    /// <summary>
    /// The 'start service' step of the installation wizard.
    /// </summary>
    public partial class InstallService : UserControl, IWizardPanel
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private bool _started;

        #endregion

        #region Methods

        public InstallService()
        {
            InitializeComponent();
        }

        private void UpdateStatus()
        {
            try
            {
                this.timerUpdate.Stop();

                ServiceController controller = ServiceController.GetServices().FirstOrDefault(sc => sc.ServiceName == ServerConfiguration.SERVICE_NAME);
                if(controller != null)
                {
                    this._started = controller.Status == ServiceControllerStatus.Running;
                    this.txtState.Text = controller.Status.ToString();
                    this.btnStart.Enabled = controller.Status != ServiceControllerStatus.Running;
                    this.btnStop.Enabled = controller.Status != ServiceControllerStatus.Stopped;
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

        public string Title
        {
            get { return "DWOS Server"; }
        }

        public string SubTitle
        {
            get { return "Start the DWOS Server service."; }
        }

        public Action <IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller) { }

        public Control PanelControl
        {
            get { return this; }
        }

        public bool IsValid
        {
            get { return this._started; }
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
    }
}