using DWOS.Services.Messages;
using DWOS.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.Services
{
    /// <summary>
    /// Implements <see cref="ILoginService"/>.
    /// </summary>
    internal class LogInService : ServiceBase, ILoginService
    {
        public LogInService(ILogService logService) : base(logService)
        {
        }

        public async Task<UserProfileResponse> LogInUserAsync(string pin, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(pin))
                throw new ArgumentNullException("pin");

            var logInRequest = new UserLogInRequest { UserPin = pin };
            var profile = await PostAsync<UserProfileResponse>("/user/login", logInRequest, cancellationToken);

            return profile;
        }

        public Task<UserResponse> GetUserAsync(UserRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return GetAsync<UserResponse>($"/user/get/?userId={request.RequestedUserId}", cancellationToken);
        }

        public Task<ApplicationSettingsResponse> GetApplicationSettings(CancellationToken cancellationToken = default(CancellationToken))
        {
            return PostAsync<ApplicationSettingsResponse>("/appsettings", new RequestBase(), cancellationToken);
        }

        public async Task<bool> VerifyServer(CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = HttpClientSingleton.Instance;
            if (cancellationToken.IsCancellationRequested)
                return false;

            if (!Uri.TryCreate(RootUrl, UriKind.Absolute, out Uri rootUri))
            {
                return false;
            }

            var isVerified = false;

            try
            {
                var responseMessage = await client.GetAsync(rootUri, cancellationToken);
                isVerified = responseMessage.IsSuccessStatusCode;
            }
            catch(Exception exception)
            {
                var errorService = ServiceContainer.Resolve<ILogService>();
                var message = string.Format("Error in {0}", "LogInService.VerifyServer");
                errorService.LogError(message, exception, context: null, toast: false);
            }

            return isVerified;
        }
    }
}
