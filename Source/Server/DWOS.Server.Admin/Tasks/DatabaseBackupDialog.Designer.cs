namespace DWOS.Server.Admin.Tasks
{
    partial class DatabaseBackupDialog
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Backup Folder", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Backup Type", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseBackupDialog));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click this button to pick the backup folder.", Infragistics.Win.ToolTipImage.Default, "Select Backup Folder", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Start the backup.", Infragistics.Win.ToolTipImage.Default, "Backup", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Shrink Transaction Log", Infragistics.Win.DefaultableBoolean.Default);
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.txtFilePath = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboBackupType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.btnFolder = new Infragistics.Win.Misc.UltraButton();
            this.btnBackup = new Infragistics.Win.Misc.UltraButton();
            this.grpBackup = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.prgBackup = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtStatus = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.chkShrinkTransactionLog = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            ((System.ComponentModel.ISupportInitialize)(this.txtFilePath)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBackupType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpBackup)).BeginInit();
            this.grpBackup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShrinkTransactionLog)).BeginInit();
            this.SuspendLayout();
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // txtFilePath
            // 
            this.txtFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilePath.AutoSize = false;
            this.txtFilePath.Location = new System.Drawing.Point(120, 27);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(247, 26);
            this.txtFilePath.TabIndex = 1;
            this.txtFilePath.Text = "C:\\Data";
            ultraToolTipInfo4.ToolTipTextFormatted = "The folder where the data files will be stored.";
            ultraToolTipInfo4.ToolTipTitle = "Backup Folder";
            this.tipManager.SetUltraToolTip(this.txtFilePath, ultraToolTipInfo4);
            // 
            // cboBackupType
            // 
            this.cboBackupType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem1.DataValue = "Full";
            valueListItem2.DataValue = "Transaction Log";
            this.cboBackupType.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.cboBackupType.Location = new System.Drawing.Point(120, 62);
            this.cboBackupType.Name = "cboBackupType";
            this.cboBackupType.Size = new System.Drawing.Size(279, 22);
            this.cboBackupType.TabIndex = 3;
            ultraToolTipInfo2.ToolTipTextFormatted = "The type of backup to run.<br/><b>Full</b> (default) - A full backup of the datab" +
    "ase.<br/><b>Transaction Log</b> - Performs a backup and truncation of the transa" +
    "ction log.";
            ultraToolTipInfo2.ToolTipTitle = "Backup Type";
            this.tipManager.SetUltraToolTip(this.cboBackupType, ultraToolTipInfo2);
            this.cboBackupType.SelectionChanged += new System.EventHandler(this.cboBackupType_SelectionChanged);
            // 
            // btnFolder
            // 
            this.btnFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = ((object)(resources.GetObject("appearance1.Image")));
            this.btnFolder.Appearance = appearance1;
            this.btnFolder.AutoSize = true;
            this.btnFolder.Location = new System.Drawing.Point(370, 27);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(26, 26);
            this.btnFolder.TabIndex = 2;
            ultraToolTipInfo3.ToolTipText = "Click this button to pick the backup folder.";
            ultraToolTipInfo3.ToolTipTitle = "Select Backup Folder";
            this.tipManager.SetUltraToolTip(this.btnFolder, ultraToolTipInfo3);
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // btnBackup
            // 
            this.btnBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBackup.Location = new System.Drawing.Point(312, 301);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(87, 23);
            this.btnBackup.TabIndex = 6;
            this.btnBackup.Text = "Backup";
            ultraToolTipInfo5.ToolTipText = "Start the backup.";
            ultraToolTipInfo5.ToolTipTitle = "Backup";
            this.tipManager.SetUltraToolTip(this.btnBackup, ultraToolTipInfo5);
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // grpBackup
            // 
            this.grpBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBackup.Controls.Add(this.chkShrinkTransactionLog);
            this.grpBackup.Controls.Add(this.cboBackupType);
            this.grpBackup.Controls.Add(this.ultraLabel1);
            this.grpBackup.Controls.Add(this.prgBackup);
            this.grpBackup.Controls.Add(this.btnFolder);
            this.grpBackup.Controls.Add(this.txtFilePath);
            this.grpBackup.Controls.Add(this.ultraLabel2);
            this.grpBackup.Controls.Add(this.btnBackup);
            this.grpBackup.Controls.Add(this.ultraLabel3);
            this.grpBackup.Controls.Add(this.txtStatus);
            this.grpBackup.Location = new System.Drawing.Point(12, 12);
            this.grpBackup.Name = "grpBackup";
            this.grpBackup.Size = new System.Drawing.Size(405, 330);
            this.grpBackup.TabIndex = 0;
            this.grpBackup.Text = "Backup";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(4, 66);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(82, 15);
            this.ultraLabel1.TabIndex = 46;
            this.ultraLabel1.Text = "Backup Type:";
            // 
            // prgBackup
            // 
            this.prgBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgBackup.Location = new System.Drawing.Point(4, 301);
            this.prgBackup.Name = "prgBackup";
            this.prgBackup.Size = new System.Drawing.Size(157, 23);
            this.prgBackup.TabIndex = 45;
            this.prgBackup.Text = "[Formatted]";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(4, 31);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(90, 15);
            this.ultraLabel2.TabIndex = 42;
            this.ultraLabel2.Text = "Backup Folder:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(6, 124);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(46, 15);
            this.ultraLabel3.TabIndex = 40;
            this.ultraLabel3.Text = "Status:";
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(4, 145);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(395, 150);
            this.txtStatus.TabIndex = 5;
            // 
            // chkShrinkTransactionLog
            // 
            this.chkShrinkTransactionLog.AutoSize = true;
            this.chkShrinkTransactionLog.Enabled = false;
            this.chkShrinkTransactionLog.Location = new System.Drawing.Point(4, 90);
            this.chkShrinkTransactionLog.Name = "chkShrinkTransactionLog";
            this.chkShrinkTransactionLog.Size = new System.Drawing.Size(146, 18);
            this.chkShrinkTransactionLog.TabIndex = 4;
            this.chkShrinkTransactionLog.Text = "Shrink transaction log";
            ultraToolTipInfo1.ToolTipTextFormatted = "Check this option to shrink the size of the transaction log file.<br/><br/>This m" +
    "ay cause performance issues in DWOS as the log file needs to grow after shrinkin" +
    "g it.";
            ultraToolTipInfo1.ToolTipTitle = "Shrink Transaction Log";
            this.tipManager.SetUltraToolTip(this.chkShrinkTransactionLog, ultraToolTipInfo1);
            // 
            // DatabaseBackupDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 354);
            this.Controls.Add(this.grpBackup);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DatabaseBackupDialog";
            this.Text = "Database Backup";
            ((System.ComponentModel.ISupportInitialize)(this.txtFilePath)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBackupType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpBackup)).EndInit();
            this.grpBackup.ResumeLayout(false);
            this.grpBackup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShrinkTransactionLog)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        private Infragistics.Win.Misc.UltraGroupBox grpBackup;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar prgBackup;
        private Infragistics.Win.Misc.UltraButton btnFolder;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtFilePath;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraButton btnBackup;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtStatus;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboBackupType;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkShrinkTransactionLog;
    }
}