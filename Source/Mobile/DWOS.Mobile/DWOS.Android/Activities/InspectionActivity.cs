using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using System.Threading.Tasks;
using DWOS.Utilities;
using DWOS.ViewModels;
using Android.Views.InputMethods;
using System.ComponentModel;
using Android.Util;
using AndroidHUD;
using DWOS.Android.Adapters;
using DWOS.Services.Messages;

namespace DWOS.Android
{
    /// <summary>
    /// Activity that allows the user to inspect an order.
    /// </summary>
    [Activity(Label = "Control Inspection", Name = "dwos.android.InspectionActivity", WindowSoftInputMode = SoftInput.AdjustPan)]
    public class InspectionActivity : BaseActivity, IInspectionQuestionCallback, ViewPager.IOnPageChangeListener, IDocumentSelectionCallback
    {
        #region Fields

        public const string INTENT_MODE = "Mode";
        public const string BUNDLEID_DISPLAYHUD = "DisplayHud";
        const string BUNDLE_ORDERID = "OrderId";
        const string BUNDLE_BATCHID = "BatchId";
        public const string INTENT_ORDERID = "OrderId";
        public const string INTENT_BATCHID = "BatchId";
        public const string INTENT_SUCCESS = "Success";
        public const string INTENT_ERROR = "Error";

        const string SavingOrderHUDMessage = "Updating order inspection";
        const string SavingBatchHUDMessage = "Updating batch inspection";

        InspectionViewModel _orderInspectionViewModel;
        BatchInspectionViewModel _batchInspectionViewModel;
        LogInViewModel _loginViewModel;
        int _orderId;
        int _batchId;
        TextView _workOrderTextView;
        TextView _nameTextView;
        TextView _referenceTextView;
        TextView _requirementsTextView;
        TextView _operatorTextView;
        TextView _inspectionTextView;
        ViewPager _answersViewPager;
        EditText _notesEditText;
        Spinner _documentsSpinner;
        Button _startLaborTimerButton;
        Button _pauseLaborTimerButton;
        Button _stopLaborTimerButton;
        Mode CurrentMode = Mode.Orders;
        bool _hudVisible = false;

        #endregion

        #region Methods

        /// <summary>
        /// Called when [create].
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        protected override async void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);
                _orderInspectionViewModel = ServiceContainer.Resolve<InspectionViewModel>();
                _batchInspectionViewModel = ServiceContainer.Resolve<BatchInspectionViewModel>();
                _loginViewModel = ServiceContainer.Resolve<LogInViewModel>();

                SetContentView(Resource.Layout.InspectionLayout);
                HandleIntent(Intent);
                SupportActionBar.SetDisplayHomeAsUpEnabled(showHomeAsUp: true);

                _workOrderTextView = FindViewById<TextView>(Resource.Id.textViewWorkOrder);
                _nameTextView = FindViewById<TextView>(Resource.Id.textViewInspectionName);
                _referenceTextView = FindViewById<TextView>(Resource.Id.textViewInspectionReference);
                _requirementsTextView = FindViewById<TextView>(Resource.Id.textViewInspectionRequirements);
                _answersViewPager = FindViewById<ViewPager>(Resource.Id.pagerQuestions);
                _operatorTextView = FindViewById<TextView>(Resource.Id.textViewInspectionOperator);
                _inspectionTextView = FindViewById<TextView>(Resource.Id.textViewInspectionDate);
                _documentsSpinner = FindViewById<Spinner>(Resource.Id.spinnerDocuments);

                _startLaborTimerButton = FindViewById<Button>(Resource.Id.startLaborButton);
                _pauseLaborTimerButton = FindViewById<Button>(Resource.Id.pauseLaborButton);
                _stopLaborTimerButton = FindViewById<Button>(Resource.Id.stopLaborButton);

                _notesEditText = FindViewById<EditText>(Resource.Id.textViewInspectionNotes);
                _notesEditText.TextChanged += ((s, e) => _orderInspectionViewModel.Notes = _notesEditText.Text);

                // Visibility for labor buttons
                if (ApplicationSettings.Settings.UsingTimeTracking)
                {
                    using (var laborButtons = FindViewById(Resource.Id.laborButtons))
                    {
                        laborButtons.Visibility = ViewStates.Visible;
                    }

                    // Tint play/pause/stop buttons
                    var tintColor = global::Android.Support.V4.Content.ContextCompat.GetColor(this, Resource.Color.textColorSecondary);
                    _startLaborTimerButton.GetCompoundDrawables()[0].CompatSetTint(tintColor);
                    _pauseLaborTimerButton.GetCompoundDrawables()[0].CompatSetTint(tintColor);
                    _stopLaborTimerButton.GetCompoundDrawables()[0].CompatSetTint(tintColor);
                }

                if (bundle == null)
                {
                    // Always retrieve data from server initializing
                    // for the first time.
                    if (CurrentMode == Mode.Orders)
                    {
                        await LoadCurrentOrderInspection();
                    }
                    else
                    {
                        await LoadCurrentBatchInspection();
                    }
                }
                else
                {
                    _orderId = bundle.GetInt(BUNDLE_ORDERID, -1);
                    if (_orderId > -1)
                    {
                        CurrentMode = Mode.Orders;
                    }
                    else
                    {
                        _batchId = bundle.GetInt(BUNDLE_BATCHID, -1);
                        CurrentMode = Mode.Batches;
                    }

                    _hudVisible = bundle.GetBoolean(BUNDLEID_DISPLAYHUD, defaultValue: false);
                    if (_hudVisible)
                    {
                        if (CurrentMode == Mode.Orders)
                            AndHUD.Shared.Show(this, SavingOrderHUDMessage);
                        else
                            AndHUD.Shared.Show(this, SavingBatchHUDMessage);
                    }

                    if (CurrentMode == Mode.Orders)
                    {
                        if (_orderId != _orderInspectionViewModel.OrderId)
                        {
                            await LoadCurrentOrderInspection();
                        }
                        else
                        {
                            ShowCurrentOrderInspection();
                        }
                    }
                    else
                    {
                        if (_batchId != _batchInspectionViewModel.BatchId)
                        {
                            await LoadCurrentBatchInspection();
                        }
                        else
                        {
                            ShowCurrentBatchInspection();
                        }
                    }
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

        private async Task LoadCurrentBatchInspection()
        {
            var result = await _batchInspectionViewModel.GetCurrentBatchInspectionForOrderAsync(_batchId);
            if (result.Success)
            {
                _batchInspectionViewModel.CheckAllConditions();
            }
            else
            {
                var toastMessage = string.Format("Unable to load Batch Inspection: {0}", result.ErrorMessage);
                Toast.MakeText(this, toastMessage, ToastLength.Long)
                    .Show();

                DisableTimerButtons();
            }
        }

        private async Task LoadCurrentOrderInspection()
        {
            var result = await _orderInspectionViewModel.GetCurrentInspectionForOrderAsync(_orderId);
            if (result.Success)
            {
                _orderInspectionViewModel.CheckAllConditions();
            }
            else
            {
                var toastMessage = string.Format("Unable to load Order Inspection: {0}", result.ErrorMessage);
                Toast.MakeText(this, toastMessage, ToastLength.Long)
                    .Show();

                DisableTimerButtons();
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

        private void ShowCurrentBatchInspection()
        {
            UnregisterViewEvents();

            if (_batchInspectionViewModel.Inspection != null)
            {
                var logInViewModel = ServiceContainer.Resolve<LogInViewModel>();
                var inspectionInfo = _batchInspectionViewModel.Inspection;
                _orderId = _batchInspectionViewModel.UnsavedOrderIds != null ? _batchInspectionViewModel.UnsavedOrderIds.First() : -1;
                _workOrderTextView.Text = string.Format("WORK ORDER {0} in BATCH {1}", _orderId, _batchInspectionViewModel.BatchId.ToString());
                _nameTextView.Text = inspectionInfo.Name;
                _referenceTextView.Text = inspectionInfo.TestReference;
                _requirementsTextView.Text = inspectionInfo.TestRequirements;
                _operatorTextView.Text = logInViewModel.UserProfile.Name;
                _inspectionTextView.Text = DateTime.Now.ToShortDateString();
                _notesEditText.Text = _batchInspectionViewModel.Notes;

                _documentsSpinner.Adapter = new DocumentListAdapter(this,
                    inspectionInfo.Documents,
                    null);

                if (_batchInspectionViewModel.QuestionsAndAnswers != null && _batchInspectionViewModel.QuestionsAndAnswers.Count > 0)
                {
                    _answersViewPager.Visibility = ViewStates.Visible;
                    _answersViewPager.Adapter = new InspectionQuestionFragmentAdapter(
                        _batchInspectionViewModel.QuestionsAndAnswers.ToList<InspectionQuestionViewModel>(), CurrentMode, FragmentManager);
                }
                else
                    _answersViewPager.Visibility = ViewStates.Gone;
            }

            RefreshTimerButtons();
            RegisterViewEvents();
        }

        private void ShowCurrentOrderInspection()
        {
            UnregisterViewEvents();

            if (_orderInspectionViewModel.Inspection != null)
            {
                var logInViewModel = ServiceContainer.Resolve<LogInViewModel>();
                var inspectionInfo = _orderInspectionViewModel.Inspection;
                _workOrderTextView.Text = string.Format("WORK ORDER {0}", _orderInspectionViewModel.OrderId.ToString());
                _nameTextView.Text = inspectionInfo.Name;
                _referenceTextView.Text = inspectionInfo.TestReference;
                _requirementsTextView.Text = inspectionInfo.TestRequirements;
                _operatorTextView.Text = logInViewModel.UserProfile.Name;
                _inspectionTextView.Text = DateTime.Now.ToShortDateString();
                _notesEditText.Text = _orderInspectionViewModel.Notes;

                _documentsSpinner.Adapter = new DocumentListAdapter(this,
                    _orderInspectionViewModel.Inspection.Documents,
                    null);

                if (_orderInspectionViewModel.QuestionsAndAnswers != null && _orderInspectionViewModel.QuestionsAndAnswers.Count > 0)
                {
                    _answersViewPager.Visibility = ViewStates.Visible;
                    _answersViewPager.Adapter = new InspectionQuestionFragmentAdapter(_orderInspectionViewModel.QuestionsAndAnswers, CurrentMode, FragmentManager);
                    _answersViewPager.AddOnPageChangeListener(this);
                }
                else
                    _answersViewPager.Visibility = ViewStates.Gone; 
            }

            RefreshTimerButtons();
            RegisterViewEvents();
        }

        private void HandleIntent(Intent intent)
        {
            if (intent != null)
            {
                var mode = intent.GetStringExtra(INTENT_MODE);
                if (mode == Mode.Orders.ToString())
                {
                    _orderId = intent.GetIntExtra(INTENT_ORDERID, defaultValue: -1);
                    CurrentMode = Mode.Orders;
                }
                else if (mode == Mode.Batches.ToString())
                {
                    _batchId = intent.GetIntExtra(INTENT_BATCHID, defaultValue: -1);
                    CurrentMode = Mode.Batches;
                }
            }
        }

        /// <summary>
        /// Perform any final cleanup before an activity is destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            if (_hudVisible)
            {
                _hudVisible = false;
                AndHUD.Shared.Dismiss();
            }

            UnregisterViewEvents();
            _workOrderTextView.Dispose();
            _nameTextView.Dispose();
            _referenceTextView.Dispose();
            _requirementsTextView.Dispose();
            _answersViewPager.Dispose();
            _notesEditText.Dispose();
            _operatorTextView.Dispose();
            _inspectionTextView.Dispose();
            _documentsSpinner.Dispose();
            _startLaborTimerButton?.Dispose();
            _pauseLaborTimerButton?.Dispose();
            _stopLaborTimerButton?.Dispose();

            base.OnDestroy();
        }

        /// <summary>
        /// Called to retrieve per-instance state from an activity before being killed
        /// so that the state can be restored in <c><see cref="M:Android.App.Activity.OnCreate(Android.OS.Bundle)" /></c> or
        /// <c><see cref="M:Android.App.Activity.OnRestoreInstanceState(Android.OS.Bundle)" /></c> (the <c><see cref="T:Android.OS.Bundle" /></c> populated by this method
        /// will be passed to both).
        /// </summary>
        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (CurrentMode == Mode.Orders)
                outState.PutInt(BUNDLE_ORDERID, _orderId);
            else
                outState.PutInt(BUNDLE_BATCHID, _batchId);
            if (_hudVisible)
                outState.PutBoolean(BUNDLEID_DISPLAYHUD, true);
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
            MenuInflater.Inflate(Resource.Menu.order_inspection, menu);
            return true;
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            var isEnabled = true;
            var hasActiveProcessTimer = false;

            if (CurrentMode == Mode.Orders)
            {
                isEnabled = _orderInspectionViewModel.IsValid && !_orderInspectionViewModel.IsBusy;
                hasActiveProcessTimer = _orderInspectionViewModel.HasActiveProcessTimer;
            }
            else
            {
                isEnabled = _batchInspectionViewModel.IsValid && !_batchInspectionViewModel.IsBusy;
                hasActiveProcessTimer = _batchInspectionViewModel.HasActiveProcessTimer;
            }

            var failMenu = menu.FindItem(Resource.Id.action_controlfail);
            var passMenu = menu.FindItem(Resource.Id.action_controlpass);
            var startTimerMenu = menu.FindItem(Resource.Id.action_start_timer_orderprocessing);
            var stopTimerMenu = menu.FindItem(Resource.Id.action_stop_timer_orderprocessing);

            try
            {
                failMenu.SetEnabled(isEnabled, this, Resource.Drawable.ic_action_fail);
                passMenu.SetEnabled(isEnabled, this, Resource.Drawable.ic_action_pass);

                startTimerMenu.SetVisible(ApplicationSettings.Settings.UsingTimeTracking);
                startTimerMenu.SetEnabled(!hasActiveProcessTimer);

                stopTimerMenu.SetVisible(ApplicationSettings.Settings.UsingTimeTracking);
                stopTimerMenu.SetEnabled(hasActiveProcessTimer);

                return true;
            }
            finally
            {
                failMenu.Dispose();
                passMenu.Dispose();
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
                bool isPass = false;
                switch (item.ItemId)
                {
                    case Resource.Id.action_controlfail:
                        isPass = false;
                        break;
                    case Resource.Id.action_controlpass:
                        isPass = true;
                        break;
                    case global::Android.Resource.Id.Home:
                        Finish();
                        return true;
                    case Resource.Id.action_start_timer_orderprocessing:
                        var startTimerTask = StartProcessTimerAsync();
                        return true;
                    case Resource.Id.action_stop_timer_orderprocessing:
                        var stopTimerTask = StopProcessTimerAsync();
                        return true;
                    default:
                        return false;
                }

                if (_loginViewModel.IsLicenseActivated)
                {
                    var result = SaveInspectionAsync(isPass);
                    return true;
                }
                else
                {
                    LogOutUserWithExpiredMessage();
                    return false;
                }
            }
            catch (Exception exception)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                var optionItemException = new Exception("Error in InspectionActivity.OnOptionsItemSelected: " + item.ItemId, exception);
                logService.LogError(_standardErrorMessage, optionItemException, this, true);
                return base.OnOptionsItemSelected(item);
            }
        }

        private async Task SaveInspectionAsync(bool isPass)
        {
            if (CurrentMode == Mode.Orders)
                await SaveOrderInspectionAsync(isPass);
            else
                await SaveBatchInspectionAsync(isPass);
        }

        private async Task SaveBatchInspectionAsync(bool isPass)
        {
            AndHUD.Shared.Show(this, SavingBatchHUDMessage);
            var orderViewModel = ServiceContainer.Resolve<OrderViewModel>();
            var orderDetail = await orderViewModel.GetOrderDetailAsync(_orderId, imageSize: 10);
            if (orderDetail != null)
            {
                _hudVisible = true;
                var result = await _batchInspectionViewModel.SaveAnswersAsync(_orderId, orderDetail.Quantity, isPass: isPass);
                AndHUD.Shared.Dismiss(this);
                if (string.IsNullOrEmpty(result.ErrorMessage) && _batchInspectionViewModel.UnsavedOrderIds.Count > 0)
                {
                    var nextOrderId = _batchInspectionViewModel.UnsavedOrderIds.First();
                    var message = string.Format("Inspection for Order {0} saved. Please continue with Order {1}", _orderId, nextOrderId);
                    _orderId = nextOrderId;
                    if (_batchInspectionViewModel.QuestionsAndAnswers != null && _batchInspectionViewModel.QuestionsAndAnswers.Count > 0)
                        _answersViewPager.Adapter = new InspectionQuestionFragmentAdapter(_batchInspectionViewModel.QuestionsAndAnswers, CurrentMode, FragmentManager);
                    var toast = Toast.MakeText(this, message, ToastLength.Long);
                    toast.SetGravity(GravityFlags.Center, 0, 0);
                    toast.Show();
                    _notesEditText.Text = string.Empty;
                    _workOrderTextView.Text = string.Format("WORK ORDER {0} in BATCH {1}", _orderId, _batchInspectionViewModel.BatchId.ToString());
                }
                else
                {
                    var returnIntent = new Intent();
                    returnIntent.PutExtra(INTENT_MODE, CurrentMode.ToString());
                    returnIntent.PutExtra(INTENT_BATCHID, _batchInspectionViewModel.BatchId);
                    returnIntent.PutExtra(INTENT_SUCCESS, result.Success);
                    returnIntent.PutExtra(INTENT_ERROR, result.ErrorMessage);
                    SetResult(Result.Ok, returnIntent);
                    Finish();
                }
            }
            else
            {
                AndHUD.Shared.Dismiss(this);
                Toast.MakeText(this, "Unable to retrieve Order " + _orderId.ToString(), ToastLength.Short)
                        .Show();
            }
        }

        private async Task SaveOrderInspectionAsync(bool isPass)
        {
            //TODO: Change this currentProcessedPartQty to be set in UI
            var orderViewModel = ServiceContainer.Resolve<OrderViewModel>();
            var currentProcessedPartQty = orderViewModel.ActiveOrder.Quantity;
            AndHUD.Shared.Show(this, SavingOrderHUDMessage);
            _hudVisible = true;
            var result = await _orderInspectionViewModel.SaveAnswersAsync(currentProcessedPartQty, isPass);
            var returnIntent = new Intent();
            returnIntent.PutExtra(INTENT_MODE, CurrentMode.ToString());
            returnIntent.PutExtra(INTENT_ORDERID, _orderInspectionViewModel.OrderId);
            returnIntent.PutExtra(INTENT_SUCCESS, result.Success);
            returnIntent.PutExtra(INTENT_ERROR, result.ErrorMessage);
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
                    result = await _orderInspectionViewModel.StartOrderProcessTimer();
                }
                else if (CurrentMode == Mode.Batches)
                {
                    result = await _batchInspectionViewModel.StartProcessTimer();
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
                    result = await _orderInspectionViewModel.StopOrderProcessTimer();
                }
                else if (CurrentMode == Mode.Batches)
                {
                    result = await _batchInspectionViewModel.StopProcessTimer();
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
                    result = await _orderInspectionViewModel.StartOrderLaborTimer();
                }
                else if (CurrentMode == Mode.Batches)
                {
                    result = await _batchInspectionViewModel.StartLaborTimer();
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

                RefreshTimerButtons();
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
                    result = await _orderInspectionViewModel.PauseOrderLaborTimer();
                }
                else if (CurrentMode == Mode.Batches)
                {
                    result = await _batchInspectionViewModel.PauseLaborTimer();
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
                        dialogService.ShowToast("Error - cannot pause labor timer.", false);
                    }
                    else
                    {
                        dialogService.ShowToast(result.ErrorMessage, false);
                    }
                }

                RefreshTimerButtons();
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
                    result = await _orderInspectionViewModel.StopOrderLaborTimer();
                }
                else if (CurrentMode == Mode.Batches)
                {
                    result = await _batchInspectionViewModel.StopLaborTimer();
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

                RefreshTimerButtons();
            }
            else
                LogOutUserWithExpiredMessage();
        }

        /// <summary>
        /// Called when it is time to register to view model events.
        /// </summary>
        protected override void RegisterViewModelEvents()
        {
            if (CurrentMode == Mode.Orders)
            {
                _orderInspectionViewModel.IsValidChanged += ViewModel_IsValidChanged;
                _orderInspectionViewModel.PropertyChanged += ViewModel_PropertyChanged;
                _orderInspectionViewModel.IsBusyChanged += ViewModel_IsBusyChanged;
            }
            else
            {
                _batchInspectionViewModel.IsValidChanged += ViewModel_IsValidChanged;
                _batchInspectionViewModel.PropertyChanged += ViewModel_PropertyChanged;
                _batchInspectionViewModel.IsBusyChanged += ViewModel_IsBusyChanged;
            }
        }

        private void RegisterViewEvents()
        {
            _documentsSpinner.ItemSelected += DocumentsSpinner_ItemSelected;
            _startLaborTimerButton.Click += StartLaborTimerButton_Click;
            _pauseLaborTimerButton.Click += PauseLaborTimerButton_Click;
            _stopLaborTimerButton.Click += StopLaborTimerButton_Click;
        }

        private void RefreshTimerButtons()
        {
            if (CurrentMode == Mode.Orders)
            {
                _startLaborTimerButton.Enabled = !_orderInspectionViewModel.HasActiveLaborTimer;
                _pauseLaborTimerButton.Enabled = _orderInspectionViewModel.HasActiveLaborTimer;
                _stopLaborTimerButton.Enabled = _orderInspectionViewModel.IsUserActiveOperator;
            }
            else if (CurrentMode == Mode.Batches)
            {
                _startLaborTimerButton.Enabled = !_batchInspectionViewModel.HasActiveLaborTimer;
                _pauseLaborTimerButton.Enabled = _batchInspectionViewModel.HasActiveLaborTimer;
                _stopLaborTimerButton.Enabled = _batchInspectionViewModel.IsUserActiveOperator;
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

        /// <summary>
        /// Called when it is time to unregister from view model events.
        /// </summary>
        protected override void UnregisterViewModelEvents()
        {
            if (CurrentMode == Mode.Orders)
            {
                _orderInspectionViewModel.IsValidChanged -= ViewModel_IsValidChanged;
                _orderInspectionViewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _orderInspectionViewModel.IsBusyChanged -= ViewModel_IsBusyChanged;
            }
            else
            {
                _batchInspectionViewModel.IsValidChanged -= ViewModel_IsValidChanged;
                _batchInspectionViewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _batchInspectionViewModel.IsBusyChanged -= ViewModel_IsBusyChanged;
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
        /// Handles the IsBusyChanged event of the ViewModel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ViewModel_IsBusyChanged(object sender, EventArgs e)
        {
            InvalidateOptionsMenu();
        }

        /// <summary>
        /// Handles the IsValidChanged event of the InspectionViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ViewModel_IsValidChanged(object sender, EventArgs e)
        {
            InvalidateOptionsMenu();
        }

        /// <summary>
        /// Handles the PropertyChanged event of the InspectionViewModel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (CurrentMode == Mode.Orders)
            {
                if (e.PropertyName == nameof(InspectionViewModel.Inspection))
                {
                    ShowCurrentOrderInspection();
                }
                else if (e.PropertyName == nameof(InspectionViewModel.HasActiveLaborTimer) || e.PropertyName == nameof(InspectionViewModel.IsUserActiveOperator))
                {
                    RefreshTimerButtons();
                }
            }
            else if (CurrentMode == Mode.Batches)
            {
                if (e.PropertyName == nameof(BatchInspectionViewModel.Inspection))
                {
                    ShowCurrentBatchInspection();
                }
                else if (e.PropertyName == nameof(BatchInspectionViewModel.HasActiveLaborTimer) || e.PropertyName == nameof(BatchInspectionViewModel.IsUserActiveOperator))
                {
                    RefreshTimerButtons();
                }
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

        public void NextQuestion()
        {
            if (_answersViewPager != null)
            {
                int nextItem = _answersViewPager.CurrentItem + 1;
                if (nextItem < _answersViewPager.Adapter.Count)
                    _answersViewPager.SetCurrentItem(nextItem, smoothScroll: true);
                else
                {
                    var imm = (InputMethodManager)GetSystemService(Activity.InputMethodService);
                    imm.HideSoftInputFromWindow(_answersViewPager.ApplicationWindowToken, HideSoftInputFlags.None);
                }
            }
        }

        public void OnPageScrollStateChanged(int state)
        {
            //No Implementation
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            //No Implementation
        }

        public void OnPageSelected(int position)
        {
            try
            {
                if (!(_answersViewPager.Adapter is InspectionQuestionFragmentAdapter adapter))
                {
                    Log.Debug("*********** InspectionActivity", "No Adapter!");
                    return;
                }

                var question = adapter.Questions[position];

                if (question.Skipped)
                {
                    this.HideSoftKeyboard();
                }
                else
                {
                    switch (question.InputType)
                    {
                        case InputTypes.List:
                        case InputTypes.Date:
                        case InputTypes.Time:
                        case InputTypes.TimeIn:
                        case InputTypes.TimeOut:
                        case InputTypes.DateTimeIn:
                        case InputTypes.DateTimeOut:
                        case InputTypes.None:
                            this.HideSoftKeyboard();
                            break;
                        default:
                            this.ShowSoftKeyboard();
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                var optionItemException = new Exception("Error in InspectionActivity.OnPageSelected", exception);
                logService.LogError("Error in InspectionActivity.OnPageSelected", optionItemException, this);
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
                else
                {
                    dialogService.ShowToast(errorMessage, true);
                }
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