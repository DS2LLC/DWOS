using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTabControl;

namespace DWOS.Utilities.Validation
{
    public class ImageDisplayValidator : DisplayValidator
    {
        #region Properties

        public ErrorProvider ErrorProvider { get; set; }

        /// <summary>
        ///     Gets or sets the tab that this control is on for validation.
        /// </summary>
        /// <value>The tab.</value>
        public UltraTab Tab { get; set; }

        #endregion

        #region Methods

        public ImageDisplayValidator() { }

        public ImageDisplayValidator(ControlValidatorBase validator) : base(validator)
        {
        }

        public ImageDisplayValidator(ControlValidatorBase validator, ErrorProvider ep) : base(validator)
        {
            ErrorProvider = ep;
        }

        #endregion

        #region Events

        protected override void validator_AfterValidation(object sender, AfterValidationEventArgs args)
        {
            SetStyleSetName(args.Passed);

            if (args.Passed)
            {
                ErrorProvider?.SetError(Validator.ValidatingControl, string.Empty);
                Tab?.Appearance.ResetForeColor();
            }
            else
            {
                if(ErrorProvider == null)
                    ErrorProvider = new ErrorProvider();

                ErrorProvider.SetError(Validator.ValidatingControl, args.Message);

                if(Tab != null)
                {
                    Tab.Selected = true;
                    Tab.Appearance.ForeColor = Color.Red;
                    Tab.Appearance.ResetForeColor();
                }
            }
        }

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            ErrorProvider?.Dispose();

            ErrorProvider = null;

            base.Dispose();
        }

        #endregion
    }
}