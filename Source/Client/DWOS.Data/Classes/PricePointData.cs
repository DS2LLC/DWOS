using DWOS.Data.Datasets;
using DWOS.Data.Order;
using DWOS.Shared.Utilities;
using System;

namespace DWOS.Data
{
    /// <summary>
    /// Represents price point data.
    /// </summary>
    public sealed class PricePointData : IComparable, IComparable<PricePointData>
    {
        #region Fields

        private readonly int _hashCode;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Price Unit of the price point.
        /// </summary>
        public OrderPrice.enumPriceUnit PriceUnit
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the minimum quantity of the price point.
        /// </summary>
        public int? MinQuantity
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the maximum quantity of the price point.
        /// </summary>
        public int? MaxQuantity
        {
            get;
            private set;
        }

        public decimal? MinWeight
        {
            get;
            private set;
        }

        public decimal? MaxWeight
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        private PricePointData(OrderPrice.enumPriceUnit priceUnit)
        {
            PriceUnit = priceUnit;
            _hashCode = (new { priceUnit }.GetHashCode());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PricePointData"/> class.
        /// </summary>
        /// <param name="priceUnit"></param>
        /// <param name="minQuantity"></param>
        /// <param name="maxQuantity"></param>
        private PricePointData(OrderPrice.enumPriceUnit priceUnit, int? minQuantity, int? maxQuantity)
        {
            PriceUnit = priceUnit;
            MinQuantity = minQuantity;
            MaxQuantity = maxQuantity;
            _hashCode = (new { priceUnit, minQuantity, maxQuantity }.GetHashCode());
        }

        private PricePointData(OrderPrice.enumPriceUnit priceUnit, decimal? minWeight, decimal? maxWeight)
        {
            PriceUnit = priceUnit;
            MinWeight = minWeight;
            MaxWeight = maxWeight;
            _hashCode = (new { priceUnit, minWeight, maxWeight }).GetHashCode();
        }

        public static PricePointData Blank(OrderPrice.enumPriceUnit priceUnit) => new PricePointData(priceUnit);

        public static PricePointData From(PartsDataset.PartProcessVolumePriceRow priceRow)
        {
            if (priceRow == null)
            {
                return null;
            }

            var priceUnit = OrderPrice.ParsePriceUnit(priceRow.PriceUnit);
            switch (OrderPrice.GetPriceByType(priceUnit))
            {
                case PriceByType.Quantity:
                    int? minQuantity = null;
                    if (!priceRow.IsMinValueNull())
                    {
                        NullableParser.TryParse(priceRow.MinValue, out minQuantity);
                    }

                    int? maxQuantity = null;
                    if (!priceRow.IsMaxValueNull())
                    {
                        NullableParser.TryParse(priceRow.MaxValue, out maxQuantity);
                    }

                    return new PricePointData(priceUnit, minQuantity, maxQuantity);
                case PriceByType.Weight:
                    decimal? minWeight = null;
                    if (!priceRow.IsMinValueNull())
                    {
                        NullableParser.TryParse(priceRow.MinValue, out  minWeight);
                    }

                    decimal? maxWeight = null;
                    if (!priceRow.IsMaxValueNull())
                    {
                        NullableParser.TryParse(priceRow.MaxValue, out maxWeight);
                    }

                    return new PricePointData(priceUnit, minWeight, maxWeight);
                default:
                    return null;
            }
        }

        public static PricePointData From(OrdersDataSet.CustomerPricePointDetailRow priceRow)
        {
            if (priceRow == null)
            {
                return null;
            }

            var priceUnit = OrderPrice.ParsePriceUnit(priceRow.PriceUnit);
            switch (OrderPrice.GetPriceByType(priceUnit))
            {
                case PriceByType.Quantity:
                    int.TryParse(priceRow.MinValue, out var minQuantity);

                    int? maxQuantity = null;
                    if (!priceRow.IsMaxValueNull())
                    {
                        NullableParser.TryParse(priceRow.MaxValue, out maxQuantity);
                    }

                    return new PricePointData(priceUnit, minQuantity, maxQuantity);
                case PriceByType.Weight:

                    decimal.TryParse(priceRow.MinValue, out var minWeight);

                    decimal? maxWeight = null;
                    if (!priceRow.IsMaxValueNull())
                    {
                        NullableParser.TryParse(priceRow.MaxValue, out maxWeight);
                    }

                    return new PricePointData(priceUnit, minWeight, maxWeight);
                default:
                    return null;
            }
        }

        public static PricePointData From(OrdersDataSet.PricePointDetailRow priceRow)
        {
            if (priceRow == null)
            {
                return null;
            }

            var priceUnit = OrderPrice.ParsePriceUnit(priceRow.PriceUnit);
            switch (OrderPrice.GetPriceByType(priceUnit))
            {
                case PriceByType.Quantity:
                    int.TryParse(priceRow.MinValue, out var minQuantity);

                    int? maxQuantity = null;
                    if (!priceRow.IsMaxValueNull())
                    {
                        NullableParser.TryParse(priceRow.MaxValue, out maxQuantity);
                    }

                    return new PricePointData(priceUnit, minQuantity, maxQuantity);
                case PriceByType.Weight:

                    decimal.TryParse(priceRow.MinValue, out var minWeight);

                    decimal? maxWeight = null;
                    if (!priceRow.IsMaxValueNull())
                    {
                        NullableParser.TryParse(priceRow.MaxValue, out maxWeight);
                    }

                    return new PricePointData(priceUnit, minWeight, maxWeight);
                default:
                    return null;
            }
        }

        public static PricePointData From(QuoteDataSet.QuotePartProcessPriceRow priceRow)
        {
            if (priceRow == null)
            {
                return null;
            }

            var priceUnit = OrderPrice.ParsePriceUnit(priceRow.PriceUnit);
            switch (OrderPrice.GetPriceByType(priceUnit))
            {
                case PriceByType.Quantity:
                    int? minQuantity = null;
                    if (!priceRow.IsMinValueNull())
                    {
                        NullableParser.TryParse(priceRow.MinValue, out minQuantity);
                    }

                    int? maxQuantity = null;
                    if (!priceRow.IsMaxValueNull())
                    {
                        NullableParser.TryParse(priceRow.MaxValue, out maxQuantity);
                    }

                    return new PricePointData(priceUnit, minQuantity, maxQuantity);
                case PriceByType.Weight:
                    decimal? minWeight = null;

                    if (!priceRow.IsMinValueNull())
                    {
                        NullableParser.TryParse(priceRow.MinValue, out minWeight);
                    }

                    decimal? maxWeight = null;
                    if (!priceRow.IsMaxValueNull())
                    {
                        NullableParser.TryParse(priceRow.MaxValue, out maxWeight);
                    }

                    return new PricePointData(priceUnit, minWeight, maxWeight);
                default:
                    return null;
            }
        }


        public static PricePointData From(PriceUnitData priceRow)
        {
            if (priceRow == null)
            {
                return null;
            }

            var priceUnit = priceRow.PriceUnit;
            switch (OrderPrice.GetPriceByType(priceUnit))
            {
                case PriceByType.Quantity:
                    var minQuantity = priceRow.MinQuantity;
                    var maxQuantity = priceRow.MaxQuantity == PriceUnitData.MAX_QUANTITY
                        ? (int?)null
                        : priceRow.MaxQuantity;

                    return new PricePointData(priceUnit, minQuantity, maxQuantity);
                case PriceByType.Weight:
                    var minWeight = priceRow.MinWeight;
                    var maxWeight = priceRow.MaxWeight == PriceUnitData.MAX_WEIGHT
                        ? null
                        : priceRow.MaxWeight;

                    return new PricePointData(priceUnit, minWeight, maxWeight);
                default:
                    return null;
            }
        }

        public bool Matches(PartsDataset.PartProcessVolumePriceRow priceRow)
        {
            if (priceRow == null)
            {
                throw new ArgumentNullException(nameof(priceRow), "Cannot be null.");
            }

            return Equals(From(priceRow));
        }

        public bool Matches(QuoteDataSet.QuotePartProcessPriceRow priceRow)
        {
            if (priceRow == null)
            {
                throw new ArgumentNullException(nameof(priceRow));
            }

            return Equals(From(priceRow));
        }

        public bool Matches(int quantity, decimal weight)
        {
            switch (OrderPrice.GetPriceByType(PriceUnit))
            {
                case PriceByType.Quantity:
                    var minQuantity = MinQuantity ?? 0;
                    var maxQuantity = MaxQuantity ?? int.MaxValue;
                    return quantity >= minQuantity && quantity <= maxQuantity;
                case PriceByType.Weight:
                    var minWeight = MinWeight ?? 0;
                    var maxWeight = MaxWeight ?? decimal.MaxValue;
                    return weight >= minWeight && weight <= maxWeight;
                default:
                    return false;
            }
        }

        public override bool Equals(object obj)
        {
            var otherData = obj as PricePointData;

            return otherData != null &&
                PriceUnit == otherData.PriceUnit &&
                MinQuantity == otherData.MinQuantity &&
                MaxQuantity == otherData.MaxQuantity &&
                MinWeight == otherData.MinWeight &&
                MaxWeight == otherData.MaxWeight;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return string.Format("{{PriceUnit: {0}, MinQuantity: {1}, MaxQuantity: {2}, MinWeight: {3}, MaxWeight: {4}}}",
                PriceUnit,
                MinQuantity.HasValue ? MinQuantity.Value.ToString() : "null",
                MaxQuantity.HasValue ? MaxQuantity.Value.ToString() : "null",
                MinWeight.HasValue ? MinWeight.Value.ToString() : "null",
                MaxWeight.HasValue ? MaxWeight.Value.ToString() : "null");
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return CompareTo(obj as PricePointData);
        }

        #endregion

        #region IComparable<PricePointData> Members

        public int CompareTo(PricePointData other)
        {
            if (other == null)
            {
                return 1;
            }

            var xType = OrderPrice.GetPriceByType(PriceUnit);
            var yType = OrderPrice.GetPriceByType(other.PriceUnit);

            if (xType != yType)
            {
                // Quantity-based price points need to be first
                return xType == PriceByType.Quantity ? -1 : 1;
            }

            switch (xType)
            {
                case PriceByType.Quantity:
                    return CompareToQuantity(other);
                case PriceByType.Weight:
                    return CompareToWeight(other);
                default:
                    return 0;
            }
        }

        private int CompareToQuantity(PricePointData other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            int xMin = MinQuantity ?? 0;
            int yMin = other.MinQuantity ?? 0;

            int xMax = MaxQuantity ?? 0;
            int yMax = other.MaxQuantity ?? 0;

            int minDiff = xMin - yMin;
            int maxDiff = xMax - yMax;

            if (minDiff == 0 && maxDiff == 0)
            {
                if (PriceUnit == other.PriceUnit)
                {
                    return 0;
                }
                // Each needs to be first
                return (PriceUnit == OrderPrice.enumPriceUnit.Each) ? -1 : 1;
            }
            if (minDiff != 0)
            {
                return minDiff;
            }

            return maxDiff;
        }

        private int CompareToWeight(PricePointData other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var xMin = MinWeight ?? 0;
            var yMin = other.MinWeight ?? 0;

            var xMax = MaxWeight ?? 0;
            var yMax = other.MaxWeight ?? 0;

            var minDiff = xMin - yMin;
            var maxDiff = xMax - yMax;

            if (minDiff == 0 && maxDiff == 0)
            {
                if (PriceUnit == other.PriceUnit)
                {
                    return 0;
                }

                // Each needs to be first
                return (PriceUnit == OrderPrice.enumPriceUnit.EachByWeight) ? -1 : 1;
            }

            if (minDiff != 0)
            {
                return minDiff > 0 ? 1 : 0;
            }

            return maxDiff > 0 ? 1 : 0;
        }

        #endregion
    }
}
