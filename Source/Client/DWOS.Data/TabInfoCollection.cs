using System.Collections.Generic;

namespace DWOS.Data
{
    /// <summary>
    /// Contains a collection of <see cref="TabInfo"/>.
    /// </summary>
    /// <remarks>
    /// This is structured for JSON serialization.
    /// </remarks>
    public class TabInfoCollection
    {
        /// <summary>
        /// Gets or sets the collection of tabs.
        /// </summary>
        public List<TabInfo> Tabs { get; set; }
    }
    
}
