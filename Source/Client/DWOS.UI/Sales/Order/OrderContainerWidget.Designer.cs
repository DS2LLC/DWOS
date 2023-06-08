namespace DWOS.UI.Sales.Order
{
    partial class OrderContainerWidget
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add an item to the selected container.", Infragistics.Win.ToolTipImage.Default, "Add Container Item", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Print all labels or selected labels.", Infragistics.Win.ToolTipImage.Default, "Print Labels", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Delete the selected container", Infragistics.Win.ToolTipImage.Default, "Delete Containers", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add a new container", Infragistics.Win.ToolTipImage.Default, "Add Container", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("OrderContainers", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderContainerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartQuantity");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsActive");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_OrderContainerItem_OrderContainers");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Weight", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShipmentPackageTypeID", 1);
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_OrderContainerItem_OrderContainers", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderContainerItemID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderContainerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShipmentPackageTypeID");
            this.btnAddItem = new Infragistics.Win.Misc.UltraButton();
            this.btnPrint = new Infragistics.Win.Misc.UltraButton();
            this.btnRemoveProcess = new Infragistics.Win.Misc.UltraButton();
            this.btnAddProcess = new Infragistics.Win.Misc.UltraButton();
            this.grdContainers = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.bsData = new System.Windows.Forms.BindingSource(this.components);
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grdContainers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAddItem
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.AddAlt_16;
            this.btnAddItem.Appearance = appearance1;
            this.btnAddItem.AutoSize = true;
            this.btnAddItem.Location = new System.Drawing.Point(-1, 44);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(26, 26);
            this.btnAddItem.TabIndex = 27;
            ultraToolTipInfo4.ToolTipText = "Add an item to the selected container.";
            ultraToolTipInfo4.ToolTipTitle = "Add Container Item";
            this.tipManager.SetUltraToolTip(this.btnAddItem, ultraToolTipInfo4);
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // btnPrint
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.Print_16;
            this.btnPrint.Appearance = appearance2;
            this.btnPrint.AutoSize = true;
            this.btnPrint.Location = new System.Drawing.Point(-1, 108);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(26, 26);
            this.btnPrint.TabIndex = 29;
            ultraToolTipInfo1.ToolTipText = "Print all labels or selected labels.";
            ultraToolTipInfo1.ToolTipTitle = "Print Labels";
            this.tipManager.SetUltraToolTip(this.btnPrint, ultraToolTipInfo1);
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnRemoveProcess
            // 
            appearance3.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            this.btnRemoveProcess.Appearance = appearance3;
            this.btnRemoveProcess.AutoSize = true;
            this.btnRemoveProcess.Location = new System.Drawing.Point(0, 76);
            this.btnRemoveProcess.Name = "btnRemoveProcess";
            this.btnRemoveProcess.Size = new System.Drawing.Size(26, 26);
            this.btnRemoveProcess.TabIndex = 28;
            ultraToolTipInfo2.ToolTipText = "Delete the selected container";
            ultraToolTipInfo2.ToolTipTitle = "Delete Containers";
            this.tipManager.SetUltraToolTip(this.btnRemoveProcess, ultraToolTipInfo2);
            this.btnRemoveProcess.Click += new System.EventHandler(this.btnRemoveProcess_Click);
            // 
            // btnAddProcess
            // 
            appearance4.Image = global::DWOS.UI.Properties.Resources.Add_16;
            this.btnAddProcess.Appearance = appearance4;
            this.btnAddProcess.AutoSize = true;
            this.btnAddProcess.Location = new System.Drawing.Point(-1, 12);
            this.btnAddProcess.Name = "btnAddProcess";
            this.btnAddProcess.Size = new System.Drawing.Size(26, 26);
            this.btnAddProcess.TabIndex = 26;
            ultraToolTipInfo3.ToolTipText = "Add a new container";
            ultraToolTipInfo3.ToolTipTitle = "Add Container";
            this.tipManager.SetUltraToolTip(this.btnAddProcess, ultraToolTipInfo3);
            this.btnAddProcess.Click += new System.EventHandler(this.btnAddProcess_Click);
            // 
            // grdContainers
            // 
            this.grdContainers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            ultraGridColumn14.Header.Caption = "Container #";
            ultraGridColumn14.Header.VisiblePosition = 0;
            ultraGridColumn14.Width = 163;
            ultraGridColumn15.Header.Caption = "Part Qty";
            ultraGridColumn15.Header.VisiblePosition = 1;
            ultraGridColumn15.Width = 117;
            ultraGridColumn16.Header.VisiblePosition = 2;
            ultraGridColumn16.Hidden = true;
            ultraGridColumn17.Header.Caption = "Is Active";
            ultraGridColumn17.Header.VisiblePosition = 4;
            ultraGridColumn18.Header.VisiblePosition = 6;
            ultraGridColumn19.DataType = typeof(decimal);
            ultraGridColumn19.Format = "###,##0.0#";
            ultraGridColumn19.Header.Caption = "Weight (Lbs)";
            ultraGridColumn19.Header.VisiblePosition = 3;
            ultraGridColumn19.MaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            131072});
            ultraGridColumn19.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            ultraGridColumn19.NullText = "0.0";
            ultraGridColumn19.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DoubleNonNegativeWithSpin;
            ultraGridColumn19.Width = 159;
            ultraGridColumn20.Header.Caption = "Package Type";
            ultraGridColumn20.Header.VisiblePosition = 5;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20});
            ultraGridColumn11.Header.Caption = "Container Item #";
            ultraGridColumn11.Header.VisiblePosition = 0;
            ultraGridColumn12.Header.VisiblePosition = 1;
            ultraGridColumn12.Hidden = true;
            ultraGridColumn13.Header.Caption = "Package Type";
            ultraGridColumn13.Header.VisiblePosition = 2;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13});
            this.grdContainers.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdContainers.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdContainers.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdContainers.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdContainers.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdContainers.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdContainers.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdContainers.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdContainers.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.grdContainers.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdContainers.Location = new System.Drawing.Point(31, 0);
            this.grdContainers.Name = "grdContainers";
            this.grdContainers.Size = new System.Drawing.Size(645, 328);
            this.grdContainers.TabIndex = 25;
            this.grdContainers.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdContainers_InitializeLayout);
            this.grdContainers.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdContainers_InitializeRow);
            this.grdContainers.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.grdContainers_AfterSelectChange);
            this.grdContainers.Error += new Infragistics.Win.UltraWinGrid.ErrorEventHandler(this.grdContainers_Error);
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // OrderContainerWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAddItem);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnRemoveProcess);
            this.Controls.Add(this.btnAddProcess);
            this.Controls.Add(this.grdContainers);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "OrderContainerWidget";
            this.Size = new System.Drawing.Size(677, 328);
            ((System.ComponentModel.ISupportInitialize)(this.grdContainers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnAddItem;
        private Infragistics.Win.Misc.UltraButton btnPrint;
        private Infragistics.Win.Misc.UltraButton btnRemoveProcess;
        private Infragistics.Win.Misc.UltraButton btnAddProcess;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdContainers;
        private System.Windows.Forms.BindingSource bsData;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
    }
}
