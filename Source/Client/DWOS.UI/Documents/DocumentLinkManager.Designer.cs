namespace DWOS.UI.Documents
{
    partial class DocumentLinkManager
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Deletes the currently selected document.", Infragistics.Win.ToolTipImage.Default, "Delete Document", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Adds a document.", Infragistics.Win.ToolTipImage.Default, "Add Document", Infragistics.Win.DefaultableBoolean.Default);
            this.tvwDocuments = new Infragistics.Win.UltraWinTree.UltraTree();
            this.btnDelete = new Infragistics.Win.Misc.UltraButton();
            this.btnAdd = new Infragistics.Win.Misc.UltraButton();
            this.taDocumentLink = new DWOS.Data.Datasets.DocumentsDataSetTableAdapters.DocumentLinkTableAdapter();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tvwDocuments)).BeginInit();
            this.SuspendLayout();
            // 
            // tvwDocuments
            // 
            this.tvwDocuments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.ImageBackground = global::DWOS.UI.Properties.Resources.Link_32;
            appearance1.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(0, 0, 32, 32);
            this.tvwDocuments.Appearance = appearance1;
            this.tvwDocuments.HideSelection = false;
            this.tvwDocuments.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tvwDocuments.Location = new System.Drawing.Point(35, 0);
            this.tvwDocuments.Name = "tvwDocuments";
            this.tvwDocuments.NodeConnectorColor = System.Drawing.SystemColors.ControlDark;
            this.tvwDocuments.Size = new System.Drawing.Size(400, 102);
            this.tvwDocuments.TabIndex = 25;
            this.tvwDocuments.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.tvwDocuments_AfterSelect);
            // 
            // btnDelete
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            this.btnDelete.Appearance = appearance2;
            this.btnDelete.AutoSize = true;
            this.btnDelete.Location = new System.Drawing.Point(3, 35);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(26, 26);
            this.btnDelete.TabIndex = 27;
            ultraToolTipInfo2.ToolTipText = "Deletes the currently selected document.";
            ultraToolTipInfo2.ToolTipTitle = "Delete Document";
            this.ultraToolTipManager.SetUltraToolTip(this.btnDelete, ultraToolTipInfo2);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            appearance3.Image = global::DWOS.UI.Properties.Resources.Add_16;
            this.btnAdd.Appearance = appearance3;
            this.btnAdd.AutoSize = true;
            this.btnAdd.Location = new System.Drawing.Point(3, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(26, 26);
            this.btnAdd.TabIndex = 26;
            ultraToolTipInfo1.ToolTipText = "Adds a document.";
            ultraToolTipInfo1.ToolTipTitle = "Add Document";
            this.ultraToolTipManager.SetUltraToolTip(this.btnAdd, ultraToolTipInfo1);
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // taDocumentLink
            // 
            this.taDocumentLink.ClearBeforeFill = false;
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // DocumentLinkManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tvwDocuments);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnAdd);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DocumentLinkManager";
            this.Size = new System.Drawing.Size(435, 102);
            ((System.ComponentModel.ISupportInitialize)(this.tvwDocuments)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinTree.UltraTree tvwDocuments;
        private Infragistics.Win.Misc.UltraButton btnDelete;
        private Infragistics.Win.Misc.UltraButton btnAdd;
        private Data.Datasets.DocumentsDataSetTableAdapters.DocumentLinkTableAdapter taDocumentLink;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
    }
}
