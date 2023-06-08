using System;
using DWOS.Reports;
using DWOS.Data.Datasets;
using NLog;
using DWOS.UI.Utilities;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data;

namespace DWOS.UI.Reports
{
    /// <summary>
    /// Allows use of <see cref="WorkOrderSummaryReport"/> with an Order ID
    /// instead of a <see cref="OrdersDataSet.OrderRow"/> with pre-loaded
    /// dependencies.
    /// </summary>
    public class WorkOrderSummaryReportWrapper : ReportWrapper
    {
        #region Fields

        private readonly Lazy<WorkOrderSummaryReport> _reportLazy;
        private readonly OrdersDataSet _dsOrders;

        #endregion

        #region Properties

        public int OrderId { get; }

        public override IReport InnerReport => _reportLazy.Value;

        public bool HideIncompleteProcesses { get; }

        #endregion

        #region Methods

        public WorkOrderSummaryReportWrapper(int orderId, bool hideIncompleteProcesses)
        {
            OrderId = orderId;
            _reportLazy = new Lazy<WorkOrderSummaryReport>(GenerateReport);
            _dsOrders = new OrdersDataSet { EnforceConstraints = false };
            HideIncompleteProcesses = hideIncompleteProcesses;
        }

        public override void Dispose()
        {
            if (_reportLazy.IsValueCreated)
            {
                _reportLazy.Value.Dispose();
            }

            _dsOrders.Dispose();
        }

        private WorkOrderSummaryReport GenerateReport()
        {
            LoadData(_dsOrders, OrderId);

            var order = _dsOrders.Order.FindByOrderID(OrderId);

            if (order == null)
            {
                LogManager.GetCurrentClassLogger().Warn($"Could not load data for Order # {OrderId}");
                return null;
            }

            return new WorkOrderSummaryReport(order) { HideIncompleteProcesses = HideIncompleteProcesses };
        }

        private static void LoadData(OrdersDataSet dsOrders, int orderId)
        {
            using (new UsingDataSetLoad(dsOrders))
            {
                using(var taPriceUnit = new PriceUnitTableAdapter())
                {
                    taPriceUnit.Fill(dsOrders.PriceUnit);
                }

                using(var taCustomerSummary = new CustomerSummaryTableAdapter())
                {
                    taCustomerSummary.FillByOrder(dsOrders.CustomerSummary, orderId);
                }

                using(var taOrderStatus = new d_OrderStatusTableAdapter())
                {
                    taOrderStatus.Fill(dsOrders.d_OrderStatus);
                }

                using(var taPriority = new d_PriorityTableAdapter())
                {
                    taPriority.Fill(dsOrders.d_Priority);
                }

                using(var taUserSummary = new UserSummaryTableAdapter())
                {
                    taUserSummary.Fill(dsOrders.UserSummary);
                }

                using(var taCustomerShippingSummary = new CustomerShippingSummaryTableAdapter())
                {
                    taCustomerShippingSummary.FillByOrder(dsOrders.CustomerShippingSummary, orderId);
                }

                using(var taCustomerAddress = new CustomerAddressTableAdapter())
                {
                    taCustomerAddress.FillByOrder(dsOrders.CustomerAddress, orderId);
                }

                using(var taPartSummary = new PartSummaryTableAdapter())
                {
                    taPartSummary.FillByOrder(dsOrders.PartSummary, orderId);
                }

                using(var taProcessingLine = new ProcessingLineTableAdapter())
                {
                    taProcessingLine.Fill(dsOrders.ProcessingLine);
                }

                using(var taMedia = new MediaTableAdapter())
                {
                    taMedia.FillByOrder(dsOrders.Media, orderId);
                }

                using(var taOrder = new OrderTableAdapter())
                {
                    taOrder.FillByOrderID(dsOrders.Order, orderId);
                }

                using(var taOrderFees = new OrderFeesTableAdapter())
                {
                    taOrderFees.FillByOrder(dsOrders.OrderFees, orderId);
                }

                using(var taOrderProcesses = new OrderProcessesTableAdapter())
                {
                    taOrderProcesses.FillBy(dsOrders.OrderProcesses, orderId);
                }

                using(var taOrder_Media = new Order_MediaTableAdapter())
                {
                    taOrder_Media.FillByOrder(dsOrders.Order_Media, orderId);
                }

                using(var taOrderDocumentLink = new Order_DocumentLinkTableAdapter())
                {
                    taOrderDocumentLink.FillByOrder(dsOrders.Order_DocumentLink, orderId);
                }

                using(var taOrderFeeType = new OrderFeeTypeTableAdapter())
                {
                    taOrderFeeType.Fill(dsOrders.OrderFeeType);
                }

                using (var taDepts = new d_DepartmentTableAdapter())
                {
                    taDepts.Fill(dsOrders.d_Department);
                }

                using (var taWS = new d_WorkStatusTableAdapter())
                {
                    taWS.Fill(dsOrders.d_WorkStatus);
                }

                using(var taOrderSerialNumber = new OrderSerialNumberTableAdapter())
                {
                    taOrderSerialNumber.FillByOrder(dsOrders.OrderSerialNumber, orderId);
                }

                using(var taOrderNote = new OrderNoteTableAdapter())
                {
                    taOrderNote.FillByOrder(dsOrders.OrderNote, orderId);
                }
            }
        }

        #endregion
    }
}
