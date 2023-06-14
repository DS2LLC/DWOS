using DWOS.Services.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.Services
{
    /// <summary>
    /// Implements <see cref="IOrderNoteService"/>.
    /// </summary>
    internal class OrderNoteService : ServiceBase, IOrderNoteService
    {
        public OrderNoteService(ILogService logService) : base(logService)
        {
        }

        public Task<OrderNotesResponse> GetOrderNotesAsync(OrderNotesRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return GetAsync<OrderNotesResponse>($"/ordernotes/get/?orderId={request.OrderId}", cancellationToken);
        }

        public async Task<ResponseBase> SaveOrderNotesAsync(SaveOrderNotesRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var response = await PostAsync<ResponseBase>("/ordernotes/save", request, cancellationToken);
            return response;
        }
    }
}
