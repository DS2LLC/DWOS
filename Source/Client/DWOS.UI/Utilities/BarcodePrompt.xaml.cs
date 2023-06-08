using System.Windows;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Interaction logic for BarcodePrompt.xaml
    /// </summary>
    /// <remarks>
    /// Prompts the user to enter a barcode. This allows a user to scan
    /// barcodes over an unreliable remote connection.
    /// </remarks>
    public partial class BarcodePrompt
    {
        #region Properties

        /// <summary>
        /// Gets the barcode's entire content.
        /// </summary>
        public string BarcodeContent => BarcodeTextBox.Text;

        #endregion

        #region Methods

        public BarcodePrompt()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void OkButtonOnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        #endregion
    }
}
