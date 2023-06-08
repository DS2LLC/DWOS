using System;
using System.Collections.Generic;
using System.Globalization;

namespace DWOS.Data
{
    /// <summary>
    /// Represents a week in a year as specified by the database.
    /// </summary>
    /// <remarks>
    /// Year starts on January 1st (SQL Server).
    /// Week starts on Sunday (server configuration).
    /// </remarks>
    public class DbWeek : IEquatable<DbWeek>
    {
        #region Fields

        public const DayOfWeek StartOfWeek = DayOfWeek.Sunday;
        private static readonly GregorianCalendar Calendar =
            new GregorianCalendar(GregorianCalendarTypes.USEnglish);

        #endregion

        #region Properties

        public int Year { get; }

        public int WeekInYear { get; }

        public DateTime StartDateOfThisWeek =>
            Calendar.AddWeeks(DateUtilities.GetFirstDayOfYear(Year).StartOfWeek(StartOfWeek), WeekInYear - 1);

        #endregion

        #region Methods

        public DbWeek(int year, int weekInYear)
        {
            Year = year;
            WeekInYear = weekInYear;
        }

        public static List<DbWeek> WeeksInRange(DateTime from, DateTime to)
        {
            var currentDate = from.Date;
            var toDate = to.StartOfWeek(StartOfWeek);

            var weeks = new List<DbWeek>();
            while (currentDate <= toDate)
            {
                var year = currentDate.Year;
                var week = Calendar.GetWeekOfYear(currentDate, CalendarWeekRule.FirstDay, StartOfWeek);
                weeks.Add(new DbWeek(year, week));

                var nextWeek = currentDate.StartOfWeek(StartOfWeek).AddDays(7);

                if (nextWeek.Year != currentDate.Year && Calendar.GetWeekOfYear(nextWeek, CalendarWeekRule.FirstDay, StartOfWeek) != 1)
                {
                    // Add first week of year
                    weeks.Add(new DbWeek(nextWeek.Year, 1));
                }

                currentDate = nextWeek;
            }

            return weeks;
        }

        public override string ToString() =>
            $"Year - {Year}, WeekInYear - {WeekInYear}";

        public string ToDisplayString()
        {
            // Even though the week starts on Sunday, and this instance still
            // covers Sunday, show Monday instead as it's typically the start
            // of the business week.
            var sunday = StartDateOfThisWeek;
            var monday = sunday.AddDays((int)DayOfWeek.Monday);
            return monday.ToShortDateString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DbWeek);
        }

        public static bool operator ==(DbWeek a, DbWeek b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        public static bool operator !=(DbWeek a, DbWeek b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            var hash = 23;
            hash = hash * 29 + Year.GetHashCode();
            hash = hash * 29 + WeekInYear.GetHashCode();
            return hash;
        }

        #endregion

        #region IEquatable<Week> Members

        public bool Equals(DbWeek other)
        {
            return other != null &&
                Year == other.Year &&
                WeekInYear == other.WeekInYear;
        }

        #endregion

        }
}
