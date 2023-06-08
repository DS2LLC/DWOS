namespace DWOS.Reports.ReportOptions
{
    partial class DateAndProductClassOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DateAndProductClassOptions));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo10 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "From Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "To Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set the date to today.", Infragistics.Win.ToolTipImage.Default, "Today", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set dates to last month.", Infragistics.Win.ToolTipImage.Default, "Last Month", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set the date to year-to-date.", Infragistics.Win.ToolTipImage.Default, "Year To Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
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
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.cboProductClass = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.chkProductClass = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkAllProductClass = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProductClass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkProductClass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllProductClass)).BeginInit();
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
            this.ultraLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(154, 48);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(69, 15);
            this.ultraLabel2.TabIndex = 46;
            this.ultraLabel2.Text = "From Date:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(296, 209);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(200, 209);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // dteFromDate
            // 
            this.dteFromDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dteFromDate.Location = new System.Drawing.Point(243, 44);
            this.dteFromDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dteFromDate.Name = "dteFromDate";
            this.dteFromDate.Size = new System.Drawing.Size(144, 22);
            this.dteFromDate.TabIndex = 1;
            ultraToolTipInfo10.ToolTipTextFormatted = "The date the report will look at orders opened on and after this date.";
            ultraToolTipInfo10.ToolTipTitle = "From Date";
            this.tipManager.SetUltraToolTip(this.dteFromDate, ultraToolTipInfo10);
            // 
            // dteToDate
            // 
            this.dteToDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dteToDate.Location = new System.Drawing.Point(243, 72);
            this.dteToDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dteToDate.Name = "dteToDate";
            this.dteToDate.Size = new System.Drawing.Size(144, 22);
            this.dteToDate.TabIndex = 2;
            ultraToolTipInfo9.ToolTipTextFormatted = "The date the report will look at orders opened on and before this date.";
            ultraToolTipInfo9.ToolTipTitle = "To Date";
            this.tipManager.SetUltraToolTip(this.dteToDate, ultraToolTipInfo9);
            // 
            // lblToday
            // 
            this.lblToday.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance7.FontData.SizeInPoints = 7F;
            appearance7.FontData.UnderlineAsString = "True";
            appearance7.ForeColor = System.Drawing.Color.Blue;
            this.lblToday.Appearance = appearance7;
            this.lblToday.AutoSize = true;
            this.lblToday.Location = new System.Drawing.Point(243, 100);
            this.lblToday.Name = "lblToday";
            this.lblToday.Size = new System.Drawing.Size(33, 13);
            this.lblToday.TabIndex = 3;
            this.lblToday.Text = "Today";
            ultraToolTipInfo8.ToolTipText = "Click to set the date to today.";
            ultraToolTipInfo8.ToolTipTitle = "Today";
            this.tipManager.SetUltraToolTip(this.lblToday, ultraToolTipInfo8);
            this.lblToday.Click += new System.EventHandler(this.lblToday_Click);
            // 
            // lblLastMonth
            // 
            this.lblLastMonth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance6.FontData.SizeInPoints = 7F;
            appearance6.FontData.UnderlineAsString = "True";
            appearance6.ForeColor = System.Drawing.Color.Blue;
            this.lblLastMonth.Appearance = appearance6;
            this.lblLastMonth.AutoSize = true;
            this.lblLastMonth.Location = new System.Drawing.Point(282, 100);
            this.lblLastMonth.Name = "lblLastMonth";
            this.lblLastMonth.Size = new System.Drawing.Size(57, 13);
            this.lblLastMonth.TabIndex = 4;
            this.lblLastMonth.Text = "Last Month";
            ultraToolTipInfo7.ToolTipText = "Click to set dates to last month.";
            ultraToolTipInfo7.ToolTipTitle = "Last Month";
            this.tipManager.SetUltraToolTip(this.lblLastMonth, ultraToolTipInfo7);
            this.lblLastMonth.Click += new System.EventHandler(this.lblLastMonth_Click);
            // 
            // lblYTD
            // 
            this.lblYTD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance5.FontData.SizeInPoints = 7F;
            appearance5.FontData.UnderlineAsString = "True";
            appearance5.ForeColor = System.Drawing.Color.Blue;
            this.lblYTD.Appearance = appearance5;
            this.lblYTD.Location = new System.Drawing.Point(345, 100);
            this.lblYTD.Name = "lblYTD";
            this.lblYTD.Size = new System.Drawing.Size(29, 12);
            this.lblYTD.TabIndex = 5;
            this.lblYTD.Text = "YTD";
            ultraToolTipInfo6.ToolTipText = "Click to set the date to year-to-date.";
            ultraToolTipInfo6.ToolTipTextFormatted = "Click to set the date to year-to-date.";
            ultraToolTipInfo6.ToolTipTitle = "Year To Date";
            this.tipManager.SetUltraToolTip(this.lblYTD, ultraToolTipInfo6);
            this.lblYTD.Click += new System.EventHandler(this.lblYTD_Click);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(154, 76);
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
            this.activityIndicator.Location = new System.Drawing.Point(12, 209);
            this.activityIndicator.Name = "activityIndicator";
            this.activityIndicator.Size = new System.Drawing.Size(170, 23);
            this.activityIndicator.TabIndex = 54;
            this.activityIndicator.TabStop = true;
            this.activityIndicator.Visible = false;
            // 
            // lblName
            // 
            appearance8.FontData.SizeInPoints = 14F;
            this.lblName.Appearance = appearance8;
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(101, 12);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(286, 26);
            this.lblName.TabIndex = 56;
            this.lblName.Text = "Delivery Performance Report";
            this.lblName.UseAppStyling = false;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(39, 153);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(86, 15);
            this.ultraLabel3.TabIndex = 60;
            this.ultraLabel3.Text = "Product Class:";
            // 
            // cboProductClass
            // 
            this.cboProductClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboProductClass.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboProductClass.Location = new System.Drawing.Point(131, 149);
            this.cboProductClass.Name = "cboProductClass";
            this.cboProductClass.Size = new System.Drawing.Size(256, 22);
            this.cboProductClass.TabIndex = 6;
            // 
            // chkProductClass
            // 
            this.chkProductClass.AutoSize = true;
            this.chkProductClass.Checked = true;
            this.chkProductClass.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkProductClass.Location = new System.Drawing.Point(12, 128);
            this.chkProductClass.Name = "chkProductClass";
            this.chkProductClass.Size = new System.Drawing.Size(137, 18);
            this.chkProductClass.TabIndex = 61;
            this.chkProductClass.Text = "Single Product Class";
            this.chkProductClass.CheckedChanged += new System.EventHandler(this.chkProductClass_CheckedChanged);
            // 
            // chkAllProductClass
            // 
            this.chkAllProductClass.AutoSize = true;
            this.chkAllProductClass.Location = new System.Drawing.Point(12, 180);
            this.chkAllProductClass.Name = "chkAllProductClass";
            this.chkAllProductClass.Size = new System.Drawing.Size(129, 18);
            this.chkAllProductClass.TabIndex = 62;
            this.chkAllProductClass.Text = "All Product Classes";
            this.chkAllProductClass.CheckedChanged += new System.EventHandler(this.chkAllProductClass_CheckedChanged);
            // 
            // DateAndProductClassOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 244);
            this.Controls.Add(this.chkAllProductClass);
            this.Controls.Add(this.chkProductClass);
            this.Controls.Add(this.cboProductClass);
            this.Controls.Add(this.ultraLabel3);
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
            this.Name = "DateAndProductClassOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Report Options";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ClosedOrdersOptions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProductClass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkProductClass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllProductClass)).EndInit();
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
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboProductClass;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkProductClass;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkAllProductClass;
    }
}