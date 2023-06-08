namespace DWOS.UI.Admin
{
	partial class SecurityGroupManager
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab1 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbon1");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup1 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup1");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup2 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup2");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Copy");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Paste");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup3 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup3");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SecurityGroupPermissionsReport");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("VideoTutorial");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("Main");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Copy");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Paste");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Copy");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Paste");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("TOCContext");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Copy");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Paste");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SecurityAuditReport");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SecurityGroupPermissionsReport");
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Infragistics.Win.UltraWinToolbars.ButtonTool("VideoTutorial");
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SecurityGroupManager));
            this.taManager = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.TableAdapterManager();
            this.taSecurityGroup_Role = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityGroup_RoleTableAdapter();
            this.taSecurityGroup = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityGroupTableAdapter();
            this.taSecurityRole = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityRoleTableAdapter();
            this.dsSecurity = new DWOS.Data.Datasets.SecurityDataSet();
            this.pnlSecurityGroupInfo = new DWOS.UI.Admin.UserManagerPanels.SecurityGroupInfo();
            this.taSecurityRoleCategory = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityRoleCategoryTableAdapter();
            this.taSecurityGroupTab = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityGroupTabTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsSecurity)).BeginInit();
            this.SuspendLayout();
            // 
            // toolbarManager
            // 
            this.toolbarManager.MenuSettings.ForceSerialization = true;
            appearance1.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.toolbarManager.Ribbon.ApplicationMenu2010.ContentArea.Settings.Appearance = appearance1;
            this.toolbarManager.Ribbon.Caption = "<span style=\"font-family:Verdana;\"><span style=\"font-weight:normal;\">Security </s" +
    "pan>Group Manager</span>";
            this.toolbarManager.Ribbon.FileMenuStyle = Infragistics.Win.UltraWinToolbars.FileMenuStyle.ApplicationMenu2010;
            ribbonTab1.Caption = "Home";
            ribbonGroup1.Caption = "Security Group";
            ribbonGroup1.PreferredToolSize = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool13});
            ribbonGroup2.Caption = "Edit";
            ribbonGroup2.PreferredToolSize = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool14,
            buttonTool15,
            buttonTool16});
            ribbonGroup3.Caption = "Reports";
            buttonTool22.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool22});
            ribbonTab1.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup1,
            ribbonGroup2,
            ribbonGroup3});
            this.toolbarManager.Ribbon.NonInheritedRibbonTabs.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonTab[] {
            ribbonTab1});
            this.toolbarManager.Ribbon.TabItemToolbar.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool17});
            this.toolbarManager.Ribbon.Visible = true;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            buttonTool1.InstanceProps.IsFirstInGroup = true;
            buttonTool3.InstanceProps.IsFirstInGroup = true;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2,
            buttonTool3,
            buttonTool12});
            ultraToolbar1.Text = "Main";
            this.toolbarManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            this.toolbarManager.ToolbarSettings.ForceSerialization = true;
            appearance2.Image = global::DWOS.UI.Properties.Resources.Add_32;
            buttonTool4.SharedPropsInternal.AppearancesLarge.Appearance = appearance2;
            buttonTool4.SharedPropsInternal.Caption = "Add";
            buttonTool4.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.DefaultForToolType;
            buttonTool4.SharedPropsInternal.Enabled = false;
            buttonTool4.SharedPropsInternal.ToolTipTextFormatted = "Add a new security group.";
            buttonTool4.SharedPropsInternal.ToolTipTitle = "Add Security Group";
            appearance3.Image = global::DWOS.UI.Properties.Resources.Delete_32;
            buttonTool5.SharedPropsInternal.AppearancesLarge.Appearance = appearance3;
            buttonTool5.SharedPropsInternal.Caption = "Delete";
            buttonTool5.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.DefaultForToolType;
            buttonTool5.SharedPropsInternal.Enabled = false;
            buttonTool5.SharedPropsInternal.ToolTipTextFormatted = "Delete the selected security group.";
            buttonTool5.SharedPropsInternal.ToolTipTitle = "Delete";
            appearance4.Image = global::DWOS.UI.Properties.Resources.Copy_32;
            buttonTool6.SharedPropsInternal.AppearancesLarge.Appearance = appearance4;
            buttonTool6.SharedPropsInternal.Caption = "Copy";
            buttonTool6.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.DefaultForToolType;
            buttonTool6.SharedPropsInternal.Enabled = false;
            buttonTool6.SharedPropsInternal.ToolTipTextFormatted = "Copy the security group.";
            buttonTool6.SharedPropsInternal.ToolTipTitle = "Copy";
            appearance5.Image = global::DWOS.UI.Properties.Resources.editpaste;
            buttonTool7.SharedPropsInternal.AppearancesLarge.Appearance = appearance5;
            buttonTool7.SharedPropsInternal.Caption = "Paste";
            buttonTool7.SharedPropsInternal.Enabled = false;
            buttonTool7.SharedPropsInternal.ToolTipTextFormatted = "Paste the copied security group.";
            buttonTool7.SharedPropsInternal.ToolTipTitle = "Paste";
            popupMenuTool1.SharedPropsInternal.Caption = "TOCContext";
            buttonTool11.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool9,
            buttonTool10,
            buttonTool11,
            buttonTool8});
            appearance6.Image = global::DWOS.UI.Properties.Resources.UserManager_32;
            buttonTool18.SharedPropsInternal.AppearancesLarge.Appearance = appearance6;
            buttonTool18.SharedPropsInternal.Caption = "User Security Audit";
            appearance7.Image = global::DWOS.UI.Properties.Resources.GroupSecurity_32;
            buttonTool20.SharedPropsInternal.AppearancesLarge.Appearance = appearance7;
            buttonTool20.SharedPropsInternal.Caption = "Security Group Permissions";
            buttonTool20.SharedPropsInternal.Enabled = false;
            buttonTool20.SharedPropsInternal.ToolTipTextFormatted = "<span style=\"font-family:Verdana;\">Displays a report summarizing all of the permi" +
    "ssions for each security group.</span>";
            buttonTool20.SharedPropsInternal.ToolTipTitle = "Security Group Permissions Report";
            appearance8.Image = global::DWOS.UI.Properties.Resources.Video_16;
            buttonTool19.SharedPropsInternal.AppearancesSmall.Appearance = appearance8;
            buttonTool19.SharedPropsInternal.Caption = "VideoTutorial";
            buttonTool19.SharedPropsInternal.ToolTipText = "Watch a video tutorial explaining security groups and users.";
            buttonTool19.SharedPropsInternal.ToolTipTitle = "Video Tutorial";
            this.toolbarManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool4,
            buttonTool5,
            buttonTool6,
            buttonTool7,
            popupMenuTool1,
            buttonTool18,
            buttonTool20,
            buttonTool19});
            // 
            // tvwTOC
            // 
            this.toolbarManager.SetContextMenuUltra(this.tvwTOC, "TOCContext");
            this.tvwTOC.Size = new System.Drawing.Size(258, 757);
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlSecurityGroupInfo);
            this.splitContainer1.Size = new System.Drawing.Size(826, 757);
            this.splitContainer1.SplitterDistance = 258;
            // 
            // helpLink1
            // 
            this.helpLink1.HelpPage = "security_group_manager_dialog.htm";
            // 
            // taManager
            // 
            this.taManager.BackupDataSetBeforeUpdate = false;
            this.taManager.MediaTableAdapter = null;
            this.taManager.SecurityGroup_RoleTableAdapter = this.taSecurityGroup_Role;
            this.taManager.SecurityGroupTableAdapter = this.taSecurityGroup;
            this.taManager.SecurityGroupTabTableAdapter = this.taSecurityGroupTab;
            this.taManager.UpdateOrder = DWOS.Data.Datasets.SecurityDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            this.taManager.User_CustomersTableAdapter = null;
            this.taManager.User_SecurityGroupTableAdapter = null;
            this.taManager.User_SecurityRolesTableAdapter = null;
            this.taManager.UserSalaryTableAdapter = null;
            this.taManager.UsersTableAdapter = null;
            // 
            // taSecurityGroup_Role
            // 
            this.taSecurityGroup_Role.ClearBeforeFill = true;
            // 
            // taSecurityGroup
            // 
            this.taSecurityGroup.ClearBeforeFill = true;
            // 
            // taSecurityRole
            // 
            this.taSecurityRole.ClearBeforeFill = true;
            // 
            // dsSecurity
            // 
            this.dsSecurity.DataSetName = "SecurityDataSet";
            this.dsSecurity.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // pnlSecurityGroupInfo
            // 
            this.pnlSecurityGroupInfo.Dataset = null;
            this.pnlSecurityGroupInfo.Editable = true;
            this.pnlSecurityGroupInfo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlSecurityGroupInfo.IsActivePanel = false;
            this.pnlSecurityGroupInfo.Location = new System.Drawing.Point(14, 23);
            this.pnlSecurityGroupInfo.Name = "pnlSecurityGroupInfo";
            this.pnlSecurityGroupInfo.Padding = new System.Windows.Forms.Padding(3);
            this.pnlSecurityGroupInfo.Size = new System.Drawing.Size(407, 310);
            this.pnlSecurityGroupInfo.TabIndex = 0;
            // 
            // taSecurityRoleCategory
            // 
            this.taSecurityRoleCategory.ClearBeforeFill = true;
            // 
            // taSecurityGroupTab
            // 
            this.taSecurityGroupTab.ClearBeforeFill = true;
            // 
            // SecurityGroupManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(842, 955);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SecurityGroupManager";
            this.Text = "Security Group Manager";
            this.Load += new System.EventHandler(this.UserManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dsSecurity)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		//private DWOS.Data.Datasets.SecurityDataSetTableAdapters.UserRolesTableAdapter taUserRoles;
		private DWOS.Data.Datasets.SecurityDataSetTableAdapters.TableAdapterManager taManager;
		private DWOS.Data.Datasets.SecurityDataSet dsSecurity;
		private DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityGroupTableAdapter taSecurityGroup;
		private DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityRoleTableAdapter taSecurityRole;
		private DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityGroup_RoleTableAdapter taSecurityGroup_Role;
		private UserManagerPanels.SecurityGroupInfo pnlSecurityGroupInfo;
		private DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityRoleCategoryTableAdapter taSecurityRoleCategory;
        private Data.Datasets.SecurityDataSetTableAdapters.SecurityGroupTabTableAdapter taSecurityGroupTab;
        //private DWOS.Data.Datasets.SecurityDataSetTableAdapters.d_RolesTableAdapter taRoles;
    }
}
