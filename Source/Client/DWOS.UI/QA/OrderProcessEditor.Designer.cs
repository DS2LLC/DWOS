namespace DWOS.UI.QA
{
    partial class OrderProcessEditor
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
            DisposeMe();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Copy the selected process.", Infragistics.Win.ToolTipImage.Default, "Copy Process", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Delete the selected work processes.", Infragistics.Win.ToolTipImage.Default, "Delete ", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Move the selected process down in ordering.", Infragistics.Win.ToolTipImage.Default, "Move Down", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Move the selected process up in ordering.", Infragistics.Win.ToolTipImage.Default, "Move Up", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add a new rework process to this order.", Infragistics.Win.ToolTipImage.Default, "Add Rework Process", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Set Current", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Edit the selected processes answers.", Infragistics.Win.ToolTipImage.Default, "Edit Process", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Print Process Sheet", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add a rework process package to this order.", Infragistics.Win.ToolTipImage.Default, "Add Rework Process Package", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StepOrder");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Completed");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Process");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Department");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("COC");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EstCompletedDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StartDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EndDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Amount", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessDurationMinutes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessAlias");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsPaperless", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LoadCapacityQuantity", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LoadCapacityVariance", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LoadCapacityWeight", 3);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MaterialCost", 4);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessingLine", 5);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BurdenCost", 6);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderProcessEditor));
            this.btnCopy = new Infragistics.Win.Misc.UltraButton();
            this.btnRemoveProcess = new Infragistics.Win.Misc.UltraButton();
            this.btnDown = new Infragistics.Win.Misc.UltraButton();
            this.btnUp = new Infragistics.Win.Misc.UltraButton();
            this.btnAddProcess = new Infragistics.Win.Misc.UltraButton();
            this.dsOrders = new DWOS.Data.Datasets.OrdersDataSet();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.btnSetCurrent = new Infragistics.Win.Misc.UltraButton();
            this.btnEdit = new Infragistics.Win.Misc.UltraButton();
            this.btnPrint = new Infragistics.Win.Misc.UltraButton();
            this.btnAddProcessPackage = new Infragistics.Win.Misc.UltraButton();
            this.grdProcesses = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraPictureBox2 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraPictureBox3 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.pnlButtons = new Infragistics.Win.Misc.UltraPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdProcesses)).BeginInit();
            this.pnlButtons.ClientArea.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCopy
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Copy_16;
            this.btnCopy.Appearance = appearance1;
            this.btnCopy.AutoSize = true;
            this.btnCopy.Location = new System.Drawing.Point(0, 192);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(26, 26);
            this.btnCopy.TabIndex = 16;
            ultraToolTipInfo9.ToolTipText = "Copy the selected process.";
            ultraToolTipInfo9.ToolTipTitle = "Copy Process";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnCopy, ultraToolTipInfo9);
            // 
            // btnRemoveProcess
            // 
            appearance16.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            this.btnRemoveProcess.Appearance = appearance16;
            this.btnRemoveProcess.AutoSize = true;
            this.btnRemoveProcess.Location = new System.Drawing.Point(0, 32);
            this.btnRemoveProcess.Name = "btnRemoveProcess";
            this.btnRemoveProcess.Size = new System.Drawing.Size(26, 26);
            this.btnRemoveProcess.TabIndex = 11;
            ultraToolTipInfo3.ToolTipText = "Delete the selected work processes.";
            ultraToolTipInfo3.ToolTipTitle = "Delete ";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnRemoveProcess, ultraToolTipInfo3);
            // 
            // btnDown
            // 
            appearance18.Image = global::DWOS.UI.Properties.Resources.Arrow_Down;
            this.btnDown.Appearance = appearance18;
            this.btnDown.AutoSize = true;
            this.btnDown.Location = new System.Drawing.Point(0, 128);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(26, 26);
            this.btnDown.TabIndex = 14;
            ultraToolTipInfo5.ToolTipText = "Move the selected process down in ordering.";
            ultraToolTipInfo5.ToolTipTitle = "Move Down";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnDown, ultraToolTipInfo5);
            // 
            // btnUp
            // 
            appearance17.Image = global::DWOS.UI.Properties.Resources.Arrow_Up;
            this.btnUp.Appearance = appearance17;
            this.btnUp.AutoSize = true;
            this.btnUp.Location = new System.Drawing.Point(0, 96);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(26, 26);
            this.btnUp.TabIndex = 13;
            ultraToolTipInfo4.ToolTipText = "Move the selected process up in ordering.";
            ultraToolTipInfo4.ToolTipTitle = "Move Up";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnUp, ultraToolTipInfo4);
            // 
            // btnAddProcess
            // 
            appearance15.Image = global::DWOS.UI.Properties.Resources.Add_16;
            this.btnAddProcess.Appearance = appearance15;
            this.btnAddProcess.AutoSize = true;
            this.btnAddProcess.Location = new System.Drawing.Point(0, 0);
            this.btnAddProcess.Name = "btnAddProcess";
            this.btnAddProcess.Size = new System.Drawing.Size(26, 26);
            this.btnAddProcess.TabIndex = 10;
            ultraToolTipInfo2.ToolTipText = "Add a new rework process to this order.";
            ultraToolTipInfo2.ToolTipTitle = "Add Rework Process";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnAddProcess, ultraToolTipInfo2);
            // 
            // dsOrders
            // 
            this.dsOrders.DataSetName = "OrdersDataSet";
            this.dsOrders.EnforceConstraints = false;
            this.dsOrders.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // btnSetCurrent
            // 
            appearance19.Image = global::DWOS.UI.Properties.Resources.Run;
            this.btnSetCurrent.Appearance = appearance19;
            this.btnSetCurrent.AutoSize = true;
            this.btnSetCurrent.Location = new System.Drawing.Point(0, 160);
            this.btnSetCurrent.Name = "btnSetCurrent";
            this.btnSetCurrent.Size = new System.Drawing.Size(26, 26);
            this.btnSetCurrent.TabIndex = 15;
            ultraToolTipInfo6.ToolTipTextFormatted = "Set the selected process as the current process.<br/>Only valid to move back to a" +
    " completed process.<br/>";
            ultraToolTipInfo6.ToolTipTitle = "Set Current";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnSetCurrent, ultraToolTipInfo6);
            // 
            // btnEdit
            // 
            appearance20.Image = global::DWOS.UI.Properties.Resources.Process_16;
            this.btnEdit.Appearance = appearance20;
            this.btnEdit.AutoSize = true;
            this.btnEdit.Location = new System.Drawing.Point(0, 224);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(26, 26);
            this.btnEdit.TabIndex = 17;
            ultraToolTipInfo7.ToolTipText = "Edit the selected processes answers.";
            ultraToolTipInfo7.ToolTipTitle = "Edit Process";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnEdit, ultraToolTipInfo7);
            // 
            // btnPrint
            // 
            appearance21.Image = global::DWOS.UI.Properties.Resources.Print_16;
            this.btnPrint.Appearance = appearance21;
            this.btnPrint.AutoSize = true;
            this.btnPrint.Location = new System.Drawing.Point(0, 256);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(26, 26);
            this.btnPrint.TabIndex = 18;
            ultraToolTipInfo8.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo8.ToolTipTextFormatted");
            ultraToolTipInfo8.ToolTipTitle = "Print Process Sheet";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnPrint, ultraToolTipInfo8);
            // 
            // btnAddProcessPackage
            // 
            appearance14.Image = global::DWOS.UI.Properties.Resources.Package_16;
            this.btnAddProcessPackage.Appearance = appearance14;
            this.btnAddProcessPackage.AutoSize = true;
            this.btnAddProcessPackage.Location = new System.Drawing.Point(0, 64);
            this.btnAddProcessPackage.Name = "btnAddProcessPackage";
            this.btnAddProcessPackage.Size = new System.Drawing.Size(26, 26);
            this.btnAddProcessPackage.TabIndex = 12;
            ultraToolTipInfo1.ToolTipText = "Add a rework process package to this order.";
            ultraToolTipInfo1.ToolTipTitle = "Add Rework Process Package";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnAddProcessPackage, ultraToolTipInfo1);
            // 
            // grdProcesses
            // 
            this.grdProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.BackColor = System.Drawing.SystemColors.Window;
            appearance2.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdProcesses.DisplayLayout.Appearance = appearance2;
            this.grdProcesses.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.Caption = "Order";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 14;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn2.Width = 55;
            ultraGridColumn3.Header.Caption = "Code";
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 14;
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridColumn4.Width = 14;
            ultraGridColumn5.Header.VisiblePosition = 6;
            ultraGridColumn5.Width = 18;
            ultraGridColumn7.Header.Caption = "Process By";
            ultraGridColumn7.Header.VisiblePosition = 9;
            ultraGridColumn7.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownCalendar;
            ultraGridColumn7.Width = 24;
            ultraGridColumn6.Header.VisiblePosition = 7;
            ultraGridColumn6.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownCalendar;
            ultraGridColumn6.Width = 24;
            ultraGridColumn8.Header.VisiblePosition = 8;
            ultraGridColumn8.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownCalendar;
            ultraGridColumn8.Width = 24;
            ultraGridColumn11.Format = "";
            ultraGridColumn11.Header.VisiblePosition = 13;
            ultraGridColumn11.MaskInput = "";
            ultraGridColumn11.Width = 15;
            ultraGridColumn12.Header.Caption = "Duration (Min.)";
            ultraGridColumn12.Header.VisiblePosition = 16;
            ultraGridColumn12.Width = 18;
            ultraGridColumn20.Header.Caption = "Alias";
            ultraGridColumn20.Header.VisiblePosition = 3;
            ultraGridColumn20.Width = 94;
            ultraGridColumn9.DataType = typeof(bool);
            ultraGridColumn9.Header.VisiblePosition = 17;
            ultraGridColumn9.Hidden = true;
            ultraGridColumn9.Width = 112;
            ultraGridColumn13.DataType = typeof(int);
            ultraGridColumn13.Header.Caption = "Capacity (pieces)";
            ultraGridColumn13.Header.VisiblePosition = 11;
            ultraGridColumn13.MaskInput = "";
            ultraGridColumn13.Width = 20;
            ultraGridColumn14.DataType = typeof(decimal);
            ultraGridColumn14.Header.Caption = "Capacity Variance";
            ultraGridColumn14.Header.VisiblePosition = 12;
            ultraGridColumn14.MaskInput = "";
            ultraGridColumn14.Width = 27;
            ultraGridColumn15.DataType = typeof(decimal);
            ultraGridColumn15.Header.Caption = "Capacity (lbs.)";
            ultraGridColumn15.Header.VisiblePosition = 10;
            ultraGridColumn15.Width = 26;
            ultraGridColumn16.DataType = typeof(decimal);
            ultraGridColumn16.Header.Caption = "Material Cost";
            ultraGridColumn16.Header.VisiblePosition = 14;
            ultraGridColumn16.Width = 43;
            ultraGridColumn17.Header.Caption = "Processing Line";
            ultraGridColumn17.Header.VisiblePosition = 5;
            ultraGridColumn17.Width = 38;
            ultraGridColumn18.DataType = typeof(decimal);
            ultraGridColumn18.Header.Caption = "Burden Cost";
            ultraGridColumn18.Header.VisiblePosition = 15;
            ultraGridColumn18.Width = 75;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn7,
            ultraGridColumn6,
            ultraGridColumn8,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn20,
            ultraGridColumn9,
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn18});
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdProcesses.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdProcesses.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance3.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance3.BorderColor = System.Drawing.SystemColors.Window;
            this.grdProcesses.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdProcesses.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.grdProcesses.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdProcesses.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.grdProcesses.DisplayLayout.MaxColScrollRegions = 1;
            this.grdProcesses.DisplayLayout.MaxRowScrollRegions = 1;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            appearance6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grdProcesses.DisplayLayout.Override.ActiveCellAppearance = appearance6;
            appearance7.BackColor = System.Drawing.SystemColors.Highlight;
            appearance7.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdProcesses.DisplayLayout.Override.ActiveRowAppearance = appearance7;
            this.grdProcesses.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdProcesses.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdProcesses.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdProcesses.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grdProcesses.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance8.BackColor = System.Drawing.SystemColors.Window;
            this.grdProcesses.DisplayLayout.Override.CardAreaAppearance = appearance8;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            appearance9.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdProcesses.DisplayLayout.Override.CellAppearance = appearance9;
            this.grdProcesses.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdProcesses.DisplayLayout.Override.CellPadding = 0;
            appearance10.BackColor = System.Drawing.SystemColors.Control;
            appearance10.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance10.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance10.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance10.BorderColor = System.Drawing.SystemColors.Window;
            this.grdProcesses.DisplayLayout.Override.GroupByRowAppearance = appearance10;
            appearance11.TextHAlignAsString = "Left";
            this.grdProcesses.DisplayLayout.Override.HeaderAppearance = appearance11;
            this.grdProcesses.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdProcesses.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance12.BackColor = System.Drawing.SystemColors.Window;
            appearance12.BorderColor = System.Drawing.Color.Silver;
            this.grdProcesses.DisplayLayout.Override.RowAppearance = appearance12;
            this.grdProcesses.DisplayLayout.Override.RowSelectorHeaderStyle = Infragistics.Win.UltraWinGrid.RowSelectorHeaderStyle.ColumnChooserButton;
            this.grdProcesses.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdProcesses.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            appearance13.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdProcesses.DisplayLayout.Override.TemplateAddRowAppearance = appearance13;
            this.grdProcesses.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdProcesses.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdProcesses.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdProcesses.ImageList = this.imageList;
            this.grdProcesses.Location = new System.Drawing.Point(35, 3);
            this.grdProcesses.Name = "grdProcesses";
            this.grdProcesses.Size = new System.Drawing.Size(526, 266);
            this.grdProcesses.TabIndex = 19;
            this.grdProcesses.Text = "ultraGrid1";
            this.grdProcesses.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdProcesses_InitializeLayout);
            this.grdProcesses.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdProcesses_InitializeRow);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "repair");
            this.imageList.Images.SetKeyName(1, "complete");
            this.imageList.Images.SetKeyName(2, "current");
            this.imageList.Images.SetKeyName(3, "fail");
            this.imageList.Images.SetKeyName(4, "paper");
            this.imageList.Images.SetKeyName(5, "paperless");
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(124, 270);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(69, 18);
            this.ultraLabel1.TabIndex = 21;
            this.ultraLabel1.Text = "Complete";
            // 
            // ultraPictureBox1
            // 
            this.ultraPictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraPictureBox1.AutoSize = true;
            this.ultraPictureBox1.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox1.Image = ((object)(resources.GetObject("ultraPictureBox1.Image")));
            this.ultraPictureBox1.Location = new System.Drawing.Point(102, 271);
            this.ultraPictureBox1.Name = "ultraPictureBox1";
            this.ultraPictureBox1.Size = new System.Drawing.Size(16, 16);
            this.ultraPictureBox1.TabIndex = 22;
            // 
            // ultraPictureBox2
            // 
            this.ultraPictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraPictureBox2.AutoSize = true;
            this.ultraPictureBox2.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox2.Image = ((object)(resources.GetObject("ultraPictureBox2.Image")));
            this.ultraPictureBox2.Location = new System.Drawing.Point(198, 271);
            this.ultraPictureBox2.Name = "ultraPictureBox2";
            this.ultraPictureBox2.Size = new System.Drawing.Size(16, 16);
            this.ultraPictureBox2.TabIndex = 24;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(220, 270);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(56, 18);
            this.ultraLabel2.TabIndex = 23;
            this.ultraLabel2.Text = "Current";
            // 
            // ultraPictureBox3
            // 
            this.ultraPictureBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraPictureBox3.AutoSize = true;
            this.ultraPictureBox3.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox3.Image = ((object)(resources.GetObject("ultraPictureBox3.Image")));
            this.ultraPictureBox3.Location = new System.Drawing.Point(283, 271);
            this.ultraPictureBox3.Name = "ultraPictureBox3";
            this.ultraPictureBox3.Size = new System.Drawing.Size(16, 16);
            this.ultraPictureBox3.TabIndex = 26;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(305, 270);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(83, 18);
            this.ultraLabel3.TabIndex = 25;
            this.ultraLabel3.Text = "Not Started";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(37, 270);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(59, 18);
            this.ultraLabel4.TabIndex = 27;
            this.ultraLabel4.Text = "Legend:";
            // 
            // pnlButtons
            // 
            // 
            // pnlButtons.ClientArea
            // 
            this.pnlButtons.ClientArea.Controls.Add(this.btnAddProcessPackage);
            this.pnlButtons.ClientArea.Controls.Add(this.btnAddProcess);
            this.pnlButtons.ClientArea.Controls.Add(this.btnRemoveProcess);
            this.pnlButtons.ClientArea.Controls.Add(this.btnUp);
            this.pnlButtons.ClientArea.Controls.Add(this.btnDown);
            this.pnlButtons.ClientArea.Controls.Add(this.btnSetCurrent);
            this.pnlButtons.ClientArea.Controls.Add(this.btnCopy);
            this.pnlButtons.ClientArea.Controls.Add(this.btnEdit);
            this.pnlButtons.ClientArea.Controls.Add(this.btnPrint);
            this.pnlButtons.Location = new System.Drawing.Point(3, 3);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(26, 284);
            this.pnlButtons.TabIndex = 28;
            // 
            // OrderProcessEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.ultraPictureBox3);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.ultraPictureBox2);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraPictureBox1);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.grdProcesses);
            this.Controls.Add(this.pnlButtons);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "OrderProcessEditor";
            this.Size = new System.Drawing.Size(564, 291);
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdProcesses)).EndInit();
            this.pnlButtons.ClientArea.ResumeLayout(false);
            this.pnlButtons.ClientArea.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnCopy;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.Misc.UltraButton btnRemoveProcess;
        private Infragistics.Win.Misc.UltraButton btnDown;
        private Infragistics.Win.Misc.UltraButton btnUp;
        private Infragistics.Win.Misc.UltraButton btnAddProcess;
        private Data.Datasets.OrdersDataSet dsOrders;
        private Infragistics.Win.Misc.UltraButton btnSetCurrent;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdProcesses;
        private System.Windows.Forms.ImageList imageList;
        private Infragistics.Win.Misc.UltraButton btnEdit;
        private Infragistics.Win.Misc.UltraButton btnPrint;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraPanel pnlButtons;
        private Infragistics.Win.Misc.UltraButton btnAddProcessPackage;
    }
}
