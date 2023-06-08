using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Datasets.OrderShipmentDataSetTableAdapters;
using DWOS.Shared.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DWOS.Reports.ReportData
{
    /// <summary>
    /// Contains data for <see cref="RepairStatementReport"/>.
    /// </summary>
    public class RepairStatementData
    {
        #region Fields

        private readonly Dictionary<DateTime, List<ReportPart>> _partsByDate =
            new Dictionary<DateTime, List<ReportPart>>();

        private readonly ILogger _log =
            LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public AddressInfo SenderAddress { get; set; }

        public DateTime ShippedDate { get; set; }

        public string CustomerName { get; set; }

        public IEnumerable<DateTime> ReceivedDates => _partsByDate.Keys;

        public UserInfo User { get; set; }

        #endregion

        #region Methods

        private RepairStatementData()
        {

        }

        public IEnumerable<ReportPart> GetParts(DateTime orderDate)
        {
            List<ReportPart> returnList = null;
            if (_partsByDate.TryGetValue(orderDate.Date, out returnList))
            {
                return returnList;
            }

            return Enumerable.Empty<ReportPart>();
        }

        public int GetCapacity(DateTime orderDate)
        {
            List<ReportPart> returnList = null;
            if (_partsByDate.TryGetValue(orderDate.Date, out returnList))
            {
                return returnList.Count;
            }

            return 0;
        }

        private void Add(OrdersDataSet.OrderRow order)
        {
            if (order.IsOrderDateNull())
            {
                return;
            }

            var orderPrice = GetOrderPrice(order);
            var orderFees = GetOrderFees(order);
            var importedPrice = order.IsImportedPriceNull() ? 0M : order.ImportedPrice;
            var orderDescription = GetOrderDescription(order);
            var productClass = GetProductClass(order);

            var serialNumbers = order.GetOrderSerialNumberRows().Where(s => s.Active).ToList();
            if (serialNumbers.Count == 0)
            {
                Add(order.OrderDate, new ReportPart()
                {
                    SerialNumber = "NA",
                    ProductClass = productClass,
                    Description = orderDescription,
                    ImportedPrice = importedPrice,
                    OrderPrice = orderPrice,
                    Fees = orderFees
                });
            }
            else
            {
                foreach (var serial in serialNumbers)
                {
                    Add(order.OrderDate, new ReportPart()
                    {
                        SerialNumber = serial.IsNumberNull() ? string.Empty : serial.Number,
                        ProductClass = productClass,
                        Description = orderDescription,
                        ImportedPrice = importedPrice / serialNumbers.Count,
                        OrderPrice = orderPrice / serialNumbers.Count,
                        Fees = orderFees
                            .Select(fee => new ReportPartFee
                            {
                                Name = fee.Name,
                                TotalPrice = fee.TotalPrice / serialNumbers.Count
                            })
                            .ToList()
                    });
                }
            }
        }

        private void Add(DateTime orderDate, ReportPart part)
        {
            if (part == null)
            {
                return;
            }

            var key = orderDate.Date;
            if (_partsByDate.ContainsKey(key))
            {
                _partsByDate[key].Add(part);
            }
            else
            {
                _partsByDate[key] = new List<ReportPart>() { part };
            }
        }

        public static RepairStatementData GetReportData(int shipmentPackageId, ISecurityUserInfo user)
        {
            var shipmentDate = GetShipmentDate(shipmentPackageId);
            var customerName = GetCustomerName(shipmentPackageId);

            var reportData = new RepairStatementData
            {
                ShippedDate = shipmentDate.Date,
                CustomerName = customerName,
                User = GetUserInfo(user)
            };

            List<int> orderIds;
            using (var taOrderShipment = new Data.Datasets.OrdersDataSetTableAdapters.OrderShipmentTableAdapter())
            {
                orderIds = taOrderShipment.GetByShipment(shipmentPackageId)
                    .Select(i => i.OrderID)
                    .ToList();
            }

            OrdersDataSet.OrderDataTable orders = RetrieveOrders(orderIds);

            foreach (var order in orders)
            {
                reportData.Add(order);
            }

            reportData.SenderAddress = GetSenderAddress(
                reportData._partsByDate.Values.SelectMany(v => v));

            return reportData;
        }

        public static RepairStatementData GetReportData(int shipmentPackageId, OrdersDataSet dsOrder, ISecurityUserInfo user)
        {
            if (dsOrder == null)
            {
                throw new ArgumentNullException(nameof(dsOrder));
            }

            var shipmentDate = GetShipmentDate(shipmentPackageId);
            var customerName = GetCustomerName(shipmentPackageId);

            var reportData = new RepairStatementData
            {
                ShippedDate = shipmentDate.Date,
                CustomerName = customerName,
                User = GetUserInfo(user)
            };


            IList<int> allOrderIds;
            using (var taOrderShipment = new Data.Datasets.OrdersDataSetTableAdapters.OrderShipmentTableAdapter())
            {
                allOrderIds = taOrderShipment.GetByShipment(shipmentPackageId)
                    .Select(i => i.OrderID)
                    .ToList();
            }

            var orders = new List<OrdersDataSet.OrderRow>();
            var ordersToRetrieve = new List<int>();
            foreach (var orderId in allOrderIds)
            {
                var deleted = dsOrder.Order
                    .Select($"OrderID = {orderId}", null, System.Data.DataViewRowState.Deleted)
                    .FirstOrDefault();

                if (deleted != null)
                {
                    continue;
                }

                var orderRow = dsOrder.Order.FindByOrderID(orderId);

                // Assumption: If orderRow does not have any order shipments,
                // it hasn't been fully loaded or edited.
                if (orderRow == null || orderRow.GetOrderShipmentRows().Length == 0)
                {
                    ordersToRetrieve.Add(orderId);
                }
                else
                {
                    orders.Add(orderRow);
                }
            }

            orders.AddRange(RetrieveOrders(ordersToRetrieve));

            foreach (var order in orders)
            {
                reportData.Add(order);
            }

            reportData.SenderAddress = GetSenderAddress(
                reportData._partsByDate.Values.SelectMany(v => v));

            return reportData;
        }

        private static OrdersDataSet.OrderDataTable RetrieveOrders(IEnumerable<int> orderIds)
        {
            OrdersDataSet dsOrders = new OrdersDataSet() { EnforceConstraints = false };
            using (var taOrder = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter { ClearBeforeFill = false })
            {
                foreach (var orderId in orderIds)
                {
                    taOrder.FillByOrderID(dsOrders.Order, orderId);
                }
            }

            using (var taOrderFeeTypes = new OrderFeeTypeTableAdapter())
            {
                taOrderFeeTypes.Fill(dsOrders.OrderFeeType);
            }

            using (var taWorkDescription = new WorkDescriptionTableAdapter())
            {
                taWorkDescription.Fill(dsOrders.WorkDescription);
            }

            using (var taOrderSerial = new Data.Datasets.OrdersDataSetTableAdapters.OrderSerialNumberTableAdapter { ClearBeforeFill = false })
            {
                using (var taOrderFees = new OrderFeesTableAdapter { ClearBeforeFill = false })
                {
                    using (var taOrderWorkDescription = new OrderWorkDescriptionTableAdapter { ClearBeforeFill = false })
                    {
                        using (var taOrderProductClass = new OrderProductClassTableAdapter { ClearBeforeFill = false })
                        {
                            foreach (var order in dsOrders.Order)
                            {
                                taOrderSerial.FillByOrder(dsOrders.OrderSerialNumber, order.OrderID);
                                taOrderFees.FillByOrder(dsOrders.OrderFees, order.OrderID);
                                taOrderWorkDescription.FillByOrder(dsOrders.OrderWorkDescription, order.OrderID);
                                taOrderProductClass.FillByOrder(dsOrders.OrderProductClass, order.OrderID);
                            }
                        }
                    }
                }
            }

            var orders = dsOrders.Order;
            return orders;
        }

        private static DateTime GetShipmentDate(int _shipmentPackageId)
        {
            DateTime shipmentDate;
            using (var taShipment = new Data.Datasets.OrderShipmentDataSetTableAdapters.ShipmentPackageTableAdapter())
            {
                var shipmentRow = taShipment.GetByShipmentPackageID(_shipmentPackageId).FirstOrDefault();

                if (shipmentRow == null)
                {
                    throw new InvalidOperationException("Shipment package does not exist.");
                }

                shipmentDate = shipmentRow.Active ? DateTime.Now : shipmentRow.CloseDate;
            }

            return shipmentDate;
        }

        private static string GetCustomerName(int _shipmentPackageId)
        {
            string customerName;
            using (var taCustomer = new CustomerTableAdapter())
            {
                var customerRow = taCustomer.GetByShipmentPackage(_shipmentPackageId).FirstOrDefault();
                customerName = customerRow?.Name ?? string.Empty;
            }

            return customerName;
        }

        private static decimal GetOrderPrice(OrdersDataSet.OrderRow order)
        {
            var basePrice = order.IsBasePriceNull() ? 0M : order.BasePrice;
            var priceUnit = order.IsPriceUnitNull() ? OrderPrice.enumPriceUnit.Each.ToString() : order.PriceUnit;
            var fees = OrderPrice.CalculateFees(order, basePrice);
            var quantity = order.IsPartQuantityNull() ? 0 : order.PartQuantity;
            var weight = order.IsWeightNull() ? 0 : order.Weight;

            var orderPrice = OrderPrice.CalculatePrice(basePrice, priceUnit, fees, quantity, weight);
            return orderPrice;
        }

        private List<ReportPartFee> GetOrderFees(OrdersDataSet.OrderRow order)
        {
            var basePrice = order.IsBasePriceNull() ? 0M : order.BasePrice;
            var priceUnit = order.IsPriceUnitNull() ? OrderPrice.enumPriceUnit.Each.ToString() : order.PriceUnit;
            var quantity = order.IsPartQuantityNull() ? 0 : order.PartQuantity;
            var weight = order.IsWeightNull() ? 0 : order.Weight;

            var fees = new List<ReportPartFee>();

            foreach (var orderFee in order.GetOrderFeesRows())
            {
                if (orderFee.Charge <= 0)
                {
                    // Do not show discounts.
                    continue;
                }

                fees.Add(new ReportPartFee
                {
                    Name = orderFee.OrderFeeTypeID,
                    TotalPrice = OrderPrice.CalculateFees(
                        orderFee.OrderFeeTypeRow.FeeType,
                        orderFee.Charge,
                        basePrice,
                        quantity,
                        priceUnit,
                        weight)
                });
            }

            return fees;
        }

        private string GetOrderDescription(OrdersDataSet.OrderRow order)
        {
            if (order == null)
            {
                return string.Empty;
            }

            // Retrieve work description, if available
            if (order.GetOrderWorkDescriptionRows().Length > 0)
            {
                var workDescriptionRow = order
                    .GetOrderWorkDescriptionRows()[0]
                    .WorkDescriptionRow;

                if (workDescriptionRow != null)
                {
                    return workDescriptionRow.Description;
                }
            }

            // Fall back to previous 'part description' implementation
            _log.Warn($"Could not find work description for WO #{order.OrderID}.");
            return GetPartDescription(order);
        }

        private static string GetPartDescription(OrdersDataSet.OrderRow order)
        {
            if (order.IsPartIDNull())
            {
                return string.Empty;
            }

            var dtPart = new PartsDataset.PartDataTable();
            using (var taPart = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
            {
                taPart.FillByPartID(dtPart, order.PartID);
            }

            if (dtPart.Count == 0)
            {
                return string.Empty;
            }

            var part = dtPart.First();

            return part.IsDescriptionNull() ? part.Name : $"{part.Name} - {part.Description}";
        }

        private static UserInfo GetUserInfo(ISecurityUserInfo user)
        {
            if (user == null)
            {
                return null;
            }


            using (var dsSecurity = new SecurityDataSet { EnforceConstraints = false })
            {
                using (new UsingDataSetLoad(dsSecurity))
                {
                    using (var taUsers = new Data.Datasets.SecurityDataSetTableAdapters.UsersTableAdapter())
                    {
                        taUsers.FillByUserID(dsSecurity.Users, user.UserID);
                    }

                    var userRow = dsSecurity.Users.FirstOrDefault();

                    if (userRow == null)
                    {
                        return null;
                    }
                    else
                    {
                        if (!userRow.IsSignatureMediaIDNull())
                        {
                            using (var taMedia = new Data.Datasets.SecurityDataSetTableAdapters.MediaTableAdapter())
                            {
                                taMedia.FillByIdWOMedia(dsSecurity.Media, userRow.SignatureMediaID);
                            }
                        }

                        var mediaRow = dsSecurity.Media.FirstOrDefault();

                        Image signature = null;

                        if (mediaRow != null)
                        {
                            signature = MediaUtilities.GetImage(
                                mediaRow.MediaID,
                                mediaRow.IsFileExtensionNull() ? null : mediaRow.FileExtension);
                        }

                        return new UserInfo
                        {
                            Name = userRow.Name,
                            Title = userRow.IsTitleNull() ? null : userRow.Title,
                            Signature = signature
                        };
                    }
                }
            }
        }

        public static string GetProductClass(OrdersDataSet.OrderRow order)
        {
            if (order == null)
            {
                return null;
            }

            var productClassRow = order.GetOrderProductClassRows().FirstOrDefault();

            return productClassRow == null || productClassRow.IsProductClassNull()
                ? string.Empty
                : productClassRow.ProductClass;
        }

        private static AddressInfo GetSenderAddress(IEnumerable<ReportPart> parts)
        {
            var productClasses = parts
                .Select(p => p.ProductClass)
                .Distinct()
                .ToList();

            if (productClasses.Count == 0)
            {
                LogManager.GetCurrentClassLogger().Warn("No product classes found.");
            }
            else if (productClasses.Count > 1)
            {
                LogManager.GetCurrentClassLogger().Warn("Multiple product classes found.");
            }

            var productClassAddress = productClasses
                .OrderBy(pc => pc).Select(GetProductClassAddress)
                .Where(addr => addr != null)
                .FirstOrDefault();

            return productClassAddress ?? GetDefaultAddress();
        }

        private static AddressInfo GetProductClassAddress(string productClass)
        {
            if (string.IsNullOrEmpty(productClass))
            {
                return null;
            }

            AddressInfo address = null;

            using (var taProductClass = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.ProductClassTableAdapter())
            {
                using (var dtProductClass = taProductClass.GetByName(productClass))
                {
                    var productClassRow = dtProductClass.FirstOrDefault();

                    if (productClassRow != null && !productClassRow.IsAddress1Null())
                    {
                        address = new AddressInfo
                        {
                            Address1 = productClassRow.Address1,
                            Address2 = productClassRow.IsAddress2Null() ? string.Empty : productClassRow.Address2,
                            City = productClassRow.IsCityNull() ? string.Empty : productClassRow.City,
                            State = productClassRow.IsStateNull() ? string.Empty : productClassRow.State,
                            Zip = productClassRow.IsZipNull() ? string.Empty : productClassRow.Zip,
                        };
                    }
                }
            }

            return address;
        }

        private static AddressInfo GetDefaultAddress()
        {
            var appSettings = ApplicationSettings.Current;

            return new AddressInfo
            {
                Address1 = appSettings.CompanyAddress1,
                Address2 = string.Empty,
                City = appSettings.CompanyCity,
                State = appSettings.CompanyState,
                Zip = appSettings.CompanyZip
            };
        }

        #endregion

        #region ReportPart

        public class ReportPart
        {
            #region Properties

            public string SerialNumber { get; set; }

            public string ProductClass { get; set; }

            public string Description { get; set; }

            public decimal ImportedPrice { get; set; }

            public decimal OrderPrice { get; set; }

            public decimal TotalPrice => ImportedPrice + OrderPrice;

            public List<ReportPartFee> Fees { get; set; }

            #endregion
        }

        #endregion

        #region ReportPartFee

        public class ReportPartFee
        {
            #region Properties

            public string Name { get; set; }

            public decimal TotalPrice { get; set; }

            #endregion
        }

        #endregion

        #region UserInfo

        public class UserInfo
        {
            public string Name { get; set; }

            public string Title { get; set; }

            public Image Signature { get; set; }
        }

        #endregion
    }
}
