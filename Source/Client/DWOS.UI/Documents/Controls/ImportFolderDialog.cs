using System;
using System.IO;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Shared.Utilities;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Documents.Controls
{
    public partial class ImportFolderDialog : Form
    {
        #region Fields

        private DocumentManager _documentManager;

        #endregion

        #region Properties

        private string SelectedPath =>
            txtLocalDirectory.Text;

        private bool IsRecursive =>
            chkRecursive.Checked;

        #endregion

        #region Methods

        public ImportFolderDialog() { InitializeComponent(); }

        public void LoadDialog(DocumentManager documentManager)
        {
            _documentManager = documentManager;
        }

        private void StartStatusbar()
        {
            prgFileStatus.Minimum = 0;
            prgFileStatus.Maximum = Directory.GetFiles(SelectedPath, "*.*", SearchOption.AllDirectories).Length;
            prgFileStatus.Value = 0;
            prgFileStatus.Visible = true;
            this.Refresh();
        }

        private void AddFolder(DocumentsDataSet.DocumentFolderRow parentFolder, string folderPath, bool recursive)
        {
            _documentManager.pnlFolderTOC.MoveToFolder(parentFolder);

            //create new folder then move to it
            var folderName  = new DirectoryInfo(folderPath).Name;
            var folderRow   = this._documentManager.pnlFolderTOC.AddNewFolder(folderName);
            _documentManager.pnlFolderTOC.MoveToFolder(folderRow);
            
            foreach (var filePath in Directory.GetFiles(folderPath))
            {
                var fileInfo = new FileInfo(filePath);
                var fileValid = true;

                if(fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                    fileValid = false;
                else
                {
                    if(DocumentManagerUtilities.INVALID_EXTENSIONS.Contains(fileInfo.Extension, false))
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            $"The '{fileInfo.Name}' file has an invalid extension.",
                            "Invalid Extension",
                            "The server has disallowed the upload of that extension.");

                        fileValid = false;
                    }
                    
                    if (fileValid && fileInfo.Length > DocumentManagerUtilities.MAX_UPLOAD_BYTES)
                    {
                        var fileSize = Math.Round(FileSystem.ConvertBytesToMegabytes(fileInfo.Length), 2);
                        var maxSize  =  Math.Round(FileSystem.ConvertBytesToMegabytes(DocumentManagerUtilities.MAX_UPLOAD_BYTES), 2);

                        var message = $"The file '{fileInfo.Name}' has a size " +
                            $"of {fileSize} MB, which is greater than the max " +
                            $"upload size of {maxSize} MB.\r\n\r\nAre you sure " +
                            $"you want to upload this file?";

                        if (MessageBoxUtilities.ShowMessageBoxYesOrNo(message, "Max File Size Exceeded") != System.Windows.Forms.DialogResult.Yes)
                            fileValid = false;
                    }
                }

                if(fileValid)
                    this._documentManager.pnlFileListView.AddFile(filePath, null);

                if (prgFileStatus.Value < prgFileStatus.Maximum)
                {
                    prgFileStatus.IncrementValue(1);
                    prgFileStatus.Refresh();
                }
            }

            if (recursive)
            {
                foreach (var directory in Directory.GetDirectories(folderPath))
                {
                    //add all files to new folder
                    AddFolder(folderRow, directory, recursive);
                }
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(SelectedPath))
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("A selected folder is required.", "No Folder Selected");
                    return;
                }

                if (!Directory.Exists(SelectedPath))
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("The selected folder does not exist.", "Folder Does Not Exit");
                    return;
                }

                using(new UsingWaitCursor(this))
                {
                    StartStatusbar();
                    AddFolder(_documentManager.pnlFolderTOC.SelectedFolder, this.SelectedPath, this.IsRecursive);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error adding folder.");
            }
        }

        #endregion
    }
}