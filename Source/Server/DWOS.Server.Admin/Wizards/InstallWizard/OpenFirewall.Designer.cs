namespace DWOS.Server.Admin.Wizards.InstallWizard
{
    partial class OpenFirewall
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
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Enabled");
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn2 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Port");
            this.txtStatus = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.btnStart = new Infragistics.Win.Misc.UltraButton();
            this.lvwFirewallRules = new Infragistics.Win.UltraWinListView.UltraListView();
            ((System.ComponentModel.ISupportInitialize)(this.lvwFirewallRules)).BeginInit();
            this.SuspendLayout();
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(165, 12);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(94, 15);
            this.txtStatus.TabIndex = 33;
            this.txtStatus.Text = "------------";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(11, 12);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(148, 15);
            this.ultraLabel5.TabIndex = 32;
            this.ultraLabel5.Text = "Windows Firewall Status:";
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.AutoSize = true;
            this.btnStart.ImageSize = new System.Drawing.Size(24, 24);
            this.btnStart.Location = new System.Drawing.Point(380, 143);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(95, 25);
            this.btnStart.TabIndex = 34;
            this.btnStart.Text = "Add Exception";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lvwFirewallRules
            // 
            this.lvwFirewallRules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwFirewallRules.ItemSettings.DefaultImage = global::DWOS.Server.Admin.Properties.Resources.network_firewall;
            this.lvwFirewallRules.Location = new System.Drawing.Point(29, 33);
            this.lvwFirewallRules.MainColumn.Key = "Rule";
            this.lvwFirewallRules.MainColumn.Text = "Rule";
            this.lvwFirewallRules.MainColumn.VisiblePositionInDetailsView = 0;
            this.lvwFirewallRules.Name = "lvwFirewallRules";
            this.lvwFirewallRules.Size = new System.Drawing.Size(446, 104);
            ultraListViewSubItemColumn1.Key = "Enabled";
            ultraListViewSubItemColumn1.Text = "Enabled";
            ultraListViewSubItemColumn1.VisiblePositionInDetailsView = 2;
            ultraListViewSubItemColumn2.Key = "Port";
            ultraListViewSubItemColumn2.Text = "Port";
            ultraListViewSubItemColumn2.VisiblePositionInDetailsView = 1;
            this.lvwFirewallRules.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1,
            ultraListViewSubItemColumn2});
            this.lvwFirewallRules.TabIndex = 38;
            this.lvwFirewallRules.Text = "Firewall Rules";
            this.lvwFirewallRules.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvwFirewallRules.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns;
            this.lvwFirewallRules.ViewSettingsDetails.FullRowSelect = true;
            // 
            // OpenFirewall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvwFirewallRules);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.ultraLabel5);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "OpenFirewall";
            this.Size = new System.Drawing.Size(505, 198);
            ((System.ComponentModel.ISupportInitialize)(this.lvwFirewallRules)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel txtStatus;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.Misc.UltraButton btnStart;
        private Infragistics.Win.UltraWinListView.UltraListView lvwFirewallRules;
    }
}
