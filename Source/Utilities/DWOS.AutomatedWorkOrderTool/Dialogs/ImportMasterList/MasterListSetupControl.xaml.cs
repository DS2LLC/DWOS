using System.Windows;
using DWOS.AutomatedWorkOrderTool.ViewModel;

namespace DWOS.AutomatedWorkOrderTool.Dialogs.ImportMasterList
{
    /// <summary>
    /// Interaction logic for MasterListSetupControl.xaml
    /// </summary>
    public partial class MasterListSetupControl
    {
        #region Fields

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(MasterListViewModel), typeof(MasterListSetupControl));

        #endregion

        #region Properties

        public MasterListViewModel ViewModel
        {
            get => GetValue(ViewModelProperty) as MasterListViewModel;
            set => SetValue(ViewModelProperty, value);
        }

        #endregion

        #region Methods

        public MasterListSetupControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
