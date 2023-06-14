using DWOS.Reports;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    public interface IFileService
    {
        string GetSpreadsheet();

        void Open(Report report);

        void Print(Report report);
    }
}
