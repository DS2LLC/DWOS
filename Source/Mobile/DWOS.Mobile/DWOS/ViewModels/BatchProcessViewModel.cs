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
    /// View Model for batch process functionality
    /// </summary>
    public class BatchProcessViewModel : ViewModelBase
    {
        #region Fields

        IList<ProcessQuestionViewModel> _questionsAndAnswers;
        readonly IDictionary<int, bool> _skipStepDictionary = new Dictionary<int, bool>();
        ProcessStepInfo _processStep;
        ProcessInfo _process;
        IList<BatchProcessInfo> _batchProcesses;
        BatchStatusInfo _batchStatus;
        int _currentBatchId;
        int _currentBatchProcessId;
        private bool _isDirty;
        private bool _hasActiveProcessTimer;
        private bool _hasActiveLaborTimer;
        private bool _isUserActiveOperator;

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
        public ProcessInfo Process
        {
            get { return _process; }
            private set
            {
                _process = value;
                OnPropertyChanged("Process");
            }
        }

        /// <summary>
        /// Gets the current process.
        /// </summary>
        /// <value>
        /// The current process.
        /// </value>
        public IList<BatchProcessInfo> BatchProcesses
        {
            get { return _batchProcesses; }
            private set
            {
                _batchProcesses = value;
                OnPropertyChanged("BatchProcesses");
            }
        }

        /// <summary>
        /// Gets the order status.
        /// </summary>
        /// <value>
        /// The order status.
        /// </value>
        public BatchStatusInfo BatchStatus
        {
            get { return _batchStatus; }
            private set
            {
                _batchStatus = value;
                OnPropertyChanged("BatchStatus");
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
        /// Gets or sets the current batch ID.
        /// </summary>
        public int BatchId
        {
            get { return _currentBatchId; }
            private set
            {
                _currentBatchId = value;
                OnPropertyChanged("BatchId");
            }
        }

        /// <summary>
        /// Gets or sets the current batch process ID.
        /// </summary>
        public int BatchProcessId
        {
            get { return _currentBatchProcessId; }
            private set
            {
                _currentBatchProcessId = value;
                OnPropertyChanged("BatchProcessId");
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

        public BatchProcessViewModel()
        {
            QuestionsAndAnswers = new List<ProcessQuestionViewModel>();
        }

        /// <summary>
        /// Populates this instance with all available processes for a batch
        /// asynchronously.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns><see cref="GetProcessesResult"/> representing the request.</returns>
        public async Task<ViewModelResult> GetBatchProcessesAsync(int batchId)
        {
            IsBusy = true;
            InvalidateViewModel();

            var batchProcessService = ServiceContainer.Resolve<IBatchProcessService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();

            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            batchProcessService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var processRequest = new BatchProcessesRequest
            {
                BatchId = batchId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var result = await batchProcessService.GetBatchProcessesAsync(processRequest);
            if (result.Success == true && string.IsNullOrEmpty(result.ErrorMessage))
            {
                BatchId = batchId;
                BatchStatus = result.BatchStatus;
                BatchProcesses = result.BatchProcesses;
                // Since the current process is unknown, do not retrieve timer info
            }

            IsBusy = false;

            return new ViewModelResult(result.Success, result.ErrorMessage);
        }

        /// <summary>
        /// Populates this instance with the current proccess for a batch
        /// asynchronously.
        /// </summary>
        /// <remarks>
        /// Sets <see cref="Process"/>, <see cref="ProcessStep"/>,
        /// <see cref="BatchId"/>, <see cref="QuestionsAndAnswers"/> if
        /// successful.
        /// </remarks>
        /// <param name="batchId">The order identifier.</param>
        /// <returns><see cref="GetProcessResult"/> representing the request. </returns>
        public async Task<ViewModelResult> GetCurrentProcessForBatchAsync(int batchId)
        {
            IsBusy = true;
            InvalidateViewModel();
            BatchId = batchId;

            ViewModelResult viewModelResult = null;
            var response = await GetActiveBatchProcessAsync(batchId);
            if (response.Success && string.IsNullOrEmpty(response.ErrorMessage))
            {
                Process = response.Process;
                BatchProcessId = response.BatchProcessId;
                var result = await GetBatchQuestionsAndAnswersAsync(BatchId, BatchProcessId, Process);
                QuestionsAndAnswers = result.QuestionsAndAnswers;
                RegisterForIsValidOnQuestions();
                var currentStep = GetFirstIncompleteStep(Process.ProcessSteps);
                if (currentStep != null)
                    SetNextStep(currentStep.ProcessStepId);
                else if (Process.ProcessSteps.Count > 0)
                    SetNextStep(Process.ProcessSteps[0].ProcessStepId);
                viewModelResult = new ViewModelResult(true, string.Empty);

                await RefreshActiveTimerAsync();
            }
            else
            {
                var message = string.Format("Unable to find current process for Batch: {0}", batchId);
                viewModelResult = new ViewModelResult(false, message);
                Errors.Add("Could not load process.");
                Validate();
            }
            IsBusy = false;

            return viewModelResult;
        }

        /// <summary>
        /// Populates this instance for a batch process asynchronously.
        /// </summary>
        /// <remarks>
        /// Sets <see cref="Process"/>, <see cref="OrderId"/>,
        /// <see cref="QuestionsAndAnswers"/> if successful.
        /// </remarks>
        /// <param name="batchProcess">The order process.</param>
        /// <returns></returns>
        public async Task<ViewModelResult> GetProcessForBatchAsync(BatchProcessInfo batchProcess)
        {
            IsBusy = true;
            ViewModelResult viewModelResult = null;
            InvalidateViewModel();

            var processService = ServiceContainer.Resolve<IOrderProcessService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            processService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var request = new ProcessRequest
            {
                ProcessId = batchProcess.ProcessId,
                UserId = loginViewModel.UserProfile.UserId
            };
            var response = await processService.GetProcessAsync(request);
            if (response.Process != null)
            {
                Process = response.Process;
                BatchId = batchProcess.BatchId;
                BatchProcessId = batchProcess.BatchProcessId;
                var result = await GetBatchQuestionsAndAnswersAsync(BatchId, batchProcess.BatchProcessId, Process);
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
                var message = string.Format("Unable to find Process: {0}", batchProcess.BatchProcessId);
                viewModelResult = new ViewModelResult(false, message);
            }
            IsBusy = false;
            return viewModelResult;
        }

        /// <summary>
        /// Invalidates the properties that represent state.
        /// </summary>
        public override void InvalidateViewModel()
        {
            UnregisterForIsValidOnQuestions();
            Process = null;
            ProcessStep = null;
            BatchStatus = null;
            BatchProcesses = null;
            QuestionsAndAnswers = null;
            BatchId = -1;
            BatchProcessId = -1;
            HasActiveProcessTimer = false;
            HasActiveLaborTimer = false;
            IsUserActiveOperator = false;
            Errors.Clear();
            IsDirty = false;

            base.InvalidateViewModel();
        }

        /// <summary>
        /// Gets the currently saved process questions & answers for a batch asynchronously.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="processId">The process identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processInfo"/> is null.
        /// </exception>
        public async Task<GetProcessQuestionsAndAnswersResult> GetBatchQuestionsAndAnswersAsync(
            int batchId, int batchProcessId, ProcessInfo processInfo)
        {
            if (processInfo == null)
                throw new ArgumentNullException("processList");

            IsBusy = true;
            var batchProcessService = ServiceContainer.Resolve<IBatchProcessService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new GetProcessQuestionsAndAnswersResult(questionsAndAnswers: null, success: false, errorMessage: NotLoggedInMessage);

            batchProcessService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var request = new BatchProcessAnswerRequest
            {
                BatchId = batchId,
                BatchProcessId = batchProcessId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var result = await batchProcessService.GetProcessAnswersAsync(request);
            var questionsAndAnswers = result.BatchProcessAnswers
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
                .Where(processStepInfo =>
                {
                    var status = GetStepStatus(processStepInfo);
                    return status == StepStatus.InProgress || status == StepStatus.Incomplete;
                })
                .FirstOrDefault();
        }

        private ProcessQuestionInfo GetQuestionForAnswer(ProcessInfo processInfo, OrderProcessAnswerInfo answerInfo)
        {
            var questionInfo = from step in processInfo.ProcessSteps
                               from question in step.ProcessQuestions
                               where question.ProcessQuestionId == answerInfo.ProcessQuestionId
                               select question;

            return questionInfo.FirstOrDefault();
        }

        /// <summary>
        /// Saves the order process' answers for a batch asynchronously.
        /// </summary>
        /// <returns>
        /// <see cref="ViewModelResult" /> with the result of the Set
        /// </returns>
        public async Task<ViewModelResult> SaveCurrentBatchProcessAnswersAsync()
        {
            IsBusy = true;

            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            var orderProcessAnswers = QuestionsAndAnswers
                .Select(processViewModel => processViewModel.AnswerInfo)
                .ToList();

            var request = new BatchProcessAnswerSaveRequest
            {
                BatchId = BatchId,
                OrderProcessAnswers = orderProcessAnswers,
                UserId = loginViewModel.UserProfile.UserId
            };

            var batchService = ServiceContainer.Resolve<IBatchProcessService>();
            batchService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var response = await batchService.SaveProcessAnswersAsync(request);
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
            else if (completedAnswers > 0)
                return StepStatus.InProgress;
            else
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
                    var partMarkInfo = ServiceContainer.Resolve<BatchViewModel>()?.FirstOrder?.PartMarkLines;

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
                foreach (var answer in _questionsAndAnswers)
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

        /// <summary>
        /// Gets the status for a batch process.
        /// </summary>
        /// <param name="processInfo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processInfo"/> is null.
        /// </exception>
        public ProcessStatus GetProcessStatus(BatchProcessInfo processInfo)
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
        private async Task<BatchCurrentProcessResponse> GetActiveBatchProcessAsync(int batchId)
        {
            var batchProcessService = ServiceContainer.Resolve<IBatchProcessService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return null;

            batchProcessService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var request = new BatchCurrentProcessRequest
            {
                BatchId = batchId,
                UserId = loginViewModel.UserProfile.UserId
            };

            return await batchProcessService.GetCurrentBatchProcess(request);
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
            // Update DecimalDifference questions in the current step
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

            decimalDifferenceQuestion.Answer = differenceAnswer.ToString();
        }

        #endregion
    }
}
