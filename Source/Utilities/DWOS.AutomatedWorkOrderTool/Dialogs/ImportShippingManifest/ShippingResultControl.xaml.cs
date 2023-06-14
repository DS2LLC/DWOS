using DWOS.AutomatedWorkOrderTool.ViewModel;
using System.Windows;

namespace DWOS.AutomatedWorkOrderTool.Dialogs.ImportShippingManifest
{
    /// <summary>
    /// Interaction logic for ShippingResultControl.xaml
    /// </summary>
    public partial class ShippingResultControl
    {
        #region Fields

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(ShippingManifestViewModel), typeof(ShippingResultControl));

        #endregion

        #region Properties

        public ShippingManifestViewModel ViewModel
        {
            get => GetValue(ViewModelProperty) as ShippingManifestViewModel;
            set => SetValue(ViewModelProperty, value);
        }

        #endregion

        #region Methods

        public ShippingResultControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
