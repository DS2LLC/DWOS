using DWOS.Services.Messages;
using DWOS.Utilities;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.ViewModels
{
    /// <summary>
    /// View Model for log in functionality
    /// </summary>
    public class LogInViewModel : ViewModelBase, ILicenseSessionService, IDisposable
    {
        #region Fields

        UserProfileInfo _userProfile;
        bool _isLoggedIn;
        string _userPin;
        string _serverUrl;
        long _keepAliveTicks = 0;
        Guid _licenseUid;
        bool _isLicenseActivated;
        int _failedLicenseServerConnections;
        const int _maxNumberOfFailures = 3;
        const int _keepAliveThresholdMilliseconds = 600000; // 10 minutes;

        Dictionary<int, UserInfo> _userCache = new Dictionary<int,UserInfo>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current user's profile.
        /// </summary>
        /// <value>
        /// The user profile.
        /// </value>
        public UserProfileInfo UserProfile
        {
            get { return _userProfile; }

            private set
            {
                _userProfile = value;
                OnPropertyChanged("UserProfile");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current user is logged in.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user is logged in; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }

            private set
            {
                _isLoggedIn = value;
                OnPropertyChanged("IsLoggedIn");
            }
        }

        /// <summary>
        /// Gets or sets the user pin.
        /// </summary>
        /// <value>
        /// The user pin.
        /// </value>
        public string UserPin
        {
            get { return _userPin; }
            set
            {
                _userPin = value;
                Validate();
                OnPropertyChanged("UserPin");
            }
        }

        /// <summary>
        /// Gets or sets a well-formed server URL for the API.
        /// </summary>
        /// <value>
        /// The server URL.
        /// </value>
        public string ServerUrlWellFormed { get; set; }

        /// <summary>
        /// Gets or sets the server URL.
        /// </summary>
        /// <value>
        /// The server URL.
        /// </value>
        public string ServerUrl
        {
            get { return _serverUrl; }
            set
            {
                if (_serverUrl != value)
                {
                    _serverUrl = value;
                    Validate();
                    OnPropertyChanged("ServerUrl");
                }
            }
        }

        /// <summary>
        /// Gets the GUID of this instance of DWOS.
        /// </summary>
        /// <value> The GUID. </value>
        private Guid LicenseUID
        {
            get
            {
                if (this._licenseUid == Guid.Empty)
                    this._licenseUid = Guid.NewGuid();

                return this._licenseUid;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this license is valid.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsLicenseActivated 
        {
            get { return _isLicenseActivated; }
            private set
            {
                if (_isLicenseActivated != value)
                {
                    _isLicenseActivated = value;
                    OnPropertyChanged("IsLicenseActivated");
                }
            }
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LogInViewModel"/> class.
        /// </summary>
        public LogInViewModel()
        {
            ServiceContainer.Register<ILicenseSessionService>(this);
            Messenger.Default.Register<NotificationMessage>(this, message => 
            {
                if (message.Notification == MessageNames.LogoutExceptionMessage)
                    LogoutFromConnectionException();
            });
        }

        /// <summary>
        /// Protected method for validating the ViewModel
        /// - Fires PropertyChanged for IsValid and Errors
        /// </summary>
        protected override void Validate()
        {
            ValidateProperty(() => string.IsNullOrEmpty(UserPin), "Please enter a Pin.");
            ValidateProperty(() => string.IsNullOrEmpty(ServerUrl), "Please enter a Server URL.");
            base.Validate();
        }

        /// <summary>
        /// Performs asynchronous login for the current user.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="ViewModelResult"/> with the succes of the login and possible error message.</returns>
        public async Task<ViewModelResult> LoginAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ViewModelResult result = null;
            IsBusy = true;
            ServerUrlWellFormed = CreateWellFormedUri(ServerUrl);
            var logInService = ServiceContainer.Resolve<ILoginService>();
            logInService.RootUrl = ServerUrlWellFormed;

            if (string.IsNullOrEmpty(UserPin))
                return new ViewModelResult(success: false, errorMessage: "Pin cannot be empty");

            var serverResponseOk = await logInService.VerifyServer();
            if (serverResponseOk)
            {
                var logInResponse = await logInService.LogInUserAsync(UserPin, cancellationToken);

                if (logInResponse.Success == true && logInResponse.UserProfile != null)
                {
                    IsLoggedIn = true;
                    IsLicenseActivated = true;
                    UserProfile = logInResponse.UserProfile;
                    await LoadServerAppSettings();

                    if (IsVersionOk())
                    {
                        ActivateLicense();
                        result = new ViewModelResult(IsLoggedIn, logInResponse.ErrorMessage);
                    }
                    else
                    {
                        var versionService = ServiceContainer.Resolve<IVersionService>();

                        IsLoggedIn = false;
                        UserProfile = null;
                        var message =
                            $"The App Version: {versionService.GetAppVersion()} is not compatible with Server Version: {ApplicationSettings.Settings.ServerVersion}";

                        result = new ViewModelResult(IsLoggedIn, errorMessage: message);
                    }
                }
                else
                {
                    IsLoggedIn = false;
                    UserProfile = null;
                    var message = string.IsNullOrEmpty(logInResponse.ErrorMessage) ? 
                        "There was a problem with the log in credentials." : logInResponse.ErrorMessage;
                    result = new ViewModelResult(IsLoggedIn, message);
                }
            }
            else
            {
                var showAddressWarning = !Uri.TryCreate(ServerUrlWellFormed, UriKind.Absolute, out var serverUri)
                    || (serverUri.HostNameType != UriHostNameType.IPv4 && serverUri.HostNameType != UriHostNameType.IPv6);

                var message = showAddressWarning
                    ? "Could not find server.\nTry entering the DWOS server's IP address instead of its name."
                    : $"The server at \"{ServerUrlWellFormed}\" is not responding.";

                result = new ViewModelResult(success: false, errorMessage: message);
            }

            IsBusy = false;
            return result;

            bool IsVersionOk()
            {
                // 18.2.0.0 introduced API versioning
                var minimumServerVersion = new Version(18, 2, 0);
                var serverVersion = new Version(ApplicationSettings.Settings.ServerVersion);
                var serverApiVersion = ApplicationSettings.Settings.ServerApiVersion;

                return minimumServerVersion <= serverVersion
                    ? serverApiVersion <= ApplicationSettingsInfo.CURRENT_API_VERSION
                    : false;
            }
        }
       
        /// <summary>
        /// Logout from Server.
        /// </summary>
        /// <remarks>
        /// Invalidates <see cref="UserProfile"/>.
        /// </remarks>
        public void Logout(bool navigateToLogin = true)
        {
            IsBusy = true;
            IsLoggedIn = false; 
            DeactivateLicense();
            UserProfile = null;

            RequestInvalidateViewModel();
            
            if (navigateToLogin)
            {
                var viewService = ServiceContainer.Resolve<IViewNavigationService>();
                if (viewService != null)
                    viewService.NavigateToView(ViewName.Login);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Performs logout after a connection exception has occurred.
        /// </summary>
        public void LogoutFromConnectionException()
        {
            if (IsLoggedIn)
            {
                IsBusy = true;
                IsLoggedIn = false;
                DeactivateLicense();
                //UserProfile = null;
                var viewService = ServiceContainer.Resolve<IViewNavigationService>();
                if (viewService != null)
                    viewService.NavigateToView(ViewName.Login);
                IsBusy = false;
            }
        }

        private async Task<bool> LoadServerAppSettings(CancellationToken cancellationToken = default(CancellationToken))
        {
            var logInService = ServiceContainer.Resolve<ILoginService>();
            logInService.RootUrl = ServerUrlWellFormed;
            var response = await logInService.GetApplicationSettings(cancellationToken);
            if (response.Success && string.IsNullOrEmpty(response.ErrorMessage))
                ApplicationSettings.Settings = response.ApplicationSettings;
            
            return response.Success;
        }

        /// <summary>
        /// Retrieves user info asynchronously.
        /// </summary>
        /// <remarks>Optimized to cache a user profile id already retrieved</remarks>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="ViewModelResult"/> with the succes of the login and possible error message.</returns>
        public async Task<GetUserResult> GetUserAsync(int userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            IsBusy = true;
            UserInfo userInfo;
            if (_userCache.TryGetValue(userId, out userInfo))
            {
                IsBusy = false;
                return new GetUserResult(userInfo, success:true, errorMessage:string.Empty);
            }
            else
            {
                var logInService = ServiceContainer.Resolve<ILoginService>();
                logInService.RootUrl = ServerUrlWellFormed;//TODO: Park in settings?
                var request = new UserRequest
                {
                    UserId = UserProfile.UserId,
                    RequestedUserId = userId
                };
                var logInResponse = await logInService.GetUserAsync(request, cancellationToken);
                _userCache[userId] = logInResponse.User;
                IsBusy = false;
                return new GetUserResult(logInResponse.User, logInResponse.Success, logInResponse.ErrorMessage); 
            }
        }

        /// <summary>
        /// Signals to keep the local session alive.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void KeepAlive()
        {
            Interlocked.Exchange(ref _keepAliveTicks, DateTime.Now.Ticks);
        }

        /// <summary>
        /// Signals to end the local session.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Stop()
        {
            Interlocked.Exchange(ref _keepAliveTicks, DateTime.MinValue.Ticks);
        }

        /// <summary>
        /// Activates a license on the DWOS Server.
        /// </summary>
        private void ActivateLicense()
        {
            try
            {
                var licenseClient = CreateLicenseServiceClient(ServerUrlWellFormed);
                licenseClient.CheckOutLicenseCompleted += LicenseClient_OnCheckOutLicenseCompleted;
                licenseClient.CheckOutLicenseAsync("ComputerName", UserProfile.UserId, UserProfile.Name, LicenseUID);
            }
            catch (Exception exception)
            {
                var message = string.Format("Error in {0}", "LogInViewModel.ActivateLicense");
                LogError(message, exception);
            }
        }

        /// <summary>
        /// Deactivates a license on the DWOS Server.
        /// </summary>
        private void DeactivateLicense()
        {
            try
            {
                if (IsLicenseActivated)
                {
                    IsBusy = true;
                    IsLicenseActivated = false;
                    var licenseClient = CreateLicenseServiceClient(ServerUrlWellFormed);
                    licenseClient.CheckInLicenseCompleted += LicenseClient_OnCheckInLicenseCompleted;
                    licenseClient.CheckInLicenseAsync(LicenseUID);
                    IsBusy = false;
                }
            }
            catch (Exception exception)
            {
                var message = string.Format("Error in {0}", "LogInViewModel.DeactivateLicense");
                LogError(message, exception);
            }
        }

        /// <summary>
        /// Keeps a license alive on the DWOS Server.
        /// </summary>
        private void KeepLicenseAlive()
        {
            try
            {
                if (IsLicenseActivated == true && LicenseUID != null)
                {
                    var licenseClient = CreateLicenseServiceClient(ServerUrlWellFormed);
                    licenseClient.KeepLicenseAliveCompleted += LicenseClient_OnKeepLicenseAliveCompleted;
                    licenseClient.KeepLicenseAliveAsync(LicenseUID);
                }
            }
            catch (Exception exception)
            {
                FailedActivation(exception);
                var message = string.Format("Error in {0}", "LogInViewModel.KeepLicenseAlive");
                LogError(message, exception);
            }
        }

        public void CheckLicense()
        {
            if (!_isLicenseActivated)
            {
                return;
            }

            var currentTicks = DateTime.Now.Ticks;
            var keepAliveTicks = Interlocked.Read(ref _keepAliveTicks);
            var tickDifference = currentTicks - keepAliveTicks;
            var threshold = TimeSpan.TicksPerMillisecond * _keepAliveThresholdMilliseconds;

            if (keepAliveTicks > 0 && tickDifference > threshold)
            {
                DeactivateLicense();
            }
            else
            {
                KeepLicenseAlive();
            }
        }

        private static LicenseServiceClient CreateLicenseServiceClient(string serverUrl)
        {
            var binding = CreateBinding();
            var endpoint = CreateEndpoint(serverUrl);
            var licenseService = new LicenseServiceClient(binding, endpoint);
            return licenseService;
        }

        private static BasicHttpBinding CreateBinding()
        {
            var timeout = new TimeSpan(0, 0, 30);
            var binding = new BasicHttpBinding
            {
                Name = "httpBinding",
                MaxBufferSize = Int32.MaxValue,
                MaxReceivedMessageSize = Int32.MaxValue,
                SendTimeout = timeout,
                OpenTimeout = timeout,
                ReceiveTimeout = timeout
            };

            return binding;
        }

        private static EndpointAddress CreateEndpoint(string serverUrl)
        {
            if (string.IsNullOrEmpty(serverUrl))
                throw new InvalidOperationException("serverUrl cannot be null");

            var uri = new Uri(serverUrl);
            var split = new string[] { ":" };
            var rootUrl = string.Format("{0}:", uri.Authority.Split(split, StringSplitOptions.RemoveEmptyEntries)[0]);
            var address = string.Format("{0}://{1}{2}", uri.Scheme, rootUrl, ApplicationSettings.Settings.LicenseServerUriPath);
            var endpoint = new EndpointAddress(address);
            return endpoint;
        }

        private void FailedActivation(Exception activationError)
        {
            try
            {
                _failedLicenseServerConnections++;

                //if hasn't been able to communicate with server then de-activate license
                if (_failedLicenseServerConnections > _maxNumberOfFailures)
                {
                    _failedLicenseServerConnections = 0;
                    DeactivateLicense();
                }
            }
            catch (Exception exception)
            {
                var message = string.Format("Error in {0}", "LogInViewModel.FailedActivation");
                LogError(message, exception);
            }
        }



        private string CreateWellFormedUri(string userEnteredUri)
        {
            var uriValue = userEnteredUri;

            try
            {
                var builder = new UriBuilder(userEnteredUri);
                builder.Scheme = builder.Scheme != "http" ? "http" : builder.Scheme;
                builder.Port = builder.Port != ApplicationSettings.Settings.ServerPort ? 
                    ApplicationSettings.Settings.ServerPort : builder.Port;
                builder.Path = builder.Path.Contains(ApplicationSettings.Settings.ServerUriPath) ?
                    builder.Path : ApplicationSettings.Settings.ServerUriPath;

                uriValue = builder.Uri.ToString().Trim();
            }
            catch (FormatException)
            {
                // Invalid URI - do nothing
            }
            catch (Exception exception)
            {
                var message = string.Format("Error in {0} userEnteredUri {1}", "LogInViewModel.CreateWellFormedUri", userEnteredUri);
                LogError(message, exception);
            }

            return uriValue;
        }

        #endregion

        #region Events

        private void LicenseClient_OnKeepLicenseAliveCompleted(object sender, KeepLicenseAliveCompletedEventArgs completedArgs)
        {
            try
            {
                if (completedArgs.Result)
                {
                    KeepAlive();
                }
                else
                {
                    IsLicenseActivated = false;
                }

                var licenseClient = (LicenseServiceClient)sender;
                licenseClient.CloseAsync();
            }
            catch (Exception exception)
            {
                var message = string.Format("Error in {0}", "LogInViewModel.LicenseClient_OnKeepLicenseAliveCompleted");
                LogError(message, exception);
            }
        }

        private void LicenseClient_OnCheckInLicenseCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs completedArgs)
        {
            try
            {
                var licenseClient = (LicenseServiceClient)sender;
                licenseClient.CloseAsync();
            }
            catch (Exception exception)
            {
                var message = string.Format("Error in {0}", "LogInViewModel.LicenseClient_OnCheckInLicenseCompleted");
                LogError(message, exception);
            }
        }

        private void LicenseClient_OnCheckOutLicenseCompleted(object sender, CheckOutLicenseCompletedEventArgs completedArgs)
        {
            var isProperlyLicensed = false;

            try
            {
                IsLicenseActivated = completedArgs.Result;
                if (IsLicenseActivated == true)
                {
                    isProperlyLicensed = true;
                }

                var licenseClient = (LicenseServiceClient)sender;
                licenseClient.CloseAsync();
            }
            catch (Exception exception)
            {
                var message = string.Format("Error in {0}", "LogInViewModel.LicenseClient_OnCheckOutLicenseCompleted");
                LogError(message, exception);
            }
            finally
            {
                if (!isProperlyLicensed)
                {
                    var dialogService = ServiceContainer.Resolve<IDialogService>();
                    string message = "Unable to obtain a valid license." + Environment.NewLine + "Ensure there are licenses available.";
                    dialogService.Show("Not Licensed", message);

                    LogError(message, completedArgs.Error);
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Logout(navigateToLogin: false);
        }

        #endregion
    }
}
