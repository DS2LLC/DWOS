namespace DWOS.UI.Admin.ChangeWorkOrder
{
    partial class SelectWorkOrderPanel
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
            this.numWorkOrder = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.btnCheckWorkOrder = new Infragistics.Win.Misc.UltraButton();
            this.lblLocked = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.numWorkOrder)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(0, 7);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(66, 14);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Work Order:";
            // 
            // numWorkOrder
            // 
            this.numWorkOrder.Location = new System.Drawing.Point(72, 3);
            this.numWorkOrder.MinValue = 0;
            this.numWorkOrder.Name = "numWorkOrder";
            this.numWorkOrder.Nullable = true;
            this.numWorkOrder.Size = new System.Drawing.Size(170, 21);
            this.numWorkOrder.TabIndex = 2;
            this.numWorkOrder.KeyUp += new System.Windows.Forms.KeyEventHandler(this.numWorkOrder_KeyUp);
            // 
            // btnCheckWorkOrder
            // 
            this.btnCheckWorkOrder.Location = new System.Drawing.Point(0, 30);
            this.btnCheckWorkOrder.Name = "btnCheckWorkOrder";
            this.btnCheckWorkOrder.Size = new System.Drawing.Size(192, 23);
            this.btnCheckWorkOrder.TabIndex = 3;
            this.btnCheckWorkOrder.Text = "Check Work Order";
            this.btnCheckWorkOrder.Click += new System.EventHandler(this.btnCheckWorkOrder_Click);
            // 
            // lblLocked
            // 
            this.lblLocked.AutoSize = true;
            this.lblLocked.Location = new System.Drawing.Point(3, 59);
            this.lblLocked.Name = "lblLocked";
            this.lblLocked.Size = new System.Drawing.Size(257, 14);
            this.lblLocked.TabIndex = 4;
            this.lblLocked.Text = "This Work Order is locked. Press Next to continue.";
            this.lblLocked.Visible = false;
            // 
            // SelectWorkOrderPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblLocked);
            this.Controls.Add(this.btnCheckWorkOrder);
            this.Controls.Add(this.numWorkOrder);
            this.Controls.Add(this.ultraLabel1);
            this.Name = "SelectWorkOrderPanel";
            this.Size = new System.Drawing.Size(291, 98);
            ((System.ComponentModel.ISupportInitialize)(this.numWorkOrder)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numWorkOrder;
        private Infragistics.Win.Misc.UltraButton btnCheckWorkOrder;
        private Infragistics.Win.Misc.UltraLabel lblLocked;
    }
}
