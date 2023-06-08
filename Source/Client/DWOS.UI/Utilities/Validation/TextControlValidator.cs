using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DWOS.Utilities.Validation
{
    public class TextControlValidator : ControlValidatorBase
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the Minimum length of the value.
        /// </summary>
        /// <remarks>
        /// Length is calculated after trimming the input value.
        /// </remarks>
        /// <value> The length of the min. </value>
        public int MinLength { get; set; }

        /// <summary>
        ///     Gets or sets the reg exp pattern to match the value.
        /// </summary>
        /// <value> The reg exp pattern. </value>
        public string RegExpPattern { get; set; }

        /// <summary>
        ///     Gets or sets the reg exp pattern presented to user (i.e. '(NNN) NNN-NNNN').
        /// </summary>
        /// <value> The reg exp text. </value>
        public string RegExpText { get; set; }

        /// <summary>
        ///     Gets or sets the default value of the control. TextValue should not equal this to be considered valid.
        /// </summary>
        /// <value> The default value. </value>
        public string DefaultValue { get; set; }

        public bool IsEnabled { get; set; }

        public Func<bool> ValidationRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether whitespace will be
        /// preserved after successful validation.
        /// </summary>
        public bool PreserveWhitespace { get; set; }

        #endregion

        #region Methods

        public TextControlValidator(Control control, string errMessage) : this(true, 0, control)
        {
            IsEnabled = true;
            ErrorMessage = errMessage;
        }

        public TextControlValidator(bool required, int minLength, Control control) : base(control)
        {
            IsEnabled = true;
            IsRequired = required;
            MinLength = minLength;
        }

        internal override void ValidateControl(object sender, CancelEventArgs e)
        {
            if (ValidationRequired != null && !ValidationRequired())
                return;

            string value = Control.Text;

            if(Control.Enabled && IsEnabled)
            {
                if(IsRequired && string.IsNullOrWhiteSpace(value))
                {
                    e.Cancel = true;
                    FireAfterValidation(false, ErrorMessage);
                    return;
                }

                if(MinLength > 0 && (!string.IsNullOrEmpty(value) && value.Trim().Length < MinLength))
                {
                    e.Cancel = true;
                    FireAfterValidation(false, "Minimum length of " + MinLength + " is required.");
                    return;
                }

                if(RegExpPattern != null && !string.IsNullOrEmpty(value) && !Regex.IsMatch(value, RegExpPattern))
                {
                    e.Cancel = true;
                    FireAfterValidation(false, "Value does not match required pattern '" + (RegExpText ?? RegExpPattern) + "'.");
                    return;
                }

                if(!string.IsNullOrEmpty(DefaultValue) && !string.IsNullOrEmpty(value))
                {
                    if(DefaultValue == value)
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "Value cannot match default value '" + DefaultValue + "'.");
                        return;
                    }
                }

                //try and trim text if we can
                if(Control.Text != null && !PreserveWhitespace)
                    Control.Text = Control.Text.Trim();
            }

            //passed
            e.Cancel = false;
            FireAfterValidation(true, value);
        }

        public override void Dispose()
        {
            RegExpPattern = null;
            RegExpText = null;

            base.Dispose();
        }

        #endregion
    }
}