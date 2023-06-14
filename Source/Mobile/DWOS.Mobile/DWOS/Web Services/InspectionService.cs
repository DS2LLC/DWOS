using DWOS.Services;
using DWOS.Services.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Implements <see cref="IInspectionService"/>.
    /// </summary>
    internal class InspectionService: ServiceBase, IInspectionService
    {
        public InspectionService(ILogService logService) : base(logService)
        {
        }

        public Task<InspectionResponse> GetNextInspectionAsync(InspectionByOrderRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return GetAsync<InspectionResponse>($"/orderInspections/getNextByOrder/?orderId={request.OrderId}", cancellationToken);
        }

        public async Task<ResponseBase> SaveInspectionAnswersAsync(InspectionSaveAnswerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var response = await PostAsync<ResponseBase>("/orderInspections/save", request, cancellationToken);
            return response;
        }
    }
}
