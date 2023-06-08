namespace DWOS.UI.Admin.SettingsPanels
{
	partial class SettingsQuality
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsQuality));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "After Completing COC", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Quality Director\'s Title", Infragistics.Win.ToolTipImage.Default, "Title", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Name of Quality Director", Infragistics.Win.ToolTipImage.Default, "Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The default Certification verbiage on DWOS certifications.", Infragistics.Win.ToolTipImage.Default, "Default COC Warranty", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Removes existing image", Infragistics.Win.ToolTipImage.Default, "", Infragistics.Win.DefaultableBoolean.Default);
            this.btnBrowse = new Infragistics.Win.Misc.UltraButton();
            this.picQualitySignature = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboPrintOption = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.txtQualityTitle = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.txtQualityName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCOC = new Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.ultraButton1 = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboPrintOption)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtQualityTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtQualityName)).BeginInit();
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(667, 429);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 45;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // picQualitySignature
            // 
            this.picQualitySignature.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picQualitySignature.BorderShadowColor = System.Drawing.Color.Empty;
            this.picQualitySignature.BorderStyle = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.picQualitySignature.Image = ((object)(resources.GetObject("picQualitySignature.Image")));
            this.picQualitySignature.Location = new System.Drawing.Point(77, 338);
            this.picQualitySignature.Name = "picQualitySignature";
            this.picQualitySignature.Size = new System.Drawing.Size(665, 85);
            this.picQualitySignature.TabIndex = 44;
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(9, 261);
            this.ultraLabel5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(154, 15);
            this.ultraLabel5.TabIndex = 43;
            this.ultraLabel5.Text = "Default Quality Signature:";
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.ContentPadding.Bottom = 35;
            this.ultraGroupBox1.Controls.Add(this.ultraButton1);
            this.ultraGroupBox1.Controls.Add(this.cboPrintOption);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel6);
            this.ultraGroupBox1.Controls.Add(this.txtQualityTitle);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel4);
            this.ultraGroupBox1.Controls.Add(this.txtQualityName);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Controls.Add(this.txtCOC);
            this.ultraGroupBox1.Controls.Add(this.btnBrowse);
            this.ultraGroupBox1.Controls.Add(this.picQualitySignature);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel5);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraGroupBox1.MinimumSize = new System.Drawing.Size(420, 360);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(748, 464);
            this.ultraGroupBox1.TabIndex = 37;
            this.ultraGroupBox1.Text = "Quality";
            // 
            // cboPrintOption
            // 
            this.cboPrintOption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPrintOption.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboPrintOption.Location = new System.Drawing.Point(149, 236);
            this.cboPrintOption.Name = "cboPrintOption";
            this.cboPrintOption.Size = new System.Drawing.Size(593, 22);
            this.cboPrintOption.TabIndex = 53;
            ultraToolTipInfo2.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo2.ToolTipTextFormatted");
            ultraToolTipInfo2.ToolTipTitle = "After Completing COC";
            this.ultraToolTipManager.SetUltraToolTip(this.cboPrintOption, ultraToolTipInfo2);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(9, 240);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(134, 15);
            this.ultraLabel6.TabIndex = 52;
            this.ultraLabel6.Text = "After Completing COC:";
            // 
            // txtQualityTitle
            // 
            this.txtQualityTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQualityTitle.Location = new System.Drawing.Point(77, 310);
            this.txtQualityTitle.Name = "txtQualityTitle";
            this.txtQualityTitle.Size = new System.Drawing.Size(665, 22);
            this.txtQualityTitle.TabIndex = 51;
            ultraToolTipInfo3.ToolTipText = "Quality Director\'s Title";
            ultraToolTipInfo3.ToolTipTitle = "Title";
            this.ultraToolTipManager.SetUltraToolTip(this.txtQualityTitle, ultraToolTipInfo3);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(18, 314);
            this.ultraLabel4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(34, 15);
            this.ultraLabel4.TabIndex = 50;
            this.ultraLabel4.Text = "Title:";
            // 
            // txtQualityName
            // 
            this.txtQualityName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQualityName.Location = new System.Drawing.Point(77, 282);
            this.txtQualityName.Name = "txtQualityName";
            this.txtQualityName.Size = new System.Drawing.Size(665, 22);
            this.txtQualityName.TabIndex = 49;
            ultraToolTipInfo4.ToolTipText = "Name of Quality Director";
            ultraToolTipInfo4.ToolTipTitle = "Name";
            this.ultraToolTipManager.SetUltraToolTip(this.txtQualityName, ultraToolTipInfo4);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(18, 286);
            this.ultraLabel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel3.TabIndex = 48;
            this.ultraLabel3.Text = "Name:";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(18, 338);
            this.ultraLabel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(46, 15);
            this.ultraLabel1.TabIndex = 47;
            this.ultraLabel1.Text = "Image:";
            // 
            // txtCOC
            // 
            this.txtCOC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.FontData.BoldAsString = "False";
            appearance1.FontData.ItalicAsString = "False";
            appearance1.FontData.Name = "Verdana";
            appearance1.FontData.SizeInPoints = 8.25F;
            appearance1.FontData.StrikeoutAsString = "False";
            appearance1.FontData.UnderlineAsString = "False";
            this.txtCOC.Appearance = appearance1;
            this.txtCOC.ContextMenuItems = ((Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems)((((((((((((((((((Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Cut | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Copy) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Paste) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Delete) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Undo) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Redo) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.SelectAll) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Font) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Image) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Link) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.LineAlignment) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Paragraph) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Bold) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Italics) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Underline) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.SpellingSuggestions) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Strikeout) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Reserved)));
            this.txtCOC.Location = new System.Drawing.Point(9, 52);
            this.txtCOC.Name = "txtCOC";
            this.txtCOC.Size = new System.Drawing.Size(733, 178);
            this.txtCOC.TabIndex = 46;
            ultraToolTipInfo5.ToolTipText = "The default Certification verbiage on DWOS certifications.";
            ultraToolTipInfo5.ToolTipTitle = "Default COC Warranty";
            this.ultraToolTipManager.SetUltraToolTip(this.txtCOC, ultraToolTipInfo5);
            this.txtCOC.Value = resources.GetString("txtCOC.Value");
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(7, 31);
            this.ultraLabel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(136, 15);
            this.ultraLabel2.TabIndex = 34;
            this.ultraLabel2.Text = "Default COC Warranty:";
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // ultraButton1
            // 
            this.ultraButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraButton1.Location = new System.Drawing.Point(586, 429);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(75, 23);
            this.ultraButton1.TabIndex = 54;
            this.ultraButton1.Text = "Clear";
            ultraToolTipInfo1.ToolTipText = "Removes existing image";
            this.ultraToolTipManager.SetUltraToolTip(this.ultraButton1, ultraToolTipInfo1);
            this.ultraButton1.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // SettingsQuality
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.ultraGroupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(420, 360);
            this.Name = "SettingsQuality";
            this.Size = new System.Drawing.Size(748, 464);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboPrintOption)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtQualityTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtQualityName)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnBrowse;
		private Infragistics.Win.UltraWinEditors.UltraPictureBox picQualitySignature;
		public Infragistics.Win.Misc.UltraLabel ultraLabel5;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
		public Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor txtCOC;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtQualityName;
		public Infragistics.Win.Misc.UltraLabel ultraLabel3;
		public Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtQualityTitle;
		public Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboPrintOption;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.Misc.UltraButton ultraButton1;
    }
}
