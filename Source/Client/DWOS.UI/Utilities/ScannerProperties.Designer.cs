namespace DWOS.UI.Utilities
{
    partial class ScannerProperties
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
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Output Format", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScannerProperties));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The compression percentage to use. Smaller percentages produce smaller files but " +
        "may be harder to see.", Infragistics.Win.ToolTipImage.Default, "Compression", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The scanner or webcam to use.", Infragistics.Win.ToolTipImage.Default, "Device", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Resolution", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Show Full UI", Infragistics.Win.DefaultableBoolean.Default);
            this.FormLabel = new Infragistics.Win.Misc.UltraLabel();
            this.optScannerType = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.numJPEGQuality = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtScannerSource = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnPickScannerSource = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.numResolution = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.chkShowFullUI = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.optScannerType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numJPEGQuality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScannerSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numResolution)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShowFullUI)).BeginInit();
            this.SuspendLayout();
            // 
            // FormLabel
            // 
            this.FormLabel.AutoSize = true;
            this.FormLabel.Location = new System.Drawing.Point(12, 17);
            this.FormLabel.Name = "FormLabel";
            this.FormLabel.Size = new System.Drawing.Size(47, 15);
            this.FormLabel.TabIndex = 27;
            this.FormLabel.Text = "Device:";
            // 
            // optScannerType
            // 
            this.optScannerType.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.optScannerType.CheckedIndex = 0;
            valueListItem1.CheckState = System.Windows.Forms.CheckState.Checked;
            valueListItem1.DataValue = "PDF";
            valueListItem1.DisplayText = "PDF";
            valueListItem2.DataValue = "TIFF";
            valueListItem2.DisplayText = "TIFF";
            this.optScannerType.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.optScannerType.ItemSpacingHorizontal = 4;
            this.optScannerType.Location = new System.Drawing.Point(74, 42);
            this.optScannerType.Name = "optScannerType";
            this.optScannerType.Size = new System.Drawing.Size(175, 19);
            this.optScannerType.TabIndex = 2;
            this.optScannerType.Text = "PDF";
            ultraToolTipInfo5.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo5.ToolTipTextFormatted");
            ultraToolTipInfo5.ToolTipTitle = "Output Format";
            this.ultraToolTipManager.SetUltraToolTip(this.optScannerType, ultraToolTipInfo5);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(12, 42);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(49, 15);
            this.ultraLabel1.TabIndex = 30;
            this.ultraLabel1.Text = "Output:";
            // 
            // numJPEGQuality
            // 
            this.numJPEGQuality.Location = new System.Drawing.Point(101, 67);
            this.numJPEGQuality.MaskInput = "nnn";
            this.numJPEGQuality.MaxValue = 100;
            this.numJPEGQuality.MinValue = 50;
            this.numJPEGQuality.Name = "numJPEGQuality";
            this.numJPEGQuality.Size = new System.Drawing.Size(100, 22);
            this.numJPEGQuality.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.numJPEGQuality.TabIndex = 3;
            ultraToolTipInfo4.ToolTipText = "The compression percentage to use. Smaller percentages produce smaller files but " +
    "may be harder to see.";
            ultraToolTipInfo4.ToolTipTitle = "Compression";
            this.ultraToolTipManager.SetUltraToolTip(this.numJPEGQuality, ultraToolTipInfo4);
            this.numJPEGQuality.Value = 80;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(12, 71);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(83, 15);
            this.ultraLabel2.TabIndex = 33;
            this.ultraLabel2.Text = "Compression:";
            // 
            // txtScannerSource
            // 
            this.txtScannerSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            editorButton1.Appearance = appearance1;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2013Button;
            this.txtScannerSource.ButtonsLeft.Add(editorButton1);
            this.txtScannerSource.Location = new System.Drawing.Point(74, 13);
            this.txtScannerSource.Name = "txtScannerSource";
            this.txtScannerSource.ReadOnly = true;
            this.txtScannerSource.Size = new System.Drawing.Size(202, 22);
            this.txtScannerSource.TabIndex = 0;
            ultraToolTipInfo3.ToolTipText = "The scanner or webcam to use.";
            ultraToolTipInfo3.ToolTipTitle = "Device";
            this.ultraToolTipManager.SetUltraToolTip(this.txtScannerSource, ultraToolTipInfo3);
            this.txtScannerSource.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtScannerSource_EditorButtonClick);
            // 
            // btnPickScannerSource
            // 
            this.btnPickScannerSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPickScannerSource.AutoSize = true;
            this.btnPickScannerSource.Location = new System.Drawing.Point(282, 11);
            this.btnPickScannerSource.Name = "btnPickScannerSource";
            this.btnPickScannerSource.Size = new System.Drawing.Size(27, 25);
            this.btnPickScannerSource.TabIndex = 1;
            this.btnPickScannerSource.Text = "...";
            this.btnPickScannerSource.Click += new System.EventHandler(this.btnPickScannerSource_Click);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(12, 99);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(69, 15);
            this.ultraLabel3.TabIndex = 37;
            this.ultraLabel3.Text = "Resolution:";
            // 
            // numResolution
            // 
            this.numResolution.Location = new System.Drawing.Point(101, 95);
            this.numResolution.MaskInput = "nnn";
            this.numResolution.MaxValue = 300;
            this.numResolution.MinValue = 50;
            this.numResolution.Name = "numResolution";
            this.numResolution.Size = new System.Drawing.Size(100, 22);
            this.numResolution.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.numResolution.TabIndex = 4;
            ultraToolTipInfo2.ToolTipTextFormatted = "The resolution (in DPI) to use when scanning images.<br/>Higher resolutions produ" +
    "ce higher-quality images with larger file sizes.";
            ultraToolTipInfo2.ToolTipTitle = "Resolution";
            this.ultraToolTipManager.SetUltraToolTip(this.numResolution, ultraToolTipInfo2);
            this.numResolution.Value = 100;
            // 
            // chkShowFullUI
            // 
            this.chkShowFullUI.Location = new System.Drawing.Point(12, 123);
            this.chkShowFullUI.Name = "chkShowFullUI";
            this.chkShowFullUI.Size = new System.Drawing.Size(120, 20);
            this.chkShowFullUI.TabIndex = 5;
            this.chkShowFullUI.Text = "Show Full UI";
            ultraToolTipInfo1.ToolTipTextFormatted = "If <b>checked</b>:<br/>- Shows scan options before scanning a document<br/> - Sho" +
    "ws an image preview before taking an image from a webcam";
            ultraToolTipInfo1.ToolTipTitle = "Show Full UI";
            this.ultraToolTipManager.SetUltraToolTip(this.chkShowFullUI, ultraToolTipInfo1);
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // ScannerProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.chkShowFullUI);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.numResolution);
            this.Controls.Add(this.btnPickScannerSource);
            this.Controls.Add(this.txtScannerSource);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.numJPEGQuality);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.optScannerType);
            this.Controls.Add(this.FormLabel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ScannerProperties";
            this.Size = new System.Drawing.Size(312, 153);
            ((System.ComponentModel.ISupportInitialize)(this.optScannerType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numJPEGQuality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScannerSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numResolution)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShowFullUI)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel FormLabel;
        private Infragistics.Win.UltraWinEditors.UltraOptionSet optScannerType;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numJPEGQuality;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtScannerSource;
        private Infragistics.Win.Misc.UltraButton btnPickScannerSource;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numResolution;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkShowFullUI;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
    }
}
