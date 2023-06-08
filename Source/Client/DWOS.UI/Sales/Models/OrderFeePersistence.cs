using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using System;
using System.Collections.Generic;

namespace DWOS.UI.Sales.Models
{
    internal class OrderFeePersistence
    {
        #region Properties

        public IDwosApplicationSettingsProvider SettingsProvider { get; }

        #endregion

        #region Methods

        public OrderFeePersistence(IDwosApplicationSettingsProvider appSettingsProvider)
        {
            SettingsProvider = appSettingsProvider
                ?? throw new ArgumentNullException(nameof(appSettingsProvider));
        }

        public List<OrderFeeType> RetrieveOrderFeeTypes()
        {
            var defaultFeeIds = new HashSet<string>();
            var defaultFeesString = SettingsProvider.Settings.DefaultFees;

            if (!string.IsNullOrEmpty(defaultFeesString))
            {
                defaultFeeIds = new HashSet<string>(defaultFeesString.Split(new[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries));
            }

            var feeTypes = new List<OrderFeeType>();

            using (var dtOrderFeeType = new OrdersDataSet.OrderFeeTypeDataTable())
            {
                using (var taOrderFeeType = new OrderFeeTypeTableAdapter())
                {
                    taOrderFeeType.Fill(dtOrderFeeType);
                }

                foreach (var orderFeeRow in dtOrderFeeType)
                {
                    Enum.TryParse<OrderPrice.enumFeeType>(orderFeeRow.FeeType, out var feeType);
                    feeTypes.Add(new OrderFeeType(orderFeeRow.OrderFeeTypeID,
                        feeType,
                        orderFeeRow.Price,
                        defaultFeeIds.Contains(orderFeeRow.OrderFeeTypeID)));
                }
            }

            return feeTypes;
        }

        #endregion
    }
}
