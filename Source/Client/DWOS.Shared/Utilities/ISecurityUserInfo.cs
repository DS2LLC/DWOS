namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Interface for classes implementing security management functionality.
    /// </summary>
    public interface ISecurityUserInfo
    {
        /// <summary>
        /// Gets the associated user's ID.
        /// </summary>
        int UserID { get; }

        /// <summary>
        /// Gets the associated user's full name.
        /// </summary>
        string UserName { get; }
    }

}