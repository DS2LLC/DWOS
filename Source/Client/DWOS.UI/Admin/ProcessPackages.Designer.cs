namespace DWOS.UI.Admin
{
	partial class ProcessPackages
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
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessPackages));
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab7 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbon1");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup13 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup1");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup14 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup2");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("UpdateProcesses");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar7 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("Main");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("UpdateProcesses");
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            this.cboCustomerFilter = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.dpProcessPackageInfo = new DWOS.UI.Admin.ProcessPackagePanels.ProcessPackageInfo();
            this.taProcessPackage_Processes = new DWOS.Data.Datasets.ProcessPackageDatasetTableAdapters.ProcessPackage_ProcessesTableAdapter();
            this.taProcessPackage = new DWOS.Data.Datasets.ProcessPackageDatasetTableAdapters.ProcessPackageTableAdapter();
            this.dsProcesses = new DWOS.Data.Datasets.ProcessPackageDataset();
            this.taProcess = new DWOS.Data.Datasets.ProcessPackageDatasetTableAdapters.ProcessTableAdapter();
            this.taManager = new DWOS.Data.Datasets.ProcessPackageDatasetTableAdapters.TableAdapterManager();
            this.taProcessAlias = new DWOS.Data.Datasets.ProcessPackageDatasetTableAdapters.ProcessAliasTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomerFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsProcesses)).BeginInit();
            this.SuspendLayout();
            // 
            // toolbarManager
            // 
            this.toolbarManager.MenuSettings.ForceSerialization = true;
            appearance44.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance44.ImageBackground")));
            appearance44.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.toolbarManager.Ribbon.ApplicationMenu2010.ContentArea.Settings.Appearance = appearance44;
            this.toolbarManager.Ribbon.FileMenuStyle = Infragistics.Win.UltraWinToolbars.FileMenuStyle.ApplicationMenu2010;
            ribbonTab7.Caption = "Home";
            ribbonGroup13.Caption = "Packages";
            buttonTool3.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool4.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup13.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool3,
            buttonTool4});
            ribbonGroup14.Caption = "Edit";
            buttonTool5.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup14.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool5});
            ribbonTab7.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup13,
            ribbonGroup14});
            this.toolbarManager.Ribbon.NonInheritedRibbonTabs.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonTab[] {
            ribbonTab7});
            this.toolbarManager.Ribbon.Visible = true;
            ultraToolbar7.DockedColumn = 0;
            ultraToolbar7.DockedRow = 0;
            ultraToolbar7.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2});
            ultraToolbar7.Text = "Main";
            this.toolbarManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar7});
            this.toolbarManager.ToolbarSettings.ForceSerialization = true;
            appearance45.Image = global::DWOS.UI.Properties.Resources.Add_32;
            buttonTool6.SharedPropsInternal.AppearancesLarge.Appearance = appearance45;
            appearance46.Image = ((object)(resources.GetObject("appearance46.Image")));
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance46;
            buttonTool6.SharedPropsInternal.Caption = "Add";
            buttonTool6.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool6.SharedPropsInternal.Enabled = false;
            buttonTool6.SharedPropsInternal.Shortcut = System.Windows.Forms.Shortcut.Ins;
            buttonTool6.SharedPropsInternal.ToolTipTextFormatted = "Add process package.";
            buttonTool6.SharedPropsInternal.ToolTipTitle = "Add";
            appearance47.Image = global::DWOS.UI.Properties.Resources.Delete_32;
            buttonTool7.SharedPropsInternal.AppearancesLarge.Appearance = appearance47;
            appearance48.Image = ((object)(resources.GetObject("appearance48.Image")));
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance48;
            buttonTool7.SharedPropsInternal.Caption = "Delete";
            buttonTool7.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool7.SharedPropsInternal.Enabled = false;
            buttonTool7.SharedPropsInternal.Shortcut = System.Windows.Forms.Shortcut.Del;
            buttonTool7.SharedPropsInternal.ToolTipTextFormatted = "Delete process package.";
            buttonTool7.SharedPropsInternal.ToolTipTitle = "Delete";
            appearance49.Image = global::DWOS.UI.Properties.Resources.Update_32;
            buttonTool8.SharedPropsInternal.AppearancesLarge.Appearance = appearance49;
            appearance50.Image = global::DWOS.UI.Properties.Resources.Update_24;
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance50;
            buttonTool8.SharedPropsInternal.Caption = "Update Processes";
            buttonTool8.SharedPropsInternal.Enabled = false;
            buttonTool8.SharedPropsInternal.ToolTipText = "Update any out of date processes for this package.";
            buttonTool8.SharedPropsInternal.ToolTipTitle = "Update Processes";
            this.toolbarManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool6,
            buttonTool7,
            buttonTool8});
            // 
            // tvwTOC
            // 
            this.tvwTOC.Size = new System.Drawing.Size(204, 799);
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cboCustomerFilter);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.splitContainer1.Panel2.Controls.Add(this.dpProcessPackageInfo);
            this.splitContainer1.Size = new System.Drawing.Size(890, 799);
            this.splitContainer1.SplitterDistance = 204;
            // 
            // helpLink1
            // 
            this.helpLink1.HelpPage = "process_packages_dialog.htm";
            // 
            // cboCustomerFilter
            // 
            appearance8.Image = global::DWOS.UI.Properties.Resources.Customer;
            this.cboCustomerFilter.Appearance = appearance8;
            this.cboCustomerFilter.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
            this.cboCustomerFilter.AutoSize = false;
            this.cboCustomerFilter.DropDownListWidth = -1;
            this.cboCustomerFilter.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCustomerFilter.Location = new System.Drawing.Point(12, 101);
            this.cboCustomerFilter.Name = "cboCustomerFilter";
            this.cboCustomerFilter.Nullable = false;
            this.cboCustomerFilter.Size = new System.Drawing.Size(183, 22);
            this.cboCustomerFilter.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboCustomerFilter.TabIndex = 25;
            this.cboCustomerFilter.Visible = false;
            this.cboCustomerFilter.ValueChanged += new System.EventHandler(this.cboCustomerFilter_ValueChanged);
            // 
            // dpProcessPackageInfo
            // 
            this.dpProcessPackageInfo.AutoSize = true;
            this.dpProcessPackageInfo.Dataset = null;
            this.dpProcessPackageInfo.Editable = true;
            this.dpProcessPackageInfo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dpProcessPackageInfo.IsActivePanel = false;
            this.dpProcessPackageInfo.Location = new System.Drawing.Point(6, 6);
            this.dpProcessPackageInfo.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.dpProcessPackageInfo.Name = "dpProcessPackageInfo";
            this.dpProcessPackageInfo.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.dpProcessPackageInfo.Size = new System.Drawing.Size(366, 268);
            this.dpProcessPackageInfo.TabIndex = 0;
            // 
            // taProcessPackage_Processes
            // 
            this.taProcessPackage_Processes.ClearBeforeFill = true;
            // 
            // taProcessPackage
            // 
            this.taProcessPackage.ClearBeforeFill = true;
            // 
            // dsProcesses
            // 
            this.dsProcesses.DataSetName = "ProcessPackageDataset";
            this.dsProcesses.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taProcess
            // 
            this.taProcess.ClearBeforeFill = true;
            // 
            // taManager
            // 
            this.taManager.BackupDataSetBeforeUpdate = false;
            this.taManager.ProcessPackage_ProcessesTableAdapter = this.taProcessPackage_Processes;
            this.taManager.ProcessPackageTableAdapter = this.taProcessPackage;
            this.taManager.UpdateOrder = DWOS.Data.Datasets.ProcessPackageDatasetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            // 
            // taProcessAlias
            // 
            this.taProcessAlias.ClearBeforeFill = true;
            // 
            // ProcessPackages
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(906, 1022);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProcessPackages";
            this.Text = "Process Packages";
            this.Load += new System.EventHandler(this.Part_Load);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomerFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsProcesses)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomerFilter;
		private DWOS.UI.Admin.ProcessPackagePanels.ProcessPackageInfo dpProcessPackageInfo;
		private DWOS.Data.Datasets.ProcessPackageDatasetTableAdapters.ProcessPackage_ProcessesTableAdapter taProcessPackage_Processes;
		private DWOS.Data.Datasets.ProcessPackageDatasetTableAdapters.ProcessPackageTableAdapter taProcessPackage;
		private DWOS.Data.Datasets.ProcessPackageDataset dsProcesses;
		private DWOS.Data.Datasets.ProcessPackageDatasetTableAdapters.ProcessTableAdapter taProcess;
		private DWOS.Data.Datasets.ProcessPackageDatasetTableAdapters.TableAdapterManager taManager;
        private DWOS.Data.Datasets.ProcessPackageDatasetTableAdapters.ProcessAliasTableAdapter taProcessAlias;
	}
}
