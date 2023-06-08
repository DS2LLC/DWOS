namespace DWOS.UI.ShippingRec.ShippingManagerPanels
{
	partial class ShipmentSummary
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Customer");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShipmentSummary));
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Action");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("User");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Time");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PackageType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Count");
            this.grpSummary = new Infragistics.Win.Misc.UltraGroupBox();
            this.grdLog = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.grpContainerCounts = new Infragistics.Win.Misc.UltraGroupBox();
            this.grdContainerCounts = new Infragistics.Win.UltraWinGrid.UltraGrid();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpSummary)).BeginInit();
            this.grpSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpContainerCounts)).BeginInit();
            this.grpContainerCounts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdContainerCounts)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.grpContainerCounts);
            this.grpData.Controls.Add(this.grpSummary);
            appearance2.Image = global::DWOS.UI.Properties.Resources.Package_16;
            this.grpData.HeaderAppearance = appearance2;
            this.grpData.Size = new System.Drawing.Size(558, 426);
            this.grpData.Text = "Shipping Summary";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.grpSummary, 0);
            this.grpData.Controls.SetChildIndex(this.grpContainerCounts, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(1371, -1021);
            // 
            // grpSummary
            // 
            this.grpSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSummary.Controls.Add(this.grdLog);
            this.grpSummary.Location = new System.Drawing.Point(3, 28);
            this.grpSummary.Name = "grpSummary";
            this.grpSummary.Size = new System.Drawing.Size(552, 236);
            this.grpSummary.TabIndex = 1;
            // 
            // grdLog
            // 
            this.grdLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdLog.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            appearance1.Image = ((object)(resources.GetObject("appearance1.Image")));
            ultraGridColumn3.CellAppearance = appearance1;
            ultraGridColumn3.Header.VisiblePosition = 0;
            ultraGridColumn3.Width = 127;
            ultraGridColumn4.Header.VisiblePosition = 1;
            ultraGridColumn4.Width = 187;
            ultraGridColumn5.Header.VisiblePosition = 2;
            ultraGridColumn5.Width = 62;
            ultraGridColumn6.Header.VisiblePosition = 3;
            ultraGridColumn6.Width = 62;
            ultraGridColumn7.Header.VisiblePosition = 4;
            ultraGridColumn7.Width = 62;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7});
            this.grdLog.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdLog.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdLog.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdLog.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.grdLog.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdLog.Location = new System.Drawing.Point(8, 6);
            this.grdLog.Name = "grdLog";
            this.grdLog.Size = new System.Drawing.Size(538, 224);
            this.grdLog.TabIndex = 0;
            this.grdLog.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdLog_InitializeLayout);
            // 
            // grpContainerCounts
            // 
            this.grpContainerCounts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpContainerCounts.Controls.Add(this.grdContainerCounts);
            this.grpContainerCounts.Location = new System.Drawing.Point(3, 270);
            this.grpContainerCounts.Name = "grpContainerCounts";
            this.grpContainerCounts.Size = new System.Drawing.Size(540, 150);
            this.grpContainerCounts.TabIndex = 4;
            this.grpContainerCounts.Text = "Container Counts";
            // 
            // grdContainerCounts
            // 
            this.grdContainerCounts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdContainerCounts.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.Header.Caption = "Type of Container";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2});
            this.grdContainerCounts.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdContainerCounts.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.grdContainerCounts.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdContainerCounts.Location = new System.Drawing.Point(6, 23);
            this.grdContainerCounts.Name = "grdContainerCounts";
            this.grdContainerCounts.Size = new System.Drawing.Size(528, 121);
            this.grdContainerCounts.TabIndex = 3;
            this.grdContainerCounts.Text = "ultraGrid1";
            // 
            // ShipmentSummary
            // 
            this.Name = "ShipmentSummary";
            this.Size = new System.Drawing.Size(564, 432);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpSummary)).EndInit();
            this.grpSummary.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpContainerCounts)).EndInit();
            this.grpContainerCounts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdContainerCounts)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraGroupBox grpSummary;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdContainerCounts;
        private Infragistics.Win.Misc.UltraGroupBox grpContainerCounts;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdLog;
    }
}
