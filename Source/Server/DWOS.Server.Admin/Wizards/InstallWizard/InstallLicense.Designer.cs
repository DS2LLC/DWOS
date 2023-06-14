namespace DWOS.Server.Admin.Wizards.InstallWizard
{
    partial class InstallLicense
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "License Key", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Customer", Infragistics.Win.DefaultableBoolean.Default);
            this.btnActivate = new Infragistics.Win.Misc.UltraButton();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.txtLicenseKey = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCustomerKey = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.txtLicenseKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerKey)).BeginInit();
            this.SuspendLayout();
            // 
            // btnActivate
            // 
            this.btnActivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActivate.Location = new System.Drawing.Point(329, 68);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(103, 23);
            this.btnActivate.TabIndex = 8;
            this.btnActivate.Text = "Activate";
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // txtLicenseKey
            // 
            this.txtLicenseKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLicenseKey.Location = new System.Drawing.Point(110, 40);
            this.txtLicenseKey.Name = "txtLicenseKey";
            this.txtLicenseKey.Size = new System.Drawing.Size(322, 22);
            this.txtLicenseKey.TabIndex = 13;
            ultraToolTipInfo1.ToolTipTextFormatted = "Enter the license key provided when you purchased DWOS.";
            ultraToolTipInfo1.ToolTipTitle = "License Key";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtLicenseKey, ultraToolTipInfo1);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(14, 44);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(76, 15);
            this.ultraLabel3.TabIndex = 12;
            this.ultraLabel3.Text = "License Key:";
            // 
            // txtCustomerKey
            // 
            this.txtCustomerKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomerKey.Location = new System.Drawing.Point(110, 12);
            this.txtCustomerKey.Name = "txtCustomerKey";
            this.txtCustomerKey.Size = new System.Drawing.Size(322, 22);
            this.txtCustomerKey.TabIndex = 11;
            ultraToolTipInfo2.ToolTipTextFormatted = "Enter your customer id.";
            ultraToolTipInfo2.ToolTipTitle = "Customer";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtCustomerKey, ultraToolTipInfo2);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(14, 16);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(64, 15);
            this.ultraLabel1.TabIndex = 10;
            this.ultraLabel1.Text = "Customer:";
            // 
            // InstallLicense
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtLicenseKey);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.txtCustomerKey);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.btnActivate);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "InstallLicense";
            this.Size = new System.Drawing.Size(435, 192);
            ((System.ComponentModel.ISupportInitialize)(this.txtLicenseKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerKey)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnActivate;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtLicenseKey;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomerKey;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
    }
}
