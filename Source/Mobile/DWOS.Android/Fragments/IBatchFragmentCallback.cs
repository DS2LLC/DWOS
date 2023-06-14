namespace DWOS.Android
{
    /// <summary>
    /// Callback interface for <see cref="BatchListFragment"/>.
    /// </summary>
    public interface IBatchFragmentCallback
    {
        /// <summary>
        /// Called when the user selects a batch.
        /// </summary>
        /// <param name="batchId">ID of the selected batch.</param>
        void OnBatchSelected(int batchId);
    }
}
