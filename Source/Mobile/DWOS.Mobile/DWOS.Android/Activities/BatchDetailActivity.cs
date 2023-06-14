using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DWOS.ViewModels;
using DWOS.Utilities;
using System.Threading.Tasks;

namespace DWOS.Android
{
    /// <summary>
    /// Activity that shows details for a batch.
    /// </summary>
    [Activity(Label = "Batch Details", Name = "dwos.android.BatchDetailActivity")]
    public class BatchDetailActivity : BaseActivity, ICheckInDialogCallback
    {
        #region Fields

        public const string INTENT_BATCHID = "BatchId";
        const string BUNDLE_BATCHID = "BatchId";
        const string _detailsFragmentTag = "BatchDetailsHostFragment";

        int _batchId;
        BatchViewModel _batchViewModel;
        LogInViewModel _loginViewModel;
        private Timer _refreshTimer;

        #endregion

        #region Methods

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);

                _batchViewModel = ServiceContainer.Resolve<BatchViewModel>();
                _loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
                SupportActionBar.SetDisplayHomeAsUpEnabled(showHomeAsUp: true);
                HandleIntent(Intent);
                SetContentView(Resource.Layout.OrderDetailsLayout);

                SupportActionBar.Subtitle = GetString(Resource.String.LoggedInFormat,
                    _loginViewModel.UserProfile.Name);

                if (bundle != null)
                    _batchId = bundle.GetInt(BUNDLE_BATCHID, -1);
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

        protected override void OnResume()
        {
            base.OnResume();
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
                _batchId = intent.GetIntExtra(INTENT_BATCHID, defaultValue: -1);
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
            if (!_batchViewModel.IsBusy)
            {
                RunOnUiThread(() =>
                {
                    var task = RefreshAsync();
                });
            }
        }

        private void LoadDetailsFragment()
        {
            var fragment = new BatchDetailsHostFragment();
            var bundle = new Bundle();
            bundle.PutInt(BatchDetailsHostFragment.BUNDLEID_BATCHID, _batchId);
            fragment.Arguments = bundle;
            var transaction = FragmentManager.BeginTransaction();
            transaction.SetTransition(FragmentTransit.FragmentOpen);
            transaction.Add(Resource.Id.orderDetailsContainer, fragment, _detailsFragmentTag);
            transaction.Commit();
        }

        /// <summary>
        /// Refreshes the Orders for both Details and Summaries and Invalidates the Action Bar.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> RefreshAsync()
        {
            if (_batchViewModel.ActiveBatch != null)
            {
                await UpdateDetailsFragmentAsync(_batchViewModel.ActiveBatch.BatchId);
                return true;
            }
            else
                return false;
        }

        private async Task<bool> UpdateDetailsFragmentAsync(int batchId)
        {
            var detailsHostFragment = FragmentManager.FindFragmentByTag<BatchDetailsHostFragment>(_detailsFragmentTag);
            if (detailsHostFragment != null)
            {
                await detailsHostFragment.LoadBatchAsync(batchId);
                return true;
            }
            else
                return false;
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
            orderProcessing.SetVisible(visible: false);
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
                var optionItemException = new Exception("Error in BatchDetailActivity.OnOptionsItemSelected: " + item.ItemId, exception);
                logService.LogError(_standardErrorMessage, optionItemException, this, true);
                return base.OnOptionsItemSelected(item);
            }
        }

        private bool OnActionCheckInSelected()
        {
            if (_loginViewModel.IsLicenseActivated)
            {
                var transaction = FragmentManager.BeginTransaction();
                var bundle = new Bundle();
                bundle.PutString(CheckInFragment.BUNDLE_EXTRA_MODE, Mode.Batches.ToString());
                var dialog = new CheckInFragment();
                dialog.Arguments = bundle;
                dialog.Show(transaction, CheckInFragment.CHECKIN_FRAGMENT_TAG);
            }
            else
                LogOutUserWithExpiredMessage();

            return true;
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

        public async void CheckInDialogDismissed(CheckInFragment dialog, bool result, ViewModels.CheckInViewModel checkInViewModel)
        {
            dialog.Dismiss();
            if (result == true)
            {
                var checkInResult = await _batchViewModel.CheckInBatchAsync(checkInViewModel);

                if (checkInResult.Success && string.IsNullOrEmpty(checkInResult.ErrorMessage))
                {
                    var message = string.Format("Batch: {0} checked in.", checkInViewModel.OrderId);
                    Toast.MakeText(this, message, ToastLength.Short).Show();
                    await RefreshAsync();
                }
                else
                {
                    var message = string.Format("Unable to check in Batch: {0}. {1}", checkInViewModel.OrderId, checkInResult.ErrorMessage);
                    Toast.MakeText(this, message, ToastLength.Short).Show();
                }
            }
        }

        #endregion
    }
}