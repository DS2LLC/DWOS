namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Interface for classes providing user notification functionality.
    /// </summary>
    public interface IUserNotifier
    {
        /// <summary>
        /// Shows the user a message.
        /// </summary>
        /// <param name="message"></param>
        void ShowNotification(string message);

    }
}
