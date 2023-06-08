using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using DWOS.Reports;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Order;
using DWOS.Data.Reports;

namespace DWOS.UI.Reports
{
    internal static class LabelReportUtilities
    {
        public const string FIXTURE_WEIGHT_FORMAT = "N0";
        private const string CITY_STATE_ZIP_FORMAT = "{0}, {1} {2}";

        public static IEnumerable<LabelFactory.TokenValue> ReworkTokensForProcess(
            OrderProcessingDataSet.OrderProcessesRow orderProcess,
            OrdersDataSet.OrderRow order)
        {
            int? fixtureCount = GetFixtureCount(orderProcess, order);

            var fixtureCountString = fixtureCount.HasValue ?
                fixtureCount.Value.ToString() :
                "Unknown";

            decimal? weightPerFixture = null;

            if (fixtureCount.HasValue)
            {
                weightPerFixture = GetFixtureWeight(order, fixtureCount.Value);
            }

            var fixtureWeightString = weightPerFixture.HasValue ?
                weightPerFixture.Value.ToString(FIXTURE_WEIGHT_FORMAT) :
                "Unknown";

            var tokens = new List<LabelFactory.TokenValue>();

            switch (orderProcess.StepOrder)
            {
                case 1:
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.STEPORDER1, orderProcess.StepOrder.ToString()));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME1, orderProcess.ProcessRow.Name));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT1, orderProcess.Department));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES1, fixtureCountString));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT1, fixtureWeightString));
                    break;
                case 2:
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.STEPORDER2, orderProcess.StepOrder.ToString()));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME2, orderProcess.ProcessRow.Name));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT2, orderProcess.Department));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES2, fixtureCountString));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT2, fixtureWeightString));
                    break;
                case 3:
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.STEPORDER3, orderProcess.StepOrder.ToString()));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME3, orderProcess.ProcessRow.Name));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT3, orderProcess.Department));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES3, fixtureCountString));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT3, fixtureWeightString));
                    break;
                case 4:
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.STEPORDER4, orderProcess.StepOrder.ToString()));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME4, orderProcess.ProcessRow.Name));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT4, orderProcess.Department));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES4, fixtureCountString));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT4, fixtureWeightString));
                    break;
                case 5:
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.STEPORDER5, orderProcess.StepOrder.ToString()));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME5, orderProcess.ProcessRow.Name));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT5, orderProcess.Department));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES5, fixtureCountString));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT5, fixtureWeightString));
                    break;
                case 6:
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.STEPORDER6, orderProcess.StepOrder.ToString()));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME6, orderProcess.ProcessRow.Name));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT6, orderProcess.Department));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES6, fixtureCountString));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT6, fixtureWeightString));
                    break;
                case 7:
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.STEPORDER7, orderProcess.StepOrder.ToString()));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME7, orderProcess.ProcessRow.Name));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT7, orderProcess.Department));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES7, fixtureCountString));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT7, fixtureWeightString));
                    break;
                case 8:
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.STEPORDER8, orderProcess.StepOrder.ToString()));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME8, orderProcess.ProcessRow.Name));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT8, orderProcess.Department));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES8, fixtureCountString));
                    tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT8, fixtureWeightString));
                    break;
            }

            return tokens;
        }

        public static CustomersDataset.CustomerRow GetCustomer(OrdersDataSet.OrderRow order)
        {
            using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
            {
                var customers = new CustomersDataset.CustomerDataTable();
                ta.FillBy(customers, order.CustomerID);

                return customers.FindByCustomerID(order.CustomerID);
            }
        }

        public static CustomersDataset.CustomerRow GetCustomer(OrdersReport.OrderRow order)
        {
            using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
            {
                var customers = new CustomersDataset.CustomerDataTable();
                ta.FillByOrderID(customers, order.OrderID);

                return customers.FirstOrDefault();
            }
        }

        public static OrdersDataSet.OrderSerialNumberRow GetSerialNumber(OrdersDataSet.OrderRow order)
        {
            var serialNumbers = order.GetOrderSerialNumberRows();
            if (serialNumbers.Length > 0)
            {
                return serialNumbers.OrderBy(s => s.PartOrder).FirstOrDefault(s => s.Active);
            }

            return null;
        }

        public static OrdersDataSet.OrderSerialNumberRow GetSerialNumber(OrdersReport.OrderRow order)
        {
            using (var taOrderSerialNumber = new OrderSerialNumberTableAdapter())
            {
                return taOrderSerialNumber.GetActiveByOrder(order.OrderID)
                    .OrderBy(s => s.PartOrder)
                    .FirstOrDefault();
            }
        }

        public static CustomersDataset.CustomerAddressRow GetCustomerAddress(OrdersDataSet.OrderRow order)
        {
            using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.CustomerAddressTableAdapter())
            {
                var addresses = new CustomersDataset.CustomerAddressDataTable();
                ta.FillByOrderID(addresses, order.OrderID);

                return addresses.FirstOrDefault();
            }
        }

        public static IEnumerable<LabelFactory.TokenValue> AddressTokens(CustomersDataset.CustomerRow customer)
        {
            if (customer.HasBillingAddress)
            {
                return new List<LabelFactory.TokenValue>
                {
                    LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERADDRESS1,
                        customer.IsAddress1Null() ? string.Empty : customer.Address1),

                    LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERADDRESS2,
                        customer.IsAddress2Null() ? string.Empty : customer.Address2),

                    LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERCITYSTATEZIP,
                        CITY_STATE_ZIP_FORMAT.FormatWith(customer.IsCityNull() ? string.Empty : customer.City,
                            customer.IsStateNull() ? string.Empty : customer.State,
                            customer.IsZipNull() ? string.Empty : customer.Zip))
                };
            }

            return Enumerable.Empty<LabelFactory.TokenValue>();
        }

        public static IEnumerable<LabelFactory.TokenValue> AddressTokens(OrderShipmentDataSet.CustomerRow customer)
        {
            return new List<LabelFactory.TokenValue>
            {
                LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERADDRESS1,
                    customer.IsAddress1Null() ? string.Empty : customer.Address1),

                LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERADDRESS2,
                    customer.IsAddress2Null() ? string.Empty : customer.Address2),

                LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERCITYSTATEZIP,
                    CITY_STATE_ZIP_FORMAT.FormatWith(customer.IsCityNull() ? string.Empty : customer.City,
                        customer.IsStateNull() ? string.Empty : customer.State,
                        customer.IsZipNull() ? string.Empty : customer.Zip))
            };
        }

        public static IEnumerable<LabelFactory.TokenValue> ShippingAddressTokens(
            OrderShipmentDataSet.CustomerAddressRow customerAddress)
        {
            if (customerAddress == null)
            {
                return Enumerable.Empty<LabelFactory.TokenValue>();
            }

            return new List<LabelFactory.TokenValue>
            {
                LabelFactory.TokenValue.From(LabelFactory.LabelTokens.SHIPPINGNAME,
                    customerAddress.Name),

                LabelFactory.TokenValue.From(LabelFactory.LabelTokens.SHIPPINGADDRESS1,
                    customerAddress.IsAddress1Null() ? string.Empty : customerAddress.Address1),

                LabelFactory.TokenValue.From(LabelFactory.LabelTokens.SHIPPINGADDRESS2,
                    customerAddress.IsAddress2Null() ? string.Empty : customerAddress.Address2),

                LabelFactory.TokenValue.From(LabelFactory.LabelTokens.SHIPPINGCITYSTATEZIP,
                    CITY_STATE_ZIP_FORMAT.FormatWith(customerAddress.IsCityNull() ? string.Empty : customerAddress.City,
                        customerAddress.IsStateNull() ? string.Empty : customerAddress.State,
                        customerAddress.IsZipNull() ? string.Empty : customerAddress.Zip))
            };
        }

        public static IEnumerable<LabelFactory.TokenValue> ShippingAddressTokens(CustomersDataset.CustomerAddressRow customerAddress)
        {
            return new List<LabelFactory.TokenValue>
            {
                LabelFactory.TokenValue.From(LabelFactory.LabelTokens.SHIPPINGNAME,
                    customerAddress.Name),

                LabelFactory.TokenValue.From(LabelFactory.LabelTokens.SHIPPINGADDRESS1,
                    customerAddress.IsAddress1Null() ? string.Empty : customerAddress.Address1),

                LabelFactory.TokenValue.From(LabelFactory.LabelTokens.SHIPPINGADDRESS2,
                    customerAddress.IsAddress2Null() ? string.Empty : customerAddress.Address2),

                LabelFactory.TokenValue.From(LabelFactory.LabelTokens.SHIPPINGCITYSTATEZIP,
                    CITY_STATE_ZIP_FORMAT.FormatWith(customerAddress.IsCityNull() ? string.Empty : customerAddress.City,
                        customerAddress.IsStateNull() ? string.Empty : customerAddress.State,
                        customerAddress.IsZipNull() ? string.Empty : customerAddress.Zip))
            };
        }

        public static int? GetFixtureCount(OrderProcessingDataSet.OrderProcessesRow orderProcess, OrdersDataSet.OrderRow order)
        {
            var capacity = LoadCapacity.FromOrderProcess(order, orderProcess) ??
                LoadCapacity.FromMatchingPartProcess(order, orderProcess.OrderProcessesID) ??
                LoadCapacity.FromProcess(order, orderProcess.ProcessID);

            return capacity?.FixtureCount;
        }

        public static decimal? GetFixtureWeight(OrdersDataSet.OrderRow order, int fixtureCount)
        {
            decimal? weightPerFixture = null;
            if (!order.IsWeightNull() && fixtureCount > 0)
            {
                weightPerFixture = order.Weight / Convert.ToDecimal(fixtureCount);
            }

            return weightPerFixture;
        }

        public static string GetPartDescription(OrdersDataSet.OrderRow order)
        {
            string partDescription = String.Empty;

            if (!order.IsPartIDNull())
            {
                using (var taParts = new DWOS.Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
                {
                    using (var dtPart = new DWOS.Data.Datasets.PartsDataset.PartDataTable())
                    {
                        taParts.FillByPartID(dtPart, order.PartID);
                        partDescription = dtPart.Count > 0 && !dtPart[0].IsDescriptionNull() ? dtPart[0].Description : String.Empty;
                    }
                }
            }

            return partDescription;
        }

        public static string GetPartDescription(OrdersReport.OrderRow order)
        {
            string partDescription = String.Empty;

            if (!order.IsPartIDNull())
            {
                using (var taParts = new DWOS.Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
                {
                    using (var dtPart = new DWOS.Data.Datasets.PartsDataset.PartDataTable())
                    {
                        taParts.FillByPartID(dtPart, order.PartID);
                        partDescription = dtPart.Count > 0 && !dtPart[0].IsDescriptionNull() ? dtPart[0].Description : String.Empty;
                    }
                }
            }

            return partDescription;
        }

        public static LabelFactory.TokenValue PartRevision(int partId)
        {
            using (var taParts = new DWOS.Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
            {
                using (var dtPart = new DWOS.Data.Datasets.PartsDataset.PartDataTable())
                {
                    taParts.FillByPartID(dtPart, partId);

                    var part = dtPart.FirstOrDefault();

                    var revision = string.Empty;
                    if (part != null && !part.IsRevisionNull())
                    {
                        revision = part.Revision;
                    }
                    return new LabelFactory.TokenValue()
                    {
                        TokenName = LabelFactory.LabelTokens.PARTREVISION.ToString(),
                        Value = revision
                    };
                }
            }
        }

        public static LabelFactory.TokenValue PackageID(int orderID)
        {
            using (var taOrderShipment = new DWOS.Data.Datasets.OrderShipmentDataSetTableAdapters.OrderShipmentTableAdapter())
            {
                using (var dtOrderShipment = new DWOS.Data.Datasets.OrderShipmentDataSet.OrderShipmentDataTable())
                {
                    taOrderShipment.Fill(dtOrderShipment, orderID);
                    DWOS.Data.Datasets.OrderShipmentDataSet.OrderShipmentRow shipment = dtOrderShipment.Where(s => s.OrderID == orderID).FirstOrDefault();
                    return new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.SHIPMENTID.ToString(), Value = shipment != null && !shipment.IsShipmentPackageIDNull() ? shipment.ShipmentPackageID.ToString() : string.Empty };
                }
            }
        }
    }
}
