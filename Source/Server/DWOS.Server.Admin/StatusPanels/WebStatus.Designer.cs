namespace DWOS.Server.Admin.StatusPanels
{
    partial class WebStatus
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Refresh Portal Configuration", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebStatus));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Web Site", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Web Server", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Data Connection", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Indicates if the customer portal can be reached.", Infragistics.Win.ToolTipImage.Default, "Customer Portal Status", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraGroupBox4 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnRepair = new Infragistics.Win.Misc.UltraButton();
            this.ultraPictureBox3 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.txtWebsiteName = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.txtWebServerName = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtDBConnection = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel15 = new Infragistics.Win.Misc.UltraLabel();
            this.txtStatus = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel19 = new Infragistics.Win.Misc.UltraLabel();
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
            this.ultraGroupBox4.Controls.Add(this.btnRepair);
            this.ultraGroupBox4.Controls.Add(this.ultraPictureBox3);
            this.ultraGroupBox4.Controls.Add(this.txtWebsiteName);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel4);
            this.ultraGroupBox4.Controls.Add(this.txtWebServerName);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox4.Controls.Add(this.txtDBConnection);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel15);
            this.ultraGroupBox4.Controls.Add(this.txtStatus);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel19);
            this.ultraGroupBox4.Controls.Add(this.picActivate);
            this.ultraGroupBox4.Controls.Add(this.btnActivate);
            this.ultraGroupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox4.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox4.Name = "ultraGroupBox4";
            this.ultraGroupBox4.Size = new System.Drawing.Size(454, 142);
            this.ultraGroupBox4.TabIndex = 5;
            this.ultraGroupBox4.Text = "Customer Portal";
            // 
            // btnRepair
            // 
            this.btnRepair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRepair.Location = new System.Drawing.Point(6, 108);
            this.btnRepair.Name = "btnRepair";
            this.btnRepair.Size = new System.Drawing.Size(99, 28);
            this.btnRepair.TabIndex = 1;
            this.btnRepair.Text = "Refresh Config";
            ultraToolTipInfo1.ToolTipTextFormatted = "Refresh Portal settings to use the same database as the rest of DWOS.<br/>Try ref" +
    "reshing Portal settings if customers are having trouble accessing the Portal.";
            ultraToolTipInfo1.ToolTipTitle = "Refresh Portal Configuration";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnRepair, ultraToolTipInfo1);
            this.btnRepair.Click += new System.EventHandler(this.btnRepair_Click);
            // 
            // ultraPictureBox3
            // 
            this.ultraPictureBox3.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox3.Image = ((object)(resources.GetObject("ultraPictureBox3.Image")));
            this.ultraPictureBox3.Location = new System.Drawing.Point(6, 22);
            this.ultraPictureBox3.Name = "ultraPictureBox3";
            this.ultraPictureBox3.Size = new System.Drawing.Size(56, 47);
            this.ultraPictureBox3.TabIndex = 31;
            // 
            // txtWebsiteName
            // 
            this.txtWebsiteName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWebsiteName.Location = new System.Drawing.Point(193, 66);
            this.txtWebsiteName.Name = "txtWebsiteName";
            this.txtWebsiteName.Size = new System.Drawing.Size(255, 15);
            this.txtWebsiteName.TabIndex = 30;
            this.txtWebsiteName.Text = "------------";
            ultraToolTipInfo2.ToolTipTextFormatted = "The name of the web site/application located on the server.";
            ultraToolTipInfo2.ToolTipTitle = "Web Site";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtWebsiteName, ultraToolTipInfo2);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(67, 66);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(60, 15);
            this.ultraLabel4.TabIndex = 29;
            this.ultraLabel4.Text = "Web Site:";
            // 
            // txtWebServerName
            // 
            this.txtWebServerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWebServerName.Location = new System.Drawing.Point(193, 43);
            this.txtWebServerName.Name = "txtWebServerName";
            this.txtWebServerName.Size = new System.Drawing.Size(193, 15);
            this.txtWebServerName.TabIndex = 28;
            this.txtWebServerName.Text = "------------";
            ultraToolTipInfo3.ToolTipTextFormatted = "The name of the web server the customer portal is installed on.";
            ultraToolTipInfo3.ToolTipTitle = "Web Server";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtWebServerName, ultraToolTipInfo3);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(68, 43);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(76, 15);
            this.ultraLabel3.TabIndex = 27;
            this.ultraLabel3.Text = "Web Server:";
            // 
            // txtDBConnection
            // 
            this.txtDBConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDBConnection.Location = new System.Drawing.Point(193, 87);
            this.txtDBConnection.Name = "txtDBConnection";
            this.txtDBConnection.Size = new System.Drawing.Size(180, 15);
            this.txtDBConnection.TabIndex = 22;
            this.txtDBConnection.Text = "------------";
            ultraToolTipInfo4.ToolTipTextFormatted = "Determines if the database connection defined by the website is valid. If it is n" +
    "ot valid, click configure to update.";
            ultraToolTipInfo4.ToolTipTitle = "Data Connection";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtDBConnection, ultraToolTipInfo4);
            // 
            // ultraLabel15
            // 
            this.ultraLabel15.AutoSize = true;
            this.ultraLabel15.Location = new System.Drawing.Point(67, 87);
            this.ultraLabel15.Name = "ultraLabel15";
            this.ultraLabel15.Size = new System.Drawing.Size(103, 15);
            this.ultraLabel15.TabIndex = 21;
            this.ultraLabel15.Text = "Data Connection:";
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(193, 22);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(180, 15);
            this.txtStatus.TabIndex = 18;
            this.txtStatus.Text = "------------";
            ultraToolTipInfo5.ToolTipText = "Indicates if the customer portal can be reached.";
            ultraToolTipInfo5.ToolTipTitle = "Customer Portal Status";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtStatus, ultraToolTipInfo5);
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
            this.btnActivate.Location = new System.Drawing.Point(349, 108);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(99, 28);
            this.btnActivate.TabIndex = 2;
            this.btnActivate.Text = "Configure...";
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // WebStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox4);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "WebStatus";
            this.Size = new System.Drawing.Size(454, 142);
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
        private Infragistics.Win.Misc.UltraLabel txtDBConnection;
        private Infragistics.Win.Misc.UltraLabel ultraLabel15;
        private Infragistics.Win.Misc.UltraLabel txtStatus;
        private Infragistics.Win.Misc.UltraLabel ultraLabel19;
        private Infragistics.Win.Misc.UltraLabel txtWebServerName;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.Misc.UltraLabel txtWebsiteName;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox3;
        private Infragistics.Win.Misc.UltraButton btnRepair;
    }
}
