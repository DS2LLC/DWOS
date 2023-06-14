using System.Windows;

namespace DWOS.DataArchiver
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow
    {
        #region Properties

        public string ErrorMessage
        {
            get => ErrorText.Text;
            set => ErrorText.Text = value;
        }

        #endregion

        #region Methods

        public ErrorWindow()
        {
            InitializeComponent();
        }

        public static void ShowError(string errorMsg)
        {
            var window = new ErrorWindow
            {
                ErrorMessage = errorMsg
            };

            window.ShowDialog();
        }

        #endregion

        #region Events

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        #endregion
    }
}
