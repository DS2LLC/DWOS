using System;

namespace DWOS.Data
{
    /// <summary>
    /// Interface for classes implementing lead-time persistence functionality.
    /// </summary>
    public interface ILeadTimePersistence
    {
        /// <summary>
        /// Gets the time of day when receiving rolls over.
        /// </summary>
        TimeSpan ReceivingRolloverTime { get; }

        /// <summary>
        /// Gets the shipping lead time in days.
        /// </summary>
        double ShippingLeadTime { get; }

        /// <summary>
        /// Gets the COC lead time in days.
        /// </summary>
        double CocLeadTime { get; }

        /// <summary>
        /// Gets the part marking lead time in days.
        /// </summary>
        double PartMarkingLeadTime { get; }

        /// <summary>
        /// Gets a value that indicates if the COC workflow is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if the workflow is enabled; otherwise, <c>false</c>.
        /// </value>
        bool CocEnabled { get; }

        /// <summary>
        /// Gets a value that indicates if the part marking workflow is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if the workflow is enabled; otherwise, <c>false</c>.
        /// </value>
        bool PartMarkingEnabled { get; }

        /// <summary>
        /// Retrieves the lead time (in days) for a process.
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        decimal GetLeadTimeDays(int processId);
    }
}