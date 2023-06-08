using System;
using System.Data;
using System.Globalization;
using DWOS.Data.Datasets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media.Imaging;

namespace DWOS.UI.Tests
{
    [TestClass]
    public class StatusImageConverterTests
    {
        [TestMethod]
        public void ConvertNotInBatchTest()
        {
            var now = DateTime.Now;
            var target = new StatusImageConverter();
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
        public void ConvertInBatchTest()
        {
            var now = DateTime.Now;
            var target = new StatusImageConverter();
            var culture= CultureInfo.InvariantCulture;

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row, now);
            row.InBatch = true;
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

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBackTest()
        {
            var target = new StatusImageConverter();
            target.ConvertBack(null, null, null, null);
        }
    }
}
