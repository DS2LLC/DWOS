using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using DWOS.ViewModels;
using DWOS.Utilities;
using System.Threading.Tasks;
using DWOS.Services.Messages;
using System.ComponentModel;

namespace DWOS.Android
{
    /// <summary>
    /// Shows a list of orders.
    /// </summary>
    public class OrdersListFragment : ListFragment
    {
        #region Fields

        const string BUNDLEID_LISTVIEWSTATE = "ordersListViewState";
        const string BUNDLEID_USESEARCHRESULTS = "useSearchResults";
        OrderViewModel _orderViewModel;
        IParcelable _listViewState;
        
        #endregion

        #region Properties

        public bool UseSearchResults { get; set; }

        #endregion

        #region Methods

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (savedInstanceState != null)
            {
                _listViewState = savedInstanceState.GetParcelable(BUNDLEID_LISTVIEWSTATE) as IParcelable;
                UseSearchResults = savedInstanceState.GetBoolean(BUNDLEID_USESEARCHRESULTS, defaultValue: false);
            }
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

            _orderViewModel = ServiceContainer.Resolve<OrderViewModel>();
            return inflater.Inflate(Resource.Layout.OrdersFragmentLayout, null);
        }

        /// <summary>
        /// Called when the fragment is visible to the user and actively running.
        /// </summary>
        public override async void OnResume()
        {
            base.OnResume();
            RegisterViewModelEvents();
            if (!UseSearchResults)
            {
                if (_orderViewModel.Orders != null)
                    LoadOrderSummaries();
                else
                    await LoadOrderSummariesAsync();
            }

            if (_orderViewModel.ActiveOrder != null)
                SetSelectedOrder(_orderViewModel.ActiveOrder.OrderId);

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
        public async Task LoadOrderSummariesAsync()
        {
            var result = await _orderViewModel.GetOrderSummariesAsync();
            if (!result.Success && IsAdded)
            {
                var toastMessage = string.Format("Unable to load Orders: {0}", result.ErrorMessage);
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
            if (ListView != null && _orderViewModel.Orders != null)
            {
                var ordersListAdapter = ListAdapter as OrdersListAdapter;
                if (ordersListAdapter != null)
                {
                    var state = ListView.OnSaveInstanceState();
                    ordersListAdapter.Orders = _orderViewModel.Orders;
                    ordersListAdapter.NotifyDataSetChanged();
                    ListView.OnRestoreInstanceState(state);
                }

                LoadCountTextView();

                if (_orderViewModel.Orders.Count < 1)
                    ListView.EmptyView.Visibility = ViewStates.Gone;
            }
        }

        /// <summary>
        /// Loads the order summaries. Builds a new Adapter to populate
        /// </summary>
        private void LoadOrderSummaries()
        {
            var orders = _orderViewModel.Orders ?? new List<OrderInfo>();

            ListAdapter = new OrdersListAdapter(Activity, orders);
            if (_listViewState != null)
                ListView.OnRestoreInstanceState(_listViewState);
            if (_orderViewModel.Orders.Count < 1)
                ListView.EmptyView.Visibility = ViewStates.Gone;
            LoadCountTextView();
        }

        /// <summary>
        /// Loads the order search results. Builds a new Adapter to populate
        /// </summary>
        public void LoadOrderSearchResults()
        {
            if (_orderViewModel.OrderSearchResults != null && _orderViewModel.OrderSearchResults.Count > 0)
            {
                ListAdapter = new OrdersListAdapter(Activity, _orderViewModel.OrderSearchResults);
                if (_listViewState != null)
                    ListView.OnRestoreInstanceState(_listViewState);
            }
            else
            {
                ListAdapter = null;
                ListView.EmptyView.Visibility = ViewStates.Gone;
            }
            
            LoadResultsTextView();
        }

        private void LoadResultsTextView()
        {
            var countLayout = View.FindViewById<LinearLayout>(Resource.Id.layoutCount);
            var countTitleTextView = View.FindViewById<TextView>(Resource.Id.textViewCountTitle);
            var countTextView = View.FindViewById<TextView>(Resource.Id.textViewCount);
            countLayout.Visibility = ViewStates.Visible;
            countTitleTextView.Text = "RESULTS";
            countTextView.Text = _orderViewModel.OrderSearchResults != null ? _orderViewModel.OrderSearchResults.Count.ToString() : "0";
        }

        private void LoadCountTextView()
        {
            var countLayout = View.FindViewById<LinearLayout>(Resource.Id.layoutCount);
            var countTitleTextView = View.FindViewById<TextView>(Resource.Id.textViewCountTitle);
            var countTextView = View.FindViewById<TextView>(Resource.Id.textViewCount);
            countLayout.Visibility = ViewStates.Visible;
            countTitleTextView.Text = "TOTAL";
            countTextView.Text = _orderViewModel.Orders.Count.ToString();
            if (!string.IsNullOrEmpty(_orderViewModel.FilterDepartmentValue) || !string.IsNullOrEmpty(_orderViewModel.FilterStatusValue))
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
            outState.PutBoolean(BUNDLEID_USESEARCHRESULTS, UseSearchResults);
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
            var callback = Activity as IOrdersFragmentCallback;
            if (callback != null)
                callback.OnOrderSelected((int)id);
        }

        /// <summary>
        /// Sets the selected order.
        /// </summary>
        /// <param name="orderInfo">The order information.</param>
        /// <exception cref="System.ArgumentNullException">orderInfo</exception>
        public void SetSelectedOrder(int orderId)
        {
            var adapter = ListAdapter as OrdersListAdapter;
            if (adapter != null && IsAdded && !IsRemoving)
            {
                var currentPosition = orderId > -1 ? adapter.GetPosition(orderId) : -1;
                ListView.SetSelection(currentPosition);
                ListView.SetItemChecked(currentPosition, value: true);
            }
        }

        /// <summary>
        /// Called when it is time to register to view model events.
        /// </summary>
        private void RegisterViewModelEvents()
        {
            _orderViewModel.PropertyChanged += OrderViewModel_OnPropertyChanged;
        }

        /// <summary>
        /// Called when it is time to unregister the view model events.
        /// </summary>
        private void UnregisterViewModelEvents()
        {
            _orderViewModel.PropertyChanged -= OrderViewModel_OnPropertyChanged;
        }

        /// <summary>
        /// Handles the OnPropertyChanged event of the OrderViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OrderViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Orders" && !UseSearchResults)
            {
                if (ListAdapter != null)
                    RefreshOrderSummaries();
                else
                    LoadOrderSummaries();
            }
        }

        #endregion
    }
}

