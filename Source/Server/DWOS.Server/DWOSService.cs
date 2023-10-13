using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using DWOS.Data;
using DWOS.Server.Properties;
using DWOS.Server.Services;
using DWOS.Server.Tasks;
using DWOS.Services;
using DWOS.Shared;
using NLog;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Topshelf;
using System.Globalization;
using System.Linq;
using DWOS.Shared.Utilities;

namespace DWOS.Server
{
    /// <summary>
    /// Service that acts as the central DWOS server.
    /// </summary>
    /// <remarks>
    /// In addition to Quartz scheduling, the main DWOS service is responsible
    /// for three other services: a license server for Client, a Web API
    /// server for Mobile, and a separate license server for Mobile.
    /// </remarks>
    public class DWOSService : ServiceControl
    {
        #region Fields

        /// <summary>
        /// Host for the license service
        /// </summary>
        private ServiceHost _licenseServiceHost;

        /// <summary>
        /// Instance of the Quartz scheduler.
        /// </summary>
        private IScheduler _scheduler;

        /// <summary>
        /// Host for the mobile app service.
        /// </summary>
        private IDisposable _restServer;

        #endregion

        #region Methods

        static DWOSService()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        public DWOSService()
        {
            ErrorReporterTarget.GetAdditionalErrorInfo = GetErrorReportInfo;
        }

        public bool Start(HostControl hostControl)
        {
            try
            {
                LogManager.GetCurrentClassLogger().Info("DWOS Service starting.");

                //ensure registry settings are loaded on startup
                ServerSettings.Default.UpdateDatabaseConnection("DWOS Services");

                if(!ValidateDatabase())
                {
#if !DEBUG
                    if(!string.IsNullOrEmpty(ServerSettings.Default.AdminEmail))
                    {
                        Utilities.MessagingUtilities.QuickSendEmail(ServerSettings.Default.AdminEmail,
                            "DWOS Server Stop",
                            $"DWOS server on {Environment.MachineName} has stopped because the database is not in a valid state.");
                    }
#endif
                    
                    LogManager.GetCurrentClassLogger().Warn("Stopping service. Invalid database state.");
                    return false;
                }

                this._licenseServiceHost?.Close();

                this._licenseServiceHost = new CustomServiceHost(typeof(LicenseService));
                this._licenseServiceHost.Open();

                ScheduleTasks();
                RegisterDependencies();

                ThreadPool.QueueUserWorkItem((o) => StartMobileServer());
                
                return true;
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on service start up.");
                return false;
            }
        }

        public void OnPause()
        {
            LogManager.GetCurrentClassLogger().Info("DWOS Service pausing.");

            this._scheduler?.PauseAll();

            Data.Datasets.UserLogging.FlushAnalytics();
        }

        public void OnContinue()
        {
            LogManager.GetCurrentClassLogger().Info("DWOS Service continuing.");

            this._scheduler?.ResumeAll();
        }

        public bool Stop(HostControl hostControl)
        {
            try
            {
                LogManager.GetCurrentClassLogger().Info("DWOS Service stopping.");
                
                Data.Datasets.UserLogging.FlushAnalytics();

                this._licenseServiceHost?.Close();
                this._licenseServiceHost = null;

                this._scheduler?.Shutdown();
                this._scheduler = null;

                _restServer?.Dispose();
                _restServer = null;

                return true;
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error stopping service.");
                return true;
            }
        }

        private void ScheduleTasks()
        {
            // get a scheduler
            var factory = new StdSchedulerFactory();
            factory.Initialize();

            this._scheduler = factory.GetScheduler().Result;
            this._scheduler.Start();

            var rand = new Random();

            // Quote notifications
            this._scheduler.ScheduleJob(new JobDetailImpl("QuoteNotifications", null, typeof(QuoteReminderTask)), new CronTriggerImpl("QuoteTrigger", null, Settings.Default.QuoteReminderCRON));

            // Shipment notifications
            this._scheduler.ScheduleJob(new JobDetailImpl("ShipmentNotification", null, typeof(ShipmentNotificationTask)), new SimpleTriggerImpl("ShipmentNotificationTrigger", new DateTimeOffset(DateTime.Now.AddSeconds(30)), null, SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromMinutes(Settings.Default.ShippingNotificationsInterval)) {MisfireInstruction = MisfireInstruction.IgnoreMisfirePolicy});

            // Portal notifications
            this._scheduler.ScheduleJob(new JobDetailImpl("CustomerPortalNotification", null, typeof(CustomerPortalNotificationTask)), new SimpleTriggerImpl("CustomerPortalNotificationTrigger", new DateTimeOffset(DateTime.Now.AddSeconds(90)), null, SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromMinutes(Settings.Default.PortalNotificationsInterval)) {MisfireInstruction = MisfireInstruction.IgnoreMisfirePolicy});

            // Scheduled report notifications
            this._scheduler.ScheduleJob(new JobDetailImpl("ReportNotification", null, typeof(ReportNotificationTask)), new SimpleTriggerImpl("ReportNotificationTrigger", new DateTimeOffset(DateTime.Now.AddSeconds(120)), null, SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromMinutes(10)) {MisfireInstruction = MisfireInstruction.IgnoreMisfirePolicy});

            // License check
            // this._scheduler.ScheduleJob(new JobDetailImpl("LicenseCheckTask", null, typeof(LicenseCheckTask)), new SimpleTriggerImpl("LicenseCheckTrigger", new DateTimeOffset(DateTime.Now.AddSeconds(15)), null, SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromHours(24)) {MisfireInstruction = MisfireInstruction.IgnoreMisfirePolicy});

            // License prune
            this._scheduler.ScheduleJob(new JobDetailImpl("LicensePrune", null, typeof(LicensePruneTask)), new SimpleTriggerImpl("LicensePruneTrigger", new DateTimeOffset(DateTime.Now.AddSeconds(5)), null, SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromMinutes(3)) {MisfireInstruction = MisfireInstruction.IgnoreMisfirePolicy});

            // Database cleanup
            this._scheduler.ScheduleJob(new JobDetailImpl("SQLCleanupTask", null, typeof(SqlMaintenanceTask)), new CronTriggerImpl("SQLCleanupTaskTrigger", null, "0 0 2 ? * *"));

            // Update statistics
            // Make the minute random to help prevent excessive server load at DS2 Servers
            //this._scheduler.ScheduleJob(new JobDetailImpl("UpdateStatsTask", null, typeof(UpdateStatsTask)), new CronTriggerImpl("UpdateStatsTaskTrigger", null, "0 " + rand.Next(0, 59).ToString() + " 23 ? * *"));

            // Refresh settings
            this._scheduler.ScheduleJob(new JobDetailImpl("RefreshSettingsTask", null, typeof(RefreshSettingsTask)), new SimpleTriggerImpl("RefreshSettingsTrigger", new DateTimeOffset(DateTime.Now.AddMinutes(10)), null, SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromMinutes(10)) { MisfireInstruction = MisfireInstruction.IgnoreMisfirePolicy });

            // Database backup
            // This task must be Durable - there's another job that can reschedule it.
            this._scheduler.ScheduleJob(new JobDetailImpl("SqlBackupTask", null, typeof(SqlBackupTask)) { Durable = true },
                new CronTriggerImpl("SqlBackupTaskTrigger",
                null,
                ServerSettings.Default.Backup.Cron));

            // Transaction backup
            // This task must be Durable because another job can reschedule it
            _scheduler.ScheduleJob(new JobDetailImpl("TransactionLogBackupTask", null, typeof(TransactionLogBackupTask)) { Durable = true },
                new CronTriggerImpl("TransactionLogBackupTaskTrigger",
                null,
                ServerSettings.Default.Backup.TransactionLogCron));

            // SYSPRO sync
            var sysproIntervalMinutes = ServerSettings.Default.SysproSettings.UpdateIntervalMinutes;
            this._scheduler.ScheduleJob(new JobDetailImpl("SysproSyncTask", null, typeof(SysproSyncTask)) { Durable = true },
                new SimpleTriggerImpl("SysproSyncTaskTrigger", new DateTimeOffset(DateTime.Now.AddMinutes(sysproIntervalMinutes)), null, SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromMinutes(sysproIntervalMinutes)) { MisfireInstruction = MisfireInstruction.IgnoreMisfirePolicy });

            // Refresh server settings
            this._scheduler.ScheduleJob(new JobDetailImpl("ReloadServerSettingsTask", null, typeof(ReloadServerSettingsTask)),
                TriggerBuilder.Create()
                .WithIdentity("ReloadServerSettingsTrigger")
                .StartAt(DateTimeOffset.Now.AddMinutes(1))
                .UsingJobData(ReloadServerSettingsTask.KEY_STARTUP, DateTime.Now.ToString(CultureInfo.InvariantCulture))
                .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromMinutes(1)).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires())
                .Build());

            // Reset schedule
            var firstScheduledResetTime = DateTime.Today.Add(ApplicationSettings.Current.ScheduleResetTime);

            if (firstScheduledResetTime < DateTime.Now)
            {
                firstScheduledResetTime = firstScheduledResetTime.AddDays(1);
            }

            this._scheduler.ScheduleJob(new JobDetailImpl("ScheduleResetTask", null, typeof(ScheduleResetTask)),
                TriggerBuilder.Create()
                .WithIdentity("ScheduleResetTaskTrigger")
                .StartAt(firstScheduledResetTime)
                .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromDays(1))
                .RepeatForever()
                .WithMisfireHandlingInstructionIgnoreMisfires())
                .Build());

            // COC notifications
            this._scheduler.ScheduleJob(
                new JobDetailImpl("CocNotification", null, typeof(CocNotificationTask)),
                new SimpleTriggerImpl("CocNotificationTrigger", new DateTimeOffset(DateTime.Now.AddSeconds(45)),
                    null, SimpleTriggerImpl.RepeatIndefinitely,
                    TimeSpan.FromMinutes(Settings.Default.COCNotificationsInterval))
                {
                    MisfireInstruction = MisfireInstruction.IgnoreMisfirePolicy
                });

            // Order Approval notifications
            _scheduler.ScheduleJob(
                new JobDetailImpl("OrderApprovalNotification", null, typeof(OrderApprovalNotificationTask)),
                new SimpleTriggerImpl("OrderApprovalNotificationTrigger", new DateTimeOffset(DateTime.Now.AddMinutes(1)),
                    null, SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromMinutes(Settings.Default.OrderApprovalNotificationsInterval))
                {
                    MisfireInstruction = MisfireInstruction.IgnoreMisfirePolicy
                });

            // Order Receipt notifications
            _scheduler.ScheduleJob(
                new JobDetailImpl("OrderReceiptNotification", null, typeof(OrderReceiptNotificationTask)),
                new SimpleTriggerImpl("OrderReceiptNotificationTrigger", new DateTimeOffset(DateTime.Now.AddMinutes(1)),
                    null, SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromMinutes(Settings.Default.OrderApprovalNotificationsInterval))
                {
                    MisfireInstruction = MisfireInstruction.IgnoreMisfirePolicy
                });

            // Order Hold notifications
            _scheduler.ScheduleJob(
                JobBuilder.Create<OrderHoldNotificationTask>().WithIdentity("OrderHoldNotification").Build(),
                TriggerBuilder.Create()
                    .WithIdentity("OrderHoldNotificationTrigger")
                    .StartAt(DateTime.Now.AddMinutes(1.25))
                    .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(Settings.Default.OrderHoldNotificationsInterval)
                        .RepeatForever()
                        .WithMisfireHandlingInstructionIgnoreMisfires())
                    .Build());

            // Late Order notifications
            _scheduler.ScheduleJob(
                JobBuilder.Create<LateOrderNotificationTask>().WithIdentity("LateOrderNotification").Build(),
                TriggerBuilder.Create()
                    .WithIdentity("LateOrderNotificationTrigger")
                    .StartAt(DateTime.Now.AddMinutes(1.50))
                    .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(Settings.Default.LateOrderNotificationsInterval)
                        .RepeatForever()
                        .WithMisfireHandlingInstructionIgnoreMisfires())
                    .Build());

            // Server Status Check
            _scheduler.ScheduleJob(
                JobBuilder.Create<StatusCheckTask>().WithIdentity("StatusCheck").Build(),
                TriggerBuilder.Create()
                    .WithIdentity("StatusCheckTrigger")
                    .StartNow()
                    .WithCronSchedule("0 0 8 ? * *")
                    .Build());
        }

        private void RegisterDependencies()
        {
            DependencyContainer.Register<Data.Date.ICalendarPersistence>(new Data.Date.CalendarPersistence());
            DependencyContainer.Register<Data.IDwosApplicationSettingsProvider>(new Data.DwosApplicationSettingsProvider());
        }

        private Dictionary<string, string> GetErrorReportInfo() => new Dictionary<string, string>
            {
                { "Company Key", ServerSettings.Default.CompanyKey },
                { "Admin Email", ServerSettings.Default.AdminEmail }
            };

        /// <summary>
        /// Validates the database is in a valid state. Ensure can connect and is correct version.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        private bool ValidateDatabase()
        {
            try
            {
                if (ServerSettings.Default.DBConnectionString == null)
                {
                    LogManager.GetCurrentClassLogger().Warn("Unable to connect to database. No database connection string defined.");
                    return false;
                }

                using (var conn = new SqlConnection(ServerSettings.Default.DBConnectionString))
                {
                    conn.Open();

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT [Value] FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion'";
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var actualDbVersion = new Version(reader[0].ToString());
                                var currentApplicationVersion = new Version(About.ApplicationVersion);
                                var requiredDbVersion = UpdateScript.FromEmbeddedResources()
                                    .Where(s => s.UpgradeVersion <= currentApplicationVersion)
                                    .Max(s => s.UpgradeVersion);

                                if (requiredDbVersion.CompareMajorMinorBuild(actualDbVersion))
                                {
                                    return true;
                                }
                                else
                                {
                                    LogManager.GetCurrentClassLogger().Warn("Database version {0} does not match application's required version {1}".FormatWith(actualDbVersion, requiredDbVersion));
                                }
                            }
                            else
                            {
                                LogManager.GetCurrentClassLogger().Warn("Unknown database version in database {0}. Database does not contain DatabaseVersion in table ApplicationSettings.".FormatWith(conn.Database));
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error validating database in DWOS Server.");
                return false;
            }
        }

        private void StartMobileServer()
        {
            try
            {
                if(ServerSettings.Default.EnableRESTServices)
                {
                    LogManager.GetCurrentClassLogger().Info("Starting REST server on port {0}.", Settings.Default.RESTServerPort);
                    _restServer = Startup.StartApp($"http://*:{Settings.Default.RESTServerPort}/");
                }
                else
                    LogManager.GetCurrentClassLogger().Info("REST server is disabled.");
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error starting REST server.");
            }
        }

#endregion

#region Events

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var exc = args.ExceptionObject as Exception;

            if (exc != null)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Unhandled error in DWOS Service.");
            }
        }

#endregion

#region CustomServiceHost

        private class CustomServiceHost : ServiceHost
        {
            public CustomServiceHost(Type svc) : base(svc) { }

            protected override void OnOpening()
            {
                //foreach end point change from LocalHost to the actual machinename [Required when using service discovery]
                foreach(ServiceEndpoint endpoint in base.Description.Endpoints)
                {
                    Uri url = endpoint.Address.Uri;
                    if(url.Host == "localhost")
                    {
                        var urlBuilder = new UriBuilder(url.Scheme, Environment.MachineName, url.Port, url.PathAndQuery);
                        endpoint.Address = new EndpointAddress(urlBuilder.Uri);
                        endpoint.ListenUri = urlBuilder.Uri;
                    }
                }

                base.OnOpening();
            }
        }

#endregion


    }
}