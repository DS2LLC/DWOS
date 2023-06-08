namespace DWOS.UI.Admin
{
	partial class AirframeManager
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("d_Airframe", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AirframeID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ManufacturerID", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartPrefix", -1, null, 1, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsActive");
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AirframeManager));
            this.cboManufacturer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.dsParts = new DWOS.Data.Datasets.PartsDataset();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.taManufacturer = new DWOS.Data.Datasets.PartsDatasetTableAdapters.d_ManufacturerTableAdapter();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.taAirframe = new DWOS.Data.Datasets.PartsDatasetTableAdapters.d_AirframeTableAdapter();
            this.grdAirframe = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            ((System.ComponentModel.ISupportInitialize)(this.cboManufacturer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsParts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdAirframe)).BeginInit();
            this.SuspendLayout();
            // 
            // cboManufacturer
            // 
            this.cboManufacturer.DataMember = "d_Manufacturer";
            this.cboManufacturer.DataSource = this.dsParts;
            this.cboManufacturer.DisplayMember = "ManufacturerID";
            this.cboManufacturer.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboManufacturer.Location = new System.Drawing.Point(220, 210);
            this.cboManufacturer.Name = "cboManufacturer";
            this.cboManufacturer.NullText = "<None>";
            this.cboManufacturer.Size = new System.Drawing.Size(144, 22);
            this.cboManufacturer.TabIndex = 20;
            this.cboManufacturer.ValueMember = "ManufacturerID";
            this.cboManufacturer.Visible = false;
            // 
            // dsParts
            // 
            this.dsParts.DataSetName = "PartsDataset";
            this.dsParts.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(473, 405);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(378, 405);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // taManufacturer
            // 
            this.taManufacturer.ClearBeforeFill = true;
            // 
            // taAirframe
            // 
            this.taAirframe.ClearBeforeFill = true;
            // 
            // grdAirframe
            // 
            this.grdAirframe.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdAirframe.DataMember = "d_Airframe";
            this.grdAirframe.DataSource = this.dsParts;
            this.grdAirframe.DisplayLayout.AddNewBox.Hidden = false;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdAirframe.DisplayLayout.Appearance = appearance1;
            ultraGridBand1.AddButtonCaption = "Model";
            ultraGridBand1.AddButtonToolTipText = "Add a new Model";
            ultraGridColumn17.Header.Caption = "Model";
            ultraGridColumn17.Header.VisiblePosition = 0;
            ultraGridColumn17.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Nothing;
            ultraGridColumn18.EditorComponent = this.cboManufacturer;
            ultraGridColumn18.Header.Caption = "Manufacturer";
            ultraGridColumn18.Header.VisiblePosition = 1;
            ultraGridColumn18.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Null;
            ultraGridColumn18.NullText = "<None>";
            ultraGridColumn18.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            ultraGridColumn19.Header.Caption = "Part Prefix";
            ultraGridColumn19.Header.VisiblePosition = 2;
            ultraGridColumn20.Header.Caption = "Is Active";
            ultraGridColumn20.Header.ToolTipText = "If checked, the model is still active.";
            ultraGridColumn20.Header.VisiblePosition = 3;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn17,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20});
            ultraGridBand1.Override.InvalidValueBehavior = Infragistics.Win.UltraWinGrid.InvalidValueBehavior.RevertValueAndRetainFocus;
            ultraGridBand1.Override.SupportDataErrorInfo = Infragistics.Win.UltraWinGrid.SupportDataErrorInfo.RowsAndCells;
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
            this.grdAirframe.DisplayLayout.MaxColScrollRegions = 1;
            this.grdAirframe.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdAirframe.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdAirframe.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grdAirframe.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            this.grdAirframe.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
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
            this.grdAirframe.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.grdAirframe.DisplayLayout.Override.SupportDataErrorInfo = Infragistics.Win.UltraWinGrid.SupportDataErrorInfo.RowsAndCells;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdAirframe.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.grdAirframe.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdAirframe.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdAirframe.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdAirframe.Location = new System.Drawing.Point(12, 12);
            this.grdAirframe.Name = "grdAirframe";
            this.grdAirframe.Size = new System.Drawing.Size(554, 387);
            this.grdAirframe.TabIndex = 19;
            this.grdAirframe.Text = "ultraGrid1";
            this.grdAirframe.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.grdAirframe.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdAirframe_InitializeLayout);
            this.grdAirframe.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdAirframe_InitializeRow);
            this.grdAirframe.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.grdAirframe_BeforeCellUpdate);
            this.grdAirframe.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.grdAirframe_BeforeRowsDeleted);
            // 
            // helpLink1
            // 
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = null;
            this.helpLink1.Location = new System.Drawing.Point(13, 412);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 21;
            // 
            // AirframeManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 440);
            this.Controls.Add(this.helpLink1);
            this.Controls.Add(this.cboManufacturer);
            this.Controls.Add(this.grdAirframe);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "AirframeManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Model Manager";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.AirframeManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cboManufacturer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsParts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdAirframe)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private DWOS.Data.Datasets.PartsDataset dsParts;
		private DWOS.Data.Datasets.PartsDatasetTableAdapters.d_ManufacturerTableAdapter taManufacturer;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
		private DWOS.Data.Datasets.PartsDatasetTableAdapters.d_AirframeTableAdapter taAirframe;
		private Infragistics.Win.UltraWinGrid.UltraGrid grdAirframe;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboManufacturer;
        private Utilities.HelpLink helpLink1;
	}
}