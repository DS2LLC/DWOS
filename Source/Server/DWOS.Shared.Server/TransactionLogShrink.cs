using System;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using NLog;

namespace DWOS.Shared.Server
{
    /// <summary>
    /// Shrinks transaction logs for a database.
    /// </summary>
    public class TransactionLogShrink
    {
        #region Fields

        public event EventHandler<StatusMessageEventArgs> StatusChanged;
        public event EventHandler<ShrinkCompletedEventArgs> Completed;

        #endregion

        #region Properties

        public string ConnectionString { get; }

        #endregion

        #region Methods

        public TransactionLogShrink(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            }

            ConnectionString = connectionString;
        }

        public void Shrink()
        {
            try
            {
                const double megabytesPerKilobyte = 0.001;
                const double freeSpaceToLeaveMegabytes = 20d;

                var connection = new SqlConnection(ConnectionString);

                var serverConnection = new ServerConnection(connection);
                serverConnection.Connect();

                var srv = new Microsoft.SqlServer.Management.Smo.Server(serverConnection);

                var database = new Database(srv, connection.Database);
                database.Refresh(); // Refresh to retrieve log files

                double originalSizeKilobytes = 0d;
                double newSizeKilobytes = 0d;
                foreach (LogFile logFile in database.LogFiles)
                {
                    originalSizeKilobytes += logFile.Size;
                    var usedSpaceKilobytes = logFile.UsedSpace;

                    var usedSpaceMegabytes = Math.Ceiling(usedSpaceKilobytes * megabytesPerKilobyte);
                    var newSizeMegabytes = (int)(usedSpaceMegabytes + freeSpaceToLeaveMegabytes);
                    logFile.Shrink(newSizeMegabytes, ShrinkMethod.Default);

                    logFile.Refresh();
                    newSizeKilobytes += logFile.Size;
                }

                Completed?.Invoke(this, new ShrinkCompletedEventArgs(originalSizeKilobytes, newSizeKilobytes));
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error while shrinking transaction log");
                StatusChanged?.Invoke(this, new StatusMessageEventArgs("Unable to shrink transaction log."));
            }
        }

        #endregion
    }
}
