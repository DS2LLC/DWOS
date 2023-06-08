using System.ComponentModel;

namespace DWOS.Dashboard
{
    /// <summary>
    /// Interface for classes implementing dashboard widget functionality.
    /// </summary>
    public interface IDashboardWidget2 : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the display name of this instance.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the control for this instance.
        /// </summary>
        /// <remarks>
        /// The instance returned by this property is typically the
        /// same as this instance.
        /// </remarks>
        System.Windows.Controls.Control Control { get; }

        /// <summary>
        /// Gets the settings for this instance.
        /// </summary>
        WidgetSettings Settings { get; set; }

        /// <summary>
        /// Starts the refresh -> show cycle for this instance.
        /// </summary>
        /// <remarks>
        /// </remarks>
        void Start();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();

        /// <summary>
        /// Called when names of departments change.
        /// </summary>
        /// <remarks>
        /// Implementers should refresh department names in the widget.
        /// </remarks>
        void OnDepartmentsChanged();
    }
}
