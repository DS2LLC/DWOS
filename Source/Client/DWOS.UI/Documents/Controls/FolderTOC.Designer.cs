namespace DWOS.Documents.Controls
{
    partial class FolderTOC
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
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderTOC));
            this.tvwFolders = new Infragistics.Win.UltraWinTree.UltraTree();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.taDocumentFolder = new DWOS.Data.Datasets.DocumentsDataSetTableAdapters.DocumentFolderTableAdapter();
            this.taDocumentFolder_SecurityGroup = new DWOS.Data.Datasets.DocumentsDataSetTableAdapters.DocumentFolder_SecurityGroupTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.tvwFolders)).BeginInit();
            this.SuspendLayout();
            // 
            // tvwFolders
            // 
            this.tvwFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwFolders.FullRowSelect = true;
            this.tvwFolders.HideSelection = false;
            this.tvwFolders.ImageList = this.imageList1;
            this.tvwFolders.Location = new System.Drawing.Point(0, 0);
            this.tvwFolders.Name = "tvwFolders";
            _override1.HotTracking = Infragistics.Win.DefaultableBoolean.True;
            _override1.Sort = Infragistics.Win.UltraWinTree.SortType.Ascending;
            this.tvwFolders.Override = _override1;
            this.tvwFolders.Scrollable = Infragistics.Win.UltraWinTree.Scrollbar.ShowIfNeeded;
            this.tvwFolders.Size = new System.Drawing.Size(408, 357);
            this.tvwFolders.TabIndex = 1;
            this.tvwFolders.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.tvwFolders_AfterSelect);
            this.tvwFolders.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvwFolders_MouseUp);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Folder_16");
            // 
            // taDocumentFolder
            // 
            this.taDocumentFolder.ClearBeforeFill = true;
            // 
            // taDocumentFolder_SecurityGroup
            // 
            this.taDocumentFolder_SecurityGroup.ClearBeforeFill = true;
            // 
            // FolderTOC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tvwFolders);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FolderTOC";
            this.Size = new System.Drawing.Size(408, 357);
            ((System.ComponentModel.ISupportInitialize)(this.tvwFolders)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTree.UltraTree tvwFolders;
        private Data.Datasets.DocumentsDataSetTableAdapters.DocumentFolderTableAdapter taDocumentFolder;
        private System.Windows.Forms.ImageList imageList1;
        private Data.Datasets.DocumentsDataSetTableAdapters.DocumentFolder_SecurityGroupTableAdapter taDocumentFolder_SecurityGroup;
    }
}
