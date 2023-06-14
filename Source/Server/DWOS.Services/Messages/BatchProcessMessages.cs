using System;
using System.Collections.Generic;

namespace DWOS.Services.Messages
{
    /// <summary>
    /// Server response containing a batch's processes.
    /// </summary>
    public class BatchProcessesResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the batch processes for this instance.
        /// </summary>
        public List<BatchProcessInfo> BatchProcesses { get; set; }

        /// <summary>
        /// Gets or sets the batch for this instance.
        /// </summary>
        public BatchStatusInfo BatchStatus { get; set; }
    }

    /// <summary>
    /// Server response containing a batch process ID.
    /// </summary>
    public class BatchCurrentProcessResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the batch process id for this instance.
        /// </summary>
        public int BatchProcessId { get; set; }

        /// <summary>
        /// Gets or sets the process for this instance.
        /// </summary>
        public ProcessInfo Process { get; set; }

        /// <summary>
        /// Gets or sets the order process ID for this instance.
        /// </summary>
        public int OrderProcessId { get; set; }

        /// <summary>
        /// Gets or sets the process alias for this instance.
        /// </summary>
        public ProcessAliasInfo ProcessAlias { get; set; }
    }

    /// <summary>
    /// Represents a batch process.
    /// </summary>
    public class BatchProcessInfo
    {
        /// <summary>
        /// Gets or sets the process ID for this instance.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the step order for this instance.
        /// </summary>
        public int StepOrder { get; set; }

        /// <summary>
        /// Gets or sets the process name for this instance.
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// Gets or sets the process alias names for this instance.
        /// </summary>
        public List<string> ProcessAliasNames { get; set; }

        /// <summary>
        /// Gets or sets the department for this instance.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets the process's start date for this instance.
        /// </summary>
        /// <value>
        /// The process's start date if it has been started;
        /// otherwise, <see cref="DateTime.MinValue"/>.
        /// </value>
        public DateTime Started { get; set; }

        /// <summary>
        /// Gets or sets the process's end date for this instance.
        /// </summary>
        /// <value>
        /// The process's end date if there is one;
        /// otherwise, <see cref="DateTime.MinValue"/>.
        /// </value>
        public DateTime Ended { get; set; }

        /// <summary>
        /// Gets or sets the batch process ID for this instance.
        /// </summary>
        public int BatchProcessId { get; set; }

        /// <summary>
        /// Gets or sets the batch ID for this instance.
        /// </summary>
        public int BatchId { get; set; }
    }

    /// <summary>
    /// Represents batch summary information
    /// </summary>
    public class BatchStatusInfo
    {
        /// <summary>
        /// Gets or sets the batch ID for this instance.
        /// </summary>
        public int BatchId { get; set; }

        /// <summary>
        /// Gets or sets the location for this instance.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the current line for this instance.
        /// </summary>
        public int? CurrentLine { get; set; }

        /// <summary>
        /// Gets or sets the work status for this instance.
        /// </summary>
        public string WorkStatus { get; set; }

        /// <summary>
        /// Gets or sets the next department for this instance.
        /// </summary>
        public string NextDepartment { get; set; }
    }
}
