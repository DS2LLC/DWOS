using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.DocumentsDataSetTableAdapters;
using DWOS.Documents.Controls;
using DWOS.Shared.Utilities;
using Ionic.Zlib;
using NLog;

namespace DWOS.UI.Documents.Controls
{
    public partial class GetFolder : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private DocumentsDataSet.DocumentFolderRow _documentFolder;
        private DocumentsDataSet _documents;
        private FileListView _fileListView;

        #endregion

        #region Methods

        public GetFolder() { InitializeComponent(); }

        internal void LoadFile(FileListView fileListView, DocumentsDataSet documents, DocumentsDataSet.DocumentFolderRow documentFolder)
        {
            this._fileListView = fileListView;
            this._documents = documents;
            this._documentFolder = documentFolder;

            this.txtFolderName.Text = this._documentFolder.Name;
            this.txtLocalDirectory.Text = Path.Combine(DocumentManagerUtilities.RootFolderPath, DocumentManagerUtilities.BuildPath(this._documents, this._documentFolder));
        }

        private void DownloadFolders(string parentFolderPath, DocumentsDataSet.DocumentFolderRow documentFolder)
        {
            string folderPath = Path.Combine(parentFolderPath, documentFolder.Name);

            if(!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            this._fileListView.LoadData(documentFolder);
            DownloadFiles(folderPath, documentFolder);

            EnumerableRowCollection <DocumentsDataSet.DocumentFolderRow> childFolderRows = this._documents.DocumentFolder.Where(df => !df.IsParentIDNull() && df.ParentID == documentFolder.DocumentFolderID);

            foreach(DocumentsDataSet.DocumentFolderRow childFolderRow in childFolderRows)
                DownloadFolders(folderPath, childFolderRow);
        }

        private void DownloadFiles(string folderPath, DocumentsDataSet.DocumentFolderRow documentFolder)
        {
            try
            {
                _log.Info("Downloading files to " + folderPath);

                foreach(DocumentsDataSet.DocumentFolder_DocumentInfoRow docFolderInfos in documentFolder.GetDocumentFolder_DocumentInfoRows())
                    DownloadFile(folderPath, docFolderInfos.DocumentInfoRow);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error downloading file.");
            }
        }

        private void DownloadFile(string folderPath, DocumentsDataSet.DocumentInfoRow documentInfo)
        {
            try
            {
                DocumentsDataSet.DocumentRevisionRow docRevision = DocumentManagerUtilities.GetCurrentDocumentRevision(documentInfo);

                //get filename
                string fileName = docRevision.FileName;

                if(String.IsNullOrWhiteSpace(fileName))
                    return;

                //get file path
                string filePath = Path.Combine(folderPath, fileName);

                //if file exists then delete local copy
                if(File.Exists(filePath))
                {
                    FileSystem.SetFileAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                }

                using(var taDocumentRevision = new DocumentRevisionTableAdapter())
                {
                    _log.Info("Downloading file '{0}' to '{1}'.", fileName, filePath);
                    byte[] bytes = taDocumentRevision.GetMediaStream(docRevision.DocumentRevisionID);

                    if(bytes != null && bytes.Length > 0)
                    {
                        if(docRevision.IsCompressed)
                        {
                            _log.Info("UnCompressing file '{0}'", docRevision.FileName);
                            bytes = ZlibStream.UncompressBuffer(bytes);
                        }

                        using(var fileStream = new BinaryWriter(File.OpenWrite(filePath)))
                            fileStream.Write(bytes);
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error downloading file.");
            }
        }

        private void DisposeMe()
        {
            this._documentFolder = null;
            this._documents = null;
            this._fileListView = null;
        }

        #endregion

        #region Events

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                using(var fld = new FolderBrowserDialog())
                {
                    fld.SelectedPath = this.txtLocalDirectory.Text;

                    if(fld.ShowDialog() == DialogResult.OK)
                        this.txtLocalDirectory.Text = fld.SelectedPath;
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error browsing for folder.");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                //For selected folder only use the local directory
                string folderPath = this.txtLocalDirectory.Text;

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                DownloadFiles(folderPath, this._documentFolder);

                //if Recursive then do it for everyone
                if (this.chkRecursive.Checked)
                {
                    EnumerableRowCollection<DocumentsDataSet.DocumentFolderRow> childFolderRows = this._documents.DocumentFolder.Where(df => !df.IsParentIDNull() && df.ParentID == this._documentFolder.DocumentFolderID);

                    foreach (DocumentsDataSet.DocumentFolderRow documentFolderRow in childFolderRows)
                        DownloadFolders(folderPath, documentFolderRow);
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting folder.");
            }
        }

        #endregion
    }
}