namespace DWOS.Data
{
    /// <summary>
    /// Represent a DWOS invoice fee item
    /// </summary>
    /// <remarks>
    /// Some properties for this class are <see cref="string"/> values for
    /// convenient use with string-building methods.
    /// </remarks>
    public class InvoiceFeeItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets the line item number for this instance.
        /// </summary>
        public string LineItemNum { get; set; }

        /// <summary>
        /// Gets or sets the order fee type ID for this instance.
        /// </summary>
        public string Item { get; set; }

        /// <summary>
        /// Gets or sets the sales account for this instance.
        /// </summary>
        public string SalesAccount { get; set; }

        /// <summary>
        /// Gets or sets the order fee type for this instance.
        /// </summary>
        /// <remarks>
        /// In a special case, this can also be the order's priority.
        /// </remarks>
        public string OrderFeeType { get; set; }

        /// <summary>
        /// Gets or sets the fee type for this instance.
        /// </summary>
        public string FeeType { get; set; }

        /// <summary>
        /// Gets the description for this instance.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the quantity for this instance.
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        /// Gets or sets the total fee amount for this instance.
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// Gets or sets the charge amount for this instance.
        /// </summary>
        public string ExtAmount { get; set; }

        #endregion
    }
}
