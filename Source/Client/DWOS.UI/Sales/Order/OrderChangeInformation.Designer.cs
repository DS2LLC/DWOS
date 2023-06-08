namespace DWOS.UI.Sales
{
    partial class OrderChangeInformation
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The ID of the order.", Infragistics.Win.ToolTipImage.Default, "Original Order", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the COC was created.", Infragistics.Win.ToolTipImage.Default, "Certified Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The user who created the COC.", Infragistics.Win.ToolTipImage.Default, "Created By", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The child order.", Infragistics.Win.ToolTipImage.Default, "Child Order", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The reason the change occurred on the order.", Infragistics.Win.ToolTipImage.Default, "Reason", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The type of change that occurred to the order.", Infragistics.Win.ToolTipImage.Default, "Change Type.", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraLabel11 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel9 = new Infragistics.Win.Misc.UltraLabel();
            this.txtOriginalOrder = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.dtDateCertified = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.cboCOCUserCreated = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtNotes = new Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtReworkOrder = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.cboReason = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.btnGoToOriginal = new Infragistics.Win.Misc.UltraButton();
            this.btnGoToRework = new Infragistics.Win.Misc.UltraButton();
            this.cboChangeType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOriginalOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDateCertified)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCOCUserCreated)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReworkOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboChangeType)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.cboChangeType);
            this.grpData.Controls.Add(this.btnGoToRework);
            this.grpData.Controls.Add(this.btnGoToOriginal);
            this.grpData.Controls.Add(this.cboReason);
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.ultraLabel1);
            this.grpData.Controls.Add(this.txtReworkOrder);
            this.grpData.Controls.Add(this.txtNotes);
            this.grpData.Controls.Add(this.ultraLabel6);
            this.grpData.Controls.Add(this.txtOriginalOrder);
            this.grpData.Controls.Add(this.ultraLabel9);
            this.grpData.Controls.Add(this.dtDateCertified);
            this.grpData.Controls.Add(this.cboCOCUserCreated);
            this.grpData.Controls.Add(this.ultraLabel11);
            this.grpData.Size = new System.Drawing.Size(325, 316);
            this.grpData.Text = "Order Change";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel11, 0);
            this.grpData.Controls.SetChildIndex(this.cboCOCUserCreated, 0);
            this.grpData.Controls.SetChildIndex(this.dtDateCertified, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel9, 0);
            this.grpData.Controls.SetChildIndex(this.txtOriginalOrder, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel6, 0);
            this.grpData.Controls.SetChildIndex(this.txtNotes, 0);
            this.grpData.Controls.SetChildIndex(this.txtReworkOrder, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            this.grpData.Controls.SetChildIndex(this.cboReason, 0);
            this.grpData.Controls.SetChildIndex(this.btnGoToOriginal, 0);
            this.grpData.Controls.SetChildIndex(this.btnGoToRework, 0);
            this.grpData.Controls.SetChildIndex(this.cboChangeType, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(111, -610);
            // 
            // ultraLabel11
            // 
            this.ultraLabel11.AutoSize = true;
            this.ultraLabel11.Location = new System.Drawing.Point(13, 168);
            this.ultraLabel11.Name = "ultraLabel11";
            this.ultraLabel11.Size = new System.Drawing.Size(72, 15);
            this.ultraLabel11.TabIndex = 40;
            this.ultraLabel11.Text = "Created By:";
            // 
            // ultraLabel9
            // 
            this.ultraLabel9.AutoSize = true;
            this.ultraLabel9.Location = new System.Drawing.Point(13, 28);
            this.ultraLabel9.Name = "ultraLabel9";
            this.ultraLabel9.Size = new System.Drawing.Size(85, 15);
            this.ultraLabel9.TabIndex = 39;
            this.ultraLabel9.Text = "Date Created:";
            // 
            // txtOriginalOrder
            // 
            this.txtOriginalOrder.Location = new System.Drawing.Point(107, 53);
            this.txtOriginalOrder.Name = "txtOriginalOrder";
            this.txtOriginalOrder.ReadOnly = true;
            this.txtOriginalOrder.Size = new System.Drawing.Size(111, 22);
            this.txtOriginalOrder.TabIndex = 45;
            ultraToolTipInfo4.ToolTipText = "The ID of the order.";
            ultraToolTipInfo4.ToolTipTitle = "Original Order";
            this.tipManager.SetUltraToolTip(this.txtOriginalOrder, ultraToolTipInfo4);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(13, 57);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(90, 15);
            this.ultraLabel6.TabIndex = 46;
            this.ultraLabel6.Text = "Original Order:";
            // 
            // dtDateCertified
            // 
            this.dtDateCertified.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.WindowsVista;
            this.dtDateCertified.Location = new System.Drawing.Point(107, 25);
            this.dtDateCertified.Name = "dtDateCertified";
            this.dtDateCertified.ReadOnly = true;
            this.dtDateCertified.Size = new System.Drawing.Size(111, 22);
            this.dtDateCertified.TabIndex = 37;
            this.dtDateCertified.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextSection;
            ultraToolTipInfo5.ToolTipText = "The date the COC was created.";
            ultraToolTipInfo5.ToolTipTitle = "Certified Date";
            this.tipManager.SetUltraToolTip(this.dtDateCertified, ultraToolTipInfo5);
            this.dtDateCertified.Value = null;
            // 
            // cboCOCUserCreated
            // 
            this.cboCOCUserCreated.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            this.cboCOCUserCreated.DropDownListWidth = -1;
            this.cboCOCUserCreated.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCOCUserCreated.Location = new System.Drawing.Point(107, 165);
            this.cboCOCUserCreated.MaxLength = 50;
            this.cboCOCUserCreated.Name = "cboCOCUserCreated";
            this.cboCOCUserCreated.ReadOnly = true;
            this.cboCOCUserCreated.Size = new System.Drawing.Size(210, 22);
            this.cboCOCUserCreated.TabIndex = 36;
            ultraToolTipInfo6.ToolTipText = "The user who created the COC.";
            ultraToolTipInfo6.ToolTipTitle = "Created By";
            this.tipManager.SetUltraToolTip(this.cboCOCUserCreated, ultraToolTipInfo6);
            // 
            // txtNotes
            // 
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance3.FontData.BoldAsString = "False";
            appearance3.FontData.ItalicAsString = "False";
            appearance3.FontData.Name = "Verdana";
            appearance3.FontData.SizeInPoints = 8.25F;
            appearance3.FontData.StrikeoutAsString = "False";
            appearance3.FontData.UnderlineAsString = "False";
            this.txtNotes.Appearance = appearance3;
            this.txtNotes.Location = new System.Drawing.Point(6, 193);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ReadOnly = true;
            this.txtNotes.Size = new System.Drawing.Size(313, 117);
            this.txtNotes.TabIndex = 65;
            this.txtNotes.Value = "";
            this.txtNotes.WrapText = false;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(13, 85);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(74, 15);
            this.ultraLabel1.TabIndex = 67;
            this.ultraLabel1.Text = "Child Order:";
            // 
            // txtReworkOrder
            // 
            this.txtReworkOrder.Location = new System.Drawing.Point(107, 81);
            this.txtReworkOrder.Name = "txtReworkOrder";
            this.txtReworkOrder.ReadOnly = true;
            this.txtReworkOrder.Size = new System.Drawing.Size(111, 22);
            this.txtReworkOrder.TabIndex = 66;
            ultraToolTipInfo3.ToolTipText = "The child order.";
            ultraToolTipInfo3.ToolTipTitle = "Child Order";
            this.tipManager.SetUltraToolTip(this.txtReworkOrder, ultraToolTipInfo3);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(13, 113);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(37, 15);
            this.ultraLabel2.TabIndex = 69;
            this.ultraLabel2.Text = "Type:";
            // 
            // cboReason
            // 
            this.cboReason.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            this.cboReason.DropDownListWidth = -1;
            this.cboReason.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboReason.Location = new System.Drawing.Point(107, 137);
            this.cboReason.MaxLength = 50;
            this.cboReason.Name = "cboReason";
            this.cboReason.ReadOnly = true;
            this.cboReason.Size = new System.Drawing.Size(210, 22);
            this.cboReason.TabIndex = 72;
            ultraToolTipInfo2.ToolTipText = "The reason the change occurred on the order.";
            ultraToolTipInfo2.ToolTipTitle = "Reason";
            this.tipManager.SetUltraToolTip(this.cboReason, ultraToolTipInfo2);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(13, 141);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(51, 15);
            this.ultraLabel4.TabIndex = 73;
            this.ultraLabel4.Text = "Reason:";
            // 
            // btnGoToOriginal
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.Run;
            this.btnGoToOriginal.Appearance = appearance2;
            this.btnGoToOriginal.AutoSize = true;
            this.btnGoToOriginal.Location = new System.Drawing.Point(235, 51);
            this.btnGoToOriginal.Name = "btnGoToOriginal";
            this.btnGoToOriginal.Size = new System.Drawing.Size(26, 26);
            this.btnGoToOriginal.TabIndex = 75;
            this.btnGoToOriginal.Click += new System.EventHandler(this.btnGoToOriginal_Click);
            // 
            // btnGoToRework
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Run;
            this.btnGoToRework.Appearance = appearance1;
            this.btnGoToRework.AutoSize = true;
            this.btnGoToRework.Location = new System.Drawing.Point(235, 79);
            this.btnGoToRework.Name = "btnGoToRework";
            this.btnGoToRework.Size = new System.Drawing.Size(26, 26);
            this.btnGoToRework.TabIndex = 76;
            this.btnGoToRework.Click += new System.EventHandler(this.btnGoToRework_Click);
            // 
            // cboChangeType
            // 
            this.cboChangeType.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            this.cboChangeType.DropDownListWidth = -1;
            this.cboChangeType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboChangeType.Location = new System.Drawing.Point(107, 109);
            this.cboChangeType.MaxLength = 50;
            this.cboChangeType.Name = "cboChangeType";
            this.cboChangeType.ReadOnly = true;
            this.cboChangeType.Size = new System.Drawing.Size(210, 22);
            this.cboChangeType.TabIndex = 77;
            ultraToolTipInfo1.ToolTipText = "The type of change that occurred to the order.";
            ultraToolTipInfo1.ToolTipTitle = "Change Type.";
            this.tipManager.SetUltraToolTip(this.cboChangeType, ultraToolTipInfo1);
            // 
            // OrderChangeInformation
            // 
            this.Name = "OrderChangeInformation";
            this.Size = new System.Drawing.Size(331, 322);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOriginalOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDateCertified)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCOCUserCreated)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReworkOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboChangeType)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel11;
		private Infragistics.Win.Misc.UltraLabel ultraLabel9;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtOriginalOrder;
		private Infragistics.Win.Misc.UltraLabel ultraLabel6;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtDateCertified;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCOCUserCreated;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor txtNotes;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboReason;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtReworkOrder;
        private Infragistics.Win.Misc.UltraButton btnGoToRework;
        private Infragistics.Win.Misc.UltraButton btnGoToOriginal;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboChangeType;
	}
}
