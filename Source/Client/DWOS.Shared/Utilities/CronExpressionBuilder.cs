using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Builds cron expressions.
    /// </summary>
    public sealed class CronExpressionBuilder
    {
        #region Fields

        private HashSet<DayOfWeek> _days;
        private int? _hour;
        private int? _minute;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="CronExpressionBuilder"/> class.
        /// </summary>
        public CronExpressionBuilder()
        {
            _days = new HashSet<DayOfWeek>();
        }

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="CronExpressionBuilder"/> class.
        /// </summary>
        /// A new instance of <see cref="CronExpressionBuilder"/>
        /// </returns>
        public static CronExpressionBuilder Create()
        {
            return new CronExpressionBuilder();
        }

        /// <summary>
        /// Adds the day of the week to the cron expression.
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns>This instance.</returns>
        public CronExpressionBuilder Add(DayOfWeek dayOfWeek)
        {
            _days.Add(dayOfWeek);
            return this;
        }

        /// <summary>
        /// Adds days of the week to the cron expression.
        /// </summary>
        /// <param name="daysOfWeek"></param>
        /// <returns>This instance.</returns>
        public CronExpressionBuilder Add(IEnumerable<DayOfWeek> daysOfWeek)
        {
            foreach (var day in daysOfWeek)
            {
                _days.Add(day);
            }

            return this;
        }

        /// <summary>
        /// Sets the hours and minutes of the cron expression.
        /// </summary>
        /// <param name="timeSinceMidnight"></param>
        /// <returns>This instance.</returns>
        public CronExpressionBuilder SetTime(TimeSpan timeSinceMidnight)
        {
            _hour = timeSinceMidnight.Hours;
            _minute = timeSinceMidnight.Minutes;
            return this;
        }

        /// <summary>
        /// Sets the hour of the cron expression.
        /// </summary>
        /// <param name="hour"></param>
        /// <returns>This instance.</returns>
        public CronExpressionBuilder SetHour(int hour)
        {
            _hour = hour;
            return this;
        }

        /// <summary>
        /// Sets the minute of the cron expression.
        /// </summary>
        /// <param name="minute"></param>
        /// <returns>This instance.</returns>
        public CronExpressionBuilder SetMinute(int minute)
        {
            _minute = minute;
            return this;
        }

        /// <summary>
        /// Builds a cron expression.
        /// </summary>
        /// <returns>Cron expression.</returns>
        public string Build()
        {
            var dayToCronMapping = new Dictionary<DayOfWeek, int>()
            {
                { DayOfWeek.Sunday, 1 },
                { DayOfWeek.Monday, 2 },
                { DayOfWeek.Tuesday, 3 },
                { DayOfWeek.Wednesday, 4 },
                { DayOfWeek.Thursday, 5 },
                { DayOfWeek.Friday, 6 },
                { DayOfWeek.Saturday, 7 }
            };

            // Seconds
            string second = "0";

            // Min
            string minute = "*";
            if (_minute.HasValue)
            {
                minute = _minute.ToString();
            }


            // Hour
            string hour = "*";
            if (_hour.HasValue)
            {
                hour = _hour.ToString();
            }

            // Day of month
            string dayOfMonth = "?";

            // month
            string month = "*";

            // day of week - comma-separated
            string dayOfWeek;
            if (_days.Count > 0)
            {
                dayOfWeek = string.Join(",", _days.Select(d => dayToCronMapping[d].ToString()));
            }
            else
            {
                dayOfWeek = "1-7";
            }

            string year = "*";

            return string.Format("{0} {1} {2} {3} {4} {5} {6}", second, minute, hour, dayOfMonth, month, dayOfWeek, year);
        }

        #endregion
    }
}
