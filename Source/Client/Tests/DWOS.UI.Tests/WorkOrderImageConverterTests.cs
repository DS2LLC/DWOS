using System;
using System.Data;
using System.Globalization;
using DWOS.Data.Datasets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media.Imaging;

namespace DWOS.UI.Tests
{
    [TestClass]
    public class WorkOrderImageConverterTests
    {
        [TestMethod]
        public void ConvertNormalTest()
        {
            var now = DateTime.Now;
            var target = new WorkOrderImageConverter();
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row, now);
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var convert = target.Convert(view, typeof(BitmapImage), null, culture);
            Assert.IsNull(convert);
        }

        [TestMethod]
        public void ConvertHoldTest()
        {
            var now = DateTime.Now;
            var target = new WorkOrderImageConverter();
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row, now);
            row.Hold = true;
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var convert = target.Convert(view, typeof(BitmapImage), null, culture);
            Assert.IsInstanceOfType(convert, typeof(BitmapImage));
        }

        //intRework, extRework, quarantine

        [TestMethod]
        public void ConvertLostTest()
        {
            var now = DateTime.Now;
            var target = new WorkOrderImageConverter();
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row, now);
            row.OrderType = (int)OrderType.Lost;
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var convert = target.Convert(view, typeof(BitmapImage), null, culture);
            Assert.IsInstanceOfType(convert, typeof(BitmapImage));
        }

        [TestMethod]
        public void ConvertInternalReworkTest()
        {
            var now = DateTime.Now;
            var target = new WorkOrderImageConverter();
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row, now);
            row.OrderType = (int)OrderType.ReworkInt;
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var convert = target.Convert(view, typeof(BitmapImage), null, culture);
            Assert.IsInstanceOfType(convert, typeof(BitmapImage));
        }

        [TestMethod]
        public void ConvertExternalReworkTest()
        {
            var now = DateTime.Now;
            var target = new WorkOrderImageConverter();
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row, now);
            row.OrderType = (int)OrderType.ReworkExt;
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var convert = target.Convert(view, typeof(BitmapImage), null, culture);
            Assert.IsInstanceOfType(convert, typeof(BitmapImage));
        }

        [TestMethod]
        public void ConvertQuarantineTest()
        {
            var now = DateTime.Now;
            var target = new WorkOrderImageConverter();
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row, now);
            row.OrderType = (int)OrderType.Quarantine;
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var convert = target.Convert(view, typeof(BitmapImage), null, culture);
            Assert.IsInstanceOfType(convert, typeof(BitmapImage));
        }

        private static void InitializeRowData(OrderStatusDataSet.OrderStatusRow row, DateTime now)
        {
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
            row.PartProcessingCount = 10;
            row.Priority = "Normal";
            row.RemainingTime = null;
            row.RequiredDate = now;
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
            var target = new WorkOrderImageConverter();
            target.ConvertBack(null, null, null, null);
        }
    }
}