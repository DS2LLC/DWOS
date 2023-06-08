namespace DWOS.UI.Admin.FieldMigration
{
    partial class FieldSelectPanel
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
            this.rbProductClass = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(3, 3);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(226, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Select a field to perform migration for:";
            // 
            // rbProductClass
            // 
            this.rbProductClass.AutoSize = true;
            this.rbProductClass.Checked = true;
            this.rbProductClass.Location = new System.Drawing.Point(3, 28);
            this.rbProductClass.Name = "rbProductClass";
            this.rbProductClass.Size = new System.Drawing.Size(103, 17);
            this.rbProductClass.TabIndex = 1;
            this.rbProductClass.TabStop = true;
            this.rbProductClass.Text = "Product Class";
            this.rbProductClass.UseVisualStyleBackColor = true;
            // 
            // FieldSelectPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rbProductClass);
            this.Controls.Add(this.ultraLabel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FieldSelectPanel";
            this.Size = new System.Drawing.Size(269, 52);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private System.Windows.Forms.RadioButton rbProductClass;
    }
}
