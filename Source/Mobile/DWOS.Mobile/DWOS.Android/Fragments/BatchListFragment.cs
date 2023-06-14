using System.Collections.Generic;
using Android.App;
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
    /// Shows a list of batches.
    /// </summary>
    public class BatchListFragment : ListFragment
    {
        #region Fields

        const string BUNDLEID_LISTVIEWSTATE = "batchListViewState";
        BatchViewModel _batchViewModel;
        IParcelable _listViewState;

        #endregion

        #region Methods

        /// <summary>
        /// Called to do initial creation of a fragment.
        /// </summary>
        /// <param name="savedInstanceState">If the fragment is being re-created from
        /// a previous saved state, this is the state.</param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _batchViewModel = ServiceContainer.Resolve<BatchViewModel>();
            
            if (savedInstanceState != null)
                _listViewState = savedInstanceState.GetParcelable(BUNDLEID_LISTVIEWSTATE) as IParcelable;
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
        /// To be added.
        /// </returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            //We can use the same layout
            return inflater.Inflate(Resource.Layout.OrdersFragmentLayout, null);
        }

        /// <summary>
        /// Called when the fragment is visible to the user and actively running.
        /// </summary>
        public override async void OnResume()
        {
            base.OnResume();
            RegisterViewModelEvents();

            if (_batchViewModel.Batches != null)
                LoadBatchSummaries();
            else
                await LoadBatchSummariesAsync();

            if (_batchViewModel.ActiveBatch != null)
                SetSelectedBatch(_batchViewModel.ActiveBatch.BatchId);

            Activity.InvalidateOptionsMenu();
        }

        /// <summary>
        /// Called when the Fragment is no longer resumed.
        /// </summary>
        public override void OnPause()
        {
            base.OnPause();
            UnregisterViewModelEvents();
        }

        /// <summary>
        /// Loads the order summaries asynchronously, making a load request of the View Model. 
        /// </summary>
        /// <returns></returns>
        public async Task LoadBatchSummariesAsync()
        {
            var result = await _batchViewModel.GetBatchSummariesAsync();
            if (!result.Success && IsAdded)
            {
                var toastMessage = string.Format("Unable to load Batches: {0}", result.ErrorMessage);
                Toast.MakeText(Activity, toastMessage, ToastLength.Long)
                    .Show();
            }
        }

        /// <summary>
        /// Refreshes order summaries.  Refreshes Adapter with new collection
        /// </summary>
        /// <returns></returns>
        private void RefreshOrderSummaries()
        {
            if (ListView != null && _batchViewModel.Batches != null)
            {
                var batchListAdapter = ListAdapter as BatchListAdapter;
                if (batchListAdapter != null)
                {
                    var state = ListView.OnSaveInstanceState();
                    batchListAdapter.Batches = _batchViewModel.Batches;
                    batchListAdapter.NotifyDataSetChanged();
                    ListView.OnRestoreInstanceState(state);
                }

                LoadCountTextView();

                if (batchListAdapter.Batches.Count < 1)
                    ListView.EmptyView.Visibility = ViewStates.Gone;
            }
        }

        /// <summary>
        /// Loads the order summaries. Builds a new Adapter to populate
        /// </summary>
        private void LoadBatchSummaries()
        {
            var batches = _batchViewModel.Batches ?? new List<Services.Messages.BatchInfo>();

            ListAdapter = new BatchListAdapter(Activity, batches);
            if (_listViewState != null)
                ListView.OnRestoreInstanceState(_listViewState);
            if (_batchViewModel.Batches.Count < 1)
                ListView.EmptyView.Visibility = ViewStates.Gone;
            LoadCountTextView();
        }

        private void SetSelectedBatch(int batchId)
        {
            var adapter = ListAdapter as BatchListAdapter;
            if (adapter != null)
            {
                var currentPosition = batchId > -1 ? adapter.GetPosition(batchId) : -1;
                ListView.SetSelection(currentPosition);
                ListView.SetItemChecked(currentPosition, value: true);
            }
        }

        private void LoadCountTextView()
        {
            var countLayout = View.FindViewById<LinearLayout>(Resource.Id.layoutCount);
            var countTitleTextView = View.FindViewById<TextView>(Resource.Id.textViewCountTitle);
            var countTextView = View.FindViewById<TextView>(Resource.Id.textViewCount);
            countLayout.Visibility = ViewStates.Visible;
            countTitleTextView.Text = "TOTAL";
            countTextView.Text = _batchViewModel.Batches != null ? _batchViewModel.Batches.Count.ToString() : "0";
            if (!string.IsNullOrEmpty(_batchViewModel.FilterDepartmentValue) || !string.IsNullOrEmpty(_batchViewModel.FilterStatusValue))
                countTitleTextView.Text = "TOTAL (filtered)";
        }

        /// <summary>
        /// Called to ask the fragment to save its current dynamic state, so it
        /// can later be reconstructed in a new instance of its process is
        /// restarted.
        /// </summary>
        /// <param name="outState">Bundle in which to place your saved state.</param>
        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutParcelable(BUNDLEID_LISTVIEWSTATE, ListView.OnSaveInstanceState());
        }

        /// <summary>
        /// Called when list item is click ed.
        /// </summary>
        /// <param name="listView">The list view.</param>
        /// <param name="view">The view.</param>
        /// <param name="position">The position.</param>
        /// <param name="id">The identifier.</param>
        public override void OnListItemClick(ListView listView, View view, int position, long id)
        {
            var callback = Activity as IBatchFragmentCallback;
            if (callback != null)
                callback.OnBatchSelected((int)id);
        }

        /// <summary>
        /// Sets the selected order.
        /// </summary>
        /// <param name="orderInfo">The order information.</param>
        /// <exception cref="System.ArgumentNullException">orderInfo</exception>
        public void SetSelectedOrder(int batchId)
        {
            var adapter = ListAdapter as BatchListAdapter;
            if (adapter != null)
            {
                var currentPosition = batchId > -1 ? adapter.GetPosition(batchId) : -1;
                ListView.SetSelection(currentPosition);
                ListView.SetItemChecked(currentPosition, value: true);
            }
        }

        /// <summary>
        /// Called when it is time to register to view model events.
        /// </summary>
        private void RegisterViewModelEvents()
        {
            _batchViewModel.PropertyChanged += BatchViewModel_OnPropertyChanged;
        }

        /// <summary>
        /// Called when it is time to unregister the view model events.
        /// </summary>
        private void UnregisterViewModelEvents()
        {
            _batchViewModel.PropertyChanged -= BatchViewModel_OnPropertyChanged;
        }

        /// <summary>
        /// Handles the OnPropertyChanged event of the OrderViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void BatchViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Batches")
            {
                if (ListAdapter != null)
                    RefreshOrderSummaries();
                else
                    LoadBatchSummaries();
            }
        }

        #endregion
    }
}