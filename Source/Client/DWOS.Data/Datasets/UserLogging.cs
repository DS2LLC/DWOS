using System;
using System.Collections.Generic;
using System.Data;

namespace DWOS.Data.Datasets
{

    public partial class UserLogging
    {
        private static List<AnalyticsInfo> _analyticsCache = new List<AnalyticsInfo>();
        private static DateTime _lastAnalyticsFlush = DateTime.Now;
        private const int MIN_BETWEEN_FLUSH = 10;

        public static void AddLogInHistory(int userID, string machine = null)
        {
            try
            {
                using (var ta = new UserLoggingTableAdapters.UserLoginHistoryTableAdapter())
                    ta.Insert(userID, DateTime.Now, machine ?? System.Environment.MachineName);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error adding login history.");
            }
        }

        public static void AddFailedLogInHistory(string machine = null)
        {
            try
            {
                using (var ta = new UserLoggingTableAdapters.FailedLoginHistoryTableAdapter())
                {
                    ta.Insert(DateTime.Now, machine ?? Environment.MachineName);
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error adding failed login history.");
            }
        }

        public static void AddActivationHistory(int userID, string action, string computerName, string uid)
        {
            try
            {
                using (var ta = new UserLoggingTableAdapters.LicenseActivationHistoryTableAdapter())
                    ta.Insert(userID, action, DateTime.Now, computerName, uid);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error writing license activation history.");
            }
        }

        /// <summary>
        /// Adds the name of the tool to keep track of the number of click counts.
        /// </summary>
        /// <param name="toolName">Name of the tool. If NULL, then flush the queue.</param>
        public static void AddAnalytics(string toolName)
        {
            try
            {
                if (toolName != null)
                {
                    toolName = toolName.TrimToMaxLength(50);

                    var day = DateTime.Now.Date.ToString();
                    var info = _analyticsCache.Find(ai => ai.ToolName == toolName && ai.Day == day);

                    if (info == null)
                        _analyticsCache.Add(new AnalyticsInfo() { ToolName = toolName, Day = day, UsageCount = 1 });
                    else
                        info.UsageCount += 1;
                }

                //if we should flush now
                if (toolName == null || DateTime.Now.Subtract(_lastAnalyticsFlush).TotalMinutes > MIN_BETWEEN_FLUSH)
                {
                    FlushAnalytics();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error writing analytics history.");
            }
        }

        /// <summary>
        /// Flushes the analytics to the DB.
        /// </summary>
        public static void FlushAnalytics()
        {
            try
            {
                const string SQL = "if exists (select 1 from ApplicationAnalytics where ToolName = '{0}' AND [Day] = '{1}') " +
                                   "UPDATE ApplicationAnalytics SET UsageCount = ((select UsageCount from ApplicationAnalytics where ToolName = '{0}' AND [Day] = '{1}') + {2}) WHERE ToolName = '{0}' AND [Day] = '{1}'"
                                   + " ELSE INSERT INTO ApplicationAnalytics VALUES ('{0}', '{1}', {2})";

                using (var a = new DWOS.Data.Datasets.UserLoggingTableAdapters.ApplicationAnalyticsTableAdapter())
                {
                    if (a.Connection.State != System.Data.ConnectionState.Open)
                        a.Connection.Open();

                    foreach (var analyticInfo in _analyticsCache)
                    {
                        var sql = SQL.FormatWith(analyticInfo.ToolName, analyticInfo.Day, analyticInfo.UsageCount);
                        using (var updateCommand = new System.Data.SqlClient.SqlCommand(sql, a.Connection))
                            updateCommand.ExecuteNonQuery();
                    }
                }

                _analyticsCache.Clear();
                _lastAnalyticsFlush = DateTime.Now;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error flushin tool analytics to db.");
            }
        }

        private class AnalyticsInfo
        {
            public string ToolName { get; set; }
            public string Day { get; set; }
            public int UsageCount { get; set; }
        }

        public static DataTable ExecuteSQL(string sql, out int recordsAffected)
        {
            using (var a = new DWOS.Data.Datasets.UserLoggingTableAdapters.ApplicationAnalyticsTableAdapter())
            {
                if (a.Connection.State != System.Data.ConnectionState.Open)
                    a.Connection.Open();

                using (var cmd = new System.Data.SqlClient.SqlCommand(sql, a.Connection))
                {
                    var dt = new DataTable();

                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                        recordsAffected = reader.RecordsAffected;
                    }

                    return dt;
                }
            }
        }
    }
}

