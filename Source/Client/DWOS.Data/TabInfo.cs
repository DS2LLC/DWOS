namespace DWOS.Data
{
    /// <summary>
    /// Contains information about a tab.
    /// </summary>
    public class TabInfo
    {
        /// <summary>
        /// Gets or sets the data type of the tab.
        /// </summary>
        /// <remarks>
        /// This value is used after serialization to initialize an instance of
        /// the tab. It is not necessarily the full name of the tab's class.
        /// </remarks>
        public string DataType { get; set; }

        /// <summary>
        /// Gets or sets the key of the tab.
        /// </summary>
        /// <remarks>
        /// This should be an unique identifier.
        /// </remarks>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the display name of the tab.
        /// </summary>
        public string Name { get; set; }
    }
}
