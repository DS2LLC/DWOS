using DWOS.Data;
using NLog;
using Quartz;
using Quartz.Impl.Triggers;
using System;
using System.Threading.Tasks;

namespace DWOS.Server.Tasks
{
    /// <summary>
    /// Automatically reloads <see cref="ServerSettings"/> and reschedules
    /// any dependent tasks.
    /// </summary>
    internal class ReloadServerSettingsTask : IJob
    {
        internal const string KEY_STARTUP = "Startup";

        #region Methods

        private static async Task ReloadSettings(IJobExecutionContext context)
        {
            ServerSettings.Default.ReLoad();
            await UpdateBackupJob(context);
            await UpdateTransactionLogBackupJob(context);
            await UpdateSysproJob(context);
            await UpdateScheduleResetJob(context);
        }

        private static async Task UpdateBackupJob(IJobExecutionContext context)
        {
            var previousTrigger = (await context.Scheduler.GetTrigger(new TriggerKey("SqlBackupTaskTrigger")).ConfigureAwait(false)) as ICronTrigger;

            var sqlBackupJob = await context.Scheduler.GetJobDetail(new JobKey("SqlBackupTask"));

            var newCronTrigger = new CronTriggerImpl("SqlBackupTaskTrigger",
                null,
                ServerSettings.Default.Backup.Cron);

            var rescheduleJob = previousTrigger == null ||
                                previousTrigger.CronExpressionString != ServerSettings.Default.Backup.Cron;

            if (!rescheduleJob)
            {
                return;
            }

            DateTimeOffset? nextScheduledFireDate;
            if (previousTrigger == null)
            {
                nextScheduledFireDate = await context.Scheduler.ScheduleJob(sqlBackupJob, newCronTrigger);
            }
            else
            {
                nextScheduledFireDate = await context.Scheduler.RescheduleJob(previousTrigger.Key, newCronTrigger);
            }

            bool rescheduleImmediately;
            var newCronExpression = new CronExpression(ServerSettings.Default.Backup.Cron);
            var currentDateTimeOffset = DateTimeOffset.Now;

            if (context.PreviousFireTimeUtc.HasValue)
            {
                // Check for scheduled time between reloads.
                var nextFireDate = newCronExpression.GetTimeAfter(context.PreviousFireTimeUtc.Value);
                rescheduleImmediately = currentDateTimeOffset > nextFireDate &&
                                        currentDateTimeOffset < nextScheduledFireDate;
            }
            else if (context.MergedJobDataMap.ContainsKey(KEY_STARTUP))
            {
                // Check for scheduled time immediately after startup but before first reloads.
                var startupDate = DateTime.Parse(context.MergedJobDataMap.GetString(KEY_STARTUP));
                var nextFireDate = newCronExpression.GetTimeAfter(startupDate);
                rescheduleImmediately = currentDateTimeOffset > nextFireDate &&
                                        currentDateTimeOffset < nextScheduledFireDate;
            }
            else
            {
                rescheduleImmediately = false;
            }

            // Fire the backup task immediately if it was scheduled between reloads
            // but isn't setup to be executed immediately.
            if (rescheduleImmediately)
            {
                await context.Scheduler.ScheduleJob(JobBuilder.Create<SqlBackupTask>().Build(),
                    TriggerBuilder.Create()
                        .WithIdentity("SqlBackupTaskTrigger-Temp")
                        .StartNow()
                        .Build());
            }
        }

        private static async Task UpdateTransactionLogBackupJob(IJobExecutionContext context)
        {
            var previousTrigger = (await context.Scheduler.GetTrigger(new TriggerKey("TransactionLogBackupTaskTrigger")).ConfigureAwait(false)) as ICronTrigger;

            var sqlBackupJob = await context.Scheduler.GetJobDetail(new JobKey("TransactionLogBackupTask"));

            var newCronTrigger = new CronTriggerImpl("TransactionLogBackupTaskTrigger",
                null,
                ServerSettings.Default.Backup.TransactionLogCron);

            var rescheduleJob = previousTrigger == null
                                || previousTrigger.CronExpressionString != ServerSettings.Default.Backup.TransactionLogCron;

            if (!rescheduleJob)
            {
                return;
            }

            DateTimeOffset? nextScheduledFireDate;
            if (previousTrigger == null)
            {
                nextScheduledFireDate = await context.Scheduler.ScheduleJob(sqlBackupJob, newCronTrigger);
            }
            else
            {
                nextScheduledFireDate = await context.Scheduler.RescheduleJob(previousTrigger.Key, newCronTrigger);
            }

            bool rescheduleImmediately;
            var newCronExpression = new CronExpression(ServerSettings.Default.Backup.TransactionLogCron);
            var currentDateTimeOffset = DateTimeOffset.Now;

            if (context.PreviousFireTimeUtc.HasValue)
            {
                // Check for scheduled time between reloads.
                var nextFireDate = newCronExpression.GetTimeAfter(context.PreviousFireTimeUtc.Value);
                rescheduleImmediately = currentDateTimeOffset > nextFireDate &&
                                        currentDateTimeOffset < nextScheduledFireDate;
            }
            else if (context.MergedJobDataMap.ContainsKey(KEY_STARTUP))
            {
                // Check for scheduled time immediately after startup but before first reloads.
                var startupDate = DateTime.Parse(context.MergedJobDataMap.GetString(KEY_STARTUP));
                var nextFireDate = newCronExpression.GetTimeAfter(startupDate);
                rescheduleImmediately = currentDateTimeOffset > nextFireDate &&
                                        currentDateTimeOffset < nextScheduledFireDate;
            }
            else
            {
                rescheduleImmediately = false;
            }

            // Fire the backup task immediately if it was scheduled between reloads
            // but isn't setup to be executed immediately.
            if (rescheduleImmediately)
            {
                await context.Scheduler.ScheduleJob(JobBuilder.Create<TransactionLogBackupTask>().Build(),
                    TriggerBuilder.Create()
                        .WithIdentity("TransactionLogBackupTaskTrigger-Temp")
                        .StartNow()
                        .Build());
            }
        }

        private static async Task UpdateSysproJob(IJobExecutionContext context)
        {
            var sysproIntervalMinutes = ServerSettings.Default.SysproSettings.UpdateIntervalMinutes;
            var previousTrigger = (await context.Scheduler.GetTrigger(new TriggerKey("SysproSyncTaskTrigger")).ConfigureAwait(false)) as ISimpleTrigger;

            var currentInterval = TimeSpan.FromMinutes(sysproIntervalMinutes);

            if (previousTrigger != null && currentInterval == previousTrigger.RepeatInterval)
            {
                return;
            }
            var syncJob = await context.Scheduler.GetJobDetail(new JobKey("SysproSyncTask"));
            var newTrigger = new SimpleTriggerImpl("SysproSyncTaskTrigger", new DateTimeOffset(DateTime.Now.AddMinutes(sysproIntervalMinutes)), null, SimpleTriggerImpl.RepeatIndefinitely, currentInterval)
            {
                MisfireInstruction = MisfireInstruction.IgnoreMisfirePolicy
            };

            if (previousTrigger == null)
            {
                await context.Scheduler.ScheduleJob(syncJob, newTrigger);
            }
            else
            {
                await context.Scheduler.RescheduleJob(previousTrigger.Key, newTrigger);
            }
        }

        private static async Task UpdateScheduleResetJob(IJobExecutionContext context)
        {
            // Get latest reset time from database
            var settings = ApplicationSettings .NewApplicationSettings();
            settings.UpdateDatabaseConnection(ServerSettings.Default.DBConnectionString);

            var previousTrigger = (await context.Scheduler.GetTrigger(new TriggerKey("ScheduleResetTaskTrigger")).ConfigureAwait(false)) as ISimpleTrigger;

            var previousStartTime = previousTrigger?.StartTimeUtc.TimeOfDay;
            if (settings.ScheduleResetTime == previousStartTime)
            {
                return;
            }

            var firstScheduleResetTime = DateTime.Today.Add(settings.ScheduleResetTime);

            var rescheduleImmediately = false;

            if (firstScheduleResetTime < DateTime.Now)
            {
                DateTime? lastRefresh = null;

                if (context.PreviousFireTimeUtc.HasValue)
                {
                    lastRefresh = context.PreviousFireTimeUtc.Value.DateTime;
                }
                else if (context.MergedJobDataMap.ContainsKey(KEY_STARTUP))
                {
                    lastRefresh = DateTime.Parse(context.MergedJobDataMap.GetString(KEY_STARTUP));
                }

                rescheduleImmediately = lastRefresh.HasValue && firstScheduleResetTime > lastRefresh.Value;

                firstScheduleResetTime = firstScheduleResetTime.AddDays(1);
            }

            var newTrigger = TriggerBuilder.Create()
                .WithIdentity("ScheduleResetTaskTrigger")
                .StartAt(firstScheduleResetTime)
                .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromDays(1))
                .RepeatForever()
                .WithMisfireHandlingInstructionIgnoreMisfires())
                .Build();

            var resetJob = await context.Scheduler.GetJobDetail(new JobKey("ScheduleResetTask"));

            if (previousTrigger == null)
            {
                await context.Scheduler.ScheduleJob(resetJob, newTrigger);
            }
            else
            {
                await context.Scheduler.RescheduleJob(previousTrigger.Key, newTrigger);
            }

            // Fire the backup task immediately if it was scheduled between reloads
            // but isn't setup to be executed immediately.
            if (rescheduleImmediately)
            {
                await context.Scheduler.ScheduleJob(JobBuilder.Create<ScheduleResetTask>().Build(),
                    TriggerBuilder.Create()
                        .WithIdentity("ScheduleResetTaskTrigger-Temp")
                        .StartNow()
                        .Build());
            }
        }

        #endregion

        #region IJob Members

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                LogManager.GetCurrentClassLogger().Info("BEGIN: {0}", nameof(ReloadServerSettingsTask));
                await ReloadSettings(context);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error reloading server settings.");
            }
            finally
            {
                LogManager.GetCurrentClassLogger().Info("END: {0}", nameof(ReloadServerSettingsTask));
            }
        }

        #endregion
    }
}
