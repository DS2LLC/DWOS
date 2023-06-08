using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using Newtonsoft.Json;
using NLog;
using DWOS.Data.Process;
using DWOS.UI.Admin.Processes;
using System.Windows.Interop;
using DWOS.Data;

namespace DWOS.UI.Admin
{
    public partial class ProcessManager: DataEditorBase
    {
        #region Fields

        private bool _loadedInActiveProcesses;
        private bool _showInActiveProcesses;

        #region Nested type: NodeEventHandler

        internal delegate void NodeEventHandler(enumNodeType nodeType);

        #endregion

        #region Nested type: enumNodeType

        internal enum enumNodeType
        {
            ProcessNode,
            ProcessStepNode,
            ProcessStepQuestionNode
        }

        #endregion

        #endregion

        #region Methods

        public ProcessManager()
        {
            this.InitializeComponent();
        }

        private void LoadData(bool initialLoad)
        {
            if(initialLoad)
            {
                this.dsProcesses.EnforceConstraints = false;

                this.taInputType.Fill(this.dsProcesses.d_InputType);
                this.taLists.Fill(this.dsProcesses.Lists);
                this.taCustomFieldToken.Fill(this.dsProcesses.CustomFieldToken);
                this.taRevisions.Fill(this.dsProcesses.Revisions);
                this.taNumericUnits.Fill(this.dsProcesses.NumericUnits);
                this.taInspectionType.Fill(this.dsProcesses.PartInspectionType);
                this.taDepartment.Fill(this.dsProcesses.d_Department);
                this.taProcessCategory.Fill(this.dsProcesses.d_ProcessCategory);

                using(new UsingDataTableLoad(this.dsProcesses.Customer))
                {
                    this.taCustomer.Fill(this.dsProcesses.Customer);
                }
                using(new UsingDataTableLoad(this.dsProcesses.ProcessAlias))
                {
                    this.taManager.ProcessAliasTableAdapter.FillByActive(this.dsProcesses.ProcessAlias, true);
                }
                using(new UsingDataTableLoad(this.dsProcesses.Process))
                {
                    this.taManager.ProcessTableAdapter.FillByActive(this.dsProcesses.Process, true);
                }
                using(new UsingDataTableLoad(this.dsProcesses.CustomerProcessAlias))
                {
                    this.taManager.CustomerProcessAliasTableAdapter.FillByActive(this.dsProcesses.CustomerProcessAlias, true);
                }
                using (new UsingDataTableLoad(this.dsProcesses.ProcessInspections))
                {
                    this.taManager.ProcessInspectionsTableAdapter.FillByActive(this.dsProcesses.ProcessInspections, true);
                }

                taProductClass.Fill(dsProcesses.ProductClass);

                this.dsProcesses.EnforceConstraints = true;

                //Load panels
                this.dpProcessInfo.LoadData(this.dsProcesses, this.taProcessSteps);
                this.dpProcessStep.LoadData(this.dsProcesses);
                this.dpProcessStepQuestion.LoadData(this.dsProcesses, this.taLists, this.taNumericUnits);

                base.AddDataPanel(this.dpProcessInfo);
                base.AddDataPanel(this.dpProcessStep);
                base.AddDataPanel(this.dpProcessStepQuestion);
            }
            else if(!this._loadedInActiveProcesses) //if not already loaded inactive then load it now
            {
                this.dsProcesses.EnforceConstraints = false;

                this.taManager.ProcessTableAdapter.FillByActive(this.dsProcesses.Process, false);
                this.taManager.ProcessInspectionsTableAdapter.FillByActive(this.dsProcesses.ProcessInspections, false);
                this.taManager.ProcessAliasTableAdapter.FillByActive(this.dsProcesses.ProcessAlias, false);
                this.taManager.CustomerProcessAliasTableAdapter.FillByActive(this.dsProcesses.CustomerProcessAlias, false);

                this.dsProcesses.EnforceConstraints = true;
                this._loadedInActiveProcesses = true;
            }
        }

        private void LoadTOC(string filter = null)
        {
            using(new UsingTreeLoad(tvwTOC))
            {
                tvwTOC.Nodes.Clear();
                var folderCache = new Dictionary<string, FolderNode>();

                UltraTreeNode rootNode = new ProcessesRootNode(this.dsProcesses);
                tvwTOC.Nodes.Add(rootNode);
                rootNode.Expanded = true;

                var processes =  this.dsProcesses.Process.Select(filter, "Name") as ProcessesDataset.ProcessRow[];
                
                //Create all dept folder nodes
                foreach(var dept in this.dsProcesses.d_Department)
                {
                    if (!_showInActiveProcesses && !dept.Active)
                    {
                        continue;
                    }

                    var deptNode = new FolderNode(dept);
                    rootNode.Nodes.Add(deptNode);
                    folderCache.Add(dept.DepartmentID, deptNode);
                }

                //add to each process to correct parent
                foreach (var pr in processes ?? Enumerable.Empty<ProcessesDataset.ProcessRow>())
                {
                    if(!folderCache.ContainsKey(pr.Department))
                    {
                        // Skip all processes in an absent department
                        continue;
                    }

                    var deptNode = folderCache[pr.Department];

                    var processNode = new ProcessNode(pr)
                    {
                        Visible = this._showInActiveProcesses || pr.Active
                    };

                    deptNode.Nodes.Add(processNode);
                }

                //expand dept node if only 1 node, useful after a search
                if(rootNode.Nodes.Count == 1)
                    rootNode.Nodes[0].Expanded = true;
            }
        }

        protected override void ReloadTOC()
        {
            this.LoadTOC();
        }

        protected override bool SaveData()
        {
            try
            {
                base.EndAllEdits();

                //get all of the deleted processes
                DataTable deletedProcesses = this.dsProcesses.Process.GetChanges(DataRowState.Deleted);

                if(deletedProcesses != null && deletedProcesses.Rows.Count > 0)
                {
                    //must delete each process from any process packages manually, as there is no relationship to do it on the db side
                    foreach(DataRow deletedProcessRow in deletedProcesses.Rows)
                    {
                        int processID = Convert.ToInt32(deletedProcessRow["ProcessID", DataRowVersion.Original]);
                        this.taProcess.DeleteProcessPackages(processID);
                    }
                }

                //then update the actual processes

                SaveNewRevisedProcesses();
                this.taManager.UpdateAll(this.dsProcesses);

                return true;
            }
            catch(Exception exc)
            {
                _log.Error(this.dsProcesses.GetDataErrors());
                ErrorMessageBox.ShowDialog("Error saving data.", exc);
                return false;
            }
        }

        /// <summary>
        /// Saves all new processes that have revisions.
        /// </summary>
        /// <remarks>
        /// This method is a workaround for TFS #4350.
        /// </remarks>
        private void SaveNewRevisedProcesses()
        {
            var newRevisedProcessIDs = this.dsProcesses.Process
                .Where(p => p.RowState == DataRowState.Added && !p.IsParentIDNull())
                .Select(p => p.ProcessID);

            var savedProcessMap = new Dictionary<int, int>();
            foreach (int newRevisedProcessID in newRevisedProcessIDs)
            {
                // skip deleted orders - causes error
                var newRevisedProcess = this.dsProcesses.Process
                    .FirstOrDefault(proc => proc.IsValidState() && proc.ProcessID == newRevisedProcessID);

                if (newRevisedProcess == null)
                {
                    continue;
                }

                int originalParentID = newRevisedProcess.ParentID;
                if (originalParentID > 0)
                {
                    // refers to existing process
                    continue;
                }
                else if (savedProcessMap.ContainsKey(newRevisedProcess.ParentID))
                {
                    // refers to process persisted by this method
                    newRevisedProcess.ParentID = savedProcessMap[originalParentID];
                    continue;
                }

                // Get all other unsaved processes in hierarchy
                var rowsToSave = new Stack<ProcessesDataset.ProcessRow>(); // save the highest process in hieararchy first
                ProcessesDataset.ProcessRow currentRow = this.dsProcesses.Process.FindByProcessID(originalParentID);
                while (currentRow != null)
                {
                    rowsToSave.Push(currentRow);

                    if (currentRow.IsParentIDNull() || currentRow.ParentID > 0)
                    {
                        currentRow = null;
                    }
                    else if (savedProcessMap.ContainsKey(currentRow.ParentID))
                    {
                        int currentRowParentID = currentRow.ParentID;
                        currentRow.ParentID = savedProcessMap[currentRowParentID];
                        currentRow = null;
                    }
                    else
                    {
                        currentRow = this.dsProcesses.Process.FindByProcessID(currentRow.ParentID);
                    }
                }

                // Save parent processes & add them to savedProcessMap
                while (rowsToSave.Count > 0)
                {
                    var rowToSave = rowsToSave.Pop();
                    int originalRowToSaveID = rowToSave.ProcessID;

                    this.taProcess.Update(rowToSave);

                    _log.Info("Successfully saved new parent process {0} ({1})",
                        rowToSave.Name, rowToSave.ProcessID);

                    if (rowsToSave.Count == 0)
                    {
                        // found direct parent to the current process
                        newRevisedProcess.ParentID = rowToSave.ProcessID;
                    }
                    else
                    {
                        rowsToSave.Peek().ParentID = rowToSave.ProcessID;
                    }

                    savedProcessMap[originalRowToSaveID] = rowToSave.ProcessID;
                }
            }
        }

        private void LoadCommands()
        {
            if(SecurityManager.Current.IsInRole("ProcessManager.Edit"))
            {
                var addP = base.Commands.AddCommand("AddProcess", new AddCommand(toolbarManager.Tools["AddProcess"], tvwTOC, enumNodeType.ProcessNode)) as AddCommand;
                addP.AddNode = this.AddNode;

                var addS = base.Commands.AddCommand("AddStep", new AddCommand(toolbarManager.Tools["AddStep"], tvwTOC, enumNodeType.ProcessStepNode)) as AddCommand;
                addS.AddNode = this.AddNode;

                var addQ = base.Commands.AddCommand("AddQuestion", new AddCommand(toolbarManager.Tools["AddQuestion"], tvwTOC, enumNodeType.ProcessStepQuestionNode)) as AddCommand;
                addQ.AddNode = this.AddNode;

                var cc = base.Commands.AddCommand("Copy", new CopyCommand(toolbarManager.Tools["Copy"], tvwTOC)) as CopyCommand;
                var pc = base.Commands.AddCommand("Paste", new PasteCommand(toolbarManager.Tools["Paste"], tvwTOC)) as PasteCommand;
                cc.AddRelatedCommandToRefresh(pc);

                var dc = base.Commands.AddCommand("Delete", new DeleteCommand(toolbarManager.Tools["Delete"], tvwTOC, this)) as DeleteCommand;
                dc.CancelCommand += (di) =>
                                    {
                                        //if selected a process step node
                                        if(di.Any(n => n is ProcessStepNode))
                                        {
                                            //if selected the remaining steps
                                            var steps = di.OfType <ProcessStepNode>().ToList();
                                            if(steps[0].Process.Nodes.Count <= steps.Count)
                                            {
                                                MessageBoxUtilities.ShowMessageBoxWarn("Process must contain at least one step.", "Delete Process");
                                                return true;
                                            }
                                        }

                                        string deletePrompt;
                                        if (di.Count == 1)
                                        {
                                            deletePrompt = "Are you sure you want to delete '{0}'?".FormatWith(di[0].ToString());
                                        }
                                        else
                                        {
                                            deletePrompt = "Are you sure you want to delete the selected processes?";
                                        }

                                        return MessageBoxUtilities.ShowMessageBoxYesOrNo(deletePrompt, "Delete Process") != DialogResult.Yes;
                                    };

                var rc = base.Commands.AddCommand("CreateRevision", new ReviseCommand(toolbarManager.Tools["CreateRevision"], tvwTOC)) as ReviseCommand;
                rc.ReviseNode += (s, rce) => this.ReviseProcess(rce.Item);

                base.Commands.AddCommand("Freeze", new FreezeCommand(toolbarManager.Tools["Freeze"], tvwTOC, dpProcessInfo));
                base.Commands.AddCommand("Export", new ExportCommand(toolbarManager.Tools["Export"], tvwTOC, dpProcessInfo));
                base.Commands.AddCommand("Import", new ImportCommand(toolbarManager.Tools["Import"], tvwTOC, this));

                dpProcessInfo.LoadCommand(base.Commands, toolbarManager);
            }

            (toolbarManager.Tools["DisplayInactive"]).ToolClick += this.ProcessManager_DisplayInactive;

            base.Commands.AddCommand("ProcessUsageSummary", new ProcessUsageReportCommand(this.toolbarManager.Tools["ProcessUsage"]));
            base.Commands.AddCommand("ProcessSheet", new PrintProcessSheetCommand(this.toolbarManager.Tools["ProcessSheet"], tvwTOC));
            base.Commands.AddCommand("Print", new PrintNodeCommand(toolbarManager.Tools["Print"], tvwTOC));
            base.Commands.AddCommand("Search", new ProcessSearchCommand(toolbarManager.Tools["AdvancedSearch"], tvwTOC, this));
        }

        private void AddNode(enumNodeType nodeType)
        {
            if (!IsValidControls())
            {
                return;
            }

            _log.Info("Adding new node: " + nodeType);

            var selectedNode = tvwTOC.SelectedNode<UltraTreeNode>();

            _validators.Enabled = false;

            switch(nodeType)
            {
                case enumNodeType.ProcessNode:
                    if (selectedNode is FolderNode || selectedNode is ProcessesRootNode)
                        this.AddProcess(selectedNode);
                    break;
                case enumNodeType.ProcessStepNode:
                    if(selectedNode is ProcessNode)
                        this.AddProcessStep((ProcessNode)selectedNode);
                    else if(selectedNode is ProcessStepNode)
                        this.AddProcessStep((ProcessNode)selectedNode.Parent);
                    else if(selectedNode is ProcessQuestionNode)
                        this.AddProcessStep((ProcessNode)selectedNode.Parent.Parent);
                    break;
                case enumNodeType.ProcessStepQuestionNode:
                    if(selectedNode is ProcessStepNode)
                        this.AddProcessStepQuestion((ProcessStepNode)selectedNode);
                    else if(selectedNode is ProcessQuestionNode)
                        this.AddProcessStepQuestion((ProcessStepNode)selectedNode.Parent);
                    break;
                default:
                    break;
            }

            _validators.Enabled = true;
        }

        private void AddProcess(UltraTreeNode pn)
        {
            _log.Info("Adding a process.");

            //create new data source
            ProcessesDataset.ProcessRow cr = this.dpProcessInfo.AddProcessRow();

            var folderNode = pn as FolderNode;
            
            if (folderNode == null)
            {
                //find first folder node
                var firstNode = tvwTOC.FindNode(fn => fn is FolderNode) as FolderNode;
                if (firstNode == null)
                {
                    //if no folder node then just create one
                    folderNode = new FolderNode(this.dsProcesses.d_Department[0]);
                    tvwTOC.Nodes[0].Nodes.Add(folderNode);
                }
                else
                {
                    folderNode = firstNode;
                }
            }

            cr.LoadCapacityType = "Weight";

            //sync departments
            cr.Department = folderNode.DataRow.DepartmentID;

            //create new ui nodes
            var cn = new ProcessNode(cr);
            folderNode.Nodes.Add(cn);
            cn.Select();
        }

        private void AddProcessStep(ProcessNode pn)
        {
            _log.Info("Adding a process step: " + pn.Text + " - " + pn.Key);

            //create new data source
            ProcessesDataset.ProcessStepsRow cr = dpProcessStep.AddProcessStepRow(pn.DataRow.ProcessID, pn.GetNextStepOrder());

            var cn = new ProcessStepNode(cr);
            pn.Nodes.Add(cn);
            cn.Select();
        }

        private void AddProcessStepQuestion(ProcessStepNode pn)
        {
            _log.Info("Adding a process step question: " + pn.Text + " - " + pn.Key);

            //create new data source
            ProcessesDataset.ProcessQuestionRow cr = this.dpProcessStepQuestion.AddProcessQuestionRow(pn.DataRow.ProcessStepID, pn.GetNextStepOrder());

            //create new ui nodes
            var cn = new ProcessQuestionNode(cr);
            pn.Nodes.Add(cn);
            cn.Select();
        }

        protected override void LoadNode(UltraTreeNode node)
        {
            ProcessNode prNode = null;
            DataPanel curDataPanel = null;
            string dataID = null;

            toolbarManager.Ribbon.ContextualTabGroups[0].Visible = false;

            if(node is ProcessNode)
            {
                prNode = (ProcessNode)node;
                prNode.LoadChildrenNodes(this);
                curDataPanel = this.dpProcessInfo;
                dataID = ((ProcessNode)node).ID;
            }
            else if(node is ProcessStepNode)
            {
                prNode = (ProcessNode)node.Parent;
                curDataPanel = this.dpProcessStep;
                dataID = ((ProcessStepNode)node).ID;
            }
            else if(node is ProcessQuestionNode)
            {
                prNode = (ProcessNode)node.Parent.Parent;
                curDataPanel = this.dpProcessStepQuestion;
                dataID = ((ProcessQuestionNode)node).ID;
            }

            //if panel set then move to it
            if(curDataPanel != null && prNode != null)
            {
                UpdateUsageCounts(prNode);
                curDataPanel.MoveToRecord(dataID);
                curDataPanel.Editable = prNode.UsageCount.GetValueOrDefault() == 0 && !prNode.DataRow.Frozen; //editable if no one using it and not frozen
                DisplayPanel(curDataPanel);
            }
            else
                DisplayPanel(null);
        }

        private void UpdateUsageCounts(ProcessNode process)
        {
            int processID = process.DataRow.ProcessID;
            if (!process.UsageCount.HasValue)
            {
                var o = taProcess.UsageCount(processID);
                process.UsageCount = o == null || o == DBNull.Value ? 0 : Convert.ToInt32(o);
            }

            // Count revisions of this process that are currently loaded.
            process.RevisionCount = dsProcesses.Process
                .Count(proc => proc.IsValidState() && !proc.IsParentIDNull() && proc.ParentID == processID);
        }

        protected override void SaveSelectedNode()
        {
            if(tvwTOC.SelectedNodes.Count > 0)
                Properties.Settings.Default.LastSelectedProcess = tvwTOC.SelectedNodes[0].Key;
        }

        private void ReviseProcess(ProcessNode nodeToRevise)
        {
            _log.Info("Revising a process.");
            var processToRevise = nodeToRevise.DataRow;

            CopyCommand.CopyNode(nodeToRevise);
            var newRevisionNode = PasteCommand.PasteNode(tvwTOC.Nodes[0] as ICopyPasteNode) as ProcessNode; //paste to root node

            if(newRevisionNode != null)
            {
                _log.Info("Pasted: " + newRevisionNode.DataRow.ToDetailedString());

                //set old process as inactive
                processToRevise.Active = false;

                //create new revision and set new process as active
                newRevisionNode.DataRow.Revision = processToRevise.IsRevisionNull()
                    ? "<None>"
                    : processToRevise.Revision.Increment();

                newRevisionNode.DataRow.Active = true;
                newRevisionNode.DataRow.ParentID = processToRevise.ProcessID;
                newRevisionNode.DataRow.SetFrozenByNull();
                newRevisionNode.DataRow.SetFrozenDateNull();
                newRevisionNode.DataRow.Frozen = false;

                // Automatically update revised inspections
                foreach (var processInspection in newRevisionNode.DataRow.GetProcessInspectionsRows())
                {
                    if (processInspection.PartInspectionTypeRow.Active)
                    {
                        continue;
                    }

                    var revisedPartInspectionTypeId = taInspectionType.GetRevisedPartInspectionType(processInspection.PartInspectionTypeID);

                    if (revisedPartInspectionTypeId.HasValue)
                    {
                        processInspection.PartInspectionTypeID = revisedPartInspectionTypeId.Value;
                    }
                    else
                    {
                        _log.Warn($"Could not find active revision for PartInspectionType {processInspection.PartInspectionTypeID}");
                    }
                }

                // Update suggestions belonging to active processes that recommend the old process
                // Assumption:
                //    - All suggestions from active processes that relate to
                //      the old node have been loaded
                if (newRevisionNode.CopiedAliasMap == null)
                {
                    _log.Error("A map of old aliases to new aliases should be available when revising a process.");
                }
                else
                {
                    foreach (var oldSuggestionRow in processToRevise.GetProcessSuggestionRowsByFK_ProcessSuggestion_Process_Suggested())
                    {
                        if (!(oldSuggestionRow.ProcessRowByFK_ProcessSuggestion_Process_Primary?.Active ?? false))
                        {
                            continue;
                        }

                        oldSuggestionRow.ProcessRowByFK_ProcessSuggestion_Process_Suggested = newRevisionNode.DataRow;

                        var oldProcessAliasId = oldSuggestionRow.SuggestedProcessAliasID.ToString();
                        if (newRevisionNode.CopiedAliasMap.TryGetValue(oldProcessAliasId, out var newProcessAliasId))
                        {
                            oldSuggestionRow.SuggestedProcessAliasID = int.Parse(newProcessAliasId);
                        }
                        else
                        {
                            _log.Warn($"New Process Alias not found for {oldProcessAliasId}");
                        }
                    }
                }

                //select old process then re-select to force refresh of UI
                nodeToRevise.Select();
                newRevisionNode.Select();
            }
        }

        protected override void LoadNodes(List<IDataNode> nodes)
        {
            foreach(var dataNode in nodes.OfType<ProcessNode>())
            {
                //ensure node count is updated
                UpdateUsageCounts(dataNode as ProcessNode);
            }
        }

        private void FilterTree(string partName)
        {
            try
            {
                if (tvwTOC.Nodes.Count == 1)
                {
                    using (new UsingWaitCursor(this))
                    {
                        using (new UsingTimeMe("loading filtered processes."))
                        {
                            var filter = " Name LIKE '%" + DataUtilities.PrepareForRowFilterLike(partName) + "%'";
                            if (!_showInActiveProcesses)
                            {
                                // Add a check for active processes when filtering if active processes are not displayed
                                filter += " AND Active = true";
                            }

                            LoadTOC(filter);

                            if (!string.IsNullOrEmpty(partName))
                                this.txtNodeFilter.Appearance.BorderColor = Color.Red;
                            else
                                this.txtNodeFilter.Appearance.ResetBorderColor();
                        }
                    }
                }
                else
                    this.txtNodeFilter.Appearance.ResetBorderColor();
            }
            catch (Exception exc)
            {
                string errorMsg = "Error updating in line filter.";
                _log.Error(exc, errorMsg);
            }
        }

        /// <summary>
        /// Selects the node associated with a process search result.
        /// </summary>
        /// <param name="processSearchResult">Result to use for finding the node.</param>
        private void GoToProcess(DWOS.Data.Datasets.ProcessesDataset.ProcessSearchRow processSearchResult)
        {
            if (processSearchResult == null)
            {
                return;
            }

            bool selectedNode = false;

            if (processSearchResult.Status == "Closed")
            {
                // Display inactive processes.
                var displayInactiveTool = (StateButtonTool)base.toolbarManager.Tools["DisplayInactive"];
                displayInactiveTool.Checked = true;
            }

            var process = dsProcesses.Process.FindByProcessID(processSearchResult.ProcessID);

            if (process != null)
            {
                txtNodeFilter.Text = null;
                FilterTree(null);

                var processNode = tvwTOC.Nodes.FindNode<ProcessNode>(node => node.DataRow.ProcessID == process.ProcessID);

                if (processNode != null)
                {
                    processNode.Select();
                    selectedNode = true;
                }
            }

            if (!selectedNode)
            {
                string msg = string.Format("Could not select process node for process ID {0}", processSearchResult.ProcessID);
                _log.Debug(msg);
            }
        }

        #endregion

        #region Events

        private void Processes_Load(object sender, EventArgs e)
        {
            _loadingData = true; //prevent TOC from loading

            try
            {
                if(DesignMode)
                    return;

                tvwTOC.Override.SelectionType = SelectType.Extended;
                LoadCommands();
                this.LoadData(true);
                this.LoadTOC();
                LoadValidators();

                tvwTOC.Override.SortComparer = new ProcessNodesSorter();
                tvwTOC.Override.Sort = SortType.Ascending;

                _loadingData = false;

                RestoreLastSelectedNode(Properties.Settings.Default.LastSelectedProcess);

                splitContainer1.Panel2.Enabled = SecurityManager.Current.IsInRole("ProcessManager.Edit");
            }
            catch(Exception exc)
            {
                splitContainer1.Enabled = false;
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
                _log.Fatal(exc, "Error loading form.");
            }
        }

        private void ProcessManager_DisplayInactive(object sender, ToolClickEventArgs e)
        {
            this._showInActiveProcesses = ((StateButtonTool)e.Tool).Checked;
            this.LoadData(false);
            this.ReloadTOC();
        }

        private void txtNodeFilter_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                if (_loadingData)
                {
                    return;
                }

                if (e.Button.Key == "GO")
                    this.FilterTree(this.txtNodeFilter.Text);

                else if (e.Button.Key == "Reset")
                {
                    this.txtNodeFilter.Text = null;
                    this.FilterTree(null);
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error updating filter.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void txtNodeFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!_loadingData && e.KeyChar == '\r')
                {
                    e.Handled = true;
                    this.FilterTree(this.txtNodeFilter.Text);
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error updating filter.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion

        #region Nodes

        #region Nested type: FolderNode

        private class FolderNode : DataNode<ProcessesDataset.d_DepartmentRow>
        {
            #region Fields

            public const string KEY_PREFIX = "FOLDER";
            private static Image _imageCache = null;

            #endregion

            #region Methods

            public FolderNode(ProcessesDataset.d_DepartmentRow cr)
                : base(cr, cr.DepartmentID, KEY_PREFIX, cr.DepartmentID)
            {
                if (_imageCache == null)
                    _imageCache = Properties.Resources.Folder_16;

                LeftImages.Add(_imageCache);

                this.UpdateNodeUI();
            }

            public override bool CanDelete
            {
                get { return false; }
            }

            public override void UpdateNodeUI()
            {
                if(base.DataRow.Active)
                {
                    Override.NodeAppearance.FontData.Italic = DefaultableBoolean.False;
                    Override.NodeAppearance.ForeColor = Color.Black;
                }
                else
                {
                    Override.NodeAppearance.FontData.Italic = DefaultableBoolean.True;
                    Override.NodeAppearance.ForeColor = Color.Salmon;
                }
            }

            #endregion

            #region ICopyPasteNode Members

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

                var psDS = this.DataRow.Table.DataSet as ProcessesDataset;

                // Do not copy process that has been deleted.
                int originalProcessID = int.Parse(proxy.OriginalPrimaryKey);
                var hasOriginalRow = psDS.Process.Any(row => row.IsValidState() && row.ProcessID == originalProcessID);

                if (!hasOriginalRow)
                {
                    return null;
                }

                var relationsToRemove = new HashSet<string>
                {
                    // Delete double relationship "FK_ProcessStepCondition_ProcessSteps";
                    // keep the other side "FK_ProcessQuestion_ProcessStepCondition"
                    "FK_ProcessQuestion_ProcessStepCondition",

                    // Do not copy suggestions that recommend the copied process
                    "FK_ProcessSuggestion_Process_Suggested",
                    "FK_ProcessSuggestion_ProcessAlias"
                };

                proxy.Remove(w => relationsToRemove.Contains(w.ParentRelation));

                var results = new List <DataRowProxyResults>();
                var process = DataNode<DataRow>.AddPastedDataRows(proxy, psDS.Process, results) as ProcessesDataset.ProcessRow;

                foreach(var processStep in process.GetProcessStepsRows())
                {
                    foreach(var condition in processStep.GetProcessStepConditionRows())
                    {
                        if (condition.IsProcessQuestionIdNull())
                        {
                            continue;
                        }

                        //need to point back to new processquestionid, not new one
                        var original = results.FirstOrDefault(c => c.Proxy.OriginalPrimaryKey == condition.ProcessQuestionId.ToString());

                        if (original != null)
                            condition.ProcessQuestionRow = original.Row as ProcessesDataset.ProcessQuestionRow;
                    }
                }

                process.Department = this.DataRow.DepartmentID;

                // Map old process aliases to new process aliases.
                var aliasMap = new Dictionary<string, string>();
                foreach (var aliasResult in results.Where(r => r.Proxy.ParentRelation == "FK_ProcessAlias_Process"))
                {
                    var originalAliasId = aliasResult.Proxy.OriginalPrimaryKey;
                    var newAliasId = aliasResult.Row["ProcessAliasID"].ToString();
                    aliasMap[originalAliasId] = newAliasId;
                }

                // Create new node for pasted process
                var node = new ProcessNode(process, aliasMap);
                base.Nodes.Add(node);
                return node;
            }

            public override bool CanPasteData(string format)
            {
                //Can paste processes only
                return format == typeof(ProcessNode).FullName;
            }

            public override string ClipboardDataFormat
            {
                get { return null; }
            }

            #endregion
        }

        #endregion

        #region Nested type: ProcessNode

        private class ProcessNode : DataNode<ProcessesDataset.ProcessRow>, IReportNode
        {
            #region Fields

            public const string KEY_PREFIX = "PR";
            private bool _isMoving;
            private static Image _imageCache = null;
            private static Image _imageCachePaper = null;

            #endregion

            #region Properties

            public bool IsDataLoaded { get; set; }

            public int? UsageCount { get; set; }

            public int? RevisionCount { get; set; }

            #endregion

            #region Methods

            static ProcessNode()
            {
                if (_imageCache == null)
                    _imageCache = Properties.Resources.Process_16;

                if (_imageCachePaper == null)
                    _imageCachePaper = Properties.Resources.Paper_16;
            }

            public ProcessNode(ProcessesDataset.ProcessRow cr)
                : base(cr, cr.ProcessID.ToString(), KEY_PREFIX, cr.Name)
            {
                LeftImages.Add(_imageCache);

                this.UpdateNodeUI();
            }

            public ProcessNode(ProcessesDataset.ProcessRow cr, IDictionary<string, string> copiedAliasMap)
                : this(cr)
            {
                CopiedAliasMap = copiedAliasMap;
            }

            public override bool CanDelete
            {
                get { return this.UsageCount < 1 && this.RevisionCount < 1; }
            }
            
            public override string ClipboardDataFormat
            {
                get { return GetType().FullName; }
            }

            public IDictionary<string, string> CopiedAliasMap { get; }

            public override void UpdateNodeUI()
            {
                try
                {
                    //if department changed then move folder
                    if(base.Parent is FolderNode && ((FolderNode)base.Parent).DataRow.DepartmentID != base.DataRow.Department)
                    {
                        if(this._isMoving)
                            return;

                        this._isMoving = true;

                        var deptNode = base.Control.Nodes.FindNodeBFS(n => n is FolderNode && ((FolderNode)n).DataRow.DepartmentID == base.DataRow.Department);

                        if(deptNode == null)
                        {
                            deptNode = new FolderNode(base.DataRow.d_DepartmentRow);
                            base.Control.Nodes[0].Nodes.Add(deptNode);
                        }

                        //move to correct node
                        Remove();
                        deptNode.Nodes.Add(this);

                        this._isMoving = false;
                    }

                    string rev = base.DataRow.IsRevisionNull() ? "" : " - " + base.DataRow.Revision;
                    Text = base.DataRow.Name + rev;

                    //Add Paper-Icon if paper based
                    RightImages.Clear();
                    if(!DataRow.IsPaperless)
                        RightImages.Add(_imageCachePaper);

                    if (base.DataRow.Active && !base.DataRow.IsIsApprovedNull() && base.DataRow.IsApproved)
                    {
                        Override.NodeAppearance.FontData.Italic = DefaultableBoolean.False;
                        Override.NodeAppearance.FontData.Bold = DefaultableBoolean.False;
                        Override.NodeAppearance.ForeColor = Color.Green;
                    }
                    else if (base.DataRow.Active)
                    {
                        Override.NodeAppearance.FontData.Italic = DefaultableBoolean.False;
                        Override.NodeAppearance.FontData.Bold = DefaultableBoolean.True;
                        Override.NodeAppearance.ForeColor = Color.Orange;
                    }
                    else
                    {
                        Override.NodeAppearance.FontData.Italic = DefaultableBoolean.True;
                        Override.NodeAppearance.FontData.Bold = DefaultableBoolean.False;
                        Override.NodeAppearance.ForeColor = Color.Salmon;
                    }
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error updating node UI.");
                }
            }

            public decimal GetNextStepOrder()
            {
                ProcessesDataset.ProcessStepsRow[] rows = base.DataRow.GetProcessStepsRows();
                decimal maxCount = 0;

                if (rows.Length > 0)
                {
                    maxCount = rows.Max(r => r.StepOrder);
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

                if (format == typeof(ProcessStepNode).FullName)
                {
                    var psDS = this.DataRow.Table.DataSet as ProcessesDataset;

                    // Do not copy step that has been deleted
                    int originalStepID = int.Parse(proxy.OriginalPrimaryKey);
                    var hasOriginalRow = psDS.ProcessSteps.Any(row => row.IsValidState() && row.ProcessStepID == originalStepID);

                    if (!hasOriginalRow)
                    {
                        return null;
                    }

                    //Delete Step Condition relations since the related step is not being copied
                    proxy.Remove(w => w.ParentRelation == "FK_ProcessQuestion_ProcessStepCondition" || w.ParentRelation == "FK_ProcessStepCondition_ProcessSteps");

                    var results = new List<DataRowProxyResults>();
                    var processStep = DataNode<DataRow>.AddPastedDataRows(proxy, psDS.ProcessSteps, results) as ProcessesDataset.ProcessStepsRow;

                    foreach (var condition in processStep.GetProcessStepConditionRows())
                    {
                        if (condition.IsProcessQuestionIdNull())
                        {
                            continue;
                        }

                        //need to point back to new processquestionid, not new one
                        var original = results.FirstOrDefault(c => c.Proxy.OriginalPrimaryKey == condition.ProcessQuestionId.ToString());

                        if (original != null)
                            condition.ProcessQuestionRow = original.Row as ProcessesDataset.ProcessQuestionRow;
                    }

                    if (processStep != null)
                    {
                        processStep.SetParentRow(base.DataRow);
                        var node = new ProcessStepNode(processStep);
                        base.Nodes.Add(node);

                        return node;
                    }
                }

                return null;
            }

            public override bool CanPasteData(string format)
            {
                return format == typeof(ProcessStepNode).FullName;
            }

            internal void LoadChildrenNodes(ProcessManager pm)
            {
                if (IsDataLoaded || pm == null)
                {
                    return;
                }

                var dsProcesses = pm.dsProcesses;

                try
                {
                    using(new UsingTimeMe("Loading Nodes for process " + DataRow.ProcessID))
                    {
                        using(new UsingTreeLoad(base.Control))
                        {
                            //Load Steps
                            dsProcesses.EnforceConstraints = false;
                            pm.taManager.ProcessStepsTableAdapter.FillBy(dsProcesses.ProcessSteps, base.DataRow.ProcessID);
                            pm.taManager.ProcessStepDocumentLinkTableAdapter.FillByProcess(dsProcesses.ProcessStepDocumentLink, base.DataRow.ProcessID);
                            pm.taManager.ProcessQuestionTableAdapter.FillBy(dsProcesses.ProcessQuestion, base.DataRow.ProcessID);
                            pm.taManager.ProcessRequisiteTableAdapter.FillByParent(dsProcesses.ProcessRequisite, base.DataRow.ProcessID);
                            pm.taManager.ProcessStepConditionTableAdapter.FillBy(dsProcesses.ProcessStepCondition, base.DataRow.ProcessID);
                            pm.taManager.ProcessQuestionFieldTableAdapter.FillByProcess(dsProcesses.ProcessQuestionField, base.DataRow.ProcessID);

                            var processSteps = base.DataRow.GetProcessStepsRows();

                            foreach (var processStep in processSteps)
                                Nodes.Add(new ProcessStepNode(processStep));

                            // Load constrained processes if needed
                            foreach (var constraint in DataRow.GetProcessRequisiteRowsByFK_ProcessRequisite_ProcessParent())
                            {
                                if (dsProcesses.Process.FindByProcessID(constraint.ChildProcessID) == null)
                                {
                                    pm.taManager.ProcessTableAdapter.FillByProcess(dsProcesses.Process, constraint.ChildProcessID);
                                }
                            }

                            // Load suggestions
                            // This node will show suggestions where the
                            // process is the primary one, but this also
                            // loads other related suggestions to copy
                            // when revising this process.
                            using (var dtTempProcessSuggestion = new ProcessesDataset.ProcessSuggestionDataTable())
                            {
                                pm.taManager.ProcessSuggestionTableAdapter.FillByPrimaryProcess(dtTempProcessSuggestion, base.DataRow.ProcessID);
                                pm.taManager.ProcessSuggestionTableAdapter.FillBySuggestedProcess(dtTempProcessSuggestion, base.DataRow.ProcessID);
                                dsProcesses.ProcessSuggestion.Merge(dtTempProcessSuggestion, true);
                            }

                            // Retrieve any suggested processes that haven't been loaded
                            foreach (var suggestion in DataRow.GetProcessSuggestionRowsByFK_ProcessSuggestion_Process_Primary())
                            {
                                if (dsProcesses.Process.FindByProcessID(suggestion.SuggestedProcessID) == null)
                                {
                                    pm.taManager.ProcessTableAdapter.FillByProcess(dsProcesses.Process,
                                        suggestion.SuggestedProcessID);
                                }

                                if (dsProcesses.ProcessAlias.FindByProcessAliasID(suggestion.SuggestedProcessAliasID) == null)
                                {
                                    // Assumption: If one of the process
                                    // aliases have not been loaded, none of
                                    // them have been loaded.
                                    pm.taManager.ProcessAliasTableAdapter.FillByProcess(dsProcesses.ProcessAlias,
                                        suggestion.SuggestedProcessID);
                                }
                            }

                            // Retrieve any primary processes that haven't been loaded
                            foreach (var suggestion in DataRow.GetProcessSuggestionRowsByFK_ProcessSuggestion_Process_Suggested())
                            {
                                if (dsProcesses.Process.FindByProcessID(suggestion.PrimaryProcessID) == null)
                                {
                                    pm.taManager.ProcessTableAdapter.FillByProcess(dsProcesses.Process,
                                        suggestion.PrimaryProcessID);
                                }

                                // Do not need to load process aliases for primary processes
                            }

                            // Load product classes
                            pm.taManager.ProcessProductClassTableAdapter
                                .FillByProcess(dsProcesses.ProcessProductClass, DataRow.ProcessID);

                            // Fix For VSTS #28133
                            // If any condition happens to point to a question
                            // that don't belong to the process and is not
                            // loaded, a constraints error breaks functionality.
                            var invalidConditions = DataRow.GetProcessStepsRows()
                                .SelectMany(ps => ps.GetProcessStepConditionRows())
                                .Where(psc => !psc.IsProcessQuestionIdNull() && psc.ProcessQuestionRow == null)
                                .ToList();

                            foreach (var invalidCondition in invalidConditions)
                            {
                                invalidCondition.Delete();
                                invalidCondition.AcceptChanges();
                            }
                        }
                    }
                }
                catch(Exception exc)
                {
                    var errInfo = "";

                    if(pm.dsProcesses != null)
                        errInfo = pm.dsProcesses.GetDataErrors();

                    LogManager.GetCurrentClassLogger().Error(exc, "Error loading process node children; " + errInfo);
                }
                finally
                {
                    IsDataLoaded = true;
                    try
                    {
                        if (dsProcesses != null)
                        {
                            dsProcesses.EnforceConstraints = true;
                        }
                    }
                    catch (Exception exc)
                    {
                        LogManager.GetCurrentClassLogger().Error(exc, "DataSet Errors: " + dsProcesses.GetDataErrors());
                    }
                }
            }

            #endregion

            #region IReportNode Members

            public IReport CreateReport(string reportType)
            {
                if("Process Summary" == reportType)
                    return new ProcessSummaryReport(base.DataRow);
                else if("Process Parts" == reportType)
                    return new ProcessPartsReport(base.DataRow.ProcessID);
                else
                    return null;
            }

            public string[] ReportTypes()
            {
                return new[]{"Process Summary", "Process Parts"};
            }

            #endregion
        }

        #endregion

        #region Nested type: ProcessQuestionNode

        private class ProcessQuestionNode : DataNode<ProcessesDataset.ProcessQuestionRow>
        {
            #region Fields
            
            public const string KEY_PREFIX = "PQ";
            private static Image _imageCache = null;
           
            #endregion

            #region Methods

            public ProcessQuestionNode(ProcessesDataset.ProcessQuestionRow cr)
                : base(cr, cr.ProcessQuestionID.ToString(), KEY_PREFIX, cr.StepOrder + " - " + cr.Name)
            {
                this.UpdateNodeUI();

                if (_imageCache == null)
                    _imageCache = Properties.Resources.Question_16;

                LeftImages.Add(_imageCache);
            }

            public override bool CanDelete
            {
                get
                {
                    if(base.Parent != null && base.Parent.Parent is ProcessNode)
                        return ((ProcessNode)base.Parent.Parent).CanDelete;
                    else
                        return base.CanDelete;
                }
            }

            public override string ClipboardDataFormat
            {
                get { return GetType().FullName; }
            }

            public override void UpdateNodeUI()
            {
                bool isSub = base.DataRow.StepOrder != Convert.ToInt32(base.DataRow.StepOrder);
                Text = (isSub ? "  " : "") + base.DataRow.StepOrder.ToString("F1") + " - " + base.DataRow.Name + (base.DataRow.IsDefaultValueNull() ? "" : " - " + base.DataRow.DefaultValue);
            }

            #endregion
        }

        #endregion

        #region Nested type: ProcessStepNode

        private class ProcessStepNode: DataNode<ProcessesDataset.ProcessStepsRow>
        {
            #region Fields
            
            public const string KEY_PREFIX = "PS";
            private static Image _imageCache = null;
            
            #endregion

            #region Properties

            public ProcessNode Process
            {
                get { return base.Parent as ProcessNode; }
            }

            #endregion

            #region Methods

            public ProcessStepNode(ProcessesDataset.ProcessStepsRow cr)
                : base(cr, cr.ProcessStepID.ToString(), KEY_PREFIX, cr.StepOrder + " - " + cr.Name)
            {
                if (_imageCache == null)
                    _imageCache = Properties.Resources.StepOrder;

                LeftImages.Add(_imageCache);

                foreach(ProcessesDataset.ProcessQuestionRow row in cr.GetProcessQuestionRows())
                    Nodes.Add(new ProcessQuestionNode(row));

                this.UpdateNodeUI();
            }

            public override bool CanDelete
            {
                get
                {
                    if(base.Parent is ProcessNode)
                        return ((ProcessNode)base.Parent).CanDelete;
                    else
                        return base.CanDelete;
                }
            }
            
            public override string ClipboardDataFormat
            {
                get { return GetType().FullName; }
            }

            public override void UpdateNodeUI()
            {
                Text = base.DataRow.StepOrder.ToString("F1") + " - " + base.DataRow.Name;

                base.RightImages.Clear();

                if(base.DataRow.COCData)
                    base.RightImages.Add(Properties.Resources.Certificate_16);

                if(base.DataRow.GetProcessStepConditionRows().Any(r => r.RowState != DataRowState.Deleted))
                    base.RightImages.Add(Properties.Resources.Equal_16);
            }

            public decimal GetNextStepOrder()
            {
                ProcessesDataset.ProcessQuestionRow[] rows = base.DataRow.GetProcessQuestionRows();
                decimal maxCount = 0;

                foreach(ProcessesDataset.ProcessQuestionRow row in rows)
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

                if (format == typeof(ProcessQuestionNode).FullName)
                {
                    var psDS = this.DataRow.Table.DataSet as ProcessesDataset;

                    // Do not copy question that has been deleted
                    int originalQuestionID = int.Parse(proxy.OriginalPrimaryKey);
                    var hasOriginalRow = psDS.ProcessQuestion.Any(row => row.IsValidState() && row.ProcessQuestionID == originalQuestionID);

                    if (!hasOriginalRow)
                    {
                        return null;
                    }

                    DataRow dr = DataNode<DataRow>.AddPastedDataRows(proxy, base.DataRow.Table.DataSet.Tables["ProcessQuestion"]);
                    var pqr = dr as ProcessesDataset.ProcessQuestionRow;

                    if(pqr != null)
                    {
                        pqr.SetParentRow(base.DataRow);
                        var node = new ProcessQuestionNode(pqr);
                        base.Nodes.Add(node);

                        return node;
                    }
                }

                return null;
            }

            public override bool CanPasteData(string format)
            {
                return format == typeof(ProcessQuestionNode).FullName;
            }

            #endregion
        }

        #endregion

        #region Nested type: ProcessesRootNode

        private class ProcessesRootNode : UltraTreeNode, ICopyPasteNode
        {
            #region Fields

            private ProcessesDataset _dataset;

            #endregion

            #region Methods

            public ProcessesRootNode(ProcessesDataset dataset)
                : base("ROOT", "Processes")
            {
                this._dataset = dataset;
                LeftImages.Add(Properties.Resources.Processes_32);
            }

            public override void Dispose()
            {
                this._dataset = null;
                base.Dispose();
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

                var relationsToRemove = new HashSet<string>
                {
                    // Delete double relationship "FK_ProcessStepCondition_ProcessSteps";
                    // keep the other side "FK_ProcessQuestion_ProcessStepCondition"
                    "FK_ProcessQuestion_ProcessStepCondition",

                    // Do not copy suggestions that recommend the copied process
                    "FK_ProcessSuggestion_Process_Suggested",
                    "FK_ProcessSuggestion_ProcessAlias"
                };

                proxy.Remove(w => relationsToRemove.Contains(w.ParentRelation));

                var results = new List<DataRowProxyResults>();
                var process = DataNode<DataRow>.AddPastedDataRows(proxy, _dataset.Process, results) as ProcessesDataset.ProcessRow;

                foreach (var processStep in process.GetProcessStepsRows())
                {
                    foreach (var condition in processStep.GetProcessStepConditionRows())
                    {
                        if (condition.IsProcessQuestionIdNull())
                        {
                            continue;
                        }

                        //need to point back to new processquestionid, not new one
                        var original = results.FirstOrDefault(c => c.Proxy.OriginalPrimaryKey == condition.ProcessQuestionId.ToString());

                        if (original != null)
                            condition.ProcessQuestionRow = original.Row as ProcessesDataset.ProcessQuestionRow;
                    }
                }

                // Map old process aliases to new process aliases.
                // Process revisions use this map to update suggestions.
                var aliasMap = new Dictionary<string, string>();
                foreach (var aliasResult in results.Where(r => r.Proxy.ParentRelation == "FK_ProcessAlias_Process"))
                {
                    var originalAliasId = aliasResult.Proxy.OriginalPrimaryKey;
                    var newAliasId = aliasResult.Row["ProcessAliasID"].ToString();
                    aliasMap[originalAliasId] = newAliasId;
                }

                var node = new ProcessNode(process, aliasMap);

                //Have to add node to correct Folder
                var folderNode = base.Control.Nodes.FindNode<FolderNode>(n => n.DataRow.DepartmentID == process.Department);

                if(folderNode == null)
                {
                    folderNode = new FolderNode(process.d_DepartmentRow);
                    base.Control.Nodes.Add(folderNode);
                }

                //reset frozen state
                node.DataRow.SetFrozenByNull();
                node.DataRow.SetFrozenDateNull();
                node.DataRow.Frozen = false;

                folderNode.Nodes.Add(node);

                return node;
            }

            public bool CanPasteData(string format)
            {
                //Can paste processes only
                return format == typeof(ProcessNode).FullName;
            }

            public string ClipboardDataFormat
            {
                get { return null; }
            }

            #endregion
        }

        #endregion

        #endregion

        #region ProcessNodesSorter

        private class ProcessNodesSorter : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                if(x is ProcessStepNode && y is ProcessStepNode)
                    return ((ProcessStepNode)x).DataRow.StepOrder.CompareTo(((ProcessStepNode)y).DataRow.StepOrder);
                else if(x is ProcessQuestionNode && y is ProcessQuestionNode)
                    return ((ProcessQuestionNode)x).DataRow.StepOrder.CompareTo(((ProcessQuestionNode)y).DataRow.StepOrder);
                else
                    return ((UltraTreeNode)x).Text.CompareTo(((UltraTreeNode)y).Text);
            }

            #endregion
        }

        #endregion

        #region Commands

        #region Nested type: AddCommand

        private class AddCommand : TreeNodeCommandBase
        {
            #region Fields

            private readonly enumNodeType _nodeType;
            public NodeEventHandler AddNode;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get
                {
                    switch(this._nodeType)
                    {
                        case enumNodeType.ProcessNode:
                            return _node is FolderNode || _node is ProcessesRootNode;
                        case enumNodeType.ProcessStepNode:
                            return (_node is ProcessNode && ((ProcessNode)_node).CanDelete) ||
                                   (_node is ProcessStepNode && ((ProcessStepNode)_node).CanDelete) ||
                                   (_node is ProcessQuestionNode && ((ProcessQuestionNode)_node).CanDelete);
                        case enumNodeType.ProcessStepQuestionNode:
                            return (_node is ProcessStepNode && ((ProcessStepNode)_node).CanDelete) ||
                                   (_node is ProcessQuestionNode && ((ProcessQuestionNode)_node).CanDelete);
                        default:
                            return false;
                    }
                }
            }

            #endregion

            #region Methods

            public AddCommand(ToolBase tool, UltraTree toc, enumNodeType nodeType)
                : base(tool)
            {
                this._nodeType = nodeType;
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if(this.AddNode != null)
                    this.AddNode(this._nodeType);
            }

            #endregion
        }

        #endregion

        #region Nested type: ReviseCommand

        private class ReviseCommand : TreeNodeCommandBase
        {
            #region Fields

            public event EventHandlerTemplate<object, ProcessNode> ReviseNode;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _node is ProcessNode; }
            }

            #endregion

            #region Methods

            public ReviseCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                //check to see if process should be revised
                var processNode = _node as ProcessNode;
                if(processNode != null && processNode.UsageCount < 1)
                {
                    string msg = "The current process is not being used by any order, are you sure you want to revise it?";
                    if(MessageBoxUtilities.ShowMessageBoxYesOrNo(msg, "Revise Process") == DialogResult.No)
                        return;
                }

                var currentNode = base._node;

                //test to see if we can change selection
                var parentNode = base._node.Parent;
                parentNode.Select();

                if(base.TreeView.SelectedNodes.Count == 1 && base.TreeView.SelectedNodes[0] == parentNode)
                {
                    base.OnAfterSelect(currentNode);

                    if(this.Enabled && this.ReviseNode != null && _node is ProcessNode)
                    {
                        this.ReviseNode(this, new EventArgsTemplate<ProcessNode>(_node as ProcessNode));
                    }
                }
            }

            public override void Dispose()
            {
                ReviseNode = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region Nested type: FreezeCommand

        private class FreezeCommand : TreeNodeCommandBase
        {
            #region Fields

            private DWOS.UI.Admin.Processes.ProcessInfo _processInfo = null;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get
                {
                    var processNode = _node as ProcessNode;

                    return processNode != null &&
                        processNode.IsRowValid &&
                        !processNode.DataRow.Frozen;
                }
            }

            #endregion

            #region Methods

            public FreezeCommand(ToolBase tool, UltraTree toc, DWOS.UI.Admin.Processes.ProcessInfo processInfo)
                : base(tool)
            {
                base.TreeView = toc;
                _processInfo = processInfo;
            }

            public override void OnClick()
            {
                try
                {
                    using (var form = new TextBoxForm())
                    {
                        form.Text                  = "Freeze Process";
                        form.FormLabel.Text        = "Frozen By:";
                        form.FormTextBox.MaxLength = _processInfo.Dataset.Process.FrozenByColumn.MaxLength;

                        if (form.ShowDialog() == DialogResult.OK && !String.IsNullOrWhiteSpace(form.FormTextBox.Text))
                        {
                            var name = form.FormTextBox.Text.Trim().Replace("'", "*");
                            _processInfo.Freeze(name);
                        }
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error freezing process.");
                }
            }

            public override void Dispose()
            {
                _processInfo = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region Nested type: ExportCommand

        private class ExportCommand : CopyCommand
        {
            #region Fields

            private DWOS.UI.Admin.Processes.ProcessInfo _processInfo = null;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get
                {
                    var processNode = _node as ProcessNode;

                    return processNode != null &&
                        processNode.IsRowValid;
                }
            }

            #endregion

            #region Methods

            public ExportCommand(ToolBase tool, UltraTree toc, DWOS.UI.Admin.Processes.ProcessInfo processInfo)
                : base(tool, toc)
            {
                base.TreeView = toc;
                _processInfo = processInfo;
            }

            public override void OnClick()
            {
                try
                {
                    using(var fileDialg = new SaveFileDialog())
                    {
                        var initialDirectory = System.IO.Path.Combine(FileSystem.UserDocumentPath(), "Processes");
                        if(System.IO.Directory.Exists(initialDirectory))
                            System.IO.Directory.CreateDirectory(initialDirectory);

                        fileDialg.AddExtension = true;
                        fileDialg.Title = "Export Process";
                        fileDialg.Filter = "Process Configuration (*.process)|*.process";
                        fileDialg.OverwritePrompt = true;
                        fileDialg.InitialDirectory = initialDirectory;
                        fileDialg.FileName = _node.Text + ".process";

                        if(fileDialg.ShowDialog() == DialogResult.OK)
                        {
                            var process = ((ProcessNode) _node).DataRow;
                            var processExport = new ProcessExport();
                            processExport.Fill(process);

                            if(System.IO.File.Exists(fileDialg.FileName))
                                System.IO.File.Delete(fileDialg.FileName);

                            var json = JsonConvert.SerializeObject(processExport, Formatting.Indented, new JsonSerializerSettings() {DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore});
                            System.IO.File.WriteAllText(fileDialg.FileName, json);

                            MessageBoxUtilities.ShowMessageBoxOK("Succesfully exported process {0}.".FormatWith(process.Name), "Export Process");
                        }
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error freezing process.");
                }
            }

            public override void Dispose()
            {
                _processInfo = null;
                base.Dispose();
            }

            #endregion
        }
        
        #endregion

        #region Nested type: ImportCommand

        private class ImportCommand : TreeNodeCommandBase
        {
            #region Fields

            private DWOS.UI.Admin.ProcessManager _processManager = null;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return true; }
            }

            #endregion

            #region Methods

            public ImportCommand(ToolBase tool, UltraTree toc, DWOS.UI.Admin.ProcessManager processManager)
                : base(tool)
            {
                base.TreeView = toc;
                _processManager = processManager;
            }

            public override void OnClick()
            {
                try
                {
                    string fileName = string.Empty;
                    using(var fileDialog = new OpenFileDialog())
                    {
                        var initialDirectory = System.IO.Path.Combine(FileSystem.UserDocumentPath(), "Processes");
                        if(System.IO.Directory.Exists(initialDirectory))
                            System.IO.Directory.CreateDirectory(initialDirectory);

                        fileDialog.Title = "Import Process";
                        fileDialog.Filter = "Process Configuration (*.process)|*.process";
                        fileDialog.InitialDirectory = initialDirectory;

                        if (fileDialog.ShowDialog() == DialogResult.OK && System.IO.File.Exists(fileDialog.FileName))
                        {
                            fileName = fileDialog.FileName;
                        }
                    }

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var processesNode = TreeView.Nodes.OfType<ProcessesRootNode>().FirstOrDefault();

                        var json = System.IO.File.ReadAllText(fileName);
                        var processExport = JsonConvert.DeserializeObject<ProcessExport>(json);
                        var processImporter = new ProcessImporter(_processManager.dsProcesses, processExport);
                        var processRow = processImporter.Import();

                        var isImportCancelled = false;

                        if (processRow != null)
                        {
                            if (_processManager.dsProcesses.d_Department.FindByDepartmentID(processRow.Department) == null)
                            {
                                // Prompt user for department
                                var selectDepartmentDialog = new ProcessImportDepartmentDialog();
                                var helper = new WindowInteropHelper(selectDepartmentDialog)
                                {
                                    Owner = _processManager.Handle
                                };

                                var departments = _processManager.dsProcesses.d_Department
                                    .Select(dept => dept.DepartmentID);

                                selectDepartmentDialog.LoadData(
                                    processRow.Name,
                                    processRow.Department,
                                    departments);

                                if (selectDepartmentDialog.ShowDialog() ?? false)
                                {
                                    if (selectDepartmentDialog.CreateNewDepartment)
                                    {
                                        _processManager.dsProcesses.d_Department.Addd_DepartmentRow(processRow.Department, true, null);
                                        processImporter.Issues.Add($"Added new department: {processRow.Department}");
                                    }
                                    else
                                    {
                                        processRow.Department = selectDepartmentDialog.SelectedDepartment;
                                    }
                                }
                                else
                                {
                                    _log.Info("Canceling import.");
                                    isImportCancelled = true;
                                    processRow.Delete();
                                }
                            }

                            if (!isImportCancelled)
                            {
                                var deptNode = processesNode.Nodes.FindNode<FolderNode>(f => f.IsRowValid && f.DataRow.DepartmentID == processRow.Department);

                                if (deptNode == null)
                                {
                                    deptNode = new FolderNode(processRow.d_DepartmentRow);
                                    processesNode.Nodes.Add(deptNode);
                                }

                                var processNode = new ProcessNode(processRow);
                                deptNode.Nodes.Add(processNode);
                                processNode.Select();
                            }
                        }

                        if (isImportCancelled)
                        {
                            MessageBoxUtilities.ShowMessageBoxOK("Canceled Import.", "Process Import");
                        }
                        else if (processImporter.Issues.Count > 0)
                        {
                            using(var form = new TextBoxForm())
                            {
                                form.Text = "Import Issues";
                                form.FormLabel.Text = "Issues:";
                                processImporter.Issues.ForEach(t => form.FormTextBox.AppendText(t + Environment.NewLine));
                                form.MakeMultiline(300, 400);
                                form.ShowDialog();
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    _log.Error(exc, "Error importing process.");
                }
            }

            public override void Dispose()
            {
                _processManager = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region Nested type: PrintProcessSheetCommand

        private class PrintProcessSheetCommand : TreeNodeCommandBase
        {
            #region Properties

            public override bool Enabled
            {
                get { return _node is ProcessNode; }
            }

            #endregion

            #region Methods

            public PrintProcessSheetCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                try
                {
                    var process = _node as ProcessNode;
                    if (process != null)
                    {
                        var report = new DWOS.Reports.ProcessSheetReport(process.DataRow);
                        report.DisplayReport();
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error printing process sheet.");
                }
            }

            #endregion
        }

        #endregion

        #region ProcessSearchCommand

        private class ProcessSearchCommand : TreeNodeCommandBase
        {
            #region Fields

            private readonly ProcessManager _processManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get
                {
                    return base.TreeView != null && base.TreeView.Nodes.Count > 0;
                }
            }

            #endregion

            #region Methods

            public ProcessSearchCommand(ToolBase tool, UltraTree toc, ProcessManager processManager)
                : base(tool)
            {
                base.TreeView = toc;
                _processManager = processManager;
            }

            public override void OnClick()
            {
                try
                {
                    using (var frm = new ProcessSearch())
                    {
                        if (frm.ShowDialog(_processManager) == DialogResult.OK)
                        {
                            _processManager.GoToProcess(frm.SelectedProcess);
                        }
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error running process search command.");
                }
            }

            #endregion
        }

        #endregion

        #endregion
    }
}