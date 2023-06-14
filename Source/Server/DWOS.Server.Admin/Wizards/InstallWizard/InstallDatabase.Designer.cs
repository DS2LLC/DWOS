namespace DWOS.Server.Admin.Wizards.InstallWizard
{
    partial class InstallDatabase
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Install SQL Server Express", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Existing Database", Infragistics.Win.DefaultableBoolean.Default);
            this.chkInstallDB = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkUseExistingDB = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.btnInstall = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.prgDownload = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.prgInstall = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.grpInstall = new Infragistics.Win.Misc.UltraGroupBox();
            this.lblInstallStatus = new Infragistics.Win.Misc.UltraLabel();
            this.lblDownloadStatus = new Infragistics.Win.Misc.UltraLabel();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.chkInstallDB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkUseExistingDB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpInstall)).BeginInit();
            this.grpInstall.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkInstallDB
            // 
            this.chkInstallDB.AutoSize = true;
            this.chkInstallDB.Checked = true;
            this.chkInstallDB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkInstallDB.Location = new System.Drawing.Point(14, 12);
            this.chkInstallDB.Name = "chkInstallDB";
            this.chkInstallDB.Size = new System.Drawing.Size(262, 18);
            this.chkInstallDB.TabIndex = 0;
            this.chkInstallDB.Text = "Install Microsoft SQL Server Express 2017";
            ultraToolTipInfo2.ToolTipTextFormatted = "Check this box if you want to install MS SQL Server Express 2012.";
            ultraToolTipInfo2.ToolTipTitle = "Install SQL Server Express";
            this.tipManager.SetUltraToolTip(this.chkInstallDB, ultraToolTipInfo2);
            this.chkInstallDB.CheckedChanged += new System.EventHandler(this.chkInstallDB_CheckedChanged);
            // 
            // chkUseExistingDB
            // 
            this.chkUseExistingDB.AutoSize = true;
            this.chkUseExistingDB.Location = new System.Drawing.Point(14, 155);
            this.chkUseExistingDB.Name = "chkUseExistingDB";
            this.chkUseExistingDB.Size = new System.Drawing.Size(324, 18);
            this.chkUseExistingDB.TabIndex = 1;
            this.chkUseExistingDB.Text = "Use existing Microsoft SQL Server Database (2012+)";
            ultraToolTipInfo1.ToolTipTextFormatted = "Check this box if you have an exsiting MS SQL Server already installed.";
            ultraToolTipInfo1.ToolTipTitle = "Existing Database";
            this.tipManager.SetUltraToolTip(this.chkUseExistingDB, ultraToolTipInfo1);
            this.chkUseExistingDB.CheckedChanged += new System.EventHandler(this.chkUseExistingDB_CheckedChanged);
            // 
            // btnInstall
            // 
            this.btnInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInstall.Location = new System.Drawing.Point(307, 82);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(87, 23);
            this.btnInstall.TabIndex = 2;
            this.btnInstall.Text = "Install";
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(25, 24);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(65, 15);
            this.ultraLabel1.TabIndex = 3;
            this.ultraLabel1.Text = "Download:";
            // 
            // prgDownload
            // 
            this.prgDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgDownload.Location = new System.Drawing.Point(101, 20);
            this.prgDownload.Name = "prgDownload";
            this.prgDownload.Size = new System.Drawing.Size(174, 23);
            this.prgDownload.TabIndex = 4;
            this.prgDownload.Text = "[Formatted]";
            // 
            // prgInstall
            // 
            this.prgInstall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgInstall.Location = new System.Drawing.Point(101, 49);
            this.prgInstall.Name = "prgInstall";
            this.prgInstall.Size = new System.Drawing.Size(174, 23);
            this.prgInstall.TabIndex = 6;
            this.prgInstall.Text = "[Formatted]";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(25, 53);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(45, 15);
            this.ultraLabel2.TabIndex = 5;
            this.ultraLabel2.Text = "Install:";
            // 
            // grpInstall
            // 
            this.grpInstall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpInstall.Controls.Add(this.lblInstallStatus);
            this.grpInstall.Controls.Add(this.lblDownloadStatus);
            this.grpInstall.Controls.Add(this.prgDownload);
            this.grpInstall.Controls.Add(this.prgInstall);
            this.grpInstall.Controls.Add(this.btnInstall);
            this.grpInstall.Controls.Add(this.ultraLabel2);
            this.grpInstall.Controls.Add(this.ultraLabel1);
            this.grpInstall.Location = new System.Drawing.Point(39, 38);
            this.grpInstall.Name = "grpInstall";
            this.grpInstall.Size = new System.Drawing.Size(400, 111);
            this.grpInstall.TabIndex = 7;
            this.grpInstall.Text = "Installation";
            // 
            // lblInstallStatus
            // 
            this.lblInstallStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInstallStatus.Location = new System.Drawing.Point(281, 53);
            this.lblInstallStatus.Name = "lblInstallStatus";
            this.lblInstallStatus.Size = new System.Drawing.Size(113, 15);
            this.lblInstallStatus.TabIndex = 8;
            this.lblInstallStatus.Text = "------------";
            // 
            // lblDownloadStatus
            // 
            this.lblDownloadStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDownloadStatus.Location = new System.Drawing.Point(281, 24);
            this.lblDownloadStatus.Name = "lblDownloadStatus";
            this.lblDownloadStatus.Size = new System.Drawing.Size(113, 15);
            this.lblDownloadStatus.TabIndex = 7;
            this.lblDownloadStatus.Text = "------------";
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // InstallDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpInstall);
            this.Controls.Add(this.chkUseExistingDB);
            this.Controls.Add(this.chkInstallDB);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "InstallDatabase";
            this.Size = new System.Drawing.Size(442, 245);
            ((System.ComponentModel.ISupportInitialize)(this.chkInstallDB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkUseExistingDB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpInstall)).EndInit();
            this.grpInstall.ResumeLayout(false);
            this.grpInstall.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkInstallDB;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkUseExistingDB;
        private Infragistics.Win.Misc.UltraButton btnInstall;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar prgDownload;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar prgInstall;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraGroupBox grpInstall;
        private Infragistics.Win.Misc.UltraLabel lblInstallStatus;
        private Infragistics.Win.Misc.UltraLabel lblDownloadStatus;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
    }
}
