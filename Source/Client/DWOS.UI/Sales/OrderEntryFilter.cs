using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.UI.Utilities;

namespace DWOS.UI.Sales
{
    /// <summary>
    /// Defines filter-related functionality related to
    /// <see cref="OrderEntry"/>.
    /// </summary>
    internal static class OrderEntryFilter
    {
        #region Methods

        /// <summary>
        /// Filters orders.
        /// </summary>
        /// <param name="filterOrdersParams"></param>
        /// <returns>
        /// A list of filtered orders; may be <c>null</c> if results are empty.
        /// </returns>
        public static ICollection<OrdersDataSet.OrderRow> FilterOrders(FilterOrdersParams filterOrdersParams)
        {
            if (filterOrdersParams == null)
            {
                throw new ArgumentNullException(nameof(filterOrdersParams));
            }

            if (filterOrdersParams.OrdersDataset == null)
            {
                throw new ArgumentException(nameof(filterOrdersParams));
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            switch (filterOrdersParams.SearchField)
            {
                case OrderSearchField.SO:
                    filteredRows = ExactSalesOrderFilter(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                        filterOrdersParams.SearchCriteria);
                    break;
                case OrderSearchField.WO:
                    filteredRows = ExactWorkOrderFilter(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                        filterOrdersParams.SearchCriteria);
                    break;
                case OrderSearchField.PO:
                    if (filterOrdersParams.ExactMatch)
                    {
                        filteredRows = ExactPurchaseOrderFilter(filterOrdersParams.Mode,
                            filterOrdersParams.OrdersDataset,
                            filterOrdersParams.SearchCriteria);
                    }
                    else
                    {
                        filteredRows = PurchaseOrderFilter(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                            filterOrdersParams.SearchCriteria);
                    }
                    break;
                case OrderSearchField.COC:
                    filteredRows = ExactCocFilter(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                        filterOrdersParams.SearchCriteria);
                    break;
                case OrderSearchField.Part:
                    if (filterOrdersParams.ExactMatch)
                    {
                        filteredRows = ExactPartNameFilter(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                            filterOrdersParams.SearchCriteria);
                    }
                    else
                    {
                        filteredRows = PartNameFilter(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                            filterOrdersParams.SearchCriteria);
                    }
                    break;
                case OrderSearchField.Customer:
                    if (filterOrdersParams.ExactMatch)
                    {
                        filteredRows = ExactCustomerNameFilter(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                            filterOrdersParams.SearchCriteria);
                    }
                    else
                    {
                        filteredRows = CustomerNameFilter(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                            filterOrdersParams.SearchCriteria);
                    }
                    break;
                case OrderSearchField.CustomerWO:
                    if (filterOrdersParams.ExactMatch)
                    {
                        filteredRows = ExactCustomerWorkOrderFilter(filterOrdersParams.Mode,
                            filterOrdersParams.OrdersDataset,
                            filterOrdersParams.SearchCriteria);
                    }
                    else
                    {
                        filteredRows = CustomerWorkOrderFilter(filterOrdersParams.Mode,
                            filterOrdersParams.OrdersDataset,
                            filterOrdersParams.SearchCriteria);
                    }
                    break;
                case OrderSearchField.User:
                    if (filterOrdersParams.ExactMatch)
                    {
                        filteredRows = ExactUserNameFilter(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                            filterOrdersParams.SearchCriteria);
                    }
                    else
                    {
                        filteredRows = UserNameFilter(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                            filterOrdersParams.SearchCriteria);
                    }
                    break;
                case OrderSearchField.Quantity:
                    filteredRows = ExactQuantityFilter(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                        filterOrdersParams.SearchCriteria);
                    break;
                case OrderSearchField.Batch:
                    filteredRows = ExactBatchFilter(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                        filterOrdersParams.SearchCriteria);
                    break;
                case OrderSearchField.Package:
                    filteredRows = ExactPackageFilter(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                        filterOrdersParams.SearchCriteria);
                    break;
                case OrderSearchField.SerialNumber:
                    if (filterOrdersParams.ExactMatch)
                    {
                        filteredRows = ExactSerialNumberSearch(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                            filterOrdersParams.SearchCriteria);
                    }
                    else
                    {
                        filteredRows = SerialNumberSearch(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                            filterOrdersParams.SearchCriteria);
                    }
                    break;
                case OrderSearchField.ProductClass:
                    if (filterOrdersParams.ExactMatch)
                    {
                        filteredRows = ExactProductClassSearch(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                            filterOrdersParams.SearchCriteria);
                    }
                    else
                    {
                        filteredRows = ProductClassSearch(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                            filterOrdersParams.SearchCriteria);
                    }
                    break;
                case OrderSearchField.Custom:
                    if (filterOrdersParams.ExactMatch)
                    {
                        filteredRows = ExactCustomFieldFilter(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                            filterOrdersParams.CustomFieldName,
                            filterOrdersParams.SearchCriteria);
                    }
                    else
                    {
                        filteredRows = CustomFieldFilter(filterOrdersParams.Mode, filterOrdersParams.OrdersDataset,
                            filterOrdersParams.CustomFieldName,
                            filterOrdersParams.SearchCriteria);
                    }
                    break;
                default:
                    filteredRows = null;
                    break;
            }

            return filteredRows;
        }

        private static ICollection<OrdersDataSet.OrderRow> ExactSalesOrderFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            int salesOrderId;
            if (!int.TryParse(searchCriteria, out salesOrderId))
            {
                return null;
            }

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                var filterExpression = dsOrders.Order.SalesOrderIDColumn.ColumnName + " = " + salesOrderId;
                return dsOrders.Order.Select(filterExpression) as OrdersDataSet.OrderRow[];
            }
            else
            {
                return DatabaseFilter(dsOrders, searchCriteria,
                    OrderTableAdapter.OrderSearchField.ExactSalesOrder);
            }
        }

        private static ICollection<OrdersDataSet.OrderRow> ExactWorkOrderFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            int orderId;
            if (!int.TryParse(searchCriteria, out orderId))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows = null;

            //see if it is already loaded
            var row = dsOrders.Order.FindByOrderID(orderId);

            if (row != null)
            {
                filteredRows = new[] {row};
            }
            else if (mode == OrderEntry.OrderEntryMode.Normal)
            {
                using (var taOrder = CreateOrderTableAdapter())
                {
                    //load from DB
                    taOrder.FillByOrderID(dsOrders.Order, orderId);
                    row = dsOrders.Order.FindByOrderID(orderId);
                    if (row != null)
                        filteredRows = new[] {row};
                }
            }

            return filteredRows;
        }

        private static ICollection<OrdersDataSet.OrderRow> ExactPurchaseOrderFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                filteredRows =
                    dsOrders.Order.Where(
                            i =>
                                !i.IsPurchaseOrderNull() &&
                                i.PurchaseOrder.Equals(searchCriteria, StringComparison.CurrentCulture))
                        .ToList();
            }
            else
            {
                filteredRows = DatabaseFilter(dsOrders, searchCriteria,
                    OrderTableAdapter.OrderSearchField.ExactPo);
            }

            return filteredRows;
        }

        private static ICollection<OrdersDataSet.OrderRow> PurchaseOrderFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                var filterExpression = "{0} LIKE '%{1}%'".FormatWith(
                    dsOrders.Order.PurchaseOrderColumn.ColumnName,
                    DataUtilities.PrepareForRowFilterLike(searchCriteria));

                filteredRows = dsOrders.Order.Select(filterExpression) as OrdersDataSet.OrderRow[];
            }
            else
            {
                filteredRows = DatabaseFilter(dsOrders, searchCriteria,
                    OrderTableAdapter.OrderSearchField.Po);
            }

            return filteredRows;
        }

        private static ICollection<OrdersDataSet.OrderRow> ExactCocFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            int cocId;
            if (!int.TryParse(searchCriteria, out cocId))
            {
                MessageBoxUtilities.ShowMessageBoxWarn("COC must be a valid number.", "Invalid Number");
                return null;
            }

            return mode != OrderEntry.OrderEntryMode.Normal
                ? null
                : DatabaseFilter(dsOrders, searchCriteria, OrderTableAdapter.OrderSearchField.ExactCoc);
        }

        private static ICollection<OrdersDataSet.OrderRow> ExactPartNameFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                using (var taOrder = CreateOrderTableAdapter())
                {
                    var dtOrdersPart = new OrdersDataSet.OrderDataTable();

                    taOrder.FillBySearch(dtOrdersPart, OrderTableAdapter.OrderSearchField.ExactPartName, searchCriteria);

                    var foundParts = new List<OrdersDataSet.OrderRow>(dtOrdersPart.Count);

                    foreach (var searchedOrderRow in dtOrdersPart)
                    {
                        var dr = dsOrders.Order.FindByOrderID(searchedOrderRow.OrderID);

                        if ((dr != null) && dr.IsValidState() && !dr.GetOrderReviewRows().Any(or => or.Status))
                            foundParts.Add(dr);
                    }

                    filteredRows = foundParts;
                }
            }
            else
            {
                filteredRows = DatabaseFilter(dsOrders, searchCriteria,
                    OrderTableAdapter.OrderSearchField.ExactPartName);
            }
            return filteredRows;
        }


        private static ICollection<OrdersDataSet.OrderRow> PartNameFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                using (var taOrder = CreateOrderTableAdapter())
                {
                    var dtOrdersPart = new OrdersDataSet.OrderDataTable();

                    taOrder.FillBySearch(dtOrdersPart, OrderTableAdapter.OrderSearchField.PartName, searchCriteria);

                    var foundParts = new List<OrdersDataSet.OrderRow>(dtOrdersPart.Count);

                    foreach (var searchedOrderRow in dtOrdersPart)
                    {
                        var dr = dsOrders.Order.FindByOrderID(searchedOrderRow.OrderID);

                        if ((dr != null) && dr.IsValidState() && !dr.GetOrderReviewRows().Any(or => or.Status))
                            foundParts.Add(dr);
                    }

                    filteredRows = foundParts;
                }
            }
            else
            {
                filteredRows = DatabaseFilter(dsOrders, searchCriteria,
                    OrderTableAdapter.OrderSearchField.PartName);
            }
            return filteredRows;
        }

        private static ICollection<OrdersDataSet.OrderRow> ExactCustomerNameFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                filteredRows = dsOrders.Order
                    .Where(
                        o => o.CustomerSummaryRow.Name.Equals(searchCriteria, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
            }
            else
            {
                filteredRows = DatabaseFilter(dsOrders, searchCriteria,
                    OrderTableAdapter.OrderSearchField.ExactCustomerName);
            }

            return filteredRows;
        }

        private static ICollection<OrdersDataSet.OrderRow> CustomerNameFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                filteredRows = dsOrders.Order
                    .Where(o => o.CustomerSummaryRow.Name.Contains(false, searchCriteria))
                    .ToList();
            }
            else
            {
                filteredRows = DatabaseFilter(dsOrders, searchCriteria,
                    OrderTableAdapter.OrderSearchField.CustomerName);
            }

            return filteredRows;
        }

        private static ICollection<OrdersDataSet.OrderRow> ExactCustomerWorkOrderFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                filteredRows = dsOrders.Order
                    .Where(
                        o =>
                            !o.IsCustomerWONull() &&
                            o.CustomerWO.Equals(searchCriteria, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
            }
            else
            {
                filteredRows = DatabaseFilter(dsOrders, searchCriteria,
                    OrderTableAdapter.OrderSearchField.ExactCustomerWo);
            }
            return filteredRows;
        }


        private static ICollection<OrdersDataSet.OrderRow> CustomerWorkOrderFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                filteredRows = dsOrders.Order
                    .Where(o => !o.IsCustomerWONull() && o.CustomerWO.Contains(false, searchCriteria))
                    .ToList();
            }
            else
            {
                filteredRows = DatabaseFilter(dsOrders, searchCriteria,
                    OrderTableAdapter.OrderSearchField.CustomerWo);
            }
            return filteredRows;
        }

        private static ICollection<OrdersDataSet.OrderRow> ExactUserNameFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                filteredRows = dsOrders.Order
                    .Where(o => (o.UsersRow != null) && o.UsersRow.Name
                                    .Equals(searchCriteria, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
            }
            else
            {
                filteredRows = DatabaseFilter(dsOrders, searchCriteria,
                    OrderTableAdapter.OrderSearchField.ExactUserName);
            }

            return filteredRows;
        }

        private static ICollection<OrdersDataSet.OrderRow> UserNameFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                filteredRows = dsOrders.Order
                    .Where(o => (o.UsersRow != null) && o.UsersRow.Name
                                    .Contains(false, searchCriteria)).ToList();
            }
            else
            {
                filteredRows = DatabaseFilter(dsOrders, searchCriteria,
                    OrderTableAdapter.OrderSearchField.UserName);
            }
            return filteredRows;
        }

        private static ICollection<OrdersDataSet.OrderRow> ExactQuantityFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            if (!int.TryParse(searchCriteria, out _))
            {
                MessageBoxUtilities.ShowMessageBoxWarn("Quantity must be a valid number.", "Invalid Number");
                return null;
            }

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                return
                    dsOrders.Order.Select(dsOrders.Order.PartQuantityColumn.ColumnName + " = " + searchCriteria) as
                        OrdersDataSet.OrderRow[];
            }
            else
            {
                return DatabaseFilter(dsOrders, searchCriteria, OrderTableAdapter.OrderSearchField.ExactQuantity);
            }
        }

        private static ICollection<OrdersDataSet.OrderRow> ExactBatchFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            int batchId;

            if (!int.TryParse(searchCriteria, out batchId))
            {
                MessageBoxUtilities.ShowMessageBoxWarn("Batch must be a valid number.", "Invalid Number");
                return null;
            }

            if (mode != OrderEntry.OrderEntryMode.Normal)
            {
                return null;
            }

            return DatabaseFilter(dsOrders, searchCriteria, OrderTableAdapter.OrderSearchField.ExactBatch);
        }

        private static ICollection<OrdersDataSet.OrderRow> ExactPackageFilter(OrderEntry.OrderEntryMode mode,
            OrdersDataSet dsOrders,
            string searchCriteria)
        {
            int packageId;
            if (!int.TryParse(searchCriteria, out packageId))
            {
                MessageBoxUtilities.ShowMessageBoxWarn("Package must be a valid number.", "Invalid Number");
                return null;
            }

            if (mode != OrderEntry.OrderEntryMode.Normal)
            {
                return null;
            }

            return DatabaseFilter(dsOrders, searchCriteria, OrderTableAdapter.OrderSearchField.ExactShipmentPackage);
        }

        private static ICollection<OrdersDataSet.OrderRow> ExactSerialNumberSearch(OrderEntry.OrderEntryMode mode, OrdersDataSet dsOrders, string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                using (var taOrder = CreateOrderTableAdapter())
                {
                    var dtOrdersPart = new OrdersDataSet.OrderDataTable();

                    taOrder.FillBySearch(dtOrdersPart, OrderTableAdapter.OrderSearchField.ExactSerialNumber, searchCriteria);

                    var foundOrders = new List<OrdersDataSet.OrderRow>(dtOrdersPart.Count);

                    foreach (var searchedOrderRow in dtOrdersPart)
                    {
                        var dr = dsOrders.Order.FindByOrderID(searchedOrderRow.OrderID);

                        if ((dr != null) && dr.IsValidState() && !dr.GetOrderReviewRows().Any(or => or.Status))
                            foundOrders.Add(dr);
                    }

                    filteredRows = foundOrders;
                }
            }
            else
            {
                filteredRows = DatabaseFilter(dsOrders, searchCriteria,
                    OrderTableAdapter.OrderSearchField.ExactSerialNumber);
            }

            return filteredRows;
        }

        private static ICollection<OrdersDataSet.OrderRow> SerialNumberSearch(OrderEntry.OrderEntryMode mode, OrdersDataSet dsOrders, string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                using (var taOrder = CreateOrderTableAdapter())
                {
                    var dtOrdersPart = new OrdersDataSet.OrderDataTable();

                    taOrder.FillBySearch(dtOrdersPart, OrderTableAdapter.OrderSearchField.SerialNumber, searchCriteria);

                    var foundOrders = new List<OrdersDataSet.OrderRow>(dtOrdersPart.Count);

                    foreach (var searchedOrderRow in dtOrdersPart)
                    {
                        var dr = dsOrders.Order.FindByOrderID(searchedOrderRow.OrderID);

                        if ((dr != null) && dr.IsValidState() && !dr.GetOrderReviewRows().Any(or => or.Status))
                            foundOrders.Add(dr);
                    }

                    filteredRows = foundOrders;
                }

            }
            else
            {
                filteredRows = DatabaseFilter(dsOrders, searchCriteria,
                    OrderTableAdapter.OrderSearchField.SerialNumber);
            }
            return filteredRows;
        }

        private static ICollection<OrdersDataSet.OrderRow> ExactProductClassSearch(OrderEntry.OrderEntryMode mode, OrdersDataSet dsOrders, string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                using (var taOrder = CreateOrderTableAdapter())
                {
                    var dtOrdersPart = new OrdersDataSet.OrderDataTable();

                    taOrder.FillBySearch(dtOrdersPart, OrderTableAdapter.OrderSearchField.ExactProductClass, searchCriteria);

                    var foundOrders = new List<OrdersDataSet.OrderRow>(dtOrdersPart.Count);

                    foreach (var searchedOrderRow in dtOrdersPart)
                    {
                        var dr = dsOrders.Order.FindByOrderID(searchedOrderRow.OrderID);

                        if ((dr != null) && dr.IsValidState() && !dr.GetOrderReviewRows().Any(or => or.Status))
                            foundOrders.Add(dr);
                    }

                    filteredRows = foundOrders;
                }
            }
            else
            {
                filteredRows = DatabaseFilter(dsOrders, searchCriteria,
                    OrderTableAdapter.OrderSearchField.ExactProductClass);
            }

            return filteredRows;
        }

        private static ICollection<OrdersDataSet.OrderRow> ProductClassSearch(OrderEntry.OrderEntryMode mode, OrdersDataSet dsOrders, string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                using (var taOrder = CreateOrderTableAdapter())
                {
                    var dtOrdersPart = new OrdersDataSet.OrderDataTable();

                    taOrder.FillBySearch(dtOrdersPart, OrderTableAdapter.OrderSearchField.ProductClass, searchCriteria);

                    var foundOrders = new List<OrdersDataSet.OrderRow>(dtOrdersPart.Count);

                    foreach (var searchedOrderRow in dtOrdersPart)
                    {
                        var dr = dsOrders.Order.FindByOrderID(searchedOrderRow.OrderID);

                        if ((dr != null) && dr.IsValidState() && !dr.GetOrderReviewRows().Any(or => or.Status))
                            foundOrders.Add(dr);
                    }

                    filteredRows = foundOrders;
                }

            }
            else
            {
                filteredRows = DatabaseFilter(dsOrders, searchCriteria,
                    OrderTableAdapter.OrderSearchField.ProductClass);
            }
            return filteredRows;
        }

        private static ICollection<OrdersDataSet.OrderRow> ExactCustomFieldFilter(OrderEntry.OrderEntryMode mode, OrdersDataSet dsOrders, string customFieldName, string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                using (var taOrder = CreateOrderTableAdapter())
                {
                    var dtOrdersSearchResult = new OrdersDataSet.OrderDataTable();

                    taOrder.FillByExactCustomField(dtOrdersSearchResult, customFieldName, searchCriteria);

                    var foundOrders = new List<OrdersDataSet.OrderRow>(dtOrdersSearchResult.Count);

                    foreach (var searchedOrderRow in dtOrdersSearchResult)
                    {
                        var dr = dsOrders.Order.FindByOrderID(searchedOrderRow.OrderID);

                        if ((dr != null) && dr.IsValidState() && !dr.GetOrderReviewRows().Any(or => or.Status))
                        {
                            foundOrders.Add(dr);
                        }
                    }

                    filteredRows = foundOrders;
                }
            }
            else
            {
                using (var taOrder = CreateOrderTableAdapter())
                {
                    var dtOrderSearch = new OrdersDataSet.OrderDataTable();
                    taOrder.FillByExactCustomField(dtOrderSearch, customFieldName, searchCriteria);
                    filteredRows = ImportAll(dsOrders, dtOrderSearch);
                }
            }

            return filteredRows;
        }

        private static ICollection<OrdersDataSet.OrderRow> CustomFieldFilter(OrderEntry.OrderEntryMode mode, OrdersDataSet dsOrders, string customFieldName, string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            var searchValue = "%" + Data.Datasets.Utilities.PrepareForLike(searchCriteria) + "%";

            ICollection<OrdersDataSet.OrderRow> filteredRows;

            if (mode == OrderEntry.OrderEntryMode.Review || mode == OrderEntry.OrderEntryMode.ImportExportReview)
            {
                using (var taOrder = CreateOrderTableAdapter())
                {
                    var dtOrdersSearchResult = new OrdersDataSet.OrderDataTable();

                    taOrder.FillByCustomField(dtOrdersSearchResult, customFieldName, searchValue);

                    var foundOrders = new List<OrdersDataSet.OrderRow>(dtOrdersSearchResult.Count);

                    foreach (var searchedOrderRow in dtOrdersSearchResult)
                    {
                        var dr = dsOrders.Order.FindByOrderID(searchedOrderRow.OrderID);

                        if ((dr != null) && dr.IsValidState() && !dr.GetOrderReviewRows().Any(or => or.Status))
                        {
                            foundOrders.Add(dr);
                        }
                    }

                    filteredRows = foundOrders;
                }
            }
            else
            {
                using (var taOrder = CreateOrderTableAdapter())
                {
                    var dtOrderSearch = new OrdersDataSet.OrderDataTable();
                    taOrder.FillByCustomField(dtOrderSearch, customFieldName, searchValue);
                    filteredRows = ImportAll(dsOrders, dtOrderSearch);
                }
            }

            return filteredRows;
        }

        /// <summary>
        /// Imports orders from <paramref name="dtOrderSearch"/> to <see cref="dsOrders"/>.
        /// </summary>
        /// <param name="dsOrders"></param>
        /// <param name="dtOrderSearch"></param>
        /// <returns>
        /// List of orders found in <paramref name="dsOrders"/> or imported
        /// into <paramref name="dsOrders"/>.
        /// </returns>
        private static List<OrdersDataSet.OrderRow> ImportAll(OrdersDataSet dsOrders, OrdersDataSet.OrderDataTable dtOrderSearch)
        {
            var found = new List<OrdersDataSet.OrderRow>(dtOrderSearch.Count);

            foreach (var searchedOrderRow in dtOrderSearch)
            {
                var dr = dsOrders.Order.FindByOrderID(searchedOrderRow.OrderID);

                if (dr == null)
                {
                    dsOrders.Order.ImportRow(searchedOrderRow);
                    dr = dsOrders.Order.FindByOrderID(searchedOrderRow.OrderID);
                }

                if (dr != null)
                {
                    found.Add(dr);
                }
            }
            return found;
        }

        /// <summary>
        /// Filters orders through database access.
        /// </summary>
        /// <remarks>
        /// If filtered orders have not yet been loaded, this method adds
        /// them to <paramref name="dsOrders"/>.
        /// </remarks>
        /// <param name="dsOrders"></param>
        /// <param name="searchCriteria"></param>
        /// <param name="searchField"></param>
        /// <returns></returns>
        private static ICollection<OrdersDataSet.OrderRow> DatabaseFilter(OrdersDataSet dsOrders,
            string searchCriteria,
            OrderTableAdapter.OrderSearchField searchField)
        {
            using (var taOrder = CreateOrderTableAdapter())
            {
                var dtOrderSearch = new OrdersDataSet.OrderDataTable();
                taOrder.FillBySearch(dtOrderSearch, searchField, searchCriteria);
                var found = new List<OrdersDataSet.OrderRow>(dtOrderSearch.Count);

                foreach (var searchedOrderRow in dtOrderSearch)
                {
                    var dr = dsOrders.Order.FindByOrderID(searchedOrderRow.OrderID);

                    if (dr == null)
                    {
                        dsOrders.Order.ImportRow(searchedOrderRow);
                        dr = dsOrders.Order.FindByOrderID(searchedOrderRow.OrderID);
                    }

                    if (dr != null)
                    {
                        found.Add(dr);
                    }
                }

                return found;
            }
        }

        private static OrderTableAdapter CreateOrderTableAdapter()
        {
            return new OrderTableAdapter
            {
                ClearBeforeFill = false
            };
        }

        #endregion

        #region FilterOrdersParams

        /// <summary>
        /// Parameters for <see cref="FilterOrders"/>.
        /// </summary>
        internal class FilterOrdersParams
        {
            #region Properties

            /// <summary>
            /// Gets or sets the orders dataset for this instance.
            /// </summary>
            public OrdersDataSet OrdersDataset { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="OrderEntry"/> mode for this instance.
            /// </summary>
            public OrderEntry.OrderEntryMode Mode { get; set; }

            /// <summary>
            /// Gets or sets the search field for this instance.
            /// </summary>
            public OrderSearchField SearchField { get; set; }

            /// <summary>
            /// Gets or sets the search string for this instance.
            /// </summary>
            public string SearchCriteria { get; set; }

            /// <summary>
            /// Gets or sets the name of the custom field for this instance.
            /// </summary>
            public string CustomFieldName { get; set; }

            /// <summary>
            /// Gets or sets the value indicating if searches should match
            /// <see cref="SearchCriteria"/> exactly for this instance.
            /// </summary>
            /// <returns>
            /// <c>true</c> if matches need to be exact; otherwise,
            /// <c>false</c>.
            /// </returns>
            public bool ExactMatch { get; set; }

            #endregion
        }

        #endregion
    }
}