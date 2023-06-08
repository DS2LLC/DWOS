namespace DWOS.UI.Documents.Controls
{
    partial class GetFolder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetFolder));
            this.txtLocalDirectory = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lbLocalDirectory = new Infragistics.Win.Misc.UltraLabel();
            this.txtFolderName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.btnBrowse = new Infragistics.Win.Misc.UltraButton();
            this.chkRecursive = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            ((System.ComponentModel.ISupportInitialize)(this.txtLocalDirectory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFolderName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRecursive)).BeginInit();
            this.SuspendLayout();
            // 
            // txtLocalDirectory
            // 
            this.txtLocalDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocalDirectory.Location = new System.Drawing.Point(140, 51);
            this.txtLocalDirectory.Name = "txtLocalDirectory";
            this.txtLocalDirectory.ReadOnly = true;
            this.txtLocalDirectory.Size = new System.Drawing.Size(244, 22);
            this.txtLocalDirectory.TabIndex = 7;
            // 
            // lbLocalDirectory
            // 
            this.lbLocalDirectory.AutoSize = true;
            this.lbLocalDirectory.Location = new System.Drawing.Point(23, 55);
            this.lbLocalDirectory.Name = "lbLocalDirectory";
            this.lbLocalDirectory.Size = new System.Drawing.Size(94, 15);
            this.lbLocalDirectory.TabIndex = 6;
            this.lbLocalDirectory.Text = "Local Directory:";
            // 
            // txtFolderName
            // 
            this.txtFolderName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolderName.Location = new System.Drawing.Point(140, 23);
            this.txtFolderName.Name = "txtFolderName";
            this.txtFolderName.ReadOnly = true;
            this.txtFolderName.Size = new System.Drawing.Size(244, 22);
            this.txtFolderName.TabIndex = 5;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(23, 27);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(44, 15);
            this.ultraLabel1.TabIndex = 4;
            this.ultraLabel1.Text = "Folder:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(390, 102);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(308, 102);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(390, 51);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(76, 23);
            this.btnBrowse.TabIndex = 10;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // chkRecursive
            // 
            this.chkRecursive.AutoSize = true;
            this.chkRecursive.Location = new System.Drawing.Point(140, 79);
            this.chkRecursive.Name = "chkRecursive";
            this.chkRecursive.Size = new System.Drawing.Size(76, 18);
            this.chkRecursive.TabIndex = 11;
            this.chkRecursive.Text = "Recursive";
            // 
            // GetFolder
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(480, 137);
            this.Controls.Add(this.chkRecursive);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtLocalDirectory);
            this.Controls.Add(this.lbLocalDirectory);
            this.Controls.Add(this.txtFolderName);
            this.Controls.Add(this.ultraLabel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GetFolder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Get Folder";
            ((System.ComponentModel.ISupportInitialize)(this.txtLocalDirectory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFolderName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRecursive)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtLocalDirectory;
        private Infragistics.Win.Misc.UltraLabel lbLocalDirectory;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtFolderName;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraButton btnBrowse;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkRecursive;
    }
}