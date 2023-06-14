namespace DWOS.Server.Admin.StatusPanels
{
    partial class DatabaseStatus
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The version of the database schema used by the application.", Infragistics.Win.ToolTipImage.Default, "Schema Version", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseStatus));
            this.ultraGroupBox4 = new Infragistics.Win.Misc.UltraGroupBox();
            this.lblSchemaVersion = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.txtDatabase = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.txtServerName = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtSize = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtVersion = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel15 = new Infragistics.Win.Misc.UltraLabel();
            this.txtDatabaseEdition = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel17 = new Infragistics.Win.Misc.UltraLabel();
            this.txtStatus = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel19 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.picActivate = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.btnActivate = new Infragistics.Win.Misc.UltraButton();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox4)).BeginInit();
            this.ultraGroupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraGroupBox4
            // 
            appearance1.FontData.BoldAsString = "True";
            appearance1.FontData.SizeInPoints = 9F;
            this.ultraGroupBox4.Appearance = appearance1;
            this.ultraGroupBox4.Controls.Add(this.lblSchemaVersion);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel5);
            this.ultraGroupBox4.Controls.Add(this.txtDatabase);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel4);
            this.ultraGroupBox4.Controls.Add(this.txtServerName);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox4.Controls.Add(this.txtSize);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox4.Controls.Add(this.txtVersion);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel15);
            this.ultraGroupBox4.Controls.Add(this.txtDatabaseEdition);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel17);
            this.ultraGroupBox4.Controls.Add(this.txtStatus);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel19);
            this.ultraGroupBox4.Controls.Add(this.ultraPictureBox1);
            this.ultraGroupBox4.Controls.Add(this.picActivate);
            this.ultraGroupBox4.Controls.Add(this.btnActivate);
            this.ultraGroupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox4.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox4.Name = "ultraGroupBox4";
            this.ultraGroupBox4.Size = new System.Drawing.Size(454, 184);
            this.ultraGroupBox4.TabIndex = 5;
            this.ultraGroupBox4.Text = "Database";
            // 
            // lblSchemaVersion
            // 
            this.lblSchemaVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSchemaVersion.Location = new System.Drawing.Point(180, 148);
            this.lblSchemaVersion.Name = "lblSchemaVersion";
            this.lblSchemaVersion.Size = new System.Drawing.Size(163, 15);
            this.lblSchemaVersion.TabIndex = 30;
            this.lblSchemaVersion.Text = "------------";
            ultraToolTipInfo1.ToolTipText = "The version of the database schema used by the application.";
            ultraToolTipInfo1.ToolTipTitle = "Schema Version";
            this.ultraToolTipManager1.SetUltraToolTip(this.lblSchemaVersion, ultraToolTipInfo1);
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(68, 148);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(101, 15);
            this.ultraLabel5.TabIndex = 29;
            this.ultraLabel5.Text = "Schema Version:";
            // 
            // txtDatabase
            // 
            this.txtDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDatabase.Location = new System.Drawing.Point(179, 64);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(216, 15);
            this.txtDatabase.TabIndex = 28;
            this.txtDatabase.Text = "------------";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(68, 64);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(63, 15);
            this.ultraLabel4.TabIndex = 27;
            this.ultraLabel4.Text = "DB Name:";
            // 
            // txtServerName
            // 
            this.txtServerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerName.Location = new System.Drawing.Point(179, 43);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(216, 15);
            this.txtServerName.TabIndex = 26;
            this.txtServerName.Text = "------------";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(68, 43);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(47, 15);
            this.ultraLabel3.TabIndex = 25;
            this.ultraLabel3.Text = "Server:";
            // 
            // txtSize
            // 
            this.txtSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSize.Location = new System.Drawing.Point(180, 127);
            this.txtSize.Name = "txtSize";
            this.txtSize.Size = new System.Drawing.Size(163, 15);
            this.txtSize.TabIndex = 24;
            this.txtSize.Text = "------------";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(68, 127);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(64, 15);
            this.ultraLabel2.TabIndex = 23;
            this.ultraLabel2.Text = "Data Size:";
            // 
            // txtVersion
            // 
            this.txtVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVersion.Location = new System.Drawing.Point(179, 106);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.Size = new System.Drawing.Size(150, 15);
            this.txtVersion.TabIndex = 22;
            this.txtVersion.Text = "------------";
            // 
            // ultraLabel15
            // 
            this.ultraLabel15.AutoSize = true;
            this.ultraLabel15.Location = new System.Drawing.Point(68, 106);
            this.ultraLabel15.Name = "ultraLabel15";
            this.ultraLabel15.Size = new System.Drawing.Size(72, 15);
            this.ultraLabel15.TabIndex = 21;
            this.ultraLabel15.Text = "DB Version:";
            // 
            // txtDatabaseEdition
            // 
            this.txtDatabaseEdition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDatabaseEdition.Location = new System.Drawing.Point(179, 85);
            this.txtDatabaseEdition.Name = "txtDatabaseEdition";
            this.txtDatabaseEdition.Size = new System.Drawing.Size(216, 15);
            this.txtDatabaseEdition.TabIndex = 20;
            this.txtDatabaseEdition.Text = "------------";
            // 
            // ultraLabel17
            // 
            this.ultraLabel17.AutoSize = true;
            this.ultraLabel17.Location = new System.Drawing.Point(68, 85);
            this.ultraLabel17.Name = "ultraLabel17";
            this.ultraLabel17.Size = new System.Drawing.Size(69, 15);
            this.ultraLabel17.TabIndex = 19;
            this.ultraLabel17.Text = "DB Edition:";
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(179, 22);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(150, 15);
            this.txtStatus.TabIndex = 18;
            this.txtStatus.Text = "------------";
            // 
            // ultraLabel19
            // 
            this.ultraLabel19.AutoSize = true;
            this.ultraLabel19.Location = new System.Drawing.Point(68, 22);
            this.ultraLabel19.Name = "ultraLabel19";
            this.ultraLabel19.Size = new System.Drawing.Size(46, 15);
            this.ultraLabel19.TabIndex = 17;
            this.ultraLabel19.Text = "Status:";
            // 
            // ultraPictureBox1
            // 
            this.ultraPictureBox1.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox1.Image = ((object)(resources.GetObject("ultraPictureBox1.Image")));
            this.ultraPictureBox1.Location = new System.Drawing.Point(6, 18);
            this.ultraPictureBox1.Name = "ultraPictureBox1";
            this.ultraPictureBox1.Size = new System.Drawing.Size(56, 47);
            this.ultraPictureBox1.TabIndex = 16;
            // 
            // picActivate
            // 
            this.picActivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picActivate.BorderShadowColor = System.Drawing.Color.Empty;
            this.picActivate.Image = ((object)(resources.GetObject("picActivate.Image")));
            this.picActivate.Location = new System.Drawing.Point(392, 18);
            this.picActivate.Name = "picActivate";
            this.picActivate.Size = new System.Drawing.Size(56, 47);
            this.picActivate.TabIndex = 3;
            // 
            // btnActivate
            // 
            this.btnActivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActivate.Location = new System.Drawing.Point(360, 150);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(88, 28);
            this.btnActivate.TabIndex = 0;
            this.btnActivate.Text = "Configure...";
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // DatabaseStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox4);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DatabaseStatus";
            this.Size = new System.Drawing.Size(454, 184);
            this.Load += new System.EventHandler(this.LicenseStatus_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox4)).EndInit();
            this.ultraGroupBox4.ResumeLayout(false);
            this.ultraGroupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox4;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picActivate;
        private Infragistics.Win.Misc.UltraButton btnActivate;
        private Infragistics.Win.Misc.UltraLabel txtVersion;
        private Infragistics.Win.Misc.UltraLabel ultraLabel15;
        private Infragistics.Win.Misc.UltraLabel txtDatabaseEdition;
        private Infragistics.Win.Misc.UltraLabel ultraLabel17;
        private Infragistics.Win.Misc.UltraLabel txtStatus;
        private Infragistics.Win.Misc.UltraLabel ultraLabel19;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
        private Infragistics.Win.Misc.UltraLabel txtSize;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel txtServerName;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel txtDatabase;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel lblSchemaVersion;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;

    }
}
