using DWOS.Services.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.Services
{
    internal class TimeTrackingService : ServiceBase, ITimeTrackingService
    {
        public TimeTrackingService(ILogService logService) : base(logService)
        {
        }

        public Task<TimerInfoResponse> GetInfoAsync(OrderTimerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return GetAsync<TimerInfoResponse>($"/timers/orderProcessInfo/?orderId={request.OrderId}&userId={request.UserId}", cancellationToken);
        }

        public Task<TimerInfoResponse> GetInfoAsync(BatchTimerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return GetAsync<TimerInfoResponse>($"/timers/batchProcessInfo/?batchId={request.BatchId}&userId={request.UserId}", cancellationToken);
        }

        public Task<ResponseBase> StartProcessTimerAsync(OrderTimerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return PostAsync<ResponseBase>("/timers/startOrderProcess", request, cancellationToken);
        }

        public Task<ResponseBase> StopProcessTimerAsync(OrderTimerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return PostAsync<ResponseBase>("/timers/stopOrderProcess", request, cancellationToken);
        }

        public Task<ResponseBase> StartProcessTimerAsync(BatchTimerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return PostAsync<ResponseBase>("/timers/startBatchProcess", request, cancellationToken);
        }

        public Task<ResponseBase> StopProcessTimerAsync(BatchTimerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return PostAsync<ResponseBase>("/timers/stopBatchProcess", request, cancellationToken);
        }

        public Task<ResponseBase> StartLaborTimerAsync(BatchTimerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return PostAsync<ResponseBase>("/timers/startBatchLabor", request, cancellationToken);
        }

        public Task<ResponseBase> StartLaborTimerAsync(OrderTimerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return PostAsync<ResponseBase>("/timers/startOrderLabor", request, cancellationToken);
        }

        public Task<ResponseBase> StopLaborTimerAsync(BatchTimerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return PostAsync<ResponseBase>("/timers/stopBatchLabor", request, cancellationToken);
        }

        public Task<ResponseBase> StopLaborTimerAsync(OrderTimerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return PostAsync<ResponseBase>("/timers/stopOrderLabor", request, cancellationToken);
        }

        public Task<ResponseBase> PauseLaborTimerAsync(BatchTimerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return PostAsync<ResponseBase>("/timers/pauseBatchLabor", request, cancellationToken);
        }

        public Task<ResponseBase> PauseLaborTimerAsync(OrderTimerRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return PostAsync<ResponseBase>("/timers/pauseOrderLabor", request, cancellationToken);
        }
    }
}
