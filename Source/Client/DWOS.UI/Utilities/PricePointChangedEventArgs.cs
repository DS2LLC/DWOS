using DWOS.Data.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.UI.Utilities
{
    public sealed class PricePointChangedEventArgs : EventArgs
    {
        #region Properties

        public PricePoint PricePoint
        {
            get;
            private set;
        }

        public decimal NewAmount
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public PricePointChangedEventArgs(PricePoint pricePoint, decimal newAmount)
        {
            PricePoint = pricePoint;
            NewAmount = newAmount;
        }

        #endregion
    }
}
