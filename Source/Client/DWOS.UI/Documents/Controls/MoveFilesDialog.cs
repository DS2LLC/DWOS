using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.DocumentsDataSetTableAdapters;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Documents.Controls
{
    public partial class MoveFilesDialog : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public DocumentsDataSet.DocumentFolderRow SourceFolderRow { get; set; }

        public DocumentsDataSet.DocumentFolderRow DestFolderRow
        {
            get { return this.moveFolderTOC.SelectedFolder; }
        }

        public DocumentsDataSet Documents { get; private set; }

        public List <DocumentsDataSet.DocumentInfoRow> SelectedFiles { get; private set; }

        #endregion

        #region Methods

        public MoveFilesDialog(DocumentsDataSet dataSet, DocumentsDataSet.DocumentFolderRow sourceFolderRow, List <DocumentsDataSet.DocumentInfoRow> files)
        {
            Documents       = dataSet;
            SelectedFiles   = files;
            SourceFolderRow = sourceFolderRow;

            InitializeComponent();
        }

        private void LoadDestinationRepository(DocumentsDataSet dataSet)
        {
            this.moveFolderTOC.InitialLoad(dataSet);
        }

        private void PopulateSelectedFiles(List <DocumentsDataSet.DocumentInfoRow> files)
        {
            this.tbxSelectedFiles.Text = files.ConvertAll(input => input.Name).Join(",");
        }
        
        private bool ExecuteMove()
        {
            try
            {
                using(var taDocFolderDocInfo = new DocumentFolder_DocumentInfoTableAdapter())
                {
                    //If Share is Checked then Add a new row
                    if(this.optMoveType.CheckedIndex == 1)
                    {
                        foreach(var file in SelectedFiles)
                        {
                            //does row exist already in destination
                            if(taDocFolderDocInfo.GetBy(DestFolderRow.DocumentFolderID, file.DocumentInfoID).Count > 0)
                            {
                                MessageBoxUtilities.ShowMessageBoxWarn("The file {0} already exists in the selected location. Please select another location.".FormatWith(file.Name), "File already exists");
                            }
                            else
                            {
                                var junctionRow = Documents.DocumentFolder_DocumentInfo.AddDocumentFolder_DocumentInfoRow(DestFolderRow, file);
                                taDocFolderDocInfo.Update(junctionRow);
                            }
                        }
                    }
                    else //else Move - Replace old FolderID with New FolderID
                    {
                        foreach(var file in SelectedFiles)
                        {
                            var junctionRow = Documents.DocumentFolder_DocumentInfo.FindByDocumentFolderIDDocumentInfoID(SourceFolderRow.DocumentFolderID, file.DocumentInfoID);

                            //does row exist already in destination
                            if (taDocFolderDocInfo.GetBy(DestFolderRow.DocumentFolderID, file.DocumentInfoID).Count > 0)
                            {
                                MessageBoxUtilities.ShowMessageBoxWarn("The file {0} already exists in the selected location. Please select another location.".FormatWith(file.Name), "File already exists");
                            }
                            else if(junctionRow != null)
                            {
                                junctionRow.DocumentFolderID = DestFolderRow.DocumentFolderID;
                                taDocFolderDocInfo.Update(junctionRow);
                            }
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                _log.Info(exc, "Error moving file to new folder.");
            }
            return true;
        }

        #endregion

        #region Events

        private void MoveFilesDialog_Load(object sender, EventArgs e)
        {
            LoadDestinationRepository(Documents);
            PopulateSelectedFiles(SelectedFiles);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(DestFolderRow != null)
            {
                if (ExecuteMove())
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            else
                MessageBoxUtilities.ShowMessageBoxWarn("A destination folder must be selected.", "Destination not selected");
        }

        #endregion
    }
}