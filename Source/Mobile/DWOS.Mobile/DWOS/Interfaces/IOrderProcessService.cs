using DWOS.Services.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Defines functionality for batch process API access.
    /// </summary>
    interface IOrderProcessService
    {
        /// <summary>
        /// Gets or sets the root URL for the API.
        /// </summary>
        string RootUrl { get; set; }

        /// <summary>
        /// Retrieves processes asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<OrderProcessesResponse> GetOrderProcessesAsync(OrderProcessesRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves detailed process information asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ProcessResponse> GetProcessAsync(ProcessRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves detailed order process information asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<OrderProcessDetailResponse> GetOrderProcessDetailAsync(OrderProcessDetailRequest request,CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets process answers asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<OrderProcessAnswerResponse> GetProcessAnswersAsync(OrderProcessAnswerRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Saves process answers asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ResponseBase> SaveProcessAnswersAsync(OrderProcessAnswerSaveRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves information about an order's current process asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<OrderCurrentProcessResponse> GetCurrentOrderProcess(OrderCurrentProcessRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}
