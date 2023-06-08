namespace DWOS.UI.Sales.Order
{
    partial class BatchCocInformation
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.txtBatchCoc = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboCreatedBy = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtBatch = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.dteDateCertified = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.cocElementHost = new System.Windows.Forms.Integration.ElementHost();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBatchCoc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCreatedBy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBatch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDateCertified)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.cocElementHost);
            this.grpData.Controls.Add(this.dteDateCertified);
            this.grpData.Controls.Add(this.txtBatch);
            this.grpData.Controls.Add(this.cboCreatedBy);
            this.grpData.Controls.Add(this.txtBatchCoc);
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.ultraLabel3);
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.ultraLabel1);
            appearance1.Image = global::DWOS.UI.Properties.Resources.Certificate_16;
            this.grpData.HeaderAppearance = appearance1;
            this.grpData.Size = new System.Drawing.Size(376, 353);
            this.grpData.Text = "Batch Certificate";
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            this.grpData.Controls.SetChildIndex(this.txtBatchCoc, 0);
            this.grpData.Controls.SetChildIndex(this.cboCreatedBy, 0);
            this.grpData.Controls.SetChildIndex(this.txtBatch, 0);
            this.grpData.Controls.SetChildIndex(this.dteDateCertified, 0);
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.cocElementHost, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(247, -517);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 33);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(88, 15);
            this.ultraLabel1.TabIndex = 1;
            this.ultraLabel1.Text = "Batch COC ID:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(6, 61);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(59, 15);
            this.ultraLabel2.TabIndex = 2;
            this.ultraLabel2.Text = "Batch ID:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(6, 89);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(72, 15);
            this.ultraLabel3.TabIndex = 3;
            this.ultraLabel3.Text = "Created By:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(6, 117);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(88, 15);
            this.ultraLabel4.TabIndex = 4;
            this.ultraLabel4.Text = "Certified Date:";
            // 
            // txtBatchCoc
            // 
            this.txtBatchCoc.Location = new System.Drawing.Point(100, 29);
            this.txtBatchCoc.Name = "txtBatchCoc";
            this.txtBatchCoc.ReadOnly = true;
            this.txtBatchCoc.Size = new System.Drawing.Size(144, 22);
            this.txtBatchCoc.TabIndex = 6;
            // 
            // cboCreatedBy
            // 
            this.cboCreatedBy.Location = new System.Drawing.Point(100, 85);
            this.cboCreatedBy.Name = "cboCreatedBy";
            this.cboCreatedBy.ReadOnly = true;
            this.cboCreatedBy.Size = new System.Drawing.Size(144, 22);
            this.cboCreatedBy.TabIndex = 7;
            // 
            // txtBatch
            // 
            this.txtBatch.Location = new System.Drawing.Point(100, 57);
            this.txtBatch.Name = "txtBatch";
            this.txtBatch.ReadOnly = true;
            this.txtBatch.Size = new System.Drawing.Size(144, 22);
            this.txtBatch.TabIndex = 8;
            // 
            // dteDateCertified
            // 
            this.dteDateCertified.Location = new System.Drawing.Point(100, 113);
            this.dteDateCertified.Name = "dteDateCertified";
            this.dteDateCertified.ReadOnly = true;
            this.dteDateCertified.Size = new System.Drawing.Size(144, 22);
            this.dteDateCertified.TabIndex = 9;
            // 
            // cocElementHost
            // 
            this.cocElementHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cocElementHost.Location = new System.Drawing.Point(11, 141);
            this.cocElementHost.Name = "cocElementHost";
            this.cocElementHost.Size = new System.Drawing.Size(354, 206);
            this.cocElementHost.TabIndex = 10;
            this.cocElementHost.Child = null;
            // 
            // BatchCocInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "BatchCocInformation";
            this.Size = new System.Drawing.Size(382, 359);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBatchCoc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCreatedBy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBatch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDateCertified)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBatchCoc;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteDateCertified;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBatch;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCreatedBy;
        private System.Windows.Forms.Integration.ElementHost cocElementHost;
    }
}
