using System;
using System.Data;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ProcessesDatasetTableAdapters;
using DWOS.Shared;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DWOS.Data;

namespace DWOS.UI.Admin.Processes
{
    public partial class ProcessStepQuestion: DataPanel
    {
        #region Fields

        private TextControlValidator _answerValidator;
        private ListsTableAdapter _taLists;
        private NumericUnitsTableAdapter _taNumericUnits;

        #endregion

        #region Properties

        public ProcessesDataset Dataset
        {
            get { return base._dataset as ProcessesDataset; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return this.Dataset.ProcessQuestion.ProcessQuestionIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public ProcessStepQuestion()
        {
            this.InitializeComponent();

            this.richTextEditorToolbar1.RichTextEditor = this.txtDescription;
        }

        public void LoadData(ProcessesDataset dataset, ListsTableAdapter taLists, NumericUnitsTableAdapter taNumericUnits)
        {
            this.Dataset = dataset;
            bsData.DataSource = this.Dataset;
            bsData.DataMember = this.Dataset.ProcessQuestion.TableName;

            this._taLists = taLists;
            this._taNumericUnits = taNumericUnits;

            //bind column to control
            base.BindValue(this.txtName, this.Dataset.ProcessQuestion.NameColumn.ColumnName);
            base.BindValue(this.txtValue, this.Dataset.ProcessQuestion.DefaultValueColumn.ColumnName);
            base.BindValue(this.cboProcessQuesInputType, this.Dataset.ProcessQuestion.InputTypeColumn.ColumnName, true);
            base.BindValue(this.cboProcessQuesList, this.Dataset.ProcessQuestion.ListIDColumn.ColumnName, true);
            base.BindValue(this.cboUnits, this.Dataset.ProcessQuestion.NumericUntisColumn.ColumnName);
            base.BindValue(this.chkEditable, this.Dataset.ProcessQuestion.OperatorEditableColumn.ColumnName);
            base.BindValue(this.chkRequired, this.Dataset.ProcessQuestion.RequiredColumn.ColumnName);
            base.BindValue(this.numMaxValue, this.Dataset.ProcessQuestion.MaxValueColumn.ColumnName);
            base.BindValue(this.numMinValue, this.Dataset.ProcessQuestion.MinValueColumn.ColumnName);
            base.BindValue(this.numTolerance, this.Dataset.ProcessQuestion.ToleranceColumn.ColumnName);
            base.BindValue(this.numOrder, this.Dataset.ProcessQuestion.StepOrderColumn.ColumnName, true);
            base.BindValue(this.txtDescription, this.Dataset.ProcessQuestion.NotesColumn.ColumnName, false);

            //bind lists
            base.BindList(this.cboProcessQuesInputType, this.Dataset.d_InputType, this.Dataset.d_InputType.InputTypeColumn.ColumnName, this.Dataset.d_InputType.InputTypeColumn.ColumnName);
            base.BindList(this.cboProcessQuesList, this.Dataset.Lists, this.Dataset.Lists.ListIDColumn.ColumnName, this.Dataset.Lists.NameColumn.ColumnName);
            base.BindList(this.cboUnits, this.Dataset.NumericUnits, this.Dataset.NumericUnits.UnitTypeColumn.ColumnName, this.Dataset.NumericUnits.UnitTypeColumn.ColumnName);

            questionFieldPopup.Setup(Dataset.CustomFieldToken);

            var customFields = Dataset
                .CustomFieldToken
                .Where(t => !t.IsTokenNameNull() && !string.IsNullOrEmpty(t.TokenName))
                .OrderBy(t => t.TokenName);

            foreach (var customField in customFields)
            {
                cboSaveField.Items.Add(customField.TokenName);
            }

            if (!customFields.Any())
            {
                cboSaveField.NullText = "No Tokens Found";
            }

            base._panelLoaded = true;
        }

        public override void AddValidators(DWOS.Utilities.Validation.ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtName, "Process Question required."){DefaultValue = "New Process Question"}, errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboProcessQuesInputType, "Process Question Input Type required."), errProvider));

            //create answer validator with no validation until answer type is selected
            this._answerValidator = new TextControlValidator(false, 0, this.txtValue);
            manager.Add(new ImageDisplayValidator(this._answerValidator, errProvider));
        }

        public ProcessesDataset.ProcessQuestionRow AddProcessQuestionRow(int processStepID, decimal stepID)
        {
            var rowVw        = bsData.AddNew() as DataRowView;
            var cr           = rowVw.Row as ProcessesDataset.ProcessQuestionRow;
            cr.Name          = "New Process Question";
            cr.ProcessStepID = processStepID;
            cr.StepOrder     = stepID;
            cr.Required      = true;

            return cr;
        }

        protected override void OnEditableStatusChange(bool editable)
        {
            base.OnEditableStatusChange(editable);
            this.UpdateReadOnlyStatus(editable);
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);
            RefreshFieldButtons();
            RefreshSaveFieldComboBox();
        }

        private void RefreshFieldButtons()
        {
            var currentQuestion = CurrentRecord as ProcessesDataset.ProcessQuestionRow;
            if (currentQuestion == null)
            {
                return;
            }

            var fields = currentQuestion.GetProcessQuestionFieldRows();

            var valueBtn = txtValue.ButtonsLeft[0];
            if (fields.Any(i => i.IsValidState() && i.FieldName == QuestionField.Answer.ToString()))
            {
                valueBtn.Appearance.BackColor = Color.SeaGreen;
            }
            else
            {
                valueBtn.Appearance.ResetBackColor();
            }

            var minBtn = numMinValue.ButtonsLeft[0];
            if (fields.Any(i => i.IsValidState() && i.FieldName == QuestionField.MinValue.ToString()))
            {
                minBtn.Appearance.BackColor = Color.SeaGreen;
            }
            else
            {
                minBtn.Appearance.ResetBackColor();
            }

            var maxBtn = numMaxValue.ButtonsLeft[0];
            if (fields.Any(i => i.IsValidState() && i.FieldName == QuestionField.MaxValue.ToString()))
            {
                maxBtn.Appearance.BackColor = Color.SeaGreen;
            }
            else
            {
                maxBtn.Appearance.ResetBackColor();
            }

            var toleranceBtn = numTolerance.ButtonsLeft[0];
            if (fields.Any(i => i.IsValidState() && i.FieldName == QuestionField.Tolerance.ToString()))
            {
                toleranceBtn.Appearance.BackColor = Color.SeaGreen;
            }
            else
            {
                toleranceBtn.Appearance.ResetBackColor();
            }
        }

        private void RefreshSaveFieldComboBox()
        {
            cboSaveField.SelectedIndex = -1;

            var currentQuestion = CurrentRecord as ProcessesDataset.ProcessQuestionRow;
            if (currentQuestion == null)
            {
                return;
            }

            var fields = currentQuestion.GetProcessQuestionFieldRows();

            var currentField = fields
                .FirstOrDefault(i => i.IsValidState() && i.FieldName == QuestionField.AnswerOut.ToString());

            if (currentField != null)
            {
                var fieldItem = cboSaveField.FindItemByValue<string>(i => i == currentField.TokenName);

                if (fieldItem == null)
                {
                    cboSaveField.Items.Add(currentField.TokenName);
                    fieldItem = cboSaveField.FindItemByValue<string>(i => i == currentField.TokenName);
                }

                cboSaveField.SelectedItem = fieldItem;
            }
        }

        private void UpdateReadOnlyStatus(bool editable)
        {
            try
            {
                numTolerance.ValueChanged -= numTolerance_ValueChanged;

                if(!String.IsNullOrEmpty(this.cboProcessQuesInputType.Text))
                {
                    var selectedValue = this.cboProcessQuesInputType.Text.ConvertToEnum<InputType>();

                    bool isInteger = selectedValue == InputType.Integer;

                    bool isDecimal = selectedValue == InputType.Decimal ||
                                     selectedValue == InputType.DecimalBefore ||
                                     selectedValue == InputType.DecimalAfter ||
                                     selectedValue == InputType.DecimalDifference ||
                                     selectedValue == InputType.PreProcessWeight ||
                                     selectedValue == InputType.PostProcessWeight;

                    bool isWeight = selectedValue == InputType.PreProcessWeight ||
                                    selectedValue == InputType.PostProcessWeight;

                    bool isList = selectedValue == InputType.List;
                    bool isDuration = selectedValue == InputType.TimeDuration;
                    bool isRampTime = selectedValue == InputType.RampTime;
                    bool isSample = selectedValue == InputType.SampleSet;

                    var canEditNumbers = editable && (isInteger || isDecimal || isDuration || isRampTime || isSample);

                    this.numMaxValue.ReadOnly = !canEditNumbers;
                    this.numMinValue.ReadOnly = !canEditNumbers;
                    this.numTolerance.ReadOnly = !canEditNumbers || isSample;

                    this.cboUnits.ReadOnly = !(isInteger || isDecimal || isDuration || isRampTime || isSample) || isWeight;
                    this.cboProcessQuesList.ReadOnly = !isList;

                    // Set background color for numerical controls to convey
                    // enabled /disabled status
                    var numberBackColor = canEditNumbers ? SystemColors.Control : SystemColors.ControlDark;
                    this.numMaxValue.BackColor = numberBackColor;
                    this.numMinValue.BackColor = numberBackColor;
                    this.numTolerance.BackColor = (isSample ? SystemColors.ControlDark : numberBackColor);

                    //store original values before reseting when changing type
                    var rv = bsData.Current as DataRowView;
                    object minObj = rv[this.Dataset.ProcessQuestion.MinValueColumn.ColumnName];
                    object maxObj = rv[this.Dataset.ProcessQuestion.MaxValueColumn.ColumnName];
                    object toleranceObj = rv[this.Dataset.ProcessQuestion.ToleranceColumn.ColumnName];

                    //reset validator for the answer
                    this._answerValidator.RegExpPattern = null;
                    this._answerValidator.RegExpText = null;

                    this.numMaxValue.ResetValue();
                    this.numMinValue.ResetValue();
                    this.numTolerance.Value = null;

                    if (isInteger || isDuration || isRampTime)
                    {
                        // Order is important - otherwise, MaskInput or NumericType would be invalid
                        this.numMaxValue.MaskInput = "-nnn,nnn";
                        this.numMinValue.MaskInput = "-nnn,nnn";
                        this.numTolerance.MaskInput = "nnn,nnn";
                        this.numMaxValue.NumericType = NumericType.Integer;
                        this.numMinValue.NumericType = NumericType.Integer;
                        this.numTolerance.NumericType = NumericType.Integer;
                        this.numMinValue.Value = 0;
                        this.numMaxValue.Value = 999;

                        this._answerValidator.RegExpPattern = @"^\d{1,6}$"; // + "|(^<[0-9a-fA-F]>$)";
                        this._answerValidator.RegExpText = "-nnn,nnn";
                    }
                    else if(isDecimal)
                    {
                        this.numMaxValue.NumericType = NumericType.Decimal;
                        this.numMinValue.NumericType = NumericType.Decimal;
                        this.numTolerance.NumericType = NumericType.Decimal;
                        this.numMaxValue.MaskInput = "-nnn,nnn.nnnn";
                        this.numMinValue.MaskInput = "-nnn,nnn.nnnn";
                        this.numTolerance.MaskInput = "nnn,nnn.nnnn";
                        this.numMinValue.Value = 0;
                        this.numMaxValue.Value = 99999;

                        this._answerValidator.RegExpPattern = @"^[-+]?\d{0,6}(\.\d+)?$";
                        this._answerValidator.RegExpText = "-nnn,nnn.nnnn";
                    }

                    //attempt to reset to original values if we can
                    if (isInteger || isDecimal || isDuration || isRampTime)
                    {
                        if(this.numMinValue.ValidateWithinRange(minObj))
                            this.numMinValue.Value = minObj;
                        else
                            this.numMinValue.DataBindings[0].WriteValue();

                        if(this.numMaxValue.ValidateWithinRange(maxObj))
                            this.numMaxValue.Value = maxObj;
                        else
                            this.numMaxValue.DataBindings[0].WriteValue();

                        if (this.numTolerance.ValidateWithinRange(toleranceObj))
                            this.numTolerance.Value = toleranceObj;
                        else
                            this.numTolerance.DataBindings[0].WriteValue();
                    }
                }
                else
                {
                    this.numMaxValue.ReadOnly = true;
                    this.numMinValue.ReadOnly = true;
                    this.cboProcessQuesList.ReadOnly = true;
                    this.cboUnits.ReadOnly = true;
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error changing question input type.", exc);
                _log.Fatal(exc, "Error changing question input type.");
            }
            finally
            {
                numTolerance.ValueChanged += numTolerance_ValueChanged;
                RefreshToleranceNotifications();
            }
        }

        public override void EndEditing()
        {
            var saveTokenName = cboSaveField.SelectedItem?.DataValue as string;

            if (string.IsNullOrEmpty(saveTokenName))
            {
                DeleteField(QuestionField.AnswerOut);
            }
            else
            {
                SaveField(QuestionField.AnswerOut, saveTokenName);
            }

            base.EndEditing();
        }

        private void ShowTooltip(Control control)
        {
            if (control == null || control.Enabled)
            {
                return;
            }

            tipManager.ShowToolTip(control);
        }

        private void HideTooltip(Control control)
        {
            if (control == null || control.Enabled || !tipManager.IsToolTipVisible(control))
            {
                return;
            }

            tipManager.HideToolTip();
        }

        private void RefreshToleranceNotifications()
        {
            var showNotifications = numTolerance.Value != null && numTolerance.Value != DBNull.Value;
            lblMinCalculated.Visible = showNotifications;
            lblMaxCalculated.Visible = showNotifications;
        }

        private void PrepareFieldPopup(QuestionField field)
        {
            var fieldString = field.ToString();
            var currentQuestion = CurrentRecord as ProcessesDataset.ProcessQuestionRow;

            if (currentQuestion == null)
            {
                return;
            }

            var fields = currentQuestion.GetProcessQuestionFieldRows();

            var currentField = fields
                .FirstOrDefault(i => i.IsValidState() && i.FieldName == fieldString);

            questionFieldPopup.LoadData(field, currentField?.TokenName);
        }

        private void SaveField(QuestionField field, string selectedToken)
        {
            var fieldString = field.ToString();
            var currentQuestion = CurrentRecord as ProcessesDataset.ProcessQuestionRow;

            if (currentQuestion == null || !currentQuestion.IsValidState() || string.IsNullOrEmpty(selectedToken))
            {
                return;
            }

            var currentField = currentQuestion
                .GetProcessQuestionFieldRows()
                .FirstOrDefault(i => i.IsValidState() && i.FieldName == fieldString);

            if (currentField == null)
            {
                var newField = Dataset.ProcessQuestionField.NewProcessQuestionFieldRow();
                newField.FieldName = fieldString;
                newField.TokenName = selectedToken;
                newField.ProcessQuestionRow = currentQuestion;

                Dataset.ProcessQuestionField.AddProcessQuestionFieldRow(newField);
            }
            else if (currentField.TokenName != selectedToken)
            {
                currentField.TokenName = selectedToken;
            }
        }

        private void DeleteField(QuestionField field)
        {
            var fieldString = field.ToString();
            var currentQuestion = CurrentRecord as ProcessesDataset.ProcessQuestionRow;

            if (currentQuestion == null || !currentQuestion.IsValidState())
            {
                return;
            }

            var currentField = currentQuestion
                .GetProcessQuestionFieldRows()
                .FirstOrDefault(i => i.IsValidState() && i.FieldName == fieldString);

            currentField?.Delete();
        }

        #endregion

        #region Events

        private void cboProcessQuesInputType_ValueChanged(object sender, EventArgs e)
        {
            UpdateReadOnlyStatus(Editable);

            try
            {
                if (!string.IsNullOrEmpty(cboProcessQuesInputType.Text))
                {
                    var selectedValue = cboProcessQuesInputType.Text
                        .ConvertToEnum<InputType>();

                    if (selectedValue == InputType.PreProcessWeight || selectedValue == InputType.PostProcessWeight)
                    {
                        cboUnits.Value = "lbs";
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error while changing question input type.");
            }
        }

        private void cboProcessQuesList_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                // Get list of in-use ListIDs
                var listIDsInUse = new List<int>();

                listIDsInUse.AddRange(Dataset.ProcessQuestion
                    .Where(question => question.IsValidState() && !question.IsListIDNull())
                    .Select(question => question.ListID));

                listIDsInUse.Add((this.cboProcessQuesList.Value as int?).GetValueOrDefault());

                using(var editor = new ListEditor())
                {
                    editor.DoNotAllowDeletionOf(listIDsInUse);
                    if(editor.ShowDialog(this) == DialogResult.OK)
                    {
                        // EnforceConstraints must be false before clearing
                        // the list table.
                        this.Dataset.EnforceConstraints = false;
                        this._taLists.Fill(this.Dataset.Lists);
                        this.Dataset.EnforceConstraints = true;
                        this.cboProcessQuesList.DataBind();

                        //attempt to select last selected list
                        int listID = editor.SelectedListID;

                        if(listID >= 0)
                        {
                            ValueListItem vli = this.cboProcessQuesList.FindItemByValue<int>(v => v == listID);

                            if(vli != null)
                                this.cboProcessQuesList.SelectedItem = vli;
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error changing process question list.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
                _log.Fatal(exc, errorMsg);
            }
        }

        private void cboUnits_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                using (var editor = new DomainValueEditor() { Text = "Units Editor" })
                {
                    using (var ta = new Data.Datasets.ProcessesDatasetTableAdapters.NumericUnitsTableAdapter())
                    {
                        var dt = new Data.Datasets.ProcessesDataset.NumericUnitsDataTable();
                        ta.Fill(dt);

                        editor.AddValue = () =>
                        {
                            var newUnitType = "New Units";
                            var rowFound = true;
                            var nextIndex = 1;
                            var tmpUnitType = newUnitType;

                            while (rowFound)
                            {
                                var row = dt.FindByUnitType(tmpUnitType);
                                if (row == null)
                                {
                                    newUnitType = tmpUnitType;
                                    rowFound = false;
                                }
                                else
                                {
                                    tmpUnitType = newUnitType + "_" + nextIndex;
                                    nextIndex++;
                                }
                            }

                            var newRow = dt.AddNumericUnitsRow(newUnitType);
                            return new DomainValueEditor.DomainValue() { Name = newUnitType, AllowDelete = true, AllowEdit = true, Row = newRow };
                        };
                        editor.NameChanged = (dv) =>
                        {
                                ((Data.Datasets.ProcessesDataset.NumericUnitsRow)dv.Row).UnitType = dv.Name.Replace("'", "*"); 
                        };

                        dt.ForEach(unit => editor.AddDomainValue(new DomainValueEditor.DomainValue() { Name = unit.UnitType, AllowDelete = true, AllowEdit = true, Row = unit }));

                        if (editor.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                        {
                            ta.Update(dt);

                            this._taNumericUnits.Fill(this.Dataset.NumericUnits); //update unit values
                            this.cboUnits.DataBind();

                            var selectedNode = editor.SelectedValue;
                            if (selectedNode != null)
                            {
                                this.cboUnits.Value = selectedNode.Name;
                            }
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error changing process question units.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
                _log.Fatal(exc, errorMsg);
            }
        }

        private void cboSaveField_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                if (e.Button.Key != "Delete")
                {
                    return;
                }

                cboSaveField.SelectedIndex = -1;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error removing save field.");
            }
        }

        private void numTolerance_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                RefreshToleranceNotifications();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error showing/hiding tolerance-related notifications.");
            }
        }

        private void txtValue_BeforeEditorButtonDropDown(object sender, BeforeEditorButtonDropDownEventArgs e)
        {
            try
            {
                PrepareFieldPopup(QuestionField.Answer);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error opening token pop-up");
            }
        }

        private void numMinValue_BeforeEditorButtonDropDown(object sender, BeforeEditorButtonDropDownEventArgs e)
        {
            try
            {
                PrepareFieldPopup(QuestionField.MinValue);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error opening token pop-up");
            }
        }

        private void numMaxValue_BeforeEditorButtonDropDown(object sender, BeforeEditorButtonDropDownEventArgs e)
        {
            try
            {
                PrepareFieldPopup(QuestionField.MaxValue);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error opening token pop-up");
            }
        }

        private void numTolerance_BeforeEditorButtonDropDown(object sender, BeforeEditorButtonDropDownEventArgs e)
        {
            try
            {
                PrepareFieldPopup(QuestionField.Tolerance);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error opening token pop-up");
            }
        }

        private void questionFieldPopup_CancelClick(object sender, EventArgs e)
        {
            try
            {
                txtValue.CloseEditorButtonDropDowns();
                numMinValue.CloseEditorButtonDropDowns();
                numMaxValue.CloseEditorButtonDropDowns();
                numTolerance.CloseEditorButtonDropDowns();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error canceling token pop-up");
            }
        }

        private void questionFieldPopup_OkClick(object sender, EventArgs e)
        {
            try
            {
                txtValue.CloseEditorButtonDropDowns();
                numMinValue.CloseEditorButtonDropDowns();
                numMaxValue.CloseEditorButtonDropDowns();
                numTolerance.CloseEditorButtonDropDowns();

                if (questionFieldPopup.IsFieldRemoved)
                {
                    DeleteField(questionFieldPopup.Field);
                }
                else
                {
                    SaveField(questionFieldPopup.Field, questionFieldPopup.SelectedToken);
                }

                RefreshFieldButtons();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error accepting token pop-up");
            }
        }


        private void pnlOrder_MouseHover(object sender, EventArgs e)
        {
            try
            {
                ShowTooltip(numOrder);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error showing order tooltip.");
            }
        }

        private void pnlOrder_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                HideTooltip(numOrder);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error hiding order tooltip.");
            }
        }

        private void pnlProcessQuesInputType_MouseHover(object sender, EventArgs e)
        {
            try
            {
                ShowTooltip(cboProcessQuesInputType);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error showing input type tooltip.");
            }
        }

        private void pnlProcessQuesInputType_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                HideTooltip(cboProcessQuesInputType);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error hiding input type tooltip.");
            }
        }

        private void pnlProcessQuesList_MouseHover(object sender, EventArgs e)
        {
            try
            {
                ShowTooltip(cboProcessQuesList);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error showing list tooltip.");
            }
        }

        private void pnlProcessQuesList_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                HideTooltip(cboProcessQuesList);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error hiding input type tooltip.");
            }
        }

        #endregion
    }
}