using System;
using System.Collections.Generic;
using System.Data;
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
using NLog;

namespace DWOS.UI.Documents.Controls
{
    public partial class ReplaceFile : Form
    {
        #region Fields

        private string _defaultFolderPath = null;
       
        #endregion

        #region Properties

        internal string FileName
        {

            get { return txtFileName.Text; }
            set { txtFileName.Text = value; }
        }

        internal string FileLocation
        {

            get { return txtLocalDirectory.Text; }
            set { txtLocalDirectory.Text = value; }
        }

        #endregion

        #region Methods

        public ReplaceFile() { InitializeComponent(); }

        public void LoadData(DocumentsDataSet.DocumentInfoRow documentInfo, string folderPath)
        {
            txtFileName.MaxLength = 255;
            this.FileName = documentInfo.Name;
            _defaultFolderPath = folderPath;
        }

        private void DisposeMe()
        {
           
        }

        #endregion

        #region Events

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                using (var fld = new OpenFileDialog())
                {
                    fld.InitialDirectory = _defaultFolderPath;

                    if (!String.IsNullOrWhiteSpace(this.txtLocalDirectory.Text))
                        fld.FileName = this.txtLocalDirectory.Text;

                    if(fld.ShowDialog() == DialogResult.OK)
                    {
                        txtLocalDirectory.Text = fld.FileName;
                        txtFileName.Text = System.IO.Path.GetFileName(fld.FileName);
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error selecting file.");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(!String.IsNullOrWhiteSpace(txtFileName.Text) && !String.IsNullOrWhiteSpace(txtLocalDirectory.Text))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion
    }
}