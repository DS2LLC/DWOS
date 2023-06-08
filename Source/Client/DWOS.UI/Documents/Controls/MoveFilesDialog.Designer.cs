namespace DWOS.UI.Documents.Controls
{
    partial class MoveFilesDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Selected files to move", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Cancel all changes and close dialog", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Execute the move and close dialog", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Move Type", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The Destination Repository", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.True);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MoveFilesDialog));
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.tbxSelectedFiles = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.optMoveType = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
            this.moveFolderTOC = new DWOS.Documents.Controls.FolderTOC();
            ((System.ComponentModel.ISupportInitialize)(this.tbxSelectedFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.optMoveType)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.ultraLabel1.Location = new System.Drawing.Point(14, 22);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(101, 23);
            this.ultraLabel1.TabIndex = 1;
            this.ultraLabel1.Text = "Selected Files:";
            // 
            // tbxSelectedFiles
            // 
            this.tbxSelectedFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxSelectedFiles.Enabled = false;
            this.tbxSelectedFiles.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxSelectedFiles.Location = new System.Drawing.Point(122, 18);
            this.tbxSelectedFiles.Name = "tbxSelectedFiles";
            this.tbxSelectedFiles.ReadOnly = true;
            this.tbxSelectedFiles.Size = new System.Drawing.Size(333, 22);
            this.tbxSelectedFiles.TabIndex = 9;
            ultraToolTipInfo5.ToolTipText = "Selected files to move";
            this.ultraToolTipManager1.SetUltraToolTip(this.tbxSelectedFiles, ultraToolTipInfo5);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.ultraLabel3.Location = new System.Drawing.Point(14, 53);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(206, 23);
            this.ultraLabel3.TabIndex = 10;
            this.ultraLabel3.Text = "Destination Repository:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(368, 299);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            ultraToolTipInfo2.ToolTipText = "Cancel all changes and close dialog";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnCancel, ultraToolTipInfo2);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(274, 299);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(87, 23);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            ultraToolTipInfo3.ToolTipText = "Execute the move and close dialog";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnOK, ultraToolTipInfo3);
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // optMoveType
            // 
            this.optMoveType.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.optMoveType.CheckedIndex = 0;
            valueListItem1.DataValue = "Default Item";
            valueListItem1.DisplayText = "Move Selected Files";
            valueListItem2.DataValue = "ValueListItem1";
            valueListItem2.DisplayText = "Share Selected Files";
            this.optMoveType.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.optMoveType.ItemSpacingVertical = 4;
            this.optMoveType.Location = new System.Drawing.Point(14, 256);
            this.optMoveType.Name = "optMoveType";
            this.optMoveType.Size = new System.Drawing.Size(235, 40);
            this.optMoveType.TabIndex = 15;
            this.optMoveType.Text = "Move Selected Files";
            ultraToolTipInfo1.ToolTipTextFormatted = "Select the type of move you want to perform:<br/>- Move: This will move from one " +
    "location to another location<br/>- Share: This will create a link to the files s" +
    "hared between different folders<br/>";
            ultraToolTipInfo1.ToolTipTitle = "Move Type";
            this.ultraToolTipManager1.SetUltraToolTip(this.optMoveType, ultraToolTipInfo1);
            // 
            // moveFolderTOC
            // 
            this.moveFolderTOC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.moveFolderTOC.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.moveFolderTOC.Location = new System.Drawing.Point(14, 71);
            this.moveFolderTOC.Name = "moveFolderTOC";
            this.moveFolderTOC.Size = new System.Drawing.Size(442, 179);
            this.moveFolderTOC.TabIndex = 11;
            ultraToolTipInfo4.Enabled = Infragistics.Win.DefaultableBoolean.True;
            ultraToolTipInfo4.ToolTipText = "The Destination Repository";
            this.ultraToolTipManager1.SetUltraToolTip(this.moveFolderTOC, ultraToolTipInfo4);
            // 
            // MoveFilesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 336);
            this.Controls.Add(this.optMoveType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.moveFolderTOC);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.tbxSelectedFiles);
            this.Controls.Add(this.ultraLabel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MoveFilesDialog";
            this.Text = "Move Files";
            this.Load += new System.EventHandler(this.MoveFilesDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tbxSelectedFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.optMoveType)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor tbxSelectedFiles;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private DWOS.Documents.Controls.FolderTOC moveFolderTOC;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.UltraWinEditors.UltraOptionSet optMoveType;
    }
}