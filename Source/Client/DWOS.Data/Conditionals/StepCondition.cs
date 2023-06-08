namespace DWOS.Data.Conditionals
{
    /// <summary>
    /// Represents a step condition.
    /// </summary>
    public sealed class StepCondition
    {
        #region Properties

        /// <summary>
        /// Gets or sets the process question ID.
        /// </summary>
        public int ProcessQuestionId { get; set; }

        /// <summary>
        /// Gets or sets the type of condition.
        /// </summary>
        public ConditionInputType InputType { get; set; }

        /// <summary>
        /// Gets or sets the condition operator.
        /// </summary>
        public EqualityOperator Operator { get; set; }

        /// <summary>
        /// Gets or sets the comparison value.
        /// </summary>
        public string Value { get; set; }

        #endregion
    }
}
