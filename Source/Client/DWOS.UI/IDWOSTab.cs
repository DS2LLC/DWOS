using System;
using System.Windows.Controls;

namespace DWOS.UI
{
    /// <summary>
    /// Provides tab-related functionality.
    /// </summary>
    public interface IDwosTab
    {
        /// <summary>
        /// Gets or sets a unique key for this instance.
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Gets or sets the data type for this instance.
        /// </summary>
        /// <remarks>
        /// This is later used by <see cref="DwosTabFactory.CreateTab"/> to
        /// create a tab instance.
        /// </remarks>
        string DataType { get; }

        /// <summary>
        /// Gets or sets the name for this instance.
        /// </summary>
        string TabName { get; set; }

        /// <summary>
        /// Gets the actual tab control.
        /// </summary>
        UserControl TabControl { get; }

        /// <summary>
        /// Saves the current layout to the local file system.
        /// </summary>
        void SaveLayout();

        /// <summary>
        /// Loads the last saved layout to the local file system.
        /// </summary>
        void LoadLayout();

        /// <summary>
        /// Loads a layout by its content.
        /// </summary>
        /// <param name="content"></param>
        void LoadLayout(string content);

        /// <summary>
        /// Initializes work-in-progress data for this instance.
        /// </summary>
        /// <param name="data"></param>
        void Initialize(WipData data);

        /// <summary>
        /// Refreshes data in this instance.
        /// </summary>
        void RefreshData(WipData data);

        /// <summary>
        /// Exports tab data to a new <see cref="DwosTabData"/> instance.
        /// </summary>
        /// <returns></returns>
        DwosTabData Export();

        /// <summary>
        /// Occurs when there is an error loading a layout.
        /// </summary>
        event EventHandler LayoutError;
    }
}
