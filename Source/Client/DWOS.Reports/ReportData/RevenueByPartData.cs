using DWOS.Data;
using DWOS.Data.Reports;
using DWOS.Data.Reports.OrdersReportTableAdapters;
using System;
using System.Collections.Generic;
using NLog;

namespace DWOS.Reports.ReportData
{
    /// <summary>
    /// Contains data for <see cref="RevenueByPartReport"/>.
    /// </summary>
    public class RevenueByPartData
    {
        #region Properties

        /// <summary>
        /// Gets the list of orders for this instance.
        /// </summary>
        public List<Order> Orders { get; } = new List<Order>();

        #endregion

        #region Methods

        /// <summary>
        /// Gets report data for a range of dates.
        /// </summary>
        /// <param name="from">The starting date.</param>
        /// <param name="to">The ending date.</param>
        /// <returns></returns>
        public static RevenueByPartData GetReportData(DateTime from, DateTime to)
        {
            // Defaults match Revenue By Program Report
            const string defaultManufacturer = "Other";
            const string defaultAirframe = "Other";

            OrdersReport dsOrdersReport = null;
            PartSummaryTableAdapter taPartSummary = null;

            try
            {
                var data = new RevenueByPartData();
                var fromDate = from.StartOfDay();
                var toDate = to.EndOfDay();

                dsOrdersReport = new OrdersReport {EnforceConstraints = false};
                taPartSummary = new PartSummaryTableAdapter() {ClearBeforeFill = false};

                using (var taOrderFees = new OrderFeesTableAdapter())
                {
                    taOrderFees.FillByClosed(dsOrdersReport.OrderFees);
                }

                using (var taFeeType = new OrderFeeTypeTableAdapter())
                {
                    taFeeType.Fill(dsOrdersReport.OrderFeeType);
                }

                using (var taOrder = new OrderTableAdapter())
                {
                    taOrder.FillByClosedOrders(dsOrdersReport.Order, toDate, fromDate);
                }

                foreach (var order in dsOrdersReport.Order)
                {
                    var partId = order.IsPartIDNull() ? -1 : order.PartID;
                    var partSummary = dsOrdersReport.PartSummary.FindByPartID(order.PartID);

                    if (partSummary == null)
                    {
                        taPartSummary.FillByPartId(dsOrdersReport.PartSummary, partId);
                        partSummary = dsOrdersReport.PartSummary.FindByPartID(order.PartID);
                    }

                    if (partSummary == null)
                    {
                        LogManager.GetCurrentClassLogger()
                            .Error("Could not locate part {0} for order {1}", partId, order.OrderID);

                        continue;
                    }

                    var basePrice = order.IsBasePriceNull() ? 0M : order.BasePrice;
                    var priceUnit = order.IsPriceUnitNull()
                        ? OrderPrice.enumPriceUnit.Each
                        : OrderPrice.ParsePriceUnit(order.PriceUnit);

                    var fees = OrderPrice.CalculateFees(order, basePrice);


                    data.Orders.Add(new Order
                    {
                        OrderId = order.OrderID,
                        CustomerName = order.CustomerName,
                        DateCompleted = order.IsCompletedDateNull() ? default(DateTime) : order.CompletedDate,
                        PartId = partId,
                        PartName = order.PartName,
                        Manufacturer = partSummary.IsManufacturerIDNull() ? defaultManufacturer : partSummary.ManufacturerID,
                        Model = partSummary.IsAirframeNull() ? defaultAirframe : partSummary.Airframe,
                        PartQuantity = order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                        Weight = order.IsWeightNull() ? 0M : order.Weight,
                        BasePrice = basePrice,
                        PriceUnit = priceUnit,
                        Fees = fees
                    });
                }

                return data;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting Revenue By Part report data.");
                return null;
            }
            finally
            {
                dsOrdersReport?.Dispose();
                taPartSummary?.Dispose();
            }
        }

        #endregion

        #region Order

        /// <summary>
        /// Represents an order.
        /// </summary>
        public class Order
        {
            #region Properties

            public int OrderId { get; set; }

            public string CustomerName { get; set; }

            public int PartId { get; set; }

            public string PartName { get; set; }

            public string Manufacturer { get; set; }

            public string Model { get; set; }

            public int PartQuantity { get; set; }

            public decimal Weight { get; set; }

            public DateTime DateCompleted { get; set; }

            public decimal Fees { get; set; }

            public decimal BasePrice { get; set; }

            public OrderPrice.enumPriceUnit PriceUnit { get; set; }

            public decimal TotalPrice
                => OrderPrice.CalculatePrice(BasePrice, PriceUnit.ToString(), Fees, PartQuantity, Weight);

            #endregion
        }

        #endregion
    }
}