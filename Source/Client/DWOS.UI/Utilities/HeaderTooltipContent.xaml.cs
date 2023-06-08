using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Interaction logic for HeaderToolTipContent.xaml
    /// </summary>
    /// <remarks>
    /// Similar to <see cref="HeaderToolTip"/> but allows you to set content
    /// instead of a content text property.
    /// </remarks>
    [ContentProperty(nameof(ToolTipContent))]
    public partial class HeaderToolTipContent
    {
        public static readonly DependencyProperty ToolTipHeaderProperty =
            DependencyProperty.Register(nameof(ToolTipHeader), typeof(string), typeof(HeaderToolTipContent));

        public static readonly DependencyProperty ToolTipContentProperty =
            DependencyProperty.Register(nameof(ToolTipContent), typeof(object), typeof(HeaderToolTipContent));

        public string ToolTipHeader
        {
            get => GetValue(ToolTipHeaderProperty) as string;
            set => SetValue(ToolTipHeaderProperty, value);
        }

        public object ToolTipContent
        {
            get => GetValue(ToolTipContentProperty);
            set => SetValue(ToolTipContentProperty, value);
        }

        public HeaderToolTipContent()
        {
            InitializeComponent();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            // Remove any margin/padding from the tooltip itself
            if (Parent is ToolTip toolTip)
            {
                toolTip.Margin = new Thickness(0);
                toolTip.Padding = new Thickness(0);
            }
        }
    }
}
