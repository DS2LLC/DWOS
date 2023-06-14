using DWOS.AutomatedWorkOrderTool.ViewModel;
using System.Windows;

namespace DWOS.AutomatedWorkOrderTool.Controls
{
    /// <summary>
    /// Interaction logic for UserControl.xaml
    /// </summary>
    public partial class UserInfoControl
    {
        #region Fields

        public static readonly DependencyProperty UserProperty = DependencyProperty.Register(
            nameof(User), typeof(UserViewModel), typeof(UserInfoControl));

        #endregion

        #region Properties

        public UserViewModel User
        {
            get => GetValue(UserProperty) as UserViewModel;
            set => SetValue(UserProperty, value);
        }

        #endregion

        #region Methods

        public UserInfoControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
