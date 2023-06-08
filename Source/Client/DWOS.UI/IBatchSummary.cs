using System;

namespace DWOS.UI
{
    /// <summary>
    /// Interface for <see cref="BatchSummary"/> - represents a batch tab.
    /// </summary>
    public interface IBatchSummary : IDwosTab
    {
        /// <summary>
        /// Gets the batch ID for the currently selected batch.
        /// </summary>
        int SelectedBatch { get; }

        /// <summary>
        /// Gets the work status of the currently selected batch.
        /// </summary>
        string SelectedWorkStatus { get; }

        /// <summary>
        /// Gets the location of the currently selected batch.
        /// </summary>
        string SelectedLocation { get; }

        /// <summary>
        /// Gets the line of the currently selected batch.
        /// </summary>
        int? SelectedLine { get; }

        /// <summary>
        /// Gets the number of active timers for the currently selected batch.
        /// </summary>
        int SelectedActiveTimerCount { get; }

        /// <summary>
        /// Occurs when the user selects a row.
        /// </summary>
        event EventHandler AfterSelectedRowChanged;

        /// <summary>
        /// Selects a batch by its batch ID.
        /// </summary>
        /// <param name="batchId"></param>
        void SelectBatch(int batchId);
    }
}