using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Reports;
using DWOS.Data.Reports.OrdersReportTableAdapters;
using NLog;


namespace DWOS.Portal.Models
{
    /// <summary>
    /// Primary data access class for the orders.
    /// </summary>
    public class OrderDataAccess
    {
        #region Fields

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the connection string for this instance.
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Gets the application settings for this instance.
        /// </summary>
        public ApplicationSettings AppSettings { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDataAccess"/>
        /// class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="appSettings">The application settings.</param>
        public OrderDataAccess(string connectionString, ApplicationSettings appSettings)
        {
            ConnectionString = connectionString;
            AppSettings = appSettings;
        }

        /// <summary>
        /// Asynchronously retrieves order summary data for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IEnumerable<OrderSummary>> GetSummaries(User user)
        {
            var customerIdsString = $"({string.Join(",", user.AllCustomerIds)})";

            // Retrieve COCs for customers
            var orderCocMap = new Dictionary<int, int>();
            var cocCmd = new SqlCommand($@"
                SELECT COC.OrderID, MAX(COC.COCID) AS COCID
                FROM COC
                INNER JOIN [Order] ON [Order].OrderID = COC.OrderID
                WHERE [Order].CustomerID IN {customerIdsString}
                GROUP BY COC.OrderID;");

            using (var da = await ExecuteReader(cocCmd))
            {
                while (da.Read())
                {
                    orderCocMap.Add((int)da["OrderID"], (int)da["COCID"]);
                }
            }

            var cmd = new SqlCommand($@"
                SELECT PartName, ManufacturerID, CustomerName, OrderID, OrderDate,
                       RequiredDate, Status, CompletedDate, Priority, PartQuantity,
                       CurrentLocation, EstShipDate, AdjustedEstShipDate,
                       CustomerWO, PurchaseOrder, CustomerID,
                       TrackingNumber, ShippingCarrierID,
                       WorkStatus, CurrentProcess, CurrentProcessStartDate
                FROM vw_OrderSummary WHERE CustomerID IN {customerIdsString}");

            var getSerialNumbers = await ShowSerialNumbers();
            var orders = new List<OrderSummary>();
            using (var da = await ExecuteReader(cmd))
            {
                while (da.Read())
                {
                    var orderSummary = ReadOrderSummary(da);

                    if (orderSummary.Status == "Closed")
                    {
                        orderCocMap.TryGetValue(orderSummary.OrderId, out var cocId);

                        if (cocId > 0)
                        {
                            orderSummary.CertificationId = cocId;
                            orderSummary.CertificationUrl = $"/api/reports/coc/{cocId}";
                        }
                    }

                    //  When a Work Order is not in processing, show the work status
                    // (or N/A) as the current process.
                    if (orderSummary.WorkStatus == AppSettings.WorkStatusPartMarking)
                    {
                        orderSummary.CurrentProcess = "Part Marking";
                        orderSummary.CurrentProcessStartDate = null;
                    }
                    else if (orderSummary.WorkStatus == AppSettings.WorkStatusFinalInspection)
                    {
                        orderSummary.CurrentProcess = "Final Inspection";
                        orderSummary.CurrentProcessStartDate = null;
                    }
                    else if (orderSummary.WorkStatus == AppSettings.WorkStatusPendingQI)
                    {
                        orderSummary.CurrentProcess = "Control Inspection";
                        orderSummary.CurrentProcessStartDate = null;
                    }
                    else if (orderSummary.WorkStatus == AppSettings.WorkStatusShipping)
                    {
                        orderSummary.CurrentProcess = "Shipping";
                        orderSummary.CurrentProcessStartDate = null;
                    }
                    else if (orderSummary.WorkStatus != AppSettings.WorkStatusInProcess)
                    {
                        orderSummary.CurrentProcess = "N/A";
                        orderSummary.CurrentProcessStartDate = null;
                    }

                    if (getSerialNumbers)
                    {
                        orderSummary.SerialNumbers = await GetSerialNumbers(orderSummary.OrderId);
                    }

                    orders.Add(orderSummary);
                }
            }

            return orders;
        }

        /// <summary>
        /// Asynchronously retrieves full order information for a given WO and user.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<Order> GetOrder(int orderId, User user)
        {
            if (user == null)
            {
                return null;
            }

            var customerIdsString = $"({string.Join(",", user.AllCustomerIds)})";

            var cmd = new SqlCommand($@"
                SELECT PartName, ManufacturerID, CustomerName, OrderID, OrderDate,
                       RequiredDate, Status, CompletedDate, Priority, PartQuantity,
                       CurrentLocation, EstShipDate, AdjustedEstShipDate,
                       CustomerWO, PurchaseOrder, CustomerID,
                       BasePrice, PriceUnit, Weight,
                       TrackingNumber, ShippingCarrierID,
                       WorkStatus, CurrentProcess, CurrentProcessStartDate
                FROM vw_OrderSummary WHERE OrderID = @orderId AND CustomerID IN {customerIdsString}");

            cmd.Parameters.AddWithValue("@orderId", orderId);

            Order order = null;

            using (var da = await ExecuteReader(cmd))
            {
                if (da.Read())
                {
                    var summary = ReadOrderSummary(da);

                    if (summary.Status == "Closed")
                    {
                        var cocId = await GetCoc(summary.OrderId);
                        if (cocId.HasValue)
                        {
                            summary.CertificationId = cocId.Value;
                            summary.CertificationUrl = $"/api/reports/coc/{cocId}";
                        }
                    }

                    //  When a Work Order is not in processing, show the work status
                    // (or N/A) as the current process.
                    if (summary.WorkStatus == AppSettings.WorkStatusPartMarking)
                    {
                        summary.CurrentProcess = "Part Marking";
                        summary.CurrentProcessStartDate = null;
                    }
                    else if (summary.WorkStatus == AppSettings.WorkStatusFinalInspection)
                    {
                        summary.CurrentProcess = "Final Inspection";
                        summary.CurrentProcessStartDate = null;
                    }
                    else if (summary.WorkStatus == AppSettings.WorkStatusPendingQI)
                    {
                        summary.CurrentProcess = "Control Inspection";
                        summary.CurrentProcessStartDate = null;
                    }
                    else if (summary.WorkStatus == AppSettings.WorkStatusShipping)
                    {
                        summary.CurrentProcess = "Shipping";
                        summary.CurrentProcessStartDate = null;
                    }
                    else if (summary.WorkStatus != AppSettings.WorkStatusInProcess)
                    {
                        summary.CurrentProcess = "N/A";
                        summary.CurrentProcessStartDate = null;
                    }

                    order = new Order
                    {
                        PartName = summary.PartName,
                        ManufacturerId = summary.ManufacturerId,
                        CustomerName = summary.CustomerName,
                        OrderId = summary.OrderId,
                        OrderDate = summary.OrderDate,
                        RequiredDate = summary.RequiredDate,
                        Status = summary.Status,
                        CompletedDate = summary.CompletedDate,
                        Priority = summary.Priority,
                        PartQuantity = summary.PartQuantity,
                        CurrentLocation = summary.CurrentLocation,
                        EstShipDate = summary.EstShipDate,
                        CustomerWorkOrder = summary.CustomerWorkOrder,
                        PurchaseOrder = summary.PurchaseOrder,
                        CustomerId = summary.CustomerId,
                        TrackingNumber = summary.TrackingNumber,
                        ShippingCarrier = summary.ShippingCarrier,
                        CertificationId = summary.CertificationId,
                        CertificationUrl = summary.CertificationUrl,
                        BasePrice = da["BasePrice"] == DBNull.Value
                            ? (decimal?)null
                            : Convert.ToDecimal(da["BasePrice"]),
                        PriceUnit = da["PriceUnit"].ToString(),
                        Weight = da["Weight"] == DBNull.Value
                            ? (decimal?)null
                            : Convert.ToDecimal(da["Weight"]),

                        Image = await GetOrderImage(orderId),
                        SerialNumbers = await ShowSerialNumbers()
                            ? await GetSerialNumbers(orderId)
                            : null,
                    };

                    var dtOrder = new OrdersReport.OrderDataTable();
                    using (var taOrders = new OrderTableAdapter())
                    {
                        taOrders.FillByOrder(dtOrder, orderId);

                        if (dtOrder.Count > 0)
                        {
                            var orderRow = dtOrder.First();
                            order.IsOnHold = orderRow.Hold;
                            order.OrderType = (OrderType)orderRow.OrderType;

                            if (orderRow.Hold)
                            {
                                order.HoldReason = taOrders.GetHoldReason(orderId);

                                if (string.IsNullOrEmpty(order.HoldReason))
                                {
                                    order.HoldReason = "Reason for hold not specified.";
                                }
                            }
                        }
                    }

                    order.Processes = await GetOrderProcesses(order);
                    order.Fees = await GetOrderFees(order);
                    order.ShipToAddress = await GetShipToAddress(order);
                }
            }

            return order;
        }

        private async Task<bool> ShowSerialNumbers()
        {
            var cmd = new SqlCommand("SELECT TOP 1 IsRequired | IsVisible FROM Fields WHERE Name = 'Serial Number'");
            var showSerialNumbers = await ExecuteScalar(cmd) as bool? ?? true;
            return showSerialNumbers;
        }

        private OrderSummary ReadOrderSummary(SqlDataReader da)
        {
            var order = new OrderSummary
            {
                PartName = da["PartName"].ToString(),
                ManufacturerId = da["ManufacturerID"].ToString(),
                CustomerName = da["CustomerName"].ToString(),
                OrderId = da["OrderId"] == DBNull.Value ? -1 : Convert.ToInt32(da["OrderId"]),
                OrderDate = da["OrderDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(da["OrderDate"]),
                RequiredDate = da["RequiredDate"] == DBNull.Value ? (DateTime?) null : Convert.ToDateTime(da["RequiredDate"]),
                Status = da["Status"].ToString(),
                CompletedDate = da["CompletedDate"] == DBNull.Value ? (DateTime?) null : Convert.ToDateTime(da["CompletedDate"]),
                Priority = da["Priority"].ToString(),
                PartQuantity = da["PartQuantity"] == DBNull.Value ? (int?) null : Convert.ToInt32(da["PartQuantity"]),
                CurrentLocation = da["CurrentLocation"].ToString(),
                WorkStatus = da["WorkStatus"].ToString(),
                CurrentProcess = da["CurrentProcess"].ToString(),
                CurrentProcessStartDate = da["CurrentProcessStartDate"] == DBNull.Value
                    ? (DateTime?) null
                    : Convert.ToDateTime(da["CurrentProcessStartDate"]),
                EstShipDate = da["AdjustedEstShipDate"] == DBNull.Value
                    ? (da["EstShipDate"] == DBNull.Value ? (DateTime?) null : Convert.ToDateTime(da["EstShipDate"]))
                    : Convert.ToDateTime(da["AdjustedEstShipDate"]),
                CustomerWorkOrder = da["CustomerWO"].ToString(),
                PurchaseOrder = da["PurchaseOrder"].ToString(),
                CustomerId = da["CustomerId"] == DBNull.Value ? -1 : Convert.ToInt32(da["CustomerId"]),
                TrackingNumber = da["TrackingNumber"].ToString(),
                ShippingCarrier = da["ShippingCarrierID"].ToString(),
            };

            return order;
        }


        private async Task<List<string>> GetSerialNumbers(int orderId)
        {
            var cmd = new SqlCommand("SELECT Number FROM OrderSerialNumber WHERE OrderID = @orderId AND Active = 1 ORDER BY PartOrder;");
            cmd.Parameters.AddWithValue("@orderId", orderId);

            var serialNumbers = new List<string>();
            using (var reader = await ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    serialNumbers.Add(reader.GetString(0));
                }
            }

            return serialNumbers;
        }

        private async Task<int?> GetCoc(int orderId)
        {
            Logger.Info("Retrieving COC ID for Order {0}.".FormatWith(orderId));
            var cmd = new SqlCommand("SELECT MAX([COCID]) FROM [COC] WHERE ([OrderID] = @orderId)");
            cmd.Parameters.AddWithValue("@orderId", orderId);

            var cocId = await ExecuteScalar(cmd);
            return cocId == null || cocId == DBNull.Value
                ? (int?) null
                : Convert.ToInt32(cocId);
        }

        private async Task<FileData> GetOrderImage(int orderId)
        {
            using (var da = await ExecuteReader("Get_PartImage", "orderID", orderId))
            {
                if (da.Read())
                {
                    var imgBytes = da["Media"] == DBNull.Value ? null : (byte[])da["Media"];

                    if (imgBytes != null && imgBytes.Length != 0)
                    {
                        return new FileData
                        {
                            Content = Convert.ToBase64String(imgBytes),

                            // Assumption from previous Portal site: image is a PNG
                            Type = "image/png"
                        };
                    }
                }
            }

            return null;
        }

        private async Task<List<OrderProcess>> GetOrderProcesses(Order orderDetails)
        {
            Logger.Info("Retrieving order processes.");

            if (orderDetails == null)
            {
                return null;
            }

            var process = new List<OrderProcess>();

            using (var da = await ExecuteReader("Get_OrderProcessSummaryInfo", "orderID", orderDetails.OrderId))
            {
                while (da.Read())
                {
                    int stepOrder = Convert.ToInt32(da["StepOrder"]);
                    var details = new OrderProcess
                    {
                        StepOrder = stepOrder,
                        Department = da["Department"].ToString(),
                        StartDate = da["StartDate"] == DBNull.Value ? new DateTime?() : Convert.ToDateTime(da["StartDate"]),
                        EndDate = da["EndDate"] == DBNull.Value ? new DateTime?() : Convert.ToDateTime(da["EndDate"])
                    };

                    switch (stepOrder)
                    {
                        case 98: //Part Marking
                            if (da["AliasName"] != DBNull.Value)
                                details.Description = da["AliasName"].ToString();
                            else if (da["ProcessName"] != DBNull.Value)
                                details.Description = da["ProcessName"].ToString();
                            else
                                details.Description = "None";
                            break;
                        case 99: //Shipping
                            if (da["ProcessName"] != DBNull.Value)
                                details.Description = da["ProcessName"].ToString();
                            else if (da["AliasName"] != DBNull.Value)
                                details.Description = da["AliasName"].ToString();
                            else
                                details.Description = "None";
                            break;
                        default: //Processes
                            if (da["CustomerAliasName"] != DBNull.Value)
                                details.Description = da["CustomerAliasName"].ToString();
                            else if (da["AliasName"] != DBNull.Value)
                                details.Description = da["AliasName"].ToString();
                            else if (da["ProcessName"] != DBNull.Value)
                                details.Description = da["ProcessName"].ToString();
                            break;
                    }


                    if (string.IsNullOrEmpty(details.Description))
                        details.Description = "None";

                    process.Add(details);
                }
            }

            // Add info on hold/quarantine/lost if needed
            if (orderDetails.OrderType == OrderType.Normal && orderDetails.IsOnHold)
            {
                var details = new OrderProcess { Description = "On Hold: " + orderDetails.HoldReason + "." };
                process.Add(details);
            }
            else if (orderDetails.OrderType == OrderType.ReworkInt && orderDetails.IsOnHold)
            {
                var details = new OrderProcess { Description = "On Hold: Internal Rework." };
                process.Add(details);
            }
            else if (orderDetails.OrderType == OrderType.Quarantine)
            {
                var details = new OrderProcess { Description = "Order has been quarantined." };
                process.Add(details);
            }
            else if (orderDetails.OrderType == OrderType.Lost)
            {
                var details = new OrderProcess { Description = "Order was lost." };
                process.Add(details);
            }

            return process;
        }

        private async Task<List<OrderFee>> GetOrderFees(Order order)
        {
            if (order == null)
            {
                return null;
            }


            // Retrieve fees
            var cmdFees = new SqlCommand(@"
                SELECT OrderFeeType.OrderFeeTypeID,
                       OrderFeeType.FeeType,
                       OrderFees.Charge
                FROM OrderFees
                INNER JOIN OrderFeeType ON OrderFees.OrderFeeTypeID = OrderFeeType.OrderFeeTypeID
                WHERE OrderFees.OrderID = @orderId;");

            cmdFees.Parameters.AddWithValue("@orderId", order.OrderId);

            var orderFees = new List<OrderFee>();
            using (var readerFees = await ExecuteReader(cmdFees))
            {
                while (readerFees.Read())
                {
                    var feeTypeId = readerFees["OrderFeeTypeID"].ToString();
                    var feeType = readerFees["FeeType"].ToString();
                    var charge = Convert.ToDecimal(readerFees["Charge"]);

                    var feeTotal = OrderPrice.CalculateFees(
                        feeType,
                        charge,
                        order.BasePrice ?? 0M,
                        order.PartQuantity ?? 0,
                        order.PriceUnit,
                        order.Weight ?? 0M);

                    orderFees.Add(new OrderFee
                    {
                        Name = feeTypeId,
                        Total =  feeTotal
                    });
                }
            }

            return orderFees;
        }

        private async Task<Address> GetShipToAddress(Order order)
        {
            if (order == null)
            {
                return null;
            }

            var cmd = new SqlCommand(@"
                SELECT TOP 1 Name,
                             Address1,
                             Address2,
                             City,
                             State,
                             Zip
                FROM CustomerAddress
                WHERE CustomerAddressID IN
                (
                    SELECT CustomerAddressID FROM [Order] WHERE OrderID = @orderId
                );");

            cmd.Parameters.AddWithValue("@orderId", order.OrderId);

            var orderFees = new List<OrderFee>();
            using (var reader = await ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    return new Address
                    {
                        Name = reader["Name"].ToString(),
                        Address1 = reader["Address1"].ToString(),
                        Address2 = reader["Address2"].ToString(),
                        City = reader["City"].ToString(),
                        State = reader["State"].ToString(),
                        Zip = reader["Zip"].ToString()
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Executes the command and returns the first column of the first
        /// row of the result.
        /// </summary>
        /// <param name="cmd">The command to execute.</param>
        /// <returns>query result</returns>
        private async Task<object> ExecuteScalar(SqlCommand cmd)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                cmd.Connection = conn;
                conn.Open();
                return await cmd.ExecuteScalarAsync();
            }
        }

        /// <summary>
        /// Executes the command and returns a <see cref="SqlDataReader"/>
        /// instance.
        /// </summary>
        /// <param name="cmd">The command to execute.</param>
        /// <returns>SQL Data Reader</returns>
        private async Task<SqlDataReader> ExecuteReader(SqlCommand cmd)
        {
            var conn = new SqlConnection(ConnectionString);
            cmd.Connection = conn;
            conn.Open();
            return await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// Executes a stored procedure with a single parameter and returns a
        /// <see cref="SqlDataReader"/> instance.
        /// </summary>
        /// <param name="storedProcedure">The stored procedure.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <returns>Sql Data Reader</returns>
        private async Task<SqlDataReader> ExecuteReader(string storedProcedure, string parameterName, object parameterValue)
        {
            var conn = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand(storedProcedure, conn) { CommandType = CommandType.StoredProcedure };

            if (parameterName != null)
                cmd.Parameters.Add(new SqlParameter(parameterName, parameterValue));

            conn.Open();
            return await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        }

        #endregion
    }
}