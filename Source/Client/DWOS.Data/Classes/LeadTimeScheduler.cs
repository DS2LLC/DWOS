using System;
using System.Linq;
using DWOS.Data.Date;

namespace DWOS.Data
{
    public class LeadTimeScheduler : ILeadTimeScheduler
    {
        /// <summary>
        /// Gets the lead time persistence for this instance.
        /// </summary>
        public ILeadTimePersistence Data { get; private set; }

        /// <summary>
        /// Gets the current date/time provider for this instance.
        /// </summary>
        public IDateTimeNowProvider NowProvider { get; set; }

        /// <summary>
        /// Initializes a new, default instance of the <see cref="LeadTimeScheduler"/>
        /// class using <see cref="DateTimeNowProvider"/>.
        /// </summary>
        public LeadTimeScheduler() : this(new DateTimeNowProvider())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LeadTimeScheduler"/> class.
        /// </summary>
        /// <param name="nowProvider"></param>
        public LeadTimeScheduler(IDateTimeNowProvider nowProvider)
        {
            NowProvider = nowProvider
                ?? throw new ArgumentNullException(nameof(nowProvider));
        }

        /// <summary>
        /// Loads data for this instance using a new
        /// <see cref="LeadTimePersistence"/> instance and the current time.
        /// </summary>
        public void LoadData()
        {
            LoadData(new LeadTimePersistence());
        }

        /// <summary>
        /// Loads data for this instance using an
        /// <see cref="ILeadTimePersistence"/> instance and an arbitrary start time.
        /// </summary>
        /// <param name="data"></param>
        public void LoadData(ILeadTimePersistence data)
        {
            Data = data;
        }

        private DateTime DateWithinBusinessHours(DateTime input)
        {
            DateTime output;
            if (input < input.StartOfBusinessDay())
            {
                output = input.StartOfBusinessDay();
            }
            else if (!input.IsWorkday() || input > input.EndOfBusinessDay())
            {
                output = input.AddBusinessDays(1).StartOfBusinessDay();
            }
            else
            {
                output = input;
            }

            return output;
        }

        private DateTime GetStartDate(DateTime startTime)
        {
            if (startTime.IsWorkday())
            {
                return DateWithinBusinessHours(startTime);
            }

            return startTime.AddBusinessDays(1).StartOfBusinessDay();
        }

        #region ILeadTimeScheduler Members

        public DateTime UpdateScheduleDates(ILeadTimeOrder order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            // Set start date
            // The general idea is to set the start date a day earlier because
            // "one day lead time" means "ready by end of day."
            var now = NowProvider.Now;
            var currentDate = GetStartDate(now).AddDays(-1);

            if (Data.ReceivingRolloverTime.Hours <= NowProvider.Now.Hour)
            {
                // PO came in late; start on it tomorrow by using today as the starting point
                currentDate = currentDate.AddDays(1);
            }

            var partQuantity = order.PartQuantity ?? 0;

            // Set estimated completion dates for processes
            foreach (var orderProcess in order.Processes.OrderBy(r => r.StepOrder))
            {
                const int hoursPerDay = 24;
                var processLeadTimeDays = (orderProcess.LeadTime?.CalculateHours(partQuantity) ?? 0) / hoursPerDay;

                var leadTimeDays = Data.GetLeadTimeDays(orderProcess.ProcessId)
                    + processLeadTimeDays;

                var completeDate = currentDate.AddBusinessDays((double)leadTimeDays);
                orderProcess.EstEndDate = completeDate;
                currentDate = DateWithinBusinessHours(completeDate);
            }

            //add additional finishing times to the order
            var finishingLeadTime = Data.ShippingLeadTime;

            if (Data.CocEnabled)
                finishingLeadTime += Data.CocLeadTime;

            if (Data.PartMarkingEnabled && order.HasPartMarking)
                finishingLeadTime += Data.PartMarkingLeadTime;

            if (finishingLeadTime > 0)
                currentDate = currentDate.AddBusinessDays(Convert.ToInt32(Math.Ceiling(finishingLeadTime)));

            return currentDate.EndOfBusinessDay();
        }

        #endregion
    }
}
