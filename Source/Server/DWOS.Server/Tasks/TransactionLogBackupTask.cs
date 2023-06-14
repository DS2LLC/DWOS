using System;
using DWOS.Data;
using NLog;
using Quartz;
using System.IO;
using System.Threading.Tasks;
using DWOS.Shared.Server;

namespace DWOS.Server.Tasks
{
    [DisallowConcurrentExecution] // do not allow concurrent backups
    internal class TransactionLogBackupTask : IJob
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        private void BeginProcessing()
        {
            var connectionString = ServerSettings.Default.DBConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                _log.Error("No database connection defined.");
                return;
            }
            else if (!ServerSettings.Default.Backup.IsEnabled || !ServerSettings.Default.Backup.EnableTransactionLogBackup)
            {
                return;
            }

            var backupDirectory = ServerSettings.Default.Backup.Directory;

            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }

            var fileName = Path.Combine(backupDirectory,
                "DWOSLog_" + DateTime.Now.ToString("M_d_yyyy_h_mm_ss_tt") + ".bak");

            var backup = new TransactionLogBackup(connectionString, fileName);
            backup.Backup();
        }

        #endregion

        #region IJob Members

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _log.Info($"BEGIN: {nameof(TransactionLogBackupTask)}");
                await Task.Run(() => BeginProcessing()).ConfigureAwait(false);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error backing up transaction log.");
            }
            finally
            {
                _log.Info($"END: {nameof(TransactionLogBackupTask)}");
            }
        }

        #endregion
    }
}
