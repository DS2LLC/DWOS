using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Shared;
using DWOS.UI.Utilities;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using NLog;
using DWOS.UI.Reports;
using DWOS.Data;
using DWOS.Reports;

namespace DWOS.UI.Tools
{
    internal class DeleteButtonCommand : TreeNodeCommandBase
    {
        #region Fields

        public Action <IDeleteNode> AfterDelete;

        #endregion

        #region Properties

        public override bool Enabled =>
            _node is IDeleteNode deleteNode && (deleteNode).CanDelete;

        #endregion

        #region Methods

        public DeleteButtonCommand(UltraButton button, UltraTree toc) : base(button)
        {
            TreeView = toc;
            KeyMapping = Keys.Delete;
        }

        public override void OnClick()
        {
            if (_node == null)
            {
                return;
            }

            UltraTreeNode parent = _node.Parent;
            UltraTreeNode _tempNode = _node;

            _tempNode.BeginEdit(); //signal to not cause validation
            parent?.Select();
            _tempNode.EndEdit(false);

            if (_tempNode is IDeleteNode deleteNode)
            {
                deleteNode.Delete();
                AfterDelete?.Invoke(deleteNode);
            }
        }

        #endregion
    }

    internal class AddNodeCommand<T> : TreeNodeCommandBase where T : UltraTreeNode
    {
        #region Fields

        public Action <T> AddNode;
        private DataEditorBase _editor;

        #endregion

        #region Properties

        public override bool Enabled => _node is T && AddNode != null;

        #endregion

        #region Methods

        public AddNodeCommand(ToolBase tool, DataEditorBase editor, UltraTree toc) : base(tool)
        {
            TreeView = toc;
            _editor = editor;
        }

        public override void OnClick()
        {
            if(Enabled && (_editor == null || _editor.IsValidControls()))
                AddNode(_node as T);
        }

        public override void Dispose()
        {
            AddNode = null;
            _editor = null;

            base.Dispose();
        }

        #endregion
    }

    internal class DeleteCommand : TreeNodeCommandBase
    {
        #region Fields

        /// <summary>
        ///     Determines if the item should be deleted.
        /// </summary>
        public delegate bool CancelDeleteEventHandler(List <IDeleteNode> itemsToDelete);

        private readonly IDataEditor _editor;

        /// <summary>
        ///     Occurs when [cancel command].
        /// </summary>
        public event CancelDeleteEventHandler CancelCommand;

        /// <summary>
        ///     Occurs when item is deleted.
        /// </summary>
        public event EventHandler <NodeDeletedEventArgs> ItemDeleted;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get
            {
                //if not authorized then not enabled
                if (!IsAuthorized() || TreeView == null)
                    return false;
                
                //enabled if all items can be deleted
                return TreeView.SelectedNodes
                    .OfType<IDeleteNode>()
                    .All(node => node.CanDelete);
            }
        }

        #endregion

        #region Methods

        public DeleteCommand(ToolBase tool, UltraTree toc, IDataEditor editor, string securityRole = null)
            : base(tool, securityRole)
        {
            _editor = editor;
            TreeView = toc;
            KeyMapping = Keys.Delete;
            AllowMultipleSelection = true;
        }

        public DeleteCommand(UltraButton tool, UltraTree toc, IDataEditor editor, string securityRole = null)
            : base(tool,securityRole)
        {
            _editor = editor;
            TreeView = toc;
            KeyMapping = Keys.Delete;
            AllowMultipleSelection = true;
        }

        public override void OnClick()
        {
            try
            {
                if (_node == null)
                    return;

                _log.Debug("In Delete Command Onclick.");

                UltraTreeNode parentNode = _node.Parent;

                if (parentNode == null || parentNode.Control == null)
                    return;
                
                var nodesToDelete = new List<IDeleteNode>();

                foreach (UltraTreeNode node in TreeView.SelectedNodes)
                {
                    if (node is IDeleteNode deleteNode && (deleteNode).CanDelete)
                    {
                        nodesToDelete.Add((IDeleteNode) node);
                    }
                }

                using (new UsingTreeLoad(parentNode.Control))
                {
                    bool cancel = false;

                    //if anyone listening, check to see if should cancel the delete
                    if (CancelCommand != null)
                        cancel = CancelCommand(nodesToDelete);

                    //if not canceling
                    if (!cancel)
                    {
                        if (_editor != null)
                        {
                            _editor.SuspendValidation = true;
                            if (_editor.ActivePanel != null)
                                _editor.ActivePanel.CancelEdits();
                                    //cancel current edits because we are about to delete it anyways
                        }

                        parentNode.Select();

                        foreach (var node in nodesToDelete)
                        {
                            node.Delete();

                            ItemDeleted?.Invoke(this, new NodeDeletedEventArgs { Node = node });
                        }

                        if (_editor != null)
                            _editor.SuspendValidation = false;
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error deleting nodes.");
            }
        }

        #endregion
    }

    internal class CopyCommand : TreeNodeCommandBase
    {
        #region Properties

        public override bool Enabled => _node is IDataNode
            && _node is ICopyPasteNode copyPasteNode
            && !string.IsNullOrEmpty(copyPasteNode.ClipboardDataFormat);

        #endregion

        #region Methods

        public CopyCommand(ToolBase tool, UltraTree toc) : base(tool)
        {
            TreeView = toc;
            KeyMapping = Keys.Control & Keys.C;
        }

        public override void OnClick()
        {
            if(Enabled)
                CopyNode(_node as ICopyPasteNode);
        }

        /// <summary>
        ///     Copies the nodes.
        /// </summary>
        /// <param name="dataNode"> The data node. </param>
        public static void CopyNode(ICopyPasteNode dataNode)
        {
            if(dataNode is IDataNode)
            {
                DataRowProxy copiedDataRow = CopyRows(((IDataNode) dataNode).DataRow);

                if(copiedDataRow != null)
                {
                    InMemoryClipboard.ClipboardFormat = dataNode.ClipboardDataFormat;
                    InMemoryClipboard.ClipboarObject = copiedDataRow;
                }
            }
        }

        public static DataRowProxy CopyRows(DataRow current)
        {
            DataRowProxy proxyCurrent = CopyRow(current);

            foreach(DataRelation rel in current.Table.ChildRelations)
            {
                foreach(DataRow child in current.GetChildRows(rel))
                {
                    DataRowProxy proxyChild = CopyRows(child);
                    proxyChild.ParentRelation = rel.RelationName;
                    proxyCurrent.ChildProxies.Add(proxyChild);
                }
            }

            return proxyCurrent;
        }

        private static DataRowProxy CopyRow(DataRow dr)
        {
            object[] newRowValues = dr.ItemArray;
            var columnNames = new List <string>(dr.Table.Columns.Count);

            foreach(DataColumn dc in dr.Table.Columns)
            {
                columnNames.Add(dc.ColumnName);
                if(dc.AutoIncrement)
                    newRowValues[dc.Ordinal] = null;
            }

            return new DataRowProxy(newRowValues)
            {
                OriginalPrimaryKey = dr.PrimaryKey(),
                ColumnNameArray = columnNames.ToArray()
            };
        }

        #endregion

        #region InMemoryClipboard Class

        public static class InMemoryClipboard
        {
            public static object ClipboarObject;
            public static string ClipboardFormat;
        }

        #endregion
    }

    internal class PasteCommand : TreeNodeCommandBase
    {
        #region Properties

        public override bool Enabled => _node is ICopyPasteNode copyPasteNode
            && !string.IsNullOrEmpty(CopyCommand.InMemoryClipboard.ClipboardFormat)
            && copyPasteNode.CanPasteData(CopyCommand.InMemoryClipboard.ClipboardFormat);

        #endregion

        #region Methods

        public PasteCommand(ToolBase tool, UltraTree toc) : base(tool)
        {
            TreeView = toc;
            KeyMapping = Keys.Control & Keys.V;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                PasteNode((ICopyPasteNode) _node);

                //refresh tool UI
                Refresh();
            }
        }

        public static UltraTreeNode PasteNode(ICopyPasteNode parentNode)
        {
            string format = CopyCommand.InMemoryClipboard.ClipboardFormat;
            var dr = CopyCommand.InMemoryClipboard.ClipboarObject as DataRowProxy;
            UltraTreeNode pastedNode = null;

            if(dr != null)
            {
                pastedNode = (parentNode).PasteData(format, dr);

                if(pastedNode != null)
                    pastedNode.Select();
            }

            //clear clipboard [more difficult to paste same row multiple times as PK of row does not change causing duplicate key errors]
            CopyCommand.InMemoryClipboard.ClipboardFormat = null;
            CopyCommand.InMemoryClipboard.ClipboarObject = null;

            return pastedNode;
        }

        #endregion
    }

    internal class CopyPasteCommand : TreeNodeCommandBase
    {
        #region Fields

        private CopyCommand _copy;
        private PasteCommand _paste;

        #endregion

        #region Properties

        public override bool Enabled => _node is IDataNode
            && _node is ICopyPasteNode
            && !string.IsNullOrEmpty(((ICopyPasteNode)_node).ClipboardDataFormat)
            && (SecurityRole == null || SecurityManager.Current.IsInRole(SecurityRole));

        #endregion

        #region Methods

        public CopyPasteCommand(ToolBase tool, UltraTree toc, string securityRole = null) : base(tool, securityRole)
        {
            TreeView = toc;

            _copy = new CopyCommand(null, null);
            _paste = new PasteCommand(null, null);
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            UltraTreeNode currentNode = _node;

            //test to see if we can change selection
            UltraTreeNode root = TreeView.Nodes[0];
            root.Select();

            if(TreeView.SelectedNodes.Count == 1 && TreeView.SelectedNodes[0] == root)
            {
                _copy.OnAfterSelect(currentNode);
                if(_copy.Enabled)
                {
                    _copy.OnClick();

                    _paste.OnAfterSelect(currentNode.Parent);

                    if(_paste.Enabled)
                        _paste.OnClick(); //select the current node
                }
            }
        }

        public override void Dispose()
        {
            if(_copy != null)
                _copy.Dispose();
            _copy = null;

            if(_paste != null)
                _paste.Dispose();
            _paste = null;

            base.Dispose();
        }

        #endregion
    }

    internal class PrintNodeCommand : TreeNodeCommandBase
    {
        #region Fields

        public event Action<IReportNode, string, bool> BeforePrinted;
        public event Action<IReportNode, string, bool> AfterPrinted;

        /// <summary>
        /// If a node has only a single report, and that report shares a name
        /// with a name in this list, show the print dialog.
        /// </summary>
        private readonly List<string> _alwaysShowPrintDialogFor = new List<string>
        {
            "Container Label",
            "Hold Label"
        };

        #endregion

        #region Properties

        public override bool Enabled => _node is IReportNode;

        public bool ValidateRowUnchangedBeforePrinting { get; set; }

        #endregion

        #region Methods

        public PrintNodeCommand(ToolBase tool, UltraTree toc) : base(tool)
        {
            TreeView = toc;
            ValidateRowUnchangedBeforePrinting = true;
            KeyMapping = Keys.Print;

            AllowMultipleSelection = true;
        }

        public override void OnClick()
        {
            bool quickPrint = false;

            try
            {
                var reportNodes = new List <IReportNode>();

                foreach(UltraTreeNode item in TreeView.SelectedNodes)
                {
                    if (item is IReportNode reportNode)
                    {
                        reportNodes.Add(reportNode);
                    }
                }

                reportNodes.Sort(new ReportSorter());

                if(reportNodes.Count > 0)
                {
                    //ensure row has saved all data before printing
                    if(ValidateRowUnchangedBeforePrinting)
                    {
                        bool isDataValid = true;

                        foreach(IReportNode item in reportNodes)
                        {
                            if(item is IDataNode && ((IDataNode) item).HasChanges)
                            {
                                isDataValid = false;
                                break;
                            }
                        }

                        if(!isDataValid)
                        {
                            MessageBoxUtilities.ShowMessageBoxWarn("The selected item(s) has changes that need to be saved before running the report. Please apply any changes before printing.", "Print Warning");
                            return;
                        }
                    }

                    //find all distinct report types
                    var reportTypes = new List <string>();
                    foreach(IReportNode item in reportNodes)
                    {
                        string[] nodeReportTypes = item.ReportTypes();

                        if(nodeReportTypes != null)
                        {
                            foreach(string rt in nodeReportTypes)
                            {
                                if(!reportTypes.Contains(rt))
                                    reportTypes.Add(rt);
                            }
                        }
                    }

                    string selectedReport = null;

                    //Added for container quick print label options per #14613 and #14998
                    const string defaultReport = "Default";

                    if (reportTypes.Count == 0)
                    {
                        selectedReport = defaultReport;
                    }
                    else if (reportTypes.Count == 1)
                    {
                        selectedReport = reportTypes[0];
                    }
                    
                    if (reportTypes.Count > 1 || _alwaysShowPrintDialogFor.Contains(selectedReport))
                    {
                        // Show 'select report' dialog
                        using (var cbo = new ComboBoxForm())
                        {
                            cbo.chkOption.Visible = true;
                            cbo.chkOption.Text = "Quick Print";
                            cbo.chkOption.Checked = UserSettings.Default.QuickPrint;
                            cbo.Text = "Reports";

                            for (int i = 0; i < reportTypes.Count; i++)
                                cbo.ComboBox.Items.Add(reportTypes[i]);

                            cbo.ComboBox.SelectedIndex = 0;
                            cbo.FormLabel.Text = "Report Type:";

                            if (cbo.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                            {
                                selectedReport = cbo.ComboBox.Text;
                                quickPrint = cbo.chkOption.Checked;
                                UserSettings.Default.QuickPrint = quickPrint;
                                UserSettings.Default.Save();
                            }
                            else
                                return; // canceled dialog
                        }
                    }

                    foreach(var rn in reportNodes)
                    {
                        var currentReportNode = rn;
                        BeforePrinted?.Invoke(currentReportNode, selectedReport, quickPrint);

                        // Print reports synchronously; otherwise, could print
                        // in wrong order or attempt to print at the same time.
                        var report = currentReportNode.CreateReport(selectedReport);

                        if (quickPrint)
                        {
                            report?.PrintReport();
                        }
                        else
                        {
                            report?.DisplayReport();
                        }

                        AfterPrinted?.Invoke(currentReportNode, selectedReport, quickPrint);

                        Application.DoEvents();
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error printing report.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }

    /// <summary>
    ///     Refresh node visibility based on if the node is IActive
    /// </summary>
    internal class DisplayInactiveCommand : TreeNodeCommandBase
    {
        #region Fields

        public event EventHandler BeforeClick;

        #endregion

        #region Properties

        public override bool Enabled => TreeView != null && TreeView.Nodes.Count > 0;

        #endregion

        #region Methods

        public DisplayInactiveCommand(StateButtonTool tool, UltraTree toc) : base(tool) { TreeView = toc; }

        public override void OnClick()
        {
            try
            {
                BeforeClick?.Invoke(this, EventArgs.Empty);

                bool showAll = ((StateButtonTool) Button.Button).Checked;

                UltraTreeNode rootNode = TreeView.Nodes[0];

                if(rootNode != null)
                {
                    foreach(UltraTreeNode node in rootNode.Nodes)
                    {
                        if (node is IActive userNode)
                        {
                            node.Visible = (showAll || userNode.IsActiveData);
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating visibility of nodes.");
            }
        }

        #endregion
    }

    internal class DeactivateNodeCommand : TreeNodeCommandBase
    {
        #region Properties

        public override bool Enabled => _node is IActive activeNode && (activeNode).IsActiveData;

        #endregion

        #region Methods

        public DeactivateNodeCommand(ToolBase tool, UltraTree toc) : base(tool) { TreeView = toc; }

        public override void OnClick()
        {
            try
            {
                if (Enabled && _node is IActive userNode)
                {
                    userNode.IsActiveData = false;
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating visibility of nodes.");
            }
        }

        #endregion
    }
}