using System;

namespace DWOS.Data
{
    public interface ILeadTimeScheduler
    {
        /// <summary>
        /// Updates estimated completion dates for all processes in the order.
        /// </summary>
        /// <param name="order"></param>
        /// <returns>The estimated shipping date for the order</returns>
        DateTime UpdateScheduleDates(ILeadTimeOrder order);
    }
}
