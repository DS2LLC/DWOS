namespace DWOS.UI.Admin.OrderReset
{
	partial class MoveOrder
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Orders will be moved to this location.", Infragistics.Win.ToolTipImage.Default, "Location", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Moved orders will have this work status.", Infragistics.Win.ToolTipImage.Default, "Work Status", Infragistics.Win.DefaultableBoolean.Default);
            this.txtSelectedOrders = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.cboLocation = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboWorkStatus = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.txtSelectedOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboWorkStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSelectedOrders
            // 
            this.txtSelectedOrders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSelectedOrders.Location = new System.Drawing.Point(22, 86);
            this.txtSelectedOrders.Multiline = true;
            this.txtSelectedOrders.Name = "txtSelectedOrders";
            this.txtSelectedOrders.ReadOnly = true;
            this.txtSelectedOrders.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSelectedOrders.Size = new System.Drawing.Size(595, 288);
            this.txtSelectedOrders.TabIndex = 2;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Location = new System.Drawing.Point(3, 65);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(46, 15);
            this.ultraLabel2.TabIndex = 69;
            this.ultraLabel2.Text = "Status:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.Location = new System.Drawing.Point(3, 35);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(57, 15);
            this.ultraLabel3.TabIndex = 73;
            this.ultraLabel3.Text = "Location:";
            // 
            // cboLocation
            // 
            this.cboLocation.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            this.cboLocation.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboLocation.Location = new System.Drawing.Point(113, 31);
            this.cboLocation.Name = "cboLocation";
            this.cboLocation.Size = new System.Drawing.Size(182, 22);
            this.cboLocation.TabIndex = 1;
            ultraToolTipInfo2.ToolTipText = "Orders will be moved to this location.";
            ultraToolTipInfo2.ToolTipTitle = "Location";
            this.ultraToolTipManager.SetUltraToolTip(this.cboLocation, ultraToolTipInfo2);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Location = new System.Drawing.Point(3, 7);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(79, 15);
            this.ultraLabel1.TabIndex = 72;
            this.ultraLabel1.Text = "Work Status:";
            // 
            // cboWorkStatus
            // 
            this.cboWorkStatus.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            this.cboWorkStatus.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboWorkStatus.Location = new System.Drawing.Point(113, 3);
            this.cboWorkStatus.Name = "cboWorkStatus";
            this.cboWorkStatus.Size = new System.Drawing.Size(182, 22);
            this.cboWorkStatus.TabIndex = 0;
            ultraToolTipInfo1.ToolTipText = "Moved orders will have this work status.";
            ultraToolTipInfo1.ToolTipTitle = "Work Status";
            this.ultraToolTipManager.SetUltraToolTip(this.cboWorkStatus, ultraToolTipInfo1);
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // MoveOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.cboLocation);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.cboWorkStatus);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.txtSelectedOrders);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MoveOrder";
            this.Size = new System.Drawing.Size(633, 377);
            ((System.ComponentModel.ISupportInitialize)(this.txtSelectedOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboWorkStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSelectedOrders;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		public Infragistics.Win.Misc.UltraLabel ultraLabel3;
		public Infragistics.Win.UltraWinEditors.UltraComboEditor cboLocation;
		public Infragistics.Win.Misc.UltraLabel ultraLabel1;
		public Infragistics.Win.UltraWinEditors.UltraComboEditor cboWorkStatus;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
    }
}
