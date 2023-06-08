using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;
using DWOS.Shared;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class ListEditor: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly ValidatorManager _validators = new ValidatorManager();
        private readonly ISet<int> _listIDsToKeep = new HashSet<int>();

        private int _lastSelectedListID = -1;
        private SecurityFormWatcher _watcher;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the currently selected list ID.
        /// </summary>
        /// <value> The selected list ID. </value>
        public int SelectedListID
        {
            get { return this._lastSelectedListID; }
        }

        #endregion

        #region Methods

        public ListEditor()
        {
            this.InitializeComponent();

            this._watcher = new SecurityFormWatcher(this, this.btnCancel);
        }

        public void DoNotAllowDeletionOf(IEnumerable<int> listIDs)
        {
            if (listIDs == null)
            {
                return;
            }

            foreach (var listID in listIDs)
            {
                _listIDsToKeep.Add(listID);
            }
        }

        private void LoadData()
        {
            this.dsProcesses.EnforceConstraints = false;
            this.dsProcesses.Lists.BeginLoadData();
            this.dsProcesses.ListValues.BeginLoadData();

            this.taLists.Fill(this.dsProcesses.Lists);
            this.taListValues.Fill(this.dsProcesses.ListValues);
            
            this.dsProcesses.Lists.EndLoadData();
            this.dsProcesses.ListValues.EndLoadData();
            this.dsProcesses.EnforceConstraints = true;
        }

        private void LoadTOC()
        {
            this.tvwTOC.Nodes.Clear();

            UltraTreeNode rootNode = new ListsRootNode();
            this.tvwTOC.Nodes.Add(rootNode);
            rootNode.Expanded = true;

            foreach(ProcessesDataset.ListsRow row in this.dsProcesses.Lists)
            {
                if(row.ListID != 4) //hide <None> list to prevent user from deleting it
                    rootNode.Nodes.Add(new ListNode(row));
            }
        }

        private void LoadValidators()
        {
            this._validators.Add(new ImageDisplayValidator(new TextControlValidator(this.txtListName, "List name required."), this.errProvider));
        }

        private void LoadCommands()
        {
            if(SecurityManager.Current.IsInRole("ListManager.Edit"))
            {
                var add = new AddCommand(this.toolbarManager.Tools["Add"], this.tvwTOC);
                add.AddNode = this.AddList;

                new DeleteListItemCommand(this.toolbarManager.Tools["Delete"], this.tvwTOC, this);
            }
        }

        private void SaveData()
        {
            try
            {
                this.bsLists.EndEdit();

                this.taManager.UpdateAll(dsProcesses);
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error saving data.", exc);
            }
        }

        private void AddList(object sender, EventArgs e)
        {
            _log.Info("Adding a list.");
            var rn = this.tvwTOC.Nodes[0] as ListsRootNode;

            //create new data source
            var rowVw = this.bsLists.AddNew() as DataRowView;
            var cr = rowVw.Row as ProcessesDataset.ListsRow;
            this.txtListName.Text = cr.Name = "New List";

            //create new ui nodes
            var cn = new ListNode(cr);
            rn.Nodes.Add(cn);

            //select new nodes
            cn.Select();

            this.txtListName.SelectAll();
            this.txtListName.Focus();
            this.grpProcessInformation.Enabled = true;

            this.bsLists.Position = this.bsLists.Find(this.dsProcesses.Lists.ListIDColumn.ColumnName, cr.ListID);
        }

        private void AddListValue(ListNode ln)
        {
            _log.Info("Adding a list value.");

            //create new data source
            ProcessesDataset.ListValuesRow cr = this.dsProcesses.ListValues.NewListValuesRow();
            cr.ListID = ln.DataRow.ListID;
            cr.Value = "New Item";

            //create new ui nodes
            UltraListViewItem item = this.CreateListItem(cr);
            item.Value = cr.Value;
            this.lvwValues.Items.Add(item);
            this.lvwValues.SelectedItems.Add(item, true);

            this.dsProcesses.ListValues.Rows.Add(cr);

            //select new nodes
            item.BringIntoView();
            item.BeginEdit();
        }

        private bool ValidateChanges(out bool cancel)
        {
            cancel = false;

            if(this.tvwTOC.SelectedNodes.Count > 0 && !this.tvwTOC.SelectedNodes[0].IsEditing)
            {
                cancel = !this._validators.ValidateControls();

                //if not cancelling, tell node to update its self before selecting new node
                if(!cancel)
                {
                    if(this.tvwTOC.SelectedNodes[0] is IDataNode)
                        ((IDataNode)this.tvwTOC.SelectedNodes[0]).UpdateNodeUI();
                }
            }

            return cancel;
        }

        private void LoadNode(ListNode node)
        {
            this.grpProcessInformation.Enabled = true;

            //filter contacts by customer
            this.bsLists.Position = this.bsLists.Find(this.dsProcesses.Lists.ListIDColumn.ColumnName, node.ID);
            this.txtListName.Focus();

            //load list view with values
            this.lvwValues.Items.Clear();
            ProcessesDataset.ListValuesRow[] rows = node.DataRow.GetListValuesRows();

            foreach(ProcessesDataset.ListValuesRow row in rows)
                this.lvwValues.Items.Add(this.CreateListItem(row));
        }

        private UltraListViewItem CreateListItem(ProcessesDataset.ListValuesRow row)
        {
            var item = new UltraListViewItem(row.ListValueID.ToString());
            item.Value = row.Value;
            item.Tag = row;
            return item;
        }

        private bool IsInUse(ProcessesDataset.ListsRow listsRow)
        {
            if (_listIDsToKeep.Contains(listsRow.ListID))
            {
                return true;
            }

            var listUsage = taLists.GetUsageCount(listsRow.ListID).GetValueOrDefault();
            var partInspectionListUsage = taPartInspectionLists.GetUsageCount(listsRow.ListID).GetValueOrDefault();
            return listUsage > 0 || partInspectionListUsage > 0;
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                //determine what last selected list was before changing selection in preperation for a save
                if(this.tvwTOC.SelectedNodes.Count == 1 && this.tvwTOC.SelectedNodes[0] is ListNode)
                    this._lastSelectedListID = ((ListNode)this.tvwTOC.SelectedNodes[0]).DataRow.ListID;

                bool cancel;
                this.ValidateChanges(out cancel);
                this.tvwTOC.Nodes[0].Select(); //change selection to root node

                if(!cancel)
                {
                    this.SaveData();
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
            Close();
        }

        private void ListEditor_Load(object sender, EventArgs e)
        {
            try
            {
                //Commands
                this.LoadCommands();

                this.bsLists.SuspendBinding();

                //Load all data
                this.LoadData();
                this.LoadTOC();
                this.LoadValidators();

                this.bsLists.ResumeBinding();

                //select first node customer node
                if(this.tvwTOC.Nodes[0].Nodes.Count > 0)
                    this.tvwTOC.ActiveNode = this.tvwTOC.Nodes[0].Nodes[0];

                this.tvwTOC.PerformAction(UltraTreeAction.SelectActiveNode, false, false);

                this.pnlInfo.Enabled = SecurityManager.Current.IsInRole("ListManager");
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
                _log.Fatal(exc, "Error loading form.");
            }
        }

        private void tvwTOC_AfterSelect(object sender, SelectEventArgs e)
        {
            try
            {
                if(e.NewSelections.Count == 1 && e.NewSelections[0] is ListNode)
                {
                    this.LoadNode(e.NewSelections[0] as ListNode);
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
                //ensure that editing is ended on current list item to ensure udpated
                if(this.lvwValues.ActiveItem != null && this.lvwValues.ActiveItem.IsInEditMode)
                    this.lvwValues.ActiveItem.EndEdit(false);

                bool cancel;
                this.ValidateChanges(out cancel);

                e.Cancel = cancel;

                //refresh sort before moving to next selection
                //if(e.NewSelections.Count > 0 && e.NewSelections[0].Parent != null)
                //	tvwTOC.RefreshSort(e.NewSelections[0].Parent.Nodes, false);
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error processing selection.", exc);
                _log.Fatal(exc, "Error processing selection.");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(this.tvwTOC.SelectedNodes.Count == 1 && this.tvwTOC.SelectedNodes[0] is ListNode)
                this.AddListValue(this.tvwTOC.SelectedNodes[0] as ListNode);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(this.lvwValues.Items.Count > 0 && this.lvwValues.SelectedItems.Count > 0)
            {
                while(this.lvwValues.SelectedItems.Count > 0)
                {
                    UltraListViewItem item = this.lvwValues.SelectedItems[0];
                    if(item.Tag is DataRow)
                        ((DataRow)item.Tag).Delete();

                    this.lvwValues.Items.Remove(item);
                    item.Dispose();
                }
            }
        }

        private void lvwValues_ItemExitedEditMode(object sender, ItemExitedEditModeEventArgs e)
        {
            //update text of underlying data row
            if(e.Item != null && e.Item.Tag is ProcessesDataset.ListValuesRow)
                ((ProcessesDataset.ListValuesRow)e.Item.Tag).Value = e.Item.Text;
        }

        #endregion

        #region Nodes

        #region Nested type: ListNode

        internal class ListNode: DataNode<ProcessesDataset.ListsRow>
        {
            public const string KEY_PREFIX = "LS";

            #region Methods

            public ListNode(ProcessesDataset.ListsRow cr)
                : base(cr, cr.ListID.ToString(), KEY_PREFIX, cr.Name)
            {
                //this.LeftImages.Add(UI.Properties.Resources.Gear);

                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                Text = base.DataRow.Name;
            }

            #endregion
        }

        #endregion

        #region Nested type: ListsRootNode

        internal class ListsRootNode: UltraTreeNode
        {
            #region Methods

            public ListsRootNode()
                : base("ROOT", "Lists")
            {
                //this.LeftImages.Add(UI.Properties.Resources.Gears);
            }

            #endregion
        }

        #endregion

        #endregion

        #region Commands

        internal class AddCommand: TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddNode;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _node is ListsRootNode; }
            }

            #endregion

            #region Methods

            public AddCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if(_node is ListsRootNode)
                {
                    if(this.AddNode != null)
                        this.AddNode(this, EventArgs.Empty);
                }
            }

            #endregion
        }

        /// <summary>
        /// Override of delete command that does not allow deleting lists used
        /// by processes or inspections.
        /// </summary>
        internal class DeleteListItemCommand : DeleteCommand
        {
            #region Fields

            private readonly Dictionary<ProcessesDataset.ListsRow, bool> _inUseCache =
                new Dictionary<ProcessesDataset.ListsRow, bool>();

            #endregion

            #region Properties

            public ListEditor EditorInstance
            {
                get;
            }

            #endregion

            #region Methods

            public DeleteListItemCommand(ToolBase tool, UltraTree toc, ListEditor editorInstance)
                : base(tool, toc, null)
            {
                base.TreeView = toc;
                this.EditorInstance = editorInstance;
            }

            public override void OnClick()
            {
                if (_node == null)
                    return;

                UltraTreeNode parentNode = _node.Parent;

                if (parentNode == null || parentNode.Control == null)
                    return;
                
                var nodesToDelete = new List<IDeleteNode>();

                //Check to make sure list item is not being used in processes or part inspection
                foreach (UltraTreeNode node in base.TreeView.SelectedNodes)
                {
                    var listNode = node as ListNode;
                    if (listNode != null && listNode.DataRow != null)
                    {
                        var listsRow = listNode.DataRow;
                        if (!_inUseCache.ContainsKey(listsRow))
                        {
                            _inUseCache.Add(listsRow, EditorInstance.IsInUse(listsRow));
                        }

                        if (_inUseCache[listsRow])
                        {
                            MessageBoxUtilities.ShowMessageBoxWarn("Unable to delete lists that are in use.", "Unable to Delete");
                        }
                        else
                        {
                            base.OnClick();
                        }
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}