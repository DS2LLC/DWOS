using GalaSoft.MvvmLight.Messaging;

namespace DWOS.DataArchiver.Messages
{
    public class MoveToStepMessage : MessageBase
    {
        #region Properties

        public Step Step { get; }

        #endregion

        #region Methods

        public MoveToStepMessage(Step step)
        {
            Step = step;
        }

        #endregion
    }
}
