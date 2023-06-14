using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DWOS.AutomatedWorkOrderTool.Messages;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace DWOS.AutomatedWorkOrderTool.Dialogs
{
    /// <summary>
    /// Provides extension methods for showing dialogs.
    /// </summary>
    /// <remarks>
    /// MahApp's <see cref="DialogManager"/> class defines similar methods, and
    /// this class leverages <see cref="DialogManager"/> to actually show the
    /// dialogs. Custom integration was required to close the dialogs.
    /// </remarks>
    public static class MetroWindowExtensions
    {
        #region Methods

        /// <summary>
        /// Creates an 'Add Customer' dialog inside of the current window.
        /// </summary>
        /// <param name="window">The window that is the parent of this dialog.</param>
        /// <param name="settings">Optional settings that override the global metro dialog settings.</param>
        /// <returns>
        /// The customer instance selected from the dialog.
        /// </returns>
        public static Task<CustomerViewModel> ShowAddCustomerDialog(this MetroWindow window, MetroDialogSettings settings = null)
        {
            var dialog = new AddCustomerDialog(window, settings);

            return window.ShowMetroDialogAsync(dialog)
                .ContinueWith(showTask =>
                {
                    return dialog.WaitForResult()
                    .ContinueWith(buttonTask =>
                    {
                        // Possibly happens off of UI thread, so run it on dispatcher's thread.
                        window.Dispatcher.Invoke(new Action(() =>
                        {
                            window.HideMetroDialogAsync(dialog, settings);
                        }));

                        return buttonTask.Result;
                    }).ContinueWith(y => y.Result);
                }).Unwrap();
        }

        /// <summary>
        /// Creates an 'Add Manufacturer' dialog inside of the current window.
        /// </summary>
        /// <param name="window">The window that is the parent of this dialog.</param>
        /// <param name="currentCustomer">Customer to add a manufacturer to.</param>
        /// <param name="settings">Optional settings that override the global metro dialog settings.</param>
        /// <returns>
        /// The manufacturer (OSP format) instance selected from the dialog.
        /// </returns>
        public static Task<ManufacturerViewModel> ShowAddManufacturerDialog(this MetroWindow window, CustomerViewModel currentCustomer, MetroDialogSettings settings = null)
        {
            var dialog = new AddManufacturerDialog(window, settings)
            {
                Customer = currentCustomer
            };

            return window.ShowMetroDialogAsync(dialog)
                .ContinueWith(showTask =>
                {
                    return dialog.WaitForResult()
                    .ContinueWith(buttonTask =>
                    {
                        // Possibly happens off of UI thread, so run it on dispatcher's thread.
                        window.Dispatcher.Invoke(new Action(() =>
                        {
                            window.HideMetroDialogAsync(dialog, settings);
                        }));

                        return buttonTask.Result;
                    }).ContinueWith(y => y.Result);
                }).Unwrap();
        }

        public static Task<OspFormatSectionViewModel> ShowAddOspFormatSectionDialog(this MetroWindow window,
            OspFormatViewModel currentFormat, List<OspFormatSectionViewModel> sections, MetroDialogSettings settings = null)
        {
            var dialog = new AddOspFormatSectionDialog(window, settings)
            {
                CurrentFormat = currentFormat,
                CurrentSections = sections
            };

            return window.ShowMetroDialogAsync(dialog)
                .ContinueWith(showTask =>
                {
                    return dialog.WaitForResult()
                    .ContinueWith(buttonTask =>
                    {
                        // Possibly happens off of UI thread, so run it on dispatcher's thread.
                        window.Dispatcher.Invoke(new Action(() =>
                        {
                            window.HideMetroDialogAsync(dialog, settings);
                        }));

                        return buttonTask.Result;
                    }).ContinueWith(y => y.Result);
                }).Unwrap();
        }

        public static Task<AddOspFormatCodeMapDialog.DialogResult> ShowAddOspFormatCodeMapDialog(this MetroWindow window,
            ShowAddCodeMapDialogMessage msg, MetroDialogSettings settings = null)
        {
            if (msg == null)
            {
                throw new ArgumentNullException(nameof(msg));
            }

            var dialog = new AddOspFormatCodeMapDialog(window, settings);
            dialog.Load(msg.CurrentFormat, msg.CurrentSections);

            return window.ShowMetroDialogAsync(dialog)
                .ContinueWith(showTask =>
                {
                    return dialog.WaitForResult()
                    .ContinueWith(buttonTask =>
                    {
                        // Possibly happens off of UI thread, so run it on dispatcher's thread.
                        window.Dispatcher.Invoke(new Action(() =>
                        {
                            window.HideMetroDialogAsync(dialog, settings);
                        }));

                        return buttonTask.Result;
                    }).ContinueWith(y => y.Result);
                }).Unwrap();
        }

        #endregion
    }
}
