using System;
using System.Collections.Generic;
using System.Linq;
using Itenso.TimePeriod;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ScheduleDatasetTableAdapters;
using System.Data;

namespace DWOS.Data.Date
{
    /// <summary>
    /// Implementation of <see cref="ICalendarPersistence"/> that connects
    /// to the database.
    /// </summary>
    public sealed class CalendarPersistence : ICalendarPersistence
    {
        // This class uses locks instead of lazy initialization because
        // values need to be reset after settings change.
        #region Fields

        private IEnumerable<ITimePeriod> _holidays;
        private readonly object _holidayLock = new object();

        private OperatingHours _workweek;
        private readonly object _workweekLock = new object();

        #endregion

        #region ICalendarPersistence Members

        public IEnumerable<ITimePeriod> Holidays
        {
            get
            {
                lock (_holidayLock)
                {
                    if (_holidays != null)
                    {
                        return _holidays;
                    }

                    using (var holidayTable = new ScheduleDataset.WorkHolidayDataTable())
                    {
                        using (var ta = new WorkHolidayTableAdapter())
                        {
                            ta.FillFromDate(holidayTable, DateTime.Now.Subtract(TimeSpan.FromDays(30)).ToShortDateString());
                        }

                        _holidays = holidayTable.Select("1=1", "Holiday").Select<DataRow, ITimePeriod>(dr => new Day((DateTime)dr["Holiday"]));
                    }

                    return _holidays;
                }
            }
        }

        public IEnumerable<DayOfWeek> Workweek => WorkweekSchedule.Workdays;

        public OperatingHours WorkweekSchedule
        {
            get
            {
                lock (_workweekLock)
                {
                    if (_workweek != null)
                    {
                        return _workweek;
                    }

                    var workweek = new OperatingHours();

                    using (var taWorkweek = new DayOfWeekTableAdapter())
                    {
                        using (var dtWorkweek = taWorkweek.GetData())
                        {
                            foreach (var dayRow in dtWorkweek)
                            {
                                var dayOfWeek = (DayOfWeek)dayRow.DayOfWeekID;
                                var day = workweek[dayOfWeek];
                                day.IsWorkday = dayRow.IsWorkday;
                                day.WorkdayStart = dayRow.WorkdayStart;
                                day.WorkdayEnd = dayRow.WorkdayEnd;
                            }
                        }
                    }

                    _workweek = workweek;
                    return _workweek;
                }
            }
        }

        public void Refresh()
        {
            lock (_holidayLock)
            {
                _holidays = null;
            }

            lock (_workweekLock)
            {
                _workweek = null;
            }
        }

        #endregion
    }
}
