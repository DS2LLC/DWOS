namespace DWOS.UI.Sales
{
    partial class PriceWidget
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Price By", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("Sync");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The lot price of the part.", Infragistics.Win.ToolTipImage.Default, "Lot Price", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton2 = new Infragistics.Win.UltraWinEditors.EditorButton("Sync");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The unit price of the part.", Infragistics.Win.ToolTipImage.Default, "Unit Price", Infragistics.Win.DefaultableBoolean.Default);
            this.grpPrice = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboPriceBy = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel10 = new Infragistics.Win.Misc.UltraLabel();
            this.curLotPrice = new Infragistics.Win.UltraWinEditors.UltraCurrencyEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.curEachPrice = new Infragistics.Win.UltraWinEditors.UltraCurrencyEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grpPrice)).BeginInit();
            this.grpPrice.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboPriceBy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.curLotPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.curEachPrice)).BeginInit();
            this.SuspendLayout();
            // 
            // grpPrice
            // 
            this.grpPrice.Controls.Add(this.cboPriceBy);
            this.grpPrice.Controls.Add(this.ultraLabel10);
            this.grpPrice.Controls.Add(this.curLotPrice);
            this.grpPrice.Controls.Add(this.ultraLabel2);
            this.grpPrice.Controls.Add(this.curEachPrice);
            this.grpPrice.Controls.Add(this.ultraLabel1);
            this.grpPrice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPrice.Location = new System.Drawing.Point(0, 0);
            this.grpPrice.Name = "grpPrice";
            this.grpPrice.Size = new System.Drawing.Size(517, 93);
            this.grpPrice.TabIndex = 0;
            this.grpPrice.Text = "Price";
            // 
            // cboPriceBy
            // 
            this.cboPriceBy.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboPriceBy.Location = new System.Drawing.Point(94, 27);
            this.cboPriceBy.Name = "cboPriceBy";
            this.cboPriceBy.Size = new System.Drawing.Size(126, 22);
            this.cboPriceBy.TabIndex = 0;
            ultraToolTipInfo1.ToolTipTextFormatted = "Pricing type to use; can be <b>Quantity</b> or <b>Weight</b>.<br/><br/>Some optio" +
    "ns may not be available due to system settings.";
            ultraToolTipInfo1.ToolTipTitle = "Price By";
            this.ultraToolTipManager.SetUltraToolTip(this.cboPriceBy, ultraToolTipInfo1);
            this.cboPriceBy.Validated += new System.EventHandler(this.cboPriceBy_Validated);
            // 
            // ultraLabel10
            // 
            this.ultraLabel10.AutoSize = true;
            this.ultraLabel10.Location = new System.Drawing.Point(6, 31);
            this.ultraLabel10.Name = "ultraLabel10";
            this.ultraLabel10.Size = new System.Drawing.Size(55, 15);
            this.ultraLabel10.TabIndex = 13;
            this.ultraLabel10.Text = "Price By:";
            // 
            // curLotPrice
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Reload_32;
            editorButton1.Appearance = appearance1;
            editorButton1.Key = "Sync";
            this.curLotPrice.ButtonsRight.Add(editorButton1);
            this.curLotPrice.Location = new System.Drawing.Point(321, 55);
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
            this.curLotPrice.Size = new System.Drawing.Size(126, 22);
            this.curLotPrice.TabIndex = 2;
            ultraToolTipInfo2.ToolTipText = "The lot price of the part.";
            ultraToolTipInfo2.ToolTipTitle = "Lot Price";
            this.ultraToolTipManager.SetUltraToolTip(this.curLotPrice, ultraToolTipInfo2);
            this.curLotPrice.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.curLotPrice_EditorButtonClick);
            this.curLotPrice.Validated += new System.EventHandler(this.curLotPrice_Validated);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(257, 59);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(58, 15);
            this.ultraLabel2.TabIndex = 2;
            this.ultraLabel2.Text = "Lot Price:";
            // 
            // curEachPrice
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.Reload_32;
            editorButton2.Appearance = appearance2;
            editorButton2.Key = "Sync";
            this.curEachPrice.ButtonsRight.Add(editorButton2);
            this.curEachPrice.Location = new System.Drawing.Point(94, 55);
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
            this.curEachPrice.Size = new System.Drawing.Size(126, 22);
            this.curEachPrice.TabIndex = 1;
            ultraToolTipInfo3.ToolTipText = "The unit price of the part.";
            ultraToolTipInfo3.ToolTipTitle = "Unit Price";
            this.ultraToolTipManager.SetUltraToolTip(this.curEachPrice, ultraToolTipInfo3);
            this.curEachPrice.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.curEachPrice_EditorButtonClick);
            this.curEachPrice.Validated += new System.EventHandler(this.curEachPrice_Validated);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 59);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(68, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Each Price:";
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // PriceWidget
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.grpPrice);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PriceWidget";
            this.Size = new System.Drawing.Size(517, 93);
            ((System.ComponentModel.ISupportInitialize)(this.grpPrice)).EndInit();
            this.grpPrice.ResumeLayout(false);
            this.grpPrice.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboPriceBy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.curLotPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.curEachPrice)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox grpPrice;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraCurrencyEditor curEachPrice;
        private Infragistics.Win.UltraWinEditors.UltraCurrencyEditor curLotPrice;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboPriceBy;
        private Infragistics.Win.Misc.UltraLabel ultraLabel10;
    }
}
