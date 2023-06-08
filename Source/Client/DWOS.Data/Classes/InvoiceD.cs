using System.Collections.Generic;

namespace DWOS.Data
{
    /// <summary>
    /// Represent a DWOS invoice
    /// </summary>
    /// <remarks>
    /// Some properties for this class are <see cref="string"/> values for
    /// convenient use with string-building methods.
    /// </remarks>
    public class InvoiceD
    {
        #region Fields

        private List<InvoiceFeeItem> _feeItems = null;
        private List<InvoiceLineItem> _lineItems = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets fee items for this instance.
        /// </summary>
        public List<InvoiceFeeItem> FeeItems
        {
            get
            {
                if (_feeItems == null)
                    _feeItems = new List<InvoiceFeeItem>();

                return _feeItems;
            }

            set { _feeItems = value; }
        }

        /// <summary>
        /// Gets or sets line items for this instance.
        /// </summary>
        public List<InvoiceLineItem> LineItems
        {
            get
            {
                if (_lineItems == null)
                {
                    _lineItems = new List<InvoiceLineItem>();
                }

                return _lineItems;
            }
            set
            {
                _lineItems = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if this instance was successfully
        /// exported.
        /// </summary>
        /// <value>
        /// <c>true</c> if this was successfully exported; otherwise, <c>false</c>.
        /// </value>
        public bool Exported { get; set; }

        /// <summary>
        /// Gets or sets the Order ID from this instance.
        /// </summary>
        /// <remarks>
        /// Also used as the value for <see cref="WO"/>.
        /// </remarks>
        public string OrderID { get; set; }

        /// <summary>
        /// Gets or sets the Work Order for this instance.
        /// </summary>
        public string WO { get; set; }

        /// <summary>
        /// Gets or sets the Purchase Order for this instance.
        /// </summary>
        public string PO { get; set; }

        /// <summary>
        /// Gets or sets the customer ID for this instance.
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// Gets or sets the customer name for this instance.
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets the invoice ID for this instance.
        /// </summary>
        public string InvoiceID { get; set; }

        /// <summary>
        /// Gets or sets the order creation date for this instance.
        /// </summary>
        public string DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the calculated order due date for this instance.
        /// </summary>
        public string DueDate { get; set; }

        /// <summary>
        /// Gets or sets the shipping date for this instance.
        /// </summary>
        public string ShipDate { get; set; }

        /// <summary>
        /// Gets or sets the payment terms for this instance.
        /// </summary>
        public string Terms { get; set; }

        /// <summary>
        /// Gets or sets the AR account for this instance.
        /// </summary>
        /// <remarks>
        /// This value was added specifically for DPS.
        /// </remarks>
        public string ARAccount { get; set; }

        /// <summary>
        /// Gets or sets the invoice item name for this instance.
        /// </summary>
        public string Item { get; set; }

        /// <summary>
        /// Gets or sets the part name for this instance.
        /// </summary>
        public string PartName { get; set; }

        /// <summary>
        /// Gets or sets the quantity for this instance.
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        /// Gets or sets the price unit for this instance.
        /// </summary>
        public string PriceUnit { get; set; }

        /// <summary>
        /// Gets or sets the shipping carrier ID for this instance.
        /// </summary>
        public string Shipping { get; set; }

        /// <summary>
        /// Gets or sets the tracking number for this instance.
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating issues encountered when exporting
        /// this instance.
        /// </summary>
        /// 
        public string Issues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the name of the Part
        /// this instance.
        /// </summary>
        public string PartDesc { get; set; }

        /// <summary>
        /// Gets or sets a value for all process codes on a part
        /// this instance.
        /// </summary>
        public string Processes { get; set; }

        /// <summary>
        /// Gets or sets a value for all process aliases on an order
        /// this instance.
        /// </summary>
        public string ProcessAliases { get; set; }

        /// <summary>
        /// Gets or sets a value for all process Descriptions of an order
        /// this instance.
        /// </summary>
        public string ProcessDesc { get; set; }
        #endregion
    }
}
