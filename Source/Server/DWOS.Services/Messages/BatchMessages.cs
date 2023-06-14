using System;
using System.Collections.Generic;

namespace DWOS.Services.Messages
{
    #region Batch Info

    /// <summary>
    /// Server response containing all batches.
    /// </summary>
    public class BatchesResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the batches for this instance.
        /// </summary>
        public List<BatchInfo> Batches { get; set; }
    }

    /// <summary>
    /// Contains batch summary information.
    /// </summary>
    public class BatchInfo
    {
        /// <summary>
        /// Gets or sets the batch ID for this instance.
        /// </summary>
        public int BatchId { get; set; }

        /// <summary>
        /// Gets or sets the fixture for this instance.
        /// </summary>
        public string Fixture { get; set; }

        /// <summary>
        /// Gets or sets the work status for this instance.
        /// </summary>
        public string WorkStatus { get; set; }

        /// <summary>
        /// Gets or sets the location for this instance.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the current line for this instance.
        /// </summary>
        public string CurrentLine { get; set; }

        /// <summary>
        /// Gets or sets the schedule priority for this instance.
        /// </summary>
        public int SchedulePriority { get; set; }
    }

    /// <summary>
    /// Client request to check a batch into a department.
    /// </summary>
     public class BatchCheckInRequest : RequestBase
     {
        /// <summary>
        /// Gets or sets the batch ID for this instance.
        /// </summary>
         public int BatchId { get; set; }

        /// <summary>
        /// Gets or sets the batch's next department.
        /// </summary>
         public string NextDepartment { get; set; }
     }

    #endregion

    #region Batch Detail

    /// <summary>
    /// Server response for detailed batch information.
    /// </summary>
     public class BatchDetailResponse : ResponseBase
     {
        /// <summary>
        /// Gets or sets the details for this instance.
        /// </summary>
         public BatchDetailInfo BatchDetail { get; set; }
     }

    /// <summary>
    /// Contains detailed batch information.
    /// </summary>
     public class BatchDetailInfo
     {
        /// <summary>
        /// Gets or sets the batch ID for this instance.
        /// </summary>
        public int BatchId { get; set; }

        /// <summary>
        /// Gets or sets the work status for this instance.
        /// </summary>
        public string WorkStatus { get; set; }

        /// <summary>
        /// Gets or sets the location for this instance.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the current line for this instance.
        /// </summary>
        public string CurrentLine { get; set; }

        /// <summary>
        /// Gets or sets the fixture for this instance.
        /// </summary>
        public string Fixture { get; set; }

        /// <summary>
        /// Gets or sets the open date for this instance.
        /// </summary>
        public DateTime OpenDate { get; set; }

        /// <summary>
        /// Gets or sets the next department for this instance.
        /// </summary>
        public string NextDept { get; set; }

        /// <summary>
        /// Gets or sets the next department for this instance.
        /// </summary>
        public string CurrentProcess { get; set; }

        /// <summary>
        /// Gets or sets the total part count for this instance.
        /// </summary>
        public int PartCount { get; set; }

        /// <summary>
        /// Gets or sets the number of orders for this instance.
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// Gets or sets the total surface for this instance.
        /// </summary>
        public double TotalSurfaceArea { get; set; }

        /// <summary>
        /// Gets or sets the total weight for this instance.
        /// </summary>
        public decimal TotalWeight { get; set; }

        /// <summary>
        /// Gets or sets a list of orders in the batch.
        /// </summary>
        public List<int> Orders { get; set; }

        /// <summary>
        /// Gets or sets the schedule priority for this instance.
        /// </summary>
        public int SchedulePriority { get; set; }

        /// <summary>
        /// Gets or sets the Sales Order ID for this instance.
        /// </summary>
        public int? SalesOrderId { get; set; }
    }

    #endregion

    #region BatchSchedule

    /// <summary>
    /// Server response containing batch schedule data.
    /// </summary>
    public class BatchScheduleResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the schedule for this instance.
        /// </summary>
        public BatchSchedule Schedule { get; set; }
    }

    /// <summary>
    ///  Represents a schedule of batches.
    /// </summary>
    public class BatchSchedule
    {
        /// <summary>
        /// Gets or sets the list of batch IDs for this instance.
        /// </summary>
        public List<int> BatchIds { get; set; }
    }

    #endregion
}
