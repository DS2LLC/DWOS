using System;

namespace DWOS.Data.Date
{
    /// <summary>
    /// Provides the current date and time.
    /// </summary>
    /// <remarks>
    /// Using this instance (and the main implementation,
    /// <see cref="DateTimeNowProvider"/>) instead of <see cref="DateTime.Now"/>
    /// makes it possible to write reliable unit tests for functionality that
    /// requires using the current date.
    /// </remarks>
    public interface IDateTimeNowProvider
    {
        /// <summary>
        /// Gets the current date and time.
        /// </summary>
        DateTime Now { get; }
    }
}
