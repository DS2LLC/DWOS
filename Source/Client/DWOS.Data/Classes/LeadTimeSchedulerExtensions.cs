using DWOS.Data.Datasets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Data
{
    /// <summary>
    /// Defines extension methods for <see cref="ILeadTimeScheduler"/>.
    /// </summary>
    public static class LeadTimeSchedulerExtensions
    {
        #region Methods

        /// <summary>
        /// Updates estimated completion dates for all processes in the order.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="processLeadTimes"></param>
        /// <returns>The estimated shipping date for the order</returns>
        public static DateTime UpdateScheduleDates(this ILeadTimeScheduler scheduler, OrdersDataSet.OrderRow order, OrderProcessLeadTimes processLeadTimes)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (processLeadTimes == null)
            {
                throw new ArgumentNullException(nameof(processLeadTimes));
            }

            return scheduler.UpdateScheduleDates(new RowLeadTimeOrder(order, processLeadTimes));
        }

        #endregion

        #region RowLeadTimeOrder

        private class RowLeadTimeOrder : ILeadTimeOrder
        {
            #region Properties

            public OrdersDataSet.OrderRow Row { get; }

            public OrderProcessLeadTimes LeadTimes { get; }

            #endregion

            #region Methods

            public RowLeadTimeOrder(OrdersDataSet.OrderRow row, OrderProcessLeadTimes leadTimes)
            {
                Row = row ?? throw new ArgumentNullException(nameof(row));
                LeadTimes = leadTimes ?? throw new ArgumentNullException(nameof(leadTimes));
            }

            #endregion

            #region ILeadTimeOrder Members

            public int OrderId => Row.OrderID;

            public int? PartQuantity => Row.IsPartQuantityNull()
                ? (int?)null
                : Row.PartQuantity;

            public bool HasPartMarking => Row.GetOrderPartMarkRows().Length > 0;

            public IEnumerable<ILeadTimeProcess> Processes => Row
                .GetOrderProcessesRows()
                .Select(p => new RowLeadTimeProcess(p, LeadTimes.LeadTimeFor(p.OrderProcessesID)));

            #endregion
        }

        #endregion

        #region RowLeadTimeProcess

        private class RowLeadTimeProcess : ILeadTimeProcess
        {
            #region Properties

            public OrdersDataSet.OrderProcessesRow Row { get; }

            #endregion

            #region Methods

            public RowLeadTimeProcess(OrdersDataSet.OrderProcessesRow row, ProcessLeadTime leadTime)
            {
                Row = row ?? throw new ArgumentNullException(nameof(row));
                LeadTime = leadTime;
            }

            #endregion

            #region ILeadTimeProcess Members

            public int ProcessId => Row.ProcessID;

            public int StepOrder => Row.StepOrder;

            public ProcessLeadTime LeadTime { get; }

            public DateTime? EstEndDate
            {
                get => Row.IsEstEndDateNull()
                    ? (DateTime?)null
                    : Row.EstEndDate;
                set
                {
                    if (value.HasValue)
                    {
                        Row.EstEndDate = value.Value;
                    }
                    else
                    {
                        Row.SetEstEndDateNull();
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
