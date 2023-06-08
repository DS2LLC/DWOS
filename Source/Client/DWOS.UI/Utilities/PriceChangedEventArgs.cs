using DWOS.Data;
using System;

namespace DWOS.UI.Utilities
{
    public sealed class PriceChangedEventArgs : EventArgs
    {
        #region Properties

        public PricingStrategy PricingStrategy
        {
            get;
        }

        public decimal NewAmount
        {
            get;
        }

        #endregion

        #region Methods

        public PriceChangedEventArgs(PricingStrategy priceStrategy, decimal amount)
        {
            PricingStrategy = priceStrategy;
            NewAmount = amount;
        }

        #endregion
    }
}
