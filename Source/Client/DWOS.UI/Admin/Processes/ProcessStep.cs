using DWOS.Data.Conditionals;
using DWOS.Data.Datasets;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using DWOS.Utilities.Validation;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinTree;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DWOS.UI.Admin.Processes
{
    public partial class ProcessStep: DataPanel
    {
        #region Fields

        private CommandManager _commandManager;

        #endregion

        #region Properties

        public ProcessesDataset Dataset
        {
            get { return base._dataset as ProcessesDataset; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return this.Dataset.ProcessSteps.ProcessStepIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public ProcessStep()
        {
            this.InitializeComponent();
        }

        public void LoadData(ProcessesDataset dataset)
        {
            this.Dataset = dataset;
            bsData.DataSource = this.Dataset;
            bsData.DataMember = this.Dataset.ProcessSteps.TableName;

            //bind column to control
            base.BindValue(this.txtName, this.Dataset.ProcessSteps.NameColumn.ColumnName, true);
            base.BindValue(this.txtDescription, this.Dataset.ProcessSteps.DescriptionColumn.ColumnName);
            base.BindValue(this.numOrder, this.Dataset.ProcessSteps.StepOrderColumn.ColumnName, true);
            base.BindValue(this.chkCOCData, this.Dataset.ProcessSteps.COCDataColumn.ColumnName);

            docLinkManagerSteps.InitializeData(Documents.LinkType.ProcessSteps,
                this.Dataset.ProcessSteps,
                this.Dataset.ProcessStepDocumentLink);

            LoadCommands();

            base._panelLoaded = true;
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtName, "Process Step name required."){DefaultValue = "New Process Step"}, errProvider));
        }

        private void LoadCommands()
        {
            _commandManager = new CommandManager();
            _commandManager.AddCommand("DeleteConditionCommand", new DeleteConditionCommand(btnDeleteCondition, tvwConditions));
            _commandManager.AddCommand("AddConditionCommand", new AddConditionCommand(btnAddCondition, tvwConditions, this));
            _commandManager.AddCommand("EditConditionCommand", new EditConditionCommand(btnEditCondition, tvwConditions));
        }

        public ProcessesDataset.ProcessStepsRow AddProcessStepRow(int processID, decimal stepID)
        {
            var rowVw    = bsData.AddNew() as DataRowView;
            var cr       = rowVw.Row as ProcessesDataset.ProcessStepsRow;
            cr.Name      = "New Process Step";
            cr.ProcessID = processID;
            cr.StepOrder = stepID;

            return cr;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            docLinkManagerSteps.ClearLinks();
            ClearStepConditions();

            var processStep = this.CurrentRecord as ProcessesDataset.ProcessStepsRow;

            if (processStep!= null)
            {
                docLinkManagerSteps.LoadLinks(processStep, processStep.GetProcessStepDocumentLinkRows());
                LoadStepConditions();
            }
        }

        private void ClearStepConditions()
        {
            tvwConditions.Nodes.Clear();
        }

        private void LoadStepConditions()
        {
            var processStep = this.CurrentRecord as ProcessesDataset.ProcessStepsRow;

            if(processStep != null)
            {
                foreach(var psc in processStep.GetProcessStepConditionRows())
                {
                    tvwConditions.Nodes.Add(new ProcessStepConditionNode(psc));
                }
            }
        }

        private void DisposeMe()
        {
            if(_commandManager != null)
                _commandManager.Dispose();

            _commandManager = null;
        }

        #endregion

        #region Events

        private void pnlOrder_MouseHover(object sender, EventArgs e)
        {
            try
            {
                // Show tooltip for disabled control
                if (numOrder.Enabled)
                {
                    return;
                }

                tipManager.ShowToolTip(numOrder);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error showing order tooltip.");
            }
        }

        private void pnlName_MouseHover(object sender, EventArgs e)
        {
            try
            {
                // Show tooltip for disabled control
                if (txtName.Enabled)
                {
                    return;
                }

                tipManager.ShowToolTip(txtName);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error showing name tooltip.");
            }
        }

        private void pnlOrder_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                // Hide tooltip for disabled control
                if (numOrder.Enabled || !tipManager.IsToolTipVisible(numOrder))
                {
                    return;
                }

                tipManager.HideToolTip();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error hiding order tooltip.");
            }
        }

        private void pnlName_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                // Hide tooltip for disabled control
                if (txtName.Enabled || !tipManager.IsToolTipVisible(txtName))
                {
                    return;
                }

                tipManager.HideToolTip();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error hiding name tooltip.");
            }
        }

        private void pnlEditCondition_MouseHover(object sender, EventArgs e)
        {
            try
            {
                // Show tooltip for disabled control
                if (btnEditCondition.Enabled)
                {
                    return;
                }

                tipManager.ShowToolTip(btnEditCondition);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error showing edit condition tooltip.");
            }
        }

        private void pnlDeleteCondition_MouseHover(object sender, EventArgs e)
        {
            try
            {
                // Show tooltip for disabled control
                if (btnDeleteCondition.Enabled)
                {
                    return;
                }

                tipManager.ShowToolTip(btnDeleteCondition);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error showing delete condition tooltip.");
            }
        }

        private void pnlEditCondition_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                // Hide tooltip for disabled control
                if (btnEditCondition.Enabled || !tipManager.IsToolTipVisible(btnEditCondition))
                {
                    return;
                }

                tipManager.HideToolTip();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error hiding edit condition tooltip.");
            }
        }

        private void pnlDeleteCondition_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                // Hide tooltip for disabled control
                if (btnDeleteCondition.Enabled || !tipManager.IsToolTipVisible(btnDeleteCondition))
                {
                    return;
                }

                tipManager.HideToolTip();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error hiding delete condition tooltip.");
            }
        }

        #endregion

        #region ProcessStepConditionNode

        private class ProcessStepConditionNode : DataNode<ProcessesDataset.ProcessStepConditionRow>
        {
            #region Fields

            private const string KEY_PREFIX = "PSC";
            private static Image _imageCache = null;

            #endregion

            #region Methods

            public ProcessStepConditionNode(ProcessesDataset.ProcessStepConditionRow cr)
                : base(cr, cr.ProcessStepConditionId.ToString(), KEY_PREFIX, "")
            {
                if (_imageCache == null)
                    _imageCache = Properties.Resources.StepOrder;

                LeftImages.Add(_imageCache);
                
                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                Text = ConditionEvaluator.ConditionToString(DataRow);
            }
            
            #endregion
        }

        #endregion

        #region DeleteConditionCommand

        private class DeleteConditionCommand: TreeNodeCommandBase
        {
            public override bool Enabled
            {
                get
                {
                    return _node is ProcessStepConditionNode;
                }
            }

            public DeleteConditionCommand(UltraButton tool, UltraTree tree)
                : base(tool)
            {
                this.TreeView = tree;
                Refresh();
            }

            public override void OnClick()
            {
                try
                {
                    var conditionNode = TreeView.SelectedNode<ProcessStepConditionNode>();

                    if (conditionNode != null)
                        conditionNode.Delete();
                }
                catch (Exception exc)
                {
                    NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error adding new step condition.");
                }
            }
        }

        #endregion
        
        #region AddConditionCommand

        private class AddConditionCommand : TreeNodeCommandBase
        {
            private ProcessStep _panel;

            public AddConditionCommand(UltraButton tool, UltraTree tree, ProcessStep panel)
                : base(tool)
            {
                _panel = panel;
                this.TreeView = tree;
            }

            public override void OnClick()
            {
                try
                {
                    var processStep = _panel.CurrentRecord as ProcessesDataset.ProcessStepsRow;

                    var condition = _panel.Dataset.ProcessStepCondition.NewProcessStepConditionRow();
                    condition.ProcessStepId = processStep.ProcessStepID;
                    condition.InputType = ConditionInputType.ProcessQuestion.ToString();
                    condition.Operator = EqualityOperator.Equal.ToString();
                    condition.Value = string.Empty;
                    condition.StepOrder = 99;
                    _panel.Dataset.ProcessStepCondition.AddProcessStepConditionRow(condition);

                    using (var frm = new StepConditionEditor())
                    {
                        frm.LoadData(condition);
                        if (frm.ShowDialog(_panel) == DialogResult.OK)
                        {
                            condition.EndEdit();
                            TreeView.Nodes.Add(new ProcessStepConditionNode(condition));
                        }
                        else
                            condition.Delete();
                    }
                }
                catch (Exception exc)
                {
                    NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error adding new step condition.");
                }
            }

            public override void Dispose()
            {
                _panel = null;
                base.Dispose();
            }
        }

        #endregion

        #region EditConditionCommand
        
        private class EditConditionCommand : TreeNodeCommandBase
        {
            public override bool Enabled
            {
                get
                {
                    return _node is ProcessStepConditionNode;
                }
            }

            public EditConditionCommand(UltraButton tool, UltraTree tree)
                : base(tool)
            {
                this.TreeView = tree;
                tree.DoubleClick += tree_DoubleClick;
                Refresh();
            }

            public override void OnClick()
            {
                try
                {
                    var conditionNode = TreeView.SelectedNode<ProcessStepConditionNode>();

                    if (conditionNode != null)
                    {
                        using (var frm = new StepConditionEditor())
                        {
                            frm.LoadData(conditionNode.DataRow);
                            frm.ShowDialog(Form.ActiveForm);
                            conditionNode.UpdateNodeUI();
                        }
                    }
                }
                catch (Exception exc)
                {
                    NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error editing step condition.");
                }
            }

            private void tree_DoubleClick(object sender, EventArgs e) { OnClick(); }
        }

        #endregion

    }
}