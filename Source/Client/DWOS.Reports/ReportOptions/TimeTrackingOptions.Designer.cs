namespace DWOS.Reports.ReportOptions
{
    partial class TimeTrackingOptions
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set dates to a year-to-date range.", Infragistics.Win.ToolTipImage.Default, "Year To Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set dates to last month.", Infragistics.Win.ToolTipImage.Default, "Last Month", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set the date to today.", Infragistics.Win.ToolTipImage.Default, "Today", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("If checked, then all customers will be included in the report.", Infragistics.Win.ToolTipImage.Default, "All Customers", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Select a specific customer from the list of available customers.", Infragistics.Win.ToolTipImage.Default, "Customer", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("If checked, then the report will only show orders for the selected customer.", Infragistics.Win.ToolTipImage.Default, "Single Customer", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "To Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The report will include orders that were processed on or after this date.", Infragistics.Win.ToolTipImage.Default, "From Date", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimeTrackingOptions));
            this.lblYTD = new Infragistics.Win.Misc.UltraLabel();
            this.lblLastMonth = new Infragistics.Win.Misc.UltraLabel();
            this.lblToday = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.activityIndicator = new Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator();
            this.chkAllCustomers = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cboCustomer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.chkByCustomer = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.dteToDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.dteFromDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.chkAllCustomers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkByCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate)).BeginInit();
            this.SuspendLayout();
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
            this.lblYTD.TabIndex = 74;
            this.lblYTD.Text = "YTD";
            ultraToolTipInfo8.ToolTipText = "Click to set dates to a year-to-date range.";
            ultraToolTipInfo8.ToolTipTitle = "Year To Date";
            this.ultraToolTipManager.SetUltraToolTip(this.lblYTD, ultraToolTipInfo8);
            this.lblYTD.Click += new System.EventHandler(this.lblYTD_Click);
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
            this.lblLastMonth.TabIndex = 73;
            this.lblLastMonth.Text = "Last Month";
            ultraToolTipInfo1.ToolTipText = "Click to set dates to last month.";
            ultraToolTipInfo1.ToolTipTitle = "Last Month";
            this.ultraToolTipManager.SetUltraToolTip(this.lblLastMonth, ultraToolTipInfo1);
            this.lblLastMonth.Click += new System.EventHandler(this.lblLastMonth_Click);
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
            this.lblToday.TabIndex = 72;
            this.lblToday.Text = "Today";
            ultraToolTipInfo2.ToolTipText = "Click to set the date to today.";
            ultraToolTipInfo2.ToolTipTitle = "Today";
            this.ultraToolTipManager.SetUltraToolTip(this.lblToday, ultraToolTipInfo2);
            this.lblToday.Click += new System.EventHandler(this.lblToday_Click);
            // 
            // ultraLabel3
            // 
            appearance4.FontData.SizeInPoints = 14F;
            this.ultraLabel3.Appearance = appearance4;
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(112, 12);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(214, 26);
            this.ultraLabel3.TabIndex = 71;
            this.ultraLabel3.Text = "Time Tracking Report";
            this.ultraLabel3.UseAppStyling = false;
            // 
            // activityIndicator
            // 
            this.activityIndicator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.activityIndicator.CausesValidation = true;
            this.activityIndicator.Location = new System.Drawing.Point(12, 228);
            this.activityIndicator.Name = "activityIndicator";
            this.activityIndicator.Size = new System.Drawing.Size(128, 23);
            this.activityIndicator.TabIndex = 70;
            this.activityIndicator.TabStop = true;
            this.activityIndicator.Visible = false;
            // 
            // chkAllCustomers
            // 
            this.chkAllCustomers.AutoSize = true;
            this.chkAllCustomers.Location = new System.Drawing.Point(25, 195);
            this.chkAllCustomers.Name = "chkAllCustomers";
            this.chkAllCustomers.Size = new System.Drawing.Size(100, 18);
            this.chkAllCustomers.TabIndex = 65;
            this.chkAllCustomers.Text = "All Customers";
            ultraToolTipInfo3.ToolTipText = "If checked, then all customers will be included in the report.";
            ultraToolTipInfo3.ToolTipTitle = "All Customers";
            this.ultraToolTipManager.SetUltraToolTip(this.chkAllCustomers, ultraToolTipInfo3);
            this.chkAllCustomers.CheckedChanged += new System.EventHandler(this.chkAllCustomers_CheckedChanged);
            // 
            // cboCustomer
            // 
            this.cboCustomer.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCustomer.Location = new System.Drawing.Point(62, 167);
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.Size = new System.Drawing.Size(283, 22);
            this.cboCustomer.TabIndex = 64;
            ultraToolTipInfo4.ToolTipText = "Select a specific customer from the list of available customers.";
            ultraToolTipInfo4.ToolTipTitle = "Customer";
            this.ultraToolTipManager.SetUltraToolTip(this.cboCustomer, ultraToolTipInfo4);
            // 
            // chkByCustomer
            // 
            this.chkByCustomer.AutoSize = true;
            this.chkByCustomer.Location = new System.Drawing.Point(25, 143);
            this.chkByCustomer.Name = "chkByCustomer";
            this.chkByCustomer.Size = new System.Drawing.Size(115, 18);
            this.chkByCustomer.TabIndex = 63;
            this.chkByCustomer.Text = "Single Customer";
            ultraToolTipInfo5.ToolTipText = "If checked, then the report will only show orders for the selected customer.";
            ultraToolTipInfo5.ToolTipTitle = "Single Customer";
            this.ultraToolTipManager.SetUltraToolTip(this.chkByCustomer, ultraToolTipInfo5);
            this.chkByCustomer.CheckedChanged += new System.EventHandler(this.chkByCustomer_CheckedChanged);
            // 
            // dteToDate
            // 
            this.dteToDate.Location = new System.Drawing.Point(201, 72);
            this.dteToDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dteToDate.Name = "dteToDate";
            this.dteToDate.Size = new System.Drawing.Size(144, 22);
            this.dteToDate.TabIndex = 62;
            ultraToolTipInfo6.ToolTipTextFormatted = "The report will include orders processed on and before this date.<br/>If you sele" +
    "ct today\'s date, the report will include in-process orders.";
            ultraToolTipInfo6.ToolTipTitle = "To Date";
            this.ultraToolTipManager.SetUltraToolTip(this.dteToDate, ultraToolTipInfo6);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(112, 76);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(54, 15);
            this.ultraLabel1.TabIndex = 69;
            this.ultraLabel1.Text = "To Date:";
            // 
            // dteFromDate
            // 
            this.dteFromDate.Location = new System.Drawing.Point(201, 44);
            this.dteFromDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dteFromDate.Name = "dteFromDate";
            this.dteFromDate.Size = new System.Drawing.Size(144, 22);
            this.dteFromDate.TabIndex = 61;
            ultraToolTipInfo7.ToolTipText = "The report will include orders that were processed on or after this date.";
            ultraToolTipInfo7.ToolTipTitle = "From Date";
            this.ultraToolTipManager.SetUltraToolTip(this.dteFromDate, ultraToolTipInfo7);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(112, 48);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(69, 15);
            this.ultraLabel2.TabIndex = 68;
            this.ultraLabel2.Text = "From Date:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(254, 228);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 23);
            this.btnCancel.TabIndex = 67;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(158, 228);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 66;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ultraPictureBox1
            // 
            this.ultraPictureBox1.AutoSize = true;
            this.ultraPictureBox1.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox1.Image = ((object)(resources.GetObject("ultraPictureBox1.Image")));
            this.ultraPictureBox1.Location = new System.Drawing.Point(22, 19);
            this.ultraPictureBox1.Name = "ultraPictureBox1";
            this.ultraPictureBox1.Size = new System.Drawing.Size(64, 64);
            this.ultraPictureBox1.TabIndex = 60;
            this.ultraPictureBox1.UseAppStyling = false;
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // TimeTrackingOptions
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
            this.Name = "TimeTrackingOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Report Options";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.TimeDataOptions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chkAllCustomers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkByCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel lblYTD;
        private Infragistics.Win.Misc.UltraLabel lblLastMonth;
        private Infragistics.Win.Misc.UltraLabel lblToday;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator activityIndicator;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkAllCustomers;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomer;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkByCustomer;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteToDate;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteFromDate;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
    }
}