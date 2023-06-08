using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using DWOS.Data.Datasets;
using DWOS.Data.Date;
using Itenso.TimePeriod;
using Moq;
using DWOS.Shared.Utilities;

namespace DWOS.UI.Tests
{
    [TestClass]
    public class ProcessByDateBadgeConverterTests
    {
        private static readonly DateTime CurrentTime = new DateTime(2017, 7, 18, 3, 17, 0);
        private Mock<IDateTimeNowProvider> _dateNowMock;

        [TestInitialize]
        public void Initialize()
        {
            ProcessByDateBadgeConverter.Initialize();

            // ICalendarPersistence is used by date utilities.
            var calendarMock = new Mock<ICalendarPersistence>();
            calendarMock.Setup(m => m.Holidays).Returns(new List<ITimePeriod>());
            calendarMock.Setup(m => m.Workweek).Returns(new List<DayOfWeek>
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
            });

            DependencyContainer.Register(calendarMock.Object);

            _dateNowMock = new Mock<IDateTimeNowProvider>();
            _dateNowMock.Setup(d => d.Now).Returns(CurrentTime);
        }

        [TestMethod]
        public void ConvertDueTodayTest()
        {
            var target = new ProcessByDateBadgeConverter(_dateNowMock.Object);
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var badge = target.Convert(view, typeof(Rectangle), null, culture) as Rectangle;
            Assert.IsNotNull(badge);

            Assert.AreEqual(Colors.Orange, (badge.Fill as SolidColorBrush)?.Color);
        }

        [TestMethod]
        public void ConvertLateTest()
        {
            var target = new ProcessByDateBadgeConverter(_dateNowMock.Object);
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            row.CurrentProcessDue = CurrentTime.AddDays(-1);
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var badge = target.Convert(view, typeof(Rectangle), null, culture) as Rectangle;
            Assert.IsNotNull(badge);

            Assert.AreEqual(Colors.Red, (badge.Fill as SolidColorBrush)?.Color);
        }

        [TestMethod]
        public void ConvertDueTomorrowTest()
        {
            var target = new ProcessByDateBadgeConverter(_dateNowMock.Object);
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            row.CurrentProcessDue = CurrentTime.AddDays(1);
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var badge = target.Convert(view, typeof(Rectangle), null, culture) as Rectangle;
            Assert.IsNotNull(badge);

            Assert.AreEqual(Colors.Yellow, (badge.Fill as SolidColorBrush)?.Color);
        }

        [TestMethod]
        public void ConvertDueAfterTomorrowTest()
        {
            var target = new ProcessByDateBadgeConverter(_dateNowMock.Object);
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            row.CurrentProcessDue = CurrentTime.AddDays(2);
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var badge = target.Convert(view, typeof(Rectangle), null, culture) as Rectangle;
            Assert.IsNotNull(badge);

            Assert.AreEqual(Colors.Transparent, (badge.Fill as SolidColorBrush)?.Color);
        }

        private static void InitializeRowData(OrderStatusDataSet.OrderStatusRow row)
        {
            row.WO = 1000;
            row.OrderType = (int) OrderType.Normal;
            row.ActiveTimerCount = 0;
            row.SetCurrentLineNull();
            row.CurrentLocation = "In Process";
            row.CurrentProcess = "Test";
            row.CurrentProcessDue = CurrentTime;
            row.Customer = "Test Customer";
            row.CustomerWO = null;
            row.EstShipDate = CurrentTime;
            row.HasPartMark = false;
            row.Hold = false;
            row.InBatch = false;
            row.NextDept = "Final Inspection";
            row.PO = null;
            row.Part = "Test Part";
            row.PartQuantity = 10;
            row.SetPartProcessingCountNull();
            row.Priority = "Normal";
            row.RemainingTime = null;
            row.RequiredDate = CurrentTime;
            row.SchedulePriority = 0;
            row.WorkStatus = "In Process";
            row.SurfaceArea = 5.0D;
            row.SetSalesOrderIDNull();
            row.WorkStatusDuration = 0;
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBackTest()
        {
            var target = new ProcessByDateBadgeConverter(_dateNowMock.Object);
            target.ConvertBack(null, null, null, null);
        }
    }
}
