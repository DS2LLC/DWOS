namespace DWOS.UI.Utilities
{
    interface ILoginManager
    {
        /// <summary>
        /// Logs in a user with the given ID.
        /// </summary>
        /// <param name="userId"></param>
        void DoLogin(int? userId);

        /// <summary>
        ///   Logs out the current user.
        /// </summary>
        void LogOut();
    }
}
