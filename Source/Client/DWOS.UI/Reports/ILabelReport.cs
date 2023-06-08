using DWOS.Reports;

namespace DWOS.UI.Reports
{
    /// <summary>
    /// Defines methods for label-based reports.
    /// </summary>
    public interface ILabelReport : IReport
    {
        void PrintLabel(string printerName);
    }
}
