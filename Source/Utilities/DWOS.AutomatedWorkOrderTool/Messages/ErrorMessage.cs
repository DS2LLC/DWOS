using System;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.Messages
{
    public class ErrorMessage : MessageBase
    {
        #region Properties

        public Exception Exception { get; }

        public string Message { get; }

        #endregion

        #region Methods

        public ErrorMessage(Exception exc, string message)
        {
            Message = message;
            Exception = exc ?? throw new ArgumentNullException(nameof(exc));
        }

        #endregion
    }
}
