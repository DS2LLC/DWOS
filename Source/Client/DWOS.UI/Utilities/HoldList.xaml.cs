using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DWOS.Data.Datasets;
using DWOS.Shared.Utilities;
using Infragistics.Windows.DataPresenter;
using DWOS.Data;
using System.ComponentModel;
using NLog;
using DWOS.UI.Utilities.Convertors;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Interaction logic for HoldList.xaml
    /// </summary>
    public partial class HoldList : System.Windows.Controls.UserControl
    {
        #region Fields
        
        private RetryPolicy _retryPolicy;
        private DispatcherTimer _refreshTimer = null;
        private Data.Datasets.OrderStatusDataSetTableAdapters.OrderHoldsTableAdapter _taHold;
        private bool _inRefresh = false;
        private bool _isDisposed;


        /// <summary>
        /// Indicates an order to automatically select after refreshing data.
        /// </summary>
        private int? _orderIdToAutoselect;

        #endregion

        #region Properties
        
        private OrderStatusDataSet.OrderHoldsDataTable OrdersHold { get; set; }

        private OrderStatusDataSet.OrderHoldsRow CurrentRow
        {
            get
            {
                if (grdHolds.SelectedItems.Records.Count == 1 && grdHolds.SelectedItems.Records[0].IsDataRecord)
                {
                    var dataRecord = grdHolds.SelectedItems.Records[0] as DataRecord;

                    if (dataRecord == null)
                        return null;

                    var row = (OrderStatusDataSet.OrderHoldsRow)((DataRowView)dataRecord.DataItem).Row;
                    if (row.IsValidState())
                        return row;
                }

                return null;
            }
        }

        #endregion

        #region Methods

        public HoldList()
        {
            InitializeComponent();
        }

        public void SelectWO(int orderId)
        {
            try
            {
                grdHolds.SelectedItemsChanged -= grdHolds_SelectedItemsChanged;

                if (this.grdHolds.Records.Count == 0)
                {
                    return;
                }

                this.grdHolds.ActiveRecord = null;
                this.grdHolds.SelectedItems.Records.Clear();

                if (orderId > 0)
                {
                    var orderRow = this.grdHolds.Records.OfType<DataRecord>()
                        .FirstOrDefault(r => ((OrderStatusDataSet.OrderHoldsRow) ((DataRowView) r.DataItem).Row).OrderID == orderId);

                    if (orderRow == null)
                    {
                        this._orderIdToAutoselect = orderId;
                    }
                    else
                    {
                        this.grdHolds.SelectedItems.Records.Add(orderRow);
                        this.grdHolds.ActiveRecord = orderRow;
                        this.grdHolds.BringRecordIntoView(orderRow);
                        this._orderIdToAutoselect = null;
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error selecting order");
            }
            finally
            {
                grdHolds.SelectedItemsChanged += grdHolds_SelectedItemsChanged;
            }
        }

        private void Initialize()
        {
            _isDisposed = false;
            _inRefresh = false;

            if(this.OrdersHold != null)
                return;

            _retryPolicy = new RetryPolicy(TimeSpan.FromSeconds(ApplicationSettings.Current.ClientUpdateIntervalSeconds));
            _refreshTimer = new System.Windows.Threading.DispatcherTimer();
            _refreshTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            _refreshTimer.Interval = TimeSpan.FromMilliseconds(this._retryPolicy.RefreshTimeInMilliSeconds);
            
            this.OrdersHold = new OrderStatusDataSet.OrderHoldsDataTable();

            grdHolds.FieldSettings.AllowEdit = false;
            grdHolds.FieldSettings.LabelClickAction = LabelClickAction.SortByMultipleFields;
            grdHolds.FieldSettings.CellClickAction = CellClickAction.SelectRecord;

            grdHolds.SelectedItemsChanged += grdHolds_SelectedItemsChanged;
            grdHolds.DataSource = OrdersHold.DefaultView;

            DWOSApp.MainForm.DataRefreshed += MainForm_DataRefreshed;
        }
        
        private void LoadData()
        {
            try
            {
                using (new UsingWaitCursorWpf(this))
                {
                    if(_taHold == null)
                        _taHold = new Data.Datasets.OrderStatusDataSetTableAdapters.OrderHoldsTableAdapter();

                    _taHold.FillAllHolds(OrdersHold);

                    foreach (var orderRow in OrdersHold)
                    {
                        if (orderRow.OrderType != 1)
                        {
                            switch (orderRow.OrderType)
                            {
                                case 5:
                                    orderRow.ReasonName = "Rework Hold";
                                    break;
                                case 7:
                                    orderRow.ReasonName = "Quarantined";
                                    break;
                            }

                            // Check the OrderHold table for info
                            var dtOrderHold = new Data.Datasets.OrdersDataSet.OrderHoldDataTable();
                            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderHoldTableAdapter())
                                ta.Fill(dtOrderHold, orderRow.OrderID);
                            
                            if (dtOrderHold.Rows.Count > 0)
                            {
                                orderRow.Notes = dtOrderHold[0].Notes;
                                orderRow.TimeIn = dtOrderHold[0].TimeIn;
                                using (var taUser = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.UsersTableAdapter())
                                    orderRow.UserName = taUser.GetUserName(dtOrderHold[0].TimeInUser);
                            }
                            else
                            {
                                // Need to look in the InternalRework table
                                var dtInternalRework = new Data.Datasets.OrdersDataSet.InternalReworkDataTable();
                                using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.InternalReworkTableAdapter())
                                {
                                    ta.FillByOriginalOrderID(dtInternalRework, orderRow.OrderID);
                                    
                                    if (dtInternalRework.Rows.Count <= 0)
                                        ta.FillByReworkOrderID(dtInternalRework, orderRow.OrderID);
                                }

                                if (dtInternalRework.Rows.Count > 0)
                                {
                                    orderRow.Notes = dtInternalRework[0].Notes;
                                    orderRow.TimeIn = dtInternalRework[0].DateCreated;
                                    using (var taUser = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.UsersTableAdapter())
                                        orderRow.UserName = taUser.GetUserName(dtInternalRework[0].CreatedBy);
                                }
                                else
                                {
                                    // Checked Order Hold and Internal Rework tables, lets look in the Order table

                                }
                            }
                        }
                    }

                    //_taHold.Fill(OrdersHold);
                }

                this._retryPolicy.OnSuccess();
            }
            catch (Exception exc)
            {
                this._retryPolicy.OnFailure();
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading HOLD data from database server.");
            }
        }

        private void RefreshData()
        {
            if (_inRefresh || _isDisposed)
                return;

            _inRefresh = true;
            
            StopRefreshTimer();
            LoadData();
            StartRefreshTimer();

            var orderIDToSelect = _orderIdToAutoselect;

            if (orderIDToSelect.HasValue)
            {
                SelectWO(orderIDToSelect.Value);
            }
            
            _inRefresh = false;
        }

        private void StopRefreshTimer()
        {
            _refreshTimer.Stop();
        }

        private void StartRefreshTimer()
        {
            if (this._retryPolicy.ShouldContinue)
            {
                _refreshTimer.Interval = TimeSpan.FromMilliseconds(this._retryPolicy.RefreshTimeInMilliSeconds);
                _refreshTimer.Start();
            }
        }

        public void Dispose()
        {
            try
            {
                _isDisposed = true;
                _retryPolicy = null;

                if (_refreshTimer != null)
                {
                    _refreshTimer.Tick -= new EventHandler(dispatcherTimer_Tick);
                    _refreshTimer.Stop();
                    _refreshTimer = null;

                    DWOSApp.MainForm.DataRefreshed -= MainForm_DataRefreshed;
                }

                if (OrdersHold != null)
                    OrdersHold.Dispose();
                OrdersHold = null;

                if (_taHold != null)
                    _taHold.Dispose();
                _taHold = null;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error disposing hold view.");
            }
        }

        private void UpdateSelectedRow()
        {
            try
            {
                var tab = DWOSApp.MainForm.ActiveTab as IOrderSummary;
                if (tab == null)
                    return;

                var currentRow = this.CurrentRow;

                if (currentRow != null)
                    tab.SelectWO(currentRow.OrderID);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on hold selected row.");
            }
        }

        #endregion

        #region Events

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            Initialize();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            Late3ColorConverter.Initialize();
            RefreshData();
        }

        private void grdHolds_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e) { UpdateSelectedRow(); }

        private void MainForm_DataRefreshed(object sender, EventArgs e)
        {
            RefreshData();
        }

        #endregion
    }
}
