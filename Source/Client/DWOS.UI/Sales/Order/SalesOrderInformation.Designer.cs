namespace DWOS.UI.Sales
{
	partial class SalesOrderInformation
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
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Customer Required Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the order was entered.", Infragistics.Win.ToolTipImage.Default, "Order Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The user who created the order.", Infragistics.Win.ToolTipImage.Default, "Entered By", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The name of the customer.", Infragistics.Win.ToolTipImage.Default, "Customer Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Invoice Number", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "PO Number", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Estimated Ship Date", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SalesOrderInformation));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The customer WO, if available.", Infragistics.Win.ToolTipImage.Default, "Customer WO", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo10 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Work Orders", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The adjusted estimated ship date for the orders.", Infragistics.Win.ToolTipImage.Default, "Adjusted Estimated Ship Date", Infragistics.Win.DefaultableBoolean.Default);
            this.dtOrderRequiredDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.dtOrderDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.cboUserCreated = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboCustomer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel15 = new Infragistics.Win.Misc.UltraLabel();
            this.txtInvoice = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtPONumber = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel14 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel13 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.txtSalesOrderID = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.dtShipDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.ultraLabel11 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel16 = new Infragistics.Win.Misc.UltraLabel();
            this.pnlGeneral = new Infragistics.Win.Misc.UltraPanel();
            this.pnlOrderRequiredDate = new Infragistics.Win.Misc.UltraPanel();
            this.txtCustomerWO = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel22 = new Infragistics.Win.Misc.UltraLabel();
            this.mediaWidget = new DWOS.UI.Utilities.MediaWidget();
            this.ultraFlowLayoutManager2 = new Infragistics.Win.Misc.UltraFlowLayoutManager(this.components);
            this.pnlWorkOrders = new Infragistics.Win.Misc.UltraPanel();
            this.grdOrders = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraLabel9 = new Infragistics.Win.Misc.UltraLabel();
            this.pnlSalesOrder = new Infragistics.Win.Misc.UltraPanel();
            this.pnlAdjustedShipDate = new Infragistics.Win.Misc.UltraPanel();
            this.dtAdjustedShipDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOrderRequiredDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOrderDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUserCreated)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInvoice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPONumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSalesOrderID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtShipDate)).BeginInit();
            this.pnlGeneral.ClientArea.SuspendLayout();
            this.pnlGeneral.SuspendLayout();
            this.pnlOrderRequiredDate.ClientArea.SuspendLayout();
            this.pnlOrderRequiredDate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerWO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFlowLayoutManager2)).BeginInit();
            this.pnlWorkOrders.ClientArea.SuspendLayout();
            this.pnlWorkOrders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdOrders)).BeginInit();
            this.pnlSalesOrder.ClientArea.SuspendLayout();
            this.pnlSalesOrder.SuspendLayout();
            this.pnlAdjustedShipDate.ClientArea.SuspendLayout();
            this.pnlAdjustedShipDate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtAdjustedShipDate)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.pnlGeneral);
            this.grpData.Controls.Add(this.pnlAdjustedShipDate);
            this.grpData.Controls.Add(this.pnlSalesOrder);
            this.grpData.Controls.Add(this.pnlWorkOrders);
            appearance5.Image = global::DWOS.UI.Properties.Resources.SalesOrder_16;
            this.grpData.HeaderAppearance = appearance5;
            this.grpData.Size = new System.Drawing.Size(469, 669);
            this.grpData.Text = "Sales Order";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.pnlWorkOrders, 0);
            this.grpData.Controls.SetChildIndex(this.pnlSalesOrder, 0);
            this.grpData.Controls.SetChildIndex(this.pnlAdjustedShipDate, 0);
            this.grpData.Controls.SetChildIndex(this.pnlGeneral, 0);
            // 
            // tipManager
            // 
            appearance1.FontData.Name = "Verdana";
            this.tipManager.Appearance = appearance1;
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(8, 656);
            // 
            // dtOrderRequiredDate
            // 
            this.dtOrderRequiredDate.DateTime = new System.DateTime(2014, 3, 4, 0, 0, 0, 0);
            this.dtOrderRequiredDate.Location = new System.Drawing.Point(79, 0);
            this.dtOrderRequiredDate.Name = "dtOrderRequiredDate";
            this.dtOrderRequiredDate.Size = new System.Drawing.Size(96, 22);
            this.dtOrderRequiredDate.TabIndex = 3;
            this.dtOrderRequiredDate.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextSection;
            ultraToolTipInfo3.ToolTipTextFormatted = "The date the order is required by the customer. Depending on shop work load, this" +
    " date may not be achievable.";
            ultraToolTipInfo3.ToolTipTitle = "Customer Required Date";
            this.tipManager.SetUltraToolTip(this.dtOrderRequiredDate, ultraToolTipInfo3);
            this.dtOrderRequiredDate.Value = new System.DateTime(2014, 3, 4, 0, 0, 0, 0);
            this.dtOrderRequiredDate.Validated += new System.EventHandler(this.dtOrderRequiredDate_Validated);
            // 
            // dtOrderDate
            // 
            this.dtOrderDate.DateTime = new System.DateTime(2014, 3, 4, 0, 0, 0, 0);
            this.dtOrderDate.Location = new System.Drawing.Point(326, 3);
            this.dtOrderDate.Name = "dtOrderDate";
            this.dtOrderDate.Size = new System.Drawing.Size(96, 22);
            this.dtOrderDate.TabIndex = 1;
            this.dtOrderDate.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextSection;
            ultraToolTipInfo2.ToolTipText = "The date the order was entered.";
            ultraToolTipInfo2.ToolTipTitle = "Order Date";
            this.tipManager.SetUltraToolTip(this.dtOrderDate, ultraToolTipInfo2);
            this.dtOrderDate.Value = new System.DateTime(2014, 3, 4, 0, 0, 0, 0);
            this.dtOrderDate.Validated += new System.EventHandler(this.dtOrderDate_Validated);
            // 
            // cboUserCreated
            // 
            this.cboUserCreated.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            this.cboUserCreated.DropDownListWidth = -1;
            this.cboUserCreated.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboUserCreated.Location = new System.Drawing.Point(128, 88);
            this.cboUserCreated.Name = "cboUserCreated";
            this.cboUserCreated.Nullable = false;
            this.cboUserCreated.ReadOnly = true;
            this.cboUserCreated.Size = new System.Drawing.Size(294, 22);
            this.cboUserCreated.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboUserCreated.TabIndex = 8;
            this.cboUserCreated.TabStop = false;
            ultraToolTipInfo7.ToolTipText = "The user who created the order.";
            ultraToolTipInfo7.ToolTipTitle = "Entered By";
            this.tipManager.SetUltraToolTip(this.cboUserCreated, ultraToolTipInfo7);
            // 
            // cboCustomer
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.Customer;
            this.cboCustomer.Appearance = appearance2;
            this.cboCustomer.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            this.cboCustomer.AutoSize = false;
            this.cboCustomer.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.StartsWith;
            this.cboCustomer.DropDownListWidth = -1;
            this.cboCustomer.Location = new System.Drawing.Point(128, 0);
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.Nullable = false;
            this.cboCustomer.NullText = "Select Customer";
            this.cboCustomer.Size = new System.Drawing.Size(294, 22);
            this.cboCustomer.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboCustomer.TabIndex = 4;
            ultraToolTipInfo6.ToolTipText = "The name of the customer.";
            ultraToolTipInfo6.ToolTipTitle = "Customer Name";
            this.tipManager.SetUltraToolTip(this.cboCustomer, ultraToolTipInfo6);
            this.cboCustomer.SelectionChanged += new System.EventHandler(this.cboCustomer_SelectionChanged);
            // 
            // ultraLabel15
            // 
            this.ultraLabel15.AutoSize = true;
            this.ultraLabel15.Location = new System.Drawing.Point(8, 60);
            this.ultraLabel15.Name = "ultraLabel15";
            this.ultraLabel15.Size = new System.Drawing.Size(51, 15);
            this.ultraLabel15.TabIndex = 71;
            this.ultraLabel15.Text = "Invoice:";
            // 
            // txtInvoice
            // 
            appearance3.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            editorButton1.Appearance = appearance3;
            editorButton1.Text = "";
            this.txtInvoice.ButtonsLeft.Add(editorButton1);
            this.txtInvoice.Location = new System.Drawing.Point(128, 56);
            this.txtInvoice.MaxLength = 50;
            this.txtInvoice.Name = "txtInvoice";
            this.txtInvoice.ReadOnly = true;
            this.txtInvoice.Size = new System.Drawing.Size(294, 22);
            this.txtInvoice.TabIndex = 7;
            this.txtInvoice.TabStop = false;
            ultraToolTipInfo8.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo8.ToolTipTextFormatted");
            ultraToolTipInfo8.ToolTipTitle = "Invoice Number";
            this.tipManager.SetUltraToolTip(this.txtInvoice, ultraToolTipInfo8);
            this.txtInvoice.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtInvoice_EditorButtonClick);
            this.txtInvoice.Leave += new System.EventHandler(this.txtInvoice_Leave);
            // 
            // txtPONumber
            // 
            this.txtPONumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtPONumber.Location = new System.Drawing.Point(327, 28);
            this.txtPONumber.MaxLength = 50;
            this.txtPONumber.Name = "txtPONumber";
            this.txtPONumber.Size = new System.Drawing.Size(95, 22);
            this.txtPONumber.TabIndex = 6;
            ultraToolTipInfo9.ToolTipTextFormatted = "The purchase order number received from the customer.<br/>";
            ultraToolTipInfo9.ToolTipTitle = "PO Number";
            this.tipManager.SetUltraToolTip(this.txtPONumber, ultraToolTipInfo9);
            this.txtPONumber.Leave += new System.EventHandler(this.txtPONumber_Leave);
            // 
            // ultraLabel14
            // 
            this.ultraLabel14.AutoSize = true;
            this.ultraLabel14.Location = new System.Drawing.Point(247, 32);
            this.ultraLabel14.Name = "ultraLabel14";
            this.ultraLabel14.Size = new System.Drawing.Size(74, 15);
            this.ultraLabel14.TabIndex = 68;
            this.ultraLabel14.Text = "PO Number:";
            // 
            // ultraLabel13
            // 
            this.ultraLabel13.AutoSize = true;
            this.ultraLabel13.Location = new System.Drawing.Point(7, 92);
            this.ultraLabel13.Name = "ultraLabel13";
            this.ultraLabel13.Size = new System.Drawing.Size(72, 15);
            this.ultraLabel13.TabIndex = 64;
            this.ultraLabel13.Text = "Entered By:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(7, 3);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(64, 15);
            this.ultraLabel3.TabIndex = 46;
            this.ultraLabel3.Text = "Customer:";
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(0, 4);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(66, 15);
            this.ultraLabel7.TabIndex = 51;
            this.ultraLabel7.Text = "Req. Date:";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(247, 7);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(72, 15);
            this.ultraLabel5.TabIndex = 47;
            this.ultraLabel5.Text = "Order Date:";
            // 
            // txtSalesOrderID
            // 
            this.txtSalesOrderID.Location = new System.Drawing.Point(128, 3);
            this.txtSalesOrderID.Name = "txtSalesOrderID";
            this.txtSalesOrderID.ReadOnly = true;
            this.txtSalesOrderID.Size = new System.Drawing.Size(111, 22);
            this.txtSalesOrderID.TabIndex = 0;
            this.txtSalesOrderID.TabStop = false;
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(6, 7);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(76, 15);
            this.ultraLabel6.TabIndex = 44;
            this.ultraLabel6.Text = "Sales Order:";
            // 
            // dtShipDate
            // 
            this.dtShipDate.DateTime = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtShipDate.Location = new System.Drawing.Point(128, 31);
            this.dtShipDate.Name = "dtShipDate";
            this.dtShipDate.Size = new System.Drawing.Size(111, 22);
            this.dtShipDate.TabIndex = 2;
            this.dtShipDate.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextSection;
            ultraToolTipInfo1.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo1.ToolTipTextFormatted");
            ultraToolTipInfo1.ToolTipTitle = "Estimated Ship Date";
            this.tipManager.SetUltraToolTip(this.dtShipDate, ultraToolTipInfo1);
            this.dtShipDate.Value = null;
            this.dtShipDate.Validated += new System.EventHandler(this.dtShipDate_Validated);
            // 
            // ultraLabel11
            // 
            this.ultraLabel11.AutoSize = true;
            this.ultraLabel11.Location = new System.Drawing.Point(7, 35);
            this.ultraLabel11.Name = "ultraLabel11";
            this.ultraLabel11.Size = new System.Drawing.Size(60, 15);
            this.ultraLabel11.TabIndex = 81;
            this.ultraLabel11.Text = "Est. Ship:";
            // 
            // ultraLabel16
            // 
            this.ultraLabel16.AutoSize = true;
            this.ultraLabel16.Location = new System.Drawing.Point(6, 32);
            this.ultraLabel16.Name = "ultraLabel16";
            this.ultraLabel16.Size = new System.Drawing.Size(88, 15);
            this.ultraLabel16.TabIndex = 85;
            this.ultraLabel16.Text = "Customer WO:";
            // 
            // pnlGeneral
            // 
            this.pnlGeneral.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            // 
            // pnlGeneral.ClientArea
            // 
            this.pnlGeneral.ClientArea.Controls.Add(this.ultraLabel6);
            this.pnlGeneral.ClientArea.Controls.Add(this.ultraLabel5);
            this.pnlGeneral.ClientArea.Controls.Add(this.dtShipDate);
            this.pnlGeneral.ClientArea.Controls.Add(this.ultraLabel11);
            this.pnlGeneral.ClientArea.Controls.Add(this.txtSalesOrderID);
            this.pnlGeneral.ClientArea.Controls.Add(this.dtOrderDate);
            this.pnlGeneral.ClientArea.Controls.Add(this.pnlOrderRequiredDate);
            this.pnlGeneral.Location = new System.Drawing.Point(8, 23);
            this.pnlGeneral.Name = "pnlGeneral";
            this.pnlGeneral.Size = new System.Drawing.Size(450, 58);
            this.pnlGeneral.TabIndex = 0;
            this.pnlGeneral.UseAppStyling = false;
            // 
            // pnlOrderRequiredDate
            // 
            // 
            // pnlOrderRequiredDate.ClientArea
            // 
            this.pnlOrderRequiredDate.ClientArea.Controls.Add(this.dtOrderRequiredDate);
            this.pnlOrderRequiredDate.ClientArea.Controls.Add(this.ultraLabel7);
            this.pnlOrderRequiredDate.Location = new System.Drawing.Point(247, 31);
            this.pnlOrderRequiredDate.Name = "pnlOrderRequiredDate";
            this.pnlOrderRequiredDate.Size = new System.Drawing.Size(175, 22);
            this.pnlOrderRequiredDate.TabIndex = 3;
            // 
            // txtCustomerWO
            // 
            this.txtCustomerWO.Location = new System.Drawing.Point(128, 28);
            this.txtCustomerWO.Name = "txtCustomerWO";
            this.txtCustomerWO.Size = new System.Drawing.Size(114, 22);
            this.txtCustomerWO.TabIndex = 5;
            ultraToolTipInfo5.ToolTipText = "The customer WO, if available.";
            ultraToolTipInfo5.ToolTipTitle = "Customer WO";
            this.tipManager.SetUltraToolTip(this.txtCustomerWO, ultraToolTipInfo5);
            this.txtCustomerWO.Leave += new System.EventHandler(this.txtCustomerWO_Leave);
            // 
            // ultraLabel22
            // 
            this.ultraLabel22.AutoSize = true;
            this.ultraLabel22.Location = new System.Drawing.Point(8, 121);
            this.ultraLabel22.Name = "ultraLabel22";
            this.ultraLabel22.Size = new System.Drawing.Size(73, 15);
            this.ultraLabel22.TabIndex = 174;
            this.ultraLabel22.Text = "Documents:";
            // 
            // mediaWidget
            // 
            this.mediaWidget.AllowEditing = false;
            this.mediaWidget.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mediaWidget.Location = new System.Drawing.Point(128, 116);
            this.mediaWidget.Name = "mediaWidget";
            this.mediaWidget.Size = new System.Drawing.Size(294, 105);
            this.mediaWidget.TabIndex = 9;
            // 
            // ultraFlowLayoutManager2
            // 
            this.ultraFlowLayoutManager2.ContainerControl = this.grpData;
            this.ultraFlowLayoutManager2.HorizontalAlignment = Infragistics.Win.Layout.DefaultableFlowLayoutAlignment.Near;
            this.ultraFlowLayoutManager2.HorizontalGap = 0;
            this.ultraFlowLayoutManager2.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.ultraFlowLayoutManager2.VerticalAlignment = Infragistics.Win.Layout.DefaultableFlowLayoutAlignment.Near;
            this.ultraFlowLayoutManager2.VerticalGap = 0;
            this.ultraFlowLayoutManager2.WrapItems = false;
            // 
            // pnlWorkOrders
            // 
            this.pnlWorkOrders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            appearance4.BackColor = System.Drawing.Color.Transparent;
            this.pnlWorkOrders.Appearance = appearance4;
            this.pnlWorkOrders.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            // 
            // pnlWorkOrders.ClientArea
            // 
            this.pnlWorkOrders.ClientArea.Controls.Add(this.grdOrders);
            this.pnlWorkOrders.ClientArea.Controls.Add(this.ultraLabel9);
            this.pnlWorkOrders.Location = new System.Drawing.Point(8, 341);
            this.pnlWorkOrders.Name = "pnlWorkOrders";
            this.pnlWorkOrders.Size = new System.Drawing.Size(450, 315);
            this.pnlWorkOrders.TabIndex = 3;
            this.pnlWorkOrders.UseAppStyling = false;
            // 
            // grdOrders
            // 
            this.grdOrders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdOrders.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            this.grdOrders.Location = new System.Drawing.Point(7, 31);
            this.grdOrders.Name = "grdOrders";
            this.grdOrders.Size = new System.Drawing.Size(433, 278);
            this.grdOrders.TabIndex = 80;
            this.grdOrders.Text = "Orders";
            ultraToolTipInfo10.ToolTipTextFormatted = "The work orders assigned to the sales order.";
            ultraToolTipInfo10.ToolTipTitle = "Work Orders";
            this.tipManager.SetUltraToolTip(this.grdOrders, ultraToolTipInfo10);
            this.grdOrders.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdOrders_InitializeLayout);
            this.grdOrders.AfterColPosChanged += new Infragistics.Win.UltraWinGrid.AfterColPosChangedEventHandler(this.grdOrders_AfterColPosChanged);
            // 
            // ultraLabel9
            // 
            this.ultraLabel9.AutoSize = true;
            this.ultraLabel9.Location = new System.Drawing.Point(7, 10);
            this.ultraLabel9.Name = "ultraLabel9";
            this.ultraLabel9.Size = new System.Drawing.Size(81, 15);
            this.ultraLabel9.TabIndex = 79;
            this.ultraLabel9.Text = "Work Orders:";
            // 
            // pnlSalesOrder
            // 
            this.pnlSalesOrder.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            // 
            // pnlSalesOrder.ClientArea
            // 
            this.pnlSalesOrder.ClientArea.Controls.Add(this.txtCustomerWO);
            this.pnlSalesOrder.ClientArea.Controls.Add(this.cboCustomer);
            this.pnlSalesOrder.ClientArea.Controls.Add(this.ultraLabel22);
            this.pnlSalesOrder.ClientArea.Controls.Add(this.cboUserCreated);
            this.pnlSalesOrder.ClientArea.Controls.Add(this.mediaWidget);
            this.pnlSalesOrder.ClientArea.Controls.Add(this.ultraLabel15);
            this.pnlSalesOrder.ClientArea.Controls.Add(this.txtInvoice);
            this.pnlSalesOrder.ClientArea.Controls.Add(this.txtPONumber);
            this.pnlSalesOrder.ClientArea.Controls.Add(this.ultraLabel16);
            this.pnlSalesOrder.ClientArea.Controls.Add(this.ultraLabel14);
            this.pnlSalesOrder.ClientArea.Controls.Add(this.ultraLabel3);
            this.pnlSalesOrder.ClientArea.Controls.Add(this.ultraLabel13);
            this.pnlSalesOrder.Location = new System.Drawing.Point(8, 112);
            this.pnlSalesOrder.Name = "pnlSalesOrder";
            this.pnlSalesOrder.Size = new System.Drawing.Size(450, 229);
            this.pnlSalesOrder.TabIndex = 2;
            // 
            // pnlAdjustedShipDate
            // 
            // 
            // pnlAdjustedShipDate.ClientArea
            // 
            this.pnlAdjustedShipDate.ClientArea.Controls.Add(this.dtAdjustedShipDate);
            this.pnlAdjustedShipDate.ClientArea.Controls.Add(this.ultraLabel1);
            this.pnlAdjustedShipDate.Location = new System.Drawing.Point(8, 81);
            this.pnlAdjustedShipDate.Name = "pnlAdjustedShipDate";
            this.pnlAdjustedShipDate.Size = new System.Drawing.Size(450, 31);
            this.pnlAdjustedShipDate.TabIndex = 1;
            // 
            // dtAdjustedShipDate
            // 
            this.dtAdjustedShipDate.DateTime = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtAdjustedShipDate.Location = new System.Drawing.Point(128, 3);
            this.dtAdjustedShipDate.Name = "dtAdjustedShipDate";
            this.dtAdjustedShipDate.Size = new System.Drawing.Size(294, 22);
            this.dtAdjustedShipDate.TabIndex = 83;
            this.dtAdjustedShipDate.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextSection;
            ultraToolTipInfo4.ToolTipText = "The adjusted estimated ship date for the orders.";
            ultraToolTipInfo4.ToolTipTitle = "Adjusted Estimated Ship Date";
            this.tipManager.SetUltraToolTip(this.dtAdjustedShipDate, ultraToolTipInfo4);
            this.dtAdjustedShipDate.Value = null;
            this.dtAdjustedShipDate.Validated += new System.EventHandler(this.dtAdjustedShipDate_Validated);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(8, 7);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(114, 15);
            this.ultraLabel1.TabIndex = 82;
            this.ultraLabel1.Text = "Adjusted Est. Ship:";
            // 
            // SalesOrderInformation
            // 
            this.MinimumSize = new System.Drawing.Size(475, 675);
            this.Name = "SalesOrderInformation";
            this.Size = new System.Drawing.Size(475, 675);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOrderRequiredDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOrderDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUserCreated)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInvoice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPONumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSalesOrderID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtShipDate)).EndInit();
            this.pnlGeneral.ClientArea.ResumeLayout(false);
            this.pnlGeneral.ClientArea.PerformLayout();
            this.pnlGeneral.ResumeLayout(false);
            this.pnlOrderRequiredDate.ClientArea.ResumeLayout(false);
            this.pnlOrderRequiredDate.ClientArea.PerformLayout();
            this.pnlOrderRequiredDate.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerWO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFlowLayoutManager2)).EndInit();
            this.pnlWorkOrders.ClientArea.ResumeLayout(false);
            this.pnlWorkOrders.ClientArea.PerformLayout();
            this.pnlWorkOrders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdOrders)).EndInit();
            this.pnlSalesOrder.ClientArea.ResumeLayout(false);
            this.pnlSalesOrder.ClientArea.PerformLayout();
            this.pnlSalesOrder.ResumeLayout(false);
            this.pnlAdjustedShipDate.ClientArea.ResumeLayout(false);
            this.pnlAdjustedShipDate.ClientArea.PerformLayout();
            this.pnlAdjustedShipDate.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtAdjustedShipDate)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtOrderRequiredDate;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtOrderDate;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboUserCreated;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomer;
		private Infragistics.Win.Misc.UltraLabel ultraLabel15;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtInvoice;
		public Infragistics.Win.UltraWinEditors.UltraTextEditor txtPONumber;
		private Infragistics.Win.Misc.UltraLabel ultraLabel14;
        private Infragistics.Win.Misc.UltraLabel ultraLabel13;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
		private Infragistics.Win.Misc.UltraLabel ultraLabel7;
		private Infragistics.Win.Misc.UltraLabel ultraLabel5;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSalesOrderID;
		private Infragistics.Win.Misc.UltraLabel ultraLabel6;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtShipDate;
        private Infragistics.Win.Misc.UltraLabel ultraLabel11;
        private Infragistics.Win.Misc.UltraLabel ultraLabel16;
		private Infragistics.Win.Misc.UltraPanel pnlGeneral;
		private Infragistics.Win.Misc.UltraFlowLayoutManager ultraFlowLayoutManager2;
        private Infragistics.Win.Misc.UltraPanel pnlWorkOrders;
        private Infragistics.Win.Misc.UltraLabel ultraLabel9;
        private Infragistics.Win.Misc.UltraLabel ultraLabel22;
        private Utilities.MediaWidget mediaWidget;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdOrders;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomerWO;
        private Infragistics.Win.Misc.UltraPanel pnlOrderRequiredDate;
        private Infragistics.Win.Misc.UltraPanel pnlSalesOrder;
        private Infragistics.Win.Misc.UltraPanel pnlAdjustedShipDate;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtAdjustedShipDate;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
    }
}
