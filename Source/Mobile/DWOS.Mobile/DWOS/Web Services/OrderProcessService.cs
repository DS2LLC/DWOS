using DWOS.Services.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.Services
{
    /// <summary>
    /// Implements <see cref="IOrderProcessService"/>.
    /// </summary>
    internal class OrderProcessService : ServiceBase, IOrderProcessService
    {
        public OrderProcessService(ILogService logService) : base(logService)
        {
        }

        public Task<OrderProcessesResponse> GetOrderProcessesAsync(OrderProcessesRequest request, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return GetAsync<OrderProcessesResponse>($"/orderprocesses/getAll/?orderId={request.OrderId}", cancellationToken);
        }

        public Task<ProcessResponse> GetProcessAsync(ProcessRequest request, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return GetAsync<ProcessResponse>($"/processes/get/?processId={request.ProcessId}", cancellationToken);
        }

        public Task<OrderProcessDetailResponse> GetOrderProcessDetailAsync(OrderProcessDetailRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return GetAsync<OrderProcessDetailResponse>($"/orderprocesses/get/?orderProcessId={request.OrderProcessId}", cancellationToken);
        }

        public async Task<OrderProcessAnswerResponse> GetProcessAnswersAsync(OrderProcessAnswerRequest request, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var response = await PostAsync<OrderProcessAnswerResponse>("/orderprocessanswer/retrieve", request, cancellationToken);
            return response;
        }


        public async Task<ResponseBase> SaveProcessAnswersAsync(OrderProcessAnswerSaveRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var response = await PostAsync<ResponseBase>("/orderprocessanswer/save", request, cancellationToken);
            return response;
        }

        public Task<OrderCurrentProcessResponse> GetCurrentOrderProcess(OrderCurrentProcessRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return GetAsync<OrderCurrentProcessResponse>($"/orderprocesses/getcurrent/?orderId={request.OrderId}", cancellationToken);
        }
    }
}
