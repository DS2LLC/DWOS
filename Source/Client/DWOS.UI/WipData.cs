using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderStatusDataSetTableAdapters;

namespace DWOS.UI
{
    /// <summary>
    /// Contains WIP screen data.
    /// </summary>
    public class WipData
    {
        #region Properties

        /// <summary>
        /// Gets the order dataset for this instance.
        /// </summary>
        public OrderStatusDataSet OrderDataSet { get; }

        /// <summary>
        /// Gets a value that indicates the types of data that this instance has.
        /// </summary>
        /// <remarks>
        /// For example, if there is no need to load <see cref="Batches"/>, this instance
        /// will not contain batch data.
        /// </remarks>
        public WipDataType DataType { get; }

        /// <summary>
        /// Gets the orders contained in this instance.
        /// </summary>
        public ICollection<OrderStatusData> Orders { get; }

        /// <summary>
        /// Gets the batches contained in this instance.
        /// </summary>
        public ICollection<BatchStatusData> Batches { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Private constructor for use with the <see cref="Empty"/> method.
        /// </summary>
        private WipData()
        {
            OrderDataSet = new OrderStatusDataSet();
            Orders = new List<OrderStatusData>();
            Batches = new List<BatchStatusData>();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="WipData"/> class.
        /// </summary>
        /// <param name="dsOrderStatus">Dataset to use</param>
        /// <param name="dataType">Data types to retrieve data for</param>
        public WipData(OrderStatusDataSet dsOrderStatus, WipDataType dataType = WipDataType.None)
        {
            OrderDataSet = dsOrderStatus ?? throw new ArgumentNullException(nameof(dsOrderStatus));
            DataType = dataType;

            Orders = dataType.HasFlag(WipDataType.Orders) ? GetOrders() : new List<OrderStatusData>();

            Batches = dataType.HasFlag(WipDataType.Batches) ? GetBatches() : new List<BatchStatusData>();
        }

        /// <summary>
        /// Returns a blank <see cref="WipData"/> instance with no data.
        /// </summary>
        /// <returns></returns>
        public static WipData Empty()
        {
            return new WipData();
        }

        private ICollection<OrderStatusData> GetOrders()
        {
            return OrderDataSet
                .OrderStatus
                .Select(OrderStatusData.CreateFrom)
                .ToList();
        }

        private ICollection<BatchStatusData> GetBatches() => OrderDataSet.BatchStatus
            .Select(BatchStatusData.CreateFrom)
            .ToList();

        #endregion

        #region WipDataType

        [Flags]
        public enum WipDataType
        {
            None = 0,
            Orders = 1,
            Batches = 2
        }
        #endregion
    }
}
