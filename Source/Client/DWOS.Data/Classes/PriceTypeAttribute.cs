using System;

namespace DWOS.Data
{
    /// <summary>
    /// Attribute used to link a <see cref="PriceByType"/> and a
    /// <see cref="PricingStrategy"/> to an <see cref="OrderPrice.enumPriceUnit"/>.
    /// </summary>
    public sealed class PriceTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PriceTypeAttribute"/> class.
        /// </summary>
        /// <param name="pricingType"></param>
        /// <param name="strategy"></param>
        public PriceTypeAttribute(PriceByType pricingType, PricingStrategy strategy)
        {
            PricingType = pricingType;
            Strategy = strategy;
        }

        /// <summary>
        /// Gets the pricing type of this instance.
        /// </summary>
        public PriceByType PricingType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the strategy of this instance.
        /// </summary>
        public PricingStrategy Strategy
        {
            get;
            private set;
        }
    }
}
