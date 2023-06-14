using DWOS.Data;
using DWOS.Services.Messages;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Order.Activity;
using DWOS.Data.Datasets;
using System.Web.Http;
using DWOS.Data.Order;

namespace DWOS.Services
{
    public class BatchProcessAnswerController: ApiController
    {
        #region Fields

        private const string PART_QUANTITY_INPUT_TYPE = "PartQty";

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves batch process answers.
        /// </summary>
        /// <remarks>
        /// This route uses POST because it can create answers for the batch's
        /// primary order.
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ServiceExceptionFilter("Error retrieving answers.")]
        public ResponseBase Retrieve(BatchProcessAnswerRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            var orderInfo = GetBatchPrimaryOrder(request.BatchId, request.BatchProcessId);

            var response = new BatchProcessAnswerResponse
            {
                Success = true,
                ErrorMessage = null,
                BatchProcessAnswers = OrderProcessAnswerController.CreateAnswers(orderInfo.OrderId, orderInfo.OrderProcessId)
            };

            AutoCompletePartQuantity(request, response);

            return response;
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "BatchOrderProcessing")]
        [ServiceExceptionFilter("Error saving answers.")]
        public ResponseBase Save(BatchProcessAnswerSaveRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            return SaveAnswers(request.UserId, request.BatchId, request.OrderProcessAnswers);
        }

        #endregion

        #region Factories

        private static BatchProcessingActivity.OrderToProcess GetBatchPrimaryOrder(int batchId, int batchProcessId)
        {
            try
            {
                return BatchProcessingActivity.GetBatchesPrimaryOrderToProcess(batchId, batchProcessId);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error batch primary order for batch Id {0}.".FormatWith(batchId));
                return new BatchProcessingActivity.OrderToProcess();
            }
        }

        private static BatchProcessAnswerSaveResponse SaveAnswers(int userId, int batchId, List<OrderProcessAnswerInfo> batchAnswers)
        {
            OrderProcessingDataSet dsOrderProcessing = null;
            Data.Datasets.OrderProcessingDataSetTableAdapters.TableAdapterManager taManager = null;

            try
            {
                _log.Debug("Saving answers for batch {0} with {1} answers", batchId, batchAnswers.Count);

                if (batchAnswers == null)
                {
                    return new BatchProcessAnswerSaveResponse() { Success = false, ErrorMessage = "Invalid data" };
                }

                dsOrderProcessing = new OrderProcessingDataSet();
                taManager = new Data.Datasets.OrderProcessingDataSetTableAdapters.TableAdapterManager();

                //Load the data
                using (var taOrderSummary = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
                {
                    taOrderSummary.FillByBatch2(dsOrderProcessing.OrderSummary, batchId);
                }

                taManager.OrderProcessAnswerTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessAnswerTableAdapter() { ClearBeforeFill = false };
                taManager.BatchTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter() { ClearBeforeFill = false };
                taManager.BatchOrderTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchOrderTableAdapter() { ClearBeforeFill = false };
                taManager.BatchProcessesTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter() { ClearBeforeFill = false };
                taManager.OrderProcessesTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter() { ClearBeforeFill = false };
                taManager.BatchProcess_OrderProcessTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcess_OrderProcessTableAdapter { ClearBeforeFill = false };

                taManager.BatchTableAdapter.FillBy(dsOrderProcessing.Batch, batchId);
                taManager.BatchOrderTableAdapter.FillBy(dsOrderProcessing.BatchOrder, batchId);
                taManager.BatchProcessesTableAdapter.FillBy(dsOrderProcessing.BatchProcesses, batchId);
                taManager.OrderProcessesTableAdapter.FillByBatch(dsOrderProcessing.OrderProcesses, batchId);
                taManager.BatchProcess_OrderProcessTableAdapter.FillByBatch(dsOrderProcessing.BatchProcess_OrderProcess, batchId);

                var batchInfo = new BatchProcessInfo(dsOrderProcessing, batchId);
                var department = batchInfo.Batch.CurrentLocation;
                var currentLine = batchInfo.Batch.IsCurrentLineNull()
                    ? (int?)null
                    : batchInfo.Batch.CurrentLine;

                CopyAnswers(batchInfo, batchAnswers);

                //save any changes thus far, because the ProcessingActivity task below queries the DB for answers
                taManager.UpdateAll(dsOrderProcessing);

                // Complete the activity for the first batch order
                var orderProcess = batchInfo.CurrentBatchProcess.GetBatchProcess_OrderProcessRows().FirstOrDefault(op => op.OrderProcessesRow.OrderID == batchInfo.BatchOrders[0].OrderID);

                if (orderProcess == null)
                {
                    return new BatchProcessAnswerSaveResponse() { Success = false, ErrorMessage = "Invalid process data" };
                }

                var primaryProcessActivity = new ProcessingActivity(batchInfo.BatchOrders[0].OrderID, new ActivityUser(userId, department, currentLine))
                {
                    OrderProcessID = orderProcess.OrderProcessID,
                    CurrentProcessedPartQty = batchInfo.BatchOrders[0].PartQuantity,
                    SkipCostUpdate = true,
                    SkipAutoCheckIn = true,
                    BatchId = batchId
                };

                var primaryProcessResults = primaryProcessActivity.Complete() as ProcessingActivity.ProcessingActivityResults;

                if (primaryProcessResults == null)
                {
                    return new BatchProcessAnswerSaveResponse() { Success = false, ErrorMessage = "Invalid process result data" };
                }

                batchInfo.ProcessingResults = primaryProcessResults;

                var userName = GetUserName(userId);
                var appSettings = ApplicationSettings.Current;

                //if closed out then process all OR if using partial loads and all answers completed
                var processIsComplete = batchInfo.ProcessingResults.IsProcessComplete ||
                    (appSettings.AllowPartialProcessLoads && batchInfo.ProcessingResults.CompletedAnswers == batchInfo.ProcessingResults.TotalAnswers);

                if (processIsComplete)
                {
                    SplitWeightAnswers(batchInfo);
                    taManager.UpdateAll(dsOrderProcessing);

                    foreach (var bo in batchInfo.BatchOrders)
                        OrderHistoryDataSet.UpdateOrderHistory(bo.OrderID, "Batch Processing", "Order processed within batch '{0}'.".FormatWith(batchId), userName);

                    var orderWorkStatuses = new List<string>();

                    foreach (var bo in batchInfo.BatchOrders)
                    {
                        orderProcess = batchInfo.CurrentBatchProcess.GetBatchProcess_OrderProcessRows().FirstOrDefault(op => op.OrderProcessesRow.OrderID == bo.OrderID);

                        if (orderProcess != null)
                        {
                            var activity = new ProcessingActivity(bo.OrderID, new ActivityUser(userId, department, currentLine))
                            {
                                OrderProcessID = orderProcess.OrderProcessID,
                                CurrentProcessedPartQty = bo.PartQuantity,
                                BatchId = batchId
                            };

                            var activityResult = activity.Complete();
                            orderWorkStatuses.Add(activityResult.WorkStatus);
                        }
                    }

                    //update the batch process
                    if (batchInfo.CurrentBatchProcess.IsStartDateNull())
                    {
                        batchInfo.CurrentBatchProcess.StartDate = DateTime.Now;
                    }

                    batchInfo.CurrentBatchProcess.EndDate = DateTime.Now;

                    var remainingProcesses = batchInfo.Batch.GetBatchProcessesRows().Count(pr => pr.IsEndDateNull());

                    var batchWorkStatus = BatchUtilities.WorkStatusForBatch(orderWorkStatuses);
                    if (BatchUtilities.CanBatchFromProcessing(remainingProcesses, batchWorkStatus))
                    {
                        // Continue batch
                        batchInfo.Batch.WorkStatus = batchWorkStatus;
                    }
                    else
                    {
                        // Close batch
                        batchInfo.Batch.WorkStatus = appSettings.WorkStatusCompleted;
                        batchInfo.Batch.Active = false;
                        batchInfo.Batch.CloseDate = DateTime.Now;

                        foreach (var bo in batchInfo.BatchOrders)
                            OrderHistoryDataSet.UpdateOrderHistory(bo.OrderID, "Batch Processing", "Batch '{0}' completed.".FormatWith(batchId), userName);
                    }
                }

                taManager.UpdateAll(dsOrderProcessing);

                if (processIsComplete)
                {
                    Data.Order.TimeCollectionUtilities.CompleteBatchProcessesTimers(batchId);
                }

                // Auto check-in
                if (!appSettings.OrderCheckInEnabled && batchInfo.Batch.WorkStatus == appSettings.WorkStatusChangingDepartment)
                {
                    var batchCheckIn = new BatchCheckInController(batchId);
                    batchCheckIn.AutoCheckIn(userId);
                }

                var response = new BatchProcessAnswerSaveResponse() { Success = true };
                if (primaryProcessResults.NextRequisiteProcessID.HasValue && primaryProcessResults.NextRequisiteHours.HasValue)
                {
                    using (var taProcess = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter())
                    {
                        var processName = taProcess.GetProcessName(primaryProcessResults.NextRequisiteProcessID.Value);
                        response.HasNextProcessTimeConstraint = true;
                        response.NextProcessTimeConstraintMessage = "Process {0} must be completed within the next {1} hours.".FormatWith(processName, primaryProcessResults.NextRequisiteHours.Value.ToString("N2"));
                    }
                }

                return response;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error saving answers for batch {0}.".FormatWith(batchId));
                return new BatchProcessAnswerSaveResponse() { Success = true, ErrorMessage = "Error saving answers for batch {0}.".FormatWith(batchId) + exc.Message };
            }
            finally
            {
                dsOrderProcessing?.Dispose();
                taManager?.Dispose();
            }
        }

        private static void SplitWeightAnswers(BatchProcessInfo batchInfo)
        {
            OrderProcessingDataSet.OrderSummaryDataTable dtOrderSummary = null;

            try
            {
                dtOrderSummary = new OrderProcessingDataSet.OrderSummaryDataTable();

                var fromAnswers = batchInfo.OrderProcessing.OrderProcessAnswer
                    .Where(opa => opa.OrderProcessesID == batchInfo.PrimaryBatchOrderProcess.OrderProcessID && opa.Completed);

                var processQuestionToWeightMap = new Dictionary<int, decimal>();

                foreach (var fromAnswer in fromAnswers)
                {
                    var isWeight = fromAnswer.ProcessQuestionRow.InputType == InputType.PreProcessWeight.ToString() ||
                        fromAnswer.ProcessQuestionRow.InputType == InputType.PostProcessWeight.ToString();

                    if (!isWeight)
                    {
                        continue;
                    }

                    decimal weight;

                    if (decimal.TryParse(fromAnswer.Answer, out weight))
                    {
                        processQuestionToWeightMap.Add(fromAnswer.ProcessQuestionID, weight);
                    }
                }

                if (processQuestionToWeightMap.Count == 0)
                {
                    return;
                }

                using (var taOrderSummary = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
                {
                    taOrderSummary.FillByBatch2(dtOrderSummary, batchInfo.Batch.BatchID);
                }

                var batchWeight = dtOrderSummary.Sum(order => order.IsWeightNull() ? 0M : order.Weight);

                foreach (var batchOrder in batchInfo.BatchOrders)
                {
                    var orderProcess = batchInfo.CurrentBatchProcess.GetBatchProcess_OrderProcessRows()
                        .FirstOrDefault(op => op.OrderProcessesRow.OrderID == batchOrder.OrderID);

                    var order = dtOrderSummary.FindByOrderID(batchOrder.OrderID);

                    if (orderProcess == null || order == null)
                    {
                        continue;
                    }

                    var toAnswers = batchInfo.OrderProcessing.OrderProcessAnswer
                        .Where(opa => opa.OrderProcessesID == orderProcess.OrderProcessID);

                    if (toAnswers.Count() == 0)
                    {
                        using (var taAnswers = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessAnswerTableAdapter())
                        {
                            taAnswers.FillByOrderProcessesID(batchInfo.OrderProcessing.OrderProcessAnswer, orderProcess.OrderProcessID);
                        }

                        toAnswers = batchInfo.OrderProcessing.OrderProcessAnswer
                            .Where(opa => opa.OrderProcessesID == orderProcess.OrderProcessID);
                    }

                    var orderWeight = order.IsWeightNull() ? 0M : order.Weight;

                    foreach (var pair in processQuestionToWeightMap)
                    {
                        var processQuestionID = pair.Key;
                        var answeredWeight = pair.Value;

                        var adjustedWeight = answeredWeight * (orderWeight / batchWeight);

                        var toAnswer = toAnswers.FirstOrDefault(opa => opa.ProcessQuestionID == processQuestionID);
                        if (toAnswer != null)
                        {
                            toAnswer.Answer = adjustedWeight.ToString();
                        }
                    }
                }
            }
            finally
            {
                dtOrderSummary?.Dispose();
            }
        }

        private static string GetUserName(int userId)
        {
            if (userId > 0)
            {
                using (var ta = new Data.Datasets.SecurityDataSetTableAdapters.UsersTableAdapter())
                {
                    var userName = ta.GetUserName(userId);

                    if (!String.IsNullOrWhiteSpace(userName))
                        return userName;
                }
            }

            if (userId == -99)
                return "System";

            return "UnKnown";
        }

        private static void CopyAnswers(BatchProcessInfo batchInfo, List<OrderProcessAnswerInfo> batchAnswers)
        {
            Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessAnswerTableAdapter taAnswers = null;

            try
            {
                _log.Debug("Copy answers for batch {0} primary order {1} with {2} answers", batchInfo.Batch, batchInfo.PrimaryBatchOrder, batchAnswers.Count);

                taAnswers = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessAnswerTableAdapter() { ClearBeforeFill = false };

                //load all process questions and steps locally
                using (var taProcessSteps = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessStepsTableAdapter())
                {
                    taProcessSteps.FillBy(batchInfo.OrderProcessing.ProcessSteps, batchInfo.CurrentBatchProcess.ProcessID);
                }

                using (var taProcessQuestion = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessQuestionTableAdapter())
                {
                    taProcessQuestion.FillBy(batchInfo.OrderProcessing.ProcessQuestion, batchInfo.CurrentBatchProcess.ProcessID);
                }

                foreach (var batchOrder in batchInfo.BatchOrders)
                {
                    var orderProcess = batchInfo.CurrentBatchProcess.GetBatchProcess_OrderProcessRows().FirstOrDefault(op => op.OrderProcessesRow.OrderID == batchOrder.OrderID);

                    if (orderProcess == null)
                        continue;

                    var orderID = batchOrder.OrderID;
                    var orderProcessID = orderProcess.OrderProcessID;

                    //create then load the process answers
                    OrderProcessingDataSet.OrderSummaryDataTable.CreateOrderAnswersWithLock(orderID);

                    taAnswers.FillByOrderProcessesID(batchInfo.OrderProcessing.OrderProcessAnswer, orderProcessID);

                    var toAnswers = batchInfo.OrderProcessing.OrderProcessAnswer.Where(opa => opa.OrderProcessesID == orderProcessID).ToList();

                    foreach (var answer in batchAnswers)
                    {
                        var toAnswer = toAnswers.FirstOrDefault(opa => opa.ProcessQuestionID == answer.ProcessQuestionId);

                        if (toAnswer == null)
                        {
                            continue;
                        }

                        //if question is part qty then set it based on this order
                        if (toAnswer.ProcessQuestionRow.InputType == InputType.PartQty.ToString())
                            toAnswer.Answer = batchOrder.PartQuantity.ToString();
                        else
                            toAnswer.Answer = answer.Answer;

                        toAnswer.Completed = answer.Completed;

                        if (answer.Completed)
                        {
                            if (answer.CompletedBy > 0)
                            {
                                toAnswer.CompletedBy = answer.CompletedBy;
                            }
                            else
                            {
                                toAnswer.SetCompletedByNull();
                            }

                            if (answer.CompletedDate > DateTime.MinValue)
                            {
                                toAnswer.CompletedData = answer.CompletedDate;
                            }
                            else
                            {
                                toAnswer.SetCompletedDataNull();
                            }
                        }
                    }
                }
            }
            finally
            {
                taAnswers?.Dispose();
            }
        }

        private static void AutoCompletePartQuantity(BatchProcessAnswerRequest request, BatchProcessAnswerResponse response)
        {

            //Need to weed out part quantity answers, don't need to answer part quantity questions for batch orders
            foreach (var bpa in response.BatchProcessAnswers)
            {
                using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessQuestionTableAdapter())
                {
                    var inputType = ta.GetProcessQuestionInputType(bpa.ProcessQuestionId);

                    if (inputType == PART_QUANTITY_INPUT_TYPE)
                    {
                        bpa.Completed = true;
                        bpa.CompletedDate = DateTime.Now;
                        bpa.CompletedBy = request.UserId;
                    }
                }
            }
        }

        #endregion

        #region BatchProcessInfo

        private class BatchProcessInfo
        {
            public OrderProcessingDataSet OrderProcessing { get; set; }
            public OrderProcessingDataSet.BatchRow Batch { get; set; }
            public OrderProcessingDataSet.BatchProcessesRow CurrentBatchProcess { get; set; }
            public OrderProcessingDataSet.BatchOrderRow PrimaryBatchOrder { get; set; }
            public ProcessingActivity.ProcessingActivityResults ProcessingResults { get; set; }
            public OrderProcessingDataSet.BatchProcess_OrderProcessRow PrimaryBatchOrderProcess { get; set; }
            public List<OrderProcessingDataSet.BatchOrderRow> BatchOrders { get; set; }

            public BatchProcessInfo(OrderProcessingDataSet dsOrderProcessing, int batchID)
            {
                OrderProcessing = dsOrderProcessing;
                Batch = dsOrderProcessing.Batch.FindByBatchID(batchID);
                BatchOrders = Batch.GetBatchOrderRows().OrderBy(bo => bo.BatchOrderID).ToList();
                PrimaryBatchOrder = BatchOrders.FirstOrDefault();
                CurrentBatchProcess = Batch.GetBatchProcessesRows().OrderBy(ob => ob.StepOrder).FirstOrDefault(r => r.IsEndDateNull());

                if (CurrentBatchProcess != null)
                    PrimaryBatchOrderProcess = CurrentBatchProcess.GetBatchProcess_OrderProcessRows().FirstOrDefault(bpop => bpop.OrderProcessesRow.OrderID == PrimaryBatchOrder.OrderID);
            }
        }

        #endregion
    }
}
