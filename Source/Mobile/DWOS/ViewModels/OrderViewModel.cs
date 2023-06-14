using DWOS.Services.Messages;
using DWOS.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DWOS.ViewModels
{
    /// <summary>
    /// View Model for order functionality
    /// </summary>
    public class OrderViewModel : ViewModelBase
    {
        #region Fields
        
        public const string NONE_TEXT = "<NONE>";

        List<OrderInfo> _orders;
        IList<OrderInfo> _orderSearchResults;
        OrderDetailInfo _activeOrder;
        string _filterDepartmentValue;
        string _filterLineValue;
        string _filterStatusValue;
        private FilterType _filterTypeValue = FilterType.Normal;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of orders.
        /// </summary>
        public List<OrderInfo> Orders
        {
            get
            {
                return _orders;
            }
            private set
            {
                _orders = value;
                OnPropertyChanged("Orders");
            }
        }

        /// <summary>
        /// Gets the <see cref="OrderDetailInfo"/> instance for the
        /// active order.
        /// </summary>
        public OrderDetailInfo ActiveOrder
        {
            get
            {
                return _activeOrder;
            }
            private set
            {
                _activeOrder = value;
                OnPropertyChanged("ActiveOrder");
            }
        }

        /// <summary>
        /// Gets a list with order search results.
        /// </summary>
        /// <value>
        /// The order search results.
        /// </value>
        public IList<OrderInfo> OrderSearchResults
        {
            get
            {
                return _orderSearchResults;
            }
            private set
            {
                _orderSearchResults = value;
                OnPropertyChanged("OrderSearchResults");
            }
        }

        /// <summary>
        /// Gets or sets the department value.
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

        public string FilterLineValue
        {
            get { return _filterLineValue; }
            set
            {
                _filterLineValue = value;
                OnPropertyChanged(nameof(FilterLineValue));
            }
        }

        public FilterType FilterTypeValue
        {
            get { return _filterTypeValue; }
            set
            {
                _filterTypeValue = value;
                OnPropertyChanged(nameof(FilterTypeValue));
            }
        }

        /// <summary>
        /// Gets or sets the work status value.
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

        public bool CanProcess => ActiveOrder != null
            && !ActiveOrder.IsInBatch
            && ActiveOrder.WorkStatus == ApplicationSettings.Settings.WorkStatusInProcess;

        public bool CanInspect => ActiveOrder != null
            && !ActiveOrder.IsInBatch
            && ActiveOrder.WorkStatus == ApplicationSettings.Settings.WorkStatusPendingInspection;

        #endregion

        #region Methods

        /// <summary>
        /// Populates this instance with order summaries asynchronously.
        /// </summary>
        /// <remarks>
        /// If successful, the <see cref="Orders"/> collection
        /// is set with the results.
        /// </remarks>
        /// <returns>
        /// A <see cref="ViewModelResult"/> with the succes of the login and possible error message.
        /// </returns>
        public async Task<ViewModelResult> GetOrderSummariesAsync()
        {
            IsBusy = true;

            var orderService = ServiceContainer.Resolve<IOrderService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            orderService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var hasError = false;
            var errorMsg = string.Empty;

            var ordersResponse = await orderService.GetOrdersAsync();
            if (ordersResponse.Success && string.IsNullOrEmpty(ordersResponse.ErrorMessage))
            {
                if (_filterTypeValue == FilterType.SchedulePriority)
                {
                    var scheduleResponse = await orderService.GetScheduleAsync(new OrderScheduleRequest
                    {
                        UserId = loginViewModel.UserProfile.UserId, 
                        Department = _filterDepartmentValue
                    });

                    if (scheduleResponse.Success && string.IsNullOrEmpty(scheduleResponse.ErrorMessage))
                    {
                        Orders = FilterBySchedule(ordersResponse.Orders, scheduleResponse.Schedule);

                        if (ActiveOrder != null && Orders.All(o => o.OrderId != ActiveOrder.OrderId))
                        {
                            ActiveOrder = null;
                        }
                    }
                    else
                    {
                        hasError = true;
                        errorMsg = scheduleResponse.ErrorMessage;
                    }
                }
                else
                {
                    Orders = Filter(ordersResponse.Orders);
                    CheckActiveOrderIsValid();
                }
            }
            else
            {
                hasError = true;
                errorMsg = ordersResponse.ErrorMessage;
            }

            if (hasError)
            {
                Orders = new List<OrderInfo>();
                ActiveOrder = null;
            }

            IsBusy = false;

            return new ViewModelResult(!hasError, errorMsg);
        }

        private void CheckActiveOrderIsValid()
        {
            var clearActiveOrder = false;
            if (ActiveOrder != null)
            {
                if (!string.IsNullOrEmpty(FilterDepartmentValue) && ActiveOrder.Location != FilterDepartmentValue)
                    clearActiveOrder = true;

                if (!string.IsNullOrEmpty(FilterStatusValue) && ActiveOrder.WorkStatus != FilterStatusValue)
                    clearActiveOrder = true;

                if (!string.IsNullOrEmpty(_filterLineValue) && ActiveOrder.CurrentLine != _filterLineValue)
                    clearActiveOrder = true;
            }

            if (clearActiveOrder)
                ActiveOrder = null;
        }

        private List<OrderInfo> Filter(List<OrderInfo> list)
        {
            var filteredOrders = list;

            if (!string.IsNullOrEmpty(FilterDepartmentValue))
                filteredOrders = filteredOrders.Where(orderInfo => orderInfo.Location == FilterDepartmentValue).ToList();

            if (!string.IsNullOrEmpty(FilterStatusValue))
                filteredOrders = filteredOrders.Where(orderInfo => orderInfo.WorkStatus == FilterStatusValue).ToList();

            if (!string.IsNullOrEmpty(_filterLineValue))
            {
                filteredOrders = filteredOrders.Where(orderInfo => orderInfo.CurrentLine == _filterLineValue).ToList();
            }

            return filteredOrders;
        }

        private List<OrderInfo> FilterBySchedule(List<OrderInfo> orders, OrderSchedule schedule)
        {
            var scheduledOrders = schedule?.OrderIds;
            if (scheduledOrders == null)
            {
                return new List<OrderInfo>();
            }

            var filteredOrders = orders;

            if (!string.IsNullOrEmpty(_filterStatusValue))
            {
                filteredOrders = filteredOrders.Where(o => o.WorkStatus == _filterStatusValue).ToList();
            }

            if (!string.IsNullOrEmpty(_filterLineValue))
            {
                filteredOrders = filteredOrders.Where(orderInfo => orderInfo.CurrentLine == _filterLineValue).ToList();
            }

            return filteredOrders
                .Where(o => scheduledOrders.Contains(o.OrderId))
                .OrderBy(o => o.SchedulePriority)
                .ToList();
        }

        /// <summary>
        /// Sets the active order asynchronously.
        /// </summary>
        /// <remarks>
        /// If successful, the <see cref="ActiveOrder"/> is set
        /// to the results.
        /// </remarks>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>
        /// A <see cref="ViewModelResult"/> with the succes of the login and
        /// possible error message.
        /// </returns>
        public async Task<ViewModelResult> SetActiveOrderDetailAsync(int orderId, int imageSize = 512)
        {
            IsBusy = true;
            var result = await GetOrderDetailWorkerAsync(orderId, imageSize);
            if (result.Success && string.IsNullOrEmpty(result.ErrorMessage))
                ActiveOrder = result.Result;
            else
                ActiveOrder = null;
            IsBusy = false;

            return new ViewModelResult(result.Success, result.ErrorMessage);
        }

        /// <summary>
        /// Gets an order's details asynchronously.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>A <see cref="ViewModelResult"/> with the succes of the login and possible error message.</returns>
        public async Task<OrderDetailInfo> GetOrderDetailAsync(int orderId, int imageSize = 512)
        {
            IsBusy = true;
            var result = await GetOrderDetailWorkerAsync(orderId, imageSize);
            IsBusy = false;

            return result.Result;
        }

        private async Task<ViewModelResult<OrderDetailInfo>> GetOrderDetailWorkerAsync(int orderId, int imageSize = 512)
        {
            var orderService = ServiceContainer.Resolve<IOrderService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult<OrderDetailInfo>(result: null, success: false, errorMessage: NotLoggedInMessage);

            orderService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var orderRequest = new OrderDetailRequest
            {
                OrderId = orderId,
                UserId = loginViewModel.UserProfile.UserId,
                ImageSize = imageSize
            };
            
            var response = await orderService.GetOrderDetailAsync(orderRequest);
            return new ViewModelResult<OrderDetailInfo>(response.OrderDetail, response.Success, response.ErrorMessage);
        }

        /// <summary>
        /// Checks in an order asynchronously.
        /// </summary>
        /// <param name="checkIn">
        /// <see cref="CheckInViewModel"/> instance to use for check in.
        /// </param>
        /// <returns></returns>
        public async Task<ViewModelResult> CheckInOrderAsync(CheckInViewModel checkIn)
        {
            IsBusy = true;

            var orderService = ServiceContainer.Resolve<IOrderService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            if (!checkIn.OrderId.HasValue)
                return new ViewModelResult(success: false, errorMessage: "Invalid Id");

            orderService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var checkInRequest = new CheckInRequest
            {
                OrderId = checkIn.OrderId.Value,
                NextDepartment = checkIn.NextDepartment,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await orderService.CheckInOrderAsync(checkInRequest);
            IsBusy = false;

            return new ViewModelResult(response.Success, response.ErrorMessage);
        }

        /// <summary>
        /// Populates <see cref="OrderSearchResults"/> with search results.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <returns></returns>
        public void FindOrders(string keywords)
        {
            IsBusy = true;
            OrderSearchResults = null;
            
            OrderSearchResults = Orders.Where(orderInfo => orderInfo.OrderId.ToString().Contains(keywords))
                .ToList();
            
            var clearActiveOrder = true;
            if (ActiveOrder != null && OrderSearchResults.Any(orderInfo => orderInfo.OrderId == ActiveOrder.OrderId))
                clearActiveOrder = false; 

            if (clearActiveOrder)
                ActiveOrder = null;
            
            IsBusy = false;
        }

        /// <summary>
        /// Populates <see cref="OrderSearchResults"/> with server search
        /// results asynchronously.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <returns></returns>
        public async Task<ViewModelResult> FindOrdersAsync(string keywords)
        {
            IsBusy = true;
            OrderSearchResults = null;
            ActiveOrder = null;
            var orderService = ServiceContainer.Resolve<IOrderService>();
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            orderService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var findOrdersRequest = new FindOrdersRequest
            {
                SearchValue = keywords,
                IncludeImage = true,
                ImageSize = 48,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await orderService.FindOrdersAsync(findOrdersRequest);
            OrderSearchResults = (IList<OrderInfo>)response.Orders;
            IsBusy = false;

            return new ViewModelResult(response.Success, response.ErrorMessage);
        }

        /// <summary>
        /// Invalidates the properties that represent state.
        /// </summary>
        public override void InvalidateViewModel()
        {
            Orders = null;
            ActiveOrder = null;
            OrderSearchResults = null;
            FilterDepartmentValue = string.Empty;
            FilterStatusValue = string.Empty;

            base.InvalidateViewModel();
        }

        #endregion
    }
}
