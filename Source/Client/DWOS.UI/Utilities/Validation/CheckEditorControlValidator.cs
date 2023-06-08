using System.ComponentModel;
using Infragistics.Win.UltraWinEditors;

namespace DWOS.Utilities.Validation
{
    public class CheckEditorControlValidator : ControlValidatorBase
    {
        #region Methods

        public CheckEditorControlValidator(UltraCheckEditor control, bool checkRequired) : base(control) { IsRequired = checkRequired; }

        internal override void ValidateControl(object sender, CancelEventArgs e)
        {
            var editor = Control as UltraCheckEditor;

            if(editor != null && editor.Enabled)
            {
                if(IsRequired && !editor.Checked)
                {
                    e.Cancel = true;
                    FireAfterValidation(false, "The checkbox is required to be checked.");
                    return;
                }
            }

            //passed
            e.Cancel = false;
            FireAfterValidation(true, "");
        }

        #endregion
    }
}