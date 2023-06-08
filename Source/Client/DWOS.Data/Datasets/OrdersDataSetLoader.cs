using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using NLog;
using System;
using System.Linq;

namespace DWOS.Data.Datasets
{
    /// <summary>
    /// Loads data for an <see cref="OrdersDataSet"/> instance.
    /// </summary>
    public sealed class OrdersDataSetLoader : IDisposable
    {
        #region Fields

        private bool _commonDataLoaded;
        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dataset for this instance.
        /// </summary>
        public OrdersDataSet Dataset { get; }

        /// <summary>
        /// Gets the table adapter manager for this instance.
        /// </summary>
        public TableAdapterManager TableAdapterManager { get; }

        /// <summary>
        /// Gets a value that indicates what optional dependencies to retrieve for orders.
        /// </summary>
        /// <value>
        /// An enumeration value that <see cref="LoadOrder(int)"/> will check before
        /// retrieving loading optional dependencies.
        /// </value>
        public OptionalDependencies Options { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="OrdersDataSetLoader"/> class.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="options"></param>
        public OrdersDataSetLoader(OrdersDataSet dataset, OptionalDependencies options = OptionalDependencies.None)
        {
            Dataset = dataset ?? throw new ArgumentNullException(nameof(dataset));
            Options = options;
            TableAdapterManager = CreateTableAdapterManager();
        }

        /// <summary>
        /// Loads data for an order and its dependencies.
        /// </summary>
        /// <param name="orderId">
        /// ID of the order to retrieve data for.
        /// </param>
        public void LoadOrder(int orderId)
        {
            using (new UsingDataSetLoad(Dataset))
            {
                LoadCommonData();
                LoadOrderInner(orderId);
            }
        }

        private void LoadOrderInner(int orderId)
        {
            if (Dataset.Order.FindByOrderID(orderId) != null)
            {
                // Order has already been loaded
                return;
            }

            TableAdapterManager.OrderTableAdapter.FillByOrderID(Dataset.Order, orderId);

            var orderRow = Dataset.Order.FindByOrderID(orderId);

            if (orderRow == null)
            {
                LogManager.GetCurrentClassLogger().Warn($"Could not load Order {orderId}");
                return;
            }

            TableAdapterManager.PartSummaryTableAdapter.FillByOrder(Dataset.PartSummary, orderId);

            TableAdapterManager.SalesOrderTableAdapter.FillByOrderID(Dataset.SalesOrder, orderId);

            TableAdapterManager.OrderTemplateTableAdapter.FillByOrder(Dataset.OrderTemplate, orderId);

            if (orderRow.OrderTemplateRow != null && orderRow.OrderTemplateRow.PartID != orderRow.PartID)
            {
                TableAdapterManager.PartSummaryTableAdapter.FillByPart(Dataset.PartSummary, orderRow.OrderTemplateRow.PartID);
            }

            using (var taCustomFields = new CustomFieldTableAdapter { ClearBeforeFill = false })
            {
                taCustomFields.FillByCustomer(Dataset.CustomField, orderRow.CustomerID);
            }

            TableAdapterManager.OrderCustomFieldsTableAdapter.FillByOrder(Dataset.OrderCustomFields, orderId);

            TableAdapterManager.OrderFeesTableAdapter.FillByOrder(Dataset.OrderFees, orderId);

            TableAdapterManager.Order_MediaTableAdapter.FillByOrder(Dataset.Order_Media, orderId);

            TableAdapterManager.MediaTableAdapter.FillByOrderWOMedia(Dataset.Media, orderId);

            TableAdapterManager.Order_DocumentLinkTableAdapter.FillByOrder(Dataset.Order_DocumentLink, orderId);

            TableAdapterManager.CustomerCommunicationTableAdapter.FillByOrder(Dataset.CustomerCommunication, orderId);

            TableAdapterManager.COCTableAdapter.FillByOrderNoData(Dataset.COC, orderId);

            TableAdapterManager.OrderReviewTableAdapter.FillByOrder(Dataset.OrderReview, orderId);

            TableAdapterManager.OrderProcessesTableAdapter.FillBy(Dataset.OrderProcesses, orderId);

            TableAdapterManager.OrderProcessesOperatorTableAdapter.FillByOrder(Dataset.OrderProcessesOperator, orderId);

            using (var taProcessAliasSummary = new ProcessAliasSummaryTableAdapter { ClearBeforeFill = false })
            {
                taProcessAliasSummary.FillByOrder(Dataset.ProcessAliasSummary, orderId);
            }

            TableAdapterManager.PartInspectionTableAdapter.FillBy(Dataset.PartInspection, orderId);

            TableAdapterManager.OrderProcessAnswerTableAdapter.FillBy(Dataset.OrderProcessAnswer, orderId);

            TableAdapterManager.OrderPartMarkTableAdapter.FillByOrder(Dataset.OrderPartMark, orderId);

            TableAdapterManager.OrderNoteTableAdapter.FillByOrder(Dataset.OrderNote, orderId);

            TableAdapterManager.OrderHoldTableAdapter.Fill(Dataset.OrderHold, orderId);

            TableAdapterManager.BatchTableAdapter.FillByOrder(Dataset.Batch, orderId);

            TableAdapterManager.BatchOrderTableAdapter.FillByOrder(Dataset.BatchOrder, orderId);

            TableAdapterManager.BatchCOCTableAdapter.FillByOrderNoData(Dataset.BatchCOC, orderId);

            TableAdapterManager.BatchCOCOrderTableAdapter.FillByOrder(Dataset.BatchCOCOrder, orderId);

            TableAdapterManager.BatchProcessesTableAdapter.FillByOrder(Dataset.BatchProcesses, orderId);

            TableAdapterManager.BatchProcessesOperatorTableAdapter.FillByOrder(Dataset.BatchProcessesOperator, orderId);

            TableAdapterManager.BatchOperatorTableAdapter.FillByOrder(Dataset.BatchOperator, orderId);

            foreach (var batchId in orderRow.GetBatchOrderRows().Select(bo => bo.BatchID))
            {
                TableAdapterManager.LaborTimeTableAdapter.FillByBatch(Dataset.LaborTime, batchId);
                TableAdapterManager.BatchOperatorTimeTableAdapter.FillByBatch(Dataset.BatchOperatorTime, batchId);
            }

            TableAdapterManager.LaborTimeTableAdapter.FillByOrder(Dataset.LaborTime, orderId);

            TableAdapterManager.OrderOperatorTableAdapter.FillByOrder(Dataset.OrderOperator, orderId);

            TableAdapterManager.OrderOperatorTimeTableAdapter.FillByOrder(Dataset.OrderOperatorTime, orderId);

            TableAdapterManager.OrderSerialNumberTableAdapter.FillByOrder(Dataset.OrderSerialNumber, orderId);

            TableAdapterManager.OrderProductClassTableAdapter.FillByOrder(Dataset.OrderProductClass, orderId);

            TableAdapterManager.OrderWorkDescriptionTableAdapter.FillByOrder(Dataset.OrderWorkDescription, orderId);

            if (OptionsHasFlag(OptionalDependencies.Containers))
            {
                TableAdapterManager.OrderContainersTableAdapter.FillByOrder(Dataset.OrderContainers, orderId);
                TableAdapterManager.OrderContainerItemTableAdapter.FillByOrder(Dataset.OrderContainerItem, orderId);
            }

            if (OptionsHasFlag(OptionalDependencies.Shipments))
            {
                TableAdapterManager.OrderShipmentTableAdapter.FillByOrder(Dataset.OrderShipment, orderId);

                foreach (var shipmentId in orderRow.GetOrderShipmentRows().Select(os => os.ShipmentID))
                {
                    TableAdapterManager.ShipmentPackageTableAdapter.FillByShipmentPackage(Dataset.ShipmentPackage, shipmentId);
                }

                TableAdapterManager.BulkCOCOrderTableAdapter.FillByOrder(Dataset.BulkCOCOrder, orderId);

                foreach (var bulkCocId in orderRow.GetBulkCOCOrderRows().Select(bulkCocOrder => bulkCocOrder.BulkCOCID))
                {
                    TableAdapterManager.BulkCOCTableAdapter.FillByBulkCOC(Dataset.BulkCOC, bulkCocId);
                }

                TableAdapterManager.BillOfLadingOrderTableAdapter.FillByOrder(Dataset.BillOfLadingOrder, orderId);

                foreach (var billOfLadingId in orderRow.GetBillOfLadingOrderRows().Select(bol => bol.BillOfLadingID))
                {
                    TableAdapterManager.BillOfLadingTableAdapter.FillByBillOfLading(Dataset.BillOfLading, billOfLadingId);
                    TableAdapterManager.BillOfLadingMediaTableAdapter.FillByBillOfLading(Dataset.BillOfLadingMedia, billOfLadingId);
                    TableAdapterManager.BillOfLadingDocumentLinkTableAdapter.FillByBillOfLading(Dataset.BillOfLadingDocumentLink, billOfLadingId);
                    TableAdapterManager.MediaTableAdapter.FillByBillOfLading(Dataset.Media, billOfLadingId);
                }
            }

            if (OptionsHasFlag(OptionalDependencies.Changes))
            {
                TableAdapterManager.OrderChangeTableAdapter.FillByOrderID(Dataset.OrderChange, orderId);

                foreach (var orderChangeRow in Dataset.OrderChange.Where(oc => oc.ParentOrderID == orderId || oc.ChildOrderID == orderId))
                {
                    if (orderChangeRow.ParentOrderID == orderId)
                    {
                        // Retrieve child
                        LoadOrderInner(orderChangeRow.ChildOrderID);
                    }
                    else
                    {
                        // Retrieve parent
                        LoadOrderInner(orderChangeRow.ParentOrderID);
                    }
                }
            }

            if (OptionsHasFlag(OptionalDependencies.Rework))
            {
                TableAdapterManager.InternalReworkTableAdapter.FillByOrderID(Dataset.InternalRework, orderId);

                foreach (var reworkRow in Dataset.InternalRework.Where(rework => rework.OriginalOrderID == orderId || (!rework.IsReworkOrderIDNull() && rework.ReworkOrderID == orderId)))
                {
                    if (reworkRow.OriginalOrderID == orderId && !reworkRow.IsReworkOrderIDNull())
                    {
                        // Retrieve rework order
                        LoadOrderInner(reworkRow.ReworkOrderID);
                    }
                    else if (reworkRow.OriginalOrderID != orderId)
                    {
                        // Retrieve original order
                        LoadOrderInner(reworkRow.OriginalOrderID);
                    }
                }
            }

            if (OptionsHasFlag(OptionalDependencies.Approvals))
            {
                TableAdapterManager.OrderApprovalTableAdapter.FillByOrder(Dataset.OrderApproval, orderId);
            }
        }

        private void LoadCommonData()
        {
            if (_commonDataLoaded)
            {
                return;
            }

            using (var taHoldReason = new d_HoldReasonTableAdapter())
                taHoldReason.Fill(Dataset.d_HoldReason);

            using (var taOrderStatus = new d_OrderStatusTableAdapter())
                taOrderStatus.Fill(Dataset.d_OrderStatus);

            using (var taPriority = new d_PriorityTableAdapter())
                taPriority.Fill(Dataset.d_Priority);

            using (var taUserSummary = new UserSummaryTableAdapter())
                taUserSummary.Fill(Dataset.UserSummary);

            using (var taOrderFeeType = new OrderFeeTypeTableAdapter())
                taOrderFeeType.Fill(Dataset.OrderFeeType);

            using (var taFeeType = new d_FeeTypeTableAdapter())
                taFeeType.Fill(Dataset.d_FeeType);

            using (var taShippingCarrier = new d_ShippingCarrierTableAdapter())
                taShippingCarrier.Fill(Dataset.d_ShippingCarrier);

            using (var taCustomerShippingSummary = new CustomerShippingSummaryTableAdapter())
                taCustomerShippingSummary.Fill(Dataset.CustomerShippingSummary);

            using (var taCustomerAddress = new CustomerAddressTableAdapter())
                taCustomerAddress.Fill(Dataset.CustomerAddress);

            using (var taPriceUnit = new PriceUnitTableAdapter())
                taPriceUnit.Fill(Dataset.PriceUnit);

            using (var taCustomerSummary = new CustomerSummaryTableAdapter())
                taCustomerSummary.FillByActiveOrInUse(Dataset.CustomerSummary);

            using (var taContactSummary = new ContactSummaryTableAdapter())
                taContactSummary.Fill(Dataset.ContactSummary);

            using (var taDepts = new d_DepartmentTableAdapter())
                taDepts.Fill(Dataset.d_Department);

            using (var taWS = new d_WorkStatusTableAdapter())
                taWS.Fill(Dataset.d_WorkStatus);

            using (var taLine = new ProcessingLineTableAdapter())
                taLine.Fill(Dataset.ProcessingLine);

            using (var taLists = new ListsTableAdapter())
                taLists.Fill(Dataset.Lists);

            using (var taListValues = new ListValuesTableAdapter())
                taListValues.Fill(Dataset.ListValues);

            using (var taPartInspectionType = new PartInspectionTypeTableAdapter())
                taPartInspectionType.Fill(Dataset.PartInspectionType);

            using (var taWorkDescription = new WorkDescriptionTableAdapter())
            {
                taWorkDescription.Fill(Dataset.WorkDescription);
            }

            if (OptionsHasFlag(OptionalDependencies.Containers) || OptionsHasFlag(OptionalDependencies.Shipments))
            {
                using (var taShipmentPackageType = new ShipmentPackageTypeTableAdapter())
                    taShipmentPackageType.Fill(Dataset.ShipmentPackageType);
            }

            if (OptionsHasFlag(OptionalDependencies.Approvals))
            {
                using (var taOrderApprovalTerm = new OrderApprovalTermTableAdapter())
                {
                    taOrderApprovalTerm.Fill(Dataset.OrderApprovalTerm);
                }
            }

            _commonDataLoaded = true;
        }

        /// <summary>
        /// Retrieves a value that indicates if <see cref="Options"/> has a
        /// given flag.
        /// </summary>
        /// <remarks>
        /// This is a higher-performance alternative to
        /// <see cref="Enum.HasFlag(Enum)"/>.
        /// </remarks>
        /// <param name="flag">An enumeration value.</param>
        /// <returns>
        /// <c>true</c> if <see cref="Options"/> has the flag;
        /// otherwise, <c>false</c>.
        /// </returns>
        private bool OptionsHasFlag(OptionalDependencies flag) =>
            (Options & flag) == flag;

        private TableAdapterManager CreateTableAdapterManager()
        {
            return new TableAdapterManager
            {
                OrderTableAdapter = new OrderTableAdapter { ClearBeforeFill = false },
                PartSummaryTableAdapter = new PartSummaryTableAdapter { ClearBeforeFill = false },
                SalesOrderTableAdapter = new SalesOrderTableAdapter { ClearBeforeFill = false },
                OrderTemplateTableAdapter = new OrderTemplateTableAdapter { ClearBeforeFill = false },
                OrderCustomFieldsTableAdapter = new OrderCustomFieldsTableAdapter { ClearBeforeFill = false },
                OrderFeesTableAdapter = new OrderFeesTableAdapter { ClearBeforeFill = false },
                Order_MediaTableAdapter = new Order_MediaTableAdapter { ClearBeforeFill = false },
                MediaTableAdapter = new MediaTableAdapter { ClearBeforeFill = false },
                Order_DocumentLinkTableAdapter = new Order_DocumentLinkTableAdapter { ClearBeforeFill = false },
                CustomerCommunicationTableAdapter = new CustomerCommunicationTableAdapter { ClearBeforeFill = false },
                COCTableAdapter = new COCTableAdapter { ClearBeforeFill = false },
                OrderReviewTableAdapter = new OrderReviewTableAdapter { ClearBeforeFill = false },
                OrderProcessesTableAdapter = new OrderProcessesTableAdapter { ClearBeforeFill = false },
                OrderProcessesOperatorTableAdapter = new OrderProcessesOperatorTableAdapter { ClearBeforeFill = false },
                PartInspectionTableAdapter = new PartInspectionTableAdapter { ClearBeforeFill = false },
                OrderProcessAnswerTableAdapter = new OrderProcessAnswerTableAdapter { ClearBeforeFill = false },
                OrderPartMarkTableAdapter = new OrderPartMarkTableAdapter { ClearBeforeFill = false },
                OrderNoteTableAdapter = new OrderNoteTableAdapter { ClearBeforeFill = false },
                OrderHoldTableAdapter = new OrderHoldTableAdapter { ClearBeforeFill = false },
                BatchTableAdapter = new BatchTableAdapter { ClearBeforeFill = false },
                BatchOrderTableAdapter = new BatchOrderTableAdapter { ClearBeforeFill = false },
                BatchCOCTableAdapter = new BatchCOCTableAdapter { ClearBeforeFill = false },
                BatchCOCOrderTableAdapter = new BatchCOCOrderTableAdapter { ClearBeforeFill = false },
                BatchProcessesTableAdapter = new BatchProcessesTableAdapter { ClearBeforeFill = false },
                BatchProcessesOperatorTableAdapter = new BatchProcessesOperatorTableAdapter { ClearBeforeFill = false },
                BatchOperatorTableAdapter = new BatchOperatorTableAdapter {  ClearBeforeFill = false },
                LaborTimeTableAdapter = new LaborTimeTableAdapter { ClearBeforeFill = false },
                BatchOperatorTimeTableAdapter = new BatchOperatorTimeTableAdapter { ClearBeforeFill = false},
                OrderOperatorTableAdapter = new OrderOperatorTableAdapter {  ClearBeforeFill = false },
                OrderOperatorTimeTableAdapter = new OrderOperatorTimeTableAdapter { ClearBeforeFill = false },
                OrderSerialNumberTableAdapter = new OrderSerialNumberTableAdapter { ClearBeforeFill = false },
                OrderProductClassTableAdapter = new OrderProductClassTableAdapter { ClearBeforeFill = false },
                OrderContainersTableAdapter = new OrderContainersTableAdapter { ClearBeforeFill = false },
                OrderContainerItemTableAdapter = new OrderContainerItemTableAdapter { ClearBeforeFill = false },
                OrderShipmentTableAdapter = new OrderShipmentTableAdapter { ClearBeforeFill = false },
                ShipmentPackageTableAdapter = new ShipmentPackageTableAdapter { ClearBeforeFill = false },
                BulkCOCOrderTableAdapter = new BulkCOCOrderTableAdapter { ClearBeforeFill = false },
                BulkCOCTableAdapter = new BulkCOCTableAdapter { ClearBeforeFill = false },
                BillOfLadingOrderTableAdapter = new BillOfLadingOrderTableAdapter { ClearBeforeFill = false },
                BillOfLadingTableAdapter = new BillOfLadingTableAdapter { ClearBeforeFill = false },
                BillOfLadingMediaTableAdapter = new BillOfLadingMediaTableAdapter {  ClearBeforeFill = false },
                BillOfLadingDocumentLinkTableAdapter = new BillOfLadingDocumentLinkTableAdapter { ClearBeforeFill = false },
                OrderChangeTableAdapter = new OrderChangeTableAdapter { ClearBeforeFill = false },
                InternalReworkTableAdapter = new InternalReworkTableAdapter {  ClearBeforeFill = false },
                OrderApprovalTableAdapter = new OrderApprovalTableAdapter {  ClearBeforeFill = false },
                OrderWorkDescriptionTableAdapter = new OrderWorkDescriptionTableAdapter {  ClearBeforeFill = false },
            };
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Dataset.Dispose();
            TableAdapterManager.Dispose();

            _disposed = true;
        }

        #endregion

        #region OptionalDependencies

        [Flags]
        public enum OptionalDependencies
        {
            None = 0,
            Containers = 1,
            Shipments = 2,
            Changes = 4,
            Rework = 8,
            Approvals = 16,
            InfoForSplit = Containers | Shipments
        }

        #endregion
    }
}
