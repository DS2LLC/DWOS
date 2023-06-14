using DWOS.Services.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Defines functionality for batch process API access.
    /// </summary>
    interface IBatchProcessService
    {
        /// <summary>
        /// Gets or sets the root URL for the API.
        /// </summary>
        string RootUrl { get; set; }

        /// <summary>
        /// Retrieves batch processes asynchronously.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<BatchProcessesResponse> GetBatchProcessesAsync(BatchProcessesRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves process answers asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<BatchProcessAnswerResponse> GetProcessAnswersAsync(BatchProcessAnswerRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Saves process answers asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ResponseBase> SaveProcessAnswersAsync(BatchProcessAnswerSaveRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves information about a batch's current process asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<BatchCurrentProcessResponse> GetCurrentBatchProcess(BatchCurrentProcessRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}
