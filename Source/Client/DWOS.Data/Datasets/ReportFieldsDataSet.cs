using System.Linq;

namespace DWOS.Data.Datasets
{
    public partial class ReportFieldsDataSet
    {
        partial class ReportDataTable
        {
            public ReportRow GetDefaultReport(ReportFieldMapper.enumReportType reportType)
            {
                var report = this.FirstOrDefault(rr => rr.IsCustomerIDNull() && rr.ReportType == (int)reportType);
                return report;
            }
        }
    }
}

namespace DWOS.Data.Datasets.ReportFieldsDataSetTableAdapters {
    
    
    public partial class ReportFieldsTableAdapter {
    }
}
