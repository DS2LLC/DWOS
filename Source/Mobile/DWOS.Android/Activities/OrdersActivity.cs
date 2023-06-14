
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using DWOS.Utilities;
using DWOS.ViewModels;
using DWOS;
using System.Threading.Tasks;
using System.Threading;
using DWOS.Services.Messages;
using Newtonsoft.Json;
using ZXing.Mobile;

namespace DWOS.Android
{
	/// <summary>
	/// Orders activity provides the UI coordination for Orders and Order Detail.
	/// </summary>
	[Activity (Label = "DWOS")]
    [MetaData("android.app.default_searchable", Value = "dwos.android.SearchableOrderActivity")]
    public class OrdersActivity : BaseActivity, IOrdersFragmentCallback, ICheckInDialogCallback, 
        IFilterFragmentCallback, IProcessDetailsFragmentCallback
	{
		#region Fields

		protected const string _detailsFragmentTag = "DetailsHostFragment";
        protected const string _ordersFragmentTag = "OrdersFragment";
        
        OrderViewModel _orderViewModel;
        LogInViewModel _loginViewModel;
        const int _orderProcessingRequestCode = 1;
        const int _controlInspectionRequestCode = 2;
        TextView _textViewNoOrder;
        
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersActivity"/> class.
        /// </summary>
        public OrdersActivity()
        {
           
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _orderViewModel = ServiceContainer.Resolve<OrderViewModel>();
            _loginViewModel = ServiceContainer.Resolve<LogInViewModel>();

            SetContentView(Resource.Layout.OrdersLayout);
            _textViewNoOrder = FindViewById<TextView>(Resource.Id.textViewNoOrder);
            
            if (bundle == null)
            {
                var transaction = FragmentManager.BeginTransaction();
                var ordersFragment = new OrdersFragment();
                var inflateDetailsFragment = FindViewById<FrameLayout>(Resource.Id.frameLayoutOrderDetails) != null ? true : false;
                transaction.SetTransition(FragmentTransit.FragmentOpen);
                transaction.Replace(Resource.Id.frameLayoutOrders, ordersFragment, _ordersFragmentTag);
                if (inflateDetailsFragment)
                {
                    var detailsHostFragment = new OrderDetailsHostFragment();
                    detailsHostFragment.Arguments = bundle;
                    transaction.Replace(Resource.Id.frameLayoutOrderDetails, detailsHostFragment, _detailsFragmentTag);
                }
                transaction.Commit();
            }
        }

        protected override void OnResume()
        {
            var detailsFragment = FragmentManager.FindFragmentByTag<OrderDetailsHostFragment>(_detailsFragmentTag);
            SetActiveOrderVisibilityForFragments(detailsFragment);
            base.OnResume();
        }

        protected override void OnDestroy()
        {
            _textViewNoOrder.Dispose();

            base.OnDestroy();
        }

        /// <summary>
        /// Called when an activity you launched exits, giving you the requestCode
        /// you started it with, the resultCode it returned, and any additional
        /// data from it.
        /// </summary>
        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == _orderProcessingRequestCode && resultCode == Result.Ok)
                await OrderProcessSavedAsync(data);
            else if (requestCode == _controlInspectionRequestCode && resultCode == Result.Ok)
                await ControlInspectionCompletedAsync(data, resultCode);
            else
                base.OnActivityResult(requestCode, resultCode, data);
        }

        /// <summary>
        /// Initialize the contents of the Activity's standard options menu.
        /// </summary>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.order_menu, menu);
            SetOrderProcessMenuVisibility(menu);
            SetOrderCheckInMenuVisibility(menu);
            SetControlInspectionMenuVisibility(menu);
            SetSearchOptions(menu);
            return true;           
        }

        private void SetSearchOptions(IMenu menu)
        {
            var searchManager = GetSystemService(Context.SearchService) as SearchManager;
            var searchableInfo = searchManager.GetSearchableInfo(ComponentName);
            var searchView = (SearchView)menu.FindItem(Resource.Id.action_search).ActionView;
            searchView.SetSearchableInfo(searchableInfo);
        }

        private void SetOrderCheckInMenuVisibility(IMenu menu)
        {
            var isVisible = _loginViewModel.UserProfile.SecurityRoles.Contains(ApplicationSettings.Settings.PartCheckInRole);
            var checkinMenu = menu.FindItem(Resource.Id.action_checkin);
            checkinMenu.SetVisible(isVisible);
        }

        private void SetControlInspectionMenuVisibility(IMenu menu)
        {
            var isVisible = false;
            var isInRole = _loginViewModel.UserProfile.SecurityRoles.Contains(ApplicationSettings.Settings.ControlInspectionRole);
            var detailsFragment = FragmentManager.FindFragmentByTag<OrderDetailsHostFragment>(_detailsFragmentTag);
            var detailsFragmentVisible = detailsFragment != null && !detailsFragment.IsHidden;
            if (isInRole && detailsFragmentVisible && _orderViewModel.ActiveOrder != null)
                isVisible = _orderViewModel.ActiveOrder.WorkStatus == ApplicationSettings.Settings.WorkStatusPendingInspection;
            var controlInspectionMenu = menu.FindItem(Resource.Id.action_controlInspection);
            controlInspectionMenu.SetVisible(isVisible);
        }

        void SetOrderProcessMenuVisibility(IMenu menu)
        {
            var isVisible = false;
            var isInRole = _loginViewModel.UserProfile.SecurityRoles.Contains(ApplicationSettings.Settings.OrderProcessingRole);
            var detailsFragment = FragmentManager.FindFragmentByTag<OrderDetailsHostFragment>(_detailsFragmentTag);
            var detailsFragmentVisible = detailsFragment != null && !detailsFragment.IsHidden;
            if (isInRole && detailsFragmentVisible && _orderViewModel.ActiveOrder != null)
                isVisible = _orderViewModel.ActiveOrder.WorkStatus == ApplicationSettings.Settings.WorkStatusInProcess;
            var orderProcessing = menu.FindItem(Resource.Id.action_orderprocessing);
            orderProcessing.SetVisible(isVisible);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
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
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private bool OnActionControlInspection()
        {
            if (_loginViewModel.IsLicenseActivated)
            {
                var intent = new Intent(this, typeof(InspectionActivity));
                intent.PutExtra(InspectionActivity.INTENT_ORDERID, _orderViewModel.ActiveOrder.OrderId);
                StartActivityForResult(intent, _controlInspectionRequestCode);
            }
            else
                LogOutUserWithExpiredMessage();

            return true;
        }

        private bool OnActionFilter()
        {
            var transaction = FragmentManager.BeginTransaction();
            var filterFragment = new FilterFragment(_orderViewModel.FilterDepartmentValue, _orderViewModel.FilterStatusValue);
            filterFragment.Show(transaction, FilterFragment.FILTER_FRAGMENT_TAG);
            
            return true;
        }

        /// <summary>
        /// Refreshes the Orders for both Details and Summaries and Invalidates the Action Bar.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> RefreshAsync()
        {
            if (_orderViewModel.ActiveOrder != null)
                await UpdateOrdersAndOrderDetailsFragmentsAsync();
            else
                await UpdateOrdersFragmentAsync();
            
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
            //ONLY WHEN WE WANT TO TEST GARBAGE COLLECTION
            //GC.Collect();
            logInViewModel.Logout();
            
            return true;
        }

        private bool OnActionOrderProcessing()
        {
            if (_loginViewModel.IsLicenseActivated)
            {
                var intent = new Intent(this, typeof(OrderProcessActivity));
                intent.PutExtra(OrderProcessActivity.INTENT_ORDERID, _orderViewModel.ActiveOrder.OrderId);
                StartActivityForResult(intent, _orderProcessingRequestCode);
                LogInfo(message: "Order Processing via OrdersActivity:OnActionOrderProcessing");
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
                var dialog = new CheckInFragment();
                dialog.Show(transaction, CheckInFragment.CHECKIN_FRAGMENT_TAG);
            }
            else
                LogOutUserWithExpiredMessage();

            return true;
        }

        public async void OnOrderSelected (int orderId)
		{
            var success = await UpdateDetailsFragmentAsync(orderId);
            if (success)
                InvalidateOptionsMenu();
            else
            {
                var detailsIntent = new Intent(this, typeof(OrderDetailActivity));
                detailsIntent.PutExtra(OrderDetailActivity.INTENT_ORDERID, orderId);
                StartActivity(detailsIntent);
            }
        }

        private async Task UpdateOrdersAndOrderDetailsFragmentsAsync()
        {
            var detailsFragment = FragmentManager.FindFragmentByTag<OrderDetailsHostFragment>(_detailsFragmentTag);
            if (detailsFragment != null)
                await detailsFragment.LoadOrderAsync(_orderViewModel.ActiveOrder.OrderId);

            var ordersFragment = FragmentManager.FindFragmentByTag<OrdersFragment>(_ordersFragmentTag);
            if (ordersFragment != null)
            {
                await ordersFragment.LoadOrderSummariesAsync();
                if (_orderViewModel.ActiveOrder != null)
                    ordersFragment.SetSelectedOrder(_orderViewModel.ActiveOrder.OrderId);
                else
                    ordersFragment.SetSelectedOrder(orderId: -1);
            }

            if (detailsFragment != null)
            {
                if (_orderViewModel.ActiveOrder != null)
                    await detailsFragment.LoadOrderAsync(_orderViewModel.ActiveOrder.OrderId);
                SetActiveOrderVisibilityForFragments(detailsFragment);
            }
        }

        private async Task UpdateOrdersFragmentAsync()
        {
            var ordersFragment = FragmentManager.FindFragmentByTag<OrdersFragment>(_ordersFragmentTag);
            if (ordersFragment != null)
            {
                await ordersFragment.LoadOrderSummariesAsync();
                if (_orderViewModel.ActiveOrder != null)
                    ordersFragment.SetSelectedOrder(_orderViewModel.ActiveOrder.OrderId);
                else
                    ordersFragment.SetSelectedOrder(orderId: -1);
            }
        }

        private async Task<bool> UpdateDetailsFragmentAsync(int orderId)
        {
            var detailsFragment = FragmentManager.FindFragmentByTag<OrderDetailsHostFragment>(_detailsFragmentTag);
            if (detailsFragment != null)
            {
                await detailsFragment.LoadOrderAsync(orderId);
                SetActiveOrderVisibilityForFragments(detailsFragment);
                return true;
            }
            else
                return false;
        }

        public async void CheckInDialogDismissed(CheckInFragment dialog, bool result, ViewModels.CheckInViewModel checkInViewModel)
        {
            dialog.Dismiss();
            if (result == true)
            {
                var checkInResult = await _orderViewModel.CheckInOrderAsync(checkInViewModel);

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
            var orderId = resultIntent.GetIntExtra(OrderProcessActivity.INTENT_ORDERID, defaultValue: -1);
            var errorMessage = resultIntent.GetStringExtra(OrderProcessActivity.INTENT_ORDERPROCESS_ERROR);
            if (string.IsNullOrEmpty(errorMessage))
            {
                var message = string.Format("Order Processing for Order: {0} updated.", orderId);
                Toast.MakeText(this, message, ToastLength.Short)
                    .Show();
                await RefreshAsync();
            }
            else
            {
                var message = string.Format("Unable save Processing Answers for order: {0}.", orderId);
                Toast.MakeText(this, message, ToastLength.Short)
                    .Show();
            }
        }

        private async Task ControlInspectionCompletedAsync(Intent resultIntent, Result resultCode)
        {
            var orderId = resultIntent.GetIntExtra(InspectionActivity.INTENT_ORDERID, defaultValue: -1);
            var error = resultIntent.GetStringExtra(InspectionActivity.INTENT_ERROR);

            if (string.IsNullOrEmpty(error))
            {
                var message = string.Format("Control Inspection for Order: {0} updated.", orderId);
                Toast.MakeText(this, message, ToastLength.Short)
                    .Show();
                await RefreshAsync();
            }
            else
            {
                var message = string.Format("Control Inspection for Order: {0} failed." + System.Environment.NewLine + "{1}", orderId, error);
                Toast.MakeText(this, message, ToastLength.Short)
                    .Show();
            }
        }

        private void LogOutUserWithExpiredMessage()
        {
            var alertDialog = new LogOutUserFragment();
            var transaction = FragmentManager.BeginTransaction();
            alertDialog.Show(transaction, LogOutUserFragment.LOGOUT_FRAGMENT_TAG);
        }

        /// <summary>
        /// Called when it is time to register to view model events.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override void RegisterViewModelEvents()
        {
            _orderViewModel.PropertyChanged += OrderViewModel_PropertyChanged;
        }

        protected override void UnregisterViewModelEvents()
        {
            _orderViewModel.PropertyChanged -= OrderViewModel_PropertyChanged;
        }

        /// <summary>
        /// Called when the Timer callback is executed.
        /// </summary>
        /// <param name="stateObject"></param>
        protected override void OnTimerCallback(object stateObject)
        {
            base.OnTimerCallback(stateObject);
            if (!_orderViewModel.IsBusy)
            {
                RunOnUiThread(() =>
                {
                    var task = RefreshAsync();
                });
            }
        }

        public async void OnFilterDimissed(FilterFragment dialog, bool result, string department, string status)
        {
            dialog.Dismiss();

            if (result == true)
            {
                _orderViewModel.FilterDepartmentValue = department;
                _orderViewModel.FilterStatusValue = status;
               
                await RefreshAsync();
            }
        }

        public void OnProcessDetailsSelected(OrderProcessInfo selectedProcessInfo)
        {
            var intent = new Intent(this, typeof(OrderProcessActivity));
            var orderProcessJson = JsonConvert.SerializeObject(selectedProcessInfo);
            intent.PutExtra(OrderProcessActivity.INTENT_ORDERPROCESSINFO, orderProcessJson);
            StartActivityForResult(intent, _orderProcessingRequestCode);
        }

        public override void OnBackPressed()
        {
            var ordersFragment = FragmentManager.FindFragmentByTag<OrdersFragment>(_ordersFragmentTag);
            if (ordersFragment != null && !ordersFragment.UseSearchResults)
            {
                var logOutDialog = new LogOutConfirmUserFragment();
                var transaction = FragmentManager.BeginTransaction();
                logOutDialog.Show(transaction, LogOutConfirmUserFragment.LOGOUT_CONFIRM_FRAGMENT_TAG);
            }
            else
                base.OnBackPressed();
        }

        protected void SetActiveOrderVisibilityForFragments(OrderDetailsHostFragment detailsFragment)
        {
            _textViewNoOrder.Visibility = _orderViewModel.ActiveOrder != null ? ViewStates.Gone : ViewStates.Visible;
            if (detailsFragment != null)
            {
                if (_textViewNoOrder.Visibility == ViewStates.Gone)
                    ShowDetailsFragment(detailsFragment);
                else
                    HideDetailsFragment(detailsFragment);
            }
        }

        private void ShowDetailsFragment(OrderDetailsHostFragment detailsFragment)
        {
            if (detailsFragment.IsHidden)
            {
                var transaction = FragmentManager.BeginTransaction();
                transaction.SetTransition(FragmentTransit.FragmentFade);
                transaction.Show(detailsFragment);
                transaction.Commit();
            }
        }

        private void HideDetailsFragment(OrderDetailsHostFragment detailsFragment)
        {
            if (!detailsFragment.IsHidden)
            {
                var transaction = FragmentManager.BeginTransaction();
                transaction.SetTransition(FragmentTransit.FragmentFade);
                transaction.Hide(detailsFragment);
                transaction.Commit();
            }
        }

        private void OrderViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveOrder")
            {
                var detailsFragment = FragmentManager.FindFragmentByTag<OrderDetailsHostFragment>(_detailsFragmentTag);
                if (detailsFragment != null)
                    SetActiveOrderVisibilityForFragments(detailsFragment);
            }
        }

        /// <summary>
        /// Initiates a barcode scan for order.
        /// </summary>
        private async Task OnActionScanForOrderAsync()
        {
            var scanner = new MobileBarcodeScanner(this);
            scanner.UseCustomOverlay = false;
            scanner.TopText = "Hold the camera up to the barcode\nAbout 6 inches away";
            scanner.BottomText = "Wait for the barcode to automatically scan!";
            var result = await scanner.Scan();
            

            if (result != null && result.Text != null)
            {
                LogInfo(message: "OrdersActivity:ScanForOrder Executed");
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

        #endregion
    }
}