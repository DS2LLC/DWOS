using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Webkit;
using Android.Widget;
using DWOS.Android.Controls;
using DWOS.Utilities;
using DWOS.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Threading.Tasks;
using AR = global::Android.Resource;
using AT = global::Android.Text;

namespace DWOS.Android
{
    /// <summary>
    /// Shows a process question and answer.
    /// </summary>
    public class OrderProcessQuestionFragment : Fragment, EditText.IOnEditorActionListener, IDatePickerFragmentCallback
    {
        #region Fields

        const string ANSWER_TAG = "Answer";
        const string DATETIME_TAG = "DateTime";
        const string BUNDLEID_QUESTION = "QuestionId";
        const string BUNDLEID_ISREADONLY = "IsReadOnly";
        const string BUNDLEID_POSITION = "Position";
        const string BUNDLEID_MODE = "CurrentMode";

        TextView _titleTextView;
        TextView _answerRange;
        EditText _answerEditText;
        DWOSSpinner _answerSpinner;
        Button _answerButton;
        TextView _answerTextView;
        TextView _requiredTextView;
        TextView _operatorTextView;
        Button _dateButton;
        Button _timeButton;
        Button _setDefaultButton;
        TextView _dateTimeTextView;
        WebView _notesWebView;
        TextView _answerHeaderTextView;
        LogInViewModel _loginViewModel;
        ProcessViewModel _processViewModel;
        BatchProcessViewModel _batchProcessViewModel;
        EventHandler<AT.TextChangedEventArgs> _answerTextChangedHandler;
        EventHandler<AdapterView.ItemSelectedEventArgs> _answerSpinnerChangedHandler;
        ProcessQuestionViewModel _question;

        #endregion

        #region Properties

        public ProcessQuestionViewModel Question
        {
            get { return _question; }
            set
            {
                if (_question == value)
                {
                    return;
                }

                if (_question != null)
                {
                    // handler
                    _question.PropertyChanged -= QuestionOnPropertyChanged;
                }

                _question = value;

                if (value != null)
                {
                    // handler
                    value.PropertyChanged += QuestionOnPropertyChanged;
                }
            }
        }

        public bool IsReadOnly { get; set; }

        public int Position { get; set; }

        public Mode CurrentMode { get; set; }

        #endregion

        #region Methods
        public OrderProcessQuestionFragment()
        {
            Position = -1;
        }

        /// <summary>
        /// Called to do initial creation of a fragment.
        /// </summary>
        /// <param name="savedInstanceState">If the fragment is being re-created from
        /// a previous saved state, this is the state.</param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            _processViewModel = ServiceContainer.Resolve<ProcessViewModel>();
            _batchProcessViewModel = ServiceContainer.Resolve<BatchProcessViewModel>();
            if (savedInstanceState != null)
            {
                int questionId = savedInstanceState.GetInt(BUNDLEID_QUESTION, -1);
                if (questionId > -1)
                {
                    CurrentMode = (Mode)savedInstanceState.GetInt(BUNDLEID_MODE);
                    if (CurrentMode == Mode.Orders && _processViewModel.ProcessStep != null)
                    {
                        var result = _processViewModel.GetStepQuestionAndAnswer(questionId);
                        Question = result.Result;
                    }
                    else if (CurrentMode == Mode.Batches && _batchProcessViewModel.ProcessStep != null)
                    {
                        var result = _batchProcessViewModel.GetStepQuestionAndAnswer(questionId);
                        Question = result.Result;
                    }
                }
                IsReadOnly = savedInstanceState.GetBoolean(BUNDLEID_ISREADONLY, defaultValue: false);
                Position = savedInstanceState.GetInt(BUNDLEID_POSITION, Position);
            }

            RegisterViewModelEvents();
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
            var view = inflater.Inflate(Resource.Layout.OrderProcessQuestionFragmentLayout, null);
            _answerEditText = view.FindViewById<EditText>(Resource.Id.editTextAnswer);
            _answerSpinner = view.FindViewById<DWOSSpinner>(Resource.Id.spinnerAnswer);
            _answerTextView = view.FindViewById<TextView>(Resource.Id.textViewAnswer);
            _answerButton = view.FindViewById<Button>(Resource.Id.buttonAnswer);
            _requiredTextView = view.FindViewById<TextView>(Resource.Id.textViewAnswerRequired);
            _operatorTextView = view.FindViewById<TextView>(Resource.Id.textViewOperator);
            _dateButton = view.FindViewById<Button>(Resource.Id.buttonDate);
            _timeButton = view.FindViewById<Button>(Resource.Id.buttonTime);
            _dateTimeTextView = view.FindViewById<TextView>(Resource.Id.textViewDate);
            _notesWebView = view.FindViewById<WebView>(Resource.Id.webViewNotes);
            _titleTextView = view.FindViewById<TextView>(Resource.Id.textViewQuestionTitle);
            _answerRange = view.FindViewById<TextView>(Resource.Id.textViewRange);
            _answerHeaderTextView = view.FindViewById<TextView>(Resource.Id.textViewAnswerHeader);
            _setDefaultButton = view.FindViewById<Button>(Resource.Id.buttonSetDefault);
            _setDefaultButton.Click += SetToDefaultButton_Click;

            return view;
        }

        /// <summary>
        /// Called when the fragment is visible to the user and actively running.
        /// </summary>
        public async override void OnResume()
        {
            base.OnResume();

            try
            {
                _answerEditText.KeyPress += AnswerEditTextOnKeyPress;

                if (Question != null)
                {
                    _titleTextView.Text = string.Format("{0} {1}", Question.StepOrder.ToString(), Question.Name);

                    string content;
                    if (string.IsNullOrEmpty(Question.Notes))
                    {
                        content = SecurityElement.Escape(OrderViewModel.NONE_TEXT);
                    }
                    else
                    {
                        content = Question.Notes;
                    }
                    _notesWebView.LoadData(content, "text/html", null);
                    _notesWebView.SetBackgroundColor(Color.Transparent);
                    LoadAnswerControls();
                    LoadDateTimeControls();
                    await LoadOperatorControls();
                    SetAnswerHeaderStyle();
                }
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error loading Question {0}", Question?.Name);
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError(errorMsg, ex);
            }
        }

        /// <summary>
        /// Called when the Fragment is no longer resumed.
        /// </summary>
        public override void OnPause()
        {
            base.OnPause();
            UnregisterEventHandlers();
            UnregisterViewModelEvents();
        }

        public override void OnDestroyView()
        {
            if (_question != null)
            {
                // Handler can attempt to update disposed text view
                _question.PropertyChanged -= QuestionOnPropertyChanged;
            }

            _titleTextView.Dispose();
            _answerRange.Dispose();
            _answerEditText.Dispose();
            _answerSpinner.Dispose();
            _answerButton.Dispose();
            _answerTextView.Dispose();
            _requiredTextView.Dispose();
            _operatorTextView.Dispose();
            _dateButton.Dispose();
            _timeButton.Dispose();
            _setDefaultButton.Dispose();
            _dateTimeTextView.Dispose();
            _notesWebView.Dispose();
            _answerHeaderTextView.Dispose();

            base.OnDestroyView();
        }

        private async Task LoadOperatorControls()
        {
            if (Question.Completed)
            {
                var userResult = await _loginViewModel.GetUserAsync(Question.CompletedBy);
                _operatorTextView.Text = userResult.User != null ? userResult.User.Name : string.Empty;
            }
            else
                _operatorTextView.Text = OrderViewModel.NONE_TEXT;
        }

        private void LoadReadOnlyDateTimeControls()
        {
            _dateButton.Visibility = ViewStates.Visible;
            _timeButton.Visibility = ViewStates.Visible;

            _dateButton.Text = Question.Completed ?
                    Question.CompletedDate.ToShortDateString() :
                    string.Empty;

            _timeButton.Text = Question.Completed ?
                    Question.CompletedDate.ToShortTimeString() :
                    string.Empty;

            _dateButton.Enabled = false;
            _timeButton.Enabled = false;
        }

        private void LoadDateTimeControls()
        {
            if (Question.OperatorEditable)
            {
                _dateButton.Visibility = ViewStates.Visible;
                _timeButton.Visibility = ViewStates.Visible;
                _dateTimeTextView.Visibility = ViewStates.Gone;

                _dateButton.Text = Question.Completed ?
                    Question.CompletedDate.ToShortDateString() :
                    DateTime.Now.ToShortDateString();

                _timeButton.Text = Question.Completed ?
                    Question.CompletedDate.ToShortTimeString() :
                    DateTime.Now.ToShortTimeString();

                _dateButton.Click += (sender, e) => LaunchDatePicker(DATETIME_TAG);
                _timeButton.Click += (sender, e) => LaunchTimePicker(DATETIME_TAG);
            }
            else
            {
                _dateButton.Visibility = ViewStates.Gone;
                _timeButton.Visibility = ViewStates.Gone;
                _dateTimeTextView.Visibility = ViewStates.Visible;
                var formatString = "M/d/yyyy h:mm tt";
                _dateTimeTextView.Text = Question.Completed ?
                    Question.CompletedDate.ToString(formatString) :
                    DateTime.Now.ToString(formatString);
            }
        }

        private void LoadAnswerControls()
        {
            if (IsReadOnly)
            {
                LoadReadOnlyAnswerEditTextControl();
                LoadReadOnlyDateTimeControls();
                LoadAnswerRangeControl();
                _setDefaultButton.Visibility = ViewStates.Invisible;
            }
            else
            {
                switch (Question.InputType)
                {
                    case InputTypes.Decimal:
                    case InputTypes.DecimalBefore:
                    case InputTypes.DecimalAfter:
                    case InputTypes.DecimalDifference:
                    case InputTypes.PreProcessWeight:
                    case InputTypes.PostProcessWeight:
                        LoadAnswerEditTextControl(AT.InputTypes.NumberFlagDecimal | AT.InputTypes.ClassNumber);
                        break;
                    case InputTypes.Integer:
                    case InputTypes.PartQty:
                    case InputTypes.TimeDuration:
                    case InputTypes.RampTime:
                        LoadAnswerEditTextControl(AT.InputTypes.ClassNumber);
                        break;
                    case InputTypes.List:
                        LoadAnswerSpinnerControl();
                        break;
                    case InputTypes.Date:
                        LoadAnswerButtonControl(AT.InputTypes.DatetimeVariationDate);
                        break;
                    case InputTypes.Time:
                    case InputTypes.TimeIn:
                    case InputTypes.TimeOut:
                        LoadAnswerButtonControl(AT.InputTypes.DatetimeVariationTime);
                        break;
                    case InputTypes.DateTimeIn:
                    case InputTypes.DateTimeOut:
                        LoadAnswerButtonControl(AT.InputTypes.DatetimeVariationNormal);
                        break;
                    case InputTypes.String:
                    case InputTypes.None:
                    default:
                        LoadAnswerEditTextControl(AT.InputTypes.ClassText);
                        break;
                }

                LoadAnswerRangeControl();

                _requiredTextView.Visibility = Question.Required ? ViewStates.Visible : ViewStates.Gone;
                _setDefaultButton.Visibility = Question.OperatorEditable && Question.DefaultValue != null
                    ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        private void ReloadAnswer()
        {
            var hasCorrectInputType = Question.InputType == InputTypes.DecimalBefore ||
                                      Question.InputType == InputTypes.DecimalAfter ||
                                      Question.InputType == InputTypes.DecimalDifference;

            if (!hasCorrectInputType)
            {
                return;
            }

            _answerEditText.Text = !string.IsNullOrEmpty(Question.Answer) && Question.Completed
                ? Question.Answer
                : string.Empty;
        }

        private void LoadAnswerSpinnerControl()
        {
            _answerSpinner.Visibility = ViewStates.Visible;
            _answerSpinner.Focusable = true;
            _answerSpinner.FocusableInTouchMode = true;

            IList<string> answerList;
            if (CurrentMode == Mode.Orders)
            {
                answerList = _processViewModel.GetAnswerList(Question.ListID);
            }
            else
            {
                answerList = _batchProcessViewModel.GetAnswerList(Question.ListID);
            }

            if (answerList != null)
            {
                var answerAdapter = new ArrayAdapterWithDefaultItem<string>(Activity, AR.Layout.SimpleSpinnerItem, answerList);
                answerAdapter.SetDropDownViewResource(AR.Layout.SimpleSpinnerDropDownItem);
                answerAdapter.Add("Select");

                if (Question.OperatorEditable)
                {
                    _answerSpinner.Adapter = answerAdapter;

                    int index;
                    if (!string.IsNullOrEmpty(Question.Answer) && Question.Completed)
                    {
                        index = answerList.IndexOf(Question.Answer);
                    }
                    else
                    {
                        index = answerAdapter.Count - 1;
                    }

                    _answerSpinner.SetSelection(index, animate: false);
                    _answerSpinnerChangedHandler = (sender, e) => SetAnswer(_answerSpinner.SelectedItem.ToString(), moveToNextQuestion: true);
                    _answerSpinner.ItemSelected += _answerSpinnerChangedHandler;
                    _answerSpinner.SameItemSelected += _answerSpinnerChangedHandler;
                }
                else
                {
                    _answerSpinner.Enabled = false;
                    _answerSpinner.Clickable = false;
                }

                var isFragmentselected = IsFragmentSelected();
                var spinnerHasFocus = _answerSpinner.RequestFocus();
                if (isFragmentselected && spinnerHasFocus)
                {
                    Activity.HideSoftKeyboard(_answerEditText);
                }
            }
            else
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error in LoadAnswerSpinnerControl for OrderProcessQuestionFragment; No Answer List Found for Question.ListID");
            }
        }

        private void SetAnswer(string answer, bool moveToNextQuestion = false)
        {
            Question.PropertyChanged -= QuestionOnPropertyChanged;

            try
            {
                Question.Answer = answer;

                if (!Question.IsValid)
                {
                    return;
                }

                Question.CompletedBy = _loginViewModel.UserProfile.UserId;
                if (Question.CompletedDate == DateTime.MinValue)
                {
                    var date = DateTime.Parse(_dateButton.Text);
                    var time = DateTime.Parse(_timeButton.Text);
                    var dateTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                    Question.CompletedDate = dateTime;
                }

                if (moveToNextQuestion)
                {
                    MoveToNextQuestion();
                }
            }
            finally
            {
                Question.PropertyChanged += QuestionOnPropertyChanged;
            }
        }

        private void LoadAnswerButtonControl(AT.InputTypes inputTypes)
        {
            var value = !string.IsNullOrEmpty(Question.Answer) ? Question.Answer : "Select";
            _answerButton.Text = value;
            _answerButton.Visibility = ViewStates.Visible;
            _answerButton.Focusable = true;
            _answerButton.FocusableInTouchMode = true;

            if (Question.OperatorEditable)
            {
                _answerButton.Enabled = true;

                if (inputTypes == AT.InputTypes.DatetimeVariationDate)
                {
                    _answerButton.Click += (sender, e) => LaunchDatePicker(ANSWER_TAG);
                }
                else if (inputTypes == AT.InputTypes.DatetimeVariationTime)
                {
                    _answerButton.Click += (sender, e) => LaunchTimePicker(ANSWER_TAG);
                }
                else if (inputTypes == AT.InputTypes.DatetimeVariationNormal)
                {
                    _answerButton.Click += (sender, e) => LaunchDateTimePicker(ANSWER_TAG);
                }
            }
            else
            {
                _answerButton.Enabled = false;
            }

            var isFragmentSelected = IsFragmentSelected();
            var buttonHasFocus = _answerButton.RequestFocus();

            if (isFragmentSelected && buttonHasFocus)
            {
                Activity.HideSoftKeyboard(_answerButton);
            }
        }

        private void LoadReadOnlyAnswerEditTextControl()
        {
            _answerEditText.Text = !string.IsNullOrEmpty(Question.Answer) && Question.Completed ? Question.Answer : string.Empty;
            _answerEditText.Visibility = ViewStates.Visible;
            _answerEditText.InputType = AT.InputTypes.Null;
            _answerEditText.Enabled = false;
            _answerEditText.Hint = string.Empty;
        }

        private void LoadAnswerEditTextControl(AT.InputTypes inputTypes)
        {
            _answerEditText.Text = !string.IsNullOrEmpty(Question.Answer) && Question.Completed ? Question.Answer : string.Empty;
            _answerEditText.Visibility = ViewStates.Visible;
            _answerEditText.InputType = inputTypes;

            _answerEditText.ImeOptions = ImeAction.Next;
            _answerEditText.SetOnEditorActionListener(this);
            _answerTextChangedHandler = (sender, e) => SetAnswer(_answerEditText.Text);
            _answerEditText.TextChanged += _answerTextChangedHandler;

            // Filter disallows new input for read-only question
            _answerEditText.SetFilters(Question.OperatorEditable
                ? new AT.IInputFilter[0]
                : new AT.IInputFilter[] { new AT.InputFilterLengthFilter(0) });

            if (IsFragmentSelected() && _answerEditText.RequestFocus()) 
            {
                Activity.ShowSoftKeyboard(_answerEditText);
            }
        }

        private bool IsFragmentSelected()
        {
            var isSelected = false;

            var pager = Activity.FindViewById<global::Android.Support.V4.View.ViewPager>(Resource.Id.questionsPager);
            if (pager != null && pager.Adapter != null)
            {
                isSelected = Position == pager.CurrentItem;
            }

            return isSelected;
        }

        private void LoadAnswerRangeControl()
        {
            if (!Question.OperatorEditable)
            {
                _answerRange.Text = "Read Only";
                return;
            }

            string rangeText = string.Empty;
            string minValue = string.Empty;
            string maxValue = string.Empty;
            string numericUnits = string.Empty;

            switch (Question.InputType)
            {
                case InputTypes.PartQty:
                    rangeText = "Part Quantity";
                    break;
                case InputTypes.Decimal:
                case InputTypes.DecimalBefore:
                case InputTypes.DecimalAfter:
                case InputTypes.DecimalDifference:
                case InputTypes.PreProcessWeight:
                case InputTypes.PostProcessWeight:
                    minValue = !string.IsNullOrEmpty(Question.MinValue) ? Question.MinValue : "0.0";
                    maxValue = !string.IsNullOrEmpty(Question.MaxValue) ? Question.MaxValue : "999.0";
                    numericUnits = !string.IsNullOrEmpty(Question.NumericUnits) ? Question.NumericUnits : "Num.";
                    rangeText = string.Format("{0} - {1} {2}", minValue, maxValue, numericUnits);
                    break;
                case InputTypes.Integer:
                case InputTypes.TimeDuration:
                case InputTypes.RampTime:
                    minValue = !string.IsNullOrEmpty(Question.MinValue) ? Question.MinValue : "0";
                    maxValue = !string.IsNullOrEmpty(Question.MaxValue) ? Question.MaxValue : "999";
                    numericUnits = !string.IsNullOrEmpty(Question.NumericUnits) ? Question.NumericUnits : "Num.";
                    rangeText = string.Format("{0} - {1} {2}", minValue, maxValue, numericUnits);
                    break;
                case InputTypes.List:
                    rangeText = "List";
                    break;
                case InputTypes.Date:
                    rangeText = "Date";
                    break;
                case InputTypes.Time:
                    rangeText = "Time";
                    break;
                case InputTypes.TimeIn:
                case InputTypes.DateTimeIn:
                    rangeText = "Time In";
                    break;
                case InputTypes.TimeOut:
                case InputTypes.DateTimeOut:
                    rangeText = "Time Out";
                    break;
                case InputTypes.None:
                case InputTypes.String:
                default:
                    rangeText = "Text";
                    break;
            }

            _answerRange.Text = rangeText;
        }

        private void LaunchDatePicker(string tag)
        {
            var transaction = ChildFragmentManager.BeginTransaction();
            var date = Question.Completed ?
                Question.CompletedDate :
                DateTime.Now;

            var datePicker = DatePickerFragment.New(DatePickerFragment.DateOrTime.Date, tag, date);

            datePicker.Show(transaction, DatePickerFragment.DATETIMEPICKER_FRAGMENT_TAG);
        }

        private void LaunchTimePicker(string tag)
        {
            var transaction = ChildFragmentManager.BeginTransaction();
            var date = Question.Completed ? Question.CompletedDate : DateTime.Now;
            var datePicker = DatePickerFragment.New(DatePickerFragment.DateOrTime.Time, tag, date);
            datePicker.Show(transaction, DatePickerFragment.DATETIMEPICKER_FRAGMENT_TAG);
        }

        private void LaunchDateTimePicker(string tag)
        {
            var transaction = ChildFragmentManager.BeginTransaction();
            var date = Question.Completed ? Question.CompletedDate : DateTime.Now;
            var datePicker = DatePickerFragment.New(DatePickerFragment.DateOrTime.DateAndTime, tag, date);
            datePicker.Show(transaction, DatePickerFragment.DATETIMEPICKER_FRAGMENT_TAG);
        }

        public void OnTimeCallback(int hourOfDay, int minute, string tag)
        {
            if (tag == DATETIME_TAG)
            {
                var date = DateTime.Parse(_dateButton.Text);
                var completed = new DateTime(date.Year, date.Month, date.Day, hourOfDay, minute, 0);
                Question.CompletedDate = completed;
                _timeButton.Text = Question.CompletedDate.ToShortTimeString();
            }
            else if (tag == ANSWER_TAG)
            {
                var time = new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Year,
                    hourOfDay, minute, 0);
                _answerButton.Text = time.ToString("hh:mm:ss tt");
                SetAnswer(_answerButton.Text, moveToNextQuestion: true);
            }
        }

        public void OnDateCallback(int year, int monthOfYear, int dayOfMonth, string tag)
        {
            if (tag == DATETIME_TAG)
            {
                var time = DateTime.Parse(_timeButton.Text);
                var completed = new DateTime(year, monthOfYear, dayOfMonth, time.Hour, time.Minute, time.Second);
                Question.CompletedDate = completed;
                _dateButton.Text = Question.CompletedDate.ToShortDateString();
            }
            else if (tag == ANSWER_TAG)
            {
                var date = new DateTime(year, monthOfYear, dayOfMonth);
                _answerButton.Text = date.ToShortDateString();
                SetAnswer(_answerButton.Text, moveToNextQuestion: true);
            }
        }
        public void OnDateTimeCallback(DateTime dateTime, string tag)
        {
            if (tag == DATETIME_TAG)
            {
                var time = DateTime.Parse(_timeButton.Text);
                var completed = dateTime;
                Question.CompletedDate = completed;
                _dateButton.Text = Question.CompletedDate.ToShortDateString();
            }
            else if (tag == ANSWER_TAG)
            {
                _answerButton.Text = dateTime.ToString();
                SetAnswer(_answerButton.Text, moveToNextQuestion: true);
            }
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
            outState.PutInt(BUNDLEID_MODE, (int)CurrentMode);

            if (Question != null)
            {
                outState.PutInt(BUNDLEID_QUESTION, Question.ProcessQuestionId);
            }

            outState.PutBoolean(BUNDLEID_ISREADONLY, IsReadOnly);
            outState.PutInt(BUNDLEID_POSITION, Position);
        }

        public bool OnEditorAction(TextView view, ImeAction actionId, KeyEvent keyEvent)
        {
            if (actionId == ImeAction.Next)
            {
                MoveToNextQuestion();
                return true;
            }

            return false;
        }

        private void MoveToNextQuestion()
        {
            var parentFragment = ParentFragment as IOrderProcessQuestionCallback;
            if (parentFragment != null)
            {
                parentFragment.NextQuestion();
            }
        }

        private void UnregisterEventHandlers()
        {
            if (_answerEditText != null)
            {
                if (_answerTextChangedHandler != null)
                {
                    _answerEditText.TextChanged -= _answerTextChangedHandler;
                }

                _answerEditText.KeyPress -= AnswerEditTextOnKeyPress;
            }

            if (_answerSpinner != null && _answerSpinnerChangedHandler != null)
            {
                _answerSpinner.ItemSelected -= _answerSpinnerChangedHandler;
                _answerSpinner.SameItemSelected -= _answerSpinnerChangedHandler;
            }
        }

        /// <summary>
        /// Registers the view model events.
        /// </summary>
        private void RegisterViewModelEvents()
        {
            if (Question != null)
            {
                Question.IsValidChanged += OnViewModelIsValidChanged;
            }
        }

        /// <summary>
        /// Unregisters the view model events.
        /// </summary>
        private void UnregisterViewModelEvents()
        {
            if (Question != null)
            {
                Question.IsValidChanged -= OnViewModelIsValidChanged;
            }
        }

        private void OnViewModelIsValidChanged(object sender, EventArgs e)
        {
            SetAnswerHeaderStyle();
        }

        private void SetAnswerHeaderStyle()
        {
            if (Question != null)
            {
                _answerHeaderTextView.Text = Question.IsValid ? "Answer" : string.Format("Answer - {0}", Question.Error);
                var value = new TypedValue();
                Activity.Theme.ResolveAttribute(global::Android.Resource.Attribute.TextColorSecondary, value, resolveRefs: true);
                var colorInt = Question.IsValid ? value.Data : ContextCompat.GetColor(Activity, Resource.Color.headerred);
                _answerHeaderTextView.SetTextColor(new Color(colorInt));
            }
        }

        private void SetToDefaultButton_Click(object sender, EventArgs e)
        {
            switch (Question.InputType)
            {
                case InputTypes.Decimal:
                case InputTypes.DecimalBefore:
                case InputTypes.DecimalAfter:
                case InputTypes.DecimalDifference:
                case InputTypes.PreProcessWeight:
                case InputTypes.PostProcessWeight:
                case InputTypes.Integer:
                case InputTypes.PartQty:
                case InputTypes.TimeDuration:
                case InputTypes.RampTime:
                case InputTypes.String:
                case InputTypes.None:
                    _answerEditText.Text = Question.DefaultValue;
                    break;
                case InputTypes.List:
                    IList<string> answerList = null;
                    if (CurrentMode == Mode.Orders)
                    {
                        answerList = _processViewModel.GetAnswerList(Question.ListID);
                    }
                    else
                    {
                        answerList = _batchProcessViewModel.GetAnswerList(Question.ListID);
                    }

                    var index = answerList.IndexOf(Question.DefaultValue);
                    _answerSpinner.SetSelection(index, animate: false);
                    break;
                case InputTypes.Date:
                case InputTypes.Time:
                case InputTypes.TimeIn:
                case InputTypes.TimeOut:
                case InputTypes.DateTimeIn:
                case InputTypes.DateTimeOut:
                    _answerButton.Text = Question.DefaultValue;
                    Question.Answer = Question.DefaultValue;
                    break;
                default:
                    break;
            }
        }

        private void QuestionOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != nameof(ProcessQuestionViewModel.Answer))
            {
                return;
            }

            ReloadAnswer();
        }

        private void AnswerEditTextOnKeyPress(object o, View.KeyEventArgs keyEventArgs)
        {
            keyEventArgs.Handled = Question != null &&
                                   !Question.OperatorEditable &&
                                   keyEventArgs.KeyCode == Keycode.Del;
        }

        #endregion
    }
}