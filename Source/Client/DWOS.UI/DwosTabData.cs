namespace DWOS.UI
{
    /// <summary>
    /// Represents tab information for an <see cref="IDwosTab"/> instance.
    /// </summary>
    public class DwosTabData
    {
        #region Fields

        private const int MAX_KEY_LENGTH = 36;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the layout for this instance.
        /// </summary>
        /// <remarks>
        /// The value for this layout must be in a format that the instance
        /// can restore from. For example, if the <see cref="IDwosTab"/>
        /// instance uses JSON to specify its layout, then this should use
        /// JSON as well.
        /// </remarks>
        public string Layout { get; set; }

        /// <summary>
        /// Gets the data type for this instance.
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Gets the unique key for this instance.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets the layout version for this instance.
        /// </summary>
        public int? Version { get; set; }

        /// <summary>
        /// Gets a value indicating if this instance is valid.
        /// </summary>
        public bool IsValid => !string.IsNullOrEmpty(Name) &&
            !string.IsNullOrEmpty(DataType) &&
            !string.IsNullOrEmpty(Key) &&
            Key.Length <= MAX_KEY_LENGTH;

        #endregion
    }
}
