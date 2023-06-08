using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Data.Order;
using System;
using System.Linq;

namespace DWOS.Data
{
    /// <summary>
    /// Controls batch check-ins.
    /// </summary>
    public class BatchCheckInController
    {
        #region Properties

        public int BatchId { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="BatchCheckInController"/> class.
        /// </summary>
        /// <param name="batchId"></param>
        public BatchCheckInController(int batchId) { this.BatchId = batchId; }

        /// <summary>
        /// Determines if the next department is a valid destination for the batch.
        /// </summary>
        /// <param name="nextDepartment"></param>
        /// <param name="processingLineId"></param>
        /// <returns>
        /// <see cref="ResponseInfo{bool}"/> instance with more information.
        /// </returns>
        public ResponseInfo<bool> IsValid(string nextDepartment, int? processingLineId)
        {
            try
            {
                var batchTable = new Data.Datasets.OrderProcessingDataSet.BatchDataTable();

                using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
                    ta.FillBy(batchTable, BatchId);

                var batchRow = batchTable.FindByBatchID(BatchId);

                if(batchRow == null)
                    return new ResponseInfo<bool>(false, "The batch {0} could not be found.".FormatWith(BatchId));

                //Is Correct Work Status
                if (batchRow.IsWorkStatusNull() || batchRow.WorkStatus != ApplicationSettings.Current.WorkStatusChangingDepartment)
                    return new ResponseInfo<bool>(false, "The batch {0} work status is not set to '{1}'.".FormatWith(BatchId, ApplicationSettings.Current.WorkStatusChangingDepartment));

                // Has processes remaining
                var processes = new OrderProcessingDataSet.BatchProcessesDataTable();

                using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter())
                    ta.FillBy(processes, BatchId);

                var currentProcess = processes.OrderBy(o => o.StepOrder).FirstOrDefault(item => item.IsEndDateNull());

                if (currentProcess == null)
                    return new ResponseInfo<bool>(false, "All processes for batch {0} have been completed.".FormatWith(BatchId));

                // Is correct department
                if (currentProcess.Department != nextDepartment)
                    return new ResponseInfo<bool>(false, "The batch '{0}' is required to be checked into the '{1}' department.".FormatWith(BatchId, currentProcess.Department));

                // Is correct processing line
                if (ApplicationSettings.Current.MultipleLinesEnabled && !batchRow.IsCurrentLineNull() && batchRow.CurrentLine != processingLineId)
                {
                    return new ResponseInfo<bool>(false, $"The batch '{BatchId}' must be checked into the correct line.");
                }

                return new ResponseInfo<bool>(true);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error validating batch check in for batch '{0}'.".FormatWith(this.BatchId));
                return new ResponseInfo<bool>(false, exc.ToString());
            }
        }

        /// <summary>
        /// Checks the batch into the given department with the given user.
        /// </summary>
        /// <param name="nextDepartment"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResponseInfo<bool> CheckIn(string nextDepartment, int userId)
        {
            try
            {
                using (var taBatch = new BatchTableAdapter())
                {
                    var currentLine = taBatch.GetCurrentLine(BatchId);
                    var isValid = IsValid(nextDepartment, currentLine);

                    if (!isValid.Response)
                        return isValid;
                }

                return DoCheckIn(nextDepartment, userId);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error checking in batch {0}.".FormatWith(BatchId));
                return new ResponseInfo<bool>(false, exc.ToString());
            }
        }

        public ResponseInfo<bool> AutoCheckIn(int userId)
        {
            try
            {
                var processes = new OrderProcessingDataSet.BatchProcessesDataTable();

                using (var ta = new BatchProcessesTableAdapter())
                {
                    ta.FillBy(processes, BatchId);
                }

                var currentProcess = processes.OrderBy(o => o.StepOrder).FirstOrDefault(item => item.IsEndDateNull());

                if (currentProcess == null)
                    return new ResponseInfo<bool>(false, "All processes for batch {0} have been completed.".FormatWith(BatchId));

                return DoCheckIn(currentProcess.Department, userId);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger()
                    .Error(exc, $"Error automatically checking in batch {BatchId}.");

                return new ResponseInfo<bool>(false, exc.ToString());
            }
        }

        private ResponseInfo<bool> DoCheckIn(string nextDepartment, int userId)
        {
            using (var taManager = new TableAdapterManager())
            {
                var dsOrderProcessing = new OrderProcessingDataSet();

                taManager.BatchTableAdapter = new BatchTableAdapter {ClearBeforeFill = false};
                taManager.BatchOrderTableAdapter = new BatchOrderTableAdapter {ClearBeforeFill = false};
                taManager.BatchProcessesTableAdapter = new BatchProcessesTableAdapter {ClearBeforeFill = false};
                taManager.OrderProcessesTableAdapter = new OrderProcessesTableAdapter {ClearBeforeFill = false};
                taManager.BatchProcess_OrderProcessTableAdapter = new BatchProcess_OrderProcessTableAdapter
                {
                    ClearBeforeFill = false
                };

                taManager.BatchTableAdapter.FillBy(dsOrderProcessing.Batch, BatchId);
                taManager.BatchOrderTableAdapter.FillBy(dsOrderProcessing.BatchOrder, BatchId);
                taManager.BatchProcessesTableAdapter.FillBy(dsOrderProcessing.BatchProcesses, BatchId);
                taManager.OrderProcessesTableAdapter.FillByBatch(dsOrderProcessing.OrderProcesses, BatchId);

                var batchRow = dsOrderProcessing.Batch.FindByBatchID(BatchId);

                if (batchRow != null)
                {
                    //update work status and current location
                    batchRow.WorkStatus = ApplicationSettings.Current.WorkStatusInProcess;
                    batchRow.CurrentLocation = nextDepartment;

                    //set first process that is not completed to started now
                    var currentBatchProcess =
                        dsOrderProcessing.BatchProcesses.OrderBy(o => o.StepOrder).FirstOrDefault(item => item.IsEndDateNull());

                    if (currentBatchProcess != null)
                    {
                        taManager.BatchProcess_OrderProcessTableAdapter.FillBy(dsOrderProcessing.BatchProcess_OrderProcess,
                            currentBatchProcess.BatchProcessID);

                        currentBatchProcess.StartDate = DateTime.Now;

                        using (var taOrder = new OrderSummaryTableAdapter())
                        {
                            foreach (var batchOrderProcess in dsOrderProcessing.BatchProcess_OrderProcess)
                            {
                                var orderProcess = batchOrderProcess?.OrderProcessesRow;
                                if (orderProcess != null)
                                {
                                    var orderId = orderProcess.OrderID;

                                    if (dsOrderProcessing.BatchOrder.All(bo => bo.OrderID != orderId))
                                    {
                                        // Skip Work Orders that are no longer in the batch.
                                        continue;
                                    }

                                    orderProcess.StartDate = DateTime.Now;
                                    taOrder.UpdateWorkStatus(ApplicationSettings.Current.WorkStatusInProcess, orderId);
                                    taOrder.UpdateOrderLocation(nextDepartment, orderId);

                                    var successMessage = $"Order {orderId} checked in to {nextDepartment}.";
                                    var userName = "Unknown";

                                    if (userId > 0)
                                    {
                                        using (var taUser = new Datasets.SecurityDataSetTableAdapters.UsersTableAdapter())
                                            userName = taUser.GetUserName(userId);
                                    }

                                    //update order history
                                    OrderHistoryDataSet.UpdateOrderHistory(orderId, "Batch CheckIn", successMessage,
                                        userName);
                                }
                            }
                        }
                    }

                    //update batch and batch processes
                    taManager.UpdateAll(dsOrderProcessing);
                    TimeCollectionUtilities.StopAllBatchTimers(BatchId);

                    return new ResponseInfo<bool>(true, $"Batch '{BatchId}' checked in to '{nextDepartment}'.");
                }
                else
                {
                    return new ResponseInfo<bool>(false, $"Batch '{BatchId}' did not check in to '{nextDepartment}'.");
                }
            }
        }

        #endregion
    }
}
