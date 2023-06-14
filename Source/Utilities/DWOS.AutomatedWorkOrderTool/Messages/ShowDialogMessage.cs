using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.Messages
{
    public class ShowDialogMessage : MessageBase
    {
        #region Properties

        public DialogType Type { get; }

        #endregion

        #region Methods

        public ShowDialogMessage(DialogType type)
        {
            Type = type;
        }

        #endregion

        #region DialogType

        public enum DialogType
        {
            AddCustomer,
            AddManufacturer,
            ImportMasterList,
            ImportShippingManifest
        }

        #endregion
    }
}
