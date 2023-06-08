using DWOS.Data.Order;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DWOS.Data.Datasets;
using DWOS.Shared.Settings;
using DWOS.Shared.Utilities;
using Moq;

namespace DWOS.Data.Tests.Order
{
    [TestClass]
    public class OrderUtilitiesTests
    {
        private const string WorkStatusPendingJoin = "Pending Join";
        private const string WorkStatusPartMarking = "Part Marking";
        private const string WorkStatusFinalInspection = "Final Inspection";
        private const string WorkStatusShipping = "Shipping";
        private const string DepartmentQualityAssurance = "QA Dept.";
        private const string DepartmentShipping = "Shipping Dept.";
        private Mock<ISettingsPersistence> _settingsPersistenceMock;
        private ApplicationSettings _appSettings;

        [TestInitialize]
        public void Initialize()
        {
            _settingsPersistenceMock = new Mock<ISettingsPersistence>();
            var pathProviderMock = new Mock<IPathProvider>();
            var settingsProviderMock = new Mock<IDwosApplicationSettingsProvider>();
            _appSettings = new ApplicationSettings(_settingsPersistenceMock.Object, pathProviderMock.Object);
            settingsProviderMock.Setup(m => m.Settings)
                .Returns(_appSettings);

            DependencyContainer.Register(settingsProviderMock.Object);

           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "WorkStatusPendingJoin")))
                .Callback((SettingValue v) => v.Value = WorkStatusPendingJoin);

            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "WorkStatusPartMarking")))
                .Callback((SettingValue v) => v.Value = WorkStatusPartMarking);

            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "WorkStatusFinalInspection")))
                .Callback((SettingValue v) => v.Value = WorkStatusFinalInspection);

            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "WorkStatusShipping")))
                .Callback((SettingValue v) => v.Value = WorkStatusShipping);

            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "DepartmentQA")))
                .Callback((SettingValue v) => v.Value = DepartmentQualityAssurance);

            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "DepartmentShipping")))
                .Callback((SettingValue v) => v.Value = DepartmentShipping);

            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "DepartmentShipping")))
                .Callback((SettingValue v) => v.Value = DepartmentShipping);
        }

        [TestMethod]
        public void WorkStatusAfterProcessingTest()
        {
           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "PartMarkingEnabled")))
                .Callback((SettingValue v) => v.Value = "true");

           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "COCEnabled")))
                .Callback((SettingValue v) => v.Value = "true");

           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "COCSkipEnabled")))
                .Callback((SettingValue v) => v.Value = "false");

            // Case - ReworkInt
            Assert.AreEqual(WorkStatusPendingJoin, OrderUtilities.WorkStatusAfterProcessing(OrderType.ReworkInt, true, false));

            // Case - Part Marking
            Assert.AreEqual(WorkStatusPartMarking, OrderUtilities.WorkStatusAfterProcessing(OrderType.Normal, true, false));

            // Case - COC
            Assert.AreEqual(WorkStatusFinalInspection, OrderUtilities.WorkStatusAfterProcessing(OrderType.Normal, false, false));

            // Case - Shipping
            _appSettings.ReloadSettings();
            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "COCEnabled")))
                .Callback((SettingValue v) => v.Value = "false");

            Assert.AreEqual(WorkStatusShipping, OrderUtilities.WorkStatusAfterProcessing(OrderType.Normal, false, false));

            // Case: skip part mark
            _appSettings.ReloadSettings();
           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "COCEnabled")))
                .Callback((SettingValue v) => v.Value = "true");
           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "PartMarkingEnabled")))
                .Callback((SettingValue v) => v.Value = "false");

            Assert.AreEqual(WorkStatusFinalInspection, OrderUtilities.WorkStatusAfterProcessing(OrderType.Normal, true, false));

            // Case: Allow skipping COC
            _appSettings.ReloadSettings();

           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "COCSkipEnabled")))
                .Callback((SettingValue v) => v.Value = "true");

            Assert.AreEqual(WorkStatusShipping, OrderUtilities.WorkStatusAfterProcessing(OrderType.Normal, false, false));

            // Case: Require COC for order
            Assert.AreEqual(WorkStatusFinalInspection, OrderUtilities.WorkStatusAfterProcessing(OrderType.Normal, false, true));
        }

        [TestMethod]
        public void WorkStatusAfterQuarantineTest()
        {
           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "COCSkipEnabled")))
                .Callback((SettingValue v) => v.Value = "false");

            // Case - COC enabled
           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "COCEnabled")))
                .Callback((SettingValue v) => v.Value = "true");

            Assert.AreEqual(WorkStatusFinalInspection, OrderUtilities.WorkStatusAfterQuarantine(false));

            // Case - COC disabled
            _appSettings.ReloadSettings();
           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "COCEnabled")))
                .Callback((SettingValue v) => v.Value = "false");

            Assert.AreEqual(WorkStatusShipping, OrderUtilities.WorkStatusAfterQuarantine(false));

            // Case: Allow skipping COC
            _appSettings.ReloadSettings();
           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "COCEnabled")))
                .Callback((SettingValue v) => v.Value = "true");

           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "COCSkipEnabled")))
                .Callback((SettingValue v) => v.Value = "true");

            Assert.AreEqual(WorkStatusShipping, OrderUtilities.WorkStatusAfterQuarantine(false));

            // Case: Require COC for order
            Assert.AreEqual(WorkStatusFinalInspection, OrderUtilities.WorkStatusAfterQuarantine(true));
        }

        [TestMethod]
        public void LocationAfterQuarantineTest()
        {
           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "COCSkipEnabled")))
                .Callback((SettingValue v) => v.Value = "false");

            // Case - COC enabled
           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "COCEnabled")))
                .Callback((SettingValue v) => v.Value = "true");

            Assert.AreEqual(DepartmentQualityAssurance, OrderUtilities.LocationAfterQuarantine(false));

            // Case - COC disabled
            _appSettings.ReloadSettings();
           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "COCEnabled")))
                .Callback((SettingValue v) => v.Value = "false");

            Assert.AreEqual(DepartmentShipping, OrderUtilities.LocationAfterQuarantine(false));

            // Case: Allow skipping COC
            _appSettings.ReloadSettings();
           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "COCEnabled")))
                .Callback((SettingValue v) => v.Value = "true");

           _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "COCSkipEnabled")))
                .Callback((SettingValue v) => v.Value = "true");

            Assert.AreEqual(DepartmentShipping, OrderUtilities.LocationAfterQuarantine(false));

            // Case: Require COC for order
            Assert.AreEqual(DepartmentQualityAssurance, OrderUtilities.LocationAfterQuarantine(true));
        }
    }
}
