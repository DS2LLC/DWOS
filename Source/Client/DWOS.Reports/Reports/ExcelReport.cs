using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infragistics.Documents.Excel;

namespace DWOS.Reports.Reports
{
    /// <summary>
    /// Generic Excel report where you can specify contents.
    /// </summary>
    public class ExcelReport : ExcelBaseReport
    {
        #region Properties

        public override string Title { get; }

        public ICollection<ReportTable> Tables { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelReport"/> class.
        /// </summary>
        /// <param name="title">The worksheet title.</param>
        /// <param name="tables">Tables to include in the report.</param>
        public ExcelReport(string title, ICollection<ReportTable> tables)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (tables == null)
            {
                throw new ArgumentNullException(nameof(tables));
            }

            Title = title;
            Tables = tables;
        }

        protected override void CreateReport(System.Threading.CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            CreateWorkBook();
            foreach (var table in Tables)
            {
                var formattingOptions = table.FormattingOptions ?? TableFormattingOptions.Default;

                var worksheet = CreateWorksheet(table.Name);

                var rowIndex = 0;
                if (table.IncludeCompanyHeader)
                {
                    rowIndex = AddCompanyHeaderRows(worksheet, table.Columns.Count - 1, table.Name, table.Title ?? $"{Title}-{table.Name}");
                    rowIndex += 2;
                }

                var tableRowIndex = rowIndex;

                var headerColIndex = 0;
                foreach (var header in table.Columns ?? Enumerable.Empty<Column>())
                {
                    CreateHeaderCell(worksheet, rowIndex, headerColIndex, header.Name);
                    worksheet.Columns[headerColIndex].Width = header.Width * 256;
                    headerColIndex++;
                }

                rowIndex++;

                foreach (var row in table.Rows ?? Enumerable.Empty<Row>())
                {
                    var colIndex = 0;
                    foreach (var obj in row.Cells)
                    {
                        var currentCell = worksheet.Rows[rowIndex].Cells[colIndex];

                        if (formattingOptions.BorderAroundCells)
                        {
                            FormatBorder(currentCell, CellBorderLineStyle.Thin);
                        }

                        if (row.BackgroundColor.HasValue)
                        {
                            currentCell.CellFormat.Fill = CellFill.CreateSolidFill(row.BackgroundColor.Value);
                        }

                        if (row.ForegroundColor.HasValue)
                        {
                            currentCell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(row.ForegroundColor.Value);
                        }

                        if (row.IsBold)
                        {
                            currentCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                        }

                        if (obj is Cell formattedCell)
                        {
                            currentCell.Value = formattedCell.Value;

                            if (formattedCell.BackgroundColor.HasValue)
                            {
                                currentCell.CellFormat.Fill = CellFill.CreateSolidFill(formattedCell.BackgroundColor.Value);
                            }

                            if (formattedCell.ForegroundColor.HasValue)
                            {
                                currentCell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(formattedCell.ForegroundColor.Value);
                            }

                            if (formattedCell.IsBold)
                            {
                                currentCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                            }
                        }
                        else
                        {
                            currentCell.Value = obj;
                        }

                        colIndex++;
                    }

                    rowIndex++;
                }

                if (table.UseExcelTable)
                {
                    CreateTable(worksheet, tableRowIndex + 1, headerColIndex - 1, rowIndex, true);
                }
            }
        }

        #endregion

        #region ReportTable

        /// <summary>
        /// Represents a table (worksheet) in an <see cref="ExcelReport"/>.
        /// </summary>
        public class ReportTable
        {
            #region Properties

            /// <summary>
            /// Gets or sets the name of this instance.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the columns of this instance.
            /// </summary>
            public ICollection<Column> Columns { get; set; }

            /// <summary>
            /// Gets or sets the rows of this instance.
            /// </summary>
            public ICollection<Row> Rows { get; set; }

            /// <summary>
            /// Gets or sets a value indicating if the table needs to include
            /// a company header.
            /// </summary>
            /// <value>
            /// <c>true</c> if the company header should be included;
            /// otherwise, <c>false</c>.
            /// </value>
            public bool IncludeCompanyHeader { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating if the report should use an
            /// Excel table (with filtering options) for data.
            /// </summary>
            /// <value>
            /// <c>true</c> if an Excel table should be used;
            /// otherwise, <c>false</c>.
            /// </value>
            public bool UseExcelTable { get; set; }

            /// <summary>
            /// Gets or sets formatting options for this table.
            /// </summary>
            public TableFormattingOptions FormattingOptions { get; set; }

            /// <summary>
            /// Gets or sets the optional title for this table.
            /// </summary>
            public string Title { get; set; }

            #endregion
        }

        #endregion

        #region Column

        /// <summary>
        /// Represents a column in a <see cref="ReportTable"/>.
        /// </summary>
        public class Column
        {
            #region Properties

            /// <summary>
            /// Gets the name of this instance.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Gets the character width of this instance.
            /// </summary>
            public int Width { get; set; } = 20;

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the <see cref="Column"/> class.
            /// </summary>
            /// <param name="name"></param>
            public Column(string name)
            {
                Name = name;
            }

            #endregion
        }

        #endregion

        #region Row

        /// <summary>
        /// Represents a row in a <see cref="ReportTable"/>.
        /// </summary>
        public class Row
        {
            #region Properties

            /// <summary>
            /// Gets or sets the cells in this instance.
            /// </summary>
            public ICollection Cells { get; set; }

            /// <summary>
            /// Gets or sets the optional background color for this instance.
            /// </summary>
            public System.Drawing.Color? BackgroundColor { get; set; }

            /// <summary>
            /// Gets or sets the optional foreground color for this instance.
            /// </summary>
            public System.Drawing.Color? ForegroundColor { get; set; }

            /// <summary>
            /// Gets or sets a value indicating if the row should be
            /// formatted to be bold.
            /// </summary>
            /// <value>
            /// <c>true</c> if the row is bold; otherwise, <c>false</c>.
            /// </value>
            public bool IsBold { get; set; }

            #endregion
        }

        #endregion

        #region Cell

        /// <summary>
        /// Represents a cell (with formatting information) in a <see cref="Row"/>.
        /// </summary>
        public class Cell
        {
            #region Properties

            public object Value { get; }

            /// <summary>
            /// Gets or sets the optional background color for this instance.
            /// </summary>
            public System.Drawing.Color? BackgroundColor { get; set; }

            /// <summary>
            /// Gets or sets the optional foreground color for this instance.
            /// </summary>
            public System.Drawing.Color? ForegroundColor { get; set; }

            /// <summary>
            /// Gets or sets a value indicating if the row should be
            /// formatted to be bold.
            /// </summary>
            /// <value>
            /// <c>true</c> if the row is bold; otherwise, <c>false</c>.
            /// </value>
            public bool IsBold { get; set; }

            #endregion

            #region Methods

            public Cell(object value)
            {
                Value = value;
            }

            #endregion


        }

        #endregion

        #region TableFormattingOptions

        public class TableFormattingOptions
        {
            public bool BorderAroundCells { get; set; }

            public static TableFormattingOptions Default => new TableFormattingOptions();
        }

        #endregion
    }
}
