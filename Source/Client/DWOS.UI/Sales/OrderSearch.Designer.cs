namespace DWOS.UI.Sales
{
    partial class OrderSearch
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("OrderSearch", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PurchaseOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Invoice");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WorkStatus");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CurrentLocation");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerWO");
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The value to search for.", Infragistics.Win.ToolTipImage.Default, "Value", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Active Only ", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Go to the selected order in order entry.", Infragistics.Win.ToolTipImage.Default, "Go To", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Show the order quick view dialog.", Infragistics.Win.ToolTipImage.Default, "Order Quick View", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The field to search for.", Infragistics.Win.ToolTipImage.Default, "Field", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderSearch));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.grdParts = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsOrderStatus = new DWOS.Data.Datasets.OrderStatusDataSet();
            this.txtSearch = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.btnSearch = new Infragistics.Win.Misc.UltraButton();
            this.chkActiveOnly = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.btnGoTo = new Infragistics.Win.Misc.UltraButton();
            this.btnView = new Infragistics.Win.Misc.UltraButton();
            this.cboQuoteSearchField = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.lblRecordCount = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.taOrderSearch = new DWOS.Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter();
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdParts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrderStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActiveOnly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboQuoteSearchField)).BeginInit();
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
            // 
            // grdParts
            // 
            this.grdParts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdParts.DataMember = "OrderSearch";
            this.grdParts.DataSource = this.dsOrderStatus;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdParts.DisplayLayout.Appearance = appearance1;
            this.grdParts.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.Caption = "WO";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 80;
            ultraGridColumn2.Header.Caption = "Customer";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 121;
            ultraGridColumn3.Header.Caption = "Part";
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 136;
            ultraGridColumn4.Header.Caption = "Created";
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Width = 123;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn5.Width = 102;
            ultraGridColumn6.Header.Caption = "PO";
            ultraGridColumn6.Header.VisiblePosition = 6;
            ultraGridColumn6.Width = 134;
            ultraGridColumn7.Header.VisiblePosition = 8;
            ultraGridColumn7.Hidden = true;
            ultraGridColumn7.Width = 84;
            ultraGridColumn8.Header.Caption = "Work Status";
            ultraGridColumn8.Header.VisiblePosition = 5;
            ultraGridColumn8.Hidden = true;
            ultraGridColumn8.Width = 71;
            ultraGridColumn9.Header.Caption = "Location";
            ultraGridColumn9.Header.VisiblePosition = 7;
            ultraGridColumn9.Hidden = true;
            ultraGridColumn9.Width = 56;
            ultraGridColumn10.Header.Caption = "Customer WO";
            ultraGridColumn10.Header.VisiblePosition = 9;
            ultraGridColumn10.Width = 130;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10});
            this.grdParts.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdParts.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdParts.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.grdParts.DisplayLayout.ColumnChooserEnabled = Infragistics.Win.DefaultableBoolean.True;
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
            this.grdParts.DisplayLayout.Override.ExpansionIndicator = Infragistics.Win.UltraWinGrid.ShowExpansionIndicator.CheckOnExpand;
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
            this.grdParts.SyncWithCurrencyManager = false;
            this.grdParts.TabIndex = 23;
            this.grdParts.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnUpdate;
            // 
            // dsOrderStatus
            // 
            this.dsOrderStatus.DataSetName = "OrderStatusDataSet";
            this.dsOrderStatus.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(265, 12);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(124, 22);
            this.txtSearch.TabIndex = 26;
            ultraToolTipInfo5.ToolTipText = "The value to search for.";
            ultraToolTipInfo5.ToolTipTitle = "Value";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtSearch, ultraToolTipInfo5);
            this.txtSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyUp);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(12, 16);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(36, 15);
            this.ultraLabel3.TabIndex = 27;
            this.ultraLabel3.Text = "Field:";
            // 
            // btnSearch
            // 
            appearance14.Image = global::DWOS.UI.Properties.Resources.Search_16;
            this.btnSearch.Appearance = appearance14;
            this.btnSearch.AutoSize = true;
            this.btnSearch.Location = new System.Drawing.Point(395, 10);
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
            this.chkActiveOnly.Location = new System.Drawing.Point(481, 14);
            this.chkActiveOnly.Name = "chkActiveOnly";
            this.chkActiveOnly.Size = new System.Drawing.Size(86, 18);
            this.chkActiveOnly.TabIndex = 29;
            this.chkActiveOnly.Text = "Active Only";
            ultraToolTipInfo3.ToolTipTextFormatted = "If <strong>checked</strong>, will only search active orders.<br/>If <span style=\"" +
    "font-weight:bold;\">unchecked</span>, will search all orders.";
            ultraToolTipInfo3.ToolTipTitle = "Active Only ";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkActiveOnly, ultraToolTipInfo3);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            this.ultraToolTipManager1.ToolTipImage = Infragistics.Win.ToolTipImage.Info;
            // 
            // btnGoTo
            // 
            appearance15.Image = global::DWOS.UI.Properties.Resources.Run;
            this.btnGoTo.Appearance = appearance15;
            this.btnGoTo.AutoSize = true;
            this.btnGoTo.Location = new System.Drawing.Point(12, 74);
            this.btnGoTo.Name = "btnGoTo";
            this.btnGoTo.Size = new System.Drawing.Size(26, 26);
            this.btnGoTo.TabIndex = 25;
            ultraToolTipInfo4.ToolTipText = "Go to the selected order in order entry.";
            ultraToolTipInfo4.ToolTipTitle = "Go To";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnGoTo, ultraToolTipInfo4);
            this.btnGoTo.Click += new System.EventHandler(this.btnGoTo_Click);
            // 
            // btnView
            // 
            appearance13.Image = global::DWOS.UI.Properties.Resources.View;
            this.btnView.Appearance = appearance13;
            this.btnView.AutoSize = true;
            this.btnView.Location = new System.Drawing.Point(12, 42);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(26, 26);
            this.btnView.TabIndex = 33;
            ultraToolTipInfo1.ToolTipText = "Show the order quick view dialog.";
            ultraToolTipInfo1.ToolTipTitle = "Order Quick View";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnView, ultraToolTipInfo1);
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // cboQuoteSearchField
            // 
            this.cboQuoteSearchField.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboQuoteSearchField.Location = new System.Drawing.Point(54, 12);
            this.cboQuoteSearchField.Name = "cboQuoteSearchField";
            this.cboQuoteSearchField.Size = new System.Drawing.Size(158, 22);
            this.cboQuoteSearchField.TabIndex = 31;
            ultraToolTipInfo2.ToolTipText = "The field to search for.";
            ultraToolTipInfo2.ToolTipTitle = "Field";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboQuoteSearchField, ultraToolTipInfo2);
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
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(218, 15);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(41, 15);
            this.ultraLabel1.TabIndex = 32;
            this.ultraLabel1.Text = "Value:";
            // 
            // taOrderSearch
            // 
            this.taOrderSearch.ClearBeforeFill = true;
            // 
            // helpLink1
            // 
            this.helpLink1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = "order_search_dialog.htm";
            this.helpLink1.Location = new System.Drawing.Point(5, 363);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 34;
            // 
            // OrderSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 391);
            this.Controls.Add(this.helpLink1);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.cboQuoteSearchField);
            this.Controls.Add(this.lblRecordCount);
            this.Controls.Add(this.chkActiveOnly);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.btnGoTo);
            this.Controls.Add(this.grdParts);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OrderSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Order Search";
            this.Load += new System.EventHandler(this.QuoteSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdParts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrderStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActiveOnly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboQuoteSearchField)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdParts;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSearch;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraButton btnSearch;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkActiveOnly;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.Misc.UltraLabel lblRecordCount;
        private Infragistics.Win.Misc.UltraButton btnGoTo;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboQuoteSearchField;
        private Data.Datasets.OrderStatusDataSet dsOrderStatus;
        private Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter taOrderSearch;
        private Infragistics.Win.Misc.UltraButton btnView;
        private Utilities.HelpLink helpLink1;
    }
}