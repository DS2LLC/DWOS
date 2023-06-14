using System;
using System.Windows;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using NLog;

namespace DWOS.AutomatedWorkOrderTool.Dialogs.ImportMasterList
{
    /// <summary>
    /// Interaction logic for MasterListWindow.xaml
    /// </summary>
    public partial class MasterListWindow
    {
        public MasterListWindow()
        {
            InitializeComponent();
        }

        public void Load(CustomerViewModel masterListCustomer)
        {
            if (InnerControl.DataContext is MasterListViewModel vm)
            {
                vm.Load(masterListCustomer);
            }

        }

        #region Events

        private void MasterListWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InnerControl.DataContext is MasterListViewModel vm)
                {
                    vm.DialogExit += OnDialogExit;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading master list window.");
            }
        }

        private void MasterListWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InnerControl.DataContext is MasterListViewModel vm)
                {
                    vm.DialogExit -= OnDialogExit;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error unloading master list window.");
            }
        }

        private void OnDialogExit(object sender, EventArgs eventArgs)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error closing dialog.");
            }
        }

        #endregion
    }
}
