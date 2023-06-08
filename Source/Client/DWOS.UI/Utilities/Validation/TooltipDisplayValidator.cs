using System.ComponentModel;
using System.Drawing;
using Infragistics.Win;
using Infragistics.Win.UltraWinToolTip;

namespace DWOS.Utilities.Validation
{
    public class TooltipDisplayValidator : DisplayValidator
    {
        #region Fields

        private UltraToolTipInfo _errorTipInfo;
        private UltraToolTipInfo _origTipInfo;

        #endregion

        #region Properties

        public UltraToolTipManager ToolTipManager { get; set; }

        #endregion

        #region Methods

        public TooltipDisplayValidator(UltraToolTipManager toolTipManager, ControlValidatorBase validator) : base(validator)
        {
            ToolTipManager = toolTipManager;
        }

        #endregion

        #region Events

        protected override void validator_AfterValidation(object sender, AfterValidationEventArgs args)
        {
            SetStyleSetName(args.Passed);

            if(!args.Passed)
            {
                var ctl = Validator.ValidatingControl;

                if(_origTipInfo == null)
                    _origTipInfo = ToolTipManager.GetUltraToolTip(ctl);

                if(_errorTipInfo == null)
                    _errorTipInfo = new UltraToolTipInfo(Validator.ErrorMessage, ToolTipImage.Error, "Error", DefaultableBoolean.True);

                ToolTipManager.SetUltraToolTip(ctl, _errorTipInfo);
                ToolTipManager.ShowToolTip(ctl, ctl.PointToScreen(new Point(ctl.Width, ctl.Height / 2)));
            }
            else if(_origTipInfo != null)
            {
                ToolTipManager.SetUltraToolTip(Validator.ValidatingControl, _origTipInfo);
            }
            else
            {
                ToolTipManager.SetUltraToolTip(Validator.ValidatingControl, null);
            }
        }

        protected override void validator_OnFormClosing(object sender, CancelEventArgs args)
        {
            //if canceling then don't hide
            if (!args.Cancel)
                ToolTipManager.HideToolTip();
        }

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            ToolTipManager = null;
            _origTipInfo = null;
            _errorTipInfo = null;

            base.Dispose();
        }

        #endregion
    }
}