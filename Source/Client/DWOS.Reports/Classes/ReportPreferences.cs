using DWOS.Reports.Properties;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Reports
{
    /// <summary>
    /// Managed report-related settings.
    /// </summary>
    public static class ReportPreferences
    {
        private const char SettingSeparator = ';';

        /// <summary>
        /// Gets or sets the ID of the last selected process.
        /// </summary>
        public static int LastReportProcessId
        {
            get => Settings.Default.LastReportProcessID;
            set => Settings.Default.LastReportProcessID = value;
        }

        public static List<int> LastProcessAnswerReportIds
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(Settings.Default.LastProcessAnswerReportIds))
                    {
                        return new List<int> { Settings.Default.LastReportProcessID };
                    }

                    return Settings.Default.LastProcessAnswerReportIds
                        .Split(SettingSeparator)
                        .Select(s => int.Parse(s))
                        .ToList();
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger()
                        .Error(exc, "Error parsing user configuration value");

                    return null;
                }
            }
            set
            {
                if (value == null)
                {
                    Settings.Default.LastProcessAnswerReportIds = string.Empty;
                }

                Settings.Default.LastProcessAnswerReportIds =
                    string.Join($"{SettingSeparator}", value);
            }
        }

        /// <summary>
        /// Saves changes to settings.
        /// </summary>
        public static void Save()
        {
            Settings.Default.Save();
        }
    }
}
