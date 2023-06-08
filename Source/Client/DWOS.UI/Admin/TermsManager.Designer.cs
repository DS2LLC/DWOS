namespace DWOS.UI.Admin
{
    partial class TermsManager
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("d_Terms", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TermsID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Terms");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Active");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_Quote_d_Terms");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_Quote_d_Terms", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuoteID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CreatedDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Revision");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UserId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ClosedDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ExpirationDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Program");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RFQ");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ContactID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Notes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TermsID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePart_Quote");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand3 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePart_Quote", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuoteID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Quantity");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartMarking");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EachPrice");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LotPrice");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn32 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Length");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Width");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn34 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SurfaceArea");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Height");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShapeType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Weight");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn58 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Notes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn36 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePart_Media_QuotePart_Media");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePartFees_QuotePart");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePartArea_QuotePart");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn35 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePart_Process_QuotePart");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn59 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePart_QuotePart_DocumentLink");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn60 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePartPrice_QuotePart");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand4 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePart_Media_QuotePart_Media", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn40 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn41 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MediaID");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand5 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePartFees_QuotePart", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn37 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartFeeID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn38 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn39 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FeeType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn42 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Charge");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn61 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FeeCalculationType");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand6 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePartArea_QuotePart", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn43 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartAreaID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn44 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn45 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ExclusionSurfaceArea");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn46 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("GrossSurfaceArea");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn47 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShapeType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn48 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePartAreaDimension_QuotePartArea");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand7 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePartAreaDimension_QuotePartArea", 5);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn49 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartAreaDimensionID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn50 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartAreaID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn51 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DimensionName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn52 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Dimension");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand8 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePart_Process_QuotePart", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn53 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn54 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn55 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn56 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StepOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn57 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessAliasID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn62 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePartProcessPrice_QuotePart_Process");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand9 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePartProcessPrice_QuotePart_Process", 7);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn63 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartProcessPriceID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn64 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn65 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PriceUnit");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn66 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Amount");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn67 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MinQuantity");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn68 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MaxQuantity");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand10 = new Infragistics.Win.UltraWinGrid.UltraGridBand("QuotePart_QuotePart_DocumentLink", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn69 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DocumentLinkID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn70 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DocumentInfoID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn71 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LinkToType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn72 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LinkToKey");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand11 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePartPrice_QuotePart", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn73 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartPriceID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn74 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Rate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn75 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LaborCost");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn76 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MaterialCost");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn77 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OverheadCost");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn78 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn79 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MarkupTotal");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn80 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TargetPrice");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn81 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_QuotePartPriceCalculation_QuotePartPrice");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand12 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_QuotePartPriceCalculation_QuotePartPrice", 10);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn82 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartPriceCalculationID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn83 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartPriceID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn84 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Step");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn85 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CalculationType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn86 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Data");
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
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TermsManager));
            this.grdManufacturer = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsQuotes = new DWOS.Data.Datasets.QuoteDataSet();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grdManufacturer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsQuotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            this.SuspendLayout();
            // 
            // grdManufacturer
            // 
            this.grdManufacturer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdManufacturer.DataMember = "d_Terms";
            this.grdManufacturer.DataSource = this.dsQuotes;
            this.grdManufacturer.DisplayLayout.AddNewBox.Hidden = false;
            appearance1.BackColor = System.Drawing.Color.Transparent;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdManufacturer.DisplayLayout.Appearance = appearance1;
            this.grdManufacturer.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridBand1.AddButtonCaption = "Terms";
            ultraGridBand1.CardSettings.CaptionField = "Name";
            ultraGridBand1.CardView = true;
            ultraGridColumn2.Header.VisiblePosition = 0;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn11.Header.VisiblePosition = 1;
            ultraGridColumn11.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn11.RowLayoutColumnInfo.OriginX = 2;
            ultraGridColumn11.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn11.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(236, 0);
            ultraGridColumn11.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(52, 0);
            ultraGridColumn11.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn11.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn11.SupportDataErrorInfo = Infragistics.Win.DefaultableBoolean.True;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn3.RowLayoutColumnInfo.LabelSpan = 3;
            ultraGridColumn3.RowLayoutColumnInfo.OriginX = 0;
            ultraGridColumn3.RowLayoutColumnInfo.OriginY = 2;
            ultraGridColumn3.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(282, 41);
            ultraGridColumn3.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(104, 0);
            ultraGridColumn3.RowLayoutColumnInfo.SpanX = 4;
            ultraGridColumn3.RowLayoutColumnInfo.SpanY = 3;
            ultraGridColumn3.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.FormattedTextEditor;
            ultraGridColumn3.SupportDataErrorInfo = Infragistics.Win.DefaultableBoolean.True;
            ultraGridColumn1.DefaultCellValue = "True";
            appearance2.TextHAlignAsString = "Center";
            ultraGridColumn1.Header.Appearance = appearance2;
            ultraGridColumn1.Header.VisiblePosition = 3;
            ultraGridColumn1.MaxWidth = 50;
            ultraGridColumn1.RowLayoutColumnInfo.OriginX = 0;
            ultraGridColumn1.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn1.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(46, 0);
            ultraGridColumn1.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(52, 0);
            ultraGridColumn1.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn1.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn1.Width = 50;
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn2,
            ultraGridColumn11,
            ultraGridColumn3,
            ultraGridColumn1,
            ultraGridColumn4});
            ultraGridBand1.Expandable = false;
            ultraGridBand1.Override.InvalidValueBehavior = Infragistics.Win.UltraWinGrid.InvalidValueBehavior.RevertValueAndRetainFocus;
            ultraGridBand1.Override.RowSelectorHeaderStyle = Infragistics.Win.UltraWinGrid.RowSelectorHeaderStyle.SeparateElement;
            ultraGridBand1.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFree;
            ultraGridBand1.Override.SupportDataErrorInfo = Infragistics.Win.UltraWinGrid.SupportDataErrorInfo.CellsOnly;
            ultraGridBand1.RowLayoutStyle = Infragistics.Win.UltraWinGrid.RowLayoutStyle.ColumnLayout;
            ultraGridColumn5.Header.VisiblePosition = 0;
            ultraGridColumn6.Header.VisiblePosition = 1;
            ultraGridColumn7.Header.VisiblePosition = 2;
            ultraGridColumn8.Header.VisiblePosition = 3;
            ultraGridColumn9.Header.VisiblePosition = 4;
            ultraGridColumn16.Header.VisiblePosition = 5;
            ultraGridColumn17.Header.VisiblePosition = 6;
            ultraGridColumn18.Header.VisiblePosition = 7;
            ultraGridColumn19.Header.VisiblePosition = 8;
            ultraGridColumn20.Header.VisiblePosition = 9;
            ultraGridColumn21.Header.VisiblePosition = 10;
            ultraGridColumn22.Header.VisiblePosition = 11;
            ultraGridColumn23.Header.VisiblePosition = 12;
            ultraGridColumn24.Header.VisiblePosition = 13;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20,
            ultraGridColumn21,
            ultraGridColumn22,
            ultraGridColumn23,
            ultraGridColumn24});
            ultraGridBand2.Hidden = true;
            ultraGridColumn25.Header.VisiblePosition = 0;
            ultraGridColumn26.Header.VisiblePosition = 1;
            ultraGridColumn27.Header.VisiblePosition = 2;
            ultraGridColumn28.Header.VisiblePosition = 3;
            ultraGridColumn29.Header.VisiblePosition = 4;
            ultraGridColumn30.Header.VisiblePosition = 5;
            ultraGridColumn31.Header.VisiblePosition = 6;
            ultraGridColumn32.Header.VisiblePosition = 7;
            ultraGridColumn33.Header.VisiblePosition = 8;
            ultraGridColumn34.Header.VisiblePosition = 9;
            ultraGridColumn10.Header.VisiblePosition = 10;
            ultraGridColumn12.Header.VisiblePosition = 11;
            ultraGridColumn13.Header.VisiblePosition = 12;
            ultraGridColumn58.Header.VisiblePosition = 13;
            ultraGridColumn36.Header.VisiblePosition = 14;
            ultraGridColumn14.Header.VisiblePosition = 15;
            ultraGridColumn15.Header.VisiblePosition = 16;
            ultraGridColumn35.Header.VisiblePosition = 17;
            ultraGridColumn59.Header.VisiblePosition = 18;
            ultraGridColumn60.Header.VisiblePosition = 19;
            ultraGridBand3.Columns.AddRange(new object[] {
            ultraGridColumn25,
            ultraGridColumn26,
            ultraGridColumn27,
            ultraGridColumn28,
            ultraGridColumn29,
            ultraGridColumn30,
            ultraGridColumn31,
            ultraGridColumn32,
            ultraGridColumn33,
            ultraGridColumn34,
            ultraGridColumn10,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn58,
            ultraGridColumn36,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn35,
            ultraGridColumn59,
            ultraGridColumn60});
            ultraGridBand3.Hidden = true;
            ultraGridColumn40.Header.VisiblePosition = 0;
            ultraGridColumn41.Header.VisiblePosition = 1;
            ultraGridBand4.Columns.AddRange(new object[] {
            ultraGridColumn40,
            ultraGridColumn41});
            ultraGridBand4.Hidden = true;
            ultraGridColumn37.Header.VisiblePosition = 0;
            ultraGridColumn38.Header.VisiblePosition = 1;
            ultraGridColumn39.Header.VisiblePosition = 2;
            ultraGridColumn42.Header.VisiblePosition = 3;
            ultraGridColumn61.Header.VisiblePosition = 4;
            ultraGridBand5.Columns.AddRange(new object[] {
            ultraGridColumn37,
            ultraGridColumn38,
            ultraGridColumn39,
            ultraGridColumn42,
            ultraGridColumn61});
            ultraGridColumn43.Header.VisiblePosition = 0;
            ultraGridColumn44.Header.VisiblePosition = 1;
            ultraGridColumn45.Header.VisiblePosition = 2;
            ultraGridColumn46.Header.VisiblePosition = 3;
            ultraGridColumn47.Header.VisiblePosition = 4;
            ultraGridColumn48.Header.VisiblePosition = 5;
            ultraGridBand6.Columns.AddRange(new object[] {
            ultraGridColumn43,
            ultraGridColumn44,
            ultraGridColumn45,
            ultraGridColumn46,
            ultraGridColumn47,
            ultraGridColumn48});
            ultraGridColumn49.Header.VisiblePosition = 0;
            ultraGridColumn50.Header.VisiblePosition = 1;
            ultraGridColumn51.Header.VisiblePosition = 2;
            ultraGridColumn52.Header.VisiblePosition = 3;
            ultraGridBand7.Columns.AddRange(new object[] {
            ultraGridColumn49,
            ultraGridColumn50,
            ultraGridColumn51,
            ultraGridColumn52});
            ultraGridColumn53.Header.VisiblePosition = 0;
            ultraGridColumn54.Header.VisiblePosition = 1;
            ultraGridColumn55.Header.VisiblePosition = 2;
            ultraGridColumn56.Header.VisiblePosition = 3;
            ultraGridColumn57.Header.VisiblePosition = 4;
            ultraGridColumn62.Header.VisiblePosition = 5;
            ultraGridBand8.Columns.AddRange(new object[] {
            ultraGridColumn53,
            ultraGridColumn54,
            ultraGridColumn55,
            ultraGridColumn56,
            ultraGridColumn57,
            ultraGridColumn62});
            ultraGridColumn63.Header.VisiblePosition = 0;
            ultraGridColumn64.Header.VisiblePosition = 1;
            ultraGridColumn65.Header.VisiblePosition = 2;
            ultraGridColumn66.Header.VisiblePosition = 3;
            ultraGridColumn67.Header.VisiblePosition = 4;
            ultraGridColumn68.Header.VisiblePosition = 5;
            ultraGridBand9.Columns.AddRange(new object[] {
            ultraGridColumn63,
            ultraGridColumn64,
            ultraGridColumn65,
            ultraGridColumn66,
            ultraGridColumn67,
            ultraGridColumn68});
            ultraGridColumn69.Header.VisiblePosition = 0;
            ultraGridColumn70.Header.VisiblePosition = 1;
            ultraGridColumn71.Header.VisiblePosition = 2;
            ultraGridColumn72.Header.VisiblePosition = 3;
            ultraGridBand10.Columns.AddRange(new object[] {
            ultraGridColumn69,
            ultraGridColumn70,
            ultraGridColumn71,
            ultraGridColumn72});
            ultraGridColumn73.Header.VisiblePosition = 0;
            ultraGridColumn74.Header.VisiblePosition = 1;
            ultraGridColumn75.Header.VisiblePosition = 2;
            ultraGridColumn76.Header.VisiblePosition = 3;
            ultraGridColumn77.Header.VisiblePosition = 4;
            ultraGridColumn78.Header.VisiblePosition = 5;
            ultraGridColumn79.Header.VisiblePosition = 6;
            ultraGridColumn80.Header.VisiblePosition = 7;
            ultraGridColumn81.Header.VisiblePosition = 8;
            ultraGridBand11.Columns.AddRange(new object[] {
            ultraGridColumn73,
            ultraGridColumn74,
            ultraGridColumn75,
            ultraGridColumn76,
            ultraGridColumn77,
            ultraGridColumn78,
            ultraGridColumn79,
            ultraGridColumn80,
            ultraGridColumn81});
            ultraGridColumn82.Header.VisiblePosition = 0;
            ultraGridColumn83.Header.VisiblePosition = 1;
            ultraGridColumn84.Header.VisiblePosition = 2;
            ultraGridColumn85.Header.VisiblePosition = 3;
            ultraGridColumn86.Header.VisiblePosition = 4;
            ultraGridBand12.Columns.AddRange(new object[] {
            ultraGridColumn82,
            ultraGridColumn83,
            ultraGridColumn84,
            ultraGridColumn85,
            ultraGridColumn86});
            this.grdManufacturer.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdManufacturer.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdManufacturer.DisplayLayout.BandsSerializer.Add(ultraGridBand3);
            this.grdManufacturer.DisplayLayout.BandsSerializer.Add(ultraGridBand4);
            this.grdManufacturer.DisplayLayout.BandsSerializer.Add(ultraGridBand5);
            this.grdManufacturer.DisplayLayout.BandsSerializer.Add(ultraGridBand6);
            this.grdManufacturer.DisplayLayout.BandsSerializer.Add(ultraGridBand7);
            this.grdManufacturer.DisplayLayout.BandsSerializer.Add(ultraGridBand8);
            this.grdManufacturer.DisplayLayout.BandsSerializer.Add(ultraGridBand9);
            this.grdManufacturer.DisplayLayout.BandsSerializer.Add(ultraGridBand10);
            this.grdManufacturer.DisplayLayout.BandsSerializer.Add(ultraGridBand11);
            this.grdManufacturer.DisplayLayout.BandsSerializer.Add(ultraGridBand12);
            this.grdManufacturer.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance3.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance3.BorderColor = System.Drawing.SystemColors.Window;
            this.grdManufacturer.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdManufacturer.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.grdManufacturer.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdManufacturer.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.grdManufacturer.DisplayLayout.MaxColScrollRegions = 1;
            this.grdManufacturer.DisplayLayout.MaxRowScrollRegions = 1;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            appearance6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdManufacturer.DisplayLayout.Override.ActiveCellAppearance = appearance6;
            appearance7.BackColor = System.Drawing.SystemColors.Highlight;
            appearance7.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdManufacturer.DisplayLayout.Override.ActiveRowAppearance = appearance7;
            this.grdManufacturer.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdManufacturer.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance8.BackColor = System.Drawing.SystemColors.Window;
            this.grdManufacturer.DisplayLayout.Override.CardAreaAppearance = appearance8;
            appearance9.BackColor = System.Drawing.Color.SlateGray;
            appearance9.ForeColor = System.Drawing.Color.White;
            appearance9.TextHAlignAsString = "Center";
            appearance9.TextVAlignAsString = "Middle";
            this.grdManufacturer.DisplayLayout.Override.CardCaptionAppearance = appearance9;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            appearance10.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdManufacturer.DisplayLayout.Override.CellAppearance = appearance10;
            this.grdManufacturer.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            appearance11.BackColor = System.Drawing.SystemColors.Control;
            appearance11.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance11.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance11.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance11.BorderColor = System.Drawing.SystemColors.Window;
            this.grdManufacturer.DisplayLayout.Override.GroupByRowAppearance = appearance11;
            appearance12.BackColor = System.Drawing.Color.SlateGray;
            appearance12.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance12.ForeColor = System.Drawing.Color.White;
            appearance12.TextHAlignAsString = "Left";
            this.grdManufacturer.DisplayLayout.Override.HeaderAppearance = appearance12;
            this.grdManufacturer.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdManufacturer.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance13.BackColor = System.Drawing.SystemColors.Window;
            appearance13.BorderColor = System.Drawing.Color.Silver;
            this.grdManufacturer.DisplayLayout.Override.RowAppearance = appearance13;
            this.grdManufacturer.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.grdManufacturer.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            appearance14.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdManufacturer.DisplayLayout.Override.TemplateAddRowAppearance = appearance14;
            this.grdManufacturer.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdManufacturer.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdManufacturer.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdManufacturer.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdManufacturer.Location = new System.Drawing.Point(12, 12);
            this.grdManufacturer.Name = "grdManufacturer";
            this.grdManufacturer.Size = new System.Drawing.Size(568, 318);
            this.grdManufacturer.TabIndex = 0;
            this.grdManufacturer.UseAppStyling = false;
            this.grdManufacturer.BeforeEnterEditMode += new System.ComponentModel.CancelEventHandler(this.grdManufacturer_BeforeEnterEditMode);
            // 
            // dsQuotes
            // 
            this.dsQuotes.DataSetName = "QuoteDataSet";
            this.dsQuotes.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(502, 342);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(420, 342);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 15;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // TermsManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 377);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grdManufacturer);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TermsManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Terms Manager";
            this.Load += new System.EventHandler(this.ManufacturerManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdManufacturer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsQuotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid grdManufacturer;
		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
        private Data.Datasets.QuoteDataSet dsQuotes;
	}
}