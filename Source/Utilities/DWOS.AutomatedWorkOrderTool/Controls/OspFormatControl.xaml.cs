using System;
using System.ComponentModel;
using System.Windows;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using NLog;

namespace DWOS.AutomatedWorkOrderTool.Controls
{
    /// <summary>
    /// Interaction logic for OspFormatControl.xaml
    /// </summary>
    public partial class OspFormatControl
    {
        #region Fields

        public static readonly DependencyProperty OspFormatProperty = DependencyProperty.Register(
            nameof(OspFormat), typeof(OspFormatViewModel), typeof(OspFormatControl), new UIPropertyMetadata(OnOspFormatPropertyChanged));

        #endregion

        #region Properties

        public OspFormatViewModel OspFormat
        {
            get => GetValue(OspFormatProperty) as OspFormatViewModel;
            set => SetValue(OspFormatProperty, value);
        }

        #endregion

        #region Methods

        public OspFormatControl()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            if (InnerControl.DataContext is OspFormatEditorViewModel vm)
            {
                vm.LoadData(OspFormat);
            }
        }

        #endregion

        #region Events

        private static void OnOspFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is OspFormatControl ospControl))
            {
                return;
            }

            ospControl.LoadData();
        }

        private void OspFormatControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InnerControl.DataContext is OspFormatEditorViewModel vm)
                {
                    vm.SectionOrderChanged += OnSectionOrderChanged;
                    vm.CodeMapOrderChanged += OnCodeMapOrderChanged;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading format control.");
            }
        }

        private void OspFormatControl_OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InnerControl.DataContext is OspFormatEditorViewModel vm)
                {
                    vm.SectionOrderChanged -= OnSectionOrderChanged;
                    vm.CodeMapOrderChanged -= OnCodeMapOrderChanged;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error unloading format control.");
            }
        }

        private void OnSectionOrderChanged(object sender, EventArgs args)
        {
            try
            {
                SectionGrid.Items.SortDescriptions.Clear();
                SectionGrid.Items.SortDescriptions.Add(new SortDescription(nameof(OspFormatEditorViewModel.FormatSection.SectionOrder), ListSortDirection.Ascending));
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error refreshing section sort.");
            }
        }

        private void OnCodeMapOrderChanged(object sender, EventArgs eventArgs)
        {
            try
            {
                CodeMapGrid.Items.SortDescriptions.Clear();
                CodeMapGrid.Items.SortDescriptions.Add(new SortDescription(nameof(OspFormatEditorViewModel.CodeMap.Section), ListSortDirection.Ascending));
                CodeMapGrid.Items.SortDescriptions.Add(new SortDescription(nameof(OspFormatEditorViewModel.CodeMap.Code), ListSortDirection.Ascending));
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error refreshing code map sort.");
            }
        }

        #endregion
    }
}
