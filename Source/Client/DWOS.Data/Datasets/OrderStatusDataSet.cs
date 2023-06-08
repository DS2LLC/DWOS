using DWOS.Shared.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Data.Datasets
{
    public partial class OrderStatusDataSet
    {
        private const string FIELD_PREFIX = "CUSTOM_";
        private const string PART_FIELD_PREFIX = "PARTCUSTOM_";

        partial class OrderStatusDataTable
        {

        }

        public interface ICustomFieldNameRow
        {
            /// <summary>
            /// Gets or sets the name of the field.
            /// </summary>
            string Name { get; }

            /// <summary>
            /// Gets a formatted copy of <see cref="Name"/> for use as a
            /// grid field name.
            /// </summary>
            /// <remarks>
            /// Necessary because a custom field may share the name of a built-in field.
            /// </remarks>
            string FormattedName { get; }
        }

        public partial class CustomFieldNameRow : ICustomFieldNameRow
        {
            /// <summary>
            /// Gets a formatted copy of <see cref="Name"/> for use as a
            /// grid field name.
            /// </summary>
            /// <remarks>
            /// Necessary because a custom field may share the name of a built-in field.
            /// </remarks>
            public string FormattedName
            {
                get
                {
                    return FIELD_PREFIX + Name;
                }
            }
        }

        public partial class PartLevelCustomFieldNameRow : ICustomFieldNameRow
        {
            /// <summary>
            /// Gets a formatted copy of <see cref="Name"/> for use as a
            /// grid field name.
            /// </summary>
            /// <remarks>
            /// Necessary because a custom field may share the name of a built-in field.
            /// </remarks>
            public string FormattedName
            {
                get
                {
                    return PART_FIELD_PREFIX + Name;
                }
            }
        }

        public partial class OrderStatusFieldRow
        {
            /// <summary>
            /// Gets a formatted copy of <see cref="Name"/> for use as a
            /// <see cref="System.Data.DataTable"/> column.
            /// </summary>
            /// <remarks>
            /// Necessary because a custom field may share the name of a built-in field.
            /// </remarks>
            public string FormattedName
            {
                get
                {

                    return FIELD_PREFIX + Name;
                }
            }
        }

        public partial class OrderStatusPartFieldRow
        {
            /// <summary>
            /// Gets a formatted copy of <see cref="Name"/> for use as a
            /// <see cref="System.Data.DataTable"/> column.
            /// </summary>
            /// <remarks>
            /// Necessary because a custom field may share the name of a built-in field.
            /// </remarks>
            public string FormattedName
            {
                get
                {

                    return PART_FIELD_PREFIX + Name;
                }
            }
        }
    }
}

namespace DWOS.Data.Datasets.OrderStatusDataSetTableAdapters 
{
    partial class OrderSearchTableAdapter
    {
        public enum OrderSearchField { All, OrderID, PartName, CustomerName, Status, PurchaseOrder, WorkStatus, CustomerWO, CurrentLocation, SerialNumber, None }

        public int FillBySearch(OrderStatusDataSet.OrderSearchDataTable dataTable, OrderSearchField field, string value, bool activeOnly)
        {
            string sqlSelect = @"SELECT [Order].OrderID, Customer.Name AS CustomerName, Part.Name AS PartName, [Order].OrderDate, [Order].Status, [Order].PurchaseOrder, [Order].Invoice, [Order].WorkStatus, [Order].CurrentLocation, [Order].CustomerWO, [Order].SchedulePriority, [ProcessingLine].Name AS CurrentLine
                                    FROM [Order] 
                                    INNER JOIN Customer ON [Order].CustomerID = Customer.CustomerID 
                                    INNER JOIN Part ON [Order].PartID = Part.PartID 
                                    LEFT OUTER JOIN ProcessingLine ON [Order].CurrentLine = ProcessingLine.ProcessingLineID";

            var sqlWhere = " WHERE";
            value = Utilities.SqlBless(value);

            switch (field)
            {
                case OrderSearchField.All:
                    sqlWhere += string.Format(" (Part.Name LIKE '%{0}%' OR Customer.Name LIKE '%{0}%' OR [Order].OrderID LIKE '%{0}%' OR Status LIKE '%{0}%' OR PurchaseOrder LIKE '%{0}%' OR WorkStatus LIKE '%{0}%' OR CustomerWO LIKE '%{0}%' OR CurrentLocation LIKE '%{0}%' OR [Order].OrderID IN (SELECT OrderID FROM OrderSerialNumber WHERE Active = 1 AND Number LIKE '%{0}%')) ", value);
                    break;   
                case OrderSearchField.PartName:
                    sqlWhere += string.Format(" Part.Name LIKE '%{0}%'", value);
                    break;
                case OrderSearchField.CustomerName:
                    sqlWhere += string.Format(" Customer.Name LIKE '%{0}%'", value);
                    break;
                case OrderSearchField.OrderID:
                    sqlWhere += string.Format(" [Order].OrderID LIKE '%{0}%'", value);
                    break;
                case OrderSearchField.SerialNumber:
                    sqlWhere += $" [Order].OrderID IN (SELECT OrderID FROM OrderSerialNumber WHERE Active = 1 AND Number LIKE '%{value}%')";
                    break;
                case OrderSearchField.Status:
                case OrderSearchField.PurchaseOrder:
                case OrderSearchField.WorkStatus:
                case OrderSearchField.CustomerWO:
                case OrderSearchField.CurrentLocation:
                    sqlWhere += string.Format(" {0} LIKE '%{1}%'", field, value);
                    break;
                case OrderSearchField.None:
                default:
                    break;
            }

            if(activeOnly)
            {
                if(field != OrderSearchField.None)
                    sqlWhere += " AND ";
                
                sqlWhere += " [Order].Status = 'Open'";
            }

            var sqlOrder = " ORDER BY [Order].OrderID";
            var sqlQuery = sqlSelect + sqlWhere + sqlOrder;

            this.Adapter.SelectCommand = new System.Data.SqlClient.SqlCommand(sqlQuery, this.Connection);

            if ((this.ClearBeforeFill == true))
                dataTable.Clear();

            return this.Adapter.Fill(dataTable);
        }
    }
    public partial class OrderStatusTableAdapter {

        private const string FIELD_OPERATORS = "Operators";
        private const string FIELD_SERIAL = "SerialNumbers";
        private const string FIELD_PRODUCT_CLASS = "ProductClass";
        private const string FIELD_CURRENT_LINE_STRING = "CurrentLineString";
        private const string FIELD_BATCHES = "BatchIds";
        private const string FIELD_REWORK_PARENT_ORDER = "ReworkParentOrder";
        private const string FIELD_REWORK_CHILD_ORDERS = "ReworkChildOrders";

        public void FillForClientDisplay(OrderStatusDataSet.OrderStatusDataTable dataTable)
        {
            this.Fill(dataTable);

            OrderStatusDataSet.OrderStatusFieldDataTable statusFieldData;
            using (var taField = new OrderStatusFieldTableAdapter())
            {
                statusFieldData = taField.GetData();
            }

            OrderStatusDataSet.OrderStatusPartFieldDataTable statusPartFieldData;
            using (var taField = new OrderStatusPartFieldTableAdapter())
            {
                statusPartFieldData = taField.GetData();
            }

            var partFieldLookupByPart = statusPartFieldData
                .GroupBy(f => f.IsPartIDNull() ? -1 : f.PartID)
                .ToDictionary(group => group.Key, group => group.ToList());

            OrderStatusDataSet.CustomFieldNameDataTable fields;
            using (var taField = new CustomFieldNameTableAdapter())
            {
                fields = taField.GetData();
            }

            OrderStatusDataSet.PartLevelCustomFieldNameDataTable partLevelFields;
            using (var taField = new PartLevelCustomFieldNameTableAdapter())
            {
                partLevelFields = taField.GetData();
            }

            OrderStatusDataSet.ProcessingLineDataTable processingLines;
            using (var taProcessingLines = new ProcessingLineTableAdapter())
            {
                processingLines = taProcessingLines.GetData();
            }

            OrderStatusDataSet.OrderStatusOperatorDataTable operators;
            using (var taOperator = new OrderStatusOperatorTableAdapter())
            {
                operators = taOperator.GetByStatus("Open", nameof(OperatorStatus.Active));
            }

            OrderStatusDataSet.OrderSerialNumberDataTable serialNumbers;
            using (var taSerialNumber = new OrderSerialNumberTableAdapter())
            {
                serialNumbers = taSerialNumber.GetActive();
            }

            OrderStatusDataSet.OrderProductClassDataTable productClasses;
            using (var taProductClass = new OrderProductClassTableAdapter())
            {
                productClasses = taProductClass.GetActive();
            }

            OrderStatusDataSet.BatchOrderSummaryDataTable dtBatchOrder;
            using (var taBatchOrder = new BatchOrderSummaryTableAdapter())
            {
                dtBatchOrder = taBatchOrder.GetActive();
            }

            OrderStatusDataSet.InternalReworkDataTable dtRework;
            using (var taRework = new InternalReworkTableAdapter())
            {
                dtRework = taRework.GetActive();
            }

            // Add columns to data table
            foreach (var fieldName in fields.Select(i => i.FormattedName))
            {
                if (!dataTable.Columns.Contains(fieldName))
                {
                    dataTable.Columns.Add(fieldName);
                }
            }

            foreach (var fieldName in partLevelFields.Select(i => i.FormattedName))
            {
                if (!dataTable.Columns.Contains(fieldName))
                {
                    dataTable.Columns.Add(fieldName);
                }
            }

            if (!dataTable.Columns.Contains(FIELD_OPERATORS))
            {
                dataTable.Columns.Add(FIELD_OPERATORS);
            }

            if (!dataTable.Columns.Contains(FIELD_SERIAL))
            {
                dataTable.Columns.Add(FIELD_SERIAL);
            }

            if (!dataTable.Columns.Contains(FIELD_CURRENT_LINE_STRING))
            {
                dataTable.Columns.Add(FIELD_CURRENT_LINE_STRING);
            }

            if (!dataTable.Columns.Contains(FIELD_PRODUCT_CLASS))
            {
                dataTable.Columns.Add(FIELD_PRODUCT_CLASS);
            }

            if (!dataTable.Columns.Contains(FIELD_BATCHES))
            {
                dataTable.Columns.Add(FIELD_BATCHES, typeof(List<int>));
            }

            if (!dataTable.Columns.Contains(FIELD_REWORK_PARENT_ORDER))
            {
                dataTable.Columns.Add(FIELD_REWORK_PARENT_ORDER, typeof(int));
            }

            if (!dataTable.Columns.Contains(FIELD_REWORK_CHILD_ORDERS))
            {
                dataTable.Columns.Add(FIELD_REWORK_CHILD_ORDERS, typeof(List<int>));
            }

            foreach (var field in statusFieldData)
            {
                var orderStatus = dataTable.FindByWO(field.OrderID);

                if (orderStatus == null || !dataTable.Columns.Contains(field.FormattedName))
                {
                    continue;
                }

                orderStatus[field.FormattedName] = field.IsValueNull() ? string.Empty : field.Value;
            }

            foreach (var orderStatus in dataTable)
            {
                const string itemSeparator = ", ";

                var operatorNames = operators
                    .Where(op => op.OrderID == orderStatus.WO)
                    .Select(op => op.Name.ToInitials(StringInitialOption.AllInitials))
                    .OrderBy(initials => initials);

                orderStatus[FIELD_OPERATORS] = string.Join(itemSeparator, operatorNames);

                if (orderStatus.RowState == System.Data.DataRowState.Modified)
                {
                    orderStatus.EndEdit();
                    orderStatus.AcceptChanges();
                }

                var serials = serialNumbers
                    .Where(serial => serial.OrderID == orderStatus.WO && !serial.IsNumberNull())
                    .OrderBy(serial => serial.PartOrder)
                    .Select(serial => serial.Number)
                    .ToList();

                if (serials.Count > 3)
                {
                    serials.RemoveRange(3, serials.Count - 3);
                    serials.Add("...");
                }

                orderStatus[FIELD_SERIAL] = string.Join(itemSeparator, serials);

                if (!orderStatus.IsCurrentLineNull())
                {
                    orderStatus[FIELD_CURRENT_LINE_STRING] =
                        processingLines.FindByProcessingLineID(orderStatus.CurrentLine)?.Name ??
                        orderStatus.CurrentLine.ToString();
                }

                var productClass = productClasses.FirstOrDefault(c => c.OrderID == orderStatus.WO);

                if (productClass != null && !productClass.IsProductClassNull())
                {
                    orderStatus[FIELD_PRODUCT_CLASS] = productClass.ProductClass;
                }

                // Batches
                var batchOrders = dtBatchOrder
                    .Where(bo => bo.OrderID == orderStatus.WO)
                    .ToList();

                if (batchOrders.Count > 0)
                {
                    orderStatus[FIELD_BATCHES] = batchOrders.Select(bo => bo.BatchID).ToList();
                }

                // Part-level fields
                if (!orderStatus.IsPartIDNull()
                    && partFieldLookupByPart.TryGetValue(orderStatus.PartID, out var partFields))
                {
                    foreach (var field in partFields)
                    {
                        orderStatus[field.FormattedName] = field.IsValueNull()
                            ? string.Empty
                            : field.Value;
                    }
                }

                // Rework
                var parentReworks = dtRework
                    .Where(rw => !rw.IsReworkOrderIDNull() && rw.ReworkOrderID == orderStatus.WO)
                    .ToList();

                if (parentReworks.Count > 0)
                {
                    orderStatus[FIELD_REWORK_PARENT_ORDER] = parentReworks[0].OriginalOrderID;
                }

                var originalOrderReworks = dtRework
                    .Where(rw => rw.OriginalOrderID == orderStatus.WO && !rw.IsReworkOrderIDNull())
                    .ToList();

                if (originalOrderReworks.Count > 0)
                {
                    orderStatus[FIELD_REWORK_CHILD_ORDERS] = originalOrderReworks
                        .Select(rw => rw.ReworkOrderID)
                        .ToList();
                }
            }
        }
    }
    public partial class BatchStatusTableAdapter {
        private const string FIELD_OPERATORS = "Operators";
        private const string FIELD_CURRENT_LINE_STRING = "CurrentLineString";

        public void FillForClientDisplay(OrderStatusDataSet.BatchStatusDataTable dataTable)
        {
            this.FillActive(dataTable);

            if (!dataTable.Columns.Contains(FIELD_OPERATORS))
            {
                dataTable.Columns.Add(FIELD_OPERATORS);
            }

            if (!dataTable.Columns.Contains(FIELD_CURRENT_LINE_STRING))
            {
                dataTable.Columns.Add(FIELD_CURRENT_LINE_STRING);
            }

            OrderStatusDataSet.BatchStatusOperatorDataTable operators;

            using (var taOperator = new BatchStatusOperatorTableAdapter())
            {
                operators = taOperator.GetByStatus(true, nameof(OperatorStatus.Active));
            }

            OrderStatusDataSet.ProcessingLineDataTable processingLines;
            using (var taProcessingLines = new ProcessingLineTableAdapter())
            {
                processingLines = taProcessingLines.GetData();
            }

            foreach (var batchStatus in dataTable)
            {
                const string operatorSeparator = ", ";

                var operatorNames = operators
                    .Where(op => op.BatchID == batchStatus.BatchID)
                    .Select(op => op.Name.ToInitials(StringInitialOption.AllInitials))
                    .OrderBy(initials => initials);

                batchStatus[FIELD_OPERATORS] = string.Join(operatorSeparator, operatorNames);

                if (batchStatus.RowState == System.Data.DataRowState.Modified)
                {
                    batchStatus.EndEdit();
                    batchStatus.AcceptChanges();
                }

                if (!batchStatus.IsCurrentLineNull())
                {
                    batchStatus[FIELD_CURRENT_LINE_STRING] =
                        processingLines.FindByProcessingLineID(batchStatus.CurrentLine)?.Name ??
                        batchStatus.CurrentLine.ToString();
                }
            }
        }
    }
}
