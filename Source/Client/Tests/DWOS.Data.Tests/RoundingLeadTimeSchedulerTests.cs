using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Date;
using DWOS.Shared.Utilities;
using Itenso.TimePeriod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DWOS.Data.Tests
{
    [TestClass]
    public class RoundingLeadTimeSchedulerTests
    {
        private const int FirstProcessId = 10;
        private Mock<ICalendarPersistence> _calendarMock;
        private Mock<ILeadTimePersistence> _dataMock;
        private Mock<IDateTimeNowProvider> _timeMock;

        [TestInitialize]
        public void Initialize()
        {
            _calendarMock = new Mock<ICalendarPersistence>();
            _calendarMock.Setup(m => m.Workweek).Returns(new List<DayOfWeek>
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
            });
            _calendarMock.Setup(holiday => holiday.Holidays).Returns(new List<ITimePeriod>());

            DependencyContainer.Register(_calendarMock.Object);

            _dataMock = new Mock<ILeadTimePersistence>();
            _timeMock = new Mock<IDateTimeNowProvider>();
        }

        [TestMethod]
        public void ScheduleAfterRolloverTest()
        {
            // Setup
            var currentTime = new DateTime(2017, 7, 5, 12, 0, 0);

            _timeMock.Setup(p => p.Now).Returns(currentTime);
            _dataMock.Setup(d => d.ReceivingRolloverTime).Returns(new TimeSpan(11, 0, 0));

            var target = new RoundingLeadTimeScheduler(_timeMock.Object);
            target.LoadData(_dataMock.Object, currentTime);

            var dsOrders = GenerateData();
            var order = dsOrders.Order.First();

            // Test
            var estimatedShippingDate = target.UpdateScheduleDates(order, OrderProcessLeadTimes.Empty);

            // Assertions
            var expectedDate = currentTime.Date;
            Assert.AreEqual(expectedDate, estimatedShippingDate);
            Assert.AreEqual(expectedDate, order.GetOrderProcessesRows().First().EstEndDate);
        }

        [TestMethod]
        public void ScheduleBeforeRolloverTimeTest()
        {
            // Setup
            var currentTime = new DateTime(2017, 7, 5, 12, 0, 0);

            _timeMock.Setup(p => p.Now).Returns(currentTime);

            _dataMock.Setup(d => d.ReceivingRolloverTime).Returns(new TimeSpan(13, 0, 0));

            var target = new RoundingLeadTimeScheduler(_timeMock.Object);
            target.LoadData(_dataMock.Object, currentTime);

            var dsOrders = GenerateData();
            var order = dsOrders.Order.First();

            // Test
            var estimatedShippingDate = target.UpdateScheduleDates(order, OrderProcessLeadTimes.Empty);

            // Assertions
            var expectedDate = currentTime.Date.AddDays(-1);
            Assert.AreEqual(expectedDate, estimatedShippingDate);
            Assert.AreEqual(expectedDate, order.GetOrderProcessesRows().First().EstEndDate);
        }

        [TestMethod]
        public void ScheduleOnWeekendTest()
        {
            // Setup data so that the scheduler has to postpone the estimated
            // ship date by a few days because it falls on a weekend
            var currentTime = new DateTime(2017, 7, 7, 12, 0, 0);

            _timeMock.Setup(p => p.Now).Returns(currentTime);
            _dataMock.Setup(d => d.ReceivingRolloverTime).Returns(new TimeSpan(11, 0, 0));

            var target = new RoundingLeadTimeScheduler(_timeMock.Object);
            target.LoadData(_dataMock.Object, currentTime);

            var dsOrders = GenerateData();
            var order = dsOrders.Order.First();

            _dataMock.Setup(d => d.GetLeadTimeDays(FirstProcessId)).Returns(1);

            // Test
            var estimatedShippingDate = target.UpdateScheduleDates(order, OrderProcessLeadTimes.Empty);

            // Assertions
            var expectedDate = new DateTime(2017, 7, 10);
            Assert.AreEqual(expectedDate, estimatedShippingDate);
            Assert.AreEqual(expectedDate, order.GetOrderProcessesRows().First().EstEndDate);
        }

        [TestMethod]
        public void ScheduleDuringSaturday()
        {
            // Setup data so that the scheduler's current time is on a weekend
            var currentTime = new DateTime(2017, 7, 8, 12, 0, 0);

            _timeMock.Setup(p => p.Now).Returns(currentTime);
            _dataMock.Setup(d => d.ReceivingRolloverTime).Returns(new TimeSpan(11, 0, 0));

            var target = new RoundingLeadTimeScheduler(_timeMock.Object);
            target.LoadData(_dataMock.Object, currentTime);

            var dsOrders = GenerateData();
            var order = dsOrders.Order.First();

            // Test
            var estimatedShippingDate = target.UpdateScheduleDates(order, OrderProcessLeadTimes.Empty);

            // Assertions
            var expectedDate = new DateTime(2017, 7, 10);
            Assert.AreEqual(expectedDate, estimatedShippingDate);
            Assert.AreEqual(expectedDate, order.GetOrderProcessesRows().First().EstEndDate);
        }

        [TestMethod]
        public void ScheduleDuringSunday()
        {
            // Setup data so that the scheduler's current time is on a weekend
            var currentTime = new DateTime(2017, 7, 9, 12, 0, 0);

            _timeMock.Setup(p => p.Now).Returns(currentTime);
            _dataMock.Setup(d => d.ReceivingRolloverTime).Returns(new TimeSpan(11, 0, 0));

            var target = new RoundingLeadTimeScheduler(_timeMock.Object);
            target.LoadData(_dataMock.Object, currentTime);

            var dsOrders = GenerateData();
            var order = dsOrders.Order.First();

            // Test
            var estimatedShippingDate = target.UpdateScheduleDates(order, OrderProcessLeadTimes.Empty);

            // Assertions
            var expectedDate = new DateTime(2017, 7, 10);
            Assert.AreEqual(expectedDate, estimatedShippingDate);
            Assert.AreEqual(expectedDate, order.GetOrderProcessesRows().First().EstEndDate);
        }

        [TestMethod]
        public void ScheduleMultipleProcessesTest()
        {
            // Setup data so that it has three processes, each with a different
            // lead time
            var currentTime = new DateTime(2017, 7, 5, 12, 0, 0);

            _timeMock.Setup(p => p.Now).Returns(currentTime);
            _dataMock.Setup(d => d.ReceivingRolloverTime).Returns(new TimeSpan(11, 0, 0));

            var target = new RoundingLeadTimeScheduler(_timeMock.Object);
            target.LoadData(_dataMock.Object, currentTime);

            var dsOrders = GenerateData();
            var order = dsOrders.Order.First();

            var secondProcess = dsOrders.OrderProcesses.NewOrderProcessesRow();
            secondProcess.OrderRow = order;
            secondProcess.ProcessID = 20;
            secondProcess.StepOrder = 2;
            secondProcess.Department = "Masking";
            secondProcess.ProcessAliasID = 2;
            dsOrders.OrderProcesses.AddOrderProcessesRow(secondProcess);

            var thirdProcess = dsOrders.OrderProcesses.NewOrderProcessesRow();
            thirdProcess.OrderRow = order;
            thirdProcess.ProcessID = 30;
            thirdProcess.StepOrder = 3;
            thirdProcess.Department = "Paint";
            thirdProcess.ProcessAliasID = 3;
            dsOrders.OrderProcesses.AddOrderProcessesRow(thirdProcess);

            var fourthProcess = dsOrders.OrderProcesses.NewOrderProcessesRow();
            fourthProcess.OrderRow = order;
            fourthProcess.ProcessID = 40;
            fourthProcess.StepOrder = 4;
            fourthProcess.Department = "Chem";
            fourthProcess.ProcessAliasID = 4;
            dsOrders.OrderProcesses.AddOrderProcessesRow(fourthProcess);

            _dataMock.Setup(d => d.GetLeadTimeDays(FirstProcessId)).Returns(1);
            _dataMock.Setup(d => d.GetLeadTimeDays(secondProcess.ProcessID)).Returns(2);
            _dataMock.Setup(d => d.GetLeadTimeDays(thirdProcess.ProcessID)).Returns(5);
            _dataMock.Setup(d => d.GetLeadTimeDays(fourthProcess.ProcessID)).Returns(0);

            // Test
            var estimatedShippingDate = target.UpdateScheduleDates(order, OrderProcessLeadTimes.Empty);

            // Assertions
            var orderProcesses = order.GetOrderProcessesRows();
            Assert.AreEqual(new DateTime(2017, 7, 6), orderProcesses[0].EstEndDate);
            Assert.AreEqual(new DateTime(2017, 7, 10), orderProcesses[1].EstEndDate);
            Assert.AreEqual(new DateTime(2017, 7, 17), orderProcesses[2].EstEndDate);
            Assert.AreEqual(new DateTime(2017, 7, 17), orderProcesses[3].EstEndDate);

            Assert.AreEqual(new DateTime(2017, 7, 17), estimatedShippingDate);
        }

        [TestMethod]
        public void ScheduleMiscellaneousLeadTimesTest()
        {
            // Setup with non-process lead times.
            var currentTime = new DateTime(2017, 7, 5, 12, 0, 0);

            _timeMock.Setup(p => p.Now).Returns(currentTime);

            _dataMock.Setup(d => d.ReceivingRolloverTime).Returns(new TimeSpan(11, 0, 0));
            _dataMock.Setup(d => d.ShippingLeadTime).Returns(1);
            _dataMock.Setup(d => d.PartMarkingEnabled).Returns(true);
            _dataMock.Setup(d => d.PartMarkingLeadTime).Returns(2);
            _dataMock.Setup(d => d.CocEnabled).Returns(true);
            _dataMock.Setup(d => d.CocLeadTime).Returns(3);

            var target = new RoundingLeadTimeScheduler(_timeMock.Object);
            target.LoadData(_dataMock.Object, currentTime);

            var dsOrders = GenerateData();
            var order = dsOrders.Order.First();

            var partMarking = dsOrders.OrderPartMark.NewOrderPartMarkRow();
            partMarking.OrderRow = order;
            dsOrders.OrderPartMark.AddOrderPartMarkRow(partMarking);

            // Test
            var estimatedShippingDate = target.UpdateScheduleDates(order, OrderProcessLeadTimes.Empty);

            // Assertions
            Assert.AreEqual(new DateTime(2017, 7, 13), estimatedShippingDate);
            Assert.AreEqual(currentTime.Date, order.GetOrderProcessesRows().First().EstEndDate);
        }

        [TestMethod]
        public void ScheduleMiscellaneousLeadTimesWithoutPartMarkingTest()
        {
            // Setup with non-process lead times.
            var currentTime = new DateTime(2017, 7, 5, 12, 0, 0);

            _timeMock.Setup(p => p.Now).Returns(currentTime);

            _dataMock.Setup(d => d.ReceivingRolloverTime).Returns(new TimeSpan(11, 0, 0));
            _dataMock.Setup(d => d.ShippingLeadTime).Returns(1);
            _dataMock.Setup(d => d.PartMarkingEnabled).Returns(true);
            _dataMock.Setup(d => d.PartMarkingLeadTime).Returns(2);
            _dataMock.Setup(d => d.CocEnabled).Returns(true);
            _dataMock.Setup(d => d.CocLeadTime).Returns(3);

            var target = new RoundingLeadTimeScheduler(_timeMock.Object);
            target.LoadData(_dataMock.Object, currentTime);

            var dsOrders = GenerateData();
            var order = dsOrders.Order.First();

            // If the order has no part mark rows, Part Mark time should not be added.

            // Test
            var estimatedShippingDate = target.UpdateScheduleDates(order, OrderProcessLeadTimes.Empty);

            // Assertions
            Assert.AreEqual(new DateTime(2017, 7, 11), estimatedShippingDate);
            Assert.AreEqual(currentTime.Date, order.GetOrderProcessesRows().First().EstEndDate);
        }

        [TestMethod]
        public void ScheduleWithProcessLeadTimeTest()
        {
            // Setup
            var currentTime = new DateTime(2017, 7, 5, 12, 0, 0);

            _timeMock.Setup(p => p.Now).Returns(currentTime);
            _dataMock.Setup(d => d.ReceivingRolloverTime).Returns(new TimeSpan(11, 0, 0));

            var target = new RoundingLeadTimeScheduler(_timeMock.Object);
            target.LoadData(_dataMock.Object, currentTime);

            var dsOrders = GenerateData();
            var order = dsOrders.Order.First();

            var leadTimes = new OrderProcessLeadTimes();

            // Test #1 - Load-based lead time
            leadTimes.AddForTesting(
                order.GetOrderProcessesRows().First().OrderProcessesID,
                new ProcessLeadTime(24, LeadTimeType.Load));

            var estimatedShippingDate = target.UpdateScheduleDates(order, leadTimes);

            // Assertions
            var expectedShippingDate = new DateTime(2017, 7, 6, 0, 0, 0);
            var expectedProcessingDate = new DateTime(2017, 7, 6, 00, 0, 0);
            Assert.AreEqual(expectedShippingDate, estimatedShippingDate);
            Assert.AreEqual(expectedProcessingDate, order.GetOrderProcessesRows().First().EstEndDate);

            // Test #2 - Quantity-based lead time
            leadTimes = new OrderProcessLeadTimes();
            leadTimes.AddForTesting(
                order.GetOrderProcessesRows().First().OrderProcessesID,
                new ProcessLeadTime(1, LeadTimeType.Piece));

            order.PartQuantity = 25;

            estimatedShippingDate = target.UpdateScheduleDates(order, leadTimes);

            // Assertions
            expectedShippingDate = new DateTime(2017, 7, 7, 0, 0, 0);
            expectedProcessingDate = new DateTime(2017, 7, 7, 0, 0, 0);
            Assert.AreEqual(expectedShippingDate, estimatedShippingDate);
            Assert.AreEqual(expectedProcessingDate, order.GetOrderProcessesRows().First().EstEndDate);
        }

        private static OrdersDataSet GenerateData()
        {
            var dsOrders = new OrdersDataSet();

            var priority = dsOrders.d_Priority.Addd_PriorityRow("Normal", "Green");
            var orderStatus = dsOrders.d_OrderStatus.Addd_OrderStatusRow("Open");
            var workStatus = dsOrders.d_WorkStatus.Addd_WorkStatusRow("Pending Order Review");

            var customer = dsOrders.CustomerSummary.NewCustomerSummaryRow();
            customer.CustomerID = 1;
            customer.Name = "Test Customer";
            customer.LeadTime = 0;
            customer.OrderReview = true;
            customer.Active = true;
            customer.RequireCOCByDefault = false;

            dsOrders.CustomerSummary.AddCustomerSummaryRow(customer);

            var part = dsOrders.PartSummary.NewPartSummaryRow();
            part.CustomerID = 1;
            part.PartID = 1;
            part.Name = "Test Part";
            part.Active = true;
            part.PartMarking = false;
            part.RequireCocByDefault = false;
            dsOrders.PartSummary.AddPartSummaryRow(part);

            var order = dsOrders.Order.NewOrderRow();
            order.OrderType = 1;
            order.OriginalOrderType = 1;
            order.CustomerSummaryRow = customer;
            order.OrderDate = new DateTime(2017, 7, 5);
            order.d_OrderStatusRow = orderStatus;
            order.d_PriorityRow = priority;
            order.PartSummaryRow = part;
            order.PartQuantity = 1;
            order.d_WorkStatusRow = workStatus;
            order.CurrentLocation = "Sales";
            order.Hold = false;
            order.RequireCoc = false;
            dsOrders.Order.AddOrderRow(order);

            var firstProcess = dsOrders.OrderProcesses.NewOrderProcessesRow();
            firstProcess.OrderRow = order;
            firstProcess.ProcessID = FirstProcessId;
            firstProcess.StepOrder = 1;
            firstProcess.Department = "Chem";
            firstProcess.ProcessAliasID = 1;
            dsOrders.OrderProcesses.AddOrderProcessesRow(firstProcess);

            return dsOrders;
        }
    }
}
