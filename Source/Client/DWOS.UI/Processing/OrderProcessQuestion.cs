using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Data.Process;
using DWOS.UI.Processing;
using DWOS.Shared;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinMaskedEdit;
using Infragistics.Win.UltraWinToolTip;
using Infragistics.Win.Misc;
using NLog;

namespace DWOS.UI.Processing
{
    public partial class OrderProcessQuestion : UserControl
    {
        #region Events

        #endregion

        /// <summary>
        /// Format string to use for time answers.
        /// </summary>
        private const string TIME_FORMAT = "hh:mm:ss tt";

        /// <summary>
        /// Format string to use for date-time answers.
        /// </summary>
        private const string DATE_TIME_FORMAT = "MM/dd/yyyy hh:mm:ss tt";

        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private SmartBox _smartBox;
        private OrderProcessingDataSet _dsOrderProcessing;
        private InputType _inputType;
        private bool _isDirty;
        private bool _isDisposed;
        private bool _userEditedDate;
        private QuestionStatus _questionSatus = QuestionStatus.NoAnswer;
        public event EventHandler OnSave;

        private List<string> _samples;

        private enum QuestionStatus
        {
            NoAnswer,
            Completed,
            NotEditable,
            Error,
            InValidAnswer
        }

        #endregion

        #region Properties

        public OrderProcessingDataSet.ProcessQuestionRow QuestionRow { get; private set; }

        public OrderProcessingDataSet.OrderProcessAnswerRow AnswerRow { get; private set; }

        public OrderProcessingDataSet.OrderProcessesRow OrderProcessRow { get; set; }

        public OrderProcessingDataSet.OrderSummaryRow OrderRow { get; private set; }

        private QuestionStatus Status
        {
            get { return this._questionSatus; }
            set
            {
                this._questionSatus = value;

                if (QuestionRow == null || this._smartBox == null)
                    return;

                switch(this._questionSatus)
                {
                    case QuestionStatus.Completed:
                        this._smartBox.Enabled = QuestionRow == null ? true : QuestionRow.OperatorEditable;
                        this.dteDateCompleted.Enabled = true;
                        this.picQuestionStatus.Image = this.imgListQuestion.Images["Check"];
                        this.toolTipManager.GetUltraToolTip(this.picQuestionStatus).ToolTipText = "The question has been completed successfully.";
                        break;
                    case QuestionStatus.NotEditable:
                        this._smartBox.Enabled = false;
                        this.dteDateCompleted.Enabled = false;
                        this.picQuestionStatus.Image = this.imgListQuestion.Images["Lock"];
                        this.toolTipManager.GetUltraToolTip(this.picQuestionStatus).ToolTipText = "The question is not editable.";
                        break;
                    case QuestionStatus.Error:
                        this._smartBox.Enabled = false;
                        this.dteDateCompleted.Enabled = false;
                        this.picQuestionStatus.Image = this.imgListQuestion.Images["Error"];
                        this.toolTipManager.GetUltraToolTip(this.picQuestionStatus).ToolTipText = "The question has an error.";
                        break;
                    case QuestionStatus.NoAnswer:
                        this._smartBox.Enabled = QuestionRow == null ? true : QuestionRow.OperatorEditable;
                        this.dteDateCompleted.Enabled = true;
                        this.picQuestionStatus.Image = this.imgListQuestion.Images["Question"];
                        this.toolTipManager.GetUltraToolTip(this.picQuestionStatus).ToolTipText = "The question has not been answered yet.";
                        break;
                    case QuestionStatus.InValidAnswer:
                        this._smartBox.Enabled = QuestionRow == null ? true : QuestionRow.OperatorEditable;
                        this.dteDateCompleted.Enabled = true;
                        this.picQuestionStatus.Image = this.imgListQuestion.Images["Error"];
                        this.toolTipManager.GetUltraToolTip(this.picQuestionStatus).ToolTipText = "An invalid answer has been set.";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private OrderProcessing2.ProcessStepNode ProcessStepNode { get; set; }

        private bool IsPaperlessMode 
        {
            get { return OrderProcessRow != null && OrderProcessRow.ProcessRow.IsPaperless; }
        }

        private bool IsPaperMode
        {
            get { return !IsPaperlessMode; }
        }

        public bool IsAdminMode { get; set; }

        #endregion

        #region Methods

        public OrderProcessQuestion() { InitializeComponent(); }

        internal void CreateQuestion(OrderProcessingDataSet.ProcessQuestionRow question,
            OrderProcessing2.ProcessStepNode processStepNode,
            OrderProcessingDataSet.OrderProcessesRow orderProcessRow,
            OrderProcessingDataSet.OrderSummaryRow orderSummary)
        {
            //Need this inorder to proper set tooltips
            Control ctrAnswer = null;

            QuestionRow = question;
            OrderProcessRow = orderProcessRow;
            ProcessStepNode = processStepNode;
            OrderRow = orderSummary;

            this.lblQuestion.Text = question.Name;
            this.lblRequired.Visible = question.Required;
            this._inputType = question.InputType.ConvertToEnum <InputType>();

            string instructions;
            using (var taOrderProcessing = new PartProcessAnswerTableAdapter())
            {
                //Just need partID here
                instructions = (string)taOrderProcessing.GetInstructions(orderProcessRow.ProcessID, orderSummary.PartID,question.ProcessQuestionID);
            }

           

            if (!string.IsNullOrWhiteSpace(instructions))
                this.txtNotes.Value = instructions;
            else if (!question.IsNotesNull())
                this.txtNotes.Value = question.Notes;

            if(this.IsPaperlessMode)
            {
                _smartBox = new SmartBox();
                var defaultValue = ProcessUtilities.DefaultValue(question, orderSummary);

                ctrAnswer = _smartBox.CreateControl(this._inputType, (question.IsListIDNull() ? 0 : question.ListID), (question.IsNumericUntisNull() ? null : question.NumericUntis), defaultValue);
                _smartBox.KeyPress += AnswerKeyPress;
                _smartBox.SelectionChanged += OrderProcessQuestion_SelectionChanged;
                if(question.InputType == InputType.SampleSet.ToString())
                {
                    if (ctrAnswer is Infragistics.Win.Misc.UltraButton)
                    {
                        Infragistics.Win.Misc.UltraButton btn = ((Infragistics.Win.Misc.UltraButton)ctrAnswer);
                        btn.Click += delegate (object sender, EventArgs e) { SmartBox_ButtonClicked(sender, e, "", question); };
                        btn.Appearance.TextHAlign = HAlign.Right;
                        btn.Appearance.BackColor = this.BackColor;
                    }

                }

            }
            else
            {
                //if this is paper based then just put a text control
                ctrAnswer = _smartBox = new SmartBox();
                _smartBox.CreateControl(InputType.String, 0, null, "See Paper Process");
            }

            this.pnlAnswerPlaceHolder.Controls.Add(_smartBox);
            _smartBox.Dock = DockStyle.Fill;

            this.lblQuestionNumber.Text = question.StepOrder.ToString();
            this.lblAnswerDescription.Text = ControlUtilities.GetControlTooltipShort(this._inputType,
                ProcessUtilities.MinValue(question, orderSummary),
                ProcessUtilities.MaxValue(question, orderSummary),
                question.IsNumericUntisNull() ? null : question.NumericUntis);

            //Set tool tips
            string tooltipText = ControlUtilities.GetControlTooltip(this._inputType,
                ProcessUtilities.MinValue(question, orderSummary),
                ProcessUtilities.MaxValue(question, orderSummary),
                question.IsNumericUntisNull() ? null : question.NumericUntis);

            //This doesn't work, tooltip not shown
            //this.toolTipManager.SetUltraToolTip(_smartBox, new UltraToolTipInfo(tooltipText, ToolTipImage.Default, "Answer Value", DefaultableBoolean.True));

            this.toolTipManager.SetUltraToolTip(ctrAnswer, new UltraToolTipInfo(tooltipText, ToolTipImage.Default, "Answer Value", DefaultableBoolean.True));
            this.toolTipManager.SetUltraToolTip(this.lblAnswerDescription, new UltraToolTipInfo(tooltipText, ToolTipImage.Default, "Answer Value", DefaultableBoolean.True));

            Status = QuestionStatus.NoAnswer;

            //ensure answer can be completed by the operator
            if(!question.OperatorEditable)
            {
                Status = QuestionStatus.NotEditable;
                this.toolTipManager.SetUltraToolTip(this.picQuestionStatus, new UltraToolTipInfo("This answer is not editable by the operator.", ToolTipImage.Default, "Answer Locked", DefaultableBoolean.True));
            }

            //if paper based then dont allow edit
            if (this.IsPaperMode)
            {
                Status = QuestionStatus.NotEditable;
                toolTipManager.SetUltraToolTip(this.picQuestionStatus, new UltraToolTipInfo("This process is paper based.", ToolTipImage.Default, "Paperless", DefaultableBoolean.True));
            }
        }
        
        internal void LoadAnswer(OrderProcessingDataSet.OrderProcessAnswerRow answer, OrderProcessingDataSet dsOrderProcessing)
        {
            _dsOrderProcessing = dsOrderProcessing;
            AnswerRow = answer;

            if (this.IsPaperMode)
                return; //dont load an answer if paper based

            try
            {
                this._userEditedDate = false;
                this.dteDateCompleted.ValueChanged -= dteDateCompleted_ValueChanged;
                this.dteDateCompleted.DateTime = answer.IsCompletedDataNull() ? DateTime.Now : answer.CompletedData;
            }
            finally
            {
                this.dteDateCompleted.ValueChanged += dteDateCompleted_ValueChanged;
            }

            this.txtOperator.Text = answer.IsCompletedByNull() || answer.CompletedBy < 1 ? SecurityManager.Current.UserName : this._dsOrderProcessing.Users.FindByUserID(answer.CompletedBy).Name;
            
            //if has value then set it
            if(!answer.IsAnswerNull() && !String.IsNullOrWhiteSpace(answer.Answer))
                _smartBox.SetValue(answer.Answer);
            else //else try to find best answer to set it to
            {
                //if answer is null then set default answer
                if(QuestionRow.OperatorEditable)
                {
                    var inputType = QuestionRow.InputType.ConvertToEnum <InputType>();

                    //Set the default answer since one is not supplied
                    if(inputType == InputType.PartQty)
                    {
                        //if part qty is the question then get the remaining parts to be processed
                        var orderSummary = _dsOrderProcessing.OrderSummary.FindByOrderID(answer.OrderID);
                        var partCount = orderSummary.IsPartQuantityNull() ? 0M : orderSummary.PartQuantity;

                        if(!OrderProcessRow.IsPartCountNull())
                            partCount = partCount - OrderProcessRow.PartCount;
                        
                        _smartBox.SetValue(Math.Max(0, partCount).ToString());
                    }
                    else if (ControlUtilities.GetCategory(_inputType) == ControlUtilities.ControlCategory.DateTime)
                        _smartBox.SetDefaultValue();
                    else if (ControlUtilities.GetCategory(_inputType) == ControlUtilities.ControlCategory.List)
                        _smartBox.SetDefaultValue();
                }
            }

            if(Status == QuestionStatus.NoAnswer && answer.Completed)
                Status = QuestionStatus.Completed;
        }

        internal void SetFocus()
        {
            //if the answer can be edited then set focus to it, else set focus to the next button
            if (_smartBox.Enabled)
            {
                _smartBox.SetFocus(true);
            }
            else
                btnNext.Focus();

            //if this is a duration and this is not completed, then auto set answer NOTE: can set till gets focus and the TimeIn and TimeOut have been set
            if(this._inputType == InputType.TimeDuration && !AnswerRow.Completed)
                UpdateTimeDuration();

            if (_inputType == InputType.DecimalDifference && !AnswerRow.Completed)
            {
                UpdateDifference();
            }
        }

        internal void ValidateStatus()
        {
            try
            {
                if (AnswerRow == null || QuestionRow == null)
                    return;

                //if row says NOT answered BUT status says it is Completed 
                if (!AnswerRow.Completed && Status == QuestionStatus.Completed && AnswerRow.IsAnswerNull())
                {
                    _log.Info("Reseting answer in question " + QuestionRow.Name);

                    Status = QuestionStatus.NoAnswer;
                    LoadAnswer(AnswerRow, _dsOrderProcessing); //reload answer
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error validating question status.");
            }
        }

        /// <summary>
        ///     Saves the answer back to the answer row and updates question status.
        /// </summary>
        private void SaveAnswer()
        {
            try
            {
                _log.Info("Saving answer - " + QuestionRow.Name);

                object value = _smartBox.GetValue();
                bool answered = false;

                //if paperless then check the answer
                if (IsPaperlessMode && value != null && !String.IsNullOrWhiteSpace(value.ToString()))
                {
                    string answer = null;
                    switch(this._inputType)
                    {
                        case InputType.Date:
                            DateTime outDate;
                            if(DateTime.TryParse(value.ToString(), out outDate))
                                answer = outDate.ToShortDateString();
                            break;
                        case InputType.Time:
                        case InputType.TimeIn:
                        case InputType.TimeOut:
                            value = value.ToString().Remove("_");
                            DateTime outTime;
                            if(DateTime.TryParse(value.ToString(), out outTime))
                                answer = outTime.ToString(TIME_FORMAT);
                            break;
                        case InputType.DateTimeIn:
                        case InputType.DateTimeOut:
                            value = value.ToString().Remove("_");
                            if(DateTime.TryParse(value.ToString(), out var outDateTime))
                                answer = outDateTime.ToString(DATE_TIME_FORMAT);
                            break;
                        case InputType.SampleSet:
                        default:
                            answer = value.ToString().Remove("_").Trim();
                            break;
                    }

                    var isValid = ControlUtilities.ValidateAnswer(this._inputType,
                        ProcessUtilities.MinValue(QuestionRow, OrderRow),
                        ProcessUtilities.MaxValue(QuestionRow, OrderRow),
                        QuestionRow.IsListIDNull() ? 0 : QuestionRow.ListID,
                        answer);

                    if(isValid)
                    {
                        //if is using time duration then do a special validate.
                        if (this._inputType == InputType.TimeDuration || this._inputType == InputType.TimeIn || this._inputType == InputType.TimeOut)
                        {
                            string newAnswer = answer;
                            isValid = ValidateTimeDuration(ref newAnswer);

                            //if validator change the answer then update it.
                            if (answer != newAnswer)
                            {
                                _smartBox.SetValue(newAnswer);
                                answer = newAnswer;
                            }
                        }

                        if (_inputType == InputType.DecimalDifference || _inputType == InputType.DecimalBefore ||
                            _inputType == InputType.DecimalAfter)
                        {
                            string newAnswer = answer;
                            isValid = ValidateDifference(ref newAnswer);

                            // If validation changes the answer, then update it.
                            if (answer != newAnswer)
                            {
                                _smartBox.SetValue(newAnswer);
                                answer = newAnswer;
                            }
                        }

                        //ensure date is after time added to department
                        if(isValid && this._inputType == InputType.Date && OrderProcessRow != null && !OrderProcessRow.IsStartDateNull())
                        {
                            DateTime outDate;
                            if(DateTime.TryParse(value.ToString(), out outDate) && outDate < OrderProcessRow.StartDate)
                                MessageBoxUtilities.ShowMessageBoxWarn("Date selected occurs before the date the order was checked into the department.", "Date Warning", "Adjust date to after {0}.".FormatWith(OrderProcessRow.StartDate.ToString()));
                        }

                        if (isValid)
                        {
                            answered = true;
                            AnswerRow.Answer = answer;
                            ProcessUtilities.EnsureFieldsExist(AnswerRow, OrderRow, _dsOrderProcessing);
                            ProcessUtilities.SetFieldValue(AnswerRow, OrderRow);
                            if ((_inputType == InputType.SampleSet)&& _samples.Count > 0)
                            {
                                using (var taProcessAnswerSample = new ProcessAnswerSampleTableAdapter())
                                {
                                    taProcessAnswerSample.DeleteByAnswer(AnswerRow.OrderProcesserAnswerID);
                                    var dtSamples = taProcessAnswerSample.GetData();
                                    BuildSampleTable(ref dtSamples);
                                    taProcessAnswerSample.Update(dtSamples);

                                    taProcessAnswerSample.Dispose();
                                }
                            }


                        }
                        else
                        {
                            Status = QuestionStatus.InValidAnswer;
                            AnswerRow.SetAnswerNull();
                            ProcessUtilities.ClearFieldValue(AnswerRow, OrderRow);
                        }
                    }
                    else
                    {
                        Status = QuestionStatus.InValidAnswer;
                    }

                    if (!QuestionRow.Required && !isValid && ControlUtilities.IsAnswerBlank(_inputType, answer))
                    {
                        // Non-required answers can be left empty.
                        answered = true;
                        ProcessUtilities.ClearFieldValue(AnswerRow, OrderRow);
                    }
                }
                else if (!QuestionRow.Required && (value == null || string.IsNullOrEmpty(value.ToString())))
                {
                    answered = true;
                    ProcessUtilities.ClearFieldValue(AnswerRow, OrderRow);
                }

                // set completed if answered
                if(IsPaperlessMode && answered)
                {
                    //Dont SAVE who and when if in Admin mode
                    if (!IsAdminMode)
                    {
                        //Set who completed it
                        AnswerRow.CompletedBy = SecurityManager.Current.UserID;

                        // Update CompletedData
                        if (_userEditedDate)
                        {
                            AnswerRow.CompletedData = dteDateCompleted.DateTime;
                        }
                        else if (AnswerRow.IsCompletedDataNull())
                        {
                            AnswerRow.CompletedData = DateTime.Now;
                        }

                        //update UI
                        try
                        {
                            this._userEditedDate = false;
                            this.dteDateCompleted.ValueChanged -= dteDateCompleted_ValueChanged;
                            this.dteDateCompleted.DateTime = AnswerRow.CompletedData;
                        }
                        finally
                        {
                            this.dteDateCompleted.ValueChanged += dteDateCompleted_ValueChanged;
                        }

                        this.txtOperator.Text = SecurityManager.Current.UserName;
                    }

                    

                    //mark as completed
                    AnswerRow.Completed = true;
                    Status = QuestionStatus.Completed;

                    if(OnSave != null)
                        OnSave(this, EventArgs.Empty);
                }
                else
                {
                    if(AnswerRow.Completed) //set only if changed
                        AnswerRow.Completed = false;
                    
                    SetFocus(); //else go back to the input box

                    if (this.toolTipManager.GetUltraToolTip(_smartBox) != null)
                        this.toolTipManager.ShowToolTip(_smartBox, PointToScreen(this.lblRequired.Location));
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error saving the answer to the question.");
            }
        }


        private void BuildSampleTable(ref OrderProcessingDataSet.ProcessAnswerSampleDataTable table)
        {
            try
            {
                double sampleValue = 0;
                for (int i = 0; i < _samples.Count; i++)
                {
                    sampleValue = Convert.ToDouble(_samples[i]);
                    table.Rows.Add(sampleValue, AnswerRow.OrderProcesserAnswerID);
                }

            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding samplevalues to table.");
            }


        }
        private void SaveAnswerSamples()
        {



        }

        private void DisposeMe()
        {
            _log.Debug("Disposing of Order Process Question " + QuestionRow.Name);

            this._dsOrderProcessing = null;
            AnswerRow = null;
            QuestionRow = null;
            ProcessStepNode = null;

            if (this._smartBox != null)
                this._smartBox.Dispose();

            this._smartBox = null;

            this._isDisposed = true;
        }

        #region Time Duration

        private bool ValidateTimeDuration(ref string answer)
        {
            try
            {
                //find the time durations types
                var timeIn = ProcessStepNode.QuestionControls.FirstOrDefault(ques => ques._inputType == InputType.TimeIn || ques._inputType == InputType.DateTimeIn);
                var timeOut = ProcessStepNode.QuestionControls.FirstOrDefault(ques => ques._inputType == InputType.TimeOut || ques._inputType == InputType.DateTimeOut);
                var timeDuration = ProcessStepNode.QuestionControls.FirstOrDefault(ques => ques._inputType == InputType.TimeDuration);

                //if has all time types required
                if (timeIn != null && timeOut != null && timeDuration != null)
                {
                    //base on what this question is
                    switch (this._inputType)
                    {
                        case InputType.TimeIn:
                        case InputType.DateTimeIn:
                            if (timeDuration.AnswerRow.Completed && !timeDuration.AnswerRow.IsAnswerNull() && timeOut.AnswerRow.Completed && !timeOut.AnswerRow.IsAnswerNull())
                            {
                                var timeOutValue = Convert.ToDateTime(timeOut.AnswerRow.Answer);
                                var timeInValue = Convert.ToDateTime(answer);
                                int? calculatedDuration = CalculateTimeDuration(timeInValue, timeOutValue);

                                if (calculatedDuration.HasValue)
                                {
                                    int durationAnswer;
                                    int.TryParse(timeDuration.AnswerRow.Answer, out durationAnswer);

                                    //if the calculated and the user defined are not them same then they cant do math
                                    if (calculatedDuration.Value != durationAnswer)
                                    {
                                        MessageBoxUtilities.ShowMessageBoxWarn("The calculated duration of '" + calculatedDuration.Value + "' does not match your duration answer in question '" + timeDuration.QuestionRow.StepOrder.ToString() + "' of '" + durationAnswer + "'", "Duration");
                                        return false;
                                    }

                                    return true;
                                }
                            }
                            break;
                        case InputType.TimeOut:
                        case InputType.DateTimeOut:
                            if (timeDuration.AnswerRow.Completed && !timeDuration.AnswerRow.IsAnswerNull() && timeIn.AnswerRow.Completed && !timeIn.AnswerRow.IsAnswerNull())
                            {
                                var timeOutValue = Convert.ToDateTime(answer);
                                var timeInValue = Convert.ToDateTime(timeIn.AnswerRow.Answer);
                                int? calculatedDuration = CalculateTimeDuration(timeInValue, timeOutValue);

                                if (calculatedDuration.HasValue)
                                {
                                    int durationAnswer;
                                    int.TryParse(timeDuration.AnswerRow.Answer, out durationAnswer);

                                    //if the calculated and the user defined are not them same then they cant do math
                                    if (calculatedDuration.Value != durationAnswer)
                                    {
                                        var calculatedTimeOut = timeInValue.AddMinutes(durationAnswer);
                                        if (MessageBoxUtilities.ShowMessageBoxYesOrNo("The calculated duration of '" + calculatedDuration.Value + "' does not match your duration answer in question '" + timeDuration.QuestionRow.StepOrder.ToString() + "' of '" + durationAnswer + "'. Do you want to set the time out to '" + calculatedTimeOut.ToShortTimeString() + "' to match the specified duration?", "Duration") == DialogResult.Yes)
                                        {
                                            answer = calculatedTimeOut.ToString(TIME_FORMAT);
                                            return true;
                                        }

                                        return false;
                                    }

                                    return true;
                                }
                            }
                            break;
                        case InputType.TimeDuration:
                            if (timeOut.AnswerRow.Completed && !timeOut.AnswerRow.IsAnswerNull() && timeIn.AnswerRow.Completed && !timeIn.AnswerRow.IsAnswerNull())
                            {
                                int durationAnswer;
                                int.TryParse(answer, out durationAnswer);
                                var units = timeDuration.QuestionRow.IsNumericUntisNull() ? "Minutes" : timeDuration.QuestionRow.NumericUntis;

                                //ensure the duration is within range
                                if (!ControlUtilities.ValidateWithinRange(InputType.Integer, ProcessUtilities.MinValue(timeDuration.QuestionRow, timeDuration.OrderRow), ProcessUtilities.MaxValue(timeDuration.QuestionRow, timeDuration.OrderRow), durationAnswer))
                                {
                                    MessageBoxUtilities.ShowMessageBoxWarn("The duration of '" + durationAnswer + "' " + units + " is not within the specified range. Please adjust either the time in or time out.", "Duration");
                                    return false;
                                }

                                var timeOutValue = Convert.ToDateTime(timeOut.AnswerRow.Answer);
                                var timeInValue = Convert.ToDateTime(timeIn.AnswerRow.Answer);
                                int? calculatedDuration = CalculateTimeDuration(timeInValue, timeOutValue);

                                if (calculatedDuration.HasValue && !String.IsNullOrEmpty(answer))
                                {
                                    //if the calculated and the user defined are not them same then they cant do math
                                    if (calculatedDuration.Value != durationAnswer)
                                    {
                                        //ensure the calculated duration is within the specified range
                                        if (!ControlUtilities.ValidateWithinRange(InputType.Integer, ProcessUtilities.MinValue(timeDuration.QuestionRow, timeDuration.OrderRow), ProcessUtilities.MaxValue(timeDuration.QuestionRow, timeDuration.OrderRow), calculatedDuration.Value))
                                        {
                                            MessageBoxUtilities.ShowMessageBoxWarn("The calculated duration of '" + calculatedDuration.Value + "' " + units + " is not the same as your answer and is not within the specified range. Please adjust either the time in or time out.", "Duration");
                                            return false;
                                        }

                                        if (MessageBoxUtilities.ShowMessageBoxYesOrNo("The correct duration between Time In '" + timeInValue.ToShortTimeString() + "' and Time Out '" + timeOutValue.ToShortTimeString() + "'  should be " + calculatedDuration.Value + ". Do you want to use that value instead?", "Duration") == DialogResult.Yes)
                                        {
                                            answer = calculatedDuration.Value.ToString();
                                            return true;
                                        }

                                        return false;
                                    }

                                    return true;
                                }
                            }
                            break;
                    }
                }

                return true;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error validating time duration answers");
                return true;
            }
        }

        private bool ValidateDifference(ref string answer)
        {
            const string warningMsgHeader = "Difference";

            // Assumption: a previous check ensures that answer is not null/empty
            try
            {
                var questionBefore =
                    ProcessStepNode.QuestionControls.FirstOrDefault(ques => ques._inputType == InputType.DecimalBefore);

                var questionAfter =
                    ProcessStepNode.QuestionControls.FirstOrDefault(ques => ques._inputType == InputType.DecimalAfter);

                var questionDifference =
                    ProcessStepNode.QuestionControls.FirstOrDefault(ques => ques._inputType == InputType.DecimalDifference);

                if (questionBefore == null || questionAfter == null || questionDifference == null)
                {
                    return true;
                }

                var beforeHasAnswer = questionBefore.AnswerRow.Completed && !questionBefore.AnswerRow.IsAnswerNull();
                var afterHasAnswer = questionAfter.AnswerRow.Completed && !questionAfter.AnswerRow.IsAnswerNull();
                var differenceHasAnswer = questionDifference.AnswerRow.Completed && !questionDifference.AnswerRow.IsAnswerNull();

                var isValid = true;

                if (_inputType == InputType.DecimalBefore && afterHasAnswer && differenceHasAnswer)
                {
                    var beforeAnswer = decimal.Parse(answer);
                    var afterAnswer = decimal.Parse(questionAfter.AnswerRow.Answer);
                    var differenceAnswer = decimal.Parse(questionDifference.AnswerRow.Answer);

                    var calculatedDifference = CalculateDifference(beforeAnswer, afterAnswer);
                    if (calculatedDifference != differenceAnswer)
                    {
                        var warningMsg =
                            $"The calculated duration of '{calculatedDifference}' does not match your duration answer in question '{questionDifference.QuestionRow.StepOrder}' of '{differenceAnswer}'";

                        MessageBoxUtilities.ShowMessageBoxWarn(warningMsg, warningMsgHeader);
                        isValid = false;
                    }
                }
                else if (_inputType == InputType.DecimalAfter && beforeHasAnswer && differenceHasAnswer)
                {
                    var beforeAnswer = decimal.Parse(questionBefore.AnswerRow.Answer);
                    var afterAnswer = decimal.Parse(answer);
                    var differenceAnswer = decimal.Parse(questionDifference.AnswerRow.Answer);

                    var calculatedDifference = CalculateDifference(beforeAnswer, afterAnswer);
                    if (calculatedDifference != differenceAnswer)
                    {
                        var warningMsg =
                            $"The calculated duration of '{calculatedDifference}' does not match your duration answer in question '{questionDifference.QuestionRow.StepOrder}' of '{differenceAnswer}'";

                        MessageBoxUtilities.ShowMessageBoxWarn(warningMsg, warningMsgHeader);
                        isValid = false;
                    }
                }
                else if (_inputType == InputType.DecimalDifference && beforeHasAnswer && afterHasAnswer)
                {
                    var differenceAnswer = decimal.Parse(answer);
                    var isDifferenceValid = ControlUtilities.ValidateWithinRange(InputType.Decimal,
                        ProcessUtilities.MinValue(questionDifference.QuestionRow, questionDifference.OrderRow),
                        ProcessUtilities.MaxValue(questionDifference.QuestionRow, questionDifference.OrderRow),
                        differenceAnswer);

                    if (!isDifferenceValid)
                    {
                        var warningMsg =
                            $"The difference of '{differenceAnswer}' is not within the specified range. Please adjust either the before or after values.";
                        MessageBoxUtilities.ShowMessageBoxWarn(warningMsg, warningMsgHeader);
                        isValid = false;
                    }
                    else
                    {
                        var beforeAnswer = decimal.Parse(questionBefore.AnswerRow.Answer);
                        var afterAnswer = decimal.Parse(questionAfter.AnswerRow.Answer);
                        var calculatedDifference = CalculateDifference(beforeAnswer, afterAnswer);

                        if (differenceAnswer != calculatedDifference)
                        {
                            var warningMsg =
                                $"The calculated duration of '{calculatedDifference}' does not match your duration answer in question '{questionDifference.QuestionRow.StepOrder}' of '{differenceAnswer}'";

                            MessageBoxUtilities.ShowMessageBoxWarn(warningMsg, warningMsgHeader);
                            isValid = false;
                        }
                    }
                }

                return isValid;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error validating time duration answers");
                return true;
            }
        }

        /// <summary>
        ///     Updates the answer if it is a time duration.
        /// </summary>
        private void UpdateTimeDuration()
        {
            try
            {
                var timeIn = ProcessStepNode.QuestionControls.FirstOrDefault(ques => ques._inputType == InputType.TimeIn || ques._inputType == InputType.DateTimeIn);
                var timeOut = ProcessStepNode.QuestionControls.FirstOrDefault(ques => ques._inputType == InputType.TimeOut || ques._inputType == InputType.DateTimeOut);

                if (timeIn != null && timeOut != null && timeIn.AnswerRow.Completed && !timeIn.AnswerRow.IsAnswerNull() && timeOut.AnswerRow.Completed && !timeOut.AnswerRow.IsAnswerNull())
                {
                    _log.Info("Validating time duration, time in '{0}' time out '{1}'.", timeIn.AnswerRow.Answer, timeOut.AnswerRow.Answer);

                    var timeOutValue = Convert.ToDateTime(timeOut.AnswerRow.Answer);
                    var timeInValue = Convert.ToDateTime(timeIn.AnswerRow.Answer);
                    int? dur = CalculateTimeDuration(timeInValue, timeOutValue);
                    
                    if(dur.HasValue && ControlUtilities.ValidateWithinRange(InputType.Integer, ProcessUtilities.MinValue(QuestionRow, OrderRow), ProcessUtilities.MaxValue(QuestionRow, OrderRow), dur.Value))
                        _smartBox.SetValue(dur.Value.ToString());
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error attempting to auto set the time duration for order process answer: " + AnswerRow.OrderProcesserAnswerID;
                _log.Error(exc, errorMsg);
            }
        }

        private void UpdateDifference()
        {
            try
            {
                var questionBefore =
                    ProcessStepNode.QuestionControls.FirstOrDefault(ques => ques._inputType == InputType.DecimalBefore);

                var questionAfter =
                    ProcessStepNode.QuestionControls.FirstOrDefault(ques => ques._inputType == InputType.DecimalAfter);

                var calculateDifference = questionBefore != null && questionAfter != null &&
                    questionBefore.AnswerRow.Completed && !questionBefore.AnswerRow.IsAnswerNull() &&
                    questionAfter.AnswerRow.Completed && !questionAfter.AnswerRow.IsAnswerNull();

                if (!calculateDifference)
                {
                    return;
                }

                var beforeAnswer = decimal.Parse(questionBefore.AnswerRow.Answer);
                var afterAnswer = decimal.Parse(questionAfter.AnswerRow.Answer);
                var difference = CalculateDifference(beforeAnswer, afterAnswer);

                if (ControlUtilities.ValidateWithinRange(InputType.Decimal, ProcessUtilities.MinValue(QuestionRow, OrderRow), ProcessUtilities.MaxValue(QuestionRow, OrderRow),
                    difference))
                {
                    _smartBox.SetValue(difference.ToString(CultureInfo.CurrentCulture));
                }
            }
            catch (Exception exc)
            {
                string errorMsg = $"Error attempting to auto set the difference for order process answer: {AnswerRow.OrderProcesserAnswerID}";
                _log.Error(exc, errorMsg);
            }
        }

        private int? CalculateTimeDuration(DateTime timeIn, DateTime timeOut)
        {
            timeIn = timeIn.AddSeconds(-timeIn.Second);
            timeOut = timeOut.AddSeconds(-timeOut.Second);

            int? duration = null;
            
            if(!QuestionRow.IsNumericUntisNull() && QuestionRow.NumericUntis.ToLower().Contains("second"))
                duration = Convert.ToInt32(timeIn.Subtract(timeOut).Duration().TotalSeconds);
            else if(!this.QuestionRow.IsNumericUntisNull() && this.QuestionRow.NumericUntis.ToLower().Contains("hour"))
                duration = Convert.ToInt32(timeIn.Subtract(timeOut).Duration().TotalHours);
            else
                duration = Convert.ToInt32(timeIn.Subtract(timeOut).Duration().TotalMinutes);

            var rampTime = ProcessStepNode.QuestionControls.Find(ques => ques._inputType == InputType.RampTime);
            
            // Check ramp time and apply if needed
            if (rampTime != null && rampTime.AnswerRow.Completed && !rampTime.AnswerRow.IsAnswerNull())
            {
                duration = duration - Convert.ToInt32(rampTime.AnswerRow.Answer);
            }

            return duration;
        }

        private decimal CalculateDifference(decimal before, decimal after)
        {
            return Math.Abs(before - after);
        }

        #endregion

        #endregion

        #region Events

        private void OrderProcessQuestionKeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char) Keys.Return)
            {
                e.Handled = true;
                this.btnNext.PerformClick();
            }
        }

        private void AnswerKeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char) Keys.Return)
            {
                e.Handled = true;
                this.btnNext.PerformClick();
            }
            else
                this._isDirty = true;
        }

        private void OrderProcessQuestion_SelectionChanged(object sender, EventArgs e)
        {
            this._isDirty = true;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            this._isDirty = false;
            SaveAnswer();
        }

        private void SmartBox_ButtonClicked(object sender, EventArgs e, string message, OrderProcessingDataSet.ProcessQuestionRow question)
        {

            UltraButton btn = (UltraButton)sender;
            SampleSetEntry sampleForm = new SampleSetEntry();

            try
            {

                using (var taProcessAnswerSample = new ProcessAnswerSampleTableAdapter())
                {
                    var dtSamples = taProcessAnswerSample.GetByProcessAnswerID(AnswerRow.OrderProcesserAnswerID);
                    _samples = dtSamples.AsEnumerable().Select(x => x["SampleValue"].ToString()).ToList();
                    taProcessAnswerSample.Dispose();
                }

                if (!(question is null))
                {
                    //sampleForm.ParentID = this.AnswerRow.OrderProcesserAnswerID;
                    sampleForm.DefaultValue = (this.AnswerRow.Answer == "") ? 0 : Convert.ToDouble(this.AnswerRow.Answer);
                    sampleForm.SampleSize = 10;
                    sampleForm.AvgMin = Convert.ToDouble(question.MinValue);
                    sampleForm.AvgMax = Convert.ToDouble(question.MaxValue);
                    sampleForm.SampleValues = _samples;
                    sampleForm.LoadSamples();
                    if (sampleForm.ShowDialog() == DialogResult.OK)
                    {
                        btn.Text = sampleForm.tbxAverageValue.Text;
                        _samples = sampleForm.SampleValues;
                        sampleForm.Dispose();

                    }

                }

            }
            catch (Exception)
            {

                throw;
            }
           
        }
        private void OrderProcessQuestion_Leave(object sender, EventArgs e)
        {
            try
            {
                if (this._isDisposed || this.IsPaperMode)
                    return;

                //reset the style back to what it is suppose to be
                Status = Status;

                //did user not save the changes
                if(_isDirty)
                {
                    //ask to save answer
                    var results = MessageBoxUtilities.ShowMessageBoxYesOrNo("Answer is not saved. Do you want to save your answer now?", "Unsaved Answer", "Tip: By hitting the Return key or clicking next the answer will automatically be saved.");
                    _isDirty    = false; //UN-mark as dirty 

                    if(results == DialogResult.Yes)
                        btnNext.PerformClick();
                    else
                    {
                        //reset answer back to original
                        if(AnswerRow != null && AnswerRow.Completed && !AnswerRow.IsAnswerNull())
                            _smartBox.SetValue(AnswerRow.Answer);
                        else if(QuestionRow != null && OrderRow != null)
                        {
                            //reset back to default value
                            var defaultValue = ProcessUtilities.DefaultValue(QuestionRow, OrderRow);

                            if (!string.IsNullOrEmpty(defaultValue))
                            {
                                _smartBox.SetValue(defaultValue);
                            }
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error on order processing question leave.");
            }
        }

        private void OrderProcessQuestion_Enter(object sender, EventArgs e)
        {
            //if no answer and this is active then change icon to an arrow 
            if(Status == QuestionStatus.NoAnswer)
                this.picQuestionStatus.Image = this.imgListQuestion.Images["Active"];
        }

        private void dteDateCompleted_ValueChanged(object sender, EventArgs e)
        {
            _userEditedDate = true;
        }

        #endregion
    }
}