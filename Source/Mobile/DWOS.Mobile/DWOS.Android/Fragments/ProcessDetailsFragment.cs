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
    /// Lists processes for an order.
    /// </summary>
    public class ProcessDetailsFragment : ListFragment
    {
        #region Fields

        const string BUNDLEID_ORDERID = "OrderId";
        ProcessViewModel _processViewModel;
        OrderViewModel _orderViewModel;

        int _orderId = -1;

        #endregion

        #region Methods

        /// <summary>
        /// Called to do initial creation of a fragment.
        /// </summary>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _processViewModel = ServiceContainer.Resolve<ProcessViewModel>();
            _orderViewModel = ServiceContainer.Resolve<OrderViewModel>();

            HandleIntent(Activity.Intent);
            if (savedInstanceState != null)
                _orderId = savedInstanceState.GetInt(BUNDLEID_ORDERID, -1);
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
        public override void OnResume()
        {
            base.OnResume();
            RegisterViewModelEvents();

            if (_orderId < 0 && _processViewModel.OrderProcesses == null)
                return;

            if (_processViewModel.OrderProcesses != null &&
                (_orderId == _processViewModel.OrderId ||
                _orderId < 0 && _processViewModel.OrderId > 0))
                LoadOrderProcesses();
        }

        public override void OnPause()
        {
            base.OnPause();
            UnregisterViewModelEvents();
        }

        private void HandleIntent(Intent intent)
        {
            if (intent != null)
                _orderId = intent.GetIntExtra(OrderDetailActivity.INTENT_ORDERID, defaultValue: -1);
        }

        private void LoadOrderProcesses()
        {
            if (_processViewModel.OrderProcesses != null)
            {
                ListAdapter = new ProcessDetailsAdapter(Activity, _processViewModel.OrderProcesses);
                _orderId = _processViewModel.OrderId;
            }
            else
            {
                ListAdapter = null;
                _orderId = -1;
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
            if (_orderViewModel.ActiveOrder != null)
                outState.PutInt(BUNDLEID_ORDERID, _orderId);
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
            var callback = Activity as IProcessDetailsFragmentCallback;
            if (callback != null)
            {
                var processDetailsAdapter = ListAdapter as ProcessDetailsAdapter;
                if (processDetailsAdapter != null)
                {
                    var processInfo = processDetailsAdapter.Processes[position];
                    callback.OnProcessDetailsSelected(processInfo);
                }
            }
        }

        /// <summary>
        /// Called when it is time to register to view model events.
        /// </summary>
        private void RegisterViewModelEvents()
        {
            _processViewModel.PropertyChanged += ProcessViewModel_OnPropertyChanged;
        }

        /// <summary>
        /// Called when it is time to unregister the view model events.
        /// </summary>
        private void UnregisterViewModelEvents()
        {
            _processViewModel.PropertyChanged -= ProcessViewModel_OnPropertyChanged;
        }

        /// <summary>
        /// Handles the OnPropertyChanged event of the OrderViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void ProcessViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "OrderProcesses")
                LoadOrderProcesses();
        }

        #endregion
    }
}