using System;
using System.ComponentModel;
using System.Windows;
using NLog;

namespace DWOS.UI.Admin.ProcessSuggestionPicker
{
    /// <summary>
    ///     Interaction logic for SelectSuggestionsControl.xaml
    /// </summary>
    public partial class SelectSuggestionsControl
    {
        #region  Fields

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(SelectProcessesContext), typeof(SelectSuggestionsControl),
            new UIPropertyMetadata(OnViewModelPropertyChanged));

        #endregion

        #region  Properties

        public SelectProcessesContext ViewModel
        {
            get => GetValue(ViewModelProperty) as SelectProcessesContext;
            set => SetValue(ViewModelProperty, value);
        }

        #endregion

        #region  Methods

        public SelectSuggestionsControl()
        {
            InitializeComponent();
        }

        private static void OnViewModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(!(d is SelectSuggestionsControl controlInstance))
            {
                return;
            }

            var oldViewModel = e.OldValue as SelectProcessesContext;
            var newViewModel = e.NewValue as SelectProcessesContext;

            // Re-register events if view model changes while control is loaded.
            // Should not happen under normal circumstances.
            if(controlInstance.IsLoaded)
            {
                if(oldViewModel != null)
                {
                    oldViewModel.SuggestionsSortChanged -= controlInstance.VmOnSuggestionsSortChanged;
                }

                if(newViewModel != null)
                {
                    newViewModel.SuggestionsSortChanged += controlInstance.VmOnSuggestionsSortChanged;
                }
            }
        }

        #endregion

        #region Events

        private void SelectSuggestionsControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;
                if(vm != null)
                {
                    vm.SuggestionsSortChanged += VmOnSuggestionsSortChanged;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading master list confirmation.");
            }
        }

        private void SelectSuggestionsControl_OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;
                if(vm != null)
                {
                    vm.SuggestionsSortChanged -= VmOnSuggestionsSortChanged;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading master list confirmation.");
            }
        }

        private void VmOnSuggestionsSortChanged(object sender, EventArgs eventArgs)
        {
            try
            {
                SuggestionsGrid.Items.SortDescriptions.Clear();
                SuggestionsGrid.Items.SortDescriptions.Add(
                    new SortDescription(nameof(SelectProcessesContext.ProcessSuggestion.Type), ListSortDirection.Descending)); // PRE before POST
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error handling parts loaded event.");
            }
        }

        #endregion
    }
}