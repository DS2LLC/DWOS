using System;
using System.Collections.Generic;
using DWOS.Data.Date;
using DWOS.Shared.Utilities;
using Itenso.TimePeriod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DWOS.Data.Tests.Date
{
    [TestClass]
    public class DaysLateCalculatorTests
    {
        private static readonly DateTime Monday = new DateTime(2017, 4, 3);
        private Mock<ICalendarPersistence> _calendarMock;
        private DaysLateCalculator _target;
        private Mock<IDateTimeNowProvider> _datetimeMock;

        [TestInitialize]
        public void Initialize()
        {
            _calendarMock = new Mock<ICalendarPersistence>();
            _calendarMock.Setup(holiday => holiday.Holidays).Returns(new List<ITimePeriod>());
            _calendarMock.Setup(m => m.Workweek).Returns(new List<DayOfWeek>
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
            });

            DependencyContainer.Register(_calendarMock.Object);

            _datetimeMock = new Mock<IDateTimeNowProvider>();
            _target = new DaysLateCalculator(_datetimeMock.Object);
        }

        [TestMethod]
        public void GetDaysLateTest()
        {
            var tuesday = Monday.AddDays(1).Date;
            var wednesday = Monday.AddDays(2).Date;
            var thursday = Monday.AddDays(3).Date;
            var friday = Monday.AddDays(4).Date;
            var lastFriday = Monday.AddDays(-3);

            _datetimeMock.Setup(dt => dt.Now).Returns(Monday);

            Assert.AreEqual(0, _target.GetDaysLate(Monday));
            Assert.AreEqual(1, _target.GetDaysLate(tuesday));
            Assert.AreEqual(2, _target.GetDaysLate(wednesday));
            Assert.AreEqual(3, _target.GetDaysLate(thursday));
            Assert.AreEqual(99, _target.GetDaysLate(friday));
            Assert.AreEqual(-1, _target.GetDaysLate(lastFriday));
        }

        [TestMethod]
        public void GetDaysLateSkipWeekendsTest()
        {
            var friday = Monday.AddDays(4).Date;
            var saturday = Monday.AddDays(5).Date;
            var sunday = Monday.AddDays(6).Date;
            var nextMonday = Monday.AddDays(7).Date;

            _datetimeMock.Setup(dt => dt.Now).Returns(friday);

            Assert.AreEqual(99, _target.GetDaysLate(saturday));
            Assert.AreEqual(99, _target.GetDaysLate(sunday));
            Assert.AreEqual(1, _target.GetDaysLate(nextMonday));
        }

        [TestMethod]
        public void GetDaysLateAutoInitializeTest()
        {
            var tuesday = Monday.AddDays(1).Date;
            var wednesday = Monday.AddDays(2).Date;

            // Monday
            _datetimeMock.Setup(dt => dt.Now).Returns(Monday);
            Assert.AreEqual(1, _target.GetDaysLate(tuesday));
            Assert.AreEqual(2, _target.GetDaysLate(wednesday));

            // Tuesday
            _datetimeMock.Setup(dt => dt.Now).Returns(tuesday);
            Assert.AreEqual(0, _target.GetDaysLate(tuesday));
            Assert.AreEqual(1, _target.GetDaysLate(wednesday));
        }
    }
}
