namespace DWOS.Data.Order.Activity
{
    /// <summary>
    /// Base class for activity results.
    /// </summary>
    public class ActivityResults
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Order ID for this instance.
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// Gets or sets the order's next work status for this instance.
        /// </summary>
        public string WorkStatus { get; set; }

        #endregion
    }
}
