using DWOS.Data;
using DWOS.Data.Reports.ProcessPartsReportTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Reports.ReportData
{
    /// <summary>
    /// Report data for <see cref="TurnAroundTimeReport"/>.
    /// </summary>
    public class TurnAroundTimeData
    {
        #region Properties

        /// <summary>
        /// Gets the start date (inclusive) for this instance.
        /// </summary>
        public DateTime FromDate { get; }

        /// <summary>
        /// Gets the end date (inclusive) for this instance.
        /// </summary>
        public DateTime ToDate { get; }

        /// <summary>
        /// Gets the orders for this instance.
        /// </summary>
        public List<OrderData> Orders { get; }

        /// <summary>
        /// Gets or sets a value that indicates if processing lines should
        /// be visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if processing lines are visible;
        /// otherwise, <c>false</c>
        /// </value>
        public bool ShowProcessingLines { get; }

        #endregion

        #region Methods

        private TurnAroundTimeData(DateTime fromDate, DateTime toDate, bool showProcessingLines, List<OrderData> orders)
        {
            FromDate = fromDate;
            ToDate = toDate;
            ShowProcessingLines = showProcessingLines;
            Orders = orders
                ?? throw new ArgumentNullException(nameof(orders));
        }

        /// <summary>
        /// Retrieves turn around time data from the database.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static TurnAroundTimeData GetReportData(DateTime fromDate, DateTime toDate)
        {
            var orders = new List<OrderData>();

            using (var dsReport = new Data.Reports.ProcessPartsReport())
            {
                using (new UsingDataSetLoad(dsReport))
                {
                    using (var taTurnAroundTime = new TurnAroundTimeTableAdapter())
                    {
                        taTurnAroundTime.Fill(dsReport.TurnAroundTime, fromDate, toDate);
                    }

                    using (var taProcessingLine = new TurnAroundTimeProcessingLineTableAdapter())
                    {
                        taProcessingLine.Fill(dsReport.TurnAroundTimeProcessingLine, fromDate, toDate);
                    }
                }

                foreach (var row in dsReport.TurnAroundTime)
                {
                    orders.Add(new OrderData
                    {
                        OrderId = row.OrderID,
                        SalesOrderId = row.IsSalesOrderIDNull() ? (int?)null : row.SalesOrderID,
                        CustomerName = row.CustomerName,
                        CustomerWorkOrder = row.IsCustomerWONull() ? null : row.CustomerWO,
                        PurchaseOrder = row.IsPurchaseOrderNull() ? null : row.PurchaseOrder,
                        PartName = row.IsPartNameNull() ? "NA" : row.PartName,
                        PartQuantity = row.IsPartQuantityNull() ? (int?)null : row.PartQuantity,
                        OrderDate = row.IsOrderDateNull() ? new DateTime() : row.OrderDate,
                        RequiredDate = row.IsRequiredDateNull() ? (DateTime?)null : row.RequiredDate,
                        EstimatedShipDate = row.IsEstShipDateNull() ? (DateTime?)null : row.EstShipDate,
                        CompletedDate = row.IsCompletedDateNull() ? (DateTime?)null : row.CompletedDate,

                        ProcessingLines = row.GetTurnAroundTimeProcessingLineRows()
                            .Select(procLine => procLine.ProcessingLineName)
                            .ToList()
                    });
                }
            }

            var showProcessingLines = ApplicationSettings.Current.MultipleLinesEnabled;

            return new TurnAroundTimeData(fromDate, toDate, showProcessingLines, orders);
        }

        #endregion

        #region OrderData

        /// <summary>
        /// Represents a single order.
        /// </summary>
        public class OrderData
        {
            #region Properties

            /// <summary>
            /// Gets or sets the order ID for this instance.
            /// </summary>
            public int OrderId { get; set; }

            /// <summary>
            /// Gets or sets the sales order ID for this instance.
            /// </summary>
            public int? SalesOrderId { get; set; }

            /// <summary>
            /// Gets or sets the customer name for this instance.
            /// </summary>
            public string CustomerName { get; set; }

            /// <summary>
            /// Gets or sets the customer work order number for this instance.
            /// </summary>
            public string CustomerWorkOrder { get; set; }

            /// <summary>
            /// Gets or sets the purchase order number for this instance.
            /// </summary>
            public string PurchaseOrder { get; set; }

            /// <summary>
            /// Gets or sets the part name for this instance.
            /// </summary>
            /// <value>
            /// The part's name if found; otherwise, has a placeholder value
            /// that indicates that the order has no part.
            /// </value>
            public string PartName { get; set; }

            /// <summary>
            /// Gets or sets the part quantity for this instance.
            /// </summary>
            /// <value>
            /// <c>null</c> if the part's quantity is not present;
            /// otherwise, the part quantity.
            /// </value>
            public int? PartQuantity { get; set; }

            /// <summary>
            /// Gets the processing lines for this instance.
            /// </summary>
            public List<string> ProcessingLines { get; set; }

            /// <summary>
            /// Gets or sets the order date for this instance.
            /// </summary>
            /// <remarks>
            /// The OrderDate column is nullable, but orders have to have
            /// order dates to appear in the report.
            /// </remarks>
            public DateTime OrderDate { get; set; }

            /// <summary>
            /// Gets or sets the required date for this instance.
            /// </summary>
            /// <value>
            /// <c>null</c> if the order does not have a required date;
            /// otherwise, the required date.
            /// </value>
            public DateTime? RequiredDate { get; set; }

            /// <summary>
            /// Gets or sets the estimated ship date for this instance.
            /// </summary>
            /// <value>
            /// <c>null</c> if the order does not have an estimated ship date;
            /// otherwise, the estimated ship date.
            /// </value>
            public DateTime? EstimatedShipDate { get; set; }

            /// <summary>
            /// Gets or sets the completed date for this instance.
            /// </summary>
            /// <value>
            /// <c>null</c> if the order does not have a completed date;
            /// otherwise, the completed date.
            /// </value>
            public DateTime? CompletedDate { get; set; }

            #endregion
        }

        #endregion
    }
}
