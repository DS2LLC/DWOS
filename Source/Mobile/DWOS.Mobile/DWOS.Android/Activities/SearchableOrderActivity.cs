using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Content.PM;
using DWOS.Utilities;
using System.Threading.Tasks;
using Android.Support.V4.App;
using DWOS.ViewModels;

namespace DWOS.Android
{
    /// <summary>
    /// Activity that allows users to see order search results.
    ///
    /// </summary>
    /// <remarks>
    /// <see cref="SearchableOrderActivity"/> inherites from the
    /// <see cref="MainActivity"/> to take advantage of its codebase and
    /// logic (particularly the Action Bar).
    /// </remarks>
    [Activity(Label = "Search Orders", Name = "dwos.android.SearchableOrderActivity", LaunchMode = LaunchMode.SingleTop, ParentActivity = typeof(MainActivity), Theme = "@style/Theme.NoActionBar")]
    [IntentFilter(new string[] { Intent.ActionSearch, "android.intent.action.SEARCH_LONG_PRESS" })]
    [MetaData("android.app.searchable", Resource = "@xml/searchable")]
    public class SearchableOrderActivity : MainActivity
    {
        #region Fields

        const string BUNDLEID_SEARCHTERMS = "searchTerms";
        string _searchTerms;
        OrderViewModel _orderViewModel;
        bool _isSearchAdvanced = false;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchableOrderActivity"/> class.
        /// </summary>
        public SearchableOrderActivity()
            : base()
        {
            _orderViewModel = ServiceContainer.Resolve<DWOS.ViewModels.OrderViewModel>();
        }

        /// <summary>
        /// Called when the Activity is first created.
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SupportActionBar.SetDisplayHomeAsUpEnabled(showHomeAsUp: true);

            if (!OrdersListFragment.IsAdded)
            {
                var transaction = FragmentManager.BeginTransaction();
                transaction.Add(Resource.Id.frameLayoutOrders, OrdersListFragment, ORDER_LIST_FRAGMENT_TAG);
                if (IsFrameDetailsVisible)
                    transaction.Add(Resource.Id.frameLayoutOrderDetails, OrdersDetailsFragment, ORDER_DETAILS_FRAGMENT_TAG);
                transaction.Commit();
            }

            if (bundle != null)
                _searchTerms = bundle.GetString(BUNDLEID_SEARCHTERMS);

            if (string.IsNullOrEmpty(_searchTerms))
                HandleIntent(Intent);
        }

        /// <summary>
        /// Standard implementation of
        /// <c><see cref="M:Android.Views.LayoutInflater.IFactory2.OnCreateView(Android.Views.View, System.String, System.String, System.String)" /></c>
        /// used when inflating with the LayoutInflater returned by <c><see cref="M:Android.Content.ContextWrapper.GetSystemService(System.String)" /></c>.
        /// </summary>
        public override View OnCreateView(View parent, string name, Context context, global::Android.Util.IAttributeSet attrs)
        {
            var view = base.OnCreateView(parent, name, context, attrs);
            var ordersFragment = FragmentManager.FindFragmentByTag<OrdersListFragment>(ORDER_LIST_FRAGMENT_TAG);
            if (ordersFragment != null)
                ordersFragment.UseSearchResults = true;

            return view;
        }

        /// <summary>
        /// This is called for activities that set launchMode to "singleTop" in
        /// their package, or if a client used the <c><see cref="F:Android.Content.ActivityFlags.SingleTop" /></c>
        /// flag when calling <c><see cref="M:Android.Content.ContextWrapper.StartActivity(Android.Content.Intent)" /></c>.
        /// </summary>
        protected async override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            HandleIntent(intent);
            if (_isSearchAdvanced)
                await FindOrdersOnServerAsync();
            else
                FindOrders();
        }
        
        /// <summary>
        /// Called after <c><see cref="M:Android.App.Activity.OnRestoreInstanceState(Android.OS.Bundle)" /></c>, <c><see cref="M:Android.App.Activity.OnRestart" /></c>, or
        /// <c><see cref="M:Android.App.Activity.OnPause" /></c>, for your activity to start interacting with the user.
        /// </summary>
        protected async override void OnResume()
        {
            try
            {
                base.OnResume();
                if (!string.IsNullOrEmpty(_searchTerms))
                {
                    if (_isSearchAdvanced)
                        await FindOrdersOnServerAsync();
                    else
                        FindOrders();
                    var ordersFragment = FragmentManager.FindFragmentByTag<OrdersListFragment>(ORDER_LIST_FRAGMENT_TAG);
                    if (ordersFragment != null)
                    {
                        ordersFragment.LoadOrderSearchResults();
                        if (_orderViewModel.OrderSearchResults != null &&
                            _orderViewModel.OrderSearchResults.Count > 0)
                        {
                            var orderId = _orderViewModel.ActiveOrder != null ? _orderViewModel.ActiveOrder.OrderId : _orderViewModel.OrderSearchResults[0].OrderId;
                            ordersFragment.SetSelectedOrder(orderId);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error in OnResume", exception);
                DWOSApplication.Current.RestoreMainActivityFromCrash = true;
                base.RestartApp();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.OnCreateOptionsMenu(menu);
            menu.FindItem(Resource.Id.action_search).SetVisible(false);
            menu.FindItem(Resource.Id.action_searchAdvanced).SetVisible(false);
            menu.FindItem(Resource.Id.action_filter).SetVisible(false);
            return true;
        }

        private void HandleIntent(Intent intent)
        {
            if (intent != null && Intent.ActionSearch.Equals(intent.Action))
            {
                _searchTerms = intent.GetStringExtra(SearchManager.Query);
                _isSearchAdvanced = intent.GetBooleanExtra(BUNDLEID_IS_SEARCH_ADVANCED, defaultValue: false);
            }
        }

        private void FindOrders()
        {
            _orderViewModel.FindOrders(_searchTerms);
        }

        private async Task FindOrdersOnServerAsync()
        {
            await _orderViewModel.FindOrdersAsync(_searchTerms);
        }

        /// <summary>
        /// Called as part of the activity lifecycle when an activity is going into
        /// the background, but has not (yet) been killed.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
        }

        /// <summary>
        /// Called to retrieve per-instance state from an activity before being killed
        /// so that the state can be restored in <c><see cref="M:Android.App.Activity.OnCreate(Android.OS.Bundle)" /></c> or
        /// <c><see cref="M:Android.App.Activity.OnRestoreInstanceState(Android.OS.Bundle)" /></c> (the <c><see cref="T:Android.OS.Bundle" /></c> populated by this method
        /// will be passed to both).
        /// </summary>
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            if (!string.IsNullOrEmpty(_searchTerms))
                outState.PutString(BUNDLEID_SEARCHTERMS, _searchTerms);
        }

        /// <summary>
        /// Called when [options item selected].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                if (item.ItemId == global::Android.Resource.Id.Home)
                {
                    OrderViewModel.RequestInvalidateViewModel();
                    Finish();
                    return true;
                }
                return base.OnOptionsItemSelected(item);
            }
            catch (Exception exception)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                var optionItemException = new Exception("Error in SearchableOrderActivity.OnOptionsItemSelected: " + item.ItemId, exception);
                logService.LogError(_standardErrorMessage, optionItemException, this, true);
                return base.OnOptionsItemSelected(item);
            }
        }

        public override void OnBackPressed()
        {
            OrderViewModel.RequestInvalidateViewModel();
            base.OnBackPressed();
        }

        #endregion
    }
}