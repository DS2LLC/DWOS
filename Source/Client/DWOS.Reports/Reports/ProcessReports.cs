using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.PartsDatasetTableAdapters;
using DWOS.Data.Process;
using DWOS.Data.Reports.ProcessPartsReportTableAdapters;
using DWOS.Reports.Properties;
using DWOS.Reports.ReportData;
using DWOS.Reports.Utilities;
using DWOS.Shared.Utilities;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;

namespace DWOS.Reports
{
    /// <summary>
    ///     A summary report of a selected process.
    /// </summary>
    public class ProcessUsageSummaryReport : ExcelBaseReport
    {
        /// <summary>
        ///     Use this subclass in place of Dataset Rows
        /// </summary>
        private class ProcessUsageRow
        {
            public string CustomerName { get; set; }
            public int Orders { get; set; }
            public long Parts { get; set; }
            public string ProcessName { get; set; }
            public int PartId { get; set; }
            public string Manufacturer { get; set; }
            public string ProcessCategory { get; set; }
            public double TotalSurfaceArea { get; set; }
        }

        #region Fields

        private const string TITLE_PRODUCT_CLASS = "Usage by Product Class";

        #endregion

        #region Properties

        public override string Title => "Process Usage Summary";

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation => PageOrientation.Portrait;

        [Browsable(true)]
        [Description("The from date of the report.")]
        [DisplayName("From Date")]
        [Category("Report")]
        public DateTime FromDate { get; set; }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("To Date")]
        [Category("Report")]
        public DateTime ToDate { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessUsageSummaryReport"/> class.
        /// </summary>
        public ProcessUsageSummaryReport()
        {
            this.FromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            this.ToDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        /// <summary>
        ///     Calls the appropriate create method for each report
        /// </summary>
        private void CreateReport()
        {
            Data.Reports.ProcessPartsReport.ProcessUsageSummaryDataTable dtProcessUsage = null;
            ProcessUsageSummaryTableAdapter taProcessSummary = null;

            Data.Reports.ProcessPartsReport.OrderProcessRevenueDataTable dtOrderRevenue = null;
            OrderProcessRevenueTableAdapter taOrderRevenue = null;

            Data.Reports.ProcessPartsReport.OrderProductClassDataTable dtProductClass = null;
            OrderProductClassTableAdapter taProductClass = null;

            try
            {
                taProcessSummary = new ProcessUsageSummaryTableAdapter();
                dtProcessUsage = new Data.Reports.ProcessPartsReport.ProcessUsageSummaryDataTable();

                dtOrderRevenue = new Data.Reports.ProcessPartsReport.OrderProcessRevenueDataTable();
                taOrderRevenue = new OrderProcessRevenueTableAdapter();

                dtProductClass = new Data.Reports.ProcessPartsReport.OrderProductClassDataTable();
                taProductClass = new OrderProductClassTableAdapter();

                taProcessSummary.Fill(dtProcessUsage, this.ToDate, this.FromDate);

                CreateExcelProcessUsageSummary(dtProcessUsage);

                bool createProductClassSummary;
                using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                {
                    var productClassField = ta.GetByCategory("Order").FirstOrDefault(f => f.Name == "Product Class");
                    createProductClassSummary = productClassField == null || productClassField.IsRequired || productClassField.IsVisible;
                }

                if (createProductClassSummary)
                {
                    taOrderRevenue.Fill(dtOrderRevenue, ToDate, FromDate);
                    taProductClass.FillByProcessDate(dtProductClass, ToDate, FromDate);
                    CreateExcelProductClassSummary(dtOrderRevenue, dtProductClass);
                }

                CreateExcelProcessDetails(dtProcessUsage);
            }
            finally
            {
                dtProcessUsage?.Dispose();
                taProcessSummary?.Dispose();

                dtOrderRevenue?.Dispose();
                taOrderRevenue?.Dispose();

                dtProductClass?.Dispose();
                taProductClass?.Dispose();
            }
        }

        /// <summary>
        ///     Creates the process usage summary report
        /// </summary>
        private void CreateExcelProcessUsageSummary(Data.Reports.ProcessPartsReport.ProcessUsageSummaryDataTable dtProcessUsage)
        {
            Worksheet procSumWks = CreateWorksheet(Title);
            SetupReportInfo();

            var rowIndex = AddExcelProcessUsageMainHeader(procSumWks);
            rowIndex = AddExcelProcessUsageRowHeader(procSumWks, rowIndex, 0);

            var procRowList = new List <ProcessUsageRow>();
            foreach(var psRow in dtProcessUsage)
            {
                var procRow = new ProcessUsageRow();
                procRow.CustomerName = psRow.CustomerName;

                if(!psRow.IsOrdersNull())
                    procRow.Orders = psRow.Orders;
                if (!psRow.IsPartIDNull())
                    procRow.PartId = psRow.PartID;

                if (!psRow.IsPartCountNull())
                {
                    procRow.Parts = psRow.PartCount;
                    procRow.TotalSurfaceArea = psRow.PartSurfaceArea * psRow.PartCount;
                }
                
                procRow.ProcessName = psRow.ProcessName;

                procRowList.Add(procRow);
            }
            
            FillProcessUsageSummary(procSumWks, procRowList, rowIndex);
        }

        /// <summary>
        /// Creates the excel process details.
        /// </summary>
        private void CreateExcelProcessDetails(Data.Reports.ProcessPartsReport.ProcessUsageSummaryDataTable dtProcessUsage)
        {
            Worksheet procSumWks = CreateWorksheet("Process Usage Details");
            SetupReportInfo();

            var rowIndex = AddExcelProcessDetailsMainHeader(procSumWks);
            rowIndex = AddExcelProcessDetailsRowHeader(procSumWks, rowIndex, 0);

            var procRowList = new List<ProcessUsageRow>();

            using (var taPart = new PartTableAdapter())
            {
                foreach (var psRow in dtProcessUsage)
                {
                    var procRow = new ProcessUsageRow();
                    procRow.CustomerName = psRow.CustomerName;
                    procRow.Orders = psRow.Orders;
                    procRow.PartId = psRow.PartID;

                    if (!psRow.IsPartCountNull())
                    {
                        procRow.Parts = psRow.PartCount;
                        procRow.TotalSurfaceArea = psRow.PartSurfaceArea * psRow.PartCount;
                    }

                    procRow.ProcessName = psRow.ProcessName;
                    procRow.Manufacturer = taPart.GetManufacturer(procRow.PartId);
                    procRow.ProcessCategory = psRow.Category;
                    procRowList.Add(procRow);
                }
                FillProcessDetails(procSumWks, procRowList, rowIndex);
            }
        }

        private void CreateExcelProductClassSummary(
            Data.Reports.ProcessPartsReport.OrderProcessRevenueDataTable dtOrderRevenue,
            Data.Reports.ProcessPartsReport.OrderProductClassDataTable dtProductClass)
        {
            // Setup worksheet
            var worksheet = CreateWorksheet(TITLE_PRODUCT_CLASS);
            SetupReportInfo();

            var rowIndex = AddExcelProductClassUsageMainHeader(worksheet);
            rowIndex = AddExcelProductClassRowHeader(worksheet, rowIndex, 0);

            // Create data
            var productClasses = new List<ProductClassUsage>();

            foreach (var orderRow in dtOrderRevenue)
            {
                var primaryProductClass = dtProductClass.FirstOrDefault(pc => pc.OrderID == orderRow.OrderID)?.ProductClass ?? "N/A";

                var matchingProductClass = productClasses.FirstOrDefault(pc => pc.ProductClass == primaryProductClass);

                if (matchingProductClass == null)
                {
                    productClasses.Add(new ProductClassUsage
                    {
                        ProductClass = primaryProductClass,
                        OrderCount = 1,
                        PartCount = orderRow.IsPartQuantityNull() ? 0 : orderRow.PartQuantity,
                        TotalSurfaceArea = orderRow.IsTotalSurfaceAreaNull() ? 0d : orderRow.TotalSurfaceArea
                    });
                }
                else
                {
                    matchingProductClass.OrderCount++;

                    if (!orderRow.IsPartQuantityNull())
                    {
                        matchingProductClass.PartCount += orderRow.PartQuantity;
                    }

                    if (!orderRow.IsTotalSurfaceAreaNull())
                    {
                        matchingProductClass.TotalSurfaceArea += orderRow.TotalSurfaceArea;
                    }
                }
            }

            FillProductClassUsageSummary(worksheet, productClasses, rowIndex);
        }

        /// <summary>
        /// Fills the customer details Report
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="procRowList">The proc row list.</param>
        /// <param name="rowIndex">Index of the row.</param>
        private void FillProcessDetails(Worksheet worksheet, List<ProcessUsageRow> procRowList, int rowIndex)
        {
            string lastProcess = null;
            string lastCustomer = null;
            string lastManufacturer = null;
            var part = 0L;
            var order = 0;
            double surfaceArea = 0;

            var startRowIndex = rowIndex;
            var orderedProcessRows = procRowList
                .OrderBy(p => p.ProcessName)
                .ThenBy(p => p.CustomerName)
                .ThenBy(p => p.Manufacturer);

            foreach (var procRow in orderedProcessRows)
            {
                if (procRow.ProcessName != lastProcess || procRow.CustomerName != lastCustomer || procRow.Manufacturer != lastManufacturer)
                {
                    part = 0;
                    order = 0;
                    surfaceArea = 0;

                    if (!string.IsNullOrEmpty(lastProcess) || !string.IsNullOrEmpty(lastCustomer) ||
                        !string.IsNullOrEmpty(lastManufacturer))
                    {
                        rowIndex++;
                    }

                    AddExcelProcessDetailsRow(worksheet, procRow, rowIndex, 0);
                    order += procRow.Orders;
                    lastProcess = procRow.ProcessName;
                    lastCustomer = procRow.CustomerName;
                    lastManufacturer = procRow.Manufacturer;
                    part += procRow.Parts;
                    surfaceArea += procRow.TotalSurfaceArea;

                }
                else
                {
                    part += procRow.Parts;
                    surfaceArea += procRow.TotalSurfaceArea;
                    order += procRow.Orders;
                    worksheet.Rows[rowIndex].Cells[4].Value = order;
                    worksheet.Rows[rowIndex].Cells[5].Value = part;
                    worksheet.Rows[rowIndex].Cells[6].Value = surfaceArea;
                }
            }

            rowIndex++;
            this.CreateTable(worksheet, startRowIndex, 6, rowIndex, true);
            AddExcelTotalDetailsRow(worksheet, rowIndex);
        }

        /// <summary>
        ///     Fills Process Usage Summary Report
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="procRowList"></param>
        /// <param name="rowIndex"></param>
        private void FillProcessUsageSummary(Worksheet worksheet, List <ProcessUsageRow> procRowList, int rowIndex)
        {
            string lastProcess = null;
            long part = 0;
            int order = 0;
            double surfaceArea = 0;
            
            foreach(var procRow in procRowList.OrderBy(p => p.ProcessName))
            {
                if(procRow.ProcessName != lastProcess)
                {
                    part = 0;
                    order = 0;
                    surfaceArea = 0;

                    if(lastProcess == null)
                    {
                        AddExcelProcessUsageRow(worksheet, procRow, rowIndex, 0);
                        order += procRow.Orders;
                        lastProcess = procRow.ProcessName;
                        part += procRow.Parts;
                        surfaceArea += procRow.TotalSurfaceArea;
                    }
                    else
                    {
                        rowIndex++;
                        AddExcelProcessUsageRow(worksheet, procRow, rowIndex, 0);
                        order += procRow.Orders;
                        lastProcess = procRow.ProcessName;
                        part += procRow.Parts;
                        surfaceArea += procRow.TotalSurfaceArea;
                    }
                }
                else
                {
                    part += procRow.Parts;
                    surfaceArea += procRow.TotalSurfaceArea;
                    order += procRow.Orders;

                    worksheet.Rows[rowIndex].Cells[1].Value = order;
                    worksheet.Rows[rowIndex].Cells[2].Value = part;
                    worksheet.Rows[rowIndex].Cells[3].Value = surfaceArea;
                }
            }

            rowIndex++;
            AddExcelTotalSummaryRow(worksheet, rowIndex, 0);
        }

        private void FillProductClassUsageSummary(Worksheet worksheet, List<ProductClassUsage> productClasses, int rowIndex)
        {
            foreach (var productClass in productClasses.OrderBy(pc => pc.ProductClass))
            {
                AddExcelProductClassUsageRow(worksheet, productClass, rowIndex, 0);
                rowIndex++;
            }

            if (productClasses.Count == 0)
            {
                rowIndex++;
            }

            AddExcelTotalSummaryRow(worksheet, rowIndex, 0);
        }

        /// <summary>
        ///     Adds a row to the process usage report worksheet.
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="procRow"></param>
        /// <param name="rowIndex"></param>
        /// <param name="startColumn"></param>
        private void AddExcelProcessUsageRow(Worksheet worksheet, ProcessUsageRow procRow, int rowIndex, int startColumn)
        {
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.ProcessName;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.Orders;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.Parts;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.TotalSurfaceArea;
        }

        /// <summary>
        /// Adds a row to the process usage report worksheet.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="procRow">The proc row.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="startColumn">The start column.</param>
        private void AddExcelProcessDetailsRow(Worksheet worksheet, ProcessUsageRow procRow, int rowIndex, int startColumn)
        {
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.ProcessName;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.CustomerName;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.Manufacturer;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.ProcessCategory;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.Orders;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.Parts;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.TotalSurfaceArea;
        }

        private void AddExcelProductClassUsageRow(Worksheet worksheet, ProductClassUsage classRow, int rowIndex, int startColumn)
        {
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = classRow.ProductClass;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = classRow.OrderCount;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = classRow.PartCount;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = classRow.TotalSurfaceArea;
        }

        /// <summary>
        ///     Adds the main header for Process Usage Report
        /// </summary>
        /// <param name="worksheet"></param>
        /// <returns></returns>
        private int AddExcelProcessUsageMainHeader(Worksheet worksheet)
        {
            int rowIndex = 0;
            //Add Header Title
            var region = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 3, "Process Usage Summary");

            //Set the cell alignment of the merged region. Since a cell and its merged region shared a cell format, this will ultimately set the format of the merged region
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            worksheet.Rows[rowIndex].Height = 18 * 20;
            region.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            region.CellFormat.Font.Name = DefaultFontName;
            region.CellFormat.Font.Height = 18 * 20;
            region.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "From Date:  " + this.FromDate;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 3, "Company:    " + ApplicationSettings.Current.CompanyName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "To Date:  " + this.ToDate;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 3, "User:             " + SecurityManager.UserName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 3, "Date:             " + DateTime.Now).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;
            return rowIndex;
        }

        /// <summary>
        /// Adds the main header for Process Usage Report
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <returns></returns>
        private int AddExcelProcessDetailsMainHeader(Worksheet worksheet)
        {
            int rowIndex = 0;
            WorksheetMergedCellsRegion region = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 6, "Process Usage Details");

            //Set the cell alignment of the merged region. Since a cell and its merged region shared a cell format, this will ultimately set the format of the merged region
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            worksheet.Rows[rowIndex].Height = 18 * 20;
            region.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            region.CellFormat.Font.Name = DefaultFontName;
            region.CellFormat.Font.Height = 18 * 20;
            region.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "From Date:  " + this.FromDate;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 6, "Company:    " + ApplicationSettings.Current.CompanyName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "To Date:  " + this.ToDate;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 6, "User:             " + SecurityManager.UserName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 6, "Date:             " + DateTime.Now).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;

            return rowIndex;
        }

        private int AddExcelProductClassUsageMainHeader(Worksheet worksheet)
        {
            int rowIndex = 0;
            WorksheetMergedCellsRegion region = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 6, TITLE_PRODUCT_CLASS);

            //Set the cell alignment of the merged region. Since a cell and its merged region shared a cell format, this will ultimately set the format of the merged region
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            worksheet.Rows[rowIndex].Height = 18 * 20;
            region.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            region.CellFormat.Font.Name = DefaultFontName;
            region.CellFormat.Font.Height = 18 * 20;
            region.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "From Date:  " + this.FromDate;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 6, "Company:    " + ApplicationSettings.Current.CompanyName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "To Date:  " + this.ToDate;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 6, "User:             " + SecurityManager.UserName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 6, "Date:             " + DateTime.Now).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;

            return rowIndex;
        }

        /// <summary>
        ///     Adds a row header to the process Usage Report
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="startColumn"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private int AddExcelProcessUsageRowHeader(Worksheet worksheet, int rowIndex, int startColumn)
        {
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Process", 100);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Orders", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Parts", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Surface Area", 25);
            rowIndex++;
            return rowIndex;
        }

        private int AddExcelProductClassRowHeader(Worksheet worksheet, int rowIndex, int startColumn)
        {
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Product Class", 100);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Orders", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Parts", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Surface Area", 25);
            rowIndex++;
            return rowIndex;
        }

        /// <summary>
        /// Adds a row header to the process Usage Report
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <returns></returns>
        private int AddExcelProcessDetailsRowHeader(Worksheet worksheet, int rowIndex, int startColumn)
        {
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Process", 100);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer", 30);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Manufacturer", 30);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Category", 30);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Orders", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Parts", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Surface Area", 25);

            rowIndex++;
            return rowIndex;
        }

        /// <summary>
        ///     Adds a total summary row to the current report
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="startColumn"></param>
        private void AddExcelTotalSummaryRow(Worksheet worksheet, int rowIndex, int startColumn)
        {
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Totals: ");
            CreateFormulaCell(worksheet, rowIndex, startColumn++, "=SUBTOTAL(9, R6C2:R" + (rowIndex) + "C" + startColumn + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, startColumn++, "=SUBTOTAL(9, R6C3:R" + (rowIndex) + "C" + startColumn + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, startColumn++, "=SUBTOTAL(9, R6C4:R" + (rowIndex) + "C" + startColumn + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
        }

        /// <summary>
        /// Adds a total summary row to the current report
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="rowIndex">Index of the row.</param>
        private void AddExcelTotalDetailsRow(Worksheet worksheet, int rowIndex)
        {
            CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 3, "Totals:").CellFormat.Alignment = HorizontalCellAlignment.Right;
            CreateFormulaCell(worksheet, rowIndex, 4, "=SUBTOTAL(9, R6C5:R" + (rowIndex) + "C" + 5 + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 5, "=SUBTOTAL(9, R6C6:R" + (rowIndex) + "C" + 6 + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(worksheet, rowIndex, 6, "=SUBTOTAL(9, R6C7:R" + (rowIndex) + "C" + 7 + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
        }

        #endregion

        #region ProductClassUsage

        private class ProductClassUsage
        {
            public string ProductClass { get; set; }

            public int OrderCount { get; set; }

            public long PartCount { get; set; }

            public double TotalSurfaceArea { get; set; }
        }

        #endregion
    }

    /// <summary>
    ///     A summary report of a selected process.
    /// </summary>
    public class ProcessSummaryReport : Report
    {
        #region Fields

        private readonly ProcessesDataset.ProcessRow _process;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Process Summary"; }
        }

        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Landscape; }
        }

        protected override int FilenameIdentifier => _process?.ProcessID ?? 0;

        #endregion

        #region Methods

        public ProcessSummaryReport(ProcessesDataset.ProcessRow process) { this._process = process; }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            ITableCell cell = null;
            ITableRow row = null;

            SetupReportInfo();

            IGroup headerGroup = _section.AddGroup();
            headerGroup.Layout = Layout.Horizontal;

            // Process Info
            IText piText = headerGroup.AddText();
            piText.Style = DefaultStyles.NormalStyle;
            piText.Width = new RelativeWidth(70);
            piText.Borders = DefaultStyles.DefaultBorders;
            piText.Background = DefaultStyles.DefaultBackground;
            piText.Margins.Bottom = 5;
            piText.Margins.Right = 5;
            piText.Paddings = new Paddings(5, 5, 5, 10);

            piText.AddContent("Process " + this._process.ProcessID, DefaultStyles.BoldStyle);
            piText.AddLineBreak();
            piText.AddContent(this._process.Name + " Rev. " + this._process.Revision); // "DPS 4.50-62 Rev. A"
            piText.AddLineBreak();
            piText.AddContent(this._process.Description); // "Conversion Coating"

            // User Info
            IText uiText = headerGroup.AddText();
            uiText.Alignment = TextAlignment.Right;
            uiText.Style = DefaultStyles.NormalStyle;
            uiText.Width = new RelativeWidth(30);
            uiText.Borders = DefaultStyles.DefaultBorders;
            uiText.Margins.Bottom = 5;
            uiText.Margins.Right = 5;
            uiText.Paddings = new Paddings(5, 5, 5, 10);
            uiText.Background = DefaultStyles.DefaultBackground;

            uiText.AddContent(ApplicationSettings.Current.CompanyName, DefaultStyles.BoldStyle);
            uiText.AddLineBreak();
            uiText.AddContent(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            uiText.AddLineBreak();
            uiText.AddContent(SecurityManager.UserName);

            // Table
            ITable stepsTable = _section.AddTable();
            stepsTable.Borders = DefaultStyles.DefaultBorders;

            // Header
            stepsTable.Header.Repeat = false;

            cell = stepsTable.CreateTableHeaderCell(5);
            cell.AddText("Order", DefaultStyles.BoldStyle);

            cell = stepsTable.CreateTableHeaderCell(65);
            cell.AddText("Step", DefaultStyles.BoldStyle);

            cell = stepsTable.CreateTableHeaderCell(5);
            cell.AddText("COC", DefaultStyles.BoldStyle);

            var steps = new List <ProcessesDataset.ProcessStepsRow>(this._process.GetProcessStepsRows());

            //sort steps by order
            steps.Sort((cp1, cp2) => { return cp1.StepOrder.CompareTo(cp2.StepOrder); });

            //Table Cells
            foreach(ProcessesDataset.ProcessStepsRow psRow in steps)
            {
                row = stepsTable.AddRow();

                //StepOrder
                cell = row.CreateTableCell(5);
                cell.AddText(psRow.StepOrder.ToString());

                //Step Name and Description
                cell = row.CreateTableCell(65);

                //Note: Workaround as AddContent with Boldstyle will continue to set bold with AddRichContent later 
                IText nameText = cell.AddText(psRow.Name, DefaultStyles.BoldStyle);
                nameText.Alignment = TextAlignment.Left;
                nameText = cell.AddText("", DefaultStyles.NormalStyle);
                nameText.Alignment = TextAlignment.Left;

                if (!psRow.IsDescriptionNull())
                {
                    //Rich text editor uses &edsp for space instead of &nbsp; so need to replace
                    var desc = psRow.Description.Replace("&edsp;", " ");

                    //Infragisitics has very limited HTML support for RichContent for cells
                    //See: https://www.infragistics.com/help/winforms/infragistics4.documents.reports.v16.2~infragistics.documents.reports.report.text.itext~addrichcontent(string)
                    nameText.AddRichContent(desc);
                    nameText.AddLineBreak();
                }

                ITable quesTable = cell.AddTable();
                quesTable.Borders = DefaultStyles.DefaultBorders;
                quesTable.Margins.All = 5;

                cell = quesTable.CreateTableHeaderCell(10);
                cell.AddText("Order", DefaultStyles.BoldStyle);

                cell = quesTable.CreateTableHeaderCell(25);
                cell.AddText("Step", DefaultStyles.BoldStyle);

                cell = quesTable.CreateTableHeaderCell(25);
                cell.AddText("Type", DefaultStyles.BoldStyle);

                cell = quesTable.CreateTableHeaderCell(20);
                cell.AddText("Default Value", DefaultStyles.BoldStyle);

                cell = quesTable.CreateTableHeaderCell(10);
                cell.AddText("Req.", DefaultStyles.BoldStyle);

                cell = quesTable.CreateTableHeaderCell(10);
                cell.AddText("Edit", DefaultStyles.BoldStyle);

                float quesIndent = 0;
                decimal lastStep = 0;

                foreach(ProcessesDataset.ProcessQuestionRow quesRow in
                    psRow.GetProcessQuestionRows().OrderBy(r => r.StepOrder))
                {
                    ITableRow quesTRow = quesTable.AddRow();

                    cell = quesTRow.CreateTableCell(10);
                    cell.AddText(quesRow.StepOrder.ToString());

                    //if has same step as last one then indent
                    if(lastStep.ToString().Split('.')[0] == quesRow.StepOrder.ToString().Split('.')[0])
                        quesIndent = 5;
                    else
                        quesIndent = 0;

                    cell = quesTRow.CreateTableCell(25);
                    IText quesText = cell.AddText(quesRow.Name);
                    quesText.Alignment = TextAlignment.Left;
                    quesText.Indents.Left = quesIndent;

                    cell = quesTRow.CreateTableCell(25);
                    cell.AddText(ProcessReportsUtilities.GetQuestionText(quesRow));

                    cell = quesTRow.CreateTableCell(20);
                    cell.AddText(quesRow.IsDefaultValueNull() ? "None" : quesRow.DefaultValue);

                    cell = quesTRow.CreateTableCell(10);
                    cell.AddText(quesRow.Required ? "Yes" : "No");

                    cell = quesTRow.CreateTableCell(10);
                    cell.AddText(quesRow.OperatorEditable ? "Yes" : "No");

                    lastStep = quesRow.StepOrder;
                }

                //COC
                cell = row.CreateTableCell(5);
                cell.AddText(psRow.COCData ? "Yes" : "No");
            }
        }

        #endregion
    }

    /// <summary>
    ///     Process Parts Report shows all of the parts being used by the defined process.
    /// </summary>
    public class ProcessPartsReport : Report
    {
        #region Fields

        private readonly int _processID;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Process Parts"; }
        }

        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        protected override int FilenameIdentifier => _processID;

        #endregion

        #region Methods

        public ProcessPartsReport(int processID) { this._processID = processID; }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            SetupReportInfo();
            _section.PageMargins.All = 20;

            AddHeader("PROCESS PARTS", false);
            AddProcessSection();
        }

        private void AddProcessSection()
        {
            var dsProcess = new Data.Reports.ProcessPartsReport();
            var taProcess = new ProcessSummaryTableAdapter();
            var taProcessPart = new ProcessPartSummaryTableAdapter();

            ITableCell cell = null;
            ITableRow row = null;

            try
            {
                dsProcess.EnforceConstraints = false;
                taProcess.Fill(dsProcess.ProcessSummary, this._processID);
                taProcessPart.Fill(dsProcess.ProcessPartSummary, this._processID);

                IGroup headerGroup = _section.AddGroup();
                headerGroup.Layout = Layout.Horizontal;

                // Process Info
                IText piText = headerGroup.AddText();
                piText.Style = DefaultStyles.NormalStyle;
                piText.Width = new RelativeWidth(70);
                piText.Borders = DefaultStyles.DefaultBorders;
                piText.Background = DefaultStyles.DefaultBackground;
                piText.Margins.Bottom = 5;
                piText.Margins.Right = 5;
                piText.Paddings = new Paddings(5, 5, 5, 10);

                piText.AddContent("Process " + dsProcess.ProcessSummary[0].ProcessID, DefaultStyles.BoldStyle);
                piText.AddLineBreak();
                piText.AddContent(dsProcess.ProcessSummary[0].Name + " Rev. " + dsProcess.ProcessSummary[0].Revision);
                // "DPS 4.50-62 Rev. A"
                piText.AddLineBreak();
                piText.AddContent(dsProcess.ProcessSummary[0].Description); // "Conversion Coating"

                // User Info
                IText uiText = headerGroup.AddText();
                uiText.Alignment = TextAlignment.Right;
                uiText.Style = DefaultStyles.NormalStyle;
                uiText.Width = new RelativeWidth(30);
                uiText.Borders = DefaultStyles.DefaultBorders;
                uiText.Margins.Bottom = 5;
                uiText.Margins.Right = 5;
                uiText.Paddings = new Paddings(5, 5, 5, 10);
                uiText.Background = DefaultStyles.DefaultBackground;

                uiText.AddContent(ApplicationSettings.Current.CompanyName, DefaultStyles.BoldStyle);
                uiText.AddLineBreak();
                uiText.AddContent(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                uiText.AddLineBreak();
                uiText.AddContent(SecurityManager.UserName);

                // Table
                ITable partsTable = _section.AddTable();
                partsTable.Borders = DefaultStyles.DefaultBorders;

                // Header
                partsTable.Header.Repeat = false;

                cell = partsTable.CreateTableHeaderCell(33);
                cell.AddText("Customer Name", DefaultStyles.BoldStyle);

                cell = partsTable.CreateTableHeaderCell(66);
                cell.AddText("Part Name", DefaultStyles.BoldStyle);

                //Table Cells
                foreach(Data.Reports.ProcessPartsReport.ProcessPartSummaryRow psRow in
                    dsProcess.ProcessSummary[0].GetProcessPartSummaryRows())
                {
                    row = partsTable.AddRow();
                    //StepOrder
                    cell = row.CreateTableCell(33);
                    cell.AddText(psRow.CustomerName);

                    //Department
                    cell = row.CreateTableCell(66);
                    cell.AddText(psRow.PartName);
                }
            }
            catch(Exception exc)
            {
                _log.Fatal(exc, "Error adding process parts summary section to report.");
            }
            finally
            {
                if(dsProcess != null)
                    dsProcess.Dispose();
                if(taProcess != null)
                    taProcess.Dispose();
                if(taProcessPart != null)
                    taProcessPart.Dispose();

                dsProcess = null;
                taProcessPart = null;
                taProcess = null;
            }
        }

        #endregion
    }

    public class OrdersByProcessReport : ExcelBaseReport
    {
        public enum OrderStatus { Open, Completed, All }

        #region Properties

        public override string Title
        {
            get
            {
                return Status.ToString() + " Orders By Process";
            }
        }

        public int ProcessID { get; set; }

        public List<int> ProcessAliases { get; set; }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        public OrderStatus Status { get; set; }

        #endregion

        #region Methods

        public OrdersByProcessReport() { ProcessID = Settings.Default.OrderByProcessReportSelected; }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            CreateReportExcel();

            Settings.Default.OrderByProcessReportSelected = ProcessID;
        }

        private void CreateReportExcel()
        {
            try
            {
                var orders = new Data.Reports.ProcessPartsReport.OrderProcessSummaryDataTable();

                using(var taOrders = new OrderProcessSummaryTableAdapter() { ClearBeforeFill = false })
                {
                    switch(Status)
                    {
                        case OrderStatus.Open:
                            if(ProcessAliases != null && ProcessAliases.Count > 0)
                            {
                                foreach(var alias in ProcessAliases)
                                    taOrders.FillByAliasStatus(orders, alias, "Open");
                            }
                            else
                                taOrders.FillByStatus(orders, ProcessID, "Open");
                            break;
                        case OrderStatus.Completed:
                            if (ProcessAliases != null && ProcessAliases.Count > 0)
                            {
                                foreach (var alias in ProcessAliases)
                                    taOrders.FillByAliasStatus(orders, alias, "Closed");
                            }
                            else
                                taOrders.FillByStatus(orders, ProcessID, "Closed");
                            break;
                        case OrderStatus.All:
                            if (ProcessAliases != null && ProcessAliases.Count > 0)
                            {
                                foreach (var alias in ProcessAliases)
                                    taOrders.FillByAlias(orders, alias);
                            }
                            else
                            taOrders.Fill(orders, ProcessID);
                            break;
                        default:
                            break;
                    }
                    
                }

                var processName = orders.Count > 0 ? orders[0].ProcessName + (orders[0].IsProcessRevNull() ? "" : " Rev." + orders[0].ProcessRev) : "No Process";

                var wks = CreateWorksheet(processName);
                var rowIndex = AddCompanyHeaderRows(wks, 7, "- " + processName) + 2;
                var startRowIndex = rowIndex;

                CreateHeaderCell(wks, rowIndex, 0, "Customer");
                CreateHeaderCell(wks, rowIndex, 1, "WO");
                CreateHeaderCell(wks, rowIndex, 2, "PO");
                CreateHeaderCell(wks, rowIndex, 3, "Status");
                CreateHeaderCell(wks, rowIndex, 4, "Part");
                CreateHeaderCell(wks, rowIndex, 5, "Process");
                CreateHeaderCell(wks, rowIndex, 6, "Process Rev");
                CreateHeaderCell(wks, rowIndex, 7, "Process Alias");
                CreateHeaderCell(wks, rowIndex, 8, "Date Completed");
                CreateHeaderCell(wks, rowIndex, 9, "Location");

                foreach(Data.Reports.ProcessPartsReport.OrderProcessSummaryRow order in orders)
                {
                    rowIndex++;
                    this.CreateCell(wks, rowIndex, 0, order.CustomerName);
                    this.CreateCell(wks, rowIndex, 1, order.OrderID);
                    this.CreateCell(wks, rowIndex, 2, order.IsPurchaseOrderNull() ? "NA" : order.PurchaseOrder);
                    this.CreateCell(wks, rowIndex, 3, order.IsStatusNull() ? "NA" : order.Status);
                    this.CreateCell(wks, rowIndex, 4, order.PartName);
                    this.CreateCell(wks, rowIndex, 5, order.ProcessName);
                    this.CreateCell(wks, rowIndex, 6, order.IsProcessRevNull() ? "NA" : order.ProcessRev);
                    this.CreateCell(wks, rowIndex, 7, order.ProcessAliasName);
                    this.CreateCell(wks, rowIndex, 8, order.IsCompletedDateNull() ? "NA" : order.CompletedDate.ToShortDateString());
                    this.CreateCell(wks, rowIndex, 9, order.CurrentLocation);
                }

                // Set column widths
                wks.Columns[0].SetWidth(26, WorksheetColumnWidthUnit.Character);
                wks.Columns[1].SetWidth(8, WorksheetColumnWidthUnit.Character);
                wks.Columns[2].SetWidth(10, WorksheetColumnWidthUnit.Character);
                wks.Columns[3].SetWidth(10, WorksheetColumnWidthUnit.Character);
                wks.Columns[4].SetWidth(26, WorksheetColumnWidthUnit.Character);
                wks.Columns[5].SetWidth(26, WorksheetColumnWidthUnit.Character);
                wks.Columns[6].SetWidth(14, WorksheetColumnWidthUnit.Character);
                wks.Columns[7].SetWidth(20, WorksheetColumnWidthUnit.Character);
                wks.Columns[8].SetWidth(20, WorksheetColumnWidthUnit.Character);
                wks.Columns[9].SetWidth(20, WorksheetColumnWidthUnit.Character);

                wks.Tables.Add("A{0}:{1}{2}".FormatWith(startRowIndex + 1, this.ExcelColumnIndexToName(9), rowIndex + 1), true);
            }
            catch(Exception exc)
            {
                string errorMsg = "Error running OrdersByProcessReport: ProcessID is " + ProcessID;
                _log.Error(exc, errorMsg);
            }
        }

        #endregion
    }

    public class ProcessRevenueReport : ExcelBaseReport
    {
        #region ProcessRevenueRow

        /// <summary>
        ///     Use this subclass in place of Dataset Rows
        /// </summary>
        private class ProcessRevenueRow
        {
            public string CustomerName { get; set; }
            public int Orders { get; set; }
            public long Parts { get; set; }
            public string ProcessName { get; set; }
            public int PartId { get; set; }
            public string Manufacturer { get; set; }
            public string ProcessCategory { get; set; }
            public decimal Revenue { get; set; }
        }

        #endregion

        #region Fields

        private const string TITLE_DEPARTMENT = "Revenue By Department";
        private const string TITLE_PRODUCT_CLASS = "Revenue by Product Class";

        #endregion

        #region Properties

        public override string Title => "Revenue By Process";

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation => PageOrientation.Portrait;

        [Browsable(true)]
        [Description("The from date of the report.")]
        [DisplayName("From Date")]
        [Category("Report")]
        public DateTime FromDate
        {
            get;
            set;
        }

        [Browsable(true)]
        [Description("The to date of the report.")]
        [DisplayName("To Date")]
        [Category("Report")]
        public DateTime ToDate
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public ProcessRevenueReport()
        {
            this.FromDate = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            this.ToDate = DateUtilities.GetLastDayOfMonth(DateTime.Now);
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            Data.Reports.ProcessPartsReport ppReport = GetReportData();
            CreateExcelProcessRevenueSummary(ppReport);
            CreateExcelDepartmentSummary(ppReport);

            bool createProductClassSummary;
            using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
            {
                var productClassField = ta.GetByCategory("Order").FirstOrDefault(f => f.Name == "Product Class");
                createProductClassSummary = productClassField == null || productClassField.IsRequired || productClassField.IsVisible;
            }

            if (createProductClassSummary)
            {
                CreateExcelProductClassSummary();
            }

            this.CreateExcelProcessDetails(ppReport);
        }

        private Data.Reports.ProcessPartsReport GetReportData()
        {
            var ppReport = new Data.Reports.ProcessPartsReport();
            var taPUS = new ProcessUsageSummaryTableAdapter();

            taPUS.FillByProcessRevenue(ppReport.ProcessUsageSummary,
                ToDate.EndOfDay(),
                FromDate.StartOfDay());

            return ppReport;
        }

        private void CreateExcelProcessRevenueSummary(Data.Reports.ProcessPartsReport ppReport)
        {
            Worksheet procSumWks = CreateWorksheet(Title);

            SetupReportInfo();

            var rowIndex = AddExcelProcessRevenueSummaryMainHeader(procSumWks, 2);
            AddExcelProcessRevenueSummaryRowHeader(procSumWks, rowIndex, 0);
            rowIndex++;

            var procRowList = new List<ProcessRevenueRow>();
            foreach(Data.Reports.ProcessPartsReport.ProcessUsageSummaryRow psRow in ppReport.ProcessUsageSummary)
            {
                var procRow = new ProcessRevenueRow();
                procRow.CustomerName = psRow.CustomerName;

                if(!psRow.IsOrdersNull())
                    procRow.Orders = psRow.Orders;
                if (!psRow.IsPartIDNull())
                    procRow.PartId = psRow.PartID;
                if (!psRow.IsPartCountNull())
                    procRow.Parts = psRow.PartCount;

                procRow.ProcessName = psRow.ProcessName;

                if (!psRow.IsTotalPriceNull())
                {
                    procRow.Revenue = psRow.TotalPrice;
                }

                procRowList.Add(procRow);
            }

            FillProcessRevenueSummary(procSumWks, procRowList, rowIndex);
        }

        private void CreateExcelDepartmentSummary(Data.Reports.ProcessPartsReport ppReport)
        {
            Worksheet worksheet = CreateWorksheet(TITLE_DEPARTMENT);
            SetupReportInfo();

            // Header
            var rowIndex = AddExcelDepartmentSummaryMainHeader(worksheet);
            rowIndex = AddExcelDepartmentSummaryRowHeader(worksheet, rowIndex, 0);

            // Content
            var departmentGroups = ppReport.ProcessUsageSummary
                .GroupBy(row => row.Department)
                .OrderBy(group => group.Key)
                .ToList();

            foreach (var deptGroup in departmentGroups)
            {
                var departmentName = deptGroup.Key;
                var departmentRevenue = deptGroup.Sum(row => row.IsTotalPriceNull() ? 0M : row.TotalPrice);

                worksheet.Rows[rowIndex].Cells[0].Value = departmentName;
                CreateCell(worksheet, rowIndex, 1, departmentRevenue, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
                rowIndex++;
            }

            if (departmentGroups.Count == 0)
            {
                rowIndex++;
            }

            // Summary
            CreateCell(worksheet, rowIndex, 0, "Total:").CellFormat.Alignment = HorizontalCellAlignment.Right;
            CreateFormulaCell(worksheet, rowIndex, 1, "=SUBTOTAL(9, R6C2:R" + (rowIndex) + "C2)", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center, cellFormat:MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
        }

        private void CreateExcelProductClassSummary()
        {
            Data.Reports.ProcessPartsReport.OrderProcessRevenueDataTable dtOrderProcessRevenue = null;
            Data.Reports.ProcessPartsReport.OrderProductClassDataTable dtProductClass = null;

            try
            {
                // Retrieve data
                dtOrderProcessRevenue = new Data.Reports.ProcessPartsReport.OrderProcessRevenueDataTable();
                using (var taOrderProcessRevenue = new OrderProcessRevenueTableAdapter())
                {
                    taOrderProcessRevenue.Fill(dtOrderProcessRevenue,
                        ToDate.EndOfDay(),
                        FromDate.StartOfDay());

                }

                dtProductClass = new Data.Reports.ProcessPartsReport.OrderProductClassDataTable();
                using (var taProductClass = new OrderProductClassTableAdapter())
                {
                    taProductClass.FillByProcessDate(dtProductClass,
                        ToDate.EndOfDay(),
                        FromDate.StartOfDay());
                }

                var productClasses = new List<ProductClassRevenue>();

                foreach (var orderRow in dtOrderProcessRevenue)
                {
                    if (orderRow.IsTotalPriceNull())
                    {
                        continue;
                    }

                    var primaryProductClass = dtProductClass.FirstOrDefault(pc => pc.OrderID == orderRow.OrderID)?.ProductClass ?? "N/A";
                    var matchingProductClass = productClasses.FirstOrDefault(pc => pc.Name == primaryProductClass);

                    if (matchingProductClass == null)
                    {
                        productClasses.Add(new ProductClassRevenue
                        {
                            Name = primaryProductClass,
                            TotalRevenue = orderRow.TotalPrice
                        });
                    }
                    else
                    {
                        matchingProductClass.TotalRevenue += orderRow.TotalPrice;
                    }
                }


                // Setup worksheet
                var worksheet = CreateWorksheet(TITLE_PRODUCT_CLASS);
                SetupReportInfo();

                // Header
                var rowIndex = AddExcelProductClassSummaryMainHeader(worksheet);
                rowIndex = AddExcelProductClassSummaryRowHeader(worksheet, rowIndex, 0);

                // Content
                foreach (var productClass in productClasses.OrderBy(s => s.Name))
                {
                    worksheet.Rows[rowIndex].Cells[0].Value = productClass.Name;
                    CreateCell(worksheet, rowIndex, 1, productClass.TotalRevenue, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
                    rowIndex++;
                }

                if (productClasses.Count == 0)
                {
                    rowIndex++;
                }

                // Summary
                CreateCell(worksheet, rowIndex, 0, "Total:").CellFormat.Alignment = HorizontalCellAlignment.Right;
                CreateFormulaCell(worksheet, rowIndex, 1, "=SUBTOTAL(9, R6C2:R" + (rowIndex) + "C2)", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center, cellFormat:MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            }
            finally
            {
                dtOrderProcessRevenue?.Dispose();
                dtProductClass?.Dispose();
            }

        }

        private void CreateExcelProcessDetails(Data.Reports.ProcessPartsReport ppReport)
        {
            Worksheet procSumWks = CreateWorksheet("Process Revenue Details");
            SetupReportInfo();

            var rowIndex = AddExcelProcessRevenueMainHeader(procSumWks, 2);
            rowIndex = AddExcelProcessRevenueRowHeader(procSumWks, rowIndex, 0);

            var taPart = new PartTableAdapter();
            var procRowList = new List<ProcessRevenueRow>();
            foreach (Data.Reports.ProcessPartsReport.ProcessUsageSummaryRow psRow in ppReport.ProcessUsageSummary)
            {
                var procRow = new ProcessRevenueRow();
                procRow.CustomerName = psRow.CustomerName;
                procRow.Orders = psRow.Orders;
                procRow.PartId = psRow.PartID;
                procRow.Parts = psRow.IsPartCountNull() ? 0 : psRow.PartCount;
                procRow.ProcessName = psRow.ProcessName;
                procRow.Manufacturer = taPart.GetManufacturer(procRow.PartId);
                procRow.ProcessCategory = psRow.Category;
                procRow.Revenue = psRow.IsTotalPriceNull() ? 0M : psRow.TotalPrice;
                
                procRowList.Add(procRow);
            }
            FillProcessDetails(procSumWks, procRowList, rowIndex);
        }

        private void FillProcessDetails(Worksheet worksheet, List<ProcessRevenueRow> procRowList, int rowIndex)
        {
            string lastProcess = null;
            var part = 0L;
            var order = 0;
            decimal revenue = 0;

            var startRowIndex = rowIndex;
            foreach (var procRow in procRowList)
            {
                if (procRow.ProcessName != lastProcess)
                {
                    part = 0;
                    order = 0;
                    revenue = 0;

                    if (lastProcess != null)
                    {
                        rowIndex++;
                    }

                    AddExcelProcessRevenueRow(worksheet, procRow, rowIndex, 0);
                    order++;
                    lastProcess = procRow.ProcessName;
                    part += procRow.Parts;
                    revenue += procRow.Revenue;
                }
                else
                {
                    part += procRow.Parts;
                    revenue += procRow.Revenue;
                    order++;
                    worksheet.Rows[rowIndex].Cells[4].Value = order;
                    worksheet.Rows[rowIndex].Cells[5].Value = part;
                    worksheet.Rows[rowIndex].Cells[6].Value = revenue;
                }
            }

            rowIndex++;
            this.CreateTable(worksheet, startRowIndex, 6, rowIndex, true);
            AddExcelTotalSummaryRow(worksheet, rowIndex);
        }

        private void FillProcessRevenueSummary(Worksheet worksheet, List<ProcessRevenueRow> procRowList, int rowIndex)
        {
            string lastProcess = null;
            long part = 0;
            int order = 0;
            decimal revenue = 0;

            foreach (ProcessRevenueRow procRow in procRowList)
            {
                if(procRow.ProcessName != lastProcess)
                {
                    part = 0;
                    order = 0;
                    revenue = 0;

                    if(lastProcess == null)
                    {
                        AddExcelProcessReviewSummaryRow(worksheet, procRow, rowIndex, 0);
                        order++;
                        lastProcess = procRow.ProcessName;
                        part += procRow.Parts;
                        revenue += procRow.Revenue;
                    }
                    else
                    {
                        rowIndex++;
                        AddExcelProcessReviewSummaryRow(worksheet, procRow, rowIndex, 0);
                        order++;
                        lastProcess = procRow.ProcessName;
                        part += procRow.Parts;
                        revenue += procRow.Revenue;
                    }
                }
                else
                {
                    part += procRow.Parts;
                    revenue += procRow.Revenue;
                    order++;

                    worksheet.Rows[rowIndex].Cells[1].Value = order;
                    worksheet.Rows[rowIndex].Cells[2].Value = part;
                    worksheet.Rows[rowIndex].Cells[3].Value = revenue;
                }
            }

            rowIndex++;
            AddExcelTotalSummaryRow(worksheet, rowIndex, 0);
        }

        private void AddExcelProcessReviewSummaryRow(Worksheet worksheet, ProcessRevenueRow procRow, int rowIndex, int startColumn)
        {
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.ProcessName;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.Orders;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.Parts;
            this.CreateCell(worksheet, rowIndex, startColumn++, procRow.Revenue, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
        }

        private void AddExcelProcessRevenueRow(Worksheet worksheet, ProcessRevenueRow procRow, int rowIndex, int startColumn)
        {
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.ProcessName;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.CustomerName;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.Manufacturer;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.ProcessCategory;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.Orders;
            worksheet.Rows[rowIndex].Cells[startColumn++].Value = procRow.Parts;
            this.CreateCell(worksheet, rowIndex, startColumn++, procRow.Revenue, false, HorizontalCellAlignment.Right, CellBorderLineStyle.Thin, MONEY_FORMAT);
        }

        private int AddExcelProcessRevenueSummaryMainHeader(Worksheet worksheet, int lastColumnIndex)
        {
            int rowIndex = 0;
            WorksheetMergedCellsRegion region;

            //Add Header Title
            region = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 3, "Process Revenue Summary");

            //Set the cell alignment of the merged region. Since a cell and its merged region shared a cell format, this will ultimately set the format of the merged region
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            worksheet.Rows[rowIndex].Height = 18 * 20;
            region.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            region.CellFormat.Font.Name = DefaultFontName;
            region.CellFormat.Font.Height = 18 * 20;
            region.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "From Date:  " + this.FromDate.ToShortDateString();
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 3, "Company:    " + ApplicationSettings.Current.CompanyName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "To Date:  " + this.ToDate.ToShortDateString();
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 3, "User:             " + SecurityManager.UserName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 3, "Date:             " + DateTime.Now).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;
            return rowIndex;
        }

        private int AddExcelDepartmentSummaryMainHeader(Worksheet worksheet)
        {
            int rowIndex = 0;

            //Add Header Title
            WorksheetMergedCellsRegion region = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 1, TITLE_DEPARTMENT);

            //Set the cell alignment of the merged region. Since a cell and its merged region shared a cell format, this will ultimately set the format of the merged region
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            worksheet.Rows[rowIndex].Height = 18 * 20;
            region.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            region.CellFormat.Font.Name = DefaultFontName;
            region.CellFormat.Font.Height = 18 * 20;
            region.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "From Date:  " + this.FromDate.ToShortDateString();
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateCell(worksheet, rowIndex, 1, "Company:    " + ApplicationSettings.Current.CompanyName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "To Date:  " + this.ToDate.ToShortDateString();
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateCell(worksheet, rowIndex, 1, "User:             " + SecurityManager.UserName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;
            CreateCell(worksheet, rowIndex, 1, "Date:             " + DateTime.Now).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;
            return rowIndex;
        }

        private int AddExcelProductClassSummaryMainHeader(Worksheet worksheet)
        {
            int rowIndex = 0;

            //Add Header Title
            WorksheetMergedCellsRegion region = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 1, TITLE_PRODUCT_CLASS);

            //Set the cell alignment of the merged region. Since a cell and its merged region shared a cell format, this will ultimately set the format of the merged region
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            worksheet.Rows[rowIndex].Height = 18 * 20;
            region.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            region.CellFormat.Font.Name = DefaultFontName;
            region.CellFormat.Font.Height = 18 * 20;
            region.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "From Date:  " + this.FromDate.ToShortDateString();
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateCell(worksheet, rowIndex, 1, "Company:    " + ApplicationSettings.Current.CompanyName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "To Date:  " + this.ToDate.ToShortDateString();
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateCell(worksheet, rowIndex, 1, "User:             " + SecurityManager.UserName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;
            CreateCell(worksheet, rowIndex, 1, "Date:             " + DateTime.Now).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;
            return rowIndex;
        }

        private int AddExcelProcessRevenueMainHeader(Worksheet worksheet, int lastColumnIndex)
        {
            int rowIndex = 0;
            WorksheetMergedCellsRegion region = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 6, "Process Revenue Details");

            //Set the cell alignment of the merged region. Since a cell and its merged region shared a cell format, this will ultimately set the format of the merged region
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            worksheet.Rows[rowIndex].Height = 18 * 20;
            region.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            region.CellFormat.Font.Name = DefaultFontName;
            region.CellFormat.Font.Height = 18 * 20;
            region.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "From Date:  " + this.FromDate.ToShortDateString();
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 6, "Company:    " + ApplicationSettings.Current.CompanyName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            worksheet.Rows[rowIndex].Cells[0].Value = "To Date:  " + this.ToDate.ToShortDateString();
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.White), null, FillPatternStyle.Solid);
            ApplyCellBorders(worksheet.Rows[rowIndex].Cells[0]);
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 6, "User:             " + SecurityManager.UserName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;
            CreateMergedCell(worksheet, rowIndex, 1, rowIndex, 6, "Date:             " + DateTime.Now).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            rowIndex++;

            return rowIndex;
        }

        private int AddExcelProcessRevenueSummaryRowHeader(Worksheet worksheet, int rowIndex, int startColumn)
        {
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Process", 100);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Orders", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Parts", 25);
            var revenueCell = CreateHeaderCell(worksheet, rowIndex, startColumn++, "Revenue", 25);

            AddRevenueCommentToHeaderCell(revenueCell);

            rowIndex++;
            return rowIndex;
        }

        private int AddExcelDepartmentSummaryRowHeader(Worksheet worksheet, int rowIndex, int startColumn)
        {
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Department", 35);
            var revenueCell = CreateHeaderCell(worksheet, rowIndex, startColumn++, "Revenue", 40);
            AddRevenueCommentToHeaderCell(revenueCell);

            return rowIndex + 1;
        }

        private int AddExcelProductClassSummaryRowHeader(Worksheet worksheet, int rowIndex, int startColumn)
        {
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Product Class", 35);
            var revenueCell = CreateHeaderCell(worksheet, rowIndex, startColumn++, "Revenue", 40);
            AddRevenueCommentToHeaderCell(revenueCell);

            return rowIndex + 1;
        }

        private int AddExcelProcessRevenueRowHeader(Worksheet worksheet, int rowIndex, int startColumn)
        {
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Process", 100);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer", 30);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Manufacturer", 30);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Category", 30);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Orders", 25);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Parts", 25);
            var revenueCell = CreateHeaderCell(worksheet, rowIndex, startColumn++, "Revenue", 25);

            AddRevenueCommentToHeaderCell(revenueCell);

            rowIndex++;
            return rowIndex;
        }

        private static void AddRevenueCommentToHeaderCell(WorksheetCell cell)
        {
            const string commentText = "If process-level price data is unavailable, may be calculated by subtracting fees from the total order price and dividing by the number of processes";
            cell.Comment = new WorksheetCellComment
            {
                Text = new FormattedString(commentText),
                Visible = true
            };

            // Resize comment bounds to fit content.
            var bounds = cell.Comment.GetBoundsInTwips();
            bounds.Width = 2500; // found through trial & error
            bounds.Height = 1875; // found through trial & error
            cell.Comment.SetBoundsInTwips(cell.Worksheet, bounds);
        }

        private void AddExcelTotalSummaryRow(Worksheet worksheet, int rowIndex, int startColumn)
        {
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Totals: ");
            this.CreateFormulaCell(worksheet, rowIndex, startColumn++, "=SUBTOTAL(9, R6C2:R" + (rowIndex) + "C" + startColumn + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateFormulaCell(worksheet, rowIndex, startColumn++, "=SUBTOTAL(9, R6C3:R" + (rowIndex) + "C" + startColumn + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateFormulaCell(worksheet, rowIndex, startColumn++, "=SUBTOTAL(9, R6C4:R" + (rowIndex) + "C" + startColumn + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center, cellFormat:MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
        }

        private void AddExcelTotalSummaryRow(Worksheet worksheet, int rowIndex)
        {
            CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 3, "Totals:").CellFormat.Alignment = HorizontalCellAlignment.Right;
            this.CreateFormulaCell(worksheet, rowIndex, 4, "=SUBTOTAL(9, R6C5:R" + (rowIndex) + "C" + 5 + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateFormulaCell(worksheet, rowIndex, 5, "=SUBTOTAL(9, R6C6:R" + (rowIndex) + "C" + 6 + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateFormulaCell(worksheet, rowIndex, 6, "=SUBTOTAL(9, R6C7:R" + (rowIndex) + "C" + 7 + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center, cellFormat:MONEY_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
        }

        #endregion

        #region ProductClassRevenue

        private class ProductClassRevenue
        {
            public string Name { get; set; }

            public decimal TotalRevenue { get; set; }
        }

        #endregion
    }

    public class ProcessSheetReport : ExcelBaseReport
    {
        #region Fields

        private const int COLUMN_COUNT = 6;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Process Sheet"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Landscape; }
        }

        public ProcessesDataset.ProcessRow Process
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public ProcessSheetReport(ProcessesDataset.ProcessRow processRow)
        {
            Process = processRow;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            _log.Debug("Process Sheet report is being run");
            CreateWorkBook();
            CreateProcessSheetReport();
        }

        private void CreateProcessSheetReport()
        {
            try
            {
                if (this.Process != null)
                {
                    var ws = CreateWorksheet("Process");
                    var rowIndex = this.AddCompanyHeaderRows(ws, COLUMN_COUNT, "") + 1;

                    CreateHeaderFooter(ws, "Work In Process");
                    ws.PrintOptions.Header = null;
                    ws.PrintOptions.TopMargin = 0.50;
                    ws.PrintOptions.LeftMargin = 0.25;
                    ws.PrintOptions.RightMargin = 0.25;
                    rowIndex++;

                    //Create Process Name Row
                    CreateMergedHeader(ws, rowIndex, 0, rowIndex, 1, "Process:").CellFormat.Alignment = HorizontalCellAlignment.Left;
                    var processNameCell = CreateMergedHeader(ws, rowIndex, 2, rowIndex, COLUMN_COUNT, this.Process.Name + (this.Process.IsRevisionNull() ? "" : " - Rev. " + this.Process.Revision));
                    processNameCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                    processNameCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                    rowIndex++;

                    //Create Process Description Row
                    CreateMergedHeader(ws, rowIndex, 0, rowIndex, 1, "Description:").CellFormat.Alignment = HorizontalCellAlignment.Left;
                    var processDescription = CreateMergedHeader(ws, rowIndex, 2, rowIndex, COLUMN_COUNT, this.Process.IsDescriptionNull() ? "" : this.Process.Description);
                    processDescription.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                    processDescription.CellFormat.Alignment = HorizontalCellAlignment.Left;
                    rowIndex++;

                    //Create Department Row
                    CreateMergedHeader(ws, rowIndex, 0, rowIndex, 1, "Department:").CellFormat.Alignment = HorizontalCellAlignment.Left;
                    var deptCell = CreateMergedHeader(ws, rowIndex, 2, rowIndex, COLUMN_COUNT, this.Process.Department);
                    deptCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                    deptCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                    rowIndex++;
                    rowIndex++;

                    //Begin adding questions
                    rowIndex = AddHeaderRow(ws, rowIndex);

                    foreach (var processStep in this.Process.GetProcessStepsRows().OrderBy(step => step.StepOrder))
                    {
                        rowIndex = AddProcessStepRow(ws, processStep, rowIndex);

                        foreach (var processQuestion in processStep.GetProcessQuestionRows().OrderBy(q => q.StepOrder))
                        {
                            rowIndex = AddProcessQuestionRow(ws, processQuestion, rowIndex);

                            if (!processQuestion.IsNotesNull())
                            {
                                rowIndex = AddProcessQuestionNotesRow(ws, processQuestion, rowIndex);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Unable to fill process sheet report.");
            }
        }

        private int AddHeaderRow(Worksheet worksheet, int rowIndex)
        {
            CreateHeaderCell(worksheet, rowIndex, 0, "Step", 12);
            CreateMergedHeader(worksheet, rowIndex, 1, rowIndex, 2, "Question");
            worksheet.Columns[2].Width = 25 * 256;

            CreateHeaderCell(worksheet, rowIndex, 3, "Answer", 35);
            CreateHeaderCell(worksheet, rowIndex, 4, "Range", 25);
            CreateHeaderCell(worksheet, rowIndex, 5, "Initials", 15);
            CreateHeaderCell(worksheet, rowIndex, 6, "Date", 12);
            rowIndex++;

            return rowIndex;
        }

        private int AddProcessStepRow(Worksheet worksheet, ProcessesDataset.ProcessStepsRow processStep, int startingRowIndex)
        {
            var rowIndex = startingRowIndex;

            try
            {
                var processStepCellFill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Beige), null, FillPatternStyle.Solid);
                var stepCell = worksheet.Rows[rowIndex].Cells[0];
                stepCell.Value = processStep.StepOrder.ToString();
                stepCell.CellFormat.Fill = processStepCellFill;
                stepCell.CellFormat.Alignment = HorizontalCellAlignment.Center;
                ApplyCellBorders(stepCell);

                var nameCell = CreateMergedHeader(worksheet, rowIndex, 1, rowIndex, COLUMN_COUNT, processStep.Name);
                nameCell.CellFormat.Fill = processStepCellFill;
                nameCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                ApplyCellBorders(nameCell.CellFormat);

                rowIndex++;

                foreach (var condition in processStep.GetProcessStepConditionRows())
                {
                    var conditionText =  DWOS.Data.Conditionals.ConditionEvaluator.ConditionToString(condition);
                    var conditionCell = CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, COLUMN_COUNT, "\t* - " + conditionText);
                    conditionCell.CellFormat.Font.Height = 180;
                    conditionCell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Red);
                    conditionCell.CellFormat.Fill = processStepCellFill;
                    conditionCell.CellFormat.Alignment = HorizontalCellAlignment.Right;
                    worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Right;
                    ApplyCellBorders(conditionCell.CellFormat);
                    rowIndex++;
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add process step row.");
            }

            return rowIndex;
        }

        private int AddProcessQuestionRow(Worksheet worksheet, ProcessesDataset.ProcessQuestionRow processQuestion, int startingRowIndex)
        {
            var rowIndex = startingRowIndex;

            try
            {
                var stepCell = worksheet.Rows[rowIndex].Cells[1];
                stepCell.Value = processQuestion.StepOrder.ToString();
                stepCell.CellFormat.Alignment = HorizontalCellAlignment.Center;
                ApplyCellBorders(stepCell);

                var nameCell = worksheet.Rows[rowIndex].Cells[2];
                nameCell.Value = processQuestion.Name;
                nameCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                ApplyCellBorders(nameCell.CellFormat);

                var answerCell = worksheet.Rows[rowIndex].Cells[3];
                answerCell.Value = "";
                answerCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                ApplyCellBorders(answerCell.CellFormat);

                var rangeCell = worksheet.Rows[rowIndex].Cells[4];
                rangeCell.Value = GetQuestionTypeText(processQuestion);
                rangeCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                ApplyCellBorders(rangeCell.CellFormat);

                var initialsCell = worksheet.Rows[rowIndex].Cells[5];
                initialsCell.Value = "";
                initialsCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                ApplyCellBorders(initialsCell.CellFormat);

                var dateCell = worksheet.Rows[rowIndex].Cells[6];
                dateCell.Value = "";
                dateCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                ApplyCellBorders(dateCell.CellFormat);

                rowIndex++;
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add process question row.");
            }

            return rowIndex;
        }

        private int AddProcessQuestionNotesRow(Worksheet worksheet, ProcessesDataset.ProcessQuestionRow processQuestion, int startingRowIndex)
        {
            var rowIndex = startingRowIndex;

            try
            {
                if (processQuestion.IsNotesNull())
                {
                    return rowIndex;
                }

                var formattedNotes = processQuestion.Notes.StripHtml();

                var notesCell = CreateMergedHeader(worksheet, rowIndex, 2, rowIndex, COLUMN_COUNT, formattedNotes);
                notesCell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                notesCell.CellFormat.Alignment = HorizontalCellAlignment.Left;
                ApplyCellBorders(notesCell.CellFormat);

                rowIndex++;
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add process question notes row.");
            }

            return rowIndex;
        }

        private static string GetQuestionTypeText(ProcessesDataset.ProcessQuestionRow question)
        {
            var inputType = (InputType)Enum.Parse(typeof(InputType), question.InputType);
            string text;

            if (inputType == InputType.List)
            {
                text = inputType.ToString();
            }
            else
            {
                text = ProcessReportsUtilities.GetQuestionText(question);
            }

            return text;
        }

        #endregion
    }

    /// <summary>
    /// Part History Report
    /// </summary>
    public class PartHistoryReport : ExcelBaseReport, IPartReport
    {
        #region Properties

        public override string Title
        {
            get { return "Part History"; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="PartHistoryReport"/> class.
        /// </summary>
        public PartHistoryReport()
        {
            CustomerID = -1;
            PartID = -1;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            if (CustomerID > -1 && PartID > -1)
            {
                // Get customer
                PartsDataset.CustomerRow customer = null;
                using (var ta = new CustomerTableAdapter())
                {
                    var dtCustomer = new PartsDataset.CustomerDataTable();
                    ta.FillByCustomer(dtCustomer, CustomerID);
                    customer = dtCustomer.FirstOrDefault();
                }

                List<PartsDataset.PartRow> parts = null;
                using (var ta = new PartTableAdapter())
                {
                    var dtParts = new PartsDataset.PartDataTable();
                    ta.FillWithAllRevisions(dtParts, PartID);
                    parts = dtParts.ToList();
                }

                var dataset = new DWOS.Data.Reports.ProcessPartsReport();

                var primaryPart = parts.FirstOrDefault(p => p.PartID == this.PartID);
                List<DWOS.Data.Reports.ProcessPartsReport.PartHistoryRow> partHistoryEntries;

                using (var ta = new PartHistoryTableAdapter())
                {
                    ta.Fill(dataset.PartHistory, FromDate, ToDate, PartID);
                    partHistoryEntries = dataset.PartHistory.OrderBy(hist => hist.OrderID).ToList();
                }

                List<DWOS.Data.Reports.ProcessPartsReport.OrderFeesRow> orderFees;
                using (var ta = new OrderFeesTableAdapter())
                {
                    ta.FillForPartHistory(dataset.OrderFees, FromDate, ToDate, PartID);
                    orderFees = dataset.OrderFees.ToList();
                }

                List<DWOS.Data.Reports.ProcessPartsReport.OrderFeeTypeRow> orderFeeTypes;
                using (var ta = new OrderFeeTypeTableAdapter())
                {
                    ta.Fill(dataset.OrderFeeType);
                    orderFeeTypes = dataset.OrderFeeType.ToList();
                }

                var worksheet = CreateWorksheet("Part History");
                var rowIndex = AddHeaderRow(worksheet, customer, primaryPart);

                foreach (var partHistory in partHistoryEntries)
                {
                    rowIndex = AddPartHistoryRow(worksheet, partHistory, rowIndex);
                }
            }
        }

        private int AddHeaderRow(Worksheet wks, PartsDataset.CustomerRow customer, PartsDataset.PartRow primaryPart)
        {
            var rowIndex = AddCompanyHeaderRows(wks, 8, String.Empty) + 2;

            CreateMergedHeader(wks, rowIndex, 0, rowIndex, 1, "Customer:");
            CreateMergedCell(wks, rowIndex, 2, rowIndex, 8, customer.Name);
            rowIndex++;

            CreateMergedHeader(wks, rowIndex, 0, rowIndex, 1, "Part:");
            CreateMergedCell(wks, rowIndex, 2, rowIndex, 8, primaryPart.Name);
            rowIndex++;
            rowIndex++;

            int colIndex = 0;
            CreateHeaderCell(wks, rowIndex, colIndex++, "Part Name", 25);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Part Revision", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Order", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Order Created Date", 25);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Order Price", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Order Price Unit", 25);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Order Processing Days", 25);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Order Process Count", 25);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Order Rework", 20);
            rowIndex++;
            return rowIndex;
        }

        private int AddPartHistoryRow(Worksheet worksheet, DWOS.Data.Reports.ProcessPartsReport.PartHistoryRow partHistoryEntry, int rowIndex)
        {
            const string notAvailable = "NA";
            int updatedRowIndex = rowIndex;
            int colIndex = 0;

            CreateCell(worksheet, updatedRowIndex, colIndex++, partHistoryEntry.PartName);

            if (partHistoryEntry.IsPartRevisionNull())
            {
                CreateCell(worksheet, updatedRowIndex, colIndex++, string.Empty);
            }
            else
            {
                CreateCell(worksheet, updatedRowIndex, colIndex++, partHistoryEntry.PartRevision);
            }

            CreateCell(worksheet, updatedRowIndex, colIndex++, partHistoryEntry.OrderID);

            if (partHistoryEntry.IsOrderDateNull())
            {
                CreateCell(worksheet, updatedRowIndex, colIndex++, string.Empty);
            }
            else
            {
                CreateCell(worksheet, updatedRowIndex, colIndex++, partHistoryEntry.OrderDate.Date);
            }

            decimal basePrice;

            if (partHistoryEntry.IsBasePriceNull())
            {
                basePrice = 0M;
            }
            else
            {
                basePrice = partHistoryEntry.BasePrice;
            }

            decimal fees = OrderPrice.CalculateFees(partHistoryEntry, 0M);

            int partQuantitity = 0;
            if (!partHistoryEntry.IsPartQuantityNull())
            {
                partQuantitity = partHistoryEntry.PartQuantity;
            }

            string priceUnit = string.Empty;
            if (!partHistoryEntry.IsPriceUnitNull())
            {
                priceUnit = partHistoryEntry.PriceUnit;
            }

            decimal weight = partHistoryEntry.IsWeightNull() ? 0M : partHistoryEntry.Weight;

            decimal totalPrice = OrderPrice.CalculatePrice(basePrice, priceUnit, fees, partQuantitity, weight);
            CreateCell(worksheet, updatedRowIndex, colIndex++, totalPrice, cellFormat:MONEY_FORMAT);
            CreateCell(worksheet, updatedRowIndex, colIndex++, priceUnit);

            if (partHistoryEntry.IsProcessingDaysNull())
            {
                CreateCell(worksheet, updatedRowIndex, colIndex++, notAvailable);
            }
            else
            {
                CreateCell(worksheet, updatedRowIndex, colIndex++, partHistoryEntry.ProcessingDays);
            }

            CreateCell(worksheet, updatedRowIndex, colIndex++, partHistoryEntry.ProcessCount);
            CreateCell(worksheet, updatedRowIndex, colIndex++, partHistoryEntry.ReworkCount > 0 ? "Y" : "N");

            updatedRowIndex++;
            return updatedRowIndex;
        }

        #endregion

        #region IPartReport Members

        /// <summary>
        /// Gets or sets the starting date of the report.
        /// </summary>
        public DateTime FromDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ending date of the report.
        /// </summary>
        public DateTime ToDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Customer ID to use for order searches.
        /// </summary>
        public int CustomerID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Part ID to use for order searches.
        /// </summary>
        public int PartID
        {
            get;
            set;
        }

        #endregion
    }

    public class BatchProductionReport : ExcelBaseReport
    {
        #region Properties

        public override string Title => "Batch Production";

        /// <summary>
        /// Gets or sets the starting date of the report.
        /// </summary>
        public DateTime FromDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ending date of the report.
        /// </summary>
        public DateTime ToDate
        {
            get;
            set;
        }

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            try
            {
                List<BatchProductionData> batchProduction;

                using (var dtBatchProduction = new Data.Reports.ProcessPartsReport.BatchProductionDataTable())
                {
                    using (var ta = new BatchProductionTableAdapter())
                    {
                        ta.Fill(dtBatchProduction, FromDate, ToDate);

                        batchProduction = dtBatchProduction.Select(BatchProductionData.From).ToList();
                    }
                }

                var worksheet = CreateWorksheet(Title);
                int rowIndex = AddHeaderRow(worksheet);

                // Calculate batch totals
                var batchTotals = batchProduction.GroupBy(o => o.BatchId).ToDictionary(g => g.Key,
                    g => g.Sum(o => o.EachPrice * o.PartQuantity));

                // Add row for each batch
                foreach (var batchProductionRow in batchProduction)
                {
                    var batchTotal = batchTotals[batchProductionRow.BatchId];
                    rowIndex = AddRow(worksheet, batchProductionRow, batchTotal, rowIndex);
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error running Batch Production report.");
            }
        }

        private int AddHeaderRow(Worksheet wks)
        {
            var rowIndex = AddCompanyHeaderRows(wks, 7, string.Empty) + 2;

            int colIndex = 0;
            CreateHeaderCell(wks, rowIndex, colIndex++, "Batch No.", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Customer", 30);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Order", 15);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Part", 30);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Part Quantity", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Each Price", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Total Batch Price", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Fixture", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Date Opened", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Date Closed", 20);

            rowIndex++;
            return rowIndex;
        }

        private int AddRow(Worksheet worksheet, BatchProductionData batchProductionData, decimal batchTotalPrice, int rowIndex)
        {
            int colIndex = 0;

            CreateCell(worksheet, rowIndex, colIndex++, batchProductionData.BatchId);
            CreateCell(worksheet, rowIndex, colIndex++, batchProductionData.CustomerName);
            CreateCell(worksheet, rowIndex, colIndex++, batchProductionData.OrderId);
            CreateCell(worksheet, rowIndex, colIndex++, batchProductionData.PartName);
            CreateCell(worksheet, rowIndex, colIndex++, batchProductionData.PartQuantity);
            CreateCell(worksheet, rowIndex, colIndex++, batchProductionData.EachPrice, cellFormat: MONEY_FORMAT);
            CreateCell(worksheet, rowIndex, colIndex++, batchTotalPrice, cellFormat: MONEY_FORMAT);
            CreateCell(worksheet, rowIndex, colIndex++, batchProductionData.Fixture);
            CreateCell(worksheet, rowIndex, colIndex++, batchProductionData.OpenDate.ToShortDateString());
            CreateCell(worksheet, rowIndex, colIndex++, batchProductionData.CloseDate?.ToShortDateString() ?? "NA");

            return rowIndex + 1;
        }

        #endregion

        #region BatchProductionData

        private class BatchProductionData
        {
            public int BatchId { get; set; }

            public int OrderId { get; set; }

            public decimal EachPrice { get; set; }

            public string Fixture { get; set; }

            public DateTime OpenDate { get; set; }

            public DateTime? CloseDate { get; set; }

            public string CustomerName { get; set; }

            public string PartName { get; set; }

            public int PartQuantity { get; set; }

            public static BatchProductionData From(Data.Reports.ProcessPartsReport.BatchProductionRow orderRow)
            {
                if (orderRow == null)
                {
                    return null;
                }

                return new BatchProductionData
                {
                    BatchId = orderRow.BatchID,
                    OrderId = orderRow.OrderID,

                    EachPrice = OrderPrice.CalculateEachPrice(
                        orderRow.IsBasePriceNull() ? 0M : orderRow.BasePrice,
                        orderRow.IsPriceUnitNull() ? OrderPrice.enumPriceUnit.Each.ToString() : orderRow.PriceUnit,
                        orderRow.IsOrderQuantityNull() ? 0 : orderRow.OrderQuantity,
                        orderRow.IsWeightNull() ? 0M : orderRow.Weight),

                    CustomerName = orderRow.CustomerName,
                    Fixture = orderRow.IsFixtureNull() ? string.Empty : orderRow.Fixture,
                    PartName = orderRow.PartName,
                    PartQuantity = orderRow.BatchOrderQuantity,
                    OpenDate = orderRow.OpenDate,
                    CloseDate = orderRow.IsCloseDateNull() ? (DateTime?)null : orderRow.CloseDate
                };
            }
        }

        #endregion
    }

    public class TimeTrackingDetailReport : ExcelBaseReport
    {
        #region Properties

        public DateTime FromDate
        {
            get;
            set;
        }

        public DateTime ToDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the customer ID for this report.
        /// </summary>
        /// <remarks>
        /// A null value represents 'show all customers.'
        /// </remarks>
        /// <value>The customer ID.</value>
        public int? CustomerID
        {
            get;
            set;
        }

        public override string Title
        {
            get
            {
                return "Time Tracking";
            }
        }

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            Data.Reports.ProcessPartsReport.TimeDataDetailDataTable dtTimeDataDetail = null;

            try
            {
                dtTimeDataDetail = new Data.Reports.ProcessPartsReport.TimeDataDetailDataTable();

                using (var taTimeDataDetail = new TimeDataDetailTableAdapter())
                {
                    taTimeDataDetail.Fill(dtTimeDataDetail,
                        CustomerID,
                        DateUtilities.StartOfDay(FromDate),
                        DateUtilities.EndOfDay(ToDate),
                        ApplicationSettings.Current.WorkStatusInProcess);
                }

                var worksheet = CreateWorksheet(this.Title);
                int rowIndex = AddHeader(worksheet);

                foreach (var row in dtTimeDataDetail)
                {
                    rowIndex = AddRow(worksheet, row, rowIndex);
                }

                if (dtTimeDataDetail.Count == 0)
                {
                    rowIndex++;
                }

                AddSummaryRow(worksheet, rowIndex);
            }
            finally
            {
                dtTimeDataDetail?.Dispose();
            }
        }

        private int AddHeader(Worksheet wks)
        {
            var rowIndex = AddMainHeader(wks);

            int colIndex = 0;
            CreateHeaderCell(wks, rowIndex, colIndex++, "WO", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Customer", 30);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Product Class", 25);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Completed Date", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "PO", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Part Quantity", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Revenue", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Labor Time (Hrs.)", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Processing Time (Hrs.)", 25);

            rowIndex++;
            return rowIndex;
        }

        private int AddMainHeader(Worksheet worksheet)
        {
            const int columnCount = 9;
            int rowIndex = 0;

            //Add Header Title
            WorksheetMergedCellsRegion region = CreateMergedCell(worksheet, rowIndex, 0, rowIndex, columnCount, Title);

            //Set the cell alignment of the merged region. Since a cell and its merged region shared a cell format, this will ultimately set the format of the merged region
            worksheet.Rows[rowIndex].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            worksheet.Rows[rowIndex].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            worksheet.Rows[rowIndex].Height = 18 * 20;
            region.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            region.CellFormat.Font.Name = DefaultFontName;
            region.CellFormat.Font.Height = 18 * 20;
            region.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(211, 211, 211)), null, FillPatternStyle.Solid);

            rowIndex++;

            CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 1, "From Date:  " + this.FromDate.ToShortDateString()).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateMergedCell(worksheet, rowIndex, 2,rowIndex, columnCount, "Company:    " + ApplicationSettings.Current.CompanyName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;

            CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 1, "To Date:  " + this.ToDate.ToShortDateString()).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateMergedCell(worksheet, rowIndex, 2, rowIndex, columnCount, "User:             " + SecurityManager.UserName).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;

            CreateMergedCell(worksheet, rowIndex, 0, rowIndex, 1, string.Empty);
            CreateMergedCell(worksheet, rowIndex, 2, rowIndex, columnCount, "Date:             " + DateTime.Now).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            rowIndex++;
            return rowIndex;
        }

        private int AddRow(Worksheet wks, Data.Reports.ProcessPartsReport.TimeDataDetailRow row, int rowIndex)
        {
            const double minutesPerHour = 60d;

            int colIndex = 0;

            CreateCell(wks, rowIndex, colIndex++, row.OrderID);
            CreateCell(wks, rowIndex, colIndex++, row.CustomerName);
            CreateCell(wks, rowIndex, colIndex++, row.IsProductClassNull() ? "N/A" : row.ProductClass);
            CreateCell(wks, rowIndex, colIndex++, row.IsCompletedDateNull() ? "NA" : row.CompletedDate.ToShortDateString());
            CreateCell(wks, rowIndex, colIndex++, row.IsPurchaseOrderNull() ? "NA" : row.PurchaseOrder);

            if (row.IsPartQuantityNull())
            {
                CreateCell(wks, rowIndex, colIndex++, "NA");
            }
            else
            {
                CreateCell(wks, rowIndex, colIndex++, row.PartQuantity);
            }

            if (row.IsRevenueNull())
            {
                CreateCell(wks, rowIndex, colIndex++, "NA");
            }
            else
            {
                CreateCell(wks, rowIndex, colIndex++, row.Revenue, cellFormat: MONEY_FORMAT);
            }

            if (row.IsLaborTimeNull())
            {
                CreateCell(wks, rowIndex, colIndex++, "NA");
            }
            else
            {
                CreateCell(wks, rowIndex, colIndex++, row.LaborTime / minutesPerHour, cellFormat: DURATION_FORMAT);
            }

            if (row.IsProcessingTimeNull())
            {
                CreateCell(wks, rowIndex, colIndex++, "NA");
            }

            else
            {
                CreateCell(wks, rowIndex, colIndex++, row.ProcessingTime / minutesPerHour, cellFormat : DURATION_FORMAT);
            }

            return rowIndex + 1;
        }

        private void AddSummaryRow(Worksheet wks, int rowIndex)
        {

            int newRowIndex = rowIndex;
            CreateMergedCell(wks, newRowIndex, 0, newRowIndex, 4, "Total:", true, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(wks, newRowIndex, 5, "=SUM(R6C6:R" + (rowIndex) + "C6" + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center, cellFormat:DURATION_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(wks, newRowIndex, 6, "=SUM(R6C7:R" + (rowIndex) + "C7" + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center, cellFormat:DURATION_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(wks, newRowIndex, 7, "=SUM(R6C8:R" + (rowIndex) + "C8" + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center, cellFormat:DURATION_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(wks, newRowIndex, 8, "=SUM(R6C9:R" + (rowIndex) + "C9" + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center, cellFormat:DURATION_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            newRowIndex++;

            CreateMergedCell(wks, newRowIndex, 0, newRowIndex, 4, "Average:", true, HorizontalCellAlignment.Right).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(wks, newRowIndex, 5, "=AVERAGE(R6C6:R" + (rowIndex) + "C6" + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center, cellFormat:DURATION_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(wks, newRowIndex, 6, "=AVERAGE(R6C7:R" + (rowIndex) + "C7" + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center, cellFormat:DURATION_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(wks, newRowIndex, 7, "=AVERAGE(R6C8:R" + (rowIndex) + "C8" + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center, cellFormat:DURATION_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateFormulaCell(wks, newRowIndex, 8, "=AVERAGE(R6C9:R" + (rowIndex) + "C9" + ")", CellReferenceMode.R1C1, true, HorizontalCellAlignment.Center, cellFormat:DURATION_FORMAT).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
        }

        #endregion
    }

    public class DeliveryPerformanceReport : ExcelBaseReport
    {
        #region Properties

        public override string Title => string.IsNullOrEmpty(ProductClass)
            ? "Delivery Performance Report"
            : $"Delivery Performance Report - {ProductClass.Replace("\n", string.Empty).Replace("\r", string.Empty)}";

        public string ProductClass { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        private readonly Lazy<ApplicationSettingsDataSet.FieldsRow> _serialNumberFieldLazy = new Lazy<ApplicationSettingsDataSet.FieldsRow>(
            () =>
            {
                ApplicationSettingsDataSet.FieldsDataTable fields;

                using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                {
                    fields = ta.GetByCategory("Order");
                }

                var requiredDateField = fields.FirstOrDefault(f => f.Name == "Serial Number");
                return requiredDateField;

            });

        #endregion

        #region Methods

        public DeliveryPerformanceReport(DateTime fromDate, DateTime toDate, string productClass)
        {
            FromDate = fromDate.Date;
            ToDate = toDate.Date;
            ProductClass = productClass;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            var data = DeliveryPerformanceData.GetReportData(FromDate, ToDate);
            var productClassItems = data.Orders.Where(o => o.ProductClass == ProductClass).ToList();
            CreateProductClassWorksheet(productClassItems);
        }

        private void CreateProductClassWorksheet(List<DeliveryPerformanceData.Order> productClassGroup)
        {
            var serialNumberField = _serialNumberFieldLazy.Value;
            var includeSerialNumbers = serialNumberField == null || serialNumberField.IsVisible || serialNumberField.IsRequired;

            var worksheet = CreateWorksheet(ProductClass);

            // Header
            var colCount = includeSerialNumbers ? 6 : 5;
            var rowIndex = AddCompanyHeaderRows(worksheet, colCount, string.Empty);
            rowIndex += 2;

            CreateCell(worksheet, rowIndex, 0, "From Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, FromDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex++;

            CreateCell(worksheet, rowIndex, 0, "To Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, ToDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex += 2;

            // Table header
            CreateHeaderCell(worksheet, rowIndex, 0, "WO", 20);
            CreateHeaderCell(worksheet, rowIndex, 1, "Customer", 20);
            CreateHeaderCell(worksheet, rowIndex, 2, "Part Number", 30);
            CreateHeaderCell(worksheet, rowIndex, 3, "Part Quantity", 20);
            CreateHeaderCell(worksheet, rowIndex, 4, "Estimated Ship Date", 25);
            CreateHeaderCell(worksheet, rowIndex, 5, "Actual Ship Date", 25);
            CreateHeaderCell(worksheet, rowIndex, 6, "Difference (Days)", 20);

            if (includeSerialNumbers)
            {
                CreateHeaderCell(worksheet, rowIndex, 7, "Serial Numbers", 25);
            }

            rowIndex++;

            foreach (var order in productClassGroup.OrderBy(s => s.OrderId))
            {
                CreateCell(worksheet, rowIndex, 0, order.OrderId);
                CreateCell(worksheet, rowIndex, 1, order.Customer);
                CreateCell(worksheet, rowIndex, 2, order.PartNumber);
                CreateCell(worksheet, rowIndex, 3, order.PartQuantity);

                CreateCell(worksheet, rowIndex, 4, order.EstimatedShipDate, cellFormat:DATE_FORMAT);
                CreateCell(worksheet, rowIndex, 5, order.ActualShipDate, cellFormat:DATE_FORMAT);
                CreateFormulaCell(worksheet, rowIndex, 6, $"=R{rowIndex + 1}C5 - R{rowIndex + 1}C6", CellReferenceMode.R1C1, cellFormat:"+0;-0;0");

                if (includeSerialNumbers)
                {
                    var serialNumbersString = order.SerialNumbers == null
                        ? string.Empty
                        : string.Join("\n", order.SerialNumbers);

                    CreateCell(worksheet, rowIndex, 7, serialNumbersString);
                }

                rowIndex++;
            }
        }

        #endregion
    }

    public class DeliveryPerformanceSummaryReport : ExcelBaseReport
    {
        #region Properties

        public override string Title => "Delivery Performance Summary Report";

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        #endregion

        #region Methods

        public DeliveryPerformanceSummaryReport(DateTime fromDate, DateTime toDate)
        {
            FromDate = fromDate.Date;
            ToDate = toDate.Date;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            var data = DeliveryPerformanceData.GetReportData(FromDate, ToDate);
            CreateSummaryWorksheet(data);
            CreatePartsSummaryWorksheet(data);
        }

        private void CreateSummaryWorksheet(DeliveryPerformanceData data)
        {
            var worksheet = CreateWorksheet("Summary");

            // Header
            var rowIndex = AddCompanyHeaderRows(worksheet, 6, string.Empty);
            rowIndex += 2;

            CreateCell(worksheet, rowIndex, 0, "From Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, FromDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex++;

            CreateCell(worksheet, rowIndex, 0, "To Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, ToDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex += 2;

            // Table header
            CreateHeaderCell(worksheet, rowIndex, 0, "Product Class", 20);
            CreateHeaderCell(worksheet, rowIndex, 1, "Total Late", 20);
            CreateHeaderCell(worksheet, rowIndex, 2, "Average Days Late", 25);
            CreateHeaderCell(worksheet, rowIndex, 3, "Total Early", 20);
            CreateHeaderCell(worksheet, rowIndex, 4, "Average Days Early", 25);
            CreateHeaderCell(worksheet, rowIndex, 5, "Total Orders", 20);
            CreateHeaderCell(worksheet, rowIndex, 6, "% On Time", 20);

            rowIndex++;

            // Table contents
            var startRowIndex = rowIndex;

            foreach (var summary in data.OrderProductClasses.OrderBy(s => s.ProductClass))
            {
                CreateCell(worksheet, rowIndex, 0, summary.ProductClass);
                CreateCell(worksheet, rowIndex, 1, summary.TotalLate);
                CreateCell(worksheet, rowIndex, 2, summary.AverageDaysLate);
                CreateCell(worksheet, rowIndex, 3, summary.TotalEarly);
                CreateCell(worksheet, rowIndex, 4, summary.AverageDaysEarly);
                CreateCell(worksheet, rowIndex, 5, summary.Total);
                CreateFormulaCell(worksheet, rowIndex, 6, $"=(R{rowIndex + 1}C6 - R{rowIndex + 1}C2)/R{rowIndex + 1}C6", CellReferenceMode.R1C1, cellFormat:PERCENT_FORMAT);
                rowIndex++;
            }

            // Totals / Averages
            if (data.OrderProductClasses.Count > 0)
            {
                var endRowIndex = rowIndex - 1;
                CreateCell(worksheet, rowIndex, 0, "Total:");
                CreateFormulaCell(worksheet, rowIndex, 1, $"=SUM(R{startRowIndex + 1}C2:R{endRowIndex + 1}C2)", CellReferenceMode.R1C1);
                CreateFormulaCell(worksheet, rowIndex, 2, $"=AVERAGE(R{startRowIndex + 1}C3:R{endRowIndex + 1}C3)", CellReferenceMode.R1C1);
                CreateFormulaCell(worksheet, rowIndex, 3, $"=SUM(R{startRowIndex + 1}C4:R{endRowIndex + 1}C4)", CellReferenceMode.R1C1);
                CreateFormulaCell(worksheet, rowIndex, 4, $"=AVERAGE(R{startRowIndex + 1}C5:R{endRowIndex + 1}C5)", CellReferenceMode.R1C1);
                CreateFormulaCell(worksheet, rowIndex, 5, $"=SUM(R{startRowIndex + 1}C6:R{endRowIndex + 1}C6)", CellReferenceMode.R1C1);
                CreateFormulaCell(worksheet, rowIndex, 6, $"=(R{rowIndex + 1}C6 - R{rowIndex + 1}C2)/R{rowIndex + 1}C6", CellReferenceMode.R1C1, cellFormat:PERCENT_FORMAT);
            }
        }

        private void CreatePartsSummaryWorksheet(DeliveryPerformanceData data)
        {
            var worksheet = CreateWorksheet("Parts Summary");

            // Header
            var rowIndex = AddCompanyHeaderRows(worksheet, 6, string.Empty);
            rowIndex += 2;

            CreateCell(worksheet, rowIndex, 0, "From Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, FromDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex++;

            CreateCell(worksheet, rowIndex, 0, "To Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, ToDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex += 2;

            // Table header
            CreateHeaderCell(worksheet, rowIndex, 0, "Product Class", 20);
            CreateHeaderCell(worksheet, rowIndex, 1, "Total Late", 20);
            CreateHeaderCell(worksheet, rowIndex, 2, "Average Days Late", 25);
            CreateHeaderCell(worksheet, rowIndex, 3, "Total Early", 20);
            CreateHeaderCell(worksheet, rowIndex, 4, "Average Days Early", 25);
            CreateHeaderCell(worksheet, rowIndex, 5, "Total Parts", 20);
            CreateHeaderCell(worksheet, rowIndex, 6, "% On Time", 20);

            rowIndex++;

            // Table contents
            var startRowIndex = rowIndex;

            foreach(var summary in data.PartProductClasses.OrderBy(s => s.ProductClass))
            {
                CreateCell(worksheet, rowIndex, 0, summary.ProductClass);
                CreateCell(worksheet, rowIndex, 1, summary.TotalLate);
                CreateCell(worksheet, rowIndex, 2, summary.AverageDaysLate);
                CreateCell(worksheet, rowIndex, 3, summary.TotalEarly);
                CreateCell(worksheet, rowIndex, 4, summary.AverageDaysEarly);
                CreateCell(worksheet, rowIndex, 5, summary.Total);
                CreateFormulaCell(worksheet, rowIndex, 6, $"=(R{rowIndex + 1}C6 - R{rowIndex + 1}C2)/R{rowIndex + 1}C6", CellReferenceMode.R1C1, cellFormat: PERCENT_FORMAT);
                rowIndex++;
            }

            // Totals / Averages
            if(data.PartProductClasses.Count > 0)
            {
                var endRowIndex = rowIndex - 1;
                CreateCell(worksheet, rowIndex, 0, "Total:");
                CreateFormulaCell(worksheet, rowIndex, 1, $"=SUM(R{startRowIndex + 1}C2:R{endRowIndex + 1}C2)", CellReferenceMode.R1C1);
                CreateFormulaCell(worksheet, rowIndex, 2, $"=AVERAGE(R{startRowIndex + 1}C3:R{endRowIndex + 1}C3)", CellReferenceMode.R1C1);
                CreateFormulaCell(worksheet, rowIndex, 3, $"=SUM(R{startRowIndex + 1}C4:R{endRowIndex + 1}C4)", CellReferenceMode.R1C1);
                CreateFormulaCell(worksheet, rowIndex, 4, $"=AVERAGE(R{startRowIndex + 1}C5:R{endRowIndex + 1}C5)", CellReferenceMode.R1C1);
                CreateFormulaCell(worksheet, rowIndex, 5, $"=SUM(R{startRowIndex + 1}C6:R{endRowIndex + 1}C6)", CellReferenceMode.R1C1);
                CreateFormulaCell(worksheet, rowIndex, 6, $"=(R{rowIndex + 1}C6 - R{rowIndex + 1}C2)/R{rowIndex + 1}C6", CellReferenceMode.R1C1, cellFormat: PERCENT_FORMAT);
            }
        }

        #endregion
    }

    public class ProcessAnswerReport : ExcelBaseReport
    {
        #region Properties

        public override string Title
        {
            get
            {
                //return Status.ToString() + " Process Answer";
                return "Process Answers";
            }
        }

        public List<int> ProcessIds { get; set; }

        public List<int> CustomerIds { get; set; }

        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Portrait; }
        }

        #endregion

        #region Methods

        public ProcessAnswerReport()
        {}

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            CreateReportExcel();
        }

        private void CreateReportExcel()
        {
            try
            {
                var reportData = ProcessAnswerData.From(ProcessIds, CustomerIds, FromDate, ToDate);
                var reportColumns = reportData.ColumnGroups
                    .SelectMany(g => g.AllColumns)
                    .OrderBy(c => c.ColumnId)
                    .ToList();

                var wks = CreateWorksheet("Process Answers");
                var lastColumnIndex = 10 + reportColumns.Count;

                // Worksheet Header
                var reportSubtitle = reportData.ProcessNames.Count > 1
                    ? "Multiple Processes"
                    : reportData.ProcessNames.FirstOrDefault() ?? "No Process";
                var rowIndex = AddCompanyHeaderRows(wks, lastColumnIndex, "- " + reportSubtitle);

                if (reportData.ProcessNames.Count > 1)
                {
                    Bold(CreateCell(wks, rowIndex, 0, "Processes:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None));
                    CreateCell(wks, rowIndex, 1, string.Join(", ", reportData.ProcessNames), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
                }

                rowIndex += 2;

                Bold(CreateCell(wks, rowIndex, 0, "From Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None));
                CreateCell(wks, rowIndex, 1, FromDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
                rowIndex++;

                Bold(CreateCell(wks, rowIndex, 0, "To Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None));
                CreateCell(wks, rowIndex, 1, ToDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);

                rowIndex += 2;

                // Table Header
                var startRowIndex = rowIndex;
                CreateHeaderCell(wks, rowIndex, 0, "Customer").CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Black);
                CreateHeaderCell(wks, rowIndex, 1, "WO").CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Black);
                CreateHeaderCell(wks, rowIndex, 2, "PO").CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Black);
                CreateHeaderCell(wks, rowIndex, 3, "Status").CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Black);
                CreateHeaderCell(wks, rowIndex, 4, "Part").CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Black);
                CreateHeaderCell(wks, rowIndex, 5, "Process").CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Black);
                CreateHeaderCell(wks, rowIndex, 6, "Process Rev").CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Black);
                CreateHeaderCell(wks, rowIndex, 7, "Process Alias").CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Black);
                CreateHeaderCell(wks, rowIndex, 8, "Order Date Completed").CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Black);
                CreateHeaderCell(wks, rowIndex, 9, "Location").CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Black);
                CreateHeaderCell(wks, rowIndex, 10, "S/N").CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Black);

                var headerColIndex = 11;

                foreach (var column in reportColumns)
                {
                    var headerCell = CreateHeaderCell(wks, rowIndex, headerColIndex, column.Name);
                    headerCell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Black);
                    headerColIndex++;
                }

                foreach (var orderData in reportData.Orders)
                {
                    // Create a row for each list of answers in an order
                    foreach (var orderDataAnswers in orderData.AnswerColumns)
                    {
                        rowIndex++;

                        CreateCell(wks, rowIndex, 0, orderData.CustomerName);
                        CreateCell(wks, rowIndex, 1, orderData.OrderId);
                        CreateCell(wks, rowIndex, 2, orderData.PurchaseOrder);
                        CreateCell(wks, rowIndex, 3, orderData.Status);
                        CreateCell(wks, rowIndex, 4, orderData.PartName);
                        CreateCell(wks, rowIndex, 5, orderData.ProcessName);
                        CreateCell(wks, rowIndex, 6, orderData.ProcessRev);
                        CreateCell(wks, rowIndex, 7, orderData.ProcessAliasName);
                        CreateCell(wks, rowIndex, 8, orderData.CompletedDate?.ToShortDateString() ?? "NA");
                        CreateCell(wks, rowIndex, 9, orderData.CurrentLocation);

                        // For serial numbers and answers, add a blank cell to draw a border
                        CreateCell(wks, rowIndex, 10, orderData.SerialNumbers.Count > 0 ? string.Join(",", orderData.SerialNumbers) : " ");

                        var colIndex = 11;

                        foreach (var column in reportColumns)
                        {
                            var reportCellString = orderDataAnswers.TryGetValue(column.ColumnId, out string value) ? value : " ";

                            object reportCellValue = reportCellString;
                            if (decimal.TryParse(reportCellString, out decimal reportCellNumber))
                            {
                                // Store the value as a number instead of a string
                                reportCellValue = reportCellNumber;
                            }

                            CreateCell(wks, rowIndex, colIndex, reportCellValue);

                            colIndex++;
                        }
                    }
                }

                // Set column widths
                wks.Columns[0].SetWidth(26, WorksheetColumnWidthUnit.Character);
                wks.Columns[1].SetWidth(10, WorksheetColumnWidthUnit.Character);
                wks.Columns[2].SetWidth(15, WorksheetColumnWidthUnit.Character);
                wks.Columns[3].SetWidth(15, WorksheetColumnWidthUnit.Character);
                wks.Columns[4].SetWidth(26, WorksheetColumnWidthUnit.Character);
                wks.Columns[5].SetWidth(26, WorksheetColumnWidthUnit.Character);
                wks.Columns[6].SetWidth(18, WorksheetColumnWidthUnit.Character);
                wks.Columns[7].SetWidth(20, WorksheetColumnWidthUnit.Character);
                wks.Columns[8].SetWidth(30, WorksheetColumnWidthUnit.Character);
                wks.Columns[9].SetWidth(20, WorksheetColumnWidthUnit.Character);
                wks.Columns[10].SetWidth(30, WorksheetColumnWidthUnit.Character);

                for (var colIndex = 11; colIndex <= lastColumnIndex; ++colIndex)
                {
                    wks.Columns[colIndex].SetWidth(20, WorksheetColumnWidthUnit.Character);
                }

                // Add table for content
                wks.Tables.Add("A{0}:{1}{2}".FormatWith(startRowIndex + 1, this.ExcelColumnIndexToName(lastColumnIndex), rowIndex + 1), true);
            }
            catch (Exception exc)
            {
                string errorMsg = "Error running ProcessAnswerReport: " +
                    $"ProcessIDs are {string.Join("\n", ProcessIds)}";
                _log.Error(exc, errorMsg);
            }
        }

        #endregion
    }

    public class EmployeeReceivingReport : ExcelBaseReport
    {
        #region Properties

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string Title => "Employee Receiving Performance";

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            FromDate = FromDate.StartOfDay();
            ToDate = ToDate.EndOfDay();

            var data = EmployeeReceivingData.GetReportData(FromDate, ToDate);
            CreateSummaryWorksheet(data);
            CreateDetailWorksheet(data);
        }

        private void CreateSummaryWorksheet(EmployeeReceivingData data)
        {
            var worksheet = CreateWorksheet("Summary");

            // Header
            var rowIndex = AddCompanyHeaderRows(worksheet, 6, string.Empty);
            rowIndex += 2;

            CreateCell(worksheet, rowIndex, 0, "From Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, FromDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex++;

            CreateCell(worksheet, rowIndex, 0, "To Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, ToDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex += 2;

            // Table header
            CreateHeaderCell(worksheet, rowIndex, 0, "User", 20);
            CreateHeaderCell(worksheet, rowIndex, 1, "Orders Created", 20);
            WrapText(CreateHeaderCell(worksheet, rowIndex, 2, "Part Quantity from Orders Created", 25));
            CreateHeaderCell(worksheet, rowIndex, 3, "Orders Reviewed", 20);
            WrapText(CreateHeaderCell(worksheet, rowIndex, 4, "Part Quantity from Orders Reviewed", 25));

            rowIndex++;

            // Table contents
            foreach (var user in data.Users.OrderBy(u => u.Name))
            {
                var ordersCreated = user.OrderActions.Count(o => o.ActionType == EmployeeReceivingData.OrderActionType.Created);
                var ordersReviewed = user.OrderActions.Count(o => o.ActionType == EmployeeReceivingData.OrderActionType.Reviewed);

                var partsInOrdersCreated = 0L;
                try
                {
                    partsInOrdersCreated += user.OrderActions
                        .Where(o => o.ActionType == EmployeeReceivingData.OrderActionType.Created)
                        .Sum(o => o.PartQuantity);
                }
                catch (OverflowException)
                {
                    _log.Warn("Arithmetic overflow occurred while running report");
                    partsInOrdersCreated = long.MaxValue;
                }

                var partsInOrdersReviewed = 0L;
                try
                {
                    partsInOrdersReviewed += user.OrderActions
                        .Where(o => o.ActionType == EmployeeReceivingData.OrderActionType.Reviewed)
                        .Sum(o => o.PartQuantity);
                }
                catch (OverflowException)
                {
                    _log.Warn("Arithmetic overflow occurred while running report");
                    partsInOrdersReviewed = long.MaxValue;
                }

                CreateCell(worksheet, rowIndex, 0, user.Name);
                CreateCell(worksheet, rowIndex, 1, ordersCreated);
                CreateCell(worksheet, rowIndex, 2, partsInOrdersCreated);
                CreateCell(worksheet, rowIndex, 3, ordersReviewed);
                CreateCell(worksheet, rowIndex, 4, partsInOrdersReviewed);
                rowIndex++;
            }
        }

        private void CreateDetailWorksheet(EmployeeReceivingData data)
        {
            var worksheet = CreateWorksheet("Details");
            var rowIndex = -1;

            foreach (var user in data.Users.OrderBy(u => u.Name))
            {
                rowIndex++;

                // User Header
                var region = CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 5, user.Name);
                region.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(Color.FromArgb(170, 170, 170)), null, FillPatternStyle.Solid);
                region.CellFormat.Alignment = HorizontalCellAlignment.Left;
                rowIndex++;

                // Header
                CreateHeaderCell(worksheet, rowIndex, 0, "WO", 15);
                CreateHeaderCell(worksheet, rowIndex, 1, "Customer", 25);
                CreateHeaderCell(worksheet, rowIndex, 2, "Part", 30);
                CreateHeaderCell(worksheet, rowIndex, 3, "Part Quantity", 15); // Might be too small
                CreateHeaderCell(worksheet, rowIndex, 4, "Action", 20);
                CreateHeaderCell(worksheet, rowIndex, 5, "Date and Time", 25);

                rowIndex++;

                // Table contents
                foreach (var order in user.OrderActions.OrderBy(o => o.OrderId).ThenBy(o => o.ActionType).ThenBy(o => o.ActionDate))
                {
                    CreateCell(worksheet, rowIndex, 0, order.OrderId);
                    CreateCell(worksheet, rowIndex, 1, order.CustomerName);
                    CreateCell(worksheet, rowIndex, 2, order.PartName);
                    CreateCell(worksheet, rowIndex, 3, order.PartQuantity);
                    CreateCell(worksheet, rowIndex, 4, order.ActionType);
                    CreateCell(worksheet, rowIndex, 5, order.ActionDate, cellFormat: $"{DATE_FORMAT} {TIME_FORMAT}");
                    rowIndex++;
                }
            }
        }

        #endregion
    }

    public class EmployeeProcessingReport : ExcelBaseReport
    {
        #region Properties

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string Title => "Employee Processing Performance";

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken)
        {
            using (new UsingTimeMe(Title))
            {
                FromDate = FromDate.StartOfDay();
                ToDate = ToDate.EndOfDay();

                var reportData = EmployeeProcessingData.GetReportDataAsync(FromDate, ToDate,
                    ApplicationSettings.Current,
                    cancellationToken).Result;

                if (cancellationToken.IsCancellationRequested)
                {
                    _log.Info("Canceled report.");
                }
                else
                {
                    if (reportData == null)
                    {
                        throw new ApplicationException("Could not retrieve employee data.");
                    }
                    else
                    {
                        CreateUserSummaryWorksheet(reportData);
                        CreateDepartmentSummaryWorksheet(reportData);
                        CreateUserDetailWorksheet(reportData);
                        CreateDepartmentDetailWorksheet(reportData);
                    }
                }
            }
        }

        private void CreateUserSummaryWorksheet(EmployeeProcessingData reportData)
        {
            var worksheet = CreateWorksheet("Employees");

            // Header
            var rowIndex = AddCompanyHeaderRows(worksheet, 6, string.Empty);
            rowIndex += 2;

            CreateCell(worksheet, rowIndex, 0, "From Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, FromDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex++;

            CreateCell(worksheet, rowIndex, 0, "To Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, ToDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex += 2;

            // Table header
            CreateHeaderCell(worksheet, rowIndex, 0, "User", 25);
            WrapText(CreateHeaderCell(worksheet, rowIndex, 1, "Orders Processed", 20));
            WrapText(CreateHeaderCell(worksheet, rowIndex, 2, "Orders Inspected", 20));
            WrapText(CreateHeaderCell(worksheet, rowIndex, 3, "Orders Through Final Inspection", 20));
            WrapText(CreateHeaderCell(worksheet, rowIndex, 4, "Orders Shipped", 20));

            rowIndex++;

            // Table contents
            foreach (var user in reportData.Users.OrderBy(u => u.Name))
            {
                var processesCompleted = user.OrdersProcessed;
                var partInspectionsCompleted = user.OrdersInspected;
                var finalInspectionsCompleted = user.OrdersFinalInspected;
                var ordersShipped = user.OrdersShipped;

                CreateCell(worksheet, rowIndex, 0, FormattedName(user));

                if (processesCompleted > 0)
                {
                    CreateCell(worksheet, rowIndex, 1, processesCompleted);
                }
                else
                {
                    CreateCell(worksheet, rowIndex, 1, string.Empty);
                }

                if (partInspectionsCompleted > 0)
                {
                    CreateCell(worksheet, rowIndex, 2, partInspectionsCompleted);
                }
                else
                {
                    CreateCell(worksheet, rowIndex, 2, string.Empty);
                }

                if (finalInspectionsCompleted > 0)
                {
                    CreateCell(worksheet, rowIndex, 3, finalInspectionsCompleted);
                }
                else
                {
                    CreateCell(worksheet, rowIndex, 3, string.Empty);
                }

                if (ordersShipped > 0)
                {
                    CreateCell(worksheet, rowIndex, 4, ordersShipped);
                }
                else
                {
                    CreateCell(worksheet, rowIndex, 4, string.Empty);
                }

                rowIndex++;

            }
        }

        private void CreateDepartmentSummaryWorksheet(EmployeeProcessingData reportData)
        {
            var worksheet = CreateWorksheet("Departments");

            // Header
            var rowIndex = AddCompanyHeaderRows(worksheet, 6, string.Empty);
            rowIndex += 2;

            CreateCell(worksheet, rowIndex, 0, "From Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, FromDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex++;

            CreateCell(worksheet, rowIndex, 0, "To Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, ToDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex += 2;

            // Table header
            CreateHeaderCell(worksheet, rowIndex, 0, "Department", 20);
            WrapText(CreateHeaderCell(worksheet, rowIndex, 1, "Processes Completed", 20));

            rowIndex++;

            // Table contents
            foreach (var department in reportData.Departments.OrderBy(d => d.Name))
            {
                CreateCell(worksheet, rowIndex, 0, department.Name);
                CreateCell(worksheet, rowIndex, 1, department.ProcessesCompleted);
                rowIndex++;
            }
        }

        private void CreateUserDetailWorksheet(EmployeeProcessingData reportData)
        {
            var worksheet = CreateWorksheet("Details (Employee)");
            var rowIndex = 0;

            var nameColumn = 0;
            var orderColumn = 1;
            var deptColumn = 2;
            var processColumn = 3;
            var productClassColumn = 4;
            var dateColumn = reportData.ShowProductClass ? 5 : 4;
            var revenueColumn = dateColumn + 1;
            var lastColumn = reportData.ShowRevenue ? revenueColumn : dateColumn;

            // Header
            CreateHeaderCell(worksheet, rowIndex, nameColumn, "Name", 25);
            CreateHeaderCell(worksheet, rowIndex, orderColumn, "WO #", 20);
            CreateHeaderCell(worksheet, rowIndex, deptColumn, "Department", 35);
            CreateHeaderCell(worksheet, rowIndex, processColumn, "Process", 35);

            if (reportData.ShowProductClass)
            {
                CreateHeaderCell(worksheet, rowIndex, productClassColumn, "Product Class", 30);
            }

            CreateHeaderCell(worksheet, rowIndex, dateColumn, "Date and Time", 25);

            if (reportData.ShowRevenue)
            {
                CreateHeaderCell(worksheet, rowIndex, revenueColumn, "Revenue", 20);
            }

            rowIndex++;

            // Table contents
            foreach (var user in reportData.Users.OrderBy(u => u.Name))
            {
                var userName = FormattedName(user);
                var totalUserRevenue = 0M;
                foreach (var order in user.OrderActions.OrderBy(o => o.OrderId).ThenBy(o => o.ActionDate))
                {
                    CreateCell(worksheet, rowIndex, nameColumn, userName);
                    CreateCell(worksheet, rowIndex, orderColumn, order.OrderId);
                    CreateCell(worksheet, rowIndex, deptColumn, order.Department);
                    CreateCell(worksheet, rowIndex, processColumn, GetProcessText(order));

                    if (reportData.ShowProductClass)
                    {
                        CreateCell(worksheet, rowIndex, productClassColumn, order.ProductClass);
                    }

                    CreateCell(worksheet, rowIndex, dateColumn, order.ActionDate, cellFormat: $"{DATE_FORMAT} {TIME_FORMAT}");

                    if (reportData.ShowRevenue)
                    {
                        CreateCell(worksheet, rowIndex, revenueColumn, order.ProcessRevenue, cellFormat: MONEY_FORMAT);
                        totalUserRevenue += order.ProcessRevenue ?? 0M;
                    }

                    rowIndex++;

                    userName = string.Empty; // Only show user name on first line
                }

                // Counts
                var processesCompleted = user.OrdersProcessed;
                var partInspectionsCompleted = user.OrdersInspected;
                var finalInspectionsCompleted = user.OrdersFinalInspected;
                var ordersShipped = user.OrdersShipped;

                if (processesCompleted > 0)
                {
                    Bold(CreateMergedCell(worksheet, rowIndex, 0, rowIndex, lastColumn - 1, "Orders Processed:",
                        false, HorizontalCellAlignment.Right));

                    CreateCell(worksheet, rowIndex, lastColumn, processesCompleted);
                    rowIndex++;
                }

                if (partInspectionsCompleted > 0)
                {
                    Bold(CreateMergedCell(worksheet, rowIndex, 0, rowIndex, lastColumn - 1, "Orders Inspected:",
                        false, HorizontalCellAlignment.Right));

                    CreateCell(worksheet, rowIndex, lastColumn, partInspectionsCompleted);
                    rowIndex++;
                }

                if (finalInspectionsCompleted > 0)
                {
                    Bold(CreateMergedCell(worksheet, rowIndex, 0, rowIndex, lastColumn - 1, "Orders Through Final Inspection:",
                        false, HorizontalCellAlignment.Right));

                    CreateCell(worksheet, rowIndex, lastColumn, finalInspectionsCompleted);
                    rowIndex++;
                }

                if (ordersShipped > 0)
                {
                    Bold(CreateMergedCell(worksheet, rowIndex, 0, rowIndex, lastColumn - 1, "Orders Shipped:",
                        false, HorizontalCellAlignment.Right));

                    CreateCell(worksheet, rowIndex, lastColumn, ordersShipped);
                    rowIndex++;
                }

                if (reportData.ShowRevenue)
                {
                    Bold(CreateMergedCell(worksheet, rowIndex, 0, rowIndex, lastColumn - 1, "Revenue for User:",
                        false, HorizontalCellAlignment.Right));

                    CreateCell(worksheet, rowIndex, lastColumn, totalUserRevenue, cellFormat: MONEY_FORMAT);

                    rowIndex++;
                }

                // Leave blank row between users
                rowIndex++;
            }
        }

        private void CreateDepartmentDetailWorksheet(EmployeeProcessingData reportData)
        {
            var worksheet = CreateWorksheet("Details (Department)");
            var rowIndex = 0;

            var nameColumn = 0;
            var orderColumn = 1;
            var processColumn = 2;
            var productClassColumn = 3;
            var dateColumn = reportData.ShowProductClass ? 4 : 3;
            var revenueColumn = dateColumn + 1;
            var lastColumn = reportData.ShowRevenue ? revenueColumn : dateColumn;

            // Header
            CreateHeaderCell(worksheet, rowIndex, nameColumn, "Department", 25);
            CreateHeaderCell(worksheet, rowIndex, orderColumn, "WO #", 20);
            CreateHeaderCell(worksheet, rowIndex, processColumn, "Process", 35);

            if (reportData.ShowProductClass)
            {
                CreateHeaderCell(worksheet, rowIndex, productClassColumn, "Product Class", 30);
            }

            CreateHeaderCell(worksheet, rowIndex, dateColumn, "Date and Time", 25);

            if (reportData.ShowRevenue)
            {
                CreateHeaderCell(worksheet, rowIndex, revenueColumn, "Revenue", 20);
            }

            rowIndex++;

            // Table contents
            foreach (var department in reportData.Departments.OrderBy(u => u.Name))
            {
                var departmentName = department.Name;
                var totalDepartmentRevenue = 0M;

                foreach (var order in department.OrderActions.OrderBy(o => o.OrderId).ThenBy(o => o.ActionDate))
                {
                    CreateCell(worksheet, rowIndex, nameColumn, departmentName);
                    CreateCell(worksheet, rowIndex, orderColumn, order.OrderId);
                    CreateCell(worksheet, rowIndex, processColumn, GetProcessText(order));

                    if (reportData.ShowProductClass)
                    {
                        CreateCell(worksheet, rowIndex, productClassColumn, order.ProductClass);
                    }

                    CreateCell(worksheet, rowIndex, dateColumn, order.ActionDate, cellFormat: $"{DATE_FORMAT} {TIME_FORMAT}");

                    if (reportData.ShowRevenue)
                    {
                        CreateCell(worksheet, rowIndex, revenueColumn, order.ProcessRevenue, cellFormat: MONEY_FORMAT);
                        totalDepartmentRevenue += order.ProcessRevenue ?? 0M;
                    }

                    rowIndex++;

                    departmentName = string.Empty; // Only show department name on first line
                }

                // Counts
                var ordersProcessed = department.OrdersProcessed;
                var processesCompleted = department.ProcessesCompleted;
                var partInspectionsCompleted = department.OrdersInspected;
                var finalInspectionsCompleted = department.OrdersFinalInspected;
                var ordersShipped = department.OrdersShipped;

                if (processesCompleted > 0)
                {
                    Bold(CreateMergedCell(worksheet, rowIndex, 0, rowIndex, lastColumn - 1, "Processes Completed:",
                        false, HorizontalCellAlignment.Right));

                    CreateCell(worksheet, rowIndex, lastColumn, processesCompleted);
                    rowIndex++;
                }

                if (ordersProcessed > 0)
                {
                    Bold(CreateMergedCell(worksheet, rowIndex, 0, rowIndex, lastColumn - 1, "Orders Processed:",
                        false, HorizontalCellAlignment.Right));

                    CreateCell(worksheet, rowIndex, lastColumn, ordersProcessed);
                    rowIndex++;
                }

                if (partInspectionsCompleted > 0)
                {
                    Bold(CreateMergedCell(worksheet, rowIndex, 0, rowIndex, lastColumn - 1, "Orders Inspected:",
                        false, HorizontalCellAlignment.Right));

                    CreateCell(worksheet, rowIndex, lastColumn, partInspectionsCompleted);
                    rowIndex++;
                }

                if (finalInspectionsCompleted > 0)
                {
                    Bold(CreateMergedCell(worksheet, rowIndex, 0, rowIndex, lastColumn - 1, "Orders Through Final Inspection:",
                        false, HorizontalCellAlignment.Right));

                    CreateCell(worksheet, rowIndex, lastColumn, finalInspectionsCompleted);
                    rowIndex++;
                }

                if (ordersShipped > 0)
                {
                    Bold(CreateMergedCell(worksheet, rowIndex, 0, rowIndex, lastColumn - 1, "Orders Shipped:",
                        false, HorizontalCellAlignment.Right));

                    CreateCell(worksheet, rowIndex, lastColumn, ordersShipped);
                    rowIndex++;
                }

                if (reportData.ShowRevenue && totalDepartmentRevenue > 0M)
                {
                    Bold(CreateMergedCell(worksheet, rowIndex, 0, rowIndex, lastColumn - 1, "Revenue for Department:",
                        false, HorizontalCellAlignment.Right));

                    CreateCell(worksheet, rowIndex, lastColumn, totalDepartmentRevenue, cellFormat: MONEY_FORMAT);

                    rowIndex++;
                }

                // Leave blank row between users
                rowIndex++;
            }
        }

        private static string FormattedName(EmployeeProcessingData.User user)
        {
            // Processing answers may be associated with invalid user IDs.
            if (string.IsNullOrEmpty(user.Name))
            {
                // Processing entries may be associated with invalid user IDs.
                return user.UserId == -99 ? "System" : $"Unknown ({user.UserId})";
            }
            else
            {
                // Processing answers may be associated with invalid user IDs.
                return user.Name;
            }
        }

        private static string GetProcessText(EmployeeProcessingData.OrderAction order)
        {
            switch (order.ActionType)
            {
                case EmployeeProcessingData.OrderActionType.Processed:
                    return string.IsNullOrEmpty(order.ProcessName)
                        ? "N/A"
                        : order.ProcessName;
                case EmployeeProcessingData.OrderActionType.PartInspection:
                    return "Inspection";
                case EmployeeProcessingData.OrderActionType.FinalInspection:
                    return "Final Inspection";
                case EmployeeProcessingData.OrderActionType.Shipped:
                    return "Shipping";
                default:
                    return "N/A";
            }
        }

        #endregion
    }

    public class ShippedByPriorityReport : ExcelBaseReport
    {
        #region Properties

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string Title => "Shipped By Priority";

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            FromDate = FromDate.StartOfDay();
            ToDate = ToDate.EndOfDay();

            var data = GetReportData(FromDate, ToDate);
            CreateSummaryWorksheet(data);
        }

        private void CreateSummaryWorksheet(ReportData data)
        {
            var worksheet = CreateWorksheet("Summary");

            // Header
            var rowIndex = AddCompanyHeaderRows(worksheet, 6, string.Empty);
            rowIndex += 2;

            CreateCell(worksheet, rowIndex, 0, "From Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, FromDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex++;

            CreateCell(worksheet, rowIndex, 0, "To Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, ToDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex += 2;

            // Table header
            CreateHeaderCell(worksheet, rowIndex, 0, "Product Class", 25);
            CreateHeaderCell(worksheet, rowIndex, 1, "Priority", 20);
            CreateHeaderCell(worksheet, rowIndex, 2, "Order Count", 20);
            WrapText(CreateHeaderCell(worksheet, rowIndex, 3, "Order % in Product Class", 20));
            CreateHeaderCell(worksheet, rowIndex, 4, "Part Count", 20);
            WrapText(CreateHeaderCell(worksheet, rowIndex, 5, "Parts % in Product Class", 20));

            rowIndex++;

            var startRowIndex = rowIndex;

            // Contents
            var productClasses = data.Orders.GroupBy(o => o.ProductClass);
            foreach (var productClass in productClasses.OrderBy(g => g.Key))
            {
                long totalOrderCount = productClass.Count();

                long totalPartCount;
                try
                {
                    totalPartCount = productClass.Sum(s => (long)s.PartQuantity);
                }
                catch (ArithmeticException exc)
                {
                    _log.Warn(exc, "Overflow occurred");
                    totalPartCount = long.MaxValue;
                }

                var priorities = productClass.GroupBy(o => o.Priority);
                foreach (var priority in priorities.OrderBy(g => g.Key))
                {
                    var orderCount = priority.Count();

                    long partCount;

                    try
                    {
                        partCount = priority.Sum(s => (long)s.PartQuantity);
                    }
                    catch (ArithmeticException exc)
                    {
                        _log.Warn(exc, "Overflow occurred");
                        partCount = long.MaxValue;
                    }

                    // Add a row for each priority in a product class
                    var orderPercent = totalOrderCount == 0 ? 0 : orderCount / Convert.ToDouble(totalOrderCount);
                    var partPercent = totalPartCount == 0 ? 0 : partCount / Convert.ToDouble(totalPartCount);

                    CreateCell(worksheet, rowIndex, 0, productClass.Key);
                    CreateCell(worksheet, rowIndex, 1, priority.Key);
                    CreateCell(worksheet, rowIndex, 2, orderCount);
                    CreateCell(worksheet, rowIndex, 3, orderPercent, cellFormat: PERCENT_FORMAT);
                    CreateCell(worksheet, rowIndex, 4, partCount);
                    CreateCell(worksheet, rowIndex, 5, partPercent, cellFormat: PERCENT_FORMAT);

                    rowIndex++;
                }
            }

            var lastRowIndex = rowIndex - 1;

            // Totals
            if (data.Orders.Count > 0)
            {
                Bold(CreateCell(worksheet, rowIndex, 0, "Totals:", true));
                Bold(CreateFormulaCell(worksheet, rowIndex, 2, $"=SUM(R{startRowIndex + 1}C3:R{lastRowIndex + 1}C3)", CellReferenceMode.R1C1, true));
                Bold(CreateFormulaCell(worksheet, rowIndex, 4, $"=SUM(R{startRowIndex + 1}C5:R{lastRowIndex + 1}C5)", CellReferenceMode.R1C1, true));

                CreateCell(worksheet, rowIndex, 1, string.Empty, true);
                CreateCell(worksheet, rowIndex, 3, string.Empty, true);
                CreateCell(worksheet, rowIndex, 5, string.Empty, true);
            }
        }

        private static ReportData GetReportData(DateTime fromDate, DateTime toDate)
        {
            var orders = new List<OrderData>();

            using (var taReport = new ShippedByPriorityTableAdapter())
            {
                using (var dtReport = taReport.GetData(fromDate, toDate))
                {
                    foreach (var orderRow in dtReport)
                    {
                        orders.Add(new OrderData
                        {
                            OrderId = orderRow.OrderID,
                            PartQuantity = orderRow.IsPartQuantityNull() ? 0 : orderRow.PartQuantity,
                            Priority = orderRow.IsPriorityNull() ? "N/A" : orderRow.Priority,
                            ProductClass = orderRow.IsProductClassNull() ? "N/A" : orderRow.ProductClass
                        });
                    }
                }
            }

            return new ReportData
            {
                Orders = orders
            };
        }

        #endregion

        #region ReportData

        private class ReportData
        {
            public List<OrderData> Orders { get; set; }
        }

        #endregion

        #region OrderData

        private class OrderData
        {
            public int OrderId { get; set; }

            public int PartQuantity { get; set; }

            public string ProductClass { get; set; }

            public string Priority { get; set; }
        }

        #endregion
    }

    /// <summary>
    /// The turn-around-time report.
    /// </summary>
    public class TurnAroundTimeReport : ExcelBaseReport
    {
        #region Properties

        public override string Title =>
            "Turn Around Time";

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            var fromDate = FromDate.StartOfDay();
            var toDate = ToDate.EndOfDay();
            var reportData = TurnAroundTimeData.GetReportData(fromDate, toDate);

            if (reportData == null)
            {
                _log.Error("Turn Around Time report data was null.");
                return;
            }

            CreateWorksheet(reportData);
        }

        private void CreateWorksheet(TurnAroundTimeData reportData)
        {
            var worksheet = CreateWorksheet("TAT");

            var lastColumnIndex = reportData.ShowProcessingLines
                ? 11
                : 10;

            var orderDateColumn = reportData.ShowProcessingLines ? 8 : 7;
            var requiredDateColumn = reportData.ShowProcessingLines ? 9 : 8;
            var estimatedShipDateColumn = reportData.ShowProcessingLines ? 10 : 9;
            var completedDateColumn = reportData.ShowProcessingLines ? 11 : 10;

            // Header

            var rowIndex = AddCompanyHeaderRows(worksheet, lastColumnIndex, string.Empty);
            rowIndex += 2;

            CreateCell(worksheet, rowIndex, 0, "From Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, reportData.FromDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex++;

            CreateCell(worksheet, rowIndex, 0, "To Date:", false, HorizontalCellAlignment.Left, CellBorderLineStyle.None).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            CreateCell(worksheet, rowIndex, 1, reportData.ToDate.ToShortDateString(), false, HorizontalCellAlignment.Left, CellBorderLineStyle.None);
            rowIndex += 2;

            // Table header
            CreateHeaderCell(worksheet, rowIndex, 00, "WO", 15);
            CreateHeaderCell(worksheet, rowIndex, 01, "Sales Order", 15);
            CreateHeaderCell(worksheet, rowIndex, 02, "Customer", 30);
            CreateHeaderCell(worksheet, rowIndex, 03, "Customer WO", 15);
            CreateHeaderCell(worksheet, rowIndex, 04, "PO", 15);
            CreateHeaderCell(worksheet, rowIndex, 05, "Part", 15);
            CreateHeaderCell(worksheet, rowIndex, 06, "Quantity", 12);

            if (reportData.ShowProcessingLines)
            {
                CreateHeaderCell(worksheet, rowIndex, 07, "Line", 25);
            }

            CreateHeaderCell(worksheet, rowIndex, orderDateColumn, "Order Date", 20);
            CreateHeaderCell(worksheet, rowIndex, requiredDateColumn, "Required Date", 20);
            CreateHeaderCell(worksheet, rowIndex, estimatedShipDateColumn, "Est. Ship Date", 20);
            CreateHeaderCell(worksheet, rowIndex, completedDateColumn, "Completed Date", 20);

            rowIndex++;

            foreach (var order in reportData.Orders.OrderBy(o => o.OrderDate))
            {
                CreateCell(worksheet, rowIndex, 0, order.OrderId);
                CreateCell(worksheet, rowIndex, 1, order.SalesOrderId);
                CreateCell(worksheet, rowIndex, 2, order.CustomerName);
                CreateCell(worksheet, rowIndex, 3, order.CustomerWorkOrder);
                CreateCell(worksheet, rowIndex, 4, order.PurchaseOrder);
                CreateCell(worksheet, rowIndex, 5, order.PartName);

                if (order.PartQuantity.HasValue)
                {
                    CreateCell(worksheet, rowIndex, 6, order.PartQuantity);
                }
                else
                {
                    CreateCell(worksheet, rowIndex, 6, "NA");
                }

                if (reportData.ShowProcessingLines)
                {
                    CreateCell(worksheet, rowIndex, 7, string.Join(",", order.ProcessingLines));
                }

                CreateCell(worksheet, rowIndex, orderDateColumn, order.OrderDate,
                    cellFormat: DATE_FORMAT);

                CreateCell(worksheet, rowIndex, requiredDateColumn, order.RequiredDate,
                    cellFormat: DATE_FORMAT);

                CreateCell(worksheet, rowIndex, estimatedShipDateColumn, order.EstimatedShipDate,
                    cellFormat: DATE_FORMAT);

                CreateCell(worksheet, rowIndex, completedDateColumn, order.CompletedDate,
                    cellFormat: DATE_FORMAT);

                rowIndex++;
            }
        }

        #endregion
    }

    #region ProcessReportsUtilities

    /// <summary>
    /// Contains utility methods for Process-related reports.
    /// </summary>
    static class ProcessReportsUtilities
    {
        /// <summary>
        /// Gets text to use for questions.
        /// </summary>
        /// <param name="question">Question instance to generate text for.</param>
        /// <returns>A non-null string representation of question.</returns>
        public static string GetQuestionText(ProcessesDataset.ProcessQuestionRow question)
        {
            var inputType = (InputType) Enum.Parse(typeof(InputType), question.InputType);
            string text = inputType.ToString();

            switch (inputType)
            {
                case InputType.Decimal:
                case InputType.DecimalBefore:
                case InputType.DecimalAfter:
                case InputType.DecimalDifference:
                case InputType.PreProcessWeight:
                case InputType.PostProcessWeight:
                    text += " [";
                    var decMaxValue = ProcessUtilities.MaxValueDecimal(question);
                    var decMinValue = ProcessUtilities.MinValueDecimal(question);

                    if (decMaxValue.HasValue && decMinValue.HasValue)
                    {
                        text += decMinValue + " - " + decMaxValue;
                    }

                    if (!question.IsNumericUntisNull())
                    {
                        text += (decMaxValue.HasValue ? " " : string.Empty) + question.NumericUntis;
                    }

                    text += "]";
                    break;
                case InputType.Integer:
                case InputType.TimeDuration:
                case InputType.RampTime:
                    text += " [";

                    var intMaxValue = ProcessUtilities.MaxValueInt32(question);
                    var intMinValue = ProcessUtilities.MinValueInt32(question);

                    if (intMaxValue.HasValue && intMinValue.HasValue)
                    {
                        text += intMinValue + " - " + intMaxValue;
                    }

                    if (!question.IsNumericUntisNull())
                    {
                        text += (intMaxValue.HasValue ? " " : string.Empty) + question.NumericUntis;
                    }

                    text += "]";
                    break;
                case InputType.List:
                    if (!question.IsListIDNull() && question.ListsRow != null)
                    {
                        text += " [" + question.ListsRow.Name + "]";
                    }
                    break;
                case InputType.Date:
                case InputType.Time:
                case InputType.None:
                case InputType.String:
                default:
                    break;
            }

            return text;
        }
    }

    #endregion
}