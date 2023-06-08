using System;
using System.Threading.Tasks;
using DWOS.Reports;
using NLog;

namespace DWOS.UI.Reports
{
    /// <summary>
    /// Creates an instance of a report to print or display.
    /// </summary>
    public class ReportGenerator
    {
        #region Properties

        /// <summary>
        /// Gets the report-generating function for this instance.
        /// </summary>
        public Func<IReport> ReportFunction { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportGenerator"/> class.
        /// </summary>
        /// <param name="reportFunc">
        /// Function that generates a report.
        /// </param>
        public ReportGenerator(Func<IReport> reportFunc)
        {
            if (reportFunc == null)
            {
                throw new ArgumentNullException(nameof(reportFunc));
            }

            ReportFunction = reportFunc;
        }

        /// <summary>
        /// Synchronously displays the report.
        /// </summary>
        public void Display()
        {
            using (var report = ReportFunction?.Invoke())
            {
                report?.DisplayReport();
            }
        }


        /// <summary>
        /// Asynchronously prints the report.
        /// </summary>
        /// <param name="numberOfCopies"></param>
        /// <param name="showStatusDialog"></param>
        public void BeginPrint(int numberOfCopies = 1, bool showStatusDialog = true)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var report = ReportFunction?.Invoke())
                    {
                        report?.PrintReport(numberOfCopies, showStatusDialog);
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error asynchronously printing report.");
                }
            });
        }

        #endregion
    }
}
