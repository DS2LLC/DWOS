using System;
using System.Windows;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using NLog;

namespace DWOS.AutomatedWorkOrderTool.Controls
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl
    {
        #region Methods

        public LoginControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            // Manually set view model's Pin because binding is unavailable.
            if (!(InnerControl.DataContext is LoginViewModel vm))
            {
                return;
            }

            vm.Pin = PasswordBox.Password;
        }

        private void LoginControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            PasswordBox.Focus();

            try
            {
                if (!(InnerControl.DataContext is LoginViewModel vm))
                {
                    return;
                }

                vm.LoginFailed += VmOnLoginFailed;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading login control.");
            }
        }

        private void LoginControl_OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(InnerControl.DataContext is LoginViewModel vm))
                {
                    return;
                }

                vm.LoginFailed -= VmOnLoginFailed;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error unloading login control.");
            }
        }

        private void VmOnLoginFailed(object sender, EventArgs eventArgs)
        {
            try
            {
                PasswordBox.Password = string.Empty;
                PasswordBox.Focus();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error handling login failure.");
            }
        }

        private void LoginControl_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((e.NewValue as bool?).GetValueOrDefault())
            {
                PasswordBox.Password = string.Empty;
            }
        }

        #endregion
    }
}
