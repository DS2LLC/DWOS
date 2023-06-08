namespace DWOS.UI.Sales.Order
{
	partial class OrderReviewSummaryPanel
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The status of the order review.", Infragistics.Win.ToolTipImage.Default, "Review Status", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Notes about the order.", Infragistics.Win.ToolTipImage.Default, "Notes", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The user who reviewed the order.", Infragistics.Win.ToolTipImage.Default, "Reviewed By", Infragistics.Win.DefaultableBoolean.Default);
            this.cboStatus = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtNotes = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel17 = new Infragistics.Win.Misc.UltraLabel();
            this.cboReviewUser = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel10 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReviewUser)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.cboStatus);
            this.grpData.Controls.Add(this.txtNotes);
            this.grpData.Controls.Add(this.ultraLabel17);
            this.grpData.Controls.Add(this.cboReviewUser);
            this.grpData.Controls.Add(this.ultraLabel10);
            this.grpData.Controls.Add(this.ultraLabel8);
            appearance1.Image = global::DWOS.UI.Properties.Resources.OrderReview_16;
            this.grpData.HeaderAppearance = appearance1;
            this.grpData.MinimumSize = new System.Drawing.Size(371, 188);
            this.grpData.Size = new System.Drawing.Size(375, 192);
            this.grpData.Text = "Order Review";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel8, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel10, 0);
            this.grpData.Controls.SetChildIndex(this.cboReviewUser, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel17, 0);
            this.grpData.Controls.SetChildIndex(this.txtNotes, 0);
            this.grpData.Controls.SetChildIndex(this.cboStatus, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(343, -187);
            // 
            // cboStatus
            // 
            this.cboStatus.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            this.cboStatus.DropDownListWidth = -1;
            this.cboStatus.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(104, 60);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Nullable = false;
            this.cboStatus.ReadOnly = true;
            this.cboStatus.Size = new System.Drawing.Size(121, 22);
            this.cboStatus.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboStatus.TabIndex = 1;
            ultraToolTipInfo1.ToolTipText = "The status of the order review.";
            ultraToolTipInfo1.ToolTipTitle = "Review Status";
            this.tipManager.SetUltraToolTip(this.cboStatus, ultraToolTipInfo1);
            // 
            // txtNotes
            // 
            this.txtNotes.AcceptsReturn = true;
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotes.Location = new System.Drawing.Point(104, 88);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Nullable = false;
            this.txtNotes.ReadOnly = true;
            this.txtNotes.Size = new System.Drawing.Size(265, 98);
            this.txtNotes.TabIndex = 2;
            ultraToolTipInfo2.ToolTipText = "Notes about the order.";
            ultraToolTipInfo2.ToolTipTitle = "Notes";
            this.tipManager.SetUltraToolTip(this.txtNotes, ultraToolTipInfo2);
            // 
            // ultraLabel17
            // 
            this.ultraLabel17.AutoSize = true;
            this.ultraLabel17.Location = new System.Drawing.Point(9, 88);
            this.ultraLabel17.Name = "ultraLabel17";
            this.ultraLabel17.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel17.TabIndex = 171;
            this.ultraLabel17.Text = "Notes:";
            // 
            // cboReviewUser
            // 
            this.cboReviewUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboReviewUser.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            this.cboReviewUser.DropDownListWidth = -1;
            this.cboReviewUser.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboReviewUser.Location = new System.Drawing.Point(104, 32);
            this.cboReviewUser.Name = "cboReviewUser";
            this.cboReviewUser.Nullable = false;
            this.cboReviewUser.ReadOnly = true;
            this.cboReviewUser.Size = new System.Drawing.Size(265, 22);
            this.cboReviewUser.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboReviewUser.TabIndex = 0;
            ultraToolTipInfo3.ToolTipText = "The user who reviewed the order.";
            ultraToolTipInfo3.ToolTipTitle = "Reviewed By";
            this.tipManager.SetUltraToolTip(this.cboReviewUser, ultraToolTipInfo3);
            // 
            // ultraLabel10
            // 
            this.ultraLabel10.AutoSize = true;
            this.ultraLabel10.Location = new System.Drawing.Point(9, 64);
            this.ultraLabel10.Name = "ultraLabel10";
            this.ultraLabel10.Size = new System.Drawing.Size(90, 15);
            this.ultraLabel10.TabIndex = 169;
            this.ultraLabel10.Text = "Review Status:";
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(9, 36);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(82, 15);
            this.ultraLabel8.TabIndex = 168;
            this.ultraLabel8.Text = "Reviewed By:";
            // 
            // OrderReviewSummaryPanel
            // 
            this.AutoScroll = true;
            this.Name = "OrderReviewSummaryPanel";
            this.Size = new System.Drawing.Size(381, 198);
            this.Resize += new System.EventHandler(this.OrderReviewSummaryPanel_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReviewUser)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboStatus;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNotes;
		private Infragistics.Win.Misc.UltraLabel ultraLabel17;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboReviewUser;
		private Infragistics.Win.Misc.UltraLabel ultraLabel10;
		private Infragistics.Win.Misc.UltraLabel ultraLabel8;
	}
}
