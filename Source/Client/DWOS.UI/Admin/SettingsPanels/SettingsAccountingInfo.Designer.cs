namespace DWOS.UI.Admin.SettingsPanels
{
    partial class SettingsAccountingInfo
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
			_displayTooltipsMain.Dispose();
			_displayTooltipsType.Dispose();
			_displayTooltipsPart.Dispose();
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("When checked, SO invoices will be indexed via auto-incrementation during every ex" +
        "port event. For example, SO-1 would export as SO-1 the first time, then SO-2 the" +
        " next, etc.", Infragistics.Win.ToolTipImage.Default, "Index SO Invoices", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Invoice Type", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsAccountingInfo));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Accounting Invoice Export Level", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Check Total Price", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Price Decimals", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The part invoice item code to use for parts in the exported invoice.", Infragistics.Win.ToolTipImage.Default, "Part Invoice Item Code", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The part invoice item name to use for parts in the exported invoice.", Infragistics.Win.ToolTipImage.Default, "Part Invoice Item Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Opens an editor for the Description column template.", Infragistics.Win.ToolTipImage.Default, "Edit QuickBooks Description Template", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Include the header row in output.", Infragistics.Win.ToolTipImage.Default, "Header Row", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo10 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Calculate Unit Price for Lot during export.", Infragistics.Win.ToolTipImage.Default, "Calculate Unit Price for Lot", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo11 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("QuickBooks transaction class", Infragistics.Win.ToolTipImage.Default, "Transaction Class", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo12 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("This field assures that DWOS can connect with QuickBooks for the Export.", Infragistics.Win.ToolTipImage.Default, "Connection", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo13 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "SYSPRO", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo14 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The list of field tokens used for exporting invoices to CSV format.", Infragistics.Win.ToolTipImage.Default, "Field Tokens", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo15 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Rounding Type", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo16 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Select to Export Invoice in a .CSV format", Infragistics.Win.ToolTipImage.Default, "CSV", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo17 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Selecting will tell DWOS to Export Invoice to QuickBooks.", Infragistics.Win.ToolTipImage.Default, "QuickBooks", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo18 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Set the maximum number of Invoices to export at one time.", Infragistics.Win.ToolTipImage.Default, "Maximum Export", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo19 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Attach a prefix to a package invoice in QuickBooks.", Infragistics.Win.ToolTipImage.Default, "Package Invoice Prefix", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo20 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Attach a prefix to a work order invoice in QuickBooks.", Infragistics.Win.ToolTipImage.Default, "Work Order Invoice Prefix", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo21 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Attach a prefix to a sales order invoice in QuickBooks.", Infragistics.Win.ToolTipImage.Default, "Sales Order Invoice Prefix", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo22 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Map the invoice header item to an order Customer WO.", Infragistics.Win.ToolTipImage.Default, "Invoice Header Item", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo23 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Determines which value, if any, should be placed in the \'Other 2\' column of an in" +
        "voice in QuickBooks.", Infragistics.Win.ToolTipImage.Default, "Invoice Column \'Other 2\'", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo24 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Map the invoice header item to an order Tracking #.", Infragistics.Win.ToolTipImage.Default, "Invoice Header Item", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo25 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Determines which value, if any, should be placed in the \'Other 1\' column of an in" +
        "voice in QuickBooks.", Infragistics.Win.ToolTipImage.Default, "Invoice Column \'Other 1\'", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo26 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Export process will stop when maximum number of errors have been encountered.", Infragistics.Win.ToolTipImage.Default, "Maximum Errors", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.chkIndexSOInvoices = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cboInvoiceType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboInvoiceLevel = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.chkCheckTotalPrice = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel15 = new Infragistics.Win.Misc.UltraLabel();
            this.numPriceDecimals = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel11 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraGroupBox3 = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtExportPartItemCode = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel10 = new Infragistics.Win.Misc.UltraLabel();
            this.txtExportPartItemName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnQuickBooksTemplate = new Infragistics.Win.Misc.UltraButton();
            this.chkHeaders = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkCalcUnitPrice = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.txtTransactionClass = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtConnectionName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.chkExportSyspro = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.txtExportTokens = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboQuickbooksRounding = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel17 = new Infragistics.Win.Misc.UltraLabel();
            this.btnSyspro = new Infragistics.Win.Misc.UltraButton();
            this.chkExportCSV = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkExportQuickbooks = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.FormLabel = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel9 = new Infragistics.Win.Misc.UltraLabel();
            this.pnlGeneralSettings = new Infragistics.Win.Misc.UltraPanel();
            this.ultraLabel16 = new Infragistics.Win.Misc.UltraLabel();
            this.numMaxExport = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtPackagePrefix = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.txtWOPrefix = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtSOPrefix = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtCustomerWO = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel14 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.cboOther2 = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtTrackingNumber = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboOther1 = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel13 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel12 = new Infragistics.Win.Misc.UltraLabel();
            this.numMaxErrors = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkIndexSOInvoices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboInvoiceType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboInvoiceLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCheckTotalPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPriceDecimals)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox3)).BeginInit();
            this.ultraGroupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtExportPartItemCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExportPartItemName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkHeaders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCalcUnitPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransactionClass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConnectionName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkExportSyspro)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExportTokens)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboQuickbooksRounding)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkExportCSV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkExportQuickbooks)).BeginInit();
            this.pnlGeneralSettings.ClientArea.SuspendLayout();
            this.pnlGeneralSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxExport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPackagePrefix)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWOPrefix)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSOPrefix)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerWO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOther2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTrackingNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOther1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxErrors)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox1.Controls.Add(this.chkIndexSOInvoices);
            this.ultraGroupBox1.Controls.Add(this.cboInvoiceType);
            this.ultraGroupBox1.Controls.Add(this.cboInvoiceLevel);
            this.ultraGroupBox1.Controls.Add(this.chkCheckTotalPrice);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel15);
            this.ultraGroupBox1.Controls.Add(this.numPriceDecimals);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel11);
            this.ultraGroupBox1.Controls.Add(this.ultraGroupBox3);
            this.ultraGroupBox1.Controls.Add(this.ultraGroupBox2);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel9);
            this.ultraGroupBox1.Controls.Add(this.pnlGeneralSettings);
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(545, 896);
            this.ultraGroupBox1.TabIndex = 39;
            this.ultraGroupBox1.Text = "Accounting";
            // 
            // chkIndexSOInvoices
            // 
            this.chkIndexSOInvoices.Location = new System.Drawing.Point(30, 55);
            this.chkIndexSOInvoices.Name = "chkIndexSOInvoices";
            this.chkIndexSOInvoices.Size = new System.Drawing.Size(181, 20);
            this.chkIndexSOInvoices.TabIndex = 100;
            this.chkIndexSOInvoices.Text = "Index SO Invoices";
            ultraToolTipInfo1.ToolTipText = "When checked, SO invoices will be indexed via auto-incrementation during every ex" +
    "port event. For example, SO-1 would export as SO-1 the first time, then SO-2 the" +
    " next, etc.";
            ultraToolTipInfo1.ToolTipTitle = "Index SO Invoices";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkIndexSOInvoices, ultraToolTipInfo1);
            // 
            // cboInvoiceType
            // 
            this.cboInvoiceType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboInvoiceType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboInvoiceType.Location = new System.Drawing.Point(172, 84);
            this.cboInvoiceType.Name = "cboInvoiceType";
            this.cboInvoiceType.Size = new System.Drawing.Size(366, 22);
            this.cboInvoiceType.TabIndex = 1;
            ultraToolTipInfo2.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo2.ToolTipTextFormatted");
            ultraToolTipInfo2.ToolTipTitle = "Invoice Type";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboInvoiceType, ultraToolTipInfo2);
            // 
            // cboInvoiceLevel
            // 
            this.cboInvoiceLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboInvoiceLevel.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboInvoiceLevel.Location = new System.Drawing.Point(172, 27);
            this.cboInvoiceLevel.Name = "cboInvoiceLevel";
            this.cboInvoiceLevel.Size = new System.Drawing.Size(366, 22);
            this.cboInvoiceLevel.TabIndex = 0;
            ultraToolTipInfo3.ToolTipTextFormatted = "Set the default accounting invoice level for exporting invoices: SalesOrder or Wo" +
    "rkOrder.";
            ultraToolTipInfo3.ToolTipTitle = "Accounting Invoice Export Level";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboInvoiceLevel, ultraToolTipInfo3);
            // 
            // chkCheckTotalPrice
            // 
            this.chkCheckTotalPrice.AutoSize = true;
            this.chkCheckTotalPrice.Location = new System.Drawing.Point(30, 153);
            this.chkCheckTotalPrice.Name = "chkCheckTotalPrice";
            this.chkCheckTotalPrice.Size = new System.Drawing.Size(239, 18);
            this.chkCheckTotalPrice.TabIndex = 3;
            this.chkCheckTotalPrice.Text = "Check total price instead of base price";
            ultraToolTipInfo4.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo4.ToolTipTextFormatted");
            ultraToolTipInfo4.ToolTipTitle = "Check Total Price";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkCheckTotalPrice, ultraToolTipInfo4);
            // 
            // ultraLabel15
            // 
            this.ultraLabel15.AutoSize = true;
            this.ultraLabel15.Location = new System.Drawing.Point(8, 88);
            this.ultraLabel15.Name = "ultraLabel15";
            this.ultraLabel15.Size = new System.Drawing.Size(82, 15);
            this.ultraLabel15.TabIndex = 99;
            this.ultraLabel15.Text = "Invoice Type:";
            // 
            // numPriceDecimals
            // 
            this.numPriceDecimals.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numPriceDecimals.Location = new System.Drawing.Point(172, 112);
            this.numPriceDecimals.MaxValue = 5;
            this.numPriceDecimals.MinValue = 1;
            this.numPriceDecimals.Name = "numPriceDecimals";
            this.numPriceDecimals.Size = new System.Drawing.Size(367, 22);
            this.numPriceDecimals.TabIndex = 2;
            ultraToolTipInfo5.ToolTipTextFormatted = "Number of decimal places to use for prices [1 - 5].";
            ultraToolTipInfo5.ToolTipTitle = "Price Decimals";
            this.ultraToolTipManager1.SetUltraToolTip(this.numPriceDecimals, ultraToolTipInfo5);
            // 
            // ultraLabel11
            // 
            this.ultraLabel11.AutoSize = true;
            this.ultraLabel11.Location = new System.Drawing.Point(8, 116);
            this.ultraLabel11.Name = "ultraLabel11";
            this.ultraLabel11.Size = new System.Drawing.Size(92, 15);
            this.ultraLabel11.TabIndex = 94;
            this.ultraLabel11.Text = "Price Decimals:";
            // 
            // ultraGroupBox3
            // 
            this.ultraGroupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox3.Controls.Add(this.txtExportPartItemCode);
            this.ultraGroupBox3.Controls.Add(this.ultraLabel10);
            this.ultraGroupBox3.Controls.Add(this.txtExportPartItemName);
            this.ultraGroupBox3.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox3.Location = new System.Drawing.Point(10, 506);
            this.ultraGroupBox3.Name = "ultraGroupBox3";
            this.ultraGroupBox3.Size = new System.Drawing.Size(535, 89);
            this.ultraGroupBox3.TabIndex = 5;
            this.ultraGroupBox3.Text = "Part Invoice Item";
            // 
            // txtExportPartItemCode
            // 
            this.txtExportPartItemCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExportPartItemCode.Location = new System.Drawing.Point(162, 55);
            this.txtExportPartItemCode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtExportPartItemCode.MaxLength = 255;
            this.txtExportPartItemCode.Name = "txtExportPartItemCode";
            this.txtExportPartItemCode.NullText = "Part Invoice Item Code";
            appearance1.FontData.ItalicAsString = "True";
            appearance1.ForeColor = System.Drawing.Color.Silver;
            this.txtExportPartItemCode.NullTextAppearance = appearance1;
            this.txtExportPartItemCode.Size = new System.Drawing.Size(366, 22);
            this.txtExportPartItemCode.TabIndex = 10;
            ultraToolTipInfo6.ToolTipText = "The part invoice item code to use for parts in the exported invoice.";
            ultraToolTipInfo6.ToolTipTitle = "Part Invoice Item Code";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtExportPartItemCode, ultraToolTipInfo6);
            // 
            // ultraLabel10
            // 
            this.ultraLabel10.AutoSize = true;
            this.ultraLabel10.Location = new System.Drawing.Point(20, 59);
            this.ultraLabel10.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel10.Name = "ultraLabel10";
            this.ultraLabel10.Size = new System.Drawing.Size(38, 15);
            this.ultraLabel10.TabIndex = 91;
            this.ultraLabel10.Text = "Code:";
            // 
            // txtExportPartItemName
            // 
            this.txtExportPartItemName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExportPartItemName.Location = new System.Drawing.Point(162, 27);
            this.txtExportPartItemName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtExportPartItemName.MaxLength = 255;
            this.txtExportPartItemName.Name = "txtExportPartItemName";
            this.txtExportPartItemName.NullText = "Part Invoice Item Name";
            appearance2.FontData.ItalicAsString = "True";
            appearance2.ForeColor = System.Drawing.Color.Silver;
            this.txtExportPartItemName.NullTextAppearance = appearance2;
            this.txtExportPartItemName.Size = new System.Drawing.Size(366, 22);
            this.txtExportPartItemName.TabIndex = 9;
            ultraToolTipInfo7.ToolTipText = "The part invoice item name to use for parts in the exported invoice.";
            ultraToolTipInfo7.ToolTipTitle = "Part Invoice Item Name";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtExportPartItemName, ultraToolTipInfo7);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(20, 31);
            this.ultraLabel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel2.TabIndex = 68;
            this.ultraLabel2.Text = "Name:";
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox2.Controls.Add(this.btnQuickBooksTemplate);
            this.ultraGroupBox2.Controls.Add(this.chkHeaders);
            this.ultraGroupBox2.Controls.Add(this.chkCalcUnitPrice);
            this.ultraGroupBox2.Controls.Add(this.txtTransactionClass);
            this.ultraGroupBox2.Controls.Add(this.txtConnectionName);
            this.ultraGroupBox2.Controls.Add(this.chkExportSyspro);
            this.ultraGroupBox2.Controls.Add(this.txtExportTokens);
            this.ultraGroupBox2.Controls.Add(this.cboQuickbooksRounding);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel17);
            this.ultraGroupBox2.Controls.Add(this.btnSyspro);
            this.ultraGroupBox2.Controls.Add(this.chkExportCSV);
            this.ultraGroupBox2.Controls.Add(this.chkExportQuickbooks);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel6);
            this.ultraGroupBox2.Controls.Add(this.FormLabel);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel5);
            this.ultraGroupBox2.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ultraGroupBox2.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox2.Location = new System.Drawing.Point(10, 177);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(535, 323);
            this.ultraGroupBox2.TabIndex = 4;
            this.ultraGroupBox2.Text = "Account Export Type";
            // 
            // btnQuickBooksTemplate
            // 
            this.btnQuickBooksTemplate.Location = new System.Drawing.Point(41, 245);
            this.btnQuickBooksTemplate.Name = "btnQuickBooksTemplate";
            this.btnQuickBooksTemplate.Size = new System.Drawing.Size(201, 23);
            this.btnQuickBooksTemplate.TabIndex = 10;
            this.btnQuickBooksTemplate.Text = "Edit Description Template...";
            ultraToolTipInfo8.ToolTipText = "Opens an editor for the Description column template.";
            ultraToolTipInfo8.ToolTipTitle = "Edit QuickBooks Description Template";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnQuickBooksTemplate, ultraToolTipInfo8);
            this.btnQuickBooksTemplate.Click += new System.EventHandler(this.btnQuickBooksTemplate_Click);
            // 
            // chkHeaders
            // 
            this.chkHeaders.AutoSize = true;
            this.chkHeaders.Location = new System.Drawing.Point(41, 109);
            this.chkHeaders.Name = "chkHeaders";
            this.chkHeaders.Size = new System.Drawing.Size(136, 18);
            this.chkHeaders.TabIndex = 5;
            this.chkHeaders.Text = "Include Header Row";
            ultraToolTipInfo9.ToolTipText = "Include the header row in output.";
            ultraToolTipInfo9.ToolTipTitle = "Header Row";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkHeaders, ultraToolTipInfo9);
            // 
            // chkCalcUnitPrice
            // 
            this.chkCalcUnitPrice.AutoSize = true;
            this.chkCalcUnitPrice.Location = new System.Drawing.Point(41, 85);
            this.chkCalcUnitPrice.Name = "chkCalcUnitPrice";
            this.chkCalcUnitPrice.Size = new System.Drawing.Size(173, 18);
            this.chkCalcUnitPrice.TabIndex = 4;
            this.chkCalcUnitPrice.Text = "Calculate Unit Price for Lot";
            ultraToolTipInfo10.ToolTipText = "Calculate Unit Price for Lot during export.";
            ultraToolTipInfo10.ToolTipTitle = "Calculate Unit Price for Lot";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkCalcUnitPrice, ultraToolTipInfo10);
            // 
            // txtTransactionClass
            // 
            this.txtTransactionClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTransactionClass.Location = new System.Drawing.Point(162, 189);
            this.txtTransactionClass.Name = "txtTransactionClass";
            this.txtTransactionClass.Size = new System.Drawing.Size(366, 22);
            this.txtTransactionClass.TabIndex = 8;
            ultraToolTipInfo11.ToolTipText = "QuickBooks transaction class";
            ultraToolTipInfo11.ToolTipTitle = "Transaction Class";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtTransactionClass, ultraToolTipInfo11);
            // 
            // txtConnectionName
            // 
            this.txtConnectionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConnectionName.Location = new System.Drawing.Point(162, 161);
            this.txtConnectionName.Name = "txtConnectionName";
            this.txtConnectionName.Size = new System.Drawing.Size(366, 22);
            this.txtConnectionName.TabIndex = 7;
            ultraToolTipInfo12.ToolTipText = "This field assures that DWOS can connect with QuickBooks for the Export.";
            ultraToolTipInfo12.ToolTipTitle = "Connection";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtConnectionName, ultraToolTipInfo12);
            // 
            // chkExportSyspro
            // 
            this.chkExportSyspro.AutoSize = true;
            this.chkExportSyspro.Enabled = false;
            this.chkExportSyspro.Location = new System.Drawing.Point(20, 274);
            this.chkExportSyspro.Name = "chkExportSyspro";
            this.chkExportSyspro.Size = new System.Drawing.Size(68, 18);
            this.chkExportSyspro.TabIndex = 11;
            this.chkExportSyspro.Text = "SYSPRO";
            ultraToolTipInfo13.ToolTipTextFormatted = "Select to Export Invoice in a SYSPRO .XML format.<br/>SYSPRO integration must be " +
    "setup on the DWOS server before you can export invoices to it.";
            ultraToolTipInfo13.ToolTipTitle = "SYSPRO";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkExportSyspro, ultraToolTipInfo13);
            this.chkExportSyspro.CheckedChanged += new System.EventHandler(this.chkExportSyspro_CheckedChanged);
            // 
            // txtExportTokens
            // 
            this.txtExportTokens.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExportTokens.Location = new System.Drawing.Point(162, 57);
            this.txtExportTokens.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtExportTokens.MaxLength = 500;
            this.txtExportTokens.Name = "txtExportTokens";
            this.txtExportTokens.NullText = "Export Fields";
            appearance3.FontData.ItalicAsString = "True";
            appearance3.ForeColor = System.Drawing.Color.Silver;
            this.txtExportTokens.NullTextAppearance = appearance3;
            this.txtExportTokens.Size = new System.Drawing.Size(366, 22);
            this.txtExportTokens.TabIndex = 3;
            ultraToolTipInfo14.ToolTipText = "The list of field tokens used for exporting invoices to CSV format.";
            ultraToolTipInfo14.ToolTipTitle = "Field Tokens";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtExportTokens, ultraToolTipInfo14);
            // 
            // cboQuickbooksRounding
            // 
            this.cboQuickbooksRounding.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboQuickbooksRounding.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboQuickbooksRounding.Location = new System.Drawing.Point(162, 217);
            this.cboQuickbooksRounding.Name = "cboQuickbooksRounding";
            this.cboQuickbooksRounding.Size = new System.Drawing.Size(366, 22);
            this.cboQuickbooksRounding.TabIndex = 9;
            ultraToolTipInfo15.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo15.ToolTipTextFormatted");
            ultraToolTipInfo15.ToolTipTitle = "Rounding Type";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboQuickbooksRounding, ultraToolTipInfo15);
            // 
            // ultraLabel17
            // 
            this.ultraLabel17.AutoSize = true;
            this.ultraLabel17.Location = new System.Drawing.Point(41, 221);
            this.ultraLabel17.Name = "ultraLabel17";
            this.ultraLabel17.Size = new System.Drawing.Size(63, 15);
            this.ultraLabel17.TabIndex = 108;
            this.ultraLabel17.Text = "Rounding:";
            // 
            // btnSyspro
            // 
            this.btnSyspro.Enabled = false;
            this.btnSyspro.Location = new System.Drawing.Point(41, 298);
            this.btnSyspro.Name = "btnSyspro";
            this.btnSyspro.Size = new System.Drawing.Size(201, 23);
            this.btnSyspro.TabIndex = 12;
            this.btnSyspro.Text = "Configure SYSPRO Integration...";
            this.btnSyspro.Click += new System.EventHandler(this.btnSyspro_Click);
            // 
            // chkExportCSV
            // 
            this.chkExportCSV.AutoSize = true;
            this.chkExportCSV.Location = new System.Drawing.Point(20, 27);
            this.chkExportCSV.Name = "chkExportCSV";
            this.chkExportCSV.Size = new System.Drawing.Size(45, 18);
            this.chkExportCSV.TabIndex = 2;
            this.chkExportCSV.Text = "CSV";
            ultraToolTipInfo16.ToolTipText = "Select to Export Invoice in a .CSV format";
            ultraToolTipInfo16.ToolTipTitle = "CSV";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkExportCSV, ultraToolTipInfo16);
            this.chkExportCSV.CheckedChanged += new System.EventHandler(this.chkExportCSV_CheckedChanged);
            // 
            // chkExportQuickbooks
            // 
            this.chkExportQuickbooks.AutoSize = true;
            this.chkExportQuickbooks.Location = new System.Drawing.Point(41, 133);
            this.chkExportQuickbooks.Name = "chkExportQuickbooks";
            this.chkExportQuickbooks.Size = new System.Drawing.Size(86, 18);
            this.chkExportQuickbooks.TabIndex = 6;
            this.chkExportQuickbooks.Text = "Quickbooks";
            ultraToolTipInfo17.ToolTipText = "Selecting will tell DWOS to Export Invoice to QuickBooks.";
            ultraToolTipInfo17.ToolTipTitle = "QuickBooks";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkExportQuickbooks, ultraToolTipInfo17);
            this.chkExportQuickbooks.CheckedChanged += new System.EventHandler(this.chkExportQuickbooks_CheckedChanged);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(41, 57);
            this.ultraLabel6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(83, 15);
            this.ultraLabel6.TabIndex = 65;
            this.ultraLabel6.Text = "Export Fields:";
            // 
            // FormLabel
            // 
            this.FormLabel.AutoSize = true;
            this.FormLabel.Location = new System.Drawing.Point(41, 165);
            this.FormLabel.Name = "FormLabel";
            this.FormLabel.Size = new System.Drawing.Size(73, 15);
            this.FormLabel.TabIndex = 76;
            this.FormLabel.Text = "Connection:";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(41, 193);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(109, 15);
            this.ultraLabel5.TabIndex = 81;
            this.ultraLabel5.Text = "Transaction Class:";
            // 
            // ultraLabel9
            // 
            this.ultraLabel9.AutoSize = true;
            this.ultraLabel9.Location = new System.Drawing.Point(8, 31);
            this.ultraLabel9.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel9.Name = "ultraLabel9";
            this.ultraLabel9.Size = new System.Drawing.Size(84, 15);
            this.ultraLabel9.TabIndex = 83;
            this.ultraLabel9.Text = "Invoice Level:";
            // 
            // pnlGeneralSettings
            // 
            this.pnlGeneralSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // pnlGeneralSettings.ClientArea
            // 
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.ultraLabel16);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.numMaxExport);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.txtPackagePrefix);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.ultraLabel8);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.txtWOPrefix);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.ultraLabel1);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.txtSOPrefix);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.txtCustomerWO);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.ultraLabel14);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.ultraLabel3);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.cboOther2);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.txtTrackingNumber);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.cboOther1);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.ultraLabel4);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.ultraLabel13);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.ultraLabel7);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.ultraLabel12);
            this.pnlGeneralSettings.ClientArea.Controls.Add(this.numMaxErrors);
            this.pnlGeneralSettings.Location = new System.Drawing.Point(10, 601);
            this.pnlGeneralSettings.Name = "pnlGeneralSettings";
            this.pnlGeneralSettings.Size = new System.Drawing.Size(535, 277);
            this.pnlGeneralSettings.TabIndex = 6;
            // 
            // ultraLabel16
            // 
            this.ultraLabel16.AutoSize = true;
            this.ultraLabel16.Location = new System.Drawing.Point(20, 129);
            this.ultraLabel16.Name = "ultraLabel16";
            this.ultraLabel16.Size = new System.Drawing.Size(138, 15);
            this.ultraLabel16.TabIndex = 101;
            this.ultraLabel16.Text = "Package Invoice Prefix:";
            // 
            // numMaxExport
            // 
            this.numMaxExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numMaxExport.Location = new System.Drawing.Point(162, 8);
            this.numMaxExport.MaxValue = 500;
            this.numMaxExport.MinValue = 1;
            this.numMaxExport.Name = "numMaxExport";
            this.numMaxExport.Size = new System.Drawing.Size(366, 22);
            this.numMaxExport.TabIndex = 5;
            ultraToolTipInfo18.ToolTipText = "Set the maximum number of Invoices to export at one time.";
            ultraToolTipInfo18.ToolTipTitle = "Maximum Export";
            this.ultraToolTipManager1.SetUltraToolTip(this.numMaxExport, ultraToolTipInfo18);
            // 
            // txtPackagePrefix
            // 
            this.txtPackagePrefix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPackagePrefix.Location = new System.Drawing.Point(162, 125);
            this.txtPackagePrefix.Name = "txtPackagePrefix";
            this.txtPackagePrefix.Size = new System.Drawing.Size(366, 22);
            this.txtPackagePrefix.TabIndex = 9;
            ultraToolTipInfo19.ToolTipText = "Attach a prefix to a package invoice in QuickBooks.";
            ultraToolTipInfo19.ToolTipTitle = "Package Invoice Prefix";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtPackagePrefix, ultraToolTipInfo19);
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(20, 72);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(111, 15);
            this.ultraLabel8.TabIndex = 77;
            this.ultraLabel8.Text = "WO Invoice Prefix:";
            // 
            // txtWOPrefix
            // 
            this.txtWOPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWOPrefix.Location = new System.Drawing.Point(162, 68);
            this.txtWOPrefix.Name = "txtWOPrefix";
            this.txtWOPrefix.Size = new System.Drawing.Size(366, 22);
            this.txtWOPrefix.TabIndex = 7;
            ultraToolTipInfo20.ToolTipText = "Attach a prefix to a work order invoice in QuickBooks.";
            ultraToolTipInfo20.ToolTipTitle = "Work Order Invoice Prefix";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtWOPrefix, ultraToolTipInfo20);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(20, 157);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(133, 15);
            this.ultraLabel1.TabIndex = 78;
            this.ultraLabel1.Text = "Customer WO Header:";
            // 
            // txtSOPrefix
            // 
            this.txtSOPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSOPrefix.Location = new System.Drawing.Point(162, 97);
            this.txtSOPrefix.Name = "txtSOPrefix";
            this.txtSOPrefix.Size = new System.Drawing.Size(366, 22);
            this.txtSOPrefix.TabIndex = 8;
            ultraToolTipInfo21.ToolTipText = "Attach a prefix to a sales order invoice in QuickBooks.";
            ultraToolTipInfo21.ToolTipTitle = "Sales Order Invoice Prefix";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtSOPrefix, ultraToolTipInfo21);
            // 
            // txtCustomerWO
            // 
            this.txtCustomerWO.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomerWO.Location = new System.Drawing.Point(162, 153);
            this.txtCustomerWO.Name = "txtCustomerWO";
            this.txtCustomerWO.Size = new System.Drawing.Size(367, 22);
            this.txtCustomerWO.TabIndex = 10;
            ultraToolTipInfo22.ToolTipText = "Map the invoice header item to an order Customer WO.";
            ultraToolTipInfo22.ToolTipTitle = "Invoice Header Item";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtCustomerWO, ultraToolTipInfo22);
            // 
            // ultraLabel14
            // 
            this.ultraLabel14.AutoSize = true;
            this.ultraLabel14.Location = new System.Drawing.Point(20, 101);
            this.ultraLabel14.Name = "ultraLabel14";
            this.ultraLabel14.Size = new System.Drawing.Size(107, 15);
            this.ultraLabel14.TabIndex = 97;
            this.ultraLabel14.Text = "SO Invoice Prefix:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(20, 185);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(116, 15);
            this.ultraLabel3.TabIndex = 79;
            this.ultraLabel3.Text = "Tracking # Header:";
            // 
            // cboOther2
            // 
            this.cboOther2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboOther2.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboOther2.Location = new System.Drawing.Point(162, 237);
            this.cboOther2.Name = "cboOther2";
            this.cboOther2.Size = new System.Drawing.Size(366, 22);
            this.cboOther2.TabIndex = 13;
            ultraToolTipInfo23.ToolTipText = "Determines which value, if any, should be placed in the \'Other 2\' column of an in" +
    "voice in QuickBooks.";
            ultraToolTipInfo23.ToolTipTitle = "Invoice Column \'Other 2\'";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboOther2, ultraToolTipInfo23);
            // 
            // txtTrackingNumber
            // 
            this.txtTrackingNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTrackingNumber.Location = new System.Drawing.Point(162, 181);
            this.txtTrackingNumber.Name = "txtTrackingNumber";
            this.txtTrackingNumber.Size = new System.Drawing.Size(366, 22);
            this.txtTrackingNumber.TabIndex = 11;
            ultraToolTipInfo24.ToolTipText = "Map the invoice header item to an order Tracking #.";
            ultraToolTipInfo24.ToolTipTitle = "Invoice Header Item";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtTrackingNumber, ultraToolTipInfo24);
            // 
            // cboOther1
            // 
            this.cboOther1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboOther1.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboOther1.Location = new System.Drawing.Point(162, 209);
            this.cboOther1.Name = "cboOther1";
            this.cboOther1.Size = new System.Drawing.Size(366, 22);
            this.cboOther1.TabIndex = 12;
            ultraToolTipInfo25.ToolTipText = "Determines which value, if any, should be placed in the \'Other 1\' column of an in" +
    "voice in QuickBooks.";
            ultraToolTipInfo25.ToolTipTitle = "Invoice Column \'Other 1\'";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboOther1, ultraToolTipInfo25);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(20, 12);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(73, 15);
            this.ultraLabel4.TabIndex = 80;
            this.ultraLabel4.Text = "Max Export:";
            // 
            // ultraLabel13
            // 
            this.ultraLabel13.Location = new System.Drawing.Point(20, 241);
            this.ultraLabel13.Name = "ultraLabel13";
            this.ultraLabel13.Size = new System.Drawing.Size(100, 23);
            this.ultraLabel13.TabIndex = 96;
            this.ultraLabel13.Text = "Other 2 Column:";
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(20, 43);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(71, 15);
            this.ultraLabel7.TabIndex = 89;
            this.ultraLabel7.Text = "Max Errors:";
            // 
            // ultraLabel12
            // 
            this.ultraLabel12.Location = new System.Drawing.Point(20, 213);
            this.ultraLabel12.Name = "ultraLabel12";
            this.ultraLabel12.Size = new System.Drawing.Size(100, 23);
            this.ultraLabel12.TabIndex = 95;
            this.ultraLabel12.Text = "Other 1 Column:";
            // 
            // numMaxErrors
            // 
            this.numMaxErrors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numMaxErrors.Location = new System.Drawing.Point(162, 39);
            this.numMaxErrors.MaxValue = 500;
            this.numMaxErrors.MinValue = 1;
            this.numMaxErrors.Name = "numMaxErrors";
            this.numMaxErrors.Size = new System.Drawing.Size(366, 22);
            this.numMaxErrors.TabIndex = 6;
            ultraToolTipInfo26.ToolTipText = "Export process will stop when maximum number of errors have been encountered.";
            ultraToolTipInfo26.ToolTipTitle = "Maximum Errors";
            this.ultraToolTipManager1.SetUltraToolTip(this.numMaxErrors, ultraToolTipInfo26);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // SettingsAccountingInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(300, 870);
            this.Name = "SettingsAccountingInfo";
            this.Size = new System.Drawing.Size(548, 896);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkIndexSOInvoices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboInvoiceType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboInvoiceLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCheckTotalPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPriceDecimals)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox3)).EndInit();
            this.ultraGroupBox3.ResumeLayout(false);
            this.ultraGroupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtExportPartItemCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExportPartItemName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkHeaders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCalcUnitPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransactionClass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConnectionName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkExportSyspro)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExportTokens)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboQuickbooksRounding)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkExportCSV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkExportQuickbooks)).EndInit();
            this.pnlGeneralSettings.ClientArea.ResumeLayout(false);
            this.pnlGeneralSettings.ClientArea.PerformLayout();
            this.pnlGeneralSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numMaxExport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPackagePrefix)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWOPrefix)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSOPrefix)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerWO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOther2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTrackingNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOther1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxErrors)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtExportTokens;
        public Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtExportPartItemName;
        public Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTransactionClass;
        public Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numMaxExport;
        public Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTrackingNumber;
        public Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomerWO;
        public Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtWOPrefix;
        public Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtConnectionName;
        public Infragistics.Win.Misc.UltraLabel FormLabel;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboInvoiceLevel;
        public Infragistics.Win.Misc.UltraLabel ultraLabel9;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkExportQuickbooks;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkExportCSV;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numMaxErrors;
        public Infragistics.Win.Misc.UltraLabel ultraLabel7;
        public Infragistics.Win.Misc.UltraLabel ultraLabel10;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtExportPartItemCode;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox3;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkCalcUnitPrice;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numPriceDecimals;
        public Infragistics.Win.Misc.UltraLabel ultraLabel11;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboOther2;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboOther1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel13;
        private Infragistics.Win.Misc.UltraLabel ultraLabel12;
        private Infragistics.Win.Misc.UltraLabel ultraLabel14;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSOPrefix;
        private Infragistics.Win.Misc.UltraLabel ultraLabel15;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboInvoiceType;
        private Infragistics.Win.Misc.UltraLabel ultraLabel16;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPackagePrefix;
        private Infragistics.Win.Misc.UltraPanel pnlGeneralSettings;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkExportSyspro;
        private Infragistics.Win.Misc.UltraButton btnSyspro;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkCheckTotalPrice;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboQuickbooksRounding;
        public Infragistics.Win.Misc.UltraLabel ultraLabel17;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkHeaders;
        private Infragistics.Win.Misc.UltraButton btnQuickBooksTemplate;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkIndexSOInvoices;
    }
}
