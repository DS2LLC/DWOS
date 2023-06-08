namespace DWOS.UI.Admin.SettingsPanels
{
    partial class SettingsPricingInfo
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Show Process Price Warnings", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Show Process Price Warnings", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsPricingInfo));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Use Volume Discounts", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Part Pricing", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.pnlProcessPriceWarning = new System.Windows.Forms.Panel();
            this.chkProcessPriceWarning = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkVolumeDiscount = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cboPartPricing = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.toolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            this.pnlProcessPriceWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkProcessPriceWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkVolumeDiscount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPartPricing)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.pnlProcessPriceWarning);
            this.ultraGroupBox1.Controls.Add(this.chkVolumeDiscount);
            this.ultraGroupBox1.Controls.Add(this.cboPartPricing);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(460, 120);
            this.ultraGroupBox1.TabIndex = 0;
            this.ultraGroupBox1.Text = "Pricing";
            // 
            // pnlProcessPriceWarning
            // 
            this.pnlProcessPriceWarning.Controls.Add(this.chkProcessPriceWarning);
            this.pnlProcessPriceWarning.Location = new System.Drawing.Point(6, 87);
            this.pnlProcessPriceWarning.Name = "pnlProcessPriceWarning";
            this.pnlProcessPriceWarning.Size = new System.Drawing.Size(188, 18);
            this.pnlProcessPriceWarning.TabIndex = 4;
            ultraToolTipInfo2.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo2.ToolTipTextFormatted");
            ultraToolTipInfo2.ToolTipTitle = "Show Process Price Warnings";
            this.toolTipManager.SetUltraToolTip(this.pnlProcessPriceWarning, ultraToolTipInfo2);
            this.pnlProcessPriceWarning.MouseLeave += new System.EventHandler(this.pnlProcessPriceWarning_MouseLeave);
            this.pnlProcessPriceWarning.MouseHover += new System.EventHandler(this.pnlProcessPriceWarning_MouseHover);
            // 
            // chkProcessPriceWarning
            // 
            this.chkProcessPriceWarning.AutoSize = true;
            this.chkProcessPriceWarning.Enabled = false;
            this.chkProcessPriceWarning.Location = new System.Drawing.Point(0, 0);
            this.chkProcessPriceWarning.Name = "chkProcessPriceWarning";
            this.chkProcessPriceWarning.Size = new System.Drawing.Size(188, 18);
            this.chkProcessPriceWarning.TabIndex = 3;
            this.chkProcessPriceWarning.Text = "Show Process Price Warnings";
            ultraToolTipInfo1.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo1.ToolTipTextFormatted");
            ultraToolTipInfo1.ToolTipTitle = "Show Process Price Warnings";
            this.toolTipManager.SetUltraToolTip(this.chkProcessPriceWarning, ultraToolTipInfo1);
            // 
            // chkVolumeDiscount
            // 
            this.chkVolumeDiscount.AutoSize = true;
            this.chkVolumeDiscount.Location = new System.Drawing.Point(6, 35);
            this.chkVolumeDiscount.Name = "chkVolumeDiscount";
            this.chkVolumeDiscount.Size = new System.Drawing.Size(148, 18);
            this.chkVolumeDiscount.TabIndex = 1;
            this.chkVolumeDiscount.Text = "Use Volume Discounts";
            ultraToolTipInfo3.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo3.ToolTipTextFormatted");
            ultraToolTipInfo3.ToolTipTitle = "Use Volume Discounts";
            this.toolTipManager.SetUltraToolTip(this.chkVolumeDiscount, ultraToolTipInfo3);
            // 
            // cboPartPricing
            // 
            this.cboPartPricing.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboPartPricing.Location = new System.Drawing.Point(86, 59);
            this.cboPartPricing.Name = "cboPartPricing";
            this.cboPartPricing.Size = new System.Drawing.Size(140, 22);
            this.cboPartPricing.TabIndex = 2;
            ultraToolTipInfo4.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo4.ToolTipTextFormatted");
            ultraToolTipInfo4.ToolTipTitle = "Part Pricing";
            this.toolTipManager.SetUltraToolTip(this.cboPartPricing, ultraToolTipInfo4);
            this.cboPartPricing.ValueChanged += new System.EventHandler(this.cboPartPricing_ValueChanged);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 63);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(74, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Part Pricing:";
            // 
            // toolTipManager
            // 
            this.toolTipManager.ContainingControl = this;
            // 
            // SettingsPricingInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(300, 120);
            this.Name = "SettingsPricingInfo";
            this.Size = new System.Drawing.Size(460, 120);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            this.pnlProcessPriceWarning.ResumeLayout(false);
            this.pnlProcessPriceWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkProcessPriceWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkVolumeDiscount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPartPricing)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboPartPricing;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkVolumeDiscount;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager toolTipManager;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkProcessPriceWarning;
        private System.Windows.Forms.Panel pnlProcessPriceWarning;
    }
}
