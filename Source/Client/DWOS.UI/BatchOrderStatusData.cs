using DWOS.Data.Datasets;

namespace DWOS.UI
{
    /// <summary>
    /// Represents an order within a batch on the WIP screen.
    /// </summary>
    public sealed class BatchOrderStatusData
    {
        /// <summary>
        /// Gets the order ID for this instance.
        /// </summary>
        public int OrderID
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the purchase order for this instance.
        /// </summary>
        public string PurchaseOrder
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the customer name for this instance.
        /// </summary>
        public string CustomerName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the part ID for this instance.
        /// </summary>
        public int PartID
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the part name for this instance.
        /// </summary>
        public string PartName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the quantity for this instance.
        /// </summary>
        public int BatchPartQuantity
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new <see cref="BatchOrderStatusData"/> instance.
        /// </summary>
        /// <param name="dbRow"></param>
        /// <returns>
        /// A new <see cref="BatchOrderStatusData"/> representing <paramref name="dbRow"/>;
        /// <c>null</c> if <paramref name="dbRow"/> is <c>null</c>.
        /// </returns>
        public static BatchOrderStatusData CreateFrom(OrderStatusDataSet.BatchOrderStatusRow dbRow)
        {
            if (dbRow == null)
            {
                return null;
            }

            return new BatchOrderStatusData()
            {
                OrderID = dbRow.OrderID,
                PurchaseOrder = dbRow.IsPurchaseOrderNull() ? string.Empty : dbRow.PurchaseOrder,
                CustomerName = dbRow.CustomerName,
                PartID = dbRow.PartID,
                PartName = dbRow.PartName,
                BatchPartQuantity = dbRow.BatchPartQuantity
            };
        }
    }
}