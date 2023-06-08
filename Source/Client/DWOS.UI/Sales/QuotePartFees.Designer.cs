namespace DWOS.UI.Sales
{
    partial class QuotePartFees
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("QuotePartFees", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartFeeID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QuotePartID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FeeType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn385 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Charge");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FeeCalculationType", -1, 1131963391, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
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
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(1131963391);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuotePartFees));
            this.cboFeeType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.grdOrderFees = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.bsOrderFee = new System.Windows.Forms.BindingSource(this.components);
            this.dsQuotes = new DWOS.Data.Datasets.QuoteDataSet();
            this.lblDefaultFees = new Infragistics.Win.Misc.UltraLabel();
            this.maskedEditorPercent = new Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit();
            this.maskedEditorCurrency = new Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit();
            ((System.ComponentModel.ISupportInitialize)(this.cboFeeType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdOrderFees)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsOrderFee)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsQuotes)).BeginInit();
            this.SuspendLayout();
            // 
            // cboFeeType
            // 
            this.cboFeeType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboFeeType.Location = new System.Drawing.Point(163, 189);
            this.cboFeeType.Name = "cboFeeType";
            this.cboFeeType.Size = new System.Drawing.Size(144, 22);
            this.cboFeeType.TabIndex = 6;
            this.cboFeeType.ValueMember = "OrderFeeTypeID";
            this.cboFeeType.Visible = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(362, 227);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grdOrderFees
            // 
            this.grdOrderFees.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdOrderFees.DataSource = this.bsOrderFee;
            this.grdOrderFees.DisplayLayout.AddNewBox.Hidden = false;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdOrderFees.DisplayLayout.Appearance = appearance1;
            this.grdOrderFees.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridBand1.AddButtonCaption = "Quote Part Fee";
            ultraGridColumn1.EditorComponent = this.cboFeeType;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn1.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn1.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            ultraGridColumn1.Width = 82;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn2.Width = 94;
            ultraGridColumn4.Header.Caption = "Fee Name";
            ultraGridColumn4.Header.VisiblePosition = 2;
            ultraGridColumn4.Width = 189;
            ultraGridColumn385.Header.VisiblePosition = 3;
            ultraGridColumn385.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            ultraGridColumn385.MaskInput = "";
            ultraGridColumn385.UseEditorMaskSettings = true;
            ultraGridColumn385.Width = 133;
            ultraGridColumn3.DefaultCellValue = "Fixed";
            ultraGridColumn3.Header.Caption = "Fee Type";
            ultraGridColumn3.Header.VisiblePosition = 4;
            ultraGridColumn3.Nullable = Infragistics.Win.UltraWinGrid.Nullable.EmptyString;
            ultraGridColumn3.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            ultraGridColumn3.Width = 102;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn4,
            ultraGridColumn385,
            ultraGridColumn3});
            this.grdOrderFees.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdOrderFees.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdOrderFees.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.grdOrderFees.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdOrderFees.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.grdOrderFees.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdOrderFees.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.grdOrderFees.DisplayLayout.MaxColScrollRegions = 1;
            this.grdOrderFees.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdOrderFees.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdOrderFees.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grdOrderFees.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            this.grdOrderFees.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.grdOrderFees.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdOrderFees.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdOrderFees.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdOrderFees.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.grdOrderFees.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdOrderFees.DisplayLayout.Override.CellAppearance = appearance8;
            this.grdOrderFees.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdOrderFees.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.grdOrderFees.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.grdOrderFees.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.grdOrderFees.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdOrderFees.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.grdOrderFees.DisplayLayout.Override.RowAppearance = appearance11;
            this.grdOrderFees.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdOrderFees.DisplayLayout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.Bottom;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdOrderFees.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.grdOrderFees.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdOrderFees.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            valueList1.Key = "FeeCalculationTypes";
            valueListItem1.DataValue = "Fixed";
            valueListItem1.DisplayText = "Fixed";
            valueListItem2.DataValue = "Percentage";
            valueListItem2.DisplayText = "Percentage";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.grdOrderFees.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1});
            this.grdOrderFees.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdOrderFees.Location = new System.Drawing.Point(2, 3);
            this.grdOrderFees.Name = "grdOrderFees";
            this.grdOrderFees.Size = new System.Drawing.Size(445, 218);
            this.grdOrderFees.TabIndex = 5;
            this.grdOrderFees.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.grdOrderFees.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdOrderFees_InitializeRow);
            this.grdOrderFees.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdOrderFees_CellChange);
            // 
            // bsOrderFee
            // 
            this.bsOrderFee.DataMember = "QuotePartFees";
            this.bsOrderFee.DataSource = this.dsQuotes;
            // 
            // dsQuotes
            // 
            this.dsQuotes.DataSetName = "QuoteDataSet";
            this.dsQuotes.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // lblDefaultFees
            // 
            appearance13.FontData.SizeInPoints = 7F;
            appearance13.FontData.UnderlineAsString = "True";
            appearance13.ForeColor = System.Drawing.Color.Blue;
            this.lblDefaultFees.Appearance = appearance13;
            this.lblDefaultFees.AutoSize = true;
            this.lblDefaultFees.Location = new System.Drawing.Point(12, 233);
            this.lblDefaultFees.Name = "lblDefaultFees";
            this.lblDefaultFees.Size = new System.Drawing.Size(64, 13);
            this.lblDefaultFees.TabIndex = 58;
            this.lblDefaultFees.Text = "Default Fees";
            this.lblDefaultFees.Click += new System.EventHandler(this.lblDefaultFees_Click);
            // 
            // maskedEditorPercent
            // 
            this.maskedEditorPercent.DataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.maskedEditorPercent.DisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            this.maskedEditorPercent.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2010;
            this.maskedEditorPercent.EditAs = Infragistics.Win.UltraWinMaskedEdit.EditAsType.UseSpecifiedMask;
            this.maskedEditorPercent.InputMask = "nnn.nn%";
            this.maskedEditorPercent.Location = new System.Drawing.Point(207, 163);
            this.maskedEditorPercent.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.maskedEditorPercent.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.maskedEditorPercent.Name = "maskedEditorPercent";
            this.maskedEditorPercent.NonAutoSizeHeight = 21;
            this.maskedEditorPercent.Nullable = false;
            this.maskedEditorPercent.Size = new System.Drawing.Size(100, 21);
            this.maskedEditorPercent.SpinButtonDisplayStyle = Infragistics.Win.SpinButtonDisplayStyle.OnRight;
            this.maskedEditorPercent.SpinButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2010Button;
            this.maskedEditorPercent.SpinIncrement = ((uint)(1u));
            this.maskedEditorPercent.TabIndex = 64;
            this.maskedEditorPercent.Visible = false;
            // 
            // maskedEditorCurrency
            // 
            this.maskedEditorCurrency.DisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            this.maskedEditorCurrency.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2010;
            this.maskedEditorCurrency.EditAs = Infragistics.Win.UltraWinMaskedEdit.EditAsType.UseSpecifiedMask;
            this.maskedEditorCurrency.InputMask = "{currency:5.2}";
            this.maskedEditorCurrency.Location = new System.Drawing.Point(207, 136);
            this.maskedEditorCurrency.MaxValue = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.maskedEditorCurrency.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.maskedEditorCurrency.Name = "maskedEditorCurrency";
            this.maskedEditorCurrency.NonAutoSizeHeight = 21;
            this.maskedEditorCurrency.Nullable = false;
            this.maskedEditorCurrency.Size = new System.Drawing.Size(100, 21);
            this.maskedEditorCurrency.SpinButtonDisplayStyle = Infragistics.Win.SpinButtonDisplayStyle.OnRight;
            this.maskedEditorCurrency.SpinButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2010Button;
            this.maskedEditorCurrency.SpinIncrement = ((uint)(1u));
            this.maskedEditorCurrency.TabIndex = 63;
            this.maskedEditorCurrency.Text = "$";
            this.maskedEditorCurrency.Visible = false;
            // 
            // QuotePartFees
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 262);
            this.Controls.Add(this.maskedEditorPercent);
            this.Controls.Add(this.maskedEditorCurrency);
            this.Controls.Add(this.lblDefaultFees);
            this.Controls.Add(this.grdOrderFees);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cboFeeType);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "QuotePartFees";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Quote Part Fees";
            this.Load += new System.EventHandler(this.OrderFees_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cboFeeType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdOrderFees)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsOrderFee)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsQuotes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.UltraWinGrid.UltraGrid grdOrderFees;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboFeeType;
        private System.Windows.Forms.BindingSource bsOrderFee;
		private Infragistics.Win.Misc.UltraLabel lblDefaultFees;
        private Data.Datasets.QuoteDataSet dsQuotes;
        private Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit maskedEditorPercent;
        private Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit maskedEditorCurrency;
	}
}