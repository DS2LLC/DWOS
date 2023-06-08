using System;

namespace DWOS.Reports
{
    /// <summary>
    /// Provides functionality that allows watchers to be notified whenever
    /// someone creates (prints) a report.
    /// </summary>
    public static class ReportNotifier
    {
        /// <summary>
        /// Occurs when <see cref="OnReportCreated"/> is called.
        /// </summary>
        public static event EventHandler<ReportCreatedEventArgs> ReportCreated;

        /// <summary>
        /// Call when a report is created.
        /// </summary>
        /// <remarks>
        /// Only reports that you want to do something with on creation need
        /// to call this method. Example: WO Traveler calls this to save
        /// order history elsewhere.
        /// </remarks>
        /// <param name="args"></param>
        public static void OnReportCreated(ReportCreatedEventArgs args)
        {
            ReportCreated?.Invoke(null, args);
        }
    }
}
