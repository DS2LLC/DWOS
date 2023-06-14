using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using System.Threading.Tasks;
using DWOS.ViewModels;
using DWOS.Utilities;

namespace DWOS.Android
{
    /// <summary>
    /// Shows multi-tab details for the current order.
    /// </summary>
    public class OrderDetailsHostFragment : Fragment
    {
        #region Fields

        public const string BUNDLEID_ORDERID = "OrderId";

        OrderDetailsFragmentAdapter _pagerAdapter;
        ViewPager _viewPager;
        OrderViewModel _orderViewModel;
        ProcessViewModel _processViewModel;
        PartViewModel _partViewModel;
        OrderNotesViewModel _orderNotesViewModel;

        #endregion

        #region Methods

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
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _orderViewModel = ServiceContainer.Resolve<OrderViewModel>();
            _processViewModel = ServiceContainer.Resolve<ProcessViewModel>();
            _partViewModel = ServiceContainer.Resolve<PartViewModel>();
            _orderNotesViewModel = ServiceContainer.Resolve<OrderNotesViewModel>();

            var view = inflater.Inflate(Resource.Layout.OrderDetailsHostFragmentLayout, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            _viewPager = view.FindViewById<ViewPager>(Resource.Id.detailsPager);
            _pagerAdapter = new OrderDetailsFragmentAdapter(ChildFragmentManager);
            _viewPager.Adapter = _pagerAdapter;
            _viewPager.OffscreenPageLimit = _pagerAdapter.Count;
        }

        /// <summary>
        /// Coordinates with hosted fragments to load the order asynchronously.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task LoadOrderAsync(int orderId)
        {
            if (orderId < 0)
                return;

            var orderDetailsFragment = _pagerAdapter.InstantiateItem(_viewPager, 0) as OrderDetailsFragment;
            var imageSize = 512;
            if (orderDetailsFragment != null)
                imageSize = orderDetailsFragment.RequestedImageSize;

            // Make all requests at the same time for improved performance
            var orderResultTask = _orderViewModel.SetActiveOrderDetailAsync(orderId, imageSize);
            var processResultTask = _processViewModel.GetOrderProcessesAsync(orderId);
            var partResultTask = _partViewModel.GetPartAsync(orderId);
            var noteResultTask = _orderNotesViewModel.GetNotesAsync(orderId);

            var allResults = await Task.WhenAll(orderResultTask, processResultTask, partResultTask, noteResultTask);

            // Show any error message from the results
            if (IsAdded)
            {
                // Order
                if (!allResults[0].Success)
                {
                    var toastMessage = string.Format("Unable to load Order: {0}", allResults[0].ErrorMessage);
                    Toast.MakeText(Activity, toastMessage, ToastLength.Long)
                        .Show();
                }

                // Processes
                if (!allResults[1].Success)
                {
                    var toastMessage = string.Format("Unable to load Order Processes: {0}", allResults[1].ErrorMessage);
                    Toast.MakeText(Activity, toastMessage, ToastLength.Long)
                        .Show();
                }

                // Part
                if (!allResults[2].Success)
                {
                    var toastMessage = string.Format("Unable to load Part Orders: {0}", allResults[2].ErrorMessage);
                    Toast.MakeText(Activity, toastMessage, ToastLength.Long)
                        .Show();
                }

                // Order Notes
                if (!allResults[3].Success)
                {
                    var toastMessage = string.Format("Unable to load order notes: {0}", allResults[3].ErrorMessage);
                    Toast.MakeText(Activity, toastMessage, ToastLength.Long)
                        .Show();
                }
            }
        }

        public override void OnDestroyView()
        {
            _viewPager.Dispose();
            base.OnDestroyView();
        }

        #endregion
    }
}