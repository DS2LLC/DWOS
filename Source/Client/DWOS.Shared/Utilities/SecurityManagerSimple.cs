namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// A simple implementation of <see cref="ISecurityUserInfo"/>.
    /// </summary>
    public class SecurityManagerSimple : ISecurityUserInfo
    {
        /// <summary>
        /// Returns an 'imposter' instance of
        /// <see cref="SecurityManagerSimple"/> that is not associated with
        /// a user.
        /// </summary>
        public static SecurityManagerSimple ServerSecurityImposter
        {
            get { return new SecurityManagerSimple {UserID = 0, UserName = "Server"}; }
        }

        /// <summary>
        /// Gets or sets the associated user's ID for this instance.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Gets or set the associated user's name for this instance.
        /// </summary>
        public string UserName { get; set; }
    }


}
