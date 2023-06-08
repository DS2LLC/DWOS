using System;
using System.Data;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.DocumentsDataSetTableAdapters;
using DWOS.Shared.Utilities;
using DWOS.UI.Documents;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinTree;
using NLog;

namespace DWOS.Documents.Controls
{
    public partial class FolderTOC : UserControl
    {
        #region Fields

        private UltraTreeDragAndDropHelper _dragAndDrop;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private DocumentsDataSet _dsDocuments;
        public event EventHandler <FolderSelectedEventArgs> AfterFolderSelected;

        #endregion

        #region Properties

        public DocumentsDataSet.DocumentFolderRow SelectedFolder =>
            SelectedFolderNode?.FolderRow;

        public FolderNode SelectedFolderNode => tvwFolders.SelectedNodes.Count == 1
            ? tvwFolders.SelectedNodes[0] as FolderNode
            : null;

        #endregion

        #region Methods

        public FolderTOC()
        {
            InitializeComponent();

            _dragAndDrop = new UltraTreeDragAndDropHelper(tvwFolders) {AllowOnlyNodeDragNDrop =  true};
            _dragAndDrop.QueryValidDropOn = QueryValidDropOn;
            _dragAndDrop.BeforeDrop    += dragAndDrop_BeforeDrop;
            _dragAndDrop.AfterDrop     += dragAndDrop_AfterDrop;
        }

        public void InitialLoad(DocumentsDataSet dsDocuments)
        {
            _dsDocuments = dsDocuments;

            //if not loaded already then load the folders
            if (_dsDocuments.DocumentFolder.Count == 0)
            {
                using(new UsingDataSetLoad(_dsDocuments))
                {
                    using(var ta = new SecurityGroupTableAdapter())
                        ta.Fill(_dsDocuments.SecurityGroup);
                    using(var ta = new DocumentFolder_SecurityGroupTableAdapter())
                        ta.FillByDeleted(_dsDocuments.DocumentFolder_SecurityGroup, false);
                    using(var ta = new UsersTableAdapter())
                        ta.Fill(_dsDocuments.Users);

                    taDocumentFolder.Fill(_dsDocuments.DocumentFolder);
                }
            }

            // Load nodes
            tvwFolders.Nodes.Clear();

            foreach (var rootFolder in _dsDocuments.DocumentFolder.Where(df => df.IsParentIDNull()))
            {
                var folderNode = new FolderNode(rootFolder);
                tvwFolders.Nodes.Add(folderNode);
                LoadNodes(folderNode);
            }
        }

        private void LoadNodes(FolderNode folder)
        {
            foreach (var childFolder in _dsDocuments.DocumentFolder.Where(df => !df.IsParentIDNull() && df.ParentID == folder.FolderRow.DocumentFolderID))
            {
                var childFolderNode = new FolderNode(childFolder);
                folder.Nodes.Add(childFolderNode);
                LoadNodes(childFolderNode);
            }
        }

        internal DocumentsDataSet.DocumentFolderRow AddNewFolder(string folderName = "New Folder")
        {
            _log.Info("Adding new folder " + folderName);

            FolderNode currentFolder = SelectedFolderNode;
            DocumentsDataSet.DocumentFolderRow folderRow = _dsDocuments.DocumentFolder.AddDocumentFolderRow(folderName, currentFolder == null ? 0 : currentFolder.FolderRow.DocumentFolderID, false);
            var folderNode = new FolderNode(folderRow);

            if(currentFolder == null)
            {
                folderRow.SetParentIDNull();
                tvwFolders.Nodes.Add(folderNode);
            }
            else
            {
                //copy existing security permission to this new folder
                DocumentsDataSet.DocumentFolder_SecurityGroupRow[] folderSecurityGroups = currentFolder.FolderRow.GetDocumentFolder_SecurityGroupRows();
                folderSecurityGroups.ForEach(folderSecurityGroup => _dsDocuments.DocumentFolder_SecurityGroup.AddDocumentFolder_SecurityGroupRow(folderRow, folderSecurityGroup.SecurityGroupRow));
                currentFolder.Nodes.Add(folderNode);
            }

            taDocumentFolder.Update(folderRow);
            taDocumentFolder_SecurityGroup.Update(_dsDocuments.DocumentFolder_SecurityGroup);

            folderNode.UpdateNodeUI();
            folderNode.BringIntoView();

            return folderRow;
        }

        internal void RenameFolder(DocumentsDataSet.DocumentFolderRow documentFolderRow, string newName)
        {
            _log.Info("Renaming folder " + documentFolderRow.Name);

            documentFolderRow.Name = newName;

            var folderNode = tvwFolders.FindNode(n => n is FolderNode && ((FolderNode)n).FolderRow.DocumentFolderID == documentFolderRow.DocumentFolderID) as FolderNode;
            folderNode?.UpdateNodeUI();

            taDocumentFolder.Update(documentFolderRow);
        }

        internal void DeleteFolder(DocumentsDataSet.DocumentFolderRow documentFolderRow, bool permanent)
        {
            foreach (var child in _dsDocuments.DocumentFolder.Where(df => !df.IsParentIDNull() && df.ParentID == documentFolderRow.DocumentFolderID))
            {
                DeleteFolder(child, permanent);
            }

            _log.Info("Deleting folder " + documentFolderRow.Name);

            if(permanent)
                documentFolderRow.Delete();
            else
                documentFolderRow.IsDeleted = true;

            var folderNode = tvwFolders.FindNode(n => n is FolderNode && ((FolderNode)n).FolderRow.DocumentFolderID == documentFolderRow.DocumentFolderID) as FolderNode;
            folderNode?.Remove();

            taDocumentFolder.Update(documentFolderRow);
        }

        internal void MoveToFolder(DocumentsDataSet.DocumentFolderRow documentFolderRow)
        {
            var folderNode = tvwFolders.FindNode(n => n is FolderNode && ((FolderNode)n).FolderRow.DocumentFolderID == documentFolderRow.DocumentFolderID) as FolderNode;

            folderNode?.Select(true);
        }

        private void DisposingMe()
        {
            _dsDocuments = null;

            if(_dragAndDrop != null)
            {
                _dragAndDrop.QueryValidDropOn = null;
                _dragAndDrop.BeforeDrop -= dragAndDrop_BeforeDrop;
                _dragAndDrop.AfterDrop -= dragAndDrop_AfterDrop;
                _dragAndDrop.Dispose();
            }

            _dragAndDrop = null;
        }

        private UltraTreeDragAndDropHelper.DropLinePositionEnum QueryValidDropOn(UltraTreeNode node)
        {
            if(node.Selected)
                return UltraTreeDragAndDropHelper.DropLinePositionEnum.None;

            return UltraTreeDragAndDropHelper.DropLinePositionEnum.AboveNode
                | UltraTreeDragAndDropHelper.DropLinePositionEnum.BelowNode
                | UltraTreeDragAndDropHelper.DropLinePositionEnum.OnNode;
        }

        #endregion

        #region Events

        private void tvwFolders_AfterSelect(object sender, SelectEventArgs e)
        {
            if (e.NewSelections.Count == 1)
            {
                if (e.NewSelections[0] is FolderNode folderNode)
                {
                    using (new UsingTimeMe("folder after select"))
                    {
                        DocumentManagerSecurity.Current.SetCurrentFolder(folderNode.FolderRow);
                        AfterFolderSelected?.Invoke(this, new FolderSelectedEventArgs { Folder = folderNode.FolderRow });
                    }
                }
            }
            else
            {
                DocumentManagerSecurity.Current.SetCurrentFolder(null);
                AfterFolderSelected(this, new FolderSelectedEventArgs { Folder = null });
            }
        }

        private void dragAndDrop_BeforeDrop(object sender, EventArgs e)
        {
            tvwFolders.Override.Sort = SortType.None;
        }

        private void dragAndDrop_AfterDrop(object sender, UltraTreeDragAndDropHelper.DropEventArgs e)
        {
            try
            {
                using (var taFolder = new DocumentFolderTableAdapter())
                {
                    foreach (var dragNode in e.DragNodes)
                    {
                        var movedFolder = dragNode as FolderNode;
                        var parentFolder = movedFolder.Parent as FolderNode;

                        if (parentFolder == null)
                        {
                            movedFolder.FolderRow.SetParentIDNull();
                        }
                        else
                        {
                            movedFolder.FolderRow.ParentID = parentFolder.FolderRow.DocumentFolderID;
                        }

                        taFolder.Update(movedFolder.FolderRow);
                    }
                }

                tvwFolders.Override.Sort = SortType.Ascending;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error moving folder via drag and drop.");
            }
        }

        private void tvwFolders_MouseUp(object sender, MouseEventArgs e)
        {
            if (!(sender is UltraTree treeControl))
            {
                return;
            }

            var nodeAtPoint = treeControl.GetNodeFromPoint(e.Location);

            if (nodeAtPoint == null)
            {
                tvwFolders.SelectedNodes.Clear();
            }
            else if (e.Button != MouseButtons.Left)
            {
                // Control has 'left click to select' built-in.
                // Must manually select when using other buttons.
                nodeAtPoint.Select(false);
            }
        }

        #endregion

        #region FolderNode

        public class FolderNode : UltraTreeNode
        {
            #region Properties

            public DocumentsDataSet.DocumentFolderRow FolderRow { get; set; }

            #endregion

            #region Methods

            public FolderNode(DocumentsDataSet.DocumentFolderRow folderRow) : base(folderRow.DocumentFolderID.ToString(), folderRow.Name)
            {
                FolderRow = folderRow;
                UpdateNodeUI();
            }

            public void UpdateNodeUI()
            {
                Text = FolderRow.Name;
                Override.NodeAppearance.Image = "Folder_16";

                LeftImages.Clear();

                //if no security groups then add a lock image to signify to user that no permissions exist
                if(FolderRow.GetDocumentFolder_SecurityGroupRows().Length < 1)
                {
                    Override.NodeAppearance.Image = "";
                    LeftImages.Add(UI.Properties.Resources.Security_16);
                }
            }

            #endregion
        }

        #endregion

        #region FolderSelectedEventArgs

        public class FolderSelectedEventArgs : EventArgs
        {
            public DocumentsDataSet.DocumentFolderRow Folder { get; set; }
        }

        #endregion

    }
}