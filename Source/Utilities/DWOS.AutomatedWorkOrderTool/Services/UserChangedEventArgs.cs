using System;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    /// <summary>
    /// Provides data for the <see cref="IUserManager.UserChanged"/> event.
    /// </summary>
    public class UserChangedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the change type for this instance.
        /// </summary>
        public ChangeType Type { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="UserChangedEventArgs"/> class.
        /// </summary>
        /// <param name="type"></param>
        public UserChangedEventArgs(ChangeType type)
        {
            Type = type;
        }

        #endregion

        #region ChangeType

        /// <summary>
        /// Represents a type of user change.
        /// </summary>
        public enum ChangeType
        {
            /// <summary>
            /// Expected - user logged in or logged out.
            /// </summary>
            Expected,

            /// <summary>
            /// Unexpected - something other than user interaction caused the
            /// user to log out.
            /// </summary>
            Unexpected
        }

        #endregion
    }
}
