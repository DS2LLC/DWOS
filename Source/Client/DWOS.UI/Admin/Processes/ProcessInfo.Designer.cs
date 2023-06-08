namespace DWOS.UI.Admin.Processes
{
	partial class ProcessInfo
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
		    _isDisposing = true;
            this.DisposeMe();

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
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The revision of the process.", Infragistics.Win.ToolTipImage.Default, "Revision", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The department that step will be completed in.", Infragistics.Win.ToolTipImage.Default, "Department", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton1 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The lead time (in hours) for the process. This is added to the category\'s lead ti" +
        "me to determine the due date.", Infragistics.Win.ToolTipImage.Default, "Lead Time", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Shows product classes and pricing information for this process.", Infragistics.Win.ToolTipImage.Default, "Product Classes", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The burden cost, per hour, of the process.", Infragistics.Win.ToolTipImage.Default, "Burden Cost Rate", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The product code of the process.", Infragistics.Win.ToolTipImage.Default, "Product Code", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton2 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Material Cost", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The default load capacity of the process.", Infragistics.Win.ToolTipImage.Default, "Load Capacity", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The +/- percent variance of the process\'s load capacity.", Infragistics.Win.ToolTipImage.Default, "Load Capacity Variance", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo10 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Load Capacity Type", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo11 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The price for the process.", Infragistics.Win.ToolTipImage.Default, "Price", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo12 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Status", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo13 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The minimum price for the process.", Infragistics.Win.ToolTipImage.Default, "Minimum Price", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo14 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Frozen By", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessInfo));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo15 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo16 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Paper Mode", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo17 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The process category used to group processes.", Infragistics.Win.ToolTipImage.Default, "Category", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo18 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The descriptive name of the process.", Infragistics.Win.ToolTipImage.Default, "Process Description", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo19 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date this process was last modified.", Infragistics.Win.ToolTipImage.Default, "Mod Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo20 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Process Code", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo21 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Process Alias", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo22 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Traveler Note", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo23 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Pop Up Note", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo24 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Customer Alias", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo25 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Customer", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo26 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Toggle the display of the inspection data on the COC.", Infragistics.Win.ToolTipImage.Default, "Display on COC", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo27 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Remove the selected inspection.", Infragistics.Win.ToolTipImage.Default, "Remove Inspection", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo28 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Shift selected inspection down.", Infragistics.Win.ToolTipImage.Default, "Shift Down", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo29 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Shift selected inspection up.", Infragistics.Win.ToolTipImage.Default, "Shift Up", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo30 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add an inspection to this process.", Infragistics.Win.ToolTipImage.Default, "Add Inspection", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinListView.UltraListViewItem ultraListViewItem1 = new Infragistics.Win.UltraWinListView.UltraListViewItem("1", new Infragistics.Win.UltraWinListView.UltraListViewSubItem[] {
            new Infragistics.Win.UltraWinListView.UltraListViewSubItem("Test Process", null),
            new Infragistics.Win.UltraWinListView.UltraListViewSubItem(null, null),
            new Infragistics.Win.UltraWinListView.UltraListViewSubItem("<None>", null)}, null);
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Inspection");
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn2 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("COC");
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn3 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Revision");
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo31 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Inspections", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ChildProcessID", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Hours");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessRequisiteID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ParentProcessID");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo32 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Import suggestions from a part with this process.", Infragistics.Win.ToolTipImage.Default, "Import", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo33 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Edits the selected suggestion.", Infragistics.Win.ToolTipImage.Default, "Edit Suggestion", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo34 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Delete the selected suggestion.", Infragistics.Win.ToolTipImage.Default, "Delete Suggestion", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Type");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DepartmentId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessAliasName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Condition");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Row");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status");
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo35 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add a suggested process to this process.", Infragistics.Win.ToolTipImage.Default, "Add Suggestion", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.cboProcessRev = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboProcessStepDept = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.numLeadTime = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.lblLeadTime = new Infragistics.Win.Misc.UltraLabel();
            this.btnProcessProductClass = new Infragistics.Win.Misc.UltraButton();
            this.curBurdenRate = new Infragistics.Win.UltraWinEditors.UltraCurrencyEditor();
            this.ultraLabel26 = new Infragistics.Win.Misc.UltraLabel();
            this.txtShortCode = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel25 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel24 = new Infragistics.Win.Misc.UltraLabel();
            this.curMaterialCost = new Infragistics.Win.UltraWinEditors.UltraCurrencyEditor();
            this.pnlLoadCapacity = new System.Windows.Forms.Panel();
            this.ultraLabel21 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel23 = new Infragistics.Win.Misc.UltraLabel();
            this.numLoadCapacity = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.numLoadCapacityVariance = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel22 = new Infragistics.Win.Misc.UltraLabel();
            this.cboLoadCapacityType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel20 = new Infragistics.Win.Misc.UltraLabel();
            this.numProcessPrice = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.cboStatus = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel19 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel18 = new Infragistics.Win.Misc.UltraLabel();
            this.numProcessMinPrice = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtFrozenBy = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel17 = new Infragistics.Win.Misc.UltraLabel();
            this.picFrozen = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraLabel16 = new Infragistics.Win.Misc.UltraLabel();
            this.txtFrozenDate = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.optPaperless = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
            this.cboProcessCategory = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel11 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel10 = new Infragistics.Win.Misc.UltraLabel();
            this.docLinkManagerProcess = new DWOS.UI.Documents.DocumentLinkManager();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel14 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.txtProcessDesc = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.dtModDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.txtProcessName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.splitAliases = new System.Windows.Forms.SplitContainer();
            this.tvwProcessAliases = new Infragistics.Win.UltraWinTree.UltraTree();
            this.pnlProcessAlias = new Infragistics.Win.Misc.UltraPanel();
            this.ultraLabel15 = new Infragistics.Win.Misc.UltraLabel();
            this.docLinkManagerProcessAlias = new DWOS.UI.Documents.DocumentLinkManager();
            this.txtProcessAliasName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.txtTravelerNote = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtPopupNote = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.pnlCustomerProcessAlias = new Infragistics.Win.Misc.UltraPanel();
            this.ultraLabel9 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCustomerProcessAliasName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboCustomer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.btnInspectionCheck = new Infragistics.Win.Misc.UltraButton();
            this.btnRemoveInspection = new Infragistics.Win.Misc.UltraButton();
            this.btnInspectionDown = new Infragistics.Win.Misc.UltraButton();
            this.btnInspectionUp = new Infragistics.Win.Misc.UltraButton();
            this.btnAddInspection = new Infragistics.Win.Misc.UltraButton();
            this.lvwInspections = new Infragistics.Win.UltraWinListView.UltraListView();
            this.ultraTabPageControl4 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraLabel13 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel12 = new Infragistics.Win.Misc.UltraLabel();
            this.grdConstraints = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraTabPageControl5 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.btnImportSuggestions = new Infragistics.Win.Misc.UltraButton();
            this.btnEditSuggestion = new Infragistics.Win.Misc.UltraButton();
            this.btnDeleteSuggestion = new Infragistics.Win.Misc.UltraButton();
            this.grdSuggestions = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnAddSuggestion = new Infragistics.Win.Misc.UltraButton();
            this.tabProcessInfo = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.bsConstraints = new System.Windows.Forms.BindingSource(this.components);
            this.imagelistLocks = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessRev)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessStepDept)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLeadTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.curBurdenRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShortCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.curMaterialCost)).BeginInit();
            this.pnlLoadCapacity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLoadCapacity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLoadCapacityVariance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLoadCapacityType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProcessPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProcessMinPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtFrozenBy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFrozenDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.optPaperless)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessCategory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProcessDesc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtModDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProcessName)).BeginInit();
            this.ultraTabPageControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitAliases)).BeginInit();
            this.splitAliases.Panel1.SuspendLayout();
            this.splitAliases.Panel2.SuspendLayout();
            this.splitAliases.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tvwProcessAliases)).BeginInit();
            this.pnlProcessAlias.ClientArea.SuspendLayout();
            this.pnlProcessAlias.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtProcessAliasName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTravelerNote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPopupNote)).BeginInit();
            this.pnlCustomerProcessAlias.ClientArea.SuspendLayout();
            this.pnlCustomerProcessAlias.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerProcessAliasName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).BeginInit();
            this.ultraTabPageControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvwInspections)).BeginInit();
            this.ultraTabPageControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdConstraints)).BeginInit();
            this.ultraTabPageControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdSuggestions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabProcessInfo)).BeginInit();
            this.tabProcessInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsConstraints)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Bottom = 5;
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.tabProcessInfo);
            appearance42.Image = global::DWOS.UI.Properties.Resources.Process_16;
            this.grpData.HeaderAppearance = appearance42;
            this.grpData.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grpData.Size = new System.Drawing.Size(1242, 1074);
            this.grpData.Text = "Process Information";
            this.grpData.WrapText = false;
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.tabProcessInfo, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(21598, 11638);
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.cboProcessRev);
            this.ultraTabPageControl1.Controls.Add(this.cboProcessStepDept);
            this.ultraTabPageControl1.Controls.Add(this.numLeadTime);
            this.ultraTabPageControl1.Controls.Add(this.lblLeadTime);
            this.ultraTabPageControl1.Controls.Add(this.btnProcessProductClass);
            this.ultraTabPageControl1.Controls.Add(this.curBurdenRate);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel26);
            this.ultraTabPageControl1.Controls.Add(this.txtShortCode);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel25);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel24);
            this.ultraTabPageControl1.Controls.Add(this.curMaterialCost);
            this.ultraTabPageControl1.Controls.Add(this.pnlLoadCapacity);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel20);
            this.ultraTabPageControl1.Controls.Add(this.numProcessPrice);
            this.ultraTabPageControl1.Controls.Add(this.cboStatus);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel19);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel18);
            this.ultraTabPageControl1.Controls.Add(this.numProcessMinPrice);
            this.ultraTabPageControl1.Controls.Add(this.ultraGroupBox1);
            this.ultraTabPageControl1.Controls.Add(this.optPaperless);
            this.ultraTabPageControl1.Controls.Add(this.cboProcessCategory);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel11);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel10);
            this.ultraTabPageControl1.Controls.Add(this.docLinkManagerProcess);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel3);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel14);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel1);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel4);
            this.ultraTabPageControl1.Controls.Add(this.txtProcessDesc);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel2);
            this.ultraTabPageControl1.Controls.Add(this.dtModDate);
            this.ultraTabPageControl1.Controls.Add(this.txtProcessName);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Padding = new System.Windows.Forms.Padding(5);
            this.ultraTabPageControl1.Size = new System.Drawing.Size(806, 655);
            // 
            // cboProcessRev
            // 
            this.cboProcessRev.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            this.cboProcessRev.DropDownListWidth = -1;
            this.cboProcessRev.Location = new System.Drawing.Point(356, 64);
            this.cboProcessRev.MaxLength = 50;
            this.cboProcessRev.Name = "cboProcessRev";
            this.cboProcessRev.NullText = "<None>";
            this.cboProcessRev.Size = new System.Drawing.Size(158, 22);
            this.cboProcessRev.TabIndex = 4;
            this.cboProcessRev.Text = "<None>";
            ultraToolTipInfo1.ToolTipText = "The revision of the process.";
            ultraToolTipInfo1.ToolTipTitle = "Revision";
            this.tipManager.SetUltraToolTip(this.cboProcessRev, ultraToolTipInfo1);
            // 
            // cboProcessStepDept
            // 
            this.cboProcessStepDept.DropDownListWidth = -1;
            this.cboProcessStepDept.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboProcessStepDept.Location = new System.Drawing.Point(134, 92);
            this.cboProcessStepDept.MaxLength = 50;
            this.cboProcessStepDept.Name = "cboProcessStepDept";
            this.cboProcessStepDept.Size = new System.Drawing.Size(142, 22);
            this.cboProcessStepDept.TabIndex = 6;
            ultraToolTipInfo2.ToolTipText = "The department that step will be completed in.";
            ultraToolTipInfo2.ToolTipTitle = "Department";
            this.tipManager.SetUltraToolTip(this.cboProcessStepDept, ultraToolTipInfo2);
            // 
            // numLeadTime
            // 
            this.numLeadTime.ButtonsRight.Add(dropDownEditorButton1);
            this.numLeadTime.Location = new System.Drawing.Point(613, 120);
            this.numLeadTime.MaskInput = "nnn.nnnn hr";
            this.numLeadTime.MaxValue = new decimal(new int[] {
            9999999,
            0,
            0,
            262144});
            this.numLeadTime.MinValue = 0;
            this.numLeadTime.Name = "numLeadTime";
            this.numLeadTime.Nullable = true;
            this.numLeadTime.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.numLeadTime.Size = new System.Drawing.Size(185, 22);
            this.numLeadTime.TabIndex = 11;
            ultraToolTipInfo3.ToolTipText = "The lead time (in hours) for the process. This is added to the category\'s lead ti" +
    "me to determine the due date.";
            ultraToolTipInfo3.ToolTipTitle = "Lead Time";
            this.tipManager.SetUltraToolTip(this.numLeadTime, ultraToolTipInfo3);
            // 
            // lblLeadTime
            // 
            this.lblLeadTime.AutoSize = true;
            this.lblLeadTime.Location = new System.Drawing.Point(522, 124);
            this.lblLeadTime.Name = "lblLeadTime";
            this.lblLeadTime.Size = new System.Drawing.Size(68, 15);
            this.lblLeadTime.TabIndex = 77;
            this.lblLeadTime.Text = "Lead Time:";
            // 
            // btnProcessProductClass
            // 
            this.btnProcessProductClass.Location = new System.Drawing.Point(282, 148);
            this.btnProcessProductClass.Name = "btnProcessProductClass";
            this.btnProcessProductClass.Size = new System.Drawing.Size(232, 23);
            this.btnProcessProductClass.TabIndex = 13;
            this.btnProcessProductClass.Text = "Product Classes...";
            ultraToolTipInfo4.ToolTipText = "Shows product classes and pricing information for this process.";
            ultraToolTipInfo4.ToolTipTitle = "Product Classes";
            this.tipManager.SetUltraToolTip(this.btnProcessProductClass, ultraToolTipInfo4);
            this.btnProcessProductClass.Click += new System.EventHandler(this.btnProcessProductClass_Click);
            // 
            // curBurdenRate
            // 
            this.curBurdenRate.Location = new System.Drawing.Point(613, 148);
            this.curBurdenRate.MaskInput = "$ nnnn.nn";
            this.curBurdenRate.MaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            327680});
            this.curBurdenRate.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.curBurdenRate.Name = "curBurdenRate";
            this.curBurdenRate.Size = new System.Drawing.Size(185, 22);
            this.curBurdenRate.TabIndex = 14;
            ultraToolTipInfo5.ToolTipText = "The burden cost, per hour, of the process.";
            ultraToolTipInfo5.ToolTipTitle = "Burden Cost Rate";
            this.tipManager.SetUltraToolTip(this.curBurdenRate, ultraToolTipInfo5);
            // 
            // ultraLabel26
            // 
            this.ultraLabel26.AutoSize = true;
            this.ultraLabel26.Location = new System.Drawing.Point(522, 152);
            this.ultraLabel26.Name = "ultraLabel26";
            this.ultraLabel26.Size = new System.Drawing.Size(79, 15);
            this.ultraLabel26.TabIndex = 76;
            this.ultraLabel26.Text = "Burden Cost:";
            // 
            // txtShortCode
            // 
            this.txtShortCode.Location = new System.Drawing.Point(613, 92);
            this.txtShortCode.MaxLength = 20;
            this.txtShortCode.Name = "txtShortCode";
            this.txtShortCode.Size = new System.Drawing.Size(185, 22);
            this.txtShortCode.TabIndex = 8;
            ultraToolTipInfo6.ToolTipText = "The product code of the process.";
            ultraToolTipInfo6.ToolTipTitle = "Product Code";
            this.tipManager.SetUltraToolTip(this.txtShortCode, ultraToolTipInfo6);
            // 
            // ultraLabel25
            // 
            this.ultraLabel25.AutoSize = true;
            this.ultraLabel25.Location = new System.Drawing.Point(522, 96);
            this.ultraLabel25.Name = "ultraLabel25";
            this.ultraLabel25.Size = new System.Drawing.Size(85, 15);
            this.ultraLabel25.TabIndex = 75;
            this.ultraLabel25.Text = "Product Code:";
            // 
            // ultraLabel24
            // 
            this.ultraLabel24.AutoSize = true;
            this.ultraLabel24.Location = new System.Drawing.Point(13, 152);
            this.ultraLabel24.Name = "ultraLabel24";
            this.ultraLabel24.Size = new System.Drawing.Size(115, 15);
            this.ultraLabel24.TabIndex = 74;
            this.ultraLabel24.Text = "Base Material Cost:";
            // 
            // curMaterialCost
            // 
            dropDownEditorButton2.Text = "";
            this.curMaterialCost.ButtonsRight.Add(dropDownEditorButton2);
            this.curMaterialCost.Location = new System.Drawing.Point(134, 148);
            this.curMaterialCost.MaskInput = "$ nnnn.nn";
            this.curMaterialCost.MaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            327680});
            this.curMaterialCost.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.curMaterialCost.Name = "curMaterialCost";
            this.curMaterialCost.Size = new System.Drawing.Size(142, 22);
            this.curMaterialCost.TabIndex = 12;
            ultraToolTipInfo7.ToolTipTextFormatted = "The base per-unit material cost of the process.<br/>This price is added to any pr" +
    "oduct class prices<br/>for the final material cost.";
            ultraToolTipInfo7.ToolTipTitle = "Material Cost";
            this.tipManager.SetUltraToolTip(this.curMaterialCost, ultraToolTipInfo7);
            // 
            // pnlLoadCapacity
            // 
            this.pnlLoadCapacity.BackColor = System.Drawing.Color.Transparent;
            this.pnlLoadCapacity.Controls.Add(this.ultraLabel21);
            this.pnlLoadCapacity.Controls.Add(this.ultraLabel23);
            this.pnlLoadCapacity.Controls.Add(this.numLoadCapacity);
            this.pnlLoadCapacity.Controls.Add(this.numLoadCapacityVariance);
            this.pnlLoadCapacity.Controls.Add(this.ultraLabel22);
            this.pnlLoadCapacity.Controls.Add(this.cboLoadCapacityType);
            this.pnlLoadCapacity.Location = new System.Drawing.Point(13, 176);
            this.pnlLoadCapacity.Name = "pnlLoadCapacity";
            this.pnlLoadCapacity.Size = new System.Drawing.Size(785, 22);
            this.pnlLoadCapacity.TabIndex = 15;
            // 
            // ultraLabel21
            // 
            this.ultraLabel21.AutoSize = true;
            this.ultraLabel21.Location = new System.Drawing.Point(0, 4);
            this.ultraLabel21.Name = "ultraLabel21";
            this.ultraLabel21.Size = new System.Drawing.Size(89, 15);
            this.ultraLabel21.TabIndex = 78;
            this.ultraLabel21.Text = "Load Capacity:";
            // 
            // ultraLabel23
            // 
            this.ultraLabel23.AutoSize = true;
            this.ultraLabel23.Location = new System.Drawing.Point(509, 4);
            this.ultraLabel23.Name = "ultraLabel23";
            this.ultraLabel23.Size = new System.Drawing.Size(58, 15);
            this.ultraLabel23.TabIndex = 80;
            this.ultraLabel23.Text = "Variance:";
            // 
            // numLoadCapacity
            // 
            this.numLoadCapacity.Location = new System.Drawing.Point(121, 0);
            this.numLoadCapacity.MaskInput = "nnnnnnnnn";
            this.numLoadCapacity.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numLoadCapacity.Name = "numLoadCapacity";
            this.numLoadCapacity.Nullable = true;
            this.numLoadCapacity.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.numLoadCapacity.Size = new System.Drawing.Size(142, 22);
            this.numLoadCapacity.TabIndex = 1;
            ultraToolTipInfo8.ToolTipText = "The default load capacity of the process.";
            ultraToolTipInfo8.ToolTipTitle = "Load Capacity";
            this.tipManager.SetUltraToolTip(this.numLoadCapacity, ultraToolTipInfo8);
            // 
            // numLoadCapacityVariance
            // 
            this.numLoadCapacityVariance.Location = new System.Drawing.Point(600, 0);
            this.numLoadCapacityVariance.MaskInput = "nn.n%";
            this.numLoadCapacityVariance.MaxValue = new decimal(new int[] {
            999,
            0,
            0,
            65536});
            this.numLoadCapacityVariance.MinValue = 0;
            this.numLoadCapacityVariance.Name = "numLoadCapacityVariance";
            this.numLoadCapacityVariance.Nullable = true;
            this.numLoadCapacityVariance.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.numLoadCapacityVariance.Size = new System.Drawing.Size(185, 22);
            this.numLoadCapacityVariance.TabIndex = 3;
            ultraToolTipInfo9.ToolTipText = "The +/- percent variance of the process\'s load capacity.";
            ultraToolTipInfo9.ToolTipTitle = "Load Capacity Variance";
            this.tipManager.SetUltraToolTip(this.numLoadCapacityVariance, ultraToolTipInfo9);
            this.numLoadCapacityVariance.ValueChanged += new System.EventHandler(this.numLoadCapacityVariance_ValueChanged);
            // 
            // ultraLabel22
            // 
            this.ultraLabel22.AutoSize = true;
            this.ultraLabel22.Location = new System.Drawing.Point(269, 4);
            this.ultraLabel22.Name = "ultraLabel22";
            this.ultraLabel22.Size = new System.Drawing.Size(68, 15);
            this.ultraLabel22.TabIndex = 79;
            this.ultraLabel22.Text = "Load Type:";
            // 
            // cboLoadCapacityType
            // 
            this.cboLoadCapacityType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem6.DataValue = "Quantity";
            valueListItem7.DataValue = "Weight";
            this.cboLoadCapacityType.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem6,
            valueListItem7});
            this.cboLoadCapacityType.Location = new System.Drawing.Point(343, 0);
            this.cboLoadCapacityType.Name = "cboLoadCapacityType";
            this.cboLoadCapacityType.Size = new System.Drawing.Size(158, 22);
            this.cboLoadCapacityType.TabIndex = 2;
            ultraToolTipInfo10.ToolTipTextFormatted = "The default load capacity\'s type.<br/><strong>Weight</strong> is the default type" +
    ".";
            ultraToolTipInfo10.ToolTipTitle = "Load Capacity Type";
            this.tipManager.SetUltraToolTip(this.cboLoadCapacityType, ultraToolTipInfo10);
            this.cboLoadCapacityType.ValueChanged += new System.EventHandler(this.cboLoadCapacityType_ValueChanged);
            // 
            // ultraLabel20
            // 
            this.ultraLabel20.AutoSize = true;
            this.ultraLabel20.Location = new System.Drawing.Point(282, 124);
            this.ultraLabel20.Name = "ultraLabel20";
            this.ultraLabel20.Size = new System.Drawing.Size(37, 15);
            this.ultraLabel20.TabIndex = 72;
            this.ultraLabel20.Text = "Price:";
            // 
            // numProcessPrice
            // 
            this.numProcessPrice.Location = new System.Drawing.Point(356, 120);
            this.numProcessPrice.MaskInput = "$ nnnn.nn";
            this.numProcessPrice.MaxValue = 9999.99999D;
            this.numProcessPrice.MinValue = 0;
            this.numProcessPrice.Name = "numProcessPrice";
            this.numProcessPrice.Nullable = true;
            this.numProcessPrice.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.numProcessPrice.Size = new System.Drawing.Size(158, 22);
            this.numProcessPrice.TabIndex = 10;
            this.numProcessPrice.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextControl;
            ultraToolTipInfo11.ToolTipText = "The price for the process.";
            ultraToolTipInfo11.ToolTipTitle = "Price";
            this.tipManager.SetUltraToolTip(this.numProcessPrice, ultraToolTipInfo11);
            // 
            // cboStatus
            // 
            this.cboStatus.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem3.DataValue = "Planned";
            valueListItem4.DataValue = "Approved";
            valueListItem5.DataValue = "Closed";
            this.cboStatus.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem3,
            valueListItem4,
            valueListItem5});
            this.cboStatus.Location = new System.Drawing.Point(134, 64);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(142, 22);
            this.cboStatus.TabIndex = 3;
            ultraToolTipInfo12.ToolTipTextFormatted = "The current status of the process.<br/><b>Approved</b> - The process can be added" +
    " to parts.<br/><b>Planned</b> - The process is in planning.<br/><b>Closed</b> - " +
    "The process is inactive.";
            ultraToolTipInfo12.ToolTipTitle = "Status";
            this.tipManager.SetUltraToolTip(this.cboStatus, ultraToolTipInfo12);
            this.cboStatus.SelectionChangeCommitted += new System.EventHandler(this.cboStatus_SelectionChangeCommitted);
            // 
            // ultraLabel19
            // 
            this.ultraLabel19.AutoSize = true;
            this.ultraLabel19.Location = new System.Drawing.Point(13, 68);
            this.ultraLabel19.Name = "ultraLabel19";
            this.ultraLabel19.Size = new System.Drawing.Size(46, 15);
            this.ultraLabel19.TabIndex = 70;
            this.ultraLabel19.Text = "Status:";
            // 
            // ultraLabel18
            // 
            this.ultraLabel18.AutoSize = true;
            this.ultraLabel18.Location = new System.Drawing.Point(13, 124);
            this.ultraLabel18.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel18.Name = "ultraLabel18";
            this.ultraLabel18.Size = new System.Drawing.Size(93, 15);
            this.ultraLabel18.TabIndex = 69;
            this.ultraLabel18.Text = "Minimum Price:";
            // 
            // numProcessMinPrice
            // 
            this.numProcessMinPrice.Location = new System.Drawing.Point(134, 120);
            this.numProcessMinPrice.MaskInput = "$ nnnn.nn";
            this.numProcessMinPrice.MaxValue = 9999.99999D;
            this.numProcessMinPrice.MinValue = 0;
            this.numProcessMinPrice.Name = "numProcessMinPrice";
            this.numProcessMinPrice.Nullable = true;
            this.numProcessMinPrice.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.numProcessMinPrice.Size = new System.Drawing.Size(142, 22);
            this.numProcessMinPrice.TabIndex = 9;
            this.numProcessMinPrice.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextControl;
            ultraToolTipInfo13.ToolTipText = "The minimum price for the process.";
            ultraToolTipInfo13.ToolTipTitle = "Minimum Price";
            this.tipManager.SetUltraToolTip(this.numProcessMinPrice, ultraToolTipInfo13);
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox1.Controls.Add(this.txtFrozenBy);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel17);
            this.ultraGroupBox1.Controls.Add(this.picFrozen);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel16);
            this.ultraGroupBox1.Controls.Add(this.txtFrozenDate);
            appearance1.Image = global::DWOS.UI.Properties.Resources.Lock_16;
            this.ultraGroupBox1.HeaderAppearance = appearance1;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(8, 311);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(758, 101);
            this.ultraGroupBox1.TabIndex = 17;
            this.ultraGroupBox1.Text = "Process Frozen";
            // 
            // txtFrozenBy
            // 
            this.txtFrozenBy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFrozenBy.Location = new System.Drawing.Point(124, 35);
            this.txtFrozenBy.Name = "txtFrozenBy";
            this.txtFrozenBy.ReadOnly = true;
            this.txtFrozenBy.Size = new System.Drawing.Size(453, 22);
            this.txtFrozenBy.TabIndex = 10;
            ultraToolTipInfo14.ToolTipTextFormatted = "The name of the person who froze the process.";
            ultraToolTipInfo14.ToolTipTitle = "Frozen By";
            this.tipManager.SetUltraToolTip(this.txtFrozenBy, ultraToolTipInfo14);
            // 
            // ultraLabel17
            // 
            this.ultraLabel17.AutoSize = true;
            this.ultraLabel17.Location = new System.Drawing.Point(16, 68);
            this.ultraLabel17.Name = "ultraLabel17";
            this.ultraLabel17.Size = new System.Drawing.Size(36, 15);
            this.ultraLabel17.TabIndex = 54;
            this.ultraLabel17.Text = "Date:";
            // 
            // picFrozen
            // 
            this.picFrozen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picFrozen.BorderShadowColor = System.Drawing.Color.Empty;
            this.picFrozen.Image = ((object)(resources.GetObject("picFrozen.Image")));
            this.picFrozen.Location = new System.Drawing.Point(702, 35);
            this.picFrozen.Name = "picFrozen";
            this.picFrozen.Size = new System.Drawing.Size(41, 50);
            this.picFrozen.TabIndex = 50;
            // 
            // ultraLabel16
            // 
            this.ultraLabel16.AutoSize = true;
            this.ultraLabel16.Location = new System.Drawing.Point(16, 40);
            this.ultraLabel16.Name = "ultraLabel16";
            this.ultraLabel16.Size = new System.Drawing.Size(66, 15);
            this.ultraLabel16.TabIndex = 53;
            this.ultraLabel16.Text = "Frozen By:";
            // 
            // txtFrozenDate
            // 
            this.txtFrozenDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFrozenDate.Location = new System.Drawing.Point(124, 63);
            this.txtFrozenDate.Name = "txtFrozenDate";
            this.txtFrozenDate.ReadOnly = true;
            this.txtFrozenDate.Size = new System.Drawing.Size(453, 22);
            this.txtFrozenDate.TabIndex = 11;
            ultraToolTipInfo15.ToolTipTextFormatted = "The date that the process was frozen on.";
            ultraToolTipInfo15.ToolTipTitle = "Date";
            this.tipManager.SetUltraToolTip(this.txtFrozenDate, ultraToolTipInfo15);
            // 
            // optPaperless
            // 
            this.optPaperless.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.optPaperless.BackColor = System.Drawing.Color.Transparent;
            this.optPaperless.BackColorInternal = System.Drawing.Color.Transparent;
            this.optPaperless.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance2.Image = global::DWOS.UI.Properties.Resources.Paperless_16;
            valueListItem1.Appearance = appearance2;
            valueListItem1.DataValue = true;
            valueListItem1.DisplayText = "Paperless";
            appearance3.Image = global::DWOS.UI.Properties.Resources.Paper_16;
            valueListItem2.Appearance = appearance3;
            valueListItem2.DataValue = false;
            valueListItem2.DisplayText = "Paper";
            this.optPaperless.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.optPaperless.Location = new System.Drawing.Point(606, 10);
            this.optPaperless.Name = "optPaperless";
            this.optPaperless.Size = new System.Drawing.Size(177, 19);
            this.optPaperless.TabIndex = 1;
            ultraToolTipInfo16.ToolTipTextFormatted = "Paperless mode will require operators to answer questions in DWOS.<br/>Paper mode" +
    " will require operqators to answer questions on paper.<br/>";
            ultraToolTipInfo16.ToolTipTitle = "Paper Mode";
            this.tipManager.SetUltraToolTip(this.optPaperless, ultraToolTipInfo16);
            // 
            // cboProcessCategory
            // 
            appearance4.Image = global::DWOS.UI.Properties.Resources.Add_16;
            editorButton1.Appearance = appearance4;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2010Button;
            this.cboProcessCategory.ButtonsLeft.Add(editorButton1);
            this.cboProcessCategory.DropDownListWidth = -1;
            this.cboProcessCategory.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboProcessCategory.Location = new System.Drawing.Point(356, 92);
            this.cboProcessCategory.MaxLength = 50;
            this.cboProcessCategory.Name = "cboProcessCategory";
            this.cboProcessCategory.Size = new System.Drawing.Size(158, 22);
            this.cboProcessCategory.TabIndex = 7;
            ultraToolTipInfo17.ToolTipText = "The process category used to group processes.";
            ultraToolTipInfo17.ToolTipTitle = "Category";
            this.tipManager.SetUltraToolTip(this.cboProcessCategory, ultraToolTipInfo17);
            this.cboProcessCategory.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.cboProcessCategory_EditorButtonClick);
            // 
            // ultraLabel11
            // 
            this.ultraLabel11.AutoSize = true;
            this.ultraLabel11.Location = new System.Drawing.Point(282, 99);
            this.ultraLabel11.Name = "ultraLabel11";
            this.ultraLabel11.Size = new System.Drawing.Size(61, 15);
            this.ultraLabel11.TabIndex = 48;
            this.ultraLabel11.Text = "Category:";
            // 
            // ultraLabel10
            // 
            this.ultraLabel10.AutoSize = true;
            this.ultraLabel10.Location = new System.Drawing.Point(13, 204);
            this.ultraLabel10.Name = "ultraLabel10";
            this.ultraLabel10.Size = new System.Drawing.Size(101, 15);
            this.ultraLabel10.TabIndex = 46;
            this.ultraLabel10.Text = "Document Links:";
            // 
            // docLinkManagerProcess
            // 
            this.docLinkManagerProcess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.docLinkManagerProcess.BackColor = System.Drawing.Color.Transparent;
            this.docLinkManagerProcess.CurrentRow = null;
            this.docLinkManagerProcess.DocumentLinkTable = null;
            this.docLinkManagerProcess.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.docLinkManagerProcess.LinkType = DWOS.UI.Documents.LinkType.Process;
            this.docLinkManagerProcess.Location = new System.Drawing.Point(132, 204);
            this.docLinkManagerProcess.Name = "docLinkManagerProcess";
            this.docLinkManagerProcess.ParentTable = null;
            this.docLinkManagerProcess.Size = new System.Drawing.Size(634, 101);
            this.docLinkManagerProcess.TabIndex = 16;
            this.docLinkManagerProcess.TableKeyColumn = null;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(13, 12);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(38, 15);
            this.ultraLabel3.TabIndex = 15;
            this.ultraLabel3.Text = "Code:";
            // 
            // ultraLabel14
            // 
            this.ultraLabel14.AutoSize = true;
            this.ultraLabel14.Location = new System.Drawing.Point(13, 96);
            this.ultraLabel14.Name = "ultraLabel14";
            this.ultraLabel14.Size = new System.Drawing.Size(77, 15);
            this.ultraLabel14.TabIndex = 43;
            this.ultraLabel14.Text = "Department:";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(282, 68);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(57, 15);
            this.ultraLabel1.TabIndex = 17;
            this.ultraLabel1.Text = "Revision:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(522, 68);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(64, 15);
            this.ultraLabel4.TabIndex = 21;
            this.ultraLabel4.Text = "Mod Date:";
            // 
            // txtProcessDesc
            // 
            this.txtProcessDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProcessDesc.Location = new System.Drawing.Point(134, 36);
            this.txtProcessDesc.Name = "txtProcessDesc";
            this.txtProcessDesc.Size = new System.Drawing.Size(632, 22);
            this.txtProcessDesc.TabIndex = 2;
            ultraToolTipInfo18.ToolTipText = "The descriptive name of the process.";
            ultraToolTipInfo18.ToolTipTitle = "Process Description";
            this.tipManager.SetUltraToolTip(this.txtProcessDesc, ultraToolTipInfo18);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(13, 40);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(73, 15);
            this.ultraLabel2.TabIndex = 18;
            this.ultraLabel2.Text = "Description:";
            // 
            // dtModDate
            // 
            this.dtModDate.DateTime = new System.DateTime(2012, 12, 15, 0, 0, 0, 0);
            this.dtModDate.Location = new System.Drawing.Point(613, 64);
            this.dtModDate.Name = "dtModDate";
            this.dtModDate.Size = new System.Drawing.Size(185, 22);
            this.dtModDate.TabIndex = 5;
            ultraToolTipInfo19.ToolTipText = "The date this process was last modified.";
            ultraToolTipInfo19.ToolTipTitle = "Mod Date";
            this.tipManager.SetUltraToolTip(this.dtModDate, ultraToolTipInfo19);
            this.dtModDate.Value = new System.DateTime(2012, 12, 15, 0, 0, 0, 0);
            // 
            // txtProcessName
            // 
            this.txtProcessName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProcessName.Location = new System.Drawing.Point(134, 8);
            this.txtProcessName.Name = "txtProcessName";
            this.txtProcessName.Size = new System.Drawing.Size(466, 22);
            this.txtProcessName.TabIndex = 0;
            ultraToolTipInfo20.ToolTipTextFormatted = "The in-house code of the process.<br/><br/>May be the same name as the process.<b" +
    "r/>";
            ultraToolTipInfo20.ToolTipTitle = "Process Code";
            this.tipManager.SetUltraToolTip(this.txtProcessName, ultraToolTipInfo20);
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.splitAliases);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Padding = new System.Windows.Forms.Padding(5);
            this.ultraTabPageControl2.Size = new System.Drawing.Size(806, 655);
            // 
            // splitAliases
            // 
            this.splitAliases.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitAliases.Location = new System.Drawing.Point(8, 8);
            this.splitAliases.Name = "splitAliases";
            // 
            // splitAliases.Panel1
            // 
            this.splitAliases.Panel1.Controls.Add(this.tvwProcessAliases);
            // 
            // splitAliases.Panel2
            // 
            this.splitAliases.Panel2.Controls.Add(this.pnlProcessAlias);
            this.splitAliases.Panel2.Controls.Add(this.pnlCustomerProcessAlias);
            this.splitAliases.Size = new System.Drawing.Size(790, 639);
            this.splitAliases.SplitterDistance = 261;
            this.splitAliases.SplitterWidth = 5;
            this.splitAliases.TabIndex = 2;
            // 
            // tvwProcessAliases
            // 
            this.tvwProcessAliases.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwProcessAliases.HideSelection = false;
            this.tvwProcessAliases.Location = new System.Drawing.Point(0, 0);
            this.tvwProcessAliases.Name = "tvwProcessAliases";
            this.tvwProcessAliases.Size = new System.Drawing.Size(261, 639);
            this.tvwProcessAliases.TabIndex = 0;
            this.tvwProcessAliases.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.tvwProcessAliases_AfterSelect);
            this.tvwProcessAliases.BeforeSelect += new Infragistics.Win.UltraWinTree.BeforeNodeSelectEventHandler(this.tvwProcessAliases_BeforeSelect);
            // 
            // pnlProcessAlias
            // 
            // 
            // pnlProcessAlias.ClientArea
            // 
            this.pnlProcessAlias.ClientArea.Controls.Add(this.ultraLabel15);
            this.pnlProcessAlias.ClientArea.Controls.Add(this.docLinkManagerProcessAlias);
            this.pnlProcessAlias.ClientArea.Controls.Add(this.txtProcessAliasName);
            this.pnlProcessAlias.ClientArea.Controls.Add(this.ultraLabel6);
            this.pnlProcessAlias.ClientArea.Controls.Add(this.txtTravelerNote);
            this.pnlProcessAlias.ClientArea.Controls.Add(this.txtPopupNote);
            this.pnlProcessAlias.ClientArea.Controls.Add(this.ultraLabel7);
            this.pnlProcessAlias.ClientArea.Controls.Add(this.ultraLabel5);
            this.pnlProcessAlias.Location = new System.Drawing.Point(3, 3);
            this.pnlProcessAlias.Name = "pnlProcessAlias";
            this.pnlProcessAlias.Size = new System.Drawing.Size(376, 228);
            this.pnlProcessAlias.TabIndex = 26;
            // 
            // ultraLabel15
            // 
            this.ultraLabel15.AutoSize = true;
            this.ultraLabel15.Location = new System.Drawing.Point(12, 156);
            this.ultraLabel15.Name = "ultraLabel15";
            this.ultraLabel15.Size = new System.Drawing.Size(101, 15);
            this.ultraLabel15.TabIndex = 44;
            this.ultraLabel15.Text = "Document Links:";
            // 
            // docLinkManagerProcessAlias
            // 
            this.docLinkManagerProcessAlias.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.docLinkManagerProcessAlias.BackColor = System.Drawing.Color.Transparent;
            this.docLinkManagerProcessAlias.CurrentRow = null;
            this.docLinkManagerProcessAlias.DocumentLinkTable = null;
            this.docLinkManagerProcessAlias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.docLinkManagerProcessAlias.LinkType = DWOS.UI.Documents.LinkType.Process;
            this.docLinkManagerProcessAlias.Location = new System.Drawing.Point(120, 151);
            this.docLinkManagerProcessAlias.Name = "docLinkManagerProcessAlias";
            this.docLinkManagerProcessAlias.ParentTable = null;
            this.docLinkManagerProcessAlias.Size = new System.Drawing.Size(235, 74);
            this.docLinkManagerProcessAlias.TabIndex = 25;
            this.docLinkManagerProcessAlias.TableKeyColumn = null;
            // 
            // txtProcessAliasName
            // 
            this.txtProcessAliasName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProcessAliasName.Location = new System.Drawing.Point(120, 13);
            this.txtProcessAliasName.Name = "txtProcessAliasName";
            this.txtProcessAliasName.Nullable = false;
            this.txtProcessAliasName.Size = new System.Drawing.Size(235, 22);
            this.txtProcessAliasName.TabIndex = 19;
            ultraToolTipInfo21.ToolTipTextFormatted = "The alias name for the process.";
            ultraToolTipInfo21.ToolTipTitle = "Process Alias";
            this.tipManager.SetUltraToolTip(this.txtProcessAliasName, ultraToolTipInfo21);
            this.txtProcessAliasName.Leave += new System.EventHandler(this.txtProcessAliasName_Leave);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(12, 44);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(75, 15);
            this.ultraLabel6.TabIndex = 22;
            this.ultraLabel6.Text = "Popup Note:";
            // 
            // txtTravelerNote
            // 
            this.txtTravelerNote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTravelerNote.Location = new System.Drawing.Point(120, 97);
            this.txtTravelerNote.Multiline = true;
            this.txtTravelerNote.Name = "txtTravelerNote";
            this.txtTravelerNote.Size = new System.Drawing.Size(235, 50);
            this.txtTravelerNote.TabIndex = 23;
            ultraToolTipInfo22.ToolTipTextFormatted = "The note that will be added to the work order traveler.";
            ultraToolTipInfo22.ToolTipTitle = "Traveler Note";
            this.tipManager.SetUltraToolTip(this.txtTravelerNote, ultraToolTipInfo22);
            // 
            // txtPopupNote
            // 
            this.txtPopupNote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPopupNote.Location = new System.Drawing.Point(120, 41);
            this.txtPopupNote.Multiline = true;
            this.txtPopupNote.Name = "txtPopupNote";
            this.txtPopupNote.Size = new System.Drawing.Size(235, 50);
            this.txtPopupNote.TabIndex = 20;
            ultraToolTipInfo23.ToolTipTextFormatted = "The popup note that will be displayed when the process is added to a part.";
            ultraToolTipInfo23.ToolTipTitle = "Pop Up Note";
            this.tipManager.SetUltraToolTip(this.txtPopupNote, ultraToolTipInfo23);
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(12, 100);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(87, 15);
            this.ultraLabel7.TabIndex = 24;
            this.ultraLabel7.Text = "Traveler Note:";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(12, 17);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(36, 15);
            this.ultraLabel5.TabIndex = 21;
            this.ultraLabel5.Text = "Alias:";
            // 
            // pnlCustomerProcessAlias
            // 
            // 
            // pnlCustomerProcessAlias.ClientArea
            // 
            this.pnlCustomerProcessAlias.ClientArea.Controls.Add(this.ultraLabel9);
            this.pnlCustomerProcessAlias.ClientArea.Controls.Add(this.ultraLabel8);
            this.pnlCustomerProcessAlias.ClientArea.Controls.Add(this.txtCustomerProcessAliasName);
            this.pnlCustomerProcessAlias.ClientArea.Controls.Add(this.cboCustomer);
            this.pnlCustomerProcessAlias.Location = new System.Drawing.Point(3, 258);
            this.pnlCustomerProcessAlias.Name = "pnlCustomerProcessAlias";
            this.pnlCustomerProcessAlias.Size = new System.Drawing.Size(373, 100);
            this.pnlCustomerProcessAlias.TabIndex = 25;
            // 
            // ultraLabel9
            // 
            this.ultraLabel9.AutoSize = true;
            this.ultraLabel9.Location = new System.Drawing.Point(12, 45);
            this.ultraLabel9.Name = "ultraLabel9";
            this.ultraLabel9.Size = new System.Drawing.Size(36, 15);
            this.ultraLabel9.TabIndex = 45;
            this.ultraLabel9.Text = "Alias:";
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(12, 20);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(64, 15);
            this.ultraLabel8.TabIndex = 26;
            this.ultraLabel8.Text = "Customer:";
            // 
            // txtCustomerProcessAliasName
            // 
            this.txtCustomerProcessAliasName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomerProcessAliasName.Location = new System.Drawing.Point(105, 41);
            this.txtCustomerProcessAliasName.Name = "txtCustomerProcessAliasName";
            this.txtCustomerProcessAliasName.Nullable = false;
            this.txtCustomerProcessAliasName.Size = new System.Drawing.Size(250, 22);
            this.txtCustomerProcessAliasName.TabIndex = 44;
            ultraToolTipInfo24.ToolTipTextFormatted = "The process name that will be displayed for the specified customer.";
            ultraToolTipInfo24.ToolTipTitle = "Customer Alias";
            this.tipManager.SetUltraToolTip(this.txtCustomerProcessAliasName, ultraToolTipInfo24);
            this.txtCustomerProcessAliasName.Leave += new System.EventHandler(this.txtCustomerProcessAliasName_Leave);
            // 
            // cboCustomer
            // 
            this.cboCustomer.DropDownListWidth = -1;
            this.cboCustomer.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCustomer.Location = new System.Drawing.Point(105, 13);
            this.cboCustomer.MaxLength = 50;
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.Size = new System.Drawing.Size(186, 22);
            this.cboCustomer.TabIndex = 43;
            ultraToolTipInfo25.ToolTipTextFormatted = "The customer this alias will be used for.";
            ultraToolTipInfo25.ToolTipTitle = "Customer";
            this.tipManager.SetUltraToolTip(this.cboCustomer, ultraToolTipInfo25);
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Controls.Add(this.btnInspectionCheck);
            this.ultraTabPageControl3.Controls.Add(this.btnRemoveInspection);
            this.ultraTabPageControl3.Controls.Add(this.btnInspectionDown);
            this.ultraTabPageControl3.Controls.Add(this.btnInspectionUp);
            this.ultraTabPageControl3.Controls.Add(this.btnAddInspection);
            this.ultraTabPageControl3.Controls.Add(this.lvwInspections);
            this.ultraTabPageControl3.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(1222, 1015);
            // 
            // btnInspectionCheck
            // 
            appearance5.Image = global::DWOS.UI.Properties.Resources.Checked_32;
            this.btnInspectionCheck.Appearance = appearance5;
            this.btnInspectionCheck.AutoSize = true;
            this.btnInspectionCheck.Location = new System.Drawing.Point(9, 142);
            this.btnInspectionCheck.Name = "btnInspectionCheck";
            this.btnInspectionCheck.Size = new System.Drawing.Size(26, 26);
            this.btnInspectionCheck.TabIndex = 57;
            ultraToolTipInfo26.ToolTipText = "Toggle the display of the inspection data on the COC.";
            ultraToolTipInfo26.ToolTipTitle = "Display on COC";
            this.tipManager.SetUltraToolTip(this.btnInspectionCheck, ultraToolTipInfo26);
            // 
            // btnRemoveInspection
            // 
            appearance6.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            this.btnRemoveInspection.Appearance = appearance6;
            this.btnRemoveInspection.AutoSize = true;
            this.btnRemoveInspection.Location = new System.Drawing.Point(9, 46);
            this.btnRemoveInspection.Name = "btnRemoveInspection";
            this.btnRemoveInspection.Size = new System.Drawing.Size(26, 26);
            this.btnRemoveInspection.TabIndex = 56;
            ultraToolTipInfo27.ToolTipText = "Remove the selected inspection.";
            ultraToolTipInfo27.ToolTipTitle = "Remove Inspection";
            this.tipManager.SetUltraToolTip(this.btnRemoveInspection, ultraToolTipInfo27);
            // 
            // btnInspectionDown
            // 
            appearance7.Image = global::DWOS.UI.Properties.Resources.Arrow_Down;
            this.btnInspectionDown.Appearance = appearance7;
            this.btnInspectionDown.AutoSize = true;
            this.btnInspectionDown.Location = new System.Drawing.Point(9, 110);
            this.btnInspectionDown.Name = "btnInspectionDown";
            this.btnInspectionDown.Size = new System.Drawing.Size(26, 26);
            this.btnInspectionDown.TabIndex = 55;
            ultraToolTipInfo28.ToolTipText = "Shift selected inspection down.";
            ultraToolTipInfo28.ToolTipTitle = "Shift Down";
            this.tipManager.SetUltraToolTip(this.btnInspectionDown, ultraToolTipInfo28);
            // 
            // btnInspectionUp
            // 
            appearance8.Image = global::DWOS.UI.Properties.Resources.Arrow_Up;
            this.btnInspectionUp.Appearance = appearance8;
            this.btnInspectionUp.AutoSize = true;
            this.btnInspectionUp.Location = new System.Drawing.Point(9, 78);
            this.btnInspectionUp.Name = "btnInspectionUp";
            this.btnInspectionUp.Size = new System.Drawing.Size(26, 26);
            this.btnInspectionUp.TabIndex = 54;
            ultraToolTipInfo29.ToolTipText = "Shift selected inspection up.";
            ultraToolTipInfo29.ToolTipTitle = "Shift Up";
            this.tipManager.SetUltraToolTip(this.btnInspectionUp, ultraToolTipInfo29);
            // 
            // btnAddInspection
            // 
            appearance9.Image = global::DWOS.UI.Properties.Resources.Add_16;
            this.btnAddInspection.Appearance = appearance9;
            this.btnAddInspection.AutoSize = true;
            this.btnAddInspection.Location = new System.Drawing.Point(9, 14);
            this.btnAddInspection.Name = "btnAddInspection";
            this.btnAddInspection.Size = new System.Drawing.Size(26, 26);
            this.btnAddInspection.TabIndex = 53;
            ultraToolTipInfo30.ToolTipText = "Add an inspection to this process.";
            ultraToolTipInfo30.ToolTipTitle = "Add Inspection";
            this.tipManager.SetUltraToolTip(this.btnAddInspection, ultraToolTipInfo30);
            // 
            // lvwInspections
            // 
            this.lvwInspections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwInspections.Items.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewItem[] {
            ultraListViewItem1});
            this.lvwInspections.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            this.lvwInspections.ItemSettings.HideSelection = false;
            this.lvwInspections.ItemSettings.HotTracking = true;
            this.lvwInspections.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.Single;
            this.lvwInspections.Location = new System.Drawing.Point(41, 14);
            this.lvwInspections.MainColumn.AutoSizeMode = ((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header | Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.VisibleItems)));
            appearance10.Image = global::DWOS.UI.Properties.Resources.Inspection_16;
            this.lvwInspections.MainColumn.ItemAppearance = appearance10;
            this.lvwInspections.MainColumn.Key = "Order";
            this.lvwInspections.MainColumn.Sorting = Infragistics.Win.UltraWinListView.Sorting.Ascending;
            this.lvwInspections.MainColumn.VisiblePositionInDetailsView = 0;
            this.lvwInspections.MainColumn.Width = 25;
            this.lvwInspections.Name = "lvwInspections";
            this.lvwInspections.Size = new System.Drawing.Size(1164, 172);
            ultraListViewSubItemColumn1.Key = "Inspection";
            ultraListViewSubItemColumn1.VisiblePositionInDetailsView = 1;
            ultraListViewSubItemColumn2.DataType = typeof(bool);
            ultraListViewSubItemColumn2.Key = "COC";
            appearance11.TextHAlignAsString = "Center";
            ultraListViewSubItemColumn2.SubItemAppearance = appearance11;
            ultraListViewSubItemColumn2.Text = "Display on COC";
            ultraListViewSubItemColumn2.VisiblePositionInDetailsView = 3;
            ultraListViewSubItemColumn2.Width = 40;
            ultraListViewSubItemColumn3.Key = "Revision";
            ultraListViewSubItemColumn3.VisiblePositionInDetailsView = 2;
            this.lvwInspections.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1,
            ultraListViewSubItemColumn2,
            ultraListViewSubItemColumn3});
            this.lvwInspections.TabIndex = 52;
            ultraToolTipInfo31.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo31.ToolTipTextFormatted");
            ultraToolTipInfo31.ToolTipTitle = "Inspections";
            this.tipManager.SetUltraToolTip(this.lvwInspections, ultraToolTipInfo31);
            this.lvwInspections.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvwInspections.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns;
            this.lvwInspections.ViewSettingsDetails.ColumnAutoSizeMode = ((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header | Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems)));
            this.lvwInspections.ViewSettingsDetails.ColumnHeaderStyle = Infragistics.Win.HeaderStyle.WindowsVista;
            this.lvwInspections.ViewSettingsDetails.FullRowSelect = true;
            // 
            // ultraTabPageControl4
            // 
            this.ultraTabPageControl4.Controls.Add(this.ultraLabel13);
            this.ultraTabPageControl4.Controls.Add(this.ultraLabel12);
            this.ultraTabPageControl4.Controls.Add(this.grdConstraints);
            this.ultraTabPageControl4.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl4.Name = "ultraTabPageControl4";
            this.ultraTabPageControl4.Size = new System.Drawing.Size(806, 655);
            // 
            // ultraLabel13
            // 
            this.ultraLabel13.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel13.Location = new System.Drawing.Point(16, 31);
            this.ultraLabel13.Name = "ultraLabel13";
            this.ultraLabel13.Size = new System.Drawing.Size(765, 40);
            this.ultraLabel13.TabIndex = 2;
            this.ultraLabel13.Text = "If any of the defined processes occur before this process then this process shoul" +
    "d be completed within the defined time constraint.";
            // 
            // ultraLabel12
            // 
            this.ultraLabel12.AutoSize = true;
            this.ultraLabel12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel12.Location = new System.Drawing.Point(16, 13);
            this.ultraLabel12.Name = "ultraLabel12";
            this.ultraLabel12.Size = new System.Drawing.Size(89, 15);
            this.ultraLabel12.TabIndex = 1;
            this.ultraLabel12.Text = "Prerequisites";
            // 
            // grdConstraints
            // 
            this.grdConstraints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance12.BackColor = System.Drawing.SystemColors.Window;
            appearance12.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdConstraints.DisplayLayout.Appearance = appearance12;
            appearance13.TextHAlignAsString = "Center";
            ultraGridColumn5.Header.Appearance = appearance13;
            ultraGridColumn5.Header.Caption = "Process";
            ultraGridColumn5.Header.VisiblePosition = 0;
            ultraGridColumn5.Width = 250;
            appearance14.TextHAlignAsString = "Center";
            ultraGridColumn6.Header.Appearance = appearance14;
            ultraGridColumn6.Header.Caption = "Time";
            ultraGridColumn6.Header.VisiblePosition = 1;
            ultraGridColumn6.MaskInput = "nn.nn Hrs";
            ultraGridColumn6.MaxValue = new decimal(new int[] {
            99999,
            0,
            0,
            131072});
            ultraGridColumn6.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            ultraGridColumn6.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn7.Header.VisiblePosition = 2;
            ultraGridColumn7.Hidden = true;
            ultraGridColumn8.Header.VisiblePosition = 3;
            ultraGridColumn8.Hidden = true;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8});
            this.grdConstraints.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdConstraints.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdConstraints.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance15.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance15.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance15.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance15.BorderColor = System.Drawing.SystemColors.Window;
            this.grdConstraints.DisplayLayout.GroupByBox.Appearance = appearance15;
            appearance16.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdConstraints.DisplayLayout.GroupByBox.BandLabelAppearance = appearance16;
            this.grdConstraints.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance17.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance17.BackColor2 = System.Drawing.SystemColors.Control;
            appearance17.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance17.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdConstraints.DisplayLayout.GroupByBox.PromptAppearance = appearance17;
            this.grdConstraints.DisplayLayout.MaxColScrollRegions = 1;
            this.grdConstraints.DisplayLayout.MaxRowScrollRegions = 1;
            appearance18.BackColor = System.Drawing.SystemColors.Window;
            appearance18.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdConstraints.DisplayLayout.Override.ActiveCellAppearance = appearance18;
            appearance19.BackColor = System.Drawing.SystemColors.Highlight;
            appearance19.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdConstraints.DisplayLayout.Override.ActiveRowAppearance = appearance19;
            this.grdConstraints.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.grdConstraints.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdConstraints.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance20.BackColor = System.Drawing.SystemColors.Window;
            this.grdConstraints.DisplayLayout.Override.CardAreaAppearance = appearance20;
            appearance21.BorderColor = System.Drawing.Color.Silver;
            appearance21.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdConstraints.DisplayLayout.Override.CellAppearance = appearance21;
            this.grdConstraints.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdConstraints.DisplayLayout.Override.CellPadding = 0;
            appearance22.BackColor = System.Drawing.SystemColors.Control;
            appearance22.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance22.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance22.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance22.BorderColor = System.Drawing.SystemColors.Window;
            this.grdConstraints.DisplayLayout.Override.GroupByRowAppearance = appearance22;
            appearance23.TextHAlignAsString = "Left";
            this.grdConstraints.DisplayLayout.Override.HeaderAppearance = appearance23;
            this.grdConstraints.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdConstraints.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance24.BackColor = System.Drawing.SystemColors.Window;
            appearance24.BorderColor = System.Drawing.Color.Silver;
            this.grdConstraints.DisplayLayout.Override.RowAppearance = appearance24;
            this.grdConstraints.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance25.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdConstraints.DisplayLayout.Override.TemplateAddRowAppearance = appearance25;
            this.grdConstraints.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdConstraints.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdConstraints.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdConstraints.Location = new System.Drawing.Point(16, 77);
            this.grdConstraints.Name = "grdConstraints";
            this.grdConstraints.Size = new System.Drawing.Size(765, 564);
            this.grdConstraints.TabIndex = 0;
            this.grdConstraints.Error += new Infragistics.Win.UltraWinGrid.ErrorEventHandler(this.grdConstraints_Error);
            // 
            // ultraTabPageControl5
            // 
            this.ultraTabPageControl5.Controls.Add(this.btnImportSuggestions);
            this.ultraTabPageControl5.Controls.Add(this.btnEditSuggestion);
            this.ultraTabPageControl5.Controls.Add(this.btnDeleteSuggestion);
            this.ultraTabPageControl5.Controls.Add(this.grdSuggestions);
            this.ultraTabPageControl5.Controls.Add(this.btnAddSuggestion);
            this.ultraTabPageControl5.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl5.Name = "ultraTabPageControl5";
            this.ultraTabPageControl5.Size = new System.Drawing.Size(806, 655);
            // 
            // btnImportSuggestions
            // 
            appearance26.Image = global::DWOS.UI.Properties.Resources.Part_16;
            this.btnImportSuggestions.Appearance = appearance26;
            this.btnImportSuggestions.AutoSize = true;
            this.btnImportSuggestions.Location = new System.Drawing.Point(9, 139);
            this.btnImportSuggestions.Name = "btnImportSuggestions";
            this.btnImportSuggestions.Size = new System.Drawing.Size(26, 26);
            this.btnImportSuggestions.TabIndex = 4;
            ultraToolTipInfo32.ToolTipText = "Import suggestions from a part with this process.";
            ultraToolTipInfo32.ToolTipTitle = "Import";
            this.tipManager.SetUltraToolTip(this.btnImportSuggestions, ultraToolTipInfo32);
            // 
            // btnEditSuggestion
            // 
            appearance27.Image = global::DWOS.UI.Properties.Resources.Edit_16;
            this.btnEditSuggestion.Appearance = appearance27;
            this.btnEditSuggestion.AutoSize = true;
            this.btnEditSuggestion.Location = new System.Drawing.Point(9, 78);
            this.btnEditSuggestion.Name = "btnEditSuggestion";
            this.btnEditSuggestion.Size = new System.Drawing.Size(26, 26);
            this.btnEditSuggestion.TabIndex = 3;
            ultraToolTipInfo33.ToolTipText = "Edits the selected suggestion.";
            ultraToolTipInfo33.ToolTipTitle = "Edit Suggestion";
            this.tipManager.SetUltraToolTip(this.btnEditSuggestion, ultraToolTipInfo33);
            // 
            // btnDeleteSuggestion
            // 
            appearance28.Image = global::DWOS.UI.Properties.Resources.Delete_32;
            this.btnDeleteSuggestion.Appearance = appearance28;
            this.btnDeleteSuggestion.AutoSize = true;
            this.btnDeleteSuggestion.Location = new System.Drawing.Point(9, 46);
            this.btnDeleteSuggestion.Name = "btnDeleteSuggestion";
            this.btnDeleteSuggestion.Size = new System.Drawing.Size(26, 26);
            this.btnDeleteSuggestion.TabIndex = 2;
            ultraToolTipInfo34.ToolTipText = "Delete the selected suggestion.";
            ultraToolTipInfo34.ToolTipTitle = "Delete Suggestion";
            this.tipManager.SetUltraToolTip(this.btnDeleteSuggestion, ultraToolTipInfo34);
            // 
            // grdSuggestions
            // 
            this.grdSuggestions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance29.BackColor = System.Drawing.SystemColors.Window;
            appearance29.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdSuggestions.DisplayLayout.Appearance = appearance29;
            this.grdSuggestions.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn9.Header.VisiblePosition = 0;
            ultraGridColumn9.Width = 116;
            ultraGridColumn10.Header.Caption = "Department";
            ultraGridColumn10.Header.VisiblePosition = 2;
            ultraGridColumn10.Width = 128;
            ultraGridColumn11.Header.Caption = "Process";
            ultraGridColumn11.Header.VisiblePosition = 3;
            ultraGridColumn11.Width = 127;
            ultraGridColumn12.Header.Caption = "Alias";
            ultraGridColumn12.Header.VisiblePosition = 4;
            ultraGridColumn12.Width = 127;
            ultraGridColumn13.Header.VisiblePosition = 5;
            ultraGridColumn13.Width = 127;
            ultraGridColumn1.Header.VisiblePosition = 6;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn1.Width = 46;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 99;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn1,
            ultraGridColumn2});
            this.grdSuggestions.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdSuggestions.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdSuggestions.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance30.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance30.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance30.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance30.BorderColor = System.Drawing.SystemColors.Window;
            this.grdSuggestions.DisplayLayout.GroupByBox.Appearance = appearance30;
            appearance31.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdSuggestions.DisplayLayout.GroupByBox.BandLabelAppearance = appearance31;
            this.grdSuggestions.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance32.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance32.BackColor2 = System.Drawing.SystemColors.Control;
            appearance32.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance32.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdSuggestions.DisplayLayout.GroupByBox.PromptAppearance = appearance32;
            this.grdSuggestions.DisplayLayout.MaxColScrollRegions = 1;
            this.grdSuggestions.DisplayLayout.MaxRowScrollRegions = 1;
            appearance33.BackColor = System.Drawing.SystemColors.Window;
            appearance33.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdSuggestions.DisplayLayout.Override.ActiveCellAppearance = appearance33;
            appearance34.BackColor = System.Drawing.SystemColors.Highlight;
            appearance34.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdSuggestions.DisplayLayout.Override.ActiveRowAppearance = appearance34;
            this.grdSuggestions.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdSuggestions.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdSuggestions.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.grdSuggestions.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.grdSuggestions.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdSuggestions.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance35.BackColor = System.Drawing.SystemColors.Window;
            this.grdSuggestions.DisplayLayout.Override.CardAreaAppearance = appearance35;
            appearance36.BorderColor = System.Drawing.Color.Silver;
            appearance36.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdSuggestions.DisplayLayout.Override.CellAppearance = appearance36;
            this.grdSuggestions.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdSuggestions.DisplayLayout.Override.CellPadding = 0;
            appearance37.BackColor = System.Drawing.SystemColors.Control;
            appearance37.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance37.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance37.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance37.BorderColor = System.Drawing.SystemColors.Window;
            this.grdSuggestions.DisplayLayout.Override.GroupByRowAppearance = appearance37;
            appearance38.TextHAlignAsString = "Left";
            this.grdSuggestions.DisplayLayout.Override.HeaderAppearance = appearance38;
            this.grdSuggestions.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdSuggestions.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance39.BackColor = System.Drawing.SystemColors.Window;
            appearance39.BorderColor = System.Drawing.Color.Silver;
            this.grdSuggestions.DisplayLayout.Override.RowAppearance = appearance39;
            this.grdSuggestions.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdSuggestions.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance40.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdSuggestions.DisplayLayout.Override.TemplateAddRowAppearance = appearance40;
            this.grdSuggestions.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdSuggestions.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdSuggestions.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdSuggestions.Location = new System.Drawing.Point(41, 14);
            this.grdSuggestions.Name = "grdSuggestions";
            this.grdSuggestions.Size = new System.Drawing.Size(762, 638);
            this.grdSuggestions.TabIndex = 5;
            this.grdSuggestions.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdSuggestions_InitializeRow);
            // 
            // btnAddSuggestion
            // 
            appearance41.Image = global::DWOS.UI.Properties.Resources.Add_16;
            this.btnAddSuggestion.Appearance = appearance41;
            this.btnAddSuggestion.AutoSize = true;
            this.btnAddSuggestion.Location = new System.Drawing.Point(9, 14);
            this.btnAddSuggestion.Name = "btnAddSuggestion";
            this.btnAddSuggestion.Size = new System.Drawing.Size(26, 26);
            this.btnAddSuggestion.TabIndex = 1;
            ultraToolTipInfo35.ToolTipText = "Add a suggested process to this process.";
            ultraToolTipInfo35.ToolTipTitle = "Add Suggestion";
            this.tipManager.SetUltraToolTip(this.btnAddSuggestion, ultraToolTipInfo35);
            // 
            // tabProcessInfo
            // 
            this.tabProcessInfo.Controls.Add(this.ultraTabSharedControlsPage1);
            this.tabProcessInfo.Controls.Add(this.ultraTabPageControl1);
            this.tabProcessInfo.Controls.Add(this.ultraTabPageControl2);
            this.tabProcessInfo.Controls.Add(this.ultraTabPageControl3);
            this.tabProcessInfo.Controls.Add(this.ultraTabPageControl4);
            this.tabProcessInfo.Controls.Add(this.ultraTabPageControl5);
            this.tabProcessInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabProcessInfo.Location = new System.Drawing.Point(8, 25);
            this.tabProcessInfo.Name = "tabProcessInfo";
            this.tabProcessInfo.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.tabProcessInfo.Size = new System.Drawing.Size(1226, 1041);
            this.tabProcessInfo.TabIndex = 0;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "General";
            ultraTab2.Key = "Aliases";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "Aliases";
            ultraTab3.Key = "Inspections";
            ultraTab3.TabPage = this.ultraTabPageControl3;
            ultraTab3.Text = "Inspections";
            ultraTab4.Key = "Time";
            ultraTab4.TabPage = this.ultraTabPageControl4;
            ultraTab4.Text = "Constraints";
            ultraTab5.Key = "Suggestions";
            ultraTab5.TabPage = this.ultraTabPageControl5;
            ultraTab5.Text = "Suggestions";
            this.tabProcessInfo.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2,
            ultraTab3,
            ultraTab4,
            ultraTab5});
            this.tabProcessInfo.SelectedTabChanged += new Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventHandler(this.tabProcessInfo_SelectedTabChanged);
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(1222, 1015);
            // 
            // imagelistLocks
            // 
            this.imagelistLocks.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imagelistLocks.ImageStream")));
            this.imagelistLocks.TransparentColor = System.Drawing.Color.Transparent;
            this.imagelistLocks.Images.SetKeyName(0, "Unlock");
            this.imagelistLocks.Images.SetKeyName(1, "Lock");
            // 
            // ProcessInfo
            // 
            this.Name = "ProcessInfo";
            this.Size = new System.Drawing.Size(1248, 1080);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessRev)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessStepDept)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLeadTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.curBurdenRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShortCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.curMaterialCost)).EndInit();
            this.pnlLoadCapacity.ResumeLayout(false);
            this.pnlLoadCapacity.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLoadCapacity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLoadCapacityVariance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLoadCapacityType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProcessPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProcessMinPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtFrozenBy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFrozenDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.optPaperless)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessCategory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProcessDesc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtModDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProcessName)).EndInit();
            this.ultraTabPageControl2.ResumeLayout(false);
            this.splitAliases.Panel1.ResumeLayout(false);
            this.splitAliases.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitAliases)).EndInit();
            this.splitAliases.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tvwProcessAliases)).EndInit();
            this.pnlProcessAlias.ClientArea.ResumeLayout(false);
            this.pnlProcessAlias.ClientArea.PerformLayout();
            this.pnlProcessAlias.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtProcessAliasName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTravelerNote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPopupNote)).EndInit();
            this.pnlCustomerProcessAlias.ClientArea.ResumeLayout(false);
            this.pnlCustomerProcessAlias.ClientArea.PerformLayout();
            this.pnlCustomerProcessAlias.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerProcessAliasName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).EndInit();
            this.ultraTabPageControl3.ResumeLayout(false);
            this.ultraTabPageControl3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvwInspections)).EndInit();
            this.ultraTabPageControl4.ResumeLayout(false);
            this.ultraTabPageControl4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdConstraints)).EndInit();
            this.ultraTabPageControl5.ResumeLayout(false);
            this.ultraTabPageControl5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdSuggestions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabProcessInfo)).EndInit();
            this.tabProcessInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bsConstraints)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboProcessRev;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtProcessName;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtProcessDesc;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtModDate;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboProcessStepDept;
		private Infragistics.Win.Misc.UltraLabel ultraLabel14;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl tabProcessInfo;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private System.Windows.Forms.SplitContainer splitAliases;
        private Infragistics.Win.UltraWinTree.UltraTree tvwProcessAliases;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTravelerNote;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPopupNote;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtProcessAliasName;
        private Infragistics.Win.Misc.UltraPanel pnlCustomerProcessAlias;
        private Infragistics.Win.Misc.UltraLabel ultraLabel9;
        private Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomerProcessAliasName;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomer;
        private Infragistics.Win.Misc.UltraPanel pnlProcessAlias;
        private Documents.DocumentLinkManager docLinkManagerProcessAlias;
        private Infragistics.Win.Misc.UltraLabel ultraLabel15;
        private Infragistics.Win.Misc.UltraLabel ultraLabel10;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
        private Infragistics.Win.Misc.UltraButton btnRemoveInspection;
        private Infragistics.Win.Misc.UltraButton btnInspectionDown;
        private Infragistics.Win.Misc.UltraButton btnInspectionUp;
        private Infragistics.Win.Misc.UltraButton btnAddInspection;
        private Infragistics.Win.UltraWinListView.UltraListView lvwInspections;
        private Infragistics.Win.Misc.UltraButton btnInspectionCheck;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboProcessCategory;
        private Infragistics.Win.Misc.UltraLabel ultraLabel11;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel13;
        private Infragistics.Win.Misc.UltraLabel ultraLabel12;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdConstraints;
        private System.Windows.Forms.BindingSource bsConstraints;
        private Infragistics.Win.UltraWinEditors.UltraOptionSet optPaperless;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtFrozenBy;
        private Infragistics.Win.Misc.UltraLabel ultraLabel17;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picFrozen;
        private Infragistics.Win.Misc.UltraLabel ultraLabel16;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtFrozenDate;
        private System.Windows.Forms.ImageList imagelistLocks;
        public Infragistics.Win.Misc.UltraLabel ultraLabel18;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numProcessMinPrice;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboStatus;
        private Infragistics.Win.Misc.UltraLabel ultraLabel19;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numProcessPrice;
        private Infragistics.Win.Misc.UltraLabel ultraLabel20;
        private Documents.DocumentLinkManager docLinkManagerProcess;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numLoadCapacityVariance;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboLoadCapacityType;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numLoadCapacity;
        private Infragistics.Win.Misc.UltraLabel ultraLabel22;
        private Infragistics.Win.Misc.UltraLabel ultraLabel21;
        private Infragistics.Win.Misc.UltraLabel ultraLabel23;
        private System.Windows.Forms.Panel pnlLoadCapacity;
        private Infragistics.Win.Misc.UltraLabel ultraLabel24;
        private Infragistics.Win.UltraWinEditors.UltraCurrencyEditor curMaterialCost;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtShortCode;
        private Infragistics.Win.Misc.UltraLabel ultraLabel25;
        private Infragistics.Win.UltraWinEditors.UltraCurrencyEditor curBurdenRate;
        private Infragistics.Win.Misc.UltraLabel ultraLabel26;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl5;
        private Infragistics.Win.Misc.UltraButton btnAddSuggestion;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdSuggestions;
        private Infragistics.Win.Misc.UltraButton btnDeleteSuggestion;
        private Infragistics.Win.Misc.UltraButton btnEditSuggestion;
        private Infragistics.Win.Misc.UltraButton btnImportSuggestions;
        private Infragistics.Win.Misc.UltraButton btnProcessProductClass;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numLeadTime;
        private Infragistics.Win.Misc.UltraLabel lblLeadTime;
    }
}
