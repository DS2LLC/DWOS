using DWOS.Data.Order;
using System;

namespace DWOS.UI.Admin.Time
{
    public sealed class OrderOperatorEntry : IOperatorEntry
    {
        #region Properties

        public int OrderId
        {
            get;
            set;
        }

        #endregion

        #region ITimeEntry Members

        public bool HasActiveTimer
        {
            get;
            set;
        }

        public string Id
        {
            get
            {
                return "WO " + OrderId.ToString();
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
            TimeCollectionUtilities.StartOrderLaborTimer(OrderId, UserId);
            TimeCollectionUtilities.StartOrderProcessTimer(OrderId);
        }

        public void PauseLaborTimer()
        {
            TimeCollectionUtilities.PauseOrderLaborTimer(OrderId, UserId);
        }

        public void StopLaborTimer()
        {
            TimeCollectionUtilities.StopOrderLaborTimer(OrderId, UserId);
        }

        public void MoveToUser(int newUserId)
        {
            if (UserId == newUserId)
            {
                const string errorMsg = "The new user's ID must be different from the current one.";
                throw new ArgumentException(errorMsg, nameof(newUserId));
            }

            TimeCollectionUtilities.StopOrderLaborTimer(OrderId, UserId);
            TimeCollectionUtilities.StartOrderLaborTimer(OrderId, newUserId);
            TimeCollectionUtilities.StartOrderProcessTimer(OrderId);
        }

        #endregion
    }
}
