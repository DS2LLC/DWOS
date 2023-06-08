using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderHistoryDataSetTableAdapters;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Shared.Utilities;

namespace DWOS.Data.Order
{
    /// <summary>
    /// Provides utility functionality related to manual scheduling.
    /// </summary>
    public static class SchedulingHelper
    {
        #region Methods

        /// <summary>
        /// Saves a schedule of orders and batches.
        /// </summary>
        /// <param name="orderSchedule">
        /// All orders that can be scheduled.
        /// </param>
        /// <param name="batchSchedule">
        /// All batches that can be scheduled.
        /// </param>
        /// <param name="currentUser">
        /// The current user.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="orderSchedule"/>, <paramref name="batchSchedule"/>,
        /// or <paramref name="currentUser"/> is <c>null</c>.
        /// </exception>
        public static void SaveSchedule(IEnumerable<OrderScheduleItem> orderSchedule, IEnumerable<BatchScheduleItem> batchSchedule, ISecurityUserInfo currentUser)
        {
            if (orderSchedule == null)
            {
                throw new ArgumentNullException(nameof(orderSchedule));
            }

            if (batchSchedule == null)
            {
                throw new ArgumentNullException(nameof(batchSchedule));
            }

            if (currentUser == null)
            {
                throw new ArgumentNullException(nameof(currentUser));
            }

            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    var taOrderStatus = new OrderTableAdapter
                    {
                        Connection = conn,
                        Transaction = transaction
                    };

                    var taBatch = new BatchTableAdapter
                    {
                        Connection = conn,
                        Transaction = transaction
                    };

                    var taBatchOrder = new BatchOrderTableAdapter
                    {
                        Connection = conn,
                        Transaction = transaction
                    };

                    var taOrderHistory = new OrderHistoryTableAdapter
                    {
                        Connection = conn,
                        Transaction = transaction
                    };

                    try
                    {
                        foreach (var scheduleItem in orderSchedule)
                        {
                            taOrderStatus.UpdateSchedulePriority(scheduleItem.OrderId, scheduleItem.CurrentSchedulePriority);

                            var scheduleMsg = GetScheduleMessage(scheduleItem);

                            if (!string.IsNullOrWhiteSpace(scheduleMsg))
                            {
                                taOrderHistory.UpdateOrderHistory(
                                    scheduleItem.OrderId,
                                    "Scheduling",
                                    scheduleMsg,
                                    currentUser.UserName);
                            }
                        }

                        foreach (var scheduleItem in batchSchedule)
                        {
                            // Update schedule
                            taBatch.UpdateSchedulePriority(scheduleItem.BatchId, scheduleItem.CurrentSchedulePriority);

                            // Add a history item for each order in the batch
                            List<int> orderIds;
                            using (var dtBatchOrder = new OrdersDataSet.BatchOrderDataTable())
                            {
                                taBatchOrder.FillByBatchID(dtBatchOrder, scheduleItem.BatchId);
                                orderIds = dtBatchOrder.Select(batchOrder => batchOrder.OrderID).ToList();
                            }
                            var scheduleMsg = GetScheduleMessage(scheduleItem);

                            if (!string.IsNullOrWhiteSpace(scheduleMsg))
                            {
                                foreach (var orderId in orderIds)
                                {
                                    taOrderHistory.UpdateOrderHistory(
                                        orderId,
                                        "Scheduling",
                                        $"Batch {scheduleItem.BatchId} - {scheduleMsg}",
                                        currentUser.UserName);
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    finally
                    {
                        taOrderStatus.Dispose();
                        taOrderHistory.Dispose();
                        taBatch.Dispose();
                    }
                }
            }
        }

        private static string GetScheduleMessage(ScheduleItemBase scheduleItem)
        {
            string scheduleMsg;

            if (scheduleItem.PreviousSchedulePriority != scheduleItem.CurrentSchedulePriority)
            {
                scheduleMsg = scheduleItem.CurrentSchedulePriority == 0
                    ? "Removed schedule."
                    : "Changed schedule priority.";
            }
            else
            {
                scheduleMsg = scheduleItem.CurrentSchedulePriority == 0
                    ? null
                    : "Schedule priority unchanged after changing department schedule.";
            }

            return scheduleMsg;
        }

        #endregion

        #region ScheduleItemBase

        /// <summary>
        /// Base class for schedule items.
        /// </summary>
        public abstract class ScheduleItemBase
        {
            /// <summary>
            /// Gets or sets the previous priority of this instance.
            /// </summary>
            public int PreviousSchedulePriority { get; set; }

            /// <summary>
            /// Gets or sets the current schedule priority of this instance.
            /// </summary>
            public int CurrentSchedulePriority { get; set; }
        }

        #endregion

        #region OrderScheduleItem

        /// <summary>
        /// Represents an order.
        /// </summary>
        /// <remarks>
        /// May represent a scheduled order or an unscheduled order.
        /// </remarks>
        public class OrderScheduleItem : ScheduleItemBase
        {
            /// <summary>
            /// Gets or sets the order ID of this instance.
            /// </summary>
            public int OrderId { get; set; }
        }

        #endregion

        #region BatchScheduleItem

        /// <summary>
        /// Represents an batch.
        /// </summary>
        /// <remarks>
        /// May represent a scheduled batch or an unscheduled batch.
        /// </remarks>
        public class BatchScheduleItem : ScheduleItemBase
        {
            /// <summary>
            /// Gets or sets the batch ID of this instance.
            /// </summary>
            public int BatchId { get; set; }
        }

        #endregion
    }
}
