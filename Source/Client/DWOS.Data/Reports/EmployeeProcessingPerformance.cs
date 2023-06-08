using DWOS.Data.Reports.ProcessPartsReportTableAdapters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.Data.Reports
{
    /// <summary>
    /// Retrieves data related to employee processing performance.
    /// </summary>
    /// <remarks>
    /// Many of the methods in this class were designed to be less
    /// resource-intensive (and potentially faster) than the typical typed
    /// DataTable/DataAdapter approach.
    /// </remarks>
    public class EmployeeProcessingPerformance : IDisposable
    {
        #region Fields

        private bool _isDisposed;

        /// <summary>
        /// Behavior for all commands.
        /// </summary>
        /// <remarks>
        /// Save memory, possibly improve performance by not buffering data
        /// </remarks>
        private const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.SequentialAccess;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the connection for this instance.
        /// </summary>
        public SqlConnection Connection { get; }

        /// <summary>
        /// Gets the start date for this instance.
        /// </summary>
        public DateTime FromDate { get; }

        /// <summary>
        /// Gets the end date for this instance.
        /// </summary>
        public DateTime ToDate { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="EmployeeProcessingPerformance"/> class.
        /// </summary>
        /// <param name="fromDate">The start date of the report.</param>
        /// <param name="toDate">The end date of the report.</param>
        public EmployeeProcessingPerformance(DateTime fromDate, DateTime toDate)
        {
            FromDate = fromDate;
            ToDate = toDate;

            Connection = DbConnectionFactory.NewConnection();
            Connection.Open();
        }

        public IDictionary<int, string> RetrieveProductClasses()
        {
            var orderProductClassDict = new Dictionary<int, string>();

            using (var dtProductClass = new ProcessPartsReport.OrderProductClassDataTable())
            {
                // Fill using 4 separate calls - makes it easier to remove any category at a later date
                // while still greatly reducing the number of database requests.
                using (var taProductClass = new OrderProductClassTableAdapter { Connection = Connection, ClearBeforeFill = false })
                {
                    taProductClass.FillByProcessDate(dtProductClass, ToDate, FromDate);
                    taProductClass.FillByInspectionDate(dtProductClass, ToDate, FromDate);
                    taProductClass.FillByDateCertified(dtProductClass, ToDate, FromDate);
                    taProductClass.FillByDateShipped(dtProductClass, ToDate, FromDate);
                }

                foreach (var productClassRow in dtProductClass)
                {
                    if (orderProductClassDict.ContainsKey(productClassRow.OrderID) || productClassRow.IsProductClassNull())
                    {
                        continue;
                    }

                    orderProductClassDict[productClassRow.OrderID] = productClassRow.ProductClass;
                }
            }

            return orderProductClassDict;
        }

        public async Task<ICollection<OrderAmount>> RetrieveOrderAmounts(
            CancellationToken cancellationToken)
        {
            const int timeoutSeconds = 0;
            const string sql = @"
SELECT [Order].OrderID,
       COALESCE([Order].BasePrice, 0),
       COALESCE(OrderProcessTotal.Amount, 0),
       [Order].PriceUnit,
       [Order].PartQuantity,
       [Order].Weight
FROM [Order]
LEFT OUTER JOIN
(
    SELECT OrderID, COALESCE(SUM(Amount), 0) AS Amount
    FROM OrderProcesses
    GROUP BY OrderID
) AS OrderProcessTotal ON [Order].OrderID = OrderProcessTotal.OrderID
WHERE [Order].OrderID IN
(
    SELECT OrderID
    FROM OrderProcesses
    WHERE OrderProcesses.EndDate >= @fromdate AND OrderProcesses.EndDate <= @toDate
);";

            var items = new List<OrderAmount>();

            using (var cmdNormal = Connection.CreateCommand())
            {
                cmdNormal.CommandType = CommandType.Text;
                cmdNormal.CommandText = sql;
                cmdNormal.CommandTimeout = timeoutSeconds;
                cmdNormal.Parameters.AddWithValue("fromDate", FromDate);
                cmdNormal.Parameters.AddWithValue("toDate", ToDate);

                using (var reader = await cmdNormal.ExecuteReaderAsync(COMMAND_BEHAVIOR, cancellationToken))
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(new OrderAmount
                        {
                            OrderId = reader.GetInt32(0),
                            BasePrice = reader.GetDecimal(1),
                            ProcessingBasePrice = reader.GetDecimal(2),
                            PriceUnit = reader.IsDBNull(3)
                                ? OrderPrice.enumPriceUnit.Each
                                : (OrderPrice.enumPriceUnit)Enum.Parse(typeof(OrderPrice.enumPriceUnit), reader.GetString(3)),
                            PartQuantity = reader.IsDBNull(4)
                                ? 0
                                : reader.GetInt32(4),
                            Weight = reader.IsDBNull(5)
                                ? 0M
                                : reader.GetDecimal(5)
                        });
                    }
                }
            }

            return items;
        }

        public async Task<ICollection<EmployeeProcessedItem>> RetrieveEmployeeProcessedAsync(
            CancellationToken cancellationToken)
        {
            // Give these large queries an unlimited time to run.
            const int timeoutSeconds = 0;

            const string selectNormalSql = @"
SELECT DISTINCT OrderProcessAnswer.CompletedBy AS UserID,
    OrderProcessAnswer.OrderID,
    OrderProcesses.EndDate,
    Process.Name AS ProcessName,
    OrderProcesses.Amount,
    OrderProcesses.Department
FROM OrderProcessAnswer
INNER JOIN OrderProcesses ON OrderProcessAnswer.OrderProcessesID = OrderProcesses.OrderProcessesID
INNER JOIN Process ON OrderProcesses.ProcessID = Process.ProcessID
WHERE OrderProcesses.EndDate >= @fromdate AND OrderProcesses.EndDate <= @toDate
    AND CompletedBy IS NOT NULL;";

            const string selectWithoutAnswersSql = @"
SELECT -1 AS UserID,
    OrderProcesses.OrderID,
    OrderProcesses.EndDate,
    Process.Name AS ProcessName,
    OrderProcesses.Amount,
    OrderProcesses.Department
FROM OrderProcesses
INNER JOIN Process ON OrderProcesses.ProcessID = Process.ProcessID
WHERE EndDate >= @fromDate
    AND EndDate <= @toDate
    AND OrderProcesses.OrderProcessesID NOT IN (
        SELECT DISTINCT OrderProcessesID
        FROM OrderProcessAnswer
        WHERE CompletedBy IS NOT NULL
);";

            var items = new List<EmployeeProcessedItem>();

            using (var cmdNormal = Connection.CreateCommand())
            {
                cmdNormal.CommandType = CommandType.Text;
                cmdNormal.CommandText = selectNormalSql;
                cmdNormal.CommandTimeout = timeoutSeconds;
                cmdNormal.Parameters.AddWithValue("fromDate", FromDate);
                cmdNormal.Parameters.AddWithValue("toDate", ToDate);

                using (var reader = await cmdNormal.ExecuteReaderAsync(COMMAND_BEHAVIOR, cancellationToken))
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(FromReader(reader));
                    }
                }
            }

            using (var cmdWithoutAnswers = Connection.CreateCommand())
            {
                cmdWithoutAnswers.CommandType = CommandType.Text;
                cmdWithoutAnswers.CommandText = selectWithoutAnswersSql;
                cmdWithoutAnswers.CommandTimeout = timeoutSeconds;

                cmdWithoutAnswers.Parameters.AddWithValue("fromDate", FromDate);
                cmdWithoutAnswers.Parameters.AddWithValue("toDate", ToDate);

                using (var reader = await cmdWithoutAnswers.ExecuteReaderAsync(COMMAND_BEHAVIOR, cancellationToken))
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(FromReader(reader));
                    }
                }
            }

            return items;

            EmployeeProcessedItem FromReader(SqlDataReader reader)
            {
                return new EmployeeProcessedItem
                {
                    UserId = reader.IsDBNull(0)
                        ? -1
                        : reader.GetInt32(0),
                    OrderId = reader.GetInt32(1),
                    EndDate = reader.IsDBNull(2)
                        ? DateTime.MinValue
                        : reader.GetDateTime(2),
                    ProcessName = reader.GetString(3),
                    ProcessAmount = reader.IsDBNull(4)
                        ? (decimal?)null
                        : reader.GetDecimal(4),
                    Department = reader.GetString(5)
                };
            }
        }

        public async Task<ICollection<DepartmentProcessedItem>> RetrieveDepartmentProcessedAsync(
            CancellationToken cancellationToken)
        {
            const string selectSql = @"
SELECT OrderProcesses.OrderID,
    OrderProcesses.Department,
    OrderProcesses.Amount,
    OrderProcesses.EndDate,
    Process.Name AS ProcessName
FROM OrderProcesses
INNER JOIN Process ON OrderProcesses.ProcessID = Process.ProcessID
WHERE EndDate >= @fromDate
    AND EndDate <= @toDate;";

            var items = new List<DepartmentProcessedItem>();


            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = selectSql;
                cmd.Parameters.AddWithValue("fromDate", FromDate);
                cmd.Parameters.AddWithValue("toDate", ToDate);

                using (var reader = await cmd.ExecuteReaderAsync(COMMAND_BEHAVIOR, cancellationToken))
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(FromReader(reader));
                    }
                }
            }

            return items;

            DepartmentProcessedItem FromReader(SqlDataReader reader)
            {
                return new DepartmentProcessedItem
                {
                    OrderId = reader.GetInt32(0),
                    Department = reader.GetString(1),
                    ProcessAmount = reader.IsDBNull(2)
                        ? (decimal?)null
                        : reader.GetDecimal(2),
                    EndDate = reader.IsDBNull(3)
                        ? (DateTime?)null
                        : reader.GetDateTime(3),
                    ProcessName = reader.GetString(4)
                };
            }
        }

        public async Task<IEnumerable<PartInspectionItem>> RetrieveInspectedAsync(
            CancellationToken cancellationToken)
        {
            const string selectSql = @"
SELECT QAUserID AS UserID, OrderID, InspectionDate
FROM PartInspection
WHERE InspectionDate >= @fromDate AND InspectionDate <= @toDate;";

            var items = new List<PartInspectionItem>();
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = selectSql;
                cmd.Parameters.AddWithValue("fromDate", FromDate);
                cmd.Parameters.AddWithValue("toDate", ToDate);

                using (var reader = await cmd.ExecuteReaderAsync(COMMAND_BEHAVIOR, cancellationToken))
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(FromReader(reader));
                    }
                }
            }

            return items;

            PartInspectionItem FromReader(SqlDataReader reader)
            {
                return new PartInspectionItem
                {
                    UserId = reader.IsDBNull(0)
                        ? -1
                        : reader.GetInt32(0),
                    OrderId = reader.GetInt32(1),
                    InspectionDate = reader.IsDBNull(2)
                        ? DateTime.MinValue
                        : reader.GetDateTime(2)
                };
            }
        }

        public async Task<ICollection<FinalInspectionItem>> RetrieveFinalInspectedAsync(
            CancellationToken cancellationToken)
        {
            const string selectSql = @"
SELECT QAUser AS UserID, OrderID, DateCertified
FROM COC
WHERE DateCertified>= @fromDate AND DateCertified<= @toDate;";

            var items = new List<FinalInspectionItem>();
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = selectSql;
                cmd.Parameters.AddWithValue("fromDate", FromDate);
                cmd.Parameters.AddWithValue("toDate", ToDate);

                using (var reader = await cmd.ExecuteReaderAsync(COMMAND_BEHAVIOR, cancellationToken))
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(FromReader(reader));
                    }
                }
            }

            return items;

            FinalInspectionItem FromReader(SqlDataReader reader)
            {
                return new FinalInspectionItem
                {
                    UserId = reader.IsDBNull(0)
                        ? -1
                        : reader.GetInt32(0),
                    OrderId = reader.GetInt32(1),
                    DateCertified = reader.GetDateTime(2)
                };
            }
        }

        public async Task<ICollection<ShippedItem>> RetrieveShippedAsync(
            CancellationToken cancellationToken)
        {
            const string selectSql = @"
SELECT OrderID, ShippingUserID AS UserID, DateShipped
FROM OrderShipment
WHERE DateShipped >= @fromDate AND DateShipped <= @toDate;";

            var items = new List<ShippedItem>();
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = selectSql;
                cmd.Parameters.AddWithValue("fromDate", FromDate);
                cmd.Parameters.AddWithValue("toDate", ToDate);

                using (var reader = await cmd.ExecuteReaderAsync(COMMAND_BEHAVIOR, cancellationToken))
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(FromReader(reader));
                    }
                }
            }

            return items;

            ShippedItem FromReader(SqlDataReader reader)
            {
                return new ShippedItem
                {
                    OrderId = reader.GetInt32(0),
                    UserId = reader.IsDBNull(1)
                        ? -1
                        : reader.GetInt32(1),

                    DateShipped = reader.IsDBNull(2)
                        ? DateTime.MinValue
                        : reader.GetDateTime(2)
                };
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (!_isDisposed)
            {
                Connection.Close();
                Connection.Dispose();
                _isDisposed = true;
            }
        }

        #endregion

        #region OrderAmount

        public class OrderAmount
        {
            public int OrderId { get; set; }

            public decimal BasePrice { get; internal set; }

            public decimal ProcessingBasePrice { get; internal set; }

            public decimal PriceFactor
            {
                get
                {
                    if (ProcessingBasePrice == 0M)
                    {
                        return 0;
                    }

                    return BasePrice / ProcessingBasePrice;
                }
            }

            public OrderPrice.enumPriceUnit PriceUnit { get; set; }

            public int PartQuantity { get; set; }

            public decimal Weight { get; set; }

            public decimal? CalculateProcessTotal(decimal? processAmount)
            {
                if (processAmount.HasValue)
                {
                    return OrderPrice.CalculatePrice(
                        processAmount.Value * PriceFactor,
                        PriceUnit.ToString(),
                        0M,
                        PartQuantity,
                        Weight);
                }

                return null;
            }
        }

        #endregion

        #region EmployeeProcessedItem

        public class EmployeeProcessedItem
        {
            #region Properties

            public int UserId { get; set; }

            public int OrderId { get; set; }

            public DateTime EndDate { get; set; }

            public string ProcessName { get; set; }

            public decimal? ProcessAmount { get; set; }

            public string Department { get; set; }

            #endregion
        }

        #endregion

        #region DepartmentProcessedItem

        public class DepartmentProcessedItem
        {
            #region Properties

            public int OrderId { get; set; }

            public string Department { get; set; }

            public DateTime? EndDate { get; set; }

            public string ProcessName { get; set; }

            public decimal? ProcessAmount { get; set; }

            #endregion
        }

        #endregion

        #region PartInspectionItem

        public class PartInspectionItem
        {
            #region Properties

            public int UserId { get; set; }

            public int OrderId { get; set; }

            public DateTime InspectionDate { get; set; }

            #endregion
        }

        #endregion

        #region FinalInspectionItem

        public class FinalInspectionItem
        {
            #region Properties

            public int UserId { get; set; }

            public int OrderId { get; set; }

            public DateTime DateCertified { get; set; }

            #endregion
        }

        #endregion

        #region ShippedItem

        public class ShippedItem
        {
            #region Properties

            public int OrderId { get; set; }

            public int UserId { get; set; }

            public DateTime DateShipped { get; set; }

            #endregion
        }

        #endregion
    }
}
