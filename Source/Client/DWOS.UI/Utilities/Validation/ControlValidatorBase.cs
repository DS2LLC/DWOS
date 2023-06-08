using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DWOS.Utilities.Validation
{
    public abstract class ControlValidatorBase : IDisposable
    {
        #region Fields

        private bool _isValid = true;
        private bool _isRequired = true;

        public event EventHandler<EventArgs> IsRequiredChanged;
        public event EventHandler<AfterValidationEventArgs> AfterValidation;

        public event EventHandler<CancelEventArgs> OnFormClosing;

        #endregion

        #region Properties

        protected Control Control { get; set; }

        /// <summary>
        /// Gets the control being validated.
        /// </summary>
        /// <value>The validating control.</value>
        public Control ValidatingControl => Control;

        /// <summary>
        ///     Gets or sets a value indicating whether field value is required.
        /// </summary>
        /// <value> <c>true</c> if this instance is required; otherwise, <c>false</c> . </value>
        public bool IsRequired
        {
            get => _isRequired;
            set
            {
                if (_isRequired != value)
                {
                    _isRequired = value;
                    IsRequiredChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Methods

        protected ControlValidatorBase(Control control) : this(control, false) { }

        protected ControlValidatorBase(Control control, bool autoValidate)
        {
            Control = control;

            if(autoValidate)
                control.Validating += ValidateControl;

            Form frm = control.FindForm();

            if(frm != null)
                frm.Closing += frm_Closing;
        }

        protected void FireAfterValidation(bool passed, string message)
        {
            _isValid = passed;

            //if validation failed then set focus
            if(!_isValid)
                Control?.Focus();

            AfterValidation?.Invoke(this, new AfterValidationEventArgs(passed, message));
        }

        #endregion

        #region Properties

        public bool IsValid
        {
            get
            {
                ValidateControl(Control, new CancelEventArgs());
                return _isValid;
            }
        }

        public string ErrorMessage { get; set; } = "Value is required.";

        #endregion

        #region Events

        private void frm_Closing(object sender, CancelEventArgs e)
        {
            OnFormClosing?.Invoke(this, e);
        }

        internal abstract void ValidateControl(object sender, CancelEventArgs e);

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            var frm = Control?.FindForm();

            if(frm != null)
                frm.Closing -= frm_Closing;

            Control = null;
        }

        #endregion
    }
}