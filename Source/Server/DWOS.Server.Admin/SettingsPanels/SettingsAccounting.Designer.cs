namespace DWOS.Server.Admin.SettingsPanels
{
    partial class SettingsAccounting
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
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.chkEnabled = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkAutoInvoice = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.btnBrowseError = new Infragistics.Win.Misc.UltraButton();
            this.txtError = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnBrowseSave = new Infragistics.Win.Misc.UltraButton();
            this.txtSave = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.numUpdateIntervalMin = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.numCleanInvoices = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkEnabled)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoInvoice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpdateIntervalMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCleanInvoices)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.ultraLabel7);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel6);
            this.ultraGroupBox1.Controls.Add(this.numCleanInvoices);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel5);
            this.ultraGroupBox1.Controls.Add(this.chkEnabled);
            this.ultraGroupBox1.Controls.Add(this.chkAutoInvoice);
            this.ultraGroupBox1.Controls.Add(this.btnBrowseError);
            this.ultraGroupBox1.Controls.Add(this.txtError);
            this.ultraGroupBox1.Controls.Add(this.btnBrowseSave);
            this.ultraGroupBox1.Controls.Add(this.txtSave);
            this.ultraGroupBox1.Controls.Add(this.numUpdateIntervalMin);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel4);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(493, 227);
            this.ultraGroupBox1.TabIndex = 0;
            this.ultraGroupBox1.Text = "Accounting Settings";
            // 
            // chkEnabled
            // 
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.Location = new System.Drawing.Point(6, 48);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(175, 18);
            this.chkEnabled.TabIndex = 0;
            this.chkEnabled.Text = "Enable SYSPRO integration";
            this.chkEnabled.CheckedChanged += new System.EventHandler(this.chkEnabled_CheckedChanged);
            // 
            // chkAutoInvoice
            // 
            this.chkAutoInvoice.AutoSize = true;
            this.chkAutoInvoice.Enabled = false;
            this.chkAutoInvoice.Location = new System.Drawing.Point(6, 184);
            this.chkAutoInvoice.Name = "chkAutoInvoice";
            this.chkAutoInvoice.Size = new System.Drawing.Size(174, 18);
            this.chkAutoInvoice.TabIndex = 7;
            this.chkAutoInvoice.Text = "Enable automatic invoicing";
            // 
            // btnBrowseError
            // 
            this.btnBrowseError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseError.Enabled = false;
            this.btnBrowseError.Location = new System.Drawing.Point(412, 157);
            this.btnBrowseError.Name = "btnBrowseError";
            this.btnBrowseError.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseError.TabIndex = 6;
            this.btnBrowseError.Text = "Browse...";
            this.btnBrowseError.Click += new System.EventHandler(this.btnBrowseError_Click);
            // 
            // txtError
            // 
            this.txtError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtError.Location = new System.Drawing.Point(147, 156);
            this.txtError.Name = "txtError";
            this.txtError.ReadOnly = true;
            this.txtError.Size = new System.Drawing.Size(259, 22);
            this.txtError.TabIndex = 5;
            // 
            // btnBrowseSave
            // 
            this.btnBrowseSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseSave.Enabled = false;
            this.btnBrowseSave.Location = new System.Drawing.Point(412, 128);
            this.btnBrowseSave.Name = "btnBrowseSave";
            this.btnBrowseSave.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseSave.TabIndex = 4;
            this.btnBrowseSave.Text = "Browse...";
            this.btnBrowseSave.Click += new System.EventHandler(this.btnBrowseSave_Click);
            // 
            // txtSave
            // 
            this.txtSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSave.Location = new System.Drawing.Point(147, 128);
            this.txtSave.Name = "txtSave";
            this.txtSave.ReadOnly = true;
            this.txtSave.Size = new System.Drawing.Size(259, 22);
            this.txtSave.TabIndex = 3;
            // 
            // numUpdateIntervalMin
            // 
            this.numUpdateIntervalMin.Enabled = false;
            this.numUpdateIntervalMin.Location = new System.Drawing.Point(147, 72);
            this.numUpdateIntervalMin.MinValue = 1;
            this.numUpdateIntervalMin.Name = "numUpdateIntervalMin";
            this.numUpdateIntervalMin.Size = new System.Drawing.Size(91, 22);
            this.numUpdateIntervalMin.TabIndex = 1;
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(6, 160);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(94, 15);
            this.ultraLabel4.TabIndex = 3;
            this.ultraLabel4.Text = "Error Directory:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(6, 133);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(94, 15);
            this.ultraLabel3.TabIndex = 2;
            this.ultraLabel3.Text = "Save Directory:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(3, 76);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(98, 15);
            this.ultraLabel2.TabIndex = 1;
            this.ultraLabel2.Text = "Update Interval:";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel1.Location = new System.Drawing.Point(6, 27);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(116, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "SYSPRO Settings:";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(3, 104);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(104, 15);
            this.ultraLabel5.TabIndex = 7;
            this.ultraLabel5.Text = "Cleanup Interval:";
            // 
            // numCleanInvoices
            // 
            this.numCleanInvoices.Enabled = false;
            this.numCleanInvoices.FormatString = "";
            this.numCleanInvoices.Location = new System.Drawing.Point(147, 97);
            this.numCleanInvoices.MinValue = 1;
            this.numCleanInvoices.Name = "numCleanInvoices";
            this.numCleanInvoices.Size = new System.Drawing.Size(91, 22);
            this.numCleanInvoices.TabIndex = 2;
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(244, 76);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(60, 15);
            this.ultraLabel6.TabIndex = 8;
            this.ultraLabel6.Text = "minute(s)";
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(244, 101);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(41, 15);
            this.ultraLabel7.TabIndex = 9;
            this.ultraLabel7.Text = "day(s)";
            // 
            // SettingsAccounting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox1);
            this.Name = "SettingsAccounting";
            this.Size = new System.Drawing.Size(493, 227);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkEnabled)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoInvoice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpdateIntervalMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCleanInvoices)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraButton btnBrowseError;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtError;
        private Infragistics.Win.Misc.UltraButton btnBrowseSave;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSave;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numUpdateIntervalMin;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkAutoInvoice;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkEnabled;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numCleanInvoices;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
    }
}
