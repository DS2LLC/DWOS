using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.PartsDatasetTableAdapters;
using DWOS.Data.Datasets.ProcessesDatasetTableAdapters;
using DWOS.Data.Process;
using DWOS.Shared;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinTree;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class PartProcessAnswers: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private const int _instructionMaxLength = 1000;
        private Control _answerControl;
        private UltraTextEditor _instructionsControl;
        private PartsDataset _dsParts;

        #endregion

        #region Properties

        public PartsDataset PartsDataset
        {
            get { return this._dsParts; }
            set { this._dsParts = value; }
        }

        public PartsDataset.PartProcessRow CurrentPartProcess { get; set; }

        #endregion

        #region Methods

        public PartProcessAnswers()
        {
            this.InitializeComponent();
        }

        public int UnAnsweredQuestions
        {
            get
            {
                if(this.tvwTOC.Nodes.Count > 0)
                {
                    int unAnsweredCount = 0;

                    foreach(PartProcessNode ppNode in this.tvwTOC.Nodes)
                    {
                        foreach(ProcessStepNode psNode in ppNode.Nodes)
                        {
                            foreach(ProcessQuestionNode pqNode in psNode.Nodes)
                            {
                                if(pqNode.IsUnAsweredRequiredQuestion)
                                    unAnsweredCount++;
                            }
                        }
                    }

                    return unAnsweredCount;
                }

                return -1;
            }
        }

        private void LoadData()
        {
            var taS = new ProcessStepsTableAdapter();
            var taQ = new ProcessQuestionTableAdapter();
            var taPA = new PartProcessAnswerTableAdapter();

            //if processes not loaded then load them
            if(this._dsParts.PartProcessAnswer.Select(this._dsParts.PartProcessAnswer.PartProcessIDColumn.ColumnName + " = " + this.CurrentPartProcess.PartProcessID).Length <= 0)
                taPA.FillBy(this._dsParts.PartProcessAnswer, this.CurrentPartProcess.PartProcessID);

            taS.FillBy(this.dsProcesses.ProcessSteps, this.CurrentPartProcess.ProcessID);
            taQ.FillBy(this.dsProcesses.ProcessQuestion, this.CurrentPartProcess.ProcessID);
        }

        private void LoadTOC()
        {
            PartsDataset.PartRow partRow = this._dsParts.Part.FindByPartID(this.CurrentPartProcess.PartID);
            PartsDataset.ProcessRow procRow = this._dsParts.Process.FindByProcessID(this.CurrentPartProcess.ProcessID);
            this.txtPartName.Text = partRow.Name;
            this.txtProcessName.Text = procRow.ProcessName;

            this.tvwTOC.Nodes.Clear();

            UltraTreeNode rootNode = new PartProcessNode(this.CurrentPartProcess, this.dsProcesses, this.CurrentPartProcess);
            rootNode.Text = this.txtProcessName.Text;
            this.tvwTOC.Nodes.Add(rootNode);
            rootNode.ExpandAll();
        }

        private void LoadNode(UltraTreeNode node)
        {
            if(node != null)
            {
                if(node is ProcessQuestionNode)
                {
                    this.grpProcessStepQuestions.Enabled = true;
                    this.grpProcessStepInstructions.Enabled = true;
                    this.LoadNode((ProcessQuestionNode)node);
                }
                else
                {
                    this.grpProcessStepQuestions.Enabled = false;
                    this.grpProcessStepInstructions.Enabled = false;
                }
            }
        }

        private void LoadNode(ProcessQuestionNode node)
        {
            try
            {
                _log.Info("Loading question " + node.Text);

                this.grpProcessStepQuestions.Enabled = true;
                this.grpProcessStepInstructions.Enabled = true;
                this.bsProcessQuestion.Position = this.bsProcessQuestion.Find(this.dsProcesses.ProcessQuestion.ProcessQuestionIDColumn.ColumnName, node.ID);

                //remove existing answer control
                if (this._answerControl != null)
                {
                    if (this._answerControl.Parent != null)
                        this._answerControl.Parent.Controls.Remove(this._answerControl);
                    this._answerControl.Dispose();
                    this._answerControl = null;
                }
                //remove existing instruction control
                if (this._instructionsControl != null)
                {
                    if (this._instructionsControl.Parent != null)
                        this._instructionsControl.Parent.Controls.Remove(this._instructionsControl);
                    this._instructionsControl.Dispose();
                    this._instructionsControl = null;
                }

                //create new control
                var listID = node.DataRow.IsListIDNull() ? 0 : node.DataRow.ListID;
                var processQuestionDefault = !node.DataRow.IsDefaultValueNull()
                    ? node.DataRow.DefaultValue
                    : node.DataRow.MinValue;

                this._answerControl = ControlUtilities.CreateControl(node.DataRow.InputType.ConvertToEnum<InputType>(),
                    listID, node.DataRow.NumericUntis, processQuestionDefault);
                this._instructionsControl = ControlUtilities.CreateControl(InputType.String, listID, null, null, _instructionMaxLength) as UltraTextEditor;

                //set values
                if (this._answerControl != null && this._instructionsControl != null)
                {
                    //if required but operator cant edit then value must be filled in.
                    if (node.DataRow.Required && !node.DataRow.OperatorEditable)
                    {
                        if (this._answerControl is UltraControlBase)
                            ((UltraControlBase)this._answerControl).StyleSetName = "Required";
                    }

                    var par = node.GetAnswer(this.CurrentPartProcess, this._dsParts.PartProcessAnswer);
                    var inputType = (InputType)Enum.Parse(typeof(InputType), node.DataRow.InputType);
                    _log.Info("Loading questions answer '{0}' of type '{1}'.", par.IsAnswerNull() ? "-" : par.Answer, inputType);
                    _log.Info("Loading questions instruction '{0}'.", par.IsInstructionNull() ? "NULL" : par.Instruction);

                    //Set answer values
                    //if no answer set yet, then ensure default answer is valid
                    if (String.IsNullOrWhiteSpace(par.Answer))
                    {
                        if (ControlUtilities.ValidateAnswer(inputType, ProcessUtilities.MinValue(node.DataRow), ProcessUtilities.MaxValue(node.DataRow), listID, processQuestionDefault))
                            par.Answer = processQuestionDefault;
                    }

                    if (inputType == InputType.PartQty)
                    {
                        //cant edit part qty, and dont bind to value
                        _answerControl.Enabled = false;
                    }
                    else
                        //bind row column to control
                        this._answerControl.DataBindings.Add(ControlUtilities.GetBindingProperty(this._answerControl), par, this._dsParts.PartProcessAnswer.AnswerColumn.ColumnName);

                    this.txtInputType.Text = inputType.ToString();
                    this.grpProcessStepQuestions.Controls.Add(this._answerControl);
                    this._answerControl.Location = this.txtAnswer.Location;
                    this._answerControl.Size = this.txtAnswer.Size;
                    this._answerControl.Focus();
                    this.txtAnswer.Visible = false;

                    this._instructionsControl.DataBindings.Add(ControlUtilities.GetBindingProperty(this._instructionsControl), par, this._dsParts.PartProcessAnswer.InstructionColumn.ColumnName);
                    this.grpProcessStepInstructions.Controls.Add(this._instructionsControl);
                    this._instructionsControl.Location = this.txtInstructions.Location;
                    this._instructionsControl.Multiline = true;
                    this._instructionsControl.MaxLength = 1000;
                    this._instructionsControl.Scrollbars = ScrollBars.Vertical;
                    this._instructionsControl.Size = this.txtInstructions.Size;
                    this.txtInstructions.Visible = false;
                }
                else
                {
                    this.txtAnswer.Visible = true;
                    this.txtInstructions.Visible = true;
                }
             
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading answer for process question.");
            }
        }

        private bool ValidateChanges(out bool cancel)
        {
            cancel = false;

            if(this.tvwTOC.SelectedNodes.Count > 0 && !this.tvwTOC.SelectedNodes[0].IsEditing)
            {
                //ensure datasource is updated

                if (this._instructionsControl != null && this._instructionsControl.DataBindings.Count > 0 && this.tvwTOC.SelectedNodes[0] is ProcessQuestionNode && !String.IsNullOrEmpty(this._instructionsControl.Text))
                    this._instructionsControl.DataBindings[0].WriteValue();

                if (this._answerControl != null && this._answerControl.DataBindings.Count > 0 && this.tvwTOC.SelectedNodes[0] is ProcessQuestionNode && !String.IsNullOrEmpty(this._answerControl.Text))
                    this._answerControl.DataBindings[0].WriteValue();

                if (this.tvwTOC.SelectedNodes[0] is IDataNode)
                    ((IDataNode)this.tvwTOC.SelectedNodes[0]).UpdateNodeUI();
            }

            return cancel;
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                bool cancel;
                this.ValidateChanges(out cancel);
                this.tvwTOC.Nodes[0].Select(); //change selection to root node

                if(!cancel)
                    Close();
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error closing form.", exc);
                _log.Fatal(exc, "Error closing form.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PartProcessAnswers_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadData();
                this.LoadTOC();

                this.tvwTOC.Override.SortComparer = new ProcessNodesSorter();

                //select first node customer node
                if(this.tvwTOC.Nodes[0].Nodes.Count > 0)
                    this.tvwTOC.ActiveNode = this.tvwTOC.Nodes[0].Nodes[0];

                this.tvwTOC.PerformAction(UltraTreeAction.SelectActiveNode, false, false);

                this.splitContainer1.Enabled = SecurityManager.Current.IsInRole("PartProcessAnswer.Edit");
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
                _log.Fatal(exc, "Error loading form.");
                this.splitContainer1.Enabled = false;
            }
        }

        private void tvwTOC_AfterSelect(object sender, SelectEventArgs e)
        {
            try
            {
                if(e.NewSelections.Count == 1)
                {
                    LoadNode(e.NewSelections[0]);
                    e.NewSelections[0].Expanded = true;
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error processing selection.", exc);
                _log.Fatal(exc, "Error processing selection.");
            }
        }

        private void tvwTOC_BeforeSelect(object sender, BeforeSelectEventArgs e)
        {
            try
            {
                bool cancel;
                this.ValidateChanges(out cancel);

                e.Cancel = cancel;
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error processing selection.", exc);
                _log.Fatal(exc, "Error processing selection.");
            }
        }

        #endregion

        #region Nodes

        #region Nested type: PartProcessNode

        internal class PartProcessNode: DataNode<PartsDataset.PartProcessRow>
        {
            public const string KEY_PREFIX = "PR";

            #region Methods

            public PartProcessNode(PartsDataset.PartProcessRow cr, ProcessesDataset dsProcesses, PartsDataset.PartProcessRow partProcessRow)
                : base(cr, cr.PartProcessID.ToString(), KEY_PREFIX, cr.ProcessID.ToString())
            {
                //add each contact to as child node
                var rows = dsProcesses.ProcessSteps.Select(dsProcesses.ProcessSteps.ProcessIDColumn.ColumnName + " = " + cr.ProcessID) as ProcessesDataset.ProcessStepsRow[];

                if(rows != null && rows.Length > 0)
                {
                    foreach(ProcessesDataset.ProcessStepsRow row in rows)
                        Nodes.Add(new ProcessStepNode(row, partProcessRow));
                }

                LeftImages.Add(Properties.Resources.Process_16);
            }

            #endregion
        }

        #endregion

        #region Nested type: ProcessNodesSorter

        internal class ProcessNodesSorter: IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                if(x is ProcessStepNode && y is ProcessStepNode)
                    return ((ProcessStepNode)x).DataRow.StepOrder.CompareTo(((ProcessStepNode)y).DataRow.StepOrder);
                else if(x is ProcessQuestionNode && y is ProcessQuestionNode)
                    return ((ProcessQuestionNode)x).DataRow.StepOrder.CompareTo(((ProcessQuestionNode)y).DataRow.StepOrder);
                else
                    return x.ToString().CompareTo(y.ToString());
            }

            #endregion
        }

        #endregion

        #region Nested type: ProcessQuestionNode

        internal class ProcessQuestionNode: DataNode<ProcessesDataset.ProcessQuestionRow>
        {
            public const string KEY_PREFIX = "PQ";
            private readonly PartsDataset.PartProcessRow _partProcessRow;
            private PartsDataset.PartProcessAnswerRow _paRow;

            #region Properties

            public bool IsUnAsweredRequiredQuestion
            {
                get { return Override.NodeAppearance.ForeColor == Color.Red; }
            }

            #endregion

            #region Methods

            public ProcessQuestionNode(ProcessesDataset.ProcessQuestionRow cr, PartsDataset.PartProcessRow partProcessRow)
                : base(cr, cr.ProcessQuestionID.ToString(), KEY_PREFIX, cr.StepOrder + " - " + cr.Name)
            {
                this._partProcessRow = partProcessRow;
                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                Text = base.DataRow.StepOrder.ToString("F1") + " - " + base.DataRow.Name + (this._paRow == null ? "" : " - " + this._paRow.Answer);

                Override.NodeAppearance.FontData.Italic = this.FindAnswerRow() ? DefaultableBoolean.False : DefaultableBoolean.True;

                //Forecolor used to track answer state (see: this.IsUnAsweredRequiredQuestion)
                if(base.DataRow.Required && !base.DataRow.OperatorEditable)
                    Override.NodeAppearance.ForeColor = (String.IsNullOrEmpty(base.DataRow.DefaultValue) && (this._paRow == null || String.IsNullOrEmpty(this._paRow.Answer)) ? Color.Red : Color.Green);
                else
                    Override.NodeAppearance.ForeColor = Color.Black;
            }

            private bool FindAnswerRow()
            {
                if(this._partProcessRow != null)
                {
                    //PartsDataset.PartProcessAnswerRow[] pars = 
                    var pars = this._partProcessRow.Table.ChildRelations[0].ChildTable.Rows;
                    //PartsDataset.PartProcessAnswerDataTable pars;
                    //using (var taPartProcessAnswer = new PartProcessAnswerTableAdapter())
                    //{
                    //    pars = taPartProcessAnswer.GetByPartProcessId(this._partProcessRow.PartProcessID);
                    //}
                    foreach (PartsDataset.PartProcessAnswerRow par in pars)
                    {
                        if(par.ProcessQuestionID == DataRow.ProcessQuestionID)
                        {
                            this._paRow = par;
                            return true;
                        }
                    }
                }

                return false;
            }

            public PartsDataset.PartProcessAnswerRow GetAnswer(PartsDataset.PartProcessRow partProcessRow, PartsDataset.PartProcessAnswerDataTable processAnswerDataTable)
            {
                if(this._paRow == null)
                {
                    this.FindAnswerRow();
                    if(this._paRow == null)
                    {
                        //if cant find the row then create it
                        this._paRow = processAnswerDataTable.NewPartProcessAnswerRow();
                        this._paRow.ProcessQuestionID = DataRow.ProcessQuestionID;
                        this._paRow.PartProcessID = partProcessRow.PartProcessID;
                        this._paRow.Instruction = GetInstructionsFromProcessQuestion(DataRow.ProcessQuestionID);

                        processAnswerDataTable.AddPartProcessAnswerRow(this._paRow);
                    }
                }

                return this._paRow;
            }

            private string GetInstructionsFromProcessQuestion(int processQuestionId)
            {
                using (var taProcessQuestion = new ProcessQuestionTableAdapter())
                {
                    var processQuestionRec = taProcessQuestion.GetByProcessQuestionId(processQuestionId);
                    return processQuestionRec.Count > 0 ? processQuestionRec[0].Notes : string.Empty;
                }  
            }

            #endregion
        }

        #endregion

        #region Nested type: ProcessStepNode

        internal class ProcessStepNode: DataNode<ProcessesDataset.ProcessStepsRow>
        {
            public const string KEY_PREFIX = "PS";

            #region Methods

            public ProcessStepNode(ProcessesDataset.ProcessStepsRow cr, PartsDataset.PartProcessRow partProcessRow)
                : base(cr, cr.ProcessStepID.ToString(), KEY_PREFIX, cr.StepOrder + " - " + cr.Name)
            {
                //add each contact to as child node
                ProcessesDataset.ProcessQuestionRow[] rows = cr.GetProcessQuestionRows();

                if(rows != null && rows.Length > 0)
                {
                    foreach(ProcessesDataset.ProcessQuestionRow row in rows)
                        Nodes.Add(new ProcessQuestionNode(row, partProcessRow));
                }

                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                Text = base.DataRow.StepOrder.ToString("F1") + " - " + base.DataRow.Name;
            }

            #endregion
        }

        #endregion

        #endregion
    }
}