using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin.ProcessSuggestionPicker
{
    /// <summary>
    ///     Interaction logic for SelectProcessesWindow.xaml
    /// </summary>
    public partial class SelectProcessesWindow
    {
        #region  Properties

        private SelectProcessesContext ViewModel =>
            DataContext as SelectProcessesContext;

        #endregion

        #region  Methods

        public SelectProcessesWindow()
        {
            InitializeComponent();
            DataContext = new SelectProcessesContext();
            Icon = Properties.Resources.Process_32.ToWpfImage();
        }

        public List<SelectedProcess> GetProcesses()
        {
            return ViewModel?.GetProcessSelection() ?? new List<SelectedProcess>();
        }

        public void LoadData(string manufacturer)
        {
            ViewModel?.LoadData(manufacturer);
        }

        #endregion

        #region Events

        private void SelectProcessesWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if(vm != null)
                {
                    vm.DialogExit += VmOnDialogExit;
                    vm.Suggestions.ListChanged += SuggestionsOnListChanged;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading select processes window.");
            }
        }

        private void SelectProcessesWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if(vm != null)
                {
                    vm.DialogExit -= VmOnDialogExit;
                    vm.Suggestions.ListChanged -= SuggestionsOnListChanged;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error unloading select processes window.");
            }
        }

        private void VmOnDialogExit(object sender, EventArgs eventArgs)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error exiting dialog");
            }
        }

        private void SuggestionsOnListChanged(object sender, ListChangedEventArgs args)
        {
            try
            {
                FinishButton.Content = ViewModel?.Suggestions.Any(s => s.IsChecked) ?? false
                    ? "Add Processes"
                    : "Add Process";
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error handling suggestions change.");
            }
        }

        #endregion
    }
}