namespace DWOS.Android
{
    /// <summary>
    /// Callback interface for <see cref="InspectionQuestionFragment"/>.
    /// </summary>
    public interface IInspectionQuestionCallback
    {
        /// <summary>
        /// Called when DWOS Mobile should automatically show the next
        /// question in an inspection.
        /// </summary>
        void NextQuestion();
    }
}
