using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DWOS.ViewModels;
using System.Threading.Tasks;
using DWOS.Utilities;
using System.ComponentModel;
using DWOS.Android.Adapters;
using System.Linq;

namespace DWOS.Android
{
    /// <summary>
    /// Shows part information for an order.
    /// </summary>
    public class PartDetailsFragment : Fragment
    {
        #region Fields

        const string BUNDLEID_ORDERID = "OrderId";
        PartViewModel _partViewModel;
        OrderViewModel _orderViewModel;
        int _orderId = -1;
        TextView _partTextView;
        TextView _revisionTextView;
        TextView _manufacturerTextView;
        TextView _materialTextView;
        TextView _modelTextView;
        TextView _sizeTextView;
        LinearLayout _customFieldsView;
        TextView _notesTextView;
        Spinner _spinnerDocuments;
        
        #endregion
        
        #region Methods

        /// <summary>
        /// Called to do initial creation of a fragment.
        /// </summary>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _partViewModel = ServiceContainer.Resolve<PartViewModel>();
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
            var view = inflater.Inflate(Resource.Layout.PartDetailsFragmentLayout, container, attachToRoot:false);

            _partTextView = view.FindViewById<TextView>(Resource.Id.textViewPart);
            _revisionTextView = view.FindViewById<TextView>(Resource.Id.textViewRev);
            _manufacturerTextView = view.FindViewById<TextView>(Resource.Id.textViewManufacturer);
            _materialTextView = view.FindViewById<TextView>(Resource.Id.textViewMaterial);
            _modelTextView = view.FindViewById<TextView>(Resource.Id.textViewModel);
            _sizeTextView = view.FindViewById<TextView>(Resource.Id.textViewSize);
            _customFieldsView = view.FindViewById<LinearLayout>(Resource.Id.layoutCustomFields);
            _notesTextView = view.FindViewById<TextView>(Resource.Id.textViewNotes);
            _spinnerDocuments = view.FindViewById<Spinner>(Resource.Id.spinnerDocuments);

            return view;
        }

        /// <summary>
        /// Called when the fragment is visible to the user and actively running.
        /// </summary>
        public override void OnResume()
        {
            base.OnResume();
            RegisterViewModelEvents();

            if (_orderId < 0 && _partViewModel.OrderId < 0)
                return;

            if ((_orderId == _partViewModel.OrderId) ||
                (_orderId < 0 &&_partViewModel.OrderId > -1))
            {
                _orderId = _partViewModel.OrderId;
                LoadPart();
            }
        }

        /// <summary>
        /// Called when the Fragment is no longer resumed.
        /// </summary>
        public override void OnPause()
        {
            base.OnPause();
            UnregisterViewModelEvents();
            UnregisterViewEvents();
        }

        /// <summary>
        /// Called when the fragment is no longer in use.
        /// </summary>
        public override void OnDestroyView()
        {
            _partTextView.Dispose();
            _revisionTextView.Dispose();
            _manufacturerTextView.Dispose();
            _materialTextView.Dispose();
            _modelTextView.Dispose();
            _sizeTextView.Dispose();
            _customFieldsView.Dispose();
            _notesTextView.Dispose();
            _spinnerDocuments.Dispose();

            base.OnDestroyView();
        }

        private void HandleIntent(Intent intent)
        {
            if (intent != null)
                _orderId = intent.GetIntExtra(OrderDetailActivity.INTENT_ORDERID, defaultValue: -1);
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

        private void LoadPart()
        {
            UnregisterViewEvents();
            _customFieldsView.RemoveAllViews();

            _orderId = -1;
            if (_partViewModel.Part != null)
            {
                _orderId = _partViewModel.OrderId;
                _partTextView.Text = !string.IsNullOrEmpty(_partViewModel.Part.Name) ? _partViewModel.Part.Name
                    : OrderViewModel.NONE_TEXT;
                _revisionTextView.Text = !string.IsNullOrEmpty(_partViewModel.Part.Rev) ? _partViewModel.Part.Rev
                    : OrderViewModel.NONE_TEXT;
                _manufacturerTextView.Text = !string.IsNullOrEmpty(_partViewModel.Part.Manufacturer) ? _partViewModel.Part.Manufacturer
                    : OrderViewModel.NONE_TEXT;
                _materialTextView.Text = !string.IsNullOrEmpty(_partViewModel.Part.Material) ? _partViewModel.Part.Material
                    : OrderViewModel.NONE_TEXT;
                _modelTextView.Text = !string.IsNullOrEmpty(_partViewModel.Part.Model) ? _partViewModel.Part.Model
                    : OrderViewModel.NONE_TEXT;
                _sizeTextView.Text = !string.IsNullOrEmpty(_partViewModel.Part.Dimensions) ? _partViewModel.Part.Dimensions
                    : OrderViewModel.NONE_TEXT;
                _notesTextView.Text = !string.IsNullOrEmpty(_partViewModel.Part.Notes) ? _partViewModel.Part.Notes
                    : OrderViewModel.NONE_TEXT;

                _spinnerDocuments.Adapter = new DocumentListAdapter(Activity,
                    _partViewModel.Part.Documents,
                    _partViewModel.Part.Media);

                foreach (var customField in _partViewModel.Part.CustomFields ?? Enumerable.Empty<Services.Messages.PartCustomField>())
                {
                    using (var view = Activity.LayoutInflater.Inflate(Resource.Layout.CustomFieldLayout, null))
                    {
                        using (var nameView = view.FindViewById<TextView>(Resource.Id.textViewFieldName))
                        {
                            nameView.Text = customField.Name;
                        }
                        using (var valueView = view.FindViewById<TextView>(Resource.Id.textViewFieldValue))
                        {
                            valueView.Text = !string.IsNullOrEmpty(customField.Value)
                                ? customField.Value
                                : OrderViewModel.NONE_TEXT;
                        }

                        _customFieldsView.AddView(view);
                    }
                }
            }

            RegisterViewEvents();
        }

        /// <summary>
        /// Called when it is time to register to view model events.
        /// </summary>
        private void RegisterViewModelEvents()
        {
            _partViewModel.PropertyChanged += PartViewModel_OnPropertyChanged;
        }

        private void RegisterViewEvents()
        {
            _spinnerDocuments.ItemSelected += SpinnerDocuments_ItemSelected;
        }

        /// <summary>
        /// Called when it is time to unregister the view model events.
        /// </summary>
        private void UnregisterViewModelEvents()
        {
            _partViewModel.PropertyChanged -= PartViewModel_OnPropertyChanged;
        }

        private void UnregisterViewEvents()
        {
            _spinnerDocuments.ItemSelected -= SpinnerDocuments_ItemSelected;
        }

        /// <summary>
        /// Handles the OnPropertyChanged event of the PartViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        private void PartViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Part")
                LoadPart();
        }

        private void SpinnerDocuments_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var position = e.Position;
            var callback = Activity as IDocumentSelectionCallback;
            var processDetailsAdapter = _spinnerDocuments.Adapter as DocumentListAdapter;

            if (callback == null || processDetailsAdapter == null)
            {
                return;
            }

            var item = processDetailsAdapter[position];

            if (item.Document != null)
            {
                callback.OnDocumentInfoSelected(item.Document);
            }
            else if (item.Media != null)
            {
                callback.OnMediaSummarySelected(item.Media);
            }
        }

        #endregion
    }
}