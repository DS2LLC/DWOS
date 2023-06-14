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
using ZXing.Mobile;
using Android.Views.InputMethods;
using System.Threading.Tasks;
using ZX = ZXing;

namespace DWOS.Android
{
    /// <summary>
    /// Provides order/batch check-in functionality in a dialog window.
    /// </summary>
    public class CheckInFragment : DialogFragment, TextView.IOnEditorActionListener
    {
        #region Fields

        public const string CHECKIN_FRAGMENT_TAG = "CheckInFragment";
        public const string BUNDLE_EXTRA_MODE = "CheckInMode";

        Button _cancelButton;
        Button _checkInButton;
        AutoCompleteTextView _workOrderAutoTextView;
        Button _scanButton;
        TextView _nextDepartmentTextView;
        OrderViewModel _orderViewModel;
        BatchViewModel _batchViewModel;
        CheckInViewModel _checkInViewModel;
        ProcessViewModel _orderProcessViewModel;
        BatchProcessViewModel _batchProcessViewModel;
        ArrayAdapter _workOrderIdAdapter;
        ProgressBar _departmentProgressBar;
        Mode _currentMode = Mode.Orders;
        bool _isPaused = false;

        #endregion

        #region Methods

        public CheckInFragment()
        {
            _checkInViewModel = new CheckInViewModel();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Arguments == null)
            {
                throw new InvalidOperationException("Arguments cannot be null.");
            }

            var mode = Arguments.GetString(BUNDLE_EXTRA_MODE);
            if (!string.IsNullOrEmpty(mode))
                _currentMode = mode == Mode.Orders.ToString() ? Mode.Orders : Mode.Batches;
        }

        /// <summary>
        /// Called to have the fragment instantiate its user interface view.
        /// </summary>
        /// <param name="inflater">The LayoutInflater object that can be used to inflate
        /// any views in the fragment,</param>
        /// <param name="container">If non-null, this is the parent view that the fragment's
        /// UI should be attached to.  The fragment should not add the view itself,
        /// but this can be used to generate the LayoutParams of the view.</param>
        /// <param name="savedInstanceState">If non-null, this fragment is being re-constructed
        /// from a previous saved state as given here.</param>
        /// <returns>
        /// Inflated View.
        /// </returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.CheckInFragmentLayout, null);

            _orderViewModel = ServiceContainer.Resolve<OrderViewModel>();
            _batchViewModel = ServiceContainer.Resolve<BatchViewModel>();
            _orderProcessViewModel = ServiceContainer.Resolve<ProcessViewModel>();
            _batchProcessViewModel = ServiceContainer.Resolve<BatchProcessViewModel>();

            _cancelButton = view.FindViewById<Button>(Resource.Id.buttonCancel);
            _checkInButton = view.FindViewById<Button>(Resource.Id.buttonCheckIn);
            _scanButton = view.FindViewById<Button>(Resource.Id.imageButtonScan);
            _workOrderAutoTextView = view.FindViewById<AutoCompleteTextView>(Resource.Id.autoEditTextWorkOrder);
            _nextDepartmentTextView = view.FindViewById<TextView>(Resource.Id.textViewDepartment);
            _departmentProgressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBarDepartment);

            _workOrderIdAdapter = new ArrayAdapter<string>(Activity, global::Android.Resource.Layout.SimpleDropDownItem1Line);
            _workOrderAutoTextView.Adapter = _workOrderIdAdapter;

            _cancelButton.Click += (sender, e) => CancelCheckIn();
            _scanButton.Click += (sender, e) => ScanForOrder();
            _checkInButton.Click += (sender, e) => CheckInOrderAsync();
            _workOrderAutoTextView.ItemClick += (sender, e) => WorkOrderSelected(e.Position);
            _workOrderAutoTextView.TextChanged += (sender, e) => WorkOrderTextChanged();
            _workOrderAutoTextView.SetOnEditorActionListener(this);
                           
            return view;
        }

        /// <summary>
        /// Called when the fragment is visible to the user and actively running.
        /// </summary>
        public override async void OnResume()
        {
            base.OnResume();
            _isPaused = false;
            var workOrders = _currentMode == Mode.Orders 
                ? _checkInViewModel.GetAvailableCheckInOrderIds()
                : _checkInViewModel.GetAvailableCheckInBatchIds();
            var workOrderStrings = workOrders.Select(id => id.ToString()).ToList();
            _workOrderIdAdapter.Clear();
            _workOrderIdAdapter.AddAll(workOrderStrings);

            if (_currentMode == Mode.Orders && _orderViewModel.ActiveOrder != null)
            {
                _checkInViewModel.OrderId = _orderViewModel.ActiveOrder.OrderId;
                _workOrderAutoTextView.Text = _checkInViewModel.OrderId.ToString();
                await LoadNextDepartmentAsync();
            }
            else if (_batchViewModel.ActiveBatch != null)
            {
                _checkInViewModel.BatchId = _batchViewModel.ActiveBatch.BatchId;
                _workOrderAutoTextView.Text = _checkInViewModel.BatchId.ToString();
                await LoadNextDepartmentAsync();
            }
        }

        public override void OnPause()
        {
            base.OnPause();
            _isPaused = true;
        }

        public override void OnDestroy()
        {
            _cancelButton.Dispose();
            _checkInButton.Dispose();
            _workOrderAutoTextView.Dispose();
            _scanButton.Dispose();
            _nextDepartmentTextView.Dispose();
            _departmentProgressBar.Dispose();

            base.OnDestroy();
        }

        /// <summary>
        /// Override to build your own custom Dialog container.
        /// </summary>
        /// <param name="savedInstanceState">The last saved instance state of the Fragment,
        /// or null if this is a freshly created Fragment.</param>
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var dialog = base.OnCreateDialog(savedInstanceState);
            dialog.SetTitle("Check In");
            dialog.SetCanceledOnTouchOutside(false);
            dialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            return dialog;
        }

        /// <summary>
        /// Sets up View Model as the result of a work order being selected
        /// </summary>
        private async void WorkOrderSelected(int itemPosition)
        {
            var item = (string)_workOrderIdAdapter.GetItem(itemPosition);
            int id = Convert.ToInt32(item);
            if (_currentMode == Mode.Orders)
                _checkInViewModel.OrderId = id;
            else
                _checkInViewModel.BatchId = id;
            _nextDepartmentTextView.Text = string.Empty;

            if (id > -1)
            {
                await LoadNextDepartmentAsync();
            }
        }

        private void WorkOrderTextChanged()
        {
            _checkInViewModel.NextDepartment = string.Empty;
            _nextDepartmentTextView.Text = _checkInViewModel.NextDepartment;
        }

        private async Task LoadNextDepartmentAsync()
        {
            _nextDepartmentTextView.Visibility = ViewStates.Invisible;
            _departmentProgressBar.Visibility = ViewStates.Visible;

            if (_currentMode == Mode.Orders && _checkInViewModel.OrderId.HasValue)
            {
                var result = await _orderProcessViewModel.GetOrderProcessesAsync(_checkInViewModel.OrderId.Value);
                if (!_isPaused)
                {
                    _nextDepartmentTextView.Visibility = ViewStates.Visible;
                    _departmentProgressBar.Visibility = ViewStates.Invisible;

                    if (result.Success == true && string.IsNullOrEmpty(result.ErrorMessage))
                        _checkInViewModel.NextDepartment = _orderProcessViewModel.OrderStatus?.NextDepartment;
                    else
                        _checkInViewModel.NextDepartment = string.Empty;

                    _nextDepartmentTextView.Text = !(string.IsNullOrEmpty(_checkInViewModel.NextDepartment)) ? _checkInViewModel.NextDepartment :
                        OrderViewModel.NONE_TEXT;
                }
            }
            else if (_currentMode == Mode.Batches && _checkInViewModel.BatchId.HasValue)
            {
                var result = await _batchProcessViewModel.GetBatchProcessesAsync(_checkInViewModel.BatchId.Value);
                if (!_isPaused)
                {
                    _nextDepartmentTextView.Visibility = ViewStates.Visible;
                    _departmentProgressBar.Visibility = ViewStates.Invisible;

                    if (result.Success == true && string.IsNullOrEmpty(result.ErrorMessage))
                        _checkInViewModel.NextDepartment = _batchProcessViewModel.BatchStatus?.NextDepartment;
                    else
                        _checkInViewModel.NextDepartment = string.Empty;

                    _nextDepartmentTextView.Text = !(string.IsNullOrEmpty(_checkInViewModel.NextDepartment)) ? _checkInViewModel.NextDepartment :
                        OrderViewModel.NONE_TEXT;
                }
            }
        }

        /// <summary>
        /// Initiates a barcode scan for order.
        /// </summary>
        private async void ScanForOrder()
        {
            MobileBarcodeScanner.Initialize(Activity.Application);
            var scanner = new MobileBarcodeScanner();
            scanner.UseCustomOverlay = false;
            scanner.TopText = "Hold the camera up to the barcode\nAbout 6 inches away";
            scanner.BottomText = "Wait for the barcode to automatically scan!";
            var options = new MobileBarcodeScanningOptions
            {
                PossibleFormats = new List<ZX.BarcodeFormat> { ZX.BarcodeFormat.CODE_128 }
            };
            var result = await scanner.Scan(options);
            var message = "Unable to find barcode.";
            Task loadNextDepartmentTask = null;

            if (result != null && result.Text != null)
            {
                if (_currentMode == Mode.Orders)
                {
                    _nextDepartmentTextView.Text = string.Empty;
                    _checkInViewModel.SetOrderIdFromScan(result.Text);
                    if (_checkInViewModel.OrderId.HasValue && _checkInViewModel.OrderId.Value > -1)
                    {
                        var position = _workOrderIdAdapter.GetPosition(_checkInViewModel.OrderId.ToString());
                        if (position > -1)
                        {
                            loadNextDepartmentTask = LoadNextDepartmentAsync();
                            message = string.Format("Work Order: {0} found. ", _checkInViewModel.OrderId.Value);
                        }
                        else
                        {
                            _nextDepartmentTextView.Text = string.Empty;
                            message = string.Format("Work Order: {0} is not valid", _checkInViewModel.OrderId.Value);
                        }
                    }
                }
                else
                {
                    _nextDepartmentTextView.Text = string.Empty;
                    _checkInViewModel.SetBatchIdFromScan(result.Text);
                    if (_checkInViewModel.BatchId.HasValue && _checkInViewModel.BatchId.Value > -1)
                    {
                        var position = _workOrderIdAdapter.GetPosition(_checkInViewModel.BatchId.ToString());
                        if (position > -1)
                        {
                            loadNextDepartmentTask = LoadNextDepartmentAsync();
                            message = string.Format("Work Order: {0} found. ", _checkInViewModel.OrderId.Value);
                        }
                        else
                        {
                            _nextDepartmentTextView.Text = string.Empty;
                            message = string.Format("Work Order: {0} is not valid", _checkInViewModel.BatchId.Value);
                        }
                    }
                }

                LogInfo(message: "CheckInFragment:ScanForOrder Executed");
            }

            DisplayShortToast(message);

            if (loadNextDepartmentTask != null)
                await loadNextDepartmentTask;
        }

        private async void CheckInOrderAsync()
        {
            int id = -1;
            var errorMessage = string.Format("Order: {0} is not valid to check in", _workOrderAutoTextView.Text);
                
            if (Int32.TryParse(_workOrderAutoTextView.Text, out id))
            {
                if (_currentMode == Mode.Orders)
                {
                    _checkInViewModel.OrderId = id;
                    if (string.IsNullOrEmpty(_checkInViewModel.NextDepartment))
                        await LoadNextDepartmentAsync();
                    if (_checkInViewModel.IsValid)
                    {
                        var callback = (ICheckInDialogCallback)Activity;
                        callback.CheckInDialogDismissed(this, true, _checkInViewModel);
                    }
                    else
                    {
                        errorMessage = _checkInViewModel.Error;
                        DisplayShortToast(errorMessage);
                    }
                }
                else
                {
                    _checkInViewModel.BatchId = id;
                    if (string.IsNullOrEmpty(_checkInViewModel.NextDepartment))
                        await LoadNextDepartmentAsync();
                    if (_checkInViewModel.IsValid)
                    {
                        var callback = (ICheckInDialogCallback)Activity;
                        callback.CheckInDialogDismissed(this, true, _checkInViewModel);
                    }
                    else
                    {
                        errorMessage = _checkInViewModel.Error;
                        DisplayShortToast(errorMessage);
                    } 
                }
            }
            else
                DisplayShortToast(errorMessage);
        }

        private void CancelCheckIn()
        {
            var callback = (ICheckInDialogCallback)Activity;
            callback.CheckInDialogDismissed(this, false, null);
        }

        private void DisplayShortToast(string message)
        {
            Toast.MakeText(Activity, message, ToastLength.Short).Show();
        }

        public bool OnEditorAction(TextView view, ImeAction actionId, KeyEvent e)
        {
            var imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(view.ApplicationWindowToken, HideSoftInputFlags.None);

            return true;
        }

        private void LogInfo(string message)
        {
            var logService = ServiceContainer.Resolve<ILogService>();
            logService.LogInfoAsync(message, Activity);
        }

        #endregion
    }
}