using DWOS.Data;
using DWOS.Shared.Utilities;
using Infragistics.Win.UltraWinEditors;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DWOS.Server.Admin.SettingsPanels
{
    /// <summary>
    /// Interface for editing backup settings.
    /// </summary>
    public partial class SettingsBackup : UserControl, ISettingsPanel
    {
        #region Fields

        private Dictionary<DayOfWeek, UltraCheckEditor> _dayControls;
        #endregion

        #region Methods

        public SettingsBackup()
        {
            InitializeComponent();

            _dayControls = new Dictionary<DayOfWeek, UltraCheckEditor>()
            {
                {  DayOfWeek.Monday, chkMonday },

                {  DayOfWeek.Tuesday, chkTuesday },

                {  DayOfWeek.Wednesday, chkWednesday },

                {  DayOfWeek.Thursday, chkThursday },

                {  DayOfWeek.Friday, chkFriday },

                {  DayOfWeek.Saturday, chkSaturday },

                {  DayOfWeek.Sunday, chkSunday }
            };
        }

        private void SetEnabledBackupControls(bool backupEnabled, bool cleanupEnabled, bool transactionLogBackupEnabled)
        {
            foreach (var dayEditor in _dayControls.Values)
            {
                dayEditor.Enabled = backupEnabled;
            }

            txtBackupDir.Enabled = backupEnabled;
            dteBackupTime.Enabled = backupEnabled;
            btnSaveTo.Enabled = backupEnabled;
            chkBackupTransactionLog.Enabled = backupEnabled;
            numTransactionLogIntervalHours.Enabled = backupEnabled && transactionLogBackupEnabled;
            chkCleanup.Enabled = backupEnabled;
            pnlCleanup.Enabled = backupEnabled && cleanupEnabled;
            chkNotify.Enabled =  backupEnabled;
        }

        #endregion

        #region Events

        private void chkEnabled_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                SetEnabledBackupControls(chkEnabled.Checked, chkCleanup.Checked, chkBackupTransactionLog.Checked);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error enabling/disabling controls.");
            }
        }

        private void chkCleanup_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                SetEnabledBackupControls(chkEnabled.Checked, chkCleanup.Checked, chkBackupTransactionLog.Checked);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error enabling/disabling controls.");
            }
        }


        private void chkBackupTransactionLog_CheckedValueChanged(object sender, EventArgs e)
        {
            try
            {
                SetEnabledBackupControls(chkEnabled.Checked, chkCleanup.Checked, chkBackupTransactionLog.Checked);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error enabling/disabling controls.");
            }
        }

        private void btnSaveTo_Click(object sender, EventArgs e)
        {
            try
            {
                string currentDirectory = txtBackupDir.Text;
                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.SelectedPath = currentDirectory;
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        txtBackupDir.Text = folderDialog.SelectedPath;
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error selecting a directory to save to.");
            }
        }

        #endregion

        #region ISettingsPanel Members

        public bool Editable
        {
            get
            {
                return true;
            }
        }

        public string PanelKey
        {
            get
            {
                return "Backup";
            }
        }

        public bool IsValid
        {
            get { return true; }
        }

        public void LoadData()
        {
            try
            {
                var settings = ServerSettings.Default.Backup;
                bool backupEnabled = settings.IsEnabled;
                var backupDays = settings.Days ?? new HashSet<DayOfWeek>();
                DateTime backupTime = settings.Time;
                string backupDirectory = settings.Directory;
                bool cleanupEnabled = settings.PerformCleanup;
                int filesToKeep = settings.FilesToKeep;
                bool emailOnSuccess = settings.EmailOnSuccess; 

                chkEnabled.Checked = backupEnabled;
                chkNotify.Checked = emailOnSuccess;

                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    _dayControls[day].Checked = backupDays.Contains(day);
                }

                dteBackupTime.DateTime = backupTime;
                txtBackupDir.Text = backupDirectory;
                chkCleanup.Checked = cleanupEnabled;
                numFilesToKeep.Value = filesToKeep;

                var transactionLogBackupEnabled = settings.EnableTransactionLogBackup;
                chkBackupTransactionLog.Checked = transactionLogBackupEnabled;

                numTransactionLogIntervalHours.Value = settings.TransactionLogIntervalHours;
                numTransactionLogDays.Value = settings.TransactionLogDaysToKeep;

                SetEnabledBackupControls(backupEnabled, cleanupEnabled, transactionLogBackupEnabled);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading settings.");
            }
        }

        public void SaveData()
        {
            try
            {
                var settings = ServerSettings.Default.Backup;
                settings.IsEnabled = chkEnabled.Checked;
                settings.EmailOnSuccess = chkNotify.Checked;

                var backupDays = new HashSet<DayOfWeek>();
                foreach (var kv in _dayControls)
                {
                    var day = kv.Key;
                    var control = kv.Value;

                    if (control.Checked)
                    {
                        backupDays.Add(day);
                    }
                }

                settings.Days = backupDays;
                settings.Time = dteBackupTime.DateTime;
                settings.Directory = txtBackupDir.Text;
                settings.PerformCleanup = chkCleanup.Checked;
                settings.FilesToKeep = (numFilesToKeep.Value as int?) ?? 1;
                settings.Cron = CronExpressionBuilder.Create()
                    .SetTime(dteBackupTime.DateTime.TimeOfDay)
                    .Add(backupDays)
                    .Build();

                var transactionBackupHours = (numTransactionLogIntervalHours.Value as int?) ?? 1;

                settings.EnableTransactionLogBackup = chkBackupTransactionLog.Checked;
                settings.TransactionLogIntervalHours = transactionBackupHours;
                settings.TransactionLogDaysToKeep = (numTransactionLogDays.Value as int?) ?? 1;
                settings.TransactionLogCron = $"0 0 0-23/{transactionBackupHours} * * ? *";

                ServerSettings.Default.Save();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error saving settings.");
            }
        }



        #endregion
    }
}
