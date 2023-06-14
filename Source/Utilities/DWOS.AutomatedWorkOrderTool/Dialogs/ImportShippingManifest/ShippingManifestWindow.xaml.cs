using System;
using System.Windows;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using NLog;

namespace DWOS.AutomatedWorkOrderTool.Dialogs.ImportShippingManifest
{
    /// <summary>
    /// Interaction logic for ShippingManifestWindow.xaml
    /// </summary>
    public partial class ShippingManifestWindow
    {
        #region Methods

        public ShippingManifestWindow()
        {
            InitializeComponent();
        }

        public void Load(CustomerViewModel customer)
        {
            if (InnerControl.DataContext is ShippingManifestViewModel vm)
            {
                vm.Load(customer);
            }
        }

        #endregion

        #region Events

        private void ShippingManifestWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InnerControl.DataContext is ShippingManifestViewModel vm)
                {
                    vm.DialogExit += OnDialogExit;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading master list window.");
            }
        }

        private void ShippingManifestWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InnerControl.DataContext is ShippingManifestViewModel vm)
                {
                    vm.DialogExit -= OnDialogExit;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading master list window.");
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
