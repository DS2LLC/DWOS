using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DWOS.Data.Conditionals;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using DWOS.Utilities.Validation;

namespace DWOS.UI.Admin.Processes
{
    public partial class StepConditionEditor : Form
    {
        #region Fields


        #endregion

        #region Properties

        internal ConditionInputType SelectedConditionInputType
        {
            get { return cboConditionType.SelectedItem != null ? (ConditionInputType) cboConditionType.SelectedItem.DataValue : ConditionInputType.None; }
        }

        internal EqualityOperator SelectedOperator
        {
            get { return cboOperator.SelectedItem != null ? (EqualityOperator)cboOperator.SelectedItem.DataValue : EqualityOperator.None; }
        }

        private Data.Datasets.ProcessesDataset.ProcessStepConditionRow StepCondition { get; set; }

        #endregion

        #region Methods

        public StepConditionEditor()
        {
            InitializeComponent();
        }

        public void LoadData(Data.Datasets.ProcessesDataset.ProcessStepConditionRow processStepCondition)
        {
            this.StepCondition  = processStepCondition;
            var processStep     = processStepCondition.ProcessStepsRow;

            txtStepName.Text = processStep.Name;

            //LOAD Input Types
            cboConditionType.Items.Add(ConditionInputType.ProcessQuestion, "Process Question");
            cboConditionType.Items.Add(ConditionInputType.PartTag, "Part Tag");
            //cboConditionType.Items.Add("Order Tag");

            var inputType = processStepCondition.InputType.ParseStringToEnum <ConditionInputType>();
            var findInput = cboConditionType.FindItemByValue <ConditionInputType>(ci => ci == inputType);

            if (findInput != null)
                cboConditionType.SelectedItem = findInput;
            else
                cboConditionType.SelectedIndex = 1;
            
            //LOAD Process Steps
            cboProcessStep.Items.Clear();
            var steps = processStep.ProcessRow.GetProcessStepsRows().OrderBy(psr => psr.StepOrder);
            foreach(var step in steps)
            {
                if(step == processStep)
                    break;

                cboProcessStep.Items.Add(step, step.StepOrder + " - " + step.Name);
            }

            if(!processStepCondition.IsProcessQuestionIdNull())
                cboProcessStep.SelectedItem = cboProcessStep.FindItemByValue<Data.Datasets.ProcessesDataset.ProcessStepsRow>(psr => psr.GetProcessQuestionRows().Any(pqr => pqr.ProcessQuestionID == processStepCondition.ProcessQuestionId));

            if (cboProcessStep.SelectedItem == null && cboProcessStep.Items.Count > 0)
                cboProcessStep.SelectedIndex = 0;

            //LOAD Questions - Should be loaded of change event of the process step above, so just select question
            if (cboProcessStep.SelectedItem != null && cboProcessQuestion.Items.Count > 0)
            {
                if(!processStepCondition.IsProcessQuestionIdNull())
                    cboProcessQuestion.SelectedItem = cboProcessQuestion.FindItemByValue<Data.Datasets.ProcessesDataset.ProcessQuestionRow>(pq => pq.ProcessQuestionID == processStepCondition.ProcessQuestionId);

                if(cboProcessQuestion.SelectedItem == null && cboProcessQuestion.Items.Count > 0)
                    cboProcessQuestion.SelectedIndex = 0;
            }

            //LOAD Operators
            cboOperator.Items.Clear();
            cboOperator.Items.Add(EqualityOperator.GreaterThan, ">");
            cboOperator.Items.Add(EqualityOperator.LessThan, "<");
            cboOperator.Items.Add(EqualityOperator.Equal, "=");
            cboOperator.Items.Add(EqualityOperator.NotEqual, "<>");

            var selectedOp = processStepCondition.Operator.ParseStringToEnum<EqualityOperator>();
            var foundOp = cboOperator.FindItemByValue <EqualityOperator>(eo => eo == selectedOp);
            if(foundOp != null)
                cboOperator.SelectedItem = foundOp;
            if(cboOperator.SelectedItem == null)
                cboOperator.SelectedIndex = 0;
            
            //LOAD Value
            smartBox.ValueChanged += txtValue_ValueChanged;
            smartBox.SetValue(processStepCondition.Value);
        }

        private bool SaveData()
        {
            StepCondition.InputType = SelectedConditionInputType.ToString();
            StepCondition.Operator = SelectedOperator.ToString();
            var answer = smartBox.GetValue();
            StepCondition.Value = answer == null ? null : answer.ToString();
            
            if(cboProcessQuestion.SelectedItem != null)
                StepCondition.ProcessQuestionId = ((Data.Datasets.ProcessesDataset.ProcessQuestionRow) cboProcessQuestion.SelectedItem.DataValue).ProcessQuestionID;
            else
                StepCondition.SetProcessQuestionIdNull();

            return true;
        }

        private void UpdateDisplayText()
        {
            // Question condition
            if (SelectedConditionInputType == ConditionInputType.ProcessQuestion && cboProcessQuestion.SelectedItem != null)
            {
                var questionRow = cboProcessQuestion.SelectedItem.DataValue as Data.Datasets.ProcessesDataset.ProcessQuestionRow;
                if (questionRow != null)
                {
                    var answer = smartBox.GetValue();

                    lblConditionText.Text = string.Format("Show this step if '{0}' is '{1}' '{2}: {3} - {4}'",
                        answer?.ToString(),
                        cboOperator.Text,
                        questionRow.ProcessStepsRow.StepOrder.ToString(),
                        questionRow.ProcessStepsRow.Name,
                        questionRow.Name);

                    return;
                }
            }

            // Part Tag condition
            if (SelectedConditionInputType == ConditionInputType.PartTag)
            {
                var answer = smartBox.GetValue();
                lblConditionText.Text = string.Format("Show this step if Part has tag '{0}'",
                    answer?.ToString());

                return;
            }

            lblConditionText.Text = "None";
        }

        private void LoadQuestions(Data.Datasets.ProcessesDataset.ProcessStepsRow step)
        {
            cboProcessQuestion.Items.Clear();

            if(step != null)
                step.GetProcessQuestionRows().ForEach(q => cboProcessQuestion.Items.Add(q, q.StepOrder + " - " + q.Name));

            if(cboProcessQuestion.Items.Count > 0)
                cboProcessQuestion.SelectedIndex = 0;
        }

        private void CreateAnswerBox()
        {
            try 
            {
                if (cboProcessStep.SelectedItem != null && cboProcessQuestion.SelectedItem != null && this.SelectedConditionInputType == ConditionInputType.ProcessQuestion)
                {
                    var question = cboProcessQuestion.SelectedItem.DataValue as Data.Datasets.ProcessesDataset.ProcessQuestionRow;

                    if (question != null)
                    {
                        var inputType = question.InputType.ParseStringToEnum<InputType>();

                        if (inputType == InputType.List || inputType != smartBox.InputType)
                            smartBox.CreateControl(inputType, (question.IsListIDNull() ? 0 : question.ListID), (question.IsNumericUntisNull() ? null : question.NumericUntis), (question.IsDefaultValueNull() ? null : question.DefaultValue));
                    }
                    else
                    {
                        smartBox.CreateControl(InputType.String, 0, null, null);
                    }
                }
                else
                    smartBox.CreateControl(InputType.String, 0, null, null);

                picInputType.Image = ControlUtilities.GetInputTypeImage(smartBox.InputType);
            }
            catch(Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error creating answer box.");
            }
        }

        #endregion

        #region Events
        
        private void cboConditionType_ValueChanged(object sender, EventArgs e)
        {
            switch(this.SelectedConditionInputType)
            {
                case ConditionInputType.PartTag:
                    cboProcessQuestion.Enabled = false;
                    cboProcessStep.Enabled = false;
                    lblValue.Text = "Tag:";
                    break;
                case ConditionInputType.None:
                case ConditionInputType.ProcessQuestion:
                default:
                    cboProcessQuestion.Enabled = true;
                    cboProcessStep.Enabled = true;
                    lblValue.Text = "Value:";
                    break;
            }

            CreateAnswerBox();
            UpdateDisplayText();
        }

        private void cboProcessStep_ValueChanged(object sender, EventArgs e)
        {
            if(cboProcessStep.SelectedItem != null)
            {
                var stepRow = cboProcessStep.SelectedItem.DataValue as Data.Datasets.ProcessesDataset.ProcessStepsRow;
                if(stepRow != null)
                    LoadQuestions(stepRow);
            }

            UpdateDisplayText();
        }

        private void cboOperator_ValueChanged(object sender, EventArgs e)
        {
            UpdateDisplayText();
        }

        private void txtValue_ValueChanged(object sender, EventArgs e)
        {
            UpdateDisplayText();
        }
        
        private void cboProcessQuestion_ValueChanged(object sender, EventArgs e)
        {
            CreateAnswerBox();
            UpdateDisplayText();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(this.SelectedConditionInputType == ConditionInputType.None)
            {
                MessageBoxUtilities.ShowMessageBoxWarn("Invalid condition type.", "Condition Type");
                return;
            }
            
            if (smartBox.GetValue() == null)
            {
                MessageBoxUtilities.ShowMessageBoxWarn("Invalid condition value.", "No Value", "A value must be specified.");
                return;
            }
            

            if (this.SelectedConditionInputType == ConditionInputType.ProcessQuestion)
            {
                if (this.cboProcessStep.SelectedItem == null)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("Invalid process step selected.", "Invalid Process Step", "Ensure you selected a process step when using Process Question conditions.");
                    return;
                }
                if (this.cboProcessQuestion.SelectedItem == null)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("Invalid process question selected.", "Invalid Process Question", "Ensure you selected a process question when using Process Question conditions.");
                    return;
                }
            }

            if(SaveData())
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        #endregion
    }
}
