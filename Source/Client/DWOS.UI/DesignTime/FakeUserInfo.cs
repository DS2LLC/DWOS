using DWOS.Shared.Utilities;
using System;

namespace DWOS.UI.DesignTime
{
    internal class FakeUserInfo : ISecurityUserInfo
    {
        #region ISecurityUserInfo Members

        public int UserID => 1;

        public string UserName => "Admin";

        #endregion
    }
}
