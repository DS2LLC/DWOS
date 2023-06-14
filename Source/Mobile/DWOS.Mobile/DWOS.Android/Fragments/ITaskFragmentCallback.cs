using DWOS.ViewModels;

namespace DWOS.Android
{
    /// <summary>
    /// Callback interface for <see cref="TaskFragment"/>.
    /// </summary>
    public interface ITaskFragmentCallback
    {
        /// <summary>
        /// Called when the task completes.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="tag"></param>
        void TaskCompleted(ViewModelResult result, object tag);
    }
}