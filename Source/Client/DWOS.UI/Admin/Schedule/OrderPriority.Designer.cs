namespace DWOS.UI.Admin.Schedule
{
	partial class OrderPriority
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("OrderPriority", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RequiredDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PurchaseOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerWO");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SchedulePriority", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.ConditionValueAppearance conditionValueAppearance1 = new Infragistics.Win.ConditionValueAppearance(new Infragistics.Win.ICondition[] {
            ((Infragistics.Win.ICondition)(new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.LessThan, 3, true, typeof(int)))),
            ((Infragistics.Win.ICondition)(new Infragistics.Win.ConditionGroup(new Infragistics.Win.ICondition[] {
                        ((Infragistics.Win.ICondition)(new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.GreaterThanOrEqualTo, 3, true, typeof(int)))),
                        ((Infragistics.Win.ICondition)(new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.LessThan, 6, true, typeof(int))))}, Infragistics.Win.LogicalOperator.And))),
            ((Infragistics.Win.ICondition)(new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.GreaterThanOrEqualTo, 7, true, typeof(int))))}, new Infragistics.Win.Appearance[] {
            appearance2,
            appearance3,
            appearance4});
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Priority");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WorkStatus");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_OrderProcesses_Order");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_OrderProcesses_Order", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderProcessesID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StepOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Department");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StartDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EndDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EstEndDate");
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
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderPriority));
            this.cboCustomer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.dsSchedule = new DWOS.Data.Datasets.ScheduleDataset();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.grdOrderPriority = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.taOrderPriority = new DWOS.Data.Datasets.ScheduleDatasetTableAdapters.OrderPriorityTableAdapter();
            this.taCustomerSummary = new DWOS.Data.Datasets.ScheduleDatasetTableAdapters.CustomerSummaryTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsSchedule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdOrderPriority)).BeginInit();
            this.SuspendLayout();
            // 
            // cboCustomer
            // 
            this.cboCustomer.DataMember = "CustomerSummary";
            this.cboCustomer.DataSource = this.dsSchedule;
            this.cboCustomer.DisplayMember = "Name";
            this.cboCustomer.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCustomer.Location = new System.Drawing.Point(29, 376);
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.Size = new System.Drawing.Size(186, 22);
            this.cboCustomer.TabIndex = 1;
            this.cboCustomer.ValueMember = "CustomerID";
            this.cboCustomer.Visible = false;
            // 
            // dsSchedule
            // 
            this.dsSchedule.DataSetName = "ScheduleDataset";
            this.dsSchedule.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(794, 366);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(712, 366);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grdOrderPriority
            // 
            this.grdOrderPriority.AllowDrop = true;
            this.grdOrderPriority.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdOrderPriority.DataMember = "OrderPriority";
            this.grdOrderPriority.DataSource = this.dsSchedule;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdOrderPriority.DisplayLayout.Appearance = appearance1;
            this.grdOrderPriority.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn1.Header.Caption = "WO";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 129;
            ultraGridColumn2.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn2.Header.Caption = "Required";
            ultraGridColumn2.Header.VisiblePosition = 5;
            ultraGridColumn2.Width = 89;
            ultraGridColumn3.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn3.Header.Caption = "PO";
            ultraGridColumn3.Header.VisiblePosition = 3;
            ultraGridColumn3.Width = 104;
            ultraGridColumn4.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn4.Header.Caption = "WO";
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridColumn4.Width = 100;
            ultraGridColumn5.DefaultCellValue = "0";
            ultraGridColumn5.Header.Caption = "Schedule Priority";
            ultraGridColumn5.Header.VisiblePosition = 7;
            ultraGridColumn5.MaxValue = 10;
            ultraGridColumn5.MinValue = 0;
            ultraGridColumn5.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.IntegerNonNegativeWithSpin;
            appearance3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            appearance4.ForeColor = System.Drawing.Color.Red;
            ultraGridColumn5.ValueBasedAppearance = conditionValueAppearance1;
            ultraGridColumn5.Width = 89;
            ultraGridColumn6.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn6.EditorComponent = this.cboCustomer;
            ultraGridColumn6.Header.Caption = "Customer";
            ultraGridColumn6.Header.VisiblePosition = 1;
            ultraGridColumn6.Width = 109;
            ultraGridColumn7.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn7.Header.Caption = "Order Priority";
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn7.Width = 89;
            ultraGridColumn8.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn8.Header.Caption = "Status";
            ultraGridColumn8.Header.VisiblePosition = 2;
            ultraGridColumn8.Width = 111;
            ultraGridColumn9.Header.VisiblePosition = 8;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9});
            ultraGridColumn10.Header.VisiblePosition = 0;
            ultraGridColumn10.Width = 112;
            ultraGridColumn11.Header.VisiblePosition = 1;
            ultraGridColumn11.Width = 109;
            ultraGridColumn12.Header.VisiblePosition = 2;
            ultraGridColumn12.Width = 110;
            ultraGridColumn13.Header.VisiblePosition = 3;
            ultraGridColumn13.Width = 104;
            ultraGridColumn14.Header.VisiblePosition = 4;
            ultraGridColumn14.Width = 99;
            ultraGridColumn15.Header.VisiblePosition = 5;
            ultraGridColumn15.Width = 89;
            ultraGridColumn16.Header.VisiblePosition = 6;
            ultraGridColumn16.Width = 89;
            ultraGridColumn17.Header.VisiblePosition = 7;
            ultraGridColumn17.Width = 89;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17});
            this.grdOrderPriority.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdOrderPriority.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdOrderPriority.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdOrderPriority.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance5.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance5.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance5.BorderColor = System.Drawing.SystemColors.Window;
            this.grdOrderPriority.DisplayLayout.GroupByBox.Appearance = appearance5;
            appearance6.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdOrderPriority.DisplayLayout.GroupByBox.BandLabelAppearance = appearance6;
            this.grdOrderPriority.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance7.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance7.BackColor2 = System.Drawing.SystemColors.Control;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance7.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdOrderPriority.DisplayLayout.GroupByBox.PromptAppearance = appearance7;
            this.grdOrderPriority.DisplayLayout.MaxColScrollRegions = 1;
            this.grdOrderPriority.DisplayLayout.MaxRowScrollRegions = 1;
            appearance8.BackColor = System.Drawing.SystemColors.Window;
            appearance8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdOrderPriority.DisplayLayout.Override.ActiveCellAppearance = appearance8;
            appearance9.BackColor = System.Drawing.SystemColors.Highlight;
            appearance9.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdOrderPriority.DisplayLayout.Override.ActiveRowAppearance = appearance9;
            this.grdOrderPriority.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdOrderPriority.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdOrderPriority.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            this.grdOrderPriority.DisplayLayout.Override.CardAreaAppearance = appearance10;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            appearance11.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdOrderPriority.DisplayLayout.Override.CellAppearance = appearance11;
            this.grdOrderPriority.DisplayLayout.Override.CellPadding = 0;
            this.grdOrderPriority.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            appearance12.BackColor = System.Drawing.SystemColors.Control;
            appearance12.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance12.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance12.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance12.BorderColor = System.Drawing.SystemColors.Window;
            this.grdOrderPriority.DisplayLayout.Override.GroupByRowAppearance = appearance12;
            appearance13.TextHAlignAsString = "Left";
            this.grdOrderPriority.DisplayLayout.Override.HeaderAppearance = appearance13;
            this.grdOrderPriority.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdOrderPriority.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance14.BackColor = System.Drawing.SystemColors.Window;
            appearance14.BorderColor = System.Drawing.Color.Silver;
            this.grdOrderPriority.DisplayLayout.Override.RowAppearance = appearance14;
            this.grdOrderPriority.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdOrderPriority.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.SingleAutoDrag;
            appearance15.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdOrderPriority.DisplayLayout.Override.TemplateAddRowAppearance = appearance15;
            this.grdOrderPriority.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdOrderPriority.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdOrderPriority.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdOrderPriority.Location = new System.Drawing.Point(12, 12);
            this.grdOrderPriority.Name = "grdOrderPriority";
            this.grdOrderPriority.Size = new System.Drawing.Size(860, 348);
            this.grdOrderPriority.TabIndex = 0;
            this.grdOrderPriority.Text = "ultraGrid1";
            this.grdOrderPriority.SelectionDrag += new System.ComponentModel.CancelEventHandler(this.grdOrderPriority_SelectionDrag);
            this.grdOrderPriority.MouseMove += new System.Windows.Forms.MouseEventHandler(this.grdOrderPriority_MouseMove);
            this.grdOrderPriority.MouseUp += new System.Windows.Forms.MouseEventHandler(this.grdOrderPriority_MouseUp);
            // 
            // taOrderPriority
            // 
            this.taOrderPriority.ClearBeforeFill = true;
            // 
            // taCustomerSummary
            // 
            this.taCustomerSummary.ClearBeforeFill = true;
            // 
            // OrderPriority
            // 
            this.AcceptButton = this.btnCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(884, 401);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cboCustomer);
            this.Controls.Add(this.grdOrderPriority);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OrderPriority";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Order Priority";
            this.Load += new System.EventHandler(this.OrderPriority_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsSchedule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdOrderPriority)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private DWOS.Data.Datasets.ScheduleDatasetTableAdapters.OrderPriorityTableAdapter taOrderPriority;
		private DWOS.Data.Datasets.ScheduleDataset dsSchedule;
		private Infragistics.Win.UltraWinGrid.UltraGrid grdOrderPriority;
		private DWOS.Data.Datasets.ScheduleDatasetTableAdapters.CustomerSummaryTableAdapter taCustomerSummary;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomer;
		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
	}
}