namespace DWOS.Server.Admin.Wizards.UpgradeWizard
{
    partial class UpdatePortalConfiguration
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
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.btnRefreshSettings = new Infragistics.Win.Misc.UltraButton();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(3, 3);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(442, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "The customer portal is installed but may need to have its settings refreshed.";
            // 
            // btnRefreshSettings
            // 
            this.btnRefreshSettings.Location = new System.Drawing.Point(3, 24);
            this.btnRefreshSettings.Name = "btnRefreshSettings";
            this.btnRefreshSettings.Size = new System.Drawing.Size(105, 23);
            this.btnRefreshSettings.TabIndex = 1;
            this.btnRefreshSettings.Text = "Refresh Config";
            this.btnRefreshSettings.Click += new System.EventHandler(this.btnRefreshSettings_Click);
            // 
            // UpdatePortalConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnRefreshSettings);
            this.Controls.Add(this.ultraLabel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "UpdatePortalConfiguration";
            this.Size = new System.Drawing.Size(453, 58);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraButton btnRefreshSettings;
    }
}
