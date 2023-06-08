using Infragistics.Windows.DataPresenter;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DWOS.UI.Admin
{
    /// <summary>
    /// Interaction logic for WorkDescriptionEditor.xaml
    /// </summary>
    public partial class WorkDescriptionEditor
    {
        #region Methods

        public WorkDescriptionEditor()
        {
            InitializeComponent();
        }

        public void LoadData()
        {
            ViewModel.LoadData();
        }

        #endregion

        #region Events

        private void ViewModel_Accepted(object sender, EventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error accepting editor.");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.LoadData();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error loading editor.");
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Dispose();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error unloading editor.");
            }
        }

        #endregion
    }
}
