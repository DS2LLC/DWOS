using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using NLog;

namespace DWOS.UI
{
    

    public partial class OrderSummary : UserControl, IOrderSummary
    {
        #region Fields

        private enum CountBy { Order, Part }

        private const string DUE_1DAY_DESC        = "The order is due in 1 business day.";
        private const string DUE_2DAY_DESC        = "The order is due in 2 business days.";
        private const string DUE_3DAY_DESC        = "The order is due in 3 business days.";
        private const string GRID_LAYOUT_FILENAME = "_v15.dat";
        private const string EST_SHIP_DATE_COL    = "EstShipDate";
        private const string WORK_ORDER_COL = "WO";
        
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        
        private int _lastOrderIdSelected;
        private CountSummary _counts                     = new CountSummary();
        private int _dayLookUpCreatedOn                  = -1;
        private Dictionary<DateTime, int> _dayLateLookup = new Dictionary<DateTime, int>();
        private CountBy _countBy                         = CountBy.Order;
        private bool _countAsPercent                     = false;

        public event EventHandler AfterSelectedRowChanged;


        private static readonly string WORK_STATUS_PARTMARKING = Properties.Settings.Default.WorkStatusPartMarking;
        private static readonly string WORK_STATUS_FINALINSPECTION = Properties.Settings.Default.WorkStatusFinalInspection;
        private static readonly string WORK_STATUS_PENDINGINSPECTION = Properties.Settings.Default.WorkStatusPendingQI;
        private static readonly string WORK_STATUS_SHIPPING = Properties.Settings.Default.WorkStatusShipping;
        private static readonly string WORK_STATUS_PENDINGREWORKPLANNING = Properties.Settings.Default.WorkStatusPendingReworkPlanning;
        private static readonly string WORK_STATUS_PENDINGOR = Properties.Settings.Default.WorkStatusPendingOR;
        private static readonly string WORK_STATUS_PENDINGJOIN = Properties.Settings.Default.WorkStatusPendingJoin;
        private static readonly string WORK_STATUS_HOLD = Properties.Settings.Default.WorkStatusHold;
        private static readonly string WORK_STATUS_INPROCESS = Properties.Settings.Default.WorkStatusInProcess;

        #endregion

        #region Properties

        public bool IsDefaultTab { get; set; }

        public string GridName { get; set; }

        public string Key { get; set; }

        public int SelectedWO
        {
            get { return this.GetSelectedRowCellValueInt(this.grdOrderStatus, WORK_ORDER_COL); }
        }

        public string SelectedWorkStatus
        {
            get { return this.GetSelectedRowCellValueString(this.grdOrderStatus, "WorkStatus"); }
        }

        public bool? SelectedHoldStatus
        {
            get { return this.GetSelectedRowCellValueBool(this.grdOrderStatus, Properties.Settings.Default.WorkStatusHold); }
        }

        public OrderType? SelectedOrderType
        {
            get
            {
                var value = this.GetSelectedRowCellValueInt(this.grdOrderStatus, "OrderType");
                return value > 0 ? (OrderType) value : new OrderType?();
            }
        }

        public OrderSummaryType SummaryType { get { return OrderSummaryType.Normal; } }

        public UltraGrid Grid { get { return grdOrderStatus; } }

        public string SelectedLocation
        {
            get { return this.GetSelectedRowCellValueString(this.grdOrderStatus, "CurrentLocation"); }
        }

        private string GridLayoutFilePath
        {
            get
            {
                //if is default then just use name
                return FileSystem.UserAppDataPath() + "\\" + (this.IsDefaultTab ? this.GridName : this.Key) + "_"  + GRID_LAYOUT_FILENAME;;
            }
        }
        
        #endregion

        #region Methods
        
        public OrderSummary()
        {
            InitializeComponent();

            this.Key = Guid.NewGuid().ToString();

            Properties.Settings.Default.SettingsSaving += (s, e) => { ultraTouchProvider1.Enabled = Properties.Settings.Default.TouchEnabled; };
        }

        public void Initialize(Data.Datasets.OrderStatusDataSet orders)
        {
            grdOrderStatus.DataSource = orders.OrderStatus;
            LoadLayouts();
            
            //set custom sort comparer after loading layout
            this.grdOrderStatus.DisplayLayout.Bands[0].Columns["Priority"].SortComparer = new PrioritySortComparer();
            
            //set filter type
            this.grdOrderStatus.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;

            UpdateFilterCount(); //required to load initial data if dataset is already full

            lblDue1Day.Appearance.BackColor = Settings.Default.Late1Day;
            lblDue2Day.Appearance.BackColor = Settings.Default.Late2Day;
            lblDue3Day.Appearance.BackColor = Settings.Default.Late3Day;
        }

        public void BeforeDataReloaded()
        {
            _lastOrderIdSelected = this.SelectedWO;

            //track when the lookup was created in case the day changes on this machine (i.e. left open over night)
            if(_dayLookUpCreatedOn != DateTime.Now.Day)
            {
                _dayLookUpCreatedOn = DateTime.Now.Day;
                _dayLateLookup.Clear();
            }

            this.grdOrderStatus.SuspendLayout();
        }

        public void AfterDataReloaded()
        {
            try
            {
                SelectWorkOrder(_lastOrderIdSelected);
                UpdateFilterCount();
                this.grdOrderStatus.ResumeLayout();
            }
            catch (Exception exc)
            {
                _log.ShowErrorDialog("Error after data reloaded.", exc);
            }
        }

        private void LoadLayouts()
        {
            try
            {
                this.grdOrderStatus.SuspendLayout();

                _log.Info("Loading layouts.");

                //cache the value list as load from file does not keep it
                var orderTypeVL = this.grdOrderStatus.DisplayLayout.ValueLists["OrderType"];
               
                //load grid layout from file
                if (File.Exists(this.GridLayoutFilePath))
                    this.grdOrderStatus.DisplayLayout.Load(this.GridLayoutFilePath);

                if(!String.IsNullOrWhiteSpace(ApplicationSettings.Current.CompanyLogoImagePath) && File.Exists(ApplicationSettings.Current.CompanyLogoImagePath))
                    //else set the background, since it is based on customer logo not assigned by default
                    this.grdOrderStatus.DisplayLayout.Appearance.ImageBackground = new Bitmap(ApplicationSettings.Current.CompanyLogoImagePath);

                this.grdOrderStatus.DisplayLayout.Bands[0].Columns["OrderType"].ValueList = orderTypeVL;
                this.grdOrderStatus.DisplayLayout.Bands[0].Columns["OrderType"].CellDisplayStyle = CellDisplayStyle.FormattedText;
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().ErrorException("Error loading layouts from the file system.", exc);
            }
            finally
            {
                this.grdOrderStatus.ResumeLayout();
            }
        }

        private int GetSelectedRowCellValueInt(UltraGrid grid, string cellName)
        {
            try
            {
                if (grid.Selected.Rows.Count == 1)
                {
                    var currentRow = grid.Selected.Rows[0];
                    if (currentRow != null && currentRow.IsDataRow && currentRow.Band.Columns.Exists(cellName))
                    {
                        object o = currentRow.GetCellValue(cellName);
                        if (o != null && o != DBNull.Value)
                            return Convert.ToInt32(o);
                    }
                }

                return -1;
            }
            catch (Exception exc)
            {
                string errorMsg = "Error converting getting cell value for: " + cellName;
                _log.Error(errorMsg, exc);
                return -1;
            }
        }

        private string GetSelectedRowCellValueString(UltraGrid grid, string cellName)
        {
            try
            {
                if (grid.Selected.Rows.Count == 1)
                {
                    UltraGridRow currentRow = grid.Selected.Rows[0];
                    if (currentRow != null && currentRow.IsDataRow && currentRow.Band.Columns.Exists(cellName))
                    {
                        object o = currentRow.GetCellValue(cellName);
                        if (o != null && o != DBNull.Value)
                            return o.ToString();
                    }
                }

                return null;
            }
            catch (Exception exc)
            {
                string errorMsg = "Error converting getting cell value for: " + cellName;
                _log.Error(errorMsg, exc);
                return null;
            }
        }

        private bool GetSelectedRowCellValueBool(UltraGrid grid, string cellName)
        {
            try
            {
                if (grid.Selected.Rows.Count == 1)
                {
                    UltraGridRow currentRow = grid.Selected.Rows[0];
                    if (currentRow != null && currentRow.IsDataRow && currentRow.Band.Columns.Exists(cellName))
                    {
                        object o = currentRow.GetCellValue(cellName);
                        if (o != null && o != DBNull.Value)
                            return Convert.ToBoolean(o);
                    }
                }

                return false;
            }
            catch (Exception exc)
            {
                string errorMsg = "Error converting getting cell value for: " + cellName;
                _log.Error(errorMsg, exc);
                return false;
            }
        }

        private Infragistics.Win.Appearance GetCellApperance(string colorName)
        {
            if (!this.grdOrderStatus.DisplayLayout.Appearances.Exists(colorName))
            {
                var app = this.grdOrderStatus.DisplayLayout.Appearances.Add(colorName);
                app.ForeColor = Color.FromName(colorName);
                return app;
            }

            return this.grdOrderStatus.DisplayLayout.Appearances[colorName];
        }

        private Infragistics.Win.Appearance GetCellBackgroundApperance(string colorName)
        {
            var key = colorName + "BACK";

            if (!this.grdOrderStatus.DisplayLayout.Appearances.Exists(key))
            {
                var app = this.grdOrderStatus.DisplayLayout.Appearances.Add(key);
                app.BackColor = Color.FromName(colorName);
                app.FontData.Underline = DefaultableBoolean.True;

                return app;
            }

            return this.grdOrderStatus.DisplayLayout.Appearances[key];
        }

        private Infragistics.Win.Appearance GetCellBackgroundApperanceImage(string imageName)
        {
            var key = imageName + "_Image";

            if (!this.grdOrderStatus.DisplayLayout.Appearances.Exists(key))
            {
                var app = this.grdOrderStatus.DisplayLayout.Appearances.Add(key);
                app.Image = imageName;
                app.ImageAlpha = Alpha.Opaque;
                return app;
            }

            return this.grdOrderStatus.DisplayLayout.Appearances[key];
        }

        public void UpdateFilterCount()
        {
            try
            {
                if (this.grdOrderStatus.Rows == null)
                    return;

                bool hasFilters = this.grdOrderStatus.Rows.ColumnFilters.Cast<ColumnFilter>().Any(filter => filter.FilterConditions.Count > 0);

                if(hasFilters)
                    UpdateFilterCount(this.grdOrderStatus.Rows.GetFilteredInNonGroupByRows());
                else
                    UpdateFilterCount(this.grdOrderStatus.Rows);

                lblTotalCount.Text  = _counts.TotalCount.ToString("N0");
                lblLateCount.Text   = _countAsPercent ? _counts.LateCountPercent.ToString("P0") : _counts.LateCount.ToString("N0");
                lblDue1Day.Text     = _countAsPercent ?_counts.Day1CountPercent.ToString("P0") : _counts.Day1Count.ToString("N0");
                lblDue2Day.Text     = _countAsPercent ? _counts.Day2CountPercent.ToString("P0") : _counts.Day2Count.ToString("N0");
                lblDue3Day.Text     = _countAsPercent ? _counts.Day3CountPercent.ToString("P0") : _counts.Day3Count.ToString("N0");

                lblQuarantine.Text     = _countAsPercent ? _counts.QuarantineCountPercent.ToString("P0") : _counts.QuarantineCount.ToString("N0");
                lblInternalRework.Text = _countAsPercent ? _counts.IntReworkCountPercent.ToString("P0") : _counts.IntReworkCount.ToString("N0");
                lblExternalRework.Text = _countAsPercent ? _counts.ExtReworkCountPercent.ToString("P0") : _counts.ExtReworkCount.ToString("N0");
                lblHold.Text           = _countAsPercent ? _counts.HoldCountPercent.ToString("P0") : _counts.HoldCount.ToString("N0");
            }
            catch (Exception exc)
            {
                string errorMsg = "Error updating filter row count.";
                _log.ErrorException(errorMsg, exc);
            }
        }

        private void UpdateFilterCount(IEnumerable<UltraGridRow> rows)
        {
            _counts.Reset();

            foreach(var row in rows)
            {
                var or      = ((DataRowView)row.ListObject).Row as OrderStatusDataSet.OrderStatusRow;
                var count   = _countBy == CountBy.Order || or.IsPartQuantityNull() ? 1 : or.PartQuantity;
                
                _counts.TotalCount += count;
                
                if (!or.IsEstShipDateNull())
                {
                    var daysTillLate = GetDaysLate(or.EstShipDate.Date);

                    if (daysTillLate < 0) //aka late
                        _counts.LateCount += count;
                    else if (daysTillLate == 0)
                        _counts.Day1Count += count;
                    else if (daysTillLate == 1)
                        _counts.Day2Count += count;
                    else if (daysTillLate == 2)
                        _counts.Day3Count += count;
                }

                var ot = (OrderType) or.OrderType;

                switch(ot)
                {
                    case OrderType.ReworkExt:
                        _counts.ExtReworkCount += count;
                        break;
                    case OrderType.ReworkInt:
                        _counts.IntReworkCount += count;
                        break;
                    case OrderType.ReworkHold:
                        _counts.HoldCount += count;
                        break;
                    case OrderType.Quarantine:
                        _counts.QuarantineCount += count;
                        break;
                }
            }
        }

        public void SaveLayout()
        {
            //persist settings
            if(this.grdOrderStatus.DisplayLayout != null)
                this.grdOrderStatus.DisplayLayout.Save(this.GridLayoutFilePath, PropertyCategories.ColumnFilters | PropertyCategories.SortedColumns | PropertyCategories.General);
        }
        
        public void SelectWorkOrder(int orderID)
        {
            try
            {
                if (this.grdOrderStatus.Rows != null && this.grdOrderStatus.Rows.Count > 0)
                {
                    this.grdOrderStatus.Selected.Rows.Clear();
                    this.grdOrderStatus.ActiveRow = this.grdOrderStatus.Rows[0];
                    this.grdOrderStatus.ActiveRow = null;

                    if(orderID > 0)
                    {
                        var orderRow = this.grdOrderStatus.Rows.FirstOrDefault(r => Convert.ToInt32(r.Cells[WORK_ORDER_COL].Value) == orderID);
                        
                        if(orderRow != null)
                        {
                            orderRow.Selected = true;
                            this.grdOrderStatus.ActiveRowScrollRegion.ScrollRowIntoView(orderRow);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().ErrorException("Error selecting order", exc);
            }
        }

        private void OnDisposeMe()
        {
            AfterSelectedRowChanged = null;
        }

        public void RefreshData()
        {
            //NOT USED: uses the before/after methods
        }

        private void FilterByOrderType(OrderType type)
        {
            grdOrderStatus.DisplayLayout.Bands[0].ColumnFilters.ClearAllFilters();

            var typeName = "";
            switch(type)
            {
                case OrderType.Normal:
                    typeName = "Normal";
                    break;
                case OrderType.ReworkExt:
                    typeName = "Ext Rework";
                    break;
                case OrderType.ReworkInt:
                    typeName = "Int Rework";
                    break;
                case OrderType.ReworkHold:
                    typeName = "Rework Hold";
                    break;
                case OrderType.Lost:
                    typeName = "Lost";
                    break;
                case OrderType.Quarantine:
                    typeName = "Quarantine";
                    break;
                default:
                    break;
            }

            grdOrderStatus.DisplayLayout.Bands[0].ColumnFilters["OrderType"].FilterConditions.Add(FilterComparisionOperator.Equals, typeName);
            UpdateFilterCount();
        }

        private void FilterByDate(int daysLate)
        {
            grdOrderStatus.DisplayLayout.Bands[0].ColumnFilters.ClearAllFilters();

            if(daysLate < 0)
            {
                var date = DateTime.Now.Date.Subtract(TimeSpan.FromDays(1)).EndOfDay();
                grdOrderStatus.DisplayLayout.Bands[0].ColumnFilters[EST_SHIP_DATE_COL].FilterConditions.Add(FilterComparisionOperator.LessThanOrEqualTo, date);
            }
            else
            {
                var dateLate = _dayLateLookup.FirstOrDefault(kvp => kvp.Value == daysLate);
                grdOrderStatus.DisplayLayout.Bands[0].ColumnFilters[EST_SHIP_DATE_COL].FilterConditions.Add(FilterComparisionOperator.Equals, dateLate.Key);
            }
            UpdateFilterCount();
        }

        private int GetDaysLate(DateTime date)
        {
            var daysTillLate = -1;

            if (date < DateTime.Now.StartOfDay())
                daysTillLate = -1;
            else
            {
                if (_dayLateLookup.Count < 1)
                {
                    _dayLateLookup.Add(DateTime.Now.StartOfDay(), 0);
                    _dayLateLookup.Add(DateUtilities.AddBusinessDays(DateTime.Now.Date, 1), 1);
                    _dayLateLookup.Add(DateUtilities.AddBusinessDays(DateTime.Now.Date, 2), 2);
                    _dayLateLookup.Add(DateUtilities.AddBusinessDays(DateTime.Now.Date, 3), 3);
                }

                daysTillLate = _dayLateLookup.ContainsKey(date) ? _dayLateLookup[date] : 99;
            }

            return daysTillLate;
        }

        public void SortData()
        {
            
        }

        #endregion

        #region Events

        private void grdOrderStatus_ClickCell(object sender, ClickCellEventArgs e)
        {
            try
            {
                if (e.Cell != null)
                {
                    e.Cell.Row.Selected = true;

                    if (e.Cell.Column.Key == "Part")
                    {
                        using (var p = new QuickViewPart())
                        {
                            using(var ta = new Data.Datasets.OrderStatusDataSetTableAdapters.OrderStatusTableAdapter())
                            {
                                int? partID = ta.GetPartID(this.SelectedWO);

                                if(partID.HasValue)
                                {
                                    p.PartID = partID.Value;
                                    p.ShowDialog(this);
                                }
                            }
                        }
                    }
                    else if (e.Cell.Column.Key == WORK_ORDER_COL)
                    {
                        using (var o = new QuickViewOrder())
                        {
                            e.Cell.Row.Selected = true;

                            if (this.SelectedWO > 0)
                            {
                                o.OrderID = this.SelectedWO;
                                o.ShowDialog(this);
                            }
                        }
                    }
                    else if (e.Cell.Column.Key == "CurrentProcess")
                    {
                        if (this.SelectedWO > 0 && e.Cell.Value.ToString() != "NA")
                        {
                            var activity     = new ProcessingActivity(this.SelectedWO);
                            activity.FindOrderProcessByName(e.Cell.Value.ToString());
                            
                            if(activity.OrderProcessID.HasValue)
                            {
                                using (var frm = new OrderProcessing2(activity) { Mode = OrderProcessing2.ProcessingMode.ViewOnly })
                                    frm.ShowDialog(this);
                            }
                        }
                       
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error displaying quick view.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void grdOrders_BeforeRowFilterChanged(object sender, BeforeRowFilterChangedEventArgs e)
        {
            // Trap for DateTime columns.
            if (e.NewColumnFilter.Column.DataType == typeof(DateTime))
            {
                // Store the original filters in an array.
                object[] originalFilterConditions = e.NewColumnFilter.FilterConditions.All;

                // Clear the filters.
                e.NewColumnFilter.FilterConditions.Clear();

                // Now loop through the original filters and replace them with a DateFilterCondition
                // for each one. The DateFilterCondition is a custom FilterCondition class that
                // only compare the Date and ignores the time.
                for (int i = 0; i < originalFilterConditions.Length; i++)
                {
                    var originalFilterCondition = (FilterCondition)originalFilterConditions[i];
                    e.NewColumnFilter.FilterConditions.Add(new DateFilterCondition(originalFilterCondition));
                }
            }
        }

        private void grdOrderStatus_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
        {
            this.UpdateFilterCount();
        }

        private void grdOrders_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row != null && e.Row.IsDataRow && !e.ReInitialize)
            {
                var or = ((DataRowView)e.Row.ListObject).Row as OrderStatusDataSet.OrderStatusRow;

                if (or != null)
                {
                    //Color Coded based on priority
                    if (or.PriorityRow != null)
                        e.Row.Cells["Priority"].Appearance = this.GetCellApperance(or.PriorityRow.Color);
                    
                    switch (or.OrderType)
                    {        
                        case 1:
                            if (or.Hold)
                            {
                                e.Row.Cells[WORK_ORDER_COL].Appearance = GetCellBackgroundApperanceImage("hold");
                                e.Row.Cells[WORK_ORDER_COL].ToolTipText = "On Hold";
                            }
                            break;
                        case 3: //OrderType.ReworkExt
                            e.Row.Cells[WORK_ORDER_COL].Appearance = GetCellBackgroundApperanceImage("extRework");
                            e.Row.Cells[WORK_ORDER_COL].ToolTipText = "External Rework";
                            break;
                        case 4: //OrderType.ReworkInt
                            e.Row.Cells[WORK_ORDER_COL].Appearance = GetCellBackgroundApperanceImage("intRework");
                            e.Row.Cells[WORK_ORDER_COL].ToolTipText = "Internal Rework";
                            break;
                        case 5: //OrderType.ReworkHold
                            e.Row.Cells[WORK_ORDER_COL].Appearance = GetCellBackgroundApperanceImage("hold");
                            e.Row.Cells[WORK_ORDER_COL].ToolTipText = "Hold Rework";
                            break;
                        case 6: //OrderType.Lost
                            e.Row.Cells[WORK_ORDER_COL].Appearance = GetCellBackgroundApperanceImage("hold");
                            e.Row.Cells[WORK_ORDER_COL].ToolTipText = "Lost";
                            break;
                        case 7: //OrderType.Quarantine
                            e.Row.Cells[WORK_ORDER_COL].Appearance = GetCellBackgroundApperanceImage("quarantine");
                            e.Row.Cells[WORK_ORDER_COL].ToolTipText = "Quarantine";
                            break;
                    }

                    //if past due date
                    if (!or.IsEstShipDateNull())
                    {
                        var daysTillLate = GetDaysLate(or.EstShipDate.Date);
                       
                        if (daysTillLate < 0) //aka late
                            e.Row.Cells[EST_SHIP_DATE_COL].Appearance = this.GetCellBackgroundApperance(Settings.Default.Late0Day.Name);
                        else if(daysTillLate == 0)
                        {
                            e.Row.Cells[EST_SHIP_DATE_COL].Appearance = this.GetCellBackgroundApperance(Settings.Default.Late1Day.Name);
                            e.Row.Cells[EST_SHIP_DATE_COL].ToolTipText = DUE_1DAY_DESC;
                        }
                        else if(daysTillLate == 1)
                        {
                            e.Row.Cells[EST_SHIP_DATE_COL].Appearance = this.GetCellBackgroundApperance(Settings.Default.Late2Day.Name);
                            e.Row.Cells[EST_SHIP_DATE_COL].ToolTipText = DUE_2DAY_DESC;
                        }
                        else if(daysTillLate == 2)
                        {
                            e.Row.Cells[EST_SHIP_DATE_COL].Appearance = this.GetCellBackgroundApperance(Settings.Default.Late3Day.Name);
                            e.Row.Cells[EST_SHIP_DATE_COL].ToolTipText = DUE_3DAY_DESC;
                        }
                    }

                    if(or.WorkStatus == WORK_STATUS_INPROCESS)
                    {
                        //short circuit the odd work statuses below
                    }
                    else if (or.WorkStatus == WORK_STATUS_PARTMARKING)
                    {
                        or.CurrentProcess   = WORK_STATUS_PARTMARKING;
                        or.NextDept         = WORK_STATUS_FINALINSPECTION;
                        e.Row.Cells["CurrentProcess"].CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.PlainText;
                    }
                    else if (or.WorkStatus == WORK_STATUS_FINALINSPECTION)
                    {
                        or.CurrentProcess   = WORK_STATUS_FINALINSPECTION;
                        or.NextDept         = WORK_STATUS_SHIPPING;
                        e.Row.Cells["CurrentProcess"].CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.PlainText;
                    }
                    else if (or.WorkStatus == WORK_STATUS_PENDINGINSPECTION)
                    {
                        or.CurrentProcess = "Control Inspection";
                        e.Row.Cells["CurrentProcess"].CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.PlainText;
                    }
                    else if (or.WorkStatus == WORK_STATUS_SHIPPING)
                    {
                        or.CurrentProcess = WORK_STATUS_SHIPPING;
                        or.NextDept       = "None";
                        e.Row.Cells["CurrentProcess"].CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.PlainText;
                    }
                    else if (or.WorkStatus == WORK_STATUS_PENDINGREWORKPLANNING)
                    {
                        or.CurrentProcess = "Rework Planning";
                        or.NextDept       = "TBD";
                        e.Row.Cells["CurrentProcess"].CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.PlainText;
                    }
                    else if (or.WorkStatus == WORK_STATUS_PENDINGOR)
                    {
                        or.CurrentProcess = "Order Review";
                        e.Row.Cells["CurrentProcess"].CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.PlainText;
                    }
                    else if (or.WorkStatus == WORK_STATUS_PENDINGJOIN)
                    {
                        or.CurrentProcess = WORK_STATUS_PENDINGJOIN;
                        e.Row.Cells["CurrentProcess"].CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.PlainText;
                    }
                    else if (or.WorkStatus == WORK_STATUS_HOLD)
                    {
                        or.CurrentProcess = "On Hold";
                        e.Row.Cells["CurrentProcess"].CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.PlainText;
                    }

                    if(or.InBatch)
                    {
                        e.Row.Cells["WorkStatus"].Appearance  = GetCellBackgroundApperanceImage("batch");
                        e.Row.Cells["WorkStatus"].ToolTipText = "In Batch";
                    }
                }
            }
        }
        
        private void grdOrderStatus_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if(AfterSelectedRowChanged != null)
                AfterSelectedRowChanged(this, e);
        }

        private void cboCountType_SelectionChanged(object sender, EventArgs e)
        {
            switch (cboCountType.Text)
            {
                case "Orders":
                    _countBy = CountBy.Order;
                    _countAsPercent = false;
                    break;
                case "Order %":
                    _countBy = CountBy.Order;
                    _countAsPercent = true;
                    break;
                case "Parts":
                    _countBy = CountBy.Part;
                    _countAsPercent = false;
                    break;
                case "Part %":
                    _countBy = CountBy.Part;
                    _countAsPercent = true;
                    break;
            }

            //redo if changed
            UpdateFilterCount();
        }
        
        private void lblTotalCount_Click(object sender, EventArgs e)
        {
            grdOrderStatus.DisplayLayout.Bands[0].ColumnFilters.ClearAllFilters();
            UpdateFilterCount();
        }

        private void lblQuarantine_Click(object sender, EventArgs e) { FilterByOrderType(OrderType.Quarantine); }

        private void lblInternalRework_Click(object sender, EventArgs e)
        {
            FilterByOrderType(OrderType.ReworkInt);
        }

        private void lblExternalRework_Click(object sender, EventArgs e)
        {
            FilterByOrderType(OrderType.ReworkExt);
        }

        private void lblHold_Click(object sender, EventArgs e)
        {
            FilterByOrderType(OrderType.ReworkHold);
        }

        private void lblLateCount_Click(object sender, EventArgs e)
        {
            FilterByDate(-1);
        }

        private void lblDue1Day_Click(object sender, EventArgs e)
        {
            FilterByDate(0);
        }

        private void lblDue2Day_Click(object sender, EventArgs e)
        {
            FilterByDate(1);
        }

        private void lblDue3Day_Click(object sender, EventArgs e)
        {
            FilterByDate(2);
        }

        #endregion

        #region Priority Sort Comparer

        public class PrioritySortComparer : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                if (x is UltraGridCell && y is UltraGridCell)
                {
                    var xCell = (UltraGridCell)x;
                    var yCell = (UltraGridCell)y;

                    if (xCell.Value is String && yCell.Value is String)
                    {
                        int xPriority = this.GetPriorityOrder(xCell.Value as String);
                        int yPriority = this.GetPriorityOrder(yCell.Value as String);

                        return xPriority.CompareTo(yPriority);
                    }
                }

                return 0;
            }

            #endregion

            private int GetPriorityOrder(string priority)
            {
                if (String.IsNullOrEmpty(priority))
                    return 99;

                priority = priority.ToLower();

                switch (priority)
                {
                    case "first priority":
                        return 1;
                    case "weekend expedite":
                        return 2;
                    case "expedite":
                        return 3;
                    case "rush":
                        return 4;
                    case "normal":
                        return 5;
                    default:
                        break;
                }

                return 10;
            }
        }

        #endregion

        #region DateFilterCondition

        /// <summary>
        ///   Filters only by date and not time.
        /// </summary>
        [Serializable]
        internal class DateFilterCondition : FilterCondition
        {
            public DateFilterCondition() { }

            public DateFilterCondition(FilterCondition originalFilterCondition)
                : base(originalFilterCondition.Column, originalFilterCondition.ComparisionOperator, originalFilterCondition.CompareValue) { }

            protected DateFilterCondition(SerializationInfo info, StreamingContext context)
                : base(info, context) { }

            public override bool MeetsCriteria(UltraGridRow row)
            {
                // If the CompareValue is not a DateTime, then just call the base implementation.
                // This is to handle the "special" filter options like "(All)" or null.
                if ((CompareValue is DateTime) == false)
                    return base.MeetsCriteria(row);

                var compareDate = (DateTime)CompareValue;
                switch (base.ComparisionOperator)
                {
                    case FilterComparisionOperator.Equals:
                        return compareDate.Date == ((DateTime)row.Cells[Column].Value).Date;
                    case FilterComparisionOperator.GreaterThan:
                        return compareDate.Date < ((DateTime)row.Cells[Column].Value).Date;
                    case FilterComparisionOperator.GreaterThanOrEqualTo:
                        return compareDate.Date <= ((DateTime)row.Cells[Column].Value).Date;
                    case FilterComparisionOperator.LessThan:
                        return compareDate.Date > ((DateTime)row.Cells[Column].Value).Date;
                    case FilterComparisionOperator.LessThanOrEqualTo:
                        return compareDate.Date >= ((DateTime)row.Cells[Column].Value).Date;
                    case FilterComparisionOperator.NotEquals:
                        return compareDate.Date != ((DateTime)row.Cells[Column].Value).Date;
                    case FilterComparisionOperator.Contains:
                    case FilterComparisionOperator.Custom:
                    case FilterComparisionOperator.DoesNotContain:
                    case FilterComparisionOperator.DoesNotEndWith:
                    case FilterComparisionOperator.DoesNotMatch:
                    case FilterComparisionOperator.DoesNotStartWith:
                    case FilterComparisionOperator.EndsWith:
                    case FilterComparisionOperator.Like:
                    case FilterComparisionOperator.Match:
                    case FilterComparisionOperator.NotLike:
                    case FilterComparisionOperator.StartsWith:
                    default:
                        return false;
                }
            }
        }

        #endregion

        #region CountSummary
        
        private class CountSummary
        {
            public int TotalCount { get; set; }
            public int LateCount { get; set; }
            public int Day1Count { get; set; }
            public int Day2Count { get; set; }
            public int Day3Count { get; set; }

            public int QuarantineCount { get; set; }
            public int ExtReworkCount { get; set; }
            public int IntReworkCount { get; set; }
            public int HoldCount { get; set; }
            
            public double LateCountPercent
            {
                get { return TotalCount > 0 ? Math.Round(Convert.ToDouble(LateCount) / Convert.ToDouble(TotalCount), 2) : 0D; }
            }

            public double Day1CountPercent 
            {
                get { return TotalCount > 0 ? Math.Round(Convert.ToDouble(Day1Count) / Convert.ToDouble(TotalCount), 2) : 0D; }
            }

            public double Day2CountPercent
            {
                get { return TotalCount > 0 ? Math.Round(Convert.ToDouble(Day2Count) / Convert.ToDouble(TotalCount), 2) : 0D; }
            }

            public double Day3CountPercent
            {
                get { return TotalCount > 0 ? Math.Round(Convert.ToDouble(Day3Count) / Convert.ToDouble(TotalCount), 2) : 0D; }
            }

            public double QuarantineCountPercent
            {
                get { return TotalCount > 0 ? Math.Round(Convert.ToDouble(QuarantineCount) / Convert.ToDouble(TotalCount), 2) : 0D; }
            }

            public double ExtReworkCountPercent
            {
                get { return TotalCount > 0 ? Math.Round(Convert.ToDouble(ExtReworkCount) / Convert.ToDouble(TotalCount), 2) : 0D; }
            }

            public double IntReworkCountPercent
            {
                get { return TotalCount > 0 ? Math.Round(Convert.ToDouble(IntReworkCount) / Convert.ToDouble(TotalCount), 2) : 0D; }
            }

            public double HoldCountPercent
            {
                get { return TotalCount > 0 ? Math.Round(Convert.ToDouble(HoldCount) / Convert.ToDouble(TotalCount), 2) : 0D; }
            }

            public void Reset()
            {
                LateCount       = 0;
                TotalCount      = 0;
                Day1Count       = 0;
                Day2Count       = 0;
                Day3Count       = 0;

                QuarantineCount = 0;
                ExtReworkCount  = 0;
                IntReworkCount  = 0;
                HoldCount       = 0;
            }
        }

        #endregion
    }
}
