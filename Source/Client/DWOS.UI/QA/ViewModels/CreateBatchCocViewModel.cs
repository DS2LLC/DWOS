using AngleSharp.Parser.Html;
using DWOS.Data;
using DWOS.Data.Coc;
using DWOS.Data.Date;
using DWOS.Shared.Utilities;
using DWOS.UI.DesignTime;
using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DWOS.UI.QA.ViewModels
{
    public class CreateBatchCocViewModel : ViewModelBase
    {
        #region Fields

        public event EventHandler Completed;
        private const string FONT_START_TAG = "<font face=\"Verdana\" size=\"8pt\" color=\"Black\">";
        private const string FONT_RED_START_TAG = "<font color=\"Red\">";
        private const string FONT_END_TAG = "</font>";
        private const string BOLD_START_TAG = "<b>";
        private const string BOLD_END_TAG = "</b>";
        private const string NEWLINE = "<br>";

        private CertificateBatch _batch;
        private int _batchCocId;
        private string _customerName;
        private DateTime _dateCertified;
        private string _infoHtml;
        private bool _viewCoc;
        private bool _printCoc;
        private int _printCopies = 1;
        private ObservableCollection<object> _selectedContacts;

        #endregion

        #region Properties

        public CertificateBatch Batch
        {
            get => _batch;
            private set => Set(nameof(Batch), ref _batch, value);
        }

        public int BatchCocId
        {
            get => _batchCocId;
            set => Set(nameof(BatchCocId), ref _batchCocId, value);
        }

        public string CustomerName
        {
            get => _customerName;
            set => Set(nameof(CustomerName), ref _customerName, value);
        }

        public DateTime DateCertified
        {
            get => _dateCertified;
            set => Set(nameof(DateCertified), ref _dateCertified, value);
        }

        public string InfoHtml
        {
            get => _infoHtml;
            set => Set(nameof(InfoHtml), ref _infoHtml, value);
        }

        public ObservableCollection<CertificateBatchOrder> Orders { get; } =
            new ObservableCollection<CertificateBatchOrder>();

        public ISecurityUserInfo QualityInspector =>
            SecurityManager.UserInfo;

        public ObservableCollection<Contact> Contacts { get; } =
            new ObservableCollection<Contact>();

        /// <summary>
        /// Gets or sets the selected contacts for this instance.
        /// </summary>
        /// <remarks>
        /// The Infragistics control used to show this requires this
        /// property have a setter and be of this type.
        /// </remarks>
        public ObservableCollection<object> SelectedContacts
        {
            get => _selectedContacts;
            set => Set(nameof(SelectedContacts), ref _selectedContacts, value);
        }

        public bool ViewCoc
        {
            get => _viewCoc;
            set
            {
                if (Set(nameof(ViewCoc), ref _viewCoc, value) && value)
                {
                    PrintCoc = false;
                }
            }
        }

        public bool PrintCoc
        {
            get => _printCoc;
            set
            {
                if (Set(nameof(PrintCoc), ref _printCoc, value) && value)
                {
                    ViewCoc = false;
                }
            }
        }

        public int PrintCopies
        {
            get => _printCopies;
            set => Set(nameof(PrintCopies), ref _printCopies, value);
        }

        public CocPersistence Persistence { get; }

        public ISecurityManager SecurityManager { get; }

        public IDateTimeNowProvider NowProvider { get; }

        public ICreateBatchCocView View { get; private set; }

        public ICommand Complete { get; }

        #endregion

        #region Methods

        public CreateBatchCocViewModel()
        {
            if (IsInDesignMode)
            {
                SecurityManager = new FakeSecurityManager();
                Persistence = new CocPersistence(new FakeSettingsProvider().Settings);
                ShowDesignModeData();
            }
            else
            {
                SecurityManager = Utilities.SecurityManager.Current;
                Persistence = new CocPersistence(ApplicationSettings.Current);
            }

            NowProvider = new DateTimeNowProvider();

            Complete = new RelayCommand(DoComplete);
        }

        private void ShowDesignModeData()
        {
            CustomerName = "Example Customer";
            DateCertified = DateTime.Now;
            Batch = new CertificateBatch(100, null, new List<CertificateBatch.BatchProcess>());

            Orders.Add(new CertificateBatchOrder(1000, 1, 5,
                "Example Part",
                5,
                new List<string>(),
                null,
                new List<CertificateBatchOrder.OrderProcess>(),
                new List<CertificateBatchOrder.CustomField>(),
                new List<CertificateBatchOrder.PartCustomField>(),
                null,
                new List<CertificateBatchOrder.OrderRework>()));
        }

        public void LoadNew(int batchId, int customerId, List<int> orderIds)
        {
            if (orderIds == null)
            {
                throw new ArgumentNullException(nameof(orderIds));
            }

            _batchCocId = -1;

            DateCertified = NowProvider.Now;
            CustomerName = Persistence.GetCustomer(customerId)?.Name;

            Orders.Clear();

            foreach (var orderId in orderIds)
            {
                var order = Persistence.GetOrder(orderId, batchId);

                if (order != null)
                {
                    Orders.Add(order);
                }
            }

            Batch = Persistence.GetBatch(batchId);
            InfoHtml = CreateInfoHtml();

            Contacts.Clear();
            var selectedContacts = new ObservableCollection<object>();

            foreach (var contact in Persistence.GetContacts(customerId))
            {
                if (string.IsNullOrEmpty(contact.EmailAddress) || !contact.Active)
                {
                    continue;
                }

                Contacts.Add(contact);

                if (contact.CocNotification)
                {
                    selectedContacts.Add(contact);
                }
            }

            SelectedContacts = selectedContacts;
        }

        public void LoadView(ICreateBatchCocView view)
        {
            View = view ?? throw new ArgumentNullException(nameof(view));
            View.InfoHtml = HtmlUtilities.CreateHtmlDocument(_infoHtml);
        }

        public void UnloadView()
        {
            View = null;
        }

        public void SyncWithView()
        {
            InfoHtml = HtmlUtilities.ExtractHtmlBody(View.InfoHtml);
        }

        private void DoComplete()
        {
            try
            {
                SyncWithView();
                BatchCocId = Persistence.SaveCertificate(ToModel());
                Persistence.QueueBatchNotifications(BatchCocId, SelectedContacts.OfType<Contact>().ToList());
                Completed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error completing batch COC");
            }
        }

        public BatchCertificate ToModel() => new BatchCertificate
        {
            BatchCocId = _batchCocId,
            Batch = _batch,
            DateCertified = _dateCertified,
            QualityInspector = SecurityManager.UserInfo,
            Orders = Orders.ToList(),
            InfoHtml = _infoHtml
        };

        private string CreateInfoHtml()
        {
            var template = Persistence.GetTemplate()
                ?? "%PROCESSTEXT%<br>%ACCEPTEDTEXT% %REJECTEDTEXT% %TOTALTEXT%<br>";

            (var cocText, var currentCountData) = GetCocInfoData();

            return template
                .Replace("%PROCESSTEXT%", cocText)
                .Replace("%ACCEPTEDTEXT%", FONT_START_TAG + currentCountData.AcceptedHtml + FONT_END_TAG)
                .Replace("%REJECTEDTEXT%", FONT_START_TAG + currentCountData.RejectedHtml + FONT_END_TAG)
                .Replace("%TOTALTEXT%", FONT_START_TAG + currentCountData.TotalHtml + FONT_END_TAG)
                .Replace("%SERIALNUMBERS%", FONT_START_TAG + currentCountData.SerialNumbersHtml + FONT_END_TAG)
                .Replace("%IMPORTVALUE%", FONT_START_TAG + currentCountData.ImportValueHtml + FONT_END_TAG);
        }

        private CocInfoData GetCocInfoData()
        {
            var cocText = new StringBuilder();

            var lastProcessId = -1;
            var orderProcesses = Orders
                .SelectMany(wo => wo.Processes)
                .ToList();

            // Batch Processes
            foreach (var batchProcess in _batch.BatchProcesses)
            {
                // Assumptions:
                // - Customers want to see an alias name
                // - It is okay to show only one alias if the batch includes
                //   more than one.
                var orderProcess = orderProcesses
                    .FirstOrDefault(op => op.ShowOnCoc && batchProcess.OrderProcessesIds.Contains(op.OrderProcessesId));

                if (orderProcess == null)
                {
                    continue;
                }

                var addHeader = lastProcessId != batchProcess.ProcessId;

                cocText.Append(OrderProcessText(orderProcess, addHeader));

                lastProcessId = batchProcess.ProcessId;
            }

            // Order Processes that were not part of the batch
            lastProcessId = -1;
            var orderProcessIdsInBatch = new HashSet<int>(
                _batch.BatchProcesses.SelectMany(bp => bp.OrderProcessesIds));

            foreach (var order in Orders)
            {
                var additionalProcesses = order.Processes
                    .Where(p => !orderProcessIdsInBatch.Contains(p.OrderProcessesId) && p.ShowOnCoc)
                    .ToList();

                if (additionalProcesses.Count == 0)
                {
                    continue;
                }

                cocText.Append(NEWLINE)
                    .Append(FONT_START_TAG)
                    .Append($"Processes for Order {order.OrderId}:")
                    .Append(FONT_END_TAG)
                    .Append(NEWLINE);

                foreach (var process in additionalProcesses)
                {
                    var addHeader = lastProcessId != process.ProcessId;

                    cocText.Append(OrderProcessText(process, addHeader));

                    lastProcessId = process.ProcessId;
                }
            }

            // Custom Fields
            var visibleCustomFields = Orders
                .SelectMany(wo => wo.CustomFields)
                .Where(wo => wo.ShowOnCoc && !string.IsNullOrEmpty(wo.Value))
                .GroupBy(field => field.CustomFieldId)
                .ToList();

            if (visibleCustomFields.Count > 0)
            {
                cocText.Append(NEWLINE);
                cocText.Append(FONT_START_TAG);

                foreach (var orderFieldGroup in visibleCustomFields)
                {
                    var fieldName = orderFieldGroup.First().Name;
                    var fieldValues = string.Join(", ", orderFieldGroup.Select(group => group.Value).Distinct());
                    cocText.Append($"&nbsp;{ValidateText(fieldName), -25}:&nbsp;{ValidateText(fieldValues)}")
                        .Append(NEWLINE);
                }

                cocText.Append(FONT_END_TAG);
            }

            // Part-Level Custom Fields
            var visiblePartCustomFields = Orders
                .SelectMany(wo => wo.PartCustomFields)
                .Where(wo => wo.ShowOnCoc && !string.IsNullOrEmpty(wo.Value))
                .GroupBy(field => field.Name)
                .ToList();

            if (visiblePartCustomFields.Count > 0)
            {
                cocText.Append(NEWLINE);
                cocText.Append(FONT_START_TAG);

                foreach (var orderFieldGroup in visiblePartCustomFields)
                {
                    var fieldName = orderFieldGroup.Key;
                    var fieldValues = string.Join(", ", orderFieldGroup.Select(group => group.Value).Distinct());
                    cocText.Append($"&nbsp;{ValidateText(fieldName), -25}:&nbsp;{ValidateText(fieldValues)}")
                        .Append(NEWLINE);
                }

                cocText.Append(FONT_END_TAG);
            }


            // Part Marking
            foreach (var order in Orders)
            {
                var partMarkSpec = order.PartMark?.ProcessSpec;
                var partMarkDate = order.PartMark?.DateMarked;

                if (!string.IsNullOrEmpty(partMarkSpec) && partMarkDate.HasValue)
                {
                    cocText.Append(NEWLINE)
                        .Append(FONT_START_TAG)
                        .Append("&nbsp;")
                        .Append(order.OrderId)
                        .Append(" Part Marked as per '")
                        .Append(ValidateText(partMarkSpec))
                        .Append("'")
                        .Append(FONT_END_TAG)
                        .Append(NEWLINE);
                }
            }

            // Rework
            foreach (var order in Orders)
            {
                foreach (var rework in order.Reworks)
                {
                    if (!rework.ShowOnCoc)
                    {
                        continue;
                    }

                    cocText.Append(NEWLINE)
                        .Append(FONT_START_TAG)
                        .Append("&nbsp;")
                        .Append(BOLD_START_TAG)
                        .Append($"{order.OrderId} - {rework.Name}")
                        .Append(BOLD_END_TAG)
                        .Append(FONT_END_TAG)
                        .Append(NEWLINE);
                }
            }

            var totalQuantity = Orders.Sum(wo => (long)wo.BatchQuantity);

            var countData = new CocCountData
            {
                AcceptedCount = totalQuantity,
                RejectedCount = 0,
                TotalCount = totalQuantity,
                SerialNumbers = Orders.SelectMany(wo => wo.SerialNumbers).ToList(),
                ImportValue = Orders.Sum(wo => wo.ImportedPrice)?.ToString("C")
            };

            return new CocInfoData(cocText.ToString(), countData);
        }

        private static string OrderProcessText(CertificateBatchOrder.OrderProcess orderProcess, bool addHeader)
        {
            var processText = new StringBuilder();
            if (addHeader)
            {
                processText.Append(FONT_START_TAG)
                    .Append("&nbsp;", $"{ValidateText(orderProcess.Description)} Per {ValidateText(orderProcess.AliasName)}")
                    .Append(FONT_END_TAG)
                    .Append(NEWLINE);
            }

            foreach (var step in orderProcess.Steps)
            {
                var questionsToShow = step.Questions
                    .Where(ShowQuestion)
                    .ToList();

                if (!step.ShowOnCoc || questionsToShow.Count == 0)
                {
                    continue;
                }

                processText.Append(FONT_START_TAG)
                    .Append("&nbsp;".Repeat(4), ValidateText(step.Name))
                    .Append(FONT_END_TAG)
                    .Append(NEWLINE);

                foreach (var question in questionsToShow)
                {
                    processText.Append(FONT_START_TAG)
                        .Append("&nbsp;".Repeat(8), ValidateText(question.Name), ": ")
                        .Append(FONT_END_TAG)
                        .Append(FONT_RED_START_TAG)
                        .Append(Format(question))
                        .Append(FONT_END_TAG)
                        .Append(NEWLINE);
                }
            }

            var inspectionsToShow = orderProcess.Inspections
                .Where(inspection => inspection.ShowOnCoc && (inspection.Questions.Count > 0 || inspection.RejectedQuantity == 0))
                .ToList();

            if (inspectionsToShow.Count > 0)
            {
                processText.Append(NEWLINE);
            }

            foreach (var inspection in inspectionsToShow)
            {
                if (inspection.Questions.Count > 0)
                {
                    processText.Append(FONT_START_TAG)
                        .Append("&nbsp;".Repeat(4), ValidateText(inspection.Name))
                        .Append(FONT_END_TAG)
                        .Append(NEWLINE);
                }
                else
                {
                    processText.Append(FONT_START_TAG)
                        .Append("&nbsp;".Repeat(4), ValidateText(inspection.Name))
                        .Append(FONT_END_TAG)
                        .Append(FONT_RED_START_TAG)
                        .Append(" Pass")
                        .Append(FONT_END_TAG)
                        .Append(NEWLINE);
                }

                foreach (var question in inspection.Questions)
                {
                    //Question
                    processText.Append(FONT_START_TAG)
                        .Append("&nbsp;".Repeat(8), ValidateText(question.QuestionName) + ": ")
                        .Append(FONT_END_TAG);

                    //Answer
                    processText.Append(FONT_RED_START_TAG)
                        .Append(string.IsNullOrEmpty(question.Answer) ? "Unknown" : ValidateText(question.Answer))
                        .Append(FONT_END_TAG)
                        .Append(NEWLINE);
                }
            }

            return processText.ToString();
        }

        private static bool ShowQuestion(CertificateBatchOrder.OrderProcessQuestion question)
        {
            if (question == null || string.IsNullOrEmpty(question.Answer))
            {
                return false;
            }

            var answer = question.Answer;
            var inputTypeCategory = ControlUtilities.GetCategory(question.InputType);

            if (!question.IsRequired && inputTypeCategory == ControlUtilities.ControlCategory.Numeric && decimal.TryParse(answer, out var n) && n == 0)
            {
                return false;
            }

            return true;
        }

        private static string Format(CertificateBatchOrder.OrderProcessQuestion question)
        {
            if (question == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(question.NumericUnit) && !string.IsNullOrWhiteSpace(question.Answer))
            {
                return ValidateText($"{question.Answer} {question.NumericUnit}");
            }

            return string.IsNullOrEmpty(question.Answer) ? "Unknown" : ValidateText(question.Answer);
        }

        private static string ValidateText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return text.Replace(">", "-")
                .Replace("<", "-")
                .Replace("&", "&amp;");
        }

        #endregion

        #region CocInfoData

        private class CocInfoData
        {
            public string Text { get; }

            public CocCountData CountData { get; }

            public CocInfoData(string text, CocCountData countData)
            {
                Text = text;
                CountData = countData;
            }

            public void Deconstruct(out string text, out CocCountData countData)
            {
                text = Text;
                countData = CountData;
            }
        }

        #endregion
    }
}
