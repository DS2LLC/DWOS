namespace DWOS.UI.Sales
{
    partial class PriceCalculatorWidget
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton6 = new Infragistics.Win.UltraWinEditors.EditorButton("Refresh");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo10 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The unit price of the part.", Infragistics.Win.ToolTipImage.Default, "Unit Price", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The lot price of the part.", Infragistics.Win.ToolTipImage.Default, "Lot Price", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("Calculator");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Number of parts that can be run in an hour.", Infragistics.Win.ToolTipImage.Default, "Rate (Per Hour)", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton2 = new Infragistics.Win.UltraWinEditors.EditorButton("Calculator");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The markup to charge for each part", Infragistics.Win.ToolTipImage.Default, "Markup Total", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton3 = new Infragistics.Win.UltraWinEditors.EditorButton("Calculator");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The overhead cost associated with the processing of one part", Infragistics.Win.ToolTipImage.Default, "Overhead Cost", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton4 = new Infragistics.Win.UltraWinEditors.EditorButton("Calculator");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The material cost associated with the processing of one part", Infragistics.Win.ToolTipImage.Default, "Material Cost", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton5 = new Infragistics.Win.UltraWinEditors.EditorButton("Calculator");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance("Calculator");
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The labor cost associated with the processing of one part", Infragistics.Win.ToolTipImage.Default, "Labor Cost", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Total Cost", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The targeted customer price of the part.", Infragistics.Win.ToolTipImage.Default, "Target Price", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Price By", Infragistics.Win.DefaultableBoolean.Default);
            this.tooltipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.curEachPrice = new Infragistics.Win.UltraWinEditors.UltraCurrencyEditor();
            this.curLotPrice = new Infragistics.Win.UltraWinEditors.UltraCurrencyEditor();
            this.numRate = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.curMarkupTotal = new Infragistics.Win.UltraWinEditors.UltraCurrencyEditor();
            this.curOverheadCost = new Infragistics.Win.UltraWinEditors.UltraCurrencyEditor();
            this.curMaterialCost = new Infragistics.Win.UltraWinEditors.UltraCurrencyEditor();
            this.curLaborCost = new Infragistics.Win.UltraWinEditors.UltraCurrencyEditor();
            this.curTotalCost = new Infragistics.Win.UltraWinEditors.UltraCurrencyEditor();
            this.curTargetPrice = new Infragistics.Win.UltraWinEditors.UltraCurrencyEditor();
            this.cboPriceBy = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.grpPricing = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraLabel10 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel9 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.curEachPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.curLotPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.curMarkupTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.curOverheadCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.curMaterialCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.curLaborCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.curTotalCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.curTargetPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPriceBy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpPricing)).BeginInit();
            this.grpPricing.SuspendLayout();
            this.SuspendLayout();
            // 
            // tooltipManager
            // 
            this.tooltipManager.ContainingControl = this;
            // 
            // curEachPrice
            // 
            appearance6.Image = global::DWOS.UI.Properties.Resources.Reload_32;
            appearance6.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance6.ImageVAlign = Infragistics.Win.VAlign.Middle;
            editorButton6.Appearance = appearance6;
            editorButton6.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton6.Key = "Refresh";
            this.curEachPrice.ButtonsRight.Add(editorButton6);
            this.curEachPrice.Location = new System.Drawing.Point(111, 187);
            this.curEachPrice.MaskInput = "{currency:6.2}";
            this.curEachPrice.MaxValue = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.curEachPrice.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.curEachPrice.Name = "curEachPrice";
            this.curEachPrice.Size = new System.Drawing.Size(152, 22);
            this.curEachPrice.TabIndex = 8;
            ultraToolTipInfo10.ToolTipText = "The unit price of the part.";
            ultraToolTipInfo10.ToolTipTitle = "Unit Price";
            this.tooltipManager.SetUltraToolTip(this.curEachPrice, ultraToolTipInfo10);
            this.curEachPrice.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.curEachPrice_EditorButtonClick);
            this.curEachPrice.Validated += new System.EventHandler(this.curEachPrice_Validated);
            // 
            // curLotPrice
            // 
            this.curLotPrice.Location = new System.Drawing.Point(388, 187);
            this.curLotPrice.MaskInput = "{currency:6.2}";
            this.curLotPrice.MaxValue = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.curLotPrice.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.curLotPrice.Name = "curLotPrice";
            this.curLotPrice.Size = new System.Drawing.Size(152, 22);
            this.curLotPrice.TabIndex = 9;
            ultraToolTipInfo9.ToolTipText = "The lot price of the part.";
            ultraToolTipInfo9.ToolTipTitle = "Lot Price";
            this.tooltipManager.SetUltraToolTip(this.curLotPrice, ultraToolTipInfo9);
            this.curLotPrice.Validated += new System.EventHandler(this.curLotPrice_Validated);
            // 
            // numRate
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Calculator_16;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            editorButton1.Appearance = appearance1;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton1.Key = "Calculator";
            this.numRate.ButtonsRight.Add(editorButton1);
            this.numRate.Location = new System.Drawing.Point(111, 56);
            this.numRate.MaskInput = "-n,nnn,nnn,nnn.nn";
            this.numRate.Name = "numRate";
            this.numRate.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.numRate.Size = new System.Drawing.Size(152, 22);
            this.numRate.TabIndex = 1;
            ultraToolTipInfo4.ToolTipText = "Number of parts that can be run in an hour.";
            ultraToolTipInfo4.ToolTipTitle = "Rate (Per Hour)";
            this.tooltipManager.SetUltraToolTip(this.numRate, ultraToolTipInfo4);
            this.numRate.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.numRate_EditorButtonClick);
            // 
            // curMarkupTotal
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.Calculator_16;
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Middle;
            editorButton2.Appearance = appearance2;
            editorButton2.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton2.Key = "Calculator";
            this.curMarkupTotal.ButtonsRight.Add(editorButton2);
            this.curMarkupTotal.Location = new System.Drawing.Point(111, 112);
            this.curMarkupTotal.MaskInput = "{currency:-6.4}";
            this.curMarkupTotal.MaxValue = new decimal(new int[] {
            1410065407,
            2,
            0,
            262144});
            this.curMarkupTotal.MinValue = new decimal(new int[] {
            1410065407,
            2,
            0,
            -2147221504});
            this.curMarkupTotal.Name = "curMarkupTotal";
            this.curMarkupTotal.ReadOnly = true;
            this.curMarkupTotal.Size = new System.Drawing.Size(152, 22);
            this.curMarkupTotal.TabIndex = 5;
            ultraToolTipInfo5.ToolTipText = "The markup to charge for each part";
            ultraToolTipInfo5.ToolTipTitle = "Markup Total";
            this.tooltipManager.SetUltraToolTip(this.curMarkupTotal, ultraToolTipInfo5);
            this.curMarkupTotal.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.curMarkupTotal_EditorButtonClick);
            // 
            // curOverheadCost
            // 
            appearance3.Image = global::DWOS.UI.Properties.Resources.Calculator_16;
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance3.ImageVAlign = Infragistics.Win.VAlign.Middle;
            editorButton3.Appearance = appearance3;
            editorButton3.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton3.Key = "Calculator";
            this.curOverheadCost.ButtonsRight.Add(editorButton3);
            this.curOverheadCost.Location = new System.Drawing.Point(388, 84);
            this.curOverheadCost.MaskInput = "{currency:6.4}";
            this.curOverheadCost.MaxValue = new decimal(new int[] {
            1410065407,
            2,
            0,
            262144});
            this.curOverheadCost.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.curOverheadCost.Name = "curOverheadCost";
            this.curOverheadCost.Size = new System.Drawing.Size(152, 22);
            this.curOverheadCost.TabIndex = 4;
            ultraToolTipInfo6.ToolTipText = "The overhead cost associated with the processing of one part";
            ultraToolTipInfo6.ToolTipTitle = "Overhead Cost";
            this.tooltipManager.SetUltraToolTip(this.curOverheadCost, ultraToolTipInfo6);
            this.curOverheadCost.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.curOverheadCost_EditorButtonClick);
            // 
            // curMaterialCost
            // 
            appearance4.Image = global::DWOS.UI.Properties.Resources.Calculator_16;
            appearance4.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance4.ImageVAlign = Infragistics.Win.VAlign.Middle;
            editorButton4.Appearance = appearance4;
            editorButton4.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton4.Key = "Calculator";
            this.curMaterialCost.ButtonsRight.Add(editorButton4);
            this.curMaterialCost.Location = new System.Drawing.Point(111, 84);
            this.curMaterialCost.MaskInput = "{currency:6.4}";
            this.curMaterialCost.MaxValue = new decimal(new int[] {
            1410065407,
            2,
            0,
            262144});
            this.curMaterialCost.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.curMaterialCost.Name = "curMaterialCost";
            this.curMaterialCost.Size = new System.Drawing.Size(152, 22);
            this.curMaterialCost.TabIndex = 3;
            ultraToolTipInfo7.ToolTipText = "The material cost associated with the processing of one part";
            ultraToolTipInfo7.ToolTipTitle = "Material Cost";
            this.tooltipManager.SetUltraToolTip(this.curMaterialCost, ultraToolTipInfo7);
            this.curMaterialCost.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.curMaterialCost_EditorButtonClick);
            // 
            // curLaborCost
            // 
            appearance5.Image = global::DWOS.UI.Properties.Resources.Calculator_16;
            appearance5.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance5.ImageVAlign = Infragistics.Win.VAlign.Middle;
            editorButton5.Appearance = appearance5;
            editorButton5.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton5.Key = "Calculator";
            this.curLaborCost.ButtonsRight.Add(editorButton5);
            this.curLaborCost.Location = new System.Drawing.Point(388, 56);
            this.curLaborCost.MaskInput = "{currency:6.4}";
            this.curLaborCost.MaxValue = new decimal(new int[] {
            1410065407,
            2,
            0,
            262144});
            this.curLaborCost.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.curLaborCost.Name = "curLaborCost";
            this.curLaborCost.Size = new System.Drawing.Size(152, 22);
            this.curLaborCost.TabIndex = 2;
            ultraToolTipInfo8.ToolTipText = "The labor cost associated with the processing of one part";
            ultraToolTipInfo8.ToolTipTitle = "Labor Cost";
            this.tooltipManager.SetUltraToolTip(this.curLaborCost, ultraToolTipInfo8);
            this.curLaborCost.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.curLaborCost_EditorButtonClick);
            // 
            // curTotalCost
            // 
            this.curTotalCost.Location = new System.Drawing.Point(111, 140);
            this.curTotalCost.Name = "curTotalCost";
            this.curTotalCost.ReadOnly = true;
            this.curTotalCost.Size = new System.Drawing.Size(152, 22);
            this.curTotalCost.TabIndex = 6;
            ultraToolTipInfo3.ToolTipTextFormatted = "The total cost of the part.<br/>Calculated by adding labor cost, material cost, o" +
    "verhead cost, and markup.";
            ultraToolTipInfo3.ToolTipTitle = "Total Cost";
            this.tooltipManager.SetUltraToolTip(this.curTotalCost, ultraToolTipInfo3);
            // 
            // curTargetPrice
            // 
            this.curTargetPrice.Location = new System.Drawing.Point(388, 140);
            this.curTargetPrice.Name = "curTargetPrice";
            this.curTargetPrice.Size = new System.Drawing.Size(152, 22);
            this.curTargetPrice.TabIndex = 7;
            ultraToolTipInfo2.ToolTipText = "The targeted customer price of the part.";
            ultraToolTipInfo2.ToolTipTitle = "Target Price";
            this.tooltipManager.SetUltraToolTip(this.curTargetPrice, ultraToolTipInfo2);
            // 
            // cboPriceBy
            // 
            this.cboPriceBy.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboPriceBy.Location = new System.Drawing.Point(111, 28);
            this.cboPriceBy.Name = "cboPriceBy";
            this.cboPriceBy.Size = new System.Drawing.Size(152, 22);
            this.cboPriceBy.TabIndex = 0;
            ultraToolTipInfo1.ToolTipTextFormatted = "Pricing type to use; can be <b>Quantity</b> or <b>Weight</b>.<br/><br/>All option" +
    "s may not be available due to system settings.";
            ultraToolTipInfo1.ToolTipTitle = "Price By";
            this.tooltipManager.SetUltraToolTip(this.cboPriceBy, ultraToolTipInfo1);
            this.cboPriceBy.Validated += new System.EventHandler(this.cboPriceBy_Validated);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel1.Location = new System.Drawing.Point(6, 191);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(75, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Each Price:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel2.Location = new System.Drawing.Point(289, 191);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(65, 15);
            this.ultraLabel2.TabIndex = 2;
            this.ultraLabel2.Text = "Lot Price:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(6, 60);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(99, 15);
            this.ultraLabel3.TabIndex = 4;
            this.ultraLabel3.Text = "Rate (Per Hour):";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(289, 60);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(70, 15);
            this.ultraLabel4.TabIndex = 5;
            this.ultraLabel4.Text = "Labor Cost:";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(3, 88);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(84, 15);
            this.ultraLabel5.TabIndex = 6;
            this.ultraLabel5.Text = "Material Cost:";
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(6, 116);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(51, 15);
            this.ultraLabel6.TabIndex = 7;
            this.ultraLabel6.Text = "Markup:";
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(289, 88);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(93, 15);
            this.ultraLabel7.TabIndex = 8;
            this.ultraLabel7.Text = "Overhead Cost:";
            // 
            // grpPricing
            // 
            this.grpPricing.Controls.Add(this.cboPriceBy);
            this.grpPricing.Controls.Add(this.ultraLabel10);
            this.grpPricing.Controls.Add(this.ultraLabel9);
            this.grpPricing.Controls.Add(this.curTargetPrice);
            this.grpPricing.Controls.Add(this.curTotalCost);
            this.grpPricing.Controls.Add(this.ultraLabel8);
            this.grpPricing.Controls.Add(this.numRate);
            this.grpPricing.Controls.Add(this.curMarkupTotal);
            this.grpPricing.Controls.Add(this.curOverheadCost);
            this.grpPricing.Controls.Add(this.curMaterialCost);
            this.grpPricing.Controls.Add(this.curLaborCost);
            this.grpPricing.Controls.Add(this.curLotPrice);
            this.grpPricing.Controls.Add(this.ultraLabel6);
            this.grpPricing.Controls.Add(this.ultraLabel2);
            this.grpPricing.Controls.Add(this.ultraLabel1);
            this.grpPricing.Controls.Add(this.curEachPrice);
            this.grpPricing.Controls.Add(this.ultraLabel3);
            this.grpPricing.Controls.Add(this.ultraLabel4);
            this.grpPricing.Controls.Add(this.ultraLabel5);
            this.grpPricing.Controls.Add(this.ultraLabel7);
            this.grpPricing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPricing.Location = new System.Drawing.Point(0, 0);
            this.grpPricing.Name = "grpPricing";
            this.grpPricing.Size = new System.Drawing.Size(568, 224);
            this.grpPricing.TabIndex = 6;
            this.grpPricing.Text = "Price";
            // 
            // ultraLabel10
            // 
            this.ultraLabel10.AutoSize = true;
            this.ultraLabel10.Location = new System.Drawing.Point(6, 32);
            this.ultraLabel10.Name = "ultraLabel10";
            this.ultraLabel10.Size = new System.Drawing.Size(55, 15);
            this.ultraLabel10.TabIndex = 11;
            this.ultraLabel10.Text = "Price By:";
            // 
            // ultraLabel9
            // 
            this.ultraLabel9.AutoSize = true;
            this.ultraLabel9.Location = new System.Drawing.Point(289, 144);
            this.ultraLabel9.Name = "ultraLabel9";
            this.ultraLabel9.Size = new System.Drawing.Size(78, 15);
            this.ultraLabel9.TabIndex = 10;
            this.ultraLabel9.Text = "Target Price:";
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(6, 144);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(67, 15);
            this.ultraLabel8.TabIndex = 9;
            this.ultraLabel8.Text = "Total Cost:";
            // 
            // PriceCalculatorWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpPricing);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PriceCalculatorWidget";
            this.Size = new System.Drawing.Size(568, 224);
            ((System.ComponentModel.ISupportInitialize)(this.curEachPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.curLotPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.curMarkupTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.curOverheadCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.curMaterialCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.curLaborCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.curTotalCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.curTargetPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPriceBy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpPricing)).EndInit();
            this.grpPricing.ResumeLayout(false);
            this.grpPricing.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tooltipManager;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraCurrencyEditor curEachPrice;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraCurrencyEditor curLotPrice;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraGroupBox grpPricing;
        private Infragistics.Win.UltraWinEditors.UltraCurrencyEditor curOverheadCost;
        private Infragistics.Win.UltraWinEditors.UltraCurrencyEditor curMaterialCost;
        private Infragistics.Win.UltraWinEditors.UltraCurrencyEditor curLaborCost;
        private Infragistics.Win.UltraWinEditors.UltraCurrencyEditor curMarkupTotal;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numRate;
        private Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private Infragistics.Win.UltraWinEditors.UltraCurrencyEditor curTotalCost;
        private Infragistics.Win.Misc.UltraLabel ultraLabel9;
        private Infragistics.Win.UltraWinEditors.UltraCurrencyEditor curTargetPrice;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboPriceBy;
        private Infragistics.Win.Misc.UltraLabel ultraLabel10;
    }
}
