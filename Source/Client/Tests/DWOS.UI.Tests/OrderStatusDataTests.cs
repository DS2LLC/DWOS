using System;
using System.Data;
using System.Linq;
using DWOS.Data.Datasets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.UI.Tests
{
    [TestClass]
    public class OrderStatusDataTests
    {
        [TestMethod]
        public void CreateFromTest()
        {
            var row = GenerateNewRow();

            var actual = OrderStatusData.CreateFrom(row);
            Assert.IsNotNull(actual);

            Assert.AreEqual(row.WO, actual.WO);
            Assert.AreEqual(row.PO, actual.PO);
            Assert.AreEqual(row.Customer, actual.Customer);
            Assert.AreEqual(row.Part, actual.Part);
            Assert.AreEqual(row.EstShipDate, actual.EstShipDate);
            Assert.AreEqual(row.Priority, actual.Priority);
            Assert.AreEqual(row.WorkStatus, actual.WorkStatus);
            Assert.AreEqual(row.CurrentLocation, actual.CurrentLocation);
            Assert.AreEqual(row.NextDept, actual.NextDept);
            Assert.AreEqual(row.CurrentProcess, actual.CurrentProcess);
            Assert.AreEqual(row.OrderType, actual.OrderType);
            Assert.AreEqual(row.PartQuantity, actual.PartQuantity);
            Assert.AreEqual(row.Hold, actual.Hold);
            Assert.AreEqual(row.InBatch, actual.InBatch);
            Assert.AreEqual(row.SurfaceArea, actual.SurfaceArea);
            Assert.AreEqual(row.RemainingTime, actual.RemainingTime);
            Assert.AreEqual(row.CurrentProcessDue, actual.CurrentProcessDue);
            Assert.AreEqual(row.SchedulePriority, actual.SchedulePriority);
            Assert.AreEqual(row.PartProcessingCount, actual.PartProcessingCount);
            Assert.AreEqual(row.SalesOrderID, actual.SalesOrderID);
            Assert.AreEqual(row.WorkStatusDuration, actual.WorkStatusDuration);
            Assert.AreEqual(row.CustomerWO, actual.CustomerWO);
            Assert.AreEqual(row.RequiredDate, actual.RequiredDate);
            Assert.AreEqual(row.ActiveTimerCount, actual.ActiveTimerCount);
            Assert.AreEqual(row.HasPartMark, actual.HasPartMark);
            Assert.AreEqual(row.CurrentLine, actual.CurrentLine);
            Assert.AreEqual(row.OrderDate, actual.OrderDate);
            Assert.AreEqual(row.OrderNoteCount, actual.OrderNoteCount);
            Assert.AreEqual(row["Operators"], actual.Operators);
            Assert.AreEqual(row["SerialNumbers"], actual.SerialNumbers);
            Assert.AreEqual(row["ProductClass"], actual.ProductClass);
            Assert.AreEqual(row["CurrentLineString"], actual.CurrentLineString);
            Assert.AreEqual(row["CUSTOM_Major"], actual["CUSTOM_Major"]);
        }

        [TestMethod]
        public void IndexerTest()
        {
            var row = GenerateNewRow();

            var actual = OrderStatusData.CreateFrom(row);
            Assert.IsNotNull(actual);

            var tableColumn = row.Table.Columns.OfType<DataColumn>().ToList();

            foreach (var column in tableColumn)
            {
                if (column.ColumnName == "PartID")
                {
                    continue;
                }

                var expectedValue = row[column];
                var actualValue = actual[column.ColumnName];

                Assert.AreEqual(expectedValue, actualValue, $"Failed for {column.ColumnName}");
            }

            Assert.IsNull(actual["NOT_IN_TABLE"]);
        }

        private static OrderStatusDataSet.OrderStatusRow GenerateNewRow()
        {
            var now = DateTime.Now;

            // Initialize table
            var dataTable = new OrderStatusDataSet.OrderStatusDataTable();
            dataTable.Columns.Add("Operators", typeof(string));
            dataTable.Columns.Add("SerialNumbers", typeof(string));
            dataTable.Columns.Add("ProductClass", typeof(string));
            dataTable.Columns.Add("CurrentLineString", typeof(string));
            dataTable.Columns.Add("CUSTOM_Major", typeof(string));

            // Initialize row
            var row = dataTable.NewOrderStatusRow();
            row.WO = 1001;
            row.OrderDate = new DateTime(2017, 1, 1);
            row.OrderType = (int) OrderType.Normal;
            row.ActiveTimerCount = 1;
            row.CurrentLine = 500;
            row.CurrentLocation = "In Process";
            row.CurrentProcess = "Test";
            row.CurrentProcessDue = now;
            row.Customer = "Test Customer";
            row.CustomerWO = "Customer WO";
            row.EstShipDate = now;
            row.HasPartMark = true;
            row.Hold = true;
            row.InBatch = true;
            row.NextDept = "Final Inspection";
            row.PO = "Purchase Order";
            row.Part = "Test Part";
            row.PartID = 1;
            row.PartQuantity = 10;
            row.PartProcessingCount = 5;
            row.Priority = "Normal";
            row.RemainingTime = "2017-06-14 14:40:00.0000000";
            row.RequiredDate = now;
            row.SchedulePriority = 1;
            row.WorkStatus = "In Process";
            row.SurfaceArea = 5.0D;
            row.SalesOrderID = 100;
            row.WorkStatusDuration = 25;
            row.OrderNoteCount = 1;
            row["Operators"] = "Testing";
            row["SerialNumbers"] = "00001";
            row["ProductClass"] = "Product Class A";
            row["CurrentLineString"] = "Current Line";
            row["CUSTOM_Major"] = "Major Value";
            row.IsInRework = false;

            dataTable.AddOrderStatusRow(row);
            return row;
        }


    }
}
