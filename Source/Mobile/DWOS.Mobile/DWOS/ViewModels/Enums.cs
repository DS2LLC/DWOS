namespace DWOS.ViewModels
{
    /// <summary>
    /// Input Types available for questions and answers
    /// </summary>
    public enum InputTypes
    {
        None,
        Date,
        Decimal,
        Integer,
        List,
        String,
        Time,
        PartQty,
        TimeIn,
        TimeOut,
        TimeDuration,
        RampTime,
        PreProcessWeight,
        PostProcessWeight,
        DecimalBefore,
        DecimalAfter,
        DecimalDifference,
        DateTimeIn,
        DateTimeOut
    }

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

    /// <summary>
    /// Represents the status of a step.
    /// </summary>
    public enum StepStatus
    {
        Completed,
        Incomplete,
        InProgress,
        Skipped
    };

    /// <summary>
    /// Represents the status of a process.
    /// </summary>
    public enum ProcessStatus
    {
        Completed,
        Incomplete,
        InProgress
    };

    /// <summary>
    /// Represents a type of order filter.
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// Normal Filter
        /// </summary>
        Normal,

        /// <summary>
        /// Filter by Schedule Priority
        /// </summary>
        SchedulePriority
    }
}
