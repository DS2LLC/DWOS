namespace DWOS.Reports.ReportOptions
{
    partial class DateAndGroupingOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DateAndGroupingOptions));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "From Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Select how the data should be grouped.", Infragistics.Win.ToolTipImage.Default, "Group By", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the report will look at orders closed on and before this date.", Infragistics.Win.ToolTipImage.Default, "To Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set the date to today.", Infragistics.Win.ToolTipImage.Default, "Today", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set dates to last month.", Infragistics.Win.ToolTipImage.Default, "Last Month", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set dates to a year-to-date range.", Infragistics.Win.ToolTipImage.Default, "Year To Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.dteReportFromDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.cboGroupBy = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.dteReportToDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.lblToday = new Infragistics.Win.Misc.UltraLabel();
            this.lblLastMonth = new Infragistics.Win.Misc.UltraLabel();
            this.lblYTD = new Infragistics.Win.Misc.UltraLabel();
            this.activityIndicator = new Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator();
            this.lblName = new Infragistics.Win.Misc.UltraLabel();
            this.lblReportType = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dteReportFromDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboGroupBy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteReportToDate)).BeginInit();
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
            this.ultraLabel2.Location = new System.Drawing.Point(126, 76);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(69, 15);
            this.ultraLabel2.TabIndex = 46;
            this.ultraLabel2.Text = "From Date:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(254, 157);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(159, 157);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // dteReportFromDate
            // 
            this.dteReportFromDate.Location = new System.Drawing.Point(201, 72);
            this.dteReportFromDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dteReportFromDate.Name = "dteReportFromDate";
            this.dteReportFromDate.Size = new System.Drawing.Size(144, 22);
            this.dteReportFromDate.TabIndex = 1;
            ultraToolTipInfo6.ToolTipTextFormatted = "The date the report will look at orders opened on and after this date.";
            ultraToolTipInfo6.ToolTipTitle = "From Date";
            this.tipManager.SetUltraToolTip(this.dteReportFromDate, ultraToolTipInfo6);
            // 
            // cboGroupBy
            // 
            this.cboGroupBy.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboGroupBy.Location = new System.Drawing.Point(201, 44);
            this.cboGroupBy.Name = "cboGroupBy";
            this.cboGroupBy.Size = new System.Drawing.Size(144, 22);
            this.cboGroupBy.TabIndex = 0;
            ultraToolTipInfo5.ToolTipText = "Select how the data should be grouped.";
            ultraToolTipInfo5.ToolTipTitle = "Group By";
            this.tipManager.SetUltraToolTip(this.cboGroupBy, ultraToolTipInfo5);
            // 
            // dteReportToDate
            // 
            this.dteReportToDate.Location = new System.Drawing.Point(201, 100);
            this.dteReportToDate.Name = "dteReportToDate";
            this.dteReportToDate.Size = new System.Drawing.Size(144, 22);
            this.dteReportToDate.TabIndex = 2;
            ultraToolTipInfo4.ToolTipText = "The date the report will look at orders closed on and before this date.";
            ultraToolTipInfo4.ToolTipTitle = "To Date";
            this.tipManager.SetUltraToolTip(this.dteReportToDate, ultraToolTipInfo4);
            // 
            // lblToday
            // 
            appearance3.FontData.SizeInPoints = 7F;
            appearance3.FontData.UnderlineAsString = "True";
            appearance3.ForeColor = System.Drawing.Color.Blue;
            this.lblToday.Appearance = appearance3;
            this.lblToday.Location = new System.Drawing.Point(201, 128);
            this.lblToday.Name = "lblToday";
            this.lblToday.Size = new System.Drawing.Size(38, 14);
            this.lblToday.TabIndex = 64;
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
            this.lblLastMonth.Location = new System.Drawing.Point(245, 128);
            this.lblLastMonth.Name = "lblLastMonth";
            this.lblLastMonth.Size = new System.Drawing.Size(60, 14);
            this.lblLastMonth.TabIndex = 65;
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
            this.lblYTD.Location = new System.Drawing.Point(311, 128);
            this.lblYTD.Name = "lblYTD";
            this.lblYTD.Size = new System.Drawing.Size(27, 16);
            this.lblYTD.TabIndex = 66;
            this.lblYTD.Text = "YTD";
            ultraToolTipInfo1.ToolTipText = "Click to set dates to a year-to-date range.";
            ultraToolTipInfo1.ToolTipTitle = "Year To Date";
            this.tipManager.SetUltraToolTip(this.lblYTD, ultraToolTipInfo1);
            this.lblYTD.Click += new System.EventHandler(this.lblYTD_Click);
            // 
            // activityIndicator
            // 
            this.activityIndicator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.activityIndicator.CausesValidation = true;
            this.activityIndicator.Location = new System.Drawing.Point(12, 157);
            this.activityIndicator.Name = "activityIndicator";
            this.activityIndicator.Size = new System.Drawing.Size(128, 23);
            this.activityIndicator.TabIndex = 54;
            this.activityIndicator.TabStop = true;
            this.activityIndicator.Visible = false;
            // 
            // lblName
            // 
            appearance4.FontData.SizeInPoints = 14F;
            this.lblName.Appearance = appearance4;
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(112, 12);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(130, 26);
            this.lblName.TabIndex = 56;
            this.lblName.Text = "Order Status";
            this.lblName.UseAppStyling = false;
            // 
            // lblReportType
            // 
            this.lblReportType.AutoSize = true;
            this.lblReportType.Location = new System.Drawing.Point(126, 48);
            this.lblReportType.Name = "lblReportType";
            this.lblReportType.Size = new System.Drawing.Size(62, 15);
            this.lblReportType.TabIndex = 59;
            this.lblReportType.Text = "Group By:";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Location = new System.Drawing.Point(126, 104);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(69, 23);
            this.ultraLabel1.TabIndex = 61;
            this.ultraLabel1.Text = "To Date:";
            // 
            // DateAndGroupingOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 192);
            this.Controls.Add(this.lblYTD);
            this.Controls.Add(this.lblLastMonth);
            this.Controls.Add(this.lblToday);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.dteReportToDate);
            this.Controls.Add(this.lblReportType);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.activityIndicator);
            this.Controls.Add(this.cboGroupBy);
            this.Controls.Add(this.dteReportFromDate);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.ultraPictureBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DateAndGroupingOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Report Options";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.dteReportFromDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboGroupBy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteReportToDate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        protected Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboGroupBy;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteReportFromDate;
        private Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator activityIndicator;
        private Infragistics.Win.Misc.UltraLabel lblName;
        private Infragistics.Win.Misc.UltraLabel lblReportType;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteReportToDate;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel lblToday;
        private Infragistics.Win.Misc.UltraLabel lblLastMonth;
        private Infragistics.Win.Misc.UltraLabel lblYTD;
    }
}