using System;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.Messages
{
    public class AddOspPartMarkMessage : MessageBase
    {
        #region Properties

        public OspPartMarkViewModel NewPartMark { get; }

        #endregion

        #region Methods

        public AddOspPartMarkMessage(OspPartMarkViewModel newPartMark)
        {
            NewPartMark = newPartMark ?? throw new ArgumentNullException(nameof(newPartMark));
        }

        #endregion
    }
}
