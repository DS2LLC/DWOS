using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Order;
using DWOS.UI.Admin;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DWOS.UI.Sales
{
    public partial class QuoteProcessWidget : UserControl, IQuoteProcessWidget
    {
        #region Fields

        private ProcessPicker _processPicker;
        private bool _editable;

        private const string COLUMN_ALIAS = "Alias";
        private const string COLUMN_PROCESS = "Process";
        private const string COLUMN_QUOTE_PART_PROCESS_ID = "QuotePartProcessID";
        private const string COLUMN_STEP_ORDER = "StepOrder";
        private const string COLUMN_ACTIVE = "Active";
        private const string COLUMN_SHOW = "Show";
        private const string GRID_COLUMN_INACTIVE = "Inactive";

        public event EventHandler<PricePointChangedEventArgs> PricePointChanged;
        public event EventHandler PriceBucketsChanged;

        private DataTable _dtProcessData;
        private IEnumerable<PricePointData> _pricePoints;
        private IPriceUnitPersistence _priceUnitPersistence;

        /// <summary>
        /// If true, syncs price point data when calling <see cref="SaveRow"/>.
        /// </summary>
        /// <remarks>
        /// Added in order to fix a bug where this control could save
        /// incomplete price point data if the first process has a blank
        /// price point.
        /// 
        /// Due to how DataTables work, deleting part process will delete
        /// pricing info regardless of this field's value.
        /// </remarks>
        private bool _syncPricePointData;

        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("Quote_Process", new UltraGridBandSettings());

        #endregion

        #region Methods

        public QuoteProcessWidget()
        {
            InitializeComponent();
        }

        private void AddNewProcess()
        {
            if (CurrentRecord == null)
            {
                return;
            }

            if (_processPicker == null)
            {
                _processPicker = new ProcessPicker(true);
            }

            _processPicker.LoadCustomerAliases(CurrentRecord.QuoteRow.CustomerID);

            if (_processPicker.ShowDialog() == DialogResult.OK)
            {
                _syncPricePointData = true;

                foreach (var process in _processPicker.SelectedProcesses)
                {
                    var nextStepOrder = this.GetNextProcessStepOrder(CurrentRecord);

                    var partProcessRow = this.Dataset.QuotePart_Process.NewQuotePart_ProcessRow();
                    partProcessRow.QuotePartRow = CurrentRecord;
                    partProcessRow.ProcessID = process.ProcessID; // Process may not be loaded
                    partProcessRow.StepOrder = nextStepOrder;
                    partProcessRow.ProcessAliasID = process.ProcessAliasID;
                    partProcessRow.ShowOnQuote = true;

                    this.Dataset.QuotePart_Process.AddQuotePart_ProcessRow(partProcessRow);

                    var defaultPrice = partProcessRow.ProcessRow.IsPriceNull() ?
                        0M :
                        partProcessRow.ProcessRow.Price;

                    _dtProcessData.Rows.Add(NewTempDataRow(partProcessRow, defaultPrice));
                }
            }
        }

        private int GetNextProcessStepOrder(QuoteDataSet.QuotePartRow quotePart)
        {
            var quotePartProcessRows = quotePart.GetQuotePart_ProcessRows();

            if (quotePartProcessRows.Count() > 0)
            {
                return quotePartProcessRows.Max(qpqpr => qpqpr.StepOrder) + 1;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Updates StepOrder for processes based on the current grid order.
        /// </summary>
        private void UpdateProcessStepOrders()
        {
            int currentStepOrder = 0;

            foreach (var row in grdPartProcesses.Rows.Where(r => r.IsDataRow))
            {
                var priceData = (row.ListObject as DataRowView).Row;

                if (priceData == null)
                {
                    continue;
                }

                // Fix
                currentStepOrder += 1;
                int stepOrder = (priceData[COLUMN_STEP_ORDER] as int?) ?? 0;

                if (stepOrder != currentStepOrder)
                {
                    priceData[COLUMN_STEP_ORDER] = currentStepOrder;
                }

                // Sync
                int quotePartProcessID = (priceData[COLUMN_QUOTE_PART_PROCESS_ID] as int?) ?? 0;
                var quotePartProcess = Dataset.QuotePart_Process.FindByQuotePartProcessID(quotePartProcessID);

                if (quotePartProcess != null && quotePartProcess.StepOrder != currentStepOrder)
                {
                    quotePartProcess.StepOrder = currentStepOrder;
                }
            }
        }

        /// <summary>
        ///   Determine how the node can be dropped on via drag and drop.
        /// </summary>
        /// <param name="node"> The node. </param>
        /// <returns> </returns>
        private UltraTreeDragAndDropHelper.DropLinePositionEnum QueryValidDropOn(UltraTreeNode node)
        {
            if(node.Selected)
                return UltraTreeDragAndDropHelper.DropLinePositionEnum.None;
            else
                return UltraTreeDragAndDropHelper.DropLinePositionEnum.AboveNode | UltraTreeDragAndDropHelper.DropLinePositionEnum.BelowNode;
        }

        #endregion

        #region Events

        private void btnAddProcess_Click(object sender, EventArgs e)
        {
            try
            {
                this.AddNewProcess();
                this.UpdateButtonEnabledStates();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on add process click.");
            }
        }

        private void btnDeleteProcess_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteCurrentSelection();
                //OnAllPricesChanged();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error deleting nodes.");
            }
        }


        #endregion

        #region IQuoteProcessWidget Members

        public QuoteDataSet Dataset
        {
            get;
            private set;
        }

        public QuoteDataSet.QuotePartRow CurrentRecord
        {
            get;
            private set;
        }

        public bool Editable
        {
            get
            {
                return _editable;
            }
            set
            {
                _editable = value;
                UpdateButtonEnabledStates();

            }
        }
        public void LoadData(QuoteDataSet dataset, bool panelIsEditable)
        {
            Dataset = dataset;
            _editable = panelIsEditable;
            _priceUnitPersistence = new PriceUnitPersistence();
            UpdateButtonEnabledStates();
        }

        public void LoadRow(QuoteDataSet.QuotePartRow quotePart)
        {
            CurrentRecord = quotePart;
            grdPartProcesses.Selected.Rows.Clear();
            grdPartProcesses.DataSource = null;
            _pricePoints = null;
            _dtProcessData = null;
            _syncPricePointData = false;

            if (CurrentRecord != null)
            {
                _pricePoints = GeneratePricePoints(CurrentRecord).OrderBy(p => p);
                _dtProcessData = NewTempTable(_pricePoints);

                foreach (var quotePartProcess in CurrentRecord.GetQuotePart_ProcessRows())
                {
                    _dtProcessData.Rows.Add(NewTempDataRow(quotePartProcess, 0M));
                }
            }

            grdPartProcesses.DataSource = _dtProcessData;
        }

        /// <summary>Saves row data.</summary>
        public void SaveRow()
        {
            // Row data is automatically saved

            if (CurrentRecord == null)
            {
                return;
            }

            // Save process data
            // (This widget automatically changes PartProcess values.)
            foreach (var dataRow in _dtProcessData.Rows.OfType<DataRow>())
            {
                if (dataRow == null)
                {
                    continue;
                }

                int quotePartProcessID = (dataRow[COLUMN_QUOTE_PART_PROCESS_ID] as int?) ?? 0;
                var quotePartProcess = Dataset.QuotePart_Process.FindByQuotePartProcessID(quotePartProcessID);

                //Handle show on quote state changes
                if (quotePartProcess != null)
                {
                    bool showOnQuote = (bool)dataRow[COLUMN_SHOW];
                    quotePartProcess.ShowOnQuote = showOnQuote;
                }
            }

        }

        public void UpdateButtonEnabledStates()
        {
            bool processSelected = grdPartProcesses.Selected.Rows.Count > 0;

            btnAddProcess.Enabled = _editable;
            btnDeleteProcess.Enabled = _editable && processSelected;
        }

        #endregion

        private void grdPartProcesses_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {
                grdPartProcesses.AfterColPosChanged -= grdPartProcesses_AfterColPosChanged;
                grdPartProcesses.AfterSortChange -= grdPartProcesses_AfterSortChange;

                var band = grdPartProcesses.DisplayLayout.Bands[0];

                band.SortedColumns.Add(COLUMN_STEP_ORDER, false);

                band.Columns[COLUMN_STEP_ORDER].Header.Caption = "Step Order";
                band.Columns[COLUMN_STEP_ORDER].MaxWidth = 80;
                band.Columns[COLUMN_STEP_ORDER].MinWidth = 80;

                band.Columns[COLUMN_QUOTE_PART_PROCESS_ID].Hidden = true;
                band.Columns[COLUMN_ACTIVE].Hidden = true;

                band.Columns[COLUMN_ALIAS].CellActivation = Activation.NoEdit;
                band.Columns[COLUMN_PROCESS].CellActivation = Activation.NoEdit;

                band.Columns[COLUMN_ALIAS].TabStop = false;
                band.Columns[COLUMN_PROCESS].TabStop = false;

                band.Columns[COLUMN_SHOW].CellActivation = Activation.AllowEdit;

                var inactiveImgColumn = band.Columns.Add(GRID_COLUMN_INACTIVE, string.Empty);
                inactiveImgColumn.DataType = typeof(Image);
                inactiveImgColumn.MaxWidth = 28;
                inactiveImgColumn.MinWidth = 28;
                inactiveImgColumn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                inactiveImgColumn.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                inactiveImgColumn.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                inactiveImgColumn.TabStop = false;

                band.Columns[COLUMN_STEP_ORDER].Header.VisiblePosition = 0;
                inactiveImgColumn.Header.VisiblePosition = 1;
                band.Columns[COLUMN_ALIAS].Header.VisiblePosition = 2;
                band.Columns[COLUMN_PROCESS].Header.VisiblePosition = 3;
                band.Columns[COLUMN_SHOW].Header.VisiblePosition = 4;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(band);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing grid layout.");
            }
            finally
            {
                grdPartProcesses.AfterColPosChanged += grdPartProcesses_AfterColPosChanged;
                grdPartProcesses.AfterSortChange += grdPartProcesses_AfterSortChange;
            }

        }

        private void grdPartProcesses_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (!e.Row.IsDataRow)
            {
                return;
            }

            if (e.Row.Cells.Exists(GRID_COLUMN_INACTIVE))
            {
                DataRow currentProcess = (e.Row.ListObject as DataRowView)?.Row;

                bool isActive = (currentProcess[COLUMN_ACTIVE] as bool?) ?? false;

                try
                {
                    grdPartProcesses.BeforeCellUpdate -= grdPartProcesses_BeforeCellUpdate;
                    grdPartProcesses.AfterCellUpdate -= grdPartProcesses_AfterCellUpdate;

                    if (isActive)
                    {
                        e.Row.Cells[GRID_COLUMN_INACTIVE].Value = null;
                    }
                    else
                    {
                        e.Row.Cells[GRID_COLUMN_INACTIVE].Value = Properties.Resources.RoundDashRed_32;
                    }
                }
                finally
                {
                    grdPartProcesses.BeforeCellUpdate += grdPartProcesses_BeforeCellUpdate;
                    grdPartProcesses.AfterCellUpdate += grdPartProcesses_AfterCellUpdate;
                }
            }
        }

        private void grdPartProcesses_SelectionDrag(object sender, CancelEventArgs e)
        {
            grdPartProcesses.DoDragDrop(grdPartProcesses.Selected.Rows, DragDropEffects.Move);
        }

        private void grdPartProcesses_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move; // is this necessary?
            var senderGrid = sender as UltraGrid;

            if (senderGrid == null)
            {
                return;
            }

            var pointInGrid = senderGrid.PointToClient(new Point(e.X, e.Y));

            if (pointInGrid.Y < 20)
            {
                grdPartProcesses.ActiveRowScrollRegion.Scroll(RowScrollAction.LineUp);
            }
            else if (pointInGrid.Y > (senderGrid.Height - 20))
            {
                grdPartProcesses.ActiveRowScrollRegion.Scroll(RowScrollAction.LineDown);
            }
        }

        private void grdPartProcesses_DragDrop(object sender, DragEventArgs e)
        {
            var pointInGrid = grdPartProcesses.PointToClient(new Point(e.X, e.Y));
            var elementOver = grdPartProcesses.DisplayLayout.UIElement.ElementFromPoint(pointInGrid);

            if (elementOver == null)
            {
                return;
            }

            var rowOver = elementOver.GetContext(typeof(UltraGridRow), true) as UltraGridRow;

            SelectedRowsCollection selectedRows = null;
            if (rowOver != null)
            {
                selectedRows = e.Data.GetData(typeof(SelectedRowsCollection)) as SelectedRowsCollection;
            }

            if (selectedRows != null)
            {
                foreach (var selectedRow in selectedRows)
                {
                    grdPartProcesses.Rows.Move(selectedRow, rowOver.Index);
                }
            }

            UpdateProcessStepOrders();

            // Refresh sort
            var band = grdPartProcesses.DisplayLayout.Bands[0];
            band.SortedColumns.Clear();
            band.SortedColumns.Add(COLUMN_STEP_ORDER, false);
        }

        private void grdPartProcesses_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            try
            {
                // Check against minimum price
                if (e.Cell.Row.IsDataRow)
                {
                    var matchingTempRow = (e.Cell.Row.ListObject as DataRowView)?.Row;
                    var matchingQuotePartProcess = this.Dataset.QuotePart_Process
                        .FindByQuotePartProcessID(matchingTempRow[COLUMN_QUOTE_PART_PROCESS_ID] as int? ?? 0);
                    var process = matchingQuotePartProcess.ProcessRow;
                    var newValue = (e.NewValue as decimal?) ?? 0M;
                    var minimumValue = process.IsMinPriceNull() ? 0M : process.MinPrice;

                    if (newValue < minimumValue)
                    {
                        const string msgFormat = "The price you entered ({0}) is less than the process's minimum price.\n" +
                            "Do you want to override the minimum price of {1}?";

                        string msg = string.Format(msgFormat,
                            newValue.ToString(OrderPrice.CurrencyFormatString),
                            minimumValue.ToString(OrderPrice.CurrencyFormatString));

                        e.Cancel = MessageBoxUtilities.ShowMessageBoxYesOrNo(msg, "Quote Manager") != DialogResult.Yes;
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error before updating per-process price.");
            }
        }

        private void grdPartProcesses_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            try
            {
                _syncPricePointData = true;
                var columnKey = e.Cell.Column.Key;

                foreach (var pricePoint in _pricePoints)
                {
                    if (TempTableColumnName(pricePoint) == columnKey)
                    {
                        //OnPriceChanged(pricePoint);
                        break;
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating per-process price.");
            }
        }

        private void grdPartProcesses_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdPartProcesses.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position in grid.");
            }
        }

        private void grdPartProcesses_AfterSortChange(object sender, BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdPartProcesses.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column sort in grid.");
            }
        }

        private static string TempTableColumnName(PricePointData pricePoint)
        {
            if (pricePoint == null)
            {
                return null;
            }

            return string.Format("Price_{0}", pricePoint.GetHashCode());
        }

        private string GetCaption(PricePointData pricePoint)
        {
            string caption;

            var priceByType = OrderPrice.GetPriceByType(pricePoint.PriceUnit);
            string displayText = _priceUnitPersistence.GetDisplayText(pricePoint.PriceUnit);

            if (priceByType == PriceByType.Quantity)
            {
                // Assumption - if MinQuantity is not present, neither is MaxQuantity
                if (pricePoint.MinQuantity.HasValue && pricePoint.MaxQuantity.HasValue)
                {
                    caption = $"{displayText}({pricePoint.MinQuantity}-{pricePoint.MaxQuantity})";
                }
                else if (pricePoint.MinQuantity.HasValue)
                {
                    caption = $"{displayText} ({pricePoint.MinQuantity}+)";
                }
                else
                {
                    caption = displayText;
                }
            }
            else if (priceByType == PriceByType.Weight)
            {
                if (pricePoint.MinWeight.HasValue && pricePoint.MaxWeight.HasValue)
                {
                    caption = $"{displayText}({pricePoint.MinWeight}-{pricePoint.MaxWeight} lbs.)";
                }
                else if (pricePoint.MinWeight.HasValue)
                {
                    caption = $"{displayText} ({pricePoint.MinWeight}+ lbs.)";
                }
                else
                {
                    caption = displayText;
                }
            }
            else
            {
                caption = displayText;
            }

            return caption;
        }

        private DataRow NewTempDataRow(QuoteDataSet.QuotePart_ProcessRow quotePartProcess, decimal defaultPrice)
        {
            DataRow newRow = _dtProcessData.NewRow();
            newRow[COLUMN_ALIAS] = quotePartProcess.ProcessAliasRow?.Name ?? quotePartProcess.QuotePartProcessID.ToString();

            if (quotePartProcess.ProcessRow == null || quotePartProcess.ProcessRow.IsProcessNameNull())
            {
                newRow[COLUMN_PROCESS] = quotePartProcess.ProcessID;
            }
            else
            {
                newRow[COLUMN_PROCESS] = quotePartProcess.ProcessRow.ProcessName;
            }

            newRow[COLUMN_QUOTE_PART_PROCESS_ID] = quotePartProcess.QuotePartProcessID.ToString();
            newRow[COLUMN_STEP_ORDER] = quotePartProcess.StepOrder;
            newRow[COLUMN_ACTIVE] = quotePartProcess.ProcessRow?.Active ?? false;
            newRow[COLUMN_SHOW] = quotePartProcess.IsShowOnQuoteNull() ? true : quotePartProcess.ShowOnQuote;

            //foreach (var pricePoint in _pricePoints)
            //{
            //    // Find value
            //    var matchingRow = quotePartProcess
            //        .GetQuotePartProcessPriceRows()
            //        .FirstOrDefault(row => pricePoint.Matches(row));

            //    newRow[TempTableColumnName(pricePoint)] = matchingRow?.Amount ?? defaultPrice;
            //}

            return newRow;
        }

        private IEnumerable<PricePointData> GeneratePricePoints(QuoteDataSet.QuotePartRow currentRecord)
        {
            OrdersDataSet.PricePointDataTable dtPricePoint = null;
            OrdersDataSet.PricePointDetailDataTable dtPricePointDetail = null;

            try
            {
                bool hasExistingProcesses = currentRecord.GetQuotePart_ProcessRows().Length > 0;
                bool hasExistingProcessPricing = false;

                if (hasExistingProcesses)
                {
                    hasExistingProcessPricing = currentRecord
                        .GetQuotePart_ProcessRows()
                        .Any(row => row.GetQuotePartProcessPriceRows().Length > 0);
                }

                var pricePoints = new List<PricePointData>();

                if (hasExistingProcessPricing)
                {
                    var pointsDictionary = new Dictionary<int, PricePointData>();

                    foreach (var process in currentRecord.GetQuotePart_ProcessRows())
                    {
                        foreach (var price in process.GetQuotePartProcessPriceRows())
                        {
                            var pricePointData = PricePointData.From(price);
                            pointsDictionary[pricePointData.GetHashCode()] = pricePointData;
                        }
                    }

                    pricePoints.AddRange(pointsDictionary.Values);

                    if (pricePoints.All(point => OrderPrice.GetPriceByType(point.PriceUnit) == PriceByType.Quantity))
                    {
                        // Use weight-based price points from either the customer defaults or the system defaults
                        var customerWeightDefaults = GenerateFromCustomerDefaults(currentRecord.QuoteRow.CustomerID)
                            ?.Where(point => OrderPrice.GetPriceByType(point.PriceUnit) == PriceByType.Weight)
                            .ToList();

                        if (customerWeightDefaults != null && customerWeightDefaults.Count > 0)
                        {
                            pricePoints.AddRange(customerWeightDefaults);
                        }
                        else
                        {
                            pricePoints.AddRange(GenerateFromSystemDefaults().Where(point => OrderPrice.GetPriceByType(point.PriceUnit) == PriceByType.Weight));
                        }
                    }
                }
                else if (ApplicationSettings.Current.EnableVolumePricing)
                {
                    var customerDefaults = GenerateFromCustomerDefaults(currentRecord.QuoteRow.CustomerID);

                    if (customerDefaults != null && customerDefaults.Count > 0)
                    {
                        pricePoints.AddRange(customerDefaults);
                    }
                    else
                    {
                        pricePoints.AddRange(GenerateFromSystemDefaults());
                    }
                }
                else
                {
                    // Non-volume pricing
                    var priceByType = (PriceByType)Enum.Parse(typeof(PriceByType), currentRecord.PriceBy);

                    pricePoints.AddRange(_priceUnitPersistence.ActivePriceUnits.Select(PricePointData.Blank));
                }

                return pricePoints;
            }
            finally
            {
                dtPricePoint?.Dispose();
                dtPricePointDetail?.Dispose();
            }
        }

        private List<PricePointData> GenerateFromCustomerDefaults(int customerId)
        {
            var pricePoints = new List<PricePointData>();
            OrdersDataSet.CustomerPricePointDataTable dtCustomerPricePoint = null;
            OrdersDataSet.CustomerPricePointDetailDataTable dtCustomerPricePointDetail = null;

            try
            {
                dtCustomerPricePoint = new OrdersDataSet.CustomerPricePointDataTable();
                using (var taPrice = new Data.Datasets.OrdersDataSetTableAdapters.CustomerPricePointTableAdapter())
                {
                    taPrice.FillByVolume(dtCustomerPricePoint, customerId);
                }

                var pricePointRow = dtCustomerPricePoint.FirstOrDefault();

                if (pricePointRow == null)
                {
                    return null;
                }

                var pricePointId = pricePointRow.CustomerPricePointID;

                dtCustomerPricePointDetail = new OrdersDataSet.CustomerPricePointDetailDataTable();
                using (var taDetail = new Data.Datasets.OrdersDataSetTableAdapters.CustomerPricePointDetailTableAdapter())
                {
                    taDetail.FillByPricePoint(dtCustomerPricePointDetail, pricePointId);
                }

                var activePriceUnits = _priceUnitPersistence
                    .ActivePriceUnits
                    .ToList();

                foreach (var detailRow in dtCustomerPricePointDetail)
                {
                    var pricePoint = PricePointData.From(detailRow);

                    if (activePriceUnits.Contains(pricePoint.PriceUnit))
                    {
                        pricePoints.Add(pricePoint);
                    }
                }
            }
            finally
            {
                dtCustomerPricePoint?.Dispose();
                dtCustomerPricePointDetail?.Dispose();
            }

            if (pricePoints.All(point => OrderPrice.GetPriceByType(point.PriceUnit) == PriceByType.Quantity))
            {
                // Use system-default weight price points
                pricePoints.AddRange(GenerateFromSystemDefaults().Where(point => OrderPrice.GetPriceByType(point.PriceUnit) == PriceByType.Weight));
            }

            return pricePoints;
        }

        private IEnumerable<PricePointData> GenerateFromSystemDefaults()
        {
            // Create from price points - volume defaults
            var pricePoints = new List<PricePointData>();
            var dtPricePoint = new OrdersDataSet.PricePointDataTable();
            using (var taPrice = new Data.Datasets.OrdersDataSetTableAdapters.PricePointTableAdapter())
            {
                taPrice.FillVolumeDefault(dtPricePoint);
            }

            int pricePointID = dtPricePoint.FirstOrDefault()?.PricePointID ?? 0;

            var dtPricePointDetail = new OrdersDataSet.PricePointDetailDataTable();
            using (var taDetail = new Data.Datasets.OrdersDataSetTableAdapters.PricePointDetailTableAdapter())
            {
                taDetail.FillByPricePoint(dtPricePointDetail, pricePointID);
            }

            var activePriceUnits = _priceUnitPersistence
                .ActivePriceUnits
                .ToList();

            foreach (var detailRow in dtPricePointDetail)
            {
                var pricePoint = PricePointData.From(detailRow);

                if (activePriceUnits.Contains(pricePoint.PriceUnit))
                {
                    pricePoints.Add(pricePoint);
                }
            }

            return pricePoints;
        }

        private static DataTable NewTempTable(IEnumerable<PricePointData> pricePoints)
        {
            if (pricePoints == null)
            {
                throw new ArgumentNullException(nameof(pricePoints), "pricePoints cannot be null");
            }

            var _dtProcessData = new DataTable();
            _dtProcessData.Columns.Add(COLUMN_ALIAS, typeof(string));
            _dtProcessData.Columns.Add(COLUMN_PROCESS, typeof(string));
            _dtProcessData.Columns.Add(COLUMN_QUOTE_PART_PROCESS_ID, typeof(int));
            _dtProcessData.Columns.Add(COLUMN_STEP_ORDER, typeof(int));
            _dtProcessData.Columns.Add(COLUMN_ACTIVE, typeof(bool));
            _dtProcessData.Columns.Add(COLUMN_SHOW, typeof(bool));

            //foreach (var pricePoint in pricePoints)
            //{
            //    _dtProcessData.Columns.Add(TempTableColumnName(pricePoint), typeof(decimal));
            //}

            return _dtProcessData;
        }

        private void DeleteCurrentSelection()
        {
            var selectedRows = grdPartProcesses.Selected.Rows;

            if (selectedRows.Count == 0)
            {
                return;
            }

            using (new UsingGridLoad(grdPartProcesses))
            {
                foreach (var row in selectedRows)
                {
                    if (!row.IsDataRow)
                    {
                        continue;
                    }

                    var selectedProcessPrice = (row.ListObject as DataRowView).Row;
                    QuoteDataSet.QuotePart_ProcessRow selectedQuotePartProcess = null;

                    if (selectedProcessPrice != null)
                    {
                        int quotePartProcessID = (selectedProcessPrice[COLUMN_QUOTE_PART_PROCESS_ID] as int?) ?? 0;
                        selectedQuotePartProcess = Dataset.QuotePart_Process.FindByQuotePartProcessID(quotePartProcessID);
                        selectedProcessPrice.Delete();
                    }

                    if (selectedQuotePartProcess != null)
                    {
                        selectedQuotePartProcess.Delete();
                    }
                }
            }

            this.UpdateButtonEnabledStates();
            this.UpdateProcessStepOrders();
        }

        private void grdPartProcesses_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
        {
            UpdateButtonEnabledStates();
        }

    }
}
