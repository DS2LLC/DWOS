using DWOS.Services.Messages;
using DWOS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DWOS.ViewModels
{
    /// <summary>
    /// View Model for batch inspection functionality.
    /// </summary>
    public class BatchInspectionViewModel : ViewModelBase
    {
        #region Fields

        InspectionInfo _currentInspection;
        IList<InspectionQuestionViewModel> _questionsAndAnswers;
        int _currentBatchId;
        string _notes;
        const string _initialError = "Please answer questions";
        IList<int> _unsavedOrderIds;
        private bool _hasActiveProcessTimer;
        private bool _hasActiveLaborTimer;
        private bool _isUserActiveOperator;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the info representing the current inspection.
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
        /// Gets or sets the current list of questions and answers.
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
        /// Gets or sets the current batch ID.
        /// </summary>
        public int BatchId
        {
            get { return _currentBatchId; }
            set
            {
                _currentBatchId = value;
                OnPropertyChanged("OrderId");
            }
        }

        /// <summary>
        /// Gets or sets notes for the current inspection.
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

        /// <summary>
        /// Gets a list of unsaved orders that are part of the batch.
        /// </summary>
        public IList<int> UnsavedOrderIds
        {
            get { return _unsavedOrderIds; }
            private set
            {
                if (_unsavedOrderIds != value)
                {
                    _unsavedOrderIds = value;
                    OnPropertyChanged("UnsavedOrderIds");
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
                if (_hasActiveLaborTimer != value)
                {
                    _hasActiveLaborTimer = value;
                    OnPropertyChanged(nameof(HasActiveLaborTimer));
                }
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
        /// Populates this instance with the current inspection for a batch asynchronously.
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public async Task<ViewModelResult> GetCurrentBatchInspectionForOrderAsync(int batchId)
        {
            IsBusy = true;
            InvalidateViewModel();
            ViewModelResult viewModelResult = null;
            var inspectionService = ServiceContainer.Resolve<IInspectionService>();
            var batchViewModel = ServiceContainer.Resolve<BatchViewModel>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);
            inspectionService.RootUrl = loginViewModel.ServerUrlWellFormed;
            if (batchViewModel.ActiveBatch != null && batchViewModel.ActiveBatch.BatchId != batchId)
            {
                viewModelResult = await batchViewModel.SetActiveBatchDetailAsync(batchId);
                if (!string.IsNullOrEmpty(viewModelResult.ErrorMessage))
                    return viewModelResult;
            }

            if (batchViewModel.ActiveBatch.Orders != null)
            {
                var orderIDs = new Queue<int>(batchViewModel.ActiveBatch.Orders);

                while (orderIDs.Count > 0)
                {
                    var request = new InspectionByOrderRequest
                    {
                        OrderId = orderIDs.Peek(),
                        UserId = loginViewModel.UserProfile.UserId
                    };

                    var response = await inspectionService.GetNextInspectionAsync(request);

                    // Response can be successful and have a null Inspection
                    // if the order has no next inspection.
                    if (response.Inspection != null)
                    {
                        _unsavedOrderIds = new List<int>(orderIDs);

                        QuestionsAndAnswers = GetQuestionsAndAnswers(response.Inspection);
                        BatchId = batchId;
                        Inspection = response.Inspection; // set last; views change based on its update
                        RegisterForIsValidOnQuestions();

                        if (QuestionsAndAnswers.Count > 0)
                        {
                            Errors.Add(_initialError);
                        }

                        viewModelResult = new ViewModelResult(response.Success, response.ErrorMessage);
                        break;
                    }
                    else if (!response.Success)
                    {
                        _unsavedOrderIds = new List<int>(orderIDs);
                        break;
                    }

                    // No inspection found
                    orderIDs.Dequeue();
                }
            }

            // Check for active timers
            await RefreshActiveTimerAsync();

            if (viewModelResult == null)
            {
                // No more inspections - try to move the batch
                var batchInspectionService = ServiceContainer.Resolve<IBatchInspectionService>();
                batchInspectionService.RootUrl = loginViewModel.ServerUrlWellFormed;

                var saveCompletedResult = await batchInspectionService.BatchInspectionSaveCompletedAsync(new BatchInspectionSaveCompletedRequest
                {
                    BatchId = batchId,
                    UserId = loginViewModel.UserProfile.UserId
                });

                if (saveCompletedResult?.Success ?? false)
                {
                    viewModelResult = new ViewModelResult(false, // Failed because it did not load an inspection
                        $"Automatically moved batch {batchId} because it had no more inspections.");
                }
                else
                {
                    viewModelResult = new ViewModelResult(false,
                        $"Error inspecting batch {batchId} - {saveCompletedResult?.ErrorMessage ?? "N/A"}");
                }
            }

            IsBusy = false;
            return viewModelResult;
        }

        /// <summary>
        /// Saves current answers asynchronously.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="partQuantity"></param>
        /// <param name="isPass"></param>
        /// <returns></returns>
        public async Task<ViewModelResult> SaveAnswersAsync(int orderId, int partQuantity, bool isPass)
        {
            IsBusy = true;

            if (UnsavedOrderIds.Contains(orderId))
            {
                var inspectionService = ServiceContainer.Resolve<IInspectionService>();
                var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
                if (!loginViewModel.IsLoggedIn)
                    return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

                inspectionService.RootUrl = loginViewModel.ServerUrlWellFormed;
                var batchInspection = CreateInspectionInfo(orderId, partQuantity, isPass);
                var request = new InspectionSaveAnswerRequest
                {
                    OrderInspection = batchInspection,
                    UserId = loginViewModel.UserProfile.UserId
                };

                var response = await inspectionService.SaveInspectionAnswersAsync(request);
                var viewModelResult = new ViewModelResult(response.Success, response.ErrorMessage);
                if (string.IsNullOrEmpty(viewModelResult.ErrorMessage))
                {
                    UnsavedOrderIds.Remove(orderId);
                    if (UnsavedOrderIds.Count > 0)
                    {
                        UnregisterForIsValidOnQuestions();
                        QuestionsAndAnswers = GetQuestionsAndAnswers(Inspection);
                        RegisterForIsValidOnQuestions();
                        if (QuestionsAndAnswers.Count > 0)
                            Errors.Add(_initialError);
                    }
                }
                IsBusy = false;
                return viewModelResult;
            }
            else
                return new ViewModelResult(success: false,
                    errorMessage: string.Format("Order ID {0} not in Batch ID {1}", orderId, BatchId));
        }

        /// <summary>
        /// Completes a batch inspection asynchronously.
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public async Task<ViewModelResult> SaveCompletedAsync(int batchId)
        {
            IsBusy = true;
            var inspectionService = ServiceContainer.Resolve<IBatchInspectionService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            inspectionService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var request = new BatchInspectionSaveCompletedRequest
            {
                BatchId = batchId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await inspectionService.BatchInspectionSaveCompletedAsync(request);
            IsBusy = false;
            return new ViewModelResult(response.Success, response.ErrorMessage);
        }

        private OrderInspectionInfo CreateInspectionInfo(int orderId, int partQuantity, bool isPass)
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
                OrderID = orderId,
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
            BatchId = -1;
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

        private async Task RefreshActiveTimerAsync()
        {
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
            {
                return;
            }

            var timeTrackingService = ServiceContainer.Resolve<ITimeTrackingService>();
            timeTrackingService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var timerInfo = await timeTrackingService.GetInfoAsync(new BatchTimerRequest
            {
                BatchId = _currentBatchId,
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

        public async Task<ViewModelResult> StartProcessTimer()
        {
            IsBusy = true;

            var timeTrackingService = ServiceContainer.Resolve<ITimeTrackingService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            timeTrackingService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var timerRequest = new BatchTimerRequest
            {
                BatchId = _currentBatchId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await timeTrackingService.StartProcessTimerAsync(timerRequest);

            IsBusy = false;

            if (response.Success)
            {
                HasActiveProcessTimer = true;
            }

            return new ViewModelResult(response.Success, response.ErrorMessage);
        }

        public async Task<ViewModelResult> StopProcessTimer()
        {
            IsBusy = true;

            var timeTrackingService = ServiceContainer.Resolve<ITimeTrackingService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            timeTrackingService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var timerRequest = new BatchTimerRequest
            {
                BatchId = _currentBatchId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await timeTrackingService.StopProcessTimerAsync(timerRequest);

            IsBusy = false;

            if (response.Success)
            {
                HasActiveProcessTimer = false;
            }

            return new ViewModelResult(response.Success, response.ErrorMessage);
        }

        public async Task<ViewModelResult> StartLaborTimer()
        {
            IsBusy = true;

            var timeTrackingService = ServiceContainer.Resolve<ITimeTrackingService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            timeTrackingService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var timerRequest = new BatchTimerRequest
            {
                BatchId = _currentBatchId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await timeTrackingService.StartLaborTimerAsync(timerRequest);

            IsBusy = false;

            if (response.Success)
            {
                HasActiveLaborTimer = true;
                HasActiveProcessTimer = true; // Server starts process timer unless it's already active
                IsUserActiveOperator = true;
            }

            return new ViewModelResult(response.Success, response.ErrorMessage);
        }

        public async Task<ViewModelResult> PauseLaborTimer()
        {
            IsBusy = true;

            var timeTrackingService = ServiceContainer.Resolve<ITimeTrackingService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            timeTrackingService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var timerRequest = new BatchTimerRequest
            {
                BatchId = _currentBatchId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await timeTrackingService.PauseLaborTimerAsync(timerRequest);

            IsBusy = false;

            if (response.Success)
            {
                HasActiveLaborTimer = false;
            }

            return new ViewModelResult(response.Success, response.ErrorMessage);
        }

        public async Task<ViewModelResult> StopLaborTimer()
        {
            IsBusy = true;

            var timeTrackingService = ServiceContainer.Resolve<ITimeTrackingService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            timeTrackingService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var timerRequest = new BatchTimerRequest
            {
                BatchId = _currentBatchId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await timeTrackingService.StopLaborTimerAsync(timerRequest);

            IsBusy = false;

            if (response.Success)
            {
                HasActiveLaborTimer = false;
                IsUserActiveOperator = false;
            }

            return new ViewModelResult(response.Success, response.ErrorMessage);
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
