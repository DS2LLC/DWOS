using DWOS.Services.Messages;
using System;

namespace DWOS.ViewModels
{
    /// <summary>
    /// View Model for process question functionality.
    /// </summary>
    public class ProcessQuestionViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Event for when IsValid changes
        /// </summary>
        public event EventHandler IsDirty;

        ProcessQuestionInfo _questionInfo;
        OrderProcessAnswerInfo _answerInfo;
        InputTypes _inputType;
        bool _isDirty = false;
        bool _skipped = false;

        #endregion

        #region Properties

        #region Question Info Properties

        /// <summary>
        /// Gets the process question ID for the current question.
        /// </summary>
        public int ProcessQuestionId { get { return _questionInfo.ProcessQuestionId; } }

        /// <summary>
        /// Gets the process step ID for the current question.
        /// </summary>
        public int ProcessStepId { get { return _questionInfo.ProcessStepId; } }

        /// <summary>
        /// Gets the name for the current question.
        /// </summary>
        public string Name { get { return _questionInfo.Name; } }

        /// <summary>
        /// Gets the step order for the current question.
        /// </summary>
        public decimal StepOrder { get { return _questionInfo.StepOrder; } }

        /// <summary>
        /// Gets the input type for the current question.
        /// </summary>
        public InputTypes InputType { get { return _inputType; } }

        /// <summary>
        /// Gets the minimum value for the current question.
        /// </summary>
        public string MinValue { get { return _questionInfo.MinValue; } }

        /// <summary>
        /// Gets the maximum value for the current question.
        /// </summary>
        public string MaxValue { get { return _questionInfo.MaxValue; } }

        /// <summary>
        /// Gets the list ID for the current question
        /// </summary>
        public int ListID { get { return _questionInfo.ListID; } }

        /// <summary>
        /// Gets the default value for the current question
        /// </summary>
        public string DefaultValue { get { return _questionInfo.DefaultValue; } }

        /// <summary>
        /// Gets a value indicating if the current question is
        /// operator editable.
        /// </summary>
        /// <value>
        /// <c>true</c> if the question is editable; otherwise, <c>false</c>.
        /// </value>
        public bool OperatorEditable { get { return _questionInfo.OperatorEditable; } }

        /// <summary>
        /// Gets a value indicating if the current question is required.
        /// </summary>
        /// <value>
        /// <c>true</c> if the question is required; otherwise, <c>false</c>.
        /// </value>
        public bool Required { get { return _questionInfo.Required; } }

        /// <summary>
        /// Gets or sets the notes for the current question.
        /// </summary>
        public string Notes { get { return _questionInfo.Notes; } }

        /// <summary>
        /// Gets or sets the numeric units for the current question.
        /// </summary>
        public string NumericUnits { get { return _questionInfo.NumericUnits; } }

        #endregion

        #region Answer Properties

        /// <summary>
        /// Gets or sets the <see cref="OrderProcessAnswerInfo"/> instance for
        /// the current answer.
        /// </summary>
        public OrderProcessAnswerInfo AnswerInfo 
        { 
            get 
            { 
                return _answerInfo ?? (_answerInfo = new OrderProcessAnswerInfo()); 
            } 
        }

        /// <summary>
        /// Gets or sets the response for the current answer.
        /// </summary>
        public string Answer
        {
            get { return AnswerInfo.Answer; }
            set
            {
                AnswerInfo.Answer = value;
                _isDirty = true;
                Validate();
                Completed = IsValid;
                OnPropertyChanged("Answer");
                FireIsDirty();
            }
        }

        /// <summary>
        /// Gets or sets the calculated response for the current answer.
        /// </summary>
        public string CalculatedAnswer { get; set; }

        /// <summary>
        /// Gets a value indicating if the current answer is complete.
        /// </summary>
        /// <value>
        /// <c>true</c> if the answer has been completed; otherwise, false
        /// </value>
        public bool Completed
        {
            get { return AnswerInfo.Completed; }
            private set
            {
                AnswerInfo.Completed = value;
                _isDirty = true;
                OnPropertyChanged("Completed");
            }
        }

        /// <summary>
        /// Gets the ID of the user who provided the current answer.
        /// </summary>
        /// <value>
        /// A valid user ID if the answer has been completed;
        /// otherwise, <c>-1</c>.
        /// </value>
        public int CompletedBy
        {
            get { return AnswerInfo.CompletedBy; }
            set
            {
                AnswerInfo.CompletedBy = value;
                _isDirty = true;
                Validate();
                OnPropertyChanged("CompletedBy");
            }
        }

        /// <summary>
        /// Gets the date of the current answer.
        /// </summary>
        /// <value>
        /// The completion date of the answer if it has been completed;
        /// otherwise, <see cref="DateTime.MinValue"/>.
        /// </value>
        public DateTime CompletedDate
        {
            get { return AnswerInfo.CompletedDate; }
            set
            {
                AnswerInfo.CompletedDate = value;
                _isDirty = true;
                Validate();
                OnPropertyChanged("CompletedDate");
            }
        }

        public bool Skipped
        {
            get { return _skipped; }
            set
            {
                _skipped = value;
                OnPropertyChanged(nameof(Skipped));
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Prevents a default instance of the <see cref="ProcessQuestionViewModel"/> class from being created.
        /// </summary>
        private ProcessQuestionViewModel()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessQuestionViewModel"/> class.
        /// </summary>
        /// <param name="questionInfo">The question information.</param>
        /// <param name="answerInfo">The answer information.</param>
        /// <exception cref="System.ArgumentNullException">
        /// questionInfo
        /// </exception>
        public ProcessQuestionViewModel(ProcessQuestionInfo questionInfo, OrderProcessAnswerInfo answerInfo)
            :base()
        {
            if (questionInfo == null)
                throw new ArgumentNullException("questionInfo");

            _questionInfo = questionInfo;
            _answerInfo = answerInfo;
            _inputType = (InputTypes)Enum.Parse(typeof(InputTypes), questionInfo.InputType);

            Validate();
        }

        /// <summary>
        /// Completes the question if the answer is valid.
        /// </summary>
        /// <remarks>
        /// This method is intended for use with non-required questions
        /// that would otherwise be incomplete when left blank.
        /// </remarks>
        public void CompleteIfValid()
        {
            if (IsDataValid() && DoesAnswerMatchCalculation())
            {
                Completed = true;

                if (CompletedDate == DateTime.MinValue)
                {
                    CompletedDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        ///  Skips this instance's question, marking it as completed.
        /// </summary>
        public void Skip()
        {
            Skipped = true;
            Answer = null;
            CompletedDate = DateTime.MinValue;
            Completed = true;
            CompletedBy = 0;
        }

        /// <summary>
        /// Resets this instance's question.
        /// </summary>
        /// <remarks>
        /// If this instance was previously skipped, this method marks it as an
        /// instance not to skip.
        /// </remarks>
        public void Reset()
        {
            Answer = null;
            CompletedDate = DateTime.MinValue;
            Completed = false;

            Skipped = false;
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        protected override void Validate()
        {
            if (_isDirty)
            {
                if (Skipped)
                {
                    Errors.Clear();
                }
                else
                {
                    ValidateProperty(() => !DoesAnswerMatchCalculation(),
                        $"Must be equal to calculated value.");

                    base.ValidateProperty(() => !IsDataValid(),
                        InputValidationHelpers.GetInvalidAnswerText(_inputType, MinValue, MaxValue, NumericUnits));
                }
            }

            base.Validate();
        }

        private void FireIsDirty()
        {
            if (_isDirty && IsDirty != null)
                IsDirty(this, new EventArgs());
        }

        private bool IsDataValid()
        {
            return InputValidationHelpers.ValidateAnswer(_inputType, MinValue, MaxValue, ListID, Answer) ||
                (!_questionInfo.Required && InputValidationHelpers.IsAnswerBlank(_inputType, Answer));
        }

        private bool DoesAnswerMatchCalculation()
        {
            return string.IsNullOrEmpty(CalculatedAnswer) || Answer == CalculatedAnswer;
        }

        #endregion
    }
}
