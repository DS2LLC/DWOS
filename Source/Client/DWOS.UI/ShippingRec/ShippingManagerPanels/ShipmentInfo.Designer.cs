namespace DWOS.UI.ShippingRec.ShippingManagerPanels
{
	partial class ShipmentInfo
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
            _dsOrders?.Dispose();
            _dsOrders = null;
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("BindingList`1", -1);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("This package\'s container type.", Infragistics.Win.ToolTipImage.Default, "Container Type", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The carrier number assigned to the customer.", Infragistics.Win.ToolTipImage.Default, "Carrier Number", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The number for this package.  Some shipments are separated into multiple boxes.", Infragistics.Win.ToolTipImage.Default, "Package Number", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The shipping address of the shipment.", Infragistics.Win.ToolTipImage.Default, "Ship To", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The carrier used to ship the Package.", Infragistics.Win.ToolTipImage.Default, "Carrier", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The user that packaged the shipment.", Infragistics.Win.ToolTipImage.Default, "Shipping Agent", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The customer the package is being sent to.", Infragistics.Win.ToolTipImage.Default, "Customer", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Tracking Number of the package", Infragistics.Win.ToolTipImage.Default, "Tracking Number", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The contact information for shipping notifications for the selected customer. Sel" +
        "ected contacts will receive notification emails at the email addresses listed.", Infragistics.Win.ToolTipImage.Default, "Notifications", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo10 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The package ship date.", Infragistics.Win.ToolTipImage.Default, "Ship Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo11 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The gross weight of the shipment package.", Infragistics.Win.ToolTipImage.Default, "Gross Weight", Infragistics.Win.DefaultableBoolean.Default);
            this.grpOrders = new Infragistics.Win.Misc.UltraGroupBox();
            this.grdOrders = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.bsOrderShipments = new System.Windows.Forms.BindingSource(this.components);
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.grpShippingInfo = new Infragistics.Win.Misc.UltraGroupBox();
            this.pnlShippingTop = new Infragistics.Win.Misc.UltraPanel();
            this.lblMaximumNumber = new Infragistics.Win.Misc.UltraLabel();
            this.cboPackageType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtCarrierNumber = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.numPackageNumber = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.cboShipTo = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboShippingCarrier = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboUser = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboCustomer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.pnlShippingTrackingNumber = new Infragistics.Win.Misc.UltraPanel();
            this.txtTrackingNumber = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel9 = new Infragistics.Win.Misc.UltraLabel();
            this.pnlShippingBottom = new Infragistics.Win.Misc.UltraPanel();
            this.cboNotifications = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.ultraLabel10 = new Infragistics.Win.Misc.UltraLabel();
            this.lblWeight = new Infragistics.Win.Misc.UltraLabel();
            this.dteShipDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.numWeight = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.bsCustomerContacts = new System.Windows.Forms.BindingSource(this.components);
            this.bsCustomerAddress = new System.Windows.Forms.BindingSource(this.components);
            this.bsShippingCarrier = new System.Windows.Forms.BindingSource(this.components);
            this.shippingFlowManager = new Infragistics.Win.Misc.UltraFlowLayoutManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpOrders)).BeginInit();
            this.grpOrders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsOrderShipments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpShippingInfo)).BeginInit();
            this.grpShippingInfo.SuspendLayout();
            this.pnlShippingTop.ClientArea.SuspendLayout();
            this.pnlShippingTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboPackageType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCarrierNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPackageNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboShipTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboShippingCarrier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).BeginInit();
            this.pnlShippingTrackingNumber.ClientArea.SuspendLayout();
            this.pnlShippingTrackingNumber.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTrackingNumber)).BeginInit();
            this.pnlShippingBottom.ClientArea.SuspendLayout();
            this.pnlShippingBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboNotifications)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteShipDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCustomerContacts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCustomerAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsShippingCarrier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.shippingFlowManager)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.grpOrders);
            this.grpData.Controls.Add(this.grpShippingInfo);
            appearance29.Image = global::DWOS.UI.Properties.Resources.Package_16;
            this.grpData.HeaderAppearance = appearance29;
            this.grpData.Size = new System.Drawing.Size(502, 558);
            this.grpData.Text = "Shipment Package Info";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.grpShippingInfo, 0);
            this.grpData.Controls.SetChildIndex(this.grpOrders, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(167, -100);
            // 
            // grpOrders
            // 
            this.grpOrders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOrders.Controls.Add(this.grdOrders);
            appearance13.Image = global::DWOS.UI.Properties.Resources.Order_16;
            this.grpOrders.HeaderAppearance = appearance13;
            this.grpOrders.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpOrders.Location = new System.Drawing.Point(4, 340);
            this.grpOrders.Name = "grpOrders";
            this.grpOrders.Size = new System.Drawing.Size(494, 212);
            this.grpOrders.TabIndex = 1;
            this.grpOrders.Text = "Orders";
            // 
            // grdOrders
            // 
            this.grdOrders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdOrders.DataSource = this.bsOrderShipments;
            appearance1.AlphaLevel = ((short)(175));
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            appearance1.ImageBackground = global::DWOS.UI.Properties.Resources.Package_32;
            appearance1.ImageBackgroundAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
            this.grdOrders.DisplayLayout.Appearance = appearance1;
            this.grdOrders.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridBand1.AddButtonCaption = "Order Shipment";
            this.grdOrders.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdOrders.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdOrders.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.grdOrders.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdOrders.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.grdOrders.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdOrders.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.grdOrders.DisplayLayout.MaxColScrollRegions = 1;
            this.grdOrders.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdOrders.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdOrders.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grdOrders.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            this.grdOrders.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.grdOrders.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.grdOrders.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdOrders.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdOrders.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.grdOrders.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdOrders.DisplayLayout.Override.CellAppearance = appearance8;
            this.grdOrders.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdOrders.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.grdOrders.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.grdOrders.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.grdOrders.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdOrders.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.grdOrders.DisplayLayout.Override.RowAppearance = appearance11;
            this.grdOrders.DisplayLayout.Override.RowSelectorHeaderStyle = Infragistics.Win.UltraWinGrid.RowSelectorHeaderStyle.SeparateElement;
            this.grdOrders.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdOrders.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.grdOrders.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdOrders.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdOrders.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdOrders.Location = new System.Drawing.Point(6, 26);
            this.grdOrders.Name = "grdOrders";
            this.grdOrders.Size = new System.Drawing.Size(479, 180);
            this.grdOrders.StyleSetName = "DataGrid";
            this.grdOrders.TabIndex = 0;
            this.grdOrders.Text = "ultraGrid1";
            this.grdOrders.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdOrders_AfterCellUpdate);
            this.grdOrders.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdOrders_InitializeLayout);
            this.grdOrders.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdOrders_InitializeRow);
            this.grdOrders.AfterRowsDeleted += new System.EventHandler(this.grdOrders_AfterRowsDeleted);
            this.grdOrders.AfterRowInsert += new Infragistics.Win.UltraWinGrid.RowEventHandler(this.grdOrders_AfterRowInsert);
            this.grdOrders.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.grdOrders_BeforeRowsDeleted);
            // 
            // bsOrderShipments
            // 
            this.bsOrderShipments.Filter = "ShipmentPackageID = 0";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(0, 4);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(64, 15);
            this.ultraLabel3.TabIndex = 18;
            this.ultraLabel3.Text = "Customer:";
            // 
            // grpShippingInfo
            // 
            this.grpShippingInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpShippingInfo.Controls.Add(this.pnlShippingTop);
            this.grpShippingInfo.Controls.Add(this.pnlShippingTrackingNumber);
            this.grpShippingInfo.Controls.Add(this.pnlShippingBottom);
            appearance28.Image = global::DWOS.UI.Properties.Resources.Shipping_16;
            this.grpShippingInfo.HeaderAppearance = appearance28;
            this.grpShippingInfo.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpShippingInfo.Location = new System.Drawing.Point(4, 26);
            this.grpShippingInfo.Name = "grpShippingInfo";
            this.grpShippingInfo.Size = new System.Drawing.Size(494, 308);
            this.grpShippingInfo.TabIndex = 0;
            this.grpShippingInfo.Text = " Shipping Information";
            // 
            // pnlShippingTop
            // 
            this.pnlShippingTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // pnlShippingTop.ClientArea
            // 
            this.pnlShippingTop.ClientArea.Controls.Add(this.lblMaximumNumber);
            this.pnlShippingTop.ClientArea.Controls.Add(this.ultraLabel3);
            this.pnlShippingTop.ClientArea.Controls.Add(this.cboPackageType);
            this.pnlShippingTop.ClientArea.Controls.Add(this.txtCarrierNumber);
            this.pnlShippingTop.ClientArea.Controls.Add(this.ultraLabel6);
            this.pnlShippingTop.ClientArea.Controls.Add(this.ultraLabel8);
            this.pnlShippingTop.ClientArea.Controls.Add(this.numPackageNumber);
            this.pnlShippingTop.ClientArea.Controls.Add(this.ultraLabel7);
            this.pnlShippingTop.ClientArea.Controls.Add(this.cboShipTo);
            this.pnlShippingTop.ClientArea.Controls.Add(this.cboShippingCarrier);
            this.pnlShippingTop.ClientArea.Controls.Add(this.ultraLabel2);
            this.pnlShippingTop.ClientArea.Controls.Add(this.ultraLabel5);
            this.pnlShippingTop.ClientArea.Controls.Add(this.ultraLabel1);
            this.pnlShippingTop.ClientArea.Controls.Add(this.cboUser);
            this.pnlShippingTop.ClientArea.Controls.Add(this.cboCustomer);
            this.pnlShippingTop.Location = new System.Drawing.Point(3, 27);
            this.pnlShippingTop.Name = "pnlShippingTop";
            this.pnlShippingTop.Size = new System.Drawing.Size(482, 162);
            this.pnlShippingTop.TabIndex = 0;
            // 
            // lblMaximumNumber
            // 
            this.lblMaximumNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance14.TextHAlignAsString = "Right";
            this.lblMaximumNumber.Appearance = appearance14;
            this.lblMaximumNumber.AutoSize = true;
            this.lblMaximumNumber.Location = new System.Drawing.Point(447, 32);
            this.lblMaximumNumber.Name = "lblMaximumNumber";
            this.lblMaximumNumber.Size = new System.Drawing.Size(33, 15);
            this.lblMaximumNumber.TabIndex = 42;
            this.lblMaximumNumber.Text = "of YY";
            // 
            // cboPackageType
            // 
            this.cboPackageType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPackageType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboPackageType.Location = new System.Drawing.Point(116, 28);
            this.cboPackageType.Name = "cboPackageType";
            this.cboPackageType.Size = new System.Drawing.Size(188, 22);
            this.cboPackageType.TabIndex = 1;
            ultraToolTipInfo1.ToolTipText = "This package\'s container type.";
            ultraToolTipInfo1.ToolTipTitle = "Container Type";
            this.tipManager.SetUltraToolTip(this.cboPackageType, ultraToolTipInfo1);
            this.cboPackageType.ValueChanged += new System.EventHandler(this.cboPackageType_ValueChanged);
            // 
            // txtCarrierNumber
            // 
            this.txtCarrierNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCarrierNumber.Location = new System.Drawing.Point(116, 140);
            this.txtCarrierNumber.MaxLength = 255;
            this.txtCarrierNumber.Name = "txtCarrierNumber";
            this.txtCarrierNumber.Size = new System.Drawing.Size(364, 22);
            this.txtCarrierNumber.TabIndex = 6;
            ultraToolTipInfo2.ToolTipText = "The carrier number assigned to the customer.";
            ultraToolTipInfo2.ToolTipTitle = "Carrier Number";
            this.tipManager.SetUltraToolTip(this.txtCarrierNumber, ultraToolTipInfo2);
            this.txtCarrierNumber.ValueChanged += new System.EventHandler(this.txtCarrierNumber_ValueChanged);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(0, 32);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(96, 15);
            this.ultraLabel6.TabIndex = 41;
            this.ultraLabel6.Text = "Container Type:";
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(-1, 116);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(48, 15);
            this.ultraLabel8.TabIndex = 21;
            this.ultraLabel8.Text = "Carrier:";
            // 
            // numPackageNumber
            // 
            this.numPackageNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numPackageNumber.Location = new System.Drawing.Point(371, 28);
            this.numPackageNumber.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            this.numPackageNumber.MinValue = 1;
            this.numPackageNumber.Name = "numPackageNumber";
            this.numPackageNumber.Size = new System.Drawing.Size(70, 22);
            this.numPackageNumber.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.numPackageNumber.TabIndex = 2;
            ultraToolTipInfo3.ToolTipText = "The number for this package.  Some shipments are separated into multiple boxes.";
            ultraToolTipInfo3.ToolTipTitle = "Package Number";
            this.tipManager.SetUltraToolTip(this.numPackageNumber, ultraToolTipInfo3);
            this.numPackageNumber.ValueChanged += new System.EventHandler(this.numPackageNumber_ValueChanged);
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(-1, 144);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(98, 15);
            this.ultraLabel7.TabIndex = 22;
            this.ultraLabel7.Text = "Carrier Number:";
            // 
            // cboShipTo
            // 
            this.cboShipTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboShipTo.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboShipTo.Location = new System.Drawing.Point(116, 84);
            this.cboShipTo.Name = "cboShipTo";
            this.cboShipTo.Size = new System.Drawing.Size(364, 22);
            this.cboShipTo.TabIndex = 4;
            ultraToolTipInfo4.ToolTipText = "The shipping address of the shipment.";
            ultraToolTipInfo4.ToolTipTitle = "Ship To";
            this.tipManager.SetUltraToolTip(this.cboShipTo, ultraToolTipInfo4);
            this.cboShipTo.ValueChanged += new System.EventHandler(this.cboShipTo_ValueChanged);
            // 
            // cboShippingCarrier
            // 
            this.cboShippingCarrier.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboShippingCarrier.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboShippingCarrier.Location = new System.Drawing.Point(116, 112);
            this.cboShippingCarrier.Name = "cboShippingCarrier";
            this.cboShippingCarrier.Size = new System.Drawing.Size(364, 22);
            this.cboShippingCarrier.TabIndex = 5;
            ultraToolTipInfo5.ToolTipText = "The carrier used to ship the Package.";
            ultraToolTipInfo5.ToolTipTitle = "Carrier";
            this.tipManager.SetUltraToolTip(this.cboShippingCarrier, ultraToolTipInfo5);
            this.cboShippingCarrier.ValueChanged += new System.EventHandler(this.cboShippingCarrier_ValueChanged);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(0, 88);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(52, 15);
            this.ultraLabel2.TabIndex = 37;
            this.ultraLabel2.Text = "Ship To:";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(0, 60);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(96, 15);
            this.ultraLabel5.TabIndex = 21;
            this.ultraLabel5.Text = "Shipping Agent:";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(310, 32);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(55, 15);
            this.ultraLabel1.TabIndex = 33;
            this.ultraLabel1.Text = "Number:";
            // 
            // cboUser
            // 
            this.cboUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboUser.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboUser.Location = new System.Drawing.Point(116, 56);
            this.cboUser.Name = "cboUser";
            this.cboUser.ReadOnly = true;
            this.cboUser.Size = new System.Drawing.Size(364, 22);
            this.cboUser.TabIndex = 3;
            ultraToolTipInfo6.ToolTipText = "The user that packaged the shipment.";
            ultraToolTipInfo6.ToolTipTitle = "Shipping Agent";
            this.tipManager.SetUltraToolTip(this.cboUser, ultraToolTipInfo6);
            this.cboUser.ValueMember = "UserID";
            // 
            // cboCustomer
            // 
            this.cboCustomer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCustomer.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCustomer.Location = new System.Drawing.Point(116, 0);
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.ReadOnly = true;
            this.cboCustomer.Size = new System.Drawing.Size(364, 22);
            this.cboCustomer.TabIndex = 0;
            ultraToolTipInfo7.ToolTipText = "The customer the package is being sent to.";
            ultraToolTipInfo7.ToolTipTitle = "Customer";
            this.tipManager.SetUltraToolTip(this.cboCustomer, ultraToolTipInfo7);
            this.cboCustomer.ValueMember = "UserID";
            // 
            // pnlShippingTrackingNumber
            // 
            this.pnlShippingTrackingNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // pnlShippingTrackingNumber.ClientArea
            // 
            this.pnlShippingTrackingNumber.ClientArea.Controls.Add(this.txtTrackingNumber);
            this.pnlShippingTrackingNumber.ClientArea.Controls.Add(this.ultraLabel9);
            this.pnlShippingTrackingNumber.Location = new System.Drawing.Point(3, 194);
            this.pnlShippingTrackingNumber.Name = "pnlShippingTrackingNumber";
            this.pnlShippingTrackingNumber.Size = new System.Drawing.Size(482, 22);
            this.pnlShippingTrackingNumber.TabIndex = 1;
            // 
            // txtTrackingNumber
            // 
            this.txtTrackingNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTrackingNumber.Location = new System.Drawing.Point(116, 0);
            this.txtTrackingNumber.MaxLength = 255;
            this.txtTrackingNumber.Name = "txtTrackingNumber";
            this.txtTrackingNumber.Size = new System.Drawing.Size(364, 22);
            this.txtTrackingNumber.TabIndex = 7;
            ultraToolTipInfo8.ToolTipText = "Tracking Number of the package";
            ultraToolTipInfo8.ToolTipTitle = "Tracking Number";
            this.tipManager.SetUltraToolTip(this.txtTrackingNumber, ultraToolTipInfo8);
            this.txtTrackingNumber.ValueChanged += new System.EventHandler(this.txtTrackingNumber_ValueChanged);
            // 
            // ultraLabel9
            // 
            this.ultraLabel9.AutoSize = true;
            this.ultraLabel9.Location = new System.Drawing.Point(0, 4);
            this.ultraLabel9.Name = "ultraLabel9";
            this.ultraLabel9.Size = new System.Drawing.Size(107, 15);
            this.ultraLabel9.TabIndex = 27;
            this.ultraLabel9.Text = "Tracking Number:";
            // 
            // pnlShippingBottom
            // 
            this.pnlShippingBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // pnlShippingBottom.ClientArea
            // 
            this.pnlShippingBottom.ClientArea.Controls.Add(this.cboNotifications);
            this.pnlShippingBottom.ClientArea.Controls.Add(this.ultraLabel10);
            this.pnlShippingBottom.ClientArea.Controls.Add(this.lblWeight);
            this.pnlShippingBottom.ClientArea.Controls.Add(this.dteShipDate);
            this.pnlShippingBottom.ClientArea.Controls.Add(this.numWeight);
            this.pnlShippingBottom.ClientArea.Controls.Add(this.ultraLabel4);
            this.pnlShippingBottom.Location = new System.Drawing.Point(3, 221);
            this.pnlShippingBottom.Name = "pnlShippingBottom";
            this.pnlShippingBottom.Size = new System.Drawing.Size(482, 80);
            this.pnlShippingBottom.TabIndex = 2;
            // 
            // cboNotifications
            // 
            this.cboNotifications.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance15.BackColor = System.Drawing.SystemColors.Window;
            appearance15.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.cboNotifications.DisplayLayout.Appearance = appearance15;
            this.cboNotifications.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.cboNotifications.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance16.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance16.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance16.BorderColor = System.Drawing.SystemColors.Window;
            this.cboNotifications.DisplayLayout.GroupByBox.Appearance = appearance16;
            appearance17.ForeColor = System.Drawing.SystemColors.GrayText;
            this.cboNotifications.DisplayLayout.GroupByBox.BandLabelAppearance = appearance17;
            this.cboNotifications.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance18.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance18.BackColor2 = System.Drawing.SystemColors.Control;
            appearance18.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance18.ForeColor = System.Drawing.SystemColors.GrayText;
            this.cboNotifications.DisplayLayout.GroupByBox.PromptAppearance = appearance18;
            this.cboNotifications.DisplayLayout.MaxColScrollRegions = 1;
            this.cboNotifications.DisplayLayout.MaxRowScrollRegions = 1;
            appearance19.BackColor = System.Drawing.SystemColors.Window;
            appearance19.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cboNotifications.DisplayLayout.Override.ActiveCellAppearance = appearance19;
            appearance20.BackColor = System.Drawing.SystemColors.Highlight;
            appearance20.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.cboNotifications.DisplayLayout.Override.ActiveRowAppearance = appearance20;
            this.cboNotifications.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.cboNotifications.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance21.BackColor = System.Drawing.SystemColors.Window;
            this.cboNotifications.DisplayLayout.Override.CardAreaAppearance = appearance21;
            appearance22.BorderColor = System.Drawing.Color.Silver;
            appearance22.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.cboNotifications.DisplayLayout.Override.CellAppearance = appearance22;
            this.cboNotifications.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.cboNotifications.DisplayLayout.Override.CellPadding = 0;
            appearance23.BackColor = System.Drawing.SystemColors.Control;
            appearance23.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance23.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance23.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance23.BorderColor = System.Drawing.SystemColors.Window;
            this.cboNotifications.DisplayLayout.Override.GroupByRowAppearance = appearance23;
            appearance24.TextHAlignAsString = "Left";
            this.cboNotifications.DisplayLayout.Override.HeaderAppearance = appearance24;
            this.cboNotifications.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.cboNotifications.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance25.BackColor = System.Drawing.SystemColors.Window;
            appearance25.BorderColor = System.Drawing.Color.Silver;
            this.cboNotifications.DisplayLayout.Override.RowAppearance = appearance25;
            this.cboNotifications.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance26.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cboNotifications.DisplayLayout.Override.TemplateAddRowAppearance = appearance26;
            this.cboNotifications.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.cboNotifications.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.cboNotifications.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.cboNotifications.Location = new System.Drawing.Point(116, 2);
            this.cboNotifications.Name = "cboNotifications";
            this.cboNotifications.Size = new System.Drawing.Size(364, 23);
            this.cboNotifications.TabIndex = 40;
            ultraToolTipInfo9.ToolTipText = "The contact information for shipping notifications for the selected customer. Sel" +
    "ected contacts will receive notification emails at the email addresses listed.";
            ultraToolTipInfo9.ToolTipTitle = "Notifications";
            this.tipManager.SetUltraToolTip(this.cboNotifications, ultraToolTipInfo9);
            // 
            // ultraLabel10
            // 
            this.ultraLabel10.AutoSize = true;
            this.ultraLabel10.Location = new System.Drawing.Point(0, 4);
            this.ultraLabel10.Name = "ultraLabel10";
            this.ultraLabel10.Size = new System.Drawing.Size(80, 15);
            this.ultraLabel10.TabIndex = 29;
            this.ultraLabel10.Text = "Notifications:";
            // 
            // lblWeight
            // 
            this.lblWeight.AutoSize = true;
            this.lblWeight.Location = new System.Drawing.Point(0, 32);
            this.lblWeight.Name = "lblWeight";
            this.lblWeight.Size = new System.Drawing.Size(85, 15);
            this.lblWeight.TabIndex = 35;
            this.lblWeight.Text = "Gross Weight:";
            // 
            // dteShipDate
            // 
            this.dteShipDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dteShipDate.Location = new System.Drawing.Point(116, 56);
            this.dteShipDate.Name = "dteShipDate";
            this.dteShipDate.Size = new System.Drawing.Size(364, 22);
            this.dteShipDate.TabIndex = 10;
            ultraToolTipInfo10.ToolTipText = "The package ship date.";
            ultraToolTipInfo10.ToolTipTitle = "Ship Date";
            this.tipManager.SetUltraToolTip(this.dteShipDate, ultraToolTipInfo10);
            this.dteShipDate.ValueChanged += new System.EventHandler(this.dteShipDate_ValueChanged);
            // 
            // numWeight
            // 
            this.numWeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance27.TextHAlignAsString = "Left";
            this.numWeight.Appearance = appearance27;
            this.numWeight.Location = new System.Drawing.Point(116, 28);
            this.numWeight.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            this.numWeight.MaskInput = "nnnnnn.nn lbs";
            this.numWeight.MaxValue = new decimal(new int[] {
            1874919423,
            2328306,
            0,
            524288});
            this.numWeight.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numWeight.Name = "numWeight";
            this.numWeight.Nullable = true;
            this.numWeight.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.numWeight.Size = new System.Drawing.Size(364, 22);
            this.numWeight.TabIndex = 9;
            ultraToolTipInfo11.ToolTipText = "The gross weight of the shipment package.";
            ultraToolTipInfo11.ToolTipTitle = "Gross Weight";
            this.tipManager.SetUltraToolTip(this.numWeight, ultraToolTipInfo11);
            this.numWeight.Value = null;
            this.numWeight.ValueChanged += new System.EventHandler(this.numWeight_ValueChanged);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(1, 60);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(65, 15);
            this.ultraLabel4.TabIndex = 39;
            this.ultraLabel4.Text = "Ship Date:";
            // 
            // bsCustomerContacts
            // 
            this.bsCustomerContacts.AllowNew = false;
            this.bsCustomerContacts.Filter = "CustomerID = 0";
            this.bsCustomerContacts.Sort = "";
            // 
            // bsCustomerAddress
            // 
            this.bsCustomerAddress.AllowNew = false;
            this.bsCustomerAddress.Filter = "CustomerID = 0";
            // 
            // bsShippingCarrier
            // 
            this.bsShippingCarrier.AllowNew = false;
            this.bsShippingCarrier.Filter = "1 = 0";
            // 
            // shippingFlowManager
            // 
            this.shippingFlowManager.ContainerControl = this.grpShippingInfo;
            this.shippingFlowManager.HorizontalAlignment = Infragistics.Win.Layout.DefaultableFlowLayoutAlignment.Near;
            this.shippingFlowManager.HorizontalGap = 0;
            this.shippingFlowManager.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.shippingFlowManager.WrapItems = false;
            // 
            // ShipmentInfo
            // 
            this.Name = "ShipmentInfo";
            this.Size = new System.Drawing.Size(508, 564);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpOrders)).EndInit();
            this.grpOrders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsOrderShipments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpShippingInfo)).EndInit();
            this.grpShippingInfo.ResumeLayout(false);
            this.pnlShippingTop.ClientArea.ResumeLayout(false);
            this.pnlShippingTop.ClientArea.PerformLayout();
            this.pnlShippingTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboPackageType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCarrierNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPackageNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboShipTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboShippingCarrier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).EndInit();
            this.pnlShippingTrackingNumber.ClientArea.ResumeLayout(false);
            this.pnlShippingTrackingNumber.ClientArea.PerformLayout();
            this.pnlShippingTrackingNumber.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtTrackingNumber)).EndInit();
            this.pnlShippingBottom.ClientArea.ResumeLayout(false);
            this.pnlShippingBottom.ClientArea.PerformLayout();
            this.pnlShippingBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboNotifications)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteShipDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCustomerContacts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCustomerAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsShippingCarrier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shippingFlowManager)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraGroupBox grpOrders;
		private Infragistics.Win.UltraWinGrid.UltraGrid grdOrders;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
		private Infragistics.Win.Misc.UltraGroupBox grpShippingInfo;
		private Infragistics.Win.Misc.UltraLabel ultraLabel10;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboUser;
		private Infragistics.Win.Misc.UltraLabel ultraLabel5;
		private Infragistics.Win.Misc.UltraLabel ultraLabel9;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTrackingNumber;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboShippingCarrier;
		private Infragistics.Win.Misc.UltraLabel ultraLabel7;
		private Infragistics.Win.Misc.UltraLabel ultraLabel8;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCarrierNumber;
		private System.Windows.Forms.BindingSource bsOrderShipments;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomer;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private System.Windows.Forms.BindingSource bsCustomerContacts;
        private Infragistics.Win.Misc.UltraLabel lblWeight;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboShipTo;
        private System.Windows.Forms.BindingSource bsCustomerAddress;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numPackageNumber;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numWeight;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteShipDate;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboPackageType;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.Misc.UltraLabel lblMaximumNumber;
        private System.Windows.Forms.BindingSource bsShippingCarrier;
        private Infragistics.Win.Misc.UltraPanel pnlShippingTrackingNumber;
        private Infragistics.Win.Misc.UltraPanel pnlShippingBottom;
        private Infragistics.Win.Misc.UltraPanel pnlShippingTop;
        private Infragistics.Win.Misc.UltraFlowLayoutManager shippingFlowManager;
        private Infragistics.Win.UltraWinGrid.UltraCombo cboNotifications;
    }
}
