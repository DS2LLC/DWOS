namespace DWOS.Server.Admin.Wizards.UpgradeWizard
{
    partial class BackupDatabase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BackupDatabase));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Data Folder", Infragistics.Win.DefaultableBoolean.Default);
            this.txtStatus = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.btnUpgrade = new Infragistics.Win.Misc.UltraButton();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.btnFolder = new Infragistics.Win.Misc.UltraButton();
            this.txtFilePath = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.prgBackup = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFilePath)).BeginInit();
            this.SuspendLayout();
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(3, 58);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(467, 100);
            this.txtStatus.TabIndex = 0;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(3, 37);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(46, 15);
            this.ultraLabel3.TabIndex = 28;
            this.ultraLabel3.Text = "Status:";
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpgrade.Location = new System.Drawing.Point(383, 164);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(87, 23);
            this.btnUpgrade.TabIndex = 29;
            this.btnUpgrade.Text = "Backup";
            this.btnUpgrade.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // btnFolder
            // 
            this.btnFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = ((object)(resources.GetObject("appearance1.Image")));
            this.btnFolder.Appearance = appearance1;
            this.btnFolder.AutoSize = true;
            this.btnFolder.Location = new System.Drawing.Point(440, 1);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(26, 26);
            this.btnFolder.TabIndex = 37;
            // 
            // txtFilePath
            // 
            this.txtFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilePath.Location = new System.Drawing.Point(119, 3);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(315, 22);
            this.txtFilePath.TabIndex = 36;
            this.txtFilePath.Text = "C:\\Data";
            ultraToolTipInfo1.ToolTipTextFormatted = "The folder where the data files will be stored.";
            ultraToolTipInfo1.ToolTipTitle = "Data Folder";
            this.tipManager.SetUltraToolTip(this.txtFilePath, ultraToolTipInfo1);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(7, 7);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(90, 15);
            this.ultraLabel2.TabIndex = 35;
            this.ultraLabel2.Text = "Backup Folder:";
            // 
            // prgBackup
            // 
            this.prgBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgBackup.Location = new System.Drawing.Point(3, 164);
            this.prgBackup.Name = "prgBackup";
            this.prgBackup.Size = new System.Drawing.Size(167, 23);
            this.prgBackup.TabIndex = 38;
            this.prgBackup.Text = "[Formatted]";
            // 
            // BackupDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.prgBackup);
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.btnUpgrade);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.txtStatus);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "BackupDatabase";
            this.Size = new System.Drawing.Size(474, 199);
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFilePath)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtStatus;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraButton btnUpgrade;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        private Infragistics.Win.Misc.UltraButton btnFolder;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtFilePath;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar prgBackup;
    }
}
