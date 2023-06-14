using System;

namespace DWOS.AutomatedWorkOrderTool.Model
{
    // Assumptions:
    // "PO" is short for "purchase order"
    // "Nbr" is short for "number"
    // "SrcCod" is short for "source code"
    public class ShippingManifestOrder
    {
        public string Priority { get; set; }

        public string KacShipper { get; set; }

        public string PurchaseOrder { get; set; }

        public string PurchaseOrderItem { get; set; }

        public string WorkOrder { get; set; }

        public string Part { get; set; }

        public string Project { get; set; }

        public DateTime? DueDate { get; set; }

        public int? Quantity { get; set; }

        public string LotCost { get; set; }

        public string InvoiceNumber { get; set; }

        public string VendorPackslip { get; set; }

        public string PurchasingInvoiceApproval { get; set; }

        public string VendorNumber { get; set; }

        public string SourceCode { get; set; }

        public OrderStatus Status { get; set; }

        public string ImportNotes { get; set; }

        public int? ExistingOrderId { get; set; }

        #region OrderStatus

        public enum OrderStatus
        {
            New,
            NewWithoutExistingOrders,
            Existing,
            Invalid
        }
        #endregion
    }
}
