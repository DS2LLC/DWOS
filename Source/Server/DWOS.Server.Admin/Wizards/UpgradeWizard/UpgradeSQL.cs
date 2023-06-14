using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.Shared.Wizard;
using NLog;
using DWOS.Server.Admin.SQL;

namespace DWOS.Server.Admin.Wizards.UpgradeWizard
{
    /// <summary>
    /// The 'upgrade database' step of the upgrade wizard.
    /// </summary>
    public partial class UpgradeSQL : UserControl, IWizardPanel, IUserNotifier
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        
        private bool _isValid;
        private UpgradeWizardController _wizard;
        private Version _fromVersion = null;
        private Version _toVersion = null;

        #endregion

        #region Methods

        public UpgradeSQL()
        {
            InitializeComponent();
        }

        private void RunScripts()
        {
            try
            {
                this.txtStatus.Text = "";

                if(_wizard.DBConnectionString != null)
                {
                    if (_fromVersion == null || _toVersion == null)
                        UpdateVersionInfo();

                    if (_fromVersion == _toVersion)
                    {
                        MessageBox.Show(@"The database is up to date.",
                            About.ApplicationName,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        ServerSettings.Default.Version = About.ApplicationVersion;
                        ServerSettings.Default.Save();

                        _isValid = true;
                        OnValidStateChanged(this);
                    }
                    else
                    {
                        using(var conn = new SqlConnection(_wizard.DBConnectionString))
                        {
                            conn.Open();

                            DbUpgrader upgrader = new DbUpgrader(this);
                            var upgradeResult = upgrader.UpgradeDb(conn, _fromVersion, _toVersion);

                            Version upgradeToVersion = upgradeResult?.UpgradeVersion;
                            bool hasErrors = upgradeResult?.HasErrors ?? true;

                            //save new database version
                            if (!hasErrors)
                            {
                                ServerSettings.Default.Version = About.ApplicationVersion;
                                ServerSettings.Default.Save();

                                MessageBox.Show(
                                    $@"Successfully upgraded to {upgradeToVersion ?? _toVersion}",
                                    About.ApplicationName,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            }

                            _isValid = !hasErrors;
                            OnValidStateChanged(this);
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error upgrading database.");
                ShowNotification("Error upgrading database: " + exc.Message);
            }
            finally
            {
                UpdateVersionInfo();
            }
        }

        private void UpdateVersionInfo()
        {
            try
            {
                if (_fromVersion == null)
                {
                    using (var conn = new SqlConnection(_wizard.DBConnectionString))
                    {
                        lblDBName.Text = conn.Database;
                        conn.Open();
                        _fromVersion = SqlUtility.GetVersion(conn);
                    }
                }

                if (_toVersion == null)
                {
                    _toVersion = VersionUtilities.Min(
                        new Version(About.ApplicationVersion),
                        UpdateScript.FromEmbeddedResources().Max(s => s.UpgradeVersion));
                }

                lblFromVersion.Text = _fromVersion == null ? "UNKNOWN" : "{0}.{1}.{2}".FormatWith(_fromVersion.Major, _fromVersion.Minor, _fromVersion.Build);
                lblToVersion.Text = "{0}.{1}.{2}".FormatWith(_toVersion.Major, _toVersion.Minor, _toVersion.Build);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error updating version info.");
            }
        }

        #endregion

        #region Events

        private void btnInstall_Click(object sender, EventArgs e)
        {
            RunScripts();

            OnValidStateChanged?.Invoke(this);
        }

        #endregion

        #region IWizardPanel

        public string Title
        {
            get { return "Upgrade DWOS Database"; }
        }

        public string SubTitle
        {
            get { return "Install the DWOS data in your database."; }
        }

        public Action <IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller) { _wizard = controller as UpgradeWizardController; }

        public Control PanelControl
        {
            get { return this; }
        }

        public bool IsValid
        {
            get { return this._isValid; }
        }

        public void OnMoveTo()
        {
            UpdateVersionInfo();

            OnValidStateChanged?.Invoke(this);
        }

        public void OnMoveFrom() { }

        public void OnFinished() { }

        #endregion

        #region INotifier Members

        public void ShowNotification(string status)
        {
            this.txtStatus.Text += status + Environment.NewLine;
            _log.Info(status);
            this.txtStatus.ScrollToCaret();
        }

        #endregion
    }
}