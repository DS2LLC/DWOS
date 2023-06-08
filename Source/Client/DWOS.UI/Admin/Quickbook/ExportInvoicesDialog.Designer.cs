namespace DWOS.UI.Admin
{
	partial class ExportInvoicesDialog
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
		    DisposeMe();

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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("OrderInvoice", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RequiredDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CompletedDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Priority");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PurchaseOrder", -1, null, 1, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CreatedBy");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ContractReviewed");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WorkStatus");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BasePrice");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PriceUnit");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerName", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartQuantity");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Invoice");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerWO");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShippingCarrier");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderInvoice_OrderFees");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderInvoice_OrderShipment");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("OrderInvoice_OrderFees", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderFeeID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderFeeTypeID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Charge");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand3 = new Infragistics.Win.UltraWinGrid.UltraGridBand("OrderInvoice_OrderShipment", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShipmentID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShippingUserID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DateShipped");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartQuantity");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn32 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShippingCarrierID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TrackingNumber");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn34 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CarrierCustomerNumber");
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
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel1 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel2 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel3 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("UpdateCustomFields");
            Infragistics.Win.UltraWinToolbars.PopupControlContainerTool popupControlContainerTool1 = new Infragistics.Win.UltraWinToolbars.PopupControlContainerTool("Settings2");
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab1 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbon1");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup1 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup1");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Export");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Print");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup2 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup2");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("InvoiceStatusReport");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Export");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Print");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupControlContainerTool popupControlContainerTool2 = new Infragistics.Win.UltraWinToolbars.PopupControlContainerTool("Settings2");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("InvoiceStatusReport");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("UpdateCustomFields");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportInvoicesDialog));
            this.grdExport = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsOrderInvoice = new DWOS.Data.Datasets.OrderInvoiceDataSet();
            this.statusBar = new Infragistics.Win.UltraWinStatusBar.UltraStatusBar();
            this.ExportToQuickbooks_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.exportInvoiceSettings = new DWOS.UI.Admin.Quickbook.ExportInvoiceSettings();
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraToolbarsManager1 = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.taOrderInvoice = new DWOS.Data.Datasets.OrderInvoiceDataSetTableAdapters.OrderInvoiceTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.grdExport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrderInvoice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBar)).BeginInit();
            this.ExportToQuickbooks_Fill_Panel.ClientArea.SuspendLayout();
            this.ExportToQuickbooks_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // grdExport
            // 
            this.grdExport.DataMember = "OrderInvoice";
            this.grdExport.DataSource = this.dsOrderInvoice;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdExport.DisplayLayout.Appearance = appearance1;
            this.grdExport.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.Caption = "WO";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 89;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn3.Header.Caption = "Order Date";
            ultraGridColumn3.Header.VisiblePosition = 5;
            ultraGridColumn3.Hidden = true;
            ultraGridColumn4.Header.VisiblePosition = 6;
            ultraGridColumn4.Hidden = true;
            ultraGridColumn5.Header.VisiblePosition = 7;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn6.Header.Caption = "Completed Date";
            ultraGridColumn6.Header.VisiblePosition = 8;
            ultraGridColumn6.Width = 164;
            ultraGridColumn7.Header.VisiblePosition = 9;
            ultraGridColumn7.Width = 157;
            ultraGridColumn8.Header.Caption = "PO";
            ultraGridColumn8.Header.VisiblePosition = 3;
            ultraGridColumn8.Width = 157;
            ultraGridColumn9.Header.VisiblePosition = 10;
            ultraGridColumn9.Hidden = true;
            ultraGridColumn10.Header.VisiblePosition = 11;
            ultraGridColumn10.Hidden = true;
            ultraGridColumn11.Header.VisiblePosition = 12;
            ultraGridColumn11.Hidden = true;
            ultraGridColumn12.Format = "$#,##0.00";
            ultraGridColumn12.Header.Caption = "Base Price";
            ultraGridColumn12.Header.VisiblePosition = 13;
            ultraGridColumn12.Width = 130;
            ultraGridColumn13.Header.Caption = "Price Unit";
            ultraGridColumn13.Header.VisiblePosition = 14;
            ultraGridColumn13.Width = 157;
            ultraGridColumn15.Header.Caption = "Customer";
            ultraGridColumn15.Header.VisiblePosition = 2;
            ultraGridColumn15.Width = 161;
            ultraGridColumn16.Header.Caption = "Part";
            ultraGridColumn16.Header.VisiblePosition = 15;
            ultraGridColumn16.Hidden = true;
            ultraGridColumn17.Header.Caption = "Quantity";
            ultraGridColumn17.Header.VisiblePosition = 16;
            ultraGridColumn17.Width = 103;
            ultraGridColumn18.Header.VisiblePosition = 17;
            ultraGridColumn18.Hidden = true;
            ultraGridColumn19.Header.Caption = "Customer WO";
            ultraGridColumn19.Header.VisiblePosition = 4;
            ultraGridColumn19.Width = 157;
            ultraGridColumn20.Header.Caption = "Shipping Carrier";
            ultraGridColumn20.Header.VisiblePosition = 18;
            ultraGridColumn20.Hidden = true;
            ultraGridColumn21.Header.VisiblePosition = 19;
            ultraGridColumn22.Header.VisiblePosition = 20;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20,
            ultraGridColumn21,
            ultraGridColumn22});
            ultraGridBand1.Header.Caption = "Orders";
            ultraGridColumn23.Header.VisiblePosition = 0;
            ultraGridColumn23.Hidden = true;
            ultraGridColumn24.Header.VisiblePosition = 1;
            ultraGridColumn24.Hidden = true;
            ultraGridColumn25.Header.Caption = "Fee Type";
            ultraGridColumn25.Header.VisiblePosition = 2;
            ultraGridColumn26.Format = "$#,##0.00";
            ultraGridColumn26.Header.VisiblePosition = 3;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn23,
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn26});
            ultraGridBand2.Hidden = true;
            ultraGridColumn27.Header.VisiblePosition = 0;
            ultraGridColumn27.Hidden = true;
            ultraGridColumn28.Header.VisiblePosition = 1;
            ultraGridColumn28.Hidden = true;
            ultraGridColumn29.Header.VisiblePosition = 2;
            ultraGridColumn29.Hidden = true;
            ultraGridColumn30.Header.Caption = "Date Shipped";
            ultraGridColumn30.Header.VisiblePosition = 3;
            ultraGridColumn31.Header.Caption = "Part Quantity";
            ultraGridColumn31.Header.VisiblePosition = 4;
            ultraGridColumn32.Header.VisiblePosition = 5;
            ultraGridColumn32.Hidden = true;
            ultraGridColumn33.Header.VisiblePosition = 6;
            ultraGridColumn34.Header.VisiblePosition = 7;
            ultraGridColumn34.Hidden = true;
            ultraGridBand3.Columns.AddRange(new object[] {
            ultraGridColumn27,
            ultraGridColumn28,
            ultraGridColumn29,
            ultraGridColumn30,
            ultraGridColumn31,
            ultraGridColumn32,
            ultraGridColumn33,
            ultraGridColumn34});
            ultraGridBand3.Hidden = true;
            this.grdExport.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdExport.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdExport.DisplayLayout.BandsSerializer.Add(ultraGridBand3);
            this.grdExport.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdExport.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.grdExport.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdExport.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.grdExport.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdExport.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.grdExport.DisplayLayout.MaxColScrollRegions = 1;
            this.grdExport.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdExport.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdExport.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grdExport.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdExport.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdExport.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.grdExport.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdExport.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.grdExport.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdExport.DisplayLayout.Override.CellAppearance = appearance8;
            this.grdExport.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.grdExport.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.grdExport.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Center";
            appearance10.TextVAlignAsString = "Middle";
            this.grdExport.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.grdExport.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdExport.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.grdExport.DisplayLayout.Override.RowAppearance = appearance11;
            this.grdExport.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdExport.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdExport.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.grdExport.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdExport.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdExport.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdExport.Location = new System.Drawing.Point(0, 0);
            this.grdExport.Name = "grdExport";
            this.grdExport.Size = new System.Drawing.Size(1296, 345);
            this.grdExport.TabIndex = 0;
            this.grdExport.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdExport_InitializeRow);
            // 
            // dsOrderInvoice
            // 
            this.dsOrderInvoice.DataSetName = "OrderInvoiceDataSet";
            this.dsOrderInvoice.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 520);
            this.statusBar.Name = "statusBar";
            ultraStatusPanel1.Style = Infragistics.Win.UltraWinStatusBar.PanelStyle.Progress;
            ultraStatusPanel1.Width = 250;
            ultraStatusPanel2.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Spring;
            ultraStatusPanel2.Text = "Exporting Invoices...";
            ultraStatusPanel3.Text = "XXX Invoices";
            this.statusBar.Panels.AddRange(new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel[] {
            ultraStatusPanel1,
            ultraStatusPanel2,
            ultraStatusPanel3});
            this.statusBar.Size = new System.Drawing.Size(1312, 23);
            this.statusBar.TabIndex = 13;
            // 
            // ExportToQuickbooks_Fill_Panel
            // 
            // 
            // ExportToQuickbooks_Fill_Panel.ClientArea
            // 
            this.ExportToQuickbooks_Fill_Panel.ClientArea.Controls.Add(this.grdExport);
            this.ExportToQuickbooks_Fill_Panel.ClientArea.Controls.Add(this.exportInvoiceSettings);
            this.ExportToQuickbooks_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.ExportToQuickbooks_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExportToQuickbooks_Fill_Panel.Location = new System.Drawing.Point(8, 175);
            this.ExportToQuickbooks_Fill_Panel.Name = "ExportToQuickbooks_Fill_Panel";
            this.ExportToQuickbooks_Fill_Panel.Size = new System.Drawing.Size(1296, 345);
            this.ExportToQuickbooks_Fill_Panel.TabIndex = 14;
            // 
            // exportInvoiceSettings
            // 
            this.exportInvoiceSettings.BackColor = System.Drawing.Color.Transparent;
            this.exportInvoiceSettings.Enabled = false;
            this.exportInvoiceSettings.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportInvoiceSettings.Location = new System.Drawing.Point(530, 38);
            this.exportInvoiceSettings.Name = "exportInvoiceSettings";
            this.exportInvoiceSettings.SettingTool = null;
            this.exportInvoiceSettings.Size = new System.Drawing.Size(737, 286);
            this.exportInvoiceSettings.TabIndex = 16;
            // 
            // _ExportToQuickbooks_Toolbars_Dock_Area_Left
            // 
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 175);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.Name = "_ExportToQuickbooks_Toolbars_Dock_Area_Left";
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(8, 345);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // ultraToolbarsManager1
            // 
            this.ultraToolbarsManager1.DesignerFlags = 1;
            this.ultraToolbarsManager1.DockWithinContainer = this;
            this.ultraToolbarsManager1.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
            this.ultraToolbarsManager1.Ribbon.ApplicationMenu.ToolAreaLeft.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool9});
            this.ultraToolbarsManager1.Ribbon.ApplicationMenu2010.NavigationMenu.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupControlContainerTool1});
            this.ultraToolbarsManager1.Ribbon.FileMenuStyle = Infragistics.Win.UltraWinToolbars.FileMenuStyle.ApplicationMenu2010;
            ribbonTab1.Caption = "Home";
            ribbonGroup1.Caption = "Tools";
            buttonTool1.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool3.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool3});
            ribbonGroup2.Caption = "Reports";
            buttonTool5.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool5});
            ribbonTab1.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup1,
            ribbonGroup2});
            this.ultraToolbarsManager1.Ribbon.NonInheritedRibbonTabs.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonTab[] {
            ribbonTab1});
            this.ultraToolbarsManager1.Ribbon.Visible = true;
            this.ultraToolbarsManager1.ShowFullMenusDelay = 500;
            this.ultraToolbarsManager1.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2010;
            appearance13.Image = ((object)(resources.GetObject("appearance13.Image")));
            buttonTool2.SharedPropsInternal.AppearancesLarge.Appearance = appearance13;
            buttonTool2.SharedPropsInternal.Caption = "Export";
            buttonTool2.SharedPropsInternal.ToolTipTextFormatted = "Export to Quickbooks";
            buttonTool2.SharedPropsInternal.ToolTipTitle = "Export";
            appearance14.Image = global::DWOS.UI.Properties.Resources.Print_32;
            buttonTool4.SharedPropsInternal.AppearancesLarge.Appearance = appearance14;
            buttonTool4.SharedPropsInternal.Caption = "Print";
            buttonTool4.SharedPropsInternal.ToolTipTextFormatted = "Print invoice information.";
            buttonTool4.SharedPropsInternal.ToolTipTitle = "Print";
            popupControlContainerTool2.ControlName = "exportInvoiceSettings";
            popupControlContainerTool2.DropDownArrowStyle = Infragistics.Win.UltraWinToolbars.DropDownArrowStyle.Standard;
            popupControlContainerTool2.SharedPropsInternal.Caption = "Settings";
            appearance15.Image = ((object)(resources.GetObject("appearance15.Image")));
            buttonTool6.SharedPropsInternal.AppearancesLarge.Appearance = appearance15;
            buttonTool6.SharedPropsInternal.Caption = "Invoice Compare";
            buttonTool6.SharedPropsInternal.ToolTipTextFormatted = "Displays the invoice comparisons.";
            buttonTool6.SharedPropsInternal.ToolTipTitle = "Invoice Compare";
            buttonTool8.SharedPropsInternal.Caption = "Update Custom Fields";
            this.ultraToolbarsManager1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool2,
            buttonTool4,
            popupControlContainerTool2,
            buttonTool6,
            buttonTool8});
            this.ultraToolbarsManager1.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.ultraToolbarsManager1_ToolClick);
            // 
            // _ExportToQuickbooks_Toolbars_Dock_Area_Right
            // 
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(1304, 175);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.Name = "_ExportToQuickbooks_Toolbars_Dock_Area_Right";
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(8, 345);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // _ExportToQuickbooks_Toolbars_Dock_Area_Top
            // 
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.Name = "_ExportToQuickbooks_Toolbars_Dock_Area_Top";
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(1312, 175);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // _ExportToQuickbooks_Toolbars_Dock_Area_Bottom
            // 
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 520);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.Name = "_ExportToQuickbooks_Toolbars_Dock_Area_Bottom";
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(1312, 0);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // taOrderInvoice
            // 
            this.taOrderInvoice.ClearBeforeFill = true;
            // 
            // ExportToQuickbooks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1312, 543);
            this.Controls.Add(this.ExportToQuickbooks_Fill_Panel);
            this.Controls.Add(this._ExportToQuickbooks_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._ExportToQuickbooks_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this._ExportToQuickbooks_Toolbars_Dock_Area_Top);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExportToQuickbooks";
            this.Text = "Export To Quickbooks";
            this.Load += new System.EventHandler(this.ExportToQuickbooks_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdExport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrderInvoice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBar)).EndInit();
            this.ExportToQuickbooks_Fill_Panel.ClientArea.ResumeLayout(false);
            this.ExportToQuickbooks_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid grdExport;
		private DWOS.Data.Datasets.OrderInvoiceDataSet dsOrderInvoice;
        private Infragistics.Win.UltraWinStatusBar.UltraStatusBar statusBar;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager ultraToolbarsManager1;
        private Infragistics.Win.Misc.UltraPanel ExportToQuickbooks_Fill_Panel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ExportToQuickbooks_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ExportToQuickbooks_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ExportToQuickbooks_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ExportToQuickbooks_Toolbars_Dock_Area_Top;
        private Quickbook.ExportInvoiceSettings exportInvoiceSettings;
        private Data.Datasets.OrderInvoiceDataSetTableAdapters.OrderInvoiceTableAdapter taOrderInvoice;
	}
}