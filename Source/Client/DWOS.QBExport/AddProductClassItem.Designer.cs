namespace DWOS.QBExport
{
    partial class AddProductClassItem
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
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.txtProductClass = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtAccountingCode = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboItemType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboAccount = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtDescription = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductClass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAccountingCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboItemType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(12, 44);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(104, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Accounting Code:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(12, 16);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(86, 15);
            this.ultraLabel2.TabIndex = 1;
            this.ultraLabel2.Text = "Product Class:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(12, 100);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(55, 15);
            this.ultraLabel3.TabIndex = 2;
            this.ultraLabel3.Text = "Account:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(12, 128);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(73, 15);
            this.ultraLabel4.TabIndex = 3;
            this.ultraLabel4.Text = "Description:";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(12, 72);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(37, 15);
            this.ultraLabel5.TabIndex = 4;
            this.ultraLabel5.Text = "Type:";
            // 
            // txtProductClass
            // 
            this.txtProductClass.Location = new System.Drawing.Point(122, 12);
            this.txtProductClass.Name = "txtProductClass";
            this.txtProductClass.ReadOnly = true;
            this.txtProductClass.Size = new System.Drawing.Size(197, 22);
            this.txtProductClass.TabIndex = 5;
            // 
            // txtAccountingCode
            // 
            this.txtAccountingCode.Location = new System.Drawing.Point(122, 40);
            this.txtAccountingCode.Name = "txtAccountingCode";
            this.txtAccountingCode.Size = new System.Drawing.Size(197, 22);
            this.txtAccountingCode.TabIndex = 6;
            // 
            // cboItemType
            // 
            this.cboItemType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboItemType.Location = new System.Drawing.Point(122, 68);
            this.cboItemType.Name = "cboItemType";
            this.cboItemType.Size = new System.Drawing.Size(197, 22);
            this.cboItemType.TabIndex = 7;
            // 
            // cboAccount
            // 
            this.cboAccount.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboAccount.Location = new System.Drawing.Point(122, 96);
            this.cboAccount.Name = "cboAccount";
            this.cboAccount.Size = new System.Drawing.Size(197, 22);
            this.cboAccount.TabIndex = 8;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(122, 124);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(197, 96);
            this.txtDescription.TabIndex = 9;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(158, 226);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(244, 226);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            // 
            // AddProductClassItem
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(331, 261);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.cboAccount);
            this.Controls.Add(this.cboItemType);
            this.Controls.Add(this.txtAccountingCode);
            this.Controls.Add(this.txtProductClass);
            this.Controls.Add(this.ultraLabel5);
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraLabel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AddProductClassItem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Product Class";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.txtProductClass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAccountingCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboItemType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtProductClass;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtAccountingCode;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboItemType;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboAccount;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDescription;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraButton btnCancel;
    }
}