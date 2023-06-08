namespace DWOS.Data
{
    /// <summary>
    /// Represent a DWOS invoice fee item
    /// </summary>
    /// <remarks>
    /// Some properties for this class are <see cref="string"/> values for
    /// convenient use with string-building methods.
    /// </remarks>
    public class InvoiceLineItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets the sales account for this instance.
        /// </summary>
        public string SalesAccount { get; set; }

        /// <summary>
        /// Gets or sets the description for this instance.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the base price for this instance.
        /// </summary>
        public string BasePrice { get; set; }

        /// <summary>
        /// Gets or sets the total price for this instance.
        /// </summary>
        public string ExtPrice { get; set; }

        /// <summary>
        /// Gets the line item number for this instance.
        /// </summary>
        public string LineItem { get; set; }

        #endregion
    }
}
