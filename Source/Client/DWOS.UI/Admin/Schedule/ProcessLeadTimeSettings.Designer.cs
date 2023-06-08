namespace DWOS.UI.Admin.Schedule
{
    partial class ProcessLeadTimeSettings
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("d_ProcessCategory", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessCategory");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LeadTime");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_Process_d_ProcessCategory");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_Process_d_ProcessCategory", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Category");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_OrderProcesses_Process");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand3 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_OrderProcesses_Process", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderProcessesID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StepOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Department");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StartDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EndDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EstEndDate");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessLeadTimeSettings));
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.grdProcessCat = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsSchedule = new DWOS.Data.Datasets.ScheduleDataset();
            this.taProcessCategory = new DWOS.Data.Datasets.ScheduleDatasetTableAdapters.d_ProcessCategoryTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.grdProcessCat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsSchedule)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(382, 292);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(463, 292);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            // 
            // grdProcessCat
            // 
            this.grdProcessCat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdProcessCat.DataMember = "d_ProcessCategory";
            this.grdProcessCat.DataSource = this.dsSchedule;
            this.grdProcessCat.DisplayLayout.AddNewBox.Hidden = false;
            ultraGridBand1.AddButtonCaption = "Category";
            ultraGridColumn16.Header.Caption = "Process Category";
            ultraGridColumn16.Header.VisiblePosition = 0;
            ultraGridColumn16.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn17.DefaultCellValue = "0";
            ultraGridColumn17.Header.Caption = "Lead Time (Days)";
            ultraGridColumn17.Header.VisiblePosition = 1;
            ultraGridColumn17.MaxValue = "99";
            ultraGridColumn17.MinValue = "0";
            ultraGridColumn17.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DoubleNonNegativeWithSpin;
            ultraGridColumn18.Header.VisiblePosition = 2;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn18});
            ultraGridBand1.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            ultraGridBand1.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            ultraGridColumn19.Header.VisiblePosition = 0;
            ultraGridColumn20.Header.VisiblePosition = 1;
            ultraGridColumn21.Header.VisiblePosition = 2;
            ultraGridColumn22.Header.VisiblePosition = 3;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn19,
            ultraGridColumn20,
            ultraGridColumn21,
            ultraGridColumn22});
            ultraGridBand2.Hidden = true;
            ultraGridColumn23.Header.VisiblePosition = 0;
            ultraGridColumn24.Header.VisiblePosition = 1;
            ultraGridColumn25.Header.VisiblePosition = 2;
            ultraGridColumn26.Header.VisiblePosition = 3;
            ultraGridColumn27.Header.VisiblePosition = 4;
            ultraGridColumn28.Header.VisiblePosition = 5;
            ultraGridColumn29.Header.VisiblePosition = 6;
            ultraGridColumn30.Header.VisiblePosition = 7;
            ultraGridBand3.Columns.AddRange(new object[] {
            ultraGridColumn23,
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn26,
            ultraGridColumn27,
            ultraGridColumn28,
            ultraGridColumn29,
            ultraGridColumn30});
            this.grdProcessCat.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdProcessCat.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdProcessCat.DisplayLayout.BandsSerializer.Add(ultraGridBand3);
            this.grdProcessCat.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            this.grdProcessCat.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.grdProcessCat.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdProcessCat.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdProcessCat.Location = new System.Drawing.Point(21, 12);
            this.grdProcessCat.Name = "grdProcessCat";
            this.grdProcessCat.Size = new System.Drawing.Size(517, 272);
            this.grdProcessCat.TabIndex = 0;
            this.grdProcessCat.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdProcessCat_InitializeLayout);
            this.grdProcessCat.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.grdProcessCat_BeforeRowsDeleted);
            // 
            // dsSchedule
            // 
            this.dsSchedule.DataSetName = "ScheduleDataset";
            this.dsSchedule.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taProcessCategory
            // 
            this.taProcessCategory.ClearBeforeFill = true;
            // 
            // ProcessLeadTimeSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 327);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grdProcessCat);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ProcessLeadTimeSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Process Lead Time Settings";
            this.Load += new System.EventHandler(this.ProcessLeadTimeSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdProcessCat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsSchedule)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid grdProcessCat;
        private Data.Datasets.ScheduleDataset dsSchedule;
        private Data.Datasets.ScheduleDatasetTableAdapters.d_ProcessCategoryTableAdapter taProcessCategory;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraButton btnCancel;
    }
}