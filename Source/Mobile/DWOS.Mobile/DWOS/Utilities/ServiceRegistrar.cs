using DWOS.Services;
using DWOS.ViewModels;

namespace DWOS.Utilities
{
    /// <summary>
    /// Class used for registering services for the app
    /// </summary>
    public static class ServiceRegistrar
    {
        /// <summary>
        /// Registers services and view models with <see cref="ServiceContainer"/>.
        /// </summary>
        /// <param name="logService">The client-side logging service to use.</param>
        public static void Startup(ILogService logService)
        {
            var loginViewModel = new LogInViewModel();
            ServiceContainer.Register<AuthenticationInfoProvider>(() => new AuthenticationInfoProvider(loginViewModel));
            ServiceContainer.Register<LogInViewModel>(() => loginViewModel);
            ServiceContainer.Register<ILoginService>(() => new LogInService(logService));
            ServiceContainer.Register<OrderViewModel>();
            ServiceContainer.Register<IOrderService>(() => new OrderService(logService));
            ServiceContainer.Register<ProcessViewModel>();
            ServiceContainer.Register<IOrderProcessService>(() => new OrderProcessService(logService));
            ServiceContainer.Register<IPartService>(() => new PartService(logService));
            ServiceContainer.Register<PartViewModel>();
            ServiceContainer.Register<IInspectionService>(() => new InspectionService(logService));
            ServiceContainer.Register<InspectionViewModel>();
            ServiceContainer.Register<IDWOSLoggingService>(() => new DWOSLoggingService(logService));
            ServiceContainer.Register<BatchViewModel>();
            ServiceContainer.Register<IBatchService>(() => new BatchService(logService));
            ServiceContainer.Register<IBatchProcessService>(() => new BatchProcessService(logService));
            ServiceContainer.Register<IBatchInspectionService>(() => new BatchInspectionService(logService));
            ServiceContainer.Register<BatchProcessViewModel>();
            ServiceContainer.Register<BatchInspectionViewModel>();
            ServiceContainer.Register<IDocumentService>(() => new DocumentService(logService));
            ServiceContainer.Register<IMediaService>(() => new MediaService(logService));
            ServiceContainer.Register<IOrderNoteService>(() => new OrderNoteService(logService));
            ServiceContainer.Register<OrderNotesViewModel>();
            ServiceContainer.Register<EditOrderNoteViewModel>();
            ServiceContainer.Register<ITimeTrackingService>(() => new TimeTrackingService(logService));
        }
    }
}
