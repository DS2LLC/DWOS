using System;
using System.ComponentModel;
using Infragistics.Win;

namespace DWOS.Utilities.Validation
{
    public abstract class DisplayValidator : IDisposable
    {
        #region Fields

        private ControlValidatorBase _validator;
        private bool _isEnabled = true;
        private bool _passedLastValidation = true;

        #endregion

        #region Properties

        public ControlValidatorBase Validator
        {
            get { return _validator; }
            set
            {
                _validator = value;

                if(_validator != null)
                {
                    _validator.AfterValidation += validator_AfterValidation;
                    _validator.OnFormClosing += validator_OnFormClosing;
                    _validator.IsRequiredChanged += Validator_IsRequiredChanged;

                    //update style
                    SetStyleSetName(_passedLastValidation);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the name of the invalid style set to use during invalid state.
        /// </summary>
        /// <value> The name of the invalid style set. </value>
        public string InvalidStyleSetName { get; set; } = "Invalid";

        /// <summary>
        ///     Gets or sets the name of the required style set to use to show that controls are required.
        /// </summary>
        /// <value> The name of the required style set. </value>
        public string RequiredStyleSetName { get; set; } = "Required";

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;

                if(_isEnabled)
                {
                    SetStyleSetName(_passedLastValidation);
                }
                else
                {
                    var baseCtl = Validator?.ValidatingControl as UltraControlBase;

                    if(baseCtl != null)
                        baseCtl.StyleSetName = null;
                }
            }
        }

        #endregion

        #region Methods

        protected DisplayValidator()
        {
        }

        protected DisplayValidator(ControlValidatorBase validator)
        {
            Validator = validator;
        }

        protected void SetStyleSetName(bool passed)
        {
            _passedLastValidation = passed;

            if (InvalidStyleSetName == null && RequiredStyleSetName == null)
            {
                return;
            }

            if (!(Validator?.ValidatingControl is UltraControlBase baseCtl))
            {
                return;
            }

            string styleSetName;
            if (!passed)
            {
                styleSetName = InvalidStyleSetName;
            }
            else if (Validator.IsRequired)
            {
                styleSetName = RequiredStyleSetName;
            }
            else
            {
                styleSetName = "Default";
            }

            baseCtl.StyleSetName = styleSetName;
        }

        #endregion

        #region Events

        protected virtual void validator_AfterValidation(object sender, AfterValidationEventArgs args) { }

        protected virtual void validator_OnFormClosing(object sender, CancelEventArgs args) { }


        protected void Validator_IsRequiredChanged(object sender, EventArgs e) =>
            SetStyleSetName(_passedLastValidation);

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            if(_validator != null)
            {
                _validator.AfterValidation -= validator_AfterValidation;
                _validator.OnFormClosing -= validator_OnFormClosing;
                _validator.Dispose();
            }

            _validator = null;
        }

        #endregion
    }
}