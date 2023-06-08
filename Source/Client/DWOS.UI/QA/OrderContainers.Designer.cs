namespace DWOS.UI.QA
{
    partial class OrderContainers
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("If checked, the work order label will be printed when the OK button is clicked.", Infragistics.Win.ToolTipImage.Default, "Print WO Label", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo10 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("If checked, the container labels will be printed when the OK button is clicked.", Infragistics.Win.ToolTipImage.Default, "Print Container Labels", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Weigh", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Zero", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Tare", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Access options related to scales.", Infragistics.Win.ToolTipImage.Default, "Scale Options", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Deletes the selected container.", Infragistics.Win.ToolTipImage.Default, "Delete Container", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add a container.", Infragistics.Win.ToolTipImage.Default, "Add Container", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Shows a preview of the selected labels.", Infragistics.Win.ToolTipImage.Default, "Preview", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add a new item to the selected container.", Infragistics.Win.ToolTipImage.Default, "Add Container Item", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("OrderContainers", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderContainerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartQuantity");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsActive");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShipmentPackageTypeID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FK_OrderContainerItem_OrderContainers");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Weight", 0);
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FK_OrderContainerItem_OrderContainers", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderContainerItemID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderContainerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ShipmentPackageTypeID");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderContainers));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.flowPrintPrompt = new System.Windows.Forms.FlowLayoutPanel();
            this.lblPrintPrompt = new Infragistics.Win.Misc.UltraLabel();
            this.chkPrintWOLabels = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkPrintContainerLabels = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.pnlWeigh = new System.Windows.Forms.Panel();
            this.btnWeigh = new Infragistics.Win.Misc.UltraButton();
            this.pnlZero = new System.Windows.Forms.Panel();
            this.btnZero = new Infragistics.Win.Misc.UltraButton();
            this.pnlTare = new System.Windows.Forms.Panel();
            this.btnTare = new Infragistics.Win.Misc.UltraButton();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.btnScaleOptions = new Infragistics.Win.Misc.UltraButton();
            this.btnRemoveProcess = new Infragistics.Win.Misc.UltraButton();
            this.btnAddProcess = new Infragistics.Win.Misc.UltraButton();
            this.btnPreview = new Infragistics.Win.Misc.UltraButton();
            this.btnAddItem = new Infragistics.Win.Misc.UltraButton();
            this.grdContainers = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.bsData = new System.Windows.Forms.BindingSource(this.components);
            this.scaleDataNotReceivedTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            this.flowPrintPrompt.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintWOLabels)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintContainerLabels)).BeginInit();
            this.pnlWeigh.SuspendLayout();
            this.pnlZero.SuspendLayout();
            this.pnlTare.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdContainers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(528, 246);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "&Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(446, 246);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // flowPrintPrompt
            // 
            this.flowPrintPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.flowPrintPrompt.AutoSize = true;
            this.flowPrintPrompt.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowPrintPrompt.Controls.Add(this.lblPrintPrompt);
            this.flowPrintPrompt.Controls.Add(this.chkPrintWOLabels);
            this.flowPrintPrompt.Controls.Add(this.chkPrintContainerLabels);
            this.flowPrintPrompt.Location = new System.Drawing.Point(12, 246);
            this.flowPrintPrompt.Name = "flowPrintPrompt";
            this.flowPrintPrompt.Size = new System.Drawing.Size(315, 23);
            this.inboxControlStyler1.SetStyleSettings(this.flowPrintPrompt, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.flowPrintPrompt.TabIndex = 11;
            this.flowPrintPrompt.WrapContents = false;
            // 
            // lblPrintPrompt
            // 
            this.lblPrintPrompt.AutoSize = true;
            this.lblPrintPrompt.Location = new System.Drawing.Point(3, 5);
            this.lblPrintPrompt.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.lblPrintPrompt.Name = "lblPrintPrompt";
            this.lblPrintPrompt.Size = new System.Drawing.Size(180, 14);
            this.lblPrintPrompt.TabIndex = 56;
            this.lblPrintPrompt.Text = "Print Outside Proc. Rework Labels:";
            // 
            // chkPrintWOLabels
            // 
            this.chkPrintWOLabels.AutoSize = true;
            this.chkPrintWOLabels.Location = new System.Drawing.Point(189, 3);
            this.chkPrintWOLabels.Name = "chkPrintWOLabels";
            this.chkPrintWOLabels.Size = new System.Drawing.Size(41, 17);
            this.chkPrintWOLabels.TabIndex = 1;
            this.chkPrintWOLabels.Text = "WO";
            ultraToolTipInfo9.ToolTipText = "If checked, the work order label will be printed when the OK button is clicked.";
            ultraToolTipInfo9.ToolTipTitle = "Print WO Label";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkPrintWOLabels, ultraToolTipInfo9);
            this.chkPrintWOLabels.CheckedChanged += new System.EventHandler(this.Print_CheckedChanged);
            // 
            // chkPrintContainerLabels
            // 
            this.chkPrintContainerLabels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkPrintContainerLabels.AutoSize = true;
            this.chkPrintContainerLabels.Location = new System.Drawing.Point(236, 3);
            this.chkPrintContainerLabels.Name = "chkPrintContainerLabels";
            this.chkPrintContainerLabels.Size = new System.Drawing.Size(76, 17);
            this.chkPrintContainerLabels.TabIndex = 2;
            this.chkPrintContainerLabels.Text = "Containers";
            ultraToolTipInfo10.ToolTipText = "If checked, the container labels will be printed when the OK button is clicked.";
            ultraToolTipInfo10.ToolTipTitle = "Print Container Labels";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkPrintContainerLabels, ultraToolTipInfo10);
            this.chkPrintContainerLabels.CheckedChanged += new System.EventHandler(this.Print_CheckedChanged);
            // 
            // pnlWeigh
            // 
            this.pnlWeigh.Controls.Add(this.btnWeigh);
            this.pnlWeigh.Location = new System.Drawing.Point(12, 150);
            this.pnlWeigh.Name = "pnlWeigh";
            this.pnlWeigh.Size = new System.Drawing.Size(26, 26);
            this.inboxControlStyler1.SetStyleSettings(this.pnlWeigh, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.pnlWeigh.TabIndex = 5;
            this.pnlWeigh.MouseLeave += new System.EventHandler(this.pnlWeigh_MouseLeave);
            this.pnlWeigh.MouseHover += new System.EventHandler(this.pnlWeigh_MouseHover);
            // 
            // btnWeigh
            // 
            this.btnWeigh.Enabled = false;
            this.btnWeigh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWeigh.Location = new System.Drawing.Point(0, 0);
            this.btnWeigh.Name = "btnWeigh";
            this.btnWeigh.Size = new System.Drawing.Size(26, 26);
            this.btnWeigh.TabIndex = 4;
            this.btnWeigh.Text = "W";
            ultraToolTipInfo8.ToolTipTextFormatted = "Sends a weigh command to the connected scale.<br/>Before using the scale, you mus" +
    "t set it up in DWOS.";
            ultraToolTipInfo8.ToolTipTitle = "Weigh";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnWeigh, ultraToolTipInfo8);
            this.btnWeigh.Click += new System.EventHandler(this.btnWeigh_Click);
            // 
            // pnlZero
            // 
            this.pnlZero.Controls.Add(this.btnZero);
            this.pnlZero.Location = new System.Drawing.Point(12, 182);
            this.pnlZero.Name = "pnlZero";
            this.pnlZero.Size = new System.Drawing.Size(26, 26);
            this.inboxControlStyler1.SetStyleSettings(this.pnlZero, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.pnlZero.TabIndex = 6;
            this.pnlZero.MouseLeave += new System.EventHandler(this.pnlZero_MouseLeave);
            this.pnlZero.MouseHover += new System.EventHandler(this.pnlZero_MouseHover);
            // 
            // btnZero
            // 
            this.btnZero.Enabled = false;
            this.btnZero.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnZero.Location = new System.Drawing.Point(0, 0);
            this.btnZero.Name = "btnZero";
            this.btnZero.Size = new System.Drawing.Size(26, 26);
            this.btnZero.TabIndex = 5;
            this.btnZero.Text = "Z";
            ultraToolTipInfo4.ToolTipTextFormatted = "Sends a zero command to the connected scale.<br/>Before using the scale, you must" +
    " set it up in DWOS.";
            ultraToolTipInfo4.ToolTipTitle = "Zero";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnZero, ultraToolTipInfo4);
            this.btnZero.Click += new System.EventHandler(this.btnZero_Click);
            // 
            // pnlTare
            // 
            this.pnlTare.Controls.Add(this.btnTare);
            this.pnlTare.Location = new System.Drawing.Point(12, 214);
            this.pnlTare.Name = "pnlTare";
            this.pnlTare.Size = new System.Drawing.Size(26, 26);
            this.inboxControlStyler1.SetStyleSettings(this.pnlTare, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.pnlTare.TabIndex = 7;
            this.pnlTare.MouseLeave += new System.EventHandler(this.pnlTare_MouseLeave);
            this.pnlTare.MouseHover += new System.EventHandler(this.pnlTare_MouseHover);
            // 
            // btnTare
            // 
            this.btnTare.Enabled = false;
            this.btnTare.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTare.Location = new System.Drawing.Point(0, 0);
            this.btnTare.Name = "btnTare";
            this.btnTare.Size = new System.Drawing.Size(26, 26);
            this.btnTare.TabIndex = 6;
            this.btnTare.Text = "T";
            ultraToolTipInfo3.ToolTipTextFormatted = "Sends a tare command to the connected scale.<br/>Before using the scale, you must" +
    " set it up in DWOS.";
            ultraToolTipInfo3.ToolTipTitle = "Tare";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnTare, ultraToolTipInfo3);
            this.btnTare.Click += new System.EventHandler(this.btnTare_Click);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // btnScaleOptions
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.Settings_32;
            this.btnScaleOptions.Appearance = appearance2;
            this.btnScaleOptions.AutoSize = true;
            this.btnScaleOptions.Location = new System.Drawing.Point(12, 118);
            this.btnScaleOptions.Name = "btnScaleOptions";
            this.btnScaleOptions.Size = new System.Drawing.Size(26, 26);
            this.btnScaleOptions.TabIndex = 4;
            ultraToolTipInfo5.ToolTipText = "Access options related to scales.";
            ultraToolTipInfo5.ToolTipTitle = "Scale Options";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnScaleOptions, ultraToolTipInfo5);
            this.btnScaleOptions.Click += new System.EventHandler(this.btnScaleOptions_Click);
            // 
            // btnRemoveProcess
            // 
            appearance3.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            this.btnRemoveProcess.Appearance = appearance3;
            this.btnRemoveProcess.AutoSize = true;
            this.btnRemoveProcess.Location = new System.Drawing.Point(12, 86);
            this.btnRemoveProcess.Name = "btnRemoveProcess";
            this.btnRemoveProcess.Size = new System.Drawing.Size(26, 26);
            this.btnRemoveProcess.TabIndex = 3;
            ultraToolTipInfo6.ToolTipText = "Deletes the selected container.";
            ultraToolTipInfo6.ToolTipTitle = "Delete Container";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnRemoveProcess, ultraToolTipInfo6);
            this.btnRemoveProcess.Click += new System.EventHandler(this.btnRemoveProcess_Click);
            // 
            // btnAddProcess
            // 
            appearance4.Image = global::DWOS.UI.Properties.Resources.Add_16;
            this.btnAddProcess.Appearance = appearance4;
            this.btnAddProcess.AutoSize = true;
            this.btnAddProcess.Location = new System.Drawing.Point(12, 22);
            this.btnAddProcess.Name = "btnAddProcess";
            this.btnAddProcess.Size = new System.Drawing.Size(26, 26);
            this.btnAddProcess.TabIndex = 1;
            ultraToolTipInfo7.ToolTipText = "Add a container.";
            ultraToolTipInfo7.ToolTipTitle = "Add Container";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnAddProcess, ultraToolTipInfo7);
            this.btnAddProcess.Click += new System.EventHandler(this.btnAddProcess_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreview.Enabled = false;
            this.btnPreview.Location = new System.Drawing.Point(333, 246);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 23);
            this.btnPreview.TabIndex = 12;
            this.btnPreview.Text = "Preview";
            ultraToolTipInfo2.ToolTipText = "Shows a preview of the selected labels.";
            ultraToolTipInfo2.ToolTipTitle = "Preview";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnPreview, ultraToolTipInfo2);
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnAddItem
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.AddAlt_16;
            this.btnAddItem.Appearance = appearance1;
            this.btnAddItem.AutoSize = true;
            this.btnAddItem.Location = new System.Drawing.Point(12, 54);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(26, 26);
            this.btnAddItem.TabIndex = 2;
            ultraToolTipInfo1.ToolTipText = "Add a new item to the selected container.";
            ultraToolTipInfo1.ToolTipTitle = "Add Container Item";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnAddItem, ultraToolTipInfo1);
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // grdContainers
            // 
            this.grdContainers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdContainers.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.Caption = "Container #";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 64;
            ultraGridColumn2.Header.Caption = "Part Qty";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 93;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Hidden = true;
            ultraGridColumn4.Header.Caption = "Is Active";
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridColumn4.Width = 116;
            ultraGridColumn11.Header.Caption = "Package Type";
            ultraGridColumn11.Header.VisiblePosition = 5;
            ultraGridColumn11.Width = 121;
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn5.DataType = typeof(decimal);
            ultraGridColumn5.Format = "";
            ultraGridColumn5.Header.Caption = "Weight (Lbs)";
            ultraGridColumn5.Header.VisiblePosition = 3;
            ultraGridColumn5.MaxValue = new decimal(new int[] {
            276447231,
            23283,
            0,
            524288});
            ultraGridColumn5.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            ultraGridColumn5.NullText = "0.0";
            ultraGridColumn5.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DoubleNonNegativeWithSpin;
            ultraGridColumn5.Width = 111;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn11,
            ultraGridColumn7,
            ultraGridColumn5});
            ultraGridColumn8.Header.Caption = "Container Item #";
            ultraGridColumn8.Header.VisiblePosition = 0;
            ultraGridColumn8.Width = 294;
            ultraGridColumn9.Header.VisiblePosition = 1;
            ultraGridColumn9.Hidden = true;
            ultraGridColumn9.Width = 144;
            ultraGridColumn12.Header.Caption = "Package Type";
            ultraGridColumn12.Header.VisiblePosition = 2;
            ultraGridColumn12.Width = 192;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn12});
            this.grdContainers.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdContainers.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdContainers.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdContainers.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdContainers.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdContainers.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle;
            this.grdContainers.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdContainers.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdContainers.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdContainers.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.grdContainers.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdContainers.Location = new System.Drawing.Point(44, 12);
            this.grdContainers.Name = "grdContainers";
            this.grdContainers.Size = new System.Drawing.Size(562, 228);
            this.grdContainers.TabIndex = 10;
            this.grdContainers.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdContainers_InitializeLayout);
            this.grdContainers.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdContainers_InitializeRow);
            // 
            // scaleDataNotReceivedTimer
            // 
            this.scaleDataNotReceivedTimer.Interval = 20000;
            this.scaleDataNotReceivedTimer.Tick += new System.EventHandler(this.scaleDataNotReceivedTimer_Tick);
            // 
            // OrderContainers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 281);
            this.Controls.Add(this.btnAddItem);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.pnlTare);
            this.Controls.Add(this.pnlZero);
            this.Controls.Add(this.flowPrintPrompt);
            this.Controls.Add(this.btnScaleOptions);
            this.Controls.Add(this.btnRemoveProcess);
            this.Controls.Add(this.btnAddProcess);
            this.Controls.Add(this.grdContainers);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pnlWeigh);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(634, 320);
            this.Name = "OrderContainers";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Containers";
            this.Load += new System.EventHandler(this.OrderContainers_Load);
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            this.flowPrintPrompt.ResumeLayout(false);
            this.flowPrintPrompt.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintWOLabels)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintContainerLabels)).EndInit();
            this.pnlWeigh.ResumeLayout(false);
            this.pnlZero.ResumeLayout(false);
            this.pnlTare.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdContainers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdContainers;
        protected System.Windows.Forms.BindingSource bsData;
        private Infragistics.Win.Misc.UltraButton btnRemoveProcess;
        private Infragistics.Win.Misc.UltraButton btnAddProcess;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPrintContainerLabels;
        private Infragistics.Win.Misc.UltraButton btnScaleOptions;
        private Infragistics.Win.Misc.UltraButton btnWeigh;
        private Infragistics.Win.Misc.UltraButton btnZero;
        private System.Windows.Forms.Timer scaleDataNotReceivedTimer;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPrintWOLabels;
        private Infragistics.Win.Misc.UltraLabel lblPrintPrompt;
        private System.Windows.Forms.FlowLayoutPanel flowPrintPrompt;
        private Infragistics.Win.Misc.UltraButton btnTare;
        private System.Windows.Forms.Panel pnlWeigh;
        private System.Windows.Forms.Panel pnlZero;
        private System.Windows.Forms.Panel pnlTare;
        private Infragistics.Win.Misc.UltraButton btnPreview;
        private Infragistics.Win.Misc.UltraButton btnAddItem;
    }
}