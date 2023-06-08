using System;
using System.Globalization;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Shared.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DWOS.UI.Tests
{
    [TestClass]
    public class PartQtyConverterTests
    {
        private Mock<ISettingsPersistence> _settingsPersistenceMock;
        private Mock<IDwosApplicationSettingsProvider> _settingsProviderMock;

        [TestInitialize]
        public void Initialize()
        {
            _settingsPersistenceMock = new Mock<ISettingsPersistence>();
            _settingsPersistenceMock.Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(val => val.Name == "AllowPartialProcessLoads"))).Callback((SettingValue v) => v.Value = "true");
            var pathProviderMock = new Mock<IPathProvider>();
            _settingsProviderMock = new Mock<IDwosApplicationSettingsProvider>();
            _settingsProviderMock.Setup(m => m.Settings).Returns(new ApplicationSettings(_settingsPersistenceMock.Object, pathProviderMock.Object));
        }

        [TestMethod]
        public void ConvertNormalTest()
        {
            var target = new PartQtyConverter(_settingsProviderMock.Object);
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var actualCount= target.Convert(view.PartQuantity, typeof(string), view, culture);
            Assert.IsNotNull(actualCount);

            // Value can be an integer or string
            Assert.AreEqual(row.PartQuantity.ToString(), actualCount.ToString());
        }

        [TestMethod]
        public void ConvertNullTest()
        {
            var target = new PartQtyConverter(_settingsProviderMock.Object);
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            row.SetPartQuantityNull();
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var actualCountString = target.Convert(view.PartQuantity, typeof(string), view, culture);
            Assert.AreEqual(string.Empty, actualCountString);
        }

        [TestMethod]
        public void ConvertPartialTest()
        {
            var target = new PartQtyConverter(_settingsProviderMock.Object);
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            row.PartProcessingCount = 5;
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var actualCount = target.Convert(view.PartQuantity, typeof(string), view, culture);
            Assert.IsNotNull(actualCount);

            Assert.AreEqual("5 / 10", actualCount.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBackTest()
        {
            var target = new PartQtyConverter(_settingsProviderMock.Object);
            target.ConvertBack(null, null, null, null);
        }

        private static void InitializeRowData(OrderStatusDataSet.OrderStatusRow row)
        {
            var now = DateTime.Now;

            row.WO = 1000;
            row.OrderType = (int) OrderType.Normal;
            row.ActiveTimerCount = 0;
            row.SetCurrentLineNull();
            row.CurrentLocation = "In Process";
            row.CurrentProcess = "Test";
            row.CurrentProcessDue = now;
            row.Customer = "Test Customer";
            row.CustomerWO = null;
            row.EstShipDate = now;
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
            row.RequiredDate = now;
            row.SchedulePriority = 0;
            row.WorkStatus = "In Process";
            row.SurfaceArea = 5.0D;
            row.SetSalesOrderIDNull();
            row.WorkStatusDuration = 0;
        }
    }
}
