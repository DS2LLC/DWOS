using DWOS.Services.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Defines functionality for part API access.
    /// </summary>
    interface IPartService
    {
        /// <summary>
        /// Gets or sets the root URL for the API.
        /// </summary>
        string RootUrl { get; set; }

        /// <summary>
        /// Retrieves part information asynchronously.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PartDetailResponse> GetOrderPartAsync(PartDetailRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}
