using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;

namespace DWOS.Data.Order
{
    /// <summary>
    /// Determines allocated quantity for blanket POs.
    /// </summary>
    public class BlanketAllocationHelper
    {
        private readonly OrdersDataSet _dataset;
        private readonly HashSet<int> _ordersWithLoadedDependents;
        private readonly OrderTableAdapter _taOrder;

        public BlanketAllocationHelper(OrdersDataSet dataset, OrderTableAdapter taOrder)
        {
            _ordersWithLoadedDependents = new HashSet<int>();
            _dataset = dataset;
            _taOrder = taOrder;
        }

        public static BlanketAllocationHelper From(OrdersDataSet dataset, OrderTableAdapter taOrder)
        {
            return new BlanketAllocationHelper(dataset, taOrder);
        }

        public int GetAllocatedQuantity(OrdersDataSet.OrderTemplateRow orderTemplate)
        {
            if (orderTemplate == null)
            {
                return 0;
            }

            return orderTemplate.GetOrderRows().Sum(GetTotalQuantity);
        }

        private int GetTotalQuantity(OrdersDataSet.OrderRow order)
        {
            if (order == null || !order.IsValidState())
            {
                return 0;
            }

            return order.IsPartQuantityNull() ? 0 : order.PartQuantity;
        }
    }
}
