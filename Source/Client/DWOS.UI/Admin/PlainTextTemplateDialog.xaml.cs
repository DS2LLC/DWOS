using NLog;
using System;
using System.Windows;

namespace DWOS.UI.Admin
{
    /// <summary>
    /// Interaction logic for PlainTextTemplateDialog.xaml
    /// </summary>
    public partial class PlainTextTemplateDialog
    {
        #region Properties

        /// <summary>
        /// Gets the template content from this instance.
        /// </summary>
        public string TemplateContent =>
            ViewModel.Content;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="PlainTextTemplateDialog"/> class.
        /// </summary>
        public PlainTextTemplateDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads template data into this instance.
        /// </summary>
        /// <param name="title">The template's title.</param>
        /// <param name="content">The template's content.</param>
        /// <param name="tokens">The template's tokens.</param>
        public void Load(string title, string content, string tokens)
        {
            ViewModel.Load(title, content, tokens);
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Accepted += ViewModel_Accepted;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error loading plain text template dialog.");
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Accepted -= ViewModel_Accepted;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error unloading plain text template dialog.");
            }
        }

        private void ViewModel_Accepted(object sender, EventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error accepting plain text template dialog.");
            }
        }


        #endregion
    }
}
