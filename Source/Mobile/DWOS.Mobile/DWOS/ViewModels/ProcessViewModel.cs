using DWOS.Services.Messages;
using DWOS.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DWOS.ViewModels
{
    /// <summary>
    /// View Model for order process functionality
    /// </summary>
    public class ProcessViewModel : ViewModelBase
    {
        #region Fields

        IList<ProcessQuestionViewModel> _questionsAndAnswers;
        readonly IDictionary<int, bool> _skipStepDictionary = new Dictionary<int, bool>();
        ProcessStepInfo _processStep;
        OrderProcessDetailInfo _process;
        ProcessAliasInfo _processAlias;
        IList<OrderProcessInfo> _orderProcesses;
        OrderStatusInfo _orderStatus;
        int _currentOrderId;
        int? _currentOrderProcessId;
        private bool _hasActiveProcessTimer;
        private bool _hasActiveLaborTimer;
        private bool _isUserActiveOperator;
        private bool _isDirty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Order Process questions and answers related to the Current Process.
        /// </summary>
        /// <value>
        /// The questions and answers.
        /// </value>
        public IList<ProcessQuestionViewModel> QuestionsAndAnswers
        {
            get { return _questionsAndAnswers; }
            private set 
            {
                if (Equals(_questionsAndAnswers, value))
                {
                    return;
                }

                foreach (var existingViewModel in _questionsAndAnswers ?? Enumerable.Empty<ProcessQuestionViewModel>())
                {
                    existingViewModel.PropertyChanged -= QuestionOnPropertyChanged;
                }

                foreach (var newViewModel in value ?? Enumerable.Empty<ProcessQuestionViewModel>())
                {
                    newViewModel.PropertyChanged += QuestionOnPropertyChanged;
                }

                _skipStepDictionary.Clear();
                _questionsAndAnswers = value;
                OnPropertyChanged("QuestionsAndAnswers");
            }
        }

        /// <summary>
        /// Gets the current process.
        /// </summary>
        /// <value>
        /// The current process.
        /// </value>
        public OrderProcessDetailInfo Process
        {
            get { return _process; }
            private set
            {
                _process = value;
                OnPropertyChanged("Process");
            }
        }

        /// <summary>
        /// Gets or sets the current process alias.
        /// </summary>
        /// <value>
        /// The current process alias.
        /// </value>
        public ProcessAliasInfo ProcessAlias
        {
            get
            {
                return _processAlias;
            }
            set
            {
                _processAlias = value;
                OnPropertyChanged("ProcessAlias");
            }
        }

        /// <summary>
        /// Gets the current order process.
        /// </summary>
        /// <value>
        /// The current process.
        /// </value>
        public IList<OrderProcessInfo> OrderProcesses
        {
            get { return _orderProcesses; }
            private set
            {
                _orderProcesses = value;
                OnPropertyChanged("OrderProcesses");
            }
        }

        /// <summary>
        /// Gets the order status.
        /// </summary>
        /// <value>
        /// The order status.
        /// </value>
        public OrderStatusInfo OrderStatus
        {
            get { return _orderStatus; }
            private set
            {
                _orderStatus = value;
                OnPropertyChanged("OrderStatus");
            }
        }

        /// <summary>
        /// Gets the Current Step in the Current Process.
        /// </summary>
        /// <value>
        /// The current step.
        /// </value>
        public ProcessStepInfo ProcessStep 
        {
            get { return _processStep; }
            private set
            {
                _processStep = value;
                OnPropertyChanged("ProcessStep");
            }
        }

        /// <summary>
        /// Gets the current order ID.
        /// </summary>
        public int OrderId
        {
            get { return _currentOrderId; }
            private set 
            { 
                _currentOrderId = value;
                OnPropertyChanged("OrderId"); 
            }
        }

        /// <summary>
        /// Gets the current order process ID.
        /// </summary>
        public int? OrderProcessId
        {
            get { return _currentOrderProcessId; }
            private set
            {
                _currentOrderProcessId = value;
                OnPropertyChanged(nameof(OrderProcessId));
            }
        }

        public bool HasActiveProcessTimer
        {
            get => _hasActiveProcessTimer;
            set
            {
                _hasActiveProcessTimer = value;
                OnPropertyChanged(nameof(HasActiveProcessTimer));
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

        /// <summary>
        /// Gets or sets a flag indicating if this instance is dirty.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirty
        {
            get { return _isDirty; }
            set 
            { 
                _isDirty = value; 
                OnPropertyChanged("IsDirty"); 
            }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ProcessViewModel"/> class.
        /// </summary>
        public ProcessViewModel()
        {
            QuestionsAndAnswers = new List<ProcessQuestionViewModel>();
        }

        /// <summary>
        /// Populates this instance with all the available processes for an order asynchronously.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns><see cref="GetProcessesResult"/> representing the request.</returns>
        public async Task<ViewModelResult> GetOrderProcessesAsync(int orderId)
        {
            IsBusy = true;
            InvalidateViewModel();
            
            var processService = ServiceContainer.Resolve<IOrderProcessService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();

            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success:false, errorMessage:NotLoggedInMessage);

            processService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var processRequest = new OrderProcessesRequest
            {
                OrderId = orderId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var result = await processService.GetOrderProcessesAsync(processRequest);
            if (result.Success == true && string.IsNullOrEmpty(result.ErrorMessage))
            {
                OrderId = orderId;
                OrderProcessId = null;
                OrderStatus = result.OrderStatus;
                OrderProcesses = result.OrderProcesses;
                // Since the current process is unknown, do not retrieve timer info
            }
            IsBusy = false;

            return new ViewModelResult(result.Success, result.ErrorMessage);
        }

        /// <summary>
        /// Populates this instance with the current proccess for
        /// order asynchronously.
        /// </summary>
        /// <remarks>
        /// Sets <see cref="Process"/>, <see cref="ProcessStep"/>,
        /// <see cref="OrderId"/>, <see cref="QuestionsAndAnswers"/>
        /// if successful.
        /// </remarks>
        /// <param name="orderId">The order identifier.</param>
        /// <returns><see cref="GetProcessResult"/> representing the request. </returns>
        public async Task<ViewModelResult> GetCurrentProcessForOrderAsync(int orderId)
        {
            IsBusy = true;
            InvalidateViewModel();

            OrderId = orderId;
            ViewModelResult viewModelResult = null;
            var response = await GetActiveOrderProcessAsync(orderId);
            if (response.Success && string.IsNullOrEmpty(response.ErrorMessage))
            {
                Process = response.Process;
                ProcessAlias = response.ProcessAlias;
                OrderProcessId = response.OrderProcessId;
                var result = await GetQuestionsAndAnswersAsync(orderId, response.OrderProcessId, Process);
                QuestionsAndAnswers = result.QuestionsAndAnswers;
                RegisterForIsValidOnQuestions();
                var currentStep = GetFirstIncompleteStep(Process.ProcessSteps);
                if (currentStep != null)
                    SetNextStep(currentStep.ProcessStepId);
                else if (Process.ProcessSteps.Count > 0)
                    SetNextStep(Process.ProcessSteps[0].ProcessStepId);

                await RefreshActiveTimerAsync();

                viewModelResult = new ViewModelResult(true, string.Empty);
            }
            else
            {
                var message = string.Format("Unable to find current process for Order: {0}", orderId);
                viewModelResult = new ViewModelResult(false, message);
                Errors.Add("Could not load process.");
                Validate();
            }
            IsBusy = false;

            return viewModelResult;
        }

        /// <summary>
        /// Populates this instance with a specific order process.
        /// </summary>
        /// <remarks>
        /// Sets <see cref="Process"/>, <see cref="OrderId"/>,
        /// <see cref="QuestionsAndAnswers"/> if successful.
        /// </remarks>
        /// <param name="orderProcess">The order process.</param>
        /// <returns></returns>
        public async Task<ViewModelResult> GetProcessForOrderAsync(OrderProcessInfo orderProcess)
        {
            IsBusy = true;
            ViewModelResult viewModelResult = null;
            InvalidateViewModel();

            var processService = ServiceContainer.Resolve<IOrderProcessService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            processService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var response = await processService.GetOrderProcessDetailAsync(new OrderProcessDetailRequest
            {
                OrderProcessId = orderProcess.OrderProcessId,
                UserId = loginViewModel.UserProfile.UserId
            });

            if (response.OrderProcessInfo != null)
            {
                Process = response.OrderProcessInfo;
                ProcessAlias = null;
                OrderId = orderProcess.OrderId;
                OrderProcessId = orderProcess.OrderProcessId;
                var result = await GetQuestionsAndAnswersAsync(OrderId, orderProcess.OrderProcessId, Process);
                QuestionsAndAnswers = result.QuestionsAndAnswers;
                var currentStep = GetFirstIncompleteStep(Process.ProcessSteps);
                if (currentStep != null)
                    SetNextStep(currentStep.ProcessStepId);
                else if (Process.ProcessSteps.Count > 0)
                    SetNextStep(Process.ProcessSteps[0].ProcessStepId);

                await RefreshActiveTimerAsync();

                viewModelResult = new ViewModelResult(response.Success, response.ErrorMessage);
            }
            else
            {
                var message = string.Format("Unable to find Process: {0}", orderProcess.OrderProcessId);
                viewModelResult = new ViewModelResult(false, message);
            }
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
        /// Invalidates the properties that represent state.
        /// </summary>
        public override void InvalidateViewModel()
        {
            UnregisterForIsValidOnQuestions();
            Process = null;
            ProcessAlias = null;
            ProcessStep = null;
            OrderStatus = null;
            OrderProcesses = null;
            QuestionsAndAnswers = null;
            OrderId = -1;
            HasActiveProcessTimer = false;
            HasActiveLaborTimer = false;
            IsUserActiveOperator = false;
            Errors.Clear();
            IsDirty = false;

            base.InvalidateViewModel();
        }

        /// <summary>
        /// Gets the currently saved process questions & answers for an order asynchronously.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="processId">The process identifier.</param>
        /// <returns></returns>
        public async Task<GetProcessQuestionsAndAnswersResult> GetQuestionsAndAnswersAsync(
            int orderId, int orderProcessId, OrderProcessDetailInfo processInfo)
        {
            if (processInfo == null)
                throw new ArgumentNullException("processList");

            IsBusy = true;
            var processService = ServiceContainer.Resolve<IOrderProcessService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new GetProcessQuestionsAndAnswersResult(questionsAndAnswers:null, success: false, errorMessage: NotLoggedInMessage);

            processService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var request = new OrderProcessAnswerRequest
            {
                OrderId = orderId,
                OrderProcessId = orderProcessId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var result = await processService.GetProcessAnswersAsync(request);
            var questionsAndAnswers = result.OrderProcessAnswers
                .Select(answerInfo => 
                    {
                        return new ProcessQuestionViewModel(GetQuestionForAnswer(processInfo, answerInfo), answerInfo);
                    })
                .ToList();
            IsBusy = false;

            return new GetProcessQuestionsAndAnswersResult(questionsAndAnswers, result.Success, result.ErrorMessage);
        }

        /// <summary>
        /// Sets the <see cref="ProcessStep"/> property in the Current Process.
        /// </summary>
        /// <param name="stepId">The step identifier.</param>
        /// <returns><see cref="SetNextStepResult"/> representing the result of this method. Sets 
        /// <see cref="ProcessStep"/> if successful.</returns>
        /// <exception cref="System.InvalidOperationException">CurrentProcess not set</exception>
        public SetNextStepResult SetNextStep(int stepId)
        {
            if (Process == null)
                throw new InvalidOperationException("Process not set");

            IList<ProcessQuestionViewModel> questionsAndAnswers = null;
            var step = Process.ProcessSteps
                .Where(processStep => processStep.ProcessStepId == stepId)
                .FirstOrDefault();

            ProcessStep = step;

            if (ProcessStep != null)
                questionsAndAnswers = GetStepQuestionsAndAnswers(ProcessStep);
            
            return ProcessStep != null ? new SetNextStepResult(questionsAndAnswers, true, null)
                : new SetNextStepResult(null, false, "Process Step not found.");
        }

        /// <summary>
        /// Gets questions and answers for a step.
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="step"/> is null.</exception>
        public IList<ProcessQuestionViewModel> GetStepQuestionsAndAnswers(ProcessStepInfo step)
        {
            if (step == null)
                throw new ArgumentNullException("step");

            if (QuestionsAndAnswers == null)
            {
                LogError("In ProcessViewModel.GetStepQuestionsAndAnswers QuestionsAndAnswers is null");
                return new List<ProcessQuestionViewModel>();
            }
            
            var questionsAndAnswers = QuestionsAndAnswers
                    .Where(questionAnswerViewModel => questionAnswerViewModel.ProcessStepId == step.ProcessStepId)
                    .ToList();
            return questionsAndAnswers;
        }

        /// <summary>
        /// Gets the step question and answer represented by the question ID.
        /// </summary>
        /// <param name="questionId">The question identifier.</param>
        /// <returns></returns>
        public ViewModelResult<ProcessQuestionViewModel> GetStepQuestionAndAnswer(int questionId)
        {
            var questionAndAnswer = QuestionsAndAnswers
                    .Where(questionAnswerViewModel => questionAnswerViewModel.ProcessQuestionId == questionId)
                    .FirstOrDefault();
            var result = questionAndAnswer != null
                ? new ViewModelResult<ProcessQuestionViewModel>(questionAndAnswer, success: true, errorMessage: string.Empty)
                : new ViewModelResult<ProcessQuestionViewModel>(null, success: false, errorMessage: string.Format("Unable to find Question: {0}", questionId)); 
            
            return result;
        }

        /// <summary>
        /// Gets the step with the associated ID.
        /// </summary>
        /// <param name="stepId">The step identifier.</param>
        /// <returns></returns>
        public ViewModelResult<ProcessStepInfo> GetStep(int stepId)
        {
            ProcessStepInfo step = null;
            if (Process != null)
            {
                step = Process.ProcessSteps
                    .Where(processStep => processStep.ProcessStepId == stepId)
                    .FirstOrDefault();
            }

            var result = step != null
                ? new ViewModelResult<ProcessStepInfo>(step, success: true, errorMessage: string.Empty)
                : new ViewModelResult<ProcessStepInfo>(null, success: false, errorMessage: string.Format("Unable to find Step: {0}", stepId));

            return result;
        }

        /// <summary>
        /// Gets the current step, which is the first incomplete step in the list.
        /// </summary>
        /// <param name="steps">The steps.</param>
        /// <returns></returns>
        public ProcessStepInfo GetFirstIncompleteStep(IList<ProcessStepInfo> steps)
        {
            return steps
                .Where(processStepInfo => { 
                    var status = GetStepStatus(processStepInfo);
                    return status == StepStatus.InProgress || status == StepStatus.Incomplete;
                })
                .FirstOrDefault();
        }

        private ProcessQuestionInfo GetQuestionForAnswer(OrderProcessDetailInfo processInfo, OrderProcessAnswerInfo answerInfo)
        {
            var questionInfo = from step in processInfo.ProcessSteps
                               from question in step.ProcessQuestions
                               where question.ProcessQuestionId == answerInfo.ProcessQuestionId
                               select question;

            return questionInfo.FirstOrDefault();
        }

        /// <summary>
        /// Saves the order process' answers for an order asynchronously.
        /// </summary>
        /// <param name="currentProcessedPartQty">The current processed part quantity.</param>
        /// <returns>
        /// <see cref="ViewModelResult" /> with the result of the Set
        /// </returns>
        public async Task<ViewModelResult> SaveCurrentOrderProcessAnswersAsync(int currentProcessedPartQty)
        {
            IsBusy = true;

            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            // Save answers
            var orderProcessAnswers = QuestionsAndAnswers
                .Select(processViewModel => processViewModel.AnswerInfo)
                .ToList();
            var request = new OrderProcessAnswerSaveRequest
            {
                OrderId = OrderId,
                OrderProcessId = OrderProcessId ??
                    orderProcessAnswers.FirstOrDefault()?.OrderProcessId ??
                    -1,

                OrderProcessAnswers = orderProcessAnswers,
                UserId = loginViewModel.UserProfile.UserId,
                CurrentProcessedPartQty = currentProcessedPartQty
            };

            var processService = ServiceContainer.Resolve<IOrderProcessService>();
            processService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var response = await processService.SaveProcessAnswersAsync(request);
            IsBusy = false;

            return new ViewModelResult(response.Success, response.ErrorMessage);
        }

        /// <summary>
        /// Gets the status for a step.
        /// </summary>
        /// <param name="stepInfo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stepInfo"/> is null.
        /// </exception>
        public StepStatus GetStepStatus(ProcessStepInfo stepInfo)
        {
            if (stepInfo == null)
                throw new ArgumentNullException(nameof(stepInfo));

            if (ShouldSkipStep(stepInfo))
                return StepStatus.Skipped;

            var questionsAndAnswers = GetStepQuestionsAndAnswers(stepInfo);

            var completedAnswers = questionsAndAnswers
                .Count(questionAnswerViewModel => questionAnswerViewModel.Completed);

            if (questionsAndAnswers.Count == completedAnswers)
                return StepStatus.Completed;

            if (completedAnswers > 0)
                return StepStatus.InProgress;
            return StepStatus.Incomplete;
        }

        private bool ShouldSkipStep(ProcessStepInfo stepInfo)
        {
            if (stepInfo == null)
            {
                throw new ArgumentNullException(nameof(stepInfo));
            }

            var previouslyExcluded = false;
            _skipStepDictionary.TryGetValue(stepInfo.ProcessStepId, out previouslyExcluded);

            // Check conditions
            var isExcluded = false;
            foreach (var condition in stepInfo.Conditions)
            {
                var type = (ConditionInputType)Enum.Parse(typeof(ConditionInputType), condition.InputType);

                if (type == ConditionInputType.ProcessQuestion)
                {
                    var question = QuestionsAndAnswers?.FirstOrDefault(i => i.ProcessQuestionId == condition.ProcessQuestionId);

                    if (question != null && question.Completed)
                    {
                        var op = (EqualityOperator)Enum.Parse(typeof(EqualityOperator), condition.Operator);
                        var meetsCondition = ConditionHelpers.MeetsCondition(question.InputType, condition.Value, op, question.Answer);

                        if (!meetsCondition)
                        {
                            isExcluded = true;
                            break;
                        }
                    }
                }
                else if (type == ConditionInputType.PartTag)
                {
                    var partMarkInfo = ServiceContainer.Resolve<OrderViewModel>()?.ActiveOrder?.PartMarkLines;

                    if (partMarkInfo == null || !partMarkInfo.Lines.Contains(condition.Value))
                    {
                        isExcluded = true;
                        break;
                    }
                }
            }

            if (previouslyExcluded != isExcluded)
            {
                // Get step answers
                foreach (var answer in _questionsAndAnswers ?? Enumerable.Empty<ProcessQuestionViewModel>())
                {
                    if (answer.ProcessStepId != stepInfo.ProcessStepId)
                    {
                        continue;
                    }

                    if (isExcluded)
                    {
                        answer.Skip();
                    }
                    else
                    {
                        answer.Reset();
                    }
                }
            }

            _skipStepDictionary[stepInfo.ProcessStepId] = isExcluded;

            return isExcluded;
        }

        /// <summary>
        /// Returns a value indicating if the process step is valid.
        /// </summary>
        /// <param name="stepInfo"></param>
        /// <returns>
        /// <c>true</c> if step is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsStepValid(ProcessStepInfo stepInfo)
        {
            if (stepInfo == null)
                throw new ArgumentNullException("stepInfo");
            
            var isValid = true;
            var questionsAndAnswers = GetStepQuestionsAndAnswers(stepInfo);
            var isAnyInvalid = questionsAndAnswers
                .Where(questionViewModel => questionViewModel.IsValid == false)
                .Any();

            if (isAnyInvalid == true)
                isValid = false;

            return isValid;
        }

        /// <summary>
        /// Gets the status for an order process.
        /// </summary>
        /// <param name="processInfo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processInfo"/> is null.
        /// </exception>
        public ProcessStatus GetProcessStatus(OrderProcessInfo processInfo)
        {
            if (processInfo == null)
                throw new ArgumentNullException("processInfo");

            var status = ProcessStatus.Incomplete;

            if (processInfo.Started > DateTime.MinValue)
                status = ProcessStatus.InProgress;
            if (processInfo.Ended > DateTime.MinValue)
                status = ProcessStatus.Completed;
                
            return status;
        }

        /// <summary>
        /// Gets the active order process.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        private async Task<OrderCurrentProcessResponse> GetActiveOrderProcessAsync(int orderId)
        {
            var processService = ServiceContainer.Resolve<IOrderProcessService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return null;

            processService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var request = new OrderCurrentProcessRequest
            {
                OrderId = orderId,
                UserId = loginViewModel.UserProfile.UserId
            };

            return await processService.GetCurrentOrderProcess(request);
        }

        /// <summary>
        /// Gets answers for a list.
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public IList<string> GetAnswerList(int listId)
        {
            IList<string> answers = null;

            if (Process != null)
            {
                var answerListInfo = Process.Lists.FirstOrDefault(listInfo => listInfo.ListId == listId);
                answers = answerListInfo != null ? answerListInfo.Values : null;
            }
            return answers;
        }

        private void RegisterForIsValidOnQuestions()
        {
            if (QuestionsAndAnswers != null)
                foreach (var questionAndAnswer in QuestionsAndAnswers)
                {
                    questionAndAnswer.IsValidChanged += QuestionAndAnswer_IsValidChanged;
                    questionAndAnswer.IsDirty += QuestionAndAnswer_IsDirty;
                }
        }

        private void UnregisterForIsValidOnQuestions()
        {
            if (QuestionsAndAnswers != null)
                foreach (var questionAndAnswer in QuestionsAndAnswers)
                {
                    questionAndAnswer.IsValidChanged -= QuestionAndAnswer_IsValidChanged;
                    questionAndAnswer.IsDirty -= QuestionAndAnswer_IsDirty;
                }
        }

        /// <summary>
        /// Protected method for validating the ViewModel
        /// - Fires PropertyChanged for IsValid and Errors
        /// </summary>
        protected override void Validate()
        {
            if (QuestionsAndAnswers != null)
            {
                foreach (var questionAndAnswer in QuestionsAndAnswers)
                {
                    var questionIsValid = questionAndAnswer.IsValid; 
                    var stepResult = GetStep(questionAndAnswer.ProcessStepId);
                    var stepName = stepResult.Result != null ? 
                        string.Format("{0} {1}", stepResult.Result.StepOrder, stepResult.Result.Name) :
                        string.Empty;
                    var errorMessage = string.Format("Step: {0} Question: {1} is invalid.",
                        stepName, questionAndAnswer.Name);
                    
                    ValidateProperty(() => !questionIsValid, errorMessage);
                }
            }

            base.Validate();
        }

        /// <summary>
        /// Gets the next process step.
        /// </summary>
        /// <returns></returns>
        public ProcessStepInfo GetNextStep()
        {
            ProcessStepInfo nextStep = null;

            if (Process != null && ProcessStep != null)
            {
                nextStep = Process.ProcessSteps
                    .SkipWhile(processStep => processStep.ProcessStepId != ProcessStep.ProcessStepId) // Skip to current step
                    .Skip(1) // Skip current step
                    .SkipWhile(ShouldSkipStep)
                    .FirstOrDefault();
            }

            return nextStep;
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

        #endregion


        #region Events

        void QuestionAndAnswer_IsValidChanged(object sender, EventArgs e)
        {
            Validate();
        }

        private void QuestionAndAnswer_IsDirty(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        private void QuestionOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var question = sender as ProcessQuestionViewModel;

            if (question == null || propertyChangedEventArgs.PropertyName != nameof(ProcessQuestionViewModel.Answer) || _processStep == null)
            {
                return;
            }

            var isInCurrentStep = question.ProcessStepId == _processStep.ProcessStepId;
            var hasCorrectInputType = question.InputType == InputTypes.DecimalBefore ||
                                      question.InputType == InputTypes.DecimalAfter;

            if (!isInCurrentStep || !hasCorrectInputType)
            {
                return;
            }

            var questionsInStep = _questionsAndAnswers
                .Where(q => q.ProcessStepId == _processStep.ProcessStepId)
                .ToList();

            var beforeAnswerString = questionsInStep.FirstOrDefault(q => q.InputType == InputTypes.DecimalBefore)?.Answer;
            var afterAnswerString = questionsInStep.FirstOrDefault(q => q.InputType == InputTypes.DecimalAfter)?.Answer;
            var decimalDifferenceQuestion = questionsInStep.FirstOrDefault(q => q.InputType == InputTypes.DecimalDifference);

            decimal beforeAnswer;
            decimal afterAnswer;
            decimal differenceAnswer;

            if (string.IsNullOrEmpty(beforeAnswerString) || string.IsNullOrEmpty(afterAnswerString) ||
                decimalDifferenceQuestion == null)
            {
                return;
            }

            if (decimal.TryParse(beforeAnswerString, out beforeAnswer) &&
                decimal.TryParse(afterAnswerString, out afterAnswer))
            {
                differenceAnswer = Math.Abs(beforeAnswer - afterAnswer);
            }
            else
            {
                differenceAnswer = 0;
            }

            // Answer difference question
            var differenceAnswerString = differenceAnswer.ToString();
            decimalDifferenceQuestion.CalculatedAnswer = differenceAnswerString;
            decimalDifferenceQuestion.Answer = differenceAnswerString;

            if (decimalDifferenceQuestion.IsValid)
            {
                var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
                decimalDifferenceQuestion.CompletedBy = loginViewModel.UserProfile.UserId;

                if (decimalDifferenceQuestion.CompletedDate == DateTime.MinValue)
                {
                    decimalDifferenceQuestion.CompletedDate = DateTime.Now;
                }
            }
        }

        #endregion
    }
}
