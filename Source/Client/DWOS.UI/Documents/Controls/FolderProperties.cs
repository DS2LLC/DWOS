using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.DocumentsDataSetTableAdapters;
using DWOS.UI.Tools;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinListView;

namespace DWOS.UI.Documents.Controls
{
    public partial class FolderProperties : Form
    {
        #region Fields

        private CommandManager _commands;
        private DocumentsDataSet.DocumentFolderRow _documentFolder;
        private DocumentsDataSet _documents;

        #endregion

        #region Properties

        internal UltraListView RemovedItemsListView
        {
            get { return this.lvwRemovedItems; }
        }

        internal DocumentsDataSet Documents
        {
            get { return this._documents; }
        }

        internal DocumentsDataSet.DocumentFolderRow DocumentFolder
        {
            get { return this._documentFolder; }
        }

        public bool HasRestoredFiles { get; set; }

        public bool HasRestoredFolders { get; set; }

        #endregion

        #region Methods

        public FolderProperties()
        {
            InitializeComponent();
        }

        internal void LoadFile(DocumentsDataSet documents, DocumentsDataSet.DocumentFolderRow documentFolder)
        {
            this._documents = documents;
            this._documentFolder = documentFolder;

            LoadCommands();

            LoadGeneralTab();
            LoadRemovedItemsTab();
            LoadSecurityTab();
        }

        private void LoadGeneralTab()
        {
            this.txtFolderName.Text = DocumentFolder.Name;
            this.txtLocalDirectory.Text = Path.Combine(DocumentManagerUtilities.RootFolderPath, DocumentManagerUtilities.BuildPath(this._documents, this._documentFolder));
        }

        private void LoadRemovedItemsTab()
        {
            this.lvwRemovedItems.Items.Clear();

            //Get all deleted folders
            using(var taFolders = new DocumentFolderTableAdapter())
            {
                DocumentsDataSet.DocumentFolderDataTable deletedFolders = taFolders.GetDeletedFolders(this._documentFolder.DocumentFolderID);

                foreach(DocumentsDataSet.DocumentFolderRow deletedFolder in deletedFolders)
                    LoadFolderInfo(deletedFolder);
            }

            //Get all deleted files
            using(var taDocs = new DocumentInfoTableAdapter())
            {
                DocumentsDataSet.DocumentInfoDataTable deletedFiles = taDocs.GetDeletedFiles(this._documentFolder.DocumentFolderID);

                foreach(DocumentsDataSet.DocumentInfoRow deletedFile in deletedFiles)
                    LoadDocumentInfo(deletedFile);
            }
        }

        private void LoadSecurityTab()
        {
            this.lvwSecurityGroups.Items.Clear();

            //load all security groups
            foreach(DocumentsDataSet.SecurityGroupRow sg in this._documents.SecurityGroup)
                LoadSecurityGroup(sg);

            //check the ones that exist for this folder
            foreach(DocumentsDataSet.DocumentFolder_SecurityGroupRow sgr in this._documentFolder.GetDocumentFolder_SecurityGroupRows())
            {
                SecurityGroupListViewItem lvi = this.lvwSecurityGroups.Items.OfType <SecurityGroupListViewItem>().FirstOrDefault(sglvi => sglvi.SecurityGroup.SecurityGroupID == sgr.SecurityGroupID);
                if(lvi != null)
                    lvi.CheckState = CheckState.Checked;
            }
        }

        private void LoadDocumentInfo(DocumentsDataSet.DocumentInfoRow documentInfo)
        {
            //Add File list view item
            var fileItem = new RemovedDocumentListViewItem(documentInfo);
            this.lvwRemovedItems.Items.Add(fileItem);
            fileItem.UpdateNodeUI();
        }

        private void LoadFolderInfo(DocumentsDataSet.DocumentFolderRow documentFolder)
        {
            //Add File list view item
            var fileItem = new RemovedFolderListViewItem(documentFolder);
            this.lvwRemovedItems.Items.Add(fileItem);
            fileItem.UpdateNodeUI();
        }

        private void LoadSecurityGroup(DocumentsDataSet.SecurityGroupRow securityGroup)
        {
            //Add File list view item
            var sgItem = new SecurityGroupListViewItem(securityGroup);
            this.lvwSecurityGroups.Items.Add(sgItem);
            sgItem.UpdateNodeUI();
        }

        private void LoadCommands()
        {
            this._commands = new CommandManager();
            this._commands.AddCommand("DestroyItemCommand", new DestroyItemCommand(this.btnDestroy, this));
            this._commands.AddCommand("RestoreItemCommand", new RestoreItemCommand(this.btnRestore, this));
        }

        private void SaveData()
        {
            var folderSecGroups = this._documentFolder.GetDocumentFolder_SecurityGroupRows().ToList();

            //update Document Security groups
            foreach(SecurityGroupListViewItem sgItem in this.lvwSecurityGroups.Items.OfType <SecurityGroupListViewItem>())
            {
                DocumentsDataSet.SecurityGroupRow securityGroup = sgItem.SecurityGroup;
                DocumentsDataSet.DocumentFolder_SecurityGroupRow folderSecGroup = folderSecGroups.FirstOrDefault(sf_sg => sf_sg.SecurityGroupID == securityGroup.SecurityGroupID);

                if(sgItem.CheckState != CheckState.Checked && folderSecGroup != null)
                {
                    //remove folder security group
                    folderSecGroups.Remove(folderSecGroup);
                    folderSecGroup.Delete();
                }
                if(sgItem.CheckState == CheckState.Checked && folderSecGroup == null)
                {
                    //add new folder security group
                    this._documents.DocumentFolder_SecurityGroup.AddDocumentFolder_SecurityGroupRow(this._documentFolder, securityGroup);
                }
                if(sgItem.CheckState == CheckState.Checked && folderSecGroup != null)
                {
                }
                if(sgItem.CheckState != CheckState.Checked && folderSecGroup == null)
                {
                }
            }

            DocumentManagerSecurity.Current.ReloadSecurityRoles();
            DocumentManagerSecurity.Current.RefreshCommands();
        }

        private void DisposeMe()
        {
            _documentFolder = null;
            _documents = null;

            if (_commands != null)
                _commands.Dispose();

            _commands = null;
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e) { SaveData(); }

        private void btnCancel_Click(object sender, EventArgs e) { Close(); }

        #endregion

        #region RemovedDocumentListViewItem

        private class RemovedDocumentListViewItem : UltraListViewItem
        {
            public RemovedDocumentListViewItem(DocumentsDataSet.DocumentInfoRow documentInfo) : base("DI-" + documentInfo.DocumentInfoID) { DocumentInfo = documentInfo; }

            public DocumentsDataSet.DocumentInfoRow DocumentInfo { get; set; }

            public void UpdateNodeUI()
            {
                //Version, User, Timestamp, Comments
                Value = this.DocumentInfo.Name;
                SubItems["Type"].Value = "File";
            }

            public override void Dispose()
            {
                DocumentInfo = null;

                base.Dispose();
            }
        }

        #endregion

        #region RemovedDocumentListViewItem

        private class RemovedFolderListViewItem : UltraListViewItem
        {
            public RemovedFolderListViewItem(DocumentsDataSet.DocumentFolderRow documentInfo) : base("DF-" + documentInfo.DocumentFolderID) { DocumentFolder = documentInfo; }

            public DocumentsDataSet.DocumentFolderRow DocumentFolder { get; set; }

            public void UpdateNodeUI()
            {
                //Version, User, Timestamp, Comments
                Value = this.DocumentFolder.Name;
                SubItems["Type"].Value = "Folder";
            }

            public override void Dispose()
            {
                DocumentFolder = null;
                base.Dispose();
            }
        }

        #endregion

        #region SecurityGroupListViewItem

        private class SecurityGroupListViewItem : UltraListViewItem
        {
            public SecurityGroupListViewItem(DocumentsDataSet.SecurityGroupRow securityGroup) : base("SG-" + securityGroup.SecurityGroupID) { SecurityGroup = securityGroup; }

            public DocumentsDataSet.SecurityGroupRow SecurityGroup { get; set; }

            public void UpdateNodeUI() { Value = this.SecurityGroup.Name; }

            public override void Dispose()
            {
                SecurityGroup = null;
                base.Dispose();
            }
        }

        #endregion

        internal class DestroyItemCommand : CommandBase
        {
            #region Fields

            private FolderProperties _fileProperties;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._fileProperties.RemovedItemsListView.Items.Count > 0 && DocumentManagerSecurity.Current.IsInRole("Documents.Destroy"); }
            }

            #endregion

            #region Methods

            public DestroyItemCommand(UltraButton tool, FolderProperties fileProperties) : base(tool)
            {
                this._fileProperties = fileProperties;
                this._fileProperties.RemovedItemsListView.ItemSelectionChanged += _revisionListView_ItemSelectionChanged;
                Refresh();
            }

            public override void OnClick()
            {
                try
                {
                    foreach(UltraListViewItem item in this._fileProperties.RemovedItemsListView.SelectedItems)
                    {
                        if(item is RemovedDocumentListViewItem)
                        {
                            var docItem = item as RemovedDocumentListViewItem;

                            //Remove from UI first
                            this._fileProperties.RemovedItemsListView.Items.Remove(item);

                            _log.Info("Destroying file {0} as permanent.", docItem.DocumentInfo.Name);

                            List <DocumentsDataSet.DocumentFolder_DocumentInfoRow> currentFolders = docItem.DocumentInfo.GetDocumentFolder_DocumentInfoRows().Where(df_di => df_di.DocumentFolderID == this._fileProperties.DocumentFolder.DocumentFolderID).ToList();

                            if(currentFolders.Count > 0)
                            {
                                currentFolders.ForEach(cf => cf.Delete());
                                _log.Info("Removing file {0} from all folders.", docItem.DocumentInfo.Name);
                                using(var ta = new DocumentFolder_DocumentInfoTableAdapter())
                                    ta.Update(currentFolders.ToArray());
                            }

                            docItem.DocumentInfo.Delete();

                            //Remove from DB
                            using(var ta = new DocumentInfoTableAdapter())
                                ta.Update(docItem.DocumentInfo);
                        }
                        else if(item is RemovedFolderListViewItem)
                        {
                            var folderItem = item as RemovedFolderListViewItem;

                            //Remove from UI first
                            this._fileProperties.RemovedItemsListView.Items.Remove(item);

                            _log.Info("Destroying folder {0} as permanent.", folderItem.DocumentFolder.Name);

                            folderItem.DocumentFolder.Delete();

                            //Remove from DB
                            using(var ta = new DocumentFolderTableAdapter())
                                ta.Update(folderItem.DocumentFolder);
                        }
                    }
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error .");
                }
            }

            public override void Dispose()
            {
                _fileProperties = null;
                base.Dispose();
            }

            #endregion

            #region Events

            private void _revisionListView_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e) { Refresh(); }

            #endregion
        }

        internal class RestoreItemCommand : CommandBase
        {
            #region Fields

            private FolderProperties _fileProperties;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._fileProperties.RemovedItemsListView.Items.Count > 0 && DocumentManagerSecurity.Current.IsInRole("Documents.Restore"); }
            }

            #endregion

            #region Methods

            public RestoreItemCommand(UltraButton tool, FolderProperties fileProperties) : base(tool)
            {
                this._fileProperties = fileProperties;
                this._fileProperties.RemovedItemsListView.ItemSelectionChanged += _revisionListView_ItemSelectionChanged;
                Refresh();
            }

            public override void OnClick()
            {
                try
                {
                    foreach(UltraListViewItem item in this._fileProperties.RemovedItemsListView.SelectedItems)
                    {
                        if(item is RemovedDocumentListViewItem)
                        {
                            var docItem = item as RemovedDocumentListViewItem;
                            this._fileProperties.RemovedItemsListView.Items.Remove(item);
                            docItem.DocumentInfo.IsDeleted = false;

                            //Remove from DB
                            using(var ta = new DocumentInfoTableAdapter())
                                ta.Update(docItem.DocumentInfo);

                            this._fileProperties.HasRestoredFiles = true;
                        }
                        else if(item is RemovedFolderListViewItem)
                        {
                            var folderItem = item as RemovedFolderListViewItem;
                            this._fileProperties.RemovedItemsListView.Items.Remove(item);

                            folderItem.DocumentFolder.IsDeleted = false;

                            //Remove from DB
                            using(var ta = new DocumentFolderTableAdapter())
                                ta.Update(folderItem.DocumentFolder);

                            this._fileProperties.HasRestoredFolders = true;
                        }
                    }
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error .");
                }
            }

            public override void Dispose()
            {
                _fileProperties = null;
                base.Dispose();
            }

            #endregion

            #region Events

            private void _revisionListView_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e) { Refresh(); }

            #endregion
        }
    }
}