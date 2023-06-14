using System;
using System.Windows;
using DWOS.Data.Datasets;
using NLog;

namespace DWOS.DataImporter.Controls
{
    /// <summary>
    /// Interaction logic for SelectProcessAlias.xaml
    /// </summary>
    public partial class SelectProcessAlias : Window
    {
        #region Properties

        public PartsDataset.ProcessAliasRow SelectedProcessAliasRow =>
            ViewModel.SelectedProcessAlias;

        #endregion

        #region Methods

        public SelectProcessAlias()
        {
            InitializeComponent();
        }

        internal void LoadData(string processName, PartsDataset dataset)
        {
            ViewModel.LoadData(processName, dataset);
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
