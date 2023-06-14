using DWOS.ViewModels;

namespace DWOS.Android
{
    /// <summary>
    /// Callback interface for <see cref="FilterFragment"/>.
    /// </summary>
    public interface IFilterFragmentCallback
    {
        /// <summary>
        /// Called when the user dismisses the filter dialog.
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="result"></param>
        /// <param name="department"></param>
        /// <param name="line"></param>
        /// <param name="status"></param>
        /// <param name="type"></param>
        void OnFilterDimissed(FilterFragment dialog, bool result, string department, string line, string status, FilterType type);
    }
}
