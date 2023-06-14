using System.Windows;

namespace DWOS.AutomatedWorkOrderTool
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow
    {
        public string ErrorMessage
        {
            get => ErrorText.Text;
            set => ErrorText.Text = value;
        }

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

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
