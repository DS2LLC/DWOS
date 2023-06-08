using System;
using DWOS.UI.Properties;

namespace DWOS.UI.Tools
{
    /// <summary>
    /// Refreshes <see cref="MainForm"/> when disposed.
    /// </summary>
    /// <remarks>
    /// This can optionally prevent <see cref="MainForm"/> from refreshing if
    /// <see cref="Settings.PauseRefreshWithOpenDialog"/> is <c>true</c>.
    /// </remarks>
    internal class MainRefreshHelper : IDisposable
    {
        public Main MainForm { get; }
        public RefreshType RefreshType { get; }

        public MainRefreshHelper(Main mainForm, RefreshType refreshType = RefreshType.Order)
        {
            if (mainForm == null)
            {
                throw new ArgumentNullException(nameof(mainForm));
            }

            MainForm = mainForm;
            RefreshType = refreshType;

            if (Settings.Default.PauseRefreshWithOpenDialog)
            {
                MainForm.EnableRefresh = false;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            MainForm.EnableRefresh = true;
            MainForm.RefreshData(RefreshType);
        }

        #endregion
    }
}
