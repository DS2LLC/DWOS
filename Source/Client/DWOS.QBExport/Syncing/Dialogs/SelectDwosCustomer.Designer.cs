namespace DWOS.QBExport.Syncing.Dialogs
{
    partial class SelectDwosCustomer
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
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCustomer = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.chkNew = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkExisting = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cboDwosCustomer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOk = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNew)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkExisting)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDwosCustomer)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(12, 16);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(134, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "QuickBooks Customer:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(12, 72);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(105, 15);
            this.ultraLabel2.TabIndex = 1;
            this.ultraLabel2.Text = "DWOS Customer:";
            // 
            // txtCustomer
            // 
            this.txtCustomer.Location = new System.Drawing.Point(152, 12);
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.ReadOnly = true;
            this.txtCustomer.Size = new System.Drawing.Size(234, 22);
            this.txtCustomer.TabIndex = 2;
            // 
            // chkNew
            // 
            this.chkNew.AutoSize = true;
            this.chkNew.Checked = true;
            this.chkNew.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNew.Location = new System.Drawing.Point(12, 101);
            this.chkNew.Name = "chkNew";
            this.chkNew.Size = new System.Drawing.Size(146, 18);
            this.chkNew.TabIndex = 7;
            this.chkNew.Text = "Create New Customer";
            this.chkNew.CheckedChanged += new System.EventHandler(this.chkNew_CheckedChanged);
            // 
            // chkExisting
            // 
            this.chkExisting.AutoSize = true;
            this.chkExisting.Location = new System.Drawing.Point(12, 40);
            this.chkExisting.Name = "chkExisting";
            this.chkExisting.Size = new System.Drawing.Size(150, 18);
            this.chkExisting.TabIndex = 5;
            this.chkExisting.Text = "Use Existing Customer";
            this.chkExisting.CheckedChanged += new System.EventHandler(this.chkExisting_CheckedChanged);
            // 
            // cboDwosCustomer
            // 
            this.cboDwosCustomer.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboDwosCustomer.Enabled = false;
            this.cboDwosCustomer.Location = new System.Drawing.Point(152, 68);
            this.cboDwosCustomer.Name = "cboDwosCustomer";
            this.cboDwosCustomer.Size = new System.Drawing.Size(234, 22);
            this.cboDwosCustomer.TabIndex = 6;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(311, 138);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(230, 138);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "OK";
            // 
            // SelectDwosCustomer
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 173);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.chkNew);
            this.Controls.Add(this.chkExisting);
            this.Controls.Add(this.cboDwosCustomer);
            this.Controls.Add(this.txtCustomer);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraLabel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SelectDwosCustomer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select DWOS Customer";
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNew)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkExisting)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDwosCustomer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomer;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkNew;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkExisting;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboDwosCustomer;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOk;
    }
}