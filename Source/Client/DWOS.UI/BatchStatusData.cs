using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets;

namespace DWOS.UI
{
    /// <summary>
    /// Represents a single batch in a <see cref="IBatchSummary"/> instance.
    /// </summary>
    public sealed class BatchStatusData
    {
        #region Properties

        /// <summary>
        /// Gets the batch ID for this instance.
        /// </summary>
        public int BatchID
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the creation date for this instance.
        /// </summary>
        public DateTime OpenDate
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the fixture for this instance.
        /// </summary>
        public string Fixture
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the order count for this instance.
        /// </summary>
        public int OrderCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the part count for this instance.
        /// </summary>
        public int PartCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the total surface area for this instance.
        /// </summary>
        public double TotalSurfaceArea
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the total weight for this instance.
        /// </summary>
        public decimal TotalWeight
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the work status for this instance.
        /// </summary>
        public string WorkStatus
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current location for this instance.
        /// </summary>
        public string CurrentLocation
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current line ID for this instance.
        /// </summary>
        public int? CurrentLineID
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current line name for this instance.
        /// </summary>
        public string CurrentLine
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of this instance's current process.
        /// </summary>
        public string CurrentProcess
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the next department for this instance.
        /// </summary>
        public string NextDept
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a displayable list of operators for this instance.
        /// </summary>
        public string Operators
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a collection of orders in this instance's batch.
        /// </summary>
        public ICollection<BatchOrderStatusData> Orders
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of active timers for this instance.
        /// </summary>
        public int ActiveTimerCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the schedule priority for this instance.
        /// </summary>
        public int SchedulePriority
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the sales order for this instance.
        /// </summary>
        public int? SalesOrderId
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="BatchStatusData"/> instance using a database row.
        /// </summary>
        /// <param name="dbRow">The database row.</param>
        /// <param name="dtProcessingLine">Data table containing line names.</param>
        /// <returns>
        /// A new instance of the <see cref="BatchStatusData"/> class.
        /// </returns>
        /// <remarks>
        /// If the row's table contains an additional column named
        /// Operators, this method uses that column's value.
        /// </remarks>
        public static BatchStatusData CreateFrom(OrderStatusDataSet.BatchStatusRow dbRow)
        {
            // A specific fill method Operators and CurrentLineString
            const string operatorColumn = "Operators";
            const string currentLineColumn = "CurrentLineString";

            if (dbRow == null)
            {
                return null;
            }

            var table = dbRow.Table;

            return new BatchStatusData()
            {
                BatchID = dbRow.BatchID,
                OpenDate = dbRow.OpenDate,
                Fixture = dbRow.IsFixtureNull() ? string.Empty : dbRow.Fixture,
                OrderCount = dbRow.IsOrderCountNull() ? 0 : dbRow.OrderCount,
                PartCount = dbRow.IsPartCountNull() ? 0 : dbRow.PartCount,
                TotalSurfaceArea = dbRow.IsTotalSurfaceAreaNull() ? 0d : dbRow.TotalSurfaceArea,
                TotalWeight = dbRow.IsTotalWeightNull() ? 0M : dbRow.TotalWeight,
                WorkStatus = dbRow.IsWorkStatusNull() ? string.Empty : dbRow.WorkStatus,
                CurrentLocation = dbRow.CurrentLocation,
                CurrentLineID = dbRow.IsCurrentLineNull() ? (int?)null : dbRow.CurrentLine,
                CurrentLine = !table.Columns.Contains(currentLineColumn) || dbRow.IsNull(currentLineColumn) ? string.Empty : dbRow[currentLineColumn].ToString(),
                CurrentProcess = dbRow.IsCurrentProcessNull() ? string.Empty : dbRow.CurrentProcess,
                NextDept = dbRow.IsNextDeptNull() ? string.Empty : dbRow.NextDept,
                Operators = !table.Columns.Contains(operatorColumn) || dbRow.IsNull(operatorColumn) ? string.Empty : dbRow[operatorColumn].ToString(),
                Orders = dbRow.GetBatchOrderStatusRows().Select(BatchOrderStatusData.CreateFrom).ToList(),
                ActiveTimerCount = dbRow.IsActiveTimerCountNull() ? 0 : dbRow.ActiveTimerCount,
                SchedulePriority = dbRow.SchedulePriority,
                SalesOrderId = dbRow.IsSalesOrderIDNull()
                    ? (int?)null
                    : dbRow.SalesOrderID
            };
        }

        #endregion
    }
}