using DWOS.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.ViewModels
{
    /// <summary>
    /// View Model for inspection question functionality.
    /// </summary>
    public class InspectionQuestionViewModel: ViewModelBase
    {
        #region Fields
        
        private OrderInspectionAnswerInfo _answer;
        private InspectionQuestionInfo _question;
        bool _isDirty = false;
        InputTypes _inputType;
        private bool _skipped;

        #endregion

        #region Properties

        /// <summary>
        /// Gets answer information for the question identified by
        /// <see cref="InspectionQuestionID"/>.
        /// </summary>
        public OrderInspectionAnswerInfo AnswerInfo
        {
            get 
            {
                return _answer ??
                    (_answer = new OrderInspectionAnswerInfo()
                    {
                        InspectionQuestionID = InspectionQuestionID
                    }); 
            }
        }

        public InspectionQuestionInfo QuestionInfo
        {
            get { return _question; }
            set { _question = value; OnPropertyChanged("Question"); }
        }

        #region Question Properties

        public int InspectionId { get { return _question.InspectionId; } }

        /// <summary>
        /// Gets the inspection question ID for the current question.
        /// </summary>
        public int InspectionQuestionID { get { return _question.InspectionQuestionID; } }

        /// <summary>
        /// Gets the name for the current question.
        /// </summary>
        public string Name { get { return _question.Name; } }

        /// <summary>
        /// Gets the step order for the current question.
        /// </summary>
        public decimal StepOrder { get { return _question.StepOrder; } }

        /// <summary>
        /// Gets the input type for the current question.
        /// </summary>
        public InputTypes InputType { get { return _inputType; } }

        /// <summary>
        /// Gets the minimum value for the current question.
        /// </summary>
        public string MinValue { get { return _question.MinValue; } }

        /// <summary>
        /// Gets the maximum value for the current question.
        /// </summary>
        public string MaxValue { get { return _question.MaxValue; } }

        /// <summary>
        /// Gets the list ID for the current question.
        /// </summary>
        public int ListID { get { return _question.ListID; } }

        /// <summary>
        /// Gets the default value for the current question.
        /// </summary>
        public string DefaultValue { get { return _question.DefaultValue; } }

        /// <summary>
        /// Gets the numeric units for the current question.
        /// </summary>
        public string NumericUnits { get { return _question.NumericUnits; } }

        public List<InspectionQuestionConditionViewModel> Conditions { get; } =
            new List<InspectionQuestionConditionViewModel>();

        #endregion

        #region Answer Properties

        /// <summary>
        /// Gets the response for the current answer.
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
            }
        }

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
            get => _skipped;
            private set
            {
                if (_skipped != value)
                {
                    _skipped = value;
                    OnPropertyChanged(nameof(Skipped));
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Prevents a default instance of the <see cref="InspectionQuestionViewModel"/> class from being created.
        /// </summary>
        private InspectionQuestionViewModel()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionQuestionViewModel"/> class.
        /// </summary>
        /// <param name="questionInfo">The question information.</param>
        /// <exception cref="System.ArgumentNullException">questionInfo</exception>
        public InspectionQuestionViewModel(InspectionQuestionInfo questionInfo)
            :base()
        {
            if (questionInfo == null)
                throw new ArgumentNullException("questionInfo");

            _question = questionInfo;
            _inputType = (InputTypes)Enum.Parse(typeof(InputTypes), questionInfo.InputType);
            Conditions.AddRange(questionInfo.Conditions.Select(InspectionQuestionConditionViewModel.From));

            Validate();
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        protected override void Validate()
        {
            if (_isDirty)
            {
                var isValid = Skipped
                    || InputValidationHelpers.ValidateAnswer(_inputType, MinValue, MaxValue, ListID, Answer);

                var error = InputValidationHelpers.GetInvalidAnswerText(_inputType, MinValue, MaxValue, NumericUnits);
                base.ValidateProperty(!isValid, error);
            }
            base.Validate();
        }
        
        public void UpdateSkipped()
        {
            Skipped = Conditions.Any(c => !c.Passed);
        }

        #endregion 
    }
}
