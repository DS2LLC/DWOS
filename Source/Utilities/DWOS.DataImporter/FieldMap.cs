using Infragistics.Documents.Excel;
using System.Runtime.Serialization;

namespace DWOS.DataImporter
{
    [DataContract]
    public class FieldMap
    {
        #region Properties

        [DataMember]
        public string ColumnName { get; set; }
        [DataMember]
        public int ColumnIndex { get; set; }

        #endregion

        #region Methods

        public static FieldMap CreateFieldMap(Worksheet workSheet, string columnName)
        {
            //use first row as column headers
            foreach (var cell in workSheet.Rows[0].Cells)
            {
                if (cell.Value == null)
                    break;

                if (columnName == cell.Value.ToString())
                {
                    return new FieldMap() { ColumnName = columnName, ColumnIndex = cell.ColumnIndex };
                }
            }

            return null;
        }

        #endregion
    }
}