using System;
using System.Linq;
using System.Threading;
using DWOS.Data;
using DWOS.Shared.Settings;
using DWOS.UI.Admin.Schedule.Manual;
using DWOS.UI.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DWOS.UI.Tests.Admin.Schedule.Manual
{
    [TestClass]
    public class SchedulingTabDataContextTests
    {
        private SchedulingTabDataContext _target;
        private Mock<ISchedulingPersistence> _persistenceMock;
        private Mock<ISecurityManager> _securityMock;
        private Mock<ISettingsPersistence> _settingsPersistenceMock;

        [TestInitialize]
        public void Initialize()
        {
            _settingsPersistenceMock = new Mock<ISettingsPersistence>();
            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "WorkStatusInProcess")))
                .Callback((SettingValue v) => v.Value = Helpers.InProcess);

            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "WorkStatusChangingDepartment")))
                .Callback((SettingValue v) => v.Value = Helpers.ChangingDepartment);

            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "SchedulingEnabled")))
                .Callback((SettingValue v) => v.Value = true.ToString());

            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "SchedulerType")))
                .Callback((SettingValue v) => v.Value = "Manual");

            _securityMock = new Mock<ISecurityManager>();

            var pathProviderMock = new Mock<IPathProvider>();
            var settings = new ApplicationSettings(_settingsPersistenceMock.Object, pathProviderMock.Object);

            var settingsProviderMock = new Mock<IDwosApplicationSettingsProvider>();
            settingsProviderMock.Setup(m => m.Settings).Returns(settings);

            _persistenceMock = new Mock<ISchedulingPersistence>();
            _persistenceMock.Setup(m => m.SecurityManager).Returns(_securityMock.Object);
            _target = new SchedulingTabDataContext(_persistenceMock.Object, settingsProviderMock.Object);
        }

        [TestMethod]
        public void InitializeManualTest()
        {
            var dsOrderStatus = Helpers.GenerateTestData();
            _target.Initialize(dsOrderStatus);
            Assert.IsNotNull(_target.Departments);
            Assert.AreEqual(2, _target.Departments.Count);

            var chemDepartment = _target.Departments.FirstOrDefault(d => d.Name == Helpers.DepartmentChem);
            Assert.IsNotNull(chemDepartment);
            Assert.AreEqual(2, chemDepartment.Unscheduled.Count);
            Assert.AreEqual(2, chemDepartment.Scheduled.Count);

            var qaDepartment = _target.Departments.FirstOrDefault(d => d.Name == Helpers.DepartmentQuality);
            Assert.IsNotNull(qaDepartment);
            Assert.AreEqual(2, qaDepartment.Unscheduled.Count);
            Assert.AreEqual(0, qaDepartment.Scheduled.Count);
        }

        [TestMethod]
        public void UpdateDataManualTest()
        {
            var wipData = Helpers.GenerateTestData();
            _target.Initialize(wipData);

            // Change Order 1000 to be in Final Inspection & out of unscheduled orders
            var orderToChange = wipData.OrderDataSet.OrderStatus.FindByWO(1000);
            orderToChange.CurrentLocation = Helpers.DepartmentQuality;
            orderToChange.WorkStatus = Helpers.FinalInspection;

            _target.UpdateData(new WipData(wipData.OrderDataSet));

            var chemDepartment = _target.Departments.FirstOrDefault(d => d.Name == Helpers.DepartmentChem);
            Assert.IsNotNull(chemDepartment);
            Assert.AreEqual(1, chemDepartment.Unscheduled.Count);
            Assert.AreEqual(2, chemDepartment.Scheduled.Count);
        }

        [TestMethod]
        public void IsEnabledTest()
        {
            // Scheduling enabled, using manual scheduling, but user is not in role
            Assert.IsFalse(_target.IsEnabled);

            _securityMock.Setup(m => m.IsInRole("OrderSchedule")).Returns(true);
            Assert.IsTrue(_target.IsEnabled);
        }

        [TestMethod]
        public void DoSaveManualTest()
        {
            var eventWaiter = new ManualResetEvent(false);

            var dsOrderStatus = Helpers.GenerateTestData();
            _target.Initialize(dsOrderStatus);
            _target.OnLoaded(); // Register necessary event handlers

            var chemDepartment = _target.Departments.FirstOrDefault(d => d.Name == Helpers.DepartmentChem);
            Assert.IsNotNull(chemDepartment);
            Assert.AreEqual(2, chemDepartment.Unscheduled.Count);
            Assert.AreEqual(2, chemDepartment.Scheduled.Count);
            chemDepartment.ScheduleOrderChanged += (sender, args) => eventWaiter.Set();

            // No changes
            Assert.IsFalse(_target.HasChanges);
            Assert.IsFalse(_target.Save.CanExecute(null));

            // Schedule an order
            chemDepartment.UnscheduledSelection = new object[] { chemDepartment.Unscheduled.First() };
            Assert.IsTrue(chemDepartment.AddSelectedToSchedule.CanExecute(null));
            chemDepartment.AddSelectedToSchedule.Execute(null);

            Assert.IsTrue(eventWaiter.WaitOne(TimeSpan.FromSeconds(1)), "Event did not fire");

            Assert.AreEqual(1, chemDepartment.Unscheduled.Count);
            Assert.AreEqual(3, chemDepartment.Scheduled.Count);

            Assert.IsTrue(_target.HasChanges);
            Assert.IsTrue(_target.Save.CanExecute(null));
            _target.Save.Execute(null);

            _persistenceMock.Verify(m => m.SaveChanges(_target), Times.Once);
        }

        [TestMethod]
        public void ChangeUserHandlerTest()
        {
            var eventWaiter = new ManualResetEvent(false);

            var dsOrderStatus = Helpers.GenerateTestData();
            _target.Initialize(dsOrderStatus);
            _target.OnLoaded(); // Register necessary event handlers

            var enabledChanged = false;
            _target.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(SchedulingTabDataContext.IsEnabled))
                {
                    enabledChanged = true;
                    eventWaiter.Set();
                }
            };

            _securityMock.Raise(m => m.UserUpdated += null, new UserUpdatedEventArgs(true));

            Assert.IsTrue(eventWaiter.WaitOne(TimeSpan.FromSeconds(1)), "Event did not fire");

            Assert.IsTrue(enabledChanged);
        }
    }
}
