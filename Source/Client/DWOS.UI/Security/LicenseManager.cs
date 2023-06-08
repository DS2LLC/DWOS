using System;
using System.Net;
using System.ServiceModel;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.UI.Licensing;
using DWOS.UI.Properties;
using NLog;
using DWOS.Shared.Utilities;

namespace DWOS.UI.Utilities
{
    /// <summary>
    ///   The license manager helps to determine the license required for DWOS to operate.
    /// </summary>
    internal class LicenseManager: IDisposable
    {
        #region Fields

        public enum LicenseManagerStatus
        {
            Activated,
            DeActivate,
            ServerDeActivation,
            NoConnectionDeActivation
        }

        public const int KEEP_ALIVE_TIME_MINUTES = 5;
        public const int MAX_NUMBER_OFF_FAILURES = 3;

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static ServerAddressInfo _licenseServerAddress;

        private int _failedServerConnections;

        private bool _isDisposed;
        private System.Timers.Timer _timer;
        private Guid _uid;

        /// <summary>
        /// Called when the license status changes. UI Thread
        /// </summary>
        public event EventHandler<LicenseStatusChangedEventArgs> LicenseStatusChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether this license is valid.
        /// </summary>
        /// <value> <c>true</c> if this instance is valid; otherwise, <c>false</c> . </value>
        public bool? IsLicenseActivated { get; private set; }

        /// <summary>
        ///   Gets the UID of this instance of DWOS.
        /// </summary>
        /// <value> The UID. </value>
        private Guid UID
        {
            get
            {
                if(this._uid == Guid.Empty)
                    this._uid = Guid.NewGuid();

                return this._uid;
            }
        }

        public DwosServerInfoProvider ServerInfoProvider { get; }

        #endregion

        #region Methods

        public LicenseManager(DwosServerInfoProvider serverInfoProvider)
        {
            ServerInfoProvider = serverInfoProvider ?? throw new ArgumentNullException(nameof(serverInfoProvider));
            Application.ApplicationExit += this.Application_ApplicationExit;

            this._timer = new System.Timers.Timer(TimeSpan.FromMinutes(KEEP_ALIVE_TIME_MINUTES).TotalMilliseconds);
            this._timer.Elapsed += this.KeepAliveTimer_Elapsed;
        }

        public bool ActivateLicense(int userId, string userName)
        {
            _log.Info("License activation started.");

            try
            {
                this.IsLicenseActivated = false;
                
                var client              = CreateLicenseClient();
                var response            = client.CheckOutLicense(new CheckOutLicenseRequest(Environment.MachineName, userId, userName, this.UID));
                this.IsLicenseActivated = response.CheckOutLicenseResult;
                _log.Info("License activation completed with: " + this.IsLicenseActivated);

                if(this.IsLicenseActivated.GetValueOrDefault())
                {
                    this.StartKeepAliveTimer();
                    this.OnLicenseStatusChanged(LicenseManagerStatus.Activated);
                }

                client.Close();
              

                return this.IsLicenseActivated.GetValueOrDefault();
            }
            catch(Exception exc)
            {
                var isSocketException = exc.InnerException != null && exc.InnerException.Message.Contains("SocketException");

                var logLevel = isSocketException || exc is EndpointNotFoundException
                    ? LogLevel.Warn
                    : LogLevel.Error;

                _log.Log(logLevel, exc, "Error activating license.");

                return false;
            }
        }

        public void DeActivateLicense()
        {
            _log.Info("License deactivation started.");

            try
            {
                this.StopKeepAliveTimer();
                
                if(this.IsLicenseActivated.GetValueOrDefault())
                {
                    try
                    {
                        this.IsLicenseActivated = false;
                        
                        var client = CreateLicenseClient();
                        client.CheckInLicense(new CheckInLicenseRequest(this.UID));
                        client.Close();
                    }
                    catch(Exception exc)
                    {
                        var logLevel = exc is EndpointNotFoundException || exc is CommunicationException
                            ? LogLevel.Warn
                            : LogLevel.Error;

                        _log.Log(logLevel, exc, "Error checking in license.");
                    }
                    
                    this.OnLicenseStatusChanged(LicenseManagerStatus.DeActivate);
                }
                 
                _log.Info("License deactivation completed.");
            }
            catch(Exception exc)
            {
                _log.Debug(exc, "Error deactivating license.");
            }
        }

        private void KeepAlive()
        {
            try
            {
                if(this.IsLicenseActivated.GetValueOrDefault())
                {
                    var client      = CreateLicenseClient();
                    var response    = client.KeepLicenseAlive(new KeepLicenseAliveRequest(this.UID));

                    //if server says I am NOT still connected
                    if(!response.KeepLicenseAliveResult)
                    {
                        _log.Info("License Server revoked license, deactivating license.");
                        this.IsLicenseActivated = false;
                        this.OnLicenseStatusChanged(LicenseManagerStatus.ServerDeActivation);
                    }

                    this._failedServerConnections = 0;
                    client.Close();
                }
            }
            catch(Exception exc)
            {
                this.FailedActivation(exc);
            }
        }

        private void StartKeepAliveTimer()
        {
            this._timer.Enabled = true;
        }

        private void StopKeepAliveTimer()
        {
            this._timer.Enabled = false;
        }

        private void FailedActivation(Exception activationError)
        {
            try
            {
                this._failedServerConnections++;

                //if hasn't been able to communicate with server then de-activate license
                if(this._failedServerConnections > MAX_NUMBER_OFF_FAILURES)
                {
                    var logLevel = activationError is EndpointNotFoundException
                        ? LogLevel.Warn
                        : LogLevel.Error;

                    _log.Log(logLevel, activationError, $"Exceeded max number of failed activations {_failedServerConnections}, deactivating license.");
                    this._failedServerConnections = 0;

                    DeActivateLicense();
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error evaluating grace period.");
            }
        }

        private void OnLicenseStatusChanged(LicenseManagerStatus status)
        {
            if (DWOSApp.MainForm == null)
                return;

            if (DWOSApp.MainForm.InvokeRequired)
            {
                DWOSApp.MainForm.BeginInvoke(new Action<LicenseManagerStatus>(OnLicenseStatusChanged), status);
            }
            else
            {
                if(this.LicenseStatusChanged != null)
                    this.LicenseStatusChanged(this, new LicenseStatusChangedEventArgs{Status = status});
            }
        }

        public LicenseServiceClient CreateLicenseClient()
        {
            var connInfo = ServerInfoProvider.ConnectionInfo ??
                DwosServerInfo.Default;

            try
            {
                //Resolve host name to IP Address
                if (_licenseServerAddress == null || _licenseServerAddress.ServerName != connInfo.ServerAddress)
                {
                    IPAddress ipAddressIPv4 = null;

                    if (!IPAddress.TryParse(connInfo.ServerAddress, out ipAddressIPv4))
                    {
                        var hostInfo = Dns.GetHostEntry(connInfo.ServerAddress);

                        if (hostInfo.AddressList.Length > 0)
                        {
                            //find the IPv4 address if multiple addresses exist
                            ipAddressIPv4 = hostInfo.AddressList.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                        }
                    }

                    if (ipAddressIPv4 == null)
                    {
                        _licenseServerAddress = null;
                    }
                    else
                    {
                        _licenseServerAddress = new ServerAddressInfo() { ServerName = connInfo.ServerAddress, ServerIP = ipAddressIPv4 };
                    }
                }
            }
            catch
            {
                _licenseServerAddress = null;
            }

            var correctedServerInfo = new DwosServerInfo()
            {
                ServerAddress = _licenseServerAddress == null ? connInfo.ServerAddress : _licenseServerAddress.ServerIP.ToString(),
                ServerPort = connInfo.ServerPort
            };

            return new LicenseServiceClient(new NetTcpBinding(SecurityMode.None, false), new EndpointAddress(correctedServerInfo.ServerAddressUri));
        }

        #endregion

        #region Events

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            try
            {
                this.DeActivateLicense();

                //ensure disposed on exit
                this.Dispose();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on exit of licensing module.");
            }
        }

        private void KeepAliveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.KeepAlive();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if(!this._isDisposed)
            {
                this._isDisposed = true;
               
                if(this._timer != null)
                {
                    this._timer.Dispose();
                    this._timer = null;
                }
            }
        }

        #endregion

        #region ServerAddressInfo
        
        private class ServerAddressInfo
        {
            public string ServerName { get; set; }
            public IPAddress ServerIP { get; set; }
        }

        #endregion
    }

    internal class LicenseStatusChangedEventArgs: EventArgs
    {
        public LicenseManager.LicenseManagerStatus Status { get; set; }
    }
}