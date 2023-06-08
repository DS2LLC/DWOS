using System;
using DWOS.Data.Datasets;

namespace DWOS.UI
{
    /// <summary>
    /// Represents a single order or batch in <see cref="ISchedulingTab"/> instances.
    /// </summary>
    public class ScheduleData
    {
        #region Properties

        /// <summary>
        /// Gets the database ID for this instance.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the current location for this instance.
        /// </summary>
        public string CurrentLocation { get; private set; }

        /// <summary>
        /// Gets the work status for this instance.
        /// </summary>
        public string WorkStatus { get; private set; }

        /// <summary>
        /// Gets the next department for this instance.
        /// </summary>
        public string NextDept { get; private set; }

        /// <summary>
        /// Gets the schedule priority for this instance.
        /// </summary>
        public int SchedulePriority { get; private set; }

        /// <summary>
        /// Gets the customer's name for this instance.
        /// </summary>
        public string Customer { get; private set; }

        /// <summary>
        /// Gets the order priority for this instance.
        /// </summary>
        public string Priority { get; private set; }

        /// <summary>
        /// Gets the part name for this instance.
        /// </summary>
        public string Part { get; private set; }

        /// <summary>
        /// Gets the part quantity for this instance.
        /// </summary>
        public int? Quantity { get; private set; }

        /// <summary>
        /// Gets the surface area for this instance.
        /// </summary>
        public double? SurfaceArea { get; private set; }

        /// <summary>
        /// Gets the required date for this instance.
        /// </summary>
        public DateTime? RequiredDate { get; private set; }

        /// <summary>
        /// Gets the current line for this instance.
        /// </summary>
        public string CurrentLine { get; private set; }

        /// <summary>
        /// Gets the type for this instance.
        /// </summary>
        public ScheduleDataType Type { get; private set; }

        public string Process { get; private set; }
        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="ScheduleData"/> instance using a database row.
        /// </summary>
        /// <param name="dbRow">The database row.</param>
        /// <returns>
        /// A new instance of the <see cref="ScheduleData"/> class.
        /// </returns>
        /// <remarks>
        /// If the row's table contains an additional column named
        /// CurrentLineString, this method uses that column's value.
        /// </remarks>
        public static ScheduleData From(OrderStatusDataSet.OrderStatusRow dbRow)
        {
            if (dbRow == null)
            {
                return null;
            }

            return new ScheduleData
            {
                Id = dbRow.WO,
                Type = ScheduleDataType.Order,
                WorkStatus = dbRow.WorkStatus,
                CurrentLocation = dbRow.CurrentLocation,
                NextDept = dbRow.IsNextDeptNull() ? null : dbRow.NextDept,
                SchedulePriority = dbRow.SchedulePriority,
                Customer = dbRow.IsCustomerNull() ? null : dbRow.Customer,
                Priority = dbRow.IsPriorityNull() ? null : dbRow.Priority,
                Part = dbRow.IsPartNull() ? null : dbRow.Part,
                Quantity = dbRow.IsPartQuantityNull() ? (int?)null : dbRow.PartQuantity,
                SurfaceArea = dbRow.IsSurfaceAreaNull() ? (double?)null : dbRow.SurfaceArea,
                RequiredDate = dbRow.IsRequiredDateNull() ? (DateTime?)null : dbRow.RequiredDate,
                Process = dbRow.CurrentProcess,

                // Use CurrentLineString from OrderStatusDataTableAdapter's 
                // 'fill for client display' method
                CurrentLine = dbRow.Table.Columns.Contains("CurrentLineString")
                    ? dbRow["CurrentLineString"]?.ToString()
                    : "NA"
            };
        }

        /// <summary>
        /// Creates a new <see cref="ScheduleData"/> instance using a database row.
        /// </summary>
        /// <param name="dbRow">The database row.</param>
        /// <returns>
        /// A new instance of the <see cref="ScheduleData"/> class.
        /// </returns>
        /// <remarks>
        /// If the row's table contains an additional column named
        /// CurrentLineString, this method uses that column's value.
        /// </remarks>
        public static ScheduleData From(OrderStatusDataSet.BatchStatusRow dbRow)
        {
            if (dbRow == null)
            {
                return null;
            }

            return new ScheduleData
            {
                Id = dbRow.BatchID,
                Type = ScheduleDataType.Batch,
                WorkStatus = dbRow.IsWorkStatusNull() ? null : dbRow.WorkStatus,
                CurrentLocation = dbRow.CurrentLocation,
                NextDept = dbRow.IsNextDeptNull() ? null : dbRow.NextDept,
                SchedulePriority = dbRow.SchedulePriority,
                Customer = "N/A",
                Priority = "Normal",
                Part = "N/A",
                Quantity = dbRow.PartCount,
                SurfaceArea = dbRow.IsTotalSurfaceAreaNull() ? 0 : dbRow.TotalSurfaceArea,
                RequiredDate = null,
                Process = dbRow.CurrentProcess,

                // Use CurrentLineString from OrderStatusDataTableAdapter's 
                // 'fill for client display' method
                CurrentLine = dbRow.Table.Columns.Contains("CurrentLineString")
                    ? dbRow["CurrentLineString"]?.ToString()
                    : "NA"
            };
        }

        #endregion

        #region ScheduleDataType

        public enum ScheduleDataType
        {
            Order,
            Batch
        }

        #endregion
    }
}
