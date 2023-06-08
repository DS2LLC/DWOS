namespace DWOS.UI.Admin.OrderReset
{
	partial class ResetOrderSteps
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
			this.txtSelectedOrders = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
			this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
			((System.ComponentModel.ISupportInitialize)(this.txtSelectedOrders)).BeginInit();
			this.SuspendLayout();
			// 
			// txtSelectedOrders
			// 
			this.txtSelectedOrders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSelectedOrders.Location = new System.Drawing.Point(22, 24);
			this.txtSelectedOrders.Multiline = true;
			this.txtSelectedOrders.Name = "txtSelectedOrders";
			this.txtSelectedOrders.ReadOnly = true;
			this.txtSelectedOrders.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtSelectedOrders.Size = new System.Drawing.Size(595, 350);
			this.txtSelectedOrders.TabIndex = 68;
			// 
			// ultraLabel2
			// 
			this.ultraLabel2.Location = new System.Drawing.Point(3, 3);
			this.ultraLabel2.Name = "ultraLabel2";
			this.ultraLabel2.Size = new System.Drawing.Size(46, 15);
			this.ultraLabel2.TabIndex = 69;
			this.ultraLabel2.Text = "Status:";
			// 
			// ResetOrderSteps
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ultraLabel2);
			this.Controls.Add(this.txtSelectedOrders);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ResetOrderSteps";
			this.Size = new System.Drawing.Size(633, 377);
			((System.ComponentModel.ISupportInitialize)(this.txtSelectedOrders)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSelectedOrders;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;

	}
}
