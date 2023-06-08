using System;
using DWOS.Data.Datasets;
using DWOS.Shared;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;

namespace DWOS.UI.Admin
{
    public partial class SecurityGroupManager: DataEditorBase
    {
        #region Methods

        public SecurityGroupManager()
        {
            this.InitializeComponent();
        }

        private void LoadData()
        {
            try
            {
                this.dsSecurity.EnforceConstraints = false;

                this.taSecurityGroup.Fill(this.dsSecurity.SecurityGroup);
                this.taSecurityGroup_Role.Fill(this.dsSecurity.SecurityGroup_Role);
                this.taSecurityRole.Fill(this.dsSecurity.SecurityRole);
                this.taSecurityRoleCategory.Fill(this.dsSecurity.SecurityRoleCategory);
                this.taSecurityGroupTab.Fill(this.dsSecurity.SecurityGroupTab);

                this.dsSecurity.EnforceConstraints = true;

                this.pnlSecurityGroupInfo.LoadData(this.dsSecurity);
                base.AddDataPanel(this.pnlSecurityGroupInfo);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error loading user data.\r\n" + this.dsSecurity.GetDataErrors());
            }
        }

        private void LoadTOC()
        {
            tvwTOC.Nodes.Clear();

            UltraTreeNode rootNode = new SecurityGroupRootNode();
            tvwTOC.Nodes.Add(rootNode);
            rootNode.Expanded = true;

            foreach(SecurityDataSet.SecurityGroupRow cr in this.dsSecurity.SecurityGroup)
                rootNode.Nodes.Add(new SecurityGroupNode(cr));
        }

        protected override void ReloadTOC()
        {
            this.LoadTOC();
        }

        private void LoadCommands()
        {
            if(SecurityManager.Current.IsInRole("SecurityGroupManager.Edit"))
            {
                var add = base.Commands.AddCommand("Add", new AddCommand(toolbarManager.Tools["Add"], tvwTOC)) as AddCommand;
                add.AddUser = this.AddNode;

                base.Commands.AddCommand("Delete", new DeleteCommand(toolbarManager.Tools["Delete"], tvwTOC, this));
                base.Commands.AddCommand("Copy", new CopyCommand(toolbarManager.Tools["Copy"], tvwTOC));
                base.Commands.AddCommand("Paste", new PasteCommand(toolbarManager.Tools["Paste"], tvwTOC));
                base.Commands.AddCommand("SecurityGroupPermissionsReport", new SecurityGroupPermissionsReportCommand(toolbarManager.Tools["SecurityGroupPermissionsReport"]));
            }

            base.Commands.AddCommand("VideoTutorial", new VideoCommand(toolbarManager.Tools["VideoTutorial"]) { Url = VideoLinks.SecurityTutorial });
        }

        protected override bool SaveData()
        {
            try
            {
                base.EndAllEdits();

                this.taManager.UpdateAll(this.dsSecurity);
                return true;
            }
            catch(Exception exc)
            {
                _log.Warn("DataSet Errors: " + dsSecurity.GetDataErrors());

                ErrorMessageBox.ShowDialog("Error saving data.", exc);
                return false;
            }
        }

        private void AddSecurityGroup(SecurityGroupRootNode rn)
        {
            //create new data source
            SecurityDataSet.SecurityGroupRow cr = this.pnlSecurityGroupInfo.AddSecurityGroupRow();

            //create new ui nodes
            var cn = new SecurityGroupNode(cr);
            rn.Nodes.Add(cn);
            cn.Select();
        }

        protected override void LoadNode(UltraTreeNode node)
        {
            if(node is SecurityGroupNode)
            {
                this.pnlSecurityGroupInfo.MoveToRecord(((SecurityGroupNode)node).ID);
                DisplayPanel(this.pnlSecurityGroupInfo);
                this.pnlSecurityGroupInfo.Editable = ((SecurityGroupNode)node).DataRow.SystemDefined;
            }
            else
                DisplayPanel(null);
        }

        protected override void SaveSelectedNode()
        {
            if(tvwTOC.SelectedNodes.Count > 0)
                Properties.Settings.Default.LastSelectedUser = tvwTOC.SelectedNodes[0].Key;
        }

        #endregion

        #region Events

        private void UserManager_Load(object sender, EventArgs e)
        {
            _loadingData = true; //prevent TOC from loading

            try
            {
                this.LoadCommands();
                this.LoadData();
                this.LoadTOC();
                base.LoadValidators();

                _loadingData = false;

                //select first node customer node
                base.RestoreLastSelectedNode(Properties.Settings.Default.LastSelectedCustomer);

                splitContainer1.Panel2.Enabled = SecurityManager.Current.IsInRole("UserManager.Edit");
            }
            catch(Exception exc)
            {
                splitContainer1.Enabled = false;
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
                _log.Fatal(exc, "Error loading form.");
            }
        }

        private void AddNode(object sender, EventArgs e)
        {
            UltraTreeNode selectedNode = tvwTOC.Nodes.Count > 0 ? tvwTOC.Nodes[0] : null;

            if(IsValidControls())
            {
                _validators.Enabled = false;

                if(selectedNode is SecurityGroupRootNode)
                    this.AddSecurityGroup((SecurityGroupRootNode)selectedNode);

                _validators.Enabled = true;
            }
        }

        #endregion

        #region Nodes

        #region Nested type: SecurityGroupNode

        internal class SecurityGroupNode: DataNode<SecurityDataSet.SecurityGroupRow>
        {
            public const string KEY_PREFIX = "SG";

            #region Methods

            public SecurityGroupNode(SecurityDataSet.SecurityGroupRow cr)
                : base(cr, cr.SecurityGroupID.ToString(), KEY_PREFIX, cr.Name)
            {
                //update UI
                LeftImages.Add(Properties.Resources.Customer);
                this.UpdateNodeUI();
            }

            public override bool CanDelete
            {
                get { return DataRow != null && !DataRow.SystemDefined; }
            }

            public override void UpdateNodeUI()
            {
                if(base.DataRow != null)
                {
                    Text = base.DataRow.Name + " - [" + base.DataRow.GetSecurityGroup_RoleRows().Length.ToString() + " of " + ((SecurityDataSet)base.DataRow.Table.DataSet).SecurityRole.Count + "]";
                    base.Override.NodeAppearance.FontData.Bold = !base.DataRow.SystemDefined ? DefaultableBoolean.False : DefaultableBoolean.True;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: SecurityGroupRootNode

        internal class SecurityGroupRootNode: UltraTreeNode
        {
            #region Methods

            public SecurityGroupRootNode()
                : base("ROOT", "Security Groups")
            {
                LeftImages.Add(Properties.Resources.Security_16);
            }

            #endregion
        }

        #endregion

        #endregion

        #region Commands

        internal class AddCommand: TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddUser;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _node is SecurityGroupRootNode && this.AddUser != null; }
            }

            #endregion

            #region Methods

            public AddCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if(_node is SecurityGroupRootNode)
                {
                    if(this.AddUser != null)
                        this.AddUser(this, EventArgs.Empty);
                }
            }

            #endregion
        }

        #endregion
    }
}