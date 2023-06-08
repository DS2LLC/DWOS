using System.ComponentModel;

namespace DWOS.Data.Datasets
{
    /// <summary>
    /// Represents an input type for a user-created question.
    /// </summary>
    [DefaultValue("None")]
    public enum InputType
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
        DateTimeOut,
        SampleSet
    }

    /// <summary>
    /// Represents a type of order.
    /// </summary>
    [DefaultValue("Normal")]
    public enum OrderType
    {
        /// <summary>
        /// Normal work order
        /// </summary>
        Normal = 1, 

        /// <summary>
        /// An order that is an external rework
        /// </summary>
        ReworkExt = 3,

        /// <summary>
        /// An order created through split/hold rework - needs to be joined after processing.
        /// </summary>
        ReworkInt = 4,

        /// <summary>
        /// An order that is on hold waiting for internal rework to complete
        /// </summary>
        ReworkHold = 5,

        /// <summary>
        /// An order that is lost
        /// </summary>
        Lost = 6,

        /// <summary>
        /// An order that is in quarantine
        /// </summary>
        Quarantine = 7
    }

    /// <summary>
    /// Represents a type of order process.
    /// </summary>
    [DefaultValue("Normal")]
    public enum OrderProcessType
    {
        Normal = 1,
        Rework = 2
    }

    /// <summary>
    /// Represents a type of order change.
    /// </summary>
    public enum OrderChangeType
    {
        /// <summary>
        /// External Rework
        /// </summary>
        ExtRework = 1,

        Split = 2,

        Rejoin = 3
    }

    /// <summary>
    /// Represents a type of rework
    /// </summary>
    public enum ReworkType
    {
        None,
        Full,
        Split,
        SplitHold,
        Quarantine,
        Lost
    }

    /// <summary>
    /// Represents a type of rework category
    /// </summary>
    public enum ReworkCategoryType
    {
        Rework,
        Quarantine,
        Lost
    }
}
