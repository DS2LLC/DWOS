using System.Windows;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Interaction logic for SectionHeader.xaml
    /// </summary>
    public partial class SectionHeader
    {
        #region Fields

        public static readonly DependencyProperty TitleTextProperty = DependencyProperty.Register(
            nameof(TitleText), typeof(string), typeof(SectionHeader));

        #endregion

        #region Properties
        public string TitleText
        {
            get => GetValue(TitleTextProperty) as string;
            set => SetValue(TitleTextProperty, value);
        }

        #endregion

        #region Methods

        public SectionHeader()
        {
            InitializeComponent();
        }

        #endregion
    }
}
