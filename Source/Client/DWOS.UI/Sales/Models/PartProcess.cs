using System.Linq;
using DWOS.Data;
using DWOS.Data.Datasets;
using NLog;

namespace DWOS.UI.Sales.Models
{
    public class PartProcess
    {
        public static ILogger LOGGER = LogManager.GetCurrentClassLogger();

        public int ProcessId { get; private set; }

        public int ProcessAliasId { get; private set; }

        public int StepOrder { get; private set; }

        public string Department { get; private set; }

        public int CocCount { get; private set; }

        public decimal? LoadCapacityVariance { get; set; }

        public decimal? LoadCapacityWeight { get; set; }

        public int? LoadCapacityQuantity { get; set; }

        public ProcessLeadTime LeadTime { get; private set; }

        public static PartProcess From(OrdersDataSet.PartProcessSummaryRow processRow)
        {
            if (processRow == null)
            {
                return null;
            }

            return new PartProcess
            {
                ProcessId = processRow.ProcessID,
                ProcessAliasId = processRow.ProcessAliasID,
                StepOrder = processRow.StepOrder,
                Department = processRow.Department,
                CocCount = processRow.IsCOCCountNull()
                    ? 0
                    : processRow.COCCount,

                LoadCapacityVariance = processRow.IsLoadCapacityVarianceNull()
                    ? (decimal?)null
                    : processRow.LoadCapacityVariance,

                LoadCapacityWeight = processRow.IsLoadCapacityWeightNull()
                    ? (decimal?)null
                    : processRow.LoadCapacityWeight,

                LoadCapacityQuantity = processRow.IsLoadCapacityQuantityNull()
                    ? (int?)null
                    : processRow.LoadCapacityQuantity,

                LeadTime = GetLeadTime(processRow)
            };
        }

        private static ProcessLeadTime GetLeadTime(OrdersDataSet.PartProcessSummaryRow partProcessRow)
        {
            ProcessLeadTime leadTime = null;
            if (partProcessRow.IsLeadTimeHoursNull() || partProcessRow.IsLeadTimeTypeNull())
            {
                LOGGER.Info($"Retrieving lead time data for process {partProcessRow.ProcessID}");
                using (var dtProcess = new ProcessesDataset.ProcessDataTable())
                {
                    using (var taProcess = new Data.Datasets.ProcessesDatasetTableAdapters.ProcessTableAdapter())
                    {
                        taProcess.FillByProcess(dtProcess, partProcessRow.ProcessID);
                    }

                    var processRow = dtProcess.FirstOrDefault();
                    if (processRow != null && !processRow.IsLeadTimeHoursNull() && !processRow.IsLeadTimeTypeNull())
                    {
                        leadTime = new ProcessLeadTime(processRow.LeadTimeHours, Parse(processRow.LeadTimeType));
                    }
                }
            }
            else
            {
                leadTime = new ProcessLeadTime(partProcessRow.LeadTimeHours, Parse(partProcessRow.LeadTimeType));
            }

            return leadTime;
        }

        private static LeadTimeType Parse(string typeString)
        {
            switch (typeString.ToLowerInvariant())
            {
                case "piece":
                    return LeadTimeType.Piece;
                default:
                    return LeadTimeType.Load;
            }
        }
    }
}
