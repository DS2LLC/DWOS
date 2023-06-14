using System;
using System.Collections.Generic;
using DWOS.Data;
using NLog;
using Quartz;
using System.Data.SqlClient;
using DWOS.Data.Order;
using DWOS.Shared.Utilities;
using System.Threading.Tasks;

namespace DWOS.Server.Tasks
{
    internal class ScheduleResetTask : IJob
    {
        #region Methods

        private void ResetSchedule(IJobExecutionContext context)
        {
            var connString = ServerSettings.Default.DBConnectionString;

            var skipReset = ApplicationSettings.Current.SchedulerType != SchedulerType.Manual ||
                            string.IsNullOrEmpty(connString) ||
                            !ApplicationSettings.Current.ScheduleResetEnabled;

            if (skipReset)
            {
                return;
            }

            // Run order reset SQL
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();

                var orderItems = new List<SchedulingHelper.OrderScheduleItem>();
                var batchItems = new List<SchedulingHelper.BatchScheduleItem>();

                using (var orderCmd = conn.CreateCommand())
                {
                    orderCmd.CommandText = "SELECT OrderID, SchedulePriority FROM [Order] WHERE SchedulePriority != 0;";

                    using (var reader = orderCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var orderId = reader.GetInt32(0);
                            var schedulePriority = reader.GetInt32(1);
                            orderItems.Add(new SchedulingHelper.OrderScheduleItem()
                            {
                                OrderId = orderId,
                                PreviousSchedulePriority = schedulePriority,
                                CurrentSchedulePriority = 0
                            });
                        }
                    }
                }

                using (var batchCmd = conn.CreateCommand())
                {
                    batchCmd.CommandText = "SELECT BatchID, SchedulePriority FROM Batch WHERE SchedulePriority != 0";

                    using (var reader = batchCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var batchId = reader.GetInt32(0);
                            var schedulePriority = reader.GetInt32(1);
                            batchItems.Add(new SchedulingHelper.BatchScheduleItem()
                            {
                                BatchId = batchId,
                                PreviousSchedulePriority = schedulePriority,
                                CurrentSchedulePriority = 0
                            });
                        }
                    }
                }

                SchedulingHelper.SaveSchedule(orderItems, batchItems, SecurityManagerSimple.ServerSecurityImposter);
            }
        }

        #endregion

        #region IJob Members

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                LogManager.GetCurrentClassLogger().Info("BEGIN: {0}", nameof(ScheduleResetTask));
                await Task.Run(() => ResetSchedule(context)).ConfigureAwait(false);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error resetting order/batch schedule.");
            }
            finally
            {
                LogManager.GetCurrentClassLogger().Info("END: {0}", nameof(ScheduleResetTask));
            }
        }

        #endregion
    }
}
