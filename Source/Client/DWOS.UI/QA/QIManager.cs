using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Shared;
using DWOS.UI.Properties;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using System.Drawing;
using NLog;
using DWOS.Data;

namespace DWOS.UI.QA
{
    public partial class QIManager : DataEditorBase
    {
        #region Fields

        private bool _showInactiveInspections;

        #endregion

        #region Methods

        public QIManager() { InitializeComponent(); }

        private void LoadData()
        {
            try
            {
                this.dsPartInspection.EnforceConstraints = false;
                this.dsPartInspection.PartInspectionType.BeginLoadData();
                this.dsPartInspection.PartInspectionQuestion.BeginLoadData();

                this.taLists.Fill(this.dsPartInspection.Lists);
                this.taListValues.Fill(this.dsPartInspection.ListValues);
                this.taInputType.FillForInspections(this.dsPartInspection.d_InputType);
                this.taNumericUnits.Fill(this.dsPartInspection.NumericUnits);

                this.taInspectionType.Fill(this.dsPartInspection.PartInspectionType);
                this.taPartInspectionQuestion.Fill(this.dsPartInspection.PartInspectionQuestion);
                this.taPartInspectionQuestionCondition.Fill(dsPartInspection.PartInspectionQuestionCondition);
                this.taInspectionTypeDocumentLink.Fill(this.dsPartInspection.PartInspectionTypeDocumentLink);

                this.taRevision.Fill(this.dsPartInspection.PartInspectionRevision);

                this.dsPartInspection.PartInspectionType.EndLoadData();
                this.dsPartInspection.PartInspectionQuestion.EndLoadData();
                this.dsPartInspection.EnforceConstraints = true;

                this.pnlInfo.LoadData(this.dsPartInspection);
                base.AddDataPanel(this.pnlInfo);

                this.pnlQuestion.LoadData(this.dsPartInspection, this.taLists);
                base.AddDataPanel(this.pnlQuestion);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error loading QI Manager data.\r\n" + this.dsPartInspection.GetDataErrors());
            }
        }

        private void LoadTOC()
        {
            tvwTOC.Nodes.Clear();

            var rootNode = new InspectionsRootNode(this.dsPartInspection);
            tvwTOC.Nodes.Add(rootNode);
            rootNode.Expanded = true;

            foreach(PartInspectionDataSet.PartInspectionTypeRow cr in this.dsPartInspection.PartInspectionType)
                rootNode.Nodes.Add(new InspectionNode(cr) { Visible = _showInactiveInspections || cr.Active});
        }

        protected override void ReloadTOC() { LoadTOC(); }

        private void LoadCommands()
        {
            if(SecurityManager.Current.IsInRole("QIManager.Edit"))
            {
                var add = base.Commands.AddCommand("Add", new AddInspectionCommand(toolbarManager.Tools["Add"], tvwTOC)) as AddInspectionCommand;
                add.AddUser = AddNode;

                var addQuestion = base.Commands.AddCommand("AddQuestion", new AddInspectionQuestionCommand(toolbarManager.Tools["AddQuestion"], tvwTOC)) as AddInspectionQuestionCommand;
                addQuestion.AddUser = AddQuestionNode;

                base.Commands.AddCommand("Delete", new DeleteCommand(toolbarManager.Tools["Delete"], tvwTOC, this));
                base.Commands.AddCommand("Copy", new CopyCommand(toolbarManager.Tools["Copy"], tvwTOC));
                base.Commands.AddCommand("Paste", new PasteCommand(toolbarManager.Tools["Paste"], tvwTOC));
                base.Commands.AddCommand("CreateRevision", new CreateRevisionCommand(toolbarManager.Tools["CreateRevision"], tvwTOC));
            }

            toolbarManager.Tools["DisplayInactive"].ToolClick += ProcessManager_DisplayInactive;
        }

        protected override bool SaveData()
        {
            try
            {
                base.EndAllEdits();
                SaveNewRevisedInspections();

                this.taManager.UpdateAll(this.dsPartInspection);

                return true;
            }
            catch(Exception exc)
            {
                _log.Warn("DataSet Errors: " + dsPartInspection.GetDataErrors());

                ErrorMessageBox.ShowDialog("Error saving data.", exc);
                return false;
            }
        }

        /// <summary>
        /// Saves all new inspections that have revisions.
        /// </summary>
        /// <remarks>
        /// This method is a workaround for VSTS #19287.
        /// </remarks>
        private void SaveNewRevisedInspections()
        {
            var newRevisedInspections = dsPartInspection.PartInspectionType
                .Where(p => p.RowState == DataRowState.Added && !p.IsParentIDNull() && p.ParentID < 0);

            var savedInspectionMap = new Dictionary<int, int>();
            foreach (var newInspection in newRevisedInspections)
            {
                if (savedInspectionMap.ContainsKey(newInspection.ParentID))
                {
                    // Refers to an inspection saved by this method
                    newInspection.ParentID = savedInspectionMap[newInspection.ParentID];
                    continue;
                }

                // Get other unsaved inspections in hierarchy
                var rowsToSave = new Stack<PartInspectionDataSet.PartInspectionTypeRow>(); // save deepest inspection first
                var currentRow = dsPartInspection.PartInspectionType.FindByPartInspectionTypeID(newInspection.ParentID);

                while (currentRow != null)
                {
                    rowsToSave.Push(currentRow);
                    if (currentRow.IsParentIDNull() || currentRow.ParentID > 0)
                    {
                        // Original/non-revised inspection
                        currentRow = null;
                    }
                    else if (savedInspectionMap.ContainsKey(currentRow.ParentID))
                    {
                        // Reached a previously saved inspection
                        currentRow.ParentID = savedInspectionMap[currentRow.ParentID];
                        currentRow = null;
                    }
                    else
                    {
                        // Parent is unsaved.
                        currentRow = dsPartInspection.PartInspectionType.FindByPartInspectionTypeID(currentRow.ParentID);
                    }
                }

                // Save all parent processes
                while (rowsToSave.Count > 0)
                {
                    var rowToSave = rowsToSave.Pop();

                    var originalId = rowToSave.PartInspectionTypeID;
                    taInspectionType.Update(rowToSave);

                    _log.Info("Successfully saved new parent part inspection type {0} ({1})",
                        rowToSave.Name, rowToSave.PartInspectionTypeID);

                    if (rowsToSave.Count == 0)
                    {
                        // found direct parent to the current process
                        newInspection.ParentID = rowToSave.PartInspectionTypeID;
                    }
                    else
                    {
                        rowsToSave.Peek().ParentID = rowToSave.PartInspectionTypeID;
                    }

                    savedInspectionMap[originalId] = rowToSave.PartInspectionTypeID;
                }
            }
        }

        private void AddInspection()
        {
            //create new data source
            PartInspectionDataSet.PartInspectionTypeRow cr = this.pnlInfo.AddRow();

            //create new ui nodes
            var cn = new InspectionNode(cr);
            tvwTOC.Nodes[0].Nodes.Add(cn);
            cn.Select();
        }

        private void AddInspectionQuestion(InspectionNode pn)
        {
            _log.Info("Adding a inspection question: " + pn.Text + " - " + pn.Key);

            //create new data source
            PartInspectionDataSet.PartInspectionQuestionRow cr = this.pnlQuestion.AddQuestionRow(pn.DataRow.PartInspectionTypeID, pn.GetNextStepOrder());

            //create new ui nodes
            var cn = new InspectionQuestionNode(cr);
            pn.Nodes.Add(cn);
            cn.Select();
        }

        protected override void LoadNode(UltraTreeNode node)
        {
            if(node is InspectionNode)
            {
                InitializeUsageCount(node as InspectionNode);
                bool moved = this.pnlInfo.MoveToRecord(((InspectionNode) node).ID);
                DisplayPanel(this.pnlInfo);
                this.pnlInfo.Editable = ((InspectionNode) node).UsageCount < 1;
                this.pnlInfo.Enabled = moved;
            }
            else if(node is InspectionQuestionNode)
            {
                InitializeUsageCount(node.Parent as InspectionNode);
                this.pnlQuestion.MoveToRecord(((InspectionQuestionNode) node).ID);
                this.pnlQuestion.Editable = ((InspectionNode)node.Parent).UsageCount < 1;
                DisplayPanel(this.pnlQuestion);
            }
            else
                DisplayPanel(null);
        }

        /// <summary>
        /// Sets usage count for node unless it is already done.
        /// </summary>
        /// <param name="node"></param>
        private void InitializeUsageCount(InspectionNode node)
        {
            if (node != null && node.UsageCount < 0)
            {
                object usageCount = this.taInspectionType.UsageCount(node.DataRow.PartInspectionTypeID);
                (node).UsageCount = usageCount == null || usageCount == DBNull.Value ? 0 : Convert.ToInt32(usageCount);
            }
        }

        protected override void SaveSelectedNode()
        {
            if(tvwTOC.SelectedNodes.Count > 0)
                Settings.Default.LastSelectedUser = tvwTOC.SelectedNodes[0].Key;
        }

        #endregion

        #region Events

        private void QIManager_Load(object sender, EventArgs e)
        {
            _loadingData = true; //prevent TOC from loading

            try
            {
                using(new UsingWaitCursor(this))
                {
                    LoadCommands();
                    LoadData();
                    LoadTOC();
                    base.LoadValidators();

                    tvwTOC.Override.SortComparer = new QuestionNodeSorter();
                    tvwTOC.Override.Sort = SortType.Ascending;

                    _loadingData = false;

                    //select first node customer node
                    base.RestoreLastSelectedNode(Settings.Default.LastSelectedCustomer);

                    splitContainer1.Panel2.Enabled = SecurityManager.Current.IsInRole("QIManager.Edit");
                }
            }
            catch(Exception exc)
            {
                splitContainer1.Enabled = false;
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
                _log.Fatal(exc, "Error loading form.");
            }
        }

        private void AddNode(object sender, EventArgs e)
        {
            UltraTreeNode selectedNode = tvwTOC.SelectedNodes[0];

            if(IsValidControls())
            {
                _validators.Enabled = false;

                if(selectedNode is InspectionsRootNode)
                    AddInspection();

                _validators.Enabled = true;
            }
        }

        private void AddQuestionNode(object sender, EventArgs e)
        {
            UltraTreeNode selectedNode = tvwTOC.SelectedNodes[0];

            if(IsValidControls())
            {
                _validators.Enabled = false;

                if(selectedNode is InspectionNode)
                    AddInspectionQuestion((InspectionNode) selectedNode);

                _validators.Enabled = true;
            }
        }

        private void ProcessManager_DisplayInactive(object sender, ToolClickEventArgs e)
        {
            try
            {
                _showInactiveInspections = ((StateButtonTool)e.Tool).Checked;
                ReloadTOC();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error displaying/hiding inactive inspections.");
            }
        }

        #endregion

        #region Nodes

        #region Nested type: InspectionNode

        internal class InspectionNode : DataNode <PartInspectionDataSet.PartInspectionTypeRow>
        {
            #region Fields

            public const string KEY_PREFIX = "US";
            public int UsageCount = -1;

            #endregion

            #region Properties

            public int RevisionCount =>
                DataRow.GetPartInspectionTypeRows().Length;

            #endregion

            #region Methods

            public InspectionNode(PartInspectionDataSet.PartInspectionTypeRow cr) : base(cr, cr.PartInspectionTypeID.ToString(), KEY_PREFIX, cr.Name)
            {
                //update UI
                LeftImages.Add(Properties.Resources.Inspection_16);

                //add each contact to as child node
                PartInspectionDataSet.PartInspectionQuestionRow[] rows = cr.GetPartInspectionQuestionRows();

                if(rows != null && rows.Length > 0)
                {
                    foreach(PartInspectionDataSet.PartInspectionQuestionRow row in rows)
                        Nodes.Add(new InspectionQuestionNode(row));
                }

                UpdateNodeUI();
            }

            public override bool CanDelete =>
                UsageCount < 1 && RevisionCount == 0;

            public override string ClipboardDataFormat
            {
                get { return GetType().FullName; }
            }

            public override void UpdateNodeUI()
            {
                Text = DataRow.IsRevisionNull() ? DataRow.Name : $"{DataRow.Name} - {DataRow.Revision}";

                if (DataRow.Active)
                {
                        Override.NodeAppearance.FontData.Italic = DefaultableBoolean.False;
                        Override.NodeAppearance.FontData.Bold = DefaultableBoolean.False;
                        Override.NodeAppearance.ForeColor = Color.Green;
                }
                else
                {
                        Override.NodeAppearance.FontData.Italic = DefaultableBoolean.True;
                        Override.NodeAppearance.FontData.Bold = DefaultableBoolean.False;
                        Override.NodeAppearance.ForeColor = Color.Salmon;
                }
            }

            public decimal GetNextStepOrder()
            {
                PartInspectionDataSet.PartInspectionQuestionRow[] rows = base.DataRow.GetPartInspectionQuestionRows();
                decimal maxCount = 0;

                foreach(PartInspectionDataSet.PartInspectionQuestionRow row in rows)
                {
                    if(row.StepOrder > maxCount)
                        maxCount = Convert.ToInt32(Math.Floor(row.StepOrder));
                }

                return maxCount + 1;
            }

            public override UltraTreeNode PasteData(string format, DataRowProxy proxy)
            {
                if (proxy == null)
                {
                    return null;
                }

                if (DataRow == null)
                {
                    LogManager.GetCurrentClassLogger().Warn("Node was disposed - skipping paste");
                    return null;
                }

                if (format == typeof(InspectionQuestionNode).FullName)
                {
                    DataRow dr = DataNode <DataRow>.AddPastedDataRows(proxy, base.DataRow.Table.DataSet.Tables["PartInspectionQuestion"]);
                    var psr = dr as PartInspectionDataSet.PartInspectionQuestionRow;

                    if(psr != null)
                    {
                        psr.SetParentRow(base.DataRow);
                        var node = new InspectionQuestionNode(psr);
                        base.Nodes.Add(node);

                        return node;
                    }
                }

                return null;
            }

            public override bool CanPasteData(string format) { return format == typeof(InspectionQuestionNode).FullName; }

            #endregion
        }

        #endregion

        #region Nested type: InspectionQuestionNode

        internal class InspectionQuestionNode : DataNode <PartInspectionDataSet.PartInspectionQuestionRow>
        {
            public const string KEY_PREFIX = "PQ";

            #region Methods

            public InspectionQuestionNode(PartInspectionDataSet.PartInspectionQuestionRow cr) : base(cr, cr.PartInspectionQuestionID.ToString(), KEY_PREFIX, cr.StepOrder + " - " + cr.Name) { UpdateNodeUI(); }

            public override bool CanDelete
            {
                get
                {
                    bool canDelete;
                    var inspectionNode = base.Parent as InspectionNode;

                    if (inspectionNode != null)
                    {
                        canDelete = inspectionNode.CanDelete;
                    }
                    else
                    {
                        canDelete = base.CanDelete;
                    }

                    return canDelete;
                }
            }

            public override void UpdateNodeUI()
            {
                bool isSub = base.DataRow.StepOrder != Convert.ToInt32(base.DataRow.StepOrder);
                Text = (isSub ? "  " : "") + base.DataRow.StepOrder.ToString("F1") + " - " + base.DataRow.Name + (base.DataRow.IsDefaultValueNull() ? "" : " - " + base.DataRow.DefaultValue);
            }

            #endregion

            public override string ClipboardDataFormat
            {
                get { return GetType().FullName; }
            }
        }

        #endregion

        #region Nested type: InspectionsRootNode

        internal class InspectionsRootNode : UltraTreeNode, ICopyPasteNode
        {
            #region Fields

            private PartInspectionDataSet _dataset;

            #endregion

            #region Methods

            public InspectionsRootNode(PartInspectionDataSet dataset) : base("ROOT", "Inspections")
            {
                this._dataset = dataset;
                LeftImages.Add(Properties.Resources.Inspection_16);
            }

            protected override void OnDispose()
            {
                this._dataset = null;
                base.OnDispose();
            }

            #endregion

            #region ICopyPasteNode Members

            public UltraTreeNode PasteData(string format, DataRowProxy proxy)
            {
                if (proxy == null)
                {
                    return null;
                }

                if (_dataset == null)
                {
                    LogManager.GetCurrentClassLogger().Warn("Node was disposed - skipping paste");
                    return null;
                }

                // Do not copy inspection that has been deleted.
                int originalInspectionTypeID = int.Parse(proxy.OriginalPrimaryKey);
                var hasOriginalRow = _dataset.PartInspectionType.Any(row => row.IsValidState() && row.PartInspectionTypeID == originalInspectionTypeID);

                if (!hasOriginalRow)
                {
                    return null;
                }

                DataRow dr = DataNode <DataRow>.AddPastedDataRows(proxy, this._dataset.PartInspectionType);
                var node = new InspectionNode(dr as PartInspectionDataSet.PartInspectionTypeRow);
                base.Nodes.Add(node);
                return node;
            }

            public bool CanPasteData(string format) { return format == typeof(InspectionNode).FullName; }

            public string ClipboardDataFormat
            {
                get { return null; }
            }

            #endregion
        }

        #endregion

        #endregion

        #region Commands

        #region Nested type: AddInspectionCommand

        internal class AddInspectionCommand : TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddUser;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _node is InspectionsRootNode && this.AddUser != null; }
            }

            #endregion

            #region Methods

            public AddInspectionCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            public override void OnClick()
            {
                if(_node is InspectionsRootNode)
                {
                    if(this.AddUser != null)
                        this.AddUser(this, EventArgs.Empty);
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: AddInspectionQuestionCommand

        internal class AddInspectionQuestionCommand : TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddUser;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get
                {
                    bool enabled = false;
                    var inspectionNode = _node as InspectionNode;

                    if (inspectionNode != null && this.AddUser != null)
                    {
                        enabled = inspectionNode.UsageCount < 1;
                    }

                    return enabled;
                }
            }

            #endregion

            #region Methods

            public AddInspectionQuestionCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            public override void OnClick()
            {
                if(_node is InspectionNode)
                {
                    if(this.AddUser != null)
                        this.AddUser(this, EventArgs.Empty);
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: CreateRevisionCommand

        internal class CreateRevisionCommand : TreeNodeCommandBase
        {
            #region Properties

            public override bool Enabled =>
                _node is InspectionNode;

            #endregion

            #region Methods

            public CreateRevisionCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            public override void OnClick()
            {
                if (!Enabled || !(_node is InspectionNode currentNode))
                {
                    return;
                }

                var currentInspection = currentNode.DataRow;

                //test to see if we can change selection
                var parentNode = base._node.Parent;
                parentNode.Select();

                if (TreeView.SelectedNodes.Count == 1 && TreeView.SelectedNodes[0] == parentNode)
                {
                    OnAfterSelect(currentNode);
                    _log.Info("Revising part inspection.");

                    CopyCommand.CopyNode(currentNode);

                    if (PasteCommand.PasteNode(TreeView.Nodes[0] as ICopyPasteNode) is InspectionNode pn)
                    {
                        _log.Info("Pasted: " + pn.DataRow.ToDetailedString());

                        //set old inspection as inactive
                        currentInspection.Active = false;

                        //create new revision and set new inspection as active
                        pn.DataRow.Revision = currentInspection.IsRevisionNull()
                            ? "<None>"
                            : currentInspection.Revision.Increment();

                        pn.DataRow.Active = true;
                        pn.DataRow.ParentID = currentInspection.PartInspectionTypeID;

                        //select old inspection then re-select to force refresh of UI
                        currentNode.Select();
                        pn.Select();
                    }
                }
            }

            #endregion
        }

        #endregion

        #endregion

        #region Nested type: QuestionNodeSorter

        internal class QuestionNodeSorter : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                if(x is InspectionQuestionNode && y is InspectionQuestionNode)
                    return ((InspectionQuestionNode) x).DataRow.StepOrder.CompareTo(((InspectionQuestionNode) y).DataRow.StepOrder);
                return ((UltraTreeNode) x).Text.CompareTo(((UltraTreeNode) y).Text);
            }

            #endregion
        }

        #endregion
    }
}