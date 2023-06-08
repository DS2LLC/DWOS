using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data;
using DWOS.Data.Reports.ProcessPartsReportTableAdapters;

namespace DWOS.Reports.ReportData
{
    public class DeliveryPerformanceData
    {
        #region Fields

        private const string UNKNOWN_PRODUCT_CLASS = "N/A";

        #endregion

        #region Properties

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public string ProductClass { get; }

        public List<Order> Orders { get; } = new List<Order>();

        public List<ProductClassSummary> OrderProductClasses => GetOrderProductClassSummary();

        public List<ProductClassSummary> PartProductClasses => GetPartProductClassSummary();

        #endregion

        #region Methods

        private DeliveryPerformanceData(DateTime fromDate, DateTime toDate, string productClass)
        {
            FromDate = fromDate;
            ToDate = toDate;
            ProductClass = productClass;
        }

        private List<ProductClassSummary> GetOrderProductClassSummary()
        {
            var summaries = new List<ProductClassSummary>();

            foreach (var productClassGroup in Orders.GroupBy(o => o.ProductClass))
            {
                var totalDaysEarly = 0;
                var totalDaysLate = 0;
                var ordersEarly = 0;
                var ordersLate = 0;

                foreach (var order in productClassGroup)
                {
                    var daysDifference = order.DaysDifference;

                    if (order.IsEarly)
                    {
                        totalDaysEarly += daysDifference;
                        ordersEarly += 1;
                    }
                    else if (order.IsLate)
                    {
                        totalDaysLate += daysDifference;
                        ordersLate += 1;
                    }
                }

                var summary = new ProductClassSummary
                {
                    ProductClass = productClassGroup.Key,
                    TotalEarly = ordersEarly,
                    TotalLate = Math.Abs(ordersLate),
                    Total = productClassGroup.Count()
                };

                if (ordersEarly != 0)
                {
                    summary.AverageDaysEarly = (int) Math.Round(totalDaysEarly/(double) ordersEarly);
                }

                if (ordersLate != 0)
                {
                    summary.AverageDaysLate = (int) Math.Round(Math.Abs(totalDaysLate/(double) ordersLate));
                }

                summaries.Add(summary);
            }

            return summaries;
        }

        private List<ProductClassSummary> GetPartProductClassSummary()
        {
            var summaries = new List<ProductClassSummary>();
            foreach (var productClassGroup in Orders.GroupBy(o => o.ProductClass))
            {
                var totalDaysEarly = 0;
                var totalDaysLate = 0;
                var partsEarly = 0;
                var partsLate = 0;

                foreach (var order in productClassGroup)
                {
                    var daysDifference = order.DaysDifference;

                    if (order.IsEarly)
                    {
                        totalDaysEarly += daysDifference;
                        partsEarly += order.PartQuantity;
                    }
                    else if (order.IsLate)
                    {
                        totalDaysLate += daysDifference;
                        partsLate += order.PartQuantity;
                    }
                }

                var summary = new ProductClassSummary
                {
                    ProductClass = productClassGroup.Key,
                    TotalEarly = partsEarly,
                    TotalLate = Math.Abs(partsLate),
                    Total = productClassGroup.Sum(o => o.PartQuantity)
                };

                if (partsEarly != 0)
                {
                    summary.AverageDaysEarly = (int)Math.Round(totalDaysEarly / (double)partsEarly);
                }

                if (partsLate != 0)
                {
                    summary.AverageDaysLate = (int)Math.Round(Math.Abs(totalDaysLate / (double)partsLate));
                }

                summaries.Add(summary);
            }

            return summaries;
        }

        public static DeliveryPerformanceData GetReportData(DateTime fromDate, DateTime toDate, string productClass = null)
        {
            var data = new DeliveryPerformanceData(fromDate.StartOfDay(), toDate.EndOfDay(), productClass);
            data.Load();
            return data;
        }

        private void Load()
        {
            Data.Reports.ProcessPartsReport dtReport = null;
            DeliveryPerformanceTableAdapter taDeliveryPerformance = null;
            OrderSerialNumberTableAdapter taSerialNumber = null;

            try
            {
                dtReport = new Data.Reports.ProcessPartsReport
                {
                    EnforceConstraints = false
                };


                taDeliveryPerformance = new DeliveryPerformanceTableAdapter();
                taSerialNumber = new OrderSerialNumberTableAdapter {ClearBeforeFill = false};

                if (string.IsNullOrEmpty(ProductClass))
                {
                    taDeliveryPerformance.Fill(dtReport.DeliveryPerformance, FromDate, ToDate);
                }
                else
                {
                    taDeliveryPerformance.FillByProductClass(dtReport.DeliveryPerformance, FromDate, ToDate, ProductClass);
                }

                foreach (var order in dtReport.DeliveryPerformance)
                {
                    taSerialNumber.FillActive(dtReport.OrderSerialNumber, order.OrderID);

                    var orderData = new Order
                    {
                        OrderId = order.OrderID,
                        Customer = order.CustomerName,

                        // Assume that an order is on time if there is no estimate.
                        EstimatedShipDate = (order.IsEstShipDateNull() ? order.ActualShipDate : order.EstShipDate).Date,
                        ActualShipDate = order.ActualShipDate.Date,
                        PartQuantity = order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                        PartNumber = order.IsPartNameNull() ? "N/A" : order.PartName,
                        ProductClass = order.IsPrimaryProductClassNull() ? UNKNOWN_PRODUCT_CLASS : order.PrimaryProductClass,
                        SerialNumbers = order.GetOrderSerialNumberRows()
                            .Where(s => !(s.IsNumberNull() || string.IsNullOrEmpty(s.Number)))
                            .Select(s => s.Number)
                            .ToList()
                    };

                    Orders.Add(orderData);
                }
            }
            finally
            {
                dtReport?.Dispose();
                taDeliveryPerformance?.Dispose();
                taSerialNumber?.Dispose();
            }
        }

        #endregion

        #region Order

        public class Order
        {
            public int OrderId { get; set; }

            public string Customer { get; set; }

            public string PartNumber { get; set; }

            public int PartQuantity { get; set; }

            public DateTime EstimatedShipDate { get; set; }

            public DateTime ActualShipDate { get; set; }

            public string ProductClass { get; set; }

            /// <summary>
            /// Gets the number of days between estimated ship date and actual ship date.
            /// </summary>
            /// <value>
            /// Positive integer if estimate is later than actual; negative
            /// for late orders; 0 for on-time orders.
            /// </value>
            public int DaysDifference => (EstimatedShipDate.Date - ActualShipDate.Date).Days;

            public bool IsEarly => DaysDifference > 0;

            public bool IsLate => DaysDifference < 0;

            public List<string> SerialNumbers { get; set; }
        }

        #endregion

        #region ProductClassSummary

        public class ProductClassSummary
        {
            public string ProductClass { get; set; }

            public int TotalLate { get; set; }

            public int AverageDaysLate { get; set; }

            public int TotalEarly { get; set; }

            public int AverageDaysEarly { get; set; }

            public int Total { get; set; }
        }

        #endregion

    }
}