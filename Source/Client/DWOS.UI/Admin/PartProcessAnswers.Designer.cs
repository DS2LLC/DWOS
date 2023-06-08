namespace DWOS.UI.Admin
{
	partial class PartProcessAnswers
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Legend", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The type of question.", Infragistics.Win.ToolTipImage.Default, "Input Type", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Required", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Operator Editable", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The answer to the question.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The instructions for the process step.", Infragistics.Win.ToolTipImage.Default, "Instructions", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PartProcessAnswers));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.tvwTOC = new Infragistics.Win.UltraWinTree.UltraTree();
            this.grpProcessStepQuestions = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtInputType = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraCheckEditor3 = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.bsProcessQuestion = new System.Windows.Forms.BindingSource(this.components);
            this.dsProcesses = new DWOS.Data.Datasets.ProcessesDataset();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtQuestion = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraCheckEditor2 = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.txtAnswer = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.taProcessQuestion = new DWOS.Data.Datasets.ProcessesDatasetTableAdapters.ProcessQuestionTableAdapter();
            this.taProcessSteps = new DWOS.Data.Datasets.ProcessesDatasetTableAdapters.ProcessStepsTableAdapter();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtProcessName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtPartName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.grpProcessStepInstructions = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtInstructions = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpProcessStepQuestions)).BeginInit();
            this.grpProcessStepQuestions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtInputType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraCheckEditor3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsProcessQuestion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsProcesses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtQuestion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraCheckEditor2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAnswer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtProcessName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpProcessStepInstructions)).BeginInit();
            this.grpProcessStepInstructions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtInstructions)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(708, 519);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(626, 519);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tvwTOC
            // 
            this.tvwTOC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwTOC.HideSelection = false;
            this.tvwTOC.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tvwTOC.Location = new System.Drawing.Point(0, 0);
            this.tvwTOC.Name = "tvwTOC";
            this.tvwTOC.NodeConnectorColor = System.Drawing.SystemColors.ControlDark;
            _override1.HotTracking = Infragistics.Win.DefaultableBoolean.True;
            _override1.NodeDoubleClickAction = Infragistics.Win.UltraWinTree.NodeDoubleClickAction.ToggleExpansion;
            _override1.Sort = Infragistics.Win.UltraWinTree.SortType.Ascending;
            this.tvwTOC.Override = _override1;
            this.tvwTOC.Size = new System.Drawing.Size(182, 511);
            this.tvwTOC.TabIndex = 8;
            ultraToolTipInfo6.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo6.ToolTipTextFormatted");
            ultraToolTipInfo6.ToolTipTitle = "Legend";
            this.ultraToolTipManager1.SetUltraToolTip(this.tvwTOC, ultraToolTipInfo6);
            this.tvwTOC.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.tvwTOC_AfterSelect);
            this.tvwTOC.BeforeSelect += new Infragistics.Win.UltraWinTree.BeforeNodeSelectEventHandler(this.tvwTOC_BeforeSelect);
            // 
            // grpProcessStepQuestions
            // 
            this.grpProcessStepQuestions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpProcessStepQuestions.Controls.Add(this.txtInputType);
            this.grpProcessStepQuestions.Controls.Add(this.ultraCheckEditor3);
            this.grpProcessStepQuestions.Controls.Add(this.ultraLabel1);
            this.grpProcessStepQuestions.Controls.Add(this.txtQuestion);
            this.grpProcessStepQuestions.Controls.Add(this.ultraLabel2);
            this.grpProcessStepQuestions.Controls.Add(this.ultraCheckEditor2);
            this.grpProcessStepQuestions.Controls.Add(this.txtAnswer);
            this.grpProcessStepQuestions.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.grpProcessStepQuestions.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpProcessStepQuestions.Location = new System.Drawing.Point(49, 201);
            this.grpProcessStepQuestions.Name = "grpProcessStepQuestions";
            this.grpProcessStepQuestions.Size = new System.Drawing.Size(502, 139);
            this.grpProcessStepQuestions.TabIndex = 33;
            this.grpProcessStepQuestions.Text = "Step Questions";
            // 
            // txtInputType
            // 
            this.txtInputType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.FontData.ItalicAsString = "True";
            appearance1.TextHAlignAsString = "Right";
            this.txtInputType.Appearance = appearance1;
            this.txtInputType.Location = new System.Drawing.Point(403, 78);
            this.txtInputType.MaxLength = 250;
            this.txtInputType.Name = "txtInputType";
            this.txtInputType.ReadOnly = true;
            this.txtInputType.Size = new System.Drawing.Size(84, 26);
            this.txtInputType.TabIndex = 29;
            this.txtInputType.Text = "String";
            ultraToolTipInfo2.ToolTipText = "The type of question.";
            ultraToolTipInfo2.ToolTipTitle = "Input Type";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtInputType, ultraToolTipInfo2);
            // 
            // ultraCheckEditor3
            // 
            this.ultraCheckEditor3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraCheckEditor3.AutoSize = true;
            this.ultraCheckEditor3.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bsProcessQuestion, "Required", true));
            this.ultraCheckEditor3.Enabled = false;
            this.ultraCheckEditor3.Location = new System.Drawing.Point(364, 106);
            this.ultraCheckEditor3.Name = "ultraCheckEditor3";
            this.ultraCheckEditor3.Size = new System.Drawing.Size(85, 22);
            this.ultraCheckEditor3.TabIndex = 2;
            this.ultraCheckEditor3.Text = "Required";
            ultraToolTipInfo3.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo3.ToolTipTextFormatted");
            ultraToolTipInfo3.ToolTipTitle = "Required";
            this.ultraToolTipManager1.SetUltraToolTip(this.ultraCheckEditor3, ultraToolTipInfo3);
            // 
            // bsProcessQuestion
            // 
            this.bsProcessQuestion.AllowNew = false;
            this.bsProcessQuestion.DataMember = "ProcessQuestion";
            this.bsProcessQuestion.DataSource = this.dsProcesses;
            // 
            // dsProcesses
            // 
            this.dsProcesses.DataSetName = "ProcessesDataset";
            this.dsProcesses.EnforceConstraints = false;
            this.dsProcesses.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(10, 31);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(74, 19);
            this.ultraLabel1.TabIndex = 28;
            this.ultraLabel1.Text = "Question:";
            // 
            // txtQuestion
            // 
            this.txtQuestion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQuestion.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bsProcessQuestion, "Name", true));
            this.txtQuestion.Location = new System.Drawing.Point(88, 27);
            this.txtQuestion.MaxLength = 50;
            this.txtQuestion.Multiline = true;
            this.txtQuestion.Name = "txtQuestion";
            this.txtQuestion.ReadOnly = true;
            this.txtQuestion.Size = new System.Drawing.Size(399, 45);
            this.txtQuestion.TabIndex = 3;
            this.txtQuestion.Text = "Solution Used";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(11, 82);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(64, 19);
            this.ultraLabel2.TabIndex = 26;
            this.ultraLabel2.Text = "Answer:";
            // 
            // ultraCheckEditor2
            // 
            this.ultraCheckEditor2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraCheckEditor2.AutoSize = true;
            this.ultraCheckEditor2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bsProcessQuestion, "OperatorEditable", true));
            this.ultraCheckEditor2.Enabled = false;
            this.ultraCheckEditor2.Location = new System.Drawing.Point(170, 106);
            this.ultraCheckEditor2.Name = "ultraCheckEditor2";
            this.ultraCheckEditor2.Size = new System.Drawing.Size(147, 22);
            this.ultraCheckEditor2.TabIndex = 1;
            this.ultraCheckEditor2.Text = "Operator Editable";
            ultraToolTipInfo4.ToolTipTextFormatted = "If checked, the operator can edit the answer to the question.";
            ultraToolTipInfo4.ToolTipTitle = "Operator Editable";
            this.ultraToolTipManager1.SetUltraToolTip(this.ultraCheckEditor2, ultraToolTipInfo4);
            // 
            // txtAnswer
            // 
            this.txtAnswer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAnswer.Location = new System.Drawing.Point(88, 78);
            this.txtAnswer.MaxLength = 250;
            this.txtAnswer.Name = "txtAnswer";
            this.txtAnswer.Size = new System.Drawing.Size(309, 26);
            this.txtAnswer.TabIndex = 6;
            this.txtAnswer.Text = "Oakite 164 Cleaner";
            ultraToolTipInfo5.ToolTipText = "The answer to the question.";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtAnswer, ultraToolTipInfo5);
            // 
            // taProcessQuestion
            // 
            this.taProcessQuestion.ClearBeforeFill = true;
            // 
            // taProcessSteps
            // 
            this.taProcessSteps.ClearBeforeFill = true;
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox1.Controls.Add(this.txtProcessName);
            this.ultraGroupBox1.Controls.Add(this.txtPartName);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel4);
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(49, 98);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(502, 97);
            this.ultraGroupBox1.TabIndex = 34;
            this.ultraGroupBox1.Text = "Part Process";
            // 
            // txtProcessName
            // 
            this.txtProcessName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProcessName.Location = new System.Drawing.Point(87, 56);
            this.txtProcessName.MaxLength = 50;
            this.txtProcessName.Name = "txtProcessName";
            this.txtProcessName.ReadOnly = true;
            this.txtProcessName.Size = new System.Drawing.Size(403, 26);
            this.txtProcessName.TabIndex = 30;
            // 
            // txtPartName
            // 
            this.txtPartName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPartName.Location = new System.Drawing.Point(87, 27);
            this.txtPartName.MaxLength = 50;
            this.txtPartName.Name = "txtPartName";
            this.txtPartName.ReadOnly = true;
            this.txtPartName.Size = new System.Drawing.Size(403, 26);
            this.txtPartName.TabIndex = 29;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(13, 31);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(40, 19);
            this.ultraLabel3.TabIndex = 28;
            this.ultraLabel3.Text = "Part:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(14, 60);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(65, 19);
            this.ultraLabel4.TabIndex = 26;
            this.ultraLabel4.Text = "Process:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(3, 2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvwTOC);
            this.inboxControlStyler1.SetStyleSettings(this.splitContainer1.Panel1, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.grpProcessStepInstructions);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2.Controls.Add(this.ultraGroupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.grpProcessStepQuestions);
            this.inboxControlStyler1.SetStyleSettings(this.splitContainer1.Panel2, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.splitContainer1.Size = new System.Drawing.Size(792, 511);
            this.splitContainer1.SplitterDistance = 182;
            this.inboxControlStyler1.SetStyleSettings(this.splitContainer1, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.splitContainer1.TabIndex = 36;
            // 
            // grpProcessStepInstructions
            // 
            this.grpProcessStepInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpProcessStepInstructions.Controls.Add(this.txtInstructions);
            this.grpProcessStepInstructions.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.grpProcessStepInstructions.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpProcessStepInstructions.Location = new System.Drawing.Point(49, 346);
            this.grpProcessStepInstructions.Name = "grpProcessStepInstructions";
            this.grpProcessStepInstructions.Size = new System.Drawing.Size(502, 124);
            this.grpProcessStepInstructions.TabIndex = 36;
            this.grpProcessStepInstructions.Text = "Instructions";
            // 
            // txtInstructions
            // 
            this.txtInstructions.AcceptsReturn = true;
            this.txtInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInstructions.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bsProcessQuestion, "Notes", true));
            this.txtInstructions.Location = new System.Drawing.Point(14, 31);
            this.txtInstructions.MaxLength = 1000;
            this.txtInstructions.Multiline = true;
            this.txtInstructions.Name = "txtInstructions";
            this.txtInstructions.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInstructions.Size = new System.Drawing.Size(479, 87);
            this.txtInstructions.TabIndex = 7;
            ultraToolTipInfo1.ToolTipText = "The instructions for the process step.";
            ultraToolTipInfo1.ToolTipTitle = "Instructions";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtInstructions, ultraToolTipInfo1);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.ultraPictureBox1);
            this.panel1.Controls.Add(this.ultraLabel5);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(606, 76);
            this.inboxControlStyler1.SetStyleSettings(this.panel1, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.panel1.TabIndex = 35;
            // 
            // ultraPictureBox1
            // 
            this.ultraPictureBox1.AutoSize = true;
            this.ultraPictureBox1.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox1.DefaultImage = global::DWOS.UI.Properties.Resources.Info_30;
            this.ultraPictureBox1.Location = new System.Drawing.Point(13, 10);
            this.ultraPictureBox1.Name = "ultraPictureBox1";
            this.ultraPictureBox1.Size = new System.Drawing.Size(30, 30);
            this.ultraPictureBox1.TabIndex = 30;
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel5.Location = new System.Drawing.Point(49, 10);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(523, 63);
            this.ultraLabel5.TabIndex = 29;
            this.ultraLabel5.Text = resources.GetString("ultraLabel5.Text");
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // PartProcessAnswers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 562);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(400, 601);
            this.Name = "PartProcessAnswers";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Edit Part Process Answers";
            this.Load += new System.EventHandler(this.PartProcessAnswers_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpProcessStepQuestions)).EndInit();
            this.grpProcessStepQuestions.ResumeLayout(false);
            this.grpProcessStepQuestions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtInputType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraCheckEditor3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsProcessQuestion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsProcesses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtQuestion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraCheckEditor2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAnswer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtProcessName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpProcessStepInstructions)).EndInit();
            this.grpProcessStepInstructions.ResumeLayout(false);
            this.grpProcessStepInstructions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtInstructions)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.UltraWinTree.UltraTree tvwTOC;
		private Infragistics.Win.Misc.UltraGroupBox grpProcessStepQuestions;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor ultraCheckEditor3;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtQuestion;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor ultraCheckEditor2;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtAnswer;
		private DWOS.Data.Datasets.ProcessesDatasetTableAdapters.ProcessQuestionTableAdapter taProcessQuestion;
		private DWOS.Data.Datasets.ProcessesDatasetTableAdapters.ProcessStepsTableAdapter taProcessSteps;
		private DWOS.Data.Datasets.ProcessesDataset dsProcesses;
		private System.Windows.Forms.BindingSource bsProcessQuestion;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
		private Infragistics.Win.Misc.UltraLabel ultraLabel4;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtProcessName;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPartName;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtInputType;
        private System.Windows.Forms.Panel panel1;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.Misc.UltraGroupBox grpProcessStepInstructions;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtInstructions;
    }
}