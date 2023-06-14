namespace DWOS.Services.Messages
{
    /// <summary>
    /// Client request to log information.
    /// </summary>
    public class LogMessageRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the message for this instance.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets user information for this instance.
        /// </summary>
        public UserProfileInfo UserProfile { get; set; }
    }
}
