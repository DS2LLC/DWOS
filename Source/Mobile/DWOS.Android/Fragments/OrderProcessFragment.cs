using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using DWOS.ViewModels;
using DWOS.Utilities;
using System.Threading.Tasks;
using DWOS.Services.Messages;

namespace DWOS.Android
{
    /// <summary>
    /// Callback interface for Activities that display the <see cref="OrderProcessFragment"/>
    /// </summary>
    public interface IOrderProcessDialogCallback
    {
        /// <summary>
        /// Notifies of the Dialog's dismissal.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="result">if set to <c>true</c> the user selected to save; false if cancel.</param>
        void OrderProcessDialogDismissed(OrderProcessFragment sender, bool result);
    }

    /// <summary>
    /// <see cref="OrderProcessFragment"/> manages the main dialog UI for Order Processing Questions
    /// </summary>
    public class OrderProcessFragment : DialogFragment, IOrderProcessStepListCallback
    {
        public const string PROCESS_FRAGMENT_TAG = "ProcessFragment";
        public const string BUNDLEID_READONLY = "ReadOnly";

        #region Fields
        ProcessViewModel _processViewModel;
        ProgressBar _loadingProgressBar;
        LinearLayout _rootLayout;
        Button _cancelButton;
        Button _saveButton;
        Button _doneButton;
        TextView _titleTextView;
        bool _isReadOnly;
        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the order identifier.
        /// </summary>
        /// <value>
        /// The order identifier.
        /// </value>
        public int OrderID { get; private set; }
        
        /// <summary>
        /// Gets or Sets the Process ID. If this is set then the Fragment will
        /// use this to lookup a process instead of by OrderID (Current Process)
        /// SHOULD USE AN INTENT WHEN MOVING TO ACTIVITY
        /// </summary>
        public OrderProcessInfo OrderProcessInfo { get; private set; }
        
        #endregion
        
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderProcessActivity"/> class.
        /// </summary>
        public OrderProcessFragment()
            : base()
        {
            OrderID = -1;
            _isReadOnly = false;
            _processViewModel = ServiceContainer.Resolve<ProcessViewModel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderProcessFragment"/> class. 
        /// Will lookup the current process for Order ID provided
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        public OrderProcessFragment(int orderId)
            :this()
        {
            OrderID = orderId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderProcessFragment"/> class.
        /// Will lookup the process from the order process info provided
        /// </summary>
        /// <param name="orderProcessInfo">The order process information.</param>
        public OrderProcessFragment(OrderProcessInfo orderProcessInfo)
            : this()
        {
            OrderProcessInfo = orderProcessInfo;
            OrderID = orderProcessInfo.OrderId;
        }

        /// <summary>
        /// Called to do initial creation of a fragment.
        /// </summary>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (savedInstanceState != null)
            _isReadOnly = savedInstanceState.GetBoolean(BUNDLEID_READONLY, defaultValue: false);
        }

        /// <summary>
        /// Called to have the fragment instantiate its user interface view.
        /// </summary>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.OrderProcessLayout, null);
            _loadingProgressBar = view.FindViewById<ProgressBar>(Resource.Id.processProgressBar);
            _rootLayout = view.FindViewById<LinearLayout>(Resource.Id.processLinearLayout);
            _titleTextView = view.FindViewById<TextView>(Resource.Id.textViewProcessTitle);
            _cancelButton = view.FindViewById<Button>(Resource.Id.buttonCancel);
            _saveButton = view.FindViewById<Button>(Resource.Id.buttonSave);
            _doneButton = view.FindViewById<Button>(Resource.Id.buttonDone);
            _doneButton.Click += (sender, e) => Cancel();
            _cancelButton.Click += (sender, e) => Cancel();
            _saveButton.Click += (sender, e) => Save();

            _loadingProgressBar.Visibility = ViewStates.Visible;
            _rootLayout.Visibility = ViewStates.Invisible;

            return view;
        }

        /// <summary>
        /// Called when the fragment is visible to the user and actively running.
        /// </summary>
        /// <since version="Added in API level 11" />
        public override async void OnResume()
        {
            base.OnResume();

            if (OrderProcessInfo != null)
            {
                OrderID = OrderProcessInfo.OrderId;
                _isReadOnly = true;
                await LoadProcessAsync(OrderProcessInfo);
            }
            else if (OrderID > -1)
                await LoadCurrentProcessAsync();
            else if (_processViewModel.CurrentProcess != null)
                OrderID = _processViewModel.CurrentOrderId;
                       
            SetCompletedQuestionsLabel();
            _titleTextView.Text = _processViewModel.CurrentProcess.Name;
            if (_isReadOnly)
            {
                _cancelButton.Visibility = ViewStates.Invisible;
                _saveButton.Visibility = ViewStates.Invisible;
                _doneButton.Visibility = ViewStates.Visible;
            }
            _loadingProgressBar.Visibility = ViewStates.Gone;
            _rootLayout.Visibility = ViewStates.Visible;
            RegisterViewModelEvents();
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
        /// Called when the fragment is no longer in use.
        /// </summary>
        public override void OnDestroy()
        {
            _loadingProgressBar.Dispose();
            _rootLayout.Dispose();
            _cancelButton.Dispose();
            _saveButton.Dispose();
            _titleTextView.Dispose();
            _doneButton.Dispose();

            base.OnDestroy();
        }

        /// <summary>
        /// Called to ask the fragment to save its current dynamic state, so it
        /// can later be reconstructed in a new instance of its process is
        /// restarted.
        /// </summary>
        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutBoolean(BUNDLEID_READONLY, _isReadOnly);
        }

        /// <summary>
        /// Override to build your own custom Dialog container.
        /// </summary>
        /// <param name="savedInstanceState">The last saved instance state of the Fragment,
        /// or null if this is a freshly created Fragment.</param>
        /// <returns>
        /// To be added.
        /// </returns>
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var dialog = base.OnCreateDialog(savedInstanceState);
            dialog.SetCanceledOnTouchOutside(false);
            
            return dialog;
        }

        /// <summary>
        /// Called when the Fragment is visible to the user.
        /// </summary>
        public override void OnStart()
        {
            base.OnStart();

            if (Dialog != null)
            {
                //global::Android.Graphics.Rect windowRect = new global::Android.Graphics.Rect();
                //Activity.Window.DecorView.GetWindowVisibleDisplayFrame(windowRect);
                //double percentage = (double)Activity.Resources.GetInteger(Resource.Integer.percentage);
                //double sizePercentage = percentage / 100;
                //var width = (int)(windowRect.Width() * sizePercentage);
                //var height = (int)(windowRect.Height() * sizePercentage);
                int width = ViewGroup.LayoutParams.MatchParent;
                int height = ViewGroup.LayoutParams.MatchParent;
                Dialog.Window.SetLayout(width, height);
            }
        }

        /// <summary>
        /// Loads the process fragments and data asynchronously.
        /// </summary>
        /// <returns></returns>
        private async Task LoadCurrentProcessAsync()
        {
            var result = await _processViewModel.GetCurrentProcessForOrderAsync(OrderID);
            if (result.Success == true && string.IsNullOrEmpty(result.ErrorMessage))
                AddFragments();
        }

        /// <summary>
        /// Loads the process fragments and data asynchronously.
        /// </summary>
        /// <returns></returns>
        private async Task LoadProcessAsync(OrderProcessInfo orderProcessInfo)
        {
            var result = await _processViewModel.GetProcessForOrderAsync(orderProcessInfo);
            if (result.Success == true && string.IsNullOrEmpty(result.ErrorMessage))
                AddFragments();
        }

        /// <summary>
        /// Adds the main fragments to the Frame Layouts.
        /// </summary>
        private void AddFragments()
        {
            var transaction = ChildFragmentManager.BeginTransaction();
            var stepListFragment = new OrderProcessStepListFragment();
            var isReadOnly = OrderProcessInfo != null;
            var questionHostFragment = new OrderProcessQuestionsHostFragment(_isReadOnly);
            transaction.SetTransition(FragmentTransit.FragmentOpen);
            transaction.Add(Resource.Id.stepsContainer, stepListFragment, OrderProcessStepListFragment.ORDER_PROCESS_STEP_FRAGMENT_TAG);
            transaction.Add(Resource.Id.questionsContainer, questionHostFragment, OrderProcessQuestionsHostFragment.ORDER_PROCESS_QUESTION_FRAGMENT_TAG);
            transaction.Commit();
        }

        private void Save()
        {
            var callback = Activity as IOrderProcessDialogCallback;
            if (callback != null)
                callback.OrderProcessDialogDismissed(this, true);
        }

        private void Cancel()
        {
            var callback = Activity as IOrderProcessDialogCallback;
            if (callback != null)
                callback.OrderProcessDialogDismissed(this, false);
        }

        public void OnStepSelected(int stepId)
        {
            _processViewModel.SetNextStep(stepId);
            var transaction = ChildFragmentManager.BeginTransaction();
            var isReadOnly = OrderProcessInfo != null;
            var questionHostFragment = new OrderProcessQuestionsHostFragment(_isReadOnly);
            transaction.SetTransition(FragmentTransit.FragmentFade);
            transaction.Replace(Resource.Id.questionsContainer, questionHostFragment, OrderProcessQuestionsHostFragment.ORDER_PROCESS_QUESTION_FRAGMENT_TAG);
            transaction.Commit();
        }

        /// <summary>
        /// Registers the view model events.
        /// </summary>
        private void RegisterViewModelEvents()
        {
            _processViewModel.IsValidChanged += OnViewModelIsValidChanged;
        }

        /// <summary>
        /// Unregisters the view model events.
        /// </summary>
        private void UnregisterViewModelEvents()
        {
            _processViewModel.IsValidChanged -= OnViewModelIsValidChanged;
        }

        private void OnViewModelIsValidChanged(object sender, EventArgs e)
        {
            _saveButton.Enabled = _processViewModel.IsValid;
        }

        private void SetCompletedQuestionsLabel()
        {
            var completedTextView = View.FindViewById<TextView>(Resource.Id.textViewCompletedQuestions);
            int count = _processViewModel.QuestionsAndAnswers != null ? _processViewModel.QuestionsAndAnswers.Count : 0;
            int answered = _processViewModel.QuestionsAndAnswers != null ?
                _processViewModel.QuestionsAndAnswers.Where(viewModel => viewModel.Completed == true).Count() :
                0;
            var countLabel = string.Format("{0} / {1}", answered.ToString(), count.ToString());
            completedTextView.Text = countLabel;
        }

        #endregion
    }
}