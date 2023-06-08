using System;
using System.ComponentModel;
using System.Windows.Forms;
using Infragistics.Win.Misc;
using NLog;

namespace DWOS.UI.Utilities
{

    /// <summary>
    ///   Watches for changes in user login state. If user logs out then will automatically close the form.
    /// </summary>
    internal class SecurityFormWatcher : IDisposable
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        public UltraButton CloseButton;
        public Form FormToClose;
        private EventHandler<UserUpdatedEventArgs> _userUpdated;

        #endregion

        #region Methods

        public SecurityFormWatcher(Form formToClose, UltraButton closeButton)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            FormToClose = formToClose;
            CloseButton = closeButton;

            FormToClose.FormClosed += FormToClose_FormClosed;

            _userUpdated = SecurityManager_OnUserUpdated;
            SecurityManager.Current.UserUpdated += _userUpdated;
        }

        private void FormToClose_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
        }

        private void SecurityManager_OnUserUpdated(object sender, UserUpdatedEventArgs e)
        {
            FormToClose.WaitForHandleCreation();

            if (FormToClose.InvokeRequired)
                FormToClose.BeginInvoke(new Action(CloseButton.PerformClick));
            else
                CloseButton.PerformClick();
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

                if (FormToClose != null)
                    FormToClose.FormClosed -= FormToClose_FormClosed;

                CloseButton = null;
                FormToClose = null;
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
