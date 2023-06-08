namespace DWOS.QBExport.Syncing.Dialogs
{
    partial class SelectQuickBooksCustomer
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Confirm customer selection.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Sync DWOS customer with existing Quickbooks customer.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Cancel the sync operation.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Create new customer in Quickbooks.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Skip syncing this customer.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.btnOk = new Infragistics.Win.Misc.UltraButton();
            this.cboQuickBooksCustomer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtCustomer = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.chkExisting = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.chkNew = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.skipBtn = new Infragistics.Win.Misc.UltraButton();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.cboQuickBooksCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkExisting)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNew)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(12, 72);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(134, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "QuickBooks Customer:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(12, 16);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(105, 15);
            this.ultraLabel2.TabIndex = 1;
            this.ultraLabel2.Text = "DWOS Customer:";
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(149, 139);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "OK";
            ultraToolTipInfo5.ToolTipText = "Confirm customer selection.";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnOk, ultraToolTipInfo5);
            // 
            // cboQuickBooksCustomer
            // 
            this.cboQuickBooksCustomer.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboQuickBooksCustomer.Enabled = false;
            this.cboQuickBooksCustomer.Location = new System.Drawing.Point(152, 68);
            this.cboQuickBooksCustomer.Name = "cboQuickBooksCustomer";
            this.cboQuickBooksCustomer.Size = new System.Drawing.Size(234, 22);
            this.cboQuickBooksCustomer.TabIndex = 3;
            // 
            // txtCustomer
            // 
            this.txtCustomer.Location = new System.Drawing.Point(152, 12);
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.ReadOnly = true;
            this.txtCustomer.Size = new System.Drawing.Size(234, 22);
            this.txtCustomer.TabIndex = 1;
            // 
            // chkExisting
            // 
            this.chkExisting.AutoSize = true;
            this.chkExisting.Location = new System.Drawing.Point(12, 40);
            this.chkExisting.Name = "chkExisting";
            this.chkExisting.Size = new System.Drawing.Size(150, 18);
            this.chkExisting.TabIndex = 2;
            this.chkExisting.Text = "Use Existing Customer";
            ultraToolTipInfo4.ToolTipText = "Sync DWOS customer with existing Quickbooks customer.";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkExisting, ultraToolTipInfo4);
            this.chkExisting.CheckedChanged += new System.EventHandler(this.chkExisting_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(311, 139);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            ultraToolTipInfo3.ToolTipText = "Cancel the sync operation.";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnCancel, ultraToolTipInfo3);
            // 
            // chkNew
            // 
            this.chkNew.AutoSize = true;
            this.chkNew.Location = new System.Drawing.Point(12, 101);
            this.chkNew.Name = "chkNew";
            this.chkNew.Size = new System.Drawing.Size(146, 18);
            this.chkNew.TabIndex = 4;
            this.chkNew.Text = "Create New Customer";
            ultraToolTipInfo2.ToolTipText = "Create new customer in Quickbooks.";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkNew, ultraToolTipInfo2);
            this.chkNew.CheckedChanged += new System.EventHandler(this.chkNew_CheckedChanged);
            // 
            // skipBtn
            // 
            this.skipBtn.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.skipBtn.Location = new System.Drawing.Point(230, 139);
            this.skipBtn.Name = "skipBtn";
            this.skipBtn.Size = new System.Drawing.Size(75, 23);
            this.skipBtn.TabIndex = 7;
            this.skipBtn.Text = "Skip";
            ultraToolTipInfo1.ToolTipText = "Skip syncing this customer.";
            this.ultraToolTipManager1.SetUltraToolTip(this.skipBtn, ultraToolTipInfo1);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // SelectQuickBooksCustomer
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 173);
            this.Controls.Add(this.skipBtn);
            this.Controls.Add(this.chkNew);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.chkExisting);
            this.Controls.Add(this.txtCustomer);
            this.Controls.Add(this.cboQuickBooksCustomer);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraLabel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SelectQuickBooksCustomer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select QuickBooks Customer";
            ((System.ComponentModel.ISupportInitialize)(this.cboQuickBooksCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkExisting)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNew)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraButton btnOk;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboQuickBooksCustomer;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomer;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkExisting;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkNew;
        private Infragistics.Win.Misc.UltraButton skipBtn;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
    }
}