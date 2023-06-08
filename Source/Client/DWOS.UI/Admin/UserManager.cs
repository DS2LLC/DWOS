using System;
using System.Data;
using System.Drawing;
using DWOS.Data.Datasets;
using DWOS.Shared;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using NLog;
using DWOS.UI.Admin.Users;
using System.Windows.Interop;

namespace DWOS.UI.Admin
{
    public partial class UserManager: DataEditorBase
    {
        #region Methods

        public UserManager()
        {
            this.InitializeComponent();
        }

        private void LoadData()
        {
            try
            {
                this.dsSecurity.EnforceConstraints = false;

                this.taUsers.Fill(this.dsSecurity.Users);
                this.taUser_SecurityGroup.Fill(this.dsSecurity.User_SecurityGroup);
                this.taSecurityRoleCategory.Fill(this.dsSecurity.SecurityRoleCategory);
                this.taSecurityGroup.Fill(this.dsSecurity.SecurityGroup);
                this.taSecurityGroup_Role.Fill(this.dsSecurity.SecurityGroup_Role);
                this.taSecurityRole.Fill(this.dsSecurity.SecurityRole);
                this.taUserSalary.Fill(this.dsSecurity.UserSalary);

                //Dont enforce constraints as we will be dynamically loading some data as the user node is clicked on
                //this.dsSecurity.EnforceConstraints = true;

                this.pnlUserInformation.LoadData(this.dsSecurity);
                base.AddDataPanel(this.pnlUserInformation);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error loading user data.\r\n" + this.dsSecurity.GetDataErrors());
            }
        }

        private void LoadTOC()
        {
            using(new UsingTreeLoad(tvwTOC))
            {
                tvwTOC.Nodes.Clear();

                UltraTreeNode rootNode = new UsersRootNode(this.dsSecurity);
                tvwTOC.Nodes.Add(rootNode);
                rootNode.Expanded = true;

                var showAll = ((StateButtonTool)toolbarManager.Tools["DisplayInActive"]).Checked;
                foreach(SecurityDataSet.UsersRow cr in this.dsSecurity.Users)
                    rootNode.Nodes.Add(new UserNode(cr, this){Visible = showAll || cr.Active});
            }
        }

        protected override void ReloadTOC()
        {
            this.LoadTOC();
        }

        private void LoadCommands()
        {
            if(SecurityManager.Current.IsInRole("UserManager.Edit"))
            {
                var add = base.Commands.AddCommand("Add", new AddCommand(toolbarManager.Tools["Add"], tvwTOC)) as AddCommand;
                add.AddUser = this.AddNode;

                base.Commands.AddCommand("DisplayInActive", new DisplayInactiveCommand(toolbarManager.Tools["DisplayInActive"], tvwTOC));
                base.Commands.AddCommand("Delete", new DeleteCommand(toolbarManager.Tools["Delete"], tvwTOC, this));
                base.Commands.AddCommand("Copy", new CopyCommand(toolbarManager.Tools["Copy"], tvwTOC));
                base.Commands.AddCommand("Paste", new PasteCommand(toolbarManager.Tools["Paste"], tvwTOC));
                base.Commands.AddCommand("SecurityAuditReport", new SecurityAuditReportCommand(toolbarManager.Tools["SecurityAuditReport"]));
            }

            base.Commands.AddCommand("EmployeeResourceCenter", new EmployeeResourceCommand(toolbarManager.Tools["EmployeeResourceCenter"], tvwTOC, dsSecurity));
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

        private void AddUser(UsersRootNode rn)
        {
            //create new data source
            SecurityDataSet.UsersRow cr = this.pnlUserInformation.AddUserRow();

            //create new ui nodes
            var cn = new UserNode(cr, this);
            rn.Nodes.Add(cn);
            cn.Select();
        }

        protected override void LoadNode(UltraTreeNode node)
        {
            if(node is UserNode)
            {
                //determine usage count if not already done
                if(((UserNode)node).UsageCount < 0)
                {
                    bool? isUsed = false;
                    this.taUsers.Get_IsUserUsed(((UserNode)node).DataRow.UserID, out isUsed);
                    ((UserNode)node).UsageCount = isUsed.GetValueOrDefault() ? 1 : 0;
                }
                
                ((UserNode) node).LoadNodeData();
                this.pnlUserInformation.MoveToRecord(((UserNode)node).ID);
                DisplayPanel(this.pnlUserInformation);
                this.pnlUserInformation.Editable = ((UserNode)node).UsageCount < 1;
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

        protected override void btnOK_Click(object sender, EventArgs e)
        {
            base.btnOK_Click(sender, e);

            //only reload the user roles if succesfully saved, else this will close the dialog due to the OnUserChange that gets called.
            if(this.DialogResult == System.Windows.Forms.DialogResult.OK)
                SecurityManager.Current.ReLoadUserSecurityRoles();
        }

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

                if(selectedNode is UsersRootNode)
                    this.AddUser((UsersRootNode)selectedNode);

                _validators.Enabled = true;
            }
        }

        #endregion

        #region Nodes

        #region Nested type: UserNode

        internal class UserNode: DataNode<SecurityDataSet.UsersRow>
        {
            #region Fields
            
            public const string KEY_PREFIX = "US";
            public int UsageCount = -1;
            private UserManager _userManager;
            private bool _isLoaded = false;

            #endregion

            #region Methods

            public UserNode(SecurityDataSet.UsersRow cr, UserManager userManager)
                : base(cr, cr.UserID.ToString(), KEY_PREFIX, cr.Name)
            {
                _userManager = userManager;

                //update UI
                LeftImages.Add(Properties.Resources.Customer);
                this.UpdateNodeUI();
            }

            public override bool CanDelete
            {
                get { return this.UsageCount < 1; }
            }

            public override void UpdateNodeUI()
            {
                Text = base.DataRow.Name + " - " + base.DataRow.UserLogOn;
                base.Override.NodeAppearance.FontData.Italic = base.DataRow.Active ? DefaultableBoolean.False : DefaultableBoolean.True;
                base.Override.NodeAppearance.ForeColor = base.DataRow.Active ? Color.Black : Color.Gray;
            }

            public override string ClipboardDataFormat
            {
                get { return GetType().FullName; }
            }

            public void LoadNodeData()
            {
                if(_isLoaded)
                    return;

                try
                {
                    if (_userManager != null)
                    {
                        if (!this.DataRow.IsMediaIDNull())
                            _userManager.taMedia.FillByIdWOMedia(_userManager.dsSecurity.Media, this.DataRow.MediaID);

                        if (!this.DataRow.IsSignatureMediaIDNull())
                            _userManager.taMedia.FillByIdWOMedia(_userManager.dsSecurity.Media, this.DataRow.SignatureMediaID);
                    }

                    _isLoaded = true;
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error loading node info.");
                }
            }

            public override void Dispose()
            {
                _userManager = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region Nested type: UsersRootNode

        internal class UsersRootNode: UltraTreeNode, ICopyPasteNode
        {
            #region Properties

            private SecurityDataSet _dataset;

            #endregion

            #region Methods

            public UsersRootNode(SecurityDataSet dataset)
                : base("ROOT", "Users")
            {
                this._dataset = dataset;
                LeftImages.Add(Properties.Resources.Customer);
            }

            public override void Dispose()
            {
                this._dataset = null;
                base.Dispose();
            }

            #endregion

            #region ICopyPasteNode Members

            public UltraTreeNode PasteData(string format, DataRowProxy proxy)
            {
                if (proxy == null)
                {
                    return null;
                }

                if (_dataset == null)
                {
                    LogManager.GetCurrentClassLogger().Warn("Node was disposed - skipping paste");
                    return null;
                }

                var childProxies = proxy.ChildProxies;
                proxy.ChildProxies = new System.Collections.Generic.List<DataRowProxy>();

                var dr = DataNode<DataRow>.AddPastedDataRows(proxy, this._dataset.Users) as SecurityDataSet.UsersRow;
                var un = new UserNode(dr, null);

                if(childProxies != null)
                {
                    foreach(var dataRowProxy in childProxies)
                    {
                        //Manually copy the user/security group roles
                        if(dataRowProxy.ParentRelation =="FK_User_SecurityGroup_Users")
                        {
                            var userSG = this._dataset.User_SecurityGroup.NewUser_SecurityGroupRow();
                            userSG.SecurityGroupID = Convert.ToInt32(dataRowProxy.ItemArray[0]);
                            userSG.UsersRow = dr;
                            this._dataset.User_SecurityGroup.AddUser_SecurityGroupRow(userSG);
                        }
                    }
                }

                base.Nodes.Add(un);

                return un;
            }

            public bool CanPasteData(string format)
            {
                return format == typeof(UserNode).FullName;
            }

            public string ClipboardDataFormat
            {
                get { return null; }
            }

            #endregion
        }

        #endregion

        #endregion

        #region Commands

        #region Nested type: AddCommand

        internal class AddCommand: TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddUser;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return this.AddUser != null; }
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
                if(this.AddUser != null)
                    this.AddUser(this, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #region Nested type: DisplayInactiveCommand

        internal class DisplayInactiveCommand: TreeNodeCommandBase
        {
            #region Fields

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.TreeView != null && base.TreeView.Nodes.Count > 0; }
            }

            #endregion

            #region Methods

            public DisplayInactiveCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                try
                {
                    var showAll = ((StateButtonTool)Button.Button).Checked;

                    var rootNode = base.TreeView.Nodes[0];

                    if(rootNode != null)
                    {
                        foreach(var node in rootNode.Nodes)
                        {
                            var userNode = node as UserNode;

                            if(userNode != null)
                                userNode.Visible = showAll || userNode.DataRow.Active;
                        }
                    }
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error updating visibility of nodes.");
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: EmployeeResourceCommand

        internal class EmployeeResourceCommand : TreeNodeCommandBase
        {
            #region Properties

            public override bool Enabled
            {
                get
                {
                    return base.Enabled &&
                        _node is UserNode;
                }
            }

            public SecurityDataSet DataSet
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public EmployeeResourceCommand(ToolBase tool, UltraTree toc, SecurityDataSet dataSet) :
                base(tool, "UserManager.ResourceCenter")
            {
                if (dataSet == null)
                {
                    throw new ArgumentNullException(nameof(dataSet));
                }

                DataSet = dataSet;
                base.TreeView = toc;
                HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
            }

            public override void OnClick()
            {
                try
                {
                    var userNode = _node as UserNode;

                    if (userNode == null)
                    {
                        return;
                    }

                    var window = new EmployeeResourceDialog();
                    var helper = new WindowInteropHelper(window) { Owner = DWOSApp.MainForm.Handle };

                    window.LoadData(DataSet, userNode.DataRow);
                    window.ShowDialog();
                    GC.KeepAlive(helper);
                }
                catch (Exception exc)
                {
                    _log.Error(exc, "Error showing salary window.");
                }
            }

            #endregion
        }

        #endregion

        #endregion
    }
}