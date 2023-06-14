namespace DWOS.Server.Admin.Tasks
{
    partial class Registration
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Customer ID", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "License Key", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Registration));
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCustomerKey = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.txtFingerprint = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtStatus = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.prgStatus = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.txtLicenseKey = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFingerprint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLicenseKey)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(12, 16);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(64, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Customer:";
            // 
            // txtCustomerKey
            // 
            this.txtCustomerKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomerKey.Location = new System.Drawing.Point(108, 12);
            this.txtCustomerKey.Name = "txtCustomerKey";
            this.txtCustomerKey.Size = new System.Drawing.Size(239, 22);
            this.txtCustomerKey.TabIndex = 1;
            ultraToolTipInfo2.ToolTipTextFormatted = "Enter your customer identifier.";
            ultraToolTipInfo2.ToolTipTitle = "Customer ID";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtCustomerKey, ultraToolTipInfo2);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(275, 286);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Close";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(194, 286);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "Activate";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtFingerprint
            // 
            this.txtFingerprint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFingerprint.Location = new System.Drawing.Point(108, 68);
            this.txtFingerprint.Name = "txtFingerprint";
            this.txtFingerprint.ReadOnly = true;
            this.txtFingerprint.Size = new System.Drawing.Size(239, 22);
            this.txtFingerprint.TabIndex = 5;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(12, 72);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(71, 15);
            this.ultraLabel2.TabIndex = 4;
            this.ultraLabel2.Text = "Fingerprint:";
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(108, 96);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(239, 184);
            this.txtStatus.TabIndex = 6;
            // 
            // prgStatus
            // 
            this.prgStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgStatus.Location = new System.Drawing.Point(12, 286);
            this.prgStatus.Name = "prgStatus";
            this.prgStatus.Size = new System.Drawing.Size(161, 23);
            this.prgStatus.Style = Infragistics.Win.UltraWinProgressBar.ProgressBarStyle.Office2007Continuous;
            this.prgStatus.TabIndex = 7;
            this.prgStatus.Text = "[Formatted]";
            this.prgStatus.Visible = false;
            // 
            // txtLicenseKey
            // 
            this.txtLicenseKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLicenseKey.Location = new System.Drawing.Point(108, 40);
            this.txtLicenseKey.Name = "txtLicenseKey";
            this.txtLicenseKey.Size = new System.Drawing.Size(239, 22);
            this.txtLicenseKey.TabIndex = 9;
            ultraToolTipInfo1.ToolTipTextFormatted = "Enter the license key you received when you purchased the software.";
            ultraToolTipInfo1.ToolTipTitle = "License Key";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtLicenseKey, ultraToolTipInfo1);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(12, 44);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(76, 15);
            this.ultraLabel3.TabIndex = 8;
            this.ultraLabel3.Text = "License Key:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(12, 99);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(46, 15);
            this.ultraLabel4.TabIndex = 10;
            this.ultraLabel4.Text = "Status:";
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // Registration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 321);
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.txtLicenseKey);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.prgStatus);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.txtFingerprint);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtCustomerKey);
            this.Controls.Add(this.ultraLabel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Registration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Activation";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Registration_FormClosed);
            this.Load += new System.EventHandler(this.Registration_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFingerprint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLicenseKey)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomerKey;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtFingerprint;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtStatus;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar prgStatus;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtLicenseKey;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
    }
}