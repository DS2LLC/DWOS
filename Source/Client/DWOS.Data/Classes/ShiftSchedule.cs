using DWOS.Data.Datasets;
using System;
using System.Collections.Generic;

namespace DWOS.Data
{
    /// <summary>
    /// Schedule for a departments specific shift.
    /// </summary>
    public class ShiftSchedule
    {
        #region Properties
        
        /// <summary>
        ///   Gets or sets the generation that this task occurs on.
        /// </summary>
        /// <value> The generation. </value>
        public int Generation { get; set; }

        /// <summary>
        /// Gets or sets the shift, this is specific to the day it occurs on.
        /// </summary>
        /// <value>The shift.</value>
        public int Shift { get; set; }

        /// <summary>
        ///   Gets or sets the department.
        /// </summary>
        /// <value> The department. </value>
        public string Department { get; set; }

        /// <summary>
        ///   Gets or sets the total part count.
        /// </summary>
        /// <value> The part count. </value>
        public int PartCount { get; set; }

        /// <summary>
        ///   Gets or sets the orders to be completed in this task.
        /// </summary>
        /// <value> The orders. </value>
        public List<ScheduleDataset.OrderScheduleRow> Orders { get; set; }

        /// <summary>
        /// Gets or sets the start date/time for this instance.
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the duration for the shift.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets the end date/time for this instance.
        /// </summary>
        public DateTime EndDateTimeResolved
        {
            get { return StartDateTime.Add(Duration); }
        }

        /// <summary>
        /// Gets or sets the maximum production count for this department shift.
        /// </summary>
        /// <value>The maximum production count.</value>
        public int MaxProductionCount { get; set; }

        #endregion
    }


}
