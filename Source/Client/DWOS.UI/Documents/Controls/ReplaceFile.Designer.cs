namespace DWOS.UI.Documents.Controls
{
    partial class ReplaceFile
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
            DisposeMe();
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The name of the file.", Infragistics.Win.ToolTipImage.Default, "File Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The location of the file to copy and replace over the existing file.", Infragistics.Win.ToolTipImage.Default, "File Location", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The location of the file to copy and replace over the existing file.", Infragistics.Win.ToolTipImage.Default, "File Location", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReplaceFile));
            this.txtFileName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.btnClose = new Infragistics.Win.Misc.UltraButton();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.txtLocalDirectory = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lbLocalDirectory = new Infragistics.Win.Misc.UltraLabel();
            this.btnBrowse = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtFileName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLocalDirectory)).BeginInit();
            this.SuspendLayout();
            // 
            // txtFileName
            // 
            this.txtFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileName.Location = new System.Drawing.Point(130, 28);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(238, 22);
            this.txtFileName.TabIndex = 1;
            ultraToolTipInfo3.ToolTipText = "The name of the file.";
            ultraToolTipInfo3.ToolTipTitle = "File Name";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtFileName, ultraToolTipInfo3);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(21, 33);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(66, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "File Name:";
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "folder");
            this.imageList.Images.SetKeyName(1, "link");
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(374, 97);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(76, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // txtLocalDirectory
            // 
            this.txtLocalDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocalDirectory.Location = new System.Drawing.Point(130, 56);
            this.txtLocalDirectory.Name = "txtLocalDirectory";
            this.txtLocalDirectory.ReadOnly = true;
            this.txtLocalDirectory.Size = new System.Drawing.Size(238, 22);
            this.txtLocalDirectory.TabIndex = 12;
            ultraToolTipInfo1.ToolTipText = "The location of the file to copy and replace over the existing file.";
            ultraToolTipInfo1.ToolTipTitle = "File Location";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtLocalDirectory, ultraToolTipInfo1);
            // 
            // lbLocalDirectory
            // 
            this.lbLocalDirectory.AutoSize = true;
            this.lbLocalDirectory.Location = new System.Drawing.Point(21, 61);
            this.lbLocalDirectory.Name = "lbLocalDirectory";
            this.lbLocalDirectory.Size = new System.Drawing.Size(80, 15);
            this.lbLocalDirectory.TabIndex = 11;
            this.lbLocalDirectory.Text = "File Location:";
            ultraToolTipInfo2.ToolTipText = "The location of the file to copy and replace over the existing file.";
            ultraToolTipInfo2.ToolTipTitle = "File Location";
            this.ultraToolTipManager1.SetUltraToolTip(this.lbLocalDirectory, ultraToolTipInfo2);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(374, 56);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(76, 23);
            this.btnBrowse.TabIndex = 14;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(292, 96);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ReplaceFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(464, 132);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtLocalDirectory);
            this.Controls.Add(this.lbLocalDirectory);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.ultraLabel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "ReplaceFile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Replace File";
            ((System.ComponentModel.ISupportInitialize)(this.txtFileName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLocalDirectory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtFileName;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraButton btnClose;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private System.Windows.Forms.ImageList imageList;
        private Infragistics.Win.Misc.UltraButton btnBrowse;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtLocalDirectory;
        private Infragistics.Win.Misc.UltraLabel lbLocalDirectory;
    }
}