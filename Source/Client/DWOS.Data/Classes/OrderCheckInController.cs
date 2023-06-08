using DWOS.Data.Datasets;
using System;
using System.Linq;
using DWOS.Data.Datasets.OrderHistoryDataSetTableAdapters;
using DWOS.Data.Order;

namespace DWOS.Data
{
    public class OrderCheckInController
    {
        public int OrderId { get; set; }

        public OrderCheckInController(int orderId) { this.OrderId = orderId; }

        /// <summary>
        /// Determines if the next department is a valid destination for the order.
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
                //Is Correct Work Status
                using (var taOrderSummary = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
                {
                    var workStatus = taOrderSummary.GetWorkStatus(OrderId);

                    if (String.IsNullOrWhiteSpace(workStatus) || workStatus != ApplicationSettings.Current.WorkStatusChangingDepartment)
                        return new ResponseInfo<bool>(false, "The order {0} work status is not set to '{1}'.".FormatWith(OrderId, ApplicationSettings.Current.WorkStatusChangingDepartment));
                }

                //Is Correct Department
                using (var taOrderProcesses = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                {
                    var orderProcesses = new DWOS.Data.Datasets.OrderProcessingDataSet.OrderProcessesDataTable();
                    taOrderProcesses.FillCurrentProcess(orderProcesses, this.OrderId);
                    OrderProcessingDataSet.OrderProcessesRow currentOrderProcess = orderProcesses.FirstOrDefault();

                    if (currentOrderProcess == null)
                        return new ResponseInfo<bool>(false, "The order {0} currently has no further processing.".FormatWith(OrderId));
                    if (currentOrderProcess.Department != nextDepartment)
                        return new ResponseInfo<bool>(false, "The order {0} is required to be checked into the '{1}' department.".FormatWith(OrderId, currentOrderProcess.Department));
                }

                // Is Correct Line
                if (ApplicationSettings.Current.MultipleLinesEnabled)
                {
                    using (var taOrderSummary = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
                    {
                        var currentLine = taOrderSummary.GetCurrentLine(OrderId);

                        if (currentLine.HasValue && processingLineId != currentLine)
                        {
                            return new ResponseInfo<bool>(false, $"The order {OrderId} must be checked into the correct line.");
                        }
                    }
                }

                //Is In an Active Batch
                using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
                {
                    var isInBatch = Convert.ToBoolean(ta.GetIsOrderInActiveBatch(OrderId).GetValueOrDefault());

                    if (isInBatch)
                        return new ResponseInfo<bool>(false, "This order {0} is currently in an active batch.".FormatWith(OrderId));
                }


                return new ResponseInfo<bool>(true);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error validating order check in for order {0}.".FormatWith(this.OrderId));
                return new ResponseInfo<bool>(false, exc.ToString());
            }
        }

        /// <summary>
        /// Checks the order into the given department with the given user.
        /// </summary>
        /// <param name="nextDepartment"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResponseInfo <bool> CheckIn(string nextDepartment, int userId)
        {
            try
            {
                using (var taOrderSummary = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
                {
                    var isValid = IsValid(nextDepartment, taOrderSummary.GetCurrentLine(OrderId));

                    if(!isValid.Response)
                        return isValid;
                }

                return DoCheckIn(nextDepartment, userId);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error checking in order {0}.".FormatWith(this.OrderId));
                return new ResponseInfo<bool>(false, exc.ToString());
            }
        }

        public ResponseInfo<bool> AutoCheckIn(int userId)
        {
            try
            {
                using (
                    var taOrderProcesses =
                        new Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                {
                    var orderProcesses = new OrderProcessingDataSet.OrderProcessesDataTable();
                    taOrderProcesses.FillCurrentProcess(orderProcesses, OrderId);
                    var currentDepartment = orderProcesses.OrderBy(op => op.StepOrder).FirstOrDefault()?.Department;

                    if (string.IsNullOrEmpty(currentDepartment))
                    {
                        return new ResponseInfo<bool>(false, $"The order {OrderId} currently has no further processing.");
                    }

                    return DoCheckIn(currentDepartment, userId);
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error automatically checking in order {0}.".FormatWith(OrderId));
                return new ResponseInfo<bool>(false, exc.ToString());
            }
        }

        private ResponseInfo<bool> DoCheckIn(string nextDepartment, int userId)
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    OrderProcessingDataSet.OrderProcessesRow currentOrderProcess;

                    using (
                        var taOrderProcesses =
                            new Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                    {
                        taOrderProcesses.Connection = conn;
                        taOrderProcesses.Transaction = transaction;

                        var orderProcesses = new OrderProcessingDataSet.OrderProcessesDataTable();
                        taOrderProcesses.FillCurrentProcess(orderProcesses, this.OrderId);
                        currentOrderProcess = orderProcesses.FirstOrDefault();

                        if (currentOrderProcess == null)
                            return new ResponseInfo<bool>(false, "The order currently has no further processing.");
                    }

                    //Check the order in
                    using (var taOrderSummary = new Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
                    {
                        taOrderSummary.Connection = conn;
                        taOrderSummary.Transaction = transaction;

                        //update work order has been checked in
                        taOrderSummary.UpdateWorkStatus(ApplicationSettings.Current.WorkStatusInProcess, OrderId);

                        //update work orders current location
                        taOrderSummary.UpdateOrderLocation(nextDepartment, OrderId);
                    }

                    //set start date of the current process
                    using (var taOrderProcesses = new Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                    {
                        taOrderProcesses.Connection = conn;
                        taOrderProcesses.Transaction = transaction;
                        taOrderProcesses.UpdateStartDate(DateTime.Now, currentOrderProcess.OrderProcessesID);
                    }

                    //update order history
                    var successMessage = "Order " + this.OrderId + " checked in to " + nextDepartment + ".";
                    var userName = "Unknown";

                    if (userId > 0)
                    {
                        using (var taUser = new Datasets.SecurityDataSetTableAdapters.UsersTableAdapter())
                        {
                            taUser.Connection = conn;
                            taUser.Transaction = transaction;
                            userName = taUser.GetUserName(userId);
                        }
                    }

                    using (var taHistory = new OrderHistoryTableAdapter())
                    {
                        taHistory.Connection = conn;
                        taHistory.Transaction = transaction;
                        taHistory.UpdateOrderHistory(OrderId, "Order CheckIn", successMessage, userName);
                    }

                    new OrderTimerHelper(conn, transaction).StopAllOrderTimers(OrderId);

                    transaction.Commit();
                    return new ResponseInfo<bool>(true, successMessage);
                }
            }
        }
    }
}
