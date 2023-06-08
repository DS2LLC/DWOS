using DWOS.Shared.Utilities;
using DWOS.UI.Utilities;
using System;

namespace DWOS.UI.DesignTime
{
    internal class FakeSecurityManager : ISecurityManager
    {
        #region ISecurityManager Members

        public ISecurityUserInfo UserInfo => new FakeUserInfo();

        public event EventHandler<UserUpdatedEventArgs> UserUpdated
        {
            add { }
            remove { }
        }

        public bool RequiresOrderReview =>
            true;

        public bool IsInGroup(int securityGroupId) =>
            true;

        public bool IsInRole(string role) =>
            true;

        #endregion
    }
}
