using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Reports;
using DWOS.Data.Reports.OrdersReportTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Reports.ReportData
{
    internal class CurrentOrderStatusPersistence : IDisposable
    {
        #region Fields

        private const int RECEIVED_LEAD_DAYS = 3;

        /// <summary>
        /// Customer ID that indicates that a specific customer was not selected.
        /// </summary>
        private const int NO_CUSTOMER_ID = -1;

        private bool _isDisposed;

        /// <summary>
        /// IDs of orders with loaded serial numbers.
        /// </summary>
        private readonly HashSet<int> _loadedSerialNumbers = new HashSet<int>();

        /// <summary>
        /// IDs of orders with loaded product classes.
        /// </summary>
        private readonly HashSet<int> _loadedProductClasses = new HashSet<int>();

        /// <summary>
        /// Acts as a cache for serial numbers and product classes
        /// </summary>
        private readonly OrdersReport _dsOrdersReport = new OrdersReport
        {
            EnforceConstraints = false
        };

        private readonly OrderSerialNumberTableAdapter _taOrderSerialNumber =
            new OrderSerialNumberTableAdapter { ClearBeforeFill = false };

        private readonly OrderProductClassTableAdapter _taOrderProductClass =
            new OrderProductClassTableAdapter { ClearBeforeFill = false };

        private readonly ApplicationSettings _appSettings;

        #endregion

        #region Methods

        public CurrentOrderStatusPersistence(ApplicationSettings settings)
        {
            _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public List<string> GetSerialNumbers(OrdersReport.OrderRow order)
        {
            if (!_loadedSerialNumbers.Contains(order.OrderID))
            {
                _taOrderSerialNumber.FillActiveByOrder(_dsOrdersReport.OrderSerialNumber, order.OrderID);
                _loadedSerialNumbers.Add(order.OrderID);
            }

            return _dsOrdersReport.OrderSerialNumber
                .Where(s => s.OrderID == order.OrderID && s.Active)
                .Select(s => s.IsNumberNull() ? string.Empty : s.Number).ToList();
        }

        public string GetProductClass(OrdersReport.OrderRow order)
        {
            if (!_loadedProductClasses.Contains(order.OrderID))
            {
                _taOrderProductClass.FillByOrder(_dsOrdersReport.OrderProductClass, order.OrderID);
                _loadedProductClasses.Add(order.OrderID);
            }

            var productClassRow = _dsOrdersReport.OrderProductClass.FirstOrDefault(p => p.OrderID == order.OrderID);

            return productClassRow == null || productClassRow.IsProductClassNull()
                ? "N/A"
                : productClassRow.ProductClass;
        }

        public List<OrderInfo> GetInHouseData(List<int> customerIds, DateTime fromDate, DateTime toDate)
        {
            var orderInfos = new List<OrderInfo>();

            // Retrieve in-house orders
            using (var dsOrderReport = new OrdersReport { EnforceConstraints = false })
            {
                if (customerIds.Count == 1 && customerIds.Contains(NO_CUSTOMER_ID))
                {
                    using (var taOrders = new OrderTableAdapter())
                        taOrders.FillOpenOrdersByDate(dsOrderReport.Order, toDate, fromDate);
                }
                else
                {
                    using (var taOrders = new OrderTableAdapter { ClearBeforeFill = false })
                    {
                        foreach (var relatedCustomerId in customerIds)
                        {
                            taOrders.FillOpenOrdersByCustomerAndDate(dsOrderReport.Order, toDate, fromDate, relatedCustomerId);
                        }
                    }
                }

                foreach (var orderRow in dsOrderReport.Order.OrderBy(o => o.OrderID))
                {
                    var info = new OrderInfo
                    {
                        CustomerName = orderRow.CustomerName,
                        OrderID = orderRow.OrderID,
                        PartDescription = orderRow.PartName,
                        CurrentLocation = orderRow.CurrentLocation,
                    };

                    if (!orderRow.IsCurrentProcessNull() && orderRow.WorkStatus == _appSettings.WorkStatusInProcess)
                        info.CurrentProcess = orderRow.CurrentProcess;
                    if (!orderRow.IsCurrentProcessStartDateNull()  && orderRow.WorkStatus == _appSettings.WorkStatusInProcess)
                        info.CurrentProcessStartDate = orderRow.CurrentProcessStartDate;
                    if (!orderRow.IsCustomerWONull())
                        info.CustomerWO = orderRow.CustomerWO;
                    if (!orderRow.IsPartIDNull())
                        info.PartNumber = orderRow.PartID;
                    if (!orderRow.IsPurchaseOrderNull())
                        info.PurchaseOrder = orderRow.PurchaseOrder;
                    if (!orderRow.IsOrderDateNull())
                        info.ReceivedDate = orderRow.OrderDate;
                    if (!orderRow.IsPartQuantityNull())
                        info.PartQuantity = orderRow.PartQuantity;
                    if (!orderRow.IsEstShipDateNull())
                        info.EstShipDate = orderRow.EstShipDate;
                    if (!orderRow.IsAdjustedEstShipDateNull())
                        info.AdjustedEstShipDate = orderRow.AdjustedEstShipDate;

                    info.Status = orderRow.WorkStatus;
                    info.SerialNumbers = GetSerialNumbers(orderRow);
                    info.ProductClass = GetProductClass(orderRow);

                    orderInfos.Add(info);
                }
            }

            return orderInfos;
        }

        public List<OrderInfo> GetReceivedData(List<int> customerIds, DateTime FromDate, DateTime ToDate)
        {
            // Retrieve received orders
            var orderInfos = new List<OrderInfo>();

            using (OrdersReport dsOrderReport = new OrdersReport { EnforceConstraints = false })
            {
                var receiveFromDate = FromDate.AddDays(-RECEIVED_LEAD_DAYS); //include lead days in received tab

                if (customerIds.Count == 1 && customerIds.Contains(NO_CUSTOMER_ID))
                {
                    using (var taOrders = new OrderTableAdapter())
                    {
                        taOrders.FillAllOrdersByDate(dsOrderReport.Order, ToDate, receiveFromDate);
                    }
                }
                else
                {
                    using (var taOrders = new OrderTableAdapter { ClearBeforeFill = false })
                    {
                        foreach (var relatedCustomerId in customerIds)
                        {
                            taOrders.FillAllOrdersByDateAndCustomer(dsOrderReport.Order, ToDate, receiveFromDate, relatedCustomerId);
                        }
                    }
                }

                foreach (OrdersReport.OrderRow item in dsOrderReport.Order.OrderBy(o => o.OrderID))
                {
                    var info = new OrderInfo
                    {
                        CustomerName = item.CustomerName,
                        OrderID = item.OrderID,
                        PartDescription = item.PartName,
                        OrderType = item.OrderType
                    };

                    if (!item.IsPurchaseOrderNull())
                        info.PurchaseOrder = item.PurchaseOrder;
                    if (!item.IsPartQuantityNull())
                        info.PartQuantity = item.PartQuantity;
                    if (!item.IsEstShipDateNull())
                        info.EstShipDate = item.EstShipDate;
                    if (!item.IsAdjustedEstShipDateNull())
                        info.AdjustedEstShipDate = item.AdjustedEstShipDate;
                    if (!item.IsCustomerWONull())
                        info.CustomerWO = item.CustomerWO;

                    info.Status = item.Hold ? "On Hold" : ((OrderType)item.OrderType).ToDisplayString();
                    info.SerialNumbers = GetSerialNumbers(item);
                    info.ProductClass = GetProductClass(item);

                    orderInfos.Add(info);
                }
            }

            return orderInfos;
        }

        public List<OrderInfo> GetAwaitingShipmentData(List<int> customerIds, DateTime FromDate, DateTime ToDate)
        {
            // Retrieve orders that have been COC'd but not shipped (completed)
            var orderInfos = new List<OrderInfo>();

            using (var dsOrderReport = new OrdersReport { EnforceConstraints = false })
            {
                if (customerIds.Count == 1 && customerIds.Contains(NO_CUSTOMER_ID))
                {
                    OrderTableAdapter taOrders = null;
                    COCTableAdapter taCoc = null;

                    try
                    {
                        taOrders = new OrderTableAdapter();
                        taCoc = new COCTableAdapter();

                        if (ApplicationSettings.Current.COCEnabled)
                            taOrders.FillAwaitingShipmentWithCOC(dsOrderReport.Order, FromDate);
                        else
                            taOrders.FillAwaitingShipment(dsOrderReport.Order, FromDate);

                        taCoc.FillCustomersCOCByClosedDate(dsOrderReport.COC, ToDate, FromDate);
                    }
                    finally
                    {
                        taOrders?.Dispose();
                        taCoc?.Dispose();
                    }
                }
                else
                {
                    COCTableAdapter taCoc = null;
                    OrderTableAdapter taOrders = null;

                    try
                    {
                        taCoc = new COCTableAdapter { ClearBeforeFill = false };
                        taOrders = new OrderTableAdapter { ClearBeforeFill = false };

                        foreach (var relatedCustomerId in customerIds)
                        {
                            taCoc.FillCOCByClosedDate(dsOrderReport.COC, ToDate, FromDate, relatedCustomerId);

                            if (ApplicationSettings.Current.COCEnabled)
                            {
                                taOrders.FillOrdersAwaitingShipmentwithCOCs(dsOrderReport.Order, relatedCustomerId, FromDate);
                            }
                            else
                            {
                                taOrders.FillOrdersAwaitingShipment(dsOrderReport.Order, relatedCustomerId, FromDate);
                            }
                        }
                    }
                    finally
                    {
                        taCoc?.Dispose();
                        taOrders?.Dispose();
                    }
                }

                foreach (var item in dsOrderReport.Order.OrderBy(o => o.OrderID))
                {
                    var info = new OrderInfo
                    {
                        OrderID = item.OrderID,
                        CustomerName = item.CustomerName,
                        PartDescription = item.PartName,
                        CurrentLocation = item.CurrentLocation
                    };

                    if (!item.IsPartIDNull())
                        info.PartNumber = item.PartID;
                    if (!item.IsPurchaseOrderNull())
                        info.PurchaseOrder = item.PurchaseOrder;
                    if (!item.IsOrderDateNull())
                        info.ReceivedDate = item.OrderDate;
                    if (!item.IsPartQuantityNull())
                        info.PartQuantity = item.PartQuantity;
                    if (!item.IsCompletedDateNull())
                        info.CompletedDate = item.CompletedDate;
                    if (!item.IsCustomerWONull())
                        info.CustomerWO = item.CustomerWO;
                    info.Status = item.Hold ? "On Hold" : ((OrderType)item.OrderType).ToDisplayString();

                    OrdersReport.COCRow coc = dsOrderReport.COC.Where(c => c.OrderID == item.OrderID).OrderByDescending(c => c.COCID).FirstOrDefault();
                    info.COC = coc?.COCID.ToString() ?? "NA";

                    info.SerialNumbers = GetSerialNumbers(item);
                    info.ProductClass = GetProductClass(item);

                    orderInfos.Add(info);
                }
            }

            return orderInfos;
        }

        public List<OrderInfo> GetShippedData(List<int> customerIds, DateTime FromDate, DateTime ToDate)
        {
            // Retrieve orders shipped
            var orderInfos = new List<OrderInfo>();

            using (var dsOrderReport = new OrdersReport { EnforceConstraints = false })
            {
                if (customerIds.Count == 1 && customerIds.Contains(NO_CUSTOMER_ID))
                {
                    OrderTableAdapter taOrders = null;
                    OrderShipmentTableAdapter taOrderShipment = null;
                    COCTableAdapter taCoc = null;

                    try
                    {
                        taOrders = new OrderTableAdapter();
                        taOrderShipment = new OrderShipmentTableAdapter();
                        taCoc = new COCTableAdapter();

                        taOrders.FillByClosedOrders(dsOrderReport.Order, ToDate, FromDate);
                        taOrderShipment.FillByClosedOrders(dsOrderReport.OrderShipment, FromDate, ToDate);
                        taCoc.FillByClosedDate(dsOrderReport.COC, FromDate, ToDate);
                    }
                    finally
                    {
                        taOrders?.Dispose();
                        taOrderShipment?.Dispose();
                        taCoc?.Dispose();
                    }
                }
                else
                {
                    OrderTableAdapter taOrders = null;
                    OrderShipmentTableAdapter taOrderShipment = null;
                    COCTableAdapter taCoc = null;

                    try
                    {
                        taOrders = new OrderTableAdapter { ClearBeforeFill = false };
                        taOrderShipment = new OrderShipmentTableAdapter { ClearBeforeFill = false };
                        taCoc = new COCTableAdapter { ClearBeforeFill = false };

                        foreach (var relatedCustomerId in customerIds)
                        {
                            taOrders.FillClosedOrderByCustomer(dsOrderReport.Order, ToDate, FromDate, relatedCustomerId);
                            taOrderShipment.FillByClosedOrdersByCustomer(dsOrderReport.OrderShipment, ToDate, FromDate, relatedCustomerId);
                            taCoc.FillCOCByClosedDate(dsOrderReport.COC, ToDate, FromDate, relatedCustomerId);
                        }
                    }
                    finally
                    {
                        taOrders?.Dispose();
                        taOrderShipment?.Dispose();
                        taCoc?.Dispose();
                    }
                }

                foreach (OrdersReport.OrderRow item in dsOrderReport.Order.OrderBy(o => o.OrderID))
                {
                    // Lost orders will be added to the 'Issues' report, they are not shipped so don't include
                    if (item.OrderType == (int)OrderType.Lost)
                    {
                        continue;
                    }

                    var info = new OrderInfo
                    {
                        CustomerName = item.CustomerName,
                        OrderID = item.OrderID,
                        PartDescription = item.PartName
                    };

                    if (!item.IsOrderDateNull() && !item.IsCompletedDateNull())
                    {
                        int days = DateUtilities.GetBusinessDays(item.OrderDate, item.CompletedDate);
                        if (days > 0)
                            info.LeadDays = days;
                    }

                    if (!item.IsPurchaseOrderNull())
                        info.PurchaseOrder = item.PurchaseOrder;
                    if (!item.IsPartQuantityNull())
                        info.PartQuantity = item.PartQuantity;
                    if (!item.IsEstShipDateNull())
                        info.EstShipDate = item.EstShipDate;
                    if (!item.IsAdjustedEstShipDateNull())
                        info.AdjustedEstShipDate = item.AdjustedEstShipDate;
                    if (!item.IsCustomerWONull())
                        info.CustomerWO = item.CustomerWO;

                    OrdersReport.COCRow coc = dsOrderReport.COC.Where(c => c.OrderID == item.OrderID).OrderByDescending(c => c.COCID).FirstOrDefault();
                    info.COC = coc?.COCID.ToString() ?? "NA";

                    OrdersReport.OrderShipmentRow shipment = dsOrderReport.OrderShipment.Where(os => os.OrderID == item.OrderID).OrderByDescending(os => os.ShipmentID).FirstOrDefault();
                    info.ShippingCarrer = shipment == null ? "NA" : shipment.ShippingCarrierID;
                    info.TrackingNumber = shipment == null || shipment.IsTrackingNumberNull() ? "NA" : shipment.TrackingNumber;

                    info.SerialNumbers = GetSerialNumbers(item);
                    info.ProductClass = GetProductClass(item);

                    orderInfos.Add(info);
                }
            }

            return orderInfos;
        }

        public List<IssueInfo> GetIssuesData(List<int> customerIds, DateTime FromDate, DateTime ToDate)
        {
            OrdersReport dsOrderReport = null;
            ListsDataSet.d_ReworkReasonDataTable dtReasons = null;

            try
            {
                dsOrderReport = new OrdersReport
                {
                    EnforceConstraints = false
                };

                DateTime fromDate = FromDate.AddDays(-RECEIVED_LEAD_DAYS); //include lead days in received tab

                // Retrieve lost and quarantined orders
                using (var taOrders = new OrderTableAdapter { ClearBeforeFill = false })
                {
                    if (customerIds.Count == 1 && customerIds.Contains(NO_CUSTOMER_ID))
                    {
                        taOrders.FillLostAndQuarantinedOrders(dsOrderReport.Order, fromDate, ToDate);
                    }
                    else
                    {
                        foreach (var relatedCustomerId in customerIds)
                        {
                            taOrders.FillLostAndQuarantinedByCustomerAndDate(dsOrderReport.Order, fromDate, ToDate, relatedCustomerId);
                        }
                    }
                }

                dtReasons = new ListsDataSet.d_ReworkReasonDataTable();
                using (var taReworkReasons = new Data.Datasets.ListsDataSetTableAdapters.d_ReworkReasonTableAdapter())
                    taReworkReasons.Fill(dtReasons);

                var issueInfos = new List<IssueInfo>();

                foreach (var order in dsOrderReport.Order.OrderBy(o => o.OrderID))
                {
                    using (var dt = new OrdersDataSet.InternalReworkDataTable())
                    {
                        using (var internalReworkTableAdapter = new Data.Datasets.OrdersDataSetTableAdapters.InternalReworkTableAdapter())
                        {
                            // Report quirk - do not show internal reworks where the order
                            // is the Rework if this order has any reworks where it's the Original.
                            internalReworkTableAdapter.FillByOriginalOrderID(dt, order.OrderID);

                            if (dt.Rows.Count == 0)
                            {
                                // try getting data by rework ID
                                internalReworkTableAdapter.FillByReworkOrderID(dt, order.OrderID);
                            }
                        }

                        ListsDataSet.d_ReworkReasonRow reason = null;
                        if (dt.Rows.Count > 0)
                        {
                            reason = dtReasons.FindByReworkReasonID(dt[0].ReworkReasonID);
                        }

                        var issueInfo = new IssueInfo
                        {
                            OrderType = (OrderType)order.OrderType,
                            CustomerName = order.CustomerName,
                            PurchaseOrder = order.IsPurchaseOrderNull()
                                ? "NA"
                                : order.PurchaseOrder,
                            EstShipDate = order.IsEstShipDateNull()
                                ? (DateTime?)null
                                : order.EstShipDate,
                            AdjustedEstShipDate = order.IsAdjustedEstShipDateNull()
                                ? (DateTime?)null
                                : order.AdjustedEstShipDate,
                            Reason = reason == null ? "Other" : reason.Name,
                            PartQuantity = order.IsPartQuantityNull()
                                ? (int?)null
                                : order.PartQuantity,
                            HoldLocation = "NA",
                            Process = "NA",
                            SerialNumbers = GetSerialNumbers(order),
                            ProductClass = GetProductClass(order)
                        };


                        if (dt.Count > 0)
                        {
                            // Report quirk - show only the first rework
                            var reworkRow = dt.FirstOrDefault();

                            if (!reworkRow.IsReworkOrderIDNull())
                            {
                                issueInfo.ReworkOrderId = reworkRow.ReworkOrderID;
                            }

                            issueInfo.OriginalOrderId = reworkRow.OriginalOrderID;

                            if (!reworkRow.IsHoldLocationIDNull())
                            {
                                issueInfo.HoldLocation = reworkRow.HoldLocationID;
                            }

                            if (!reworkRow.IsProcessAliasIDNull())
                            {
                                using (var dtProcessAlias = new ProcessesDataset.ProcessAliasDataTable())
                                {
                                    using (var taProcessAlias = new Data.Datasets.ProcessesDatasetTableAdapters.ProcessAliasTableAdapter())
                                    {
                                        taProcessAlias.GetAliasName(dtProcessAlias, reworkRow.ProcessAliasID);
                                    }

                                    if (dtProcessAlias.Count > 0)
                                    {
                                        issueInfo.Process = dtProcessAlias.FirstOrDefault().Name;
                                    }
                                }
                            }
                        }
                        else
                        {
                            issueInfo.ReworkOrderId = order.OrderID;
                        }

                        issueInfos.Add(issueInfo);
                    }
                }

                return issueInfos;
            }
            finally
            {
                dsOrderReport?.Dispose();
                dtReasons?.Dispose();
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _dsOrdersReport.Dispose();
                _taOrderProductClass.Dispose();
                _taOrderSerialNumber.Dispose();
                _isDisposed = true;
            }
        }

        #endregion

        #region OrderInfo

        public class OrderInfo
        {
            #region Properties

            public int PartNumber { get; set; }

            public int PartQuantity { get; set; }

            public int OrderID { get; set; }

            public string CustomerWO { get; set; }

            public string CustomerName { get; set; }

            public string PartDescription { get; set; }

            public string PurchaseOrder { get; set; }

            public string CurrentLocation { get; set; }

            public string CurrentProcess { get; set; }

            public DateTime? CurrentProcessStartDate { get; set; }

            public string Status { get; set; }

            public string COC { get; set; }

            public int? LeadDays { get; set; }

            public int OrderType { get; set; }

            public string ShippingCarrer { get; set; }

            public string TrackingNumber { get; set; }

            public DateTime ReceivedDate { get; set; }

            public DateTime CompletedDate { get; set; }

            public DateTime EstShipDate { get; set; }

            public DateTime? AdjustedEstShipDate { get; set; }

            public List<string> SerialNumbers { get; set; }

            public string ProductClass { get; set; }

            #endregion
        }

        #endregion

        #region IssueInfo

        public class IssueInfo
        {
            #region Properties

            public int? ReworkOrderId { get; set; }

            public OrderType OrderType { get; set; }

            public int? OriginalOrderId { get; set; }

            public string CustomerName { get; set; }

            public string PurchaseOrder { get; set; }

            public string HoldLocation { get; set; }

            public DateTime? EstShipDate { get; set; }

            public DateTime? AdjustedEstShipDate { get; set; }

            public string Reason { get; set; }

            public string Process { get; set; }

            public int? PartQuantity { get; set; }

            public List<string> SerialNumbers { get; set; }

            public string ProductClass { get; set; }

            #endregion
        }

        #endregion
    }
}
