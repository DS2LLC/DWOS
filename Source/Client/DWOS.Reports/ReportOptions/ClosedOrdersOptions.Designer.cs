namespace DWOS.Reports.ReportOptions
{
	partial class ClosedOrdersOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClosedOrdersOptions));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo19 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "From Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo18 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "To Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo17 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Single Customer", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo16 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "All Customers", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo15 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Include Boxed Orders", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo14 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set the date to today.", Infragistics.Win.ToolTipImage.Default, "Today", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo13 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set dates to last month.", Infragistics.Win.ToolTipImage.Default, "Last Month", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Select a specific customer from the list of available customers.", Infragistics.Win.ToolTipImage.Default, "Customer", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo12 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set dates to a year-to-date range.", Infragistics.Win.ToolTipImage.Default, "Year To Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo11 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("If checked, groups orders by product class instead of customer.", Infragistics.Win.ToolTipImage.Default, "Group By Product Class", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.dteFromDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.dteToDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.chkByCustomer = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkAllCustomers = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkInculdeBoxed = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.lblToday = new Infragistics.Win.Misc.UltraLabel();
            this.lblLastMonth = new Infragistics.Win.Misc.UltraLabel();
            this.cboCustomer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.lblYTD = new Infragistics.Win.Misc.UltraLabel();
            this.chkProductClass = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.activityIndicator = new Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkByCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllCustomers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkInculdeBoxed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkProductClass)).BeginInit();
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
            this.btnCancel.Location = new System.Drawing.Point(254, 241);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(158, 241);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 8;
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
            ultraToolTipInfo19.ToolTipTextFormatted = "The date the report will look at orders closed on and after this date.";
            ultraToolTipInfo19.ToolTipTitle = "From Date";
            this.tipManager.SetUltraToolTip(this.dteFromDate, ultraToolTipInfo19);
            // 
            // dteToDate
            // 
            this.dteToDate.Location = new System.Drawing.Point(201, 72);
            this.dteToDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dteToDate.Name = "dteToDate";
            this.dteToDate.Size = new System.Drawing.Size(144, 22);
            this.dteToDate.TabIndex = 2;
            ultraToolTipInfo18.ToolTipTextFormatted = "The date the report will look at orders closed on and before this date.";
            ultraToolTipInfo18.ToolTipTitle = "To Date";
            this.tipManager.SetUltraToolTip(this.dteToDate, ultraToolTipInfo18);
            // 
            // chkByCustomer
            // 
            this.chkByCustomer.AutoSize = true;
            this.chkByCustomer.Location = new System.Drawing.Point(25, 114);
            this.chkByCustomer.Name = "chkByCustomer";
            this.chkByCustomer.Size = new System.Drawing.Size(115, 18);
            this.chkByCustomer.TabIndex = 3;
            this.chkByCustomer.Text = "Single Customer";
            ultraToolTipInfo17.ToolTipTextFormatted = "If checked, then the report will only show orders for the selected customer.";
            ultraToolTipInfo17.ToolTipTitle = "Single Customer";
            this.tipManager.SetUltraToolTip(this.chkByCustomer, ultraToolTipInfo17);
            this.chkByCustomer.CheckedChanged += new System.EventHandler(this.chkByCustomer_CheckedChanged);
            // 
            // chkAllCustomers
            // 
            this.chkAllCustomers.AutoSize = true;
            this.chkAllCustomers.Location = new System.Drawing.Point(25, 166);
            this.chkAllCustomers.Name = "chkAllCustomers";
            this.chkAllCustomers.Size = new System.Drawing.Size(100, 18);
            this.chkAllCustomers.TabIndex = 5;
            this.chkAllCustomers.Text = "All Customers";
            ultraToolTipInfo16.ToolTipTextFormatted = "If checked, then all customers will be included in the report.";
            ultraToolTipInfo16.ToolTipTitle = "All Customers";
            this.tipManager.SetUltraToolTip(this.chkAllCustomers, ultraToolTipInfo16);
            this.chkAllCustomers.CheckedChanged += new System.EventHandler(this.chkAllCustomers_CheckedChanged);
            // 
            // chkInculdeBoxed
            // 
            this.chkInculdeBoxed.AutoSize = true;
            this.chkInculdeBoxed.Location = new System.Drawing.Point(62, 190);
            this.chkInculdeBoxed.Name = "chkInculdeBoxed";
            this.chkInculdeBoxed.Size = new System.Drawing.Size(145, 18);
            this.chkInculdeBoxed.TabIndex = 6;
            this.chkInculdeBoxed.Text = "Include Boxed Orders";
            ultraToolTipInfo15.ToolTipTextFormatted = "If checked, the report will include orders that are currently in <br/>shipping in" +
    " a box but not closed out yet.<br/>";
            ultraToolTipInfo15.ToolTipTitle = "Include Boxed Orders";
            this.tipManager.SetUltraToolTip(this.chkInculdeBoxed, ultraToolTipInfo15);
            // 
            // lblToday
            // 
            appearance7.FontData.SizeInPoints = 7F;
            appearance7.FontData.UnderlineAsString = "True";
            appearance7.ForeColor = System.Drawing.Color.Blue;
            this.lblToday.Appearance = appearance7;
            this.lblToday.AutoSize = true;
            this.lblToday.Location = new System.Drawing.Point(201, 100);
            this.lblToday.Name = "lblToday";
            this.lblToday.Size = new System.Drawing.Size(33, 13);
            this.lblToday.TabIndex = 57;
            this.lblToday.Text = "Today";
            ultraToolTipInfo14.ToolTipText = "Click to set the date to today.";
            ultraToolTipInfo14.ToolTipTitle = "Today";
            this.tipManager.SetUltraToolTip(this.lblToday, ultraToolTipInfo14);
            this.lblToday.Click += new System.EventHandler(this.lblToday_Click);
            // 
            // lblLastMonth
            // 
            appearance6.FontData.SizeInPoints = 7F;
            appearance6.FontData.UnderlineAsString = "True";
            appearance6.ForeColor = System.Drawing.Color.Blue;
            this.lblLastMonth.Appearance = appearance6;
            this.lblLastMonth.AutoSize = true;
            this.lblLastMonth.Location = new System.Drawing.Point(240, 100);
            this.lblLastMonth.Name = "lblLastMonth";
            this.lblLastMonth.Size = new System.Drawing.Size(57, 13);
            this.lblLastMonth.TabIndex = 58;
            this.lblLastMonth.Text = "Last Month";
            ultraToolTipInfo13.ToolTipText = "Click to set dates to last month.";
            ultraToolTipInfo13.ToolTipTitle = "Last Month";
            this.tipManager.SetUltraToolTip(this.lblLastMonth, ultraToolTipInfo13);
            this.lblLastMonth.Click += new System.EventHandler(this.lblLastMonth_Click);
            // 
            // cboCustomer
            // 
            this.cboCustomer.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCustomer.Location = new System.Drawing.Point(62, 138);
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.Size = new System.Drawing.Size(283, 22);
            this.cboCustomer.TabIndex = 4;
            ultraToolTipInfo7.ToolTipText = "Select a specific customer from the list of available customers.";
            ultraToolTipInfo7.ToolTipTitle = "Customer";
            this.tipManager.SetUltraToolTip(this.cboCustomer, ultraToolTipInfo7);
            // 
            // lblYTD
            // 
            appearance5.FontData.SizeInPoints = 7F;
            appearance5.FontData.UnderlineAsString = "True";
            appearance5.ForeColor = System.Drawing.Color.Blue;
            this.lblYTD.Appearance = appearance5;
            this.lblYTD.Location = new System.Drawing.Point(303, 100);
            this.lblYTD.Name = "lblYTD";
            this.lblYTD.Size = new System.Drawing.Size(29, 14);
            this.lblYTD.TabIndex = 59;
            this.lblYTD.Text = "YTD";
            ultraToolTipInfo12.ToolTipText = "Click to set dates to a year-to-date range.";
            ultraToolTipInfo12.ToolTipTitle = "Year To Date";
            this.tipManager.SetUltraToolTip(this.lblYTD, ultraToolTipInfo12);
            this.lblYTD.Click += new System.EventHandler(this.lblYTD_Click);
            // 
            // chkProductClass
            // 
            this.chkProductClass.AutoSize = true;
            this.chkProductClass.Location = new System.Drawing.Point(62, 214);
            this.chkProductClass.Name = "chkProductClass";
            this.chkProductClass.Size = new System.Drawing.Size(155, 18);
            this.chkProductClass.TabIndex = 7;
            this.chkProductClass.Text = "Group By Product Class";
            ultraToolTipInfo11.ToolTipText = "If checked, groups orders by product class instead of customer.";
            ultraToolTipInfo11.ToolTipTitle = "Group By Product Class";
            this.tipManager.SetUltraToolTip(this.chkProductClass, ultraToolTipInfo11);
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
            this.activityIndicator.Location = new System.Drawing.Point(12, 241);
            this.activityIndicator.Name = "activityIndicator";
            this.activityIndicator.Size = new System.Drawing.Size(128, 23);
            this.activityIndicator.TabIndex = 54;
            this.activityIndicator.TabStop = true;
            this.activityIndicator.Visible = false;
            // 
            // ultraLabel3
            // 
            appearance8.FontData.SizeInPoints = 14F;
            this.ultraLabel3.Appearance = appearance8;
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(112, 12);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(213, 26);
            this.ultraLabel3.TabIndex = 56;
            this.ultraLabel3.Text = "Closed Orders Report";
            this.ultraLabel3.UseAppStyling = false;
            // 
            // ClosedOrdersOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 276);
            this.Controls.Add(this.chkProductClass);
            this.Controls.Add(this.lblYTD);
            this.Controls.Add(this.lblLastMonth);
            this.Controls.Add(this.lblToday);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.activityIndicator);
            this.Controls.Add(this.chkInculdeBoxed);
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
            this.Name = "ClosedOrdersOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Report Options";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ClosedOrdersOptions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkByCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllCustomers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkInculdeBoxed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkProductClass)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		protected Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkInculdeBoxed;
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
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkProductClass;
    }
}