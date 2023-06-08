using System;

namespace DWOS.Utilities.Validation
{
    public class AfterValidationEventArgs : EventArgs
    {
        #region Properties

        public bool Passed { get; }

        public string Message { get; }

        #endregion

        #region Methods

        public AfterValidationEventArgs(bool passed, string message)
        {
            Passed = passed;
            Message = message;
        }

        #endregion
    }
}
