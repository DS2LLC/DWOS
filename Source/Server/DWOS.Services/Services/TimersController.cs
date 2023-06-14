using System.Linq;
using DWOS.Services.Messages;
using System;
using DWOS.Data.Order;
using DWOS.Data;
using DWOS.Data.Datasets;
using System.Web.Http;

namespace DWOS.Services
{
    public class TimersController : ApiController
    {
        #region Methods

        [HttpGet]
        [ServiceExceptionFilter("Error finding order timer.")]
        public ResponseBase OrderProcessInfo(int orderId, int userId)
        {
            return GetOrderTimerInfo(orderId, userId);
        }

        [HttpGet]
        [ServiceExceptionFilter("Error finding order timer.")]
        public ResponseBase BatchProcessInfo(int batchId, int userId)
        {
            return GetBatchTimerInfo(batchId, userId);
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "OrderProcessing,ControlInspection")]
        [ServiceExceptionFilter("Error starting order timer.")]
        public ResponseBase StartOrderProcess(OrderTimerRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            return DoStartOrderTimer(request.OrderId, request.UserId);
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "OrderProcessing,ControlInspection")]
        [ServiceExceptionFilter("Error stopping order timer.")]
        public ResponseBase StopOrderProcess(OrderTimerRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            return DoStopOrderTimer(request.OrderId, request.UserId);
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "OrderProcessing,ControlInspection")]
        [ServiceExceptionFilter("Error starting order timer.")]
        public ResponseBase StartOrderLabor(OrderTimerRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            return DoStartOrderLaborTimer(request.OrderId, request.UserId);
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "OrderProcessing,ControlInspection")]
        [ServiceExceptionFilter("Error stopping order timer.")]
        public ResponseBase StopOrderLabor(OrderTimerRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            return DoStopOrderLaborTimer(request.OrderId, request.UserId);
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "OrderProcessing,ControlInspection")]
        [ServiceExceptionFilter("Error pausing order timer.")]
        public ResponseBase PauseOrderLabor(OrderTimerRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            return DoPauseOrderLaborTimer(request.OrderId, request.UserId);
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "BatchOrderProcessing,ControlInspection")]
        [ServiceExceptionFilter("Error starting batch timer.")]
        public ResponseBase StartBatchProcess(BatchTimerRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            return DoStartBatchTimer(request.BatchId, request.UserId);
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "BatchOrderProcessing,ControlInspection")]
        [ServiceExceptionFilter("Error stopping batch timer.")]
        public ResponseBase StopBatchProcess(BatchTimerRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            return DoStopBatchTimer(request.BatchId, request.UserId);
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "BatchOrderProcessing,ControlInspection")]
        [ServiceExceptionFilter("Error starting batch timer.")]
        public ResponseBase StartBatchLabor(BatchTimerRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            return DoStartBatchLaborTimer(request.BatchId, request.UserId);
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "BatchOrderProcessing,ControlInspection")]
        [ServiceExceptionFilter("Error stopping batch timer.")]
        public ResponseBase StopBatchLabor(BatchTimerRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            return DoStopBatchLaborTimer(request.BatchId, request.UserId);
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "BatchOrderProcessing,ControlInspection")]
        [ServiceExceptionFilter("Error stopping batch timer.")]
        public ResponseBase PauseBatchLabor(BatchTimerRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            return DoPauseBatchLaborTimer(request.BatchId, request.UserId);
        }

        #region Factory Methods

        private static TimerInfoResponse GetOrderTimerInfo(int orderId, int userId)
        {
            using (var dtOrder = new Data.Reports.OrdersReport.OrderDataTable())
            {
                using (var taOrder = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
                {
                    taOrder.FillByOrder(dtOrder, orderId);
                }

                var orderRow = dtOrder.FirstOrDefault();

                if (orderRow == null)
                {
                    return new TimerInfoResponse { Success = false, ErrorMessage = "Order not found." };
                }

                int? currentOrderProcessId;
                using (var taOrderProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                {
                    currentOrderProcessId = taOrderProcesses.GetOrderProcessesID(orderId, orderRow.WorkStatus);
                }

                if (!currentOrderProcessId.HasValue)
                {
                    return new TimerInfoResponse { Success = false, ErrorMessage = "Order Process not found." };
                }

                return GetInfoForOrderProcess(orderId, userId, currentOrderProcessId.Value);
            }
        }

        private static TimerInfoResponse GetBatchTimerInfo(int batchId, int userId)
        {
            string workStatus;
            using (var taBatch = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
            {
                workStatus = taBatch.GetWorkStatus(batchId);
            }

            int batchProcessId;
            using (var taBatchProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter())
            {
                batchProcessId = taBatchProcesses.GetBatchProcessesID(batchId, workStatus);
            }

            if (batchProcessId <= 0)
            {
                return new TimerInfoResponse { Success = false, ErrorMessage = "Batch Process not found." };
            }

            return GetInfoForBatchProcess(batchId, userId, batchProcessId);
        }

        private static ResponseBase DoStartOrderTimer(int orderId, int userId)
        {
            using (var dtOrder = new Data.Reports.OrdersReport.OrderDataTable())
            {
                using (var taOrder = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
                {
                    taOrder.FillByOrder(dtOrder, orderId);
                }

                var orderRow = dtOrder.FirstOrDefault();

                if (orderRow == null)
                {
                    return ResponseBase.Error("Order not found.");
                }

                int? currentOrderProcessId;
                using (var taOrderProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                {
                    currentOrderProcessId = taOrderProcesses.GetOrderProcessesID(orderId, orderRow.WorkStatus);
                }

                if (!currentOrderProcessId.HasValue)
                {
                    return ResponseBase.Error("Order Process not found.");
                }

                bool isInBatch;
                using (var taBatch = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
                {
                    isInBatch = Convert.ToBoolean(taBatch.GetIsOrderInActiveBatch(orderId) ?? 0);
                }

                var correctRole = GetRoleForOrder(orderRow.WorkStatus);

                var timerInfo = GetInfoForOrderProcess(orderId, userId, currentOrderProcessId.Value);

                if (isInBatch || timerInfo.HasActiveProcessTimer || !UserHasRole(userId, correctRole))
                {
                    return ResponseBase.Error("Unable to start timer at this time.");
                }
                else
                {
                    TimeCollectionUtilities.StartOrderProcessTimer(orderId);
                    return new ResponseBase
                    {
                        Success = true
                    };
                }
            }
        }

        private static ResponseBase DoStopOrderTimer(int orderId, int userId)
        {
            using (var dtOrder = new Data.Reports.OrdersReport.OrderDataTable())
            {
                using (var taOrder = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
                {
                    taOrder.FillByOrder(dtOrder, orderId);
                }

                var orderRow = dtOrder.FirstOrDefault();

                if (orderRow == null)
                {
                    return ResponseBase.Error("Order not found.");
                }

                int? currentOrderProcessId;
                using (var taOrderProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                {
                    currentOrderProcessId = taOrderProcesses.GetOrderProcessesID(orderId, orderRow.WorkStatus);
                }

                if (!currentOrderProcessId.HasValue)
                {
                    return ResponseBase.Error("Order Process not found.");
                }

                bool isInBatch;
                using (var taBatch = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
                {
                    isInBatch = Convert.ToBoolean(taBatch.GetIsOrderInActiveBatch(orderId) ?? 0);
                }

                var correctRole = GetRoleForOrder(orderRow.WorkStatus);

                var timerInfo = GetInfoForOrderProcess(orderId, userId, currentOrderProcessId.Value);

                if (isInBatch || !timerInfo.HasActiveProcessTimer || !UserHasRole(userId, correctRole))
                {
                    return ResponseBase.Error("Unable to stop timer at this time.");
                }
                else
                {
                    TimeCollectionUtilities.StopOrderProcessTimer(orderId);
                    return new ResponseBase
                    {
                        Success = true
                    };
                }
            }
        }

        private static ResponseBase DoStartOrderLaborTimer(int orderId, int userId)
        {
            using (var dtOrder = new Data.Reports.OrdersReport.OrderDataTable())
            {
                using (var taOrder = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
                {
                    taOrder.FillByOrder(dtOrder, orderId);
                }

                var orderRow = dtOrder.FirstOrDefault();

                if (orderRow == null)
                {
                    return ResponseBase.Error("Order not found.");
                }

                int? currentOrderProcessId;
                using (var taOrderProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                {
                    currentOrderProcessId = taOrderProcesses.GetOrderProcessesID(orderId, orderRow.WorkStatus);
                }

                if (!currentOrderProcessId.HasValue)
                {
                    return ResponseBase.Error("Order Process not found.");
                }

                bool isInBatch;
                using (var taBatch = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
                {
                    isInBatch = Convert.ToBoolean(taBatch.GetIsOrderInActiveBatch(orderId) ?? 0);
                }

                var correctRole = GetRoleForOrder(orderRow.WorkStatus);

                var timerInfo = GetInfoForOrderProcess(orderId, userId, currentOrderProcessId.Value);

                if (isInBatch || timerInfo.HasActiveLaborTimer || !UserHasRole(userId, correctRole))
                {
                    return ResponseBase.Error("Unable to start timer at this time.");
                }
                else
                {
                    TimeCollectionUtilities.StartOrderLaborTimer(orderId, userId);
                    TimeCollectionUtilities.StartOrderProcessTimer(orderId);
                    return new ResponseBase
                    {
                        Success = true
                    };
                }
            }
        }

        private static ResponseBase DoStopOrderLaborTimer(int orderId, int userId)
        {
            using (var dtOrder = new Data.Reports.OrdersReport.OrderDataTable())
            {
                using (var taOrder = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
                {
                    taOrder.FillByOrder(dtOrder, orderId);
                }

                var orderRow = dtOrder.FirstOrDefault();

                if (orderRow == null)
                {
                    return ResponseBase.Error("Order not found.");
                }

                int? currentOrderProcessId;
                using (var taOrderProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                {
                    currentOrderProcessId = taOrderProcesses.GetOrderProcessesID(orderId, orderRow.WorkStatus);
                }

                if (!currentOrderProcessId.HasValue)
                {
                    return ResponseBase.Error("Order Process not found.");
                }

                bool isInBatch;
                using (var taBatch = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
                {
                    isInBatch = Convert.ToBoolean(taBatch.GetIsOrderInActiveBatch(orderId) ?? 0);
                }

                var correctRole = GetRoleForOrder(orderRow.WorkStatus);

                var timerInfo = GetInfoForOrderProcess(orderId, userId, currentOrderProcessId.Value);

                if (isInBatch || !timerInfo.IsUserActiveOperator || !UserHasRole(userId, correctRole))
                {
                    return ResponseBase.Error("Unable to stop timer at this time.");
                }
                else
                {
                    TimeCollectionUtilities.StopOrderLaborTimer(orderId, userId);
                    return new ResponseBase
                    {
                        Success = true
                    };
                }
            }
        }

        private static ResponseBase DoPauseOrderLaborTimer(int orderId, int userId)
        {
            using (var dtOrder = new Data.Reports.OrdersReport.OrderDataTable())
            {
                using (var taOrder = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
                {
                    taOrder.FillByOrder(dtOrder, orderId);
                }

                var orderRow = dtOrder.FirstOrDefault();

                if (orderRow == null)
                {
                    return ResponseBase.Error("Order not found.");
                }

                int? currentOrderProcessId;
                using (var taOrderProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                {
                    currentOrderProcessId = taOrderProcesses.GetOrderProcessesID(orderId, orderRow.WorkStatus);
                }

                if (!currentOrderProcessId.HasValue)
                {
                    return ResponseBase.Error("Order Process not found.");
                }

                bool isInBatch;
                using (var taBatch = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
                {
                    isInBatch = Convert.ToBoolean(taBatch.GetIsOrderInActiveBatch(orderId) ?? 0);
                }

                var correctRole = GetRoleForOrder(orderRow.WorkStatus);

                var timerInfo = GetInfoForOrderProcess(orderId, userId, currentOrderProcessId.Value);

                if (isInBatch || !timerInfo.HasActiveLaborTimer || !UserHasRole(userId, correctRole))
                {
                    return ResponseBase.Error("Unable to pause timer at this time.");
                }
                else
                {
                    TimeCollectionUtilities.PauseOrderLaborTimer(orderId, userId);
                    return new ResponseBase
                    {
                        Success = true
                    };
                }
            }
        }

        private static ResponseBase DoStartBatchTimer(int batchId, int userId)
        {
            string workStatus;
            using (var taBatch = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
            {
                workStatus = taBatch.GetWorkStatus(batchId);
            }

            int batchProcessId;
            using (var taBatchProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter())
            {
                batchProcessId = taBatchProcesses.GetBatchProcessesID(batchId, workStatus);
            }

            if (batchProcessId <= 0)
            {
                return new TimerInfoResponse { Success = false, ErrorMessage = "Batch Process not found." };
            }

            var correctRole = GetRoleForBatch(workStatus);
            var timerInfo = GetInfoForBatchProcess(batchId, userId, batchProcessId);

            if (timerInfo.HasActiveProcessTimer || !UserHasRole(userId, correctRole))
            {
                return ResponseBase.Error("Unable to start timer at this time.");
            }
            else
            {
                TimeCollectionUtilities.StartBatchProcessTimer(batchId);
                return new ResponseBase
                {
                    Success = true
                };
            }
        }

        private static ResponseBase DoStopBatchTimer(int batchId, int userId)
        {
            string workStatus;
            using (var taBatch = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
            {
                workStatus = taBatch.GetWorkStatus(batchId);
            }

            int batchProcessId;
            using (var taBatchProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter())
            {
                batchProcessId = taBatchProcesses.GetBatchProcessesID(batchId, workStatus);
            }

            if (batchProcessId <= 0)
            {
                return new TimerInfoResponse { Success = false, ErrorMessage = "Batch Process not found." };
            }

            var correctRole = GetRoleForBatch(workStatus);
            var timerInfo = GetInfoForBatchProcess(batchId, userId, batchProcessId);

            if (!timerInfo.HasActiveProcessTimer || !UserHasRole(userId, correctRole))
            {
                return ResponseBase.Error("Unable to stop timer at this time.");
            }
            else
            {
                TimeCollectionUtilities.StopBatchProcessTimer(batchId);
                return new ResponseBase
                {
                    Success = true
                };
            }
        }

        private static ResponseBase DoStartBatchLaborTimer(int batchId, int userId)
        {
            string workStatus;
            using (var taBatch = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
            {
                workStatus = taBatch.GetWorkStatus(batchId);
            }

            int batchProcessId;
            using (var taBatchProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter())
            {
                batchProcessId = taBatchProcesses.GetBatchProcessesID(batchId, workStatus);
            }

            if (batchProcessId <= 0)
            {
                return new TimerInfoResponse { Success = false, ErrorMessage = "Batch Process not found." };
            }

            var correctRole = GetRoleForBatch(workStatus);
            var timerInfo = GetInfoForBatchProcess(batchId, userId, batchProcessId);

            if (timerInfo.HasActiveLaborTimer || !UserHasRole(userId, correctRole))
            {
                return ResponseBase.Error("Unable to start timer at this time.");
            }
            else
            {
                TimeCollectionUtilities.StartBatchLaborTimer(batchId, userId);
                TimeCollectionUtilities.StartBatchProcessTimer(batchId);
                return new ResponseBase
                {
                    Success = true
                };
            }
        }

        private static ResponseBase DoStopBatchLaborTimer(int batchId, int userId)
        {
            string workStatus;
            using (var taBatch = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
            {
                workStatus = taBatch.GetWorkStatus(batchId);
            }

            int batchProcessId;
            using (var taBatchProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter())
            {
                batchProcessId = taBatchProcesses.GetBatchProcessesID(batchId, workStatus);
            }

            if (batchProcessId <= 0)
            {
                return new TimerInfoResponse { Success = false, ErrorMessage = "Batch Process not found." };
            }

            var correctRole = GetRoleForBatch(workStatus);
            var timerInfo = GetInfoForBatchProcess(batchId, userId, batchProcessId);

            if (!timerInfo.IsUserActiveOperator || !UserHasRole(userId, correctRole))
            {
                return ResponseBase.Error("Unable to stop timer at this time.");
            }
            else
            {
                TimeCollectionUtilities.StopBatchLaborTimer(batchId, userId);
                return new ResponseBase
                {
                    Success = true
                };
            }
        }

        private static ResponseBase DoPauseBatchLaborTimer(int batchId, int userId)
        {
            string workStatus;
            using (var taBatch = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
            {
                workStatus = taBatch.GetWorkStatus(batchId);
            }

            int batchProcessId;
            using (var taBatchProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter())
            {
                batchProcessId = taBatchProcesses.GetBatchProcessesID(batchId, workStatus);
            }

            if (batchProcessId <= 0)
            {
                return new TimerInfoResponse { Success = false, ErrorMessage = "Batch Process not found." };
            }

            var correctRole = GetRoleForBatch(workStatus);
            var timerInfo = GetInfoForBatchProcess(batchId, userId, batchProcessId);

            if (!timerInfo.HasActiveLaborTimer || !UserHasRole(userId, correctRole))
            {
                return ResponseBase.Error("Unable to pause timer at this time.");
            }
            else
            {
                TimeCollectionUtilities.PauseBatchLaborTimer(batchId, userId);
                return new ResponseBase
                {
                    Success = true
                };
            }
        }


        /// <summary>
        /// Retrieves timer information.
        /// </summary>
        /// <remarks>
        /// Designed to only get timer information for order processes -
        /// the mobile app does not support out-of-process timers.
        /// </remarks>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <param name="orderProcessId"></param>
        /// <returns></returns>
        private static TimerInfoResponse GetInfoForOrderProcess(int orderId, int userId, int orderProcessId)
        {
            int activeProcessTimerCount;
            using (var taOrderProcessesTime = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTimeTableAdapter())
            {
                activeProcessTimerCount = taOrderProcessesTime.GetActiveTimerCount(orderProcessId) ?? 0;
            }

            int activeLaborTimerCount;
            using (var taLaborTime = new Data.Datasets.OrderProcessingDataSetTableAdapters.LaborTimeTableAdapter())
            {
                activeLaborTimerCount = taLaborTime.GetOrderUserActiveTimerCount(orderId, userId) ?? 0;
            }

            int activeOperatorCount;
            using (var taOperator = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesOperatorTableAdapter())
            {
                activeOperatorCount = taOperator.GetUserOperatorCount(OperatorStatus.Active.ToString(),
                            userId,
                            orderId) ?? 0;
            }

            return new TimerInfoResponse
            {
                Success = true,
                HasActiveProcessTimer = activeProcessTimerCount > 0,
                HasActiveLaborTimer = activeLaborTimerCount > 0,
                IsUserActiveOperator = activeOperatorCount > 0
            };
        }

        /// <summary>
        /// Retrieves timer information.
        /// </summary>
        /// <remarks>
        /// Designed to only get timer information for batch processes -
        /// the mobile app does not support out-of-process timers.
        /// </remarks>
        /// <param name="batchId"></param>
        /// <param name="userId"></param>
        /// <param name="batchProcessId"></param>
        /// <returns></returns>
        private static TimerInfoResponse GetInfoForBatchProcess(int batchId, int userId, int batchProcessId)
        {
            int activeProcessTimerCount;
            using (var taOrderProcessesTime = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTimeTableAdapter())
            {
                activeProcessTimerCount = taOrderProcessesTime.GetActiveTimerCount(batchProcessId) ?? 0;
            }

            int activeLaborTimerCount;
            using (var taLaborTime = new Data.Datasets.OrderProcessingDataSetTableAdapters.LaborTimeTableAdapter())
            {
                activeLaborTimerCount = taLaborTime.GetBatchUserActiveTimerCount(batchId, userId) ?? 0;
            }

            int activeOperatorCount;
            using (var taOperator = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesOperatorTableAdapter())
            {
                activeOperatorCount = taOperator.GetUserOperatorCount(OperatorStatus.Active.ToString(),
                            userId,
                            batchId) ?? 0;
            }

            return new TimerInfoResponse
            {
                Success = true,
                HasActiveProcessTimer = activeProcessTimerCount > 0,
                HasActiveLaborTimer = activeLaborTimerCount > 0,
                IsUserActiveOperator = activeOperatorCount > 0
            };
        }

        private static bool UserHasRole(int userId, string correctRole)
        {
            var hasCorrectRole = false;
            using (var taUserRoles = new Data.Datasets.SecurityDataSetTableAdapters.User_SecurityRolesTableAdapter())
            {
                var userSecurityRoles = new SecurityDataSet.User_SecurityRolesDataTable();
                taUserRoles.FillAllByUser(userSecurityRoles, userId);

                hasCorrectRole = userSecurityRoles
                    .OfType<SecurityDataSet.User_SecurityRolesRow>()
                    .Any(role => role.SecurityRoleID == correctRole);
            }

            return hasCorrectRole;
        }

        private static string GetRoleForOrder(string workStatus)
        {
            if (workStatus == ApplicationSettings.Current.WorkStatusInProcess)
            {
                return "OrderProcessing";
            }
            if (workStatus == ApplicationSettings.Current.WorkStatusPendingQI)
            {
                return "ControlInspection";
            }

            return string.Empty;
        }


        private static string GetRoleForBatch(string workStatus)
        {
            var correctRole = string.Empty;
            if (workStatus == ApplicationSettings.Current.WorkStatusInProcess)
            {
                correctRole = "BatchOrderProcessing";
            }
            else if (workStatus == ApplicationSettings.Current.WorkStatusPendingQI)
            {
                correctRole = "ControlInspection";
            }

            return correctRole;
        }

        #endregion

        #endregion
    }
}
