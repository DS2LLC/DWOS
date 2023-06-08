using System;
using System.Linq;
using System.Threading;
using DWOS.UI.Admin.Schedule.Manual;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.UI.Tests.Admin.Schedule.Manual
{
    [TestClass]
    public class DepartmentDataContextTests
    {
        [TestMethod]
        public void RefreshFieldsTest()
        {
            var eventWaiter = new ManualResetEvent(false);
            var target = new DepartmentDataContext(Helpers.DepartmentQuality);
            target.FieldsRefreshed += (sender, args) =>
            {
                eventWaiter.Set();
            };

            target.RefreshFields();
            Assert.IsTrue(eventWaiter.WaitOne(TimeSpan.FromSeconds(1)), "Field refresh event did not fire");
        }

        [TestMethod]
        public void LoadTest()
        {
            var wipData = Helpers.GenerateTestData();
            var dsOrderStatus = wipData.OrderDataSet;
            var target = new DepartmentDataContext(Helpers.DepartmentChem);

            var eventWaiter = new ManualResetEvent(false);
            target.DataRefreshed += (sender, args) =>
            {
                eventWaiter.Set();
            };

            // First load
            var orders = dsOrderStatus.OrderStatus.Where(
                                          o => (o.WorkStatus == Helpers.ChangingDepartment && o.NextDept == Helpers.DepartmentChem) ||
                                               (o.WorkStatus == Helpers.InProcess && o.CurrentLocation == Helpers.DepartmentChem))
                                      .Select(ScheduleData.From);

            var batches = dsOrderStatus.BatchStatus.Where(
                                          o => (o.WorkStatus == Helpers.ChangingDepartment && o.NextDept == Helpers.DepartmentChem) ||
                                               (o.WorkStatus == Helpers.InProcess && o.CurrentLocation == Helpers.DepartmentChem))
                                      .Select(ScheduleData.From);

            var scheduleItems = orders.Concat(batches).ToList();

            target.Load(scheduleItems);

            Assert.IsTrue(eventWaiter.WaitOne(TimeSpan.FromSeconds(1)), "Data refresh event did not fire");

            Assert.IsNotNull(target.Unscheduled);
            Assert.AreEqual(2, target.Unscheduled.Count);

            Assert.IsNotNull(target.Scheduled);
            Assert.AreEqual(2, target.Scheduled.Count);

            // Ensure that every other load keeps selection status
            target.UnscheduledSelection = new object[] { target.Unscheduled.First() };
            target.ScheduledSelection = new object[] { target.Scheduled.First() };

            var unscheduledSelectionOrderId = target.UnscheduledSelection.OfType<DepartmentDataContext.ScheduleData>().First().Id;
            var scheduledSelectionOrderId = target.ScheduledSelection.OfType<DepartmentDataContext.ScheduleData>().First().Id;

            target.Load(scheduleItems);

            Assert.IsNotNull(target.UnscheduledSelection);
            Assert.IsNotNull(target.UnscheduledSelection.FirstOrDefault());
            Assert.AreEqual(unscheduledSelectionOrderId,
                target.UnscheduledSelection.OfType<DepartmentDataContext.ScheduleData>().First().Id);

            Assert.IsNotNull(target.ScheduledSelection);
            Assert.IsNotNull(target.ScheduledSelection.FirstOrDefault());
            Assert.AreEqual(scheduledSelectionOrderId,
                target.ScheduledSelection.OfType<DepartmentDataContext.ScheduleData>().First().Id);
        }

        [TestMethod]
        public void AddSelectedToScheduleTest()
        {
            // Setup
            var wipData = Helpers.GenerateTestData();
            var dsOrderStatus = wipData.OrderDataSet;
            var target = new DepartmentDataContext(Helpers.DepartmentChem);

            target.Load(dsOrderStatus.OrderStatus.Where(
                    o => (o.WorkStatus == Helpers.ChangingDepartment && o.NextDept == Helpers.DepartmentChem) ||
                         (o.WorkStatus == Helpers.InProcess && o.CurrentLocation == Helpers.DepartmentChem))
                .Select(ScheduleData.From));

            Assert.IsFalse(target.AddSelectedToSchedule.CanExecute(null));

            var eventWaiter = new ManualResetEvent(false);
            target.ScheduleOrderChanged += (sender, args) =>
            {
                eventWaiter.Set();
            };

            target.UnscheduledSelection = new object[] { target.Unscheduled.First() };

            // Test
            Assert.IsTrue(target.AddSelectedToSchedule.CanExecute(null));
            target.AddSelectedToSchedule.Execute(null);

            Assert.IsNull(target.UnscheduledSelection);
            Assert.AreEqual(0, target.Unscheduled.Count);
            Assert.AreEqual(2, target.Scheduled.Count);
            Assert.IsTrue(target.Scheduled.All(o => o.SchedulePriority > 0));
            Assert.IsTrue(eventWaiter.WaitOne(TimeSpan.FromSeconds(1)), "Schedule change event did not fire");
        }

        [TestMethod]
        public void RemoveSelectedFromScheduleTest()
        {
            // Setup
            var wipData = Helpers.GenerateTestData();
            var dsOrderStatus = wipData.OrderDataSet;
            var target = new DepartmentDataContext(Helpers.DepartmentChem);

            target.Load(dsOrderStatus.OrderStatus.Where(
                    o => (o.WorkStatus == Helpers.ChangingDepartment && o.NextDept == Helpers.DepartmentChem) ||
                         (o.WorkStatus == Helpers.InProcess && o.CurrentLocation == Helpers.DepartmentChem))
                .Select(ScheduleData.From));

            Assert.IsFalse(target.RemoveSelectedFromSchedule.CanExecute(null));

            var eventWaiter = new ManualResetEvent(false);
            target.ScheduleOrderChanged += (sender, args) =>
            {
                eventWaiter.Set();
            };

            target.ScheduledSelection = new object[] { target.Scheduled.First() };

            // Test
            Assert.IsTrue(target.RemoveSelectedFromSchedule.CanExecute(null));
            target.RemoveSelectedFromSchedule.Execute(null);

            Assert.IsNull(target.ScheduledSelection);
            Assert.AreEqual(0, target.Scheduled.Count);
            Assert.AreEqual(2, target.Unscheduled.Count);
            Assert.IsTrue(target.Unscheduled.All(o => o.SchedulePriority == 0));
            Assert.IsTrue(eventWaiter.WaitOne(TimeSpan.FromSeconds(1)), "Schedule change event did not fire");
        }

        [TestMethod]
        public void MoveSelectedUpTest()
        {
            // Setup
            var wipData = Helpers.GenerateTestData();
            var dsOrderStatus = wipData.OrderDataSet;
            var target = new DepartmentDataContext(Helpers.DepartmentChem);

            target.Load(dsOrderStatus.OrderStatus.Where(
                    o => (o.WorkStatus == Helpers.ChangingDepartment && o.NextDept == Helpers.DepartmentChem) ||
                         (o.WorkStatus == Helpers.InProcess && o.CurrentLocation == Helpers.DepartmentChem))
                .Select(ScheduleData.From));

            target.UnscheduledSelection = new object[] { target.Unscheduled.First() };
            target.AddSelectedToSchedule.Execute(null);

            Assert.AreEqual(2, target.Scheduled.Count);

            var eventWaiter = new ManualResetEvent(false);
            target.ScheduleOrderChanged += (sender, args) =>
            {
                eventWaiter.Set();
            };

            Assert.IsFalse(target.MoveSelectedUp.CanExecute(null));

            target.ScheduledSelection = new object[] { target.Scheduled[1] };

            var orderedSchedule = target.Scheduled.OrderBy(o => o.SchedulePriority).ToList();
            var expectedFirstOrderId = orderedSchedule[1].Id;
            var expectedSecondOrderId = orderedSchedule[0].Id;

            // Test
            Assert.IsTrue(target.MoveSelectedUp.CanExecute(null));
            target.MoveSelectedUp.Execute(null);

            var orderedScheduleAfterChange = target.Scheduled.OrderBy(o => o.SchedulePriority).ToList();
            Assert.AreEqual(expectedFirstOrderId, orderedScheduleAfterChange[0].Id);
            Assert.AreEqual(expectedSecondOrderId, orderedScheduleAfterChange[1].Id);

            Assert.IsTrue(eventWaiter.WaitOne(TimeSpan.FromSeconds(1)), "Schedule change event did not fire");
        }

        [TestMethod]
        public void MoveSelectedDownTest()
        {
            // Setup
            var wipData = Helpers.GenerateTestData();
            var dsOrderStatus = wipData.OrderDataSet;
            var target = new DepartmentDataContext(Helpers.DepartmentChem);

            target.Load(dsOrderStatus.OrderStatus.Where(
                    o => (o.WorkStatus == Helpers.ChangingDepartment && o.NextDept == Helpers.DepartmentChem) ||
                         (o.WorkStatus == Helpers.InProcess && o.CurrentLocation == Helpers.DepartmentChem))
                .Select(ScheduleData.From));

            target.UnscheduledSelection = new object[] { target.Unscheduled.First() };
            target.AddSelectedToSchedule.Execute(null);

            Assert.AreEqual(2, target.Scheduled.Count);

            var eventWaiter = new ManualResetEvent(false);
            target.ScheduleOrderChanged += (sender, args) =>
            {
                eventWaiter.Set();
            };

            Assert.IsFalse(target.MoveSelectedDown.CanExecute(null));

            target.ScheduledSelection = new object[] { target.Scheduled[0] };

            var orderedSchedule = target.Scheduled.OrderBy(o => o.SchedulePriority).ToList();
            var expectedFirstOrderId = orderedSchedule[1].Id;
            var expectedSecondOrderId = orderedSchedule[0].Id;

            // Test
            Assert.IsTrue(target.MoveSelectedDown.CanExecute(null));
            target.MoveSelectedDown.Execute(null);

            var orderedScheduleAfterChange = target.Scheduled.OrderBy(o => o.SchedulePriority).ToList();
            Assert.AreEqual(expectedFirstOrderId, orderedScheduleAfterChange[0].Id);
            Assert.AreEqual(expectedSecondOrderId, orderedScheduleAfterChange[1].Id);

            Assert.IsTrue(eventWaiter.WaitOne(TimeSpan.FromSeconds(1)), "Schedule change event did not fire");
        }
    }
}
