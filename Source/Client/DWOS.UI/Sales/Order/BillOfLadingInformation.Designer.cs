namespace DWOS.UI.Sales.Order
{
    partial class BillOfLadingInformation
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The unique ID of the shipping package.", Infragistics.Win.ToolTipImage.Default, "Shipping Package", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the bill of lading was created.", Infragistics.Win.ToolTipImage.Default, "Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The unique ID of the bill of lading.", Infragistics.Win.ToolTipImage.Default, "Bill of Lading ID", Infragistics.Win.DefaultableBoolean.Default);
            this.txtPackage = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.dteDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.txtBillOfLadingId = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.mediaWidget = new DWOS.UI.Utilities.MediaWidget();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPackage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBillOfLadingId)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.mediaWidget);
            this.grpData.Controls.Add(this.txtPackage);
            this.grpData.Controls.Add(this.dteDate);
            this.grpData.Controls.Add(this.txtBillOfLadingId);
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.ultraLabel3);
            this.grpData.Controls.Add(this.ultraLabel1);
            appearance1.Image = global::DWOS.UI.Properties.Resources.Paper_16;
            this.grpData.HeaderAppearance = appearance1;
            this.grpData.Size = new System.Drawing.Size(465, 294);
            this.grpData.Text = "Bill of Lading";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            this.grpData.Controls.SetChildIndex(this.txtBillOfLadingId, 0);
            this.grpData.Controls.SetChildIndex(this.dteDate, 0);
            this.grpData.Controls.SetChildIndex(this.txtPackage, 0);
            this.grpData.Controls.SetChildIndex(this.mediaWidget, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(431, -134);
            // 
            // txtPackage
            // 
            this.txtPackage.Location = new System.Drawing.Point(127, 59);
            this.txtPackage.Name = "txtPackage";
            this.txtPackage.ReadOnly = true;
            this.txtPackage.Size = new System.Drawing.Size(328, 22);
            this.txtPackage.TabIndex = 2;
            ultraToolTipInfo1.ToolTipText = "The unique ID of the shipping package.";
            ultraToolTipInfo1.ToolTipTitle = "Shipping Package";
            this.tipManager.SetUltraToolTip(this.txtPackage, ultraToolTipInfo1);
            // 
            // dteDate
            // 
            this.dteDate.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.WindowsVista;
            this.dteDate.Location = new System.Drawing.Point(127, 87);
            this.dteDate.Name = "dteDate";
            this.dteDate.ReadOnly = true;
            this.dteDate.Size = new System.Drawing.Size(328, 22);
            this.dteDate.TabIndex = 3;
            this.dteDate.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextSection;
            ultraToolTipInfo2.ToolTipText = "The date the bill of lading was created.";
            ultraToolTipInfo2.ToolTipTitle = "Date";
            this.tipManager.SetUltraToolTip(this.dteDate, ultraToolTipInfo2);
            // 
            // txtBillOfLadingId
            // 
            this.txtBillOfLadingId.Location = new System.Drawing.Point(127, 31);
            this.txtBillOfLadingId.Name = "txtBillOfLadingId";
            this.txtBillOfLadingId.ReadOnly = true;
            this.txtBillOfLadingId.Size = new System.Drawing.Size(328, 22);
            this.txtBillOfLadingId.TabIndex = 1;
            ultraToolTipInfo3.ToolTipText = "The unique ID of the bill of lading.";
            ultraToolTipInfo3.ToolTipTitle = "Bill of Lading ID";
            this.tipManager.SetUltraToolTip(this.txtBillOfLadingId, ultraToolTipInfo3);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(11, 63);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(110, 15);
            this.ultraLabel4.TabIndex = 12;
            this.ultraLabel4.Text = "Shipping Package:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(11, 91);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(36, 15);
            this.ultraLabel3.TabIndex = 11;
            this.ultraLabel3.Text = "Date:";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(11, 35);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(100, 15);
            this.ultraLabel1.TabIndex = 10;
            this.ultraLabel1.Text = "Bill of Lading ID:";
            // 
            // mediaWidget
            // 
            this.mediaWidget.AllowEditing = false;
            this.mediaWidget.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mediaWidget.Location = new System.Drawing.Point(127, 115);
            this.mediaWidget.Name = "mediaWidget";
            this.mediaWidget.Size = new System.Drawing.Size(328, 166);
            this.mediaWidget.TabIndex = 13;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(11, 115);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(73, 15);
            this.ultraLabel2.TabIndex = 14;
            this.ultraLabel2.Text = "Documents:";
            // 
            // BillOfLadingInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "BillOfLadingInformation";
            this.Size = new System.Drawing.Size(471, 300);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPackage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBillOfLadingId)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPackage;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteDate;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBillOfLadingId;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Utilities.MediaWidget mediaWidget;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
    }
}
