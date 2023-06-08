namespace DWOS.Shared
{
    /// <summary>
    /// Represents an item in a combo box.
    /// </summary>
    public class ComboBoxItem
    {
        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ComboBoxItem"/> class.
        /// </summary>
        /// <param name="value">The value of the item.</param>
        /// <param name="displayText">The display text of the item.</param>
        public ComboBoxItem(object value, string displayText)
        {
            Value = value;
            DisplayText = displayText;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value for this instance.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the display text for this instance.
        /// </summary>
        public string DisplayText { get; set; }

        #endregion
    }
}
