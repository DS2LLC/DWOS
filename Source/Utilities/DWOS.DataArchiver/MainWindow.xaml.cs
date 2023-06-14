using System.Windows;
using CommonServiceLocator;
using DWOS.DataArchiver.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.DataArchiver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Methods

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            ServiceLocator.Current.GetInstance<IMessenger>().Register<CloseProgramMessage>(this, _ => Close());
        }

        private void MainWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ServiceLocator.Current.GetInstance<IMessenger>().Unregister(this);
        }

        #endregion
    }
}
