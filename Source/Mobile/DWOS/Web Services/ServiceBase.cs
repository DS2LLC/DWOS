using DWOS.Services.Messages;
using DWOS.Utilities;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.Services
{
    /// <summary>
    /// Base class for any provider of services
    /// </summary>
    internal class ServiceBase
    {
        #region Fields

        protected const string ErrorMessageCancelled = "Request canceled.";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the root URL for the API.
        /// </summary>
        /// <remarks>
        /// Used to get a full server URL.
        /// </remarks>
        public string RootUrl { get; set; }

        protected ILogService LogService { get; }

        #endregion

        #region Methods

        protected ServiceBase(ILogService logService)
        {
            LogService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        protected async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken, bool notifyKeepAlive = true) where T: ResponseBase, new()
        {
            try
            {
                if (notifyKeepAlive)
                {
                    var licenseSession = ServiceContainer.Resolve<ILicenseSessionService>();
                    licenseSession.KeepAlive();
                }

                var getUrl = RootUrl + url;
                var client = HttpClientSingleton.Instance;

                // Send request
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, getUrl);

                var authInfo = RetrieveAuthInfo();
                if (authInfo != null)
                {
                    httpRequest.Headers.Add("Authorization", NewBasicAuthHeader(authInfo));
                }

                if (cancellationToken.IsCancellationRequested)
                    return new T { Success = false, ErrorMessage = ErrorMessageCancelled };

                var responseMessage = await client.SendAsync(httpRequest, cancellationToken);

                // Read Response
                using (var responseStream = await responseMessage.Content.ReadAsStreamAsync())
                {
                    if (responseStream.Length == 0)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return new T { Success = false, ErrorMessage = ErrorMessageCancelled };
                        }
                        else
                        {
                            return new T { Success = false, ErrorMessage = $"No content returned from request to {getUrl}" };
                        }
                    }

                    using (var responseReader = new StreamReader(responseStream))
                    {
                        using (var jsonReader = new JsonTextReader(responseReader))
                        {
                            return await Task.Run(() =>
                            JsonSerializer.Create(JsonSerializationSettings.Settings).Deserialize<T>(jsonReader));
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                return HandleException<T>(url, exc);
            }
        }

        /// <summary>
        /// Sends a HTTP POST request to the server.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The partial request path.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="notifyKeepAlive">
        /// If <c>true</c>, sends a request to the keep the session alive
        /// using <see cref="ILicenseSessionService.KeepAlive"/>.
        /// </param>
        /// <returns></returns>
        protected async Task<T> PostAsync<T>(string url, RequestBase request, CancellationToken cancellationToken, bool notifyKeepAlive = true) where T : ResponseBase, new()
        {
            try
            {
                if (notifyKeepAlive)
                {
                    var licenseSession = ServiceContainer.Resolve<ILicenseSessionService>();
                    licenseSession.KeepAlive();
                }

                var postUrl = RootUrl + url;
                var client = HttpClientSingleton.Instance;

                // Send request
                var json = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(request, JsonSerializationSettings.Settings));
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, postUrl);
                httpRequest.Content = content;
                if (cancellationToken.IsCancellationRequested)
                    return new T { Success = false, ErrorMessage = ErrorMessageCancelled };

                var authInfo = RetrieveAuthInfo();
                if (authInfo != null)
                {
                    httpRequest.Headers.Add("Authorization", NewBasicAuthHeader(authInfo));
                }

                var responseMessage = await client.SendAsync(httpRequest, cancellationToken);

                // Read Response
                using (var responseStream = await responseMessage.Content.ReadAsStreamAsync())
                {
                    if (responseStream.Length == 0)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return new T { Success = false, ErrorMessage = ErrorMessageCancelled };
                        }
                        else
                        {
                            return new T { Success = false, ErrorMessage = $"No content returned from request to {postUrl}" };
                        }
                    }

                    using (var responseReader = new StreamReader(responseStream))
                    {
                        using (var jsonReader = new JsonTextReader(responseReader))
                        {
                            return await Task.Run(() =>
                            JsonSerializer.Create(JsonSerializationSettings.Settings).Deserialize<T>(jsonReader));
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                return HandleException<T>(url, exc);
            }
        }

        private T HandleException<T>(string url, Exception exception) where T : ResponseBase, new()
        {
            var message = $"Error with accessing {url}";

            if (exception is WebException webException)
            {
                if (webException.Status == WebExceptionStatus.ConnectFailure)
                {
                    Messenger.Default.Send(new NotificationMessage(MessageNames.LogoutExceptionMessage));
                }

                LogService.LogError(message, webException);
                return new T { ErrorMessage = webException.Message };
            }
            else
            {
                LogService.LogError(message, exception);
                return new T { ErrorMessage = exception.Message };
            }
        }

        private static AuthenticationInfo RetrieveAuthInfo()
        {
            return ServiceContainer.Resolve<AuthenticationInfoProvider>().Info;
        }

        private static string NewBasicAuthHeader(AuthenticationInfo authInfo)
        {
            var content = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{authInfo.UserId}:{authInfo.Pin}"));
            return $"Basic {content}";
        }

        #endregion
    }
}
