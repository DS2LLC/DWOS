using System;
using System.Collections.Generic;

namespace DWOS.Services.Messages
{
    /// <summary>
    /// Server response containing order process information.
    /// </summary>
    public class OrderProcessesResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the order processes for this instance.
        /// </summary>
        public List<OrderProcessInfo> OrderProcesses { get; set; }

        /// <summary>
        /// Gets or sets the order for this instance.
        /// </summary>
        public OrderStatusInfo OrderStatus { get; set; }
    }

    /// <summary>
    /// Represents an order's process.
    /// </summary>
    public class OrderProcessInfo
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
        /// Gets or sets the process alias name for this instance.
        /// </summary>
        public string ProcessAliasName { get; set; }

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
        /// Gets or sets the order process ID for this instance.
        /// </summary>
        public int OrderProcessId { get; set; }

        /// <summary>
        /// Gets or sets the OrderID for this instance.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the fixture count for this instance.
        /// </summary>
        /// <remarks>
        /// This is a load capacity value.
        /// </remarks>
        /// <value>
        /// The fixture count if one is available; otherwise, <c>null</c>.
        /// </value>
        public int? FixtureCount { get; set; }

        /// <summary>
        /// Gets or sets the weight per fixture for this instance.
        /// </summary>
        /// <remarks>
        /// This is a load capacity value.
        /// </remarks>
        /// <value>
        /// The weight per fixture if one is available; otherwise, <c>null</c>.
        /// </value>
        public decimal? WeightPerFixture { get; set; }
    }

    /// <summary>
    /// Represents order summary information.
    /// </summary>
    public class OrderStatusInfo
    {
        /// <summary>
        /// Gets or sets the order ID for this instance.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the current location for this instance.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the current processing line for this instance.
        /// </summary>
        public int? CurrentLine { get; set; }

        /// <summary>
        /// Gets or sets the  current work status for this instance.
        /// </summary>
        public string WorkStatus { get; set; }

        /// <summary>
        /// Gets or sets the next department for this instance.
        /// </summary>
        public string NextDepartment { get; set; }
    }

    /// <summary>
    /// Server response containing order process information.
    /// </summary>
    public class OrderProcessDetailResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the order process info for this instance.
        /// </summary>
        public OrderProcessDetailInfo OrderProcessInfo { get; set; }
    }

    /// <summary>
    /// Represents order process detail information.
    /// </summary>
    public class OrderProcessDetailInfo
    {
        /// <summary>
        /// Gets or sets the order process ID for this instance.
        /// </summary>
        public int OrderProcessId { get; set; }

        /// <summary>
        /// Gets or sets the process ID for this instance.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the name for this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if this instance represents a
        /// paperless process.
        /// </summary>
        /// <value>
        /// <c>true</c> if the process is paperless; otherwise, <c>false</c>.
        /// </value>
        public bool IsPaperless { get; set; }

        /// <summary>
        /// Gets or sets the process steps for this instance.
        /// </summary>
        public List<ProcessStepInfo> ProcessSteps { get; set; }

        /// <summary>
        /// Gets or sets a collection of lists used by every question in
        /// this instance.
        /// </summary>
        public List<ListInfo> Lists { get; set; }

        /// <summary>
        /// Gets or sets the documents for this instance.
        /// </summary>
        public List<DocumentInfo> Documents { get; set; }
    }

    /// <summary>
    /// Server response with information about an order's current process.
    /// </summary>
    public class OrderCurrentProcessResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the process for this instance.
        /// </summary>
        public OrderProcessDetailInfo Process { get; set; }

        /// <summary>
        /// Gets or sets the order process ID for this instance.
        /// </summary>
        public int OrderProcessId { get; set; }

        /// <summary>
        /// Gets or sets the process alias for this instance.
        /// </summary>
        public ProcessAliasInfo ProcessAlias { get; set; }
    }
}
