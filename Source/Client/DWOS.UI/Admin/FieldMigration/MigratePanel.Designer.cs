namespace DWOS.UI.Admin.FieldMigration
{
    partial class MigratePanel
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
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.lblCustomField = new Infragistics.Win.Misc.UltraLabel();
            this.lblField = new Infragistics.Win.Misc.UltraLabel();
            this.lblSummary = new Infragistics.Win.Misc.UltraLabel();
            this.btnMigrate = new Infragistics.Win.Misc.UltraButton();
            this.pgbProgress = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(3, 3);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(32, 14);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Field:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(3, 35);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(74, 14);
            this.ultraLabel2.TabIndex = 1;
            this.ultraLabel2.Text = "Custom Field:";
            // 
            // lblCustomField
            // 
            this.lblCustomField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCustomField.Location = new System.Drawing.Point(130, 35);
            this.lblCustomField.Name = "lblCustomField";
            this.lblCustomField.Size = new System.Drawing.Size(157, 36);
            this.lblCustomField.TabIndex = 2;
            this.lblCustomField.Text = "ultraLabel3";
            // 
            // lblField
            // 
            this.lblField.AutoSize = true;
            this.lblField.Location = new System.Drawing.Point(130, 3);
            this.lblField.Name = "lblField";
            this.lblField.Size = new System.Drawing.Size(75, 14);
            this.lblField.TabIndex = 3;
            this.lblField.Text = "Product Class";
            // 
            // lblSummary
            // 
            this.lblSummary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSummary.Location = new System.Drawing.Point(3, 77);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(284, 41);
            this.lblSummary.TabIndex = 4;
            this.lblSummary.Text = "<Summary Contents>";
            // 
            // btnMigrate
            // 
            this.btnMigrate.Location = new System.Drawing.Point(3, 124);
            this.btnMigrate.Name = "btnMigrate";
            this.btnMigrate.Size = new System.Drawing.Size(75, 23);
            this.btnMigrate.TabIndex = 6;
            this.btnMigrate.Text = "Migrate";
            this.btnMigrate.Click += new System.EventHandler(this.btnMigrate_Click);
            // 
            // pgbProgress
            // 
            this.pgbProgress.Location = new System.Drawing.Point(84, 124);
            this.pgbProgress.Name = "pgbProgress";
            this.pgbProgress.Size = new System.Drawing.Size(203, 23);
            this.pgbProgress.TabIndex = 7;
            this.pgbProgress.Text = "[Formatted]";
            // 
            // MigratePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pgbProgress);
            this.Controls.Add(this.btnMigrate);
            this.Controls.Add(this.lblSummary);
            this.Controls.Add(this.lblField);
            this.Controls.Add(this.lblCustomField);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraLabel1);
            this.Name = "MigratePanel";
            this.Size = new System.Drawing.Size(290, 157);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel lblCustomField;
        private Infragistics.Win.Misc.UltraLabel lblField;
        private Infragistics.Win.Misc.UltraLabel lblSummary;
        private Infragistics.Win.Misc.UltraButton btnMigrate;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar pgbProgress;
    }
}
