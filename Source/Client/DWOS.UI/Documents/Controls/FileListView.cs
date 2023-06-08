using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Shared.Utilities;
using DWOS.UI.Documents;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinListView;
using Ionic.Zlib;
using NLog;

namespace DWOS.Documents.Controls
{
    public partial class FileListView : UserControl
    {
        #region Fields

        public enum FileStatus
        {
            Current,
            Missing,
            Modified,
            Old,
            Unknown
        }

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private DocumentsDataSet _documents;
        private List <int> _foldersLoaded = new List <int>();
        private string _rootWorkingPath;
        public event EventHandler <FilesSelectedEventArgs> AfterFilesSelected;
        public event EventHandler <FilesSelectedEventArgs> AfterFileDoubleClicked;

        #endregion

        #region Properties

        internal DocumentsDataSet.DocumentFolderRow SelectedFolder { get; set; }

        internal List <DocumentsDataSet.DocumentInfoRow> SelectedFiles
        {
            get { return this.lvwFiles.SelectedItems.OfType <FileListViewItem>().Select(fli => fli.DocumentInfo).ToList(); }
        }

        internal string SelectedFolderPath { get; set; }

        private string RootWorkingPath
        {
            get { return this._rootWorkingPath; }
            set { this._rootWorkingPath = value; }
        }

        #endregion

        #region Methods

        public FileListView() { InitializeComponent(); }

        internal void InitialLoad(DocumentsDataSet documents, string rootWorkingPath)
        {
            this._documents = documents;
            this._rootWorkingPath = rootWorkingPath;
        }

        /// <summary>
        /// Loads the files for the specified folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        internal void LoadFiles(DocumentsDataSet.DocumentFolderRow folder)
        {
            using(new UsingWaitCursor(this.ParentForm))
            {
                ClearView();

                if(this._documents == null)
                    return;

                SelectedFolder = folder;
                SelectedFolderPath = String.Empty;

                if(folder != null)
                {
                    SelectedFolderPath = Path.Combine(RootWorkingPath, DocumentManagerUtilities.BuildPath(this._documents, folder));
                    _log.Info("Moving to path " + SelectedFolderPath);

                    LoadData(folder);

                    //add each file related to this folder to the list
                    foreach(DocumentsDataSet.DocumentFolder_DocumentInfoRow documentFolderDocumentInfoRow in folder.GetDocumentFolder_DocumentInfoRows())
                        LoadFile(documentFolderDocumentInfoRow.DocumentInfoRow);
                }
            }
        }

        /// <summary>
        /// Loads the data for the specified folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="forceReload">if set to <c>true</c> [force reload].</param>
        internal void LoadData(DocumentsDataSet.DocumentFolderRow folder, bool forceReload = false)
        {
            //if data has not been loaded then load it
            if(forceReload || !this._foldersLoaded.Contains(folder.DocumentFolderID))
            {
                this._documents.EnforceConstraints = false;

                this.taDocumentInfo.FillByFolder(this._documents.DocumentInfo, folder.DocumentFolderID);
                this.taDocumentFolder_DocumentInfo.FillByFolder(this._documents.DocumentFolder_DocumentInfo, folder.DocumentFolderID);
                this.taDocumentLock.FillByFolder(this._documents.DocumentLock, folder.DocumentFolderID);
                this.taDocumentRevision.FillByFolder(this._documents.DocumentRevision, folder.DocumentFolderID);
                
                this._documents.EnforceConstraints = true;

                if(!forceReload)
                    this._foldersLoaded.Add(folder.DocumentFolderID);
            }
        }

        /// <summary>
        /// Loads the file into the view.
        /// </summary>
        /// <param name="documentInfo">The document information.</param>
        private void LoadFile(DocumentsDataSet.DocumentInfoRow documentInfo)
        {
            //Add File list view item
            var fileItem = new FileListViewItem(documentInfo, this);
            this.lvwFiles.Items.Add(fileItem);
            fileItem.UpdateNodeUI();
        }

        internal void AddFiles(List <string> filePaths, string comments)
        {
            foreach(string filePath in filePaths)
                AddFile(filePath, comments);
        }

        internal void AddFile(string filePath, string comments)
        {
            _log.Info("Adding new file {0}.", filePath);

            string fileName = Path.GetFileName(filePath).TrimToMaxLength(255);
            string fileExt = Path.GetExtension(filePath).TrimStart('.');
            string fileHash = FileSystem.GetMD5HashFromFile(filePath);

            var documentInfo = this._documents.DocumentInfo.AddDocumentInfoRow(fileName, null, 1, false, fileExt, false);
            var documentRevision = this._documents.DocumentRevision.AddDocumentRevisionRow(fileName, documentInfo, null, SecurityManager.Current.UserID, 1, fileHash, DateTime.UtcNow, false, comments);
            var docFolder = this._documents.DocumentFolder_DocumentInfo.AddDocumentFolder_DocumentInfoRow(SelectedFolder, documentInfo);

            this.taDocumentInfo.Update(documentInfo);
            this.taDocumentRevision.Update(documentRevision);
            this.taDocumentFolder_DocumentInfo.Update(docFolder);

            //upload file to new revision
            UploadFile(documentRevision, filePath);

            LoadFile(documentInfo);
        }

        internal void RenameFile(DocumentsDataSet.DocumentInfoRow documentInfo, string newName)
        {
            documentInfo.Name = newName;

            this.taDocumentInfo.Update(documentInfo);

            var fileItem = FindItem(documentInfo);
            if (fileItem != null)
                fileItem.UpdateNodeUI();
        }

        internal void ReplaceFile(DocumentsDataSet.DocumentInfoRow documentInfo, string newName, string filePath)
        {
            var originalName = documentInfo.Name;

            //update the name of the document
            documentInfo.Name = newName;
            this.taDocumentInfo.Update(documentInfo);

            //check out the file
            CheckOutFile(documentInfo);

            //check in with new file location
            CheckInFile(documentInfo, $"Replacing file {originalName}.", filePath);

            // Update local copy of file
            GetFile(documentInfo, true);
        }

        internal void GetFiles(List <DocumentsDataSet.DocumentInfoRow> files, bool forceGet)
        {
            foreach(DocumentsDataSet.DocumentInfoRow documentInfoRow in files)
                GetFile(documentInfoRow, forceGet);
        }

        private void GetFile(DocumentsDataSet.DocumentInfoRow documentInfoRow, bool forceGet)
        {
            try
            {
                _log.Info($"Getting the file {documentInfoRow.Name}.");

                string filePath = Path.Combine(SelectedFolderPath, documentInfoRow.Name);
                var info = new FileInfo(filePath);
                if(!Directory.Exists(SelectedFolderPath))
                    Directory.CreateDirectory(SelectedFolderPath);

                if(forceGet && File.Exists(filePath))
                {
                    if(info.IsReadOnly)
                        info.IsReadOnly = false;
                    File.Delete(filePath);
                }

                if(!File.Exists(filePath))
                {
                    DocumentsDataSet.DocumentRevisionRow lastRevision = documentInfoRow.GetDocumentRevisionRows().FirstOrDefault(dlr => dlr.RevisionNumber == documentInfoRow.CurrentRevision);

                    if(lastRevision != null)
                        DownloadFile(lastRevision, filePath);
                }

                FileListViewItem fileItem = FindItem(documentInfoRow);
                if(fileItem != null)
                    fileItem.UpdateNodeUI();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting file to local directory.");
            }
        }

        internal void DeleteFiles(List <DocumentsDataSet.DocumentInfoRow> files, bool permanentDelete)
        {
            foreach(DocumentsDataSet.DocumentInfoRow documentInfoRow in files)
                DeleteFile(documentInfoRow, permanentDelete);
        }

        private void DeleteFile(DocumentsDataSet.DocumentInfoRow documentInfoRow, bool permanentDelete)
        {
            try
            {
                //Remove from UI first
                FileListViewItem fileItem = FindItem(documentInfoRow);
                if(fileItem != null)
                    this.lvwFiles.Items.Remove(fileItem);

                if(permanentDelete)
                {
                    _log.Info("Deleting file {0} as permanent.", documentInfoRow.Name);

                    List <DocumentsDataSet.DocumentFolder_DocumentInfoRow> currentFolders = documentInfoRow.GetDocumentFolder_DocumentInfoRows().Where(df_di => df_di.DocumentFolderID == SelectedFolder.DocumentFolderID).ToList();

                    if(currentFolders.Count > 0)
                    {
                        currentFolders.ForEach(cf => cf.Delete());
                        _log.Info("Removing file {0} from all folders.", documentInfoRow.Name);
                        this.taDocumentFolder_DocumentInfo.Update(currentFolders.ToArray());
                    }

                    documentInfoRow.Delete();
                }
                else
                {
                    _log.Info("Deleting file {0} as NOT permanent.", documentInfoRow.Name);
                    documentInfoRow.IsDeleted = true;
                }

                //deleteFileList.Add(documentInfoRow);
                //Remove from DB
                this.taDocumentInfo.Update(documentInfoRow);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting file to local directory.");
            }
        }

        internal void CheckOutFiles(List <DocumentsDataSet.DocumentInfoRow> files)
        {
            foreach(DocumentsDataSet.DocumentInfoRow documentInfoRow in files)
                CheckOutFile(documentInfoRow);
        }

        private void CheckOutFile(DocumentsDataSet.DocumentInfoRow documentInfoRow)
        {
            if(documentInfoRow.DocumentLocked)
                return; //can't lock if already locked

            _log.Info("Checking out the file {0}.", documentInfoRow.Name);

            int userID = SecurityManager.Current.UserID;

            //Create new loc
            DocumentsDataSet.DocumentLockRow docLock = this._documents.DocumentLock.AddDocumentLockRow(userID, documentInfoRow, DateTime.UtcNow, DateTime.Now, Environment.MachineName.TrimToMaxLength(50), SelectedFolderPath.TrimToMaxLength(255));
            docLock.SetDateUnlockedUTCNull();

            //Set file as locked
            documentInfoRow.DocumentLocked = true;

            //lockRowList.Add(docLock);
            //infoRowList.Add(documentInfoRow);
            this.taDocumentLock.Update(docLock);
            this.taDocumentInfo.Update(documentInfoRow);

            string filePath = Path.Combine(SelectedFolderPath, documentInfoRow.Name);
            if (File.Exists(filePath))
            {
                try
                {
                    FileSystem.SetFileAttributes(filePath, FileAttributes.Normal);
                }
                catch (ArgumentException exc)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn($"Could not mark {filePath} as writable.",
                        "Check Out");

                    _log.Warn(exc, "Unable to mark file as read-only.");
                }
            }

            FileListViewItem fileItem = FindItem(documentInfoRow);
            if(fileItem != null)
                fileItem.UpdateNodeUI();
        }

        internal void CheckInFiles(List <DocumentsDataSet.DocumentInfoRow> files, string comments)
        {
            foreach(DocumentsDataSet.DocumentInfoRow documentInfoRow in files)
                CheckInFile(documentInfoRow, comments);
        }

        private void CheckInFile(DocumentsDataSet.DocumentInfoRow documentInfoRow, string comments, string filePath = null)
        {
            if(!documentInfoRow.DocumentLocked)
                return; //can't unlock if not locked

            filePath = filePath ?? Path.Combine(SelectedFolderPath, documentInfoRow.Name);

            if (!File.Exists(filePath))
                return;

            _log.Info("Checking in the file {0}.", documentInfoRow.Name);
            int userID = SecurityManager.Current.UserID;

            //Update Locks; NOTE: Ensure there are no open locks
            List <DocumentsDataSet.DocumentLockRow> lastLocks = documentInfoRow.GetDocumentLockRows().Where(dlr => dlr.IsDateUnlockedUTCNull()).ToList();

            //if this user did not lock it then dont check in
            if(!lastLocks.Exists(lr => lr.LockedByUser == userID))
                return;

            //Update Locks; NOTE: Ensure there are no open locks
            lastLocks.ForEach(l => l.DateUnlockedUTC = DateTime.UtcNow);
            this.taDocumentLock.Update(lastLocks.ToArray());

            //Get current revision
            int lastRevision = documentInfoRow.GetDocumentRevisionRows().Max(dlr => dlr.RevisionNumber);
            int nextRevision = lastRevision + 1;

            //Add new revision
            string fileHash = FileSystem.GetMD5HashFromFile(filePath);
            DocumentsDataSet.DocumentRevisionRow revision = this._documents.DocumentRevision.AddDocumentRevisionRow(documentInfoRow.Name, documentInfoRow, null, userID, nextRevision, fileHash, DateTime.UtcNow, false, comments);
            this.taDocumentRevision.Update(revision);

            //upload file to new revision
            UploadFile(revision, filePath);

            //Update document info
            documentInfoRow.DocumentLocked = false;
            documentInfoRow.CurrentRevision = nextRevision;
            this.taDocumentInfo.Update(documentInfoRow);

            try
            {
                FileSystem.SetFileAttributes(filePath, FileAttributes.ReadOnly);
            }
            catch (ArgumentException exc)
            {
                MessageBoxUtilities.ShowMessageBoxWarn($"Could not mark {filePath} as read-only.",
                    "Check In");

                _log.Warn(exc, "Unable to mark file as read-only.");
            }

            //Update node
            FileListViewItem fileItem = FindItem(documentInfoRow);
            if(fileItem != null)
                fileItem.UpdateNodeUI();
        }

        internal void UndoCheckOutFiles(List <DocumentsDataSet.DocumentInfoRow> files)
        {
            foreach(DocumentsDataSet.DocumentInfoRow documentInfoRow in files)
                UndoCheckOutFile(documentInfoRow);
        }

        private void UndoCheckOutFile(DocumentsDataSet.DocumentInfoRow documentInfoRow)
        {
            try
            {
                if(!documentInfoRow.DocumentLocked)
                    return; //can't unlock if not locked

                _log.Info("Undo Checking out the file {0}.", documentInfoRow.Name);

                //Close all locks; NOTE: Ensure there are no open locks
                List <DocumentsDataSet.DocumentLockRow> lastLocks = documentInfoRow.GetDocumentLockRows().Where(dlr => dlr.IsDateUnlockedUTCNull()).ToList();
                lastLocks.ForEach(l => l.DateUnlockedUTC = DateTime.UtcNow);
                this.taDocumentLock.Update(lastLocks.ToArray());

                //Get current revision
                int lastRevision = documentInfoRow.GetDocumentRevisionRows().Max(dlr => dlr.RevisionNumber);

                //Update document info
                documentInfoRow.DocumentLocked = false;
                documentInfoRow.CurrentRevision = lastRevision;
                this.taDocumentInfo.Update(documentInfoRow);

                //Update node
                FileListViewItem fileItem = FindItem(documentInfoRow);
                if(fileItem != null)
                    fileItem.UpdateNodeUI();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting file to local directory.");
            }
        }

        internal void ViewFile(DocumentsDataSet.DocumentInfoRow documentInfoRow)
        {
            try
            {
                _log.Info("Getting the file {0}.", documentInfoRow.Name);

                string filePath = Path.Combine(SelectedFolderPath, documentInfoRow.Name);

                if(!Directory.Exists(SelectedFolderPath))
                    Directory.CreateDirectory(SelectedFolderPath);

                if(!File.Exists(filePath))
                {
                    DocumentsDataSet.DocumentRevisionRow lastRevision = documentInfoRow.GetDocumentRevisionRows().FirstOrDefault(dlr => dlr.RevisionNumber == documentInfoRow.CurrentRevision);

                    if(lastRevision != null)
                        DownloadFile(lastRevision, filePath);
                }

                FileLauncher.New()
                    .HandleErrorsWith((exception, errorMsg) => MessageBoxUtilities.ShowMessageBoxWarn(errorMsg, "Unable to Start", filePath))
                    .Launch(filePath);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error viewing file.");
            }
        }

        private FileListViewItem FindItem(DocumentsDataSet.DocumentInfoRow documentInfoRow)
        {
            foreach(FileListViewItem fileItem in this.lvwFiles.Items.OfType <FileListViewItem>())
            {
                if(fileItem.DocumentInfo.DocumentInfoID == documentInfoRow.DocumentInfoID)
                    return fileItem;
            }

            return null;
        }

        private void UploadFile(DocumentsDataSet.DocumentRevisionRow documentRevision, string filePath)
        {
            _log.Info("Uploading file '{0}' from '{1}'", documentRevision.FileName, filePath);

            byte[] bytes = MediaUtilities.CreateMediaStream(filePath, -1);
            string fileExt = Path.GetExtension(documentRevision.FileName);
            fileExt = fileExt == null ? null : fileExt.TrimStart('.');

            bool shouldCompress = DocumentManagerUtilities.ShouldCompressFileType(fileExt);

            if(shouldCompress)
            {
                _log.Info("Compressing file '{0}'", documentRevision.FileName);
                bytes = ZlibStream.CompressBuffer(bytes);
            }

            documentRevision.IsCompressed = shouldCompress;
            //uploadList.Add(new Tuple<byte[], bool, int>(bytes, shouldCompress, documentRevision.DocumentRevisionID));
            this.taDocumentRevision.UpdateDocumentData(bytes, shouldCompress, documentRevision.DocumentRevisionID);
        }

        private void DownloadFile(DocumentsDataSet.DocumentRevisionRow documentRevision, string filePath)
        {
            _log.Info("Downloading file '{0}' to '{1}'.", documentRevision.FileName, filePath);
            byte[] bytes = this.taDocumentRevision.GetMediaStream(documentRevision.DocumentRevisionID);

            if (bytes != null) //Allow 0 length files as you could have an empty text file
            {
                if(bytes.Length > 0 && documentRevision.IsCompressed)
                {
                    _log.Info("UnCompressing file '{0}'", documentRevision.FileName);
                    bytes = ZlibStream.UncompressBuffer(bytes);
                }

                using(var fileStream = new BinaryWriter(File.OpenWrite(filePath)))
                    fileStream.Write(bytes);

                FileSystem.SetFileAttributes(filePath, FileAttributes.ReadOnly);
            }
        }

        /// <summary>
        ///     Clears the current list view.
        /// </summary>
        private void ClearView()
        {
            this.lvwFiles.Items.Clear();
            SelectedFolder = null;
        }

        private void DisposingMe()
        {
            this._documents = null;
            this._foldersLoaded = null;
        }

        internal void RefreshNodes() { this.lvwFiles.Items.OfType <FileListViewItem>().ForEach(lvi => lvi.UpdateNodeUI()); }

        #endregion

        #region Events

        private void lvwFiles_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            if(AfterFilesSelected != null)
                AfterFilesSelected(this, new FilesSelectedEventArgs {Files = SelectedFiles});
        }

        private void lvwFiles_ItemDoubleClick(object sender, ItemDoubleClickEventArgs e)
        {
            if(AfterFileDoubleClicked != null)
                AfterFileDoubleClicked(this, new FilesSelectedEventArgs {Files = SelectedFiles});
        }

        private void lvwFiles_MouseUp(object sender, MouseEventArgs e)
        {
            if (!(sender is UltraListView listView))
            {
                return;
            }

            var itemFromPoint = listView.ItemFromPoint(e.Location);

            if (itemFromPoint == null)
            {
                listView.SelectedItems.Clear();
            }
            else if (e.Button != MouseButtons.Left && listView.SelectedItems.IndexOf(itemFromPoint.Key) == -1)
            {
                // Control has 'left click to select' built-in.
                // Must manually select when using other buttons.

                // If using right-click, make sure that the list doesn't
                // include the item before changing the selection.
                if (e.Button != MouseButtons.Right || listView.SelectedItems.IndexOf(itemFromPoint.Key) == -1)
                {
                    listView.SelectedItems.Clear(); // Select only the item you clicked

                    listView.SelectedItems.Add(itemFromPoint);
                    itemFromPoint.Activate();
                }
            }
        }

        #endregion

        #region FileListViewItem

        private class FileListViewItem : UltraListViewItem
        {
            #region Properties

            public FileListView FileList { get; set; }

            public DocumentsDataSet.DocumentInfoRow DocumentInfo { get; set; }

            #endregion

            #region Methods

            public FileListViewItem(DocumentsDataSet.DocumentInfoRow documentInfo, FileListView fileList) : base(documentInfo.DocumentInfoID.ToString())
            {
                FileList = fileList;
                DocumentInfo = documentInfo;
            }

            public void UpdateNodeUI()
            {
                Value                     = DocumentInfo.Name;
                SubItems["Version"].Value = DocumentInfo.CurrentRevision;
                Appearance.Image          = "Normal";

                var dockLock = DocumentManagerUtilities.GetCurrentDocumentLock(DocumentInfo);

                if(DocumentInfo.DocumentLocked && dockLock != null)
                {
                    DocumentsDataSet.UsersRow user = FileList._documents.Users.FindByUserID(dockLock.LockedByUser);
                    SubItems["Locked By"].Value = (user == null ? dockLock.LockedByUser.ToString() : user.Name) + (dockLock.IsComputerNameNull() ? " ()" : " (" + dockLock.ComputerName + ")");
                    Appearance.Image = "Locked";
                }
                else
                    SubItems["Locked By"].Value = String.Empty;

                SubItems["Status"].Value = GetFileStatus();
            }

            private FileStatus GetFileStatus()
            {
                try
                {
                    string filePath = Path.Combine(FileList.SelectedFolderPath, DocumentInfo.Name);

                    if(File.Exists(filePath))
                    {
                        var revision = DocumentManagerUtilities.GetCurrentDocumentRevision(DocumentInfo);

                        if(revision == null)
                            return FileStatus.Unknown;
                        
                        var fi = new FileInfo(filePath);

                        //Compare the file hash to verify status
                        if(!revision.IsFileHashNull())
                        {
                            string localFileHash = FileSystem.GetMD5HashFromFile(filePath);

                            if(localFileHash == revision.FileHash)
                                return FileStatus.Current;
                        }

                        //compare date time stamp
                        if(fi.LastWriteTimeUtc > revision.DateCreatedUTC)
                            return FileStatus.Modified;

                        if(fi.LastWriteTimeUtc < revision.DateCreatedUTC)
                            return FileStatus.Old;
                    }
                    else
                        return FileStatus.Missing;

                    return FileStatus.Unknown;
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error determining status of file.");
                    return FileStatus.Unknown;
                }
            }

            public override void Dispose()
            {
                FileList = null;
                DocumentInfo = null;

                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region FilesSelectedEventArgs

        public class FilesSelectedEventArgs : EventArgs
        {
            public List <DocumentsDataSet.DocumentInfoRow> Files { get; set; }
        }

        #endregion
    }
}