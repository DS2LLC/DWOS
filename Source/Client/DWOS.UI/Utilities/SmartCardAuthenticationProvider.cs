using System;
using System.Drawing;
using BasicCard.Framework;
using BasicCard.Terminals;
using DWOS.Data;
using DWOS.Data.Datasets.SecurityDataSetTableAdapters;
using DWOS.SmartCardManager;
using NLog;

namespace DWOS.UI.Utilities
{

    internal class SmartCardAuthenticationProvider : IAuthenticationProvider
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ILoginManager _loginManager;
        private bool _enabled;

        #endregion

        #region Properties

        public int? CurrentUserID { get; set; }

        #endregion

        #region Methods

        public SmartCardAuthenticationProvider(ILoginManager loginManager)
        {
            _loginManager = loginManager;
        }

        private void StartCardWatcher()
        {
            try
            {
                //start watching for changes in card reader
                CardReaderObserver.Instance.Start(false);
                CardReaderObserver.Instance.AddCardEventHandler(OnCardEvent);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error starting smart card watcher.");
            }
        }

        private void StopCardWatcher()
        {
            try
            {
                if (CardReaderObserver.Instance.ObserverState != ObserverState.Stopped)
                {
                    //stop watching for changes in card reader
                    CardReaderObserver.Instance.RemoveCardEventHandler(OnCardEvent);
                    CardReaderObserver.Instance.Stop();
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error stopping smart card watcher.");
            }
        }

        private void OnCardInserted(DWOSSmartCardService card)
        {
            try
            {
                _log.Info("Smart card inserted.");

                CurrentUserID = null;
                string userLogonID = card.ReadUserLogOnID();

                if (!String.IsNullOrWhiteSpace(userLogonID))
                {
                    using (var ta = new UsersTableAdapter())
                    {
                        var id = ta.GetUserIdByUserLogOnId(userLogonID);

                        if (id != null)
                            CurrentUserID = Convert.ToInt32(id);
                    }
                }

                if (Enabled && DWOSApp.MainForm != null)
                {
                    DWOSApp.MainForm.WaitForHandleCreation();

                    if (DWOSApp.MainForm.InvokeRequired)
                    {
                        DWOSApp.MainForm.BeginInvoke(new Action<int?>(_loginManager.DoLogin), CurrentUserID);
                    }
                    else
                    {
                        _loginManager.DoLogin(CurrentUserID);
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on smart card inserted.");
            }
        }

        private void OnCardRemoved()
        {
            _log.Info("Smart card removed.");
            CurrentUserID = null;

            //notify if they want to know
            if (Enabled)
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

        #endregion

        #region Events

        private void OnCardEvent(object sender, CardEvent e)
        {
            if (e.EventType == CardEventType.Remove)
                OnCardRemoved();
            else if (e.EventType == CardEventType.Insert)
            {
                try
                {
                    var reader = sender as ICardReader;

                    using (var connection = reader.Connect(true))
                    {
                        using (var card = new DWOSSmartCardService())
                        {
                            card.Init(connection);
                            OnCardInserted(card);
                        }
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Error reading smart card.");
                }
            }
        }

        #endregion

        #region IAuthenticationProvider Members

        public Image ProviderThumbnail
        {
            get { return Properties.Resources.SmartCard; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;

                    if (_enabled)
                        StartCardWatcher();
                    else
                        StopCardWatcher();
                }
            }
        }

        public int? GetUserID()
        {
            return CurrentUserID;
        }

        public void Dispose()
        {
            _log.Info("Disposing of SmartCardAuthenticationProvider");
            StopCardWatcher();
        }

        public LoginType LogInType { get { return LoginType.Smartcard; } }

        #endregion
    }
}
