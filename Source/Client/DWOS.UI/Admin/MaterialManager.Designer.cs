namespace DWOS.UI.Admin
{
	partial class MaterialManager
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("d_Material", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MaterialID", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsActive");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_Part_d_Material");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartCount", 0);
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_Part_d_Material", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Material");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Revision");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Notes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ManufacturerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Active");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Airframe");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Length");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Width");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SurfaceArea");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartMarking");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ParentID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AssemblyNumber");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LastModified");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShapeType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Height");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Weight");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LotPrice");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EachPrice");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn39 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RequireCocByDefault");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_Part_Media_Part");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_PartProcess_Part");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn51 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_PartArea_Part");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn52 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Part_Part_DocumentLink");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn44 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_Part_PartMarking_Part");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand3 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_Part_Media_Part", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MediaID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn32 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DefaultMedia");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand4 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_PartProcess_Part", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn34 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn35 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn36 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StepOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn37 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessAliasID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn53 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LoadCapacityWeight");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn54 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LoadCapacityQuantity");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn38 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_ProcessAnswer_PartProcess");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn55 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_PartProcessVolumePrice_PartProcess");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand5 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_ProcessAnswer_PartProcess", 3);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn40 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessAnswerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn41 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessQuestionID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn42 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn43 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Answer");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand6 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_PartProcessVolumePrice_PartProcess", 3);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn45 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartProcessVolumePriceID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn46 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn47 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PriceUnit");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn48 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Amount");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn49 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MinValue");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn50 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MaxValue");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand7 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_PartArea_Part", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn56 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartAreaID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn57 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn58 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ExclusionSurfaceArea");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn59 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("GrossSurfaceArea");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn60 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShapeType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn61 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_PartAreaDimension_PartArea");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand8 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_PartAreaDimension_PartArea", 6);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn62 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartAreaDimensionID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn63 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartAreaID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn64 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DimensionName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn65 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Dimension");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand9 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Part_Part_DocumentLink", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn66 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DocumentLinkID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn67 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DocumentInfoID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn68 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LinkToType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn69 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LinkToKey");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand10 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_Part_PartMarking_Part", 1);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MaterialManager));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.taMaterial = new DWOS.Data.Datasets.PartsDatasetTableAdapters.d_MaterialTableAdapter();
            this.dsParts = new DWOS.Data.Datasets.PartsDataset();
            this.grdMaterial = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnMerge = new Infragistics.Win.Misc.UltraButton();
            this.taParts = new DWOS.Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter();
            this.taManagerParts = new DWOS.Data.Datasets.PartsDatasetTableAdapters.TableAdapterManager();
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            ((System.ComponentModel.ISupportInitialize)(this.dsParts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdMaterial)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(323, 351);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(241, 351);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // taMaterial
            // 
            this.taMaterial.ClearBeforeFill = true;
            // 
            // dsParts
            // 
            this.dsParts.DataSetName = "PartsDataset";
            this.dsParts.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // grdMaterial
            // 
            this.grdMaterial.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdMaterial.DataMember = "d_Material";
            this.grdMaterial.DataSource = this.dsParts;
            this.grdMaterial.DisplayLayout.AddNewBox.Hidden = false;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdMaterial.DisplayLayout.Appearance = appearance1;
            this.grdMaterial.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridBand1.AddButtonCaption = "Material";
            ultraGridColumn1.Header.Caption = "Material";
            ultraGridColumn1.Header.ToolTipText = "The material name.";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.MinLength = 1;
            ultraGridColumn1.MinValue = "";
            ultraGridColumn1.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn1.SupportDataErrorInfo = Infragistics.Win.DefaultableBoolean.True;
            ultraGridColumn1.Width = 179;
            ultraGridColumn2.Header.Caption = "Active";
            ultraGridColumn2.Header.ToolTipText = "If checked, the material is still active and usable.";
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn3.Header.VisiblePosition = 3;
            ultraGridColumn4.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn4.Header.Caption = "Usage Count";
            ultraGridColumn4.Header.ToolTipText = "The number of times the material is used by parts.";
            ultraGridColumn4.Header.VisiblePosition = 1;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4});
            ultraGridBand1.Override.SupportDataErrorInfo = Infragistics.Win.UltraWinGrid.SupportDataErrorInfo.RowsAndCells;
            ultraGridColumn5.Header.VisiblePosition = 0;
            ultraGridColumn6.Header.VisiblePosition = 1;
            ultraGridColumn7.Header.VisiblePosition = 2;
            ultraGridColumn8.Header.VisiblePosition = 3;
            ultraGridColumn9.Header.VisiblePosition = 4;
            ultraGridColumn10.Header.VisiblePosition = 5;
            ultraGridColumn11.Header.VisiblePosition = 6;
            ultraGridColumn12.Header.VisiblePosition = 7;
            ultraGridColumn13.Header.VisiblePosition = 8;
            ultraGridColumn14.Header.VisiblePosition = 9;
            ultraGridColumn15.Header.VisiblePosition = 10;
            ultraGridColumn16.Header.VisiblePosition = 11;
            ultraGridColumn17.Header.VisiblePosition = 12;
            ultraGridColumn18.Header.VisiblePosition = 14;
            ultraGridColumn19.Header.VisiblePosition = 13;
            ultraGridColumn20.Header.VisiblePosition = 15;
            ultraGridColumn21.Header.VisiblePosition = 16;
            ultraGridColumn22.Header.VisiblePosition = 17;
            ultraGridColumn23.Header.VisiblePosition = 19;
            ultraGridColumn24.Header.VisiblePosition = 18;
            ultraGridColumn25.Header.VisiblePosition = 20;
            ultraGridColumn26.Header.VisiblePosition = 21;
            ultraGridColumn27.Header.VisiblePosition = 22;
            ultraGridColumn39.Header.VisiblePosition = 23;
            ultraGridColumn28.Header.VisiblePosition = 24;
            ultraGridColumn29.Header.VisiblePosition = 25;
            ultraGridColumn51.Header.VisiblePosition = 26;
            ultraGridColumn52.Header.VisiblePosition = 27;
            ultraGridColumn44.Header.VisiblePosition = 28;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20,
            ultraGridColumn21,
            ultraGridColumn22,
            ultraGridColumn23,
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn26,
            ultraGridColumn27,
            ultraGridColumn39,
            ultraGridColumn28,
            ultraGridColumn29,
            ultraGridColumn51,
            ultraGridColumn52,
            ultraGridColumn44});
            ultraGridBand2.Hidden = true;
            ultraGridColumn30.Header.VisiblePosition = 0;
            ultraGridColumn31.Header.VisiblePosition = 1;
            ultraGridColumn32.Header.VisiblePosition = 2;
            ultraGridBand3.Columns.AddRange(new object[] {
            ultraGridColumn30,
            ultraGridColumn31,
            ultraGridColumn32});
            ultraGridBand3.Hidden = true;
            ultraGridColumn33.Header.VisiblePosition = 0;
            ultraGridColumn34.Header.VisiblePosition = 1;
            ultraGridColumn35.Header.VisiblePosition = 2;
            ultraGridColumn36.Header.VisiblePosition = 3;
            ultraGridColumn37.Header.VisiblePosition = 4;
            ultraGridColumn53.Header.VisiblePosition = 5;
            ultraGridColumn54.Header.VisiblePosition = 6;
            ultraGridColumn38.Header.VisiblePosition = 7;
            ultraGridColumn55.Header.VisiblePosition = 8;
            ultraGridBand4.Columns.AddRange(new object[] {
            ultraGridColumn33,
            ultraGridColumn34,
            ultraGridColumn35,
            ultraGridColumn36,
            ultraGridColumn37,
            ultraGridColumn53,
            ultraGridColumn54,
            ultraGridColumn38,
            ultraGridColumn55});
            ultraGridBand4.Hidden = true;
            ultraGridColumn40.Header.VisiblePosition = 0;
            ultraGridColumn41.Header.VisiblePosition = 1;
            ultraGridColumn42.Header.VisiblePosition = 2;
            ultraGridColumn43.Header.VisiblePosition = 3;
            ultraGridBand5.Columns.AddRange(new object[] {
            ultraGridColumn40,
            ultraGridColumn41,
            ultraGridColumn42,
            ultraGridColumn43});
            ultraGridBand5.Hidden = true;
            ultraGridColumn45.Header.VisiblePosition = 0;
            ultraGridColumn46.Header.VisiblePosition = 1;
            ultraGridColumn47.Header.VisiblePosition = 2;
            ultraGridColumn48.Header.VisiblePosition = 3;
            ultraGridColumn49.Header.VisiblePosition = 4;
            ultraGridColumn50.Header.VisiblePosition = 5;
            ultraGridBand6.Columns.AddRange(new object[] {
            ultraGridColumn45,
            ultraGridColumn46,
            ultraGridColumn47,
            ultraGridColumn48,
            ultraGridColumn49,
            ultraGridColumn50});
            ultraGridColumn56.Header.VisiblePosition = 0;
            ultraGridColumn57.Header.VisiblePosition = 1;
            ultraGridColumn58.Header.VisiblePosition = 2;
            ultraGridColumn59.Header.VisiblePosition = 3;
            ultraGridColumn60.Header.VisiblePosition = 4;
            ultraGridColumn61.Header.VisiblePosition = 5;
            ultraGridBand7.Columns.AddRange(new object[] {
            ultraGridColumn56,
            ultraGridColumn57,
            ultraGridColumn58,
            ultraGridColumn59,
            ultraGridColumn60,
            ultraGridColumn61});
            ultraGridColumn62.Header.VisiblePosition = 0;
            ultraGridColumn63.Header.VisiblePosition = 1;
            ultraGridColumn64.Header.VisiblePosition = 2;
            ultraGridColumn65.Header.VisiblePosition = 3;
            ultraGridBand8.Columns.AddRange(new object[] {
            ultraGridColumn62,
            ultraGridColumn63,
            ultraGridColumn64,
            ultraGridColumn65});
            ultraGridColumn66.Header.VisiblePosition = 0;
            ultraGridColumn67.Header.VisiblePosition = 1;
            ultraGridColumn68.Header.VisiblePosition = 2;
            ultraGridColumn69.Header.VisiblePosition = 3;
            ultraGridBand9.Columns.AddRange(new object[] {
            ultraGridColumn66,
            ultraGridColumn67,
            ultraGridColumn68,
            ultraGridColumn69});
            this.grdMaterial.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdMaterial.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdMaterial.DisplayLayout.BandsSerializer.Add(ultraGridBand3);
            this.grdMaterial.DisplayLayout.BandsSerializer.Add(ultraGridBand4);
            this.grdMaterial.DisplayLayout.BandsSerializer.Add(ultraGridBand5);
            this.grdMaterial.DisplayLayout.BandsSerializer.Add(ultraGridBand6);
            this.grdMaterial.DisplayLayout.BandsSerializer.Add(ultraGridBand7);
            this.grdMaterial.DisplayLayout.BandsSerializer.Add(ultraGridBand8);
            this.grdMaterial.DisplayLayout.BandsSerializer.Add(ultraGridBand9);
            this.grdMaterial.DisplayLayout.BandsSerializer.Add(ultraGridBand10);
            this.grdMaterial.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdMaterial.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.grdMaterial.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdMaterial.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.grdMaterial.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdMaterial.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.grdMaterial.DisplayLayout.MaxColScrollRegions = 1;
            this.grdMaterial.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdMaterial.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdMaterial.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grdMaterial.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            this.grdMaterial.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.grdMaterial.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdMaterial.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdMaterial.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.grdMaterial.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdMaterial.DisplayLayout.Override.CellAppearance = appearance8;
            this.grdMaterial.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdMaterial.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.grdMaterial.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.grdMaterial.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.grdMaterial.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdMaterial.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.grdMaterial.DisplayLayout.Override.RowAppearance = appearance11;
            this.grdMaterial.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdMaterial.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            this.grdMaterial.DisplayLayout.Override.SupportDataErrorInfo = Infragistics.Win.UltraWinGrid.SupportDataErrorInfo.RowsAndCells;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdMaterial.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.grdMaterial.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdMaterial.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdMaterial.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdMaterial.Location = new System.Drawing.Point(12, 12);
            this.grdMaterial.Name = "grdMaterial";
            this.grdMaterial.Size = new System.Drawing.Size(389, 333);
            this.grdMaterial.TabIndex = 19;
            this.grdMaterial.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdMaterial_InitializeLayout);
            this.grdMaterial.BeforeExitEditMode += new Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventHandler(this.grdMaterial_BeforeExitEditMode);
            this.grdMaterial.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.grdMaterial_BeforeRowsDeleted);
            // 
            // btnMerge
            // 
            this.btnMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMerge.Location = new System.Drawing.Point(55, 351);
            this.btnMerge.Name = "btnMerge";
            this.btnMerge.Size = new System.Drawing.Size(76, 23);
            this.btnMerge.TabIndex = 20;
            this.btnMerge.Text = "&Merge";
            this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
            // 
            // taParts
            // 
            this.taParts.ClearBeforeFill = false;
            // 
            // taManagerParts
            // 
            this.taManagerParts.BackupDataSetBeforeUpdate = false;
            this.taManagerParts.d_AirframeTableAdapter = null;
            this.taManagerParts.d_ManufacturerTableAdapter = null;
            this.taManagerParts.d_MaterialTableAdapter = this.taMaterial;
            this.taManagerParts.d_PriorityTableAdapter = null;
            this.taManagerParts.MediaTableAdapter = null;
            this.taManagerParts.Part_DocumentLinkTableAdapter = null;
            this.taManagerParts.Part_MediaTableAdapter = null;
            this.taManagerParts.Part_PartMarkingTableAdapter = null;
            this.taManagerParts.PartAreaDimensionTableAdapter = null;
            this.taManagerParts.PartAreaTableAdapter = null;
            this.taManagerParts.PartProcessAnswerTableAdapter = null;
            this.taManagerParts.PartProcessTableAdapter = null;
            this.taManagerParts.PartProcessVolumePriceTableAdapter = null;
            this.taManagerParts.PartTableAdapter = this.taParts;
            this.taManagerParts.Receiving_DocumentLinkTableAdapter = null;
            this.taManagerParts.Receiving_MediaTableAdapter = null;
            this.taManagerParts.ReceivingTableAdapter = null;
            this.taManagerParts.UpdateOrder = DWOS.Data.Datasets.PartsDatasetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            // 
            // helpLink1
            // 
            this.helpLink1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = "material_manager_dialog.htm";
            this.helpLink1.Location = new System.Drawing.Point(12, 357);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 21;
            // 
            // MaterialManager
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(413, 386);
            this.Controls.Add(this.helpLink1);
            this.Controls.Add(this.btnMerge);
            this.Controls.Add(this.grdMaterial);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MaterialManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Material Manager";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MaterialManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dsParts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdMaterial)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private DWOS.Data.Datasets.PartsDatasetTableAdapters.d_MaterialTableAdapter taMaterial;
		private DWOS.Data.Datasets.PartsDataset dsParts;
		private Infragistics.Win.UltraWinGrid.UltraGrid grdMaterial;
		private Infragistics.Win.Misc.UltraButton btnMerge;
		private DWOS.Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter taParts;
		private DWOS.Data.Datasets.PartsDatasetTableAdapters.TableAdapterManager taManagerParts;
        private Utilities.HelpLink helpLink1;
	}
}