namespace DWOS.QBExport.Syncing
{
    partial class SyncSummary
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
            this.tvwSummary = new Infragistics.Win.UltraWinTree.UltraTree();
            ((System.ComponentModel.ISupportInitialize)(this.tvwSummary)).BeginInit();
            this.SuspendLayout();
            // 
            // tvwSummary
            // 
            this.tvwSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwSummary.Location = new System.Drawing.Point(3, 3);
            this.tvwSummary.Name = "tvwSummary";
            this.tvwSummary.Size = new System.Drawing.Size(491, 291);
            this.tvwSummary.TabIndex = 0;
            // 
            // SyncSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tvwSummary);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SyncSummary";
            this.Size = new System.Drawing.Size(498, 297);
            ((System.ComponentModel.ISupportInitialize)(this.tvwSummary)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTree.UltraTree tvwSummary;
    }
}
