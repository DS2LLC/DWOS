using System;
using System.Linq;
using DWOS.Data.Date;

namespace DWOS.Data
{
    /// <summary>
    /// Calculates the process-by dates for each process in the order based on lead times for each process.
    /// </summary>
    /// <remarks>
    /// The lead time for each day is rounded up.
    /// </remarks>
    public class RoundingLeadTimeScheduler : ILeadTimeScheduler
    {
        #region Properties

        /// <summary>
        /// Gets the lead time persistence for this instance.
        /// </summary>
        public ILeadTimePersistence Data { get; private set; }

        /// <summary>
        /// Gets the start date for this instance.
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Gets the current date/time provider for this instance.
        /// </summary>
        public IDateTimeNowProvider NowProvider { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new, default instance of the <see cref="RoundingLeadTimeScheduler"/>
        /// class using <see cref="DateTimeNowProvider"/>.
        /// </summary>
        public RoundingLeadTimeScheduler() : this(new DateTimeNowProvider())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundingLeadTimeScheduler"/> class.
        /// </summary>
        /// <param name="nowProvider"></param>
        public RoundingLeadTimeScheduler(IDateTimeNowProvider nowProvider)
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
            LoadData(new LeadTimePersistence(), NowProvider.Now);
        }

        /// <summary>
        /// Loads data for this instance using an
        /// <see cref="ILeadTimePersistence"/> instance and an arbitrary start time.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startDate"></param>
        public void LoadData(ILeadTimePersistence data, DateTime startDate)
        {
            Data = data;
            SetStartDate(startDate.Date);
        }

        private void SetStartDate(DateTime startTime)
        {
            if (startTime.DayOfWeek == DayOfWeek.Saturday)
            {
                startTime = startTime.AddDays(2);
                startTime = startTime.AddHours(-startTime.Hour + 1);
            }
            if (startTime.DayOfWeek == DayOfWeek.Sunday)
            {
                startTime = startTime.AddDays(1);
                startTime = startTime.AddHours(-startTime.Hour + 1);
            }

            StartDate = startTime.StartOfDay();
        }

        #endregion

        #region ILeadTimeScheduler Members

        public DateTime UpdateScheduleDates(ILeadTimeOrder order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var currentDate = StartDate.AddDays(-1);

            var receivingLeadTime = Data.ReceivingRolloverTime.Hours > NowProvider.Now.Hour ? 0 : 1;
            currentDate = currentDate.AddBusinessDays(receivingLeadTime);

            var partQuantity = order.PartQuantity ?? 0;

            foreach (var orderProcess in order.Processes.OrderBy(r => r.StepOrder))
            {
                const int hoursPerDay = 24;
                var processLeadTimeDays = (orderProcess.LeadTime?.CalculateHours(partQuantity) ?? 0) / hoursPerDay;

                var leadTimeDays = Data.GetLeadTimeDays(orderProcess.ProcessId)
                    + processLeadTimeDays;

                var leadTimeWholeDays = Convert.ToInt32(Math.Ceiling(leadTimeDays));
                var completeDate = currentDate.AddBusinessDays(leadTimeWholeDays);

                orderProcess.EstEndDate = completeDate;

                currentDate = completeDate;
            }

            //add additional finishing times to the order
            var finishingLeadTime = Data.ShippingLeadTime;

            if (Data.CocEnabled)
                finishingLeadTime += Data.CocLeadTime;

            if (Data.PartMarkingEnabled && order.HasPartMarking)
                finishingLeadTime += Data.PartMarkingLeadTime;

            if (finishingLeadTime > 0)
                currentDate = currentDate.AddBusinessDays(Convert.ToInt32(Math.Ceiling(finishingLeadTime)));

            return currentDate;
        }

        #endregion
    }
}
