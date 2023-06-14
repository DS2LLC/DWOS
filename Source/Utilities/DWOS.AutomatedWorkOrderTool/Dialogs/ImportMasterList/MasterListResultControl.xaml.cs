using System.Windows;
using DWOS.AutomatedWorkOrderTool.ViewModel;

namespace DWOS.AutomatedWorkOrderTool.Dialogs.ImportMasterList
{
    /// <summary>
    /// Interaction logic for MasterListResultControl.xaml
    /// </summary>
    public partial class MasterListResultControl
    {
        #region Fields

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(MasterListViewModel), typeof(MasterListResultControl));

        #endregion

        #region Properties

        public MasterListViewModel ViewModel
        {
            get => GetValue(ViewModelProperty) as MasterListViewModel;
            set => SetValue(ViewModelProperty, value);
        }

        #endregion

        #region Methods

        public MasterListResultControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
