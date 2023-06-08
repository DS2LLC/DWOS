using System;
using System.ComponentModel;
using NLog;

namespace DWOS.UI.Utilities
{
    internal class SecurityWatcher : IDisposable
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private Action _closeAction;
        private EventHandler<UserUpdatedEventArgs> _userUpdated;

        #endregion

        #region Methods

        public SecurityWatcher(Action closeAction)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            _closeAction = closeAction;

            _userUpdated = SecurityManager_OnUserUpdate;
            SecurityManager.Current.UserUpdated += _userUpdated;
        }

        private void SecurityManager_OnUserUpdate(object sender, UserUpdatedEventArgs userUpdatedEventArgs)
        {
            DWOSApp.MainForm.WaitForHandleCreation();

            if (DWOSApp.MainForm.InvokeRequired)
                DWOSApp.MainForm.BeginInvoke(_closeAction);
            else
                _closeAction();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (_userUpdated != null)
                {
                    SecurityManager.Current.UserUpdated -= _userUpdated;
                    _userUpdated = null;
                }

                _closeAction = null;
            }
            catch (Exception exc)
            {
                string errorMsg = "Error on security form watcher closing.";
                _log.Error(exc, errorMsg);
            }
        }

        #endregion
    }
}
