namespace DWOS.Data
{
    /// <summary>
    /// Provides a <see cref="ApplicationSettings"/> instance in a way that
    /// easily supports dependency injection.
    /// </summary>
    public interface IDwosApplicationSettingsProvider
    {
        /// <summary>
        /// Gets the settings for this instance.
        /// </summary>
        ApplicationSettings Settings { get; }
    }
}
