using System.Collections.Generic;

namespace DWOS.Data.Order
{
    /// <summary>
    /// Retrieves price unit data from persistence.
    /// </summary>
    public interface IPriceUnitPersistence
    {
        /// <summary>
        /// Gets a collection of active price units.
        /// </summary>
        IEnumerable<OrderPrice.enumPriceUnit> ActivePriceUnits { get; }

        /// <summary>
        /// Returns price unit data for the given price unit.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="priceUnitId"></param>
        /// <returns></returns>
        PriceUnitData FindByPriceUnitId(int customerId, string priceUnitId);

        /// <summary>
        /// Is the price unit active?
        /// </summary>
        /// <param name="priceUnitId"></param>
        /// <returns>true if yes; otherwise, false</returns>
        bool IsActive(string priceUnitId);
    }
}