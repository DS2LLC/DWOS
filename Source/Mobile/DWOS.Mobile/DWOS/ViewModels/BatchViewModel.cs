using DWOS.Services.Messages;
using DWOS.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DWOS.ViewModels
{
    /// <summary>
    /// View Model for batch functionality
    /// </summary>
    public class BatchViewModel : ViewModelBase
    {
        #region Fields

        public const string NONE_TEXT = "<NONE>";

        string _filterDepartmentValue;
        string _filterStatusValue;
        BatchDetailInfo _activeBatch;
        OrderDetailInfo _firstOrder;
        List<BatchInfo> _batches;
        private FilterType _filterTypeValue;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="BatchIdDetailInfo"/> instance for the
        /// active batch.
        /// </summary>
        /// <value>
        /// The active batch.
        /// </value>
        public BatchDetailInfo ActiveBatch
        {
            get { return _activeBatch; }
            private set
            {
                _activeBatch = value;
                OnPropertyChanged("ActiveBatch");
            }
        }

        public OrderDetailInfo FirstOrder
        {
            get { return _firstOrder; }
            private set
            {
                _firstOrder = value;
                OnPropertyChanged(nameof(FirstOrder));
            }
        }

        /// <summary>
        /// Gets or sets the department filter.
        /// </summary>
        /// <value>
        /// The filter department value.
        /// </value>
        public string FilterDepartmentValue
        {
            get { return _filterDepartmentValue; }
            set
            {
                _filterDepartmentValue = value;
                OnPropertyChanged("FilterDepartmentValue");
            }
        }

        /// <summary>
        /// Gets or sets the work status filter.
        /// </summary>
        /// <value>
        /// The filter status value.
        /// </value>
        public string FilterStatusValue
        {
            get { return _filterStatusValue; }
            set
            {
                _filterStatusValue = value;
                OnPropertyChanged("FilterStatusValue");
            }
        }

        /// <summary>
        /// Gets the list of batches.
        /// </summary>
        /// <value>
        /// The batches.
        /// </value>
        public List<BatchInfo> Batches
        {
            get
            {
                return _batches;
            }
            private set
            {
                _batches = value;
                OnPropertyChanged("Batches");
            }
        }

        public FilterType FilterTypeValue
        {
            get => _filterTypeValue;
            set
            {
                _filterTypeValue = value;
                OnPropertyChanged(nameof(FilterTypeValue));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invalidates the properties that represent state.
        /// </summary>
        public override void InvalidateViewModel()
        {
            ActiveBatch = null;
            FirstOrder = null;
            Batches = null;
            FilterStatusValue = string.Empty;
            FilterDepartmentValue = string.Empty;

            base.InvalidateViewModel();
        }

        /// <summary>
        /// Gets the batch summaries asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<ViewModelResult> GetBatchSummariesAsync()
        {
            IsBusy = true;

            var batchService = ServiceContainer.Resolve<IBatchService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            batchService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var response = await batchService.GetBatchesAsync();
            if (response.Success && string.IsNullOrEmpty(response.ErrorMessage))
            {
                if (_filterTypeValue == FilterType.SchedulePriority)
                {
                    var scheduleResponse = await batchService.GetScheduleAsync(new BatchScheduleRequest
                    {
                        UserId = loginViewModel.UserProfile.UserId, 
                        Department = _filterDepartmentValue
                    });

                    if (scheduleResponse.Success && string.IsNullOrEmpty(scheduleResponse.ErrorMessage))
                    {
                        Batches = FilterBySchedule(response.Batches, scheduleResponse.Schedule);

                        if (ActiveBatch != null && Batches.All(o => o.BatchId != ActiveBatch.BatchId))
                        {
                            ActiveBatch = null;
                        }
                    }
                    else
                    {
                        Batches = null;
                        ActiveBatch = null;
                        FirstOrder = null;
                    }
                }
                else
                {
                    Batches = FilterBatches(response.Batches);
                    CheckActiveBatchIsValid();
                }
            }
            else
            {
                Batches = null;
                ActiveBatch = null;
                FirstOrder = null;
            }

            IsBusy = false;

            return new ViewModelResult(response.Success, response.ErrorMessage);
        }

        /// <summary>
        /// Sets the active batch asynchronously.
        /// </summary>
        /// <param name="batchId">The batch identifier.</param>
        /// <returns></returns>
        public async Task<ViewModelResult> SetActiveBatchDetailAsync(int batchId)
        {
            IsBusy = true;

            var orderViewModel = ServiceContainer.Resolve<OrderViewModel>();

            var result = await GetBatchDetailWorkerAsync(batchId);
            if (result.Success && string.IsNullOrEmpty(result.ErrorMessage))
            {
                ActiveBatch = result.Result;

                if (ActiveBatch == null)
                {
                    FirstOrder = null;
                }
                else
                {
                    FirstOrder = await orderViewModel.GetOrderDetailAsync(ActiveBatch.Orders.OrderBy(o => o).FirstOrDefault());
                }
            }
            else
            {
                ActiveBatch = null;
                FirstOrder = null;
            }

            IsBusy = false;
            return new ViewModelResult(result.Success, result.ErrorMessage);
        }

        /// <summary>
        /// Gets an batch's details asynchronously.
        /// </summary>
        /// <param name="batchId">The batch identifier.</param>
        /// <returns>
        /// A <see cref="ViewModelResult" /> with the succes of the login and possible error message.
        /// </returns>
        public async Task<BatchDetailInfo> GetBatchDetailAsync(int batchId)
        {
            IsBusy = true;
            var result = await GetBatchDetailWorkerAsync(batchId);
            IsBusy = false;

            return result.Result;
        }

        /// <summary>
        /// Gets the batch detail worker asynchronous.
        /// </summary>
        /// <param name="batchId">The batch identifier.</param>
        /// <returns></returns>
        private async Task<ViewModelResult<BatchDetailInfo>> GetBatchDetailWorkerAsync(int batchId)
        {
            var batchService = ServiceContainer.Resolve<IBatchService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult<BatchDetailInfo>(result: null, success: false, errorMessage: NotLoggedInMessage);

            var batchRequest = new BatchDetailRequest
            {
                BatchId = batchId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await batchService.GetBatchDetailAsync(batchRequest);
            return new ViewModelResult<BatchDetailInfo>(response.BatchDetail, response.Success, response.ErrorMessage);
        }

        private List<BatchInfo> FilterBatches(List<BatchInfo> list)
        {
            var filteredBatches = list;

            if (!string.IsNullOrEmpty(FilterDepartmentValue))
                filteredBatches = filteredBatches.Where(batchInfo => batchInfo.Location == FilterDepartmentValue).ToList();

            if (!string.IsNullOrEmpty(FilterStatusValue))
                filteredBatches = filteredBatches.Where(batchInfo => batchInfo.WorkStatus == FilterStatusValue).ToList();

            return filteredBatches;
        }

        private void CheckActiveBatchIsValid()
        {
            var clearActivebatch = false;
            if (ActiveBatch != null)
            {
                if (!string.IsNullOrEmpty(FilterDepartmentValue) && ActiveBatch.Location != FilterDepartmentValue)
                    clearActivebatch = true;

                if (!string.IsNullOrEmpty(FilterStatusValue) && ActiveBatch.WorkStatus != FilterStatusValue)
                    clearActivebatch = true;
            }

            if (clearActivebatch)
            {
                ActiveBatch = null;
                FirstOrder = null;
            }
        }

        /// <summary>
        /// Checks in a batch asynchronously.
        /// </summary>
        /// <param name="checkIn">
        /// <see cref="CheckInViewModel"/> instance to use for check in.
        /// </param>
        /// <returns></returns>
        public async Task<ViewModelResult> CheckInBatchAsync(CheckInViewModel checkIn)
        {
            IsBusy = true;

            var batchService = ServiceContainer.Resolve<IBatchService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            if (!checkIn.BatchId.HasValue)
                return new ViewModelResult(success: false, errorMessage: "Invalid Id");

            batchService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var checkInRequest = new BatchCheckInRequest
            {
                BatchId = checkIn.BatchId.Value,
                NextDepartment = checkIn.NextDepartment,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await batchService.CheckInBatchAsync(checkInRequest);
            IsBusy = false;

            return new ViewModelResult(response.Success, response.ErrorMessage);
        }

        private List<BatchInfo> FilterBySchedule(List<BatchInfo> orders, BatchSchedule schedule)
        {
            var scheduledBatches = schedule?.BatchIds;
            if (scheduledBatches == null)
            {
                return new List<BatchInfo>();
            }

            var filteredBatches = orders;

            if (!string.IsNullOrEmpty(_filterStatusValue))
            {
                filteredBatches = filteredBatches.Where(o => o.WorkStatus == _filterStatusValue).ToList();
            }

            return filteredBatches
                .Where(o => scheduledBatches.Contains(o.BatchId))
                .OrderBy(o => o.SchedulePriority)
                .ToList();
        }

        #endregion
    }
}
