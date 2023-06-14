using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.Data.Datasets.QuoteDataSetTableAdapters;
using DWOS.Server.Utilities;
using HtmlAgilityPack;
using NLog;
using Quartz;

namespace DWOS.Server.Tasks
{
    internal class QuoteReminderTask : IJob
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static readonly MessagingUtilities _messaging;

        #endregion

        #region Methods

        static QuoteReminderTask() { _messaging = new MessagingUtilities("QuoteReminders"); }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Task.Run(() => BeginProcessing()).ConfigureAwait(false);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error running quote reminder task.");
            }
        }

        private void BeginProcessing()
        {
            _log.Info("BEGIN: QuoteReminderTask");

            try
            {
                if(String.IsNullOrEmpty(ServerSettings.Default.DBConnectionString))
                {
                    _log.Error("No database connection defined.");
                    return;
                }

                //Get the email template
                ApplicationSettingsDataSet.TemplatesRow template = GetTemplate();

                if(template == null)
                {
                    _log.Error("No quote reminder template available.");
                    return;
                }

                //Group all quotes to be sent
                Dictionary <int, QuotesByUser> quotesByUsers = GroupQuotesByUser();

                foreach(var quotesByUser in quotesByUsers)
                {
                    QuoteDataSet.QuoteStatusSummaryRow firstQuote = quotesByUser.Value.Quotes.FirstOrDefault();

                    if(firstQuote == null)
                        continue;

                    if(firstQuote.IsUserEmailNull())
                    {
                        _log.Info("No email for user {0}", firstQuote.UserId);
                        continue;
                    }

                    var msginfo = new MessageInfo {FromAddress = new EmailAddress {Address = ApplicationSettings.Current.EmailFromAddress, DisplayName = "No Reply"}, Subject = "Open Quote Reminder", ToAddresses = new List <EmailAddress> {new EmailAddress {Address = firstQuote.UserEmail, DisplayName = firstQuote.IsUserNameNull() ? null : firstQuote.UserName}}, Body = BuildEmailContent(template, quotesByUser.Value), IsHtml = true};

                    //queue email to be sent
                    _messaging.QueueEmail(msginfo);
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
                _log.Info("END: QuoteReminderTask");
            }
        }

        private Dictionary <int, QuotesByUser> GroupQuotesByUser()
        {
            var quotesByUsers = new Dictionary <int, QuotesByUser>();

            try
            {
                using(var ta = new QuoteStatusSummaryTableAdapter())
                {
                    using(QuoteDataSet.QuoteStatusSummaryDataTable openQuotes = ta.GetData())
                    {
                        foreach(QuoteDataSet.QuoteStatusSummaryRow openQuote in openQuotes)
                        {
                            if(!quotesByUsers.ContainsKey(openQuote.UserId))
                                quotesByUsers.Add(openQuote.UserId, new QuotesByUser {UserID = openQuote.UserId, Quotes = new List <QuoteDataSet.QuoteStatusSummaryRow>()});

                            QuotesByUser userQuotes = quotesByUsers[openQuote.UserId];
                            userQuotes.Quotes.Add(openQuote);
                        }
                    }
                }

                return quotesByUsers;
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error sending email notifications.";
                _log.Error(exc, errorMsg);
                return quotesByUsers;
            }
        }

        private string BuildEmailContent(ApplicationSettingsDataSet.TemplatesRow template, QuotesByUser quotesByUser)
        {
            string html = template.Template;
            HtmlNode rowTemplate = null;

            //replace common tokens
            html = html.Replace("%COUNT%", quotesByUser.Quotes.Count.ToString());

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var navigator = doc.CreateNavigator() as HtmlNodeNavigator;

            //get the row template and remove from the html doc
            if(navigator.MoveToId("ROW_TEMPLATE"))
            {
                rowTemplate = navigator.CurrentNode;
                rowTemplate.Id = ""; //reset id so it does not get cloned
                rowTemplate.Remove();
            }

            //go to the table node and add new row based on template row
            if(rowTemplate != null && navigator.MoveToId("QUOTE_TABLE"))
            {
                HtmlNode tableNode = navigator.CurrentNode;

                foreach(QuoteDataSet.QuoteStatusSummaryRow quote in quotesByUser.Quotes)
                {
                    //Update the new rows style
                    HtmlNode quoteRow = rowTemplate.Clone();
                    string quoteRowHtml = quoteRow.InnerHtml;

                    //Replace known Tokens
                    quoteRowHtml = quoteRowHtml.Replace("%QUOTEID%", HttpUtility.HtmlEncode(quote.QuoteID + (quote.IsRevisionNull() ? "" : "-" + quote.Revision)));
                    quoteRowHtml = quoteRowHtml.Replace("%COMPANY%", HttpUtility.HtmlEncode(quote.CustomerName));
                    quoteRowHtml = quoteRowHtml.Replace("%CONTACT%", HttpUtility.HtmlEncode(quote.ContactName));
                    quoteRowHtml = quoteRowHtml.Replace("%CREATED%", HttpUtility.HtmlEncode(quote.CreatedDate.ToShortDateString()));
                    quoteRowHtml = quoteRowHtml.Replace("%EXPIRE%", HttpUtility.HtmlEncode(quote.ExpirationDate.ToShortDateString()));

                    quoteRow.InnerHtml = quoteRowHtml;

                    //add new row to end of table
                    tableNode.AppendChild(quoteRow);
                }
            }

            return doc.DocumentNode.OuterHtml;
        }

        private ApplicationSettingsDataSet.TemplatesRow GetTemplate()
        {
            using(var ta = new TemplatesTableAdapter())
            {
                ApplicationSettingsDataSet.TemplatesDataTable templates = ta.GetDataById("QuoteReminder");
                return templates.FirstOrDefault();
            }
        }

        #endregion

        #region Nested type: QuotesByUser

        private class QuotesByUser
        {
            public int UserID { get; set; }
            public List <QuoteDataSet.QuoteStatusSummaryRow> Quotes { get; set; }
        }

        #endregion
    }
}