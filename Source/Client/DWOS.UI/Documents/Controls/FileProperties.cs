using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.DocumentsDataSetTableAdapters;
using DWOS.Data.Datasets.ProcessesDatasetTableAdapters;
using DWOS.Shared.Utilities;
using DWOS.UI.Tools;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinListView;

namespace DWOS.UI.Documents.Controls
{
    public partial class FileProperties : Form
    {
        #region Fields

        private CommandManager _commands;
        private DocumentsDataSet.DocumentFolderRow _documentFolder;
        private DocumentsDataSet.DocumentInfoRow _documentInfo;
        private DocumentsDataSet _documents;

        #endregion

        #region Properties

        internal DocumentsDataSet.DocumentRevisionRow SelectedRevision
        {
            get
            {
                var rev = this.lvwHistory.SelectedItems.FirstOrDefault(lvi => lvi is RevisionListViewItem) as RevisionListViewItem;

                return rev?.DocumentRevision;
            }
        }

        #endregion

        #region Methods

        public FileProperties() { InitializeComponent(); }

        internal void LoadFile(DocumentsDataSet documents, DocumentsDataSet.DocumentFolderRow documentFolder, DocumentsDataSet.DocumentInfoRow documentInfo)
        {
            this._documents = documents;
            this._documentInfo = documentInfo;
            this._documentFolder = documentFolder;

            LoadCommands();

            LoadGeneralTab();

            if(_documentInfo.DocumentLocked)
            {
                this.tabFileProperties.Tabs["CheckOut"].Enabled = true;
                LoadCheckOutTab();
            }
            else
                this.tabFileProperties.Tabs["CheckOut"].Enabled = false;

            LoadHistoryTab();
            LoadLocationsTab();
        }

        private void LoadGeneralTab()
        {
            txtFileName.Text       = _documentInfo.Name;
            txtDescription.Text    = _documentInfo.IsDescriptionNull() ? string.Empty : _documentInfo.Description;
            txtLocalDirectory.Text = Path.Combine(DocumentManagerUtilities.RootFolderPath,
                DocumentManagerUtilities.BuildPath(this._documents, this._documentFolder));

            txtFileSize.Text       = "0 MB";

            var revision = _documentInfo.GetDocumentRevisionRows().FirstOrDefault(dr => dr.RevisionNumber == _documentInfo.CurrentRevision) ??
                _documentInfo.GetDocumentRevisionRows().FirstOrDefault();

            if(revision != null)
            {
                using(var taDocRev = new DocumentRevisionTableAdapter())
                {
                    var size = taDocRev.GetFileSize(revision.DocumentRevisionID);
                    txtFileSize.Text = FileSystem.ConvertBytesToString(size.GetValueOrDefault());
                }
            }
        }

        private void LoadCheckOutTab()
        {
            DocumentsDataSet.DocumentLockRow docLock = DocumentManagerUtilities.GetCurrentDocumentLock(_documentInfo);

            if(docLock != null)
            {
                this.txtCheckOutUserName.Text = docLock.LockedByUser.ToString();
                this.txtCheckOutTime.Text = docLock.DateLockedUTC.ToLocalTime().ToString();
                this.txtCheckOutLocation.Text = docLock.IsLocalFilePathNull() ? "Unknown" : docLock.LocalFilePath;
                this.txtCheckOutComputer.Text = docLock.IsComputerNameNull() ? "Unknown" : docLock.ComputerName;
            }
        }

        private void LoadHistoryTab()
        {
            this.lvwHistory.Items.Clear();

            foreach (DocumentsDataSet.DocumentRevisionRow documentRev in _documentInfo.GetDocumentRevisionRows())
            {
                DocumentsDataSet.UsersRow user = null;

                if (!documentRev.IsUserCreatedNull())
                {
                    user = _documents.Users.FindByUserID(documentRev.UserCreated);
                }

                var fileItem = new RevisionListViewItem(documentRev, user);
                this.lvwHistory.Items.Add(fileItem);
                fileItem.UpdateNodeUI();
            }
        }

        private void LoadLocationsTab()
        {
            LoadFolderLocations();
            LoadDocLinkLocations();
        }

        private void LoadDocLinkLocations()
        {
            using(var taDocLink = new DocumentLinkTableAdapter())
            {
                using(var tableDocLink = new DocumentsDataSet.DocumentLinkDataTable())
                {
                    taDocLink.FillByDocumentInfo(tableDocLink, _documentInfo.DocumentInfoID);

                    foreach(DocumentsDataSet.DocumentLinkRow row in tableDocLink.Rows)
                    {
                        var name = GetTypeName(row.LinkToType, row.LinkToKey);
                        var item = new UltraListViewItem(_documentInfo.Name, new string[] {row.LinkToType, name});
                        item.Appearance.Image = "link";
                        this.lvwLocations.Items.Add(item);
                    }
                }
            }
        }

        private void LoadFolderLocations()
        {
            this.lvwLocations.Items.Clear();
            
            using(var taDocInfo = new DocumentFolder_DocumentInfoTableAdapter())
            {
                using(var tableDocInfo = new DocumentsDataSet.DocumentFolder_DocumentInfoDataTable())
                {
                    taDocInfo.FillByDocument(tableDocInfo, _documentInfo.DocumentInfoID);

                    foreach(DocumentsDataSet.DocumentFolder_DocumentInfoRow row in tableDocInfo.Rows)
                    {
                        var folderName = String.Empty;

                        var folder = _documents.DocumentFolder.FindByDocumentFolderID(row.DocumentFolderID);
                        if (folder != null)
                            folderName = DocumentManagerUtilities.BuildPath(_documents, folder);

                        var item = new UltraListViewItem(_documentInfo.Name, new string[] { "Folder", folderName });
                        item.Appearance.Image = "folder";
                        this.lvwLocations.Items.Add(item);
                    }
                }
            }
        }

        private string GetProcessName(int ID)
        {
            var taProcess = new ProcessTableAdapter();
            return taProcess.GetProcessName(ID) as string;
        }

        private KeyValuePair <string, int> GetStepNameProcessID(int ID)
        {
            using (var taProcessStep = new ProcessStepsTableAdapter())
            {
                using (var tableStep = new ProcessesDataset.ProcessStepsDataTable())
                {
                    taProcessStep.GetStepName(tableStep, ID);
                    object name = tableStep.Rows[0].ItemArray[2];
                    object processID = tableStep.Rows[0].ItemArray[1];
                    return new KeyValuePair<string, int>(name.ToString(), Convert.ToInt32(processID));
                }
            }
        }

        private KeyValuePair <string, int> GetAliasNameProcessID(int ID)
        {
            using (var taProcessAlias = new ProcessAliasTableAdapter())
            {
                using (var tableAlias = new ProcessesDataset.ProcessAliasDataTable())
                {
                    taProcessAlias.GetAliasName(tableAlias, ID);
                    object name = tableAlias.Rows[0].ItemArray[1];
                    object processID = tableAlias.Rows[0].ItemArray[2];
                    return new KeyValuePair<string, int>(name.ToString(), Convert.ToInt32(processID));
                }
            }
        }

        private string GetTypeName(string type, int ID)
        {
            if(type == "Process")
                return GetProcessName(ID);
            
            if(type == "ProcessSteps")
            {
                KeyValuePair <string, int> pair = GetStepNameProcessID(ID);
                string name = pair.Key;
                int processID = pair.Value;
                string processName = GetProcessName(Convert.ToInt32(processID));
                string stepName = processName + "\\" + name;
                return stepName;
            }
           
            if(type == "ProcessAlias")
            {
                KeyValuePair <string, int> pair = GetAliasNameProcessID(ID);
                string name = pair.Key;
                int processID = pair.Value;
                string processName = GetProcessName(Convert.ToInt32(processID));
                string aliasName = processName + "\\" + name;
                return aliasName;
            }
            
            return "Unknown";
        }

        private void LoadCommands()
        {
            this._commands = new CommandManager();
            this._commands.AddCommand("GetFileCommand", new GetFileCommand(this.btnGetVersion, this));
        }

        private void DisposeMe()
        {
            if (_commands != null)
                _commands.Dispose();

            _commands = null;
        }

        #endregion

        #region RevisionListViewItem

        private class RevisionListViewItem : UltraListViewItem
        {
            public RevisionListViewItem(DocumentsDataSet.DocumentRevisionRow documentRevision, DocumentsDataSet.UsersRow userChanged)
                : base(documentRevision.DocumentRevisionID.ToString())
            {
                DocumentRevision = documentRevision;
                UserChanged = userChanged;
            }

            public DocumentsDataSet.DocumentRevisionRow DocumentRevision { get; set; }

            public DocumentsDataSet.UsersRow UserChanged { get; set;  }

            public void UpdateNodeUI()
            {
                //Version, User, Timestamp, Comments
                Value = DocumentRevision.RevisionNumber.ToString();

                SubItems["User"].Value = UserChanged?.Name ??
                    (DocumentRevision.IsUserCreatedNull() ? string.Empty : DocumentRevision.UserCreated.ToString());

                SubItems["Timestamp"].Value = DocumentRevision.IsDateCreatedUTCNull() ? string.Empty : DocumentRevision.DateCreatedUTC.ToLocalTime().ToString();
                SubItems["Comments"].Value = DocumentRevision.IsCommentsNull() ? string.Empty : DocumentRevision.Comments;
            }

            public override void Dispose()
            {
                DocumentRevision = null;

                base.Dispose();
            }
        }

        #endregion

        #region GetFileCommand

        internal class GetFileCommand : CommandBase
        {
            #region Fields

            private FileProperties _fileProperties;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get
                {
                    return base.Enabled &&
                        this._fileProperties.lvwHistory.Items.Count > 0 &&
                        DocumentManagerSecurity.Current.IsInRole("Documents.GetFiles");
                }
            }

            #endregion

            #region Methods

            public GetFileCommand(UltraButton tool, FileProperties fileProperties) : base(tool)
            {
                this._fileProperties = fileProperties;
                this._fileProperties.lvwHistory.ItemSelectionChanged += _revisionListView_ItemSelectionChanged;
            }

            public override void OnClick()
            {
                try
                {
                    using(var dlg = new GetFile())
                    {
                        dlg.LoadFile(this._fileProperties._documents,
                            this._fileProperties._documentFolder,
                            this._fileProperties._documentInfo,
                            this._fileProperties.SelectedRevision);

                        dlg.ShowDialog(this._fileProperties);
                    }
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error .");
                }
            }

            #endregion

            #region Events

            private void _revisionListView_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e) { Refresh(); }

            #endregion
        }

        #endregion
    }
}