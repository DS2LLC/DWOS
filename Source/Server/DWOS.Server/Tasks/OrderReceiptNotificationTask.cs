using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Server.Utilities;
using NLog;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using System.Collections.Generic;

namespace DWOS.Server.Tasks
{
    [DisallowConcurrentExecution]
    public class OrderReceiptNotificationTask : IJob
    {
        #region Fields

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly MessagingUtilities Messaging =
            new MessagingUtilities("OrderReceiptNotifications");

        private readonly EmailLogoCopier _copier = new EmailLogoCopier();

        #endregion

        #region Properties

        private bool ProductClassEnabled =>
            FieldUtilities.IsFieldEnabled("Order", "Product Class");

        #endregion

        #region Methods

        private void BeginProcessing()
        {
            try
            {
                _copier.CopyCompanyLogo(Messaging.EmailPickupDirectory);

                // Initial email - sent to contacts
                using (var dsOrders = RetrieveInitialEmailData())
                {
                    var newOrderReceipts = dsOrders
                        .OrderReceiptNotification
                        .Where(orderReceipt => orderReceipt.IsNotificationSentNull())
                        .ToList();

                    foreach (var receipt in newOrderReceipts)
                    {
                        if (QueueEmailNotification(receipt))
                        {
                            receipt.NotificationSent = DateTime.Now;
                        }
                        else
                        {
                            Log.Warn($"Did not send email for Order Receipt " +
                                $"{receipt.OrderReceiptNotificationID} - there may be " +
                                $"0 contacts authorized to receive Order Receipt notifications.");
                        }
                    }

                    using (var taOrderReceipt = new OrderReceiptNotificationTableAdapter())
                    {
                        taOrderReceipt.Update(dsOrders.OrderReceiptNotification);
                    }
                }


            }
            finally
            {
                Messaging.BeginProcessingEmails();
            }
        }

        private OrdersDataSet RetrieveInitialEmailData()
        {
            var dsOrders = new OrdersDataSet
            {
                EnforceConstraints = false
            };

            // Load common data
            using (var taOrderFeeType = new OrderFeeTypeTableAdapter())
            {
                taOrderFeeType.Fill(dsOrders.OrderFeeType);
            }

            // OrderReceipts
            using (var taOrderReceipt = new OrderReceiptNotificationTableAdapter())
            {
                taOrderReceipt.Fill(dsOrders.OrderReceiptNotification);
            }

            // Load orders and associated data
            using (var taOrder = new OrderTableAdapter())
            {
                taOrder.FillOrderReceiptNotification(dsOrders.Order);
            }

            using (var taOrderFee = new OrderFeesTableAdapter { ClearBeforeFill = false })
            {
                foreach (var orderRow in dsOrders.Order)
                {
                    taOrderFee.FillByOrder(dsOrders.OrderFees, orderRow.OrderID);
                }
            }

            using (var taOrderSerialNumbers = new OrderSerialNumberTableAdapter { ClearBeforeFill = false })
            {
                foreach (var orderRow in dsOrders.Order)
                {
                    taOrderSerialNumbers.FillActiveByOrder(dsOrders.OrderSerialNumber, orderRow.OrderID);
                }
            }

            using (var taParts = new PartSummaryTableAdapter { ClearBeforeFill = false })
            {
                foreach (var orderRow in dsOrders.Order)
                {
                    if (orderRow.IsPartIDNull())
                    {
                        continue;
                    }

                    if (dsOrders.PartSummary.FindByPartID(orderRow.PartID) != null)
                    {
                        continue;
                    }

                    taParts.FillByPart(dsOrders.PartSummary, orderRow.PartID);
                }
            }

            return dsOrders;
        }

               
        private bool QueueEmailNotification(OrdersDataSet.OrderReceiptNotificationRow  orderReceipt)
        {
            try
            {
                if (orderReceipt == null || orderReceipt.OrderRow == null)
                {
                    Log.Error("Could not find Order Receipt and/or its order.");
                    return false;
                }

                var dtCustomerShippingSummary = new OrdersDataSet.CustomerShippingSummaryDataTable();

                using (var taCustomerShippingMethod = new CustomerShippingSummaryTableAdapter { ClearBeforeFill = false })
                {
                    dtCustomerShippingSummary= taCustomerShippingMethod.GetByOrder(orderReceipt.OrderID);
                }
                
                OrdersDataSet.CustomerShippingSummaryRow drCustomerShippingMethod = (OrdersDataSet.CustomerShippingSummaryRow) dtCustomerShippingSummary.Rows[0];
               
                var CustomerShippingMethod = drCustomerShippingMethod.IsNameNull()
                        ? string.Empty
                            : drCustomerShippingMethod.Name;

                var customerId = orderReceipt.OrderRow.CustomerID;

                using (var dsCustomer = new CustomersDataset { EnforceConstraints = false })
                {
                    // Get contacts to send email to
                    using (var taContact = new ContactTableAdapter { ClearBeforeFill = false })
                    {
                        taContact.FillBy(dsCustomer.Contact, customerId);
                        taContact.FillBySecondaryCustomer(dsCustomer.Contact, customerId);
                    }

                    using (var taAdditionalCustomer = new ContactAdditionalCustomerTableAdapter { ClearBeforeFill = false })
                    {
                        taAdditionalCustomer.FillBySecondaryCustomer(dsCustomer.ContactAdditionalCustomer, customerId);
                    }

                   

                    using (var taCustomer = new CustomerTableAdapter { ClearBeforeFill = false })
                    {
           
                        foreach (var contact in dsCustomer.Contact)
                        {
                            taCustomer.FillBy(dsCustomer.Customer, contact.CustomerID);
                        }
                    }

                    using (var taCountry = new CountryTableAdapter { ClearBeforeFill = false})
                    {

                        taCountry.Fill(dsCustomer.Country);
                    }

                    using (var taAddresses = new DWOS.Data.Datasets.CustomersDatasetTableAdapters.CustomerAddressTableAdapter { ClearBeforeFill = false })
                    {

                        taAddresses.Fill(dsCustomer.CustomerAddress);
                    }

                    var contacts = dsCustomer.Contact
                        .Where(c => !c.IsNameNull() && !c.IsEmailAddressNull() && c.Active && c.CustomerRow.Active && c.OrderReceiptNotification)
                        .ToList();

                    var sentEmail = false;

                    if (contacts.Count > 0)
                    {
                        // Retrieve values to use in email
                        var purchaseOrder = orderReceipt.OrderRow.IsPurchaseOrderNull()
                            ? string.Empty
                            : orderReceipt.OrderRow.PurchaseOrder;

                        var serialNumbers = orderReceipt.OrderRow.GetOrderSerialNumberRows()
                            .Where(orderSerialNumber => orderSerialNumber.Active && !orderSerialNumber.IsNumberNull())
                            .Select(orderSerialNumber => orderSerialNumber.Number)
                            .ToList();

                        var partName = orderReceipt.OrderRow.PartSummaryRow == null
                            ? "N/A"
                            : orderReceipt.OrderRow.PartSummaryRow.Name;
                        var partDesc = orderReceipt.OrderRow.PartSummaryRow == null
                            ? "N/A"
                            : orderReceipt.OrderRow.PartSummaryRow.Description;

                        var partrev = orderReceipt.OrderRow.PartSummaryRow.Revision == null
                            ? "N/A"
                            : orderReceipt.OrderRow.PartSummaryRow.Revision;

                        var partQty = orderReceipt.OrderRow.PartQuantity.ToString();
                        var OrderDate = orderReceipt.OrderRow.OrderDate.ToString();
                        var discount = CalculateDiscount(orderReceipt.OrderRow);
                        var subtotal = CalculateSubtotal(orderReceipt.OrderRow) + discount;
                        var feeString = GetFeeString(orderReceipt.OrderRow);
                        var estShipDate = orderReceipt.OrderRow.EstShipDate.ToShortDateString();

                        var total = CalculateTotal(orderReceipt.OrderRow);

                        var CustomerRow = dsCustomer.Customer
                        .Where(c => c.CustomerID == customerId)
                        .ToList();

                        var CustomerName = "&nbsp; ";
                        var CustomerAddress1 = "&nbsp; ";
                        var CustomerAddress2 = "&nbsp; ";
                        var CustomerCity = "&nbsp; ";
                        var CustomerState = "&nbsp; ";
                        var CustomerZip = "&nbsp; ";
                        var CustomerCountry = "&nbsp; ";

                        if (CustomerRow.Count > 0)
                        {
                            CustomerName = (String)CustomerRow[0].Name.ToString();
                            CustomerAddress1 = (String)CustomerRow[0].Address1;
                            CustomerAddress2 = (String)CustomerRow[0].Address2;
                            CustomerCity = (String)CustomerRow[0].City;
                            CustomerState = (String)CustomerRow[0].State;
                            CustomerZip = (String)CustomerRow[0].Zip;

                            var CountryRow = dsCustomer.Country
                            .Where(c => c.CountryID == CustomerRow[0].CountryID)
                            .ToList();

                            CustomerCountry = CountryRow.Count > 0 ? CountryRow[0].Name : "&nbsp;";
                        }

                        var ShippingName = "&nbsp; ";
                        var ShippingAddress1 = "&nbsp; ";
                        var ShippingAddress2 = "&nbsp; ";
                        var ShippingCity = "&nbsp; ";
                        var ShippingState = "&nbsp; ";
                        var ShippingZip = "&nbsp; ";
                        var ShippingCountry = "&nbsp; ";

                        var ShippingRow = dsCustomer.CustomerAddress
                        .Where(c => c.CustomerAddressID == orderReceipt.OrderRow.CustomerAddressID)
                        .ToList();

                        if (ShippingRow.Count > 0)
                        {
                            ShippingName = (String)ShippingRow[0].Name.ToString();
                            ShippingAddress1 = (String)ShippingRow[0].Address1;
                            ShippingAddress2 = (String)ShippingRow[0].Address2;
                            ShippingCity = (String)ShippingRow[0].City;
                            ShippingState = (String)ShippingRow[0].State;
                            ShippingZip = (String)ShippingRow[0].Zip;

                            var CountryRow = dsCustomer.Country
                            .Where(c => c.CountryID == ShippingRow[0].CountryID)
                            .ToList();

                            ShippingCountry = CountryRow.Count > 0 ? CountryRow[0].Name : "&nbsp;";
                        }




                        // Construct email
                        var msgInfo = new MessageInfo
                        {
                            FromAddress = new EmailAddress
                            {
                                Address = ApplicationSettings.Current.EmailFromAddress,
                                DisplayName = "No Reply"
                            },
                            Subject = $"Order Receipt Acknowledgment for WO#:{orderReceipt.OrderID}"
                        };

                        msgInfo.ToAddresses.AddRange(contacts.Select(c => new EmailAddress { Address = c.EmailAddress, DisplayName = c.Name }));

                        var html = new NewReceiptNotificationHtml();
                        html.ReplaceTokens(
                            new HtmlNotification.Token("%WO%", orderReceipt.OrderID.ToString()),
                            new HtmlNotification.Token("%PO%", purchaseOrder),
                            new HtmlNotification.Token("%ORDER_DATE%", OrderDate),
                            new HtmlNotification.Token("%SN%", string.Join(", ", serialNumbers)),
                            new HtmlNotification.Token("%PART_NO%", partName),
                            new HtmlNotification.Token("%PART_QTY%", partQty),
                            new HtmlNotification.Token("%PART_REV%", partrev),
                            new HtmlNotification.Token("%DESCRIPTION%", partDesc),
                            new HtmlNotification.Token("%SHIP_METHOD%", CustomerShippingMethod.ToString()),
                            new HtmlNotification.Token("%CUSTOMER_ADDRESS_1%", CustomerAddress1),
                            new HtmlNotification.Token("%CUSTOMER_ADDRESS_2%", CustomerAddress2),
                            new HtmlNotification.Token("%CUSTOMER%", CustomerName),
                            new HtmlNotification.Token("%CUSTOMER_CITY%", CustomerCity),
                            new HtmlNotification.Token("%CUSTOMER_STATE%", CustomerState),
                            new HtmlNotification.Token("%CUSTOMER_ZIP%", CustomerZip),
                            new HtmlNotification.Token("%CUSTOMER_COUNTRY%", CustomerCountry),
                            new HtmlNotification.Token("%SHIPPING_NAME%", ShippingName),
                            new HtmlNotification.Token("%SHIPPING_ADDRESS_1%", ShippingAddress1),
                            new HtmlNotification.Token("%SHIPPING_ADDRESS_2%", ShippingAddress2),
                            new HtmlNotification.Token("%SHIPPING_CITY%", ShippingCity),
                            new HtmlNotification.Token("%SHIPPING_STATE%", ShippingState),
                            new HtmlNotification.Token("%SHIPPING_ZIP%", ShippingZip),
                            new HtmlNotification.Token("%SHIPPING_COUNTRY%", ShippingCountry),
                            new HtmlNotification.Token("%EST_SHIP%", estShipDate),
                            new HtmlNotification.Token("%EST_SHIP%", CustomerShippingMethod),
                            new HtmlNotification.Token("%BASE_PRICE%", orderReceipt.OrderRow.BasePrice.ToString()),
                            new HtmlNotification.Token("%PRICE_UNIT%", orderReceipt.OrderRow.PriceUnit),
                            new HtmlNotification.Token("%FEE%", feeString),
                            new HtmlNotification.Token("%TOTAL%", total.ToString(OrderPrice.CurrencyFormatString)),
                            new HtmlNotification.Token(HtmlNotification.TAG_LOGO, _copier.CopiedLogoPath));



                        html.ReplaceSystemTokens();

                        msgInfo.Body = html.HTMLOutput;
                        msgInfo.IsHtml = true;

                        // Queue email
                        Messaging.QueueEmail(msgInfo);

                        // Add customer communication
                        using (var taCustomerCommunication = new CustomerCommunicationTableAdapter())
                        {
                            foreach (var contact in contacts)
                            {
                                taCustomerCommunication.Insert(contact.ContactID,
                                    orderReceipt.OrderID,
                                    "Order Receipt Notification Sent",
                                    DateTime.Now,
                                    orderReceipt.UserID);
                            }
                        }

                        sentEmail = true;
                    }

                    return sentEmail;
                }
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Error queuing notification for email.");
                return false;
            }
        }

        /// <summary>
        /// Gets a HTML string representation of the order's fee.
        /// </summary>
        /// <param name="orderRow">The order row.</param>
        /// <returns>
        /// A HTML string representation of the order's fees.
        /// </returns>
        private static string GetFeeString(OrdersDataSet.OrderRow orderRow)
        {
            if (orderRow == null)
            {
                return "N/A";
            }

            var fees = orderRow.GetOrderFeesRows()
                .Where(f => f.Charge > 0)
                .ToList();

            if (fees.Count == 0)
            {
                return "No Fees";
            }

            var feeStrings = new List<string>();

            foreach (var fee in fees)
            {
                var feeTotal = OrderPrice.CalculateFees(
                    fee.OrderFeeTypeRow.FeeType,
                    fee.Charge,
                    orderRow.IsBasePriceNull() ? 0M : orderRow.BasePrice,
                    orderRow.IsPartQuantityNull() ? 0 : orderRow.PartQuantity,
                    orderRow.IsPriceUnitNull() ? "Each" : orderRow.PriceUnit,
                    orderRow.IsWeightNull() ? 0M : orderRow.Weight);

                feeStrings.Add(fee.OrderFeeTypeID + " - " + feeTotal.ToString(OrderPrice.CurrencyFormatString));
            }

            return string.Join("<br>", feeStrings);
        }

        /// <summary>
        /// Calculates the total discount for the order.
        /// </summary>
        /// <param name="orderRow">The order.</param>
        /// <returns>The order's total discount.</returns>
        private static decimal CalculateDiscount(OrdersDataSet.OrderRow orderRow)
        {
            if (orderRow == null)
            {
                return 0M;
            }

            var discounts = orderRow.GetOrderFeesRows()
                .Where(f => f.Charge <= 0)
                .ToList();

            if (discounts.Count == 0)
            {
                return 0M;
            }

            return discounts.Sum(discount =>
                OrderPrice.CalculateFees(
                    discount.OrderFeeTypeRow.FeeType,
                    discount.Charge,
                    orderRow.IsBasePriceNull() ? 0M : orderRow.BasePrice,
                    orderRow.IsPartQuantityNull() ? 0 : orderRow.PartQuantity,
                    orderRow.IsPriceUnitNull() ? "Each" : orderRow.PriceUnit,
                    orderRow.IsWeightNull() ? 0M : orderRow.Weight
                ));
        }

        /// <summary>
        /// Calculates the order's subtotal.
        /// </summary>
        /// <param name="orderRow">The order.</param>
        /// <returns>The order's subtotal.</returns>
        private static decimal CalculateSubtotal(OrdersDataSet.OrderRow orderRow)
        {
            if (orderRow == null)
            {
                return 0M;
            }

            var basePrice = orderRow.IsBasePriceNull() ? 0M : orderRow.BasePrice;

            return OrderPrice.CalculatePrice(
                basePrice,
                orderRow.IsPriceUnitNull() ? "Each" : orderRow.PriceUnit,
                0M,
                orderRow.IsPartQuantityNull() ? 0 : orderRow.PartQuantity,
                orderRow.IsWeightNull() ? 0M : orderRow.Weight);

        }

        /// <summary>
        /// Calculates the total for the order.
        /// </summary>
        /// <param name="orderRow">The order.</param>
        /// <returns>
        /// The order's total; includes fees and discounts.
        /// </returns>
        private static decimal CalculateTotal(OrdersDataSet.OrderRow orderRow)
        {
            if (orderRow == null)
            {
                return 0M;
            }

            var basePrice = orderRow.IsBasePriceNull() ? 0M : orderRow.BasePrice;

            return OrderPrice.CalculatePrice(
                basePrice,
                orderRow.IsPriceUnitNull() ? "Each" : orderRow.PriceUnit,
                OrderPrice.CalculateFees(orderRow, basePrice),
                orderRow.IsPartQuantityNull() ? 0 : orderRow.PartQuantity,
                orderRow.IsWeightNull() ? 0M : orderRow.Weight);
        }

        private static bool ReceivesOrderReceiptNotificationsForCustomer(CustomersDataset.ContactRow contact, int customerId)
        {
            return false;
        }

        #endregion

        #region IJob Members

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Log.Info($"BEGIN: {nameof(OrderReceiptNotificationTask)}");


                await Task.Run(() => BeginProcessing()).ConfigureAwait(false);

            }
            catch (Exception exc)
            {
                const string errorMsg = "Error sending email notifications.";
                Log.Error(exc, errorMsg);
            }
            finally
            {
                Log.Info($"END: {nameof(OrderReceiptNotificationTask)}");
            }
        }

        #endregion

        #region NewReceiptNotificationHtml

        internal class NewReceiptNotificationHtml : HtmlNotification
        {
            #region Methods

            protected override void CreateNotification()
            {
                using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.TemplatesTableAdapter())
                {
                    using (var templates = ta.GetDataById("OrderReceiptNotification"))
                    {
                        HTMLOutput = templates.FirstOrDefault().Template;
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
