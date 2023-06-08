using System;
using System.ComponentModel;
using Infragistics.Win.UltraWinEditors;
using NLog;

namespace DWOS.Utilities.Validation
{
    public class CurrencyControlValidator : ControlValidatorBase
    {
        #region Methods

        public CurrencyControlValidator(UltraCurrencyEditor control) : base(control) { }

        internal override void ValidateControl(object sender, CancelEventArgs e)
        {
            try
            {
                var numEditor = (UltraCurrencyEditor) Control;

                if(Control.Enabled)
                {
                    if(numEditor.ValueObject == null)
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "Value is required.");
                        return;
                    }

                    decimal value = 0;
                    if(value < numEditor.MinValue)
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "Value must be greater than or equal to " + numEditor.MinValue);
                        return;
                    }

                    if(value > numEditor.MaxValue)
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "Value must be less than or equal to " + numEditor.MaxValue);
                        return;
                    }
                }

                //passed
                e.Cancel = false;
                FireAfterValidation(true, string.Empty);
            }
            catch(Exception exc)
            {
                string errorMsg = "Error validating the currency control: " + Control.Name;
                LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
            }
        }

        #endregion
    }
}