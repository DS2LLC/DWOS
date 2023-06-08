namespace DWOS.UI.QA
{
	partial class QIManager
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
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab1 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbon1");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup1 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup1");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddQuestion");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool3 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("DisplayInactive", "");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup2 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup2");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Copy");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Paste");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CreateRevision");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Copy");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Paste");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("TOCContext");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Copy");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Paste");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddQuestion");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("DisplayInactive", "");
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CreateRevision");
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QIManager));
            this.taInspectionType = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionTypeTableAdapter();
            this.dsPartInspection = new DWOS.Data.Datasets.PartInspectionDataSet();
            this.taPartInspectionQuestion = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionQuestionTableAdapter();
            this.taManager = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.TableAdapterManager();
            this.taPartInspectionQuestionCondition = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionQuestionConditionTableAdapter();
            this.taInspectionTypeDocumentLink = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionTypeDocumentLinkTableAdapter();
            this.pnlInfo = new DWOS.UI.QA.QIManagerPanels.QIInspectionInfo();
            this.taLists = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.ListsTableAdapter();
            this.taListValues = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.ListValuesTableAdapter();
            this.taNumericUnits = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.NumericUnitsTableAdapter();
            this.pnlQuestion = new DWOS.UI.QA.QIManagerPanels.QIQuestion();
            this.taInputType = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.d_InputTypeTableAdapter();
            this.taRevision = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionRevisionTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsPartInspection)).BeginInit();
            this.SuspendLayout();
            // 
            // toolbarManager
            // 
            this.toolbarManager.MenuSettings.ForceSerialization = true;
            appearance1.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.toolbarManager.Ribbon.ApplicationMenu2010.ContentArea.Settings.Appearance = appearance1;
            this.toolbarManager.Ribbon.Caption = "QI Manager";
            this.toolbarManager.Ribbon.FileMenuStyle = Infragistics.Win.UltraWinToolbars.FileMenuStyle.ApplicationMenu2010;
            ribbonTab1.Caption = "Home";
            ribbonGroup1.Caption = "Quality Inspections";
            ribbonGroup1.PreferredToolSize = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool13,
            buttonTool17,
            stateButtonTool3});
            ribbonGroup2.Caption = "Edit";
            buttonTool3.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool12.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool2.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool1.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool3,
            buttonTool12,
            buttonTool2,
            buttonTool1});
            ribbonTab1.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup1,
            ribbonGroup2});
            this.toolbarManager.Ribbon.NonInheritedRibbonTabs.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonTab[] {
            ribbonTab1});
            this.toolbarManager.Ribbon.Visible = true;
            this.toolbarManager.ToolbarSettings.ForceSerialization = true;
            appearance2.Image = global::DWOS.UI.Properties.Resources.Add_32;
            buttonTool4.SharedPropsInternal.AppearancesLarge.Appearance = appearance2;
            buttonTool4.SharedPropsInternal.Caption = "Add Inspection";
            buttonTool4.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.DefaultForToolType;
            buttonTool4.SharedPropsInternal.Enabled = false;
            buttonTool4.SharedPropsInternal.ToolTipTextFormatted = "Add a new inspection.";
            buttonTool4.SharedPropsInternal.ToolTipTitle = "Add Inspection";
            appearance3.Image = global::DWOS.UI.Properties.Resources.Delete_32;
            buttonTool5.SharedPropsInternal.AppearancesLarge.Appearance = appearance3;
            buttonTool5.SharedPropsInternal.Caption = "Delete";
            buttonTool5.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.DefaultForToolType;
            buttonTool5.SharedPropsInternal.Enabled = false;
            buttonTool5.SharedPropsInternal.ToolTipTextFormatted = "Delete the selected inspection.";
            buttonTool5.SharedPropsInternal.ToolTipTitle = "Delete";
            appearance4.Image = global::DWOS.UI.Properties.Resources.Copy_32;
            buttonTool6.SharedPropsInternal.AppearancesLarge.Appearance = appearance4;
            buttonTool6.SharedPropsInternal.Caption = "Copy";
            buttonTool6.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.DefaultForToolType;
            buttonTool6.SharedPropsInternal.Enabled = false;
            buttonTool6.SharedPropsInternal.ToolTipTextFormatted = "Copy the inspection.";
            buttonTool6.SharedPropsInternal.ToolTipTitle = "Copy";
            appearance5.Image = global::DWOS.UI.Properties.Resources.editpaste;
            buttonTool7.SharedPropsInternal.AppearancesLarge.Appearance = appearance5;
            buttonTool7.SharedPropsInternal.Caption = "Paste";
            buttonTool7.SharedPropsInternal.Enabled = false;
            buttonTool7.SharedPropsInternal.ToolTipTextFormatted = "Paste the copied inspection.";
            buttonTool7.SharedPropsInternal.ToolTipTitle = "Paste";
            popupMenuTool1.SharedPropsInternal.Caption = "TOCContext";
            buttonTool11.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool9,
            buttonTool10,
            buttonTool11,
            buttonTool8});
            appearance6.Image = global::DWOS.UI.Properties.Resources.Add_32;
            buttonTool18.SharedPropsInternal.AppearancesLarge.Appearance = appearance6;
            buttonTool18.SharedPropsInternal.Caption = "Add Question";
            buttonTool18.SharedPropsInternal.Enabled = false;
            buttonTool18.SharedPropsInternal.ToolTipTextFormatted = "Add a new question.";
            buttonTool18.SharedPropsInternal.ToolTipTitle = "Add Question";
            appearance7.Image = global::DWOS.UI.Properties.Resources.Checked_32;
            stateButtonTool2.SharedPropsInternal.AppearancesLarge.Appearance = appearance7;
            appearance8.Image = global::DWOS.UI.Properties.Resources.Checked_32;
            stateButtonTool2.SharedPropsInternal.AppearancesSmall.Appearance = appearance8;
            stateButtonTool2.SharedPropsInternal.Caption = "Display Inactive";
            appearance9.Image = global::DWOS.UI.Properties.Resources.Revision;
            buttonTool14.SharedPropsInternal.AppearancesLarge.Appearance = appearance9;
            appearance10.Image = global::DWOS.UI.Properties.Resources.Revision;
            buttonTool14.SharedPropsInternal.AppearancesSmall.Appearance = appearance10;
            buttonTool14.SharedPropsInternal.Caption = "Create Revision";
            buttonTool14.SharedPropsInternal.Enabled = false;
            this.toolbarManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool4,
            buttonTool5,
            buttonTool6,
            buttonTool7,
            popupMenuTool1,
            buttonTool18,
            stateButtonTool2,
            buttonTool14});
            // 
            // tvwTOC
            // 
            this.toolbarManager.SetContextMenuUltra(this.tvwTOC, "TOCContext");
            this.tvwTOC.Size = new System.Drawing.Size(266, 900);
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlQuestion);
            this.splitContainer1.Panel2.Controls.Add(this.pnlInfo);
            this.splitContainer1.Size = new System.Drawing.Size(1125, 900);
            this.splitContainer1.SplitterDistance = 266;
            // 
            // helpLink1
            // 
            this.helpLink1.HelpPage = "qi_manager_dialog.htm";
            // 
            // taInspectionType
            // 
            this.taInspectionType.ClearBeforeFill = true;
            // 
            // dsPartInspection
            // 
            this.dsPartInspection.DataSetName = "PartInspectionDataSet";
            this.dsPartInspection.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taPartInspectionQuestion
            // 
            this.taPartInspectionQuestion.ClearBeforeFill = true;
            // 
            // taManager
            // 
            this.taManager.BackupDataSetBeforeUpdate = false;
            this.taManager.InspectionAnswerSampleTableAdapter = null;
            this.taManager.PartInspectionAnswerTableAdapter = null;
            this.taManager.PartInspectionQuestionConditionTableAdapter = this.taPartInspectionQuestionCondition;
            this.taManager.PartInspectionQuestionTableAdapter = this.taPartInspectionQuestion;
            this.taManager.PartInspectionTableAdapter = null;
            this.taManager.PartInspectionTypeDocumentLinkTableAdapter = this.taInspectionTypeDocumentLink;
            this.taManager.PartInspectionTypeTableAdapter = this.taInspectionType;
            this.taManager.UpdateOrder = DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            // 
            // taPartInspectionQuestionCondition
            // 
            this.taPartInspectionQuestionCondition.ClearBeforeFill = false;
            // 
            // taInspectionTypeDocumentLink
            // 
            this.taInspectionTypeDocumentLink.ClearBeforeFill = true;
            // 
            // pnlInfo
            // 
            this.pnlInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlInfo.Dataset = null;
            this.pnlInfo.Editable = true;
            this.pnlInfo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlInfo.IsActivePanel = false;
            this.pnlInfo.Location = new System.Drawing.Point(3, 359);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Padding = new System.Windows.Forms.Padding(5);
            this.pnlInfo.Size = new System.Drawing.Size(833, 535);
            this.pnlInfo.TabIndex = 0;
            // 
            // taLists
            // 
            this.taLists.ClearBeforeFill = true;
            // 
            // taListValues
            // 
            this.taListValues.ClearBeforeFill = true;
            // 
            // taNumericUnits
            // 
            this.taNumericUnits.ClearBeforeFill = true;
            // 
            // pnlQuestion
            // 
            this.pnlQuestion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlQuestion.Dataset = null;
            this.pnlQuestion.Editable = true;
            this.pnlQuestion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlQuestion.IsActivePanel = false;
            this.pnlQuestion.Location = new System.Drawing.Point(3, 3);
            this.pnlQuestion.Name = "pnlQuestion";
            this.pnlQuestion.Padding = new System.Windows.Forms.Padding(5);
            this.pnlQuestion.Size = new System.Drawing.Size(833, 350);
            this.pnlQuestion.TabIndex = 1;
            // 
            // taInputType
            // 
            this.taInputType.ClearBeforeFill = true;
            // 
            // taRevision
            // 
            this.taRevision.ClearBeforeFill = true;
            // 
            // QIManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(1141, 1100);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "QIManager";
            this.Text = "Control Inspection Manager";
            this.Load += new System.EventHandler(this.QIManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dsPartInspection)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionTypeTableAdapter taInspectionType;
		private Data.Datasets.PartInspectionDataSet dsPartInspection;
		private Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionQuestionTableAdapter taPartInspectionQuestion;
		private Data.Datasets.PartInspectionDataSetTableAdapters.TableAdapterManager taManager;
		private QIManagerPanels.QIInspectionInfo pnlInfo;
		private Data.Datasets.PartInspectionDataSetTableAdapters.ListsTableAdapter taLists;
		private Data.Datasets.PartInspectionDataSetTableAdapters.ListValuesTableAdapter taListValues;
		private Data.Datasets.PartInspectionDataSetTableAdapters.NumericUnitsTableAdapter taNumericUnits;
		private QIManagerPanels.QIQuestion pnlQuestion;
		private Data.Datasets.PartInspectionDataSetTableAdapters.d_InputTypeTableAdapter taInputType;
        private Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionTypeDocumentLinkTableAdapter taInspectionTypeDocumentLink;
        private Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionRevisionTableAdapter taRevision;
        private Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionQuestionConditionTableAdapter taPartInspectionQuestionCondition;

        //private DWOS.Data.Datasets.SecurityDataSetTableAdapters.UserRolesTableAdapter taUserRoles;
        //private DWOS.Data.Datasets.SecurityDataSetTableAdapters.d_RolesTableAdapter taRoles;
    }
}
