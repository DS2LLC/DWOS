using System;
using System.Linq;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.SecurityDataSetTableAdapters;
using DWOS.UI.Utilities;

namespace DWOS.UI.Documents
{
    internal class DocumentManagerSecurity
    {
        #region Fields

        private static Lazy<DocumentManagerSecurity> _documentSecurity =
            new Lazy<DocumentManagerSecurity>(() => new DocumentManagerSecurity(SecurityManager.Current));

        private SecurityDataSet.SecurityGroup_RoleDataTable _securityGroup_Roles;

        public event EventHandler FolderChanged;

        #endregion

        #region Properties

        private DocumentsDataSet.DocumentFolderRow Folder { get; set; }

        public static DocumentManagerSecurity Current => _documentSecurity.Value;

        public ISecurityManager SecurityManagerInstance { get; set; }

        #endregion

        #region Methods

        private DocumentManagerSecurity(ISecurityManager securityManager)
        {
            SecurityManagerInstance = securityManager;
        }

        public void ReloadSecurityRoles()
        {
            _securityGroup_Roles = null;
            _securityGroup_Roles = new SecurityDataSet.SecurityGroup_RoleDataTable();

            using(var ta = new SecurityGroup_RoleTableAdapter())
                ta.Fill(_securityGroup_Roles);
        }

        public void RefreshCommands()
        {
            FolderChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetCurrentFolder(DocumentsDataSet.DocumentFolderRow folder)
        {
            Folder = folder;

            FolderChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool IsInRole(string role)
        {
            if (Folder == null)
                return false;

            var securityGroups = Folder.GetDocumentFolder_SecurityGroupRows()
                .Where(sg => HasRole(sg.SecurityGroupRow, role));

            return securityGroups.Any(sg => SecurityManagerInstance.IsInGroup(sg.SecurityGroupID));
        }

        public bool IsDocumentAdministrator()
        {
            return SecurityManagerInstance.IsInGroup(ApplicationSettings.Current.DocumentAdministratorSecurityGroupId);
        }

        public void Cleanup()
        {
            _securityGroup_Roles = null;
        }

        private bool HasRole(DocumentsDataSet.SecurityGroupRow securityGroupRow, string role)
        {
            if(_securityGroup_Roles == null)
                ReloadSecurityRoles();

            return _securityGroup_Roles.FindBySecurityGroupIDSecurityRoleID(securityGroupRow.SecurityGroupID, role) != null;
        }

        #endregion
    }
}