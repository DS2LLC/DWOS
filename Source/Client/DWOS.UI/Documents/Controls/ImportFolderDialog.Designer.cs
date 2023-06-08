namespace DWOS.UI.Documents.Controls
{
    partial class ImportFolderDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportFolderDialog));
            this.btnBrowse = new Infragistics.Win.Misc.UltraButton();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.txtLocalDirectory = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lbLocalDirectory = new Infragistics.Win.Misc.UltraLabel();
            this.chkRecursive = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.prgFileStatus = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.txtLocalDirectory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRecursive)).BeginInit();
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(397, 12);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(76, 23);
            this.btnBrowse.TabIndex = 15;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(397, 75);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(315, 75);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtLocalDirectory
            // 
            this.txtLocalDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocalDirectory.Location = new System.Drawing.Point(125, 12);
            this.txtLocalDirectory.Name = "txtLocalDirectory";
            this.txtLocalDirectory.Size = new System.Drawing.Size(266, 22);
            this.txtLocalDirectory.TabIndex = 12;
            // 
            // lbLocalDirectory
            // 
            this.lbLocalDirectory.AutoSize = true;
            this.lbLocalDirectory.Location = new System.Drawing.Point(12, 16);
            this.lbLocalDirectory.Name = "lbLocalDirectory";
            this.lbLocalDirectory.Size = new System.Drawing.Size(94, 15);
            this.lbLocalDirectory.TabIndex = 11;
            this.lbLocalDirectory.Text = "Local Directory:";
            // 
            // chkRecursive
            // 
            this.chkRecursive.Location = new System.Drawing.Point(125, 40);
            this.chkRecursive.Name = "chkRecursive";
            this.chkRecursive.Size = new System.Drawing.Size(120, 20);
            this.chkRecursive.TabIndex = 16;
            this.chkRecursive.Text = "Recursive";
            // 
            // prgFileStatus
            // 
            this.prgFileStatus.Location = new System.Drawing.Point(12, 75);
            this.prgFileStatus.Name = "prgFileStatus";
            this.prgFileStatus.Size = new System.Drawing.Size(297, 23);
            this.prgFileStatus.TabIndex = 17;
            this.prgFileStatus.Text = "[Formatted]";
            this.prgFileStatus.Visible = false;
            // 
            // ImportFolderDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(485, 110);
            this.Controls.Add(this.prgFileStatus);
            this.Controls.Add(this.chkRecursive);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtLocalDirectory);
            this.Controls.Add(this.lbLocalDirectory);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportFolderDialog";
            this.Text = "Add Folder";
            ((System.ComponentModel.ISupportInitialize)(this.txtLocalDirectory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRecursive)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnBrowse;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtLocalDirectory;
        private Infragistics.Win.Misc.UltraLabel lbLocalDirectory;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkRecursive;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar prgFileStatus;
    }
}