using DWOS.Services;
using DWOS.Services.Messages;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Implements <see cref="IBatchService"/>.
    /// </summary>
    internal class BatchService : ServiceBase, IBatchService
    {
        public BatchService(ILogService logService) : base(logService)
        {
        }

        public Task<BatchesResponse> GetBatchesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<BatchesResponse>("/batch/getAll", cancellationToken);
        }

        public Task<BatchDetailResponse> GetBatchDetailAsync(BatchDetailRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return GetAsync<BatchDetailResponse>($"/batch/get/?batchId={request.BatchId}", cancellationToken);
        }

        public async Task<ResponseBase> CheckInBatchAsync(BatchCheckInRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var response = await PostAsync<ResponseBase>("/batch/checkin", request, cancellationToken);
            return response;
        }

        public Task<BatchScheduleResponse> GetScheduleAsync(BatchScheduleRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var dept = WebUtility.UrlEncode(request.Department);
            return GetAsync<BatchScheduleResponse>($"/batch/schedule/?department={dept}", cancellationToken);
        }
    }
}
