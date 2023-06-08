using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderInvoiceDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DWOS.Data
{
    /// <summary>
    /// Defines helper methods related to invoicing.
    /// </summary>
    public static class InvoiceHelpers
    {
        public static string GetSOInvoice(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            OrderInvoiceDataSet.OrderInvoiceSalesOrderDataTable salesOrders;
            string invoice;

            using (var taInvoiceSalesOrder = new OrderInvoiceSalesOrderTableAdapter())
            {
                salesOrders = taInvoiceSalesOrder.GetBySalesOrderID(order.SalesOrderID);              
            }

            var salesOrder = salesOrders.First();
            invoice = GetInvoiceWithPrefix(order);
            if (salesOrder.IsInvoiceNull())
                return invoice;
            else
            {
                invoice = salesOrder.Invoice;
                return invoice;
            }    

        }

        /// <summary>
        /// Returns an invoice number using the appropriate prefix.
        /// </summary>
        /// <param name="order"></param>
        /// <returns>
        /// An invoice number if order is not <c>null</c>; otherwise, <c>null</c>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="order"/> specifies an invalid invoice level.
        /// </exception>
        public static string GetInvoiceWithPrefix(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            return GetInvoiceWithPrefix(order, ApplicationSettings.Current.InvoiceLevel);
        }

        /// <summary>
        /// Returns an invoice number using the appropriate prefix.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="defaultInvoiceLevel"></param>
        /// <returns>
        /// An invoice number if order is not <c>null</c>; otherwise, <c>null</c>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="order"/> specifies an invalid invoice level.
        /// </exception>
        public static string GetInvoiceWithPrefix(OrderInvoiceDataSet.OrderInvoiceRow order, InvoiceLevelType defaultInvoiceLevel)
        {
            if (order == null)
            {
                return string.Empty;
            }

            int identifier;
            string prefix;

            var invoiceLevel = GetLevel(order, defaultInvoiceLevel);
            if (invoiceLevel == InvoiceLevelType.Default)
            {
                string errorMsg = "A WO ({0}) had an invalid invoice level of 'Default'".FormatWith(order.OrderID);
                throw new ArgumentException(errorMsg, "order");
            }
            else if (invoiceLevel == InvoiceLevelType.SalesOrder)
            {
                if (order.IsSalesOrderIDNull())
                {
                    string errorMsg = "A WO ({0}) without a sales order had an invoice level of 'SalesOrder'".FormatWith(order.OrderID);
                    throw new ArgumentException(errorMsg, "order");
                }

                identifier = order.SalesOrderID;
                prefix = ApplicationSettings.Current.InvoiceSalesOrderPrefix;
            }
            else if (invoiceLevel == InvoiceLevelType.Package)
            {
                identifier = order.IsShipmentPackageIDNull() ? 0 : order.ShipmentPackageID;
                prefix = ApplicationSettings.Current.InvoicePackagePrefix;
            }
            else
            {
                identifier = order.OrderID;
                prefix = ApplicationSettings.Current.InvoiceWorkOrderPrefix;
            }

            if(!String.IsNullOrWhiteSpace(prefix))
            {
                //if prefix contains a special identifier {X}, then X is the max number of digits from the right side to take (trim from left)
                var match = Regex.Match(prefix, "{.}");
                if(match.Success)
                {
                    prefix = prefix.Remove(match.Value); //remove {4} from prefix

                    int maxValues = 0;
                    if (int.TryParse(match.Value.Substring(1, match.Value.Length - 2), out maxValues))
                    {
                        if(identifier.ToString().Length > maxValues)
                            return prefix + identifier.ToString().SubstringFrom(identifier.ToString().Length - maxValues);
                    }
                }

                return prefix + identifier;
            }

            return identifier.ToString();
        }

        public static string GetSOInvoiceWithSuffix(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            var invoiceName = GetInvoiceWithPrefix(order, InvoiceLevelType.SalesOrder);

            const string incrementOne = "-1";
            OrderInvoiceDataSet.OrderInvoiceSalesOrderDataTable salesOrders;
            List<string> invoices = new List<string>();
            List<int> indices = new List<int>();

            using (var taInvoiceSalesOrder = new OrderInvoiceSalesOrderTableAdapter())
            {
                salesOrders = taInvoiceSalesOrder.GetBySalesOrderID(order.SalesOrderID);
            }

            salesOrders.ForEach(so =>
            {
                if (!so.IsInvoiceNull())
                    invoices.Add(so.Invoice);
            });

            if (!invoices.Any()) //no need to modify
                return invoiceName;
            else if (invoices.All(i => i == invoiceName))
                return invoiceName + incrementOne;
            else
            {
                invoices.ForEach(s =>
                {
                    var parsed = s.Split('-');
                    var index = int.TryParse(parsed[parsed.Length - 1], out int result) ? result : 0;
                    if (index != 0)
                        indices.Add(index);
                });
            }

            if (indices.Any())
            {
                var nextIndex = indices.Max() + 1;
                return $"{invoiceName}-{nextIndex}";
            }
            else //Could not auto-increment
                return invoices[0];
        
        }

        /// <summary>
        /// Retrieves the invoice level for the order.
        /// </summary>
        /// <param name="order"></param>
        public static InvoiceLevelType GetLevel(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            return GetLevel(order, ApplicationSettings.Current.InvoiceLevel);
        }

        /// <summary>
        /// Retrieves the invoice level for the order.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="defaultInvoiceLevel"></param>
        /// <returns></returns>
        public static InvoiceLevelType GetLevel(OrderInvoiceDataSet.OrderInvoiceRow order, InvoiceLevelType defaultInvoiceLevel)
        {
            if (order == null)
            {
                return InvoiceLevelType.WorkOrder;
            }

            // Determine level specified by customer
            string customerLevel;
            if (order.OrderInvoiceCustomerRow.InvoiceLevelID.EquivalentTo(InvoiceLevelType.Default.ToString()))
            {
                customerLevel = defaultInvoiceLevel.ToString();
            }
            else
            {
                customerLevel = order.OrderInvoiceCustomerRow.InvoiceLevelID;
            }

            // Cannot use SalesOrder invoicing without sales order
            string invoiceLevel;
            if (customerLevel.EquivalentTo(InvoiceLevelType.SalesOrder.ToString()) && order.IsSalesOrderIDNull())
            {
                invoiceLevel = InvoiceLevelType.WorkOrder.ToString();
            }
            else
            {
                invoiceLevel = customerLevel;
            }

            InvoiceLevelType returnValue;

            if (!Enum.TryParse(invoiceLevel, out returnValue))
            {
                returnValue = InvoiceLevelType.WorkOrder;
            }

            return returnValue;
        }
    }
}
