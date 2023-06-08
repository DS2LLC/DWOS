namespace DWOS.UI.Sales.Order
{
    partial class BasicSerialNumberWidget
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The serial number of the order\'s part.", Infragistics.Win.ToolTipImage.Default, "Serial Number", Infragistics.Win.DefaultableBoolean.Default);
            this.txtSerialNumber = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.txtSerialNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSerialNumber
            // 
            this.txtSerialNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSerialNumber.Location = new System.Drawing.Point(0, 0);
            this.txtSerialNumber.MaxLength = 20;
            this.txtSerialNumber.Name = "txtSerialNumber";
            this.txtSerialNumber.Size = new System.Drawing.Size(115, 21);
            this.txtSerialNumber.TabIndex = 0;
            ultraToolTipInfo1.ToolTipText = "The serial number of the order\'s part.";
            ultraToolTipInfo1.ToolTipTitle = "Serial Number";
            this.tipManager.SetUltraToolTip(this.txtSerialNumber, ultraToolTipInfo1);
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // BasicSerialNumberWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.txtSerialNumber);
            this.Name = "BasicSerialNumberWidget";
            this.Size = new System.Drawing.Size(115, 21);
            ((System.ComponentModel.ISupportInitialize)(this.txtSerialNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSerialNumber;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
    }
}
