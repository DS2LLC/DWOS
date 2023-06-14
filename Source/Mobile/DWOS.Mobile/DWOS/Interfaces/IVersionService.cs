namespace DWOS
{
    /// <summary>
    /// Defines a method that retrieves the application's version.
    /// </summary>
    public interface IVersionService
    {
        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <returns></returns>
        string GetAppVersion();
    }
}
