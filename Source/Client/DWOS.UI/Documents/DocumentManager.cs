using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.DocumentsDataSetTableAdapters;
using DWOS.Documents.Controls;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Documents.Controls;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinToolbars;

namespace DWOS.UI.Documents
{
    public partial class DocumentManager : Form
    {
        #region Fields

        private CommandManager _commands;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of currently selected documents.
        /// </summary>
        public List<DocumentsDataSet.DocumentInfoRow> SelectedDocuments =>
            pnlFileListView.SelectedFiles;

        #endregion

        #region Methods

        public DocumentManager()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            this.pnlFolderTOC.InitialLoad(this.dsDocuments);
            this.pnlFileListView.InitialLoad(this.dsDocuments, DocumentManagerUtilities.RootFolderPath);
            this.pnlFolderTOC.AfterFolderSelected += pnlFolderTOC_AfterFolderSelected;
        }

        private void LoadCommands()
        {
            this._commands = new CommandManager();
            this._commands.AddCommand("AddFolder", new CreateFolderCommand(this.toolbarsManager.Tools["AddFolder"], this));
            this._commands.AddCommand("RenameFolder", new RenameFolderCommand(this.toolbarsManager.Tools["RenameFolder"], this));

            this._commands.AddCommand("AddFiles", new AddFileCommand(this.toolbarsManager.Tools["AddFiles"], this));
            this._commands.AddCommand("AddFileFolder", new AddFileFolderCommand(this.toolbarsManager.Tools["AddFileFolder"], this));
            this._commands.AddCommand("MoveFiles", new MoveFilesCommand(this.toolbarsManager.Tools["MoveFiles"], this));
            this._commands.AddCommand("DeleteFolder", new DeleteFolderCommand(this.toolbarsManager.Tools["DeleteFolder"], this));
            this._commands.AddCommand("ExploreFolder", new ExploreFolderCommand(this.toolbarsManager.Tools["ExploreFolder"], this));
            this._commands.AddCommand("GetFolder", new GetFolderCommand(this.toolbarsManager.Tools["GetFolder"], this));
            this._commands.AddCommand("FolderProperties", new FolderPropertiesCommand(this.toolbarsManager.Tools["FolderProperties"], this));

            this._commands.AddCommand("Refresh", new RefreshFilesCommand(this.toolbarsManager.Tools["Refresh"], this));
            this._commands.AddCommand("DeleteFile", new DeleteFileCommand(this.toolbarsManager.Tools["DeleteFile"], this));
            this._commands.AddCommand("GetFile", new GetFileCommand(this.toolbarsManager.Tools["GetFile"], this));
            this._commands.AddCommand("ViewFile", new ViewFileCommand(this.toolbarsManager.Tools["ViewFile"], this));
            this._commands.AddCommand("FileProperties", new FilePropertiesCommand(this.toolbarsManager.Tools["FileProperties"], this));

            this._commands.AddCommand("CheckOutFile", new CheckOutFileCommand(this.toolbarsManager.Tools["CheckOutFile"], this));
            this._commands.AddCommand("CheckInFile", new CheckInFileCommand(this.toolbarsManager.Tools["CheckInFile"], this));
            this._commands.AddCommand("UndoCheckOut", new UndoCheckOutFileCommand(this.toolbarsManager.Tools["UndoCheckOut"], this));

            this._commands.AddCommand("RenameFile", new RenameFileCommand(this.toolbarsManager.Tools["RenameFile"], this));
            this._commands.AddCommand("ReplaceFile", new ReplaceAndRenameFileCommand(this.toolbarsManager.Tools["ReplaceFile"], this));

            DocumentManagerSecurity.Current.SetCurrentFolder(null);
        }

        internal void RefreshCommands()
        {
            this._commands?.RefreshAll();
        }

        private void ResetButtons()
        {
            this.pnl1.Visible = true;
            this.pnl1.ClientArea.Controls.Add(this.btnOK);
            this.pnl2.Visible = true;
            this.pnl2.ClientArea.Controls.Add(this.btnClose);
            this.btnClose.Visible = true;
            this.btnOK.Visible = true;
        }

        private void DisposingMe()
        {
            if(this._commands != null)
            {
                ResetButtons();
                this._commands.Dispose();
                this._commands = null;
            }

            DocumentManagerSecurity.Current.Cleanup();
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e) { Close(); }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.dsDocuments.RejectChanges();
            Dispose();
            Close();
        }

        private void pnlFolderTOC_AfterFolderSelected(object sender, FolderTOC.FolderSelectedEventArgs e)
        {
            if(e.Folder != null)
                this.pnlFileListView.LoadFiles(e.Folder);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if(DesignMode)
                return;

            LoadData();
            LoadCommands();
        }

        #endregion

        #region Commands

        internal class AddFileCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Methods

            public AddFileCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.AddFile") { this._documentManager = documentManager; }

            public override void OnClick()
            {
                try
                {
                    var dlg = new OpenFileDialog {CheckFileExists = true, Title = "Add Files", Multiselect = true};

                    if (dlg.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    using(new UsingWaitCursor(_documentManager))
                    {
                        if (dlg.FileNames.Length > 0)
                        {
                            foreach (var fileName in dlg.FileNames)
                                AddFile(fileName);
                        }
                        else
                        {
                            AddFile(dlg.FileName);
                        }
                    }
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error adding new file(s).");
                }
            }

            private void AddFile(string fileName)
            {
                var fileInfo = new FileInfo(fileName);
                var fileValid = true;

                if (DocumentManagerUtilities.INVALID_EXTENSIONS.Contains(fileInfo.Extension, false))
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
                    var maxSize = Math.Round(FileSystem.ConvertBytesToMegabytes(DocumentManagerUtilities.MAX_UPLOAD_BYTES), 2);

                    var message = $"The file '{fileInfo.Name}' has a size " +
                        $"of {fileSize} MB, which is greater than the max " +
                        $"upload size of {maxSize} MB.\r\n\r\nAre you sure " +
                        $"you want to upload this file?";

                    if (MessageBoxUtilities.ShowMessageBoxYesOrNo(message, "Max File Size Exceeded") != System.Windows.Forms.DialogResult.Yes)
                        fileValid = false;
                }

                if(fileValid)
                    this._documentManager.pnlFileListView.AddFile(fileName, null);
            }

            public override void Dispose()
            {
                this._documentManager = null;
                base.Dispose();
            }

            #endregion
        }

        internal class AddFileFolderCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Methods

            public AddFileFolderCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.AddFile") { this._documentManager = documentManager; }

            public override void OnClick()
            {
                try
                {
                    using(var dlg = new ImportFolderDialog())
                    {
                        dlg.LoadDialog(_documentManager);
                        dlg.ShowDialog();
                    }
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error adding folder.");
                }
            }

            public override void Dispose()
            {
                this._documentManager = null;
                base.Dispose();
            }

            #endregion
        }

        internal class CreateFolderCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled || DocumentManagerSecurity.Current.IsDocumentAdministrator(); }
            }

            #endregion

            #region Methods

            public CreateFolderCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.AddFolder")
            {
                this._documentManager = documentManager;
            }

            public override void OnClick()
            {
                try
                {
                    this._documentManager.pnlFolderTOC.AddNewFolder();
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error adding a new folder.");
                }
            }

            public override void Dispose()
            {
                this._documentManager = null;
                base.Dispose();
            }

            #endregion
        }

        internal class CheckInFileCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._documentManager.pnlFileListView.SelectedFiles.Count > 0 && this._documentManager.pnlFileListView.SelectedFiles.All(di => di.DocumentLocked); }
            }

            #endregion

            #region Methods

            public CheckInFileCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.CheckOutFile")
            {
                this._documentManager = documentManager;
                this._documentManager.pnlFileListView.AfterFilesSelected += pnlFileListView_AfterFilesSelected;
            }

            public override void OnClick()
            {
                try
                {
                    this._documentManager.pnlFileListView.CheckInFiles(this._documentManager.pnlFileListView.SelectedFiles, null);
                    this._documentManager.RefreshCommands();
                }
                catch(Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error checking-in files.", exc);
                }
            }

            public override void Dispose()
            {
                this._documentManager.pnlFileListView.AfterFilesSelected -= pnlFileListView_AfterFilesSelected;
                this._documentManager = null;
                base.Dispose();
            }

            #endregion

            #region Events

            private void pnlFileListView_AfterFilesSelected(object sender, FileListView.FilesSelectedEventArgs e) { Refresh(); }

            #endregion
        }

        internal class CheckOutFileCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._documentManager.pnlFileListView.SelectedFiles.Count > 0 && this._documentManager.pnlFileListView.SelectedFiles.All(di => !di.DocumentLocked); }
            }

            #endregion

            #region Methods

            public CheckOutFileCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.CheckOutFile")
            {
                this._documentManager = documentManager;
                this._documentManager.pnlFileListView.AfterFilesSelected += pnlFileListView_AfterFilesSelected;
            }

            public override void OnClick()
            {
                try
                {
                    this._documentManager.pnlFileListView.CheckOutFiles(this._documentManager.pnlFileListView.SelectedFiles);
                    this._documentManager.RefreshCommands();
                }
                catch(Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error checking out files", exc);
                }
            }

            public override void Dispose()
            {
                this._documentManager.pnlFileListView.AfterFilesSelected -= pnlFileListView_AfterFilesSelected;
                this._documentManager = null;
                base.Dispose();
            }

            #endregion

            #region Events

            private void pnlFileListView_AfterFilesSelected(object sender, FileListView.FilesSelectedEventArgs e) { Refresh(); }

            #endregion
        }

        internal class RenameFileCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._documentManager.pnlFileListView.SelectedFiles.Count == 1 && !this._documentManager.pnlFileListView.SelectedFiles[0].DocumentLocked; }
            }

            #endregion

            #region Methods

            public RenameFileCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.CheckOutFile")
            {
                this._documentManager = documentManager;
                this._documentManager.pnlFileListView.AfterFilesSelected += pnlFileListView_AfterFilesSelected;
            }

            public override void OnClick()
            {
                try
                {
                    var docInfo = this._documentManager.pnlFileListView.SelectedFiles[0];

                    using(var form = new TextBoxForm())
                    {
                        form.Text = "Edit Name";
                        form.FormLabel.Text = "Name:";
                        form.FormTextBox.Text = docInfo.Name;
                        form.FormTextBox.MaxLength = _documentManager.dsDocuments.DocumentInfo.NameColumn.MaxLength;

                        if(form.ShowDialog(_documentManager) == DialogResult.OK && !String.IsNullOrWhiteSpace(form.FormTextBox.Text))
                        {
                            this._documentManager.pnlFileListView.RenameFile(docInfo, form.FormTextBox.Text);
                        }
                    }
                }
                catch (Exception exc)
                {
                    _log.Error(exc, "Error .");
                }
            }

            public override void Dispose()
            {
                this._documentManager.pnlFileListView.AfterFilesSelected -= pnlFileListView_AfterFilesSelected;
                this._documentManager = null;
                base.Dispose();
            }

            #endregion

            #region Events

            private void pnlFileListView_AfterFilesSelected(object sender, FileListView.FilesSelectedEventArgs e) { Refresh(); }

            #endregion
        }

        internal class ReplaceAndRenameFileCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._documentManager.pnlFileListView.SelectedFiles.Count == 1 && !this._documentManager.pnlFileListView.SelectedFiles[0].DocumentLocked; }
            }

            #endregion

            #region Methods

            public ReplaceAndRenameFileCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.CheckOutFile")
            {
                this._documentManager = documentManager;
                this._documentManager.pnlFileListView.AfterFilesSelected += pnlFileListView_AfterFilesSelected;
            }

            public override void OnClick()
            {
                try
                {
                    var docInfo = this._documentManager.pnlFileListView.SelectedFiles[0];

                    using (var form = new ReplaceFile())
                    {
                        form.LoadData(docInfo, _documentManager.pnlFileListView.SelectedFolderPath);

                        if (form.ShowDialog(_documentManager) == DialogResult.OK)
                        {
                            this._documentManager.pnlFileListView.ReplaceFile(docInfo, form.FileName, form.FileLocation);
                        }
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error replacing file.", exc);
                }
            }

            public override void Dispose()
            {
                this._documentManager.pnlFileListView.AfterFilesSelected -= pnlFileListView_AfterFilesSelected;
                this._documentManager = null;
                base.Dispose();
            }

            #endregion

            #region Events

            private void pnlFileListView_AfterFilesSelected(object sender, FileListView.FilesSelectedEventArgs e) { Refresh(); }

            #endregion
        }

        internal class DeleteFileCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._documentManager.pnlFileListView.SelectedFiles.Count > 0; }
            }

            #endregion

            #region Methods

            public DeleteFileCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.DeleteFile")
            {
                this._documentManager = documentManager;
                this._documentManager.pnlFileListView.AfterFilesSelected += pnlFileListView_AfterFilesSelected;
            }

            public override void OnClick()
            {
                try
                {
                    var selectedFiles = _documentManager.pnlFileListView.SelectedFiles;

                    var message = selectedFiles.Count == 1
                        ? $"Are you sure that you want to delete '{selectedFiles.First().Name}'?"
                        : $"Are you sure that you want to delete {selectedFiles.Count} files?";

                    var header = selectedFiles.Count == 1
                        ? "Delete File"
                        : "Delete Files";

                    if (MessageBoxUtilities.ShowMessageBoxYesOrNo(message, header) == DialogResult.Yes)
                    {
                        this._documentManager.pnlFileListView.DeleteFiles(selectedFiles, false);
                        this._documentManager.RefreshCommands();
                    }
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error .");
                }
            }

            public override void Dispose()
            {
                this._documentManager.pnlFileListView.AfterFilesSelected -= pnlFileListView_AfterFilesSelected;
                this._documentManager = null;
                base.Dispose();
            }

            #endregion

            #region Events

            private void pnlFileListView_AfterFilesSelected(object sender, FileListView.FilesSelectedEventArgs e) { Refresh(); }

            #endregion
        }

        internal class DeleteFolderCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._documentManager.pnlFolderTOC.SelectedFolder != null; }
            }

            #endregion

            #region Methods

            public DeleteFolderCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.DeleteFolder") { this._documentManager = documentManager; }

            public override void OnClick()
            {
                try
                {
                    var selectedFolder = _documentManager.pnlFolderTOC.SelectedFolder;
                    var message = $"Are you sure that you want to delete '{selectedFolder.Name}'?";

                    if (MessageBoxUtilities.ShowMessageBoxYesOrNo(message, "Delete Folder") == DialogResult.Yes)
                    {
                        this._documentManager.pnlFolderTOC.DeleteFolder(selectedFolder, false);
                        this._documentManager.RefreshCommands();
                    }
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error deleting folder.");
                }
            }

            public override void Dispose()
            {
                this._documentManager = null;
                base.Dispose();
            }

            #endregion
        }

        internal abstract class DocumentManagerCommandBase : CommandBase
        {
            #region Fields

            #endregion

            #region Properties

            public string DocumentSecurityRole { get; set; }

            public override bool Enabled
            {
                get { return DocumentSecurityRole == null || DocumentManagerSecurity.Current.IsInRole(DocumentSecurityRole); }
            }

            #endregion

            #region Methods

            protected DocumentManagerCommandBase(ToolBase tool, string securityRole) : base(tool)
            {
                DocumentSecurityRole = securityRole;
                DocumentManagerSecurity.Current.FolderChanged += Current_FolderChanged;
            }

            public override void Dispose()
            {
                DocumentManagerSecurity.Current.FolderChanged -= Current_FolderChanged;
                base.Dispose();
            }

            #endregion

            #region Events

            private void Current_FolderChanged(object sender, EventArgs e) { Refresh(); }

            #endregion
        }

        internal class ExploreFolderCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._documentManager.pnlFolderTOC.SelectedFolder != null; }
            }

            #endregion

            #region Methods

            public ExploreFolderCommand(ToolBase tool, DocumentManager documentManager) : base(tool, null) { this._documentManager = documentManager; }

            public override void OnClick()
            {
                try
                {
                    string folderPath = this._documentManager.pnlFileListView.SelectedFolderPath;

                    if(!String.IsNullOrWhiteSpace(folderPath))
                    {
                        if(!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        try
                        {
                            Process.Start(folderPath);
                        }
                        catch (Exception exc)
                        {
                            MessageBoxUtilities.ShowMessageBoxWarn("The program to display this folder is not found or is unable to start.", "Open Folder", folderPath);
                            _log.Debug(exc, "Error starting process for: " + folderPath);
                        }
                    }
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error exploring folder.");
                }
            }

            public override void Dispose()
            {
                this._documentManager = null;
                base.Dispose();
            }

            #endregion
        }

        internal class FilePropertiesCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._documentManager.pnlFileListView.SelectedFiles.Count == 1; }
            }

            #endregion

            #region Methods

            public FilePropertiesCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.FileProperties")
            {
                this._documentManager = documentManager;
                this._documentManager.pnlFileListView.AfterFilesSelected += pnlFileListView_AfterFilesSelected;
            }

            public override void OnClick()
            {
                try
                {
                    using(var dlg = new FileProperties())
                    {
                        dlg.LoadFile(this._documentManager.dsDocuments, this._documentManager.pnlFolderTOC.SelectedFolder, this._documentManager.pnlFileListView.SelectedFiles[0]);
                        dlg.ShowDialog(ActiveForm);

                        this._documentManager.pnlFileListView.RefreshNodes();
                    }
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error .");
                }
            }

            public override void Dispose()
            {
                this._documentManager.pnlFileListView.AfterFilesSelected -= pnlFileListView_AfterFilesSelected;
                this._documentManager = null;
                base.Dispose();
            }

            #endregion

            #region Events

            private void pnlFileListView_AfterFilesSelected(object sender, FileListView.FilesSelectedEventArgs e) { Refresh(); }

            #endregion
        }

        internal class FolderPropertiesCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return (base.Enabled || DocumentManagerSecurity.Current.IsDocumentAdministrator()) && this._documentManager.pnlFolderTOC.SelectedFolder != null; }
            }

            #endregion

            #region Methods

            public FolderPropertiesCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.FolderProperties") { this._documentManager = documentManager; }

            public override void OnClick()
            {
                try
                {
                    if(!this.Enabled)
                        return;

                    using(var dlg = new FolderProperties())
                    {
                        dlg.LoadFile(this._documentManager.dsDocuments, this._documentManager.pnlFolderTOC.SelectedFolder);
                        
                        if(dlg.ShowDialog(ActiveForm) == DialogResult.OK)
                        {
                            if(this._documentManager.dsDocuments.DocumentFolder_SecurityGroup.GetChanges() != null)
                            {
                                using(var ta = new DocumentFolder_SecurityGroupTableAdapter())
                                    ta.Update(this._documentManager.dsDocuments.DocumentFolder_SecurityGroup);
                            }
                            if(this._documentManager.pnlFolderTOC.SelectedFolderNode != null)
                                this._documentManager.pnlFolderTOC.SelectedFolderNode.UpdateNodeUI();

                            if(dlg.HasRestoredFiles)
                            {
                                //reload data and files as they could have had restored files
                                this._documentManager.pnlFileListView.LoadData(this._documentManager.pnlFileListView.SelectedFolder, true);
                                this._documentManager.pnlFileListView.LoadFiles(this._documentManager.pnlFileListView.SelectedFolder);
                            }

                            if(dlg.HasRestoredFolders)
                            {
                                //reload data and files as they could have had restored files
                                int folderID = this._documentManager.pnlFolderTOC.SelectedFolder.DocumentFolderID;
                                this._documentManager.pnlFolderTOC.InitialLoad(this._documentManager.dsDocuments);
                                this._documentManager.pnlFolderTOC.MoveToFolder(this._documentManager.dsDocuments.DocumentFolder.FindByDocumentFolderID(folderID));
                            }
                        }
                    }
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error displaying folder properties.");
                }
            }

            public override void Dispose()
            {
                this._documentManager = null;
                base.Dispose();
            }

            #endregion
        }

        internal class GetFileCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._documentManager.pnlFileListView.SelectedFiles.Count > 0; }
            }

            #endregion

            #region Methods

            public GetFileCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.GetFiles")
            {
                this._documentManager = documentManager;
                this._documentManager.pnlFileListView.AfterFilesSelected += pnlFileListView_AfterFilesSelected;
            }

            public override void OnClick()
            {
                try
                {
                    this._documentManager.pnlFileListView.GetFiles(this._documentManager.pnlFileListView.SelectedFiles, true);
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error .");
                }
            }

            public override void Dispose()
            {
                this._documentManager.pnlFileListView.AfterFilesSelected -= pnlFileListView_AfterFilesSelected;
                this._documentManager = null;
                base.Dispose();
            }

            #endregion

            #region Events

            private void pnlFileListView_AfterFilesSelected(object sender, FileListView.FilesSelectedEventArgs e) { Refresh(); }

            #endregion
        }

        internal class GetFolderCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._documentManager.pnlFolderTOC.SelectedFolder != null; }
            }

            #endregion

            #region Methods

            public GetFolderCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.GetFiles") { this._documentManager = documentManager; }

            public override void OnClick()
            {
                try
                {
                    using(var dlg = new GetFolder())
                    {
                        dlg.LoadFile(this._documentManager.pnlFileListView, this._documentManager.dsDocuments, this._documentManager.pnlFolderTOC.SelectedFolder);
                        dlg.ShowDialog(this._documentManager);
                        this._documentManager.pnlFileListView.RefreshNodes();
                        this._documentManager.Refresh();
                    }
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error deleting folder.");
                }
            }

            public override void Dispose()
            {
                this._documentManager = null;
                base.Dispose();
            }

            #endregion
        }

        internal class MoveFilesCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled || this._documentManager.pnlFileListView.SelectedFiles.Count > 0; }
            }

            #endregion

            #region Methods

            public MoveFilesCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.MoveFiles")
            {
                this._documentManager = documentManager;
                this._documentManager.pnlFileListView.AfterFilesSelected += pnlFileListView_AfterFilesSelected;
            }

            public override void OnClick()
            {
                try
                {
                    using(var dlg = new MoveFilesDialog(this._documentManager.dsDocuments, this._documentManager.pnlFolderTOC.SelectedFolder, this._documentManager.pnlFileListView.SelectedFiles))
                    {
                        if(dlg.ShowDialog() == DialogResult.OK)
                        {
                            //ensure current file list is refreshed
                            this._documentManager.pnlFileListView.LoadFiles(this._documentManager.pnlFolderTOC.SelectedFolder);
                        }
                    }
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error during move files.");
                }
            }

            public override void Dispose()
            {
                this._documentManager.pnlFileListView.AfterFilesSelected -= pnlFileListView_AfterFilesSelected;
                this._documentManager = null;
                base.Dispose();
            }

            #endregion

            #region Events

            private void pnlFileListView_AfterFilesSelected(object sender, FileListView.FilesSelectedEventArgs e) { Refresh(); }

            #endregion
        }

        internal class RefreshFilesCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._documentManager.pnlFolderTOC.SelectedFolder != null; }
            }

            #endregion

            #region Methods

            public RefreshFilesCommand(ToolBase tool, DocumentManager documentManager) : base(tool, null) { this._documentManager = documentManager; }

            public override void OnClick()
            {
                try
                {
                    this._documentManager.pnlFileListView.RefreshNodes();
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error renaming folder.");
                }
            }

            public override void Dispose()
            {
                this._documentManager = null;
                base.Dispose();
            }

            #endregion
        }

        internal class RenameFolderCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._documentManager.pnlFolderTOC.SelectedFolder != null; }
            }

            #endregion

            #region Methods

            public RenameFolderCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.RenameFolder") { this._documentManager = documentManager; }

            public override void OnClick()
            {
                try
                {
                    using(var tb = new TextBoxForm())
                    {
                        tb.Text = "Rename Folder";
                        tb.FormLabel.Text = "Folder Name:";
                        tb.FormTextBox.Text = this._documentManager.pnlFolderTOC.SelectedFolder.Name;
                        tb.FormTextBox.Focus();

                        tb.Validator.Add(new ImageDisplayValidator(new TextControlValidator(tb.FormTextBox, "User name required."), tb.ErrorProvider));

                        if(tb.ShowDialog(ActiveForm) == DialogResult.OK)
                        {
                            this._documentManager.pnlFolderTOC.RenameFolder(this._documentManager.pnlFolderTOC.SelectedFolder, tb.FormTextBox.Text);
                            this._documentManager.pnlFileListView.RefreshNodes();
                        }
                    }
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error renaming folder.");
                }
            }

            public override void Dispose()
            {
                this._documentManager = null;
                base.Dispose();
            }

            #endregion
        }

        internal class UndoCheckOutFileCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._documentManager.pnlFileListView.SelectedFiles.Count > 0 && this._documentManager.pnlFileListView.SelectedFiles.All(di => di.DocumentLocked); }
            }

            #endregion

            #region Methods

            public UndoCheckOutFileCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.UndoCheckOutFile")
            {
                this._documentManager = documentManager;
                this._documentManager.pnlFileListView.AfterFilesSelected += pnlFileListView_AfterFilesSelected;
            }

            public override void OnClick()
            {
                try
                {
                    this._documentManager.pnlFileListView.UndoCheckOutFiles(this._documentManager.pnlFileListView.SelectedFiles);
                    this._documentManager.RefreshCommands();
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error .");
                }
            }

            public override void Dispose()
            {
                this._documentManager.pnlFileListView.AfterFilesSelected -= pnlFileListView_AfterFilesSelected;
                this._documentManager = null;
                base.Dispose();
            }

            #endregion

            #region Events

            private void pnlFileListView_AfterFilesSelected(object sender, FileListView.FilesSelectedEventArgs e) { Refresh(); }

            #endregion
        }

        internal class ViewFileCommand : DocumentManagerCommandBase
        {
            #region Fields

            private DocumentManager _documentManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && this._documentManager.pnlFileListView.SelectedFiles.Count > 0; }
            }

            #endregion

            #region Methods

            public ViewFileCommand(ToolBase tool, DocumentManager documentManager) : base(tool, "Documents.ViewFile")
            {
                this._documentManager = documentManager;
                this._documentManager.pnlFileListView.AfterFilesSelected += pnlFileListView_AfterFilesSelected;
                this._documentManager.pnlFileListView.AfterFileDoubleClicked += pnlFileListView_AfterFileDoubleClicked;
            }

            public override void OnClick()
            {
                try
                {
                    this._documentManager.pnlFileListView.ViewFile(this._documentManager.pnlFileListView.SelectedFiles.FirstOrDefault());
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error showing file.");
                }
            }

            public override void Dispose()
            {
                this._documentManager.pnlFileListView.AfterFilesSelected -= pnlFileListView_AfterFilesSelected;
                this._documentManager.pnlFileListView.AfterFileDoubleClicked -= pnlFileListView_AfterFileDoubleClicked;
                this._documentManager = null;
                base.Dispose();
            }

            #endregion

            #region Events

            private void pnlFileListView_AfterFilesSelected(object sender, FileListView.FilesSelectedEventArgs e) { Refresh(); }

            private void pnlFileListView_AfterFileDoubleClicked(object sender, FileListView.FilesSelectedEventArgs e) { OnClick(); }

            #endregion
        }

        #endregion
    }
}