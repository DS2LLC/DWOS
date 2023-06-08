using System;
using System.Data;
using System.Linq;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Reports;
using DWOS.Data.Reports.OrdersReportTableAdapters;

namespace DWOS.Reports
{
    internal sealed class OrderCostData
    {
        #region Properties

        public string Customer { get; set; }

        public int OrderId { get; set; }

        public int? PartQuantity { get; set; }

        public decimal? Revenue { get; set; }

        public double LaborMinutes { get; set; }

        public decimal LaborCost { get; set; }

        public decimal BurdenCost { get; set; }

        public decimal MaterialCost { get; set; }

        #endregion

        #region Methods

        // Assumption: order row has its fees loaded
        public static OrderCostData From(OrdersReport.OrderRow order, DateTime defaultEndTime)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var basePrice = order.IsBasePriceNull() ? 0 : order.BasePrice;


            var partQuantity = order.IsPartQuantityNull() ? (int?)null : order.PartQuantity;

            decimal? total;

            if (order.IsPriceUnitNull() || order.IsPartQuantityNull() || order.IsBasePriceNull())
            {
                total = null;
            }
            else
            {
                total = OrderPrice.CalculatePrice(basePrice,
                    order.IsPriceUnitNull() ? OrderPrice.enumPriceUnit.Each.ToString() : order.PriceUnit,
                    OrderPrice.CalculateFees(order, basePrice),
                    partQuantity ?? 0,
                    order.IsWeightNull() ? 0M : order.Weight);
            }

            var costTotals = OperatorTotals.GetOperatorTotals(order, defaultEndTime);

            using (var taOrder = new OrderTableAdapter())
            {
                return new OrderCostData()
                {
                    Customer = order.CustomerName,
                    OrderId = order.OrderID,
                    PartQuantity = partQuantity,
                    Revenue = total,
                    LaborMinutes = costTotals.Minutes,
                    LaborCost = costTotals.Salary,
                    BurdenCost = costTotals.Burden,
                    MaterialCost = taOrder.GetTotalMaterialCost(order.OrderID) ?? 0M
                };
            }
        }

        #endregion

        #region OperatorTotals

        private sealed class OperatorTotals
        {
            #region Properties

            public decimal Salary
            {
                get;
                set;
            }

            public decimal Burden
            {
                get;
                set;
            }

            public double Minutes
            {
                get;
                set;
            }

            #endregion

            #region Methods

            public static OperatorTotals GetOperatorTotals(OrdersReport.OrderRow order, DateTime defaultEndTime)
            {
                if (order == null)
                {
                    throw new ArgumentNullException(nameof(order));
                }

                SecurityDataSet.UserSalaryDataTable dtUserSalary = null;
                Data.Datasets.SecurityDataSetTableAdapters.UserSalaryTableAdapter taUserSalary = null;
                LaborSummaryTableAdapter taLaborSummary = null;

                try
                {
                    dtUserSalary = new SecurityDataSet.UserSalaryDataTable();
                    taUserSalary = new Data.Datasets.SecurityDataSetTableAdapters.UserSalaryTableAdapter()
                    {
                        ClearBeforeFill = false
                    };

                    taLaborSummary = new LaborSummaryTableAdapter();

                    // Retrieve data
                    var userIds = order.GetLaborSummaryRows().Select(row => row.UserID);

                    foreach (var userId in userIds)
                    {
                        taUserSalary.FillByUser(dtUserSalary, userId);
                    }

                    // Get the total amount of labor
                    var salaryTotal = 0M;
                    var burdenTotal = 0M;
                    var minutesTotal = 0D;

                    foreach (var laborSummary in order.GetLaborSummaryRows())
                    {
                        var startTime = laborSummary.StartTime;

                        var endTime = laborSummary.IsEndTimeNull() ?
                            defaultEndTime :
                            laborSummary.EndTime;

                        decimal salaryLaborTotal = 0M;
                        decimal burdenLaborTotal = 0M;

                        foreach (var summaryMinutesPerDate in DateUtilities.MinutesGroupedByDay(startTime, endTime))
                        {
                            var summaryDate = summaryMinutesPerDate.Key;
                            var summaryMinutes = summaryMinutesPerDate.Value;

                            // Find salary for user by this date
                            // largest EffectiveDate that is <= date and matches UserID
                            var salaryData = dtUserSalary
                                .Where(salary => salary.UserID == laborSummary.UserID && salary.EffectiveDate <= summaryDate)
                                .OrderByDescending(salary => salary.EffectiveDate)
                                .FirstOrDefault();

                            if (salaryData != null)
                            {
                                salaryLaborTotal += summaryMinutes * (salaryData.Salary / DateUtilities.MINUTES_PER_HOUR);
                                burdenLaborTotal += summaryMinutes * (salaryData.Burden / DateUtilities.MINUTES_PER_HOUR);
                            }
                        }

                        var minutesLaborTotal = (endTime - startTime).TotalMinutes;

                        if (!laborSummary.IsBatchProcessIDNull() && (salaryLaborTotal != 0M || burdenLaborTotal != 0M))
                        {
                            // Split price
                            var totalQty = taLaborSummary.GetTotalBatchQuantity(laborSummary.BatchProcessID);
                            var qtyInBatch = taLaborSummary.GetQuantityInBatch(laborSummary.OrderID, laborSummary.BatchProcessID);

                            var fractionToCount = Convert.ToDecimal(qtyInBatch) / Convert.ToDecimal(totalQty);

                            salaryLaborTotal = salaryLaborTotal * fractionToCount;
                            burdenLaborTotal = burdenLaborTotal * fractionToCount;
                            minutesLaborTotal = minutesLaborTotal * Convert.ToDouble(fractionToCount);
                        }

                        salaryTotal += salaryLaborTotal;
                        burdenTotal += burdenLaborTotal;
                        minutesTotal += minutesLaborTotal;
                    }

                    return new OperatorTotals()
                    {
                        Burden = burdenTotal,
                        Salary = salaryTotal,
                        Minutes = minutesTotal
                    };
                }
                finally
                {
                    dtUserSalary?.Dispose();
                    taUserSalary?.Dispose();
                    taLaborSummary?.Dispose();
                }
            }

            #endregion
        }

        #endregion
    }
}