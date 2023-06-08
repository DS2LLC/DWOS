namespace DWOS.UI.Sales.Order
{
    partial class AdvancedSerialNumberWidget
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
            _displayDisabledTooltips.Dispose();
            _displayDisabledTooltips = null;
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Number");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Row");
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Adds a new serial number for this order.", Infragistics.Win.ToolTipImage.Default, "Add Serial Number", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Deletes all selected serial numbers.", Infragistics.Win.ToolTipImage.Default, "Delete Serial Numbers", Infragistics.Win.DefaultableBoolean.Default);
            this.grdSerialNumbers = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnAdd = new Infragistics.Win.Misc.UltraButton();
            this.btnDelete = new Infragistics.Win.Misc.UltraButton();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grdSerialNumbers)).BeginInit();
            this.SuspendLayout();
            // 
            // grdSerialNumbers
            // 
            this.grdSerialNumbers.AllowDrop = true;
            this.grdSerialNumbers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdSerialNumbers.DisplayLayout.Appearance = appearance1;
            this.grdSerialNumbers.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridBand1.AddButtonCaption = "Serial Number";
            ultraGridColumn1.AutoEditMode = Infragistics.Win.DefaultableBoolean.True;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.MaxLength = 20;
            ultraGridColumn1.Width = 264;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Hidden = true;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2});
            ultraGridBand1.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdSerialNumbers.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdSerialNumbers.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdSerialNumbers.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.grdSerialNumbers.DisplayLayout.EmptyRowSettings.ShowEmptyRows = true;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.grdSerialNumbers.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdSerialNumbers.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.grdSerialNumbers.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdSerialNumbers.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.grdSerialNumbers.DisplayLayout.MaxColScrollRegions = 1;
            this.grdSerialNumbers.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdSerialNumbers.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdSerialNumbers.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grdSerialNumbers.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            this.grdSerialNumbers.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.grdSerialNumbers.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.None;
            this.grdSerialNumbers.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.grdSerialNumbers.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.grdSerialNumbers.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdSerialNumbers.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdSerialNumbers.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.grdSerialNumbers.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdSerialNumbers.DisplayLayout.Override.CellAppearance = appearance8;
            this.grdSerialNumbers.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdSerialNumbers.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.grdSerialNumbers.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.grdSerialNumbers.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.grdSerialNumbers.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.Select;
            this.grdSerialNumbers.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.grdSerialNumbers.DisplayLayout.Override.RowAppearance = appearance11;
            this.grdSerialNumbers.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdSerialNumbers.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            this.grdSerialNumbers.DisplayLayout.Override.SummaryFooterCaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdSerialNumbers.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.grdSerialNumbers.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdSerialNumbers.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdSerialNumbers.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControl;
            this.grdSerialNumbers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdSerialNumbers.Location = new System.Drawing.Point(32, 0);
            this.grdSerialNumbers.Name = "grdSerialNumbers";
            this.grdSerialNumbers.Size = new System.Drawing.Size(302, 114);
            this.grdSerialNumbers.TabIndex = 2;
            this.grdSerialNumbers.Text = "ultraGrid1";
            this.grdSerialNumbers.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdSerialNumbers_InitializeLayout);
            this.grdSerialNumbers.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.grdSerialNumbers_AfterSelectChange);
            this.grdSerialNumbers.SelectionDrag += new System.ComponentModel.CancelEventHandler(this.grdSerialNumbers_SelectionDrag);
            this.grdSerialNumbers.DoubleClickCell += new Infragistics.Win.UltraWinGrid.DoubleClickCellEventHandler(this.grdSerialNumbers_DoubleClickCell);
            this.grdSerialNumbers.DragDrop += new System.Windows.Forms.DragEventHandler(this.grdSerialNumbers_DragDrop);
            this.grdSerialNumbers.DragOver += new System.Windows.Forms.DragEventHandler(this.grdSerialNumbers_DragOver);
            this.grdSerialNumbers.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdSerialNumbers_KeyDown);
            // 
            // btnAdd
            // 
            appearance13.Image = global::DWOS.UI.Properties.Resources.Add_16;
            this.btnAdd.Appearance = appearance13;
            this.btnAdd.Location = new System.Drawing.Point(0, 0);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(26, 26);
            this.btnAdd.TabIndex = 0;
            ultraToolTipInfo2.ToolTipText = "Adds a new serial number for this order.";
            ultraToolTipInfo2.ToolTipTitle = "Add Serial Number";
            this.ultraToolTipManager.SetUltraToolTip(this.btnAdd, ultraToolTipInfo2);
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            appearance14.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            this.btnDelete.Appearance = appearance14;
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(0, 32);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(26, 26);
            this.btnDelete.TabIndex = 1;
            ultraToolTipInfo1.ToolTipText = "Deletes all selected serial numbers.";
            ultraToolTipInfo1.ToolTipTitle = "Delete Serial Numbers";
            this.ultraToolTipManager.SetUltraToolTip(this.btnDelete, ultraToolTipInfo1);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // AdvancedSerialNumberWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.grdSerialNumbers);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "AdvancedSerialNumberWidget";
            this.Size = new System.Drawing.Size(334, 114);
            ((System.ComponentModel.ISupportInitialize)(this.grdSerialNumbers)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid grdSerialNumbers;
        private Infragistics.Win.Misc.UltraButton btnAdd;
        private Infragistics.Win.Misc.UltraButton btnDelete;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
    }
}
