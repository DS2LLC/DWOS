namespace DWOS.UI.Sales
{
    partial class HoldEventLog
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Hold Notes", Infragistics.Win.ToolTipImage.Default, "Notes", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("User who will place the work order on Hold.", Infragistics.Win.ToolTipImage.Default, "Entered By", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Reason why the work order is being put on Hold.", Infragistics.Win.ToolTipImage.Default, "Reason", Infragistics.Win.DefaultableBoolean.Default);
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel13 = new Infragistics.Win.Misc.UltraLabel();
            this.txtReason = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.txtUserName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboHoldReason = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.chkPrintLabel = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            ((System.ComponentModel.ISupportInitialize)(this.txtReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboHoldReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintLabel)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(246, 217);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(71, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ultraLabel13
            // 
            this.ultraLabel13.AutoSize = true;
            this.ultraLabel13.Location = new System.Drawing.Point(16, 16);
            this.ultraLabel13.Name = "ultraLabel13";
            this.ultraLabel13.Size = new System.Drawing.Size(72, 15);
            this.ultraLabel13.TabIndex = 71;
            this.ultraLabel13.Text = "Entered By:";
            // 
            // txtReason
            // 
            this.txtReason.AcceptsReturn = true;
            this.txtReason.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReason.Location = new System.Drawing.Point(110, 68);
            this.txtReason.MaxLength = 255;
            this.txtReason.Multiline = true;
            this.txtReason.Name = "txtReason";
            this.txtReason.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReason.Size = new System.Drawing.Size(207, 143);
            this.txtReason.TabIndex = 3;
            ultraToolTipInfo3.ToolTipText = "Hold Notes";
            ultraToolTipInfo3.ToolTipTitle = "Notes";
            this.ultraToolTipManager.SetUltraToolTip(this.txtReason, ultraToolTipInfo3);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(16, 44);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(51, 15);
            this.ultraLabel1.TabIndex = 74;
            this.ultraLabel1.Text = "Reason:";
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtUserName.Location = new System.Drawing.Point(110, 12);
            this.txtUserName.MaxLength = 50;
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.ReadOnly = true;
            this.txtUserName.Size = new System.Drawing.Size(207, 22);
            this.txtUserName.TabIndex = 1;
            ultraToolTipInfo2.ToolTipText = "User who will place the work order on Hold.";
            ultraToolTipInfo2.ToolTipTitle = "Entered By";
            this.ultraToolTipManager.SetUltraToolTip(this.txtUserName, ultraToolTipInfo2);
            // 
            // cboHoldReason
            // 
            this.cboHoldReason.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboHoldReason.Location = new System.Drawing.Point(110, 40);
            this.cboHoldReason.Name = "cboHoldReason";
            this.cboHoldReason.Size = new System.Drawing.Size(207, 22);
            this.cboHoldReason.TabIndex = 2;
            ultraToolTipInfo1.ToolTipText = "Reason why the work order is being put on Hold.";
            ultraToolTipInfo1.ToolTipTitle = "Reason";
            this.ultraToolTipManager.SetUltraToolTip(this.cboHoldReason, ultraToolTipInfo1);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(16, 71);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel2.TabIndex = 76;
            this.ultraLabel2.Text = "Notes:";
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // chkPrintLabel
            // 
            this.chkPrintLabel.AutoSize = true;
            this.chkPrintLabel.Location = new System.Drawing.Point(16, 221);
            this.chkPrintLabel.Name = "chkPrintLabel";
            this.chkPrintLabel.Size = new System.Drawing.Size(111, 18);
            this.chkPrintLabel.TabIndex = 775;
            this.chkPrintLabel.Text = "Print Hold Label";
            // 
            // HoldEventLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 252);
            this.Controls.Add(this.chkPrintLabel);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.cboHoldReason);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.txtReason);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.ultraLabel13);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "HoldEventLog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Hold Log";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.UserEventLog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboHoldReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintLabel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.Misc.UltraLabel ultraLabel13;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtReason;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtUserName;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboHoldReason;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPrintLabel;
    }
}