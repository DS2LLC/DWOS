using System;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using NLog;
using DWOS.Data;

namespace DWOS.Shared.Server
{
    /// <summary>
    /// Performs transaction log backup for a database.
    /// </summary>
    public class TransactionLogBackup
    {
        #region Fields

        /// <summary>
        /// Occurs when the backup status changes.
        /// </summary>
        public event EventHandler<StatusMessageEventArgs> StatusChanged;

        /// <summary>
        /// Occurs when the backup gets closer to completion.
        /// </summary>
        public event EventHandler<PercentCompletedEventArgs> PercentCompleted;

        /// <summary>
        /// Occurs when the backup is complete;
        /// </summary>
        public event EventHandler<EventArgs> Completed;
        private readonly Backup _backup;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the connection string for this instance.
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Gets the file name for this instance.
        /// </summary>
        public string FileName { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionLogBackup"/> class.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fileName"></param>
        public TransactionLogBackup(string connectionString, string fileName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
            }

            ConnectionString = connectionString;
            FileName = fileName;

            var databaseName = new SqlConnection(ConnectionString).Database;

            //Define a Backup object variable.
            _backup = new Backup
            {
                Action = BackupActionType.Log,
                BackupSetDescription = "Transaction log backup of DWOS.",
                BackupSetName = databaseName + " " + DateTime.Now.ToString().Replace("\\", "."),
                Database = databaseName
            };

            // Set the backup device to 'file'
            var bdi = new BackupDeviceItem(fileName, DeviceType.File);
            _backup.Devices.Add(bdi);

            _backup.ExpirationDate = DateTime.Now.AddDays(ServerSettings.Default.Backup.TransactionLogDaysToKeep);

            // The log will be truncated after backup completion
            _backup.Complete += Backup_Complete;
            _backup.PercentComplete += Backup_PercentComplete;
        }

        /// <summary>
        /// Starts an asynchronous backup of the database.
        /// </summary>
        public void StartBackup()
        {
            var serverConnection = new ServerConnection(new SqlConnection(ConnectionString));
            serverConnection.Connect();

            var srv = new Microsoft.SqlServer.Management.Smo.Server(serverConnection);
            _backup.SqlBackupAsync(srv);
        }

        /// <summary>
        /// Starts a synchronous backup of the database.
        /// </summary>
        public void Backup()
        {
            var serverConnection = new ServerConnection(new SqlConnection(ConnectionString));
            serverConnection.Connect();

            var srv = new Microsoft.SqlServer.Management.Smo.Server(serverConnection);
            _backup.SqlBackup(srv);
        }

        #endregion

        #region Events

        private void OnStatusChanged(string message) =>
            StatusChanged?.Invoke(this, new StatusMessageEventArgs(message));

        private void Backup_PercentComplete(object sender, PercentCompleteEventArgs e)
        {
            PercentCompleted?.Invoke(this, new PercentCompletedEventArgs(e.Percent));

            if (!string.IsNullOrEmpty(e.Message))
            {
                OnStatusChanged(e.Message);
            }
        }

        private void Backup_Complete(object sender, ServerMessageEventArgs e)
        {
            if (_backup.AsyncStatus?.LastException != null)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(_backup.AsyncStatus.LastException, "Error during transaction log backup.");

                StatusChanged?.Invoke(this, new StatusMessageEventArgs("Backup Error: " + e.Error.Message));
            }
            else
            {
                _backup.Devices.Clear();
                Completed?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
