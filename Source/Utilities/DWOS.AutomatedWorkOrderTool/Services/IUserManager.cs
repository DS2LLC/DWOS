using System;
using System.Windows.Media;
using DWOS.AutomatedWorkOrderTool.Model;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    /// <summary>
    /// Manages information for the current user.
    /// </summary>
    public interface IUserManager
    {
        /// <summary>
        /// Occurs when the user changes.
        /// </summary>
        event EventHandler<UserChangedEventArgs> UserChanged;

        /// <summary>
        /// Gets the current user.
        /// </summary>
        DwosUser CurrentUser { get; }

        /// <summary>
        /// Attempts to login with the given PIN.
        /// </summary>
        /// <param name="pin">PIN to use for login.</param>
        /// <returns>
        /// <c>true</c> if login succeeds; otherwise, <c>false</c>.
        /// </returns>
        bool LogIn(string pin);

        /// <summary>
        /// Logs the current user out.
        /// </summary>
        void LogOut();

        /// <summary>
        /// Retrieves a media with the given ID
        /// </summary>
        /// <remarks>
        /// Assumption: Media for users is always an image.
        /// </remarks>
        /// <param name="user"></param>
        /// <returns></returns>
        ImageSource GetImage(DwosUser user);
    }
}
