using System;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.Messages
{
    public class AddOspFormatSectionMessage : MessageBase
    {
        #region Properties

        public OspFormatSectionViewModel NewSection { get; }

        #endregion

        #region Methods

        public AddOspFormatSectionMessage(OspFormatSectionViewModel newSection)
        {
            NewSection = newSection ?? throw new ArgumentNullException(nameof(newSection));
        }

        #endregion
    }
}
