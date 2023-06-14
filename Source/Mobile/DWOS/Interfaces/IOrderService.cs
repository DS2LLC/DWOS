using DWOS.Services.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Defines functionality for order API access.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Gets or sets the root URL for the API.
        /// </summary>
        string RootUrl { get; set; }

        /// <summary>
        /// Retrieves orders asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        Task<OrdersResponse> GetOrdersAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves order details asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        Task<OrderDetailResponse> GetOrderDetailAsync(OrderDetailRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Checks an order in asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        Task<ResponseBase> CheckInOrderAsync(CheckInRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Performs order lookup asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        Task<OrdersResponse> FindOrdersAsync(FindOrdersRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a schedule of orders asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        Task<OrderScheduleResponse> GetScheduleAsync(OrderScheduleRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}
