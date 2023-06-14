using System;
using System.Windows.Input;
using DWOS.AutomatedWorkOrderTool.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        #region Fields

        public event EventHandler LoginFailed;

        private string _pin;

        #endregion

        #region Properties

        public IUserManager UserManager { get; }

        public string Pin
        {
            get => _pin;
            set => Set(nameof(Pin), ref _pin, value);
        }

        public ICommand Login { get; }

        #endregion

        #region Methods

        public LoginViewModel(IUserManager userManager, IMessenger messenger)
            : base(messenger)
        {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            Login = new RelayCommand(DoLogin, CanLogin);
        }

        private void DoLogin()
        {
            if (!UserManager.LogIn(_pin))
            {
                LoginFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool CanLogin() => !string.IsNullOrEmpty(nameof(_pin));

        #endregion
    }
}
