using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DWOS.Data;
using DWOS.Server.LicenseActivation;
using DWOS.Server.Properties;
using DWOS.Shared;
using NLog;
using Quartz;

namespace DWOS.Server.Tasks
{
    internal class UpdateStatsTask : IJob
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

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
                throw new JobExecutionException(exc);
            }
        }

        private void BeginProcessing()
        {
            _log.Info("BEGIN: UpdateStatsTask");

            try
            {
                var client      = new ActivationServiceClient();
                var usageStats  = new UsageStatistics();

                var stats = new List <UsageStat>();
                usageStats.Stastics = stats;
                usageStats.StatisticsStartTime = LicenseManager.LicenseManager.Default.UsageStats.StatStartTime;
                usageStats.StatisticsStopTime = DateTime.Now;

                int activeCustomers = ExecuteSQL("SELECT count(*) AS CustomerCount FROM Customer WHERE (Active = 1)");
                stats.Add(new UsageStat {Metric = "Customers.ActiveCount", Value = activeCustomers});

                int ordersCreated = ExecuteSQL("SELECT count(*) FROM [Order] WHERE (cast(OrderDate AS DATE) = cast(GETDATE() AS DATE))");
                stats.Add(new UsageStat {Metric = "Orders.NewCount", Value = ordersCreated});

                int openOrders = ExecuteSQL("SELECT count(*) FROM [Order] WHERE Status = 'Open'");
                stats.Add(new UsageStat { Metric = "Orders.ActiveCount", Value = openOrders });

                stats.Add(new UsageStat {Metric = "License.TotalActivations", Value = LicenseManager.LicenseManager.Default.UsageStats.TotalActivations});
                stats.Add(new UsageStat { Metric = "License.TotalDeActivations", Value = LicenseManager.LicenseManager.Default.UsageStats.TotalDeActivations });
                stats.Add(new UsageStat { Metric = "License.TotalOutOfLicense", Value = LicenseManager.LicenseManager.Default.UsageStats.TotalOutOfLicense });
                stats.Add(new UsageStat { Metric = "License.TotalPrunes", Value = LicenseManager.LicenseManager.Default.UsageStats.TotalPrunes });

                stats.Add(new UsageStat { Metric = "License.AvgActivations", Value = LicenseManager.LicenseManager.Default.UsageStats.GetAvgActivations() });
                stats.Add(new UsageStat { Metric = "License.MaxActivations", Value = LicenseManager.LicenseManager.Default.UsageStats.GetMaxActivations() });
                stats.Add(new UsageStat { Metric = "License.MinActivations", Value = LicenseManager.LicenseManager.Default.UsageStats.GetMinActivations() });
                
                //Added - 10.19.14
                stats.Add(new UsageStat { Metric = "Server.Version", Value = new Version(About.ApplicationVersion).ToDecimal() });

                if(ordersCreated > 0 || LicenseManager.LicenseManager.Default.UsageStats.TotalActivations > 0)
                    client.UpdateStats(Settings.Default.ProductID, ServerSettings.Default.CompanyKey, usageStats);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error processing UpdateStatsTask.");
            }
            finally
            {
                //Reset the stats since they got updated
                LicenseManager.LicenseManager.Default.UsageStats.Reset();
                _log.Info("END: UpdateStatsTask");
            }
        }

        private int ExecuteSQL(string sql)
        {
            try
            {
                if(ServerSettings.Default.DBConnectionString != null)
                {
                    _log.Info("Executing sql: " + sql);

                    using(var conn = new SqlConnection(ServerSettings.Default.DBConnectionString))
                    {
                        conn.Open();

                        using(SqlCommand command = conn.CreateCommand())
                        {
                            command.CommandText = sql;
                            command.CommandType = CommandType.Text;
                            object value = command.ExecuteScalar();

                            if(value != null)
                            {
                                int output = 0;
                                if(Int32.TryParse(value.ToString(), out output))
                                    return output;
                            }
                        }
                    }
                }

                return 0;
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error executing sql " + sql + ".");
                return 0;
            }
        }

        #endregion
    }
}