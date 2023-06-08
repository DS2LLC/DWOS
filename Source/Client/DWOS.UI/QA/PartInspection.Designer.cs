namespace DWOS.UI
{
	partial class PartInspection
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
		    OnDisposeMe();

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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo10 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The part number.", Infragistics.Win.ToolTipImage.Default, "Part Number", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Date of QA inspection.", Infragistics.Win.ToolTipImage.Default, "Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Close dialog but do not save changes.", Infragistics.Win.ToolTipImage.Default, "Cancel", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Fail one or more of the parts being inspected.", Infragistics.Win.ToolTipImage.Default, "Fail", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Work Order Number", Infragistics.Win.ToolTipImage.Default, "Work Order", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("View the order processing answers just completed.", Infragistics.Win.ToolTipImage.Default, "View Answers", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Print the work order summary.", Infragistics.Win.ToolTipImage.Default, "Print Work Order Summary", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Pass all of items being inspected.", Infragistics.Win.ToolTipImage.Default, "Pass", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Name of user doing the inspection.", Infragistics.Win.ToolTipImage.Default, "QA Personnel", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Process Document Links", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PartInspection));
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtPartID = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.bsPartsInspection = new System.Windows.Forms.BindingSource(this.components);
            this.dsPartInspection = new DWOS.Data.Datasets.PartInspectionDataSet();
            this.dteDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnFail = new Infragistics.Win.Misc.UltraButton();
            this.txtNotes = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.flowQuestions = new System.Windows.Forms.FlowLayoutPanel();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.txtOrderID = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnView = new Infragistics.Win.Misc.UltraButton();
            this.btnPrint = new Infragistics.Win.Misc.UltraButton();
            this.btnPass = new Infragistics.Win.Misc.UltraButton();
            this.cboUser = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.docLinkList1 = new DWOS.UI.Documents.Controls.DocLinkList();
            this.ultraLabel10 = new Infragistics.Win.Misc.UltraLabel();
            this.taInspectionType = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionTypeTableAdapter();
            this.taPartInspection = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionTableAdapter();
            this.taUsers = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.UsersTableAdapter();
            this.dsOrderProcessing = new DWOS.Data.Datasets.OrderProcessingDataSet();
            this.taPartProcess = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.PartProcessTableAdapter();
            this.taOrderSummary = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter();
            this.taProcessQuestion = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessQuestionTableAdapter();
            this.taProcess = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter();
            this.taProcessSteps = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessStepsTableAdapter();
            this.taOrderProcesses = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter();
            this.taOrderProcessAnswer = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessAnswerTableAdapter();
            this.taPartInspectionAnswer = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionAnswerTableAdapter();
            this.taPartInspectionQuestion = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionQuestionTableAdapter();
            this.PartInspection_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.flowOrderInfo = new Infragistics.Win.Misc.UltraPanel();
            this.grpPartInfo = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnPartMedia = new Infragistics.Win.Misc.UltraButton();
            this.grpOrderInfo = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnOrderMedia = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.flowProcessInfo = new Infragistics.Win.Misc.UltraPanel();
            this.pnl1 = new Infragistics.Win.Misc.UltraPanel();
            this.ultraPanel6 = new Infragistics.Win.Misc.UltraPanel();
            this.ultraLabel26 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel18 = new Infragistics.Win.Misc.UltraLabel();
            this.lblTestReq = new Infragistics.Win.Misc.UltraLabel();
            this.lblTestRef = new Infragistics.Win.Misc.UltraLabel();
            this.lblInspectionType = new Infragistics.Win.Misc.UltraLabel();
            this.pnlDocuments = new Infragistics.Win.Misc.UltraPanel();
            this.flowQuestionPanel = new Infragistics.Win.Misc.UltraPanel();
            this.flowBottom = new Infragistics.Win.Misc.UltraPanel();
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            this.taLists = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.ListsTableAdapter();
            this.taListValues = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.ListValuesTableAdapter();
            this.taNumericUnits = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.NumericUnitsTableAdapter();
            this.taInputType = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.d_InputTypeTableAdapter();
            this.flowLayoutManager = new Infragistics.Win.Misc.UltraFlowLayoutManager(this.components);
            this.taCondition = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionQuestionConditionTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsPartsInspection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsPartInspection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrderProcessing)).BeginInit();
            this.PartInspection_Fill_Panel.ClientArea.SuspendLayout();
            this.PartInspection_Fill_Panel.SuspendLayout();
            this.flowOrderInfo.ClientArea.SuspendLayout();
            this.flowOrderInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpPartInfo)).BeginInit();
            this.grpPartInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpOrderInfo)).BeginInit();
            this.grpOrderInfo.SuspendLayout();
            this.flowProcessInfo.ClientArea.SuspendLayout();
            this.flowProcessInfo.SuspendLayout();
            this.pnl1.ClientArea.SuspendLayout();
            this.pnl1.SuspendLayout();
            this.ultraPanel6.ClientArea.SuspendLayout();
            this.ultraPanel6.SuspendLayout();
            this.pnlDocuments.ClientArea.SuspendLayout();
            this.pnlDocuments.SuspendLayout();
            this.flowQuestionPanel.ClientArea.SuspendLayout();
            this.flowQuestionPanel.SuspendLayout();
            this.flowBottom.ClientArea.SuspendLayout();
            this.flowBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.flowLayoutManager)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(11, 34);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(39, 18);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Part:";
            // 
            // txtPartID
            // 
            this.txtPartID.AutoSize = false;
            this.txtPartID.Location = new System.Drawing.Point(116, 30);
            this.txtPartID.Name = "txtPartID";
            this.txtPartID.ReadOnly = true;
            this.txtPartID.Size = new System.Drawing.Size(148, 25);
            this.txtPartID.TabIndex = 5;
            this.txtPartID.Text = "12345";
            ultraToolTipInfo10.ToolTipText = "The part number.";
            ultraToolTipInfo10.ToolTipTitle = "Part Number";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtPartID, ultraToolTipInfo10);
            // 
            // bsPartsInspection
            // 
            this.bsPartsInspection.DataMember = "PartInspection";
            this.bsPartsInspection.DataSource = this.dsPartInspection;
            // 
            // dsPartInspection
            // 
            this.dsPartInspection.DataSetName = "PartInspectionDataSet";
            this.dsPartInspection.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // dteDate
            // 
            this.dteDate.AutoSize = false;
            this.dteDate.Location = new System.Drawing.Point(328, 62);
            this.dteDate.Name = "dteDate";
            this.dteDate.ReadOnly = true;
            this.dteDate.Size = new System.Drawing.Size(149, 25);
            this.dteDate.TabIndex = 4;
            ultraToolTipInfo3.ToolTipText = "Date of QA inspection.";
            ultraToolTipInfo3.ToolTipTitle = "Date";
            this.ultraToolTipManager1.SetUltraToolTip(this.dteDate, ultraToolTipInfo3);
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(11, 65);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(103, 18);
            this.ultraLabel5.TabIndex = 32;
            this.ultraLabel5.Text = "QA Personnel:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(427, 62);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 27);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            ultraToolTipInfo1.ToolTipText = "Close dialog but do not save changes.";
            ultraToolTipInfo1.ToolTipTitle = "Cancel";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnCancel, ultraToolTipInfo1);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnFail
            // 
            this.btnFail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance10.Image = global::DWOS.UI.Properties.Resources.RunFail;
            this.btnFail.Appearance = appearance10;
            this.btnFail.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnFail.Location = new System.Drawing.Point(345, 62);
            this.btnFail.Name = "btnFail";
            this.btnFail.Size = new System.Drawing.Size(76, 27);
            this.btnFail.TabIndex = 9;
            this.btnFail.Text = "Fail";
            ultraToolTipInfo9.ToolTipText = "Fail one or more of the parts being inspected.";
            ultraToolTipInfo9.ToolTipTitle = "Fail";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnFail, ultraToolTipInfo9);
            this.btnFail.Click += new System.EventHandler(this.btnFail_Click);
            // 
            // txtNotes
            // 
            this.txtNotes.AcceptsReturn = true;
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotes.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNotes.Location = new System.Drawing.Point(9, 0);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.NullText = "Notes";
            appearance8.ForeColor = System.Drawing.Color.LightGray;
            this.txtNotes.NullTextAppearance = appearance8;
            this.txtNotes.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(496, 36);
            this.txtNotes.TabIndex = 4;
            // 
            // flowQuestions
            // 
            this.flowQuestions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowQuestions.AutoScroll = true;
            this.flowQuestions.BackColor = System.Drawing.Color.Silver;
            this.flowQuestions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowQuestions.Location = new System.Drawing.Point(9, 3);
            this.flowQuestions.Name = "flowQuestions";
            this.flowQuestions.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.flowQuestions.Size = new System.Drawing.Size(496, 233);
            this.inboxControlStyler1.SetStyleSettings(this.flowQuestions, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.flowQuestions.TabIndex = 0;
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // txtOrderID
            // 
            this.txtOrderID.AutoSize = false;
            this.txtOrderID.Location = new System.Drawing.Point(116, 31);
            this.txtOrderID.Name = "txtOrderID";
            this.txtOrderID.ReadOnly = true;
            this.txtOrderID.Size = new System.Drawing.Size(148, 25);
            this.txtOrderID.TabIndex = 1;
            this.txtOrderID.Text = "12345";
            ultraToolTipInfo2.ToolTipText = "Work Order Number";
            ultraToolTipInfo2.ToolTipTitle = "Work Order";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtOrderID, ultraToolTipInfo2);
            // 
            // btnView
            // 
            this.btnView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnView.Location = new System.Drawing.Point(6, 62);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(75, 23);
            this.btnView.TabIndex = 6;
            this.btnView.Text = "View";
            ultraToolTipInfo6.ToolTipText = "View the order processing answers just completed.";
            ultraToolTipInfo6.ToolTipTitle = "View Answers";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnView, ultraToolTipInfo6);
            this.btnView.Click += new System.EventHandler(this.btnViewAnswers_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrint.Location = new System.Drawing.Point(87, 63);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 7;
            this.btnPrint.Text = "Print";
            ultraToolTipInfo8.ToolTipText = "Print the work order summary.";
            ultraToolTipInfo8.ToolTipTitle = "Print Work Order Summary";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnPrint, ultraToolTipInfo8);
            this.btnPrint.Click += new System.EventHandler(this.btnPrintAnswers_Click);
            // 
            // btnPass
            // 
            this.btnPass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance9.Image = global::DWOS.UI.Properties.Resources.RunComplete;
            this.btnPass.Appearance = appearance9;
            this.btnPass.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnPass.Location = new System.Drawing.Point(264, 62);
            this.btnPass.Name = "btnPass";
            this.btnPass.Size = new System.Drawing.Size(75, 27);
            this.btnPass.TabIndex = 8;
            this.btnPass.Text = "Pass";
            ultraToolTipInfo7.ToolTipText = "Pass all of items being inspected.";
            ultraToolTipInfo7.ToolTipTitle = "Pass";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnPass, ultraToolTipInfo7);
            this.btnPass.Click += new System.EventHandler(this.btnPass_Click);
            // 
            // cboUser
            // 
            this.cboUser.AutoSize = false;
            this.cboUser.DataMember = "Users";
            this.cboUser.DataSource = this.dsPartInspection;
            this.cboUser.DisplayMember = "Name";
            this.cboUser.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboUser.Location = new System.Drawing.Point(116, 62);
            this.cboUser.Name = "cboUser";
            this.cboUser.ReadOnly = true;
            this.cboUser.Size = new System.Drawing.Size(148, 25);
            this.cboUser.TabIndex = 3;
            ultraToolTipInfo4.ToolTipText = "Name of user doing the inspection.";
            ultraToolTipInfo4.ToolTipTitle = "QA Personnel";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboUser, ultraToolTipInfo4);
            this.cboUser.ValueMember = "UserID";
            // 
            // docLinkList1
            // 
            this.docLinkList1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.docLinkList1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.docLinkList1.Location = new System.Drawing.Point(9, 4);
            this.docLinkList1.Margin = new System.Windows.Forms.Padding(0);
            this.docLinkList1.Name = "docLinkList1";
            this.docLinkList1.Size = new System.Drawing.Size(496, 27);
            this.docLinkList1.TabIndex = 4;
            ultraToolTipInfo5.ToolTipTextFormatted = "Provides quick access to pre-defined links for this process or alias.";
            ultraToolTipInfo5.ToolTipTitle = "Process Document Links";
            this.ultraToolTipManager1.SetUltraToolTip(this.docLinkList1, ultraToolTipInfo5);
            // 
            // ultraLabel10
            // 
            this.ultraLabel10.AutoSize = true;
            this.ultraLabel10.Location = new System.Drawing.Point(11, 34);
            this.ultraLabel10.Name = "ultraLabel10";
            this.ultraLabel10.Size = new System.Drawing.Size(91, 18);
            this.ultraLabel10.TabIndex = 44;
            this.ultraLabel10.Text = "Work Order:";
            // 
            // taInspectionType
            // 
            this.taInspectionType.ClearBeforeFill = true;
            // 
            // taPartInspection
            // 
            this.taPartInspection.ClearBeforeFill = true;
            // 
            // taUsers
            // 
            this.taUsers.ClearBeforeFill = true;
            // 
            // dsOrderProcessing
            // 
            this.dsOrderProcessing.DataSetName = "OrderProcessingDataSet";
            this.dsOrderProcessing.EnforceConstraints = false;
            this.dsOrderProcessing.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taPartProcess
            // 
            this.taPartProcess.ClearBeforeFill = true;
            // 
            // taOrderSummary
            // 
            this.taOrderSummary.ClearBeforeFill = true;
            // 
            // taProcessQuestion
            // 
            this.taProcessQuestion.ClearBeforeFill = true;
            // 
            // taProcess
            // 
            this.taProcess.ClearBeforeFill = true;
            // 
            // taProcessSteps
            // 
            this.taProcessSteps.ClearBeforeFill = true;
            // 
            // taOrderProcesses
            // 
            this.taOrderProcesses.ClearBeforeFill = true;
            // 
            // taOrderProcessAnswer
            // 
            this.taOrderProcessAnswer.ClearBeforeFill = true;
            // 
            // taPartInspectionAnswer
            // 
            this.taPartInspectionAnswer.ClearBeforeFill = true;
            // 
            // taPartInspectionQuestion
            // 
            this.taPartInspectionQuestion.ClearBeforeFill = true;
            // 
            // PartInspection_Fill_Panel
            // 
            // 
            // PartInspection_Fill_Panel.ClientArea
            // 
            this.PartInspection_Fill_Panel.ClientArea.Controls.Add(this.flowOrderInfo);
            this.PartInspection_Fill_Panel.ClientArea.Controls.Add(this.flowProcessInfo);
            this.PartInspection_Fill_Panel.ClientArea.Controls.Add(this.pnlDocuments);
            this.PartInspection_Fill_Panel.ClientArea.Controls.Add(this.flowQuestionPanel);
            this.PartInspection_Fill_Panel.ClientArea.Controls.Add(this.flowBottom);
            this.PartInspection_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.PartInspection_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PartInspection_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.PartInspection_Fill_Panel.Name = "PartInspection_Fill_Panel";
            this.PartInspection_Fill_Panel.Size = new System.Drawing.Size(515, 661);
            this.PartInspection_Fill_Panel.TabIndex = 0;
            // 
            // flowOrderInfo
            // 
            this.flowOrderInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // flowOrderInfo.ClientArea
            // 
            this.flowOrderInfo.ClientArea.Controls.Add(this.grpPartInfo);
            this.flowOrderInfo.ClientArea.Controls.Add(this.grpOrderInfo);
            this.flowOrderInfo.Location = new System.Drawing.Point(0, 5);
            this.flowOrderInfo.Name = "flowOrderInfo";
            this.flowOrderInfo.Size = new System.Drawing.Size(512, 168);
            this.flowOrderInfo.TabIndex = 1;
            // 
            // grpPartInfo
            // 
            this.grpPartInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPartInfo.Controls.Add(this.btnPartMedia);
            this.grpPartInfo.Controls.Add(this.txtPartID);
            this.grpPartInfo.Controls.Add(this.ultraLabel1);
            this.grpPartInfo.Font = new System.Drawing.Font("Verdana", 10F);
            appearance1.Image = global::DWOS.UI.Properties.Resources.Part_16;
            this.grpPartInfo.HeaderAppearance = appearance1;
            this.grpPartInfo.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.grpPartInfo.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpPartInfo.Location = new System.Drawing.Point(9, 104);
            this.grpPartInfo.Name = "grpPartInfo";
            this.grpPartInfo.Size = new System.Drawing.Size(496, 64);
            this.grpPartInfo.TabIndex = 2;
            this.grpPartInfo.Text = "Part Information";
            // 
            // btnPartMedia
            // 
            this.btnPartMedia.Location = new System.Drawing.Point(278, 30);
            this.btnPartMedia.Name = "btnPartMedia";
            this.btnPartMedia.Size = new System.Drawing.Size(199, 23);
            this.btnPartMedia.TabIndex = 6;
            this.btnPartMedia.Text = "Show/Edit Media";
            this.btnPartMedia.Click += new System.EventHandler(this.btnPartMedia_Click);
            // 
            // grpOrderInfo
            // 
            this.grpOrderInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOrderInfo.Controls.Add(this.btnOrderMedia);
            this.grpOrderInfo.Controls.Add(this.ultraLabel2);
            this.grpOrderInfo.Controls.Add(this.txtOrderID);
            this.grpOrderInfo.Controls.Add(this.ultraLabel10);
            this.grpOrderInfo.Controls.Add(this.dteDate);
            this.grpOrderInfo.Controls.Add(this.cboUser);
            this.grpOrderInfo.Controls.Add(this.ultraLabel5);
            this.grpOrderInfo.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            appearance2.Image = global::DWOS.UI.Properties.Resources.Order_16;
            this.grpOrderInfo.HeaderAppearance = appearance2;
            this.grpOrderInfo.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grpOrderInfo.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpOrderInfo.Location = new System.Drawing.Point(9, 2);
            this.grpOrderInfo.Name = "grpOrderInfo";
            this.grpOrderInfo.Size = new System.Drawing.Size(496, 95);
            this.grpOrderInfo.TabIndex = 1;
            this.grpOrderInfo.Text = "Order Information";
            // 
            // btnOrderMedia
            // 
            this.btnOrderMedia.Location = new System.Drawing.Point(278, 31);
            this.btnOrderMedia.Name = "btnOrderMedia";
            this.btnOrderMedia.Size = new System.Drawing.Size(199, 23);
            this.btnOrderMedia.TabIndex = 2;
            this.btnOrderMedia.Text = "Show/Edit Media";
            this.btnOrderMedia.Click += new System.EventHandler(this.btnOrderMedia_Click);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(278, 65);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(44, 18);
            this.ultraLabel2.TabIndex = 45;
            this.ultraLabel2.Text = "Date:";
            // 
            // flowProcessInfo
            // 
            this.flowProcessInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // flowProcessInfo.ClientArea
            // 
            this.flowProcessInfo.ClientArea.Controls.Add(this.pnl1);
            this.flowProcessInfo.Location = new System.Drawing.Point(0, 178);
            this.flowProcessInfo.Name = "flowProcessInfo";
            this.flowProcessInfo.Size = new System.Drawing.Size(512, 100);
            this.flowProcessInfo.TabIndex = 2;
            // 
            // pnl1
            // 
            this.pnl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance3.BackColor = System.Drawing.Color.Khaki;
            this.pnl1.Appearance = appearance3;
            // 
            // pnl1.ClientArea
            // 
            this.pnl1.ClientArea.Controls.Add(this.ultraPanel6);
            this.pnl1.ClientArea.Controls.Add(this.lblInspectionType);
            this.pnl1.Location = new System.Drawing.Point(9, 3);
            this.pnl1.Name = "pnl1";
            this.pnl1.Size = new System.Drawing.Size(496, 96);
            this.pnl1.TabIndex = 3;
            // 
            // ultraPanel6
            // 
            appearance4.BackColor = System.Drawing.Color.White;
            this.ultraPanel6.Appearance = appearance4;
            this.ultraPanel6.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            // 
            // ultraPanel6.ClientArea
            // 
            this.ultraPanel6.ClientArea.Controls.Add(this.ultraLabel26);
            this.ultraPanel6.ClientArea.Controls.Add(this.ultraLabel18);
            this.ultraPanel6.ClientArea.Controls.Add(this.lblTestReq);
            this.ultraPanel6.ClientArea.Controls.Add(this.lblTestRef);
            this.ultraPanel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ultraPanel6.Location = new System.Drawing.Point(0, 35);
            this.ultraPanel6.Name = "ultraPanel6";
            this.ultraPanel6.Size = new System.Drawing.Size(496, 61);
            this.ultraPanel6.TabIndex = 30;
            // 
            // ultraLabel26
            // 
            this.ultraLabel26.AutoSize = true;
            this.ultraLabel26.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel26.Location = new System.Drawing.Point(15, 29);
            this.ultraLabel26.Name = "ultraLabel26";
            this.ultraLabel26.Size = new System.Drawing.Size(104, 18);
            this.ultraLabel26.TabIndex = 16;
            this.ultraLabel26.Text = "Requirements:";
            this.ultraLabel26.UseAppStyling = false;
            // 
            // ultraLabel18
            // 
            this.ultraLabel18.AutoSize = true;
            this.ultraLabel18.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel18.Location = new System.Drawing.Point(15, 4);
            this.ultraLabel18.Name = "ultraLabel18";
            this.ultraLabel18.Size = new System.Drawing.Size(78, 18);
            this.ultraLabel18.TabIndex = 15;
            this.ultraLabel18.Text = "Reference:";
            this.ultraLabel18.UseAppStyling = false;
            // 
            // lblTestReq
            // 
            appearance5.ForeColor = System.Drawing.Color.DimGray;
            this.lblTestReq.Appearance = appearance5;
            this.lblTestReq.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTestReq.Location = new System.Drawing.Point(125, 29);
            this.lblTestReq.Name = "lblTestReq";
            this.lblTestReq.Size = new System.Drawing.Size(355, 19);
            this.lblTestReq.TabIndex = 14;
            this.lblTestReq.Text = "NA";
            this.lblTestReq.UseAppStyling = false;
            // 
            // lblTestRef
            // 
            appearance6.ForeColor = System.Drawing.Color.DimGray;
            this.lblTestRef.Appearance = appearance6;
            this.lblTestRef.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTestRef.Location = new System.Drawing.Point(125, 4);
            this.lblTestRef.Name = "lblTestRef";
            this.lblTestRef.Size = new System.Drawing.Size(355, 19);
            this.lblTestRef.TabIndex = 13;
            this.lblTestRef.Text = "NA";
            this.lblTestRef.UseAppStyling = false;
            // 
            // lblInspectionType
            // 
            appearance7.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblInspectionType.Appearance = appearance7;
            this.lblInspectionType.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInspectionType.Location = new System.Drawing.Point(3, 5);
            this.lblInspectionType.Name = "lblInspectionType";
            this.lblInspectionType.Size = new System.Drawing.Size(477, 26);
            this.lblInspectionType.TabIndex = 11;
            this.lblInspectionType.Text = "None";
            this.lblInspectionType.UseAppStyling = false;
            // 
            // pnlDocuments
            // 
            this.pnlDocuments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // pnlDocuments.ClientArea
            // 
            this.pnlDocuments.ClientArea.Controls.Add(this.docLinkList1);
            this.pnlDocuments.Location = new System.Drawing.Point(0, 283);
            this.pnlDocuments.Name = "pnlDocuments";
            this.pnlDocuments.Size = new System.Drawing.Size(512, 35);
            this.pnlDocuments.TabIndex = 3;
            // 
            // flowQuestionPanel
            // 
            this.flowQuestionPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // flowQuestionPanel.ClientArea
            // 
            this.flowQuestionPanel.ClientArea.Controls.Add(this.flowQuestions);
            this.flowQuestionPanel.Location = new System.Drawing.Point(0, 323);
            this.flowQuestionPanel.Name = "flowQuestionPanel";
            this.flowQuestionPanel.Size = new System.Drawing.Size(512, 239);
            this.flowQuestionPanel.TabIndex = 4;
            // 
            // flowBottom
            // 
            this.flowBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowBottom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            // 
            // flowBottom.ClientArea
            // 
            this.flowBottom.ClientArea.Controls.Add(this.txtNotes);
            this.flowBottom.ClientArea.Controls.Add(this.btnView);
            this.flowBottom.ClientArea.Controls.Add(this.btnPass);
            this.flowBottom.ClientArea.Controls.Add(this.helpLink1);
            this.flowBottom.ClientArea.Controls.Add(this.btnPrint);
            this.flowBottom.ClientArea.Controls.Add(this.btnCancel);
            this.flowBottom.ClientArea.Controls.Add(this.btnFail);
            this.flowBottom.Location = new System.Drawing.Point(0, 567);
            this.flowBottom.Name = "flowBottom";
            this.flowBottom.Size = new System.Drawing.Size(515, 92);
            this.flowBottom.TabIndex = 0;
            // 
            // helpLink1
            // 
            this.helpLink1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = "control_inspection_dialog.htm";
            this.helpLink1.Location = new System.Drawing.Point(9, 40);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 5;
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
            // taInputType
            // 
            this.taInputType.ClearBeforeFill = true;
            // 
            // flowLayoutManager
            // 
            this.flowLayoutManager.ContainerControl = this.PartInspection_Fill_Panel.ClientArea;
            this.flowLayoutManager.HorizontalAlignment = Infragistics.Win.Layout.DefaultableFlowLayoutAlignment.Near;
            this.flowLayoutManager.HorizontalGap = 0;
            this.flowLayoutManager.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.flowLayoutManager.VerticalAlignment = Infragistics.Win.Layout.DefaultableFlowLayoutAlignment.Near;
            this.flowLayoutManager.WrapItems = false;
            // 
            // taCondition
            // 
            this.taCondition.ClearBeforeFill = true;
            // 
            // PartInspection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(515, 661);
            this.Controls.Add(this.PartInspection_Fill_Panel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "PartInspection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Control Inspection";
            this.Load += new System.EventHandler(this.PartInspection_Load);
            this.Shown += new System.EventHandler(this.PartInspection_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.txtPartID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsPartsInspection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsPartInspection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrderProcessing)).EndInit();
            this.PartInspection_Fill_Panel.ClientArea.ResumeLayout(false);
            this.PartInspection_Fill_Panel.ResumeLayout(false);
            this.flowOrderInfo.ClientArea.ResumeLayout(false);
            this.flowOrderInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpPartInfo)).EndInit();
            this.grpPartInfo.ResumeLayout(false);
            this.grpPartInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpOrderInfo)).EndInit();
            this.grpOrderInfo.ResumeLayout(false);
            this.grpOrderInfo.PerformLayout();
            this.flowProcessInfo.ClientArea.ResumeLayout(false);
            this.flowProcessInfo.ResumeLayout(false);
            this.pnl1.ClientArea.ResumeLayout(false);
            this.pnl1.ResumeLayout(false);
            this.ultraPanel6.ClientArea.ResumeLayout(false);
            this.ultraPanel6.ClientArea.PerformLayout();
            this.ultraPanel6.ResumeLayout(false);
            this.pnlDocuments.ClientArea.ResumeLayout(false);
            this.pnlDocuments.ResumeLayout(false);
            this.flowQuestionPanel.ClientArea.ResumeLayout(false);
            this.flowQuestionPanel.ResumeLayout(false);
            this.flowBottom.ClientArea.ResumeLayout(false);
            this.flowBottom.ClientArea.PerformLayout();
            this.flowBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.flowLayoutManager)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPartID;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboUser;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteDate;
		private Infragistics.Win.Misc.UltraLabel ultraLabel5;
		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnFail;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNotes;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
		private DWOS.Data.Datasets.PartInspectionDataSet dsPartInspection;
        private DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionTypeTableAdapter taInspectionType;
		private DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionTableAdapter taPartInspection;
		private DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.UsersTableAdapter taUsers;
		private System.Windows.Forms.BindingSource bsPartsInspection;
		private DWOS.Data.Datasets.OrderProcessingDataSet dsOrderProcessing;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.PartProcessTableAdapter taPartProcess;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter taOrderSummary;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessQuestionTableAdapter taProcessQuestion;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter taProcess;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtOrderID;
		private Infragistics.Win.Misc.UltraLabel ultraLabel10;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessStepsTableAdapter taProcessSteps;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter taOrderProcesses;
        private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessAnswerTableAdapter taOrderProcessAnswer;
		private DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionAnswerTableAdapter taPartInspectionAnswer;
		private DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionQuestionTableAdapter taPartInspectionQuestion;
        private System.Windows.Forms.FlowLayoutPanel flowQuestions;
		private Infragistics.Win.Misc.UltraPanel PartInspection_Fill_Panel;
        private Infragistics.Win.Misc.UltraGroupBox grpOrderInfo;
        private Infragistics.Win.Misc.UltraPanel pnl1;
        private Infragistics.Win.Misc.UltraPanel ultraPanel6;
        private Infragistics.Win.Misc.UltraLabel ultraLabel26;
        private Infragistics.Win.Misc.UltraLabel ultraLabel18;
        private Infragistics.Win.Misc.UltraLabel lblTestReq;
        private Infragistics.Win.Misc.UltraLabel lblTestRef;
        private Infragistics.Win.Misc.UltraLabel lblInspectionType;
		private Data.Datasets.PartInspectionDataSetTableAdapters.ListsTableAdapter taLists;
		private Data.Datasets.PartInspectionDataSetTableAdapters.ListValuesTableAdapter taListValues;
		private Data.Datasets.PartInspectionDataSetTableAdapters.NumericUnitsTableAdapter taNumericUnits;
        private Data.Datasets.PartInspectionDataSetTableAdapters.d_InputTypeTableAdapter taInputType;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraButton btnPrint;
        private Infragistics.Win.Misc.UltraButton btnView;
        private Infragistics.Win.Misc.UltraButton btnPass;
        private Utilities.HelpLink helpLink1;
        private Documents.Controls.DocLinkList docLinkList1;
        private Infragistics.Win.Misc.UltraPanel flowQuestionPanel;
        private Infragistics.Win.Misc.UltraPanel flowBottom;
        private Infragistics.Win.Misc.UltraFlowLayoutManager flowLayoutManager;
        private Infragistics.Win.Misc.UltraPanel flowProcessInfo;
        private Infragistics.Win.Misc.UltraPanel flowOrderInfo;
        private Infragistics.Win.Misc.UltraGroupBox grpPartInfo;
        private Infragistics.Win.Misc.UltraButton btnOrderMedia;
        private Infragistics.Win.Misc.UltraButton btnPartMedia;
        private Infragistics.Win.Misc.UltraPanel pnlDocuments;
        private Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionQuestionConditionTableAdapter taCondition;
    }
}