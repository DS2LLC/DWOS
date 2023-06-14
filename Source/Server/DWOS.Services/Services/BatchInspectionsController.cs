using DWOS.Data;
using DWOS.Services.Messages;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets;
using System.Web.Http;
using DWOS.Data.Order;

namespace DWOS.Services
{
    public class BatchInspectionsController : ApiController
    {
        #region Fields
        
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "ControlInspection")]
        [ServiceExceptionFilter("Error saving answers.")]
        public ResponseBase SaveCompleted(BatchInspectionSaveCompletedRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            return DoSaveCompleted(request.BatchId);
        }

        #endregion

        #region Factories

        private static ResponseBase DoSaveCompleted(int batchId)
        {
            var response = new InspectionSaveAnswerResponse();

            try
            {
                _log.Debug("Completing batch inspection for batch {0}.", batchId);

                var dsProcessing    = new OrderProcessingDataSet();
                var taManager       = new Data.Datasets.OrderProcessingDataSetTableAdapters.TableAdapterManager();

                //Load the data
                using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter()) 
                    ta.FillByBatch2(dsProcessing.OrderSummary, batchId);

                taManager.BatchTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter() { ClearBeforeFill = false };
                taManager.BatchOrderTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchOrderTableAdapter() { ClearBeforeFill = false };
                taManager.BatchProcessesTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter() { ClearBeforeFill = false };
                taManager.OrderProcessesTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter() { ClearBeforeFill = false };

                taManager.BatchTableAdapter.FillBy(dsProcessing.Batch, batchId);
                taManager.BatchOrderTableAdapter.FillBy(dsProcessing.BatchOrder, batchId);
                taManager.BatchProcessesTableAdapter.FillBy(dsProcessing.BatchProcesses, batchId);
                taManager.OrderProcessesTableAdapter.FillByBatch(dsProcessing.OrderProcesses, batchId);

                var batchInfo = new BatchInspectionInfo(dsProcessing, batchId);

                _log.Debug("Batch Info initial details => {0}.", batchInfo);

                if (!batchInfo.Validate())
                    return new InspectionSaveAnswerResponse() { Success = false, ErrorMessage = "Batch not in a valid state." };
                
                var primaryBatchOrder = batchInfo.BatchOrders.OrderBy(bo => bo.BatchOrderID).FirstOrDefault();

                if (primaryBatchOrder == null)
                    return new InspectionSaveAnswerResponse() { Success = false, ErrorMessage = "No orders in this batch." };
                var boDepartment = string.Empty;

                var appSettings = ApplicationSettings.Current;
                var orderWorkStatuses = new List<string>();
                using(var taOrder = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter())
                {
                    using(var taPartInspection = new Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionTableAdapter())
                    {
                        foreach (var batchOrder in batchInfo.BatchOrders)
                        {
                            var order = taOrder.GetByOrderID(batchOrder.OrderID)[0];

                            if (order.WorkStatus == appSettings.WorkStatusPendingReworkAssessment)
                            {
                                // Order may need to be reworked
                                batchOrder.Delete();
                                continue;
                            }

                            var dt = new PartInspectionDataSet.PartInspectionDataTable();
                            taPartInspection.FillByOrderID(dt, batchOrder.OrderID);

                            var partInspection = dt.FirstOrDefault();

                            if (partInspection != null)
                            {
                                if (partInspection.RejectedQty > 0 && partInspection.AcceptedQty == 0) // rework needed, remove from batch
                                {
                                    batchOrder.Delete();
                                }
                                else
                                {
                                    boDepartment = order.CurrentLocation;
                                }
                            }

                            orderWorkStatuses.Add(order.WorkStatus);
                        }
                    }
                }
                
                var batchOrderCount = batchInfo.Batch.GetBatchOrderRows().Count(b => b.IsValidState());
                var boWorkStatus = BatchUtilities.WorkStatusForBatch(orderWorkStatuses);

                //if no more valid orders; then close out the batch
                if (batchOrderCount < 1)
                {
                    Data.Order.TimeCollectionUtilities.StopAllBatchTimers(batchInfo.Batch.BatchID);
                    batchInfo.Batch.WorkStatus = appSettings.WorkStatusCompleted;
                    batchInfo.Batch.Active     = false;
                    batchInfo.Batch.CloseDate  = DateTime.Now;
                }
                else if (boWorkStatus == appSettings.WorkStatusPendingQI)
                {
                    // Continue performing batch inspection
                }
                else
                {
                    Data.Order.TimeCollectionUtilities.StopAllBatchTimers(batchInfo.Batch.BatchID);

                    var remainingProcesses = batchInfo.Batch.GetBatchProcessesRows().Count(pr => pr.IsEndDateNull());

                    if (BatchUtilities.CanBatchFromInspection(remainingProcesses, boWorkStatus))
                    {
                        // Continue batch
                        // Update status and location of batch to match orders.
                        // Auto check-in can change the order/batch's location.
                        batchInfo.Batch.WorkStatus = boWorkStatus;
                        batchInfo.Batch.CurrentLocation = boDepartment;
                    }
                    else
                    {
                        // Close batch
                        batchInfo.Batch.WorkStatus = appSettings.WorkStatusCompleted;
                        batchInfo.Batch.Active = false;
                        batchInfo.Batch.CloseDate = DateTime.Now;
                    }
                }

                _log.Debug("Batch Info final details => {0}.", batchInfo);
                taManager.UpdateAll(dsProcessing);

                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                return response;
            }
        }

        #endregion Factories

        #region BatchInspectionInfo

        private class BatchInspectionInfo
        {
            private OrderProcessingDataSet OrderProcessing { get; set; }
            public OrderProcessingDataSet.BatchRow Batch { get; set; }
            public OrderProcessingDataSet.BatchProcessesRow CurrentBatchProcess { get; set; }
            public List<OrderProcessingDataSet.BatchOrderRow> BatchOrders { get; set; }

            public BatchInspectionInfo(OrderProcessingDataSet dsOrderProcessing, int batchID)
            {
                NLog.LogManager.GetCurrentClassLogger().Debug("Creating BatchInspectionInfo for batch {0}.", batchID);

                OrderProcessing = dsOrderProcessing;
                Batch = dsOrderProcessing.Batch.FindByBatchID(batchID);

                if (Batch != null)
                {
                    BatchOrders = Batch.GetBatchOrderRows().OrderBy(bo => bo.BatchOrderID).ToList();
                    CurrentBatchProcess = Batch.GetBatchProcessesRows().OrderBy(ob => ob.StepOrder).FirstOrDefault(r => r.IsEndDateNull());
                }
            }

            internal bool Validate()
            {
                if (Batch == null)
                    return false;

                if (BatchOrders == null || BatchOrders.Count < 1)
                {
                    return false;
                }

                return true;
            }

            public override string ToString()
            {
                if(Batch == null)
                    return "No batch";

                return "Batch {0}; Active {1}; Status {2}, Batch Process {3}; Batch Orders {4}".FormatWith(Batch.BatchID, Batch.Active, (Batch.IsWorkStatusNull() ? "NA" : Batch.WorkStatus), (CurrentBatchProcess == null ? "NONE" : CurrentBatchProcess.BatchProcessID.ToString()), BatchOrders.Count);
            }
        }

        #endregion
    }
}
