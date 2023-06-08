using System;
using System.Globalization;

namespace DWOS.Shared
{
    /// <summary>
    /// Release date for an assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class AssemblyReleaseDateAttribute : Attribute
    {
        #region Fields

        private const string DATE_FORMAT = "yyyy-MM-dd";

        #endregion

        #region Properties

        /// <summary>
        /// Gets the assembly's release date.
        /// </summary>
        /// <remarks>
        /// If <see cref="Date"/> does not have a value, the string value
        /// is invalid.
        /// </remarks>
        public DateTime? Date
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AssemblyReleaseDateAttribute"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor takes a string instead of a <see cref="DateTime"/>
        /// value as a parameter because attribute arguments must be constants.
        /// </remarks>
        /// <param name="date">Date in yyyy-MM-dd format</param>
        public AssemblyReleaseDateAttribute(string date)
        {
            DateTime parsedDate;

            bool validDate = DateTime.TryParseExact(date,
                DATE_FORMAT,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedDate);

            if (validDate)
            {
                Date = parsedDate;
            }
        }

        #endregion
    }
}
