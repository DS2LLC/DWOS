using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.COCDatasetTableAdapters;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinTree;
using NLog;
using DWOS.UI.Reports;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.Data.Order;


namespace DWOS.UI
{
    public partial class CreateCOC : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private const string FONT_START_TAG = "<font face=\"Verdana\" size=\"8pt\" color=\"Black\">";
        private const string FONT_END_TAG = "</font>";
        private const string FONT_RED_START_TAG = "<font color=\"Red\">";
        private const string BOLD_START_TAG = "<b>";
        private const string BOLD_END_TAG = "</b>";
        private const string RETURN_TAG = "<br/>";
        private const int STATUS_FLYOUT_TIME = 3000;
        private const int TOTAL_FLYOUT_TIME_MS = 6000;
        private CocCountData _currentCountData;
        private int _orderID;
        private int _customerId;
        private int _originalPartQty;
        private int _partID;
        private SecurityFormWatcher _securityWatcher;
        private DWOS.Utilities.Validation.ValidatorManager _validators;

        #endregion

        #region Properties

        public int OrderID
        {
            get { return this._orderID; }
            set { this._orderID = value; }
        }

        #endregion

        #region Methods

        public CreateCOC()
        {
            InitializeComponent();

            if(DesignMode)
                return;

            this._securityWatcher = new SecurityFormWatcher(this, this.btnCancel);
            _validators = new DWOS.Utilities.Validation.ValidatorManager();
        }

        private void LoadData()
        {
            this.dsCOC.EnforceConstraints = false;
            this.dsCOC.COC.BeginLoadData();
            this.dsCOC.COCPart.BeginLoadData();
            this.dsCOC.Users.BeginLoadData();
            this.dsCOC.Media.BeginLoadData();
            this.dsCOC.Process.BeginLoadData();
            this.dsCOC.ProcessSteps.BeginLoadData();
            this.dsCOC.ProcessQuestion.BeginLoadData();
            this.dsCOC.ReworkSummary.BeginLoadData();
            this.dsCOC.d_Contact.BeginLoadData();

            // Dont load COC - always create a new COC every time Final Inspection is ran
            this.taCOCPart.FillByOrder(this.dsCOC.COCPart, this._orderID);
            this.taUsers.Fill(this.dsCOC.Users);

            // Calling Fill() populates the entire table; only signatures are needed.
            this.taMedia.FillSignatureMedia(this.dsCOC.Media);

            if(this.dsCOC.COCPart.Count > 0)
            {
                this._partID = this.dsCOC.COCPart[0].PartID;
                _customerId = taOrderSummary.GetCustomerId(_orderID) ?? -1;

                this.taOrderProcesses.Fill(this.dsCOC.OrderProcesses, this._orderID);
                this.taProcess.Fill(this.dsCOC.Process, this._orderID);
                this.taProcessSteps.Fill(this.dsCOC.ProcessSteps, this._orderID);
                this.taProcessQuestion.Fill(this.dsCOC.ProcessQuestion, this._orderID);
                this.taOrderProcessAnswer.Fill(this.dsCOC.OrderProcessAnswer, this._orderID);
                this.taContact.FillActiveByCustomer(this.dsCOC.d_Contact, _customerId);

                if (ApplicationSettings.Current.AllowAdditionalCustomersForContacts)
                {
                    taContact.FillSecondaryContactsByCustomer(dsCOC.d_Contact, _customerId);
                }
            }

            taReworkSummary.FillByOrder(dsCOC.ReworkSummary, _orderID);

            this.dsCOC.COC.EndLoadData();
            this.dsCOC.COCPart.EndLoadData();
            this.dsCOC.Users.EndLoadData();
            this.dsCOC.Media.EndLoadData();
            this.dsCOC.Process.EndLoadData();
            this.dsCOC.ProcessSteps.EndLoadData();
            this.dsCOC.ProcessQuestion.EndLoadData();
            this.dsCOC.ReworkSummary.EndLoadData();
            this.dsCOC.d_Contact.EndLoadData();
            this.dsCOC.EnforceConstraints = true;
        }

        private void LoadInspection()
        {
            //find user id
            int userID = SecurityManager.Current.UserID;
            this._originalPartQty = this.taCOC.GetPartQuantity(this._orderID).GetValueOrDefault(0);

            //if no record then create new COC
            if(this.bsCOC.Count == 0)
            {
                var drv              = this.bsCOC.AddNew() as DataRowView;
                var cocRow           = drv.Row as COCDataset.COCRow;
                cocRow.OrderID       = this._orderID;
                cocRow.DateCertified = DateTime.Now;
                cocRow.PartQuantity  = this._originalPartQty;
                cocRow.QAUser        = userID;
                cocRow.COCInfo       = LoadProcessCOCData();
                cocRow.IsCompressed  = false;
            }

            this.bsCOC.ResetCurrentItem();

            // Load part info
            var part = this.dsCOC.COCPart.FindByPartID(this._partID);
            if (part != null)
            {
                this.txtPartName.Text = part.Name;
                this.txtPartID.Text = part.IsMaterialNull() ? string.Empty : part.Material;
            }

            // Load order info
            this.numOrderQty.Value = this._originalPartQty;

            this.numGrossWeight.Value = taOrderSummary.GetGrossWeight(this._orderID) ?? 0M;

            // Setup media widgets
            this.partMediaWidget.Setup(new MediaWidget.SetupArgs()
            {
                MediaJunctionTable = this.dsCOC.Part_Media,
                MediaTable = this.dsCOC.Media,
                MediaJunctionTableParentRowColumn = this.dsCOC.Part_Media.PartIDColumn,
                MediaJunctionTableDefaultColumn = this.dsCOC.Part_Media.DefaultMediaColumn,
                AllowEditing = true,
                MediaLinkType = Documents.LinkType.Part,
                DocumentLinkTable = this.dsCOC.Part_DocumentLink,
                ScannerSettingsType = ScannerSettingsType.Part
            });

            this.orderMediaWidget.Setup(new MediaWidget.SetupArgs()
            {
                MediaJunctionTable = this.dsCOC.Order_Media,
                MediaTable = this.dsCOC.Media,
                MediaJunctionTableParentRowColumn = this.dsCOC.Order_Media.OrderIDColumn,
                AllowEditing = true,
                MediaLinkType = Documents.LinkType.WorkOrder,
                DocumentLinkTable = this.dsCOC.Order_DocumentLink,
                ScannerSettingsType = ScannerSettingsType.Order
            });

            this.dsCOC.EnforceConstraints = false; // manually loading media

            this.taPart_Media.FillByPart(this.dsCOC.Part_Media, this._partID);
            this.taPart_DocumentLink.FillByPart(this.dsCOC.Part_DocumentLink, this._partID);
            this.taMedia.FillByPart(this.dsCOC.Media, this._partID);

            this.taOrder_Media.FillByOrder(this.dsCOC.Order_Media, this._orderID);
            this.taOrder_DocumentLink.FillByOrder(this.dsCOC.Order_DocumentLink, this._orderID);
            this.taMedia.FillByOrder(this.dsCOC.Media, this._orderID);

            this.dsCOC.EnforceConstraints = true;

            this.partMediaWidget.LoadMedia(this.dsCOC.Part_Media.ToList<DataRow>(),
                this.dsCOC.Part_DocumentLink.ToList<DataRow>(),
                this._partID);

            this.orderMediaWidget.LoadMedia(this.dsCOC.Order_Media.ToList<DataRow>(),
                this.dsCOC.Order_DocumentLink.ToList<DataRow>(),
                this._orderID);

            //select operator
            if(userID >= 0)
            {
                ValueListItem userItem = this.cboInspector.FindItemByValue <int>(i => i == userID);
                if(userItem != null)
                    this.cboInspector.SelectedItem = userItem;
            }

            // Setup notifications dropdown
            cboNotifications.DataSource = dsCOC.d_Contact
                .Where(c => c.Active && !c.IsEmailAddressNull() && !string.IsNullOrEmpty(c.EmailAddress))
                .ToList();

            cboNotifications.ValueMember = dsCOC.d_Contact.ContactIDColumn.ColumnName;
            cboNotifications.DisplayMember = dsCOC.d_Contact.EmailAddressColumn.ColumnName;

            // Automatically check contacts with COCNotification
            foreach (var item in cboNotifications.Items)
            {
                var contactId = item.DataValue as int? ?? -1;
                var row = dsCOC.d_Contact.FindByContactID(contactId);

                if (row?.COCNotification ?? false)
                {
                    item.CheckState = CheckState.Checked;
                }
            }

            this.bsCOC.EndEdit();
        }

        private string LoadProcessCOCData()
        {
            if (this._partID <= 0)
            {
                return null;
            }

            string template = "%PROCESSTEXT%<br/>%ACCEPTEDTEXT% %REJECTEDTEXT% %TOTALTEXT%<br/>";

            using (var taTemplates = new TemplatesTableAdapter())
            {
                var templateRow = taTemplates
                    .GetDataById("COCData")
                    .FirstOrDefault();

                if (templateRow != null)
                {
                    template = templateRow.Template;
                }
            }

            var cocText = new StringBuilder();
            int lastProcessID = 0;
            int processIndex = 1;

            //get each order process step
            foreach(var orderProcess in this.dsCOC.OrderProcesses)
            {
                //if not showing COC data then skip this order process
                if (!orderProcess.COCData)
                    continue;

                //if first consecutive hit of the processes then write process header
                var addProcessHeader = orderProcess.ProcessRow.ProcessID != lastProcessID;
                lastProcessID = orderProcess.ProcessRow.ProcessID;
                
                if (processIndex > 1)
                    cocText.Append("&nbsp;");
                cocText.Append(OrderProcessCOCData(orderProcess, addProcessHeader));
                processIndex++;
            }

            //Add Custom Fields to COC
            using (var taOCF = new OrderCustomFieldsTableAdapter())
            {
                var orderCustomFields = taOCF.GetCOCValues(_customerId, _orderID);

                if(orderCustomFields.Count > 0)
                {
                    cocText.Append("<br/>");
                    cocText.Append(FONT_START_TAG);

                    foreach(OrdersDataSet.OrderCustomFieldsRow orderCustomField in orderCustomFields)
                    {
                        if(!orderCustomField.IsValueNull() && !String.IsNullOrWhiteSpace(orderCustomField.Value))
                        {
                            cocText.Append(String.Format("&nbsp;{0, -25}:&nbsp;{1}", ValidateText(orderCustomField["Name"].ToString()), ValidateText(orderCustomField.Value)))
                                .Append("<br/>");
                        }
                    }
                    cocText.Append(FONT_END_TAG);
                }
            }

            // Add part-level custom fields.
            using (var taPartCustomFields = new COCPartCustomFieldsTableAdapter())
            {
                var partCustomFields = taPartCustomFields.GetByPart(_partID);

                if(partCustomFields.Count > 0)
                {
                    cocText.Append("<br/>");
                    cocText.Append(FONT_START_TAG);

                    foreach (var orderCustomField in partCustomFields)
                    {
                        if (!orderCustomField.IsValueNull() && !string.IsNullOrWhiteSpace(orderCustomField.Value))
                        {
                            cocText
                                .Append($"&nbsp;{ValidateText(orderCustomField["Name"].ToString()), -25}:&nbsp;{ValidateText(orderCustomField.Value)}")
                                .Append("<br/>");
                        }
                    }

                    cocText.Append(FONT_END_TAG);
                }
            }

            //Add Part Marking data //Show actual spec used
            using (var taOPM = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderPartMarkTableAdapter())
            {
                var orderPMTable = new OrderProcessingDataSet.OrderPartMarkDataTable();
                taOPM.Fill(orderPMTable, _orderID);

                var orderPMRow = orderPMTable.FirstOrDefault();

                if (orderPMRow != null && !orderPMRow.IsProcessSpecNull() && !orderPMRow.IsPartMarkedDateNull())
                {
                    cocText.Append("<br/>")
                        .Append(FONT_START_TAG)
                        .Append("&nbsp;Part Marked as per '", this.ValidateText(orderPMRow.ProcessSpec), "'")
                        .Append(FONT_END_TAG)
                        .Append("<br/>");
                }
            }

            // Rework
            var fullRework =
                dsCOC.ReworkSummary.Where(r => r.OriginalOrderID == _orderID && r.IsReworkOrderIDNull());

            var splitRework =
                dsCOC.ReworkSummary.Where(r => !r.IsReworkOrderIDNull() && r.ReworkOrderID == _orderID);

            foreach (var rework in fullRework.Concat(splitRework))
            {
                if (!rework.ShowOnDocuments)
                {
                    continue;
                }

                cocText.Append("<br />")
                    .Append(FONT_START_TAG)
                    .Append("&nbsp;")
                    .Append(BOLD_START_TAG)
                    .Append(rework.Name)
                    .Append(BOLD_END_TAG)
                    .Append(FONT_END_TAG)
                    .Append("<br />");
            }

            //Add Total Count
            this._currentCountData = GetCountData();

            return template
                .Replace("%PROCESSTEXT%", cocText.ToString())
                .Replace("%ACCEPTEDTEXT%", FONT_START_TAG + this._currentCountData.AcceptedHtml + FONT_END_TAG)
                .Replace("%REJECTEDTEXT%", FONT_START_TAG + this._currentCountData.RejectedHtml + FONT_END_TAG)
                .Replace("%TOTALTEXT%", FONT_START_TAG + this._currentCountData.TotalHtml + FONT_END_TAG)
                .Replace("%SERIALNUMBERS%", FONT_START_TAG + this._currentCountData.SerialNumbersHtml + FONT_END_TAG)
                .Replace("%IMPORTVALUE%", FONT_START_TAG + this._currentCountData.ImportValueHtml + FONT_END_TAG);
        }

        private string OrderProcessCOCData(COCDataset.OrderProcessesRow orderProcess, bool addProcessHeader)
        {
            var orderProcessText = new StringBuilder();
            
            if (addProcessHeader)
            {
                string processAliasName = null;
                var CustomerProcessAliases = taProcess.GetCustomerAlias(orderProcess.ProcessID, _customerId);
                if (ApplicationSettings.Current.DisplayCustomerProcessAliasOnCoc && CustomerProcessAliases.Count > 0)
                {
                    processAliasName = taProcess.GetCustomerProcessAliasName(orderProcess.OrderProcessesID);
                }

                if (string.IsNullOrEmpty(processAliasName))
                {
                    processAliasName = taProcess.GetProcessAliasName(orderProcess.OrderProcessesID);
                }

                orderProcessText.Append(FONT_START_TAG)
                    .Append($"{ValidateText(orderProcess.ProcessRow.Description)} Per {ValidateText(processAliasName)}")
                    .Append(FONT_END_TAG)
                    .Append("<br/>");
            }

            //for each step that is required on COC
            foreach (var step in orderProcess.ProcessRow.GetProcessStepsRows())
            {
                //if has no COC Data then skip step
                if (!step.COCData)
                    continue;

                var stepText = new StringBuilder();
                var answerCount = 0;
                stepText.Append(FONT_START_TAG)
                    .Append("&nbsp;".Repeat(4), ValidateText(step.Name))
                    .Append(FONT_END_TAG)
                    .Append("<br/>");

                foreach (var question in step.GetProcessQuestionRows())
                {
                    // There may be duplicate answer rows.
                    // Use one that has been completed without a null answer.
                    var answerRow = question
                        .GetOrderProcessAnswerRows()
                        .FirstOrDefault(row => row.OrderProcessesID == orderProcess.OrderProcessesID
                            && row.Completed
                            && !row.IsAnswerNull());

                    //if no answer then skip writing out
                    if (answerRow == null)
                    {
                        continue;
                    }

                    var answer = answerRow.Answer;

                    // If this is an optional numeric question, and it is 0, skip it
                    var inputType = (InputType)Enum.Parse(typeof(InputType), question.InputType);
                    var inputTypeCategory = ControlUtilities.GetCategory(inputType);

                    if (!question.Required && inputTypeCategory == ControlUtilities.ControlCategory.Numeric && decimal.TryParse(answer, out var n) && n == 0)
                    {
                        continue;
                    }

                    answerCount++;

                    //append  units if exist
                    if (!question.IsNumericUntisNull() && !String.IsNullOrWhiteSpace(answer))
                    {
                        if (!answer.Contains(question.NumericUntis))
                            answer = answer + " " + question.NumericUntis;
                    }

                    stepText.Append(FONT_START_TAG)
                        .Append("&nbsp;".Repeat(8), ValidateText(question.Name), ": ")
                        .Append(FONT_END_TAG)
                        .Append(FONT_RED_START_TAG)
                        .Append(String.IsNullOrEmpty(answer) ? "Unknown" : ValidateText(answer))
                        .Append(FONT_END_TAG)
                        .Append("<br/>");
                }

                if (answerCount > 0)
                    orderProcessText.Append(stepText);
                else
                    _log.Info("COC {0} Process {1} Step {2} has no answers, skipping add to COC.".FormatWith(_orderID, orderProcess.ProcessRow.ProcessID, step.Name));
            }

            using (var taProcessInspections = new Data.Datasets.COCDatasetTableAdapters.ProcessInspectionsTableAdapter())
                taProcessInspections.FillByProcess(this.dsCOC.ProcessInspections, orderProcess.ProcessRow.ProcessID);

            var taPartInspection = new Data.Datasets.COCDatasetTableAdapters.PartInspectionTableAdapter();
            var inspections = taPartInspection.GetData(orderProcess.OrderProcessesID);
            //bool firstInspection = true;
            int InspIndex = 1;

            using (var taP = new PartInspectionAnswersTableAdapter())
            {
                foreach (var processInspection in this.dsCOC.ProcessInspections)
                {
                    var partInspection = inspections.FirstOrDefault(partInsp => partInsp.PartInspectionTypeID == processInspection.PartInspectionTypeID);

                    if (partInspection != null && processInspection.COCData)
                    {
                        var answers = taP.GetData(partInspection.PartInspectionID) ?? Enumerable.Empty<COCDataset.PartInspectionAnswersRow>();

                        if (answers.Any() || partInspection.RejectedQty == 0)
                        {

                           
                            if (answers.Any())
                            {
                                orderProcessText.Append(FONT_START_TAG)
                                    //.Append("&nbsp;".Repeat(4), "&#9899; &nbsp;")
                                    .Append("&nbsp;".Repeat(4),ValidateText( partInspection.Name))
                                    .Append(FONT_END_TAG);
                            }
                            else
                            {
                                orderProcessText.Append(FONT_START_TAG)
                                    //.Append("&nbsp;".Repeat(4), "&#9899; &nbsp;")
                                    .Append("&nbsp;".Repeat(4),ValidateText(partInspection.Name))
                                    .Append(FONT_END_TAG)
                                    .Append(FONT_RED_START_TAG)
                                    .Append(" Pass")
                                    .Append(FONT_END_TAG);
                            }
                            int QuestIndex = 1;
                            foreach (COCDataset.PartInspectionAnswersRow ans in answers)
                            {
                                orderProcessText.Append("<br/>");
                                //Question
                                orderProcessText.Append(FONT_START_TAG)
                                    //.Append( "- &nbsp;")
                                    .Append("&nbsp;".Repeat(8),ValidateText(ans.QuestionName) + ": ")
                                    .Append(FONT_END_TAG);

                                //Answer
                                orderProcessText.Append(FONT_RED_START_TAG)
                                    .Append(ans.IsAnswerNull() || string.IsNullOrEmpty(ans.Answer) ? "Unknown" : ValidateText(ans.Answer));
                                
                                //UoM if one is set
                                if (!ans.IsNumericUntisNull())
                                    orderProcessText.Append(" " + ans.NumericUntis);

                                orderProcessText.Append(FONT_END_TAG);

                                if ((ans.InputType == "SampleSet" )&&(!ans.IsMax1Null()) && (!ans.IsMin1Null()))
                                {
                                    orderProcessText.Append(FONT_START_TAG)
                                    .Append(", Max Value: ", ValidateText(ans.Max1.ToString()))
                                    .Append(", Min Value: ", ValidateText(ans.Min1.ToString()))
                                    .Append(", Samples Taken: ", ValidateText(ans.Samples.ToString()))
                                    .Append(FONT_END_TAG)
                                    .Append("<br/>");

                                }
                                
                                QuestIndex++;
                            }
                            InspIndex++;
                        }
                    }
                }
            }

            return orderProcessText.ToString();
        }

        private CocCountData GetCountData()
        {
            int totalCount = (this.numOrderQty.Value as int?) ?? this._originalPartQty;
            int acceptedCount = Convert.ToInt32(((DataRowView)this.bsCOC.Current).Row[this.dsCOC.COC.PartQuantityColumn.ColumnName]);

            List<string> serialNumbers;
            using (var taOrderSerialNumbers = new Data.Datasets.COCDatasetTableAdapters.OrderSerialNumberTableAdapter())
            {
                taOrderSerialNumbers.Fill(this.dsCOC.OrderSerialNumber);

                serialNumbers = dsCOC.OrderSerialNumber
                    .Where(i => i.OrderID == _orderID && i.Active)
                    .Select(s => s.IsNumberNull() ? string.Empty : s.Number)
                    .ToList();
            }

            var importedValue = String.Empty;
            using (var taOrder = new OrderTableAdapter())
            {
                var orderTable = new OrdersDataSet.OrderDataTable();
                taOrder.FillByOrderID(orderTable, _orderID);
                
                var orderRow = orderTable.FirstOrDefault();

                if (orderRow != null && !orderRow.IsImportedPriceNull())
                    importedValue = orderRow.ImportedPrice.ToString("C");
            }

            return new CocCountData()
            {
                AcceptedCount = acceptedCount,
                RejectedCount = totalCount - acceptedCount,
                TotalCount = totalCount,
                SerialNumbers = serialNumbers,
                ImportValue = importedValue
            };
        }

        private string ValidateText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return text.Replace(">", "-")
                .Replace("<", "-")
                .Replace("&", "&amp;");
        }

        private bool SaveData()
        {
            try
            {
                if (!_validators.ValidateControls())
                {
                    return false;
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error saving data.", exc);
                return false;
            }

            var successfullySavedData = false;

            try
            {
                this.bsCOC.EndEdit();

                var cocRow = this.dsCOC.COC.FirstOrDefault();

                if(cocRow != null)
                {
                    cocRow.COCInfo = ZipUtilities.CompressString(cocRow.COCInfo);
                    cocRow.IsCompressed = true;
                }

                CreateNotifications();

                this.taManager.UpdateAll(this.dsCOC);

                //Update status
                this.taOrderSummary.UpdateWorkStatus( ApplicationSettings.Current.WorkStatusShipping, this._orderID);

                //update work orders current location to shipping department as that is the next step
                this.taOrderSummary.UpdateOrderLocation(ApplicationSettings.Current.DepartmentShipping, this._orderID);

                OrderHistoryDataSet.UpdateOrderHistory(this._orderID, "Final Inspection", "COC created and order moved to " + ApplicationSettings.Current.DepartmentShipping + ".", SecurityManager.Current.UserName);
                TimeCollectionUtilities.StopAllOrderTimers(_orderID);

                successfullySavedData = true;
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error saving data.", exc);
                _log.Warn("DataSet Errors: {0}", this.dsCOC.GetDataErrors());

                successfullySavedData = false;
            }

            try
            {
                var cocRow = this.dsCOC.COC.FirstOrDefault();

                if (!successfullySavedData && cocRow != null && cocRow.IsCompressed)
                {
                    // Undo compression
                    cocRow.COCInfo = ZipUtilities.DecompressString(cocRow.COCInfo);
                    cocRow.IsCompressed = false;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "There was an error while recovering from a final inspection error.");
            }

            return successfullySavedData;
        }

        private void CreateNotifications()
        {
            var cocRow = dsCOC.COC.FirstOrDefault();

            if (cocRow == null)
            {
                return;
            }

            foreach (var item in cboNotifications.CheckedItems)
            {
                var contactId = item.DataValue as int? ?? -1;
                var contact = dsCOC.d_Contact.FindByContactID(contactId);

                if (contact == null)
                {
                    continue;
                }

                var newNotification = dsCOC.COCNotification.NewCOCNotificationRow();
                newNotification.COCRow = cocRow;
                newNotification.d_ContactRow = contact;
                dsCOC.COCNotification.AddCOCNotificationRow(newNotification);
            }
        }

        private void PrintReport()
        {
            try
            {
                //if visible then we can create the COC
                if (!this.numPrintQty.Visible || !this.chkPdf.Visible)
                {
                    return;
                }

                var report = new COCReport(this.dsCOC);

                if (((StateEditorButton)this.numPrintQty.ButtonsLeft[0]).Checked)
                {
                    var printQty = Convert.ToInt32(this.numPrintQty.Value);
                    Settings.Default.PrintQuantity = printQty; //update what was set
                    report.PrintReport(printQty);
                }

                if (this.chkPdf.Checked)
                {
                    report.DisplayReport();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error printing labels.");
            }
        }

        private void UpdateCOCText()
        {
            if (this._currentCountData == null)
            {
                return;
            }

            var cocRow = ((DataRowView)this.bsCOC.Current).Row as COCDataset.COCRow;

            if (cocRow != null && cocRow.COCInfo != null)
            {
                this.numPartQty.DataBindings[0].WriteValue();
                var newData = GetCountData();

                cocRow.COCInfo = cocRow.COCInfo
                    .Replace(this._currentCountData.AcceptedHtml, newData.AcceptedHtml)
                    .Replace(this._currentCountData.RejectedHtml, newData.RejectedHtml)
                    .Replace(this._currentCountData.TotalHtml, newData.TotalHtml)
                    .Replace(this._currentCountData.SerialNumbersHtml, newData.SerialNumbersHtml)
                    .Replace(this._currentCountData.ImportValueHtml, newData.ImportValueHtml);

                this._currentCountData = newData;
                this.txtCOCInfo.DataBindings[0].ReadValue(); //tell control to reread from datasource
            }
        }

        private void OnDisposing()
        {
            partMediaWidget.ClearMedia();
            orderMediaWidget.ClearMedia();
            _validators?.Dispose();
        }

        #endregion

        #region Events

        private void CreateCOC_Load(object sender, EventArgs e)
        {
            try
            {
                LoadData();
                LoadInspection();
                this.numPrintQty.Value = Settings.Default.PrintQuantity;

                this.btnOK.Select();

                using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
                {
                    var customers = new CustomersDataset.CustomerDataTable();
                    ta.FillByOrderID(customers, _orderID);
                    var customer = customers.FirstOrDefault();

                    if(customer != null && !customer.IsPrintCOCNull())
                    {
                        numPrintQty.Visible = customer.PrintCOC;
                        lblQuickPrint.Visible = customer.PrintCOC;
                        chkPdf.Visible = customer.PrintCOC;
                    }

                }



                var defaultCompleteSetting = ApplicationSettings.Current.DefaultCOCPrintSetting;

                ((StateEditorButton)numPrintQty.ButtonsLeft[0]).Checked = (defaultCompleteSetting == ReportPrintSetting.Printer);
                chkPdf.Checked = defaultCompleteSetting == ReportPrintSetting.Pdf;
            }
            catch(Exception exc)
            {
                this.grpInspection.Enabled = false;
                this.grpOrder.Enabled = false;
                this.grpPart.Enabled = false;
                this.numPrintQty.Enabled = false;
                this.btnOK.Enabled = false;
                ErrorMessageBox.ShowDialog("Error loading COC for Order " + this._orderID, exc);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if(SaveData())
                {
                    this.PrintReport();
                    this.DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error closing form.", exc);
                _log.Fatal(exc, "Error closing form.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) { Close(); }

        private void numPartQty_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateCOCText();
            }
            catch (Exception exc)
            {
                string errorMsg = "Error updating part qty text.";
                _log.Error(exc, errorMsg);
            }
        }

        private void numOrderQty_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateCOCText();
            }
            catch (Exception exc)
            {
                string errorMsg = "Error updating part qty text after updating order qty.";
                _log.Error(exc, errorMsg);
            }
        }

        private void chkPdf_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPdf.Checked)
                {
                    ((StateEditorButton)this.numPrintQty.ButtonsLeft[0]).Checked = false;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling checked change for view COC checkbox.");
            }
        }

        private void numPrintQty_AfterEditorButtonCheckStateChanged(object sender, EditorButtonEventArgs e)
        {
            try
            {
                if (((StateEditorButton)this.numPrintQty.ButtonsLeft[0]).Checked)
                {
                    chkPdf.Checked = false;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling check state changed for print COC checkbox.");
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                var report = new COCReport(this.dsCOC);
                report.DisplayReport();
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error showing COC preview.", exc);
            }
        }

        #endregion

        #region Nodes

        internal class MediaNode : DataNode <COCDataset.Order_MediaRow>
        {
            #region Fields

            public const string KEY_PREFIX = "ME";

            #endregion

            #region Properties

            public string Name
            {
                get;
                private set;
            }

            public string FileExtension
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public MediaNode(COCDataset.Order_MediaRow cr, string name, string fileExtension) : base(cr, cr.MediaID.ToString(), KEY_PREFIX, name)
            {
                this.Name = name;
                this.FileExtension = fileExtension;
                base.Override.CellClickAction = CellClickAction.SelectNodeOnly;

                base.LeftImages.Clear();
                base.LeftImages.Add(MediaUtilities.GetGenericThumbnail(fileExtension));
            }

            public override void UpdateNodeUI() { Text = this.Name; }

            #endregion
        }

        #endregion
    }
}