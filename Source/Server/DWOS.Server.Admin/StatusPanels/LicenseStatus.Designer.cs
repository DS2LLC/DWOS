namespace DWOS.Server.Admin.StatusPanels
{
    partial class LicenseStatus
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The number of licenses in use.", Infragistics.Win.ToolTipImage.Default, "License Usage", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the license will expire and client connections will be refused.", Infragistics.Win.ToolTipImage.Default, "License Expiration", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Number of licenses registered to this customer key.", Infragistics.Win.ToolTipImage.Default, "License Count", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseStatus));
            this.ultraGroupBox4 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnRefresh = new Infragistics.Win.Misc.UltraButton();
            this.lblLicenseUsage = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.txtLicenseExpiration = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel23 = new Infragistics.Win.Misc.UltraLabel();
            this.txtLicenseCount = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel21 = new Infragistics.Win.Misc.UltraLabel();
            this.txtLicenseKey = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel13 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCustomerID = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel11 = new Infragistics.Win.Misc.UltraLabel();
            this.txtLicenseStatus = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.picActivate = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraPictureBox4 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
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
            this.ultraGroupBox4.Controls.Add(this.btnRefresh);
            this.ultraGroupBox4.Controls.Add(this.lblLicenseUsage);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel4);
            this.ultraGroupBox4.Controls.Add(this.txtLicenseExpiration);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel23);
            this.ultraGroupBox4.Controls.Add(this.txtLicenseCount);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel21);
            this.ultraGroupBox4.Controls.Add(this.txtLicenseKey);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel13);
            this.ultraGroupBox4.Controls.Add(this.txtCustomerID);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel11);
            this.ultraGroupBox4.Controls.Add(this.txtLicenseStatus);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel8);
            this.ultraGroupBox4.Controls.Add(this.picActivate);
            this.ultraGroupBox4.Controls.Add(this.ultraPictureBox4);
            this.ultraGroupBox4.Controls.Add(this.btnActivate);
            this.ultraGroupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox4.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox4.Name = "ultraGroupBox4";
            this.ultraGroupBox4.Size = new System.Drawing.Size(454, 153);
            this.ultraGroupBox4.TabIndex = 5;
            this.ultraGroupBox4.Text = "License Activation";
            // 
            // btnRefresh
            // 
            appearance2.Image = global::DWOS.Server.Admin.Properties.Resources.Refresh;
            this.btnRefresh.Appearance = appearance2;
            this.btnRefresh.AutoSize = true;
            this.btnRefresh.Location = new System.Drawing.Point(9, 116);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(26, 26);
            this.btnRefresh.TabIndex = 34;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblLicenseUsage
            // 
            this.lblLicenseUsage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLicenseUsage.Location = new System.Drawing.Point(167, 127);
            this.lblLicenseUsage.Name = "lblLicenseUsage";
            this.lblLicenseUsage.Size = new System.Drawing.Size(177, 15);
            this.lblLicenseUsage.TabIndex = 33;
            this.lblLicenseUsage.Text = "------------";
            ultraToolTipInfo1.ToolTipText = "The number of licenses in use.";
            ultraToolTipInfo1.ToolTipTitle = "License Usage";
            this.ultraToolTipManager1.SetUltraToolTip(this.lblLicenseUsage, ultraToolTipInfo1);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(68, 127);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(90, 15);
            this.ultraLabel4.TabIndex = 32;
            this.ultraLabel4.Text = "License Usage:";
            // 
            // txtLicenseExpiration
            // 
            this.txtLicenseExpiration.Location = new System.Drawing.Point(167, 106);
            this.txtLicenseExpiration.Name = "txtLicenseExpiration";
            this.txtLicenseExpiration.Size = new System.Drawing.Size(92, 15);
            this.txtLicenseExpiration.TabIndex = 13;
            this.txtLicenseExpiration.Text = "------------";
            ultraToolTipInfo2.ToolTipText = "The date the license will expire and client connections will be refused.";
            ultraToolTipInfo2.ToolTipTitle = "License Expiration";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtLicenseExpiration, ultraToolTipInfo2);
            // 
            // ultraLabel23
            // 
            this.ultraLabel23.AutoSize = true;
            this.ultraLabel23.Location = new System.Drawing.Point(68, 106);
            this.ultraLabel23.Name = "ultraLabel23";
            this.ultraLabel23.Size = new System.Drawing.Size(67, 15);
            this.ultraLabel23.TabIndex = 12;
            this.ultraLabel23.Text = "Expiration:";
            // 
            // txtLicenseCount
            // 
            this.txtLicenseCount.Location = new System.Drawing.Point(167, 85);
            this.txtLicenseCount.Name = "txtLicenseCount";
            this.txtLicenseCount.Size = new System.Drawing.Size(93, 15);
            this.txtLicenseCount.TabIndex = 11;
            this.txtLicenseCount.Text = "------------";
            ultraToolTipInfo3.ToolTipText = "Number of licenses registered to this customer key.";
            ultraToolTipInfo3.ToolTipTitle = "License Count";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtLicenseCount, ultraToolTipInfo3);
            // 
            // ultraLabel21
            // 
            this.ultraLabel21.AutoSize = true;
            this.ultraLabel21.Location = new System.Drawing.Point(67, 85);
            this.ultraLabel21.Name = "ultraLabel21";
            this.ultraLabel21.Size = new System.Drawing.Size(89, 15);
            this.ultraLabel21.TabIndex = 10;
            this.ultraLabel21.Text = "License Count:";
            // 
            // txtLicenseKey
            // 
            this.txtLicenseKey.Location = new System.Drawing.Point(167, 64);
            this.txtLicenseKey.Name = "txtLicenseKey";
            this.txtLicenseKey.Size = new System.Drawing.Size(147, 15);
            this.txtLicenseKey.TabIndex = 9;
            this.txtLicenseKey.Text = "------------";
            // 
            // ultraLabel13
            // 
            this.ultraLabel13.AutoSize = true;
            this.ultraLabel13.Location = new System.Drawing.Point(67, 64);
            this.ultraLabel13.Name = "ultraLabel13";
            this.ultraLabel13.Size = new System.Drawing.Size(76, 15);
            this.ultraLabel13.TabIndex = 8;
            this.ultraLabel13.Text = "License Key:";
            // 
            // txtCustomerID
            // 
            this.txtCustomerID.Location = new System.Drawing.Point(167, 43);
            this.txtCustomerID.Name = "txtCustomerID";
            this.txtCustomerID.Size = new System.Drawing.Size(147, 15);
            this.txtCustomerID.TabIndex = 7;
            this.txtCustomerID.Text = "------------";
            // 
            // ultraLabel11
            // 
            this.ultraLabel11.AutoSize = true;
            this.ultraLabel11.Location = new System.Drawing.Point(68, 43);
            this.ultraLabel11.Name = "ultraLabel11";
            this.ultraLabel11.Size = new System.Drawing.Size(80, 15);
            this.ultraLabel11.TabIndex = 6;
            this.ultraLabel11.Text = "Customer Id:";
            // 
            // txtLicenseStatus
            // 
            this.txtLicenseStatus.Location = new System.Drawing.Point(167, 22);
            this.txtLicenseStatus.Name = "txtLicenseStatus";
            this.txtLicenseStatus.Size = new System.Drawing.Size(147, 15);
            this.txtLicenseStatus.TabIndex = 5;
            this.txtLicenseStatus.Text = "------------";
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(68, 22);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(46, 15);
            this.ultraLabel8.TabIndex = 4;
            this.ultraLabel8.Text = "Status:";
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
            // ultraPictureBox4
            // 
            this.ultraPictureBox4.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox4.Image = ((object)(resources.GetObject("ultraPictureBox4.Image")));
            this.ultraPictureBox4.Location = new System.Drawing.Point(6, 18);
            this.ultraPictureBox4.Name = "ultraPictureBox4";
            this.ultraPictureBox4.Size = new System.Drawing.Size(56, 47);
            this.ultraPictureBox4.TabIndex = 2;
            // 
            // btnActivate
            // 
            this.btnActivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActivate.Location = new System.Drawing.Point(360, 119);
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
            // LicenseStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox4);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "LicenseStatus";
            this.Size = new System.Drawing.Size(454, 153);
            this.Load += new System.EventHandler(this.LicenseStatus_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox4)).EndInit();
            this.ultraGroupBox4.ResumeLayout(false);
            this.ultraGroupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox4;
        private Infragistics.Win.Misc.UltraLabel txtLicenseExpiration;
        private Infragistics.Win.Misc.UltraLabel ultraLabel23;
        private Infragistics.Win.Misc.UltraLabel txtLicenseCount;
        private Infragistics.Win.Misc.UltraLabel ultraLabel21;
        private Infragistics.Win.Misc.UltraLabel txtLicenseKey;
        private Infragistics.Win.Misc.UltraLabel ultraLabel13;
        private Infragistics.Win.Misc.UltraLabel txtCustomerID;
        private Infragistics.Win.Misc.UltraLabel ultraLabel11;
        private Infragistics.Win.Misc.UltraLabel txtLicenseStatus;
        private Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picActivate;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox4;
        private Infragistics.Win.Misc.UltraButton btnActivate;
        private Infragistics.Win.Misc.UltraButton btnRefresh;
        private Infragistics.Win.Misc.UltraLabel lblLicenseUsage;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;

    }
}
