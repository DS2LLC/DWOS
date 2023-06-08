using DWOS.Data.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.UI.Admin.Time
{
    public sealed class BatchProcessTimerEntry : IProcessTimerEntry
    {
        #region Properties

        public int BatchId
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
                return "Batch " + BatchId.ToString();
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
            TimeCollectionUtilities.StopBatchProcessTimer(BatchId);
        }

        #endregion
    }
}
