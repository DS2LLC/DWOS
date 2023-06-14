using System;
using System.Windows.Input;
using DWOS.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using NLog;
using GalaSoft.MvvmLight.CommandWpf;

namespace DWOS.DataArchiver.ViewModel
{
    public class ConnectionWindowViewModel : ViewModelBase
    {
        #region Fields

        public event EventHandler OnConnect;

        private string _serverAddress;
        private int _serverPort;

        #endregion

        #region Properties

        public string ServerAddress
        {
            get => _serverAddress;
            set => Set(nameof(ServerAddress), ref _serverAddress, value);
        }

        public int ServerPort
        {
            get => _serverPort;
            set => Set(nameof(ServerPort), ref _serverPort, value);
        }

        public ICommand Save { get; }

        #endregion

        #region Methods

        public ConnectionWindowViewModel(IMessenger messenger) :
            base(messenger)
        {
            Save = new RelayCommand(DoSave, CanConnect);

            if (IsInDesignMode)
            {
                ServerAddress = "localhost";
                ServerPort = 8080;
            }
            else
            {
                ServerAddress = UserSettings.Default.ServerAddress;
                ServerPort = UserSettings.Default.ServerPort;
            }
        }

        private void DoSave()
        {
            try
            {
                UserSettings.Default.ServerAddress = _serverAddress.Trim();
                UserSettings.Default.ServerPort = _serverPort;
                UserSettings.Default.Save();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error saving connection settings");
            }

            OnConnect?.Invoke(this, EventArgs.Empty);
        }

        private bool CanConnect() => !string.IsNullOrEmpty(_serverAddress) &&
            _serverPort > 0 &&
            _serverPort <= 65535;

        #endregion
    }
}
