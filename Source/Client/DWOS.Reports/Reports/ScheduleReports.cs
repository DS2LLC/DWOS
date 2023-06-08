using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ScheduleDatasetTableAdapters;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;

namespace DWOS.Reports
{
    public class DepartmentShiftWorkScheduleReport : ExcelBaseReport
    {
        #region Properties

        public override string Title
        {
            get { return "Work Schedule"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Landscape; }
        }

        private List<DepartmentShiftWorkItem> Items { get; set; } 

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentShiftWorkScheduleReport"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public DepartmentShiftWorkScheduleReport(List <DepartmentShiftWorkItem> items)
        {
            Items = items;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        /// <summary>
        /// Creates the report.
        /// </summary>
        private void CreateReport()
        {
            if (Items.Count > 0)
            {

                foreach (var item in Items)
                {
                    var ss = new WorkScheduleSettings();

                    item.Orders.Sort((osr1, osr2) =>
                    {
                        var sortByPriority = !osr1.IsPriorityNull() &&
                            !osr2.IsPriorityNull() &&
                            osr1.Priority != osr2.Priority &&
                            ss.PriorityWeights.Exists(pw => pw.Name == osr1.Priority) &&
                            ss.PriorityWeights.Exists(pw => pw.Name == osr2.Priority);

                        if (sortByPriority)
                            return ss.PriorityWeights.FirstOrDefault(pw => pw.Name == osr2.Priority).Weight.CompareTo(ss.PriorityWeights.FirstOrDefault(pw => pw.Name == osr1.Priority).Weight); //reverse order because higher priority is first
                        //sort by process
                        else if (osr1[WorkScheduleSettings.PROCESS_NAME_COLUMN].ToString() != osr2[WorkScheduleSettings.PROCESS_NAME_COLUMN].ToString())
                            return osr1[WorkScheduleSettings.PROCESS_NAME_COLUMN].ToString().CompareTo(osr2[WorkScheduleSettings.PROCESS_NAME_COLUMN].ToString());
                        //then sort by material
                        else if(!osr1.IsMaterialNull() && !osr2.IsMaterialNull())
                            return osr1.Material.CompareTo(osr2.Material);
                        else
                            return osr1.OrderID.CompareTo(osr2.OrderID);
                    });

                    // Create the worksheet
                    var worksheet = this.CreateWorksheet(item.Department);
                    var rowIndex = AddCompanyHeaderRows(worksheet, 3, " - " + item.Department) + 2;
                    var startRowIndex = rowIndex + 1;

                    AddOrderHeaderRow(worksheet, item.StartDate, item.Department, item.Shift, ref rowIndex);
                    int number = 1;
                    foreach (ScheduleDataset.OrderScheduleRow order in item.Orders)
                    {
                        rowIndex++;
                        AddOrderRow(worksheet, order, number++, rowIndex);
                    }

                    this.AddTotalsRow(worksheet, startRowIndex, rowIndex);

                    // Create a table (subtract 1 from row index so that that 'total' row is not included in the table)
                    this.CreateTable(worksheet, startRowIndex + 1, 7, rowIndex - 1, true);

                    // Set the column Widths
                    worksheet.Columns[0].SetWidth(10, WorksheetColumnWidthUnit.Character);
                    worksheet.Columns[1].SetWidth(15, WorksheetColumnWidthUnit.Character);
                    worksheet.Columns[2].SetWidth(20, WorksheetColumnWidthUnit.Character);
                    worksheet.Columns[3].SetWidth(15, WorksheetColumnWidthUnit.Character);
                    worksheet.Columns[4].SetWidth(25, WorksheetColumnWidthUnit.Character);
                    worksheet.Columns[5].SetWidth(15, WorksheetColumnWidthUnit.Character);
                    worksheet.Columns[6].SetWidth(25, WorksheetColumnWidthUnit.Character);
                    worksheet.Columns[7].SetWidth(65, WorksheetColumnWidthUnit.Character);
                }
            }
        }

        /// <summary>
        /// Adds the order row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="order">The order.</param>
        /// <param name="number">The number.</param>
        /// <param name="rowIndex">Index of the row.</param>
        private void AddOrderRow(Worksheet worksheet, ScheduleDataset.OrderScheduleRow order, int number, int rowIndex)
        {
            try
            {
                this.CreateCell(worksheet, rowIndex, 0, number);
                this.CreateCell(worksheet, rowIndex, 1, order.OrderID);
                this.CreateCell(worksheet, rowIndex, 2, order.CurrentLocation);
                this.CreateCell(worksheet, rowIndex, 3, order.IsPriorityNull() ? "NA" : order.Priority);
                this.CreateCell(worksheet, rowIndex, 4, order.PartName);
                this.CreateCell(worksheet, rowIndex, 5, order.IsPartQuantityNull() ? 0 : order.PartQuantity);
                this.CreateCell(worksheet, rowIndex, 6, order.IsMaterialNull() ? "NA" : order.Material);
                this.CreateCell(worksheet, rowIndex, 7, order[WorkScheduleSettings.PROCESS_NAME_COLUMN]);
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Unable to determine order details.");
            }
        }
        
        /// <summary>
        /// Adds the order header row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="shiftStartTime">The shift start time.</param>
        /// <param name="department">The department.</param>
        /// <param name="shiftNumber">The shift number.</param>
        /// <param name="rowIndex">Index of the row.</param>
        private void AddOrderHeaderRow(Worksheet worksheet, DateTime shiftStartTime, string department, int shiftNumber, ref int rowIndex)
        {
            this.CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 7, department + " Shift " + shiftNumber + " - " + shiftStartTime);
            rowIndex++;

            this.CreateHeaderCell(worksheet, rowIndex, 0, "Num");
            this.CreateHeaderCell(worksheet, rowIndex, 1, "WO");
            this.CreateHeaderCell(worksheet, rowIndex, 2, "Current Location");
            this.CreateHeaderCell(worksheet, rowIndex, 3, "Priority");
            this.CreateHeaderCell(worksheet, rowIndex, 4, "Part");
            this.CreateHeaderCell(worksheet, rowIndex, 5, "Qty");
            this.CreateHeaderCell(worksheet, rowIndex, 6, "Material");
            this.CreateHeaderCell(worksheet, rowIndex, 7, "Processes");
        }

        /// <summary>
        /// Adds the totals row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="stopIndex">Index of the stop.</param>
        private void AddTotalsRow(Worksheet worksheet, int startIndex, int stopIndex)
        {
            var rowIndex = stopIndex + 1;

            this.CreateCell(worksheet, rowIndex, 0, "Totals:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateFormulaCell(worksheet, rowIndex, 1, "=COUNTA(R{0}C{1}:R{2}C{3})".FormatWith(startIndex + 2, 2, rowIndex, 2), CellReferenceMode.R1C1, true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(worksheet, rowIndex, 2, "", true);
            this.CreateCell(worksheet, rowIndex, 3, "", true);
            this.CreateCell(worksheet, rowIndex, 4, "", true);
            this.CreateFormulaCell(worksheet, rowIndex, 5, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startIndex + 2, 6, rowIndex, 6), CellReferenceMode.R1C1, true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(worksheet, rowIndex, 6, "", true);
            this.CreateCell(worksheet, rowIndex, 7, "", true);
        }

        #endregion
    }

    public class DepartmentShiftWorkItem
    {
        public List<ScheduleDataset.OrderScheduleRow> Orders { get; set; }
        public string Department { get; set; }
        public DateTime StartDate { get; set; }
        public int Shift { get; set; }
    }

    public class ScheduleStatusReport : ExcelBaseReport
    {
        #region Properties

        public override string Title
        {
            get { return "Estimated Schedule"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Landscape; }
        }

        private ScheduleDataset.OrderScheduleDataTable Orders { get; set; }

        public WorkScheduler Scheduler { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleStatusReport"/> class.
        /// </summary>
        /// <param name="orders">The orders.</param>
        public ScheduleStatusReport(ScheduleDataset.OrderScheduleDataTable orders)
        {
            Orders = orders;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        /// <summary>
        /// Creates the report.
        /// </summary>
        private void CreateReport()
        {
            CreateSummaryTab();
            CreateDetailsTab();
        }

        #region Summary Tab
        
        private void CreateSummaryTab()
        {
            // Create the worksheet
            var worksheet = this.CreateWorksheet(this.Title);
            var rowIndex = AddCompanyHeaderRows(worksheet, 3, "Summary") + 2;
            var startRowIndex = rowIndex + 1;

            // Add the column headers
            this.AddColumnHeaders(worksheet, rowIndex);

            // Add the data
            DataView view = Orders.DefaultView;
            view.Sort = "EstShipDate";
            foreach (DataRowView item in view)
            {
                rowIndex++;
                AddOrderRow(worksheet, (ScheduleDataset.OrderScheduleRow)item.Row, rowIndex);
            }

            // Add a totals row
            this.AddTotalsRow(worksheet, startRowIndex, rowIndex);

            // Create a table (subtract 1 from row index so that that 'total' row is not included in the table)
            this.CreateTable(worksheet, startRowIndex, 7, rowIndex - 1, true);

            // Set the column Widths
            worksheet.Columns[0].SetWidth(15, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[1].SetWidth(15, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[2].SetWidth(20, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[3].SetWidth(15, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[4].SetWidth(20, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[5].SetWidth(10, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[6].SetWidth(20, WorksheetColumnWidthUnit.Character);
            worksheet.Columns[7].SetWidth(20, WorksheetColumnWidthUnit.Character);
        }

        /// <summary>
        /// Adds the column headers.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="rowIndex">Index of the row.</param>
        private void AddColumnHeaders(Worksheet worksheet, int rowIndex)
        {
            this.CreateHeaderCell(worksheet, rowIndex, 0, "WO");
            this.CreateHeaderCell(worksheet, rowIndex, 1, "PO");
            this.CreateHeaderCell(worksheet, rowIndex, 2, "Customer");
            this.CreateHeaderCell(worksheet, rowIndex, 3, "Priority");
            this.CreateHeaderCell(worksheet, rowIndex, 4, "Part");
            this.CreateHeaderCell(worksheet, rowIndex, 5, "Qty");
            this.CreateHeaderCell(worksheet, rowIndex, 6, "Required");
            this.CreateHeaderCell(worksheet, rowIndex, 7, "Est Ship Date");
            this.CreateHeaderCell(worksheet, rowIndex, 8, "Days Late");
            this.CreateHeaderCell(worksheet, rowIndex, 9, "Days Tardy");
        }

        /// <summary>
        /// Adds the order row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="order">The order.</param>
        /// <param name="rowIndex">Index of the row.</param>
        private void AddOrderRow(Worksheet worksheet, ScheduleDataset.OrderScheduleRow order, int rowIndex)
        {
            try
            {
                this.CreateCell(worksheet, rowIndex, 0, order.OrderID);
                this.CreateCell(worksheet, rowIndex, 1, order.IsPurchaseOrderNull() ? "NA" : order.PurchaseOrder);
                this.CreateCell(worksheet, rowIndex, 2, order.CustomerName);
                this.CreateCell(worksheet, rowIndex, 3, order.IsPriorityNull() ? "NA" : order.Priority);
                this.CreateCell(worksheet, rowIndex, 4, order.PartName);
                this.CreateCell(worksheet, rowIndex, 5, order.IsPartQuantityNull() ? 0 : order.PartQuantity);
                this.CreateCell(worksheet, rowIndex, 6, order.IsRequiredDateNull() ? "NA" : order.RequiredDate.ToShortDateString());
                this.CreateCell(worksheet, rowIndex, 7, order.IsEstShipDateNull() ? "NA" : order.EstShipDate.ToShortDateString());

                if(!order.IsRequiredDateNull() && !order.IsEstShipDateNull())
                {
                    var days = DateUtilities.GetBusinessDays(order.RequiredDate, order.EstShipDate);
                    if (order.RequiredDate > order.EstShipDate)
                        days *= -1;

                    this.CreateCell(worksheet, rowIndex, 8, days);
                    this.CreateCell(worksheet, rowIndex, 9, days > 0 ? days : 0);
                }
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Unable to determine order details.");
            }
        }

        /// <summary>
        /// Adds the totals row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="stopIndex">Index of the stop.</param>
        private void AddTotalsRow(Worksheet worksheet, int startIndex, int stopIndex)
        {
            var rowIndex = stopIndex + 1;

            this.CreateCell(worksheet, rowIndex, 0, "Totals:", true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateFormulaCell(worksheet, rowIndex, 1, "=COUNTA(R{0}C{1}:R{2}C{3})".FormatWith(startIndex + 1, 2, rowIndex, 2), CellReferenceMode.R1C1, true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(worksheet, rowIndex, 2, "", true);
            this.CreateCell(worksheet, rowIndex, 3, "", true);
            this.CreateCell(worksheet, rowIndex, 4, "", true);
            this.CreateFormulaCell(worksheet, rowIndex, 5, "=SUM(R{0}C{1}:R{2}C{3})".FormatWith(startIndex + 1, 6, rowIndex, 6), CellReferenceMode.R1C1, true).CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            this.CreateCell(worksheet, rowIndex, 6, "", true);
            this.CreateCell(worksheet, rowIndex, 7, "", true);
        }

        #endregion

        #region Details Tab

        private void CreateDetailsTab()
        {
            // Create the worksheet
            var worksheet = this.CreateWorksheet("Details");
            var rowIndex = AddCompanyHeaderRows(worksheet, 3, "Details") + 2;
            var startRowIndex = rowIndex + 1;

            // Add the column headers
            AddDetailColumnHeaders(worksheet, startRowIndex);
            startRowIndex++;

            AddDetailRows(worksheet, startRowIndex, 0);
        }

        private void AddDetailColumnHeaders(Worksheet worksheet, int rowIndex)
        {
            this.CreateHeaderCell(worksheet, rowIndex, 0, "WO");
        }

        private void AddDetailRows(Worksheet worksheet, int rowIndex, int columnStartIndex)
        {
            var maxGenerations = ((ScheduleDataset)Orders.DataSet).OrderProcesses.Max(opr => opr.IsNull("Generation") ? 1 : Convert.ToInt32(opr["Generation"]));

            for(int c = 0; c < maxGenerations; c++)
                worksheet.Rows[rowIndex - 1].Cells[columnStartIndex + c + 1].Value = DateTime.Now.AddDays(c).ToShortDateString();

            foreach (var orderSchedule in Orders)
            {
                var orderProcesses = orderSchedule.GetOrderProcessesRows();

                if(orderProcesses.Length > 0)
                {
                    worksheet.Rows[rowIndex].Cells[columnStartIndex].Value = orderSchedule.OrderID;

                    //if order is Late
                    if (!orderSchedule.IsEstShipDateNull() && !orderSchedule.IsRequiredDateNull() && orderSchedule.RequiredDate.Date < orderSchedule.EstShipDate.Date)
                        worksheet.Rows[rowIndex].Cells[columnStartIndex].CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Red);

                    if (!orderSchedule.IsRequiredDateNull() && orderSchedule.RequiredDate >= DateTime.Now.Date)
                    {
                        var days = DateUtilities.GetBusinessDays(DateTime.Now.Date, orderSchedule.RequiredDate);
                        worksheet.Rows[rowIndex].Cells[columnStartIndex + 1 + days].CellFormat.RightBorderColorInfo = new WorkbookColorInfo(System.Drawing.Color.Red);
                        worksheet.Rows[rowIndex].Cells[columnStartIndex + 1 + days].CellFormat.RightBorderStyle = CellBorderLineStyle.Thick;
                    }
                    else //else due before schedule even starts
                    {
                        worksheet.Rows[rowIndex].Cells[columnStartIndex].CellFormat.RightBorderColorInfo = new WorkbookColorInfo(System.Drawing.Color.Red);
                        worksheet.Rows[rowIndex].Cells[columnStartIndex].CellFormat.RightBorderStyle = CellBorderLineStyle.Thick;
                    }

                    var minGeneration = orderProcesses.Min(opr => opr.IsNull("Generation") ? 1 : Convert.ToInt32(opr["Generation"]));
                    var maxGeneration = orderProcesses.Max(opr => opr.IsNull("Generation") ? 1 : Convert.ToInt32(opr["Generation"]));

                    for(int c = minGeneration; c < maxGeneration; c++)
                        worksheet.Rows[rowIndex].Cells[columnStartIndex + c + 1].CellFormat.Fill = CellFill.CreateSolidFill(System.Drawing.Color.Yellow);

                    foreach(var orderProcess in orderProcesses)
                    {
                        if(!orderProcess.IsStartDateNull() && !orderProcess.IsEndDateNull() && !orderProcess.IsNull("Generation"))
                        {
                            var generation = Convert.ToInt32(orderProcess["Generation"]);
                            worksheet.Rows[rowIndex].Cells[columnStartIndex + generation].CellFormat.Fill = CellFill.CreateSolidFill(System.Drawing.Color.Blue);
                        }
                    }

                    rowIndex++;
                }
            }
        }

        //private Dictionary<string, System.Drawing.Color> _departmentColors = new Dictionary<string, System.Drawing.Color>();

        //private void SetCellFormat(string department, WorksheetCell worksheetCell)
        //{
        //    if(!_departmentColors.ContainsKey(department))
        //    {
        //        var r = new Random();
        //        _departmentColors.Add(department, System.Drawing.Color.FromArgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255)));
        //    }

        //    //worksheetCell.CellFormat.Fill = CellFill.CreateSolidFill(_departmentColors[department]);
        //    worksheetCell.CellFormat.Fill = CellFill.CreateSolidFill(System.Drawing.Color.Blue);
        //}

        #endregion

        #endregion
    }

    public class WorkScheduleActualsReport : Report
    {
        #region Properties

        public override string Title
        {
            get { return "Scheduled Orders Completed"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Landscape; }
        }

        [Browsable(true)]
        [Description("The date the schedule was completed for.")]
        [DisplayName("Run Date")]
        [Category("Report")]
        public DateTime RunDate { get; set; }

        [Browsable(true)]
        [Description("The shift to report on.")]
        [DisplayName("Shift")]
        [Category("Report")]
        public int ShiftNumber { get; set; }

        [Browsable(true)]
        [TypeConverter(typeof(DepartmentConverter))]
        [DisplayName("Department")]
        [Category("Report")]
        [Description("The department schedule to display.")]
        public string Department { get; set; }

        #endregion

        #region Methods

        public WorkScheduleActualsReport()
        {
            Department = ApplicationSettings.Current.DepartmentSales;
            ShiftNumber = 1;
            RunDate = DateTime.Now;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            var dtOrders = new ScheduleDataset.OrderScheduleDataTable();
            dtOrders.Constraints.Clear();

            //NOTE: Date parameter must of type DateTime to work correctly
            using(var ta = new OrderScheduleTableAdapter())
                ta.FillByScheduled(dtOrders, Department, RunDate, ShiftNumber);

            SetupReportInfo();

            IGroup headerGroup = _section.AddGroup();
            headerGroup.Layout = Layout.Horizontal;

            // Report Info
            IText uiDate = headerGroup.AddText();
            uiDate.Alignment = TextAlignment.Left;
            uiDate.Style = DefaultStyles.NormalStyle;
            uiDate.Width = new RelativeWidth(50);
            uiDate.Borders = DefaultStyles.DefaultBorders;
            uiDate.Margins.Bottom = 5;
            uiDate.Margins.Right = 5;
            uiDate.Paddings = new Paddings(5, 5, 5, 10);
            uiDate.Background = DefaultStyles.DefaultBackground;

            uiDate.AddContent(Title, DefaultStyles.BlueXLargeStyle);
            uiDate.AddLineBreak();
            uiDate.AddContent("Start Time:  " + RunDate.ToShortDateString());

            // User Info
            IText uiText = headerGroup.AddText();
            uiText.Alignment = TextAlignment.Right;
            uiText.Style = DefaultStyles.NormalStyle;
            uiText.Width = new RelativeWidth(50);
            uiText.Borders = DefaultStyles.DefaultBorders;
            uiText.Margins.Bottom = 5;
            uiText.Margins.Right = 5;
            uiText.Paddings = new Paddings(5, 5, 5, 10);
            uiText.Background = DefaultStyles.DefaultBackground;

            uiText.AddContent(ApplicationSettings.Current.CompanyName, DefaultStyles.BoldStyle);
            uiText.AddLineBreak();
            uiText.AddContent(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            uiText.AddLineBreak();
            uiText.AddContent(SecurityManager.UserName == null ? "System" : SecurityManager.UserName);

            //Orders Summary
            ITable orderTable = null;
            orderTable = _section.AddTable();
            orderTable.Borders = DefaultStyles.DefaultBorders;
            orderTable.Margins.Vertical = 5;
            orderTable.Width = new RelativeWidth(100);

            AddOrderHeaderRow(orderTable);
            int number = 1;

            foreach(ScheduleDataset.OrderScheduleRow item in dtOrders)
                AddOrderRow(number++, item, orderTable);
        }

        private void AddOrderRow(int number, ScheduleDataset.OrderScheduleRow order, ITable table)
        {
            ITableCell cell = null;
            ITableRow row = null;

            try
            {
                Style style = order.IsNull("EndDate") ? DefaultStyles.RedStyle : DefaultStyles.GreenStrikeOutStyle;

                //  - Order Row
                row = table.AddRow();

                cell = row.CreateTableCell(5);
                cell.AddText(number.ToString(), style);

                cell = row.CreateTableCell(5);
                cell.AddText(order.OrderID.ToString(), style);

                cell = row.CreateTableCell(10);
                cell.AddText(order.CurrentLocation, style);

                cell = row.CreateTableCell(10);
                if(order.IsRequiredDateNull())
                    cell.AddText("NA", style);
                else
                    cell.AddText(order.RequiredDate.ToShortDateString(), style);

                cell = row.CreateTableCell(10);
                cell.AddText(order.IsPriorityNull() ? "NA" : order.Priority, style);

                cell = row.CreateTableCell(15);
                cell.AddText(order.PartName, style);

                cell = row.CreateTableCell(5);
                cell.AddText(order.IsPartQuantityNull() ? "NA" : order.PartQuantity.ToString(), style);

                cell = row.CreateTableCell(10);
                cell.AddText(order.IsMaterialNull() ? "NA" : order.Material, style);

                cell = row.CreateTableCell(30);
                cell.AddText(order[WorkScheduleSettings.PROCESS_NAME_COLUMN].ToString(), style);
            }
            catch(Exception exc)
            {
                _log.Warn(exc, "Unable to determine order details.");
            }
        }

        private void AddOrderHeaderRow(ITable table)
        {
            ITableCell cell = table.AddRow().CreateTableCell(100);
            cell.Background = new Background(Brushes.LightBlue);
            cell.AddText(Department + " Shift " + ShiftNumber + " - " + RunDate, DefaultStyles.BlackXLargeStyle, TextAlignment.Left);

            ITableRow headerRow = table.AddRow();

            cell = headerRow.CreateTableCell(5);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("Num", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(5);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("WO", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(10);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("Current Location", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(10);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("Required", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(10);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("Priority", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(15);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("Part", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(5);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("Qty", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(10);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("Material", DefaultStyles.BoldStyle, TextAlignment.Center);

            cell = headerRow.CreateTableCell(30);
            cell.Background = new Background(Brushes.AliceBlue);
            cell.AddText("Processes", DefaultStyles.BoldStyle, TextAlignment.Center);
        }

        #endregion
    }

    public class ProcessScheduleReport : ExcelBaseReport
    {
        #region Fields

        private const int COLUMN_COUNT = 9;
        private const int HISTORY_DAYS = 10;
        private int _startDateColumn = 0;
        private int _endDateColumn = 0;

        #endregion Fields

        #region Properties

        public override string Title
        {
            get { return "Process Schedule"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Landscape; }
        }

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            CreateWorkInProgressReport();
        }
        
        private void CreateWorkInProgressReport()
        {
            Data.Reports.OrdersReport.OrderDataTable orders = null;
            var orderProcesses = new OrderStatusDataSet.OrderProcessSummaryDataTable();

            var orderInfos = new List<OrderInfo>();

            using(var ta = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
                orders = ta.GetOpenOrders();
            using (var ta = new Data.Datasets.OrderStatusDataSetTableAdapters.OrderProcessSummaryTableAdapter())
                ta.Fill(orderProcesses);

            foreach (var item in orders)
            {
                var info = new OrderInfo
                {
                    OrderID = item.OrderID,
                    PurchaseOrder = item.IsPurchaseOrderNull() ? "NA" : item.PurchaseOrder,
                    CustomerName = item.CustomerName,
                    ReqDate = item.IsRequiredDateNull()? (DateTime?) null : item.RequiredDate,
                    EstShipDate = item.IsEstShipDateNull() ? (DateTime?) null : item.EstShipDate,
                    PartName = item.PartName,
                    PartQuantity = item.IsPartQuantityNull() ? 0 : item.PartQuantity,
                    Priority = item.IsPriorityNull() ? "NA" : item.Priority,
                };

                var processes = orderProcesses.Select("OrderID = " + item.OrderID) as OrderStatusDataSet.OrderProcessSummaryRow[];

                info.OrderProcesses = new List<OrderInfo.OrderProcessInfo>();

                foreach (var process in processes)
                    info.OrderProcesses.Add(new OrderInfo.OrderProcessInfo()
                    {
                        ProcessName = process.Name,
                        StartDate = process.IsStartDateNull() ? (DateTime?)null : process.StartDate,
                        EndDate = process.IsEndDateNull() ? (DateTime?)null : process.EndDate,
                        EstEndDate = process.IsEstEndDateNull() ? (DateTime?)null : process.EstEndDate,
                        StepOrder = process.StepOrder,
                        Department = process.Department
                    });

                orderInfos.Add(info);
            }

            if (orderInfos.Count > 0)
                HasData = true;

            FillWorkInProcessReport(orderInfos);
        }

        private void FillWorkInProcessReport(List<OrderInfo> orderInfos)
        {
            var ws = CreateWorksheet(Title);
            var rowIndex = this.AddCompanyHeaderRows(ws, COLUMN_COUNT, " - All") + 1;
            CreateHeaderFooter(ws, Title);

            int startRowIndex = 0;
            if (orderInfos.Count > 0)
            {
                startRowIndex = rowIndex;
                var dates = GetStartAndEndDate(orderInfos);
                rowIndex = AddWorkInProcessHeader(ws, rowIndex, dates);

                foreach (OrderInfo item in orderInfos)
                    AddWorkInProcessRow(ws, item, rowIndex++, 0, dates);

                long totalPartQuantity;

                try
                {
                    totalPartQuantity = orderInfos.Sum(oi => (long)oi.PartQuantity);
                }
                catch (OverflowException)
                {
                    _log.Warn("Arithmetic overflow occurred while running report");
                    totalPartQuantity = long.MaxValue;
                }

                AddWorkInProcessSummaryRow(orderInfos.Count, totalPartQuantity, ws, rowIndex);
            }
            else
            {
                AddWorkInProcessHeader(ws, rowIndex++, new List<DateTime>());
                AddEmptyWorkInProcessRow(ws, rowIndex++, 0);
                AddWorkInProcessSummaryRow(0, 0, ws, rowIndex);
            }

            ws.Tables.Add("A{0}:{1}{2}".FormatWith(startRowIndex + 1, this.ExcelColumnIndexToName(_endDateColumn - 1), rowIndex + 1), true);
        }

        private int AddWorkInProcessHeader(Worksheet worksheet, int rowIndex, List<DateTime> dates)
        {
            var startColumn = 0;
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "WO", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "PO", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Customer", 35);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Part", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Quantity", 12);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Priority", 12);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Req Date", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Ship Date", 15);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Days Till Late", 20);
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Completed", 12); 
            CreateHeaderCell(worksheet, rowIndex, startColumn++, "Total", 12);
            
            _startDateColumn = startColumn;

            foreach (var dateTime in dates)
                CreateHeaderCell(worksheet, rowIndex, startColumn++, dateTime.ToShortDateString(), 20);

            _endDateColumn = startColumn;

            var index = dates.IndexOf(DateTime.Now.Date);
            if (index > 0)
            {
                var processColIndex = _startDateColumn + index;
                worksheet.Rows[rowIndex].Cells[processColIndex].CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Red);
            }

            rowIndex++;

            return rowIndex;
        }

        private void AddWorkInProcessRow(Worksheet worksheet, OrderInfo orderInfo, int rowIndex, int startColumn, List<DateTime> dates)
        {
            try
            {
                //format all of the cells
                for (int i = 0; i < _endDateColumn; i++)
                {
                    var cell = worksheet.Rows[rowIndex].Cells[i];
                    cell.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                    cell.CellFormat.Alignment = HorizontalCellAlignment.Center;
                    ApplyCellBorders(cell);
                }

                var columnIndex = startColumn;

                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.OrderID;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PurchaseOrder;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.CustomerName;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PartName;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.PartQuantity;
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.Priority;

                if (orderInfo.ReqDate.HasValue)
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.ReqDate.Value.Date;
                }

                //if est ship date is after required date
                if (orderInfo.EstShipDate.HasValue && orderInfo.ReqDate.HasValue &&
                    orderInfo.ReqDate.Value.Date < orderInfo.EstShipDate.Value.Date)
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex - 1].CellFormat.Fill =
                        new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Red), null,
                            FillPatternStyle.Solid);

                    var warningMessage = "WARN: Required Date '{0}' is before the Scheduled Ship Date '{1}' and will be late."
                        .FormatWith(orderInfo.ReqDate.Value.ToShortDateString(),
                            orderInfo.EstShipDate.Value.ToShortDateString());

                    worksheet.Rows[rowIndex].Cells[columnIndex - 1].Comment = new WorksheetCellComment()
                    {
                        Text = new FormattedString(warningMessage)
                    };
                }

                if (orderInfo.EstShipDate.HasValue)
                {
                    worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.EstShipDate.Value.Date;
                }

                var lastProcessDate = orderInfo.OrderProcesses.Max(o => o.EstEndDate);

                //if est ship date is less then last process date
                if (orderInfo.EstShipDate.HasValue && lastProcessDate.HasValue &&
                    orderInfo.EstShipDate.Value.Date < lastProcessDate.Value.Date)
                {
                    var errorMsg =
                        "ERROR: Orders Scheduled Ship Date '{0}' occurs before the last processes date of '{1}'."
                            .FormatWith(orderInfo.EstShipDate.Value.ToShortDateString(),
                                lastProcessDate.Value.ToShortDateString());

                    worksheet.Rows[rowIndex].Cells[columnIndex - 1].CellFormat.Fill =
                        new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Red), null,
                            FillPatternStyle.Solid);

                    worksheet.Rows[rowIndex].Cells[columnIndex - 1].Comment = new WorksheetCellComment
                    {
                        Text = new FormattedString(errorMsg)
                    };
                }


                //Days till late
                if (orderInfo.ReqDate.HasValue)
                {
                    var daysTillLate = DateUtilities.GetBusinessDays(DateTime.Now.Date, orderInfo.ReqDate.Value.Date);
                    if (orderInfo.ReqDate.Value.Date < DateTime.Now.Date)
                        daysTillLate *= -1;

                    var lateCell = worksheet.Rows[rowIndex].Cells[columnIndex++];
                    lateCell.Value = daysTillLate;

                    if (daysTillLate < 0)
                    {
                        lateCell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(System.Drawing.Color.Red);
                        lateCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                    }
                }
                else
                    worksheet.Rows[rowIndex].Cells[columnIndex++].Value = "NA";

                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.OrderProcesses.Count(op => op.EndDate.HasValue && op.StartDate.HasValue);
                worksheet.Rows[rowIndex].Cells[columnIndex++].Value = orderInfo.OrderProcesses.Count;
                
                DateTime? lastEstEndDate = null;

                //add all order process columns
                foreach (var orderProcessInfo in orderInfo.OrderProcesses.OrderBy(opi => opi.StepOrder))
                {
                    if (orderProcessInfo.EstEndDate.HasValue)
                    {
                        var index = dates.IndexOf(orderProcessInfo.EstEndDate.Value.Date);
                        
                        if (index >= 0)
                        {
                            var processColIndex = _startDateColumn + index;
                            worksheet.Rows[rowIndex].Cells[processColIndex].Value = orderProcessInfo.Department;

                            var daysLate = Convert.ToInt32(orderProcessInfo.EstEndDate.Value.Date.Subtract(DateTime.Now.Date).TotalDays);

                            if(daysLate < 0)
                                worksheet.Rows[rowIndex].Cells[processColIndex].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Red), null, FillPatternStyle.Solid);
                            else
                            {
                                if(orderProcessInfo.EndDate.HasValue)
                                    worksheet.Rows[rowIndex].Cells[processColIndex].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Green), null, FillPatternStyle.Solid);
                                else if(daysLate == 0) //due today
                                    worksheet.Rows[rowIndex].Cells[processColIndex].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Yellow), null, FillPatternStyle.Solid);
                            }

                            //if started
                            if (orderProcessInfo.StartDate.HasValue && !orderProcessInfo.EndDate.HasValue)
                                worksheet.Rows[rowIndex].Cells[processColIndex].Value += " (S)";
                            //if completed
                            else if (orderProcessInfo.StartDate.HasValue && orderProcessInfo.EndDate.HasValue)
                                worksheet.Rows[rowIndex].Cells[processColIndex].Value += " (F)";

                            //if completed
                            //if(orderProcessInfo.EndDate.HasValue)
                            //{
                            //    var daysLate = DateUtilities.GetBusinessDays(orderProcessInfo.EndDate.Value.Date, orderProcessInfo.EstEndDate.Value.Date);

                            //    if (orderProcessInfo.EstEndDate.Value.Date > orderProcessInfo.EndDate.Value.Date)
                            //        daysLate *= -1;

                            //    if(daysLate < 0)
                            //        worksheet.Rows[rowIndex].Cells[processColIndex].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Red), null, FillPatternStyle.Solid);
                            //    else
                            //        worksheet.Rows[rowIndex].Cells[processColIndex].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Green), null, FillPatternStyle.Solid);

                            //    worksheet.Rows[rowIndex].Cells[processColIndex].Value = orderProcessInfo.Department + " ({0})".FormatWith(daysLate);
                            //}
                            //else if (DateTime.Now.Date > orderProcessInfo.EstEndDate.Value.Date) //if late
                            //{
                            //    worksheet.Rows[rowIndex].Cells[processColIndex].CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.Red), null, FillPatternStyle.Solid);
                            //}
                        }
                    }

                    lastEstEndDate = orderProcessInfo.EstEndDate.HasValue ? orderProcessInfo.EstEndDate : (DateTime?)null;
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddEmptyWorkInProcessRow(Worksheet worksheet, int rowIndex, int startColumn)
        {
            try
            {
                var region1 = CreateMergedHeader(worksheet, rowIndex, startColumn, rowIndex, startColumn + COLUMN_COUNT, "None");
                region1.CellFormat.Alignment = HorizontalCellAlignment.Center;
                region1.CellFormat.Fill = new CellFillPattern(new WorkbookColorInfo(System.Drawing.Color.White), null, FillPatternStyle.Solid);
                ApplyCellBorders(region1.CellFormat);
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to add order row.");
            }
        }

        private void AddWorkInProcessSummaryRow(int orderCount, long totalPartCount, Worksheet worksheet, int rowIndex)
        {
            CreateMergedHeader(worksheet, rowIndex, 0, rowIndex, 3, "Total: ");
            CreateMergedHeader(worksheet, rowIndex, 4, rowIndex, 7, "Total Orders: " + orderCount.ToString("N0")).CellFormat.Alignment = HorizontalCellAlignment.Center;
            CreateMergedHeader(worksheet, rowIndex, 8, rowIndex, _endDateColumn - 1, "Total Parts: " + totalPartCount.ToString("N0")).CellFormat.Alignment = HorizontalCellAlignment.Center;
        }

        private List<DateTime> GetStartAndEndDate(List<OrderInfo> orderInfos)
        {
            var dates = new List<DateTime>();
            var minDate = DateTime.Now.AddDays(-HISTORY_DAYS).Date;

            foreach (var orderInfo in orderInfos)
            {
                foreach (var process in orderInfo.OrderProcesses)
                {
                    //if(process.StartDate.HasValue)
                    //{
                    //    var date = process.StartDate.Value.Date;
                    //    if(date > minDate && !dates.Contains(date))
                    //        dates.Add(date);
                    //}
                    //if (process.EndDate.HasValue)
                    //{
                    //    var date = process.EndDate.Value.Date;
                    //    if (date > minDate && !dates.Contains(date))
                    //        dates.Add(date);
                    //}
                    if (process.EstEndDate.HasValue)
                    {
                        var date = process.EstEndDate.Value.Date;
                        if (date > minDate && !dates.Contains(date))
                            dates.Add(date);
                    }
                }
            }

            if (dates.Count > 0)
                dates.Sort();

            return dates;
        }

        #endregion

        #region OrderInfo

        private class OrderInfo
        {
            public int OrderID { get; set; }
            public string PurchaseOrder { get; set; }
            public string CustomerName { get; set; }
            public DateTime? ReqDate { get; set; }
            public DateTime? EstShipDate { get; set; }
            public string PartName { get; set; }
            public int PartQuantity { get; set; }
            public string Priority { get; set; }
            public List<OrderProcessInfo> OrderProcesses { get; set; }

            public class OrderProcessInfo
            {
                public int StepOrder { get; set; }
                public string ProcessName { get; set; }
                public DateTime? StartDate { get; set; }
                public DateTime? EndDate { get; set; }
                public DateTime? EstEndDate { get; set; }
                public string Department { get; set; }
            }
        }

        #endregion
    }
}