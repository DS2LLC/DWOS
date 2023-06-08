using System.Collections.Generic;

namespace DWOS.UI.Admin.Time
{
    /// <summary>
    /// Interface for <see cref="TimeManager"/>.
    /// </summary>
    interface ITimeManagerView
    {
        /// <summary>
        /// Shows a 'move orders' dialog.
        /// </summary>
        /// <param name="entriesToMove">Initial collection of entries to move.</param>
        void MoveOrdersDialog(IEnumerable<IOperatorEntry> entriesToMove);

        /// <summary>
        /// Shows a 'manage processing timers' dialog.
        /// </summary>
        void ManageProcessingTimeDialog();
    }
}
