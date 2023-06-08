using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DWOS.Data.Date;
using Itenso.TimePeriod;
using System.Collections.Generic;
using DWOS.Shared.Utilities;
using System.Linq;

namespace DWOS.Data.Tests
{
    [TestClass]
    public class DateUtilitiesTests
    {
        private const int MINUTES_IN_HOUR = 60;
        private const int MINUTES_IN_DAY = MINUTES_IN_HOUR * 24;
        private Mock<ICalendarPersistence> _calendarMock;
        private Mock<IDateTimeNowProvider> _dateTimeNowMock;

        [TestInitialize]
        public void Initialize()
        {
            _calendarMock = new Mock<ICalendarPersistence>();
            _calendarMock.Setup(m => m.Holidays).Returns(new List<ITimePeriod>());
            _calendarMock.Setup(m => m.Workweek).Returns(new List<DayOfWeek>
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
            });

            DependencyContainer.Register(_calendarMock.Object);

            _dateTimeNowMock = new Mock<IDateTimeNowProvider>();
            _dateTimeNowMock.Setup(m => m.Now).Returns(DateTime.Now);

            DependencyContainer.Register(_dateTimeNowMock.Object);
        }

        [TestMethod]
        public void GetBusinessDaysTest()
        {
            DateTime dateFrom;
            DateTime dateTo;

            // Normal case
            dateFrom = new DateTime(2015, 10, 19);
            dateTo = new DateTime(2015, 10, 20);
            Assert.AreEqual(1, DateUtilities.GetBusinessDays(dateFrom, dateTo));

            // Do not consider hour/minute/second
            dateFrom = new DateTime(2015, 10, 19, 1, 0, 0);
            dateTo = new DateTime(2015, 10, 21).AddSeconds(-1);
            Assert.AreEqual(1, DateUtilities.GetBusinessDays(dateFrom, dateTo));

            // Do not count weekends
            dateFrom = new DateTime(2015, 10, 23);
            dateTo = new DateTime(2015, 10, 26);
            Assert.AreEqual(1, DateUtilities.GetBusinessDays(dateFrom, dateTo));

            // Do not count holidays
            _calendarMock.Setup(holiday => holiday.Holidays).Returns(new List<ITimePeriod>() { new Day(2015, 10, 20) });
            dateFrom = new DateTime(2015, 10, 19);
            dateTo = new DateTime(2015, 10, 21);
            Assert.AreEqual(1, DateUtilities.GetBusinessDays(dateFrom, dateTo));

            // Use workweek from calendar
            _calendarMock.Setup(m => m.Workweek).Returns(new List<DayOfWeek> { DayOfWeek.Monday});
            dateFrom = new DateTime(2015, 10, 21);
            dateTo = new DateTime(2015, 10, 28);
            Assert.AreEqual(1, DateUtilities.GetBusinessDays(dateFrom, dateTo));
        }

        [TestMethod]
        public void GetBusinessHoursTest()
        {
            DateTime dateFrom;
            DateTime dateTo;

            // Normal cases
            dateFrom = new DateTime(2015, 10, 19);
            dateTo = new DateTime(2015, 10, 20);
            Assert.AreEqual(24, DateUtilities.GetBusinessHours(dateFrom, dateTo));

            dateFrom = new DateTime(2015, 10, 19);
            dateTo = new DateTime(2015, 10, 21);
            Assert.AreEqual(48, DateUtilities.GetBusinessHours(dateFrom, dateTo));

            dateFrom = new DateTime(2015, 10, 19, 9, 37, 00);
            dateTo = new DateTime(2015, 10, 19, 10, 36, 00);
            Assert.AreEqual(0, DateUtilities.GetBusinessHours(dateFrom, dateTo));

            dateFrom = new DateTime(2015, 10, 19, 9, 37, 00);
            dateTo = new DateTime(2015, 10, 20, 9, 37, 00);
            Assert.AreEqual(24, DateUtilities.GetBusinessHours(dateFrom, dateTo));

            dateFrom = new DateTime(2015, 10, 19, 9, 37, 00);
            dateTo = new DateTime(2015, 10, 20, 10, 37, 00);
            Assert.AreEqual(25, DateUtilities.GetBusinessHours(dateFrom, dateTo));

            dateFrom = new DateTime(2015, 10, 19, 9, 37, 00);
            dateTo = new DateTime(2015, 10, 20, 8, 37, 00);
            Assert.AreEqual(23, DateUtilities.GetBusinessHours(dateFrom, dateTo));

            dateFrom = new DateTime(2015, 10, 19, 0, 0, 00);
            dateTo = new DateTime(2015, 10, 20, 0, 0, 00).AddSeconds(-1);
            Assert.AreEqual(23, DateUtilities.GetBusinessHours(dateFrom, dateTo));

            // Do not count weekends
            dateFrom = new DateTime(2015, 10, 23, 9, 37, 00);
            dateTo = new DateTime(2015, 10, 26, 9, 37, 00);
            Assert.AreEqual(24, DateUtilities.GetBusinessHours(dateFrom, dateTo));

            // Do not count holidays
            _calendarMock.Setup(holiday => holiday.Holidays).Returns(new List<ITimePeriod>() { new Day(2015, 10, 20) });
            dateFrom = new DateTime(2015, 10, 19, 9, 37, 00);
            dateTo = new DateTime(2015, 10, 21, 9, 37, 00);
            Assert.AreEqual(24, DateUtilities.GetBusinessHours(dateFrom, dateTo));

            // Use workweek from calendar
            _calendarMock.Setup(m => m.Workweek).Returns(new List<DayOfWeek> { DayOfWeek.Monday});
            dateFrom = new DateTime(2015, 10, 21, 9, 37, 0);
            dateTo = new DateTime(2015, 10, 28, 9, 37, 0);
            Assert.AreEqual(24, DateUtilities.GetBusinessHours(dateFrom, dateTo));
        }

        [TestMethod]
        public void GetBusinessDaysTillLateTest()
        {
            _dateTimeNowMock.Setup(m => m.Now)
                .Returns(new DateTime(2019, 1, 7, 8, 0, 0));

            var today = new DateTime(2019, 1, 7, 8, 0, 0);
            DateTime dateTo;

            dateTo = today.AddDays(1);
            Assert.IsTrue(DateUtilities.GetBusinessDaysTillLate(dateTo) >= 0);

            dateTo = today.AddDays(3);
            Assert.IsTrue(DateUtilities.GetBusinessDaysTillLate(dateTo) >= 1);

            dateTo = today;
            Assert.AreEqual(0, DateUtilities.GetBusinessDaysTillLate(dateTo));

            dateTo = today.AddDays(-1);
            Assert.AreEqual(-1, DateUtilities.GetBusinessDaysTillLate(dateTo));
        }

        [TestMethod]
        public void AddBusinessDaysTest()
        {
            var monday = new DateTime(2017, 7, 3);
            var tuesday = new DateTime(2017, 7, 4);
            var wednesday = new DateTime(2017, 7, 5);
            var friday = new DateTime(2017, 7, 7);
            var saturday = new DateTime(2017, 7, 8);
            var sunday = new DateTime(2017, 7, 9);

            // Normal case without holidays
            Assert.AreEqual(new DateTime(2017, 7, 4), monday.AddBusinessDays(1));
            Assert.AreEqual(new DateTime(2017, 7, 5), monday.AddBusinessDays(2));
            Assert.AreEqual(new DateTime(2017, 7, 6), monday.AddBusinessDays(3));
            Assert.AreEqual(new DateTime(2017, 7, 7), monday.AddBusinessDays(4));
            Assert.AreEqual(new DateTime(2017, 7, 3), tuesday.AddBusinessDays(-1));

            // Skip weekends
            Assert.AreEqual(new DateTime(2017, 7, 10), monday.AddBusinessDays(5));
            Assert.AreEqual(new DateTime(2017, 7, 11), monday.AddBusinessDays(6));
            Assert.AreEqual(new DateTime(2017, 6, 30), monday.AddBusinessDays(-1));
            Assert.AreEqual(new DateTime(2017, 7, 10), friday.AddBusinessDays(1));

            // Start date is on weekend
            Assert.AreEqual(new DateTime(2017, 7, 10), saturday.AddBusinessDays(1));
            Assert.AreEqual(new DateTime(2017, 7, 10), sunday.AddBusinessDays(1));
            Assert.AreEqual(new DateTime(2017, 7, 7), saturday.AddBusinessDays(-1));
            Assert.AreEqual(new DateTime(2017, 7, 7), sunday.AddBusinessDays(-1));

            // Skip holidays
            _calendarMock.Setup(holiday => holiday.Holidays).Returns(new List<ITimePeriod>() { new Day(tuesday) });
            Assert.AreEqual(new DateTime(2017, 7, 5), monday.AddBusinessDays(1));
            Assert.AreEqual(new DateTime(2017, 7, 3), wednesday.AddBusinessDays(-1));

            // Start date is on holiday
            Assert.AreEqual(new DateTime(2017, 7, 5), tuesday.AddBusinessDays(1));
            Assert.AreEqual(new DateTime(2017, 7, 3), tuesday.AddBusinessDays(-1));

            // Use workweek from calendar
            _calendarMock.Setup(m => m.Workweek).Returns(new List<DayOfWeek> { DayOfWeek.Monday});
            Assert.AreEqual(monday.AddDays(7), monday.AddBusinessDays(1));
        }

        [TestMethod]
        public void ToDifferenceShortHandTest()
        {
            // Negative
            Assert.AreEqual("0 M", DateUtilities.ToDifferenceShortHand(-1));

            // Minutes
            Assert.AreEqual("0 M", DateUtilities.ToDifferenceShortHand(0));
            Assert.AreEqual("5 M", DateUtilities.ToDifferenceShortHand(5));
            Assert.AreEqual("59 M", DateUtilities.ToDifferenceShortHand(59));

            // Hours
            Assert.AreEqual("1.0 H", DateUtilities.ToDifferenceShortHand(MINUTES_IN_HOUR));
            Assert.AreEqual("1.0 H", DateUtilities.ToDifferenceShortHand(61));
            Assert.AreEqual("24.0 H", DateUtilities.ToDifferenceShortHand(MINUTES_IN_DAY - 1));

            // Days
            Assert.AreEqual("1.0 D", DateUtilities.ToDifferenceShortHand(MINUTES_IN_DAY));
            Assert.AreEqual("2.0 D", DateUtilities.ToDifferenceShortHand((MINUTES_IN_DAY * 2) - 1));
            Assert.AreEqual("2.0 D", DateUtilities.ToDifferenceShortHand((MINUTES_IN_DAY * 2)));
        }

        [TestMethod]
        public void FromDifferenceShortHandTest()
        {
            Assert.AreEqual(0, DateUtilities.FromDifferenceShortHand(string.Empty));
            Assert.AreEqual(0, DateUtilities.FromDifferenceShortHand("."));

            // Negative
            Assert.AreEqual(0, DateUtilities.FromDifferenceShortHand("-1 M"));
            Assert.AreEqual(0, DateUtilities.FromDifferenceShortHand("-1 H"));
            Assert.AreEqual(0, DateUtilities.FromDifferenceShortHand("-1 D"));


            
            // Minutes
            Assert.AreEqual(0, DateUtilities.FromDifferenceShortHand("0 M"));
            Assert.AreEqual(1, DateUtilities.FromDifferenceShortHand("1 M"));
            Assert.AreEqual(2, DateUtilities.FromDifferenceShortHand("1.5 M"));
            Assert.AreEqual(59, DateUtilities.FromDifferenceShortHand("59 M"));
            Assert.AreEqual(MINUTES_IN_HOUR, DateUtilities.FromDifferenceShortHand("60 M"));
            Assert.AreEqual(MINUTES_IN_HOUR, DateUtilities.FromDifferenceShortHand("60.1 M"));

            // Hours
            Assert.AreEqual(MINUTES_IN_HOUR, DateUtilities.FromDifferenceShortHand("1 H"));
            Assert.AreEqual(MINUTES_IN_HOUR, DateUtilities.FromDifferenceShortHand("1. H"));
            Assert.AreEqual(MINUTES_IN_HOUR, DateUtilities.FromDifferenceShortHand("1.0 H"));
            Assert.AreEqual(MINUTES_IN_HOUR * 1.5, DateUtilities.FromDifferenceShortHand("1.5 H"));
            Assert.AreEqual(MINUTES_IN_HOUR * 2, DateUtilities.FromDifferenceShortHand("2.0 H"));

            // Days
            Assert.AreEqual(MINUTES_IN_DAY, DateUtilities.FromDifferenceShortHand("1 D"));
            Assert.AreEqual(MINUTES_IN_DAY, DateUtilities.FromDifferenceShortHand("1. D"));
            Assert.AreEqual(MINUTES_IN_DAY, DateUtilities.FromDifferenceShortHand("1.0 D"));
            Assert.AreEqual(MINUTES_IN_DAY * 1.5, DateUtilities.FromDifferenceShortHand("1.5 D"));
            Assert.AreEqual(MINUTES_IN_DAY * 2, DateUtilities.FromDifferenceShortHand("2.0 D"));
        }

        [TestMethod]
        public void MintesGroupedByDayTest()
        {
            DateTime startTime;
            DateTime endTime;
            IDictionary<DateTime, int> result;

            // One Hour
            startTime = new DateTime(2016, 1, 1, 12, 0, 0);
            endTime = new DateTime(2016, 1, 1, 13, 0, 0);
            result = DateUtilities.MinutesGroupedByDay(startTime, endTime);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(60, result[startTime.Date]);

            // One Day
            startTime = new DateTime(2016, 1, 1, 12, 0, 0);
            endTime = new DateTime(2016, 1, 2, 12, 0, 0);
            result = DateUtilities.MinutesGroupedByDay(startTime, endTime);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 2);
            Assert.AreEqual(12 * 60, result[startTime.Date]);
            Assert.AreEqual(12 * 60, result[endTime.Date]);

            // One Day, One Hour
            startTime = new DateTime(2016, 1, 1, 12, 0, 0);
            endTime = new DateTime(2016, 1, 2, 13, 0, 0);
            result = DateUtilities.MinutesGroupedByDay(startTime, endTime);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 2);
            Assert.AreEqual(12 * 60, result[startTime.Date]);
            Assert.AreEqual(13 * 60, result[endTime.Date]);

            startTime = new DateTime(2016, 1, 1, 12, 0, 0);
            endTime = new DateTime(2016, 1, 3, 13, 0, 0);
            result = DateUtilities.MinutesGroupedByDay(startTime, endTime);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 3);
            Assert.AreEqual(12 * 60, result[startTime.Date]);
            Assert.AreEqual(24 * 60, result[startTime.Date.AddDays(1)]);
            Assert.AreEqual(13 * 60, result[endTime.Date]);

            // Two Days, One Hour
            startTime = new DateTime(2016, 1, 1, 12, 0, 0);
            endTime = new DateTime(2016, 1, 4, 13, 0, 0);
            result = DateUtilities.MinutesGroupedByDay(startTime, endTime);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 4);
            Assert.AreEqual(12 * 60, result[startTime.Date]);
            Assert.AreEqual(24 * 60, result[startTime.Date.AddDays(1)]);
            Assert.AreEqual(24 * 60, result[startTime.Date.AddDays(2)]);
            Assert.AreEqual(13 * 60, result[endTime.Date]);


            // 15 Minutes, 1 Second
            startTime = new DateTime(2016, 1, 1, 12, 0, 0);
            endTime = new DateTime(2016, 1, 1, 12, 15, 1);
            result = DateUtilities.MinutesGroupedByDay(startTime, endTime);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(15, result[startTime.Date]);

            // 15 Minutes, 29 Seconds
            startTime = new DateTime(2016, 1, 1, 12, 0, 0);
            endTime = new DateTime(2016, 1, 1, 12, 15, 29);
            result = DateUtilities.MinutesGroupedByDay(startTime, endTime);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(15, result[startTime.Date]);

            // 15 Minutes, 30 Seconds
            startTime = new DateTime(2016, 1, 1, 12, 0, 0);
            endTime = new DateTime(2016, 1, 1, 12, 15, 30);
            result = DateUtilities.MinutesGroupedByDay(startTime, endTime);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(16, result[startTime.Date]);

            // 15 Minutes, 31 Seconds
            startTime = new DateTime(2016, 1, 1, 12, 0, 0);
            endTime = new DateTime(2016, 1, 1, 12, 15, 31);
            result = DateUtilities.MinutesGroupedByDay(startTime, endTime);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(16, result[startTime.Date]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MinutesGroupedByDayExceptionTest()
        {
            DateUtilities.MinutesGroupedByDay(DateTime.Now, DateTime.Now.AddSeconds(-1));
        }

        [TestMethod]
        public void MonthsInRangeTest()
        {
            // Case - 1 Month Difference
            var fromDate = new DateTime(2016, 12, 30);
            var toDate = fromDate.AddMonths(1);
            var actual = DateUtilities.MonthsInRange(fromDate, toDate).ToList();
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(new DateTime(2016, 12, 1), actual[0]);
            Assert.AreEqual(new DateTime(2017, 1, 1), actual[1]);

            // Case - 1 Year Difference
            fromDate = new DateTime(2016, 12, 30);
            toDate = fromDate.AddYears(1);
            actual = DateUtilities.MonthsInRange(fromDate, toDate).ToList();
            Assert.AreEqual(13, actual.Count);
            Assert.AreEqual(new DateTime(2016, 12, 1), actual[0]);
            Assert.AreEqual(new DateTime(2017, 1, 1), actual[1]);
            Assert.AreEqual(new DateTime(2017, 2, 1), actual[2]);
            Assert.AreEqual(new DateTime(2017, 3, 1), actual[3]);
            Assert.AreEqual(new DateTime(2017, 4, 1), actual[4]);
            Assert.AreEqual(new DateTime(2017, 5, 1), actual[5]);
            Assert.AreEqual(new DateTime(2017, 6, 1), actual[6]);
            Assert.AreEqual(new DateTime(2017, 7, 1), actual[7]);
            Assert.AreEqual(new DateTime(2017, 8, 1), actual[8]);
            Assert.AreEqual(new DateTime(2017, 9, 1), actual[9]);
            Assert.AreEqual(new DateTime(2017, 10, 1), actual[10]);
            Assert.AreEqual(new DateTime(2017, 11, 1), actual[11]);
            Assert.AreEqual(new DateTime(2017, 12, 1), actual[12]);

            // Case - 0 Month Difference
            fromDate = new DateTime(2016, 12, 1);
            toDate = new DateTime(2016, 12, 1);
            actual = DateUtilities.MonthsInRange(fromDate, toDate).ToList();
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(new DateTime(2016, 12, 1), actual[0]);

            // Case - Negative Difference
            fromDate = new DateTime(2016, 12, 1);
            toDate = fromDate.AddMonths(-1);
            actual = DateUtilities.MonthsInRange(fromDate, toDate).ToList();
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void GetFirstDayOfYearTest()
        {
            Assert.AreEqual(new DateTime(2016, 1, 1), DateUtilities.GetFirstDayOfYear(2016));
            Assert.AreEqual(new DateTime(2017, 1, 1), DateUtilities.GetFirstDayOfYear(2017));
            Assert.AreEqual(new DateTime(1, 1, 1), DateUtilities.GetFirstDayOfYear(1));
        }
    }
}
