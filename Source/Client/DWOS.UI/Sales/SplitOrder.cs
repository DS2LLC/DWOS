using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DWOS.Data.Datasets;
using NLog;
using DWOS.UI.Utilities;
using DWOS.Data.Order;

namespace DWOS.UI.Admin
{
    public partial class SplitOrder : Form
    {
        #region Fields

        private readonly int _total;
        private int _numSplitCurrentValue = 2;
        private SummarySettings _partSummaryTotal;
        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("SplitOrder", new UltraGridBandSettings());

        #endregion

        #region Properties

        public int ReasonCode
        {
            get { return cboReasonCode.SelectedItem == null ? 1 : Convert.ToInt32(cboReasonCode.SelectedItem.DataValue); }
        }

        public bool PrintTravelers
        {
            get { return chkPrint.Checked; }
            set { chkPrint.Checked = value; }
        }

        public BindingList<SplitOrderInfo> SplitOrders { get; } = new BindingList<SplitOrderInfo>();

        #endregion

        #region Methods

        public SplitOrder(int totalQty, int workOrder)
        {
            this.InitializeComponent();

            this.txtTotalParts.Text = totalQty.ToString();
            this._total = totalQty;
            this.numSplits.MaxValue = this._total;
            int splitQty = this._total / 2;

            this.txtWorkOrder.Text = workOrder.ToString();

            //Add the original WO
            this.SplitOrders.Add(new SplitOrderInfo() { Order = workOrder.ToString(), PartQty = splitQty, IsOriginalOrder = true });

            //Add the first split
            this.SplitOrders.Add(new SplitOrderInfo() { Order = "New", PartQty = (this._total - splitQty) });

            grdOrders.DataSource = this.SplitOrders;
        }

        private void AddNewSplit()
        {
            //Can have as many splits as there are parts
            if(grdOrders.Rows.Count < this._total)
                 this.SplitOrders.Add(new SplitOrderInfo() { Order = "New", PartQty = 1 });

            this.ValidateSplitTotals();
        }

        private void DeleteSplit()
        {
            //Remove the last split
            if (this.SplitOrders.Count > 2)
            {
                this.SplitOrders.RemoveAt(this.SplitOrders.Count - 1);
                this.ValidateSplitTotals();
            }
        }

        private void ValidateSplitTotals()
        {
            const string partTotalError = "Sum of split parts must equal totals parts for order.";

            try
            {
                //Force an update as cell could be in edit mode and not reflecting current value
                grdOrders.UpdateData();

                var partCount = SplitOrders
                    .Select(s => (long)s.PartQty)
                    .DefaultIfEmpty()
                    .Sum();

                //Check against total for splits
                if (partCount != _total)
                {
                    ShowValidationError(partTotalError);
                }
                else
                {
                    ClearValidationError();
                }
            }
            catch (ArithmeticException arithmeticException)
            {
                LogManager.GetCurrentClassLogger()
                    .Warn(arithmeticException, "Overflow occurred");

                ShowValidationError(partTotalError);
            }
            catch (Exception exc)
            {
                string errorMsg = "Error validating split totals.";
                LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
                ShowValidationError(errorMsg);
            }
        }

        private void ShowValidationError(string partTotalError)
        {
            FormatSummary(_partSummaryTotal, false, System.Drawing.Color.Red);
            _partSummaryTotal.ToolTipText = partTotalError;
            btnOK.Enabled = false;
        }

        private void ClearValidationError()
        {
            FormatSummary(_partSummaryTotal, false, System.Drawing.Color.Black);
            _partSummaryTotal.ToolTipText = string.Empty;
            btnOK.Enabled = true;
        }

        private void FormatSummary(SummarySettings ss, bool currency, System.Drawing.Color? color = null)
        {
            ss.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
            ss.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
            ss.Appearance.ForeColor = color ?? System.Drawing.Color.Black;
            ss.Appearance.TextHAlign = HAlign.Right;

            if (currency)
                ss.DisplayFormat = "{0:C}";
            else
                ss.DisplayFormat = "{0:N0}";
        }

        #endregion

        #region Events
        
        private void SplitOrder_Load(object sender, EventArgs e)
        {
            using (var ta = new Data.Datasets.ListsDataSetTableAdapters.d_OrderChangeReasonTableAdapter())
            {
                var changeReasons = ta.GetDataByChangeType((int)OrderChangeType.Split);

                cboReasonCode.DataSource = changeReasons.DefaultView;
                cboReasonCode.DisplayMember = changeReasons.NameColumn.ColumnName;
                cboReasonCode.ValueMember = changeReasons.OrderChangeReasonIDColumn.ColumnName;

                if (cboReasonCode.Items.Count > 0)
                    cboReasonCode.SelectedIndex = 0;
            }
        }

        private void numSplits_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                //User can either click to increment or can put in a specific number
                int newValue = Convert.ToInt32(this.numSplits.Value);

                //Check against last value to determine if its a split increase or decrease
                if (newValue > _numSplitCurrentValue)
                {
                    int splitsToAdd = (newValue - _numSplitCurrentValue);
                    for (int x = 0; x < splitsToAdd; x++)
                        this.AddNewSplit();
                }
                else if (newValue < _numSplitCurrentValue)
                {
                    int splitsToRemove = (_numSplitCurrentValue - newValue);
                    for (int x = 0; x < splitsToRemove; x++)
                        this.DeleteSplit();
                }

                //Retain the last value
                _numSplitCurrentValue = newValue;
            }
            catch (Exception exc)
            {
                string errorMsg = "Error changing split value.";
                LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
            }
        }

        private void grdOrders_CellChange(object sender, CellEventArgs e)
        {
            var cellEditor = e.Cell.EditorResolved;

            if(cellEditor != null && cellEditor.IsValid && cellEditor.Value != DBNull.Value)
                this.ValidateSplitTotals();
        }

        private void grdOrders_AfterExitEditMode(object sender, EventArgs e)
        {
            this.ValidateSplitTotals();
        }

        private void grdOrders_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                grdOrders.AfterColPosChanged -= grdOrders_AfterColPosChanged;
                grdOrders.AfterSortChange -= grdOrders_AfterSortChange;

                //Order columns and add user friendly column names
                grdOrders.DisplayLayout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
                grdOrders.DisplayLayout.Bands[0].Columns[0].Header.Caption = "Order";
                grdOrders.DisplayLayout.Bands[0].Columns[0].Header.VisiblePosition = 0;
                grdOrders.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Parts";
                grdOrders.DisplayLayout.Bands[0].Columns[1].Header.VisiblePosition = 1;
                grdOrders.DisplayLayout.Bands[0].Columns[1].MinValue = 1;
                grdOrders.DisplayLayout.Bands[0].Columns[1].Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
                grdOrders.DisplayLayout.Bands[0].Columns[1].CellAppearance.TextHAlign = HAlign.Right;
                grdOrders.DisplayLayout.Bands[0].Columns[1].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.IntegerPositiveWithSpin;
                grdOrders.DisplayLayout.Bands[0].Columns[1].CellActivation = Activation.AllowEdit;
                grdOrders.DisplayLayout.Bands[0].Columns[1].CellClickAction = CellClickAction.Edit;
                grdOrders.DisplayLayout.Bands[0].Columns[2].Hidden = true;  //IsOriginalOrder column

                //Don't allow editing on the first column
                grdOrders.DisplayLayout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.Disabled;

                //Summary row
                SummarySettings ss = this.grdOrders.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Count, this.grdOrders.DisplayLayout.Bands[0].Columns["Order"]);
                this.FormatSummary(ss, false);
                _partSummaryTotal = this.grdOrders.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Sum, this.grdOrders.DisplayLayout.Bands[0].Columns["PartQty"]);
                this.FormatSummary(_partSummaryTotal, false);

                this.grdOrders.DisplayLayout.Bands[0].SummaryFooterCaption = "Split Totals:";
                e.Layout.Override.SummaryFooterCaptionAppearance.FontData.Bold = DefaultableBoolean.True;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(grdOrders.DisplayLayout.Bands[0]);
            }
            catch (Exception exc)
            {
                string errorMsg = "Error inititalizing split order grid.";
                LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
            }
            finally
            {
                grdOrders.AfterColPosChanged += grdOrders_AfterColPosChanged;
                grdOrders.AfterSortChange += grdOrders_AfterSortChange;
            }
        }

        private void grdOrders_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdOrders.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position in grid.");
            }
        }

        private void grdOrders_AfterSortChange(object sender, BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdOrders.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column sort in grid.");
            }
        }

        #endregion
    }
}