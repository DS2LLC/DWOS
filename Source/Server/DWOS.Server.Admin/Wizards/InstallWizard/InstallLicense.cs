using System;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Server.Admin.LicenseActivation;
using DWOS.Shared;
using DWOS.Shared.Wizard;
using NLog;

namespace DWOS.Server.Admin.Wizards.InstallWizard
{
    /// <summary>
    /// The 'enter license information' step of the installation wizard.
    /// </summary>
    public partial class InstallLicense : UserControl, IWizardPanel
    {
        #region Fields

        private bool _activated;
        private ActivationServiceClient _client;

        #endregion

        #region Methods

        public InstallLicense()
        {
            InitializeComponent();
        }

        private void Activate()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                this._activated = false;

                OnValidStateChanged?.Invoke(this);

                if (this._client == null)
                {
                    this._client = new ActivationServiceClient();
                    this._client.ActivateCompleted += client_ActivateCompleted;
                }

                this._client.ActivateAsync(this.txtCustomerKey.Text, ServerSettings.Default.Fingerprint, this.txtLicenseKey.Text);
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error activating software.", exc);
            }
        }

        private void LoadSettings()
        {
            this.txtCustomerKey.Text = ServerSettings.Default.CompanyKey;
            this.txtLicenseKey.Text = ServerSettings.Default.LicenseKey;
        }

        private void SaveSettings()
        {
            //Save the company key, license, and fingerprint
            ServerSettings.Default.CompanyKey = this.txtCustomerKey.Text;
            ServerSettings.Default.LicenseKey = this.txtLicenseKey.Text;
            ServerSettings.Default.Save();
        }

        #endregion

        #region Events

        private void client_ActivateCompleted(object sender, ActivateCompletedEventArgs e)
        {
            try
            {
                if(e.Error != null)
                    return;

                if(!String.IsNullOrEmpty(e.Result.ErrorInformation))
                {
                    MessageBox.Show("Error activating the software. \r\n" + e.Result.ErrorInformation, "Activation", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }

                if(e.Result.Successful)
                {
                    MessageBox.Show("Successfully activated!", "Activation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this._activated = true;
                    SaveSettings();
                }
                else
                    MessageBox.Show("Unable to activate.", "Activation");
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error during activation.");
            }
            finally
            {
                Cursor = Cursors.Default;

                OnValidStateChanged?.Invoke(this);
            }
        }

        private void btnActivate_Click(object sender, EventArgs e) { Activate(); }

        #endregion

        #region IWizardPanel

        public string Title
        {
            get { return "Activate License"; }
        }

        public string SubTitle
        {
            get { return "Use the provided activation information to activate the software."; }
        }

        public Action <IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller) { }

        public Control PanelControl
        {
            get { return this; }
        }

        public bool IsValid
        {
            get { return this._activated; }
        }

        public void OnMoveTo() { LoadSettings(); }

        public void OnMoveFrom() { }

        public void OnFinished() { }

        #endregion
    }
}