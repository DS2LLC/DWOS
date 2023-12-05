namespace DWOS.UI.Admin
{
	partial class ExportInvoicesDialog
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
		    DisposeMe();

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
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel1 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel2 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel3 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("UpdateCustomFields");
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab1 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbon1");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup1 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup1");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Export");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Print");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup2 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup2");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("InvoiceStatusReport");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Export");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Print");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("InvoiceStatusReport");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("UpdateCustomFields");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportInvoicesDialog));
            this.grdExport = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsOrderInvoice = new DWOS.Data.Datasets.OrderInvoiceDataSet();
            this.statusBar = new Infragistics.Win.UltraWinStatusBar.UltraStatusBar();
            this.ExportToQuickbooks_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraToolbarsManager1 = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.taOrderInvoice = new DWOS.Data.Datasets.OrderInvoiceDataSetTableAdapters.OrderInvoiceTableAdapter();
            this.timerExport = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grdExport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrderInvoice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBar)).BeginInit();
            this.ExportToQuickbooks_Fill_Panel.ClientArea.SuspendLayout();
            this.ExportToQuickbooks_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // grdExport
            // 
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdExport.DisplayLayout.Appearance = appearance1;
            this.grdExport.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            this.grdExport.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdExport.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.grdExport.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdExport.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.grdExport.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdExport.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.grdExport.DisplayLayout.MaxBandDepth = 1;
            this.grdExport.DisplayLayout.MaxColScrollRegions = 1;
            this.grdExport.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdExport.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdExport.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grdExport.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdExport.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdExport.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdExport.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdExport.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdExport.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.grdExport.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdExport.DisplayLayout.Override.CellAppearance = appearance8;
            this.grdExport.DisplayLayout.Override.CellPadding = 0;
            this.grdExport.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.grdExport.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Center";
            appearance10.TextVAlignAsString = "Middle";
            this.grdExport.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.grdExport.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdExport.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.grdExport.DisplayLayout.Override.RowAppearance = appearance11;
            this.grdExport.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdExport.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdExport.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.grdExport.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdExport.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdExport.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdExport.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdExport.Location = new System.Drawing.Point(0, 0);
            this.grdExport.Name = "grdExport";
            this.grdExport.Size = new System.Drawing.Size(1296, 369);
            this.grdExport.TabIndex = 0;
            this.grdExport.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdExport_InitializeLayout);
            this.grdExport.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdExport_InitializeRow);
            // 
            // dsOrderInvoice
            // 
            this.dsOrderInvoice.DataSetName = "OrderInvoiceDataSet";
            this.dsOrderInvoice.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 520);
            this.statusBar.Name = "statusBar";
            ultraStatusPanel1.Style = Infragistics.Win.UltraWinStatusBar.PanelStyle.Progress;
            ultraStatusPanel1.Width = 250;
            ultraStatusPanel2.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Spring;
            ultraStatusPanel2.Text = "Exporting Invoices...";
            ultraStatusPanel3.Text = "XXX Invoices";
            this.statusBar.Panels.AddRange(new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel[] {
            ultraStatusPanel1,
            ultraStatusPanel2,
            ultraStatusPanel3});
            this.statusBar.Size = new System.Drawing.Size(1312, 23);
            this.statusBar.TabIndex = 13;
            // 
            // ExportToQuickbooks_Fill_Panel
            // 
            // 
            // ExportToQuickbooks_Fill_Panel.ClientArea
            // 
            this.ExportToQuickbooks_Fill_Panel.ClientArea.Controls.Add(this.grdExport);
            this.ExportToQuickbooks_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.ExportToQuickbooks_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExportToQuickbooks_Fill_Panel.Location = new System.Drawing.Point(8, 151);
            this.ExportToQuickbooks_Fill_Panel.Name = "ExportToQuickbooks_Fill_Panel";
            this.ExportToQuickbooks_Fill_Panel.Size = new System.Drawing.Size(1296, 369);
            this.ExportToQuickbooks_Fill_Panel.TabIndex = 14;
            // 
            // _ExportToQuickbooks_Toolbars_Dock_Area_Left
            // 
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 151);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.Name = "_ExportToQuickbooks_Toolbars_Dock_Area_Left";
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(8, 369);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Left.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // ultraToolbarsManager1
            // 
            this.ultraToolbarsManager1.DesignerFlags = 1;
            this.ultraToolbarsManager1.DockWithinContainer = this;
            this.ultraToolbarsManager1.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
            this.ultraToolbarsManager1.Ribbon.ApplicationMenu.ToolAreaLeft.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool9});
            this.ultraToolbarsManager1.Ribbon.FileMenuStyle = Infragistics.Win.UltraWinToolbars.FileMenuStyle.ApplicationMenu2010;
            ribbonTab1.Caption = "Home";
            ribbonGroup1.Caption = "Tools";
            buttonTool1.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool3.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool3});
            ribbonGroup2.Caption = "Reports";
            buttonTool5.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool5});
            ribbonTab1.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup1,
            ribbonGroup2});
            this.ultraToolbarsManager1.Ribbon.NonInheritedRibbonTabs.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonTab[] {
            ribbonTab1});
            this.ultraToolbarsManager1.Ribbon.Visible = true;
            this.ultraToolbarsManager1.ShowFullMenusDelay = 500;
            this.ultraToolbarsManager1.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2010;
            appearance13.Image = ((object)(resources.GetObject("appearance13.Image")));
            buttonTool2.SharedPropsInternal.AppearancesLarge.Appearance = appearance13;
            buttonTool2.SharedPropsInternal.Caption = "Export";
            buttonTool2.SharedPropsInternal.ToolTipTextFormatted = "Export to Quickbooks";
            buttonTool2.SharedPropsInternal.ToolTipTitle = "Export";
            appearance14.Image = global::DWOS.UI.Properties.Resources.Print_32;
            buttonTool4.SharedPropsInternal.AppearancesLarge.Appearance = appearance14;
            buttonTool4.SharedPropsInternal.Caption = "Print";
            buttonTool4.SharedPropsInternal.ToolTipTextFormatted = "Print invoice information.";
            buttonTool4.SharedPropsInternal.ToolTipTitle = "Print";
            appearance15.Image = ((object)(resources.GetObject("appearance15.Image")));
            buttonTool6.SharedPropsInternal.AppearancesLarge.Appearance = appearance15;
            buttonTool6.SharedPropsInternal.Caption = "Invoice Compare";
            buttonTool6.SharedPropsInternal.ToolTipTextFormatted = "Displays the invoice comparisons.";
            buttonTool6.SharedPropsInternal.ToolTipTitle = "Invoice Compare";
            buttonTool8.SharedPropsInternal.Caption = "Update Custom Fields";
            this.ultraToolbarsManager1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool2,
            buttonTool4,
            buttonTool6,
            buttonTool8});
            this.ultraToolbarsManager1.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.ultraToolbarsManager1_ToolClick);
            // 
            // _ExportToQuickbooks_Toolbars_Dock_Area_Right
            // 
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(1304, 151);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.Name = "_ExportToQuickbooks_Toolbars_Dock_Area_Right";
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(8, 369);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Right.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // _ExportToQuickbooks_Toolbars_Dock_Area_Top
            // 
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.Name = "_ExportToQuickbooks_Toolbars_Dock_Area_Top";
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(1312, 151);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Top.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // _ExportToQuickbooks_Toolbars_Dock_Area_Bottom
            // 
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 520);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.Name = "_ExportToQuickbooks_Toolbars_Dock_Area_Bottom";
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(1312, 0);
            this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // taOrderInvoice
            // 
            this.taOrderInvoice.ClearBeforeFill = true;
            // 
            // timerExport
            // 
            this.timerExport.Interval = 10000;
            this.timerExport.Tick += new System.EventHandler(this.timerExport_Tick);
            // 
            // ExportInvoicesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1312, 543);
            this.Controls.Add(this.ExportToQuickbooks_Fill_Panel);
            this.Controls.Add(this._ExportToQuickbooks_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._ExportToQuickbooks_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._ExportToQuickbooks_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this._ExportToQuickbooks_Toolbars_Dock_Area_Top);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExportInvoicesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Export Invoices";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExportInvoicesDialog_FormClosing);
            this.Load += new System.EventHandler(this.ExportToQuickbooks_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdExport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrderInvoice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBar)).EndInit();
            this.ExportToQuickbooks_Fill_Panel.ClientArea.ResumeLayout(false);
            this.ExportToQuickbooks_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid grdExport;
		private DWOS.Data.Datasets.OrderInvoiceDataSet dsOrderInvoice;
        private Infragistics.Win.UltraWinStatusBar.UltraStatusBar statusBar;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager ultraToolbarsManager1;
        private Infragistics.Win.Misc.UltraPanel ExportToQuickbooks_Fill_Panel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ExportToQuickbooks_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ExportToQuickbooks_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ExportToQuickbooks_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ExportToQuickbooks_Toolbars_Dock_Area_Top;
        private Data.Datasets.OrderInvoiceDataSetTableAdapters.OrderInvoiceTableAdapter taOrderInvoice;
        private System.Windows.Forms.Timer timerExport;
    }
}