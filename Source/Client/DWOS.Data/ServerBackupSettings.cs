using DWOS.Shared.Data;
using DWOS.Shared.Settings;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DWOS.Data
{
    /// <summary>
    /// Defines server settings for database backup.
    /// </summary>
    public sealed class ServerBackupSettings : NestedRegistrySettingsBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating if database backup is enabled.
        /// </summary>
        [DataColumn]
        public bool IsEnabled
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating the days of the week that the
        /// backup task should run on.
        /// </summary>
        [DataColumn(FieldConverterType = typeof(BackupJsonConverter<HashSet<DayOfWeek>>))]
        public HashSet<DayOfWeek> Days
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value for the time to run the backup task.
        /// </summary>
        [DataColumn(FieldConverterType = typeof(TimeConverter), DefaultValue = "03:00")]
        public DateTime Time
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the directory to save database backups to.
        /// </summary>
        [DataColumn(DefaultValue = @"C:\Data")]
        public string Directory
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the cron expression to use for the backup task.
        /// </summary>
        [DataColumn(DefaultValue = @"0 0 3 * * ?")]
        public string Cron
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the number of files to keep when performing cleanup
        /// after the database backup task.
        /// </summary>
        [DataColumn(DefaultValue = 7)]
        public int FilesToKeep
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating if cleanup should be done after
        /// the database backup task.
        /// </summary>
        [DataColumn]
        public bool PerformCleanup
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating if an email should be sent to the
        /// email at <see cref="ServerSettings.AdminEmail"/> after
        /// successfully running the database backup task.
        /// </summary>
        [DataColumn]
        public bool EmailOnSuccess
        {
            get; set;
        }

        #region Transaction Log

        /// <summary>
        /// Gets or sets a flag indicating if transaction log backups are enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if the server is managing transaction log backups; otherwise, false.
        /// </value>
        [DataColumn(DefaultValue = true)]
        public bool EnableTransactionLogBackup
        {
            get; set;
        }

        [DataColumn(DefaultValue = 1)]
        public int TransactionLogIntervalHours
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the cron expression to use for transaction log backups.
        /// </summary>
        /// <remarks>
        /// The default schedules the transaction log backup every hour on the
        /// hour to match the default for
        /// <see cref="TransactionLogIntervalHours"/>.
        /// </remarks>
        [DataColumn(DefaultValue = "0 0 0-23/1 * * ? *")]
        public string TransactionLogCron
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the number of days to keep transaction logs for when
        /// <see cref="PerformCleanup"/> is <c>true</c>.
        /// </summary>
        [DataColumn(DefaultValue = 14)]
        public int TransactionLogDaysToKeep
        {
            get; set;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ServerBackupSettings"/> class.
        /// </summary>
        /// <param name="hive"></param>
        /// <param name="baseKey"></param>
        public ServerBackupSettings(RegistryKey hive, string baseKey)
            : base(hive, baseKey)
        {

        }

        protected override void AfterLoad()
        {
            base.AfterLoad();

            if (Days == null)
            {
                Days = new HashSet<DayOfWeek>();
            }
        }

        #endregion

        #region BackupJsonConverter

        private sealed class BackupJsonConverter<T> : IFieldConverter
        {
            #region IFieldConverter Members

            public object ConvertFromField(object value)
            {
                if (value == null)
                {
                    return default(T);
                }

                return JsonConvert.DeserializeObject(value.ToString(), typeof(T));
            }

            public object ConvertToField(object value)
            {
                if (value == null)
                {
                    return null;
                }

                return JsonConvert.SerializeObject(value,
                    Formatting.None,
                    new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.None });
            }

            #endregion
        }

        #endregion

        #region TimeConverter

        private sealed class TimeConverter : IFieldConverter
        {
            #region IFieldConverter Members

            public object ConvertFromField(object value)
            {
                if (value == null)
                {
                    return default(DateTime);
                }

                return DateTime.ParseExact(value.ToString(), "HH:mm", null);
            }

            public object ConvertToField(object value)
            {
                var valueAsTime = value as DateTime?;
                if (!valueAsTime.HasValue)
                {
                    return null;
                }

                return valueAsTime.Value.ToString("HH:mm");
            }

            #endregion
        }

        #endregion
    }
}
