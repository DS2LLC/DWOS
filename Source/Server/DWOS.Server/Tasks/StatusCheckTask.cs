using DWOS.Data;
using DWOS.Server.Utilities;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace DWOS.Server.Tasks
{
    public class StatusCheckTask : IJob
    {
        #region Constants

        private const decimal MIN_FREE_PERCENT = 0.05M;
        private const decimal SIZE_LIMIT_GIGABYTES = 9.5M;

        #endregion

        #region Fields

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Log.Info($"BEGIN: {nameof(StatusCheckTask)}");
                CheckHardDrive();
                await CheckExpressDatabaseSize();
            }
            catch (Exception exc)
            {
                const string errorMsg = "Error sending email notifications.";
                Log.Error(exc, errorMsg);
            }
            finally
            {
                Log.Info($"END: {nameof(StatusCheckTask)}");
            }
        }


        private void CheckHardDrive()
        {
            const int bytesInMegabyte = 1000000;
            const int bytesInGigabyte = 1000000000;

            var drivesWithLowDiskSpace = new List<DriveInfo>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType != DriveType.Fixed)
                {
                    continue;
                }

                if (drive.AvailableFreeSpace < drive.TotalSize * MIN_FREE_PERCENT)
                {
                    drivesWithLowDiskSpace.Add(drive);
                }
            }

            if (drivesWithLowDiskSpace.Count > 0)
            {
                var errorMessageParts = new List<string>
                {
                    "DWOS server is running out of drive space on the following fixed disks:"
                };

                foreach (var drive in drivesWithLowDiskSpace)
                {
                    errorMessageParts.Add(
                        $"{drive.Name} - {drive.AvailableFreeSpace / bytesInMegabyte} " +
                        $"MB remaining of {drive.TotalSize / bytesInGigabyte} GB.");
                }
#if !DEBUG
                MessagingUtilities.QuickSendEmail(
                    ServerSettings.Default.AdminEmail,
                    "DWOS: Server Drive Space Issue",
                    string.Join(Environment.NewLine, errorMessageParts));
#endif
            }
        }

        private async Task CheckExpressDatabaseSize()
        {
            const int expressEdition = 4;
            const int kilobytesInGigabyte = 1000000;
            try
            {
                string connectionString = ServerSettings.Default.DBConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    Log.Error("No database connection defined.");
                    return;
                }

                using (var dbConnection = new SqlConnection(connectionString))
                {
                    dbConnection.Open();

                    var engineEdition = 0;
                    using (var engineEditionCmd = dbConnection.CreateCommand())
                    {
                        engineEditionCmd.CommandType = System.Data.CommandType.Text;
                        engineEditionCmd.CommandText = "SELECT SERVERPROPERTY('EngineEdition');";

                        using (var reader = await engineEditionCmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                engineEdition = Convert.ToInt32(reader[0]);
                            }
                        }
                    }

                    if (engineEdition == expressEdition)
                    {
                        // Using Express server - check for size limit
                        // Assumption: Server is running 2008 R2 or newer where
                        // the size limit is 10 GB.

                        int sizeInPages = int.MaxValue;
                        using (var sizeCmd = dbConnection.CreateCommand())
                        {
                            // The size limit does not include filestream or transaction log.
                            // See:
                            // https://blogs.msdn.microsoft.com/brian_swan/2010/06/03/sql-express-size-limit-filestream-excepted/
                            // https://blog.sqlauthority.com/2015/10/21/sql-server-database-size-limitation-in-sql-express/

                            sizeCmd.CommandType = CommandType.Text;
                            sizeCmd.CommandText = "SELECT SUM(SIZE) FROM sys.database_files WHERE type_desc = 'ROWS';";

                            using (var reader = await sizeCmd.ExecuteReaderAsync())
                            {
                                if (reader.Read())
                                {
                                    sizeInPages = Convert.ToInt32(reader[0]);
                                }
                            }
                        }

                        var sizeInGigabytes = (sizeInPages * 8) / Convert.ToDecimal(kilobytesInGigabyte);
                        if (sizeInGigabytes > SIZE_LIMIT_GIGABYTES)
                        {
                            var errorMsg = $"The database for DWOS is approaching " +
                                $"the Express Edition's 10 GB size limit. " +
                                $"Current Size: {sizeInGigabytes} GB";
#if !DEBUG
                            MessagingUtilities.QuickSendEmail(
                                ServerSettings.Default.AdminEmail,
                                "DWOS: Server Drive Space Issue",
                                errorMsg);
#endif
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                // Generate warning - some servers may be setup so that you
                // can't query sys database.
                Log.Warn(exc, "Unable to check express status and/or database size");
            }
        }

#endregion
    }
}
