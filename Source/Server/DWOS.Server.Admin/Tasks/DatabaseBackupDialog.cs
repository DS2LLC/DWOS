using DWOS.Shared.Server;
using NLog;
using System;
using System.IO;
using System.Windows.Forms;

namespace DWOS.Server.Admin.Tasks
{
    public partial class DatabaseBackupDialog : Form
    {
        #region Fields

        private const string BACKUP_TYPE_FULL = "Full";
        private const string BACKUP_TYPE_LOG = "Transaction Log";
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        private string BackupType =>
            cboBackupType.SelectedItem?.DataValue as string;

        #endregion

        #region Methods

        public DatabaseBackupDialog()
        {
            InitializeComponent();
            cboBackupType.SelectedIndex = 0;
        }

        private void DoBackup()
        {
            txtStatus.Text = "";

            string backupDirectory = txtFilePath.Text;
            var connectionString = Data.ServerSettings.Default.DBConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                UpdateStatus("Error backing up the database. No database connection specified.");
                return;
            }

            UpdateStatus("Creating new backup.");
            var dbVersion = DatabaseUtilities.GetDbVersion(connectionString);

            var backupType = BackupType;

            if (backupType == BACKUP_TYPE_FULL)
            {
                // Setup full backup
                var fileName = Path.Combine(backupDirectory,
                    "DWOS_" + dbVersion + "_" + DateTime.Now.ToString("M_d_yyyy_h_mm_ss_tt") + ".bak");

                var databaseBackup = new DatabaseBackup(connectionString, fileName);
                databaseBackup.PercentCompleted += DatabaseBackup_PercentCompleted;
                databaseBackup.StatusChanged += DatabaseBackup_StatusChanged;
                databaseBackup.Completed += DatabaseBackup_Completed;

                // Execute the backup process
                UpdateStatus("Full backup in process.");
                databaseBackup.StartBackup(); //ASYNC
            }
            else if (backupType == BACKUP_TYPE_LOG)
            {
                // Setup transaction log backup
                var fileName = Path.Combine(backupDirectory,
                    "DWOSLog_" + DateTime.Now.ToString("M_d_yyyy_h_mm_ss_tt") + ".bak");

                var databaseBackup = new TransactionLogBackup(connectionString, fileName);
                databaseBackup.PercentCompleted += DatabaseBackup_PercentCompleted;
                databaseBackup.StatusChanged += DatabaseBackup_StatusChanged;
                databaseBackup.Completed += DatabaseBackup_Completed;

                // Execute the backup process
                UpdateStatus("Transaction log backup in process.");
                databaseBackup.StartBackup(); // ASYNC
            }
            else
            {
                UpdateStatus("Invalid backup type.");
            }
        }

        private void UpdateStatus(string status)
        {
            txtStatus.Text += DateTime.Now.ToString("g") + " " + status + Environment.NewLine;
            _log.Info(status);
            txtStatus.ScrollToCaret();
        }

        #endregion

        #region Events

        private void btnBackup_Click(object sender, EventArgs e)
        {
            try
            {
                DoBackup();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error backing up the database.");
                UpdateStatus("Error backing up the database: " + exc.Message);
            }
        }

        private void DatabaseBackup_PercentCompleted(object sender, PercentCompletedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, PercentCompletedEventArgs>(DatabaseBackup_PercentCompleted), sender, e);
                return;
            }

            prgBackup.Value = Math.Max(e.Percent, prgBackup.Value);
        }

        private void DatabaseBackup_StatusChanged(object sender, StatusMessageEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, StatusMessageEventArgs>(DatabaseBackup_StatusChanged), sender, e);
                return;
            }

            UpdateStatus(e.Message);
        }

        private void DatabaseBackup_Completed(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, EventArgs>(DatabaseBackup_Completed), sender, e);
                return;
            }

            if (BackupType == BACKUP_TYPE_LOG && chkShrinkTransactionLog.Checked)
            {
                UpdateStatus("Backup Completed - Starting shrink.");
                var logShrink = new TransactionLogShrink(Data.ServerSettings.Default.DBConnectionString);
                logShrink.StatusChanged += DatabaseBackup_StatusChanged;
                logShrink.Completed += LogShrink_Completed;
                logShrink.Shrink();
            }
            else
            {
                UpdateStatus("Backup Completed.");
            }
        }

        private void LogShrink_Completed(object sender, ShrinkCompletedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, EventArgs>(DatabaseBackup_Completed), sender, e);
                return;
            }

            UpdateStatus($"Shrink Completed. You saved {e.OriginalSizeKilobytes - e.NewSizeKilobytes} KB");
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            using (var folderPicker = new FolderBrowserDialog())
            {
                if (Directory.Exists(txtFilePath.Text))
                {
                    folderPicker.SelectedPath = txtFilePath.Text;
                }

                if (folderPicker.ShowDialog(this) == DialogResult.OK)
                {
                    txtFilePath.Text = folderPicker.SelectedPath;
                }
            }
        }

        private void cboBackupType_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                var backupType = BackupType;

                chkShrinkTransactionLog.Enabled = backupType == BACKUP_TYPE_LOG;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing backup type.");
            }
        }

        #endregion
    }
}
