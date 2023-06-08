using DWOS.Data.Order;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Data
{
    /// <summary>
    /// Defines extension methods for <see cref="IPriceUnitPersistence"/> instances.
    /// </summary>
    public static class PriceUnitPersistenceExtensions
    {
        /// <summary>
        /// Gets the default price unit for new orders.
        /// </summary>
        public static OrderPrice.enumPriceUnit DefaultPriceUnit(this IPriceUnitPersistence pricePersistence)
        {
            if (pricePersistence == null)
            {
                throw new ArgumentNullException(nameof(pricePersistence));
            }

            if (pricePersistence.IsActive(OrderPrice.enumPriceUnit.Lot.ToString()))
            {
                return OrderPrice.enumPriceUnit.Lot;
            }
            else if (pricePersistence.IsActive(OrderPrice.enumPriceUnit.LotByWeight.ToString()))
            {
                return OrderPrice.enumPriceUnit.LotByWeight;
            }
            else
            {
                return OrderPrice.enumPriceUnit.Each;
            }
        }

        /// <summary>
        /// Determines a price unit based on the current pricing type.
        /// </summary>
        /// <param name="pricePersistence"></param>
        /// <param name="customerId"></param>
        /// <param name="partQuantity"></param>
        /// <param name="weight"></param>
        /// <param name="currentPriceUnit">
        /// The current price unit - used in determining which <see cref="PriceByType"/> to use
        /// </param>
        /// <returns></returns>
        public static OrderPrice.enumPriceUnit DeterminePriceUnit(this IPriceUnitPersistence pricePersistence, int customerId, int partQuantity, decimal weight, OrderPrice.enumPriceUnit currentPriceUnit)
        {
            var priceByType = OrderPrice.GetPriceByType(currentPriceUnit);

            return pricePersistence.DeterminePriceUnit(customerId, partQuantity, weight, priceByType);
        }

        /// <summary>
        /// Determines a price unit based on a
        /// <see cref="PriceByType"/> instance.
        /// </summary>
        /// <param name="pricePersistence"></param>
        /// <param name="customerId"></param>
        /// <param name="partQuantity"></param>
        /// <param name="weight"></param>
        /// <param name="priceByType"></param>
        /// <returns></returns>
        public static OrderPrice.enumPriceUnit DeterminePriceUnit(this IPriceUnitPersistence pricePersistence, int customerId, int partQuantity, decimal weight, PriceByType priceByType)
        {
            if (pricePersistence == null)
            {
                throw new ArgumentNullException(nameof(pricePersistence));
            }

            OrderPrice.enumPriceUnit priceUnit;
            switch (priceByType)
            {
                case PriceByType.Weight:
                    // Weight
                    var eachByWeight = pricePersistence.FindByPriceUnitId(customerId, OrderPrice.enumPriceUnit.EachByWeight.ToString());
                    var lotByWeight = pricePersistence.FindByPriceUnitId(customerId, OrderPrice.enumPriceUnit.LotByWeight.ToString());

                    if (eachByWeight != null && lotByWeight != null)
                    {
                        var smallestPoint = eachByWeight.MinWeight < lotByWeight.MinWeight
                            ? eachByWeight
                            : lotByWeight;

                        var largestPoint = eachByWeight.MinWeight < lotByWeight.MinWeight
                            ? lotByWeight
                            : eachByWeight;

                        priceUnit = (weight < largestPoint.MinWeight) ? smallestPoint.PriceUnit : largestPoint.PriceUnit;
                    }
                    else
                    {
                        priceUnit = OrderPrice.enumPriceUnit.LotByWeight;
                    }
                    break;
                default:
                    // Quantity
                    var priceUnitEach = pricePersistence.FindByPriceUnitId(customerId, OrderPrice.enumPriceUnit.Each.ToString());
                    var priceUnitLot = pricePersistence.FindByPriceUnitId(customerId, OrderPrice.enumPriceUnit.Lot.ToString());

                    if (priceUnitEach != null && priceUnitLot != null)
                    {
                        var smallestPoint = priceUnitEach.MinQuantity < priceUnitLot.MinQuantity
                            ? priceUnitEach
                            : priceUnitLot;

                        var largestPoint = priceUnitEach.MinQuantity < priceUnitLot.MinQuantity
                            ? priceUnitLot
                            : priceUnitEach;

                        priceUnit = (partQuantity < largestPoint.MinQuantity) ? smallestPoint.PriceUnit : largestPoint.PriceUnit;
                    }
                    else
                    {
                        priceUnit = OrderPrice.enumPriceUnit.Lot;
                    }

                    break;
            }

            return priceUnit;
        }

        /// <summary>
        /// Gets the display name for a price unit.
        /// </summary>
        /// <param name="pricePersistence"></param>
        /// <param name="priceUnit">A price unit to retrieve user-friendly text for.</param>
        /// <returns>The string representation of a price unit.</returns>
        public static string GetDisplayText(this IPriceUnitPersistence pricePersistence, OrderPrice.enumPriceUnit priceUnit)
        {
            if (pricePersistence == null)
            {
                throw new ArgumentNullException(nameof(pricePersistence));
            }

            // Customer ID does not matter when retriving just the name
            var priceUnitRow = pricePersistence.FindByPriceUnitId(-1, priceUnit.ToString());

            if (priceUnitRow == null)
            {
                return priceUnit.ToString();
            }
            else
            {
                return priceUnitRow.DisplayName;
            }
        }

        /// <summary>
        /// Gets an enumeration of every active <see cref="PriceByType"/>.
        /// </summary>
        /// <param name="pricePersistence"></param>
        /// <returns></returns>
        public static IEnumerable<PriceByType> PriceByTypes(this IPriceUnitPersistence pricePersistence)
        {
            if (pricePersistence == null)
            {
                throw new ArgumentNullException(nameof(pricePersistence));
            }

            return pricePersistence.ActivePriceUnits
                .Select(OrderPrice.GetPriceByType)
                .Distinct();
        }

        public static bool IsActive(this IPriceUnitPersistence pricePersistence, OrderPrice.enumPriceUnit priceUnit)
        {
            if (pricePersistence == null)
            {
                throw new ArgumentNullException(nameof(pricePersistence));
            }

            return pricePersistence.IsActive(priceUnit.ToString());
        }
    }
}
