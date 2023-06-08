using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Date;
using DWOS.Shared.Settings;
using DWOS.Shared.Utilities;
using DWOS.UI.Utilities.Convertors;
using Itenso.TimePeriod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DWOS.UI.Tests.Utilities.Convertors
{
    [TestClass]
    public class Late3ColorConverterTests
    {
        private static readonly DateTime Now = new DateTime(2017, 7, 17, 12, 10, 0);
        private Mock<IDateTimeNowProvider> _dateProviderMock;
        private Mock<ISettingsPersistence> _settingsPersistenceMock;
        private Mock<IDwosApplicationSettingsProvider> _settingsProviderMock;
        private ApplicationSettings _appSettings;

        [TestInitialize]
        public void Initialize()
        {
            Late3ColorConverter.Initialize();
            _dateProviderMock = new Mock<IDateTimeNowProvider>();
            _dateProviderMock.Setup(m => m.Now).Returns(Now);

            // ICalendarPersistence is used by date utilities.
            var calendarMock = new Mock<ICalendarPersistence>();
            calendarMock.Setup(m => m.Holidays).Returns(new List<ITimePeriod>());
            calendarMock.Setup(m => m.Workweek).Returns(new List<DayOfWeek>
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
            });
            DependencyContainer.Register(calendarMock.Object);

            // Setup settings
            _settingsPersistenceMock = new Mock<ISettingsPersistence>();

            var pathProviderMock = new Mock<IPathProvider>();
            _appSettings = new ApplicationSettings(_settingsPersistenceMock.Object, pathProviderMock.Object);
            _settingsProviderMock = new Mock<IDwosApplicationSettingsProvider>();
            _settingsProviderMock.Setup(m => m.Settings)
                .Returns(_appSettings);

            // Default settings
            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "IncludeHoldsInLateOrders")))
                .Callback((SettingValue v) => v.Value = true.ToString());
        }

        [TestMethod]
        public void ConvertEarlyTest()
        {
            var value = Create(Now.AddDays(3).Date);
            var culture = CultureInfo.InvariantCulture;

            var target = new Late3ColorConverter(
                _dateProviderMock.Object,
                _settingsProviderMock.Object);

            var actual = target.Convert(value, typeof(SolidColorBrush), null, culture) as SolidColorBrush;
            Assert.IsNull(actual);
        }


        [TestMethod]
        public void ConvertTwoDaysUntilLateTest()
        {
            var value = Create(Now.AddDays(2).Date);
            var culture = CultureInfo.InvariantCulture;

            var target = new Late3ColorConverter(
                _dateProviderMock.Object,
                _settingsProviderMock.Object);

            var actual = target.Convert(value, typeof(SolidColorBrush), null, culture) as SolidColorBrush;
            Assert.IsNotNull(actual);
            Assert.AreEqual(Colors.Blue, actual.Color);
        }

        [TestMethod]
        public void ConvertOneDaysUntilLateTest()
        {
            var value = Create(Now.AddDays(1).Date);
            var culture = CultureInfo.InvariantCulture;

            var target = new Late3ColorConverter(
                _dateProviderMock.Object,
                _settingsProviderMock.Object);

            var actual = target.Convert(value, typeof(SolidColorBrush), null, culture) as SolidColorBrush;
            Assert.IsNotNull(actual);
            Assert.AreEqual(Colors.Yellow, actual.Color);
        }

        [TestMethod]
        public void ConvertZeroDaysUntilLateTest()
        {
            var value = Create(Now.Date);
            var culture = CultureInfo.InvariantCulture;

            var target = new Late3ColorConverter(
                _dateProviderMock.Object,
                _settingsProviderMock.Object);

            var actual = target.Convert(value, typeof(SolidColorBrush), null, culture) as SolidColorBrush;
            Assert.IsNotNull(actual);
            Assert.AreEqual(Colors.Orange, actual.Color);
        }

        [TestMethod]
        public void ConvertLateTest()
        {
            var value = Create(Now.AddDays(-1).Date);
            var culture = CultureInfo.InvariantCulture;

            var target = new Late3ColorConverter(
                _dateProviderMock.Object,
                _settingsProviderMock.Object);

            var actual = target.Convert(value, typeof(SolidColorBrush), null, culture) as SolidColorBrush;
            Assert.IsNotNull(actual);
            Assert.AreEqual(Colors.Red, actual.Color);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBackTest()
        {
            var target = new Late3ColorConverter(
                _dateProviderMock.Object,
                _settingsProviderMock.Object);

            target.ConvertBack(null, null, null, null);
        }

        [TestMethod]
        public void ConvertLateHoldTest()
        {
            // Case - 'Include' setting is enabled
            var value = Create(Now.AddDays(-1).Date, true);
            var culture = CultureInfo.InvariantCulture;

            var target = new Late3ColorConverter(
                _dateProviderMock.Object,
                _settingsProviderMock.Object);

            var actual = target.Convert(value, typeof(SolidColorBrush), null, culture) as SolidColorBrush;
            Assert.IsNotNull(actual);
            Assert.AreEqual(Colors.Red, actual.Color);

            // Case - 'Include' setting is disabled
            _appSettings.ReloadSettings();
            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "IncludeHoldsInLateOrders")))
                .Callback((SettingValue v) => v.Value = false.ToString());

            actual = target.Convert(value, typeof(SolidColorBrush), null, culture) as SolidColorBrush;
            Assert.IsNull(actual);
        }

        private static OrderStatusData Create(DateTime estShipDate, bool hold = false)
        {
            // Initialize row
            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            row.WO = 1000;
            row.OrderType = (int)OrderType.Normal;
            row.ActiveTimerCount = 0;
            row.SetCurrentLineNull();
            row.CurrentLocation = "In Process";
            row.CurrentProcess = "Test";
            row.Customer = "Test Customer";
            row.CustomerWO = null;
            row.HasPartMark = false;
            row.Hold = hold;
            row.InBatch = false;
            row.NextDept = "Final Inspection";
            row.PO = null;
            row.Part = "Test Part";
            row.PartQuantity = 10;
            row.PartProcessingCount = 10;
            row.Priority = "Normal";
            row.RemainingTime = null;
            row.SchedulePriority = 0;
            row.WorkStatus = "In Process";
            row.SurfaceArea = 5.0D;
            row.SetSalesOrderIDNull();
            row.WorkStatusDuration = 0;

            // Set dates
            row.EstShipDate = estShipDate;
            row.RequiredDate = estShipDate.AddDays(1);

            // Construct model
            return OrderStatusData.CreateFrom(row);
        }
    }
}
