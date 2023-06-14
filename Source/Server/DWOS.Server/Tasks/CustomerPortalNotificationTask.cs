using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Server.Utilities;
using NLog;
using Quartz;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DWOS.Server.Tasks
{
    public class CustomerPortalNotificationTask : IJob
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static readonly MessagingUtilities _messaging;
        private readonly Size _logoSize = new Size(400, 200);
        private EmailLogoCopier _copier = new EmailLogoCopier();

        #endregion

        #region Methods

        static CustomerPortalNotificationTask()
        {
            _messaging = new MessagingUtilities("CustomerPortalNotifications");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Task.Run(() => BeginProcessing()).ConfigureAwait(false);
            }
            catch(Exception exc)
            {
                throw new JobExecutionException(exc);
            }
        }

        private void BeginProcessing()
        {
            _log.Info("BEGIN: CustomerPortalNotificationTask");

            try
            {
                _copier.ResizeCompanyLogo(_messaging.EmailPickupDirectory, _logoSize);

                using(var taContacts = new ContactTableAdapter())
                {
                    CustomersDataset.ContactDataTable contacts = taContacts.GetPortalAuthToSend();

                    //for each shipment that was shipped
                    foreach(CustomersDataset.ContactRow contact in contacts)
                    {
                        if(contact.IsEmailAddressNull())
                            continue;

                        var msginfo = new MessageInfo {FromAddress = new EmailAddress {Address = ApplicationSettings.Current.EmailFromAddress, DisplayName = "No Reply"}, Subject = ApplicationSettings.Current.CompanyName + " Portal Authorization"};

                        msginfo.ToAddresses.Add(new EmailAddress { Address = contact.EmailAddress, DisplayName = contact.Name });

                       if(!String.IsNullOrWhiteSpace(ApplicationSettings.Current.PortalAuthorizationEmailCC))
                            msginfo.ToAddresses.Add(new EmailAddress { Address = ApplicationSettings.Current.PortalAuthorizationEmailCC });

                        string password = Shared.Utilities.RandomUtils.CreateRandomPassword(10);

                        var html = new CustomerPortalNotificationHTML();
                        html.ReplaceTokens(
                            new HtmlNotification.Token(CustomerPortalNotificationHTML.TAG_USERNAME, contact.EmailAddress)
                            , new HtmlNotification.Token(CustomerPortalNotificationHTML.TAG_PASSWORD, password) 
                            , new HtmlNotification.Token(CustomerPortalNotificationHTML.TAG_LOGO, _copier.CopiedLogoPath));
                        html.ReplaceSystemTokens();

                        msginfo.Body = html.HTMLOutput;
                        msginfo.IsHtml = true;

                        //queue email to be sent
                        _messaging.QueueEmail(msginfo);

                        taContacts.ResetPassword(password, contact.ContactID);
                        taContacts.UpdatePortalAuthSent(DateTime.Now, contact.ContactID);
                    }
                }
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error sending email notifications.";
                _log.Error(exc, errorMsg);
            }
            finally
            {
                _messaging.BeginProcessingEmails();
                _log.Info("END: CustomerPortalNotificationTask");
            }
        }

        #endregion

        internal class CustomerPortalNotificationHTML : HtmlNotification
        {
            #region Fields

            public static string TAG_USERNAME  = "%USERNAME%";
            public static string TAG_PASSWORD = "%PASSWORD%";

            #endregion
            
            #region Methods

            protected override void CreateNotification()
            {
                using (var ta = new TemplatesTableAdapter())
                {
                    ApplicationSettingsDataSet.TemplatesDataTable templates = ta.GetDataById("CustomerPortal");
                    HTMLOutput = templates.FirstOrDefault().Template;
                }

            }

            #endregion
        }
    }
}