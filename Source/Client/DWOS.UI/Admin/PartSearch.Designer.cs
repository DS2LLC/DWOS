﻿namespace DWOS.UI.Admin
{
    partial class PartSearch
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("PartSearch", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn35 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn36 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn38 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Material");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn43 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ManufacturerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn45 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Airframe");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn44 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Active");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn51 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AssemblyNumber");
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Show the part quick view dialog.", Infragistics.Win.ToolTipImage.Default, "Part Quick View", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Active Only", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The field to search for.", Infragistics.Win.ToolTipImage.Default, "Field", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The value to search for.", Infragistics.Win.ToolTipImage.Default, "Value", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PartSearch));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.grdParts = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsParts = new DWOS.Data.Datasets.PartsDataset();
            this.btnView = new Infragistics.Win.Misc.UltraButton();
            this.btnSearch = new Infragistics.Win.Misc.UltraButton();
            this.chkActiveOnly = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.cboQuoteSearchField = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtSearch = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lblRecordCount = new Infragistics.Win.Misc.UltraLabel();
            this.taPartSearch = new DWOS.Data.Datasets.PartsDatasetTableAdapters.PartSearchTableAdapter();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdParts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsParts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActiveOnly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboQuoteSearchField)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(717, 356);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 23);
            this.btnCancel.TabIndex = 22;
            this.btnCancel.Text = "&Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(622, 356);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 21;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grdParts
            // 
            this.grdParts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdParts.DataMember = "PartSearch";
            this.grdParts.DataSource = this.dsParts;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdParts.DisplayLayout.Appearance = appearance1;
            this.grdParts.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn35.Header.VisiblePosition = 0;
            ultraGridColumn35.Hidden = true;
            ultraGridColumn36.Header.VisiblePosition = 1;
            ultraGridColumn36.Width = 131;
            ultraGridColumn1.Header.Caption = "Customer Name";
            ultraGridColumn1.Header.VisiblePosition = 2;
            ultraGridColumn1.Width = 125;
            ultraGridColumn38.Header.VisiblePosition = 3;
            ultraGridColumn38.Width = 133;
            ultraGridColumn43.Header.Caption = "Manufacturer";
            ultraGridColumn43.Header.VisiblePosition = 4;
            ultraGridColumn43.Width = 74;
            ultraGridColumn45.Header.VisiblePosition = 6;
            ultraGridColumn45.Width = 127;
            ultraGridColumn44.Header.VisiblePosition = 5;
            ultraGridColumn44.Width = 47;
            ultraGridColumn51.Header.Caption = "Assembly";
            ultraGridColumn51.Header.VisiblePosition = 7;
            ultraGridColumn51.Width = 106;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn35,
            ultraGridColumn36,
            ultraGridColumn1,
            ultraGridColumn38,
            ultraGridColumn43,
            ultraGridColumn45,
            ultraGridColumn44,
            ultraGridColumn51});
            this.grdParts.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdParts.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdParts.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.grdParts.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdParts.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.grdParts.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdParts.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.grdParts.DisplayLayout.MaxColScrollRegions = 1;
            this.grdParts.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdParts.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdParts.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grdParts.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdParts.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdParts.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdParts.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.grdParts.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdParts.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.grdParts.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdParts.DisplayLayout.Override.CellAppearance = appearance8;
            this.grdParts.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.grdParts.DisplayLayout.Override.CellPadding = 0;
            this.grdParts.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.grdParts.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.grdParts.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.grdParts.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdParts.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.grdParts.DisplayLayout.Override.RowAppearance = appearance11;
            this.grdParts.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdParts.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.grdParts.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdParts.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdParts.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdParts.Location = new System.Drawing.Point(44, 42);
            this.grdParts.Name = "grdParts";
            this.grdParts.Size = new System.Drawing.Size(764, 308);
            this.grdParts.TabIndex = 23;
            // 
            // dsParts
            // 
            this.dsParts.DataSetName = "PartsDataset";
            this.dsParts.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // btnView
            // 
            appearance13.Image = global::DWOS.UI.Properties.Resources.View;
            this.btnView.Appearance = appearance13;
            this.btnView.AutoSize = true;
            this.btnView.Location = new System.Drawing.Point(12, 42);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(26, 26);
            this.btnView.TabIndex = 24;
            ultraToolTipInfo4.ToolTipText = "Show the part quick view dialog.";
            ultraToolTipInfo4.ToolTipTitle = "Part Quick View";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnView, ultraToolTipInfo4);
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // btnSearch
            // 
            appearance14.Image = global::DWOS.UI.Properties.Resources.Search_16;
            this.btnSearch.Appearance = appearance14;
            this.btnSearch.AutoSize = true;
            this.btnSearch.Location = new System.Drawing.Point(427, 10);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(69, 26);
            this.btnSearch.TabIndex = 28;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // chkActiveOnly
            // 
            this.chkActiveOnly.AutoSize = true;
            this.chkActiveOnly.Checked = true;
            this.chkActiveOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActiveOnly.Location = new System.Drawing.Point(502, 14);
            this.chkActiveOnly.Name = "chkActiveOnly";
            this.chkActiveOnly.Size = new System.Drawing.Size(86, 18);
            this.chkActiveOnly.TabIndex = 29;
            this.chkActiveOnly.Text = "Active Only";
            ultraToolTipInfo3.ToolTipTextFormatted = "If <strong>checked</strong>, will only search active parts.<br />If <strong>unche" +
    "cked</strong>, will search all parts.";
            ultraToolTipInfo3.ToolTipTitle = "Active Only";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkActiveOnly, ultraToolTipInfo3);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            this.ultraToolTipManager1.ToolTipImage = Infragistics.Win.ToolTipImage.Info;
            // 
            // cboQuoteSearchField
            // 
            this.cboQuoteSearchField.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboQuoteSearchField.Location = new System.Drawing.Point(86, 12);
            this.cboQuoteSearchField.Name = "cboQuoteSearchField";
            this.cboQuoteSearchField.Size = new System.Drawing.Size(158, 22);
            this.cboQuoteSearchField.TabIndex = 35;
            ultraToolTipInfo1.ToolTipText = "The field to search for.";
            ultraToolTipInfo1.ToolTipTitle = "Field";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboQuoteSearchField, ultraToolTipInfo1);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(297, 12);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(124, 22);
            this.txtSearch.TabIndex = 33;
            ultraToolTipInfo2.ToolTipText = "The value to search for.";
            ultraToolTipInfo2.ToolTipTitle = "Value";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtSearch, ultraToolTipInfo2);
            this.txtSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyUp);
            // 
            // lblRecordCount
            // 
            this.lblRecordCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRecordCount.AutoSize = true;
            this.lblRecordCount.Location = new System.Drawing.Point(44, 361);
            this.lblRecordCount.Name = "lblRecordCount";
            this.lblRecordCount.Size = new System.Drawing.Size(97, 15);
            this.lblRecordCount.TabIndex = 30;
            this.lblRecordCount.Text = "Record Count: 0";
            // 
            // taPartSearch
            // 
            this.taPartSearch.ClearBeforeFill = true;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(250, 16);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(41, 15);
            this.ultraLabel1.TabIndex = 36;
            this.ultraLabel1.Text = "Value:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(44, 16);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(36, 15);
            this.ultraLabel3.TabIndex = 34;
            this.ultraLabel3.Text = "Field:";
            // 
            // PartSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 391);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.cboQuoteSearchField);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.lblRecordCount);
            this.Controls.Add(this.chkActiveOnly);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.grdParts);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PartSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Part Search";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PartSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdParts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsParts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActiveOnly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboQuoteSearchField)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdParts;
        private Infragistics.Win.Misc.UltraButton btnView;
        private Infragistics.Win.Misc.UltraButton btnSearch;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkActiveOnly;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Data.Datasets.PartsDataset dsParts;
        private Infragistics.Win.Misc.UltraLabel lblRecordCount;
        private Data.Datasets.PartsDatasetTableAdapters.PartSearchTableAdapter taPartSearch;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboQuoteSearchField;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSearch;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
    }
}