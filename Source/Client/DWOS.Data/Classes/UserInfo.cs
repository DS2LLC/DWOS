using DWOS.Shared.Utilities;

namespace DWOS.Data.Coc
{
    public class User : ISecurityUserInfo
    {
        #region Methods

        public User(int userId, string userName)
        {
            UserID = userId;
            UserName = userName;
        }

        #endregion

        #region ISecurityUserInfo Members

        public int UserID { get; }

        public string UserName { get; }

        #endregion
    }
}