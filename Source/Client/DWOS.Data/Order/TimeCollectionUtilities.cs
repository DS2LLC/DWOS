namespace DWOS.Data.Order
{
    /// <summary>
    /// Contains utility methods related to time collection.
    /// </summary>
    public static class TimeCollectionUtilities
    {
        #region Methods

        #region Order

        /// <summary>
        /// Stops timers after order processing, syncing time spent processing.
        /// </summary>
        /// <param name="orderId"></param>
        public static void CompleteOrderProcessTimers(int orderId)
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    new OrderTimerHelper(conn, transaction).CompleteOrderProcessTimers(orderId);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Stops all order timers without syncing them.
        /// </summary>
        /// <param name="orderId"></param>
        public static void StopAllOrderTimers(int orderId)
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    new OrderTimerHelper(conn, transaction).StopAllOrderTimers(orderId);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Starts a labor timer for a user and order.
        /// </summary>
        /// <param name="orderId">Order to start a timer for.</param>
        /// <param name="userId">User to start a timer for.</param>
        public static void StartOrderLaborTimer(int orderId, int userId)
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    new OrderTimerHelper(conn, transaction).StartOrderLaborTimer(orderId, userId);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Pauses a user's labor timer for an order.
        /// </summary>
        /// <param name="orderId">Order to pause timer for.</param>
        /// <param name="userId">User to pause timer for.</param>
        public static void PauseOrderLaborTimer(int orderId, int userId)
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    new OrderTimerHelper(conn, transaction).PauseOrderLaborTimer(orderId, userId);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Stops a user's labor timer for an order.
        /// </summary>
        /// <param name="orderId">Order to stop timer for.</param>
        /// <param name="userId">User to stop timer for.</param>
        public static void StopOrderLaborTimer(int orderId, int userId)
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    new OrderTimerHelper(conn, transaction).StopOrderLaborTimer(orderId, userId);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Starts the order process timer for an order.
        /// </summary>
        /// <param name="orderId"></param>
        public static void StartOrderProcessTimer(int orderId)
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    new OrderTimerHelper(conn, transaction).StartOrderProcessTimer(orderId);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Stops an order process timer for an order.
        /// </summary>
        /// <param name="orderId"></param>
        public static void StopOrderProcessTimer(int orderId)
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    new OrderTimerHelper(conn, transaction).StopOrderProcessTimer(orderId);
                    transaction.Commit();
                }
            }
        }

        #endregion

        #region Batch

        /// <summary>
        /// Stops timers after batch processing, syncing time spent processing.
        /// </summary>
        /// <param name="batchId"></param>
        public static void CompleteBatchProcessesTimers(int batchId)
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    new BatchTimerHelper(conn, transaction).CompleteBatchProcessesTimers(batchId);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Stops all batch timers without syncing them.
        /// </summary>
        /// <param name="batchId"></param>
        public static void StopAllBatchTimers(int batchId)
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    new BatchTimerHelper(conn, transaction).StopAllBatchTimers(batchId);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Starts a user's labor timer for a batch.
        /// </summary>
        /// <param name="batchId">Batch to start timer for.</param>
        /// <param name="userId">User to start timer for.</param>
        public static void StartBatchLaborTimer(int batchId, int userId)
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    new BatchTimerHelper(conn, transaction).StartBatchLaborTimer(batchId, userId);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Pauses a user's labor timer for a batch.
        /// </summary>
        /// <param name="batchId">Batch to pause timer for.</param>
        /// <param name="userId">User to pause timer for.</param>
        public static void PauseBatchLaborTimer(int batchId, int userId)
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    new BatchTimerHelper(conn, transaction).PauseBatchLaborTimer(batchId, userId);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Stops a user's labor timer for a batch.
        /// </summary>
        /// <param name="batchId">Batch to stop timer for.</param>
        /// <param name="userId">User to stop timer for.</param>
        public static void StopBatchLaborTimer(int batchId, int userId)
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    new BatchTimerHelper(conn, transaction).StopBatchLaborTimer(batchId, userId);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Starts a process timer for a batch.
        /// </summary>
        /// <param name="batchId"></param>
        public static void StartBatchProcessTimer(int batchId)
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    new BatchTimerHelper(conn, transaction).StartBatchProcessTimer(batchId);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Stops a process timer for a batch.
        /// </summary>
        /// <param name="batchId"></param>
        public static void StopBatchProcessTimer(int batchId)
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    new BatchTimerHelper(conn, transaction).StopBatchProcessTimer(batchId);
                    transaction.Commit();
                }
            }
        }

        #endregion

        #endregion
    }
}
