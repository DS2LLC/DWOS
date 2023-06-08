namespace DWOS.UI.Admin
{
    using Infragistics.Win.UltraWinGrid;

    partial class ManufacturerManager
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("d_Manufacturer", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ManufacturerID", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn34 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("COC");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn35 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_Manufacturer_Part");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn36 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_d_Airframe_d_Manufacturer");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("d_Manufacturer_Part", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn37 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn38 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn39 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn40 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Material");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn42 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Revision");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn43 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Notes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn46 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ManufacturerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn47 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Active");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn48 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Airframe");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Length");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Width");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SurfaceArea");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartMarking");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ParentID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AssemblyNumber");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LastModified");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShapeType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Height");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Weight");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LotPrice");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EachPrice");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RequireCocByDefault");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn49 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_Part_Media_Part");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn50 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_PartProcess_Part");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_PartArea_Part");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Part_Part_DocumentLink");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_Part_PartMarking_Part");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand3 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_Part_Media_Part", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn51 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn52 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MediaID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn53 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DefaultMedia");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand4 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_PartProcess_Part", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn54 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn55 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn56 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn57 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StepOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessAliasID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LoadCapacityWeight");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LoadCapacityQuantity");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn58 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_ProcessAnswer_PartProcess");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_PartProcessVolumePrice_PartProcess");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand5 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_ProcessAnswer_PartProcess", 3);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn59 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessAnswerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn60 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessQuestionID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn61 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn62 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Answer");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand6 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_PartProcessVolumePrice_PartProcess", 3);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartProcessVolumePriceID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PriceUnit");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Amount");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn69 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MinValue");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn73 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MaxValue");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand7 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_PartArea_Part", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartAreaID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ExclusionSurfaceArea");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn32 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("GrossSurfaceArea");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn41 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShapeType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn44 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_PartAreaDimension_PartArea");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand8 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_PartAreaDimension_PartArea", 6);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn45 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartAreaDimensionID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn65 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartAreaID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn66 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DimensionName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn67 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Dimension");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand9 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Part_Part_DocumentLink", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn68 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DocumentLinkID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn70 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DocumentInfoID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn71 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LinkToType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn72 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LinkToKey");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand10 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_Part_PartMarking_Part", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn74 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Part_PartMarkingID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn75 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn76 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessSpec");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn77 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Def1");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn78 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Def2");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn79 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Def3");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn80 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Def4");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand11 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_d_Airframe_d_Manufacturer", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn63 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AirframeID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn64 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ManufacturerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartPrefix");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsActive");
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManufacturerManager));
            this.taManufacturer = new DWOS.Data.Datasets.PartsDatasetTableAdapters.d_ManufacturerTableAdapter();
            this.grdManufacturer = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsParts = new DWOS.Data.Datasets.PartsDataset();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            ((System.ComponentModel.ISupportInitialize)(this.grdManufacturer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsParts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            this.SuspendLayout();
            // 
            // taManufacturer
            // 
            this.taManufacturer.ClearBeforeFill = true;
            // 
            // grdManufacturer
            // 
            this.grdManufacturer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdManufacturer.DataMember = "d_Manufacturer";
            this.grdManufacturer.DataSource = this.dsParts;
            this.grdManufacturer.DisplayLayout.AddNewBox.Hidden = false;
            appearance1.BackColor = System.Drawing.Color.Transparent;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdManufacturer.DisplayLayout.Appearance = appearance1;
            this.grdManufacturer.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridBand1.AddButtonCaption = "Manufacturer";
            ultraGridColumn33.Header.Caption = "Manufacturer";
            ultraGridColumn33.Header.VisiblePosition = 0;
            ultraGridColumn33.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn33.RowLayoutColumnInfo.OriginX = 0;
            ultraGridColumn33.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn33.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(0, 61);
            ultraGridColumn33.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn33.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn34.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
            ultraGridColumn34.Header.VisiblePosition = 1;
            ultraGridColumn34.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn34.RowLayoutColumnInfo.OriginX = 2;
            ultraGridColumn34.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn34.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(0, 61);
            ultraGridColumn34.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn34.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn34.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.FormattedTextEditor;
            ultraGridColumn35.Header.VisiblePosition = 2;
            ultraGridColumn36.Header.VisiblePosition = 3;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn33,
            ultraGridColumn34,
            ultraGridColumn35,
            ultraGridColumn36});
            ultraGridBand1.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFree;
            ultraGridColumn37.Header.VisiblePosition = 0;
            ultraGridColumn38.Header.VisiblePosition = 1;
            ultraGridColumn39.Header.VisiblePosition = 2;
            ultraGridColumn40.Header.VisiblePosition = 3;
            ultraGridColumn42.Header.VisiblePosition = 4;
            ultraGridColumn43.Header.VisiblePosition = 5;
            ultraGridColumn46.Header.VisiblePosition = 6;
            ultraGridColumn47.Header.VisiblePosition = 7;
            ultraGridColumn48.Header.VisiblePosition = 8;
            ultraGridColumn1.Header.VisiblePosition = 9;
            ultraGridColumn2.Header.VisiblePosition = 10;
            ultraGridColumn3.Header.VisiblePosition = 11;
            ultraGridColumn4.Header.VisiblePosition = 12;
            ultraGridColumn5.Header.VisiblePosition = 13;
            ultraGridColumn6.Header.VisiblePosition = 14;
            ultraGridColumn7.Header.VisiblePosition = 15;
            ultraGridColumn10.Header.VisiblePosition = 16;
            ultraGridColumn11.Header.VisiblePosition = 17;
            ultraGridColumn12.Header.VisiblePosition = 19;
            ultraGridColumn14.Header.VisiblePosition = 18;
            ultraGridColumn15.Header.VisiblePosition = 20;
            ultraGridColumn16.Header.VisiblePosition = 21;
            ultraGridColumn17.Header.VisiblePosition = 22;
            ultraGridColumn27.Header.VisiblePosition = 23;
            ultraGridColumn49.Header.VisiblePosition = 24;
            ultraGridColumn50.Header.VisiblePosition = 25;
            ultraGridColumn18.Header.VisiblePosition = 26;
            ultraGridColumn19.Header.VisiblePosition = 27;
            ultraGridColumn28.Header.VisiblePosition = 28;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn37,
            ultraGridColumn38,
            ultraGridColumn39,
            ultraGridColumn40,
            ultraGridColumn42,
            ultraGridColumn43,
            ultraGridColumn46,
            ultraGridColumn47,
            ultraGridColumn48,
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn27,
            ultraGridColumn49,
            ultraGridColumn50,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn28});
            ultraGridBand2.Hidden = true;
            ultraGridColumn51.Header.VisiblePosition = 0;
            ultraGridColumn52.Header.VisiblePosition = 1;
            ultraGridColumn53.Header.VisiblePosition = 2;
            ultraGridBand3.Columns.AddRange(new object[] {
            ultraGridColumn51,
            ultraGridColumn52,
            ultraGridColumn53});
            ultraGridBand3.Hidden = true;
            ultraGridColumn54.Header.VisiblePosition = 0;
            ultraGridColumn55.Header.VisiblePosition = 1;
            ultraGridColumn56.Header.VisiblePosition = 2;
            ultraGridColumn57.Header.VisiblePosition = 3;
            ultraGridColumn8.Header.VisiblePosition = 4;
            ultraGridColumn20.Header.VisiblePosition = 5;
            ultraGridColumn21.Header.VisiblePosition = 6;
            ultraGridColumn58.Header.VisiblePosition = 7;
            ultraGridColumn22.Header.VisiblePosition = 8;
            ultraGridBand4.Columns.AddRange(new object[] {
            ultraGridColumn54,
            ultraGridColumn55,
            ultraGridColumn56,
            ultraGridColumn57,
            ultraGridColumn8,
            ultraGridColumn20,
            ultraGridColumn21,
            ultraGridColumn58,
            ultraGridColumn22});
            ultraGridBand4.Hidden = true;
            ultraGridColumn59.Header.VisiblePosition = 0;
            ultraGridColumn60.Header.VisiblePosition = 1;
            ultraGridColumn61.Header.VisiblePosition = 2;
            ultraGridColumn62.Header.VisiblePosition = 3;
            ultraGridBand5.Columns.AddRange(new object[] {
            ultraGridColumn59,
            ultraGridColumn60,
            ultraGridColumn61,
            ultraGridColumn62});
            ultraGridBand5.Hidden = true;
            ultraGridColumn23.Header.VisiblePosition = 0;
            ultraGridColumn24.Header.VisiblePosition = 1;
            ultraGridColumn25.Header.VisiblePosition = 2;
            ultraGridColumn26.Header.VisiblePosition = 3;
            ultraGridColumn69.Header.VisiblePosition = 4;
            ultraGridColumn73.Header.VisiblePosition = 5;
            ultraGridBand6.Columns.AddRange(new object[] {
            ultraGridColumn23,
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn26,
            ultraGridColumn69,
            ultraGridColumn73});
            ultraGridColumn29.Header.VisiblePosition = 0;
            ultraGridColumn30.Header.VisiblePosition = 1;
            ultraGridColumn31.Header.VisiblePosition = 2;
            ultraGridColumn32.Header.VisiblePosition = 3;
            ultraGridColumn41.Header.VisiblePosition = 4;
            ultraGridColumn44.Header.VisiblePosition = 5;
            ultraGridBand7.Columns.AddRange(new object[] {
            ultraGridColumn29,
            ultraGridColumn30,
            ultraGridColumn31,
            ultraGridColumn32,
            ultraGridColumn41,
            ultraGridColumn44});
            ultraGridColumn45.Header.VisiblePosition = 0;
            ultraGridColumn65.Header.VisiblePosition = 1;
            ultraGridColumn66.Header.VisiblePosition = 2;
            ultraGridColumn67.Header.VisiblePosition = 3;
            ultraGridBand8.Columns.AddRange(new object[] {
            ultraGridColumn45,
            ultraGridColumn65,
            ultraGridColumn66,
            ultraGridColumn67});
            ultraGridColumn68.Header.VisiblePosition = 0;
            ultraGridColumn70.Header.VisiblePosition = 1;
            ultraGridColumn71.Header.VisiblePosition = 2;
            ultraGridColumn72.Header.VisiblePosition = 3;
            ultraGridBand9.Columns.AddRange(new object[] {
            ultraGridColumn68,
            ultraGridColumn70,
            ultraGridColumn71,
            ultraGridColumn72});
            ultraGridColumn74.Header.VisiblePosition = 0;
            ultraGridColumn75.Header.VisiblePosition = 1;
            ultraGridColumn76.Header.VisiblePosition = 2;
            ultraGridColumn77.Header.VisiblePosition = 3;
            ultraGridColumn78.Header.VisiblePosition = 4;
            ultraGridColumn79.Header.VisiblePosition = 5;
            ultraGridColumn80.Header.VisiblePosition = 6;
            ultraGridBand10.Columns.AddRange(new object[] {
            ultraGridColumn74,
            ultraGridColumn75,
            ultraGridColumn76,
            ultraGridColumn77,
            ultraGridColumn78,
            ultraGridColumn79,
            ultraGridColumn80});
            ultraGridColumn63.Header.VisiblePosition = 0;
            ultraGridColumn64.Header.VisiblePosition = 1;
            ultraGridColumn9.Header.VisiblePosition = 2;
            ultraGridColumn13.Header.VisiblePosition = 3;
            ultraGridBand11.Columns.AddRange(new object[] {
            ultraGridColumn63,
            ultraGridColumn64,
            ultraGridColumn9,
            ultraGridColumn13});
            ultraGridBand11.Hidden = true;
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
            this.grdManufacturer.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.grdManufacturer.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdManufacturer.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.grdManufacturer.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdManufacturer.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.grdManufacturer.DisplayLayout.MaxColScrollRegions = 1;
            this.grdManufacturer.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdManufacturer.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdManufacturer.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grdManufacturer.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdManufacturer.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.grdManufacturer.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdManufacturer.DisplayLayout.Override.CellAppearance = appearance8;
            this.grdManufacturer.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdManufacturer.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.grdManufacturer.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.grdManufacturer.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.grdManufacturer.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdManufacturer.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdManufacturer.DisplayLayout.Override.SupportDataErrorInfo = Infragistics.Win.UltraWinGrid.SupportDataErrorInfo.RowsAndCells;
            appearance11.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdManufacturer.DisplayLayout.Override.TemplateAddRowAppearance = appearance11;
            this.grdManufacturer.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdManufacturer.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdManufacturer.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdManufacturer.Location = new System.Drawing.Point(12, 12);
            this.grdManufacturer.Name = "grdManufacturer";
            this.grdManufacturer.Size = new System.Drawing.Size(568, 318);
            this.grdManufacturer.TabIndex = 0;
            this.grdManufacturer.Text = "ultraGrid1";
            this.grdManufacturer.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdManufacturer_AfterCellUpdate);
            this.grdManufacturer.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdManufacturer_InitializeLayout);
            this.grdManufacturer.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.grdManufacturer_BeforeCellUpdate);
            this.grdManufacturer.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.grdManufacturer_BeforeRowsDeleted);
            // 
            // dsParts
            // 
            this.dsParts.DataSetName = "PartsDataset";
            this.dsParts.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
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
            // helpLink1
            // 
            this.helpLink1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = "manufacturer_manager_dialog.htm";
            this.helpLink1.Location = new System.Drawing.Point(12, 349);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 17;
            // 
            // ManufacturerManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 377);
            this.Controls.Add(this.helpLink1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grdManufacturer);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "ManufacturerManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Manufacturer Manager";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ManufacturerManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdManufacturer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsParts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private DWOS.Data.Datasets.PartsDatasetTableAdapters.d_ManufacturerTableAdapter taManufacturer;
		private Infragistics.Win.UltraWinGrid.UltraGrid grdManufacturer;
		private DWOS.Data.Datasets.PartsDataset dsParts;
		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
        private Utilities.HelpLink helpLink1;
	}
}