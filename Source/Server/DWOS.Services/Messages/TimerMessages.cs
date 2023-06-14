using System.Collections.Generic;

namespace DWOS.Services.Messages
{
    /// <summary>
    /// Client-side request for order timer information.
    /// </summary>
    public class OrderTimerRequest : RequestBase
    {
        public int OrderId { get; set; }
    }

    /// <summary>
    /// Client-side request for batch timer information.
    /// </summary>
    public class BatchTimerRequest : RequestBase
    {
        public int BatchId { get; set; }
    }

    /// <summary>
    /// Response to requests for timer (order or batch) information.
    /// </summary>
    public class TimerInfoResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets a value that indicates if the order/batch has an
        /// active process timer.
        /// </summary>
        /// <value>
        /// <c>true</c> if there is a timer; otherwise, <c>false</c>.
        /// </value>
        public bool HasActiveProcessTimer { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the order/batch and user
        /// have an active labor timer.
        /// </summary>
        /// <value>
        /// <c>true</c> if there is a timer; otherwise, <c>false</c>.
        /// </value>
        public bool HasActiveLaborTimer { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the user is an
        /// active operator for this order/batch.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user is an active operator;
        /// otherwise, <c>false</c>
        /// </value>
        public bool IsUserActiveOperator { get; set; }
    }
}
