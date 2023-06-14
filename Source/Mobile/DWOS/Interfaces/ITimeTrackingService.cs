using System.Threading;
using System.Threading.Tasks;
using DWOS.Services.Messages;

namespace DWOS
{
    interface ITimeTrackingService
    {
        /// <summary>
        /// Gets or sets the root URL for the API.
        /// </summary>
        string RootUrl { get; set; }

        Task<TimerInfoResponse> GetInfoAsync(OrderTimerRequest request, CancellationToken cancellationToken = default(CancellationToken));

        Task<TimerInfoResponse> GetInfoAsync(BatchTimerRequest request, CancellationToken cancellationToken = default(CancellationToken));

        Task<ResponseBase> StartProcessTimerAsync(BatchTimerRequest request, CancellationToken cancellationToken = default(CancellationToken));
        Task<ResponseBase> StartProcessTimerAsync(OrderTimerRequest request, CancellationToken cancellationToken = default(CancellationToken));
        Task<ResponseBase> StopProcessTimerAsync(BatchTimerRequest request, CancellationToken cancellationToken = default(CancellationToken));
        Task<ResponseBase> StopProcessTimerAsync(OrderTimerRequest request, CancellationToken cancellationToken = default(CancellationToken));
        Task<ResponseBase> StartLaborTimerAsync(BatchTimerRequest request, CancellationToken cancellationToken = default(CancellationToken));
        Task<ResponseBase> StartLaborTimerAsync(OrderTimerRequest request, CancellationToken cancellationToken = default(CancellationToken));
        Task<ResponseBase> StopLaborTimerAsync(BatchTimerRequest request, CancellationToken cancellationToken = default(CancellationToken));
        Task<ResponseBase> StopLaborTimerAsync(OrderTimerRequest request, CancellationToken cancellationToken = default(CancellationToken));
        Task<ResponseBase> PauseLaborTimerAsync(BatchTimerRequest request, CancellationToken cancellationToken = default(CancellationToken));
        Task<ResponseBase> PauseLaborTimerAsync(OrderTimerRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}