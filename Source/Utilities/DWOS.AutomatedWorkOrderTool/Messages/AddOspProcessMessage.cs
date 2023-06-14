using System;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.Messages
{
    public class AddOspProcessMessage : MessageBase
    {
        #region Properties

        public OspProcessViewModel NewProcess { get; }

        #endregion

        #region Methods

        public AddOspProcessMessage(OspProcessViewModel newProcess)
        {
            NewProcess = newProcess ?? throw new ArgumentNullException(nameof(newProcess));
        }

        #endregion
    }
}
