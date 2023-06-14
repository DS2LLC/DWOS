using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DWOS.ViewModels;
using System.Threading.Tasks;
using DWOS.Utilities;

namespace DWOS.Android
{
    /// <summary>
    /// Shows batch details for the current batch.
    /// </summary>
    public class BatchDetailsFragment : Fragment
    {
        #region Fields

        public const string BUNDLE_EXTRA_BATCH_ID = "BatchId";

        BatchViewModel _batchViewModel;
        int _batchId = -1;
        TextView _textViewBatch;
        TextView _textViewCurrentDepartment;
        TextView _textViewProcess;
        TextView _textViewWorkStatus;
        TextView _textViewCreated;
        TextView _textViewSalesOrder;
        TextView _textViewOrderCount;
        TextView _textViewSurfaceArea;
        TextView _textViewNextDepartment;
        TextView _textViewFixture;
        TextView _textViewPartQuantity;
        TextView _textViewWeight;
        TextView _textViewSchedulePriorityLabel;
        TextView _textViewSchedulePriority;

        #endregion

        #region Methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            HandleIntent(Activity.Intent);
            _batchId = savedInstanceState != null ? savedInstanceState.GetInt(BUNDLE_EXTRA_BATCH_ID, -1) : _batchId;
            _batchViewModel = ServiceContainer.Resolve<BatchViewModel>();
            var view = inflater.Inflate(Resource.Layout.BatchDetailsFragmentLayout, null);
            _textViewBatch = view.FindViewById<TextView>(Resource.Id.textViewBatch);
            _textViewCurrentDepartment = view.FindViewById<TextView>(Resource.Id.textViewCurrentDepartment);
            _textViewProcess = view.FindViewById<TextView>(Resource.Id.textViewProcess);
            _textViewWorkStatus = view.FindViewById<TextView>(Resource.Id.textViewWorkStatus);
            _textViewCreated = view.FindViewById<TextView>(Resource.Id.textViewCreated);
            _textViewSalesOrder = view.FindViewById<TextView>(Resource.Id.textViewSalesOrder);
            _textViewOrderCount = view.FindViewById<TextView>(Resource.Id.textViewOrders);
            _textViewSurfaceArea = view.FindViewById<TextView>(Resource.Id.textViewSurfaceArea);
            _textViewNextDepartment = view.FindViewById<TextView>(Resource.Id.textViewNextDepartment);
            _textViewFixture = view.FindViewById<TextView>(Resource.Id.textViewFixture);
            _textViewPartQuantity = view.FindViewById<TextView>(Resource.Id.textViewParts);
            _textViewWeight = view.FindViewById<TextView>(Resource.Id.textViewWeight);
            _textViewSchedulePriorityLabel = view.FindViewById<TextView>(Resource.Id.textViewSchedulePriorityLabel);
            _textViewSchedulePriority = view.FindViewById<TextView>(Resource.Id.textViewSchedulePriority);

            return view;
        }

        private void HandleIntent(Intent intent)
        {
            if (intent != null)
                _batchId = intent.GetIntExtra(BatchDetailActivity.INTENT_BATCHID, defaultValue: -1);
        }

        /// <summary>
        /// Called when the fragment is visible to the user and actively running.
        /// </summary>
        public override async void OnResume()
        {
            base.OnResume();
            RegisterViewModelEvents();

            if (_batchViewModel.ActiveBatch != null && _batchViewModel.ActiveBatch.BatchId > -1)
            {
                _batchId = _batchViewModel.ActiveBatch.BatchId;
                LoadBatchDetails();
            }
            else
            {
                await LoadBatchDetailsAsync(_batchId);
            }
        }

        /// <summary>
        /// Called when the Fragment is no longer resumed.
        /// </summary>
        public override void OnPause()
        {
            base.OnPause();
            UnregisterViewModelEvents();
        }

        public override void OnDestroyView()
        {
            _textViewBatch.Dispose();
            _textViewCurrentDepartment.Dispose();
            _textViewProcess.Dispose();
            _textViewWorkStatus.Dispose();
            _textViewCreated.Dispose();
            _textViewSalesOrder.Dispose();
            _textViewOrderCount.Dispose();
            _textViewSurfaceArea.Dispose();
            _textViewNextDepartment.Dispose();
            _textViewFixture.Dispose();
            _textViewPartQuantity.Dispose();
            _textViewWeight.Dispose();
            _textViewSchedulePriorityLabel.Dispose();
            _textViewSchedulePriority.Dispose();
            
            base.OnDestroyView();
        }

        /// <summary>
        /// Called to ask the fragment to save its current dynamic state, so it
        /// can later be reconstructed in a new instance of its process is
        /// restarted.
        /// </summary>
        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            if (_batchViewModel.ActiveBatch != null && _batchViewModel.ActiveBatch.BatchId == _batchId)
                outState.PutInt(BUNDLE_EXTRA_BATCH_ID, _batchId);
        }

        public async Task LoadBatchDetailsAsync(int batchId)
        {
            var result = await _batchViewModel.SetActiveBatchDetailAsync(batchId);
            if (!result.Success && IsAdded)
            {
                //Clear all text?
                var toastMessage = string.Format("Unable to load Batch: {0}", result.ErrorMessage);
                Toast.MakeText(Activity, toastMessage, ToastLength.Long)
                    .Show();
            }
        }

        private void LoadBatchDetails()
        {
            Services.Messages.BatchDetailInfo activeBatch = _batchViewModel.ActiveBatch;
            _textViewBatch.Text = activeBatch.BatchId.ToString();

            _textViewCurrentDepartment.Text = string.IsNullOrEmpty(activeBatch.CurrentLine)
                ? activeBatch.Location
                : $"{activeBatch.Location} - {activeBatch.CurrentLine}";

            _textViewProcess.Text = activeBatch.CurrentProcess;
            _textViewWorkStatus.Text = activeBatch.WorkStatus;
            _textViewCreated.Text =
                activeBatch.OpenDate == DateTime.MinValue ? string.Empty : activeBatch.OpenDate.ToShortDateString();

            _textViewSalesOrder.Text = activeBatch.SalesOrderId.HasValue
                ? activeBatch.SalesOrderId.ToString()
                : "N/A";

            _textViewOrderCount.Text = activeBatch.OrderCount.ToString();
            _textViewSurfaceArea.Text = activeBatch.TotalSurfaceArea.ToString("0 in\xB2");
            _textViewNextDepartment.Text = activeBatch.NextDept;
            _textViewFixture.Text = activeBatch.Fixture;
            _textViewPartQuantity.Text = activeBatch.PartCount.ToString();
            _textViewWeight.Text = activeBatch.TotalWeight.ToString("0 lbs");

            if (ApplicationSettings.Settings.UsingManualScheduling)
            {
                _textViewSchedulePriorityLabel.Visibility = ViewStates.Visible;
                _textViewSchedulePriority.Visibility = ViewStates.Visible;
                _textViewSchedulePriority.Text = activeBatch.SchedulePriority > 0
                    ? activeBatch.SchedulePriority.ToString()
                    : "Non-Scheduled";
            }
            else
            {
                _textViewSchedulePriorityLabel.Visibility = ViewStates.Gone;
                _textViewSchedulePriority.Visibility = ViewStates.Gone;
                _textViewSchedulePriority.Text = string.Empty;
            }
        }

        private void UnregisterViewModelEvents()
        {
            _batchViewModel.PropertyChanged -= BatchViewModel_OnPropertyChanged;
        }

        private void RegisterViewModelEvents()
        {
            _batchViewModel.PropertyChanged += BatchViewModel_OnPropertyChanged;
        }

        private void BatchViewModel_OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveBatch" && _batchViewModel.ActiveBatch != null)
                LoadBatchDetails();
        }

        #endregion
    }
}