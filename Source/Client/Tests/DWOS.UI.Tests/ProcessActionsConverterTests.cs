using System;
using System.Data;
using System.Globalization;
using System.Windows.Media.Imaging;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Shared.Settings;
using DWOS.Shared.Utilities;
using DWOS.UI.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DWOS.UI.Tests
{
    [TestClass]
    public class ProcessActionsConverterTests
    {
        private const string InProcess = "In Process";
        private const string ChangingDepartment = "Changing Department";
        private const string PendingInspection = "Pending Inspection";
        private const string FinalInspection = "Final Inspection";
        private const string PendingOrderReview = "Pending Order Review";

        private Mock<ISettingsPersistence> _settingsPersistenceMock;

        [TestInitialize]
        public void Initialize()
        {
            _settingsPersistenceMock = new Mock<ISettingsPersistence>();

            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(val => val.Name == "WorkStatusInProcess")))
                .Callback((SettingValue v) => v.Value = InProcess);

            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(val => val.Name == "WorkStatusChangingDepartment")))
                .Callback((SettingValue v) => v.Value = ChangingDepartment);

            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(val => val.Name == "WorkStatusPendingQI")))
                .Callback((SettingValue v) => v.Value = PendingInspection);

            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(val => val.Name == "WorkStatusFinalInspection")))
                .Callback((SettingValue v) => v.Value = FinalInspection);

            _settingsPersistenceMock
                .Setup(m => m.LoadFromDataStore(It.Is<SettingValue>(val => val.Name == "WorkStatusPendingOR")))
                .Callback((SettingValue v) => v.Value = PendingOrderReview);

            var pathProviderMock = new Mock<IPathProvider>();
            var settingsProviderMock = new Mock<IDwosApplicationSettingsProvider>();
            settingsProviderMock.Setup(m => m.Settings)
                .Returns(new ApplicationSettings(_settingsPersistenceMock.Object, pathProviderMock.Object));

            DependencyContainer.Register(settingsProviderMock.Object);
        }

        [TestMethod]
        public void ConvertBatchTest()
        {
            var target = new ProcessActionsConverter();
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            row.InBatch = true;
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var convert= target.Convert(view, typeof(BitmapImage), null, culture);
            Assert.IsNull(convert);
        }

        [TestMethod]
        public void ConvertInProcessInDepartmentTest()
        {
            var target = new ProcessActionsConverter();
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            row.CurrentLocation = Settings.Default.CurrentDepartment;
            row.WorkStatus = InProcess;
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var convert= target.Convert(view, typeof(BitmapImage), null, culture);
            Assert.IsInstanceOfType(convert, typeof(BitmapImage));
        }

        [TestMethod]
        public void ConvertInProcessOutsideDepartmentTest()
        {
            var target = new ProcessActionsConverter();
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            row.CurrentLocation = Settings.Default.CurrentDepartment + "Test";
            row.WorkStatus = InProcess;
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var convert= target.Convert(view, typeof(BitmapImage), null, culture);
            Assert.IsNull(convert);
        }

        [TestMethod]
        public void ConvertChangingInDepartmentTest()
        {
            var target = new ProcessActionsConverter();
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            row.NextDept = Settings.Default.CurrentDepartment;
            row.WorkStatus = ChangingDepartment;
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var convert= target.Convert(view, typeof(BitmapImage), null, culture);
            Assert.IsInstanceOfType(convert, typeof(BitmapImage));
        }

        [TestMethod]
        public void ConvertChangingOutsideDepartmentTest()
        {
            var target = new ProcessActionsConverter();
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            row.NextDept = Settings.Default.CurrentDepartment + "Test";
            row.WorkStatus = ChangingDepartment;
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var convert= target.Convert(view, typeof(BitmapImage), null, culture);
            Assert.IsNull(convert);
        }

        [TestMethod]
        public void ConvertPendingInspectionTest()
        {
            var target = new ProcessActionsConverter();
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            row.WorkStatus = PendingInspection;
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var convert= target.Convert(view, typeof(BitmapImage), null, culture);
            Assert.IsInstanceOfType(convert, typeof(BitmapImage));
        }

        [TestMethod]
        public void ConvertFinalInspectionTest()
        {
            var target = new ProcessActionsConverter();
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            row.WorkStatus = FinalInspection;
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var convert= target.Convert(view, typeof(BitmapImage), null, culture);
            Assert.IsInstanceOfType(convert, typeof(BitmapImage));
        }

        [TestMethod]
        public void ConvertPendingOrderReviewTest()
        {
            var target = new ProcessActionsConverter();
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            row.WorkStatus = PendingOrderReview;
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var convert= target.Convert(view, typeof(BitmapImage), null, culture);
            Assert.IsInstanceOfType(convert, typeof(BitmapImage));
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBackTest()
        {
            var target = new ProcessActionsConverter();
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
