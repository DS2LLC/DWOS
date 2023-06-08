using DWOS.Data.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.UI.Admin.Time
{
    public sealed class OrderProcessTimerEntry : IProcessTimerEntry
    {
        #region Properties

        public int OrderId
        {
            get;
            set;
        }

        #endregion

        #region IProcessTimerEntry Members

        public string Id
        {
            get
            {
                return "WO " + OrderId.ToString();
            }
        }

        public string WorkStatus
        {
            get;
            set;
        }

        public int DurationMinutes
        {
            get;
            set;
        }

        public void StopTimer()
        {
            TimeCollectionUtilities.StopOrderProcessTimer(OrderId);
        }

        #endregion
    }
}
