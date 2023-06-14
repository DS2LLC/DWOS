using DWOS.Services.Messages;
using DWOS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DWOS.ViewModels
{
    /// <summary>
    /// View model for inspection functionality.
    /// </summary>
    public class InspectionViewModel : ViewModelBase
    {
        #region Fields

        InspectionInfo _currentInspection;
        IList<InspectionQuestionViewModel> _questionsAndAnswers;
        int _currentOrderId;
        string _notes;
        bool _hasActiveProcessTimer;
        private bool _hasActiveLaborTimer;
        private bool _isUserActiveOperator;
        const string _initialError = "Please answer questions";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current inspection.
        /// </summary>
        public InspectionInfo Inspection
        {
            get { return _currentInspection; }
            set
            {
                _currentInspection = value;
                OnPropertyChanged("Inspection");
            }
        }

        /// <summary>
        /// Gets or sets the list of questions and answers for the
        /// current inspection.
        /// </summary>
        public IList<InspectionQuestionViewModel> QuestionsAndAnswers
        {
            get { return _questionsAndAnswers; }
            set
            {
                _questionsAndAnswers = value;
                OnPropertyChanged("QuestionsAndAnsers");
            }
        }

        /// <summary>
        /// Gets or sets the current Order ID.
        /// </summary>
        public int OrderId
        {
            get { return _currentOrderId; }
            set
            {
                _currentOrderId = value;
                OnPropertyChanged("OrderId");
            }
        }

        /// <summary>
        /// Gets or sets the notes for the current inspection.
        /// </summary>
        public string Notes
        {
            get { return _notes; }
            set
            {
                if (_notes != value)
                {
                    _notes = value;
                    OnPropertyChanged("Notes");
                }
            }
        }

        public bool HasActiveProcessTimer
        {
            get => _hasActiveProcessTimer;
            set
            {
                if (_hasActiveProcessTimer != value)
                {
                    _hasActiveProcessTimer = value;
                    OnPropertyChanged(nameof(HasActiveProcessTimer));
                }
            }
        }

        public bool HasActiveLaborTimer
        {
            get => _hasActiveLaborTimer;
            set
            {
                _hasActiveLaborTimer = value;
                OnPropertyChanged(nameof(HasActiveLaborTimer));
            }
        }

        public bool IsUserActiveOperator
        {
            get => _isUserActiveOperator;
            set
            {
                if (_isUserActiveOperator != value)
                {
                    _isUserActiveOperator = value;
                    OnPropertyChanged(nameof(IsUserActiveOperator));
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Populates this instance asynchronously with the current inspection for
        /// an order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<ViewModelResult> GetCurrentInspectionForOrderAsync(int orderId)
        {
            IsBusy = true;
            InvalidateViewModel();
            ViewModelResult viewModelResult = null;
            var inspectionService = ServiceContainer.Resolve<IInspectionService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            inspectionService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var request = new InspectionByOrderRequest
            {
                OrderId = orderId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await inspectionService.GetNextInspectionAsync(request);
            if (response.Inspection != null)
            {
                QuestionsAndAnswers = GetQuestionsAndAnswers(response.Inspection);
                OrderId = orderId;
                Inspection = response.Inspection;
                RegisterForIsValidOnQuestions();
                if (QuestionsAndAnswers.Count > 0)
                    Errors.Add(_initialError);
            }

            await RefreshActiveTimerAsync();

            viewModelResult = new ViewModelResult(response.Success, response.ErrorMessage);

            IsBusy = false;
            return viewModelResult;
        }

        private async Task RefreshActiveTimerAsync()
        {
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
            {
                return;
            }

            var timeTrackingService = ServiceContainer.Resolve<ITimeTrackingService>();
            timeTrackingService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var timerInfo = await timeTrackingService.GetInfoAsync(new OrderTimerRequest
            {
                OrderId = _currentOrderId,
                UserId = loginViewModel.UserProfile.UserId
            });

            if (timerInfo.Success)
            {
                HasActiveProcessTimer = timerInfo.HasActiveProcessTimer;
                HasActiveLaborTimer = timerInfo.HasActiveLaborTimer;
                IsUserActiveOperator = timerInfo.IsUserActiveOperator;
            }
            else
            {
                HasActiveProcessTimer = false;
                HasActiveLaborTimer = false;
                IsUserActiveOperator = false;
            }
        }

        /// <summary>
        /// Saves answers ansynchronously.
        /// </summary>
        /// <param name="partQuantity"></param>
        /// <param name="isPass"></param>
        /// <returns></returns>
        public async Task<ViewModelResult> SaveAnswersAsync(int partQuantity, bool isPass)
        {
            IsBusy = true;
            var inspectionService = ServiceContainer.Resolve<IInspectionService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            inspectionService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var orderInspection = CreateOrderInspectionInfo(partQuantity, isPass);
            var request = new InspectionSaveAnswerRequest
            {
                OrderInspection = orderInspection,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await inspectionService.SaveInspectionAnswersAsync(request);
            var viewModelResult = new ViewModelResult(response.Success, response.ErrorMessage);

            IsBusy = false;
            return viewModelResult;
        }

        private OrderInspectionInfo CreateOrderInspectionInfo(int partQuantity, bool isPass)
        {
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            var answers = QuestionsAndAnswers
                .Select(questionViewModel => questionViewModel.AnswerInfo)
                .ToList();

            // Dependent orders have a qty of 0
            var acceptedQty = isPass == true ? Math.Max(partQuantity, 1) : 0;
            var rejectedQty = isPass == false ? Math.Max(partQuantity, 1) : 0;

            var orderInspectionInfo = new OrderInspectionInfo
            {
                InspectionId = Inspection.InspectionId,
                OrderID = OrderId,
                Status = isPass,
                PartQuantity = partQuantity,
                UserID = loginViewModel.UserProfile.UserId,
                InspectionDate = DateTime.Now,
                Notes = Notes,
                AcceptedQty = acceptedQty,
                RejectedQty = rejectedQty,
                InspectionAnswers = answers
            };

            return orderInspectionInfo;
        }

        private IList<InspectionQuestionViewModel> GetQuestionsAndAnswers(InspectionInfo inspection)
        {
            var questionsAndAnswers = inspection.InspectionQuestions
                .Select(questionInfo =>
                    {
                        return new InspectionQuestionViewModel(questionInfo);
                    })
                .ToList();

            return questionsAndAnswers;
        }

        /// <summary>
        /// Invalidates the properties that represent state.
        /// </summary>
        public override void InvalidateViewModel()
        {
            UnregisterForIsValidOnQuestions();
            Inspection = null;
            QuestionsAndAnswers = null;
            OrderId = -1;
            Errors.Clear();
            Notes = null;

            base.InvalidateViewModel();
        }

        /// <summary>
        /// Gets answers for a list.
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public IList<string> GetAnswerList(int listId)
        {
            IList<string> answers = null;

            if (Inspection != null)
            {
                var answerListInfo = Inspection.Lists.FirstOrDefault(listInfo => listInfo.ListId == listId);
                answers = answerListInfo != null ? answerListInfo.Values : null;
            }
            return answers;
        }

        public async Task<ViewModelResult> StartOrderProcessTimer()
        {
            IsBusy = true;

            var processService = ServiceContainer.Resolve<ITimeTrackingService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            processService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var orderTimerRequest = new OrderTimerRequest
            {
                OrderId = _currentOrderId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await processService.StartProcessTimerAsync(orderTimerRequest);

            IsBusy = false;

            if (response.Success)
            {
                HasActiveProcessTimer = true;
            }

            return new ViewModelResult(response.Success, response.ErrorMessage);
        }

        public async Task<ViewModelResult> StopOrderProcessTimer()
        {
            IsBusy = true;

            var processService = ServiceContainer.Resolve<ITimeTrackingService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            processService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var orderTimerRequest = new OrderTimerRequest
            {
                OrderId = _currentOrderId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await processService.StopProcessTimerAsync(orderTimerRequest);

            IsBusy = false;

            if (response.Success)
            {
                HasActiveProcessTimer = false;
            }

            return new ViewModelResult(response.Success, response.ErrorMessage);
        }

        public async Task<ViewModelResult> StartOrderLaborTimer()
        {
            IsBusy = true;

            var processService = ServiceContainer.Resolve<ITimeTrackingService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            processService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var orderTimerRequest = new OrderTimerRequest
            {
                OrderId = _currentOrderId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await processService.StartLaborTimerAsync(orderTimerRequest);

            IsBusy = false;

            if (response.Success)
            {
                HasActiveLaborTimer = true;
                HasActiveProcessTimer = true; // Server starts process timer unless it's already active
                IsUserActiveOperator = true;
            }

            return new ViewModelResult(response.Success, response.ErrorMessage);
        }

        public async Task<ViewModelResult> PauseOrderLaborTimer()
        {
            IsBusy = true;

            var processService = ServiceContainer.Resolve<ITimeTrackingService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            processService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var orderTimerRequest = new OrderTimerRequest
            {
                OrderId = _currentOrderId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await processService.PauseLaborTimerAsync(orderTimerRequest);

            IsBusy = false;

            if (response.Success)
            {
                HasActiveLaborTimer = false;
            }

            return new ViewModelResult(response.Success, response.ErrorMessage);
        }

        public async Task<ViewModelResult> StopOrderLaborTimer()
        {
            IsBusy = true;

            var processService = ServiceContainer.Resolve<ITimeTrackingService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            processService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var orderTimerRequest = new OrderTimerRequest
            {
                OrderId = _currentOrderId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await processService.StopLaborTimerAsync(orderTimerRequest);

            IsBusy = false;

            if (response.Success)
            {
                HasActiveLaborTimer = false;
                IsUserActiveOperator = false;
            }

            return new ViewModelResult(response.Success, response.ErrorMessage);
        }

        private void RegisterForIsValidOnQuestions()
        {
            if (QuestionsAndAnswers != null)
                foreach (var questionAndAnswer in QuestionsAndAnswers)
                    questionAndAnswer.IsValidChanged += QuestionAndAnswer_IsValidChanged;
        }

        private void UnregisterForIsValidOnQuestions()
        {
            if (QuestionsAndAnswers != null)
                foreach (var questionAndAnswer in QuestionsAndAnswers)
                    questionAndAnswer.IsValidChanged -= QuestionAndAnswer_IsValidChanged;
        }

        /// <summary>
        /// Protected method for validating the ViewModel
        /// - Fires PropertyChanged for IsValid and Errors
        /// </summary>
        protected override void Validate()
        {
            if (Errors.Contains(_initialError))
                Errors.Remove(_initialError);

            if (QuestionsAndAnswers != null)
            {
                foreach (var questionAndAnswer in QuestionsAndAnswers)
                {
                    var questionIsValid = questionAndAnswer.IsValid;
                    var errorMessage = string.Format("Question: {0} is invalid.", questionAndAnswer.Name);

                    ValidateProperty(() => !questionIsValid, errorMessage);
                }
            }
            base.Validate();
        }

        public void CheckAllConditions()
        {
            foreach (var questionAnswer in QuestionsAndAnswers)
            {
                foreach (var condition in questionAnswer.Conditions)
                {
                    var checkQuestion = QuestionsAndAnswers
                        .FirstOrDefault(qa => qa.InspectionQuestionID == condition.CheckInspectionQuestionId);

                    condition.Check(checkQuestion);
                }

                if (questionAnswer.Conditions.Count > 0)
                {
                    questionAnswer.UpdateSkipped();
                }
            }
        }

        public void CheckConditions(InspectionQuestionViewModel question)
        {
            foreach (var questionResponse in QuestionsAndAnswers)
            {
                var conditionsToCheck = questionResponse.Conditions
                    .Where(condition => condition.CheckInspectionQuestionId == question.InspectionQuestionID);

                var anyConditionChecked = false;
                foreach (var condition in conditionsToCheck)
                {
                    condition.Check(question);
                    anyConditionChecked = true;
                }

                if (anyConditionChecked)
                {
                    questionResponse.UpdateSkipped();
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the IsValidChanged event of the QuestionAndAnswer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void QuestionAndAnswer_IsValidChanged(object sender, EventArgs e)
        {
            Validate();
        }

        #endregion
    }
}
