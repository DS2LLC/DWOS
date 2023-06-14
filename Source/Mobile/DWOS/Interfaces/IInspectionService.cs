using System.Threading.Tasks;
using System.Threading;
using DWOS.Services.Messages;

namespace DWOS
{
    /// <summary>
    /// Defines functionality for inspection API access.
    /// </summary>
    public interface IInspectionService
    {
        /// <summary>
        /// Gets or sets the root URL for the API.
        /// </summary>
        string RootUrl { get; set; }

        /// <summary>
        /// Gets the next inspection for an order asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<InspectionResponse> GetNextInspectionAsync(InspectionByOrderRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Saves inspection answers asynchronously.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseBase> SaveInspectionAnswersAsync(InspectionSaveAnswerRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}
