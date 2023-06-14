using System;

namespace DWOS.AutomatedWorkOrderTool.Messages
{
    public class ConfirmActionMessage
    {
        #region Properties

        public string Title { get; }

        public string Message { get; }

        public Action OnConfirm { get; }

        #endregion

        #region Methods

        public ConfirmActionMessage(string title, string message, Action onConfirm)
        {
            Title = title;
            Message = message;
            OnConfirm = onConfirm ?? throw new ArgumentNullException(nameof(onConfirm));
        }

        #endregion
    }
}
