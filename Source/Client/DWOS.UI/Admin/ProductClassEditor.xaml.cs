using NLog;
using System;
using System.Windows;

namespace DWOS.UI.Admin
{
    /// <summary>
    /// Interaction logic for ProductClassEditor.xaml
    /// </summary>
    public partial class ProductClassEditor
    {
        #region Methods

        public ProductClassEditor()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Saved += ViewModel_Saved;
                ViewModel.LoadData();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error loading Product Class Editor.");
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Saved -= ViewModel_Saved;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error unloading Product Class Editor.");
            }
        }

        private void ViewModel_Saved(object sender, EventArgs e)
        {
            DialogResult = true;
        }

        #endregion
    }
}
