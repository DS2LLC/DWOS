namespace DWOS.UI.Admin.FieldMigration
{
    partial class SummaryPanel
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
            this.tvwSummary = new Infragistics.Win.UltraWinTree.UltraTree();
            this.ultraTreePrintDocument1 = new Infragistics.Win.UltraWinTree.UltraTreePrintDocument(this.components);
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
            this.tvwSummary.Size = new System.Drawing.Size(137, 144);
            this.tvwSummary.TabIndex = 0;
            // 
            // SummaryPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tvwSummary);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SummaryPanel";
            this.Size = new System.Drawing.Size(143, 150);
            ((System.ComponentModel.ISupportInitialize)(this.tvwSummary)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTree.UltraTree tvwSummary;
        private Infragistics.Win.UltraWinTree.UltraTreePrintDocument ultraTreePrintDocument1;
    }
}
