using System.Drawing;

namespace DWOS.UI.Admin
{
    /// <summary>
    /// Settings panel used in <see cref="Settings>"/>.
    /// </summary>
    internal interface ISettingsPanel
    {
        /// <summary>
        /// Gets a value indicating if this instance is editable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance accepts edits;
        /// otherwise, <c>false</c>.
        /// </value>
        bool Editable { get; }

        /// <summary>
        /// Gets the key for this instance.
        /// </summary>
        string PanelKey { get; }

        /// <summary>
        /// Loads data for this instance.
        /// </summary>
        void LoadData();

        /// <summary>
        /// Saves data for this instance.
        /// </summary>
        void SaveData();

        /// <summary>
        /// Gets the minimum size for this instance.
        /// </summary>
        Size MinimumSize { get; }

        bool CanDock
        {
            get;
        }
    }


}
