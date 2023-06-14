namespace DWOS.Server.Admin.Wizards.InstallWizard
{
    partial class InstallSQL
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Database Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Data Folder", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallSQL));
            this.txtStatus = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.btnInstall = new Infragistics.Win.Misc.UltraButton();
            this.txtDBName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtFilePath = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.btnFolder = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDBName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFilePath)).BeginInit();
            this.SuspendLayout();
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(3, 88);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(467, 70);
            this.txtStatus.TabIndex = 0;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(3, 68);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(46, 15);
            this.ultraLabel3.TabIndex = 28;
            this.ultraLabel3.Text = "Status:";
            // 
            // btnInstall
            // 
            this.btnInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInstall.Location = new System.Drawing.Point(383, 164);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(87, 23);
            this.btnInstall.TabIndex = 29;
            this.btnInstall.Text = "Install";
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // txtDBName
            // 
            this.txtDBName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDBName.Location = new System.Drawing.Point(121, 3);
            this.txtDBName.Name = "txtDBName";
            this.txtDBName.Size = new System.Drawing.Size(349, 22);
            this.txtDBName.TabIndex = 31;
            ultraToolTipInfo2.ToolTipTextFormatted = "The name of the database in SQL Server. Default name is \'DWOS\'.";
            ultraToolTipInfo2.ToolTipTitle = "Database Name";
            this.tipManager.SetUltraToolTip(this.txtDBName, ultraToolTipInfo2);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(9, 7);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(100, 15);
            this.ultraLabel1.TabIndex = 30;
            this.ultraLabel1.Text = "Database Name:";
            // 
            // txtFilePath
            // 
            this.txtFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilePath.Location = new System.Drawing.Point(121, 30);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(315, 22);
            this.txtFilePath.TabIndex = 33;
            ultraToolTipInfo1.ToolTipTextFormatted = "The folder where the data files will be stored.";
            ultraToolTipInfo1.ToolTipTitle = "Data Folder";
            this.tipManager.SetUltraToolTip(this.txtFilePath, ultraToolTipInfo1);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(9, 34);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(75, 15);
            this.ultraLabel2.TabIndex = 32;
            this.ultraLabel2.Text = "Data Folder:";
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
            this.btnFolder.Location = new System.Drawing.Point(442, 28);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(26, 26);
            this.btnFolder.TabIndex = 34;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // InstallSQL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.txtDBName);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.btnInstall);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.txtStatus);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "InstallSQL";
            this.Size = new System.Drawing.Size(474, 199);
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDBName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFilePath)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtStatus;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraButton btnInstall;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDBName;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtFilePath;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        private Infragistics.Win.Misc.UltraButton btnFolder;
    }
}
