using System.Linq;
using DWOS.Data;
using DWOS.Services.Messages;
using System;
using System.Collections.Generic;
using DWOS.Data.Datasets;
using System.Web.Http;
using DWOS.Shared.Utilities;

namespace DWOS.Services
{
    public class BatchController : ApiController
    {
        #region Methods

        [HttpGet]
        [ServiceExceptionFilter("Error getting all batches.")]
        public BatchesResponse GetAll()
        {
            using (new UsingTimeMe("/batch/getall"))
            {
                return new BatchesResponse
                {
                    Success = true,
                    ErrorMessage = null,
                    Batches = CreateBatchSummaries()
                };
            }
        }

        [HttpGet]
        [ServiceExceptionFilter("Error getting a batch.")]
        public ResponseBase Get(int batchId)
        {
            return new BatchDetailResponse
            {
                Success = true,
                ErrorMessage = null,
                BatchDetail = CreateBatchDetail(batchId)
            };
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "PartCheckIn")]
        [ServiceExceptionFilter("Error checking in batch.")]
        public ResponseBase Checkin(BatchCheckInRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            using (new UsingTimeMe("/batch/checkin"))
            {
                var batchCheckIn = new BatchCheckInController(request.BatchId);
                var checkInResponse = batchCheckIn.CheckIn(request.NextDepartment, request.UserId);

                var response = new ResponseBase { Success = checkInResponse.Response, ErrorMessage = null };

                if (!checkInResponse.Response)
                    response.ErrorMessage = checkInResponse.Description;

                return response;
            }
        }

        [HttpGet]
        [ServiceExceptionFilter("Error getting batch schedule.")]
        public ResponseBase Schedule(string department)
        {
            using (new UsingTimeMe("/batch/schedule"))
            {
                return new BatchScheduleResponse { Success = true, ErrorMessage = null, Schedule = CreateSchedule(department) };
            }
        }

        #endregion

        #region Factories

        private static BatchDetailInfo CreateBatchDetail(int batchId)
        {
            try
            {
                using (var taBatch = new Data.Datasets.OrderStatusDataSetTableAdapters.BatchStatusTableAdapter())
                {
                    var batchStatusTable = new Data.Datasets.OrderStatusDataSet.BatchStatusDataTable();

                    taBatch.FillActive(batchStatusTable);

                    var batchStatus = batchStatusTable.FindByBatchID(batchId);

                    if (batchStatus == null)
                        return null;

                    var batch = new BatchDetailInfo();
                    batch.BatchId = batchStatus.BatchID;
                    batch.Fixture = batchStatus.IsFixtureNull() ? null : batchStatus.Fixture;
                    batch.Location = batchStatus.CurrentLocation;

                    batch.CurrentLine = batchStatus.IsCurrentLineNull()
                        ? null
                        : GetProcessingLineName(batchStatus.CurrentLine);

                    batch.WorkStatus = batchStatus.IsWorkStatusNull() ? null : batchStatus.WorkStatus;
                    batch.OpenDate = batchStatus.OpenDate;
                    batch.NextDept = batchStatus.IsNextDeptNull() ? null : batchStatus.NextDept;
                    batch.CurrentProcess = batchStatus.IsCurrentProcessNull() ? null : batchStatus.CurrentProcess;
                    batch.PartCount = batchStatus.IsPartCountNull() ? 0 : batchStatus.PartCount;
                    batch.OrderCount = batchStatus.IsOrderCountNull() ? 0 : batchStatus.OrderCount;
                    batch.TotalSurfaceArea = batchStatus.IsTotalSurfaceAreaNull() ? 0 : batchStatus.TotalSurfaceArea;
                    batch.TotalWeight = batchStatus.IsTotalWeightNull() ? 0 : batchStatus.TotalWeight;
                    batch.SchedulePriority = batchStatus.SchedulePriority;
                    batch.SalesOrderId = batchStatus.IsSalesOrderIDNull()
                        ? (int?)null
                        : batchStatus.SalesOrderID;

                    using(var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchOrderTableAdapter())
                    {
                        var batchOrdersTable = new Data.Datasets.OrderProcessingDataSet.BatchOrderDataTable();
                        ta.FillBy(batchOrdersTable, batchId);

                        batch.Orders = batchOrdersTable.Select(r => r.OrderID).ToList();
                    }

                    return batch;
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error creating batch details for batch " + batchId);
                return null;
            }
        }

        private static List<BatchInfo> CreateBatchSummaries()
        {
            var batches = new List<BatchInfo>();
            
            try
            {
                using(var taBatch = new Data.Datasets.OrderStatusDataSetTableAdapters.BatchStatusTableAdapter())
                {
                    var batchStatusTable = new Data.Datasets.OrderStatusDataSet.BatchStatusDataTable();

                    taBatch.FillActive(batchStatusTable);

                    foreach(var batchStatus in batchStatusTable)
                    {
                        var batch = new BatchInfo();
                        batch.BatchId = batchStatus.BatchID;
                        batch.Fixture = batchStatus.IsFixtureNull() ? null : batchStatus.Fixture;
                        batch.Location = batchStatus.CurrentLocation;

                        batch.CurrentLine = batchStatus.IsCurrentLineNull()
                            ? null
                            : GetProcessingLineName(batchStatus.CurrentLine);

                        batch.WorkStatus = batchStatus.IsWorkStatusNull() ? null : batchStatus.WorkStatus;

                        batch.SchedulePriority = batchStatus.SchedulePriority;

                        batches.Add(batch);
                    }
                }

                return batches;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error creating batch summaries");
                return null;
            }
        }

        private static BatchSchedule CreateSchedule(string department)
        {
            List<int> batchIds;
            using (var dtBatchStatus = new OrderStatusDataSet.BatchStatusDataTable())
            {
                using (var taBatchStatus = new Data.Datasets.OrderStatusDataSetTableAdapters.BatchStatusTableAdapter())
                {
                    taBatchStatus.FillActive(dtBatchStatus);
                }

                IEnumerable<OrderStatusDataSet.BatchStatusRow> scheduledOrders;

                if (string.IsNullOrEmpty(department))
                {
                    scheduledOrders = dtBatchStatus
                        .Where(o => o.SchedulePriority > 0 && (o.WorkStatus == ApplicationSettings.Current.WorkStatusInProcess || o.WorkStatus == ApplicationSettings.Current.WorkStatusChangingDepartment));
                }
                else
                {
                    var inProcess = dtBatchStatus
                        .Where(o => o.CurrentLocation == department && o.WorkStatus == ApplicationSettings.Current.WorkStatusInProcess);

                    var checkIn = dtBatchStatus
                        .Where(o => !o.IsNextDeptNull() && o.NextDept == department && o.WorkStatus == ApplicationSettings.Current.WorkStatusChangingDepartment);

                    scheduledOrders = inProcess.Concat(checkIn)
                        .Where(o => o.SchedulePriority > 0);
                }

                batchIds = scheduledOrders
                    .OrderBy(o => o.SchedulePriority)
                    .Select(o => o.BatchID)
                    .ToList();
            }

            return new BatchSchedule
            {
                BatchIds = batchIds
            };
        }

        #endregion

        private static string GetProcessingLineName(int processingLineId)
        {
            using (var taProcessingLine = new Data.Datasets.OrderStatusDataSetTableAdapters.ProcessingLineTableAdapter())
            {
                return taProcessingLine.GetById(processingLineId).FirstOrDefault()?.Name;
            }
        }
    }
}