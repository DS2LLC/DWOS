namespace DWOS.UI.Sales.Customer
{
    partial class AddRelationshipDialog
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
            this.cboRelatedCustomer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.btnAdd = new Infragistics.Win.Misc.UltraButton();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCurrentCustomer = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            ((System.ComponentModel.ISupportInitialize)(this.cboRelatedCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentCustomer)).BeginInit();
            this.SuspendLayout();
            // 
            // cboRelatedCustomer
            // 
            this.cboRelatedCustomer.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboRelatedCustomer.Location = new System.Drawing.Point(129, 40);
            this.cboRelatedCustomer.Name = "cboRelatedCustomer";
            this.cboRelatedCustomer.Size = new System.Drawing.Size(190, 22);
            this.cboRelatedCustomer.TabIndex = 2;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(163, 76);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(244, 76);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(12, 16);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(111, 15);
            this.ultraLabel1.TabIndex = 3;
            this.ultraLabel1.Text = "Current Customer:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(12, 44);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(48, 15);
            this.ultraLabel3.TabIndex = 5;
            this.ultraLabel3.Text = "Sibling:";
            // 
            // txtCurrentCustomer
            // 
            this.txtCurrentCustomer.Location = new System.Drawing.Point(129, 12);
            this.txtCurrentCustomer.Name = "txtCurrentCustomer";
            this.txtCurrentCustomer.ReadOnly = true;
            this.txtCurrentCustomer.Size = new System.Drawing.Size(190, 22);
            this.txtCurrentCustomer.TabIndex = 1;
            // 
            // AddRelationshipDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(331, 106);
            this.Controls.Add(this.txtCurrentCustomer);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.cboRelatedCustomer);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AddRelationshipDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Customer Relationship";
            ((System.ComponentModel.ISupportInitialize)(this.cboRelatedCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentCustomer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboRelatedCustomer;
        private Infragistics.Win.Misc.UltraButton btnAdd;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCurrentCustomer;
    }
}