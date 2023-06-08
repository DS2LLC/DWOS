using System;
using System.Windows.Forms;

namespace DWOS.Shared.Wizard
{
    /// <summary>
    /// Base object for implementing controllers for <see cref="WizardDialog"/>.
    /// </summary>
    public abstract class WizardController: IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the title of the wizard.
        /// </summary>
        public abstract string WizardTitle { get; }

        /// <summary>
        /// <summary>
        /// Gets or sets the dialog for the wizard.
        /// </summary>
        public WizardDialog Dialog { get; set; }

        #endregion

        #region Methods

        /// Called when the user finishes the wizard.
        /// </summary>
        public abstract void Finished();

        /// <summary>
        /// Shows <see cref="Dialog"/> with an owner.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public DialogResult ShowDialog(IWin32Window owner)
        {
            return this.Dialog.ShowDialog(owner);
        }

        public virtual void Dispose()
        {
            if(this.Dialog != null)
            {
                this.Dialog.Close();
                this.Dialog.Dispose();
                this.Dialog = null;
            }
        }

        #endregion
    }
}
