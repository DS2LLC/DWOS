namespace DWOS.UI.Admin.OrderReset
{
    partial class OpenOrder
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
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.cboLocation = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboWorkStatus = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtSelectedOrders = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.cboLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboWorkStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSelectedOrders)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(5, 4);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(57, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Location:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(5, 32);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(79, 15);
            this.ultraLabel2.TabIndex = 1;
            this.ultraLabel2.Text = "Work Status:";
            // 
            // cboLocation
            // 
            this.cboLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLocation.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboLocation.Location = new System.Drawing.Point(90, 4);
            this.cboLocation.Name = "cboLocation";
            this.cboLocation.Size = new System.Drawing.Size(517, 22);
            this.cboLocation.TabIndex = 2;
            // 
            // cboWorkStatus
            // 
            this.cboWorkStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboWorkStatus.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboWorkStatus.Location = new System.Drawing.Point(90, 32);
            this.cboWorkStatus.Name = "cboWorkStatus";
            this.cboWorkStatus.Size = new System.Drawing.Size(517, 22);
            this.cboWorkStatus.TabIndex = 3;
            // 
            // txtSelectedOrders
            // 
            this.txtSelectedOrders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSelectedOrders.Location = new System.Drawing.Point(5, 92);
            this.txtSelectedOrders.Multiline = true;
            this.txtSelectedOrders.Name = "txtSelectedOrders";
            this.txtSelectedOrders.ReadOnly = true;
            this.txtSelectedOrders.Size = new System.Drawing.Size(602, 315);
            this.txtSelectedOrders.TabIndex = 4;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(5, 64);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(46, 15);
            this.ultraLabel3.TabIndex = 5;
            this.ultraLabel3.Text = "Status:";
            // 
            // OpenOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.txtSelectedOrders);
            this.Controls.Add(this.cboWorkStatus);
            this.Controls.Add(this.cboLocation);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraLabel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "OpenOrder";
            this.Size = new System.Drawing.Size(610, 410);
            ((System.ComponentModel.ISupportInitialize)(this.cboLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboWorkStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSelectedOrders)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboLocation;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboWorkStatus;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSelectedOrders;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
    }
}
