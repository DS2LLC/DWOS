using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DWOS.ViewModels;
using System.Threading.Tasks;
using DWOS.Utilities;
using System.ComponentModel;
using Android.App;
using System;
using DWOS.Services.Messages;
using System.Collections.Generic;

namespace DWOS.Android
{
    public class OrderNotesFragment : ListFragment
    {
         #region Fields

        const string BUNDLEID_ORDERID = "OrderId";
        OrderNotesViewModel _orderNotesViewModel;
        OrderViewModel _orderViewModel;
        int _orderId = -1;
        Button _buttonAddOrder;

        #endregion

        #region Properties

        private bool CanAddNote
        {
            get
            {
                var securityRoles = ServiceContainer.Resolve<LogInViewModel>().UserProfile?.SecurityRoles
                    ?? new List<string>();

                return securityRoles.Contains(ApplicationSettings.Settings.OrderNoteAddRole)
                    || securityRoles.Contains(ApplicationSettings.Settings.EditOrderRole); ;
            }
        }

        private bool CanEditNote
        {
            get
            {
                var securityRoles = ServiceContainer.Resolve<LogInViewModel>().UserProfile?.SecurityRoles
                    ?? new List<string>();

                return securityRoles.Contains(ApplicationSettings.Settings.EditOrderRole);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called to do initial creation of a fragment.
        /// </summary>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _orderNotesViewModel = ServiceContainer.Resolve<OrderNotesViewModel>();
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
            return inflater.Inflate(Resource.Layout.OrderNotesLayout, null);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            _buttonAddOrder = view.FindViewById<Button>(Resource.Id.buttonAddNote);
            _buttonAddOrder.Click += ButtonAddOrder_Click;
        }

        /// <summary>
        /// Called when the fragment is visible to the user and actively running.
        /// </summary>
        public override void OnResume()
        {
            base.OnResume();
            RegisterViewModelEvents();

            if (_orderId < 0 && _orderNotesViewModel.OrderId < 0)
                return;

            if ((_orderId == _orderNotesViewModel.OrderId) ||
                (_orderId < 0 && _orderNotesViewModel.OrderId > 0))
            {
                _orderId = _orderNotesViewModel.OrderId;
                LoadNotes();
            }

            _buttonAddOrder.Visibility = CanAddNote
                ? ViewStates.Visible
                : ViewStates.Gone;
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
            _buttonAddOrder.Dispose();
            base.OnDestroyView();
        }

        private void HandleIntent(Intent intent)
        {
            if (intent != null)
                _orderId = intent.GetIntExtra(OrderDetailActivity.INTENT_ORDERID, defaultValue: -1);
        }

        private void LoadNotes()
        {
            if (_orderNotesViewModel.Notes != null)
            {
                ListAdapter = new OrderNotesAdapter(Activity, _orderNotesViewModel.Notes);
                _orderId = _orderNotesViewModel.OrderId;
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

        public override void OnListItemClick(ListView listView, View view, int position, long id)
        {
            base.OnListItemClick(listView, view, position, id);

            if (!CanEditNote)
            {
                return;
            }

            var callback = Activity as IOrderNotesFragmentCallback;
            var notesAdapter = ListAdapter as OrderNotesAdapter;
            if (notesAdapter != null)
            {
                var note = notesAdapter.Notes[position];
                callback?.OnNoteSelected(note);
            }
        }

        /// <summary>
        /// Called when it is time to register to view model events.
        /// </summary>
        private void RegisterViewModelEvents()
        {
            _orderNotesViewModel.PropertyChanged += OrderNotesViewModel_OnPropertyChanged;
        }

        /// <summary>
        /// Called when it is time to unregister the view model events.
        /// </summary>
        private void UnregisterViewModelEvents()
        {
            _orderNotesViewModel.PropertyChanged -= OrderNotesViewModel_OnPropertyChanged;
        }

        #endregion

        #region Events

        private void OrderNotesViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OrderNotesViewModel.Notes))
            {
                LoadNotes();
            }
        }

        private void ButtonAddOrder_Click(object sender, EventArgs e)
        {
            try
            {
                var callback = Activity as IOrderNotesFragmentCallback;
                callback?.OnNoteSelected(new OrderNote
                {
                    OrderId = _orderId,
                    OrderNoteId = -1,
                    Note = string.Empty,
                    NoteType = OrderNote.Internal
                });
            }
            catch (Exception exc)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error clicking 'add note'", exc);
            }
        }

        #endregion
    }
}