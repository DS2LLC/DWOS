using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ListsDataSetTableAdapters;
using DWOS.Shared;
using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraMessageBox;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinMaskedEdit;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinToolTip;
using Infragistics.Win.UltraWinTree;
using NLog;
using DWOS.Data;

namespace DWOS.UI.Utilities
{
    internal static class DataUtilities
    {
        public static void LoadComboBox(ComboBoxTool cbo, DataTable table, string columnValue, string columnText)
        {
            ValueListItemsCollection list = cbo.ValueList.ValueListItems;
            list.Clear();

            foreach(DataRow row in table.Rows)
            {
                if(!row.IsNull(columnValue))
                    list.Add(row[columnValue], row[columnText].ToString());
            }

            if(list.Count > 0)
                cbo.SelectedIndex = 0;
        }

        public static void LoadComboBox(UltraComboEditor cbo, DataTable table, string columnValue, string columnText)
        {
            cbo.Items.Clear();
            cbo.BeginUpdate();

            foreach(DataRow row in table.Rows)
            {
                if(!row.IsNull(columnValue))
                    cbo.Items.Add(row[columnValue], row[columnText].ToString());
            }

            if(cbo.Items.Count > 0)
                cbo.SelectedIndex = 0;

            cbo.EndUpdate();
        }

        public static void LoadComboBox(UltraComboEditor cbo, DataRow[] rows, string columnValue, string columnText)
        {
            cbo.Items.Clear();
            cbo.BeginUpdate();

            foreach(DataRow row in rows)
            {
                if(!row.IsNull(columnValue))
                    cbo.Items.Add(row[columnValue], row[columnText].ToString());
            }

            if(cbo.Items.Count > 0)
                cbo.SelectedIndex = 0;

            cbo.EndUpdate();
        }

        public static void LoadComboBox(UltraComboEditor cbo, DataTable table, string columnValue, string columnText, string columnColor)
        {
            cbo.Items.Clear();
            cbo.BeginUpdate();

            foreach(DataRow row in table.Rows)
            {
                if(!row.IsNull(columnValue))
                {
                    ValueListItem item = cbo.Items.Add(row[columnValue], row[columnText].ToString());

                    if(!row.IsNull(columnColor) && !String.IsNullOrEmpty(row[columnColor].ToString()))
                        item.Appearance.ForeColor = Color.FromName(row[columnColor].ToString());
                }
            }

            if(cbo.Items.Count > 0)
                cbo.SelectedIndex = 0;

            cbo.EndUpdate();
        }

        public static ValueList CreateValueList(DataTable table, string columnValue, string columnText, string columnColor)
        {
            var list = new ValueList();

            foreach(DataRow row in table.Rows)
            {
                if(!row.IsNull(columnValue))
                {
                    var item = new ValueListItem(row[columnValue], row[columnText].ToString());

                    if(columnColor != null && !row.IsNull(columnColor) && !String.IsNullOrEmpty(row[columnColor].ToString()))
                        item.Appearance.ForeColor = Color.FromName(row[columnColor].ToString());

                    list.ValueListItems.Add(item);
                }
            }

            return list;
        }

        public static ValueListItem FindItemByValue(this ComboBoxTool cbo, Predicate <ValueListItem> predicate)
        {
            if(cbo != null && cbo.ValueList != null && cbo.ValueList.ValueListItems != null)
            {
                foreach(ValueListItem item in cbo.ValueList.ValueListItems)
                {
                    if(predicate(item))
                        return item;
                }
            }

            return null;
        }

        public static ValueListItem FindItemByValue<T>(this UltraComboEditor cbo, Predicate <T> predicate)
        {
            foreach(ValueListItem item in cbo.Items)
            {
                if(item.DataValue != null && item.DataValue is T && predicate((T) item.DataValue))
                    return item;
            }

            return null;
        }

        public static T SelectedItemDataValue<T>(this ComboBoxTool cbo)
        {
            if(cbo != null)
            {
                var item = cbo.SelectedItem as ValueListItem;
                if(item != null && item.DataValue is T)
                    return (T) item.DataValue;
            }

            return default(T);
        }

        public static int EndAllRowEdits(this DataTable dt)
        {
            int i = 0;
            foreach(DataRow item in dt.Rows)
            {
                bool isProposed = item.HasVersion(DataRowVersion.Proposed);
                if(isProposed)
                {
                    item.EndEdit();
                    i++;
                }
            }

            return i;
        }

        public static int IsRowInEditState(DataSet ds)
        {
            int editingCount = 0;

            foreach(DataTable table in ds.Tables)
            {
                foreach(DataRow row in table.Rows)
                {
                    if(row.HasVersion(DataRowVersion.Proposed))
                        editingCount++;
                }
            }

            return editingCount;
        }

        public static List <DataRow> GetRowsByRowState(DataTable dt, DataRowState rowState)
        {
            var rows = new List <DataRow>();

            foreach(DataRow row in dt.Rows)
            {
                if(row.RowState.HasFlag(rowState))
                    rows.Add(row);
            }

            return rows;
        }

        public static Dictionary <DataRow, object[]> GetRowsAndValuesByRowState(DataTable dt, DataRowState rowState)
        {
            var rows = new Dictionary <DataRow, object[]>();

            foreach(DataRow row in dt.Rows)
            {
                if(row.RowState.HasFlag(rowState))
                {
                    var items = new object[row.Table.Columns.Count];
                    for(int i = 0; i < row.Table.Columns.Count; i++)
                        items[i] = row[i, DataRowVersion.Original];
                    rows.Add(row, items);
                }
            }

            return rows;
        }

        public static DataRow FindDeletedRow(this DataTable dt, Func <Dictionary <string, object>, bool> compare)
        {
            foreach(DataRow row in dt.Rows)
            {
                if(row.RowState.HasFlag(DataRowState.Deleted))
                {
                    var values = new Dictionary <string, object>();

                    for(int i = 0; i < row.Table.Columns.Count; i++)
                        values.Add(dt.Columns[i].ColumnName, row[i, DataRowVersion.Original]);

                    if(compare(values))
                        return row;
                }
            }

            return null;
        }

        public static List <DataRow> FindDeletedRows(this DataTable dt)
        {
            var deletedRows = new List <DataRow>();

            foreach(DataRow row in dt.Rows)
            {
                if(row.RowState.HasFlag(DataRowState.Deleted))
                    deletedRows.Add(row);
            }

            return deletedRows;
        }

        public static string GetValue(object o, string defaultValue)
        {
            if(o == null || o == DBNull.Value)
                return defaultValue;
            return o.ToString();
        }

        public static string ToDetailedString(this DataRow dr)
        {
            var sb = new StringBuilder();

            if(dr != null)
            {
                object[] itemArray = dr.ItemArray;
                sb.Append(dr.GetType().FullName);

                foreach(object item in itemArray)
                {
                    sb.Append("|");

                    if(item != null && item != DBNull.Value)
                        sb.Append(item);
                    else
                        sb.Append("<Null>");
                }
                sb.Append(Environment.NewLine);

                foreach(DataRelation rel in dr.Table.ChildRelations)
                {
                    sb.AppendLine("-- Relation: " + rel.RelationName);

                    DataRow[] childRows = dr.GetChildRows(rel);
                    foreach(DataRow cr in childRows)
                        sb.AppendLine(cr.ToDetailedString());
                }
            }

            return sb.ToString();
        }

        public static string PrimaryKey(this DataRow dr)
        {
            if(dr != null && dr.Table != null && dr.Table.PrimaryKey.Length > 0)
                return dr[dr.Table.PrimaryKey[0], DataRowVersion.Default].ToString();

            return null;
        }

        public static bool IsValueChanged(this DataRow row, DataColumn column)
        {
            if (row.IsNull(column, DataRowVersion.Original) || row.IsNull(column, DataRowVersion.Current))
            {
                return !row.IsNull(column, DataRowVersion.Original) || !row.IsNull(column, DataRowVersion.Current);
            }
            else if (!row[column, DataRowVersion.Original].Equals(row[column, DataRowVersion.Current]))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a value that indicates if a proposed value is different from the current.
        /// </summary>
        /// <param name="row">The row to check.</param>
        /// <param name="column">The column to check.</param>
        /// <returns>
        /// <c>true</c> if the current and proposed versions are different;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool IsProposedValueChanged(this DataRow row, DataColumn column)
        {
            if (row.IsNull(column, DataRowVersion.Current) || row.IsNull(column, DataRowVersion.Proposed))
            {
                return !row.IsNull(column, DataRowVersion.Current) || !row.IsNull(column, DataRowVersion.Proposed);
            }
            else if (!row[column, DataRowVersion.Current].Equals(row[column, DataRowVersion.Proposed]))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Prepares user input for use in a 'LIKE' filter argument of
        /// <see cref="DataTable.Select(string)"/> or any other method using
        /// RowFilter syntax.
        /// </summary>
        /// <param name="input">user input to prepare</param>
        /// <returns>Prepared copy of the input string.</returns>
        public static string PrepareForRowFilterLike(string input)
        {
            const string charactersToEscape = @"\[\]*%";

            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input
                .ReplaceWith(@"([" + charactersToEscape + "])", @"[${0}]")
                .ReplaceWith("'", "''");
        }

        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hours, minutes, seconds, milliseconds, dateTime.Kind);
        }
    }

    public static class ControlUtilities
    {
        /// <summary>
        /// The maximum length of a Text input.
        /// </summary>
        /// <remarks>
        /// OrderProcessAnswer.Answer, PartInspectionAnswer.Answer, and
        /// ProcessStepCondition.Value have a maximum length of 255.
        /// </remarks>
        private const int MAX_TEXT_LENGTH = 255;

        public enum ControlCategory { DateTime, Numeric, String, List }

        public static ControlCategory GetCategory(InputType inputType)
        {
            switch(inputType)
            {
                case InputType.Decimal:
                case InputType.DecimalBefore:
                case InputType.DecimalAfter:
                case InputType.DecimalDifference:
                case InputType.Integer:
                case InputType.PartQty:
                case InputType.TimeDuration:
                case InputType.RampTime:
                case InputType.PreProcessWeight:
                case InputType.PostProcessWeight:
                    return ControlCategory.Numeric;
                case InputType.List:
                    return ControlCategory.List;
                case InputType.Date:
                case InputType.Time:
                case InputType.TimeIn:
                case InputType.TimeOut:
                case InputType.DateTimeIn:
                case InputType.DateTimeOut:
                    return ControlCategory.DateTime;
                case InputType.None:
                case InputType.String:
                default:
                    return ControlCategory.String;
            }
        }

        public static Image GetInputTypeImage(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.Decimal:
                case InputType.DecimalBefore:
                case InputType.DecimalAfter:
                case InputType.DecimalDifference:
                case InputType.Integer:
                case InputType.PartQty:
                case InputType.TimeDuration:
                case InputType.RampTime:
                case InputType.PreProcessWeight:
                case InputType.PostProcessWeight:
                    return Properties.Resources.numbers_16;
                case InputType.List:
                    return Properties.Resources.StepOrder;
                case InputType.Date:
                    return Properties.Resources.Calendar_16;
                case InputType.Time:
                case InputType.TimeIn:
                case InputType.TimeOut:
                case InputType.DateTimeIn:
                case InputType.DateTimeOut:
                    return Properties.Resources.Clock_16;
                case InputType.None:
                case InputType.String:
                default:
                    return Properties.Resources.CustomField_16;
            }
        }

        public static Control CreateControl(InputType inputType, int listID, string numericUnits, string defaultValue = null, int? maxTextLength = null)
        {
            switch(inputType)
            {
                case InputType.Decimal:
                case InputType.DecimalBefore:
                case InputType.DecimalAfter:
                case InputType.DecimalDifference:
                case InputType.Integer:
                case InputType.PartQty:
                case InputType.TimeDuration:
                case InputType.RampTime:
                case InputType.PreProcessWeight:
                case InputType.PostProcessWeight:
                    return CreateNumericControl(inputType, numericUnits, defaultValue);
                case InputType.List:
                    return CreateComboControl(listID);
                case InputType.Date:
                case InputType.Time:
                case InputType.TimeIn:
                case InputType.TimeOut:
                case InputType.DateTimeIn:
                case InputType.DateTimeOut:
                    return CreateDateTimeControl(inputType);
                case InputType.SampleSet:
                    return CreateButtonControl(listID);
                case InputType.None:
                case InputType.String:
                
                default:
                    return CreateTextControl(defaultValue, maxTextLength);
            }
        }

        public static Control CreateControlInspection(Data.ProcessStrictnessLevel inspectionLevel, InputType inputType, int listID, string numericUnits, string defaultValue = null)
        {
            if (inputType == InputType.List && inspectionLevel != Data.ProcessStrictnessLevel.Strict)
            {
                return CreateComboControl(listID, defaultValue);
            }
            else
            {
                return CreateControl(inputType, listID, numericUnits, defaultValue);
            }
        }

        public static string GetControlTooltip(InputType inputType, string minValue, string maxValue, string numericUnits)
        {
            switch(inputType)
            {
                case InputType.PartQty:
                    return "Set the correct quantity of parts.";
                case InputType.Decimal:
                case InputType.DecimalBefore:
                case InputType.DecimalAfter:
                case InputType.DecimalDifference:
                case InputType.Integer:
                case InputType.TimeDuration:
                case InputType.RampTime:
                case InputType.PreProcessWeight:
                case InputType.PostProcessWeight:
                    return String.Format("Set the [{0}] value between {1} and {2}", numericUnits, minValue, maxValue);
                case InputType.List:
                    return "Select correct value from the list.";
                case InputType.Date:
                    return "Set the correct date.";
                case InputType.Time:
                    return "Set the correct time.";
                case InputType.TimeIn:
                case InputType.DateTimeIn:
                    return "Set the correct start time.";
                case InputType.TimeOut:
                case InputType.DateTimeOut:
                    return "Set the correct end time.";
                case InputType.None:
                case InputType.String:
                default:
                    return "Set the text as required.";
            }
        }

        public static string GetControlTooltipShort(InputType inputType, string minValue, string maxValue, string numericUnits)
        {
            switch(inputType)
            {
                case InputType.PartQty:
                    return "Part Quantity";
                case InputType.Decimal:
                case InputType.DecimalBefore:
                case InputType.DecimalAfter:
                case InputType.DecimalDifference:
                case InputType.PreProcessWeight:
                case InputType.PostProcessWeight:
                    if(String.IsNullOrEmpty(minValue))
                        minValue = "0.0";
                    if(String.IsNullOrEmpty(maxValue))
                        maxValue = "999.0";
                    if(String.IsNullOrEmpty(numericUnits))
                        numericUnits = "Num.";
                    return String.Format("{0} - {1} {2}", minValue, maxValue, numericUnits);
                case InputType.Integer:
                case InputType.TimeDuration:
                case InputType.RampTime:
                    if(String.IsNullOrEmpty(minValue))
                        minValue = "0";
                    if(String.IsNullOrEmpty(maxValue))
                        maxValue = "999";
                    if(String.IsNullOrEmpty(numericUnits))
                        numericUnits = "Num.";
                    return String.Format("{0} - {1} {2}", minValue, maxValue, numericUnits);
                case InputType.List:
                    return "List";
                case InputType.Date:
                    return "Date";
                case InputType.Time:
                    return "Time";
                case InputType.TimeIn:
                case InputType.DateTimeIn:
                    return "Time In";
                case InputType.TimeOut:
                case InputType.DateTimeOut:
                    return "Time Out";
                case InputType.None:
                case InputType.String:
                default:
                    return "Text";
            }
        }

        private static UltraNumericEditor CreateNumericControl(InputType inputType, string numericUnits, string defaultValue = null)
        {
            var c = new UltraNumericEditor();

            if(!String.IsNullOrEmpty(numericUnits))
            {
                string escapedNumeric = null;

                foreach(char item in numericUnits)
                    escapedNumeric += "\\" + item;

                numericUnits = " " + escapedNumeric; //add leading space
            }

            switch(inputType)
            {
                case InputType.Decimal:
                case InputType.DecimalBefore:
                case InputType.DecimalAfter:
                case InputType.DecimalDifference:
                    c.NumericType = NumericType.Decimal;
                    c.MaxValue = Int32.MaxValue;
                    c.MinValue = Int32.MinValue;

                    decimal decDefault;
                    var processingDecimalPlaces = ApplicationSettings.Current.ProcessingDecimalPlaces;
                    if(!String.IsNullOrEmpty(defaultValue) && Decimal.TryParse(defaultValue, out decDefault))
                    {
                        c.Value = decDefault;

                        //set mask input
                        int maxWholePlace = decDefault.NumberOfWholeNumberPlaces();
                        c.MaskInput = "{double:-" + Math.Max(maxWholePlace, 6) + "." + processingDecimalPlaces + "}" + numericUnits;
                    }
                    else
                        c.MaskInput = "{double:-6." + processingDecimalPlaces +"}" + numericUnits;

                    break;
                case InputType.PreProcessWeight:
                case InputType.PostProcessWeight:
                    c.NumericType = NumericType.Decimal;
                    c.MaxValue = int.MaxValue;
                    c.MinValue = int.MinValue;
                    c.MaskInput = string.Format("nnnn.{0} lbs",
                        string.Concat(Enumerable.Repeat("n", ApplicationSettings.Current.WeightDecimalPlaces)));

                    break;
                case InputType.PartQty:
                case InputType.Integer:
                case InputType.TimeDuration:
                case InputType.RampTime:
                    c.NumericType = NumericType.Integer;
                    c.MaxValue = Int32.MaxValue;
                    c.MinValue = Int32.MinValue;

                    int intDefault;
                    if(!String.IsNullOrEmpty(defaultValue) && Int32.TryParse(defaultValue, out intDefault))
                        c.Value = intDefault;

                    c.MaskInput = "-nnn,nnn,nnn" + numericUnits;

                    break;
                default:
                    return null;
            }

            return c;
        }

        private static int NumberOfWholeNumberPlaces(this decimal number)
        {

            string testdec = Convert.ToString(Math.Truncate(number)); //get all the numbers on the LEFT side
            return testdec.Length; //total length minus beginning numbers and decimal = number of decimal points
        }

        private static UltraTextEditor CreateTextControl(string defaultValue = null, int? maxTextLength = null) 
        {
            return maxTextLength == null ? new UltraTextEditor {Text = defaultValue, MaxLength = MAX_TEXT_LENGTH } : new UltraTextEditor { Text = defaultValue, MaxLength = maxTextLength.Value }; 
        }

        private static UltraComboEditor CreateComboControl(int listID, string defaultValue = null)
        {
            var c = new UltraComboEditor();
            var ta = new ListValuesTableAdapter();
            var dsLists = new ListsDataSet();
            ta.FillBy(dsLists.ListValues, listID);
            c.DropDownStyle = DropDownStyle.DropDownList;
            c.DropDownListWidth = -1;

            DataUtilities.LoadComboBox(c, dsLists.ListValues, dsLists.ListValues.ValueColumn.ColumnName, dsLists.ListValues.ValueColumn.ColumnName);

            if(!String.IsNullOrEmpty(defaultValue))
            {
                ValueListItem found = c.FindItemByValue <object>(p => p != null && p.ToString() == defaultValue);
                if(found != null)
                    c.SelectedItem = found;
            }

            return c;
        }

       
        private static UltraButton CreateButtonControl(int listID, string defaultValue = null)
        {
            var c = new UltraButton();
            var ta = new ListValuesTableAdapter();
            var dsLists = new ListsDataSet();
            ta.FillBy(dsLists.ListValues, listID);
            c.Text = "Enter Sample Data";

            return c;
        }

        private static UltraDateTimeEditor CreateDateTimeControl(InputType inputType)
        {
            var c = new UltraDateTimeEditor();

            switch(inputType)
            {
                case InputType.Date:
                    c.MaskInput = "{date}";
                    c.AutoFillDate = AutoFillDate.None;
                    break;
                case InputType.Time:
                case InputType.TimeIn:
                case InputType.TimeOut:
                    c.MaskInput = "{longtime}";
                    c.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
                    c.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
                    break;
                case InputType.DateTimeIn:
                case InputType.DateTimeOut:
                    c.MaskInput = "{date} {longtime}";
                    c.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
                    c.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
                    break;
                default:
                    return null;
            }

            return c;
        }

        public static bool ValidateAnswer(InputType inputType, string minValue, string maxValue, int listID, string answer)
        {
            switch(inputType)
            {
                case InputType.SampleSet:
                case InputType.Decimal:
                case InputType.DecimalBefore:
                case InputType.DecimalAfter:
                case InputType.DecimalDifference:
                case InputType.PreProcessWeight:
                case InputType.PostProcessWeight:
                    Decimal decMaxValue, decMinValue;
                    Decimal decValue;
                    //ensure min value is set
                    if(String.IsNullOrWhiteSpace(minValue))
                        minValue = "0.0";
                    if(String.IsNullOrWhiteSpace(maxValue))
                        maxValue = "99999.0";
                    return Decimal.TryParse(answer, out decValue) && Decimal.TryParse(maxValue, out decMaxValue) && Decimal.TryParse(minValue, out decMinValue) && decValue <= decMaxValue && decValue >= decMinValue;
                case InputType.PartQty:
                    int partQty = 0;
                    return int.TryParse(answer, out partQty);
                case InputType.Integer:
                case InputType.TimeDuration:
                case InputType.RampTime:
                    Int32 intMaxValue, intMinValue;
                    Int32 intValue;
                    //ensure min value is set
                    if(String.IsNullOrWhiteSpace(minValue))
                        minValue = "0";
                    if(String.IsNullOrWhiteSpace(maxValue))
                        maxValue = "999";
                    return Int32.TryParse(answer, out intValue) && Int32.TryParse(maxValue, out intMaxValue) && Int32.TryParse(minValue, out intMinValue) && intValue <= intMaxValue && intValue >= intMinValue;
                case InputType.Date:
                case InputType.Time:
                case InputType.TimeIn:
                case InputType.TimeOut:
                case InputType.DateTimeIn:
                case InputType.DateTimeOut:
                    DateTime dt;
                    return DateTime.TryParse(answer, out dt);
                case InputType.None:
                case InputType.String:
                case InputType.List:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Can the given answered be considered blank for the given input type?
        /// </summary>
        /// <param name="inputType">type of input</param>
        /// <param name="answer">response to check</param>
        /// <returns>true if the answer is blank; otherwise, false</returns>
        public static bool IsAnswerBlank(InputType inputType, string answer)
        {
            switch (inputType)
            {
                case InputType.Decimal:
                case InputType.DecimalBefore:
                case InputType.DecimalAfter:
                case InputType.DecimalDifference:
                case InputType.PreProcessWeight:
                case InputType.PostProcessWeight:
                    decimal decimalResult;
                    decimal.TryParse(answer, out decimalResult);
                    return decimalResult == 0M;
                case InputType.PartQty:
                case InputType.Integer:
                case InputType.RampTime:
                case InputType.TimeDuration:
                    int integerResult;
                    int.TryParse(answer, out integerResult);
                    return integerResult == 0;
                case InputType.List:
                case InputType.None:
                case InputType.String:
                    return String.IsNullOrEmpty(answer);
                case InputType.Date:
                case InputType.Time:
                case InputType.TimeIn:
                case InputType.TimeOut:
                case InputType.DateTimeIn:
                case InputType.DateTimeOut:
                    // DateTime inputs are not considered blank.
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        ///     Validates that the number is within valid min/max range of numeric editor.
        /// </summary>
        /// <param name="ctl"> The CTL. </param>
        /// <param name="value"> The value. </param>
        /// <returns> </returns>
        public static bool ValidateWithinRange(this UltraNumericEditor ctl, object value) { return ValidateWithinRange(ctl.NumericType == NumericType.Integer ? InputType.Integer : InputType.Decimal, ctl.MinValue.ToString(), ctl.MaxValue.ToString(), value); }

        public static bool SetValue(this UltraNumericEditor number, double value)
        {
            if(value >= Convert.ToDouble(number.MinValue)  && value <= Convert.ToDouble(number.MaxValue))
            {
                number.Value = value;
                return true;
            }
            
            if (value < Convert.ToDouble(number.MinValue))
            {
                number.Value = number.MinValue;
                return false;
            }

            if (value > Convert.ToDouble(number.MaxValue))
            {
                number.Value = number.MaxValue;
                return false;
            }

            return false;
        }

        public static bool ValidateWithinRange(InputType inputType, string minValue, string maxValue, object answer)
        {
            if(InputType.Integer == inputType)
            {
                int valueInt;
                return (answer != null && answer != DBNull.Value && int.TryParse(answer.ToString(), out valueInt) && valueInt <= int.Parse(maxValue) && valueInt >= int.Parse(minValue));
            }
            decimal valueDec;

            return (answer != null && answer != DBNull.Value && decimal.TryParse(answer.ToString(), out valueDec) && valueDec <= decimal.Parse(maxValue) && valueDec >= decimal.Parse(minValue));
        }

        public static void SelectAllText(this Control control)
        {
            if(control is UltraComboEditor)
                return;
            if(control is TextEditorControlBase)
            {
                ((TextEditorControlBase) control).SelectAll();
                return;
            }

            if(control is UltraWinEditorMaskedControlBase)
            {
                ((UltraWinEditorMaskedControlBase) control).SelectAll();
                return;
            }

            if(control is TextBoxBase)
                ((TextBox) control).SelectAll();
        }

        public static string GetBindingProperty(Control control)
        {
            // NOTE: For Infragistics.Win.UltraWinEditors.UltraDateTimeEditor do not use DateTime because that value does not synch with BindingSource
            // if(control is Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)
            //    return "DateTime";
            if(control is UltraCheckEditor)
                return "Checked";
            if(control is UltraCurrencyEditor)
                return "ValueObject";
            if(control is UltraLabel)
                return "Text";
            if (control is UltraOptionSet)
                return "Value";
            if (control is UltraGroupBox)
                return "Text";
            //if(control is UltraTextEditor || control is UltraNumericEditor || control is UltraComboEditor)
            return "Value";
        }

        public static string GetBindingProperty(Component control)
        {
            //NOTE: For Infragistics.Win.UltraWinEditors.UltraDateTimeEditor do not use DateTime because that value does not synch with BindingSource
            //if(control is Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)
            //    return "DateTime";
            if(control is UltraCheckEditor)
                return "Checked";
            if(control is UltraCurrencyEditor)
                return "ValueObject";

            //if(control is UltraTextEditor || control is UltraNumericEditor || control is UltraComboEditor)
            return "Value";
        }

        /// <summary>
        ///     Binds the object datasource's property to the specified controls default bindable property.
        /// </summary>
        /// <param name="control"> The control. </param>
        /// <param name="dataSource"> The data source. </param>
        /// <param name="propertyName"> Name of the property. </param>
        public static void Bind(this Control control, object dataSource, string propertyName) { control.DataBindings.Add(new Binding(GetBindingProperty(control), dataSource, propertyName, true)); }

        /// <summary>
        ///     Saves the control binding values back to the object datasource.
        /// </summary>
        /// <param name="control"> The control. </param>
        public static void Save(this Control control)
        {
            foreach(Binding db in control.DataBindings.OfType <Binding>())
                db.WriteValue();
        }

        /// <summary>
        ///     Prevents the mouse wheel scrolling from changing the selection in a combobox.
        /// </summary>
        /// <param name="cbo"> The cbo. </param>
        public static void PreventMouseWheelScrolling(this UltraComboEditor cbo)
        {
            MouseEventHandler meh = (s, e) =>
                                    {
                                        var handledArgs = e as HandledMouseEventArgs;
                                        if(handledArgs != null)
                                            handledArgs.Handled = true;
                                    };

            cbo.ControlAdded += (s, e) =>
                                {
                                    var textBox = e.Control as TextBox;
                                    if(textBox != null)
                                        textBox.MouseWheel += meh;
                                };

            cbo.ControlRemoved += (s, e) =>
                                  {
                                      var textBox = e.Control as TextBox;
                                      if(textBox != null)
                                          textBox.MouseWheel -= meh;
                                  };
        }

        /// <summary>
        /// Performs a busy wait until the control's handle has been created.
        /// </summary>
        /// <param name="control"></param>
        public static void WaitForHandleCreation(this Control control)
        {
            var log = LogManager.GetCurrentClassLogger();
            if (control != null)
            {
                while (!control.IsHandleCreated && !control.IsDisposed)
                {
                    log.Info("Handle is not created for main UI. Waiting 2 seconds.");
                    Thread.Sleep(2000);
                }
            }
        }


        #region Nested type: FormatProvider

        public class FormatProvider : IFormatProvider
        {
            #region IFormatProvider Members

            public object GetFormat(Type formatType) { return null; }

            #endregion
        }

        #endregion
    }

    public static class MessageBoxUtilities
    {
        private static readonly UltraMessageBoxManager _manager = new UltraMessageBoxManager {MinimumWidth = 400, MaximumWidth = 800};

        private static UltraMessageBoxInfo CreateInfo(string message, string header, string footer)
        {
            var info = new UltraMessageBoxInfo();
            info.Caption = About.ApplicationName;
            info.Text = message;
            info.Header = header;
            info.Footer = footer;
            info.ShowHelpButton = DefaultableBoolean.False;

            return info;
        }

        public static DialogResult ShowMessageBoxYesOrNo(string message, string header, string footer = null)
        {
            UltraMessageBoxInfo info = CreateInfo(message, header, footer);
            info.Icon = MessageBoxIcon.Question;
            info.Buttons = MessageBoxButtons.YesNo;
            info.Owner = Form.ActiveForm;

            return _manager.ShowMessageBox(info);
        }

        public static DialogResult ShowMessageBoxOKCancel(string message, string header, string footer = null)
        {
            UltraMessageBoxInfo info = CreateInfo(message, header, footer);
            info.Icon = MessageBoxIcon.Question;
            info.Buttons = MessageBoxButtons.OKCancel;
            info.Owner = Form.ActiveForm;

            return _manager.ShowMessageBox(info);
        }

        public static void ShowMessageBoxWarn(string message, string header, string footer = null)
        {
            UltraMessageBoxInfo info = CreateInfo(message, header, footer);
            info.Icon = MessageBoxIcon.Warning;
            info.Buttons = MessageBoxButtons.OK;
            info.Owner = Form.ActiveForm;

            _manager.ShowMessageBox(info);
        }

        public static void ShowMessageBoxOK(string message, string header, string footer = null)
        {
            UltraMessageBoxInfo info = CreateInfo(message, header, footer);
            info.Icon = MessageBoxIcon.Information;
            info.Buttons = MessageBoxButtons.OK;
            info.Owner = Form.ActiveForm;

            _manager.ShowMessageBox(info);
        }

        public static void ShowMessageBoxError(string message, string header, string footer = null)
        {
            UltraMessageBoxInfo info = CreateInfo(message, header, footer);
            info.Icon = MessageBoxIcon.Error;
            info.Buttons = MessageBoxButtons.OK;
            info.Owner = Form.ActiveForm;

            _manager.ShowMessageBox(info);
        }
    }

    public static class WPFUtilities
    {
        public static System.Windows.Media.Color SetTransparency(this System.Windows.Media.Color color, int transparent)
        {
            if(transparent < 1)
                color.A = 0;
            else if(transparent > 0 && transparent < 100)
                color.A = Convert.ToByte(((double) transparent / 100.0) * 255);
            else
                color.A = 255;

            return color;
        }

        public static System.Windows.Media.Imaging.BitmapImage ToWpfImage(this System.Drawing.Image img)
        {
            // If the image's raw format does not have an encoder, default to PNG
            var imgFormat = img.RawFormat;

            if (ImageCodecInfo.GetImageEncoders().All(codec => codec.FormatID != img.RawFormat.Guid))
            {
                imgFormat = ImageFormat.Png;
            }

            var ms = new MemoryStream();  // no using here! BitmapImage will dispose the stream after loading
            img.Save(ms, imgFormat);
            ms.Seek(0, SeekOrigin.Begin);

            var ix = new System.Windows.Media.Imaging.BitmapImage();
            ix.BeginInit();
            ix.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            ix.StreamSource = ms;
            ix.EndInit();

            ix.Freeze();

            return ix;
        }

        public static System.Windows.Media.Imaging.BitmapImage ToWpfImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) 
                return null;

            var image = new System.Windows.Media.Imaging.BitmapImage();
            
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            
            image.Freeze();
            return image;
        }

        /// <summary>
        /// To the WPF image.
        /// </summary>
        /// <param name="imgFileName">Name of the img file in the Images folder 'RunCurrent24.png'.</param>
        /// <returns>System.Windows.Media.Imaging.BitmapImage.</returns>
        public static System.Windows.Media.Imaging.BitmapImage ToWpfImage(string imgFileName)
        {
            var image = new System.Windows.Media.Imaging.BitmapImage();
            
            image.BeginInit();
            image.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat;
            image.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            image.UriSource = new Uri("pack://application:,,,/Resources/images/{0}".FormatWith(imgFileName), UriKind.Absolute);
            image.EndInit();

            image.Freeze();
            return image;
        }
        
        public static void Select(this Infragistics.Windows.DataPresenter.DataRecord record, bool clearSelection = true)
        {
            if(clearSelection)
            {
                record.RecordManager.DataPresenter.ActiveRecord = null;
                record.RecordManager.DataPresenter.SelectedItems.Records.Clear();
            }

            record.RecordManager.DataPresenter.SelectedItems.Records.Add(record);
            record.RecordManager.DataPresenter.ActiveRecord = record;
        }
    }

    public class SortableObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        public SortableObservableCollection(): base()
        {
        }

        public SortableObservableCollection(List<T> list)
            : base(list)
        {
        }

        public SortableObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public void Sort<TKey>(Func<T, TKey> keySelector, System.ComponentModel.ListSortDirection direction)
        {
            if(Items.Count < 1)
                return;

            switch (direction)
            {
                case System.ComponentModel.ListSortDirection.Ascending:
                    {
                        ApplySort(Items.OrderBy(keySelector));
                        break;
                    }
                case System.ComponentModel.ListSortDirection.Descending:
                    {
                        ApplySort(Items.OrderByDescending(keySelector));
                        break;
                    }
            }
        }

        public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            ApplySort(Items.OrderBy(keySelector, comparer));
        }

        private void ApplySort(IEnumerable<T> sortedItems)
        {
            var sortedItemsList = sortedItems.ToList();

            foreach (var item in sortedItemsList)
            {
                Move(IndexOf(item), sortedItemsList.IndexOf(item));
            }
        }
    }

    public static class ApplicationExtensions
    {
        private static readonly System.Windows.Threading.DispatcherOperationCallback exitFrameCallback = ExitFrame;

        private static System.Object ExitFrame(System.Object state)
        {
            var frame = state as System.Windows.Threading.DispatcherFrame;
            // Exit the nested message loop.
            frame.Continue = false;

            return null;
        }

        public static void DoEvents(this System.Windows.Application app)
        {
            // Create new nested message pump.
            var nestedFrame = new System.Windows.Threading.DispatcherFrame();

            // Dispatch a callback to the current message queue, when getting called,
            // this callback will end the nested message loop.
            // note that the priority of this callback should be lower than the that of UI event messages.
            System.Windows.Threading.DispatcherOperation exitOperation = System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(
                                                                                                                                           System.Windows.Threading.DispatcherPriority.Background, exitFrameCallback, nestedFrame);

            // pump the nested message loop, the nested message loop will
            // immediately process the messages left inside the message queue.
            System.Windows.Threading.Dispatcher.PushFrame(nestedFrame);

            // If the "exitFrame" callback doesn't get finished, Abort it.
            if (exitOperation.Status != System.Windows.Threading.DispatcherOperationStatus.Completed)
                exitOperation.Abort();
        }

        /// <summary>
        /// 	Temporarily go to another thread and sleep, then come back using the callback after the specified time.
        /// </summary>
        /// <param name="app"> The app. </param>
        /// <param name="callback"> The callback. </param>
        /// <param name="seconds"> The seconds to sleep on the other thread. </param>
        public static void Sleep(this System.Windows.Application app, System.Action callback, double seconds)
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = System.TimeSpan.FromSeconds(seconds);
            timer.Start();

            timer.Tick += delegate
            {
                timer.Stop();
                app.Dispatcher.BeginInvoke(callback);
            };
        }

        public static void Sleep(this System.Windows.Application app, System.Action<object> callback, object state, double seconds)
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = System.TimeSpan.FromSeconds(seconds);
            timer.Start();

            timer.Tick += delegate
            {
                timer.Stop();
                app.Dispatcher.BeginInvoke(callback, state);
            };
        }
    }
    

    [Serializable]
    public class DataRowProxy
    {
        #region Properties
        
        [XmlArray("ChildDataRows")]
        public List <DataRowProxy> ChildProxies;

        [XmlArray("ColumnNames")]
        public string[] ColumnNameArray;

        [XmlArray("DataRowValues")]
        public object[] ItemArray;

        [XmlAttribute("OriginalPrimaryKey")]
        public string OriginalPrimaryKey;

        [XmlAttribute("ParentRelationName")]
        public string ParentRelation;

        #endregion

        #region Methods
        
        public DataRowProxy(object[] itemArray)
        {
            this.ItemArray = itemArray;
            this.ChildProxies = new List <DataRowProxy>();
        }

        public DataRowProxy() { }

        public override string ToString() { return "Rel:{0}, Children {1}".FormatWith(this.ParentRelation, this.ChildProxies.Count); }

        public List <DataRowProxy> Find(Predicate <DataRowProxy> where)
        {
            var list = new List <DataRowProxy>();

            foreach(var child in ChildProxies)
            {
                if(where(child))
                    list.Add(child);

                list.AddRange(child.Find(where));
            }

            return list;
        }

        public int Remove(Predicate<DataRowProxy> where)
        {
            var removed = 0;
            var toRemove = new List <DataRowProxy>();

            foreach (var child in ChildProxies)
            {
                if(where(child))
                {
                    toRemove.Add(child);
                    removed++;
                }
                else
                    removed += child.Remove(where);
            }

            toRemove.ForEach(r => this.ChildProxies.Remove(r));

            return removed;
        }

        #endregion
    }

    public class DisplayDisabledTooltips : IDisposable
    {
        #region Fields

        private Control _currentToolTipControl;
        private Control _mainControl;
        private UltraToolTipManager _tipManager;

        #endregion

        #region Methods

        public DisplayDisabledTooltips(Control mainControl, UltraToolTipManager tipManager)
        {
            mainControl.MouseMove += mainControl_MouseMove;
            this._tipManager = tipManager;
            this._mainControl = mainControl;
        }

        public void Dispose()
        {
            if(this._mainControl != null)
                this._mainControl.MouseMove -= mainControl_MouseMove;

            this._tipManager = null;
            this._mainControl = null;
        }

        #endregion

        #region Events

        private void mainControl_MouseMove(object sender, MouseEventArgs e)
        {
            var control = this._mainControl.GetChildAtPoint(e.Location) as UltraControlBase;

            if(control != null)
            {
                if(!control.Enabled && this._currentToolTipControl == null)
                {
                    _tipManager.GetUltraToolTip(control); // Prevents exception if control has no tooltip

                    // trigger the tooltip with no delay and some basic positioning just to give you an idea
                    this._tipManager.ShowToolTip(control);
                    this._currentToolTipControl = control;
                }
            }
            else
            {
                if(this._currentToolTipControl != null)
                    this._tipManager.HideToolTip();
                this._currentToolTipControl = null;
            }
        }

        #endregion
    }

    #region Badge Draw Filter

    /// <summary>
    /// The <see cref="T:GeoExPT.EPTBadgeCountManager" /> will draw a custom badge with a count
    /// based on a callback provided onto an Infragistics Button.
    /// Use the <see cref="BadgeCountManager.SharedInstance" /> property to access functionality.
    /// </summary>
    public class BadgeCountManager : IUIElementDrawFilter, IDisposable
    {
        #region Fields

        private static Dictionary<int, Image> _images = new Dictionary<int, Image>();
        private List<BadgeCountItem> _badgeCounters = new List<BadgeCountItem>();
        private UltraToolbarsManager _toolbarManager;

        #endregion

        #region Methods

        /// <summary>
        /// Prevents a default instance of the <see cref="T:GeoExPT.EPTBadgeCountManager" /> class from being created.
        /// </summary>
        /// <param name="toolbar">The toolbar.</param>
        public BadgeCountManager(UltraToolbarsManager toolbar)
        {
            this._toolbarManager = toolbar;
            this._toolbarManager.DrawFilter = this;
        }


        /// <summary>
        /// Adds badge count functionality to a button.
        /// </summary>
        /// <param name="buttonKey">The button key set on the Infragistics tool.</param>
        /// <param name="countProperty">The count property used by the <see cref="T:GeoExPT.EPTBadgeCountManager" /> to query for a count.</param>
        /// <param name="propertyChangedImplementation">The property changed implementation used by the <see cref="T:GeoExPT.EPTBadgeCountManager" />
        /// to monitor changes to the countProperty.</param>
        /// <exception cref="System.ArgumentNullException">When buttonKey is null or empty.</exception>
        public void AddBadgeCount(string buttonKey, Expression<Func<int>> countProperty, INotifyPropertyChanged propertyChangedImplementation = null)
        {
            if (string.IsNullOrEmpty(buttonKey))
                throw new ArgumentNullException("buttonKey");

            Func<int> countPropertyGetter;
            var countPropertyName = GetPropertyNameAndGetter(countProperty, out countPropertyGetter);
            var badgeItem = new BadgeCountItem { GetCount = countPropertyGetter, ToolKey = buttonKey, CountPropertyChanged = propertyChangedImplementation, CountPropertyName = countPropertyName };
            this._badgeCounters.Add(badgeItem);

            if (propertyChangedImplementation != null)
                propertyChangedImplementation.PropertyChanged += OnPropertyChanged;

            RefreshBadge(badgeItem);
        }

        /// <summary>
        /// Removes the badge from the button represented by the key provided.
        /// </summary>
        /// <param name="buttonKey">The button key set on the Infragistics tool.</param>
        /// <exception cref="System.ArgumentNullException">When buttonKey is null or empty.</exception>
        public void RemoveBadge(string buttonKey)
        {
            if (string.IsNullOrEmpty(buttonKey))
                throw new ArgumentNullException("buttonKey");

            var badgeCountItem = this._badgeCounters.FirstOrDefault(i => i.ToolKey == buttonKey);

            if (badgeCountItem != null && badgeCountItem.CountPropertyChanged != null)
            {
                badgeCountItem.CountPropertyChanged.PropertyChanged -= OnPropertyChanged;
                badgeCountItem.GetCount = null; //null out so will redraw with no badge
                RefreshBadge(badgeCountItem);

                badgeCountItem.Dispose();
                this._badgeCounters.Remove(badgeCountItem);
            }
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var badgeItem = this._badgeCounters.FirstOrDefault(item => item.CountPropertyChanged == sender);

            if (badgeItem != null)
                RefreshBadge(badgeItem);
        }

        private void RefreshBadge(BadgeCountItem badgeItem)
        {
            if (badgeItem != null)
            {
                if (badgeItem.UIElement == null) //if UI not cached then refresh entire toolbar
                {
                    this._toolbarManager.BeginUpdate();
                    this._toolbarManager.EndUpdate();
                }
                else
                    badgeItem.UIElement.Invalidate(true);
            }
        }

        private static string GetPropertyNameAndGetter(Expression<Func<int>> property, out Func<int> getter)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new InvalidOperationException("Invalid Body Expression passed.");

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new InvalidOperationException("Member Expression is not a property");

            getter = property.Compile();

            return propertyInfo.Name;
        }

        private static Image GetCountImage(int count)
        {
            if (count > 99)
                count = int.MaxValue;

            if (!_images.ContainsKey(count))
                _images.Add(count, DrawCountImage(count));

            return _images[count];
        }

        private static Image DrawCountImage(int count)
        {
            try
            {
                Bitmap bitmap;

                if (count < 10) //Single Digit
                    bitmap = Properties.Resources.Badge_16.Clone() as Bitmap;
                else if (count < 100) //Double Digit
                    bitmap = Properties.Resources.BadgeBig_16.Clone() as Bitmap;
                else
                {
                    bitmap = Properties.Resources.BadgeMax_16.Clone() as Bitmap;
                    return bitmap; //MAX has default image
                }

                using (var g = Graphics.FromImage(bitmap))
                {
                    using (var font = new Font("Tahoma Bold", 9f))
                    {
                        var rect = count < 10 ? new Rectangle(0, 0, 16, 16) : new Rectangle(0, 0, 20, 16);
                        var stringFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        g.DrawString(count.ToString(), font, Brushes.White, rect, stringFormat);
                    }
                }

                return bitmap;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error creating badge image.");
                return null;
            }
        }

        #endregion

        #region IUIElementDrawFilter

        /// <summary>
        /// Called during the drawing operation of a UIElement for a specific phase
        /// of the operation. This will only be called for the phases returned
        /// from the GetPhasesToFilter method.
        /// </summary>
        /// <param name="drawPhase">Contains a single bit which identifies the current draw phase.</param>
        /// <param name="drawParams">The <see cref="T:Infragistics.Win.UIElementDrawParams" /> used to provide rendering information.</param>
        /// <returns>
        /// Returning true from this method indicates that this phase has been handled and the default processing should be skipped.
        /// </returns>
        public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
        {
            bool returnVal = false;

            var buttonUI = drawParams.Element as ButtonToolUIElement;

            //Ensure on a toolbar not in a submenu
            if (buttonUI != null && buttonUI.Tool != null && buttonUI.Tool.OwnerIsRibbonGroup)
            {
                var badgeItem = this._badgeCounters.FirstOrDefault(item => item.ToolKey == buttonUI.Tool.Key);

                if (badgeItem != null && badgeItem.GetCount != null)
                {
                    badgeItem.UIElement = buttonUI;
                    var count = badgeItem.GetCount();

                    if (count > 0)
                    {
                        var useBiggerRect = count > 9 && count < 100;
                        var countImage = GetCountImage(count);
                        var buttonRect = buttonUI.Rect;

                        drawParams.AppearanceData.ImageHAlign = HAlign.Right;
                        buttonRect.X = useBiggerRect ? buttonRect.Right - 24 : buttonRect.Right - 20;
                        buttonRect.Width = useBiggerRect ? 20 : 16;
                        buttonRect.Height = 16;
                        drawParams.DrawImage(countImage, buttonRect, true, new ImageAttributes());

                        returnVal = true;
                    }
                }
            }

            return returnVal;
        }

        /// <summary>
        /// Called before each element is about to be drawn.
        /// </summary>
        /// <param name="drawParams">The <see cref="T:Infragistics.Win.UIElementDrawParams" /> used to provide rendering information.</param>
        /// <returns>
        /// Bit flags indicating which phases of the drawing operation to filter. The DrawElement method will be called only for those phases.
        /// </returns>
        public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
        {
            return DrawPhase.AfterDrawElement;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDispose)
        {
            if (isDispose)
            {
                if (_images != null)
                {
                    foreach (var image in _images)
                        image.Value.Dispose();

                    _images.Clear();
                    _images = null;
                }
            }
        }

        #endregion

        #region BadgeCountItem

        /// <summary>
        /// A helper class used by the <see cref="T:GeoExPT.EPTBadgeCountManager" /> to organize badges.
        /// </summary>
        private class BadgeCountItem : IDisposable
        {
            /// <summary>
            /// Gets or sets the Infragisitics tool key.
            /// </summary>
            /// <value>
            /// The tool key.
            /// </value>
            public string ToolKey { get; set; }
            /// <summary>
            /// Gets or sets the name of the property used to query for counts displayed on the badge.
            /// </summary>
            /// <value>
            /// The name of the count property.
            /// </value>
            public string CountPropertyName { get; set; }
            /// <summary>
            /// Gets or sets the <see cref="T:System.ComponentModel.INotifyPropertyChanged"/> implementation
            /// that fires updates when the Count Property changes.
            /// </summary>
            /// <value>
            /// The count property changed.
            /// </value>
            public INotifyPropertyChanged CountPropertyChanged { get; set; }
            /// <summary>
            /// Gets or sets the Get-Getter that represents the Count property.
            /// </summary>
            /// <value>
            /// The get count.
            /// </value>
            public Func<int> GetCount { get; set; }
            /// <summary>
            /// Gets or sets the UI element as cached item.
            /// </summary>
            /// <value>The UI element.</value>
            public ButtonToolUIElement UIElement { get; set; }

            public void Dispose()
            {
                ToolKey = null;
                GetCount = null;
                CountPropertyName = null;
                UIElement = null;
            }
        }

        #endregion
    }

    #endregion

    #region Drag And Drop

    /// <summary>
    ///   Helper class to implement drag and drop on a tree. Must implement OnCanDrop and if sorting is enabled, should implement BeforeDrop and AfterDrop.
    /// </summary>
    public class UltraTreeDragAndDropHelper : IDisposable
    {
        #region DropLinePositionEnum enum

        [Flags]
        public enum DropLinePositionEnum
        {
            None = 1,
            OnNode = 2,
            AboveNode = 4,
            BelowNode = 8,
            All = OnNode | AboveNode | BelowNode
        }

        #endregion

        #region Fields

        private TreeDropHighlightFilter _drawFilter;
        private UltraTree _tree;

        public event EventHandler BeforeDrop;
        public event EventHandler<DropEventArgs> AfterDrop;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the function to call to determine the valid states that can be done to the node.
        /// </summary>
        /// <value> The query valid drop on. </value>
        public Func<UltraTreeNode, DropLinePositionEnum> QueryValidDropOn { get; set; }

        public Predicate<string[]> AllowDrag { get; set; }

        public bool AllowOnlyNodeDragNDrop { get; set; }

        #endregion

        #region Methods

        public UltraTreeDragAndDropHelper(UltraTree tree)
        {
            this._tree = tree;
            this._tree.AllowDrop = true;

            this._tree.DragOver += this.tree_DragOver;
            this._tree.DragDrop += this.tree_DragDrop;
            this._tree.QueryContinueDrag += this.tree_QueryContinueDrag;
            this._tree.DragLeave += this.tree_DragLeave;
            this._tree.DragEnter += this.tree_DragEnter;
            this._tree.SelectionDragStart += this.tree_SelectionDragStart;
            
            this._drawFilter = new TreeDropHighlightFilter();
            this._drawFilter.Invalidate += this.DrawFilter_Invalidate;
            this._drawFilter.QueryStateAllowedForNode += this.DrawFilter_QueryStateAllowedForNode;
            this._tree.DrawFilter = this._drawFilter;

            this._tree.Appearances.Add("DropHighLightAppearance");
            this._tree.Appearances["DropHighLightAppearance"].BackColor = Color.Cyan;
        }

        public void Dispose()
        {
            if (this._tree != null)
            {
                this._tree.DragOver -= this.tree_DragOver;
                this._tree.DragDrop -= this.tree_DragDrop;
                this._tree.QueryContinueDrag -= this.tree_QueryContinueDrag;
                this._tree.DragLeave -= this.tree_DragLeave;
                this._tree.SelectionDragStart -= this.tree_SelectionDragStart;
            }

            if (this._drawFilter != null)
            {
                this._drawFilter.Invalidate -= this.DrawFilter_Invalidate;
                this._drawFilter.QueryStateAllowedForNode -= this.DrawFilter_QueryStateAllowedForNode;
                this._drawFilter.Dispose();
            }

            this._tree = null;
            this._drawFilter = null;
        }

        #endregion

        #region Events

        /// <summary>
        ///   This event will fire when the user attempts to drag a node.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="System.EventArgs" /> instance containing the event data. </param>
        private void tree_SelectionDragStart(object sender, EventArgs e)
        {
            //Start a DragDrop operation
            this._tree.DoDragDrop(this._tree.SelectedNodes, DragDropEffects.Move);
        }

        /// <summary>
        ///   The DragDrop event. Here we respond to a Drop on the tree
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="System.Windows.Forms.DragEventArgs" /> instance containing the event data. </param>
        private void tree_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if(this.BeforeDrop != null)
                    this.BeforeDrop(this, EventArgs.Empty);

                UltraTreeNode aNode; //A dummy node variable used for various things
                SelectedNodesCollection SelectedNodes; //The SelectedNodes which will be dropped
                UltraTreeNode DropNode; //The Node to Drop On

                //An integer used for loops
                int i;

                //Set the DropNode
                DropNode = this._drawFilter.DropHightLightNode;

                //Get the Data and put it into a SelectedNodes collection,
                //then clone it and work with the clone
                //These are the nodes that are being dragged and dropped
                SelectedNodes = (SelectedNodesCollection) e.Data.GetData(typeof(SelectedNodesCollection));

                if(SelectedNodes == null)
                    return;

                SelectedNodes = SelectedNodes.Clone() as SelectedNodesCollection;

                //Sort the selected nodes into their visible position. 
                //This is done so that they stay in the same order when
                //they are repositioned. 
                SelectedNodes.SortByPosition();

                //Determine where we are dropping based on the current
                //DropLinePosition of the DrawFilter
                switch(this._drawFilter.DropLinePosition)
                {
                    case DropLinePositionEnum.OnNode: //Drop ON the node
                    {
                        //Loop through the SelectedNodes and reposition
                        //them to the node that was dropped on.
                        //Note that the DrawFilter keeps track of what
                        //node the mouse is over, so we can just use
                        //DropHighLightNode as the drop target. 
                        for(i = 0; i <= (SelectedNodes.Count - 1); i++)
                        {
                            aNode = SelectedNodes[i];
                            aNode.Reposition(DropNode.Nodes);
                        }
                        break;
                    }
                    case DropLinePositionEnum.BelowNode: //Drop Below the node
                    {
                        for(i = 0; i <= (SelectedNodes.Count - 1); i++)
                        {
                            aNode = SelectedNodes[i];
                            aNode.Reposition(DropNode, NodePosition.Next);
                            //Change the DropNode to the node that was just repositioned so that the next added node goes below it. 
                            DropNode = aNode;
                        }
                        break;
                    }
                    case DropLinePositionEnum.AboveNode: //New Index should be the same as the Drop
                    {
                        for(i = 0; i <= (SelectedNodes.Count - 1); i++)
                        {
                            aNode = SelectedNodes[i];
                            aNode.Reposition(DropNode, NodePosition.Previous);
                        }
                        break;
                    }
                }

                if(this.AfterDrop != null)
                    this.AfterDrop(this, new DropEventArgs() {DropNode = DropNode, DragNodes = SelectedNodes, DropType = _drawFilter.DropLinePosition});
            }
            catch(Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn(exc, "Error on drag and drop.");
            }
            finally
            {
                //After the drop is complete, erase the current drop highlight. 
                if(this._drawFilter != null)
                    this._drawFilter.ClearDropHighlight();
            }
        }

        /// <summary>
        ///   Handles the DragOver event of the tree control.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="System.Windows.Forms.DragEventArgs" /> instance containing the event data. </param>
        private void tree_DragOver(object sender, DragEventArgs e)
        {
            var pointInTree = this._tree.PointToClient(new Point(e.X, e.Y)); //Get the position of the mouse in the tree, as opposed to form coords
            var node        = this._tree.GetNodeFromPoint(pointInTree); //Get the node the mouse is over.
            
            //Make sure the mouse is over a node
            if (node == null)
            {
                e.Effect = DragDropEffects.None; //The Mouse is not over a node //Do not allow dropping here
                this._drawFilter.ClearDropHighlight(); //Erase any DropHighlight
                return;
            }
            var formats = e.Data.GetFormats();
            var dropLinePosition = this.QueryValidDropOn(node);

            if (dropLinePosition.HasFlag(DropLinePositionEnum.None))
            {
                this._drawFilter.ClearDropHighlight();
                e.Effect = DragDropEffects.None;
            }
            else
            {
                //It is okay to drop on this node, so tell the DrawFilter where we are by calling SetDropHighlightNode
                this._drawFilter.SetDropHighlightNode(node, pointInTree);
                e.Effect = DragDropEffects.Move; //Allow Dropping here. 
            }
        }

        /// <summary>
        ///   Test to see if we want to continue dragging
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="System.Windows.Forms.QueryContinueDragEventArgs" /> instance containing the event data. </param>
        private void tree_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            //Did the user press escape? 
            if (e.EscapePressed)
            {
                //User pressed escape
                //Cancel the Drag
                e.Action = DragAction.Cancel;
                //Clear the Drop highlight, since we are no longer
                //dragging
                this._drawFilter.ClearDropHighlight();
            }
        }

        /// <summary>
        ///   Occassionally, the DrawFilter will let us know that the control needs to be invalidated.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="System.EventArgs" /> instance containing the event data. </param>
        private void DrawFilter_Invalidate(object sender, EventArgs e)
        {
            //Any time the drophighlight changes, the control needs 
            //to know that it has to repaint. 
            //It would be more efficient to only invalidate the area
            //that needs it, but this works and is very clean.
            this._tree.Invalidate();
        }

        /// <summary>
        ///   This event is fired by the DrawFilter to let us determine what kinds of drops we want to allow on any particular node
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The <see cref="TreeDropHighlightFilter.QueryStateAllowedForNodeEventArgs" /> instance containing the event data. </param>
        private void DrawFilter_QueryStateAllowedForNode(Object sender, TreeDropHighlightFilter.QueryStateAllowedForNodeEventArgs e)
        {
            var dropLinePosition = this.QueryValidDropOn(e.Node);
            e.StatesAllowed = dropLinePosition;
        }

        private void tree_DragEnter(object sender, DragEventArgs e)
        {
            if(AllowOnlyNodeDragNDrop)
            {
                if(!e.Data.GetDataPresent(typeof(SelectedNodesCollection)))
                {
                    this._drawFilter.ClearDropHighlight();
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        /// <summary>
        ///   Fires when the user drags outside the control.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="System.EventArgs" /> instance containing the event data. </param>
        private void tree_DragLeave(object sender, EventArgs e)
        {
            //When the mouse goes outside the control, clear the 
            //drophighlight. 
            //Since the DropHighlight is cleared when the 
            //mouse is not over a node, anyway, 
            //this is probably not needed
            //But, just in case the user goes from a node directly
            //off the control...
            this._drawFilter.ClearDropHighlight();
        }

        #endregion

        #region TreeDropHighlightFilter

        /// <summary>
        ///   This class has to implement the DrawFilter Interface so it can be used as a DrawFilter by the tree
        /// </summary>
        public class TreeDropHighlightFilter : IUIElementDrawFilter
        {
            #region Delegates

            public delegate void QueryStateAllowedForNodeEventHandler(object sender, QueryStateAllowedForNodeEventArgs e);

            #endregion

            private Color mvarDropHighLightBackColor;
            private Color mvarDropHighLightForeColor;

            //The DropHighLightNode is a reference to the node the
            //Mouse is currently over
            private UltraTreeNode mvarDropHighLightNode;
            private Color mvarDropLineColor;

            private DropLinePositionEnum mvarDropLinePosition;

            //The width of the DropLine
            private int mvarDropLineWidth;
            private int mvarEdgeSensitivity;

            public TreeDropHighlightFilter()
            {
                //Initialize the properties to the defaults
                this.InitProperties();
            }

            public UltraTreeNode DropHightLightNode
            {
                get { return this.mvarDropHighLightNode; }
                set
                {
                    //If the Node is being set to the same value,
                    // just exit
                    if (this.mvarDropHighLightNode.Equals(value))
                        return;
                    this.mvarDropHighLightNode = value;
                    //The DropNode has changed.
                    this.PositionChanged();
                }
            }
            public DropLinePositionEnum DropLinePosition
            {
                get { return this.mvarDropLinePosition; }
                set
                {
                    //If the position is the same as it was, 
                    //just exit
                    if (this.mvarDropLinePosition == value)
                        return;
                    this.mvarDropLinePosition = value;
                    //The Drop Position has changed
                    this.PositionChanged();
                }
            }

            public int DropLineWidth
            {
                get { return this.mvarDropLineWidth; }
                set { this.mvarDropLineWidth = value; }
            }

            //The BackColor of the DropHighLight node
            //This only affect the node when it is being dropped On. 
            //Not Above or Below. 
            public Color DropHighLightBackColor
            {
                get { return this.mvarDropHighLightBackColor; }
                set { this.mvarDropHighLightBackColor = value; }
            }

            //The ForeColor of the DropHighLight node
            //This only affect the node when it is being dropped On. 
            //Not Above or Below. 
            public Color DropHighLightForeColor
            {
                get { return this.mvarDropHighLightForeColor; }
                set { this.mvarDropHighLightForeColor = value; }
            }

            //The color of the DropLine
            public Color DropLineColor
            {
                get { return this.mvarDropLineColor; }
                set { this.mvarDropLineColor = value; }
            }

            //Determines how close to the top or bottom edge of a node
            //the mouse must be to be consider dropping Above or Below
            //respectively. 
            //By default the top 1/3 of the node is Above, the bottom 1/3
            //is Below, and the middle is On. 
            public int EdgeSensitivity
            {
                get { return this.mvarEdgeSensitivity; }
                set { this.mvarEdgeSensitivity = value; }
            }

            #region IUIElementDrawFilter Members

            DrawPhase IUIElementDrawFilter.GetPhasesToFilter(ref UIElementDrawParams drawParams)
            {
                return DrawPhase.AfterDrawElement | DrawPhase.BeforeDrawElement;
            }

            //The actual drawing code
            bool IUIElementDrawFilter.DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
            {
                UIElement aUIElement;
                Graphics g;
                UltraTreeNode aNode;

                //If there//s no DropHighlight node or no position
                //specified, then we don//t need to draw anything special. 
                //Just exit Function
                if ((this.mvarDropHighLightNode == null) || (this.mvarDropLinePosition == DropLinePositionEnum.None))
                    return false;

                //Create a new QueryStateAllowedForNodeEventArgs object
                //to pass to the event
                var eArgs = new QueryStateAllowedForNodeEventArgs();

                //Initialize the object with the correct info
                eArgs.Node = this.mvarDropHighLightNode;
                eArgs.DropLinePosition = this.mvarDropLinePosition;

                //Default to all states allowed. 
                eArgs.StatesAllowed = DropLinePositionEnum.All;

                //Raise the event
                this.QueryStateAllowedForNode(this, eArgs);

                //Check to see if the user allowed the current state
                //for this node. If not, exit function
                if ((eArgs.StatesAllowed & this.mvarDropLinePosition) != this.mvarDropLinePosition)
                    return false;

                //Get the element being drawn
                aUIElement = drawParams.Element;

                //Determine which drawing phase we are in. 
                switch (drawPhase)
                {
                    case DrawPhase.BeforeDrawElement:
                        {
                            //We are in BeforeDrawElement, so we are only concerned with 
                            //drawing the OnNode state. 
                            if ((this.mvarDropLinePosition & DropLinePositionEnum.OnNode) == DropLinePositionEnum.OnNode)
                            {
                                //Check to see if we are drawing a NodeTextUIElement
                                if (aUIElement.GetType() == typeof(NodeTextUIElement))
                                {
                                    //Get a reference to the node that this
                                    //NodeTextUIElement is associated with
                                    aNode = (UltraTreeNode)aUIElement.GetContext(typeof(UltraTreeNode));

                                    //See if this is the DropHighlightNode
                                    if (aNode.Equals(this.mvarDropHighLightNode))
                                    {
                                        //Set the ForeColor and Backcolor of the node 
                                        //to the DropHighlight colors 
                                        //Note that AppearanceData only affects the
                                        //node for this one paint. It will not
                                        //change any properties of the node
                                        drawParams.AppearanceData.BackColor = this.mvarDropHighLightBackColor;
                                        drawParams.AppearanceData.ForeColor = this.mvarDropHighLightForeColor;
                                    }
                                }
                            }
                            break;
                        }
                    case DrawPhase.AfterDrawElement:
                        {
                            //We're in AfterDrawElement
                            //So the only states we are conderned with are
                            //Below and Above
                            //Check to see if we are drawing the Tree Element
                            if (aUIElement.GetType() == typeof(UltraTreeUIElement))
                            {
                                //Declare a pen to us for drawing Droplines
                                var p = new Pen(this.mvarDropLineColor, this.mvarDropLineWidth);

                                //Get a reference to the Graphics object
                                //we are drawing to. 
                                g = drawParams.Graphics;

                                //Get the NodeSelectableAreaUIElement for the 
                                //current DropNode. We will use this for
                                //positioning and sizing the DropLine
                                NodeSelectableAreaUIElement tElement;
                                tElement = (NodeSelectableAreaUIElement)drawParams.Element.GetDescendant(typeof(NodeSelectableAreaUIElement), this.mvarDropHighLightNode);

                                //The left edge of the DropLine
                                int LeftEdge = tElement.Rect.Left - 4;

                                //We need a reference to the control to 
                                //determine the right edge of the line
                                UltraTree aTree;
                                aTree = (UltraTree)tElement.GetContext(typeof(UltraTree));
                                int RightEdge = aTree.DisplayRectangle.Right - 4;

                                //Used to store the Vertical position of the 
                                //DropLine
                                int LineVPosition;

                                if ((this.mvarDropLinePosition & DropLinePositionEnum.AboveNode) == DropLinePositionEnum.AboveNode)
                                {
                                    //Draw line above node
                                    LineVPosition = this.mvarDropHighLightNode.Bounds.Top;
                                    g.DrawLine(p, LeftEdge, LineVPosition, RightEdge, LineVPosition);
                                    p.Width = 1;
                                    g.DrawLine(p, LeftEdge, LineVPosition - 3, LeftEdge, LineVPosition + 2);
                                    g.DrawLine(p, LeftEdge + 1, LineVPosition - 2, LeftEdge + 1, LineVPosition + 1);
                                    g.DrawLine(p, RightEdge, LineVPosition - 3, RightEdge, LineVPosition + 2);
                                    g.DrawLine(p, RightEdge - 1, LineVPosition - 2, RightEdge - 1, LineVPosition + 1);
                                }
                                if ((this.mvarDropLinePosition & DropLinePositionEnum.BelowNode) == DropLinePositionEnum.BelowNode)
                                {
                                    //Draw Line below node
                                    LineVPosition = this.mvarDropHighLightNode.Bounds.Bottom;
                                    g.DrawLine(p, LeftEdge, LineVPosition, RightEdge, LineVPosition);
                                    p.Width = 1;
                                    g.DrawLine(p, LeftEdge, LineVPosition - 3, LeftEdge, LineVPosition + 2);
                                    g.DrawLine(p, LeftEdge + 1, LineVPosition - 2, LeftEdge + 1, LineVPosition + 1);
                                    g.DrawLine(p, RightEdge, LineVPosition - 3, RightEdge, LineVPosition + 2);
                                    g.DrawLine(p, RightEdge - 1, LineVPosition - 2, RightEdge - 1, LineVPosition + 1);
                                }
                            }
                            break;
                        }
                }
                return false;
            }

            #endregion

            public event EventHandler Invalidate;

            public event QueryStateAllowedForNodeEventHandler QueryStateAllowedForNode;

            private void InitProperties()
            {
                this.mvarDropHighLightNode = null;
                this.mvarDropLinePosition = DropLinePositionEnum.None;
                this.mvarDropHighLightBackColor = SystemColors.Highlight;
                this.mvarDropHighLightForeColor = SystemColors.HighlightText;
                this.mvarDropLineColor = SystemColors.ControlText;
                this.mvarEdgeSensitivity = 0;
                this.mvarDropLineWidth = 2;
            }

            //Clean up
            public void Dispose()
            {
                this.mvarDropHighLightNode = null;
            }

            //When the DropNode or DropPosition change, we fire the
            //Invalidate event to let the program know to invalidate
            //the Tree control. 
            //This is neccessary since the DrawFilter does not have a 
            //reference to the Tree Control (although it probably could)
            private void PositionChanged()
            {
                // if nobody is listening then just return
                //
                if (null == this.Invalidate)
                    return;

                EventArgs e = EventArgs.Empty;

                this.Invalidate(this, e);
            }

            //Set the DropNode to Nothing and the position to None. This
            //Will clear whatever Drophighlight is in the tree
            public void ClearDropHighlight()
            {
                this.SetDropHighlightNode(null, DropLinePositionEnum.None);
            }

            //Call this proc every time the DragOver event of the 
            //Tree fires. 
            //Note that the point pass in MUST be in Tree coords, not
            //form coords
            public void SetDropHighlightNode(UltraTreeNode Node, Point PointInTreeCoords)
            {
                //The distance from the edge of the node used to 
                //determine whether to drop Above, Below, or On a node
                int DistanceFromEdge;

                //The new DropLinePosition
                DropLinePositionEnum NewDropLinePosition;

                DistanceFromEdge = this.mvarEdgeSensitivity;
                //Check to see if DistanceFromEdge is 0
                if (DistanceFromEdge == 0)
                {
                    //It is, so we use the default value - one third. 
                    DistanceFromEdge = Node.Bounds.Height / 3;
                }

                //Determine which part of the node the point is in
                if (PointInTreeCoords.Y < (Node.Bounds.Top + DistanceFromEdge))
                {
                    //Point is in the top of the node
                    NewDropLinePosition = DropLinePositionEnum.AboveNode;
                }
                else
                {
                    if (PointInTreeCoords.Y > ((Node.Bounds.Bottom - DistanceFromEdge) - 1))
                    {
                        //Point is in the bottom of the node
                        NewDropLinePosition = DropLinePositionEnum.BelowNode;
                    }
                    else
                    {
                        //Point is in the middle of the node
                        NewDropLinePosition = DropLinePositionEnum.OnNode;
                    }
                }

                //Now that we have the new DropLinePosition, call the
                //real proc to get things rolling
                SetDropHighlightNode(Node, NewDropLinePosition);
            }

            private void SetDropHighlightNode(UltraTreeNode Node, DropLinePositionEnum DropLinePosition)
            {
                //Use to store whether there have been any changes in 
                //DropNode or DropLinePosition
                bool IsPositionChanged = false;

                try
                {
                    //Check to see if the nodes are equal and if 
                    //the dropline position are equal
                    if (this.mvarDropHighLightNode != null && this.mvarDropHighLightNode.Equals(Node) && (this.mvarDropLinePosition == DropLinePosition))
                    {
                        //They are both equal. Nothing has changed. 
                        IsPositionChanged = false;
                    }
                    else
                    {
                        //One or both have changed
                        IsPositionChanged = true;
                    }
                }
                catch
                {
                    //If we reach this code, it means mvarDropHighLightNode 
                    //is null, so it could not be compared
                    if (this.mvarDropHighLightNode == null)
                    {
                        //Check to see if Node is nothing, so we//ll know
                        //if Node = mvarDropHighLightNode
                        IsPositionChanged = !(Node == null);
                    }
                }

                //Set both properties without calling PositionChanged
                this.mvarDropHighLightNode = Node;
                this.mvarDropLinePosition = DropLinePosition;

                //Check to see if the PositionChanged
                if (IsPositionChanged)
                {
                    //Position did change.
                    this.PositionChanged();
                }
            }

            #region Nested type: QueryStateAllowedForNodeEventArgs

            public class QueryStateAllowedForNodeEventArgs : EventArgs
            {
                public DropLinePositionEnum DropLinePosition;
                public UltraTreeNode Node;
                public DropLinePositionEnum StatesAllowed;
            }

            #endregion

            //Only need to trap for 2 phases:
            //AfterDrawElement: for drawing the DropLine
            //BeforeDrawElement: for drawing the DropHighlight
        }

        #endregion

        public class DropEventArgs : EventArgs
        {
            public DropLinePositionEnum DropType { get; set; }
            public UltraTreeNode DropNode { get; set; }
            public SelectedNodesCollection DragNodes { get; set; }
        }
    }

    #endregion
}