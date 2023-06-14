using DWOS.Services;
using DWOS.Services.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Implements <see cref="IBatchProcessService"/>.
    /// </summary>
    internal class BatchProcessService : ServiceBase, IBatchProcessService
    {
        public BatchProcessService(ILogService logService) : base(logService)
        {
        }

        public Task<BatchProcessesResponse> GetBatchProcessesAsync(BatchProcessesRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return GetAsync<BatchProcessesResponse>($"/batchprocesses/get/?batchId={request.BatchId}", cancellationToken);
        }

        public Task<BatchProcessAnswerResponse> GetProcessAnswersAsync(BatchProcessAnswerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException("request");

            return PostAsync<BatchProcessAnswerResponse>("/batchprocessanswer/retrieve", request, cancellationToken);
        }

        public async Task<ResponseBase> SaveProcessAnswersAsync(BatchProcessAnswerSaveRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var response = await PostAsync<ResponseBase>("/batchprocessanswer/save", request, cancellationToken);
            return response;
        }

        public Task<BatchCurrentProcessResponse> GetCurrentBatchProcess(BatchCurrentProcessRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return GetAsync<BatchCurrentProcessResponse>($"/batchprocesses/getbatchcurrent/?batchId={request.BatchId}", cancellationToken);
        }
    }
}
