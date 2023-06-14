namespace DWOS.Server.Admin.SettingsPanels
{
    /// <summary>
    /// Interface for settings panel classes.
    /// </summary>
    internal interface ISettingsPanel
    {
        /// <summary>
        /// Gets a value indicating if the panel is currently editable.
        /// </summary>
        /// <value>
        /// <c>true</c> if the panel is editable; otherwise, <c>false</c>.
        /// </value>
        bool Editable { get; }

        /// <summary>
        /// Gets the identifier key for the panel.
        /// </summary>
        string PanelKey { get; }

        /// <summary>
        /// Gets a value indicating if the panel's contents are valid.
        /// </summary>
        /// <value>
        /// <c>true</c> if the panel's contents are valid; otherwise, <c>false</c>.
        /// </value>
        bool IsValid { get; }

        /// <summary>
        /// Initializes this panel.
        /// </summary>
        void LoadData();

        /// <summary>
        /// Persists changes made in this panel.
        /// </summary>
        void SaveData();
    }
}
