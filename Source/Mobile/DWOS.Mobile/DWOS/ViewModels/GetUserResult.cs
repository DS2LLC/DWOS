using DWOS.Services.Messages;
using System;

namespace DWOS.ViewModels
{
    /// <summary>
    /// Represents a result of requesting a specific user.
    /// </summary>
    public class GetUserResult : ViewModelResult
    {
        #region Properties

        /// <summary>
        /// Gets the user for this instance.
        /// </summary>
        public UserInfo User { get; private set; }

        #endregion

        #region Methods

        private GetUserResult()
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="GetUserResult"/> class.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="success"></param>
        /// <param name="errorMessage"></param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="userInfo"/> is null.
        /// </exception>
        public GetUserResult(UserInfo userInfo, bool success, string errorMessage)
            : base(success, errorMessage)
        {
            if (userInfo == null)
                throw new ArgumentNullException("profile");

            User = userInfo;
        }

        #endregion
    }


}
