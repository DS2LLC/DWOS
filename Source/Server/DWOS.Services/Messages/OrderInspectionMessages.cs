using System;
using System.Collections.Generic;

namespace DWOS.Services.Messages
{
    /// <summary>
    /// Server response containing an order's inspection.
    /// </summary>
    public class InspectionResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the inspection for this instance.
        /// </summary>
        public InspectionInfo Inspection { get; set; }
    }

    /// <summary>
    /// Represents a inspection type.
    /// </summary>
    public class InspectionInfo
    {
        /// <summary>
        /// Gets or sets the part inspection type ID for this instance.
        /// </summary>
        public int InspectionId { get; set; }

        /// <summary>
        /// Gets or sets the name for this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the reference for this instance.
        /// </summary>
        public string TestReference { get; set; }

        /// <summary>
        /// Gets or sets the requirements for this instance.
        /// </summary>
        public string TestRequirements { get; set; }

        /// <summary>
        /// Gets or sets the questions for this instance.
        /// </summary>
        public List<InspectionQuestionInfo> InspectionQuestions { get; set; }

        /// <summary>
        /// Gets or sets a collection containing all lists used by questions
        /// for this instance.
        /// </summary>
        public List<ListInfo> Lists { get; set; }

        /// <summary>
        /// Gets or sets the documents for this instance.
        /// </summary>
        public List<DocumentInfo> Documents { get; set; }
    }

    /// <summary>
    /// Represents a question in a inspection.
    /// </summary>
    public class InspectionQuestionInfo
    {
        /// <summary>
        /// Gets or sets the inspection for this instance.
        /// </summary>
        public int InspectionId { get; set; }

        /// <summary>
        /// Gets or sets the part inspection question ID for this instance.
        /// </summary>
        public int InspectionQuestionID { get; set; }

        /// <summary>
        /// Gets or sets the name for this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the step order for this instance.
        /// </summary>
        public decimal StepOrder { get; set; }

        /// <summary>
        /// Gets or sets the input type for this instance.
        /// </summary>
        public string InputType { get; set; }

        /// <summary>
        /// Gets or sets the minimum value for this instance.
        /// </summary>
        public string MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value for this instance.
        /// </summary>
        public string MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the list ID for this instance.
        /// </summary>
        public int ListID { get; set; }

        /// <summary>
        /// Gets or sets the default value for this instance.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the numeric units for this instance.
        /// </summary>
        public string NumericUnits { get; set; }

        /// <summary>
        /// Gets or sets a the list of conditions that belong to this instance.
        /// </summary>
        /// <remarks>
        /// These are 'main' conditions for the question and you need to check
        /// a different question to see if the condition passes.
        /// </remarks>
        public List<InspectionQuestionCondition> Conditions { get; set; }
    }

    public class InspectionQuestionCondition
    {
        /// <summary>
        /// Gets or sets the ID of the question to check.
        /// </summary>
        public int CheckInspectionQuestionId { get; set; }

        /// <summary>
        /// Gets or sets the operator for this instance.
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Gets or sets the value for this instance.
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Client request to save an order inspection.
    /// </summary>
    public class InspectionSaveAnswerRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the order inspection to save.
        /// </summary>
        public OrderInspectionInfo OrderInspection { get; set; }
    }

    /// <summary>
    /// Server response to a request for saving an order inspection.
    /// </summary>
    public class InspectionSaveAnswerResponse : ResponseBase
    {
    }

    /// <summary>
    /// Represents an order inspection.
    /// </summary>
    public class OrderInspectionInfo
    {
        /// <summary>
        /// Gets or sets the order inspection ID for this instance.
        /// </summary>
        /// <remarks>
        /// This value is not required.
        /// </remarks>
        /// <value>The order inspection identifier.</value>
        public int OrderInspectionId { get; set; }

        /// <summary>
        /// Gets or inspection ID for this instance.
        /// </summary>
        public int InspectionId { get; set; }

        /// <summary>
        /// Gets or sets the order ID for this instance.
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the pass/fail status of
        /// this instance.
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Gets or sets the total part quantity for this instance.
        /// </summary>
        public int PartQuantity { get; set; }

        /// <summary>
        /// Gets or sets the inspector ID for this instance.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the inspection date for this instance.
        /// </summary>
        public DateTime InspectionDate { get; set; }

        /// <summary>
        /// Gets or sets the notes for this instance.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the accepted part quantity for this instance.
        /// </summary>
        public int AcceptedQty { get; set; }

        /// <summary>
        /// Gets or sets the rejected part quantity for this instance.
        /// </summary>
        public int RejectedQty { get; set; }

        /// <summary>
        /// Gets or sets the answers for this instance.
        /// </summary>
        public List<OrderInspectionAnswerInfo> InspectionAnswers { get; set; }
    }

    /// <summary>
    /// Represents an answer for an order inspection.
    /// </summary>
    public class OrderInspectionAnswerInfo
    {
        /// <summary>
        /// Gets or sets the order inspection answer ID for this instance.
        /// </summary>
        /// <remarks>
        /// This value is not required.
        /// </remarks>
        /// <value>The order inspection answer identifier.</value>
        public int OrderInspectionAnswerId { get; set; }

        /// <summary>
        /// Gets or sets the order inspection ID for this instance.
        /// </summary>
        /// <remarks>
        /// This value is not required.
        /// </remarks>
        /// <value>The order inspection identifier.</value>
        public int OrderInspectionId { get; set; }

        /// <summary>
        /// Gets or sets the question ID for this instance.
        /// </summary>
        /// <value>The inspection question identifier.</value>
        public int InspectionQuestionID { get; set; }

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
