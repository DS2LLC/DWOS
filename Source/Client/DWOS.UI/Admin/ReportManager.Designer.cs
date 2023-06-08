namespace DWOS.UI.Admin
{
    partial class ReportManager
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
            Infragistics.Win.UltraWinToolbars.ContextualTabGroup contextualTabGroup1 = new Infragistics.Win.UltraWinToolbars.ContextualTabGroup("aliasContext");
            Infragistics.Win.UltraWinToolbars.ContextualTabGroup contextualTabGroup2 = new Infragistics.Win.UltraWinToolbars.ContextualTabGroup("constraintsContext");
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab1 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbon1");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup1 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup1");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool54 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("Main");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Edit");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportManager));
            this.pnlReportInfo = new DWOS.UI.Admin.ReportPanels.ReportInfoPanel();
            this.pnlReportType = new DWOS.UI.Admin.ReportPanels.ReportTypePanel();
            this.dsReportFields = new DWOS.Data.Datasets.ReportFieldsDataSet();
            this.taReportFields = new DWOS.Data.Datasets.ReportFieldsDataSetTableAdapters.ReportFieldsTableAdapter();
            this.taReportFieldsCustomerSummary = new DWOS.Data.Datasets.ReportFieldsDataSetTableAdapters.ReportFieldsCustomerSummaryTableAdapter();
            this.taManager = new DWOS.Data.Datasets.ReportFieldsDataSetTableAdapters.TableAdapterManager();
            this.taReport = new DWOS.Data.Datasets.ReportFieldsDataSetTableAdapters.ReportTableAdapter();
            this.taCustomField = new DWOS.Data.Datasets.ReportFieldsDataSetTableAdapters.CustomFieldNameTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsReportFields)).BeginInit();
            this.SuspendLayout();
            // 
            // toolbarManager
            // 
            this.toolbarManager.MenuSettings.ForceSerialization = true;
            appearance1.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.toolbarManager.Ribbon.ApplicationMenu2010.ContentArea.Settings.Appearance = appearance1;
            this.toolbarManager.Ribbon.FileMenuStyle = Infragistics.Win.UltraWinToolbars.FileMenuStyle.ApplicationMenu2010;
            contextualTabGroup1.Caption = "Aliases";
            contextualTabGroup1.Key = "aliasContext";
            contextualTabGroup1.Visible = false;
            contextualTabGroup2.Caption = "Constraints";
            contextualTabGroup2.Key = "constraintsContext";
            contextualTabGroup2.Visible = false;
            this.toolbarManager.Ribbon.NonInheritedContextualTabGroups.AddRange(new Infragistics.Win.UltraWinToolbars.ContextualTabGroup[] {
            contextualTabGroup1,
            contextualTabGroup2});
            ribbonTab1.Caption = "Home";
            ribbonGroup1.Caption = "Reports";
            ribbonGroup1.PreferredToolSize = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool54.InstanceProps.IsFirstInGroup = true;
            ribbonGroup1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool27,
            buttonTool54});
            ribbonTab1.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup1});
            this.toolbarManager.Ribbon.NonInheritedRibbonTabs.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonTab[] {
            ribbonTab1});
            this.toolbarManager.Ribbon.Visible = true;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool20,
            buttonTool2});
            ultraToolbar1.Text = "Main";
            this.toolbarManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            this.toolbarManager.ToolbarSettings.ForceSerialization = true;
            appearance2.Image = global::DWOS.UI.Properties.Resources.Delete_32;
            buttonTool8.SharedPropsInternal.AppearancesLarge.Appearance = appearance2;
            buttonTool8.SharedPropsInternal.Caption = "Delete";
            buttonTool8.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool8.SharedPropsInternal.Enabled = false;
            appearance3.Image = global::DWOS.UI.Properties.Resources.Add_32;
            buttonTool18.SharedPropsInternal.AppearancesLarge.Appearance = appearance3;
            buttonTool18.SharedPropsInternal.Caption = "Add";
            buttonTool18.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool18.SharedPropsInternal.Enabled = false;
            appearance4.Image = global::DWOS.UI.Properties.Resources.Edit_16;
            buttonTool3.SharedPropsInternal.AppearancesLarge.Appearance = appearance4;
            appearance5.Image = global::DWOS.UI.Properties.Resources.Edit_16;
            buttonTool3.SharedPropsInternal.AppearancesSmall.Appearance = appearance5;
            buttonTool3.SharedPropsInternal.Caption = "Edit";
            buttonTool3.SharedPropsInternal.Enabled = false;
            this.toolbarManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool8,
            buttonTool18,
            buttonTool3});
            // 
            // tvwTOC
            // 
            this.toolbarManager.SetContextMenuUltra(this.tvwTOC, "TOCContext");
            this.tvwTOC.Size = new System.Drawing.Size(253, 863);
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlReportType);
            this.splitContainer1.Panel2.Controls.Add(this.pnlReportInfo);
            this.splitContainer1.Size = new System.Drawing.Size(1956, 863);
            this.splitContainer1.SplitterDistance = 253;
            // 
            // helpLink1
            // 
            this.helpLink1.HelpPage = "process_manager_dialog.htm";
            // 
            // pnlReportInfo
            // 
            this.pnlReportInfo.CollAddField = ((System.Collections.Generic.List<string>)(resources.GetObject("pnlReportInfo.CollAddField")));
            this.pnlReportInfo.CustomFields = null;
            this.pnlReportInfo.CustomFieldsTable = null;
            this.pnlReportInfo.Dataset = null;
            this.pnlReportInfo.DefaultFields = null;
            this.pnlReportInfo.DefaultFieldsTable = null;
            this.pnlReportInfo.Editable = true;
            this.pnlReportInfo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlReportInfo.IsActivePanel = false;
            this.pnlReportInfo.Location = new System.Drawing.Point(16, 337);
            this.pnlReportInfo.MinimumSize = new System.Drawing.Size(618, 307);
            this.pnlReportInfo.Name = "pnlReportInfo";
            this.pnlReportInfo.NumberOfGridRows = 0;
            this.pnlReportInfo.Padding = new System.Windows.Forms.Padding(3);
            this.pnlReportInfo.ReportFieldRows = null;
            this.pnlReportInfo.ReportFields = null;
            this.pnlReportInfo.ReportId = 0;
            this.pnlReportInfo.Size = new System.Drawing.Size(618, 307);
            this.pnlReportInfo.TabIndex = 2;
            // 
            // pnlReportType
            // 
            this.pnlReportType.Dataset = null;
            this.pnlReportType.Editable = true;
            this.pnlReportType.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlReportType.IsActivePanel = false;
            this.pnlReportType.Location = new System.Drawing.Point(16, 24);
            this.pnlReportType.MinimumSize = new System.Drawing.Size(618, 307);
            this.pnlReportType.Name = "pnlReportType";
            this.pnlReportType.Padding = new System.Windows.Forms.Padding(3);
            this.pnlReportType.Size = new System.Drawing.Size(618, 307);
            this.pnlReportType.TabIndex = 3;
            // 
            // dsReportFields
            // 
            this.dsReportFields.DataSetName = "ReportFieldsDataSet";
            this.dsReportFields.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taReportFields
            // 
            this.taReportFields.ClearBeforeFill = true;
            // 
            // taReportFieldsCustomerSummary
            // 
            this.taReportFieldsCustomerSummary.ClearBeforeFill = true;
            // 
            // taManager
            // 
            this.taManager.BackupDataSetBeforeUpdate = false;
            this.taManager.ReportFieldsCustomerSummaryTableAdapter = this.taReportFieldsCustomerSummary;
            this.taManager.ReportFieldsTableAdapter = this.taReportFields;
            this.taManager.ReportTableAdapter = this.taReport;
            this.taManager.UpdateOrder = DWOS.Data.Datasets.ReportFieldsDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            // 
            // taReport
            // 
            this.taReport.ClearBeforeFill = true;
            // 
            // taCustomField
            // 
            this.taCustomField.ClearBeforeFill = true;
            // 
            // ReportManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(1988, 1100);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReportManager";
            this.Text = "Report Manager";
            this.Load += new System.EventHandler(this.ReportManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dsReportFields)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private ReportPanels.ReportInfoPanel pnlReportInfo;
        private ReportPanels.ReportTypePanel pnlReportType;
        private Data.Datasets.ReportFieldsDataSet dsReportFields;
        private Data.Datasets.ReportFieldsDataSetTableAdapters.ReportFieldsTableAdapter taReportFields;
        private Data.Datasets.ReportFieldsDataSetTableAdapters.ReportFieldsCustomerSummaryTableAdapter taReportFieldsCustomerSummary;
        private Data.Datasets.ReportFieldsDataSetTableAdapters.TableAdapterManager taManager;
        private Data.Datasets.ReportFieldsDataSetTableAdapters.ReportTableAdapter taReport;
        private Data.Datasets.ReportFieldsDataSetTableAdapters.CustomFieldNameTableAdapter taCustomField;
    }
}
