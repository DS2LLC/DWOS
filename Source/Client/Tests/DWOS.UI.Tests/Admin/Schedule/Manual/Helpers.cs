using System;
using DWOS.Data.Datasets;

namespace DWOS.UI.Tests.Admin.Schedule.Manual
{
    public static class Helpers
    {
        public const string InProcess = "InProcess";
        public const string ChangingDepartment = "ChangingDepartment";
        public const string FinalInspection = "FinalInspection";
        public const string DepartmentChem = "Chem";
        public const string DepartmentQuality = "QA";
        public const string DepartmentSales = "Sales";

        public static WipData GenerateTestData()
        {
            var dsOrderStatus = new OrderStatusDataSet();

            // Unscheduled order - Chem
            var unscheduledChemOrder = dsOrderStatus.OrderStatus.NewOrderStatusRow();
            InitializeRowData(unscheduledChemOrder, 1000);
            unscheduledChemOrder.CurrentLocation = DepartmentChem;
            unscheduledChemOrder.WorkStatus = InProcess;
            unscheduledChemOrder.SchedulePriority = 0;
            dsOrderStatus.OrderStatus.AddOrderStatusRow(unscheduledChemOrder);

            // Scheduled order - Chem
            var scheduledChemOrder = dsOrderStatus.OrderStatus.NewOrderStatusRow();
            InitializeRowData(scheduledChemOrder, 1001);
            scheduledChemOrder.CurrentLocation = DepartmentChem;
            scheduledChemOrder.WorkStatus = InProcess;
            scheduledChemOrder.SchedulePriority = 1;
            dsOrderStatus.OrderStatus.AddOrderStatusRow(scheduledChemOrder);

            // Unscheduled batch - Chem
            var unscheduledChemBatch = dsOrderStatus.BatchStatus.NewBatchStatusRow();
            InitializeRowData(unscheduledChemBatch, 1);
            unscheduledChemBatch.CurrentLocation = DepartmentChem;
            unscheduledChemBatch.WorkStatus = InProcess;
            unscheduledChemBatch.SchedulePriority = 0;
            dsOrderStatus.BatchStatus.AddBatchStatusRow(unscheduledChemBatch);

            // Scheduled batch - Chem
            var scheduledChemBatch = dsOrderStatus.BatchStatus.NewBatchStatusRow();
            InitializeRowData(scheduledChemBatch, 2);
            scheduledChemBatch.CurrentLocation = DepartmentChem;
            scheduledChemBatch.WorkStatus = InProcess;
            scheduledChemBatch.SchedulePriority = 1;
            dsOrderStatus.BatchStatus.AddBatchStatusRow(scheduledChemBatch);

            // Unscheduled order - QA
            var unscheduledQualityOrder = dsOrderStatus.OrderStatus.NewOrderStatusRow();
            InitializeRowData(unscheduledQualityOrder, 1004);
            unscheduledQualityOrder.CurrentLocation = DepartmentQuality;
            unscheduledQualityOrder.WorkStatus = InProcess;
            unscheduledQualityOrder.SchedulePriority = 0;
            dsOrderStatus.OrderStatus.AddOrderStatusRow(unscheduledQualityOrder);

            // Unscheduled order - in Sales, going to QA
            var unscheduledSalesOrder = dsOrderStatus.OrderStatus.NewOrderStatusRow();
            InitializeRowData(unscheduledSalesOrder, 1005);
            unscheduledSalesOrder.CurrentLocation = DepartmentSales;
            unscheduledSalesOrder.WorkStatus = ChangingDepartment;
            unscheduledSalesOrder.NextDept = DepartmentQuality;
            unscheduledSalesOrder.SchedulePriority = 0;
            dsOrderStatus.OrderStatus.AddOrderStatusRow(unscheduledSalesOrder);

            return new WipData(dsOrderStatus);
        }

        private static void InitializeRowData(OrderStatusDataSet.OrderStatusRow row, int orderId)
        {
            var now = DateTime.Now;

            row.WO = orderId;
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

        private static void InitializeRowData(OrderStatusDataSet.BatchStatusRow row, int batchId)
        {
            var now = DateTime.Now;
            row.ActiveTimerCount = 0;
            row.BatchID = batchId;
            row.CurrentLine = 0;
            row.CurrentLocation = "In Process";
            row.CurrentProcess = "Test";
            row.Fixture = string.Empty;
            row.NextDept = "Final Inspection";
            row.OpenDate = now;
            row.OrderCount = 1;
            row.PartCount = 10;
            row.SchedulePriority = 0;
            row.TotalSurfaceArea = 5.0D;
            row.WorkStatus = "In Process";
        }
    }
}
