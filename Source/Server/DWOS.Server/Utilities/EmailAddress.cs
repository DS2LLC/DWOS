namespace DWOS.Server.Utilities
{
    /// <summary>
    /// Represents an email address.
    /// </summary>
    internal class EmailAddress
    {
        public static EmailAddress Default => new EmailAddress();

        /// <summary>
        /// Gets or sets the email address for this instance..
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///   Gets or sets the display name for this instance.
        /// </summary>
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return DisplayName == null
                ? Address
                : $"\"{DisplayName}\" <{Address}>";
        }
    }
}
