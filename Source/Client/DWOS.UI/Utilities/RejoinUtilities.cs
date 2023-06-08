using System.Collections.Generic;
using System.Linq;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;

namespace DWOS.UI.Utilities
{
    public static class RejoinUtilities
    {
        public static bool CanRejoin(OrdersDataSet.OrderRow checkOrder, OrdersDataSet.OrderRow originalOrder)
        {
            if (checkOrder == null || originalOrder == null || checkOrder.WorkStatus == ApplicationSettings.Current.WorkStatusHold)
            {
                return false;
            }

            var checkPartId = checkOrder.IsPartIDNull() ? -1 : checkOrder.PartID;
            var originalPartId = originalOrder.IsPartIDNull() ? -1 : originalOrder.PartID;

            var checkPo = checkOrder.IsPurchaseOrderNull() ? null : checkOrder.PurchaseOrder;
            var originalPo = originalOrder.IsPurchaseOrderNull() ? null : originalOrder.PurchaseOrder;

            var checkStatus = checkOrder.IsStatusNull() ? null : checkOrder.Status;
            var originalStatus = originalOrder.IsStatusNull() ? null : originalOrder.Status;

            return checkOrder.OrderID != originalOrder.OrderID &&
                checkPartId == originalPartId &&
                checkPo == originalPo &&
                checkStatus == originalStatus &&
                checkOrder.WorkStatus == originalOrder.WorkStatus &&
                (IsWorkStatusAfterProcessing(checkOrder.WorkStatus) || CanRejoinProcess(checkOrder, originalOrder));
        }

        private static bool IsWorkStatusAfterProcessing(string workStatus)
        {
            return workStatus == ApplicationSettings.Current.WorkStatusPartMarking ||
                workStatus == ApplicationSettings.Current.WorkStatusFinalInspection ||
                workStatus == ApplicationSettings.Current.WorkStatusShipping;
        }

        private static bool CanRejoinProcess(OrdersDataSet.OrderRow checkOrder, OrdersDataSet.OrderRow originalOrder)
        {
            return checkOrder.CurrentLocation == originalOrder.CurrentLocation &&
                GetCurrentProcessForRejoin(checkOrder)?.ProcessID == GetCurrentProcessForRejoin(originalOrder)?.ProcessID;
        }

        private static OrdersDataSet.OrderProcessesRow GetCurrentProcessForRejoin(OrdersDataSet.OrderRow order)
        {
            if (order == null)
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderProcessesRow> processes = order.GetOrderProcessesRows();

            if (processes.Count == 0)
            {
                using (var taOrderProcesses = new OrderProcessesTableAdapter())
                {
                    var dtOrderProcess = new OrdersDataSet.OrderProcessesDataTable();
                    taOrderProcesses.FillBy(dtOrderProcess, order.OrderID);
                    processes = dtOrderProcess.ToList();
                }
            }

            if (order.WorkStatus == ApplicationSettings.Current.WorkStatusInProcess ||
                order.WorkStatus == ApplicationSettings.Current.WorkStatusChangingDepartment ||
                order.WorkStatus == ApplicationSettings.Current.WorkStatusPendingOR ||
                order.WorkStatus == ApplicationSettings.Current.WorkStatusPendingImportExportReview)
            {
                // Return first order process with a null EndDate
                return processes
                    .OrderBy(p => p.StepOrder)
                    .FirstOrDefault(p => p.IsEndDateNull());
            }
            else if (order.WorkStatus == ApplicationSettings.Current.WorkStatusPendingQI ||
                order.WorkStatus == ApplicationSettings.Current.WorkStatusPendingReworkPlanning ||
                order.WorkStatus == ApplicationSettings.Current.WorkStatusPendingJoin)
            {
                // Return the previous process
                return processes
                    .OrderByDescending(p => p.StepOrder)
                    .FirstOrDefault(p => !p.IsStartDateNull() && !p.IsEndDateNull());
            }
            else if (order.WorkStatus == ApplicationSettings.Current.WorkStatusHold)
            {
                // Unused - cannot rejoin orders in hold
                return null;
            }

            // Return the previous non-rework process
            return processes
                .OrderByDescending(p => p.StepOrder)
                .FirstOrDefault(p => !p.IsStartDateNull() && !p.IsEndDateNull() && p.OrderProcessType == (int)OrderProcessType.Normal);
        }


    }
}
