namespace DWOS.Data
{
    /// <summary>
    /// Interface for classes implementing functionality to provide local file
    /// system paths.
    /// </summary>
    public interface IPathProvider
    {
        /// <summary>
        /// Gets the primary image directory for the application.
        /// </summary>
        string ImageDirectory { get; }
    }
}