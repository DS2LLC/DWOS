namespace NLog
{
    /// <summary>
    /// Contains additional information about a user.
    /// </summary>
    public class ClientUserInfo
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <remarks>
        /// Identifier is the unique identifier from your system for this user.
        /// </remarks>
        /// <value>The identifier.</value>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the UUID.
        /// </summary>
        /// <remarks>
        /// A device identifier. Could be used to identify users across
        /// devices, or machines that are breaking for many users.
        /// </remarks>
        /// <value>The UUID.</value>
        public string UUID { get; set; }
    }
}
