namespace DWOS.UI.Admin.ReportPanels
{
    partial class ReportInfoPanel
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
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Delete the selected report field from this report.", Infragistics.Win.ToolTipImage.Default, "Delete Report Field", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add a field to this report.", Infragistics.Win.ToolTipImage.Default, "Add Report Field", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Moves the selected report field\'s display order up a value.", Infragistics.Win.ToolTipImage.Default, "Move Report Field ", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Moves the selected report field\'s display order down a value.", Infragistics.Win.ToolTipImage.Default, "Move Report Field", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraLabel16 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.cboCustomer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.cboReportType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.grdFields = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnDeleteField = new Infragistics.Win.Misc.UltraButton();
            this.btnAddField = new Infragistics.Win.Misc.UltraButton();
            this.btnMoveUp = new Infragistics.Win.Misc.UltraButton();
            this.btnMoveDown = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReportType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdFields)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.btnMoveDown);
            this.grpData.Controls.Add(this.btnMoveUp);
            this.grpData.Controls.Add(this.btnAddField);
            this.grpData.Controls.Add(this.btnDeleteField);
            this.grpData.Controls.Add(this.grdFields);
            this.grpData.Controls.Add(this.cboReportType);
            this.grpData.Controls.Add(this.ultraLabel16);
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.ultraLabel6);
            this.grpData.Controls.Add(this.cboCustomer);
            this.grpData.Dock = System.Windows.Forms.DockStyle.None;
            this.grpData.Size = new System.Drawing.Size(611, 301);
            this.grpData.Text = "Report Fields";
            this.grpData.Controls.SetChildIndex(this.cboCustomer, 0);
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel6, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel16, 0);
            this.grpData.Controls.SetChildIndex(this.cboReportType, 0);
            this.grpData.Controls.SetChildIndex(this.grdFields, 0);
            this.grpData.Controls.SetChildIndex(this.btnDeleteField, 0);
            this.grpData.Controls.SetChildIndex(this.btnAddField, 0);
            this.grpData.Controls.SetChildIndex(this.btnMoveUp, 0);
            this.grpData.Controls.SetChildIndex(this.btnMoveDown, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(806, -3999);
            // 
            // ultraLabel16
            // 
            this.ultraLabel16.AutoSize = true;
            this.ultraLabel16.Location = new System.Drawing.Point(6, 32);
            this.ultraLabel16.Name = "ultraLabel16";
            this.ultraLabel16.Size = new System.Drawing.Size(64, 15);
            this.ultraLabel16.TabIndex = 47;
            this.ultraLabel16.Text = "Customer:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(6, 60);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(79, 15);
            this.ultraLabel4.TabIndex = 46;
            this.ultraLabel4.Text = "Report Type:";
            // 
            // cboCustomer
            // 
            this.cboCustomer.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCustomer.Location = new System.Drawing.Point(99, 28);
            this.cboCustomer.MaxLength = 50;
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.NullText = "- Company Default -";
            this.cboCustomer.Size = new System.Drawing.Size(221, 22);
            this.cboCustomer.TabIndex = 47;
            this.cboCustomer.ValueChanged += new System.EventHandler(this.cboCustomer_ValueChanged);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(6, 88);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(84, 15);
            this.ultraLabel6.TabIndex = 41;
            this.ultraLabel6.Text = "Report Fields:";
            // 
            // cboReportType
            // 
            this.cboReportType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboReportType.Location = new System.Drawing.Point(99, 56);
            this.cboReportType.MaxLength = 50;
            this.cboReportType.Name = "cboReportType";
            this.cboReportType.ReadOnly = true;
            this.cboReportType.Size = new System.Drawing.Size(221, 22);
            this.cboReportType.TabIndex = 48;
            // 
            // grdFields
            // 
            this.grdFields.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            this.grdFields.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.grdFields.Location = new System.Drawing.Point(99, 88);
            this.grdFields.Name = "grdFields";
            this.grdFields.Size = new System.Drawing.Size(501, 207);
            this.grdFields.TabIndex = 49;
            this.grdFields.Text = "Report Fields";
            this.grdFields.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdFields_InitializeLayout);
            this.grdFields.AfterRowsDeleted += new System.EventHandler(this.grdFields_AfterRowsDeleted);
            this.grdFields.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.grdFields_BeforeRowsDeleted);
            // 
            // btnDeleteField
            // 
            appearance4.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            this.btnDeleteField.Appearance = appearance4;
            this.btnDeleteField.AutoSize = true;
            this.btnDeleteField.Location = new System.Drawing.Point(67, 141);
            this.btnDeleteField.Name = "btnDeleteField";
            this.btnDeleteField.Size = new System.Drawing.Size(26, 26);
            this.btnDeleteField.TabIndex = 51;
            ultraToolTipInfo4.ToolTipText = "Delete the selected report field from this report.";
            ultraToolTipInfo4.ToolTipTitle = "Delete Report Field";
            this.tipManager.SetUltraToolTip(this.btnDeleteField, ultraToolTipInfo4);
            this.btnDeleteField.Click += new System.EventHandler(this.btnDeleteField_Click);
            // 
            // btnAddField
            // 
            appearance3.Image = global::DWOS.UI.Properties.Resources.Add_32;
            this.btnAddField.Appearance = appearance3;
            this.btnAddField.AutoSize = true;
            this.btnAddField.Enabled = false;
            this.btnAddField.Location = new System.Drawing.Point(67, 109);
            this.btnAddField.Name = "btnAddField";
            this.btnAddField.Size = new System.Drawing.Size(26, 26);
            this.btnAddField.TabIndex = 50;
            ultraToolTipInfo3.ToolTipText = "Add a field to this report.";
            ultraToolTipInfo3.ToolTipTitle = "Add Report Field";
            this.tipManager.SetUltraToolTip(this.btnAddField, ultraToolTipInfo3);
            this.btnAddField.Click += new System.EventHandler(this.btnAddField_Click);
            // 
            // btnMoveUp
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.Arrow_Up;
            this.btnMoveUp.Appearance = appearance2;
            this.btnMoveUp.AutoSize = true;
            this.btnMoveUp.Location = new System.Drawing.Point(67, 173);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(26, 26);
            this.btnMoveUp.TabIndex = 52;
            ultraToolTipInfo2.ToolTipText = "Moves the selected report field\'s display order up a value.";
            ultraToolTipInfo2.ToolTipTitle = "Move Report Field ";
            this.tipManager.SetUltraToolTip(this.btnMoveUp, ultraToolTipInfo2);
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveFieldUp_Click);
            // 
            // btnMoveDown
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Arrow_Down;
            this.btnMoveDown.Appearance = appearance1;
            this.btnMoveDown.AutoSize = true;
            this.btnMoveDown.Location = new System.Drawing.Point(67, 205);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(26, 26);
            this.btnMoveDown.TabIndex = 53;
            ultraToolTipInfo1.ToolTipText = "Moves the selected report field\'s display order down a value.";
            ultraToolTipInfo1.ToolTipTitle = "Move Report Field";
            this.tipManager.SetUltraToolTip(this.btnMoveDown, ultraToolTipInfo1);
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveFieldDown_Click);
            // 
            // ReportInfoPanel
            // 
            this.Name = "ReportInfoPanel";
            this.Size = new System.Drawing.Size(617, 307);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReportType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdFields)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel16;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomer;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboReportType;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdFields;
        private Infragistics.Win.Misc.UltraButton btnDeleteField;
        private Infragistics.Win.Misc.UltraButton btnAddField;
        private Infragistics.Win.Misc.UltraButton btnMoveDown;
        private Infragistics.Win.Misc.UltraButton btnMoveUp;
	}
}
