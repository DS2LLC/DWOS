using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using NLog;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace DWOS.Data.Order
{
    /// <summary>
    /// Starts, stops, and pauses time for orders.
    /// </summary>
    internal class OrderTimerHelper
    {
        #region Properties

        public SqlConnection Connection { get; }

        public SqlTransaction Transaction { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderTimerHelper"/>
        /// class.
        /// </summary>
        /// <param name="conn">The open database connection to use.</param>
        /// <param name="transaction">The database transaction to use.</param>
        public OrderTimerHelper(SqlConnection conn, SqlTransaction transaction)
        {
            Connection = conn ?? throw new ArgumentNullException(nameof(conn));
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        /// <summary>
        /// Stops timers after order processing, syncing time spent processing.
        /// </summary>
        /// <param name="orderId"></param>
        public void CompleteOrderProcessTimers(int orderId)
        {
            if (orderId <= 0)
            {
                return;
            }

            int orderProcessesId;

            using (var taOrderProcesses = new OrderProcessesTableAdapter())
            {
                taOrderProcesses.Connection = Connection;
                taOrderProcesses.Transaction = Transaction;

                // Process will be complete - get 'previous' one
                orderProcessesId = taOrderProcesses.GetPreviousOrderProcessesID(orderId) ?? 0;
            }

            StopAllOrderTimers(orderId);
            SyncOrderProcessesTime(orderProcessesId);
        }

        /// <summary>
        /// Stops all order timers without syncing them.
        /// </summary>
        /// <param name="orderId"></param>
        public void StopAllOrderTimers(int orderId)
        {
            if (orderId <= 0)
            {
                return;
            }

            DateTime endTime = DateTime.Now;

            // Stop process timers
            int processTimerCount;

            using (var taOrderProcessesTime = new OrderProcessesTimeTableAdapter())
            {
                taOrderProcessesTime.Connection = Connection;
                taOrderProcessesTime.Transaction = Transaction;

                var timers = taOrderProcessesTime.GetActiveTimersForOrder(orderId);
                processTimerCount = timers.Count;

                foreach (var timer in timers)
                {
                    timer.EndTime = endTime;
                }

                taOrderProcessesTime.Update(timers);
            }

            // Mark all operators as inactive
            var operatorCount = 0;

            using (var taOrderProcessesOperator = new OrderProcessesOperatorTableAdapter())
            {
                taOrderProcessesOperator.Connection = Connection;
                taOrderProcessesOperator.Transaction = Transaction;

                var operators = taOrderProcessesOperator.GetOperatorsForOrder(nameof(OperatorStatus.Active),
                    orderId);

                operatorCount += operators.Count;

                foreach (var processOperator in operators)
                {
                    processOperator.Status = nameof(OperatorStatus.Inactive);
                }

                taOrderProcessesOperator.Update(operators);
            }

            using (var taOrderOperator = new OrderOperatorTableAdapter())
            {
                taOrderOperator.Connection = Connection;
                taOrderOperator.Transaction = Transaction;

                var operators = taOrderOperator.GetOperatorsForOrder(nameof(OperatorStatus.Active),
                    orderId);

                operatorCount += operators.Count;

                foreach (var processOperator in operators)
                {
                    processOperator.Status = nameof(OperatorStatus.Inactive);
                }

                taOrderOperator.Update(operators);
            }

            // Stop labor timers
            var laborTimerCount = 0;

            using (var taLaborTime = new LaborTimeTableAdapter())
            {
                taLaborTime.Connection = Connection;
                taLaborTime.Transaction = Transaction;

                var timers = taLaborTime.GetActiveTimersForOrder(orderId);
                laborTimerCount += timers.Count;

                foreach (var timer in timers)
                {
                    timer.EndTime = endTime;
                }

                taLaborTime.Update(timers);
            }

            using (var taOrderOperatorTime = new OrderOperatorTimeTableAdapter())
            {
                taOrderOperatorTime.Connection = Connection;
                taOrderOperatorTime.Transaction = Transaction;

                var timers = taOrderOperatorTime.GetActiveTimersForOrder(orderId);
                laborTimerCount += timers.Count;

                foreach (var timer in timers)
                {
                    timer.EndTime = endTime;
                }

                taOrderOperatorTime.Update(timers);
            }

            LogManager.GetCurrentClassLogger().Debug("Stopped timers for OrderID={0}\n\tProcess: {1}\n\tOperators: {2}\n\tLabor: {3}",
                orderId,
                processTimerCount,
                operatorCount,
                laborTimerCount);
        }

        private void SyncOrderProcessesTime(int orderProcessesId)
        {
            const int minutesInHour = 60;

            if (orderProcessesId <= 0)
            {
                return;
            }

            int? durationInMinutes;
            using (var taOrderProcessesTime = new OrderProcessesTimeTableAdapter())
            {
                taOrderProcessesTime.Connection = Connection;
                taOrderProcessesTime.Transaction = Transaction;

                durationInMinutes = taOrderProcessesTime.GetTotalDuration(orderProcessesId, ApplicationSettings.Current.WorkStatusInProcess);
            }

            // Process duration
            if (durationInMinutes.HasValue)
            {
                using (var taOrderProcesses = new OrderProcessesTableAdapter())
                {
                    taOrderProcesses.Connection = Connection;
                    taOrderProcesses.Transaction = Transaction;

                    using (var dtOrderProcesses = new OrderProcessingDataSet.OrderProcessesDataTable())
                    {
                        taOrderProcesses.FillByID(dtOrderProcesses, orderProcessesId);

                        var orderProcess = dtOrderProcesses.FirstOrDefault();

                        if (orderProcess != null)
                        {
                            orderProcess.ProcessDurationMinutes = durationInMinutes.Value;

                            // Calculate burden cost from duration
                            var burdenRate = taOrderProcesses.GetBurdenRate(orderProcessesId);

                            if (burdenRate.HasValue)
                            {
                                var burdenPerMinute = burdenRate.Value / minutesInHour;
                                var roundedBurdenCost = Math.Round(burdenPerMinute * durationInMinutes.Value,
                                    ApplicationSettings.Current.PriceDecimalPlaces);

                                orderProcess.BurdenCost = roundedBurdenCost;
                            }
                        }


                        taOrderProcesses.Update(dtOrderProcesses);

                        LogManager.GetCurrentClassLogger().Debug("Updated time and burden cost for OrderProcessesID = {0}", orderProcessesId);
                    }
                }
            }
        }

        /// <summary>
        /// Starts a labor timer for a user and order.
        /// </summary>
        /// <param name="orderId">Order to start a timer for.</param>
        /// <param name="userId">User to start a timer for.</param>
        public void StartOrderLaborTimer(int orderId, int userId)
        {
            var workStatus = GetWorkStatus(orderId);

            if (ApplicationSettings.Current.ProcessingWorkStatuses.Contains(workStatus))
            {
                // Start order processing timer
                OrderProcessesTableAdapter taOrderProcesses = null;
                OrderProcessesOperatorTableAdapter taOperator = null;
                LaborTimeTableAdapter taLaborTime = null;

                try
                {
                    taOrderProcesses = new OrderProcessesTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taOperator = new OrderProcessesOperatorTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taLaborTime = new LaborTimeTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    int orderProcessesId = taOrderProcesses.GetOrderProcessesID(orderId, GetWorkStatus(orderId));

                    if (orderProcessesId <= 0)
                    {
                        return;
                    }

                    var processOperator = GetActiveProcessOperator(taOperator, orderProcessesId, orderId, userId);

                    var timerCount = taLaborTime.GetOrderActiveTimerCount(processOperator.OrderProcessesOperatorID) ?? 0;

                    if (timerCount == 0)
                    {
                        var dtLaborTime = new OrderProcessingDataSet.LaborTimeDataTable();
                        var newTimer = dtLaborTime.NewLaborTimeRow();
                        newTimer.OrderProcessesOperatorID = processOperator.OrderProcessesOperatorID;
                        newTimer.StartTime = DateTime.Now;
                        newTimer.WorkStatus = workStatus;

                        dtLaborTime.AddLaborTimeRow(newTimer);
                        taLaborTime.Update(dtLaborTime);

                        LogManager.GetCurrentClassLogger()
                            .Debug("Start labor timer for OrderID = {0} OrderProcessesID = {1}", orderId, orderProcessesId);
                    }
                }
                finally
                {
                    taOrderProcesses?.Dispose();
                    taOperator?.Dispose();
                    taLaborTime?.Dispose();
                }
            }
            else
            {
                // Start out-of-process timer
                OrderOperatorTableAdapter taOperator = null;
                OrderOperatorTimeTableAdapter taOperatorTime = null;

                try
                {
                    taOperator = new OrderOperatorTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taOperatorTime = new OrderOperatorTimeTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    var processOperator = GetActiveOperator(taOperator, orderId, userId);

                    var timerCount = taOperatorTime.GetActiveTimerCount(processOperator.OrderOperatorID) ?? 0;

                    if (timerCount == 0)
                    {
                        var dtOrderOperatorTime = new OrderProcessingDataSet.OrderOperatorTimeDataTable();
                        var newTimer = dtOrderOperatorTime.NewOrderOperatorTimeRow();
                        newTimer.OrderOperatorID = processOperator.OrderOperatorID;
                        newTimer.StartTime = DateTime.Now;
                        newTimer.WorkStatus = workStatus;

                        dtOrderOperatorTime.AddOrderOperatorTimeRow(newTimer);
                        taOperatorTime.Update(dtOrderOperatorTime);

                        LogManager.GetCurrentClassLogger()
                            .Debug("Start labor timer for OrderID = {0}", orderId);
                    }
                }
                finally
                {
                    taOperator?.Dispose();
                    taOperatorTime?.Dispose();
                }
            }
        }

        /// <summary>
        /// Pauses a user's labor timer for an order.
        /// </summary>
        /// <param name="orderId">Order to pause timer for.</param>
        /// <param name="userId">User to pause timer for.</param>
        public void PauseOrderLaborTimer(int orderId, int userId)
        {
            var workStatus = GetWorkStatus(orderId);

            if (ApplicationSettings.Current.ProcessingWorkStatuses.Contains(workStatus))
            {
                // Pause processing timer
                OrderProcessesTableAdapter taOrderProcesses = null;
                OrderProcessesOperatorTableAdapter taOperator = null;
                LaborTimeTableAdapter taLaborTime = null;

                try
                {
                    taOrderProcesses = new OrderProcessesTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taOperator = new OrderProcessesOperatorTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taLaborTime = new LaborTimeTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    int orderProcessesId = taOrderProcesses.GetOrderProcessesID(orderId, workStatus);

                    var processOperator = taOperator
                        .GetOperator(orderProcessesId, userId)
                        .FirstOrDefault();

                    if (orderProcessesId > 0 && processOperator != null)
                    {
                        var dtActiveTimer = taLaborTime.GetOrderActiveTimer(processOperator.OrderProcessesOperatorID);

                        if (dtActiveTimer != null && dtActiveTimer.Count == 1)
                        {
                            var activeTimer = dtActiveTimer.First();
                            activeTimer.EndTime = DateTime.Now;
                            taLaborTime.Update(dtActiveTimer);
                            LogManager.GetCurrentClassLogger()
                                .Debug("Pause labor timer for OrderID = {0} OrderProcessesID = {1}", orderId, orderProcessesId);
                        }
                    }
                }
                finally
                {
                    taOrderProcesses?.Dispose();
                    taOperator?.Dispose();
                    taLaborTime?.Dispose();
                }
            }
            else
            {
                // Pause out-of-processing timer
                OrderOperatorTableAdapter taOrderOperator = null;
                OrderOperatorTimeTableAdapter taOrderOperatorTime = null;

                try
                {
                    taOrderOperator = new OrderOperatorTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taOrderOperatorTime = new OrderOperatorTimeTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    var processOperator = taOrderOperator
                        .GetOperator(orderId, userId)
                        .FirstOrDefault();

                    if (processOperator != null)
                    {
                        var dtActiveTimer = taOrderOperatorTime.GetActiveTimers(processOperator.OrderOperatorID);
                        foreach (var activeTimer in dtActiveTimer)
                        {
                            activeTimer.EndTime = DateTime.Now;
                        }

                        taOrderOperatorTime.Update(dtActiveTimer);

                        LogManager.GetCurrentClassLogger()
                            .Debug("Pause labor timer for OrderID = {0}", orderId);
                    }
                }
                finally
                {
                    taOrderOperator?.Dispose();
                    taOrderOperatorTime?.Dispose();
                }
            }
        }

        /// <summary>
        /// Stops a user's labor timer for an order.
        /// </summary>
        /// <param name="orderId">Order to stop timer for.</param>
        /// <param name="userId">User to stop timer for.</param>
        public void StopOrderLaborTimer(int orderId, int userId)
        {
            var workStatus = GetWorkStatus(orderId);

            if (ApplicationSettings.Current.ProcessingWorkStatuses.Contains(workStatus))
            {
                // Stop processing timer
                OrderProcessesTableAdapter taOrderProcesses = null;
                OrderProcessesOperatorTableAdapter taOperator = null;
                LaborTimeTableAdapter taLaborTime = null;

                try
                {
                    taOrderProcesses = new OrderProcessesTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taOperator = new OrderProcessesOperatorTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taLaborTime = new LaborTimeTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    int orderProcessesId = taOrderProcesses.GetOrderProcessesID(orderId, workStatus);
                    var dtOperatorTable = taOperator.GetOperator(orderProcessesId, userId);
                    var processOperator = dtOperatorTable.FirstOrDefault();

                    if (orderProcessesId > 0 && processOperator != null)
                    {
                        // Mark operator as inactive
                        processOperator.Status = nameof(OperatorStatus.Inactive);
                        taOperator.Update(dtOperatorTable);
                        LogManager.GetCurrentClassLogger()
                            .Debug("Marked operator inactive for OrderID = {0} OrderProcessesID = {1}", orderId, orderProcessesId);

                        // Stop Timer
                        var dtActiveTimer = taLaborTime.GetOrderActiveTimer(processOperator.OrderProcessesOperatorID);

                        if (dtActiveTimer != null && dtActiveTimer.Count == 1)
                        {
                            var activeTimer = dtActiveTimer.First();
                            activeTimer.EndTime = DateTime.Now;
                            taLaborTime.Update(dtActiveTimer);
                            LogManager.GetCurrentClassLogger()
                                .Debug("Stop labor timer for OrderID = {0} OrderProcessesID = {1}", orderId, orderProcessesId);
                        }
                    }
                }
                finally
                {
                    taOrderProcesses?.Dispose();
                    taOperator?.Dispose();
                    taLaborTime?.Dispose();
                }
            }
            else
            {
                // Stop out-of-process timer
                OrderOperatorTableAdapter taOrderOperator = null;
                OrderOperatorTimeTableAdapter taOrderOperatorTime = null;

                try
                {
                    taOrderOperator = new OrderOperatorTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taOrderOperatorTime = new OrderOperatorTimeTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    var dtOperator = taOrderOperator.GetOperator(orderId, userId);
                    var processOperator = dtOperator.FirstOrDefault();

                    if (processOperator != null)
                    {
                        // Mark operator as inactive
                        processOperator.Status = nameof(OperatorStatus.Inactive);
                        taOrderOperator.Update(dtOperator);
                        LogManager.GetCurrentClassLogger()
                            .Debug("Marked operator inactive for OrderID = {0}", orderId);

                        // Stop Timer
                        var dtActiveTimer = taOrderOperatorTime.GetActiveTimers(processOperator.OrderOperatorID);
                        foreach (var activeTimer in dtActiveTimer)
                        {
                            activeTimer.EndTime = DateTime.Now;
                        }

                        taOrderOperatorTime.Update(dtActiveTimer);

                        LogManager.GetCurrentClassLogger()
                            .Debug("Stop labor timer for OrderID = {0}", orderId);
                    }
                }
                finally
                {
                    taOrderOperator?.Dispose();
                    taOrderOperatorTime?.Dispose();
                }
            }
        }

        /// <summary>
        /// Starts the order process timer for an order.
        /// </summary>
        /// <param name="orderId"></param>
        public void StartOrderProcessTimer(int orderId)
        {
            OrderProcessesTimeTableAdapter taOrderProcessesTime = null;
            OrderProcessesTableAdapter taOrderProcesses = null;

            try
            {
                taOrderProcessesTime = new OrderProcessesTimeTableAdapter
                {
                    Connection = Connection,
                    Transaction = Transaction
                };

                taOrderProcesses = new OrderProcessesTableAdapter
                {
                    Connection = Connection,
                    Transaction = Transaction
                };

                string workStatus;
                using (var taOrderSummary = new OrderSummaryTableAdapter())
                {
                    taOrderSummary.Connection = Connection;
                    taOrderSummary.Transaction = Transaction;

                    workStatus = taOrderSummary.GetWorkStatus(orderId);
                }

                int orderProcessesId = taOrderProcesses.GetOrderProcessesID(orderId, workStatus);

                int timerCount = taOrderProcessesTime.GetActiveTimerCount(orderProcessesId) ?? 0;

                if (orderProcessesId > 0 && timerCount == 0)
                {
                    var dtOrderProcessesTime = new OrderProcessingDataSet.OrderProcessesTimeDataTable();
                    var newTimer = dtOrderProcessesTime.NewOrderProcessesTimeRow();
                    newTimer.OrderProcessesID = orderProcessesId;
                    newTimer.StartTime = DateTime.Now;
                    newTimer.WorkStatus = workStatus;
                    dtOrderProcessesTime.AddOrderProcessesTimeRow(newTimer);
                    taOrderProcessesTime.Update(dtOrderProcessesTime);

                    LogManager.GetCurrentClassLogger()
                        .Debug("Start process timer for OrderID = {0} OrderProcessesID = {1}", orderId, orderProcessesId);
                }
            }
            finally
            {
                taOrderProcessesTime?.Dispose();
                taOrderProcesses?.Dispose();
            }
        }

        /// <summary>
        /// Stops an order process timer for an order.
        /// </summary>
        /// <param name="orderId"></param>
        public void StopOrderProcessTimer(int orderId)
        {
            OrderProcessesTimeTableAdapter taOrderProcessesTime = null;
            OrderProcessesTableAdapter taOrderProcesses = null;

            try
            {
                taOrderProcessesTime = new OrderProcessesTimeTableAdapter
                {
                    Connection = Connection,
                    Transaction = Transaction
                };

                taOrderProcesses = new OrderProcessesTableAdapter
                {
                    Connection = Connection,
                    Transaction = Transaction
                };

                string workStatus;
                using (var taOrderSummary = new OrderSummaryTableAdapter())
                {
                    taOrderSummary.Connection = Connection;
                    taOrderSummary.Transaction = Transaction;

                    workStatus = taOrderSummary.GetWorkStatus(orderId);
                }

                int orderProcessesId = taOrderProcesses.GetOrderProcessesID(orderId, workStatus);
                var dtActiveTimer = taOrderProcessesTime.GetActiveTimer(orderProcessesId);

                if (dtActiveTimer != null && dtActiveTimer.Count == 1)
                {
                    var activeTimer = dtActiveTimer.First();
                    activeTimer.EndTime = DateTime.Now;
                    taOrderProcessesTime.Update(dtActiveTimer);

                    LogManager.GetCurrentClassLogger()
                        .Debug("Stop process timer for OrderID = {0} OrderProcessesID = {1}", orderId, orderProcessesId);
                }
            }
            finally
            {
                taOrderProcessesTime?.Dispose();
                taOrderProcesses?.Dispose();
            }
        }

        private OrderProcessingDataSet.OrderProcessesOperatorRow GetActiveProcessOperator(OrderProcessesOperatorTableAdapter taOperator,
            int orderProcessesId, int orderId, int userId)
        {
            OrderProcessingDataSet.OrderProcessesOperatorRow processOperator;
            var dtOperator = taOperator.GetOperator(orderProcessesId, userId);

            if (dtOperator.Count == 0)
            {
                processOperator = dtOperator.NewOrderProcessesOperatorRow();
                processOperator.OrderProcessesID = orderProcessesId;
                processOperator.UserID = userId;
                processOperator.Status = nameof(OperatorStatus.Active);
                dtOperator.AddOrderProcessesOperatorRow(processOperator);

                taOperator.Update(dtOperator);

                LogManager.GetCurrentClassLogger()
                    .Debug("Created new operator for OrderID = {0} OrderProcessID = {1}", orderId, orderProcessesId);
            }
            else
            {
                processOperator = dtOperator[0];
            }

            if (processOperator.Status != nameof(OperatorStatus.Active))
            {
                processOperator.Status = nameof(OperatorStatus.Active);
                taOperator.Update(dtOperator);

                LogManager.GetCurrentClassLogger()
                    .Debug("Marked existing operator as active for OrderID = {0} OrderProcessID = {1}", orderId, orderProcessesId);
            }

            return processOperator;
        }

        private OrderProcessingDataSet.OrderOperatorRow GetActiveOperator(OrderOperatorTableAdapter taOperator,
            int orderId, int userId)
        {
            OrderProcessingDataSet.OrderOperatorRow orderOperator;
            var dtOperator = taOperator.GetOperator(orderId, userId);

            if (dtOperator.Count == 0)
            {
                orderOperator = dtOperator.NewOrderOperatorRow();
                orderOperator.OrderID = orderId;
                orderOperator.UserID = userId;
                orderOperator.Status = nameof(OperatorStatus.Active);
                dtOperator.AddOrderOperatorRow(orderOperator);

                taOperator.Update(dtOperator);

                LogManager.GetCurrentClassLogger()
                    .Debug("Created new operator for OrderID = {0}", orderId);
            }
            else
            {
                orderOperator = dtOperator[0];
            }

            if (orderOperator.Status != nameof(OperatorStatus.Active))
            {
                orderOperator.Status = nameof(OperatorStatus.Active);
                taOperator.Update(dtOperator);

                LogManager.GetCurrentClassLogger()
                    .Debug("Marked existing operator as active for OrderID = {0}", orderId);
            }

            return orderOperator;
        }

        private string GetWorkStatus(int orderId)
        {
            string workStatus;
            using (var taOrderSummary = new OrderSummaryTableAdapter())
            {
                taOrderSummary.Connection = Connection;
                taOrderSummary.Transaction = Transaction;

                workStatus = taOrderSummary.GetWorkStatus(orderId);
            }

            return workStatus;
        }

        #endregion
    }
}
