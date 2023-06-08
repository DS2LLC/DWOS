namespace DWOS.UI
{
	partial class UserSettingsView
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
			_displayDisabledTooltips.Dispose();
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Reset Layouts", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The server that DWOS points to.", Infragistics.Win.ToolTipImage.Default, "DWOS Server", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("Delete");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Printer", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Touch Enabled", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Enable Animations", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Highlight Full Row", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.chkResetLayout = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.txtDatabaseName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.cboPrinter = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.chkTouchEnabled = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkAnimations = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkFullRowHighlight = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.btnChangeServer = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.chkResetLayout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDatabaseName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPrinter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkTouchEnabled)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAnimations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFullRowHighlight)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(15, 16);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(88, 15);
            this.ultraLabel2.TabIndex = 33;
            this.ultraLabel2.Text = "DWOS Server:";
            // 
            // chkResetLayout
            // 
            this.chkResetLayout.Location = new System.Drawing.Point(150, 68);
            this.chkResetLayout.Name = "chkResetLayout";
            this.chkResetLayout.Size = new System.Drawing.Size(310, 20);
            this.chkResetLayout.TabIndex = 32;
            this.chkResetLayout.Text = "Reset Layouts to Default";
            ultraToolTipInfo6.ToolTipTextFormatted = "Resets the layout customizations back to their defaults.<br/><br/><span style=\"co" +
    "lor:Red; font-weight:bold;\">NOTE: Requires you to restart the application.</span" +
    "><br/>";
            ultraToolTipInfo6.ToolTipTitle = "Reset Layouts";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkResetLayout, ultraToolTipInfo6);
            // 
            // txtDatabaseName
            // 
            this.txtDatabaseName.Location = new System.Drawing.Point(150, 12);
            this.txtDatabaseName.Name = "txtDatabaseName";
            this.txtDatabaseName.ReadOnly = true;
            this.txtDatabaseName.Size = new System.Drawing.Size(310, 22);
            this.txtDatabaseName.TabIndex = 34;
            ultraToolTipInfo5.ToolTipText = "The server that DWOS points to.";
            ultraToolTipInfo5.ToolTipTitle = "DWOS Server";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtDatabaseName, ultraToolTipInfo5);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // cboPrinter
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            editorButton1.Appearance = appearance1;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton1.Key = "Delete";
            this.cboPrinter.ButtonsRight.Add(editorButton1);
            this.cboPrinter.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboPrinter.Location = new System.Drawing.Point(150, 40);
            this.cboPrinter.Name = "cboPrinter";
            this.cboPrinter.NullText = "<None>";
            this.cboPrinter.Size = new System.Drawing.Size(310, 22);
            this.cboPrinter.TabIndex = 35;
            ultraToolTipInfo4.ToolTipTextFormatted = "This is the printer used for printing standard paper documents. If no printer is " +
    "selected then the default printer will be used.";
            ultraToolTipInfo4.ToolTipTitle = "Printer";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboPrinter, ultraToolTipInfo4);
            this.cboPrinter.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.cboPrinter_EditorButtonClick);
            // 
            // chkTouchEnabled
            // 
            this.chkTouchEnabled.Checked = global::DWOS.UI.Properties.Settings.Default.TouchEnabled;
            this.chkTouchEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTouchEnabled.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::DWOS.UI.Properties.Settings.Default, "TouchEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkTouchEnabled.Location = new System.Drawing.Point(150, 94);
            this.chkTouchEnabled.Name = "chkTouchEnabled";
            this.chkTouchEnabled.Size = new System.Drawing.Size(310, 20);
            this.chkTouchEnabled.TabIndex = 38;
            this.chkTouchEnabled.Text = "Touch Enabled";
            ultraToolTipInfo3.ToolTipTextFormatted = "If checked, touch enabled mode will be on.";
            ultraToolTipInfo3.ToolTipTitle = "Touch Enabled";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkTouchEnabled, ultraToolTipInfo3);
            // 
            // chkAnimations
            // 
            this.chkAnimations.Checked = global::DWOS.UI.Properties.Settings.Default.WIPAnimations;
            this.chkAnimations.Location = new System.Drawing.Point(150, 120);
            this.chkAnimations.Name = "chkAnimations";
            this.chkAnimations.Size = new System.Drawing.Size(310, 20);
            this.chkAnimations.TabIndex = 39;
            this.chkAnimations.Text = "Enable Animations";
            ultraToolTipInfo2.ToolTipTextFormatted = "If checked, .";
            ultraToolTipInfo2.ToolTipTitle = "Enable Animations";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkAnimations, ultraToolTipInfo2);
            // 
            // chkFullRowHighlight
            // 
            this.chkFullRowHighlight.Checked = global::DWOS.UI.Properties.Settings.Default.WIPAnimations;
            this.chkFullRowHighlight.Location = new System.Drawing.Point(150, 146);
            this.chkFullRowHighlight.Name = "chkFullRowHighlight";
            this.chkFullRowHighlight.Size = new System.Drawing.Size(310, 20);
            this.chkFullRowHighlight.TabIndex = 40;
            this.chkFullRowHighlight.Text = "Highlight Entire Row if Late";
            ultraToolTipInfo1.ToolTipTextFormatted = "If checked, the row will be hightlighted instead of the individual cell if an ord" +
    "er is late.";
            ultraToolTipInfo1.ToolTipTitle = "Highlight Full Row";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkFullRowHighlight, ultraToolTipInfo1);
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(15, 44);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(92, 15);
            this.ultraLabel8.TabIndex = 36;
            this.ultraLabel8.Text = "Default Printer:";
            // 
            // btnChangeServer
            // 
            this.btnChangeServer.AutoSize = true;
            this.btnChangeServer.Location = new System.Drawing.Point(477, 11);
            this.btnChangeServer.Name = "btnChangeServer";
            this.btnChangeServer.Size = new System.Drawing.Size(111, 25);
            this.btnChangeServer.TabIndex = 37;
            this.btnChangeServer.Text = "Change Server...";
            this.btnChangeServer.Click += new System.EventHandler(this.btnChangeServer_Click);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(150, 195);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(284, 15);
            this.ultraLabel1.TabIndex = 41;
            this.ultraLabel1.Text = "Some settings may require restart to take affect.";
            // 
            // UserSettingsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.chkFullRowHighlight);
            this.Controls.Add(this.chkAnimations);
            this.Controls.Add(this.chkTouchEnabled);
            this.Controls.Add(this.btnChangeServer);
            this.Controls.Add(this.cboPrinter);
            this.Controls.Add(this.ultraLabel8);
            this.Controls.Add(this.txtDatabaseName);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.chkResetLayout);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "UserSettingsView";
            this.Size = new System.Drawing.Size(694, 290);
            ((System.ComponentModel.ISupportInitialize)(this.chkResetLayout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDatabaseName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPrinter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkTouchEnabled)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAnimations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFullRowHighlight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkResetLayout;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDatabaseName;
		private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboPrinter;
		private Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private Infragistics.Win.Misc.UltraButton btnChangeServer;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkTouchEnabled;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkAnimations;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkFullRowHighlight;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
	}
}
