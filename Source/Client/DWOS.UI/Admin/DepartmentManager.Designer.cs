namespace DWOS.UI.Admin
{
	partial class DepartmentManager
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("d_Department", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DepartmentID");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SystemName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Active");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AccountingCode");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn70 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EmailAddress");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_WorkScheduleSummary_d_Department");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_ProcessingLine_d_Department");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_Batch_d_Department");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_WorkScheduleSummary_d_Department", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WorkScheduleSummaryID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DepartmentID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ScheduledDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Shift");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EstimatedParts");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RunDate");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand3 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_ProcessingLine_d_Department", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessingLineID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DepartmentID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_Batch_ProcessingLine");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand4 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_Batch_ProcessingLine", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BatchID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OpenDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Fixture");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WorkStatus");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CurrentLocation");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("NextDept");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CurrentProcess");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartCount");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderCount");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TotalSurfaceArea");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TotalWeight");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ActiveTimerCount");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CurrentLine");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SchedulePriority");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn32 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SalesOrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BatchStatus_BatchOrderStatus");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn34 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_BatchOrder_Batch");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand5 = new Infragistics.Win.UltraWinGrid.UltraGridBand("BatchStatus_BatchOrderStatus", 3);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn35 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BatchID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn36 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn37 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PurchaseOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn38 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn39 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn40 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn41 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BatchPartQuantity");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand6 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_BatchOrder_Batch", 3);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn42 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BatchID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn43 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand7 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_Batch_d_Department", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn44 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BatchID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn45 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OpenDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn46 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Fixture");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn47 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WorkStatus");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn48 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CurrentLocation");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn49 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("NextDept");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn50 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CurrentProcess");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn51 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartCount");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn52 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderCount");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn53 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TotalSurfaceArea");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn54 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TotalWeight");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn55 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ActiveTimerCount");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn56 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CurrentLine");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn57 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SchedulePriority");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn58 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SalesOrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn59 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BatchStatus_BatchOrderStatus");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn60 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_BatchOrder_Batch");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand8 = new Infragistics.Win.UltraWinGrid.UltraGridBand("BatchStatus_BatchOrderStatus", 6);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn61 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BatchID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn62 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn63 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PurchaseOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn64 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn65 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn66 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn67 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BatchPartQuantity");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand9 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_BatchOrder_Batch", 6);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn68 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BatchID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn69 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DepartmentManager));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.dsOrderStatus = new DWOS.Data.Datasets.OrderStatusDataSet();
            this.taDepartment = new DWOS.Data.Datasets.OrderStatusDataSetTableAdapters.d_DepartmentTableAdapter();
            this.grdAirframe = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsApplicationSettings = new DWOS.Data.Datasets.ApplicationSettingsDataSet();
            this.taApplicationSettings = new DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters.ApplicationSettingsTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrderStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdAirframe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsApplicationSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(466, 324);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(371, 324);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // dsOrderStatus
            // 
            this.dsOrderStatus.DataSetName = "OrderStatusDataSet";
            this.dsOrderStatus.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taDepartment
            // 
            this.taDepartment.ClearBeforeFill = true;
            // 
            // grdAirframe
            // 
            this.grdAirframe.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdAirframe.DataMember = "d_Department";
            this.grdAirframe.DataSource = this.dsOrderStatus;
            this.grdAirframe.DisplayLayout.AddNewBox.Hidden = false;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdAirframe.DisplayLayout.Appearance = appearance1;
            ultraGridBand1.AddButtonCaption = "Department";
            ultraGridBand1.AddButtonToolTipText = "Add a new department";
            ultraGridColumn1.DefaultCellValue = "New Department";
            appearance2.TextHAlignAsString = "Center";
            ultraGridColumn1.Header.Appearance = appearance2;
            ultraGridColumn1.Header.Caption = "Department";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn3.DefaultCellValue = "True";
            appearance3.TextHAlignAsString = "Center";
            ultraGridColumn3.Header.Appearance = appearance3;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn11.Header.Caption = "Accounting Code";
            ultraGridColumn11.Header.VisiblePosition = 3;
            ultraGridColumn70.Header.Caption = "Email Address";
            ultraGridColumn70.Header.VisiblePosition = 4;
            ultraGridColumn4.Header.VisiblePosition = 5;
            ultraGridColumn12.Header.VisiblePosition = 6;
            ultraGridColumn16.Header.VisiblePosition = 7;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn11,
            ultraGridColumn70,
            ultraGridColumn4,
            ultraGridColumn12,
            ultraGridColumn16});
            ultraGridBand1.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            ultraGridBand1.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            ultraGridBand1.Override.InvalidValueBehavior = Infragistics.Win.UltraWinGrid.InvalidValueBehavior.RevertValueAndRetainFocus;
            ultraGridBand1.Override.SupportDataErrorInfo = Infragistics.Win.UltraWinGrid.SupportDataErrorInfo.CellsOnly;
            ultraGridColumn5.Header.VisiblePosition = 0;
            ultraGridColumn6.Header.VisiblePosition = 1;
            ultraGridColumn7.Header.VisiblePosition = 2;
            ultraGridColumn8.Header.VisiblePosition = 3;
            ultraGridColumn9.Header.VisiblePosition = 4;
            ultraGridColumn10.Header.VisiblePosition = 5;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10});
            ultraGridBand2.Hidden = true;
            ultraGridColumn13.Header.VisiblePosition = 0;
            ultraGridColumn14.Header.VisiblePosition = 1;
            ultraGridColumn15.Header.VisiblePosition = 2;
            ultraGridColumn17.Header.VisiblePosition = 3;
            ultraGridBand3.Columns.AddRange(new object[] {
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn17});
            ultraGridBand3.Hidden = true;
            ultraGridColumn18.Header.VisiblePosition = 0;
            ultraGridColumn19.Header.VisiblePosition = 1;
            ultraGridColumn20.Header.VisiblePosition = 2;
            ultraGridColumn21.Header.VisiblePosition = 3;
            ultraGridColumn22.Header.VisiblePosition = 4;
            ultraGridColumn23.Header.VisiblePosition = 5;
            ultraGridColumn24.Header.VisiblePosition = 6;
            ultraGridColumn25.Header.VisiblePosition = 7;
            ultraGridColumn26.Header.VisiblePosition = 8;
            ultraGridColumn27.Header.VisiblePosition = 9;
            ultraGridColumn28.Header.VisiblePosition = 10;
            ultraGridColumn29.Header.VisiblePosition = 11;
            ultraGridColumn30.Header.VisiblePosition = 12;
            ultraGridColumn31.Header.VisiblePosition = 13;
            ultraGridColumn32.Header.VisiblePosition = 14;
            ultraGridColumn33.Header.VisiblePosition = 15;
            ultraGridColumn34.Header.VisiblePosition = 16;
            ultraGridBand4.Columns.AddRange(new object[] {
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
            ultraGridColumn28,
            ultraGridColumn29,
            ultraGridColumn30,
            ultraGridColumn31,
            ultraGridColumn32,
            ultraGridColumn33,
            ultraGridColumn34});
            ultraGridBand4.Hidden = true;
            ultraGridColumn35.Header.VisiblePosition = 0;
            ultraGridColumn36.Header.VisiblePosition = 1;
            ultraGridColumn37.Header.VisiblePosition = 2;
            ultraGridColumn38.Header.VisiblePosition = 3;
            ultraGridColumn39.Header.VisiblePosition = 4;
            ultraGridColumn40.Header.VisiblePosition = 5;
            ultraGridColumn41.Header.VisiblePosition = 6;
            ultraGridBand5.Columns.AddRange(new object[] {
            ultraGridColumn35,
            ultraGridColumn36,
            ultraGridColumn37,
            ultraGridColumn38,
            ultraGridColumn39,
            ultraGridColumn40,
            ultraGridColumn41});
            ultraGridBand5.Hidden = true;
            ultraGridColumn42.Header.VisiblePosition = 0;
            ultraGridColumn43.Header.VisiblePosition = 1;
            ultraGridBand6.Columns.AddRange(new object[] {
            ultraGridColumn42,
            ultraGridColumn43});
            ultraGridBand6.Hidden = true;
            ultraGridColumn44.Header.VisiblePosition = 0;
            ultraGridColumn45.Header.VisiblePosition = 1;
            ultraGridColumn46.Header.VisiblePosition = 2;
            ultraGridColumn47.Header.VisiblePosition = 3;
            ultraGridColumn48.Header.VisiblePosition = 4;
            ultraGridColumn49.Header.VisiblePosition = 5;
            ultraGridColumn50.Header.VisiblePosition = 6;
            ultraGridColumn51.Header.VisiblePosition = 7;
            ultraGridColumn52.Header.VisiblePosition = 8;
            ultraGridColumn53.Header.VisiblePosition = 9;
            ultraGridColumn54.Header.VisiblePosition = 10;
            ultraGridColumn55.Header.VisiblePosition = 11;
            ultraGridColumn56.Header.VisiblePosition = 12;
            ultraGridColumn57.Header.VisiblePosition = 13;
            ultraGridColumn58.Header.VisiblePosition = 14;
            ultraGridColumn59.Header.VisiblePosition = 15;
            ultraGridColumn60.Header.VisiblePosition = 16;
            ultraGridBand7.Columns.AddRange(new object[] {
            ultraGridColumn44,
            ultraGridColumn45,
            ultraGridColumn46,
            ultraGridColumn47,
            ultraGridColumn48,
            ultraGridColumn49,
            ultraGridColumn50,
            ultraGridColumn51,
            ultraGridColumn52,
            ultraGridColumn53,
            ultraGridColumn54,
            ultraGridColumn55,
            ultraGridColumn56,
            ultraGridColumn57,
            ultraGridColumn58,
            ultraGridColumn59,
            ultraGridColumn60});
            ultraGridBand7.Hidden = true;
            ultraGridColumn61.Header.VisiblePosition = 0;
            ultraGridColumn62.Header.VisiblePosition = 1;
            ultraGridColumn63.Header.VisiblePosition = 2;
            ultraGridColumn64.Header.VisiblePosition = 3;
            ultraGridColumn65.Header.VisiblePosition = 4;
            ultraGridColumn66.Header.VisiblePosition = 5;
            ultraGridColumn67.Header.VisiblePosition = 6;
            ultraGridBand8.Columns.AddRange(new object[] {
            ultraGridColumn61,
            ultraGridColumn62,
            ultraGridColumn63,
            ultraGridColumn64,
            ultraGridColumn65,
            ultraGridColumn66,
            ultraGridColumn67});
            ultraGridBand8.Hidden = true;
            ultraGridColumn68.Header.VisiblePosition = 0;
            ultraGridColumn69.Header.VisiblePosition = 1;
            ultraGridBand9.Columns.AddRange(new object[] {
            ultraGridColumn68,
            ultraGridColumn69});
            ultraGridBand9.Hidden = true;
            this.grdAirframe.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdAirframe.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdAirframe.DisplayLayout.BandsSerializer.Add(ultraGridBand3);
            this.grdAirframe.DisplayLayout.BandsSerializer.Add(ultraGridBand4);
            this.grdAirframe.DisplayLayout.BandsSerializer.Add(ultraGridBand5);
            this.grdAirframe.DisplayLayout.BandsSerializer.Add(ultraGridBand6);
            this.grdAirframe.DisplayLayout.BandsSerializer.Add(ultraGridBand7);
            this.grdAirframe.DisplayLayout.BandsSerializer.Add(ultraGridBand8);
            this.grdAirframe.DisplayLayout.BandsSerializer.Add(ultraGridBand9);
            this.grdAirframe.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdAirframe.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance4.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance4.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance4.BorderColor = System.Drawing.SystemColors.Window;
            this.grdAirframe.DisplayLayout.GroupByBox.Appearance = appearance4;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdAirframe.DisplayLayout.GroupByBox.BandLabelAppearance = appearance5;
            this.grdAirframe.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance6.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance6.BackColor2 = System.Drawing.SystemColors.Control;
            appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance6.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdAirframe.DisplayLayout.GroupByBox.PromptAppearance = appearance6;
            this.grdAirframe.DisplayLayout.MaxColScrollRegions = 1;
            this.grdAirframe.DisplayLayout.MaxRowScrollRegions = 1;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            appearance7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdAirframe.DisplayLayout.Override.ActiveCellAppearance = appearance7;
            appearance8.BackColor = System.Drawing.SystemColors.Highlight;
            appearance8.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdAirframe.DisplayLayout.Override.ActiveRowAppearance = appearance8;
            this.grdAirframe.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            this.grdAirframe.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.grdAirframe.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdAirframe.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdAirframe.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance9.BackColor = System.Drawing.SystemColors.Window;
            this.grdAirframe.DisplayLayout.Override.CardAreaAppearance = appearance9;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            appearance10.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdAirframe.DisplayLayout.Override.CellAppearance = appearance10;
            this.grdAirframe.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdAirframe.DisplayLayout.Override.CellPadding = 0;
            appearance11.BackColor = System.Drawing.SystemColors.Control;
            appearance11.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance11.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance11.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance11.BorderColor = System.Drawing.SystemColors.Window;
            this.grdAirframe.DisplayLayout.Override.GroupByRowAppearance = appearance11;
            appearance12.TextHAlignAsString = "Left";
            this.grdAirframe.DisplayLayout.Override.HeaderAppearance = appearance12;
            this.grdAirframe.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdAirframe.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance13.BackColor = System.Drawing.SystemColors.Window;
            appearance13.BorderColor = System.Drawing.Color.Silver;
            this.grdAirframe.DisplayLayout.Override.RowAppearance = appearance13;
            this.grdAirframe.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance14.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdAirframe.DisplayLayout.Override.TemplateAddRowAppearance = appearance14;
            this.grdAirframe.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdAirframe.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdAirframe.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdAirframe.Location = new System.Drawing.Point(12, 12);
            this.grdAirframe.Name = "grdAirframe";
            this.grdAirframe.Size = new System.Drawing.Size(547, 306);
            this.grdAirframe.TabIndex = 20;
            this.grdAirframe.Text = "ultraGrid1";
            this.grdAirframe.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdAirframe_InitializeLayout);
            this.grdAirframe.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdAirframe_InitializeRow);
            this.grdAirframe.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.grdAirframe_BeforeRowsDeleted);
            // 
            // dsApplicationSettings
            // 
            this.dsApplicationSettings.DataSetName = "ApplicationSettingsDataSet";
            this.dsApplicationSettings.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taApplicationSettings
            // 
            this.taApplicationSettings.ClearBeforeFill = true;
            // 
            // DepartmentManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 359);
            this.Controls.Add(this.grdAirframe);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DepartmentManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Department Manager";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.AirframeManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrderStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdAirframe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsApplicationSettings)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
		private Data.Datasets.OrderStatusDataSet dsOrderStatus;
		private Data.Datasets.OrderStatusDataSetTableAdapters.d_DepartmentTableAdapter taDepartment;
		private Infragistics.Win.UltraWinGrid.UltraGrid grdAirframe;
		private Data.Datasets.ApplicationSettingsDataSet dsApplicationSettings;
		private Data.Datasets.ApplicationSettingsDataSetTableAdapters.ApplicationSettingsTableAdapter taApplicationSettings;
	}
}