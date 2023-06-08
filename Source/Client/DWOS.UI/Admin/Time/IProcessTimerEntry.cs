using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.UI.Admin.Time
{
    public interface IProcessTimerEntry
    {
        #region Properties

        string Id { get; }

        string WorkStatus { get; }

        int DurationMinutes { get; }

        #endregion

        #region Methods

        void StopTimer();

        #endregion
    }
}
