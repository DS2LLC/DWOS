using DWOS.Services.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.Services
{
    /// <summary>
    /// Implements <see cref="IMediaService"/>.
    /// </summary>
    internal class MediaService : ServiceBase, IMediaService
    {
        public MediaService(ILogService logService) : base(logService)
        {
        }

        public Task<MediaDetailResponse> GetMediaAsync(MediaDetailRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return GetAsync<MediaDetailResponse>($"/media/get/?mediaId={request.MediaId}", cancellationToken);
        }
    }
}
