using System;
using System.IO;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.DocumentsDataSetTableAdapters;
using DWOS.Shared.Utilities;
using Ionic.Zlib;
using NLog;

namespace DWOS.UI.Documents.Controls
{
    public partial class GetFile : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private DocumentsDataSet.DocumentFolderRow _documentFolder;
        private DocumentsDataSet.DocumentRevisionRow _documentRevision;
        private DocumentsDataSet _documents;

        #endregion

        #region Methods

        public GetFile() { InitializeComponent(); }

        internal void LoadFile(DocumentsDataSet documents, DocumentsDataSet.DocumentFolderRow documentFolder, DocumentsDataSet.DocumentInfoRow documentInfo, DocumentsDataSet.DocumentRevisionRow documentRevision)
        {
            this._documents = documents;
            this._documentFolder = documentFolder;
            this._documentRevision = documentRevision;

            this.txtFileName.Text = this._documentRevision.FileName;
            this.txtVersion.Text = this._documentRevision.RevisionNumber.ToString();
            this.txtLocalDirectory.Text = Path.Combine(DocumentManagerUtilities.RootFolderPath, DocumentManagerUtilities.BuildPath(this._documents, this._documentFolder));
        }

        private void DownloadFile()
        {
            try
            {
                //get directory 
                string directory = this.txtLocalDirectory.Text;
                if(String.IsNullOrWhiteSpace(directory))
                    return;

                //get filename
                string fileName = this.txtFileName.Text;

                if(String.IsNullOrWhiteSpace(fileName))
                    return;

                //get file path
                string filePath = Path.Combine(directory, fileName);

                if(!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                //if file exists then delete local copy
                if(File.Exists(filePath))
                {
                    FileSystem.SetFileAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                }

                using(var taDocumentRevision = new DocumentRevisionTableAdapter())
                {
                    _log.Info("Downloading file '{0}' to '{1}'.", fileName, filePath);
                    byte[] bytes = taDocumentRevision.GetMediaStream(this._documentRevision.DocumentRevisionID);

                    if(bytes != null && bytes.Length > 0)
                    {
                        if(this._documentRevision.IsCompressed)
                        {
                            _log.Info("UnCompressing file '{0}'", this._documentRevision.FileName);
                            bytes = ZlibStream.UncompressBuffer(bytes);
                        }

                        using(var fileStream = new BinaryWriter(File.OpenWrite(filePath)))
                            fileStream.Write(bytes);

                        FileSystem.SetFileAttributes(filePath, FileAttributes.ReadOnly);
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error downloading file.");
            }
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

        private void btnOK_Click(object sender, EventArgs e) { DownloadFile(); }

        #endregion
    }
}