namespace DWOS.UI.Sales
{
	partial class OrderFees
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("OrderFees", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderFeeID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderFeeTypeID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Charge");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FeeType", 0);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderFees));
            this.cboFeeType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.grdOrderFees = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.bsOrderFee = new System.Windows.Forms.BindingSource(this.components);
            this.dsOrders = new DWOS.Data.Datasets.OrdersDataSet();
            this.lblDefaultFees = new Infragistics.Win.Misc.UltraLabel();
            this.maskedEditorCurrency = new Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit();
            this.maskedEditorPercent = new Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit();
            ((System.ComponentModel.ISupportInitialize)(this.cboFeeType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdOrderFees)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsOrderFee)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).BeginInit();
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
            this.btnOK.Location = new System.Drawing.Point(246, 226);
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
            ultraGridBand1.AddButtonCaption = "Order Fee/Discount";
            ultraGridColumn10.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn10.Header.VisiblePosition = 0;
            ultraGridColumn10.Hidden = true;
            ultraGridColumn10.Width = 267;
            ultraGridColumn11.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn11.Header.VisiblePosition = 1;
            ultraGridColumn11.Hidden = true;
            ultraGridColumn11.Width = 266;
            ultraGridColumn12.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn12.EditorComponent = this.cboFeeType;
            ultraGridColumn12.Header.Caption = "Fee Type";
            ultraGridColumn12.Header.VisiblePosition = 2;
            ultraGridColumn12.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            ultraGridColumn12.Width = 119;
            ultraGridColumn13.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance2.TextHAlignAsString = "Right";
            ultraGridColumn13.CellAppearance = appearance2;
            ultraGridColumn13.Header.VisiblePosition = 3;
            ultraGridColumn13.MaxValue = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            ultraGridColumn13.MinValue = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            ultraGridColumn13.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn13.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DoublePositiveWithSpin;
            ultraGridColumn13.Width = 97;
            ultraGridColumn14.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn14.Header.Caption = "Charge Type";
            ultraGridColumn14.Header.VisiblePosition = 4;
            ultraGridColumn14.Width = 92;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14});
            this.grdOrderFees.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdOrderFees.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdOrderFees.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance3.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance3.BorderColor = System.Drawing.SystemColors.Window;
            this.grdOrderFees.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdOrderFees.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.grdOrderFees.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdOrderFees.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.grdOrderFees.DisplayLayout.MaxColScrollRegions = 1;
            this.grdOrderFees.DisplayLayout.MaxRowScrollRegions = 1;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            appearance6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdOrderFees.DisplayLayout.Override.ActiveCellAppearance = appearance6;
            appearance7.BackColor = System.Drawing.SystemColors.Highlight;
            appearance7.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdOrderFees.DisplayLayout.Override.ActiveRowAppearance = appearance7;
            this.grdOrderFees.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            this.grdOrderFees.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.grdOrderFees.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdOrderFees.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdOrderFees.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdOrderFees.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance8.BackColor = System.Drawing.SystemColors.Window;
            this.grdOrderFees.DisplayLayout.Override.CardAreaAppearance = appearance8;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            appearance9.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdOrderFees.DisplayLayout.Override.CellAppearance = appearance9;
            this.grdOrderFees.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdOrderFees.DisplayLayout.Override.CellPadding = 0;
            appearance10.BackColor = System.Drawing.SystemColors.Control;
            appearance10.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance10.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance10.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance10.BorderColor = System.Drawing.SystemColors.Window;
            this.grdOrderFees.DisplayLayout.Override.GroupByRowAppearance = appearance10;
            appearance11.TextHAlignAsString = "Left";
            this.grdOrderFees.DisplayLayout.Override.HeaderAppearance = appearance11;
            this.grdOrderFees.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdOrderFees.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance12.BackColor = System.Drawing.SystemColors.Window;
            appearance12.BorderColor = System.Drawing.Color.Silver;
            this.grdOrderFees.DisplayLayout.Override.RowAppearance = appearance12;
            this.grdOrderFees.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdOrderFees.DisplayLayout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.Bottom;
            appearance13.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdOrderFees.DisplayLayout.Override.TemplateAddRowAppearance = appearance13;
            this.grdOrderFees.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdOrderFees.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdOrderFees.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdOrderFees.Location = new System.Drawing.Point(2, 3);
            this.grdOrderFees.Name = "grdOrderFees";
            this.grdOrderFees.Size = new System.Drawing.Size(329, 217);
            this.grdOrderFees.TabIndex = 5;
            this.grdOrderFees.Text = "ultraGrid1";
            this.grdOrderFees.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.grdOrderFees.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdOrderFees_InitializeLayout);
            this.grdOrderFees.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdOrderFees_InitializeRow);
            this.grdOrderFees.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdOrderFees_CellChange);
            // 
            // bsOrderFee
            // 
            this.bsOrderFee.DataMember = "OrderFees";
            this.bsOrderFee.DataSource = this.dsOrders;
            // 
            // dsOrders
            // 
            this.dsOrders.DataSetName = "OrdersDataSet";
            this.dsOrders.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // lblDefaultFees
            // 
            appearance14.FontData.SizeInPoints = 7F;
            appearance14.FontData.UnderlineAsString = "True";
            appearance14.ForeColor = System.Drawing.Color.Blue;
            this.lblDefaultFees.Appearance = appearance14;
            this.lblDefaultFees.AutoSize = true;
            this.lblDefaultFees.Location = new System.Drawing.Point(12, 233);
            this.lblDefaultFees.Name = "lblDefaultFees";
            this.lblDefaultFees.Size = new System.Drawing.Size(64, 13);
            this.lblDefaultFees.TabIndex = 58;
            this.lblDefaultFees.Text = "Default Fees";
            this.lblDefaultFees.Click += new System.EventHandler(this.lblDefaultFees_Click);
            // 
            // maskedEditorCurrency
            // 
            this.maskedEditorCurrency.DisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            this.maskedEditorCurrency.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2010;
            this.maskedEditorCurrency.EditAs = Infragistics.Win.UltraWinMaskedEdit.EditAsType.UseSpecifiedMask;
            this.maskedEditorCurrency.InputMask = "{currency:-5.2}";
            this.maskedEditorCurrency.Location = new System.Drawing.Point(103, 217);
            this.maskedEditorCurrency.MaxValue = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.maskedEditorCurrency.MinValue = new decimal(new int[] {
            99999,
            0,
            0,
            -2147483648});
            this.maskedEditorCurrency.Name = "maskedEditorCurrency";
            this.maskedEditorCurrency.NonAutoSizeHeight = 21;
            this.maskedEditorCurrency.Nullable = false;
            this.maskedEditorCurrency.Size = new System.Drawing.Size(100, 21);
            this.maskedEditorCurrency.SpinButtonDisplayStyle = Infragistics.Win.SpinButtonDisplayStyle.OnRight;
            this.maskedEditorCurrency.SpinButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2010Button;
            this.maskedEditorCurrency.SpinIncrement = ((uint)(1u));
            this.maskedEditorCurrency.TabIndex = 59;
            this.maskedEditorCurrency.Text = "$";
            this.maskedEditorCurrency.Visible = false;
            // 
            // maskedEditorPercent
            // 
            this.maskedEditorPercent.DataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.maskedEditorPercent.DisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            this.maskedEditorPercent.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2010;
            this.maskedEditorPercent.EditAs = Infragistics.Win.UltraWinMaskedEdit.EditAsType.UseSpecifiedMask;
            this.maskedEditorPercent.InputMask = "-nnn.nn%";
            this.maskedEditorPercent.Location = new System.Drawing.Point(103, 244);
            this.maskedEditorPercent.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.maskedEditorPercent.MinValue = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.maskedEditorPercent.Name = "maskedEditorPercent";
            this.maskedEditorPercent.NonAutoSizeHeight = 21;
            this.maskedEditorPercent.Nullable = false;
            this.maskedEditorPercent.Size = new System.Drawing.Size(100, 21);
            this.maskedEditorPercent.SpinButtonDisplayStyle = Infragistics.Win.SpinButtonDisplayStyle.OnRight;
            this.maskedEditorPercent.SpinButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2010Button;
            this.maskedEditorPercent.SpinIncrement = ((uint)(1u));
            this.maskedEditorPercent.TabIndex = 60;
            this.maskedEditorPercent.Visible = false;
            // 
            // OrderFees
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 261);
            this.Controls.Add(this.maskedEditorPercent);
            this.Controls.Add(this.maskedEditorCurrency);
            this.Controls.Add(this.lblDefaultFees);
            this.Controls.Add(this.grdOrderFees);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cboFeeType);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OrderFees";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Order Fees & Discounts";
            this.Load += new System.EventHandler(this.OrderFees_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cboFeeType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdOrderFees)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsOrderFee)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.UltraWinGrid.UltraGrid grdOrderFees;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboFeeType;
		private System.Windows.Forms.BindingSource bsOrderFee;
		private DWOS.Data.Datasets.OrdersDataSet dsOrders;
		private Infragistics.Win.Misc.UltraLabel lblDefaultFees;
        private Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit maskedEditorCurrency;
        private Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit maskedEditorPercent;
	}
}