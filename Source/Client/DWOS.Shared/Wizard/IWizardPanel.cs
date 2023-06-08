using System;
using System.Windows.Forms;

namespace DWOS.Shared.Wizard
{
    /// <summary>
    /// Interface for <see cref="WizardDialog"/> panel implementations.
    /// </summary>
    public interface IWizardPanel: IDisposable
    {
        /// <summary>
        /// Gets the title of this instance.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the subtitle of this instance.
        /// </summary>
        string SubTitle { get; }

        /// <summary>
        /// Gets the action to invoke when <see cref="IsValid"/> changes?
        /// </summary>
        Action<IWizardPanel> OnValidStateChanged { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="controller"></param>
        void Initialize(WizardController controller);

        /// <summary>
        /// Gets the control for the panel.
        /// </summary>
        Control PanelControl { get; }

        /// <summary>
        /// Gets or sets a valid indicating if the panel is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Called when the wizard dialog switches to this instance.
        /// </summary>
        void OnMoveTo();

        /// <summary>
        /// Called when the wizard dialog switches from this instance.
        /// </summary>
        void OnMoveFrom();

        /// <summary>
        /// Called when the user finishes the wizard dialog.
        /// </summary>
        void OnFinished();
    }
}
