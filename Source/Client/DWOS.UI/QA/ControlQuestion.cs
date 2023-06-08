using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinMaskedEdit;
using Infragistics.Win.UltraWinToolTip;
using NLog;

namespace DWOS.UI.QA
{
    public partial class ControlQuestion : UserControl
    {
        #region Fields

        private Control _answerInput;
        private QuestionStatus _questionSatus = QuestionStatus.Normal;
        private bool _saved;
        private bool _skipped;

        private int _partQuantuty;

        public event EventHandler OnSave;

        private List<string> _samples;

        private int _answerID = -1;

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private enum QuestionStatus
        {
            Completed,
            Error,
            Normal,
            InValidAnswer
        }

        #endregion

        #region Properties
        public int AnswerID
        {
            set { _answerID = value; }
        }

        public List<string> Samples
        {
            get { return this._samples; }
        }
        private QuestionStatus Status
        {
            get { return this._questionSatus; }
            set
            {
                this._questionSatus = value;

                switch(this._questionSatus)
                {
                    case QuestionStatus.Completed:
                        this._answerInput.Enabled = true;
                        this.dteDateCompleted.Enabled = true;
                        this.picQuestionStatus.Image = this.imgListQuestion.Images["Check"];
                        this.toolTipManager1.GetUltraToolTip(this.picQuestionStatus).ToolTipText = "The question has been completed successfully.";
                        break;
                    case QuestionStatus.Error:
                        this._answerInput.Enabled = false;
                        this.dteDateCompleted.Enabled = false;
                        this.picQuestionStatus.Image = this.imgListQuestion.Images["Error"];
                        this.toolTipManager1.GetUltraToolTip(this.picQuestionStatus).ToolTipText = "The question has an error.";
                        break;
                    case QuestionStatus.Normal:
                        this._answerInput.Enabled = true;
                        this.dteDateCompleted.Enabled = true;
                        this.picQuestionStatus.Image = this.imgListQuestion.Images["Question"];
                        this.toolTipManager1.GetUltraToolTip(this.picQuestionStatus).ToolTipText = "The question has not been answered yet.";
                        break;
                    case QuestionStatus.InValidAnswer:
                        this._answerInput.Enabled = true;
                        this.dteDateCompleted.Enabled = true;
                        this.picQuestionStatus.Image = this.imgListQuestion.Images["Error"];
                        this.toolTipManager1.GetUltraToolTip(this.picQuestionStatus).ToolTipText = "An invalid answer has been set.";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal bool IsComplete =>
            _saved || _skipped;

        public InputType InputType { get; private set; }
        public int PartQuantity
        {
            set { _partQuantuty = value; }
        }

        public bool Skipped
        {
            get => _skipped;
            set
            {
                if (_skipped != value)
                {
                    _skipped = value;
                    Visible = !value;
                }
            }
        }

        public PartInspectionDataSet.PartInspectionQuestionRow QuestionRow { get; private set; }

        #endregion

        #region Methods

        public ControlQuestion() { InitializeComponent(); }

        internal void CreateQuestion(PartInspectionDataSet.PartInspectionQuestionRow question)
        {
            QuestionRow = question;
            this.lblStepName.Text = question.Name;
            this.txtOperator.Text = SecurityManager.Current.UserName;

            this.InputType = question.InputType.ConvertToEnum <InputType>();

            this._answerInput = ControlUtilities.CreateControlInspection(
                Data.ApplicationSettings.Current.ProcessStrictnessLevel,
                this.InputType,
                question.IsListIDNull() ? 0 : question.ListID,
                question.IsNumericUntisNull() ? null : question.NumericUntis,
                question.IsDefaultValueNull() ? null : question.DefaultValue);

            this._answerInput.KeyPress += ControlKeyPress;
            this._answerInput.TextChanged += ControlTextChanged;

            if (this.InputType == InputType.SampleSet)
            {
                if (_answerInput is Infragistics.Win.Misc.UltraButton)
                {
                    Infragistics.Win.Misc.UltraButton btn = ((Infragistics.Win.Misc.UltraButton)_answerInput);
                    btn.Click += delegate (object sender, EventArgs e) { AnswerInput_ButtonClicked(sender, e, "", question); };
                    btn.Appearance.TextHAlign = HAlign.Right;
                    btn.Appearance.BackColor = this.BackColor;
                }

            }



            //This fixes the issue with the numeric control clipping the parent panel
            if (this._answerInput is UltraNumericEditor)
            {
                this._answerInput.AutoSize = false;
                this._answerInput.Height = 15;
            }

            this.pnlAnswerPlaceHolder.Controls.Add(this._answerInput);
            this._answerInput.Dock = DockStyle.Fill;

            this.lblStepNumber.Text = question.StepOrder.ToString();
            this.lblAnswerDescription.Text = ControlUtilities.GetControlTooltipShort(this.InputType, (question.IsMinValueNull() ? null : question.MinValue), (question.IsMaxValueNull() ? null : question.MaxValue), (question.IsNumericUntisNull() ? null : question.NumericUntis));

            string tooltipText = ControlUtilities.GetControlTooltip(this.InputType, (question.IsMinValueNull() ? null : question.MinValue), (question.IsMaxValueNull() ? null : question.MaxValue), (question.IsNumericUntisNull() ? null : question.NumericUntis));
            this.toolTipManager1.SetUltraToolTip(this._answerInput, new UltraToolTipInfo(tooltipText, ToolTipImage.Default, "Answer Value", DefaultableBoolean.True));
            this.toolTipManager1.SetUltraToolTip(this.lblAnswerDescription, new UltraToolTipInfo(tooltipText, ToolTipImage.Default, "Answer Value", DefaultableBoolean.True));

            Status = QuestionStatus.Normal;
        }

        internal void SetFocus()
        {
            if(this._answerInput is UltraCurrencyEditor)
                ((UltraCurrencyEditor) this._answerInput).SelectAll();
            else if(this._answerInput is UltraTextEditor)
                ((UltraTextEditor) this._answerInput).SelectAll();
            else if(this._answerInput is UltraNumericEditor)
                ((UltraNumericEditor) this._answerInput).SelectAll();
            else
                this._answerInput.Select();

            this._answerInput.Focus();
        }

        private void SaveAnswer()
        {
            object value = GetAnswer();
            bool answered = false;
            Status = QuestionStatus.Normal;

            if(value != null && !String.IsNullOrEmpty(value.ToString()))
            {
                string answer = null;
                switch(this.InputType)
                {
                    case InputType.Date:
                        DateTime date;
                        if(DateTime.TryParse(value.ToString(), out date))
                            answer = date.ToShortDateString();
                        break;
                    case InputType.Time:
                    case InputType.TimeIn:
                    case InputType.TimeOut:
                        value = value.ToString().Replace("_", "");
                        if(DateTime.TryParse(value.ToString(), out var outTime))
                            answer = outTime.ToString("hh:mm:ss tt");
                        break;
                    case InputType.DateTimeIn:
                    case InputType.DateTimeOut:
                        value = value.ToString().Replace("_", "");
                        if (DateTime.TryParse(value.ToString(), out var outDateTime))
                        {
                            answer = outDateTime.ToString("MM/dd/yyyy hh:mm:ss tt");
                        }
                        break;
                    case InputType.SampleSet:
                        answer = value.ToString().Replace("_", "");
                        break;
                    default:
                        answer = value.ToString().Replace("_", "");
                        break;
                }

                bool isValid = ControlUtilities.ValidateAnswer(this.InputType, (QuestionRow.IsMinValueNull() ? null : QuestionRow.MinValue), (QuestionRow.IsMaxValueNull() ? null : QuestionRow.MaxValue), QuestionRow.IsListIDNull() ? 0 : QuestionRow.ListID, answer);

                if(isValid)
                    answered = true;
                else
                    Status = QuestionStatus.InValidAnswer;
            }

            if(answered)
            {
                this.txtOperator.Text = SecurityManager.Current.UserName; //update UI
                this.dteDateCompleted.DateTime = DateTime.Now;
                Status = QuestionStatus.Completed;
            }
            else
            {
                SetFocus(); //else go back to the input box

                if(this.toolTipManager1.GetUltraToolTip(this._answerInput) != null)
                    this.toolTipManager1.ShowToolTip(this._answerInput, PointToScreen(this.lblAnswerDescription.Location));
            }

            this._saved = answered;

            if(answered && OnSave != null)
                OnSave(this, EventArgs.Empty);
        }

        private object GetAnswer()
        {
            if(this._answerInput == null)
                return null;

            if(this._answerInput is UltraCheckEditor)
                return ((UltraCheckEditor) this._answerInput).Checked;

            if(this._answerInput is UltraCurrencyEditor)
                return ((UltraCurrencyEditor) this._answerInput).ValueObject;

            if(this._answerInput is UltraTextEditor)
                return ((UltraTextEditor) this._answerInput).Value;

            if(this._answerInput is UltraNumericEditor)
            {
                ((UltraNumericEditor) this._answerInput).MaskDisplayMode = MaskMode.IncludeLiterals;
                return ((UltraNumericEditor) this._answerInput).Value;
            }
            if(this._answerInput is UltraComboEditor)
                return ((UltraComboEditor) this._answerInput).Value;

            return this._answerInput.Text;
        }

        internal string GetCompletedAnswer()
        {
            if (_answerInput is UltraNumericEditor numericEditor)
            {
                // Cannot use Text property - may have padding chars in it if
                // the control still has focus
                return numericEditor.Value?.ToString();
            }

            return _answerInput.Text;
        }

        private void BuildSampleTable(ref OrderProcessingDataSet.ProcessAnswerSampleDataTable table)
        {
            try
            {
                double sampleValue = 0;
                for (int i = 0; i < _samples.Count; i++)
                {
                    sampleValue = Convert.ToDouble(_samples[i]);
                    //table.Rows.Add(sampleValue, QuestionRow.GetParentRow("FK_PartInspectionAnswer_Samples"));
                    //table.Rows.Add(sampleValue, QuestionRow.GetParentRow("FK_PartInspectionAnswer_Samples"));
                }

            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding samplevalues to table.");
            }


        }

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

        private void ControlKeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char) Keys.Return)
            {
                e.Handled = true;
                this.btnNext.PerformClick();
            }
        }

        private void btnNext_Click(object sender, EventArgs e) { SaveAnswer(); }

        private void OrderProcessQuestion_Leave(object sender, EventArgs e)
        {
            //Auto save the answer
            if(!this._saved)
                SaveAnswer();
        }

        private void ControlTextChanged(object sender, EventArgs e)
        {
            //when value of the control changes then set saved to false to ensure it will try and save again
            this._saved = false;
        }

        private void AnswerInput_ButtonClicked(object sender, EventArgs e, string message, PartInspectionDataSet.PartInspectionQuestionRow question)
        {

            Infragistics.Win.Misc.UltraButton btn = (Infragistics.Win.Misc.UltraButton)sender;
            DWOS.UI.Processing.SampleSetEntry sampleForm = new DWOS.UI.Processing.SampleSetEntry();


            using (var taSamples = new Data.Datasets.PartInspectionDataSetTableAdapters.InspectionAnswerSampleTableAdapter())
            {
                if(_answerID != -1)
                {
                    var dtSamples = taSamples.GetByPartInspectionAnswer(_answerID);
                    _samples = dtSamples.AsEnumerable().Select(x => x["SampleValue"].ToString()).ToList();
                }
                taSamples.Dispose();
            }

            if (!(question is null))
            {
                sampleForm.DefaultValue = (question.DefaultValue == "") ? 0 : Convert.ToDouble(question.DefaultValue);
                sampleForm.DefaultValue = Convert.ToDouble(question.DefaultValue);
                sampleForm.SampleSize = SetSampleSize(_partQuantuty);
                sampleForm.AvgMax = Convert.ToDouble(question.MaxValue);
                sampleForm.AvgMin = Convert.ToDouble(question.MinValue);
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

        private int SetSampleSize(int quantity)
        {
            
            try
            {
                var size = "10";
                int qty = 10;
                if (quantity > 0)
                    qty = quantity;
                string sampleSizetxt = !(QuestionRow.IsSampleSizeNull())? QuestionRow.SampleSize:null;

                if (sampleSizetxt != null)
                { 
                    string[] samplethresholds = sampleSizetxt.Split('|');

                    foreach (var threshold in samplethresholds)
                    {
                        
                        string[] thresholdParam = threshold.Split(',');

                        if(thresholdParam.Length <= 1)
                        {
                            int i = 0;
                            bool isNum = int.TryParse(thresholdParam[0], out i);
                            
                            return i;
                        }
                        var floor = thresholdParam[0];
                        var limit = thresholdParam[1];
                        size = thresholdParam[2];

                        if (qty >= Convert.ToInt32(floor) && (limit.ToUpper() == "MAX" || qty <= Convert.ToInt32(limit)))
                        {
                            return Convert.ToInt32(size);
                        }

                    }
                }
                return 0;

            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error determining samplesize.");
            }
            return 0;
        }

        #endregion
    }
}