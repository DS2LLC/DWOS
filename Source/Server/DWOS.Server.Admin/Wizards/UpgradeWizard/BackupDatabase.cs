using DWOS.Shared.Server;
using DWOS.Shared.Wizard;
using NLog;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace DWOS.Server.Admin.Wizards.UpgradeWizard
{
    /// <summary>
    /// The 'backup database' step of the upgrade wizard.
    /// </summary>
    public partial class BackupDatabase : UserControl, IWizardPanel
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private UpgradeWizardController _wizard;

        #endregion

        #region Methods

        public BackupDatabase()
        {
            InitializeComponent();
        }

        private void RunScripts()
        {
            try
            {
                this.txtStatus.Text = "";

                string backupDirectory = this.txtFilePath.Text;

                if (_wizard.DBConnectionString == null)
                {
                    UpdateStatus("Error backing up the database. No database connection specified.");
                    return;
                }

                UpdateStatus("Creating new backup.");
                var dbVersion = DatabaseUtilities.GetDbVersion(_wizard.DBConnectionString);
                var fileName = Path.Combine(backupDirectory,
                    "DWOS_" + dbVersion.ToString() + "_" + DateTime.Now.ToString("M_d_yyyy_h_mm_ss_tt") + ".bak");

                var databaseBackup = new DatabaseBackup(_wizard.DBConnectionString, fileName);
                databaseBackup.PercentCompleted += DatabaseBackup_PercentCompleted;
                databaseBackup.StatusChanged += DatabaseBackup_StatusChanged;
                databaseBackup.Completed += DatabaseBackup_Completed;

                // Execute the backup process
                UpdateStatus("Backup in process.");
                databaseBackup.StartBackup(); //ASYNC

                _wizard.BackupFileName = fileName;
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error backing up the database.");
                UpdateStatus("Error backing up the database: " + exc.Message);
            }
        }

        private void UpdateStatus(string status)
        {
            this.txtStatus.Text += DateTime.Now.ToString("g") + @" " + status + Environment.NewLine;
            _log.Info(status);
            this.txtStatus.ScrollToCaret();
        }

        #endregion

        #region Events

        private void btnInstall_Click(object sender, EventArgs e)
        {
            RunScripts();

            OnValidStateChanged?.Invoke(this);
        }

        private void DatabaseBackup_PercentCompleted(object sender, PercentCompletedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<object, PercentCompletedEventArgs>(DatabaseBackup_PercentCompleted), sender, e);
                return;
            }

            prgBackup.Value = e.Percent;
        }

        private void DatabaseBackup_StatusChanged(object sender, StatusMessageEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<object, StatusMessageEventArgs>(DatabaseBackup_StatusChanged), sender, e);
                return;
            }

            UpdateStatus(e.Message);
        }

        private void DatabaseBackup_Completed(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<object, EventArgs>(DatabaseBackup_Completed), sender, e);
                return;
            }

            UpdateStatus("Backup Completed.");

            OnValidStateChanged(this);
        }

        #endregion

        #region IWizardPanel

        public string Title
        {
            get { return "Backup DWOS Database"; }
        }

        public string SubTitle
        {
            get { return "Backup the database before the server is upgraded."; }
        }

        public Action <IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller)
        {
            _wizard = controller as UpgradeWizardController;
        }

        public Control PanelControl
        {
            get { return this; }
        }

        public bool IsValid
        {
            get
            {
                // This step is optional.
                return true;
            }
        }

        public void OnMoveTo()
        {
            OnValidStateChanged?.Invoke(this);
        }

        public void OnMoveFrom() { }

        public void OnFinished() { }

        #endregion
    }
}