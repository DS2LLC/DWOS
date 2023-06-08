using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Interop;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;
using DWOS.UI.Admin.SecurityGroupPanels;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinToolTip;
using Infragistics.Win.UltraWinTree;
using NLog;

namespace DWOS.UI.Admin.UserManagerPanels
{
    public partial class SecurityGroupInfo: DataPanel
    {
        private const string ALL_CATEGORY = "- All -";
        private SecurityDataSet.SecurityGroupRow _lastLoadedGroup;

        #region Properties

        public SecurityDataSet Dataset
        {
            get { return base._dataset as SecurityDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return this.Dataset.SecurityGroup.SecurityGroupIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public SecurityGroupInfo()
        {
            this.InitializeComponent();
        }

        public void LoadData(SecurityDataSet dataset)
        {
            this.Dataset = dataset;
            bsData.DataSource = this.Dataset;
            bsData.DataMember = this.Dataset.SecurityGroup.TableName;

            //bind column to control
            base.BindValue(this.txtName, this.Dataset.SecurityGroup.NameColumn.ColumnName);
            base.BindValue(this.chkSystemDefined, this.Dataset.SecurityGroup.SystemDefinedColumn.ColumnName);

            this.Dataset.SecurityRoleCategory.AddSecurityRoleCategoryRow(ALL_CATEGORY);
            base.BindList(this.cboSecurityCategory, this.Dataset.SecurityRoleCategory, this.Dataset.SecurityRoleCategory.SecurityRoleCategoryIDColumn.ColumnName, this.Dataset.SecurityRoleCategory.SecurityRoleCategoryIDColumn.ColumnName);

            this.LoadRoleNodes();

            ValueListItem allItem = this.cboSecurityCategory.FindItemByValue<string>(role => role == ALL_CATEGORY);

            if(allItem != null)
                this.cboSecurityCategory.SelectedItem = allItem;
            else
                this.cboSecurityCategory.SelectedIndex = this.cboSecurityCategory.Items.Count - 1;

            base._panelLoaded = true;
        }

        private void LoadRoleNodes()
        {
            this.tvwRoles.Nodes.Clear();

            //load available Roles
            foreach(SecurityDataSet.SecurityRoleRow role in this.Dataset.SecurityRole)
                this.tvwRoles.Nodes.Add(new SecurtyRoleNode(role));
        }

        private void FilterRoleNodes(string category)
        {
            foreach(SecurtyRoleNode sgNode in this.tvwRoles.Nodes.OfType<SecurtyRoleNode>())
                sgNode.Visible = (category == null || category == ALL_CATEGORY) || sgNode.DataRow.SecurityRoleCategoryID == category;
        }

        public override void AddValidators(DWOS.Utilities.Validation.ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtName, "Security group name required.") { DefaultValue = "New Group"}, errProvider));
            manager.Add(new ImageDisplayValidator(new SecurityGroupNameValidator(this.txtName, this)));
        }

        public SecurityDataSet.SecurityGroupRow AddSecurityGroupRow()
        {
            var rowVw        = bsData.AddNew() as DataRowView;
            var cr           = rowVw.Row as SecurityDataSet.SecurityGroupRow;
            cr.Name          = "New Group";
            cr.SystemDefined = false;

            return cr;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            this.LoadUsersRoles();
            UpdateTabCount();
            base.AfterMovedToNewRecord(id);
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            this.SaveGroupsRoles();
        }

        public override void EndEditing()
        {
            //ensure roles are saved for current user
            this.SaveGroupsRoles();
            base.EndEditing();
        }

        private void UpdateTabCount()
        {
            var group = base.CurrentRecord as SecurityDataSet.SecurityGroupRow;

            if (group == null)
            {
                return;
            }

            var tabCount = group.GetSecurityGroupTabRows().Length;

            string tabCountText;

            switch (tabCount)
            {
                case 0:
                    tabCountText = "No Tabs Selected";
                    break;
                case 1:
                    tabCountText = "1 Tab Selected";
                    break;
                default:
                    tabCountText = $"{tabCount} Tabs Selected";
                    break;
            }

            lblTabCount.Text = tabCountText;
        }

        private void LoadUsersRoles()
        {
            this._lastLoadedGroup = null;
            var group = base.CurrentRecord as SecurityDataSet.SecurityGroupRow;

            if(group != null)
            {
                //get all of this groups roles
                SecurityDataSet.SecurityGroup_RoleRow[] groupRoles = group.GetSecurityGroup_RoleRows();

                foreach(SecurtyRoleNode roleNode in this.tvwRoles.Nodes.OfType<SecurtyRoleNode>())
                {
                    //find if group has current role
                    roleNode.CheckedState = groupRoles.Any(r => r.SecurityRoleID == roleNode.DataRow.SecurityRoleID) ? CheckState.Checked : CheckState.Unchecked;
                }

                this._lastLoadedGroup = group;
            }
        }

        private void SaveGroupsRoles()
        {
            var group = base.CurrentRecord as SecurityDataSet.SecurityGroupRow;

            if(group != null && this._lastLoadedGroup == group)
            {
                //SYNCH all users security groups with changes to selected groups
                SecurityDataSet.SecurityGroup_RoleRow[] groupRoles = group.GetSecurityGroup_RoleRows();

                foreach(SecurtyRoleNode sgNode in this.tvwRoles.Nodes.OfType<SecurtyRoleNode>())
                {
                    SecurityDataSet.SecurityGroup_RoleRow matchingRole = groupRoles.FirstOrDefault(gr => gr.RowState != DataRowState.Deleted && gr.RowState != DataRowState.Detached && gr.SecurityRoleID == sgNode.DataRow.SecurityRoleID);

                    if(sgNode.CheckedState == CheckState.Checked)
                    {
                        //if no role then add it
                        if(matchingRole == null)
                        {
                            var deleted = this.Dataset.SecurityGroup_Role.FindDeletedRows();
                            var delMatchingRole = deleted.FirstOrDefault(dr => dr["SecurityRoleID", DataRowVersion.Original].ToString() == sgNode.DataRow.SecurityRoleID && Convert.ToInt32(dr["SecurityGroupID", DataRowVersion.Original]) == group.SecurityGroupID);

                            if(delMatchingRole != null)
                                delMatchingRole.SetAdded();
                            else
                                this.Dataset.SecurityGroup_Role.AddSecurityGroup_RoleRow(group, sgNode.DataRow);
                        }
                    }
                    else if(sgNode.CheckedState == CheckState.Unchecked)
                    {
                        //if role exists then remove, if not already deleted
                        if(matchingRole != null)
                            matchingRole.Delete();
                    }
                }

                base.OnUpdateCurrentNodeUI();
            }
        }

        #endregion

        #region Events

        private void tvwRoles_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            try
            {
                if(e.Element is NodeTextUIElement)
                {
                    var pt = new Point(e.Element.Rect.X, e.Element.Rect.Y);
                    var node = this.tvwRoles.GetNodeFromPoint(pt) as SecurtyRoleNode;
                    if(node != null && !node.DataRow.IsDescriptionNull())
                    {
                        node.Override.TipStyleNode = TipStyleNode.Hide;

                        var tipInfo = new UltraToolTipInfo(node.DataRow.Description, ToolTipImage.Default, node.Text, DefaultableBoolean.True);
                        tipManager.SetUltraToolTip(this.tvwRoles, tipInfo);
                        tipManager.ShowToolTip(this.tvwRoles);
                        //this.tipManager.GetUltraToolTip(this.tvwRoles);
                    }
                }
                else
                {
                    tipManager.HideToolTip();
                    tipManager.SetUltraToolTip(this.tvwRoles, null);
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting mouse tip on security role node.");
            }
        }

        private void cboSecurityCategory_ValueChanged(object sender, EventArgs e)
        {
            this.FilterRoleNodes(this.cboSecurityCategory.Text);
        }

        private void pnlSystemDefined_MouseHover(object sender, EventArgs e)
        {
            try
            {
                // Show tooltip for disabled control
                if (chkSystemDefined.Enabled)
                {
                    return;
                }

                tipManager.ShowToolTip(chkSystemDefined);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error showing sys defined tooltip.");
            }
        }

        private void pnlSystemDefined_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                // Hide tooltip for disabled control
                if (chkSystemDefined.Enabled || !tipManager.IsToolTipVisible(chkSystemDefined))
                {
                    return;
                }

                tipManager.HideToolTip();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error hiding sys defined tooltip.");
            }
        }

        private void btnTabs_Click(object sender, EventArgs e)
        {
            try
            {
                var group = CurrentRecord as SecurityDataSet.SecurityGroupRow;

                if (group == null)
                {
                    return;
                }

                // Show dialog
                var window = new SecurityGroupTabsDialog();
                var helper = new WindowInteropHelper(window) { Owner = DWOSApp.MainForm.Handle };

                window.Load(Dataset, group);
                window.ShowDialog();
                UpdateTabCount();

                GC.KeepAlive(helper);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error showing tab selection dialog.");
            }
        }

        #endregion

        #region RoleNode

        internal class SecurtyRoleNode: DataNode<SecurityDataSet.SecurityRoleRow>
        {
            public const string KEY_PREFIX = "SG";

            #region Methods

            public SecurtyRoleNode(SecurityDataSet.SecurityRoleRow cr)
                : base(cr, cr.SecurityRoleID, KEY_PREFIX, cr.SecurityRoleID)
            {
                base.Override.NodeStyle = NodeStyle.CheckBox;
                this.UpdateNodeUI();
            }

            public override bool CanDelete
            {
                get { return false; }
            }

            public override void UpdateNodeUI()
            {
                Text = base.DataRow.SecurityRoleID;
            }

            #endregion

            public override bool CanPasteData(string format)
            {
                return false;
            }
        }

        #endregion

        #region SecurityGroupNameValidator

        private sealed class SecurityGroupNameValidator : ControlValidatorBase
        {
            private readonly SecurityGroupInfo _view;

            public SecurityGroupNameValidator(UltraTextEditor control, SecurityGroupInfo view)
                : base(control)
            {
                _view = view;
            }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                var editor = Control as UltraTextEditor;

                if(editor != null && editor.Enabled)
                {
                    if(IsGroupNameDuplicate())
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "Only one group '" + editor.Text + "' can exist.");
                        return;
                    }
                }

                //passed
                e.Cancel = false;
                FireAfterValidation(true, string.Empty);
            }

            private bool IsGroupNameDuplicate()
            {
                var currentRecord = _view.CurrentRecord as SecurityDataSet.SecurityGroupRow;

                // Current record's Name can be null if left blank
                if (currentRecord == null || currentRecord.IsNull(_view.Dataset.SecurityGroup.NameColumn))
                {
                    return false;
                }

                return _view.Dataset.SecurityGroup.Any(
                    i => i.IsValidState() && i.SecurityGroupID != currentRecord.SecurityGroupID && i.Name == currentRecord.Name);
            }
        }

        #endregion

    }
}