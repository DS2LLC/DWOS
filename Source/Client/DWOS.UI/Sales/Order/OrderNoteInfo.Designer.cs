namespace DWOS.UI.Sales
{
    partial class OrderNoteInfo
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Note Type", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderNoteInfo));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The order notes.", Infragistics.Win.ToolTipImage.Default, "Notes", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the note was created.", Infragistics.Win.ToolTipImage.Default, "Date Note Created", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The user who added the note.", Infragistics.Win.ToolTipImage.Default, "User", Infragistics.Win.DefaultableBoolean.Default);
            this.cboNoteType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.txtNotes = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.dteDateAdded = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.cboUser = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboNoteType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDateAdded)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUser)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.ultraLabel5);
            this.grpData.Controls.Add(this.dteDateAdded);
            this.grpData.Controls.Add(this.ultraLabel6);
            this.grpData.Controls.Add(this.cboUser);
            this.grpData.Controls.Add(this.txtNotes);
            this.grpData.Controls.Add(this.cboNoteType);
            this.grpData.Controls.Add(this.ultraLabel7);
            this.grpData.Controls.Add(this.ultraLabel8);
            appearance1.Image = global::DWOS.UI.Properties.Resources.Note_16;
            this.grpData.HeaderAppearance = appearance1;
            this.grpData.Size = new System.Drawing.Size(367, 304);
            this.grpData.Text = "Order Note";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel8, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel7, 0);
            this.grpData.Controls.SetChildIndex(this.cboNoteType, 0);
            this.grpData.Controls.SetChildIndex(this.txtNotes, 0);
            this.grpData.Controls.SetChildIndex(this.cboUser, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel6, 0);
            this.grpData.Controls.SetChildIndex(this.dteDateAdded, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel5, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(299, -386);
            // 
            // cboNoteType
            // 
            this.cboNoteType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem1.DataValue = "Internal";
            valueListItem1.DisplayText = "Internal";
            valueListItem2.DataValue = "External";
            valueListItem2.DisplayText = "External";
            this.cboNoteType.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.cboNoteType.Location = new System.Drawing.Point(105, 85);
            this.cboNoteType.Name = "cboNoteType";
            this.cboNoteType.Size = new System.Drawing.Size(225, 22);
            this.cboNoteType.TabIndex = 2;
            this.cboNoteType.Text = "Internal";
            ultraToolTipInfo4.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo4.ToolTipTextFormatted");
            ultraToolTipInfo4.ToolTipTitle = "Note Type";
            this.tipManager.SetUltraToolTip(this.cboNoteType, ultraToolTipInfo4);
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(8, 117);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel7.TabIndex = 28;
            this.ultraLabel7.Text = "Notes:";
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(8, 89);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(37, 15);
            this.ultraLabel8.TabIndex = 27;
            this.ultraLabel8.Text = "Type:";
            // 
            // txtNotes
            // 
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotes.Location = new System.Drawing.Point(105, 113);
            this.txtNotes.MaxLength = 255;
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(225, 185);
            this.txtNotes.TabIndex = 3;
            ultraToolTipInfo3.ToolTipText = "The order notes.";
            ultraToolTipInfo3.ToolTipTitle = "Notes";
            this.tipManager.SetUltraToolTip(this.txtNotes, ultraToolTipInfo3);
            // 
            // dteDateAdded
            // 
            this.dteDateAdded.Location = new System.Drawing.Point(105, 57);
            this.dteDateAdded.Name = "dteDateAdded";
            this.dteDateAdded.ReadOnly = true;
            this.dteDateAdded.Size = new System.Drawing.Size(144, 22);
            this.dteDateAdded.TabIndex = 1;
            ultraToolTipInfo1.ToolTipText = "The date the note was created.";
            ultraToolTipInfo1.ToolTipTitle = "Date Note Created";
            this.tipManager.SetUltraToolTip(this.dteDateAdded, ultraToolTipInfo1);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(8, 61);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(36, 15);
            this.ultraLabel6.TabIndex = 32;
            this.ultraLabel6.Text = "Date:";
            // 
            // cboUser
            // 
            this.cboUser.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboUser.Location = new System.Drawing.Point(105, 29);
            this.cboUser.Name = "cboUser";
            this.cboUser.ReadOnly = true;
            this.cboUser.Size = new System.Drawing.Size(225, 22);
            this.cboUser.TabIndex = 0;
            ultraToolTipInfo2.ToolTipText = "The user who added the note.";
            ultraToolTipInfo2.ToolTipTitle = "User";
            this.tipManager.SetUltraToolTip(this.cboUser, ultraToolTipInfo2);
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(6, 33);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(35, 15);
            this.ultraLabel5.TabIndex = 30;
            this.ultraLabel5.Text = "User:";
            // 
            // OrderNoteInfo
            // 
            this.Name = "OrderNoteInfo";
            this.Size = new System.Drawing.Size(373, 310);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboNoteType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDateAdded)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUser)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboNoteType;
		private Infragistics.Win.Misc.UltraLabel ultraLabel7;
		private Infragistics.Win.Misc.UltraLabel ultraLabel8;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNotes;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteDateAdded;
		private Infragistics.Win.Misc.UltraLabel ultraLabel6;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboUser;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
	}
}
