namespace DWOS.UI.Sales
{
	partial class OrderRework
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
			if(disposing && (components != null))
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderRework));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtOrderID = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtReason = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.cboReasonCode = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReasonCode)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(204, 227);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(122, 227);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "&OK";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(12, 12);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel1.TabIndex = 4;
            this.ultraLabel1.Text = "Order:";
            // 
            // txtOrderID
            // 
            this.txtOrderID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOrderID.Location = new System.Drawing.Point(92, 8);
            this.txtOrderID.Name = "txtOrderID";
            this.txtOrderID.ReadOnly = true;
            this.txtOrderID.Size = new System.Drawing.Size(190, 22);
            this.txtOrderID.TabIndex = 5;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(12, 67);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel2.TabIndex = 6;
            this.ultraLabel2.Text = "Notes:";
            // 
            // txtReason
            // 
            this.txtReason.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReason.Location = new System.Drawing.Point(92, 64);
            this.txtReason.MaxLength = 255;
            this.txtReason.Multiline = true;
            this.txtReason.Name = "txtReason";
            this.txtReason.Size = new System.Drawing.Size(190, 157);
            this.txtReason.TabIndex = 9;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(12, 40);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(51, 15);
            this.ultraLabel3.TabIndex = 8;
            this.ultraLabel3.Text = "Reason:";
            // 
            // cboReasonCode
            // 
            this.cboReasonCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboReasonCode.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboReasonCode.Location = new System.Drawing.Point(92, 36);
            this.cboReasonCode.Name = "cboReasonCode";
            this.cboReasonCode.Size = new System.Drawing.Size(190, 22);
            this.cboReasonCode.TabIndex = 10;
            // 
            // OrderRework
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(294, 262);
            this.Controls.Add(this.cboReasonCode);
            this.Controls.Add(this.txtReason);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.txtOrderID);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OrderRework";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "External Rework";
            this.Load += new System.EventHandler(this.OrderRework_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReasonCode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtOrderID;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtReason;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboReasonCode;
	}
}