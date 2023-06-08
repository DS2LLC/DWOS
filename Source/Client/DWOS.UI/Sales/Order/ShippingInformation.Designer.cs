namespace DWOS.UI.Sales
{
	partial class ShippingInformation
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The shipping carrier used to ship the order to the customer.", Infragistics.Win.ToolTipImage.Default, "Shipping Carrier", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The carrier tracking number used to track the shipment.", Infragistics.Win.ToolTipImage.Default, "Tracking Number", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the order was shipped.", Infragistics.Win.ToolTipImage.Default, "Date shipped", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The user who shipped the order.", Infragistics.Win.ToolTipImage.Default, "Shipping Agent", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The number of parts shipped.", Infragistics.Win.ToolTipImage.Default, "Part Quantity", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The package number for this shipment.", Infragistics.Win.ToolTipImage.Default, "Package Number", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The gross weight of the entire package.", Infragistics.Win.ToolTipImage.Default, "Gross Weight", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Type of shipping package.", Infragistics.Win.ToolTipImage.Default, "Shipping Package Type", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Resends shipping notification email to customer contacts.", Infragistics.Win.ToolTipImage.Default, "Resend Notification Email", Infragistics.Win.DefaultableBoolean.Default);
            this.cboCourier = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.txtTrackingNumber = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.dteDateCertified = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.cboUser = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.numPartQty = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtShipment = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.numGrossWeight = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.lblGrossWeight = new Infragistics.Win.Misc.UltraLabel();
            this.txtPackageType = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.btnResend = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCourier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTrackingNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDateCertified)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGrossWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPackageType)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.btnResend);
            this.grpData.Controls.Add(this.ultraLabel3);
            this.grpData.Controls.Add(this.txtPackageType);
            this.grpData.Controls.Add(this.lblGrossWeight);
            this.grpData.Controls.Add(this.numGrossWeight);
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.txtShipment);
            this.grpData.Controls.Add(this.ultraLabel1);
            this.grpData.Controls.Add(this.numPartQty);
            this.grpData.Controls.Add(this.ultraLabel5);
            this.grpData.Controls.Add(this.dteDateCertified);
            this.grpData.Controls.Add(this.ultraLabel6);
            this.grpData.Controls.Add(this.cboUser);
            this.grpData.Controls.Add(this.txtTrackingNumber);
            this.grpData.Controls.Add(this.cboCourier);
            this.grpData.Controls.Add(this.ultraLabel7);
            this.grpData.Controls.Add(this.ultraLabel8);
            appearance2.Image = global::DWOS.UI.Properties.Resources.Shipping_16;
            this.grpData.HeaderAppearance = appearance2;
            this.grpData.Size = new System.Drawing.Size(367, 304);
            this.grpData.Text = "Order Shipment";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel8, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel7, 0);
            this.grpData.Controls.SetChildIndex(this.cboCourier, 0);
            this.grpData.Controls.SetChildIndex(this.txtTrackingNumber, 0);
            this.grpData.Controls.SetChildIndex(this.cboUser, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel6, 0);
            this.grpData.Controls.SetChildIndex(this.dteDateCertified, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel5, 0);
            this.grpData.Controls.SetChildIndex(this.numPartQty, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.txtShipment, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.grpData.Controls.SetChildIndex(this.numGrossWeight, 0);
            this.grpData.Controls.SetChildIndex(this.lblGrossWeight, 0);
            this.grpData.Controls.SetChildIndex(this.txtPackageType, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.grpData.Controls.SetChildIndex(this.btnResend, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(-181, -1910);
            // 
            // cboCourier
            // 
            this.cboCourier.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCourier.Location = new System.Drawing.Point(119, 141);
            this.cboCourier.Name = "cboCourier";
            this.cboCourier.ReadOnly = true;
            this.cboCourier.Size = new System.Drawing.Size(225, 22);
            this.cboCourier.TabIndex = 4;
            ultraToolTipInfo9.ToolTipText = "The shipping carrier used to ship the order to the customer.";
            ultraToolTipInfo9.ToolTipTitle = "Shipping Carrier";
            this.tipManager.SetUltraToolTip(this.cboCourier, ultraToolTipInfo9);
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(8, 173);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(89, 15);
            this.ultraLabel7.TabIndex = 28;
            this.ultraLabel7.Text = "Tracking Num:";
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(8, 145);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(48, 15);
            this.ultraLabel8.TabIndex = 27;
            this.ultraLabel8.Text = "Carrier:";
            // 
            // txtTrackingNumber
            // 
            this.txtTrackingNumber.Location = new System.Drawing.Point(119, 169);
            this.txtTrackingNumber.MaxLength = 255;
            this.txtTrackingNumber.Name = "txtTrackingNumber";
            this.txtTrackingNumber.ReadOnly = true;
            this.txtTrackingNumber.Size = new System.Drawing.Size(225, 22);
            this.txtTrackingNumber.TabIndex = 5;
            ultraToolTipInfo8.ToolTipText = "The carrier tracking number used to track the shipment.";
            ultraToolTipInfo8.ToolTipTitle = "Tracking Number";
            this.tipManager.SetUltraToolTip(this.txtTrackingNumber, ultraToolTipInfo8);
            // 
            // dteDateCertified
            // 
            this.dteDateCertified.Location = new System.Drawing.Point(119, 85);
            this.dteDateCertified.Name = "dteDateCertified";
            this.dteDateCertified.ReadOnly = true;
            this.dteDateCertified.Size = new System.Drawing.Size(144, 22);
            this.dteDateCertified.TabIndex = 2;
            ultraToolTipInfo6.ToolTipText = "The date the order was shipped.";
            ultraToolTipInfo6.ToolTipTitle = "Date shipped";
            this.tipManager.SetUltraToolTip(this.dteDateCertified, ultraToolTipInfo6);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(8, 89);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(86, 15);
            this.ultraLabel6.TabIndex = 32;
            this.ultraLabel6.Text = "Date Shipped:";
            // 
            // cboUser
            // 
            this.cboUser.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboUser.Location = new System.Drawing.Point(119, 57);
            this.cboUser.Name = "cboUser";
            this.cboUser.ReadOnly = true;
            this.cboUser.Size = new System.Drawing.Size(225, 22);
            this.cboUser.TabIndex = 1;
            ultraToolTipInfo7.ToolTipText = "The user who shipped the order.";
            ultraToolTipInfo7.ToolTipTitle = "Shipping Agent";
            this.tipManager.SetUltraToolTip(this.cboUser, ultraToolTipInfo7);
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(8, 61);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(96, 15);
            this.ultraLabel5.TabIndex = 30;
            this.ultraLabel5.Text = "Shipping Agent:";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(8, 201);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(58, 15);
            this.ultraLabel1.TabIndex = 60;
            this.ultraLabel1.Text = "Quantity:";
            // 
            // numPartQty
            // 
            this.numPartQty.Location = new System.Drawing.Point(119, 197);
            this.numPartQty.MaskInput = "nnn,nnn";
            this.numPartQty.MaxValue = 999999;
            this.numPartQty.MinValue = 1;
            this.numPartQty.Name = "numPartQty";
            this.numPartQty.NullText = "0";
            this.numPartQty.PromptChar = ' ';
            this.numPartQty.Size = new System.Drawing.Size(144, 22);
            this.numPartQty.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.OnMouseEnter;
            this.numPartQty.TabIndex = 6;
            ultraToolTipInfo5.ToolTipText = "The number of parts shipped.";
            ultraToolTipInfo5.ToolTipTitle = "Part Quantity";
            this.tipManager.SetUltraToolTip(this.numPartQty, ultraToolTipInfo5);
            this.numPartQty.Value = 1;
            // 
            // txtShipment
            // 
            this.txtShipment.Location = new System.Drawing.Point(119, 29);
            this.txtShipment.Name = "txtShipment";
            this.txtShipment.ReadOnly = true;
            this.txtShipment.Size = new System.Drawing.Size(225, 22);
            this.txtShipment.TabIndex = 0;
            ultraToolTipInfo4.ToolTipText = "The package number for this shipment.";
            ultraToolTipInfo4.ToolTipTitle = "Package Number";
            this.tipManager.SetUltraToolTip(this.txtShipment, ultraToolTipInfo4);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(8, 33);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(105, 15);
            this.ultraLabel2.TabIndex = 62;
            this.ultraLabel2.Text = "Package Number:";
            // 
            // numGrossWeight
            // 
            appearance1.TextHAlignAsString = "Left";
            this.numGrossWeight.Appearance = appearance1;
            this.numGrossWeight.Location = new System.Drawing.Point(119, 225);
            this.numGrossWeight.MaskInput = "n lbs.";
            this.numGrossWeight.MaxValue = new decimal(new int[] {
            1874919423,
            2328306,
            0,
            524288});
            this.numGrossWeight.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numGrossWeight.Name = "numGrossWeight";
            this.numGrossWeight.Nullable = true;
            this.numGrossWeight.NullText = "(Estimated from Order Weights)";
            this.numGrossWeight.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.numGrossWeight.ReadOnly = true;
            this.numGrossWeight.Size = new System.Drawing.Size(225, 22);
            this.numGrossWeight.TabIndex = 7;
            ultraToolTipInfo3.ToolTipText = "The gross weight of the entire package.";
            ultraToolTipInfo3.ToolTipTitle = "Gross Weight";
            this.tipManager.SetUltraToolTip(this.numGrossWeight, ultraToolTipInfo3);
            // 
            // lblGrossWeight
            // 
            this.lblGrossWeight.AutoSize = true;
            this.lblGrossWeight.Location = new System.Drawing.Point(8, 229);
            this.lblGrossWeight.Name = "lblGrossWeight";
            this.lblGrossWeight.Size = new System.Drawing.Size(85, 15);
            this.lblGrossWeight.TabIndex = 64;
            this.lblGrossWeight.Text = "Gross Weight:";
            // 
            // txtPackageType
            // 
            this.txtPackageType.Location = new System.Drawing.Point(119, 113);
            this.txtPackageType.Name = "txtPackageType";
            this.txtPackageType.ReadOnly = true;
            this.txtPackageType.Size = new System.Drawing.Size(225, 22);
            this.txtPackageType.TabIndex = 3;
            ultraToolTipInfo2.ToolTipText = "Type of shipping package.";
            ultraToolTipInfo2.ToolTipTitle = "Shipping Package Type";
            this.tipManager.SetUltraToolTip(this.txtPackageType, ultraToolTipInfo2);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(8, 117);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(88, 15);
            this.ultraLabel3.TabIndex = 65;
            this.ultraLabel3.Text = "Package Type:";
            // 
            // btnResend
            // 
            this.btnResend.Location = new System.Drawing.Point(119, 253);
            this.btnResend.Name = "btnResend";
            this.btnResend.Size = new System.Drawing.Size(225, 23);
            this.btnResend.TabIndex = 8;
            this.btnResend.Text = "Resend Notification Email";
            ultraToolTipInfo1.ToolTipText = "Resends shipping notification email to customer contacts.";
            ultraToolTipInfo1.ToolTipTitle = "Resend Notification Email";
            this.tipManager.SetUltraToolTip(this.btnResend, ultraToolTipInfo1);
            this.btnResend.Click += new System.EventHandler(this.btnResend_Click);
            // 
            // ShippingInformation
            // 
            this.Name = "ShippingInformation";
            this.Size = new System.Drawing.Size(373, 310);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCourier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTrackingNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDateCertified)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGrossWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPackageType)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCourier;
		private Infragistics.Win.Misc.UltraLabel ultraLabel7;
		private Infragistics.Win.Misc.UltraLabel ultraLabel8;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTrackingNumber;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteDateCertified;
		private Infragistics.Win.Misc.UltraLabel ultraLabel6;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboUser;
		private Infragistics.Win.Misc.UltraLabel ultraLabel5;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraNumericEditor numPartQty;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtShipment;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel lblGrossWeight;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numGrossWeight;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPackageType;
        private Infragistics.Win.Misc.UltraButton btnResend;
    }
}
