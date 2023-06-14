using DWOS.Services.Messages;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.Services
{
    /// <summary>
    /// Implements <see cref="IOrderService"/>
    /// </summary>
    internal class OrderService : ServiceBase, IOrderService
    {
        public OrderService(ILogService logService) : base(logService)
        {
        }

        public Task<OrdersResponse> GetOrdersAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<OrdersResponse>("/orders/getAll", cancellationToken);
        }

        public Task<OrderDetailResponse> GetOrderDetailAsync(OrderDetailRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return GetAsync<OrderDetailResponse>($"/orders/get/?orderId={request.OrderId}&imageSize={request.ImageSize}", cancellationToken);
        }

        public async Task<ResponseBase> CheckInOrderAsync(CheckInRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var response = await PostAsync<ResponseBase>("/orders/checkin", request, cancellationToken);
                return response;
        }

        public Task<OrdersResponse> FindOrdersAsync(FindOrdersRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var searchValue = WebUtility.UrlEncode(request.SearchValue);
            return GetAsync<OrdersResponse>($"/orders/find/?searchValue={searchValue}&includeImage={request.IncludeImage}&imageSize={request.ImageSize}", cancellationToken);
        }

        public Task<OrderScheduleResponse> GetScheduleAsync(OrderScheduleRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var dept = WebUtility.UrlEncode(request.Department);
            return GetAsync<OrderScheduleResponse>($"/orders/schedule?department={dept}", cancellationToken);
        }
    }
}
