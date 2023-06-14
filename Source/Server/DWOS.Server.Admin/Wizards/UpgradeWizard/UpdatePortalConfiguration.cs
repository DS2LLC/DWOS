using System;
using System.Windows.Forms;
using DWOS.Shared.Wizard;
using NLog;

namespace DWOS.Server.Admin.Wizards.UpgradeWizard
{
    public partial class UpdatePortalConfiguration : UserControl, IWizardPanel
    {
        #region Fields

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();
        private UpgradeWizardController _wizard;
        private bool _refreshedSettings;

        #endregion

        #region Methods

        public UpdatePortalConfiguration()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void btnRefreshSettings_Click(object sender, EventArgs e)
        {
            var result = WebStatusHelpers.UpdatePortalConfiguration(_wizard.DBConnectionString);

            if (result.Success)
            {
                var successMsg = @"Successfully refreshed Portal settings.";
                MessageBox.Show(this, successMsg, @"DWOS Server Admin",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                _log.Info(successMsg);
                _refreshedSettings = true;
            }
            else
            {
                var errorMsg = $@"Failed to refresh Portal settings.{Environment.NewLine}{result.ErrorMessage}";

                MessageBox.Show(this, errorMsg,
                    @"DWOS Server Admin", MessageBoxButtons.OK, MessageBoxIcon.Error);

                _log.Warn(errorMsg);
            }

            IsValid = true;
            OnValidStateChanged?.Invoke(this);
        }

        #endregion

        #region IWizardPanel Members

        public string Title => "Update Portal Configuration";

        public string SubTitle => "Updates the customer portal's configuration.";

        public Action<IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller)
        {
            _wizard = controller as UpgradeWizardController;
        }

        public Control PanelControl => this;

        public bool IsValid { get; private set; }

        public void OnMoveTo()
        {
            OnValidStateChanged?.Invoke(this);
        }

        public void OnMoveFrom()
        {
            if (!_refreshedSettings)
            {
                _log.Warn("Did not refresh settings but continuing upgrade.");
            }
        }

        public void OnFinished()
        {
            if (!_refreshedSettings)
            {
                _log.Warn("Did not refresh settings but continuing upgrade.");
            }
        }

        #endregion

    }
}
