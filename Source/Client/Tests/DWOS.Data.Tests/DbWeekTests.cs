using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWOS.Data.Tests
{
    [TestClass]
    public class DbWeekTests
    {
        [TestMethod]
        public void WeeksInRangeTest()
        {
            // Case - YTD
            var from = new DateTime(2018, 1, 1);
            var to = new DateTime(2018, 8, 21);

            var actual = DbWeek.WeeksInRange(from, to);
            Assert.AreEqual(34, actual.Count);
            CollectionAssert.Contains(actual, new DbWeek(2018, 1));
            CollectionAssert.Contains(actual, new DbWeek(2018, 34));
            CollectionAssert.DoesNotContain(actual, new DbWeek(2017, 53));
            CollectionAssert.DoesNotContain(actual, new DbWeek(2017, 52));

            // Case - Include last day of 2017
            // (It's in the same week as January 1, 2018.)
            from = new DateTime(2017, 12, 31);
            to = new DateTime(2018, 8, 21);

            actual = DbWeek.WeeksInRange(from, to);
            Assert.AreEqual(35, actual.Count);
            CollectionAssert.Contains(actual, new DbWeek(2018, 1));
            CollectionAssert.Contains(actual, new DbWeek(2018, 34));
            CollectionAssert.Contains(actual, new DbWeek(2017, 53));
            CollectionAssert.DoesNotContain(actual, new DbWeek(2017, 52));

            // Case - Include last full week of 2017
            from = new DateTime(2017, 12, 24);
            to = new DateTime(2018, 8, 21);

            actual = DbWeek.WeeksInRange(from, to);
            Assert.AreEqual(36, actual.Count);
            CollectionAssert.Contains(actual, new DbWeek(2018, 1));
            CollectionAssert.Contains(actual, new DbWeek(2018, 34));
            CollectionAssert.Contains(actual, new DbWeek(2017, 53));
            CollectionAssert.Contains(actual, new DbWeek(2017, 52));
        }

        [TestMethod]
        public void DisplayStringTest()
        {
            var calendar = new GregorianCalendar();
            var from = new DateTime(2017, 12, 24);
            var to = new DateTime(2018, 8, 21);

            var weeks = DbWeek.WeeksInRange(from, to);
            foreach (var week in weeks)
            {
                var sunday = calendar.AddWeeks(DateUtilities.GetFirstDayOfYear(week.Year).StartOfWeek(DbWeek.StartOfWeek), week.WeekInYear - 1);
                var monday = sunday.AddDays((int)DayOfWeek.Monday);
                Assert.AreEqual(monday.ToShortDateString(), week.ToDisplayString());
            }
        }
    }
}
