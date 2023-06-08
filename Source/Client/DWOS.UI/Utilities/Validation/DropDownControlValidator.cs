using System.ComponentModel;
using Infragistics.Win.UltraWinEditors;

namespace DWOS.Utilities.Validation
{
    public class DropDownControlValidator : ControlValidatorBase
    {
        #region Methods

        public DropDownControlValidator(UltraComboEditor control, bool selectionRequired) : base(control) { IsRequired = selectionRequired; }

        internal override void ValidateControl(object sender, CancelEventArgs e)
        {
            var editor = Control as UltraComboEditor;

            if(editor != null && editor.Enabled)
            {
                if(IsRequired && editor.SelectedItem == null)
                {
                    e.Cancel = true;
                    FireAfterValidation(false, "A value is required to be selected.");
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