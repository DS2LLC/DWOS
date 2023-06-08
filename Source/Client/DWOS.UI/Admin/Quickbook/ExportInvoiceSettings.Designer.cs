namespace DWOS.UI.Admin.Quickbook
{
    partial class ExportInvoiceSettings
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
            this.txtTransactionClass = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.numMaxExport = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.txtTrackingNumber = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCustomerWO = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtWOPrefix = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtConnectionName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.FormLabel = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransactionClass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxExport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTrackingNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerWO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWOPrefix)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConnectionName)).BeginInit();
            this.SuspendLayout();
            // 
            // txtTransactionClass
            // 
            this.txtTransactionClass.Location = new System.Drawing.Point(133, 148);
            this.txtTransactionClass.Name = "txtTransactionClass";
            this.txtTransactionClass.Size = new System.Drawing.Size(194, 22);
            this.txtTransactionClass.TabIndex = 55;
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(8, 151);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(109, 15);
            this.ultraLabel5.TabIndex = 54;
            this.ultraLabel5.Text = "Transaction Class:";
            // 
            // numMaxExport
            // 
            this.numMaxExport.Location = new System.Drawing.Point(133, 119);
            this.numMaxExport.MaxValue = 500;
            this.numMaxExport.MinValue = 1;
            this.numMaxExport.Name = "numMaxExport";
            this.numMaxExport.Size = new System.Drawing.Size(194, 22);
            this.numMaxExport.TabIndex = 48;
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(8, 123);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(73, 15);
            this.ultraLabel4.TabIndex = 53;
            this.ultraLabel4.Text = "Max Export:";
            // 
            // txtTrackingNumber
            // 
            this.txtTrackingNumber.Location = new System.Drawing.Point(133, 91);
            this.txtTrackingNumber.Name = "txtTrackingNumber";
            this.txtTrackingNumber.Size = new System.Drawing.Size(194, 22);
            this.txtTrackingNumber.TabIndex = 47;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(8, 95);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(102, 15);
            this.ultraLabel3.TabIndex = 52;
            this.ultraLabel3.Text = "Tracking # Field:";
            // 
            // txtCustomerWO
            // 
            this.txtCustomerWO.Location = new System.Drawing.Point(133, 63);
            this.txtCustomerWO.Name = "txtCustomerWO";
            this.txtCustomerWO.Size = new System.Drawing.Size(194, 22);
            this.txtCustomerWO.TabIndex = 46;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(8, 67);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(119, 15);
            this.ultraLabel2.TabIndex = 51;
            this.ultraLabel2.Text = "Customer WO Field:";
            // 
            // txtWOPrefix
            // 
            this.txtWOPrefix.Location = new System.Drawing.Point(133, 35);
            this.txtWOPrefix.Name = "txtWOPrefix";
            this.txtWOPrefix.Size = new System.Drawing.Size(194, 22);
            this.txtWOPrefix.TabIndex = 45;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(8, 39);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(87, 15);
            this.ultraLabel1.TabIndex = 50;
            this.ultraLabel1.Text = "Invoice Prefix:";
            // 
            // txtConnectionName
            // 
            this.txtConnectionName.Location = new System.Drawing.Point(133, 7);
            this.txtConnectionName.Name = "txtConnectionName";
            this.txtConnectionName.Size = new System.Drawing.Size(194, 22);
            this.txtConnectionName.TabIndex = 44;
            // 
            // FormLabel
            // 
            this.FormLabel.AutoSize = true;
            this.FormLabel.Location = new System.Drawing.Point(8, 11);
            this.FormLabel.Name = "FormLabel";
            this.FormLabel.Size = new System.Drawing.Size(73, 15);
            this.FormLabel.TabIndex = 49;
            this.FormLabel.Text = "Connection:";
            // 
            // ExportInvoiceSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtTransactionClass);
            this.Controls.Add(this.ultraLabel5);
            this.Controls.Add(this.numMaxExport);
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.txtTrackingNumber);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.txtCustomerWO);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.txtWOPrefix);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.txtConnectionName);
            this.Controls.Add(this.FormLabel);
            this.Name = "ExportInvoiceSettings";
            ((System.ComponentModel.ISupportInitialize)(this.txtTransactionClass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxExport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTrackingNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerWO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWOPrefix)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConnectionName)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTransactionClass;
        public Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numMaxExport;
        public Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTrackingNumber;
        public Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomerWO;
        public Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtWOPrefix;
        public Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtConnectionName;
        public Infragistics.Win.Misc.UltraLabel FormLabel;

    }
}
