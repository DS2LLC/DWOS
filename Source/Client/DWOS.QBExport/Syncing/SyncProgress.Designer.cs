namespace DWOS.QBExport.Syncing
{
    partial class SyncProgress
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
            DisposeCodeBehind(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Messages received during sync.", Infragistics.Win.ToolTipImage.Default, "Messages", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Errors encountered during sync.", Infragistics.Win.ToolTipImage.Default, "Errors", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Warnings encountered during the sync.", Infragistics.Win.ToolTipImage.Default, "Warnings", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Track the sync progress", Infragistics.Win.ToolTipImage.Default, "Sync Progress", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Begin Sync", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab6 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.txtMessages = new System.Windows.Forms.TextBox();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.txtErrors = new System.Windows.Forms.TextBox();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.txtWarnings = new System.Windows.Forms.TextBox();
            this.pgbSyncProgress = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.btnSync = new Infragistics.Win.Misc.UltraButton();
            this.ultraTabControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.ultraTabPageControl3.SuspendLayout();
            this.ultraTabPageControl1.SuspendLayout();
            this.ultraTabPageControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).BeginInit();
            this.ultraTabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Controls.Add(this.txtMessages);
            this.ultraTabPageControl3.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(569, 186);
            // 
            // txtMessages
            // 
            this.txtMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessages.Location = new System.Drawing.Point(2, 3);
            this.txtMessages.Multiline = true;
            this.txtMessages.Name = "txtMessages";
            this.txtMessages.ReadOnly = true;
            this.txtMessages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessages.Size = new System.Drawing.Size(563, 180);
            this.txtMessages.TabIndex = 0;
            ultraToolTipInfo5.ToolTipText = "Messages received during sync.";
            ultraToolTipInfo5.ToolTipTitle = "Messages";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtMessages, ultraToolTipInfo5);
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.txtErrors);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(569, 186);
            // 
            // txtErrors
            // 
            this.txtErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtErrors.Location = new System.Drawing.Point(2, 3);
            this.txtErrors.Multiline = true;
            this.txtErrors.Name = "txtErrors";
            this.txtErrors.ReadOnly = true;
            this.txtErrors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtErrors.Size = new System.Drawing.Size(563, 180);
            this.txtErrors.TabIndex = 0;
            ultraToolTipInfo1.ToolTipText = "Errors encountered during sync.";
            ultraToolTipInfo1.ToolTipTitle = "Errors";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtErrors, ultraToolTipInfo1);
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.txtWarnings);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(569, 186);
            // 
            // txtWarnings
            // 
            this.txtWarnings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWarnings.Location = new System.Drawing.Point(2, 3);
            this.txtWarnings.Multiline = true;
            this.txtWarnings.Name = "txtWarnings";
            this.txtWarnings.ReadOnly = true;
            this.txtWarnings.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWarnings.Size = new System.Drawing.Size(563, 180);
            this.txtWarnings.TabIndex = 0;
            ultraToolTipInfo2.ToolTipText = "Warnings encountered during the sync.";
            ultraToolTipInfo2.ToolTipTitle = "Warnings";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtWarnings, ultraToolTipInfo2);
            // 
            // pgbSyncProgress
            // 
            this.pgbSyncProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgbSyncProgress.Location = new System.Drawing.Point(106, 234);
            this.pgbSyncProgress.Name = "pgbSyncProgress";
            this.pgbSyncProgress.Size = new System.Drawing.Size(478, 23);
            this.pgbSyncProgress.TabIndex = 1;
            this.pgbSyncProgress.Text = "[Formatted]";
            ultraToolTipInfo4.ToolTipText = "Track the sync progress";
            ultraToolTipInfo4.ToolTipTitle = "Sync Progress";
            this.ultraToolTipManager1.SetUltraToolTip(this.pgbSyncProgress, ultraToolTipInfo4);
            // 
            // btnSync
            // 
            this.btnSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSync.Location = new System.Drawing.Point(12, 234);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(87, 23);
            this.btnSync.TabIndex = 0;
            this.btnSync.Text = "Sync";
            ultraToolTipInfo3.ToolTipTextFormatted = "Begin syncing data.<br/>QuickBooks must be open during the sync. You may wan to r" +
    "un QuickBooks as Administrator.";
            ultraToolTipInfo3.ToolTipTitle = "Begin Sync";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnSync, ultraToolTipInfo3);
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // ultraTabControl1
            // 
            this.ultraTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraTabControl1.Controls.Add(this.ultraTabSharedControlsPage1);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl1);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl2);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl3);
            this.ultraTabControl1.Location = new System.Drawing.Point(12, 3);
            this.ultraTabControl1.Name = "ultraTabControl1";
            this.ultraTabControl1.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.ultraTabControl1.Size = new System.Drawing.Size(573, 212);
            this.ultraTabControl1.TabIndex = 2;
            ultraTab6.Key = "message";
            ultraTab6.TabPage = this.ultraTabPageControl3;
            ultraTab6.Text = "Messages";
            ultraTab4.Key = "error";
            ultraTab4.TabPage = this.ultraTabPageControl1;
            ultraTab4.Text = "Errors";
            ultraTab5.Key = "warning";
            ultraTab5.TabPage = this.ultraTabPageControl2;
            ultraTab5.Text = "Warnings";
            this.ultraTabControl1.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab6,
            ultraTab4,
            ultraTab5});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(569, 186);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 218);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(330, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "NOTE: QuickBooks must be open before beginning sync.";
            // 
            // SyncProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ultraTabControl1);
            this.Controls.Add(this.btnSync);
            this.Controls.Add(this.pgbSyncProgress);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SyncProgress";
            this.Size = new System.Drawing.Size(595, 265);
            this.ultraTabPageControl3.ResumeLayout(false);
            this.ultraTabPageControl3.PerformLayout();
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl1.PerformLayout();
            this.ultraTabPageControl2.ResumeLayout(false);
            this.ultraTabPageControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).EndInit();
            this.ultraTabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar pgbSyncProgress;
        private Infragistics.Win.Misc.UltraButton btnSync;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl ultraTabControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMessages;
        private System.Windows.Forms.TextBox txtErrors;
        private System.Windows.Forms.TextBox txtWarnings;
    }
}
