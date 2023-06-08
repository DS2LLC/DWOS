namespace DWOS.UI.Admin.Processes
{
	partial class ProcessStep
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
			    DisposeMe();
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
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Display on COC", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The order in which the process step will occur within its process.", Infragistics.Win.ToolTipImage.Default, "Process Step Order", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The name of the process step.", Infragistics.Win.ToolTipImage.Default, "Process Step Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The description of the step, including any notes.", Infragistics.Win.ToolTipImage.Default, "Process Step Description", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Deletes the selected condition.", Infragistics.Win.ToolTipImage.Default, "Delete Condition", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add a new condition.", Infragistics.Win.ToolTipImage.Default, "Add Condition", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Edits the selected condition.", Infragistics.Win.ToolTipImage.Default, "Edit Condition", Infragistics.Win.DefaultableBoolean.Default);
            this.chkCOCData = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.numOrder = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.txtName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel10 = new Infragistics.Win.Misc.UltraLabel();
            this.txtDescription = new Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor();
            this.richTextEditorToolbar1 = new DWOS.UI.Utilities.RichTextEditorToolbar();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.docLinkManagerSteps = new DWOS.UI.Documents.DocumentLinkManager();
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            this.tvwConditions = new Infragistics.Win.UltraWinTree.UltraTree();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.btnDeleteCondition = new Infragistics.Win.Misc.UltraButton();
            this.btnAddCondition = new Infragistics.Win.Misc.UltraButton();
            this.pnlOrder = new System.Windows.Forms.Panel();
            this.pnlName = new System.Windows.Forms.Panel();
            this.btnEditCondition = new Infragistics.Win.Misc.UltraButton();
            this.pnlEditCondition = new System.Windows.Forms.Panel();
            this.pnlDeleteCondition = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCOCData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwConditions)).BeginInit();
            this.pnlOrder.SuspendLayout();
            this.pnlName.SuspendLayout();
            this.pnlEditCondition.SuspendLayout();
            this.pnlDeleteCondition.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.pnlDeleteCondition);
            this.grpData.Controls.Add(this.pnlEditCondition);
            this.grpData.Controls.Add(this.btnAddCondition);
            this.grpData.Controls.Add(this.ultraLabel1);
            this.grpData.Controls.Add(this.tvwConditions);
            this.grpData.Controls.Add(this.helpLink1);
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.docLinkManagerSteps);
            this.grpData.Controls.Add(this.richTextEditorToolbar1);
            this.grpData.Controls.Add(this.txtDescription);
            this.grpData.Controls.Add(this.chkCOCData);
            this.grpData.Controls.Add(this.ultraLabel5);
            this.grpData.Controls.Add(this.ultraLabel10);
            this.grpData.Controls.Add(this.ultraLabel8);
            this.grpData.Controls.Add(this.pnlOrder);
            this.grpData.Controls.Add(this.pnlName);
            appearance5.Image = global::DWOS.UI.Properties.Resources.StepOrder;
            this.grpData.HeaderAppearance = appearance5;
            this.grpData.Size = new System.Drawing.Size(455, 423);
            this.grpData.Text = "Process Step";
            this.grpData.Controls.SetChildIndex(this.pnlName, 0);
            this.grpData.Controls.SetChildIndex(this.pnlOrder, 0);
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel8, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel10, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel5, 0);
            this.grpData.Controls.SetChildIndex(this.chkCOCData, 0);
            this.grpData.Controls.SetChildIndex(this.txtDescription, 0);
            this.grpData.Controls.SetChildIndex(this.richTextEditorToolbar1, 0);
            this.grpData.Controls.SetChildIndex(this.docLinkManagerSteps, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.grpData.Controls.SetChildIndex(this.helpLink1, 0);
            this.grpData.Controls.SetChildIndex(this.tvwConditions, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.btnAddCondition, 0);
            this.grpData.Controls.SetChildIndex(this.pnlEditCondition, 0);
            this.grpData.Controls.SetChildIndex(this.pnlDeleteCondition, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(504, -1672);
            // 
            // chkCOCData
            // 
            this.chkCOCData.Location = new System.Drawing.Point(197, 30);
            this.chkCOCData.Name = "chkCOCData";
            this.chkCOCData.Size = new System.Drawing.Size(112, 20);
            this.chkCOCData.TabIndex = 1;
            this.chkCOCData.Text = "Display on COC";
            ultraToolTipInfo5.ToolTipTextFormatted = "If CoC Data is checked then all answers to this step will appear on the COC durin" +
    "g the final inspection.";
            ultraToolTipInfo5.ToolTipTitle = "Display on COC";
            this.tipManager.SetUltraToolTip(this.chkCOCData, ultraToolTipInfo5);
            // 
            // numOrder
            // 
            this.numOrder.Location = new System.Drawing.Point(0, 0);
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
            this.numOrder.TabIndex = 0;
            ultraToolTipInfo6.ToolTipText = "The order in which the process step will occur within its process.";
            ultraToolTipInfo6.ToolTipTitle = "Process Step Order";
            this.tipManager.SetUltraToolTip(this.numOrder, ultraToolTipInfo6);
            this.numOrder.Value = 1D;
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(6, 32);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel5.TabIndex = 40;
            this.ultraLabel5.Text = "Order:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(0, 0);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(236, 22);
            this.txtName.TabIndex = 2;
            this.txtName.Text = "Alkaline Wash";
            ultraToolTipInfo7.ToolTipText = "The name of the process step.";
            ultraToolTipInfo7.ToolTipTitle = "Process Step Name";
            this.tipManager.SetUltraToolTip(this.txtName, ultraToolTipInfo7);
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(6, 290);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(73, 15);
            this.ultraLabel8.TabIndex = 39;
            this.ultraLabel8.Text = "Description:";
            // 
            // ultraLabel10
            // 
            this.ultraLabel10.AutoSize = true;
            this.ultraLabel10.Location = new System.Drawing.Point(6, 60);
            this.ultraLabel10.Name = "ultraLabel10";
            this.ultraLabel10.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel10.TabIndex = 38;
            this.ultraLabel10.Text = "Name:";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.ContextMenuItems = ((Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems)((((((((((((((((((Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Cut | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Copy) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Paste) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Delete) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Undo) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Redo) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.SelectAll) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Font) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Image) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Link) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.LineAlignment) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Paragraph) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Bold) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Italics) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Underline) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.SpellingSuggestions) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Strikeout) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Reserved)));
            this.txtDescription.Location = new System.Drawing.Point(118, 290);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(326, 127);
            this.txtDescription.TabIndex = 9;
            ultraToolTipInfo4.ToolTipText = "The description of the step, including any notes.";
            ultraToolTipInfo4.ToolTipTitle = "Process Step Description";
            this.tipManager.SetUltraToolTip(this.txtDescription, ultraToolTipInfo4);
            this.txtDescription.Value = "";
            // 
            // richTextEditorToolbar1
            // 
            this.richTextEditorToolbar1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextEditorToolbar1.Location = new System.Drawing.Point(118, 258);
            this.richTextEditorToolbar1.Name = "richTextEditorToolbar1";
            this.richTextEditorToolbar1.RichTextEditor = this.txtDescription;
            this.richTextEditorToolbar1.Size = new System.Drawing.Size(297, 26);
            this.richTextEditorToolbar1.TabIndex = 8;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(6, 180);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(101, 15);
            this.ultraLabel2.TabIndex = 48;
            this.ultraLabel2.Text = "Document Links:";
            // 
            // docLinkManagerSteps
            // 
            this.docLinkManagerSteps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.docLinkManagerSteps.BackColor = System.Drawing.Color.Transparent;
            this.docLinkManagerSteps.CurrentRow = null;
            this.docLinkManagerSteps.DocumentLinkTable = null;
            this.docLinkManagerSteps.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.docLinkManagerSteps.LinkType = DWOS.UI.Documents.LinkType.Process;
            this.docLinkManagerSteps.Location = new System.Drawing.Point(118, 180);
            this.docLinkManagerSteps.Name = "docLinkManagerSteps";
            this.docLinkManagerSteps.ParentTable = null;
            this.docLinkManagerSteps.Size = new System.Drawing.Size(326, 72);
            this.docLinkManagerSteps.TabIndex = 7;
            this.docLinkManagerSteps.TableKeyColumn = null;
            // 
            // helpLink1
            // 
            this.helpLink1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = "process_step_group.htm";
            this.helpLink1.Location = new System.Drawing.Point(12, 401);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 49;
            // 
            // tvwConditions
            // 
            appearance4.ImageBackground = global::DWOS.UI.Properties.Resources.Equal_32;
            appearance4.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(0, 0, 32, 32);
            this.tvwConditions.Appearance = appearance4;
            this.tvwConditions.Location = new System.Drawing.Point(153, 84);
            this.tvwConditions.Name = "tvwConditions";
            this.tvwConditions.Size = new System.Drawing.Size(291, 90);
            this.tvwConditions.TabIndex = 6;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 84);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(69, 15);
            this.ultraLabel1.TabIndex = 51;
            this.ultraLabel1.Text = "Conditions:";
            // 
            // btnDeleteCondition
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            this.btnDeleteCondition.Appearance = appearance1;
            this.btnDeleteCondition.AutoSize = true;
            this.btnDeleteCondition.Location = new System.Drawing.Point(0, 0);
            this.btnDeleteCondition.Name = "btnDeleteCondition";
            this.btnDeleteCondition.Size = new System.Drawing.Size(26, 26);
            this.btnDeleteCondition.TabIndex = 5;
            ultraToolTipInfo1.ToolTipText = "Deletes the selected condition.";
            ultraToolTipInfo1.ToolTipTitle = "Delete Condition";
            this.tipManager.SetUltraToolTip(this.btnDeleteCondition, ultraToolTipInfo1);
            // 
            // btnAddCondition
            // 
            appearance3.Image = global::DWOS.UI.Properties.Resources.Add_16;
            this.btnAddCondition.Appearance = appearance3;
            this.btnAddCondition.AutoSize = true;
            this.btnAddCondition.Location = new System.Drawing.Point(121, 84);
            this.btnAddCondition.Name = "btnAddCondition";
            this.btnAddCondition.Size = new System.Drawing.Size(26, 26);
            this.btnAddCondition.TabIndex = 3;
            ultraToolTipInfo3.ToolTipText = "Add a new condition.";
            ultraToolTipInfo3.ToolTipTitle = "Add Condition";
            this.tipManager.SetUltraToolTip(this.btnAddCondition, ultraToolTipInfo3);
            // 
            // pnlOrder
            // 
            this.pnlOrder.Controls.Add(this.numOrder);
            this.pnlOrder.Location = new System.Drawing.Point(118, 28);
            this.pnlOrder.Name = "pnlOrder";
            this.pnlOrder.Size = new System.Drawing.Size(73, 22);
            this.pnlOrder.TabIndex = 52;
            this.pnlOrder.MouseLeave += new System.EventHandler(this.pnlOrder_MouseLeave);
            this.pnlOrder.MouseHover += new System.EventHandler(this.pnlOrder_MouseHover);
            // 
            // pnlName
            // 
            this.pnlName.Controls.Add(this.txtName);
            this.pnlName.Location = new System.Drawing.Point(118, 56);
            this.pnlName.Name = "pnlName";
            this.pnlName.Size = new System.Drawing.Size(236, 22);
            this.pnlName.TabIndex = 53;
            this.pnlName.MouseLeave += new System.EventHandler(this.pnlName_MouseLeave);
            this.pnlName.MouseHover += new System.EventHandler(this.pnlName_MouseHover);
            // 
            // btnEditCondition
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.Edit_16;
            this.btnEditCondition.Appearance = appearance2;
            this.btnEditCondition.Location = new System.Drawing.Point(0, 0);
            this.btnEditCondition.Name = "btnEditCondition";
            this.btnEditCondition.Size = new System.Drawing.Size(26, 26);
            this.btnEditCondition.TabIndex = 4;
            ultraToolTipInfo2.ToolTipText = "Edits the selected condition.";
            ultraToolTipInfo2.ToolTipTitle = "Edit Condition";
            this.tipManager.SetUltraToolTip(this.btnEditCondition, ultraToolTipInfo2);
            // 
            // pnlEditCondition
            // 
            this.pnlEditCondition.Controls.Add(this.btnEditCondition);
            this.pnlEditCondition.Location = new System.Drawing.Point(121, 116);
            this.pnlEditCondition.Name = "pnlEditCondition";
            this.pnlEditCondition.Size = new System.Drawing.Size(26, 26);
            this.pnlEditCondition.TabIndex = 4;
            this.pnlEditCondition.MouseLeave += new System.EventHandler(this.pnlEditCondition_MouseLeave);
            this.pnlEditCondition.MouseHover += new System.EventHandler(this.pnlEditCondition_MouseHover);
            // 
            // pnlDeleteCondition
            // 
            this.pnlDeleteCondition.Controls.Add(this.btnDeleteCondition);
            this.pnlDeleteCondition.Location = new System.Drawing.Point(121, 148);
            this.pnlDeleteCondition.Name = "pnlDeleteCondition";
            this.pnlDeleteCondition.Size = new System.Drawing.Size(26, 26);
            this.pnlDeleteCondition.TabIndex = 5;
            this.pnlDeleteCondition.MouseLeave += new System.EventHandler(this.pnlDeleteCondition_MouseLeave);
            this.pnlDeleteCondition.MouseHover += new System.EventHandler(this.pnlDeleteCondition_MouseHover);
            // 
            // ProcessStep
            // 
            this.Name = "ProcessStep";
            this.Size = new System.Drawing.Size(461, 429);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCOCData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwConditions)).EndInit();
            this.pnlOrder.ResumeLayout(false);
            this.pnlOrder.PerformLayout();
            this.pnlName.ResumeLayout(false);
            this.pnlName.PerformLayout();
            this.pnlEditCondition.ResumeLayout(false);
            this.pnlDeleteCondition.ResumeLayout(false);
            this.pnlDeleteCondition.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkCOCData;
		private Infragistics.Win.UltraWinEditors.UltraNumericEditor numOrder;
		private Infragistics.Win.Misc.UltraLabel ultraLabel5;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtName;
		private Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private Infragistics.Win.Misc.UltraLabel ultraLabel10;
		private Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor txtDescription;
		private Utilities.RichTextEditorToolbar richTextEditorToolbar1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Documents.DocumentLinkManager docLinkManagerSteps;
        private Utilities.HelpLink helpLink1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinTree.UltraTree tvwConditions;
        private Infragistics.Win.Misc.UltraButton btnDeleteCondition;
        private Infragistics.Win.Misc.UltraButton btnAddCondition;
        private System.Windows.Forms.Panel pnlOrder;
        private System.Windows.Forms.Panel pnlName;
        private Infragistics.Win.Misc.UltraButton btnEditCondition;
        private System.Windows.Forms.Panel pnlDeleteCondition;
        private System.Windows.Forms.Panel pnlEditCondition;
    }
}
