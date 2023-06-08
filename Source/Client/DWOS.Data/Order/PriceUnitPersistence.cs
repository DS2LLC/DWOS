using DWOS.Data.Datasets;
using DWOS.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Data.Order
{
    /// <summary>
    /// Implementation of <see cref="IPriceUnitPersistence"/> that connects
    /// to the database.
    /// </summary>
    public sealed class PriceUnitPersistence : IPriceUnitPersistence
    {
        #region Fields

        private readonly Lazy<PersistenceData> _dbData = new Lazy<PersistenceData>(() => new PersistenceData());

        #endregion

        #region IPriceUnitPersistence Members

        public IEnumerable<OrderPrice.enumPriceUnit> ActivePriceUnits =>
            _dbData.Value.PriceUnits
                .Where(unit => unit.Active)
                .Select(unit => OrderPrice.ParsePriceUnit(unit.PriceUnitID));

        public PriceUnitData FindByPriceUnitId(int customerId, string priceUnitId)
        {
            return CustomerDefaultPricePoint(customerId, priceUnitId) ?? SystemDefaultPricePoint(priceUnitId);
        }

        private PriceUnitData CustomerDefaultPricePoint(int customerId, string priceUnitId)
        {
            var priceUnitRow = _dbData.Value.PriceUnits.FindByPriceUnitID(priceUnitId);
            var customerPricePoint = _dbData.Value.CustomerPricePoints.FirstOrDefault(i => i.CustomerID == customerId);

            if (customerPricePoint == null)
            {
                return null;
            }

            var detailRow = _dbData.Value.CustomerPricePointDetails
                .FirstOrDefault(i => i.CustomerPricePointID == customerPricePoint.CustomerPricePointID && i.PriceUnit == priceUnitId);

            if (detailRow == null)
            {
                return null;
            }

            var priceUnitData = new PriceUnitData()
            {
                Active = priceUnitRow.Active,
                PriceUnit = OrderPrice.ParsePriceUnit(priceUnitRow.PriceUnitID),
                DisplayName = priceUnitRow.DisplayName
            };

            switch (OrderPrice.ParsePriceUnit(priceUnitId))
            {
                case OrderPrice.enumPriceUnit.Each:
                case OrderPrice.enumPriceUnit.Lot:
                    priceUnitData.MinQuantity = int.Parse(detailRow.MinValue);
                    int? maxQuantity = null;

                    if (!detailRow.IsMaxValueNull())
                    {
                        NullableParser.TryParse(detailRow.MaxValue, out maxQuantity);
                    }

                    priceUnitData.MaxQuantity = maxQuantity ?? PriceUnitData.MAX_QUANTITY;
                    break;
                case OrderPrice.enumPriceUnit.EachByWeight:
                case OrderPrice.enumPriceUnit.LotByWeight:
                    priceUnitData.MinWeight = decimal.Parse(detailRow.MinValue);
                    decimal? maxWeight = null;

                    if (!detailRow.IsMaxValueNull())
                    {
                        NullableParser.TryParse(detailRow.MaxValue, out maxWeight);
                    }

                    priceUnitData.MaxWeight = maxWeight ?? PriceUnitData.MAX_WEIGHT;
                    break;
            }

            return priceUnitData;
        }

        private PriceUnitData SystemDefaultPricePoint(string priceUnitId)
        {
            var priceUnitRow = _dbData.Value.PriceUnits.FindByPriceUnitID(priceUnitId);
            var detailRow = _dbData.Value.PricePointDetails.FirstOrDefault(i => i.PriceUnit == priceUnitId);
            if (priceUnitRow == null || detailRow == null)
            {
                return null;
            }

            var priceUnitData = new PriceUnitData()
            {
                Active = priceUnitRow.Active,
                PriceUnit = OrderPrice.ParsePriceUnit(priceUnitRow.PriceUnitID),
                DisplayName = priceUnitRow.DisplayName
            };

            switch (OrderPrice.ParsePriceUnit(priceUnitId))
            {
                case OrderPrice.enumPriceUnit.Each:
                case OrderPrice.enumPriceUnit.Lot:
                    priceUnitData.MinQuantity = int.Parse(detailRow.MinValue);
                    int? maxQuantity = null;

                    if (!detailRow.IsMaxValueNull())
                    {
                        NullableParser.TryParse(detailRow.MaxValue, out maxQuantity);
                    }

                    priceUnitData.MaxQuantity = maxQuantity ?? PriceUnitData.MAX_QUANTITY;
                    break;
                case OrderPrice.enumPriceUnit.EachByWeight:
                case OrderPrice.enumPriceUnit.LotByWeight:
                    priceUnitData.MinWeight = decimal.Parse(detailRow.MinValue);
                    decimal? maxWeight = null;

                    if (!detailRow.IsMaxValueNull())
                    {
                        NullableParser.TryParse(detailRow.MaxValue, out maxWeight);
                    }

                    priceUnitData.MaxWeight = maxWeight ?? PriceUnitData.MAX_WEIGHT;
                    break;
            }

            return priceUnitData;
        }

        public bool IsActive(string priceUnitId)
        {
            return _dbData.Value.PriceUnits.Any(unit => unit.Active && unit.PriceUnitID == priceUnitId);
        }

        #endregion

        #region PersistenceData

        private sealed class PersistenceData
        {
            #region Properties

            public OrdersDataSet.PriceUnitDataTable  PriceUnits { get; }

            public OrdersDataSet.PricePointDataTable PricePoints { get; }

            public OrdersDataSet.PricePointDetailDataTable PricePointDetails { get; }

            public OrdersDataSet.CustomerPricePointDataTable CustomerPricePoints { get; }

            public OrdersDataSet.CustomerPricePointDetailDataTable CustomerPricePointDetails { get; }

            #endregion

            #region Methods

            public PersistenceData()
            {
                // Load price units
                PriceUnits = new OrdersDataSet.PriceUnitDataTable();
                using (var taPriceUnit = new Datasets.OrdersDataSetTableAdapters.PriceUnitTableAdapter())
                {
                    taPriceUnit.Fill(PriceUnits);
                }

                // Load default price points
                PricePoints = new OrdersDataSet.PricePointDataTable();

                using (var taPricePoints = new Datasets.OrdersDataSetTableAdapters.PricePointTableAdapter())
                {
                    taPricePoints.FillDefault(PricePoints);
                }

                int defaultPricePointId = PricePoints.FirstOrDefault()?.PricePointID ?? 0;

                PricePointDetails = new OrdersDataSet.PricePointDetailDataTable();

                using (var taPriceDetails = new Datasets.OrdersDataSetTableAdapters.PricePointDetailTableAdapter())
                {
                    taPriceDetails.FillByPricePoint(PricePointDetails, defaultPricePointId);
                }

                // Load customer price points
                CustomerPricePoints = new OrdersDataSet.CustomerPricePointDataTable();

                using (var taPricePoints = new Datasets.OrdersDataSetTableAdapters.CustomerPricePointTableAdapter())
                {
                    taPricePoints.FillDefaultAll(CustomerPricePoints);
                }

                CustomerPricePointDetails = new OrdersDataSet.CustomerPricePointDetailDataTable();

                using (var taPriceDetails = new Datasets.OrdersDataSetTableAdapters.CustomerPricePointDetailTableAdapter { ClearBeforeFill = false })
                {
                    foreach (var pricePointRow in CustomerPricePoints)
                    {
                        taPriceDetails.FillByPricePoint(CustomerPricePointDetails, pricePointRow.CustomerPricePointID);
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
