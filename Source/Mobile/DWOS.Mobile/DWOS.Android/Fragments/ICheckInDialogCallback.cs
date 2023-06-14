using DWOS.ViewModels;

namespace DWOS.Android
{
    /// <summary>
    /// Callback interface for <see cref="CheckInFragment"/>.
    /// </summary>
    public interface ICheckInDialogCallback
    {
        /// <summary>
        /// Called when the dialog is dismissed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="result">if set to <c>true</c> the user selected to check in; false if cancel.</param>
        /// <param name="checkInViewModel">The view model representing the selected tiems.</param>
        void CheckInDialogDismissed(CheckInFragment sender, bool result, CheckInViewModel checkInViewModel);
    }

}
