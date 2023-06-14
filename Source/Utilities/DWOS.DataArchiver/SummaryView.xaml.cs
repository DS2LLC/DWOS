using System.Windows;
using DWOS.DataArchiver.ViewModel;

namespace DWOS.DataArchiver
{
    /// <summary>
    /// Interaction logic for SummaryView.xaml
    /// </summary>
    public partial class SummaryView
    {
        #region Methods

        public SummaryView()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void SummaryView_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue as bool?).GetValueOrDefault())
            {
                return;
            }

            if (InnerControl.DataContext is SummaryViewModel vm)
            {
                vm.LoadData();
            }
        }

        #endregion
    }
}
