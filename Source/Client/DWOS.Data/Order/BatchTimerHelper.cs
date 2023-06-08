using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using NLog;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace DWOS.Data.Order
{
    /// <summary>
    /// Starts, stops, and pauses timers for batches.
    /// </summary>
    internal class BatchTimerHelper
    {
        #region Properties

        public SqlConnection Connection { get; }

        public SqlTransaction Transaction { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchTimerHelper"/> class.
        /// </summary>
        /// <param name="conn">The open database connection to use.</param>
        /// <param name="transaction">The database transaction to use.</param>
        public BatchTimerHelper(SqlConnection conn, SqlTransaction transaction)
        {
            Connection = conn ?? throw new ArgumentNullException(nameof(conn));
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        /// <summary>
        /// Stops timers after batch processing, syncing time spent processing.
        /// </summary>
        /// <param name="batchId"></param>
        public void CompleteBatchProcessesTimers(int batchId)
        {
            if (batchId <= 0)
            {
                return;
            }

            int batchProcessesId;

            using (var taBatchProcesses = new BatchProcessesTableAdapter())
            {
                taBatchProcesses.Connection = Connection;
                taBatchProcesses.Transaction = Transaction;

                // Process will be complete - get 'previous' one
                batchProcessesId = taBatchProcesses.GetPreviousBatchProcessID(batchId) ?? 0;
            }

            StopAllBatchTimers(batchId);
            SyncBatchProcessesTime(batchProcessesId);
        }

        /// <summary>
        /// Stops all batch timers without syncing them.
        /// </summary>
        /// <param name="batchId"></param>
        public void StopAllBatchTimers(int batchId)
        {
            if (batchId <= 0)
            {
                return;
            }

            DateTime endTime = DateTime.Now;

            // Stop process timers
            int processTimerCount;

            using (var taBatchProcessesTime = new BatchProcessesTimeTableAdapter())
            {
                taBatchProcessesTime.Connection = Connection;
                taBatchProcessesTime.Transaction = Transaction;

                var timers = taBatchProcessesTime.GetActiveTimersForBatch(batchId);
                processTimerCount = timers.Count;

                foreach (var timer in timers)
                {
                    timer.EndTime = endTime;
                }

                taBatchProcessesTime.Update(timers);
            }

            // Mark all operators as inactive
            var operatorCount = 0;

            using (var taBatchProcessesOperator = new BatchProcessesOperatorTableAdapter())
            {
                taBatchProcessesOperator.Connection = Connection;
                taBatchProcessesOperator.Transaction = Transaction;

                var operators = taBatchProcessesOperator.GetOperatorsForBatch(nameof(OperatorStatus.Active),
                    batchId);

                operatorCount += operators.Count;

                foreach (var processOperator in operators)
                {
                    processOperator.Status = nameof(OperatorStatus.Inactive);
                }

                taBatchProcessesOperator.Update(operators);
            }

            using (var taBatchOperator = new BatchOperatorTableAdapter())
            {
                taBatchOperator.Connection = Connection;
                taBatchOperator.Transaction = Transaction;

                var operators = taBatchOperator.GetOperatorsForBatch(nameof(OperatorStatus.Active),
                    batchId);

                operatorCount += operators.Count;

                foreach (var processOperator in operators)
                {
                    processOperator.Status = nameof(OperatorStatus.Inactive);
                }

                taBatchOperator.Update(operators);
            }

            // Stop labor timers
            var laborTimerCount = 0;

            using (var taLaborTime = new LaborTimeTableAdapter())
            {
                taLaborTime.Connection = Connection;
                taLaborTime.Transaction = Transaction;

                var timers = taLaborTime.GetActiveTimersForBatch(batchId);
                laborTimerCount += timers.Count;

                foreach (var timer in timers)
                {
                    timer.EndTime = endTime;
                }

                taLaborTime.Update(timers);
            }

            using (var taBatchOperatorTimer = new BatchOperatorTimeTableAdapter())
            {
                taBatchOperatorTimer.Connection = Connection;
                taBatchOperatorTimer.Transaction = Transaction;

                var timers = taBatchOperatorTimer.GetActiveTimersForBatch(batchId);
                laborTimerCount += timers.Count;

                foreach (var timer in timers)
                {
                    timer.EndTime = endTime;
                }

                taBatchOperatorTimer.Update(timers);
            }

            LogManager.GetCurrentClassLogger().Debug("Stopped timers for BatchID={0}\n\tProcess: {1}\n\tOperators: {2}\n\tLabor: {3}",
                batchId,
                processTimerCount,
                operatorCount,
                laborTimerCount);
        }

        private void SyncBatchProcessesTime(int batchProcessId)
        {
            const int minutesInHour = 60;

            BatchProcessesTableAdapter taBatchProcesses = null;
            OrderProcessesTableAdapter taOrderProcesses = null;

            OrderProcessingDataSet.BatchProcessesDataTable dtBatchProcesses = null;
            OrderProcessingDataSet.OrderProcessesDataTable dtOrderProcesses = null;

            try
            {
                if (batchProcessId <= 0)
                {
                    return;
                }

                int? durationInMinutes;
                using (var taBatchProcessesTime = new BatchProcessesTimeTableAdapter())
                {
                    taBatchProcessesTime.Connection = Connection;
                    taBatchProcessesTime.Transaction = Transaction;

                    durationInMinutes = taBatchProcessesTime.GetTotalDuration(batchProcessId, ApplicationSettings.Current.WorkStatusInProcess);
                }

                if (durationInMinutes.HasValue)
                {
                    taBatchProcesses = new BatchProcessesTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taOrderProcesses = new OrderProcessesTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    dtBatchProcesses = new OrderProcessingDataSet.BatchProcessesDataTable();
                    dtOrderProcesses = new OrderProcessingDataSet.OrderProcessesDataTable();

                    taBatchProcesses.FillByBatchProcess(dtBatchProcesses, batchProcessId);
                    taOrderProcesses.FillByBatchProcess(dtOrderProcesses, batchProcessId);

                    var batchProcess = dtBatchProcesses.FirstOrDefault();

                    if (batchProcess != null)
                    {
                        batchProcess.ProcessDurationMinutes = durationInMinutes.Value;
                    }

                    taBatchProcesses.Update(dtBatchProcesses);

                    LogManager.GetCurrentClassLogger().Debug("Updated time for BatchProcessID = {0}", batchProcessId);

                    var burdenRate = taOrderProcesses.GetBurdenRate(dtOrderProcesses.FirstOrDefault()?.OrderProcessesID ?? -1);
                    decimal? burdenPerOrder = null;

                    if (burdenRate.HasValue && dtOrderProcesses.Count > 0)
                    {
                        var burdenPerMinute = burdenRate.Value / minutesInHour;
                        var totalBurden = burdenPerMinute * durationInMinutes.Value;
                        burdenPerOrder = Math.Round(totalBurden / dtOrderProcesses.Count,
                            ApplicationSettings.Current.PriceDecimalPlaces);
                    }

                    foreach (var row in dtOrderProcesses)
                    {
                        row.ProcessDurationMinutes = durationInMinutes.Value;

                        if (burdenPerOrder.HasValue)
                        {
                            row.BurdenCost = burdenPerOrder.Value;
                        }

                        LogManager.GetCurrentClassLogger().Debug("Updated time & burden cost for OrderProcessesID = {0}", row.OrderProcessesID);
                    }

                    taOrderProcesses.Update(dtOrderProcesses);
                }
            }
            finally
            {
                taBatchProcesses?.Dispose();
                taOrderProcesses?.Dispose();

                dtBatchProcesses?.Dispose();
                dtOrderProcesses?.Dispose();
            }
        }

        /// <summary>
        /// Starts a user's labor timer for a batch.
        /// </summary>
        /// <param name="batchId">Batch to start timer for.</param>
        /// <param name="userId">User to start timer for.</param>
        public void StartBatchLaborTimer(int batchId, int userId)
        {
            var workStatus = GetWorkStatus(batchId);

            if (ApplicationSettings.Current.ProcessingWorkStatuses.Contains(workStatus))
            {
                // Start processing timer
                BatchProcessesTableAdapter taBatchProcesses = null;
                BatchProcessesOperatorTableAdapter taOperator = null;
                LaborTimeTableAdapter taLaborTime = null;

                try
                {
                    taBatchProcesses = new BatchProcessesTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taOperator = new BatchProcessesOperatorTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taLaborTime = new LaborTimeTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    int batchProcessId = taBatchProcesses.GetBatchProcessesID(batchId, workStatus);

                    if (batchProcessId <= 0)
                    {
                        return;
                    }

                    var processOperator = GetActiveProcessOperator(taOperator, batchProcessId, batchId, userId);

                    var timerCount = taLaborTime.GetBatchActiveTimerCount(processOperator.BatchProcessesOperatorID) ?? 0;

                    if (timerCount == 0)
                    {
                        var dtLaborTime = new OrderProcessingDataSet.LaborTimeDataTable();
                        var newTimer = dtLaborTime.NewLaborTimeRow();
                        newTimer.BatchProcessesOperatorID = processOperator.BatchProcessesOperatorID;
                        newTimer.StartTime = DateTime.Now;
                        newTimer.WorkStatus = workStatus;

                        dtLaborTime.AddLaborTimeRow(newTimer);
                        taLaborTime.Update(dtLaborTime);

                        LogManager.GetCurrentClassLogger()
                            .Debug("Start labor timer for BatchID = {0} BatchProcessID = {1}", batchId, batchProcessId);
                    }
                }
                finally
                {
                    taBatchProcesses?.Dispose();
                    taOperator?.Dispose();
                    taLaborTime?.Dispose();
                }
            }
            else
            {
                // Start out-of-process timer
                BatchOperatorTableAdapter taOperator = null;
                BatchOperatorTimeTableAdapter taOperatorTime = null;

                try
                {
                    taOperator = new BatchOperatorTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taOperatorTime = new BatchOperatorTimeTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    var processOperator = GetActiveOperator(taOperator, batchId, userId);
                    var timerCount = taOperatorTime.GetActiveTimerCount(processOperator.BatchOperatorID) ?? 0;

                    if (timerCount == 0)
                    {
                        var dtOrderOperatorTime = new OrderProcessingDataSet.BatchOperatorTimeDataTable();
                        var newTimer = dtOrderOperatorTime.NewBatchOperatorTimeRow();
                        newTimer.BatchOperatorID = processOperator.BatchOperatorID;
                        newTimer.StartTime = DateTime.Now;
                        newTimer.WorkStatus = workStatus;

                        dtOrderOperatorTime.AddBatchOperatorTimeRow(newTimer);
                        taOperatorTime.Update(dtOrderOperatorTime);

                        LogManager.GetCurrentClassLogger()
                            .Debug("Start labor timer for BatchID = {0}", batchId);
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
        /// Pauses a user's labor timer for a batch.
        /// </summary>
        /// <param name="batchId">Batch to pause timer for.</param>
        /// <param name="userId">User to pause timer for.</param>
        public void PauseBatchLaborTimer(int batchId, int userId)
        {
            var workStatus = GetWorkStatus(batchId);

            if (ApplicationSettings.Current.ProcessingWorkStatuses.Contains(workStatus))
            {
                BatchProcessesTableAdapter taBatchProcesses = null;
                BatchProcessesOperatorTableAdapter taOperator = null;
                LaborTimeTableAdapter taLaborTime = null;

                try
                {
                    // Pause processing timer
                    taBatchProcesses = new BatchProcessesTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taOperator = new BatchProcessesOperatorTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taLaborTime = new LaborTimeTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    int batchProcessesId = taBatchProcesses.GetBatchProcessesID(batchId, workStatus);
                    var currentUserId = userId;
                    var processOperator = taOperator
                        .GetOperator(batchProcessesId, currentUserId)
                        .FirstOrDefault();

                    if (batchProcessesId > 0 && processOperator != null)
                    {
                        var dtActiveTimer = taLaborTime.GetBatchActiveTimer(processOperator.BatchProcessesOperatorID);

                        if (dtActiveTimer != null && dtActiveTimer.Count == 1)
                        {
                            var activeTimer = dtActiveTimer.First();
                            activeTimer.EndTime = DateTime.Now;
                            taLaborTime.Update(dtActiveTimer);
                            LogManager.GetCurrentClassLogger()
                                .Debug("Pause labor timer for BatchID = {0} BatchProcessID = {1}", batchId, batchProcessesId);
                        }
                    }
                }
                finally
                {
                    taBatchProcesses?.Dispose();
                    taOperator?.Dispose();
                    taLaborTime?.Dispose();
                }
            }
            else
            {
                // Pause out-of-process timer
                BatchOperatorTableAdapter taOrderOperator = null;
                BatchOperatorTimeTableAdapter taOrderOperatorTime = null;

                try
                {
                    taOrderOperator = new BatchOperatorTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taOrderOperatorTime = new BatchOperatorTimeTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    var processOperator = taOrderOperator
                        .GetOperator(batchId, userId)
                        .FirstOrDefault();

                    if (processOperator != null)
                    {
                        var dtActiveTimer = taOrderOperatorTime.GetActiveTimers(processOperator.BatchOperatorID);
                        foreach (var activeTimer in dtActiveTimer)
                        {
                            activeTimer.EndTime = DateTime.Now;
                        }

                        taOrderOperatorTime.Update(dtActiveTimer);

                        LogManager.GetCurrentClassLogger()
                            .Debug("Pause labor timer for BatchID = {0}", batchId);
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
        /// Stops a user's labor timer for a batch.
        /// </summary>
        /// <param name="batchId">Batch to stop timer for.</param>
        /// <param name="userId">User to stop timer for.</param>
        public void StopBatchLaborTimer(int batchId, int userId)
        {
            var workStatus = GetWorkStatus(batchId);

            if (ApplicationSettings.Current.ProcessingWorkStatuses.Contains(workStatus))
            {
                // Stop process timer
                BatchProcessesTableAdapter taBatchProcesses = null;
                BatchProcessesOperatorTableAdapter taOperator = null;
                LaborTimeTableAdapter taLaborTime = null;

                try
                {
                    taBatchProcesses = new BatchProcessesTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taOperator = new BatchProcessesOperatorTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taLaborTime = new LaborTimeTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    int batchProcessId = taBatchProcesses.GetBatchProcessesID(batchId, workStatus);
                    var currentUserId = userId;
                    var dtOperatorTable = taOperator.GetOperator(batchProcessId, currentUserId);
                    var processOperator = dtOperatorTable.FirstOrDefault();

                    if (batchProcessId > 0 && processOperator != null)
                    {
                        // Mark operator as inactive
                        processOperator.Status = nameof(OperatorStatus.Inactive);
                        taOperator.Update(dtOperatorTable);
                        LogManager.GetCurrentClassLogger()
                            .Debug("Marked operator inactive for BatchID = {0} BatchProcessID = {1}", batchId, batchProcessId);

                        // Stop Timer
                        var dtActiveTimer = taLaborTime.GetBatchActiveTimer(processOperator.BatchProcessesOperatorID);

                        if (dtActiveTimer != null && dtActiveTimer.Count == 1)
                        {
                            var activeTimer = dtActiveTimer.First();
                            activeTimer.EndTime = DateTime.Now;
                            taLaborTime.Update(dtActiveTimer);
                            LogManager.GetCurrentClassLogger()
                                .Debug("Stop labor timer for BatchID = {0} BatchProcessID = {1}", batchId, batchProcessId);
                        }
                    }
                }
                finally
                {
                    taBatchProcesses?.Dispose();
                    taOperator?.Dispose();
                    taLaborTime?.Dispose();
                }
            }
            else
            {
                // Stop out-of-process timer
                BatchOperatorTableAdapter taOrderOperator = null;
                BatchOperatorTimeTableAdapter taOrderOperatorTime = null;

                try
                {
                    taOrderOperator = new BatchOperatorTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    taOrderOperatorTime = new BatchOperatorTimeTableAdapter
                    {
                        Connection = Connection,
                        Transaction = Transaction
                    };

                    var dtOperator = taOrderOperator.GetOperator(batchId, userId);
                    var processOperator = dtOperator.FirstOrDefault();

                    if (processOperator != null)
                    {
                        // Mark operator as inactive
                        processOperator.Status = nameof(OperatorStatus.Inactive);
                        taOrderOperator.Update(dtOperator);
                        LogManager.GetCurrentClassLogger()
                            .Debug("Marked operator inactive for BatchID = {0}", batchId);

                        // Stop Timer
                        var dtActiveTimer = taOrderOperatorTime.GetActiveTimers(processOperator.BatchOperatorID);
                        foreach (var activeTimer in dtActiveTimer)
                        {
                            activeTimer.EndTime = DateTime.Now;
                        }

                        taOrderOperatorTime.Update(dtActiveTimer);

                        LogManager.GetCurrentClassLogger()
                            .Debug("Stop labor timer for BatchID = {0}", batchId);
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
        /// Starts a process timer for a batch.
        /// </summary>
        /// <param name="batchId"></param>
        public void StartBatchProcessTimer(int batchId)
        {
            BatchProcessesTimeTableAdapter taBatchProcessesTime = null;
            BatchProcessesTableAdapter taBatchProcesses = null;

            try
            {
                taBatchProcessesTime = new BatchProcessesTimeTableAdapter
                {
                    Connection = Connection,
                    Transaction = Transaction
                };

                taBatchProcesses = new BatchProcessesTableAdapter
                {
                    Connection = Connection,
                    Transaction = Transaction
                };

                string workStatus;
                using (var taBatch = new BatchTableAdapter())
                {
                    taBatch.Connection = Connection;
                    taBatch.Transaction = Transaction;

                    workStatus = taBatch.GetWorkStatus(batchId);
                }

                int batchProcessId = taBatchProcesses.GetBatchProcessesID(batchId, workStatus);

                int timerCount = taBatchProcessesTime.GetActiveTimerCount(batchProcessId) ?? 0;

                if (batchProcessId > 0 && timerCount == 0)
                {
                    var dtBatchProcessesTime = new OrderProcessingDataSet.BatchProcessesTimeDataTable();
                    var newTimer = dtBatchProcessesTime.NewBatchProcessesTimeRow();
                    newTimer.BatchProcessID = batchProcessId;
                    newTimer.StartTime = DateTime.Now;
                    newTimer.WorkStatus = workStatus;
                    dtBatchProcessesTime.AddBatchProcessesTimeRow(newTimer);
                    taBatchProcessesTime.Update(dtBatchProcessesTime);

                    LogManager.GetCurrentClassLogger()
                        .Debug("Start process timer for BatchID = {0} BatchProcessID = {1}", batchId, batchProcessId);
                }
            }
            finally
            {
                taBatchProcessesTime?.Dispose();
                taBatchProcesses?.Dispose();
            }
        }

        /// <summary>
        /// Stops a process timer for a batch.
        /// </summary>
        /// <param name="batchId"></param>
        public void StopBatchProcessTimer(int batchId)
        {
            BatchProcessesTimeTableAdapter taBatchProcessesTime = null;
            BatchProcessesTableAdapter taBatchProcesses = null;

            try
            {
                taBatchProcessesTime = new BatchProcessesTimeTableAdapter
                {
                    Connection = Connection,
                    Transaction = Transaction
                };

                taBatchProcesses = new BatchProcessesTableAdapter
                {
                    Connection = Connection,
                    Transaction = Transaction
                };

                string workStatus;
                using (var taBatch = new BatchTableAdapter())
                {
                    taBatch.Connection = Connection;
                    taBatch.Transaction = Transaction;

                    workStatus = taBatch.GetWorkStatus(batchId);
                }

                int batchProcessesId = taBatchProcesses.GetBatchProcessesID(batchId, workStatus);
                var dtActiveTimer = taBatchProcessesTime.GetActiveTimer(batchProcessesId);

                if (dtActiveTimer != null && dtActiveTimer.Count == 1)
                {
                    var activeTimer = dtActiveTimer.First();
                    activeTimer.EndTime = DateTime.Now;
                    taBatchProcessesTime.Update(dtActiveTimer);

                    LogManager.GetCurrentClassLogger()
                        .Debug("Stop process timer for BatchID = {0} BatchProcesses = {1}", batchId, batchProcessesId);
                }
            }
            finally
            {
                taBatchProcessesTime?.Dispose();
                taBatchProcesses?.Dispose();
            }
        }

        private OrderProcessingDataSet.BatchProcessesOperatorRow GetActiveProcessOperator(BatchProcessesOperatorTableAdapter taOperator,
            int batchProcessesId, int batchId, int userId)
        {
            OrderProcessingDataSet.BatchProcessesOperatorRow processOperator;
            var dtOperator = taOperator.GetOperator(batchProcessesId, userId);

            if (dtOperator.Count == 0)
            {
                processOperator = dtOperator.NewBatchProcessesOperatorRow();
                processOperator.BatchProcessID = batchProcessesId;
                processOperator.UserID = userId;
                processOperator.Status = nameof(OperatorStatus.Active);
                dtOperator.AddBatchProcessesOperatorRow(processOperator);

                taOperator.Update(dtOperator);

                LogManager.GetCurrentClassLogger()
                    .Debug("Created new operator for BatchID = {0} BatchProcessID = {1}", batchId, batchProcessesId);
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
                    .Debug("Marked existing operator as active for BatchID = {0} BatchProcessID = {1}", batchId, batchProcessesId);
            }

            return processOperator;
        }

        private OrderProcessingDataSet.BatchOperatorRow GetActiveOperator(BatchOperatorTableAdapter taOperator,
            int batchId, int userId)
        {
            OrderProcessingDataSet.BatchOperatorRow processOperator;
            var dtOperator = taOperator.GetOperator(batchId, userId);

            if (dtOperator.Count == 0)
            {
                processOperator = dtOperator.NewBatchOperatorRow();
                processOperator.BatchID = batchId;
                processOperator.UserID = userId;
                processOperator.Status = nameof(OperatorStatus.Active);
                dtOperator.AddBatchOperatorRow(processOperator);

                taOperator.Update(dtOperator);

                LogManager.GetCurrentClassLogger()
                    .Debug("Created new operator for BatchID = {0}", batchId);
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
                    .Debug("Marked existing operator as active for BatchID = {0}", batchId);
            }

            return processOperator;
        }

        private string GetWorkStatus(int batchId)
        {
            string workStatus;
            using (var taBatch = new BatchTableAdapter())
            {
                taBatch.Connection = Connection;
                taBatch.Transaction = Transaction;

                workStatus = taBatch.GetWorkStatus(batchId);
            }

            return workStatus;
        }

        #endregion
    }
}
