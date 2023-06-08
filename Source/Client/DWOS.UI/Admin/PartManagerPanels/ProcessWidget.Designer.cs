namespace DWOS.UI.Admin.PartManagerPanels
{
    partial class ProcessWidget
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

            if (_processPicker != null)
            {
                _processPicker.Dispose();
                _processPicker = null;
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add a process to this part.", Infragistics.Win.ToolTipImage.Default, "Add Process", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Delete the selected process from this part.", Infragistics.Win.ToolTipImage.Default, "Delete Process", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Edit the selected processes answers.", Infragistics.Win.ToolTipImage.Default, "Edit Process Answers", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add an existing process package to this part.", Infragistics.Win.ToolTipImage.Default, "Add Process Package", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Save existing processes as a new process package.", Infragistics.Win.ToolTipImage.Default, "Save As Process Package", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Shift process up.", Infragistics.Win.ToolTipImage.Default, "Shift Up", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Shift process down.", Infragistics.Win.ToolTipImage.Default, "Shift Down", Infragistics.Win.DefaultableBoolean.Default);
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
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Processes", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessWidget));
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Edit price points for this part.", Infragistics.Win.ToolTipImage.Default, "Edit Price Points", Infragistics.Win.DefaultableBoolean.Default);
            this.btnAddProcess = new Infragistics.Win.Misc.UltraButton();
            this.btnDeleteProcess = new Infragistics.Win.Misc.UltraButton();
            this.btnEditProcess = new Infragistics.Win.Misc.UltraButton();
            this.btnAddProcessPackage = new Infragistics.Win.Misc.UltraButton();
            this.btnSaveAsProcessPackage = new Infragistics.Win.Misc.UltraButton();
            this.btnUp = new Infragistics.Win.Misc.UltraButton();
            this.btnDown = new Infragistics.Win.Misc.UltraButton();
            this.grdProcess = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.messageBoxProcessNotes = new Infragistics.Win.UltraMessageBox.UltraMessageBoxManager(this.components);
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.btnSettings = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.grdProcess)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAddProcess
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Add_16;
            this.btnAddProcess.Appearance = appearance1;
            this.btnAddProcess.Location = new System.Drawing.Point(3, 3);
            this.btnAddProcess.Name = "btnAddProcess";
            this.btnAddProcess.Size = new System.Drawing.Size(26, 26);
            this.btnAddProcess.TabIndex = 0;
            ultraToolTipInfo9.ToolTipText = "Add a process to this part.";
            ultraToolTipInfo9.ToolTipTitle = "Add Process";
            this.tipManager.SetUltraToolTip(this.btnAddProcess, ultraToolTipInfo9);
            this.btnAddProcess.Click += new System.EventHandler(this.btnAddProcess_Click);
            // 
            // btnDeleteProcess
            // 
            appearance20.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            this.btnDeleteProcess.Appearance = appearance20;
            this.btnDeleteProcess.Location = new System.Drawing.Point(28, 3);
            this.btnDeleteProcess.Name = "btnDeleteProcess";
            this.btnDeleteProcess.Size = new System.Drawing.Size(26, 26);
            this.btnDeleteProcess.TabIndex = 1;
            ultraToolTipInfo8.ToolTipText = "Delete the selected process from this part.";
            ultraToolTipInfo8.ToolTipTitle = "Delete Process";
            this.tipManager.SetUltraToolTip(this.btnDeleteProcess, ultraToolTipInfo8);
            // 
            // btnEditProcess
            // 
            appearance19.Image = global::DWOS.UI.Properties.Resources.Edit_16;
            this.btnEditProcess.Appearance = appearance19;
            this.btnEditProcess.Location = new System.Drawing.Point(53, 3);
            this.btnEditProcess.Name = "btnEditProcess";
            this.btnEditProcess.Size = new System.Drawing.Size(26, 26);
            this.btnEditProcess.TabIndex = 2;
            ultraToolTipInfo7.ToolTipText = "Edit the selected processes answers.";
            ultraToolTipInfo7.ToolTipTitle = "Edit Process Answers";
            this.tipManager.SetUltraToolTip(this.btnEditProcess, ultraToolTipInfo7);
            // 
            // btnAddProcessPackage
            // 
            appearance18.Image = global::DWOS.UI.Properties.Resources.Package_16;
            this.btnAddProcessPackage.Appearance = appearance18;
            this.btnAddProcessPackage.Location = new System.Drawing.Point(78, 3);
            this.btnAddProcessPackage.Name = "btnAddProcessPackage";
            this.btnAddProcessPackage.Size = new System.Drawing.Size(26, 26);
            this.btnAddProcessPackage.TabIndex = 3;
            ultraToolTipInfo6.ToolTipText = "Add an existing process package to this part.";
            ultraToolTipInfo6.ToolTipTitle = "Add Process Package";
            this.tipManager.SetUltraToolTip(this.btnAddProcessPackage, ultraToolTipInfo6);
            this.btnAddProcessPackage.Click += new System.EventHandler(this.btnAddProcessPackage_Click);
            // 
            // btnSaveAsProcessPackage
            // 
            appearance17.Image = global::DWOS.UI.Properties.Resources.Save_All;
            this.btnSaveAsProcessPackage.Appearance = appearance17;
            this.btnSaveAsProcessPackage.Location = new System.Drawing.Point(103, 3);
            this.btnSaveAsProcessPackage.Name = "btnSaveAsProcessPackage";
            this.btnSaveAsProcessPackage.Size = new System.Drawing.Size(26, 26);
            this.btnSaveAsProcessPackage.TabIndex = 4;
            ultraToolTipInfo5.ToolTipText = "Save existing processes as a new process package.";
            ultraToolTipInfo5.ToolTipTitle = "Save As Process Package";
            this.tipManager.SetUltraToolTip(this.btnSaveAsProcessPackage, ultraToolTipInfo5);
            this.btnSaveAsProcessPackage.Click += new System.EventHandler(this.btnSaveAsProcessPackage_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance16.Image = global::DWOS.UI.Properties.Resources.Arrow_Up;
            this.btnUp.Appearance = appearance16;
            this.btnUp.Location = new System.Drawing.Point(343, 32);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(26, 26);
            this.btnUp.TabIndex = 6;
            ultraToolTipInfo4.ToolTipText = "Shift process up.";
            ultraToolTipInfo4.ToolTipTitle = "Shift Up";
            this.tipManager.SetUltraToolTip(this.btnUp, ultraToolTipInfo4);
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance15.Image = global::DWOS.UI.Properties.Resources.Arrow_Down;
            this.btnDown.Appearance = appearance15;
            this.btnDown.Location = new System.Drawing.Point(343, 59);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(26, 26);
            this.btnDown.TabIndex = 7;
            ultraToolTipInfo3.ToolTipText = "Shift process down.";
            ultraToolTipInfo3.ToolTipTitle = "Shift Down";
            this.tipManager.SetUltraToolTip(this.btnDown, ultraToolTipInfo3);
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // grdProcess
            // 
            this.grdProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance3.BackColor = System.Drawing.SystemColors.Window;
            appearance3.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdProcess.DisplayLayout.Appearance = appearance3;
            this.grdProcess.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            this.grdProcess.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdProcess.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance4.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance4.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance4.BorderColor = System.Drawing.SystemColors.Window;
            this.grdProcess.DisplayLayout.GroupByBox.Appearance = appearance4;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdProcess.DisplayLayout.GroupByBox.BandLabelAppearance = appearance5;
            this.grdProcess.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance6.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance6.BackColor2 = System.Drawing.SystemColors.Control;
            appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance6.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdProcess.DisplayLayout.GroupByBox.PromptAppearance = appearance6;
            this.grdProcess.DisplayLayout.MaxColScrollRegions = 1;
            this.grdProcess.DisplayLayout.MaxRowScrollRegions = 1;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            appearance7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdProcess.DisplayLayout.Override.ActiveCellAppearance = appearance7;
            appearance8.BackColor = System.Drawing.SystemColors.Highlight;
            appearance8.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdProcess.DisplayLayout.Override.ActiveRowAppearance = appearance8;
            this.grdProcess.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdProcess.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdProcess.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.grdProcess.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdProcess.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance9.BackColor = System.Drawing.SystemColors.Window;
            this.grdProcess.DisplayLayout.Override.CardAreaAppearance = appearance9;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            appearance10.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdProcess.DisplayLayout.Override.CellAppearance = appearance10;
            this.grdProcess.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdProcess.DisplayLayout.Override.CellPadding = 0;
            appearance11.BackColor = System.Drawing.SystemColors.Control;
            appearance11.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance11.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance11.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance11.BorderColor = System.Drawing.SystemColors.Window;
            this.grdProcess.DisplayLayout.Override.GroupByRowAppearance = appearance11;
            appearance12.TextHAlignAsString = "Left";
            this.grdProcess.DisplayLayout.Override.HeaderAppearance = appearance12;
            this.grdProcess.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.Select;
            this.grdProcess.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance13.BackColor = System.Drawing.SystemColors.Window;
            appearance13.BorderColor = System.Drawing.Color.Silver;
            this.grdProcess.DisplayLayout.Override.RowAppearance = appearance13;
            this.grdProcess.DisplayLayout.Override.RowSelectorHeaderStyle = Infragistics.Win.UltraWinGrid.RowSelectorHeaderStyle.ColumnChooserButton;
            this.grdProcess.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdProcess.DisplayLayout.Override.SummaryFooterCaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance14.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdProcess.DisplayLayout.Override.TemplateAddRowAppearance = appearance14;
            this.grdProcess.DisplayLayout.Override.WrapHeaderText = Infragistics.Win.DefaultableBoolean.True;
            this.grdProcess.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdProcess.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdProcess.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControlOnLastCell;
            this.grdProcess.Location = new System.Drawing.Point(3, 32);
            this.grdProcess.Name = "grdProcess";
            this.grdProcess.Size = new System.Drawing.Size(334, 181);
            this.grdProcess.TabIndex = 8;
            ultraToolTipInfo2.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo2.ToolTipTextFormatted");
            ultraToolTipInfo2.ToolTipTitle = "Processes";
            this.tipManager.SetUltraToolTip(this.grdProcess, ultraToolTipInfo2);
            this.grdProcess.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdProcess_AfterCellUpdate);
            this.grdProcess.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdProcess_InitializeLayout);
            this.grdProcess.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdProcess_InitializeRow);
            this.grdProcess.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.grdProcess_BeforeCellUpdate);
            // 
            // messageBoxProcessNotes
            // 
            this.messageBoxProcessNotes.ContainingControl = this;
            // 
            // tipManager
            // 
            this.tipManager.AutoPopDelay = 10000;
            this.tipManager.ContainingControl = this;
            // 
            // btnSettings
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.Settings_32;
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.btnSettings.Appearance = appearance2;
            this.btnSettings.Location = new System.Drawing.Point(128, 3);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(26, 26);
            this.btnSettings.TabIndex = 5;
            ultraToolTipInfo1.ToolTipText = "Edit price points for this part.";
            ultraToolTipInfo1.ToolTipTitle = "Edit Price Points";
            this.tipManager.SetUltraToolTip(this.btnSettings, ultraToolTipInfo1);
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // ProcessWidget
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.grdProcess);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnSaveAsProcessPackage);
            this.Controls.Add(this.btnAddProcessPackage);
            this.Controls.Add(this.btnEditProcess);
            this.Controls.Add(this.btnDeleteProcess);
            this.Controls.Add(this.btnAddProcess);
            this.Name = "ProcessWidget";
            this.Size = new System.Drawing.Size(372, 216);
            ((System.ComponentModel.ISupportInitialize)(this.grdProcess)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnAddProcess;
        private Infragistics.Win.Misc.UltraButton btnDeleteProcess;
        private Infragistics.Win.Misc.UltraButton btnEditProcess;
        private Infragistics.Win.Misc.UltraButton btnAddProcessPackage;
        private Infragistics.Win.Misc.UltraButton btnSaveAsProcessPackage;
        private Infragistics.Win.Misc.UltraButton btnUp;
        private Infragistics.Win.Misc.UltraButton btnDown;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdProcess;
        private Infragistics.Win.UltraMessageBox.UltraMessageBoxManager messageBoxProcessNotes;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        private Infragistics.Win.Misc.UltraButton btnSettings;
    }
}
