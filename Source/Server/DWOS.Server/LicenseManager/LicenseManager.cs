using System;
using System.Linq;
using DWOS.Data;
using DWOS.Server.Utilities;
using NLog;

namespace DWOS.LicenseManager
{
    public class LicenseManager
    {
        #region Fields

        private const int GRACE_DAYS    = 10;
        private const int GRACE_HOURS   = 12;

        private static readonly Logger _log                 = LogManager.GetCurrentClassLogger();

        private static Lazy<LicenseManager> _licenseManagerInstance = new Lazy<LicenseManager>(
            () => new LicenseManager());

        private readonly object _activationSyncObject       = new object();
        private readonly DateTime _startUpTimeStamp;

        private readonly IUserStorageProvider _users =
            UserStorageProviderFactory.NewInstance();

        private LicenseFile _currentLicense;
        private LicenseUsageStats _usageStats = new LicenseUsageStats();

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the default instance of license manager.
        /// </summary>
        /// <value> The default. </value>
        public static LicenseManager Default
        {
            get
            {
                return _licenseManagerInstance.Value;
            }
        }

        /// <summary>
        ///   Gets or sets the current key.
        /// </summary>
        /// <value> The current key. </value>
        public LicenseFile CurrentLicense
        {
            private get { return this._currentLicense; }
            set
            {
                var valueAsString = value?.ToString() ?? "Empty";
                _log.Info("Setting LicenseFile on License Manager: " + valueAsString);

                this._currentLicense = value;
                this.ValidateLicense();
                this.SaveLicenseToStorage();
            }
        }

        /// <summary>
        ///   Gets the avaliable activations.
        /// </summary>
        /// <value> The avaliable activations. </value>
        private int AvaliableActivations
        {
            get
            {
                if(this.CurrentLicense == null)
                    return 0;

                return this.CurrentLicense.Activations - this._users.Count;
            }
        }

        public bool IsInGracePeriod { get; private set; }

        public bool IsLicenseValid { get; private set; }

        internal LicenseUsageStats UsageStats
        {
            get { return _usageStats; }
        }

        #endregion

        #region Methods

        private LicenseManager()
        {
            _log.Info("License Manager initialized.");

            this.LoadLicenseFromStorage();
            this._startUpTimeStamp  = DateTime.Now;
        }

        /// <summary>
        /// Activates the specified user.
        /// </summary>
        /// <param name="computerName">Name of the computer.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="uid">The unique id of the user.</param>
        /// <returns></returns>
        internal bool CheckOutLicense(string computerName, int userId, string userName, Guid uid)
        {
            lock (this._activationSyncObject)
            {
                if(this.IsInGracePeriod || this.IsLicenseValid)
                {
                    var user = this._users.Get(uid);

                    if(user == null)
                    {
                        //if we can activate another user then activate
                        if(this.AvaliableActivations > 0)
                        {
                            user = new UserActivation{UserId = userId, Activated = DateTime.Now, LastCommunication = DateTime.Now, UID = uid, ComputerName = computerName, UserName = userName};
                            _log.Info("User {0} activated.", user);
                            Data.Datasets.UserLogging.AddActivationHistory(user.UserId, "Activate", computerName, uid.ToString());
                            this._users.Add(user);
                            
                            _usageStats.TotalActivations += 1;
                            _usageStats.AddActivation();
                        }
                        else
                            _usageStats.TotalOutOfLicense += 1;
                    }
                    else
                    {
                        //Else just update this users timestamps
                        user.LastCommunication  = DateTime.Now;
                        user.Activated          = DateTime.Now;
                    }

                    return user != null;
                }
                else
                {
                    _log.Info("User {0} NOT activated, Server License is NOT valid.", userName);
                    return false;
                }
            }
        }

        /// <summary>
        ///   Deactivate the user.
        /// </summary>
        /// <param name="uid"> The uid. </param>
        /// <exception cref="System.NotImplementedException"></exception>
        internal void CheckInLicense(Guid uid)
        {
            lock (this._activationSyncObject)
            {
                var user = this._users.Get(uid);
                if(user != null)
                {
                    this._users.Remove(user);
                    _log.Info("User {0} DE-activated.", user);
                    Data.Datasets.UserLogging.AddActivationHistory(user.UserId, "Deactivate", user.ComputerName, uid.ToString());
                    _usageStats.TotalDeActivations += 1;
                }
            }
        }

        /// <summary>
        ///   Keeps the user activation alive.
        /// </summary>
        /// <param name="uid"> The uid. </param>
        /// <returns> If true, then user is still active. </returns>
        internal bool KeepAlive(Guid uid)
        {
            lock (this._activationSyncObject)
            {
                var user = this._users.Get(uid);
                
                if(user != null)
                {
                    _log.Debug("User {0} KeepAlive.", user);
                    user.LastCommunication = DateTime.Now;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///   Periodically tell license manager to ensure that the license has not expired.
        /// </summary>
        internal void ValidateLicense()
        {
            try
            {
                _log.Info("Validating license.");

                this.IsInGracePeriod = false;

                if(this.CurrentLicense != null)
                {
                    //if license has expired
                    if(DateTime.Today > this.CurrentLicense.LicenseExpiration)
                    {
                        this.IsLicenseValid = false;

                        //if license is within grace period
                        if(DateTime.Today.Subtract(this.CurrentLicense.LicenseExpiration).TotalDays <= GRACE_DAYS)
                            this.IsInGracePeriod = true;

                        if(this.IsInGracePeriod)
                        {
                            var remainingDays = GRACE_DAYS - DateTime.Today.Subtract(this.CurrentLicense.LicenseExpiration).TotalDays;
                            SendEmailToAdministrator("The DWOS server license is expired. Please update the license before the server is disabled in " + remainingDays + " days.");
                        }
                        else
                            SendEmailToAdministrator("The DWOS server license is expired and you are past your grace period. DWOS will no longer function until this license is renewed.");
                    }
                    else
                    {
                        this.IsLicenseValid = true;

                        if (this.CurrentLicense.LicenseExpiration.Subtract(DateTime.Today).TotalDays <= GRACE_DAYS)
                        {
                            var remainingDays = this.CurrentLicense.LicenseExpiration.Subtract(DateTime.Today).TotalDays;
                            SendEmailToAdministrator("The DWOS server license will expire in " + remainingDays + " days. Please update the license before the server is disabled.");
                        }
                    }
                }
                else if (DateTime.Today.Subtract(this._startUpTimeStamp).TotalDays <= GRACE_HOURS)
                {
                    //if not contacted home then put in grace mode
                    IsInGracePeriod = true;
                }
            }
            catch(Exception exc)
            {
                this.IsLicenseValid = false;
                this.IsInGracePeriod = false;
                _log.Error(exc, "Error validating license.");
            }
        }

        /// <summary>
        ///   Prunes any inactive users.
        /// </summary>
        internal void PruneInactiveUsers()
        {
            try
            {
                lock (this._activationSyncObject)
                {
                    var timeOut      = TimeSpan.FromMinutes(ServerSettings.Default.InactivityTimeout);

                    var inactiveUsers = this._users.CurrentUsers
                        .Where(ua => DateTime.Now.Subtract(ua.LastCommunication) > timeOut)
                        .ToList();
                    
                    foreach (var user in inactiveUsers)
                    {
                        this._users.Remove(user);
                        Data.Datasets.UserLogging.AddActivationHistory(user.UserId, "Pruned", user.ComputerName, user.UID.ToString());
                        _usageStats.TotalPrunes += 1;
                        _log.Info("Pruning user: {0}", user);
                    };

                    if(inactiveUsers.Count > 0)
                        _log.Info("Pruning de-activated {0} users due to inactivity.", inactiveUsers.Count);
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error during prune operation.");
            }
        }

        internal LicenseSummary GetLicenseSummary()
        {
            lock (this._activationSyncObject)
            {
                var lStatus = new LicenseSummary();
                lStatus.CurrentActivations = this._users.CurrentUsers;

                if (this.CurrentLicense != null)
                {
                    lStatus.TotalActivations = this.CurrentLicense.Activations;
                    lStatus.AvaliableActivations = lStatus.CurrentActivations.Count - this.CurrentLicense.Activations;
                }

                return lStatus;
            }
        }

        private void LoadLicenseFromStorage()
        {
            try
            {
                //Load License File
                var protectedLicense = ServerSettings.Default.GetEncryptedLicense();
                var license = LicenseFile.UnProtect(protectedLicense);

                if (license == null)
                    license = new LicenseFile { Activations = 3, LicenseExpiration = DateTime.Now.AddHours(4) };
                else
                    _log.Info("Retrieved license from secure storage: {0}", license.ToString());

                this._currentLicense = license;
                this.ValidateLicense();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error reading license from storage.");
            }
        }

        private void SaveLicenseToStorage()
        {
            try
            {
                var protectedLicense = LicenseFile.Protect(this._currentLicense);
                ServerSettings.Default.SetEncryptedLicense(protectedLicense);
                _log.Info("Saved license to secure storage.");
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error saving license to storage.");
            }
        }

        private void SendEmailToAdministrator(string message)
        {
            //Notify Admin
            var adminEmail = ServerSettings.Default.AdminEmail;

            if(!String.IsNullOrEmpty(adminEmail))
            {
                if(this.CurrentLicense != null)
                {
                    message += Environment.NewLine + Environment.NewLine;
                    message += "Company: " + this.CurrentLicense.CompanyName + Environment.NewLine;
                    message += "Expiration: " + this.CurrentLicense.LicenseExpiration.ToShortDateString() + Environment.NewLine;
                    message += "Licenses: " + this.CurrentLicense.Activations + Environment.NewLine;
                    message += "Server: " + Environment.MachineName + Environment.NewLine;
                    message += "Company Key: " + ServerSettings.Default.CompanyKey + Environment.NewLine;
                    message += "Fingerprint: " + ServerSettings.Default.Fingerprint + Environment.NewLine;
                    message += "Version: " + ServerSettings.Default.Version + Environment.NewLine;
                }

                MessagingUtilities.QuickSendEmail(adminEmail, "DWOS - License Issue", message);
            }

        }

        #endregion
    }
}