namespace DWOS.UI.Sales.Customer
{
    partial class RelatedContactInformation
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
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click this button to go to the original contact.", Infragistics.Win.ToolTipImage.Default, "Go to Original Contact", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click this button to go to the original customer.", Infragistics.Win.ToolTipImage.Default, "Go to Original Customer", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtContactName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtCustomerName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnGoToOriginal = new Infragistics.Win.Misc.UltraButton();
            this.btnGoToCustomer = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContactName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerName)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.btnGoToCustomer);
            this.grpData.Controls.Add(this.btnGoToOriginal);
            this.grpData.Controls.Add(this.txtCustomerName);
            this.grpData.Controls.Add(this.txtContactName);
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.ultraLabel1);
            appearance3.Image = global::DWOS.UI.Properties.Resources.Contact_16;
            this.grpData.HeaderAppearance = appearance3;
            this.grpData.Size = new System.Drawing.Size(446, 110);
            this.grpData.Text = "Contact for Related Customer";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.grpData.Controls.SetChildIndex(this.txtContactName, 0);
            this.grpData.Controls.SetChildIndex(this.txtCustomerName, 0);
            this.grpData.Controls.SetChildIndex(this.btnGoToOriginal, 0);
            this.grpData.Controls.SetChildIndex(this.btnGoToCustomer, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(422, -2020);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(11, 37);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel1.TabIndex = 1;
            this.ultraLabel1.Text = "Name:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(11, 70);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(64, 15);
            this.ultraLabel2.TabIndex = 2;
            this.ultraLabel2.Text = "Customer:";
            // 
            // txtContactName
            // 
            this.txtContactName.AutoSize = false;
            this.txtContactName.Location = new System.Drawing.Point(81, 31);
            this.txtContactName.Name = "txtContactName";
            this.txtContactName.ReadOnly = true;
            this.txtContactName.Size = new System.Drawing.Size(185, 26);
            this.txtContactName.TabIndex = 3;
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.AutoSize = false;
            this.txtCustomerName.Location = new System.Drawing.Point(81, 64);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(185, 25);
            this.txtCustomerName.TabIndex = 5;
            // 
            // btnGoToOriginal
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.Run;
            this.btnGoToOriginal.Appearance = appearance2;
            this.btnGoToOriginal.AutoSize = true;
            this.btnGoToOriginal.Location = new System.Drawing.Point(272, 31);
            this.btnGoToOriginal.Name = "btnGoToOriginal";
            this.btnGoToOriginal.Size = new System.Drawing.Size(26, 26);
            this.btnGoToOriginal.TabIndex = 4;
            ultraToolTipInfo2.ToolTipText = "Click this button to go to the original contact.";
            ultraToolTipInfo2.ToolTipTitle = "Go to Original Contact";
            this.tipManager.SetUltraToolTip(this.btnGoToOriginal, ultraToolTipInfo2);
            this.btnGoToOriginal.Click += new System.EventHandler(this.btnGoToOriginal_Click);
            // 
            // btnGoToCustomer
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Run;
            this.btnGoToCustomer.Appearance = appearance1;
            this.btnGoToCustomer.AutoSize = true;
            this.btnGoToCustomer.Location = new System.Drawing.Point(272, 63);
            this.btnGoToCustomer.Name = "btnGoToCustomer";
            this.btnGoToCustomer.Size = new System.Drawing.Size(26, 26);
            this.btnGoToCustomer.TabIndex = 6;
            ultraToolTipInfo1.ToolTipText = "Click this button to go to the original customer.";
            ultraToolTipInfo1.ToolTipTitle = "Go to Original Customer";
            this.tipManager.SetUltraToolTip(this.btnGoToCustomer, ultraToolTipInfo1);
            this.btnGoToCustomer.Click += new System.EventHandler(this.btnGoToCustomer_Click);
            // 
            // RelatedContactInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "RelatedContactInformation";
            this.Size = new System.Drawing.Size(452, 116);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContactName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerName)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtContactName;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomerName;
        private Infragistics.Win.Misc.UltraButton btnGoToCustomer;
        private Infragistics.Win.Misc.UltraButton btnGoToOriginal;
    }
}
