namespace DWOS.UI.Sales
{
	partial class COCInformation
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The unique ID of the COC.", Infragistics.Win.ToolTipImage.Default, "COC ID", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the COC was created.", Infragistics.Win.ToolTipImage.Default, "Certified Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The user who created the COC.", Infragistics.Win.ToolTipImage.Default, "Created By", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The number of parts which passed certification.", Infragistics.Win.ToolTipImage.Default, "Part Quantity", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.ultraLabel17 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel11 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel9 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCOCID = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.dtDateCertified = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.cboCOCUserCreated = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.numPartQty = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtCOCInfo = new Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCOCID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDateCertified)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCOCUserCreated)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.txtCOCInfo);
            this.grpData.Controls.Add(this.ultraLabel6);
            this.grpData.Controls.Add(this.numPartQty);
            this.grpData.Controls.Add(this.txtCOCID);
            this.grpData.Controls.Add(this.ultraLabel9);
            this.grpData.Controls.Add(this.dtDateCertified);
            this.grpData.Controls.Add(this.cboCOCUserCreated);
            this.grpData.Controls.Add(this.ultraLabel17);
            this.grpData.Controls.Add(this.ultraLabel11);
            appearance2.Image = global::DWOS.UI.Properties.Resources.Certificate_16;
            this.grpData.HeaderAppearance = appearance2;
            this.grpData.Size = new System.Drawing.Size(330, 304);
            this.grpData.Text = "Certificate of Conformance";
            this.grpData.SizeChanged += new System.EventHandler(this.grpData_SizeChanged);
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel11, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel17, 0);
            this.grpData.Controls.SetChildIndex(this.cboCOCUserCreated, 0);
            this.grpData.Controls.SetChildIndex(this.dtDateCertified, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel9, 0);
            this.grpData.Controls.SetChildIndex(this.txtCOCID, 0);
            this.grpData.Controls.SetChildIndex(this.numPartQty, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel6, 0);
            this.grpData.Controls.SetChildIndex(this.txtCOCInfo, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(419, -5);
            // 
            // ultraLabel17
            // 
            this.ultraLabel17.AutoSize = true;
            this.ultraLabel17.Location = new System.Drawing.Point(6, 117);
            this.ultraLabel17.Name = "ultraLabel17";
            this.ultraLabel17.Size = new System.Drawing.Size(85, 15);
            this.ultraLabel17.TabIndex = 41;
            this.ultraLabel17.Text = "Part Quantity:";
            // 
            // ultraLabel11
            // 
            this.ultraLabel11.AutoSize = true;
            this.ultraLabel11.Location = new System.Drawing.Point(6, 61);
            this.ultraLabel11.Name = "ultraLabel11";
            this.ultraLabel11.Size = new System.Drawing.Size(72, 15);
            this.ultraLabel11.TabIndex = 40;
            this.ultraLabel11.Text = "Created By:";
            // 
            // ultraLabel9
            // 
            this.ultraLabel9.AutoSize = true;
            this.ultraLabel9.Location = new System.Drawing.Point(6, 89);
            this.ultraLabel9.Name = "ultraLabel9";
            this.ultraLabel9.Size = new System.Drawing.Size(88, 15);
            this.ultraLabel9.TabIndex = 39;
            this.ultraLabel9.Text = "Certified Date:";
            // 
            // txtCOCID
            // 
            this.txtCOCID.Location = new System.Drawing.Point(100, 29);
            this.txtCOCID.Name = "txtCOCID";
            this.txtCOCID.ReadOnly = true;
            this.txtCOCID.Size = new System.Drawing.Size(111, 22);
            this.txtCOCID.TabIndex = 45;
            ultraToolTipInfo2.ToolTipText = "The unique ID of the COC.";
            ultraToolTipInfo2.ToolTipTitle = "COC ID";
            this.tipManager.SetUltraToolTip(this.txtCOCID, ultraToolTipInfo2);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(6, 33);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(52, 15);
            this.ultraLabel6.TabIndex = 46;
            this.ultraLabel6.Text = "COC ID:";
            // 
            // dtDateCertified
            // 
            this.dtDateCertified.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.WindowsVista;
            this.dtDateCertified.Location = new System.Drawing.Point(100, 85);
            this.dtDateCertified.Name = "dtDateCertified";
            this.dtDateCertified.ReadOnly = true;
            this.dtDateCertified.Size = new System.Drawing.Size(111, 22);
            this.dtDateCertified.TabIndex = 37;
            this.dtDateCertified.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextSection;
            ultraToolTipInfo3.ToolTipText = "The date the COC was created.";
            ultraToolTipInfo3.ToolTipTitle = "Certified Date";
            this.tipManager.SetUltraToolTip(this.dtDateCertified, ultraToolTipInfo3);
            this.dtDateCertified.Value = null;
            // 
            // cboCOCUserCreated
            // 
            this.cboCOCUserCreated.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            this.cboCOCUserCreated.DropDownListWidth = -1;
            this.cboCOCUserCreated.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCOCUserCreated.Location = new System.Drawing.Point(100, 57);
            this.cboCOCUserCreated.MaxLength = 50;
            this.cboCOCUserCreated.Name = "cboCOCUserCreated";
            this.cboCOCUserCreated.ReadOnly = true;
            this.cboCOCUserCreated.Size = new System.Drawing.Size(210, 22);
            this.cboCOCUserCreated.TabIndex = 36;
            ultraToolTipInfo4.ToolTipText = "The user who created the COC.";
            ultraToolTipInfo4.ToolTipTitle = "Created By";
            this.tipManager.SetUltraToolTip(this.cboCOCUserCreated, ultraToolTipInfo4);
            // 
            // numPartQty
            // 
            this.numPartQty.Location = new System.Drawing.Point(100, 113);
            this.numPartQty.MaskInput = "nnn,nnn,nnn";
            this.numPartQty.MinValue = 1;
            this.numPartQty.Name = "numPartQty";
            this.numPartQty.NullText = "0";
            this.numPartQty.PromptChar = ' ';
            this.numPartQty.Size = new System.Drawing.Size(111, 22);
            this.numPartQty.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.OnMouseEnter;
            this.numPartQty.TabIndex = 64;
            ultraToolTipInfo1.ToolTipText = "The number of parts which passed certification.";
            ultraToolTipInfo1.ToolTipTitle = "Part Quantity";
            this.tipManager.SetUltraToolTip(this.numPartQty, ultraToolTipInfo1);
            this.numPartQty.Value = 1;
            // 
            // txtCOCInfo
            // 
            this.txtCOCInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.FontData.BoldAsString = "False";
            appearance1.FontData.ItalicAsString = "False";
            appearance1.FontData.Name = "Verdana";
            appearance1.FontData.SizeInPoints = 8.25F;
            appearance1.FontData.StrikeoutAsString = "False";
            appearance1.FontData.UnderlineAsString = "False";
            this.txtCOCInfo.Appearance = appearance1;
            this.txtCOCInfo.ContextMenuItems = ((Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems)(((((((((((((((((Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Cut | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Copy) 
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
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Reserved)));
            this.txtCOCInfo.Location = new System.Drawing.Point(6, 141);
            this.txtCOCInfo.Name = "txtCOCInfo";
            this.txtCOCInfo.Size = new System.Drawing.Size(318, 157);
            this.txtCOCInfo.TabIndex = 65;
            this.txtCOCInfo.TextSectionBreakMode = Infragistics.Win.FormattedLinkLabel.TextSectionBreakMode.Word;
            this.txtCOCInfo.Value = "";
            this.txtCOCInfo.WrapText = false;
            // 
            // COCInformation
            // 
            this.Name = "COCInformation";
            this.Size = new System.Drawing.Size(336, 310);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCOCID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDateCertified)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCOCUserCreated)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraLabel ultraLabel17;
		private Infragistics.Win.Misc.UltraLabel ultraLabel11;
		private Infragistics.Win.Misc.UltraLabel ultraLabel9;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCOCID;
		private Infragistics.Win.Misc.UltraLabel ultraLabel6;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtDateCertified;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCOCUserCreated;
		private Infragistics.Win.UltraWinEditors.UltraNumericEditor numPartQty;
		private Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor txtCOCInfo;
	}
}
