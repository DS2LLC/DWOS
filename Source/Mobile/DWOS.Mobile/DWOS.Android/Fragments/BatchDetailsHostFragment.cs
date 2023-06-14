using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V4.View;
using System.Threading.Tasks;

namespace DWOS.Android
{
    /// <summary>
    /// Shows multi-tab details for the current batch.
    /// </summary>
    public class BatchDetailsHostFragment : Fragment
    {
        #region Fields

        public const string BATCHDETAILS_FRAGMENT_TAG = "BatchDetailsFragment";
        public const string BUNDLEID_BATCHID = "BatchId";

        BatchDetailsFragmentAdapter _pagerAdapter;
        ViewPager _viewPager;

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
            var view = inflater.Inflate(Resource.Layout.BatchDetailsHostFragmentLayout, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            _viewPager = view.FindViewById<ViewPager>(Resource.Id.detailsPager);
            _pagerAdapter = new BatchDetailsFragmentAdapter(ChildFragmentManager);
            _viewPager.Adapter = _pagerAdapter;
            _viewPager.OffscreenPageLimit = _pagerAdapter.Count;
        }

        /// <summary>
        /// Coordinates with hosted fragments to load the batch asynchronously.
        /// </summary>
        /// <param name="batchId">The batch identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task LoadBatchAsync(int batchId)
        {
            if (batchId < 0)
                return;

            var batchDetailsFragment = _pagerAdapter.InstantiateItem(_viewPager, 0) as BatchDetailsFragment;
            var processDetailsFragment = _pagerAdapter.InstantiateItem(_viewPager, 1) as BatchProcessDetailsFragment;
            if (batchDetailsFragment != null)
                await batchDetailsFragment.LoadBatchDetailsAsync(batchId);
            if (processDetailsFragment != null)
                await processDetailsFragment.LoadBatchProcessesAsync(batchId);
        }

        public override void OnDestroyView()
        {
            _viewPager.Dispose();
            base.OnDestroyView();
        }

        #endregion
    }
}