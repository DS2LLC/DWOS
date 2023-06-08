namespace DWOS.UI.Admin.SettingsPanels
{
    partial class SettingsContractReview
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
            this.grpMain = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtContactReview = new Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor();
            ((System.ComponentModel.ISupportInitialize)(this.grpMain)).BeginInit();
            this.grpMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMain
            // 
            this.grpMain.Controls.Add(this.txtContactReview);
            this.grpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpMain.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.grpMain.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpMain.Location = new System.Drawing.Point(0, 0);
            this.grpMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpMain.Name = "grpMain";
            this.grpMain.Size = new System.Drawing.Size(471, 400);
            this.grpMain.TabIndex = 0;
            this.grpMain.Text = "Contract Review";
            // 
            // txtContactReview
            // 
            this.txtContactReview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContactReview.Location = new System.Drawing.Point(6, 27);
            this.txtContactReview.Name = "txtContactReview";
            this.txtContactReview.Size = new System.Drawing.Size(459, 367);
            this.txtContactReview.TabIndex = 0;
            this.txtContactReview.Value = "";
            // 
            // SettingsContractReview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpMain);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(460, 400);
            this.Name = "SettingsContractReview";
            this.Size = new System.Drawing.Size(471, 400);
            ((System.ComponentModel.ISupportInitialize)(this.grpMain)).EndInit();
            this.grpMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox grpMain;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor txtContactReview;
    }
}
