using DWOS.Data.Datasets;
using System;

namespace DWOS.Data.Order
{
    /// <summary>
    ///  Represents a price point.
    /// </summary>
    public struct PricePoint : IEquatable<PricePoint>
    {
        #region Properties

        /// <summary>
        /// Gets the minimum quantity for this instance.
        /// </summary>
        public int? MinQuantity
        {
            get;
        }

        /// <summary>
        /// Gets the maximum quantity for this instance.
        /// </summary>
        public int? MaxQuantity
        {
            get;
        }

        /// <summary>
        /// Gets the minimum weight for this instance.
        /// </summary>
        public decimal? MinWeight
        {
            get;
        }

        /// <summary>
        /// Gets the maximum weight for this instance.
        /// </summary>
        public decimal? MaxWeight
        {
            get;
        }

        /// <summary>
        /// Gets the price unit for this instance.
        /// </summary>
        public OrderPrice.enumPriceUnit PriceUnit
        {
            get;
        }

        /// <summary>
        /// Gets the price-by type for this instance.
        /// </summary>
        public PriceByType PriceByType =>
            OrderPrice.GetPriceByType(PriceUnit);

        #endregion

        #region Methods

        private PricePoint(int? minQuantity, int? maxQuantity, OrderPrice.enumPriceUnit priceUnit)
        {
            MinQuantity = minQuantity;
            MaxQuantity = maxQuantity;
            MinWeight = null;
            MaxWeight = null;
            PriceUnit = priceUnit;
        }

        private PricePoint(decimal? minWeight, decimal? maxWeight, OrderPrice.enumPriceUnit priceUnit)
        {
            MinWeight = minWeight;
            MaxWeight = maxWeight;
            MinQuantity = null;
            MaxQuantity = null;
            PriceUnit = priceUnit;
        }

        /// <summary>
        /// Creates a new <see cref="PricePoint"/> instance based on two quantities.
        /// </summary>
        /// <param name="minQuantity"></param>
        /// <param name="maxQuantity"></param>
        /// <param name="priceUnit"></param>
        /// <returns></returns>
        public static PricePoint FromQuantities(int? minQuantity, int? maxQuantity, OrderPrice.enumPriceUnit priceUnit) =>
            new PricePoint(minQuantity, maxQuantity, priceUnit);

        /// <summary>
        /// Does this instance match a quantity and price unit?
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="priceUnit"></param>
        /// <param name="priceUnit"></param>
        /// <returns>
        /// <c>true</c> if this instance matches; otherwise, <c>false</c>.
        /// </returns>
        public bool Matches(int quantity, decimal weight, OrderPrice.enumPriceUnit priceUnit)
        {
            if (PriceUnit != priceUnit)
            {
                return false;
            }

            switch (OrderPrice.GetPriceByType(priceUnit))
            {
                case PriceByType.Quantity:
                    return MatchesQuantity(quantity);
                case PriceByType.Weight:
                    return MatchesWeight(weight);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Does this instance match a quantity?
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns>
        /// <c>true</c> if this instance matches; otherwise, <c>false</c>.
        /// </returns>
        public bool MatchesQuantity(int quantity)
        {
            if (PriceByType != PriceByType.Quantity)
            {
                // Is not a quantity-based price point
                return false;
            }

            if (!MinQuantity.HasValue)
            {
                // price unit does not specify qty.
                return true;
            }

            // The 'partQty == 0' check allows price to update if
            // the user hasn't entered quantity yet.
            var minQuantity = MinQuantity ?? 0;
            var maxQuantity = MaxQuantity ?? int.MaxValue;

            return (quantity >= minQuantity && quantity <= maxQuantity) ||
                (quantity == 0 && minQuantity == 1);
        }

        /// <summary>
        /// Does this instance match a weight?
        /// </summary>
        /// <param name="weight"></param>
        /// <returns>
        /// <c>true</c> if this instance matches; otherwise, <c>false</c>.
        /// </returns>
        public bool MatchesWeight(decimal weight)
        {
            if (PriceByType != PriceByType.Weight)
            {
                // Is not a weight-based price point
                return false;
            }

            if (!MinWeight.HasValue)
            {
                // price unit does not specify qty.
                return true;
            }

            var minWeight = MinWeight ?? 0;
            var maxWeight = MaxWeight ?? decimal.MaxValue;

            return (weight >= minWeight && weight <= maxWeight);
        }

        /// <summary>
        /// Creates a new <see cref="PricePoint"/> instance from a
        /// <see cref="OrdersDataSet.PartProcessPriceSummaryRow"/> instance.
        /// </summary>
        /// <param name="row"></param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="row"/> is null.
        /// </exception>
        /// <returns></returns>
        public static PricePoint From(OrdersDataSet.PartProcessPriceSummaryRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            var priceUnit = OrderPrice.ParsePriceUnit(row.PriceUnit);

            switch (OrderPrice.GetPriceByType(priceUnit))
            {
                case PriceByType.Quantity:
                    var minQuantity = row.IsMinValueNull() ? (int?)null : int.Parse(row.MinValue);
                    var maxQuantity = row.IsMaxValueNull() ? (int?)null : int.Parse(row.MaxValue);
                    return new PricePoint(minQuantity, maxQuantity, priceUnit);
                case PriceByType.Weight:
                    var minWeight = row.IsMinValueNull() ? (decimal?)null : decimal.Parse(row.MinValue);
                    var maxWeight = row.IsMaxValueNull() ? (decimal?)null : decimal.Parse(row.MaxValue);
                    return new PricePoint(minWeight, maxWeight, priceUnit);
                default:
                    return new PricePoint();
            }
        }

        public override bool Equals(object obj)
        {
            var objAsPricePoint = obj as PricePoint?;

            if (!objAsPricePoint.HasValue)
            {
                return false;
            }

            return Equals(objAsPricePoint.Value);
        }

        public override int GetHashCode()
        {
            return (new { MinQuantity, MaxQuantity, MinWeight, MaxWeight, PriceUnit }.GetHashCode());
        }

        public static bool operator ==(PricePoint left, PricePoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PricePoint left, PricePoint right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region IEquatable<PricePoint> Members

        public bool Equals(PricePoint other)
        {
            return MinQuantity == other.MinQuantity &&
                MaxQuantity == other.MaxQuantity &&
                MinWeight == other.MinWeight &&
                MaxWeight == other.MaxWeight &&
                PriceUnit == other.PriceUnit;
        }

        #endregion
    }
}
