using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.Reports;
using DWOS.Server.Utilities;
using DWOS.Shared.Utilities;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DWOS.Server.Tasks
{
    [DisallowConcurrentExecution] // Do not run multiple times at once - may cause issues
    public class ReportNotificationTask : IJob
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private static readonly MessagingUtilities _messaging =
            new MessagingUtilities("ReportEmailTask");

        private EmailLogoCopier _copier = new EmailLogoCopier();

        #endregion

        #region Methods

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Task.Run(() => BeginProcessing()).ConfigureAwait(false);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error sending report notifications");
            }
        }

        private void BeginProcessing()
        {
            _log.Info("BEGIN: ReportNotificationTask");

            var reportSentCount = 0;

            try
            {
                var data        = GetData();
                var outputPath  = _messaging.EmailPickupDirectory;

                _copier.CopyCompanyLogo(_messaging.EmailPickupDirectory);

                var reportCache = new Dictionary<ReportCacheKey, PublishedReport>();

                using(var ta = new ReportTaskTableAdapter())
                {
                    //for each report task to be sent
                    foreach(ApplicationSettingsDataSet.ReportTaskRow task in data.ReportTask)
                    {
                        var taskContact = task.ContactSummaryRow;

                        if (taskContact.IsEmailAddressNull() || !taskContact.Active)
                        {
                            continue;
                        }

                        var exp = new CronExpression(task.Schedule); // i.e. "0 20 23 ? * MON-FRI"
                        DateTimeOffset? nextTime = exp.GetTimeAfter(new DateTimeOffset(task.LastRun.ToUniversalTime()));

                        //should have fired by now?
                        if (nextTime.Value.LocalDateTime >= DateTime.Now)
                        {
                            continue;
                        }

                        // Create/publish new report or use cached report
                        var reportType = task.ReportTypeRow;
                        var cacheKey = new ReportCacheKey(reportType.TypeName, taskContact.CustomerID);

                        if (!reportCache.TryGetValue(cacheKey, out var publishedReport))
                        {
                            var report = CreateReport(reportType, taskContact, task.LastRun);

                            if (report != null)
                            {
                                var fileName = report.GetFileName(outputPath);

                                if (File.Exists(fileName))
                                {
                                    _log.Warn($"Report file already exists: {fileName}");
                                }
                                else
                                {
                                    try
                                    {
                                        report.PublishReport(outputPath);
                                    }
                                    catch (IOException exc)
                                    {
                                        // Handle possibility of #21535
                                        // PublishReport(outputPath) fails if the file already exists.
                                        // That might happen if there are multiple instances of this
                                        // task running at the same time.
                                        _log.Warn(exc, "File already exists");
                                    }
                                }

                                publishedReport = new PublishedReport(report, fileName);
                                reportCache.Add(cacheKey, publishedReport);
                            }
                        }

                        var sendReport = publishedReport != null &&
                                         (!publishedReport.Report.HasData.HasValue || publishedReport.Report.HasData.Value) &&
                                         !String.IsNullOrEmpty(publishedReport.FileName);

                        if (sendReport)
                        {
                            var template = data.Templates.FindByTemplateID($"ReportNotification_{reportType.TypeName}")
                                ?? data.Templates.FindByTemplateID("ReportNotification");

                            SendEmailMessage(task, template, publishedReport.FileName);
                            reportSentCount++;
                        }

                        task.LastRun = DateTime.Now;
                        ta.Update(data.ReportTask);
                    }
                }
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error sending report notifications.";
                _log.Error(exc, errorMsg);
            }
            finally
            {
                _messaging.BeginProcessingEmails();
                _log.Info("END: ReportNotificationTask. Sent {0} reports.".FormatWith(reportSentCount));
            }
        }

        private ApplicationSettingsDataSet GetData()
        {
            var data = new ApplicationSettingsDataSet();
            data.EnforceConstraints = false;

            using(var ta = new ReportTypeTableAdapter())
                ta.Fill(data.ReportType);
            using(var ta = new ReportTaskTableAdapter())
                ta.Fill(data.ReportTask);
            using(var ta = new ContactSummaryTableAdapter())
                ta.FillByReportTask(data.ContactSummary);

            using (var ta = new ContactAdditionalCustomerTableAdapter())
            {
                ta.FillByReportTask(data.ContactAdditionalCustomer);
            }

            using (var ta = new TemplatesTableAdapter())
            {
                ta.Fill(data.Templates);
            }

            return data;
        }

        private Report CreateReport(ApplicationSettingsDataSet.ReportTypeRow reportType,
            ApplicationSettingsDataSet.ContactSummaryRow contact,
            DateTime lastRun)
        {
            var customerIds = GetCustomerIds(contact);

            if (Report.SecurityManager == null)
                Report.SecurityManager = SecurityManagerSimple.ServerSecurityImposter;

            Report report = null;

            switch (reportType.TypeName)
            {
                case "OpenOrdersReport":
                    // NOTE: This report notification type may be unused
                    _log.Error("Using unsupported OpenOrdersReport.");
                    report = new OpenOrdersByCustomerReport();
                    ((OpenOrdersByCustomerReport)report).CustomerID = contact.CustomerID;
                    ((OpenOrdersByCustomerReport)report).ReportType = ReportExportType.PDF;
                    break;
                case "CurrentOrderStatusReport":
                    report = new CurrentOrderStatusReport();
                    ((CurrentOrderStatusReport)report).CustomerIds = customerIds;

                    //determine if we should run for yesterday if this is running before 8 AM 
                    var date = DateTime.Now.Hour > 8 ? DateTime.Now : DateTime.Now.AddDays(-1);
                    ((CurrentOrderStatusReport)report).FromDate = date.Date;
                    ((CurrentOrderStatusReport)report).ToDate = date.Date;
                    break;
                case "OrderReceiptReport":
                    var neverRunDate = new DateTime(2000, 1, 1);

                    report = new OrderReceiptReport();
                    ((OrderReceiptReport)report).CustomerIds = customerIds;

                    ((OrderReceiptReport)report).FromDate = lastRun <= neverRunDate ? DateTime.Now.Date : lastRun;
                    ((OrderReceiptReport)report).ToDate = DateTime.Now.AddDays(1);
                    break;
            }

            return report;
        }

        private static List<int> GetCustomerIds(ApplicationSettingsDataSet.ContactSummaryRow contact)
        {
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }

            return new List<int> { contact.CustomerID }
                .Concat(contact.GetContactAdditionalCustomerRows().Select(c => c.CustomerID))
                .ToList();
        }

        private void SendEmailMessage(
            ApplicationSettingsDataSet.ReportTaskRow reportTask,
            ApplicationSettingsDataSet.TemplatesRow reportTemplate,
            string reportFilePath)
        {
            var msgInfo = new MessageInfo
            {
                FromAddress = new EmailAddress {Address = ApplicationSettings.Current.EmailFromAddress, DisplayName = "No Reply"},
                Subject = ApplicationSettings.Current.CompanyName + " - " + reportTask.ReportTypeRow.DisplayName + " Report"
            };

            msgInfo.ToAddresses = new List<EmailAddress>()
            {
                new EmailAddress {DisplayName = reportTask.ContactSummaryRow.Name, Address = reportTask.ContactSummaryRow.EmailAddress}
            };

            msgInfo.Attachments.Add(reportFilePath);

            var msgBody = new ReportNotificationHTML(reportTemplate);
            msgBody.ReplaceTokens(
                                new HtmlNotification.Token(ReportNotificationHTML.TAG_REPORT_NAME, reportTask.ReportTypeRow.DisplayName)
                              , new HtmlNotification.Token(ReportNotificationHTML.TAG_LOGO, _copier.CopiedLogoPath));
            msgBody.ReplaceSystemTokens();

            msgInfo.IsHtml = true;
            msgInfo.Body = msgBody.HTMLOutput;

            //queue email to be sent
            _messaging.QueueEmail(msgInfo);
        }

        #endregion

        #region ReportNotificationHTML

        internal class ReportNotificationHTML : HtmlNotification
        {
            #region Fields

            public static string TAG_REPORT_NAME = "%REPORTNAME%";

            #endregion

            #region Properties

            public ApplicationSettingsDataSet.TemplatesRow Template { get; }

            #endregion

            #region Methods

            public ReportNotificationHTML(ApplicationSettingsDataSet.TemplatesRow template)
            {
                Template = template ?? throw new ArgumentNullException(nameof(template));
            }

            protected override void CreateNotification()
            {
                HTMLOutput = Template.Template;
            }

            #endregion
        }

        #endregion

        #region ReportCacheEntry

        private class ReportCacheKey : IEquatable<ReportCacheKey>
        {
            public string ReportType { get; }

            public int CustomerId { get; }

            public ReportCacheKey(string reportType, int customerId)
            {
                ReportType = reportType;
                CustomerId = customerId;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return Equals(obj as ReportCacheKey);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = 23;

                    if (ReportType != null)
                    {
                        hash = hash * 29 + ReportType.GetHashCode();
                    }

                    hash = hash * 29 + CustomerId.GetHashCode();
                    return hash;
                }
            }

            public bool Equals(ReportCacheKey other)
            {
                return other != null &&
                    ReportType == other.ReportType &&
                    CustomerId == other.CustomerId;
            }
        }

        #endregion

        #region PublishedReport

        private class PublishedReport
        {
            public Report Report { get; }

            public string FileName { get; }

            public PublishedReport(Report report, string fileName)
            {
                Report = report;
                FileName = fileName;
            }
        }

        #endregion
    }
}