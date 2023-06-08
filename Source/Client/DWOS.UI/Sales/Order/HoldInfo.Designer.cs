namespace DWOS.UI.Sales.Order
{
    partial class HoldInfo
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Start time of the hold.", Infragistics.Win.ToolTipImage.Default, "Time In", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Time Out", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Hold Notes", Infragistics.Win.ToolTipImage.Default, "Notes", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The reason for the hold.", Infragistics.Win.ToolTipImage.Default, "Reason", Infragistics.Win.DefaultableBoolean.Default);
            this.dteTimeIn = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.dteTimeOut = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.txtNotes = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.cboReason = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteTimeIn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteTimeOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReason)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.cboReason);
            this.grpData.Controls.Add(this.ultraLabel3);
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.ultraLabel1);
            this.grpData.Controls.Add(this.txtNotes);
            this.grpData.Controls.Add(this.dteTimeOut);
            this.grpData.Controls.Add(this.dteTimeIn);
            appearance1.Image = global::DWOS.UI.Properties.Resources.Hold_16;
            this.grpData.HeaderAppearance = appearance1;
            this.grpData.Size = new System.Drawing.Size(446, 341);
            this.grpData.Text = "Hold";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.dteTimeIn, 0);
            this.grpData.Controls.SetChildIndex(this.dteTimeOut, 0);
            this.grpData.Controls.SetChildIndex(this.txtNotes, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.grpData.Controls.SetChildIndex(this.cboReason, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(422, -438);
            // 
            // dteTimeIn
            // 
            this.dteTimeIn.Location = new System.Drawing.Point(79, 26);
            this.dteTimeIn.MaskInput = "{date} {time}";
            this.dteTimeIn.Name = "dteTimeIn";
            this.dteTimeIn.ReadOnly = true;
            this.dteTimeIn.Size = new System.Drawing.Size(156, 22);
            this.dteTimeIn.TabIndex = 1;
            ultraToolTipInfo4.ToolTipText = "Start time of the hold.";
            ultraToolTipInfo4.ToolTipTitle = "Time In";
            this.tipManager.SetUltraToolTip(this.dteTimeIn, ultraToolTipInfo4);
            // 
            // dteTimeOut
            // 
            this.dteTimeOut.Location = new System.Drawing.Point(79, 54);
            this.dteTimeOut.MaskInput = "{date} {time}";
            this.dteTimeOut.Name = "dteTimeOut";
            this.dteTimeOut.ReadOnly = true;
            this.dteTimeOut.Size = new System.Drawing.Size(156, 22);
            this.dteTimeOut.TabIndex = 2;
            ultraToolTipInfo3.ToolTipTextFormatted = "Stop time of the hold.<br/>If the hold is active, this field will be blank.";
            ultraToolTipInfo3.ToolTipTitle = "Time Out";
            this.tipManager.SetUltraToolTip(this.dteTimeOut, ultraToolTipInfo3);
            // 
            // txtNotes
            // 
            this.txtNotes.AcceptsReturn = true;
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotes.Location = new System.Drawing.Point(79, 110);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ReadOnly = true;
            this.txtNotes.Size = new System.Drawing.Size(356, 225);
            this.txtNotes.TabIndex = 4;
            ultraToolTipInfo2.ToolTipText = "Hold Notes";
            ultraToolTipInfo2.ToolTipTitle = "Notes";
            this.tipManager.SetUltraToolTip(this.txtNotes, ultraToolTipInfo2);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(11, 30);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(53, 15);
            this.ultraLabel1.TabIndex = 4;
            this.ultraLabel1.Text = "Time In:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(11, 58);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(62, 15);
            this.ultraLabel2.TabIndex = 5;
            this.ultraLabel2.Text = "Time Out:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(11, 110);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel3.TabIndex = 6;
            this.ultraLabel3.Text = "Notes:";
            // 
            // cboReason
            // 
            this.cboReason.Location = new System.Drawing.Point(79, 82);
            this.cboReason.Name = "cboReason";
            this.cboReason.ReadOnly = true;
            this.cboReason.Size = new System.Drawing.Size(156, 22);
            this.cboReason.TabIndex = 3;
            ultraToolTipInfo1.ToolTipText = "The reason for the hold.";
            ultraToolTipInfo1.ToolTipTitle = "Reason";
            this.tipManager.SetUltraToolTip(this.cboReason, ultraToolTipInfo1);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(11, 86);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(51, 15);
            this.ultraLabel4.TabIndex = 7;
            this.ultraLabel4.Text = "Reason:";
            // 
            // HoldInfo
            // 
            this.Name = "HoldInfo";
            this.Size = new System.Drawing.Size(452, 347);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteTimeIn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteTimeOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReason)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNotes;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteTimeOut;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteTimeIn;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboReason;
    }
}
