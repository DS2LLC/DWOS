namespace DWOS.UI.Admin.SettingsPanels
{
	partial class SettingsEmailInfo
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
			if(disposing && (components != null))
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The password used for email authentication.", Infragistics.Win.ToolTipImage.Default, "Password", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The user name associated with the email client.", Infragistics.Win.ToolTipImage.Default, "User Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The port used by the server for sending emails.", Infragistics.Win.ToolTipImage.Default, "Server Port", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The SSL start mode currently in use. The default mode is \'Automatic\'.", Infragistics.Win.ToolTipImage.Default, "SSL Start Mode", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The email authentication method currently in use.", Infragistics.Win.ToolTipImage.Default, "Authentication", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The email server (SMTP) address.", Infragistics.Win.ToolTipImage.Default, "Server Address", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The address to send emails from.", Infragistics.Win.ToolTipImage.Default, "From Address", Infragistics.Win.DefaultableBoolean.Default);
            this.txtPassword = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.txtUserName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.txtPort = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.cboSslStartMode = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboAuthType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtOutput = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnTest = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.txtServerAddress = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtFromAddress = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.htmlmailer1 = new nsoftware.IPWorks.Htmlmailer(this.components);
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboSslStartMode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAuthType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOutput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServerAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFromAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(118, 139);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtPassword.MaxLength = 255;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.NullText = "Password";
            appearance1.FontData.ItalicAsString = "True";
            appearance1.ForeColor = System.Drawing.Color.Silver;
            this.txtPassword.NullTextAppearance = appearance1;
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(335, 22);
            this.txtPassword.TabIndex = 55;
            ultraToolTipInfo7.ToolTipText = "The password used for email authentication.";
            ultraToolTipInfo7.ToolTipTitle = "Password";
            this.ultraToolTipManager.SetUltraToolTip(this.txtPassword, ultraToolTipInfo7);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(14, 143);
            this.ultraLabel6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(63, 15);
            this.ultraLabel6.TabIndex = 56;
            this.ultraLabel6.Text = "Password:";
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.Location = new System.Drawing.Point(118, 111);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtUserName.MaxLength = 255;
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.NullText = "Login Name";
            appearance2.FontData.ItalicAsString = "True";
            appearance2.ForeColor = System.Drawing.Color.Silver;
            this.txtUserName.NullTextAppearance = appearance2;
            this.txtUserName.Size = new System.Drawing.Size(335, 22);
            this.txtUserName.TabIndex = 53;
            ultraToolTipInfo3.ToolTipText = "The user name associated with the email client.";
            ultraToolTipInfo3.ToolTipTitle = "User Name";
            this.ultraToolTipManager.SetUltraToolTip(this.txtUserName, ultraToolTipInfo3);
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(14, 115);
            this.ultraLabel5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(72, 15);
            this.ultraLabel5.TabIndex = 54;
            this.ultraLabel5.Text = "User Name:";
            // 
            // txtPort
            // 
            this.txtPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPort.Location = new System.Drawing.Point(118, 83);
            this.txtPort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtPort.MaxLength = 6;
            this.txtPort.Name = "txtPort";
            this.txtPort.NullText = "Port";
            appearance3.FontData.ItalicAsString = "True";
            appearance3.ForeColor = System.Drawing.Color.Silver;
            this.txtPort.NullTextAppearance = appearance3;
            this.txtPort.Size = new System.Drawing.Size(335, 22);
            this.txtPort.TabIndex = 51;
            ultraToolTipInfo4.ToolTipText = "The port used by the server for sending emails.";
            ultraToolTipInfo4.ToolTipTitle = "Server Port";
            this.ultraToolTipManager.SetUltraToolTip(this.txtPort, ultraToolTipInfo4);
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.ultraLabel8);
            this.ultraGroupBox1.Controls.Add(this.cboSslStartMode);
            this.ultraGroupBox1.Controls.Add(this.cboAuthType);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel7);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Controls.Add(this.txtOutput);
            this.ultraGroupBox1.Controls.Add(this.btnTest);
            this.ultraGroupBox1.Controls.Add(this.txtPassword);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel6);
            this.ultraGroupBox1.Controls.Add(this.txtUserName);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel5);
            this.ultraGroupBox1.Controls.Add(this.txtPort);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel4);
            this.ultraGroupBox1.Controls.Add(this.txtServerAddress);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox1.Controls.Add(this.txtFromAddress);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(460, 400);
            this.ultraGroupBox1.TabIndex = 39;
            this.ultraGroupBox1.Text = "Email Information";
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(14, 199);
            this.ultraLabel8.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(98, 15);
            this.ultraLabel8.TabIndex = 65;
            this.ultraLabel8.Text = "SSL Start Mode:";
            // 
            // cboSslStartMode
            // 
            this.cboSslStartMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSslStartMode.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboSslStartMode.Location = new System.Drawing.Point(119, 195);
            this.cboSslStartMode.Name = "cboSslStartMode";
            this.cboSslStartMode.Size = new System.Drawing.Size(334, 22);
            this.cboSslStartMode.TabIndex = 64;
            ultraToolTipInfo1.ToolTipText = "The SSL start mode currently in use. The default mode is \'Automatic\'.";
            ultraToolTipInfo1.ToolTipTitle = "SSL Start Mode";
            this.ultraToolTipManager.SetUltraToolTip(this.cboSslStartMode, ultraToolTipInfo1);
            // 
            // cboAuthType
            // 
            this.cboAuthType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboAuthType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboAuthType.Location = new System.Drawing.Point(118, 167);
            this.cboAuthType.Name = "cboAuthType";
            this.cboAuthType.Size = new System.Drawing.Size(335, 22);
            this.cboAuthType.TabIndex = 63;
            ultraToolTipInfo2.ToolTipText = "The email authentication method currently in use.";
            ultraToolTipInfo2.ToolTipTitle = "Authentication";
            this.ultraToolTipManager.SetUltraToolTip(this.cboAuthType, ultraToolTipInfo2);
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(14, 171);
            this.ultraLabel7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(92, 15);
            this.ultraLabel7.TabIndex = 62;
            this.ultraLabel7.Text = "Authentication:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(14, 220);
            this.ultraLabel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(94, 15);
            this.ultraLabel2.TabIndex = 60;
            this.ultraLabel2.Text = "Testing Output:";
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Location = new System.Drawing.Point(14, 241);
            this.txtOutput.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.Scrollbars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(439, 124);
            this.txtOutput.TabIndex = 65;
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Location = new System.Drawing.Point(368, 371);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(85, 23);
            this.btnTest.TabIndex = 66;
            this.btnTest.Text = "Begin Test";
            this.btnTest.Click += new System.EventHandler(this.BtnTest_Click);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(14, 87);
            this.ultraLabel4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(74, 15);
            this.ultraLabel4.TabIndex = 52;
            this.ultraLabel4.Text = "Server Port:";
            // 
            // txtServerAddress
            // 
            this.txtServerAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerAddress.Location = new System.Drawing.Point(118, 55);
            this.txtServerAddress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtServerAddress.MaxLength = 255;
            this.txtServerAddress.Name = "txtServerAddress";
            this.txtServerAddress.NullText = "SMTP Server Name";
            appearance4.FontData.ItalicAsString = "True";
            appearance4.ForeColor = System.Drawing.Color.Silver;
            this.txtServerAddress.NullTextAppearance = appearance4;
            this.txtServerAddress.Size = new System.Drawing.Size(335, 22);
            this.txtServerAddress.TabIndex = 49;
            ultraToolTipInfo5.ToolTipText = "The email server (SMTP) address.";
            ultraToolTipInfo5.ToolTipTitle = "Server Address";
            this.ultraToolTipManager.SetUltraToolTip(this.txtServerAddress, ultraToolTipInfo5);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(14, 59);
            this.ultraLabel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(96, 15);
            this.ultraLabel3.TabIndex = 50;
            this.ultraLabel3.Text = "Server Address:";
            // 
            // txtFromAddress
            // 
            this.txtFromAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFromAddress.Location = new System.Drawing.Point(118, 27);
            this.txtFromAddress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtFromAddress.Name = "txtFromAddress";
            this.txtFromAddress.NullText = "From Email Address";
            appearance5.FontData.ItalicAsString = "True";
            appearance5.ForeColor = System.Drawing.Color.Silver;
            this.txtFromAddress.NullTextAppearance = appearance5;
            this.txtFromAddress.Size = new System.Drawing.Size(335, 22);
            this.txtFromAddress.TabIndex = 47;
            ultraToolTipInfo6.ToolTipText = "The address to send emails from.";
            ultraToolTipInfo6.ToolTipTitle = "From Address";
            this.ultraToolTipManager.SetUltraToolTip(this.txtFromAddress, ultraToolTipInfo6);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(14, 31);
            this.ultraLabel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(88, 15);
            this.ultraLabel1.TabIndex = 48;
            this.ultraLabel1.Text = "From Address:";
            // 
            // htmlmailer1
            // 
            this.htmlmailer1.About = "IP*Works! 2016 [Build 6723]";
            this.htmlmailer1.InvokeThrough = this;
            this.htmlmailer1.OnPITrail += new nsoftware.IPWorks.Htmlmailer.OnPITrailHandler(this.Htmlmailer1_OnPITrail);
            this.htmlmailer1.OnSSLStatus += new nsoftware.IPWorks.Htmlmailer.OnSSLStatusHandler(this.htmlmailer1_OnSSLStatus);
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // SettingsEmailInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(460, 400);
            this.Name = "SettingsEmailInfo";
            this.Size = new System.Drawing.Size(460, 400);
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboSslStartMode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAuthType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOutput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServerAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFromAddress)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPassword;
		public Infragistics.Win.Misc.UltraLabel ultraLabel6;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtUserName;
		public Infragistics.Win.Misc.UltraLabel ultraLabel5;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPort;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
		public Infragistics.Win.Misc.UltraLabel ultraLabel4;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtServerAddress;
		public Infragistics.Win.Misc.UltraLabel ultraLabel3;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtFromAddress;
		public Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.Misc.UltraButton btnTest;
		private nsoftware.IPWorks.Htmlmailer htmlmailer1;
		public Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtOutput;
        public Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboAuthType;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
        public Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboSslStartMode;
    }
}
