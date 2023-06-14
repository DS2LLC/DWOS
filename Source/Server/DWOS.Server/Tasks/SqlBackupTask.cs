using System;
using System.Linq;
using System.Data.SqlClient;
using DWOS.Data;
using NLog;
using Quartz;
using Microsoft.SqlServer.Management.Smo;
using System.IO;
using Microsoft.SqlServer.Management.Common;
using DWOS.Server.Utilities;
using System.Threading.Tasks;

namespace DWOS.Server.Tasks
{
    internal sealed class SqlBackupTask : IJob
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        private void BeginProcessing()
        {
            _log.Info("BEGIN: SqlBackupTask");

            SqlConnection dbConnection = null;
            ServerConnection managementConnection = null;

            try
            {
                string connectionString = ServerSettings.Default.DBConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    _log.Error("No database connection defined.");
                    return;
                }
                else if (!ServerSettings.Default.Backup.IsEnabled)
                {
                    return;
                }

                string backupDirectory = ServerSettings.Default.Backup.Directory;

                if (!Directory.Exists(backupDirectory))
                {
                    Directory.CreateDirectory(backupDirectory);
                }

                dbConnection = new SqlConnection(connectionString);

                var dbVersion = GetDbVersion(connectionString);
                var fileName = Path.Combine(backupDirectory,
                    "DWOS_" + dbVersion.ToString() + "_" + DateTime.Now.ToString("M_d_yyyy_h_mm_ss_tt") + ".bak");
                managementConnection = new ServerConnection(dbConnection);
                managementConnection.Connect();

                var srv = new Microsoft.SqlServer.Management.Smo.Server(managementConnection);
                //Define a Backup object variable.
                var _backup = new Backup();
                _backup.Action               = BackupActionType.Database;
                _backup.BackupSetDescription = "Full backup of DWOS";
                _backup.BackupSetName        = managementConnection.DatabaseName + " " + DateTime.Now.ToString().Replace("\\", ".");
                _backup.Database             = managementConnection.DatabaseName;
                
                // Set the backup device to 'file'
                var bdi = new BackupDeviceItem(fileName, DeviceType.File);
                _backup.Devices.Add(bdi);
                
                _backup.Incremental = false; // If 'false', the backup will be full
                _backup.ExpirationDate = DateTime.Now.AddDays(14);
                _backup.CopyOnly = true;

                // The log will be truncated after backup completion
                _backup.LogTruncation = BackupTruncateLogType.Truncate;
                
                // Execute the backup process
                _backup.SqlBackup(srv);

                if (ServerSettings.Default.Backup.PerformCleanup)
                {
                    CleanupFiles();
                }

                if (ServerSettings.Default.Backup.EmailOnSuccess)
                {
                    EmailAdmin(fileName);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    EmailAdminError(ex);
                }
                catch (Exception emailSendException)
                {
                    _log.Error(emailSendException, "Error sending email after SQL backup error.");
                }

                throw;
            }
            finally
            {
                _log.Info("END: SqlBackupTask");

                if (managementConnection != null)
                {
                    managementConnection.Disconnect();
                }

                if (dbConnection != null)
                {
                    dbConnection.Dispose();
                }
            }
        }

        private void CleanupFiles()
        {
            // BeginProcessing() should create this backup directory
            var backupDirectory = new DirectoryInfo(ServerSettings.Default.Backup.Directory);

            // Cleanup full backup files
            var backupFilesToDelete = backupDirectory
                .GetFiles("DWOS_*.bak")
                .OrderByDescending(n => n.CreationTimeUtc)
                .Skip(ServerSettings.Default.Backup.FilesToKeep);

            foreach (var fileToDelete in backupFilesToDelete)
            {
                fileToDelete.Delete();
            }

            // Cleanup transaction log files
            var transactionLogCutoffDate = DateTime.Today.AddDays(-1 * ServerSettings.Default.Backup.TransactionLogDaysToKeep);
            var transactionLogBackupsToDelete = backupDirectory
                .GetFiles("DWOSLog_*.bak")
                .Where(n => n.CreationTime < transactionLogCutoffDate);

            foreach (var fileToDelete in transactionLogBackupsToDelete)
            {
                fileToDelete.Delete();
            }
        }

        private void EmailAdmin(string fileName)
        {
            const string msgFormat = "The DWOS Database was successfully saved on {0} at {1}.\n" +
                "A file containing the database's data was saved at \"{2}\"";

            if (string.IsNullOrEmpty(ServerSettings.Default.AdminEmail))
            {
                return;
            }

            DateTime backupTime = DateTime.Now;

            string title = "DWOS Server - Successful Database Backup";
            string msg = string.Format(msgFormat,
                backupTime.ToLongDateString(),
                backupTime.ToLongTimeString(),
                fileName);

            MessagingUtilities.QuickSendEmail(ServerSettings.Default.AdminEmail,
                title,
                msg);
        }

        private void EmailAdminError(Exception ex)
        {
            if (string.IsNullOrEmpty(ServerSettings.Default.AdminEmail))
            {
                return;
            }

            var backupTime =  DateTime.Now;

            var title = "DWOS Server - Cannot Backup Database Backup";
            var msg = $"There was an error backing up the DWOS Database on " +
                $"{backupTime.ToLongDateString()} at {backupTime.ToLongTimeString()}." +
                Environment.NewLine +
                "Details for support:" + Environment.NewLine +
                ex.Message + Environment.NewLine +
                ex.StackTrace;

            MessagingUtilities.QuickSendEmail(
                ServerSettings.Default.AdminEmail,
                title,
                msg);
        }

        private static Version GetDbVersion(string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT [Value] FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion'";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return new Version(reader[0].ToString());
                    }
                }
            }

            return null;
        }

        #endregion

        #region IJob Members

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Task.Run(() => BeginProcessing()).ConfigureAwait(false);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error backing up SQL Server.");
            }
        }

        #endregion

    }
}
