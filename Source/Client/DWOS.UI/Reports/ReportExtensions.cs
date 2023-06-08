using System;
using System.Diagnostics;
using DWOS.Data;
using DWOS.Reports;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Reports
{
    public static class ReportExtensions
    {
        #region Methods

        /// <summary>
        ///   Prints the report to the default printer.
        /// </summary>
        public static void PrintReport(this IReport report)
        {
            report.PrintReport(1);
        }

        public static void PrintReport(this IReport report, int numCopies)
        {
            report.PrintReport(numCopies, true);
        }

        /// <summary>
        /// Prints the report to the default printer.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="numCopies">The num copies.</param>
        /// <param name="showStatusDialog">if set to <c>true</c> [show status dialog].</param>
        public static void PrintReport(this IReport report, int numCopies, bool showStatusDialog)
        {
            string errorMsg = null;

            try
            {
                if (report is ReportWrapper wrappedReport)
                {
                    wrappedReport.InnerReport.PrintReport(numCopies, showStatusDialog);
                    return;
                }

                if (report is ILabelReport labelReport)
                {
                    // Labels use a different default printer than other reports.
                    var labelPrinterName = !string.IsNullOrEmpty(UserSettings.Default.ShippingLabelPrinterName)
                        ? UserSettings.Default.ShippingLabelPrinterName
                        : PrinterUtilities.SelectPrinterNameDialog(Utilities.PrinterType.Label);

                    if (!string.IsNullOrEmpty(labelPrinterName))
                    {
                        labelReport.PrintLabel(labelPrinterName);
                        //labelReport.DisplayReport(new System.Threading.CancellationToken());
                    }

                    return;
                }

                //if default printer is defined by user then use it, else get the current default printer

                var printerName = !string.IsNullOrEmpty(UserSettings.Default.DefaultPrinterName)
                    ? UserSettings.Default.DefaultPrinterName
                    : PrinterUtilities.SelectPrinterNameDialog(Utilities.PrinterType.Paper);

                if (!string.IsNullOrEmpty(printerName))
                {
                    if (report is ExcelBaseReport excelReport)
                    {
                        excelReport.NumberOfCopies = numCopies;
                        var fileName = excelReport.PublishProtectedReport(System.IO.Path.GetTempPath());
                        errorMsg = PrintFile(fileName, 1, printerName);
                    }
                    else if (report is Report pdfReport)
                    {
                        if (Properties.Settings.Default.PrintPdfUsingReader)
                        {
                            // HACK - Print from the systems's PDF viewer if
                            // there happens to be an issue with Infragistics's
                            // print feature. See #30085.
                            var fileName = pdfReport.PublishReport(System.IO.Path.GetTempPath());
                            errorMsg = PrintFile(fileName, numCopies, printerName);
                        }
                        else
                        {
                            // Print using Infragistics's functionality.
                            pdfReport.CreateReportInt();

                            for (int i = 0; i < numCopies; i++)
                            {
                                pdfReport.IGReport.Print(printerName, showStatusDialog);
                            }
                        }
                    }
                    else
                    {
                        LogManager.GetCurrentClassLogger().Error("Invalid report type.");
                    }
                }
            }
            catch(Exception exc)
            {
                LogLevel exceptionLevel = LogLevel.Error;
                //if printer has been deleted then remove
                if (exc.ToString().Contains(" The specified printer has been deleted"))
                {
                    UserSettings.Default.DefaultPrinterName = null;
                    errorMsg = "The specified printer has been deleted.";
                    exceptionLevel = LogLevel.Warn;
                }
                else if (exc is System.Drawing.Printing.InvalidPrinterException)
                {
                    exceptionLevel = LogLevel.Warn;
                }

                LogManager.GetCurrentClassLogger().Log(exceptionLevel, exc, "Error printing report. " + errorMsg);
            }
        }

        private static string PrintFile(string fileName, int numCopies, string printerName)
        {
            var info = new ProcessStartInfo(fileName)
            {
                Arguments = "\"" + printerName + "\"",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true,
                Verb = "PrintTo"
            };

            bool printedReport = false;
            var errorMsg = string.Empty;

            try
            {
                for (var i = 0; i < numCopies; ++i)
                {
                    // If program is already open, it may not start a separate process.
                    // For example, Excel seems to reuse the same process.
                    Process.Start(info)?.WaitForExit();
                }

                printedReport = true;
            }
            catch (Exception exc)
            {
                errorMsg = "Error starting process for: " + fileName;
                MessageBoxUtilities.ShowMessageBoxWarn("The program associated with this file type is not found or is unable to start.", "Print Report", fileName);
                LogManager.GetCurrentClassLogger().Info(exc, errorMsg);
            }

            if (!printedReport)
            {
                // Try to open its folder with Explorer
                try
                {
                    ProcessStartInfo explorerProcessInfo = new ProcessStartInfo()
                    {
                        FileName = "explorer.exe",
                        Arguments = $"/select,\"{fileName}\""
                    };

                    Process.Start(explorerProcessInfo);
                }
                catch (Exception exc)
                {
                    string directoryName = System.IO.Path.GetDirectoryName(fileName);
                    MessageBoxUtilities.ShowMessageBoxWarn("The folder that contains this file is not found or is unable to be opened.", "Print Report", fileName);
                    LogManager.GetCurrentClassLogger().Info(exc, "Error starting Explorer process for: {0}", directoryName);
                }
            }

            return errorMsg;
        }

        #endregion
    }

}
