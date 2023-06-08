using System;
using DWOS.Data.Datasets;
using System.Collections.Generic;
using System.Linq;
using DWOS.Shared.Utilities;

namespace DWOS.Data.Order
{
    /// <summary>
    /// Contains utility methods related to orders.
    /// </summary>
    public static class OrderUtilities
    {
        #region Methods

        /// <summary>
        /// Calculates gross weight for an order.
        /// </summary>
        /// <param name="order"></param>
        /// <returns>The order's gross weight.</returns>
        public static decimal CalculateGrossWeight(OrdersDataSet.OrderRow order)
        {
            OrdersDataSet.OrderContainersDataTable dtContainers = null;

            decimal grossWeight = 0M;

            try
            {
                if (order == null)
                {
                    return 0M;
                }

                IEnumerable<OrdersDataSet.OrderContainersRow> containers = order.GetOrderContainersRows();

                if (!containers.Any())
                {
                    // Load containers
                    dtContainers = new OrdersDataSet.OrderContainersDataTable();

                    using (var taContainers = new Datasets.OrdersDataSetTableAdapters.OrderContainersTableAdapter())
                    {
                        taContainers.FillByOrder(dtContainers, order.OrderID);
                    }

                    containers = dtContainers;
                }

                if (containers.Any())
                {
                    var containersWeight = containers.Sum(container => container.IsWeightNull() ? 0M : container.Weight);

                    if (containersWeight == 0M)
                    {
                        grossWeight = GetOrderWeight(order);
                    }
                    else
                    {
                        grossWeight = containersWeight;
                    }
                }
                else
                {
                    grossWeight = GetOrderWeight(order);
                }
            }
            finally
            {
                dtContainers?.Dispose();
            }

            return grossWeight;
        }

        /// <summary>
        /// Calculates gross weight for an order.
        /// </summary>
        /// <param name="order"></param>
        /// <returns>The order's gross weight.</returns>
        public static decimal CalculateGrossWeight(COCDataset.OrderSummaryRow order)
        {
            if (order == null)
            {
                return 0M;
            }

            using (var taOrder = new Datasets.OrdersDataSetTableAdapters.OrderTableAdapter())
            {
                return CalculateGrossWeight(taOrder.GetByOrderID(order.OrderID).FirstOrDefault());
            }
        }

        /// <summary>
        /// Calculates total surface area.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>
        /// The order's total surface area.
        /// </returns>
        public static double CalculateTotalSurfaceAreaInches(int orderId)
        {
            OrderProcessingDataSet.OrderSummaryDataTable dtOrders = null;
            OrderProcessingDataSet.PartDataTable dtParts = null;

            double totalSurfaceArea = 0d;

            try
            {
                dtOrders = new OrderProcessingDataSet.OrderSummaryDataTable();
                dtParts = new OrderProcessingDataSet.PartDataTable();

                using (var taOrders = new Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
                {
                    taOrders.FillById(dtOrders, orderId);
                }

                var order = dtOrders.FirstOrDefault();

                if (order == null)
                {
                    return 0d;
                }

                var partQuantity = order.IsPartQuantityNull() ?
                    0 :
                    order.PartQuantity;

                using (var taPart = new Datasets.OrderProcessingDataSetTableAdapters.PartTableAdapter())
                {
                    taPart.FillByPart(dtParts, order.PartID);
                }

                var partSurfaceArea = dtParts.FirstOrDefault()?.SurfaceArea ?? 0d;
                totalSurfaceArea = partQuantity * partSurfaceArea;
            }
            finally
            {
                dtOrders?.Dispose();
                dtParts?.Dispose();
            }

            return totalSurfaceArea;
        }

        /// <summary>
        /// Gets a display string for the order using a formatted string.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetDisplayString(OrdersDataSet.OrderRow order, string format)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (string.IsNullOrEmpty(format))
            {
                return order.OrderID.ToString();
            }

            var reqDate = order.IsRequiredDateNull() ?
                string.Empty :
                "[" + order.RequiredDate.ToShortDateString() + "]";

            var custName = order.CustomerSummaryRow?.Name ?? string.Empty;

            var orderId = order.OrderID;

            return ApplyFormat(format, orderId, reqDate, custName, order.FromShippingManifest);
        }

        /// <summary>
        /// Gets a display string for the sales order using a formatted string.
        /// </summary>
        /// <param name="salesOrder"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetDisplayString(OrdersDataSet.SalesOrderRow salesOrder, string format)
        {
            if (salesOrder == null)
            {
                throw new ArgumentNullException(nameof(salesOrder));
            }

            if (string.IsNullOrEmpty(format))
            {
                return salesOrder.SalesOrderID.ToString();
            }

            var reqDate = salesOrder.IsRequiredDateNull() ?
                string.Empty :
                "[" + salesOrder.RequiredDate.ToShortDateString() + "]";

            var custName = salesOrder.CustomerSummaryRow?.Name ?? string.Empty;

            var salesOrderId = salesOrder.SalesOrderID;

            return ApplyFormat(format, salesOrderId, reqDate, custName, false);
        }

        private static string ApplyFormat(string format, int id, string formattedReqDate, string custName, bool fromShippingManifest)
        {
            var adjustedFormat = format;

            if (string.IsNullOrEmpty(formattedReqDate) && (format.Contains("<REQUIREDDATE>") || format.Contains("%REQUIREDDATE%")))
            {
                // Remove space afterwards - otherwise, could make output look incorrect
                adjustedFormat = format
                    .Replace("<REQUIREDDATE> ", string.Empty)
                    .Replace("%REQUIREDDATE% ", string.Empty);
            }

            // Format string with '%' is preferred, but the format used angle brackets in 16.3.0.
            var formattedString = adjustedFormat
                .Replace("%ID%", id.ToString())
                .Replace("%REQUIREDDATE%", formattedReqDate)
                .Replace("%CUSTOMERNAME%", custName)
                .Replace("<ID>", id.ToString())
                .Replace("<REQUIREDDATE>", formattedReqDate)
                .Replace("<CUSTOMERNAME>", custName)
                .TrimEnd();

            if (fromShippingManifest)
            {
                formattedString += " [Imported]";
            }

            var includesId = format.Contains("<ID>") || format.Contains("%ID%");

            return includesId ? formattedString : $"{id} {formattedString}";
        }

        /// <summary>
        /// Gets a display string for a Blanket PO using a formatted string.
        /// </summary>
        /// <param name="blanketPo"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetDisplayString(OrdersDataSet.OrderTemplateRow blanketPo, string format)
        {
            if (blanketPo == null)
            {
                throw new ArgumentNullException(nameof(blanketPo));
            }

            if (string.IsNullOrEmpty(format))
            {
                return blanketPo.OrderTemplateID.ToString();
            }

            var custName = blanketPo.CustomerSummaryRow?.Name ?? string.Empty;
            return ApplyFormat(format, blanketPo.OrderTemplateID, string.Empty, custName, false);
        }

        /// <summary>
        /// Retrieves the weight of an order, falling back to the weight
        /// defined on the part if necessary.
        /// </summary>
        /// <param name="order">
        /// The order instance to calculate weight for.
        /// </param>
        /// <returns>
        /// The weight of an order.
        /// </returns>
        public static decimal GetOrderWeight(OrdersDataSet.OrderRow order)
        {
            if (order == null)
            {
                return 0M;
            }

            decimal orderWeight;

            if (!order.IsWeightNull() && order.Weight != 0)
            {
                orderWeight = order.Weight;
            }
            else if (!order.IsPartQuantityNull())
            {
                decimal partWeight = 0M;

                using (var ta = new Datasets.PartsDatasetTableAdapters.PartTableAdapter())
                {
                    var part = ta.GetByOrder(order.OrderID)
                        .FindByPartID(order.PartID);

                    if (part != null && !part.IsWeightNull())
                    {
                        partWeight = part.Weight;
                    }
                }

                orderWeight = (partWeight * order.PartQuantity);
            }
            else
            {
                orderWeight = 0M;
            }

            return orderWeight;
        }

        /// <summary>
        /// Gets the work status of an order after processing.
        /// </summary>
        /// <param name="orderType">The type of order.</param>
        /// <param name="isPartMarking">
        /// Value that indicates if the order will use part marking.
        /// </param>
        /// <param name="orderRequiresCert">
        /// Value that indicates if the order requires a COC.
        /// </param>
        /// <returns>The order's next work status.</returns>
        public static string WorkStatusAfterProcessing(OrderType orderType, bool isPartMarking, bool orderRequiresCert)
        {
            var settings = DependencyContainer.Resolve<IDwosApplicationSettingsProvider>()?.Settings;

            if (settings == null)
            {
                throw new ApplicationException("Settings are not setup");
            }

            if (orderType == OrderType.ReworkInt) //if internal rework then set to pending join
                return settings.WorkStatusPendingJoin;
            if (settings.PartMarkingEnabled && isPartMarking) //if part marking required then send to part marking
                return settings.WorkStatusPartMarking;
            if (settings.COCEnabled && (!settings.AllowSkippingCoc || orderRequiresCert)) //if coc enabled then send to Final Inspection
                return settings.WorkStatusFinalInspection;

            //else proceed directly to shipping, do not pass go
            return settings.WorkStatusShipping;
        }

        /// <summary>
        /// Gets the work status of an order after quarantine.
        /// </summary>
        /// <param name="orderRequiresCert">
        /// Value that indicates if the order requires a COC.
        /// </param>
        /// <returns>The order's next work status.</returns>
        public static string WorkStatusAfterQuarantine(bool orderRequiresCert)
        {
            var settings = DependencyContainer.Resolve<IDwosApplicationSettingsProvider>()?.Settings;

            if (settings == null)
            {
                throw new ApplicationException("Settings are not setup");
            }

            return settings.COCEnabled && (!settings.AllowSkippingCoc || orderRequiresCert)
                ? settings.WorkStatusFinalInspection
                : settings.WorkStatusShipping;
        }

        /// <summary>
        /// Gets the location of an order after quarantine.
        /// </summary>
        /// <param name="orderRequiresCert">
        /// Value that indicates if the order requires a COC.
        /// </param>
        /// <returns>The order's next location.</returns>
        public static string LocationAfterQuarantine(bool orderRequiresCert)
        {
            var settings = DependencyContainer.Resolve<IDwosApplicationSettingsProvider>()?.Settings;

            if (settings == null)
            {
                throw new ApplicationException("Settings are not setup");
            }
            return settings.COCEnabled && (!settings.AllowSkippingCoc || orderRequiresCert)
                ? settings.DepartmentQA
                : settings.DepartmentShipping;
        }

        /// <summary>
        /// Gets the work status of an order after part marking.
        /// </summary>
        /// <param name="orderRequiresCert">
        /// Value that indicates if the order requires a COC.
        /// </param>
        /// <returns>The order's next work status.</returns>
        public static string WorkStatusAfterPartMark(bool orderRequiresCert) =>
            WorkStatusAfterQuarantine(orderRequiresCert);

        /// <summary>
        /// Gets the location of an order after part marking.
        /// </summary>
        /// <param name="orderRequiresCert">
        /// Value that indicates if the order requires a COC.
        /// </param>
        /// <returns>The order's next location.</returns>
        public static string LocationAfterPartMark(bool orderRequiresCert) =>
            LocationAfterQuarantine(orderRequiresCert);

        #endregion
    }
}
