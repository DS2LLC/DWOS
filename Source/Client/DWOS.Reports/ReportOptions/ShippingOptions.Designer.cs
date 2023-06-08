namespace DWOS.Reports.ReportOptions
{
    partial class ShippingOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShippingOptions));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "From Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "To Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Single Customer", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "All Customers", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set the date to today.", Infragistics.Win.ToolTipImage.Default, "Today", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set dates to last month.", Infragistics.Win.ToolTipImage.Default, "Last Month", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set the dates to a year-to-date range.", Infragistics.Win.ToolTipImage.Default, "Year To Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Select a specific customer from the list.", Infragistics.Win.ToolTipImage.Default, "Customers", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.dteFromDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.dteToDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.chkByCustomer = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkAllCustomers = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.lblToday = new Infragistics.Win.Misc.UltraLabel();
            this.lblLastMonth = new Infragistics.Win.Misc.UltraLabel();
            this.lblYTD = new Infragistics.Win.Misc.UltraLabel();
            this.cboCustomer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.activityIndicator = new Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkByCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllCustomers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraPictureBox1
            // 
            this.ultraPictureBox1.AutoSize = true;
            this.ultraPictureBox1.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox1.Image = ((object)(resources.GetObject("ultraPictureBox1.Image")));
            this.ultraPictureBox1.Location = new System.Drawing.Point(22, 19);
            this.ultraPictureBox1.Name = "ultraPictureBox1";
            this.ultraPictureBox1.Size = new System.Drawing.Size(64, 64);
            this.ultraPictureBox1.TabIndex = 0;
            this.ultraPictureBox1.UseAppStyling = false;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(112, 48);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(69, 15);
            this.ultraLabel2.TabIndex = 46;
            this.ultraLabel2.Text = "From Date:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(254, 228);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(158, 228);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // dteFromDate
            // 
            this.dteFromDate.Location = new System.Drawing.Point(201, 44);
            this.dteFromDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dteFromDate.Name = "dteFromDate";
            this.dteFromDate.Size = new System.Drawing.Size(144, 22);
            this.dteFromDate.TabIndex = 1;
            ultraToolTipInfo8.ToolTipTextFormatted = "The date the report will look at orders opened on and after this date.";
            ultraToolTipInfo8.ToolTipTitle = "From Date";
            this.tipManager.SetUltraToolTip(this.dteFromDate, ultraToolTipInfo8);
            // 
            // dteToDate
            // 
            this.dteToDate.Location = new System.Drawing.Point(201, 72);
            this.dteToDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dteToDate.Name = "dteToDate";
            this.dteToDate.Size = new System.Drawing.Size(144, 22);
            this.dteToDate.TabIndex = 2;
            ultraToolTipInfo7.ToolTipTextFormatted = "The date the report will look at orders opened on and before this date.";
            ultraToolTipInfo7.ToolTipTitle = "To Date";
            this.tipManager.SetUltraToolTip(this.dteToDate, ultraToolTipInfo7);
            // 
            // chkByCustomer
            // 
            this.chkByCustomer.AutoSize = true;
            this.chkByCustomer.Location = new System.Drawing.Point(25, 143);
            this.chkByCustomer.Name = "chkByCustomer";
            this.chkByCustomer.Size = new System.Drawing.Size(115, 18);
            this.chkByCustomer.TabIndex = 3;
            this.chkByCustomer.Text = "Single Customer";
            ultraToolTipInfo6.ToolTipTextFormatted = "If checked, then the report will only show orders for the selected customer.";
            ultraToolTipInfo6.ToolTipTitle = "Single Customer";
            this.tipManager.SetUltraToolTip(this.chkByCustomer, ultraToolTipInfo6);
            this.chkByCustomer.CheckedChanged += new System.EventHandler(this.chkByCustomer_CheckedChanged);
            // 
            // chkAllCustomers
            // 
            this.chkAllCustomers.AutoSize = true;
            this.chkAllCustomers.Location = new System.Drawing.Point(25, 195);
            this.chkAllCustomers.Name = "chkAllCustomers";
            this.chkAllCustomers.Size = new System.Drawing.Size(100, 18);
            this.chkAllCustomers.TabIndex = 5;
            this.chkAllCustomers.Text = "All Customers";
            ultraToolTipInfo4.ToolTipTextFormatted = "If checked, then all customers will be included in the report.";
            ultraToolTipInfo4.ToolTipTitle = "All Customers";
            this.tipManager.SetUltraToolTip(this.chkAllCustomers, ultraToolTipInfo4);
            this.chkAllCustomers.CheckedChanged += new System.EventHandler(this.chkAllCustomers_CheckedChanged);
            // 
            // lblToday
            // 
            appearance3.FontData.SizeInPoints = 7F;
            appearance3.FontData.UnderlineAsString = "True";
            appearance3.ForeColor = System.Drawing.Color.Blue;
            this.lblToday.Appearance = appearance3;
            this.lblToday.AutoSize = true;
            this.lblToday.Location = new System.Drawing.Point(201, 100);
            this.lblToday.Name = "lblToday";
            this.lblToday.Size = new System.Drawing.Size(33, 13);
            this.lblToday.TabIndex = 57;
            this.lblToday.Text = "Today";
            ultraToolTipInfo3.ToolTipText = "Click to set the date to today.";
            ultraToolTipInfo3.ToolTipTitle = "Today";
            this.tipManager.SetUltraToolTip(this.lblToday, ultraToolTipInfo3);
            this.lblToday.Click += new System.EventHandler(this.lblToday_Click);
            // 
            // lblLastMonth
            // 
            appearance2.FontData.SizeInPoints = 7F;
            appearance2.FontData.UnderlineAsString = "True";
            appearance2.ForeColor = System.Drawing.Color.Blue;
            this.lblLastMonth.Appearance = appearance2;
            this.lblLastMonth.AutoSize = true;
            this.lblLastMonth.Location = new System.Drawing.Point(240, 100);
            this.lblLastMonth.Name = "lblLastMonth";
            this.lblLastMonth.Size = new System.Drawing.Size(57, 13);
            this.lblLastMonth.TabIndex = 58;
            this.lblLastMonth.Text = "Last Month";
            ultraToolTipInfo2.ToolTipText = "Click to set dates to last month.";
            ultraToolTipInfo2.ToolTipTitle = "Last Month";
            this.tipManager.SetUltraToolTip(this.lblLastMonth, ultraToolTipInfo2);
            this.lblLastMonth.Click += new System.EventHandler(this.lblLastMonth_Click);
            // 
            // lblYTD
            // 
            appearance1.FontData.SizeInPoints = 7F;
            appearance1.FontData.UnderlineAsString = "True";
            appearance1.ForeColor = System.Drawing.Color.Blue;
            this.lblYTD.Appearance = appearance1;
            this.lblYTD.Location = new System.Drawing.Point(303, 100);
            this.lblYTD.Name = "lblYTD";
            this.lblYTD.Size = new System.Drawing.Size(26, 13);
            this.lblYTD.TabIndex = 59;
            this.lblYTD.Text = "YTD";
            ultraToolTipInfo1.ToolTipText = "Click to set the dates to a year-to-date range.";
            ultraToolTipInfo1.ToolTipTitle = "Year To Date";
            this.tipManager.SetUltraToolTip(this.lblYTD, ultraToolTipInfo1);
            this.lblYTD.Click += new System.EventHandler(this.lblYTD_Click);
            // 
            // cboCustomer
            // 
            this.cboCustomer.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCustomer.Location = new System.Drawing.Point(62, 167);
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.Size = new System.Drawing.Size(283, 22);
            this.cboCustomer.TabIndex = 4;
            ultraToolTipInfo5.ToolTipText = "Select a specific customer from the list.";
            ultraToolTipInfo5.ToolTipTitle = "Customers";
            this.tipManager.SetUltraToolTip(this.cboCustomer, ultraToolTipInfo5);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(112, 76);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(54, 15);
            this.ultraLabel1.TabIndex = 48;
            this.ultraLabel1.Text = "To Date:";
            // 
            // activityIndicator
            // 
            this.activityIndicator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.activityIndicator.CausesValidation = true;
            this.activityIndicator.Location = new System.Drawing.Point(12, 228);
            this.activityIndicator.Name = "activityIndicator";
            this.activityIndicator.Size = new System.Drawing.Size(128, 23);
            this.activityIndicator.TabIndex = 54;
            this.activityIndicator.TabStop = true;
            this.activityIndicator.Visible = false;
            // 
            // ultraLabel3
            // 
            appearance4.FontData.SizeInPoints = 14F;
            this.ultraLabel3.Appearance = appearance4;
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(112, 12);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(162, 26);
            this.ultraLabel3.TabIndex = 56;
            this.ultraLabel3.Text = "Shipping Report";
            this.ultraLabel3.UseAppStyling = false;
            // 
            // ShippingOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 263);
            this.Controls.Add(this.lblYTD);
            this.Controls.Add(this.lblLastMonth);
            this.Controls.Add(this.lblToday);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.activityIndicator);
            this.Controls.Add(this.chkAllCustomers);
            this.Controls.Add(this.cboCustomer);
            this.Controls.Add(this.chkByCustomer);
            this.Controls.Add(this.dteToDate);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.dteFromDate);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.ultraPictureBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ShippingOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Report Options";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ClosedOrdersOptions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkByCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllCustomers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		protected Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkAllCustomers;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomer;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkByCustomer;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteToDate;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteFromDate;
		private Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator activityIndicator;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
		private Infragistics.Win.Misc.UltraLabel lblLastMonth;
        private Infragistics.Win.Misc.UltraLabel lblToday;
        private Infragistics.Win.Misc.UltraLabel lblYTD;
	}
}