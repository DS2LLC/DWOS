namespace DWOS.Data.Order
{
    /// <summary>
    /// Represents price unit data as it relates to pricing.
    /// </summary>
    /// <remarks>
    /// This is commonly used as an abstraction for the <c>d_PriceUnit</c>,
    /// <c>PricePoint</c>, and <c>PricePointDetail</c> tables to provide
    /// complete information about a price unit and its default pricing info.
    /// </remarks>
    public sealed class PriceUnitData
    {
        #region Fields

        /// <summary>
        /// The value for <see cref="MaxQuantity"/> when the database does
        /// not specify an upper bound.
        /// </summary>
        public const int MAX_QUANTITY = 999999;

        /// <summary>
        /// The value for <see cref="MaxQuantity"/> when the database does
        /// not specify an upper bound.
        /// </summary>
        public const decimal MAX_WEIGHT = 999999.99M;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the price unit for this instance.
        /// </summary>
        public OrderPrice.enumPriceUnit PriceUnit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the display name for this instance.
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum quantity for this instance.
        /// </summary>
        public int MinQuantity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum quantity for this instance.
        /// </summary>
        public int MaxQuantity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value that determines if a price unit covers the maximum
        /// allowed quantity.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance covers the maximum allowed quantity;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool CoversMaximumQuantity
        {
            get
            {
                return MaxQuantity == MAX_QUANTITY;
            }
        }

        /// <summary>
        /// Gets or sets the minimum weight for this instance.
        /// </summary>
        public decimal? MinWeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum weight for this instance.
        /// </summary>
        public decimal? MaxWeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating that the price unit for this
        /// instance should be shown to users.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool Active
        {
            get;
            set;
        }

        #endregion
    }
}
