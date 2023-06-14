namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class ValidationResult
    {
        #region Properties

        public bool IsSuccessful { get; }

        public string Message { get; }

        #endregion

        #region Methods

        private ValidationResult(bool isSuccessful, string message)
        {
            IsSuccessful = isSuccessful;
            Message = message;
        }

        public static ValidationResult Success() =>
            new ValidationResult(true, string.Empty);

        public static ValidationResult Failure(string msg) =>
            new ValidationResult(false, msg);

        #endregion
    }
}
