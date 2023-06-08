using System;
using System.ComponentModel;
using Infragistics.Win.UltraWinEditors;
using NLog;

namespace DWOS.Utilities.Validation
{
    public class DateTimeValidator : ControlValidatorBase
    {
        #region Properties

        public Func<bool> ValidationRequired { get; set; }

        #endregion

        #region Methods

        public DateTimeValidator(UltraDateTimeEditor control, string errMessage) : base(control) { ErrorMessage = errMessage; }

        internal override void ValidateControl(object sender, CancelEventArgs e)
        {
            try
            {
                if (ValidationRequired != null && !ValidationRequired())
                {
                    return;
                }

                var numEditor = (UltraDateTimeEditor) Control;

                if(Control.Enabled)
                {
                    if(numEditor.Value == null)
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, ErrorMessage);
                        return;
                    }
                }

                //passed
                e.Cancel = false;
                FireAfterValidation(true, string.Empty);
            }
            catch(Exception exc)
            {
                var errorMsg = "Error validating the date time editor control: " + Control.Name;
                LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
            }
        }

        #endregion
    }
}