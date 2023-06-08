using System;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Contains user update event data
    /// </summary>
    public class UserUpdatedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating if the user ID changed during the update.
        /// </summary>
        /// <remarks>
        /// <c>true</c> if the user ID changed; otherwise, <c>false</c>.
        /// </remarks>
        public bool UserIdChanged { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="UserUpdatedEventArgs"/> class.
        /// </summary>
        /// <param name="userIdChanged"></param>
        public UserUpdatedEventArgs(bool userIdChanged)
        {
            UserIdChanged = userIdChanged;
        }

        #endregion
    }
}
