using DWOS.Data;
using DWOS.Reports;
using Microsoft.Win32;
using System.Drawing.Printing;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    internal class FileService : IFileService
    {
        #region IFileService Members

        public string GetSpreadsheet()
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "xls files|*.xlsx;*.xls;*.csv"
            };

            if (dialog.ShowDialog() ?? false)
            {
                return dialog.FileName;
            }

            return null;
        }

        public void Open(Report report) => report?.DisplayReport();

        public void Print(Report report)
        {
            if (report == null)
            {
                return;
            }

            var printerName = !string.IsNullOrEmpty(UserSettings.Default.DefaultPrinterName)
                ? UserSettings.Default.DefaultPrinterName
                : new PrinterSettings().PrinterName;

            if (report.IGReport != null)
            {
                report.CreateReportInt();
                report.IGReport.Print(printerName, false);
            }
            else
            {
                // Fallback to display
                report.DisplayReport();
            }
        }

        #endregion
    }
}
