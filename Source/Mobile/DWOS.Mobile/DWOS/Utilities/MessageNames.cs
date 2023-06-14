namespace DWOS.Utilities
{
    /// <summary>
    /// Defines strings to use for messaging.
    /// </summary>
    internal static class MessageNames
    {
        /// <summary>
        /// Message sent when there is a problem when sending a request and
        /// the user must be logged-out.
        /// </summary>
        public const string LogoutExceptionMessage = "LogoutFromConnectionException";

        /// <summary>
        /// Message sent when a view model needs to be invalidated.
        /// </summary>
        public const string InvalidateViewModelMessage = "InvalidateViewModel";
    }
}
