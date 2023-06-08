namespace DWOS.Reports.ReportOptions
{
    partial class PartOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PartOptions));
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the report will look at orders opened on and after this date.", Infragistics.Win.ToolTipImage.Default, "From Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the report will look at orders opened on and before this date.", Infragistics.Win.ToolTipImage.Default, "To Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set the date to today.", Infragistics.Win.ToolTipImage.Default, "Today", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set dates to last month.", Infragistics.Win.ToolTipImage.Default, "Last Month", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to set the dates to a year-to-date range.", Infragistics.Win.ToolTipImage.Default, "Year To Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Select a customer from the list.", Infragistics.Win.ToolTipImage.Default, "Customer", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Part", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.lblTitle = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.dteFromDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.dteToDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.lblToday = new Infragistics.Win.Misc.UltraLabel();
            this.lblLastMonth = new Infragistics.Win.Misc.UltraLabel();
            this.lblYTD = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.cboCustomer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboPart = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.activityIndicator = new Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.taCustomer = new DWOS.Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter();
            this.taPart = new DWOS.Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter();
            this.dsCustomers = new DWOS.Data.Datasets.CustomersDataset();
            this.dsParts = new DWOS.Data.Datasets.PartsDataset();
            this.toolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsCustomers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsParts)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraPictureBox1
            // 
            this.ultraPictureBox1.AutoSize = true;
            this.ultraPictureBox1.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox1.Image = ((object)(resources.GetObject("ultraPictureBox1.Image")));
            this.ultraPictureBox1.Location = new System.Drawing.Point(22, 19);
            this.ultraPictureBox1.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.ultraPictureBox1.Name = "ultraPictureBox1";
            this.ultraPictureBox1.Size = new System.Drawing.Size(64, 64);
            this.ultraPictureBox1.TabIndex = 0;
            // 
            // lblTitle
            // 
            appearance1.FontData.SizeInPoints = 14F;
            this.lblTitle.Appearance = appearance1;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(112, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(117, 26);
            this.lblTitle.TabIndex = 57;
            this.lblTitle.Text = "Part Report";
            this.lblTitle.UseAppStyling = false;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(112, 48);
            this.ultraLabel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(69, 15);
            this.ultraLabel2.TabIndex = 58;
            this.ultraLabel2.Text = "From Date:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(112, 76);
            this.ultraLabel3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(54, 15);
            this.ultraLabel3.TabIndex = 59;
            this.ultraLabel3.Text = "To Date:";
            // 
            // dteFromDate
            // 
            this.dteFromDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dteFromDate.Location = new System.Drawing.Point(201, 44);
            this.dteFromDate.Name = "dteFromDate";
            this.dteFromDate.Size = new System.Drawing.Size(171, 22);
            this.dteFromDate.TabIndex = 1;
            ultraToolTipInfo7.ToolTipText = "The date the report will look at orders opened on and after this date.";
            ultraToolTipInfo7.ToolTipTitle = "From Date";
            this.toolTipManager.SetUltraToolTip(this.dteFromDate, ultraToolTipInfo7);
            // 
            // dteToDate
            // 
            this.dteToDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dteToDate.Location = new System.Drawing.Point(201, 72);
            this.dteToDate.Name = "dteToDate";
            this.dteToDate.Size = new System.Drawing.Size(171, 22);
            this.dteToDate.TabIndex = 2;
            ultraToolTipInfo6.ToolTipText = "The date the report will look at orders opened on and before this date.";
            ultraToolTipInfo6.ToolTipTitle = "To Date";
            this.toolTipManager.SetUltraToolTip(this.dteToDate, ultraToolTipInfo6);
            // 
            // lblToday
            // 
            appearance4.FontData.SizeInPoints = 7F;
            appearance4.FontData.UnderlineAsString = "True";
            appearance4.ForeColor = System.Drawing.Color.Blue;
            this.lblToday.Appearance = appearance4;
            this.lblToday.AutoSize = true;
            this.lblToday.Location = new System.Drawing.Point(201, 100);
            this.lblToday.Name = "lblToday";
            this.lblToday.Size = new System.Drawing.Size(33, 13);
            this.lblToday.TabIndex = 62;
            this.lblToday.Text = "Today";
            ultraToolTipInfo5.ToolTipText = "Click to set the date to today.";
            ultraToolTipInfo5.ToolTipTitle = "Today";
            this.toolTipManager.SetUltraToolTip(this.lblToday, ultraToolTipInfo5);
            this.lblToday.Click += new System.EventHandler(this.lblToday_Click);
            // 
            // lblLastMonth
            // 
            appearance3.FontData.SizeInPoints = 7F;
            appearance3.FontData.UnderlineAsString = "True";
            appearance3.ForeColor = System.Drawing.Color.Blue;
            this.lblLastMonth.Appearance = appearance3;
            this.lblLastMonth.AutoSize = true;
            this.lblLastMonth.Location = new System.Drawing.Point(240, 100);
            this.lblLastMonth.Name = "lblLastMonth";
            this.lblLastMonth.Size = new System.Drawing.Size(57, 13);
            this.lblLastMonth.TabIndex = 63;
            this.lblLastMonth.Text = "Last Month";
            ultraToolTipInfo4.ToolTipText = "Click to set dates to last month.";
            ultraToolTipInfo4.ToolTipTitle = "Last Month";
            this.toolTipManager.SetUltraToolTip(this.lblLastMonth, ultraToolTipInfo4);
            this.lblLastMonth.Click += new System.EventHandler(this.lblLastMonth_Click);
            // 
            // lblYTD
            // 
            appearance2.FontData.SizeInPoints = 7F;
            appearance2.FontData.UnderlineAsString = "True";
            appearance2.ForeColor = System.Drawing.Color.Blue;
            this.lblYTD.Appearance = appearance2;
            this.lblYTD.AutoSize = true;
            this.lblYTD.Location = new System.Drawing.Point(303, 100);
            this.lblYTD.Name = "lblYTD";
            this.lblYTD.Size = new System.Drawing.Size(23, 13);
            this.lblYTD.TabIndex = 64;
            this.lblYTD.Text = "YTD";
            ultraToolTipInfo3.ToolTipText = "Click to set the dates to a year-to-date range.";
            ultraToolTipInfo3.ToolTipTitle = "Year To Date";
            this.toolTipManager.SetUltraToolTip(this.lblYTD, ultraToolTipInfo3);
            this.lblYTD.Click += new System.EventHandler(this.lblYTD_Click);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(25, 143);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(64, 15);
            this.ultraLabel6.TabIndex = 65;
            this.ultraLabel6.Text = "Customer:";
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(25, 192);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(32, 15);
            this.ultraLabel7.TabIndex = 66;
            this.ultraLabel7.Text = "Part:";
            // 
            // cboCustomer
            // 
            this.cboCustomer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCustomer.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCustomer.Location = new System.Drawing.Point(25, 164);
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.Size = new System.Drawing.Size(347, 22);
            this.cboCustomer.TabIndex = 3;
            ultraToolTipInfo2.ToolTipText = "Select a customer from the list.";
            ultraToolTipInfo2.ToolTipTitle = "Customer";
            this.toolTipManager.SetUltraToolTip(this.cboCustomer, ultraToolTipInfo2);
            this.cboCustomer.ValueChanged += new System.EventHandler(this.cboCustomer_ValueChanged);
            // 
            // cboPart
            // 
            this.cboPart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPart.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboPart.Location = new System.Drawing.Point(25, 213);
            this.cboPart.Name = "cboPart";
            this.cboPart.Size = new System.Drawing.Size(347, 22);
            this.cboPart.TabIndex = 4;
            ultraToolTipInfo1.ToolTipTextFormatted = "Select a specific part from the list.<br/>The report will include all revisions o" +
    "f the selected part.<br/>";
            ultraToolTipInfo1.ToolTipTitle = "Part";
            this.toolTipManager.SetUltraToolTip(this.cboPart, ultraToolTipInfo1);
            // 
            // activityIndicator
            // 
            this.activityIndicator.CausesValidation = true;
            this.activityIndicator.Location = new System.Drawing.Point(25, 253);
            this.activityIndicator.Name = "activityIndicator";
            this.activityIndicator.Size = new System.Drawing.Size(130, 23);
            this.activityIndicator.TabIndex = 69;
            this.activityIndicator.TabStop = true;
            this.activityIndicator.Visible = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(191, 253);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(283, 253);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // taCustomer
            // 
            this.taCustomer.ClearBeforeFill = true;
            // 
            // taPart
            // 
            this.taPart.ClearBeforeFill = true;
            // 
            // dsCustomers
            // 
            this.dsCustomers.DataSetName = "CustomersDataset";
            this.dsCustomers.EnforceConstraints = false;
            this.dsCustomers.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // dsParts
            // 
            this.dsParts.DataSetName = "PartsDataset";
            this.dsParts.EnforceConstraints = false;
            this.dsParts.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // toolTipManager
            // 
            this.toolTipManager.ContainingControl = this;
            this.toolTipManager.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Office2013;
            // 
            // PartOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 288);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.activityIndicator);
            this.Controls.Add(this.cboPart);
            this.Controls.Add(this.cboCustomer);
            this.Controls.Add(this.ultraLabel7);
            this.Controls.Add(this.ultraLabel6);
            this.Controls.Add(this.lblYTD);
            this.Controls.Add(this.lblLastMonth);
            this.Controls.Add(this.lblToday);
            this.Controls.Add(this.dteToDate);
            this.Controls.Add(this.dteFromDate);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.ultraPictureBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PartOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Report Options";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsCustomers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsParts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
        private Infragistics.Win.Misc.UltraLabel lblTitle;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteFromDate;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteToDate;
        private Infragistics.Win.Misc.UltraLabel lblToday;
        private Infragistics.Win.Misc.UltraLabel lblLastMonth;
        private Infragistics.Win.Misc.UltraLabel lblYTD;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomer;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboPart;
        private Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator activityIndicator;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter taCustomer;
        private Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter taPart;
        private Data.Datasets.CustomersDataset dsCustomers;
        private Data.Datasets.PartsDataset dsParts;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager toolTipManager;
    }
}