using System;
using System.Drawing;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets.SecurityDataSetTableAdapters;
using Microsoft.Win32;
using NLog;
using System.Windows.Interop;

namespace DWOS.UI.Utilities
{
    internal class UserPinAuthenticationProvider : IAuthenticationProvider
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ILoginManager _loginManager;

        #endregion

        #region Properties

        public IDwosApplicationSettingsProvider SettingsProvider { get; }

        #endregion

        #region Methods

        public UserPinAuthenticationProvider(IDwosApplicationSettingsProvider settingsProvider,
            ILoginManager loginManager)
        {
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
            SettingsProvider = settingsProvider;
            _loginManager = loginManager;
        }

        #endregion

        #region Events

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            try
            {
                if (e.Reason == SessionSwitchReason.SessionLock)
                {
                    //if locking the desktop then log out the user
                    if (this.Enabled)
                    {
                        DWOSApp.MainForm.WaitForHandleCreation();

                        if (DWOSApp.MainForm.InvokeRequired)
                        {
                            DWOSApp.MainForm.BeginInvoke(new Action(_loginManager.LogOut));
                        }
                        else
                        {
                            _loginManager.LogOut();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error logging application out on windows desktop lock.");
            }
        }

        #endregion


        #region IAuthenticationProvider Members

        public Image ProviderThumbnail
        {
            get { return Properties.Resources.Asterik; }
        }

        public bool Enabled { get; set; }

        public int? GetUserID()
        {
            bool continueTrying = true;

            try
            {
                _log.Info("Getting user login pin...");
                var settings = SettingsProvider.Settings;

                while (continueTrying)
                {
                    using (var frm = new LogIn())
                    {
                        if (frm.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                        {
                            var userLogInPin = frm.UserPin;

                            if (!String.IsNullOrEmpty(userLogInPin))
                            {
                                userLogInPin = userLogInPin.Trim().ExtractDigits();
                                _log.Info("UserPinAuthenticationProvider found user logon id");

                                using (var ta = new UsersTableAdapter())
                                {
                                    var id = ta.GetUserIdByUserLoginPin(userLogInPin);

                                    if (id.HasValue)
                                        return id;
                                }
                            }

                            _log.Info("Login failed.");
                            Data.Datasets.UserLogging.AddFailedLogInHistory();

                            // Show 'contact us' prompt if this is the demo server.
                            if (settings.IsDemoServer)
                            {
                                var demoNotificationWindow = new DemoServerPrompt();
                                var helper = new WindowInteropHelper(demoNotificationWindow) { Owner = DWOSApp.MainForm.Handle };
                                demoNotificationWindow.ShowDialog();
                                continueTrying = false;
                            }
                        }
                        else
                            continueTrying = false;
                    }
                }

                return null;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error get current user id.");
                return null;
            }
        }

        public void Dispose()
        {
            _log.Info("Disposing of UserPinAuthenticationProvider");
            SystemEvents.SessionSwitch -= new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
        }

        public LoginType LogInType { get { return LoginType.Pin; } }

        #endregion
    }
}
