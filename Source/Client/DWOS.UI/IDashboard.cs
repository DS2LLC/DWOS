using DWOS.UI.Dashboard;

namespace DWOS.UI
{
    /// <summary>
    /// Interface for <see cref="DashboardTab"/> - represents a dashboard tab.
    /// </summary>
    public interface IDashboard : IDwosTab
    {
        /// <summary>
        /// Call on department update.
        /// </summary>
        void OnDepartmentsChanged();

        /// <summary>
        /// Call when closing.
        /// </summary>
        void OnClose();
    }
}