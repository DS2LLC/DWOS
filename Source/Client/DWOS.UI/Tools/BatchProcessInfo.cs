using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Data.Order;
using DWOS.Data.Order.Activity;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Tools
{
    internal class BatchProcessInfo
    {
        #region Fields

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public OrderProcessingDataSet OrderProcessing { get; set; }

        public TableAdapterManager Manager { get; set; }

        public ApplicationSettings AppSettings { get; }

        public int BatchId { get; set; }

        public OrderProcessingDataSet.BatchRow Batch { get; set; }

        public OrderProcessingDataSet.BatchProcessesRow CurrentBatchProcess { get; set; }

        public OrderProcessingDataSet.BatchOrderRow PrimaryBatchOrder { get; set; }

        public ProcessingActivity.ProcessingActivityResults ProcessingResults { get; set; }

        public OrderProcessingDataSet.BatchProcess_OrderProcessRow PrimaryBatchOrderProcess { get; set; }

        public List<OrderProcessingDataSet.BatchOrderRow> BatchOrders { get; set; }

        public bool TopMost { get; set; }

        //if closed out then process all OR if using partial loads and all answers completed
        public bool IsBatchProcessComplete => ProcessingResults.IsProcessComplete ||
                                              (AppSettings.AllowPartialProcessLoads &&
                                               ProcessingResults.CompletedAnswers == ProcessingResults.TotalAnswers);

        #endregion

        #region Methods

        private BatchProcessInfo(OrderProcessingDataSet dsOrderProcessing,
            TableAdapterManager taManager,
            ApplicationSettings appSettings,
            int batchId)
        {
            OrderProcessing = dsOrderProcessing;
            Manager = taManager;
            AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            BatchId = batchId;
            Batch = dsOrderProcessing.Batch.FindByBatchID(batchId);
            BatchOrders = Batch.GetBatchOrderRows().OrderBy(bo => bo.BatchOrderID).ToList();
            PrimaryBatchOrder = BatchOrders.FirstOrDefault();
            CurrentBatchProcess = Batch.GetBatchProcessesRows().OrderBy(ob => ob.StepOrder).FirstOrDefault(r => r.IsEndDateNull());

            if (CurrentBatchProcess != null && PrimaryBatchOrder != null)
                PrimaryBatchOrderProcess = CurrentBatchProcess.GetBatchProcess_OrderProcessRows().FirstOrDefault(bpop => bpop.OrderProcessesRow.OrderID == PrimaryBatchOrder.OrderID);
        }

        public ValidationResults Validate()
        {
            if (Batch == null)
            {
                return new ValidationResults
                {
                    IsValid = false,
                    ErrorMessage = "Batch not found."
                };
            }

            if (BatchOrders == null || BatchOrders.Count < 1)
            {
                return new ValidationResults
                {
                    IsValid = false,
                    ErrorMessage = "Batch " + Batch.BatchID + " has no orders.",
                    ErrorHeader = "No Orders"
                };
            }

            if (CurrentBatchProcess == null)
            {
                return new ValidationResults
                {
                    IsValid = false,
                    ErrorMessage = "Batch " + Batch.BatchID + " has no process that is currently in process.",
                    ErrorHeader = "No Active Process",
                    ErrorFooter = "There must be a batch process that has been started but not completed."
                };
            }

            if (PrimaryBatchOrderProcess == null)
            {
                return new ValidationResults
                {
                    IsValid = false,
                    ErrorMessage = "Unable to find process for order " + PrimaryBatchOrder.OrderID,
                    ErrorHeader = "No Order Process"
                };
            }

            return new ValidationResults
            {
                IsValid = true
            };
        }

        public static BatchProcessInfo NewBatchProcessInfo(int batchId)
        {
            var dsProcessing = new OrderProcessingDataSet();
            var taManager = new TableAdapterManager();

            //Load the data
            using (new UsingDataSetLoad(dsProcessing))
            {
                using (var taOrderSummary = new OrderSummaryTableAdapter())
                {
                    taOrderSummary.FillByBatch2(dsProcessing.OrderSummary, batchId);
                }

                taManager.OrderProcessAnswerTableAdapter = new OrderProcessAnswerTableAdapter() { ClearBeforeFill = false };
                taManager.BatchTableAdapter = new BatchTableAdapter() { ClearBeforeFill = false };
                taManager.BatchOrderTableAdapter = new BatchOrderTableAdapter() { ClearBeforeFill = false };
                taManager.BatchProcessesTableAdapter = new BatchProcessesTableAdapter() { ClearBeforeFill = false };
                taManager.OrderProcessesTableAdapter = new OrderProcessesTableAdapter() { ClearBeforeFill = false };
                taManager.BatchProcess_OrderProcessTableAdapter = new BatchProcess_OrderProcessTableAdapter
                {
                    ClearBeforeFill = false
                };

                taManager.BatchTableAdapter.FillBy(dsProcessing.Batch, batchId);
                taManager.BatchOrderTableAdapter.FillBy(dsProcessing.BatchOrder, batchId);
                taManager.BatchProcessesTableAdapter.FillBy(dsProcessing.BatchProcesses, batchId);
                taManager.OrderProcessesTableAdapter.FillByBatch(dsProcessing.OrderProcesses, batchId);
                taManager.BatchProcess_OrderProcessTableAdapter.FillByBatch(dsProcessing.BatchProcess_OrderProcess,
                    batchId);
            }

            return new BatchProcessInfo(dsProcessing, taManager, ApplicationSettings.Current, batchId);
        }

        public void SaveChanges()
        {
            Manager.UpdateAll(OrderProcessing);
        }

        public void AnswerQuestions()
        {
            //load order processing form and answer the questions
            var processingActivity = new ProcessingActivity(
                PrimaryBatchOrderProcess.OrderProcessesRow.OrderID,
                new ActivityUser(SecurityManager.Current.UserID, Properties.Settings.Default.CurrentDepartment,
                    Properties.Settings.Default.CurrentLine))
            {
                OrderProcessID = PrimaryBatchOrderProcess.OrderProcessID,
                CurrentProcessedPartQty = PrimaryBatchOrder.PartQuantity,
                SkipCostUpdate = true,
                SkipAutoCheckIn = true,
                BatchId = BatchId
            };

            using (var op = new OrderProcessing2(processingActivity) { Mode = OrderProcessing2.ProcessingMode.Batch, TopMost = TopMost })
            {
                if (op.ShowDialog(DWOSApp.MainForm) == DialogResult.OK && !op.IsProcessingCanceled && op.ActivityResults != null)
                {
                    _log.Info("Batch Order Processing completed for batch process " + CurrentBatchProcess.BatchProcessID);
                    ProcessingResults = op.ActivityResults;
                }
                else
                {
                    _log.Info("Batch Order Processing canceled for batch process " + CurrentBatchProcess.BatchProcessID);
                    MessageBoxUtilities.ShowMessageBoxWarn("Batch Processing canceled and will not be applied to the load.", "Batch Processing Canceled");
                }
            }

            //load all process questions and steps locally
            using (var taProcessSteps = new ProcessStepsTableAdapter())
            {
                taProcessSteps.FillBy(OrderProcessing.ProcessSteps, CurrentBatchProcess.ProcessID);
            }

            using (var taProcessQuestion = new ProcessQuestionTableAdapter())
            {
                taProcessQuestion.FillBy(OrderProcessing.ProcessQuestion, CurrentBatchProcess.ProcessID);
            }

            _log.Info("Batch answering questions: Order " + PrimaryBatchOrder.OrderID + " for Order Process " + PrimaryBatchOrderProcess.OrderProcessID);

            OrderProcessing.OrderProcessAnswer.Clear(); //remove existing ones since they were updated in the order processing dialog and reload

            using (var taOrderProcessAnswer = new OrderProcessAnswerTableAdapter())
            {
                taOrderProcessAnswer.FillByOrderProcessesID(OrderProcessing.OrderProcessAnswer,
                    PrimaryBatchOrderProcess.OrderProcessID);
            }

            var partQtyAnswers = OrderProcessing.OrderProcessAnswer
                .Where(opa => opa.OrderProcessesID == PrimaryBatchOrderProcess.OrderProcessID && opa.ProcessQuestionRow.InputType == InputType.PartQty.ToString());

            foreach (var pqa in partQtyAnswers)
            {
                pqa.Answer = PrimaryBatchOrder.PartQuantity.ToString();

                if (pqa.IsCompletedByNull()) //ensure all answers are answered since PartQty is skipped in Order Processing with batch
                {
                    pqa.Completed = true;
                    pqa.CompletedBy = SecurityManager.Current.UserID;
                    pqa.CompletedData = DateTime.Now;
                }
            }
        }

        public void CopyAnswers()
        {
            //copy answers to all other orders
            if (BatchOrders.Count > 1)
            {
                DoCopyAnswers();
            }

            if (IsBatchProcessComplete)
            {
                SplitWeightAnswers();
            }
        }

        public void DoCopyAnswers()
        {
            _log.Info("Copying answers for batch " + Batch.BatchID);

            var fromAnswers = OrderProcessing.OrderProcessAnswer
                .Where(opa => opa.OrderProcessesID == PrimaryBatchOrderProcess.OrderProcessID && opa.Completed)
                .ToList();

            var taAnswers = new OrderProcessAnswerTableAdapter()
            {
                ClearBeforeFill = false
            };

            foreach (var batchOrder in BatchOrders.Skip(1))
            {
                var orderProcess = CurrentBatchProcess.GetBatchProcess_OrderProcessRows()
                    .FirstOrDefault(op => op.OrderProcessesRow.OrderID == batchOrder.OrderID);

                if (orderProcess == null)
                {
                    continue;
                }

                //create then load the process answers
                OrderProcessingDataSet.OrderSummaryDataTable.CreateOrderAnswers(batchOrder.OrderID);
                taAnswers.FillByOrderProcessesID(OrderProcessing.OrderProcessAnswer, orderProcess.OrderProcessID);

                var toAnswers = OrderProcessing.OrderProcessAnswer
                    .Where(opa => opa.OrderProcessesID == orderProcess.OrderProcessID)
                    .ToList();

                foreach (var fromAnswer in fromAnswers)
                {
                    var toAnswer = toAnswers.FirstOrDefault(opa => opa.ProcessQuestionID == fromAnswer.ProcessQuestionID);

                    if (toAnswer != null && !toAnswer.Completed)
                    {
                        //if question is part qty then set it based on this order
                        var isPartQty = toAnswer.ProcessQuestionRow.InputType == InputType.PartQty.ToString();

                        if (isPartQty)
                        {
                            toAnswer.Answer = batchOrder.PartQuantity.ToString();
                        }
                        else
                        {
                            toAnswer.Answer = fromAnswer.IsAnswerNull() ?
                                null :
                                fromAnswer.Answer;
                        }

                        toAnswer.Completed = fromAnswer.Completed;

                        if (fromAnswer.IsCompletedDataNull())
                        {
                            toAnswer.SetCompletedDataNull();
                        }
                        else
                        {
                            toAnswer.CompletedData = fromAnswer.CompletedData;
                        }

                        if (fromAnswer.IsCompletedByNull())
                        {
                            toAnswer.SetCompletedByNull();
                        }
                        else
                        {
                            toAnswer.CompletedBy = fromAnswer.CompletedBy;
                        }
                    }
                }
            }
        }

        public void SplitWeightAnswers()
        {
            OrderProcessingDataSet.OrderSummaryDataTable dtOrderSummary = null;

            try
            {
                dtOrderSummary = new OrderProcessingDataSet.OrderSummaryDataTable();

                var fromAnswers = OrderProcessing.OrderProcessAnswer
                    .Where(opa => opa.OrderProcessesID == PrimaryBatchOrderProcess.OrderProcessID && opa.Completed);

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

                using (var taOrderSummary = new OrderSummaryTableAdapter())
                {
                    taOrderSummary.FillByBatch2(dtOrderSummary, Batch.BatchID);
                }

                var batchWeight = dtOrderSummary.Sum(order => order.IsWeightNull() ? 0M : order.Weight);

                foreach (var batchOrder in BatchOrders)
                {
                    var orderProcess = CurrentBatchProcess.GetBatchProcess_OrderProcessRows()
                        .FirstOrDefault(op => op.OrderProcessesRow.OrderID == batchOrder.OrderID);

                    var order = dtOrderSummary.FindByOrderID(batchOrder.OrderID);

                    if (orderProcess == null || order == null)
                    {
                        continue;
                    }

                    var toAnswers = OrderProcessing.OrderProcessAnswer
                        .Where(opa => opa.OrderProcessesID == orderProcess.OrderProcessID)
                        .ToList();

                    if (toAnswers.Count == 0)
                    {
                        using (var taAnswers = new OrderProcessAnswerTableAdapter())
                        {
                            taAnswers.FillByOrderProcessesID(OrderProcessing.OrderProcessAnswer, orderProcess.OrderProcessID);
                        }

                        toAnswers = OrderProcessing.OrderProcessAnswer
                            .Where(opa => opa.OrderProcessesID == orderProcess.OrderProcessID)
                            .ToList();
                    }

                    var orderWeight = order.IsWeightNull() ? 0M : order.Weight;

                    foreach (var pair in processQuestionToWeightMap)
                    {
                        var processQuestionId = pair.Key;
                        var answeredWeight = pair.Value;

                        var adjustedWeight = answeredWeight * (orderWeight / batchWeight);

                        var toAnswer = toAnswers.FirstOrDefault(opa => opa.ProcessQuestionID == processQuestionId);
                        if (toAnswer != null)
                        {
                            toAnswer.Answer = adjustedWeight.ToString(CultureInfo.CurrentCulture);
                        }
                    }
                }
            }
            finally
            {
                dtOrderSummary?.Dispose();
            }
        }

        public void CompleteProcessing()
        {
            if (!IsBatchProcessComplete)
            {
                return;
            }

            // During batch processing, the cost update gets skipped because
            // pre-process/post-process weights are total batch weights.
            try
            {
                Data.Order.ProcessCostUtilities.UpdateCost(PrimaryBatchOrderProcess.OrderProcessID);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating order cost.");
            }

            foreach (var bo in BatchOrders)
            {
                OrderHistoryDataSet.UpdateOrderHistory(bo.OrderID,
                    "Batch Processing",
                    string.Format("Order processed within batch '{0}'.", BatchId),
                    SecurityManager.Current.UserName);
            }

            var orderWorkStatuses = new List<string>
            {
                ProcessingResults.WorkStatus
            };

            foreach (var bo in BatchOrders.Skip(1))
            {
                var orderProcess = CurrentBatchProcess
                    .GetBatchProcess_OrderProcessRows()
                    .FirstOrDefault(op => op.OrderProcessesRow.OrderID == bo.OrderID);

                if (orderProcess != null)
                {
                    var activity = new ProcessingActivity(bo.OrderID,
                        new ActivityUser(SecurityManager.Current.UserID,
                            Properties.Settings.Default.CurrentDepartment,
                            Properties.Settings.Default.CurrentLine))
                    {
                        OrderProcessID = orderProcess.OrderProcessID,
                        CurrentProcessedPartQty = bo.PartQuantity,
                        BatchId = BatchId
                    };

                    var activityResult = activity.Complete();
                    orderWorkStatuses.Add(activityResult.WorkStatus);
                }
            }

            //update the batch process
            if (CurrentBatchProcess.IsStartDateNull())
            {
                CurrentBatchProcess.StartDate = DateTime.Now;
            }

            CurrentBatchProcess.EndDate = DateTime.Now;

            var remainingProcesses = Batch.GetBatchProcessesRows().Count(pr => pr.IsEndDateNull());
            var batchWorkStatus = BatchUtilities.WorkStatusForBatch(orderWorkStatuses);

            if (BatchUtilities.CanBatchFromProcessing(remainingProcesses, batchWorkStatus))
            {
                // Continue batch
                Batch.WorkStatus = ProcessingResults.WorkStatus;
            }
            else
            {
                // Close batch
                Batch.WorkStatus = AppSettings.WorkStatusCompleted;
                Batch.Active = false;
                Batch.CloseDate = DateTime.Now;

                foreach (var bo in BatchOrders)
                {
                    OrderHistoryDataSet.UpdateOrderHistory(bo.OrderID,
                        "Batch Processing",
                        string.Format("Batch '{0}' completed.", BatchId),
                        SecurityManager.Current.UserName);
                }

                DWOSApp.MainForm.FlyoutManager.DisplayFlyout("Batch", "Batch {0} closed".FormatWith(BatchId));
            }

            SaveChanges();

            Data.Order.TimeCollectionUtilities.CompleteBatchProcessesTimers(BatchId);

            if (!AppSettings.OrderCheckInEnabled &&
                Batch.WorkStatus == AppSettings.WorkStatusChangingDepartment)
            {
                // Auto check-in
                var batchCheckIn = new BatchCheckInController(BatchId);
                var checkInResult = batchCheckIn.AutoCheckIn(SecurityManager.Current.UserID);

                if (!checkInResult.Response)
                {
                    _log.Warn($"Auto check-in failed for batch {BatchId}.");
                }
            }
        }

        #endregion

        #region ValidationResults

        public class ValidationResults
        {
            public bool IsValid { get; set; }

            public string ErrorMessage { get; set; }

            public string ErrorHeader { get; set; }

            public string ErrorFooter { get; set; }
        }

        #endregion
    }
}