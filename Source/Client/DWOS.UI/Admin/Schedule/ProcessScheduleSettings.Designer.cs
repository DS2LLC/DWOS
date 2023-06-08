namespace DWOS.UI.Admin
{
	partial class ProcessScheduleSettings
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("WorkSchedule", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DepartmentScheduleID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DepartmentID", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShiftNumber", -1, null, 1, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartProduction");
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("WorkSchedule", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Weight");
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
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.grdWorkSchedule = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.numLatenessWeight = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.numProcessWeight = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.numOrderPriortyWeight = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.grdOrderPriorities = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.numCustPriorityNorm = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.numCustPriorityEle = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.numCustPriorityHigh = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.bsScheduleSettings = new System.Windows.Forms.BindingSource(this.components);
            this.ultraTabControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdWorkSchedule)).BeginInit();
            this.ultraTabPageControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLatenessWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProcessWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOrderPriortyWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdOrderPriorities)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCustPriorityNorm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCustPriorityEle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCustPriorityHigh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsScheduleSettings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).BeginInit();
            this.ultraTabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.grdWorkSchedule);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(679, 309);
            // 
            // grdWorkSchedule
            // 
            this.grdWorkSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdWorkSchedule.DisplayLayout.AddNewBox.Hidden = false;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdWorkSchedule.DisplayLayout.Appearance = appearance1;
            this.grdWorkSchedule.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn5.Header.VisiblePosition = 0;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn6.Header.Caption = "Department";
            ultraGridColumn6.Header.VisiblePosition = 1;
            ultraGridColumn6.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            ultraGridColumn6.Width = 210;
            ultraGridColumn7.Header.Caption = "Shift Number";
            ultraGridColumn7.Header.VisiblePosition = 2;
            ultraGridColumn7.Width = 195;
            ultraGridColumn8.Header.Caption = "Production Rate";
            ultraGridColumn8.Header.VisiblePosition = 3;
            ultraGridColumn8.Width = 230;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8});
            this.grdWorkSchedule.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdWorkSchedule.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdWorkSchedule.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.grdWorkSchedule.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdWorkSchedule.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.grdWorkSchedule.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdWorkSchedule.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.grdWorkSchedule.DisplayLayout.MaxColScrollRegions = 1;
            this.grdWorkSchedule.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdWorkSchedule.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdWorkSchedule.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grdWorkSchedule.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            this.grdWorkSchedule.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.grdWorkSchedule.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.grdWorkSchedule.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdWorkSchedule.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdWorkSchedule.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.grdWorkSchedule.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdWorkSchedule.DisplayLayout.Override.CellAppearance = appearance8;
            this.grdWorkSchedule.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdWorkSchedule.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.grdWorkSchedule.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.grdWorkSchedule.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.grdWorkSchedule.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdWorkSchedule.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.grdWorkSchedule.DisplayLayout.Override.RowAppearance = appearance11;
            this.grdWorkSchedule.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdWorkSchedule.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdWorkSchedule.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.grdWorkSchedule.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdWorkSchedule.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdWorkSchedule.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdWorkSchedule.Location = new System.Drawing.Point(3, 3);
            this.grdWorkSchedule.Name = "grdWorkSchedule";
            this.grdWorkSchedule.Size = new System.Drawing.Size(673, 303);
            this.grdWorkSchedule.TabIndex = 1;
            this.grdWorkSchedule.Text = "ultraGrid1";
            this.grdWorkSchedule.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdWorkSchedule_InitializeLayout);
            this.grdWorkSchedule.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.grdWorkSchedule_CellChange);
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.ultraGroupBox1);
            this.ultraTabPageControl2.Controls.Add(this.grdOrderPriorities);
            this.ultraTabPageControl2.Controls.Add(this.ultraGroupBox2);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(679, 309);
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.numLatenessWeight);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Controls.Add(this.numProcessWeight);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Controls.Add(this.numOrderPriortyWeight);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(12, 145);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(202, 131);
            this.ultraGroupBox1.TabIndex = 10;
            this.ultraGroupBox1.Text = "Schedule Weights";
            // 
            // numLatenessWeight
            // 
            this.numLatenessWeight.Location = new System.Drawing.Point(150, 91);
            this.numLatenessWeight.MaskInput = "nn";
            this.numLatenessWeight.MaxValue = 100;
            this.numLatenessWeight.MinValue = -100;
            this.numLatenessWeight.Name = "numLatenessWeight";
            this.numLatenessWeight.Size = new System.Drawing.Size(43, 22);
            this.numLatenessWeight.TabIndex = 6;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 39);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(86, 15);
            this.ultraLabel1.TabIndex = 5;
            this.ultraLabel1.Text = "Order Priority:";
            // 
            // numProcessWeight
            // 
            this.numProcessWeight.Location = new System.Drawing.Point(150, 63);
            this.numProcessWeight.MaskInput = "nn";
            this.numProcessWeight.MaxValue = 100;
            this.numProcessWeight.MinValue = -100;
            this.numProcessWeight.Name = "numProcessWeight";
            this.numProcessWeight.Size = new System.Drawing.Size(43, 22);
            this.numProcessWeight.TabIndex = 4;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(6, 67);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(65, 15);
            this.ultraLabel2.TabIndex = 3;
            this.ultraLabel2.Text = "Processes:";
            // 
            // numOrderPriortyWeight
            // 
            this.numOrderPriortyWeight.Location = new System.Drawing.Point(150, 35);
            this.numOrderPriortyWeight.MaskInput = "nn";
            this.numOrderPriortyWeight.MaxValue = 100;
            this.numOrderPriortyWeight.MinValue = -100;
            this.numOrderPriortyWeight.Name = "numOrderPriortyWeight";
            this.numOrderPriortyWeight.Size = new System.Drawing.Size(43, 22);
            this.numOrderPriortyWeight.TabIndex = 2;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(6, 95);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(59, 15);
            this.ultraLabel3.TabIndex = 1;
            this.ultraLabel3.Text = "Lateness:";
            // 
            // grdOrderPriorities
            // 
            this.grdOrderPriorities.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance13.BackColor = System.Drawing.SystemColors.Window;
            appearance13.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdOrderPriorities.DisplayLayout.Appearance = appearance13;
            this.grdOrderPriorities.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn9.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn9.Header.VisiblePosition = 0;
            ultraGridColumn9.Width = 263;
            ultraGridColumn10.Header.VisiblePosition = 1;
            ultraGridColumn10.MinValueExclusive = -1000;
            ultraGridColumn10.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
            ultraGridColumn10.Width = 155;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn9,
            ultraGridColumn10});
            this.grdOrderPriorities.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdOrderPriorities.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdOrderPriorities.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.True;
            appearance14.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance14.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance14.BorderColor = System.Drawing.SystemColors.Window;
            this.grdOrderPriorities.DisplayLayout.GroupByBox.Appearance = appearance14;
            appearance15.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdOrderPriorities.DisplayLayout.GroupByBox.BandLabelAppearance = appearance15;
            this.grdOrderPriorities.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance16.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance16.BackColor2 = System.Drawing.SystemColors.Control;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance16.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdOrderPriorities.DisplayLayout.GroupByBox.PromptAppearance = appearance16;
            this.grdOrderPriorities.DisplayLayout.MaxColScrollRegions = 1;
            this.grdOrderPriorities.DisplayLayout.MaxRowScrollRegions = 1;
            appearance17.BackColor = System.Drawing.SystemColors.Window;
            appearance17.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdOrderPriorities.DisplayLayout.Override.ActiveCellAppearance = appearance17;
            appearance18.BackColor = System.Drawing.SystemColors.Highlight;
            appearance18.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdOrderPriorities.DisplayLayout.Override.ActiveRowAppearance = appearance18;
            this.grdOrderPriorities.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdOrderPriorities.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdOrderPriorities.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.grdOrderPriorities.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdOrderPriorities.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdOrderPriorities.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance19.BackColor = System.Drawing.SystemColors.Window;
            this.grdOrderPriorities.DisplayLayout.Override.CardAreaAppearance = appearance19;
            appearance20.BorderColor = System.Drawing.Color.Silver;
            appearance20.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdOrderPriorities.DisplayLayout.Override.CellAppearance = appearance20;
            this.grdOrderPriorities.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdOrderPriorities.DisplayLayout.Override.CellPadding = 0;
            appearance21.BackColor = System.Drawing.SystemColors.Control;
            appearance21.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance21.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance21.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance21.BorderColor = System.Drawing.SystemColors.Window;
            this.grdOrderPriorities.DisplayLayout.Override.GroupByRowAppearance = appearance21;
            appearance22.TextHAlignAsString = "Left";
            this.grdOrderPriorities.DisplayLayout.Override.HeaderAppearance = appearance22;
            this.grdOrderPriorities.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdOrderPriorities.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance23.BackColor = System.Drawing.SystemColors.Window;
            appearance23.BorderColor = System.Drawing.Color.Silver;
            this.grdOrderPriorities.DisplayLayout.Override.RowAppearance = appearance23;
            this.grdOrderPriorities.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdOrderPriorities.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance24.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdOrderPriorities.DisplayLayout.Override.TemplateAddRowAppearance = appearance24;
            this.grdOrderPriorities.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdOrderPriorities.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdOrderPriorities.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdOrderPriorities.Location = new System.Drawing.Point(220, 8);
            this.grdOrderPriorities.Name = "grdOrderPriorities";
            this.grdOrderPriorities.Size = new System.Drawing.Size(456, 298);
            this.grdOrderPriorities.TabIndex = 10;
            this.grdOrderPriorities.Text = "Order Priorities";
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Controls.Add(this.numCustPriorityNorm);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel6);
            this.ultraGroupBox2.Controls.Add(this.numCustPriorityEle);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel7);
            this.ultraGroupBox2.Controls.Add(this.numCustPriorityHigh);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel8);
            this.ultraGroupBox2.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox2.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox2.Location = new System.Drawing.Point(12, 8);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(202, 131);
            this.ultraGroupBox2.TabIndex = 9;
            this.ultraGroupBox2.Text = "Customer Priorities";
            // 
            // numCustPriorityNorm
            // 
            this.numCustPriorityNorm.Location = new System.Drawing.Point(150, 91);
            this.numCustPriorityNorm.MaskInput = "nn";
            this.numCustPriorityNorm.MaxValue = 100;
            this.numCustPriorityNorm.MinValue = -100;
            this.numCustPriorityNorm.Name = "numCustPriorityNorm";
            this.numCustPriorityNorm.Size = new System.Drawing.Size(43, 22);
            this.numCustPriorityNorm.TabIndex = 6;
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(6, 39);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(35, 15);
            this.ultraLabel6.TabIndex = 5;
            this.ultraLabel6.Text = "High:";
            // 
            // numCustPriorityEle
            // 
            this.numCustPriorityEle.Location = new System.Drawing.Point(150, 63);
            this.numCustPriorityEle.MaskInput = "nn";
            this.numCustPriorityEle.MaxValue = 100;
            this.numCustPriorityEle.MinValue = -100;
            this.numCustPriorityEle.Name = "numCustPriorityEle";
            this.numCustPriorityEle.Size = new System.Drawing.Size(43, 22);
            this.numCustPriorityEle.TabIndex = 4;
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(6, 67);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(58, 15);
            this.ultraLabel7.TabIndex = 3;
            this.ultraLabel7.Text = "Elevated:";
            // 
            // numCustPriorityHigh
            // 
            this.numCustPriorityHigh.Location = new System.Drawing.Point(150, 35);
            this.numCustPriorityHigh.MaskInput = "nn";
            this.numCustPriorityHigh.MaxValue = 100;
            this.numCustPriorityHigh.MinValue = -100;
            this.numCustPriorityHigh.Name = "numCustPriorityHigh";
            this.numCustPriorityHigh.Size = new System.Drawing.Size(43, 22);
            this.numCustPriorityHigh.TabIndex = 2;
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(6, 95);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(50, 15);
            this.ultraLabel8.TabIndex = 1;
            this.ultraLabel8.Text = "Normal:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(615, 353);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(533, 353);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ultraTabControl1
            // 
            this.ultraTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraTabControl1.Controls.Add(this.ultraTabSharedControlsPage1);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl1);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl2);
            this.ultraTabControl1.Location = new System.Drawing.Point(12, 12);
            this.ultraTabControl1.Name = "ultraTabControl1";
            this.ultraTabControl1.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.ultraTabControl1.Size = new System.Drawing.Size(683, 335);
            this.ultraTabControl1.TabIndex = 12;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "Shifts";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "Priorities";
            this.ultraTabControl1.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(679, 309);
            // 
            // ProcessScheduleSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 388);
            this.Controls.Add(this.ultraTabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ProcessScheduleSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Schedule Settings";
            this.ultraTabPageControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdWorkSchedule)).EndInit();
            this.ultraTabPageControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLatenessWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProcessWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOrderPriortyWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdOrderPriorities)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCustPriorityNorm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCustPriorityEle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCustPriorityHigh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsScheduleSettings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).EndInit();
            this.ultraTabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid grdWorkSchedule;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
		private Infragistics.Win.UltraWinEditors.UltraNumericEditor numCustPriorityNorm;
		private Infragistics.Win.Misc.UltraLabel ultraLabel6;
		private Infragistics.Win.UltraWinEditors.UltraNumericEditor numCustPriorityEle;
		private Infragistics.Win.Misc.UltraLabel ultraLabel7;
		private Infragistics.Win.UltraWinEditors.UltraNumericEditor numCustPriorityHigh;
		private Infragistics.Win.Misc.UltraLabel ultraLabel8;
		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private System.Windows.Forms.BindingSource bsScheduleSettings;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl ultraTabControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdOrderPriorities;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numLatenessWeight;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numProcessWeight;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numOrderPriortyWeight;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
	}
}