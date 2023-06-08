using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using DWOS.Utilities.Validation;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using NLog;
using ValidatorManager = DWOS.Utilities.Validation.ValidatorManager;

namespace DWOS.UI.Sales.Order
{
    public partial class AdvancedSerialNumberWidget : UserControl, ISerialNumberWidget
    {
        #region Fields

        private const int MaxSerialNumberCount = 10000;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly BindingList<SerialNumberInfo> _serialNumberData;
        private DisplayDisabledTooltips _displayDisabledTooltips;
        private bool _readOnly;

        #endregion

        #region Properties

        public OrdersDataSet.OrderRow CurrentOrder { get; private set; }

        public OrdersDataSet Dataset { get; set; }

        private bool CanAddRow => _serialNumberData.Count < MaxSerialNumberCount && !_readOnly;

        #endregion

        #region Methods

        public AdvancedSerialNumberWidget()
        {
            InitializeComponent();
            _serialNumberData = new BindingList<SerialNumberInfo>();
            _displayDisabledTooltips = new DisplayDisabledTooltips(this, ultraToolTipManager);
        }

        private void AddRow()
        {
            if (ReadOnly)
            {
                return;
            }

            var currentValue = _serialNumberData.LastOrDefault()?.Number ?? grdSerialNumbers.ActiveCell?.Text;
            var newRow = grdSerialNumbers.DisplayLayout.Bands[0].AddNew();
            newRow.Cells[0].Value = currentValue?.Increment();
        }

        private void ShowAddRowWarning()
        {
            MessageBoxUtilities.ShowMessageBoxWarn(
                $"Unable to add another serial number - the maximum number of serial numbers for one order is {MaxSerialNumberCount:N0}",
                "Serial Numbers");
        }

        #endregion

        #region Events

        private void grdSerialNumbers_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter)
                {
                    return;
                }

                // is selected last?
                if (grdSerialNumbers.ActiveRow?.HasNextSibling() ?? false)
                {
                    grdSerialNumbers.PerformAction(UltraGridAction.BelowCell);
                }
                else if (CanAddRow)
                {
                    AddRow();
                }
                else
                {
                    ShowAddRowWarning();
                }

                grdSerialNumbers.ActiveCell = grdSerialNumbers.ActiveRow?.Cells[0];
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error handling KeyDown event");
            }
        }

        private void grdSerialNumbers_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            try
            {
                btnDelete.Enabled = !ReadOnly && grdSerialNumbers.Selected.Rows.Count > 0;
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error handling Select Change event.");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (CanAddRow)
                {
                    AddRow();

                    // Select new row
                    grdSerialNumbers.ActiveCell = grdSerialNumbers.Rows.LastOrDefault()?.Cells[0];
                    grdSerialNumbers.PerformAction(UltraGridAction.EnterEditMode);
                }
                else
                {
                    ShowAddRowWarning();
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error adding serial number.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ReadOnly)
                {
                    grdSerialNumbers.DeleteSelectedRows(true);
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error deleting serial numbers.");
            }
        }

        private void grdSerialNumbers_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            try
            {
                if (e.Cell.Row.IsEmptyRow)
                {
                    if (CanAddRow)
                    {
                        AddRow();

                        // Select new row
                        grdSerialNumbers.ActiveCell = grdSerialNumbers.Rows.LastOrDefault()?.Cells[0];
                        grdSerialNumbers.PerformAction(UltraGridAction.EnterEditMode);
                    }
                    else
                    {
                        ShowAddRowWarning();
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error double-clicking cell.");
            }
        }

        private void grdSerialNumbers_SelectionDrag(object sender, CancelEventArgs e)
        {
            try
            {
                if (!ReadOnly)
                {
                    grdSerialNumbers.DoDragDrop(grdSerialNumbers.Selected.Rows, DragDropEffects.Copy);
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error handling selection drag event.");
            }
        }

        private void grdSerialNumbers_DragOver(object sender, DragEventArgs e)
        {
            const int scrollX = 10;

            if (ReadOnly)
            {
                return;
            }

            try
            {
                e.Effect = DragDropEffects.Copy;

                var pointInGrid = grdSerialNumbers.PointToClient(new Point(e.X, e.Y));

                if (pointInGrid.Y < scrollX)
                {
                    grdSerialNumbers.ActiveRowScrollRegion.Scroll(RowScrollAction.LineUp);
                }
                else if (pointInGrid.Y > grdSerialNumbers.Height - scrollX)
                {
                    grdSerialNumbers.ActiveRowScrollRegion.Scroll(RowScrollAction.LineDown);
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error dragging & dropping serial number.");
            }
        }

        private void grdSerialNumbers_DragDrop(object sender, DragEventArgs e)
        {

            try
            {
                grdSerialNumbers.BeforeRowRegionScroll += CancelScroll;
                var selRows = e.Data.GetData(typeof(SelectedRowsCollection)) as SelectedRowsCollection;
                var firstRow = selRows?.OfType<UltraGridRow>().FirstOrDefault();

                if (firstRow == null)
                {
                    _logger.Warn("Could not retrieve row that user dragged from.");
                    return;
                }

                if (ReadOnly)
                {
                    return;
                }

                var uieOver = grdSerialNumbers.DisplayLayout.UIElement.ElementFromPoint(grdSerialNumbers.PointToClient(new Point(e.X, e.Y)));
                var ugrOver = uieOver.GetContext(typeof(UltraGridRow), true) as UltraGridRow;

                // rowCount is used to prevent users from adding more than the max number of serial numbers.
                var rowCount = _serialNumberData.Count;

                while (true)
                {
                    if (rowCount >= MaxSerialNumberCount || !(ugrOver?.IsEmptyRow ?? true))
                    {
                        break;
                    }

                    rowCount++;

                    grdSerialNumbers.DisplayLayout.Bands[0].AddNew();
                    grdSerialNumbers.Update();
                    uieOver = grdSerialNumbers.DisplayLayout.UIElement.ElementFromPoint(grdSerialNumbers.PointToClient(new Point(e.X, e.Y)));
                    var ugrOverTmp = uieOver.GetContext(typeof(UltraGridRow), true) as UltraGridRow;

                    // Towards the end, uieOver might not be a row's UI element
                    // and ugrOverTmp is null. If this happens, iterate once
                    // again because it should work the next time.
                    if (ugrOverTmp != null)
                    {
                        ugrOver = ugrOverTmp;
                    }
                }

                if (ugrOver == null)
                {
                    _logger.Warn("Could not retrieve row that user dragged to.");
                }
                else
                {
                    _logger.Info("Automatically creating new serial numbers.");

                    var currentNumber = firstRow.Cells[0].Value?.ToString();

                    for (var rowIndex = firstRow.Index + 1; rowIndex <= ugrOver.Index; ++rowIndex)
                    {
                        if (!string.IsNullOrEmpty(currentNumber))
                        {
                            currentNumber = currentNumber.Increment();
                        }

                        grdSerialNumbers.Rows[rowIndex].Cells[0].Value = currentNumber;
                    }
                }

                if (rowCount >= MaxSerialNumberCount)
                {
                    ShowAddRowWarning();
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error auto-filling serial numbers.");
            }
            finally
            {
                grdSerialNumbers.BeforeRowRegionScroll -= CancelScroll;
            }
        }

        private void CancelScroll(object sender, BeforeRowRegionScrollEventArgs beforeRowRegionScrollEventArgs)
        {
            beforeRowRegionScrollEventArgs.Cancel = true;
        }

        private void grdSerialNumbers_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                var band = grdSerialNumbers.DisplayLayout.Bands[0];
                var totalSummary = band.Summaries.Add(SummaryType.Count, band.Columns["Number"]);
                totalSummary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
                totalSummary.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                totalSummary.Appearance.ForeColor = Color.Black;
                totalSummary.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error handling InitializeLayout event");
            }
        }

        #endregion

        #region ISerialNumberWidget Members

        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                btnAdd.Enabled = !value;
                btnDelete.Enabled = !value & btnDelete.Enabled;
                grdSerialNumbers.DisplayLayout.Override.AllowUpdate = value ?
                    DefaultableBoolean.False :
                    DefaultableBoolean.True;

                grdSerialNumbers.DisplayLayout.Override.AllowAddNew = value ?
                    AllowAddNew.No :
                    AllowAddNew.Yes;

                grdSerialNumbers.DisplayLayout.Override.AllowDelete = value ?
                    DefaultableBoolean.False :
                    DefaultableBoolean.True;
            }
        }

        public void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new SerialNumberValidator(this), errProvider));
            errProvider.SetIconAlignment(this, ErrorIconAlignment.TopLeft);
        }

        public void LoadData(OrdersDataSet dataset)
        {
            Dataset = dataset;
            grdSerialNumbers.DataSource = _serialNumberData;
        }

        public void LoadOrder(int orderId)
        {
            LoadOrder(Dataset.Order.FindByOrderID(orderId));
        }

        public void LoadOrder(OrdersDataSet.OrderRow row)
        {
            grdSerialNumbers.Selected.Rows.Clear();
            _serialNumberData.Clear();

            CurrentOrder = row;

            if (CurrentOrder == null)
            {
                return;
            }

            var serialNumberRows =
                CurrentOrder.GetOrderSerialNumberRows()
                    .Where(r => r.IsValidState() && r.Active)
                    .OrderBy(r => r.PartOrder)
                    .ToList();

            foreach (var serialNumberRow in serialNumberRows)
            {
                _serialNumberData.Add(new SerialNumberInfo(serialNumberRow));
            }
        }

        public void SaveRow()
        {
            if (CurrentOrder == null || !CurrentOrder.IsValidState())
            {
                return;
            }

            // Mark deleted rows as inactive
            var removalDate = DateTime.Now;
            foreach (var existingRow in CurrentOrder.GetOrderSerialNumberRows().Where(s => s.IsValidState() && s.Active)
            )
            {
                if (_serialNumberData.Any(s => s.Row == existingRow))
                {
                    continue;
                }

                existingRow.Active = false;
                existingRow.DateRemoved = removalDate;
            }

            var partOrder = 0;
            foreach (var serialNumber in _serialNumberData)
            {
                partOrder++;

                var addNewRow = true;
                if (serialNumber.Row != null)
                {
                    if (serialNumber.Row.RowState == DataRowState.Added)
                    {
                        serialNumber.Row.Number = serialNumber.Number;
                        serialNumber.Row.PartOrder = partOrder;
                        addNewRow = false;
                    }
                    else
                    {
                        var currentNumber = serialNumber.Row.IsNumberNull() ? string.Empty : serialNumber.Row.Number;

                        if (currentNumber == serialNumber.Number)
                        {
                            if (serialNumber.Row.PartOrder != partOrder)
                            {
                                serialNumber.Row.PartOrder = partOrder;
                            }

                            addNewRow = false;
                        }
                        else
                        {
                            serialNumber.Row.Active = false;
                            serialNumber.Row.DateRemoved = removalDate;
                        }
                    }
                }

                if (addNewRow && !string.IsNullOrEmpty(serialNumber.Number))
                {
                    // Create new row
                    var newNumberRow = Dataset.OrderSerialNumber.NewOrderSerialNumberRow();
                    newNumberRow.OrderRow = CurrentOrder;
                    newNumberRow.Number = serialNumber.Number;
                    newNumberRow.PartOrder = partOrder;
                    newNumberRow.Active = true;

                    Dataset.OrderSerialNumber.AddOrderSerialNumberRow(newNumberRow);
                    serialNumber.Row = newNumberRow;
                }
            }
        }

        #endregion

        #region SerialNumberInfo

        private class SerialNumberInfo
        {
            public string Number { get; set; }

            public OrdersDataSet.OrderSerialNumberRow Row { get; set; }

            // Constructor required to create serial numbers on frontend
            // ReSharper disable once UnusedMember.Local
            public SerialNumberInfo()
            {
            }

            public SerialNumberInfo(OrdersDataSet.OrderSerialNumberRow row)
            {
                if (row == null)
                {
                    throw new ArgumentNullException(nameof(row));
                }

                Row = row;
                Number = Row.IsNumberNull() ? string.Empty : Row.Number;
            }
        }

        #endregion

        #region SerialNumberValidator

        private class SerialNumberValidator : ControlValidatorBase
        {
            public AdvancedSerialNumberWidget Widget { get; }

            public SerialNumberValidator(AdvancedSerialNumberWidget widget)
                : base(widget)
            {
                if (widget == null)
                {
                    throw new ArgumentNullException(nameof(widget));
                }

                Widget = widget;
            }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                try
                {
                    if (!Widget.Enabled || Widget.CurrentOrder == null || Widget.CurrentOrder.RowState != DataRowState.Added)
                    {
                        e.Cancel = false;
                        FireAfterValidation(true, string.Empty);
                        return;
                    }

                    if (Widget.CurrentOrder.PartQuantity != Widget._serialNumberData.Count(d => !string.IsNullOrEmpty(d.Number)))
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "Must include a serial number for each part in the order.");
                    }
                    else
                    {
                        e.Cancel = false;
                        FireAfterValidation(true, string.Empty);
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error validating serial numbers.");
                }
            }
        }

        #endregion
    }
}