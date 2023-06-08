using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DWOS.Data.Datasets;

namespace DWOS.UI
{
    /// <summary>
    /// Represents a single order in a <see cref="IOrderSummary"/> instance.
    /// </summary>
    public class OrderStatusData
    {
        #region Properties

        /// <summary>
        /// Gets the work order for this instance.
        /// </summary>
        public int WO { get; private set; }

        /// <summary>
        /// Gets the purchase order for this instance.
        /// </summary>
        public string PO { get; private set; }

        /// <summary>
        /// Gets the customer's name for this instance.
        /// </summary>
        public string Customer { get; private set; }

        /// <summary>
        /// Gets the part's name for this instance.
        /// </summary>
        public string Part { get; private set; }

        /// <summary>
        /// Gets the order date for this instance.
        /// </summary>
        public DateTime? OrderDate { get; private set; }

        /// <summary>
        /// Gets the estimated ship date for this instance.
        /// </summary>
        public DateTime? EstShipDate { get; private set; }

        /// <summary>
        /// Gets the priority for this instance.
        /// </summary>
        public string Priority { get; private set; }

        /// <summary>
        /// Gets the work status for this instance.
        /// </summary>
        public string WorkStatus { get; private set; }

        /// <summary>
        /// Gets the current location for this instance.
        /// </summary>
        public string CurrentLocation { get; private set; }

        /// <summary>
        /// Gets the next department for this instance.
        /// </summary>
        public string NextDept { get; private set; }

        /// <summary>
        /// Gets the current process for this instance.
        /// </summary>
        public string CurrentProcess { get; private set; }

        /// <summary>
        /// Gets the order type for this instance.
        /// </summary>
        public int OrderType { get; private set; }

        /// <summary>
        /// Gets the part quantity for this instance.
        /// </summary>
        public int? PartQuantity { get; private set; }

        /// <summary>
        /// Gets a value indicating if this instance is on hold.
        /// </summary>
        public bool Hold { get; private set; }

        /// <summary>
        /// Gets a value indicating if this instance is in a batch.
        /// </summary>
        public bool InBatch { get; private set; }

        /// <summary>
        /// Gets a value indicating if this instance is in rework.
        /// </summary>
        public bool IsInRework { get; private set; }

        /// <summary>
        /// Gets the parent WO for this order if it was created via split rework.
        /// </summary>
        public int? ReworkParentWO { get; private set; }

        /// <summary>
        /// Gets a list of Order IDs for orders that were split from this order
        /// via split rework.
        /// </summary>
        public IdsInfo ReworkChildOrders { get; private set; }

        /// <summary>
        /// Gets the total surface are for this instance.
        /// </summary>
        public double? SurfaceArea { get; private set; }

        /// <summary>
        /// Gets the remaining time for this instance.
        /// </summary>
        public string RemainingTime { get; private set; }

        /// <summary>
        /// Gets the current process due date for this instance.
        /// </summary>
        public DateTime? CurrentProcessDue { get; private set; }

        /// <summary>
        /// Gets the schedule priority for this instance.
        /// </summary>
        public int SchedulePriority { get; private set; }

        /// <summary>
        /// Gets the current part processing count for this instance.
        /// </summary>
        public int? PartProcessingCount { get; private set; }

        /// <summary>
        /// Gets the sales order for this instance.
        /// </summary>
        public int? SalesOrderID { get; private set; }

        /// <summary>
        /// Gets the work status duration for this instance.
        /// </summary>
        public int? WorkStatusDuration { get; private set; }

        /// <summary>
        /// Gets the customer work order for this instance.
        /// </summary>
        public string CustomerWO { get; private set; }

        /// <summary>
        /// Gets the required date for this instance.
        /// </summary>
        public DateTime? RequiredDate { get; private set; }

        /// <summary>
        /// Gets the number of active timers for this instance.
        /// </summary>
        public int ActiveTimerCount { get; private set; }

        /// <summary>
        /// Gets a value indicating if this instance has a part mark.
        /// </summary>
        public bool HasPartMark { get; private set; }

        /// <summary>
        /// Gets the current line ID for this instance.
        /// </summary>
        public int? CurrentLine { get; private set; }

        /// <summary>
        /// Gets a displayable list of operators for this instance.
        /// </summary>
        public string Operators { get; private set; }

        /// <summary>
        /// Gets a displayable list of serial numbers for this instance.
        /// </summary>
        public string SerialNumbers { get; private set; }

        /// <summary>
        /// Gets the product class for this instance.
        /// </summary>
        public string ProductClass { get; private set; }

        /// <summary>
        /// Gets the current line name for this instance.
        /// </summary>
        public string CurrentLineString { get; private set; }

        /// <summary>
        /// Gets a name -> value mapping of custom fields for this instance.
        /// </summary>
        public IDictionary<string, object> CustomFields { get; set; }

        /// <summary>
        /// Gets the number of notes for this instance.
        /// </summary>
        public int OrderNoteCount { get; private set; }

        /// <summary>
        /// Gets a value indicating if this instance's order has notes.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="OrderNoteCount"/> is greater than 0;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool HasNotes => OrderNoteCount > 0;

        /// <summary>
        /// Gets the batch IDs for this instance.
        /// </summary>
        public IdsInfo Batches { get; private set; }

        /// <summary>
        /// Gets the value for any custom field or property for this instance.
        /// </summary>
        /// <param name="key">Name of the custom field or property</param>
        /// <returns>
        /// Value if found; otherwise, <c>null</c>.
        /// </returns>
        public object this[string key]
        {
            get
            {
                object obj;

                return CustomFields.TryGetValue(key, out obj)
                    ? obj
                    : GetValueFromProperty(key);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="OrderStatusData"/> instance using a database row.
        /// </summary>
        /// <param name="dbRow">The database row.</param>
        /// <returns>
        /// A new instance of the <see cref="OrderStatusData"/> class.
        /// </returns>
        /// <remarks>
        /// This method can use several columns that may be added to a
        /// <see cref="OrderStatusDataSet.OrderStatusDataTable"/> instance
        /// at runtime.
        /// </remarks>
        public static OrderStatusData CreateFrom(OrderStatusDataSet.OrderStatusRow dbRow)
        {
            const string operatorsColumn = "Operators";
            const string serialNumbersColumn = "SerialNumbers";
            const string productClassColumn = "ProductClass";
            const string currentLineStringColumn = "CurrentLineString";
            const string batchesColumn = "BatchIds";
            const string reworkParentColumn = "ReworkParentOrder";
            const string reworkChildrenColumn = "ReworkChildOrders";

            if (dbRow == null)
            {
                throw new ArgumentNullException(nameof(dbRow));
            }

            var table = dbRow.Table;

            var order = new OrderStatusData
            {
                WO = dbRow.WO,
                PO = dbRow.IsPONull() ? null : dbRow.PO,
                Customer = dbRow.IsCustomerNull() ? null : dbRow.Customer,
                Part = dbRow.IsPartNull() ? null : dbRow.Part,
                OrderDate = dbRow.IsOrderDateNull() ? (DateTime?)null : dbRow.OrderDate,
                EstShipDate = dbRow.IsEstShipDateNull() ? (DateTime?) null : dbRow.EstShipDate,
                Priority = dbRow.IsPriorityNull() ? null : dbRow.Priority,
                WorkStatus = dbRow.WorkStatus,
                CurrentLocation = dbRow.CurrentLocation,
                NextDept = dbRow.IsNextDeptNull() ? null : dbRow.NextDept,
                CurrentProcess = dbRow.IsCurrentProcessNull() ? null : dbRow.CurrentProcess,
                OrderType = dbRow.OrderType,
                PartQuantity = dbRow.IsPartQuantityNull() ? (int?) null : dbRow.PartQuantity,
                Hold = dbRow.Hold,
                InBatch = !dbRow.IsInBatchNull() && dbRow.InBatch,
                IsInRework = !dbRow.IsIsInReworkNull() && dbRow.IsInRework,
                ReworkParentWO = !table.Columns.Contains(reworkParentColumn) || dbRow.IsNull(reworkParentColumn)
                    ? (int?)null
                    : Convert.ToInt32(dbRow[reworkParentColumn]),
                ReworkChildOrders = !table.Columns.Contains(reworkChildrenColumn) || dbRow.IsNull(reworkChildrenColumn)
                    ? null
                    : IdsInfo.New(dbRow[reworkChildrenColumn] as IEnumerable<int>),
                SurfaceArea = dbRow.IsSurfaceAreaNull() ? (double?) null : dbRow.SurfaceArea,
                RemainingTime = dbRow.IsRemainingTimeNull() ? null : dbRow.RemainingTime,
                CurrentProcessDue = dbRow.IsCurrentProcessDueNull() ? (DateTime?) null : dbRow.CurrentProcessDue,
                SchedulePriority = dbRow.SchedulePriority,
                PartProcessingCount = dbRow.IsPartProcessingCountNull() ? (int?) null : dbRow.PartProcessingCount,
                SalesOrderID = dbRow.IsSalesOrderIDNull() ? (int?) null : dbRow.SalesOrderID,
                WorkStatusDuration = dbRow.IsWorkStatusDurationNull() ? (int?) null : dbRow.WorkStatusDuration,
                CustomerWO = dbRow.IsCustomerWONull() ? null : dbRow.CustomerWO,
                RequiredDate = dbRow.IsRequiredDateNull() ? (DateTime?) null : dbRow.RequiredDate,
                ActiveTimerCount = dbRow.IsActiveTimerCountNull() ? 0 : dbRow.ActiveTimerCount,
                HasPartMark = !dbRow.IsHasPartMarkNull() && dbRow.HasPartMark,
                CurrentLine = dbRow.IsCurrentLineNull() ? (int?) null : dbRow.CurrentLine,
                Operators = !table.Columns.Contains(operatorsColumn) || dbRow.IsNull(operatorsColumn) ? null : dbRow[operatorsColumn].ToString(),
                OrderNoteCount = dbRow.IsOrderNoteCountNull() ? 0 : dbRow.OrderNoteCount,
                SerialNumbers = !table.Columns.Contains(serialNumbersColumn) || dbRow.IsNull(serialNumbersColumn) ? null : dbRow[serialNumbersColumn].ToString(),
                ProductClass = !table.Columns.Contains(productClassColumn) || dbRow.IsNull(productClassColumn) ? null : dbRow[productClassColumn].ToString(),
                CurrentLineString = !table.Columns.Contains(currentLineStringColumn) || dbRow.IsNull(currentLineStringColumn) ? null : dbRow[currentLineStringColumn].ToString(),
                Batches = !table.Columns.Contains(batchesColumn) || dbRow.IsNull(batchesColumn)
                    ? null
                    : IdsInfo.New(dbRow[batchesColumn] as IEnumerable<int>),
                CustomFields = new Dictionary<string, object>()
            };

            foreach (var column in dbRow.Table.Columns
                .OfType<DataColumn>()
                .Where(c => c.ColumnName.StartsWith("CUSTOM_") || c.ColumnName.StartsWith("PARTCUSTOM_")))
            {
                order.CustomFields[column.ColumnName] = dbRow[column];
            }

            return order;
        }

        private object GetValueFromProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return null;
            }

            var prop = typeof(OrderStatusData).GetProperty(propertyName);
            return prop?.GetValue(this, null);
        }

        #endregion

        #region IdsInfo

        public class IdsInfo
        {
            public List<int> Ids { get; }

            public IdsInfo(List<int> ids)
            {
                Ids = ids;
            }

            public static IdsInfo New(IEnumerable<int> ids)
            {
                if (ids == null)
                {
                    return null;
                }
                return new IdsInfo(ids.OrderBy(id => id).ToList());
            }

            public override string ToString() =>
                string.Join(", ", Ids);
        }

        #endregion
    }
}
