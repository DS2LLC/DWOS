namespace DWOS.Reports.ReportOptions
{
    partial class DateOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DateOptions));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "From Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "To Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set the date to today.", Infragistics.Win.ToolTipImage.Default, "Today", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set dates to last month.", Infragistics.Win.ToolTipImage.Default, "Last Month", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set the date to year-to-date.", Infragistics.Win.ToolTipImage.Default, "Year To Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.dteFromDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.dteToDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.lblToday = new Infragistics.Win.Misc.UltraLabel();
            this.lblLastMonth = new Infragistics.Win.Misc.UltraLabel();
            this.lblYTD = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.activityIndicator = new Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator();
            this.lblName = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate)).BeginInit();
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
            this.ultraLabel2.Location = new System.Drawing.Point(112, 55);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(69, 15);
            this.ultraLabel2.TabIndex = 46;
            this.ultraLabel2.Text = "From Date:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(254, 143);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(158, 143);
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
            // dteFromDate
            // 
            this.dteFromDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dteFromDate.Location = new System.Drawing.Point(201, 51);
            this.dteFromDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dteFromDate.Name = "dteFromDate";
            this.dteFromDate.Size = new System.Drawing.Size(144, 22);
            this.dteFromDate.TabIndex = 1;
            ultraToolTipInfo5.ToolTipTextFormatted = "The date the report will look at orders opened on and after this date.";
            ultraToolTipInfo5.ToolTipTitle = "From Date";
            this.tipManager.SetUltraToolTip(this.dteFromDate, ultraToolTipInfo5);
            // 
            // dteToDate
            // 
            this.dteToDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dteToDate.Location = new System.Drawing.Point(201, 79);
            this.dteToDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dteToDate.Name = "dteToDate";
            this.dteToDate.Size = new System.Drawing.Size(144, 22);
            this.dteToDate.TabIndex = 2;
            ultraToolTipInfo4.ToolTipTextFormatted = "The date the report will look at orders opened on and before this date.";
            ultraToolTipInfo4.ToolTipTitle = "To Date";
            this.tipManager.SetUltraToolTip(this.dteToDate, ultraToolTipInfo4);
            // 
            // lblToday
            // 
            this.lblToday.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance3.FontData.SizeInPoints = 7F;
            appearance3.FontData.UnderlineAsString = "True";
            appearance3.ForeColor = System.Drawing.Color.Blue;
            this.lblToday.Appearance = appearance3;
            this.lblToday.AutoSize = true;
            this.lblToday.Location = new System.Drawing.Point(201, 107);
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
            this.lblLastMonth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.FontData.SizeInPoints = 7F;
            appearance2.FontData.UnderlineAsString = "True";
            appearance2.ForeColor = System.Drawing.Color.Blue;
            this.lblLastMonth.Appearance = appearance2;
            this.lblLastMonth.AutoSize = true;
            this.lblLastMonth.Location = new System.Drawing.Point(240, 107);
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
            this.lblYTD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.FontData.SizeInPoints = 7F;
            appearance1.FontData.UnderlineAsString = "True";
            appearance1.ForeColor = System.Drawing.Color.Blue;
            this.lblYTD.Appearance = appearance1;
            this.lblYTD.Location = new System.Drawing.Point(303, 107);
            this.lblYTD.Name = "lblYTD";
            this.lblYTD.Size = new System.Drawing.Size(29, 12);
            this.lblYTD.TabIndex = 59;
            this.lblYTD.Text = "YTD";
            ultraToolTipInfo1.ToolTipText = "Click to set the date to year-to-date.";
            ultraToolTipInfo1.ToolTipTextFormatted = "Click to set the date to year-to-date.";
            ultraToolTipInfo1.ToolTipTitle = "Year To Date";
            this.tipManager.SetUltraToolTip(this.lblYTD, ultraToolTipInfo1);
            this.lblYTD.Click += new System.EventHandler(this.lblYTD_Click);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(112, 83);
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
            this.activityIndicator.Location = new System.Drawing.Point(12, 143);
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
            this.lblName.Size = new System.Drawing.Size(177, 26);
            this.lblName.TabIndex = 56;
            this.lblName.Text = "Quote Log Report";
            this.lblName.UseAppStyling = false;
            // 
            // DateOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 178);
            this.Controls.Add(this.lblYTD);
            this.Controls.Add(this.lblLastMonth);
            this.Controls.Add(this.lblToday);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.activityIndicator);
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
            this.Name = "DateOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Report Options";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ClosedOrdersOptions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		protected Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteToDate;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteFromDate;
		private Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator activityIndicator;
		private Infragistics.Win.Misc.UltraLabel lblName;
		private Infragistics.Win.Misc.UltraLabel lblLastMonth;
        private Infragistics.Win.Misc.UltraLabel lblToday;
        private Infragistics.Win.Misc.UltraLabel lblYTD;
	}
}