namespace DWOS.Data
{
    /// <summary>
    /// Represent a field token for exporting an invoice
    /// </summary>
    public class InvoiceToken
    {
        #region Properties

        /// <summary>
        /// Gets or sets the token that this instance represents.
        /// </summary>
        public InvoiceFieldMapper.enumTokens TokenName { get; set; }

        /// <summary>
        /// Gets or sets the header text for this instance.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>
        /// </value>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the order of this instance in relation to other
        /// <see cref="InvoiceToken"/> instances.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the value for this instance.
        /// </summary>
        public string Value { get; set; }

        #endregion
    }


}
