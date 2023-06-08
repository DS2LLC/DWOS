namespace DWOS.Reports.ReportOptions
{
    partial class OrderProcessOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderProcessOptions));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Select the process to find all orders who have used it.", Infragistics.Win.ToolTipImage.Default, "Process", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Select the order status.", Infragistics.Win.ToolTipImage.Default, "Order Status", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Active Only", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Select the process alias to find all orders who have used it.", Infragistics.Win.ToolTipImage.Default, "Process Alias", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.cboProcess = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboOrderStatus = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.chkActiveOnly = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cboProcessAlias = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.activityIndicator = new Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcess)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOrderStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActiveOnly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessAlias)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraPictureBox1
            // 
            this.ultraPictureBox1.AutoSize = true;
            this.ultraPictureBox1.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox1.Image = ((object)(resources.GetObject("ultraPictureBox1.Image")));
            this.ultraPictureBox1.Location = new System.Drawing.Point(25, 23);
            this.ultraPictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ultraPictureBox1.Name = "ultraPictureBox1";
            this.ultraPictureBox1.Size = new System.Drawing.Size(64, 64);
            this.ultraPictureBox1.TabIndex = 0;
            this.ultraPictureBox1.UseAppStyling = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(258, 237);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(104, 28);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(149, 237);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(102, 28);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // cboProcess
            // 
            this.cboProcess.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            this.cboProcess.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
            this.cboProcess.Location = new System.Drawing.Point(25, 134);
            this.cboProcess.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cboProcess.Name = "cboProcess";
            this.cboProcess.Size = new System.Drawing.Size(339, 25);
            this.cboProcess.TabIndex = 2;
            ultraToolTipInfo4.ToolTipText = "Select the process to find all orders who have used it.";
            ultraToolTipInfo4.ToolTipTitle = "Process";
            this.tipManager.SetUltraToolTip(this.cboProcess, ultraToolTipInfo4);
            this.cboProcess.SelectionChanged += new System.EventHandler(this.cboProcess_SelectionChanged);
            // 
            // cboOrderStatus
            // 
            this.cboOrderStatus.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboOrderStatus.Location = new System.Drawing.Point(178, 62);
            this.cboOrderStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cboOrderStatus.Name = "cboOrderStatus";
            this.cboOrderStatus.Size = new System.Drawing.Size(186, 25);
            this.cboOrderStatus.TabIndex = 0;
            ultraToolTipInfo3.ToolTipText = "Select the order status.";
            ultraToolTipInfo3.ToolTipTitle = "Order Status";
            this.tipManager.SetUltraToolTip(this.cboOrderStatus, ultraToolTipInfo3);
            // 
            // chkActiveOnly
            // 
            this.chkActiveOnly.AutoSize = true;
            this.chkActiveOnly.Checked = true;
            this.chkActiveOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActiveOnly.Location = new System.Drawing.Point(264, 105);
            this.chkActiveOnly.Name = "chkActiveOnly";
            this.chkActiveOnly.Size = new System.Drawing.Size(98, 21);
            this.chkActiveOnly.TabIndex = 1;
            this.chkActiveOnly.Text = "Active Only";
            ultraToolTipInfo5.ToolTipTextFormatted = "Display on processes that are active in the process or process alias drop down.";
            ultraToolTipInfo5.ToolTipTitle = "Active Only";
            this.tipManager.SetUltraToolTip(this.chkActiveOnly, ultraToolTipInfo5);
            this.chkActiveOnly.CheckedChanged += new System.EventHandler(this.chkActiveOnly_CheckedChanged);
            // 
            // cboProcessAlias
            // 
            this.cboProcessAlias.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            this.cboProcessAlias.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
            this.cboProcessAlias.CheckedListSettings.CheckBoxStyle = Infragistics.Win.CheckStyle.CheckBox;
            this.cboProcessAlias.CheckedListSettings.EditorValueSource = Infragistics.Win.EditorWithComboValueSource.CheckedItems;
            valueListItem1.CheckState = System.Windows.Forms.CheckState.Checked;
            valueListItem1.DataValue = 0;
            valueListItem1.DisplayText = "All";
            this.cboProcessAlias.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1});
            this.cboProcessAlias.Location = new System.Drawing.Point(25, 194);
            this.cboProcessAlias.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cboProcessAlias.Name = "cboProcessAlias";
            this.cboProcessAlias.Size = new System.Drawing.Size(339, 25);
            this.cboProcessAlias.TabIndex = 3;
            this.cboProcessAlias.Text = "All";
            ultraToolTipInfo1.ToolTipText = "Select the process alias to find all orders who have used it.";
            ultraToolTipInfo1.ToolTipTitle = "Process Alias";
            this.tipManager.SetUltraToolTip(this.cboProcessAlias, ultraToolTipInfo1);
            // 
            // activityIndicator
            // 
            this.activityIndicator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.activityIndicator.CausesValidation = true;
            this.activityIndicator.Location = new System.Drawing.Point(14, 237);
            this.activityIndicator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.activityIndicator.Name = "activityIndicator";
            this.activityIndicator.Size = new System.Drawing.Size(114, 28);
            this.activityIndicator.TabIndex = 54;
            this.activityIndicator.TabStop = true;
            this.activityIndicator.Visible = false;
            // 
            // ultraLabel3
            // 
            appearance2.FontData.SizeInPoints = 14F;
            this.ultraLabel3.Appearance = appearance2;
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(110, 15);
            this.ultraLabel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(254, 26);
            this.ultraLabel3.TabIndex = 56;
            this.ultraLabel3.Text = "Orders By Process Report";
            this.ultraLabel3.UseAppStyling = false;
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(110, 65);
            this.ultraLabel5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(54, 18);
            this.ultraLabel5.TabIndex = 61;
            this.ultraLabel5.Text = "Status:";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(25, 108);
            this.ultraLabel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(62, 18);
            this.ultraLabel1.TabIndex = 68;
            this.ultraLabel1.Text = "Process:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(25, 168);
            this.ultraLabel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(98, 18);
            this.ultraLabel2.TabIndex = 69;
            this.ultraLabel2.Text = "Process Alias:";
            // 
            // OrderProcessOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 280);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.cboProcessAlias);
            this.Controls.Add(this.chkActiveOnly);
            this.Controls.Add(this.cboOrderStatus);
            this.Controls.Add(this.ultraLabel5);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.activityIndicator);
            this.Controls.Add(this.cboProcess);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.ultraPictureBox1);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OrderProcessOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Report Options";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.OrderProcessOptions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cboProcess)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOrderStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActiveOnly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessAlias)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
        protected Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboProcess;
		private Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator activityIndicator;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboOrderStatus;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkActiveOnly;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboProcessAlias;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
	}
}