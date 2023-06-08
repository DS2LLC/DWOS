namespace DWOS.UI.Sales.Order
{
    partial class SerialNumberQuickViewPanel
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.advSerialNumbers = new DWOS.UI.Sales.Order.AdvancedSerialNumberWidget();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.advSerialNumbers);
            appearance1.Image = global::DWOS.UI.Properties.Resources.View;
            this.grpData.HeaderAppearance = appearance1;
            this.grpData.Text = "Serial Numbers";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.advSerialNumbers, 0);
            // 
            // advSerialNumbers
            // 
            this.advSerialNumbers.Dataset = null;
            this.advSerialNumbers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advSerialNumbers.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.advSerialNumbers.Location = new System.Drawing.Point(8, 23);
            this.advSerialNumbers.Name = "advSerialNumbers";
            this.advSerialNumbers.ReadOnly = false;
            this.advSerialNumbers.Size = new System.Drawing.Size(430, 533);
            this.advSerialNumbers.TabIndex = 1;
            // 
            // SerialNumberPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "SerialNumberPanel";
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AdvancedSerialNumberWidget advSerialNumbers;
    }
}
