namespace DWOS.UI.Admin.Quickbook
{
    partial class InvoiceCompareOptions
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dteFromDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.dteToDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate)).BeginInit();
            this.SuspendLayout();
            // 
            // dteFromDate
            // 
            this.dteFromDate.Location = new System.Drawing.Point(100, 12);
            this.dteFromDate.Name = "dteFromDate";
            this.dteFromDate.Size = new System.Drawing.Size(168, 22);
            this.dteFromDate.TabIndex = 0;
            // 
            // dteToDate
            // 
            this.dteToDate.Location = new System.Drawing.Point(100, 39);
            this.dteToDate.Name = "dteToDate";
            this.dteToDate.Size = new System.Drawing.Size(168, 22);
            this.dteToDate.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(173, 83);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(87, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(12, 16);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(69, 15);
            this.ultraLabel1.TabIndex = 3;
            this.ultraLabel1.Text = "From Date:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(12, 43);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(54, 15);
            this.ultraLabel2.TabIndex = 4;
            this.ultraLabel2.Text = "To Date:";
            // 
            // InvoiceCompareOptions
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 118);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dteToDate);
            this.Controls.Add(this.dteFromDate);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "InvoiceCompareOptions";
            this.ShowInTaskbar = false;
            this.Text = "Invoice Compare Options";
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        internal Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteFromDate;
        internal Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteToDate;
    }
}