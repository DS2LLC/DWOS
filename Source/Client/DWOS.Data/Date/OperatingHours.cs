using System;
using System.Collections.Generic;
using System.Linq;
namespace DWOS.Data.Date
{
    /// <summary>
    /// Represents operating hours for a company.
    /// </summary>
    public class OperatingHours
    {
        #region Fields

        private List<Day> _days = new List<Day>
        {
            new Day(DayOfWeek.Sunday),
            new Day(DayOfWeek.Monday),
            new Day(DayOfWeek.Tuesday),
            new Day(DayOfWeek.Wednesday),
            new Day(DayOfWeek.Thursday),
            new Day(DayOfWeek.Friday),
            new Day(DayOfWeek.Saturday)
        };

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="Day"/> instance for the given <see cref="DayOfWeek"/>.
        /// </summary>
        /// <param name="dayOfWeek">
        /// The <see cref="DayOfWeek"/> to retrieve information for.
        /// </param>
        /// <returns>
        /// An <see cref="Day"/> instance.
        /// </returns>
        public Day this[DayOfWeek dayOfWeek] =>
            _days[(int)dayOfWeek];

        /// <summary>
        /// Gets a list of <see cref="DayOfWeek"/> that belong to the
        /// workweek for this instance.
        /// </summary>
        public IEnumerable<DayOfWeek> Workdays =>
            _days.Where(d => d.IsWorkday).Select(d => d.DayOfWeek);

        #endregion

        #region Day

        /// <summary>
        /// Represents a day within <see cref="OperatingHours"/>.
        /// </summary>
        public class Day
        {
            #region Properties

            /// <summary>
            /// Gets the day of week for this instance.
            /// </summary>
            public DayOfWeek DayOfWeek { get; }

            /// <summary>
            /// Gets or sets a value that indicates that this instance belongs
            /// to the company's workweek.
            /// </summary>
            /// <value>
            /// <c>true</c> if it's within the workweek;
            /// otherwise, <c>false</c>.
            /// </value>
            public bool IsWorkday { get; set; }

            /// <summary>
            /// Gets or sets the start of the workday.
            /// </summary>
            /// <remarks>
            /// Default value should be 9:00 AM.
            /// </remarks>
            public TimeSpan WorkdayStart { get; set; } =
                new TimeSpan(9, 0, 0);

            /// <summary>
            /// Gets or sets the end of the workday.
            /// </summary>
            /// <remarks>
            /// Default value should be 5:00 PM.
            /// </remarks>
            public TimeSpan WorkdayEnd { get; set; } =
                new TimeSpan(17, 0, 0);

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the <see cref="Day"/> class.
            /// </summary>
            /// <param name="dayOfWeek"></param>
            public Day(DayOfWeek dayOfWeek)
            {
                DayOfWeek = dayOfWeek;
            }

            #endregion
        }

        #endregion
    }
}
