namespace DWOS.Android
{
    /// <summary>
    /// Callback interface for <see cref="OrderProcessQuestionFragment"/>.
    /// </summary>
    public interface IOrderProcessQuestionCallback
    {
        /// <summary>
        /// Called when DWOS Mobile should automatically show the next
        /// question in a process.
        /// </summary>
        void NextQuestion();
    }
}
