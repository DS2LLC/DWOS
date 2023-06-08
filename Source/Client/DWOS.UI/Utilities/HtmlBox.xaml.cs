using System.Windows;
using System.Windows.Controls;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Interaction logic for HtmlBox.xaml
    /// </summary>
    public partial class HtmlBox : UserControl
    {
        #region Fields

        public static readonly DependencyProperty HtmlProperty = DependencyProperty.Register(
            nameof(Html), typeof(string), typeof(HtmlBox),
            new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = false,
                PropertyChangedCallback = Html_ValueChanged
            });

        #endregion

        #region Properties

        public string Html
        {
            get => GetValue(HtmlProperty) as string;
            set => SetValue(HtmlProperty, value);
        }

        #endregion

        #region Methods

        public HtmlBox()
        {
            InitializeComponent();
        }

        private void SetHtmlContent(string value)
        {
            Browser.NavigateToString(value);
        }

        #endregion

        #region Events

        private static void Html_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = (HtmlBox)d;

            owner?.SetHtmlContent(e.NewValue as string);
        }

        #endregion
    }
}
