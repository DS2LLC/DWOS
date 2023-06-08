using System;
using System.Linq;
using DWOS.Data.Datasets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.UI.Tests
{
    [TestClass]
    public class BatchStatusDataTests
    {
        [TestMethod]
        public void CreateFromTest()
        {
            // Setup
            var now = DateTime.Now;

            var dsOrderStatus = new OrderStatusDataSet();

            dsOrderStatus.BatchStatus.Columns.Add("Operators", typeof(string));
            dsOrderStatus.BatchStatus.Columns.Add("CurrentLineString", typeof(string));

            var batch = dsOrderStatus.BatchStatus.NewBatchStatusRow();
            batch.BatchID = 1;
            batch.ActiveTimerCount = 1;
            batch.CurrentLocation = "Chem";
            batch.CurrentLine = 10;
            batch.CurrentProcess = "Test Process";
            batch.Fixture = "B";
            batch.NextDept = "None";
            batch.OpenDate = now;
            batch.OrderCount = 1;
            batch.PartCount = 5;
            batch.TotalSurfaceArea = 25.5D;
            batch.TotalWeight = 10.5M;
            batch.WorkStatus = "In Process";
            batch.SchedulePriority = 0;
            batch["Operators"] = "Test";
            batch["CurrentLineString"] = "Line";
            dsOrderStatus.BatchStatus.AddBatchStatusRow(batch);

            var batchOrder = dsOrderStatus.BatchOrderStatus.NewBatchOrderStatusRow();
            batchOrder.OrderID = 1;
            batchOrder.BatchStatusRow = batch;
            batchOrder.PartID = 5;
            batchOrder.PartName = "Test Part";
            batchOrder.PurchaseOrder = "001";
            batchOrder.BatchPartQuantity = 5;
            batchOrder.CustomerName = "Test Customer";
            dsOrderStatus.BatchOrderStatus.AddBatchOrderStatusRow(batchOrder);

            // Test
            var actual = BatchStatusData.CreateFrom(batch);
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Orders);
            Assert.AreEqual(1, actual.Orders.Count);

            Assert.AreEqual(batch.CurrentLocation, actual.CurrentLocation);
            Assert.AreEqual(batch.ActiveTimerCount, actual.ActiveTimerCount);
            Assert.AreEqual(batch.BatchID, actual.BatchID);
            Assert.AreEqual(batch.CurrentLine, actual.CurrentLineID);
            Assert.AreEqual("Line", actual.CurrentLine);
            Assert.AreEqual(batch.CurrentProcess, actual.CurrentProcess);
            Assert.AreEqual(batch.Fixture, actual.Fixture);
            Assert.AreEqual(batch.NextDept, actual.NextDept);
            Assert.AreEqual(batch.OpenDate, actual.OpenDate);
            Assert.AreEqual(batch.OrderCount, actual.OrderCount);
            Assert.AreEqual(batch.PartCount, actual.PartCount);
            Assert.AreEqual(batch.TotalSurfaceArea, actual.TotalSurfaceArea);
            Assert.AreEqual(batch.TotalWeight, actual.TotalWeight);
            Assert.AreEqual(batch.WorkStatus, actual.WorkStatus);
            Assert.AreEqual(batch["Operators"].ToString(), actual.Operators);

            var actualOrder = actual.Orders.FirstOrDefault();
            Assert.IsNotNull(actualOrder);
            Assert.AreEqual(batchOrder.OrderID, actualOrder.OrderID);
            Assert.AreEqual(batchOrder.PurchaseOrder, actualOrder.PurchaseOrder);
            Assert.AreEqual(batchOrder.CustomerName, actualOrder.CustomerName);
            Assert.AreEqual(batchOrder.PartID, actualOrder.PartID);
            Assert.AreEqual(batchOrder.PartName, actualOrder.PartName);
            Assert.AreEqual(batchOrder.BatchPartQuantity, actualOrder.BatchPartQuantity);
        }
    }
}
