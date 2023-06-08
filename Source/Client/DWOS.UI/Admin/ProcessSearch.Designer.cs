namespace DWOS.UI.Admin
{
    partial class ProcessSearch
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The field to search for.", Infragistics.Win.ToolTipImage.Default, "Field", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The value to search for.", Infragistics.Win.ToolTipImage.Default, "Value", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Active Only", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("ProcessSearch", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Revision");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Department");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Category");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsPaperless");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ModifiedDate", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_ProcessSteps_Process1");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_ProcessAlias_Process1");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_ProcessInspections_Process1");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_ProcessRequisite_Process2");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_ProcessRequisite_Process11");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_ProcessAlias_Process3");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_ProcessSteps_Process1", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessStepID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StepOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("COCData");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_ProcessQuestion_ProcessSteps");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_ProcessStepCondition_ProcessSteps");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_ProcessAlias_ProcessStepsDocumentLink");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand3 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_ProcessQuestion_ProcessSteps", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessQuestionID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessStepID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MinValue");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn101 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MaxValue");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn102 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ListID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn103 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DefaultValue");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn104 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("InputType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn105 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("NumericUntis");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn106 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OperatorEditable");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn107 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Required");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn108 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StepOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn109 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Notes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn110 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_ProcessQuestion_ProcessStepCondition");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand4 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_ProcessQuestion_ProcessStepCondition", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn111 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessStepConditionId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn112 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessStepId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn113 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessQuestionId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn114 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("InputType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn115 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Operator");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn116 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn117 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StepOrder");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand5 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_ProcessStepCondition_ProcessSteps", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn118 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessStepConditionId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn119 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessStepId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn120 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessQuestionId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn121 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("InputType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn122 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Operator");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn123 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn124 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StepOrder");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand6 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_ProcessAlias_ProcessStepsDocumentLink", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DocumentLinkID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DocumentInfoID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn32 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LinkToType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LinkToKey");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand7 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_ProcessAlias_Process1", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn125 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessAliasID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn126 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn127 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn128 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PopUpNotes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn129 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TravelerNotes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn130 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_CustomerProcessAlias_ProcessAlias");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_ProcessAlias_ProcessAliasDocumentLink");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand8 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_CustomerProcessAlias_ProcessAlias", 6);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn131 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerProcessAliasID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn132 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessAliasID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn133 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn134 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand9 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_ProcessAlias_ProcessAliasDocumentLink", 6);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn34 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DocumentLinkID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn35 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn36 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DocumentInfoID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn37 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LinkToType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn38 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LinkToKey");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand10 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_ProcessInspections_Process1", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn135 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessInspectionID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn136 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartInspectionTypeID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn137 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StepOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn138 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn139 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("COCData");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand11 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_ProcessRequisite_Process2", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn140 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessRequisiteID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn141 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ParentProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn142 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ChildProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn143 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Hours");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand12 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_ProcessRequisite_Process11", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn144 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessRequisiteID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn145 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ParentProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn146 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ChildProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn147 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Hours");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand13 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_ProcessAlias_Process3", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn148 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessAliasID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn149 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn150 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn151 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_CustomerProcessAlias_ProcessAlias1");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn152 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessAliasSearch_CustomerProcessAliasSearch");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand14 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_CustomerProcessAlias_ProcessAlias1", 12);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn153 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerProcessAliasID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn154 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessAliasID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn155 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn156 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand15 = new Infragistics.Win.UltraWinGrid.UltraGridBand("ProcessAliasSearch_CustomerProcessAliasSearch", 12);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn157 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerProcessAliasID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn158 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessAliasID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn159 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn160 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
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
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Go to the selected process in process manager.", Infragistics.Win.ToolTipImage.Default, "Go To", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessSearch));
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboProcessSearchField = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtSearch = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnSearch = new Infragistics.Win.Misc.UltraButton();
            this.chkActiveOnly = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.grdProcesses = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsProcesses = new DWOS.Data.Datasets.ProcessesDataset();
            this.taProcessSearch = new DWOS.Data.Datasets.ProcessesDatasetTableAdapters.ProcessSearchTableAdapter();
            this.taProcessAliasSearch = new DWOS.Data.Datasets.ProcessesDatasetTableAdapters.ProcessAliasSearchTableAdapter();
            this.taCustomerProcessAliasSearch = new DWOS.Data.Datasets.ProcessesDatasetTableAdapters.CustomerProcessAliasSearchTableAdapter();
            this.btnGoTo = new Infragistics.Win.Misc.UltraButton();
            this.lblRecordCount = new Infragistics.Win.Misc.UltraLabel();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessSearchField)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActiveOnly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdProcesses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsProcesses)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.ultraLabel1.Location = new System.Drawing.Point(12, 15);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(36, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Field:";
            // 
            // cboProcessSearchField
            // 
            this.cboProcessSearchField.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboProcessSearchField.Location = new System.Drawing.Point(54, 12);
            this.cboProcessSearchField.Name = "cboProcessSearchField";
            this.cboProcessSearchField.Size = new System.Drawing.Size(158, 22);
            this.cboProcessSearchField.TabIndex = 1;
            ultraToolTipInfo4.ToolTipText = "The field to search for.";
            ultraToolTipInfo4.ToolTipTitle = "Field";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboProcessSearchField, ultraToolTipInfo4);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(218, 15);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(41, 15);
            this.ultraLabel2.TabIndex = 2;
            this.ultraLabel2.Text = "Value:";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(265, 12);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(158, 22);
            this.txtSearch.TabIndex = 3;
            ultraToolTipInfo3.ToolTipText = "The value to search for.";
            ultraToolTipInfo3.ToolTipTitle = "Value";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtSearch, ultraToolTipInfo3);
            this.txtSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyUp);
            // 
            // btnSearch
            // 
            appearance14.Image = global::DWOS.UI.Properties.Resources.Search_16;
            this.btnSearch.Appearance = appearance14;
            this.btnSearch.AutoSize = true;
            this.btnSearch.Location = new System.Drawing.Point(429, 10);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(69, 26);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // chkActiveOnly
            // 
            this.chkActiveOnly.AutoSize = true;
            this.chkActiveOnly.Checked = true;
            this.chkActiveOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActiveOnly.Location = new System.Drawing.Point(504, 14);
            this.chkActiveOnly.Name = "chkActiveOnly";
            this.chkActiveOnly.Size = new System.Drawing.Size(86, 18);
            this.chkActiveOnly.TabIndex = 5;
            this.chkActiveOnly.Text = "Active Only";
            ultraToolTipInfo2.ToolTipTextFormatted = "If <strong>checked</strong>, will only search active processes.<br/>If <strong>un" +
    "checked</strong>, will search all processes.";
            ultraToolTipInfo2.ToolTipTitle = "Active Only";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkActiveOnly, ultraToolTipInfo2);
            // 
            // grdProcesses
            // 
            this.grdProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdProcesses.DataMember = "ProcessSearch";
            this.grdProcesses.DataSource = this.dsProcesses;
            appearance2.BackColor = System.Drawing.SystemColors.Window;
            appearance2.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdProcesses.DisplayLayout.Appearance = appearance2;
            this.grdProcesses.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn1.Width = 304;
            ultraGridColumn2.Header.Caption = "Code";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 351;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 57;
            ultraGridColumn22.Header.VisiblePosition = 3;
            ultraGridColumn22.Width = 57;
            ultraGridColumn23.Header.VisiblePosition = 4;
            ultraGridColumn23.Width = 57;
            ultraGridColumn24.Header.Caption = "Paperless";
            ultraGridColumn24.Header.VisiblePosition = 5;
            ultraGridColumn24.Width = 46;
            ultraGridColumn25.Header.Caption = "Modified Date";
            ultraGridColumn25.Header.VisiblePosition = 7;
            ultraGridColumn25.Width = 50;
            ultraGridColumn26.Header.VisiblePosition = 6;
            ultraGridColumn26.Width = 106;
            ultraGridColumn4.Header.VisiblePosition = 8;
            ultraGridColumn5.Header.VisiblePosition = 9;
            ultraGridColumn6.Header.VisiblePosition = 10;
            ultraGridColumn7.Header.VisiblePosition = 11;
            ultraGridColumn8.Header.VisiblePosition = 12;
            ultraGridColumn9.Header.VisiblePosition = 13;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn22,
            ultraGridColumn23,
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn26,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9});
            ultraGridColumn10.Header.VisiblePosition = 0;
            ultraGridColumn10.Width = 131;
            ultraGridColumn11.Header.VisiblePosition = 1;
            ultraGridColumn11.Width = 99;
            ultraGridColumn12.Header.VisiblePosition = 2;
            ultraGridColumn12.Width = 136;
            ultraGridColumn13.Header.VisiblePosition = 3;
            ultraGridColumn13.Width = 136;
            ultraGridColumn14.Header.VisiblePosition = 4;
            ultraGridColumn14.Width = 113;
            ultraGridColumn15.Header.VisiblePosition = 5;
            ultraGridColumn15.Width = 92;
            ultraGridColumn16.Header.VisiblePosition = 6;
            ultraGridColumn17.Header.VisiblePosition = 7;
            ultraGridColumn27.Header.VisiblePosition = 8;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn27});
            ultraGridBand2.Hidden = true;
            ultraGridColumn18.Header.VisiblePosition = 0;
            ultraGridColumn18.Width = 53;
            ultraGridColumn19.Header.VisiblePosition = 1;
            ultraGridColumn19.Width = 55;
            ultraGridColumn20.Header.VisiblePosition = 2;
            ultraGridColumn20.Width = 57;
            ultraGridColumn21.Header.VisiblePosition = 3;
            ultraGridColumn21.Width = 57;
            ultraGridColumn101.Header.VisiblePosition = 4;
            ultraGridColumn101.Width = 57;
            ultraGridColumn102.Header.VisiblePosition = 5;
            ultraGridColumn102.Width = 34;
            ultraGridColumn103.Header.VisiblePosition = 6;
            ultraGridColumn103.Width = 57;
            ultraGridColumn104.Header.VisiblePosition = 7;
            ultraGridColumn104.Width = 57;
            ultraGridColumn105.Header.VisiblePosition = 8;
            ultraGridColumn105.Width = 57;
            ultraGridColumn106.Header.VisiblePosition = 9;
            ultraGridColumn106.Width = 61;
            ultraGridColumn107.Header.VisiblePosition = 10;
            ultraGridColumn107.Width = 39;
            ultraGridColumn108.Header.VisiblePosition = 11;
            ultraGridColumn108.Width = 47;
            ultraGridColumn109.Header.VisiblePosition = 12;
            ultraGridColumn109.Width = 57;
            ultraGridColumn110.Header.VisiblePosition = 13;
            ultraGridBand3.Columns.AddRange(new object[] {
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20,
            ultraGridColumn21,
            ultraGridColumn101,
            ultraGridColumn102,
            ultraGridColumn103,
            ultraGridColumn104,
            ultraGridColumn105,
            ultraGridColumn106,
            ultraGridColumn107,
            ultraGridColumn108,
            ultraGridColumn109,
            ultraGridColumn110});
            ultraGridBand3.Hidden = true;
            ultraGridColumn111.Header.VisiblePosition = 0;
            ultraGridColumn111.Width = 125;
            ultraGridColumn112.Header.VisiblePosition = 1;
            ultraGridColumn112.Width = 87;
            ultraGridColumn113.Header.VisiblePosition = 2;
            ultraGridColumn113.Width = 108;
            ultraGridColumn114.Header.VisiblePosition = 3;
            ultraGridColumn114.Width = 93;
            ultraGridColumn115.Header.VisiblePosition = 4;
            ultraGridColumn115.Width = 93;
            ultraGridColumn116.Header.VisiblePosition = 5;
            ultraGridColumn116.Width = 93;
            ultraGridColumn117.Header.VisiblePosition = 6;
            ultraGridColumn117.Width = 70;
            ultraGridBand4.Columns.AddRange(new object[] {
            ultraGridColumn111,
            ultraGridColumn112,
            ultraGridColumn113,
            ultraGridColumn114,
            ultraGridColumn115,
            ultraGridColumn116,
            ultraGridColumn117});
            ultraGridBand4.Hidden = true;
            ultraGridColumn118.Header.VisiblePosition = 0;
            ultraGridColumn118.Width = 130;
            ultraGridColumn119.Header.VisiblePosition = 1;
            ultraGridColumn119.Width = 90;
            ultraGridColumn120.Header.VisiblePosition = 2;
            ultraGridColumn120.Width = 111;
            ultraGridColumn121.Header.VisiblePosition = 3;
            ultraGridColumn121.Width = 95;
            ultraGridColumn122.Header.VisiblePosition = 4;
            ultraGridColumn122.Width = 95;
            ultraGridColumn123.Header.VisiblePosition = 5;
            ultraGridColumn123.Width = 95;
            ultraGridColumn124.Header.VisiblePosition = 6;
            ultraGridColumn124.Width = 72;
            ultraGridBand5.Columns.AddRange(new object[] {
            ultraGridColumn118,
            ultraGridColumn119,
            ultraGridColumn120,
            ultraGridColumn121,
            ultraGridColumn122,
            ultraGridColumn123,
            ultraGridColumn124});
            ultraGridBand5.Hidden = true;
            ultraGridColumn29.Header.VisiblePosition = 0;
            ultraGridColumn30.Header.VisiblePosition = 1;
            ultraGridColumn31.Header.VisiblePosition = 2;
            ultraGridColumn32.Header.VisiblePosition = 3;
            ultraGridColumn33.Header.VisiblePosition = 4;
            ultraGridBand6.Columns.AddRange(new object[] {
            ultraGridColumn29,
            ultraGridColumn30,
            ultraGridColumn31,
            ultraGridColumn32,
            ultraGridColumn33});
            ultraGridColumn125.Header.VisiblePosition = 0;
            ultraGridColumn125.Width = 144;
            ultraGridColumn126.Header.VisiblePosition = 1;
            ultraGridColumn126.Width = 151;
            ultraGridColumn127.Header.VisiblePosition = 2;
            ultraGridColumn127.Width = 110;
            ultraGridColumn128.Header.VisiblePosition = 3;
            ultraGridColumn128.Width = 151;
            ultraGridColumn129.Header.VisiblePosition = 4;
            ultraGridColumn129.Width = 151;
            ultraGridColumn130.Header.VisiblePosition = 5;
            ultraGridColumn28.Header.VisiblePosition = 6;
            ultraGridBand7.Columns.AddRange(new object[] {
            ultraGridColumn125,
            ultraGridColumn126,
            ultraGridColumn127,
            ultraGridColumn128,
            ultraGridColumn129,
            ultraGridColumn130,
            ultraGridColumn28});
            ultraGridBand7.Hidden = true;
            ultraGridColumn131.Header.VisiblePosition = 0;
            ultraGridColumn131.Width = 235;
            ultraGridColumn132.Header.VisiblePosition = 1;
            ultraGridColumn132.Width = 156;
            ultraGridColumn133.Header.VisiblePosition = 2;
            ultraGridColumn133.Width = 135;
            ultraGridColumn134.Header.VisiblePosition = 3;
            ultraGridColumn134.Width = 162;
            ultraGridBand8.Columns.AddRange(new object[] {
            ultraGridColumn131,
            ultraGridColumn132,
            ultraGridColumn133,
            ultraGridColumn134});
            ultraGridBand8.Hidden = true;
            ultraGridColumn34.Header.VisiblePosition = 0;
            ultraGridColumn35.Header.VisiblePosition = 1;
            ultraGridColumn36.Header.VisiblePosition = 2;
            ultraGridColumn37.Header.VisiblePosition = 3;
            ultraGridColumn38.Header.VisiblePosition = 4;
            ultraGridBand9.Columns.AddRange(new object[] {
            ultraGridColumn34,
            ultraGridColumn35,
            ultraGridColumn36,
            ultraGridColumn37,
            ultraGridColumn38});
            ultraGridColumn135.Header.VisiblePosition = 0;
            ultraGridColumn135.Width = 188;
            ultraGridColumn136.Header.VisiblePosition = 1;
            ultraGridColumn136.Width = 198;
            ultraGridColumn137.Header.VisiblePosition = 2;
            ultraGridColumn137.Width = 111;
            ultraGridColumn138.Header.VisiblePosition = 3;
            ultraGridColumn138.Width = 108;
            ultraGridColumn139.Header.VisiblePosition = 4;
            ultraGridColumn139.Width = 102;
            ultraGridBand10.Columns.AddRange(new object[] {
            ultraGridColumn135,
            ultraGridColumn136,
            ultraGridColumn137,
            ultraGridColumn138,
            ultraGridColumn139});
            ultraGridBand10.Hidden = true;
            ultraGridColumn140.Header.VisiblePosition = 0;
            ultraGridColumn140.Width = 208;
            ultraGridColumn141.Header.VisiblePosition = 1;
            ultraGridColumn141.Width = 185;
            ultraGridColumn142.Header.VisiblePosition = 2;
            ultraGridColumn142.Width = 170;
            ultraGridColumn143.Header.VisiblePosition = 3;
            ultraGridColumn143.Width = 144;
            ultraGridBand11.Columns.AddRange(new object[] {
            ultraGridColumn140,
            ultraGridColumn141,
            ultraGridColumn142,
            ultraGridColumn143});
            ultraGridBand11.Hidden = true;
            ultraGridColumn144.Header.VisiblePosition = 0;
            ultraGridColumn144.Width = 213;
            ultraGridColumn145.Header.VisiblePosition = 1;
            ultraGridColumn145.Width = 190;
            ultraGridColumn146.Header.VisiblePosition = 2;
            ultraGridColumn146.Width = 174;
            ultraGridColumn147.Header.VisiblePosition = 3;
            ultraGridColumn147.Width = 147;
            ultraGridBand12.Columns.AddRange(new object[] {
            ultraGridColumn144,
            ultraGridColumn145,
            ultraGridColumn146,
            ultraGridColumn147});
            ultraGridBand12.Hidden = true;
            ultraGridColumn148.Header.VisiblePosition = 0;
            ultraGridColumn148.Hidden = true;
            ultraGridColumn148.Width = 253;
            ultraGridColumn149.Header.VisiblePosition = 1;
            ultraGridColumn149.Hidden = true;
            ultraGridColumn149.Width = 299;
            ultraGridColumn150.Header.Caption = "Alias";
            ultraGridColumn150.Header.VisiblePosition = 2;
            ultraGridColumn150.Width = 705;
            ultraGridColumn151.Header.VisiblePosition = 3;
            ultraGridColumn152.Header.VisiblePosition = 4;
            ultraGridBand13.Columns.AddRange(new object[] {
            ultraGridColumn148,
            ultraGridColumn149,
            ultraGridColumn150,
            ultraGridColumn151,
            ultraGridColumn152});
            ultraGridColumn153.Header.VisiblePosition = 0;
            ultraGridColumn153.Width = 240;
            ultraGridColumn154.Header.VisiblePosition = 1;
            ultraGridColumn154.Width = 161;
            ultraGridColumn155.Header.VisiblePosition = 2;
            ultraGridColumn155.Width = 138;
            ultraGridColumn156.Header.VisiblePosition = 3;
            ultraGridColumn156.Width = 166;
            ultraGridBand14.Columns.AddRange(new object[] {
            ultraGridColumn153,
            ultraGridColumn154,
            ultraGridColumn155,
            ultraGridColumn156});
            ultraGridBand14.Hidden = true;
            ultraGridColumn157.Header.VisiblePosition = 3;
            ultraGridColumn157.Hidden = true;
            ultraGridColumn157.Width = 226;
            ultraGridColumn158.Header.VisiblePosition = 2;
            ultraGridColumn158.Hidden = true;
            ultraGridColumn158.Width = 227;
            ultraGridColumn159.Header.Caption = "Customer Name";
            ultraGridColumn159.Header.VisiblePosition = 1;
            ultraGridColumn159.Width = 341;
            ultraGridColumn160.Header.Caption = "Alias";
            ultraGridColumn160.Header.VisiblePosition = 0;
            ultraGridColumn160.Width = 345;
            ultraGridBand15.Columns.AddRange(new object[] {
            ultraGridColumn157,
            ultraGridColumn158,
            ultraGridColumn159,
            ultraGridColumn160});
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand3);
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand4);
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand5);
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand6);
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand7);
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand8);
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand9);
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand10);
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand11);
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand12);
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand13);
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand14);
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand15);
            this.grdProcesses.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdProcesses.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance3.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance3.BorderColor = System.Drawing.SystemColors.Window;
            this.grdProcesses.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdProcesses.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.grdProcesses.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdProcesses.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.grdProcesses.DisplayLayout.MaxColScrollRegions = 1;
            this.grdProcesses.DisplayLayout.MaxRowScrollRegions = 1;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            appearance6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdProcesses.DisplayLayout.Override.ActiveCellAppearance = appearance6;
            appearance7.BackColor = System.Drawing.SystemColors.Highlight;
            appearance7.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdProcesses.DisplayLayout.Override.ActiveRowAppearance = appearance7;
            this.grdProcesses.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdProcesses.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdProcesses.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdProcesses.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.grdProcesses.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdProcesses.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance8.BackColor = System.Drawing.SystemColors.Window;
            this.grdProcesses.DisplayLayout.Override.CardAreaAppearance = appearance8;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            appearance9.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdProcesses.DisplayLayout.Override.CellAppearance = appearance9;
            this.grdProcesses.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.grdProcesses.DisplayLayout.Override.CellPadding = 0;
            this.grdProcesses.DisplayLayout.Override.ExpansionIndicator = Infragistics.Win.UltraWinGrid.ShowExpansionIndicator.CheckOnExpand;
            this.grdProcesses.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            appearance10.BackColor = System.Drawing.SystemColors.Control;
            appearance10.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance10.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance10.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance10.BorderColor = System.Drawing.SystemColors.Window;
            this.grdProcesses.DisplayLayout.Override.GroupByRowAppearance = appearance10;
            appearance11.TextHAlignAsString = "Left";
            this.grdProcesses.DisplayLayout.Override.HeaderAppearance = appearance11;
            this.grdProcesses.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdProcesses.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance12.BackColor = System.Drawing.SystemColors.Window;
            appearance12.BorderColor = System.Drawing.Color.Silver;
            this.grdProcesses.DisplayLayout.Override.RowAppearance = appearance12;
            this.grdProcesses.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdProcesses.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance13.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdProcesses.DisplayLayout.Override.TemplateAddRowAppearance = appearance13;
            this.grdProcesses.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdProcesses.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdProcesses.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdProcesses.Location = new System.Drawing.Point(44, 42);
            this.grdProcesses.Name = "grdProcesses";
            this.grdProcesses.Size = new System.Drawing.Size(764, 308);
            this.grdProcesses.SyncWithCurrencyManager = false;
            this.grdProcesses.TabIndex = 6;
            this.grdProcesses.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnUpdate;
            this.grdProcesses.BeforeRowExpanded += new Infragistics.Win.UltraWinGrid.CancelableRowEventHandler(this.grdProcesses_BeforeRowExpanded);
            // 
            // dsProcesses
            // 
            this.dsProcesses.DataSetName = "ProcessesDataset";
            this.dsProcesses.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taProcessSearch
            // 
            this.taProcessSearch.ClearBeforeFill = true;
            // 
            // taProcessAliasSearch
            // 
            this.taProcessAliasSearch.ClearBeforeFill = true;
            // 
            // taCustomerProcessAliasSearch
            // 
            this.taCustomerProcessAliasSearch.ClearBeforeFill = true;
            // 
            // btnGoTo
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Run;
            this.btnGoTo.Appearance = appearance1;
            this.btnGoTo.AutoSize = true;
            this.btnGoTo.Location = new System.Drawing.Point(12, 42);
            this.btnGoTo.Name = "btnGoTo";
            this.btnGoTo.Size = new System.Drawing.Size(26, 26);
            this.btnGoTo.TabIndex = 7;
            ultraToolTipInfo1.ToolTipText = "Go to the selected process in process manager.";
            ultraToolTipInfo1.ToolTipTitle = "Go To";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnGoTo, ultraToolTipInfo1);
            this.btnGoTo.Click += new System.EventHandler(this.btnGoTo_Click);
            // 
            // lblRecordCount
            // 
            this.lblRecordCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRecordCount.AutoSize = true;
            this.lblRecordCount.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.lblRecordCount.Location = new System.Drawing.Point(44, 361);
            this.lblRecordCount.Name = "lblRecordCount";
            this.lblRecordCount.Size = new System.Drawing.Size(97, 15);
            this.lblRecordCount.TabIndex = 8;
            this.lblRecordCount.Text = "Record Count: 0";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(622, 356);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(717, 356);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "&Cancel";
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // ProcessSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 391);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblRecordCount);
            this.Controls.Add(this.btnGoTo);
            this.Controls.Add(this.grdProcesses);
            this.Controls.Add(this.chkActiveOnly);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.cboProcessSearchField);
            this.Controls.Add(this.ultraLabel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProcessSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Process Search";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ProcessSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessSearchField)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActiveOnly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdProcesses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsProcesses)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboProcessSearchField;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSearch;
        private Infragistics.Win.Misc.UltraButton btnSearch;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkActiveOnly;
        private Data.Datasets.ProcessesDataset dsProcesses;
        private Data.Datasets.ProcessesDatasetTableAdapters.ProcessSearchTableAdapter taProcessSearch;
        private Data.Datasets.ProcessesDatasetTableAdapters.ProcessAliasSearchTableAdapter taProcessAliasSearch;
        private Data.Datasets.ProcessesDatasetTableAdapters.CustomerProcessAliasSearchTableAdapter taCustomerProcessAliasSearch;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdProcesses;
        private Infragistics.Win.Misc.UltraButton btnGoTo;
        private Infragistics.Win.Misc.UltraLabel lblRecordCount;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;

    }
}