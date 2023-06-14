using DWOS.Data;
using DWOS.Shared.Utilities;
using Infragistics.Documents.Excel;
using Microsoft.Win32;
using NLog;
using System;
using System.Data;

namespace DWOS.DataImporter
{
    public abstract class DataExchange
    {
        #region Fields

        protected static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public IUserNotifier Notifier
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public DataExchange(IUserNotifier notifier)
        {
            if (notifier == null)
            {
                throw new ArgumentNullException(nameof(notifier));
            }

            Notifier = notifier;
        }

        protected bool IsValid(DataRow row)
        {
            try
            {
                row.EndEdit();
                return !row.HasErrors;
            }
            catch
            {
                return false;
            }
        }
        
        public void Export()
        {
            var saveFile = new SaveFileDialog();
            saveFile.Filter = "xls files|*.xlsx";

            if(saveFile.ShowDialog().GetValueOrDefault())
            {
                Export(saveFile.FileName);
            }
        }

        public void Import()
        {
            var openFile = new OpenFileDialog();
            openFile.Filter = "xls files|*.xlsx;*.xls;*.csv";

            if (openFile.ShowDialog().GetValueOrDefault())
            {
                Import(openFile.FileName);
            }
        }

        protected abstract void Export(string fileFolder);

        protected abstract void Import(string file);

        protected Workbook ExportToWorkbook(DataTable table)
        {
            try
            {
                var _workbook = new Workbook(WorkbookFormat.Excel2007);
                _workbook.DocumentProperties.Company = ApplicationSettings.Current.CompanyName;
                _workbook.DocumentProperties.Title = table.TableName;
                
                var title = table.TableName.TrimToMaxLength(31);

                var worksheet = _workbook.Worksheets.Add(title);
                worksheet.PrintOptions.Orientation = Orientation.Landscape;
                worksheet.DefaultColumnWidth = 5000;

                int currentColumn = 0;
                foreach (DataColumn column in table.Columns)
                {
                    worksheet.Rows[0].Cells[currentColumn].Value = column.Caption;

                    currentColumn++;
                }

                FormatHeader(worksheet.Rows[0]);

                int currentRowIndex = 1;

                foreach (DataRow row in table.Rows)
                {
                    int currentCellIndex = 0;

                    foreach (DataColumn col in table.Columns)
                    {
                        FormatBorder(worksheet.Rows[currentRowIndex].Cells[currentCellIndex], CellBorderLineStyle.Thin);
                        worksheet.Rows[currentRowIndex].Cells[currentCellIndex].CellFormat.FormatString = GetCellFormatByType(col.DataType);

                        if (!row.IsNull(col))
                            worksheet.Rows[currentRowIndex].Cells[currentCellIndex].Value = row[col.ColumnName];

                        currentCellIndex++;
                    }

                    currentRowIndex++;
                }

                return _workbook;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error creating excel document.");
                return null;
            }
        }

        #region Excel Formatting

        private  string GetCellFormatByType(Type t)
        {
            if (t == typeof(DateTime))
                return "m/d/yyyy";

            if (t == typeof(Decimal))
                return "#,##0";

            return null;
        }

        private void FormatHeader(WorksheetRow row, CellBorderLineStyle borderStyle = CellBorderLineStyle.Thin)
        {
            FormatBorder(row, borderStyle);

            row.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            row.CellFormat.Alignment = HorizontalCellAlignment.Center;
            row.CellFormat.Font.Name = "Verdana";
            row.CellFormat.Font.Height = 10 * 20; //Font size height is measured in twips. One twips is 1/20th of a point
            row.CellFormat.Fill = CellFill.CreateSolidFill(new WorkbookColorInfo(WorkbookThemeColorType.Accent1, 0));
        }

        private void FormatBorder(WorksheetRow row, CellBorderLineStyle borderStyle)
        {
            row.CellFormat.BottomBorderStyle = borderStyle;
            row.CellFormat.LeftBorderStyle = borderStyle;
            row.CellFormat.RightBorderStyle = borderStyle;
            row.CellFormat.TopBorderStyle = borderStyle;

            row.CellFormat.Font.Name = "Verdana";
            row.CellFormat.Font.Height = 10 * 20; //Font size height is measured in twips. One twips is 1/20th of a point
        }

        private void FormatBorder(WorksheetCell cell, CellBorderLineStyle borderStyle, HorizontalCellAlignment horizontalAlign = HorizontalCellAlignment.Left)
        {
            cell.CellFormat.BottomBorderStyle = borderStyle;
            cell.CellFormat.LeftBorderStyle = borderStyle;
            cell.CellFormat.RightBorderStyle = borderStyle;
            cell.CellFormat.TopBorderStyle = borderStyle;

            cell.CellFormat.Alignment = horizontalAlign;
            cell.CellFormat.Font.Name = "Verdana";
            cell.CellFormat.Font.Height = 10 * 20; //Font size height is measured in twips. One twips is 1/20th of a point
        }

        #endregion

        #endregion
    }
}