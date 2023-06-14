namespace DWOS.Android
{
    /// <summary>
    /// Callback interface for <see cref="OrderProcessStepListFragment"/>.
    /// </summary>
    public interface IOrderProcessStepListCallback
    {
        /// <summary>
        /// Called when the user selects a step in a process.
        /// </summary>
        /// <param name="stepId"></param>
        void OnStepSelected(int stepId);
    }
}
