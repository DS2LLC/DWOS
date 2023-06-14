using DWOS.AutomatedWorkOrderTool.ViewModel;
using System.Windows;

namespace DWOS.AutomatedWorkOrderTool.Dialogs.ImportShippingManifest
{
    /// <summary>
    /// Interaction logic for ShippingImportControl.xaml
    /// </summary>
    public partial class ShippingImportControl
    {
        #region Fields

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(ShippingManifestViewModel), typeof(ShippingImportControl));

        #endregion

        #region Properties

        public ShippingManifestViewModel ViewModel
        {
            get => GetValue(ViewModelProperty) as ShippingManifestViewModel;
            set => SetValue(ViewModelProperty, value);
        }

        #endregion

        #region Methods

        public ShippingImportControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
