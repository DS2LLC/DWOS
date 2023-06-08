namespace DWOS.UI.Admin
{
	partial class UserManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserManager));
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab1 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbon1");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup1 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup1");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("DisplayInActive", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Infragistics.Win.UltraWinToolbars.ButtonTool("EmployeeResourceCenter");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup2 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup2");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Copy");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Paste");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup3 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup3");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SecurityAuditReport");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Copy");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Paste");
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("TOCContext");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Copy");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Paste");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("DisplayInActive", "");
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SecurityAuditReport");
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Infragistics.Win.UltraWinToolbars.ButtonTool("EmployeeResourceCenter");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            this.pnlUserInformation = new DWOS.UI.Admin.Users.UserInformation();
            this.taUsers = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.UsersTableAdapter();
            this.taManager = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.TableAdapterManager();
            this.taMedia = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.MediaTableAdapter();
            this.taSecurityGroup_Role = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityGroup_RoleTableAdapter();
            this.taSecurityGroup = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityGroupTableAdapter();
            this.taUser_SecurityGroup = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.User_SecurityGroupTableAdapter();
            this.taUserSalary = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.UserSalaryTableAdapter();
            this.taSecurityRole = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityRoleTableAdapter();
            this.taUser_SecurityRoles = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.User_SecurityRolesTableAdapter();
            this.dsSecurity = new DWOS.Data.Datasets.SecurityDataSet();
            this.taSecurityRoleCategory = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityRoleCategoryTableAdapter();
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
            this.toolbarManager.Office2007UICompatibility = false;
            appearance1.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance1.ImageBackground")));
            appearance1.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.toolbarManager.Ribbon.ApplicationMenu2010.ContentArea.Settings.Appearance = appearance1;
            this.toolbarManager.Ribbon.Caption = "User Manager";
            this.toolbarManager.Ribbon.FileMenuStyle = Infragistics.Win.UltraWinToolbars.FileMenuStyle.ApplicationMenu2010;
            ribbonTab1.Caption = "Home";
            ribbonGroup1.Caption = "Users";
            ribbonGroup1.PreferredToolSize = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool19.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool13,
            stateButtonTool1,
            buttonTool19});
            ribbonGroup2.Caption = "Edit";
            ribbonGroup2.PreferredToolSize = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool14,
            buttonTool15,
            buttonTool16});
            ribbonGroup3.Caption = "Reports";
            buttonTool17.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool17});
            ribbonTab1.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup1,
            ribbonGroup2,
            ribbonGroup3});
            this.toolbarManager.Ribbon.NonInheritedRibbonTabs.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonTab[] {
            ribbonTab1});
            this.toolbarManager.Ribbon.Visible = true;
            this.toolbarManager.ToolbarSettings.ForceSerialization = true;
            appearance2.Image = global::DWOS.UI.Properties.Resources.UserManager_32;
            buttonTool4.SharedPropsInternal.AppearancesLarge.Appearance = appearance2;
            appearance3.Image = ((object)(resources.GetObject("appearance3.Image")));
            buttonTool4.SharedPropsInternal.AppearancesSmall.Appearance = appearance3;
            buttonTool4.SharedPropsInternal.Caption = "Add";
            buttonTool4.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.DefaultForToolType;
            buttonTool4.SharedPropsInternal.Enabled = false;
            buttonTool4.SharedPropsInternal.ToolTipTextFormatted = "Add a new user.";
            buttonTool4.SharedPropsInternal.ToolTipTitle = "Add User";
            appearance4.Image = global::DWOS.UI.Properties.Resources.Delete_32;
            buttonTool5.SharedPropsInternal.AppearancesLarge.Appearance = appearance4;
            appearance5.Image = ((object)(resources.GetObject("appearance5.Image")));
            buttonTool5.SharedPropsInternal.AppearancesSmall.Appearance = appearance5;
            buttonTool5.SharedPropsInternal.Caption = "Delete";
            buttonTool5.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.DefaultForToolType;
            buttonTool5.SharedPropsInternal.Enabled = false;
            buttonTool5.SharedPropsInternal.ToolTipTextFormatted = "Delete the selected user.";
            buttonTool5.SharedPropsInternal.ToolTipTitle = "Delete";
            appearance6.Image = global::DWOS.UI.Properties.Resources.Copy_32;
            buttonTool6.SharedPropsInternal.AppearancesLarge.Appearance = appearance6;
            appearance7.Image = ((object)(resources.GetObject("appearance7.Image")));
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance7;
            buttonTool6.SharedPropsInternal.Caption = "Copy";
            buttonTool6.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.DefaultForToolType;
            buttonTool6.SharedPropsInternal.Enabled = false;
            buttonTool6.SharedPropsInternal.ToolTipTextFormatted = "Copy the selected user.";
            buttonTool6.SharedPropsInternal.ToolTipTitle = "Copy";
            appearance8.Image = global::DWOS.UI.Properties.Resources.editpaste;
            buttonTool7.SharedPropsInternal.AppearancesLarge.Appearance = appearance8;
            appearance9.Image = ((object)(resources.GetObject("appearance9.Image")));
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance9;
            buttonTool7.SharedPropsInternal.Caption = "Paste";
            buttonTool7.SharedPropsInternal.Enabled = false;
            buttonTool7.SharedPropsInternal.ToolTipTextFormatted = "Paste copied user.";
            buttonTool7.SharedPropsInternal.ToolTipTitle = "Paste";
            popupMenuTool1.SharedPropsInternal.Caption = "TOCContext";
            buttonTool11.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool9,
            buttonTool10,
            buttonTool11,
            buttonTool8});
            appearance10.Image = global::DWOS.UI.Properties.Resources.Checked_32;
            stateButtonTool2.SharedPropsInternal.AppearancesLarge.Appearance = appearance10;
            stateButtonTool2.SharedPropsInternal.Caption = "Display Inactive";
            stateButtonTool2.SharedPropsInternal.Enabled = false;
            stateButtonTool2.SharedPropsInternal.ToolTipTextFormatted = "If checked, will display inactive users.";
            stateButtonTool2.SharedPropsInternal.ToolTipTitle = "Display Inactive  Users";
            appearance11.Image = global::DWOS.UI.Properties.Resources.UserSecurity_32;
            buttonTool18.SharedPropsInternal.AppearancesLarge.Appearance = appearance11;
            buttonTool18.SharedPropsInternal.Caption = "User Security Permissions";
            buttonTool18.SharedPropsInternal.Enabled = false;
            buttonTool18.SharedPropsInternal.ToolTipTextFormatted = "<span style=\"font-family:Verdana;\">Displays a report summarizing all of the permi" +
    "ssions for each user.</span>";
            buttonTool18.SharedPropsInternal.ToolTipTitle = "User Security Permissions Report";
            appearance12.Image = global::DWOS.UI.Properties.Resources.Paper32;
            buttonTool20.SharedPropsInternal.AppearancesLarge.Appearance = appearance12;
            appearance13.Image = global::DWOS.UI.Properties.Resources.Paper32;
            buttonTool20.SharedPropsInternal.AppearancesSmall.Appearance = appearance13;
            buttonTool20.SharedPropsInternal.Caption = "Employee Resource Center";
            buttonTool20.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.DefaultForToolType;
            buttonTool20.SharedPropsInternal.Enabled = false;
            buttonTool20.SharedPropsInternal.ToolTipText = "Manage resources for the selected employee.";
            buttonTool20.SharedPropsInternal.ToolTipTitle = "Employee Resource Center";
            this.toolbarManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool4,
            buttonTool5,
            buttonTool6,
            buttonTool7,
            popupMenuTool1,
            stateButtonTool2,
            buttonTool18,
            buttonTool20});
            // 
            // tvwTOC
            // 
            this.toolbarManager.SetContextMenuUltra(this.tvwTOC, "TOCContext");
            this.tvwTOC.Size = new System.Drawing.Size(262, 894);
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlUserInformation);
            this.splitContainer1.Size = new System.Drawing.Size(974, 894);
            this.splitContainer1.SplitterDistance = 262;
            // 
            // helpLink1
            // 
            this.helpLink1.HelpPage = "user_manager_dialog.htm";
            // 
            // pnlUserInformation
            // 
            this.pnlUserInformation.Dataset = null;
            this.pnlUserInformation.Editable = true;
            this.pnlUserInformation.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlUserInformation.IsActivePanel = false;
            this.pnlUserInformation.Location = new System.Drawing.Point(15, 12);
            this.pnlUserInformation.Name = "pnlUserInformation";
            this.pnlUserInformation.Padding = new System.Windows.Forms.Padding(5);
            this.pnlUserInformation.Size = new System.Drawing.Size(407, 310);
            this.pnlUserInformation.TabIndex = 0;
            // 
            // taUsers
            // 
            this.taUsers.ClearBeforeFill = true;
            // 
            // taManager
            // 
            this.taManager.BackupDataSetBeforeUpdate = false;
            this.taManager.MediaTableAdapter = this.taMedia;
            this.taManager.SecurityGroup_RoleTableAdapter = this.taSecurityGroup_Role;
            this.taManager.SecurityGroupTableAdapter = this.taSecurityGroup;
            this.taManager.UpdateOrder = DWOS.Data.Datasets.SecurityDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            this.taManager.User_CustomersTableAdapter = null;
            this.taManager.User_SecurityGroupTableAdapter = this.taUser_SecurityGroup;
            this.taManager.User_SecurityRolesTableAdapter = null;
            this.taManager.UserSalaryTableAdapter = this.taUserSalary;
            this.taManager.UsersTableAdapter = this.taUsers;
            // 
            // taMedia
            // 
            this.taMedia.ClearBeforeFill = false;
            // 
            // taSecurityGroup_Role
            // 
            this.taSecurityGroup_Role.ClearBeforeFill = true;
            // 
            // taSecurityGroup
            // 
            this.taSecurityGroup.ClearBeforeFill = true;
            // 
            // taUser_SecurityGroup
            // 
            this.taUser_SecurityGroup.ClearBeforeFill = true;
            // 
            // taUserSalary
            // 
            this.taUserSalary.ClearBeforeFill = true;
            // 
            // taSecurityRole
            // 
            this.taSecurityRole.ClearBeforeFill = true;
            // 
            // taUser_SecurityRoles
            // 
            this.taUser_SecurityRoles.ClearBeforeFill = true;
            // 
            // dsSecurity
            // 
            this.dsSecurity.DataSetName = "SecurityDataSet";
            this.dsSecurity.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taSecurityRoleCategory
            // 
            this.taSecurityRoleCategory.ClearBeforeFill = true;
            // 
            // UserManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(990, 1092);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UserManager";
            this.Text = "User Manager";
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

		private DWOS.UI.Admin.Users.UserInformation pnlUserInformation;
		private DWOS.Data.Datasets.SecurityDataSetTableAdapters.UsersTableAdapter taUsers;
		//private DWOS.Data.Datasets.SecurityDataSetTableAdapters.UserRolesTableAdapter taUserRoles;
		private DWOS.Data.Datasets.SecurityDataSetTableAdapters.TableAdapterManager taManager;
		private DWOS.Data.Datasets.SecurityDataSet dsSecurity;
		private DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityGroupTableAdapter taSecurityGroup;
		private DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityRoleTableAdapter taSecurityRole;
		private DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityGroup_RoleTableAdapter taSecurityGroup_Role;
		private DWOS.Data.Datasets.SecurityDataSetTableAdapters.User_SecurityGroupTableAdapter taUser_SecurityGroup;
		private DWOS.Data.Datasets.SecurityDataSetTableAdapters.User_SecurityRolesTableAdapter taUser_SecurityRoles;
		private DWOS.Data.Datasets.SecurityDataSetTableAdapters.SecurityRoleCategoryTableAdapter taSecurityRoleCategory;
		private Data.Datasets.SecurityDataSetTableAdapters.MediaTableAdapter taMedia;
        private Data.Datasets.SecurityDataSetTableAdapters.UserSalaryTableAdapter taUserSalary;
        //private DWOS.Data.Datasets.SecurityDataSetTableAdapters.d_RolesTableAdapter taRoles;
    }
}
