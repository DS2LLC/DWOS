using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities.Convertors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.UI.Tests.Utilities.Convertors
{
    [TestClass]
    public class ProcessTimeConverterTests
    {
        [TestMethod]
        public void ConvertRemainingTimeTest()
        {
            const int timeRemainingMinutes = 30;
            var expected = DateUtilities.ToDifferenceShortHand(timeRemainingMinutes);
            var culture = CultureInfo.InvariantCulture;

            var target = new ProcessTimeConverter();

            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            var row = dataTable.NewOrderStatusRow();
            InitializeRowData(row);
            row.RemainingTime = timeRemainingMinutes.ToString();
            dataTable.AddOrderStatusRow(row);
            var view = OrderStatusData.CreateFrom(row);

            var actual = target.Convert(view, typeof(OrderStatusData), null, culture) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBackTest()
        {
            var target = new ProcessTimeConverter();
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
