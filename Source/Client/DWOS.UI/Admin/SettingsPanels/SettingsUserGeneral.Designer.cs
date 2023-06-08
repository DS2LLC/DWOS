namespace DWOS.UI.Admin.SettingsPanels
{
    partial class SettingsUserGeneral
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Delete WO Media After Upload", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The minimum time (in seconds) to show each tab when using Presentation Mode.", Infragistics.Win.ToolTipImage.Default, "Presentation Mode Speed", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.cboPartMarkType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.chkCleanupMedia = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.numPresentationModeSpeed = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboPartMarkType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCleanupMedia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPresentationModeSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox1.Controls.Add(this.cboPartMarkType);
            this.ultraGroupBox1.Controls.Add(this.chkCleanupMedia);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Controls.Add(this.numPresentationModeSpeed);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(300, 110);
            this.ultraGroupBox1.TabIndex = 0;
            this.ultraGroupBox1.Text = "General User Information";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(6, 77);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(137, 14);
            this.ultraLabel3.TabIndex = 5;
            this.ultraLabel3.Text = "Part Marking Device Type:";
            // 
            // cboPartMarkType
            // 
            this.cboPartMarkType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPartMarkType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboPartMarkType.Location = new System.Drawing.Point(151, 73);
            this.cboPartMarkType.Name = "cboPartMarkType";
            this.cboPartMarkType.Size = new System.Drawing.Size(143, 21);
            this.cboPartMarkType.TabIndex = 4;
            // 
            // chkCleanupMedia
            // 
            this.chkCleanupMedia.AutoSize = true;
            this.chkCleanupMedia.Location = new System.Drawing.Point(8, 50);
            this.chkCleanupMedia.Name = "chkCleanupMedia";
            this.chkCleanupMedia.Size = new System.Drawing.Size(176, 17);
            this.chkCleanupMedia.TabIndex = 3;
            this.chkCleanupMedia.Text = "Delete WO Media After Upload";
            ultraToolTipInfo1.ToolTipTextFormatted = "If <strong>true</strong>, DWOS will delete work order media files after upload.<b" +
    "r/> If <strong>false</strong>, WO media files will stay on your computer.";
            ultraToolTipInfo1.ToolTipTitle = "Delete WO Media After Upload";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkCleanupMedia, ultraToolTipInfo1);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(269, 26);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(25, 14);
            this.ultraLabel2.TabIndex = 2;
            this.ultraLabel2.Text = "sec.";
            // 
            // numPresentationModeSpeed
            // 
            this.numPresentationModeSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numPresentationModeSpeed.Location = new System.Drawing.Point(147, 23);
            this.numPresentationModeSpeed.MinValue = 1;
            this.numPresentationModeSpeed.Name = "numPresentationModeSpeed";
            this.numPresentationModeSpeed.Size = new System.Drawing.Size(116, 21);
            this.numPresentationModeSpeed.TabIndex = 1;
            ultraToolTipInfo2.ToolTipText = "The minimum time (in seconds) to show each tab when using Presentation Mode.";
            ultraToolTipInfo2.ToolTipTitle = "Presentation Mode Speed";
            this.ultraToolTipManager1.SetUltraToolTip(this.numPresentationModeSpeed, ultraToolTipInfo2);
            this.numPresentationModeSpeed.ValidationError += new Infragistics.Win.UltraWinEditors.UltraNumericEditorBase.ValidationErrorEventHandler(this.numPresentationModeSpeed_ValidationError);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 27);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(138, 14);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Presentation Mode Speed:";
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // SettingsUserGeneral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox1);
            this.MinimumSize = new System.Drawing.Size(300, 110);
            this.Name = "SettingsUserGeneral";
            this.Size = new System.Drawing.Size(300, 110);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboPartMarkType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCleanupMedia)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPresentationModeSpeed)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numPresentationModeSpeed;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkCleanupMedia;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboPartMarkType;
    }
}
