using System;
using System.Collections.Generic;
using System.Linq;
using Itenso.TimePeriod;
using DWOS.Data.Date;
using System.Text.RegularExpressions;
using DWOS.Shared.Utilities;
using NLog;

namespace DWOS.Data
{
    /// <summary>
    /// Defines utility methods for <see cref="DateTime"/> instances.
    /// </summary>
    public static class DateUtilities
    {
        #region Fields

        /// <summary>
        /// The number of minutes in an hour.
        /// </summary>
        public const int MINUTES_PER_HOUR = 60;

        /// <summary>
        /// The number of hours in a day.
        /// </summary>
        public const int HOURS_PER_DAY = 24;

        /// <summary>
        /// The number of days in a week.
        /// </summary>
        private const int DAYS_PER_WEEK = 7;

        #endregion

        #region Properties

        private static IEnumerable<ITimePeriod> Holidays
        {
            get
            {
                try
                {
                    var holidayPersistence = DependencyContainer.Resolve<ICalendarPersistence>();
                    return holidayPersistence.Holidays;
                }
                catch (KeyNotFoundException)
                {
                    LogManager.GetCurrentClassLogger().Warn("{0} not registered", nameof(ICalendarPersistence));
                    return (new CalendarPersistence()).Holidays;
                }
            }
        }

        private static IEnumerable<DayOfWeek> Workweek
        {
            get
            {
                var workweek = new List<DayOfWeek>();
                try
                {
                    var holidayPersistence = DependencyContainer.Resolve<ICalendarPersistence>();
                    workweek.AddRange(holidayPersistence.Workweek);
                }
                catch (KeyNotFoundException)
                {
                    LogManager.GetCurrentClassLogger().Warn("{0} not registered", nameof(ICalendarPersistence));
                    workweek.AddRange(new CalendarPersistence().Workweek);
                }

                if (workweek.Count == 0)
                {
                    LogManager.GetCurrentClassLogger().Warn("Workweek is empty - using default.");
                    workweek.Add(DayOfWeek.Monday);
                }

                return workweek;
            }
        }

        private static OperatingHours WorkweekSchedule
        {
            get
            {
                var workweek = new OperatingHours();
                try
                {
                    var calendarPersistence = DependencyContainer.Resolve<ICalendarPersistence>();
                    workweek = calendarPersistence.WorkweekSchedule;
                }
                catch (KeyNotFoundException)
                {
                    LogManager.GetCurrentClassLogger().Warn("{0} not registered", nameof(ICalendarPersistence));
                }

                if (!workweek.Workdays.Any())
                {
                    LogManager.GetCurrentClassLogger().Warn("Workweek is empty - using default.");
                    workweek[DayOfWeek.Monday].IsWorkday = true;
                }

                return workweek;
            }
        }

        private static IDateTimeNowProvider DateProvider
        {
            get
            {
                try
                {
                    return DependencyContainer.Resolve<IDateTimeNowProvider>();
                }
                catch (KeyNotFoundException)
                {
                    LogManager.GetCurrentClassLogger().Warn("{0} not registered", nameof(IDateTimeNowProvider));
                    return new DateTimeNowProvider();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the first day of year.
        /// </summary>
        /// <param name="dtDate">The dt date.</param>
        /// <returns>First day of the year</returns>
        public static DateTime GetFirstDayOfYear(DateTime dtDate)
        {
            var y = new Year(dtDate);
            return y.FirstDayStart;
        }

        /// <summary>
        /// Gets the first day of the year.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfYear(int year)
        {
            return new Year(year).FirstDayStart;
        }

        /// <summary>
        ///   Get the first day of the month for
        ///   any full date submitted
        /// </summary>
        /// <param name="dtDate"> </param>
        /// <returns> </returns>
        public static DateTime GetFirstDayOfMonth(DateTime dtDate)
        {
            var m = new Month(dtDate);
            return m.FirstDayStart;
        }

        /// <summary>
        ///   Get the last day of the month for any
        ///   full date
        /// </summary>
        /// <param name="dtDate"> </param>
        /// <returns> </returns>
        public static DateTime GetLastDayOfMonth(DateTime dtDate)
        {
            var m = new Month(dtDate);
            return m.LastDayStart;
        }

        /// <summary>
        ///   Adds working days to the start date.
        /// </summary>
        /// <param name="start"> The start. </param>
        /// <param name="days"> The days. </param>
        /// <returns> </returns>
        public static DateTime AddBusinessDays(this DateTime start, int days)
        {
            var startDay = new Day(start);
            var filter = CreateVisitorFilter();

            var ds      = new DaySeeker(filter);
            Day endDay  = ds.FindDay(startDay, days);

            return endDay.Start;
        }

        public static DateTime AddBusinessDays(this DateTime start, double days)
        {
            int wholeDays = (int)Math.Truncate(days);
            var timeOfDay = start.TimeOfDay.Add(TimeSpan.FromDays(days - wholeDays));
            var date = start.AddBusinessDays(wholeDays);
            return date.Add(timeOfDay);
        }

        /// <summary>
        ///   Get the date for the first day of the week containing another date.
        /// </summary>
        /// <param name="dt"> The dt. </param>
        /// <param name="startOfWeek"> The start of week. </param>
        /// <returns> </returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if(diff < 0)
                diff += DAYS_PER_WEEK;

            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Gets the date for the last day of the week containing another date.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var w = new Week(dt);
            return w.LastDayOfWeek;
        }

        /// <summary>
        ///   Gets the number of business days between two dates.
        /// </summary>
        /// <param name="dateFrom"> The date from. </param>
        /// <param name="dateTo"> The date to. </param>
        /// <returns> </returns>
        public static int GetBusinessDays(DateTime dateFrom, DateTime dateTo)
        {
            dateFrom = TimeTrim.Hour(dateFrom);
            dateTo = TimeTrim.Hour(dateTo, 23, 59);

            var filter = CreateCollectorFilter();

            var testPeriod = new CalendarTimeRange(dateFrom, dateTo);
            var collector = new CalendarPeriodCollector(filter, testPeriod);

            collector.CollectDays();
            return collector.Periods.Count;
        }

        /// <summary>
        /// Returns the number of business hours between two dates.
        /// </summary>
        /// <param name="dateFrom">The date from.</param>
        /// <param name="dateTo">The date to.</param>
        /// <returns></returns>
        public static int GetBusinessHours(DateTime dateFrom, DateTime dateTo)
        {
            // Itenso.TimePeriod  hours calculation has an off-by-one error in v2.0.0.
            // Using TimeSpan to calculate time difference.
            int days = GetBusinessDays(dateFrom, dateTo);

            TimeSpan timeFrom = dateFrom.TimeOfDay;
            TimeSpan timeTo = dateTo.TimeOfDay;
            return (days * HOURS_PER_DAY) + timeTo.Subtract(timeFrom).Hours;
        }

        /// <summary>
        /// Gets the business days till late, exclude weeknds
        /// </summary>
        /// <param name="dateTo">The date to.</param>
        /// <returns>The number of business days until the time is considered late, or -1 if it is already late.</returns>
        public static int GetBusinessDaysTillLate(DateTime dateTo)
        {
            var now = DateProvider.Now.Date;
            dateTo = dateTo.Date;

            if(dateTo < now)
                return -1; //aka late

            //ensure dateTo is not on a weekend
            while (dateTo.DayOfWeek == DayOfWeek.Saturday || dateTo.DayOfWeek == DayOfWeek.Sunday)
            {
                //RULE: move back to previous workday
                dateTo = dateTo.AddDays(-1);
            }

            //double check to see if new to date is before today
            if (dateTo < now)
                return -1; //aka still late
            
            if(dateTo == now)
                return 0;

            var filter = CreateCollectorFilter();

            var timeRange = new CalendarTimeRange(now, dateTo); //remove time from the equation
            var collector   = new CalendarPeriodCollector(filter, timeRange);

            collector.CollectDays();
            
            return collector.Periods.Count;
        }

        /// <summary>
        ///   Get the start of the day, by reseting the time portion of the datetime.
        /// </summary>
        /// <param name="dt"> The dt. </param>
        /// <returns> </returns>
        public static DateTime StartOfDay(this DateTime dt)
        {
            return TimeTrim.Hour(dt);
        }

        public static DateTime StartOfBusinessDay(this DateTime dt)
        {
            var endOfDay = WorkweekSchedule[dt.DayOfWeek].WorkdayStart;
            return dt.Date.Add(endOfDay);
        }

        /// <summary>
        /// Ends the of day.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="trimSeconds">if set to <c>true</c> [trim seconds]. This is required to prevent database time stamps that don't track seconds from rounding up to the next whole minute.</param>
        /// <returns>DateTime.</returns>
        public static DateTime EndOfDay(this DateTime dt, bool trimSeconds = true)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, trimSeconds ? 0 : 59); 
        }

        public static DateTime EndOfBusinessDay(this DateTime dt)
        {
            var endOfDay = WorkweekSchedule[dt.DayOfWeek].WorkdayEnd;
            return dt.Date.Add(endOfDay);
        }

        public static bool IsWorkday(this DateTime dt) =>
            WorkweekSchedule[dt.DayOfWeek].IsWorkday;

        /// <summary>
        /// Difference in months between two dates
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public static int TotalMonths(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            monthsApart = Math.Abs(monthsApart);

            return monthsApart +1;
        }

        /// <summary>
        /// Returns a string representation for the number of minutes.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static string ToDifferenceShortHand(int minutes)
        {
            var time = minutes;

            if (time < 0)
                return "0 M";

            if (Math.Abs(time) < MINUTES_PER_HOUR)
                return time + " M";

            var hours = time / Convert.ToDouble(MINUTES_PER_HOUR);

            if (Math.Abs(hours) < HOURS_PER_DAY)
                return hours.ToString("n1") + " H";

            var days = hours / Convert.ToDouble(HOURS_PER_DAY);

            return days.ToString("n1") + " D";
        }

        /// <summary>
        /// Returns the number of minutes represented by the input string.
        /// </summary>
        /// <remarks>
        /// This method expects a string generated in the format used by
        /// <see cref="ToDifferenceShortHand(int)"/>.
        /// </remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int FromDifferenceShortHand(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }

            var trimmedInput = input.Trim().ToUpper();
            var numericString = Regex.Match(trimmedInput, @"^[+-]?\d*\.?\d*").Value;

            int minutes = 0;
            if (!string.IsNullOrEmpty(numericString) && numericString != ".")
            {
                var period = trimmedInput.Last();
                var numericInput = double.Parse(numericString);

                if (period == 'M')
                {
                    // Minute
                    minutes = Convert.ToInt32(numericInput);
                }
                else if (period == 'H')
                {
                    // Hour
                    minutes = Convert.ToInt32(numericInput * Convert.ToDouble(MINUTES_PER_HOUR));
                }
                else if (period == 'D')
                {
                    // Day
                    minutes = Convert.ToInt32(numericInput * Convert.ToDouble(MINUTES_PER_HOUR * HOURS_PER_DAY));
                }
            }

            return Math.Max(minutes, 0);
        }

        /// <summary>
        /// Gets the number of minutes between two dates grouped by date.
        /// </summary>
        /// <remarks>
        /// Keys are individual dates. Values are number of minutes per day.
        /// </remarks>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static IDictionary<DateTime, int> MinutesGroupedByDay(DateTime startTime, DateTime endTime)
        {
            if (endTime < startTime)
            {
                throw new ArgumentException(nameof(endTime),
                    "endTime cannot come before startTime");
            }

            TimeSpan span = endTime - startTime;

            if (span.Days == 0)
            {
                return new Dictionary<DateTime, int>()
                {
                    { startTime.Date, Convert.ToInt32(Math.Round(span.TotalMinutes)) }
                };
            }
            else
            {
                var dict = new Dictionary<DateTime, int>();

                var daysDifference = Convert.ToInt32(Math.Round((endTime.Date - startTime.Date).TotalDays, MidpointRounding.AwayFromZero));

                dict[startTime.Date] = Convert.ToInt32(Math.Round((startTime.Date.AddDays(1) - startTime).TotalMinutes));

                // Add an entire day for each full day in the range.
                for (int i = 1; i < daysDifference; ++i)
                {
                    dict[startTime.Date.AddDays(i)] = Convert.ToInt32(TimeSpan.FromDays(1).TotalMinutes);
                }

                dict[endTime.Date] = Convert.ToInt32(Math.Round((endTime - endTime.Date).TotalMinutes));

                return dict;
            }
        }

        /// <summary>
        /// Returns a collection containing the first day of each month in a
        /// range of months.
        /// </summary>
        /// <param name="from">The starting date of the range.</param>
        /// <param name="to">The ending date of the range.</param>
        /// <returns></returns>
        public static IEnumerable<DateTime> MonthsInRange(DateTime from, DateTime to)
        {
            var currentDate = new DateTime(from.Year, from.Month, 1);

            var months = new List<DateTime>();
            while (currentDate <= to)
            {
                months.Add(currentDate);
                currentDate = currentDate.AddMonths(1);
            }

            return months;
        }

        /// <summary>
        /// Creates a new filter that includes only business days.
        /// </summary>
        /// <returns></returns>
        private static CalendarVisitorFilter CreateVisitorFilter()
        {
            var filter = new CalendarVisitorFilter();
            filter.WeekDays.Clear();

            foreach (var day in Workweek)
            {
                filter.WeekDays.Add(day);
            }

            filter.ExcludePeriods.AddAll(Holidays);
            return filter;
        }

        private static CalendarPeriodCollectorFilter CreateCollectorFilter()
        {
            var filter = new CalendarPeriodCollectorFilter();

            filter.WeekDays.Clear();

            foreach (var day in Workweek)
            {
                filter.WeekDays.Add(day);
            }

            filter.ExcludePeriods.AddAll(Holidays);
            return filter;
        }

        #endregion
    }
}