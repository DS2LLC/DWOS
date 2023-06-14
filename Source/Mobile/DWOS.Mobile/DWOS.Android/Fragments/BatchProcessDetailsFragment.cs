using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DWOS.ViewModels;
using DWOS.Utilities;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DWOS.Android
{
    /// <summary>
    /// Shows process details for the current batch.
    /// </summary>
    public class BatchProcessDetailsFragment : ListFragment
    {
        #region Fields

        const string BUNDLEID_BATCHID = "BatchId";
        BatchProcessViewModel _batchProcessViewModel;
        BatchViewModel _batchViewModel;

        int _batchId = -1;

        #endregion

        #region Methods

        /// <summary>
        /// Called to do initial creation of a fragment.
        /// </summary>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _batchProcessViewModel = ServiceContainer.Resolve<BatchProcessViewModel>();
            _batchViewModel = ServiceContainer.Resolve<BatchViewModel>();

            HandleIntent(Activity.Intent);
            if (savedInstanceState != null)
                _batchId = savedInstanceState.GetInt(BUNDLEID_BATCHID, -1);
        }

        /// <summary>
        /// Called to have the fragment instantiate its user interface view.
        /// </summary>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.ProcessDetailsLayout, null);

            return view;
        }

        /// <summary>
        /// Called when the fragment is visible to the user and actively running.
        /// </summary>
        public override async void OnResume()
        {
            base.OnResume();
            RegisterViewModelEvents();

            if (_batchId < 0 && _batchProcessViewModel.BatchProcesses == null)
                return;

            if (_batchProcessViewModel.BatchProcesses != null &&
                (_batchId == _batchProcessViewModel.BatchId || _batchId < 0 && _batchProcessViewModel.BatchId > -1))
            {
                _batchId = _batchProcessViewModel.BatchId;
                LoadBatchProcesses();
            }
            else
                await LoadBatchProcessesAsync(_batchId);
        }

        /// <summary>
        /// Called when the Fragment is no longer resumed.
        /// </summary>
        public override void OnPause()
        {
            base.OnPause();
            UnregisterViewModelEvents();
        }

        private void HandleIntent(Intent intent)
        {
            if (intent != null)
                _batchId = intent.GetIntExtra(BatchDetailActivity.INTENT_BATCHID, defaultValue: -1);
        }

        /// <summary>
        /// Loads the batch processes asynchronously.
        /// </summary>
        /// <param name="batchId">The order identifier.</param>
        /// <returns></returns>
        public async Task LoadBatchProcessesAsync(int batchId)
        {
            var result = await _batchProcessViewModel.GetBatchProcessesAsync(batchId);
            if (!result.Success && IsAdded)
            {
                var toastMessage = string.Format("Unable to load Batch Processes: {0}", result.ErrorMessage);
                Toast.MakeText(Activity, toastMessage, ToastLength.Long)
                    .Show();
            }
        }

        private void LoadBatchProcesses()
        {
            if (_batchProcessViewModel.BatchProcesses != null)
            {
                ListAdapter = new BatchProcessDetailsAdapter(Activity, _batchProcessViewModel.BatchProcesses);
                _batchId = _batchProcessViewModel.BatchId;
            }
            else
            {
                ListAdapter = null;
                _batchId = -1;
            }
        }

        /// <summary>
        /// Called to ask the fragment to save its current dynamic state, so it
        /// can later be reconstructed in a new instance of its process is
        /// restarted.
        /// </summary>
        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            if (_batchViewModel.ActiveBatch != null)
                outState.PutInt(BUNDLEID_BATCHID, _batchId);
        }

        /// <summary>
        /// This method will be called when an item in the list is selected.
        /// </summary>
        /// <param name="listView">The ListView where the click happened</param>
        /// <param name="view">The view that was clicked within the ListView</param>
        /// <param name="position">The position of the view in the list</param>
        /// <param name="id">The row id of the item that was clicked</param>
        public override void OnListItemClick(ListView listView, View view, int position, long id)
        {
            base.OnListItemClick(listView, view, position, id);
            var callback = Activity as IBatchProcessDetailsFragmentCallback;
            if (callback != null)
            {
                var processDetailsAdapter = ListAdapter as BatchProcessDetailsAdapter;
                if (processDetailsAdapter != null)
                {
                    var processInfo = processDetailsAdapter.Processes[position];
                    callback.OnBatchProcessDetailsSelected(processInfo);
                }
            }
        }

        /// <summary>
        /// Called when it is time to register to view model events.
        /// </summary>
        private void RegisterViewModelEvents()
        {
            _batchProcessViewModel.PropertyChanged += ProcessViewModel_OnPropertyChanged;
        }

        /// <summary>
        /// Called when it is time to unregister the view model events.
        /// </summary>
        private void UnregisterViewModelEvents()
        {
            _batchProcessViewModel.PropertyChanged -= ProcessViewModel_OnPropertyChanged;
        }

        /// <summary>
        /// Handles the OnPropertyChanged event of the OrderViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void ProcessViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BatchProcesses")
                LoadBatchProcesses();
        }

        #endregion
    }
}