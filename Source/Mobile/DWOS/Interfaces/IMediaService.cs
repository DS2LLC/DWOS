using DWOS.Services.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Defines functionality for media API access.
    /// </summary>
    public interface IMediaService
    {
        /// <summary>
        /// Gets or sets the root URL for the API.
        /// </summary>
        string RootUrl { get; set; }

        /// <summary>
        /// Gets media asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The media</returns>
        /// <exception cref="System.ArgumentNullException">request is null</exception>
        Task<MediaDetailResponse> GetMediaAsync(MediaDetailRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}
