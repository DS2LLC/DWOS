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
    [DisallowConcurrentExecution]
    public class OrderHoldNotificationTask : IJob
    {
        #region  Fields

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly MessagingUtilities Messaging =
            new MessagingUtilities("OrderHoldNotifications");

        private readonly EmailLogoCopier _copier = new EmailLogoCopier();

        #endregion

        #region  Methods

        private void BeginProcessing()
        {
            OrdersDataSet dsOrders = null;
            try
            {
                _copier.CopyCompanyLogo(Messaging.EmailPickupDirectory);
                dsOrders = GetData();

                foreach(var hold in dsOrders.OrderHold)
                {
                    var holdReason = dsOrders.d_HoldReason.FindByHoldReasonID(hold.HoldReasonID);

                    QueueEmailNotifications(hold, holdReason);

                    var sentDate = DateTime.Now;

                    foreach(var holdNotification in hold.GetOrderHoldNotificationRows()) holdNotification.NotificationSent = sentDate;
                }

                using(var taOrderHoldNotification = new OrderHoldNotificationTableAdapter())
                {
                    taOrderHoldNotification.Update(dsOrders.OrderHoldNotification);
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

            using(var taHoldReason = new d_HoldReasonTableAdapter())
            {
                taHoldReason.Fill(dsOrders.d_HoldReason);
            }

            using(var taContact = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.ContactSummaryTableAdapter())
            {
                taContact.Fill(dsOrders.ContactSummary);
            }

            using(var taOrderHoldNotification = new OrderHoldNotificationTableAdapter())
            {
                taOrderHoldNotification.FillUnsent(dsOrders.OrderHoldNotification);
            }

            using(var taOrderHold = new OrderHoldTableAdapter {ClearBeforeFill = false})
            {
                foreach(var holdNotification in dsOrders.OrderHoldNotification) taOrderHold.FillByOrderHold(dsOrders.OrderHold, holdNotification.OrderHoldID);
            }

            using(var taOrders = new OrderTableAdapter {ClearBeforeFill = false})
            {
                foreach(var orderHold in dsOrders.OrderHold) taOrders.FillByOrderID(dsOrders.Order, orderHold.OrderID);
            }

            using(var taOrderSerialNumbers = new OrderSerialNumberTableAdapter {ClearBeforeFill = false})
            {
                foreach(var order in dsOrders.Order) taOrderSerialNumbers.FillByOrder(dsOrders.OrderSerialNumber, order.OrderID);
            }

            using (var taParts = new PartSummaryTableAdapter { ClearBeforeFill = false })
            {
                foreach (var order in dsOrders.Order) taParts.FillByPart(dsOrders.PartSummary, order.PartID);
            }

            return dsOrders;
        }

        private void QueueEmailNotifications(OrdersDataSet.OrderHoldRow hold, OrdersDataSet.d_HoldReasonRow holdReason)
        {
            var notifications = hold?.GetOrderHoldNotificationRows();

            if(hold == null || notifications == null || notifications.Length == 0) return;

            var validContacts = notifications
                                .Select(n => n.ContactSummaryRow)
                                .Where(HasValidCustomerAddress)
                                .ToList();

            if(validContacts.Count == 0) return;

            var addresses = validContacts
                            .Select(c => new EmailAddress {Address = c.EmailAddress})
                            .ToList();

            // Create email body
            var htmlBody = new OrderHoldNotificationEmail();

            var order = hold.OrderRow;
            var purchaseOrder = order.IsPurchaseOrderNull() ? string.Empty : order.PurchaseOrder;
            var serialNumbers = string.Join(", ", order.GetOrderSerialNumberRows()
                                                       .Where(orderSerialNumber => orderSerialNumber.Active && !orderSerialNumber.IsNumberNull())
                                                       .Select(orderSerialNumber => orderSerialNumber.Number));
            var partSummaryName = order.PartSummaryRow?.Name ?? string.Empty;

            htmlBody.ReplaceHoldTokens(_copier.CopiedLogoPath, hold.OrderID, holdReason?.Name, purchaseOrder, serialNumbers, partSummaryName);
            htmlBody.ReplaceSystemTokens();

            // Create email
            var msginfo = new MessageInfo
                          {
                              FromAddress =
                                  new EmailAddress
                                  {
                                      Address = ApplicationSettings.Current.EmailFromAddress,
                                      DisplayName = "No Reply"
                                  },
                              Subject = $"Order Hold - {hold.OrderID}",
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
            using(var taCustomerCommunication = new CustomerCommunicationTableAdapter())
            {
                foreach(var contact in validContacts)
                    taCustomerCommunication.Insert(
                        contact.ContactID,
                        hold.OrderID,
                        "Hold Notification Sent",
                        DateTime.Now,
                        hold.TimeInUser);
            }
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Log.Info($"BEGIN: {nameof(OrderApprovalNotificationTask)}");

                if(ApplicationSettings.Current.OrderApprovalEnabled) await Task.Run(() => BeginProcessing()).ConfigureAwait(false);
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

        #region Static Methods

        private static bool HasValidCustomerAddress(OrdersDataSet.ContactSummaryRow contact)
        {
            if(contact == null) return false;

            var address = contact.IsEmailAddressNull()
                ? string.Empty
                : contact.EmailAddress;

            if(address.IsValidEmail()) return true;

            Log.Warn("Unable to send an email to \"{0}\"; this email address is invalid.", address);
            return false;
        }

        #endregion

        #region NewApprovalNotificationHtml

        internal class NewApprovalNotificationHtml : HtmlNotification
        {
            #region  Methods

            protected override void CreateNotification()
            {
                using(var ta = new TemplatesTableAdapter())
                {
                    using(var templates = ta.GetDataById("OrderApprovalNotification"))
                    {
                        HTMLOutput = templates.FirstOrDefault().Template;
                    }
                }
            }

            #endregion
        }

        #endregion

        #region OrderHoldNotificationEmail

        private class OrderHoldNotificationEmail : HtmlNotification
        {
            #region  Fields

            private const string TAG_ORDER = "%ORDER%";
            private const string TAG_HOLD_REASON = "%HOLDREASON%";
            private const string TAG_PURCHASE_ORDER = "%PURCHASEORDER%";
            private const string TAG_SERIAL_NUMBERS = "%SERIALNUMBERS%";
            private const string TAG_PART = "%PART%";

            #endregion

            #region  Methods

            protected override void CreateNotification()
            {
                using(var ta = new TemplatesTableAdapter())
                {
                    var templates = ta.GetDataById("HoldNotification");
                    HTMLOutput = templates.FirstOrDefault()?.Template;
                }
            }

            public void ReplaceHoldTokens(string logoFilePath, int orderId, string holdReason, string purchaseOrder, string serialNumbers, string partName)
            {
                ReplaceTokens(new Token(TAG_LOGO, logoFilePath),
                    new Token(TAG_ORDER, orderId.ToString()),
                    new Token(TAG_HOLD_REASON, holdReason),
                    new Token(TAG_PURCHASE_ORDER, purchaseOrder),
                    new Token(TAG_SERIAL_NUMBERS, serialNumbers),
                    new Token(TAG_PART, partName));
            }

            #endregion
        }

        #endregion
    }
}