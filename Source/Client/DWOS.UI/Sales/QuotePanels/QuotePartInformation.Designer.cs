namespace DWOS.UI.Sales
{
    using Infragistics.Win.UltraWinEditors;
    using Infragistics.Win.UltraWinMaskedEdit;

    partial class QuotePartInformation
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
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The number of parts in the quote.", Infragistics.Win.ToolTipImage.Default, "Part Quantity", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("If checked, then this part will be required to go through part marking after the " +
        "COC is completed.", Infragistics.Win.ToolTipImage.Default, "Part Marking", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The weight of the part in pounds.", Infragistics.Win.ToolTipImage.Default, "Weight", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The name of the part.", Infragistics.Win.ToolTipImage.Default, "Quote Part Name", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuotePartInformation));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The total quoted weight in pounds.", Infragistics.Win.ToolTipImage.Default, "Total Weight", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Additional fees added to the quote.", Infragistics.Win.ToolTipImage.Default, "Fees", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The total price for the quoted part.", Infragistics.Win.ToolTipImage.Default, "Total Price", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo10 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Additional info about the part.", Infragistics.Win.ToolTipImage.Default, "Notes", Infragistics.Win.DefaultableBoolean.Default);
            this.numPartQty = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel18 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.chkPartMarking = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.mediaWidget = new DWOS.UI.Utilities.MediaWidget();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.partShapeWidget = new DWOS.UI.Utilities.PartShapeWidget();
            this.numWeight = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.lblWeight = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel21 = new Infragistics.Win.Misc.UltraLabel();
            this.calcPriceWidget = new DWOS.UI.Sales.PriceCalculatorWidget();
            this.txtName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.pnlPartInfo = new Infragistics.Win.Misc.UltraPanel();
            this.picQuantityDesync = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.numTotalWeight = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.picQuantitySync = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.pnlFee = new Infragistics.Win.Misc.UltraPanel();
            this.txtFeesTotal = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.pnlTotalPrice = new Infragistics.Win.Misc.UltraPanel();
            this.txtTotal = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.pnlMedia = new Infragistics.Win.Misc.UltraPanel();
            this.pnlProcess = new Infragistics.Win.Misc.UltraPanel();
            this.processWidget = new DWOS.UI.Sales.QuoteProcessWidget();
            this.processPriceWidget = new DWOS.UI.Sales.QuoteProcessPriceWidget();
            this.ultraFlowLayoutManager = new Infragistics.Win.Misc.UltraFlowLayoutManager(this.components);
            this.ultraMessageBoxManager1 = new Infragistics.Win.UltraMessageBox.UltraMessageBoxManager(this.components);
            this.simplePriceWidget = new DWOS.UI.Sales.PriceWidget();
            this.pnlNotes = new Infragistics.Win.Misc.UltraPanel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtNotes = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.pnlTotals = new Infragistics.Win.Misc.UltraPanel();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPartMarking)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).BeginInit();
            this.pnlPartInfo.ClientArea.SuspendLayout();
            this.pnlPartInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTotalWeight)).BeginInit();
            this.pnlFee.ClientArea.SuspendLayout();
            this.pnlFee.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtFeesTotal)).BeginInit();
            this.pnlTotalPrice.ClientArea.SuspendLayout();
            this.pnlTotalPrice.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotal)).BeginInit();
            this.pnlMedia.ClientArea.SuspendLayout();
            this.pnlMedia.SuspendLayout();
            this.pnlProcess.ClientArea.SuspendLayout();
            this.pnlProcess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFlowLayoutManager)).BeginInit();
            this.pnlNotes.ClientArea.SuspendLayout();
            this.pnlNotes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).BeginInit();
            this.pnlTotals.ClientArea.SuspendLayout();
            this.pnlTotals.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.pnlPartInfo);
            this.grpData.Controls.Add(this.pnlTotals);
            this.grpData.Controls.Add(this.calcPriceWidget);
            this.grpData.Controls.Add(this.simplePriceWidget);
            this.grpData.Controls.Add(this.pnlFee);
            this.grpData.Controls.Add(this.pnlNotes);
            this.grpData.Controls.Add(this.pnlMedia);
            this.grpData.Controls.Add(this.pnlProcess);
            appearance3.Image = global::DWOS.UI.Properties.Resources.Part_16;
            this.grpData.HeaderAppearance = appearance3;
            this.grpData.Size = new System.Drawing.Size(564, 819);
            this.grpData.Text = "Part Information";
            this.grpData.Controls.SetChildIndex(this.pnlProcess, 0);
            this.grpData.Controls.SetChildIndex(this.pnlMedia, 0);
            this.grpData.Controls.SetChildIndex(this.pnlNotes, 0);
            this.grpData.Controls.SetChildIndex(this.pnlFee, 0);
            this.grpData.Controls.SetChildIndex(this.simplePriceWidget, 0);
            this.grpData.Controls.SetChildIndex(this.calcPriceWidget, 0);
            this.grpData.Controls.SetChildIndex(this.pnlTotals, 0);
            this.grpData.Controls.SetChildIndex(this.pnlPartInfo, 0);
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            // 
            // picLockImage
            // 
            this.ultraFlowLayoutManager.SetIncludeInLayout(this.picLockImage, false);
            this.picLockImage.Location = new System.Drawing.Point(5057, 12221);
            // 
            // numPartQty
            // 
            this.numPartQty.Location = new System.Drawing.Point(94, 1);
            this.numPartQty.MaskInput = "nnn,nnn,nnn";
            this.numPartQty.MinValue = 0;
            this.numPartQty.Name = "numPartQty";
            this.numPartQty.Nullable = true;
            this.numPartQty.PromptChar = ' ';
            this.numPartQty.Size = new System.Drawing.Size(126, 22);
            this.numPartQty.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.OnMouseEnter;
            this.numPartQty.TabIndex = 4;
            ultraToolTipInfo5.ToolTipText = "The number of parts in the quote.";
            ultraToolTipInfo5.ToolTipTitle = "Part Quantity";
            this.tipManager.SetUltraToolTip(this.numPartQty, ultraToolTipInfo5);
            this.numPartQty.Value = 4;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(3, 5);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(29, 15);
            this.ultraLabel2.TabIndex = 80;
            this.ultraLabel2.Text = "Qty:";
            // 
            // ultraLabel18
            // 
            this.ultraLabel18.AutoSize = true;
            this.ultraLabel18.Location = new System.Drawing.Point(0, 4);
            this.ultraLabel18.Name = "ultraLabel18";
            this.ultraLabel18.Size = new System.Drawing.Size(32, 15);
            this.ultraLabel18.TabIndex = 81;
            this.ultraLabel18.Text = "Part:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(0, 0);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(65, 15);
            this.ultraLabel4.TabIndex = 91;
            this.ultraLabel4.Text = "Processes:";
            // 
            // chkPartMarking
            // 
            this.chkPartMarking.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPartMarking.AutoSize = true;
            this.chkPartMarking.BackColor = System.Drawing.Color.Transparent;
            this.chkPartMarking.BackColorInternal = System.Drawing.Color.Transparent;
            this.chkPartMarking.Location = new System.Drawing.Point(425, 4);
            this.chkPartMarking.Name = "chkPartMarking";
            this.chkPartMarking.Size = new System.Drawing.Size(93, 18);
            this.chkPartMarking.TabIndex = 1;
            this.chkPartMarking.Text = "Part Marking";
            ultraToolTipInfo3.ToolTipText = "If checked, then this part will be required to go through part marking after the " +
    "COC is completed.";
            ultraToolTipInfo3.ToolTipTitle = "Part Marking";
            this.tipManager.SetUltraToolTip(this.chkPartMarking, ultraToolTipInfo3);
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(0, 57);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(33, 15);
            this.ultraLabel5.TabIndex = 101;
            this.ultraLabel5.Text = "Size:";
            // 
            // mediaWidget
            // 
            this.mediaWidget.AllowEditing = false;
            this.mediaWidget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mediaWidget.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mediaWidget.Location = new System.Drawing.Point(60, 0);
            this.mediaWidget.Name = "mediaWidget";
            this.mediaWidget.Size = new System.Drawing.Size(490, 116);
            this.mediaWidget.TabIndex = 8;
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(0, 0);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(43, 15);
            this.ultraLabel6.TabIndex = 103;
            this.ultraLabel6.Text = "Media:";
            // 
            // partShapeWidget
            // 
            this.partShapeWidget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.partShapeWidget.CurrentPart = null;
            this.partShapeWidget.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.partShapeWidget.HeightColumnName = "Height";
            this.partShapeWidget.IsRecordLoading = false;
            this.partShapeWidget.LengthColumnName = "Length";
            this.partShapeWidget.Location = new System.Drawing.Point(94, 54);
            this.partShapeWidget.Name = "partShapeWidget";
            this.partShapeWidget.ShapeTypeColumnName = "ShapeType";
            this.partShapeWidget.Size = new System.Drawing.Size(453, 58);
            this.partShapeWidget.SurfaceAreaColumnName = "SurfaceArea";
            this.partShapeWidget.TabIndex = 3;
            this.partShapeWidget.WidthColumnName = "Width";
            this.partShapeWidget.SurfaceAreaChanged += new System.EventHandler(this.partShapeWidget_SurfaceAreaChanged);
            // 
            // numWeight
            // 
            this.numWeight.Location = new System.Drawing.Point(94, 29);
            this.numWeight.MaskInput = "nnnn.nn lbs";
            this.numWeight.MaxValue = 9999.99999999D;
            this.numWeight.MinValue = 0D;
            this.numWeight.Name = "numWeight";
            this.numWeight.Nullable = true;
            this.numWeight.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.numWeight.Size = new System.Drawing.Size(126, 22);
            this.numWeight.TabIndex = 2;
            this.numWeight.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextControl;
            ultraToolTipInfo2.ToolTipText = "The weight of the part in pounds.";
            ultraToolTipInfo2.ToolTipTitle = "Weight";
            this.tipManager.SetUltraToolTip(this.numWeight, ultraToolTipInfo2);
            // 
            // lblWeight
            // 
            this.lblWeight.AutoSize = true;
            this.lblWeight.Location = new System.Drawing.Point(-1, 33);
            this.lblWeight.Name = "lblWeight";
            this.lblWeight.Size = new System.Drawing.Size(76, 15);
            this.lblWeight.TabIndex = 1002;
            this.lblWeight.Text = "Part Weight:";
            // 
            // ultraLabel21
            // 
            this.ultraLabel21.AutoSize = true;
            this.ultraLabel21.Location = new System.Drawing.Point(0, 3);
            this.ultraLabel21.Name = "ultraLabel21";
            this.ultraLabel21.Size = new System.Drawing.Size(35, 15);
            this.ultraLabel21.TabIndex = 1005;
            this.ultraLabel21.Text = "Fees:";
            // 
            // calcPriceWidget
            // 
            this.calcPriceWidget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.calcPriceWidget.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.calcPriceWidget.Location = new System.Drawing.Point(13, 171);
            this.calcPriceWidget.Name = "calcPriceWidget";
            this.calcPriceWidget.Size = new System.Drawing.Size(550, 214);
            this.calcPriceWidget.SurfaceArea = 0D;
            this.calcPriceWidget.TabIndex = 2;
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtName.Location = new System.Drawing.Point(94, 0);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(349, 22);
            this.txtName.TabIndex = 0;
            ultraToolTipInfo1.ToolTipText = "The name of the part.";
            ultraToolTipInfo1.ToolTipTitle = "Quote Part Name";
            this.tipManager.SetUltraToolTip(this.txtName, ultraToolTipInfo1);
            this.txtName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtName_KeyUp);
            // 
            // pnlPartInfo
            // 
            this.pnlPartInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // pnlPartInfo.ClientArea
            // 
            this.pnlPartInfo.ClientArea.Controls.Add(this.ultraLabel18);
            this.pnlPartInfo.ClientArea.Controls.Add(this.ultraLabel5);
            this.pnlPartInfo.ClientArea.Controls.Add(this.txtName);
            this.pnlPartInfo.ClientArea.Controls.Add(this.numWeight);
            this.pnlPartInfo.ClientArea.Controls.Add(this.lblWeight);
            this.pnlPartInfo.ClientArea.Controls.Add(this.partShapeWidget);
            this.pnlPartInfo.ClientArea.Controls.Add(this.chkPartMarking);
            this.pnlPartInfo.Location = new System.Drawing.Point(13, 28);
            this.pnlPartInfo.Name = "pnlPartInfo";
            this.pnlPartInfo.Size = new System.Drawing.Size(550, 116);
            this.pnlPartInfo.TabIndex = 0;
            // 
            // picQuantityDesync
            // 
            this.picQuantityDesync.BorderShadowColor = System.Drawing.Color.Empty;
            this.picQuantityDesync.Image = ((object)(resources.GetObject("picQuantityDesync.Image")));
            this.picQuantityDesync.Location = new System.Drawing.Point(230, 4);
            this.picQuantityDesync.Name = "picQuantityDesync";
            this.picQuantityDesync.Size = new System.Drawing.Size(32, 16);
            this.picQuantityDesync.TabIndex = 1005;
            ultraToolTipInfo4.ToolTipTextFormatted = "<b>Quantity and Weight are not synced.</b>";
            this.tipManager.SetUltraToolTip(this.picQuantityDesync, ultraToolTipInfo4);
            this.picQuantityDesync.Visible = false;
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(268, 5);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(81, 15);
            this.ultraLabel7.TabIndex = 1004;
            this.ultraLabel7.Text = "Total Weight:";
            // 
            // numTotalWeight
            // 
            this.numTotalWeight.Location = new System.Drawing.Point(355, 1);
            this.numTotalWeight.MaskInput = "nnnn.nn lbs";
            this.numTotalWeight.MaxValue = new decimal(new int[] {
            276447231,
            23283,
            0,
            524288});
            this.numTotalWeight.MinValue = 0D;
            this.numTotalWeight.Name = "numTotalWeight";
            this.numTotalWeight.Nullable = true;
            this.numTotalWeight.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.numTotalWeight.Size = new System.Drawing.Size(188, 22);
            this.numTotalWeight.TabIndex = 5;
            this.numTotalWeight.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextControl;
            ultraToolTipInfo6.ToolTipText = "The total quoted weight in pounds.";
            ultraToolTipInfo6.ToolTipTitle = "Total Weight";
            this.tipManager.SetUltraToolTip(this.numTotalWeight, ultraToolTipInfo6);
            // 
            // picQuantitySync
            // 
            this.picQuantitySync.BorderShadowColor = System.Drawing.Color.Empty;
            this.picQuantitySync.Image = ((object)(resources.GetObject("picQuantitySync.Image")));
            this.picQuantitySync.Location = new System.Drawing.Point(230, 4);
            this.picQuantitySync.Name = "picQuantitySync";
            this.picQuantitySync.Size = new System.Drawing.Size(32, 16);
            this.picQuantitySync.TabIndex = 1006;
            ultraToolTipInfo7.ToolTipTextFormatted = "<b>Quantity</b> and <b>Weight</b> are synced.<br/>If the part has a weight, DWOS " +
    "will automatically<br/>calculate the quantity or weight if you change <br/>one o" +
    "f them.";
            this.tipManager.SetUltraToolTip(this.picQuantitySync, ultraToolTipInfo7);
            // 
            // pnlFee
            // 
            this.pnlFee.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // pnlFee.ClientArea
            // 
            this.pnlFee.ClientArea.Controls.Add(this.txtFeesTotal);
            this.pnlFee.ClientArea.Controls.Add(this.ultraLabel21);
            this.pnlFee.ClientArea.Controls.Add(this.pnlTotalPrice);
            this.pnlFee.Location = new System.Drawing.Point(13, 474);
            this.pnlFee.Name = "pnlFee";
            this.pnlFee.Size = new System.Drawing.Size(550, 26);
            this.pnlFee.TabIndex = 4;
            // 
            // txtFeesTotal
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Add_16;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            editorButton1.Appearance = appearance1;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.txtFeesTotal.ButtonsRight.Add(editorButton1);
            this.txtFeesTotal.Location = new System.Drawing.Point(94, 1);
            this.txtFeesTotal.Name = "txtFeesTotal";
            this.txtFeesTotal.ReadOnly = true;
            this.txtFeesTotal.Size = new System.Drawing.Size(172, 22);
            this.txtFeesTotal.TabIndex = 1008;
            this.txtFeesTotal.TabStop = false;
            ultraToolTipInfo8.ToolTipText = "Additional fees added to the quote.";
            ultraToolTipInfo8.ToolTipTitle = "Fees";
            this.tipManager.SetUltraToolTip(this.txtFeesTotal, ultraToolTipInfo8);
            this.txtFeesTotal.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.numFeesTotal_EditorButtonClick);
            // 
            // pnlTotalPrice
            // 
            // 
            // pnlTotalPrice.ClientArea
            // 
            this.pnlTotalPrice.ClientArea.Controls.Add(this.txtTotal);
            this.pnlTotalPrice.ClientArea.Controls.Add(this.ultraLabel1);
            this.pnlTotalPrice.Location = new System.Drawing.Point(289, 1);
            this.pnlTotalPrice.Name = "pnlTotalPrice";
            this.pnlTotalPrice.Size = new System.Drawing.Size(212, 22);
            this.pnlTotalPrice.TabIndex = 1007;
            // 
            // txtTotal
            // 
            this.txtTotal.Location = new System.Drawing.Point(44, 0);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(165, 22);
            this.txtTotal.TabIndex = 1007;
            this.txtTotal.TabStop = false;
            ultraToolTipInfo9.ToolTipText = "The total price for the quoted part.";
            ultraToolTipInfo9.ToolTipTitle = "Total Price";
            this.tipManager.SetUltraToolTip(this.txtTotal, ultraToolTipInfo9);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(0, 6);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(38, 15);
            this.ultraLabel1.TabIndex = 1006;
            this.ultraLabel1.Text = "Total:";
            // 
            // pnlMedia
            // 
            this.pnlMedia.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // pnlMedia.ClientArea
            // 
            this.pnlMedia.ClientArea.Controls.Add(this.ultraLabel6);
            this.pnlMedia.ClientArea.Controls.Add(this.mediaWidget);
            this.pnlMedia.Location = new System.Drawing.Point(13, 553);
            this.pnlMedia.Name = "pnlMedia";
            this.pnlMedia.Size = new System.Drawing.Size(550, 120);
            this.pnlMedia.TabIndex = 6;
            // 
            // pnlProcess
            // 
            this.pnlProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // pnlProcess.ClientArea
            // 
            this.pnlProcess.ClientArea.Controls.Add(this.processWidget);
            this.pnlProcess.ClientArea.Controls.Add(this.ultraLabel4);
            this.pnlProcess.ClientArea.Controls.Add(this.processPriceWidget);
            this.pnlProcess.Location = new System.Drawing.Point(13, 673);
            this.pnlProcess.Name = "pnlProcess";
            this.pnlProcess.Size = new System.Drawing.Size(550, 140);
            this.pnlProcess.TabIndex = 7;
            // 
            // processWidget
            // 
            this.processWidget.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.processWidget.Editable = false;
            this.processWidget.Location = new System.Drawing.Point(71, 0);
            this.processWidget.Name = "processWidget";
            this.processWidget.Size = new System.Drawing.Size(473, 137);
            this.processWidget.TabIndex = 9;
            // 
            // processPriceWidget
            // 
            this.processPriceWidget.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.processPriceWidget.Editable = false;
            this.processPriceWidget.Location = new System.Drawing.Point(71, 0);
            this.processPriceWidget.Name = "processPriceWidget";
            this.processPriceWidget.Size = new System.Drawing.Size(473, 137);
            this.processPriceWidget.TabIndex = 10;
            this.processPriceWidget.Visible = false;
            this.processPriceWidget.PricePointChanged += new System.EventHandler<DWOS.UI.Utilities.PricePointChangedEventArgs>(this.processPriceWidget_PricePointChanged);
            this.processPriceWidget.PriceBucketsChanged += new System.EventHandler(this.processPriceWidget_PriceBucketsChanged);
            // 
            // ultraFlowLayoutManager
            // 
            this.ultraFlowLayoutManager.ContainerControl = this.grpData;
            this.ultraFlowLayoutManager.HorizontalAlignment = Infragistics.Win.Layout.DefaultableFlowLayoutAlignment.Near;
            this.ultraFlowLayoutManager.HorizontalGap = 0;
            this.ultraFlowLayoutManager.Margins.Left = 5;
            this.ultraFlowLayoutManager.Margins.Right = 5;
            this.ultraFlowLayoutManager.Margins.Top = 5;
            this.ultraFlowLayoutManager.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.ultraFlowLayoutManager.VerticalAlignment = Infragistics.Win.Layout.DefaultableFlowLayoutAlignment.Near;
            this.ultraFlowLayoutManager.VerticalGap = 0;
            this.ultraFlowLayoutManager.WrapItems = false;
            // 
            // ultraMessageBoxManager1
            // 
            this.ultraMessageBoxManager1.ContainingControl = this;
            // 
            // simplePriceWidget
            // 
            this.simplePriceWidget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.simplePriceWidget.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.simplePriceWidget.Location = new System.Drawing.Point(13, 385);
            this.simplePriceWidget.Name = "simplePriceWidget";
            this.simplePriceWidget.Size = new System.Drawing.Size(550, 89);
            this.simplePriceWidget.TabIndex = 3;
            this.simplePriceWidget.PriceSyncClicked += new System.EventHandler<DWOS.UI.Utilities.PriceChangedEventArgs>(this.simplePriceWidget_PriceSyncClicked);
            // 
            // pnlNotes
            // 
            this.pnlNotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // pnlNotes.ClientArea
            // 
            this.pnlNotes.ClientArea.Controls.Add(this.ultraLabel3);
            this.pnlNotes.ClientArea.Controls.Add(this.txtNotes);
            this.pnlNotes.Location = new System.Drawing.Point(13, 500);
            this.pnlNotes.Name = "pnlNotes";
            this.pnlNotes.Size = new System.Drawing.Size(550, 53);
            this.pnlNotes.TabIndex = 5;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(3, 3);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel3.TabIndex = 1;
            this.ultraLabel3.Text = "Notes:";
            // 
            // txtNotes
            // 
            this.txtNotes.AcceptsReturn = true;
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotes.Location = new System.Drawing.Point(94, 0);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.NullText = "<Notes>";
            appearance2.ForeColor = System.Drawing.Color.Silver;
            this.txtNotes.NullTextAppearance = appearance2;
            this.txtNotes.Size = new System.Drawing.Size(453, 47);
            this.txtNotes.TabIndex = 0;
            ultraToolTipInfo10.ToolTipText = "Additional info about the part.";
            ultraToolTipInfo10.ToolTipTitle = "Notes";
            this.tipManager.SetUltraToolTip(this.txtNotes, ultraToolTipInfo10);
            // 
            // pnlTotals
            // 
            this.pnlTotals.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // pnlTotals.ClientArea
            // 
            this.pnlTotals.ClientArea.Controls.Add(this.picQuantityDesync);
            this.pnlTotals.ClientArea.Controls.Add(this.ultraLabel2);
            this.pnlTotals.ClientArea.Controls.Add(this.ultraLabel7);
            this.pnlTotals.ClientArea.Controls.Add(this.numPartQty);
            this.pnlTotals.ClientArea.Controls.Add(this.numTotalWeight);
            this.pnlTotals.ClientArea.Controls.Add(this.picQuantitySync);
            this.pnlTotals.Location = new System.Drawing.Point(13, 144);
            this.pnlTotals.Name = "pnlTotals";
            this.pnlTotals.Size = new System.Drawing.Size(550, 27);
            this.pnlTotals.TabIndex = 1;
            // 
            // QuotePartInformation
            // 
            this.MinimumSize = new System.Drawing.Size(570, 825);
            this.Name = "QuotePartInformation";
            this.Size = new System.Drawing.Size(570, 825);
            this.EditableStatusChanged += new System.EventHandler(this.QuotePartInformation_EditableStatusChanged);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPartMarking)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).EndInit();
            this.pnlPartInfo.ClientArea.ResumeLayout(false);
            this.pnlPartInfo.ClientArea.PerformLayout();
            this.pnlPartInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numTotalWeight)).EndInit();
            this.pnlFee.ClientArea.ResumeLayout(false);
            this.pnlFee.ClientArea.PerformLayout();
            this.pnlFee.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtFeesTotal)).EndInit();
            this.pnlTotalPrice.ClientArea.ResumeLayout(false);
            this.pnlTotalPrice.ClientArea.PerformLayout();
            this.pnlTotalPrice.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtTotal)).EndInit();
            this.pnlMedia.ClientArea.ResumeLayout(false);
            this.pnlMedia.ClientArea.PerformLayout();
            this.pnlMedia.ResumeLayout(false);
            this.pnlProcess.ClientArea.ResumeLayout(false);
            this.pnlProcess.ClientArea.PerformLayout();
            this.pnlProcess.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraFlowLayoutManager)).EndInit();
            this.pnlNotes.ClientArea.ResumeLayout(false);
            this.pnlNotes.ClientArea.PerformLayout();
            this.pnlNotes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).EndInit();
            this.pnlTotals.ClientArea.ResumeLayout(false);
            this.pnlTotals.ClientArea.PerformLayout();
            this.pnlTotals.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion
		private Infragistics.Win.UltraWinEditors.UltraNumericEditor numPartQty;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.Misc.UltraLabel ultraLabel18;
		private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPartMarking;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Utilities.MediaWidget mediaWidget;
        private Utilities.PartShapeWidget partShapeWidget;
        private UltraNumericEditor numWeight;
        private Infragistics.Win.Misc.UltraLabel lblWeight;
        private Infragistics.Win.Misc.UltraLabel ultraLabel21;
        private PriceCalculatorWidget calcPriceWidget;
        private UltraTextEditor txtName;
        private Infragistics.Win.Misc.UltraPanel pnlPartInfo;
        private Infragistics.Win.Misc.UltraPanel pnlFee;
        private Infragistics.Win.Misc.UltraPanel pnlMedia;
        private Infragistics.Win.Misc.UltraPanel pnlProcess;
        private Infragistics.Win.Misc.UltraFlowLayoutManager ultraFlowLayoutManager;
        private QuoteProcessWidget processWidget;
        private Infragistics.Win.UltraMessageBox.UltraMessageBoxManager ultraMessageBoxManager1;
        private QuoteProcessPriceWidget processPriceWidget;
        private PriceWidget simplePriceWidget;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraPanel pnlNotes;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private UltraTextEditor txtNotes;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private UltraNumericEditor numTotalWeight;
        private UltraPictureBox picQuantityDesync;
        private UltraPictureBox picQuantitySync;
        private Infragistics.Win.Misc.UltraPanel pnlTotals;
        private Infragistics.Win.Misc.UltraPanel pnlTotalPrice;
        private UltraTextEditor txtTotal;
        private UltraTextEditor txtFeesTotal;
    }
}
