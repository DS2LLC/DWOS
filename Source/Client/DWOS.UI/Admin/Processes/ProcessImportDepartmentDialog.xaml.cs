using DWOS.UI.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DWOS.UI.Admin.Processes
{
    /// <summary>
    /// Dialog that prompts the user for a department when importing a process.
    /// </summary>
    public partial class ProcessImportDepartmentDialog
    {
        #region Properties

        public bool CreateNewDepartment =>
            ViewModel.CreateNewDepartment;

        public string SelectedDepartment =>
            ViewModel.SelectedDepartment;

        #endregion

        #region Methods

        public ProcessImportDepartmentDialog()
        {
            InitializeComponent();
            Icon = Properties.Resources.Process_32.ToWpfImage();
        }

        public void LoadData(string processName, string importedDepartment, IEnumerable<string> departments)
        {
            ViewModel.LoadData(processName, importedDepartment, departments);
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Accepted += ViewModel_Accepted;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error loading dialog.");
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Accepted -= ViewModel_Accepted;

            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error unloading dialog.");
            }
        }

        private void ViewModel_Accepted(object sender, EventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error accepting dialog.");
            }
        }

        #endregion
    }
}
