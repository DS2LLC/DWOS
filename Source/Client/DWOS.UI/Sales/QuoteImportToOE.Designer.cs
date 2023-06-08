namespace DWOS.UI.Admin
{
    partial class ImportQuoteToOE
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
            if (disposing && (components != null))
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("QuoteSearch", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuoteID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CreatedDate", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Program");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RFQ");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ContactName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ClosedDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuoteSearch_QuotePart1");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("QuoteSearch_QuotePart1", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuoteID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Quantity");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePartArea_QuotePart1");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePart_Process_QuotePart1");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePartPrice_QuotePart1");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePartFees_QuotePart1");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand3 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePartArea_QuotePart1", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartAreaID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ExclusionSurfaceArea");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("GrossSurfaceArea");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShapeType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePartAreaDimension_QuotePartArea");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand4 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePartAreaDimension_QuotePartArea", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartAreaDimensionID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartAreaID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DimensionName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Dimension");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand5 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePart_Process_QuotePart1", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StepOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn32 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessAliasID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePartProcessPrice_QuotePart_Process");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand6 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePartProcessPrice_QuotePart_Process", 4);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn34 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartProcessPriceID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn35 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn36 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PriceUnit");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn37 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Amount");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn59 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MinValue");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn60 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MaxValue");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand7 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePartPrice_QuotePart1", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn40 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartPriceID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn41 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Rate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn42 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LaborCost");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn43 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MaterialCost");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn44 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OverheadCost");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn45 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn46 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MarkupTotal");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn58 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TargetPrice");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn47 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePartPriceCalculation_QuotePartPrice");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand8 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePartPriceCalculation_QuotePartPrice", 6);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn48 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartPriceCalculationID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn49 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartPriceID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn50 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Step");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn51 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CalculationType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn52 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Data");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand9 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePartFees_QuotePart1", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn53 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartFeeID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn54 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn55 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FeeType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn56 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Charge");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn57 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FeeCalculationType");
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The value to search for.", Infragistics.Win.ToolTipImage.Default, "Value", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Active Only", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Go to the selected quote in quote manager.", Infragistics.Win.ToolTipImage.Default, "Go To", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The field to search for.", Infragistics.Win.ToolTipImage.Default, "Field", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportQuoteToOE));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.grdParts = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsQuotes = new DWOS.Data.Datasets.QuoteDataSet();
            this.txtSearch = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.btnSearch = new Infragistics.Win.Misc.UltraButton();
            this.chkActiveOnly = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.btnGoTo = new Infragistics.Win.Misc.UltraButton();
            this.cboQuoteSearchField = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.lblRecordCount = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.taQuoteSearch = new DWOS.Data.Datasets.QuoteDataSetTableAdapters.QuoteSearchTableAdapter();
            this.taQuotePartSearch = new DWOS.Data.Datasets.QuoteDataSetTableAdapters.QuotePartSearchTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdParts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsQuotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActiveOnly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboQuoteSearchField)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(717, 356);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 23);
            this.btnCancel.TabIndex = 22;
            this.btnCancel.Text = "&Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(622, 356);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 21;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grdParts
            // 
            this.grdParts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdParts.DataMember = "QuoteSearch";
            this.grdParts.DataSource = this.dsQuotes;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdParts.DisplayLayout.Appearance = appearance1;
            this.grdParts.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.Caption = "Quote ID";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 63;
            ultraGridColumn2.Header.Caption = "Created Date";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 90;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 94;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Width = 94;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Width = 97;
            ultraGridColumn6.Header.Caption = "Customer Name";
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn6.Width = 102;
            ultraGridColumn7.Header.Caption = "Contact Name";
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn7.Width = 97;
            ultraGridColumn13.Header.VisiblePosition = 7;
            ultraGridColumn13.Width = 87;
            ultraGridColumn8.Header.VisiblePosition = 8;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn13,
            ultraGridColumn8});
            ultraGridColumn9.Header.VisiblePosition = 0;
            ultraGridColumn9.Hidden = true;
            ultraGridColumn9.Width = 192;
            ultraGridColumn10.Header.VisiblePosition = 1;
            ultraGridColumn10.Hidden = true;
            ultraGridColumn10.Width = 199;
            ultraGridColumn11.Header.VisiblePosition = 2;
            ultraGridColumn11.Width = 427;
            ultraGridColumn12.Header.VisiblePosition = 3;
            ultraGridColumn12.Width = 278;
            ultraGridColumn14.Header.VisiblePosition = 4;
            ultraGridColumn15.Header.VisiblePosition = 5;
            ultraGridColumn16.Header.VisiblePosition = 6;
            ultraGridColumn17.Header.VisiblePosition = 7;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17});
            ultraGridColumn18.Header.VisiblePosition = 0;
            ultraGridColumn18.Width = 140;
            ultraGridColumn19.Header.VisiblePosition = 1;
            ultraGridColumn19.Width = 108;
            ultraGridColumn20.Header.VisiblePosition = 2;
            ultraGridColumn20.Width = 168;
            ultraGridColumn21.Header.VisiblePosition = 3;
            ultraGridColumn21.Width = 144;
            ultraGridColumn22.Header.VisiblePosition = 4;
            ultraGridColumn22.Width = 126;
            ultraGridColumn23.Header.VisiblePosition = 5;
            ultraGridBand3.Columns.AddRange(new object[] {
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20,
            ultraGridColumn21,
            ultraGridColumn22,
            ultraGridColumn23});
            ultraGridColumn24.Header.VisiblePosition = 0;
            ultraGridColumn24.Width = 239;
            ultraGridColumn25.Header.VisiblePosition = 1;
            ultraGridColumn25.Width = 157;
            ultraGridColumn26.Header.VisiblePosition = 2;
            ultraGridColumn26.Width = 152;
            ultraGridColumn27.Header.VisiblePosition = 3;
            ultraGridColumn27.Width = 119;
            ultraGridBand4.Columns.AddRange(new object[] {
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn26,
            ultraGridColumn27});
            ultraGridColumn28.Header.VisiblePosition = 0;
            ultraGridColumn28.Width = 191;
            ultraGridColumn29.Header.VisiblePosition = 1;
            ultraGridColumn29.Width = 129;
            ultraGridColumn30.Header.VisiblePosition = 2;
            ultraGridColumn30.Width = 109;
            ultraGridColumn31.Header.VisiblePosition = 3;
            ultraGridColumn31.Width = 112;
            ultraGridColumn32.Header.VisiblePosition = 4;
            ultraGridColumn32.Width = 145;
            ultraGridColumn33.Header.VisiblePosition = 5;
            ultraGridBand5.Columns.AddRange(new object[] {
            ultraGridColumn28,
            ultraGridColumn29,
            ultraGridColumn30,
            ultraGridColumn31,
            ultraGridColumn32,
            ultraGridColumn33});
            ultraGridColumn34.Header.VisiblePosition = 0;
            ultraGridColumn34.Width = 134;
            ultraGridColumn35.Header.VisiblePosition = 1;
            ultraGridColumn35.Width = 117;
            ultraGridColumn36.Header.VisiblePosition = 2;
            ultraGridColumn36.Width = 95;
            ultraGridColumn37.Header.VisiblePosition = 3;
            ultraGridColumn37.Width = 79;
            ultraGridColumn59.Header.VisiblePosition = 4;
            ultraGridColumn59.Width = 121;
            ultraGridColumn60.Header.VisiblePosition = 5;
            ultraGridColumn60.Width = 121;
            ultraGridBand6.Columns.AddRange(new object[] {
            ultraGridColumn34,
            ultraGridColumn35,
            ultraGridColumn36,
            ultraGridColumn37,
            ultraGridColumn59,
            ultraGridColumn60});
            ultraGridColumn40.Header.VisiblePosition = 0;
            ultraGridColumn40.Width = 110;
            ultraGridColumn41.Header.VisiblePosition = 1;
            ultraGridColumn41.Width = 60;
            ultraGridColumn42.Header.VisiblePosition = 2;
            ultraGridColumn42.Width = 83;
            ultraGridColumn43.Header.VisiblePosition = 3;
            ultraGridColumn43.Width = 86;
            ultraGridColumn44.Header.VisiblePosition = 4;
            ultraGridColumn44.Width = 94;
            ultraGridColumn45.Header.VisiblePosition = 5;
            ultraGridColumn45.Width = 86;
            ultraGridColumn46.Header.VisiblePosition = 6;
            ultraGridColumn46.Width = 86;
            ultraGridColumn58.Header.VisiblePosition = 7;
            ultraGridColumn58.Width = 81;
            ultraGridColumn47.Header.VisiblePosition = 8;
            ultraGridBand7.Columns.AddRange(new object[] {
            ultraGridColumn40,
            ultraGridColumn41,
            ultraGridColumn42,
            ultraGridColumn43,
            ultraGridColumn44,
            ultraGridColumn45,
            ultraGridColumn46,
            ultraGridColumn58,
            ultraGridColumn47});
            ultraGridColumn48.Header.VisiblePosition = 0;
            ultraGridColumn48.Width = 193;
            ultraGridColumn49.Header.VisiblePosition = 1;
            ultraGridColumn49.Width = 126;
            ultraGridColumn50.Header.VisiblePosition = 2;
            ultraGridColumn50.Width = 115;
            ultraGridColumn51.Header.VisiblePosition = 3;
            ultraGridColumn51.Width = 118;
            ultraGridColumn52.Header.VisiblePosition = 4;
            ultraGridColumn52.Width = 115;
            ultraGridBand8.Columns.AddRange(new object[] {
            ultraGridColumn48,
            ultraGridColumn49,
            ultraGridColumn50,
            ultraGridColumn51,
            ultraGridColumn52});
            ultraGridColumn53.Header.VisiblePosition = 0;
            ultraGridColumn53.Width = 146;
            ultraGridColumn54.Header.VisiblePosition = 1;
            ultraGridColumn54.Width = 118;
            ultraGridColumn55.Header.VisiblePosition = 2;
            ultraGridColumn55.Width = 139;
            ultraGridColumn56.Header.VisiblePosition = 3;
            ultraGridColumn56.Width = 115;
            ultraGridColumn57.Header.VisiblePosition = 4;
            ultraGridColumn57.Width = 168;
            ultraGridBand9.Columns.AddRange(new object[] {
            ultraGridColumn53,
            ultraGridColumn54,
            ultraGridColumn55,
            ultraGridColumn56,
            ultraGridColumn57});
            this.grdParts.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdParts.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdParts.DisplayLayout.BandsSerializer.Add(ultraGridBand3);
            this.grdParts.DisplayLayout.BandsSerializer.Add(ultraGridBand4);
            this.grdParts.DisplayLayout.BandsSerializer.Add(ultraGridBand5);
            this.grdParts.DisplayLayout.BandsSerializer.Add(ultraGridBand6);
            this.grdParts.DisplayLayout.BandsSerializer.Add(ultraGridBand7);
            this.grdParts.DisplayLayout.BandsSerializer.Add(ultraGridBand8);
            this.grdParts.DisplayLayout.BandsSerializer.Add(ultraGridBand9);
            this.grdParts.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdParts.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.grdParts.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdParts.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.grdParts.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdParts.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.grdParts.DisplayLayout.MaxColScrollRegions = 1;
            this.grdParts.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdParts.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdParts.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grdParts.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdParts.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdParts.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdParts.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.grdParts.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdParts.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.grdParts.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdParts.DisplayLayout.Override.CellAppearance = appearance8;
            this.grdParts.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.grdParts.DisplayLayout.Override.CellPadding = 0;
            this.grdParts.DisplayLayout.Override.ExpansionIndicator = Infragistics.Win.UltraWinGrid.ShowExpansionIndicator.CheckOnExpand;
            this.grdParts.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.grdParts.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.grdParts.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.grdParts.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdParts.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.grdParts.DisplayLayout.Override.RowAppearance = appearance11;
            this.grdParts.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdParts.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.grdParts.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdParts.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdParts.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdParts.Location = new System.Drawing.Point(44, 42);
            this.grdParts.Name = "grdParts";
            this.grdParts.Size = new System.Drawing.Size(764, 308);
            this.grdParts.SyncWithCurrencyManager = false;
            this.grdParts.TabIndex = 23;
            this.grdParts.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnUpdate;
            this.grdParts.BeforeRowExpanded += new Infragistics.Win.UltraWinGrid.CancelableRowEventHandler(this.grdParts_BeforeRowExpanded);
            // 
            // dsQuotes
            // 
            this.dsQuotes.DataSetName = "QuoteDataSet";
            this.dsQuotes.EnforceConstraints = false;
            this.dsQuotes.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(265, 12);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(124, 26);
            this.txtSearch.TabIndex = 26;
            ultraToolTipInfo4.ToolTipText = "The value to search for.";
            ultraToolTipInfo4.ToolTipTitle = "Value";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtSearch, ultraToolTipInfo4);
            this.txtSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyUp);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(12, 16);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(45, 19);
            this.ultraLabel3.TabIndex = 27;
            this.ultraLabel3.Text = "Field:";
            // 
            // btnSearch
            // 
            appearance13.Image = global::DWOS.UI.Properties.Resources.Search_16;
            this.btnSearch.Appearance = appearance13;
            this.btnSearch.AutoSize = true;
            this.btnSearch.Location = new System.Drawing.Point(395, 10);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 29);
            this.btnSearch.TabIndex = 28;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // chkActiveOnly
            // 
            this.chkActiveOnly.AutoSize = true;
            this.chkActiveOnly.Checked = true;
            this.chkActiveOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActiveOnly.Location = new System.Drawing.Point(481, 14);
            this.chkActiveOnly.Name = "chkActiveOnly";
            this.chkActiveOnly.Size = new System.Drawing.Size(106, 22);
            this.chkActiveOnly.TabIndex = 29;
            this.chkActiveOnly.Text = "Active Only";
            ultraToolTipInfo2.ToolTipTextFormatted = "If <strong>checked</strong>, will only search active quotes.<br />If <strong>unch" +
    "ecked</strong>, will search all quotes.";
            ultraToolTipInfo2.ToolTipTitle = "Active Only";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkActiveOnly, ultraToolTipInfo2);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            this.ultraToolTipManager1.ToolTipImage = Infragistics.Win.ToolTipImage.Info;
            // 
            // btnGoTo
            // 
            appearance14.Image = global::DWOS.UI.Properties.Resources.Run;
            this.btnGoTo.Appearance = appearance14;
            this.btnGoTo.AutoSize = true;
            this.btnGoTo.Location = new System.Drawing.Point(12, 55);
            this.btnGoTo.Name = "btnGoTo";
            this.btnGoTo.Size = new System.Drawing.Size(26, 26);
            this.btnGoTo.TabIndex = 25;
            ultraToolTipInfo3.ToolTipText = "Go to the selected quote in quote manager.";
            ultraToolTipInfo3.ToolTipTitle = "Go To";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnGoTo, ultraToolTipInfo3);
            this.btnGoTo.Click += new System.EventHandler(this.btnGoTo_Click);
            // 
            // cboQuoteSearchField
            // 
            this.cboQuoteSearchField.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboQuoteSearchField.Location = new System.Drawing.Point(54, 12);
            this.cboQuoteSearchField.Name = "cboQuoteSearchField";
            this.cboQuoteSearchField.Size = new System.Drawing.Size(158, 26);
            this.cboQuoteSearchField.TabIndex = 31;
            ultraToolTipInfo1.ToolTipText = "The field to search for.";
            ultraToolTipInfo1.ToolTipTitle = "Field";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboQuoteSearchField, ultraToolTipInfo1);
            // 
            // lblRecordCount
            // 
            this.lblRecordCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRecordCount.AutoSize = true;
            this.lblRecordCount.Location = new System.Drawing.Point(44, 361);
            this.lblRecordCount.Name = "lblRecordCount";
            this.lblRecordCount.Size = new System.Drawing.Size(122, 19);
            this.lblRecordCount.TabIndex = 30;
            this.lblRecordCount.Text = "Record Count: 0";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(218, 15);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(51, 19);
            this.ultraLabel1.TabIndex = 32;
            this.ultraLabel1.Text = "Value:";
            // 
            // taQuoteSearch
            // 
            this.taQuoteSearch.ClearBeforeFill = true;
            // 
            // taQuotePartSearch
            // 
            this.taQuotePartSearch.ClearBeforeFill = true;
            // 
            // ImportQuoteToOE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 391);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.cboQuoteSearchField);
            this.Controls.Add(this.lblRecordCount);
            this.Controls.Add(this.chkActiveOnly);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.btnGoTo);
            this.Controls.Add(this.grdParts);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ImportQuoteToOE";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Quote Search";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.QuoteSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdParts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsQuotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActiveOnly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboQuoteSearchField)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdParts;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSearch;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraButton btnSearch;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkActiveOnly;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.Misc.UltraLabel lblRecordCount;
        private Infragistics.Win.Misc.UltraButton btnGoTo;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboQuoteSearchField;
        private Data.Datasets.QuoteDataSet dsQuotes;
        private Data.Datasets.QuoteDataSetTableAdapters.QuoteSearchTableAdapter taQuoteSearch;
        private Data.Datasets.QuoteDataSetTableAdapters.QuotePartSearchTableAdapter taQuotePartSearch;
    }
}