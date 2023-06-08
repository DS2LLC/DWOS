using System;

namespace DWOS.Data.Order.Activity
{
    /// <summary>
    /// Represents a user performing an order activity.
    /// </summary>
    public class ActivityUser
    {
        #region Properties

        /// <summary>
        /// Gets the user ID for this instance.
        /// </summary>
        public int UserId { get; }

        /// <summary>
        /// Gets the department for this instance.
        /// </summary>
        public string CurrentDepartment { get; }

        /// <summary>
        /// Gets the processing line ID for this instance.
        /// </summary>
        public int? CurrentLineId { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityUser"/> class.
        /// </summary>
        /// <remarks>
        /// If <paramref name="currentLineId"/> is a non-positive number,
        /// <see cref="CurrentLineId"/> is initialized to <c>null</c>.
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="currentDepartment"></param>
        /// <param name="currentLineId"></param>
        public ActivityUser(int userId, string currentDepartment, int? currentLineId)
        {
            UserId = userId;
            CurrentDepartment = currentDepartment;

            if (currentLineId > 0)
            {
                CurrentLineId = currentLineId;
            }
        }

        #endregion
    }
}
