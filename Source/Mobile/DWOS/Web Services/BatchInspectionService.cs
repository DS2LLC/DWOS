using DWOS.Services;
using DWOS.Services.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Implements <see cref="IBatchInspectionService"/>.
    /// </summary>
    class BatchInspectionService : ServiceBase, IBatchInspectionService
    {
        public BatchInspectionService(ILogService logService) : base(logService)
        {
        }

        public async Task<ResponseBase> BatchInspectionSaveCompletedAsync(BatchInspectionSaveCompletedRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var response = await PostAsync<ResponseBase>("/batchInspections/saveCompleted", request, cancellationToken);
            return response;
        }
    }
}
