namespace DWOS.UI.Admin
{
	partial class OrderFeeManager
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("OrderFeeType", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderFeeTypeID", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Price");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("InvoiceItemName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FeeType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_OrderFees_OrderFeeType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_CustomerFee_OrderFeeType");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_OrderFees_OrderFeeType", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderFeeID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderFeeTypeID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Charge");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand3 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_CustomerFee_OrderFeeType", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerFeeID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderFeeTypeID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Charge");
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderFeeManager));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.grdFees = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsOrders = new DWOS.Data.Datasets.OrdersDataSet();
            this.taOrderFeeType = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderFeeTypeTableAdapter();
            this.taFeeType = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.d_FeeTypeTableAdapter();
            this.maskedEditorPercent = new Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit();
            this.maskedEditorCurrency = new Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdFees)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(421, 321);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(326, 321);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grdFees
            // 
            this.grdFees.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdFees.DataMember = "OrderFeeType";
            this.grdFees.DataSource = this.dsOrders;
            this.grdFees.DisplayLayout.AddNewBox.Hidden = false;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdFees.DisplayLayout.Appearance = appearance1;
            this.grdFees.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridBand1.AddButtonCaption = "Fee/Discount";
            ultraGridBand1.AddButtonToolTipText = "Add a new fee or discount";
            ultraGridColumn1.Header.Caption = "Fee Name";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.MinLength = 1;
            ultraGridColumn1.MinValue = "";
            ultraGridColumn1.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn1.NullText = "New Fee";
            appearance2.TextHAlignAsString = "Right";
            ultraGridColumn2.CellAppearance = appearance2;
            ultraGridColumn2.Header.Caption = "Default Charge";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.MaxValue = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            ultraGridColumn2.MinValue = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            ultraGridColumn2.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn2.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DoublePositiveWithSpin;
            ultraGridColumn8.DefaultCellValue = "Processing Fee";
            ultraGridColumn8.Header.Caption = "Invoice Item";
            ultraGridColumn8.Header.VisiblePosition = 2;
            ultraGridColumn8.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn9.Header.Caption = "Fee Type";
            ultraGridColumn9.Header.VisiblePosition = 3;
            ultraGridColumn9.Nullable = Infragistics.Win.UltraWinGrid.Nullable.EmptyString;
            ultraGridColumn3.Header.VisiblePosition = 4;
            ultraGridColumn10.Header.VisiblePosition = 5;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn3,
            ultraGridColumn10});
            ultraGridBand1.Override.InvalidValueBehavior = Infragistics.Win.UltraWinGrid.InvalidValueBehavior.RevertValueAndRetainFocus;
            ultraGridBand1.Override.SupportDataErrorInfo = Infragistics.Win.UltraWinGrid.SupportDataErrorInfo.CellsOnly;
            ultraGridColumn4.Header.VisiblePosition = 0;
            ultraGridColumn5.Header.VisiblePosition = 1;
            ultraGridColumn6.Header.VisiblePosition = 2;
            ultraGridColumn7.Header.VisiblePosition = 3;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7});
            ultraGridBand2.Hidden = true;
            ultraGridColumn11.Header.VisiblePosition = 0;
            ultraGridColumn12.Header.VisiblePosition = 1;
            ultraGridColumn13.Header.VisiblePosition = 2;
            ultraGridColumn14.Header.VisiblePosition = 3;
            ultraGridBand3.Columns.AddRange(new object[] {
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14});
            ultraGridBand3.Hidden = true;
            this.grdFees.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdFees.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdFees.DisplayLayout.BandsSerializer.Add(ultraGridBand3);
            this.grdFees.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdFees.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance3.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance3.BorderColor = System.Drawing.SystemColors.Window;
            this.grdFees.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdFees.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.grdFees.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdFees.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.grdFees.DisplayLayout.MaxColScrollRegions = 1;
            this.grdFees.DisplayLayout.MaxRowScrollRegions = 1;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            appearance6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdFees.DisplayLayout.Override.ActiveCellAppearance = appearance6;
            appearance7.BackColor = System.Drawing.SystemColors.Highlight;
            appearance7.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdFees.DisplayLayout.Override.ActiveRowAppearance = appearance7;
            this.grdFees.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            this.grdFees.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.grdFees.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdFees.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdFees.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance8.BackColor = System.Drawing.SystemColors.Window;
            this.grdFees.DisplayLayout.Override.CardAreaAppearance = appearance8;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            appearance9.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdFees.DisplayLayout.Override.CellAppearance = appearance9;
            this.grdFees.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdFees.DisplayLayout.Override.CellPadding = 0;
            appearance10.BackColor = System.Drawing.SystemColors.Control;
            appearance10.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance10.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance10.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance10.BorderColor = System.Drawing.SystemColors.Window;
            this.grdFees.DisplayLayout.Override.GroupByRowAppearance = appearance10;
            appearance11.TextHAlignAsString = "Left";
            this.grdFees.DisplayLayout.Override.HeaderAppearance = appearance11;
            this.grdFees.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdFees.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance12.BackColor = System.Drawing.SystemColors.Window;
            appearance12.BorderColor = System.Drawing.Color.Silver;
            this.grdFees.DisplayLayout.Override.RowAppearance = appearance12;
            this.grdFees.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance13.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdFees.DisplayLayout.Override.TemplateAddRowAppearance = appearance13;
            this.grdFees.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdFees.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdFees.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdFees.Location = new System.Drawing.Point(12, 12);
            this.grdFees.Name = "grdFees";
            this.grdFees.Size = new System.Drawing.Size(500, 303);
            this.grdFees.TabIndex = 19;
            this.grdFees.Text = "ultraGrid1";
            this.grdFees.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdFees_InitializeLayout);
            this.grdFees.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdFees_InitializeRow);
            this.grdFees.AfterRowInsert += new Infragistics.Win.UltraWinGrid.RowEventHandler(this.grdFees_AfterRowInsert);
            this.grdFees.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.grdFees_BeforeCellUpdate);
            this.grdFees.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.grdFees_BeforeRowsDeleted);
            // 
            // dsOrders
            // 
            this.dsOrders.DataSetName = "OrdersDataSet";
            this.dsOrders.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taOrderFeeType
            // 
            this.taOrderFeeType.ClearBeforeFill = false;
            // 
            // taFeeType
            // 
            this.taFeeType.ClearBeforeFill = true;
            // 
            // maskedEditorPercent
            // 
            this.maskedEditorPercent.DataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.maskedEditorPercent.DisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            this.maskedEditorPercent.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2010;
            this.maskedEditorPercent.EditAs = Infragistics.Win.UltraWinMaskedEdit.EditAsType.UseSpecifiedMask;
            this.maskedEditorPercent.InputMask = "-nnn.nn%";
            this.maskedEditorPercent.Location = new System.Drawing.Point(200, 321);
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
            this.maskedEditorPercent.TabIndex = 62;
            this.maskedEditorPercent.Visible = false;
            // 
            // maskedEditorCurrency
            // 
            this.maskedEditorCurrency.DisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            this.maskedEditorCurrency.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2010;
            this.maskedEditorCurrency.EditAs = Infragistics.Win.UltraWinMaskedEdit.EditAsType.UseSpecifiedMask;
            this.maskedEditorCurrency.InputMask = "{currency:-5.2}";
            this.maskedEditorCurrency.Location = new System.Drawing.Point(200, 294);
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
            this.maskedEditorCurrency.TabIndex = 61;
            this.maskedEditorCurrency.Text = "$";
            this.maskedEditorCurrency.Visible = false;
            // 
            // OrderFeeManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 356);
            this.Controls.Add(this.maskedEditorPercent);
            this.Controls.Add(this.maskedEditorCurrency);
            this.Controls.Add(this.grdFees);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OrderFeeManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Order Fee Manager";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FeeManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdFees)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
		private Infragistics.Win.UltraWinGrid.UltraGrid grdFees;
		private Data.Datasets.OrdersDataSet dsOrders;
		private Data.Datasets.OrdersDataSetTableAdapters.OrderFeeTypeTableAdapter taOrderFeeType;
        private Data.Datasets.OrdersDataSetTableAdapters.d_FeeTypeTableAdapter taFeeType;
        private Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit maskedEditorPercent;
        private Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit maskedEditorCurrency;
	}
}