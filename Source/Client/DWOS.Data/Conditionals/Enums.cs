namespace DWOS.Data.Conditionals
{
    /// <summary>
    /// Represents an equality operator.
    /// </summary>
    public enum EqualityOperator
    {
        None,
        GreaterThan,
        LessThan,
        Equal,
        NotEqual
    }

    /// <summary>
    /// Represents a type of condition.
    /// </summary>
    public enum ConditionInputType
    {
        None,

        /// <summary>
        /// Check the answer for another question.
        /// </summary>
        ProcessQuestion,

        /// <summary>
        /// Check the part's part marking fields.
        /// </summary>
        PartTag
    }
}
