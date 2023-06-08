namespace DWOS.UI.Sales.Order
{
    partial class BulkCOCInformation
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The unique ID of the bulk certificate.", Infragistics.Win.ToolTipImage.Default, "Bulk Cert ID", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The user who created the bulk certificate.", Infragistics.Win.ToolTipImage.Default, "Created By", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the bulk certificate was created.", Infragistics.Win.ToolTipImage.Default, "Certified Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The unique ID of the shipping package.", Infragistics.Win.ToolTipImage.Default, "Shipping Package", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.txtBulkCOCID = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboCreatedBy = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.dteDateCertified = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.txtPackage = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBulkCOCID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCreatedBy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDateCertified)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPackage)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.txtPackage);
            this.grpData.Controls.Add(this.dteDateCertified);
            this.grpData.Controls.Add(this.cboCreatedBy);
            this.grpData.Controls.Add(this.txtBulkCOCID);
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.ultraLabel3);
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.ultraLabel1);
            appearance1.Image = global::DWOS.UI.Properties.Resources.Certificate_16;
            this.grpData.HeaderAppearance = appearance1;
            this.grpData.Text = "Bulk Certificate";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            this.grpData.Controls.SetChildIndex(this.txtBulkCOCID, 0);
            this.grpData.Controls.SetChildIndex(this.cboCreatedBy, 0);
            this.grpData.Controls.SetChildIndex(this.dteDateCertified, 0);
            this.grpData.Controls.SetChildIndex(this.txtPackage, 0);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(11, 29);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(79, 15);
            this.ultraLabel1.TabIndex = 1;
            this.ultraLabel1.Text = "Bulk Cert ID:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(11, 57);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(72, 15);
            this.ultraLabel2.TabIndex = 2;
            this.ultraLabel2.Text = "Created By:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(11, 85);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(88, 15);
            this.ultraLabel3.TabIndex = 3;
            this.ultraLabel3.Text = "Certified Date:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(11, 113);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(110, 15);
            this.ultraLabel4.TabIndex = 4;
            this.ultraLabel4.Text = "Shipping Package:";
            // 
            // txtBulkCOCID
            // 
            this.txtBulkCOCID.Location = new System.Drawing.Point(127, 25);
            this.txtBulkCOCID.Name = "txtBulkCOCID";
            this.txtBulkCOCID.ReadOnly = true;
            this.txtBulkCOCID.Size = new System.Drawing.Size(111, 22);
            this.txtBulkCOCID.TabIndex = 6;
            ultraToolTipInfo4.ToolTipText = "The unique ID of the bulk certificate.";
            ultraToolTipInfo4.ToolTipTitle = "Bulk Cert ID";
            this.tipManager.SetUltraToolTip(this.txtBulkCOCID, ultraToolTipInfo4);
            // 
            // cboCreatedBy
            // 
            this.cboCreatedBy.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            this.cboCreatedBy.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCreatedBy.Location = new System.Drawing.Point(127, 53);
            this.cboCreatedBy.Name = "cboCreatedBy";
            this.cboCreatedBy.ReadOnly = true;
            this.cboCreatedBy.Size = new System.Drawing.Size(210, 22);
            this.cboCreatedBy.TabIndex = 7;
            ultraToolTipInfo3.ToolTipText = "The user who created the bulk certificate.";
            ultraToolTipInfo3.ToolTipTitle = "Created By";
            this.tipManager.SetUltraToolTip(this.cboCreatedBy, ultraToolTipInfo3);
            // 
            // dteDateCertified
            // 
            this.dteDateCertified.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.WindowsVista;
            this.dteDateCertified.Location = new System.Drawing.Point(127, 81);
            this.dteDateCertified.Name = "dteDateCertified";
            this.dteDateCertified.ReadOnly = true;
            this.dteDateCertified.Size = new System.Drawing.Size(111, 22);
            this.dteDateCertified.TabIndex = 8;
            this.dteDateCertified.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextSection;
            ultraToolTipInfo2.ToolTipText = "The date the bulk certificate was created.";
            ultraToolTipInfo2.ToolTipTitle = "Certified Date";
            this.tipManager.SetUltraToolTip(this.dteDateCertified, ultraToolTipInfo2);
            // 
            // txtPackage
            // 
            this.txtPackage.Location = new System.Drawing.Point(127, 109);
            this.txtPackage.Name = "txtPackage";
            this.txtPackage.ReadOnly = true;
            this.txtPackage.Size = new System.Drawing.Size(111, 22);
            this.txtPackage.TabIndex = 9;
            ultraToolTipInfo1.ToolTipText = "The unique ID of the shipping package.";
            ultraToolTipInfo1.ToolTipTitle = "Shipping Package";
            this.tipManager.SetUltraToolTip(this.txtPackage, ultraToolTipInfo1);
            // 
            // BulkCOCInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "BulkCOCInformation";
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBulkCOCID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCreatedBy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDateCertified)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPackage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBulkCOCID;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCreatedBy;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteDateCertified;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPackage;
    }
}
