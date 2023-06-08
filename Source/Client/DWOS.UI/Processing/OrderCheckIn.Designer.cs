namespace DWOS.UI
{
    partial class OrderCheckIn
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Department", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Work Order", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderCheckIn));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtDepartment = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lableWO = new Infragistics.Win.Misc.UltraLabel();
            this.picPartImage = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.taMedia = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.MediaTableAdapter();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.cboOrder = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            this.taOrderWorkStatusSummary = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderWorkStatusSummaryTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOrder)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(294, 213);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(212, 213);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(12, 40);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(77, 15);
            this.ultraLabel1.TabIndex = 25;
            this.ultraLabel1.Text = "Department:";
            // 
            // txtDepartment
            // 
            this.txtDepartment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDepartment.Location = new System.Drawing.Point(96, 36);
            this.txtDepartment.Name = "txtDepartment";
            this.txtDepartment.ReadOnly = true;
            this.txtDepartment.Size = new System.Drawing.Size(276, 22);
            this.txtDepartment.TabIndex = 1;
            this.txtDepartment.TabStop = false;
            ultraToolTipInfo2.ToolTipTextFormatted = "The current department that the order is being checked into.";
            ultraToolTipInfo2.ToolTipTitle = "Department";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtDepartment, ultraToolTipInfo2);
            // 
            // lableWO
            // 
            this.lableWO.AutoSize = true;
            this.lableWO.Location = new System.Drawing.Point(12, 12);
            this.lableWO.Name = "lableWO";
            this.lableWO.Size = new System.Drawing.Size(75, 15);
            this.lableWO.TabIndex = 27;
            this.lableWO.Text = "Work Order:";
            // 
            // picPartImage
            // 
            this.picPartImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.picPartImage.Appearance = appearance1;
            this.picPartImage.BorderShadowColor = System.Drawing.Color.Empty;
            this.picPartImage.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.picPartImage.DefaultImage = global::DWOS.UI.Properties.Resources.NoImage;
            this.picPartImage.Location = new System.Drawing.Point(96, 64);
            this.picPartImage.Name = "picPartImage";
            this.picPartImage.Size = new System.Drawing.Size(276, 143);
            this.picPartImage.TabIndex = 2;
            // 
            // taMedia
            // 
            this.taMedia.ClearBeforeFill = true;
            // 
            // cboOrder
            // 
            this.cboOrder.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
            this.cboOrder.DisplayMember = "OrderID";
            this.cboOrder.Location = new System.Drawing.Point(96, 9);
            this.cboOrder.Name = "cboOrder";
            this.cboOrder.Size = new System.Drawing.Size(276, 22);
            this.cboOrder.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboOrder.TabIndex = 0;
            ultraToolTipInfo1.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo1.ToolTipTextFormatted");
            ultraToolTipInfo1.ToolTipTitle = "Work Order";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboOrder, ultraToolTipInfo1);
            this.cboOrder.ValueMember = "OrderID";
            this.cboOrder.ValueChanged += new System.EventHandler(this.cboOrder_ValueChanged);
            this.cboOrder.TextChanged += new System.EventHandler(this.cboOrder_TextChanged);
            this.cboOrder.Leave += new System.EventHandler(this.cboOrder_TextChanged);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // helpLink1
            // 
            this.helpLink1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = "order_check_in_dialog.htm";
            this.helpLink1.Location = new System.Drawing.Point(12, 220);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 28;
            // 
            // taOrderWorkStatusSummary
            // 
            this.taOrderWorkStatusSummary.ClearBeforeFill = true;
            // 
            // OrderCheckIn
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 248);
            this.Controls.Add(this.helpLink1);
            this.Controls.Add(this.cboOrder);
            this.Controls.Add(this.picPartImage);
            this.Controls.Add(this.lableWO);
            this.Controls.Add(this.txtDepartment);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "OrderCheckIn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Order Check In";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PartCheckIn_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOrder)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDepartment;
		private Infragistics.Win.Misc.UltraLabel lableWO;
		private Infragistics.Win.UltraWinEditors.UltraPictureBox picPartImage;
		private DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.MediaTableAdapter taMedia;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboOrder;
		private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Utilities.HelpLink helpLink1;
        private Data.Datasets.OrderProcessingDataSetTableAdapters.OrderWorkStatusSummaryTableAdapter taOrderWorkStatusSummary;
    }
}