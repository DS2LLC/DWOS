namespace DWOS.Data
{
    /// <summary>
    /// <see cref="IDwosApplicationSettingsProvider"/> implementation that uses
    /// <see cref="ApplicationSettings.Current"/>.
    /// </summary>
    public class DwosApplicationSettingsProvider : IDwosApplicationSettingsProvider
    {
        #region IApplicationSettingsProvider Members

        public ApplicationSettings Settings => ApplicationSettings.Current;

        #endregion
    }
}
