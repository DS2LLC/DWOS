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
    public class OrderApprovalNotificationTask : IJob
    {
        #region Fields

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly MessagingUtilities Messaging =
            new MessagingUtilities("OrderApprovalNotifications");

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
                    var newApprovals = dsOrders
                        .OrderApproval
                        .Where(approval => approval.IsInitialEmailSentNull())
                        .ToList();

                    foreach (var approval in newApprovals)
                    {
                        if (QueueEmailNotification(approval))
                        {
                            approval.InitialEmailSent = DateTime.Now;
                        }
                        else
                        {
                            Log.Warn($"Did not send email for approval " +
                                $"{approval.OrderApprovalID} - there may be " +
                                $"0 contacts authorized to receive approval notifications.");
                        }
                    }

                    using (var taOrderApproval = new OrderApprovalTableAdapter())
                    {
                        taOrderApproval.Update(dsOrders.OrderApproval);
                    }
                }

                // Reminder email - sent to department/product class.
                using (var dsOrders = RetrieveReminderEmailData())
                {
                    foreach (var approval in dsOrders.OrderApproval)
                    {
                        if (approval.Status != nameof(OrderApprovalStatus.Pending) || !approval.IsReminderSentNull())
                        {
                            continue;
                        }

                        if (QueueReminder(approval))
                        {
                            approval.ReminderSent = DateTime.Now;
                        }
                        else
                        {
                            Log.Warn($"Did not send reminder email for approval " +
                                $"{approval.OrderApprovalID} - its order may" +
                                $"be in a department without an email address");
                        }
                    }

                    using (var taOrderApproval = new OrderApprovalTableAdapter())
                    {
                        taOrderApproval.Update(dsOrders.OrderApproval);
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

            // Approvals
            using (var taOrderApproval = new OrderApprovalTableAdapter())
            {
                taOrderApproval.FillNewApprovals(dsOrders.OrderApproval);
            }

            // Load orders and associated data
            using (var taOrder = new OrderTableAdapter())
            {
                taOrder.FillNewApprovals(dsOrders.Order);
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

        private OrdersDataSet RetrieveReminderEmailData()
        {
            var currentTime = DateTime.Now;
            var toDate = DateTime.Today
                .AddBusinessDays(-ApplicationSettings.Current.OrderApprovalReminderDays)
                .Add(currentTime.TimeOfDay);

            var dsOrders = new OrdersDataSet
            {
                EnforceConstraints = false
            };

            using (var taOrderApproval = new OrderApprovalTableAdapter())
            {
                taOrderApproval.FillForReminder(dsOrders.OrderApproval, toDate);
            }

            using (var taOrder = new OrderTableAdapter { ClearBeforeFill = false })
            {
                using (var taProductClass = new OrderProductClassTableAdapter { ClearBeforeFill = false })
                {
                    var orderIds = dsOrders.OrderApproval
                        .Select(appr => appr.OrderID)
                        .Distinct();

                    foreach (var orderId in orderIds)
                    {
                        taOrder.FillByOrderID(dsOrders.Order, orderId);
                        taProductClass.FillByOrder(dsOrders.OrderProductClass, orderId);
                    }
                }
            }

            return dsOrders;
        }

        /// <summary>
        /// Queues an email notification for an order approval.
        /// </summary>
        /// <param name="approval"></param>
        /// <returns>
        /// <c>true</c> if an email was successfully queued;
        /// otherwise, <c>false</c>.
        /// </returns>
        private bool QueueEmailNotification(OrdersDataSet.OrderApprovalRow approval)
        {
            try
            {
                if (approval == null || approval.OrderRow == null)
                {
                    Log.Error("Could not find approval and/or its order.");
                    return false;
                }

                var customerId = approval.OrderRow.CustomerID;

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

                    var contacts = dsCustomer.Contact
                        .Where(c => !c.IsNameNull() && !c.IsEmailAddressNull() && c.Active && c.CustomerRow.Active && ReceivesApprovalNotificationsForCustomer(c, customerId))
                        .ToList();

                    var sentEmail = false;

                    if (contacts.Count > 0)
                    {
                        // Retrieve values to use in email
                        var purchaseOrder = approval.OrderRow.IsPurchaseOrderNull()
                            ? string.Empty
                            : approval.OrderRow.PurchaseOrder;

                        var serialNumbers = approval.OrderRow.GetOrderSerialNumberRows()
                            .Where(orderSerialNumber => orderSerialNumber.Active && !orderSerialNumber.IsNumberNull())
                            .Select(orderSerialNumber => orderSerialNumber.Number)
                            .ToList();

                        var partName = approval.OrderRow.PartSummaryRow == null
                            ? "N/A"
                            : approval.OrderRow.PartSummaryRow.Name;

                        var operatorNotes = approval.IsNotesNull()
                            ? string.Empty
                            : approval.Notes.Replace(Environment.NewLine, "<br>");

                        var discount = CalculateDiscount(approval.OrderRow);
                        var subtotal = CalculateSubtotal(approval.OrderRow) + discount;
                        var feeString = GetFeeString(approval.OrderRow);

                        var total = CalculateTotal(approval.OrderRow);

                        // Construct email
                        var srtSN = "";
                        var show = false;
                        Log.Debug($"Customer ID: {customerId}");
                        using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
                        {
                            var result = ta.GetShowSNinApprovalSubjectLineByID(customerId);
                            Log.Debug($"Result: {result}");
                            show = (bool)result;
                        }

                       
                        if (show)
                            srtSN = " S/N: " + string.Join(",", serialNumbers);  
                        var msgInfo = new MessageInfo
                        {
                            FromAddress = new EmailAddress
                            {
                                Address = ApplicationSettings.Current.EmailFromAddress,
                                DisplayName = "No Reply"
                            },
                            Subject = $"Order Approval Required - WO#:{approval.OrderID}" + srtSN
                        };

                        msgInfo.ToAddresses.AddRange(contacts.Select(c => new EmailAddress { Address = c.EmailAddress, DisplayName = c.Name }));

                        var html = new NewApprovalNotificationHtml();
                        html.ReplaceTokens(
                            new HtmlNotification.Token("%ORDER%", approval.OrderID.ToString()),
                            new HtmlNotification.Token("%PURCHASEORDER%", purchaseOrder),
                            new HtmlNotification.Token("%SERIALNUMBERS%", string.Join(", ", serialNumbers)),
                            new HtmlNotification.Token("%PART%", partName),
                            new HtmlNotification.Token("%OPERATORNOTES%", operatorNotes),
                            new HtmlNotification.Token("%SUBTOTAL%", subtotal.ToString(OrderPrice.CurrencyFormatString)),
                            new HtmlNotification.Token("%FEES%", feeString),
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
                                    approval.OrderID,
                                    "Approval Notification Sent",
                                    DateTime.Now,
                                    approval.UserID);
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

        private bool QueueReminder(OrdersDataSet.OrderApprovalRow approval)
        {
            try
            {
                if (approval == null || approval.OrderRow == null)
                {
                    Log.Error("Could not find approval and/or its order.");
                    return false;
                }

                var emailAddresses = new List<EmailAddress>();

                // Retrieve department email
                using (var taDepartment = new d_DepartmentTableAdapter())
                {
                    var deptEmail = taDepartment.GetEmailAddress(approval.OrderRow.CurrentLocation);

                    if (!string.IsNullOrEmpty(deptEmail))
                    {
                        emailAddresses.Add(new EmailAddress
                        {
                            Address = deptEmail,
                            DisplayName = approval.OrderRow.CurrentLocation
                        });
                    }
                }

                // Retrieve product class email(s)
                if (ProductClassEnabled)
                {
                    using (var taProductClass = new ProductClassTableAdapter())
                    {
                        foreach (var productClassRow in approval.OrderRow.GetOrderProductClassRows())
                        {
                            if (productClassRow.IsProductClassNull())
                            {
                                continue;
                            }

                            var productClassEmail = taProductClass.GetEmailAddress(productClassRow.ProductClass);

                            if (!string.IsNullOrEmpty(productClassEmail))
                            {
                                emailAddresses.Add(new EmailAddress
                                {
                                    Address = productClassEmail,
                                    DisplayName = productClassRow.ProductClass
                                });
                            }
                        }
                    }
                }

                var filteredEmailAddresses = emailAddresses
                    .DistinctBy(email => email.Address)
                    .ToList();

                var sentEmails = false;
                if (filteredEmailAddresses.Count > 0)
                {
                    // Construct email
                    var msgInfo = new MessageInfo
                    {
                        FromAddress = new EmailAddress
                        {
                            Address = ApplicationSettings.Current.EmailFromAddress,
                            DisplayName = "No Reply"
                        },
                        Subject = $"DWOS - Reminder for Approval {approval.OrderID}"
                    };

                    msgInfo.ToAddresses.AddRange(filteredEmailAddresses);
                    msgInfo.Body = $"Approval {approval.OrderApprovalID} for WO " +
                        $"{approval.OrderID} still requires customer approval.";

                    msgInfo.IsHtml = false;

                    // Queue email
                    Messaging.QueueEmail(msgInfo);
                    sentEmails = true;
                }

                return sentEmails;
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Error queuing reminder for approval.");
                return false;
            }
        }

        private static bool ReceivesApprovalNotificationsForCustomer(CustomersDataset.ContactRow contact, int customerId)
        {
            return contact.ApprovalNotification
                && (contact.CustomerID == customerId || contact.GetContactAdditionalCustomerRows().Any(c => c.CustomerID == customerId && c.IncludeInApprovalNotifications));
        }

        #endregion

        #region IJob Members

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Log.Info($"BEGIN: {nameof(OrderApprovalNotificationTask)}");

                if (ApplicationSettings.Current.OrderApprovalEnabled)
                {
                    await Task.Run(() => BeginProcessing()).ConfigureAwait(false);
                }
            }
            catch (Exception exc)
            {
                const string errorMsg = "Error sending email notifications.";
                Log.Error(exc, errorMsg);
            }
            finally
            {
                Log.Info($"END: {nameof(OrderApprovalNotificationTask)}");
            }
        }

        #endregion

        #region NewApprovalNotificationHtml

        internal class NewApprovalNotificationHtml : HtmlNotification
        {
            #region Methods

            protected override void CreateNotification()
            {
                using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.TemplatesTableAdapter())
                {
                    using (var templates = ta.GetDataById("OrderApprovalNotification"))
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
