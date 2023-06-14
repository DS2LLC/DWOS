using DWOS.Data.Order.Activity;
using DWOS.Services.Messages;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DWOS.Services
{
    public class BatchProcessesController : ApiController
    {
        #region Methods

        [HttpGet]
        [ServiceExceptionFilter("Error getting batch process details.")]
        public ResponseBase Get(int batchId)
        {
            return new BatchProcessesResponse
            {
                Success = true,
                ErrorMessage = null,
                BatchProcesses = Create(batchId),
                BatchStatus = CreateBatchStatus(batchId)
            };
        }

        [HttpGet]
        [ServiceExceptionFilter("Error getting current batch process.")]
        public ResponseBase GetBatchCurrent(int batchId)
        {
            var batchData = CreateCurrentProcessByBatch(batchId);

            if (batchData == null)
            {
                return new ResponseBase() { ErrorMessage = $"Could not find process for batch {batchId}.", Success = false };
            }
            else
            {
                return new BatchCurrentProcessResponse
                {
                    Success = true,
                    ErrorMessage = null,
                    OrderProcessId = batchData.Item1,
                    BatchProcessId = batchData.Item2,
                    Process = batchData.Item3
                };
            }
        }

        #endregion

        #region Factories

        private static List<BatchProcessInfo> Create(int batchId)
        {
            var batchProcesses = new List<BatchProcessInfo>();
            Data.Datasets.OrderProcessingDataSet.BatchProcessesDataTable dtBatchProceses = null;
            Data.Datasets.OrderProcessingDataSet.BatchProcess_OrderProcessDataTable dtBatchProcessOrderProcess = null;
            Data.Datasets.OrderProcessingDataSet.OrderProcessesDataTable dtOrderProcesses = null;
            Data.Datasets.OrderProcessingDataSet.ProcessDataTable dtProceses = null;
            Data.Datasets.OrderProcessingDataSet.ProcessAliasDataTable dtProcessAliases = null;

            try
            {
                dtBatchProceses = new Data.Datasets.OrderProcessingDataSet.BatchProcessesDataTable();
                dtBatchProcessOrderProcess = new Data.Datasets.OrderProcessingDataSet.BatchProcess_OrderProcessDataTable();
                dtOrderProcesses = new Data.Datasets.OrderProcessingDataSet.OrderProcessesDataTable();
                dtProceses = new Data.Datasets.OrderProcessingDataSet.ProcessDataTable();
                dtProcessAliases = new Data.Datasets.OrderProcessingDataSet.ProcessAliasDataTable();

                using (var taBatch = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter())
                {
                    taBatch.FillBy(dtBatchProceses, batchId);
                }

                using (var taBatchProcessOrderProcess = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcess_OrderProcessTableAdapter())
                {
                    taBatchProcessOrderProcess.FillByBatch(dtBatchProcessOrderProcess, batchId);
                }

                using (var taOrderProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                {
                    taOrderProcesses.FillByBatch(dtOrderProcesses, batchId);
                }

                using (var taProcess = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter())
                {
                    taProcess.FillByBatch(dtProceses, batchId);
                }

                using (var taProcessAlias = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessAliasTableAdapter { ClearBeforeFill = false })
                {
                    foreach (var processAliasId in dtOrderProcesses.Select(row => row.ProcessAliasID).Distinct())
                    {
                        taProcessAlias.FillByProcessAlias(dtProcessAliases, processAliasId);
                    }
                }

                foreach (var batchProcessRow in dtBatchProceses)
                {
                    // Create batch process info
                    var bpi = new BatchProcessInfo();
                    bpi.BatchId = batchProcessRow.BatchID;
                    bpi.BatchProcessId = batchProcessRow.BatchProcessID;
                    bpi.Department = batchProcessRow.Department;
                    bpi.Ended = batchProcessRow.IsEndDateNull() ? DateTime.MinValue : batchProcessRow.EndDate;
                    bpi.Started = batchProcessRow.IsStartDateNull() ? DateTime.MinValue : batchProcessRow.StartDate;
                    bpi.StepOrder = batchProcessRow.StepOrder;
                    bpi.ProcessId = batchProcessRow.ProcessID;

                    // Process name
                    var process = dtProceses.FindByProcessID(batchProcessRow.ProcessID);
                    bpi.ProcessName = process == null ? "Unknown" : process.Name;

                    // Process alias names
                    var orderProcesses = dtBatchProcessOrderProcess
                        .Where(row => row.BatchProcessID == batchProcessRow.BatchProcessID)
                        .Select(row => dtOrderProcesses.FindByOrderProcessesID(row.OrderProcessID));

                    var aliases = orderProcesses.Select(row => row.ProcessAliasID).Distinct()
                        .Select(dtProcessAliases.FindByProcessAliasID);

                    bpi.ProcessAliasNames = aliases.Select(row => row.Name)
                        .OrderBy(name => name)
                        .ToList();

                    // Add batch process info
                    batchProcesses.Add(bpi);
                }

                return batchProcesses;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting batch processes for batch id '{0}'.".FormatWith(batchId));
                return null;
            }
            finally
            {
                dtBatchProceses?.Dispose();
                dtBatchProcessOrderProcess?.Dispose();
                dtOrderProcesses?.Dispose();
                dtProceses?.Dispose();
                dtProcessAliases?.Dispose();
            }
        }

        private static BatchStatusInfo CreateBatchStatus(int batchId)
        {
            var batch = new BatchStatusInfo();

            try
            {
                using (var taBatch = new Data.Datasets.OrderStatusDataSetTableAdapters.BatchStatusTableAdapter())
                {
                    var batchStatusTable = new Data.Datasets.OrderStatusDataSet.BatchStatusDataTable();

                    taBatch.FillActive(batchStatusTable);

                    var batchStatus = batchStatusTable.FindByBatchID(batchId);

                    if (batchStatus != null)
                    {
                        batch.BatchId = batchStatus.BatchID;
                        batch.Location = batchStatus.CurrentLocation;
                        batch.CurrentLine = batchStatus.IsCurrentLineNull() ? (int?)null : batchStatus.CurrentLine;
                        batch.WorkStatus = batchStatus.IsWorkStatusNull() ? null : batchStatus.WorkStatus;
                        batch.NextDepartment = batchStatus.IsNextDeptNull() ? null : batchStatus.NextDept;
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error creating batch status info for batch " + batchId);
            }

            return batch;
        }

        /// <summary>
        /// Creates the current process for the batch.
        /// </summary>
        /// <param name="batchID">The batch identifier.</param>
        /// <returns>Order Process ID and the ProcessInfo.</returns>
        private static Tuple<int, int, ProcessInfo> CreateCurrentProcessByBatch(int batchID)
        {
            try
            {
                var orderToProcessInfo = BatchProcessingActivity.GetBatchesPrimaryOrderToProcess(batchID);
                
                if (orderToProcessInfo != null)
                    return new Tuple<int, int, ProcessInfo>(orderToProcessInfo.OrderProcessId, orderToProcessInfo.BatchProcessId, ServiceUtilities.CreateInfoForProcess(orderToProcessInfo.ProcessId));

                return null;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error creating process info from order.");
                return null;
            }
        }

        #endregion
    }
}
