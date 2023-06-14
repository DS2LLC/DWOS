using DWOS.Services.Messages;

namespace DWOS.Android
{
    /// <summary>
    /// Callback interface for <see cref="ProcessDetailsFragment"/>.
    /// </summary>
    public interface IProcessDetailsFragmentCallback
    {
        /// <summary>
        /// Called when the user selects an order process.
        /// </summary>
        /// <param name="selectedProcessInfo"></param>
        void OnProcessDetailsSelected(OrderProcessInfo selectedProcessInfo);
    }
}