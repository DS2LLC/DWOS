using System;
using DWOS.Shared.Utilities;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    public class AwotSecurityUserInfo : ISecurityUserInfo
    {
        #region Properties

        public IUserManager UserManager { get; }

        #endregion

        #region Methods

        public AwotSecurityUserInfo(IUserManager userManager)
        {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        #endregion

        #region ISecurityUserInfo Members

        public int UserID =>
            UserManager.CurrentUser?.Id ?? 0;

        public string UserName =>
            UserManager.CurrentUser?.Name;

        #endregion
    }
}
