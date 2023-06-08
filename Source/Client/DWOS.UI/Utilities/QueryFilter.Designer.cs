namespace DWOS.UI.Utilities
{
	partial class QueryFilter
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
			Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
			Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
			Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Column");
			Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
			Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Operator");
			Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
			Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
			Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
			Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DataType");
			Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
			Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
			Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
			Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
			Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
			Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
			Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
			Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
			Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
			Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
			Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
			this.grdFilter = new Infragistics.Win.UltraWinGrid.UltraGrid();
			this.btnCancel = new Infragistics.Win.Misc.UltraButton();
			this.btnOK = new Infragistics.Win.Misc.UltraButton();
			((System.ComponentModel.ISupportInitialize)(this.grdFilter)).BeginInit();
			this.SuspendLayout();
			// 
			// grdFilter
			// 
			this.grdFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			appearance13.BackColor = System.Drawing.SystemColors.Window;
			appearance13.BorderColor = System.Drawing.SystemColors.InactiveCaption;
			this.grdFilter.DisplayLayout.Appearance = appearance13;
			ultraGridColumn1.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
			appearance1.TextHAlignAsString = "Center";
			ultraGridColumn1.Header.Appearance = appearance1;
			ultraGridColumn1.Header.Caption = "Query On";
			ultraGridColumn1.Header.VisiblePosition = 0;
			ultraGridColumn1.Width = 141;
			ultraGridColumn2.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
			appearance2.TextHAlignAsString = "Center";
			ultraGridColumn2.Header.Appearance = appearance2;
			ultraGridColumn2.Header.VisiblePosition = 1;
			ultraGridColumn2.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
			ultraGridColumn2.Width = 95;
			appearance3.TextHAlignAsString = "Center";
			ultraGridColumn3.Header.Appearance = appearance3;
			ultraGridColumn3.Header.VisiblePosition = 2;
			ultraGridColumn3.Width = 135;
			ultraGridColumn4.Header.VisiblePosition = 3;
			ultraGridColumn4.Hidden = true;
			ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4});
			this.grdFilter.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
			this.grdFilter.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
			this.grdFilter.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
			appearance14.BackColor = System.Drawing.SystemColors.ActiveBorder;
			appearance14.BackColor2 = System.Drawing.SystemColors.ControlDark;
			appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
			appearance14.BorderColor = System.Drawing.SystemColors.Window;
			this.grdFilter.DisplayLayout.GroupByBox.Appearance = appearance14;
			appearance15.ForeColor = System.Drawing.SystemColors.GrayText;
			this.grdFilter.DisplayLayout.GroupByBox.BandLabelAppearance = appearance15;
			this.grdFilter.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
			appearance16.BackColor = System.Drawing.SystemColors.ControlLightLight;
			appearance16.BackColor2 = System.Drawing.SystemColors.Control;
			appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
			appearance16.ForeColor = System.Drawing.SystemColors.GrayText;
			this.grdFilter.DisplayLayout.GroupByBox.PromptAppearance = appearance16;
			this.grdFilter.DisplayLayout.MaxColScrollRegions = 1;
			this.grdFilter.DisplayLayout.MaxRowScrollRegions = 1;
			appearance17.BackColor = System.Drawing.SystemColors.Window;
			appearance17.ForeColor = System.Drawing.SystemColors.ControlText;
			this.grdFilter.DisplayLayout.Override.ActiveCellAppearance = appearance17;
			appearance18.BackColor = System.Drawing.SystemColors.Highlight;
			appearance18.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.grdFilter.DisplayLayout.Override.ActiveRowAppearance = appearance18;
			this.grdFilter.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
			this.grdFilter.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
			this.grdFilter.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
			appearance19.BackColor = System.Drawing.SystemColors.Window;
			this.grdFilter.DisplayLayout.Override.CardAreaAppearance = appearance19;
			appearance20.BorderColor = System.Drawing.Color.Silver;
			appearance20.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
			this.grdFilter.DisplayLayout.Override.CellAppearance = appearance20;
			this.grdFilter.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
			this.grdFilter.DisplayLayout.Override.CellPadding = 0;
			appearance21.BackColor = System.Drawing.SystemColors.Control;
			appearance21.BackColor2 = System.Drawing.SystemColors.ControlDark;
			appearance21.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
			appearance21.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
			appearance21.BorderColor = System.Drawing.SystemColors.Window;
			this.grdFilter.DisplayLayout.Override.GroupByRowAppearance = appearance21;
			appearance22.TextHAlignAsString = "Left";
			this.grdFilter.DisplayLayout.Override.HeaderAppearance = appearance22;
			this.grdFilter.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
			this.grdFilter.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
			appearance23.BackColor = System.Drawing.SystemColors.Window;
			appearance23.BorderColor = System.Drawing.Color.Silver;
			this.grdFilter.DisplayLayout.Override.RowAppearance = appearance23;
			this.grdFilter.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
			appearance24.BackColor = System.Drawing.SystemColors.ControlLight;
			this.grdFilter.DisplayLayout.Override.TemplateAddRowAppearance = appearance24;
			this.grdFilter.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
			this.grdFilter.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
			this.grdFilter.Location = new System.Drawing.Point(3, 3);
			this.grdFilter.Name = "grdFilter";
			this.grdFilter.Size = new System.Drawing.Size(417, 240);
			this.grdFilter.TabIndex = 0;
			this.grdFilter.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdFilter_InitializeRow);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(334, 249);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(78, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "&Cancel";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(252, 249);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(76, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "&OK";
			// 
			// QueryFilter
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(424, 284);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.grdFilter);
			this.Name = "QueryFilter";
			this.Text = "Query Filter";
			((System.ComponentModel.ISupportInitialize)(this.grdFilter)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinGrid.UltraGrid grdFilter;
		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
	}
}