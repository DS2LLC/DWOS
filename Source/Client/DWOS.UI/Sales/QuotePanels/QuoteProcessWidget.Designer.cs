namespace DWOS.UI.Sales
{
    partial class QuoteProcessWidget
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add processes to this quote part.", Infragistics.Win.ToolTipImage.Default, "Add Process", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Delete the selected process from this part.", Infragistics.Win.ToolTipImage.Default, "Delete Process", Infragistics.Win.DefaultableBoolean.Default);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuoteProcessPriceWidget));
            this.btnAddProcess = new Infragistics.Win.Misc.UltraButton();
            this.btnDeleteProcess = new Infragistics.Win.Misc.UltraButton();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.grdPartProcesses = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraGridBagLayoutManager1 = new Infragistics.Win.Misc.UltraGridBagLayoutManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grdPartProcesses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAddProcess
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Add_16;
            this.btnAddProcess.Appearance = appearance1;
            this.btnAddProcess.Location = new System.Drawing.Point(0, 0);
            this.btnAddProcess.Name = "btnAddProcess";
            this.btnAddProcess.Size = new System.Drawing.Size(26, 26);
            this.btnAddProcess.TabIndex = 0;
            ultraToolTipInfo2.ToolTipText = "Add processes to this quote part.";
            ultraToolTipInfo2.ToolTipTitle = "Add Process";
            this.tipManager.SetUltraToolTip(this.btnAddProcess, ultraToolTipInfo2);
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
            ultraToolTipInfo1.ToolTipText = "Delete the selected process from this part.";
            ultraToolTipInfo1.ToolTipTitle = "Delete Process";
            this.tipManager.SetUltraToolTip(this.btnDeleteProcess, ultraToolTipInfo1);
            this.btnDeleteProcess.Click += new System.EventHandler(this.btnDeleteProcess_Click);
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // grdPartProcesses
            // 
            this.grdPartProcesses.AllowDrop = true;
            this.grdPartProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.BackColor = System.Drawing.SystemColors.Window;
            appearance2.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdPartProcesses.DisplayLayout.Appearance = appearance2;
            this.grdPartProcesses.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdPartProcesses.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance3.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance3.BorderColor = System.Drawing.SystemColors.Window;
            this.grdPartProcesses.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdPartProcesses.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.grdPartProcesses.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdPartProcesses.DisplayLayout.GroupByBox.Hidden = true;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdPartProcesses.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.grdPartProcesses.DisplayLayout.MaxColScrollRegions = 1;
            this.grdPartProcesses.DisplayLayout.MaxRowScrollRegions = 1;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            appearance6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdPartProcesses.DisplayLayout.Override.ActiveCellAppearance = appearance6;
            appearance7.BackColor = System.Drawing.SystemColors.Highlight;
            appearance7.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdPartProcesses.DisplayLayout.Override.ActiveRowAppearance = appearance7;
            this.grdPartProcesses.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdPartProcesses.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance8.BackColor = System.Drawing.SystemColors.Window;
            this.grdPartProcesses.DisplayLayout.Override.CardAreaAppearance = appearance8;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            appearance9.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdPartProcesses.DisplayLayout.Override.CellAppearance = appearance9;
            this.grdPartProcesses.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdPartProcesses.DisplayLayout.Override.CellPadding = 0;
            appearance10.BackColor = System.Drawing.SystemColors.Control;
            appearance10.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance10.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance10.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance10.BorderColor = System.Drawing.SystemColors.Window;
            this.grdPartProcesses.DisplayLayout.Override.GroupByRowAppearance = appearance10;
            appearance11.TextHAlignAsString = "Left";
            this.grdPartProcesses.DisplayLayout.Override.HeaderAppearance = appearance11;
            this.grdPartProcesses.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdPartProcesses.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance12.BackColor = System.Drawing.SystemColors.Window;
            appearance12.BorderColor = System.Drawing.Color.Silver;
            this.grdPartProcesses.DisplayLayout.Override.RowAppearance = appearance12;
            this.grdPartProcesses.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance13.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdPartProcesses.DisplayLayout.Override.TemplateAddRowAppearance = appearance13;
            this.grdPartProcesses.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdPartProcesses.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdPartProcesses.Location = new System.Drawing.Point(32, 0);
            this.grdPartProcesses.Name = "grdPartProcesses";
            this.grdPartProcesses.Size = new System.Drawing.Size(166, 90);
            this.grdPartProcesses.TabIndex = 3;
            ultraToolTipInfo2.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo2.ToolTipTextFormatted");
            this.tipManager.SetUltraToolTip(this.grdPartProcesses, ultraToolTipInfo2);
            this.grdPartProcesses.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdPartProcesses_AfterCellUpdate);
            this.grdPartProcesses.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdPartProcesses_InitializeLayout);
            this.grdPartProcesses.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdPartProcesses_InitializeRow);
            this.grdPartProcesses.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.grdPartProcesses_AfterSelectChange);
            this.grdPartProcesses.SelectionDrag += new System.ComponentModel.CancelEventHandler(this.grdPartProcesses_SelectionDrag);
            this.grdPartProcesses.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.grdPartProcesses_BeforeCellUpdate);
            this.grdPartProcesses.DragDrop += new System.Windows.Forms.DragEventHandler(this.grdPartProcesses_DragDrop);
            this.grdPartProcesses.DragOver += new System.Windows.Forms.DragEventHandler(this.grdPartProcesses_DragOver);

            //this.grdPartProcesses.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdPartProcesses_InitializeLayout);
            // 
            // QuoteProcessWidget
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.grdPartProcesses);
            this.Controls.Add(this.btnDeleteProcess);
            this.Controls.Add(this.btnAddProcess);
            this.Name = "QuoteProcessWidget";
            this.Size = new System.Drawing.Size(198, 90);
            ((System.ComponentModel.ISupportInitialize)(this.grdPartProcesses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutManager1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnAddProcess;
        private Infragistics.Win.Misc.UltraButton btnDeleteProcess;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        //private Infragistics.Win.UltraWinTree.UltraTree tvwProcesses;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdPartProcesses;
        private Infragistics.Win.Misc.UltraGridBagLayoutManager ultraGridBagLayoutManager1;
    }
}
