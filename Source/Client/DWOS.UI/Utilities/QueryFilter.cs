using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace DWOS.UI.Utilities
{
    public partial class QueryFilter : Form
    {
        #region Fields

        private const string COLUMN_COL = "Column";
        private const string OPERATOR_COL = "Operator";
        private const string VALUE_COL = "Value";
        private const string DATATYPE_COL = "DataType";

        private const string OPER_EQUAL = "=";
        private const string OPER_NOTEQUAL = "<>";
        private const string OPER_LESSTHAN = "<";
        private const string OPER_GREATERTHAN = ">";
        private DataTable _dtFilter;
        private DataTable _dtInput;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the filter sql WHERE clause.
        /// </summary>
        /// <value> The filter. </value>
        public string Filter
        {
            get
            {
                this.grdFilter.UpdateData();
                var sb = new StringBuilder();

                foreach(DataRow row in this._dtFilter.Rows)
                {
                    if(!row.IsNull(VALUE_COL) && !row.IsNull(OPERATOR_COL))
                    {
                        if(sb.Length > 0)
                            sb.Append(" AND ");

                        if(row[COLUMN_COL].ToString() == "string")
                            sb.AppendFormat(" {0} {1} '{2}'", row[COLUMN_COL], row[OPERATOR_COL], row[VALUE_COL]);
                        else if(row[COLUMN_COL].ToString() == "datetime")
                            sb.AppendFormat(" {0} {1} '{2}'", row[COLUMN_COL], row[OPERATOR_COL], row[VALUE_COL]);
                        else
                            sb.AppendFormat(" {0} {1} {2}", row[COLUMN_COL], row[OPERATOR_COL], row[VALUE_COL]);
                    }
                }

                return sb.ToString();
            }
        }

        #endregion

        #region Methods

        public QueryFilter() { InitializeComponent(); }

        public void LoadFilter(DataTable dtInput)
        {
            this._dtFilter = new DataTable();
            this._dtFilter.Columns.Add(COLUMN_COL, typeof(string));
            this._dtFilter.Columns.Add(OPERATOR_COL, typeof(string));
            this._dtFilter.Columns.Add(VALUE_COL, typeof(string));
            this._dtFilter.Columns.Add(DATATYPE_COL, typeof(string));

            this._dtInput = dtInput;

            foreach(DataColumn column in this._dtInput.Columns)
            {
                if(!column.AutoIncrement)
                    this._dtFilter.Rows.Add(column.Caption, null, null, column.DataType.ToString());
            }

            this.grdFilter.DataSource = this._dtFilter;
        }

        private string[] GetOperatorValues(string dataType)
        {
            switch(dataType)
            {
                case "System.String":
                case "System.Boolean":
                    return new[] {OPER_EQUAL, OPER_NOTEQUAL};
                case "System.Int32":
                case "System.Decimal":
                case "System.DateTime":
                    return new[] {OPER_EQUAL, OPER_NOTEQUAL, OPER_LESSTHAN, OPER_GREATERTHAN};
                default:
                    break;
            }

            return new string[] {};
        }

        #endregion

        #region Events

        private void grdFilter_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if(!e.ReInitialize)
            {
                //create operator valuelist based on data type
                var operatorVL = new ValueList();

                foreach(string item in GetOperatorValues(e.Row.Cells[DATATYPE_COL].Value.ToString()))
                    operatorVL.ValueListItems.Add(item);

                e.Row.Cells[OPERATOR_COL].ValueList = operatorVL;
                e.Row.Cells[OPERATOR_COL].ValueList.MoveFirstItemLastItem(false);

                //create list of values if their is a lookup table
                string column = e.Row.Cells[COLUMN_COL].Value.ToString();
                var valueValues = new List <ValueListItem>();

                foreach(DataRelation item in this._dtInput.ParentRelations)
                {
                    if(item.ChildColumns[0].ColumnName == column)
                    {
                        if(item.ParentTable is IDomainTable)
                        {
                            string displayColumn = ((IDomainTable) item.ParentTable).DisplayColumn;
                            string valueColumn = item.ParentColumns[0].ColumnName;

                            foreach(DataRow childRow in item.ParentTable.Rows)
                            {
                                if(!childRow.IsNull(valueColumn) && !childRow.IsNull(displayColumn))
                                    valueValues.Add(new ValueListItem(childRow[valueColumn], childRow[displayColumn].ToString()));
                            }
                        }

                        break;
                    }
                }

                if(valueValues.Count > 0)
                {
                    var valuesVL = new ValueList();

                    foreach(ValueListItem valueValue in valueValues)
                        valuesVL.ValueListItems.Add(valueValue);

                    e.Row.Cells[VALUE_COL].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
                    e.Row.Cells[VALUE_COL].ValueList = valuesVL;
                    e.Row.Cells[VALUE_COL].ValueList.MoveFirstItemLastItem(false);
                }
                else
                    e.Row.Cells[VALUE_COL].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
            }
        }

        #endregion
    }
}