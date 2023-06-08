using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets;
using NLog;

namespace DWOS.Data
{
    public class OrderProcessLeadTimes
    {
        #region Fields

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<int, ProcessLeadTime> _leadTimes =
            new Dictionary<int, ProcessLeadTime>();

        #endregion

        #region Properties

        public static OrderProcessLeadTimes Empty => new OrderProcessLeadTimes();

        #endregion

        #region Methods

        public decimal LeadTimeHoursFor(int orderProcessesId, int quantity)
        {
            _leadTimes.TryGetValue(orderProcessesId, out var leadTime);
            return leadTime?.CalculateHours(quantity) ?? 0M;
        }

        public ProcessLeadTime LeadTimeFor(int orderProcessesId)
        {
            _leadTimes.TryGetValue(orderProcessesId, out var leadTime);
            return leadTime;
        }

        public void Add(int orderProcessesID, OrdersDataSet.PartProcessSummaryRow partProcessRow)
        {
            if (partProcessRow.IsLeadTimeHoursNull() || partProcessRow.IsLeadTimeTypeNull())
            {
                _log.Info($"Retrieving lead time data for process {partProcessRow.ProcessID}");
                using (var dtProcess = new ProcessesDataset.ProcessDataTable())
                {
                    using (var taProcess = new Datasets.ProcessesDatasetTableAdapters.ProcessTableAdapter())
                    {
                        taProcess.FillByProcess(dtProcess, partProcessRow.ProcessID);
                    }

                    var processRow = dtProcess.FirstOrDefault();
                    if (processRow != null && !processRow.IsLeadTimeHoursNull() && !processRow.IsLeadTimeTypeNull())
                    {
                        _leadTimes.Add(orderProcessesID, new ProcessLeadTime(processRow.LeadTimeHours, Parse(processRow.LeadTimeType)));
                    }
                }
            }
            else
            {
                _leadTimes.Add(orderProcessesID, new ProcessLeadTime(partProcessRow.LeadTimeHours, Parse(partProcessRow.LeadTimeType)));
            }
        }

        public void Add(int orderProcessesID, ProcessesDataset.ProcessRow processRow)
        {
            if (processRow.IsLeadTimeHoursNull() || processRow.IsLeadTimeTypeNull())
            {
                return;
            }

            _leadTimes.Add(orderProcessesID, new ProcessLeadTime(processRow.LeadTimeHours, Parse(processRow.LeadTimeType)));
        }

        public void AddForTesting(int orderProcessesId, ProcessLeadTime leadTime)
        {
            if (leadTime == null)
            {
                return;
            }

            _leadTimes.Add(orderProcessesId, leadTime);
        }

        private LeadTimeType Parse(string typeString)
        {
            switch(typeString.ToLowerInvariant())
            {
                case "piece":
                    return LeadTimeType.Piece;
                default:
                    return LeadTimeType.Load;
            }
        }

        #endregion
    }
}
