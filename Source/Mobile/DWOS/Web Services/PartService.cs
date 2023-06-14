using DWOS.Services.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.Services
{
    /// <summary>
    /// Implements <see cref="IPartService"/>.
    /// </summary>
    internal class PartService : ServiceBase, IPartService
    {
        public PartService(ILogService logService) : base(logService)
        {
        }

        public Task<PartDetailResponse> GetOrderPartAsync(PartDetailRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return GetAsync<PartDetailResponse>($"/parts/get?orderId={request.OrderId}", cancellationToken);
        }
    }
}
