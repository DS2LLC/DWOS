using System;
using System.ComponentModel;
using Infragistics.Win.UltraWinEditors;

namespace DWOS.Utilities.Validation
{
    public class NumericControlValidator : ControlValidatorBase
    {
        #region Properties

        public double? MinimumValue { get; set; }

        public Func <bool> ValidationRequired { get; set; }

        #endregion

        #region Methods

        public NumericControlValidator(UltraNumericEditor control) : base(control) { }

        internal override void ValidateControl(object sender, CancelEventArgs e)
        {
            if(ValidationRequired != null && !ValidationRequired())
                return;

            var numEditor = (UltraNumericEditor) Control;

            if(Control.Enabled)
            {
                if((numEditor.Value == null || numEditor.Value == DBNull.Value) && !numEditor.Nullable)
                {
                    e.Cancel = true;
                    FireAfterValidation(false, "Numeric value must not be null.");
                    return;
                }

                if(numEditor.Value != null && numEditor.Value != DBNull.Value)
                {
                    double value = Convert.ToDouble(numEditor.Value);

                    if(MinimumValue.HasValue && value < MinimumValue.Value)
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "Value must be greater than or equal to " + MinimumValue.Value);
                        return;
                    }

                    if(value < Convert.ToDouble(numEditor.MinValue))
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "Value must be greater than or equal to " + numEditor.MinValue);
                        return;
                    }

                    if(value > Convert.ToDouble(numEditor.MaxValue))
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "Value must be less than or equal to " + numEditor.MaxValue);
                        return;
                    }
                }
            }

            //passed
            e.Cancel = false;
            FireAfterValidation(true, "");
        }

        #endregion
    }
}