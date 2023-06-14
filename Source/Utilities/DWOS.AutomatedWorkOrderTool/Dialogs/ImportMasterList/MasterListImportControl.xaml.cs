using System.Windows;
using DWOS.AutomatedWorkOrderTool.ViewModel;

namespace DWOS.AutomatedWorkOrderTool.Dialogs.ImportMasterList
{
    /// <summary>
    /// Interaction logic for MasterListImportControl.xaml
    /// </summary>
    public partial class MasterListImportControl
    {
        #region Fields

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(MasterListViewModel), typeof(MasterListImportControl));

        #endregion

        #region Properties

        public MasterListViewModel ViewModel
        {
            get => GetValue(ViewModelProperty) as MasterListViewModel;
            set => SetValue(ViewModelProperty, value);
        }

        #endregion

        #region Methods

        public MasterListImportControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
