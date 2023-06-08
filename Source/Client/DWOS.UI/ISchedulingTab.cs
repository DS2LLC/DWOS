using DWOS.Data.Datasets;
using DWOS.UI.Admin.Schedule.Manual;

namespace DWOS.UI
{
    /// <summary>
    /// Interface for <see cref="SchedulingTab"/> - represents a scheduling tab.
    /// </summary>
    public interface ISchedulingTab : IDwosTab
    {
        /// <summary>
        /// Called when settings change.
        /// </summary>
        void RefreshSettings();

        /// <summary>
        /// Called when closed.
        /// </summary>
        void OnClose();
    }
}