using System;
using System.ComponentModel;
using System.Windows;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using NLog;

namespace DWOS.AutomatedWorkOrderTool.Dialogs.ImportShippingManifest
{
    /// <summary>
    /// Interaction logic for ShippingConfirmControl.xaml
    /// </summary>
    public partial class ShippingConfirmControl
    {
        #region Fields

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(ShippingManifestViewModel), typeof(ShippingConfirmControl),
            new UIPropertyMetadata(OnViewModelPropertyChanged));

        #endregion

        #region Properties

        public ShippingManifestViewModel ViewModel
        {
            get => GetValue(ViewModelProperty) as ShippingManifestViewModel;
            set => SetValue(ViewModelProperty, value);
        }

        #endregion

        #region Methods

        public ShippingConfirmControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void ShippingConfirmControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;
                if (vm != null)
                {
                    vm.OrdersLoaded += VmOnOrdersLoaded;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading shipping manifest confirmation.");
            }
        }

        private void ShippingConfirmControl_OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;
                if (vm != null)
                {
                    vm.OrdersLoaded -= VmOnOrdersLoaded;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading shipping manifest confirmation.");
            }

        }

        private void VmOnOrdersLoaded(object sender, EventArgs eventArgs)
        {
            try
            {
                OrderGrid.Items.SortDescriptions.Clear();
                OrderGrid.Items.SortDescriptions.Add(
                    new SortDescription(nameof(ShippingManifestOrder.Status), ListSortDirection.Descending));
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error handling parts loaded event.");
            }
        }

        private static void OnViewModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ShippingConfirmControl controlInstance))
            {
                return;
            }

            var oldViewModel = e.OldValue as ShippingManifestViewModel;
            var newViewModel = e.NewValue as ShippingManifestViewModel;

            // Re-register events if view model changes while control is loaded.
            // Should not happen under normal circumstances.
            if (controlInstance.IsLoaded)
            {
                if (oldViewModel != null)
                {
                    oldViewModel.OrdersLoaded -= controlInstance.VmOnOrdersLoaded;
                }

                if (newViewModel != null)
                {
                    newViewModel.OrdersLoaded += controlInstance.VmOnOrdersLoaded;
                }
            }
        }

        #endregion
    }
}
