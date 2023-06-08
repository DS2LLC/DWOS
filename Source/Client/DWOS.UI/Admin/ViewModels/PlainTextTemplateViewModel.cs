using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Windows.Input;

namespace DWOS.UI.Admin.ViewModels
{
    /// <summary>
    /// View Model for <see cref="PlainTextTemplateDialog"/>.
    /// </summary>
    public class PlainTextTemplateViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Occurs when the user accepts the dialog.
        /// </summary>
        public event EventHandler Accepted;

        private string _title;
        private string _content;
        private string _tokens;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the title of the dialog.
        /// </summary>
        public string Title
        {
            get => _title;
            private set => Set(nameof(Title), ref _title, value);
        }

        /// <summary>
        /// Gets or sets the template dialog.
        /// </summary>
        public string Content
        {
            get => _content;
            set => Set(nameof(Content), ref _content, value);
        }

        /// <summary>
        /// Gets or set the list of template tokens.
        /// </summary>
        public string Tokens
        {
            get => _tokens;
            private set => Set(nameof(Tokens), ref _tokens, value);
        }

        /// <summary>
        /// Command that fires <see cref="Accepted"/>.
        /// </summary>
        public ICommand Accept { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="PlainTextTemplateViewModel"/> class.
        /// </summary>
        public PlainTextTemplateViewModel()
        {
            Accept = new RelayCommand(
                DoAccept,
                () => !string.IsNullOrEmpty(_content));
        }

        private void DoAccept()
        {
            if (string.IsNullOrEmpty(_content))
            {
                return;
            }

            Accepted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Loads template data into this instance.
        /// </summary>
        /// <param name="title">The template's title.</param>
        /// <param name="content">The template's content.</param>
        /// <param name="tokens">The template's tokens.</param>
        public void Load(string title, string content, string tokens)
        {
            Title = title;
            Content = content;
            Tokens = tokens;
        }

        #endregion
    }
}
