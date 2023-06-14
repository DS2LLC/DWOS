using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DWOS.ViewModels;
using DWOS.Utilities;
using System.Threading.Tasks;
using DWOS.Services.Messages;
using Newtonsoft.Json;
using DWOS.Services;
using Android.Support.V4.App;
using AndroidHUD;
using DWOS.Android.Adapters;

namespace DWOS.Android
{
    /// <summary>
    /// Activity that allows a user to process an order or batch.
    /// </summary>
    [Activity(Label = "Order Processing", Name = "dwos.android.OrderProcessActivity", ParentActivity = typeof(MainActivity))]
    public class OrderProcessActivity : BaseActivity, IOrderProcessStepListCallback, IOrderProcessQuestionsHostFragmentCallback, IDocumentSelectionCallback
    {
        #region Fields

        public const string BUNDLEID_READONLY = "ReadOnly";
        public const string BUNDLEID_DISPLAYHUD = "DisplayHud";
        public const string INTENT_MODE = "Mode";
        public const string INTENT_ORDERID = "OrderId";
        public const string INTENT_BATCHID = "BatchId";
        public const string INTENT_ORDERPROCESSINFO = "OrderProcessInfo";
        public const string INTENT_BATCHPROCESSINFO = "BatchProcessInfo";
        public const string INTENT_ORDERPROCESS_ERROR = "OrderProcessError";

        const string SavingOrderHUDMessage = "Saving order process";
        const string SavingBatchHUDMessage = "Saving batch process";
        ProcessViewModel _orderProcessViewModel;
        BatchProcessViewModel _batchProcessViewModel;
        LogInViewModel _loginViewModel;
        ProgressBar _loadingProgressBar;
        LinearLayout _rootLayout;
        TextView _readOnlyTextView;
        Spinner _documentsSpinner;
        Button _startLaborTimerButton;
        Button _pauseLaborTimerButton;
        Button _stopLaborTimerButton;
        bool _isReadOnly;
        Mode CurrentMode = Mode.Orders;
        bool _hudVisible = false;

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the order identifier.
        /// </summary>
        /// <value>
        /// The order identifier.
        /// </value>
        public int OrderID { get; private set; }

        /// <summary>
        /// Gets the batch identifier.
        /// </summary>
        /// <value>
        /// The order identifier.
        /// </value>
        public int BatchID { get; private set; }

        /// <summary>
        /// Gets the order process information.
        /// </summary>
        /// <value>
        /// The order process information.
        /// </value>
        public OrderProcessInfo OrderProcessInfo { get; private set; }

        /// <summary>
        /// Gets the batch process information.
        /// </summary>
        /// <value>
        /// The order process information.
        /// </value>
        public BatchProcessInfo BatchProcessInfo { get; private set; }
        
        #endregion
        
        #region Methods

        /// <summary>
        /// Called when the activity is starting.
        /// </summary>
        /// <param name="savedInstanceState">If the activity is being re-initialized after
        /// previously being shut down then this Bundle contains the data it most
        /// recently supplied in <c><see cref="M:Android.App.Activity.OnSaveInstanceState(Android.OS.Bundle)" /></c>.  <format type="text/html"><b><i>Note: Otherwise it is null.</i></b></format></param>
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                OrderID = -1;
                _isReadOnly = false;
                _orderProcessViewModel = ServiceContainer.Resolve<ProcessViewModel>();
                _batchProcessViewModel = ServiceContainer.Resolve<BatchProcessViewModel>();
                _loginViewModel = ServiceContainer.Resolve<LogInViewModel>();

                SupportActionBar.SetDisplayHomeAsUpEnabled(showHomeAsUp: true);
                HandleIntent(Intent);
                SetContentView(Resource.Layout.OrderProcessLayout);
                _loadingProgressBar = FindViewById<ProgressBar>(Resource.Id.processProgressBar);
                _rootLayout = FindViewById<LinearLayout>(Resource.Id.processLinearLayout);
                _readOnlyTextView = FindViewById<TextView>(Resource.Id.textViewReadOnly);
                _documentsSpinner = FindViewById<Spinner>(Resource.Id.spinnerDocuments);

                _startLaborTimerButton = FindViewById<Button>(Resource.Id.startLaborButton);
                _pauseLaborTimerButton = FindViewById<Button>(Resource.Id.pauseLaborButton);
                _stopLaborTimerButton = FindViewById<Button>(Resource.Id.stopLaborButton);

                _loadingProgressBar.Visibility = ViewStates.Visible;
                _rootLayout.Visibility = ViewStates.Invisible;

                if (savedInstanceState != null)
                {
                    // Process is already loaded, so refresh the UI
                    _isReadOnly = savedInstanceState.GetBoolean(BUNDLEID_READONLY, defaultValue: false);
                    if (IsProcessPaperBased())
                        ShowPaperBasedViews();

                    _hudVisible = savedInstanceState.GetBoolean(BUNDLEID_DISPLAYHUD, defaultValue: false);
                    if (_hudVisible)
                    {
                        if (CurrentMode == Mode.Orders)
                            AndHUD.Shared.Show(this, SavingOrderHUDMessage);
                        else
                            AndHUD.Shared.Show(this, SavingBatchHUDMessage);
                    }

                    RegisterViewEvents();
                    RefreshTimerButtons();
                }
                else
                    await LoadProcessAsync();

                SetCompletedQuestionsLabel();
                PopulateDocumentsSpinner();
                Title = CurrentMode == Mode.Orders
                    ? $"Order {_orderProcessViewModel.OrderId}: {_orderProcessViewModel.Process?.Name}"
                    : $"Batch {_batchProcessViewModel.BatchId}: {_batchProcessViewModel.Process?.Name}";

                _loadingProgressBar.Visibility = ViewStates.Gone;
                _rootLayout.Visibility = ViewStates.Visible;
                _readOnlyTextView.Visibility = _isReadOnly == true ? ViewStates.Visible : ViewStates.Gone;

                using (var documentLayout = FindViewById(Resource.Id.documentLayout))
                {
                    documentLayout.Visibility = ViewStates.Visible;
                }

                // Visibility for labor buttons
                if (ApplicationSettings.Settings.UsingTimeTracking)
                {
                    using (var laborButtons = FindViewById(Resource.Id.laborButtons))
                    {
                        laborButtons.Visibility = ViewStates.Visible;
                    }

                    // Tint play/pause/stop buttons
                    var drawableTintColor = global::Android.Support.V4.Content.ContextCompat.GetColor(this, Resource.Color.textColorSecondary);
                    _startLaborTimerButton.GetCompoundDrawables()[0].CompatSetTint(drawableTintColor);
                    _pauseLaborTimerButton.GetCompoundDrawables()[0].CompatSetTint(drawableTintColor);
                    _stopLaborTimerButton.GetCompoundDrawables()[0].CompatSetTint(drawableTintColor);
                }

                SupportActionBar.Subtitle = GetString(Resource.String.LoggedInFormat,
                _loginViewModel.UserProfile.Name);
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

            // Reset spinner
            // Because OnCreate is async, OnResume can run before OnCreate finishes.
            var docSpinner = _documentsSpinner;

            if (docSpinner != null)
            {
                docSpinner.SetSelection(0);
            }
        }

        /// <summary>
        /// Processes the intent and sets up proper properties for loading a process.
        /// </summary>
        /// <param name="intent">The intent.</param>
        private void HandleIntent(Intent intent)
        {
            if (intent != null)
            {
                var mode = intent.GetStringExtra(INTENT_MODE);
                if (mode == "Orders")
                {
                    OrderID = intent.GetIntExtra(INTENT_ORDERID, defaultValue: -1);
                    if (OrderID < 0)
                    {
                        var orderProcessInfoJson = intent.GetStringExtra(INTENT_ORDERPROCESSINFO);
                        if (!string.IsNullOrEmpty(orderProcessInfoJson))
                        {
                            OrderProcessInfo = JsonConvert.DeserializeObject<OrderProcessInfo>(orderProcessInfoJson,
                                JsonSerializationSettings.Settings);
                            OrderID = OrderProcessInfo.OrderId;
                        }
                    }
                    CurrentMode = Mode.Orders;
                }
                else if (mode == "Batches")
                {
                    BatchID = intent.GetIntExtra(INTENT_BATCHID, defaultValue: -1);
                    if (BatchID < 0)
                    {
                        var batchProcessInfoJson = intent.GetStringExtra(INTENT_BATCHPROCESSINFO);
                        if (!string.IsNullOrEmpty(batchProcessInfoJson))
                        {
                            BatchProcessInfo = JsonConvert.DeserializeObject<BatchProcessInfo>(batchProcessInfoJson,
                                JsonSerializationSettings.Settings);
                            BatchID = BatchProcessInfo.BatchId;
                        }
                    }
                    CurrentMode = Mode.Batches;
                }
            }
        }

        /// <summary>
        /// Called when the fragment is no longer in use.
        /// </summary>
        protected override void OnDestroy()
        {
            if (_hudVisible)
            {
                _hudVisible = false;
                AndHUD.Shared.Dismiss();
            }

            UnregisterViewEvents();
            _loadingProgressBar.Dispose();
            _documentsSpinner.Dispose();
            _rootLayout.Dispose();
            _readOnlyTextView.Dispose();
            _startLaborTimerButton?.Dispose();
            _pauseLaborTimerButton?.Dispose();
            _stopLaborTimerButton?.Dispose();

            base.OnDestroy();
        }

        /// <summary>
        /// Called to ask the fragment to save its current dynamic state, so it
        /// can later be reconstructed in a new instance of its process is
        /// restarted.
        /// </summary>
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutBoolean(BUNDLEID_READONLY, _isReadOnly);
            if (_hudVisible)
                outState.PutBoolean(BUNDLEID_DISPLAYHUD, true);
        }

        /// <summary>
        /// This method is called after <c><see cref="M:Android.App.Activity.OnStart" /></c> when the activity is
        /// being re-initialized from a previously saved state, given here in
        /// <format type="text/html"><var>savedInstanceState</var></format>.
        /// </summary>
        /// <param name="savedInstanceState">the data most recently supplied in <c><see cref="M:Android.App.Activity.OnSaveInstanceState(Android.OS.Bundle)" /></c>.</param>
        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            _isReadOnly = savedInstanceState.GetBoolean(BUNDLEID_READONLY, defaultValue: false);
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
            MenuInflater.Inflate(Resource.Menu.order_process_menu, menu);
            return true;
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            var acceptMenu = menu.FindItem(Resource.Id.action_done_orderprocessing);
            var saveMenu = menu.FindItem(Resource.Id.action_save_orderprocessing);
            var startTimerMenu = menu.FindItem(Resource.Id.action_start_timer_orderprocessing);
            var stopTimerMenu = menu.FindItem(Resource.Id.action_stop_timer_orderprocessing);

            try
            {
                if (_isReadOnly)
                {
                    acceptMenu.SetVisible(true);
                    saveMenu.SetVisible(false);
                    startTimerMenu.SetVisible(false);
                    stopTimerMenu.SetVisible(false);
                }
                else
                {
                    acceptMenu.SetVisible(false);
                    saveMenu.SetVisible(true);
                    var isSaveEnabled = CurrentMode == Mode.Orders ? _orderProcessViewModel.IsValid && !_orderProcessViewModel.IsBusy :
                        _batchProcessViewModel.IsValid && !_batchProcessViewModel.IsBusy;
                    saveMenu.SetEnabled(isSaveEnabled, this, Resource.Drawable.ic_action_save);

                    startTimerMenu.SetVisible(ApplicationSettings.Settings.UsingTimeTracking);
                    stopTimerMenu.SetVisible(ApplicationSettings.Settings.UsingTimeTracking);

                    if (CurrentMode == Mode.Orders)
                    {
                        startTimerMenu.SetEnabled(!_orderProcessViewModel.HasActiveProcessTimer);
                        stopTimerMenu.SetEnabled(_orderProcessViewModel.HasActiveProcessTimer);
                    }
                    else if (CurrentMode == Mode.Batches)
                    {
                        startTimerMenu.SetEnabled(!_batchProcessViewModel.HasActiveProcessTimer);
                        stopTimerMenu.SetEnabled(_batchProcessViewModel.HasActiveProcessTimer);
                    }
                }

                return true;
            }
            finally
            {
                acceptMenu.Dispose();
                saveMenu.Dispose();
                startTimerMenu.Dispose();
                stopTimerMenu.Dispose();
            }
        }

        /// <summary>
        /// This hook is called whenever an item in your options menu is selected.
        /// </summary>
        /// <param name="item">The menu item that was selected.</param>
        /// <returns>
        /// To be added.
        /// </returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                switch (item.ItemId)
                {
                    case Resource.Id.action_save_orderprocessing:
                        var task = SaveProcessAsync();
                        return true;
                    case Resource.Id.action_done_orderprocessing:
                    case global::Android.Resource.Id.Home:
                        if (!_isReadOnly)
                        {
                            var isDirty = CurrentMode == Mode.Orders ? _orderProcessViewModel.IsDirty : _batchProcessViewModel.IsDirty;
                            if (isDirty)
                            {
                                var isValid = CurrentMode == Mode.Orders ? _orderProcessViewModel.IsValid : _batchProcessViewModel.IsValid;
                                if (isValid)
                                    ConfirmChanges();
                                else
                                    ConfirmInvalidChanges();
                            }
                            else
                                Finish();
                        }
                        else
                            Finish();
                        return true;
                    case Resource.Id.action_start_timer_orderprocessing:
                        var startTimerTask = StartProcessTimerAsync();
                        return true;
                    case Resource.Id.action_stop_timer_orderprocessing:
                        var stopTimerTask = StopProcessTimerAsync();
                        return true;
                    default:
                        return base.OnOptionsItemSelected(item);
                }
            }
            catch (Exception exception)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                var optionItemException = new Exception("Error in OrderProcessActivity.OnOptionsItemSelected: " + item.ItemId, exception);
                logService.LogError(_standardErrorMessage, optionItemException, this, true);
                return base.OnOptionsItemSelected(item);
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

        private async Task SaveProcessAsync()
        {
            if (_loginViewModel.IsLicenseActivated)
            {
                if (CurrentMode == Mode.Orders)
                    await SaveOrderProcessAsync();
                else if (CurrentMode == Mode.Batches)
                    await SaveBatchProcessAsync();
            }
            else
                LogOutUserWithExpiredMessage();
        }

        private async Task SaveOrderProcessAsync()
        {
            var orderId = _orderProcessViewModel.OrderId;
            AndHUD.Shared.Show(this, SavingOrderHUDMessage);
            _hudVisible = true;
            //TODO: Change this currentProcessedPartQty to be set in UI
            var orderViewModel = ServiceContainer.Resolve<OrderViewModel>();
            var currentProcessedPartQty = orderViewModel.ActiveOrder.Quantity;
            var result = await _orderProcessViewModel.SaveCurrentOrderProcessAnswersAsync(currentProcessedPartQty);
            _orderProcessViewModel.InvalidateViewModel();
            var returnIntent = new Intent();
            returnIntent.PutExtra(INTENT_ORDERID, orderViewModel.ActiveOrder.OrderId);
            returnIntent.PutExtra(INTENT_ORDERPROCESS_ERROR, result.ErrorMessage);
            SetResult(Result.Ok, returnIntent);
            Finish();
        }

        private async Task SaveBatchProcessAsync()
        {
            var batchId = _batchProcessViewModel.BatchId;
            var batchViewModel = ServiceContainer.Resolve<BatchViewModel>();
            AndHUD.Shared.Show(this, SavingBatchHUDMessage);
            _hudVisible = true;
            var result = await _batchProcessViewModel.SaveCurrentBatchProcessAnswersAsync();
            _batchProcessViewModel.InvalidateViewModel();
            var returnIntent = new Intent();
            returnIntent.PutExtra(INTENT_BATCHID, batchViewModel.ActiveBatch.BatchId);
            returnIntent.PutExtra(INTENT_ORDERPROCESS_ERROR, result.ErrorMessage);
            SetResult(Result.Ok, returnIntent);
            Finish();
        }

        private async Task StartProcessTimerAsync()
        {
            if (_loginViewModel.IsLicenseActivated)
            {
                AndHUD.Shared.Show(this, "Starting Process Timer");
                _hudVisible = true;

                ViewModelResult result = null;
                if (CurrentMode == Mode.Orders)
                {
                    result = await _orderProcessViewModel.StartOrderProcessTimer();
                }
                else if (CurrentMode == Mode.Batches)
                {
                    result = await _batchProcessViewModel.StartProcessTimer();
                }

                _hudVisible = false;
                AndHUD.Shared.Dismiss();

                var dialogService = ServiceContainer.Resolve<IDialogService>();
                if (result != null && dialogService != null)
                {
                    if (result.Success)
                    {
                        dialogService.ShowToast("Started processing timer.", true);
                    }
                    else if (string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        dialogService.ShowToast("Error - cannot start process timer.", false);
                    }
                    else
                    {
                        dialogService.ShowToast(result.ErrorMessage, false);
                    }
                }
            }
            else
                LogOutUserWithExpiredMessage();
        }

        private async Task StopProcessTimerAsync()
        {
            if (_loginViewModel.IsLicenseActivated)
            {
                AndHUD.Shared.Show(this, "Stopping Process Timer");
                _hudVisible = true;

                ViewModelResult result = null;
                if (CurrentMode == Mode.Orders)
                {
                    result = await _orderProcessViewModel.StopOrderProcessTimer();
                }
                else if (CurrentMode == Mode.Batches)
                {
                    result = await _batchProcessViewModel.StopProcessTimer();
                }

                _hudVisible = false;
                AndHUD.Shared.Dismiss();

                var dialogService = ServiceContainer.Resolve<IDialogService>();
                if (result != null && dialogService != null)
                {
                    if (result.Success)
                    {
                        dialogService.ShowToast("Stopped processing timer.", true);
                    }
                    else if (string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        dialogService.ShowToast("Error - cannot stop process timer.", false);
                    }
                    else
                    {
                        dialogService.ShowToast(result.ErrorMessage, false);
                    }
                }
            }
            else
                LogOutUserWithExpiredMessage();
        }

        private async Task StartLaborTimerAsync()
        {
            if (_loginViewModel.IsLicenseActivated)
            {
                AndHUD.Shared.Show(this, "Starting Labor Timer");
                _hudVisible = true;

                ViewModelResult result = null;
                if (CurrentMode == Mode.Orders)
                {
                    result = await _orderProcessViewModel.StartOrderLaborTimer();
                }
                else if (CurrentMode == Mode.Batches)
                {
                    result = await _batchProcessViewModel.StartLaborTimer();
                }

                _hudVisible = false;
                AndHUD.Shared.Dismiss();

                var dialogService = ServiceContainer.Resolve<IDialogService>();
                if (result != null && dialogService != null)
                {
                    if (result.Success)
                    {
                        dialogService.ShowToast("Started labor timer.", true);
                    }
                    else if (string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        dialogService.ShowToast("Error - cannot start labor timer.", false);
                    }
                    else
                    {
                        dialogService.ShowToast(result.ErrorMessage, false);
                    }
                }
            }
            else
                LogOutUserWithExpiredMessage();
        }

        private async Task PauseLaborTimerAsync()
        {
            if (_loginViewModel.IsLicenseActivated)
            {
                AndHUD.Shared.Show(this, "Pausing Labor Timer");
                _hudVisible = true;

                ViewModelResult result = null;
                if (CurrentMode == Mode.Orders)
                {
                    result = await _orderProcessViewModel.PauseOrderLaborTimer();
                }
                else if (CurrentMode == Mode.Batches)
                {
                    result = await _batchProcessViewModel.PauseLaborTimer();
                }

                _hudVisible = false;
                AndHUD.Shared.Dismiss();

                var dialogService = ServiceContainer.Resolve<IDialogService>();
                if (result != null && dialogService != null)
                {
                    if (result.Success)
                    {
                        dialogService.ShowToast("Paused labor timer.", true);
                    }
                    else if (string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        dialogService.ShowToast("Error - cannot stop pause timer.", false);
                    }
                    else
                    {
                        dialogService.ShowToast(result.ErrorMessage, false);
                    }
                }
            }
            else
                LogOutUserWithExpiredMessage();
        }


        private async Task StopLaborTimerAsync()
        {
            if (_loginViewModel.IsLicenseActivated)
            {
                AndHUD.Shared.Show(this, "Stopping Labor Timer");
                _hudVisible = true;

                ViewModelResult result = null;
                if (CurrentMode == Mode.Orders)
                {
                    result = await _orderProcessViewModel.StopOrderLaborTimer();
                }
                else if (CurrentMode == Mode.Batches)
                {
                    result = await _batchProcessViewModel.StopLaborTimer();
                }

                _hudVisible = false;
                AndHUD.Shared.Dismiss();

                var dialogService = ServiceContainer.Resolve<IDialogService>();
                if (result != null && dialogService != null)
                {
                    if (result.Success)
                    {
                        dialogService.ShowToast("Stopped labor timer.", true);
                    }
                    else if (string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        dialogService.ShowToast("Error - cannot stop labor timer.", false);
                    }
                    else
                    {
                        dialogService.ShowToast(result.ErrorMessage, false);
                    }
                }
            }
            else
                LogOutUserWithExpiredMessage();
        }

        /// <summary>
        /// Loads a process asynchronously based on what properties are set. 
        /// Will load a specific process if <see cref="OrderProcessInfo"/> is set; 
        /// will load the current process if only <see cref="OrderID"/> is set.
        /// </summary>
        /// <returns></returns>
        private async Task LoadProcessAsync()
        {
            if (CurrentMode == Mode.Orders)
                await LoadOrderProcessAsync();
            else
                await LoadBatchProcessAsync();
        }

        private async Task LoadBatchProcessAsync()
        {
            UnregisterViewEvents();

            if (BatchProcessInfo != null)
            {
                if (BatchProcessInfo.Started > DateTime.MinValue &&
                    BatchProcessInfo.Ended < BatchProcessInfo.Started
                    && BatchProcessInfo.BatchId > -1)
                {
                    BatchID = BatchProcessInfo.BatchId;
                    await LoadCurrentBatchProcessAsync();
                    LogInfo(message: "OrderProcessActivity:LoadBatchProcessAsync");
                }
                else
                {
                    BatchID = BatchProcessInfo.BatchId;
                    _isReadOnly = true;
                    await LoadProcessAsync(BatchProcessInfo);
                }
            }
            else if (BatchID > -1)
            {
                await LoadCurrentBatchProcessAsync();
            }

            RegisterViewEvents();
        }

        private async Task LoadOrderProcessAsync()
        {
            UnregisterViewEvents();

            if (OrderProcessInfo != null)
            {
                if (OrderProcessInfo.Started > DateTime.MinValue &&
                    OrderProcessInfo.Ended < OrderProcessInfo.Started
                    && OrderProcessInfo.OrderId > -1)
                {
                    OrderID = OrderProcessInfo.OrderId;
                    await LoadCurrentOrderProcessAsync();
                    LogInfo(message: "OrderProcessActivity:LoadOrderProcessAsync");
                }
                else
                {
                    OrderID = OrderProcessInfo.OrderId;
                    _isReadOnly = true;
                    await LoadProcessAsync(OrderProcessInfo);
                }
            }
            else if (OrderID > -1)
            {
                await LoadCurrentOrderProcessAsync();
            }

            RegisterViewEvents();
        }

        /// <summary>
        /// Loads the current order process fragments and data asynchronously.
        /// </summary>
        /// <returns></returns>
        private async Task LoadCurrentOrderProcessAsync()
        {
            if (_orderProcessViewModel.OrderId != OrderID && _orderProcessViewModel.Process != null)
                SetupProcessControls();
            else
            {
                var result = await _orderProcessViewModel.GetCurrentProcessForOrderAsync(OrderID);
                if (IsDestroyed || _isPaused)
                    return;//Need to bail because our Activity is gone and trying to work with Views will throw a Java.IllegalStateException

                if (result.Success && string.IsNullOrEmpty(result.ErrorMessage))
                {
                    SetupProcessControls();
                }
                else
                {
                    // Show error message
                    var errorMsg = string.IsNullOrEmpty(result.ErrorMessage) ? "An error has occurred." : result.ErrorMessage;
                    var dialogService = ServiceContainer.Resolve<IDialogService>();
                    dialogService?.ShowToast(errorMsg, false);

                    using (var errorLayout = FindViewById<LinearLayout>(Resource.Id.errorLinearLayout))
                    {
                        errorLayout.Visibility = ViewStates.Visible;
                    }

                    DisableTimerButtons();
                }
            }
        }

        /// <summary>
        /// Loads the current batch process fragments and data asynchronously.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private async Task LoadCurrentBatchProcessAsync()
        {
            if (_batchProcessViewModel.BatchId != BatchID && _orderProcessViewModel.Process != null)
                SetupProcessControls();
            else
            {
                var result = await _batchProcessViewModel.GetCurrentProcessForBatchAsync(BatchID);
                if (IsDestroyed || _isPaused)
                    return;//Need to bail because our Activity is gone and trying to work with Views will throw a Java.IllegalStateException

                if (result.Success && string.IsNullOrEmpty(result.ErrorMessage))
                {
                    SetupProcessControls();
                }
                else
                {
                    // Show error message
                    var errorMsg = string.IsNullOrEmpty(result.ErrorMessage) ? "An error has occurred." : result.ErrorMessage;
                    var dialogService = ServiceContainer.Resolve<IDialogService>();
                    dialogService?.ShowToast(errorMsg, false);

                    using (var errorLayout = FindViewById<LinearLayout>(Resource.Id.errorLinearLayout))
                    {
                        errorLayout.Visibility = ViewStates.Visible;
                    }

                    DisableTimerButtons();
                }
            }
        }

        /// <summary>
        /// Loads the process fragments and data asynchronously for an order.
        /// </summary>
        /// <returns></returns>
        private async Task LoadProcessAsync(OrderProcessInfo orderProcessInfo)
        {
            var result = await _orderProcessViewModel.GetProcessForOrderAsync(orderProcessInfo);
            if (IsDestroyed || _isPaused)
                return;//Need to bail because our Activity is gone and trying to work with Views will throw a Java.IllegalStateException

            if (result.Success == true && string.IsNullOrEmpty(result.ErrorMessage))
                SetupProcessControls();
        }

        /// <summary>
        /// Loads the process fragments and data asynchronously for a batch.
        /// </summary>
        /// <param name="batchProcessInfo">The batch process information.</param>
        /// <returns></returns>
        private async Task LoadProcessAsync(BatchProcessInfo batchProcessInfo)
        {
            var result = await _batchProcessViewModel.GetProcessForBatchAsync(batchProcessInfo);
            if (result.Success == true && string.IsNullOrEmpty(result.ErrorMessage))
                SetupProcessControls();
        }

        private void SetupProcessControls()
        {
            if (IsProcessPaperBased())
                ShowPaperBasedViews();
            else
            {
                var transaction = FragmentManager.BeginTransaction();
                var stepListFragment = new OrderProcessStepListFragment { CurrentMode = CurrentMode };
                var questionHostFragment = new OrderProcessQuestionsHostFragment(_isReadOnly) { CurrentMode = CurrentMode };
                transaction.SetTransition(FragmentTransit.FragmentOpen);
                transaction.Add(Resource.Id.stepsContainer, stepListFragment, OrderProcessStepListFragment.ORDER_PROCESS_STEP_FRAGMENT_TAG);
                transaction.Add(Resource.Id.questionsContainer, questionHostFragment, OrderProcessQuestionsHostFragment.ORDER_PROCESS_QUESTION_FRAGMENT_TAG);
                transaction.Commit();
            }

            RefreshTimerButtons();
        }

        private void RefreshTimerButtons()
        {
            if (CurrentMode == Mode.Orders)
            {
                _startLaborTimerButton.Enabled = !_orderProcessViewModel.HasActiveLaborTimer;
                _pauseLaborTimerButton.Enabled = _orderProcessViewModel.HasActiveLaborTimer;
                _stopLaborTimerButton.Enabled = _orderProcessViewModel.IsUserActiveOperator;
            }
            else if (CurrentMode == Mode.Batches)
            {
                _startLaborTimerButton.Enabled = !_batchProcessViewModel.HasActiveLaborTimer;
                _pauseLaborTimerButton.Enabled = _batchProcessViewModel.HasActiveLaborTimer;
                _stopLaborTimerButton.Enabled = _batchProcessViewModel.IsUserActiveOperator;
            }
            else
            {
                // Unsupported mode
                DisableTimerButtons();
            }
        }

        private void DisableTimerButtons()
        {
            _startLaborTimerButton.Enabled = false;
            _pauseLaborTimerButton.Enabled = false;
            _stopLaborTimerButton.Enabled = false;
        }

        private void ShowPaperBasedViews()
        {
            var paperLayout = FindViewById<LinearLayout>(Resource.Id.paperBasedLinearLayout);
            paperLayout.Visibility = ViewStates.Visible;
        }

        /// <summary>
        /// Determines whether the active process is paper based.
        /// </summary>
        /// <returns></returns>
        private bool IsProcessPaperBased()
        {
            if (CurrentMode == Mode.Orders && _orderProcessViewModel.Process != null && !_orderProcessViewModel.Process.IsPaperless)
                return true;

            if (CurrentMode == Mode.Batches && _batchProcessViewModel.Process != null && !_batchProcessViewModel.Process.IsPaperless)
                return true;

            return false;
        }

        private void RemoveExistingHostFragment()
        {
            //A bad hack to unkook Host Fragment from View Model. However when a new step is selected which replaces the 
            //Host framgent it is still hooked up to View Model Events which it recieves before being full dettached.  
            //If it asks for data on the server then bad things happen.
            var hostFragment = FragmentManager.FindFragmentByTag<OrderProcessQuestionsHostFragment>(OrderProcessQuestionsHostFragment.ORDER_PROCESS_QUESTION_FRAGMENT_TAG);
            if (hostFragment != null)
                hostFragment.PrepareToDetachFragment();
        }

        public void OnStepSelected(int stepId)
        {
            try
            {
                RemoveExistingHostFragment();
                if (CurrentMode == Mode.Orders)
                    _orderProcessViewModel.SetNextStep(stepId);
                else
                    _batchProcessViewModel.SetNextStep(stepId);
                var transaction = FragmentManager.BeginTransaction();
                var isReadOnly = CurrentMode == Mode.Orders ? OrderProcessInfo != null : BatchProcessInfo != null;
                var questionHostFragment = new OrderProcessQuestionsHostFragment(_isReadOnly) { CurrentMode = CurrentMode };
                transaction.SetTransition(FragmentTransit.FragmentFade);
                transaction.Replace(Resource.Id.questionsContainer, questionHostFragment, OrderProcessQuestionsHostFragment.ORDER_PROCESS_QUESTION_FRAGMENT_TAG);
                transaction.Commit();
            }
            catch (Exception exception)
            {
                var id = string.Empty;
                if (CurrentMode == Mode.Orders)
                    id = "OrderID: " + _orderProcessViewModel.OrderId.ToString();
                else
                    id = "BatchID: " + _batchProcessViewModel.BatchId.ToString();
                var errorMessage = string.Format("Error in OrderProcessActivity.OnStepSelected StepID: {0}, {1}", stepId, id);
                var logService = ServiceContainer.Resolve<ILogService>();
                var onStepSelectedException = new Exception(errorMessage, exception);
                logService.LogError(_standardErrorMessage, onStepSelectedException, this, true);
            }
        }

        /// <summary>
        /// Registers the view model events.
        /// </summary>
        protected override void RegisterViewModelEvents()
        {
            if (CurrentMode == Mode.Orders)
            {
                _orderProcessViewModel.IsValidChanged += OnViewModelIsValidChanged;
                _orderProcessViewModel.IsBusyChanged += OnViewModelIsBusyChanged;
                _orderProcessViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
            else
            {
                _batchProcessViewModel.IsValidChanged += OnViewModelIsValidChanged;
                _batchProcessViewModel.IsBusyChanged += OnViewModelIsBusyChanged;
                _batchProcessViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void RegisterViewEvents()
        {
            _documentsSpinner.ItemSelected += DocumentsSpinner_ItemSelected;
            _startLaborTimerButton.Click += StartLaborTimerButton_Click;
            _pauseLaborTimerButton.Click += PauseLaborTimerButton_Click;
            _stopLaborTimerButton.Click += StopLaborTimerButton_Click;
        }

        /// <summary>
        /// Unregisters the view model events.
        /// </summary>
        protected override void UnregisterViewModelEvents()
        {
            if (CurrentMode == Mode.Orders)
            {
                _orderProcessViewModel.IsValidChanged -= OnViewModelIsValidChanged;
                _orderProcessViewModel.IsBusyChanged -= OnViewModelIsBusyChanged;
                _orderProcessViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }
            else
            {
                _batchProcessViewModel.IsValidChanged -= OnViewModelIsValidChanged;
                _batchProcessViewModel.IsBusyChanged -= OnViewModelIsBusyChanged;
                _batchProcessViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }
        }

        private void UnregisterViewEvents()
        {
            _documentsSpinner.ItemSelected -= DocumentsSpinner_ItemSelected;
            _startLaborTimerButton.Click -= StartLaborTimerButton_Click;
            _pauseLaborTimerButton.Click -= PauseLaborTimerButton_Click;
            _stopLaborTimerButton.Click -= StopLaborTimerButton_Click;
        }

        /// <summary>
        ///  Handles the IsBusyChanged event of the ViewModel
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnViewModelIsBusyChanged(object sender, EventArgs e)
        {
            InvalidateOptionsMenu();
        }

        /// <summary>
        ///  Handles the IsValidChanged event of the ViewModel
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnViewModelIsValidChanged(object sender, EventArgs e)
        {
            InvalidateOptionsMenu();
            SetCompletedQuestionsLabel();
            PopulateDocumentsSpinner();
        }

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (CurrentMode == Mode.Orders)
            {
                if (e.PropertyName == nameof(ProcessViewModel.HasActiveLaborTimer) || e.PropertyName == nameof(ProcessViewModel.IsUserActiveOperator))
                {
                    RefreshTimerButtons();
                }
            }
            else if (CurrentMode == Mode.Batches)
            {
                if (e.PropertyName == nameof(BatchProcessViewModel.HasActiveLaborTimer) || e.PropertyName == nameof(BatchProcessViewModel.IsUserActiveOperator))
                {
                    RefreshTimerButtons();
                }
            }
        }

        private void SetCompletedQuestionsLabel()
        {
            var completedTextView = FindViewById<TextView>(Resource.Id.textViewCompletedQuestions);
            int count = 0;
            int answered = 0;

            if (CurrentMode == Mode.Orders)
            {
                count = _orderProcessViewModel.QuestionsAndAnswers != null ? _orderProcessViewModel.QuestionsAndAnswers.Count : 0;
                answered = _orderProcessViewModel.QuestionsAndAnswers != null ?
                    _orderProcessViewModel.QuestionsAndAnswers.Where(viewModel => viewModel.Completed == true).Count() :
                    0;
            }
            else
            {
                count = _batchProcessViewModel.QuestionsAndAnswers != null ? _batchProcessViewModel.QuestionsAndAnswers.Count : 0;
                answered = _batchProcessViewModel.QuestionsAndAnswers != null ?
                    _batchProcessViewModel.QuestionsAndAnswers.Where(viewModel => viewModel.Completed == true).Count() :
                    0;
            }

            var countLabel = string.Format("{0} / {1}", answered.ToString(), count.ToString());
            completedTextView.Text = countLabel;
        }

        private void PopulateDocumentsSpinner()
        {
            var allProcessDocuments = new List<DocumentInfo>();
            if (CurrentMode == Mode.Orders)
            {
                var processDocuments = _orderProcessViewModel.Process?.Documents;
                var aliasDocuments = _orderProcessViewModel.ProcessAlias?.Documents;

                if (processDocuments != null)
                {
                    allProcessDocuments.AddRange(processDocuments);
                }

                if (aliasDocuments != null)
                {
                    allProcessDocuments.AddRange(aliasDocuments);
                }
            }
            else if (CurrentMode == Mode.Batches)
            {
                var processDocuments = _batchProcessViewModel.Process?.Documents;

                if (processDocuments != null)
                {
                    allProcessDocuments.AddRange(processDocuments);
                }
            }

            var documentListAdapter = new DocumentListAdapter(this,
                allProcessDocuments,
                null);

            documentListAdapter.IncludeDocumentType = true;
            _documentsSpinner.Adapter = documentListAdapter;
        }

        /// <summary>
        /// Called when the activity has detected the user's press of the back
        /// key.
        /// </summary>
        public override void OnBackPressed()
        {
            if (CurrentMode == Mode.Orders)
                OnOrdersBackPressed();
            else
                OnBatchBackPressed();
        }

        private void OnOrdersBackPressed()
        {
            if (!_isReadOnly && _orderProcessViewModel.IsDirty)
            {
                if (_orderProcessViewModel.IsValid)
                    ConfirmChanges();
                else
                    ConfirmInvalidChanges();
            }
            else
                base.OnBackPressed();
        }

        private void OnBatchBackPressed()
        {
            if (!_isReadOnly && _batchProcessViewModel.IsDirty)
            {
                if (_batchProcessViewModel.IsValid)
                    ConfirmChanges();
                else
                    ConfirmInvalidChanges();
            }
            else
                base.OnBackPressed();
        }

        private void ConfirmChanges()
        {
            var saveConfirmDialog = SaveConfirmFragment.New(message: "Save Answers?", title: "Save",
                    positiveCallback: async (dialog) =>
                    {
                        dialog.Dismiss();
                        await SaveProcessAsync();
                    },
                    negativeCallback: (dialog) =>
                    {
                        dialog.Dismiss();
                        _orderProcessViewModel.InvalidateViewModel();
                        Finish();
                    },
                    neutralCallback: (dialog) =>
                    {
                        dialog.Dismiss();
                    });

            var transaction = FragmentManager.BeginTransaction();
            saveConfirmDialog.Show(transaction, SaveConfirmFragment.LOGOUT_CONFIRM_FRAGMENT_TAG);
        }

        private void ConfirmInvalidChanges()
        {
            var alertDialogBuilder = new AlertDialog.Builder(this);
            alertDialogBuilder
                .SetTitle("Invalid Answers")
                .SetMessage("Some of the answers are invalid and the Order Process will not be saved." + 
                    System.Environment.NewLine + System.Environment.NewLine  + "Continue anyways?")
                .SetPositiveButton(Resource.String.SaveYes, (sender, args) =>
                {
                    var dialog = sender as Dialog;
                    dialog.Dismiss();
                    if (CurrentMode == Mode.Orders)
                        _orderProcessViewModel.InvalidateViewModel();
                    else
                        _batchProcessViewModel.InvalidateViewModel();
                    Finish();
                })
                .SetNegativeButton(Resource.String.SaveNo, (sender, eventArgs) =>
                {
                    var dialog = sender as Dialog;
                    dialog.Dismiss();
                });

            alertDialogBuilder
                .Create()
                .Show();
        }

        public void OnNextQuestion()
        {

            ProcessStepInfo nextStep = null;
            if (CurrentMode == Mode.Orders)
                nextStep = _orderProcessViewModel.GetNextStep();
            else
                nextStep = _batchProcessViewModel.GetNextStep();
            if (nextStep != null)
            {
                var stepFragment = FragmentManager.FindFragmentByTag<OrderProcessStepListFragment>(
                    OrderProcessStepListFragment.ORDER_PROCESS_STEP_FRAGMENT_TAG);
                if (stepFragment != null && !stepFragment.IsHidden)
                {
                    stepFragment.SetListViewSelection(nextStep);
                    OnStepSelected(nextStep.ProcessStepId);
                }
            }
            else
            {
                var stepListFragment = FragmentManager.FindFragmentByTag<OrderProcessStepListFragment>(
                    OrderProcessStepListFragment.ORDER_PROCESS_STEP_FRAGMENT_TAG);
                if (stepListFragment != null)
                    stepListFragment.RequestRefresh();
            }
        }

        private void DocumentsSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var position = e.Position;
            var documentListAdapter = _documentsSpinner.Adapter as DocumentListAdapter;
            if (documentListAdapter == null)
            {
                return;
            }

            var item = documentListAdapter[position];

            if (item.Document != null)
            {
                OnDocumentInfoSelected(item.Document);
            }
            else if (item.Media != null)
            {
                OnMediaSummarySelected(item.Media);
            }
        }

        private void StartLaborTimerButton_Click(object sender, EventArgs e)
        {
            var task = StartLaborTimerAsync();
        }

        private void PauseLaborTimerButton_Click(object sender, EventArgs e)
        {
            var task = PauseLaborTimerAsync();
        }

        private void StopLaborTimerButton_Click(object sender, EventArgs e)
        {
            new AlertDialog.Builder(this)
                .SetTitle("Stop Timer")
                .SetMessage("Are you sure that you want to clock out of this job?")
                .SetPositiveButton(Resource.String.SaveYes, (dialogSender, args) =>
                {
                    var task = StopLaborTimerAsync();
                })
                .SetNegativeButton(Resource.String.SaveNo, (dialogSender, args) => { })
                .Create()
                .Show();
        }

        #endregion
    }
}