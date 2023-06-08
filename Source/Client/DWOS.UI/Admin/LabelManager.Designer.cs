namespace DWOS.UI.Admin
{
    partial class LabelManager
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
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddProductClass");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Edit");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool54 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Import");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Export");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Copy");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Paste");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Export");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool47 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Import");
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Edit");
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddProductClass");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LabelManager));
            this.dsLabels = new DWOS.Data.Datasets.LabelDataSet();
            this.taLabelCustomerSummary = new DWOS.Data.Datasets.LabelDataSetTableAdapters.LabelCustomerSummaryTableAdapter();
            this.taLabels = new DWOS.Data.Datasets.LabelDataSetTableAdapters.LabelsTableAdapter();
            this.taManager = new DWOS.Data.Datasets.LabelDataSetTableAdapters.TableAdapterManager();
            this.taLabelMedia = new DWOS.Data.Datasets.LabelDataSetTableAdapters.LabelMediaTableAdapter();
            this.taLabelType = new DWOS.Data.Datasets.LabelDataSetTableAdapters.LabelTypeTableAdapter();
            this.taProductClassLabels = new DWOS.Data.Datasets.LabelDataSetTableAdapters.ProductClassLabelsTableAdapter();
            this.pnlLabelInfo = new DWOS.UI.Admin.LabelPanels.LabelInfoPanel();
            this.pnlLabelType = new DWOS.UI.Admin.LabelPanels.LabelTypePanel();
            this.pnlLabelProductClass = new DWOS.UI.Admin.LabelPanels.LabelProductClassPanel();
            this.taProductClass = new DWOS.Data.Datasets.LabelDataSetTableAdapters.ProductClassTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsLabels)).BeginInit();
            this.SuspendLayout();
            // 
            // toolbarManager
            // 
            this.toolbarManager.MenuSettings.ForceSerialization = true;
            this.toolbarManager.Office2007UICompatibility = false;
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
            ribbonGroup1.Caption = "Labels";
            ribbonGroup1.PreferredToolSize = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool54.InstanceProps.IsFirstInGroup = true;
            ribbonGroup1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool27,
            buttonTool5,
            buttonTool7,
            buttonTool54,
            buttonTool14,
            buttonTool15});
            ribbonTab1.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup1});
            this.toolbarManager.Ribbon.NonInheritedRibbonTabs.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonTab[] {
            ribbonTab1});
            this.toolbarManager.Ribbon.Visible = true;
            this.toolbarManager.ToolbarSettings.ForceSerialization = true;
            appearance2.Image = global::DWOS.UI.Properties.Resources.Delete_32;
            buttonTool8.SharedPropsInternal.AppearancesLarge.Appearance = appearance2;
            buttonTool8.SharedPropsInternal.Caption = "Delete";
            buttonTool8.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool8.SharedPropsInternal.Enabled = false;
            appearance3.Image = global::DWOS.UI.Properties.Resources.Copy_32;
            buttonTool12.SharedPropsInternal.AppearancesLarge.Appearance = appearance3;
            buttonTool12.SharedPropsInternal.Caption = "Copy";
            buttonTool12.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool12.SharedPropsInternal.Enabled = false;
            appearance4.Image = global::DWOS.UI.Properties.Resources.editpaste;
            buttonTool13.SharedPropsInternal.AppearancesLarge.Appearance = appearance4;
            buttonTool13.SharedPropsInternal.Caption = "Paste";
            buttonTool13.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool13.SharedPropsInternal.Enabled = false;
            appearance5.Image = global::DWOS.UI.Properties.Resources.Add_32;
            buttonTool18.SharedPropsInternal.AppearancesLarge.Appearance = appearance5;
            buttonTool18.SharedPropsInternal.Caption = "Add Customer Label";
            buttonTool18.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool18.SharedPropsInternal.Enabled = false;
            buttonTool18.SharedPropsInternal.ToolTipText = "Adds a new customer label.";
            buttonTool18.SharedPropsInternal.ToolTipTitle = "Add Customer Label";
            appearance6.Image = global::DWOS.UI.Properties.Resources.Export_32;
            buttonTool23.SharedPropsInternal.AppearancesLarge.Appearance = appearance6;
            appearance7.Image = global::DWOS.UI.Properties.Resources.Export_16;
            buttonTool23.SharedPropsInternal.AppearancesSmall.Appearance = appearance7;
            buttonTool23.SharedPropsInternal.Caption = "Export";
            buttonTool23.SharedPropsInternal.Enabled = false;
            buttonTool23.SharedPropsInternal.ToolTipText = "Export the selected label to the file system.";
            buttonTool23.SharedPropsInternal.ToolTipTitle = "Export Label";
            appearance8.Image = global::DWOS.UI.Properties.Resources.Import_32;
            buttonTool47.SharedPropsInternal.AppearancesLarge.Appearance = appearance8;
            appearance9.Image = global::DWOS.UI.Properties.Resources.Import_16;
            buttonTool47.SharedPropsInternal.AppearancesSmall.Appearance = appearance9;
            buttonTool47.SharedPropsInternal.Caption = "Import";
            buttonTool47.SharedPropsInternal.Enabled = false;
            buttonTool47.SharedPropsInternal.ToolTipText = "Import a label from the file system.";
            buttonTool47.SharedPropsInternal.ToolTipTitle = "Import Label";
            appearance10.Image = global::DWOS.UI.Properties.Resources.ThermalPrinterIcon32x32;
            buttonTool1.SharedPropsInternal.AppearancesLarge.Appearance = appearance10;
            appearance11.Image = global::DWOS.UI.Properties.Resources.ThermalPrinterIcon32x32;
            buttonTool1.SharedPropsInternal.AppearancesSmall.Appearance = appearance11;
            buttonTool1.SharedPropsInternal.Caption = "Edit";
            buttonTool1.SharedPropsInternal.Enabled = false;
            appearance12.Image = global::DWOS.UI.Properties.Resources.Add_32;
            buttonTool6.SharedPropsInternal.AppearancesLarge.Appearance = appearance12;
            appearance13.Image = global::DWOS.UI.Properties.Resources.Add_16;
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance13;
            buttonTool6.SharedPropsInternal.Caption = "Add Product Class Label";
            buttonTool6.SharedPropsInternal.Enabled = false;
            buttonTool6.SharedPropsInternal.ToolTipText = "Adds a new product class label.";
            buttonTool6.SharedPropsInternal.ToolTipTitle = "Add Product Class";
            this.toolbarManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool8,
            buttonTool12,
            buttonTool13,
            buttonTool18,
            buttonTool23,
            buttonTool47,
            buttonTool1,
            buttonTool6});
            // 
            // tvwTOC
            // 
            this.toolbarManager.SetContextMenuUltra(this.tvwTOC, "TOCContext");
            this.tvwTOC.Size = new System.Drawing.Size(253, 900);
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlLabelProductClass);
            this.splitContainer1.Panel2.Controls.Add(this.pnlLabelType);
            this.splitContainer1.Panel2.Controls.Add(this.pnlLabelInfo);
            this.splitContainer1.Size = new System.Drawing.Size(1684, 900);
            this.splitContainer1.SplitterDistance = 253;
            // 
            // helpLink1
            // 
            this.helpLink1.HelpPage = "process_manager_dialog.htm";
            // 
            // dsLabels
            // 
            this.dsLabels.DataSetName = "LabelDataSet";
            this.dsLabels.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taLabelCustomerSummary
            // 
            this.taLabelCustomerSummary.ClearBeforeFill = true;
            // 
            // taLabels
            // 
            this.taLabels.ClearBeforeFill = false;
            // 
            // taManager
            // 
            this.taManager.BackupDataSetBeforeUpdate = false;
            this.taManager.LabelMediaTableAdapter = this.taLabelMedia;
            this.taManager.LabelsTableAdapter = this.taLabels;
            this.taManager.LabelTypeTableAdapter = this.taLabelType;
            this.taManager.ProductClassLabelsTableAdapter = this.taProductClassLabels;
            this.taManager.ProductClassTableAdapter = null;
            this.taManager.UpdateOrder = DWOS.Data.Datasets.LabelDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            // 
            // taLabelMedia
            // 
            this.taLabelMedia.ClearBeforeFill = false;
            // 
            // taLabelType
            // 
            this.taLabelType.ClearBeforeFill = false;
            // 
            // taProductClassLabels
            // 
            this.taProductClassLabels.ClearBeforeFill = false;
            // 
            // pnlLabelInfo
            // 
            this.pnlLabelInfo.Dataset = null;
            this.pnlLabelInfo.Editable = true;
            this.pnlLabelInfo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlLabelInfo.IsActivePanel = false;
            this.pnlLabelInfo.Location = new System.Drawing.Point(281, 110);
            this.pnlLabelInfo.Name = "pnlLabelInfo";
            this.pnlLabelInfo.Padding = new System.Windows.Forms.Padding(3);
            this.pnlLabelInfo.Size = new System.Drawing.Size(473, 307);
            this.pnlLabelInfo.TabIndex = 0;
            // 
            // pnlLabelType
            // 
            this.pnlLabelType.Dataset = null;
            this.pnlLabelType.Editable = true;
            this.pnlLabelType.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlLabelType.IsActivePanel = false;
            this.pnlLabelType.Location = new System.Drawing.Point(171, 334);
            this.pnlLabelType.Name = "pnlLabelType";
            this.pnlLabelType.Padding = new System.Windows.Forms.Padding(3);
            this.pnlLabelType.Size = new System.Drawing.Size(473, 307);
            this.pnlLabelType.TabIndex = 1;
            // 
            // pnlLabelProductClass
            // 
            this.pnlLabelProductClass.Dataset = null;
            this.pnlLabelProductClass.Editable = true;
            this.pnlLabelProductClass.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlLabelProductClass.IsActivePanel = false;
            this.pnlLabelProductClass.Location = new System.Drawing.Point(127, 94);
            this.pnlLabelProductClass.Name = "pnlLabelProductClass";
            this.pnlLabelProductClass.Padding = new System.Windows.Forms.Padding(3);
            this.pnlLabelProductClass.Size = new System.Drawing.Size(473, 307);
            this.pnlLabelProductClass.TabIndex = 2;
            // 
            // taProductClass
            // 
            this.taProductClass.ClearBeforeFill = true;
            // 
            // LabelManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(1700, 1100);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LabelManager";
            this.Text = "Label Manager";
            this.Load += new System.EventHandler(this.Processes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dsLabels)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Data.Datasets.LabelDataSet dsLabels;
        private Data.Datasets.LabelDataSetTableAdapters.LabelCustomerSummaryTableAdapter taLabelCustomerSummary;
        private Data.Datasets.LabelDataSetTableAdapters.LabelsTableAdapter taLabels;
        private Data.Datasets.LabelDataSetTableAdapters.TableAdapterManager taManager;
        private LabelPanels.LabelInfoPanel pnlLabelInfo;
        private Data.Datasets.LabelDataSetTableAdapters.LabelTypeTableAdapter taLabelType;
        private Data.Datasets.LabelDataSetTableAdapters.LabelMediaTableAdapter taLabelMedia;
        private LabelPanels.LabelTypePanel pnlLabelType;
        private Data.Datasets.LabelDataSetTableAdapters.ProductClassLabelsTableAdapter taProductClassLabels;
        private LabelPanels.LabelProductClassPanel pnlLabelProductClass;
        private Data.Datasets.LabelDataSetTableAdapters.ProductClassTableAdapter taProductClass;
    }
}
