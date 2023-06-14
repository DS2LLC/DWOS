using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using DWOS.Android.Controls;
using DWOS.Utilities;
using DWOS.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AR = global::Android.Resource;
using AT = global::Android.Text;

namespace DWOS.Android
{
    /// <summary>
    /// Shows inspection questions for the current inspection.
    /// </summary>
    public class InspectionQuestionFragment : Fragment, EditText.IOnEditorActionListener, IDatePickerFragmentCallback
    {
        #region Fields

        const string ANSWER_TAG = "Answer";
        const string BUNDLEID_MODE = "CurrentMode";

        LogInViewModel _loginViewModel;
        InspectionViewModel _orderInspectionViewModel;
        BatchInspectionViewModel _batchInspectionViewModel;
        View _responseView;
        View _skipQuestionView;
        Button _skipQuestionButton;
        TextView _answerRange;
        TextView _answerHeaderTextView;
        EditText _answerEditText;
        DWOSSpinner _answerSpinner;
        Button _answerButton;
        Button _setDefaultButton;

        #endregion

        #region Properties

        public InspectionQuestionViewModel Question { get; set; }

        public Mode CurrentMode { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Called to do initial creation of a fragment.
        /// </summary>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            _orderInspectionViewModel = ServiceContainer.Resolve<InspectionViewModel>();
            _batchInspectionViewModel = ServiceContainer.Resolve<BatchInspectionViewModel>();

            RegisterViewModelEvents();

            if (savedInstanceState != null)
            {
                CurrentMode = (Mode)savedInstanceState.GetInt(BUNDLEID_MODE);
            }
        }

        /// <summary>
        /// Called to have the fragment instantiate its user interface view.
        /// </summary>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.InspectionQuestionFragmentLayout, null);
            _responseView = view.FindViewById<View>(Resource.Id.layoutQuestionResponse);
            _skipQuestionView = view.FindViewById<View>(Resource.Id.viewSkipQuestion);
            _skipQuestionButton = view.FindViewById<Button>(Resource.Id.buttonSkipQuestion);
            _skipQuestionButton.Click += SkipQuestionButton_Click;
            _answerRange = view.FindViewById<TextView>(Resource.Id.textViewRange);
            _answerEditText = view.FindViewById<EditText>(Resource.Id.editTextAnswer);
            _answerSpinner = view.FindViewById<DWOSSpinner>(Resource.Id.spinnerAnswer);
            _answerButton = view.FindViewById<Button>(Resource.Id.buttonAnswer);
            _answerHeaderTextView = view.FindViewById<TextView>(Resource.Id.textViewAnswerHeader);
            _setDefaultButton = view.FindViewById<Button>(Resource.Id.buttonSetDefault);
            _setDefaultButton.Click += SetToDefaultButton_Click;
            LoadAnswerControls();
            return view;
        }

        /// <summary>
        /// Called when the Fragment is no longer resumed.
        /// </summary>
        public override void OnPause()
        {
            base.OnPause();
            UnregisterViewModelEvents();
        }

        public override void OnResume()
        {
            base.OnResume();
            RefreshInterface();
        }

        /// <summary>
        /// Called when the fragment is no longer in use.
        /// </summary>
        public override void OnDestroyView()
        {
            _responseView.Dispose();
            _skipQuestionView.Dispose();
            _skipQuestionButton.Dispose();
            _answerRange.Dispose();
            _answerEditText.Dispose();
            _answerSpinner.Dispose();
            _answerButton.Dispose();
            _answerHeaderTextView.Dispose();
            _setDefaultButton.Dispose();

            base.OnDestroyView();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt(BUNDLEID_MODE, (int)CurrentMode);
        }

        private void LoadAnswerControls()
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
            _setDefaultButton.Visibility = Question.DefaultValue != null ? ViewStates.Visible : ViewStates.Gone; 
        }

        private void LoadAnswerEditTextControl(AT.InputTypes inputTypes)
        {
            _answerEditText.Text = !string.IsNullOrEmpty(Question.Answer) && Question.Completed ? Question.Answer : string.Empty;
            _answerEditText.Visibility = ViewStates.Visible;
            _answerEditText.InputType = inputTypes;
            _answerEditText.ImeOptions = ImeAction.Next;
            _answerEditText.SetOnEditorActionListener(this);
            _answerEditText.TextChanged += (sender, e) => SetAnswer(_answerEditText.Text);
        }

        private void LoadAnswerButtonControl(AT.InputTypes inputTypes)
        {
            var value = !string.IsNullOrEmpty(Question.Answer) ? Question.Answer : "Select";
            _answerButton.Text = value;
            _answerButton.Visibility = ViewStates.Visible;

            if (inputTypes == AT.InputTypes.DatetimeVariationDate)
            {
                _answerButton.Click += (sender, e) => LaunchDatePicker(ANSWER_TAG);
            }
            else if (inputTypes == AT.InputTypes.DatetimeVariationTime)
            {
                _answerButton.Click += (sender, e) => LaunchTimePicker(ANSWER_TAG);
            }
            else if (inputTypes == AT.InputTypes.DatetimeVariationTime)
            {
                _answerButton.Click += (sender, e) => LaunchDateTimePicker(ANSWER_TAG);
            }
        }

        private void LoadAnswerRangeControl()
        {
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
                    minValue = !String.IsNullOrEmpty(Question.MinValue) ? Question.MinValue : "0.0";
                    maxValue = !String.IsNullOrEmpty(Question.MaxValue) ? Question.MaxValue : "999.0";
                    numericUnits = !String.IsNullOrEmpty(Question.NumericUnits) ? Question.NumericUnits : "Num.";
                    rangeText = String.Format("{0} - {1} {2}", minValue, maxValue, numericUnits);
                    break;
                case InputTypes.Integer:
                case InputTypes.TimeDuration:
                case InputTypes.RampTime:
                    minValue = !String.IsNullOrEmpty(Question.MinValue) ? Question.MinValue : "0";
                    maxValue = !String.IsNullOrEmpty(Question.MaxValue) ? Question.MaxValue : "999";
                    numericUnits = !String.IsNullOrEmpty(Question.NumericUnits) ? Question.NumericUnits : "Num.";
                    rangeText = String.Format("{0} - {1} {2}", minValue, maxValue, numericUnits);
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

        private void LoadAnswerSpinnerControl()
        {
            _answerSpinner.Visibility = ViewStates.Visible;

            IList<string> answerList;
            if (CurrentMode == Mode.Orders)
            {
                answerList = _orderInspectionViewModel.GetAnswerList(Question.ListID);
            }
            else
            {
                answerList = _batchInspectionViewModel.GetAnswerList(Question.ListID);
            }

            if (answerList != null)
            {
                var answerAdapter = new ArrayAdapterWithDefaultItem<string>(Activity, AR.Layout.SimpleSpinnerItem, answerList);
                answerAdapter.SetDropDownViewResource(AR.Layout.SimpleSpinnerDropDownItem);
                answerAdapter.Add("Select");

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

                EventHandler<AdapterView.ItemSelectedEventArgs> spinnerSelectedHandler = (sender, e) =>
                    SetAnswer(_answerSpinner.SelectedItem.ToString(), moveToNextQuestion: true);

                _answerSpinner.ItemSelected += spinnerSelectedHandler;
                _answerSpinner.SameItemSelected += spinnerSelectedHandler;
            }
        }

        private void SetAnswer(string answer, bool moveToNextQuestion = false)
        {
            Question.Answer = answer;

            if (CurrentMode == Mode.Orders)
            {
                _orderInspectionViewModel.CheckConditions(Question);
            }
            else if (CurrentMode == Mode.Batches)
            {
                _batchInspectionViewModel.CheckConditions(Question);
            }

            if (Question.IsValid)
            {
                Question.CompletedBy = _loginViewModel.UserProfile.UserId;
                if (Question.CompletedDate == DateTime.MinValue)
                {
                    Question.CompletedDate = DateTime.Now;
                }

                if (moveToNextQuestion)
                {
                    MoveToNextQuestion();
                }
            }
        }

        private void RefreshInterface()
        {
            var skipQuestion = Question.Skipped;

            if (_skipQuestionView != null)
            {
                _skipQuestionView.Visibility = skipQuestion
                    ? ViewStates.Visible
                    : ViewStates.Gone;
            }

            if (_responseView != null)
            {
                _responseView.Visibility = skipQuestion
                    ? ViewStates.Gone
                    : ViewStates.Visible;
            }

            if (_answerHeaderTextView != null)
            {
                _answerHeaderTextView.Visibility = skipQuestion
                    ? ViewStates.Gone
                    : ViewStates.Visible;
            }
            
            if (_skipQuestionButton != null)
            {
                int lastQuestionId;

                if (CurrentMode == Mode.Orders)
                {
                    lastQuestionId = _orderInspectionViewModel
                        .QuestionsAndAnswers.LastOrDefault()?.InspectionQuestionID ?? -1;
                }
                else if (CurrentMode == Mode.Batches)
                {
                    lastQuestionId = _batchInspectionViewModel
                        .QuestionsAndAnswers.LastOrDefault()?.InspectionQuestionID ?? -1;
                }
                else
                {
                    lastQuestionId = -1;
                }

                _skipQuestionButton.Visibility = Question.InspectionQuestionID == lastQuestionId
                    ? ViewStates.Gone
                    : ViewStates.Visible;
            }
        }

        private void LaunchDatePicker(string tag)
        {
            var transaction = ChildFragmentManager.BeginTransaction();
            var date = Question.Completed ? Question.CompletedDate : DateTime.Now;
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
            var time = new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Year,
                    hourOfDay, minute, 0);
            _answerButton.Text = time.ToString("hh:mm:ss tt");
            SetAnswer(_answerButton.Text, moveToNextQuestion: true);
        }

        public void OnDateCallback(int year, int monthOfYear, int dayOfMonth, string tag)
        {
            var date = new DateTime(year, monthOfYear, dayOfMonth);
            _answerButton.Text = date.ToShortDateString();
            SetAnswer(_answerButton.Text);
        }

        public void OnDateTimeCallback(DateTime dateTime, string tag)
        {
            _answerButton.Text = dateTime.ToString();
            SetAnswer(_answerButton.Text);
        }

        /// <summary>
        /// Registers the view model events.
        /// </summary>
        private void RegisterViewModelEvents()
        {
            if (Question != null)
            {
                Question.IsValidChanged += OnViewModelIsValidChanged;
                Question.PropertyChanged += OnViewModelPropertyChanged;
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
                Question.PropertyChanged -= OnViewModelPropertyChanged;
            }
        }

        private void OnViewModelIsValidChanged(object sender, EventArgs e)
        {
            SetAnswerHeaderStyle();
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(InspectionQuestionViewModel.Skipped))
                {
                    RefreshInterface();
                }
            }
            catch (Exception exc)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error in OnViewModelPropertyChanged", exc);
            }
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
            var parentFragment = Activity as IInspectionQuestionCallback;
            if (parentFragment != null)
            {
                parentFragment.NextQuestion();
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
                        answerList = _orderInspectionViewModel.GetAnswerList(Question.ListID);
                    }
                    else
                    {
                        answerList = _batchInspectionViewModel.GetAnswerList(Question.ListID);
                    }

                    if (answerList != null && answerList.Count > 0)
                    {
                        var index = answerList.IndexOf(Question.DefaultValue);
                        _answerSpinner.SetSelection(index, animate: false);
                    }

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

        private void SkipQuestionButton_Click(object sender, EventArgs e)
        {
            MoveToNextQuestion();
        }

        #endregion
    }
}