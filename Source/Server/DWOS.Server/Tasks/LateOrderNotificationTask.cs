using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Server.Utilities;
using NLog;
using Quartz;

namespace DWOS.Server.Tasks
{
    /// <summary>
    /// Processes late order notifications.
    /// </summary>
    [DisallowConcurrentExecution]
    public class LateOrderNotificationTask : IJob
    {
        #region Fields

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static readonly MessagingUtilities Messaging =
            new MessagingUtilities("LateOrderNotifications");

        private readonly EmailLogoCopier _copier = new EmailLogoCopier();


        #endregion

        #region Methods

        private void BeginProcessing()
        {
            OrdersDataSet dsOrders = null;
            try
            {
                _copier.CopyCompanyLogo(Messaging.EmailPickupDirectory);
                dsOrders = GetData();

                foreach (var order in dsOrders.Order)
                {
                    QueueEmailNotifications(order);

                    var sentDate = DateTime.Now;

                    foreach (var notification in order.GetLateOrderNotificationRows())
                    {
                        notification.NotificationSent = sentDate;
                    }
                }

                using (var taLateOrderNotification = new LateOrderNotificationTableAdapter())
                {
                    taLateOrderNotification.Update(dsOrders.LateOrderNotification);
                }
            }
            finally
            {
                Messaging.BeginProcessingEmails();
                dsOrders?.Dispose();
            }
        }

        private OrdersDataSet GetData()
        {
            var dsOrders = new OrdersDataSet
            {
                EnforceConstraints = false
            };

            using (var taContact = new Data.Datasets.OrdersDataSetTableAdapters.ContactSummaryTableAdapter())
            {
                taContact.Fill(dsOrders.ContactSummary);
            }

            using (var taLateOrderNotification = new LateOrderNotificationTableAdapter())
            {
                taLateOrderNotification.FillUnsent(dsOrders.LateOrderNotification);
            }

            // Load orders and associated data
            using (var taOrder = new OrderTableAdapter { ClearBeforeFill = false })
            {
                var orderIds = dsOrders.LateOrderNotification
                    .Select(notification => notification.OrderID);

                foreach (var orderId in orderIds)
                {
                    taOrder.FillByOrderID(dsOrders.Order, orderId);
                }
            }

            using (var taOrderSerialNumbers = new OrderSerialNumberTableAdapter { ClearBeforeFill = false })
            {
                foreach (var order in dsOrders.Order)
                {
                    taOrderSerialNumbers.FillByOrder(dsOrders.OrderSerialNumber, order.OrderID);
                }
            }

            using (var taParts = new PartSummaryTableAdapter { ClearBeforeFill = false })
            {
                foreach (var order in dsOrders.Order)
                {
                    taParts.FillByPart(dsOrders.PartSummary, order.PartID);
                }
            }

            return dsOrders;
        }

        private void QueueEmailNotifications(OrdersDataSet.OrderRow order)
        {
            var notifications = order.GetLateOrderNotificationRows();

            if (order == null || notifications == null || notifications.Length == 0)
            {
                return;
            }

            var validContacts = notifications
                .Select(n => n.ContactSummaryRow)
                .Where(HasValidCustomerAddress)
                .ToList();

            if (validContacts.Count == 0)
            {
                return;
            }

            var addresses = validContacts
                .DistinctBy(c => c.EmailAddress)
                .Select(c => new EmailAddress { Address = c.EmailAddress })
                .ToList();

            // Create email body
            var htmlBody = new LateOrderNotificationEmail();

            htmlBody.ReplaceLateOrderTokens(_copier.CopiedLogoPath, order);
            htmlBody.ReplaceSystemTokens();

            // Create email
            var msginfo = new MessageInfo
            {
                FromAddress = new EmailAddress
                {
                    Address = ApplicationSettings.Current.EmailFromAddress,
                    DisplayName = "No Reply"
                },
                Subject = $"Notification About Order {order.OrderID}",
                ToAddresses = addresses,
                Attachments = new List<string>(),
                Body = htmlBody.HTMLOutput,
                IsHtml = true
            };

            // Send email
            try
            {
                Messaging.QueueEmail(msginfo);
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Error queuing emails for notification.");
            }

            // Add communications for the order
            using (var taCustomerCommunication = new CustomerCommunicationTableAdapter())
            {
                foreach (var contact in validContacts)
                {
                    taCustomerCommunication.Insert(
                        contact.ContactID,
                        order.OrderID,
                        "Late Order Notification Sent",
                        DateTime.Now,
                        notifications.First().UserID);
                }
            }
        }
        private static bool HasValidCustomerAddress(OrdersDataSet.ContactSummaryRow contact)
        {
            if (contact == null)
            {
                return false;
            }

            var address = contact.IsEmailAddressNull()
                ? string.Empty
                : contact.EmailAddress;

            if (address.IsValidEmail())
            {
                return true;
            }

            Log.Warn("Unable to send an email to \"{0}\"; this email address is invalid.", address);
            return false;
        }

        #endregion

        #region IJob Implementation

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

        #region LateOrderNotificationEmail

        private class LateOrderNotificationEmail : HtmlNotification
        {
            #region  Fields

            private const string TAG_ORDER = "%ORDER%";
            private const string TAG_PURCHASE_ORDER = "%PURCHASEORDER%";
            private const string TAG_SERIAL_NUMBERS = "%SERIALNUMBERS%";
            private const string TAG_PART = "%PART%";
            private const string TAG_ORIGINAL_SHIP_DATE = "%ORIGINALSHIPDATE%";
            private const string TAG_ADJUSTED_SHIP_DATE = "%ADJUSTEDSHIPDATE%";

            #endregion

            #region  Methods

            protected override void CreateNotification()
            {
                using(var ta = new TemplatesTableAdapter())
                {
                    var templates = ta.GetDataById("LateOrderNotification");
                    HTMLOutput = templates.FirstOrDefault()?.Template;
                }
            }

            public void ReplaceLateOrderTokens(string logoFilePath, OrdersDataSet.OrderRow order)
            {
                if (order == null)
                {
                    return;
                }

                var purchaseOrder = order.IsPurchaseOrderNull() ? string.Empty : order.PurchaseOrder;
                var serialNumbers = string.Join(", ", order.GetOrderSerialNumberRows()
                    .Where(orderSerialNumber => orderSerialNumber.Active && !orderSerialNumber.IsNumberNull())
                    .Select(orderSerialNumber => orderSerialNumber.Number));

                var partSummaryName = order.PartSummaryRow?.Name ?? string.Empty;

                ReplaceTokens(new Token(TAG_LOGO, logoFilePath),
                    new Token(TAG_ORDER, order.OrderID.ToString()),
                    new Token(TAG_PURCHASE_ORDER, purchaseOrder),
                    new Token(TAG_SERIAL_NUMBERS, serialNumbers),
                    new Token(TAG_PART, partSummaryName),
                    new Token(TAG_ORIGINAL_SHIP_DATE, order.IsEstShipDateNull() ? "N/A" : order.EstShipDate.ToShortDateString()),
                    new Token(TAG_ADJUSTED_SHIP_DATE, order.IsAdjustedEstShipDateNull() ? "N/A" : order.AdjustedEstShipDate.ToShortDateString()));
            }

            #endregion
        }

        #endregion
    }
}
