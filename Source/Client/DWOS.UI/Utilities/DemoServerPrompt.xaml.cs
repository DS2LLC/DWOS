using NLog;
using System;
using System.Diagnostics;
using System.Windows;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Interaction logic for DemoServerPrompt.xaml
    /// </summary>
    public partial class DemoServerPrompt
    {
        public DemoServerPrompt()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("http://www.getdwos.com/ContactUs.aspx");
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error opening Contact Us page.");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
