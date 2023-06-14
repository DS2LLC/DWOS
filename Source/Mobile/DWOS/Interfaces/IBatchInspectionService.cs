using DWOS.Services.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Defines functionality for batch inspection API access.
    /// </summary>
    public interface IBatchInspectionService
    {
        /// <summary>
        /// Gets or sets the root URL for the API.
        /// </summary>
        string RootUrl { get; set; }

        /// <summary>
        /// Completes a batch inspection asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ResponseBase> BatchInspectionSaveCompletedAsync(BatchInspectionSaveCompletedRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}
