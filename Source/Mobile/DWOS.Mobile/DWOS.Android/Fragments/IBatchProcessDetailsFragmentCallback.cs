using DWOS.Services.Messages;

namespace DWOS.Android
{
    /// <summary>
    /// Callback interface for <see cref="BatchProcessDetailsFragment"/>.
    /// </summary>
    public interface IBatchProcessDetailsFragmentCallback
    {
        /// <summary>
        /// Called when the user selects a batch process.
        /// </summary>
        /// <param name="selectedProcessInfo"></param>
        void OnBatchProcessDetailsSelected(BatchProcessInfo selectedProcessInfo);
    }
}
