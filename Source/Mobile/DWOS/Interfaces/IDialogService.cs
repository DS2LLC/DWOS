using System;

namespace DWOS
{
    /// <summary>
    /// Defines methods to show UI Dialogs.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows a dialog to the user.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="OKButtonContent"></param>
        /// <param name="onOkButtonClick"></param>
        void Show(string title, string message, string OKButtonContent = "OK", Action onOkButtonClick = null);


        /// <summary>
        /// Shows a toast to the user
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isShort">
        /// <c>true</c> if the toast should be short in duration;
        /// otherwise, <c>false</c>
        /// </param>
        void ShowToast(string message, bool isShort = true);
    }
}
