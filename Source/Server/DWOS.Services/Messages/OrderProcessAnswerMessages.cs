using System;
using System.Collections.Generic;

namespace DWOS.Services.Messages
{
    /// <summary>
    /// Client request for process answers.
    /// </summary>
    public class OrderProcessAnswerRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the order ID for this instance.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the order process ID for this instance.
        /// </summary>
        public int OrderProcessId { get; set; }
    }

    /// <summary>
    /// Server response containing order process information.
    /// </summary>
    public class OrderProcessAnswerResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the answers for this instance.
        /// </summary>
        public List<OrderProcessAnswerInfo> OrderProcessAnswers { get; set; }
    }

    /// <summary>
    /// Client request to save process answers.
    /// </summary>
    public class OrderProcessAnswerSaveRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the order ID for this instance.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the answers to save.
        /// </summary>
        public List<OrderProcessAnswerInfo> OrderProcessAnswers { get; set; }

        /// <summary>
        /// Gets or sets the current part quantity for this instance.
        /// </summary>
        public int CurrentProcessedPartQty { get; set; }

        /// <summary>
        /// Gets or sets the order process ID for this instance.
        /// </summary>
        public int OrderProcessId { get; set; }
    }

    /// <summary>
    /// Server response for requests to save process answers.
    /// </summary>
    public class OrderProcessAnswerSaveResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether this order's next process
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

    /// <summary>
    /// Represents an order process answer.
    /// </summary>
    public class OrderProcessAnswerInfo
    {
        /// <summary>
        /// Gets or sets the answer ID for this instance.
        /// </summary>
        public int OrderProcessAnswerId { get; set; }

        /// <summary>
        /// Gets or sets the order ID for this instance.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the question ID for this instance.
        /// </summary>
        public int ProcessQuestionId { get; set; }

        /// <summary>
        /// Gets or sets thr order process ID for this instance.
        /// </summary>
        public int OrderProcessId { get; set; }

        /// <summary>
        /// Gets or sets the answer for this instance.
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the answer is complete.
        /// </summary>
        /// <value>
        /// <c>true</c> if the answer has been completed;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool Completed { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who completed this answer.
        /// </summary>
        /// <value>
        /// The ID of the user who completed this answer if it has been
        /// completed; otherwise, a negative value.
        /// </value>
        public int CompletedBy { get; set; }

        /// <summary>
        /// Gets or sets the completion date of this instance.
        /// </summary>
        /// <value>
        /// The completion date of the answer if it has been completed;
        /// otherwise, <see cref="DateTime.MinValue"/>.
        /// </value>
        public DateTime CompletedDate { get; set; }
    }
}
