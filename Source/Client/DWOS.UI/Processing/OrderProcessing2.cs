using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Conditionals;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Shared;
using DWOS.UI.Processing;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinTree;
using NLog;
using DWOS.UI.Documents;
using DWOS.Data.Order.Activity;
using Infragistics.Win.UltraWinEditors;

namespace DWOS.UI
{
    public partial class OrderProcessing2 : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private bool _correctDept;

        private OrderProcessingDataSet.OrderProcessesRow _currentOrderProcess;
        private OrderProcessingDataSet.ProcessRow _currentProcess;

        private bool _isDirty;

        internal enum ProcesssStepStatus
        {
            Completed,
            InProgress,
            UnCompleted
        }

        public enum ProcessingMode { Normal, ViewOnly, Batch, Administrator }

        #endregion

        #region Properties

        public ProcessingActivity Activity { get; set; }

        public ProcessingActivity.ProcessingActivityResults ActivityResults { get; private set; }

        public bool IsProcessingCanceled { get; private set; }

        public ProcessingMode Mode { get; set; }

        private OrderProcessingDataSet.OrderSummaryRow OrderSummary => dsOrderProcessing.OrderSummary.FirstOrDefault();

        #endregion

        #region Methods

        public OrderProcessing2(ProcessingActivity activity)
        {
            InitializeComponent();

            Mode = ProcessingMode.Normal;
            Activity = activity;
            new SecurityFormWatcher(this, this.btnCancel);
        }

        private void LoadData()
        {
            //ensure this order has the answers created.
            OrderProcessingDataSet.OrderSummaryDataTable.CreateOrderAnswers(Activity.OrderID);

            this.dsOrderProcessing.EnforceConstraints = false;

            this.taOrder.FillById(this.dsOrderProcessing.OrderSummary, Activity.OrderID);
            this.taOrderProcessAnswer.FillBy(this.dsOrderProcessing.OrderProcessAnswer, Activity.OrderID);
            
            taCustomField.FillByOrder(dsOrderProcessing.CustomField,
                Activity.OrderID);

            taOrderCustomField.FillByOrder(dsOrderProcessing.OrderCustomFields,
                Activity.OrderID);

            if (Mode == ProcessingMode.Batch && Activity.BatchId.HasValue)
            {
                // Load serial numbers for batch
                taOrderSerialNumber.FillActiveByBatch(dsOrderProcessing.OrderSerialNumber,
                    Activity.BatchId.Value);
            }
            else
            {
                // Load serial numbers for order
                taOrderSerialNumber.FillActiveByOrder(dsOrderProcessing.OrderSerialNumber,
                    Activity.OrderID);
            }

            this.taUsers.Fill(this.dsOrderProcessing.Users);

            //find current process step we are working on
            if(Activity.OrderProcessID.HasValue)
            {
                using(var tOP = new OrderProcessesTableAdapter())
                {
                    //Attempt to find process by ID
                    tOP.FillByID(this.dsOrderProcessing.OrderProcesses, Activity.OrderProcessID.Value);
                    this._currentOrderProcess = this.dsOrderProcessing.OrderProcesses.FirstOrDefault();
                }
            }

            //Load Process Info
            if(this._currentOrderProcess != null)
            {
                using(var taP = new ProcessTableAdapter())
                {
                    taP.FillByProcess(this.dsOrderProcessing.Process, this._currentOrderProcess.ProcessID);
                    this._currentProcess    = this.dsOrderProcessing.Process.FirstOrDefault();
                    this._correctDept       = this._currentProcess.Department == Settings.Default.CurrentDepartment || Mode == ProcessingMode.Administrator; //if this not in this department, then mark as read-only

                    this.taProcessSteps.FillBy(this.dsOrderProcessing.ProcessSteps, this._currentOrderProcess.ProcessID);
                    this.taProcessQuestion.FillBy(this.dsOrderProcessing.ProcessQuestion, this._currentOrderProcess.ProcessID);
                    this.taQuestionField.FillByProcess(this.dsOrderProcessing.ProcessQuestionField, this._currentOrderProcess.ProcessID);
                }

                //If in batch mode auto answer the part qty so we can hide it to user
                if(Mode == ProcessingMode.Batch)
                {
                    IEnumerable <OrderProcessingDataSet.OrderProcessAnswerRow> partQtyRows = this._currentOrderProcess.GetOrderProcessAnswerRows().Where(opa => !opa.Completed && opa.ProcessQuestionRow.InputType == InputType.PartQty.ToString());
                    partQtyRows.ForEach(pqr => pqr.Completed = true);
                }

                taProcessStepCondition.Fill(dsOrderProcessing.ProcessStepCondition, _currentOrderProcess.ProcessID);
            }

            btnNextImage.Enabled = this.dsOrderProcessing.Media.Count > 1;
            numPartQty.Enabled = false;

            //if we are showing the order summary header info for this order
            if (Mode != ProcessingMode.Batch && this.dsOrderProcessing.OrderSummary.Count > 0)
            {
                var orderSummaryRow = this.dsOrderProcessing.OrderSummary[0];

                if (Mode == ProcessingMode.Normal) //allow editing of remaining parts
                {
                    var processedPartCount = (_currentOrderProcess != null && !_currentOrderProcess.IsPartCountNull() ? _currentOrderProcess.PartCount : 0);
                    int partCount = 0;

                    if (!orderSummaryRow.IsPartQuantityNull())
                    {
                        partCount = orderSummaryRow.PartQuantity - processedPartCount;
                    }

                    bool isOverallocated = partCount <= 0;

                    numPartQty.Enabled = true;

                    if (isOverallocated)
                    {
                        numPartQty.MinValue = 0;
                        numPartQty.MaxValue = 0;
                        numPartQty.Value = 0;
                    }
                    else
                    {
                        numPartQty.MinValue = Math.Min(partCount, 1); //set min value to the part count or 1, hopefully 1 is the lowest number
                        numPartQty.MaxValue = partCount;       //dont let user over allocate number of parts
                        numPartQty.Value = partCount;
                    }
                }
                else if (orderSummaryRow.IsPartQuantityNull())
                {
                    numPartQty.Value = 0;
                }
                else
                {
                    numPartQty.Value = orderSummaryRow.PartQuantity; //if in admin OR view mode show order count
                }

                //load part number
                this.taPartTable.FillBy(dsOrderProcessing.Part, orderSummaryRow.PartID);
                var partRow = this.dsOrderProcessing.Part.FirstOrDefault();


                //Setup media widget
                this.mediaWidget.Setup(new MediaWidget.SetupArgs()
                {
                    MediaJunctionTable = this.dsOrderProcessing.Part_Media,
                    MediaTable = this.dsOrderProcessing.Media,
                    MediaJunctionTableParentRowColumn = this.dsOrderProcessing.Part_Media.PartIDColumn,
                    MediaJunctionTableDefaultColumn = this.dsOrderProcessing.Part_Media.DefaultMediaColumn,
                    AllowEditing = true,
                    MediaLinkType = LinkType.Part,
                    DocumentLinkTable = this.dsOrderProcessing.Part_DocumentLink,
                    ScannerSettingsType = ScannerSettingsType.Part
                });
                
                //if part exists
                if (partRow != null)
                {
                    this.taMedia.FillByPartID(this.dsOrderProcessing.Media, partRow.PartID);
                    this.taPart_Media.FillByPartID(this.dsOrderProcessing.Part_Media, partRow.PartID);
                    this.taPartDocumentLink.FillByPartID(this.dsOrderProcessing.Part_DocumentLink, partRow.PartID);
                    this.mediaWidget.LoadMedia(partRow.GetPart_MediaRows().ToList<System.Data.DataRow>(),
                        partRow.GetPart_DocumentLinkRows().ToList<System.Data.DataRow>(),
                        partRow.PartID);

                    this.txtPartMaterial.Value = partRow.IsMaterialNull() ? null : partRow.Material;
                    this.txtPartName.Value = partRow.Name;
                }
                else
                {
                    this.mediaWidget.ClearMedia();
                }
            }

            LoadProcessDocumentLinks();
            LoadFields();

            // Show/Hide Required Date
            ApplicationSettingsDataSet.FieldsDataTable fields;

            using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
            {
                fields = ta.GetByCategory("Order");
            }

            var requiredDateField = fields.FirstOrDefault(f => f.Name == "Required Date");

            if (requiredDateField != null && !requiredDateField.IsVisible)
            {
                pnlOrderReqDate.Visible = false;
            }
        }

        private bool SaveData(bool canceling)
        {
            try
            {
                IsProcessingCanceled = canceling;

                if(Mode == ProcessingMode.ViewOnly)
                    return true;

                _log.Info("In saving data.");

                //if not in correct department then complete and return, Occurs before canceling because no matter how we get out of here we need to complete to move to the right department
                if(!this._correctDept)
                {
                    ActivityResults = Activity.Complete() as ProcessingActivity.ProcessingActivityResults;
                    return true;
                }

                if(canceling)
                    return true;

                // update media
                this.taManager_Media.UpdateAll(this.dsOrderProcessing);

                //save answers to db before trying to complete the activity
                SaveAnswers();

                //If in admin mode then do not update order status or complete the activity, only allow answers to be updated.
                if(Mode == ProcessingMode.Administrator)
                    return true;

                if(Mode != ProcessingMode.Batch) //in normal mode allow user to change value
                    Activity.CurrentProcessedPartQty = Convert.ToInt32(numPartQty.Value);

                //Complete the activity
                ActivityResults = Activity.Complete() as ProcessingActivity.ProcessingActivityResults;

                //IF there is a process that has to be completed next within a time constraint, then display warning
                if (ActivityResults != null && ActivityResults.NextRequisiteProcessID.HasValue && ActivityResults.NextRequisiteHours.HasValue)
                {
                    var processName = taProcess.GetProcessName(ActivityResults.NextRequisiteProcessID.Value);
                    MessageBoxUtilities.ShowMessageBoxWarn("Process {0} must be completed within the next {1} hours.".FormatWith(processName, ActivityResults.NextRequisiteHours.Value.ToString("N2")), "Processing Time Constraint");
                }

                return true;
            }
            catch(Exception exc)
            {
                _log.Warn("DataSet Errors: " + dsOrderProcessing.GetDataErrors());

                ErrorMessageBox.ShowDialog($"Error saving data for {Activity.OrderID}.", exc);
                return false;
            }
        }

        private void SaveAnswers()
        {
            if (Mode == ProcessingMode.Administrator)
            {
                // Add order history items for changed answers
                const int maxAnswerLength = 73;
                const string answerSuffix = "...";

                var updatedAnswers = dsOrderProcessing
                    .OrderProcessAnswer
                    .Where(r => r.RowState == DataRowState.Added || r.RowState == DataRowState.Modified);

                using (var taOrderHistory = new Data.Datasets.OrderHistoryDataSetTableAdapters.OrderHistoryTableAdapter())
                {
                    foreach (var updatedAnswer in updatedAnswers)
                    {
                        var newAnswerText = updatedAnswer.IsAnswerNull()
                            ? string.Empty
                            : updatedAnswer.Answer;

                        newAnswerText = newAnswerText.TrimToMaxLength(maxAnswerLength, answerSuffix);

                        


                        var originalAnswerText = string.Empty;
                        if (updatedAnswer.HasVersion(DataRowVersion.Original))
                        {
                            // Add item for updated answer
                            var originalAnswerObject = updatedAnswer[dsOrderProcessing.OrderProcessAnswer.AnswerColumn, DataRowVersion.Original];
                            originalAnswerText = (originalAnswerObject == DBNull.Value || originalAnswerObject == null)
                                ? string.Empty
                                : originalAnswerObject.ToString();

                            originalAnswerText = originalAnswerText.TrimToMaxLength(maxAnswerLength, answerSuffix);
                        }

                        // If you update this text, it may cause large text to be improperly truncated.
                        // Please update maxAnswerLength if the length of this text changes.
                        var historyDescription = $"Updated answer for question {updatedAnswer.ProcessQuestionID} " +
                            $"of order process {Activity.OrderProcessID}." +
                            $"\nOld: \"{originalAnswerText}\"" +
                            $"\nNew: \"{newAnswerText}\"";

                        taOrderHistory.UpdateOrderHistory(Activity.OrderID,
                            "OrderProcessing",
                            historyDescription,
                            SecurityManager.Current.UserName);
                    }
                }
            }

            taOrderProcessAnswer.Update(dsOrderProcessing.OrderProcessAnswer);

            // Double-check database to ensure that DWOS doesn't try to add
            // rows that are already there.
            var addedCustomFields = dsOrderProcessing
                .OrderCustomFields
                .Where(i => i.RowState == DataRowState.Added)
                .ToList();

            if (addedCustomFields.Count > 0)
            {
                using (var dtOrderCustomFieldsTemp = new OrderProcessingDataSet.OrderCustomFieldsDataTable())
                {
                    taOrderCustomField.FillByOrder(dtOrderCustomFieldsTemp, Activity.OrderID);

                    foreach (var addedCustomField in addedCustomFields)
                    {
                        if (dtOrderCustomFieldsTemp.FindByOrderIDCustomFieldID(addedCustomField.OrderID, addedCustomField.CustomFieldID) == null)
                        {
                            continue;
                        }

                        _log.Info($"Marking custom field {addedCustomField.CustomFieldID} for WO {addedCustomField.OrderID} as 'Modified'.");
                        addedCustomField.AcceptChanges();
                        addedCustomField.SetModified();
                    }
                }
            }

            taOrderCustomField.Update(dsOrderProcessing.OrderCustomFields);
        }


        private void SaveAnswerSamples(int opa)
        {
            
        
        
        }
        private void UpdateStatusBar()
        {
            try
            {
                int completedQuestions = 0;
                int totalQuestions = 0;
               
                var processSteps = this.tvwProcessSteps.Nodes.OfType<ProcessStepNode>().ToList();
                processSteps.ForEach(psn =>
                                        {
                                            completedQuestions += psn.CompletedQuestionCount;
                                            totalQuestions += psn.TotalQuestionCount;
                                        });

                //show paper processes as always completed
                if (_currentProcess != null && !_currentProcess.IsPaperless)
                    completedQuestions = totalQuestions;

                
                this.statusBar.Panels["Progress"].ProgressBarInfo.Maximum = totalQuestions;
                this.statusBar.Panels["Progress"].ProgressBarInfo.Value   = completedQuestions;
                this.statusBar.Panels["Progress"].ProgressBarInfo.Label   = completedQuestions + "/" + totalQuestions;
                this.statusBar.Panels["Progress"].ToolTipText             = "Completed {0} of {1} Process Questions".FormatWith(completedQuestions, totalQuestions);

                this.statusBar.Panels["Department"].Text = Settings.Default.CurrentDepartment;
                this.statusBar.Panels["UserName"].Text = SecurityManager.Current.UserName;
                
                if(_currentOrderProcess != null && _currentOrderProcess.ProcessRow != null)
                {
                    this.statusBar.Panels["PaperMode"].Text = _currentOrderProcess.ProcessRow.IsPaperless ? "Paperless Mode" : "Paper Mode";
                    this.statusBar.Panels["PaperMode"].Appearance.Image = _currentOrderProcess.ProcessRow.IsPaperless ? Properties.Resources.Paperless_16 : Properties.Resources.Paper_16;
                    this.statusBar.Panels["PaperMode"].Appearance.ForeColor = _currentOrderProcess.ProcessRow.IsPaperless ? Color.Green : Color.Blue;
                    this.statusBar.Panels["PaperMode"].ToolTipText = _currentOrderProcess.ProcessRow.IsPaperless ? "Paperless Mode: All questions are completed in DWOS." : "Paper Mode: All questions are completed on paper process sheets.";
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error updating status bar.");
            }
        }

        private void DisplayWarning(string text)
        {
            this.lblWarning.Text = text;
            this.lblWarning.Visible = true;
        }

        private void LoadProcessSteps()
        {
            this.tvwProcessSteps.Nodes.Clear();

            if(this._currentOrderProcess == null)
                return;

            if(this._currentOrderProcess.ProcessRow != null)
                _log.Info("Loading process step " + this._currentOrderProcess.ProcessRow.Name);

            this.lblProcessName.Text = this._currentProcess.Name;

            using(new UsingTreeLoad(this.tvwProcessSteps))
            {
                foreach(var processStep in this.dsOrderProcessing.ProcessSteps)
                {
                    var psNode = new ProcessStepNode(processStep, this._currentOrderProcess);
                    psNode.UpdateStepStatus();
                    this.tvwProcessSteps.Nodes.Add(psNode);
                }
            }

            ValidateStepConditions();

            // Select first node that should be selected
            var selectNode = this.tvwProcessSteps.Nodes
                .OfType<ProcessStepNode>()
                .FirstOrDefault(node => node.StepStatus == ProcesssStepStatus.InProgress || node.StepStatus == ProcesssStepStatus.UnCompleted);

            if(selectNode != null)
                selectNode.Select();

            //display warning if not in right department and in normal mode
            if (!this._correctDept && Mode == ProcessingMode.Normal)
            {
                DisplayWarning("No questions to answer. Please continue part to the next department.");
                this.btnOK.Enabled = false;
            }
        }

        private void LoadQuestions(ProcessStepNode processStepNode)
        {
            try
            {
                using(new UsingWaitCursor(this))
                {
                    _log.Info("Loading questions for step: " + processStepNode.Text);

                    SuspendLayout();
                    this.flowQuestions.SuspendLayout();
                    this.pnlProcessStep.SuspendLayout();

                    this.flowQuestions.Controls.Clear();

                    //Set up Step Header
                    this.lblStepNumber.Text     = processStepNode.DataRow.StepOrder.ToString();
                    this.lblStepName.Text       = processStepNode.DataRow.Name;
                    this.txtStepDesc.Value      = processStepNode.DataRow.IsDescriptionNull() ? "" : processStepNode.DataRow.Description;
                    this.lblStepDepartment.Text = this._currentProcess.Department;

                    //Create questions if not already created
                    if(processStepNode.QuestionControls == null)
                    {
                        processStepNode.QuestionControls = new List <OrderProcessQuestion>();

                        foreach(var pqRow in processStepNode.DataRow.GetProcessQuestionRows())
                        {
                            var questionControl = new OrderProcessQuestion { IsAdminMode = this.Mode == ProcessingMode.Administrator };

                            questionControl.CreateQuestion(pqRow, processStepNode, _currentOrderProcess, OrderSummary);
                            questionControl.OnSave += questionControl_OnSave;

                            var opaRow = processStepNode.GetAnswer(pqRow);
                            

                            if(opaRow != null)
                                questionControl.LoadAnswer(opaRow, this.dsOrderProcessing);
                            
                            questionControl.Enabled = Mode != ProcessingMode.ViewOnly && this._correctDept;
                            
                            processStepNode.QuestionControls.Add(questionControl);
                        }
                    }

                    foreach (var questionControl in processStepNode.QuestionControls)
                    {
                        //If in batch mode then don't show part qty
                        if (Mode == ProcessingMode.Batch && questionControl.AnswerRow.ProcessQuestionRow.InputType == InputType.PartQty.ToString())
                            continue;
                        
                        questionControl.ValidateStatus(); //Validate if the answer has changed outside of the question
                        this.flowQuestions.Controls.Add(questionControl);
                    }

                    MoveToNextQuestion(null);
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading questions", exc);
            }
            finally
            {
                this.flowQuestions.ResumeLayout();
                this.pnlProcessStep.ResumeLayout();
                ResumeLayout();
            }
        }

        private void DisposeMe()
        {
            try
            {
                this.tvwProcessSteps.Nodes.DisposeAll();
                this._currentOrderProcess = null;
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error disposing of the Order Processing form.");
            }
        }

        private void LoadProcessDocumentLinks()
        {
            this.docLinkList1.ClearLinks();

            if(this._currentProcess != null)
            {
                this.docLinkList1.LoadLinks(LinkType.Process, this._currentProcess.ProcessID);
                this.docLinkList1.LoadLinks(LinkType.ProcessAlias, this._currentOrderProcess.ProcessAliasID);
                this.docLinkList1.SelectDefaultLink();
            }
        }

        private void LoadFields()
        {
            var tableRowCount = 0;

            // Custom Fields
            var showCustomFields = dsOrderProcessing.OrderCustomFields
                .Any(c =>
                {
                    var customField = c.CustomFieldRow;

                    return customField != null && customField.ProcessUnique && customField.IsVisible;
                });

            if (showCustomFields)
            {
                AddFieldHeader("Custom Fields:", tableRowCount);
                tableRowCount++;

                foreach (var orderCustomField in dsOrderProcessing.OrderCustomFields)
                {
                    var customField = orderCustomField.CustomFieldRow;

                    if (customField == null || !customField.ProcessUnique || !customField.IsVisible)
                    {
                        continue;
                    }

                    var label = new Label
                    {
                        Anchor = AnchorStyles.Left | AnchorStyles.Top,
                        Text = customField.Name + ":",
                        TextAlign = ContentAlignment.TopLeft,
                        AutoSize = true,
                        Padding = new Padding(0, 3, 0, 0)
                    };

                    var txtBox = new UltraTextEditor
                    {
                        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                        MaxLength = 255,
                        AcceptsReturn = true,
                        Multiline = true,
                        Scrollbars = ScrollBars.Both,
                        Height = 35,
                        WordWrap = false,
                        Text = orderCustomField.IsValueNull() ? string.Empty : orderCustomField.Value,
                        ReadOnly = true
                    };

                    tableFields.Controls.Add(label, 0, tableRowCount);
                    tableFields.Controls.Add(txtBox, 1, tableRowCount);
                    tableRowCount++;
                }
            }

            // Serial Numbers
            var showSerialNumbers = FieldUtilities.IsFieldEnabled("Order", "Serial Number") &&
                dsOrderProcessing.OrderSerialNumber.Any(c => c.Active && !c.IsNumberNull());

            if (showSerialNumbers)
            {
                AddFieldHeader("Serial Numbers:", tableRowCount);
                tableRowCount++;

                var serialNumbers = Enumerable.Select(dsOrderProcessing.OrderSerialNumber
                                             .Where(s => s.Active && !s.IsNumberNull()), s => s.Number);

                foreach (var serialNumber in serialNumbers)
                {
                    var label = new Label
                    {
                        Anchor = AnchorStyles.Left | AnchorStyles.Top,
                        Text = serialNumber,
                        TextAlign = ContentAlignment.TopLeft,
                        AutoSize = true,
                        Padding = new Padding(0, 3, 0, 3)
                    };

                    tableFields.Controls.Add(label, 0, tableRowCount);
                    tableFields.SetColumnSpan(label, 2);
                }
            }

            // Show/Hide fields table
            if (showCustomFields || showSerialNumbers)
            {
                // Adjust horizontal container - helps custom field table not
                // show horizontal scroll bar by default
                const int amount = 25; // trial & error
                splitContainer1.SplitterDistance += amount;
                splitContainer1.SplitterDistance -= amount;
            }
            else
            {
                sidebarSplitContainer.Panel1Collapsed = true;
            }
        }

        private void AddFieldHeader(string text, int tableRowCount)
        {
            var headerLabel = new Label
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 10),
                Font = new Font("Verdana", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0)
            };

            tableFields.Controls.Add(headerLabel, 0, tableRowCount);
            tableFields.SetColumnSpan(headerLabel, 2);
        }

        private void LoadStepDocumentLinks(ProcessStepNode processStepNode)
        {
            try
            {
                this.docLinkList2.ClearLinks();

                if(processStepNode.ID != null)
                {
                    this.docLinkList2.LoadLinks(LinkType.ProcessSteps, Convert.ToInt32(processStepNode.ID));
                    this.docLinkList2.SelectDefaultLink();
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading step document links.");
            }
        }

        private void CheckForRequisiteViolation()
        {
            try
            {
                using (var tOP = new Data.Datasets.OrdersDataSetTableAdapters.OrderProcessesTableAdapter())
                {
                    var orderProcesses = new OrdersDataSet.OrderProcessesDataTable();
                    tOP.FillBy(orderProcesses, Activity.OrderID);

                    var orderProcess = orderProcesses.FirstOrDefault(op => op.OrderProcessesID == _currentOrderProcess.OrderProcessesID);

                    if (orderProcess != null && !orderProcess.IsRequisiteProcessIDNull() && !orderProcess.IsRequisiteHoursNull())
                    {
                        var requisiteProcessID = orderProcess.RequisiteProcessID;

                        //get the last completed mathcing pre-requisite process and ensure we are not to late
                        var op = orderProcesses.LastOrDefault(o => !o.IsEndDateNull() && o.ProcessID == requisiteProcessID);
                        if (op != null)
                        {
                            var difference = DateTime.Now.Subtract(op.EndDate);
                            if ((decimal)difference.TotalHours > orderProcess.RequisiteHours)
                            {
                                MessageBoxUtilities.ShowMessageBoxWarn("Process has exceeded time constraint for the previous process {0}, constrained to {1} hours.".FormatWith(taProcess.GetProcessName(requisiteProcessID), orderProcess.RequisiteHours.ToString("n2")), "Time Constraint Exceeded");
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error checking for requisite violation.");
            }
        }

        private void MoveToNextQuestion(Control currentQuestion)
        {
            try
            {
                var nextQuestionIndex = currentQuestion == null ? 0 : this.flowQuestions.Controls.IndexOf(currentQuestion) + 1;

                //move to next question if still are questions in this step
                for (int index = nextQuestionIndex; index < this.flowQuestions.Controls.Count; index++)
                {
                    var nextQuestion = this.flowQuestions.Controls[index] as OrderProcessQuestion;

                    if (!nextQuestion.AnswerRow.Completed)
                    {
                        this.flowQuestions.ScrollControlIntoView(nextQuestion);
                        nextQuestion.SetFocus();
                        return;
                    }
                }

                if (currentQuestion == null) //if no current question then don't change steps
                {
                    //if all questions in this step are answered and we just moved to this step then select the last question
                    if (this.flowQuestions.Controls.Count > 0)
                    {
                        var lastQuestion = this.flowQuestions.Controls[this.flowQuestions.Controls.Count - 1] as OrderProcessQuestion;
                        if (lastQuestion != null)
                        {
                            this.flowQuestions.ScrollControlIntoView(lastQuestion);
                            lastQuestion.SetFocus();
                        }
                    }
                    return;
                }

                var currentProcessStepNode = tvwProcessSteps.SelectedNode<ProcessStepNode>();

                //else move to next Step
                if (currentProcessStepNode != null)
                {
                    var node = currentProcessStepNode.GetNextVisibleSibling();

                    if (node != null)
                        node.Select(); //select
                    else
                        btnOK.Focus(); //else move to OK button
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error moving to next question.");
            }
        }

        private void ValidateStepConditions()
        {
            tvwProcessSteps.Nodes.OfType <ProcessStepNode>().ForEach(psn => psn.ValidationStepConditions());
        }

        #endregion

        #region Events

        private void OrderProcessing2_Load(object sender, EventArgs e)
        {
            try
            {
                _log.Info("OrderProcessing: Loading order: " + Activity.OrderID + " in " + Mode.ToString() + " Mode.");

                using(new UsingWaitCursor(this))
                {
                    Activity.Initialize();

                    //if not showing header then hide it and move grid up
                    if (Mode == ProcessingMode.Batch)
                    {
                        this.grpOrderInfo.Visible = false;
                        this.grpPartInfo.Visible  = false;
                        this.pnlProcessSteps.Dock = DockStyle.Fill;
                    }

                    Refresh();
                    LoadData();

                    //if did NOT find current processing step we should be on
                    if(this._currentOrderProcess == null)
                        DisplayWarning("Order is not in correct process. Please move order to the current department.");
                    else
                    {
                        CheckForRequisiteViolation();
                        LoadProcessSteps();
                    }

                    UpdateStatusBar();

                    //disable OK button if in view only mode
                    this.btnOK.Enabled = Mode != ProcessingMode.ViewOnly;

                    if (Mode == ProcessingMode.ViewOnly)
                    {
                        this.lblWarning.Appearance.Image = Properties.Resources.Question_16;
                        this.lblWarning.Appearance.ForeColor = Color.Blue;
                        this.lblWarning.Text = "View Only Mode";
                        this.lblWarning.Visible = true;
                    }

                    //IF paper based then show message to user
                    if(_currentProcess != null && !_currentProcess.IsPaperless)
                    {
                        if (!this.lblWarning.Visible) //If not showing some other warning then put Paper info
                        {
                            this.lblWarning.Appearance.Image = Properties.Resources.Paper_16;
                            this.lblWarning.Appearance.ForeColor = Color.Green;
                            this.lblWarning.Text = "Process is Paper-Based.";
                            this.lblWarning.Visible = true;
                        }

                        MessageBoxUtilities.ShowMessageBoxOK("This is a paper-based process. Please ensure the process is completed on paper.", "Paper Process", "A paper-based process requires clicking OK in order processing to proceed.");
                    }
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
            }
        }

        private void OrderProcessing2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(this.Mode == ProcessingMode.ViewOnly)
                return;

            if(DialogResult != DialogResult.OK && this._isDirty)
            {
                var saveData = MessageBoxUtilities.ShowMessageBoxYesOrNo("Some changes have not been saved. Would you like to save your changes?", "Unsaved Changes", "Use the OK button to save changes.");

                if (saveData == DialogResult.Yes)
                    SaveData(false);
            }
        }

        private void tvwProcessSteps_AfterSelect(object sender, SelectEventArgs e)
        {
            if(e.NewSelections.Count == 1 && e.NewSelections[0] is ProcessStepNode)
            {
                _log.Debug("Loading Question event: {0}", e.NewSelections[0].Text);
                var node = e.NewSelections[0] as ProcessStepNode;

                LoadQuestions(node);
                LoadStepDocumentLinks(node);
               
                if(node != null)
                    node.UpdateStepStatus();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                _log.Info("In OK button");

                if(SaveData(false))
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error closing form.", exc);
                _log.Fatal(exc, "Error closing form.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                _log.Info("In Cancel button");

                SaveData(true);
                Close();
                DialogResult = DialogResult.Cancel;
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error closing form.", exc);
                _log.Fatal(exc, "Error closing form.");
            }
        }

        private void questionControl_OnSave(object sender, EventArgs e)
        {
            try
            {
                this._isDirty = true;
                
                _log.Debug("Question saved event: {0}", this.tvwProcessSteps.SelectedNodes.Count == 1 ? this.tvwProcessSteps.SelectedNodes[0].Text : "NA");

                var currentProcessStepNode = tvwProcessSteps.SelectedNode<ProcessStepNode>();

                if (currentProcessStepNode != null)
                    currentProcessStepNode.UpdateStepStatus();

                var questionControl = sender as OrderProcessQuestion;

                //if this is a Part Qty question then update the part qty in the header with the set value
                if (questionControl != null && questionControl.QuestionRow != null && questionControl.AnswerRow != null)
                {
                    var inputType = questionControl.QuestionRow.InputType.ConvertToEnum<InputType>();

                    if(inputType == InputType.PartQty && questionControl.AnswerRow.Completed && !questionControl.AnswerRow.IsAnswerNull())
                    {
                        var partQty = 0;
                        
                        if(int.TryParse(questionControl.AnswerRow.Answer, out partQty))
                        {
                            if (partQty >= Convert.ToInt32(numPartQty.MinValue) && partQty <= Convert.ToInt32(numPartQty.MaxValue))
                                numPartQty.Value = partQty;
                        }
                    }
                }

                ValidateStepConditions();
                MoveToNextQuestion(sender as Control);
                UpdateStatusBar();
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error moving to next question.", exc);
            }
        }

        private void btnVideo_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    Process.Start(VideoLinks.ProcessingTutorial);
                }
                catch (Exception exc)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("The program associated with playing videos is not found or is unable to start.", "Play Video");
                    NLog.LogManager.GetCurrentClassLogger().Debug(exc, "Error starting process for: " + VideoLinks.ProcessingTutorial);
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error displaying video tutorial.");
            }
        }

        #endregion

        #region ProcessStepNode

        internal class ProcessStepNode : DataNode <OrderProcessingDataSet.ProcessStepsRow>
        {
            #region Fields

            private OrderProcessingDataSet.OrderProcessesRow _opr;
            private ProcesssStepStatus _processStatus;
            private int? _questionCount;
            private const string KEY_PREFIX = "PSN";

            #endregion

            #region Properties

            public List <OrderProcessQuestion> QuestionControls { get; set; }

            public ProcesssStepStatus StepStatus
            {
                get { return this._processStatus; }
                set
                {
                    this._processStatus = value;

                    switch(this._processStatus)
                    {
                        case ProcesssStepStatus.Completed:
                            Override.NodeAppearance.Image = "Complete";
                            break;
                        case ProcesssStepStatus.InProgress:
                            Override.NodeAppearance.Image = "Inprocess";
                            break;
                        case ProcesssStepStatus.UnCompleted:
                            Override.NodeAppearance.Image = "Incomplete";
                            break;
                        default:
                            break;
                    }
                }
            }

            public int CompletedQuestionCount
            {
                get
                {
                    int completedQuestionCount = 0;
                    var ds = this._opr.Table.DataSet as OrderProcessingDataSet;
                    OrderProcessingDataSet.ProcessQuestionRow[] questions = base.DataRow.GetProcessQuestionRows();

                    if(this._questionCount == null)
                        this._questionCount = questions.Count();

                    foreach(OrderProcessingDataSet.ProcessQuestionRow question in questions)
                    {
                        // There should be (at most) one answer per question, but
                        // that might not be true.
                        var answerForQuestion = _opr.GetOrderProcessAnswerRows()
                            .Where(opa => opa.ProcessQuestionID == question.ProcessQuestionID)
                            .OrderBy(opa => opa.OrderProcesserAnswerID)
                            .FirstOrDefault();

                        if (answerForQuestion != null && answerForQuestion.Completed)
                        {
                            completedQuestionCount++;
                        }
                    }

                    return completedQuestionCount;
                }
            }

            public int TotalQuestionCount
            {
                get
                {
                    if(!this._questionCount.HasValue)
                        this._questionCount = base.DataRow.GetProcessQuestionRows().Count();

                    return this._questionCount.GetValueOrDefault();
                }
            }

            public bool IsExcluded { get; set; }

            #endregion

            #region Methods

            public ProcessStepNode(OrderProcessingDataSet.ProcessStepsRow cr, OrderProcessingDataSet.OrderProcessesRow opr) : base(cr, cr.ProcessStepID.ToString(), KEY_PREFIX, cr.StepOrder + " - " + cr.Name)
            {
                _opr = opr;

                if (this.DataRow.GetProcessStepConditionRows().Any())
                    this.RightImages.Add(Properties.Resources.Equal_16);
            }

            public void UpdateStepStatus()
            {
                ProcesssStepStatus status;

                if (TotalQuestionCount < 0 || this.CompletedQuestionCount > this.TotalQuestionCount)
                {
                    // Invalid data
                    status = ProcesssStepStatus.UnCompleted;
                }
                else if (TotalQuestionCount == CompletedQuestionCount)
                {
                    status = ProcesssStepStatus.Completed;
                }
                else if (this.CompletedQuestionCount == 0)
                {
                    status = ProcesssStepStatus.UnCompleted;
                }
                else
                {
                    status = ProcesssStepStatus.InProgress;
                }

                this.StepStatus = status;
            }

            public OrderProcessingDataSet.OrderProcessAnswerRow GetAnswer(OrderProcessingDataSet.ProcessQuestionRow question)
            {
                // There should be (at most) one answer per question, but
                // that might not be true.
                // If there is, prefer showing the duplicate with the smallest ID
                return question.GetOrderProcessAnswerRows()
                    .OrderBy(opa => opa.OrderProcesserAnswerID)
                    .FirstOrDefault(opa => opa.OrderProcessesID == this._opr.OrderProcessesID);
            }

            public override void Dispose()
            {
                if(QuestionControls != null)
                    QuestionControls.ForEach(q => q.Dispose());

                QuestionControls = null;
                this._opr = null;

                base.Dispose();
            }

            public void ValidationStepConditions()
            {
                try
                {
                    var previouslyExcluded = IsExcluded;
                    IsExcluded = false;

                    foreach (var stepConditionRow in this.DataRow.GetProcessStepConditionRows())
                    {
                        var evaluator = new StepConditionEvaluator()
                        {
                            OrderId = this._opr.OrderID,
                            OrderProcessAnswers = this._opr.GetOrderProcessAnswerRows()
                        };

                        var condition = new StepCondition()
                        {
                            ProcessQuestionId = stepConditionRow.IsProcessQuestionIdNull() ? 0 : stepConditionRow.ProcessQuestionId,
                            InputType = stepConditionRow.InputType.ConvertToEnum<ConditionInputType>(),
                            Operator = stepConditionRow.Operator.ConvertToEnum<EqualityOperator>(),
                            Value = stepConditionRow.Value
                        };

                        var meetsCondition = evaluator.Evaluate(condition);

                        //if was unable to determine the condition at this time then continue to next condition
                        if (meetsCondition == null)
                        {
                            continue;
                        }

                        //if meets the condition
                        if (!meetsCondition.Value)
                        {
                            IsExcluded = true;
                            break;
                        }
                    }

                    if (previouslyExcluded != IsExcluded)
                    {
                        var answers = _opr.GetOrderProcessAnswerRows()
                            .Where(opa => opa.ProcessQuestionRow.ProcessStepID == base.DataRow.ProcessStepID);

                        foreach (var opa2 in answers)
                        {
                            if (IsExcluded)
                            {
                                opa2.SetCompletedByNull();
                                opa2.SetCompletedDataNull();
                                opa2.SetAnswerNull();
                            }

                            opa2.Completed = IsExcluded;
                        }

                        this.Visible = !IsExcluded;
                        UpdateStepStatus();
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error validating step conditionals.");
                }
            }

            public override string ToString()
            {
                if (DataRow == null)
                {
                    return "ProcessStepNode - No Step";
                }
                else
                {
                    return string.Format("ProcessStepNode - {0} ({1})",
                        DataRow.Name,
                        DataRow.ProcessStepID);
                }
            }

            #endregion
        }

        #endregion
    }
}