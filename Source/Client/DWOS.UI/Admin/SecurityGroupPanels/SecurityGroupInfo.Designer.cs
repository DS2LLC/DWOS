namespace DWOS.UI.Admin.UserManagerPanels
{
	partial class SecurityGroupInfo
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
			_lastLoadedGroup = null;

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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Permissions", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Is System Defined", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Select the category to filter the list of permissions by.", Infragistics.Win.ToolTipImage.Default, "Security Category", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Edit Default Tabs", Infragistics.Win.DefaultableBoolean.Default);
            this.txtName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.tvwRoles = new Infragistics.Win.UltraWinTree.UltraTree();
            this.chkSystemDefined = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cboSecurityCategory = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.pnlSystemDefined = new System.Windows.Forms.Panel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.btnTabs = new Infragistics.Win.Misc.UltraButton();
            this.lblTabCount = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwRoles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSystemDefined)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSecurityCategory)).BeginInit();
            this.pnlSystemDefined.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.lblTabCount);
            this.grpData.Controls.Add(this.btnTabs);
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.cboSecurityCategory);
            this.grpData.Controls.Add(this.tvwRoles);
            this.grpData.Controls.Add(this.ultraLabel3);
            this.grpData.Controls.Add(this.txtName);
            this.grpData.Controls.Add(this.ultraLabel1);
            this.grpData.Controls.Add(this.pnlSystemDefined);
            appearance1.Image = global::DWOS.UI.Properties.Resources.Security_16;
            this.grpData.HeaderAppearance = appearance1;
            this.grpData.Size = new System.Drawing.Size(401, 304);
            this.grpData.Text = "Security Group";
            this.grpData.Controls.SetChildIndex(this.pnlSystemDefined, 0);
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.txtName, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.grpData.Controls.SetChildIndex(this.tvwRoles, 0);
            this.grpData.Controls.SetChildIndex(this.cboSecurityCategory, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            this.grpData.Controls.SetChildIndex(this.btnTabs, 0);
            this.grpData.Controls.SetChildIndex(this.lblTabCount, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(316, -512);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(104, 33);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(163, 22);
            this.txtName.TabIndex = 0;
            ultraToolTipInfo4.ToolTipTextFormatted = "The display name of the security group.";
            ultraToolTipInfo4.ToolTipTitle = "Name";
            this.tipManager.SetUltraToolTip(this.txtName, ultraToolTipInfo4);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 37);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel1.TabIndex = 2;
            this.ultraLabel1.Text = "Name:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(6, 117);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(77, 15);
            this.ultraLabel3.TabIndex = 5;
            this.ultraLabel3.Text = "Permissions:";
            // 
            // tvwRoles
            // 
            this.tvwRoles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwRoles.Location = new System.Drawing.Point(104, 117);
            this.tvwRoles.Name = "tvwRoles";
            this.tvwRoles.Size = new System.Drawing.Size(289, 181);
            this.tvwRoles.TabIndex = 4;
            ultraToolTipInfo3.ToolTipTextFormatted = "Check all permissions available to users in this security group.&edsp;";
            ultraToolTipInfo3.ToolTipTitle = "Permissions";
            this.tipManager.SetUltraToolTip(this.tvwRoles, ultraToolTipInfo3);
            this.tvwRoles.MouseEnterElement += new Infragistics.Win.UIElementEventHandler(this.tvwRoles_MouseEnterElement);
            // 
            // chkSystemDefined
            // 
            this.chkSystemDefined.Enabled = false;
            this.chkSystemDefined.Location = new System.Drawing.Point(0, 0);
            this.chkSystemDefined.Name = "chkSystemDefined";
            this.chkSystemDefined.Size = new System.Drawing.Size(116, 20);
            this.chkSystemDefined.TabIndex = 1;
            this.chkSystemDefined.Text = "System Defined";
            ultraToolTipInfo5.ToolTipTextFormatted = "If checked, the security group has been defined by the system.";
            ultraToolTipInfo5.ToolTipTitle = "Is System Defined";
            this.tipManager.SetUltraToolTip(this.chkSystemDefined, ultraToolTipInfo5);
            // 
            // cboSecurityCategory
            // 
            this.cboSecurityCategory.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            this.cboSecurityCategory.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboSecurityCategory.Location = new System.Drawing.Point(104, 60);
            this.cboSecurityCategory.Name = "cboSecurityCategory";
            this.cboSecurityCategory.Size = new System.Drawing.Size(163, 22);
            this.cboSecurityCategory.TabIndex = 2;
            ultraToolTipInfo2.ToolTipText = "Select the category to filter the list of permissions by.";
            ultraToolTipInfo2.ToolTipTitle = "Security Category";
            this.tipManager.SetUltraToolTip(this.cboSecurityCategory, ultraToolTipInfo2);
            this.cboSecurityCategory.ValueChanged += new System.EventHandler(this.cboSecurityCategory_ValueChanged);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(6, 64);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(61, 15);
            this.ultraLabel2.TabIndex = 8;
            this.ultraLabel2.Text = "Category:";
            // 
            // pnlSystemDefined
            // 
            this.pnlSystemDefined.Controls.Add(this.chkSystemDefined);
            this.pnlSystemDefined.Location = new System.Drawing.Point(273, 34);
            this.pnlSystemDefined.Name = "pnlSystemDefined";
            this.pnlSystemDefined.Size = new System.Drawing.Size(116, 20);
            this.pnlSystemDefined.TabIndex = 9;
            this.pnlSystemDefined.MouseLeave += new System.EventHandler(this.pnlSystemDefined_MouseLeave);
            this.pnlSystemDefined.MouseHover += new System.EventHandler(this.pnlSystemDefined_MouseHover);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(6, 93);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(90, 15);
            this.ultraLabel4.TabIndex = 10;
            this.ultraLabel4.Text = "Login Settings:";
            // 
            // btnTabs
            // 
            this.btnTabs.Location = new System.Drawing.Point(102, 88);
            this.btnTabs.Name = "btnTabs";
            this.btnTabs.Size = new System.Drawing.Size(75, 23);
            this.btnTabs.TabIndex = 3;
            this.btnTabs.Text = "Edit Tabs...";
            ultraToolTipInfo1.ToolTipTextFormatted = "Edit default WIP screen tabs for this security group.<br/> If a user is in a grou" +
    "p with default tabs, all WIP screen tabs will be replaced by those default tabs " +
    "on login.";
            ultraToolTipInfo1.ToolTipTitle = "Edit Default Tabs";
            this.tipManager.SetUltraToolTip(this.btnTabs, ultraToolTipInfo1);
            this.btnTabs.Click += new System.EventHandler(this.btnTabs_Click);
            // 
            // lblTabCount
            // 
            this.lblTabCount.AutoSize = true;
            this.lblTabCount.Location = new System.Drawing.Point(183, 93);
            this.lblTabCount.Name = "lblTabCount";
            this.lblTabCount.Size = new System.Drawing.Size(95, 15);
            this.lblTabCount.TabIndex = 11;
            this.lblTabCount.Text = "0 Tabs Selected";
            // 
            // SecurityGroupInfo
            // 
            this.Name = "SecurityGroupInfo";
            this.Size = new System.Drawing.Size(407, 310);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwRoles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSystemDefined)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSecurityCategory)).EndInit();
            this.pnlSystemDefined.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtName;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
		private Infragistics.Win.UltraWinTree.UltraTree tvwRoles;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkSystemDefined;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboSecurityCategory;
        private System.Windows.Forms.Panel pnlSystemDefined;
        private Infragistics.Win.Misc.UltraButton btnTabs;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel lblTabCount;
    }
}
