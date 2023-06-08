using System.Windows;

namespace DWOS.UI.Admin.ProcessSuggestionPicker
{
    /// <summary>
    ///     Interaction logic for SelectProcessControl.xaml
    /// </summary>
    public partial class SelectProcessControl
    {
        #region  Fields

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(SelectProcessesContext), typeof(SelectProcessControl));

        #endregion

        #region  Properties

        public SelectProcessesContext ViewModel
        {
            get => GetValue(ViewModelProperty) as SelectProcessesContext;
            set => SetValue(ViewModelProperty, value);
        }

        #endregion

        #region  Methods

        public SelectProcessControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}