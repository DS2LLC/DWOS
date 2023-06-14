using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using DWOS.ViewModels;
using DWOS.Utilities;
using DWOS.Services.Messages;
using Newtonsoft.Json;

namespace DWOS.Android
{
    /// <summary>
    /// Activity that shows details for an order.
    /// </summary>
    [Activity(Label = "Order Details", Name = "dwos.android.OrderDetailActivity")]
    public class OrderDetailActivity : BaseActivity, ICheckInDialogCallback, IDocumentSelectionCallback, IOrderNotesFragmentCallback
    {
        #region Fields

        public const string INTENT_ORDERID = "OrderId";
        const string BUNDLE_ORDERID = "OrderId";
        const string _detailsFragmentTag = "DetailsHostFragment";
        
        int _orderId;
        OrderViewModel _orderViewModel;
        LogInViewModel _loginViewModel;
        private Timer _refreshTimer;
        const int _orderProcessingRequestCode = 1;
        const int _controlInspectionRequestCode = 2;
        const int _orderNoteRequestCode = 3;

        #endregion

        #region Methods

        /// <summary>
        /// Called when [create].
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        protected override void OnCreate (Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);

                _orderViewModel = ServiceContainer.Resolve<OrderViewModel>();
                _loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
                SupportActionBar.SetDisplayHomeAsUpEnabled(showHomeAsUp: true);
                HandleIntent(Intent);
                SetContentView(Resource.Layout.OrderDetailsLayout);

                SupportActionBar.Subtitle = GetString(Resource.String.LoggedInFormat,
                    _loginViewModel.UserProfile.Name);

                if (bundle != null)
                    _orderId = bundle.GetInt(BUNDLE_ORDERID, -1);
                else
                    LoadDetailsFragment();
            }
            catch (Exception exception)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error in OnCreate", exception);
                DWOSApplication.Current.RestoreMainActivityFromCrash = true;
                base.RestartApp();
            }
        }

        protected override async void OnResume()
        {
            base.OnResume();

            if (_orderViewModel.ActiveOrder?.OrderId != _orderId)
            {
                await RefreshAsync();
            }

            StartRefreshTimer();
        }

        protected override void OnPause()
        {
            base.OnPause();
            StopRefreshTimer();
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

        private void HandleIntent(Intent intent)
        {
            if (intent != null)
                _orderId = intent.GetIntExtra(INTENT_ORDERID, defaultValue: -1);
        }

        protected override void RegisterViewModelEvents()
        {
            //Nothing to wire up to
        }

        protected override void UnregisterViewModelEvents()
        {
            //Nothing to wire up to
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

        private void LoadDetailsFragment()
        {
            var fragment = new OrderDetailsHostFragment();
            var bundle = new Bundle();
            bundle.PutInt(OrderDetailsHostFragment.BUNDLEID_ORDERID, _orderId);
            fragment.Arguments = bundle;
            var transaction = FragmentManager.BeginTransaction();
            transaction.SetTransition(FragmentTransit.FragmentOpen);
            transaction.Add(Resource.Id.orderDetailsContainer, fragment, _detailsFragmentTag);
            transaction.Commit();
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
            SetFilterMenu(menu);
            return true;
        }

        private void SetFilterMenu(IMenu menu)
        {
            var filterMenu = menu.FindItem(Resource.Id.action_filter);
            filterMenu.SetVisible(false);
        }

        private void SetSearchOptions(IMenu menu)
        {
            menu.FindItem(Resource.Id.action_search)
                .SetVisible(false);

            menu.FindItem(Resource.Id.action_searchAdvanced)
                .SetVisible(false);
        }

        private void SetOrderCheckInMenuVisibility(IMenu menu)
        {
            var isVisible = _loginViewModel.UserProfile.SecurityRoles.Contains(ApplicationSettings.Settings.PartCheckInRole);
            var checkinMenu = menu.FindItem(Resource.Id.action_checkin);
            checkinMenu.SetVisible(isVisible);
        }

        private void SetControlInspectionMenuVisibility(IMenu menu)
        {
            var controlInspectionMenu = menu.FindItem(Resource.Id.action_controlInspection);
            controlInspectionMenu.SetVisible(visible: false);
        }

        private void SetOrderProcessMenuVisibility(IMenu menu)
        {
            var orderProcessing = menu.FindItem(Resource.Id.action_orderprocessing);
            orderProcessing.SetVisible(visible:false);
        }

        /// <summary>
        /// This hook is called whenever an item in your options menu is selected.
        /// </summary>
        /// <param name="item">The menu item that was selected.</param>
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
                    case Resource.Id.action_controlInspection:
                        return OnActionControlInspection();
                    case global::Android.Resource.Id.Home:
                        Finish();
                        return true;
                    default:
                        return base.OnOptionsItemSelected(item);
                }
            }
            catch (Exception exception)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                var optionItemException = new Exception("Error in OrderDetailActivity.OnOptionsItemSelected: " + item.ItemId, exception);
                logService.LogError(_standardErrorMessage, optionItemException, this, true);
                return base.OnOptionsItemSelected(item);
            }
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
            else if (requestCode == _orderNoteRequestCode && resultCode == Result.Ok)
                await OrderNoteSavedAsync(data);
            else
                base.OnActivityResult(requestCode, resultCode, data);
        }

        private bool OnActionCheckInSelected()
        {
            if (_loginViewModel.IsLicenseActivated)
            {
                var transaction = FragmentManager.BeginTransaction();
                var bundle = new Bundle();
                bundle.PutString(CheckInFragment.BUNDLE_EXTRA_MODE, Mode.Orders.ToString());
                var dialog = new CheckInFragment();
                dialog.Arguments = bundle;
                dialog.Show(transaction, CheckInFragment.CHECKIN_FRAGMENT_TAG);
            }
            else
                LogOutUserWithExpiredMessage();

            return true;
        }

        private bool OnActionOrderProcessing()
        {
            if (_loginViewModel.IsLicenseActivated)
            {
                var intent = new Intent(this, typeof(OrderProcessActivity));
                intent.PutExtra(OrderProcessActivity.INTENT_ORDERID, _orderViewModel.ActiveOrder.OrderId);
                StartActivityForResult(intent, _orderProcessingRequestCode);
            }
            else
                LogOutUserWithExpiredMessage();

            return true;
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

        public async void CheckInDialogDismissed(CheckInFragment dialog, bool result, ViewModels.CheckInViewModel checkInViewModel)
        {
            dialog.Dismiss();
            if (result == true)
            {
                var checkInResult = await _orderViewModel.CheckInOrderAsync(checkInViewModel);

                if (checkInResult.Success && string.IsNullOrEmpty(checkInResult.ErrorMessage))
                {
                    var message = string.Format("Order: {0} checked in.", checkInViewModel.OrderId);
                    Toast.MakeText(this, message, ToastLength.Short).Show();
                    await RefreshAsync();
                }
                else
                {
                    var message = string.Format("Unable to check in Order: {0}. {1}", checkInViewModel.OrderId, checkInResult.ErrorMessage);
                    Toast.MakeText(this, message, ToastLength.Short).Show();
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
                Toast.MakeText(this, message, ToastLength.Short).Show();
                await RefreshAsync();
            }
            else
            {
                var message = string.Format("Unable save Processing Answers for order: {0}.", orderId);
                Toast.MakeText(this, message, ToastLength.Short).Show();
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

        private bool OnActionLogout()
        {
            var logInViewModel = ServiceContainer.Resolve<LogInViewModel>();
            logInViewModel.Logout();

            return true;
        }

        private bool OnActionAbout()
        {
            var transaction = FragmentManager.BeginTransaction();
            var aboutDialog = new AboutFragment();
            aboutDialog.Show(transaction, AboutFragment.ABOUT_FRAGMENT_TAG);

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

        /// <summary>
        /// Refreshes the Orders for both Details and Summaries and Invalidates the Action Bar.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> RefreshAsync()
        {
            if (_orderId > 0)
            {
                await UpdateDetailsFragmentAsync(_orderId);
                return true;
            }
            else
                return false;
        }

        private async Task<bool> UpdateDetailsFragmentAsync(int orderId)
        {
            var detailsHostFragment = FragmentManager.FindFragmentByTag<OrderDetailsHostFragment>(_detailsFragmentTag);
            if (detailsHostFragment != null)
            {
                await detailsHostFragment.LoadOrderAsync(orderId);
                return true;
            }
            else
                return false;
        }

        #endregion
    }
}

