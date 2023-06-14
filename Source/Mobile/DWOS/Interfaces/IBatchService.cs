using DWOS.Services.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Defines functionality for batch API access.
    /// </summary>
    interface IBatchService
    {
        /// <summary>
        /// Gets or sets the root URL for the API.
        /// </summary>
        string RootUrl { get; set; }

        /// <summary>
        /// Gets the batches asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        Task<BatchesResponse> GetBatchesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the batch detail asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        Task<BatchDetailResponse> GetBatchDetailAsync(BatchDetailRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Checks batch in asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        Task<ResponseBase> CheckInBatchAsync(BatchCheckInRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the batch schedule asynchronously.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<BatchScheduleResponse> GetScheduleAsync(BatchScheduleRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}
