namespace DWOS.UI.Admin.OrderReset
{
    partial class CloseOrder
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
			this.dtCloseDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
			this.txtSelectedOrders = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
			this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
			((System.ComponentModel.ISupportInitialize)(this.dtCloseDate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtSelectedOrders)).BeginInit();
			this.SuspendLayout();
			// 
			// ultraLabel1
			// 
			this.ultraLabel1.Location = new System.Drawing.Point(7, 7);
			this.ultraLabel1.Name = "ultraLabel1";
			this.ultraLabel1.Size = new System.Drawing.Size(71, 15);
			this.ultraLabel1.TabIndex = 64;
			this.ultraLabel1.Text = "Close Date:";
			// 
			// dtCloseDate
			// 
			this.dtCloseDate.DateTime = new System.DateTime(2012, 4, 11, 0, 0, 0, 0);
			this.dtCloseDate.Location = new System.Drawing.Point(84, 3);
			this.dtCloseDate.Name = "dtCloseDate";
			this.dtCloseDate.Size = new System.Drawing.Size(144, 22);
			this.dtCloseDate.TabIndex = 65;
			this.dtCloseDate.Value = new System.DateTime(2012, 4, 11, 0, 0, 0, 0);
			// 
			// txtSelectedOrders
			// 
			this.txtSelectedOrders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSelectedOrders.Location = new System.Drawing.Point(22, 56);
			this.txtSelectedOrders.Multiline = true;
			this.txtSelectedOrders.Name = "txtSelectedOrders";
			this.txtSelectedOrders.ReadOnly = true;
			this.txtSelectedOrders.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtSelectedOrders.Size = new System.Drawing.Size(595, 318);
			this.txtSelectedOrders.TabIndex = 68;
			// 
			// ultraLabel2
			// 
			this.ultraLabel2.Location = new System.Drawing.Point(6, 35);
			this.ultraLabel2.Name = "ultraLabel2";
			this.ultraLabel2.Size = new System.Drawing.Size(46, 15);
			this.ultraLabel2.TabIndex = 69;
			this.ultraLabel2.Text = "Status:";
			// 
			// CloseOrder
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ultraLabel2);
			this.Controls.Add(this.txtSelectedOrders);
			this.Controls.Add(this.dtCloseDate);
			this.Controls.Add(this.ultraLabel1);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "CloseOrder";
			this.Size = new System.Drawing.Size(633, 377);
			((System.ComponentModel.ISupportInitialize)(this.dtCloseDate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtSelectedOrders)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtCloseDate;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSelectedOrders;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;

	}
}
