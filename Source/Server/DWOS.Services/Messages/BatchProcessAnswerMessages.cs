using System.Collections.Generic;

namespace DWOS.Services.Messages
{
    /// <summary>
    /// Client request for batch process answers.
    /// </summary>
    public class BatchProcessAnswerRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the batch ID for this instance.
        /// </summary>
        public int BatchId { get; set; }

        /// <summary>
        /// Gets or sets the batch process ID for this instance.
        /// </summary>
        public int BatchProcessId { get; set; }
    }

    /// <summary>
    /// Server response with batch processes answers.
    /// </summary>
    public class BatchProcessAnswerResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the batch process answers for this instance.
        /// </summary>
        public List<OrderProcessAnswerInfo> BatchProcessAnswers { get; set; }
    }

    /// <summary>
    /// Client request to save batch process answers.
    /// </summary>
    public class BatchProcessAnswerSaveRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the batch ID for this instance.
        /// </summary>
        public int BatchId { get; set; }

        /// <summary>
        /// Gets or sets the order process answers to save.
        /// </summary>
        public List<OrderProcessAnswerInfo> OrderProcessAnswers { get; set; }
    }

    /// <summary>
    /// Server response for a request to save batch process answers.
    /// </summary>
    public class BatchProcessAnswerSaveResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether the batch's next process
        /// has a time constraint.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has next process time constraint;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool HasNextProcessTimeConstraint { get; set; }

        /// <summary>
        /// Gets or sets the message to show when there is a time constraint
        /// for the batch's next process.
        /// </summary>
        public string NextProcessTimeConstraintMessage { get; set; }
    }
}
