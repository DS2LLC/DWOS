using System;
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DWOS.Utilities;
using DWOS.ViewModels;
using System.Threading.Tasks;
using DWOS.Services.Messages;
using Newtonsoft.Json;
using ZXing.Mobile;
using ZX = ZXing;
using Android.Support.Design.Widget;
using V7 = Android.Support.V7;

namespace DWOS.Android
{
    /// <summary>
    /// Primary activity that shows lists of orders and batches.
    /// </summary>
    /// <remarks>
    /// This activity is capable of conditionally showing order and batch
    /// details if an order or batch is selected and the view supports it.
    /// </remarks>
    [Activity(Label = "DWOS", Name = "dwos.android.MainActivity", Theme = "@style/Theme.NoActionBar")]
    [MetaData("android.app.default_searchable", Value = "dwos.android.SearchableOrderActivity")]
    public class MainActivity : BaseActivity, IOrdersFragmentCallback, IBatchFragmentCallback, ICheckInDialogCallback,
        IFilterFragmentCallback, IProcessDetailsFragmentCallback, IBatchProcessDetailsFragmentCallback, IDocumentSelectionCallback, IOrderNotesFragmentCallback
    {
        #region Fields

        protected const string ORDER_LIST_FRAGMENT_TAG = "OrdersListFragment";
        protected const string ORDER_DETAILS_FRAGMENT_TAG = "OrdersDetailsFragment";
        protected const string BATCH_LIST_FRAGMENT_TAG = "BatchListFragment";
        protected const string BATCH_DETAILS_FRAGMENT_TAG = "BatchDetailsFragment";
        const string BUNDLEID_SELECTEDTAB = "SelectedTabIndex";
        protected const string BUNDLEID_IS_SEARCH_ADVANCED = "IsSearchAdvanced";

        protected const int TAB_POSITION_ORDERS = 0;
        protected const int TAB_POSITION_BATCH = 1;

        OrderViewModel _orderViewModel;
        LogInViewModel _loginViewModel;
        BatchViewModel _batchViewModel;
        const int _orderProcessingRequestCode = 1;
        const int _controlInspectionRequestCode = 2;
        const int _orderNoteRequestCode = 3;
        TextView _textViewNoOrder;
        OrderDetailsHostFragment _orderDetailsHostFragment;
        OrdersListFragment _ordersListFragment;
        BatchDetailsHostFragment _batchDetailsFragment;
        BatchListFragment _batchListFragment;
        bool _refreshItemsBeacuseFilterChanged = false;
        bool? _isSearchAdvanced;
        int _selectedTab;
        private Timer _refreshTimer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current mode.
        /// </summary>
        /// <value>
        /// The current mode.
        /// </value>
        private Mode CurrentMode
        {
            get
            {
                return (_selectedTab == TAB_POSITION_BATCH) ? Mode.Batches : Mode.Orders;
            }
        }

        /// <summary>
        /// Gets a value indicating whether frame details is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if frame details visible; otherwise, <c>false</c>.
        /// </value>
        protected bool IsFrameDetailsVisible
        {
            get
            {
                var inflateDetailsFragment = FindViewById<FrameLayout>(Resource.Id.frameLayoutOrderDetails) != null ? true : false;
                return inflateDetailsFragment;
            }
        }

        protected OrderDetailsHostFragment OrdersDetailsFragment
        {
            get { return _orderDetailsHostFragment ?? (_orderDetailsHostFragment = InitializeOrderDetailsFragment()); }
        }

        private OrderDetailsHostFragment InitializeOrderDetailsFragment()
        {
            OrderDetailsHostFragment fragment = null;

            try
            {
                if (FragmentManager != null)
                    fragment = FragmentManager.FindFragmentByTag<OrderDetailsHostFragment>(ORDER_DETAILS_FRAGMENT_TAG);
            }
            catch (Exception)
            {

            }
            finally
            {
                if (fragment == null)
                    fragment = new OrderDetailsHostFragment();
            }

            return fragment;
        }

        protected OrdersListFragment @OrdersListFragment
        {
            get { return _ordersListFragment ?? (_ordersListFragment = InitializeOrderListFragment()); }
        }

        private OrdersListFragment InitializeOrderListFragment()
        {
            OrdersListFragment fragment = null;

            try
            {
                if (FragmentManager != null)
                    fragment = FragmentManager.FindFragmentByTag<OrdersListFragment>(ORDER_LIST_FRAGMENT_TAG);
            }
            catch (Exception)
            {

            }
            finally
            {
                if (fragment == null)
                    fragment = new OrdersListFragment();
            }

            return fragment;
        }

        private BatchDetailsHostFragment @BatchDetailsFragment
        {
            get { return _batchDetailsFragment ?? (_batchDetailsFragment = InitializeBatchDetailsFragment()); }
        }

        private BatchDetailsHostFragment InitializeBatchDetailsFragment()
        {
            BatchDetailsHostFragment fragment = null;

            try
            {
                if (FragmentManager != null)
                    fragment = FragmentManager.FindFragmentByTag<BatchDetailsHostFragment>(BATCH_DETAILS_FRAGMENT_TAG);
            }
            catch (Exception)
            {

            }
            finally
            {
                if (fragment == null)
                    fragment = new BatchDetailsHostFragment();
            }

            return fragment;
        }

        private BatchListFragment @BatchListFragment
        {
            get { return _batchListFragment ?? (_batchListFragment = InitializeBatchListFragment()); }
        }

        private BatchListFragment InitializeBatchListFragment()
        {
            BatchListFragment fragment = null;

            try
            {
                if (FragmentManager != null)
                    fragment = FragmentManager.FindFragmentByTag<BatchListFragment>(BATCH_LIST_FRAGMENT_TAG);
            }
            catch (Exception)
            {

            }
            finally
            {
                if (fragment == null)
                    fragment = new BatchListFragment();
            }

            return fragment;
        }

        private bool ShouldInitializeTabs
        {
            get { return !(Intent != null && Intent.ActionSearch.Equals(Intent.Action)); }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainActivity"/> class.
        /// </summary>
        public MainActivity()
        {

        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _orderViewModel = ServiceContainer.Resolve<OrderViewModel>();
            _loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            _batchViewModel = ServiceContainer.Resolve<BatchViewModel>();

            SetContentView(Resource.Layout.OrdersLayout);
            SetSupportActionBar(FindViewById<V7.Widget.Toolbar>(Resource.Id.toolbar));

            SupportActionBar.Subtitle = GetString(Resource.String.LoggedInFormat,
                _loginViewModel.UserProfile.Name);

            _textViewNoOrder = FindViewById<TextView>(Resource.Id.textViewNoOrder);

            if (ShouldInitializeTabs)
            {
                var tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);

                tabLayout.AddTab(tabLayout.NewTab()
                    .SetText("Orders"), TAB_POSITION_ORDERS, false);

                tabLayout.AddTab(tabLayout.NewTab()
                    .SetText("Batch"), TAB_POSITION_BATCH, false);

                tabLayout.ClearOnTabSelectedListeners();
                tabLayout.AddOnTabSelectedListener(new TabSelectListener(this));
                InitializeTabs(bundle);
            }
        }

        private void InitializeTabs(Bundle bundle)
        {

            var tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
            Mode modeToUse;

            if (bundle == null)
            {
                modeToUse = Mode.Orders;
            }
            else
            {
                int selectedIndex = bundle.GetInt(BUNDLEID_SELECTEDTAB, 0);
                modeToUse = selectedIndex == TAB_POSITION_BATCH ? Mode.Batches : Mode.Orders;

                if (modeToUse == Mode.Batches && OrdersListFragment.IsAdded && !OrdersListFragment.IsHidden)
                {
                    var transaction = FragmentManager.BeginTransaction();
                    transaction.Hide(OrdersListFragment);
                    if (IsFrameDetailsVisible)
                        transaction.Hide(OrdersDetailsFragment);
                    transaction.Commit();
                }
                else if (modeToUse == Mode.Orders && BatchListFragment.IsAdded & !BatchListFragment.IsHidden)
                {
                    var transaction = FragmentManager.BeginTransaction();
                    transaction.Hide(BatchListFragment);
                    if (IsFrameDetailsVisible)
                        transaction.Hide(BatchDetailsFragment);
                    transaction.Commit();
                }

            }

            if (modeToUse == Mode.Orders)
            {
                tabLayout.GetTabAt(TAB_POSITION_ORDERS)
                    .Select();
            }
            else if (modeToUse == Mode.Batches)
            {
                tabLayout.GetTabAt(TAB_POSITION_BATCH)
                    .Select();
            }
        }

        /// <summary>
        /// Called after <c><see cref="M:Android.App.Activity.OnRestoreInstanceState(Android.OS.Bundle)" /></c>, <c><see cref="M:Android.App.Activity.OnRestart" /></c>, or
        /// <c><see cref="M:Android.App.Activity.OnPause" /></c>, for your activity to start interacting with the user.
        /// </summary>
        protected override void OnResume()
        {
            try
            {
                if (CurrentMode == Mode.Orders)
                {
                    var detailsFragment = FragmentManager.FindFragmentByTag<OrderDetailsHostFragment>(ORDER_DETAILS_FRAGMENT_TAG);
                    SetDetailsFragmentVisibility(detailsFragment);
                }
                else
                {
                    var detailsFragment = FragmentManager.FindFragmentByTag<BatchDetailsHostFragment>(BatchDetailsHostFragment.BATCHDETAILS_FRAGMENT_TAG);
                }

                StartRefreshTimer();

                base.OnResume();
            }
            catch (Exception exception)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error in OnResume", exception);
                DWOSApplication.Current.RestoreMainActivityFromCrash = true;
                base.RestartApp();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            StopRefreshTimer();
        }

        /// <summary>
        /// Perform any final cleanup before an activity is destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            if (_textViewNoOrder != null)
                _textViewNoOrder.Dispose();
            _orderDetailsHostFragment = null;
            _ordersListFragment = null;
            _batchDetailsFragment = null;
            _batchListFragment = null;

            base.OnDestroy();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt(BUNDLEID_SELECTEDTAB, _selectedTab);
        }

        /// <summary>
        /// Called when an activity you launched exits, giving you the requestCode
        /// you started it with, the resultCode it returned, and any additional
        /// data from it.
        /// </summary>
        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == _orderProcessingRequestCode && resultCode == Result.Ok)
            {
                await OrderProcessSavedAsync(data);
            }
            else if (requestCode == _controlInspectionRequestCode)
            {
                if (resultCode == Result.Ok)
                {
                    await ControlInspectionCompletedAsync(data, resultCode);
                }
                else
                {
                    // Refresh data - may have completed batch without performing any inspections
                    await RefreshAsync();
                }
            }
            else if (requestCode == _orderNoteRequestCode && resultCode == Result.Ok)
            {
                await OrderNoteSavedAsync(data);
            }
            else
            {
                base.OnActivityResult(requestCode, resultCode, data);
            }
        }

        private void StartRefreshTimer()
        {
            var refreshInterval = TimeSpan.FromSeconds(ApplicationSettings.Settings.ClientUpdateIntervalSeconds);

            if (refreshInterval <= TimeSpan.Zero)
            {
                refreshInterval = TimeSpan.FromMinutes(3);
            }

            _refreshTimer = new Timer(OnRefreshTimerCallback,
                null,
                refreshInterval,
                refreshInterval);
        }

        private void StopRefreshTimer()
        {
            _refreshTimer?.Dispose();
            _refreshTimer = null;
        }

        private async Task AddBatchFragments(FragmentTransaction transaction)
        {
            if (BatchListFragment.IsAdded)
                transaction.Show(BatchListFragment);
            else
                transaction.Add(Resource.Id.frameLayoutOrders, BatchListFragment, BATCH_LIST_FRAGMENT_TAG);
            if (IsFrameDetailsVisible)
            {
                if (BatchDetailsFragment.IsAdded)
                    transaction.Show(BatchDetailsFragment);
                else
                    transaction.Add(Resource.Id.frameLayoutOrderDetails, BatchDetailsFragment, BATCH_DETAILS_FRAGMENT_TAG);

                if (_textViewNoOrder != null)
                {
                    _textViewNoOrder.Visibility = _batchViewModel.ActiveBatch != null ? ViewStates.Gone : ViewStates.Visible;
                    _textViewNoOrder.Text = string.Format("No Batches selected");
                    if (_textViewNoOrder.Visibility == ViewStates.Gone)
                        transaction.Show(BatchDetailsFragment);
                    else
                        transaction.Hide(BatchDetailsFragment);
                }
            }

            if (_refreshItemsBeacuseFilterChanged)
            {
                await RefreshAsync();
                _refreshItemsBeacuseFilterChanged = false;
            }
        }

        private void RemoveBatchFragments(FragmentTransaction transaction)
        {
            transaction.Hide(BatchListFragment);
            if (IsFrameDetailsVisible)
                transaction.Hide(BatchDetailsFragment);
        }

        private async Task AddOrderFragments(FragmentTransaction transaction)
        {
            if (OrdersListFragment.IsAdded)
                transaction.Show(OrdersListFragment);
            else
                transaction.Add(Resource.Id.frameLayoutOrders, OrdersListFragment, ORDER_LIST_FRAGMENT_TAG);
            if (IsFrameDetailsVisible)
            {
                if (OrdersDetailsFragment.IsAdded)
                    transaction.Show(OrdersDetailsFragment);
                else
                    transaction.Add(Resource.Id.frameLayoutOrderDetails, OrdersDetailsFragment, ORDER_DETAILS_FRAGMENT_TAG);

                if (_textViewNoOrder != null)
                {
                    _textViewNoOrder.Visibility = _orderViewModel.ActiveOrder != null ? ViewStates.Gone : ViewStates.Visible;
                    _textViewNoOrder.Text = string.Format("No Orders selected");
                    if (_textViewNoOrder.Visibility == ViewStates.Gone)
                        transaction.Show(OrdersDetailsFragment);
                    else
                        transaction.Hide(OrdersDetailsFragment);
                }
            }

            if (_refreshItemsBeacuseFilterChanged)
            {
                await RefreshAsync();
                _refreshItemsBeacuseFilterChanged = false;
            }
        }

        private void RemoveOrderFragments(FragmentTransaction transaction)
        {
            transaction.Hide(OrdersListFragment);
            if (IsFrameDetailsVisible)
                transaction.Hide(OrdersDetailsFragment);
        }

        /// <summary>
        /// Initialize the contents of the Activity's standard options menu.
        /// </summary>
        /// <param name="menu">The options menu in which you place your items.</param>
        /// <returns>
        /// To be added.
        /// </returns>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.order_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        /// <summary>
        /// Prepare the Screen's standard options menu to be displayed.
        /// </summary>
        /// <param name="menu">The options menu as last shown or first initialized by
        /// onCreateOptionsMenu().</param>
        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            SetCheckInMenuVisibility(menu);
            if (CurrentMode == Mode.Orders)
            {
                SetOrderProcessMenuVisibility(menu);
                SetOrderControlInspectionMenuVisibility(menu);
                SetSearchOptions(menu);
            }
            else
            {
                SetBatchProcessMenuVisibility(menu);
                SetBatchControlInspectionMenuVisibility(menu);
                SetSearchOptions(menu);
            }
            return base.OnPrepareOptionsMenu(menu);
        }

        private void SetSearchOptions(IMenu menu)
        {
            if (CurrentMode == Mode.Orders)
            {
                var searchManager = GetSystemService(Context.SearchService) as SearchManager;
                var searchableInfo = searchManager.GetSearchableInfo(ComponentName);
                var searchMenu = menu.FindItem(Resource.Id.action_search);
                searchMenu.SetVisible(visible: true);
                var searchView = (SearchView)searchMenu.ActionView;
                searchView.SetSearchableInfo(searchableInfo);
                searchView.SetInputType(global::Android.Text.InputTypes.ClassNumber);

                var searchAdvancedMenu = menu.FindItem(Resource.Id.action_searchAdvanced);
                searchAdvancedMenu.SetVisible(visible: true);
                var searchAdvancedView = (SearchView)searchAdvancedMenu.ActionView;
                searchAdvancedView.SetSearchableInfo(searchableInfo);
            }
            else
            {
                menu.FindItem(Resource.Id.action_search)
                    .SetVisible(visible: false);
                menu.FindItem(Resource.Id.action_searchAdvanced)
                    .SetVisible(visible: false);
            }
        }

        private void SetCheckInMenuVisibility(IMenu menu)
        {
            var isVisible = _loginViewModel.UserProfile != null &&
                _loginViewModel.UserProfile.SecurityRoles.Contains(ApplicationSettings.Settings.PartCheckInRole);
            var checkinMenu = menu.FindItem(Resource.Id.action_checkin);
            checkinMenu.SetVisible(isVisible);
        }

        private void SetOrderControlInspectionMenuVisibility(IMenu menu)
        {
            var isVisible = false;
            var isInRole = _loginViewModel.UserProfile != null &&
                _loginViewModel.UserProfile.SecurityRoles.Contains(ApplicationSettings.Settings.ControlInspectionRole);
            var detailsFragment = FragmentManager.FindFragmentByTag<OrderDetailsHostFragment>(ORDER_DETAILS_FRAGMENT_TAG);
            if (isInRole && detailsFragment != null)
                isVisible = _orderViewModel.CanInspect;
            var controlInspectionMenu = menu.FindItem(Resource.Id.action_controlInspection);
            controlInspectionMenu.SetVisible(isVisible);
        }

        private void SetBatchProcessMenuVisibility(IMenu menu)
        {
            var isVisible = false;
            var isInRole = _loginViewModel.UserProfile != null &&
                _loginViewModel.UserProfile.SecurityRoles.Contains(ApplicationSettings.Settings.BatchOrderProcessingRole);
            var detailsFragment = FragmentManager.FindFragmentByTag<BatchDetailsHostFragment>(BATCH_DETAILS_FRAGMENT_TAG);
            if (isInRole && detailsFragment != null && _batchViewModel.ActiveBatch != null)
                isVisible = _batchViewModel.ActiveBatch.WorkStatus == ApplicationSettings.Settings.WorkStatusInProcess;
            var orderProcessing = menu.FindItem(Resource.Id.action_orderprocessing);
            orderProcessing.SetVisible(isVisible);
        }

        private void SetBatchControlInspectionMenuVisibility(IMenu menu)
        {
            var isVisible = false;
            var isInRole = _loginViewModel.UserProfile != null &&
                _loginViewModel.UserProfile.SecurityRoles.Contains(ApplicationSettings.Settings.ControlInspectionRole);
            var detailsFragment = FragmentManager.FindFragmentByTag<BatchDetailsHostFragment>(BATCH_DETAILS_FRAGMENT_TAG);
            if (isInRole && detailsFragment != null && _batchViewModel.ActiveBatch != null)
                isVisible = _batchViewModel.ActiveBatch.WorkStatus == ApplicationSettings.Settings.WorkStatusPendingInspection;
            var controlInspectionMenu = menu.FindItem(Resource.Id.action_controlInspection);
            controlInspectionMenu.SetVisible(isVisible);
        }

        void SetOrderProcessMenuVisibility(IMenu menu)
        {
            var isVisible = false;
            var isInRole = _loginViewModel.UserProfile != null &&
                _loginViewModel.UserProfile.SecurityRoles.Contains(ApplicationSettings.Settings.OrderProcessingRole);
            var detailsFragment = FragmentManager.FindFragmentByTag<OrderDetailsHostFragment>(ORDER_DETAILS_FRAGMENT_TAG);
            if (isInRole && detailsFragment != null)
                isVisible = _orderViewModel.CanProcess;
            var orderProcessing = menu.FindItem(Resource.Id.action_orderprocessing);
            orderProcessing.SetVisible(isVisible);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                switch (item.ItemId)
                {
                    case Resource.Id.action_checkin:
                        return OnActionCheckInSelected();
                    case Resource.Id.action_orderprocessing:
                        return OnActionOrderProcessing();
                    case Resource.Id.action_logout:
                        return OnActionLogout();
                    case Resource.Id.action_about:
                        return OnActionAbout();
                    case Resource.Id.action_feedback:
                        return OnActionFeedback();
                    case Resource.Id.action_knowledgebase:
                        return OnActionKnowledgeBase();
                    case Resource.Id.action_help:
                        return OnActionHelp();
                    case Resource.Id.action_refresh:
                        var task = RefreshAsync();
                        return true;
                    case Resource.Id.action_filter:
                        return OnActionFilter();
                    case Resource.Id.action_scan:
                        var scanTask = OnActionScanForOrderAsync();
                        return true;
                    case Resource.Id.action_controlInspection:
                        return OnActionControlInspection();
                    case Resource.Id.action_searchAdvanced:
                        _isSearchAdvanced = true;
                        return base.OnOptionsItemSelected(item);
                    default:
                        return base.OnOptionsItemSelected(item);
                }
            }
            catch (Exception exception)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                var optionItemException = new Exception("Error in MainActivity.OnOptionsItemSelected: " + item.ItemId, exception);
                logService.LogError(_standardErrorMessage, optionItemException, this, true);
                return base.OnOptionsItemSelected(item);
            }
        }

        private bool OnActionControlInspection()
        {
            if (_loginViewModel.IsLicenseActivated)
            {
                var intent = new Intent(this, typeof(InspectionActivity));
                if (CurrentMode == Mode.Orders)
                    intent.PutExtra(InspectionActivity.INTENT_ORDERID, _orderViewModel.ActiveOrder.OrderId);
                else
                    intent.PutExtra(InspectionActivity.INTENT_BATCHID, _batchViewModel.ActiveBatch.BatchId);
                intent.PutExtra(InspectionActivity.INTENT_MODE, CurrentMode.ToString());
                StartActivityForResult(intent, _controlInspectionRequestCode);
            }
            else
                LogOutUserWithExpiredMessage();

            return true;
        }

        private bool OnActionFilter()
        {
            var transaction = FragmentManager.BeginTransaction();
            var filterFragment = new FilterFragment(_orderViewModel.FilterDepartmentValue, _orderViewModel.FilterLineValue, _orderViewModel.FilterStatusValue, _orderViewModel.FilterTypeValue);
            filterFragment.Show(transaction, FilterFragment.FILTER_FRAGMENT_TAG);

            return true;
        }

        /// <summary>
        /// Refreshes the Orders for both Details and Summaries and Invalidates the Action Bar.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> RefreshAsync()
        {
            if (CurrentMode == Mode.Orders)
            {
                if (_orderViewModel.ActiveOrder != null)
                    await UpdateOrdersAndOrderDetailsFragmentsAsync();
                else
                    await UpdateOrdersFragmentAsync();
            }
            else
            {
                if (_batchViewModel.ActiveBatch != null)
                    await UpdateBatchesAndBatchDetailsFragmentsAsync();
                else
                    await UpdateBatchesFragmentAsync();
            }

            InvalidateOptionsMenu();

            return true;
        }

        private bool OnActionHelp()
        {
            var url = GetString(Resource.String.HelpUrl);
            LaunchUrl(url);

            return true;
        }

        private bool OnActionKnowledgeBase()
        {
            var url = GetString(Resource.String.KnowledgeBaseUrl);
            LaunchUrl(url);

            return true;
        }

        private bool OnActionFeedback()
        {
            var url = GetString(Resource.String.FeedbackUrl);
            LaunchUrl(url);

            return true;
        }

        private void LaunchUrl(string url)
        {
            var uri = global::Android.Net.Uri.Parse(url);
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private bool OnActionAbout()
        {
            var transaction = FragmentManager.BeginTransaction();
            var aboutDialog = new AboutFragment();
            aboutDialog.Show(transaction, AboutFragment.ABOUT_FRAGMENT_TAG);

            return true;
        }

        private bool OnActionLogout()
        {
            var logInViewModel = ServiceContainer.Resolve<LogInViewModel>();
            logInViewModel.Logout();

            return true;
        }

        private bool OnActionOrderProcessing()
        {
            if (_loginViewModel.IsLicenseActivated)
            {
                var intent = new Intent(this, typeof(OrderProcessActivity));
                intent.PutExtra(OrderProcessActivity.INTENT_MODE, CurrentMode.ToString());
                if (CurrentMode == Mode.Orders)
                    intent.PutExtra(OrderProcessActivity.INTENT_ORDERID, _orderViewModel.ActiveOrder.OrderId);
                else if (CurrentMode == Mode.Batches)
                    intent.PutExtra(OrderProcessActivity.INTENT_BATCHID, _batchViewModel.ActiveBatch.BatchId);
                StartActivityForResult(intent, _orderProcessingRequestCode);
                LogInfo(message: "OnActionOrderProcessing");
            }
            else
                LogOutUserWithExpiredMessage();

            return true;
        }

        private bool OnActionCheckInSelected()
        {
            if (_loginViewModel.IsLicenseActivated)
            {
                var transaction = FragmentManager.BeginTransaction();
                var bundle = new Bundle();
                bundle.PutString(CheckInFragment.BUNDLE_EXTRA_MODE, CurrentMode.ToString());
                var dialog = new CheckInFragment();
                dialog.Arguments = bundle;
                dialog.Show(transaction, CheckInFragment.CHECKIN_FRAGMENT_TAG);
            }
            else
                LogOutUserWithExpiredMessage();

            return true;
        }

        public async void OnOrderSelected (int orderId)
        {
            var success = await UpdateOrderDetailsFragmentAsync(orderId);
            if (success)
                InvalidateOptionsMenu();
            else
            {
                var detailsIntent = new Intent(this, typeof(OrderDetailActivity));
                detailsIntent.PutExtra(OrderDetailActivity.INTENT_ORDERID, orderId);
                StartActivity(detailsIntent);
            }
        }

        public async void OnBatchSelected(int batchId)
        {
            var success = await UpdateBatchDetailsFragmentAsync(batchId);
            if (success)
                InvalidateOptionsMenu();
            else
            {
                var detailsIntent = new Intent(this, typeof(BatchDetailActivity));
                detailsIntent.PutExtra(BatchDetailActivity.INTENT_BATCHID, batchId);
                StartActivity(detailsIntent);
            }
        }

        private async Task UpdateOrdersAndOrderDetailsFragmentsAsync()
        {
            try
            {
                var detailsFragment = FragmentManager.FindFragmentByTag<OrderDetailsHostFragment>(ORDER_DETAILS_FRAGMENT_TAG);
                if (detailsFragment != null)
                    await detailsFragment.LoadOrderAsync(_orderViewModel.ActiveOrder.OrderId);

                var ordersFragment = FragmentManager.FindFragmentByTag<OrdersListFragment>(ORDER_LIST_FRAGMENT_TAG);
                if (ordersFragment != null)
                {
                    await ordersFragment.LoadOrderSummariesAsync();
                    if (IsDestroyed || _isPaused)
                        return;//Need to bail because our Activity is gone and trying to work with Views will throw a Java.IllegalStateException

                    if (_orderViewModel.ActiveOrder != null)
                        ordersFragment.SetSelectedOrder(_orderViewModel.ActiveOrder.OrderId);
                    else
                        ordersFragment.SetSelectedOrder(orderId: -1);
                }

                if (detailsFragment != null && !(IsDestroyed || _isPaused))
                    SetDetailsFragmentVisibility(detailsFragment);
            }
            catch (Exception exception)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error in UpdateOrdersAndOrderDetailsFragmentsAsync", exception);
            }
        }

        private async Task UpdateOrdersFragmentAsync()
        {
            try
            {
                var ordersFragment = FragmentManager.FindFragmentByTag<OrdersListFragment>(ORDER_LIST_FRAGMENT_TAG);
                if (ordersFragment != null)
                {
                    await ordersFragment.LoadOrderSummariesAsync();
                    if (IsDestroyed || _isPaused)
                        return;//Need to bail because our Activity is gone and trying to work with Views will throw a Java.IllegalStateException

                    if (_orderViewModel.ActiveOrder != null)
                        ordersFragment.SetSelectedOrder(_orderViewModel.ActiveOrder.OrderId);
                    else
                        ordersFragment.SetSelectedOrder(orderId: -1);
                }
            }
            catch (Exception exception)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error in UpdateOrdersFragmentAsync", exception);
            }
        }

        private async Task<bool> UpdateOrderDetailsFragmentAsync(int orderId)
        {
            var detailsFragment = FragmentManager.FindFragmentByTag<OrderDetailsHostFragment>(ORDER_DETAILS_FRAGMENT_TAG);
            if (detailsFragment != null)
            {
                await detailsFragment.LoadOrderAsync(orderId);

                return true;
            }
            else
                return false;
        }

        private async Task UpdateBatchesFragmentAsync()
        {
            try
            {
                var batchesFragment = FragmentManager.FindFragmentByTag<BatchListFragment>(BATCH_LIST_FRAGMENT_TAG);
                if (batchesFragment != null)
                {
                    await batchesFragment.LoadBatchSummariesAsync();
                    if (IsDestroyed || _isPaused)
                        return;//Need to bail because our Activity is gone and trying to work with Views will throw a Java.IllegalStateException

                    if (_batchViewModel.ActiveBatch != null)
                        batchesFragment.SetSelectedOrder(_batchViewModel.ActiveBatch.BatchId);
                    else
                        batchesFragment.SetSelectedOrder(batchId: -1);
                }
            }
            catch (Exception exception)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error in UpdateBatchesFragmentAsync", exception);
            }
        }

        private async Task UpdateBatchesAndBatchDetailsFragmentsAsync()
        {
            try
            {
                var detailsFragment = FragmentManager.FindFragmentByTag<BatchDetailsHostFragment>(BATCH_DETAILS_FRAGMENT_TAG);
                if (detailsFragment != null)
                    await detailsFragment.LoadBatchAsync(_batchViewModel.ActiveBatch.BatchId);

                var batchesFragment = FragmentManager.FindFragmentByTag<BatchListFragment>(BATCH_LIST_FRAGMENT_TAG);
                if (batchesFragment != null)
                {
                    await batchesFragment.LoadBatchSummariesAsync();
                    if (IsDestroyed || _isPaused)
                        return;//Need to bail because our Activity is gone and trying to work with Views will throw a Java.IllegalStateException

                    if (_batchViewModel.ActiveBatch != null)
                        batchesFragment.SetSelectedOrder(_batchViewModel.ActiveBatch.BatchId);
                    else
                        batchesFragment.SetSelectedOrder(batchId: -1);
                }

                if (detailsFragment != null && !(IsDestroyed || _isPaused))
                    SetDetailsFragmentVisibility(detailsFragment);
            }
            catch (Exception exception)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error in UpdateBatchesAndBatchDetailsFragmentsAsync", exception);
            }
        }

        private async Task<bool> UpdateBatchDetailsFragmentAsync(int batchId)
        {
            try
            {
                var detailsFragment = FragmentManager.FindFragmentByTag<BatchDetailsHostFragment>(BATCH_DETAILS_FRAGMENT_TAG);
                if (detailsFragment != null)
                {
                    await detailsFragment.LoadBatchAsync(batchId);
                    if (IsDestroyed || _isPaused)
                        return true;//Need to bail because our Activity is gone and trying to work with Views will throw a Java.IllegalStateException

                    SetDetailsFragmentVisibility(detailsFragment);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception exception)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error in UpdateBatchDetailsFragmentAsync", exception);
            }

            return false;
        }

        public async void CheckInDialogDismissed(CheckInFragment dialog, bool result, ViewModels.CheckInViewModel checkInViewModel)
        {
            dialog.Dismiss();
            if (result == true)
            {
                ViewModelResult checkInResult;

                if (CurrentMode == Mode.Orders)
                    checkInResult = await _orderViewModel.CheckInOrderAsync(checkInViewModel);
                else
                    checkInResult = await _batchViewModel.CheckInBatchAsync(checkInViewModel);

                if (checkInResult.Success && string.IsNullOrEmpty(checkInResult.ErrorMessage))
                {
                    var message = string.Format("Order: {0} checked in.", checkInViewModel.OrderId);
                    Toast.MakeText(this, message, ToastLength.Short)
                        .Show();
                    await RefreshAsync();
                }
                else
                {
                    var message = string.Format("Unable to check in Order: {0}. {1}", checkInViewModel.OrderId, checkInResult.ErrorMessage);
                    Toast.MakeText(this, message, ToastLength.Short)
                        .Show();
                }
            }
        }

        private async Task OrderProcessSavedAsync(Intent resultIntent)
        {
            var itemType = CurrentMode == Mode.Orders ? "Order" : "Batch";
            var id = CurrentMode == Mode.Orders
                ? resultIntent.GetIntExtra(OrderProcessActivity.INTENT_ORDERID, defaultValue: -1)
                : resultIntent.GetIntExtra(OrderProcessActivity.INTENT_BATCHID, defaultValue: -1);
            var errorMessage = resultIntent.GetStringExtra(OrderProcessActivity.INTENT_ORDERPROCESS_ERROR);
            if (string.IsNullOrEmpty(errorMessage))
            {
                var message = string.Format("Order Processing for {0}: {1} updated.", itemType, id);
                Toast.MakeText(this, message, ToastLength.Short)
                    .Show();
                await RefreshAsync();
            }
            else
            {
                var message = string.Format("Unable save Processing Answers for {0}: {1}.", itemType, id);
                Toast.MakeText(this, message, ToastLength.Short)
                    .Show();
            }
        }

        private async Task ControlInspectionCompletedAsync(Intent resultIntent, Result resultCode)
        {
            var mode = resultIntent.GetStringExtra(InspectionActivity.INTENT_MODE);

            if (mode == Mode.Orders.ToString())
                await ControlOrderInspectionCompletedAsync(resultIntent);
            else if (mode == Mode.Batches.ToString())
                await ControlBatchInspectionCompletedAsync(resultIntent);
        }

        private async Task ControlBatchInspectionCompletedAsync(Intent resultIntent)
        {
            var batchId = resultIntent.GetIntExtra(InspectionActivity.INTENT_BATCHID, defaultValue: -1);
            var error = resultIntent.GetStringExtra(InspectionActivity.INTENT_ERROR);

            if (string.IsNullOrEmpty(error))
            {
                var batchInspectionViewModel = ServiceContainer.Resolve<BatchInspectionViewModel>();
                var result = await batchInspectionViewModel.SaveCompletedAsync(batchId);
                if (string.IsNullOrEmpty(result.ErrorMessage))
                {
                    var message = string.Format("Control Inspection for Batch: {0} updated.", batchId);
                    Toast.MakeText(this, message, ToastLength.Short)
                        .Show();
                    await RefreshAsync();
                    var batchDetail = await _batchViewModel.GetBatchDetailAsync(batchId);
                    if (batchDetail != null && batchDetail.WorkStatus == ApplicationSettings.Settings.WorkStatusPendingInspection)
                    {
                        batchInspectionViewModel.InvalidateViewModel();
                        var intent = new Intent(this, typeof(InspectionActivity));
                        intent.PutExtra(InspectionActivity.INTENT_MODE, Mode.Batches.ToString());
                        intent.PutExtra(InspectionActivity.INTENT_BATCHID, batchId);
                        StartActivityForResult(intent, _controlInspectionRequestCode);
                    }
                }
                else
                {
                    var message = string.Format("Control Inspection for Batch: {0} failed." + System.Environment.NewLine + "{1}", batchId, result.ErrorMessage);
                    Toast.MakeText(this, message, ToastLength.Short)
                        .Show();
                }
            }
            else
            {
                var message = string.Format("Control Inspection for Batch: {0} failed." + System.Environment.NewLine + "{1}", batchId, error);
                Toast.MakeText(this, message, ToastLength.Short)
                    .Show();
            }
        }

        private async Task ControlOrderInspectionCompletedAsync(Intent resultIntent)
        {
            var orderId = resultIntent.GetIntExtra(InspectionActivity.INTENT_ORDERID, defaultValue: -1);
            var error = resultIntent.GetStringExtra(InspectionActivity.INTENT_ERROR);

            if (string.IsNullOrEmpty(error))
            {
                var message = string.Format("Control Inspection for Order: {0} updated.", orderId);
                Toast.MakeText(this, message, ToastLength.Short)
                    .Show();
                await RefreshAsync();
                var orderDetail = await _orderViewModel.GetOrderDetailAsync(orderId, imageSize: 10);
                if (orderDetail != null && orderDetail.WorkStatus == ApplicationSettings.Settings.WorkStatusPendingInspection)
                {
                    var orderInspectionViewModel = ServiceContainer.Resolve<InspectionViewModel>();
                    orderInspectionViewModel.InvalidateViewModel();
                    var intent = new Intent(this, typeof(InspectionActivity));
                    intent.PutExtra(InspectionActivity.INTENT_MODE, Mode.Orders.ToString());
                    intent.PutExtra(InspectionActivity.INTENT_ORDERID, orderId);
                    StartActivityForResult(intent, _controlInspectionRequestCode);
                }
            }
            else
            {
                var message = string.Format("Control Inspection for Order: {0} failed." + System.Environment.NewLine + "{1}", orderId, error);
                Toast.MakeText(this, message, ToastLength.Short)
                    .Show();
            }
        }

        private async Task OrderNoteSavedAsync(Intent resultIntent)
        {
            if (resultIntent == null)
            {
                return;
            }

            var orderId = resultIntent.GetIntExtra(EditOrderNoteActivity.INTENT_OUT_ORDER_ID, defaultValue: -1);
            var error = resultIntent.GetStringExtra(EditOrderNoteActivity.INTENT_OUT_ERROR_MESSAGE);

            if (string.IsNullOrEmpty(error))
            {
                var message = $"Updated note for Order {orderId}";
                Toast.MakeText(this, message, ToastLength.Short)
                    .Show();

                await RefreshAsync();
            }
            else
            {
                var message = $"Failed to update Order Note: {System.Environment.NewLine}{error}";
                Toast.MakeText(this, message, ToastLength.Short)
                    .Show();
            }
        }

        /// <summary>
        /// Called when it is time to register to view model events.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override void RegisterViewModelEvents()
        {
            _orderViewModel.PropertyChanged += OrderViewModel_PropertyChanged;
            _batchViewModel.PropertyChanged += BatchViewModel_PropertyChanged;
        }

        protected override void UnregisterViewModelEvents()
        {
            _orderViewModel.PropertyChanged -= OrderViewModel_PropertyChanged;
            _batchViewModel.PropertyChanged -= BatchViewModel_PropertyChanged;
        }

        private void OnRefreshTimerCallback(object stateObject)
        {
            if (!_orderViewModel.IsBusy)
            {
                RunOnUiThread(() =>
                {
                    var task = RefreshAsync();
                });
            }
        }

        public async void OnFilterDimissed(FilterFragment dialog, bool result, string department, string line, string status, FilterType type)
        {
            dialog.Dismiss();

            if (result == true)
            {
                _orderViewModel.FilterDepartmentValue = department;
                _orderViewModel.FilterLineValue = line;
                _orderViewModel.FilterStatusValue = status;
                _orderViewModel.FilterTypeValue = type;
                _batchViewModel.FilterDepartmentValue = department;
                _batchViewModel.FilterStatusValue = status;
                _batchViewModel.FilterTypeValue = type;
                _refreshItemsBeacuseFilterChanged = true;
                await RefreshAsync();
            }
        }

        public void OnProcessDetailsSelected(OrderProcessInfo selectedProcessInfo)
        {
            var intent = new Intent(this, typeof(OrderProcessActivity));
            intent.PutExtra(OrderProcessActivity.INTENT_MODE, CurrentMode.ToString());
            var orderProcessJson = JsonConvert.SerializeObject(selectedProcessInfo);
            intent.PutExtra(OrderProcessActivity.INTENT_ORDERPROCESSINFO, orderProcessJson);
            StartActivityForResult(intent, _orderProcessingRequestCode);
        }

        public void OnBatchProcessDetailsSelected(BatchProcessInfo selectedProcessInfo)
        {
            var intent = new Intent(this, typeof(OrderProcessActivity));
            intent.PutExtra(OrderProcessActivity.INTENT_MODE, CurrentMode.ToString());
            var orderProcessJson = JsonConvert.SerializeObject(selectedProcessInfo);
            intent.PutExtra(OrderProcessActivity.INTENT_BATCHPROCESSINFO, orderProcessJson);
            StartActivityForResult(intent, _orderProcessingRequestCode);
        }

        public async void OnDocumentInfoSelected(DocumentInfo selectedDocument)
        {
            const string successMessage = "Successfully downloaded document.";

            var dialogService = ServiceContainer.Resolve<IDialogService>();
            var fileService = ServiceContainer.Resolve<IFileService>();
            var localDocumentPath = fileService.GetPath(selectedDocument);

            if (System.IO.File.Exists(localDocumentPath))
            {
                this.OpenFile(localDocumentPath);
            }
            else if (!string.IsNullOrEmpty(localDocumentPath))
            {
                var response = await fileService.Download(selectedDocument);

                if (response.Success)
                {
                    dialogService.ShowToast(successMessage, true);
                    this.OpenFile(localDocumentPath);
                }
                else if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    dialogService.ShowToast(response.ErrorMessage, true);
                }
            }
        }

        public async void OnMediaSummarySelected(MediaSummary selectedMedia)
        {
            const string errorMessage = "Could not retrieve media.";
            const string successMessage = "Successfully downloaded media.";

            var dialogService = ServiceContainer.Resolve<IDialogService>();
            var fileService = ServiceContainer.Resolve<IFileService>();
            var localDocumentPath = fileService.GetPath(selectedMedia);

            if (System.IO.File.Exists(localDocumentPath))
            {
                this.OpenFile(localDocumentPath);
            }
            else if (!string.IsNullOrEmpty(localDocumentPath))
            {
                var response = await fileService.Download(selectedMedia);

                if (response.Success)
                {
                    dialogService.ShowToast(successMessage, true);
                    this.OpenFile(localDocumentPath);
                }
                else if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    dialogService.ShowToast(errorMessage, true);
                }
            }
        }

        public void OnNoteSelected(OrderNote note)
        {
            var intent = new Intent(this, typeof(EditOrderNoteActivity));
            var orderProcessJson = JsonConvert.SerializeObject(note);
            intent.PutExtra(EditOrderNoteActivity.INTENT_INFO, orderProcessJson);
            StartActivityForResult(intent, _orderNoteRequestCode);
        }

        public override void OnBackPressed()
        {
            var confirmBackPress = true;
            if (CurrentMode == Mode.Orders)
            {
                var ordersFragment = FragmentManager.FindFragmentByTag<OrdersListFragment>(ORDER_LIST_FRAGMENT_TAG);
                confirmBackPress = ordersFragment != null && !ordersFragment.UseSearchResults;
            }

            if (confirmBackPress)
            {
                var logOutDialog = new LogOutConfirmUserFragment();
                var transaction = FragmentManager.BeginTransaction();
                logOutDialog.Show(transaction, LogOutConfirmUserFragment.LOGOUT_CONFIRM_FRAGMENT_TAG);
            }
            else
                base.OnBackPressed();
        }

        protected void SetDetailsFragmentVisibility(Fragment detailsFragment)
        {
            if (detailsFragment != null && _textViewNoOrder != null)
            {
                if (CurrentMode == Mode.Orders)
                    _textViewNoOrder.Visibility = _orderViewModel.ActiveOrder != null ? ViewStates.Gone : ViewStates.Visible;
                else
                    _textViewNoOrder.Visibility = _batchViewModel.ActiveBatch != null ? ViewStates.Gone : ViewStates.Visible;
                _textViewNoOrder.Text = string.Format("No {0} selected", CurrentMode.ToString());
                if (detailsFragment != null)
                {
                    if (_textViewNoOrder.Visibility == ViewStates.Gone)
                        ShowDetailsFragment(detailsFragment);
                    else
                        HideDetailsFragment(detailsFragment);
                }
            }
        }

        private void ShowDetailsFragment(Fragment detailsFragment)
        {
            var transaction = FragmentManager.BeginTransaction();
            transaction.SetTransition(FragmentTransit.FragmentFade);
            transaction.Show(detailsFragment);
            transaction.Commit();
        }

        private void HideDetailsFragment(Fragment detailsFragment)
        {
            var transaction = FragmentManager.BeginTransaction();
            transaction.SetTransition(FragmentTransit.FragmentFade);
            transaction.Hide(detailsFragment);
            transaction.Commit();
        }

        private void OrderViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveOrder" && CurrentMode == Mode.Orders)
            {
                var detailsFragment = FragmentManager.FindFragmentByTag(ORDER_DETAILS_FRAGMENT_TAG);
                if (detailsFragment != null)
                    SetDetailsFragmentVisibility(detailsFragment);
                InvalidateOptionsMenu();
            }
        }

        private void BatchViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveBatch" && CurrentMode == Mode.Batches)
            {
                var detailsFragment = FragmentManager.FindFragmentByTag(BATCH_DETAILS_FRAGMENT_TAG);
                if (detailsFragment != null)
                    SetDetailsFragmentVisibility(detailsFragment);
                InvalidateOptionsMenu();
            }
        }

        /// <summary>
        /// Initiates a barcode scan for order.
        /// </summary>
        private async Task OnActionScanForOrderAsync()
        {
            MobileBarcodeScanner.Initialize(Application);
            var scanner = new MobileBarcodeScanner();
            scanner.UseCustomOverlay = false;
            scanner.TopText = "Hold the camera up to the barcode\nAbout 6 inches away";
            scanner.BottomText = "Wait for the barcode to automatically scan!";
            var options = new MobileBarcodeScanningOptions
            {
                PossibleFormats = new List<ZX.BarcodeFormat> { ZX.BarcodeFormat.CODE_128 }
            };
            var result = await scanner.Scan(options);


            if (result != null && result.Text != null)
            {
                LogInfo(message: "OrdersActivity:ScanForOrder");
                var orderId = GetOrderIdFromScan(result.Text);
                if (orderId > -1)
                {
                    var intent = new Intent(this, typeof(SearchableOrderActivity));
                    intent.SetAction(Intent.ActionSearch);
                    intent.PutExtra(SearchManager.Query, orderId.ToString());
                    StartActivity(intent);
                }
                else
                {
                    Toast.MakeText(this, text: "Invalid barcode.", duration: ToastLength.Short)
                    .Show();
                }
            }
            else
            {
                Toast.MakeText(this, text:"Unable to find barcode.", duration:ToastLength.Short)
                    .Show();
            }
        }

        /// <summary>
        /// Gets the order identifier property from a scanned barcode value.
        /// </summary>
        /// <param name="scannedValue">The scanned value.</param>
        /// <returns></returns>
        public int GetOrderIdFromScan(string scannedValue)
        {
            int orderId = -1;
            if (Int32.TryParse(scannedValue.Replace("~", ""), out orderId))
                return orderId;
            else
                return -1;
        }

        /// <summary>
        /// Same as <c><see cref="M:Android.Content.Context.StartActivity(Android.Content.Intent, Android.OS.Bundle)" /></c> with no options
        /// specified.
        /// </summary>
        /// <param name="intent">The description of the activity to start.</param>
        public override void StartActivity(Intent intent)
        {
            // check if search intent
            if (intent != null && Intent.ActionSearch.Equals(intent.Action))
            {
                var isAdvanced = _isSearchAdvanced.HasValue ? _isSearchAdvanced.Value : false;
                intent.PutExtra(BUNDLEID_IS_SEARCH_ADVANCED, isAdvanced);
                _isSearchAdvanced = null;
            }

            base.StartActivity(intent);
        }

        #endregion

        #region TabSelectListener

        private sealed class TabSelectListener : Java.Lang.Object, TabLayout.IOnTabSelectedListener
        {
            #region Properties

            public MainActivity Instance
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public TabSelectListener(MainActivity instance)
            {
                Instance = instance;
            }

            #endregion

            #region TabLayout.IOnTabSelectedListener Members

            public void OnTabReselected(TabLayout.Tab tab)
            {
                // Do nothing
            }

            public async void OnTabSelected(TabLayout.Tab tab)
            {
                Instance._selectedTab = tab.Position;
                switch (tab.Position)
                {
                    case TAB_POSITION_ORDERS:
                        var orderTransaction = Instance.FragmentManager.BeginTransaction();
                        await Instance.AddOrderFragments(orderTransaction);
                        orderTransaction.Commit();
                        break;
                    case TAB_POSITION_BATCH:
                        var transaction = Instance.FragmentManager.BeginTransaction();
                        await Instance.AddBatchFragments(transaction);
                        transaction.Commit();
                        break;
                    default:
                        break;
                }
            }

            public void OnTabUnselected(TabLayout.Tab tab)
            {
                switch (tab.Position)
                {
                    case TAB_POSITION_ORDERS:
                        var orderTransaction = Instance.FragmentManager.BeginTransaction();
                        Instance.RemoveOrderFragments(orderTransaction);
                        orderTransaction.Commit();
                        break;
                    case TAB_POSITION_BATCH:
                        var transaction = Instance.FragmentManager.BeginTransaction();
                        Instance.RemoveBatchFragments(transaction);
                        transaction.Commit();
                        break;
                    default:
                        break;
                }
            }

            #endregion
        }

        #endregion
    }
}