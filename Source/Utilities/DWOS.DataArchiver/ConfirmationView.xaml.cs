using System.Windows;
using DWOS.DataArchiver.ViewModel;

namespace DWOS.DataArchiver
{
    /// <summary>
    /// Interaction logic for ConfirmationView.xaml
    /// </summary>
    public partial class ConfirmationView
    {
        #region Methods

        public ConfirmationView()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private async void ConfirmationView_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue as bool?).GetValueOrDefault())
            {
                return;
            }

            if (InnerControl.DataContext is ConfirmationViewModel vm)
            {
                await vm.LoadData();
            }
        }

        #endregion
    }
}
