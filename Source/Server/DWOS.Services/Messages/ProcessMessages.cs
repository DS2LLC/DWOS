using System.Collections.Generic;

namespace DWOS.Services.Messages
{
    #region Order Info

    /// <summary>
    /// Server response with process information.
    /// </summary>
    public class ProcessResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the process information for this instance.
        /// </summary>
        public ProcessInfo Process { get; set; }
    }

    /// <summary>
    /// Represents a process.
    /// </summary>
    public class ProcessInfo
    {
        /// <summary>
        /// Gets or sets the process ID for this instance.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the description for this instance.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the name for this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the department for this instance.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets the revision for this instance.
        /// </summary>
        public string Revision { get; set; }

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
    /// Represents a step in a process that contains questions.
    /// </summary>
    public class ProcessStepInfo
    {
        /// <summary>
        /// Gets or sets the process step ID for this instance.
        /// </summary>
        public int ProcessStepId { get; set; }

        /// <summary>
        /// Gets or sets the process ID for this instance.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the name for this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description for this instance.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the step order for this instance.
        /// </summary>
        public decimal StepOrder { get; set; }

        /// <summary>
        /// Gets or sets the process questions for this instance.
        /// </summary>
        public List<ProcessQuestionInfo> ProcessQuestions { get; set; }

        /// <summary>
        /// Gets or sets the documents for this instance.
        /// </summary>
        public List<DocumentInfo> Documents { get; set; }

        /// <summary>
        /// Gets or sets the conditions for this instance.
        /// </summary>
        public List<ProcessStepCondition> Conditions { get; set; }
    }

    /// <summary>
    /// Represents a process alias.
    /// </summary>
    public class ProcessAliasInfo
    {
        /// <summary>
        /// Gets or sets the process ID for this instance.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the process alias ID for this instance.
        /// </summary>
        public int ProcessAliasId { get; set; }

        /// <summary>
        /// Gets or sets the documents for this instance.
        /// </summary>
        public List<DocumentInfo> Documents { get; set; }
    }

    /// <summary>
    /// Represents a question in a processes step.
    /// </summary>
    public class ProcessQuestionInfo
    {
        /// <summary>
        /// Gets or sets the process question ID for this instance.
        /// </summary>
        public int ProcessQuestionId { get; set; }

        /// <summary>
        /// Gets or sets the process step ID for this instance.
        /// </summary>
        public int ProcessStepId { get; set; }

        /// <summary>
        /// Gets or sets the name for this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the step order for this instance.
        /// </summary>
        public decimal StepOrder { get; set; }

        /// <summary>
        /// Get or sets the input type for this instance.
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
        /// Gets or sets a value indicating if this instance is
        /// editable by operators.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is editable; otherwise< <c>false</c>.
        /// </value>
        public bool OperatorEditable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if this instance is reequired.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is required; otherwise< <c>false</c>.
        /// </value>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the notes for this instance.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the numeric units for this instance.
        /// </summary>
        public string NumericUnits { get; set; }
    }

    /// <summary>
    /// A list with values
    /// </summary>
    public class ListInfo
    {
        /// <summary>
        /// Gets or sets the list ID for this instance.
        /// </summary>
        public int ListId { get; set; }

        /// <summary>
        /// Gets or set the list values for this instance.
        /// </summary>
        public List<string> Values { get; set; }
    }

    /// <summary>
    /// Represents a process step condition.
    /// </summary>
    public class ProcessStepCondition
    {
        /// <summary>
        /// Gets or sets the input type for this instance.
        /// </summary>
        public string InputType { get; set; }

        /// <summary>
        /// Gets or sets the condition operator for this instance.
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Gets or sets the target process question ID for this instance.
        /// </summary>
        public int ProcessQuestionId { get; set; }

        /// <summary>
        /// Gets or sets the parent process step ID for this instance.
        /// </summary>
        public int ProcessStepId { get; set; }

        /// <summary>
        /// Gets or sets the condition value for this instance.
        /// </summary>
        public string Value { get; set; }
    }

    #endregion
}
