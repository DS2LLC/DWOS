namespace DWOS.UI.Admin.FieldMigration
{
    partial class CustomFieldSelectPanel
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
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboCustomField = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomField)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(3, 3);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(263, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Select a custom field to retrieve values from:";
            // 
            // cboCustomField
            // 
            this.cboCustomField.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCustomField.Location = new System.Drawing.Point(3, 23);
            this.cboCustomField.Name = "cboCustomField";
            this.cboCustomField.Size = new System.Drawing.Size(264, 22);
            this.cboCustomField.TabIndex = 1;
            this.cboCustomField.ValueChanged += new System.EventHandler(this.cboCustomField_ValueChanged);
            // 
            // CustomFieldSelectPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboCustomField);
            this.Controls.Add(this.ultraLabel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "CustomFieldSelectPanel";
            this.Size = new System.Drawing.Size(287, 57);
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomField)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomField;
    }
}
