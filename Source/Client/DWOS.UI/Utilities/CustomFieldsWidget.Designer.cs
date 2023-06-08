namespace DWOS.UI.Utilities
{
    partial class CustomFieldsWidget
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
            this.tableCustomFields = new System.Windows.Forms.TableLayoutPanel();
            this.pnlCustomFields = new Infragistics.Win.Misc.UltraPanel();
            this.pnlCustomFields.ClientArea.SuspendLayout();
            this.pnlCustomFields.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableCustomFields
            // 
            this.tableCustomFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableCustomFields.AutoSize = true;
            this.tableCustomFields.ColumnCount = 2;
            this.tableCustomFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tableCustomFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableCustomFields.Location = new System.Drawing.Point(0, 0);
            this.tableCustomFields.Margin = new System.Windows.Forms.Padding(0);
            this.tableCustomFields.Name = "tableCustomFields";
            this.tableCustomFields.RowCount = 1;
            this.tableCustomFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableCustomFields.Size = new System.Drawing.Size(280, 28);
            this.tableCustomFields.TabIndex = 0;
            // 
            // pnlCustomFields
            // 
            this.pnlCustomFields.AutoScroll = true;
            this.pnlCustomFields.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            // 
            // pnlCustomFields.ClientArea
            // 
            this.pnlCustomFields.ClientArea.Controls.Add(this.tableCustomFields);
            this.pnlCustomFields.Location = new System.Drawing.Point(0, 0);
            this.pnlCustomFields.Margin = new System.Windows.Forms.Padding(0);
            this.pnlCustomFields.Name = "pnlCustomFields";
            this.pnlCustomFields.Size = new System.Drawing.Size(286, 29);
            this.pnlCustomFields.TabIndex = 2;
            this.pnlCustomFields.UseAppStyling = false;
            // 
            // CustomFieldsWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.pnlCustomFields);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "CustomFieldsWidget";
            this.Size = new System.Drawing.Size(286, 34);
            this.pnlCustomFields.ClientArea.ResumeLayout(false);
            this.pnlCustomFields.ClientArea.PerformLayout();
            this.pnlCustomFields.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableCustomFields;
        private Infragistics.Win.Misc.UltraPanel pnlCustomFields;
    }
}
