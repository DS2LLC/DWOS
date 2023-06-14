using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.Messages
{
    public class SuccessMessage : MessageBase
    {
        #region Properties

        public SuccessType Type { get; }

        #endregion

        #region Methods

        public SuccessMessage(SuccessType type)
        {
            Type = type;
        }

        #endregion

        #region SuccessType

        public enum SuccessType
        {
            SaveOspFormat
        }

        #endregion
    }
}
