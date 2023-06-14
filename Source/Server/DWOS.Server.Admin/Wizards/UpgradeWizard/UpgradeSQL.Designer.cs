namespace DWOS.Server.Admin.Wizards.UpgradeWizard
{
    partial class UpgradeSQL
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
            this.txtStatus = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.btnUpgrade = new Infragistics.Win.Misc.UltraButton();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.lblToVersion = new Infragistics.Win.Misc.UltraLabel();
            this.lblFromVersion = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.lblDBName = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(3, 66);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(467, 92);
            this.txtStatus.TabIndex = 0;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(3, 45);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(46, 15);
            this.ultraLabel3.TabIndex = 28;
            this.ultraLabel3.Text = "Status:";
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpgrade.Location = new System.Drawing.Point(383, 164);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(87, 23);
            this.btnUpgrade.TabIndex = 29;
            this.btnUpgrade.Text = "Upgrade";
            this.btnUpgrade.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(3, 3);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(85, 15);
            this.ultraLabel1.TabIndex = 30;
            this.ultraLabel1.Text = "From Version:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(192, 3);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(70, 15);
            this.ultraLabel2.TabIndex = 31;
            this.ultraLabel2.Text = "To Version:";
            // 
            // lblToVersion
            // 
            this.lblToVersion.AutoSize = true;
            this.lblToVersion.Location = new System.Drawing.Point(268, 3);
            this.lblToVersion.Name = "lblToVersion";
            this.lblToVersion.Size = new System.Drawing.Size(36, 15);
            this.lblToVersion.TabIndex = 32;
            this.lblToVersion.Text = "X.X.X";
            // 
            // lblFromVersion
            // 
            this.lblFromVersion.AutoSize = true;
            this.lblFromVersion.Location = new System.Drawing.Point(94, 3);
            this.lblFromVersion.Name = "lblFromVersion";
            this.lblFromVersion.Size = new System.Drawing.Size(36, 15);
            this.lblFromVersion.TabIndex = 33;
            this.lblFromVersion.Text = "X.X.X";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(3, 24);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(100, 15);
            this.ultraLabel4.TabIndex = 34;
            this.ultraLabel4.Text = "Database Name:";
            // 
            // lblDBName
            // 
            this.lblDBName.AutoSize = true;
            this.lblDBName.Location = new System.Drawing.Point(109, 24);
            this.lblDBName.Name = "lblDBName";
            this.lblDBName.Size = new System.Drawing.Size(58, 15);
            this.lblDBName.TabIndex = 35;
            this.lblDBName.Text = "DB Name";
            // 
            // UpgradeSQL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblDBName);
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.lblFromVersion);
            this.Controls.Add(this.lblToVersion);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.btnUpgrade);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.txtStatus);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "UpgradeSQL";
            this.Size = new System.Drawing.Size(474, 199);
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtStatus;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraButton btnUpgrade;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        private Infragistics.Win.Misc.UltraLabel lblToVersion;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel lblFromVersion;
        private Infragistics.Win.Misc.UltraLabel lblDBName;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
    }
}
