namespace DWOS.QBExport.Syncing
{
    partial class SourceSelection
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Use QuickBooks as the source when syncing data.", Infragistics.Win.ToolTipImage.Default, "QuickBooks", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Use DWOS as the source when syncing data.", Infragistics.Win.ToolTipImage.Default, "DWOS", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SourceSelection));
            this.rbQB = new System.Windows.Forms.RadioButton();
            this.rbDWOS = new System.Windows.Forms.RadioButton();
            this.chkMatchMaker = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.chkMatchMaker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbQB
            // 
            this.rbQB.AutoSize = true;
            this.rbQB.Checked = true;
            this.rbQB.Location = new System.Drawing.Point(7, 34);
            this.rbQB.Name = "rbQB";
            this.rbQB.Size = new System.Drawing.Size(92, 17);
            this.rbQB.TabIndex = 0;
            this.rbQB.TabStop = true;
            this.rbQB.Text = "QuickBooks";
            ultraToolTipInfo2.ToolTipText = "Use QuickBooks as the source when syncing data.";
            ultraToolTipInfo2.ToolTipTitle = "QuickBooks";
            this.ultraToolTipManager1.SetUltraToolTip(this.rbQB, ultraToolTipInfo2);
            this.rbQB.UseVisualStyleBackColor = true;
            // 
            // rbDWOS
            // 
            this.rbDWOS.AutoSize = true;
            this.rbDWOS.Location = new System.Drawing.Point(7, 55);
            this.rbDWOS.Name = "rbDWOS";
            this.rbDWOS.Size = new System.Drawing.Size(62, 17);
            this.rbDWOS.TabIndex = 1;
            this.rbDWOS.TabStop = true;
            this.rbDWOS.Text = "DWOS";
            ultraToolTipInfo1.ToolTipText = "Use DWOS as the source when syncing data.";
            ultraToolTipInfo1.ToolTipTitle = "DWOS";
            this.ultraToolTipManager1.SetUltraToolTip(this.rbDWOS, ultraToolTipInfo1);
            this.rbDWOS.UseVisualStyleBackColor = true;
            // 
            // chkMatchMaker
            // 
            this.chkMatchMaker.Location = new System.Drawing.Point(7, 75);
            this.chkMatchMaker.Name = "chkMatchMaker";
            this.chkMatchMaker.Size = new System.Drawing.Size(140, 20);
            this.chkMatchMaker.TabIndex = 2;
            this.chkMatchMaker.Text = "Matchmaker";
            this.chkMatchMaker.Visible = false;
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox1.Controls.Add(this.rbQB);
            this.ultraGroupBox1.Controls.Add(this.chkMatchMaker);
            this.ultraGroupBox1.Controls.Add(this.rbDWOS);
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(3, 57);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(533, 110);
            this.ultraGroupBox1.TabIndex = 3;
            this.ultraGroupBox1.Text = "Source";
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel1.Location = new System.Drawing.Point(66, 4);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(470, 47);
            this.ultraLabel1.TabIndex = 4;
            this.ultraLabel1.Text = "Select the source for the data sync.  Data will be retrieved from the source and " +
    "reconciled with the data at the destination. Data from the selected source will " +
    "overwrite destination data.";
            // 
            // ultraPictureBox1
            // 
            this.ultraPictureBox1.AutoSize = true;
            this.ultraPictureBox1.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox1.Image = ((object)(resources.GetObject("ultraPictureBox1.Image")));
            this.ultraPictureBox1.Location = new System.Drawing.Point(10, 4);
            this.ultraPictureBox1.Name = "ultraPictureBox1";
            this.ultraPictureBox1.Size = new System.Drawing.Size(31, 31);
            this.ultraPictureBox1.TabIndex = 5;
            // 
            // SourceSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraPictureBox1);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.ultraGroupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SourceSelection";
            this.Size = new System.Drawing.Size(540, 170);
            ((System.ComponentModel.ISupportInitialize)(this.chkMatchMaker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbQB;
        private System.Windows.Forms.RadioButton rbDWOS;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkMatchMaker;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
    }
}
