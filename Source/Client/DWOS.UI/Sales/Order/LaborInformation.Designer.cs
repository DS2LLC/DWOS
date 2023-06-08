namespace DWOS.UI.Sales.Order
{
    partial class LaborInformation
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StartTime");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EndTime");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Process", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsBatched", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Duration", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Quantity", 2);
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The total labor time spent processing this work order.", Infragistics.Win.ToolTipImage.Default, "Total Labor Time (Processing)", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The total labor time spent inspecting this work order.", Infragistics.Win.ToolTipImage.Default, "Total Labor Time (Inspection)", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The total labor time for this work order.", Infragistics.Win.ToolTipImage.Default, "Total Labor Time", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The total labor time spent performing miscellaneous activities on this work order" +
        ".", Infragistics.Win.ToolTipImage.Default, "Total Labor Time (Miscellaneous)", Infragistics.Win.DefaultableBoolean.Default);
            this.grdLabor = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtTotalProcess = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtTotalInspection = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtTotalAll = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtTotalOther = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdLabor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalProcess)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalInspection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalOther)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.txtTotalOther);
            this.grpData.Controls.Add(this.ultraLabel3);
            this.grpData.Controls.Add(this.txtTotalAll);
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.txtTotalInspection);
            this.grpData.Controls.Add(this.txtTotalProcess);
            this.grpData.Controls.Add(this.ultraLabel1);
            this.grpData.Controls.Add(this.grdLabor);
            appearance13.Image = global::DWOS.UI.Properties.Resources.Stopwatch_16;
            this.grpData.HeaderAppearance = appearance13;
            this.grpData.Text = "Labor";
            this.grpData.Controls.SetChildIndex(this.grdLabor, 0);
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.txtTotalProcess, 0);
            this.grpData.Controls.SetChildIndex(this.txtTotalInspection, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.grpData.Controls.SetChildIndex(this.txtTotalAll, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.grpData.Controls.SetChildIndex(this.txtTotalOther, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            // 
            // grdLabor
            // 
            this.grdLabor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdLabor.DisplayLayout.Appearance = appearance1;
            this.grdLabor.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.Caption = "Operator";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 35;
            ultraGridColumn2.Format = "hh:mm tt";
            ultraGridColumn2.Header.Caption = "Time-In";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.MaskInput = "";
            ultraGridColumn2.Width = 44;
            ultraGridColumn3.Format = "hh:mm tt";
            ultraGridColumn3.Header.Caption = "Time-Out";
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.MaskInput = "";
            ultraGridColumn3.Width = 44;
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridColumn4.Width = 46;
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.Width = 46;
            ultraGridColumn6.Header.Caption = "Batched";
            ultraGridColumn6.Header.VisiblePosition = 6;
            ultraGridColumn6.MaxWidth = 55;
            ultraGridColumn6.MinWidth = 55;
            ultraGridColumn6.Width = 55;
            ultraGridColumn7.Header.Caption = "Duration (Min.)";
            ultraGridColumn7.Header.VisiblePosition = 3;
            ultraGridColumn7.Width = 83;
            ultraGridColumn8.Header.VisiblePosition = 7;
            ultraGridColumn8.MaxWidth = 55;
            ultraGridColumn8.MinWidth = 55;
            ultraGridColumn8.NullText = "n/a";
            ultraGridColumn8.Width = 55;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8});
            this.grdLabor.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdLabor.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdLabor.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.grdLabor.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdLabor.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.grdLabor.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdLabor.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.grdLabor.DisplayLayout.MaxColScrollRegions = 1;
            this.grdLabor.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdLabor.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdLabor.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grdLabor.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.grdLabor.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdLabor.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.grdLabor.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdLabor.DisplayLayout.Override.CellAppearance = appearance8;
            this.grdLabor.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdLabor.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.grdLabor.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.grdLabor.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.grdLabor.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdLabor.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.grdLabor.DisplayLayout.Override.RowAppearance = appearance11;
            this.grdLabor.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdLabor.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.grdLabor.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdLabor.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdLabor.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdLabor.Location = new System.Drawing.Point(8, 23);
            this.grdLabor.Name = "grdLabor";
            this.grdLabor.Size = new System.Drawing.Size(427, 418);
            this.grdLabor.TabIndex = 1;
            this.grdLabor.Text = "ultraGrid1";
            this.grdLabor.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdLabor_AfterCellUpdate);
            this.grdLabor.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdLabor_InitializeLayout);
            this.grdLabor.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdLabor_InitializeRow);
            this.grdLabor.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.grdLabor_BeforeCellUpdate);
            this.grdLabor.Error += new Infragistics.Win.UltraWinGrid.ErrorEventHandler(this.grdLabor_Error);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(106, 451);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(180, 15);
            this.ultraLabel1.TabIndex = 2;
            this.ultraLabel1.Text = "Total Labor Time (Processing):";
            // 
            // txtTotalProcess
            // 
            this.txtTotalProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotalProcess.Location = new System.Drawing.Point(291, 447);
            this.txtTotalProcess.Name = "txtTotalProcess";
            this.txtTotalProcess.ReadOnly = true;
            this.txtTotalProcess.Size = new System.Drawing.Size(143, 22);
            this.txtTotalProcess.TabIndex = 3;
            ultraToolTipInfo4.ToolTipText = "The total labor time spent processing this work order.";
            ultraToolTipInfo4.ToolTipTitle = "Total Labor Time (Processing)";
            this.tipManager.SetUltraToolTip(this.txtTotalProcess, ultraToolTipInfo4);
            // 
            // txtTotalInspection
            // 
            this.txtTotalInspection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotalInspection.Location = new System.Drawing.Point(291, 475);
            this.txtTotalInspection.Name = "txtTotalInspection";
            this.txtTotalInspection.ReadOnly = true;
            this.txtTotalInspection.Size = new System.Drawing.Size(143, 22);
            this.txtTotalInspection.TabIndex = 4;
            ultraToolTipInfo3.ToolTipText = "The total labor time spent inspecting this work order.";
            ultraToolTipInfo3.ToolTipTitle = "Total Labor Time (Inspection)";
            this.tipManager.SetUltraToolTip(this.txtTotalInspection, ultraToolTipInfo3);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(106, 479);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(179, 15);
            this.ultraLabel2.TabIndex = 5;
            this.ultraLabel2.Text = "Total Labor Time (Inspection):";
            // 
            // txtTotalAll
            // 
            this.txtTotalAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotalAll.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotalAll.Location = new System.Drawing.Point(292, 531);
            this.txtTotalAll.Name = "txtTotalAll";
            this.txtTotalAll.ReadOnly = true;
            this.txtTotalAll.Size = new System.Drawing.Size(143, 22);
            this.txtTotalAll.TabIndex = 6;
            ultraToolTipInfo2.ToolTipText = "The total labor time for this work order.";
            ultraToolTipInfo2.ToolTipTitle = "Total Labor Time";
            this.tipManager.SetUltraToolTip(this.txtTotalAll, ultraToolTipInfo2);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel3.Location = new System.Drawing.Point(106, 535);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(116, 15);
            this.ultraLabel3.TabIndex = 7;
            this.ultraLabel3.Text = "Total Labor Time:";
            // 
            // txtTotalOther
            // 
            this.txtTotalOther.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotalOther.Location = new System.Drawing.Point(292, 503);
            this.txtTotalOther.Name = "txtTotalOther";
            this.txtTotalOther.ReadOnly = true;
            this.txtTotalOther.Size = new System.Drawing.Size(143, 22);
            this.txtTotalOther.TabIndex = 5;
            ultraToolTipInfo1.ToolTipText = "The total labor time spent performing miscellaneous activities on this work order" +
    ".";
            ultraToolTipInfo1.ToolTipTitle = "Total Labor Time (Miscellaneous)";
            this.tipManager.SetUltraToolTip(this.txtTotalOther, ultraToolTipInfo1);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(106, 507);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(148, 15);
            this.ultraLabel4.TabIndex = 9;
            this.ultraLabel4.Text = "Total Labor Time (Misc.):";
            // 
            // LaborInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "LaborInformation";
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdLabor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalProcess)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalInspection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalOther)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid grdLabor;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTotalProcess;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTotalInspection;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTotalAll;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTotalOther;
    }
}
