using DWOS.UI.Sales.ViewModels;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DWOS.UI.Sales.Controls
{
    /// <summary>
    /// Interaction logic for SalesOrderSecondStep.xaml
    /// </summary>
    public partial class SalesOrderSecondStep : UserControl
    {
        #region Fields

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(SalesOrderWizardViewModel), typeof(SalesOrderSecondStep),
            new FrameworkPropertyMetadata { PropertyChangedCallback = ViewModel_PropertyChanged });

        private readonly GridSettingsPersistence<XamDataGridSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<XamDataGridSettings>("SalesOrderWizard_WorkOrders", new XamDataGridSettings());


        #endregion

        #region Properties

        internal SalesOrderWizardViewModel ViewModel
        {
            get => GetValue(ViewModelProperty) as SalesOrderWizardViewModel;
            set => SetValue(ViewModelProperty, value);
        }

        #endregion

        #region Methods

        public SalesOrderSecondStep()
        {
            InitializeComponent();
        }

        private static void ViewModel_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is SalesOrderSecondStep control))
            {
                return;
            }

            if (e.OldValue is SalesOrderWizardViewModel oldValue)
            {
                oldValue.WorkOrderMoved -= control.ViewModel_OnWorkOrderMoved;
            }

            if (e.NewValue is SalesOrderWizardViewModel newValue)
            {
                newValue.WorkOrderMoved += control.ViewModel_OnWorkOrderMoved;
            }
        }

        private void ViewModel_OnWorkOrderMoved(object sender, EventArgs e)
        {
            WorkOrderGrid.Records.RefreshSort();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(WorkOrderGrid);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error loading control.");
            }
        }

        private void Control_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new XamDataGridSettings();
                settings.RetrieveSettingsFrom(WorkOrderGrid);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error unloading control.");
            }
        }


        #endregion
    }
}
