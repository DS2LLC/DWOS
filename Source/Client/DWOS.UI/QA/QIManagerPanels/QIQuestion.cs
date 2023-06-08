using System;
using System.Data;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.PartInspectionDataSetTableAdapters;
using DWOS.Shared;
using DWOS.UI.Admin;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using System.Drawing;
using DWOS.Data.Conditionals;
using DWOS.UI.Tools;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinTree;
using System.Windows.Interop;

namespace DWOS.UI.QA.QIManagerPanels
{
    public partial class QIQuestion : DataPanel
    {
        #region Fields

        private TextControlValidator _answerValidator;
        private ListsTableAdapter _taLists;
        private readonly DisplayDisabledTooltips _displayTooltipsMain;

        #endregion

        #region Properties

        public PartInspectionDataSet Dataset
        {
            get { return base._dataset as PartInspectionDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.PartInspectionQuestion.PartInspectionQuestionIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public QIQuestion()
        {
            InitializeComponent();
            _displayTooltipsMain = new DisplayDisabledTooltips(grpData, tipManager);
        }

        public void LoadData(PartInspectionDataSet dataset, ListsTableAdapter taLists)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.PartInspectionQuestion.TableName;

            this._taLists = taLists;

            //bind column to control
            base.BindValue(this.txtName, Dataset.PartInspectionQuestion.NameColumn.ColumnName);
            base.BindValue(this.txtValue, Dataset.PartInspectionQuestion.DefaultValueColumn.ColumnName);
            base.BindValue(this.cboProcessQuesInputType, Dataset.PartInspectionQuestion.InputTypeColumn.ColumnName, true);
            base.BindValue(this.cboProcessQuesList, Dataset.PartInspectionQuestion.ListIDColumn.ColumnName, true);
            base.BindValue(this.cboUnits, Dataset.PartInspectionQuestion.NumericUntisColumn.ColumnName);
            base.BindValue(this.numMaxValue, Dataset.PartInspectionQuestion.MaxValueColumn.ColumnName, true);
            base.BindValue(this.numMinValue, Dataset.PartInspectionQuestion.MinValueColumn.ColumnName, true);
            base.BindValue(this.numOrder, Dataset.PartInspectionQuestion.StepOrderColumn.ColumnName, true);
            base.BindValue(this.txtSampleSize, Dataset.PartInspectionQuestion.SampleSizeColumn.ColumnName, true);

            //bind lists
            base.BindList(this.cboProcessQuesInputType, Dataset.d_InputType, Dataset.d_InputType.InputTypeColumn.ColumnName, Dataset.d_InputType.InputTypeColumn.ColumnName);
            base.BindList(this.cboProcessQuesList, Dataset.Lists, Dataset.Lists.ListIDColumn.ColumnName, Dataset.Lists.NameColumn.ColumnName);
            base.BindList(this.cboUnits, Dataset.NumericUnits, Dataset.NumericUnits.UnitTypeColumn.ColumnName, Dataset.NumericUnits.UnitTypeColumn.ColumnName);

            // Load commands
            Commands.AddCommand("AddConditionCommand", new AddConditionCommand(btnAddCondition, tvwConditions, this));
            Commands.AddCommand("EditConditionCommand", new EditConditionCommand(btnEditCondition, tvwConditions, this));
            Commands.AddCommand("DeleteConditionCommand", new DeleteConditionCommand(btnDeleteCondition, tvwConditions));

            base._panelLoaded = true;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            tvwConditions.Nodes.Clear();

            var question = CurrentRecord as PartInspectionDataSet.PartInspectionQuestionRow;

            if (question != null)
            {
                foreach (var conditionRow in question.GetMainConditionRows())
                {
                    tvwConditions.Nodes.Add(new ConditionNode(conditionRow));
                }
            }
        }

        public override void AddValidators(DWOS.Utilities.Validation.ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtName, "Inspection Question required.") {DefaultValue = "New Inspection Question"}, errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboProcessQuesInputType, "Inspection Question Input Type required."), errProvider));

            //create answer validator with no validation until answer type is selected
            this._answerValidator = new TextControlValidator(false, 0, this.txtValue);
            manager.Add(new ImageDisplayValidator(this._answerValidator, errProvider));
        }

        public PartInspectionDataSet.PartInspectionQuestionRow AddQuestionRow(int inspectionTypeID, decimal stepID)
        {
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as PartInspectionDataSet.PartInspectionQuestionRow;
            cr.Name = "New Inspection Question";
            cr.PartInspectionTypeID = inspectionTypeID;
            cr.StepOrder = stepID;
            cr.InputType = Dataset.d_InputType[0].InputType;

            return cr;
        }

        protected override void OnEditableStatusChange(bool editable)
        {
            base.OnEditableStatusChange(editable);
            UpdateReadOnlyStatus();
        }

        private void UpdateReadOnlyStatus()
        {
            try
            {
                if(!String.IsNullOrEmpty(this.cboProcessQuesInputType.Text))
                {
                    var selectedValue = this.cboProcessQuesInputType.Text.ConvertToEnum <InputType>();

                    bool isInteger = selectedValue == InputType.Integer;

                    bool isDecimal = selectedValue == InputType.Decimal ||
                        selectedValue == InputType.DecimalBefore ||
                        selectedValue == InputType.DecimalAfter ||
                        selectedValue == InputType.DecimalDifference ||
                        selectedValue == InputType.PreProcessWeight ||
                        selectedValue == InputType.PostProcessWeight ||
                        selectedValue == InputType.SampleSet;

                    bool isList = selectedValue == InputType.List;
                    bool isDuration = selectedValue == InputType.TimeDuration;
                    bool isRampTime = selectedValue == InputType.RampTime;

                    this.numMaxValue.ReadOnly = !(isInteger || isDecimal || isDuration || isRampTime);
                    this.numMinValue.ReadOnly = !(isInteger || isDecimal || isDuration || isRampTime);
                    this.cboUnits.ReadOnly = !(isInteger || isDecimal || isDuration || isRampTime);
                    this.cboProcessQuesList.ReadOnly = !isList;

                    //store original values before reseting when changing type
                    var rv = bsData.Current as DataRowView;
                    object minObj = rv[Dataset.PartInspectionQuestion.MinValueColumn.ColumnName];
                    object maxObj = rv[Dataset.PartInspectionQuestion.MaxValueColumn.ColumnName];

                    //reset validator for the answer
                    this._answerValidator.RegExpPattern = null;
                    this._answerValidator.RegExpText = null;

                    this.numMaxValue.ResetValue();
                    this.numMinValue.ResetValue();

                    this.txtSampleSize.Enabled = selectedValue == InputType.SampleSet;

                    if (isInteger || isDuration || isRampTime)
                    {
                        this.numMaxValue.MaskInput = "-nnn,nnn";
                        this.numMinValue.MaskInput = "-nnn,nnn";
                        this.numMaxValue.NumericType = NumericType.Integer;
                        this.numMinValue.NumericType = NumericType.Integer;
                        this.numMinValue.Value = 0;
                        this.numMaxValue.Value = 999;

                        this._answerValidator.RegExpPattern = @"^\d{1,6}$"; // + "|(^<[0-9a-fA-F]>$)";
                        this._answerValidator.RegExpText = "-nnn,nnn";
                    }
                    else if(isDecimal)
                    {
                        this.numMaxValue.NumericType = NumericType.Decimal;
                        this.numMinValue.NumericType = NumericType.Decimal;
                        this.numMaxValue.MaskInput = "-nnn,nnn.nnnn";
                        this.numMinValue.MaskInput = "-nnn,nnn.nnnn";
                        this.numMinValue.Value = 0;
                        this.numMaxValue.Value = 99999;

                        this._answerValidator.RegExpPattern = @"^[-+]?\d{0,6}(\.\d{1,4})?$";
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
                    }
                }
                else
                {
                    this.numMaxValue.ReadOnly = true;
                    this.numMinValue.ReadOnly = true;
                    this.cboProcessQuesList.ReadOnly = true;
                    this.cboUnits.ReadOnly = true;
                    // Still allow users to update conditions when inspection is in-use.
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error changing question input type.", exc);
                _log.Fatal(exc, "Error changing question input type.");
            }
        }

        #endregion

        #region Events

        private void cboProcessQuesInputType_ValueChanged(object sender, EventArgs e) { UpdateReadOnlyStatus(); }

        private void cboProcessQuesList_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                using(var editor = new ListEditor())
                {
                    if(editor.ShowDialog(this) == DialogResult.OK)
                    {
                        this.Dataset.EnforceConstraints = false;
                        
                        this._taLists.Fill(Dataset.Lists); //update list values
                        this.cboProcessQuesList.DataBind();

                        //attempt to select last selected list
                        int listID = editor.SelectedListID;

                        if(listID >= 0)
                        {
                            ValueListItem vli = this.cboProcessQuesList.FindItemByValue <int>(v => v == listID);

                            if(vli != null)
                                this.cboProcessQuesList.SelectedItem = vli;
                        }

                        this.Dataset.EnforceConstraints = true;
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

        #endregion

        #region ProcessStepConditionNode

        private class ConditionNode : DataNode<PartInspectionDataSet.PartInspectionQuestionConditionRow>
        {
            #region Fields

            private const string KEY_PREFIX = "CONDITION";
            private static Image _imageCache = null;

            #endregion

            #region Methods

            public ConditionNode(PartInspectionDataSet.PartInspectionQuestionConditionRow cr)
                : base(cr, cr.PartInspectionQuestionConditionID.ToString(), KEY_PREFIX, string.Empty)
            {
                if (_imageCache == null)
                    _imageCache = Properties.Resources.StepOrder;

                LeftImages.Add(_imageCache);

                UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                Text = ConditionEvaluator.ConditionToString(DataRow);
            }

            #endregion
        }

        #endregion

        #region AddConditionCommand

        private class AddConditionCommand : TreeNodeCommandBase
        {
            #region Fields

            private QIQuestion _panel;

            #endregion

            #region Methods

            public AddConditionCommand(UltraButton tool, UltraTree tree, QIQuestion panel)
                : base(tool)
            {
                _panel = panel;
                TreeView = tree;
            }

            public override void OnClick()
            {
                try
                {
                    if (!(_panel.CurrentRecord is PartInspectionDataSet.PartInspectionQuestionRow questionRow))
                    {
                        return;
                    }

                    var dialog = new EditConditionDialog();
                    dialog.LoadNewCondition(_panel.Dataset, questionRow);
                    new WindowInteropHelper(dialog) { Owner = _panel.Handle };

                    if (dialog.ShowDialog() ?? false)
                    {
                        var conditionRow = dialog.ApplyChanges();
                        TreeView.Nodes.Add(new ConditionNode(conditionRow));
                    }
                }
                catch (Exception exc)
                {
                    _log.Error(exc, "Error adding new step condition.");
                }
            }

            public override void Dispose()
            {
                _panel = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region EditConditionCommand

        private class EditConditionCommand : TreeNodeCommandBase
        {
            #region Fields

            private QIQuestion _panel;

            #endregion

            #region Properties

            public override bool Enabled => _node is ConditionNode;

            #endregion

            #region Methods

            public EditConditionCommand(UltraButton tool, UltraTree tree, QIQuestion panel)
                : base(tool)
            {
                _panel = panel ?? throw new ArgumentNullException(nameof(panel));
                TreeView = tree;
                tree.DoubleClick += Tree_DoubleClick;
                Refresh();
            }

            public override void OnClick()
            {
                try
                {
                    if (!(_node is ConditionNode conditionNode) || conditionNode.DataRow == null)
                    {
                        return;
                    }

                    var dialog = new EditConditionDialog();
                    dialog.LoadForExistingCondition(_panel.Dataset, conditionNode.DataRow);
                    new WindowInteropHelper(dialog) { Owner = _panel.Handle };

                    if (dialog.ShowDialog() ?? false)
                    {
                        dialog.ApplyChanges();
                        conditionNode.UpdateNodeUI();
                    }
                }
                catch (Exception exc)
                {
                    _log.Error(exc, "Error adding new step condition.");
                }
            }

            public override void Dispose()
            {
                _panel = null;
                base.Dispose();
            }

            #endregion

            #region Events

            private void Tree_DoubleClick(object sender, EventArgs e)
            {
                OnClick();
            }

            #endregion
        }

        #endregion


        #region DeleteConditionCommand

        private class DeleteConditionCommand : TreeNodeCommandBase
        {
            public override bool Enabled => _node is ConditionNode;

            public DeleteConditionCommand(UltraButton tool, UltraTree tree)
                : base(tool)
            {
                TreeView = tree;
                Refresh();
            }

            public override void OnClick()
            {
                try
                {
                    TreeView.SelectedNode<ConditionNode>()?.Delete();
                }
                catch (Exception exc)
                {
                    _log.Error(exc, "Error adding new step condition.");
                }
            }
        }

        #endregion

        private void grpData_Click(object sender, EventArgs e)
        {

        }
    }
}