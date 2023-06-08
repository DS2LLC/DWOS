namespace DWOS.UI.Admin
{
    partial class PriceUnitManager
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("PriceUnit", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn404 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PriceUnitID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn407 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DisplayName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn410 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Active");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn411 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_Order_d_PriceUnit");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn412 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_OrderTemplate_d_PriceUnit");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_PricePointDetail_d_PriceUnit");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_PartProcessVolumePrice_d_PriceUnit");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_CustomerPricePointDetail_d_PriceUnit");
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PriceUnitManager));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.grdAirframe = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsOrders = new DWOS.Data.Datasets.OrdersDataSet();
            this.taPriceUnit = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.PriceUnitTableAdapter();
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdAirframe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(486, 330);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(391, 330);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grdAirframe
            // 
            this.grdAirframe.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdAirframe.DataMember = "PriceUnit";
            this.grdAirframe.DataSource = this.dsOrders;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdAirframe.DisplayLayout.Appearance = appearance1;
            this.grdAirframe.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridBand1.AddButtonCaption = "Fee";
            ultraGridBand1.AddButtonToolTipText = "Add a new fee";
            ultraGridColumn404.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn404.Header.VisiblePosition = 2;
            ultraGridColumn404.Hidden = true;
            ultraGridColumn404.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn407.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn407.Header.Caption = "Price Unit";
            ultraGridColumn407.Header.VisiblePosition = 0;
            ultraGridColumn407.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn407.Width = 395;
            ultraGridColumn410.Header.Caption = "Enabled";
            ultraGridColumn410.Header.VisiblePosition = 1;
            ultraGridColumn410.Width = 161;
            ultraGridColumn411.Header.VisiblePosition = 4;
            ultraGridColumn412.Header.VisiblePosition = 5;
            ultraGridColumn1.Header.VisiblePosition = 3;
            ultraGridColumn2.Header.VisiblePosition = 6;
            ultraGridColumn3.Header.VisiblePosition = 7;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn404,
            ultraGridColumn407,
            ultraGridColumn410,
            ultraGridColumn411,
            ultraGridColumn412,
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3});
            ultraGridBand1.Override.InvalidValueBehavior = Infragistics.Win.UltraWinGrid.InvalidValueBehavior.RevertValueAndRetainFocus;
            ultraGridBand1.Override.SupportDataErrorInfo = Infragistics.Win.UltraWinGrid.SupportDataErrorInfo.CellsOnly;
            this.grdAirframe.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdAirframe.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdAirframe.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.grdAirframe.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdAirframe.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.grdAirframe.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdAirframe.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.grdAirframe.DisplayLayout.MaxBandDepth = 1;
            this.grdAirframe.DisplayLayout.MaxColScrollRegions = 1;
            this.grdAirframe.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdAirframe.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdAirframe.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grdAirframe.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdAirframe.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdAirframe.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdAirframe.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdAirframe.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.grdAirframe.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdAirframe.DisplayLayout.Override.CellAppearance = appearance8;
            this.grdAirframe.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdAirframe.DisplayLayout.Override.CellPadding = 0;
            this.grdAirframe.DisplayLayout.Override.ColumnSizingArea = Infragistics.Win.UltraWinGrid.ColumnSizingArea.EntireColumn;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.grdAirframe.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.grdAirframe.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.grdAirframe.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdAirframe.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.grdAirframe.DisplayLayout.Override.RowAppearance = appearance11;
            this.grdAirframe.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdAirframe.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.grdAirframe.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdAirframe.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdAirframe.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdAirframe.Location = new System.Drawing.Point(7, 7);
            this.grdAirframe.Name = "grdAirframe";
            this.grdAirframe.Size = new System.Drawing.Size(577, 312);
            this.grdAirframe.TabIndex = 19;
            this.grdAirframe.Text = "ultraGrid1";
            this.grdAirframe.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdAirframe_AfterCellUpdate);
            this.grdAirframe.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdAirframe_InitializeLayout);
            this.grdAirframe.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdAirframe_CellChange);
            // 
            // dsOrders
            // 
            this.dsOrders.DataSetName = "OrdersDataSet";
            this.dsOrders.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taPriceUnit
            // 
            this.taPriceUnit.ClearBeforeFill = false;
            // 
            // helpLink1
            // 
            this.helpLink1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = "price_unit_manager_dialog.htm";
            this.helpLink1.Location = new System.Drawing.Point(7, 336);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 20;
            // 
            // PriceUnitManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 365);
            this.Controls.Add(this.helpLink1);
            this.Controls.Add(this.grdAirframe);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "PriceUnitManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Price Unit Manager";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.AirframeManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdAirframe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
		private Infragistics.Win.UltraWinGrid.UltraGrid grdAirframe;
        private Data.Datasets.OrdersDataSet dsOrders;
        private Data.Datasets.OrdersDataSetTableAdapters.PriceUnitTableAdapter taPriceUnit;
        private Utilities.HelpLink helpLink1;
	}
}