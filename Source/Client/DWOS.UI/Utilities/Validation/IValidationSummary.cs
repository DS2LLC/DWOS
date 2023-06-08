namespace DWOS.Utilities.Validation
{
    public interface IValidationSummary
    {
        void Reset();
        void Complete();
        void StatusUpdate(DisplayValidator validator, bool isValid);
    }
}