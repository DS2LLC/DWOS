using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.UI.Admin;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinGrid;
using NLog;

namespace DWOS.UI.QA
{
    using Infragistics.Win.UltraWinMaskedEdit;
    using Data.Order.Activity;

    public partial class OrderProcessEditor : UserControl
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private CommandManager _cmdManager = new CommandManager();
        private DisplayDisabledTooltips _displayTips;
        private BindingList<ProcessItem> _processes = new BindingList<ProcessItem>();
        private bool _viewOnly;
        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("OrderProcess", new UltraGridBandSettings());

        #endregion

        #region Properties

        public OrdersDataSet.OrderRow OrderRow { get; set; }

        public bool IsInRework { get; set; }

        public bool DisplayCOCColumn { get; set; }

        public bool ViewOnly
        {
            get
            {
                return _viewOnly;
            }
            set
            {
                _viewOnly = value;
                grdProcesses.DisplayLayout.Override.AllowUpdate = value ?
                    DefaultableBoolean.False :
                    DefaultableBoolean.Default;

                foreach (var control in pnlButtons.Controls.OfType<Control>())
                {
                    control.Enabled = !value;
                }
            }
        }

        #endregion

        #region Methods

        public OrderProcessEditor()
        {
            InitializeComponent();

            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                // Setup dependency injection to prevent design-time errors.
                DesignTime.DesignTimeUtilities.SetupDependencyInjection();
            }

            if(DesignMode)
                return;

            DisplayCOCColumn = true;
            this._displayTips = new DisplayDisabledTooltips(this, this.ultraToolTipManager1);
            grdProcesses.SetDataBinding(_processes, null, true);
        }

        public void LoadData(OrdersDataSet orders, bool isInRework = false)
        {
            this.dsOrders = orders;
            IsInRework = isInRework;
            
            LoadCommands();
        }

        public void LoadProcesses(int orderID)
        {
            try
            {
                using(new UsingGridLoad(grdProcesses))
                {
                    grdProcesses.Selected.Rows.Clear();
                    grdProcesses.ActiveRow = null;
                    _processes.Clear();

                    OrderRow = this.dsOrders.Order.FindByOrderID(orderID);

                    if(OrderRow != null)
                    {
                        //for each order process step
                        foreach(var op in OrderRow.GetOrderProcessesRows().Where(r => r.IsValidState()))
                        {
                            //if process alias not loaded then load now; can occur via order entry changing the order processes on the fly
                            if(op.ProcessAliasSummaryRow == null && op.ProcessAliasID > 0)
                            {
                                using(var taPS = new ProcessAliasSummaryTableAdapter {ClearBeforeFill = false})
                                    taPS.FillById(dsOrders.ProcessAliasSummary, op.ProcessAliasID);
                            }

                            AddProcessItem(op);
                        }
                    }

                    grdProcesses.DataBind();
                }

                if(_processes.Count > 0)
                    grdProcesses.PerformAction(UltraGridAction.FirstRowInBand);
                _cmdManager.RefreshAll();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error loading order processing info");
            }
        }

        private void LoadCommands()
        {
            _cmdManager.AddCommand("btnAddInspection", new AddProcessCommand(this.btnAddProcess, this.grdProcesses, this));
            _cmdManager.AddCommand("DeleteProcess", new DeleteProcessesCommand(btnRemoveProcess, grdProcesses, this));
            _cmdManager.AddCommand("AddProcessPackage", new AddProcessPackageCommand(btnAddProcessPackage, grdProcesses, this));
            _cmdManager.AddCommand("MoveProcessStepDownCommand", new MoveProcessStepDownCommand(this.btnDown, this.grdProcesses, this));
            _cmdManager.AddCommand("MoveProcessStepUpCommand", new MoveProcessStepUpCommand(this.btnUp, this.grdProcesses, this));
            _cmdManager.AddCommand("CopyInspectionCommand", new CopyProcessCommand(this.btnCopy, this.grdProcesses, this));
            _cmdManager.AddCommand("PrintCommand", new PrintProcessCommand(this.btnPrint, this.grdProcesses, this));
            
            if(!IsInRework)
            {
                _cmdManager.AddCommand("SetCurrentProcessCommand", new SetCurrentProcessCommand(this.btnSetCurrent, this.grdProcesses, this));
                _cmdManager.AddCommand("EditProcessCommand", new EditProcessCommand(this.btnEdit, this.grdProcesses, this));
            }
            else
            {
                btnSetCurrent.Visible = false;
                btnEdit.Visible = false;
            }
        }

        private void ResyncProcessStepOrder()
        {
            var processes   = _processes.OrderBy(pi => pi.StepOrder).ToList();
            var index       = 1;

            foreach(var process in processes)
            {
                process.StepOrder = index;
                index++;
            }

            grdProcesses.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
            this._cmdManager.RefreshAll();
        }

        private ProcessItem AddProcessItem(OrdersDataSet.OrderProcessesRow orderProcess)
        {
            var pi = new ProcessItem(orderProcess);

            using(var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter())
                pi.IsPaperless = ta.GetIsPaperless(orderProcess.ProcessID).GetValueOrDefault(true);

            _processes.Add(pi);
            return pi;
        }

        private void DisposeMe()
        {
            if(_processes != null)
                _processes.Clear();

            if(this._displayTips != null)
                this._displayTips.Dispose();

            this._displayTips = null;

            if(_cmdManager != null)
                _cmdManager.Dispose();

            _cmdManager = null;
        }

        private Infragistics.Win.Appearance GetCellBackgroundAppearanceImage(string imageName)
        {
            var key = imageName + "_Image";

            if (!this.grdProcesses.DisplayLayout.Appearances.Exists(key))
            {
                var app = this.grdProcesses.DisplayLayout.Appearances.Add(key);
                app.Image = imageName;
                app.ImageAlpha = Alpha.Opaque;
                return app;
            }

            return this.grdProcesses.DisplayLayout.Appearances[key];
        }

        private void EditableDateCell(UltraGridColumn col, bool enable)
        {
            if (enable)
            {
                col.CellActivation = Activation.AllowEdit;
                col.CellClickAction = CellClickAction.Edit;
                col.Nullable = Infragistics.Win.UltraWinGrid.Nullable.EmptyString;
                col.MaskInput = "mm/dd/yyyy";
                col.MaskDataMode = MaskMode.IncludeBoth;
                col.MaskDisplayMode = MaskMode.IncludeBoth;
                col.MaskClipMode = MaskMode.IncludeLiterals;
                col.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownCalendar;
            }
            else
            {
                col.CellActivation = Activation.NoEdit;
                col.CellClickAction = CellClickAction.RowSelect;
                col.MaskInput = "mm/dd/yyyy";
                col.MaskDataMode = MaskMode.IncludeBoth;
                col.MaskDisplayMode = MaskMode.IncludeBoth;
                col.MaskClipMode = MaskMode.IncludeLiterals;
            }
        }

        private void EditableDateTimeCell(UltraGridColumn col, bool enable)
        {
            if (enable)
            {
                col.CellActivation = Activation.AllowEdit;
                col.CellClickAction = CellClickAction.Edit;
                col.Nullable = Infragistics.Win.UltraWinGrid.Nullable.EmptyString;
            }
            else
            {
                col.CellActivation = Activation.NoEdit;
                col.CellClickAction = CellClickAction.RowSelect;
            }

            col.MaskInput = "{date} {time}";
            col.MaskDataMode = MaskMode.IncludeBoth;
            col.MaskDisplayMode = MaskMode.IncludeBoth;
            col.MaskClipMode = MaskMode.IncludeLiterals;
            col.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTimeWithSpin;
        }

        #endregion

        #region Events

        private void grdProcesses_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                grdProcesses.AfterColPosChanged -= grdProcesses_AfterColPosChanged;
                grdProcesses.AfterSortChange -= grdProcesses_AfterSortChange;

                var hideCapacity = !ApplicationSettings.Current.UseLoadCapacity;
                var hideEstCompleted = !ApplicationSettings.Current.SchedulingEnabled || this.IsInRework;
                var hideProcessedPartCount = !ApplicationSettings.Current.AllowPartialProcessLoads || this.IsInRework;

                var excludeCapacityFromChooser = hideCapacity ? ExcludeFromColumnChooser.True : ExcludeFromColumnChooser.Default;

                var currencyFormat = string.Format("$#,##0.{0}",
                    string.Concat(Enumerable.Repeat('#', ApplicationSettings.Current.PriceDecimalPlaces)));

                var band = e.Layout.Bands[0];

                foreach(var col in band.Columns)
                {
                    col.Header.Appearance.TextHAlign = HAlign.Center;
                    col.CellClickAction              = CellClickAction.RowSelect;
                    col.CellActivation               = Activation.ActivateOnly;
                }

                band.Columns["EstCompletedDate"].Hidden = hideEstCompleted;
                band.Columns["OrderProcess"].Hidden     = true;
                band.Columns["IsOriginal"].Hidden       = true;
                band.Columns["IsCurrent"].Hidden        = true;
                band.Columns["Completed"].Hidden        = true;
                band.Columns["IsPaperless"].Hidden      = true;
                band.Columns["ProcessedPartCount"].Hidden = hideProcessedPartCount;

                band.Columns["EstCompletedDate"].ExcludeFromColumnChooser = hideEstCompleted ? ExcludeFromColumnChooser.True : ExcludeFromColumnChooser.Default;
                band.Columns["OrderProcess"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                band.Columns["IsOriginal"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                band.Columns["IsCurrent"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                band.Columns["Completed"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                band.Columns["IsPaperless"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                band.Columns["ProcessedPartCount"].ExcludeFromColumnChooser = hideProcessedPartCount ? ExcludeFromColumnChooser.True : ExcludeFromColumnChooser.Default;

                band.Columns["Process"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;

                band.Columns["EstCompletedDate"].Header.Caption = "Process By";
                band.Columns["StartDate"].Header.Caption = "Started";
                band.Columns["EndDate"].Header.Caption = "Ended";
                band.Columns["StepOrder"].Header.Caption = "Order";
                band.Columns["ProcessedPartCount"].Header.Caption = "Parts";
                band.Columns["ProcessDurationMinutes"].Header.Caption = "Duration (Minutes)";
                band.Columns["MaterialCost"].Header.Caption = "Material Cost";
                band.Columns["BurdenCost"].Header.Caption = "Burden Cost";

                band.Columns["LoadCapacityWeight"].Hidden = hideCapacity;
                band.Columns["LoadCapacityWeight"].ExcludeFromColumnChooser = excludeCapacityFromChooser;
                band.Columns["LoadCapacityWeight"].Header.Caption = "Capacity (lbs.)";
                band.Columns["LoadCapacityWeight"].Header.ToolTipText = "Load Capacity in pounds";
                band.Columns["LoadCapacityWeight"].MaskInput = string.Format("nnnn.{0}",
                    string.Concat(Enumerable.Repeat("n", ApplicationSettings.Current.WeightDecimalPlaces)));

                band.Columns["LoadCapacityQuantity"].Hidden = hideCapacity;
                band.Columns["LoadCapacityQuantity"].ExcludeFromColumnChooser = excludeCapacityFromChooser;
                band.Columns["LoadCapacityQuantity"].Header.Caption = "Capacity (pieces)";
                band.Columns["LoadCapacityQuantity"].Header.ToolTipText = "Load Capacity in pieces";
                band.Columns["LoadCapacityVariance"].Header.Caption = "Capacity Variance";
                band.Columns["LoadCapacityVariance"].Header.ToolTipText = "Load Capacity Variance";

                band.Columns["LoadCapacityVariance"].Hidden = hideCapacity;
                band.Columns["LoadCapacityVariance"].ExcludeFromColumnChooser = excludeCapacityFromChooser;
                band.Columns["LoadCapacityVariance"].Format = "#%";


                band.Columns["Amount"].Format = currencyFormat;
                band.Columns["MaterialCost"].Format = currencyFormat;
                band.Columns["BurdenCost"].Format = currencyFormat;

                if(DisplayCOCColumn)
                {
                    band.Columns["COC"].CellActivation = Activation.AllowEdit;
                    band.Columns["COC"].CellClickAction = CellClickAction.Edit;
                }

                band.Columns["COC"].Hidden = !DisplayCOCColumn;
                band.Columns["COC"].ExcludeFromColumnChooser = DisplayCOCColumn ? ExcludeFromColumnChooser.Default : ExcludeFromColumnChooser.True;

                if (!IsInRework)
                {
                    this.EditableDateCell(band.Columns["StartDate"], true);
                    this.EditableDateCell(band.Columns["EndDate"], true);

                    if (ApplicationSettings.Current.SchedulerType == SchedulerType.ProcessLeadTimeHour)
                    {
                        EditableDateTimeCell(band.Columns["EstCompletedDate"], true);
                    }
                    else
                    {
                        EditableDateCell(band.Columns["EstCompletedDate"], true);
                    }

                    band.Columns["ProcessedPartCount"].CellActivation = Activation.AllowEdit;
                    band.Columns["ProcessedPartCount"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.IntegerNonNegativeWithSpin;
                    band.Columns["ProcessedPartCount"].CellClickAction = CellClickAction.Edit;
                }

                if (SecurityManager.Current.IsInRole("OrderProcess.EditTime"))
                {
                    band.Columns["ProcessDurationMinutes"].CellActivation = Activation.AllowEdit;
                    band.Columns["ProcessDurationMinutes"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.IntegerNonNegativeWithSpin;
                    band.Columns["ProcessDurationMinutes"].CellClickAction = CellClickAction.Edit;
                }

                band.Columns["ProcessingLine"].Header.Caption = "Processing Line";
                band.Columns["ProcessAlias"].Hidden = true;

                //Reset Sort
                band.SortedColumns.Clear();
                band.Columns["StepOrder"].SortIndicator = SortIndicator.Ascending;

                // Load grid settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(band);

                // Force ProcessingLine to be hidden if it should be disabled.
                if (!ApplicationSettings.Current.MultipleLinesEnabled)
                {
                    band.Columns["ProcessingLine"].Hidden = true;
                    band.Columns["ProcessingLine"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                }
            }
            finally
            {
                grdProcesses.AfterColPosChanged += grdProcesses_AfterColPosChanged;
                grdProcesses.AfterSortChange += grdProcesses_AfterSortChange;
            }
        }

        private void grdProcesses_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            var process = e.Row.ListObject as ProcessItem;

            if (process.IsOriginal || !this.IsInRework) //dont display rework icon if in Order Entry
            {
                e.Row.Appearance.BackColor = Color.LightCyan;
                e.Row.Appearance.ForeColor = Color.Black;
                
                if (process.Completed)
                    e.Row.Cells["StepOrder"].Appearance = GetCellBackgroundAppearanceImage("complete");
                else if (process.IsCurrent)
                    e.Row.Cells["StepOrder"].Appearance = GetCellBackgroundAppearanceImage("current");
                else
                    e.Row.Cells["StepOrder"].Appearance = GetCellBackgroundAppearanceImage("fail");
            }
            else
                e.Row.Cells["StepOrder"].Appearance = GetCellBackgroundAppearanceImage("repair");

            
            if(!process.IsPaperless)
                e.Row.Cells["Process"].Appearance = GetCellBackgroundAppearanceImage("paper");

            //dont let the user set a date if there never was one. DWOS uses this to determine state of order
            if (e.Row.Cells["StartDate"].Value == null || e.Row.Cells["StartDate"].Value.ToString() == "-")
                e.Row.Cells["StartDate"].Activation = Activation.Disabled;
            
            if (e.Row.Cells["EndDate"].Value == null || e.Row.Cells["EndDate"].Value.ToString() == "-")
                e.Row.Cells["EndDate"].Activation = Activation.Disabled;
         
            //only allow value to be changed if already has a value
            e.Row.Cells["ProcessedPartCount"].Activation = e.Row.Cells["ProcessedPartCount"].Value != null && Convert.ToInt32(e.Row.Cells["ProcessedPartCount"].Value) > 0 ? Activation.AllowEdit : Activation.Disabled;
        }

        private void grdProcesses_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            try
            {
                var band = grdProcesses.DisplayLayout.Bands[0];
                var gridSettings = new UltraGridBandSettings();
                gridSettings.RetrieveSettingsFrom(band);

                // Save settings
                _gridSettingsPersistence.SaveSettings(gridSettings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after showing/hiding/moving a column.");
            }
        }

        private void grdProcesses_AfterSortChange(object sender, BandEventArgs e)
        {
            try
            {
                var band = grdProcesses.DisplayLayout.Bands[0];
                var gridSettings = new UltraGridBandSettings();
                gridSettings.RetrieveSettingsFrom(band);

                // Save settings
                _gridSettingsPersistence.SaveSettings(gridSettings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after sorting a column.");
            }
        }

        #endregion

        #region ProcessItem

        private class ProcessItem
        {
            public OrdersDataSet.OrderProcessesRow OrderProcess { get; set; }

            public int StepOrder
            {
                get
                {
                    return OrderProcess != null && OrderProcess.IsValidState() ? OrderProcess.StepOrder : 0;
                }
                set
                {
                    OrderProcess.StepOrder = value;
                }
            }

            public bool IsOriginal 
            {
                get { return OrderProcess != null && OrderProcess.IsValidState() && OrderProcess.OrderID > 0; }
            }

            public bool Completed
            {
                get { return this.OrderProcess != null && this.OrderProcess.IsValidState() && !this.OrderProcess.IsEndDateNull(); }
            }

            public bool IsCurrent
            {
                get { return this.OrderProcess != null && this.OrderProcess.IsValidState() && (!this.OrderProcess.IsStartDateNull() && this.OrderProcess.IsEndDateNull()); }
            }

            public string Process => OrderProcess != null && OrderProcess.IsValidState() && OrderProcess.ProcessAliasSummaryRow != null
                ? OrderProcess.ProcessAliasSummaryRow.ProcessName
                : string.Empty;

            public string ProcessAlias => OrderProcess != null && OrderProcess.IsValidState() && OrderProcess.ProcessAliasSummaryRow != null
                ? OrderProcess.ProcessAliasSummaryRow.Name
                : string.Empty;

            public string Department
            {
                get { return OrderProcess != null && OrderProcess.IsValidState() ? OrderProcess.Department : ""; }
            }

            public string ProcessingLine => OrderProcess?.ProcessingLineRow?.Name;

            public bool COC
            {
                get { return this.OrderProcess != null && this.OrderProcess.IsValidState() && this.OrderProcess.COCData; }
                set { OrderProcess.COCData = value; }
            }
            
            public string StartDate
            {
                get { return OrderProcess != null && OrderProcess.IsValidState() ? OrderProcess.IsStartDateNull() ? "-" : OrderProcess.StartDate.ToShortDateString() : ""; }
                set
                {
                    if (value == null || String.IsNullOrWhiteSpace(value))
                        OrderProcess.SetStartDateNull();
                    else
                        OrderProcess.StartDate = Convert.ToDateTime(value);
                }
            }

            public decimal? Amount
            {
                get
                {
                    if (OrderProcess == null || OrderProcess.IsAmountNull())
                    {
                        return null;
                    }

                    return OrderProcess.Amount;
                }
            }

            public decimal? MaterialCost
            {
                get
                {
                    if (OrderProcess == null || OrderProcess.IsMaterialCostNull())
                    {
                        return null;
                    }

                    return OrderProcess.MaterialCost;
                }
            }

            public decimal? BurdenCost
            {
                get
                {
                    if (OrderProcess == null || OrderProcess.IsBurdenCostNull())
                    {
                        return null;
                    }

                    return OrderProcess.BurdenCost;
                }
            }

            public int? ProcessDurationMinutes
            {
                get
                {
                    if (OrderProcess == null || OrderProcess.IsProcessDurationMinutesNull())
                    {
                        return null;
                    }

                    return OrderProcess.ProcessDurationMinutes;
                }
                set
                {
                    if (OrderProcess == null)
                    {
                        return;
                    }

                    if (value.HasValue)
                    {
                        OrderProcess.ProcessDurationMinutes = value.Value;
                    }
                    else
                    {
                        OrderProcess.SetProcessDurationMinutesNull();
                    }
                }
            }

            public string EndDate
            {
                get { return OrderProcess != null && OrderProcess.IsValidState() ? OrderProcess.IsEndDateNull() ? "-" : OrderProcess.EndDate.ToShortDateString() : ""; }
                set
                {
                    if(value == null || String.IsNullOrWhiteSpace(value))
                        OrderProcess.SetEndDateNull();
                    else
                        OrderProcess.EndDate = Convert.ToDateTime(value);
                }
            }

            public DateTime? EstCompletedDate
            {
                get => (OrderProcess != null && OrderProcess.IsValidState() && !OrderProcess.IsEstEndDateNull())
                    ? OrderProcess.EstEndDate
                    : (DateTime?)null;
                set
                {
                    if (value.HasValue)
                    {
                        OrderProcess.EstEndDate = Convert.ToDateTime(value);
                    }
                    else
                    {
                        OrderProcess.SetEstEndDateNull();
                    }
                }
            }

            public int ProcessedPartCount
            {
                get { return OrderProcess != null && OrderProcess.IsValidState() ? (OrderProcess.IsPartCountNull() ? 0 : OrderProcess.PartCount) : 0; }
                set
                {
                    if(!OrderProcess.IsValidState())
                        return;

                    if (value < 0)
                        OrderProcess.SetPartCountNull();
                    else
                        OrderProcess.PartCount = value;
                }
            }

            public decimal? LoadCapacityWeight
            {
                get
                {
                    if (OrderProcess == null || OrderProcess.IsLoadCapacityWeightNull())
                    {
                        return null;
                    }
                    else
                    {
                        return OrderProcess.LoadCapacityWeight;
                    }
                }
            }

            public int? LoadCapacityQuantity
            {
                get
                {
                    if (OrderProcess == null || OrderProcess.IsLoadCapacityQuantityNull())
                    {
                        return null;
                    }
                    else
                    {
                        return OrderProcess.LoadCapacityQuantity;
                    }
                }
            }

            public decimal? LoadCapacityVariance
            {
                get
                {
                    if (OrderProcess == null || OrderProcess.IsLoadCapacityVarianceNull())
                    {
                        return null;
                    }
                    else
                    {
                        return OrderProcess.LoadCapacityVariance;
                    }
                }
            }

            public ProcessItem(OrdersDataSet.OrderProcessesRow orderProcess)
            {
                this.OrderProcess = orderProcess;
            }

            public bool IsPaperless { get; set; }
        }

        #endregion

        #region Commands

        private class AddProcessCommand : GridCommand
        {
            #region Methods

            public AddProcessCommand(UltraButton tool, UltraGrid listView, OrderProcessEditor processInfo) : base(tool, listView, processInfo) { }

            public override void OnClick()
            {
                try
                {
                    using(var frmProcesses = new ProcessPicker())
                    {
                        frmProcesses.LoadCustomerAliases(_reworkInfo.OrderRow.CustomerID);

                        //display Add Process dialog
                        if(frmProcesses.ShowDialog(_reworkInfo) == DialogResult.OK && frmProcesses.SelectedProcesses.Count > 0)
                        {
                            foreach(var selectedProcess in frmProcesses.SelectedProcesses)
                            {
                                if(selectedProcess.ProcessID > 0)
                                {
                                    _processGrid.UpdateData(); //ensure changes are written to data source

                                    var allProcesses = _reworkInfo._processes.OrderBy(pi => pi.StepOrder).ToArray();
                                    var prevProcess = _processGrid.ActiveRow != null ? _processGrid.ActiveRow.ListObject as ProcessItem : null;

                                    //if the selected process is completed then ensure we can add a new process after it
                                    if(prevProcess != null && prevProcess.Completed && allProcesses.Last() != prevProcess)
                                        prevProcess = allProcesses.LastOrDefault(p => p.Completed); //get last completed process

                                    if(prevProcess == null)
                                        prevProcess = allProcesses.LastOrDefault(pi => pi.OrderProcess != null);

                                    //remove the order from a batch if needed
                                    RemoveOrderFromBatch(_reworkInfo.OrderRow.OrderID);

                                    //create new data row
                                    var op = _reworkInfo.dsOrders.OrderProcesses.NewRow() as OrdersDataSet.OrderProcessesRow;
                                    op.OrderID = _reworkInfo.OrderRow.OrderID;
                                    op.ProcessID = selectedProcess.ProcessID;
                                    op.Department = selectedProcess.Department;
                                    op.StepOrder = prevProcess == null ? 1 : prevProcess.OrderProcess.StepOrder + 1;
                                    op.ProcessAliasID = selectedProcess.ProcessAliasID;
                                    op.COCData = false;
                                    op.OrderProcessType = (int) OrderProcessType.Rework;

                                    //Ensure process is in dataset
                                    using(var taProcesses = new ProcessAliasSummaryTableAdapter {ClearBeforeFill = false})
                                        taProcesses.FillById(_reworkInfo.dsOrders.ProcessAliasSummary, op.ProcessAliasID);

                                    //add to DB
                                    _reworkInfo.dsOrders.OrderProcesses.Rows.Add(op);
                                    
                                    //add to list view
                                    if(_reworkInfo.AddProcessItem(op) != null)
                                        _reworkInfo.ResyncProcessStepOrder();
                                }
                            }
                        }
                    }
                }
                catch(Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error adding the process to the part.", exc);
                }
            }

            private static void RemoveOrderFromBatch(int orderID)
            {
                try
                {
                    using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter())
                    {
                        var isInBatch = ta.GetInBatch(orderID);

                        if (isInBatch != null && isInBatch != DBNull.Value && Convert.ToBoolean(isInBatch))
                        {
                            _log.Info("In removing order from batch for order {0}".FormatWith(orderID));

                            using (var taBatch = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchOrderTableAdapter() { ClearBeforeFill = false })
                            {
                                MessageBoxUtilities.ShowMessageBoxWarn("Order is in an active batch. The order will be removed from the batch.", "Add Process");
                                taBatch.DeleteOrderFromActiveBatch(orderID);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error removing order '{0}' from active batches.".FormatWith(orderID));
                }
            }

            #endregion
        }

        private class AddProcessPackageCommand : GridCommand
        {
            #region Methods

            public AddProcessPackageCommand(UltraButton tool, UltraGrid listView, OrderProcessEditor processInfo) : base(tool, listView, processInfo) { }

            public override void OnClick()
            {
                try
                {
                    int? processPackageId = null;
                    using (var processPackageManager = new ProcessPackages())
                    {
                        if (processPackageManager.ShowDialog(_reworkInfo) == DialogResult.OK)
                        {
                            processPackageId = processPackageManager.SelectedProcessPackage?.ProcessPackageID;
                        }
                    }

                    if (!processPackageId.HasValue)
                    {
                        return;
                    }

                    _processGrid.UpdateData(); //ensure changes are written to data source

                    var allProcesses = _reworkInfo._processes.OrderBy(pi => pi.StepOrder).ToArray();
                    var prevProcess = _processGrid.ActiveRow != null ? _processGrid.ActiveRow.ListObject as ProcessItem : null;

                    //if the selected process is completed then ensure we can add a new process after it
                    if (prevProcess != null && prevProcess.Completed && allProcesses.Last() != prevProcess)
                        prevProcess = allProcesses.LastOrDefault(p => p.Completed); //get last completed process

                    if (prevProcess == null)
                        prevProcess = allProcesses.LastOrDefault(pi => pi.OrderProcess != null);

                    //remove the order from a batch if needed
                    RemoveOrderFromBatch(_reworkInfo.OrderRow.OrderID);

                    foreach (var process in GetProcesses(processPackageId.Value))
                    {
                        //create new data row
                        var op = _reworkInfo.dsOrders.OrderProcesses.NewRow() as OrdersDataSet.OrderProcessesRow;
                        op.OrderID = _reworkInfo.OrderRow.OrderID;
                        op.ProcessID = process.ProcessId;
                        op.Department = GetDepartmentName(process.ProcessId);
                        op.StepOrder = prevProcess == null ? 1 : prevProcess.OrderProcess.StepOrder + 1;
                        op.ProcessAliasID = process.ProcessAliasId;
                        op.COCData = false;
                        op.OrderProcessType = (int)OrderProcessType.Rework;

                        //Ensure process is in dataset
                        using (var taProcesses = new ProcessAliasSummaryTableAdapter { ClearBeforeFill = false })
                            taProcesses.FillById(_reworkInfo.dsOrders.ProcessAliasSummary, op.ProcessAliasID);

                        //add to DB
                        _reworkInfo.dsOrders.OrderProcesses.Rows.Add(op);

                        //add to list view
                        prevProcess = _reworkInfo.AddProcessItem(op);
                    }

                    _reworkInfo.ResyncProcessStepOrder();
            }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error adding the process to the part.", exc);
                }
            }

            private static IEnumerable<PackagedProcess> GetProcesses(int processPackageId)
            {
                using (var dtProcessPackageProcess = new ProcessPackageDataset.ProcessPackage_ProcessesDataTable())
                {
                    using (var taProcessPackageProcess = new Data.Datasets.ProcessPackageDatasetTableAdapters.ProcessPackage_ProcessesTableAdapter())
                    {
                        taProcessPackageProcess.FillByProcessPackageID(dtProcessPackageProcess, processPackageId);
                    }

                    return dtProcessPackageProcess.OrderBy(p => p.StepOrder)
                        .Select(p => new PackagedProcess(p.ProcessID, p.ProcessAliasID))
                        .ToList();
                }
            }

            private static void RemoveOrderFromBatch(int orderId)
            {
                try
                {
                    using (var ta = new OrderTableAdapter())
                    {
                        var isInBatch = ta.GetInBatch(orderId);

                        if (isInBatch != null && isInBatch != DBNull.Value && Convert.ToBoolean(isInBatch))
                        {
                            _log.Info("In removing order from batch for order {0}".FormatWith(orderId));

                            using (var taBatch = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchOrderTableAdapter() { ClearBeforeFill = false })
                            {
                                MessageBoxUtilities.ShowMessageBoxWarn("Order is in an active batch. The order will be removed from the batch.", "Add Process");
                                taBatch.DeleteOrderFromActiveBatch(orderId);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    _log.Error(exc, "Error removing order '{0}' from active batches.".FormatWith(orderId));
                }
            }

            private static string GetDepartmentName(int processProcessId)
            {
                using (var dtProcess = new OrderProcessingDataSet.ProcessDataTable())
                {
                    using (var taProcess = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter())
                    {
                        taProcess.FillByProcess(dtProcess, processProcessId);
                    }

                    return dtProcess.FirstOrDefault()?.Department;
                }
            }

            #endregion

            #region PackagedProcess

            private class PackagedProcess
            {
                #region Properties

                public int ProcessId { get; }

                public int ProcessAliasId { get; }

                #endregion

                #region Methods

                public PackagedProcess(int processId, int processAliasId)
                {
                    ProcessId = processId;
                    ProcessAliasId = processAliasId;
                }

                #endregion
            }

            #endregion
        }
        
        private class CopyProcessCommand : GridCommand
        {
            #region Properties

            public override bool Enabled
            {
                get { return _processGrid.ActiveRow != null; }
            }

            #endregion

            #region Methods

            public CopyProcessCommand(UltraButton tool, UltraGrid listView, OrderProcessEditor processInfo) : base(tool, listView, processInfo) { }

            public override void OnClick()
            {
                try
                {
                    if(_processGrid.ActiveRow == null)
                        return;

                    _processGrid.UpdateData();

                    var orderProcess    = _processGrid.ActiveRow.ListObject as ProcessItem;
                    int maxStep         = _reworkInfo._processes.Max(o => o.StepOrder);

                    var op              = _reworkInfo.dsOrders.OrderProcesses.NewRow() as OrdersDataSet.OrderProcessesRow;
                    op.OrderID          = orderProcess.OrderProcess.OrderID;
                    op.ProcessID        = orderProcess.OrderProcess.ProcessID;
                    op.Department       = orderProcess.OrderProcess.Department;
                    op.StepOrder        = ++maxStep;
                    op.ProcessAliasID   = orderProcess.OrderProcess.ProcessAliasID;
                    op.COCData          = false;
                    op.OrderProcessType = (int)OrderProcessType.Rework;

                    _reworkInfo.dsOrders.OrderProcesses.Rows.Add(op);
                    _reworkInfo.AddProcessItem(op);
                    _reworkInfo.ResyncProcessStepOrder();
                }
                catch(Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error adding the process to the part.", exc);
                }
            }

            #endregion
        }

        private class DeleteProcessesCommand : GridCommand
        {
            #region Properties

            public override bool Enabled
            {
                get
                {
                    var selectedRows = _processGrid.Selected.Rows;

                    return selectedRows.Count > 0
                        && selectedRows.OfType<UltraGridRow>()
                            .Select(row => row.ListObject as ProcessItem)
                            .All(process => process != null && !process.Completed);
                }
            }

            #endregion

            #region Methods

            public DeleteProcessesCommand(UltraButton tool, UltraGrid listView, OrderProcessEditor processInfo)
                : base(tool, listView, processInfo)
            {
            }

            public override void OnClick()
            {
                try
                {
                    var selectedProcesses = _processGrid.Selected.Rows
                        .OfType<UltraGridRow>()
                        .Select(row => row.ListObject as ProcessItem)
                        .Where(process => process != null && !process.Completed)
                        .ToList();

                    foreach (var process in selectedProcesses)
                    {
                        process.OrderProcess.Delete();
                        _reworkInfo._processes.Remove(process);
                        _reworkInfo.ResyncProcessStepOrder();
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error deleting a process from the order.", exc);
                }
            }

            #endregion
        }

        private class GridCommand : CommandBase
        {
            #region Fields

            protected UltraGrid _processGrid;
            protected OrderProcessEditor _reworkInfo;

            #endregion

            #region Properties

            #endregion

            #region Methods

            protected GridCommand(UltraButton tool, UltraGrid listView, OrderProcessEditor processInfo)
                : base(tool)
            {
                this._reworkInfo = processInfo;
                this._processGrid = listView;
                this._processGrid.AfterSelectChange += listView_AfterSelectChange;
                base.Refresh();
            }
            
            public override void Dispose()
            {
                if(this._processGrid != null)
                    this._processGrid.AfterSelectChange -= listView_AfterSelectChange;

                this._processGrid = null;
                this._reworkInfo = null;

                base.Dispose();
            }

            #endregion

            #region Events

            private void listView_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
            {
                base.Refresh();
            }

            #endregion
        }

        private class MoveProcessStepDownCommand : GridCommand
        {
            #region Properties

            public override bool Enabled
            {
                get
                {
                    //if not last step AND not completed
                    if (_reworkInfo._processes.Count == 0)
                    {
                        return false;
                    }

                    var selectedRows = _processGrid.Selected.Rows;
                    var lastStepOrder = _reworkInfo._processes
                        .Where(p => !p.Completed)
                        .Select(p => p.StepOrder)
                        .DefaultIfEmpty()
                        .Max();

                    return selectedRows.Count > 0
                        && selectedRows.OfType<UltraGridRow>()
                            .Select(row => row.ListObject as ProcessItem)
                            .All(process => process != null && !process.Completed && process.StepOrder < lastStepOrder);
                }
            }

            #endregion

            #region Methods

            public MoveProcessStepDownCommand(UltraButton tool, UltraGrid listView, OrderProcessEditor processInfo) : base(tool, listView, processInfo) { }

            public override void OnClick()
            {
                try
                {
                    var selectedProcesses = _processGrid.Selected.Rows
                        .OfType<UltraGridRow>()
                        .Select(row => row.ListObject as ProcessItem)
                        .Where(process => process != null && !process.Completed)
                        .OrderBy(process => process.StepOrder)
                        .ToList();

                    var firstProcess = selectedProcesses.FirstOrDefault();
                    var lastProcess = selectedProcesses.LastOrDefault();

                    if (firstProcess == null || lastProcess == null)
                    {
                        return;
                    }

                    var nextProcess = _reworkInfo._processes
                        .OrderBy(p => p.StepOrder)
                        .FirstOrDefault(p => p.StepOrder > lastProcess.StepOrder);

                    if (nextProcess == null || nextProcess.Completed)
                    {
                        return;
                    }

                    var firstStepOrder = selectedProcesses.FirstOrDefault().StepOrder;

                    //Suspend the sorting until the StepOrder is reassigned
                    using (new UsingGridLoad(_processGrid))
                    {
                        nextProcess.StepOrder = firstStepOrder;

                        for (var i = 0; i < selectedProcesses.Count; ++i)
                        {
                            selectedProcesses[i].StepOrder = firstStepOrder + i + 1;
                        }
                    }

                    _reworkInfo.ResyncProcessStepOrder();
                }
                catch(Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error moving process up.", exc);
                }
            }

            #endregion
        }

        private class MoveProcessStepUpCommand : GridCommand
        {
            #region Properties

            public override bool Enabled
            {
                get
                {
                    // Not first row AND not completed
                    var selectedRows = _processGrid.Selected.Rows;
                    var firstAvailableStepOrder = _reworkInfo._processes
                        .Where(p => !p.Completed)
                        .Select(p => p.StepOrder)
                        .DefaultIfEmpty()
                        .Min();

                    return selectedRows.Count > 0
                        && selectedRows.OfType<UltraGridRow>()
                            .Select(row => row.ListObject as ProcessItem)
                            .All(process => process != null && !process.Completed && process.StepOrder > firstAvailableStepOrder);
                }
            }

            #endregion

            #region Methods

            public MoveProcessStepUpCommand(UltraButton tool, UltraGrid listView, OrderProcessEditor processInfo) : base(tool, listView, processInfo) { }

            public override void OnClick()
            {
                try
                {
                   var selectedProcesses = _processGrid.Selected.Rows
                       .OfType<UltraGridRow>()
                       .Select(row => row.ListObject as ProcessItem)
                       .Where(process => process != null && !process.Completed)
                       .OrderBy(process => process.StepOrder)
                       .ToList();

                    var firstProcess = selectedProcesses.FirstOrDefault();

                    if (firstProcess == null)
                    {
                        return;
                    }

                    var previousProcess = _reworkInfo._processes
                        .OrderByDescending(p => p.StepOrder)
                        .FirstOrDefault(p => p.StepOrder < firstProcess.StepOrder);

                    if (previousProcess == null || previousProcess.Completed)
                    {
                        return;
                    }

                    var stepOrderStart = previousProcess.StepOrder;
                    var stepOrderEnd = selectedProcesses.LastOrDefault().StepOrder;

                    //Suspend the sorting until the StepOrder is reassigned
                    using (new UsingGridLoad(_processGrid))
                    {
                        for (var i = 0; i < selectedProcesses.Count; ++i)
                        {
                            selectedProcesses[i].StepOrder = stepOrderStart + i;
                        }

                        previousProcess.StepOrder = stepOrderEnd;
                    }

                    _reworkInfo.ResyncProcessStepOrder();
                }
                catch(Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error moving process up.", exc);
                }
            }

            #endregion
        }

        private class SetCurrentProcessCommand : GridCommand
        {
            #region Properties

            public override bool Enabled
            {
                get { return _processGrid.ActiveRow != null && _processGrid.ActiveRow.ListObject is ProcessItem && ((ProcessItem)_processGrid.ActiveRow.ListObject).Completed; }
            }

            #endregion

            #region Methods

            public SetCurrentProcessCommand(UltraButton tool, UltraGrid listView, OrderProcessEditor processInfo) : base(tool, listView, processInfo) { }

            public override void OnClick()
            {
                try
                {
                    OrdersDataSet.OrderRow orderRow = _reworkInfo.OrderRow;

                    if (orderRow != null && _processGrid.ActiveRow != null && _processGrid.ActiveRow.ListObject is ProcessItem && ((ProcessItem)_processGrid.ActiveRow.ListObject).OrderProcess != null)
                    {
                        _log.Info("Setting current process step to order: " + orderRow.OrderID);

                        var orderProcesses  = _reworkInfo._processes.OrderBy(opr => opr.StepOrder).ToList();
                        var currentProcess  = (ProcessItem) _processGrid.ActiveRow.ListObject;
                        bool foundProcess   = false;

                        foreach(var orderProcess in orderProcesses)
                        {
                            if (orderProcess == currentProcess)
                            {
                                currentProcess.OrderProcess.OrderRow.WorkStatus = ApplicationSettings.Current.WorkStatusChangingDepartment; //update to allow order to move to new department
                                foundProcess = true;
                            }

                            if(foundProcess)
                            {
                                orderProcess.OrderProcess.SetStartDateNull();
                                orderProcess.OrderProcess.SetEndDateNull(); //null out end date of all processes after this one

                                //reset all inspections to false
                                orderProcess.OrderProcess.GetPartInspectionRows().ForEach(pi => pi.Status = false);
                            }
                        }

                        _processGrid.Rows.Refresh(RefreshRow.FireInitializeRow);

                    }
                }
                catch(Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error setting the current process of the order.", exc);
                }
            }

            #endregion
        }

        private class EditProcessCommand : GridCommand
        {
            #region Properties

            public override bool Enabled
            {
                get { return _processGrid.ActiveRow != null && _processGrid.ActiveRow.ListObject is ProcessItem && ((ProcessItem)_processGrid.ActiveRow.ListObject).Completed && ((ProcessItem)_processGrid.ActiveRow.ListObject).IsOriginal; }
            }

            #endregion

            #region Methods

            public EditProcessCommand(UltraButton tool, UltraGrid listView, OrderProcessEditor processInfo) : base(tool, listView, processInfo) { }

            public override void OnClick()
            {
                try
                {
                    OrdersDataSet.OrderRow orderRow = _reworkInfo.OrderRow;

                    if (orderRow != null && _processGrid.ActiveRow != null && _processGrid.ActiveRow.ListObject is ProcessItem && ((ProcessItem)_processGrid.ActiveRow.ListObject).OrderProcess != null)
                    {
                        _log.Info("Editing current process: " + orderRow.OrderID);

                        var currentProcess = (ProcessItem)_processGrid.ActiveRow.ListObject;

                        if (currentProcess != null)
                        {
                            var activity = new ProcessingActivity(currentProcess.OrderProcess.OrderID,
                                new ActivityUser(SecurityManager.Current.UserID,
                                    Properties.Settings.Default.CurrentDepartment,
                                    Properties.Settings.Default.CurrentLine))
                            {
                                OrderProcessID = currentProcess.OrderProcess.OrderProcessesID
                            };

                            using (var frm = new OrderProcessing2(activity) { Mode = OrderProcessing2.ProcessingMode.Administrator})
                                frm.ShowDialog(Form.ActiveForm);
                        }
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error setting the current process of the order.", exc);
                }
            }

            #endregion
        }

        private class PrintProcessCommand : GridCommand
        {
            #region Properties

            public override bool Enabled
            {
                get { return _processGrid.ActiveRow != null && _processGrid.ActiveRow.ListObject is ProcessItem && !((ProcessItem)_processGrid.ActiveRow.ListObject).IsPaperless; }
            }

            #endregion

            #region Methods

            public PrintProcessCommand(UltraButton tool, UltraGrid listView, OrderProcessEditor processInfo) : base(tool, listView, processInfo) { }

            public override void OnClick()
            {
                try
                {
                    OrdersDataSet.OrderRow orderRow = _reworkInfo.OrderRow;

                    if (orderRow != null && _processGrid.ActiveRow != null && _processGrid.ActiveRow.ListObject is ProcessItem && ((ProcessItem)_processGrid.ActiveRow.ListObject).OrderProcess != null)
                    {
                        var currentProcess = (ProcessItem)_processGrid.ActiveRow.ListObject;

                        if (currentProcess != null)
                        {
                            if(currentProcess.OrderProcess.OrderProcessesID < 1)
                            {
                                MessageBoxUtilities.ShowMessageBoxWarn("Unable to print until changes have been saved.", "Unsaved Changes", "Save any changes to the order before printing to ensure data is consistent.");
                            }
                            else
                            {
                                _log.Debug("Printing process sheet: " + currentProcess.OrderProcess.OrderProcessesID);

                                var report = new DWOS.Reports.OrderProcessSheetReport() {OrderProcessId = currentProcess.OrderProcess.OrderProcessesID};
                                report.DisplayReport();
                                //DWOS.Reports.ReportExtensions.PrintReport(report);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error setting the current process of the order.", exc);
                }
            }

            #endregion
        }

        #endregion
    }
}