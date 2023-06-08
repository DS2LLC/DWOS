using DWOS.Data;
using NLog;
using System.Drawing;

namespace DWOS.UI.Utilities
{
    internal class UserPinAndSmartCardAuthenticationProvider : IAuthenticationProvider
    {
        #region Fields

        private SmartCardAuthenticationProvider _smartCardAuth;
        private UserPinAuthenticationProvider _userPinAuth;
        private bool _enabled;
        private readonly ILoginManager _loginManager;

        #endregion

        #region Methods

        public UserPinAndSmartCardAuthenticationProvider(
            IDwosApplicationSettingsProvider settingsProvider,
            ILoginManager loginManager)
        {
            _loginManager = loginManager;
            _smartCardAuth = new SmartCardAuthenticationProvider(_loginManager);
            _userPinAuth = new UserPinAuthenticationProvider(settingsProvider, _loginManager);
        }

        #endregion

        #region IAuthenticationProvider Members

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                }
            }
        }

        public Image ProviderThumbnail
        {
            get { return Properties.Resources.duel_login; }
        }

        public int? GetUserID()
        {
            _userPinAuth.Enabled = Enabled;
            var pinUserID = _userPinAuth.GetUserID();

            _smartCardAuth.Enabled = Enabled;
            var smartCardUserID = _smartCardAuth.GetUserID();

            if (smartCardUserID == null || pinUserID == null)
                return null;
            else if (smartCardUserID.Equals(pinUserID))
                return smartCardUserID;
            else
                return null;
        }

        public void Dispose()
        {
            LogManager.GetCurrentClassLogger()
                .Info("Disposing of UserPinAndSmartCardAuthenticationProvider");
            _smartCardAuth.Dispose();
            _userPinAuth.Dispose();
        }

        public LoginType LogInType { get { return LoginType.PinAndSmartcard; } }

        #endregion
    }
}
