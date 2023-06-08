using DWOS.Data.Order;
using System;

namespace DWOS.UI.Admin.Time
{
    public sealed class BatchOperatorEntry : IOperatorEntry
    {
        #region Properties

        public int BatchId
        {
            get;
            set;
        }

        #endregion

        #region IOperatorEntry

        public bool HasActiveTimer
        {
            get;
            set;
        }

        public string Id
        {
            get
            {
                return "Batch " + BatchId.ToString();
            }
        }

        public int UserId
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public int DurationMinutes
        {
            get;
            set;
        }

        public void StartLaborTimer()
        {
            TimeCollectionUtilities.StartBatchLaborTimer(BatchId, UserId);
            TimeCollectionUtilities.StartBatchProcessTimer(BatchId);
        }

        public void PauseLaborTimer()
        {
            TimeCollectionUtilities.PauseBatchLaborTimer(BatchId, UserId);
        }

        public void StopLaborTimer()
        {
            TimeCollectionUtilities.StopBatchLaborTimer(BatchId, UserId);
        }

        public void MoveToUser(int newUserId)
        {
            if (UserId == newUserId)
            {
                const string errorMsg = "The new user's ID must be different from the current one.";
                throw new ArgumentException(errorMsg, nameof(newUserId));
            }

            TimeCollectionUtilities.StopBatchLaborTimer(BatchId, UserId);
            TimeCollectionUtilities.StartBatchLaborTimer(BatchId, newUserId);
            TimeCollectionUtilities.StartBatchProcessTimer(BatchId);
        }

        #endregion
    }
}
