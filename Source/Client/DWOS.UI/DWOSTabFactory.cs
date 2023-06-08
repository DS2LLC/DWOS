using DWOS.UI.Dashboard;

namespace DWOS.UI
{
    /// <summary>
    /// Provides a method that creates an <see cref="IDwosTab"/> instance.
    /// </summary>
    public static class DwosTabFactory
    {
        /// <summary>
        /// Creates a tab.
        /// </summary>
        /// <param name="dataType">The tab's data type.</param>
        /// <param name="key">The tab's unique key.</param>
        /// <param name="name">The tab's name.</param>
        /// <returns>
        /// A new tab instance if <paramref name="dataType"/> matches a known
        /// data type; otherwise, <c>null</c>.
        /// </returns>
        public static IDwosTab CreateTab(string dataType, string key, string name)
        {
            IDwosTab tab = null;

            if(dataType == OrderSummary2.DATA_TYPE)
                tab = new OrderSummary2();
            else if(dataType == DashboardTab.DATA_TYPE)
                tab = new DashboardTab();
            else if (dataType == BatchSummary.DATA_TYPE)
                tab = new BatchSummary();
            else if (dataType == Admin.Schedule.Manual.SchedulingTab.DATA_TYPE)
                tab = new Admin.Schedule.Manual.SchedulingTab();

            if(tab != null)
            {
                tab.TabName = name;
                tab.Key = key;
            }

            return tab;
        }
    }
}