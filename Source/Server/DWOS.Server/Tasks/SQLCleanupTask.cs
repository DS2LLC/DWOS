using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DWOS.Data;
using NLog;
using Quartz;

namespace DWOS.Server.Tasks
{
    internal class SqlMaintenanceTask : IJob
    {
        #region Fields

        /// <summary>
        /// Timeout for SQL commands.
        /// </summary>
        /// <remarks>
        /// Commands may take longer than the default timeout of 30 seconds.
        /// </remarks>
        private const int COMMAND_TIMEOUT = 600; // seconds

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private static string CERT_NOTIFICATION_CLEANUP = @"
DELETE
FROM COCNotification
WHERE NotificationSent < DATEADD(DAY, -7, GETDATE());

DELETE
FROM BulkCOCNotification
WHERE NotificationSent < DATEADD(DAY, -7, GETDATE());
";

        private const string SYSPRO_INVOICE_CLEANUP = @"
DELETE
FROM SysproInvoice
WHERE Status = 'Successful'
AND Created < DATEADD(DAY, 0 - @cleanupDays, GETDATE());
";

        #endregion

        #region Methods

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Task.Run(() => BeginProcessing()).ConfigureAwait(false);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error cleaning up database.");
            }
        }

        private void BeginProcessing()
        {
            _log.Info("BEGIN: SQLCleanupTask");

            try
            {
                var connectionString = ServerSettings.Default.DBConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    _log.Error("No database connection defined.");
                    return;
                }

                var builder = new SqlConnectionStringBuilder(connectionString);
                using (var conn = new SqlConnection(ServerSettings.Default.DBConnectionString))
                {
                    conn.Open();
                    ExecuteSP(conn, "Delete_AuditHistory");
                    ExecuteSP(conn, "Delete_UnusedMedia");
                    ExecuteSP(conn, "Delete_UnusedDocuments");
                    ExecuteSP(conn, "Update_QuoteActiveStatus");
                    ExecuteSP(conn, "Delete_UnusedPartmarking");

                    ExecuteSP(conn, "Delete_LicenseActivationHistory");
                    ExecuteSP(conn, "Delete_UnusedOrderHistory");
                    ExecuteSP(conn, "Delete_UnusedRecieving");

                    if (ServerSettings.Default.EnableNightlyDatabaseShrink)
                    {
                        ExecuteSP(conn, "MANAGE_ShrinkDatabase", new KeyValuePair<string, object>("@DatabaseName", builder.InitialCatalog), new KeyValuePair<string, object>("@FreeSpace", 10));
                    }

                    ExecuteSP(conn, "MANAGE_DefragIndexes", new KeyValuePair<string, object>("@databaseName", builder.InitialCatalog), new KeyValuePair<string, object>("@onlineDefragThreshold", 10), new KeyValuePair<string, object>("@offlineDefragThreshold", 30), new KeyValuePair<string, object>("@updateStatsThreshold", 3));

                    ExecuteSql(conn, "Cleanup_CertNotifications", CERT_NOTIFICATION_CLEANUP);
                    ExecuteSql(conn, "Cleanup_Syspro", SYSPRO_INVOICE_CLEANUP, new KeyValuePair<string, object>("@cleanupDays", ServerSettings.Default.SysproSettings.CleanSuccessfulInvoiceDays));
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error processing SqlMaintenanceTask.");
            }

            _log.Info("END: SqlMaintenanceTask");
        }

        private void ExecuteSP(SqlConnection conn, string spName, params KeyValuePair <string, object>[] paramaters)
        {
            try
            {
                _log.Info("Executing procedure: " + spName);

                using(var command = conn.CreateCommand())
                {
                    if(paramaters != null && paramaters.Length > 0)
                    {
                        foreach(var kvp in paramaters)
                        {
                            var param           = command.CreateParameter();
                            param.ParameterName = kvp.Key;
                            param.Value         = kvp.Value;
                        }
                    }

                    command.CommandTimeout = COMMAND_TIMEOUT;
                    command.CommandText = spName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error executing stored procedure " + spName + ".");
            }
        }

        private void ExecuteSql(SqlConnection conn, string id, string sql, params KeyValuePair <string, object>[] parameters)
        {
            try
            {
                _log.Info($"Executing {id}");

                using (var command = conn.CreateCommand())
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        foreach(var kvp in parameters)
                        {
                            command.Parameters.AddWithValue(kvp.Key, kvp.Value);
                        }
                    }

                    command.CommandTimeout = COMMAND_TIMEOUT;
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Error executing SQL - {id}");
            }
        }

        #endregion
    }
}