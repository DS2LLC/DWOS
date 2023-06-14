using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DWOS.ViewModels;
using DWOS.Utilities;
using System.Threading.Tasks;
using Android.Graphics;
using System.ComponentModel;
using DWOS.Android.Adapters;
using System.Collections.Generic;

namespace DWOS.Android
{
    /// <summary>
    /// Shows order details for the current order.
    /// </summary>
    public class OrderDetailsFragment : Fragment
    {
        #region Fields

        public const string BUNDLE_EXTRA_ORDER_ID = "OrderId";

        OrderViewModel _orderViewModel;
        int _orderId = -1;
        LinearLayout _layoutDetails;
        TextView _textViewCompany;
        TextView _textViewDepartment;
        TextView _textViewMiscNotes;
        TextView _textViewWorkStatus;
        TextView _textViewWorkOrder;
        TextView _textViewPart;
        TextView _textViewType;
        TextView _textViewQuantity;
        TextView _textViewPriority;
        TextView _textViewSchedulePriorityLabel;
        TextView _textViewSchedulePriority;
        TextView _textViewOrderDate;
        TextView _textViewEstShipDate;
        TextView _textViewRequiredDate;
        TextView _textViewCustomerWO;
        TextView _textViewPurchaseOrder;
        TextView _textViewShipping;
        ImageView _imageViewOrder;
        Spinner _spinnerDocuments;

        #endregion

        #region Properties

        public int RequestedImageSize
        {
            get { return _imageViewOrder != null && _imageViewOrder.Width > 0 ? _imageViewOrder.Width : 512; }
        }

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
        /// <returns>
        /// To be added.
        /// </returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            HandleIntent(Activity.Intent);
            _orderId = savedInstanceState != null ? savedInstanceState.GetInt(BUNDLE_EXTRA_ORDER_ID, -1) : _orderId;

            _orderViewModel = ServiceContainer.Resolve<OrderViewModel>();
            var view = inflater.Inflate(Resource.Layout.OrderDetailsFragmentLayout, null);

            _layoutDetails = view.FindViewById<LinearLayout>(Resource.Id.detailsLayout);
            _textViewCompany = view.FindViewById<TextView>(Resource.Id.textViewCompany);
            _textViewWorkOrder = view.FindViewById<TextView>(Resource.Id.textViewWorkOrder);
            _textViewDepartment = view.FindViewById<TextView>(Resource.Id.textViewDepartment);
            _textViewMiscNotes = view.FindViewById<TextView>(Resource.Id.textViewMiscNotes);
            _textViewWorkStatus = view.FindViewById<TextView>(Resource.Id.textViewWorkStatus);
            _textViewPart = view.FindViewById<TextView>(Resource.Id.textViewPart);
            _textViewType = view.FindViewById<TextView>(Resource.Id.textViewType);
            _textViewQuantity = view.FindViewById<TextView>(Resource.Id.textViewQuantity);
            _textViewPriority = view.FindViewById<TextView>(Resource.Id.textViewPriority);
            _textViewSchedulePriorityLabel = view.FindViewById<TextView>(Resource.Id.textViewSchedulePriorityLabel);
            _textViewSchedulePriority = view.FindViewById<TextView>(Resource.Id.textViewSchedulePriority);
            _textViewOrderDate = view.FindViewById<TextView>(Resource.Id.textViewOrderDate);
            _textViewEstShipDate = view.FindViewById<TextView>(Resource.Id.textViewEstDate);
            _textViewRequiredDate = view.FindViewById<TextView>(Resource.Id.textViewReqDate);
            _textViewCustomerWO = view.FindViewById<TextView>(Resource.Id.textViewCustomerWO);
            _textViewPurchaseOrder = view.FindViewById<TextView>(Resource.Id.textViewPO);
            _textViewShipping = view.FindViewById<TextView>(Resource.Id.textViewShippingMethod);
            _imageViewOrder = view.FindViewById<ImageView>(Resource.Id.imageViewOrderDetails);
            _spinnerDocuments = view.FindViewById<Spinner>(Resource.Id.spinnerDocuments);

            return view;
        }

        private void HandleIntent(Intent intent)
        {
            if (intent != null)
                _orderId = intent.GetIntExtra(OrderDetailActivity.INTENT_ORDERID, defaultValue: -1);
        }

        /// <summary>
        /// Called when the fragment is visible to the user and actively running.
        /// </summary>
        public override void OnResume()
        {
            base.OnResume();
            RegisterViewModelEvents();

            if (_orderViewModel.ActiveOrder == null)
            {
                if (_orderId < 0)
                {
                    _layoutDetails.Visibility = ViewStates.Gone;
                }
                else
                {
                    _layoutDetails.Visibility = ViewStates.Visible;
                }
            }
            else if (_orderViewModel.ActiveOrder.OrderId > -1)
            {
                _orderId = _orderViewModel.ActiveOrder.OrderId;
                LoadOrderDetails();
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

        public override void OnDetach()
        {
            base.OnDetach();
        }

        /// <summary>
        /// Called to ask the fragment to save its current dynamic state, so it
        /// can later be reconstructed in a new instance of its process is
        /// restarted.
        /// </summary>
        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            if (_orderViewModel.ActiveOrder != null && _orderViewModel.ActiveOrder.OrderId == _orderId)
                outState.PutInt(BUNDLE_EXTRA_ORDER_ID, _orderId);
        }

        /// <summary>
        /// Loads the order details controls with the View Model's Active Order.
        /// </summary>
        private void LoadOrderDetails()
        {
            UnregisterViewEvents();

            _layoutDetails.Visibility = ViewStates.Visible;
            _textViewWorkOrder.Text = _orderViewModel.ActiveOrder.OrderId.ToString();
            _textViewCompany.Text = _orderViewModel.ActiveOrder.CustomerName;
            _textViewCustomerWO.Text =_orderViewModel.ActiveOrder.CustomerWO;

            _textViewDepartment.Text = string.IsNullOrEmpty(_orderViewModel.ActiveOrder.CurrentLine)
                ? _orderViewModel.ActiveOrder.Location
                : $"{_orderViewModel.ActiveOrder.Location} - {_orderViewModel.ActiveOrder.CurrentLine}";

            var miscNotes = new List<string>();

            if (_orderViewModel.ActiveOrder.OrderNoteCount > 0)
            {
                miscNotes.Add("Has Notes");
            }

            if (_orderViewModel.ActiveOrder.IsInBatch)
            {
                miscNotes.Add("In Batch");
            }

            _textViewMiscNotes.Text = string.Join(" | ", miscNotes);

            _textViewWorkStatus.Text =_orderViewModel.ActiveOrder.WorkStatus;
            _textViewPart.Text = _orderViewModel.ActiveOrder.PartName;
            _textViewType.Text = _orderViewModel.ActiveOrder.OrderType;
            _textViewQuantity.Text = _orderViewModel.ActiveOrder.Quantity.ToString();
            _textViewPriority.Text = _orderViewModel.ActiveOrder.Priority;
            _textViewOrderDate.Text = 
                _orderViewModel.ActiveOrder.OrderDate == DateTime.MinValue ? string.Empty : _orderViewModel.ActiveOrder.OrderDate.ToShortDateString();
            _textViewEstShipDate.Text = 
                _orderViewModel.ActiveOrder.EstShipDate == DateTime.MinValue ? string.Empty : _orderViewModel.ActiveOrder.EstShipDate.ToShortDateString();
            _textViewRequiredDate.Text = 
                _orderViewModel.ActiveOrder.ReqDate == DateTime.MinValue ? string.Empty : _orderViewModel.ActiveOrder.ReqDate.ToShortDateString();
            _textViewCustomerWO.Text = _orderViewModel.ActiveOrder.CustomerWO;
            _textViewPurchaseOrder.Text = _orderViewModel.ActiveOrder.PO;
            _textViewShipping.Text = _orderViewModel.ActiveOrder.ShippingMethod ?? "None";

            if (ApplicationSettings.Settings.UsingManualScheduling)
            {
                _textViewSchedulePriorityLabel.Visibility = ViewStates.Visible;
                _textViewSchedulePriority.Visibility = ViewStates.Visible;
                _textViewSchedulePriority.Text = _orderViewModel.ActiveOrder.SchedulePriority > 0
                    ? _orderViewModel.ActiveOrder.SchedulePriority.ToString()
                    : "Non-Scheduled";
            }
            else
            {
                _textViewSchedulePriorityLabel.Visibility = ViewStates.Gone;
                _textViewSchedulePriority.Visibility = ViewStates.Gone;
                _textViewSchedulePriority.Text = string.Empty;
            }

            if (_orderViewModel.ActiveOrder.PartImage != null &&
                _orderViewModel.ActiveOrder.PartImage.Length > 0)
            {
                using (var image = BitmapFactory.DecodeByteArray(
                    _orderViewModel.ActiveOrder.PartImage, 0, _orderViewModel.ActiveOrder.PartImage.Length))
                {
                    _imageViewOrder.SetImageBitmap(image);
                }
                _imageViewOrder.Visibility = ViewStates.Visible;
            }
            else
                _imageViewOrder.Visibility = ViewStates.Invisible;

            _spinnerDocuments.Adapter = new DocumentListAdapter(Activity,
                _orderViewModel.ActiveOrder.Documents,
                _orderViewModel.ActiveOrder.Media);

            RegisterViewEvents();
        }

        public override void OnDestroyView()
        {
            _layoutDetails.Dispose();
            _textViewCompany.Dispose();
            _textViewWorkOrder.Dispose();
            _textViewDepartment.Dispose();
            _textViewMiscNotes.Dispose();
            _textViewWorkStatus.Dispose();
            _textViewPart.Dispose();
            _textViewType.Dispose();
            _textViewQuantity.Dispose();
            _textViewPriority.Dispose();
            _textViewSchedulePriorityLabel.Dispose();
            _textViewSchedulePriority.Dispose();
            _textViewOrderDate.Dispose();
            _textViewEstShipDate.Dispose();
            _textViewRequiredDate.Dispose();
            _textViewCustomerWO.Dispose();
            _textViewPurchaseOrder.Dispose();
            _textViewShipping.Dispose();
            _imageViewOrder.SetImageBitmap(null);
            _imageViewOrder.Dispose();
            _spinnerDocuments.Dispose();

            base.OnDestroyView();
        }

        /// <summary>
        /// Called when it is time to register to view model events.
        /// </summary>
        private void RegisterViewModelEvents()
        {
            _orderViewModel.PropertyChanged += OrderViewModel_OnPropertyChanged;
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
            _orderViewModel.PropertyChanged -= OrderViewModel_OnPropertyChanged;
        }

        private void UnregisterViewEvents()
        {
            _spinnerDocuments.ItemSelected -= SpinnerDocuments_ItemSelected;
        }

        /// <summary>
        /// Handles the OnPropertyChanged event of the OrderViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OrderViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveOrder" && _orderViewModel.ActiveOrder != null)
                LoadOrderDetails();
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

