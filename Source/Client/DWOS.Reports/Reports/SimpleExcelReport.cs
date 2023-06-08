using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using System;
using System.Data;
using System.Threading;

namespace DWOS.Reports
{
    public class SimpleExcelReport : ExcelBaseReport
    {
        #region Fields

        private DataTable _table;
        private string _title;

        #endregion

        #region Properties

        public override string Title
        {
            get { return this._title; }
        }

        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Landscape; }
        }

        public Func <DataColumn, string> GetColumnFormat { get; set; }

        public Func <DataColumn, bool> GetColumnHidden { get; set; }

        #endregion

        #region Methods

        public SimpleExcelReport(string title, DataTable dt)
        {
            this._title = title;
            this._table = dt;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            try
            {
                CreateWorkBook();
                var worksheet = CreateWorksheet(this._table.TableName);
                
                worksheet.DefaultColumnWidth = 5000;

                int currentColumn = 0;
                foreach(DataColumn column in this._table.Columns)
                {
                    worksheet.Rows[0].Cells[currentColumn].Value = column.Caption;

                    currentColumn++;
                }

                FormatHeader(worksheet.Rows[0]);

                int currentRowIndex = 1;

                foreach(DataRow row in this._table.Rows)
                {
                    int currentCellIndex = 0;

                    foreach(DataColumn col in this._table.Columns)
                    {
                        FormatBorder(worksheet.Rows[currentRowIndex].Cells[currentCellIndex], CellBorderLineStyle.Thin);
                        worksheet.Rows[currentRowIndex].Cells[currentCellIndex].CellFormat.FormatString = GetColumnFormat == null ? GetCellFormatByType(col.DataType) : GetColumnFormat(col);

                        if(!row.IsNull(col))
                            worksheet.Rows[currentRowIndex].Cells[currentCellIndex].Value = row[col.ColumnName];

                        currentCellIndex++;
                    }

                    currentRowIndex++;
                }

                if(GetColumnHidden != null)
                {
                    foreach(DataColumn col in this._table.Columns)
                        worksheet.Columns[col.Ordinal].Hidden = GetColumnHidden(col);
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error creating excel document.");
            }
        }

        #endregion
    }
}