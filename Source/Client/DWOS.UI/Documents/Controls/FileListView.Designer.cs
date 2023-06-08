namespace DWOS.Documents.Controls
{
    partial class FileListView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            DisposingMe();

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Version");
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn2 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Locked By");
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn3 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Status");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileListView));
            this.lvwFiles = new Infragistics.Win.UltraWinListView.UltraListView();
            this.imagelistDetails = new System.Windows.Forms.ImageList(this.components);
            this.taDocumentFolder_DocumentInfo = new DWOS.Data.Datasets.DocumentsDataSetTableAdapters.DocumentFolder_DocumentInfoTableAdapter();
            this.taDocumentInfo = new DWOS.Data.Datasets.DocumentsDataSetTableAdapters.DocumentInfoTableAdapter();
            this.taDocumentLock = new DWOS.Data.Datasets.DocumentsDataSetTableAdapters.DocumentLockTableAdapter();
            this.taDocumentRevision = new DWOS.Data.Datasets.DocumentsDataSetTableAdapters.DocumentRevisionTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.lvwFiles)).BeginInit();
            this.SuspendLayout();
            // 
            // lvwFiles
            // 
            this.lvwFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwFiles.ItemSettings.HideSelection = false;
            this.lvwFiles.ItemSettings.HotTracking = true;
            this.lvwFiles.Location = new System.Drawing.Point(0, 0);
            this.lvwFiles.MainColumn.DataType = typeof(string);
            this.lvwFiles.MainColumn.Text = "Name";
            this.lvwFiles.MainColumn.VisiblePositionInDetailsView = 0;
            this.lvwFiles.Name = "lvwFiles";
            this.lvwFiles.Size = new System.Drawing.Size(792, 418);
            ultraListViewSubItemColumn1.DataType = typeof(int);
            ultraListViewSubItemColumn1.Key = "Version";
            ultraListViewSubItemColumn1.VisiblePositionInDetailsView = 1;
            ultraListViewSubItemColumn2.DataType = typeof(string);
            ultraListViewSubItemColumn2.Key = "Locked By";
            ultraListViewSubItemColumn2.VisiblePositionInDetailsView = 3;
            ultraListViewSubItemColumn3.DataType = typeof(string);
            ultraListViewSubItemColumn3.Key = "Status";
            ultraListViewSubItemColumn3.VisiblePositionInDetailsView = 2;
            this.lvwFiles.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1,
            ultraListViewSubItemColumn2,
            ultraListViewSubItemColumn3});
            this.lvwFiles.TabIndex = 1;
            this.lvwFiles.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvwFiles.ViewSettingsDetails.ColumnAutoSizeMode = ((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header | Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.VisibleItems)));
            this.lvwFiles.ViewSettingsDetails.FullRowSelect = true;
            this.lvwFiles.ViewSettingsDetails.ImageList = this.imagelistDetails;
            this.lvwFiles.ItemDoubleClick += new Infragistics.Win.UltraWinListView.ItemDoubleClickEventHandler(this.lvwFiles_ItemDoubleClick);
            this.lvwFiles.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.lvwFiles_ItemSelectionChanged);
            this.lvwFiles.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvwFiles_MouseUp);
            // 
            // imagelistDetails
            // 
            this.imagelistDetails.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imagelistDetails.ImageStream")));
            this.imagelistDetails.TransparentColor = System.Drawing.Color.Transparent;
            this.imagelistDetails.Images.SetKeyName(0, "Locked");
            this.imagelistDetails.Images.SetKeyName(1, "Normal");
            // 
            // taDocumentFolder_DocumentInfo
            // 
            this.taDocumentFolder_DocumentInfo.ClearBeforeFill = false;
            // 
            // taDocumentInfo
            // 
            this.taDocumentInfo.ClearBeforeFill = false;
            // 
            // taDocumentLock
            // 
            this.taDocumentLock.ClearBeforeFill = false;
            // 
            // taDocumentRevision
            // 
            this.taDocumentRevision.ClearBeforeFill = false;
            // 
            // FileListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvwFiles);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FileListView";
            this.Size = new System.Drawing.Size(792, 418);
            ((System.ComponentModel.ISupportInitialize)(this.lvwFiles)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinListView.UltraListView lvwFiles;
        private Data.Datasets.DocumentsDataSetTableAdapters.DocumentFolder_DocumentInfoTableAdapter taDocumentFolder_DocumentInfo;
        private Data.Datasets.DocumentsDataSetTableAdapters.DocumentInfoTableAdapter taDocumentInfo;
        private Data.Datasets.DocumentsDataSetTableAdapters.DocumentLockTableAdapter taDocumentLock;
        private Data.Datasets.DocumentsDataSetTableAdapters.DocumentRevisionTableAdapter taDocumentRevision;
        private System.Windows.Forms.ImageList imagelistDetails;
    }
}
