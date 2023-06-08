namespace DWOS.UI.Sales.Customer
{
	partial class CustomerShipping
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("A shipping carrier used by the customer.", Infragistics.Win.ToolTipImage.Default, "Shipping Carrier", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The shipping carriers code used by the customer to ship with this shipper.", Infragistics.Win.ToolTipImage.Default, "Shipping Carrier Code", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Default Shipping Method", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Determines if the shipping method is active or not.", Infragistics.Win.ToolTipImage.Default, "Active", Infragistics.Win.DefaultableBoolean.Default);
            this.cboCarrier = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.lblCarrier = new Infragistics.Win.Misc.UltraLabel();
            this.txtCarrierNumber = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lblCarrierNumber = new Infragistics.Win.Misc.UltraLabel();
            this.chkDefault = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkActive = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCarrier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCarrierNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkDefault)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActive)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.chkActive);
            this.grpData.Controls.Add(this.chkDefault);
            this.grpData.Controls.Add(this.lblCarrier);
            this.grpData.Controls.Add(this.lblCarrierNumber);
            this.grpData.Controls.Add(this.cboCarrier);
            this.grpData.Controls.Add(this.txtCarrierNumber);
            appearance1.Image = global::DWOS.UI.Properties.Resources.Shipping_16;
            this.grpData.HeaderAppearance = appearance1;
            this.grpData.Size = new System.Drawing.Size(385, 304);
            this.grpData.Text = "Shipping Method";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.txtCarrierNumber, 0);
            this.grpData.Controls.SetChildIndex(this.cboCarrier, 0);
            this.grpData.Controls.SetChildIndex(this.lblCarrierNumber, 0);
            this.grpData.Controls.SetChildIndex(this.lblCarrier, 0);
            this.grpData.Controls.SetChildIndex(this.chkDefault, 0);
            this.grpData.Controls.SetChildIndex(this.chkActive, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(133, -894);
            // 
            // cboCarrier
            // 
            this.cboCarrier.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            this.cboCarrier.DropDownListWidth = -1;
            this.cboCarrier.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCarrier.Location = new System.Drawing.Point(100, 28);
            this.cboCarrier.MaxLength = 50;
            this.cboCarrier.Name = "cboCarrier";
            this.cboCarrier.Size = new System.Drawing.Size(215, 22);
            this.cboCarrier.TabIndex = 0;
            ultraToolTipInfo3.ToolTipText = "A shipping carrier used by the customer.";
            ultraToolTipInfo3.ToolTipTitle = "Shipping Carrier";
            this.tipManager.SetUltraToolTip(this.cboCarrier, ultraToolTipInfo3);
            // 
            // lblCarrier
            // 
            this.lblCarrier.AutoSize = true;
            this.lblCarrier.Location = new System.Drawing.Point(6, 32);
            this.lblCarrier.Name = "lblCarrier";
            this.lblCarrier.Size = new System.Drawing.Size(48, 15);
            this.lblCarrier.TabIndex = 39;
            this.lblCarrier.Text = "Carrier:";
            // 
            // txtCarrierNumber
            // 
            this.txtCarrierNumber.Location = new System.Drawing.Point(100, 56);
            this.txtCarrierNumber.Name = "txtCarrierNumber";
            this.txtCarrierNumber.Nullable = false;
            this.txtCarrierNumber.Size = new System.Drawing.Size(277, 22);
            this.txtCarrierNumber.TabIndex = 2;
            ultraToolTipInfo4.ToolTipText = "The shipping carriers code used by the customer to ship with this shipper.";
            ultraToolTipInfo4.ToolTipTitle = "Shipping Carrier Code";
            this.tipManager.SetUltraToolTip(this.txtCarrierNumber, ultraToolTipInfo4);
            // 
            // lblCarrierNumber
            // 
            this.lblCarrierNumber.AutoSize = true;
            this.lblCarrierNumber.Location = new System.Drawing.Point(6, 59);
            this.lblCarrierNumber.Name = "lblCarrierNumber";
            this.lblCarrierNumber.Size = new System.Drawing.Size(81, 15);
            this.lblCarrierNumber.TabIndex = 36;
            this.lblCarrierNumber.Text = "Carrier Code:";
            // 
            // chkDefault
            // 
            this.chkDefault.AutoSize = true;
            this.chkDefault.Location = new System.Drawing.Point(100, 84);
            this.chkDefault.Name = "chkDefault";
            this.chkDefault.Size = new System.Drawing.Size(161, 18);
            this.chkDefault.TabIndex = 3;
            this.chkDefault.Text = "Default Shipping Method";
            ultraToolTipInfo2.ToolTipTextFormatted = "If checked, this shipping method will be used by default for all new orders.&edsp" +
    ";";
            ultraToolTipInfo2.ToolTipTitle = "Default Shipping Method";
            this.tipManager.SetUltraToolTip(this.chkDefault, ultraToolTipInfo2);
            // 
            // chkActive
            // 
            this.chkActive.AutoSize = true;
            this.chkActive.Location = new System.Drawing.Point(321, 31);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(56, 18);
            this.chkActive.TabIndex = 1;
            this.chkActive.Text = "Active";
            ultraToolTipInfo1.ToolTipText = "Determines if the shipping method is active or not.";
            ultraToolTipInfo1.ToolTipTitle = "Active";
            this.tipManager.SetUltraToolTip(this.chkActive, ultraToolTipInfo1);
            // 
            // CustomerShipping
            // 
            this.Name = "CustomerShipping";
            this.Size = new System.Drawing.Size(391, 310);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCarrier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCarrierNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkDefault)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActive)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCarrier;
		private Infragistics.Win.Misc.UltraLabel lblCarrier;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCarrierNumber;
		private Infragistics.Win.Misc.UltraLabel lblCarrierNumber;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkDefault;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkActive;
    }
}
