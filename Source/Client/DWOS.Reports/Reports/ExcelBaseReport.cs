using DWOS.Data;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

namespace DWOS.Reports
{
    public abstract class ExcelBaseReport : Report
    {
        #region Fields

        public const string DURATION_FORMAT = "#,##0.0";
        public const string DATE_FORMAT = "m/d/yyyy";
        public const string TIME_FORMAT = "[$-409]h:mm AM/PM;@";
        public const string PERCENT_FORMAT = "0.00%";
        public const string NUMBER_FORMAT_DEC2 = "#,##0.00";
        public const string NUMBER_FORMAT = "#,##0";
        private const int WORKSHEET_MAX_LENGTH = 31;
        protected Workbook _workbook;
        private int _numberOfCopies = 1;

        #endregion

        #region Properties

        public static string MONEY_FORMAT
        {
            get
            {
                return "\\$###,##0." + "0".Repeat(ApplicationSettings.Current.PriceDecimalPlaces);
            }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Landscape; }
        }

        protected string DefaultFontName { get; set; }

        /// <summary>
        ///     Gets or sets the default size of the font in points.
        /// </summary>
        /// <value>The default size of the font.</value>
        protected int DefaultFontSize { get; set; }

        protected System.Drawing.Color DefaultHeaderBackgroundColor { get; set; }

        protected System.Drawing.Color DefaultAltRowBackgroundColor { get; set; }

        protected override int FilenameIdentifier { get; } = _random.Next(0, 9999);

        protected override string ReportFileName =>
            CleanFileName(Title) + "_" + DateCreated.ToString("MM.dd.yyyy.H.mm.ss") + "_" + FilenameIdentifier + ".xlsx";

        /// <summary>
        /// Gets or set the number of copies to print.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This 
        /// </para>
        /// <para>
        /// Previously, the program would print multiple copies by opening
        /// Excel multiple times and print once. However, this did not always
        /// print the desired number of copies (see #36285).
        /// </para>
        /// </remarks>
        public int NumberOfCopies
        {
            get => _numberOfCopies;
            set
            {
                if (value < 1 || value > 65535)
                {
                    _numberOfCopies = 1;
                }
                else
                {
                    _numberOfCopies = value;
                }
            }
        }

        #endregion

        #region Methods

        protected ExcelBaseReport()
        {
            DefaultFontName = "Verdana";
            DefaultFontSize = 10;
            DefaultHeaderBackgroundColor = System.Drawing.Color.LightGray;
            DefaultAltRowBackgroundColor = System.Drawing.Color.Cornsilk;
        }

        public override void DisplayReport(CancellationToken cancellationToken)
        {
            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            try
            {
                string path = FileSystem.GetFolder(FileSystem.enumFolderType.Reports, true);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string fileName = string.Empty;
                if (!cancellationToken.IsCancellationRequested)
                {
                    fileName = PublishReport(path, cancellationToken);
                }

                if (!string.IsNullOrEmpty(fileName) && !cancellationToken.IsCancellationRequested)
                {
                    //show it
                    FileLauncher.New()
                        .HandleErrorsWith((exception, errorMsg) => { throw new Exception(errorMsg, exception); })
                        .Launch(fileName);
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error displaying report.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        public override string PublishReport(string outputPath, CancellationToken cancellationToken)
        {
            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            CreateReport(cancellationToken);

            string fileName = Path.Combine(outputPath, ReportFileName);

            //export it
            if (this._workbook != null && _workbook.Worksheets.Count > 0)
            {
                // Set # of copies.
                foreach (var worksheet in _workbook.Worksheets)
                {
                    worksheet.PrintOptions.NumberOfCopies = NumberOfCopies;
                }

                // Save workbook.
                this._workbook.Save(fileName);
                return fileName;
            }
            else
                return null;
        }

        private static string CleanFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return fileName;
            }

            return fileName
                .Replace("<", "_")
                .Replace(">", "_")
                .Replace(":", "_")
                .Replace("\"", "_")
                .Replace("/", "_")
                .Replace("\\", "_")
                .Replace("|", "_")
                .Replace("?", "_")
                .Replace("*", "_")
                .Replace("\n", "_")
                .Replace("\r", "_");
        }

        public string PublishProtectedReport(string outputPath)
        {
            CreateReport(CancellationToken.None);

            string reportName = Title.Replace("/", "_") + "_" + DateCreated.ToString("MM.dd.yyyy.H.mm.ss") + "_" + FilenameIdentifier + ".xlsx";
            string fileName = Path.Combine(outputPath, reportName);

            //export it
            if (this._workbook != null && _workbook.Worksheets.Count > 0)
            {
                _workbook.Protect(false, false);
                _workbook.CalculationMode = CalculationMode.Manual; // prevent save prompt

                // Set # of copies.
                foreach (var worksheet in _workbook.Worksheets)
                {
                    worksheet.PrintOptions.NumberOfCopies = NumberOfCopies;
                }

                // Save workbook.
                this._workbook.Save(fileName);
                return fileName;
            }
            else
                return null;
        }

        public virtual byte[] PublishExcelReportToBinary(string author)
        {
            CreateWorkBook(author);
            CreateReport(CancellationToken.None);

            using(var ms = new MemoryStream())
            {
                this._workbook.Save(ms);
                return ms.ToArray();
            }
        }

        protected void CreateHeaderFooter(Worksheet worksheet, string title)
        {
            string city = ApplicationSettings.Current.CompanyCity;
            string state = ApplicationSettings.Current.CompanyState;
            string zip = ApplicationSettings.Current.CompanyZip;
            string address1 = ApplicationSettings.Current.CompanyAddress1;
            string address2 = city + "," + state + " " + zip;
            string phone = ApplicationSettings.Current.CompanyPhone;
            string fax = ApplicationSettings.Current.CompanyFax;
            string name = ApplicationSettings.Current.CompanyName;
            string version = About.ApplicationVersionMajorMinor;

            worksheet.PrintOptions.TopMargin = 1.1;
            worksheet.PrintOptions.CenterHorizontally = true;
            worksheet.PrintOptions.Orientation = Orientation.Landscape;
            worksheet.PrintOptions.LeftMargin = 0.5;
            worksheet.PrintOptions.RightMargin = 0.5;

            //Header
            worksheet.PrintOptions.Header = "&L&16\n&I&U" + name + "&C\n&18&B" + title + "&R&11 " + address1 + "\n" + address2 + "\n PHONE: " + phone + "\nFAX: " + fax;

            //Footer
            worksheet.PrintOptions.Footer = "&L&D&C&8Powered By DWOS " + version + "\nA Product of Dynamic Software Solutions &R&P of &N";
            worksheet.PrintOptions.PageNumbering = Infragistics.Documents.Excel.PageNumbering.Automatic;
            worksheet.PrintOptions.HeaderMargin = 0;
        }
        
        protected string ExcelColumnIndexToName(int colIndex)
        {
            var range = "";
            if (colIndex < 0) 
                return range;

            for (var i = 1; colIndex + i > 0; i = 0)
            {
                range = ((char)(65 + colIndex % 26)).ToString() + range;
                colIndex /= 26;
            }

            if (range.Length > 1) 
                range = ((char)((int)range[0] - 1)).ToString() + range.Substring(1);
            
            return range;
        }

        protected void FitAllColumnsOnSinglePage(Worksheet worksheet)
        {
            // Set the scaling for printing all columns on a single page
            worksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
            worksheet.PrintOptions.MaxPagesHorizontally = 1;
            worksheet.PrintOptions.MaxPagesVertically = 1000;
        }

        protected void AddTopLogo(Worksheet worksheet, int startRowIndex, int startColumnIndex, int endRowIndex, int endColumnIndex)
        {
            var companyLogoImagePath = ApplicationSettings.Current.CompanyLogoImagePath;

            if (!string.IsNullOrEmpty(companyLogoImagePath))
            {
                var topLeftCell = worksheet.Rows[startRowIndex].Cells[startColumnIndex];
                var bottomRightCell = worksheet.Rows[endRowIndex].Cells[endColumnIndex];
                var imageShape = new WorksheetImage(new Bitmap(companyLogoImagePath))
                {
                    TopLeftCornerCell = topLeftCell,
                    TopLeftCornerPosition = new PointF(10.0F, 25.0F),
                    BottomRightCornerCell = bottomRightCell,
                    BottomRightCornerPosition = new PointF(90.0F, 75.0F)
                };
                imageShape.Fill = new ShapeFillSolid(System.Drawing.Color.Transparent);
                imageShape.Outline = new ShapeOutlineSolid(System.Drawing.Color.Transparent);
                worksheet.Shapes.Add(imageShape);
            }
        }

        protected void CreateTable(Worksheet worksheet, int startRowIndex, int endColumnIndex, int endRowIndex, bool hasHeaders)
        {
            // Create a table for sorting/filtering
            worksheet.Tables.Add("A{0}:{1}{2}".FormatWith(startRowIndex, this.ExcelColumnIndexToName(endColumnIndex), endRowIndex), hasHeaders);
            worksheet.Tables[0].DisplayBandedRows = false;

            // Header text is changed to white when the table is added, set back to black
            foreach (var col in worksheet.Tables[0].Columns)
                col.HeaderCell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Black);
        }

        #region Workbook and Worksheet Methods

        protected void CreateWorkBook(string author = null)
        {
            if(_workbook == null)
            {
                //Create the workbook for the report
                this._workbook = new Workbook(WorkbookFormat.Excel2007);
                this._workbook.DocumentProperties.Author = author ?? SecurityManager.UserName;
                this._workbook.DocumentProperties.Company = ApplicationSettings.Current.CompanyName;
                this._workbook.DocumentProperties.Title = Title;
            }
        }

        /// <summary>
        ///     Creates the worksheet.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="frozenColumns">The frozen columns.</param>
        /// <param name="frozenRows">The frozen rows.</param>
        /// <returns></returns>
        protected Worksheet CreateWorksheet(string title, int frozenColumns = 0, int frozenRows = 0)
        {
            title = FormatWorksheetTitle(title);

            if(_workbook == null)
                CreateWorkBook();

            var wks = this._workbook.Worksheets.Add(title);
            wks.PrintOptions.Orientation = ReportPageOrientation == PageOrientation.Landscape ? Orientation.Landscape : Orientation.Default;
            wks.DisplayOptions.PanesAreFrozen = frozenColumns > 0 || frozenRows > 0;
            wks.DisplayOptions.FrozenPaneSettings.FrozenColumns = frozenColumns;
            wks.DisplayOptions.FrozenPaneSettings.FrozenRows = frozenRows;

            return wks;
        }

        protected static string FormatWorksheetTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return "Unknown";
            }

            return title
                .Remove(":", "\\", "/", "?", "*", "[", "]")
                .TrimToMaxLength(WORKSHEET_MAX_LENGTH);
        }

        protected static string FormatWorksheetTitle(string title, string suffix)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(suffix))
            {
                return FormatWorksheetTitle(title);
            }

            var formattedTitle = title
                .Remove(":", "\\", "/", "?", "*", "[", "]")
                .TrimToMaxLength(WORKSHEET_MAX_LENGTH - suffix.Length);

            return formattedTitle + FormatWorksheetTitle(suffix);
        }

        #endregion

        #region Create Header Methods

        /// <summary>
        ///     Adds the company header rows and returns next valid row index.
        /// </summary>
        /// <returns></returns>
        protected int AddCompanyHeaderRows(Worksheet worksheet, int lastColumnIndex)
        {
            int rowIndex = 0;

            //Create the merged region that will be a header to the column headers
            WorksheetMergedCellsRegion mergedRegion = worksheet.MergedCellsRegions.Add(rowIndex, 0, rowIndex, lastColumnIndex);
            mergedRegion.Value = ApplicationSettings.Current.CompanyName;

            //Set the cell alignment of the merged region. Since a cell and its merged region shared a cell format, this will ultimately set the format of the merged region
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            worksheet.Rows[rowIndex].Height = 18 * 20;

            mergedRegion.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            mergedRegion.CellFormat.Font.Name = DefaultFontName;
            mergedRegion.CellFormat.Font.Height = 18 * 20; //Font size height is measured in twips. One twips is 1/20th of a point

            rowIndex++;

            worksheet.Rows[rowIndex].Cells[0].Value = "Report:";
            worksheet.Rows[rowIndex].Cells[1].Value = Title;

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].Value = "Date:";
            worksheet.Rows[rowIndex].Cells[1].Value = DateCreated.ToShortDateString();
            
            this.CreateHeaderFooter(worksheet, Title);

            return rowIndex;
        }

        /// <summary>
        ///     Adds the company header rows and returns next valid row index.
        /// </summary>
        /// <returns></returns>
        protected int AddCompanyHeaderRows(Worksheet worksheet, int lastColumnIndex, string reportName, string title = null)
        {
            int rowIndex = 0;

            //Create the merged region that will be a header to the column headers

            WorksheetMergedCellsRegion mergedRegion = worksheet.MergedCellsRegions.Add(rowIndex, 0, rowIndex, lastColumnIndex);
            mergedRegion.Value = title ?? (Title + " " + reportName);

            //Set the cell alignment of the merged region. Since a cell and its merged region shared a cell format, this will ultimately set the format of the merged region
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            worksheet.Rows[rowIndex].Height = 18 * 20;

            mergedRegion.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            mergedRegion.CellFormat.Font.Name = DefaultFontName;
            mergedRegion.CellFormat.Font.Height = 18 * 20; //Font size height is measured in twips. One twips is 1/20th of a point

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "Company:  " + ApplicationSettings.Current.CompanyName;
            //worksheet.Rows[rowIndex].Cells[2].Value = ApplicationSettings.Current.CompanyName;

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "Date:  " + DateCreated;
            //worksheet.Rows[rowIndex].Cells[2].Value = DateTime.Now;

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "User:  " + SecurityManager.UserName;
            //worksheet.Rows[rowIndex].Cells[2].Value = SecurityManager.UserName;

            this.CreateHeaderFooter(worksheet, Title + " " + reportName);

            return rowIndex;
        }

        /// <summary>
        ///     Creates the header cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="cellText">The cell text.</param>
        /// <param name="borderStyle">The border style.</param>
        /// <returns></returns>
        protected WorksheetCell CreateHeaderCell(Worksheet worksheet, int rowIndex, int columnIndex, string cellText, CellBorderLineStyle borderStyle = CellBorderLineStyle.Thin)
        {
            WorksheetCell cell = worksheet.Rows[rowIndex].Cells[columnIndex];

            cell.Value = cellText;

            //Set the border style
            cell.CellFormat.BottomBorderStyle = borderStyle;
            cell.CellFormat.LeftBorderStyle = borderStyle;
            cell.CellFormat.RightBorderStyle = borderStyle;
            cell.CellFormat.TopBorderStyle = borderStyle;

            //Set the cell style
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
            cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            cell.CellFormat.Font.Name = DefaultFontName;
            cell.CellFormat.Font.Height = DefaultFontSize * 20; //Font size height is measured in twips. One twips is 1/20th of a point
            cell.CellFormat.Fill = CellFill.CreateSolidFill(DefaultHeaderBackgroundColor);

            return cell;
        }

        protected WorksheetCell CreateHeaderCell(Worksheet worksheet, int rowIndex, int columnIndex, string cellText, int? characterWidth)
        {
            WorksheetCell cell = worksheet.Rows[rowIndex].Cells[columnIndex];

            cell.Value = cellText;

            //Set the border style
            cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
            cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
            cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
            cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;

            //Set the cell style
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
            cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            cell.CellFormat.Font.Name = DefaultFontName;
            cell.CellFormat.Font.Height = DefaultFontSize * 20; //Font size height is measured in twips. One twips is 1/20th of a point
            cell.CellFormat.Fill = CellFill.CreateSolidFill(DefaultHeaderBackgroundColor);

            if(characterWidth.HasValue)
                worksheet.Columns[columnIndex].Width = characterWidth.Value * 256;

            return cell;
        }

        /// <summary>
        ///     Creates the merged header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="firstRowIndex">First index of the row.</param>
        /// <param name="firstColumnIndex">First index of the column.</param>
        /// <param name="lastRowIndex">Last index of the row.</param>
        /// <param name="lastColumnIndex">Last index of the column.</param>
        /// <param name="headerText">The header text.</param>
        /// <param name="borderStyle">The border style.</param>
        protected WorksheetMergedCellsRegion CreateMergedHeader(Worksheet worksheet, int firstRowIndex, int firstColumnIndex, int lastRowIndex, int lastColumnIndex, string headerText, CellBorderLineStyle borderStyle = CellBorderLineStyle.Thin)
        {
            //Create the merged region that will be a header to the column headers
            WorksheetMergedCellsRegion mergedRegion = worksheet.MergedCellsRegions.Add(firstRowIndex, firstColumnIndex, lastRowIndex, lastColumnIndex);

            //Set the value of the merged region
            mergedRegion.Value = headerText;

            //Give the merged region a solid background color
            //mergedRegion.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.LightGray), new WorkbookColorInfo(System.Drawing.Color.LightGray), FillPatternStyle.Solid);
            mergedRegion.CellFormat.Fill = CellFill.CreateSolidFill(DefaultHeaderBackgroundColor);

            //Set the border style
            mergedRegion.CellFormat.BottomBorderStyle = borderStyle;
            mergedRegion.CellFormat.LeftBorderStyle = borderStyle;
            mergedRegion.CellFormat.RightBorderStyle = borderStyle;
            mergedRegion.CellFormat.TopBorderStyle = borderStyle;

            //Set the cell alignment of the merged region. Since a cell and its merged region shared a cell format, this will ultimately set the format of the merged region
            worksheet.Rows[firstRowIndex].Cells[firstColumnIndex].CellFormat.Alignment = HorizontalCellAlignment.Center;
            worksheet.Rows[firstRowIndex].Cells[firstColumnIndex].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            mergedRegion.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            mergedRegion.CellFormat.Font.Name = DefaultFontName;
            mergedRegion.CellFormat.Font.Height = DefaultFontSize * 20; //Font size height is measured in twips. One twips is 1/20th of a point

            return mergedRegion;
        }

        #endregion

        #region Create Cell Methods

        /// <summary>
        ///     Creates the merged cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="firstRowIndex">First index of the row.</param>
        /// <param name="firstColumnIndex">First index of the column.</param>
        /// <param name="lastRowIndex">Last index of the row.</param>
        /// <param name="lastColumnIndex">Last index of the column.</param>
        /// <param name="text">The text.</param>
        /// <param name="borderStyle">The border style.</param>
        protected WorksheetMergedCellsRegion CreateMergedCell(Worksheet worksheet, int firstRowIndex, int firstColumnIndex, int lastRowIndex, int lastColumnIndex, string text, bool applyBackgroundColor = false, HorizontalCellAlignment horizontalAlignment = HorizontalCellAlignment.Left, CellBorderLineStyle borderStyle = CellBorderLineStyle.Thin)
        {
            //Create the merged region that will be a header to the column headers
            WorksheetMergedCellsRegion mergedRegion = worksheet.MergedCellsRegions.Add(firstRowIndex, firstColumnIndex, lastRowIndex, lastColumnIndex);

            //Set the value of the merged region
            mergedRegion.Value = text;

            //Give the merged region a solid background color
            if(applyBackgroundColor)
                mergedRegion.CellFormat.Fill = CellFill.CreateSolidFill(DefaultHeaderBackgroundColor);

            //Set the border style
            mergedRegion.CellFormat.BottomBorderStyle = borderStyle;
            mergedRegion.CellFormat.LeftBorderStyle = borderStyle;
            mergedRegion.CellFormat.RightBorderStyle = borderStyle;
            mergedRegion.CellFormat.TopBorderStyle = borderStyle;

            //Set the cell alignment of the middle cell in the merged region. Since a cell and its merged region shared a cell format, this will ultimately set the format of the merged region
            worksheet.Rows[firstRowIndex].Cells[firstColumnIndex].CellFormat.Alignment = horizontalAlignment;
            worksheet.Rows[firstRowIndex].Cells[firstColumnIndex].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            mergedRegion.CellFormat.Font.Name = DefaultFontName;
            mergedRegion.CellFormat.Font.Height = DefaultFontSize * 20; //Font size height is measured in twips. One twips is 1/20th of a point

            return mergedRegion;
        }

        /// <summary>
        ///     Creates the cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <param name="applyBackgroundColor">
        ///     if set to <c>true</c> [apply background color].
        /// </param>
        /// <param name="horizontalAlignment">The horizontal alignment.</param>
        /// <param name="borderStyle">The border style.</param>
        /// <returns></returns>
        protected WorksheetCell CreateCell(Worksheet worksheet, int rowIndex, int columnIndex, object cellValue, bool applyBackgroundColor = false, HorizontalCellAlignment horizontalAlignment = HorizontalCellAlignment.Left, CellBorderLineStyle borderStyle = CellBorderLineStyle.Thin, string cellFormat = null)
        {
            WorksheetCell cell = worksheet.Rows[rowIndex].Cells[columnIndex];

            cell.Value = cellValue;

            //Set the cell background color
            if(applyBackgroundColor)
                cell.CellFormat.Fill = CellFill.CreateSolidFill(DefaultHeaderBackgroundColor);

            //Set the border style
            cell.CellFormat.BottomBorderStyle = borderStyle;
            cell.CellFormat.LeftBorderStyle = borderStyle;
            cell.CellFormat.RightBorderStyle = borderStyle;
            cell.CellFormat.TopBorderStyle = borderStyle;
            cell.CellFormat.Alignment = horizontalAlignment;
            cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            cell.CellFormat.Font.Name = DefaultFontName;
            cell.CellFormat.Font.Height = DefaultFontSize * 20; //Font size height is measured in twips. One twips is 1/20th of a point

            if(cellFormat != null)
                cell.CellFormat.FormatString = cellFormat;

            return cell;
        }

        #endregion

        #region Create Formula Cells

        /// <summary>
        ///     Creates the formula cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="cellFormula">The cell formula (ie. =J12*L12, =Sum(A1:A8)</param>
        /// <param name="applyBackgroundColor">
        ///     if set to <c>true</c> [apply background color].
        /// </param>
        /// <param name="horizontalAlignment">The horizontal alignment.</param>
        /// <param name="borderStyle">The border style.</param>
        /// <returns></returns>
        protected WorksheetCell CreateFormulaCell(Worksheet worksheet, int rowIndex, int columnIndex, string cellFormula, CellReferenceMode referenceMode = CellReferenceMode.A1, bool applyBackgroundColor = false, HorizontalCellAlignment horizontalAlignment = HorizontalCellAlignment.Left, CellBorderLineStyle borderStyle = CellBorderLineStyle.Thin, string cellFormat = null)
        {
            WorksheetCell cell = worksheet.Rows[rowIndex].Cells[columnIndex];

            Formula formula = Formula.Parse(cellFormula, referenceMode);
            formula.ApplyTo(cell);

            //Set the cell background color
            if(applyBackgroundColor)
                cell.CellFormat.Fill = CellFill.CreateSolidFill(DefaultHeaderBackgroundColor);

            //Set the border style
            cell.CellFormat.BottomBorderStyle = borderStyle;
            cell.CellFormat.LeftBorderStyle = borderStyle;
            cell.CellFormat.RightBorderStyle = borderStyle;
            cell.CellFormat.TopBorderStyle = borderStyle;
            cell.CellFormat.Alignment = horizontalAlignment;
            cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            cell.CellFormat.Font.Name = DefaultFontName;
            cell.CellFormat.Font.Height = DefaultFontSize * 20; //Font size height is measured in twips. One twips is 1/20th of a point

            if(cellFormat != null)
                cell.CellFormat.FormatString = cellFormat;

            return cell;
        }

        #endregion

        #region Format Cells

        /// <summary>
        ///     Formats the border.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="borderStyle">The border style.</param>
        /// <param name="horizontalAlign">The horizontal align.</param>
        protected void FormatBorder(WorksheetCell cell, CellBorderLineStyle borderStyle, HorizontalCellAlignment horizontalAlign = HorizontalCellAlignment.Left)
        {
            cell.CellFormat.BottomBorderStyle = borderStyle;
            cell.CellFormat.LeftBorderStyle = borderStyle;
            cell.CellFormat.RightBorderStyle = borderStyle;
            cell.CellFormat.TopBorderStyle = borderStyle;

            cell.CellFormat.Alignment = horizontalAlign;
            cell.CellFormat.Font.Name = DefaultFontName;
            cell.CellFormat.Font.Height = DefaultFontSize * 20; //Font size height is measured in twips. One twips is 1/20th of a point
        }

        /// <summary>
        ///     Formats the border.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="borderStyle">The border style.</param>
        protected void FormatBorder(WorksheetRow row, CellBorderLineStyle borderStyle)
        {
            row.CellFormat.BottomBorderStyle = borderStyle;
            row.CellFormat.LeftBorderStyle = borderStyle;
            row.CellFormat.RightBorderStyle = borderStyle;
            row.CellFormat.TopBorderStyle = borderStyle;

            row.CellFormat.Font.Name = DefaultFontName;
            row.CellFormat.Font.Height = DefaultFontSize * 20; //Font size height is measured in twips. One twips is 1/20th of a point
        }

        /// <summary>
        ///     Formats the alternate row.
        /// </summary>
        /// <param name="row">The row.</param>
        protected void FormatAlternateRow(WorksheetRow row) { row.CellFormat.Fill = CellFill.CreateSolidFill(DefaultAltRowBackgroundColor); }

        /// <summary>
        ///     Formats the header row with default font, border, and fill.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="borderStyle">The border style.</param>
        protected void FormatHeader(WorksheetRow row, CellBorderLineStyle borderStyle = CellBorderLineStyle.Thin)
        {
            FormatBorder(row, borderStyle);

            row.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            row.CellFormat.Alignment = HorizontalCellAlignment.Center;
            row.CellFormat.Font.Name = DefaultFontName;
            row.CellFormat.Font.Height = DefaultFontSize * 20; //Font size height is measured in twips. One twips is 1/20th of a point
            row.CellFormat.Fill = CellFill.CreateSolidFill(DefaultHeaderBackgroundColor);
        }

        protected void FormatHeaderCells(Worksheet worksheet, int row, int fromCell, int toCell, CellBorderLineStyle borderStyle = CellBorderLineStyle.Thin)
        {
            for(int i = fromCell; i <= toCell; i++)
            {
                FormatBorder(worksheet.Rows[row].Cells[i], borderStyle);

                worksheet.Rows[row].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                worksheet.Rows[row].CellFormat.Alignment = HorizontalCellAlignment.Center;
                worksheet.Rows[row].CellFormat.Font.Name = DefaultFontName;
                worksheet.Rows[row].CellFormat.Font.Height = DefaultFontSize * 20; //Font size height is measured in twips. One twips is 1/20th of a point
                worksheet.Rows[row].CellFormat.Fill = CellFill.CreateSolidFill(DefaultHeaderBackgroundColor);
            }
        }

        /// <summary>
        ///     Gets the type of the cell format by.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        protected string GetCellFormatByType(Type t)
        {
            if(t.Equals(typeof(DateTime)))
                return DATE_FORMAT;

            if(t.Equals(typeof(Decimal)))
                return MONEY_FORMAT;

            return null;
        }

        /// <summary>
        ///     Sets the column widths.
        /// </summary>
        /// <param name="wks">The worksheet.</param>
        /// <param name="firstColumnIndex">First index of the column.</param>
        /// <param name="lastColumIndex">Last index of the colum.</param>
        /// <param name="width">The width.</param>
        protected void SetColumnWidths(Worksheet wks, int firstColumnIndex, int lastColumIndex, int width = 16)
        {
            if(firstColumnIndex >= 0 && firstColumnIndex <= lastColumIndex)
            {
                for(int i = firstColumnIndex; i <= lastColumIndex; i++)
                {
                    //Width is 256ths of the '0' digit character width in the workbook's default font.
                    wks.Columns[i].Width = width * 256;
                }
            }
        }

        protected void Bold(WorksheetCell cell)
        {
            if (cell == null)
            {
                throw new ArgumentNullException(nameof(cell));
            }

            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
        }

        protected void Bold(WorksheetMergedCellsRegion cellRegion)
        {
            if (cellRegion == null)
            {
                throw new ArgumentNullException(nameof(cellRegion));
            }

            cellRegion.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
        }

        protected void WrapText(WorksheetCell cell)
        {
            if (cell == null)
            {
                throw new ArgumentNullException(nameof(cell));
            }

            cell.CellFormat.WrapText = ExcelDefaultableBoolean.True;
        }

        #endregion

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            this._workbook = null;

            base.Dispose();
        }

        #endregion
    }
}