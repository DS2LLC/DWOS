using System;
using System.Windows;
using DWOS.DataArchiver.ViewModel;
using GalaSoft.MvvmLight.Ioc;

namespace DWOS.DataArchiver
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow
    {
        #region Methods

        public ConnectionWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void ConnectionWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = SimpleIoc.Default.GetInstance<ConnectionWindowViewModel>();
            vm.OnConnect += VmOnOnConnect;
        }

        private void ConnectionWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            var vm = SimpleIoc.Default.GetInstance<ConnectionWindowViewModel>();
            vm.OnConnect -= VmOnOnConnect;
        }
        private void VmOnOnConnect(object sender, EventArgs eventArgs)
        {
            DialogResult = true;
        }

        #endregion
    }
}
