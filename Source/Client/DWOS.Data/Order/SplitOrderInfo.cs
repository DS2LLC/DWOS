namespace DWOS.Data.Order
{
    /// <summary>
    /// Represents split information for a Work Order.
    /// </summary>
    public class SplitOrderInfo
    {
        #region Fields

        /// <summary>
        /// Gets or sets the identifier for this instance.
        /// </summary>
        /// <remarks>
        /// This may be the original WO number or "New" if it represents an
        /// order to be created.
        /// </remarks>
        public string Order { get; set; }

        /// <summary>
        /// Gets or sets the quantity for this instance.
        /// </summary>
        public int PartQty { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if this instance represents
        /// the original Work Order in a split.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance represents the original Work Order;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool IsOriginalOrder { get; set; }

        #endregion
    }
}
