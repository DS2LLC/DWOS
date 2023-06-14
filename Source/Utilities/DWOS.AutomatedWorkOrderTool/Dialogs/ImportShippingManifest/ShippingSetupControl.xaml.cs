using System.Windows;
using DWOS.AutomatedWorkOrderTool.ViewModel;

namespace DWOS.AutomatedWorkOrderTool.Dialogs.ImportShippingManifest
{
    /// <summary>
    /// Interaction logic for ShippingSetupControl.xaml
    /// </summary>
    public partial class ShippingSetupControl
    {
        #region Fields

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(ShippingManifestViewModel), typeof(ShippingSetupControl));

        #endregion

        #region Properties

        public ShippingManifestViewModel ViewModel
        {
            get => GetValue(ViewModelProperty) as ShippingManifestViewModel;
            set => SetValue(ViewModelProperty, value);
        }

        #endregion

        #region Methods

        public ShippingSetupControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
