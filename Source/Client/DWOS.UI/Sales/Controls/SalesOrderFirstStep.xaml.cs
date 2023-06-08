using DWOS.UI.Sales.ViewModels;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace DWOS.UI.Sales.Controls
{
    /// <summary>
    /// Interaction logic for SalesOrderFirstStep.xaml
    /// </summary>
    public partial class SalesOrderFirstStep : UserControl
    {
        #region Fields

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(SalesOrderWizardViewModel), typeof(SalesOrderFirstStep),
            new FrameworkPropertyMetadata { PropertyChangedCallback = ViewModel_PropertyChanged });

        private MediaWidgetEmbeddable _mediaWidget;

        #endregion

        #region Properties

        public SalesOrderWizardViewModel ViewModel
        {
            get => GetValue(ViewModelProperty) as SalesOrderWizardViewModel;
            set => SetValue(ViewModelProperty, value);
        }

        #endregion

        #region Methods

        public SalesOrderFirstStep()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private static void ViewModel_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (!(d is SalesOrderFirstStep instance))
                {
                    return;
                }

                if (e.OldValue is SalesOrderWizardViewModel oldValue)
                {
                    oldValue.Saved -= instance.Vm_Saved;
                }

                if (e.NewValue is SalesOrderWizardViewModel newValue)
                {
                    newValue.Saved += instance.Vm_Saved;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error changing ViewModel");
            }
        }

        private void Self_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if (vm == null)
                {
                    LogManager.GetCurrentClassLogger().Error("Unable to load first step - ViewModel is not set.");
                    return;
                }

                // Load WinForms media widget
                _mediaWidget = new MediaWidgetEmbeddable();
                _mediaWidget.Setup(new MediaWidgetEmbeddable.SetupArgs
                {
                    AllowEditing = true,
                    ScannerSettingsType = Data.ScannerSettingsType.Order,
                    ShowDefault = false,
                    MediaLinks = ViewModel.MediaLinks,
                    DocumentLinks = ViewModel.DocumentLinks
                });

                DocumentControlGrid.Children.Add(new WindowsFormsHost
                {
                    Child = _mediaWidget
                });
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error loading first step control.");
            }
        }

        private void Self_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;
                if (vm != null)
                {
                    vm.Saved -= Vm_Saved;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error unloading first step control.");
            }
        }

        private void Vm_Saved(object sender, EventArgs e)
        {
            try
            {
                // Reset media widget
                _mediaWidget?.LoadMedia(new List<MediaLink>(), new List<DocumentLink>(), -1);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling save event.");
            }
        }

        #endregion
    }
}
