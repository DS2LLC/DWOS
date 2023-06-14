using System;
using System.Windows;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using NLog;
using System.ComponentModel;
using DWOS.AutomatedWorkOrderTool.Model;

namespace DWOS.AutomatedWorkOrderTool.Dialogs.ImportMasterList
{
    /// <summary>
    /// Interaction logic for MasterListConfirmControl.xaml
    /// </summary>
    public partial class MasterListConfirmControl
    {
        #region Fields

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(MasterListViewModel), typeof(MasterListConfirmControl),
            new UIPropertyMetadata(OnViewModelPropertyChanged));

        #endregion

        #region Properties

        public MasterListViewModel ViewModel
        {
            get => GetValue(ViewModelProperty) as MasterListViewModel;
            set => SetValue(ViewModelProperty, value);
        }

        #endregion

        #region Methods

        public MasterListConfirmControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void MasterListConfirmControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;
                if (vm != null)
                {
                    vm.PartsLoaded += VmOnPartsLoaded;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading master list confirmation.");
            }
        }

        private void MasterListConfirmControl_OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;
                if (vm != null)
                {
                    vm.PartsLoaded -= VmOnPartsLoaded;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading master list confirmation.");
            }
        }

        private void VmOnPartsLoaded(object sender, EventArgs eventArgs)
        {
            try
            {
                PartGrid.Items.SortDescriptions.Clear();
                PartGrid.Items.SortDescriptions.Add(
                    new SortDescription(nameof(MasterListPart.Status), ListSortDirection.Descending));
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error handling parts loaded event.");
            }
        }
        private static void OnViewModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MasterListConfirmControl controlInstance))
            {
                return;
            }

            var oldViewModel = e.OldValue as MasterListViewModel;
            var newViewModel = e.NewValue as MasterListViewModel;

            // Re-register events if view model changes while control is loaded.
            // Should not happen under normal circumstances.
            if (controlInstance.IsLoaded)
            {
                if (oldViewModel != null)
                {
                    oldViewModel.PartsLoaded -= controlInstance.VmOnPartsLoaded;
                }

                if (newViewModel != null)
                {
                    newViewModel.PartsLoaded += controlInstance.VmOnPartsLoaded;
                }
            }
        }

        #endregion
    }
}
