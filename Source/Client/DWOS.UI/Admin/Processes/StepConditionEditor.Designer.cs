namespace DWOS.UI.Admin.Processes
{
    partial class StepConditionEditor
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Condition Type", Infragistics.Win.ToolTipImage.Default, "Condition Type", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Process Step", Infragistics.Win.ToolTipImage.Default, "Process Step", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Process Question", Infragistics.Win.ToolTipImage.Default, "Process Question", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The operator to use for comparison.", Infragistics.Win.ToolTipImage.Default, "Operator", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The name of the step.", Infragistics.Win.ToolTipImage.Default, "Step", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The value to use for comparison.", Infragistics.Win.ToolTipImage.Default, "Value", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Summary of the condition.", Infragistics.Win.ToolTipImage.Default, "Condition", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StepConditionEditor));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboConditionType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboProcessStep = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.cboProcessQuestion = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.lblValue = new Infragistics.Win.Misc.UltraLabel();
            this.cboOperator = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.txtStepName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.picInputType = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.smartBox = new DWOS.UI.Utilities.SmartBox();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.lblConditionText = new Infragistics.Win.Misc.UltraLabel();
            this.errProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.cboConditionType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessQuestion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOperator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStepName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(326, 345);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 25);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(218, 345);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 25);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(16, 35);
            this.ultraLabel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(103, 17);
            this.ultraLabel1.TabIndex = 2;
            this.ultraLabel1.Text = "Condition Type:";
            // 
            // cboConditionType
            // 
            this.cboConditionType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboConditionType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboConditionType.Location = new System.Drawing.Point(151, 31);
            this.cboConditionType.MaxLength = 50;
            this.cboConditionType.Name = "cboConditionType";
            this.cboConditionType.Size = new System.Drawing.Size(257, 24);
            this.cboConditionType.TabIndex = 35;
            ultraToolTipInfo7.ToolTipText = "Condition Type";
            ultraToolTipInfo7.ToolTipTitle = "Condition Type";
            this.ultraToolTipManager.SetUltraToolTip(this.cboConditionType, ultraToolTipInfo7);
            this.cboConditionType.ValueChanged += new System.EventHandler(this.cboConditionType_ValueChanged);
            // 
            // cboProcessStep
            // 
            this.cboProcessStep.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::DWOS.UI.Properties.Resources.StepOrder;
            this.cboProcessStep.Appearance = appearance1;
            this.cboProcessStep.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboProcessStep.Location = new System.Drawing.Point(151, 64);
            this.cboProcessStep.MaxLength = 50;
            this.cboProcessStep.Name = "cboProcessStep";
            this.cboProcessStep.Size = new System.Drawing.Size(257, 24);
            this.cboProcessStep.TabIndex = 37;
            ultraToolTipInfo5.ToolTipText = "Process Step";
            ultraToolTipInfo5.ToolTipTitle = "Process Step";
            this.ultraToolTipManager.SetUltraToolTip(this.cboProcessStep, ultraToolTipInfo5);
            this.cboProcessStep.ValueChanged += new System.EventHandler(this.cboProcessStep_ValueChanged);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(16, 68);
            this.ultraLabel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(90, 17);
            this.ultraLabel2.TabIndex = 36;
            this.ultraLabel2.Text = "Process Step:";
            // 
            // cboProcessQuestion
            // 
            this.cboProcessQuestion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.Image = global::DWOS.UI.Properties.Resources.Question_16;
            this.cboProcessQuestion.Appearance = appearance2;
            this.cboProcessQuestion.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboProcessQuestion.Location = new System.Drawing.Point(151, 94);
            this.cboProcessQuestion.MaxLength = 50;
            this.cboProcessQuestion.Name = "cboProcessQuestion";
            this.cboProcessQuestion.Size = new System.Drawing.Size(257, 24);
            this.cboProcessQuestion.TabIndex = 39;
            ultraToolTipInfo6.ToolTipText = "Process Question";
            ultraToolTipInfo6.ToolTipTitle = "Process Question";
            this.ultraToolTipManager.SetUltraToolTip(this.cboProcessQuestion, ultraToolTipInfo6);
            this.cboProcessQuestion.ValueChanged += new System.EventHandler(this.cboProcessQuestion_ValueChanged);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(16, 98);
            this.ultraLabel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(116, 17);
            this.ultraLabel3.TabIndex = 38;
            this.ultraLabel3.Text = "Process Question:";
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(16, 95);
            this.lblValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(45, 17);
            this.lblValue.TabIndex = 40;
            this.lblValue.Text = "Value:";
            // 
            // cboOperator
            // 
            this.cboOperator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboOperator.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboOperator.Location = new System.Drawing.Point(151, 61);
            this.cboOperator.MaxLength = 50;
            this.cboOperator.Name = "cboOperator";
            this.cboOperator.Size = new System.Drawing.Size(257, 24);
            this.cboOperator.TabIndex = 42;
            ultraToolTipInfo3.ToolTipText = "The operator to use for comparison.";
            ultraToolTipInfo3.ToolTipTitle = "Operator";
            this.ultraToolTipManager.SetUltraToolTip(this.cboOperator, ultraToolTipInfo3);
            this.cboOperator.ValueChanged += new System.EventHandler(this.cboOperator_ValueChanged);
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(16, 65);
            this.ultraLabel5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(65, 17);
            this.ultraLabel5.TabIndex = 41;
            this.ultraLabel5.Text = "Operator:";
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(16, 35);
            this.ultraLabel6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(38, 17);
            this.ultraLabel6.TabIndex = 44;
            this.ultraLabel6.Text = "Step:";
            // 
            // txtStepName
            // 
            this.txtStepName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStepName.Location = new System.Drawing.Point(151, 31);
            this.txtStepName.Name = "txtStepName";
            this.txtStepName.ReadOnly = true;
            this.txtStepName.Size = new System.Drawing.Size(257, 24);
            this.txtStepName.TabIndex = 43;
            ultraToolTipInfo4.ToolTipText = "The name of the step.";
            ultraToolTipInfo4.ToolTipTitle = "Step";
            this.ultraToolTipManager.SetUltraToolTip(this.txtStepName, ultraToolTipInfo4);
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox1.Controls.Add(this.cboConditionType);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Controls.Add(this.cboProcessStep);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox1.Controls.Add(this.cboProcessQuestion);
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(5, 12);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(426, 129);
            this.ultraGroupBox1.TabIndex = 45;
            this.ultraGroupBox1.Text = "Input";
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox2.Controls.Add(this.picInputType);
            this.ultraGroupBox2.Controls.Add(this.smartBox);
            this.ultraGroupBox2.Controls.Add(this.cboOperator);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel6);
            this.ultraGroupBox2.Controls.Add(this.lblValue);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel5);
            this.ultraGroupBox2.Controls.Add(this.txtStepName);
            this.ultraGroupBox2.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox2.Location = new System.Drawing.Point(5, 147);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(426, 123);
            this.ultraGroupBox2.TabIndex = 46;
            this.ultraGroupBox2.Text = "Comparison";
            // 
            // picInputType
            // 
            this.picInputType.BorderShadowColor = System.Drawing.Color.Empty;
            this.picInputType.Location = new System.Drawing.Point(124, 92);
            this.picInputType.Name = "picInputType";
            this.picInputType.Size = new System.Drawing.Size(20, 20);
            this.picInputType.TabIndex = 46;
            // 
            // smartBox
            // 
            this.smartBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.smartBox.Location = new System.Drawing.Point(151, 89);
            this.smartBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.smartBox.Name = "smartBox";
            this.smartBox.Size = new System.Drawing.Size(257, 28);
            this.smartBox.TabIndex = 45;
            ultraToolTipInfo2.ToolTipText = "The value to use for comparison.";
            ultraToolTipInfo2.ToolTipTitle = "Value";
            this.ultraToolTipManager.SetUltraToolTip(this.smartBox, ultraToolTipInfo2);
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(13, 276);
            this.ultraLabel7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(69, 17);
            this.ultraLabel7.TabIndex = 43;
            this.ultraLabel7.Text = "Condition:";
            // 
            // lblConditionText
            // 
            this.lblConditionText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConditionText.BorderStyleOuter = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.lblConditionText.Location = new System.Drawing.Point(36, 299);
            this.lblConditionText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lblConditionText.Name = "lblConditionText";
            this.lblConditionText.Size = new System.Drawing.Size(377, 40);
            this.lblConditionText.TabIndex = 43;
            this.lblConditionText.Text = "None...";
            ultraToolTipInfo1.ToolTipText = "Summary of the condition.";
            ultraToolTipInfo1.ToolTipTitle = "Condition";
            this.ultraToolTipManager.SetUltraToolTip(this.lblConditionText, ultraToolTipInfo1);
            // 
            // errProvider
            // 
            this.errProvider.ContainerControl = this;
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // StepConditionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(442, 383);
            this.Controls.Add(this.lblConditionText);
            this.Controls.Add(this.ultraLabel7);
            this.Controls.Add(this.ultraGroupBox2);
            this.Controls.Add(this.ultraGroupBox1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StepConditionEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Step Condition";
            ((System.ComponentModel.ISupportInitialize)(this.cboConditionType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessQuestion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOperator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStepName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboConditionType;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboProcessStep;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboProcessQuestion;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel lblValue;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboOperator;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtStepName;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private Infragistics.Win.Misc.UltraLabel lblConditionText;
        protected System.Windows.Forms.ErrorProvider errProvider;
        private Utilities.SmartBox smartBox;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picInputType;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
    }
}