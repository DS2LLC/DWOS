using System;

namespace DWOS.UI.Admin.ChangeWorkOrder
{
    /// <summary>
    /// Interface for <see cref="ChangeWorkOrderMain"/> panels.
    /// </summary>
    public interface IChangeWorkOrderPanel
    {
        /// <summary>
        ///   Handle back to the main form.
        /// </summary>
        ChangeWorkOrderMain MainForm { get; set; }

        /// <summary>
        ///   Get the method to call when <see cref="CanNavigateToNextPanel"/>
        ///   changes.
        /// </summary>
        Action<IChangeWorkOrderPanel, bool> OnCanNavigateToNextPanelChange { get; set; }

        /// <summary>
        ///   Determines if the Next button is enabled
        /// </summary>
        bool CanNavigateToNextPanel { get; }

        /// <summary>
        ///   Called when navigating to this panel
        /// </summary>
        void OnNavigateTo();

        /// <summary>
        ///   Called before navigating away from this panel.
        /// </summary>
        void OnNavigateFrom();

        /// <summary>
        ///   Called when finish is clicked.
        /// </summary>
        void Finish();
    }
}
