namespace DWOS.UI.Sales
{
    partial class InternalReworkInformation
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The original order that the rework came from.", Infragistics.Win.ToolTipImage.Default, "Original Order", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo13 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the COC was created.", Infragistics.Win.ToolTipImage.Default, "Certified Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The user who created the internal rework.", Infragistics.Win.ToolTipImage.Default, "Created By", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The order that was created during the rework", Infragistics.Win.ToolTipImage.Default, "Rework Order", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The type of rework.", Infragistics.Win.ToolTipImage.Default, "Rework Type", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("If the order was put on hold, this where the order is being held at.", Infragistics.Win.ToolTipImage.Default, "Hold Location", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The reason the order was reworked.", Infragistics.Win.ToolTipImage.Default, "Reason", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo14 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("If Active then the rework has not been joined back with the original order.", Infragistics.Win.ToolTipImage.Default, "Active", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo12 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Go to the original order.", Infragistics.Win.ToolTipImage.Default, "Go To", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo11 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Go to the rework order.", Infragistics.Win.ToolTipImage.Default, "Go To", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraLabel11 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel9 = new Infragistics.Win.Misc.UltraLabel();
            this.txtOriginalOrder = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.dtDateCertified = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.cboUserCreated = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtReworkOrder = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtReworkType = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtHoldLocation = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboReason = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.chkActive = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.btnGoToOriginal = new Infragistics.Win.Misc.UltraButton();
            this.btnGoToRework = new Infragistics.Win.Misc.UltraButton();
            this.pnlActive = new System.Windows.Forms.Panel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.txtNotes = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOriginalOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDateCertified)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUserCreated)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReworkOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReworkType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHoldLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActive)).BeginInit();
            this.pnlActive.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.txtNotes);
            this.grpData.Controls.Add(this.ultraLabel5);
            this.grpData.Controls.Add(this.btnGoToRework);
            this.grpData.Controls.Add(this.btnGoToOriginal);
            this.grpData.Controls.Add(this.cboReason);
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.ultraLabel3);
            this.grpData.Controls.Add(this.txtHoldLocation);
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.txtReworkType);
            this.grpData.Controls.Add(this.ultraLabel1);
            this.grpData.Controls.Add(this.txtReworkOrder);
            this.grpData.Controls.Add(this.ultraLabel6);
            this.grpData.Controls.Add(this.txtOriginalOrder);
            this.grpData.Controls.Add(this.ultraLabel9);
            this.grpData.Controls.Add(this.dtDateCertified);
            this.grpData.Controls.Add(this.cboUserCreated);
            this.grpData.Controls.Add(this.ultraLabel11);
            this.grpData.Controls.Add(this.pnlActive);
            this.grpData.Size = new System.Drawing.Size(325, 405);
            this.grpData.Text = "Internal Rework";
            this.grpData.Controls.SetChildIndex(this.pnlActive, 0);
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel11, 0);
            this.grpData.Controls.SetChildIndex(this.cboUserCreated, 0);
            this.grpData.Controls.SetChildIndex(this.dtDateCertified, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel9, 0);
            this.grpData.Controls.SetChildIndex(this.txtOriginalOrder, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel6, 0);
            this.grpData.Controls.SetChildIndex(this.txtReworkOrder, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.txtReworkType, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.grpData.Controls.SetChildIndex(this.txtHoldLocation, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            this.grpData.Controls.SetChildIndex(this.cboReason, 0);
            this.grpData.Controls.SetChildIndex(this.btnGoToOriginal, 0);
            this.grpData.Controls.SetChildIndex(this.btnGoToRework, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel5, 0);
            this.grpData.Controls.SetChildIndex(this.txtNotes, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(-11, -858);
            // 
            // ultraLabel11
            // 
            this.ultraLabel11.AutoSize = true;
            this.ultraLabel11.Location = new System.Drawing.Point(13, 195);
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
            this.txtOriginalOrder.TabIndex = 2;
            ultraToolTipInfo7.ToolTipText = "The original order that the rework came from.";
            ultraToolTipInfo7.ToolTipTitle = "Original Order";
            this.tipManager.SetUltraToolTip(this.txtOriginalOrder, ultraToolTipInfo7);
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
            this.dtDateCertified.TabIndex = 0;
            this.dtDateCertified.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextSection;
            ultraToolTipInfo13.ToolTipText = "The date the COC was created.";
            ultraToolTipInfo13.ToolTipTitle = "Certified Date";
            this.tipManager.SetUltraToolTip(this.dtDateCertified, ultraToolTipInfo13);
            this.dtDateCertified.Value = null;
            // 
            // cboUserCreated
            // 
            this.cboUserCreated.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            this.cboUserCreated.DropDownListWidth = -1;
            this.cboUserCreated.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboUserCreated.Location = new System.Drawing.Point(107, 192);
            this.cboUserCreated.MaxLength = 50;
            this.cboUserCreated.Name = "cboUserCreated";
            this.cboUserCreated.ReadOnly = true;
            this.cboUserCreated.Size = new System.Drawing.Size(210, 22);
            this.cboUserCreated.TabIndex = 9;
            ultraToolTipInfo9.ToolTipText = "The user who created the internal rework.";
            ultraToolTipInfo9.ToolTipTitle = "Created By";
            this.tipManager.SetUltraToolTip(this.cboUserCreated, ultraToolTipInfo9);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(13, 85);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(88, 15);
            this.ultraLabel1.TabIndex = 67;
            this.ultraLabel1.Text = "Rework Order:";
            // 
            // txtReworkOrder
            // 
            this.txtReworkOrder.Location = new System.Drawing.Point(107, 81);
            this.txtReworkOrder.Name = "txtReworkOrder";
            this.txtReworkOrder.ReadOnly = true;
            this.txtReworkOrder.Size = new System.Drawing.Size(111, 22);
            this.txtReworkOrder.TabIndex = 4;
            ultraToolTipInfo6.ToolTipText = "The order that was created during the rework";
            ultraToolTipInfo6.ToolTipTitle = "Rework Order";
            this.tipManager.SetUltraToolTip(this.txtReworkOrder, ultraToolTipInfo6);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(13, 113);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(83, 15);
            this.ultraLabel2.TabIndex = 69;
            this.ultraLabel2.Text = "Rework Type:";
            // 
            // txtReworkType
            // 
            this.txtReworkType.Location = new System.Drawing.Point(107, 109);
            this.txtReworkType.Name = "txtReworkType";
            this.txtReworkType.ReadOnly = true;
            this.txtReworkType.Size = new System.Drawing.Size(210, 22);
            this.txtReworkType.TabIndex = 6;
            ultraToolTipInfo5.ToolTipText = "The type of rework.";
            ultraToolTipInfo5.ToolTipTitle = "Rework Type";
            this.tipManager.SetUltraToolTip(this.txtReworkType, ultraToolTipInfo5);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(13, 141);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(86, 15);
            this.ultraLabel3.TabIndex = 71;
            this.ultraLabel3.Text = "Hold Location:";
            // 
            // txtHoldLocation
            // 
            this.txtHoldLocation.Location = new System.Drawing.Point(107, 137);
            this.txtHoldLocation.Name = "txtHoldLocation";
            this.txtHoldLocation.ReadOnly = true;
            this.txtHoldLocation.Size = new System.Drawing.Size(210, 22);
            this.txtHoldLocation.TabIndex = 7;
            ultraToolTipInfo4.ToolTipText = "If the order was put on hold, this where the order is being held at.";
            ultraToolTipInfo4.ToolTipTitle = "Hold Location";
            this.tipManager.SetUltraToolTip(this.txtHoldLocation, ultraToolTipInfo4);
            // 
            // cboReason
            // 
            this.cboReason.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            this.cboReason.DropDownListWidth = -1;
            this.cboReason.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboReason.Location = new System.Drawing.Point(107, 164);
            this.cboReason.MaxLength = 50;
            this.cboReason.Name = "cboReason";
            this.cboReason.ReadOnly = true;
            this.cboReason.Size = new System.Drawing.Size(210, 22);
            this.cboReason.TabIndex = 8;
            ultraToolTipInfo3.ToolTipText = "The reason the order was reworked.";
            ultraToolTipInfo3.ToolTipTitle = "Reason";
            this.tipManager.SetUltraToolTip(this.cboReason, ultraToolTipInfo3);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(13, 168);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(51, 15);
            this.ultraLabel4.TabIndex = 73;
            this.ultraLabel4.Text = "Reason:";
            // 
            // chkActive
            // 
            this.chkActive.AutoSize = true;
            this.chkActive.Enabled = false;
            this.chkActive.Location = new System.Drawing.Point(0, 0);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(56, 18);
            this.chkActive.TabIndex = 1;
            this.chkActive.Text = "Active";
            ultraToolTipInfo14.ToolTipText = "If Active then the rework has not been joined back with the original order.";
            ultraToolTipInfo14.ToolTipTitle = "Active";
            this.tipManager.SetUltraToolTip(this.chkActive, ultraToolTipInfo14);
            // 
            // btnGoToOriginal
            // 
            appearance4.Image = global::DWOS.UI.Properties.Resources.Run;
            this.btnGoToOriginal.Appearance = appearance4;
            this.btnGoToOriginal.AutoSize = true;
            this.btnGoToOriginal.Location = new System.Drawing.Point(235, 51);
            this.btnGoToOriginal.Name = "btnGoToOriginal";
            this.btnGoToOriginal.Size = new System.Drawing.Size(26, 26);
            this.btnGoToOriginal.TabIndex = 3;
            ultraToolTipInfo12.ToolTipText = "Go to the original order.";
            ultraToolTipInfo12.ToolTipTitle = "Go To";
            this.tipManager.SetUltraToolTip(this.btnGoToOriginal, ultraToolTipInfo12);
            this.btnGoToOriginal.Click += new System.EventHandler(this.btnGoToOriginal_Click);
            // 
            // btnGoToRework
            // 
            appearance3.Image = global::DWOS.UI.Properties.Resources.Run;
            this.btnGoToRework.Appearance = appearance3;
            this.btnGoToRework.AutoSize = true;
            this.btnGoToRework.Location = new System.Drawing.Point(235, 79);
            this.btnGoToRework.Name = "btnGoToRework";
            this.btnGoToRework.Size = new System.Drawing.Size(26, 26);
            this.btnGoToRework.TabIndex = 5;
            ultraToolTipInfo11.ToolTipText = "Go to the rework order.";
            ultraToolTipInfo11.ToolTipTitle = "Go To";
            this.tipManager.SetUltraToolTip(this.btnGoToRework, ultraToolTipInfo11);
            this.btnGoToRework.Click += new System.EventHandler(this.btnGoToRework_Click);
            // 
            // pnlActive
            // 
            this.pnlActive.Controls.Add(this.chkActive);
            this.pnlActive.Location = new System.Drawing.Point(235, 27);
            this.pnlActive.Name = "pnlActive";
            this.pnlActive.Size = new System.Drawing.Size(56, 18);
            this.pnlActive.TabIndex = 74;
            this.pnlActive.MouseLeave += new System.EventHandler(this.pnlActive_MouseLeave);
            this.pnlActive.MouseHover += new System.EventHandler(this.pnlActive_MouseHover);
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(13, 220);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel5.TabIndex = 75;
            this.ultraLabel5.Text = "Notes:";
            // 
            // txtNotes
            // 
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotes.Location = new System.Drawing.Point(107, 220);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ReadOnly = true;
            this.txtNotes.Size = new System.Drawing.Size(210, 171);
            this.txtNotes.TabIndex = 76;
            // 
            // InternalReworkInformation
            // 
            this.Name = "InternalReworkInformation";
            this.Size = new System.Drawing.Size(331, 411);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOriginalOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDateCertified)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUserCreated)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReworkOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReworkType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHoldLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActive)).EndInit();
            this.pnlActive.ResumeLayout(false);
            this.pnlActive.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel11;
		private Infragistics.Win.Misc.UltraLabel ultraLabel9;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtOriginalOrder;
		private Infragistics.Win.Misc.UltraLabel ultraLabel6;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtDateCertified;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboUserCreated;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkActive;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboReason;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtHoldLocation;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtReworkType;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtReworkOrder;
        private Infragistics.Win.Misc.UltraButton btnGoToRework;
        private Infragistics.Win.Misc.UltraButton btnGoToOriginal;
        private System.Windows.Forms.Panel pnlActive;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNotes;
    }
}
