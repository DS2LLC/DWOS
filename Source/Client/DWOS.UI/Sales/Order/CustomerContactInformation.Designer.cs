namespace DWOS.UI.Sales
{
	partial class CustomerContactInformation
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Date the customer communication took place.", Infragistics.Win.ToolTipImage.Default, "Contact Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Contacted By", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The customer contact the communication occurred with.", Infragistics.Win.ToolTipImage.Default, "Customer Contact", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Notes about the communication that took place.", Infragistics.Win.ToolTipImage.Default, "Notes", Infragistics.Win.DefaultableBoolean.Default);
            this.dtContactDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.cboCommUserCreated = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboCommContact = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtNotes = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel17 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel16 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel11 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel9 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtContactDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCommUserCreated)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCommContact)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.ultraLabel16);
            this.grpData.Controls.Add(this.dtContactDate);
            this.grpData.Controls.Add(this.cboCommUserCreated);
            this.grpData.Controls.Add(this.cboCommContact);
            this.grpData.Controls.Add(this.txtNotes);
            this.grpData.Controls.Add(this.ultraLabel17);
            this.grpData.Controls.Add(this.ultraLabel9);
            this.grpData.Controls.Add(this.ultraLabel11);
            appearance2.Image = global::DWOS.UI.Properties.Resources.Phone_16;
            this.grpData.HeaderAppearance = appearance2;
            this.grpData.Size = new System.Drawing.Size(416, 271);
            this.grpData.Text = "Customer Communication";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel11, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel9, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel17, 0);
            this.grpData.Controls.SetChildIndex(this.txtNotes, 0);
            this.grpData.Controls.SetChildIndex(this.cboCommContact, 0);
            this.grpData.Controls.SetChildIndex(this.cboCommUserCreated, 0);
            this.grpData.Controls.SetChildIndex(this.dtContactDate, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel16, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(344, -725);
            // 
            // dtContactDate
            // 
            this.dtContactDate.AutoFillDate = Infragistics.Win.UltraWinMaskedEdit.AutoFillDate.MonthAndYear;
            this.dtContactDate.DateTime = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtContactDate.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.WindowsVista;
            this.dtContactDate.Location = new System.Drawing.Point(100, 85);
            this.dtContactDate.MaskInput = "{date} {time}";
            this.dtContactDate.Name = "dtContactDate";
            this.dtContactDate.Size = new System.Drawing.Size(149, 22);
            this.dtContactDate.TabIndex = 30;
            this.dtContactDate.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextSection;
            ultraToolTipInfo1.ToolTipText = "Date the customer communication took place.";
            ultraToolTipInfo1.ToolTipTitle = "Contact Date";
            this.tipManager.SetUltraToolTip(this.dtContactDate, ultraToolTipInfo1);
            this.dtContactDate.Value = null;
            // 
            // cboCommUserCreated
            // 
            this.cboCommUserCreated.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCommUserCreated.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            this.cboCommUserCreated.DropDownListWidth = -1;
            this.cboCommUserCreated.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCommUserCreated.Location = new System.Drawing.Point(100, 57);
            this.cboCommUserCreated.MaxLength = 50;
            this.cboCommUserCreated.Name = "cboCommUserCreated";
            this.cboCommUserCreated.NullText = "N/A";
            this.cboCommUserCreated.Size = new System.Drawing.Size(286, 22);
            this.cboCommUserCreated.TabIndex = 29;
            ultraToolTipInfo2.ToolTipTextFormatted = "The user who communicated with the customer contact.<br/>This field may be blank " +
    "if DWOS automatically contacted the customer.";
            ultraToolTipInfo2.ToolTipTitle = "Contacted By";
            this.tipManager.SetUltraToolTip(this.cboCommUserCreated, ultraToolTipInfo2);
            // 
            // cboCommContact
            // 
            this.cboCommContact.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCommContact.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance1.Image = global::DWOS.UI.Properties.Resources.Add_16;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            editorButton1.Appearance = appearance1;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.cboCommContact.ButtonsRight.Add(editorButton1);
            this.cboCommContact.DisplayMember = "Name";
            this.cboCommContact.DropDownListWidth = -1;
            this.cboCommContact.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCommContact.Location = new System.Drawing.Point(100, 28);
            this.cboCommContact.MaxLength = 50;
            this.cboCommContact.Name = "cboCommContact";
            this.cboCommContact.Size = new System.Drawing.Size(286, 22);
            this.cboCommContact.TabIndex = 28;
            ultraToolTipInfo3.ToolTipText = "The customer contact the communication occurred with.";
            ultraToolTipInfo3.ToolTipTitle = "Customer Contact";
            this.tipManager.SetUltraToolTip(this.cboCommContact, ultraToolTipInfo3);
            this.cboCommContact.ValueMember = "ContactID";
            this.cboCommContact.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.cboCommContact_EditorButtonClick);
            // 
            // txtNotes
            // 
            this.txtNotes.AcceptsReturn = true;
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotes.Location = new System.Drawing.Point(100, 113);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Nullable = false;
            this.txtNotes.Size = new System.Drawing.Size(286, 153);
            this.txtNotes.TabIndex = 31;
            ultraToolTipInfo4.ToolTipText = "Notes about the communication that took place.";
            ultraToolTipInfo4.ToolTipTitle = "Notes";
            this.tipManager.SetUltraToolTip(this.txtNotes, ultraToolTipInfo4);
            // 
            // ultraLabel17
            // 
            this.ultraLabel17.AutoSize = true;
            this.ultraLabel17.Location = new System.Drawing.Point(6, 113);
            this.ultraLabel17.Name = "ultraLabel17";
            this.ultraLabel17.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel17.TabIndex = 35;
            this.ultraLabel17.Text = "Notes:";
            // 
            // ultraLabel16
            // 
            this.ultraLabel16.AutoSize = true;
            this.ultraLabel16.Location = new System.Drawing.Point(6, 32);
            this.ultraLabel16.Name = "ultraLabel16";
            this.ultraLabel16.Size = new System.Drawing.Size(53, 15);
            this.ultraLabel16.TabIndex = 34;
            this.ultraLabel16.Text = "Contact:";
            // 
            // ultraLabel11
            // 
            this.ultraLabel11.AutoSize = true;
            this.ultraLabel11.Location = new System.Drawing.Point(6, 61);
            this.ultraLabel11.Name = "ultraLabel11";
            this.ultraLabel11.Size = new System.Drawing.Size(85, 15);
            this.ultraLabel11.TabIndex = 33;
            this.ultraLabel11.Text = "Contacted By:";
            // 
            // ultraLabel9
            // 
            this.ultraLabel9.AutoSize = true;
            this.ultraLabel9.Location = new System.Drawing.Point(6, 89);
            this.ultraLabel9.Name = "ultraLabel9";
            this.ultraLabel9.Size = new System.Drawing.Size(84, 15);
            this.ultraLabel9.TabIndex = 32;
            this.ultraLabel9.Text = "Contact Date:";
            // 
            // CustomerContactInformation
            // 
            this.Name = "CustomerContactInformation";
            this.Size = new System.Drawing.Size(422, 277);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtContactDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCommUserCreated)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCommContact)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtContactDate;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCommUserCreated;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCommContact;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNotes;
		private Infragistics.Win.Misc.UltraLabel ultraLabel17;
		private Infragistics.Win.Misc.UltraLabel ultraLabel16;
		private Infragistics.Win.Misc.UltraLabel ultraLabel11;
		private Infragistics.Win.Misc.UltraLabel ultraLabel9;
	}
}
