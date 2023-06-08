using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinGrid;
using NLog;

namespace DWOS.UI.Admin.OrderReset
{
    public partial class OrderSelection: UserControl, IOrderResetPanel
    {
        #region Fields

        private bool _isDataLoaded;

        private bool _hasDataForClosedOrders;

        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("ResetOrder_OrderSelection", Default);

        #endregion

        #region Properties

        public static UltraGridBandSettings Default =>
            new UltraGridBandSettings
            {
                ColumnSort = new Dictionary<string, UltraGridBandSettings.ColumnSortSettings>
                {
                    { "WO", new UltraGridBandSettings.ColumnSortSettings { IsDescending = false, SortIndex = 0, IsGroupByColumn = false } }
                }
            };

        #endregion

        #region Methods

        public OrderSelection()
        {
            this.InitializeComponent();
        }

        private List<int> GetSelectedOrders()
        {
            var selectedOrders = new List<int>();

            foreach(var row in this.grdOrders.DisplayLayout.Rows)
            {
                if(row.Cells["Selected"].Value != null && ((bool)row.Cells["Selected"].Value))
                    selectedOrders.Add(Convert.ToInt32(row.Cells["WO"].Value));
            }

            return selectedOrders;
        }

        private void LoadData()
        {
            this._isDataLoaded = true;
            this._hasDataForClosedOrders = ResetMain.CurrentTask == OrderResetMain.OrderResetTaskType.OpenOrder;

            using(new UsingGridLoad(this.grdOrders))
            {
                if (this._hasDataForClosedOrders)
                {
                    int maxCount = UserSettings.Default.MaxClosedOrders;
                    string status = Properties.Settings.Default.OrderStatusClosed;
                    this.taOrderStatus.FillRecentByStatus(this.dsOrderStatus.OrderStatus, maxCount, status);
                }
                else
                {
                    string status = Properties.Settings.Default.OrderStatusOpen;
                    this.taOrderStatus.FillByStatus(this.dsOrderStatus.OrderStatus, status);
                }

                this.taPriority.Fill(this.dsOrderStatus.Priority);
            }
        }

        private void LoadData(int orderId)
        {
            var status = _hasDataForClosedOrders
                ? Properties.Settings.Default.OrderStatusClosed
                : Properties.Settings.Default.OrderStatusOpen;

            taOrderStatus.FillByOrderAndStatus(dsOrderStatus.OrderStatus, orderId, status);
            taPriority.Fill(dsOrderStatus.Priority);
        }

        private void UpdateFilter()
        {
            this.grdOrders.DisplayLayout.Bands[0].ColumnFilters.ClearAllFilters();

            switch(this.ResetMain.CurrentTask)
            {
                case OrderResetMain.OrderResetTaskType.CloseOrder:
                    //update current locations
                    this.grdOrders.DisplayLayout.Bands[0].ColumnFilters["CurrentLocation"].FilterConditions.Add(FilterComparisionOperator.Equals, ApplicationSettings.Current.DepartmentShipping);

                    //if not sales or shipping then also filters orders that are "In Process"
                    this.grdOrders.DisplayLayout.Bands[0].ColumnFilters["WorkStatus"].FilterConditions.Add(FilterComparisionOperator.Equals, ApplicationSettings.Current.DepartmentShipping);
                    break;
                default:
                    break;
            }
        }
        private void SearchForWorkOrders()
        {
            var orderId = numWorkOrder.Value as int?;

            if (orderId.HasValue)
            {
                LoadData(orderId.Value);
            }
            else
            {
                LoadData();
            }

            RefreshCanNavigate();
        }

        private void RefreshCanNavigate()
        {
            this.CanNavigateToNextPanel = this.GetSelectedOrders().Count > 0;
            OnCanNavigateToNextPanelChange?.Invoke(this, this.CanNavigateToNextPanel);
        }

        #endregion

        #region Events

        private void grdOrders_CellChange(object sender, CellEventArgs e)
        {
            if(this._isDataLoaded)
            {
                this.grdOrders.UpdateData();
                RefreshCanNavigate();
            }
        }

        private void grdOrders_AfterCellUpdate(object sender, CellEventArgs e) {}

        private void btnSearchWorkOrder_Click(object sender, EventArgs e)
        {
            try
            {
                SearchForWorkOrders();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error searching for Work Orders");
            }
        }

        private void numWorkOrder_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SearchForWorkOrders();
                    e.Handled = true;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error searching for Work Orders");
            }
        }


        private void grdOrders_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                grdOrders.AfterColPosChanged -= grdOrders_AfterColPosChanged;
                grdOrders.AfterSortChange -= grdOrders_AfterSortChange;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(grdOrders.DisplayLayout.Bands[0]);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing grid layout.");
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
                LogManager.GetCurrentClassLogger().Error(exc, "Error handling column position change.");
            }
        }

        private void grdOrders_AfterSortChange(object sender, BandEventArgs e)
        {
            try
            {
                // Save settings - this event gets called after enabling/disabling group by.
                // AfterColPosChanged gets fired too, but the grid's settings have not been updated
                // to include the change of sort.
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdOrders.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error handling sort change.");
            }
        }

        #endregion

        #region IOrderResetPanel Members

        public OrderResetMain ResetMain { get; set; }

        public void OnNavigateTo()
        {
            numWorkOrder.Value = null;
            this.LoadData();
            this.UpdateFilter();

            RefreshCanNavigate();
        }

        public void OnNavigateFrom()
        {
            //update selected orders
            this.ResetMain.SelectedOrders = this.GetSelectedOrders();
        }

        public void Finish() { }

        public bool CanNavigateToNextPanel { get; private set; }

        public Action<IOrderResetPanel, bool> OnCanNavigateToNextPanelChange { get; set; }

        #endregion
    }
}