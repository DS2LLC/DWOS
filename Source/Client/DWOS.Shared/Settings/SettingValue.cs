namespace DWOS.Shared.Settings
{
    /// <summary>
    /// Represents a setting.
    /// </summary>
    public class SettingValue
    {
        #region Properties

        /// <summary>
        /// Gets or sets the setting name for this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the setting value for this instance.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the converted value for this instance.
        /// </summary>
        public object ConvertedValue { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return this.Name;
        }

        #endregion
    }
}
