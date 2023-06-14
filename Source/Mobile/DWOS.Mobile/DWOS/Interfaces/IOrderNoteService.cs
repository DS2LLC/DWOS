using DWOS.Services.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Defines functionality for part API access.
    /// </summary>
    interface IOrderNoteService
    {
        /// <summary>
        /// Gets or sets the root URL for the API.
        /// </summary>
        string RootUrl { get; set; }

        /// <summary>
        /// Retrieves order notes asynchronously.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OrderNotesResponse> GetOrderNotesAsync(OrderNotesRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Saves order notes asynchronously.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseBase> SaveOrderNotesAsync(SaveOrderNotesRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}
