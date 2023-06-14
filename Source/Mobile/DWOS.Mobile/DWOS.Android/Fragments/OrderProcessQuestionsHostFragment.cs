using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using DWOS.ViewModels;
using DWOS.Utilities;
using DWOS.Android.Adapters;

namespace DWOS.Android
{
    /// <summary>
    /// Shows multiple process questions.
    /// </summary>
    public class OrderProcessQuestionsHostFragment : Fragment, IOrderProcessQuestionCallback, ViewPager.IOnPageChangeListener
    {
        #region Fields

        public const string ORDER_PROCESS_QUESTION_FRAGMENT_TAG = "OrderProcessQuestionFragment";
        const string BUNDLEID_CURRENT_QUESTION_POSITION = "CurrentQuestionPositionKey";
        const string BUNDLEID_READONLY = "IsReadOnly";
        const string BUNDLEID_MODE = "CurrentMode";

        ViewPager _questionViewPager;
        ProcessViewModel _processViewModel;
        BatchProcessViewModel _batchProcessViewModel;
        Spinner _spinnerDocuments;
        int _restoredIndex = -1;
        int _previousIndex = -1;
        bool _isReadOnly;

        #endregion

        #region Properties

        public Mode CurrentMode { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderProcessQuestionsHostFragment"/> class.
        /// </summary>
        /// <remarks>
        /// Android requires this default constructor.
        /// </remarks>
        public OrderProcessQuestionsHostFragment()
            : this(false)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderProcessQuestionsHostFragment"/> class.
        /// </summary>
        /// <param name="isReadOnly">if set to <c>true</c> the questions are all read only.</param>
        public OrderProcessQuestionsHostFragment(bool isReadOnly)
        {
            _isReadOnly = isReadOnly;
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
            
            _processViewModel = ServiceContainer.Resolve<ProcessViewModel>();
            _batchProcessViewModel = ServiceContainer.Resolve<BatchProcessViewModel>();
            var view = inflater.Inflate(Resource.Layout.OrderProcessQuestionHostFragmentLayout, null);
            _questionViewPager = view.FindViewById<ViewPager>(Resource.Id.questionsPager);
            _questionViewPager.AddOnPageChangeListener(this);
            _spinnerDocuments = view.FindViewById<Spinner>(Resource.Id.spinnerDocuments);

            if (savedInstanceState != null)
            {
                _restoredIndex = savedInstanceState.GetInt(BUNDLEID_CURRENT_QUESTION_POSITION, -1);
                _isReadOnly = savedInstanceState.GetBoolean(BUNDLEID_READONLY, defaultValue: false);
                CurrentMode = (Mode)savedInstanceState.GetInt(BUNDLEID_MODE);
            }

            return view;
        }

        /// <summary>
        /// Called when the fragment is visible to the user and actively running.
        /// </summary>
        public override void OnResume()
        {
            base.OnResume();
            LoadQuestions();
            RegisterViewModelEvents();
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

        private void LoadQuestions()
        {
            UnregisterViewEvents();

            IList<ProcessQuestionViewModel> questions = null;
            if (CurrentMode == Mode.Orders && _processViewModel.ProcessStep != null)
            {
                questions = _processViewModel.GetStepQuestionsAndAnswers(_processViewModel.ProcessStep);

                _spinnerDocuments.Adapter = new DocumentListAdapter(Activity,
                    _processViewModel.ProcessStep.Documents,
                    null);
            }
            else if (CurrentMode == Mode.Batches && _batchProcessViewModel.ProcessStep != null)
            {
                questions = _batchProcessViewModel.GetStepQuestionsAndAnswers(_batchProcessViewModel.ProcessStep);
                questions = questions.SkipWhile(processQuestion => processQuestion.InputType == InputTypes.PartQty)
                                     .ToList<ProcessQuestionViewModel>();

                _spinnerDocuments.Adapter = new DocumentListAdapter(Activity,
                    _batchProcessViewModel.ProcessStep.Documents,
                    null);
            }
            else
            {
                _spinnerDocuments.Adapter = null;
            }

            var noQuestionsTextView = View.FindViewById<TextView>(Resource.Id.noQuestionstextView);
            if (questions != null && questions.Count(q => !q.Skipped) > 0)
            {
                if (_questionViewPager.Adapter == null)
                    _questionViewPager.Adapter = new ProcessQuestionFragmentAdapter(questions, ChildFragmentManager, _isReadOnly, CurrentMode);
                if (_restoredIndex > -1)
                    _questionViewPager.SetCurrentItem(_restoredIndex, smoothScroll: false);
                else
                    _questionViewPager.SetCurrentItem(0, smoothScroll: false);
                noQuestionsTextView.Visibility = ViewStates.Gone;

                _previousIndex = Math.Max(_restoredIndex, 0);

                if (!_isReadOnly)
                {
                    questions[_previousIndex].CompleteIfValid();
                }
            }
            else
            {
                _questionViewPager.Visibility = ViewStates.Gone;
                _previousIndex = -1;
            }

            RegisterViewEvents();
        }

        
        public override void OnDestroyView()
        {
            _questionViewPager.Dispose();
            _spinnerDocuments.Dispose();

            base.OnDestroyView();
        }

        /// <summary>
        /// Called to ask the fragment to save its current dynamic state, so it
        /// can later be reconstructed in a new instance of its process is
        /// restarted.
        /// </summary>
        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            //TODO:This is null sometimes. WHY????
            if (_questionViewPager != null)
                outState.PutInt(BUNDLEID_CURRENT_QUESTION_POSITION, _questionViewPager.CurrentItem);

            outState.PutBoolean(BUNDLEID_READONLY, _isReadOnly);
            outState.PutInt(BUNDLEID_MODE, (int)CurrentMode);
        }

        public void NextQuestion()
        {
            if (_questionViewPager != null)
            {
                var adapter = (ProcessQuestionFragmentAdapter)_questionViewPager.Adapter;
                int nextItem = _questionViewPager.CurrentItem + 1;
                if (nextItem < _questionViewPager.Adapter.Count)
                    _questionViewPager.SetCurrentItem(nextItem, smoothScroll: true);
                else
                {
                    var callback = Activity as IOrderProcessQuestionsHostFragmentCallback;
                    if (callback != null)
                        callback.OnNextQuestion();
                }
            }
        }

        /// <summary>
        /// Called when it is time to register to view model events.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        private void RegisterViewModelEvents()
        {
            if (CurrentMode == Mode.Orders)
                _processViewModel.PropertyChanged += ViewModel_PropertyChanged;
            else
                _batchProcessViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void RegisterViewEvents()
        {
            _spinnerDocuments.ItemSelected += _spinnerDocuments_ItemSelected;
        }

        private void UnregisterViewModelEvents()
        {
            if (CurrentMode == Mode.Orders)
                _processViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            else
                _batchProcessViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void UnregisterViewEvents()
        {
            _spinnerDocuments.ItemSelected -= _spinnerDocuments_ItemSelected;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ProcessStep")
                LoadQuestions();
        }

        private void _spinnerDocuments_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var position = e.Position;
            var callback = Activity as IDocumentSelectionCallback;
            var documentListAdapter = _spinnerDocuments.Adapter as DocumentListAdapter;

            if (callback == null || documentListAdapter == null)
            {
                return;
            }

            var item = documentListAdapter[position];

            if (item.Document != null)
            {
                callback.OnDocumentInfoSelected(item.Document);
            }
            else if (item.Media != null)
            {
                callback.OnMediaSummarySelected(item.Media);
            }
        }

        /// <summary>
        /// Gets the initial focus.
        /// </summary>
        /// <returns></returns>
        public View GetInitialFocus()
        {
            var view = _questionViewPager.FindViewById<EditText>(Resource.Id.editTextAnswer);
            if (view != null && view.Visibility == ViewStates.Visible)
                return view;
            else
                return null;
        }

        /// <summary>
        /// Prepares to detach fragment.
        /// </summary>
        public void PrepareToDetachFragment()
        {
            UnregisterViewModelEvents();
        }

        public void OnPageScrollStateChanged(int state)
        {
            //No Implementation
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            //No Implementation
        }

        public void OnPageSelected(int position)
        {
            try
            {
                var adapter = _questionViewPager.Adapter as ProcessQuestionFragmentAdapter;

                if (adapter == null)
                {
                    Log.Debug("*********** OrderProcessQuestionHostFragment", "No Adapter!");
                    return;
                }

                var currentQuestion = adapter.Questions[position];
                switch (currentQuestion.InputType)
                {
                    case InputTypes.List:
                    case InputTypes.Date:
                    case InputTypes.Time:
                    case InputTypes.TimeIn:
                    case InputTypes.TimeOut:
                    case InputTypes.DateTimeIn:
                    case InputTypes.DateTimeOut:
                    case InputTypes.None:
                        Activity.HideSoftKeyboard();
                        break;
                    default:
                        Activity.ShowSoftKeyboard();
                        break;
                }

                // Done in case this question is the last but isn't required.
                if (!_isReadOnly)
                {
                    currentQuestion.CompleteIfValid();
                }

                _previousIndex = position;
            }
            catch (Exception exception)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                var optionItemException = new Exception("Error in OrderProcessQuestionsHostFragment.OnPageSelected", exception);
                logService.LogError("Error in OrderProcessQuestionsHostFragment.OnPageSelected", optionItemException, this);
            }
        }

        #endregion
    }
}