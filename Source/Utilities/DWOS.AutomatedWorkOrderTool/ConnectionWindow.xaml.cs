using System;
using System.Windows;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using GalaSoft.MvvmLight.Ioc;

namespace DWOS.AutomatedWorkOrderTool
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow 
    {
        public ConnectionWindow()
        {
            InitializeComponent();
        }

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
    }
}
