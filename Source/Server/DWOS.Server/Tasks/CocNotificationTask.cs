using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.Data.Datasets.COCDatasetTableAdapters;
using DWOS.Server.Utilities;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Reports;
using System.Threading.Tasks;

namespace DWOS.Server.Tasks
{
    /// <summary>
    /// Processes certificate notifications.
    /// </summary>
    public class CocNotificationTask : IJob
    {
        #region Fields

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static readonly MessagingUtilities Messaging;
        private readonly EmailLogoCopier _copier = new EmailLogoCopier();

        #endregion

        #region Methods

        static CocNotificationTask()
        {
            Messaging = new MessagingUtilities("COCNotifications");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Log.Info($"BEGIN: {nameof(CocNotificationTask)}");
                await Task.Run(() => BeginProcessing()).ConfigureAwait(false);
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error sending email notifications.";
                Log.Error(exc, errorMsg);
            }
            finally
            {
                Log.Info($"END: {nameof(CocNotificationTask)}");
            }
        }

        private void BeginProcessing()
        {
            try
            {
                _copier.CopyCompanyLogo(Messaging.EmailPickupDirectory);

                var cocDataset = GetData();

                // COC
                foreach (var coc in cocDataset.COC)
                {
                    QueueEmailNotification(coc);

                    var sentDate = DateTime.Now;
                    foreach (var notification in coc.GetCOCNotificationRows())
                    {
                        notification.NotificationSent = sentDate;
                    }
                }

                using (var taNotification = new COCNotificationTableAdapter())
                {
                    taNotification.Update(cocDataset.COCNotification);
                }

                // Bulk Certificates
                foreach (var bulkCoc in cocDataset.BulkCOC)
                {
                    QueueEmailNotification(bulkCoc);
                    var sentDate = DateTime.Now;
                    foreach (var notification in bulkCoc.GetBulkCOCNotificationRows())
                    {
                        notification.NotificationSent = sentDate;
                    }
                }

                using (var taNotification = new BulkCOCNotificationTableAdapter())
                {
                    taNotification.Update(cocDataset.BulkCOCNotification);
                }

                // Batch Certificates
                foreach (var batchCoc in cocDataset.BatchCOC)
                {
                    QueueEmailNotification(batchCoc);
                    var sentDate = DateTime.Now;
                    foreach (var notification in batchCoc.GetBatchCOCNotificationRows())
                    {
                        notification.NotificationSent = sentDate;
                    }
                }

                using (var taNotification = new BatchCOCNotificationTableAdapter())
                {
                    taNotification.Update(cocDataset.BatchCOCNotification);
                }
            }
            finally
            {
                Messaging.BeginProcessingEmails();
            }
        }

        private void QueueEmailNotification(COCDataset.COCRow coc)
        {
            var notifications = coc?.GetCOCNotificationRows();
            if (coc == null || notifications == null || notifications.Length == 0)
            {
                return;
            }

            var validContacts = notifications
                .Select(n => n.d_ContactRow)
                .Where(HasValidCustomerAddress)
                .ToList();

            if (validContacts.Count == 0)
            {
                return;
            }

            var addresses = validContacts
                .Select(c => new EmailAddress { Address = c.EmailAddress })
                .ToList();

            // Create COC
            var report = new COCReport(coc.COCID);
            var reportPath = report.PublishReport(Messaging.EmailPickupDirectory);

            var orderType = "WO";
            var orderId = coc.OrderID;

            QueueEmail(new QueueEmailParams(orderId, orderType, addresses, reportPath));

            // Add communications to order
            using (var taCustomerCommunication = new Data.Datasets.OrdersDataSetTableAdapters.CustomerCommunicationTableAdapter())
            {
                foreach (var contact in validContacts)
                {
                    var qaUserId = coc.IsQAUserNull()
                        ? (int?)null
                        : coc.QAUser;

                    taCustomerCommunication.Insert(contact.ContactID, coc.OrderID, "COC Notification Sent", DateTime.Now, qaUserId);
                }
            }
        }

        private void QueueEmailNotification(COCDataset.BulkCOCRow bulkCoc)
        {
            var notifications = bulkCoc?.GetBulkCOCNotificationRows();
            if (bulkCoc == null || notifications == null || notifications.Length == 0)
            {
                return;
            }

            var validContacts = notifications
                .Select(n => n.d_ContactRow)
                .Where(HasValidCustomerAddress)
                .ToList();

            if (validContacts.Count == 0)
            {
                return;
            }

            var addresses = validContacts
                .Select(c => new EmailAddress { Address = c.EmailAddress })
                .ToList();

            // Create COC
            var report = new BulkCOCReport(bulkCoc.BulkCOCID);
            var reportPath = report.PublishReport(Messaging.EmailPickupDirectory);

            var orderType = "Package";
            var orderId = bulkCoc.ShipmentPackageID;

            QueueEmail(new QueueEmailParams(orderId, orderType, addresses, reportPath));

            // Add communications for each order in bulk COC
            using (var taCustomerCommunication = new Data.Datasets.OrdersDataSetTableAdapters.CustomerCommunicationTableAdapter())
            {
                foreach (var bulkCocOrder in bulkCoc.GetBulkCOCOrderRows())
                {
                    foreach (var contact in validContacts)
                    {
                        taCustomerCommunication.Insert(contact.ContactID,
                            bulkCocOrder.OrderID,
                            "Bulk Certificate Notification Sent",
                            DateTime.Now,
                            bulkCoc.QAUser);
                    }
                }
            }
        }

        private void QueueEmailNotification(COCDataset.BatchCOCRow batchCoc)
        {
            var notifications = batchCoc?.GetBatchCOCNotificationRows();
            if (batchCoc == null || notifications == null || notifications.Length == 0)
            {
                return;
            }

            var validContacts = notifications
                .Select(n => n.d_ContactRow)
                .Where(HasValidCustomerAddress)
                .ToList();

            if (validContacts.Count == 0)
            {
                return;
            }

            var addresses = validContacts
                .Select(c => new EmailAddress { Address = c.EmailAddress })
                .ToList();

            // Create COC
            string reportPath;
            string typeToUse;
            int idToUse;
            using (var report = BatchCocReport.FromBatchCocId(batchCoc.BatchCOCID))
            {
                reportPath = report.PublishReport(Messaging.EmailPickupDirectory);
                typeToUse = report.IdentifierType;
                idToUse = report.Identifier;
            }

            // Send COC
            QueueEmail(new QueueEmailParams(idToUse, typeToUse, addresses, reportPath));

            // Add communications for each order in bulk COC
            using (var taCustomerCommunication = new Data.Datasets.OrdersDataSetTableAdapters.CustomerCommunicationTableAdapter())
            {
                foreach (var batchCocOrder in batchCoc.GetBatchCOCOrderRows())
                {
                    foreach (var contact in validContacts)
                    {
                        taCustomerCommunication.Insert(contact.ContactID,
                            batchCocOrder.OrderID,
                            "Batch Certificate Notification Sent",
                            DateTime.Now,
                            batchCoc.QAUser);
                    }
                }
            }
        }

        private void QueueEmail(QueueEmailParams queueEmailParams)
        {
            // Create email body
            var htmlBody = new CertificateNotificationEmail();
            htmlBody.ReplaceCertificateTokens(_copier.CopiedLogoPath, queueEmailParams.OrderId, queueEmailParams.OrderType);
            htmlBody.ReplaceSystemTokens();

            var msginfo = new MessageInfo
            {
                FromAddress =
                    new EmailAddress
                    {
                        Address = ApplicationSettings.Current.EmailFromAddress,
                        DisplayName = "No Reply"
                    },
                Subject = $"Certificate Notification - {queueEmailParams.OrderType} {queueEmailParams.OrderId}",
                ToAddresses = queueEmailParams.Addresses,
                Attachments = new List<string> {queueEmailParams.ReportPath},
                Body = htmlBody.HTMLOutput,
                IsHtml = true
            };

            try
            {
                Messaging.QueueEmail(msginfo);
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Error queuing emails for notification.");
            }
        }

        private static COCDataset GetData()
        {
            var dsCoc = new COCDataset { EnforceConstraints = false };

            using (var taCocNotifications = new COCNotificationTableAdapter())
            {
                taCocNotifications.FillUnsent(dsCoc.COCNotification);
            }

            using (var taBulkCocNotifications = new BulkCOCNotificationTableAdapter())
            {
                taBulkCocNotifications.FillUnsent(dsCoc.BulkCOCNotification);
            }

            using (var taBatchCocNotification = new BatchCOCNotificationTableAdapter())
            {
                taBatchCocNotification.FillUnsent(dsCoc.BatchCOCNotification);
            }

            using (var taContact = new d_ContactTableAdapter { ClearBeforeFill = false })
            {
                var distinctContactIds = dsCoc.COCNotification.Select(n => n.ContactID)
                    .Concat(dsCoc.BulkCOCNotification.Select(n => n.ContactID))
                    .Concat(dsCoc.BatchCOCNotification.Select(n => n.ContactID))
                    .Distinct();

                foreach (var contactId in distinctContactIds)
                {
                    taContact.FillById(dsCoc.d_Contact, contactId);
                }
            }

            using (var taCoc = new COCTableAdapter { ClearBeforeFill = false })
            {
                foreach (var cocId in dsCoc.COCNotification.Select(n => n.COCID))
                {
                    taCoc.FillBy(dsCoc.COC, cocId);
                }
            }

            using (var taBulkCoc = new BulkCOCTableAdapter { ClearBeforeFill = false })
            {
                foreach (var bulkCocId in dsCoc.BulkCOCNotification.Select(n => n.BulkCOCID))
                {
                    taBulkCoc.FillBy(dsCoc.BulkCOC, bulkCocId);
                }
            }

            using (var taBulkCocOrder = new BulkCOCOrderTableAdapter { ClearBeforeFill = false })
            {
                foreach (var bulkCocId in dsCoc.BulkCOCNotification.Select(n => n.BulkCOCID))
                {
                    taBulkCocOrder.FillByBulkCoc(dsCoc.BulkCOCOrder, bulkCocId);
                }
            }

            using (var taBatchCoc = new BatchCOCTableAdapter { ClearBeforeFill = false })
            {
                using (var taBatchCocOrder = new BatchCOCOrderTableAdapter { ClearBeforeFill = false })
                {
                    foreach (var batchCocId in dsCoc.BatchCOCNotification.Select(n => n.BatchCOCID))
                    {
                        taBatchCoc.FillByBatchCoc(dsCoc.BatchCOC, batchCocId);
                        taBatchCocOrder.FillByBatchCoc(dsCoc.BatchCOCOrder, batchCocId);
                    }
                }
            }

            return dsCoc;
        }

        private static bool HasValidCustomerAddress(COCDataset.d_ContactRow contact)
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

        #region CertificateNotificationEmail

        private class CertificateNotificationEmail : HtmlNotification
        {
            private const string TAG_ORDER = "%ORDER%";
            private const string TAG_ORDERTYPE = "%ORDERTYPE%";

            protected override void CreateNotification()
            {
                using (var ta = new TemplatesTableAdapter())
                {
                    var templates = ta.GetDataById("CertificateNotification");
                    HTMLOutput = templates.FirstOrDefault()?.Template;
                }
            }

            public void ReplaceCertificateTokens(string logoFilePath, int id, string type)
            {
                ReplaceTokens(new Token(TAG_LOGO, logoFilePath),
                    new Token(TAG_ORDER, id.ToString()),
                    new Token(TAG_ORDERTYPE, type));
            }
        }

        #endregion

        #region QueueEmailParams

        private class QueueEmailParams
        {
            public QueueEmailParams(int orderId, string orderType, List<EmailAddress> addresses, string reportPath)
            {
                OrderId = orderId;
                OrderType = orderType;
                Addresses = addresses;
                ReportPath = reportPath;
            }

            public int OrderId { get; }

            public string OrderType { get; }

            public List<EmailAddress> Addresses { get; }

            public string ReportPath { get; }
        }

        #endregion
    }
}