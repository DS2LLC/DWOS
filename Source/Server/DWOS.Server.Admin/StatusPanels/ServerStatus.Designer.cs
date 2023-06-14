namespace DWOS.Server.Admin.StatusPanels
{
    partial class ServerStatus
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
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("ViewIpAddresses");
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The IP address of this server.", Infragistics.Win.ToolTipImage.Default, "Server IP Address", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The machine name of this server.", Infragistics.Win.ToolTipImage.Default, "Server Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton2 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerStatus));
            this.ultraGroupBox4 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.txtServerIp = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtServerAddress = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.txtMinClientVersion = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtState = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtLatestVersion = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraPictureBox2 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.txtVersion = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel15 = new Infragistics.Win.Misc.UltraLabel();
            this.txtStatus = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel19 = new Infragistics.Win.Misc.UltraLabel();
            this.picActivate = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.btnActivate = new Infragistics.Win.Misc.UltraButton();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox4)).BeginInit();
            this.ultraGroupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtServerIp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServerAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMinClientVersion)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox4
            // 
            appearance4.FontData.BoldAsString = "True";
            appearance4.FontData.SizeInPoints = 9F;
            this.ultraGroupBox4.Appearance = appearance4;
            this.ultraGroupBox4.Controls.Add(this.ultraLabel5);
            this.ultraGroupBox4.Controls.Add(this.txtServerIp);
            this.ultraGroupBox4.Controls.Add(this.txtServerAddress);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel4);
            this.ultraGroupBox4.Controls.Add(this.txtMinClientVersion);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox4.Controls.Add(this.txtState);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox4.Controls.Add(this.txtLatestVersion);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox4.Controls.Add(this.ultraPictureBox2);
            this.ultraGroupBox4.Controls.Add(this.txtVersion);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel15);
            this.ultraGroupBox4.Controls.Add(this.txtStatus);
            this.ultraGroupBox4.Controls.Add(this.ultraLabel19);
            this.ultraGroupBox4.Controls.Add(this.picActivate);
            this.ultraGroupBox4.Controls.Add(this.btnActivate);
            this.ultraGroupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox4.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox4.Name = "ultraGroupBox4";
            this.ultraGroupBox4.Size = new System.Drawing.Size(454, 190);
            this.ultraGroupBox4.TabIndex = 5;
            this.ultraGroupBox4.Text = "Server";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(68, 162);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(63, 15);
            this.ultraLabel5.TabIndex = 34;
            this.ultraLabel5.Text = "Server IP:";
            // 
            // txtServerIp
            // 
            this.txtServerIp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            editorButton1.Key = "ViewIpAddresses";
            editorButton1.Text = "...";
            this.txtServerIp.ButtonsRight.Add(editorButton1);
            this.txtServerIp.Location = new System.Drawing.Point(194, 158);
            this.txtServerIp.Name = "txtServerIp";
            this.txtServerIp.ReadOnly = true;
            this.txtServerIp.Size = new System.Drawing.Size(150, 22);
            this.txtServerIp.TabIndex = 33;
            ultraToolTipInfo1.ToolTipText = "The IP address of this server.";
            ultraToolTipInfo1.ToolTipTitle = "Server IP Address";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtServerIp, ultraToolTipInfo1);
            this.txtServerIp.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtServerIp_EditorButtonClick);
            // 
            // txtServerAddress
            // 
            this.txtServerAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerAddress.Location = new System.Drawing.Point(193, 130);
            this.txtServerAddress.Name = "txtServerAddress";
            this.txtServerAddress.ReadOnly = true;
            this.txtServerAddress.Size = new System.Drawing.Size(151, 22);
            this.txtServerAddress.TabIndex = 32;
            ultraToolTipInfo2.ToolTipText = "The machine name of this server.";
            ultraToolTipInfo2.ToolTipTitle = "Server Name";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtServerAddress, ultraToolTipInfo2);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(68, 134);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(84, 15);
            this.ultraLabel4.TabIndex = 31;
            this.ultraLabel4.Text = "Server Name:";
            // 
            // txtMinClientVersion
            // 
            this.txtMinClientVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.Image = global::DWOS.Server.Admin.Properties.Resources.Save_16;
            editorButton2.Appearance = appearance2;
            editorButton2.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2013Button;
            this.txtMinClientVersion.ButtonsRight.Add(editorButton2);
            this.txtMinClientVersion.Location = new System.Drawing.Point(194, 102);
            this.txtMinClientVersion.Name = "txtMinClientVersion";
            this.txtMinClientVersion.Size = new System.Drawing.Size(150, 22);
            this.txtMinClientVersion.TabIndex = 30;
            this.txtMinClientVersion.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtMinClientVersion_EditorButtonClick);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(68, 106);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(116, 15);
            this.ultraLabel1.TabIndex = 29;
            this.ultraLabel1.Text = "Min. Client Version:";
            // 
            // txtState
            // 
            this.txtState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtState.Location = new System.Drawing.Point(193, 43);
            this.txtState.Name = "txtState";
            this.txtState.Size = new System.Drawing.Size(180, 15);
            this.txtState.TabIndex = 28;
            this.txtState.Text = "------------";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(68, 43);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(87, 15);
            this.ultraLabel3.TabIndex = 27;
            this.ultraLabel3.Text = "Server Status:";
            // 
            // txtLatestVersion
            // 
            this.txtLatestVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLatestVersion.Location = new System.Drawing.Point(194, 85);
            this.txtLatestVersion.Name = "txtLatestVersion";
            this.txtLatestVersion.Size = new System.Drawing.Size(150, 15);
            this.txtLatestVersion.TabIndex = 26;
            this.txtLatestVersion.Text = "------------";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(68, 85);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(127, 15);
            this.ultraLabel2.TabIndex = 25;
            this.ultraLabel2.Text = "Latest Client Version:";
            // 
            // ultraPictureBox2
            // 
            this.ultraPictureBox2.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox2.Image = ((object)(resources.GetObject("ultraPictureBox2.Image")));
            this.ultraPictureBox2.Location = new System.Drawing.Point(6, 22);
            this.ultraPictureBox2.Name = "ultraPictureBox2";
            this.ultraPictureBox2.Size = new System.Drawing.Size(56, 47);
            this.ultraPictureBox2.TabIndex = 23;
            // 
            // txtVersion
            // 
            this.txtVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVersion.Location = new System.Drawing.Point(194, 64);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.Size = new System.Drawing.Size(180, 15);
            this.txtVersion.TabIndex = 22;
            this.txtVersion.Text = "------------";
            // 
            // ultraLabel15
            // 
            this.ultraLabel15.AutoSize = true;
            this.ultraLabel15.Location = new System.Drawing.Point(68, 64);
            this.ultraLabel15.Name = "ultraLabel15";
            this.ultraLabel15.Size = new System.Drawing.Size(93, 15);
            this.ultraLabel15.TabIndex = 21;
            this.ultraLabel15.Text = "Server Version:";
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
            // 
            // ultraLabel19
            // 
            this.ultraLabel19.AutoSize = true;
            this.ultraLabel19.Location = new System.Drawing.Point(68, 22);
            this.ultraLabel19.Name = "ultraLabel19";
            this.ultraLabel19.Size = new System.Drawing.Size(43, 15);
            this.ultraLabel19.TabIndex = 17;
            this.ultraLabel19.Text = "Setup:";
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
            this.btnActivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActivate.Location = new System.Drawing.Point(357, 158);
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
            // timerUpdate
            // 
            this.timerUpdate.Interval = 5000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // ServerStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox4);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ServerStatus";
            this.Size = new System.Drawing.Size(454, 190);
            this.Load += new System.EventHandler(this.LicenseStatus_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox4)).EndInit();
            this.ultraGroupBox4.ResumeLayout(false);
            this.ultraGroupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtServerIp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServerAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMinClientVersion)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox4;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picActivate;
        private Infragistics.Win.Misc.UltraButton btnActivate;
        private Infragistics.Win.Misc.UltraLabel txtVersion;
        private Infragistics.Win.Misc.UltraLabel ultraLabel15;
        private Infragistics.Win.Misc.UltraLabel txtStatus;
        private Infragistics.Win.Misc.UltraLabel ultraLabel19;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox2;
        private Infragistics.Win.Misc.UltraLabel txtLatestVersion;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel txtState;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtMinClientVersion;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtServerIp;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtServerAddress;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private System.Windows.Forms.Timer timerUpdate;
    }
}
