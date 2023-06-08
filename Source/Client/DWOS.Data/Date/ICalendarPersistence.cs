using System;
using Itenso.TimePeriod;
using System.Collections.Generic;

namespace DWOS.Data.Date
{
    /// <summary>
    /// Retrieves holidays from persistence.
    /// </summary>
    public interface ICalendarPersistence
    {
        /// <summary>
        /// Gets a collection of <see cref="ITimePeriod"/> instances
        /// representing holidays.
        /// </summary>
        IEnumerable<ITimePeriod> Holidays { get; }

        /// <summary>
        /// Gets the <see cref="DayOfWeek"/> values that represent the workweek.
        /// </summary>
        IEnumerable<DayOfWeek> Workweek { get; }

        /// <summary>
        /// Gets the operating hours for the workweek.
        /// </summary>
        OperatingHours WorkweekSchedule { get; }

        /// <summary>
        /// Refresh holiday settings.
        /// </summary>
        void Refresh();
    }
}
