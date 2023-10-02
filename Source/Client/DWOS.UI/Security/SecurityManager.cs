using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.SecurityDataSetTableAdapters;
using DWOS.Reports;
using DWOS.Shared.Utilities;
using NLog;
using System.Collections.ObjectModel;
using DWOS.UI.Licensing;

namespace DWOS.UI.Utilities
{
    /// <summary>
    ///   Provides user security management and login capabilities.
    /// </summary>
    public class SecurityManager: ISecurityUserInfo, ISecurityManager, ILoginManager
    {
        #region Fields

        public static readonly ReadOnlyCollection<string> ReservedPins =
            new ReadOnlyCollection<string>(new string[] {"111111"});

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static readonly Lazy<SecurityManager> _securityMangerLazy = new Lazy<SecurityManager>(() =>
            new SecurityManager(DependencyContainer.Resolve<DwosServerInfoProvider>(), DependencyContainer.Resolve<IDwosApplicationSettingsProvider>()));

        private IAuthenticationProvider _authenticator;
        private SecurityDataSet _dsSecurity = new SecurityDataSet { EnforceConstraints = false };
        private LicenseManager _license;

        public event EventHandler<UserUpdatedEventArgs> UserUpdated;

        #endregion

        #region Properties

        public static SecurityManager Current =>
            _securityMangerLazy.Value;

        public Image AuthenticationProviderThumbnail =>
            _authenticator.ProviderThumbnail;

        /// <summary>
        ///   Gets the current user row.
        /// </summary>
        public SecurityDataSet.UsersRow CurrentUser { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether this instance is valid user.
        /// </summary>
        /// <value> <c>true</c> if this instance is valid user; otherwise, <c>false</c> . </value>
        public bool IsValidUser
        {
            get { return CurrentUser != null; }
        }

        private bool ImpersonateMaster
        {
            get { return Environment.GetCommandLineArgs().Any(cla => cla == "REP"); }
        }

        /// <summary>
        ///   Gets the user ID of the currently logged in user.
        /// </summary>
        /// <value> The user ID. </value>
        public int UserID => CurrentUser?.UserID ?? 0;

        /// <summary>
        ///   Gets the name of the user based on the user name in the database.
        /// </summary>
        /// <value> The name of the user. </value>
        public string UserName => CurrentUser?.Name;

        public bool RequiresOrderReview => CurrentUser?.RequireOrderReview ?? true;

        public ISecurityUserInfo UserInfo => this;

        public IDwosApplicationSettingsProvider SettingsProvider { get; }

        #endregion

        #region Methods

        private SecurityManager(DwosServerInfoProvider serverInfoProvider, IDwosApplicationSettingsProvider settingsProvider)
        {
            if (serverInfoProvider == null)
                throw new ArgumentNullException(nameof(serverInfoProvider));

            SettingsProvider = settingsProvider
                ?? throw new ArgumentNullException(nameof(settingsProvider));

            _authenticator = GetAuthenticationProvider(ApplicationSettings.Current.LoginType);

            _license = new LicenseManager(serverInfoProvider);
            _license.LicenseStatusChanged += LicenseManager_LicenseStatusChanged;
        }

        public void Initialize()
        {
            try
            {
                if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    return;

                //link reports to use this security manager
                Report.SecurityManager = this;

                ErrorReporterTarget.GetAdditionalErrorInfo = GetErrorReportInfo;
                ErrorReporterTarget.GetAdditionalUserInfo = GetUserInfo;
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Error starting up Security Manager.");
            }
        }

        public LicenseServiceClient CreateLicenseClient()
        {
            return _license.CreateLicenseClient();
        }

        private IAuthenticationProvider GetAuthenticationProvider(LoginType loginType)
        {
            IAuthenticationProvider authProvider = null;

            switch (loginType)
            {
                case LoginType.Smartcard:
                    authProvider = new SmartCardAuthenticationProvider(this);
                    break;
                case LoginType.PinOrSmartcard:
                    switch(Properties.Settings.Default.UserLogInType)
                    {
                        case LoginType.Smartcard:
                            authProvider = new SmartCardAuthenticationProvider(this);
                            break;
                        default:
                            authProvider = new UserPinAuthenticationProvider(SettingsProvider, this);
                            break;
                    }
                    break;
                case LoginType.Pin:
                    authProvider = new UserPinAuthenticationProvider(SettingsProvider, this);
                    break;
                case LoginType.PinAndSmartcard:
                    authProvider = new UserPinAndSmartCardAuthenticationProvider(SettingsProvider, this);
                    break;
            }

            return authProvider;
        }

        private Dictionary<string, string> GetErrorReportInfo()
        {
            var dict = new Dictionary<string, string>();

            if(CurrentUser != null)
            {
                dict.Add("DWOS User", CurrentUser.Name);
                if(!CurrentUser.IsEmailAddressNull())
                    dict.Add("DWOS User Email", CurrentUser.EmailAddress);
            }

            dict.Add("Company Key", ApplicationSettings.Current.CompanyKey);
            dict.Add("DWOS Product", "DWOS Client");

            return dict;
        }

        private ClientUserInfo GetUserInfo()
        {
            try
            {
                if (CurrentUser == null)
                    return null;
                else
                    return new ClientUserInfo
                    {
                        EmailAddress = CurrentUser.IsEmailAddressNull() ? null : CurrentUser.EmailAddress,
                        FullName = CurrentUser.Name,
                        UUID = ApplicationSettings.Current.CompanyKey,
                        Identifier = CurrentUser.UserID.ToString()
                    };
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Unable to get user info");
                return null;
            }
        }

        /// <summary>
        ///   Log in the user.
        /// </summary>
        public void Login()
        {
            _log.Info("Logging in.");

            //IF Hack is set to always use my permissions
            if(ImpersonateMaster)
                DoLogin(1); 
            else
            {
                if(_authenticator != null && _authenticator.LogInType != ApplicationSettings.Current.LoginType)
                {
                    _authenticator.Dispose();
                    _authenticator = null;
                }

                if (_authenticator == null)
                    _authenticator = GetAuthenticationProvider(ApplicationSettings.Current.LoginType);

                if(!_authenticator.Enabled)
                    _authenticator.Enabled = true;

                DoLogin(_authenticator.GetUserID());
            }
        }

        /// <summary>
        ///   Logs out the current user.
        /// </summary>
        public void LogOut()
        {
            _log.Info("Logging out.");

            var userChanged = CurrentUser != null;
            CurrentUser = null;
            _license.DeActivateLicense();

            OnUserChange(userChanged);
        }

        /// <summary>
        ///   Suspends the authentication mechanism.
        /// </summary>
        public void SuspendAuthentication()
        {
            if(_authenticator != null)
                _authenticator.Enabled = false;
        }

        /// <summary>
        ///   Resumes the authentication mechanism.
        /// </summary>
        public void ResumeAuthentication()
        {
            if(_authenticator != null)
                _authenticator.Enabled = true;
        }

        /// <summary>
        ///   Reload the users security roles.
        /// </summary>
        public void ReLoadUserSecurityRoles()
        {
            //re-login the current user which will force reload of all security roles
            DoLogin(UserID);
        }

        /// <summary>
        ///   Determines whether the user is in the specified role.
        /// </summary>
        /// <param name="role"> The role. </param>
        /// <returns> <c>true</c> if [is in role] [the specified role]; otherwise, <c>false</c> . </returns>
        public bool IsInRole(string role)
        {
            if(_dsSecurity != null && IsValidUser && !String.IsNullOrWhiteSpace(role))
                return _dsSecurity.User_SecurityRoles.FindByUserIDSecurityRoleID(UserID, role) != null;

            return false;
        }

        /// <summary>
        /// Determines whether the user is in the specified security group.
        /// </summary>
        /// <param name="securityGroupId">The security group id.</param>
        /// <returns><c>true</c> if [is in group] [the specified security group id]; otherwise, <c>false</c>.</returns>
        public bool IsInGroup(int securityGroupId)
        {
            if (_dsSecurity != null && IsValidUser && securityGroupId > 0)
                return _dsSecurity.User_SecurityGroup.FindBySecurityGroupIDUserID(securityGroupId, UserID) != null;

            return false;
        }

        public void DoLogin(int? userID)
        {
            var userIdChanged = false;

            try
            {
                userIdChanged = CurrentUser?.UserID != userID;
                CurrentUser = null;

                if (userID.HasValue)
                {
                    //attempt to load this user
                    using (var taUsers = new UsersTableAdapter { ClearBeforeFill = true })
                        taUsers.FillByUserID(_dsSecurity.Users, userID.Value);

                    var user = _dsSecurity.Users.FindByUserID(userID.Value);

                    //if found user then success!
                    if (user != null && user.Active)
                    {
                        //checkout license
                        if (_license.ActivateLicense(user.UserID, user.Name))
                        {
                            _log.Info("Logged on user " + user.UserID + " " + user.Name);

                            CurrentUser = user;

                            //reload this users roles
                            using (var taURoles = new User_SecurityRolesTableAdapter { ClearBeforeFill = true })
                                taURoles.FillAllByUser(_dsSecurity.User_SecurityRoles, CurrentUser.UserID);

                            using (var taUSecurityGroups = new User_SecurityGroupTableAdapter { ClearBeforeFill = true })
                                taUSecurityGroups.FillByUser(_dsSecurity.User_SecurityGroup, CurrentUser.UserID);

                            _log.Info(
                                "Loaded UserId {0} with {1} roles.",
                                CurrentUser.UserID,
                                _dsSecurity.User_SecurityRoles.Count);

                            // Load this user's groups
                            using (var taSecurityGroup = new SecurityGroupTableAdapter())
                            {
                                taSecurityGroup.FillByUser(_dsSecurity.SecurityGroup, CurrentUser.UserID);
                            }

                            // Load tabs for this user's groups
                            using (var taSecurityGroupTabs = new SecurityGroupTabTableAdapter())
                            {
                                taSecurityGroupTabs.FillByUser(_dsSecurity.SecurityGroupTab, CurrentUser.UserID);
                            }

                            UserLogging.AddLogInHistory(user.UserID);
                        }
                        else
                            MessageBoxUtilities.ShowMessageBoxWarn(
                                "Unable to obtain a valid license.",
                                "Not Licensed",
                                "Ensure there are licenses available.");
                    }
                }

            }
            catch (Exception exc)
            {
                CurrentUser = null;
                _log.Error(exc, "Error logging in user.");
            }
            finally
            {
                //Fire that a user changed
                OnUserChange(userIdChanged);
            }
        }

        private void OnUserChange(bool userIdChanged)
        {
            _log.Info("SecurityManager.OnUserChange");
            UserUpdated?.Invoke(this, new UserUpdatedEventArgs(userIdChanged));
        }

        public void Dispose()
        {
            if(_dsSecurity != null)
                _dsSecurity.Dispose();

            if(_authenticator != null)
                _authenticator.Dispose();

            if(_license != null)
                _license.Dispose();

            _authenticator = null;
            CurrentUser = null;
            _dsSecurity = null;
            _license = null;

            ErrorReporterTarget.GetAdditionalErrorInfo = null;
        }

        #endregion

        #region Events

        private void LicenseManager_LicenseStatusChanged(object sender, LicenseStatusChangedEventArgs e)
        {
            //if the license manager de-activate the license on it's own then log the user out
            switch(e.Status)
            {
                case LicenseManager.LicenseManagerStatus.ServerDeActivation:
                    MessageBoxUtilities.ShowMessageBoxWarn("Your license has been deactivated from the server.", "License DeActivated");
                    LogOut();
                    break;
                case LicenseManager.LicenseManagerStatus.NoConnectionDeActivation:
                    MessageBoxUtilities.ShowMessageBoxWarn("Your license has been deactivated due to being unable to connect to the server.", "License DeActivated");
                    LogOut();
                    break;
                case LicenseManager.LicenseManagerStatus.Activated:
                case LicenseManager.LicenseManagerStatus.DeActivate:
                default:
                    break;
            }
        }

        #endregion
    }
}