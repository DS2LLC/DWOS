namespace DWOS.UI.QA.QIManagerPanels
{
	partial class QIQuestion
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
				_displayTooltipsMain.Dispose();
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
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Maximum Value", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Minimum Value", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Numeric Units", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The order in which the process question will occur within its process step.", Infragistics.Win.ToolTipImage.Default, "Question Order", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The question or statement display to the operator asking for an answer or confirm" +
        "ation that the operator understands the process step.", Infragistics.Win.ToolTipImage.Default, "Step Question", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo10 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Question Answer", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo11 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Question Input Type", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QIQuestion));
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo12 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "List Type", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add a new condition.", Infragistics.Win.ToolTipImage.Default, "Add Condition", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Edits the selected condition.", Infragistics.Win.ToolTipImage.Default, "Edit Condition", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Deletes the selected condition.", Infragistics.Win.ToolTipImage.Default, "Delete Condition", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Define Minumum Sample Size based on quantity thresholds. Enter list of thresholds" +
        " and values in the format Min,samplesize|max,samplesize|max,samplesize Example:(" +
        "1,10,2|11,20,5|21,30,10)", Infragistics.Win.ToolTipImage.Default, "Sample Size Quantity Thresholds", Infragistics.Win.DefaultableBoolean.Default);
            this.numMaxValue = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.numMinValue = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.cboUnits = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel13 = new Infragistics.Win.Misc.UltraLabel();
            this.numOrder = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel16 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.txtName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel15 = new Infragistics.Win.Misc.UltraLabel();
            this.txtValue = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboProcessQuesInputType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel11 = new Infragistics.Win.Misc.UltraLabel();
            this.cboProcessQuesList = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel9 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel12 = new Infragistics.Win.Misc.UltraLabel();
            this.tvwConditions = new Infragistics.Win.UltraWinTree.UltraTree();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.btnAddCondition = new Infragistics.Win.Misc.UltraButton();
            this.btnEditCondition = new Infragistics.Win.Misc.UltraButton();
            this.btnDeleteCondition = new Infragistics.Win.Misc.UltraButton();
            this.txtSampleSize = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUnits)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessQuesInputType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessQuesList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwConditions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSampleSize)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.txtSampleSize);
            this.grpData.Controls.Add(this.btnDeleteCondition);
            this.grpData.Controls.Add(this.btnEditCondition);
            this.grpData.Controls.Add(this.btnAddCondition);
            this.grpData.Controls.Add(this.ultraLabel1);
            this.grpData.Controls.Add(this.tvwConditions);
            this.grpData.Controls.Add(this.ultraLabel16);
            this.grpData.Controls.Add(this.numOrder);
            this.grpData.Controls.Add(this.numMaxValue);
            this.grpData.Controls.Add(this.numMinValue);
            this.grpData.Controls.Add(this.cboUnits);
            this.grpData.Controls.Add(this.ultraLabel13);
            this.grpData.Controls.Add(this.ultraLabel12);
            this.grpData.Controls.Add(this.ultraLabel9);
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.txtName);
            this.grpData.Controls.Add(this.ultraLabel15);
            this.grpData.Controls.Add(this.txtValue);
            this.grpData.Controls.Add(this.cboProcessQuesInputType);
            this.grpData.Controls.Add(this.ultraLabel6);
            this.grpData.Controls.Add(this.ultraLabel11);
            this.grpData.Controls.Add(this.cboProcessQuesList);
            appearance6.Image = global::DWOS.UI.Properties.Resources.Question_16;
            this.grpData.HeaderAppearance = appearance6;
            this.grpData.Size = new System.Drawing.Size(467, 305);
            this.grpData.Text = "Inspection Question";
            this.grpData.Click += new System.EventHandler(this.grpData_Click);
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.cboProcessQuesList, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel11, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel6, 0);
            this.grpData.Controls.SetChildIndex(this.cboProcessQuesInputType, 0);
            this.grpData.Controls.SetChildIndex(this.txtValue, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel15, 0);
            this.grpData.Controls.SetChildIndex(this.txtName, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel9, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel12, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel13, 0);
            this.grpData.Controls.SetChildIndex(this.cboUnits, 0);
            this.grpData.Controls.SetChildIndex(this.numMinValue, 0);
            this.grpData.Controls.SetChildIndex(this.numMaxValue, 0);
            this.grpData.Controls.SetChildIndex(this.numOrder, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel16, 0);
            this.grpData.Controls.SetChildIndex(this.tvwConditions, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.btnAddCondition, 0);
            this.grpData.Controls.SetChildIndex(this.btnEditCondition, 0);
            this.grpData.Controls.SetChildIndex(this.btnDeleteCondition, 0);
            this.grpData.Controls.SetChildIndex(this.txtSampleSize, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(519, -1241);
            // 
            // numMaxValue
            // 
            this.numMaxValue.Location = new System.Drawing.Point(253, 236);
            this.numMaxValue.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.numMaxValue.MaskInput = "nnnnnnnnn";
            this.numMaxValue.Name = "numMaxValue";
            this.numMaxValue.NullText = "0";
            this.numMaxValue.PromptChar = ' ';
            this.numMaxValue.ReadOnly = true;
            this.numMaxValue.Size = new System.Drawing.Size(75, 22);
            this.numMaxValue.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.OnMouseEnter;
            this.numMaxValue.TabIndex = 11;
            ultraToolTipInfo6.ToolTipTextFormatted = "If the Input Type is <span style=\"font-weight:bold;\">Decimal </span>or <span styl" +
    "e=\"font-weight:bold;\">Integer</span>, then this will be the maximum value that t" +
    "he operator is allowed to enter.";
            ultraToolTipInfo6.ToolTipTitle = "Maximum Value";
            this.tipManager.SetUltraToolTip(this.numMaxValue, ultraToolTipInfo6);
            // 
            // numMinValue
            // 
            this.numMinValue.Location = new System.Drawing.Point(98, 236);
            this.numMinValue.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.numMinValue.MaskInput = "nnnnnnnnn";
            this.numMinValue.Name = "numMinValue";
            this.numMinValue.NullText = "0";
            this.numMinValue.PromptChar = ' ';
            this.numMinValue.ReadOnly = true;
            this.numMinValue.Size = new System.Drawing.Size(75, 22);
            this.numMinValue.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.OnMouseEnter;
            this.numMinValue.TabIndex = 10;
            ultraToolTipInfo7.ToolTipTextFormatted = "If the Input Type is <span style=\"font-weight:bold;\">Decimal </span>or <span styl" +
    "e=\"font-weight:bold;\">Integer</span>, then this will be the minimum value that t" +
    "he operator is allowed to enter.";
            ultraToolTipInfo7.ToolTipTitle = "Minimum Value";
            this.tipManager.SetUltraToolTip(this.numMinValue, ultraToolTipInfo7);
            // 
            // cboUnits
            // 
            this.cboUnits.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            this.cboUnits.DropDownListWidth = -1;
            this.cboUnits.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboUnits.Location = new System.Drawing.Point(379, 236);
            this.cboUnits.MaxLength = 50;
            this.cboUnits.Name = "cboUnits";
            this.cboUnits.ReadOnly = true;
            this.cboUnits.Size = new System.Drawing.Size(76, 22);
            this.cboUnits.TabIndex = 12;
            ultraToolTipInfo8.ToolTipTextFormatted = "If the Input Type is <span style=\"font-weight:bold;\">Decimal </span>or <span styl" +
    "e=\"font-weight:bold;\">Integer</span>, then this will be the units of measure tha" +
    "t the numeric input is in.";
            ultraToolTipInfo8.ToolTipTitle = "Numeric Units";
            this.tipManager.SetUltraToolTip(this.cboUnits, ultraToolTipInfo8);
            // 
            // ultraLabel13
            // 
            this.ultraLabel13.AutoSize = true;
            this.ultraLabel13.Location = new System.Drawing.Point(334, 240);
            this.ultraLabel13.Name = "ultraLabel13";
            this.ultraLabel13.Size = new System.Drawing.Size(38, 15);
            this.ultraLabel13.TabIndex = 39;
            this.ultraLabel13.Text = "Units:";
            // 
            // numOrder
            // 
            this.numOrder.Location = new System.Drawing.Point(100, 28);
            this.numOrder.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.numOrder.MaskInput = "nnn.n";
            this.numOrder.MaxValue = 999.9D;
            this.numOrder.MinValue = 0;
            this.numOrder.Name = "numOrder";
            this.numOrder.NullText = "0";
            this.numOrder.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.numOrder.PromptChar = ' ';
            this.numOrder.Size = new System.Drawing.Size(73, 22);
            this.numOrder.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.numOrder.TabIndex = 1;
            ultraToolTipInfo5.ToolTipText = "The order in which the process question will occur within its process step.";
            ultraToolTipInfo5.ToolTipTitle = "Question Order";
            this.tipManager.SetUltraToolTip(this.numOrder, ultraToolTipInfo5);
            this.numOrder.Value = 1D;
            // 
            // ultraLabel16
            // 
            this.ultraLabel16.AutoSize = true;
            this.ultraLabel16.Location = new System.Drawing.Point(6, 32);
            this.ultraLabel16.Name = "ultraLabel16";
            this.ultraLabel16.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel16.TabIndex = 47;
            this.ultraLabel16.Text = "Order:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(6, 60);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(60, 15);
            this.ultraLabel4.TabIndex = 46;
            this.ultraLabel4.Text = "Question:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(99, 56);
            this.txtName.MaxLength = 50;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(356, 22);
            this.txtName.TabIndex = 2;
            ultraToolTipInfo9.ToolTipText = "The question or statement display to the operator asking for an answer or confirm" +
    "ation that the operator understands the process step.";
            ultraToolTipInfo9.ToolTipTitle = "Step Question";
            this.tipManager.SetUltraToolTip(this.txtName, ultraToolTipInfo9);
            // 
            // ultraLabel15
            // 
            this.ultraLabel15.AutoSize = true;
            this.ultraLabel15.Location = new System.Drawing.Point(6, 212);
            this.ultraLabel15.Name = "ultraLabel15";
            this.ultraLabel15.Size = new System.Drawing.Size(51, 15);
            this.ultraLabel15.TabIndex = 45;
            this.ultraLabel15.Text = "Answer:";
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(98, 208);
            this.txtValue.MaxLength = 250;
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(357, 22);
            this.txtValue.TabIndex = 9;
            ultraToolTipInfo10.ToolTipTextFormatted = "The default answer to the question. This allows the answer to be pre-filled out f" +
    "or the operator. (Optional)";
            ultraToolTipInfo10.ToolTipTitle = "Question Answer";
            this.tipManager.SetUltraToolTip(this.txtValue, ultraToolTipInfo10);
            // 
            // cboProcessQuesInputType
            // 
            this.cboProcessQuesInputType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboProcessQuesInputType.Location = new System.Drawing.Point(98, 180);
            this.cboProcessQuesInputType.MaxLength = 50;
            this.cboProcessQuesInputType.Name = "cboProcessQuesInputType";
            this.cboProcessQuesInputType.Size = new System.Drawing.Size(149, 22);
            this.cboProcessQuesInputType.TabIndex = 7;
            ultraToolTipInfo11.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo11.ToolTipTextFormatted");
            ultraToolTipInfo11.ToolTipTitle = "Question Input Type";
            this.tipManager.SetUltraToolTip(this.cboProcessQuesInputType, ultraToolTipInfo11);
            this.cboProcessQuesInputType.ValueChanged += new System.EventHandler(this.cboProcessQuesInputType_ValueChanged);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(6, 184);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(71, 15);
            this.ultraLabel6.TabIndex = 41;
            this.ultraLabel6.Text = "Input Type:";
            // 
            // ultraLabel11
            // 
            this.ultraLabel11.AutoSize = true;
            this.ultraLabel11.Location = new System.Drawing.Point(6, 240);
            this.ultraLabel11.Name = "ultraLabel11";
            this.ultraLabel11.Size = new System.Drawing.Size(65, 15);
            this.ultraLabel11.TabIndex = 42;
            this.ultraLabel11.Text = "Min Value:";
            // 
            // cboProcessQuesList
            // 
            appearance5.Image = global::DWOS.UI.Properties.Resources.Add_16;
            editorButton1.Appearance = appearance5;
            this.cboProcessQuesList.ButtonsLeft.Add(editorButton1);
            this.cboProcessQuesList.DropDownListWidth = -1;
            this.cboProcessQuesList.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboProcessQuesList.Location = new System.Drawing.Point(306, 180);
            this.cboProcessQuesList.Name = "cboProcessQuesList";
            this.cboProcessQuesList.NullText = "<None>";
            this.cboProcessQuesList.ReadOnly = true;
            this.cboProcessQuesList.Size = new System.Drawing.Size(149, 22);
            this.cboProcessQuesList.TabIndex = 8;
            ultraToolTipInfo12.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo12.ToolTipTextFormatted");
            ultraToolTipInfo12.ToolTipTitle = "List Type";
            this.tipManager.SetUltraToolTip(this.cboProcessQuesList, ultraToolTipInfo12);
            this.cboProcessQuesList.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.cboProcessQuesList_EditorButtonClick);
            // 
            // ultraLabel9
            // 
            this.ultraLabel9.AutoSize = true;
            this.ultraLabel9.Location = new System.Drawing.Point(179, 240);
            this.ultraLabel9.Name = "ultraLabel9";
            this.ultraLabel9.Size = new System.Drawing.Size(68, 15);
            this.ultraLabel9.TabIndex = 43;
            this.ultraLabel9.Text = "Max Value:";
            // 
            // ultraLabel12
            // 
            this.ultraLabel12.AutoSize = true;
            this.ultraLabel12.Location = new System.Drawing.Point(271, 184);
            this.ultraLabel12.Name = "ultraLabel12";
            this.ultraLabel12.Size = new System.Drawing.Size(29, 15);
            this.ultraLabel12.TabIndex = 44;
            this.ultraLabel12.Text = "List:";
            // 
            // tvwConditions
            // 
            appearance4.ImageBackground = global::DWOS.UI.Properties.Resources.Equal_32;
            appearance4.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(0, 0, 32, 32);
            this.tvwConditions.Appearance = appearance4;
            this.tvwConditions.Location = new System.Drawing.Point(132, 84);
            this.tvwConditions.Name = "tvwConditions";
            this.tvwConditions.Size = new System.Drawing.Size(323, 90);
            this.tvwConditions.TabIndex = 6;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 84);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(69, 15);
            this.ultraLabel1.TabIndex = 49;
            this.ultraLabel1.Text = "Conditions:";
            // 
            // btnAddCondition
            // 
            appearance3.Image = global::DWOS.UI.Properties.Resources.Add_16;
            this.btnAddCondition.Appearance = appearance3;
            this.btnAddCondition.AutoSize = true;
            this.btnAddCondition.Location = new System.Drawing.Point(100, 84);
            this.btnAddCondition.Name = "btnAddCondition";
            this.btnAddCondition.Size = new System.Drawing.Size(26, 26);
            this.btnAddCondition.TabIndex = 3;
            ultraToolTipInfo4.ToolTipText = "Add a new condition.";
            ultraToolTipInfo4.ToolTipTitle = "Add Condition";
            this.tipManager.SetUltraToolTip(this.btnAddCondition, ultraToolTipInfo4);
            // 
            // btnEditCondition
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.Edit_16;
            this.btnEditCondition.Appearance = appearance2;
            this.btnEditCondition.Location = new System.Drawing.Point(100, 116);
            this.btnEditCondition.Name = "btnEditCondition";
            this.btnEditCondition.Size = new System.Drawing.Size(26, 26);
            this.btnEditCondition.TabIndex = 4;
            ultraToolTipInfo3.ToolTipText = "Edits the selected condition.";
            ultraToolTipInfo3.ToolTipTitle = "Edit Condition";
            this.tipManager.SetUltraToolTip(this.btnEditCondition, ultraToolTipInfo3);
            // 
            // btnDeleteCondition
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            this.btnDeleteCondition.Appearance = appearance1;
            this.btnDeleteCondition.AutoSize = true;
            this.btnDeleteCondition.Location = new System.Drawing.Point(100, 148);
            this.btnDeleteCondition.Name = "btnDeleteCondition";
            this.btnDeleteCondition.Size = new System.Drawing.Size(26, 26);
            this.btnDeleteCondition.TabIndex = 5;
            ultraToolTipInfo2.ToolTipText = "Deletes the selected condition.";
            ultraToolTipInfo2.ToolTipTitle = "Delete Condition";
            this.tipManager.SetUltraToolTip(this.btnDeleteCondition, ultraToolTipInfo2);
            // 
            // txtSampleSize
            // 
            this.txtSampleSize.Enabled = false;
            this.txtSampleSize.Location = new System.Drawing.Point(98, 264);
            this.txtSampleSize.Name = "txtSampleSize";
            this.txtSampleSize.Size = new System.Drawing.Size(357, 22);
            this.txtSampleSize.TabIndex = 50;
            ultraToolTipInfo1.ToolTipText = "Define Minumum Sample Size based on quantity thresholds. Enter list of thresholds" +
    " and values in the format Min,samplesize|max,samplesize|max,samplesize Example:(" +
    "1,10,2|11,20,5|21,30,10)";
            ultraToolTipInfo1.ToolTipTitle = "Sample Size Quantity Thresholds";
            this.tipManager.SetUltraToolTip(this.txtSampleSize, ultraToolTipInfo1);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(6, 268);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(79, 15);
            this.ultraLabel2.TabIndex = 51;
            this.ultraLabel2.Text = "Sample Size:";
            // 
            // QIQuestion
            // 
            this.Name = "QIQuestion";
            this.Size = new System.Drawing.Size(473, 311);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUnits)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessQuesInputType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessQuesList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwConditions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSampleSize)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraNumericEditor numMaxValue;
		private Infragistics.Win.UltraWinEditors.UltraNumericEditor numMinValue;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboUnits;
		private Infragistics.Win.Misc.UltraLabel ultraLabel13;
		private Infragistics.Win.UltraWinEditors.UltraNumericEditor numOrder;
		private Infragistics.Win.Misc.UltraLabel ultraLabel16;
		private Infragistics.Win.Misc.UltraLabel ultraLabel4;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtName;
		private Infragistics.Win.Misc.UltraLabel ultraLabel15;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtValue;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboProcessQuesInputType;
		private Infragistics.Win.Misc.UltraLabel ultraLabel6;
		private Infragistics.Win.Misc.UltraLabel ultraLabel11;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboProcessQuesList;
		private Infragistics.Win.Misc.UltraLabel ultraLabel9;
		private Infragistics.Win.Misc.UltraLabel ultraLabel12;
        private Infragistics.Win.UltraWinTree.UltraTree tvwConditions;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraButton btnAddCondition;
        private Infragistics.Win.Misc.UltraButton btnEditCondition;
        private Infragistics.Win.Misc.UltraButton btnDeleteCondition;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSampleSize;
    }
}
