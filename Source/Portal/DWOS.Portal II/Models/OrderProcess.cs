using System;

namespace DWOS.Portal.Models
{
    /// <summary>
    /// Represents a process of a work order.
    /// </summary>
    public class OrderProcess
    {
        #region Properties

        /// <summary>
        /// Gets or sets the step order.
        /// </summary>
        /// <value>
        /// The step order.
        /// </value>
        public int StepOrder { get; set; }

        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        /// <value>
        /// The department.
        /// </value>
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        #endregion
    }
}