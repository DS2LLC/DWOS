namespace DWOS.UI.Sales
{
    partial class QuoteProcessPriceWidget
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

            if (disposing && _processPicker != null)
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
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add processes to this quote part.", Infragistics.Win.ToolTipImage.Default, "Add Process", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Delete the selected process from this part.", Infragistics.Win.ToolTipImage.Default, "Delete Process", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuoteProcessPriceWidget));
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Edit price points for this part.", Infragistics.Win.ToolTipImage.Default, "Edit Price Points", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.btnAddProcess = new Infragistics.Win.Misc.UltraButton();
            this.btnDeleteProcess = new Infragistics.Win.Misc.UltraButton();
            this.grdProcesses = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnSettings = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.grdProcesses)).BeginInit();
            this.SuspendLayout();
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // btnAddProcess
            // 
            appearance15.Image = global::DWOS.UI.Properties.Resources.Add_16;
            this.btnAddProcess.Appearance = appearance15;
            this.btnAddProcess.Location = new System.Drawing.Point(0, 0);
            this.btnAddProcess.Name = "btnAddProcess";
            this.btnAddProcess.Size = new System.Drawing.Size(26, 26);
            this.btnAddProcess.TabIndex = 0;
            ultraToolTipInfo4.ToolTipText = "Add processes to this quote part.";
            ultraToolTipInfo4.ToolTipTitle = "Add Process";
            this.tipManager.SetUltraToolTip(this.btnAddProcess, ultraToolTipInfo4);
            this.btnAddProcess.Click += new System.EventHandler(this.btnAddProcess_Click);
            // 
            // btnDeleteProcess
            // 
            appearance14.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            this.btnDeleteProcess.Appearance = appearance14;
            this.btnDeleteProcess.Location = new System.Drawing.Point(0, 32);
            this.btnDeleteProcess.Name = "btnDeleteProcess";
            this.btnDeleteProcess.Size = new System.Drawing.Size(26, 26);
            this.btnDeleteProcess.TabIndex = 1;
            ultraToolTipInfo3.ToolTipText = "Delete the selected process from this part.";
            ultraToolTipInfo3.ToolTipTitle = "Delete Process";
            this.tipManager.SetUltraToolTip(this.btnDeleteProcess, ultraToolTipInfo3);
            this.btnDeleteProcess.Click += new System.EventHandler(this.btnDeleteProcess_Click);
            // 
            // grdProcesses
            // 
            this.grdProcesses.AllowDrop = true;
            this.grdProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.BackColor = System.Drawing.SystemColors.Window;
            appearance2.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdProcesses.DisplayLayout.Appearance = appearance2;
            this.grdProcesses.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
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
            this.grdProcesses.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.grdProcesses.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.grdProcesses.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdProcesses.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance8.BackColor = System.Drawing.SystemColors.Window;
            this.grdProcesses.DisplayLayout.Override.CardAreaAppearance = appearance8;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            appearance9.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdProcesses.DisplayLayout.Override.CellAppearance = appearance9;
            this.grdProcesses.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdProcesses.DisplayLayout.Override.CellPadding = 0;
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
            this.grdProcesses.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdProcesses.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdProcesses.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            this.grdProcesses.DisplayLayout.Override.SummaryFooterCaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance13.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdProcesses.DisplayLayout.Override.TemplateAddRowAppearance = appearance13;
            this.grdProcesses.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdProcesses.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdProcesses.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControlOnLastCell;
            this.grdProcesses.Location = new System.Drawing.Point(32, 0);
            this.grdProcesses.Name = "grdProcesses";
            this.grdProcesses.Size = new System.Drawing.Size(389, 90);
            this.grdProcesses.TabIndex = 3;
            ultraToolTipInfo2.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo2.ToolTipTextFormatted");
            this.tipManager.SetUltraToolTip(this.grdProcesses, ultraToolTipInfo2);
            this.grdProcesses.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdProcesses_AfterCellUpdate);
            this.grdProcesses.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdProcesses_InitializeLayout);
            this.grdProcesses.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdProcesses_InitializeRow);
            this.grdProcesses.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.grdProcesses_AfterSelectChange);
            this.grdProcesses.SelectionDrag += new System.ComponentModel.CancelEventHandler(this.grdProcesses_SelectionDrag);
            this.grdProcesses.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.grdProcesses_BeforeCellUpdate);
            this.grdProcesses.DragDrop += new System.Windows.Forms.DragEventHandler(this.grdProcesses_DragDrop);
            this.grdProcesses.DragOver += new System.Windows.Forms.DragEventHandler(this.grdProcesses_DragOver);
            // 
            // btnSettings
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Settings_32;
            this.btnSettings.Appearance = appearance1;
            this.btnSettings.Location = new System.Drawing.Point(0, 64);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(26, 26);
            this.btnSettings.TabIndex = 2;
            ultraToolTipInfo1.ToolTipText = "Edit price points for this part.";
            ultraToolTipInfo1.ToolTipTitle = "Edit Price Points";
            this.tipManager.SetUltraToolTip(this.btnSettings, ultraToolTipInfo1);
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // QuoteProcessPriceWidget
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.grdProcesses);
            this.Controls.Add(this.btnDeleteProcess);
            this.Controls.Add(this.btnAddProcess);
            this.Name = "QuoteProcessPriceWidget";
            this.Size = new System.Drawing.Size(421, 90);
            ultraToolTipInfo5.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo5.ToolTipTextFormatted");
            this.tipManager.SetUltraToolTip(this, ultraToolTipInfo5);
            ((System.ComponentModel.ISupportInitialize)(this.grdProcesses)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        private Infragistics.Win.Misc.UltraButton btnDeleteProcess;
        private Infragistics.Win.Misc.UltraButton btnAddProcess;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdProcesses;
        private Infragistics.Win.Misc.UltraButton btnSettings;
    }
}
