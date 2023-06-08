using System;
using System.Diagnostics;
using System.Linq;
using System.ComponentModel;
using DWOS.Data.Datasets;
using DWOS.Shared.Utilities;
using NLog;
using System.Collections.Generic;

namespace DWOS.Data
{
    /// <summary>
    /// Defines utility methods related to order pricing.
    /// </summary>
    public static class OrderPrice
    {
        #region Properties

        /// <summary>
        /// Gets a currency format string using the current value for
        /// <see cref="ApplicationSettings.PriceDecimalPlaces"/>.
        /// </summary>
        public static string CurrencyFormatString
        {
            get { return "C" + ApplicationSettings.Current.PriceDecimalPlaces; }
        }

        public static IEnumerable<enumPriceUnit> AllPriceUnits =>
            Enum.GetValues(typeof(enumPriceUnit)).Cast<enumPriceUnit>();

        #endregion

        #region Methods

        /// <summary>
        /// Is the given price unit for lot pricing?
        /// </summary>
        /// <param name="priceUnit"></param>
        /// <returns>
        /// <c>true</c> if the price unit is for lot price; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPriceUnitLot(enumPriceUnit priceUnit)
        {
            return GetPricingStrategy(priceUnit) == PricingStrategy.Lot;
        }

        /// <summary>
        /// Is the given price unit for lot pricing?
        /// </summary>
        /// <param name="priceUnit"></param>
        /// <returns>
        /// <c>true</c> if the price unit is for lot price; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPriceUnitLot(string priceUnit)
        {
            return IsPriceUnitLot(ParsePriceUnit(priceUnit));
        }

        /// <summary>
        /// Calculates an order's total price.
        /// </summary>
        /// <param name="basePrice"></param>
        /// <param name="priceUnit"></param>
        /// <param name="fees"></param>
        /// <param name="quantity"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static decimal CalculatePrice(decimal basePrice, string priceUnit, decimal fees, int quantity, decimal weight)
        {
            switch (priceUnit)
            {
                case "Each":
                    return (basePrice * Convert.ToDecimal(quantity)) + fees;
                case "EachByWeight":
                    return (basePrice * weight) + fees;
                case "Lot":
                case "LotByWeight":
                    return basePrice + fees;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Calculates order fees.
        /// </summary>
        /// <remarks>
        /// This method assumes that <paramref name="orderRow"/> has all of
        /// its fees loaded.
        /// </remarks>
        /// <param name="orderRow"></param>
        /// <param name="basePrice"></param>
        /// <returns></returns>
        public static decimal CalculateFees(OrdersDataSet.OrderRow orderRow, decimal basePrice)
        {
            decimal fees = 0;
            var or = orderRow.GetOrderFeesRows();

            //Fee can be either fixed or percentage
            foreach (var order in or)
            {
                decimal weight = orderRow.IsWeightNull() ? 0M : orderRow.Weight;
                var partQuantity = orderRow.IsPartQuantityNull() ? 0 : orderRow.PartQuantity;

                if (order.OrderFeeTypeRow != null)
                {
                    fees += CalculateFees(order.OrderFeeTypeRow.FeeType, order.Charge, basePrice, partQuantity==0?1:partQuantity, orderRow.PriceUnit, weight);
                }
                else
                {
                    Debug.Assert(true, "Should not be in here. The Order Fee rows for this order has not been loaded.");
                    fees += order.Charge;  //Default as a fixed type charge
                }
            }

            return fees;
        }

        /// <summary>
        /// Calculates order fees.
        /// </summary>
        /// <remarks>
        /// This method assumes that <paramref name="orderRow"/> has all of
        /// its fees loaded.
        /// </remarks>
        /// <param name="orderRow"></param>
        /// <param name="basePrice"></param>
        /// <returns></returns>
        public static decimal CalculateFees(Reports.OrdersReport.OrderRow orderRow, decimal basePrice)
        {
            decimal fees = 0;
            var or = orderRow.GetOrderFeesRows();
            //Fee can be either fixed or percentage
            foreach (var order in or)
            {
                decimal weight = orderRow.IsWeightNull() ? 0M : orderRow.Weight;

                if (order.OrderFeeTypeRow != null)
                    fees += CalculateFees(order.OrderFeeTypeRow.FeeType, order.Charge, basePrice, orderRow.PartQuantity, orderRow.PriceUnit, weight);
                else
                {
                    Debug.Assert(true, "Should not be in here. The Order Fee rows for this order has not been loaded.");
                    fees += order.Charge;  //Default as a fixed type charge
                }
            }

            return fees;
        }

        /// <summary>
        /// Calculates order fees.
        /// </summary>
        /// <remarks>
        /// This method assumes that <paramref name="orderRow"/> has all of
        /// its fees loaded.
        /// </remarks>
        /// <param name="orderRow"></param>
        /// <param name="basePrice"></param>
        /// <returns></returns>
        public static decimal CalculateFees(OrderInvoiceDataSet.OrderInvoiceRow orderRow, decimal basePrice)
        {
            decimal fees = 0;
            var or = orderRow.GetOrderFeesRows();
            //Fee can be either fixed or percentage
            foreach (var order in or)
            {
                decimal weight = orderRow.IsWeightNull() ? 0M : orderRow.Weight;

                if (order.OrderFeeTypeRow != null)
                    fees += CalculateFees(order.OrderFeeTypeRow.FeeType, order.Charge, basePrice, orderRow.PartQuantity, orderRow.PriceUnit, weight);
                else
                {
                    Debug.Assert(true, "Should not be in here. The Order Fee rows for this order has not been loaded.");
                    fees += order.Charge;  //Default as a fixed type charge
                }
            }

            return fees;
        }

        /// <summary>
        /// Calculates order fees.
        /// </summary>
        /// <remarks>
        /// This method assumes that <paramref name="partHistoryRow"/> has all of
        /// its fees loaded.
        /// </remarks>
        /// <param name="partHistoryRow"></param>
        /// <param name="basePrice"></param>
        /// <returns></returns>
        public static decimal CalculateFees(DWOS.Data.Reports.ProcessPartsReport.PartHistoryRow partHistoryRow, decimal basePrice)
        {
            decimal fees = 0;
            var pr = partHistoryRow.GetOrderFeesRows();
            foreach (var fee in pr)
            {
                decimal weight = partHistoryRow.IsWeightNull() ? 0M : partHistoryRow.Weight;

                if (fee.OrderFeeTypeRow != null)
                {
                    fees += CalculateFees(fee.OrderFeeTypeRow.FeeType, fee.Charge, basePrice, partHistoryRow.PartQuantity, partHistoryRow.PriceUnit, weight);
                }
                else
                {
                    Debug.Assert(true, "Should not be in here. The Order Fee rows for this part history row have not been loaded.");
                    fees += fee.Charge; // Default as a fixed type charge
                }
            }

            return fees;
        }

        /// <summary>
        /// Calculates the total for an order fee.
        /// </summary>
        /// <param name="feeType"></param>
        /// <param name="charge"></param>
        /// <param name="basePrice"></param>
        /// <param name="partQuantity"></param>
        /// <param name="priceUnit"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static decimal CalculateFees(string feeType, decimal charge, decimal basePrice, int partQuantity, string priceUnit, decimal weight)
        {
            const int PERCENT = 100;
            DWOS.Data.OrderPrice.enumFeeType enumfeeType;

            if (!Enum.TryParse<DWOS.Data.OrderPrice.enumFeeType>(feeType, out enumfeeType))
            {
                //Default as a fixed type charge
                return charge;
            }
            if (partQuantity == 0)
                return 0;
            switch (enumfeeType)
            {
                case OrderPrice.enumFeeType.Percentage:
                    switch (priceUnit)
                    {
                        case "Each":
                            return (basePrice * partQuantity) * (charge / PERCENT);
                        case "EachByWeight":
                            return (basePrice * weight) * (charge / PERCENT);
                        case "Lot":
                        case "LotByWeight":
                            return basePrice * (charge / PERCENT);
                        default:
                            return charge;
                    }
                case OrderPrice.enumFeeType.Fixed:
                default:
                    return charge;
            }
        }

        /// <summary>
        /// Converts the user-facing string representation of a price unit to
        /// an equivalent enumerated object.
        /// </summary>
        /// <remarks>
        /// This method differs from the equivalent call to Enum.Parse(...)
        /// because this processes user-facing strings that can differ
        /// from enumerated object names.
        ///
        /// The parsing operation is always case-insensitive.
        /// </remarks>
        /// <param name="displayString">A string displayed to end users.</param>
        /// <returns></returns>
        public static enumPriceUnit ParsePriceUnit(string displayString)
        {
            const enumPriceUnit defaultValue = enumPriceUnit.Each;

            if (string.IsNullOrEmpty(displayString))
            {
                return defaultValue;
            }
            switch (displayString.ToUpper().Replace(" ", string.Empty))
            {
                case "EACH":
                    return enumPriceUnit.Each;
                case "LOT":
                    return enumPriceUnit.Lot;
                case "EACHBYWEIGHT":
                    return enumPriceUnit.EachByWeight;
                case "LOTBYWEIGHT":
                    return enumPriceUnit.LotByWeight;
                default:
                    return defaultValue;
            }
        }

        /// <summary>
        /// Calculates an 'each' price given an order's total prior to fees, the
        /// price unit, part quantity, and weight.
        /// </summary>
        /// <param name="orderTotalBeforeFees">An order's total, prior to fees.</param>
        /// <param name="priceUnit">The price unit of the order.</param>
        /// <param name="quantity">The part quantity of the order.</param>
        /// <param name="weight">The order's weight.</param>
        /// <returns></returns>
        public static decimal CalculateEachPrice(decimal basePrice, string priceUnit, int quantity, decimal weight)
        {
            switch(ParsePriceUnit(priceUnit))
            {
                case enumPriceUnit.Each:
                case enumPriceUnit.EachByWeight:
                    return basePrice;
                case enumPriceUnit.Lot:
                    if (quantity == 0)
                    {
                        return 0M;
                    }
                    else
                    {
                        return basePrice / quantity;
                    }
                case enumPriceUnit.LotByWeight:
                    if (weight == 0M)
                    {
                        return 0M;
                    }
                    else
                    {
                        return basePrice / weight;
                    }
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Determines what part price (if any) to use for an order's base price.
        /// </summary>
        /// <param name="part">A part specifying pricing for different price units.</param>
        /// <param name="priceUnit">The price unit of the order.</param>
        /// <returns></returns>
        public static decimal? DetermineBasePrice(OrdersDataSet.PartSummaryRow part, enumPriceUnit priceUnit)
        {
            switch (priceUnit)
            {
                case enumPriceUnit.Each:
                case enumPriceUnit.EachByWeight:
                    if (part != null && !part.IsEachPriceNull())
                    {
                        return part.EachPrice;
                    }

                    break;
                case enumPriceUnit.Lot:
                case enumPriceUnit.LotByWeight:
                    if (part != null && !part.IsLotPriceNull())
                    {
                        return part.LotPrice;
                    }
                    break;
                default:
                    return null;
            }

            return null;
        }

        /// <summary>
        /// Gets all pricing units for the given pricing type.
        /// </summary>
        /// <param name="priceByType"></param>
        /// <returns></returns>
        public static IEnumerable<OrderPrice.enumPriceUnit> GetPriceUnits(PriceByType priceByType)
        {
            var priceUnits = new List<enumPriceUnit>();

            foreach (var priceUnit in Enum.GetValues(typeof(enumPriceUnit)).Cast<enumPriceUnit>())
            {
                if (GetPriceByType(priceUnit) == priceByType)
                {
                    priceUnits.Add(priceUnit);
                }
            }

            return priceUnits;
        }

        /// <summary>
        /// Gets the value that has the opposite pricing type of the given
        /// price unit.
        /// </summary>
        /// <param name="priceUnit"></param>
        /// <returns></returns>
        public static OrderPrice.enumPriceUnit GetOppositeValue(OrderPrice.enumPriceUnit priceUnit)
        {
            switch (priceUnit)
            {
                case OrderPrice.enumPriceUnit.Each:
                    return OrderPrice.enumPriceUnit.Lot;
                case OrderPrice.enumPriceUnit.Lot:
                    return OrderPrice.enumPriceUnit.Each;
                case OrderPrice.enumPriceUnit.EachByWeight:
                    return OrderPrice.enumPriceUnit.LotByWeight;
                case OrderPrice.enumPriceUnit.LotByWeight:
                    return OrderPrice.enumPriceUnit.EachByWeight;
                default:
                    return priceUnit;
            }
        }

        /// <summary>
        /// Returns the quantity price unit version of the given price unit.
        /// </summary>
        /// <param name="priceUnit"></param>
        /// <returns></returns>
        public static enumPriceUnit GetQuantityValue(enumPriceUnit priceUnit)
        {
            switch (GetPricingStrategy(priceUnit))
            {
                case PricingStrategy.Lot:
                    return enumPriceUnit.Lot;
                case PricingStrategy.Each:
                default:
                    return enumPriceUnit.Each;
            }
        }

        /// <summary>
        /// Returns the weight price unit version of the given price unit.
        /// </summary>
        /// <param name="priceUnit"></param>
        /// <returns></returns>
        public static enumPriceUnit GetWeightValue(enumPriceUnit priceUnit)
        {
            switch (GetPricingStrategy(priceUnit))
            {
                case PricingStrategy.Lot:
                    return enumPriceUnit.LotByWeight;
                case PricingStrategy.Each:
                default:
                    return enumPriceUnit.EachByWeight;
            }
        }

        public static enumPriceUnit GetPriceUnit(PriceByType priceBy, PricingStrategy pricingStrategy)
        {
            switch(priceBy)
            {
                case PriceByType.Quantity:
                    return pricingStrategy == PricingStrategy.Each ? enumPriceUnit.Each : enumPriceUnit.Lot;
                case PriceByType.Weight:
                    return pricingStrategy == PricingStrategy.Each ? enumPriceUnit.EachByWeight : enumPriceUnit.LotByWeight;
                default:
                    return enumPriceUnit.Each;
            }
        }

        /// <summary>
        /// Retrieves the <see cref="PriceByType"/> of an
        /// <see cref="enumPriceUnit"/> instance.
        /// </summary>
        /// <param name="priceUnit"></param>
        /// <returns></returns>
        public static PriceByType GetPriceByType(enumPriceUnit priceUnit)
        {
            var pricingType = typeof(enumPriceUnit).GetMember(priceUnit.ToString())
                .FirstOrDefault()
                ?.GetCustomAttributes(typeof(PriceTypeAttribute), false)
                ?.Cast<PriceTypeAttribute>()
                ?.First()
                ?.PricingType;

            return pricingType ?? PriceByType.Quantity;
        }

        /// <summary>
        /// Retrieves the <see cref="PricingStrategy"/> of an
        /// <see cref="enumPriceUnit"/> instance.
        /// </summary>
        /// <param name="priceUnit"></param>
        /// <returns></returns>
        public static PricingStrategy GetPricingStrategy(enumPriceUnit priceUnit)
        {
            var strategy = typeof(enumPriceUnit).GetMember(priceUnit.ToString())
                .FirstOrDefault()
                ?.GetCustomAttributes(typeof(PriceTypeAttribute), false)
                ?.Cast<PriceTypeAttribute>()
                ?.First()
                ?.Strategy;

            return strategy ?? PricingStrategy.Each;
        }

        /// <summary>
        /// Recalculates the original base price.
        /// </summary>
        /// <param name="priceUnit"></param>
        /// <param name="originalQty"></param>
        /// <param name="newQty"></param>
        /// <param name="originalBasePrice"></param>
        /// <returns>New base price.</returns>
        public static decimal RecalculatePrice(enumPriceUnit priceUnit, int originalQty, int newQty, decimal originalBasePrice)
        {
            const decimal maxValue = 999999.99999M;

            if (!IsPriceUnitLot(priceUnit))
            {
                return originalBasePrice;
            }

            decimal qtyRatio = Convert.ToDecimal(newQty) / Convert.ToDecimal(originalQty);
            return Math.Min(qtyRatio * originalBasePrice, maxValue);
        }

        public static Tuple<decimal, string> CalculateQuoteImportPrice(QuoteDataSet.QuotePartRow quotePartRec)
        {
            decimal basePrice = 0;
            string priceUnit;

            var useEach = quotePartRec.EachPrice > 0;
            var useLot = quotePartRec.LotPrice > 0;
            switch (useEach)
            {
                case true when useLot:
                    var lotThreshold = Math.Floor(quotePartRec.LotPrice / quotePartRec.EachPrice);
                    useLot = quotePartRec.Quantity <= lotThreshold;
                    basePrice = useLot ? quotePartRec.LotPrice : quotePartRec.EachPrice;
                    break;
                case true when !useLot:
                    basePrice = quotePartRec.EachPrice;
                    break;
                case false when useLot:
                    basePrice = quotePartRec.LotPrice;
                    break;
                case false when !useLot:
                    basePrice = 0;
                    break;
            }

            if (useLot)
                priceUnit = quotePartRec.PriceBy == "Quantity" ? OrderPrice.enumPriceUnit.Lot.ToString() : OrderPrice.enumPriceUnit.LotByWeight.ToString();
            else
                priceUnit = quotePartRec.PriceBy == "Quantity" ? OrderPrice.enumPriceUnit.Each.ToString() : OrderPrice.enumPriceUnit.EachByWeight.ToString();

            return new Tuple<decimal, string>(basePrice, priceUnit);
        }

        #endregion

        #region enumFeeType

        /// <summary>
        /// Represents a fee type.
        /// </summary>
        public enum enumFeeType
        {
            Fixed,
            Percentage
        };

        #endregion

        #region enumPriceUnit

        /// <summary>
        /// Represents a price unit.
        /// </summary>
        public enum enumPriceUnit
        {
            [Description("Each")]
            [PriceType(PriceByType.Quantity, PricingStrategy.Each)]
            Each,

            [Description("Lot")]
            [PriceType(PriceByType.Quantity, PricingStrategy.Lot)]
            Lot,

            [Description("Each By Weight")]
            [PriceType(PriceByType.Weight, PricingStrategy.Each)]
            EachByWeight,

            [Description("Lot By Weight")]
            [PriceType(PriceByType.Weight, PricingStrategy.Lot)]
            LotByWeight
        };

        #endregion

    }
}