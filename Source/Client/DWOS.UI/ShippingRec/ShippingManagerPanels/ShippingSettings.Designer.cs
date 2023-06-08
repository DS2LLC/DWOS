namespace DWOS.UI.ShippingRec.ShippingManagerPanels
{
	partial class ShippingSettings
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("Delete");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo11 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Printer", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo10 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Print COC", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Print Packing Slip", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton2 = new Infragistics.Win.UltraWinEditors.EditorButton("Delete");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Label Printer", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Print Order Label", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Shipping Rollover", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Page description language used by the label printer.", Infragistics.Win.ToolTipImage.Default, "Label Printer Language", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Selecting the Quick Print checkbox will instruct DWOS to Autoprint at this time.", Infragistics.Win.ToolTipImage.Default, "Quick Print", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The DPI to use for printing labels [10 - 600].", Infragistics.Win.ToolTipImage.Default, "Label Printer DPI", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The number of copies to quick print for the bill of lading.", Infragistics.Win.ToolTipImage.Default, "Bill of Lading Copies", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Print Package Label", Infragistics.Win.DefaultableBoolean.Default);
            this.cboPrinter = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.chkPrintCOC = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkPrintShippingManifest = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.cboLabelPrinter = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.chkPrintOrderLabel = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.dteRolloverTime = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.cboLabelPrinterLanguage = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.chkQuickPrint = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.numLabelPrinterDpi = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.numBillOfLadingCopies = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.chkPrintPackageLabel = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.numPackingSlipCount = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.lblBillOfLading = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.cboPrinter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintCOC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintShippingManifest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLabelPrinter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintOrderLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteRolloverTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLabelPrinterLanguage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkQuickPrint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLabelPrinterDpi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBillOfLadingCopies)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintPackageLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPackingSlipCount)).BeginInit();
            this.SuspendLayout();
            // 
            // cboPrinter
            // 
            this.cboPrinter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            editorButton1.Appearance = appearance1;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton1.Key = "Delete";
            this.cboPrinter.ButtonsRight.Add(editorButton1);
            this.cboPrinter.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboPrinter.Location = new System.Drawing.Point(153, 7);
            this.cboPrinter.Name = "cboPrinter";
            this.cboPrinter.NullText = "<None>";
            this.cboPrinter.Size = new System.Drawing.Size(237, 22);
            this.cboPrinter.TabIndex = 1;
            ultraToolTipInfo11.ToolTipTextFormatted = "This is the printer used for printing standard paper documents. If no printer is " +
    "selected then the default printer will be used.";
            ultraToolTipInfo11.ToolTipTitle = "Printer";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboPrinter, ultraToolTipInfo11);
            this.cboPrinter.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.cboPrinter_EditorButtonClick);
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(7, 14);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(84, 15);
            this.ultraLabel8.TabIndex = 24;
            this.ultraLabel8.Text = "Paper Printer:";
            // 
            // chkPrintCOC
            // 
            this.chkPrintCOC.Location = new System.Drawing.Point(153, 175);
            this.chkPrintCOC.Name = "chkPrintCOC";
            this.chkPrintCOC.Size = new System.Drawing.Size(120, 20);
            this.chkPrintCOC.TabIndex = 7;
            this.chkPrintCOC.Text = "Print COC";
            ultraToolTipInfo10.ToolTipTextFormatted = "If checked, the COC will print every time an order is scanned in.";
            ultraToolTipInfo10.ToolTipTitle = "Print COC";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkPrintCOC, ultraToolTipInfo10);
            // 
            // chkPrintShippingManifest
            // 
            this.chkPrintShippingManifest.Location = new System.Drawing.Point(153, 201);
            this.chkPrintShippingManifest.Name = "chkPrintShippingManifest";
            this.chkPrintShippingManifest.Size = new System.Drawing.Size(178, 20);
            this.chkPrintShippingManifest.TabIndex = 8;
            this.chkPrintShippingManifest.Text = "Print Packing Slip";
            ultraToolTipInfo9.ToolTipTextFormatted = "If checked, a packing slip (shipping manifest) will be printed when a package is " +
    "completed.";
            ultraToolTipInfo9.ToolTipTitle = "Print Packing Slip";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkPrintShippingManifest, ultraToolTipInfo9);
            this.chkPrintShippingManifest.CheckedChanged += new System.EventHandler(this.chkPrintShippingManifest_CheckedChanged);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // cboLabelPrinter
            // 
            this.cboLabelPrinter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            editorButton2.Appearance = appearance2;
            editorButton2.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton2.Key = "Delete";
            this.cboLabelPrinter.ButtonsRight.Add(editorButton2);
            this.cboLabelPrinter.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboLabelPrinter.Location = new System.Drawing.Point(153, 35);
            this.cboLabelPrinter.Name = "cboLabelPrinter";
            this.cboLabelPrinter.NullText = "<None>";
            this.cboLabelPrinter.Size = new System.Drawing.Size(237, 22);
            this.cboLabelPrinter.TabIndex = 2;
            ultraToolTipInfo8.ToolTipTextFormatted = "If selected, this is the label printer used to print order labels.";
            ultraToolTipInfo8.ToolTipTitle = "Label Printer";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboLabelPrinter, ultraToolTipInfo8);
            this.cboLabelPrinter.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.cboLabelPrinter_EditorButtonClick);
            // 
            // chkPrintOrderLabel
            // 
            this.chkPrintOrderLabel.Location = new System.Drawing.Point(153, 255);
            this.chkPrintOrderLabel.Name = "chkPrintOrderLabel";
            this.chkPrintOrderLabel.Size = new System.Drawing.Size(120, 20);
            this.chkPrintOrderLabel.TabIndex = 10;
            this.chkPrintOrderLabel.Text = "Print Order Label";
            ultraToolTipInfo7.ToolTipTextFormatted = "If checked, an order label will be printed each time an order is added to a packa" +
    "ge.";
            ultraToolTipInfo7.ToolTipTitle = "Print Order Label";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkPrintOrderLabel, ultraToolTipInfo7);
            // 
            // dteRolloverTime
            // 
            this.dteRolloverTime.DateTime = new System.DateTime(2017, 3, 10, 0, 0, 0, 0);
            this.dteRolloverTime.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.dteRolloverTime.Location = new System.Drawing.Point(153, 119);
            this.dteRolloverTime.MaskInput = "{LOC}hh:mm";
            this.dteRolloverTime.Name = "dteRolloverTime";
            this.dteRolloverTime.Size = new System.Drawing.Size(144, 22);
            this.dteRolloverTime.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.dteRolloverTime.TabIndex = 5;
            ultraToolTipInfo6.ToolTipTextFormatted = "After the defined time of the day, the shipping date will be moved to the next bu" +
    "siness day.";
            ultraToolTipInfo6.ToolTipTitle = "Shipping Rollover";
            this.ultraToolTipManager1.SetUltraToolTip(this.dteRolloverTime, ultraToolTipInfo6);
            this.dteRolloverTime.Value = new System.DateTime(2017, 3, 10, 0, 0, 0, 0);
            // 
            // cboLabelPrinterLanguage
            // 
            this.cboLabelPrinterLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLabelPrinterLanguage.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboLabelPrinterLanguage.Location = new System.Drawing.Point(153, 63);
            this.cboLabelPrinterLanguage.Name = "cboLabelPrinterLanguage";
            this.cboLabelPrinterLanguage.NullText = "<None>";
            this.cboLabelPrinterLanguage.Size = new System.Drawing.Size(237, 22);
            this.cboLabelPrinterLanguage.TabIndex = 3;
            ultraToolTipInfo3.ToolTipText = "Page description language used by the label printer.";
            ultraToolTipInfo3.ToolTipTitle = "Label Printer Language";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboLabelPrinterLanguage, ultraToolTipInfo3);
            // 
            // chkQuickPrint
            // 
            this.chkQuickPrint.Location = new System.Drawing.Point(153, 307);
            this.chkQuickPrint.Name = "chkQuickPrint";
            this.chkQuickPrint.Size = new System.Drawing.Size(120, 20);
            this.chkQuickPrint.TabIndex = 12;
            this.chkQuickPrint.Text = "Quick Print";
            ultraToolTipInfo4.ToolTipText = "Selecting the Quick Print checkbox will instruct DWOS to Autoprint at this time.";
            ultraToolTipInfo4.ToolTipTitle = "Quick Print";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkQuickPrint, ultraToolTipInfo4);
            // 
            // numLabelPrinterDpi
            // 
            this.numLabelPrinterDpi.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numLabelPrinterDpi.Location = new System.Drawing.Point(153, 91);
            this.numLabelPrinterDpi.MaxValue = 600;
            this.numLabelPrinterDpi.MinValue = 10;
            this.numLabelPrinterDpi.Name = "numLabelPrinterDpi";
            this.numLabelPrinterDpi.Size = new System.Drawing.Size(237, 22);
            this.numLabelPrinterDpi.TabIndex = 4;
            ultraToolTipInfo2.ToolTipText = "The DPI to use for printing labels [10 - 600].";
            ultraToolTipInfo2.ToolTipTitle = "Label Printer DPI";
            this.ultraToolTipManager1.SetUltraToolTip(this.numLabelPrinterDpi, ultraToolTipInfo2);
            // 
            // numBillOfLadingCopies
            // 
            this.numBillOfLadingCopies.Location = new System.Drawing.Point(153, 147);
            this.numBillOfLadingCopies.MaxValue = 10;
            this.numBillOfLadingCopies.MinValue = 1;
            this.numBillOfLadingCopies.Name = "numBillOfLadingCopies";
            this.numBillOfLadingCopies.Size = new System.Drawing.Size(144, 22);
            this.numBillOfLadingCopies.TabIndex = 6;
            ultraToolTipInfo1.ToolTipText = "The number of copies to quick print for the bill of lading.";
            ultraToolTipInfo1.ToolTipTitle = "Bill of Lading Copies";
            this.ultraToolTipManager1.SetUltraToolTip(this.numBillOfLadingCopies, ultraToolTipInfo1);
            // 
            // chkPrintPackageLabel
            // 
            this.chkPrintPackageLabel.Location = new System.Drawing.Point(153, 281);
            this.chkPrintPackageLabel.Name = "chkPrintPackageLabel";
            this.chkPrintPackageLabel.Size = new System.Drawing.Size(178, 20);
            this.chkPrintPackageLabel.TabIndex = 11;
            this.chkPrintPackageLabel.Text = "Print Package Label";
            ultraToolTipInfo5.ToolTipTextFormatted = "If checked, a package label will be printed each time a package is added.";
            ultraToolTipInfo5.ToolTipTitle = "Print Package Label";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkPrintPackageLabel, ultraToolTipInfo5);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(7, 42);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(81, 15);
            this.ultraLabel1.TabIndex = 27;
            this.ultraLabel1.Text = "Label Printer:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(7, 123);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(115, 15);
            this.ultraLabel2.TabIndex = 31;
            this.ultraLabel2.Text = "Ship Date Rollover:";
            // 
            // numPackingSlipCount
            // 
            this.numPackingSlipCount.Location = new System.Drawing.Point(173, 227);
            this.numPackingSlipCount.MaxValue = 10;
            this.numPackingSlipCount.MinValue = 1;
            this.numPackingSlipCount.Name = "numPackingSlipCount";
            this.numPackingSlipCount.Size = new System.Drawing.Size(100, 22);
            this.numPackingSlipCount.TabIndex = 9;
            this.numPackingSlipCount.Value = 1;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(7, 67);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(140, 15);
            this.ultraLabel3.TabIndex = 35;
            this.ultraLabel3.Text = "Label Printer Language:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(7, 95);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(106, 15);
            this.ultraLabel4.TabIndex = 36;
            this.ultraLabel4.Text = "Label Printer DPI:";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.Location = new System.Drawing.Point(353, 325);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(28, 10);
            this.ultraLabel5.TabIndex = 37;
            this.ultraLabel5.Text = "ultraLabel5";
            // 
            // lblBillOfLading
            // 
            this.lblBillOfLading.AutoSize = true;
            this.lblBillOfLading.Location = new System.Drawing.Point(7, 151);
            this.lblBillOfLading.Name = "lblBillOfLading";
            this.lblBillOfLading.Size = new System.Drawing.Size(124, 15);
            this.lblBillOfLading.TabIndex = 38;
            this.lblBillOfLading.Text = "Bill of Lading Copies:";
            // 
            // ShippingSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.numBillOfLadingCopies);
            this.Controls.Add(this.lblBillOfLading);
            this.Controls.Add(this.ultraLabel5);
            this.Controls.Add(this.numLabelPrinterDpi);
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.cboLabelPrinterLanguage);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.chkQuickPrint);
            this.Controls.Add(this.numPackingSlipCount);
            this.Controls.Add(this.chkPrintPackageLabel);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.dteRolloverTime);
            this.Controls.Add(this.chkPrintOrderLabel);
            this.Controls.Add(this.cboLabelPrinter);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.chkPrintShippingManifest);
            this.Controls.Add(this.chkPrintCOC);
            this.Controls.Add(this.cboPrinter);
            this.Controls.Add(this.ultraLabel8);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ShippingSettings";
            this.Size = new System.Drawing.Size(400, 373);
            ((System.ComponentModel.ISupportInitialize)(this.cboPrinter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintCOC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintShippingManifest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLabelPrinter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintOrderLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteRolloverTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLabelPrinterLanguage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkQuickPrint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLabelPrinterDpi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBillOfLadingCopies)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintPackageLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPackingSlipCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboPrinter;
		private Infragistics.Win.Misc.UltraLabel ultraLabel8;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPrintCOC;
		private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboLabelPrinter;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPrintShippingManifest;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPrintOrderLabel;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteRolloverTime;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numPackingSlipCount;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkQuickPrint;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboLabelPrinterLanguage;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numLabelPrinterDpi;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numBillOfLadingCopies;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPrintPackageLabel;
        private Infragistics.Win.Misc.UltraLabel lblBillOfLading;
    }
}
