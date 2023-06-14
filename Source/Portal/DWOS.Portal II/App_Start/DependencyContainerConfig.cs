using DWOS.Shared.Utilities;

namespace DWOS.Portal
{
    /// <summary>
    /// Initializes <see cref="DependencyContainer"/> for use throughout the Portal site.
    /// </summary>
    public static class DependencyContainerConfig
    {
        public static void RegisterDependencies()
        {
            DependencyContainer.Register<Data.Date.ICalendarPersistence>(new Data.Date.CalendarPersistence());
        }
    }
}