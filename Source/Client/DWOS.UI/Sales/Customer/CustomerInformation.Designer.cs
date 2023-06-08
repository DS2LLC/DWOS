namespace DWOS.UI.Sales
{
	partial class CustomerInformation
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

			_shippingValidator?.Dispose();
			_shippingValidator = null;

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
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Determines if the user is active or not.", Infragistics.Win.ToolTipImage.Default, "Active", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The name of the customer.", Infragistics.Win.ToolTipImage.Default, "Customer Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The customer\'s country.", Infragistics.Win.ToolTipImage.Default, "Country", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The Zip Code.", Infragistics.Win.ToolTipImage.Default, "Zip Code", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The city name.", Infragistics.Win.ToolTipImage.Default, "City", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The two letter state abbreviation.", Infragistics.Win.ToolTipImage.Default, "State", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Address Line 2", Infragistics.Win.ToolTipImage.Default, "Address Line 2", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Address Line 1", Infragistics.Win.ToolTipImage.Default, "Address Line 1", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Require COC by Default", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo10 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Print COC", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo11 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The current customer standing.", Infragistics.Win.ToolTipImage.Default, "Customer Standing", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo12 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Print COC", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo13 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Number of days added to the customers orders to determine the shipping date.", Infragistics.Win.ToolTipImage.Default, "Lead Time", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo14 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Invoice Level", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo15 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The standard payment terms.", Infragistics.Win.ToolTipImage.Default, "Payment Terms", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo16 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo(" Accounting ID uniquely identifies the customer in the accounting system.", Infragistics.Win.ToolTipImage.Default, "Customer Accounting ID", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo17 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Customer Priority", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo18 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Check to have all invoices set to be printed for the customer.", Infragistics.Win.ToolTipImage.Default, "Print Invoice", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo19 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Order Review", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomerInformation));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo20 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Email Invoice", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo21 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Notes about the customer.", Infragistics.Win.ToolTipImage.Default, "Notes", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FieldID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Category", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Required");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DefaultValue");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("FieldID");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Category");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Name");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Required");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("DefaultValue");
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo22 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The country of the shipping address.", Infragistics.Win.ToolTipImage.Default, "Country", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo23 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Determines if this shipping address is active or not.", Infragistics.Win.ToolTipImage.Default, "Active", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo24 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Require Statement of Repairs", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo25 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Default", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo26 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The Zip Code.", Infragistics.Win.ToolTipImage.Default, "Zip Code", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo27 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The two letter state abbreviation.", Infragistics.Win.ToolTipImage.Default, "State", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo28 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The city name.", Infragistics.Win.ToolTipImage.Default, "City", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo29 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Address Line 2", Infragistics.Win.ToolTipImage.Default, "Address Line 2", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo30 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Address Line 1", Infragistics.Win.ToolTipImage.Default, "Address Line 1", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo31 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The name of the shipping address.", Infragistics.Win.ToolTipImage.Default, "Address Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.pnlGeneralTop = new Infragistics.Win.Misc.UltraPanel();
            this.chkActive = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.txtCustomerName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.pnlAddress = new Infragistics.Win.Misc.UltraPanel();
            this.cboCustomerCountry = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel17 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCustomerZip = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtCustomerCity = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtCustomerState = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCustomerAddress2 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCustomerAddress1 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.pnlRequireCoc = new Infragistics.Win.Misc.UltraPanel();
            this.chkRequireCoc = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.pnlGeneralBottom = new Infragistics.Win.Misc.UltraPanel();
            this.cboShowSNonApprovalSubjectLine = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.btnDefaultFees = new Infragistics.Win.Misc.UltraButton();
            this.btnPricePoints = new Infragistics.Win.Misc.UltraButton();
            this.cboCustomerStanding = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.chkPrintCOC = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.numLeadTime = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.cboInvoiceLevel = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel10 = new Infragistics.Win.Misc.UltraLabel();
            this.cboTerms = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel11 = new Infragistics.Win.Misc.UltraLabel();
            this.txtAccountingID = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboPriority = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel9 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.chkPrint = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkOrderReview = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkEmail = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel12 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCustomerNotes = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.grdFields = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsFields = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvwAddress = new Infragistics.Win.UltraWinTree.UltraTree();
            this.pnlShipTo = new Infragistics.Win.Misc.UltraPanel();
            this.cboShipCountry = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel18 = new Infragistics.Win.Misc.UltraLabel();
            this.chkShipActive = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkShipRepairStatement = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkShipDefault = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel16 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel15 = new Infragistics.Win.Misc.UltraLabel();
            this.txtShipZip = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtShipState = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtShipCity = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtShipAddress2 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel14 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel13 = new Infragistics.Win.Misc.UltraLabel();
            this.txtShipAddress1 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtShipName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.bsCustomerAddress = new System.Windows.Forms.BindingSource(this.components);
            this.generalLayoutManager = new Infragistics.Win.Misc.UltraFlowLayoutManager(this.components);
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.tabCustomer = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            this.ultraTabPageControl1.SuspendLayout();
            this.pnlGeneralTop.ClientArea.SuspendLayout();
            this.pnlGeneralTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkActive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerName)).BeginInit();
            this.pnlAddress.ClientArea.SuspendLayout();
            this.pnlAddress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomerCountry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerZip)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerCity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerState)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerAddress2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerAddress1)).BeginInit();
            this.pnlRequireCoc.ClientArea.SuspendLayout();
            this.pnlRequireCoc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkRequireCoc)).BeginInit();
            this.pnlGeneralBottom.ClientArea.SuspendLayout();
            this.pnlGeneralBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboShowSNonApprovalSubjectLine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomerStanding)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintCOC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLeadTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboInvoiceLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTerms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAccountingID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPriority)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkOrderReview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEmail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerNotes)).BeginInit();
            this.ultraTabPageControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdFields)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsFields)).BeginInit();
            this.ultraTabPageControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tvwAddress)).BeginInit();
            this.pnlShipTo.ClientArea.SuspendLayout();
            this.pnlShipTo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboShipCountry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShipActive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShipRepairStatement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShipDefault)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipZip)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipState)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipCity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipAddress2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipAddress1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCustomerAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.generalLayoutManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabCustomer)).BeginInit();
            this.tabCustomer.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.tabCustomer);
            appearance8.Image = global::DWOS.UI.Properties.Resources.Customer;
            this.grpData.HeaderAppearance = appearance8;
            this.grpData.Size = new System.Drawing.Size(903, 559);
            this.grpData.Text = "Customer Information";
            this.grpData.Controls.SetChildIndex(this.tabCustomer, 0);
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(8073, -5635);
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.pnlGeneralTop);
            this.ultraTabPageControl1.Controls.Add(this.pnlAddress);
            this.ultraTabPageControl1.Controls.Add(this.pnlRequireCoc);
            this.ultraTabPageControl1.Controls.Add(this.pnlGeneralBottom);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(6, 28);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
            this.ultraTabPageControl1.Size = new System.Drawing.Size(873, 497);
            // 
            // pnlGeneralTop
            // 
            this.pnlGeneralTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.pnlGeneralTop.Appearance = appearance1;
            // 
            // pnlGeneralTop.ClientArea
            // 
            this.pnlGeneralTop.ClientArea.Controls.Add(this.chkActive);
            this.pnlGeneralTop.ClientArea.Controls.Add(this.txtCustomerName);
            this.pnlGeneralTop.ClientArea.Controls.Add(this.ultraLabel2);
            this.pnlGeneralTop.Location = new System.Drawing.Point(5, 0);
            this.pnlGeneralTop.Name = "pnlGeneralTop";
            this.pnlGeneralTop.Size = new System.Drawing.Size(868, 26);
            this.pnlGeneralTop.TabIndex = 1;
            // 
            // chkActive
            // 
            this.chkActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkActive.Location = new System.Drawing.Point(805, 4);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(60, 20);
            this.chkActive.TabIndex = 1;
            this.chkActive.Text = "Active";
            ultraToolTipInfo1.ToolTipText = "Determines if the user is active or not.";
            ultraToolTipInfo1.ToolTipTitle = "Active";
            this.tipManager.SetUltraToolTip(this.chkActive, ultraToolTipInfo1);
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomerName.Location = new System.Drawing.Point(94, 2);
            this.txtCustomerName.MaxLength = 255;
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Nullable = false;
            this.txtCustomerName.Size = new System.Drawing.Size(705, 22);
            this.txtCustomerName.TabIndex = 0;
            ultraToolTipInfo2.ToolTipText = "The name of the customer.";
            ultraToolTipInfo2.ToolTipTitle = "Customer Name";
            this.tipManager.SetUltraToolTip(this.txtCustomerName, ultraToolTipInfo2);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(0, 4);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel2.TabIndex = 21;
            this.ultraLabel2.Text = "Name:";
            // 
            // pnlAddress
            // 
            this.pnlAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.BackColor = System.Drawing.Color.White;
            this.pnlAddress.Appearance = appearance2;
            // 
            // pnlAddress.ClientArea
            // 
            this.pnlAddress.ClientArea.Controls.Add(this.cboCustomerCountry);
            this.pnlAddress.ClientArea.Controls.Add(this.ultraLabel17);
            this.pnlAddress.ClientArea.Controls.Add(this.ultraLabel3);
            this.pnlAddress.ClientArea.Controls.Add(this.txtCustomerZip);
            this.pnlAddress.ClientArea.Controls.Add(this.txtCustomerCity);
            this.pnlAddress.ClientArea.Controls.Add(this.txtCustomerState);
            this.pnlAddress.ClientArea.Controls.Add(this.ultraLabel5);
            this.pnlAddress.ClientArea.Controls.Add(this.txtCustomerAddress2);
            this.pnlAddress.ClientArea.Controls.Add(this.ultraLabel4);
            this.pnlAddress.ClientArea.Controls.Add(this.txtCustomerAddress1);
            this.pnlAddress.Location = new System.Drawing.Point(5, 26);
            this.pnlAddress.Name = "pnlAddress";
            this.pnlAddress.Size = new System.Drawing.Size(868, 114);
            this.pnlAddress.TabIndex = 2;
            // 
            // cboCustomerCountry
            // 
            this.cboCustomerCountry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCustomerCountry.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCustomerCountry.Location = new System.Drawing.Point(94, 86);
            this.cboCustomerCountry.Name = "cboCustomerCountry";
            this.cboCustomerCountry.Size = new System.Drawing.Size(771, 22);
            this.cboCustomerCountry.TabIndex = 7;
            ultraToolTipInfo3.ToolTipText = "The customer\'s country.";
            ultraToolTipInfo3.ToolTipTitle = "Country";
            this.tipManager.SetUltraToolTip(this.cboCustomerCountry, ultraToolTipInfo3);
            // 
            // ultraLabel17
            // 
            this.ultraLabel17.AutoSize = true;
            this.ultraLabel17.Location = new System.Drawing.Point(0, 90);
            this.ultraLabel17.Name = "ultraLabel17";
            this.ultraLabel17.Size = new System.Drawing.Size(54, 15);
            this.ultraLabel17.TabIndex = 32;
            this.ultraLabel17.Text = "Country:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(0, 8);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(66, 15);
            this.ultraLabel3.TabIndex = 25;
            this.ultraLabel3.Text = "Address 1:";
            // 
            // txtCustomerZip
            // 
            this.txtCustomerZip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomerZip.Location = new System.Drawing.Point(280, 58);
            this.txtCustomerZip.MaxLength = 50;
            this.txtCustomerZip.Name = "txtCustomerZip";
            this.txtCustomerZip.NullText = "ZIP";
            this.txtCustomerZip.Size = new System.Drawing.Size(585, 22);
            this.txtCustomerZip.TabIndex = 6;
            ultraToolTipInfo4.ToolTipText = "The Zip Code.";
            ultraToolTipInfo4.ToolTipTitle = "Zip Code";
            this.tipManager.SetUltraToolTip(this.txtCustomerZip, ultraToolTipInfo4);
            // 
            // txtCustomerCity
            // 
            this.txtCustomerCity.Location = new System.Drawing.Point(94, 58);
            this.txtCustomerCity.MaxLength = 50;
            this.txtCustomerCity.Name = "txtCustomerCity";
            this.txtCustomerCity.NullText = "CITY";
            this.txtCustomerCity.Size = new System.Drawing.Size(145, 22);
            this.txtCustomerCity.TabIndex = 4;
            ultraToolTipInfo5.ToolTipText = "The city name.";
            ultraToolTipInfo5.ToolTipTitle = "City";
            this.tipManager.SetUltraToolTip(this.txtCustomerCity, ultraToolTipInfo5);
            // 
            // txtCustomerState
            // 
            this.txtCustomerState.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCustomerState.Location = new System.Drawing.Point(245, 58);
            this.txtCustomerState.MaxLength = 2;
            this.txtCustomerState.Name = "txtCustomerState";
            this.txtCustomerState.NullText = "ST";
            this.txtCustomerState.Size = new System.Drawing.Size(29, 22);
            this.txtCustomerState.TabIndex = 5;
            ultraToolTipInfo6.ToolTipText = "The two letter state abbreviation.";
            ultraToolTipInfo6.ToolTipTitle = "State";
            this.tipManager.SetUltraToolTip(this.txtCustomerState, ultraToolTipInfo6);
            this.txtCustomerState.ValueChanged += new System.EventHandler(this.txtCustomerState_ValueChanged);
            this.txtCustomerState.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCustomerState_KeyPress);
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(0, 62);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(92, 15);
            this.ultraLabel5.TabIndex = 31;
            this.ultraLabel5.Text = "City, State Zip:";
            // 
            // txtCustomerAddress2
            // 
            this.txtCustomerAddress2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomerAddress2.Location = new System.Drawing.Point(94, 30);
            this.txtCustomerAddress2.Name = "txtCustomerAddress2";
            this.txtCustomerAddress2.Nullable = false;
            this.txtCustomerAddress2.Size = new System.Drawing.Size(771, 22);
            this.txtCustomerAddress2.TabIndex = 3;
            ultraToolTipInfo7.ToolTipText = "Address Line 2";
            ultraToolTipInfo7.ToolTipTitle = "Address Line 2";
            this.tipManager.SetUltraToolTip(this.txtCustomerAddress2, ultraToolTipInfo7);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(0, 34);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(66, 15);
            this.ultraLabel4.TabIndex = 27;
            this.ultraLabel4.Text = "Address 2:";
            // 
            // txtCustomerAddress1
            // 
            this.txtCustomerAddress1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomerAddress1.Location = new System.Drawing.Point(94, 4);
            this.txtCustomerAddress1.Name = "txtCustomerAddress1";
            this.txtCustomerAddress1.Nullable = false;
            this.txtCustomerAddress1.Size = new System.Drawing.Size(771, 22);
            this.txtCustomerAddress1.TabIndex = 2;
            ultraToolTipInfo8.ToolTipText = "Address Line 1";
            ultraToolTipInfo8.ToolTipTitle = "Address Line 1";
            this.tipManager.SetUltraToolTip(this.txtCustomerAddress1, ultraToolTipInfo8);
            // 
            // pnlRequireCoc
            // 
            this.pnlRequireCoc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance3.BackColor = System.Drawing.Color.Transparent;
            this.pnlRequireCoc.Appearance = appearance3;
            // 
            // pnlRequireCoc.ClientArea
            // 
            this.pnlRequireCoc.ClientArea.Controls.Add(this.chkRequireCoc);
            this.pnlRequireCoc.Location = new System.Drawing.Point(5, 140);
            this.pnlRequireCoc.Name = "pnlRequireCoc";
            this.pnlRequireCoc.Size = new System.Drawing.Size(868, 28);
            this.pnlRequireCoc.TabIndex = 3;
            // 
            // chkRequireCoc
            // 
            this.chkRequireCoc.AutoSize = true;
            this.chkRequireCoc.Location = new System.Drawing.Point(3, 4);
            this.chkRequireCoc.Name = "chkRequireCoc";
            this.chkRequireCoc.Size = new System.Drawing.Size(156, 18);
            this.chkRequireCoc.TabIndex = 0;
            this.chkRequireCoc.Text = "Require COC by Default";
            ultraToolTipInfo9.ToolTipTextFormatted = "If <b>checked</b>, require a COC for this customer\'s orders by default.";
            ultraToolTipInfo9.ToolTipTitle = "Require COC by Default";
            this.tipManager.SetUltraToolTip(this.chkRequireCoc, ultraToolTipInfo9);
            // 
            // pnlGeneralBottom
            // 
            this.pnlGeneralBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance4.BackColor = System.Drawing.Color.Transparent;
            this.pnlGeneralBottom.Appearance = appearance4;
            // 
            // pnlGeneralBottom.ClientArea
            // 
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.cboShowSNonApprovalSubjectLine);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.btnDefaultFees);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.btnPricePoints);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.cboCustomerStanding);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.chkPrintCOC);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.numLeadTime);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.cboInvoiceLevel);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.ultraLabel6);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.ultraLabel10);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.cboTerms);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.ultraLabel1);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.ultraLabel11);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.txtAccountingID);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.cboPriority);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.ultraLabel9);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.ultraLabel7);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.ultraLabel8);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.chkPrint);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.chkOrderReview);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.chkEmail);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.ultraLabel12);
            this.pnlGeneralBottom.ClientArea.Controls.Add(this.txtCustomerNotes);
            this.pnlGeneralBottom.Location = new System.Drawing.Point(5, 168);
            this.pnlGeneralBottom.Name = "pnlGeneralBottom";
            this.pnlGeneralBottom.Size = new System.Drawing.Size(868, 329);
            this.pnlGeneralBottom.TabIndex = 4;
            // 
            // cboShowSNonApprovalSubjectLine
            // 
            this.cboShowSNonApprovalSubjectLine.AutoSize = true;
            this.cboShowSNonApprovalSubjectLine.Location = new System.Drawing.Point(375, 116);
            this.cboShowSNonApprovalSubjectLine.Name = "cboShowSNonApprovalSubjectLine";
            this.cboShowSNonApprovalSubjectLine.Size = new System.Drawing.Size(290, 18);
            this.cboShowSNonApprovalSubjectLine.TabIndex = 45;
            this.cboShowSNonApprovalSubjectLine.Text = "Show Serial Numbers on Approval Subject Line";
            ultraToolTipInfo10.ToolTipTextFormatted = "If checked, customer approval email will contain serial numbers in the subject li" +
    "ne";
            ultraToolTipInfo10.ToolTipTitle = "Print COC";
            this.tipManager.SetUltraToolTip(this.cboShowSNonApprovalSubjectLine, ultraToolTipInfo10);
            // 
            // btnDefaultFees
            // 
            this.btnDefaultFees.Location = new System.Drawing.Point(375, 140);
            this.btnDefaultFees.Name = "btnDefaultFees";
            this.btnDefaultFees.Size = new System.Drawing.Size(193, 23);
            this.btnDefaultFees.TabIndex = 17;
            this.btnDefaultFees.Text = "Edit Default Fees/Discounts...";
            this.btnDefaultFees.Click += new System.EventHandler(this.btnDefaultFees_Click);
            // 
            // btnPricePoints
            // 
            this.btnPricePoints.Location = new System.Drawing.Point(94, 140);
            this.btnPricePoints.Name = "btnPricePoints";
            this.btnPricePoints.Size = new System.Drawing.Size(184, 23);
            this.btnPricePoints.TabIndex = 16;
            this.btnPricePoints.Text = "Edit Default Price Points...";
            this.btnPricePoints.Click += new System.EventHandler(this.btnPricePoints_Click);
            // 
            // cboCustomerStanding
            // 
            this.cboCustomerStanding.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCustomerStanding.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCustomerStanding.Location = new System.Drawing.Point(375, 2);
            this.cboCustomerStanding.Name = "cboCustomerStanding";
            this.cboCustomerStanding.Size = new System.Drawing.Size(490, 22);
            this.cboCustomerStanding.TabIndex = 7;
            ultraToolTipInfo11.ToolTipText = "The current customer standing.";
            ultraToolTipInfo11.ToolTipTitle = "Customer Standing";
            this.tipManager.SetUltraToolTip(this.cboCustomerStanding, ultraToolTipInfo11);
            // 
            // chkPrintCOC
            // 
            this.chkPrintCOC.AutoSize = true;
            this.chkPrintCOC.Location = new System.Drawing.Point(94, 116);
            this.chkPrintCOC.Name = "chkPrintCOC";
            this.chkPrintCOC.Size = new System.Drawing.Size(184, 18);
            this.chkPrintCOC.TabIndex = 15;
            this.chkPrintCOC.Text = "Print COC in Final Inspection";
            ultraToolTipInfo12.ToolTipTextFormatted = "If checked, then the COC can be printed in Final Inspection.";
            ultraToolTipInfo12.ToolTipTitle = "Print COC";
            this.tipManager.SetUltraToolTip(this.chkPrintCOC, ultraToolTipInfo12);
            // 
            // numLeadTime
            // 
            this.numLeadTime.Location = new System.Drawing.Point(94, 30);
            this.numLeadTime.MaxValue = 365;
            this.numLeadTime.MinValue = 1;
            this.numLeadTime.Name = "numLeadTime";
            this.numLeadTime.Size = new System.Drawing.Size(180, 22);
            this.numLeadTime.TabIndex = 8;
            ultraToolTipInfo13.ToolTipText = "Number of days added to the customers orders to determine the shipping date.";
            ultraToolTipInfo13.ToolTipTitle = "Lead Time";
            this.tipManager.SetUltraToolTip(this.numLeadTime, ultraToolTipInfo13);
            this.numLeadTime.Value = 10;
            // 
            // cboInvoiceLevel
            // 
            this.cboInvoiceLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboInvoiceLevel.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboInvoiceLevel.Location = new System.Drawing.Point(375, 60);
            this.cboInvoiceLevel.Name = "cboInvoiceLevel";
            this.cboInvoiceLevel.Size = new System.Drawing.Size(490, 22);
            this.cboInvoiceLevel.TabIndex = 12;
            ultraToolTipInfo14.ToolTipTextFormatted = "The accounting invoice level for exporting invoices.";
            ultraToolTipInfo14.ToolTipTitle = "Invoice Level";
            this.tipManager.SetUltraToolTip(this.cboInvoiceLevel, ultraToolTipInfo14);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(0, 34);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(68, 15);
            this.ultraLabel6.TabIndex = 36;
            this.ultraLabel6.Text = "Lead Time:";
            // 
            // ultraLabel10
            // 
            this.ultraLabel10.AutoSize = true;
            this.ultraLabel10.Location = new System.Drawing.Point(285, 64);
            this.ultraLabel10.Name = "ultraLabel10";
            this.ultraLabel10.Size = new System.Drawing.Size(84, 15);
            this.ultraLabel10.TabIndex = 44;
            this.ultraLabel10.Text = "Invoice Level:";
            // 
            // cboTerms
            // 
            this.cboTerms.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            this.cboTerms.Location = new System.Drawing.Point(94, 2);
            this.cboTerms.MaxLength = 50;
            this.cboTerms.Name = "cboTerms";
            this.cboTerms.Size = new System.Drawing.Size(180, 22);
            this.cboTerms.TabIndex = 6;
            ultraToolTipInfo15.ToolTipText = "The standard payment terms.";
            ultraToolTipInfo15.ToolTipTitle = "Payment Terms";
            this.tipManager.SetUltraToolTip(this.cboTerms, ultraToolTipInfo15);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(280, 91);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(89, 15);
            this.ultraLabel1.TabIndex = 42;
            this.ultraLabel1.Text = "Accounting ID:";
            // 
            // ultraLabel11
            // 
            this.ultraLabel11.AutoSize = true;
            this.ultraLabel11.Location = new System.Drawing.Point(0, 6);
            this.ultraLabel11.Name = "ultraLabel11";
            this.ultraLabel11.Size = new System.Drawing.Size(45, 15);
            this.ultraLabel11.TabIndex = 33;
            this.ultraLabel11.Text = "Terms:";
            // 
            // txtAccountingID
            // 
            this.txtAccountingID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAccountingID.Location = new System.Drawing.Point(375, 88);
            this.txtAccountingID.Name = "txtAccountingID";
            this.txtAccountingID.Nullable = false;
            this.txtAccountingID.Size = new System.Drawing.Size(490, 22);
            this.txtAccountingID.TabIndex = 14;
            ultraToolTipInfo16.ToolTipText = " Accounting ID uniquely identifies the customer in the accounting system.";
            ultraToolTipInfo16.ToolTipTitle = "Customer Accounting ID";
            this.tipManager.SetUltraToolTip(this.txtAccountingID, ultraToolTipInfo16);
            // 
            // cboPriority
            // 
            this.cboPriority.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPriority.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboPriority.Location = new System.Drawing.Point(375, 30);
            this.cboPriority.Name = "cboPriority";
            this.cboPriority.Size = new System.Drawing.Size(490, 22);
            this.cboPriority.TabIndex = 9;
            ultraToolTipInfo17.ToolTipTextFormatted = "The priority of the customer, this will affect scheduling priority versus orders " +
    "from other customers.";
            ultraToolTipInfo17.ToolTipTitle = "Customer Priority";
            this.tipManager.SetUltraToolTip(this.cboPriority, ultraToolTipInfo17);
            // 
            // ultraLabel9
            // 
            this.ultraLabel9.AutoSize = true;
            this.ultraLabel9.Location = new System.Drawing.Point(309, 6);
            this.ultraLabel9.Name = "ultraLabel9";
            this.ultraLabel9.Size = new System.Drawing.Size(60, 15);
            this.ultraLabel9.TabIndex = 8;
            this.ultraLabel9.Text = "Standing:";
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(0, 64);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(61, 15);
            this.ultraLabel7.TabIndex = 40;
            this.ultraLabel7.Text = "Invoicing:";
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(319, 34);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(50, 15);
            this.ultraLabel8.TabIndex = 11;
            this.ultraLabel8.Text = "Priority:";
            // 
            // chkPrint
            // 
            this.chkPrint.Location = new System.Drawing.Point(94, 62);
            this.chkPrint.Name = "chkPrint";
            this.chkPrint.Size = new System.Drawing.Size(60, 20);
            this.chkPrint.TabIndex = 10;
            this.chkPrint.Text = "Print";
            ultraToolTipInfo18.ToolTipText = "Check to have all invoices set to be printed for the customer.";
            ultraToolTipInfo18.ToolTipTitle = "Print Invoice";
            this.tipManager.SetUltraToolTip(this.chkPrint, ultraToolTipInfo18);
            // 
            // chkOrderReview
            // 
            this.chkOrderReview.Location = new System.Drawing.Point(94, 89);
            this.chkOrderReview.Name = "chkOrderReview";
            this.chkOrderReview.Size = new System.Drawing.Size(142, 20);
            this.chkOrderReview.TabIndex = 13;
            this.chkOrderReview.Text = "Uses Order Review";
            ultraToolTipInfo19.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo19.ToolTipTextFormatted");
            ultraToolTipInfo19.ToolTipTitle = "Order Review";
            this.tipManager.SetUltraToolTip(this.chkOrderReview, ultraToolTipInfo19);
            // 
            // chkEmail
            // 
            this.chkEmail.Location = new System.Drawing.Point(182, 62);
            this.chkEmail.Name = "chkEmail";
            this.chkEmail.Size = new System.Drawing.Size(69, 20);
            this.chkEmail.TabIndex = 11;
            this.chkEmail.Text = "Email";
            ultraToolTipInfo20.ToolTipTextFormatted = "Check to have all invoices set to be emailed to the customer.<br/>When invoicing " +
    "through QuickBooks, the company must have a valid email address.";
            ultraToolTipInfo20.ToolTipTitle = "Email Invoice";
            this.tipManager.SetUltraToolTip(this.chkEmail, ultraToolTipInfo20);
            // 
            // ultraLabel12
            // 
            this.ultraLabel12.AutoSize = true;
            this.ultraLabel12.Location = new System.Drawing.Point(0, 169);
            this.ultraLabel12.Name = "ultraLabel12";
            this.ultraLabel12.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel12.TabIndex = 34;
            this.ultraLabel12.Text = "Notes:";
            // 
            // txtCustomerNotes
            // 
            this.txtCustomerNotes.AcceptsReturn = true;
            this.txtCustomerNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomerNotes.Location = new System.Drawing.Point(94, 169);
            this.txtCustomerNotes.Multiline = true;
            this.txtCustomerNotes.Name = "txtCustomerNotes";
            this.txtCustomerNotes.Nullable = false;
            this.txtCustomerNotes.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCustomerNotes.Size = new System.Drawing.Size(771, 157);
            this.txtCustomerNotes.TabIndex = 18;
            ultraToolTipInfo21.ToolTipText = "Notes about the customer.";
            ultraToolTipInfo21.ToolTipTitle = "Notes";
            this.tipManager.SetUltraToolTip(this.txtCustomerNotes, ultraToolTipInfo21);
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.grdFields);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(873, 497);
            // 
            // grdFields
            // 
            this.grdFields.DataSource = this.dsFields;
            ultraGridColumn1.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn1.Header.VisiblePosition = 3;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.CellActivation = Infragistics.Win.UltraWinGrid.Activation.Disabled;
            ultraGridColumn2.Header.VisiblePosition = 4;
            ultraGridColumn3.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn3.Header.VisiblePosition = 0;
            ultraGridColumn4.Header.VisiblePosition = 1;
            ultraGridColumn5.Header.Caption = "Default Value";
            ultraGridColumn5.Header.VisiblePosition = 2;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5});
            this.grdFields.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdFields.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdFields.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdFields.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.grdFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdFields.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdFields.Location = new System.Drawing.Point(0, 0);
            this.grdFields.Name = "grdFields";
            this.grdFields.Size = new System.Drawing.Size(873, 497);
            this.grdFields.TabIndex = 0;
            this.grdFields.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdFields_InitializeLayout);
            // 
            // dsFields
            // 
            this.dsFields.Band.AllowAdd = Infragistics.Win.DefaultableBoolean.False;
            this.dsFields.Band.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            ultraDataColumn1.DataType = typeof(int);
            ultraDataColumn1.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn2.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn3.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn4.DataType = typeof(bool);
            this.dsFields.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5});
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Controls.Add(this.splitContainer1);
            this.ultraTabPageControl3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(873, 497);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvwAddress);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlShipTo);
            this.splitContainer1.Size = new System.Drawing.Size(873, 497);
            this.splitContainer1.SplitterDistance = 215;
            this.splitContainer1.TabIndex = 0;
            // 
            // tvwAddress
            // 
            this.tvwAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwAddress.HideSelection = false;
            this.tvwAddress.Location = new System.Drawing.Point(0, 0);
            this.tvwAddress.Name = "tvwAddress";
            this.tvwAddress.Size = new System.Drawing.Size(215, 497);
            this.tvwAddress.TabIndex = 0;
            this.tvwAddress.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.tvwAddress_AfterSelect);
            this.tvwAddress.BeforeSelect += new Infragistics.Win.UltraWinTree.BeforeNodeSelectEventHandler(this.tvwAddress_BeforeSelect);
            // 
            // pnlShipTo
            // 
            // 
            // pnlShipTo.ClientArea
            // 
            this.pnlShipTo.ClientArea.Controls.Add(this.cboShipCountry);
            this.pnlShipTo.ClientArea.Controls.Add(this.ultraLabel18);
            this.pnlShipTo.ClientArea.Controls.Add(this.chkShipActive);
            this.pnlShipTo.ClientArea.Controls.Add(this.chkShipRepairStatement);
            this.pnlShipTo.ClientArea.Controls.Add(this.chkShipDefault);
            this.pnlShipTo.ClientArea.Controls.Add(this.ultraLabel16);
            this.pnlShipTo.ClientArea.Controls.Add(this.ultraLabel15);
            this.pnlShipTo.ClientArea.Controls.Add(this.txtShipZip);
            this.pnlShipTo.ClientArea.Controls.Add(this.txtShipState);
            this.pnlShipTo.ClientArea.Controls.Add(this.txtShipCity);
            this.pnlShipTo.ClientArea.Controls.Add(this.txtShipAddress2);
            this.pnlShipTo.ClientArea.Controls.Add(this.ultraLabel14);
            this.pnlShipTo.ClientArea.Controls.Add(this.ultraLabel13);
            this.pnlShipTo.ClientArea.Controls.Add(this.txtShipAddress1);
            this.pnlShipTo.ClientArea.Controls.Add(this.txtShipName);
            this.pnlShipTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlShipTo.Enabled = false;
            this.pnlShipTo.Location = new System.Drawing.Point(0, 0);
            this.pnlShipTo.Name = "pnlShipTo";
            this.pnlShipTo.Size = new System.Drawing.Size(654, 497);
            this.pnlShipTo.TabIndex = 0;
            // 
            // cboShipCountry
            // 
            this.cboShipCountry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboShipCountry.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboShipCountry.Location = new System.Drawing.Point(105, 115);
            this.cboShipCountry.Name = "cboShipCountry";
            this.cboShipCountry.Size = new System.Drawing.Size(545, 22);
            this.cboShipCountry.TabIndex = 7;
            ultraToolTipInfo22.ToolTipText = "The country of the shipping address.";
            ultraToolTipInfo22.ToolTipTitle = "Country";
            this.tipManager.SetUltraToolTip(this.cboShipCountry, ultraToolTipInfo22);
            this.cboShipCountry.ValueChanged += new System.EventHandler(this.cboShipCountry_ValueChanged);
            // 
            // ultraLabel18
            // 
            this.ultraLabel18.AutoSize = true;
            this.ultraLabel18.Location = new System.Drawing.Point(3, 119);
            this.ultraLabel18.Name = "ultraLabel18";
            this.ultraLabel18.Size = new System.Drawing.Size(54, 15);
            this.ultraLabel18.TabIndex = 10;
            this.ultraLabel18.Text = "Country:";
            // 
            // chkShipActive
            // 
            this.chkShipActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkShipActive.AutoSize = true;
            this.chkShipActive.Location = new System.Drawing.Point(572, 6);
            this.chkShipActive.Name = "chkShipActive";
            this.chkShipActive.Size = new System.Drawing.Size(56, 18);
            this.chkShipActive.TabIndex = 1;
            this.chkShipActive.Text = "Active";
            ultraToolTipInfo23.ToolTipText = "Determines if this shipping address is active or not.";
            ultraToolTipInfo23.ToolTipTitle = "Active";
            this.tipManager.SetUltraToolTip(this.chkShipActive, ultraToolTipInfo23);
            // 
            // chkShipRepairStatement
            // 
            this.chkShipRepairStatement.AutoSize = true;
            this.chkShipRepairStatement.Location = new System.Drawing.Point(3, 169);
            this.chkShipRepairStatement.Name = "chkShipRepairStatement";
            this.chkShipRepairStatement.Size = new System.Drawing.Size(185, 18);
            this.chkShipRepairStatement.TabIndex = 9;
            this.chkShipRepairStatement.Text = "Require statement of repairs";
            ultraToolTipInfo24.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo24.ToolTipTextFormatted");
            ultraToolTipInfo24.ToolTipTitle = "Require Statement of Repairs";
            this.tipManager.SetUltraToolTip(this.chkShipRepairStatement, ultraToolTipInfo24);
            // 
            // chkShipDefault
            // 
            this.chkShipDefault.AutoSize = true;
            this.chkShipDefault.Location = new System.Drawing.Point(3, 145);
            this.chkShipDefault.Name = "chkShipDefault";
            this.chkShipDefault.Size = new System.Drawing.Size(62, 18);
            this.chkShipDefault.TabIndex = 8;
            this.chkShipDefault.Text = "Default";
            ultraToolTipInfo25.ToolTipTextFormatted = "Determines if this shipping address is the default.<br/>";
            ultraToolTipInfo25.ToolTipTitle = "Default";
            this.tipManager.SetUltraToolTip(this.chkShipDefault, ultraToolTipInfo25);
            this.chkShipDefault.CheckedChanged += new System.EventHandler(this.chkShipDefault_CheckedChanged);
            // 
            // ultraLabel16
            // 
            this.ultraLabel16.AutoSize = true;
            this.ultraLabel16.Location = new System.Drawing.Point(3, 91);
            this.ultraLabel16.Name = "ultraLabel16";
            this.ultraLabel16.Size = new System.Drawing.Size(96, 15);
            this.ultraLabel16.TabIndex = 9;
            this.ultraLabel16.Text = "City, State, Zip:";
            // 
            // ultraLabel15
            // 
            this.ultraLabel15.AutoSize = true;
            this.ultraLabel15.Location = new System.Drawing.Point(3, 63);
            this.ultraLabel15.Name = "ultraLabel15";
            this.ultraLabel15.Size = new System.Drawing.Size(66, 15);
            this.ultraLabel15.TabIndex = 8;
            this.ultraLabel15.Text = "Address 2:";
            // 
            // txtShipZip
            // 
            this.txtShipZip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtShipZip.Location = new System.Drawing.Point(322, 87);
            this.txtShipZip.MaxLength = 50;
            this.txtShipZip.Name = "txtShipZip";
            this.txtShipZip.NullText = "ZIP";
            appearance5.ForeColor = System.Drawing.Color.Gainsboro;
            this.txtShipZip.NullTextAppearance = appearance5;
            this.txtShipZip.Size = new System.Drawing.Size(328, 22);
            this.txtShipZip.TabIndex = 6;
            ultraToolTipInfo26.ToolTipText = "The Zip Code.";
            ultraToolTipInfo26.ToolTipTitle = "Zip Code";
            this.tipManager.SetUltraToolTip(this.txtShipZip, ultraToolTipInfo26);
            // 
            // txtShipState
            // 
            this.txtShipState.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtShipState.Location = new System.Drawing.Point(287, 87);
            this.txtShipState.MaxLength = 2;
            this.txtShipState.Name = "txtShipState";
            this.txtShipState.NullText = "ST";
            appearance6.ForeColor = System.Drawing.Color.Gainsboro;
            this.txtShipState.NullTextAppearance = appearance6;
            this.txtShipState.Size = new System.Drawing.Size(29, 22);
            this.txtShipState.TabIndex = 5;
            ultraToolTipInfo27.ToolTipText = "The two letter state abbreviation.";
            ultraToolTipInfo27.ToolTipTitle = "State";
            this.tipManager.SetUltraToolTip(this.txtShipState, ultraToolTipInfo27);
            this.txtShipState.ValueChanged += new System.EventHandler(this.txtShipState_ValueChanged);
            // 
            // txtShipCity
            // 
            this.txtShipCity.Location = new System.Drawing.Point(105, 87);
            this.txtShipCity.MaxLength = 50;
            this.txtShipCity.Name = "txtShipCity";
            this.txtShipCity.NullText = "CITY";
            appearance7.ForeColor = System.Drawing.Color.Gainsboro;
            this.txtShipCity.NullTextAppearance = appearance7;
            this.txtShipCity.Size = new System.Drawing.Size(176, 22);
            this.txtShipCity.TabIndex = 4;
            ultraToolTipInfo28.ToolTipText = "The city name.";
            ultraToolTipInfo28.ToolTipTitle = "City";
            this.tipManager.SetUltraToolTip(this.txtShipCity, ultraToolTipInfo28);
            // 
            // txtShipAddress2
            // 
            this.txtShipAddress2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtShipAddress2.Location = new System.Drawing.Point(105, 59);
            this.txtShipAddress2.Name = "txtShipAddress2";
            this.txtShipAddress2.Nullable = false;
            this.txtShipAddress2.Size = new System.Drawing.Size(545, 22);
            this.txtShipAddress2.TabIndex = 3;
            ultraToolTipInfo29.ToolTipText = "Address Line 2";
            ultraToolTipInfo29.ToolTipTitle = "Address Line 2";
            this.tipManager.SetUltraToolTip(this.txtShipAddress2, ultraToolTipInfo29);
            // 
            // ultraLabel14
            // 
            this.ultraLabel14.AutoSize = true;
            this.ultraLabel14.Location = new System.Drawing.Point(3, 35);
            this.ultraLabel14.Name = "ultraLabel14";
            this.ultraLabel14.Size = new System.Drawing.Size(66, 15);
            this.ultraLabel14.TabIndex = 3;
            this.ultraLabel14.Text = "Address 1:";
            // 
            // ultraLabel13
            // 
            this.ultraLabel13.AutoSize = true;
            this.ultraLabel13.Location = new System.Drawing.Point(3, 7);
            this.ultraLabel13.Name = "ultraLabel13";
            this.ultraLabel13.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel13.TabIndex = 2;
            this.ultraLabel13.Text = "Name:";
            // 
            // txtShipAddress1
            // 
            this.txtShipAddress1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtShipAddress1.Location = new System.Drawing.Point(105, 31);
            this.txtShipAddress1.Name = "txtShipAddress1";
            this.txtShipAddress1.Nullable = false;
            this.txtShipAddress1.Size = new System.Drawing.Size(545, 22);
            this.txtShipAddress1.TabIndex = 2;
            ultraToolTipInfo30.ToolTipText = "Address Line 1";
            ultraToolTipInfo30.ToolTipTitle = "Address Line 1";
            this.tipManager.SetUltraToolTip(this.txtShipAddress1, ultraToolTipInfo30);
            // 
            // txtShipName
            // 
            this.txtShipName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtShipName.Location = new System.Drawing.Point(105, 3);
            this.txtShipName.MaxLength = 255;
            this.txtShipName.Name = "txtShipName";
            this.txtShipName.Nullable = false;
            this.txtShipName.Size = new System.Drawing.Size(477, 22);
            this.txtShipName.TabIndex = 0;
            ultraToolTipInfo31.ToolTipText = "The name of the shipping address.";
            ultraToolTipInfo31.ToolTipTitle = "Address Name";
            this.tipManager.SetUltraToolTip(this.txtShipName, ultraToolTipInfo31);
            this.txtShipName.Validated += new System.EventHandler(this.txtShipName_Validated);
            // 
            // bsCustomerAddress
            // 
            this.bsCustomerAddress.Filter = "";
            // 
            // generalLayoutManager
            // 
            this.generalLayoutManager.ContainerControl = this.ultraTabPageControl1;
            this.generalLayoutManager.HorizontalAlignment = Infragistics.Win.Layout.DefaultableFlowLayoutAlignment.Near;
            this.generalLayoutManager.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.generalLayoutManager.VerticalAlignment = Infragistics.Win.Layout.DefaultableFlowLayoutAlignment.Near;
            this.generalLayoutManager.VerticalGap = 0;
            this.generalLayoutManager.WrapItems = false;
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(873, 497);
            // 
            // tabCustomer
            // 
            this.tabCustomer.Controls.Add(this.ultraTabSharedControlsPage1);
            this.tabCustomer.Controls.Add(this.ultraTabPageControl1);
            this.tabCustomer.Controls.Add(this.ultraTabPageControl2);
            this.tabCustomer.Controls.Add(this.ultraTabPageControl3);
            this.tabCustomer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCustomer.Location = new System.Drawing.Point(8, 23);
            this.tabCustomer.Name = "tabCustomer";
            this.tabCustomer.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.tabCustomer.Size = new System.Drawing.Size(887, 533);
            this.tabCustomer.TabIndex = 0;
            this.tabCustomer.TabPageMargins.Bottom = 5;
            this.tabCustomer.TabPageMargins.Left = 5;
            this.tabCustomer.TabPageMargins.Right = 5;
            this.tabCustomer.TabPageMargins.Top = 5;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "General";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "Fields";
            ultraTab3.Key = "ShipTo";
            ultraTab3.TabPage = this.ultraTabPageControl3;
            ultraTab3.Text = "Ship To";
            this.tabCustomer.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2,
            ultraTab3});
            this.tabCustomer.SelectedTabChanged += new Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventHandler(this.tabCustomer_SelectedTabChanged);
            // 
            // CustomerInformation
            // 
            this.Name = "CustomerInformation";
            this.Size = new System.Drawing.Size(909, 565);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            this.ultraTabPageControl1.ResumeLayout(false);
            this.pnlGeneralTop.ClientArea.ResumeLayout(false);
            this.pnlGeneralTop.ClientArea.PerformLayout();
            this.pnlGeneralTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkActive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerName)).EndInit();
            this.pnlAddress.ClientArea.ResumeLayout(false);
            this.pnlAddress.ClientArea.PerformLayout();
            this.pnlAddress.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomerCountry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerZip)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerCity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerState)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerAddress2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerAddress1)).EndInit();
            this.pnlRequireCoc.ClientArea.ResumeLayout(false);
            this.pnlRequireCoc.ClientArea.PerformLayout();
            this.pnlRequireCoc.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkRequireCoc)).EndInit();
            this.pnlGeneralBottom.ClientArea.ResumeLayout(false);
            this.pnlGeneralBottom.ClientArea.PerformLayout();
            this.pnlGeneralBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboShowSNonApprovalSubjectLine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomerStanding)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintCOC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLeadTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboInvoiceLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTerms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAccountingID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPriority)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkOrderReview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEmail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerNotes)).EndInit();
            this.ultraTabPageControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdFields)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsFields)).EndInit();
            this.ultraTabPageControl3.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tvwAddress)).EndInit();
            this.pnlShipTo.ClientArea.ResumeLayout(false);
            this.pnlShipTo.ClientArea.PerformLayout();
            this.pnlShipTo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboShipCountry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShipActive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShipRepairStatement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShipDefault)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipZip)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipState)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipCity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipAddress2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipAddress1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCustomerAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.generalLayoutManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabCustomer)).EndInit();
            this.tabCustomer.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion
		private Infragistics.Win.UltraWinDataSource.UltraDataSource dsFields;
        private System.Windows.Forms.BindingSource bsCustomerAddress;
        private Infragistics.Win.Misc.UltraFlowLayoutManager generalLayoutManager;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl tabCustomer;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.Misc.UltraPanel pnlGeneralTop;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkActive;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomerName;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraPanel pnlAddress;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomerZip;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomerCity;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomerState;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomerAddress2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomerAddress1;
        private Infragistics.Win.Misc.UltraPanel pnlRequireCoc;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkRequireCoc;
        private Infragistics.Win.Misc.UltraPanel pnlGeneralBottom;
        private Infragistics.Win.Misc.UltraButton btnPricePoints;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomerStanding;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPrintCOC;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numLeadTime;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboInvoiceLevel;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.Misc.UltraLabel ultraLabel10;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboTerms;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel11;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtAccountingID;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboPriority;
        private Infragistics.Win.Misc.UltraLabel ultraLabel9;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPrint;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkOrderReview;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkEmail;
        private Infragistics.Win.Misc.UltraLabel ultraLabel12;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomerNotes;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdFields;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Infragistics.Win.UltraWinTree.UltraTree tvwAddress;
        private Infragistics.Win.Misc.UltraPanel pnlShipTo;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkShipActive;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkShipRepairStatement;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkShipDefault;
        private Infragistics.Win.Misc.UltraLabel ultraLabel16;
        private Infragistics.Win.Misc.UltraLabel ultraLabel15;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtShipZip;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtShipState;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtShipCity;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtShipAddress2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel14;
        private Infragistics.Win.Misc.UltraLabel ultraLabel13;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtShipAddress1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtShipName;
        private Infragistics.Win.Misc.UltraButton btnDefaultFees;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomerCountry;
        private Infragistics.Win.Misc.UltraLabel ultraLabel17;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboShipCountry;
        private Infragistics.Win.Misc.UltraLabel ultraLabel18;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor cboShowSNonApprovalSubjectLine;
    }
}
